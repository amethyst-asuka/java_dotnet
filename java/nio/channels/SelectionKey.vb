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
	''' A token representing the registration of a <seealso cref="SelectableChannel"/> with a
	''' <seealso cref="Selector"/>.
	''' 
	''' <p> A selection key is created each time a channel is registered with a
	''' selector.  A key remains valid until it is <i>cancelled</i> by invoking its
	''' <seealso cref="#cancel cancel"/> method, by closing its channel, or by closing its
	''' selector.  Cancelling a key does not immediately remove it from its
	''' selector; it is instead added to the selector's <a
	''' href="Selector.html#ks"><i>cancelled-key set</i></a> for removal during the
	''' next selection operation.  The validity of a key may be tested by invoking
	''' its <seealso cref="#isValid isValid"/> method.
	''' 
	''' <a name="opsets"></a>
	''' 
	''' <p> A selection key contains two <i>operation sets</i> represented as
	''' integer values.  Each bit of an operation set denotes a category of
	''' selectable operations that are supported by the key's channel.
	''' 
	''' <ul>
	''' 
	'''   <li><p> The <i>interest set</i> determines which operation categories will
	'''   be tested for readiness the next time one of the selector's selection
	'''   methods is invoked.  The interest set is initialized with the value given
	'''   when the key is created; it may later be changed via the {@link
	'''   #interestOps(int)} method. </p></li>
	''' 
	'''   <li><p> The <i>ready set</i> identifies the operation categories for which
	'''   the key's channel has been detected to be ready by the key's selector.
	'''   The ready set is initialized to zero when the key is created; it may later
	'''   be updated by the selector during a selection operation, but it cannot be
	'''   updated directly. </p></li>
	''' 
	''' </ul>
	''' 
	''' <p> That a selection key's ready set indicates that its channel is ready for
	''' some operation category is a hint, but not a guarantee, that an operation in
	''' such a category may be performed by a thread without causing the thread to
	''' block.  A ready set is most likely to be accurate immediately after the
	''' completion of a selection operation.  It is likely to be made inaccurate by
	''' external events and by I/O operations that are invoked upon the
	''' corresponding channel.
	''' 
	''' <p> This class defines all known operation-set bits, but precisely which
	''' bits are supported by a given channel depends upon the type of the channel.
	''' Each subclass of <seealso cref="SelectableChannel"/> defines an {@link
	''' SelectableChannel#validOps() validOps()} method which returns a set
	''' identifying just those operations that are supported by the channel.  An
	''' attempt to set or test an operation-set bit that is not supported by a key's
	''' channel will result in an appropriate run-time exception.
	''' 
	''' <p> It is often necessary to associate some application-specific data with a
	''' selection key, for example an object that represents the state of a
	''' higher-level protocol and handles readiness notifications in order to
	''' implement that protocol.  Selection keys therefore support the
	''' <i>attachment</i> of a single arbitrary object to a key.  An object can be
	''' attached via the <seealso cref="#attach attach"/> method and then later retrieved via
	''' the <seealso cref="#attachment() attachment"/> method.
	''' 
	''' <p> Selection keys are safe for use by multiple concurrent threads.  The
	''' operations of reading and writing the interest set will, in general, be
	''' synchronized with certain operations of the selector.  Exactly how this
	''' synchronization is performed is implementation-dependent: In a naive
	''' implementation, reading or writing the interest set may block indefinitely
	''' if a selection operation is already in progress; in a high-performance
	''' implementation, reading or writing the interest set may block briefly, if at
	''' all.  In any case, a selection operation will always use the interest-set
	''' value that was current at the moment that the operation began.  </p>
	''' 
	''' 
	''' @author Mark Reinhold
	''' @author JSR-51 Expert Group
	''' @since 1.4
	''' </summary>
	''' <seealso cref= SelectableChannel </seealso>
	''' <seealso cref= Selector </seealso>

	Public MustInherit Class SelectionKey

		''' <summary>
		''' Constructs an instance of this class.
		''' </summary>
		Protected Friend Sub New()
		End Sub


		' -- Channel and selector operations --

		''' <summary>
		''' Returns the channel for which this key was created.  This method will
		''' continue to return the channel even after the key is cancelled.
		''' </summary>
		''' <returns>  This key's channel </returns>
		Public MustOverride Function channel() As SelectableChannel

		''' <summary>
		''' Returns the selector for which this key was created.  This method will
		''' continue to return the selector even after the key is cancelled.
		''' </summary>
		''' <returns>  This key's selector </returns>
		Public MustOverride Function selector() As Selector

		''' <summary>
		''' Tells whether or not this key is valid.
		''' 
		''' <p> A key is valid upon creation and remains so until it is cancelled,
		''' its channel is closed, or its selector is closed.  </p>
		''' </summary>
		''' <returns>  <tt>true</tt> if, and only if, this key is valid </returns>
		Public MustOverride ReadOnly Property valid As Boolean

		''' <summary>
		''' Requests that the registration of this key's channel with its selector
		''' be cancelled.  Upon return the key will be invalid and will have been
		''' added to its selector's cancelled-key set.  The key will be removed from
		''' all of the selector's key sets during the next selection operation.
		''' 
		''' <p> If this key has already been cancelled then invoking this method has
		''' no effect.  Once cancelled, a key remains forever invalid. </p>
		''' 
		''' <p> This method may be invoked at any time.  It synchronizes on the
		''' selector's cancelled-key set, and therefore may block briefly if invoked
		''' concurrently with a cancellation or selection operation involving the
		''' same selector.  </p>
		''' </summary>
		Public MustOverride Sub cancel()


		' -- Operation-set accessors --

		''' <summary>
		''' Retrieves this key's interest set.
		''' 
		''' <p> It is guaranteed that the returned set will only contain operation
		''' bits that are valid for this key's channel.
		''' 
		''' <p> This method may be invoked at any time.  Whether or not it blocks,
		''' and for how long, is implementation-dependent.  </p>
		''' </summary>
		''' <returns>  This key's interest set
		''' </returns>
		''' <exception cref="CancelledKeyException">
		'''          If this key has been cancelled </exception>
		Public MustOverride Function interestOps() As Integer

		''' <summary>
		''' Sets this key's interest set to the given value.
		''' 
		''' <p> This method may be invoked at any time.  Whether or not it blocks,
		''' and for how long, is implementation-dependent.  </p>
		''' </summary>
		''' <param name="ops">  The new interest set
		''' </param>
		''' <returns>  This selection key
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''          If a bit in the set does not correspond to an operation that
		'''          is supported by this key's channel, that is, if
		'''          {@code (ops & ~channel().validOps()) != 0}
		''' </exception>
		''' <exception cref="CancelledKeyException">
		'''          If this key has been cancelled </exception>
		Public MustOverride Function interestOps(  ops As Integer) As SelectionKey

		''' <summary>
		''' Retrieves this key's ready-operation set.
		''' 
		''' <p> It is guaranteed that the returned set will only contain operation
		''' bits that are valid for this key's channel.  </p>
		''' </summary>
		''' <returns>  This key's ready-operation set
		''' </returns>
		''' <exception cref="CancelledKeyException">
		'''          If this key has been cancelled </exception>
		Public MustOverride Function readyOps() As Integer


		' -- Operation bits and bit-testing convenience methods --

		''' <summary>
		''' Operation-set bit for read operations.
		''' 
		''' <p> Suppose that a selection key's interest set contains
		''' <tt>OP_READ</tt> at the start of a <a
		''' href="Selector.html#selop">selection operation</a>.  If the selector
		''' detects that the corresponding channel is ready for reading, has reached
		''' end-of-stream, has been remotely shut down for further reading, or has
		''' an error pending, then it will add <tt>OP_READ</tt> to the key's
		''' ready-operation set and add the key to its selected-key&nbsp;set.  </p>
		''' </summary>
		Public Shared ReadOnly OP_READ As Integer = 1 << 0

		''' <summary>
		''' Operation-set bit for write operations.
		''' 
		''' <p> Suppose that a selection key's interest set contains
		''' <tt>OP_WRITE</tt> at the start of a <a
		''' href="Selector.html#selop">selection operation</a>.  If the selector
		''' detects that the corresponding channel is ready for writing, has been
		''' remotely shut down for further writing, or has an error pending, then it
		''' will add <tt>OP_WRITE</tt> to the key's ready set and add the key to its
		''' selected-key&nbsp;set.  </p>
		''' </summary>
		Public Shared ReadOnly OP_WRITE As Integer = 1 << 2

		''' <summary>
		''' Operation-set bit for socket-connect operations.
		''' 
		''' <p> Suppose that a selection key's interest set contains
		''' <tt>OP_CONNECT</tt> at the start of a <a
		''' href="Selector.html#selop">selection operation</a>.  If the selector
		''' detects that the corresponding socket channel is ready to complete its
		''' connection sequence, or has an error pending, then it will add
		''' <tt>OP_CONNECT</tt> to the key's ready set and add the key to its
		''' selected-key&nbsp;set.  </p>
		''' </summary>
		Public Shared ReadOnly OP_CONNECT As Integer = 1 << 3

		''' <summary>
		''' Operation-set bit for socket-accept operations.
		''' 
		''' <p> Suppose that a selection key's interest set contains
		''' <tt>OP_ACCEPT</tt> at the start of a <a
		''' href="Selector.html#selop">selection operation</a>.  If the selector
		''' detects that the corresponding server-socket channel is ready to accept
		''' another connection, or has an error pending, then it will add
		''' <tt>OP_ACCEPT</tt> to the key's ready set and add the key to its
		''' selected-key&nbsp;set.  </p>
		''' </summary>
		Public Shared ReadOnly OP_ACCEPT As Integer = 1 << 4

		''' <summary>
		''' Tests whether this key's channel is ready for reading.
		''' 
		''' <p> An invocation of this method of the form <tt>k.isReadable()</tt>
		''' behaves in exactly the same way as the expression
		''' 
		''' <blockquote><pre>{@code
		''' k.readyOps() & OP_READ != 0
		''' }</pre></blockquote>
		''' 
		''' <p> If this key's channel does not support read operations then this
		''' method always returns <tt>false</tt>.  </p>
		''' </summary>
		''' <returns>  <tt>true</tt> if, and only if,
		'''            {@code readyOps() & OP_READ} is nonzero
		''' </returns>
		''' <exception cref="CancelledKeyException">
		'''          If this key has been cancelled </exception>
		Public Property readable As Boolean
			Get
				Return (readyOps() And OP_READ) <> 0
			End Get
		End Property

		''' <summary>
		''' Tests whether this key's channel is ready for writing.
		''' 
		''' <p> An invocation of this method of the form <tt>k.isWritable()</tt>
		''' behaves in exactly the same way as the expression
		''' 
		''' <blockquote><pre>{@code
		''' k.readyOps() & OP_WRITE != 0
		''' }</pre></blockquote>
		''' 
		''' <p> If this key's channel does not support write operations then this
		''' method always returns <tt>false</tt>.  </p>
		''' </summary>
		''' <returns>  <tt>true</tt> if, and only if,
		'''          {@code readyOps() & OP_WRITE} is nonzero
		''' </returns>
		''' <exception cref="CancelledKeyException">
		'''          If this key has been cancelled </exception>
		Public Property writable As Boolean
			Get
				Return (readyOps() And OP_WRITE) <> 0
			End Get
		End Property

		''' <summary>
		''' Tests whether this key's channel has either finished, or failed to
		''' finish, its socket-connection operation.
		''' 
		''' <p> An invocation of this method of the form <tt>k.isConnectable()</tt>
		''' behaves in exactly the same way as the expression
		''' 
		''' <blockquote><pre>{@code
		''' k.readyOps() & OP_CONNECT != 0
		''' }</pre></blockquote>
		''' 
		''' <p> If this key's channel does not support socket-connect operations
		''' then this method always returns <tt>false</tt>.  </p>
		''' </summary>
		''' <returns>  <tt>true</tt> if, and only if,
		'''          {@code readyOps() & OP_CONNECT} is nonzero
		''' </returns>
		''' <exception cref="CancelledKeyException">
		'''          If this key has been cancelled </exception>
		Public Property connectable As Boolean
			Get
				Return (readyOps() And OP_CONNECT) <> 0
			End Get
		End Property

		''' <summary>
		''' Tests whether this key's channel is ready to accept a new socket
		''' connection.
		''' 
		''' <p> An invocation of this method of the form <tt>k.isAcceptable()</tt>
		''' behaves in exactly the same way as the expression
		''' 
		''' <blockquote><pre>{@code
		''' k.readyOps() & OP_ACCEPT != 0
		''' }</pre></blockquote>
		''' 
		''' <p> If this key's channel does not support socket-accept operations then
		''' this method always returns <tt>false</tt>.  </p>
		''' </summary>
		''' <returns>  <tt>true</tt> if, and only if,
		'''          {@code readyOps() & OP_ACCEPT} is nonzero
		''' </returns>
		''' <exception cref="CancelledKeyException">
		'''          If this key has been cancelled </exception>
		Public Property acceptable As Boolean
			Get
				Return (readyOps() And OP_ACCEPT) <> 0
			End Get
		End Property


		' -- Attachments --

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private attachment_Renamed As Object = Nothing

		Private Shared ReadOnly attachmentUpdater As java.util.concurrent.atomic.AtomicReferenceFieldUpdater(Of SelectionKey, Object) = java.util.concurrent.atomic.AtomicReferenceFieldUpdater.newUpdater(GetType(SelectionKey), GetType(Object), "attachment")

		''' <summary>
		''' Attaches the given object to this key.
		''' 
		''' <p> An attached object may later be retrieved via the {@link #attachment()
		''' attachment} method.  Only one object may be attached at a time; invoking
		''' this method causes any previous attachment to be discarded.  The current
		''' attachment may be discarded by attaching <tt>null</tt>.  </p>
		''' </summary>
		''' <param name="ob">
		'''         The object to be attached; may be <tt>null</tt>
		''' </param>
		''' <returns>  The previously-attached object, if any,
		'''          otherwise <tt>null</tt> </returns>
		Public Function attach(  ob As Object) As Object
			Return attachmentUpdater.getAndSet(Me, ob)
		End Function

		''' <summary>
		''' Retrieves the current attachment.
		''' </summary>
		''' <returns>  The object currently attached to this key,
		'''          or <tt>null</tt> if there is no attachment </returns>
		Public Function attachment() As Object
			Return attachment_Renamed
		End Function

	End Class

End Namespace