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
    ''' This class represents a datagram packet.
    ''' <p>
    ''' Datagram packets are used to implement a connectionless packet
    ''' delivery service. Each message is routed from one machine to
    ''' another based solely on information contained within that packet.
    ''' Multiple packets sent from one machine to another might be routed
    ''' differently, and might arrive in any order. Packet delivery is
    ''' not guaranteed.
    '''
    ''' @author  Pavani Diwanji
    ''' @author  Benjamin Renaud
    ''' @since   JDK1.0
    ''' </summary>
    Public NotInheritable Class DatagramPacket

        ''' <summary>
        ''' Perform class initialization
        ''' </summary>
        Shared Sub New()
            ' java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper)
            '  init()
        End Sub

        Private Class CookiePolicyAnonymousInnerClassHelper
            Implements CookiePolicy

            Public Overridable Function shouldAccept(uri As URI, cookie As HttpCookie) As Boolean Implements CookiePolicy.shouldAccept
                Return True
            End Function
        End Class

        Private Class CookiePolicyAnonymousInnerClassHelper2
            Implements CookiePolicy

            Public Overridable Function shouldAccept(uri As URI, cookie As HttpCookie) As Boolean Implements CookiePolicy.shouldAccept
                Return False
            End Function
        End Class

        Private Class CookiePolicyAnonymousInnerClassHelper3
            Implements CookiePolicy

            Public Overridable Function shouldAccept(uri As URI, cookie As HttpCookie) As Boolean Implements CookiePolicy.shouldAccept
                If uri Is Nothing OrElse cookie Is Nothing Then Return False
                Return HttpCookie.domainMatches(cookie.domain, uri.host)
            End Function
        End Class

        Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
            Implements PrivilegedAction(Of T)

            Public Overridable Function run() As Void
                'JAVA TO VB CONVERTER TODO TASK: The library is specified in the 'DllImport' attribute for .NET:
                '				System.loadLibrary("net")
                Return Nothing
            End Function
        End Class

        '
        '     * The fields of this class are package-private since DatagramSocketImpl
        '     * classes needs to access them.
        '
        Friend buf As SByte()
        Friend offset As Integer
        Friend length As Integer
        Friend bufLength As Integer
        Friend address As InetAddress
        Friend port As Integer

        ''' <summary>
        ''' Constructs a {@code DatagramPacket} for receiving packets of
        ''' length {@code length}, specifying an offset into the buffer.
        ''' <p>
        ''' The {@code length} argument must be less than or equal to
        ''' {@code buf.length}.
        ''' </summary>
        ''' <param name="buf">      buffer for holding the incoming datagram. </param>
        ''' <param name="offset">   the offset for the buffer </param>
        ''' <param name="length">   the number of bytes to read.
        '''
        ''' @since 1.2 </param>
        Sub New(byte buf() , int offset, int length)
			dataata(buf, offset, length)
            Me.address = Nothing
            Me.port = -1
        End Sub

        ''' <summary>
        ''' Constructs a {@code DatagramPacket} for receiving packets of
        ''' length {@code length}.
        ''' <p>
        ''' The {@code length} argument must be less than or equal to
        ''' {@code buf.length}.
        ''' </summary>
        ''' <param name="buf">      buffer for holding the incoming datagram. </param>
        ''' <param name="length">   the number of bytes to read. </param>
        Sub New(SByte buf() , Integer length)
			Me.New(buf, 0, length)
        End Sub

        ''' <summary>
        ''' Constructs a datagram packet for sending packets of length
        ''' {@code length} with offset {@code ioffset}to the
        ''' specified port number on the specified host. The
        ''' {@code length} argument must be less than or equal to
        ''' {@code buf.length}.
        ''' </summary>
        ''' <param name="buf">      the packet data. </param>
        ''' <param name="offset">   the packet data offset. </param>
        ''' <param name="length">   the packet data length. </param>
        ''' <param name="address">  the destination address. </param>
        ''' <param name="port">     the destination port number. </param>
        ''' <seealso cref= java.net.InetAddress
        '''
        ''' @since 1.2 </seealso>
        Sub New(SByte buf() , Integer offset, Integer length, InetAddress address, Integer port)
			dataata(buf, offset, length)
            address = address
            port = port
        End Sub

        ''' <summary>
        ''' Constructs a datagram packet for sending packets of length
        ''' {@code length} with offset {@code ioffset}to the
        ''' specified port number on the specified host. The
        ''' {@code length} argument must be less than or equal to
        ''' {@code buf.length}.
        ''' </summary>
        ''' <param name="buf">      the packet data. </param>
        ''' <param name="offset">   the packet data offset. </param>
        ''' <param name="length">   the packet data length. </param>
        ''' <param name="address">  the destination socket address. </param>
        ''' <exception cref="IllegalArgumentException"> if address type is not supported </exception>
        ''' <seealso cref= java.net.InetAddress
        '''
        ''' @since 1.4 </seealso>
        Sub New(SByte buf() , Integer offset, Integer length, SocketAddress address)
			dataata(buf, offset, length)
            SocketAddress = address
        End Sub

        ''' <summary>
        ''' Constructs a datagram packet for sending packets of length
        ''' {@code length} to the specified port number on the specified
        ''' host. The {@code length} argument must be less than or equal
        ''' to {@code buf.length}.
        ''' </summary>
        ''' <param name="buf">      the packet data. </param>
        ''' <param name="length">   the packet length. </param>
        ''' <param name="address">  the destination address. </param>
        ''' <param name="port">     the destination port number. </param>
        ''' <seealso cref=     java.net.InetAddress </seealso>
        Sub New(SByte buf() , Integer length, InetAddress address, Integer port)
			Me(buf, 0, length, address, port)
        End Sub

        ''' <summary>
        ''' Constructs a datagram packet for sending packets of length
        ''' {@code length} to the specified port number on the specified
        ''' host. The {@code length} argument must be less than or equal
        ''' to {@code buf.length}.
        ''' </summary>
        ''' <param name="buf">      the packet data. </param>
        ''' <param name="length">   the packet length. </param>
        ''' <param name="address">  the destination address. </param>
        ''' <exception cref="IllegalArgumentException"> if address type is not supported
        ''' @since 1.4 </exception>
        ''' <seealso cref=     java.net.InetAddress </seealso>
        Sub New(SByte buf() , Integer length, SocketAddress address)
			Me(buf, 0, length, address)
        End Sub

        ''' <summary>
        ''' Returns the IP address of the machine to which this datagram is being
        ''' sent or from which the datagram was received.
        ''' </summary>
        ''' <returns>  the IP address of the machine to which this datagram is being
        '''          sent or from which the datagram was received. </returns>
        ''' <seealso cref=     java.net.InetAddress </seealso>
        ''' <seealso cref= #setAddress(java.net.InetAddress) </seealso>
        Public InetAddress address
			Return address

        ''' <summary>
        ''' Returns the port number on the remote host to which this datagram is
        ''' being sent or from which the datagram was received.
        ''' </summary>
        ''' <returns>  the port number on the remote host to which this datagram is
        '''          being sent or from which the datagram was received. </returns>
        ''' <seealso cref= #setPort(int) </seealso>
        Public  Integer port
			Return port

        ''' <summary>
        ''' Returns the data buffer. The data received or the data to be sent
        ''' starts from the {@code offset} in the buffer,
        ''' and runs for {@code length} java.lang.[Long].
        ''' </summary>
        ''' <returns>  the buffer used to receive or  send data </returns>
        ''' <seealso cref= #setData(byte[], int, int) </seealso>
        Public  SByte() data
			Return buf

        ''' <summary>
        ''' Returns the offset of the data to be sent or the offset of the
        ''' data received.
        ''' </summary>
        ''' <returns>  the offset of the data to be sent or the offset of the
        '''          data received.
        '''
        ''' @since 1.2 </returns>
        Public  Integer offset
			Return offset

        ''' <summary>
        ''' Returns the length of the data to be sent or the length of the
        ''' data received.
        ''' </summary>
        ''' <returns>  the length of the data to be sent or the length of the
        '''          data received. </returns>
        ''' <seealso cref= #setLength(int) </seealso>
        Public  Integer length
			Return length

        ''' <summary>
        ''' Set the data buffer for this packet. This sets the
        ''' data, length and offset of the packet.
        ''' </summary>
        ''' <param name="buf"> the buffer to set for this packet
        ''' </param>
        ''' <param name="offset"> the offset into the data
        ''' </param>
        ''' <param name="length"> the length of the data
        '''       and/or the length of the buffer used to receive data
        ''' </param>
        ''' <exception cref="NullPointerException"> if the argument is null
        ''' </exception>
        ''' <seealso cref= #getData </seealso>
        ''' <seealso cref= #getOffset </seealso>
        ''' <seealso cref= #getLength
        '''
        ''' @since 1.2 </seealso>
        Public Sub dataata(SByte() buf, Integer offset, Integer length)
            ' this will check to see if buf is null
            If length < 0 OrElse offset < 0 OrElse (length + offset) < 0 OrElse ((length + offset) > buf.Length) Then Throw New IllegalArgumentException("illegal length or offset")
            Me.buf = buf
            Me.length = length
            Me.bufLength = length
            Me.offset = offset

        ''' <summary>
        ''' Sets the IP address of the machine to which this datagram
        ''' is being sent. </summary>
        ''' <param name="iaddr"> the {@code InetAddress}
        ''' @since   JDK1.1 </param>
        ''' <seealso cref= #getAddress() </seealso>
        Public Sub addressess(InetAddress iaddr)
            address = iaddr

        ''' <summary>
        ''' Sets the port number on the remote host to which this datagram
        ''' is being sent. </summary>
        ''' <param name="iport"> the port number
        ''' @since   JDK1.1 </param>
        ''' <seealso cref= #getPort() </seealso>
        Public Sub portort(Integer iport)
            If iport < 0 OrElse iport > &HFFFF Then Throw New IllegalArgumentException("Port out of range:" & iport)
            port = iport

        ''' <summary>
        ''' Sets the SocketAddress (usually IP address + port number) of the remote
        ''' host to which this datagram is being sent.
        ''' </summary>
        ''' <param name="address"> the {@code SocketAddress} </param>
        ''' <exception cref="IllegalArgumentException"> if address is null or is a
        '''          SocketAddress subclass not supported by this socket
        '''
        ''' @since 1.4 </exception>
        ''' <seealso cref= #getSocketAddress </seealso>
        Public Sub socketAddressess(SocketAddress address)
            If address Is Nothing OrElse Not (TypeOf address Is InetSocketAddress) Then Throw New IllegalArgumentException("unsupported address type")
            Dim addr As InetSocketAddress = CType(address, InetSocketAddress)
            If addr.unresolved Then Throw New IllegalArgumentException("unresolved address")
            address = addr.address
            port = addr.port

            ''' <summary>
            ''' Gets the SocketAddress (usually IP address + port number) of the remote
            ''' host that this packet is being sent to or is coming from.
            ''' </summary>
            ''' <returns> the {@code SocketAddress}
            ''' @since 1.4 </returns>
            ''' <seealso cref= #setSocketAddress </seealso>
            Public SocketAddress socketAddress
			Return New InetSocketAddress(address, port)

        ''' <summary>
        ''' Set the data buffer for this packet. With the offset of
        ''' this DatagramPacket set to 0, and the length set to
        ''' the length of {@code buf}.
        ''' </summary>
        ''' <param name="buf"> the buffer to set for this packet.
        ''' </param>
        ''' <exception cref="NullPointerException"> if the argument is null.
        ''' </exception>
        ''' <seealso cref= #getLength </seealso>
        ''' <seealso cref= #getData
        '''
        ''' @since JDK1.1 </seealso>
        Public Sub dataata(SByte() buf)
            If buf Is Nothing Then Throw New NullPointerException("null packet buffer")
            Me.buf = buf
            Me.offset = 0
            Me.length = buf.Length
            Me.bufLength = buf.Length

        ''' <summary>
        ''' Set the length for this packet. The length of the packet is
        ''' the number of bytes from the packet's data buffer that will be
        ''' sent, or the number of bytes of the packet's data buffer that
        ''' will be used for receiving data. The length must be lesser or
        ''' equal to the offset plus the length of the packet's buffer.
        ''' </summary>
        ''' <param name="length"> the length to set for this packet.
        ''' </param>
        ''' <exception cref="IllegalArgumentException"> if the length is negative
        ''' of if the length is greater than the packet's data buffer
        ''' length.
        ''' </exception>
        ''' <seealso cref= #getLength </seealso>
        ''' <seealso cref= #setData
        '''
        ''' @since JDK1.1 </seealso>
        Public   Sub  lengthgth(Integer length)
			If (length + offset) > buf.Length OrElse length < 0 OrElse (length + offset) < 0 Then Throw New IllegalArgumentException("illegal length")
			Me.length = length
			Me.bufLength = Me.length

		''' <summary>
		''' Perform class load-time initializations.
		''' </summary>
		private native static  Sub  init()
	End Class

End Namespace