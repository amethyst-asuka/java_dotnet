'
' * Copyright (c) 2000, 2007, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.imageio.stream


	''' <summary>
	''' An implementation of <code>ImageOutputStream</code> that writes its
	''' output directly to a <code>File</code> or
	''' <code>RandomAccessFile</code>.
	''' 
	''' </summary>
	Public Class FileImageOutputStream
		Inherits ImageOutputStreamImpl

		Private raf As java.io.RandomAccessFile

		''' <summary>
		''' The referent to be registered with the Disposer. </summary>
		Private ReadOnly disposerReferent As Object

		''' <summary>
		''' The DisposerRecord that closes the underlying RandomAccessFile. </summary>
		Private ReadOnly disposerRecord As com.sun.imageio.stream.CloseableDisposerRecord

		''' <summary>
		''' Constructs a <code>FileImageOutputStream</code> that will write
		''' to a given <code>File</code>.
		''' </summary>
		''' <param name="f"> a <code>File</code> to write to.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>f</code> is
		''' <code>null</code>. </exception>
		''' <exception cref="SecurityException"> if a security manager exists
		''' and does not allow write access to the file. </exception>
		''' <exception cref="FileNotFoundException"> if <code>f</code> does not denote
		''' a regular file or it cannot be opened for reading and writing for any
		''' other reason. </exception>
		''' <exception cref="IOException"> if an I/O error occurs. </exception>
		Public Sub New(ByVal f As java.io.File)
			Me.New(If(f Is Nothing, Nothing, New java.io.RandomAccessFile(f, "rw")))
		End Sub

		''' <summary>
		''' Constructs a <code>FileImageOutputStream</code> that will write
		''' to a given <code>RandomAccessFile</code>.
		''' </summary>
		''' <param name="raf"> a <code>RandomAccessFile</code> to write to.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>raf</code> is
		''' <code>null</code>. </exception>
		Public Sub New(ByVal raf As java.io.RandomAccessFile)
			If raf Is Nothing Then Throw New System.ArgumentException("raf == null!")
			Me.raf = raf

			disposerRecord = New com.sun.imageio.stream.CloseableDisposerRecord(raf)
			If Me.GetType() = GetType(FileImageOutputStream) Then
				disposerReferent = New Object
				sun.java2d.Disposer.addRecord(disposerReferent, disposerRecord)
			Else
				disposerReferent = New com.sun.imageio.stream.StreamFinalizer(Me)
			End If
		End Sub

		Public Overrides Function read() As Integer
			checkClosed()
			bitOffset = 0
			Dim val As Integer = raf.read()
			If val <> -1 Then streamPos += 1
			Return val
		End Function

		Public Overrides Function read(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer) As Integer
			checkClosed()
			bitOffset = 0
			Dim nbytes As Integer = raf.read(b, [off], len)
			If nbytes <> -1 Then streamPos += nbytes
			Return nbytes
		End Function

		Public Overrides Sub write(ByVal b As Integer)
			flushBits() ' this will call checkClosed() for us
			raf.write(b)
			streamPos += 1
		End Sub

		Public Overrides Sub write(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer)
			flushBits() ' this will call checkClosed() for us
			raf.write(b, [off], len)
			streamPos += len
		End Sub

		Public Overrides Function length() As Long
			Try
				checkClosed()
				Return raf.length()
			Catch e As java.io.IOException
				Return -1L
			End Try
		End Function

		''' <summary>
		''' Sets the current stream position and resets the bit offset to
		''' 0.  It is legal to seeking past the end of the file; an
		''' <code>EOFException</code> will be thrown only if a read is
		''' performed.  The file length will not be increased until a write
		''' is performed.
		''' </summary>
		''' <exception cref="IndexOutOfBoundsException"> if <code>pos</code> is smaller
		''' than the flushed position. </exception>
		''' <exception cref="IOException"> if any other I/O error occurs. </exception>
		Public Overrides Sub seek(ByVal pos As Long)
			checkClosed()
			If pos < flushedPos Then Throw New System.IndexOutOfRangeException("pos < flushedPos!")
			bitOffset = 0
			raf.seek(pos)
			streamPos = raf.filePointer
		End Sub

		Public Overrides Sub close()
			MyBase.close()
			disposerRecord.Dispose() ' this closes the RandomAccessFile
			raf = Nothing
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Overrides Sub Finalize()
			' Empty finalizer: for performance reasons we instead use the
			' Disposer mechanism for ensuring that the underlying
			' RandomAccessFile is closed prior to garbage collection
		End Sub
	End Class

End Namespace