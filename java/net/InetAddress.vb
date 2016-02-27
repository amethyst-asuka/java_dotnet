Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics
Imports System.Collections.Generic
Imports System.Runtime.InteropServices
Imports sun.security.action
Imports sun.net.spi.nameservice
Imports java.security

'
' * Copyright (c) 1995, 2015, Oracle and/or its affiliates. All rights reserved.
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
    ''' This class represents an Internet Protocol (IP) address.
    ''' 
    ''' <p> An IP address is either a 32-bit or 128-bit unsigned number
    ''' used by IP, a lower-level protocol on which protocols like UDP and
    ''' TCP are built. The IP address architecture is defined by <a
    ''' href="http://www.ietf.org/rfc/rfc790.txt"><i>RFC&nbsp;790:
    ''' Assigned Numbers</i></a>, <a
    ''' href="http://www.ietf.org/rfc/rfc1918.txt"> <i>RFC&nbsp;1918:
    ''' Address Allocation for Private Internets</i></a>, <a
    ''' href="http://www.ietf.org/rfc/rfc2365.txt"><i>RFC&nbsp;2365:
    ''' Administratively Scoped IP Multicast</i></a>, and <a
    ''' href="http://www.ietf.org/rfc/rfc2373.txt"><i>RFC&nbsp;2373: IP
    ''' Version 6 Addressing Architecture</i></a>. An instance of an
    ''' InetAddress consists of an IP address and possibly its
    ''' corresponding host name (depending on whether it is constructed
    ''' with a host name or whether it has already done reverse host name
    ''' resolution).
    ''' 
    ''' <h3> Address types </h3>
    ''' 
    ''' <blockquote><table cellspacing=2 summary="Description of unicast and multicast address types">
    '''   <tr><th valign=top><i>unicast</i></th>
    '''       <td>An identifier for a single interface. A packet sent to
    '''         a unicast address is delivered to the interface identified by
    '''         that address.
    ''' 
    '''         <p> The Unspecified Address -- Also called anylocal or wildcard
    '''         address. It must never be assigned to any node. It indicates the
    '''         absence of an address. One example of its use is as the target of
    '''         bind, which allows a server to accept a client connection on any
    '''         interface, in case the server host has multiple interfaces.
    ''' 
    '''         <p> The <i>unspecified</i> address must not be used as
    '''         the destination address of an IP packet.
    ''' 
    '''         <p> The <i>Loopback</i> Addresses -- This is the address
    '''         assigned to the loopback interface. Anything sent to this
    '''         IP address loops around and becomes IP input on the local
    '''         host. This address is often used when testing a
    '''         client.</td></tr>
    '''   <tr><th valign=top><i>multicast</i></th>
    '''       <td>An identifier for a set of interfaces (typically belonging
    '''         to different nodes). A packet sent to a multicast address is
    '''         delivered to all interfaces identified by that address.</td></tr>
    ''' </table></blockquote>
    ''' 
    ''' <h4> IP address scope </h4>
    ''' 
    ''' <p> <i>Link-local</i> addresses are designed to be used for addressing
    ''' on a single link for purposes such as auto-address configuration,
    ''' neighbor discovery, or when no routers are present.
    ''' 
    ''' <p> <i>Site-local</i> addresses are designed to be used for addressing
    ''' inside of a site without the need for a global prefix.
    ''' 
    ''' <p> <i>Global</i> addresses are unique across the internet.
    ''' 
    ''' <h4> Textual representation of IP addresses </h4>
    ''' 
    ''' The textual representation of an IP address is address family specific.
    ''' 
    ''' <p>
    ''' 
    ''' For IPv4 address format, please refer to <A
    ''' HREF="Inet4Address.html#format">Inet4Address#format</A>; For IPv6
    ''' address format, please refer to <A
    ''' HREF="Inet6Address.html#format">Inet6Address#format</A>.
    ''' 
    ''' <P>There is a <a href="doc-files/net-properties.html#Ipv4IPv6">couple of
    ''' System Properties</a> affecting how IPv4 and IPv6 addresses are used.</P>
    ''' 
    ''' <h4> Host Name Resolution </h4>
    ''' 
    ''' Host name-to-IP address <i>resolution</i> is accomplished through
    ''' the use of a combination of local machine configuration information
    ''' and network naming services such as the Domain Name System (DNS)
    ''' and Network Information Service(NIS). The particular naming
    ''' services(s) being used is by default the local machine configured
    ''' one. For any host name, its corresponding IP address is returned.
    ''' 
    ''' <p> <i>Reverse name resolution</i> means that for any IP address,
    ''' the host associated with the IP address is returned.
    ''' 
    ''' <p> The InetAddress class provides methods to resolve host names to
    ''' their IP addresses and vice versa.
    ''' 
    ''' <h4> InetAddress Caching </h4>
    ''' 
    ''' The InetAddress class has a cache to store successful as well as
    ''' unsuccessful host name resolutions.
    ''' 
    ''' <p> By default, when a security manager is installed, in order to
    ''' protect against DNS spoofing attacks,
    ''' the result of positive host name resolutions are
    ''' cached forever. When a security manager is not installed, the default
    ''' behavior is to cache entries for a finite (implementation dependent)
    ''' period of time. The result of unsuccessful host
    ''' name resolution is cached for a very short period of time (10
    ''' seconds) to improve performance.
    ''' 
    ''' <p> If the default behavior is not desired, then a Java security property
    ''' can be set to a different Time-to-live (TTL) value for positive
    ''' caching. Likewise, a system admin can configure a different
    ''' negative caching TTL value when needed.
    ''' 
    ''' <p> Two Java security properties control the TTL values used for
    '''  positive and negative host name resolution caching:
    ''' 
    ''' <blockquote>
    ''' <dl>
    ''' <dt><b>networkaddress.cache.ttl</b></dt>
    ''' <dd>Indicates the caching policy for successful name lookups from
    ''' the name service. The value is specified as as integer to indicate
    ''' the number of seconds to cache the successful lookup. The default
    ''' setting is to cache for an implementation specific period of time.
    ''' <p>
    ''' A value of -1 indicates "cache forever".
    ''' </dd>
    ''' <dt><b>networkaddress.cache.negative.ttl</b> (default: 10)</dt>
    ''' <dd>Indicates the caching policy for un-successful name lookups
    ''' from the name service. The value is specified as as integer to
    ''' indicate the number of seconds to cache the failure for
    ''' un-successful lookups.
    ''' <p>
    ''' A value of 0 indicates "never cache".
    ''' A value of -1 indicates "cache forever".
    ''' </dd>
    ''' </dl>
    ''' </blockquote>
    ''' 
    ''' @author  Chris Warth </summary>
    ''' <seealso cref=     java.net.InetAddress#getByAddress(byte[]) </seealso>
    ''' <seealso cref=     java.net.InetAddress#getByAddress(java.lang.String, byte[]) </seealso>
    ''' <seealso cref=     java.net.InetAddress#getAllByName(java.lang.String) </seealso>
    ''' <seealso cref=     java.net.InetAddress#getByName(java.lang.String) </seealso>
    ''' <seealso cref=     java.net.InetAddress#getLocalHost()
    ''' @since JDK1.0 </seealso>
    <Serializable>
    Public Class InetAddress
        ''' <summary>
        ''' Specify the address family: Internet Protocol, Version 4
        ''' @since 1.4
        ''' </summary>
        Friend Const IPv4 As Integer = 1

        ''' <summary>
        ''' Specify the address family: Internet Protocol, Version 6
        ''' @since 1.4
        ''' </summary>
        Friend Const IPv6 As Integer = 2

        ' Specify address family preference 
        <NonSerialized>
        Friend Shared preferIPv6Address As Boolean = False

        Friend Class InetAddressHolder

            Friend Sub New()
            End Sub

            Friend Sub New(ByVal hostName As String, ByVal address As Integer, ByVal family As Integer)
                Me.originalHostName = hostName
                Me.hostName = hostName
                Me.address = address
                Me.family = family
            End Sub

            Friend Overridable Sub init(ByVal hostName As String, ByVal family As Integer)
                Me.originalHostName = hostName
                Me.hostName = hostName
                If family <> -1 Then Me._family = family
            End Sub

            Friend Overridable Property hostName As String

            ''' <summary>
            ''' Reserve the original application specified hostname.
            ''' 
            ''' The original hostname is useful for domain-based endpoint
            ''' identification (see RFC 2818 and RFC 6125).  If an address
            ''' was created with a raw IP address, a reverse name lookup
            ''' may introduce endpoint identification security issue via
            ''' DNS forging.
            ''' 
            ''' Oracle JSSE provider is using this original hostname, via
            ''' sun.misc.JavaNetAccess, for SSL/TLS endpoint identification.
            ''' 
            ''' Note: May define a new public method in the future if necessary.
            ''' </summary>
            Friend Overridable Property originalHostName As String

            ''' <summary>
            ''' Holds a 32-bit IPv4 address.
            ''' </summary>
            Friend Overridable ReadOnly Property address As Integer

            ''' <summary>
            ''' Specifies the address family type, for instance, '1' for IPv4
            ''' addresses, and '2' for IPv6 addresses.
            ''' </summary>
            Friend Overridable ReadOnly Property family As Integer
        End Class

        ' Used to store the serializable fields of InetAddress 
        <NonSerialized>
        Friend ReadOnly holder_Renamed As InetAddressHolder

        Friend Overridable Function holder() As InetAddressHolder
            Return holder_Renamed
        End Function

        ' Used to store the name service provider 
        Private Shared nameServices As IList(Of NameService) = Nothing

        ' Used to store the best available hostname 
        <NonSerialized>
        Private canonicalHostName As String = Nothing

        ''' <summary>
        ''' use serialVersionUID from JDK 1.0.2 for interoperability </summary>
        Private Const serialVersionUID As Long = 3286316764910316507L

        '    
        '     * Load net library into runtime, and perform initializations.
        '     
        Shared Sub New()
            preferIPv6Address = java.security.AccessController.doPrivileged(New GetBooleanAction("java.net.preferIPv6Addresses"))
            java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
            init()
            ' create the impl
            impl = InetAddressImplFactory.create()

            ' get name service if provided and requested
            Dim provider As String = Nothing
            Dim propPrefix As String = "sun.net.spi.nameservice.provider."
            Dim n As Integer = 1
            nameServices = New List(Of NameService)
            provider = java.security.AccessController.doPrivileged(New GetPropertyAction(propPrefix + n))
            Do While provider IsNot Nothing
                Dim ns As NameService = createNSProvider(provider)
                If ns IsNot Nothing Then nameServices.Add(ns)

                n += 1
                provider = java.security.AccessController.doPrivileged(New GetPropertyAction(propPrefix + n))
            Loop

            ' if not designate any name services provider,
            ' create a default one
            If nameServices.Count = 0 Then
                Dim ns As NameService = createNSProvider("default")
                nameServices.Add(ns)
            End If
            Try
                Dim unsafe_Renamed As sun.misc.Unsafe = sun.misc.Unsafe.unsafe
                FIELDS_OFFSET = unsafe_Renamed.objectFieldOffset(GetType(InetAddress).getDeclaredField("holder"))
                UNSAFE = unsafe_Renamed
            Catch e As ReflectiveOperationException
                Throw New [Error](e)
            End Try
        End Sub

        Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
            Implements PrivilegedAction(Of T)

            Public Overridable Function run() As Void
                'JAVA TO VB CONVERTER TODO TASK: The library is specified in the 'DllImport' attribute for .NET:
                '				System.loadLibrary("net")
                Return Nothing
            End Function
        End Class

        ''' <summary>
        ''' Constructor for the Socket.accept() method.
        ''' This creates an empty InetAddress, which is filled in by
        ''' the accept() method.  This InetAddress, however, is not
        ''' put in the address cache, since it is not created by name.
        ''' </summary>
        Friend Sub New()
            holder_Renamed = New InetAddressHolder
        End Sub

        ''' <summary>
        ''' Replaces the de-serialized object with an Inet4Address object.
        ''' </summary>
        ''' <returns> the alternate object to the de-serialized object.
        ''' </returns>
        ''' <exception cref="ObjectStreamException"> if a new object replacing this
        ''' object could not be created </exception>
        Private Function readResolve() As Object
            ' will replace the deserialized 'this' object
            Return New Inet4Address(holder().hostName, holder().address)
        End Function

        ''' <summary>
        ''' Utility routine to check if the InetAddress is an
        ''' IP multicast address. </summary>
        ''' <returns> a {@code boolean} indicating if the InetAddress is
        ''' an IP multicast address
        ''' @since   JDK1.1 </returns>
        Public Overridable Property multicastAddress As Boolean
            Get
                Return False
            End Get
        End Property

        ''' <summary>
        ''' Utility routine to check if the InetAddress in a wildcard address. </summary>
        ''' <returns> a {@code boolean} indicating if the Inetaddress is
        '''         a wildcard address.
        ''' @since 1.4 </returns>
        Public Overridable Property anyLocalAddress As Boolean
            Get
                Return False
            End Get
        End Property

        ''' <summary>
        ''' Utility routine to check if the InetAddress is a loopback address.
        ''' </summary>
        ''' <returns> a {@code boolean} indicating if the InetAddress is
        ''' a loopback address; or false otherwise.
        ''' @since 1.4 </returns>
        Public Overridable Property loopbackAddress As Boolean
            Get
                Return False
            End Get
        End Property

        ''' <summary>
        ''' Utility routine to check if the InetAddress is an link local address.
        ''' </summary>
        ''' <returns> a {@code boolean} indicating if the InetAddress is
        ''' a link local address; or false if address is not a link local unicast address.
        ''' @since 1.4 </returns>
        Public Overridable Property linkLocalAddress As Boolean
            Get
                Return False
            End Get
        End Property

        ''' <summary>
        ''' Utility routine to check if the InetAddress is a site local address.
        ''' </summary>
        ''' <returns> a {@code boolean} indicating if the InetAddress is
        ''' a site local address; or false if address is not a site local unicast address.
        ''' @since 1.4 </returns>
        Public Overridable Property siteLocalAddress As Boolean
            Get
                Return False
            End Get
        End Property

        ''' <summary>
        ''' Utility routine to check if the multicast address has global scope.
        ''' </summary>
        ''' <returns> a {@code boolean} indicating if the address has
        '''         is a multicast address of global scope, false if it is not
        '''         of global scope or it is not a multicast address
        ''' @since 1.4 </returns>
        Public Overridable Property mCGlobal As Boolean
            Get
                Return False
            End Get
        End Property

        ''' <summary>
        ''' Utility routine to check if the multicast address has node scope.
        ''' </summary>
        ''' <returns> a {@code boolean} indicating if the address has
        '''         is a multicast address of node-local scope, false if it is not
        '''         of node-local scope or it is not a multicast address
        ''' @since 1.4 </returns>
        Public Overridable Property mCNodeLocal As Boolean
            Get
                Return False
            End Get
        End Property

        ''' <summary>
        ''' Utility routine to check if the multicast address has link scope.
        ''' </summary>
        ''' <returns> a {@code boolean} indicating if the address has
        '''         is a multicast address of link-local scope, false if it is not
        '''         of link-local scope or it is not a multicast address
        ''' @since 1.4 </returns>
        Public Overridable Property mCLinkLocal As Boolean
            Get
                Return False
            End Get
        End Property

        ''' <summary>
        ''' Utility routine to check if the multicast address has site scope.
        ''' </summary>
        ''' <returns> a {@code boolean} indicating if the address has
        '''         is a multicast address of site-local scope, false if it is not
        '''         of site-local scope or it is not a multicast address
        ''' @since 1.4 </returns>
        Public Overridable Property mCSiteLocal As Boolean
            Get
                Return False
            End Get
        End Property

        ''' <summary>
        ''' Utility routine to check if the multicast address has organization scope.
        ''' </summary>
        ''' <returns> a {@code boolean} indicating if the address has
        '''         is a multicast address of organization-local scope,
        '''         false if it is not of organization-local scope
        '''         or it is not a multicast address
        ''' @since 1.4 </returns>
        Public Overridable Property mCOrgLocal As Boolean
            Get
                Return False
            End Get
        End Property


        ''' <summary>
        ''' Test whether that address is reachable. Best effort is made by the
        ''' implementation to try to reach the host, but firewalls and server
        ''' configuration may block requests resulting in a unreachable status
        ''' while some specific ports may be accessible.
        ''' A typical implementation will use ICMP ECHO REQUESTs if the
        ''' privilege can be obtained, otherwise it will try to establish
        ''' a TCP connection on port 7 (Echo) of the destination host.
        ''' <p>
        ''' The timeout value, in milliseconds, indicates the maximum amount of time
        ''' the try should take. If the operation times out before getting an
        ''' answer, the host is deemed unreachable. A negative value will result
        ''' in an IllegalArgumentException being thrown.
        ''' </summary>
        ''' <param name="timeout"> the time, in milliseconds, before the call aborts </param>
        ''' <returns> a {@code boolean} indicating if the address is reachable. </returns>
        ''' <exception cref="IOException"> if a network error occurs </exception>
        ''' <exception cref="IllegalArgumentException"> if {@code timeout} is negative.
        ''' @since 1.5 </exception>
        Public Overridable Function isReachable(ByVal timeout As Integer) As Boolean
            Return isReachable(Nothing, 0, timeout)
        End Function

        ''' <summary>
        ''' Test whether that address is reachable. Best effort is made by the
        ''' implementation to try to reach the host, but firewalls and server
        ''' configuration may block requests resulting in a unreachable status
        ''' while some specific ports may be accessible.
        ''' A typical implementation will use ICMP ECHO REQUESTs if the
        ''' privilege can be obtained, otherwise it will try to establish
        ''' a TCP connection on port 7 (Echo) of the destination host.
        ''' <p>
        ''' The {@code network interface} and {@code ttl} parameters
        ''' let the caller specify which network interface the test will go through
        ''' and the maximum number of hops the packets should go through.
        ''' A negative value for the {@code ttl} will result in an
        ''' IllegalArgumentException being thrown.
        ''' <p>
        ''' The timeout value, in milliseconds, indicates the maximum amount of time
        ''' the try should take. If the operation times out before getting an
        ''' answer, the host is deemed unreachable. A negative value will result
        ''' in an IllegalArgumentException being thrown.
        ''' </summary>
        ''' <param name="netif">   the NetworkInterface through which the
        '''                    test will be done, or null for any interface </param>
        ''' <param name="ttl">     the maximum numbers of hops to try or 0 for the
        '''                  default </param>
        ''' <param name="timeout"> the time, in milliseconds, before the call aborts </param>
        ''' <exception cref="IllegalArgumentException"> if either {@code timeout}
        '''                          or {@code ttl} are negative. </exception>
        ''' <returns> a {@code boolean}indicating if the address is reachable. </returns>
        ''' <exception cref="IOException"> if a network error occurs
        ''' @since 1.5 </exception>
        Public Overridable Function isReachable(ByVal netif As NetworkInterface, ByVal ttl As Integer, ByVal timeout As Integer) As Boolean
            If ttl < 0 Then Throw New IllegalArgumentException("ttl can't be negative")
            If timeout < 0 Then Throw New IllegalArgumentException("timeout can't be negative")

            Return impl.isReachable(Me, timeout, netif, ttl)
        End Function

        ''' <summary>
        ''' Gets the host name for this IP address.
        ''' 
        ''' <p>If this InetAddress was created with a host name,
        ''' this host name will be remembered and returned;
        ''' otherwise, a reverse name lookup will be performed
        ''' and the result will be returned based on the system
        ''' configured name lookup service. If a lookup of the name service
        ''' is required, call
        ''' <seealso cref="#getCanonicalHostName() getCanonicalHostName"/>.
        ''' 
        ''' <p>If there is a security manager, its
        ''' {@code checkConnect} method is first called
        ''' with the hostname and {@code -1}
        ''' as its arguments to see if the operation is allowed.
        ''' If the operation is not allowed, it will return
        ''' the textual representation of the IP address.
        ''' </summary>
        ''' <returns>  the host name for this IP address, or if the operation
        '''    is not allowed by the security check, the textual
        '''    representation of the IP address.
        ''' </returns>
        ''' <seealso cref= InetAddress#getCanonicalHostName </seealso>
        ''' <seealso cref= SecurityManager#checkConnect </seealso>
        Public Overridable Property hostName As String
            Get
                Return getHostName(True)
            End Get
        End Property

        ''' <summary>
        ''' Returns the hostname for this address.
        ''' If the host is equal to null, then this address refers to any
        ''' of the local machine's available network addresses.
        ''' this is package private so SocketPermission can make calls into
        ''' here without a security check.
        ''' 
        ''' <p>If there is a security manager, this method first
        ''' calls its {@code checkConnect} method
        ''' with the hostname and {@code -1}
        ''' as its arguments to see if the calling code is allowed to know
        ''' the hostname for this IP address, i.e., to connect to the host.
        ''' If the operation is not allowed, it will return
        ''' the textual representation of the IP address.
        ''' </summary>
        ''' <returns>  the host name for this IP address, or if the operation
        '''    is not allowed by the security check, the textual
        '''    representation of the IP address.
        ''' </returns>
        ''' <param name="check"> make security check if true
        ''' </param>
        ''' <seealso cref= SecurityManager#checkConnect </seealso>
        Friend Overridable Function getHostName(ByVal check As Boolean) As String
            If holder().hostName Is Nothing Then holder().hostName = InetAddress.getHostFromNameService(Me, check)
            Return holder().hostName
        End Function

        ''' <summary>
        ''' Gets the fully qualified domain name for this IP address.
        ''' Best effort method, meaning we may not be able to return
        ''' the FQDN depending on the underlying system configuration.
        ''' 
        ''' <p>If there is a security manager, this method first
        ''' calls its {@code checkConnect} method
        ''' with the hostname and {@code -1}
        ''' as its arguments to see if the calling code is allowed to know
        ''' the hostname for this IP address, i.e., to connect to the host.
        ''' If the operation is not allowed, it will return
        ''' the textual representation of the IP address.
        ''' </summary>
        ''' <returns>  the fully qualified domain name for this IP address,
        '''    or if the operation is not allowed by the security check,
        '''    the textual representation of the IP address.
        ''' </returns>
        ''' <seealso cref= SecurityManager#checkConnect
        ''' 
        ''' @since 1.4 </seealso>
        Public Overridable Property canonicalHostName As String
            Get
                If canonicalHostName Is Nothing Then canonicalHostName = InetAddress.getHostFromNameService(Me, True)
                Return canonicalHostName
            End Get
        End Property

        ''' <summary>
        ''' Returns the hostname for this address.
        ''' 
        ''' <p>If there is a security manager, this method first
        ''' calls its {@code checkConnect} method
        ''' with the hostname and {@code -1}
        ''' as its arguments to see if the calling code is allowed to know
        ''' the hostname for this IP address, i.e., to connect to the host.
        ''' If the operation is not allowed, it will return
        ''' the textual representation of the IP address.
        ''' </summary>
        ''' <returns>  the host name for this IP address, or if the operation
        '''    is not allowed by the security check, the textual
        '''    representation of the IP address.
        ''' </returns>
        ''' <param name="check"> make security check if true
        ''' </param>
        ''' <seealso cref= SecurityManager#checkConnect </seealso>
        Private Shared Function getHostFromNameService(ByVal addr As InetAddress, ByVal check As Boolean) As String
            Dim host As String = Nothing
            For Each nameService As NameService In nameServices
                Try
                    ' first lookup the hostname
                    host = nameService.getHostByAddr(addr.address)

                    '                 check to see if calling code is allowed to know
                    '                 * the hostname for this IP address, ie, connect to the host
                    '                 
                    If check Then
                        Dim sec As SecurityManager = System.securityManager
                        If sec IsNot Nothing Then sec.checkConnect(host, -1)
                    End If

                    '                 now get all the IP addresses for this hostname,
                    '                 * and make sure one of them matches the original IP
                    '                 * address. We do this to try and prevent spoofing.
                    '                 

                    Dim arr As InetAddress() = InetAddress.getAllByName0(host, check)
                    Dim ok As Boolean = False

                    If arr IsNot Nothing Then
                        Dim i As Integer = 0
                        Do While (Not ok) AndAlso i < arr.Length
                            ok = addr.Equals(arr(i))
                            i += 1
                        Loop
                    End If

                    'XXX: if it looks a spoof just return the address?
                    If Not ok Then
                        host = addr.hostAddress
                        Return host
                    End If

                    Exit For

                Catch e As SecurityException
                    host = addr.hostAddress
                    Exit For
                Catch e As UnknownHostException
                    host = addr.hostAddress
                    ' let next provider resolve the hostname
                End Try
            Next nameService

            Return host
        End Function

        ''' <summary>
        ''' Returns the raw IP address of this {@code InetAddress}
        ''' object. The result is in network byte order: the highest order
        ''' byte of the address is in {@code getAddress()[0]}.
        ''' </summary>
        ''' <returns>  the raw IP address of this object. </returns>
        Public Overridable Property address As SByte()
            Get
                Return Nothing
            End Get
        End Property

        ''' <summary>
        ''' Returns the IP address string in textual presentation.
        ''' </summary>
        ''' <returns>  the raw IP address in a string format.
        ''' @since   JDK1.0.2 </returns>
        Public Overridable Property hostAddress As String
            Get
                Return Nothing
            End Get
        End Property

        ''' <summary>
        ''' Returns a hashcode for this IP address.
        ''' </summary>
        ''' <returns>  a hash code value for this IP address. </returns>
        Public Overrides Function GetHashCode() As Integer
            Return -1
        End Function

        ''' <summary>
        ''' Compares this object against the specified object.
        ''' The result is {@code true} if and only if the argument is
        ''' not {@code null} and it represents the same IP address as
        ''' this object.
        ''' <p>
        ''' Two instances of {@code InetAddress} represent the same IP
        ''' address if the length of the byte arrays returned by
        ''' {@code getAddress} is the same for both, and each of the
        ''' array components is the same for the byte arrays.
        ''' </summary>
        ''' <param name="obj">   the object to compare against. </param>
        ''' <returns>  {@code true} if the objects are the same;
        '''          {@code false} otherwise. </returns>
        ''' <seealso cref=     java.net.InetAddress#getAddress() </seealso>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Return False
        End Function

        ''' <summary>
        ''' Converts this IP address to a {@code String}. The
        ''' string returned is of the form: hostname / literal IP
        ''' address.
        ''' 
        ''' If the host name is unresolved, no reverse name service lookup
        ''' is performed. The hostname part will be represented by an empty string.
        ''' </summary>
        ''' <returns>  a string representation of this IP address. </returns>
        Public Overrides Function ToString() As String
            Dim hostName_Renamed As String = holder().hostName
            Return (If(hostName_Renamed IsNot Nothing, hostName_Renamed, "")) & "/" & hostAddress
        End Function

        '    
        '     * Cached addresses - our own litle nis, not!
        '     
        Private Shared addressCache As New Cache(Cache.Type.Positive)

        Private Shared negativeCache As New Cache(Cache.Type.Negative)

        Private Shared addressCacheInit As Boolean = False

        Friend Shared unknown_array As InetAddress() ' put THIS in cache

        Friend Shared impl As InetAddressImpl

        Private Shared ReadOnly lookupTable As New Dictionary(Of String, Void)

        ''' <summary>
        ''' Represents a cache entry
        ''' </summary>
        Friend NotInheritable Class CacheEntry

            Friend Sub New(ByVal addresses As InetAddress(), ByVal expiration As Long)
                Me.addresses = addresses
                Me.expiration = expiration
            End Sub

            Friend addresses As InetAddress()
            Friend expiration As Long
        End Class

        ''' <summary>
        ''' A cache that manages entries based on a policy specified
        ''' at creation time.
        ''' </summary>
        Friend NotInheritable Class Cache
            Private cache_Renamed As java.util.LinkedHashMap(Of String, CacheEntry)
            Private type_Renamed As Type

            Friend Enum Type
                Positive
                Negative
            End Enum

            ''' <summary>
            ''' Create cache
            ''' </summary>
            Public Sub New(ByVal type As Type)
                Me.type_Renamed = type
                cache_Renamed = New java.util.LinkedHashMap(Of String, CacheEntry)
            End Sub

            Private Property policy As Integer
                Get
                    If type_Renamed = Type.Positive Then
                        Return sun.net.InetAddressCachePolicy.get()
                    Else
                        Return sun.net.InetAddressCachePolicy.negative
                    End If
                End Get
            End Property

            ''' <summary>
            ''' Add an entry to the cache. If there's already an
            ''' entry then for this host then the entry will be
            ''' replaced.
            ''' </summary>
            Public Function put(ByVal host As String, ByVal addresses As InetAddress()) As Cache
                Dim policy_Renamed As Integer = policy
                If policy_Renamed = sun.net.InetAddressCachePolicy.NEVER Then Return Me

                ' purge any expired entries

                If policy_Renamed <> sun.net.InetAddressCachePolicy.FOREVER Then

                    ' As we iterate in insertion order we can
                    ' terminate when a non-expired entry is found.
                    Dim expired As New LinkedList(Of String)
                    Dim now As Long = System.currentTimeMillis()
                    For Each key As String In cache_Renamed.Keys
                        Dim entry As CacheEntry = cache_Renamed(key)

                        If entry.expiration >= 0 AndAlso entry.expiration < now Then
                            expired.AddLast(key)
                        Else
                            Exit For
                        End If
                    Next key

                    For Each key As String In expired
                        cache_Renamed.remove(key)
                    Next key
                End If

                ' create new entry and add it to the cache
                ' -- as a HashMap replaces existing entries we
                '    don't need to explicitly check if there is
                '    already an entry for this host.
                Dim expiration As Long
                If policy_Renamed = sun.net.InetAddressCachePolicy.FOREVER Then
                    expiration = -1
                Else
                    expiration = System.currentTimeMillis() + (policy_Renamed * 1000)
                End If
                Dim entry As New CacheEntry(addresses, expiration)
                cache_Renamed(host) = entry
                Return Me
            End Function

            ''' <summary>
            ''' Query the cache for the specific host. If found then
            ''' return its CacheEntry, or null if not found.
            ''' </summary>
            Public Function [get](ByVal host As String) As CacheEntry
                Dim policy_Renamed As Integer = policy
                If policy_Renamed = sun.net.InetAddressCachePolicy.NEVER Then Return Nothing
                Dim entry As CacheEntry = cache_Renamed(host)

                ' check if entry has expired
                If entry IsNot Nothing AndAlso policy_Renamed <> sun.net.InetAddressCachePolicy.FOREVER Then
                    If entry.expiration >= 0 AndAlso entry.expiration < System.currentTimeMillis() Then
                        cache_Renamed.remove(host)
                        entry = Nothing
                    End If
                End If

                Return entry
            End Function
        End Class

        '    
        '     * Initialize cache and insert anyLocalAddress into the
        '     * unknown array with no expiry.
        '     
        Private Shared Sub cacheInitIfNeeded()
            Debug.Assert(Thread.holdsLock(addressCache))
            If addressCacheInit Then Return
            unknown_array = New InetAddress(0) {}
            unknown_array(0) = impl.anyLocalAddress()

            addressCache.put(impl.anyLocalAddress().hostName, unknown_array)

            addressCacheInit = True
        End Sub

        '    
        '     * Cache the given hostname and addresses.
        '     
        Private Shared Sub cacheAddresses(ByVal hostname As String, ByVal addresses As InetAddress(), ByVal success As Boolean)
            hostname = hostname.ToLower()
            SyncLock addressCache
                cacheInitIfNeeded()
                If success Then
                    addressCache.put(hostname, addresses)
                Else
                    negativeCache.put(hostname, addresses)
                End If
            End SyncLock
        End Sub

        '    
        '     * Lookup hostname in cache (positive & negative cache). If
        '     * found return addresses, null if not found.
        '     
        Private Shared Function getCachedAddresses(ByVal hostname As String) As InetAddress()
            hostname = hostname.ToLower()

            ' search both positive & negative caches

            SyncLock addressCache
                cacheInitIfNeeded()

                Dim entry As CacheEntry = addressCache.get(hostname)
                If entry Is Nothing Then entry = negativeCache.get(hostname)

                If entry IsNot Nothing Then Return entry.addresses
            End SyncLock

            ' not found
            Return Nothing
        End Function

        Private Shared Function createNSProvider(ByVal provider As String) As NameService
            If provider Is Nothing Then Return Nothing

            Dim nameService As NameService = Nothing
            If provider.Equals("default") Then
                ' initialize the default name service
                'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
                '				nameService = New NameService()
                '			{
                '				public InetAddress[] lookupAllHostAddr(String host) throws UnknownHostException
                '				{
                '					Return impl.lookupAllHostAddr(host);
                '				}
                '				public String getHostByAddr(byte[] addr) throws UnknownHostException
                '				{
                '					Return impl.getHostByAddr(addr);
                '				}
                '			};
            Else
                Dim providerName As String = provider
                Try
                    nameService = java.security.AccessController.doPrivileged(New PrivilegedExceptionActionAnonymousInnerClassHelper(Of T)
                   )
                Catch e As java.security.PrivilegedActionException
                End Try
            End If

            Return nameService
        End Function

        Private Class PrivilegedExceptionActionAnonymousInnerClassHelper(Of T)
            Implements PrivilegedExceptionAction(Of T)

            Public Overridable Function run() As NameService
                Dim itr As IEnumerator(Of NameServiceDescriptor) = java.util.ServiceLoader.load(GetType(NameServiceDescriptor)).GetEnumerator()
                Do While itr.MoveNext()
                    Dim nsd As NameServiceDescriptor = itr.Current
                    If providerName.equalsIgnoreCase(nsd.type & "," & nsd.providerName) Then
                        Try
                            Return nsd.createNameService()
                        Catch e As Exception
                            e.printStackTrace()
                            Console.Error.WriteLine("Cannot create name service:" & providerName & ": " & e)
                        End Try
                    End If
                Loop

                Return Nothing
            End Function
        End Class


        ''' <summary>
        ''' Creates an InetAddress based on the provided host name and IP address.
        ''' No name service is checked for the validity of the address.
        ''' 
        ''' <p> The host name can either be a machine name, such as
        ''' "{@code java.sun.com}", or a textual representation of its IP
        ''' address.
        ''' <p> No validity checking is done on the host name either.
        ''' 
        ''' <p> If addr specifies an IPv4 address an instance of Inet4Address
        ''' will be returned; otherwise, an instance of Inet6Address
        ''' will be returned.
        ''' 
        ''' <p> IPv4 address byte array must be 4 bytes long and IPv6 byte array
        ''' must be 16 bytes long
        ''' </summary>
        ''' <param name="host"> the specified host </param>
        ''' <param name="addr"> the raw IP address in network byte order </param>
        ''' <returns>  an InetAddress object created from the raw IP address. </returns>
        ''' <exception cref="UnknownHostException">  if IP address is of illegal length
        ''' @since 1.4 </exception>
        Public Shared Function getByAddress(ByVal host As String, ByVal addr As SByte()) As InetAddress
            If host IsNot Nothing AndAlso host.Length() > 0 AndAlso host.Chars(0) = "["c Then
                If host.Chars(host.Length() - 1) = "]"c Then host = host.Substring(1, host.Length() - 1 - 1)
            End If
            If addr IsNot Nothing Then
                If addr.Length = Inet4Address.INADDRSZ Then
                    Return New Inet4Address(host, addr)
                ElseIf addr.Length = Inet6Address.INADDRSZ Then
                    Dim newAddr As SByte() = sun.net.util.IPAddressUtil.convertFromIPv4MappedAddress(addr)
                    If newAddr IsNot Nothing Then
                        Return New Inet4Address(host, newAddr)
                    Else
                        Return New Inet6Address(host, addr)
                    End If
                End If
            End If
            Throw New UnknownHostException("addr is of illegal length")
        End Function


        ''' <summary>
        ''' Determines the IP address of a host, given the host's name.
        ''' 
        ''' <p> The host name can either be a machine name, such as
        ''' "{@code java.sun.com}", or a textual representation of its
        ''' IP address. If a literal IP address is supplied, only the
        ''' validity of the address format is checked.
        ''' 
        ''' <p> For {@code host} specified in literal IPv6 address,
        ''' either the form defined in RFC 2732 or the literal IPv6 address
        ''' format defined in RFC 2373 is accepted. IPv6 scoped addresses are also
        ''' supported. See <a href="Inet6Address.html#scoped">here</a> for a description of IPv6
        ''' scoped addresses.
        ''' 
        ''' <p> If the host is {@code null} then an {@code InetAddress}
        ''' representing an address of the loopback interface is returned.
        ''' See <a href="http://www.ietf.org/rfc/rfc3330.txt">RFC&nbsp;3330</a>
        ''' section&nbsp;2 and <a href="http://www.ietf.org/rfc/rfc2373.txt">RFC&nbsp;2373</a>
        ''' section&nbsp;2.5.3. </p>
        ''' </summary>
        ''' <param name="host">   the specified host, or {@code null}. </param>
        ''' <returns>     an IP address for the given host name. </returns>
        ''' <exception cref="UnknownHostException">  if no IP address for the
        '''               {@code host} could be found, or if a scope_id was specified
        '''               for a global IPv6 address. </exception>
        ''' <exception cref="SecurityException"> if a security manager exists
        '''             and its checkConnect method doesn't allow the operation </exception>
        Public Shared Function getByName(ByVal host As String) As InetAddress
            Return InetAddress.getAllByName(host)(0)
        End Function

        ' called from deployment cache manager
        Private Shared Function getByName(ByVal host As String, ByVal reqAddr As InetAddress) As InetAddress
            Return InetAddress.getAllByName(host, reqAddr)(0)
        End Function

        ''' <summary>
        ''' Given the name of a host, returns an array of its IP addresses,
        ''' based on the configured name service on the system.
        ''' 
        ''' <p> The host name can either be a machine name, such as
        ''' "{@code java.sun.com}", or a textual representation of its IP
        ''' address. If a literal IP address is supplied, only the
        ''' validity of the address format is checked.
        ''' 
        ''' <p> For {@code host} specified in <i>literal IPv6 address</i>,
        ''' either the form defined in RFC 2732 or the literal IPv6 address
        ''' format defined in RFC 2373 is accepted. A literal IPv6 address may
        ''' also be qualified by appending a scoped zone identifier or scope_id.
        ''' The syntax and usage of scope_ids is described
        ''' <a href="Inet6Address.html#scoped">here</a>.
        ''' <p> If the host is {@code null} then an {@code InetAddress}
        ''' representing an address of the loopback interface is returned.
        ''' See <a href="http://www.ietf.org/rfc/rfc3330.txt">RFC&nbsp;3330</a>
        ''' section&nbsp;2 and <a href="http://www.ietf.org/rfc/rfc2373.txt">RFC&nbsp;2373</a>
        ''' section&nbsp;2.5.3. </p>
        ''' 
        ''' <p> If there is a security manager and {@code host} is not
        ''' null and {@code host.length() } is not equal to zero, the
        ''' security manager's
        ''' {@code checkConnect} method is called
        ''' with the hostname and {@code -1}
        ''' as its arguments to see if the operation is allowed.
        ''' </summary>
        ''' <param name="host">   the name of the host, or {@code null}. </param>
        ''' <returns>     an array of all the IP addresses for a given host name.
        ''' </returns>
        ''' <exception cref="UnknownHostException">  if no IP address for the
        '''               {@code host} could be found, or if a scope_id was specified
        '''               for a global IPv6 address. </exception>
        ''' <exception cref="SecurityException">  if a security manager exists and its
        '''               {@code checkConnect} method doesn't allow the operation.
        ''' </exception>
        ''' <seealso cref= SecurityManager#checkConnect </seealso>
        Public Shared Function getAllByName(ByVal host As String) As InetAddress()
            Return getAllByName(host, Nothing)
        End Function

        Private Shared Function getAllByName(ByVal host As String, ByVal reqAddr As InetAddress) As InetAddress()

            If host Is Nothing OrElse host.Length() = 0 Then
                Dim ret As InetAddress() = New InetAddress(0) {}
                ret(0) = impl.loopbackAddress()
                Return ret
            End If

            Dim ipv6Expected As Boolean = False
            If host.Chars(0) = "["c Then
                ' This is supposed to be an IPv6 literal
                If host.Length() > 2 AndAlso host.Chars(host.Length() - 1) = "]"c Then
                    host = host.Substring(1, host.Length() - 1 - 1)
                    ipv6Expected = True
                Else
                    ' This was supposed to be a IPv6 address, but it's not!
                    Throw New UnknownHostException(host & ": invalid IPv6 address")
                End If
            End If

            ' if host is an IP address, we won't do further lookup
            If Character.digit(host.Chars(0), 16) <> -1 OrElse (host.Chars(0) = ":"c) Then
                Dim addr As SByte() = Nothing
                Dim numericZone As Integer = -1
                Dim ifname As String = Nothing
                ' see if it is IPv4 address
                addr = sun.net.util.IPAddressUtil.textToNumericFormatV4(host)
                If addr Is Nothing Then
                    ' This is supposed to be an IPv6 literal
                    ' Check if a numeric or string zone id is present
                    Dim pos As Integer
                    pos = host.IndexOf("%")
                    If pos <> -1 Then
                        numericZone = checkNumericZone(host)
                        If numericZone = -1 Then ' remainder of string must be an ifname ifname = host.Substring(pos+1)
                        End If
                        addr = sun.net.util.IPAddressUtil.textToNumericFormatV6(host)
                        If addr Is Nothing AndAlso host.Contains(":") Then Throw New UnknownHostException(host & ": invalid IPv6 address")
                    ElseIf ipv6Expected Then
                        ' Means an IPv4 litteral between brackets!
                        Throw New UnknownHostException("[" & host & "]")
                    End If
                    Dim ret As InetAddress() = New InetAddress(0) {}
                    If addr IsNot Nothing Then
                        If addr.Length = Inet4Address.INADDRSZ Then
                            ret(0) = New Inet4Address(Nothing, addr)
                        Else
                            If ifname IsNot Nothing Then
                                ret(0) = New Inet6Address(Nothing, addr, ifname)
                            Else
                                ret(0) = New Inet6Address(Nothing, addr, numericZone)
                            End If
                        End If
                        Return ret
                    End If
                ElseIf ipv6Expected Then
                    ' We were expecting an IPv6 Litteral, but got something else
                    Throw New UnknownHostException("[" & host & "]")
                End If
                Return getAllByName0(host, reqAddr, True)
        End Function

        ''' <summary>
        ''' Returns the loopback address.
        ''' <p>
        ''' The InetAddress returned will represent the IPv4
        ''' loopback address, 127.0.0.1, or the IPv6 loopback
        ''' address, ::1. The IPv4 loopback address returned
        ''' is only one of many in the form 127.*.*.*
        ''' </summary>
        ''' <returns>  the InetAddress loopback instance.
        ''' @since 1.7 </returns>
        Public Property Shared loopbackAddress As InetAddress
            Get
                Return impl.loopbackAddress()
            End Get
        End Property


        ''' <summary>
        ''' check if the literal address string has %nn appended
        ''' returns -1 if not, or the numeric value otherwise.
        ''' 
        ''' %nn may also be a string that represents the displayName of
        ''' a currently available NetworkInterface.
        ''' </summary>
        Private Shared Function checkNumericZone(ByVal s As String) As Integer
            Dim percent As Integer = s.IndexOf("%"c)
            Dim slen As Integer = s.Length()
            Dim digit As Integer, zone As Integer = 0
            If percent = -1 Then Return -1
            For i As Integer = percent + 1 To slen - 1
                Dim c As Char = s.Chars(i)
                If c = "]"c Then
                    If i = percent + 1 Then Return -1
                    Exit For
                End If
                digit = Character.digit(c, 10)
                If digit < 0 Then Return -1
                zone = (zone * 10) + digit
            Next i
            Return zone
        End Function

        Private Shared Function getAllByName0(ByVal host As String) As InetAddress()
            Return getAllByName0(host, True)
        End Function

        ''' <summary>
        ''' package private so SocketPermission can call it
        ''' </summary>
        Shared Function getAllByName0(ByVal host As String, ByVal check As Boolean) As InetAddress()
            Return getAllByName0(host, Nothing, check)
        End Function

        Private Shared Function getAllByName0(ByVal host As String, ByVal reqAddr As InetAddress, ByVal check As Boolean) As InetAddress()

            ' If it gets here it is presumed to be a hostname 
            ' Cache.get can return: null, unknownAddress, or InetAddress[] 

            '         make sure the connection to the host is allowed, before we
            '         * give out a hostname
            '         
            If check Then
                Dim security As SecurityManager = System.securityManager
                If security IsNot Nothing Then security.checkConnect(host, -1)
            End If

            Dim addresses As InetAddress() = getCachedAddresses(host)

            ' If no entry in cache, then do the host lookup 
            If addresses Is Nothing Then addresses = getAddressesFromNameService(host, reqAddr)

            If addresses = unknown_array Then Throw New UnknownHostException(host)

            Return addresses.Clone()
        End Function

        Private Shared Function getAddressesFromNameService(ByVal host As String, ByVal reqAddr As InetAddress) As InetAddress()
            Dim addresses As InetAddress() = Nothing
            Dim success As Boolean = False
            Dim ex As UnknownHostException = Nothing

            ' Check whether the host is in the lookupTable.
            ' 1) If the host isn't in the lookupTable when
            '    checkLookupTable() is called, checkLookupTable()
            '    would add the host in the lookupTable and
            '    return null. So we will do the lookup.
            ' 2) If the host is in the lookupTable when
            '    checkLookupTable() is called, the current thread
            '    would be blocked until the host is removed
            '    from the lookupTable. Then this thread
            '    should try to look up the addressCache.
            '     i) if it found the addresses in the
            '        addressCache, checkLookupTable()  would
            '        return the addresses.
            '     ii) if it didn't find the addresses in the
            '         addressCache for any reason,
            '         it should add the host in the
            '         lookupTable and return null so the
            '         following code would do  a lookup itself.
            addresses = checkLookupTable(host)
            If addresses Is Nothing Then
                Try
                    ' This is the first thread which looks up the addresses
                    ' this host or the cache entry for this host has been
                    ' expired so this thread should do the lookup.
                    For Each nameService As NameService In nameServices
                        Try
                            '                        
                            '                         * Do not put the call to lookup() inside the
                            '                         * constructor.  if you do you will still be
                            '                         * allocating space when the lookup fails.
                            '                         

                            addresses = nameService.lookupAllHostAddr(host)
                            success = True
                            Exit For
                        Catch uhe As UnknownHostException
                            If host.equalsIgnoreCase("localhost") Then
                                Dim local As InetAddress() = {impl.loopbackAddress()}
                                addresses = local
                                success = True
                                Exit For
                            Else
                                addresses = unknown_array
                                success = False
                                ex = uhe
                            End If
                        End Try
                    Next nameService

                    ' More to do?
                    If reqAddr IsNot Nothing AndAlso addresses.Length > 1 AndAlso (Not addresses(0).Equals(reqAddr)) Then
                        ' Find it?
                        Dim i As Integer = 1
                        Do While i < addresses.Length
                            If addresses(i).Equals(reqAddr) Then Exit Do
                            i += 1
                        Loop
                        ' Rotate
                        If i < addresses.Length Then
                            Dim tmp As InetAddress, tmp2 As InetAddress = reqAddr
                            For j As Integer = 0 To i - 1
                                tmp = addresses(j)
                                addresses(j) = tmp2
                                tmp2 = tmp
                            Next j
                            addresses(i) = tmp2
                        End If
                    End If
                    ' Cache the address.
                    cacheAddresses(host, addresses, success)

                    If (Not success) AndAlso ex IsNot Nothing Then Throw ex

                Finally
                    ' Delete host from the lookupTable and notify
                    ' all threads waiting on the lookupTable monitor.
                    updateLookupTable(host)
                End Try
            End If

            Return addresses
        End Function


        Private Shared Function checkLookupTable(ByVal host As String) As InetAddress()
            SyncLock lookupTable
                ' If the host isn't in the lookupTable, add it in the
                ' lookuptable and return null. The caller should do
                ' the lookup.
                If lookupTable.ContainsKey(host) = False Then
                    lookupTable(host) = Nothing
                    Return Nothing
                End If

                ' If the host is in the lookupTable, it means that another
                ' thread is trying to look up the addresses of this host.
                ' This thread should wait.
                Do While lookupTable.ContainsKey(host)
                    Try
                        lookupTable.wait()
                    Catch e As InterruptedException
                    End Try
                Loop
            End SyncLock

            ' The other thread has finished looking up the addresses of
            ' the host. This thread should retry to get the addresses
            ' from the addressCache. If it doesn't get the addresses from
            ' the cache, it will try to look up the addresses itself.
            Dim addresses As InetAddress() = getCachedAddresses(host)
            If addresses Is Nothing Then
                SyncLock lookupTable
                    lookupTable(host) = Nothing
                    Return Nothing
                End SyncLock
            End If

            Return addresses
        End Function

        Private Shared Sub updateLookupTable(ByVal host As String)
            SyncLock lookupTable
                lookupTable.Remove(host)
                lookupTable.notifyAll()
            End SyncLock
        End Sub

        ''' <summary>
        ''' Returns an {@code InetAddress} object given the raw IP address .
        ''' The argument is in network byte order: the highest order
        ''' byte of the address is in {@code getAddress()[0]}.
        ''' 
        ''' <p> This method doesn't block, i.e. no reverse name service lookup
        ''' is performed.
        ''' 
        ''' <p> IPv4 address byte array must be 4 bytes long and IPv6 byte array
        ''' must be 16 bytes long
        ''' </summary>
        ''' <param name="addr"> the raw IP address in network byte order </param>
        ''' <returns>  an InetAddress object created from the raw IP address. </returns>
        ''' <exception cref="UnknownHostException">  if IP address is of illegal length
        ''' @since 1.4 </exception>
        Public Shared Function getByAddress(ByVal addr As SByte()) As InetAddress
            Return getByAddress(Nothing, addr)
        End Function

        Private Shared cachedLocalHost As InetAddress = Nothing
        Private Shared cacheTime As Long = 0
        Private Const maxCacheTime As Long = 5000L
        Private Shared ReadOnly cacheLock As New Object

        ''' <summary>
        ''' Returns the address of the local host. This is achieved by retrieving
        ''' the name of the host from the system, then resolving that name into
        ''' an {@code InetAddress}.
        ''' 
        ''' <P>Note: The resolved address may be cached for a short period of time.
        ''' </P>
        ''' 
        ''' <p>If there is a security manager, its
        ''' {@code checkConnect} method is called
        ''' with the local host name and {@code -1}
        ''' as its arguments to see if the operation is allowed.
        ''' If the operation is not allowed, an InetAddress representing
        ''' the loopback address is returned.
        ''' </summary>
        ''' <returns>     the address of the local host.
        ''' </returns>
        ''' <exception cref="UnknownHostException">  if the local host name could not
        '''             be resolved into an address.
        ''' </exception>
        ''' <seealso cref= SecurityManager#checkConnect </seealso>
        ''' <seealso cref= java.net.InetAddress#getByName(java.lang.String) </seealso>
        Public Property Shared localHost As InetAddress
            Get

                Dim security As SecurityManager = System.securityManager
                Try
                    Dim local As String = impl.localHostName

                    If security IsNot Nothing Then security.checkConnect(local, -1)

                    If local.Equals("localhost") Then Return impl.loopbackAddress()

                    Dim ret As InetAddress = Nothing
                    SyncLock cacheLock
                        Dim now As Long = System.currentTimeMillis()
                        If cachedLocalHost IsNot Nothing Then
                            If (now - cacheTime) < maxCacheTime Then ' Less than 5s old?
                                ret = cachedLocalHost
                            Else
                                cachedLocalHost = Nothing
                            End If
                        End If

                        ' we are calling getAddressesFromNameService directly
                        ' to avoid getting localHost from cache
                        If ret Is Nothing Then
                            Dim localAddrs As InetAddress()
                            Try
                                localAddrs = InetAddress.getAddressesFromNameService(local, Nothing)
                            Catch uhe As UnknownHostException
                                ' Rethrow with a more informative error message.
                                Dim uhe2 As New UnknownHostException(local & ": " & uhe.Message)
                                uhe2.initCause(uhe)
                                Throw uhe2
                            End Try
                            cachedLocalHost = localAddrs(0)
                            cacheTime = now
                            ret = localAddrs(0)
                        End If
                    End SyncLock
                    Return ret
                Catch e As java.lang.SecurityException
                    Return impl.loopbackAddress()
                End Try
            End Get
        End Property

        ''' <summary>
        ''' Perform class load-time initializations.
        ''' </summary>
        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Private Shared Sub init()
        End Sub


        '    
        '     * Returns the InetAddress representing anyLocalAddress
        '     * (typically 0.0.0.0 or ::0)
        '     
        Shared Function anyLocalAddress() As InetAddress
            Return impl.anyLocalAddress()
        End Function

        '    
        '     * Load and instantiate an underlying impl class
        '     
        Friend Shared Function loadImpl(ByVal implName As String) As InetAddressImpl
            Dim impl As Object = Nothing

            '        
            '         * Property "impl.prefix" will be prepended to the classname
            '         * of the implementation object we instantiate, to which we
            '         * delegate the real work (like native methods).  This
            '         * property can vary across implementations of the java.
            '         * classes.  The default is an empty String "".
            '         
            Dim prefix As String = java.security.AccessController.doPrivileged(New GetPropertyAction("impl.prefix", ""))
            Try
                impl = Type.GetType("java.net." & prefix + implName).newInstance()
            Catch e As [Class]NotFoundException
				Console.Error.WriteLine("Class not found: java.net." & prefix + implName & ":" & vbLf & "check impl.prefix property " & "in your properties file.")
            Catch e As InstantiationException
                Console.Error.WriteLine("Could not instantiate: java.net." & prefix + implName & ":" & vbLf & "check impl.prefix property " & "in your properties file.")
            Catch e As IllegalAccessException
                Console.Error.WriteLine("Cannot access class: java.net." & prefix + implName & ":" & vbLf & "check impl.prefix property " & "in your properties file.")
            End Try

            If impl Is Nothing Then
                Try
                    impl = Type.GetType(implName).newInstance()
                Catch e As Exception
                    Throw New [Error]("System property impl.prefix incorrect")
                End Try
            End If

            Return CType(impl, InetAddressImpl)
        End Function

        Private Sub readObjectNoData(ByVal s As java.io.ObjectInputStream)
            If Me.GetType().classLoader IsNot Nothing Then Throw New SecurityException("invalid address type")
        End Sub

        Private Shared ReadOnly FIELDS_OFFSET As Long
        Private Shared ReadOnly UNSAFE As sun.misc.Unsafe


        Private Sub readObject(ByVal s As java.io.ObjectInputStream)
            If Me.GetType().classLoader IsNot Nothing Then Throw New SecurityException("invalid address type")
            Dim gf As java.io.ObjectInputStream.GetField = s.readFields()
            Dim host As String = CStr(gf.get("hostName", Nothing))
            Dim address_Renamed As Integer = gf.get("address", 0)
            Dim family As Integer = gf.get("family", 0)
            Dim h As New InetAddressHolder(host, address_Renamed, family)
            UNSAFE.putObject(Me, FIELDS_OFFSET, h)
        End Sub

        ' needed because the serializable fields no longer exist 

        ''' <summary>
        ''' @serialField hostName String
        ''' @serialField address int
        ''' @serialField family int
        ''' </summary>
        Private Shared ReadOnly serialPersistentFields As java.io.ObjectStreamField() = {New java.io.ObjectStreamField("hostName", GetType(String)), New java.io.ObjectStreamField("address", GetType(Integer)), New java.io.ObjectStreamField("family", GetType(Integer))}

        Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
            If Me.GetType().classLoader IsNot Nothing Then Throw New SecurityException("invalid address type")
            Dim pf As java.io.ObjectOutputStream.PutField = s.putFields()
            pf.put("hostName", holder().hostName)
            pf.put("address", holder().address)
            pf.put("family", holder().family)
            s.writeFields()
        End Sub
    End Class

    '
    ' * Simple factory to create the impl
    ' 
    Friend Class InetAddressImplFactory

        Friend Shared Function create() As InetAddressImpl
            Return InetAddress.loadImpl(If(iPv6Supported, "Inet6AddressImpl", "Inet4AddressImpl"))
        End Function

        'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
        <DllImport("unknown")>
        Friend Shared Function isIPv6Supported() As Boolean
        End Function
    End Class

End Namespace