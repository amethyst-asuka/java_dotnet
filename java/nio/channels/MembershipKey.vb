'
' * Copyright (c) 2007, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.nio.channels


	''' <summary>
	''' A token representing the membership of an Internet Protocol (IP) multicast
	''' group.
	''' 
	''' <p> A membership key may represent a membership to receive all datagrams sent
	''' to the group, or it may be <em>source-specific</em>, meaning that it
	''' represents a membership that receives only datagrams from a specific source
	''' address. Whether or not a membership key is source-specific may be determined
	''' by invoking its <seealso cref="#sourceAddress() sourceAddress"/> method.
	''' 
	''' <p> A membership key is valid upon creation and remains valid until the
	''' membership is dropped by invoking the <seealso cref="#drop() drop"/> method, or
	''' the channel is closed. The validity of the membership key may be tested
	''' by invoking its <seealso cref="#isValid() isValid"/> method.
	''' 
	''' <p> Where a membership key is not source-specific and the underlying operation
	''' system supports source filtering, then the <seealso cref="#block block"/> and {@link
	''' #unblock unblock} methods can be used to block or unblock multicast datagrams
	''' from particular source addresses.
	''' </summary>
	''' <seealso cref= MulticastChannel
	''' 
	''' @since 1.7 </seealso>
	Public MustInherit Class MembershipKey

		''' <summary>
		''' Initializes a new instance of this class.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Tells whether or not this membership is valid.
		''' 
		''' <p> A multicast group membership is valid upon creation and remains
		''' valid until the membership is dropped by invoking the <seealso cref="#drop() drop"/>
		''' method, or the channel is closed.
		''' </summary>
		''' <returns>  {@code true} if this membership key is valid, {@code false}
		'''          otherwise </returns>
		Public MustOverride ReadOnly Property valid As Boolean

		''' <summary>
		''' Drop membership.
		''' 
		''' <p> If the membership key represents a membership to receive all datagrams
		''' then the membership is dropped and the channel will no longer receive any
		''' datagrams sent to the group. If the membership key is source-specific
		''' then the channel will no longer receive datagrams sent to the group from
		''' that source address.
		''' 
		''' <p> After membership is dropped it may still be possible to receive
		''' datagrams sent to the group. This can arise when datagrams are waiting to
		''' be received in the socket's receive buffer. After membership is dropped
		''' then the channel may <seealso cref="MulticastChannel#join join"/> the group again
		''' in which case a new membership key is returned.
		''' 
		''' <p> Upon return, this membership object will be <seealso cref="#isValid() invalid"/>.
		''' If the multicast group membership is already invalid then invoking this
		''' method has no effect. Once a multicast group membership is invalid,
		''' it remains invalid forever.
		''' </summary>
		Public MustOverride Sub drop()

		''' <summary>
		''' Block multicast datagrams from the given source address.
		''' 
		''' <p> If this membership key is not source-specific, and the underlying
		''' operating system supports source filtering, then this method blocks
		''' multicast datagrams from the given source address. If the given source
		''' address is already blocked then this method has no effect.
		''' After a source address is blocked it may still be possible to receive
		''' datagrams from that source. This can arise when datagrams are waiting to
		''' be received in the socket's receive buffer.
		''' </summary>
		''' <param name="source">
		'''          The source address to block
		''' </param>
		''' <returns>  This membership key
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''          If the {@code source} parameter is not a unicast address or
		'''          is not the same address type as the multicast group </exception>
		''' <exception cref="IllegalStateException">
		'''          If this membership key is source-specific or is no longer valid </exception>
		''' <exception cref="UnsupportedOperationException">
		'''          If the underlying operating system does not support source
		'''          filtering </exception>
		''' <exception cref="IOException">
		'''          If an I/O error occurs </exception>
		Public MustOverride Function block(ByVal source As java.net.InetAddress) As MembershipKey

		''' <summary>
		''' Unblock multicast datagrams from the given source address that was
		''' previously blocked using the <seealso cref="#block(InetAddress) block"/> method.
		''' </summary>
		''' <param name="source">
		'''          The source address to unblock
		''' </param>
		''' <returns>  This membership key
		''' </returns>
		''' <exception cref="IllegalStateException">
		'''          If the given source address is not currently blocked or the
		'''          membership key is no longer valid </exception>
		Public MustOverride Function unblock(ByVal source As java.net.InetAddress) As MembershipKey

		''' <summary>
		''' Returns the channel for which this membership key was created. This
		''' method will continue to return the channel even after the membership
		''' becomes <seealso cref="#isValid invalid"/>.
		''' </summary>
		''' <returns>  the channel </returns>
		Public MustOverride Function channel() As MulticastChannel

		''' <summary>
		''' Returns the multicast group for which this membership key was created.
		''' This method will continue to return the group even after the membership
		''' becomes <seealso cref="#isValid invalid"/>.
		''' </summary>
		''' <returns>  the multicast group </returns>
		Public MustOverride Function group() As java.net.InetAddress

		''' <summary>
		''' Returns the network interface for which this membership key was created.
		''' This method will continue to return the network interface even after the
		''' membership becomes <seealso cref="#isValid invalid"/>.
		''' </summary>
		''' <returns>  the network interface </returns>
		Public MustOverride Function networkInterface() As java.net.NetworkInterface

		''' <summary>
		''' Returns the source address if this membership key is source-specific,
		''' or {@code null} if this membership is not source-specific.
		''' </summary>
		''' <returns>  The source address if this membership key is source-specific,
		'''          otherwise {@code null} </returns>
		Public MustOverride Function sourceAddress() As java.net.InetAddress
	End Class

End Namespace