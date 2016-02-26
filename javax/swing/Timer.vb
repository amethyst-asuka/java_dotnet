Imports System

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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



Namespace javax.swing






	''' <summary>
	''' Fires one or more {@code ActionEvent}s at specified
	''' intervals. An example use is an animation object that uses a
	''' <code>Timer</code> as the trigger for drawing its frames.
	''' <p>
	''' Setting up a timer
	''' involves creating a <code>Timer</code> object,
	''' registering one or more action listeners on it,
	''' and starting the timer using
	''' the <code>start</code> method.
	''' For example,
	''' the following code creates and starts a timer
	''' that fires an action event once per second
	''' (as specified by the first argument to the <code>Timer</code> constructor).
	''' The second argument to the <code>Timer</code> constructor
	''' specifies a listener to receive the timer's action events.
	''' 
	''' <pre>
	'''  int delay = 1000; //milliseconds
	'''  ActionListener taskPerformer = new ActionListener() {
	'''      public void actionPerformed(ActionEvent evt) {
	'''          <em>//...Perform a task...</em>
	'''      }
	'''  };
	'''  new Timer(delay, taskPerformer).start();</pre>
	''' 
	''' <p>
	''' {@code Timers} are constructed by specifying both a delay parameter
	''' and an {@code ActionListener}. The delay parameter is used
	''' to set both the initial delay and the delay between event
	''' firing, in milliseconds. Once the timer has been started,
	''' it waits for the initial delay before firing its
	''' first <code>ActionEvent</code> to registered listeners.
	''' After this first event, it continues to fire events
	''' every time the between-event delay has elapsed, until it
	''' is stopped.
	''' <p>
	''' After construction, the initial delay and the between-event
	''' delay can be changed independently, and additional
	''' <code>ActionListeners</code> may be added.
	''' <p>
	''' If you want the timer to fire only the first time and then stop,
	''' invoke <code>setRepeats(false)</code> on the timer.
	''' <p>
	''' Although all <code>Timer</code>s perform their waiting
	''' using a single, shared thread
	''' (created by the first <code>Timer</code> object that executes),
	''' the action event handlers for <code>Timer</code>s
	''' execute on another thread -- the event-dispatching thread.
	''' This means that the action handlers for <code>Timer</code>s
	''' can safely perform operations on Swing components.
	''' However, it also means that the handlers must execute quickly
	''' to keep the GUI responsive.
	''' 
	''' <p>
	''' In v 1.3, another <code>Timer</code> class was added
	''' to the Java platform: <code>java.util.Timer</code>.
	''' Both it and <code>javax.swing.Timer</code>
	''' provide the same basic functionality,
	''' but <code>java.util.Timer</code>
	''' is more general and has more features.
	''' The <code>javax.swing.Timer</code> has two features
	''' that can make it a little easier to use with GUIs.
	''' First, its event handling metaphor is familiar to GUI programmers
	''' and can make dealing with the event-dispatching thread
	''' a bit simpler.
	''' Second, its
	''' automatic thread sharing means that you don't have to
	''' take special steps to avoid spawning
	''' too many threads.
	''' Instead, your timer uses the same thread
	''' used to make cursors blink,
	''' tool tips appear,
	''' and so on.
	''' 
	''' <p>
	''' You can find further documentation
	''' and several examples of using timers by visiting
	''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/misc/timer.html"
	''' target = "_top">How to Use Timers</a>,
	''' a section in <em>The Java Tutorial.</em>
	''' For more examples and help in choosing between
	''' this <code>Timer</code> class and
	''' <code>java.util.Timer</code>,
	''' see
	''' <a href="http://java.sun.com/products/jfc/tsc/articles/timer/"
	''' target="_top">Using Timers in Swing Applications</a>,
	''' an article in <em>The Swing Connection.</em>
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' </summary>
	''' <seealso cref= java.util.Timer <code>java.util.Timer</code>
	''' 
	''' 
	''' @author Dave Moore </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<Serializable> _
	Public Class Timer
	'    
	'     * NOTE: all fields need to be handled in readResolve
	'     

		Protected Friend listenerList As New javax.swing.event.EventListenerList

		' The following field strives to maintain the following:
		'    If coalesce is true, only allow one Runnable to be queued on the
		'    EventQueue and be pending (ie in the process of notifying the
		'    ActionListener). If we didn't do this it would allow for a
		'    situation where the app is taking too long to process the
		'    actionPerformed, and thus we'ld end up queing a bunch of Runnables
		'    and the app would never return: not good. This of course implies
		'    you can get dropped events, but such is life.
		' notify is used to indicate if the ActionListener can be notified, when
		' the Runnable is processed if this is true it will notify the listeners.
		' notify is set to true when the Timer fires and the Runnable is queued.
		' It will be set to false after notifying the listeners (if coalesce is
		' true) or if the developer invokes stop.
		<NonSerialized> _
		Private ReadOnly notify As New java.util.concurrent.atomic.AtomicBoolean(False)

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private initialDelay, delay As Integer
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private repeats As Boolean = True, coalesce As Boolean = True

		<NonSerialized> _
		Private ReadOnly doPostEvent As Runnable

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private Shared logTimers As Boolean

		<NonSerialized> _
		Private ReadOnly lock As Lock = New ReentrantLock

		' This field is maintained by TimerQueue.
		' eventQueued can also be reset by the TimerQueue, but will only ever
		' happen in applet case when TimerQueues thread is destroyed.
		' access to this field is synchronized on getLock() lock.
		<NonSerialized> _
		Friend delayedTimer As TimerQueue.DelayedTimer = Nothing

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private actionCommand As String

		''' <summary>
		''' Creates a {@code Timer} and initializes both the initial delay and
		''' between-event delay to {@code delay} milliseconds. If {@code delay}
		''' is less than or equal to zero, the timer fires as soon as it
		''' is started. If <code>listener</code> is not <code>null</code>,
		''' it's registered as an action listener on the timer.
		''' </summary>
		''' <param name="delay"> milliseconds for the initial and between-event delay </param>
		''' <param name="listener">  an initial listener; can be <code>null</code>
		''' </param>
		''' <seealso cref= #addActionListener </seealso>
		''' <seealso cref= #setInitialDelay </seealso>
		''' <seealso cref= #setRepeats </seealso>
		Public Sub New(ByVal delay As Integer, ByVal listener As ActionListener)
			MyBase.New()
			Me.delay = delay
			Me.initialDelay = delay

			doPostEvent = New DoPostEvent(Me)

			If listener IsNot Nothing Then addActionListener(listener)
		End Sub

	'    
	'     * The timer's AccessControlContext.
	'     
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		 <NonSerialized> _
		 Private acc As java.security.AccessControlContext = java.security.AccessController.context

		''' <summary>
		''' Returns the acc this timer was constructed with.
		''' </summary>
		 Friend Property accessControlContext As java.security.AccessControlContext
			 Get
			   If acc Is Nothing Then Throw New SecurityException("Timer is missing AccessControlContext")
			   Return acc
			 End Get
		 End Property

		''' <summary>
		''' DoPostEvent is a runnable class that fires actionEvents to
		''' the listeners on the EventDispatchThread, via invokeLater. </summary>
		''' <seealso cref= Timer#post </seealso>
		Friend Class DoPostEvent
			Implements Runnable

			Private ReadOnly outerInstance As Timer

			Public Sub New(ByVal outerInstance As Timer)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub run()
				If logTimers Then Console.WriteLine("Timer ringing: " & Timer.this)
				If outerInstance.notify.get() Then
					outerInstance.fireActionPerformed(New ActionEvent(Timer.this, 0, outerInstance.actionCommand, System.currentTimeMillis(), 0))
					If outerInstance.coalesce Then outerInstance.cancelEvent()
				End If
			End Sub

			Friend Overridable Property timer As Timer
				Get
					Return Timer.this
				End Get
			End Property
		End Class

		''' <summary>
		''' Adds an action listener to the <code>Timer</code>.
		''' </summary>
		''' <param name="listener"> the listener to add
		''' </param>
		''' <seealso cref= #Timer </seealso>
		Public Overridable Sub addActionListener(ByVal listener As ActionListener)
			listenerList.add(GetType(ActionListener), listener)
		End Sub


		''' <summary>
		''' Removes the specified action listener from the <code>Timer</code>.
		''' </summary>
		''' <param name="listener"> the listener to remove </param>
		Public Overridable Sub removeActionListener(ByVal listener As ActionListener)
			listenerList.remove(GetType(ActionListener), listener)
		End Sub


		''' <summary>
		''' Returns an array of all the action listeners registered
		''' on this timer.
		''' </summary>
		''' <returns> all of the timer's <code>ActionListener</code>s or an empty
		'''         array if no action listeners are currently registered
		''' </returns>
		''' <seealso cref= #addActionListener </seealso>
		''' <seealso cref= #removeActionListener
		''' 
		''' @since 1.4 </seealso>
		Public Overridable Property actionListeners As ActionListener()
			Get
				Return listenerList.getListeners(GetType(ActionListener))
			End Get
		End Property


		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.
		''' </summary>
		''' <param name="e"> the action event to fire </param>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireActionPerformed(ByVal e As ActionEvent)
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList

			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(ActionListener) Then CType(___listeners(i+1), ActionListener).actionPerformed(e)
			Next i
		End Sub

		''' <summary>
		''' Returns an array of all the objects currently registered as
		''' <code><em>Foo</em>Listener</code>s
		''' upon this <code>Timer</code>.
		''' <code><em>Foo</em>Listener</code>s
		''' are registered using the <code>add<em>Foo</em>Listener</code> method.
		''' <p>
		''' You can specify the <code>listenerType</code> argument
		''' with a class literal, such as <code><em>Foo</em>Listener.class</code>.
		''' For example, you can query a <code>Timer</code>
		''' instance <code>t</code>
		''' for its action listeners
		''' with the following code:
		''' 
		''' <pre>ActionListener[] als = (ActionListener[])(t.getListeners(ActionListener.class));</pre>
		''' 
		''' If no such listeners exist,
		''' this method returns an empty array.
		''' </summary>
		''' <param name="listenerType">  the type of listeners requested;
		'''          this parameter should specify an interface
		'''          that descends from <code>java.util.EventListener</code> </param>
		''' <returns> an array of all objects registered as
		'''          <code><em>Foo</em>Listener</code>s
		'''          on this timer,
		'''          or an empty array if no such
		'''          listeners have been added </returns>
		''' <exception cref="ClassCastException"> if <code>listenerType</code> doesn't
		'''          specify a class or interface that implements
		'''          <code>java.util.EventListener</code>
		''' </exception>
		''' <seealso cref= #getActionListeners </seealso>
		''' <seealso cref= #addActionListener </seealso>
		''' <seealso cref= #removeActionListener
		''' 
		''' @since 1.3 </seealso>
		Public Overridable Function getListeners(Of T As EventListener)(ByVal listenerType As Type) As T()
			Return listenerList.getListeners(listenerType)
		End Function

		''' <summary>
		''' Returns the timer queue.
		''' </summary>
		Private Function timerQueue() As TimerQueue
			Return TimerQueue.sharedInstance()
		End Function


		''' <summary>
		''' Enables or disables the timer log. When enabled, a message
		''' is posted to <code>System.out</code> whenever the timer goes off.
		''' </summary>
		''' <param name="flag">  <code>true</code> to enable logging </param>
		''' <seealso cref= #getLogTimers </seealso>
		Public Shared Property logTimers As Boolean
			Set(ByVal flag As Boolean)
				logTimers = flag
			End Set
			Get
				Return logTimers
			End Get
		End Property




		''' <summary>
		''' Sets the <code>Timer</code>'s between-event delay, the number of milliseconds
		''' between successive action events. This does not affect the initial delay
		''' property, which can be set by the {@code setInitialDelay} method.
		''' </summary>
		''' <param name="delay"> the delay in milliseconds </param>
		''' <seealso cref= #setInitialDelay </seealso>
		Public Overridable Property delay As Integer
			Set(ByVal delay As Integer)
				If delay < 0 Then
					Throw New System.ArgumentException("Invalid delay: " & delay)
				Else
					Me.delay = delay
				End If
			End Set
			Get
				Return delay
			End Get
		End Property




		''' <summary>
		''' Sets the <code>Timer</code>'s initial delay, the time
		''' in milliseconds to wait after the timer is started
		''' before firing the first event. Upon construction, this
		''' is set to be the same as the between-event delay,
		''' but then its value is independent and remains unaffected
		''' by changes to the between-event delay.
		''' </summary>
		''' <param name="initialDelay"> the initial delay, in milliseconds </param>
		''' <seealso cref= #setDelay </seealso>
		Public Overridable Property initialDelay As Integer
			Set(ByVal initialDelay As Integer)
				If initialDelay < 0 Then
					Throw New System.ArgumentException("Invalid initial delay: " & initialDelay)
				Else
					Me.initialDelay = initialDelay
				End If
			End Set
			Get
				Return initialDelay
			End Get
		End Property




		''' <summary>
		''' If <code>flag</code> is <code>false</code>,
		''' instructs the <code>Timer</code> to send only one
		''' action event to its listeners.
		''' </summary>
		''' <param name="flag"> specify <code>false</code> to make the timer
		'''             stop after sending its first action event </param>
		Public Overridable Property repeats As Boolean
			Set(ByVal flag As Boolean)
				repeats = flag
			End Set
			Get
				Return repeats
			End Get
		End Property




		''' <summary>
		''' Sets whether the <code>Timer</code> coalesces multiple pending
		''' <code>ActionEvent</code> firings.
		''' A busy application may not be able
		''' to keep up with a <code>Timer</code>'s event generation,
		''' causing multiple
		''' action events to be queued.  When processed,
		''' the application sends these events one after the other, causing the
		''' <code>Timer</code>'s listeners to receive a sequence of
		''' events with no delay between them. Coalescing avoids this situation
		''' by reducing multiple pending events to a single event.
		''' <code>Timer</code>s
		''' coalesce events by default.
		''' </summary>
		''' <param name="flag"> specify <code>false</code> to turn off coalescing </param>
		Public Overridable Property coalesce As Boolean
			Set(ByVal flag As Boolean)
				Dim old As Boolean = coalesce
				coalesce = flag
				If (Not old) AndAlso coalesce Then cancelEvent()
			End Set
			Get
				Return coalesce
			End Get
		End Property




		''' <summary>
		''' Sets the string that will be delivered as the action command
		''' in <code>ActionEvent</code>s fired by this timer.
		''' <code>null</code> is an acceptable value.
		''' </summary>
		''' <param name="command"> the action command
		''' @since 1.6 </param>
		Public Overridable Property actionCommand As String
			Set(ByVal command As String)
				Me.actionCommand = command
			End Set
			Get
				Return actionCommand
			End Get
		End Property




		''' <summary>
		''' Starts the <code>Timer</code>,
		''' causing it to start sending action events
		''' to its listeners.
		''' </summary>
		''' <seealso cref= #stop </seealso>
		 Public Overridable Sub start()
			timerQueue().addTimer(Me, initialDelay)
		 End Sub


		''' <summary>
		''' Returns <code>true</code> if the <code>Timer</code> is running.
		''' </summary>
		''' <seealso cref= #start </seealso>
		Public Overridable Property running As Boolean
			Get
				Return timerQueue().containsTimer(Me)
			End Get
		End Property


		''' <summary>
		''' Stops the <code>Timer</code>,
		''' causing it to stop sending action events
		''' to its listeners.
		''' </summary>
		''' <seealso cref= #start </seealso>
		Public Overridable Sub [stop]()
			lock.lock()
			Try
				cancelEvent()
				timerQueue().removeTimer(Me)
			Finally
				lock.unlock()
			End Try
		End Sub


		''' <summary>
		''' Restarts the <code>Timer</code>,
		''' canceling any pending firings and causing
		''' it to fire with its initial delay.
		''' </summary>
		Public Overridable Sub restart()
			lock.lock()
			Try
				[stop]()
				start()
			Finally
				lock.unlock()
			End Try
		End Sub


		''' <summary>
		''' Resets the internal state to indicate this Timer shouldn't notify
		''' any of its listeners. This does not stop a repeatable Timer from
		''' firing again, use <code>stop</code> for that.
		''' </summary>
		Friend Overridable Sub cancelEvent()
			notify.set(False)
		End Sub


		Friend Overridable Sub post()
			 If notify.compareAndSet(False, True) OrElse (Not coalesce) Then
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				 java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<Void>()
	'			 {
	'				 public Void run()
	'				 {
	'					 SwingUtilities.invokeLater(doPostEvent);
	'					 Return Nothing;
	'				}
	'			}, getAccessControlContext());
			 End If
		End Sub

		Friend Overridable Property lock As Lock
			Get
				Return lock
			End Get
		End Property

		Private Sub readObject(ByVal [in] As ObjectInputStream)
			Me.acc = java.security.AccessController.context
			[in].defaultReadObject()
		End Sub

	'    
	'     * We have to use readResolve because we can not initialize final
	'     * fields for deserialized object otherwise
	'     
		Private Function readResolve() As Object
			Dim ___timer As New Timer(delay, Nothing)
			___timer.listenerList = listenerList
			___timer.initialDelay = initialDelay
			___timer.delay = delay
			___timer.repeats = repeats
			___timer.coalesce = coalesce
			___timer.actionCommand = actionCommand
			Return ___timer
		End Function
	End Class

End Namespace