'
' * Copyright (c) 1999, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.naming.event

	''' <summary>
	''' Specifies the methods that a listener interested in namespace changes
	''' must implement.
	''' Specifically, the listener is interested in <tt>NamingEvent</tt>s
	''' with event types of <tt>OBJECT_ADDED</TT>, <TT>OBJECT_RENAMED</TT>, or
	''' <TT>OBJECT_REMOVED</TT>.
	''' <p>
	''' Such a listener must:
	''' <ol>
	''' <li>Implement this interface and its methods.
	''' <li>Implement <tt>NamingListener.namingExceptionThrown()</tt> so that
	''' it will be notified of exceptions thrown while attempting to
	''' collect information about the events.
	''' <li>Register with the source using the source's <tt>addNamingListener()</tt>
	'''    method.
	''' </ol>
	''' A listener that wants to be notified of <tt>OBJECT_CHANGED</tt> event types
	''' should also implement the <tt>ObjectChangeListener</tt>
	''' interface.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' </summary>
	''' <seealso cref= NamingEvent </seealso>
	''' <seealso cref= ObjectChangeListener </seealso>
	''' <seealso cref= EventContext </seealso>
	''' <seealso cref= EventDirContext
	''' @since 1.3 </seealso>
	Public Interface NamespaceChangeListener
		Inherits NamingListener

		''' <summary>
		''' Called when an object has been added.
		''' <p>
		''' The binding of the newly added object can be obtained using
		''' <tt>evt.getNewBinding()</tt>. </summary>
		''' <param name="evt"> The nonnull event. </param>
		''' <seealso cref= NamingEvent#OBJECT_ADDED </seealso>
		Sub objectAdded(ByVal evt As NamingEvent)

		''' <summary>
		''' Called when an object has been removed.
		''' <p>
		''' The binding of the newly removed object can be obtained using
		''' <tt>evt.getOldBinding()</tt>. </summary>
		''' <param name="evt"> The nonnull event. </param>
		''' <seealso cref= NamingEvent#OBJECT_REMOVED </seealso>
		Sub objectRemoved(ByVal evt As NamingEvent)

		''' <summary>
		''' Called when an object has been renamed.
		''' <p>
		''' The binding of the renamed object can be obtained using
		''' <tt>evt.getNewBinding()</tt>. Its old binding (before the rename)
		''' can be obtained using <tt>evt.getOldBinding()</tt>.
		''' One of these may be null if the old/new binding was outside the
		''' scope in which the listener has registered interest. </summary>
		''' <param name="evt"> The nonnull event. </param>
		''' <seealso cref= NamingEvent#OBJECT_RENAMED </seealso>
		Sub objectRenamed(ByVal evt As NamingEvent)
	End Interface

End Namespace