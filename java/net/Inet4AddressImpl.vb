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
	' * Package private implementation of InetAddressImpl for IPv4.
	' *
	' * @since 1.4
	' 
	Friend Class Inet4AddressImpl
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
		Private Function isReachable0(ByVal addr As SByte(), ByVal timeout As Integer, ByVal ifaddr As SByte(), ByVal ttl As Integer) As Boolean
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function anyLocalAddress() As InetAddress Implements InetAddressImpl.anyLocalAddress
			If anyLocalAddress_Renamed Is Nothing Then
				anyLocalAddress_Renamed = New Inet4Address ' {0x00,0x00,0x00,0x00}
				anyLocalAddress_Renamed.holder().hostName = "0.0.0.0"
			End If
			Return anyLocalAddress_Renamed
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function loopbackAddress() As InetAddress Implements InetAddressImpl.loopbackAddress
			If loopbackAddress_Renamed Is Nothing Then
				Dim loopback As SByte() = {&H7f,&H0,&H0,&H1}
				loopbackAddress_Renamed = New Inet4Address("localhost", loopback)
			End If
			Return loopbackAddress_Renamed
		End Function

	  Public Overridable Function isReachable(ByVal addr As InetAddress, ByVal timeout As Integer, ByVal netif As NetworkInterface, ByVal ttl As Integer) As Boolean Implements InetAddressImpl.isReachable
		  Dim ifaddr As SByte() = Nothing
		  If netif IsNot Nothing Then
	'          
	'           * Let's make sure we use an address of the proper family
	'           
			  Dim it As System.Collections.IEnumerator(Of InetAddress) = netif.inetAddresses
			  Dim inetaddr As InetAddress = Nothing
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			  Do While Not(TypeOf inetaddr Is Inet4Address) AndAlso it.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				  inetaddr = it.nextElement()
			  Loop
			  If TypeOf inetaddr Is Inet4Address Then ifaddr = inetaddr.address
		  End If
		  Return isReachable0(addr.address, timeout, ifaddr, ttl)
	  End Function
		Private anyLocalAddress_Renamed As InetAddress
		Private loopbackAddress_Renamed As InetAddress
	End Class

End Namespace