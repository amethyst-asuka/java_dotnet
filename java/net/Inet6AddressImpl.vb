Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System.Runtime.InteropServices

'
' * Copyright (c) 2002, 2013, Oracle and/or its affiliates. All rights reserved.
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

	'
	' * Package private implementation of InetAddressImpl for dual
	' * IPv4/IPv6 stack.
	' * <p>
	' * If InetAddress.preferIPv6Address is true then anyLocalAddress(),
	' * loopbackAddress(), and localHost() will return IPv6 addresses,
	' * otherwise IPv4 addresses.
	' *
	' * @since 1.4
	' 

	Friend Class Inet6AddressImpl
		Implements InetAddressImpl

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Function getLocalHostName() As String
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Function lookupAllHostAddr(ByVal hostname As String) As InetAddress()
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Public Function getHostByAddr(ByVal addr As SByte()) As String
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Function isReachable0(ByVal addr As SByte(), ByVal scope As Integer, ByVal timeout As Integer, ByVal inf As SByte(), ByVal ttl As Integer, ByVal if_scope As Integer) As Boolean
		End Function

		Public Overridable Function isReachable(ByVal addr As InetAddress, ByVal timeout As Integer, ByVal netif As NetworkInterface, ByVal ttl As Integer) As Boolean Implements InetAddressImpl.isReachable
			Dim ifaddr As SByte() = Nothing
			Dim scope As Integer = -1
			Dim netif_scope As Integer = -1
			If netif IsNot Nothing Then
	'            
	'             * Let's make sure we bind to an address of the proper family.
	'             * Which means same family as addr because at this point it could
	'             * be either an IPv6 address or an IPv4 address (case of a dual
	'             * stack system).
	'             
				Dim it As System.Collections.IEnumerator(Of InetAddress) = netif.inetAddresses
				Dim inetaddr As InetAddress = Nothing
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Do While it.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					inetaddr = it.nextElement()
					If inetaddr.GetType().IsInstanceOfType(addr) Then
						ifaddr = inetaddr.address
						If TypeOf inetaddr Is Inet6Address Then netif_scope = CType(inetaddr, Inet6Address).scopeId
						Exit Do
					End If
				Loop
				If ifaddr Is Nothing Then Return False
			End If
			If TypeOf addr Is Inet6Address Then scope = CType(addr, Inet6Address).scopeId
			Return isReachable0(addr.address, scope, timeout, ifaddr, ttl, netif_scope)
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function anyLocalAddress() As InetAddress Implements InetAddressImpl.anyLocalAddress
			If anyLocalAddress_Renamed Is Nothing Then
				If InetAddress.preferIPv6Address Then
					anyLocalAddress_Renamed = New Inet6Address
					anyLocalAddress_Renamed.holder().hostName = "::"
				Else
					anyLocalAddress_Renamed = (New Inet4AddressImpl).anyLocalAddress()
				End If
			End If
			Return anyLocalAddress_Renamed
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function loopbackAddress() As InetAddress Implements InetAddressImpl.loopbackAddress
			If loopbackAddress_Renamed Is Nothing Then
				 If InetAddress.preferIPv6Address Then
					 Dim loopback As SByte() = {&H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H1}
					 loopbackAddress_Renamed = New Inet6Address("localhost", loopback)
				 Else
					loopbackAddress_Renamed = (New Inet4AddressImpl).loopbackAddress()
				 End If
			End If
			Return loopbackAddress_Renamed
		End Function

		Private anyLocalAddress_Renamed As InetAddress
		Private loopbackAddress_Renamed As InetAddress
	End Class

End Namespace