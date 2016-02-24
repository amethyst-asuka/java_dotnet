Imports System.Runtime.InteropServices

'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
' * ORACLE PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' 

Namespace java.nio



	''' <summary>
	''' A direct byte buffer whose content is a memory-mapped region of a file.
	''' 
	''' <p> Mapped byte buffers are created via the {@link
	''' java.nio.channels.FileChannel#map FileChannel.map} method.  This class
	''' extends the <seealso cref="ByteBuffer"/> class with operations that are specific to
	''' memory-mapped file regions.
	''' 
	''' <p> A mapped byte buffer and the file mapping that it represents remain
	''' valid until the buffer itself is garbage-collected.
	''' 
	''' <p> The content of a mapped byte buffer can change at any time, for example
	''' if the content of the corresponding region of the mapped file is changed by
	''' this program or another.  Whether or not such changes occur, and when they
	''' occur, is operating-system dependent and therefore unspecified.
	''' 
	''' <a name="inaccess"></a><p> All or part of a mapped byte buffer may become
	''' inaccessible at any time, for example if the mapped file is truncated.  An
	''' attempt to access an inaccessible region of a mapped byte buffer will not
	''' change the buffer's content and will cause an unspecified exception to be
	''' thrown either at the time of the access or at some later time.  It is
	''' therefore strongly recommended that appropriate precautions be taken to
	''' avoid the manipulation of a mapped file by this program, or by a
	''' concurrently running program, except to read or write the file's content.
	''' 
	''' <p> Mapped byte buffers otherwise behave no differently than ordinary direct
	''' byte buffers. </p>
	''' 
	''' 
	''' @author Mark Reinhold
	''' @author JSR-51 Expert Group
	''' @since 1.4
	''' </summary>

	Public MustInherit Class MappedByteBuffer
		Inherits ByteBuffer

		' This is a little bit backwards: By rights MappedByteBuffer should be a
		' subclass of DirectByteBuffer, but to keep the spec clear and simple, and
		' for optimization purposes, it's easier to do it the other way around.
		' This works because DirectByteBuffer is a package-private class.

		' For mapped buffers, a FileDescriptor that may be used for mapping
		' operations if valid; null if the buffer is not mapped.
		Private ReadOnly fd As java.io.FileDescriptor

		' This should only be invoked by the DirectByteBuffer constructors
		'
		Friend Sub New(ByVal mark As Integer, ByVal pos As Integer, ByVal lim As Integer, ByVal cap As Integer, ByVal fd As java.io.FileDescriptor) ' package-private
			MyBase.New(mark, pos, lim, cap)
			Me.fd = fd
		End Sub

		Friend Sub New(ByVal mark As Integer, ByVal pos As Integer, ByVal lim As Integer, ByVal cap As Integer) ' package-private
			MyBase.New(mark, pos, lim, cap)
			Me.fd = Nothing
		End Sub

		Private Sub checkMapped()
			If fd Is Nothing Then Throw New UnsupportedOperationException
		End Sub

		' Returns the distance (in bytes) of the buffer from the page aligned address
		' of the mapping. Computed each time to avoid storing in every direct buffer.
		Private Function mappingOffset() As Long
			Dim ps As Integer = Bits.pageSize()
			Dim offset As Long = address Mod ps
			Return If(offset >= 0, offset, (ps + offset))
		End Function

		Private Function mappingAddress(ByVal mappingOffset As Long) As Long
			Return address - mappingOffset
		End Function

		Private Function mappingLength(ByVal mappingOffset As Long) As Long
			Return CLng(capacity()) + mappingOffset
		End Function

		''' <summary>
		''' Tells whether or not this buffer's content is resident in physical
		''' memory.
		''' 
		''' <p> A return value of <tt>true</tt> implies that it is highly likely
		''' that all of the data in this buffer is resident in physical memory and
		''' may therefore be accessed without incurring any virtual-memory page
		''' faults or I/O operations.  A return value of <tt>false</tt> does not
		''' necessarily imply that the buffer's content is not resident in physical
		''' memory.
		''' 
		''' <p> The returned value is a hint, rather than a guarantee, because the
		''' underlying operating system may have paged out some of the buffer's data
		''' by the time that an invocation of this method returns.  </p>
		''' </summary>
		''' <returns>  <tt>true</tt> if it is likely that this buffer's content
		'''          is resident in physical memory </returns>
		Public Property loaded As Boolean
			Get
				checkMapped()
				If (address = 0) OrElse (capacity() = 0) Then Return True
				Dim offset As Long = mappingOffset()
				Dim length As Long = mappingLength(offset)
				Return isLoaded0(mappingAddress(offset), length, Bits.pageCount(length))
			End Get
		End Property

		' not used, but a potential target for a store, see load() for details.
		Private Shared unused As SByte

		''' <summary>
		''' Loads this buffer's content into physical memory.
		''' 
		''' <p> This method makes a best effort to ensure that, when it returns,
		''' this buffer's content is resident in physical memory.  Invoking this
		''' method may cause some number of page faults and I/O operations to
		''' occur. </p>
		''' </summary>
		''' <returns>  This buffer </returns>
		Public Function load() As MappedByteBuffer
			checkMapped()
			If (address = 0) OrElse (capacity() = 0) Then Return Me
			Dim offset As Long = mappingOffset()
			Dim length As Long = mappingLength(offset)
			load0(mappingAddress(offset), length)

			' Read a byte from each page to bring it into memory. A checksum
			' is computed as we go along to prevent the compiler from otherwise
			' considering the loop as dead code.
			Dim unsafe As sun.misc.Unsafe = sun.misc.Unsafe.unsafe
			Dim ps As Integer = Bits.pageSize()
			Dim count As Integer = Bits.pageCount(length)
			Dim a As Long = mappingAddress(offset)
			Dim x As SByte = 0
			For i As Integer = 0 To count - 1
				x = x Xor unsafe.getByte(a)
				a += ps
			Next i
			If unused <> 0 Then unused = x

			Return Me
		End Function

		''' <summary>
		''' Forces any changes made to this buffer's content to be written to the
		''' storage device containing the mapped file.
		''' 
		''' <p> If the file mapped into this buffer resides on a local storage
		''' device then when this method returns it is guaranteed that all changes
		''' made to the buffer since it was created, or since this method was last
		''' invoked, will have been written to that device.
		''' 
		''' <p> If the file does not reside on a local device then no such guarantee
		''' is made.
		''' 
		''' <p> If this buffer was not mapped in read/write mode ({@link
		''' java.nio.channels.FileChannel.MapMode#READ_WRITE}) then invoking this
		''' method has no effect. </p>
		''' </summary>
		''' <returns>  This buffer </returns>
		Public Function force() As MappedByteBuffer
			checkMapped()
			If (address <> 0) AndAlso (capacity() <> 0) Then
				Dim offset As Long = mappingOffset()
				force0(fd, mappingAddress(offset), mappingLength(offset))
			End If
			Return Me
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Function isLoaded0(ByVal address As Long, ByVal length As Long, ByVal pageCount As Integer) As Boolean
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Sub load0(ByVal address As Long, ByVal length As Long)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Sub force0(ByVal fd As java.io.FileDescriptor, ByVal address As Long, ByVal length As Long)
		End Sub
	End Class

End Namespace