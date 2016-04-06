'
' * Copyright (c) 1996, 2004, Oracle and/or its affiliates. All rights reserved.
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
	''' A character stream that collects its output in a string buffer, which can
	''' then be used to construct a string.
	''' <p>
	''' Closing a <tt>StringWriter</tt> has no effect. The methods in this class
	''' can be called after the stream has been closed without generating an
	''' <tt>IOException</tt>.
	''' 
	''' @author      Mark Reinhold
	''' @since       JDK1.1
	''' </summary>

	Public Class StringWriter
		Inherits Writer

		Private buf As StringBuffer

		''' <summary>
		''' Create a new string writer using the default initial string-buffer
		''' size.
		''' </summary>
		Public Sub New()
			buf = New StringBuffer
			lock = buf
		End Sub

		''' <summary>
		''' Create a new string writer using the specified initial string-buffer
		''' size.
		''' </summary>
		''' <param name="initialSize">
		'''        The number of <tt>char</tt> values that will fit into this buffer
		'''        before it is automatically expanded
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''         If <tt>initialSize</tt> is negative </exception>
		Public Sub New(  initialSize As Integer)
			If initialSize < 0 Then Throw New IllegalArgumentException("Negative buffer size")
			buf = New StringBuffer(initialSize)
			lock = buf
		End Sub

		''' <summary>
		''' Write a single character.
		''' </summary>
		Public Overrides Sub write(  c As Integer)
			buf.append(ChrW(c))
		End Sub

		''' <summary>
		''' Write a portion of an array of characters.
		''' </summary>
		''' <param name="cbuf">  Array of characters </param>
		''' <param name="off">   Offset from which to start writing characters </param>
		''' <param name="len">   Number of characters to write </param>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public  Sub  write(char cbuf() , int off, int len)
			If (off < 0) OrElse (off > cbuf.length) OrElse (len < 0) OrElse ((off + len) > cbuf.length) OrElse ((off + len) < 0) Then
				Throw New IndexOutOfBoundsException
			ElseIf len = 0 Then
				Return
			End If
			buf.append(cbuf, off, len)

		''' <summary>
		''' Write a string.
		''' </summary>
		public  Sub  write(String str)
			buf.append(str)

		''' <summary>
		''' Write a portion of a string.
		''' </summary>
		''' <param name="str">  String to be written </param>
		''' <param name="off">  Offset from which to start writing characters </param>
		''' <param name="len">  Number of characters to write </param>
		public  Sub  write(String str, Integer off, Integer len)
			buf.append(str.Substring(off, len))

		''' <summary>
		''' Appends the specified character sequence to this writer.
		''' 
		''' <p> An invocation of this method of the form <tt>out.append(csq)</tt>
		''' behaves in exactly the same way as the invocation
		''' 
		''' <pre>
		'''     out.write(csq.toString()) </pre>
		''' 
		''' <p> Depending on the specification of <tt>toString</tt> for the
		''' character sequence <tt>csq</tt>, the entire sequence may not be
		''' appended. For instance, invoking the <tt>toString</tt> method of a
		''' character buffer will return a subsequence whose content depends upon
		''' the buffer's position and limit.
		''' </summary>
		''' <param name="csq">
		'''         The character sequence to append.  If <tt>csq</tt> is
		'''         <tt>null</tt>, then the four characters <tt>"null"</tt> are
		'''         appended to this writer.
		''' </param>
		''' <returns>  This writer
		''' 
		''' @since  1.5 </returns>
		public StringWriter append(CharSequence csq)
			If csq Is Nothing Then
				write("null")
			Else
				write(csq.ToString())
			End If
			Return Me

		''' <summary>
		''' Appends a subsequence of the specified character sequence to this writer.
		''' 
		''' <p> An invocation of this method of the form <tt>out.append(csq, start,
		''' end)</tt> when <tt>csq</tt> is not <tt>null</tt>, behaves in
		''' exactly the same way as the invocation
		''' 
		''' <pre>
		'''     out.write(csq.subSequence(start, end).toString()) </pre>
		''' </summary>
		''' <param name="csq">
		'''         The character sequence from which a subsequence will be
		'''         appended.  If <tt>csq</tt> is <tt>null</tt>, then characters
		'''         will be appended as if <tt>csq</tt> contained the four
		'''         characters <tt>"null"</tt>.
		''' </param>
		''' <param name="start">
		'''         The index of the first character in the subsequence
		''' </param>
		''' <param name="end">
		'''         The index of the character following the last character in the
		'''         subsequence
		''' </param>
		''' <returns>  This writer
		''' </returns>
		''' <exception cref="IndexOutOfBoundsException">
		'''          If <tt>start</tt> or <tt>end</tt> are negative, <tt>start</tt>
		'''          is greater than <tt>end</tt>, or <tt>end</tt> is greater than
		'''          <tt>csq.length()</tt>
		''' 
		''' @since  1.5 </exception>
		public StringWriter append(CharSequence csq, Integer start, Integer end)
			Dim cs As CharSequence = (If(csq Is Nothing, "null", csq))
			write(cs.subSequence(start, end).ToString())
			Return Me

		''' <summary>
		''' Appends the specified character to this writer.
		''' 
		''' <p> An invocation of this method of the form <tt>out.append(c)</tt>
		''' behaves in exactly the same way as the invocation
		''' 
		''' <pre>
		'''     out.write(c) </pre>
		''' </summary>
		''' <param name="c">
		'''         The 16-bit character to append
		''' </param>
		''' <returns>  This writer
		''' 
		''' @since 1.5 </returns>
		public StringWriter append(Char c)
			write(c)
			Return Me

		''' <summary>
		''' Return the buffer's current value as a string.
		''' </summary>
		public String ToString()
			Return buf.ToString()

		''' <summary>
		''' Return the string buffer itself.
		''' </summary>
		''' <returns> StringBuffer holding the current buffer value. </returns>
		public StringBuffer buffer
			Return buf

		''' <summary>
		''' Flush the stream.
		''' </summary>
		public  Sub  flush()

		''' <summary>
		''' Closing a <tt>StringWriter</tt> has no effect. The methods in this
		''' class can be called after the stream has been closed without generating
		''' an <tt>IOException</tt>.
		''' </summary>
		public  Sub  close() throws IOException

	End Class

End Namespace