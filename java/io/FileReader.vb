'
' * Copyright (c) 1996, 2001, Oracle and/or its affiliates. All rights reserved.
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
	''' Convenience class for reading character files.  The constructors of this
	''' class assume that the default character encoding and the default byte-buffer
	''' size are appropriate.  To specify these values yourself, construct an
	''' InputStreamReader on a FileInputStream.
	''' 
	''' <p><code>FileReader</code> is meant for reading streams of characters.
	''' For reading streams of raw bytes, consider using a
	''' <code>FileInputStream</code>.
	''' </summary>
	''' <seealso cref= InputStreamReader </seealso>
	''' <seealso cref= FileInputStream
	''' 
	''' @author      Mark Reinhold
	''' @since       JDK1.1 </seealso>
	Public Class FileReader
		Inherits InputStreamReader

	   ''' <summary>
	   ''' Creates a new <tt>FileReader</tt>, given the name of the
	   ''' file to read from.
	   ''' </summary>
	   ''' <param name="fileName"> the name of the file to read from </param>
	   ''' <exception cref="FileNotFoundException">  if the named file does not exist,
	   '''                   is a directory rather than a regular file,
	   '''                   or for some other reason cannot be opened for
	   '''                   reading. </exception>
		Public Sub New(ByVal fileName As String)
			MyBase.New(New FileInputStream(fileName))
		End Sub

	   ''' <summary>
	   ''' Creates a new <tt>FileReader</tt>, given the <tt>File</tt>
	   ''' to read from.
	   ''' </summary>
	   ''' <param name="file"> the <tt>File</tt> to read from </param>
	   ''' <exception cref="FileNotFoundException">  if the file does not exist,
	   '''                   is a directory rather than a regular file,
	   '''                   or for some other reason cannot be opened for
	   '''                   reading. </exception>
		Public Sub New(ByVal file_Renamed As File)
			MyBase.New(New FileInputStream(file_Renamed))
		End Sub

	   ''' <summary>
	   ''' Creates a new <tt>FileReader</tt>, given the
	   ''' <tt>FileDescriptor</tt> to read from.
	   ''' </summary>
	   ''' <param name="fd"> the FileDescriptor to read from </param>
		Public Sub New(ByVal fd As FileDescriptor)
			MyBase.New(New FileInputStream(fd))
		End Sub

	End Class

End Namespace