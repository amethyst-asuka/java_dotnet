Imports Microsoft.VisualBasic
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
	''' Instances of this class support both reading and writing to a
	''' random access file. A random access file behaves like a large
	''' array of bytes stored in the file system. There is a kind of cursor,
	''' or index into the implied array, called the <em>file pointer</em>;
	''' input operations read bytes starting at the file pointer and advance
	''' the file pointer past the bytes read. If the random access file is
	''' created in read/write mode, then output operations are also available;
	''' output operations write bytes starting at the file pointer and advance
	''' the file pointer past the bytes written. Output operations that write
	''' past the current end of the implied array cause the array to be
	''' extended. The file pointer can be read by the
	''' {@code getFilePointer} method and set by the {@code seek}
	''' method.
	''' <p>
	''' It is generally true of all the reading routines in this class that
	''' if end-of-file is reached before the desired number of bytes has been
	''' read, an {@code EOFException} (which is a kind of
	''' {@code IOException}) is thrown. If any byte cannot be read for
	''' any reason other than end-of-file, an {@code IOException} other
	''' than {@code EOFException} is thrown. In particular, an
	''' {@code IOException} may be thrown if the stream has been closed.
	''' 
	''' @author  unascribed
	''' @since   JDK1.0
	''' </summary>

	Public Class RandomAccessFile
		Implements DataOutput, DataInput, Closeable

		Private fd As FileDescriptor
		Private channel As java.nio.channels.FileChannel = Nothing
		Private rw As Boolean

		''' <summary>
		''' The path of the referenced file
		''' (null if the stream is created with a file descriptor)
		''' </summary>
		Private ReadOnly path As String

		Private closeLock As New Object
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private closed As Boolean = False

		Private Const O_RDONLY As Integer = 1
		Private Const O_RDWR As Integer = 2
		Private Const O_SYNC As Integer = 4
		Private Const O_DSYNC As Integer = 8

		''' <summary>
		''' Creates a random access file stream to read from, and optionally
		''' to write to, a file with the specified name. A new
		''' <seealso cref="FileDescriptor"/> object is created to represent the
		''' connection to the file.
		''' 
		''' <p> The <tt>mode</tt> argument specifies the access mode with which the
		''' file is to be opened.  The permitted values and their meanings are as
		''' specified for the <a
		''' href="#mode"><tt>RandomAccessFile(File,String)</tt></a> constructor.
		''' 
		''' <p>
		''' If there is a security manager, its {@code checkRead} method
		''' is called with the {@code name} argument
		''' as its argument to see if read access to the file is allowed.
		''' If the mode allows writing, the security manager's
		''' {@code checkWrite} method
		''' is also called with the {@code name} argument
		''' as its argument to see if write access to the file is allowed.
		''' </summary>
		''' <param name="name">   the system-dependent filename </param>
		''' <param name="mode">   the access <a href="#mode">mode</a> </param>
		''' <exception cref="IllegalArgumentException">  if the mode argument is not equal
		'''               to one of <tt>"r"</tt>, <tt>"rw"</tt>, <tt>"rws"</tt>, or
		'''               <tt>"rwd"</tt> </exception>
		''' <exception cref="FileNotFoundException">
		'''            if the mode is <tt>"r"</tt> but the given string does not
		'''            denote an existing regular file, or if the mode begins with
		'''            <tt>"rw"</tt> but the given string does not denote an
		'''            existing, writable regular file and a new regular file of
		'''            that name cannot be created, or if some other error occurs
		'''            while opening or creating the file </exception>
		''' <exception cref="SecurityException">         if a security manager exists and its
		'''               {@code checkRead} method denies read access to the file
		'''               or the mode is "rw" and the security manager's
		'''               {@code checkWrite} method denies write access to the file </exception>
		''' <seealso cref=        java.lang.SecurityException </seealso>
		''' <seealso cref=        java.lang.SecurityManager#checkRead(java.lang.String) </seealso>
		''' <seealso cref=        java.lang.SecurityManager#checkWrite(java.lang.String)
		''' @revised 1.4
		''' @spec JSR-51 </seealso>
		Public Sub New(ByVal name As String, ByVal mode As String)
			Me.New(If(name IsNot Nothing, New File(name), Nothing), mode)
		End Sub

		''' <summary>
		''' Creates a random access file stream to read from, and optionally to
		''' write to, the file specified by the <seealso cref="File"/> argument.  A new {@link
		''' FileDescriptor} object is created to represent this file connection.
		''' 
		''' <p>The <a name="mode"><tt>mode</tt></a> argument specifies the access mode
		''' in which the file is to be opened.  The permitted values and their
		''' meanings are:
		''' 
		''' <table summary="Access mode permitted values and meanings">
		''' <tr><th align="left">Value</th><th align="left">Meaning</th></tr>
		''' <tr><td valign="top"><tt>"r"</tt></td>
		'''     <td> Open for reading only.  Invoking any of the <tt>write</tt>
		'''     methods of the resulting object will cause an {@link
		'''     java.io.IOException} to be thrown. </td></tr>
		''' <tr><td valign="top"><tt>"rw"</tt></td>
		'''     <td> Open for reading and writing.  If the file does not already
		'''     exist then an attempt will be made to create it. </td></tr>
		''' <tr><td valign="top"><tt>"rws"</tt></td>
		'''     <td> Open for reading and writing, as with <tt>"rw"</tt>, and also
		'''     require that every update to the file's content or metadata be
		'''     written synchronously to the underlying storage device.  </td></tr>
		''' <tr><td valign="top"><tt>"rwd"&nbsp;&nbsp;</tt></td>
		'''     <td> Open for reading and writing, as with <tt>"rw"</tt>, and also
		'''     require that every update to the file's content be written
		'''     synchronously to the underlying storage device. </td></tr>
		''' </table>
		''' 
		''' The <tt>"rws"</tt> and <tt>"rwd"</tt> modes work much like the {@link
		''' java.nio.channels.FileChannel#force(boolean) force(boolean)} method of
		''' the <seealso cref="java.nio.channels.FileChannel"/> class, passing arguments of
		''' <tt>true</tt> and <tt>false</tt>, respectively, except that they always
		''' apply to every I/O operation and are therefore often more efficient.  If
		''' the file resides on a local storage device then when an invocation of a
		''' method of this class returns it is guaranteed that all changes made to
		''' the file by that invocation will have been written to that device.  This
		''' is useful for ensuring that critical information is not lost in the
		''' event of a system crash.  If the file does not reside on a local device
		''' then no such guarantee is made.
		''' 
		''' <p>The <tt>"rwd"</tt> mode can be used to reduce the number of I/O
		''' operations performed.  Using <tt>"rwd"</tt> only requires updates to the
		''' file's content to be written to storage; using <tt>"rws"</tt> requires
		''' updates to both the file's content and its metadata to be written, which
		''' generally requires at least one more low-level I/O operation.
		''' 
		''' <p>If there is a security manager, its {@code checkRead} method is
		''' called with the pathname of the {@code file} argument as its
		''' argument to see if read access to the file is allowed.  If the mode
		''' allows writing, the security manager's {@code checkWrite} method is
		''' also called with the path argument to see if write access to the file is
		''' allowed.
		''' </summary>
		''' <param name="file">   the file object </param>
		''' <param name="mode">   the access mode, as described
		'''                    <a href="#mode">above</a> </param>
		''' <exception cref="IllegalArgumentException">  if the mode argument is not equal
		'''               to one of <tt>"r"</tt>, <tt>"rw"</tt>, <tt>"rws"</tt>, or
		'''               <tt>"rwd"</tt> </exception>
		''' <exception cref="FileNotFoundException">
		'''            if the mode is <tt>"r"</tt> but the given file object does
		'''            not denote an existing regular file, or if the mode begins
		'''            with <tt>"rw"</tt> but the given file object does not denote
		'''            an existing, writable regular file and a new regular file of
		'''            that name cannot be created, or if some other error occurs
		'''            while opening or creating the file </exception>
		''' <exception cref="SecurityException">         if a security manager exists and its
		'''               {@code checkRead} method denies read access to the file
		'''               or the mode is "rw" and the security manager's
		'''               {@code checkWrite} method denies write access to the file </exception>
		''' <seealso cref=        java.lang.SecurityManager#checkRead(java.lang.String) </seealso>
		''' <seealso cref=        java.lang.SecurityManager#checkWrite(java.lang.String) </seealso>
		''' <seealso cref=        java.nio.channels.FileChannel#force(boolean)
		''' @revised 1.4
		''' @spec JSR-51 </seealso>
		Public Sub New(ByVal file_Renamed As File, ByVal mode As String)
			Dim name As String = (If(file_Renamed IsNot Nothing, file_Renamed.path, Nothing))
			Dim imode As Integer = -1
			If mode.Equals("r") Then
				imode = O_RDONLY
			ElseIf mode.StartsWith("rw") Then
				imode = O_RDWR
				rw = True
				If mode.length() > 2 Then
					If mode.Equals("rws") Then
						imode = imode Or O_SYNC
					ElseIf mode.Equals("rwd") Then
						imode = imode Or O_DSYNC
					Else
						imode = -1
					End If
				End If
			End If
			If imode < 0 Then Throw New IllegalArgumentException("Illegal mode """ & mode & """ must be one of " & """r"", ""rw"", ""rws""," & " or ""rwd""")
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then
				security.checkRead(name)
				If rw Then security.checkWrite(name)
			End If
			If name Is Nothing Then Throw New NullPointerException
			If file_Renamed.invalid Then Throw New FileNotFoundException("Invalid file path")
			fd = New FileDescriptor
			fd.attach(Me)
			path = name
			open(name, imode)
		End Sub

		''' <summary>
		''' Returns the opaque file descriptor object associated with this
		''' stream.
		''' </summary>
		''' <returns>     the file descriptor object associated with this stream. </returns>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		''' <seealso cref=        java.io.FileDescriptor </seealso>
		Public Property fD As FileDescriptor
			Get
				If fd IsNot Nothing Then Return fd
				Throw New IOException
			End Get
		End Property

		''' <summary>
		''' Returns the unique <seealso cref="java.nio.channels.FileChannel FileChannel"/>
		''' object associated with this file.
		''' 
		''' <p> The {@link java.nio.channels.FileChannel#position()
		''' position} of the returned channel will always be equal to
		''' this object's file-pointer offset as returned by the {@link
		''' #getFilePointer getFilePointer} method.  Changing this object's
		''' file-pointer offset, whether explicitly or by reading or writing bytes,
		''' will change the position of the channel, and vice versa.  Changing the
		''' file's length via this object will change the length seen via the file
		''' channel, and vice versa.
		''' </summary>
		''' <returns>  the file channel associated with this file
		''' 
		''' @since 1.4
		''' @spec JSR-51 </returns>
		Public Property channel As java.nio.channels.FileChannel
			Get
				SyncLock Me
					If channel Is Nothing Then channel = sun.nio.ch.FileChannelImpl.open(fd, path, True, rw, Me)
					Return channel
				End SyncLock
			End Get
		End Property

		''' <summary>
		''' Opens a file and returns the file descriptor.  The file is
		''' opened in read-write mode if the O_RDWR bit in {@code mode}
		''' is true, else the file is opened as read-only.
		''' If the {@code name} refers to a directory, an IOException
		''' is thrown.
		''' </summary>
		''' <param name="name"> the name of the file </param>
		''' <param name="mode"> the mode flags, a combination of the O_ constants
		'''             defined above </param>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Sub open0(ByVal name As String, ByVal mode As Integer)
		End Sub

		' wrap native call to allow instrumentation
		''' <summary>
		''' Opens a file and returns the file descriptor.  The file is
		''' opened in read-write mode if the O_RDWR bit in {@code mode}
		''' is true, else the file is opened as read-only.
		''' If the {@code name} refers to a directory, an IOException
		''' is thrown.
		''' </summary>
		''' <param name="name"> the name of the file </param>
		''' <param name="mode"> the mode flags, a combination of the O_ constants
		'''             defined above </param>
		Private Sub open(ByVal name As String, ByVal mode As Integer)
			open0(name, mode)
		End Sub

		' 'Read' primitives

		''' <summary>
		''' Reads a byte of data from this file. The byte is returned as an
		''' integer in the range 0 to 255 ({@code 0x00-0x0ff}). This
		''' method blocks if no input is yet available.
		''' <p>
		''' Although {@code RandomAccessFile} is not a subclass of
		''' {@code InputStream}, this method behaves in exactly the same
		''' way as the <seealso cref="InputStream#read()"/> method of
		''' {@code InputStream}.
		''' </summary>
		''' <returns>     the next byte of data, or {@code -1} if the end of the
		'''             file has been reached. </returns>
		''' <exception cref="IOException">  if an I/O error occurs. Not thrown if
		'''                          end-of-file has been reached. </exception>
		Public Overridable Function read() As Integer
			Return read0()
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Function read0() As Integer
		End Function

		''' <summary>
		''' Reads a sub array as a sequence of bytes. </summary>
		''' <param name="b"> the buffer into which the data is read. </param>
		''' <param name="off"> the start offset of the data. </param>
		''' <param name="len"> the number of bytes to read. </param>
		''' <exception cref="IOException"> If an I/O error has occurred. </exception>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Function readBytes(byte ByVal  As b(), ByVal [off] As Integer, ByVal len As Integer) As Integer
		End Function

		''' <summary>
		''' Reads up to {@code len} bytes of data from this file into an
		''' array of bytes. This method blocks until at least one byte of input
		''' is available.
		''' <p>
		''' Although {@code RandomAccessFile} is not a subclass of
		''' {@code InputStream}, this method behaves in exactly the
		''' same way as the <seealso cref="InputStream#read(byte[], int, int)"/> method of
		''' {@code InputStream}.
		''' </summary>
		''' <param name="b">     the buffer into which the data is read. </param>
		''' <param name="off">   the start offset in array {@code b}
		'''                   at which the data is written. </param>
		''' <param name="len">   the maximum number of bytes read. </param>
		''' <returns>     the total number of bytes read into the buffer, or
		'''             {@code -1} if there is no more data because the end of
		'''             the file has been reached. </returns>
		''' <exception cref="IOException"> If the first byte cannot be read for any reason
		''' other than end of file, or if the random access file has been closed, or if
		''' some other I/O error occurs. </exception>
		''' <exception cref="NullPointerException"> If {@code b} is {@code null}. </exception>
		''' <exception cref="IndexOutOfBoundsException"> If {@code off} is negative,
		''' {@code len} is negative, or {@code len} is greater than
		''' {@code b.length - off} </exception>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public int read(byte b() , int off, int len) throws IOException
			Return readBytes(b, off, len)

		''' <summary>
		''' Reads up to {@code b.length} bytes of data from this file
		''' into an array of bytes. This method blocks until at least one byte
		''' of input is available.
		''' <p>
		''' Although {@code RandomAccessFile} is not a subclass of
		''' {@code InputStream}, this method behaves in exactly the
		''' same way as the <seealso cref="InputStream#read(byte[])"/> method of
		''' {@code InputStream}.
		''' </summary>
		''' <param name="b">   the buffer into which the data is read. </param>
		''' <returns>     the total number of bytes read into the buffer, or
		'''             {@code -1} if there is no more data because the end of
		'''             this file has been reached. </returns>
		''' <exception cref="IOException"> If the first byte cannot be read for any reason
		''' other than end of file, or if the random access file has been closed, or if
		''' some other I/O error occurs. </exception>
		''' <exception cref="NullPointerException"> If {@code b} is {@code null}. </exception>
		public Integer read(SByte b()) throws IOException
			Return readBytes(b, 0, b.length)

		''' <summary>
		''' Reads {@code b.length} bytes from this file into the byte
		''' array, starting at the current file pointer. This method reads
		''' repeatedly from the file until the requested number of bytes are
		''' read. This method blocks until the requested number of bytes are
		''' read, the end of the stream is detected, or an exception is thrown.
		''' </summary>
		''' <param name="b">   the buffer into which the data is read. </param>
		''' <exception cref="EOFException">  if this file reaches the end before reading
		'''               all the bytes. </exception>
		''' <exception cref="IOException">   if an I/O error occurs. </exception>
		public final void readFully(SByte b()) throws IOException
			readFully(b, 0, b.length)

		''' <summary>
		''' Reads exactly {@code len} bytes from this file into the byte
		''' array, starting at the current file pointer. This method reads
		''' repeatedly from the file until the requested number of bytes are
		''' read. This method blocks until the requested number of bytes are
		''' read, the end of the stream is detected, or an exception is thrown.
		''' </summary>
		''' <param name="b">     the buffer into which the data is read. </param>
		''' <param name="off">   the start offset of the data. </param>
		''' <param name="len">   the number of bytes to read. </param>
		''' <exception cref="EOFException">  if this file reaches the end before reading
		'''               all the bytes. </exception>
		''' <exception cref="IOException">   if an I/O error occurs. </exception>
		public final void readFully(SByte b() , Integer off, Integer len) throws IOException
			Dim n As Integer = 0
			Do
				Dim count As Integer = Me.read(b, off + n, len - n)
				If count < 0 Then Throw New EOFException
				n += count
			Loop While n < len

		''' <summary>
		''' Attempts to skip over {@code n} bytes of input discarding the
		''' skipped bytes.
		''' <p>
		''' 
		''' This method may skip over some smaller number of bytes, possibly zero.
		''' This may result from any of a number of conditions; reaching end of
		''' file before {@code n} bytes have been skipped is only one
		''' possibility. This method never throws an {@code EOFException}.
		''' The actual number of bytes skipped is returned.  If {@code n}
		''' is negative, no bytes are skipped.
		''' </summary>
		''' <param name="n">   the number of bytes to be skipped. </param>
		''' <returns>     the actual number of bytes skipped. </returns>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		public Integer skipBytes(Integer n) throws IOException
			Dim pos As Long
			Dim len As Long
			Dim newpos As Long

			If n <= 0 Then Return 0
			pos = filePointer
			len = length()
			newpos = pos + n
			If newpos > len Then newpos = len
			seek(newpos)

			' return the actual number of bytes skipped 
			Return CInt(newpos - pos)

		' 'Write' primitives

		''' <summary>
		''' Writes the specified byte to this file. The write starts at
		''' the current file pointer.
		''' </summary>
		''' <param name="b">   the {@code byte} to be written. </param>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		public void write(Integer b) throws IOException
			write0(b)

		private native void write0(Integer b) throws IOException

		''' <summary>
		''' Writes a sub array as a sequence of bytes. </summary>
		''' <param name="b"> the data to be written
		''' </param>
		''' <param name="off"> the start offset in the data </param>
		''' <param name="len"> the number of bytes that are written </param>
		''' <exception cref="IOException"> If an I/O error has occurred. </exception>
		private native void writeBytes(SByte b() , Integer off, Integer len) throws IOException

		''' <summary>
		''' Writes {@code b.length} bytes from the specified byte array
		''' to this file, starting at the current file pointer.
		''' </summary>
		''' <param name="b">   the data. </param>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		public void write(SByte b()) throws IOException
			writeBytes(b, 0, b.length)

		''' <summary>
		''' Writes {@code len} bytes from the specified byte array
		''' starting at offset {@code off} to this file.
		''' </summary>
		''' <param name="b">     the data. </param>
		''' <param name="off">   the start offset in the data. </param>
		''' <param name="len">   the number of bytes to write. </param>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		public void write(SByte b() , Integer off, Integer len) throws IOException
			writeBytes(b, off, len)

		' 'Random access' stuff

		''' <summary>
		''' Returns the current offset in this file.
		''' </summary>
		''' <returns>     the offset from the beginning of the file, in bytes,
		'''             at which the next read or write occurs. </returns>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		public native Long filePointer throws IOException

		''' <summary>
		''' Sets the file-pointer offset, measured from the beginning of this
		''' file, at which the next read or write occurs.  The offset may be
		''' set beyond the end of the file. Setting the offset beyond the end
		''' of the file does not change the file length.  The file length will
		''' change only by writing after the offset has been set beyond the end
		''' of the file.
		''' </summary>
		''' <param name="pos">   the offset position, measured in bytes from the
		'''                   beginning of the file, at which to set the file
		'''                   pointer. </param>
		''' <exception cref="IOException">  if {@code pos} is less than
		'''                          {@code 0} or if an I/O error occurs. </exception>
		public void seek(Long pos) throws IOException
			If pos < 0 Then
				Throw New IOException("Negative seek offset")
			Else
				seek0(pos)
			End If

		private native void seek0(Long pos) throws IOException

		''' <summary>
		''' Returns the length of this file.
		''' </summary>
		''' <returns>     the length of this file, measured in bytes. </returns>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		public native Long length() throws IOException

		''' <summary>
		''' Sets the length of this file.
		''' 
		''' <p> If the present length of the file as returned by the
		''' {@code length} method is greater than the {@code newLength}
		''' argument then the file will be truncated.  In this case, if the file
		''' offset as returned by the {@code getFilePointer} method is greater
		''' than {@code newLength} then after this method returns the offset
		''' will be equal to {@code newLength}.
		''' 
		''' <p> If the present length of the file as returned by the
		''' {@code length} method is smaller than the {@code newLength}
		''' argument then the file will be extended.  In this case, the contents of
		''' the extended portion of the file are not defined.
		''' </summary>
		''' <param name="newLength">    The desired length of the file </param>
		''' <exception cref="IOException">  If an I/O error occurs
		''' @since      1.2 </exception>
		public native void lengthgth(Long newLength) throws IOException

		''' <summary>
		''' Closes this random access file stream and releases any system
		''' resources associated with the stream. A closed random access
		''' file cannot perform input or output operations and cannot be
		''' reopened.
		''' 
		''' <p> If this file has an associated channel then the channel is closed
		''' as well.
		''' </summary>
		''' <exception cref="IOException">  if an I/O error occurs.
		''' 
		''' @revised 1.4
		''' @spec JSR-51 </exception>
		public void close() throws IOException
			SyncLock closeLock
				If closed Then Return
				closed = True
			End SyncLock
			If channel IsNot Nothing Then channel.close()

			fd.closeAll(New CloseableAnonymousInnerClassHelper

		'
		'  Some "reading/writing Java data types" methods stolen from
		'  DataInputStream and DataOutputStream.
		'

		''' <summary>
		''' Reads a {@code boolean} from this file. This method reads a
		''' single byte from the file, starting at the current file pointer.
		''' A value of {@code 0} represents
		''' {@code false}. Any other value represents {@code true}.
		''' This method blocks until the byte is read, the end of the stream
		''' is detected, or an exception is thrown.
		''' </summary>
		''' <returns>     the {@code boolean} value read. </returns>
		''' <exception cref="EOFException">  if this file has reached the end. </exception>
		''' <exception cref="IOException">   if an I/O error occurs. </exception>
		public final Boolean readBoolean() throws IOException
			Dim ch As Integer = Me.read()
			If ch < 0 Then Throw New EOFException
			Return (ch <> 0)

		''' <summary>
		''' Reads a signed eight-bit value from this file. This method reads a
		''' byte from the file, starting from the current file pointer.
		''' If the byte read is {@code b}, where
		''' <code>0&nbsp;&lt;=&nbsp;b&nbsp;&lt;=&nbsp;255</code>,
		''' then the result is:
		''' <blockquote><pre>
		'''     (byte)(b)
		''' </pre></blockquote>
		''' <p>
		''' This method blocks until the byte is read, the end of the stream
		''' is detected, or an exception is thrown.
		''' </summary>
		''' <returns>     the next byte of this file as a signed eight-bit
		'''             {@code byte}. </returns>
		''' <exception cref="EOFException">  if this file has reached the end. </exception>
		''' <exception cref="IOException">   if an I/O error occurs. </exception>
		public final SByte readByte() throws IOException
			Dim ch As Integer = Me.read()
			If ch < 0 Then Throw New EOFException
			Return CByte(ch)

		''' <summary>
		''' Reads an unsigned eight-bit number from this file. This method reads
		''' a byte from this file, starting at the current file pointer,
		''' and returns that byte.
		''' <p>
		''' This method blocks until the byte is read, the end of the stream
		''' is detected, or an exception is thrown.
		''' </summary>
		''' <returns>     the next byte of this file, interpreted as an unsigned
		'''             eight-bit number. </returns>
		''' <exception cref="EOFException">  if this file has reached the end. </exception>
		''' <exception cref="IOException">   if an I/O error occurs. </exception>
		public final Integer readUnsignedByte() throws IOException
			Dim ch As Integer = Me.read()
			If ch < 0 Then Throw New EOFException
			Return ch

		''' <summary>
		''' Reads a signed 16-bit number from this file. The method reads two
		''' bytes from this file, starting at the current file pointer.
		''' If the two bytes read, in order, are
		''' {@code b1} and {@code b2}, where each of the two values is
		''' between {@code 0} and {@code 255}, inclusive, then the
		''' result is equal to:
		''' <blockquote><pre>
		'''     (short)((b1 &lt;&lt; 8) | b2)
		''' </pre></blockquote>
		''' <p>
		''' This method blocks until the two bytes are read, the end of the
		''' stream is detected, or an exception is thrown.
		''' </summary>
		''' <returns>     the next two bytes of this file, interpreted as a signed
		'''             16-bit number. </returns>
		''' <exception cref="EOFException">  if this file reaches the end before reading
		'''               two bytes. </exception>
		''' <exception cref="IOException">   if an I/O error occurs. </exception>
		public final Short readShort() throws IOException
			Dim ch1 As Integer = Me.read()
			Dim ch2 As Integer = Me.read()
			If (ch1 Or ch2) < 0 Then Throw New EOFException
			Return CShort(Fix((ch1 << 8) + (ch2 << 0)))

		''' <summary>
		''' Reads an unsigned 16-bit number from this file. This method reads
		''' two bytes from the file, starting at the current file pointer.
		''' If the bytes read, in order, are
		''' {@code b1} and {@code b2}, where
		''' <code>0&nbsp;&lt;=&nbsp;b1, b2&nbsp;&lt;=&nbsp;255</code>,
		''' then the result is equal to:
		''' <blockquote><pre>
		'''     (b1 &lt;&lt; 8) | b2
		''' </pre></blockquote>
		''' <p>
		''' This method blocks until the two bytes are read, the end of the
		''' stream is detected, or an exception is thrown.
		''' </summary>
		''' <returns>     the next two bytes of this file, interpreted as an unsigned
		'''             16-bit integer. </returns>
		''' <exception cref="EOFException">  if this file reaches the end before reading
		'''               two bytes. </exception>
		''' <exception cref="IOException">   if an I/O error occurs. </exception>
		public final Integer readUnsignedShort() throws IOException
			Dim ch1 As Integer = Me.read()
			Dim ch2 As Integer = Me.read()
			If (ch1 Or ch2) < 0 Then Throw New EOFException
			Return (ch1 << 8) + (ch2 << 0)

		''' <summary>
		''' Reads a character from this file. This method reads two
		''' bytes from the file, starting at the current file pointer.
		''' If the bytes read, in order, are
		''' {@code b1} and {@code b2}, where
		''' <code>0&nbsp;&lt;=&nbsp;b1,&nbsp;b2&nbsp;&lt;=&nbsp;255</code>,
		''' then the result is equal to:
		''' <blockquote><pre>
		'''     (char)((b1 &lt;&lt; 8) | b2)
		''' </pre></blockquote>
		''' <p>
		''' This method blocks until the two bytes are read, the end of the
		''' stream is detected, or an exception is thrown.
		''' </summary>
		''' <returns>     the next two bytes of this file, interpreted as a
		'''                  {@code char}. </returns>
		''' <exception cref="EOFException">  if this file reaches the end before reading
		'''               two bytes. </exception>
		''' <exception cref="IOException">   if an I/O error occurs. </exception>
		public final Char readChar() throws IOException
			Dim ch1 As Integer = Me.read()
			Dim ch2 As Integer = Me.read()
			If (ch1 Or ch2) < 0 Then Throw New EOFException
			Return CChar((ch1 << 8) + (ch2 << 0))

		''' <summary>
		''' Reads a signed 32-bit integer from this file. This method reads 4
		''' bytes from the file, starting at the current file pointer.
		''' If the bytes read, in order, are {@code b1},
		''' {@code b2}, {@code b3}, and {@code b4}, where
		''' <code>0&nbsp;&lt;=&nbsp;b1, b2, b3, b4&nbsp;&lt;=&nbsp;255</code>,
		''' then the result is equal to:
		''' <blockquote><pre>
		'''     (b1 &lt;&lt; 24) | (b2 &lt;&lt; 16) + (b3 &lt;&lt; 8) + b4
		''' </pre></blockquote>
		''' <p>
		''' This method blocks until the four bytes are read, the end of the
		''' stream is detected, or an exception is thrown.
		''' </summary>
		''' <returns>     the next four bytes of this file, interpreted as an
		'''             {@code int}. </returns>
		''' <exception cref="EOFException">  if this file reaches the end before reading
		'''               four bytes. </exception>
		''' <exception cref="IOException">   if an I/O error occurs. </exception>
		public final Integer readInt() throws IOException
			Dim ch1 As Integer = Me.read()
			Dim ch2 As Integer = Me.read()
			Dim ch3 As Integer = Me.read()
			Dim ch4 As Integer = Me.read()
			If (ch1 Or ch2 Or ch3 Or ch4) < 0 Then Throw New EOFException
			Return ((ch1 << 24) + (ch2 << 16) + (ch3 << 8) + (ch4 << 0))

		''' <summary>
		''' Reads a signed 64-bit integer from this file. This method reads eight
		''' bytes from the file, starting at the current file pointer.
		''' If the bytes read, in order, are
		''' {@code b1}, {@code b2}, {@code b3},
		''' {@code b4}, {@code b5}, {@code b6},
		''' {@code b7}, and {@code b8,} where:
		''' <blockquote><pre>
		'''     0 &lt;= b1, b2, b3, b4, b5, b6, b7, b8 &lt;=255,
		''' </pre></blockquote>
		''' <p>
		''' then the result is equal to:
		''' <blockquote><pre>
		'''     ((long)b1 &lt;&lt; 56) + ((long)b2 &lt;&lt; 48)
		'''     + ((long)b3 &lt;&lt; 40) + ((long)b4 &lt;&lt; 32)
		'''     + ((long)b5 &lt;&lt; 24) + ((long)b6 &lt;&lt; 16)
		'''     + ((long)b7 &lt;&lt; 8) + b8
		''' </pre></blockquote>
		''' <p>
		''' This method blocks until the eight bytes are read, the end of the
		''' stream is detected, or an exception is thrown.
		''' </summary>
		''' <returns>     the next eight bytes of this file, interpreted as a
		'''             {@code long}. </returns>
		''' <exception cref="EOFException">  if this file reaches the end before reading
		'''               eight bytes. </exception>
		''' <exception cref="IOException">   if an I/O error occurs. </exception>
		public final Long readLong() throws IOException
			Return (CLng(readInt()) << 32) + (readInt() And &HFFFFFFFFL)

		''' <summary>
		''' Reads a {@code float} from this file. This method reads an
		''' {@code int} value, starting at the current file pointer,
		''' as if by the {@code readInt} method
		''' and then converts that {@code int} to a {@code float}
		''' using the {@code intBitsToFloat} method in class
		''' {@code Float}.
		''' <p>
		''' This method blocks until the four bytes are read, the end of the
		''' stream is detected, or an exception is thrown.
		''' </summary>
		''' <returns>     the next four bytes of this file, interpreted as a
		'''             {@code float}. </returns>
		''' <exception cref="EOFException">  if this file reaches the end before reading
		'''             four bytes. </exception>
		''' <exception cref="IOException">   if an I/O error occurs. </exception>
		''' <seealso cref=        java.io.RandomAccessFile#readInt() </seealso>
		''' <seealso cref=        java.lang.Float#intBitsToFloat(int) </seealso>
		public final Single readFloat() throws IOException
			Return Float.intBitsToFloat(readInt())

		''' <summary>
		''' Reads a {@code double} from this file. This method reads a
		''' {@code long} value, starting at the current file pointer,
		''' as if by the {@code readLong} method
		''' and then converts that {@code long} to a {@code double}
		''' using the {@code longBitsToDouble} method in
		''' class {@code Double}.
		''' <p>
		''' This method blocks until the eight bytes are read, the end of the
		''' stream is detected, or an exception is thrown.
		''' </summary>
		''' <returns>     the next eight bytes of this file, interpreted as a
		'''             {@code double}. </returns>
		''' <exception cref="EOFException">  if this file reaches the end before reading
		'''             eight bytes. </exception>
		''' <exception cref="IOException">   if an I/O error occurs. </exception>
		''' <seealso cref=        java.io.RandomAccessFile#readLong() </seealso>
		''' <seealso cref=        java.lang.Double#longBitsToDouble(long) </seealso>
		public final Double readDouble() throws IOException
			Return Double.longBitsToDouble(readLong())

		''' <summary>
		''' Reads the next line of text from this file.  This method successively
		''' reads bytes from the file, starting at the current file pointer,
		''' until it reaches a line terminator or the end
		''' of the file.  Each byte is converted into a character by taking the
		''' byte's value for the lower eight bits of the character and setting the
		''' high eight bits of the character to zero.  This method does not,
		''' therefore, support the full Unicode character set.
		''' 
		''' <p> A line of text is terminated by a carriage-return character
		''' ({@code '\u005Cr'}), a newline character ({@code '\u005Cn'}), a
		''' carriage-return character immediately followed by a newline character,
		''' or the end of the file.  Line-terminating characters are discarded and
		''' are not included as part of the string returned.
		''' 
		''' <p> This method blocks until a newline character is read, a carriage
		''' return and the byte following it are read (to see if it is a newline),
		''' the end of the file is reached, or an exception is thrown.
		''' </summary>
		''' <returns>     the next line of text from this file, or null if end
		'''             of file is encountered before even one byte is read. </returns>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>

		public final String readLine() throws IOException
			Dim input As New StringBuffer
			Dim c As Integer = -1
			Dim eol As Boolean = False

			Do While Not eol
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Select Case c = read()
				Case -1, ControlChars.Lf
					eol = True
				Case ControlChars.Cr
					eol = True
					Dim cur As Long = filePointer
					If (read()) <> ControlChars.Lf Then seek(cur)
				Case Else
					input.append(ChrW(c))
				End Select
			Loop

			If (c = -1) AndAlso (input.length() = 0) Then Return Nothing
			Return input.ToString()

		''' <summary>
		''' Reads in a string from this file. The string has been encoded
		''' using a
		''' <a href="DataInput.html#modified-utf-8">modified UTF-8</a>
		''' format.
		''' <p>
		''' The first two bytes are read, starting from the current file
		''' pointer, as if by
		''' {@code readUnsignedShort}. This value gives the number of
		''' following bytes that are in the encoded string, not
		''' the length of the resulting string. The following bytes are then
		''' interpreted as bytes encoding characters in the modified UTF-8 format
		''' and are converted into characters.
		''' <p>
		''' This method blocks until all the bytes are read, the end of the
		''' stream is detected, or an exception is thrown.
		''' </summary>
		''' <returns>     a Unicode string. </returns>
		''' <exception cref="EOFException">            if this file reaches the end before
		'''               reading all the bytes. </exception>
		''' <exception cref="IOException">             if an I/O error occurs. </exception>
		''' <exception cref="UTFDataFormatException">  if the bytes do not represent
		'''               valid modified UTF-8 encoding of a Unicode string. </exception>
		''' <seealso cref=        java.io.RandomAccessFile#readUnsignedShort() </seealso>
		public final String readUTF() throws IOException
			Return DataInputStream.readUTF(Me)

		''' <summary>
		''' Writes a {@code boolean} to the file as a one-byte value. The
		''' value {@code true} is written out as the value
		''' {@code (byte)1}; the value {@code false} is written out
		''' as the value {@code (byte)0}. The write starts at
		''' the current position of the file pointer.
		''' </summary>
		''' <param name="v">   a {@code boolean} value to be written. </param>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		public final void writeBoolean(Boolean v) throws IOException
			write(If(v, 1, 0))
			'written++;

		''' <summary>
		''' Writes a {@code byte} to the file as a one-byte value. The
		''' write starts at the current position of the file pointer.
		''' </summary>
		''' <param name="v">   a {@code byte} value to be written. </param>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		public final void writeByte(Integer v) throws IOException
			write(v)
			'written++;

		''' <summary>
		''' Writes a {@code short} to the file as two bytes, high byte first.
		''' The write starts at the current position of the file pointer.
		''' </summary>
		''' <param name="v">   a {@code short} to be written. </param>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		public final void writeShort(Integer v) throws IOException
			write((CInt(CUInt(v) >> 8)) And &HFF)
			write((CInt(CUInt(v) >> 0)) And &HFF)
			'written += 2;

		''' <summary>
		''' Writes a {@code char} to the file as a two-byte value, high
		''' byte first. The write starts at the current position of the
		''' file pointer.
		''' </summary>
		''' <param name="v">   a {@code char} value to be written. </param>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		public final void writeChar(Integer v) throws IOException
			write((CInt(CUInt(v) >> 8)) And &HFF)
			write((CInt(CUInt(v) >> 0)) And &HFF)
			'written += 2;

		''' <summary>
		''' Writes an {@code int} to the file as four bytes, high byte first.
		''' The write starts at the current position of the file pointer.
		''' </summary>
		''' <param name="v">   an {@code int} to be written. </param>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		public final void writeInt(Integer v) throws IOException
			write((CInt(CUInt(v) >> 24)) And &HFF)
			write((CInt(CUInt(v) >> 16)) And &HFF)
			write((CInt(CUInt(v) >> 8)) And &HFF)
			write((CInt(CUInt(v) >> 0)) And &HFF)
			'written += 4;

		''' <summary>
		''' Writes a {@code long} to the file as eight bytes, high byte first.
		''' The write starts at the current position of the file pointer.
		''' </summary>
		''' <param name="v">   a {@code long} to be written. </param>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		public final void writeLong(Long v) throws IOException
			write(CInt(CInt(CUInt(v) >> 56)) And &HFF)
			write(CInt(CInt(CUInt(v) >> 48)) And &HFF)
			write(CInt(CInt(CUInt(v) >> 40)) And &HFF)
			write(CInt(CInt(CUInt(v) >> 32)) And &HFF)
			write(CInt(CInt(CUInt(v) >> 24)) And &HFF)
			write(CInt(CInt(CUInt(v) >> 16)) And &HFF)
			write(CInt(CInt(CUInt(v) >> 8)) And &HFF)
			write(CInt(CInt(CUInt(v) >> 0)) And &HFF)
			'written += 8;

		''' <summary>
		''' Converts the float argument to an {@code int} using the
		''' {@code floatToIntBits} method in class {@code Float},
		''' and then writes that {@code int} value to the file as a
		''' four-byte quantity, high byte first. The write starts at the
		''' current position of the file pointer.
		''' </summary>
		''' <param name="v">   a {@code float} value to be written. </param>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		''' <seealso cref=        java.lang.Float#floatToIntBits(float) </seealso>
		public final void writeFloat(Single v) throws IOException
			writeInt(Float.floatToIntBits(v))

		''' <summary>
		''' Converts the double argument to a {@code long} using the
		''' {@code doubleToLongBits} method in class {@code Double},
		''' and then writes that {@code long} value to the file as an
		''' eight-byte quantity, high byte first. The write starts at the current
		''' position of the file pointer.
		''' </summary>
		''' <param name="v">   a {@code double} value to be written. </param>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		''' <seealso cref=        java.lang.Double#doubleToLongBits(double) </seealso>
		public final void writeDouble(Double v) throws IOException
			writeLong(Double.doubleToLongBits(v))

		''' <summary>
		''' Writes the string to the file as a sequence of bytes. Each
		''' character in the string is written out, in sequence, by discarding
		''' its high eight bits. The write starts at the current position of
		''' the file pointer.
		''' </summary>
		''' <param name="s">   a string of bytes to be written. </param>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		public final void writeBytes(String s) throws IOException
			Dim len As Integer = s.length()
			Dim b As SByte() = New SByte(len - 1){}
			s.getBytes(0, len, b, 0)
			writeBytes(b, 0, len)

		''' <summary>
		''' Writes a string to the file as a sequence of characters. Each
		''' character is written to the data output stream as if by the
		''' {@code writeChar} method. The write starts at the current
		''' position of the file pointer.
		''' </summary>
		''' <param name="s">   a {@code String} value to be written. </param>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		''' <seealso cref=        java.io.RandomAccessFile#writeChar(int) </seealso>
		public final void writeChars(String s) throws IOException
			Dim clen As Integer = s.length()
			Dim blen As Integer = 2*clen
			Dim b As SByte() = New SByte(blen - 1){}
			Dim c As Char() = New Char(clen - 1){}
			s.getChars(0, clen, c, 0)
			Dim i As Integer = 0
			Dim j As Integer = 0
			Do While i < clen
				b(j) = CByte(CInt(CUInt(c(i)) >> 8))
				j += 1
				b(j) = CByte(CInt(CUInt(c(i)) >> 0))
				j += 1
				i += 1
			Loop
			writeBytes(b, 0, blen)

		''' <summary>
		''' Writes a string to the file using
		''' <a href="DataInput.html#modified-utf-8">modified UTF-8</a>
		''' encoding in a machine-independent manner.
		''' <p>
		''' First, two bytes are written to the file, starting at the
		''' current file pointer, as if by the
		''' {@code writeShort} method giving the number of bytes to
		''' follow. This value is the number of bytes actually written out,
		''' not the length of the string. Following the length, each character
		''' of the string is output, in sequence, using the modified UTF-8 encoding
		''' for each character.
		''' </summary>
		''' <param name="str">   a string to be written. </param>
		''' <exception cref="IOException">  if an I/O error occurs. </exception>
		public final void writeUTF(String str) throws IOException
			DataOutputStream.writeUTF(str, Me)

		private static native void initIDs()

		private native void close0() throws IOException

		static RandomAccessFile()
			initIDs()
	End Class


	Private Class CloseableAnonymousInnerClassHelper
		Implements Closeable

		Public Overridable Sub close() Implements Closeable.close
		   close0()
		End Sub
	End Class
End Namespace