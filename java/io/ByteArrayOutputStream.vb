Imports System
Imports System.Runtime.CompilerServices

'
' * Copyright (c) 1994, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.io


	''' <summary>
	''' This class implements an output stream in which the data is
	''' written into a byte array. The buffer automatically grows as data
	''' is written to it.
	''' The data can be retrieved using <code>toByteArray()</code> and
	''' <code>toString()</code>.
	''' <p>
	''' Closing a <tt>ByteArrayOutputStream</tt> has no effect. The methods in
	''' this class can be called after the stream has been closed without
	''' generating an <tt>IOException</tt>.
	''' 
	''' @author  Arthur van Hoff
	''' @since   JDK1.0
	''' </summary>

	Public Class ByteArrayOutputStream
		Inherits OutputStream

		''' <summary>
		''' The buffer where data is stored.
		''' </summary>
		Protected Friend buf As SByte()

		''' <summary>
		''' The number of valid bytes in the buffer.
		''' </summary>
		Protected Friend count As Integer

		''' <summary>
		''' Creates a new byte array output stream. The buffer capacity is
		''' initially 32 bytes, though its size increases if necessary.
		''' </summary>
		Public Sub New()
			Me.New(32)
		End Sub

		''' <summary>
		''' Creates a new byte array output stream, with a buffer capacity of
		''' the specified size, in bytes.
		''' </summary>
		''' <param name="size">   the initial size. </param>
		''' <exception cref="IllegalArgumentException"> if size is negative. </exception>
		Public Sub New(ByVal size As Integer)
			If size < 0 Then Throw New IllegalArgumentException("Negative initial size: " & size)
			buf = New SByte(size - 1){}
		End Sub

		''' <summary>
		''' Increases the capacity if necessary to ensure that it can hold
		''' at least the number of elements specified by the minimum
		''' capacity argument.
		''' </summary>
		''' <param name="minCapacity"> the desired minimum capacity </param>
		''' <exception cref="OutOfMemoryError"> if {@code minCapacity < 0}.  This is
		''' interpreted as a request for the unsatisfiably large capacity
		''' {@code (long)  java.lang.[Integer].MAX_VALUE + (minCapacity -  java.lang.[Integer].MAX_VALUE)}. </exception>
		Private Sub ensureCapacity(ByVal minCapacity As Integer)
			' overflow-conscious code
			If minCapacity - buf.Length > 0 Then grow(minCapacity)
		End Sub

		''' <summary>
		''' The maximum size of array to allocate.
		''' Some VMs reserve some header words in an array.
		''' Attempts to allocate larger arrays may result in
		''' OutOfMemoryError: Requested array size exceeds VM limit
		''' </summary>
		Private Shared ReadOnly MAX_ARRAY_SIZE As Integer =  java.lang.[Integer].MAX_VALUE - 8

		''' <summary>
		''' Increases the capacity to ensure that it can hold at least the
		''' number of elements specified by the minimum capacity argument.
		''' </summary>
		''' <param name="minCapacity"> the desired minimum capacity </param>
		Private Sub grow(ByVal minCapacity As Integer)
			' overflow-conscious code
			Dim oldCapacity As Integer = buf.Length
			Dim newCapacity As Integer = oldCapacity << 1
			If newCapacity - minCapacity < 0 Then newCapacity = minCapacity
			If newCapacity - MAX_ARRAY_SIZE > 0 Then newCapacity = hugeCapacity(minCapacity)
			buf = java.util.Arrays.copyOf(buf, newCapacity)
		End Sub

		Private Shared Function hugeCapacity(ByVal minCapacity As Integer) As Integer
			If minCapacity < 0 Then ' overflow Throw New OutOfMemoryError
			Return If(minCapacity > MAX_ARRAY_SIZE,  java.lang.[Integer].Max_Value, MAX_ARRAY_SIZE)
		End Function

		''' <summary>
		''' Writes the specified byte to this byte array output stream.
		''' </summary>
		''' <param name="b">   the byte to be written. </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Sub write(ByVal b As Integer)
			ensureCapacity(count + 1)
			buf(count) = CByte(b)
			count += 1
		End Sub

        ''' <summary>
        ''' Writes <code>len</code> bytes from the specified byte array
        ''' starting at offset <code>off</code> to this byte array output stream.
        ''' </summary>
        ''' <param name="b">     the data. </param>
        ''' <param name="off">   the start offset in the data. </param>
        ''' <param name="len">   the number of bytes to write. </param>
        Public Sub write(b() As Byte, off As Integer, len As Integer)
            If (off < 0) OrElse (off > b.Length) OrElse (len < 0) OrElse ((off + len) - b.Length > 0) Then Throw New IndexOutOfBoundsException
            ensureCapacity(count + len)
            Array.Copy(b, off, buf, count, len)
            count += len
        End Sub
        ''' <summary>
        ''' Writes the complete contents of this byte array output stream to
        ''' the specified output stream argument, as if by calling the output
        ''' stream's write method using <code>out.write(buf, 0, count)</code>.
        ''' </summary>
        ''' <param name="out">   the output stream to which to write the data. </param>
        ''' <exception cref="IOException">  if an I/O error occurs. </exception>
        Public Sub writeTo(out As OutputStream) ' throws IOException
            out.write(buf, 0, count)
        End Sub
        ''' <summary>
        ''' Resets the <code>count</code> field of this byte array output
        ''' stream to zero, so that all currently accumulated output in the
        ''' output stream is discarded. The output stream can be used again,
        ''' reusing the already allocated buffer space.
        ''' </summary>
        ''' <seealso cref=     java.io.ByteArrayInputStream#count </seealso>
        Public Sub reset()
            count = 0
        End Sub
        ''' <summary>
        ''' Creates a newly allocated byte array. Its size is the current
        ''' size of this output stream and the valid contents of the buffer
        ''' have been copied into it.
        ''' </summary>
        ''' <returns>  the current contents of this output stream, as a byte array. </returns>
        ''' <seealso cref=     java.io.ByteArrayOutputStream#size() </seealso>
        Public Function toByteArray() As SByte()
            Return java.util.Arrays.copyOf(buf, count)
        End Function

        ''' <summary>
        ''' Returns the current size of the buffer.
        ''' </summary>
        ''' <returns>  the value of the <code>count</code> field, which is the number
        '''          of valid bytes in this output stream. </returns>
        ''' <seealso cref=     java.io.ByteArrayOutputStream#count </seealso>
        ''' 
        Public ReadOnly Property size() As Integer
            Get
                Return count
            End Get
        End Property
        ''' <summary>
        ''' Converts the buffer's contents into a string decoding bytes using the
        ''' platform's default character set. The length of the new <tt>String</tt>
        ''' is a function of the character set, and hence may not be equal to the
        ''' size of the buffer.
        ''' 
        ''' <p> This method always replaces malformed-input and unmappable-character
        ''' sequences with the default replacement string for the platform's
        ''' default character set. The <seealso cref="java.nio.charset.CharsetDecoder"/>
        ''' class should be used when more control over the decoding process is
        ''' required.
        ''' </summary>
        ''' <returns> String decoded from the buffer's contents.
        ''' @since  JDK1.1 </returns>
        Public Overrides Function ToString() As String
            Return New java.lang.String(buf, 0, count)
        End Function
        ''' <summary>
        ''' Converts the buffer's contents into a string by decoding the bytes using
        ''' the named <seealso cref="java.nio.charset.Charset charset"/>. The length of the new
        ''' <tt>String</tt> is a function of the charset, and hence may not be equal
        ''' to the length of the byte array.
        ''' 
        ''' <p> This method always replaces malformed-input and unmappable-character
        ''' sequences with this charset's default replacement string. The {@link
        ''' java.nio.charset.CharsetDecoder} class should be used when more control
        ''' over the decoding process is required.
        ''' </summary>
        ''' <param name="charsetName">  the name of a supported
        '''             <seealso cref="java.nio.charset.Charset charset"/> </param>
        ''' <returns>     String decoded from the buffer's contents. </returns>
        ''' <exception cref="UnsupportedEncodingException">
        '''             If the named charset is not supported
        ''' @since      JDK1.1 </exception>
        Public synchronized String ToString(String charsetName) throws UnsupportedEncodingException
			Return New String(buf, 0, count, charsetName)

		''' <summary>
		''' Creates a newly allocated string. Its size is the current size of
		''' the output stream and the valid contents of the buffer have been
		''' copied into it. Each character <i>c</i> in the resulting string is
		''' constructed from the corresponding element <i>b</i> in the byte
		''' array such that:
		''' <blockquote><pre>
		'''     c == (char)(((hibyte &amp; 0xff) &lt;&lt; 8) | (b &amp; 0xff))
		''' </pre></blockquote>
		''' </summary>
		''' @deprecated This method does not properly convert bytes into characters.
		''' As of JDK&nbsp;1.1, the preferred way to do this is via the
		''' <code>toString(String enc)</code> method, which takes an encoding-name
		''' argument, or the <code>toString()</code> method, which uses the
		''' platform's default character encoding.
		''' 
		''' <param name="hibyte">    the high byte of each resulting Unicode character. </param>
		''' <returns>     the current contents of the output stream, as a string. </returns>
		''' <seealso cref=        java.io.ByteArrayOutputStream#size() </seealso>
		''' <seealso cref=        java.io.ByteArrayOutputStream#toString(String) </seealso>
		''' <seealso cref=        java.io.ByteArrayOutputStream#toString() </seealso>
		<Obsolete("This method does not properly convert bytes into characters.")> _
		public synchronized String ToString(Integer hibyte)
			Return New String(buf, hibyte, 0, count)

		''' <summary>
		''' Closing a <tt>ByteArrayOutputStream</tt> has no effect. The methods in
		''' this class can be called after the stream has been closed without
		''' generating an <tt>IOException</tt>.
		''' </summary>
		public  Sub  close() throws IOException

	End Class

End Namespace