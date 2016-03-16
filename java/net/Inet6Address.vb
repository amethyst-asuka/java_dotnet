Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' This class represents an Internet Protocol version 6 (IPv6) address.
	''' Defined by <a href="http://www.ietf.org/rfc/rfc2373.txt">
	''' <i>RFC&nbsp;2373: IP Version 6 Addressing Architecture</i></a>.
	''' 
	''' <h3> <A NAME="format">Textual representation of IP addresses</a> </h3>
	''' 
	''' Textual representation of IPv6 address used as input to methods
	''' takes one of the following forms:
	''' 
	''' <ol>
	'''   <li><p> <A NAME="lform">The preferred form</a> is x:x:x:x:x:x:x:x,
	'''   where the 'x's are
	'''   the hexadecimal values of the eight 16-bit pieces of the
	'''   address. This is the full form.  For example,
	''' 
	'''   <blockquote><table cellpadding=0 cellspacing=0 summary="layout">
	'''   <tr><td>{@code 1080:0:0:0:8:800:200C:417A}<td></tr>
	'''   </table></blockquote>
	''' 
	'''   <p> Note that it is not necessary to write the leading zeros in
	'''   an individual field. However, there must be at least one numeral
	'''   in every field, except as described below.</li>
	''' 
	'''   <li><p> Due to some methods of allocating certain styles of IPv6
	'''   addresses, it will be common for addresses to contain long
	'''   strings of zero bits. In order to make writing addresses
	'''   containing zero bits easier, a special syntax is available to
	'''   compress the zeros. The use of "::" indicates multiple groups
	'''   of 16-bits of zeros. The "::" can only appear once in an address.
	'''   The "::" can also be used to compress the leading and/or trailing
	'''   zeros in an address. For example,
	''' 
	'''   <blockquote><table cellpadding=0 cellspacing=0 summary="layout">
	'''   <tr><td>{@code 1080::8:800:200C:417A}<td></tr>
	'''   </table></blockquote>
	''' 
	'''   <li><p> An alternative form that is sometimes more convenient
	'''   when dealing with a mixed environment of IPv4 and IPv6 nodes is
	'''   x:x:x:x:x:x:d.d.d.d, where the 'x's are the hexadecimal values
	'''   of the six high-order 16-bit pieces of the address, and the 'd's
	'''   are the decimal values of the four low-order 8-bit pieces of the
	'''   standard IPv4 representation address, for example,
	''' 
	'''   <blockquote><table cellpadding=0 cellspacing=0 summary="layout">
	'''   <tr><td>{@code ::FFFF:129.144.52.38}<td></tr>
	'''   <tr><td>{@code ::129.144.52.38}<td></tr>
	'''   </table></blockquote>
	''' 
	'''   <p> where "::FFFF:d.d.d.d" and "::d.d.d.d" are, respectively, the
	'''   general forms of an IPv4-mapped IPv6 address and an
	'''   IPv4-compatible IPv6 address. Note that the IPv4 portion must be
	'''   in the "d.d.d.d" form. The following forms are invalid:
	''' 
	'''   <blockquote><table cellpadding=0 cellspacing=0 summary="layout">
	'''   <tr><td>{@code ::FFFF:d.d.d}<td></tr>
	'''   <tr><td>{@code ::FFFF:d.d}<td></tr>
	'''   <tr><td>{@code ::d.d.d}<td></tr>
	'''   <tr><td>{@code ::d.d}<td></tr>
	'''   </table></blockquote>
	''' 
	'''   <p> The following form:
	''' 
	'''   <blockquote><table cellpadding=0 cellspacing=0 summary="layout">
	'''   <tr><td>{@code ::FFFF:d}<td></tr>
	'''   </table></blockquote>
	''' 
	'''   <p> is valid, however it is an unconventional representation of
	'''   the IPv4-compatible IPv6 address,
	''' 
	'''   <blockquote><table cellpadding=0 cellspacing=0 summary="layout">
	'''   <tr><td>{@code ::255.255.0.d}<td></tr>
	'''   </table></blockquote>
	''' 
	'''   <p> while "::d" corresponds to the general IPv6 address
	'''   "0:0:0:0:0:0:0:d".</li>
	''' </ol>
	''' 
	''' <p> For methods that return a textual representation as output
	''' value, the full form is used. Inet6Address will return the full
	''' form because it is unambiguous when used in combination with other
	''' textual data.
	''' 
	''' <h4> Special IPv6 address </h4>
	''' 
	''' <blockquote>
	''' <table cellspacing=2 summary="Description of IPv4-mapped address">
	''' <tr><th valign=top><i>IPv4-mapped address</i></th>
	'''         <td>Of the form::ffff:w.x.y.z, this IPv6 address is used to
	'''         represent an IPv4 address. It allows the native program to
	'''         use the same address data structure and also the same
	'''         socket when communicating with both IPv4 and IPv6 nodes.
	''' 
	'''         <p>In InetAddress and Inet6Address, it is used for internal
	'''         representation; it has no functional role. Java will never
	'''         return an IPv4-mapped address.  These classes can take an
	'''         IPv4-mapped address as input, both in byte array and text
	'''         representation. However, it will be converted into an IPv4
	'''         address.</td></tr>
	''' </table></blockquote>
	''' 
	''' <h4><A NAME="scoped">Textual representation of IPv6 scoped addresses</a></h4>
	''' 
	''' <p> The textual representation of IPv6 addresses as described above can be
	''' extended to specify IPv6 scoped addresses. This extension to the basic
	''' addressing architecture is described in [draft-ietf-ipngwg-scoping-arch-04.txt].
	''' 
	''' <p> Because link-local and site-local addresses are non-global, it is possible
	''' that different hosts may have the same destination address and may be
	''' reachable through different interfaces on the same originating system. In
	''' this case, the originating system is said to be connected to multiple zones
	''' of the same scope. In order to disambiguate which is the intended destination
	''' zone, it is possible to append a zone identifier (or <i>scope_id</i>) to an
	''' IPv6 address.
	''' 
	''' <p> The general format for specifying the <i>scope_id</i> is the following:
	''' 
	''' <blockquote><i>IPv6-address</i>%<i>scope_id</i></blockquote>
	''' <p> The IPv6-address is a literal IPv6 address as described above.
	''' The <i>scope_id</i> refers to an interface on the local system, and it can be
	''' specified in two ways.
	''' <ol><li><i>As a numeric identifier.</i> This must be a positive integer
	''' that identifies the particular interface and scope as understood by the
	''' system. Usually, the numeric values can be determined through administration
	''' tools on the system. Each interface may have multiple values, one for each
	''' scope. If the scope is unspecified, then the default value used is zero.</li>
	''' <li><i>As a string.</i> This must be the exact string that is returned by
	''' <seealso cref="java.net.NetworkInterface#getName()"/> for the particular interface in
	''' question. When an Inet6Address is created in this way, the numeric scope-id
	''' is determined at the time the object is created by querying the relevant
	''' NetworkInterface.</li></ol>
	''' 
	''' <p> Note also, that the numeric <i>scope_id</i> can be retrieved from
	''' Inet6Address instances returned from the NetworkInterface class. This can be
	''' used to find out the current scope ids configured on the system.
	''' @since 1.4
	''' </summary>

	Public NotInheritable Class Inet6Address
		Inherits InetAddress

		Friend Const INADDRSZ As Integer = 16

	'    
	'     * cached scope_id - for link-local address use only.
	'     
		<NonSerialized> _
		Private cached_scope_id As Integer ' 0

		Private Class Inet6AddressHolder
			Private ReadOnly outerInstance As Inet6Address


			Private Sub New(ByVal outerInstance As Inet6Address)
					Me.outerInstance = outerInstance
				ipaddress = New SByte(INADDRSZ - 1){}
			End Sub

			Private Sub New(ByVal outerInstance As Inet6Address, ByVal ipaddress As SByte(), ByVal scope_id As Integer, ByVal scope_id_set As Boolean, ByVal ifname As NetworkInterface, ByVal scope_ifname_set As Boolean)
					Me.outerInstance = outerInstance
				Me.ipaddress = ipaddress
				Me.scope_id = scope_id
				Me.scope_id_set = scope_id_set
				Me.scope_ifname_set = scope_ifname_set
				Me.scope_ifname = ifname
			End Sub

			''' <summary>
			''' Holds a 128-bit (16 bytes) IPv6 address.
			''' </summary>
			Friend ipaddress As SByte()

			''' <summary>
			''' scope_id. The scope specified when the object is created. If the object
			''' is created with an interface name, then the scope_id is not determined
			''' until the time it is needed.
			''' </summary>
			Friend scope_id As Integer ' 0

			''' <summary>
			''' This will be set to true when the scope_id field contains a valid
			''' integer scope_id.
			''' </summary>
			Friend scope_id_set As Boolean ' false

			''' <summary>
			''' scoped interface. scope_id is derived from this as the scope_id of the first
			''' address whose scope is the same as this address for the named interface.
			''' </summary>
			Friend scope_ifname As NetworkInterface ' null

			''' <summary>
			''' set if the object is constructed with a scoped
			''' interface instead of a numeric scope id.
			''' </summary>
			Friend scope_ifname_set As Boolean ' false;

			Friend Overridable Property addr As SByte()
				Set(ByVal addr As SByte())
					If addr.Length = INADDRSZ Then ' normal IPv6 address Array.Copy(addr, 0, ipaddress, 0, INADDRSZ)
				End Set
			End Property

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			void init(byte addr() , int scope_id)
				addr = addr

				If scope_id >= 0 Then
					Me.scope_id = scope_id
					Me.scope_id_set = True
				End If

			void init(SByte addr() , NetworkInterface nif) throws UnknownHostException
				addr = addr

				If nif IsNot Nothing Then
					Me.scope_id = deriveNumericScope(ipaddress, nif)
					Me.scope_id_set = True
					Me.scope_ifname = nif
					Me.scope_ifname_set = True
				End If

			String hostAddress
				Dim s As String = numericToTextFormat(ipaddress)
				If scope_ifname IsNot Nothing Then ' must check this first
					s = s & "%" & scope_ifname.name
				ElseIf scope_id_set Then
					s = s & "%" & scope_id
				End If
				Return s

			public Boolean Equals(Object o)
				If Not(TypeOf o Is Inet6AddressHolder) Then Return False
				Dim that As Inet6AddressHolder = CType(o, Inet6AddressHolder)

				Return java.util.Arrays.Equals(Me.ipaddress, that.ipaddress)

			public Integer GetHashCode()
				If ipaddress IsNot Nothing Then

					Dim hash As Integer = 0
					Dim i As Integer=0
					Do While i<INADDRSZ
						Dim j As Integer=0
						Dim component As Integer=0
						Do While j<4 AndAlso i<INADDRSZ
							component = (component << 8) + ipaddress(i)
							j += 1
							i += 1
						Loop
						hash += component
					Loop
					Return hash

				Else
					Return 0
				End If

			Boolean iPv4CompatibleAddress
				If (ipaddress(0) = &H0) AndAlso (ipaddress(1) = &H0) AndAlso (ipaddress(2) = &H0) AndAlso (ipaddress(3) = &H0) AndAlso (ipaddress(4) = &H0) AndAlso (ipaddress(5) = &H0) AndAlso (ipaddress(6) = &H0) AndAlso (ipaddress(7) = &H0) AndAlso (ipaddress(8) = &H0) AndAlso (ipaddress(9) = &H0) AndAlso (ipaddress(10) = &H0) AndAlso (ipaddress(11) = &H0) Then Return True
				Return False

			Boolean multicastAddress
				Return ((ipaddress(0) And &Hff) = &Hff)

			Boolean anyLocalAddress
				Dim test As SByte = &H0
				For i As Integer = 0 To INADDRSZ - 1
					test = test Or ipaddress(i)
				Next i
				Return (test = &H0)

			Boolean loopbackAddress
				Dim test As SByte = &H0
				For i As Integer = 0 To 14
					test = test Or ipaddress(i)
				Next i
				Return (test = &H0) AndAlso (ipaddress(15) = &H1)

			Boolean linkLocalAddress
				Return ((ipaddress(0) And &Hff) = &Hfe AndAlso (ipaddress(1) And &Hc0) = &H80)


			Boolean siteLocalAddress
				Return ((ipaddress(0) And &Hff) = &Hfe AndAlso (ipaddress(1) And &Hc0) = &Hc0)

			Boolean mCGlobal
				Return ((ipaddress(0) And &Hff) = &Hff AndAlso (ipaddress(1) And &Hf) = &He)

			Boolean mCNodeLocal
				Return ((ipaddress(0) And &Hff) = &Hff AndAlso (ipaddress(1) And &Hf) = &H1)

			Boolean mCLinkLocal
				Return ((ipaddress(0) And &Hff) = &Hff AndAlso (ipaddress(1) And &Hf) = &H2)

			Boolean mCSiteLocal
				Return ((ipaddress(0) And &Hff) = &Hff AndAlso (ipaddress(1) And &Hf) = &H5)

			Boolean mCOrgLocal
				Return ((ipaddress(0) And &Hff) = &Hff AndAlso (ipaddress(1) And &Hf) = &H8)
		End Class

		private final transient Inet6AddressHolder holder6

		private static final Long serialVersionUID = 6880410070516793377L

		' Perform native initialization
		static Inet6Address()
			init()
			Try
				Dim unsafe_Renamed As sun.misc.Unsafe = sun.misc.Unsafe.unsafe
				FIELDS_OFFSET = unsafe_Renamed.objectFieldOffset(GetType(Inet6Address).getDeclaredField("holder6"))
				UNSAFE = unsafe_Renamed
			Catch e As ReflectiveOperationException
				Throw New [Error](e)
			End Try

		Inet6Address()
			MyBase()
			holder_Renamed.init(Nothing, IPv6)
			holder6 = New Inet6AddressHolder(Me)

	'     checking of value for scope_id should be done by caller
	'     * scope_id must be >= 0, or -1 to indicate not being set
	'     
		Inet6Address(String hostName, SByte addr() , Integer scope_id)
			holder_Renamed.init(hostName, IPv6)
			holder6 = New Inet6AddressHolder(Me)
			holder6.init(addr, scope_id)

		Inet6Address(String hostName, SByte addr())
			holder6 = New Inet6AddressHolder(Me)
			Try
				initif(hostName, addr, Nothing) ' cant happen if ifname is null
			Catch e As UnknownHostException
			End Try

		Inet6Address(String hostName, SByte addr() , NetworkInterface nif) throws UnknownHostException
			holder6 = New Inet6AddressHolder(Me)
			initif(hostName, addr, nif)

		Inet6Address(String hostName, SByte addr() , String ifname) throws UnknownHostException
			holder6 = New Inet6AddressHolder(Me)
			initstr(hostName, addr, ifname)

		''' <summary>
		''' Create an Inet6Address in the exact manner of {@link
		''' InetAddress#getByAddress(String,byte[])} except that the IPv6 scope_id is
		''' set to the value corresponding to the given interface for the address
		''' type specified in {@code addr}. The call will fail with an
		''' UnknownHostException if the given interface does not have a numeric
		''' scope_id assigned for the given address type (eg. link-local or site-local).
		''' See <a href="Inet6Address.html#scoped">here</a> for a description of IPv6
		''' scoped addresses.
		''' </summary>
		''' <param name="host"> the specified host </param>
		''' <param name="addr"> the raw IP address in network byte order </param>
		''' <param name="nif"> an interface this address must be associated with. </param>
		''' <returns>  an Inet6Address object created from the raw IP address. </returns>
		''' <exception cref="UnknownHostException">
		'''          if IP address is of illegal length, or if the interface does not
		'''          have a numeric scope_id assigned for the given address type.
		''' 
		''' @since 1.5 </exception>
		public static Inet6Address getByAddress(String host, SByte() addr, NetworkInterface nif) throws UnknownHostException
			If host IsNot Nothing AndAlso host.length() > 0 AndAlso host.Chars(0) = "["c Then
				If host.Chars(host.length()-1) = "]"c Then host = host.Substring(1, host.length() -1 - 1)
			End If
			If addr IsNot Nothing Then
				If addr.length = Inet6Address.INADDRSZ Then Return New Inet6Address(host, addr, nif)
			End If
			Throw New UnknownHostException("addr is of illegal length")

		''' <summary>
		''' Create an Inet6Address in the exact manner of {@link
		''' InetAddress#getByAddress(String,byte[])} except that the IPv6 scope_id is
		''' set to the given numeric value. The scope_id is not checked to determine
		''' if it corresponds to any interface on the system.
		''' See <a href="Inet6Address.html#scoped">here</a> for a description of IPv6
		''' scoped addresses.
		''' </summary>
		''' <param name="host"> the specified host </param>
		''' <param name="addr"> the raw IP address in network byte order </param>
		''' <param name="scope_id"> the numeric scope_id for the address. </param>
		''' <returns>  an Inet6Address object created from the raw IP address. </returns>
		''' <exception cref="UnknownHostException">  if IP address is of illegal length.
		''' 
		''' @since 1.5 </exception>
		public static Inet6Address getByAddress(String host, SByte() addr, Integer scope_id) throws UnknownHostException
			If host IsNot Nothing AndAlso host.length() > 0 AndAlso host.Chars(0) = "["c Then
				If host.Chars(host.length()-1) = "]"c Then host = host.Substring(1, host.length() -1 - 1)
			End If
			If addr IsNot Nothing Then
				If addr.length = Inet6Address.INADDRSZ Then Return New Inet6Address(host, addr, scope_id)
			End If
			Throw New UnknownHostException("addr is of illegal length")

		private  Sub  initstr(String hostName, SByte addr() , String ifname) throws UnknownHostException
			Try
				Dim nif As NetworkInterface = NetworkInterface.getByName(ifname)
				If nif Is Nothing Then Throw New UnknownHostException("no such interface " & ifname)
				initif(hostName, addr, nif)
			Catch e As SocketException
				Throw New UnknownHostException("SocketException thrown" & ifname)
			End Try

		private  Sub  initif(String hostName, SByte addr() , NetworkInterface nif) throws UnknownHostException
			Dim family As Integer = -1
			holder6.init(addr, nif)

			If addr.length = INADDRSZ Then ' normal IPv6 address family = IPv6
			holder_Renamed.init(hostName, family)

	'     check the two Ipv6 addresses and return false if they are both
	'     * non global address types, but not the same.
	'     * (ie. one is sitelocal and the other linklocal)
	'     * return true otherwise.
	'     

		private static Boolean isDifferentLocalAddressType(SByte() thisAddr, SByte() otherAddr)

			If Inet6Address.isLinkLocalAddress(thisAddr) AndAlso (Not Inet6Address.isLinkLocalAddress(otherAddr)) Then Return False
			If Inet6Address.isSiteLocalAddress(thisAddr) AndAlso (Not Inet6Address.isSiteLocalAddress(otherAddr)) Then Return False
			Return True

		private static Integer deriveNumericScope(SByte() thisAddr, NetworkInterface ifc) throws UnknownHostException
			Dim addresses As System.Collections.IEnumerator(Of InetAddress) = ifc.inetAddresses
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Do While addresses.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim addr As InetAddress = addresses.nextElement()
				If Not(TypeOf addr Is Inet6Address) Then Continue Do
				Dim ia6_addr As Inet6Address = CType(addr, Inet6Address)
				' check if site or link local prefixes match 
				If Not isDifferentLocalAddressType(thisAddr, ia6_addr.address) Then Continue Do
				' found a matching address - return its scope_id 
				Return ia6_addr.scopeId
			Loop
			Throw New UnknownHostException("no scope_id found")

		private Integer deriveNumericScope(String ifname) throws UnknownHostException
			Dim en As System.Collections.IEnumerator(Of NetworkInterface)
			Try
				en = NetworkInterface.networkInterfaces
			Catch e As SocketException
				Throw New UnknownHostException("could not enumerate local network interfaces")
			End Try
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Do While en.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim ifc As NetworkInterface = en.nextElement()
				If ifc.name.Equals(ifname) Then Return deriveNumericScope(holder6.ipaddress, ifc)
			Loop
			Throw New UnknownHostException("No matching address found for interface : " & ifname)

		''' <summary>
		''' @serialField ipaddress byte[]
		''' @serialField scope_id int
		''' @serialField scope_id_set boolean
		''' @serialField scope_ifname_set boolean
		''' @serialField ifname String
		''' </summary>

		private static final java.io.ObjectStreamField() serialPersistentFields = { New java.io.ObjectStreamField("ipaddress", GetType(SByte())), New java.io.ObjectStreamField("scope_id", GetType(Integer)), New java.io.ObjectStreamField("scope_id_set", GetType(Boolean)), New java.io.ObjectStreamField("scope_ifname_set", GetType(Boolean)), New java.io.ObjectStreamField("ifname", GetType(String)) }

		private static final Long FIELDS_OFFSET
		private static final sun.misc.Unsafe UNSAFE


		''' <summary>
		''' restore the state of this object from stream
		''' including the scope information, only if the
		''' scoped interface name is valid on this system
		''' </summary>
		private  Sub  readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
			Dim scope_ifname As NetworkInterface = Nothing

			If Me.GetType().classLoader IsNot Nothing Then Throw New SecurityException("invalid address type")

			Dim gf As java.io.ObjectInputStream.GetField = s.readFields()
			Dim ipaddress As SByte() = CType(gf.get("ipaddress", Nothing), SByte())
			Dim scope_id As Integer = CInt(Fix(gf.get("scope_id", -1)))
			Dim scope_id_set As Boolean = CBool(gf.get("scope_id_set", False))
			Dim scope_ifname_set As Boolean = CBool(gf.get("scope_ifname_set", False))
			Dim ifname As String = CStr(gf.get("ifname", Nothing))

			If ifname IsNot Nothing AndAlso (Not "".Equals(ifname)) Then
				Try
					scope_ifname = NetworkInterface.getByName(ifname)
					If scope_ifname Is Nothing Then
	'                     the interface does not exist on this system, so we clear
	'                     * the scope information completely 
						scope_id_set = False
						scope_ifname_set = False
						scope_id = 0
					Else
						scope_ifname_set = True
						Try
							scope_id = deriveNumericScope(ipaddress, scope_ifname)
						Catch e As UnknownHostException
							' typically should not happen, but it may be that
							' the machine being used for deserialization has
							' the same interface name but without IPv6 configured.
						End Try
					End If
				Catch e As SocketException
				End Try
			End If

			' if ifname was not supplied, then the numeric info is used 

			ipaddress = ipaddress.clone()

			' Check that our invariants are satisfied
			If ipaddress.Length <> INADDRSZ Then Throw New java.io.InvalidObjectException("invalid address length: " & ipaddress.Length)

			If holder_Renamed.family <> IPv6 Then Throw New java.io.InvalidObjectException("invalid address family type")

			Dim h As New Inet6AddressHolder(Me, ipaddress, scope_id, scope_id_set, scope_ifname, scope_ifname_set)

			UNSAFE.putObject(Me, FIELDS_OFFSET, h)

		''' <summary>
		''' default behavior is overridden in order to write the
		''' scope_ifname field as a String, rather than a NetworkInterface
		''' which is not serializable
		''' </summary>
		private synchronized  Sub  writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
				Dim ifname As String = Nothing

			If holder6.scope_ifname IsNot Nothing Then
				ifname = holder6.scope_ifname.name
				holder6.scope_ifname_set = True
			End If
			Dim pfields As java.io.ObjectOutputStream.PutField = s.putFields()
			pfields.put("ipaddress", holder6.ipaddress)
			pfields.put("scope_id", holder6.scope_id)
			pfields.put("scope_id_set", holder6.scope_id_set)
			pfields.put("scope_ifname_set", holder6.scope_ifname_set)
			pfields.put("ifname", ifname)
			s.writeFields()

		''' <summary>
		''' Utility routine to check if the InetAddress is an IP multicast
		''' address. 11111111 at the start of the address identifies the
		''' address as being a multicast address.
		''' </summary>
		''' <returns> a {@code boolean} indicating if the InetAddress is an IP
		'''         multicast address
		''' 
		''' @since JDK1.1 </returns>
		public Boolean multicastAddress
			Return holder6.multicastAddress

		''' <summary>
		''' Utility routine to check if the InetAddress in a wildcard address.
		''' </summary>
		''' <returns> a {@code boolean} indicating if the Inetaddress is
		'''         a wildcard address.
		''' 
		''' @since 1.4 </returns>
		public Boolean anyLocalAddress
			Return holder6.anyLocalAddress

		''' <summary>
		''' Utility routine to check if the InetAddress is a loopback address.
		''' </summary>
		''' <returns> a {@code boolean} indicating if the InetAddress is a loopback
		'''         address; or false otherwise.
		''' 
		''' @since 1.4 </returns>
		public Boolean loopbackAddress
			Return holder6.loopbackAddress

		''' <summary>
		''' Utility routine to check if the InetAddress is an link local address.
		''' </summary>
		''' <returns> a {@code boolean} indicating if the InetAddress is a link local
		'''         address; or false if address is not a link local unicast address.
		''' 
		''' @since 1.4 </returns>
		public Boolean linkLocalAddress
			Return holder6.linkLocalAddress

		' static version of above 
		static Boolean isLinkLocalAddress(SByte() ipaddress)
			Return ((ipaddress(0) And &Hff) = &Hfe AndAlso (ipaddress(1) And &Hc0) = &H80)

		''' <summary>
		''' Utility routine to check if the InetAddress is a site local address.
		''' </summary>
		''' <returns> a {@code boolean} indicating if the InetAddress is a site local
		'''         address; or false if address is not a site local unicast address.
		''' 
		''' @since 1.4 </returns>
		public Boolean siteLocalAddress
			Return holder6.siteLocalAddress

		' static version of above 
		static Boolean isSiteLocalAddress(SByte() ipaddress)
			Return ((ipaddress(0) And &Hff) = &Hfe AndAlso (ipaddress(1) And &Hc0) = &Hc0)

		''' <summary>
		''' Utility routine to check if the multicast address has global scope.
		''' </summary>
		''' <returns> a {@code boolean} indicating if the address has is a multicast
		'''         address of global scope, false if it is not of global scope or
		'''         it is not a multicast address
		''' 
		''' @since 1.4 </returns>
		public Boolean mCGlobal
			Return holder6.mCGlobal

		''' <summary>
		''' Utility routine to check if the multicast address has node scope.
		''' </summary>
		''' <returns> a {@code boolean} indicating if the address has is a multicast
		'''         address of node-local scope, false if it is not of node-local
		'''         scope or it is not a multicast address
		''' 
		''' @since 1.4 </returns>
		public Boolean mCNodeLocal
			Return holder6.mCNodeLocal

		''' <summary>
		''' Utility routine to check if the multicast address has link scope.
		''' </summary>
		''' <returns> a {@code boolean} indicating if the address has is a multicast
		'''         address of link-local scope, false if it is not of link-local
		'''         scope or it is not a multicast address
		''' 
		''' @since 1.4 </returns>
		public Boolean mCLinkLocal
			Return holder6.mCLinkLocal

		''' <summary>
		''' Utility routine to check if the multicast address has site scope.
		''' </summary>
		''' <returns> a {@code boolean} indicating if the address has is a multicast
		'''         address of site-local scope, false if it is not  of site-local
		'''         scope or it is not a multicast address
		''' 
		''' @since 1.4 </returns>
		public Boolean mCSiteLocal
			Return holder6.mCSiteLocal

		''' <summary>
		''' Utility routine to check if the multicast address has organization scope.
		''' </summary>
		''' <returns> a {@code boolean} indicating if the address has is a multicast
		'''         address of organization-local scope, false if it is not of
		'''         organization-local scope or it is not a multicast address
		''' 
		''' @since 1.4 </returns>
		public Boolean mCOrgLocal
			Return holder6.mCOrgLocal
		''' <summary>
		''' Returns the raw IP address of this {@code InetAddress} object. The result
		''' is in network byte order: the highest order byte of the address is in
		''' {@code getAddress()[0]}.
		''' </summary>
		''' <returns>  the raw IP address of this object. </returns>
		public SByte() address
			Return holder6.ipaddress.clone()

		''' <summary>
		''' Returns the numeric scopeId, if this instance is associated with
		''' an interface. If no scoped_id is set, the returned value is zero.
		''' </summary>
		''' <returns> the scopeId, or zero if not set.
		''' 
		''' @since 1.5 </returns>
		 public Integer scopeId
			Return holder6.scope_id

		''' <summary>
		''' Returns the scoped interface, if this instance was created with
		''' with a scoped interface.
		''' </summary>
		''' <returns> the scoped interface, or null if not set.
		''' @since 1.5 </returns>
		 public NetworkInterface scopedInterface
			Return holder6.scope_ifname

		''' <summary>
		''' Returns the IP address string in textual presentation. If the instance
		''' was created specifying a scope identifier then the scope id is appended
		''' to the IP address preceded by a "%" (per-cent) character. This can be
		''' either a numeric value or a string, depending on which was used to create
		''' the instance.
		''' </summary>
		''' <returns>  the raw IP address in a string format. </returns>
		public String hostAddress
			Return holder6.hostAddress

		''' <summary>
		''' Returns a hashcode for this IP address.
		''' </summary>
		''' <returns>  a hash code value for this IP address. </returns>
		public Integer GetHashCode()
			Return holder6.GetHashCode()

		''' <summary>
		''' Compares this object against the specified object. The result is {@code
		''' true} if and only if the argument is not {@code null} and it represents
		''' the same IP address as this object.
		''' 
		''' <p> Two instances of {@code InetAddress} represent the same IP address
		''' if the length of the byte arrays returned by {@code getAddress} is the
		''' same for both, and each of the array components is the same for the byte
		''' arrays.
		''' </summary>
		''' <param name="obj">   the object to compare against.
		''' </param>
		''' <returns>  {@code true} if the objects are the same; {@code false} otherwise.
		''' </returns>
		''' <seealso cref=     java.net.InetAddress#getAddress() </seealso>
		public Boolean Equals(Object obj)
			If obj Is Nothing OrElse Not(TypeOf obj Is Inet6Address) Then Return False

			Dim inetAddr As Inet6Address = CType(obj, Inet6Address)

			Return holder6.Equals(inetAddr.holder6)

		''' <summary>
		''' Utility routine to check if the InetAddress is an
		''' IPv4 compatible IPv6 address.
		''' </summary>
		''' <returns> a {@code boolean} indicating if the InetAddress is an IPv4
		'''         compatible IPv6 address; or false if address is IPv4 address.
		''' 
		''' @since 1.4 </returns>
		public Boolean iPv4CompatibleAddress
			Return holder6.iPv4CompatibleAddress

		' Utilities
		private final static Integer INT16SZ = 2

	'    
	'     * Convert IPv6 binary address into presentation (printable) format.
	'     *
	'     * @param src a byte array representing the IPv6 numeric address
	'     * @return a String representing an IPv6 address in
	'     *         textual representation format
	'     * @since 1.4
	'     
		static String numericToTextFormat(SByte() src)
			Dim sb As New StringBuilder(39)
			For i As Integer = 0 To (INADDRSZ \ INT16SZ) - 1
				sb.append( java.lang.[Integer].toHexString(((src(i<<1)<<8) And &Hff00) Or (src((i<<1)+1) And &Hff)))
				If i < (INADDRSZ \ INT16SZ) -1 Then sb.append(":")
			Next i
			Return sb.ToString()

		''' <summary>
		''' Perform class load-time initializations.
		''' </summary>
		private static native  Sub  init()
	End Class

End Namespace