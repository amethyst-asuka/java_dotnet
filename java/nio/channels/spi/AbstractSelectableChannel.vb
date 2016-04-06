Imports System.Diagnostics

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

Namespace java.nio.channels.spi



	''' <summary>
	''' Base implementation class for selectable channels.
	''' 
	''' <p> This class defines methods that handle the mechanics of channel
	''' registration, deregistration, and closing.  It maintains the current
	''' blocking mode of this channel as well as its current set of selection keys.
	''' It performs all of the synchronization required to implement the {@link
	''' java.nio.channels.SelectableChannel} specification.  Implementations of the
	''' abstract protected methods defined in this class need not synchronize
	''' against other threads that might be engaged in the same operations.  </p>
	''' 
	''' 
	''' @author Mark Reinhold
	''' @author Mike McCloskey
	''' @author JSR-51 Expert Group
	''' @since 1.4
	''' </summary>

	Public MustInherit Class AbstractSelectableChannel
		Inherits SelectableChannel

		' The provider that created this channel
		Private ReadOnly provider_Renamed As SelectorProvider

		' Keys that have been created by registering this channel with selectors.
		' They are saved because if this channel is closed the keys must be
		' deregistered.  Protected by keyLock.
		'
		Private keys As SelectionKey() = Nothing
		Private keyCount As Integer = 0

		' Lock for key set and count
		Private ReadOnly keyLock As New Object

		' Lock for registration and configureBlocking operations
		Private ReadOnly regLock As New Object

		' Blocking mode, protected by regLock
		Friend blocking As Boolean = True

		''' <summary>
		''' Initializes a new instance of this class.
		''' </summary>
		''' <param name="provider">
		'''         The provider that created this channel </param>
		Protected Friend Sub New(  provider As SelectorProvider)
			Me.provider_Renamed = provider
		End Sub

		''' <summary>
		''' Returns the provider that created this channel.
		''' </summary>
		''' <returns>  The provider that created this channel </returns>
		Public NotOverridable Overrides Function provider() As SelectorProvider
			Return provider_Renamed
		End Function


		' -- Utility methods for the key set --

		Private Sub addKey(  k As SelectionKey)
			Debug.Assert(Thread.holdsLock(keyLock))
			Dim i As Integer = 0
			If (keys IsNot Nothing) AndAlso (keyCount < keys.Length) Then
				' Find empty element of key array
				For i = 0 To keys.Length - 1
					If keys(i) Is Nothing Then Exit For
				Next i
			ElseIf keys Is Nothing Then
				keys = New SelectionKey(2){}
			Else
				' Grow key array
				Dim n As Integer = keys.Length * 2
				Dim ks As SelectionKey() = New SelectionKey(n - 1){}
				For i = 0 To keys.Length - 1
					ks(i) = keys(i)
				Next i
				keys = ks
				i = keyCount
			End If
			keys(i) = k
			keyCount += 1
		End Sub

		Private Function findKey(  sel As Selector) As SelectionKey
			SyncLock keyLock
				If keys Is Nothing Then Return Nothing
				For i As Integer = 0 To keys.Length - 1
					If (keys(i) IsNot Nothing) AndAlso (keys(i).selector() Is sel) Then Return keys(i)
				Next i
				Return Nothing
			End SyncLock
		End Function

		Friend Overridable Sub removeKey(  k As SelectionKey) ' package-private
			SyncLock keyLock
				For i As Integer = 0 To keys.Length - 1
					If keys(i) Is k Then
						keys(i) = Nothing
						keyCount -= 1
					End If
				Next i
				CType(k, AbstractSelectionKey).invalidate()
			End SyncLock
		End Sub

		Private Function haveValidKeys() As Boolean
			SyncLock keyLock
				If keyCount = 0 Then Return False
				For i As Integer = 0 To keys.Length - 1
					If (keys(i) IsNot Nothing) AndAlso keys(i).valid Then Return True
				Next i
				Return False
			End SyncLock
		End Function


		' -- Registration --

		Public Property NotOverridable Overrides registered As Boolean
			Get
				SyncLock keyLock
					Return keyCount <> 0
				End SyncLock
			End Get
		End Property

		Public NotOverridable Overrides Function keyFor(  sel As Selector) As SelectionKey
			Return findKey(sel)
		End Function

		''' <summary>
		''' Registers this channel with the given selector, returning a selection key.
		''' 
		''' <p>  This method first verifies that this channel is open and that the
		''' given initial interest set is valid.
		''' 
		''' <p> If this channel is already registered with the given selector then
		''' the selection key representing that registration is returned after
		''' setting its interest set to the given value.
		''' 
		''' <p> Otherwise this channel has not yet been registered with the given
		''' selector, so the <seealso cref="AbstractSelector#register register"/> method of
		''' the selector is invoked while holding the appropriate locks.  The
		''' resulting key is added to this channel's key set before being returned.
		''' </p>
		''' </summary>
		''' <exception cref="ClosedSelectorException"> {@inheritDoc}
		''' </exception>
		''' <exception cref="IllegalBlockingModeException"> {@inheritDoc}
		''' </exception>
		''' <exception cref="IllegalSelectorException"> {@inheritDoc}
		''' </exception>
		''' <exception cref="CancelledKeyException"> {@inheritDoc}
		''' </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		Public NotOverridable Overrides Function register(  sel As Selector,   ops As Integer,   att As Object) As SelectionKey
			SyncLock regLock
				If Not open Then Throw New ClosedChannelException
				If (ops And (Not validOps())) <> 0 Then Throw New IllegalArgumentException
				If blocking Then Throw New IllegalBlockingModeException
				Dim k As SelectionKey = findKey(sel)
				If k IsNot Nothing Then
					k.interestOps(ops)
					k.attach(att)
				End If
				If k Is Nothing Then
					' New registration
					SyncLock keyLock
						If Not open Then Throw New ClosedChannelException
						k = CType(sel, AbstractSelector).register(Me, ops, att)
						addKey(k)
					End SyncLock
				End If
				Return k
			End SyncLock
		End Function


		' -- Closing --

		''' <summary>
		''' Closes this channel.
		''' 
		''' <p> This method, which is specified in the {@link
		''' AbstractInterruptibleChannel} class and is invoked by the {@link
		''' java.nio.channels.Channel#close close} method, in turn invokes the
		''' <seealso cref="#implCloseSelectableChannel implCloseSelectableChannel"/> method in
		''' order to perform the actual work of closing this channel.  It then
		''' cancels all of this channel's keys.  </p>
		''' </summary>
		Protected Friend NotOverridable Overrides Sub implCloseChannel()
			implCloseSelectableChannel()
			SyncLock keyLock
				Dim count As Integer = If(keys Is Nothing, 0, keys.Length)
				For i As Integer = 0 To count - 1
					Dim k As SelectionKey = keys(i)
					If k IsNot Nothing Then k.cancel()
				Next i
			End SyncLock
		End Sub

		''' <summary>
		''' Closes this selectable channel.
		''' 
		''' <p> This method is invoked by the {@link java.nio.channels.Channel#close
		''' close} method in order to perform the actual work of closing the
		''' channel.  This method is only invoked if the channel has not yet been
		''' closed, and it is never invoked more than once.
		''' 
		''' <p> An implementation of this method must arrange for any other thread
		''' that is blocked in an I/O operation upon this channel to return
		''' immediately, either by throwing an exception or by returning normally.
		''' </p>
		''' </summary>
		''' <exception cref="IOException">
		'''          If an I/O error occurs </exception>
		Protected Friend MustOverride Sub implCloseSelectableChannel()


		' -- Blocking --

		Public Property NotOverridable Overrides blocking As Boolean
			Get
				SyncLock regLock
					Return blocking
				End SyncLock
			End Get
		End Property

		Public NotOverridable Overrides Function blockingLock() As Object
			Return regLock
		End Function

		''' <summary>
		''' Adjusts this channel's blocking mode.
		''' 
		''' <p> If the given blocking mode is different from the current blocking
		''' mode then this method invokes the {@link #implConfigureBlocking
		''' implConfigureBlocking} method, while holding the appropriate locks, in
		''' order to change the mode.  </p>
		''' </summary>
		Public NotOverridable Overrides Function configureBlocking(  block As Boolean) As SelectableChannel
			SyncLock regLock
				If Not open Then Throw New ClosedChannelException
				If blocking = block Then Return Me
				If block AndAlso haveValidKeys() Then Throw New IllegalBlockingModeException
				implConfigureBlocking(block)
				blocking = block
			End SyncLock
			Return Me
		End Function

		''' <summary>
		''' Adjusts this channel's blocking mode.
		''' 
		''' <p> This method is invoked by the {@link #configureBlocking
		''' configureBlocking} method in order to perform the actual work of
		''' changing the blocking mode.  This method is only invoked if the new mode
		''' is different from the current mode.  </p>
		''' </summary>
		''' <param name="block">  If <tt>true</tt> then this channel will be placed in
		'''                blocking mode; if <tt>false</tt> then it will be placed
		'''                non-blocking mode
		''' </param>
		''' <exception cref="IOException">
		'''         If an I/O error occurs </exception>
		Protected Friend MustOverride Sub implConfigureBlocking(  block As Boolean)

	End Class

End Namespace