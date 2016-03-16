Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System.Threading

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
Namespace java.awt




	''' <summary>
	''' The default KeyboardFocusManager for AWT applications. Focus traversal is
	''' done in response to a Component's focus traversal keys, and using a
	''' Container's FocusTraversalPolicy.
	''' <p>
	''' Please see
	''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/misc/focus.html">
	''' How to Use the Focus Subsystem</a>,
	''' a section in <em>The Java Tutorial</em>, and the
	''' <a href="../../java/awt/doc-files/FocusSpec.html">Focus Specification</a>
	''' for more information.
	''' 
	''' @author David Mendenhall
	''' </summary>
	''' <seealso cref= FocusTraversalPolicy </seealso>
	''' <seealso cref= Component#setFocusTraversalKeys </seealso>
	''' <seealso cref= Component#getFocusTraversalKeys
	''' @since 1.4 </seealso>
	Public Class DefaultKeyboardFocusManager
		Inherits KeyboardFocusManager

		Private Shared ReadOnly focusLog As sun.util.logging.PlatformLogger = sun.util.logging.PlatformLogger.getLogger("java.awt.focus.DefaultKeyboardFocusManager")

		' null weak references to not create too many objects
		Private Shared ReadOnly NULL_WINDOW_WR As New WeakReference(Of Window)(Nothing)
		Private Shared ReadOnly NULL_COMPONENT_WR As New WeakReference(Of Component)(Nothing)
		Private realOppositeWindowWR As WeakReference(Of Window) = NULL_WINDOW_WR
		Private realOppositeComponentWR As WeakReference(Of Component) = NULL_COMPONENT_WR
		Private inSendMessage As Integer
		Private enqueuedKeyEvents As New LinkedList(Of java.awt.event.KeyEvent)
		Private typeAheadMarkers As New LinkedList(Of TypeAheadMarker)
		Private consumeNextKeyTyped_Renamed As Boolean

		Shared Sub New()
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			sun.awt.AWTAccessor.setDefaultKeyboardFocusManagerAccessor(New sun.awt.AWTAccessor.DefaultKeyboardFocusManagerAccessor()
	'		{
	'				public  Sub  consumeNextKeyTyped(DefaultKeyboardFocusManager dkfm, KeyEvent e)
	'				{
	'					dkfm.consumeNextKeyTyped(e);
	'				}
	'			});
		End Sub

		Private Class TypeAheadMarker
			Friend after As Long
			Friend untilFocused As Component

			Friend Sub New(ByVal after As Long, ByVal untilFocused As Component)
				Me.after = after
				Me.untilFocused = untilFocused
			End Sub
			''' <summary>
			''' Returns string representation of the marker
			''' </summary>
			Public Overrides Function ToString() As String
				Return ">>> Marker after " & after & " on " & untilFocused
			End Function
		End Class

		Private Function getOwningFrameDialog(ByVal window_Renamed As Window) As Window
			Do While window_Renamed IsNot Nothing AndAlso Not(TypeOf window_Renamed Is Frame OrElse TypeOf window_Renamed Is Dialog)
				window_Renamed = CType(window_Renamed.parent, Window)
			Loop
			Return window_Renamed
		End Function

	'    
	'     * This series of restoreFocus methods is used for recovering from a
	'     * rejected focus or activation change. Rejections typically occur when
	'     * the user attempts to focus a non-focusable Component or Window.
	'     
		Private Sub restoreFocus(ByVal fe As java.awt.event.FocusEvent, ByVal newFocusedWindow As Window)
			Dim realOppositeComponent As Component = Me.realOppositeComponentWR.get()
			Dim vetoedComponent As Component = fe.component

			If newFocusedWindow IsNot Nothing AndAlso restoreFocus(newFocusedWindow, vetoedComponent, False) Then
			ElseIf realOppositeComponent IsNot Nothing AndAlso doRestoreFocus(realOppositeComponent, vetoedComponent, False) Then
			ElseIf fe.oppositeComponent IsNot Nothing AndAlso doRestoreFocus(fe.oppositeComponent, vetoedComponent, False) Then
			Else
				clearGlobalFocusOwnerPriv()
			End If
		End Sub
		Private Sub restoreFocus(ByVal we As java.awt.event.WindowEvent)
			Dim realOppositeWindow As Window = Me.realOppositeWindowWR.get()
			If realOppositeWindow IsNot Nothing AndAlso restoreFocus(realOppositeWindow, Nothing, False) Then
				' do nothing, everything is done in restoreFocus()
			ElseIf we.oppositeWindow IsNot Nothing AndAlso restoreFocus(we.oppositeWindow, Nothing, False) Then
				' do nothing, everything is done in restoreFocus()
			Else
				clearGlobalFocusOwnerPriv()
			End If
		End Sub
		Private Function restoreFocus(ByVal aWindow As Window, ByVal vetoedComponent As Component, ByVal clearOnFailure As Boolean) As Boolean
			Dim toFocus As Component = KeyboardFocusManager.getMostRecentFocusOwner(aWindow)

			If toFocus IsNot Nothing AndAlso toFocus IsNot vetoedComponent AndAlso doRestoreFocus(toFocus, vetoedComponent, False) Then
				Return True
			ElseIf clearOnFailure Then
				clearGlobalFocusOwnerPriv()
				Return True
			Else
				Return False
			End If
		End Function
		Private Function restoreFocus(ByVal toFocus As Component, ByVal clearOnFailure As Boolean) As Boolean
			Return doRestoreFocus(toFocus, Nothing, clearOnFailure)
		End Function
		Private Function doRestoreFocus(ByVal toFocus As Component, ByVal vetoedComponent As Component, ByVal clearOnFailure As Boolean) As Boolean
			If toFocus IsNot vetoedComponent AndAlso toFocus.showing AndAlso toFocus.canBeFocusOwner() AndAlso toFocus.requestFocus(False, sun.awt.CausedFocusEvent.Cause.ROLLBACK) Then
				Return True
			Else
				Dim nextFocus As Component = toFocus.nextFocusCandidate
				If nextFocus IsNot Nothing AndAlso nextFocus IsNot vetoedComponent AndAlso nextFocus.requestFocusInWindow(sun.awt.CausedFocusEvent.Cause.ROLLBACK) Then
					Return True
				ElseIf clearOnFailure Then
					clearGlobalFocusOwnerPriv()
					Return True
				Else
					Return False
				End If
			End If
		End Function

		''' <summary>
		''' A special type of SentEvent which updates a counter in the target
		''' KeyboardFocusManager if it is an instance of
		''' DefaultKeyboardFocusManager.
		''' </summary>
		Private Class DefaultKeyboardFocusManagerSentEvent
			Inherits SentEvent

	'        
	'         * serialVersionUID
	'         
			Private Const serialVersionUID As Long = -2924743257508701758L

			Public Sub New(ByVal nested As AWTEvent, ByVal toNotify As sun.awt.AppContext)
				MyBase.New(nested, toNotify)
			End Sub
			Public NotOverridable Overrides Sub dispatch()
				Dim manager As KeyboardFocusManager = KeyboardFocusManager.currentKeyboardFocusManager
				Dim defaultManager As DefaultKeyboardFocusManager = If(TypeOf manager Is DefaultKeyboardFocusManager, CType(manager, DefaultKeyboardFocusManager), Nothing)

				If defaultManager IsNot Nothing Then
					SyncLock defaultManager
						defaultManager.inSendMessage += 1
					End SyncLock
				End If

				MyBase.dispatch()

				If defaultManager IsNot Nothing Then
					SyncLock defaultManager
						defaultManager.inSendMessage -= 1
					End SyncLock
				End If
			End Sub
		End Class

		''' <summary>
		''' Sends a synthetic AWTEvent to a Component. If the Component is in
		''' the current AppContext, then the event is immediately dispatched.
		''' If the Component is in a different AppContext, then the event is
		''' posted to the other AppContext's EventQueue, and this method blocks
		''' until the event is handled or target AppContext is disposed.
		''' Returns true if successfuly dispatched event, false if failed
		''' to dispatch.
		''' </summary>
		Friend Shared Function sendMessage(ByVal target As Component, ByVal e As AWTEvent) As Boolean
			e.isPosted = True
			Dim myAppContext As sun.awt.AppContext = sun.awt.AppContext.appContext
			Dim targetAppContext As sun.awt.AppContext = target.appContext
			Dim se As SentEvent = New DefaultKeyboardFocusManagerSentEvent(e, myAppContext)

			If myAppContext Is targetAppContext Then
				se.dispatch()
			Else
				If targetAppContext.disposed Then Return False
				sun.awt.SunToolkit.postEvent(targetAppContext, se)
				If EventQueue.dispatchThread Then
					Dim edt As EventDispatchThread = CType(Thread.CurrentThread, EventDispatchThread)
                    edt.pumpEvents(SentEvent.ID, New ConditionalAnonymousInnerClassHelper)
                Else
					SyncLock se
						Do While (Not se.dispatched) AndAlso Not targetAppContext.disposed
							Try
								se.wait(1000)
							Catch ie As InterruptedException
								Exit Do
							End Try
						Loop
					End SyncLock
				End If
			End If
			Return se.dispatched
		End Function

		Private Class ConditionalAnonymousInnerClassHelper
			Implements Conditional

			Public Overridable Function evaluate() As Boolean Implements Conditional.evaluate
				Return (Not se.dispatched) AndAlso Not targetAppContext.disposed
			End Function
		End Class

	'    
	'     * Checks if the focus window event follows key events waiting in the type-ahead
	'     * queue (if any). This may happen when a user types ahead in the window, the client
	'     * listeners hang EDT for a while, and the user switches b/w toplevels. In that
	'     * case the focus window events may be dispatched before the type-ahead events
	'     * get handled. This may lead to wrong focus behavior and in order to avoid it,
	'     * the focus window events are reposted to the end of the event queue. See 6981400.
	'     
		Private Function repostIfFollowsKeyEvents(ByVal e As java.awt.event.WindowEvent) As Boolean
			If Not(TypeOf e Is sun.awt.TimedWindowEvent) Then Return False
			Dim we As sun.awt.TimedWindowEvent = CType(e, sun.awt.TimedWindowEvent)
			Dim time As Long = we.when
			SyncLock Me
				Dim ke As java.awt.event.KeyEvent = If(enqueuedKeyEvents.Count = 0, Nothing, enqueuedKeyEvents.First.Value)
				If ke IsNot Nothing AndAlso time >= ke.when Then
					Dim marker As TypeAheadMarker = If(typeAheadMarkers.Count = 0, Nothing, typeAheadMarkers.First.Value)
					If marker IsNot Nothing Then
						Dim toplevel As Window = marker.untilFocused.containingWindow
						' Check that the component awaiting focus belongs to
						' the current focused window. See 8015454.
						If toplevel IsNot Nothing AndAlso toplevel.focused Then
							sun.awt.SunToolkit.postEvent(sun.awt.AppContext.appContext, New SequencedEvent(e))
							Return True
						End If
					End If
				End If
			End SyncLock
			Return False
		End Function

		''' <summary>
		''' This method is called by the AWT event dispatcher requesting that the
		''' current KeyboardFocusManager dispatch the specified event on its behalf.
		''' DefaultKeyboardFocusManagers dispatch all FocusEvents, all WindowEvents
		''' related to focus, and all KeyEvents. These events are dispatched based
		''' on the KeyboardFocusManager's notion of the focus owner and the focused
		''' and active Windows, sometimes overriding the source of the specified
		''' AWTEvent. If this method returns <code>false</code>, then the AWT event
		''' dispatcher will attempt to dispatch the event itself.
		''' </summary>
		''' <param name="e"> the AWTEvent to be dispatched </param>
		''' <returns> <code>true</code> if this method dispatched the event;
		'''         <code>false</code> otherwise </returns>
		Public Overrides Function dispatchEvent(ByVal e As AWTEvent) As Boolean
			If focusLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) AndAlso (TypeOf e Is java.awt.event.WindowEvent OrElse TypeOf e Is java.awt.event.FocusEvent) Then focusLog.fine("" & e)
			Select Case e.iD
				Case java.awt.event.WindowEvent.WINDOW_GAINED_FOCUS
					If repostIfFollowsKeyEvents(CType(e, java.awt.event.WindowEvent)) Then Exit Select

					Dim we As java.awt.event.WindowEvent = CType(e, java.awt.event.WindowEvent)
					Dim oldFocusedWindow As Window = globalFocusedWindow
					Dim newFocusedWindow As Window = we.window
					If newFocusedWindow Is oldFocusedWindow Then Exit Select

					If Not(newFocusedWindow.focusableWindow AndAlso newFocusedWindow.visible AndAlso newFocusedWindow.displayable) Then
						' we can not accept focus on such window, so reject it.
						restoreFocus(we)
						Exit Select
					End If
					' If there exists a current focused window, then notify it
					' that it has lost focus.
					If oldFocusedWindow IsNot Nothing Then
						Dim isEventDispatched As Boolean = sendMessage(oldFocusedWindow, New java.awt.event.WindowEvent(oldFocusedWindow, java.awt.event.WindowEvent.WINDOW_LOST_FOCUS, newFocusedWindow))
						' Failed to dispatch, clear by ourselfves
						If Not isEventDispatched Then
							globalFocusOwner = Nothing
							globalFocusedWindow = Nothing
						End If
					End If

					' Because the native libraries do not post WINDOW_ACTIVATED
					' events, we need to synthesize one if the active Window
					' changed.
					Dim newActiveWindow As Window = getOwningFrameDialog(newFocusedWindow)
					Dim currentActiveWindow As Window = globalActiveWindow
					If newActiveWindow IsNot currentActiveWindow Then
						sendMessage(newActiveWindow, New java.awt.event.WindowEvent(newActiveWindow, java.awt.event.WindowEvent.WINDOW_ACTIVATED, currentActiveWindow))
						If newActiveWindow IsNot globalActiveWindow Then
							' Activation change was rejected. Unlikely, but
							' possible.
							restoreFocus(we)
							Exit Select
						End If
					End If

					globalFocusedWindow = newFocusedWindow

					If newFocusedWindow IsNot globalFocusedWindow Then
						' Focus change was rejected. Will happen if
						' newFocusedWindow is not a focusable Window.
						restoreFocus(we)
						Exit Select
					End If

					' Restore focus to the Component which last held it. We do
					' this here so that client code can override our choice in
					' a WINDOW_GAINED_FOCUS handler.
					'
					' Make sure that the focus change request doesn't change the
					' focused Window in case we are no longer the focused Window
					' when the request is handled.
					If inSendMessage = 0 Then
						' Identify which Component should initially gain focus
						' in the Window.
						'
						' * If we're in SendMessage, then this is a synthetic
						'   WINDOW_GAINED_FOCUS message which was generated by a
						'   the FOCUS_GAINED handler. Allow the Component to
						'   which the FOCUS_GAINED message was targeted to
						'   receive the focus.
						' * Otherwise, look up the correct Component here.
						'   We don't use Window.getMostRecentFocusOwner because
						'   window is focused now and 'null' will be returned


						' Calculating of most recent focus owner and focus
						' request should be synchronized on KeyboardFocusManager.class
						' to prevent from thread race when user will request
						' focus between calculation and our request.
						' But if focus transfer is synchronous, this synchronization
						' may cause deadlock, thus we don't synchronize this block.
						Dim toFocus As Component = KeyboardFocusManager.getMostRecentFocusOwner(newFocusedWindow)
						If (toFocus Is Nothing) AndAlso newFocusedWindow.focusableWindow Then toFocus = newFocusedWindow.focusTraversalPolicy.getInitialComponent(newFocusedWindow)
						Dim tempLost As Component = Nothing
						SyncLock GetType(KeyboardFocusManager)
							tempLost = newFocusedWindow.temporaryLostComponentent(Nothing)
						End SyncLock

						' The component which last has the focus when this window was focused
						' should receive focus first
						If focusLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINER) Then focusLog.finer("tempLost {0}, toFocus {1}", tempLost, toFocus)
						If tempLost IsNot Nothing Then tempLost.requestFocusInWindow(sun.awt.CausedFocusEvent.Cause.ACTIVATION)

						If toFocus IsNot Nothing AndAlso toFocus IsNot tempLost Then toFocus.requestFocusInWindow(sun.awt.CausedFocusEvent.Cause.ACTIVATION)
					End If

					Dim realOppositeWindow As Window = Me.realOppositeWindowWR.get()
					If realOppositeWindow IsNot we.oppositeWindow Then we = New java.awt.event.WindowEvent(newFocusedWindow, java.awt.event.WindowEvent.WINDOW_GAINED_FOCUS, realOppositeWindow)
					Return typeAheadAssertions(newFocusedWindow, we)

				Case java.awt.event.WindowEvent.WINDOW_ACTIVATED
					Dim we As java.awt.event.WindowEvent = CType(e, java.awt.event.WindowEvent)
					Dim oldActiveWindow As Window = globalActiveWindow
					Dim newActiveWindow As Window = we.window
					If oldActiveWindow Is newActiveWindow Then Exit Select

					' If there exists a current active window, then notify it that
					' it has lost activation.
					If oldActiveWindow IsNot Nothing Then
						Dim isEventDispatched As Boolean = sendMessage(oldActiveWindow, New java.awt.event.WindowEvent(oldActiveWindow, java.awt.event.WindowEvent.WINDOW_DEACTIVATED, newActiveWindow))
						' Failed to dispatch, clear by ourselfves
						If Not isEventDispatched Then globalActiveWindow = Nothing
						If globalActiveWindow IsNot Nothing Then Exit Select
					End If

					globalActiveWindow = newActiveWindow

					If newActiveWindow IsNot globalActiveWindow Then Exit Select

					Return typeAheadAssertions(newActiveWindow, we)

				Case java.awt.event.FocusEvent.FOCUS_GAINED
					Dim fe As java.awt.event.FocusEvent = CType(e, java.awt.event.FocusEvent)
					Dim cause As sun.awt.CausedFocusEvent.Cause = If(TypeOf fe Is sun.awt.CausedFocusEvent, CType(fe, sun.awt.CausedFocusEvent).cause, sun.awt.CausedFocusEvent.Cause.UNKNOWN)
					Dim oldFocusOwner As Component = globalFocusOwner
					Dim newFocusOwner As Component = fe.component
					If oldFocusOwner Is newFocusOwner Then
						If focusLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then focusLog.fine("Skipping {0} because focus owner is the same", e)
						' We can't just drop the event - there could be
						' type-ahead markers associated with it.
						dequeueKeyEvents(-1, newFocusOwner)
						Exit Select
					End If

					' If there exists a current focus owner, then notify it that
					' it has lost focus.
					If oldFocusOwner IsNot Nothing Then
						Dim isEventDispatched As Boolean = sendMessage(oldFocusOwner, New sun.awt.CausedFocusEvent(oldFocusOwner, java.awt.event.FocusEvent.FOCUS_LOST, fe.temporary, newFocusOwner, cause))
						' Failed to dispatch, clear by ourselfves
						If Not isEventDispatched Then
							globalFocusOwner = Nothing
							If Not fe.temporary Then globalPermanentFocusOwner = Nothing
						End If
					End If

					' Because the native windowing system has a different notion
					' of the current focus and activation states, it is possible
					' that a Component outside of the focused Window receives a
					' FOCUS_GAINED event. We synthesize a WINDOW_GAINED_FOCUS
					' event in that case.
					Dim newFocusedWindow As Window = sun.awt.SunToolkit.getContainingWindow(newFocusOwner)
					Dim currentFocusedWindow As Window = globalFocusedWindow
					If newFocusedWindow IsNot Nothing AndAlso newFocusedWindow IsNot currentFocusedWindow Then
						sendMessage(newFocusedWindow, New java.awt.event.WindowEvent(newFocusedWindow, java.awt.event.WindowEvent.WINDOW_GAINED_FOCUS, currentFocusedWindow))
						If newFocusedWindow IsNot globalFocusedWindow Then
							' Focus change was rejected. Will happen if
							' newFocusedWindow is not a focusable Window.

							' Need to recover type-ahead, but don't bother
							' restoring focus. That was done by the
							' WINDOW_GAINED_FOCUS handler
							dequeueKeyEvents(-1, newFocusOwner)
							Exit Select
						End If
					End If

					If Not(newFocusOwner.focusable AndAlso newFocusOwner.showing AndAlso (newFocusOwner.enabled OrElse cause.Equals(sun.awt.CausedFocusEvent.Cause.UNKNOWN))) Then
						' Refuse focus on a disabled component if the focus event
						' isn't of UNKNOWN reason (i.e. not a result of a direct request
						' but traversal, activation or system generated).
						' we should not accept focus on such component, so reject it.
						dequeueKeyEvents(-1, newFocusOwner)
						If KeyboardFocusManager.autoFocusTransferEnabled Then
							' If FOCUS_GAINED is for a disposed component (however
							' it shouldn't happen) its toplevel parent is null. In this
							' case we have to try to restore focus in the current focused
							' window (for the details: 6607170).
							If newFocusedWindow Is Nothing Then
								restoreFocus(fe, currentFocusedWindow)
							Else
								restoreFocus(fe, newFocusedWindow)
							End If
							mostRecentFocusOwnerner(newFocusedWindow, Nothing) ' see: 8013773
						End If
						Exit Select
					End If

					globalFocusOwner = newFocusOwner

					If newFocusOwner IsNot globalFocusOwner Then
						' Focus change was rejected. Will happen if
						' newFocusOwner is not focus traversable.
						dequeueKeyEvents(-1, newFocusOwner)
						If KeyboardFocusManager.autoFocusTransferEnabled Then restoreFocus(fe, CType(newFocusedWindow, Window))
						Exit Select
					End If

					If Not fe.temporary Then
						globalPermanentFocusOwner = newFocusOwner

						If newFocusOwner IsNot globalPermanentFocusOwner Then
							' Focus change was rejected. Unlikely, but possible.
							dequeueKeyEvents(-1, newFocusOwner)
							If KeyboardFocusManager.autoFocusTransferEnabled Then restoreFocus(fe, CType(newFocusedWindow, Window))
							Exit Select
						End If
					End If

					nativeFocusOwner = getHeavyweight(newFocusOwner)

					Dim realOppositeComponent As Component = Me.realOppositeComponentWR.get()
					If realOppositeComponent IsNot Nothing AndAlso realOppositeComponent IsNot fe.oppositeComponent Then
						fe = New sun.awt.CausedFocusEvent(newFocusOwner, java.awt.event.FocusEvent.FOCUS_GAINED, fe.temporary, realOppositeComponent, cause)
						CType(fe, AWTEvent).isPosted = True
					End If
					Return typeAheadAssertions(newFocusOwner, fe)

				Case java.awt.event.FocusEvent.FOCUS_LOST
					Dim fe As java.awt.event.FocusEvent = CType(e, java.awt.event.FocusEvent)
					Dim currentFocusOwner As Component = globalFocusOwner
					If currentFocusOwner Is Nothing Then
						If focusLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then focusLog.fine("Skipping {0} because focus owner is null", e)
						Exit Select
					End If
					' Ignore cases where a Component loses focus to itself.
					' If we make a mistake because of retargeting, then the
					' FOCUS_GAINED handler will correct it.
					If currentFocusOwner Is fe.oppositeComponent Then
						If focusLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then focusLog.fine("Skipping {0} because current focus owner is equal to opposite", e)
						Exit Select
					End If

					globalFocusOwner = Nothing

					If globalFocusOwner IsNot Nothing Then
						' Focus change was rejected. Unlikely, but possible.
						restoreFocus(currentFocusOwner, True)
						Exit Select
					End If

					If Not fe.temporary Then
						globalPermanentFocusOwner = Nothing

						If globalPermanentFocusOwner IsNot Nothing Then
							' Focus change was rejected. Unlikely, but possible.
							restoreFocus(currentFocusOwner, True)
							Exit Select
						End If
					Else
						Dim owningWindow As Window = currentFocusOwner.containingWindow
						If owningWindow IsNot Nothing Then owningWindow.temporaryLostComponent = currentFocusOwner
					End If

					nativeFocusOwner = Nothing

					fe.source = currentFocusOwner

					realOppositeComponentWR = If(fe.oppositeComponent IsNot Nothing, New WeakReference(Of Component)(currentFocusOwner), NULL_COMPONENT_WR)

					Return typeAheadAssertions(currentFocusOwner, fe)

				Case java.awt.event.WindowEvent.WINDOW_DEACTIVATED
					Dim we As java.awt.event.WindowEvent = CType(e, java.awt.event.WindowEvent)
					Dim currentActiveWindow As Window = globalActiveWindow
					If currentActiveWindow Is Nothing Then Exit Select

					If currentActiveWindow IsNot e.source Then Exit Select

					globalActiveWindow = Nothing
					If globalActiveWindow IsNot Nothing Then Exit Select

					we.source = currentActiveWindow
					Return typeAheadAssertions(currentActiveWindow, we)

				Case java.awt.event.WindowEvent.WINDOW_LOST_FOCUS
					If repostIfFollowsKeyEvents(CType(e, java.awt.event.WindowEvent)) Then Exit Select

					Dim we As java.awt.event.WindowEvent = CType(e, java.awt.event.WindowEvent)
					Dim currentFocusedWindow As Window = globalFocusedWindow
					Dim losingFocusWindow As Window = we.window
					Dim activeWindow_Renamed As Window = globalActiveWindow
					Dim oppositeWindow As Window = we.oppositeWindow
					If focusLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINE) Then focusLog.fine("Active {0}, Current focused {1}, losing focus {2} opposite {3}", activeWindow_Renamed, currentFocusedWindow, losingFocusWindow, oppositeWindow)
					If currentFocusedWindow Is Nothing Then Exit Select

					' Special case -- if the native windowing system posts an
					' event claiming that the active Window has lost focus to the
					' focused Window, then discard the event. This is an artifact
					' of the native windowing system not knowing which Window is
					' really focused.
					If inSendMessage = 0 AndAlso losingFocusWindow Is activeWindow_Renamed AndAlso oppositeWindow Is currentFocusedWindow Then Exit Select

					Dim currentFocusOwner As Component = globalFocusOwner
					If currentFocusOwner IsNot Nothing Then
						' The focus owner should always receive a FOCUS_LOST event
						' before the Window is defocused.
						Dim oppositeComp As Component = Nothing
						If oppositeWindow IsNot Nothing Then
							oppositeComp = oppositeWindow.temporaryLostComponent
							If oppositeComp Is Nothing Then oppositeComp = oppositeWindow.mostRecentFocusOwner
						End If
						If oppositeComp Is Nothing Then oppositeComp = oppositeWindow
						sendMessage(currentFocusOwner, New sun.awt.CausedFocusEvent(currentFocusOwner, java.awt.event.FocusEvent.FOCUS_LOST, True, oppositeComp, sun.awt.CausedFocusEvent.Cause.ACTIVATION))
					End If

					globalFocusedWindow = Nothing
					If globalFocusedWindow IsNot Nothing Then
						' Focus change was rejected. Unlikely, but possible.
						restoreFocus(currentFocusedWindow, Nothing, True)
						Exit Select
					End If

					we.source = currentFocusedWindow
					realOppositeWindowWR = If(oppositeWindow IsNot Nothing, New WeakReference(Of Window)(currentFocusedWindow), NULL_WINDOW_WR)
					typeAheadAssertions(currentFocusedWindow, we)

					If oppositeWindow Is Nothing Then
						' Then we need to deactive the active Window as well.
						' No need to synthesize in other cases, because
						' WINDOW_ACTIVATED will handle it if necessary.
						sendMessage(activeWindow_Renamed, New java.awt.event.WindowEvent(activeWindow_Renamed, java.awt.event.WindowEvent.WINDOW_DEACTIVATED, Nothing))
						If globalActiveWindow IsNot Nothing Then restoreFocus(currentFocusedWindow, Nothing, True)
					End If
					Exit Select

				Case java.awt.event.KeyEvent.KEY_TYPED, KeyEvent.KEY_PRESSED, KeyEvent.KEY_RELEASED
					Return typeAheadAssertions(Nothing, e)

				Case Else
					Return False
			End Select

			Return True
		End Function

		''' <summary>
		''' Called by <code>dispatchEvent</code> if no other
		''' KeyEventDispatcher in the dispatcher chain dispatched the KeyEvent, or
		''' if no other KeyEventDispatchers are registered. If the event has not
		''' been consumed, its target is enabled, and the focus owner is not null,
		''' this method dispatches the event to its target. This method will also
		''' subsequently dispatch the event to all registered
		''' KeyEventPostProcessors. After all this operations are finished,
		''' the event is passed to peers for processing.
		''' <p>
		''' In all cases, this method returns <code>true</code>, since
		''' DefaultKeyboardFocusManager is designed so that neither
		''' <code>dispatchEvent</code>, nor the AWT event dispatcher, should take
		''' further action on the event in any situation.
		''' </summary>
		''' <param name="e"> the KeyEvent to be dispatched </param>
		''' <returns> <code>true</code> </returns>
		''' <seealso cref= Component#dispatchEvent </seealso>
		Public Overrides Function dispatchKeyEvent(ByVal e As java.awt.event.KeyEvent) As Boolean
			Dim focusOwner_Renamed As Component = If(CType(e, AWTEvent).isPosted, focusOwner, e.component)

			If focusOwner_Renamed IsNot Nothing AndAlso focusOwner_Renamed.showing AndAlso focusOwner_Renamed.canBeFocusOwner() Then
				If Not e.consumed Then
					Dim comp As Component = e.component
					If comp IsNot Nothing AndAlso comp.enabled Then redispatchEvent(comp, e)
				End If
			End If
			Dim stopPostProcessing As Boolean = False
			Dim processors As IList(Of KeyEventPostProcessor) = keyEventPostProcessors
			If processors IsNot Nothing Then
				Dim iter As IEnumerator(Of KeyEventPostProcessor) = processors.GetEnumerator()
				Do While (Not stopPostProcessing) AndAlso iter.MoveNext()
					stopPostProcessing = iter.Current.postProcessKeyEvent(e)
				Loop
			End If
			If Not stopPostProcessing Then postProcessKeyEvent(e)

			' Allow the peer to process KeyEvent
			Dim source As Component = e.component
			Dim peer As java.awt.peer.ComponentPeer = source.peer

			If peer Is Nothing OrElse TypeOf peer Is java.awt.peer.LightweightPeer Then
				' if focus owner is lightweight then its native container
				' processes event
				Dim target As Container = source.nativeContainer
				If target IsNot Nothing Then peer = target.peer
			End If
			If peer IsNot Nothing Then peer.handleEvent(e)

			Return True
		End Function

		''' <summary>
		''' This method will be called by <code>dispatchKeyEvent</code>. It will
		''' handle any unconsumed KeyEvents that map to an AWT
		''' <code>MenuShortcut</code> by consuming the event and activating the
		''' shortcut.
		''' </summary>
		''' <param name="e"> the KeyEvent to post-process </param>
		''' <returns> <code>true</code> </returns>
		''' <seealso cref= #dispatchKeyEvent </seealso>
		''' <seealso cref= MenuShortcut </seealso>
		Public Overrides Function postProcessKeyEvent(ByVal e As java.awt.event.KeyEvent) As Boolean
			If Not e.consumed Then
				Dim target As Component = e.component
				Dim p As Container = CType(If(TypeOf target Is Container, target, target.parent), Container)
				If p IsNot Nothing Then p.postProcessKeyEvent(e)
			End If
			Return True
		End Function

		Private Sub pumpApprovedKeyEvents()
			Dim ke As java.awt.event.KeyEvent
			Do
				ke = Nothing
				SyncLock Me
					If enqueuedKeyEvents.Count <> 0 Then
						ke = enqueuedKeyEvents.First.Value
						If typeAheadMarkers.Count <> 0 Then
							Dim marker As TypeAheadMarker = typeAheadMarkers.First.Value
							' Fixed 5064013: may appears that the events have the same time
							' if (ke.getWhen() >= marker.after) {
							' The fix is rolled out.

							If ke.when > marker.after Then ke = Nothing
						End If
						If ke IsNot Nothing Then
							If focusLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINER) Then focusLog.finer("Pumping approved event {0}", ke)
							enqueuedKeyEvents.RemoveFirst()
						End If
					End If
				End SyncLock
				If ke IsNot Nothing Then preDispatchKeyEvent(ke)
			Loop While ke IsNot Nothing
		End Sub

		''' <summary>
		''' Dumps the list of type-ahead queue markers to stderr
		''' </summary>
		Friend Overridable Sub dumpMarkers()
			If focusLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINEST) Then
				focusLog.finest(">>> Markers dump, time: {0}", System.currentTimeMillis())
				SyncLock Me
					If typeAheadMarkers.Count <> 0 Then
						Dim iter As IEnumerator(Of TypeAheadMarker) = typeAheadMarkers.GetEnumerator()
						Do While iter.MoveNext()
							Dim marker As TypeAheadMarker = iter.Current
							focusLog.finest("    {0}", marker)
						Loop
					End If
				End SyncLock
			End If
		End Sub

		Private Function typeAheadAssertions(ByVal target As Component, ByVal e As AWTEvent) As Boolean

			' Clear any pending events here as well as in the FOCUS_GAINED
			' handler. We need this call here in case a marker was removed in
			' response to a call to dequeueKeyEvents.
			pumpApprovedKeyEvents()

			Select Case e.iD
				Case java.awt.event.KeyEvent.KEY_TYPED, KeyEvent.KEY_PRESSED, KeyEvent.KEY_RELEASED
					Dim ke As java.awt.event.KeyEvent = CType(e, java.awt.event.KeyEvent)
					SyncLock Me
						If e.isPosted AndAlso typeAheadMarkers.Count <> 0 Then
							Dim marker As TypeAheadMarker = typeAheadMarkers.First.Value
							' Fixed 5064013: may appears that the events have the same time
							' if (ke.getWhen() >= marker.after) {
							' The fix is rolled out.

							If ke.when > marker.after Then
								If focusLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINER) Then focusLog.finer("Storing event {0} because of marker {1}", ke, marker)
								enqueuedKeyEvents.AddLast(ke)
								Return True
							End If
						End If
					End SyncLock

					' KeyEvent was posted before focus change request
					Return preDispatchKeyEvent(ke)

				Case java.awt.event.FocusEvent.FOCUS_GAINED
					If focusLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINEST) Then focusLog.finest("Markers before FOCUS_GAINED on {0}", target)
					dumpMarkers()
					' Search the marker list for the first marker tied to
					' the Component which just gained focus. Then remove
					' that marker, any markers which immediately follow
					' and are tied to the same component, and all markers
					' that preceed it. This handles the case where
					' multiple focus requests were made for the same
					' Component in a row and when we lost some of the
					' earlier requests. Since FOCUS_GAINED events will
					' not be generated for these additional requests, we
					' need to clear those markers too.
					SyncLock Me
						Dim found As Boolean = False
						If hasMarker(target) Then
							Dim iter As IEnumerator(Of TypeAheadMarker) = typeAheadMarkers.GetEnumerator()
							Do While iter.MoveNext()
								If iter.Current.untilFocused Is target Then
									found = True
								ElseIf found Then
									Exit Do
								End If
								iter.remove()
							Loop
						Else
							' Exception condition - event without marker
							If focusLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINER) Then focusLog.finer("Event without marker {0}", e)
						End If
					End SyncLock
					focusLog.finest("Markers after FOCUS_GAINED")
					dumpMarkers()

					redispatchEvent(target, e)

					' Now, dispatch any pending KeyEvents which have been
					' released because of the FOCUS_GAINED event so that we don't
					' have to wait for another event to be posted to the queue.
					pumpApprovedKeyEvents()
					Return True

				Case Else
					redispatchEvent(target, e)
					Return True
			End Select
		End Function

		''' <summary>
		''' Returns true if there are some marker associated with component <code>comp</code>
		''' in a markers' queue
		''' @since 1.5
		''' </summary>
		Private Function hasMarker(ByVal comp As Component) As Boolean
			Dim iter As IEnumerator(Of TypeAheadMarker) = typeAheadMarkers.GetEnumerator()
			Do While iter.MoveNext()
				If iter.Current.untilFocused Is comp Then Return True
			Loop
			Return False
		End Function

		''' <summary>
		''' Clears markers queue
		''' @since 1.5
		''' </summary>
		Friend Overrides Sub clearMarkers()
			SyncLock Me
				typeAheadMarkers.Clear()
			End SyncLock
		End Sub

		Private Function preDispatchKeyEvent(ByVal ke As java.awt.event.KeyEvent) As Boolean
			If CType(ke, AWTEvent).isPosted Then
				Dim focusOwner_Renamed As Component = focusOwner
				ke.source = (If(focusOwner_Renamed IsNot Nothing, focusOwner_Renamed, focusedWindow))
			End If
			If ke.source Is Nothing Then Return True

			' Explicitly set the key event timestamp here (not in Component.dispatchEventImpl):
			' - A key event is anyway passed to this method which starts its actual dispatching.
			' - If a key event is put to the type ahead queue, its time stamp should not be registered
			'   until its dispatching actually starts (by this method).
			EventQueue.currentEventAndMostRecentTime = ke

			''' <summary>
			''' Fix for 4495473.
			''' This fix allows to correctly dispatch events when native
			''' event proxying mechanism is active.
			''' If it is active we should redispatch key events after
			''' we detected its correct target.
			''' </summary>
			If KeyboardFocusManager.isProxyActive(ke) Then
				Dim source As Component = CType(ke.source, Component)
				Dim target As Container = source.nativeContainer
				If target IsNot Nothing Then
					Dim peer As java.awt.peer.ComponentPeer = target.peer
					If peer IsNot Nothing Then
						peer.handleEvent(ke)
						''' <summary>
						''' Fix for 4478780 - consume event after it was dispatched by peer.
						''' </summary>
						ke.consume()
					End If
				End If
				Return True
			End If

			Dim dispatchers As IList(Of KeyEventDispatcher) = keyEventDispatchers
			If dispatchers IsNot Nothing Then
				Dim iter As IEnumerator(Of KeyEventDispatcher) = dispatchers.GetEnumerator()
				Do While iter.MoveNext()
					 If iter.Current.dispatchKeyEvent(ke) Then Return True
				Loop
			End If
			Return dispatchKeyEvent(ke)
		End Function

	'    
	'     * @param e is a KEY_PRESSED event that can be used
	'     *          to track the next KEY_TYPED related.
	'     
		Private Sub consumeNextKeyTyped(ByVal e As java.awt.event.KeyEvent)
			consumeNextKeyTyped_Renamed = True
		End Sub

		Private Sub consumeTraversalKey(ByVal e As java.awt.event.KeyEvent)
			e.consume()
			consumeNextKeyTyped_Renamed = (e.iD = java.awt.event.KeyEvent.KEY_PRESSED) AndAlso Not e.actionKey
		End Sub

	'    
	'     * return true if event was consumed
	'     
		Private Function consumeProcessedKeyEvent(ByVal e As java.awt.event.KeyEvent) As Boolean
			If (e.iD = java.awt.event.KeyEvent.KEY_TYPED) AndAlso consumeNextKeyTyped_Renamed Then
				e.consume()
				consumeNextKeyTyped_Renamed = False
				Return True
			End If
			Return False
		End Function

		''' <summary>
		''' This method initiates a focus traversal operation if and only if the
		''' KeyEvent represents a focus traversal key for the specified
		''' focusedComponent. It is expected that focusedComponent is the current
		''' focus owner, although this need not be the case. If it is not,
		''' focus traversal will nevertheless proceed as if focusedComponent
		''' were the focus owner.
		''' </summary>
		''' <param name="focusedComponent"> the Component that is the basis for a focus
		'''        traversal operation if the specified event represents a focus
		'''        traversal key for the Component </param>
		''' <param name="e"> the event that may represent a focus traversal key </param>
		Public Overrides Sub processKeyEvent(ByVal focusedComponent As Component, ByVal e As java.awt.event.KeyEvent)
			' consume processed event if needed
			If consumeProcessedKeyEvent(e) Then Return

			' KEY_TYPED events cannot be focus traversal keys
			If e.iD = java.awt.event.KeyEvent.KEY_TYPED Then Return

			If focusedComponent.focusTraversalKeysEnabled AndAlso (Not e.consumed) Then
				Dim stroke As AWTKeyStroke = AWTKeyStroke.getAWTKeyStrokeForEvent(e), oppStroke As AWTKeyStroke = AWTKeyStroke.getAWTKeyStroke(stroke.keyCode, stroke.modifiers, (Not stroke.onKeyRelease))
				Dim toTest As java.util.Set(Of AWTKeyStroke)
				Dim contains, containsOpp As Boolean

				toTest = focusedComponent.getFocusTraversalKeys(KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS)
				contains = toTest.contains(stroke)
				containsOpp = toTest.contains(oppStroke)

				If contains OrElse containsOpp Then
					consumeTraversalKey(e)
					If contains Then focusNextComponent(focusedComponent)
					Return
				ElseIf e.iD = java.awt.event.KeyEvent.KEY_PRESSED Then
					' Fix for 6637607: consumeNextKeyTyped should be reset.
					consumeNextKeyTyped_Renamed = False
				End If

				toTest = focusedComponent.getFocusTraversalKeys(KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS)
				contains = toTest.contains(stroke)
				containsOpp = toTest.contains(oppStroke)

				If contains OrElse containsOpp Then
					consumeTraversalKey(e)
					If contains Then focusPreviousComponent(focusedComponent)
					Return
				End If

				toTest = focusedComponent.getFocusTraversalKeys(KeyboardFocusManager.UP_CYCLE_TRAVERSAL_KEYS)
				contains = toTest.contains(stroke)
				containsOpp = toTest.contains(oppStroke)

				If contains OrElse containsOpp Then
					consumeTraversalKey(e)
					If contains Then upFocusCycle(focusedComponent)
					Return
				End If

				If Not((TypeOf focusedComponent Is Container) AndAlso CType(focusedComponent, Container).focusCycleRoot) Then Return

				toTest = focusedComponent.getFocusTraversalKeys(KeyboardFocusManager.DOWN_CYCLE_TRAVERSAL_KEYS)
				contains = toTest.contains(stroke)
				containsOpp = toTest.contains(oppStroke)

				If contains OrElse containsOpp Then
					consumeTraversalKey(e)
					If contains Then downFocusCycle(CType(focusedComponent, Container))
				End If
			End If
		End Sub

		''' <summary>
		''' Delays dispatching of KeyEvents until the specified Component becomes
		''' the focus owner. KeyEvents with timestamps later than the specified
		''' timestamp will be enqueued until the specified Component receives a
		''' FOCUS_GAINED event, or the AWT cancels the delay request by invoking
		''' <code>dequeueKeyEvents</code> or <code>discardKeyEvents</code>.
		''' </summary>
		''' <param name="after"> timestamp of current event, or the current, system time if
		'''        the current event has no timestamp, or the AWT cannot determine
		'''        which event is currently being handled </param>
		''' <param name="untilFocused"> Component which will receive a FOCUS_GAINED event
		'''        before any pending KeyEvents </param>
		''' <seealso cref= #dequeueKeyEvents </seealso>
		''' <seealso cref= #discardKeyEvents </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Overrides Sub enqueueKeyEvents(ByVal after As Long, ByVal untilFocused As Component)
			If untilFocused Is Nothing Then Return

			If focusLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINER) Then focusLog.finer("Enqueue at {0} for {1}", after, untilFocused)

			Dim insertionIndex As Integer = 0, i As Integer = typeAheadMarkers.Count
			Dim iter As IEnumerator(Of TypeAheadMarker) = typeAheadMarkers.listIterator(i)

			Do While i > 0
				Dim marker As TypeAheadMarker = iter.previous()
				If marker.after <= after Then
					insertionIndex = i
					Exit Do
				End If
				i -= 1
			Loop

'JAVA TO VB CONVERTER TODO TASK: There is no .NET LinkedList equivalent to the 2-parameter Java 'add' method:
			typeAheadMarkers.Add(insertionIndex, New TypeAheadMarker(after, untilFocused))
		End Sub

		''' <summary>
		''' Releases for normal dispatching to the current focus owner all
		''' KeyEvents which were enqueued because of a call to
		''' <code>enqueueKeyEvents</code> with the same timestamp and Component.
		''' If the given timestamp is less than zero, the outstanding enqueue
		''' request for the given Component with the <b>oldest</b> timestamp (if
		''' any) should be cancelled.
		''' </summary>
		''' <param name="after"> the timestamp specified in the call to
		'''        <code>enqueueKeyEvents</code>, or any value &lt; 0 </param>
		''' <param name="untilFocused"> the Component specified in the call to
		'''        <code>enqueueKeyEvents</code> </param>
		''' <seealso cref= #enqueueKeyEvents </seealso>
		''' <seealso cref= #discardKeyEvents </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Overrides Sub dequeueKeyEvents(ByVal after As Long, ByVal untilFocused As Component)
			If untilFocused Is Nothing Then Return

			If focusLog.isLoggable(sun.util.logging.PlatformLogger.Level.FINER) Then focusLog.finer("Dequeue at {0} for {1}", after, untilFocused)

			Dim marker As TypeAheadMarker
			Dim iter As IEnumerator(Of TypeAheadMarker) = typeAheadMarkers.listIterator(If(after >= 0, typeAheadMarkers.Count, 0))

			If after < 0 Then
				Do While iter.MoveNext()
					marker = iter.Current
					If marker.untilFocused Is untilFocused Then
						iter.remove()
						Return
					End If
				Loop
			Else
				Do While iter.hasPrevious()
					marker = iter.previous()
					If marker.untilFocused Is untilFocused AndAlso marker.after = after Then
						iter.remove()
						Return
					End If
				Loop
			End If
		End Sub

		''' <summary>
		''' Discards all KeyEvents which were enqueued because of one or more calls
		''' to <code>enqueueKeyEvents</code> with the specified Component, or one of
		''' its descendants.
		''' </summary>
		''' <param name="comp"> the Component specified in one or more calls to
		'''        <code>enqueueKeyEvents</code>, or a parent of such a Component </param>
		''' <seealso cref= #enqueueKeyEvents </seealso>
		''' <seealso cref= #dequeueKeyEvents </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Overrides Sub discardKeyEvents(ByVal comp As Component)
			If comp Is Nothing Then Return

			Dim start As Long = -1

			Dim iter As IEnumerator(Of TypeAheadMarker) = typeAheadMarkers.GetEnumerator()
			Do While iter.MoveNext()
				Dim marker As TypeAheadMarker = iter.Current
				Dim toTest As Component = marker.untilFocused
				Dim match As Boolean = (toTest Is comp)
				Do While (Not match) AndAlso toTest IsNot Nothing AndAlso Not(TypeOf toTest Is Window)
					toTest = toTest.parent
					match = (toTest Is comp)
				Loop
				If match Then
					If start < 0 Then start = marker.after
					iter.remove()
				ElseIf start >= 0 Then
					purgeStampedEvents(start, marker.after)
					start = -1
				End If
			Loop

			purgeStampedEvents(start, -1)
		End Sub

		' Notes:
		'   * must be called inside a synchronized block
		'   * if 'start' is < 0, then this function does nothing
		'   * if 'end' is < 0, then all KeyEvents from 'start' to the end of the
		'     queue will be removed
		Private Sub purgeStampedEvents(ByVal start As Long, ByVal [end] As Long)
			If start < 0 Then Return

			Dim iter As IEnumerator(Of java.awt.event.KeyEvent) = enqueuedKeyEvents.GetEnumerator()
			Do While iter.MoveNext()
				Dim ke As java.awt.event.KeyEvent = iter.Current
				Dim time As Long = ke.when

				If start < time AndAlso ([end] < 0 OrElse time <= [end]) Then iter.remove()

				If [end] >= 0 AndAlso time > [end] Then Exit Do
			Loop
		End Sub

		''' <summary>
		''' Focuses the Component before aComponent, typically based on a
		''' FocusTraversalPolicy.
		''' </summary>
		''' <param name="aComponent"> the Component that is the basis for the focus
		'''        traversal operation </param>
		''' <seealso cref= FocusTraversalPolicy </seealso>
		''' <seealso cref= Component#transferFocusBackward </seealso>
		Public Overrides Sub focusPreviousComponent(ByVal aComponent As Component)
			If aComponent IsNot Nothing Then aComponent.transferFocusBackward()
		End Sub

		''' <summary>
		''' Focuses the Component after aComponent, typically based on a
		''' FocusTraversalPolicy.
		''' </summary>
		''' <param name="aComponent"> the Component that is the basis for the focus
		'''        traversal operation </param>
		''' <seealso cref= FocusTraversalPolicy </seealso>
		''' <seealso cref= Component#transferFocus </seealso>
		Public Overrides Sub focusNextComponent(ByVal aComponent As Component)
			If aComponent IsNot Nothing Then aComponent.transferFocus()
		End Sub

		''' <summary>
		''' Moves the focus up one focus traversal cycle. Typically, the focus owner
		''' is set to aComponent's focus cycle root, and the current focus cycle
		''' root is set to the new focus owner's focus cycle root. If, however,
		''' aComponent's focus cycle root is a Window, then the focus owner is set
		''' to the focus cycle root's default Component to focus, and the current
		''' focus cycle root is unchanged.
		''' </summary>
		''' <param name="aComponent"> the Component that is the basis for the focus
		'''        traversal operation </param>
		''' <seealso cref= Component#transferFocusUpCycle </seealso>
		Public Overrides Sub upFocusCycle(ByVal aComponent As Component)
			If aComponent IsNot Nothing Then aComponent.transferFocusUpCycle()
		End Sub

		''' <summary>
		''' Moves the focus down one focus traversal cycle. If aContainer is a focus
		''' cycle root, then the focus owner is set to aContainer's default
		''' Component to focus, and the current focus cycle root is set to
		''' aContainer. If aContainer is not a focus cycle root, then no focus
		''' traversal operation occurs.
		''' </summary>
		''' <param name="aContainer"> the Container that is the basis for the focus
		'''        traversal operation </param>
		''' <seealso cref= Container#transferFocusDownCycle </seealso>
		Public Overrides Sub downFocusCycle(ByVal aContainer As Container)
			If aContainer IsNot Nothing AndAlso aContainer.focusCycleRoot Then aContainer.transferFocusDownCycle()
		End Sub
	End Class

End Namespace