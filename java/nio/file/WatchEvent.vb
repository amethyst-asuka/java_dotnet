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

Namespace java.nio.file

	''' <summary>
	''' An event or a repeated event for an object that is registered with a {@link
	''' WatchService}.
	''' 
	''' <p> An event is classified by its <seealso cref="#kind() kind"/> and has a {@link
	''' #count() count} to indicate the number of times that the event has been
	''' observed. This allows for efficient representation of repeated events. The
	''' <seealso cref="#context() context"/> method returns any context associated with
	''' the event. In the case of a repeated event then the context is the same for
	''' all events.
	''' 
	''' <p> Watch events are immutable and safe for use by multiple concurrent
	''' threads.
	''' </summary>
	''' @param   <T>     The type of the context object associated with the event
	''' 
	''' @since 1.7 </param>

	Public Interface WatchEvent(Of T)

		''' <summary>
		''' An event kind, for the purposes of identification.
		''' 
		''' @since 1.7 </summary>
		''' <seealso cref= StandardWatchEventKinds </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interface inner types are not converted:
'		Public Shared interface Kind(Of T)
	'	{
	'		''' <summary>
	'		''' Returns the name of the event kind.
	'		''' </summary>
	'		''' <returns> the name of the event kind </returns>
	'		String name();
	'
	'		''' <summary>
	'		''' Returns the type of the <seealso cref="WatchEvent#context context"/> value.
	'		''' 
	'		''' </summary>
	'		''' <returns> the type of the context value </returns>
	'		Class type();
	'	}

		''' <summary>
		''' An event modifier that qualifies how a <seealso cref="Watchable"/> is registered
		''' with a <seealso cref="WatchService"/>.
		''' 
		''' <p> This release does not define any <em>standard</em> modifiers.
		''' 
		''' @since 1.7 </summary>
		''' <seealso cref= Watchable#register </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interface inner types are not converted:
'		Public Shared interface Modifier
	'	{
	'		''' <summary>
	'		''' Returns the name of the modifier.
	'		''' </summary>
	'		''' <returns> the name of the modifier </returns>
	'		String name();
	'	}

		''' <summary>
		''' Returns the event kind.
		''' </summary>
		''' <returns>  the event kind </returns>
		Function kind() As Kind(Of T)

		''' <summary>
		''' Returns the event count. If the event count is greater than {@code 1}
		''' then this is a repeated event.
		''' </summary>
		''' <returns>  the event count </returns>
		Function count() As Integer

		''' <summary>
		''' Returns the context for the event.
		''' 
		''' <p> In the case of <seealso cref="StandardWatchEventKinds#ENTRY_CREATE ENTRY_CREATE"/>,
		''' <seealso cref="StandardWatchEventKinds#ENTRY_DELETE ENTRY_DELETE"/>, and {@link
		''' StandardWatchEventKinds#ENTRY_MODIFY ENTRY_MODIFY} events the context is
		''' a {@code Path} that is the <seealso cref="Path#relativize relative"/> path between
		''' the directory registered with the watch service, and the entry that is
		''' created, deleted, or modified.
		''' </summary>
		''' <returns>  the event context; may be {@code null} </returns>
		Function context() As T
	End Interface

End Namespace