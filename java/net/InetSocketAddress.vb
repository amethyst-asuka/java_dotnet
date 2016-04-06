Imports System

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


	''' 
	''' <summary>
	''' This class implements an IP Socket Address (IP address + port number)
	''' It can also be a pair (hostname + port number), in which case an attempt
	''' will be made to resolve the hostname. If resolution fails then the address
	''' is said to be <I>unresolved</I> but can still be used on some circumstances
	''' like connecting through a proxy.
	''' <p>
	''' It provides an immutable object used by sockets for binding, connecting, or
	''' as returned values.
	''' <p>
	''' The <i>wildcard</i> is a special local IP address. It usually means "any"
	''' and can only be used for {@code bind} operations.
	''' </summary>
	''' <seealso cref= java.net.Socket </seealso>
	''' <seealso cref= java.net.ServerSocket
	''' @since 1.4 </seealso>
	Public Class InetSocketAddress
		Inherits SocketAddress

		' Private implementation class pointed to by all public methods.
		Private Class InetSocketAddressHolder
			' The hostname of the Socket Address
			Private hostname As String
			' The IP address of the Socket Address
			Private addr As InetAddress
			' The port number of the Socket Address
			Private port As Integer

			Private Sub New(  hostname As String,   addr As InetAddress,   port As Integer)
				Me.hostname = hostname
				Me.addr = addr
				Me.port = port
			End Sub

			Private Property port As Integer
				Get
					Return port
				End Get
			End Property

			Private Property address As InetAddress
				Get
					Return addr
				End Get
			End Property

			Private Property hostName As String
				Get
					If hostname IsNot Nothing Then Return hostname
					If addr IsNot Nothing Then Return addr.hostName
					Return Nothing
				End Get
			End Property

			Private Property hostString As String
				Get
					If hostname IsNot Nothing Then Return hostname
					If addr IsNot Nothing Then
						If addr.holder().hostName IsNot Nothing Then
							Return addr.holder().hostName
						Else
							Return addr.hostAddress
						End If
					End If
					Return Nothing
				End Get
			End Property

			Private Property unresolved As Boolean
				Get
					Return addr Is Nothing
				End Get
			End Property

			Public Overrides Function ToString() As String
				If unresolved Then
					Return hostname & ":" & port
				Else
					Return addr.ToString() & ":" & port
				End If
			End Function

			Public NotOverridable Overrides Function Equals(  obj As Object) As Boolean
				If obj Is Nothing OrElse Not(TypeOf obj Is InetSocketAddressHolder) Then Return False
				Dim that As InetSocketAddressHolder = CType(obj, InetSocketAddressHolder)
				Dim sameIP As Boolean
				If addr IsNot Nothing Then
					sameIP = addr.Equals(that.addr)
				ElseIf hostname IsNot Nothing Then
					sameIP = (that.addr Is Nothing) AndAlso hostname.equalsIgnoreCase(that.hostname)
				Else
					sameIP = (that.addr Is Nothing) AndAlso (that.hostname Is Nothing)
				End If
				Return sameIP AndAlso (port = that.port)
			End Function

			Public NotOverridable Overrides Function GetHashCode() As Integer
				If addr IsNot Nothing Then Return addr.GetHashCode() + port
				If hostname IsNot Nothing Then Return hostname.ToLower().GetHashCode() + port
				Return port
			End Function
		End Class

		<NonSerialized> _
		Private ReadOnly holder As InetSocketAddressHolder

		Private Shadows Const serialVersionUID As Long = 5076001401234631237L

		Private Shared Function checkPort(  port As Integer) As Integer
			If port < 0 OrElse port > &HFFFF Then Throw New IllegalArgumentException("port out of range:" & port)
			Return port
		End Function

		Private Shared Function checkHost(  hostname As String) As String
			If hostname Is Nothing Then Throw New IllegalArgumentException("hostname can't be null")
			Return hostname
		End Function

		''' <summary>
		''' Creates a socket address where the IP address is the wildcard address
		''' and the port number a specified value.
		''' <p>
		''' A valid port value is between 0 and 65535.
		''' A port number of {@code zero} will let the system pick up an
		''' ephemeral port in a {@code bind} operation.
		''' <p> </summary>
		''' <param name="port">    The port number </param>
		''' <exception cref="IllegalArgumentException"> if the port parameter is outside the specified
		''' range of valid port values. </exception>
		Public Sub New(  port As Integer)
			Me.New(InetAddress.anyLocalAddress(), port)
		End Sub

		''' 
		''' <summary>
		''' Creates a socket address from an IP address and a port number.
		''' <p>
		''' A valid port value is between 0 and 65535.
		''' A port number of {@code zero} will let the system pick up an
		''' ephemeral port in a {@code bind} operation.
		''' <P>
		''' A {@code null} address will assign the <i>wildcard</i> address.
		''' <p> </summary>
		''' <param name="addr">    The IP address </param>
		''' <param name="port">    The port number </param>
		''' <exception cref="IllegalArgumentException"> if the port parameter is outside the specified
		''' range of valid port values. </exception>
		Public Sub New(  addr As InetAddress,   port As Integer)
			holder = New InetSocketAddressHolder(Nothing,If(addr Is Nothing, InetAddress.anyLocalAddress(), addr), checkPort(port))
		End Sub

		''' 
		''' <summary>
		''' Creates a socket address from a hostname and a port number.
		''' <p>
		''' An attempt will be made to resolve the hostname into an InetAddress.
		''' If that attempt fails, the address will be flagged as <I>unresolved</I>.
		''' <p>
		''' If there is a security manager, its {@code checkConnect} method
		''' is called with the host name as its argument to check the permission
		''' to resolve it. This could result in a SecurityException.
		''' <P>
		''' A valid port value is between 0 and 65535.
		''' A port number of {@code zero} will let the system pick up an
		''' ephemeral port in a {@code bind} operation.
		''' <P> </summary>
		''' <param name="hostname"> the Host name </param>
		''' <param name="port">    The port number </param>
		''' <exception cref="IllegalArgumentException"> if the port parameter is outside the range
		''' of valid port values, or if the hostname parameter is <TT>null</TT>. </exception>
		''' <exception cref="SecurityException"> if a security manager is present and
		'''                           permission to resolve the host name is
		'''                           denied. </exception>
		''' <seealso cref=     #isUnresolved() </seealso>
		Public Sub New(  hostname As String,   port As Integer)
			checkHost(hostname)
			Dim addr As InetAddress = Nothing
			Dim host As String = Nothing
			Try
				addr = InetAddress.getByName(hostname)
			Catch e As UnknownHostException
				host = hostname
			End Try
			holder = New InetSocketAddressHolder(host, addr, checkPort(port))
		End Sub

		' private constructor for creating unresolved instances
		Private Sub New(  port As Integer,   hostname As String)
			holder = New InetSocketAddressHolder(hostname, Nothing, port)
		End Sub

		''' 
		''' <summary>
		''' Creates an unresolved socket address from a hostname and a port number.
		''' <p>
		''' No attempt will be made to resolve the hostname into an InetAddress.
		''' The address will be flagged as <I>unresolved</I>.
		''' <p>
		''' A valid port value is between 0 and 65535.
		''' A port number of {@code zero} will let the system pick up an
		''' ephemeral port in a {@code bind} operation.
		''' <P> </summary>
		''' <param name="host">    the Host name </param>
		''' <param name="port">    The port number </param>
		''' <exception cref="IllegalArgumentException"> if the port parameter is outside
		'''                  the range of valid port values, or if the hostname
		'''                  parameter is <TT>null</TT>. </exception>
		''' <seealso cref=     #isUnresolved() </seealso>
		''' <returns>  a {@code InetSocketAddress} representing the unresolved
		'''          socket address
		''' @since 1.5 </returns>
		Public Shared Function createUnresolved(  host As String,   port As Integer) As InetSocketAddress
			Return New InetSocketAddress(checkPort(port), checkHost(host))
		End Function

		''' <summary>
		''' @serialField hostname String
		''' @serialField addr InetAddress
		''' @serialField port int
		''' </summary>
		Private Shared ReadOnly serialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("hostname", GetType(String)), New java.io.ObjectStreamField("addr", GetType(InetAddress)), New java.io.ObjectStreamField("port", GetType(Integer))}

		Private Sub writeObject(  out As java.io.ObjectOutputStream)
			' Don't call defaultWriteObject()
			 Dim pfields As java.io.ObjectOutputStream.PutField = out.putFields()
			 pfields.put("hostname", holder.hostname)
			 pfields.put("addr", holder.addr)
			 pfields.put("port", holder.port)
			 out.writeFields()
		End Sub

		Private Sub readObject(  [in] As java.io.ObjectInputStream)
			' Don't call defaultReadObject()
			Dim oisFields As java.io.ObjectInputStream.GetField = [in].readFields()
			Dim oisHostname As String = CStr(oisFields.get("hostname", Nothing))
			Dim oisAddr As InetAddress = CType(oisFields.get("addr", Nothing), InetAddress)
			Dim oisPort As Integer = oisFields.get("port", -1)

			' Check that our invariants are satisfied
			checkPort(oisPort)
			If oisHostname Is Nothing AndAlso oisAddr Is Nothing Then Throw New java.io.InvalidObjectException("hostname and addr " & "can't both be null")

			Dim h As New InetSocketAddressHolder(oisHostname, oisAddr, oisPort)
			UNSAFE.putObject(Me, FIELDS_OFFSET, h)
		End Sub

		Private Sub readObjectNoData()
			Throw New java.io.InvalidObjectException("Stream data required")
		End Sub

		Private Shared ReadOnly FIELDS_OFFSET As Long
		Private Shared ReadOnly UNSAFE As sun.misc.Unsafe
		Shared Sub New()
			Try
				Dim unsafe_Renamed As sun.misc.Unsafe = sun.misc.Unsafe.unsafe
				FIELDS_OFFSET = unsafe_Renamed.objectFieldOffset(GetType(InetSocketAddress).getDeclaredField("holder"))
				UNSAFE = unsafe_Renamed
			Catch e As ReflectiveOperationException
				Throw New [Error](e)
			End Try
		End Sub

		''' <summary>
		''' Gets the port number.
		''' </summary>
		''' <returns> the port number. </returns>
		Public Property port As Integer
			Get
				Return holder.port
			End Get
		End Property

		''' 
		''' <summary>
		''' Gets the {@code InetAddress}.
		''' </summary>
		''' <returns> the InetAdress or {@code null} if it is unresolved. </returns>
		Public Property address As InetAddress
			Get
				Return holder.address
			End Get
		End Property

		''' <summary>
		''' Gets the {@code hostname}.
		''' Note: This method may trigger a name service reverse lookup if the
		''' address was created with a literal IP address.
		''' </summary>
		''' <returns>  the hostname part of the address. </returns>
		Public Property hostName As String
			Get
				Return holder.hostName
			End Get
		End Property

		''' <summary>
		''' Returns the hostname, or the String form of the address if it
		''' doesn't have a hostname (it was created using a literal).
		''' This has the benefit of <b>not</b> attempting a reverse lookup.
		''' </summary>
		''' <returns> the hostname, or String representation of the address.
		''' @since 1.7 </returns>
		Public Property hostString As String
			Get
				Return holder.hostString
			End Get
		End Property

		''' <summary>
		''' Checks whether the address has been resolved or not.
		''' </summary>
		''' <returns> {@code true} if the hostname couldn't be resolved into
		'''          an {@code InetAddress}. </returns>
		Public Property unresolved As Boolean
			Get
				Return holder.unresolved
			End Get
		End Property

		''' <summary>
		''' Constructs a string representation of this InetSocketAddress.
		''' This String is constructed by calling toString() on the InetAddress
		''' and concatenating the port number (with a colon). If the address
		''' is unresolved then the part before the colon will only contain the hostname.
		''' </summary>
		''' <returns>  a string representation of this object. </returns>
		Public Overrides Function ToString() As String
			Return holder.ToString()
		End Function

		''' <summary>
		''' Compares this object against the specified object.
		''' The result is {@code true} if and only if the argument is
		''' not {@code null} and it represents the same address as
		''' this object.
		''' <p>
		''' Two instances of {@code InetSocketAddress} represent the same
		''' address if both the InetAddresses (or hostnames if it is unresolved) and port
		''' numbers are equal.
		''' If both addresses are unresolved, then the hostname and the port number
		''' are compared.
		''' 
		''' Note: Hostnames are case insensitive. e.g. "FooBar" and "foobar" are
		''' considered equal.
		''' </summary>
		''' <param name="obj">   the object to compare against. </param>
		''' <returns>  {@code true} if the objects are the same;
		'''          {@code false} otherwise. </returns>
		''' <seealso cref= java.net.InetAddress#equals(java.lang.Object) </seealso>
		Public NotOverridable Overrides Function Equals(  obj As Object) As Boolean
			If obj Is Nothing OrElse Not(TypeOf obj Is InetSocketAddress) Then Return False
			Return holder.Equals(CType(obj, InetSocketAddress).holder)
		End Function

		''' <summary>
		''' Returns a hashcode for this socket address.
		''' </summary>
		''' <returns>  a hash code value for this socket address. </returns>
		Public NotOverridable Overrides Function GetHashCode() As Integer
			Return holder.GetHashCode()
		End Function
	End Class

End Namespace