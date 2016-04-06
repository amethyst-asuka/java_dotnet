Imports System.Runtime.InteropServices

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
	''' This class represents an Internet Protocol version 4 (IPv4) address.
	''' Defined by <a href="http://www.ietf.org/rfc/rfc790.txt">
	''' <i>RFC&nbsp;790: Assigned Numbers</i></a>,
	''' <a href="http://www.ietf.org/rfc/rfc1918.txt">
	''' <i>RFC&nbsp;1918: Address Allocation for Private Internets</i></a>,
	''' and <a href="http://www.ietf.org/rfc/rfc2365.txt"><i>RFC&nbsp;2365:
	''' Administratively Scoped IP Multicast</i></a>
	''' 
	''' <h3> <A NAME="format">Textual representation of IP addresses</a> </h3>
	''' 
	''' Textual representation of IPv4 address used as input to methods
	''' takes one of the following forms:
	''' 
	''' <blockquote><table cellpadding=0 cellspacing=0 summary="layout">
	''' <tr><td>{@code d.d.d.d}</td></tr>
	''' <tr><td>{@code d.d.d}</td></tr>
	''' <tr><td>{@code d.d}</td></tr>
	''' <tr><td>{@code d}</td></tr>
	''' </table></blockquote>
	''' 
	''' <p> When four parts are specified, each is interpreted as a byte of
	''' data and assigned, from left to right, to the four bytes of an IPv4
	''' address.
	''' 
	''' <p> When a three part address is specified, the last part is
	''' interpreted as a 16-bit quantity and placed in the right most two
	''' bytes of the network address. This makes the three part address
	''' format convenient for specifying Class B net- work addresses as
	''' 128.net.host.
	''' 
	''' <p> When a two part address is supplied, the last part is
	''' interpreted as a 24-bit quantity and placed in the right most three
	''' bytes of the network address. This makes the two part address
	''' format convenient for specifying Class A network addresses as
	''' net.host.
	''' 
	''' <p> When only one part is given, the value is stored directly in
	''' the network address without any byte rearrangement.
	''' 
	''' <p> For methods that return a textual representation as output
	''' value, the first form, i.e. a dotted-quad string, is used.
	''' 
	''' <h4> The Scope of a Multicast Address </h4>
	''' 
	''' Historically the IPv4 TTL field in the IP header has doubled as a
	''' multicast scope field: a TTL of 0 means node-local, 1 means
	''' link-local, up through 32 means site-local, up through 64 means
	''' region-local, up through 128 means continent-local, and up through
	''' 255 are global. However, the administrative scoping is preferred.
	''' Please refer to <a href="http://www.ietf.org/rfc/rfc2365.txt">
	''' <i>RFC&nbsp;2365: Administratively Scoped IP Multicast</i></a>
	''' @since 1.4
	''' </summary>

	Public NotInheritable Class Inet4Address
		Inherits InetAddress

		Friend Const INADDRSZ As Integer = 4

		''' <summary>
		''' use serialVersionUID from InetAddress, but Inet4Address instance
		'''  is always replaced by an InetAddress instance before being
		'''  serialized 
		''' </summary>
		Private Const serialVersionUID As Long = 3286316764910316507L

	'    
	'     * Perform initializations.
	'     
		Shared Sub New()
			init()
		End Sub

		Friend Sub New()
			MyBase.New()
			holder().hostName = Nothing
			holder().address = 0
			holder().family = IPv4
		End Sub

		Friend Sub New(  hostName As String,   addr As SByte())
			holder().hostName = hostName
			holder().family = IPv4
			If addr IsNot Nothing Then
				If addr.Length = INADDRSZ Then
					Dim address_Renamed As Integer = addr(3) And &HFF
					address_Renamed = address_Renamed Or ((addr(2) << 8) And &HFF00)
					address_Renamed = address_Renamed Or ((addr(1) << 16) And &HFF0000)
					address_Renamed = address_Renamed Or ((addr(0) << 24) And &HFF000000L)
					holder().address = address_Renamed
				End If
			End If
			holder().originalHostName = hostName
		End Sub
		Friend Sub New(  hostName As String,   address As Integer)
			holder().hostName = hostName
			holder().family = IPv4
			holder().address = address
			holder().originalHostName = hostName
		End Sub

		''' <summary>
		''' Replaces the object to be serialized with an InetAddress object.
		''' </summary>
		''' <returns> the alternate object to be serialized.
		''' </returns>
		''' <exception cref="ObjectStreamException"> if a new object replacing this
		''' object could not be created </exception>
		Private Function writeReplace() As Object
			' will replace the to be serialized 'this' object
			Dim inet As New InetAddress
			inet.holder().hostName = holder().hostName
			inet.holder().address = holder().address

			''' <summary>
			''' Prior to 1.4 an InetAddress was created with a family
			''' based on the platform AF_INET value (usually 2).
			''' For compatibility reasons we must therefore write the
			''' the InetAddress with this family.
			''' </summary>
			inet.holder().family = 2

			Return inet
		End Function

		''' <summary>
		''' Utility routine to check if the InetAddress is an
		''' IP multicast address. IP multicast address is a Class D
		''' address i.e first four bits of the address are 1110. </summary>
		''' <returns> a {@code boolean} indicating if the InetAddress is
		''' an IP multicast address
		''' @since   JDK1.1 </returns>
		Public  Overrides ReadOnly Property  multicastAddress As Boolean
			Get
				Return ((holder().address And &Hf0000000L) = &He0000000L)
			End Get
		End Property

		''' <summary>
		''' Utility routine to check if the InetAddress in a wildcard address. </summary>
		''' <returns> a {@code boolean} indicating if the Inetaddress is
		'''         a wildcard address.
		''' @since 1.4 </returns>
		Public  Overrides ReadOnly Property  anyLocalAddress As Boolean
			Get
				Return holder().address = 0
			End Get
		End Property

		''' <summary>
		''' Utility routine to check if the InetAddress is a loopback address.
		''' </summary>
		''' <returns> a {@code boolean} indicating if the InetAddress is
		''' a loopback address; or false otherwise.
		''' @since 1.4 </returns>
		Public  Overrides ReadOnly Property  loopbackAddress As Boolean
			Get
				' 127.x.x.x 
				Dim byteAddr As SByte() = address
				Return byteAddr(0) = 127
			End Get
		End Property

		''' <summary>
		''' Utility routine to check if the InetAddress is an link local address.
		''' </summary>
		''' <returns> a {@code boolean} indicating if the InetAddress is
		''' a link local address; or false if address is not a link local unicast address.
		''' @since 1.4 </returns>
		Public  Overrides ReadOnly Property  linkLocalAddress As Boolean
			Get
				' link-local unicast in IPv4 (169.254.0.0/16)
				' defined in "Documenting Special Use IPv4 Address Blocks
				' that have been Registered with IANA" by Bill Manning
				' draft-manning-dsua-06.txt
				Dim address_Renamed As Integer = holder().address
				Return (((CInt(CUInt(address_Renamed) >> 24)) And &HFF) = 169) AndAlso (((CInt(CUInt(address_Renamed) >> 16)) And &HFF) = 254)
			End Get
		End Property

		''' <summary>
		''' Utility routine to check if the InetAddress is a site local address.
		''' </summary>
		''' <returns> a {@code boolean} indicating if the InetAddress is
		''' a site local address; or false if address is not a site local unicast address.
		''' @since 1.4 </returns>
		Public  Overrides ReadOnly Property  siteLocalAddress As Boolean
			Get
				' refer to RFC 1918
				' 10/8 prefix
				' 172.16/12 prefix
				' 192.168/16 prefix
				Dim address_Renamed As Integer = holder().address
				Return (((CInt(CUInt(address_Renamed) >> 24)) And &HFF) = 10) OrElse ((((CInt(CUInt(address_Renamed) >> 24)) And &HFF) = 172) AndAlso (((CInt(CUInt(address_Renamed) >> 16)) And &HF0) = 16)) OrElse ((((CInt(CUInt(address_Renamed) >> 24)) And &HFF) = 192) AndAlso (((CInt(CUInt(address_Renamed) >> 16)) And &HFF) = 168))
			End Get
		End Property

		''' <summary>
		''' Utility routine to check if the multicast address has global scope.
		''' </summary>
		''' <returns> a {@code boolean} indicating if the address has
		'''         is a multicast address of global scope, false if it is not
		'''         of global scope or it is not a multicast address
		''' @since 1.4 </returns>
		Public  Overrides ReadOnly Property  mCGlobal As Boolean
			Get
				' 224.0.1.0 to 238.255.255.255
				Dim byteAddr As SByte() = address
				Return ((byteAddr(0) And &Hff) >= 224 AndAlso (byteAddr(0) And &Hff) <= 238) AndAlso Not((byteAddr(0) And &Hff) = 224 AndAlso byteAddr(1) = 0 AndAlso byteAddr(2) = 0)
			End Get
		End Property

		''' <summary>
		''' Utility routine to check if the multicast address has node scope.
		''' </summary>
		''' <returns> a {@code boolean} indicating if the address has
		'''         is a multicast address of node-local scope, false if it is not
		'''         of node-local scope or it is not a multicast address
		''' @since 1.4 </returns>
		Public  Overrides ReadOnly Property  mCNodeLocal As Boolean
			Get
				' unless ttl == 0
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
		Public  Overrides ReadOnly Property  mCLinkLocal As Boolean
			Get
				' 224.0.0/24 prefix and ttl == 1
				Dim address_Renamed As Integer = holder().address
				Return (((CInt(CUInt(address_Renamed) >> 24)) And &HFF) = 224) AndAlso (((CInt(CUInt(address_Renamed) >> 16)) And &HFF) = 0) AndAlso (((CInt(CUInt(address_Renamed) >> 8)) And &HFF) = 0)
			End Get
		End Property

		''' <summary>
		''' Utility routine to check if the multicast address has site scope.
		''' </summary>
		''' <returns> a {@code boolean} indicating if the address has
		'''         is a multicast address of site-local scope, false if it is not
		'''         of site-local scope or it is not a multicast address
		''' @since 1.4 </returns>
		Public  Overrides ReadOnly Property  mCSiteLocal As Boolean
			Get
				' 239.255/16 prefix or ttl < 32
				Dim address_Renamed As Integer = holder().address
				Return (((CInt(CUInt(address_Renamed) >> 24)) And &HFF) = 239) AndAlso (((CInt(CUInt(address_Renamed) >> 16)) And &HFF) = 255)
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
		Public  Overrides ReadOnly Property  mCOrgLocal As Boolean
			Get
				' 239.192 - 239.195
				Dim address_Renamed As Integer = holder().address
				Return (((CInt(CUInt(address_Renamed) >> 24)) And &HFF) = 239) AndAlso (((CInt(CUInt(address_Renamed) >> 16)) And &HFF) >= 192) AndAlso (((CInt(CUInt(address_Renamed) >> 16)) And &HFF) <= 195)
			End Get
		End Property

		''' <summary>
		''' Returns the raw IP address of this {@code InetAddress}
		''' object. The result is in network byte order: the highest order
		''' byte of the address is in {@code getAddress()[0]}.
		''' </summary>
		''' <returns>  the raw IP address of this object. </returns>
		Public  Overrides ReadOnly Property  address As SByte()
			Get
				Dim address_Renamed As Integer = holder().address
				Dim addr As SByte() = New SByte(INADDRSZ - 1){}
    
				addr(0) = CByte((CInt(CUInt(address_Renamed) >> 24)) And &HFF)
				addr(1) = CByte((CInt(CUInt(address_Renamed) >> 16)) And &HFF)
				addr(2) = CByte((CInt(CUInt(address_Renamed) >> 8)) And &HFF)
				addr(3) = CByte(address_Renamed And &HFF)
				Return addr
			End Get
		End Property

		''' <summary>
		''' Returns the IP address string in textual presentation form.
		''' </summary>
		''' <returns>  the raw IP address in a string format.
		''' @since   JDK1.0.2 </returns>
		Public  Overrides ReadOnly Property  hostAddress As String
			Get
				Return numericToTextFormat(address)
			End Get
		End Property

		''' <summary>
		''' Returns a hashcode for this IP address.
		''' </summary>
		''' <returns>  a hash code value for this IP address. </returns>
		Public Overrides Function GetHashCode() As Integer
			Return holder().address
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
		Public Overrides Function Equals(  obj As Object) As Boolean
			Return (obj IsNot Nothing) AndAlso (TypeOf obj Is Inet4Address) AndAlso (CType(obj, InetAddress).holder().address = holder().address)
		End Function

		' Utilities
	'    
	'     * Converts IPv4 binary address into a string suitable for presentation.
	'     *
	'     * @param src a byte array representing an IPv4 numeric address
	'     * @return a String representing the IPv4 address in
	'     *         textual representation format
	'     * @since 1.4
	'     

		Friend Shared Function numericToTextFormat(  src As SByte()) As String
			Return (src(0) And &Hff) & "." & (src(1) And &Hff) & "." & (src(2) And &Hff) & "." & (src(3) And &Hff)
		End Function

		''' <summary>
		''' Perform class load-time initializations.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub init()
		End Sub
	End Class

End Namespace