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
	''' An OutputStreamWriter is a bridge from character streams to byte streams:
	''' Characters written to it are encoded into bytes using a specified {@link
	''' java.nio.charset.Charset charset}.  The charset that it uses
	''' may be specified by name or may be given explicitly, or the platform's
	''' default charset may be accepted.
	''' 
	''' <p> Each invocation of a write() method causes the encoding converter to be
	''' invoked on the given character(s).  The resulting bytes are accumulated in a
	''' buffer before being written to the underlying output stream.  The size of
	''' this buffer may be specified, but by default it is large enough for most
	''' purposes.  Note that the characters passed to the write() methods are not
	''' buffered.
	''' 
	''' <p> For top efficiency, consider wrapping an OutputStreamWriter within a
	''' BufferedWriter so as to avoid frequent converter invocations.  For example:
	''' 
	''' <pre>
	''' Writer out
	'''   = new BufferedWriter(new OutputStreamWriter(System.out));
	''' </pre>
	''' 
	''' <p> A <i>surrogate pair</i> is a character represented by a sequence of two
	''' <tt>char</tt> values: A <i>high</i> surrogate in the range '&#92;uD800' to
	''' '&#92;uDBFF' followed by a <i>low</i> surrogate in the range '&#92;uDC00' to
	''' '&#92;uDFFF'.
	''' 
	''' <p> A <i>malformed surrogate element</i> is a high surrogate that is not
	''' followed by a low surrogate or a low surrogate that is not preceded by a
	''' high surrogate.
	''' 
	''' <p> This class always replaces malformed surrogate elements and unmappable
	''' character sequences with the charset's default <i>substitution sequence</i>.
	''' The <seealso cref="java.nio.charset.CharsetEncoder"/> class should be used when more
	''' control over the encoding process is required.
	''' </summary>
	''' <seealso cref= BufferedWriter </seealso>
	''' <seealso cref= OutputStream </seealso>
	''' <seealso cref= java.nio.charset.Charset
	''' 
	''' @author      Mark Reinhold
	''' @since       JDK1.1 </seealso>

	Public Class OutputStreamWriter
		Inherits Writer

		Private ReadOnly se As sun.nio.cs.StreamEncoder

		''' <summary>
		''' Creates an OutputStreamWriter that uses the named charset.
		''' </summary>
		''' <param name="out">
		'''         An OutputStream
		''' </param>
		''' <param name="charsetName">
		'''         The name of a supported
		'''         <seealso cref="java.nio.charset.Charset charset"/>
		''' </param>
		''' <exception cref="UnsupportedEncodingException">
		'''             If the named encoding is not supported </exception>
		Public Sub New(ByVal out As OutputStream, ByVal charsetName As String)
			MyBase.New(out)
			If charsetName Is Nothing Then Throw New NullPointerException("charsetName")
			se = sun.nio.cs.StreamEncoder.forOutputStreamWriter(out, Me, charsetName)
		End Sub

		''' <summary>
		''' Creates an OutputStreamWriter that uses the default character encoding.
		''' </summary>
		''' <param name="out">  An OutputStream </param>
		Public Sub New(ByVal out As OutputStream)
			MyBase.New(out)
			Try
				se = sun.nio.cs.StreamEncoder.forOutputStreamWriter(out, Me, CStr(Nothing))
			Catch e As UnsupportedEncodingException
				Throw New [Error](e)
			End Try
		End Sub

		''' <summary>
		''' Creates an OutputStreamWriter that uses the given charset.
		''' </summary>
		''' <param name="out">
		'''         An OutputStream
		''' </param>
		''' <param name="cs">
		'''         A charset
		''' 
		''' @since 1.4
		''' @spec JSR-51 </param>
		Public Sub New(ByVal out As OutputStream, ByVal cs As java.nio.charset.Charset)
			MyBase.New(out)
			If cs Is Nothing Then Throw New NullPointerException("charset")
			se = sun.nio.cs.StreamEncoder.forOutputStreamWriter(out, Me, cs)
		End Sub

		''' <summary>
		''' Creates an OutputStreamWriter that uses the given charset encoder.
		''' </summary>
		''' <param name="out">
		'''         An OutputStream
		''' </param>
		''' <param name="enc">
		'''         A charset encoder
		''' 
		''' @since 1.4
		''' @spec JSR-51 </param>
		Public Sub New(ByVal out As OutputStream, ByVal enc As java.nio.charset.CharsetEncoder)
			MyBase.New(out)
			If enc Is Nothing Then Throw New NullPointerException("charset encoder")
			se = sun.nio.cs.StreamEncoder.forOutputStreamWriter(out, Me, enc)
		End Sub

		''' <summary>
		''' Returns the name of the character encoding being used by this stream.
		''' 
		''' <p> If the encoding has an historical name then that name is returned;
		''' otherwise the encoding's canonical name is returned.
		''' 
		''' <p> If this instance was created with the {@link
		''' #OutputStreamWriter(OutputStream, String)} constructor then the returned
		''' name, being unique for the encoding, may differ from the name passed to
		''' the constructor.  This method may return <tt>null</tt> if the stream has
		''' been closed. </p>
		''' </summary>
		''' <returns> The historical name of this encoding, or possibly
		'''         <code>null</code> if the stream has been closed
		''' </returns>
		''' <seealso cref= java.nio.charset.Charset
		''' 
		''' @revised 1.4
		''' @spec JSR-51 </seealso>
		Public Overridable Property encoding As String
			Get
				Return se.encoding
			End Get
		End Property

		''' <summary>
		''' Flushes the output buffer to the underlying byte stream, without flushing
		''' the byte stream itself.  This method is non-private only so that it may
		''' be invoked by PrintStream.
		''' </summary>
		Friend Overridable Sub flushBuffer()
			se.flushBuffer()
		End Sub

		''' <summary>
		''' Writes a single character.
		''' </summary>
		''' <exception cref="IOException">  If an I/O error occurs </exception>
		Public Overrides Sub write(ByVal c As Integer)
			se.write(c)
		End Sub

		''' <summary>
		''' Writes a portion of an array of characters.
		''' </summary>
		''' <param name="cbuf">  Buffer of characters </param>
		''' <param name="off">   Offset from which to start writing characters </param>
		''' <param name="len">   Number of characters to write
		''' </param>
		''' <exception cref="IOException">  If an I/O error occurs </exception>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public  Sub  write(char cbuf() , int off, int len) throws IOException
			se.write(cbuf, off, len)

		''' <summary>
		''' Writes a portion of a string.
		''' </summary>
		''' <param name="str">  A String </param>
		''' <param name="off">  Offset from which to start writing characters </param>
		''' <param name="len">  Number of characters to write
		''' </param>
		''' <exception cref="IOException">  If an I/O error occurs </exception>
		public  Sub  write(String str, Integer off, Integer len) throws IOException
			se.write(str, off, len)

		''' <summary>
		''' Flushes the stream.
		''' </summary>
		''' <exception cref="IOException">  If an I/O error occurs </exception>
		public  Sub  flush() throws IOException
			se.flush()

		public  Sub  close() throws IOException
			se.close()
	End Class

End Namespace