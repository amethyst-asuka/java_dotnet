Imports System.Runtime.InteropServices

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
	''' A file output stream is an output stream for writing data to a
	''' <code>File</code> or to a <code>FileDescriptor</code>. Whether or not
	''' a file is available or may be created depends upon the underlying
	''' platform.  Some platforms, in particular, allow a file to be opened
	''' for writing by only one <tt>FileOutputStream</tt> (or other
	''' file-writing object) at a time.  In such situations the constructors in
	''' this class will fail if the file involved is already open.
	''' 
	''' <p><code>FileOutputStream</code> is meant for writing streams of raw bytes
	''' such as image data. For writing streams of characters, consider using
	''' <code>FileWriter</code>.
	''' 
	''' @author  Arthur van Hoff </summary>
	''' <seealso cref=     java.io.File </seealso>
	''' <seealso cref=     java.io.FileDescriptor </seealso>
	''' <seealso cref=     java.io.FileInputStream </seealso>
	''' <seealso cref=     java.nio.file.Files#newOutputStream
	''' @since   JDK1.0 </seealso>
	Public Class FileOutputStream
		Inherits OutputStream

		''' <summary>
		''' The system dependent file descriptor.
		''' </summary>
		Private ReadOnly fd As FileDescriptor

		''' <summary>
		''' True if the file is opened for append.
		''' </summary>
		Private ReadOnly append As Boolean

		''' <summary>
		''' The associated channel, initialized lazily.
		''' </summary>
		Private channel As java.nio.channels.FileChannel

		''' <summary>
		''' The path of the referenced file
		''' (null if the stream is created with a file descriptor)
		''' </summary>
		Private ReadOnly path As String

		Private ReadOnly closeLock As New Object
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private closed As Boolean = False

		''' <summary>
		''' Creates a file output stream to write to the file with the
		''' specified name. A new <code>FileDescriptor</code> object is
		''' created to represent this file connection.
		''' <p>
		''' First, if there is a security manager, its <code>checkWrite</code>
		''' method is called with <code>name</code> as its argument.
		''' <p>
		''' If the file exists but is a directory rather than a regular file, does
		''' not exist but cannot be created, or cannot be opened for any other
		''' reason then a <code>FileNotFoundException</code> is thrown.
		''' </summary>
		''' <param name="name">   the system-dependent filename </param>
		''' <exception cref="FileNotFoundException">  if the file exists but is a directory
		'''                   rather than a regular file, does not exist but cannot
		'''                   be created, or cannot be opened for any other reason </exception>
		''' <exception cref="SecurityException">  if a security manager exists and its
		'''               <code>checkWrite</code> method denies write access
		'''               to the file. </exception>
		''' <seealso cref=        java.lang.SecurityManager#checkWrite(java.lang.String) </seealso>
		Public Sub New(  name As String)
			Me.New(If(name IsNot Nothing, New File(name), Nothing), False)
		End Sub

		''' <summary>
		''' Creates a file output stream to write to the file with the specified
		''' name.  If the second argument is <code>true</code>, then
		''' bytes will be written to the end of the file rather than the beginning.
		''' A new <code>FileDescriptor</code> object is created to represent this
		''' file connection.
		''' <p>
		''' First, if there is a security manager, its <code>checkWrite</code>
		''' method is called with <code>name</code> as its argument.
		''' <p>
		''' If the file exists but is a directory rather than a regular file, does
		''' not exist but cannot be created, or cannot be opened for any other
		''' reason then a <code>FileNotFoundException</code> is thrown.
		''' </summary>
		''' <param name="name">        the system-dependent file name </param>
		''' <param name="append">      if <code>true</code>, then bytes will be written
		'''                   to the end of the file rather than the beginning </param>
		''' <exception cref="FileNotFoundException">  if the file exists but is a directory
		'''                   rather than a regular file, does not exist but cannot
		'''                   be created, or cannot be opened for any other reason. </exception>
		''' <exception cref="SecurityException">  if a security manager exists and its
		'''               <code>checkWrite</code> method denies write access
		'''               to the file. </exception>
		''' <seealso cref=        java.lang.SecurityManager#checkWrite(java.lang.String)
		''' @since     JDK1.1 </seealso>
		Public Sub New(  name As String,   append As Boolean)
			Me.New(If(name IsNot Nothing, New File(name), Nothing), append)
		End Sub

		''' <summary>
		''' Creates a file output stream to write to the file represented by
		''' the specified <code>File</code> object. A new
		''' <code>FileDescriptor</code> object is created to represent this
		''' file connection.
		''' <p>
		''' First, if there is a security manager, its <code>checkWrite</code>
		''' method is called with the path represented by the <code>file</code>
		''' argument as its argument.
		''' <p>
		''' If the file exists but is a directory rather than a regular file, does
		''' not exist but cannot be created, or cannot be opened for any other
		''' reason then a <code>FileNotFoundException</code> is thrown.
		''' </summary>
		''' <param name="file">               the file to be opened for writing. </param>
		''' <exception cref="FileNotFoundException">  if the file exists but is a directory
		'''                   rather than a regular file, does not exist but cannot
		'''                   be created, or cannot be opened for any other reason </exception>
		''' <exception cref="SecurityException">  if a security manager exists and its
		'''               <code>checkWrite</code> method denies write access
		'''               to the file. </exception>
		''' <seealso cref=        java.io.File#getPath() </seealso>
		''' <seealso cref=        java.lang.SecurityException </seealso>
		''' <seealso cref=        java.lang.SecurityManager#checkWrite(java.lang.String) </seealso>
		Public Sub New(  file_Renamed As File)
			Me.New(file_Renamed, False)
		End Sub

		''' <summary>
		''' Creates a file output stream to write to the file represented by
		''' the specified <code>File</code> object. If the second argument is
		''' <code>true</code>, then bytes will be written to the end of the file
		''' rather than the beginning. A new <code>FileDescriptor</code> object is
		''' created to represent this file connection.
		''' <p>
		''' First, if there is a security manager, its <code>checkWrite</code>
		''' method is called with the path represented by the <code>file</code>
		''' argument as its argument.
		''' <p>
		''' If the file exists but is a directory rather than a regular file, does
		''' not exist but cannot be created, or cannot be opened for any other
		''' reason then a <code>FileNotFoundException</code> is thrown.
		''' </summary>
		''' <param name="file">               the file to be opened for writing. </param>
		''' <param name="append">      if <code>true</code>, then bytes will be written
		'''                   to the end of the file rather than the beginning </param>
		''' <exception cref="FileNotFoundException">  if the file exists but is a directory
		'''                   rather than a regular file, does not exist but cannot
		'''                   be created, or cannot be opened for any other reason </exception>
		''' <exception cref="SecurityException">  if a security manager exists and its
		'''               <code>checkWrite</code> method denies write access
		'''               to the file. </exception>
		''' <seealso cref=        java.io.File#getPath() </seealso>
		''' <seealso cref=        java.lang.SecurityException </seealso>
		''' <seealso cref=        java.lang.SecurityManager#checkWrite(java.lang.String)
		''' @since 1.4 </seealso>
		Public Sub New(  file_Renamed As File,   append As Boolean)
			Dim name As String = (If(file_Renamed IsNot Nothing, file_Renamed.path, Nothing))
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkWrite(name)
			If name Is Nothing Then Throw New NullPointerException
			If file_Renamed.invalid Then Throw New FileNotFoundException("Invalid file path")
			Me.fd = New FileDescriptor
			fd.attach(Me)
			Me.append = append
			Me.path = name

			open(name, append)
		End Sub

		''' <summary>
		''' Creates a file output stream to write to the specified file
		''' descriptor, which represents an existing connection to an actual
		''' file in the file system.
		''' <p>
		''' First, if there is a security manager, its <code>checkWrite</code>
		''' method is called with the file descriptor <code>fdObj</code>
		''' argument as its argument.
		''' <p>
		''' If <code>fdObj</code> is null then a <code>NullPointerException</code>
		''' is thrown.
		''' <p>
		''' This constructor does not throw an exception if <code>fdObj</code>
		''' is <seealso cref="java.io.FileDescriptor#valid() invalid"/>.
		''' However, if the methods are invoked on the resulting stream to attempt
		''' I/O on the stream, an <code>IOException</code> is thrown.
		''' </summary>
		''' <param name="fdObj">   the file descriptor to be opened for writing </param>
		''' <exception cref="SecurityException">  if a security manager exists and its
		'''               <code>checkWrite</code> method denies
		'''               write access to the file descriptor </exception>
		''' <seealso cref=        java.lang.SecurityManager#checkWrite(java.io.FileDescriptor) </seealso>
		Public Sub New(  fdObj As FileDescriptor)
			Dim security As SecurityManager = System.securityManager
			If fdObj Is Nothing Then Throw New NullPointerException
			If security IsNot Nothing Then security.checkWrite(fdObj)
			Me.fd = fdObj
			Me.append = False
			Me.path = Nothing

			fd.attach(Me)
		End Sub

		''' <summary>
		''' Opens a file, with the specified name, for overwriting or appending. </summary>
		''' <param name="name"> name of file to be opened </param>
		''' <param name="append"> whether the file is to be opened in append mode </param>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Sub open0(  name As String,   append As Boolean)
		End Sub

		' wrap native call to allow instrumentation
		''' <summary>
		''' Opens a file, with the specified name, for overwriting or appending. </summary>
		''' <param name="name"> name of file to be opened </param>
		''' <param name="append"> whether the file is to be opened in append mode </param>
		Private Sub open(  name As String,   append As Boolean)
			open0(name, append)
		End Sub

		''' <summary>
		''' Writes the specified byte to this file output stream.
		''' </summary>
		''' <param name="b">   the byte to be written. </param>
		''' <param name="append">   {@code true} if the write operation first
		'''     advances the position to the end of file </param>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Sub write(  b As Integer,   append As Boolean)
		End Sub

		''' <summary>
		''' Writes the specified byte to this file output stream. Implements
		''' the <code>write</code> method of <code>OutputStream</code>.
		''' </summary>
		''' <param name="b">   the byte to be written. </param>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		Public Overrides Sub write(  b As Integer)
			write(b, append)
		End Sub

		''' <summary>
		''' Writes a sub array as a sequence of bytes. </summary>
		''' <param name="b"> the data to be written </param>
		''' <param name="off"> the start offset in the data </param>
		''' <param name="len"> the number of bytes that are written </param>
		''' <param name="append"> {@code true} to first advance the position to the
		'''     end of file </param>
		''' <exception cref="IOException"> If an I/O error has occurred. </exception>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Sub writeBytes(byte    As b(),   [off] As Integer,   len As Integer,   append As Boolean)
		End Sub

		''' <summary>
		''' Writes <code>b.length</code> bytes from the specified byte array
		''' to this file output stream.
		''' </summary>
		''' <param name="b">   the data. </param>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		Public Overrides Sub write(  b As SByte())
			writeBytes(b, 0, b.Length, append)
		End Sub

		''' <summary>
		''' Writes <code>len</code> bytes from the specified byte array
		''' starting at offset <code>off</code> to this file output stream.
		''' </summary>
		''' <param name="b">     the data. </param>
		''' <param name="off">   the start offset in the data. </param>
		''' <param name="len">   the number of bytes to write. </param>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public  Sub  write(byte b() , int off, int len) throws IOException
			writeBytes(b, off, len, append)

		''' <summary>
		''' Closes this file output stream and releases any system resources
		''' associated with this stream. This file output stream may no longer
		''' be used for writing bytes.
		''' 
		''' <p> If this stream has an associated channel then the channel is closed
		''' as well.
		''' </summary>
		''' <exception cref="IOException">  if an I/O error occurs.
		''' 
		''' @revised 1.4
		''' @spec JSR-51 </exception>
		public  Sub  close() throws IOException
			SyncLock closeLock
				If closed Then Return
				closed = True
			End SyncLock

			If channel IsNot Nothing Then channel.close()

			fd.closeAll(New CloseableAnonymousInnerClassHelper

		''' <summary>
		''' Returns the file descriptor associated with this stream.
		''' </summary>
		''' <returns>  the <code>FileDescriptor</code> object that represents
		'''          the connection to the file in the file system being used
		'''          by this <code>FileOutputStream</code> object.
		''' </returns>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		''' <seealso cref=        java.io.FileDescriptor </seealso>
		 public final FileDescriptor fD throws IOException
			If fd IsNot Nothing Then Return fd
			Throw New IOException

		''' <summary>
		''' Returns the unique <seealso cref="java.nio.channels.FileChannel FileChannel"/>
		''' object associated with this file output stream.
		''' 
		''' <p> The initial {@link java.nio.channels.FileChannel#position()
		''' position} of the returned channel will be equal to the
		''' number of bytes written to the file so far unless this stream is in
		''' append mode, in which case it will be equal to the size of the file.
		''' Writing bytes to this stream will increment the channel's position
		''' accordingly.  Changing the channel's position, either explicitly or by
		''' writing, will change this stream's file position.
		''' </summary>
		''' <returns>  the file channel associated with this file output stream
		''' 
		''' @since 1.4
		''' @spec JSR-51 </returns>
		public java.nio.channels.FileChannel channel
			SyncLock Me
				If channel Is Nothing Then channel = sun.nio.ch.FileChannelImpl.open(fd, path, False, True, append, Me)
				Return channel
			End SyncLock

		''' <summary>
		''' Cleans up the connection to the file, and ensures that the
		''' <code>close</code> method of this file output stream is
		''' called when there are no more references to this stream.
		''' </summary>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		''' <seealso cref=        java.io.FileInputStream#close() </seealso>
		protected  Sub  Finalize() throws IOException
			If fd IsNot Nothing Then
				If fd Is FileDescriptor.out OrElse fd Is FileDescriptor.err Then
					flush()
				Else
	'                 if fd is shared, the references in FileDescriptor
	'                 * will ensure that finalizer is only called when
	'                 * safe to do so. All references using the fd have
	'                 * become unreachable. We can call close()
	'                 
					close()
				End If
			End If

		private native  Sub  close0() throws IOException

		private static native  Sub  initIDs()

		static FileOutputStream()
			initIDs()

	End Class


	Private Class CloseableAnonymousInnerClassHelper
		Implements Closeable

		Public Overridable Sub close() Implements Closeable.close
		   close0()
		End Sub
	End Class
End Namespace