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
	''' A nexus for I/O operations.
	''' 
	''' <p> A channel represents an open connection to an entity such as a hardware
	''' device, a file, a network socket, or a program component that is capable of
	''' performing one or more distinct I/O operations, for example reading or
	''' writing.
	''' 
	''' <p> A channel is either open or closed.  A channel is open upon creation,
	''' and once closed it remains closed.  Once a channel is closed, any attempt to
	''' invoke an I/O operation upon it will cause a <seealso cref="ClosedChannelException"/>
	''' to be thrown.  Whether or not a channel is open may be tested by invoking
	''' its <seealso cref="#isOpen isOpen"/> method.
	''' 
	''' <p> Channels are, in general, intended to be safe for multithreaded access
	''' as described in the specifications of the interfaces and classes that extend
	''' and implement this interface.
	''' 
	''' 
	''' @author Mark Reinhold
	''' @author JSR-51 Expert Group
	''' @since 1.4
	''' </summary>

	Public Interface Channel
		Inherits java.io.Closeable

		''' <summary>
		''' Tells whether or not this channel is open.
		''' </summary>
		''' <returns> <tt>true</tt> if, and only if, this channel is open </returns>
		ReadOnly Property open As Boolean

		''' <summary>
		''' Closes this channel.
		''' 
		''' <p> After a channel is closed, any further attempt to invoke I/O
		''' operations upon it will cause a <seealso cref="ClosedChannelException"/> to be
		''' thrown.
		''' 
		''' <p> If this channel is already closed then invoking this method has no
		''' effect.
		''' 
		''' <p> This method may be invoked at any time.  If some other thread has
		''' already invoked it, however, then another invocation will block until
		''' the first invocation is complete, after which it will return without
		''' effect. </p>
		''' </summary>
		''' <exception cref="IOException">  If an I/O error occurs </exception>
		Sub close()

	End Interface

End Namespace