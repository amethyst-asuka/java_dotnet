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
	''' Convenience class for writing character files.  The constructors of this
	''' class assume that the default character encoding and the default byte-buffer
	''' size are acceptable.  To specify these values yourself, construct an
	''' OutputStreamWriter on a FileOutputStream.
	''' 
	''' <p>Whether or not a file is available or may be created depends upon the
	''' underlying platform.  Some platforms, in particular, allow a file to be
	''' opened for writing by only one <tt>FileWriter</tt> (or other file-writing
	''' object) at a time.  In such situations the constructors in this class
	''' will fail if the file involved is already open.
	''' 
	''' <p><code>FileWriter</code> is meant for writing streams of characters.
	''' For writing streams of raw bytes, consider using a
	''' <code>FileOutputStream</code>.
	''' </summary>
	''' <seealso cref= OutputStreamWriter </seealso>
	''' <seealso cref= FileOutputStream
	''' 
	''' @author      Mark Reinhold
	''' @since       JDK1.1 </seealso>

	Public Class FileWriter
		Inherits OutputStreamWriter

		''' <summary>
		''' Constructs a FileWriter object given a file name.
		''' </summary>
		''' <param name="fileName">  String The system-dependent filename. </param>
		''' <exception cref="IOException">  if the named file exists but is a directory rather
		'''                  than a regular file, does not exist but cannot be
		'''                  created, or cannot be opened for any other reason </exception>
		Public Sub New(ByVal fileName As String)
			MyBase.New(New FileOutputStream(fileName))
		End Sub

		''' <summary>
		''' Constructs a FileWriter object given a file name with a boolean
		''' indicating whether or not to append the data written.
		''' </summary>
		''' <param name="fileName">  String The system-dependent filename. </param>
		''' <param name="append">    boolean if <code>true</code>, then data will be written
		'''                  to the end of the file rather than the beginning. </param>
		''' <exception cref="IOException">  if the named file exists but is a directory rather
		'''                  than a regular file, does not exist but cannot be
		'''                  created, or cannot be opened for any other reason </exception>
		Public Sub New(ByVal fileName As String, ByVal append As Boolean)
			MyBase.New(New FileOutputStream(fileName, append))
		End Sub

		''' <summary>
		''' Constructs a FileWriter object given a File object.
		''' </summary>
		''' <param name="file">  a File object to write to. </param>
		''' <exception cref="IOException">  if the file exists but is a directory rather than
		'''                  a regular file, does not exist but cannot be created,
		'''                  or cannot be opened for any other reason </exception>
		Public Sub New(ByVal file_Renamed As File)
			MyBase.New(New FileOutputStream(file_Renamed))
		End Sub

		''' <summary>
		''' Constructs a FileWriter object given a File object. If the second
		''' argument is <code>true</code>, then bytes will be written to the end
		''' of the file rather than the beginning.
		''' </summary>
		''' <param name="file">  a File object to write to </param>
		''' <param name="append">    if <code>true</code>, then bytes will be written
		'''                      to the end of the file rather than the beginning </param>
		''' <exception cref="IOException">  if the file exists but is a directory rather than
		'''                  a regular file, does not exist but cannot be created,
		'''                  or cannot be opened for any other reason
		''' @since 1.4 </exception>
		Public Sub New(ByVal file_Renamed As File, ByVal append As Boolean)
			MyBase.New(New FileOutputStream(file_Renamed, append))
		End Sub

		''' <summary>
		''' Constructs a FileWriter object associated with a file descriptor.
		''' </summary>
		''' <param name="fd">  FileDescriptor object to write to. </param>
		Public Sub New(ByVal fd As FileDescriptor)
			MyBase.New(New FileOutputStream(fd))
		End Sub

	End Class

End Namespace