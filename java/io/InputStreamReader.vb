'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' An InputStreamReader is a bridge from byte streams to character streams: It
	''' reads bytes and decodes them into characters using a specified {@link
	''' java.nio.charset.Charset charset}.  The charset that it uses
	''' may be specified by name or may be given explicitly, or the platform's
	''' default charset may be accepted.
	''' 
	''' <p> Each invocation of one of an InputStreamReader's read() methods may
	''' cause one or more bytes to be read from the underlying byte-input stream.
	''' To enable the efficient conversion of bytes to characters, more bytes may
	''' be read ahead from the underlying stream than are necessary to satisfy the
	''' current read operation.
	''' 
	''' <p> For top efficiency, consider wrapping an InputStreamReader within a
	''' BufferedReader.  For example:
	''' 
	''' <pre>
	''' BufferedReader in
	'''   = new BufferedReader(new InputStreamReader(System.in));
	''' </pre>
	''' </summary>
	''' <seealso cref= BufferedReader </seealso>
	''' <seealso cref= InputStream </seealso>
	''' <seealso cref= java.nio.charset.Charset
	''' 
	''' @author      Mark Reinhold
	''' @since       JDK1.1 </seealso>

	Public Class InputStreamReader
		Inherits Reader

		Private ReadOnly sd As sun.nio.cs.StreamDecoder

		''' <summary>
		''' Creates an InputStreamReader that uses the default charset.
		''' </summary>
		''' <param name="in">   An InputStream </param>
		Public Sub New(  [in] As InputStream)
			MyBase.New([in])
			Try
				sd = sun.nio.cs.StreamDecoder.forInputStreamReader([in], Me, CStr(Nothing)) ' ## check lock object
			Catch e As UnsupportedEncodingException
				' The default encoding should always be available
				Throw New [Error](e)
			End Try
		End Sub

		''' <summary>
		''' Creates an InputStreamReader that uses the named charset.
		''' </summary>
		''' <param name="in">
		'''         An InputStream
		''' </param>
		''' <param name="charsetName">
		'''         The name of a supported
		'''         <seealso cref="java.nio.charset.Charset charset"/>
		''' </param>
		''' <exception cref="UnsupportedEncodingException">
		'''             If the named charset is not supported </exception>
		Public Sub New(  [in] As InputStream,   charsetName As String)
			MyBase.New([in])
			If charsetName Is Nothing Then Throw New NullPointerException("charsetName")
			sd = sun.nio.cs.StreamDecoder.forInputStreamReader([in], Me, charsetName)
		End Sub

		''' <summary>
		''' Creates an InputStreamReader that uses the given charset.
		''' </summary>
		''' <param name="in">       An InputStream </param>
		''' <param name="cs">       A charset
		''' 
		''' @since 1.4
		''' @spec JSR-51 </param>
		Public Sub New(  [in] As InputStream,   cs As java.nio.charset.Charset)
			MyBase.New([in])
			If cs Is Nothing Then Throw New NullPointerException("charset")
			sd = sun.nio.cs.StreamDecoder.forInputStreamReader([in], Me, cs)
		End Sub

		''' <summary>
		''' Creates an InputStreamReader that uses the given charset decoder.
		''' </summary>
		''' <param name="in">       An InputStream </param>
		''' <param name="dec">      A charset decoder
		''' 
		''' @since 1.4
		''' @spec JSR-51 </param>
		Public Sub New(  [in] As InputStream,   dec As java.nio.charset.CharsetDecoder)
			MyBase.New([in])
			If dec Is Nothing Then Throw New NullPointerException("charset decoder")
			sd = sun.nio.cs.StreamDecoder.forInputStreamReader([in], Me, dec)
		End Sub

		''' <summary>
		''' Returns the name of the character encoding being used by this stream.
		''' 
		''' <p> If the encoding has an historical name then that name is returned;
		''' otherwise the encoding's canonical name is returned.
		''' 
		''' <p> If this instance was created with the {@link
		''' #InputStreamReader(InputStream, String)} constructor then the returned
		''' name, being unique for the encoding, may differ from the name passed to
		''' the constructor. This method will return <code>null</code> if the
		''' stream has been closed.
		''' </p> </summary>
		''' <returns> The historical name of this encoding, or
		'''         <code>null</code> if the stream has been closed
		''' </returns>
		''' <seealso cref= java.nio.charset.Charset
		''' 
		''' @revised 1.4
		''' @spec JSR-51 </seealso>
		Public Overridable Property encoding As String
			Get
				Return sd.encoding
			End Get
		End Property

		''' <summary>
		''' Reads a single character.
		''' </summary>
		''' <returns> The character read, or -1 if the end of the stream has been
		'''         reached
		''' </returns>
		''' <exception cref="IOException">  If an I/O error occurs </exception>
		Public Overrides Function read() As Integer
			Return sd.read()
		End Function

		''' <summary>
		''' Reads characters into a portion of an array.
		''' </summary>
		''' <param name="cbuf">     Destination buffer </param>
		''' <param name="offset">   Offset at which to start storing characters </param>
		''' <param name="length">   Maximum number of characters to read
		''' </param>
		''' <returns>     The number of characters read, or -1 if the end of the
		'''             stream has been reached
		''' </returns>
		''' <exception cref="IOException">  If an I/O error occurs </exception>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public int read(char cbuf() , int offset, int length) throws IOException
			Return sd.read(cbuf, offset, length)

		''' <summary>
		''' Tells whether this stream is ready to be read.  An InputStreamReader is
		''' ready if its input buffer is not empty, or if bytes are available to be
		''' read from the underlying byte stream.
		''' </summary>
		''' <exception cref="IOException">  If an I/O error occurs </exception>
		public Boolean ready() throws IOException
			Return sd.ready()

		public  Sub  close() throws IOException
			sd.close()
	End Class

End Namespace