Imports System

'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt.event



	''' <summary>
	''' An event which executes the <code>run()</code> method on a <code>Runnable
	''' </code> when dispatched by the AWT event dispatcher thread. This class can
	''' be used as a reference implementation of <code>ActiveEvent</code> rather
	''' than declaring a new class and defining <code>dispatch()</code>.<p>
	''' 
	''' Instances of this class are placed on the <code>EventQueue</code> by calls
	''' to <code>invokeLater</code> and <code>invokeAndWait</code>. Client code
	''' can use this fact to write replacement functions for <code>invokeLater
	''' </code> and <code>invokeAndWait</code> without writing special-case code
	''' in any <code>AWTEventListener</code> objects.
	''' <p>
	''' An unspecified behavior will be caused if the {@code id} parameter
	''' of any particular {@code InvocationEvent} instance is not
	''' in the range from {@code INVOCATION_FIRST} to {@code INVOCATION_LAST}.
	''' 
	''' @author      Fred Ecks
	''' @author      David Mendenhall
	''' </summary>
	''' <seealso cref=         java.awt.ActiveEvent </seealso>
	''' <seealso cref=         java.awt.EventQueue#invokeLater </seealso>
	''' <seealso cref=         java.awt.EventQueue#invokeAndWait </seealso>
	''' <seealso cref=         AWTEventListener
	''' 
	''' @since       1.2 </seealso>
	Public Class InvocationEvent
		Inherits java.awt.AWTEvent
		Implements java.awt.ActiveEvent

		Shared Sub New()
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			sun.awt.AWTAccessor.setInvocationEventAccessor(New sun.awt.AWTAccessor.InvocationEventAccessor()
	'		{
	'			@Override public  Sub  dispose(InvocationEvent invocationEvent)
	'			{
	'				invocationEvent.finishedDispatching(False);
	'			}
	'		});
		End Sub

		''' <summary>
		''' Marks the first integer id for the range of invocation event ids.
		''' </summary>
		Public Const INVOCATION_FIRST As Integer = 1200

		''' <summary>
		''' The default id for all InvocationEvents.
		''' </summary>
		Public Const INVOCATION_DEFAULT As Integer = INVOCATION_FIRST

		''' <summary>
		''' Marks the last integer id for the range of invocation event ids.
		''' </summary>
		Public Const INVOCATION_LAST As Integer = INVOCATION_DEFAULT

		''' <summary>
		''' The Runnable whose run() method will be called.
		''' </summary>
		Protected Friend runnable As Runnable

		''' <summary>
		''' The (potentially null) Object whose notifyAll() method will be called
		''' immediately after the Runnable.run() method has returned or thrown an exception
		''' or after the event was disposed.
		''' </summary>
		''' <seealso cref= #isDispatched </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Protected Friend notifier As Object

		''' <summary>
		''' The (potentially null) Runnable whose run() method will be called
		''' immediately after the event was dispatched or disposed.
		''' </summary>
		''' <seealso cref= #isDispatched
		''' @since 1.8 </seealso>
		Private ReadOnly listener As Runnable

		''' <summary>
		''' Indicates whether the <code>run()</code> method of the <code>runnable</code>
		''' was executed or not.
		''' </summary>
		''' <seealso cref= #isDispatched
		''' @since 1.7 </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private dispatched As Boolean = False

		''' <summary>
		''' Set to true if dispatch() catches Throwable and stores it in the
		''' exception instance variable. If false, Throwables are propagated up
		''' to the EventDispatchThread's dispatch loop.
		''' </summary>
		Protected Friend catchExceptions As Boolean

		''' <summary>
		''' The (potentially null) Exception thrown during execution of the
		''' Runnable.run() method. This variable will also be null if a particular
		''' instance does not catch exceptions.
		''' </summary>
		Private exception As Exception = Nothing

		''' <summary>
		''' The (potentially null) Throwable thrown during execution of the
		''' Runnable.run() method. This variable will also be null if a particular
		''' instance does not catch exceptions.
		''' </summary>
		Private throwable As Throwable = Nothing

		''' <summary>
		''' The timestamp of when this event occurred.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getWhen </seealso>
		Private [when] As Long

	'    
	'     * JDK 1.1 serialVersionUID.
	'     
		Private Const serialVersionUID As Long = 436056344909459450L

		''' <summary>
		''' Constructs an <code>InvocationEvent</code> with the specified
		''' source which will execute the runnable's <code>run</code>
		''' method when dispatched.
		''' <p>This is a convenience constructor.  An invocation of the form
		''' <tt>InvocationEvent(source, runnable)</tt>
		''' behaves in exactly the same way as the invocation of
		''' <tt><seealso cref="#InvocationEvent(Object, Runnable, Object, boolean) InvocationEvent"/>(source, runnable, null, false)</tt>.
		''' <p> This method throws an <code>IllegalArgumentException</code>
		''' if <code>source</code> is <code>null</code>.
		''' </summary>
		''' <param name="source">    The <code>Object</code> that originated the event </param>
		''' <param name="runnable">  The <code>Runnable</code> whose <code>run</code>
		'''                  method will be executed </param>
		''' <exception cref="IllegalArgumentException"> if <code>source</code> is null
		''' </exception>
		''' <seealso cref= #getSource() </seealso>
		''' <seealso cref= #InvocationEvent(Object, Runnable, Object, boolean) </seealso>
		Public Sub New(  source As Object,   runnable As Runnable)
			Me.New(source, INVOCATION_DEFAULT, runnable, Nothing, Nothing, False)
		End Sub

		''' <summary>
		''' Constructs an <code>InvocationEvent</code> with the specified
		''' source which will execute the runnable's <code>run</code>
		''' method when dispatched.  If notifier is non-<code>null</code>,
		''' <code>notifyAll()</code> will be called on it
		''' immediately after <code>run</code> has returned or thrown an exception.
		''' <p>An invocation of the form <tt>InvocationEvent(source,
		''' runnable, notifier, catchThrowables)</tt>
		''' behaves in exactly the same way as the invocation of
		''' <tt><seealso cref="#InvocationEvent(Object, int, Runnable, Object, boolean) InvocationEvent"/>(source, InvocationEvent.INVOCATION_DEFAULT, runnable, notifier, catchThrowables)</tt>.
		''' <p>This method throws an <code>IllegalArgumentException</code>
		''' if <code>source</code> is <code>null</code>.
		''' </summary>
		''' <param name="source">            The <code>Object</code> that originated
		'''                          the event </param>
		''' <param name="runnable">          The <code>Runnable</code> whose
		'''                          <code>run</code> method will be
		'''                          executed </param>
		''' <param name="notifier">          The {@code Object} whose <code>notifyAll</code>
		'''                          method will be called after
		'''                          <code>Runnable.run</code> has returned or
		'''                          thrown an exception or after the event was
		'''                          disposed </param>
		''' <param name="catchThrowables">   Specifies whether <code>dispatch</code>
		'''                          should catch Throwable when executing
		'''                          the <code>Runnable</code>'s <code>run</code>
		'''                          method, or should instead propagate those
		'''                          Throwables to the EventDispatchThread's
		'''                          dispatch loop </param>
		''' <exception cref="IllegalArgumentException"> if <code>source</code> is null
		''' </exception>
		''' <seealso cref= #getSource() </seealso>
		''' <seealso cref=     #InvocationEvent(Object, int, Runnable, Object, boolean) </seealso>
		Public Sub New(  source As Object,   runnable As Runnable,   notifier As Object,   catchThrowables As Boolean)
			Me.New(source, INVOCATION_DEFAULT, runnable, notifier, Nothing, catchThrowables)
		End Sub

		''' <summary>
		''' Constructs an <code>InvocationEvent</code> with the specified
		''' source which will execute the runnable's <code>run</code>
		''' method when dispatched.  If listener is non-<code>null</code>,
		''' <code>listener.run()</code> will be called immediately after
		''' <code>run</code> has returned, thrown an exception or the event
		''' was disposed.
		''' <p>This method throws an <code>IllegalArgumentException</code>
		''' if <code>source</code> is <code>null</code>.
		''' </summary>
		''' <param name="source">            The <code>Object</code> that originated
		'''                          the event </param>
		''' <param name="runnable">          The <code>Runnable</code> whose
		'''                          <code>run</code> method will be
		'''                          executed </param>
		''' <param name="listener">          The <code>Runnable</code>Runnable whose
		'''                          <code>run()</code> method will be called
		'''                          after the {@code InvocationEvent}
		'''                          was dispatched or disposed </param>
		''' <param name="catchThrowables">   Specifies whether <code>dispatch</code>
		'''                          should catch Throwable when executing
		'''                          the <code>Runnable</code>'s <code>run</code>
		'''                          method, or should instead propagate those
		'''                          Throwables to the EventDispatchThread's
		'''                          dispatch loop </param>
		''' <exception cref="IllegalArgumentException"> if <code>source</code> is null </exception>
		Public Sub New(  source As Object,   runnable As Runnable,   listener As Runnable,   catchThrowables As Boolean)
			Me.New(source, INVOCATION_DEFAULT, runnable, Nothing, listener, catchThrowables)
		End Sub

		''' <summary>
		''' Constructs an <code>InvocationEvent</code> with the specified
		''' source and ID which will execute the runnable's <code>run</code>
		''' method when dispatched.  If notifier is non-<code>null</code>,
		''' <code>notifyAll</code> will be called on it immediately after
		''' <code>run</code> has returned or thrown an exception.
		''' <p>This method throws an
		''' <code>IllegalArgumentException</code> if <code>source</code>
		''' is <code>null</code>.
		''' </summary>
		''' <param name="source">            The <code>Object</code> that originated
		'''                          the event </param>
		''' <param name="id">     An integer indicating the type of event.
		'''                     For information on allowable values, see
		'''                     the class description for <seealso cref="InvocationEvent"/> </param>
		''' <param name="runnable">          The <code>Runnable</code> whose
		'''                          <code>run</code> method will be executed </param>
		''' <param name="notifier">          The <code>Object</code> whose <code>notifyAll</code>
		'''                          method will be called after
		'''                          <code>Runnable.run</code> has returned or
		'''                          thrown an exception or after the event was
		'''                          disposed </param>
		''' <param name="catchThrowables">   Specifies whether <code>dispatch</code>
		'''                          should catch Throwable when executing the
		'''                          <code>Runnable</code>'s <code>run</code>
		'''                          method, or should instead propagate those
		'''                          Throwables to the EventDispatchThread's
		'''                          dispatch loop </param>
		''' <exception cref="IllegalArgumentException"> if <code>source</code> is null </exception>
		''' <seealso cref= #getSource() </seealso>
		''' <seealso cref= #getID() </seealso>
		Protected Friend Sub New(  source As Object,   id As Integer,   runnable As Runnable,   notifier As Object,   catchThrowables As Boolean)
			Me.New(source, id, runnable, notifier, Nothing, catchThrowables)
		End Sub

		Private Sub New(  source As Object,   id As Integer,   runnable As Runnable,   notifier As Object,   listener As Runnable,   catchThrowables As Boolean)
			MyBase.New(source, id)
			Me.runnable = runnable
			Me.notifier = notifier
			Me.listener = listener
			Me.catchExceptions = catchThrowables
			Me.when = System.currentTimeMillis()
		End Sub
		''' <summary>
		''' Executes the Runnable's <code>run()</code> method and notifies the
		''' notifier (if any) when <code>run()</code> has returned or thrown an exception.
		''' </summary>
		''' <seealso cref= #isDispatched </seealso>
		Public Overridable Sub dispatch()
			Try
				If catchExceptions Then
					Try
						runnable.run()
					Catch t As Throwable
						If TypeOf t Is Exception Then exception = CType(t, Exception)
						throwable = t
					End Try
				Else
					runnable.run()
				End If
			Finally
				finishedDispatching(True)
			End Try
		End Sub

		''' <summary>
		''' Returns any Exception caught while executing the Runnable's <code>run()
		''' </code> method.
		''' </summary>
		''' <returns>  A reference to the Exception if one was thrown; null if no
		'''          Exception was thrown or if this InvocationEvent does not
		'''          catch exceptions </returns>
		Public Overridable Property exception As Exception
			Get
				Return If(catchExceptions, exception, Nothing)
			End Get
		End Property

		''' <summary>
		''' Returns any Throwable caught while executing the Runnable's <code>run()
		''' </code> method.
		''' </summary>
		''' <returns>  A reference to the Throwable if one was thrown; null if no
		'''          Throwable was thrown or if this InvocationEvent does not
		'''          catch Throwables
		''' @since 1.5 </returns>
		Public Overridable Property throwable As Throwable
			Get
				Return If(catchExceptions, throwable, Nothing)
			End Get
		End Property

		''' <summary>
		''' Returns the timestamp of when this event occurred.
		''' </summary>
		''' <returns> this event's timestamp
		''' @since 1.4 </returns>
		Public Overridable Property [when] As Long
			Get
				Return [when]
			End Get
		End Property

		''' <summary>
		''' Returns {@code true} if the event is dispatched or any exception is
		''' thrown while dispatching, {@code false} otherwise. The method should
		''' be called by a waiting thread that calls the {@code notifier.wait()} method.
		''' Since spurious wakeups are possible (as explained in <seealso cref="Object#wait()"/>),
		''' this method should be used in a waiting loop to ensure that the event
		''' got dispatched:
		''' <pre>
		'''     while (!event.isDispatched()) {
		'''         notifier.wait();
		'''     }
		''' </pre>
		''' If the waiting thread wakes up without dispatching the event,
		''' the {@code isDispatched()} method returns {@code false}, and
		''' the {@code while} loop executes once more, thus, causing
		''' the awakened thread to revert to the waiting mode.
		''' <p>
		''' If the {@code notifier.notifyAll()} happens before the waiting thread
		''' enters the {@code notifier.wait()} method, the {@code while} loop ensures
		''' that the waiting thread will not enter the {@code notifier.wait()} method.
		''' Otherwise, there is no guarantee that the waiting thread will ever be woken
		''' from the wait.
		''' </summary>
		''' <returns> {@code true} if the event has been dispatched, or any exception
		''' has been thrown while dispatching, {@code false} otherwise </returns>
		''' <seealso cref= #dispatch </seealso>
		''' <seealso cref= #notifier </seealso>
		''' <seealso cref= #catchExceptions
		''' @since 1.7 </seealso>
		Public Overridable Property dispatched As Boolean
			Get
				Return dispatched
			End Get
		End Property

		''' <summary>
		''' Called when the event was dispatched or disposed </summary>
		''' <param name="dispatched"> true if the event was dispatched
		'''                   false if the event was disposed </param>
		Private Sub finishedDispatching(  dispatched As Boolean)
			Me.dispatched = dispatched

			If notifier IsNot Nothing Then
				SyncLock notifier
					notifier.notifyAll()
				End SyncLock
			End If

			If listener IsNot Nothing Then listener.run()
		End Sub

		''' <summary>
		''' Returns a parameter string identifying this event.
		''' This method is useful for event-logging and for debugging.
		''' </summary>
		''' <returns>  A string identifying the event and its attributes </returns>
		Public Overrides Function paramString() As String
			Dim typeStr As String
			Select Case id
				Case INVOCATION_DEFAULT
					typeStr = "INVOCATION_DEFAULT"
				Case Else
					typeStr = "unknown type"
			End Select
			Return typeStr & ",runnable=" & runnable & ",notifier=" & notifier & ",catchExceptions=" & catchExceptions & ",when=" & [when]
		End Function
	End Class

End Namespace