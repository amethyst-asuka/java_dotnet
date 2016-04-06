Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections
Imports System.Threading
Imports sun.awt

'
' * Copyright (c) 1996, 2014, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt










	''' <summary>
	''' <code>EventQueue</code> is a platform-independent class
	''' that queues events, both from the underlying peer classes
	''' and from trusted application classes.
	''' <p>
	''' It encapsulates asynchronous event dispatch machinery which
	''' extracts events from the queue and dispatches them by calling
	''' <seealso cref="#dispatchEvent(AWTEvent) dispatchEvent(AWTEvent)"/> method
	''' on this <code>EventQueue</code> with the event to be dispatched
	''' as an argument.  The particular behavior of this machinery is
	''' implementation-dependent.  The only requirements are that events
	''' which were actually enqueued to this queue (note that events
	''' being posted to the <code>EventQueue</code> can be coalesced)
	''' are dispatched:
	''' <dl>
	'''   <dt> Sequentially.
	'''   <dd> That is, it is not permitted that several events from
	'''        this queue are dispatched simultaneously.
	'''   <dt> In the same order as they are enqueued.
	'''   <dd> That is, if <code>AWTEvent</code>&nbsp;A is enqueued
	'''        to the <code>EventQueue</code> before
	'''        <code>AWTEvent</code>&nbsp;B then event B will not be
	'''        dispatched before event A.
	''' </dl>
	''' <p>
	''' Some browsers partition applets in different code bases into
	''' separate contexts, and establish walls between these contexts.
	''' In such a scenario, there will be one <code>EventQueue</code>
	''' per context. Other browsers place all applets into the same
	''' context, implying that there will be only a single, global
	''' <code>EventQueue</code> for all applets. This behavior is
	''' implementation-dependent.  Consult your browser's documentation
	''' for more information.
	''' <p>
	''' For information on the threading issues of the event dispatch
	''' machinery, see <a href="doc-files/AWTThreadIssues.html#Autoshutdown"
	''' >AWT Threading Issues</a>.
	''' 
	''' @author Thomas Ball
	''' @author Fred Ecks
	''' @author David Mendenhall
	''' 
	''' @since       1.1
	''' </summary>
	Public Class EventQueue
		Private Shared ReadOnly threadInitNumber As New java.util.concurrent.atomic.AtomicInteger(0)

		Private Const LOW_PRIORITY As Integer = 0
		Private Const NORM_PRIORITY As Integer = 1
		Private Const HIGH_PRIORITY As Integer = 2
		Private Const ULTIMATE_PRIORITY As Integer = 3

		Private Shared ReadOnly NUM_PRIORITIES As Integer = ULTIMATE_PRIORITY + 1

	'    
	'     * We maintain one Queue for each priority that the EventQueue supports.
	'     * That is, the EventQueue object is actually implemented as
	'     * NUM_PRIORITIES queues and all Events on a particular internal Queue
	'     * have identical priority. Events are pulled off the EventQueue starting
	'     * with the Queue of highest priority. We progress in decreasing order
	'     * across all Queues.
	'     
		Private queues As Queue() = New Queue(NUM_PRIORITIES - 1){}

	'    
	'     * The next EventQueue on the stack, or null if this EventQueue is
	'     * on the top of the stack.  If nextQueue is non-null, requests to post
	'     * an event are forwarded to nextQueue.
	'     
		Private nextQueue As EventQueue

	'    
	'     * The previous EventQueue on the stack, or null if this is the
	'     * "base" EventQueue.
	'     
		Private previousQueue As EventQueue

	'    
	'     * A single lock to synchronize the push()/pop() and related operations with
	'     * all the EventQueues from the AppContext. Synchronization on any particular
	'     * event queue(s) is not enough: we should lock the whole stack.
	'     
		Private ReadOnly pushPopLock As java.util.concurrent.locks.Lock
		Private ReadOnly pushPopCond As java.util.concurrent.locks.Condition

	'    
	'     * Dummy runnable to wake up EDT from getNextEvent() after
	'     push/pop is performed
	'     
		Private Shared ReadOnly dummyRunnable As Runnable = New RunnableAnonymousInnerClassHelper

		Private Class RunnableAnonymousInnerClassHelper
			Implements Runnable

			Public Overridable Sub run() Implements Runnable.run
			End Sub
		End Class

		Private dispatchThread As EventDispatchThread

		Private ReadOnly threadGroup As ThreadGroup = Thread.currentThread().threadGroup
		Private ReadOnly classLoader As  ClassLoader = Thread.currentThread().contextClassLoader

	'    
	'     * The time stamp of the last dispatched InputEvent or ActionEvent.
	'     
		Private mostRecentEventTime As Long = System.currentTimeMillis()

	'    
	'     * The time stamp of the last KeyEvent .
	'     
		Private mostRecentKeyEventTime As Long = System.currentTimeMillis()

		''' <summary>
		''' The modifiers field of the current event, if the current event is an
		''' InputEvent or ActionEvent.
		''' </summary>
		Private currentEvent As WeakReference(Of AWTEvent)

        '    
        '     * Non-zero if a thread is waiting in getNextEvent(int) for an event of
        '     * a particular ID to be posted to the queue.
        '     
        Private waitForID As Integer

        '    
        '     * AppContext corresponding to the queue.
        '     
        Private ReadOnly appContext As AppContext

		Private ReadOnly name As String = "AWT-EventQueue-" & threadInitNumber.andIncrement

		Private fwDispatcher As FwDispatcher

        Private Shared eventLog As sun.util.logging.PlatformLogger

        Private Shared ReadOnly Property eventLog As sun.util.logging.PlatformLogger
            Get
                If eventLog Is Nothing Then eventLog = sun.util.logging.PlatformLogger.getLogger("java.awt.event.EventQueue")
                Return eventLog
            End Get
        End Property

        Shared Sub New()
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			AWTAccessor.setEventQueueAccessor(New AWTAccessor.EventQueueAccessor()
	'		{
	'				public Thread getDispatchThread(EventQueue eventQueue)
	'				{
	'					Return eventQueue.getDispatchThread();
	'				}
	'				public boolean isDispatchThreadImpl(EventQueue eventQueue)
	'				{
	'					Return eventQueue.isDispatchThreadImpl();
	'				}
	'				public  Sub  removeSourceEvents(EventQueue eventQueue, Object source, boolean removeAllEvents)
	'				{
	'					eventQueue.removeSourceEvents(source, removeAllEvents);
	'				}
	'				public boolean noEvents(EventQueue eventQueue)
	'				{
	'					Return eventQueue.noEvents();
	'				}
	'				public  Sub  wakeup(EventQueue eventQueue, boolean isShutdown)
	'				{
	'					eventQueue.wakeup(isShutdown);
	'				}
	'				public  Sub  invokeAndWait(Object source, Runnable r) throws InterruptedException, InvocationTargetException
	'				{
	'					EventQueue.invokeAndWait(source, r);
	'				}
	'				public  Sub  setFwDispatcher(EventQueue eventQueue, FwDispatcher dispatcher)
	'				{
	'					eventQueue.setFwDispatcher(dispatcher);
	'				}
	'
	'				@Override public long getMostRecentEventTime(EventQueue eventQueue)
	'				{
	'					Return eventQueue.getMostRecentEventTimeImpl();
	'				}
	'			});
		End Sub

		Public Sub New()
			For i As Integer = 0 To NUM_PRIORITIES - 1
				queues(i) = New Queue
			Next i
	'        
	'         * NOTE: if you ever have to start the associated event dispatch
	'         * thread at this point, be aware of the following problem:
	'         * If this EventQueue instance is created in
	'         * SunToolkit.createNewAppContext() the started dispatch thread
	'         * may call AppContext.getAppContext() before createNewAppContext()
	'         * completes thus causing mess in thread group to appcontext mapping.
	'         

			appContext = AppContext.appContext
			pushPopLock = CType(appContext.get(AppContext.EVENT_QUEUE_LOCK_KEY), java.util.concurrent.locks.Lock)
			pushPopCond = CType(appContext.get(AppContext.EVENT_QUEUE_COND_KEY), java.util.concurrent.locks.Condition)
		End Sub

		''' <summary>
		''' Posts a 1.1-style event to the <code>EventQueue</code>.
		''' If there is an existing event on the queue with the same ID
		''' and event source, the source <code>Component</code>'s
		''' <code>coalesceEvents</code> method will be called.
		''' </summary>
		''' <param name="theEvent"> an instance of <code>java.awt.AWTEvent</code>,
		'''          or a subclass of it </param>
		''' <exception cref="NullPointerException"> if <code>theEvent</code> is <code>null</code> </exception>
		Public Overridable Sub postEvent(  theEvent As AWTEvent)
			SunToolkit.flushPendingEvents(appContext)
			postEventPrivate(theEvent)
		End Sub

		''' <summary>
		''' Posts a 1.1-style event to the <code>EventQueue</code>.
		''' If there is an existing event on the queue with the same ID
		''' and event source, the source <code>Component</code>'s
		''' <code>coalesceEvents</code> method will be called.
		''' </summary>
		''' <param name="theEvent"> an instance of <code>java.awt.AWTEvent</code>,
		'''          or a subclass of it </param>
		Private Sub postEventPrivate(  theEvent As AWTEvent)
			theEvent.isPosted = True
			pushPopLock.lock()
			Try
				If nextQueue IsNot Nothing Then
					' Forward the event to the top of EventQueue stack
					nextQueue.postEventPrivate(theEvent)
					Return
				End If
				If dispatchThread Is Nothing Then
					If theEvent.source Is AWTAutoShutdown.instance Then
						Return
					Else
						initDispatchThread()
					End If
				End If
				postEvent(theEvent, getPriority(theEvent))
			Finally
				pushPopLock.unlock()
			End Try
		End Sub

		Private Shared Function getPriority(  theEvent As AWTEvent) As Integer
			If TypeOf theEvent Is PeerEvent Then
				Dim peerEvent As PeerEvent = CType(theEvent, PeerEvent)
				If (peerEvent.flags And PeerEvent.ULTIMATE_PRIORITY_EVENT) <> 0 Then Return ULTIMATE_PRIORITY
				If (peerEvent.flags And PeerEvent.PRIORITY_EVENT) <> 0 Then Return HIGH_PRIORITY
				If (peerEvent.flags And PeerEvent.LOW_PRIORITY_EVENT) <> 0 Then Return LOW_PRIORITY
			End If
			Dim id As Integer = theEvent.iD
			If (id >= PaintEvent.PAINT_FIRST) AndAlso (id <= PaintEvent.PAINT_LAST) Then Return LOW_PRIORITY
			Return NORM_PRIORITY
		End Function

		''' <summary>
		''' Posts the event to the internal Queue of specified priority,
		''' coalescing as appropriate.
		''' </summary>
		''' <param name="theEvent"> an instance of <code>java.awt.AWTEvent</code>,
		'''          or a subclass of it </param>
		''' <param name="priority">  the desired priority of the event </param>
		Private Sub postEvent(  theEvent As AWTEvent,   priority As Integer)
			If coalesceEvent(theEvent, priority) Then Return

			Dim newItem As New EventQueueItem(theEvent)

			cacheEQItem(newItem)

			Dim notifyID As Boolean = (theEvent.iD = Me.waitForID)

			If queues(priority).head Is Nothing Then
				Dim shouldNotify As Boolean = noEvents()
					queues(priority).tail = newItem
					queues(priority).head = queues(priority).tail

				If shouldNotify Then
					If theEvent.source IsNot AWTAutoShutdown.instance Then AWTAutoShutdown.instance.notifyThreadBusy(dispatchThread)
					pushPopCond.signalAll()
				ElseIf notifyID Then
					pushPopCond.signalAll()
				End If
			Else
				' The event was not coalesced or has non-Component source.
				' Insert it at the end of the appropriate Queue.
				queues(priority).tail.next = newItem
				queues(priority).tail = newItem
				If notifyID Then pushPopCond.signalAll()
			End If
		End Sub

		Private Function coalescePaintEvent(  e As PaintEvent) As Boolean
			Dim sourcePeer As java.awt.peer.ComponentPeer = CType(e.source, Component).peer
			If sourcePeer IsNot Nothing Then sourcePeer.coalescePaintEvent(e)
			Dim cache As EventQueueItem() = CType(e.source, Component).eventCache
			If cache Is Nothing Then Return False
			Dim index As Integer = eventToCacheIndex(e)

			If index <> -1 AndAlso cache(index) IsNot Nothing Then
				Dim merged As PaintEvent = mergePaintEvents(e, CType(cache(index).event, PaintEvent))
				If merged IsNot Nothing Then
					cache(index).event = merged
					Return True
				End If
			End If
			Return False
		End Function

		Private Function mergePaintEvents(  a As PaintEvent,   b As PaintEvent) As PaintEvent
			Dim aRect As Rectangle = a.updateRect
			Dim bRect As Rectangle = b.updateRect
			If bRect.contains(aRect) Then Return b
			If aRect.contains(bRect) Then Return a
			Return Nothing
		End Function

		Private Function coalesceMouseEvent(  e As MouseEvent) As Boolean
			Dim cache As EventQueueItem() = CType(e.source, Component).eventCache
			If cache Is Nothing Then Return False
			Dim index As Integer = eventToCacheIndex(e)
			If index <> -1 AndAlso cache(index) IsNot Nothing Then
				cache(index).event = e
				Return True
			End If
			Return False
		End Function

		Private Function coalescePeerEvent(  e As PeerEvent) As Boolean
			Dim cache As EventQueueItem() = CType(e.source, Component).eventCache
			If cache Is Nothing Then Return False
			Dim index As Integer = eventToCacheIndex(e)
			If index <> -1 AndAlso cache(index) IsNot Nothing Then
				e = e.coalesceEvents(CType(cache(index).event, PeerEvent))
				If e IsNot Nothing Then
					cache(index).event = e
					Return True
				Else
					cache(index) = Nothing
				End If
			End If
			Return False
		End Function

	'    
	'     * Should avoid of calling this method by any means
	'     * as it's working time is dependant on EQ length.
	'     * In the wors case this method alone can slow down the entire application
	'     * 10 times by stalling the Event processing.
	'     * Only here by backward compatibility reasons.
	'     
		Private Function coalesceOtherEvent(  e As AWTEvent,   priority As Integer) As Boolean
			Dim id As Integer = e.iD
			Dim source As Component = CType(e.source, Component)
			Dim entry As EventQueueItem = queues(priority).head
			Do While entry IsNot Nothing
				' Give Component.coalesceEvents a chance
				If entry.event.source Is source AndAlso entry.event.iD = id Then
					Dim coalescedEvent As AWTEvent = source.coalesceEvents(entry.event, e)
					If coalescedEvent IsNot Nothing Then
						entry.event = coalescedEvent
						Return True
					End If
				End If
				entry = entry.next
			Loop
			Return False
		End Function

		Private Function coalesceEvent(  e As AWTEvent,   priority As Integer) As Boolean
			If Not(TypeOf e.source Is Component) Then Return False
			If TypeOf e Is PeerEvent Then Return coalescePeerEvent(CType(e, PeerEvent))
			' The worst case
			If CType(e.source, Component).coalescingEnabled AndAlso coalesceOtherEvent(e, priority) Then Return True
			If TypeOf e Is PaintEvent Then Return coalescePaintEvent(CType(e, PaintEvent))
			If TypeOf e Is MouseEvent Then Return coalesceMouseEvent(CType(e, MouseEvent))
			Return False
		End Function

		Private Sub cacheEQItem(  entry As EventQueueItem)
			Dim index As Integer = eventToCacheIndex(entry.event)
			If index <> -1 AndAlso TypeOf entry.event.source Is Component Then
				Dim source As Component = CType(entry.event.source, Component)
				If source.eventCache Is Nothing Then source.eventCache = New EventQueueItem(CACHE_LENGTH - 1){}
				source.eventCache(index) = entry
			End If
		End Sub

		Private Sub uncacheEQItem(  entry As EventQueueItem)
			Dim index As Integer = eventToCacheIndex(entry.event)
			If index <> -1 AndAlso TypeOf entry.event.source Is Component Then
				Dim source As Component = CType(entry.event.source, Component)
				If source.eventCache Is Nothing Then Return
				source.eventCache(index) = Nothing
			End If
		End Sub

		Private Const PAINT As Integer = 0
		Private Const UPDATE As Integer = 1
		Private Const MOVE As Integer = 2
		Private Const DRAG As Integer = 3
		Private Const PEER As Integer = 4
		Private Const CACHE_LENGTH As Integer = 5

		Private Shared Function eventToCacheIndex(  e As AWTEvent) As Integer
			Select Case e.iD
			Case PaintEvent.PAINT
				Return PAINT
			Case PaintEvent.UPDATE
				Return UPDATE
			Case MouseEvent.MOUSE_MOVED
				Return MOVE
			Case MouseEvent.MOUSE_DRAGGED
				' Return -1 for SunDropTargetEvent since they are usually synchronous
				' and we don't want to skip them by coalescing with MouseEvent or other drag events
				Return If(TypeOf e Is sun.awt.dnd.SunDropTargetEvent, -1, DRAG)
			Case Else
				Return If(TypeOf e Is PeerEvent, PEER, -1)
			End Select
		End Function

		''' <summary>
		''' Returns whether an event is pending on any of the separate
		''' Queues. </summary>
		''' <returns> whether an event is pending on any of the separate Queues </returns>
		Private Function noEvents() As Boolean
			For i As Integer = 0 To NUM_PRIORITIES - 1
				If queues(i).head IsNot Nothing Then Return False
			Next i

			Return True
		End Function

		''' <summary>
		''' Removes an event from the <code>EventQueue</code> and
		''' returns it.  This method will block until an event has
		''' been posted by another thread. </summary>
		''' <returns> the next <code>AWTEvent</code> </returns>
		''' <exception cref="InterruptedException">
		'''            if any thread has interrupted this thread </exception>
		Public Overridable Property nextEvent As AWTEvent
			Get
				Do
		'            
		'             * SunToolkit.flushPendingEvents must be called outside
		'             * of the synchronized block to avoid deadlock when
		'             * event queues are nested with push()/pop().
		'             
					SunToolkit.flushPendingEvents(appContext)
					pushPopLock.lock()
					Try
						Dim event_Renamed As AWTEvent = nextEventPrivate
						If event_Renamed IsNot Nothing Then Return event_Renamed
						AWTAutoShutdown.instance.notifyThreadFree(dispatchThread)
						pushPopCond.await()
					Finally
						pushPopLock.unlock()
					End Try
				Loop While True
			End Get
		End Property

	'    
	'     * Must be called under the lock. Doesn't call flushPendingEvents()
	'     
		Friend Overridable Property nextEventPrivate As AWTEvent
			Get
				For i As Integer = NUM_PRIORITIES - 1 To 0 Step -1
					If queues(i).head IsNot Nothing Then
						Dim entry As EventQueueItem = queues(i).head
						queues(i).head = entry.next
						If entry.next Is Nothing Then queues(i).tail = Nothing
						uncacheEQItem(entry)
						Return entry.event
					End If
				Next i
				Return Nothing
			End Get
		End Property

		Friend Overridable Function getNextEvent(  id As Integer) As AWTEvent
			Do
	'            
	'             * SunToolkit.flushPendingEvents must be called outside
	'             * of the synchronized block to avoid deadlock when
	'             * event queues are nested with push()/pop().
	'             
				SunToolkit.flushPendingEvents(appContext)
				pushPopLock.lock()
				Try
					For i As Integer = 0 To NUM_PRIORITIES - 1
						Dim entry As EventQueueItem = queues(i).head
						Dim prev As EventQueueItem = Nothing
						Do While entry IsNot Nothing
							If entry.event.iD = id Then
								If prev Is Nothing Then
									queues(i).head = entry.next
								Else
									prev.next = entry.next
								End If
								If queues(i).tail Is entry Then queues(i).tail = prev
								uncacheEQItem(entry)
								Return entry.event
							End If
							prev = entry
							entry = entry.next
						Loop
					Next i
					waitForID = id
					pushPopCond.await()
					waitForID = 0
				Finally
					pushPopLock.unlock()
				End Try
			Loop While True
		End Function

		''' <summary>
		''' Returns the first event on the <code>EventQueue</code>
		''' without removing it. </summary>
		''' <returns> the first event </returns>
		Public Overridable Function peekEvent() As AWTEvent
			pushPopLock.lock()
			Try
				For i As Integer = NUM_PRIORITIES - 1 To 0 Step -1
					If queues(i).head IsNot Nothing Then Return queues(i).head.event
				Next i
			Finally
				pushPopLock.unlock()
			End Try

			Return Nothing
		End Function

		''' <summary>
		''' Returns the first event with the specified id, if any. </summary>
		''' <param name="id"> the id of the type of event desired </param>
		''' <returns> the first event of the specified id or <code>null</code>
		'''    if there is no such event </returns>
		Public Overridable Function peekEvent(  id As Integer) As AWTEvent
			pushPopLock.lock()
			Try
				For i As Integer = NUM_PRIORITIES - 1 To 0 Step -1
					Dim q As EventQueueItem = queues(i).head
					Do While q IsNot Nothing
						If q.event.iD = id Then Return q.event
						q = q.next
					Loop
				Next i
			Finally
				pushPopLock.unlock()
			End Try

			Return Nothing
		End Function

		Private Shared ReadOnly javaSecurityAccess As sun.misc.JavaSecurityAccess = sun.misc.SharedSecrets.javaSecurityAccess

		''' <summary>
		''' Dispatches an event. The manner in which the event is
		''' dispatched depends upon the type of the event and the
		''' type of the event's source object:
		''' 
		''' <table border=1 summary="Event types, source types, and dispatch methods">
		''' <tr>
		'''     <th>Event Type</th>
		'''     <th>Source Type</th>
		'''     <th>Dispatched To</th>
		''' </tr>
		''' <tr>
		'''     <td>ActiveEvent</td>
		'''     <td>Any</td>
		'''     <td>event.dispatch()</td>
		''' </tr>
		''' <tr>
		'''     <td>Other</td>
		'''     <td>Component</td>
		'''     <td>source.dispatchEvent(AWTEvent)</td>
		''' </tr>
		''' <tr>
		'''     <td>Other</td>
		'''     <td>MenuComponent</td>
		'''     <td>source.dispatchEvent(AWTEvent)</td>
		''' </tr>
		''' <tr>
		'''     <td>Other</td>
		'''     <td>Other</td>
		'''     <td>No action (ignored)</td>
		''' </tr>
		''' </table>
		''' <p> </summary>
		''' <param name="event"> an instance of <code>java.awt.AWTEvent</code>,
		'''          or a subclass of it </param>
		''' <exception cref="NullPointerException"> if <code>event</code> is <code>null</code>
		''' @since           1.2 </exception>
		Protected Friend Overridable Sub dispatchEvent(  [event] As AWTEvent)
			Dim src As Object = event_Renamed.source
			Dim action As java.security.PrivilegedAction(Of Void) = New PrivilegedActionAnonymousInnerClassHelper(Of T)

			Dim stack As java.security.AccessControlContext = java.security.AccessController.context
			Dim srcAcc As java.security.AccessControlContext = getAccessControlContextFrom(src)
			Dim eventAcc As java.security.AccessControlContext = event_Renamed.accessControlContext
			If srcAcc Is Nothing Then
				javaSecurityAccess.doIntersectionPrivilege(action, stack, eventAcc)
			Else
                javaSecurityAccess.doIntersectionPrivilege(New PrivilegedActionAnonymousInnerClassHelper2(Of T))
            End If
		End Sub

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As Void
				' In case fwDispatcher is installed and we're already on the
				' dispatch thread (e.g. performing DefaultKeyboardFocusManager.sendMessage),
				' dispatch the event straight away.
				If outerInstance.fwDispatcher Is Nothing OrElse outerInstance.dispatchThreadImpl Then
                    outerInstance.dispatchEventImpl([event], src)
                Else
                    outerInstance.fwDispatcher.scheduleDispatch(New RunnableAnonymousInnerClassHelper2)
                End If
				Return Nothing
			End Function

			Private Class RunnableAnonymousInnerClassHelper2
				Implements Runnable

				Public Overrides Sub run() Implements Runnable.run
					dispatchEventImpl(event, src)
				End Sub
			End Class
		End Class

		Private Class PrivilegedActionAnonymousInnerClassHelper2(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As Void
				javaSecurityAccess.doIntersectionPrivilege(action, eventAcc)
				Return Nothing
			End Function
		End Class

		Private Shared Function getAccessControlContextFrom(  src As Object) As java.security.AccessControlContext
			Return If(TypeOf src Is Component, CType(src, Component).accessControlContext, If(TypeOf src Is MenuComponent, CType(src, MenuComponent).accessControlContext, If(TypeOf src Is TrayIcon, CType(src, TrayIcon).accessControlContext, Nothing)))
		End Function

		''' <summary>
		''' Called from dispatchEvent() under a correct AccessControlContext
		''' </summary>
		Private Sub dispatchEventImpl(  [event] As AWTEvent,   src As Object)
			event_Renamed.isPosted = True
			If TypeOf event_Renamed Is ActiveEvent Then
				' This could become the sole method of dispatching in time.
				currentEventAndMostRecentTimeImpl = event_Renamed
				CType(event_Renamed, ActiveEvent).dispatch()
			ElseIf TypeOf src Is Component Then
				CType(src, Component).dispatchEvent(event_Renamed)
				event_Renamed.dispatched()
			ElseIf TypeOf src Is MenuComponent Then
				CType(src, MenuComponent).dispatchEvent(event_Renamed)
			ElseIf TypeOf src Is TrayIcon Then
				CType(src, TrayIcon).dispatchEvent(event_Renamed)
			ElseIf TypeOf src Is AWTAutoShutdown Then
				If noEvents() Then dispatchThread.stopDispatching()
			Else
				If eventLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then eventLog.fine("Unable to dispatch event: " & event_Renamed)
			End If
		End Sub

		''' <summary>
		''' Returns the timestamp of the most recent event that had a timestamp, and
		''' that was dispatched from the <code>EventQueue</code> associated with the
		''' calling thread. If an event with a timestamp is currently being
		''' dispatched, its timestamp will be returned. If no events have yet
		''' been dispatched, the EventQueue's initialization time will be
		''' returned instead.In the current version of
		''' the JDK, only <code>InputEvent</code>s,
		''' <code>ActionEvent</code>s, and <code>InvocationEvent</code>s have
		''' timestamps; however, future versions of the JDK may add timestamps to
		''' additional event types. Note that this method should only be invoked
		''' from an application's <seealso cref="#isDispatchThread event dispatching thread"/>.
		''' If this method is
		''' invoked from another thread, the current system time (as reported by
		''' <code>System.currentTimeMillis()</code>) will be returned instead.
		''' </summary>
		''' <returns> the timestamp of the last <code>InputEvent</code>,
		'''         <code>ActionEvent</code>, or <code>InvocationEvent</code> to be
		'''         dispatched, or <code>System.currentTimeMillis()</code> if this
		'''         method is invoked on a thread other than an event dispatching
		'''         thread </returns>
		''' <seealso cref= java.awt.event.InputEvent#getWhen </seealso>
		''' <seealso cref= java.awt.event.ActionEvent#getWhen </seealso>
		''' <seealso cref= java.awt.event.InvocationEvent#getWhen </seealso>
		''' <seealso cref= #isDispatchThread
		''' 
		''' @since 1.4 </seealso>
		PublicShared ReadOnly PropertymostRecentEventTime As Long
        Get
        Return Toolkit.eventQueue.mostRecentEventTimeImpl
        End Get
        End Property
        Private Property mostRecentEventTimeImpl As Long
            Get
                pushPopLock.lock()
                Try
                    Return If(Thread.CurrentThread Is dispatchThread, mostRecentEventTime, System.currentTimeMillis())
                Finally
                    pushPopLock.unlock()
                End Try
            End Get
        End Property

        ''' <returns> most recent event time on all threads. </returns>
        Friend Overridable Property mostRecentEventTimeEx As Long
            Get
                pushPopLock.lock()
                Try
                    Return mostRecentEventTime
                Finally
                    pushPopLock.unlock()
                End Try
            End Get
        End Property

		''' <summary>
		''' Returns the the event currently being dispatched by the
		''' <code>EventQueue</code> associated with the calling thread. This is
		''' useful if a method needs access to the event, but was not designed to
		''' receive a reference to it as an argument. Note that this method should
		''' only be invoked from an application's event dispatching thread. If this
		''' method is invoked from another thread, null will be returned.
		''' </summary>
		''' <returns> the event currently being dispatched, or null if this method is
		'''         invoked on a thread other than an event dispatching thread
		''' @since 1.4 </returns>
		PublicShared ReadOnly PropertycurrentEvent As AWTEvent
        Get
        Return Toolkit.eventQueue.currentEventImpl
        End Get
        End Property
        Private Property currentEventImpl As AWTEvent
            Get
                pushPopLock.lock()
                Try
                    Return If(Thread.CurrentThread Is dispatchThread, currentEvent.get(), Nothing)
                Finally
                    pushPopLock.unlock()
                End Try
            End Get
        End Property

        ''' <summary>
        ''' Replaces the existing <code>EventQueue</code> with the specified one.
        ''' Any pending events are transferred to the new <code>EventQueue</code>
        ''' for processing by it.
        ''' </summary>
        ''' <param name="newEventQueue"> an <code>EventQueue</code>
        '''          (or subclass thereof) instance to be use </param>
        ''' <seealso cref=      java.awt.EventQueue#pop </seealso>
        ''' <exception cref="NullPointerException"> if <code>newEventQueue</code> is <code>null</code>
        ''' @since           1.2 </exception>
        Public Overridable Sub push(  newEventQueue As EventQueue)
            If eventLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then eventLog.fine("EventQueue.push(" & newEventQueue & ")")

            pushPopLock.lock()
            Try
                Dim topQueue As EventQueue = Me
                Do While topQueue.nextQueue IsNot Nothing
                    topQueue = topQueue.nextQueue
                Loop
                If topQueue.fwDispatcher IsNot Nothing Then Throw New RuntimeException("push() to queue with fwDispatcher")
                If (topQueue.dispatchThread IsNot Nothing) AndAlso (topQueue.dispatchThread.eventQueue Is Me) Then
                    newEventQueue.dispatchThread = topQueue.dispatchThread
                    topQueue.dispatchThread.eventQueue = newEventQueue
                End If

                ' Transfer all events forward to new EventQueue.
                Do While topQueue.peekEvent() IsNot Nothing
                    Try
                        ' Use getNextEventPrivate() as it doesn't call flushPendingEvents()
                        newEventQueue.postEventPrivate(topQueue.nextEventPrivate)
                    Catch ie As InterruptedException
                        If eventLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then eventLog.fine("Interrupted push", ie)
                    End Try
                Loop

                ' Wake up EDT waiting in getNextEvent(), so it can
                ' pick up a new EventQueue. Post the waking event before
                ' topQueue.nextQueue is assigned, otherwise the event would
                ' go newEventQueue
                topQueue.postEventPrivate(New InvocationEvent(topQueue, dummyRunnable))

                newEventQueue.previousQueue = topQueue
                topQueue.nextQueue = newEventQueue

                If appContext.get(appContext.EVENT_QUEUE_KEY) Is topQueue Then appContext.put(appContext.EVENT_QUEUE_KEY, newEventQueue)

                pushPopCond.signalAll()
            Finally
                pushPopLock.unlock()
            End Try
        End Sub

        ''' <summary>
        ''' Stops dispatching events using this <code>EventQueue</code>.
        ''' Any pending events are transferred to the previous
        ''' <code>EventQueue</code> for processing.
        ''' <p>
        ''' Warning: To avoid deadlock, do not declare this method
        ''' synchronized in a subclass.
        ''' </summary>
        ''' <exception cref="EmptyStackException"> if no previous push was made
        '''  on this <code>EventQueue</code> </exception>
        ''' <seealso cref=      java.awt.EventQueue#push
        ''' @since           1.2 </seealso>
        Protected Friend Overridable Sub pop()
            If eventLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then eventLog.fine("EventQueue.pop(" & Me & ")")

            pushPopLock.lock()
            Try
                Dim topQueue As EventQueue = Me
                Do While topQueue.nextQueue IsNot Nothing
                    topQueue = topQueue.nextQueue
                Loop
                Dim prevQueue As EventQueue = topQueue.previousQueue
                If prevQueue Is Nothing Then Throw New java.util.EmptyStackException

                topQueue.previousQueue = Nothing
                prevQueue.nextQueue = Nothing

                ' Transfer all events back to previous EventQueue.
                Do While topQueue.peekEvent() IsNot Nothing
                    Try
                        prevQueue.postEventPrivate(topQueue.nextEventPrivate)
                    Catch ie As InterruptedException
                        If eventLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then eventLog.fine("Interrupted pop", ie)
                    End Try
                Loop

                If (topQueue.dispatchThread IsNot Nothing) AndAlso (topQueue.dispatchThread.eventQueue Is Me) Then
                    prevQueue.dispatchThread = topQueue.dispatchThread
                    topQueue.dispatchThread.eventQueue = prevQueue
                End If

                If appContext.get(appContext.EVENT_QUEUE_KEY) Is Me Then appContext.put(appContext.EVENT_QUEUE_KEY, prevQueue)

                ' Wake up EDT waiting in getNextEvent(), so it can
                ' pick up a new EventQueue
                topQueue.postEventPrivate(New InvocationEvent(topQueue, dummyRunnable))

                pushPopCond.signalAll()
            Finally
                pushPopLock.unlock()
            End Try
        End Sub

        ''' <summary>
        ''' Creates a new {@code secondary loop} associated with this
        ''' event queue. Use the <seealso cref="SecondaryLoop#enter"/> and
        ''' <seealso cref="SecondaryLoop#exit"/> methods to start and stop the
        ''' event loop and dispatch the events from this queue.
        ''' </summary>
        ''' <returns> secondaryLoop A new secondary loop object, which can
        '''                       be used to launch a new nested event
        '''                       loop and dispatch events from this queue
        ''' </returns>
        ''' <seealso cref= SecondaryLoop#enter </seealso>
        ''' <seealso cref= SecondaryLoop#exit
        ''' 
        ''' @since 1.7 </seealso>
        Public Overridable Function createSecondaryLoop() As SecondaryLoop
            Return createSecondaryLoop(Nothing, Nothing, 0)
        End Function

        Friend Overridable Function createSecondaryLoop(  cond As Conditional,   filter As EventFilter,   interval As Long) As SecondaryLoop
            pushPopLock.lock()
            Try
                If nextQueue IsNot Nothing Then Return nextQueue.createSecondaryLoop(cond, filter, interval)
                If fwDispatcher IsNot Nothing Then Return fwDispatcher.createSecondaryLoop()
                If dispatchThread Is Nothing Then initDispatchThread()
                Return New WaitDispatchSupport(dispatchThread, cond, filter, interval)
            Finally
                pushPopLock.unlock()
            End Try
        End Function

		''' <summary>
		''' Returns true if the calling thread is
		''' <seealso cref="Toolkit#getSystemEventQueue the current AWT EventQueue"/>'s
		''' dispatch thread. Use this method to ensure that a particular
		''' task is being executed (or not being) there.
		''' <p>
		''' Note: use the <seealso cref="#invokeLater"/> or <seealso cref="#invokeAndWait"/>
		''' methods to execute a task in
		''' <seealso cref="Toolkit#getSystemEventQueue the current AWT EventQueue"/>'s
		''' dispatch thread.
		''' <p>
		''' </summary>
		''' <returns> true if running in
		''' <seealso cref="Toolkit#getSystemEventQueue the current AWT EventQueue"/>'s
		''' dispatch thread </returns>
		''' <seealso cref=             #invokeLater </seealso>
		''' <seealso cref=             #invokeAndWait </seealso>
		''' <seealso cref=             Toolkit#getSystemEventQueue
		''' @since           1.2 </seealso>
		PublicShared ReadOnly PropertydispatchThread As Boolean
        Get
				Dim eq As EventQueue = Toolkit.eventQueue
				Return eq.dispatchThreadImpl
			End Get
		End Property

		Friend Property dispatchThreadImpl As Boolean
			Get
				Dim eq As EventQueue = Me
				pushPopLock.lock()
				Try
					Dim [next] As EventQueue = eq.nextQueue
					Do While [next] IsNot Nothing
						eq = [next]
						[next] = eq.nextQueue
					Loop
					If eq.fwDispatcher IsNot Nothing Then Return eq.fwDispatcher.dispatchThread
					Return (Thread.CurrentThread Is eq.dispatchThread)
				Finally
					pushPopLock.unlock()
				End Try
			End Get
		End Property

		Friend Sub initDispatchThread()
			pushPopLock.lock()
			Try
				If dispatchThread Is Nothing AndAlso (Not threadGroup.destroyed) AndAlso (Not appContext.disposed) Then
					dispatchThread = java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper3(Of T)
				   )
					dispatchThread.start()
				End If
			Finally
				pushPopLock.unlock()
			End Try
		End Sub

		Private Class PrivilegedActionAnonymousInnerClassHelper3(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As EventDispatchThread
				Dim t As New EventDispatchThread(outerInstance.threadGroup, outerInstance.name, EventQueue.this)
				t.contextClassLoader = outerInstance.classLoader
				t.priority = Thread.NORM_PRIORITY + 1
				t.daemon = False
				AWTAutoShutdown.instance.notifyThreadBusy(t)
				Return t
			End Function
		End Class

		Friend Sub detachDispatchThread(  edt As EventDispatchThread)
	'        
	'         * Minimize discard possibility for non-posted events
	'         
			SunToolkit.flushPendingEvents(appContext)
	'        
	'         * This synchronized block is to secure that the event dispatch
	'         * thread won't die in the middle of posting a new event to the
	'         * associated event queue. It is important because we notify
	'         * that the event dispatch thread is busy after posting a new event
	'         * to its queue, so the EventQueue.dispatchThread reference must
	'         * be valid at that point.
	'         
			pushPopLock.lock()
			Try
				If edt Is dispatchThread Then dispatchThread = Nothing
				AWTAutoShutdown.instance.notifyThreadFree(edt)
	'            
	'             * Event was posted after EDT events pumping had stopped, so start
	'             * another EDT to handle this event
	'             
				If peekEvent() IsNot Nothing Then initDispatchThread()
			Finally
				pushPopLock.unlock()
			End Try
		End Sub

	'    
	'     * Gets the <code>EventDispatchThread</code> for this
	'     * <code>EventQueue</code>.
	'     * @return the event dispatch thread associated with this event queue
	'     *         or <code>null</code> if this event queue doesn't have a
	'     *         working thread associated with it
	'     * @see    java.awt.EventQueue#initDispatchThread
	'     * @see    java.awt.EventQueue#detachDispatchThread
	'     
		Friend Property dispatchThread As EventDispatchThread
			Get
				pushPopLock.lock()
				Try
					Return dispatchThread
				Finally
					pushPopLock.unlock()
				End Try
			End Get
		End Property

	'    
	'     * Removes any pending events for the specified source object.
	'     * If removeAllEvents parameter is <code>true</code> then all
	'     * events for the specified source object are removed, if it
	'     * is <code>false</code> then <code>SequencedEvent</code>, <code>SentEvent</code>,
	'     * <code>FocusEvent</code>, <code>WindowEvent</code>, <code>KeyEvent</code>,
	'     * and <code>InputMethodEvent</code> are kept in the queue, but all other
	'     * events are removed.
	'     *
	'     * This method is normally called by the source's
	'     * <code>removeNotify</code> method.
	'     
		Friend Sub removeSourceEvents(  source As Object,   removeAllEvents As Boolean)
			SunToolkit.flushPendingEvents(appContext)
			pushPopLock.lock()
			Try
				For i As Integer = 0 To NUM_PRIORITIES - 1
					Dim entry As EventQueueItem = queues(i).head
					Dim prev As EventQueueItem = Nothing
					Do While entry IsNot Nothing
						If (entry.event.source Is source) AndAlso (removeAllEvents OrElse Not(TypeOf entry.event Is SequencedEvent OrElse TypeOf entry.event Is SentEvent OrElse TypeOf entry.event Is FocusEvent OrElse TypeOf entry.event Is WindowEvent OrElse TypeOf entry.event Is KeyEvent OrElse TypeOf entry.event Is InputMethodEvent)) Then
							If TypeOf entry.event Is SequencedEvent Then CType(entry.event, SequencedEvent).Dispose()
							If TypeOf entry.event Is SentEvent Then CType(entry.event, SentEvent).Dispose()
							If TypeOf entry.event Is InvocationEvent Then AWTAccessor.invocationEventAccessor.Dispose(CType(entry.event, InvocationEvent))
							If prev Is Nothing Then
								queues(i).head = entry.next
							Else
								prev.next = entry.next
							End If
							uncacheEQItem(entry)
						Else
							prev = entry
						End If
						entry = entry.next
					Loop
					queues(i).tail = prev
				Next i
			Finally
				pushPopLock.unlock()
			End Try
		End Sub

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Overridable Property mostRecentKeyEventTime As Long
			Get
				pushPopLock.lock()
				Try
					Return mostRecentKeyEventTime
				Finally
					pushPopLock.unlock()
				End Try
			End Get
		End Property

		Friend Shared Property currentEventAndMostRecentTime As AWTEvent
			Set(  e As AWTEvent)
				Toolkit.eventQueue.currentEventAndMostRecentTimeImpl = e
			End Set
		End Property
		Private Property currentEventAndMostRecentTimeImpl As AWTEvent
			Set(  e As AWTEvent)
				pushPopLock.lock()
				Try
					If Thread.CurrentThread IsNot dispatchThread Then Return
    
					currentEvent = New WeakReference(Of )(e)
    
					' This series of 'instanceof' checks should be replaced with a
					' polymorphic type (for example, an interface which declares a
					' getWhen() method). However, this would require us to make such
					' a type public, or to place it in sun.awt. Both of these approaches
					' have been frowned upon. So for now, we hack.
					'
					' In tiger, we will probably give timestamps to all events, so this
					' will no longer be an issue.
					Dim mostRecentEventTime2 As Long = java.lang.[Long].MIN_VALUE
					If TypeOf e Is InputEvent Then
						Dim ie As InputEvent = CType(e, InputEvent)
						mostRecentEventTime2 = ie.when
						If TypeOf e Is KeyEvent Then mostRecentKeyEventTime = ie.when
					ElseIf TypeOf e Is InputMethodEvent Then
						Dim ime As InputMethodEvent = CType(e, InputMethodEvent)
						mostRecentEventTime2 = ime.when
					ElseIf TypeOf e Is ActionEvent Then
						Dim ae As ActionEvent = CType(e, ActionEvent)
						mostRecentEventTime2 = ae.when
					ElseIf TypeOf e Is InvocationEvent Then
						Dim ie As InvocationEvent = CType(e, InvocationEvent)
						mostRecentEventTime2 = ie.when
					End If
					mostRecentEventTime = System.Math.Max(mostRecentEventTime, mostRecentEventTime2)
				Finally
					pushPopLock.unlock()
				End Try
			End Set
		End Property

		''' <summary>
		''' Causes <code>runnable</code> to have its <code>run</code>
		''' method called in the <seealso cref="#isDispatchThread dispatch thread"/> of
		''' <seealso cref="Toolkit#getSystemEventQueue the system EventQueue"/>.
		''' This will happen after all pending events are processed.
		''' </summary>
		''' <param name="runnable">  the <code>Runnable</code> whose <code>run</code>
		'''                  method should be executed
		'''                  asynchronously in the
		'''                  <seealso cref="#isDispatchThread event dispatch thread"/>
		'''                  of <seealso cref="Toolkit#getSystemEventQueue the system EventQueue"/> </param>
		''' <seealso cref=             #invokeAndWait </seealso>
		''' <seealso cref=             Toolkit#getSystemEventQueue </seealso>
		''' <seealso cref=             #isDispatchThread
		''' @since           1.2 </seealso>
		Public Shared Sub invokeLater(  runnable As Runnable)
			Toolkit.eventQueue.postEvent(New InvocationEvent(Toolkit.defaultToolkit, runnable))
		End Sub

		''' <summary>
		''' Causes <code>runnable</code> to have its <code>run</code>
		''' method called in the <seealso cref="#isDispatchThread dispatch thread"/> of
		''' <seealso cref="Toolkit#getSystemEventQueue the system EventQueue"/>.
		''' This will happen after all pending events are processed.
		''' The call blocks until this has happened.  This method
		''' will throw an Error if called from the
		''' <seealso cref="#isDispatchThread event dispatcher thread"/>.
		''' </summary>
		''' <param name="runnable">  the <code>Runnable</code> whose <code>run</code>
		'''                  method should be executed
		'''                  synchronously in the
		'''                  <seealso cref="#isDispatchThread event dispatch thread"/>
		'''                  of <seealso cref="Toolkit#getSystemEventQueue the system EventQueue"/> </param>
		''' <exception cref="InterruptedException">  if any thread has
		'''                  interrupted this thread </exception>
		''' <exception cref="InvocationTargetException">  if an throwable is thrown
		'''                  when running <code>runnable</code> </exception>
		''' <seealso cref=             #invokeLater </seealso>
		''' <seealso cref=             Toolkit#getSystemEventQueue </seealso>
		''' <seealso cref=             #isDispatchThread
		''' @since           1.2 </seealso>
		Public Shared Sub invokeAndWait(  runnable As Runnable)
			invokeAndWait(Toolkit.defaultToolkit, runnable)
		End Sub

		Friend Shared Sub invokeAndWait(  source As Object,   runnable As Runnable)
			If EventQueue.dispatchThread Then Throw New [Error]("Cannot call invokeAndWait from the event dispatcher thread")

'JAVA TO VB CONVERTER TODO TASK: Local classes are not converted by Java to VB Converter:
'			class AWTInvocationLock
	'		{
	'		}
			Dim lock As Object = New AWTInvocationLock

			Dim event_Renamed As New InvocationEvent(source, runnable, lock, True)

			SyncLock lock
				Toolkit.eventQueue.postEvent(event_Renamed)
				Do While Not event_Renamed.dispatched
					lock.wait()
				Loop
			End SyncLock

			Dim eventThrowable As Throwable = event_Renamed.throwable
			If eventThrowable IsNot Nothing Then Throw New InvocationTargetException(eventThrowable)
		End Sub

	'    
	'     * Called from PostEventQueue.postEvent to notify that a new event
	'     * appeared. First it proceeds to the EventQueue on the top of the
	'     * stack, then notifies the associated dispatch thread if it exists
	'     * or starts a new one otherwise.
	'     
		Private Sub wakeup(  isShutdown As Boolean)
			pushPopLock.lock()
			Try
				If nextQueue IsNot Nothing Then
					' Forward call to the top of EventQueue stack.
					nextQueue.wakeup(isShutdown)
				ElseIf dispatchThread IsNot Nothing Then
					pushPopCond.signalAll()
				ElseIf Not isShutdown Then
					initDispatchThread()
				End If
			Finally
				pushPopLock.unlock()
			End Try
		End Sub

		' The method is used by AWTAccessor for javafx/AWT single threaded mode.
		Private Property fwDispatcher As FwDispatcher
			Set(  dispatcher As FwDispatcher)
				If nextQueue IsNot Nothing Then
					nextQueue.fwDispatcher = dispatcher
				Else
					fwDispatcher = dispatcher
				End If
			End Set
		End Property
	End Class

	''' <summary>
	''' The Queue object holds pointers to the beginning and end of one internal
	''' queue. An EventQueue object is composed of multiple internal Queues, one
	''' for each priority supported by the EventQueue. All Events on a particular
	''' internal Queue have identical priority.
	''' </summary>
	Friend Class Queue
		Friend head As EventQueueItem
		Friend tail As EventQueueItem
	End Class

End Namespace