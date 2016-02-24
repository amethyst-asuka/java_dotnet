'
' * Copyright (c) 2002, 2005, Oracle and/or its affiliates. All rights reserved.
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
	' * Package private interface to "implementation" used by
	' * {@link InetAddress}.
	' * <p>
	' * See {@link java.net.Inet4AddressImp} and
	' * {@link java.net.Inet6AddressImp}.
	' *
	' * @since 1.4
	' 
	Friend Interface InetAddressImpl

		ReadOnly Property localHostName As String
		Function lookupAllHostAddr(ByVal hostname As String) As InetAddress()
		Function getHostByAddr(ByVal addr As SByte()) As String

		Function anyLocalAddress() As InetAddress
		Function loopbackAddress() As InetAddress
		Function isReachable(ByVal addr As InetAddress, ByVal timeout As Integer, ByVal netif As NetworkInterface, ByVal ttl As Integer) As Boolean
	End Interface

End Namespace