Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.InteropServices

'
' * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.net



	''' <summary>
	''' This stream extends FileInputStream to implement a
	''' SocketInputStream. Note that this class should <b>NOT</b> be
	''' public.
	''' 
	''' @author      Jonathan Payne
	''' @author      Arthur van Hoff
	''' </summary>
	Friend Class SocketInputStream
		Inherits java.io.FileInputStream

		Shared Sub New()
			init()
		End Sub

		Private eof As Boolean
		Private impl As AbstractPlainSocketImpl = Nothing
		Private temp As SByte()
		Private socket_Renamed As Socket = Nothing

		''' <summary>
		''' Creates a new SocketInputStream. Can only be called
		''' by a Socket. This method needs to hang on to the owner Socket so
		''' that the fd will not be closed. </summary>
		''' <param name="impl"> the implemented socket input stream </param>
		Friend Sub New(  impl As AbstractPlainSocketImpl)
			MyBase.New(impl.fileDescriptor)
			Me.impl = impl
			socket_Renamed = impl.socket
		End Sub

		''' <summary>
		''' Returns the unique <seealso cref="java.nio.channels.FileChannel FileChannel"/>
		''' object associated with this file input stream.</p>
		''' 
		''' The {@code getChannel} method of {@code SocketInputStream}
		''' returns {@code null} since it is a socket based stream.</p>
		''' </summary>
		''' <returns>  the file channel associated with this file input stream
		''' 
		''' @since 1.4
		''' @spec JSR-51 </returns>
		Public Property NotOverridable Overrides channel As java.nio.channels.FileChannel
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Reads into an array of bytes at the specified offset using
		''' the received socket primitive. </summary>
		''' <param name="fd"> the FileDescriptor </param>
		''' <param name="b"> the buffer into which the data is read </param>
		''' <param name="off"> the start offset of the data </param>
		''' <param name="len"> the maximum number of bytes read </param>
		''' <param name="timeout"> the read timeout in ms </param>
		''' <returns> the actual number of bytes read, -1 is
		'''          returned when the end of the stream is reached. </returns>
		''' <exception cref="IOException"> If an I/O error has occurred. </exception>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Function socketRead0(  fd As java.io.FileDescriptor, byte    As b(),   [off] As Integer,   len As Integer,   timeout As Integer) As Integer
		End Function

		' wrap native call to allow instrumentation
		''' <summary>
		''' Reads into an array of bytes at the specified offset using
		''' the received socket primitive. </summary>
		''' <param name="fd"> the FileDescriptor </param>
		''' <param name="b"> the buffer into which the data is read </param>
		''' <param name="off"> the start offset of the data </param>
		''' <param name="len"> the maximum number of bytes read </param>
		''' <param name="timeout"> the read timeout in ms </param>
		''' <returns> the actual number of bytes read, -1 is
		'''          returned when the end of the stream is reached. </returns>
		''' <exception cref="IOException"> If an I/O error has occurred. </exception>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		private int socketRead(java.io.FileDescriptor fd, byte b() , int off, int len, int timeout) throws java.io.IOException
			Return socketRead0(fd, b, off, len, timeout)

		''' <summary>
		''' Reads into a byte array data from the socket. </summary>
		''' <param name="b"> the buffer into which the data is read </param>
		''' <returns> the actual number of bytes read, -1 is
		'''          returned when the end of the stream is reached. </returns>
		''' <exception cref="IOException"> If an I/O error has occurred. </exception>
		public Integer read(SByte b()) throws java.io.IOException
			Return read(b, 0, b.length)

		''' <summary>
		''' Reads into a byte array <i>b</i> at offset <i>off</i>,
		''' <i>length</i> bytes of data. </summary>
		''' <param name="b"> the buffer into which the data is read </param>
		''' <param name="off"> the start offset of the data </param>
		''' <param name="length"> the maximum number of bytes read </param>
		''' <returns> the actual number of bytes read, -1 is
		'''          returned when the end of the stream is reached. </returns>
		''' <exception cref="IOException"> If an I/O error has occurred. </exception>
		public Integer read(SByte b() , Integer off, Integer length) throws java.io.IOException
			Return read(b, off, length, impl.timeout)

		Integer read(SByte b() , Integer off, Integer length, Integer timeout) throws java.io.IOException
			Dim n As Integer

			' EOF already encountered
			If eof Then Return -1

			' connection reset
			If impl.connectionReset Then Throw New SocketException("Connection reset")

			' bounds check
			If length <= 0 OrElse off < 0 OrElse off + length > b.length Then
				If length = 0 Then Return 0
				Throw New ArrayIndexOutOfBoundsException
			End If

			Dim gotReset As Boolean = False

			' acquire file descriptor and do the read
			Dim fd As java.io.FileDescriptor = impl.acquireFD()
			Try
				n = socketRead(fd, b, off, length, timeout)
				If n > 0 Then Return n
			Catch rstExc As sun.net.ConnectionResetException
				gotReset = True
			Finally
				impl.releaseFD()
			End Try

	'        
	'         * We receive a "connection reset" but there may be bytes still
	'         * buffered on the socket
	'         
			If gotReset Then
				impl.connectionResetPendinging()
				impl.acquireFD()
				Try
					n = socketRead(fd, b, off, length, timeout)
					If n > 0 Then Return n
				Catch rstExc As sun.net.ConnectionResetException
				Finally
					impl.releaseFD()
				End Try
			End If

	'        
	'         * If we get here we are at EOF, the socket has been closed,
	'         * or the connection has been reset.
	'         
			If impl.closedOrPending Then Throw New SocketException("Socket closed")
			If impl.connectionResetPending Then impl.connectionResetset()
			If impl.connectionReset Then Throw New SocketException("Connection reset")
			eof = True
			Return -1

		''' <summary>
		''' Reads a single byte from the socket.
		''' </summary>
		public Integer read() throws java.io.IOException
			If eof Then Return -1
			temp = New SByte(0){}
			Dim n As Integer = read(temp, 0, 1)
			If n <= 0 Then Return -1
			Return temp(0) And &Hff

		''' <summary>
		''' Skips n bytes of input. </summary>
		''' <param name="numbytes"> the number of bytes to skip </param>
		''' <returns>  the actual number of bytes skipped. </returns>
		''' <exception cref="IOException"> If an I/O error has occurred. </exception>
		public Long skip(Long numbytes) throws java.io.IOException
			If numbytes <= 0 Then Return 0
			Dim n As Long = numbytes
			Dim buflen As Integer = CInt(Fix (System.Math.Min(1024, n)))
			Dim data As SByte() = New SByte(buflen - 1){}
			Do While n > 0
				Dim r As Integer = read(data, 0, CInt(Fix (System.Math.Min(CLng(buflen), n))))
				If r < 0 Then Exit Do
				n -= r
			Loop
			Return numbytes - n

		''' <summary>
		''' Returns the number of bytes that can be read without blocking. </summary>
		''' <returns> the number of immediately available bytes </returns>
		public Integer available() throws java.io.IOException
			Return impl.available()

		''' <summary>
		''' Closes the stream.
		''' </summary>
		private Boolean closing = False
		public  Sub  close() throws java.io.IOException
			' Prevent recursion. See BugId 4484411
			If closing Then Return
			closing = True
			If socket_Renamed IsNot Nothing Then
				If Not socket_Renamed.closed Then socket_Renamed.close()
			Else
				impl.close()
			End If
			closing = False

		void eOFEOF(Boolean eof)
			Me.eof = eof

		''' <summary>
		''' Overrides finalize, the fd is closed by the Socket.
		''' </summary>
		protected  Sub  Finalize()

		''' <summary>
		''' Perform class load-time initializations.
		''' </summary>
		private native static  Sub  init()
	End Class

End Namespace