Imports System.Collections.Generic
Imports System.Runtime.InteropServices
Imports sun.security.action

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
	''' This class represents a Network Interface made up of a name,
	''' and a list of IP addresses assigned to this interface.
	''' It is used to identify the local interface on which a multicast group
	''' is joined.
	''' 
	''' Interfaces are normally known by names such as "le0".
	''' 
	''' @since 1.4
	''' </summary>
	Public NotInheritable Class NetworkInterface
		Private name As String
		Private displayName As String
		Private index As Integer
		Private addrs As InetAddress()
		Private bindings As InterfaceAddress()
		Private childs As NetworkInterface()
		Private parent As NetworkInterface = Nothing
		Private virtual As Boolean = False
		Private Shared ReadOnly defaultInterface_Renamed As NetworkInterface
		Private Shared ReadOnly defaultIndex As Integer ' index of defaultInterface

		Shared Sub New()
			java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)

			init()
			defaultInterface_Renamed = DefaultInterface.default
			If defaultInterface_Renamed IsNot Nothing Then
				defaultIndex = defaultInterface_Renamed.index
			Else
				defaultIndex = 0
			End If
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
		''' Returns an NetworkInterface object with index set to 0 and name to null.
		''' Setting such an interface on a MulticastSocket will cause the
		''' kernel to choose one interface for sending multicast packets.
		''' 
		''' </summary>
		Friend Sub New()
		End Sub

		Friend Sub New(ByVal name As String, ByVal index As Integer, ByVal addrs As InetAddress())
			Me.name = name
			Me.index = index
			Me.addrs = addrs
		End Sub

		''' <summary>
		''' Get the name of this network interface.
		''' </summary>
		''' <returns> the name of this network interface </returns>
		Public Property name As String
			Get
					Return name
			End Get
		End Property

		''' <summary>
		''' Convenience method to return an Enumeration with all or a
		''' subset of the InetAddresses bound to this network interface.
		''' <p>
		''' If there is a security manager, its {@code checkConnect}
		''' method is called for each InetAddress. Only InetAddresses where
		''' the {@code checkConnect} doesn't throw a SecurityException
		''' will be returned in the Enumeration. However, if the caller has the
		''' <seealso cref="NetPermission"/>("getNetworkInformation") permission, then all
		''' InetAddresses are returned. </summary>
		''' <returns> an Enumeration object with all or a subset of the InetAddresses
		''' bound to this network interface </returns>
		Public Property inetAddresses As System.Collections.IEnumerator(Of InetAddress)
			Get
    
	'JAVA TO VB CONVERTER TODO TASK: Local classes are not converted by Java to VB Converter:
	'			class checkedAddresses implements java.util.Enumeration(Of InetAddress)
		'		{
		'
		'			private int i=0, count=0;
		'			private InetAddress local_addrs[];
		'
		'			checkedAddresses()
		'			{
		'				local_addrs = New InetAddress[addrs.length];
		'				boolean trusted = True;
		'
		'				SecurityManager sec = System.getSecurityManager();
		'				if (sec != Nothing)
		'				{
		'					try
		'					{
		'						sec.checkPermission(New NetPermission("getNetworkInformation"));
		'					}
		'					catch (SecurityException e)
		'					{
		'						trusted = False;
		'					}
		'				}
		'				for (int j=0; j<addrs.length; j += 1)
		'				{
		'					try
		'					{
		'						if (sec != Nothing && !trusted)
		'						{
		'							sec.checkConnect(addrs[j].getHostAddress(), -1);
		'						}
		'						local_addrs[count] = addrs[j];
		'						count += 1;
		'					}
		'					catch (SecurityException e)
		'					{
		'					}
		'				}
		'
		'			}
		'
		'			public InetAddress nextElement()
		'			{
		'				if (i < count)
		'				{
		'					Return local_addrs[i++];
		'				}
		'				else
		'				{
		'					throw New NoSuchElementException();
		'				}
		'			}
		'
		'			public boolean hasMoreElements()
		'			{
		'				Return (i < count);
		'			}
		'		}
				Return New checkedAddresses
    
			End Get
		End Property

		''' <summary>
		''' Get a List of all or a subset of the {@code InterfaceAddresses}
		''' of this network interface.
		''' <p>
		''' If there is a security manager, its {@code checkConnect}
		''' method is called with the InetAddress for each InterfaceAddress.
		''' Only InterfaceAddresses where the {@code checkConnect} doesn't throw
		''' a SecurityException will be returned in the List.
		''' </summary>
		''' <returns> a {@code List} object with all or a subset of the
		'''         InterfaceAddresss of this network interface
		''' @since 1.6 </returns>
		Public Property interfaceAddresses As IList(Of InterfaceAddress)
			Get
				Dim lst As IList(Of InterfaceAddress) = New List(Of InterfaceAddress)(1)
				Dim sec As SecurityManager = System.securityManager
				For j As Integer = 0 To bindings.Length - 1
					Try
						If sec IsNot Nothing Then sec.checkConnect(bindings(j).address.hostAddress, -1)
						lst.Add(bindings(j))
					Catch e As SecurityException
					End Try
				Next j
				Return lst
			End Get
		End Property

		''' <summary>
		''' Get an Enumeration with all the subinterfaces (also known as virtual
		''' interfaces) attached to this network interface.
		''' <p>
		''' For instance eth0:1 will be a subinterface to eth0.
		''' </summary>
		''' <returns> an Enumeration object with all of the subinterfaces
		''' of this network interface
		''' @since 1.6 </returns>
		Public Property subInterfaces As System.Collections.IEnumerator(Of NetworkInterface)
			Get
	'JAVA TO VB CONVERTER TODO TASK: Local classes are not converted by Java to VB Converter:
	'			class subIFs implements java.util.Enumeration(Of NetworkInterface)
		'		{
		'
		'			private int i=0;
		'
		'			subIFs()
		'			{
		'			}
		'
		'			public NetworkInterface nextElement()
		'			{
		'				if (i < childs.length)
		'				{
		'					Return childs[i++];
		'				}
		'				else
		'				{
		'					throw New NoSuchElementException();
		'				}
		'			}
		'
		'			public boolean hasMoreElements()
		'			{
		'				Return (i < childs.length);
		'			}
		'		}
				Return New subIFs
    
			End Get
		End Property

		''' <summary>
		''' Returns the parent NetworkInterface of this interface if this is
		''' a subinterface, or {@code null} if it is a physical
		''' (non virtual) interface or has no parent.
		''' </summary>
		''' <returns> The {@code NetworkInterface} this interface is attached to.
		''' @since 1.6 </returns>
		Public Property parent As NetworkInterface
			Get
				Return parent
			End Get
		End Property

		''' <summary>
		''' Returns the index of this network interface. The index is an integer greater
		''' or equal to zero, or {@code -1} for unknown. This is a system specific value
		''' and interfaces with the same name can have different indexes on different
		''' machines.
		''' </summary>
		''' <returns> the index of this network interface or {@code -1} if the index is
		'''         unknown </returns>
		''' <seealso cref= #getByIndex(int)
		''' @since 1.7 </seealso>
		Public Property index As Integer
			Get
				Return index
			End Get
		End Property

		''' <summary>
		''' Get the display name of this network interface.
		''' A display name is a human readable String describing the network
		''' device.
		''' </summary>
		''' <returns> a non-empty string representing the display name of this network
		'''         interface, or null if no display name is available. </returns>
		Public Property displayName As String
			Get
				' strict TCK conformance 
				Return If("".Equals(displayName), Nothing, displayName)
			End Get
		End Property

		''' <summary>
		''' Searches for the network interface with the specified name.
		''' </summary>
		''' <param name="name">
		'''          The name of the network interface.
		''' </param>
		''' <returns>  A {@code NetworkInterface} with the specified name,
		'''          or {@code null} if there is no network interface
		'''          with the specified name.
		''' </returns>
		''' <exception cref="SocketException">
		'''          If an I/O error occurs.
		''' </exception>
		''' <exception cref="NullPointerException">
		'''          If the specified name is {@code null}. </exception>
		Public Shared Function getByName(ByVal name As String) As NetworkInterface
			If name Is Nothing Then Throw New NullPointerException
			Return getByName0(name)
		End Function

		''' <summary>
		''' Get a network interface given its index.
		''' </summary>
		''' <param name="index"> an integer, the index of the interface </param>
		''' <returns> the NetworkInterface obtained from its index, or {@code null} if
		'''         there is no interface with such an index on the system </returns>
		''' <exception cref="SocketException">  if an I/O error occurs. </exception>
		''' <exception cref="IllegalArgumentException"> if index has a negative value </exception>
		''' <seealso cref= #getIndex()
		''' @since 1.7 </seealso>
		Public Shared Function getByIndex(ByVal index As Integer) As NetworkInterface
			If index < 0 Then Throw New IllegalArgumentException("Interface index can't be negative")
			Return getByIndex0(index)
		End Function

		''' <summary>
		''' Convenience method to search for a network interface that
		''' has the specified Internet Protocol (IP) address bound to
		''' it.
		''' <p>
		''' If the specified IP address is bound to multiple network
		''' interfaces it is not defined which network interface is
		''' returned.
		''' </summary>
		''' <param name="addr">
		'''          The {@code InetAddress} to search with.
		''' </param>
		''' <returns>  A {@code NetworkInterface}
		'''          or {@code null} if there is no network interface
		'''          with the specified IP address.
		''' </returns>
		''' <exception cref="SocketException">
		'''          If an I/O error occurs.
		''' </exception>
		''' <exception cref="NullPointerException">
		'''          If the specified address is {@code null}. </exception>
		Public Shared Function getByInetAddress(ByVal addr As InetAddress) As NetworkInterface
			If addr Is Nothing Then Throw New NullPointerException
			If Not(TypeOf addr Is Inet4Address OrElse TypeOf addr Is Inet6Address) Then Throw New IllegalArgumentException("invalid address type")
			Return getByInetAddress0(addr)
		End Function

		''' <summary>
		''' Returns all the interfaces on this machine. The {@code Enumeration}
		''' contains at least one element, possibly representing a loopback
		''' interface that only supports communication between entities on
		''' this machine.
		''' 
		''' NOTE: can use getNetworkInterfaces()+getInetAddresses()
		'''       to obtain all IP addresses for this node
		''' </summary>
		''' <returns> an Enumeration of NetworkInterfaces found on this machine </returns>
		''' <exception cref="SocketException">  if an I/O error occurs. </exception>

		Public Property Shared networkInterfaces As System.Collections.IEnumerator(Of NetworkInterface)
			Get
				Dim netifs As NetworkInterface() = all
    
				' specified to return null if no network interfaces
				If netifs Is Nothing Then Return Nothing
    
				Return New EnumerationAnonymousInnerClassHelper(Of E)
			End Get
		End Property

		Private Class EnumerationAnonymousInnerClassHelper(Of E)
			Implements System.Collections.IEnumerator(Of E)

			Private i As Integer = 0
			Public Overridable Function nextElement() As NetworkInterface
				If netifs IsNot Nothing AndAlso i < netifs.length Then
					Dim netif As NetworkInterface = netifs(i)
					i += 1
					Return netif
				Else
					Throw New java.util.NoSuchElementException
				End If
			End Function

			Public Overridable Function hasMoreElements() As Boolean
				Return (netifs IsNot Nothing AndAlso i < netifs.length)
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function getAll() As NetworkInterface()
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function getByName0(ByVal name As String) As NetworkInterface
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function getByIndex0(ByVal index As Integer) As NetworkInterface
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function getByInetAddress0(ByVal addr As InetAddress) As NetworkInterface
		End Function

		''' <summary>
		''' Returns whether a network interface is up and running.
		''' </summary>
		''' <returns>  {@code true} if the interface is up and running. </returns>
		''' <exception cref="SocketException"> if an I/O error occurs.
		''' @since 1.6 </exception>

		Public Property up As Boolean
			Get
				Return isUp0(name, index)
			End Get
		End Property

		''' <summary>
		''' Returns whether a network interface is a loopback interface.
		''' </summary>
		''' <returns>  {@code true} if the interface is a loopback interface. </returns>
		''' <exception cref="SocketException"> if an I/O error occurs.
		''' @since 1.6 </exception>

		Public Property loopback As Boolean
			Get
				Return isLoopback0(name, index)
			End Get
		End Property

		''' <summary>
		''' Returns whether a network interface is a point to point interface.
		''' A typical point to point interface would be a PPP connection through
		''' a modem.
		''' </summary>
		''' <returns>  {@code true} if the interface is a point to point
		'''          interface. </returns>
		''' <exception cref="SocketException"> if an I/O error occurs.
		''' @since 1.6 </exception>

		Public Property pointToPoint As Boolean
			Get
				Return isP2P0(name, index)
			End Get
		End Property

		''' <summary>
		''' Returns whether a network interface supports multicasting or not.
		''' </summary>
		''' <returns>  {@code true} if the interface supports Multicasting. </returns>
		''' <exception cref="SocketException"> if an I/O error occurs.
		''' @since 1.6 </exception>

		Public Function supportsMulticast() As Boolean
			Return supportsMulticast0(name, index)
		End Function

		''' <summary>
		''' Returns the hardware address (usually MAC) of the interface if it
		''' has one and if it can be accessed given the current privileges.
		''' If a security manager is set, then the caller must have
		''' the permission <seealso cref="NetPermission"/>("getNetworkInformation").
		''' </summary>
		''' <returns>  a byte array containing the address, or {@code null} if
		'''          the address doesn't exist, is not accessible or a security
		'''          manager is set and the caller does not have the permission
		'''          NetPermission("getNetworkInformation")
		''' </returns>
		''' <exception cref="SocketException"> if an I/O error occurs.
		''' @since 1.6 </exception>
		Public Property hardwareAddress As SByte()
			Get
				Dim sec As SecurityManager = System.securityManager
				If sec IsNot Nothing Then
					Try
						sec.checkPermission(New NetPermission("getNetworkInformation"))
					Catch e As SecurityException
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						If Not inetAddresses.hasMoreElements() Then Return Nothing
					End Try
				End If
				For Each addr As InetAddress In addrs
					If TypeOf addr Is Inet4Address Then Return getMacAddr0(CType(addr, Inet4Address).address, name, index)
				Next addr
				Return getMacAddr0(Nothing, name, index)
			End Get
		End Property

		''' <summary>
		''' Returns the Maximum Transmission Unit (MTU) of this interface.
		''' </summary>
		''' <returns> the value of the MTU for that interface. </returns>
		''' <exception cref="SocketException"> if an I/O error occurs.
		''' @since 1.6 </exception>
		Public Property mTU As Integer
			Get
				Return getMTU0(name, index)
			End Get
		End Property

		''' <summary>
		''' Returns whether this interface is a virtual interface (also called
		''' subinterface).
		''' Virtual interfaces are, on some systems, interfaces created as a child
		''' of a physical interface and given different settings (like address or
		''' MTU). Usually the name of the interface will the name of the parent
		''' followed by a colon (:) and a number identifying the child since there
		''' can be several virtual interfaces attached to a single physical
		''' interface.
		''' </summary>
		''' <returns> {@code true} if this interface is a virtual interface.
		''' @since 1.6 </returns>
		Public Property virtual As Boolean
			Get
				Return virtual
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function isUp0(ByVal name As String, ByVal ind As Integer) As Boolean
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function isLoopback0(ByVal name As String, ByVal ind As Integer) As Boolean
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function supportsMulticast0(ByVal name As String, ByVal ind As Integer) As Boolean
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function isP2P0(ByVal name As String, ByVal ind As Integer) As Boolean
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function getMacAddr0(ByVal inAddr As SByte(), ByVal name As String, ByVal ind As Integer) As SByte()
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function getMTU0(ByVal name As String, ByVal ind As Integer) As Integer
		End Function

		''' <summary>
		''' Compares this object against the specified object.
		''' The result is {@code true} if and only if the argument is
		''' not {@code null} and it represents the same NetworkInterface
		''' as this object.
		''' <p>
		''' Two instances of {@code NetworkInterface} represent the same
		''' NetworkInterface if both name and addrs are the same for both.
		''' </summary>
		''' <param name="obj">   the object to compare against. </param>
		''' <returns>  {@code true} if the objects are the same;
		'''          {@code false} otherwise. </returns>
		''' <seealso cref=     java.net.InetAddress#getAddress() </seealso>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Not(TypeOf obj Is NetworkInterface) Then Return False
			Dim that As NetworkInterface = CType(obj, NetworkInterface)
			If Me.name IsNot Nothing Then
				If Not Me.name.Equals(that.name) Then Return False
			Else
				If that.name IsNot Nothing Then Return False
			End If

			If Me.addrs Is Nothing Then
				Return that.addrs Is Nothing
			ElseIf that.addrs Is Nothing Then
				Return False
			End If

			' Both addrs not null. Compare number of addresses 

			If Me.addrs.Length <> that.addrs.Length Then Return False

			Dim thatAddrs As InetAddress() = that.addrs
			Dim count As Integer = thatAddrs.Length

			For i As Integer = 0 To count - 1
				Dim found As Boolean = False
				For j As Integer = 0 To count - 1
					If addrs(i).Equals(thatAddrs(j)) Then
						found = True
						Exit For
					End If
				Next j
				If Not found Then Return False
			Next i
			Return True
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return If(name Is Nothing, 0, name.GetHashCode())
		End Function

		Public Overrides Function ToString() As String
			Dim result As String = "name:"
			result += If(name Is Nothing, "null", name)
			If displayName IsNot Nothing Then result &= " (" & displayName & ")"
			Return result
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub init()
		End Sub

		''' <summary>
		''' Returns the default network interface of this system
		''' </summary>
		''' <returns> the default interface </returns>
		Shared [default] As NetworkInterface
			Get
				Return defaultInterface_Renamed
			End Get
		End Property
	End Class

End Namespace