Imports System

'
' * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' This class represents a proxy setting, typically a type (http, socks) and
	''' a socket address.
	''' A {@code Proxy} is an immutable object.
	''' </summary>
	''' <seealso cref=     java.net.ProxySelector
	''' @author Yingxian Wang
	''' @author Jean-Christophe Collet
	''' @since   1.5 </seealso>
	Public Class Proxy

		''' <summary>
		''' Represents the proxy type.
		''' 
		''' @since 1.5
		''' </summary>
		Public Enum Type
			''' <summary>
			''' Represents a direct connection, or the absence of a proxy.
			''' </summary>
			DIRECT
			''' <summary>
			''' Represents proxy for high level protocols such as HTTP or FTP.
			''' </summary>
			HTTP
			''' <summary>
			''' Represents a SOCKS (V4 or V5) proxy.
			''' </summary>
			SOCKS
		End Enum

		Private type_Renamed As Type
		Private sa As SocketAddress

		''' <summary>
		''' A proxy setting that represents a {@code DIRECT} connection,
		''' basically telling the protocol handler not to use any proxying.
		''' Used, for instance, to create sockets bypassing any other global
		''' proxy settings (like SOCKS):
		''' <P>
		''' {@code Socket s = new Socket(Proxy.NO_PROXY);}
		''' 
		''' </summary>
		Public Shared ReadOnly NO_PROXY As New Proxy

		' Creates the proxy that represents a {@code DIRECT} connection.
		Private Sub New()
			type_Renamed = Type.DIRECT
			sa = Nothing
		End Sub

		''' <summary>
		''' Creates an entry representing a PROXY connection.
		''' Certain combinations are illegal. For instance, for types Http, and
		''' Socks, a SocketAddress <b>must</b> be provided.
		''' <P>
		''' Use the {@code Proxy.NO_PROXY} constant
		''' for representing a direct connection.
		''' </summary>
		''' <param name="type"> the {@code Type} of the proxy </param>
		''' <param name="sa"> the {@code SocketAddress} for that proxy </param>
		''' <exception cref="IllegalArgumentException"> when the type and the address are
		''' incompatible </exception>
		Public Sub New(  type As Type,   sa As SocketAddress)
			If (type = Type.DIRECT) OrElse Not(TypeOf sa Is InetSocketAddress) Then Throw New IllegalArgumentException("type " & type & " is not compatible with address " & sa)
			Me.type_Renamed = type
			Me.sa = sa
		End Sub

		''' <summary>
		''' Returns the proxy type.
		''' </summary>
		''' <returns> a Type representing the proxy type </returns>
		Public Overridable Function type() As Type
			Return type_Renamed
		End Function

		''' <summary>
		''' Returns the socket address of the proxy, or
		''' {@code null} if its a direct connection.
		''' </summary>
		''' <returns> a {@code SocketAddress} representing the socket end
		'''         point of the proxy </returns>
		Public Overridable Function address() As SocketAddress
			Return sa
		End Function

		''' <summary>
		''' Constructs a string representation of this Proxy.
		''' This String is constructed by calling toString() on its type
		''' and concatenating " @ " and the toString() result from its address
		''' if its type is not {@code DIRECT}.
		''' </summary>
		''' <returns>  a string representation of this object. </returns>
		Public Overrides Function ToString() As String
			If type() = Type.DIRECT Then Return "DIRECT"
			Return type() & " @ " & address()
		End Function

			''' <summary>
			''' Compares this object against the specified object.
			''' The result is {@code true} if and only if the argument is
			''' not {@code null} and it represents the same proxy as
			''' this object.
			''' <p>
			''' Two instances of {@code Proxy} represent the same
			''' address if both the SocketAddresses and type are equal.
			''' </summary>
			''' <param name="obj">   the object to compare against. </param>
			''' <returns>  {@code true} if the objects are the same;
			'''          {@code false} otherwise. </returns>
			''' <seealso cref= java.net.InetSocketAddress#equals(java.lang.Object) </seealso>
		Public NotOverridable Overrides Function Equals(  obj As Object) As Boolean
			If obj Is Nothing OrElse Not(TypeOf obj Is Proxy) Then Return False
			Dim p As Proxy = CType(obj, Proxy)
			If p.type() = type() Then
				If address() Is Nothing Then
					Return (p.address() Is Nothing)
				Else
					Return address().Equals(p.address())
				End If
			End If
			Return False
		End Function

		''' <summary>
		''' Returns a hashcode for this Proxy.
		''' </summary>
		''' <returns>  a hash code value for this Proxy. </returns>
		Public NotOverridable Overrides Function GetHashCode() As Integer
			If address() Is Nothing Then Return type().GetHashCode()
			Return type().GetHashCode() + address().GetHashCode()
		End Function
	End Class

End Namespace