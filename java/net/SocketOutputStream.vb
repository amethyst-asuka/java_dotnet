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
	''' This stream extends FileOutputStream to implement a
	''' SocketOutputStream. Note that this class should <b>NOT</b> be
	''' public.
	''' 
	''' @author      Jonathan Payne
	''' @author      Arthur van Hoff
	''' </summary>
	Friend Class SocketOutputStream
		Inherits java.io.FileOutputStream

		Shared Sub New()
			init()
		End Sub

		Private impl As AbstractPlainSocketImpl = Nothing
		Private temp As SByte() = New SByte(0){}
		Private socket_Renamed As Socket = Nothing

		''' <summary>
		''' Creates a new SocketOutputStream. Can only be called
		''' by a Socket. This method needs to hang on to the owner Socket so
		''' that the fd will not be closed. </summary>
		''' <param name="impl"> the socket output stream inplemented </param>
		Friend Sub New(ByVal impl As AbstractPlainSocketImpl)
			MyBase.New(impl.fileDescriptor)
			Me.impl = impl
			socket_Renamed = impl.socket
		End Sub

		''' <summary>
		''' Returns the unique <seealso cref="java.nio.channels.FileChannel FileChannel"/>
		''' object associated with this file output stream. </p>
		''' 
		''' The {@code getChannel} method of {@code SocketOutputStream}
		''' returns {@code null} since it is a socket based stream.</p>
		''' </summary>
		''' <returns>  the file channel associated with this file output stream
		''' 
		''' @since 1.4
		''' @spec JSR-51 </returns>
		Public Property NotOverridable Overrides channel As java.nio.channels.FileChannel
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Writes to the socket. </summary>
		''' <param name="fd"> the FileDescriptor </param>
		''' <param name="b"> the data to be written </param>
		''' <param name="off"> the start offset in the data </param>
		''' <param name="len"> the number of bytes that are written </param>
		''' <exception cref="IOException"> If an I/O error has occurred. </exception>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Sub socketWrite0(ByVal fd As java.io.FileDescriptor, ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer)
		End Sub

		''' <summary>
		''' Writes to the socket with appropriate locking of the
		''' FileDescriptor. </summary>
		''' <param name="b"> the data to be written </param>
		''' <param name="off"> the start offset in the data </param>
		''' <param name="len"> the number of bytes that are written </param>
		''' <exception cref="IOException"> If an I/O error has occurred. </exception>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		private void socketWrite(byte b() , int off, int len) throws java.io.IOException

			If len <= 0 OrElse off < 0 OrElse off + len > b.length Then
				If len = 0 Then Return
				Throw New ArrayIndexOutOfBoundsException
			End If

			Dim fd_Renamed As java.io.FileDescriptor = impl.acquireFD()
			Try
				socketWrite0(fd_Renamed, b, off, len)
			Catch se As SocketException
				If TypeOf se Is sun.net.ConnectionResetException Then
					impl.connectionResetPendinging()
					se = New SocketException("Connection reset")
				End If
				If impl.closedOrPending Then
					Throw New SocketException("Socket closed")
				Else
					Throw se
				End If
			Finally
				impl.releaseFD()
			End Try

		''' <summary>
		''' Writes a byte to the socket. </summary>
		''' <param name="b"> the data to be written </param>
		''' <exception cref="IOException"> If an I/O error has occurred. </exception>
		public void write(Integer b) throws java.io.IOException
			temp(0) = CByte(b)
			socketWrite(temp, 0, 1)

		''' <summary>
		''' Writes the contents of the buffer <i>b</i> to the socket. </summary>
		''' <param name="b"> the data to be written </param>
		''' <exception cref="SocketException"> If an I/O error has occurred. </exception>
		public void write(SByte b()) throws java.io.IOException
			socketWrite(b, 0, b.length)

		''' <summary>
		''' Writes <i>length</i> bytes from buffer <i>b</i> starting at
		''' offset <i>len</i>. </summary>
		''' <param name="b"> the data to be written </param>
		''' <param name="off"> the start offset in the data </param>
		''' <param name="len"> the number of bytes that are written </param>
		''' <exception cref="SocketException"> If an I/O error has occurred. </exception>
		public void write(SByte b() , Integer off, Integer len) throws java.io.IOException
			socketWrite(b, off, len)

		''' <summary>
		''' Closes the stream.
		''' </summary>
		private Boolean closing = False
		public void close() throws java.io.IOException
			' Prevent recursion. See BugId 4484411
			If closing Then Return
			closing = True
			If socket_Renamed IsNot Nothing Then
				If Not socket_Renamed.closed Then socket_Renamed.close()
			Else
				impl.close()
			End If
			closing = False

		''' <summary>
		''' Overrides finalize, the fd is closed by the Socket.
		''' </summary>
		protected void Finalize()

		''' <summary>
		''' Perform class load-time initializations.
		''' </summary>
		private native static void init()

	End Class

End Namespace