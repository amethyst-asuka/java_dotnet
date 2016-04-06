'
' * Copyright (c) 1996, 2005, Oracle and/or its affiliates. All rights reserved.
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
	''' Abstract class for reading filtered character streams.
	''' The abstract class <code>FilterReader</code> itself
	''' provides default methods that pass all requests to
	''' the contained stream. Subclasses of <code>FilterReader</code>
	''' should override some of these methods and may also provide
	''' additional methods and fields.
	''' 
	''' @author      Mark Reinhold
	''' @since       JDK1.1
	''' </summary>

	Public MustInherit Class FilterReader
		Inherits Reader

		''' <summary>
		''' The underlying character-input stream.
		''' </summary>
		Protected Friend [in] As Reader

		''' <summary>
		''' Creates a new filtered reader.
		''' </summary>
		''' <param name="in">  a Reader object providing the underlying stream. </param>
		''' <exception cref="NullPointerException"> if <code>in</code> is <code>null</code> </exception>
		Protected Friend Sub New(  [in] As Reader)
			MyBase.New([in])
			Me.in = [in]
		End Sub

		''' <summary>
		''' Reads a single character.
		''' </summary>
		''' <exception cref="IOException">  If an I/O error occurs </exception>
		Public Overrides Function read() As Integer
			Return [in].read()
		End Function

		''' <summary>
		''' Reads characters into a portion of an array.
		''' </summary>
		''' <exception cref="IOException">  If an I/O error occurs </exception>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public int read(char cbuf() , int off, int len) throws IOException
			Return [in].read(cbuf, off, len)

		''' <summary>
		''' Skips characters.
		''' </summary>
		''' <exception cref="IOException">  If an I/O error occurs </exception>
		public Long skip(Long n) throws IOException
			Return [in].skip(n)

		''' <summary>
		''' Tells whether this stream is ready to be read.
		''' </summary>
		''' <exception cref="IOException">  If an I/O error occurs </exception>
		public Boolean ready() throws IOException
			Return [in].ready()

		''' <summary>
		''' Tells whether this stream supports the mark() operation.
		''' </summary>
		public Boolean markSupported()
			Return [in].markSupported()

		''' <summary>
		''' Marks the present position in the stream.
		''' </summary>
		''' <exception cref="IOException">  If an I/O error occurs </exception>
		public  Sub  mark(Integer readAheadLimit) throws IOException
			[in].mark(readAheadLimit)

		''' <summary>
		''' Resets the stream.
		''' </summary>
		''' <exception cref="IOException">  If an I/O error occurs </exception>
		public  Sub  reset() throws IOException
			[in].reset()

		public  Sub  close() throws IOException
			[in].close()

	End Class

End Namespace