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

Namespace java.nio.channels



	''' <summary>
	''' A channel that can be multiplexed via a <seealso cref="Selector"/>.
	''' 
	''' <p> In order to be used with a selector, an instance of this class must
	''' first be <i>registered</i> via the {@link #register(Selector,int,Object)
	''' register} method.  This method returns a new <seealso cref="SelectionKey"/> object
	''' that represents the channel's registration with the selector.
	''' 
	''' <p> Once registered with a selector, a channel remains registered until it
	''' is <i>deregistered</i>.  This involves deallocating whatever resources were
	''' allocated to the channel by the selector.
	''' 
	''' <p> A channel cannot be deregistered directly; instead, the key representing
	''' its registration must be <i>cancelled</i>.  Cancelling a key requests that
	''' the channel be deregistered during the selector's next selection operation.
	''' A key may be cancelled explicitly by invoking its {@link
	''' SelectionKey#cancel() cancel} method.  All of a channel's keys are cancelled
	''' implicitly when the channel is closed, whether by invoking its {@link
	''' Channel#close close} method or by interrupting a thread blocked in an I/O
	''' operation upon the channel.
	''' 
	''' <p> If the selector itself is closed then the channel will be deregistered,
	''' and the key representing its registration will be invalidated, without
	''' further delay.
	''' 
	''' <p> A channel may be registered at most once with any particular selector.
	''' 
	''' <p> Whether or not a channel is registered with one or more selectors may be
	''' determined by invoking the <seealso cref="#isRegistered isRegistered"/> method.
	''' 
	''' <p> Selectable channels are safe for use by multiple concurrent
	''' threads. </p>
	''' 
	''' 
	''' <a name="bm"></a>
	''' <h2>Blocking mode</h2>
	''' 
	''' A selectable channel is either in <i>blocking</i> mode or in
	''' <i>non-blocking</i> mode.  In blocking mode, every I/O operation invoked
	''' upon the channel will block until it completes.  In non-blocking mode an I/O
	''' operation will never block and may transfer fewer bytes than were requested
	''' or possibly no bytes at all.  The blocking mode of a selectable channel may
	''' be determined by invoking its <seealso cref="#isBlocking isBlocking"/> method.
	''' 
	''' <p> Newly-created selectable channels are always in blocking mode.
	''' Non-blocking mode is most useful in conjunction with selector-based
	''' multiplexing.  A channel must be placed into non-blocking mode before being
	''' registered with a selector, and may not be returned to blocking mode until
	''' it has been deregistered.
	''' 
	''' 
	''' @author Mark Reinhold
	''' @author JSR-51 Expert Group
	''' @since 1.4
	''' </summary>
	''' <seealso cref= SelectionKey </seealso>
	''' <seealso cref= Selector </seealso>

	Public MustInherit Class SelectableChannel
		Inherits java.nio.channels.spi.AbstractInterruptibleChannel
		Implements Channel

		''' <summary>
		''' Initializes a new instance of this class.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Returns the provider that created this channel.
		''' </summary>
		''' <returns>  The provider that created this channel </returns>
		Public MustOverride Function provider() As java.nio.channels.spi.SelectorProvider

		''' <summary>
		''' Returns an <a href="SelectionKey.html#opsets">operation set</a>
		''' identifying this channel's supported operations.  The bits that are set
		''' in this integer value denote exactly the operations that are valid for
		''' this channel.  This method always returns the same value for a given
		''' concrete channel class.
		''' </summary>
		''' <returns>  The valid-operation set </returns>
		Public MustOverride Function validOps() As Integer

		' Internal state:
		'   keySet, may be empty but is never null, typ. a tiny array
		'   boolean isRegistered, protected by key set
		'   regLock, lock object to prevent duplicate registrations
		'   boolean isBlocking, protected by regLock

		''' <summary>
		''' Tells whether or not this channel is currently registered with any
		''' selectors.  A newly-created channel is not registered.
		''' 
		''' <p> Due to the inherent delay between key cancellation and channel
		''' deregistration, a channel may remain registered for some time after all
		''' of its keys have been cancelled.  A channel may also remain registered
		''' for some time after it is closed.  </p>
		''' </summary>
		''' <returns> <tt>true</tt> if, and only if, this channel is registered </returns>
		Public MustOverride ReadOnly Property registered As Boolean
		'
		' sync(keySet) { return isRegistered; }

		''' <summary>
		''' Retrieves the key representing the channel's registration with the given
		''' selector.
		''' </summary>
		''' <param name="sel">
		'''          The selector
		''' </param>
		''' <returns>  The key returned when this channel was last registered with the
		'''          given selector, or <tt>null</tt> if this channel is not
		'''          currently registered with that selector </returns>
		Public MustOverride Function keyFor(  sel As Selector) As SelectionKey
		'
		' sync(keySet) { return findKey(sel); }

		''' <summary>
		''' Registers this channel with the given selector, returning a selection
		''' key.
		''' 
		''' <p> If this channel is currently registered with the given selector then
		''' the selection key representing that registration is returned.  The key's
		''' interest set will have been changed to <tt>ops</tt>, as if by invoking
		''' the <seealso cref="SelectionKey#interestOps(int) interestOps(int)"/> method.  If
		''' the <tt>att</tt> argument is not <tt>null</tt> then the key's attachment
		''' will have been set to that value.  A <seealso cref="CancelledKeyException"/> will
		''' be thrown if the key has already been cancelled.
		''' 
		''' <p> Otherwise this channel has not yet been registered with the given
		''' selector, so it is registered and the resulting new key is returned.
		''' The key's initial interest set will be <tt>ops</tt> and its attachment
		''' will be <tt>att</tt>.
		''' 
		''' <p> This method may be invoked at any time.  If this method is invoked
		''' while another invocation of this method or of the {@link
		''' #configureBlocking(boolean) configureBlocking} method is in progress
		''' then it will first block until the other operation is complete.  This
		''' method will then synchronize on the selector's key set and therefore may
		''' block if invoked concurrently with another registration or selection
		''' operation involving the same selector. </p>
		''' 
		''' <p> If this channel is closed while this operation is in progress then
		''' the key returned by this method will have been cancelled and will
		''' therefore be invalid. </p>
		''' </summary>
		''' <param name="sel">
		'''         The selector with which this channel is to be registered
		''' </param>
		''' <param name="ops">
		'''         The interest set for the resulting key
		''' </param>
		''' <param name="att">
		'''         The attachment for the resulting key; may be <tt>null</tt>
		''' </param>
		''' <exception cref="ClosedChannelException">
		'''          If this channel is closed
		''' </exception>
		''' <exception cref="ClosedSelectorException">
		'''          If the selector is closed
		''' </exception>
		''' <exception cref="IllegalBlockingModeException">
		'''          If this channel is in blocking mode
		''' </exception>
		''' <exception cref="IllegalSelectorException">
		'''          If this channel was not created by the same provider
		'''          as the given selector
		''' </exception>
		''' <exception cref="CancelledKeyException">
		'''          If this channel is currently registered with the given selector
		'''          but the corresponding key has already been cancelled
		''' </exception>
		''' <exception cref="IllegalArgumentException">
		'''          If a bit in the <tt>ops</tt> set does not correspond to an
		'''          operation that is supported by this channel, that is, if
		'''          {@code set & ~validOps() != 0}
		''' </exception>
		''' <returns>  A key representing the registration of this channel with
		'''          the given selector </returns>
		Public MustOverride Function register(  sel As Selector,   ops As Integer,   att As Object) As SelectionKey
		'
		' sync(regLock) {
		'   sync(keySet) { look for selector }
		'   if (channel found) { set interest ops -- may block in selector;
		'                        return key; }
		'   create new key -- may block somewhere in selector;
		'   sync(keySet) { add key; }
		'   attach(attachment);
		'   return key;
		' }

		''' <summary>
		''' Registers this channel with the given selector, returning a selection
		''' key.
		''' 
		''' <p> An invocation of this convenience method of the form
		''' 
		''' <blockquote><tt>sc.register(sel, ops)</tt></blockquote>
		''' 
		''' behaves in exactly the same way as the invocation
		''' 
		''' <blockquote><tt>sc.{@link
		''' #register(java.nio.channels.Selector,int,java.lang.Object)
		''' register}(sel, ops, null)</tt></blockquote>
		''' </summary>
		''' <param name="sel">
		'''         The selector with which this channel is to be registered
		''' </param>
		''' <param name="ops">
		'''         The interest set for the resulting key
		''' </param>
		''' <exception cref="ClosedChannelException">
		'''          If this channel is closed
		''' </exception>
		''' <exception cref="ClosedSelectorException">
		'''          If the selector is closed
		''' </exception>
		''' <exception cref="IllegalBlockingModeException">
		'''          If this channel is in blocking mode
		''' </exception>
		''' <exception cref="IllegalSelectorException">
		'''          If this channel was not created by the same provider
		'''          as the given selector
		''' </exception>
		''' <exception cref="CancelledKeyException">
		'''          If this channel is currently registered with the given selector
		'''          but the corresponding key has already been cancelled
		''' </exception>
		''' <exception cref="IllegalArgumentException">
		'''          If a bit in <tt>ops</tt> does not correspond to an operation
		'''          that is supported by this channel, that is, if {@code set &
		'''          ~validOps() != 0}
		''' </exception>
		''' <returns>  A key representing the registration of this channel with
		'''          the given selector </returns>
		Public Function register(  sel As Selector,   ops As Integer) As SelectionKey
			Return register(sel, ops, Nothing)
		End Function

		''' <summary>
		''' Adjusts this channel's blocking mode.
		''' 
		''' <p> If this channel is registered with one or more selectors then an
		''' attempt to place it into blocking mode will cause an {@link
		''' IllegalBlockingModeException} to be thrown.
		''' 
		''' <p> This method may be invoked at any time.  The new blocking mode will
		''' only affect I/O operations that are initiated after this method returns.
		''' For some implementations this may require blocking until all pending I/O
		''' operations are complete.
		''' 
		''' <p> If this method is invoked while another invocation of this method or
		''' of the <seealso cref="#register(Selector, int) register"/> method is in progress
		''' then it will first block until the other operation is complete. </p>
		''' </summary>
		''' <param name="block">  If <tt>true</tt> then this channel will be placed in
		'''                blocking mode; if <tt>false</tt> then it will be placed
		'''                non-blocking mode
		''' </param>
		''' <returns>  This selectable channel
		''' </returns>
		''' <exception cref="ClosedChannelException">
		'''          If this channel is closed
		''' </exception>
		''' <exception cref="IllegalBlockingModeException">
		'''          If <tt>block</tt> is <tt>true</tt> and this channel is
		'''          registered with one or more selectors
		''' </exception>
		''' <exception cref="IOException">
		'''         If an I/O error occurs </exception>
		Public MustOverride Function configureBlocking(  block As Boolean) As SelectableChannel
		'
		' sync(regLock) {
		'   sync(keySet) { throw IBME if block && isRegistered; }
		'   change mode;
		' }

		''' <summary>
		''' Tells whether or not every I/O operation on this channel will block
		''' until it completes.  A newly-created channel is always in blocking mode.
		''' 
		''' <p> If this channel is closed then the value returned by this method is
		''' not specified. </p>
		''' </summary>
		''' <returns> <tt>true</tt> if, and only if, this channel is in blocking mode </returns>
		Public MustOverride ReadOnly Property blocking As Boolean

		''' <summary>
		''' Retrieves the object upon which the {@link #configureBlocking
		''' configureBlocking} and <seealso cref="#register register"/> methods synchronize.
		''' This is often useful in the implementation of adaptors that require a
		''' specific blocking mode to be maintained for a short period of time.
		''' </summary>
		''' <returns>  The blocking-mode lock object </returns>
		Public MustOverride Function blockingLock() As Object

	End Class

End Namespace