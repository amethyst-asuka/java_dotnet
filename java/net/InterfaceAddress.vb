'
' * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' This class represents a Network Interface address. In short it's an
	''' IP address, a subnet mask and a broadcast address when the address is
	''' an IPv4 one. An IP address and a network prefix length in the case
	''' of IPv6 address.
	''' </summary>
	''' <seealso cref= java.net.NetworkInterface
	''' @since 1.6 </seealso>
	Public Class InterfaceAddress
		Private address As InetAddress = Nothing
		Private broadcast As Inet4Address = Nothing
		Private maskLength As Short = 0

	'    
	'     * Package private constructor. Can't be built directly, instances are
	'     * obtained through the NetworkInterface class.
	'     
		Friend Sub New()
		End Sub

		''' <summary>
		''' Returns an {@code InetAddress} for this address.
		''' </summary>
		''' <returns> the {@code InetAddress} for this address. </returns>
		Public Overridable Property address As InetAddress
			Get
				Return address
			End Get
		End Property

		''' <summary>
		''' Returns an {@code InetAddress} for the broadcast address
		''' for this InterfaceAddress.
		''' <p>
		''' Only IPv4 networks have broadcast address therefore, in the case
		''' of an IPv6 network, {@code null} will be returned.
		''' </summary>
		''' <returns> the {@code InetAddress} representing the broadcast
		'''         address or {@code null} if there is no broadcast address. </returns>
		Public Overridable Property broadcast As InetAddress
			Get
				Return broadcast
			End Get
		End Property

		''' <summary>
		''' Returns the network prefix length for this address. This is also known
		''' as the subnet mask in the context of IPv4 addresses.
		''' Typical IPv4 values would be 8 (255.0.0.0), 16 (255.255.0.0)
		''' or 24 (255.255.255.0). <p>
		''' Typical IPv6 values would be 128 (::1/128) or 10 (fe80::203:baff:fe27:1243/10)
		''' </summary>
		''' <returns> a {@code short} representing the prefix length for the
		'''         subnet of that address. </returns>
		 Public Overridable Property networkPrefixLength As Short
			 Get
				Return maskLength
			 End Get
		 End Property

		''' <summary>
		''' Compares this object against the specified object.
		''' The result is {@code true} if and only if the argument is
		''' not {@code null} and it represents the same interface address as
		''' this object.
		''' <p>
		''' Two instances of {@code InterfaceAddress} represent the same
		''' address if the InetAddress, the prefix length and the broadcast are
		''' the same for both.
		''' </summary>
		''' <param name="obj">   the object to compare against. </param>
		''' <returns>  {@code true} if the objects are the same;
		'''          {@code false} otherwise. </returns>
		''' <seealso cref=     java.net.InterfaceAddress#hashCode() </seealso>
		Public Overrides Function Equals(  obj As Object) As Boolean
			If Not(TypeOf obj Is InterfaceAddress) Then Return False
			Dim cmp As InterfaceAddress = CType(obj, InterfaceAddress)
			If Not(If(address Is Nothing, cmp.address Is Nothing, address.Equals(cmp.address))) Then Return False
			If Not(If(broadcast Is Nothing, cmp.broadcast Is Nothing, broadcast.Equals(cmp.broadcast))) Then Return False
			If maskLength <> cmp.maskLength Then Return False
			Return True
		End Function

		''' <summary>
		''' Returns a hashcode for this Interface address.
		''' </summary>
		''' <returns>  a hash code value for this Interface address. </returns>
		Public Overrides Function GetHashCode() As Integer
			Return address.GetHashCode() + (If(broadcast IsNot Nothing, broadcast.GetHashCode(), 0)) + maskLength
		End Function

		''' <summary>
		''' Converts this Interface address to a {@code String}. The
		''' string returned is of the form: InetAddress / prefix length [ broadcast address ].
		''' </summary>
		''' <returns>  a string representation of this Interface address. </returns>
		Public Overrides Function ToString() As String
			Return address & "/" & maskLength & " [" & broadcast & "]"
		End Function

	End Class

End Namespace