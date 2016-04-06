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
	''' Abstract class for writing filtered character streams.
	''' The abstract class <code>FilterWriter</code> itself
	''' provides default methods that pass all requests to the
	''' contained stream. Subclasses of <code>FilterWriter</code>
	''' should override some of these methods and may also
	''' provide additional methods and fields.
	''' 
	''' @author      Mark Reinhold
	''' @since       JDK1.1
	''' </summary>

	Public MustInherit Class FilterWriter
		Inherits Writer

		''' <summary>
		''' The underlying character-output stream.
		''' </summary>
		Protected Friend out As Writer

		''' <summary>
		''' Create a new filtered writer.
		''' </summary>
		''' <param name="out">  a Writer object to provide the underlying stream. </param>
		''' <exception cref="NullPointerException"> if <code>out</code> is <code>null</code> </exception>
		Protected Friend Sub New(  out As Writer)
			MyBase.New(out)
			Me.out = out
		End Sub

		''' <summary>
		''' Writes a single character.
		''' </summary>
		''' <exception cref="IOException">  If an I/O error occurs </exception>
		Public Overrides Sub write(  c As Integer)
			out.write(c)
		End Sub

		''' <summary>
		''' Writes a portion of an array of characters.
		''' </summary>
		''' <param name="cbuf">  Buffer of characters to be written </param>
		''' <param name="off">   Offset from which to start reading characters </param>
		''' <param name="len">   Number of characters to be written
		''' </param>
		''' <exception cref="IOException">  If an I/O error occurs </exception>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public  Sub  write(char cbuf() , int off, int len) throws IOException
			out.write(cbuf, off, len)

		''' <summary>
		''' Writes a portion of a string.
		''' </summary>
		''' <param name="str">  String to be written </param>
		''' <param name="off">  Offset from which to start reading characters </param>
		''' <param name="len">  Number of characters to be written
		''' </param>
		''' <exception cref="IOException">  If an I/O error occurs </exception>
		public  Sub  write(String str, Integer off, Integer len) throws IOException
			out.write(str, off, len)

		''' <summary>
		''' Flushes the stream.
		''' </summary>
		''' <exception cref="IOException">  If an I/O error occurs </exception>
		public  Sub  flush() throws IOException
			out.flush()

		public  Sub  close() throws IOException
			out.close()

	End Class

End Namespace