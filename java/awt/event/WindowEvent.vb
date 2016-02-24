Imports System

'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' A low-level event that indicates that a window has changed its status. This
	''' low-level event is generated by a Window object when it is opened, closed,
	''' activated, deactivated, iconified, or deiconified, or when focus is
	''' transfered into or out of the Window.
	''' <P>
	''' The event is passed to every <code>WindowListener</code>
	''' or <code>WindowAdapter</code> object which registered to receive such
	''' events using the window's <code>addWindowListener</code> method.
	''' (<code>WindowAdapter</code> objects implement the
	''' <code>WindowListener</code> interface.) Each such listener object
	''' gets this <code>WindowEvent</code> when the event occurs.
	''' <p>
	''' An unspecified behavior will be caused if the {@code id} parameter
	''' of any particular {@code WindowEvent} instance is not
	''' in the range from {@code WINDOW_FIRST} to {@code WINDOW_LAST}.
	''' 
	''' @author Carl Quinn
	''' @author Amy Fowler
	''' </summary>
	''' <seealso cref= WindowAdapter </seealso>
	''' <seealso cref= WindowListener </seealso>
	''' <seealso cref= <a href="https://docs.oracle.com/javase/tutorial/uiswing/events/windowlistener.html">Tutorial: Writing a Window Listener</a>
	''' 
	''' @since JDK1.1 </seealso>
	Public Class WindowEvent
		Inherits ComponentEvent

		''' <summary>
		''' The first number in the range of ids used for window events.
		''' </summary>
		Public Const WINDOW_FIRST As Integer = 200

		''' <summary>
		''' The window opened event.  This event is delivered only
		''' the first time a window is made visible.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const WINDOW_OPENED As Integer = WINDOW_FIRST ' 200

		''' <summary>
		''' The "window is closing" event. This event is delivered when
		''' the user attempts to close the window from the window's system menu.
		''' If the program does not explicitly hide or dispose the window
		''' while processing this event, the window close operation will be
		''' cancelled.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared ReadOnly WINDOW_CLOSING As Integer = 1 + WINDOW_FIRST 'Event.WINDOW_DESTROY

		''' <summary>
		''' The window closed event. This event is delivered after the displayable
		''' window has been closed as the result of a call to dispose. </summary>
		''' <seealso cref= java.awt.Component#isDisplayable </seealso>
		''' <seealso cref= Window#dispose </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared ReadOnly WINDOW_CLOSED As Integer = 2 + WINDOW_FIRST

		''' <summary>
		''' The window iconified event. This event is delivered when
		''' the window has been changed from a normal to a minimized state.
		''' For many platforms, a minimized window is displayed as
		''' the icon specified in the window's iconImage property. </summary>
		''' <seealso cref= java.awt.Frame#setIconImage </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared ReadOnly WINDOW_ICONIFIED As Integer = 3 + WINDOW_FIRST 'Event.WINDOW_ICONIFY

		''' <summary>
		''' The window deiconified event type. This event is delivered when
		''' the window has been changed from a minimized to a normal state.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared ReadOnly WINDOW_DEICONIFIED As Integer = 4 + WINDOW_FIRST 'Event.WINDOW_DEICONIFY

		''' <summary>
		''' The window-activated event type. This event is delivered when the Window
		''' becomes the active Window. Only a Frame or a Dialog can be the active
		''' Window. The native windowing system may denote the active Window or its
		''' children with special decorations, such as a highlighted title bar. The
		''' active Window is always either the focused Window, or the first Frame or
		''' Dialog that is an owner of the focused Window.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared ReadOnly WINDOW_ACTIVATED As Integer = 5 + WINDOW_FIRST

		''' <summary>
		''' The window-deactivated event type. This event is delivered when the
		''' Window is no longer the active Window. Only a Frame or a Dialog can be
		''' the active Window. The native windowing system may denote the active
		''' Window or its children with special decorations, such as a highlighted
		''' title bar. The active Window is always either the focused Window, or the
		''' first Frame or Dialog that is an owner of the focused Window.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared ReadOnly WINDOW_DEACTIVATED As Integer = 6 + WINDOW_FIRST

		''' <summary>
		''' The window-gained-focus event type. This event is delivered when the
		''' Window becomes the focused Window, which means that the Window, or one
		''' of its subcomponents, will receive keyboard events.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared ReadOnly WINDOW_GAINED_FOCUS As Integer = 7 + WINDOW_FIRST

		''' <summary>
		''' The window-lost-focus event type. This event is delivered when a Window
		''' is no longer the focused Window, which means keyboard events will no
		''' longer be delivered to the Window or any of its subcomponents.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared ReadOnly WINDOW_LOST_FOCUS As Integer = 8 + WINDOW_FIRST

		''' <summary>
		''' The window-state-changed event type.  This event is delivered
		''' when a Window's state is changed by virtue of it being
		''' iconified, maximized etc.
		''' @since 1.4
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared ReadOnly WINDOW_STATE_CHANGED As Integer = 9 + WINDOW_FIRST

		''' <summary>
		''' The last number in the range of ids used for window events.
		''' </summary>
		Public Shared ReadOnly WINDOW_LAST As Integer = WINDOW_STATE_CHANGED

		''' <summary>
		''' The other Window involved in this focus or activation change. For a
		''' WINDOW_ACTIVATED or WINDOW_GAINED_FOCUS event, this is the Window that
		''' lost activation or focus. For a WINDOW_DEACTIVATED or WINDOW_LOST_FOCUS
		''' event, this is the Window that gained activation or focus. For any other
		''' type of WindowEvent, or if the focus or activation change occurs with a
		''' native application, a Java application in a different VM, or with no
		''' other Window, null is returned.
		''' </summary>
		''' <seealso cref= #getOppositeWindow
		''' @since 1.4 </seealso>
		<NonSerialized> _
		Friend opposite As java.awt.Window

		''' <summary>
		''' TBS
		''' </summary>
		Friend oldState As Integer
		Friend newState As Integer


	'    
	'     * JDK 1.1 serialVersionUID
	'     
		Private Const serialVersionUID As Long = -1567959133147912127L


		''' <summary>
		''' Constructs a <code>WindowEvent</code> object.
		''' <p>This method throws an
		''' <code>IllegalArgumentException</code> if <code>source</code>
		''' is <code>null</code>.
		''' </summary>
		''' <param name="source">    The <code>Window</code> object
		'''                    that originated the event </param>
		''' <param name="id">        An integer indicating the type of event.
		'''                     For information on allowable values, see
		'''                     the class description for <seealso cref="WindowEvent"/> </param>
		''' <param name="opposite">  The other window involved in the focus or activation
		'''                      change, or <code>null</code> </param>
		''' <param name="oldState">  Previous state of the window for window state change event.
		'''                  See {@code #getOldState()} for allowable values </param>
		''' <param name="newState">  New state of the window for window state change event.
		'''                  See {@code #getNewState()} for allowable values </param>
		''' <exception cref="IllegalArgumentException"> if <code>source</code> is null </exception>
		''' <seealso cref= #getWindow() </seealso>
		''' <seealso cref= #getID() </seealso>
		''' <seealso cref= #getOppositeWindow() </seealso>
		''' <seealso cref= #getOldState() </seealso>
		''' <seealso cref= #getNewState()
		''' @since 1.4 </seealso>
		Public Sub New(ByVal source As java.awt.Window, ByVal id As Integer, ByVal opposite As java.awt.Window, ByVal oldState As Integer, ByVal newState As Integer)
			MyBase.New(source, id)
			Me.opposite = opposite
			Me.oldState = oldState
			Me.newState = newState
		End Sub

		''' <summary>
		''' Constructs a <code>WindowEvent</code> object with the
		''' specified opposite <code>Window</code>. The opposite
		''' <code>Window</code> is the other <code>Window</code>
		''' involved in this focus or activation change.
		''' For a <code>WINDOW_ACTIVATED</code> or
		''' <code>WINDOW_GAINED_FOCUS</code> event, this is the
		''' <code>Window</code> that lost activation or focus.
		''' For a <code>WINDOW_DEACTIVATED</code> or
		''' <code>WINDOW_LOST_FOCUS</code> event, this is the
		''' <code>Window</code> that gained activation or focus.
		''' If this focus change occurs with a native application, with a
		''' Java application in a different VM, or with no other
		''' <code>Window</code>, then the opposite Window is <code>null</code>.
		''' <p>This method throws an
		''' <code>IllegalArgumentException</code> if <code>source</code>
		''' is <code>null</code>.
		''' </summary>
		''' <param name="source">     The <code>Window</code> object that
		'''                   originated the event </param>
		''' <param name="id">        An integer indicating the type of event.
		'''                     For information on allowable values, see
		'''                     the class description for <seealso cref="WindowEvent"/>.
		'''                  It is expected that this constructor will not
		'''                  be used for other then
		'''                  {@code WINDOW_ACTIVATED},{@code WINDOW_DEACTIVATED},
		'''                  {@code WINDOW_GAINED_FOCUS}, or {@code WINDOW_LOST_FOCUS}.
		'''                  {@code WindowEvent} types,
		'''                  because the opposite <code>Window</code> of other event types
		'''                  will always be {@code null}. </param>
		''' <param name="opposite">   The other <code>Window</code> involved in the
		'''                   focus or activation change, or <code>null</code> </param>
		''' <exception cref="IllegalArgumentException"> if <code>source</code> is null </exception>
		''' <seealso cref= #getWindow() </seealso>
		''' <seealso cref= #getID() </seealso>
		''' <seealso cref= #getOppositeWindow()
		''' @since 1.4 </seealso>
		Public Sub New(ByVal source As java.awt.Window, ByVal id As Integer, ByVal opposite As java.awt.Window)
			Me.New(source, id, opposite, 0, 0)
		End Sub

		''' <summary>
		''' Constructs a <code>WindowEvent</code> object with the specified
		''' previous and new window states.
		''' <p>This method throws an
		''' <code>IllegalArgumentException</code> if <code>source</code>
		''' is <code>null</code>.
		''' </summary>
		''' <param name="source">    The <code>Window</code> object
		'''                  that originated the event </param>
		''' <param name="id">        An integer indicating the type of event.
		'''                     For information on allowable values, see
		'''                     the class description for <seealso cref="WindowEvent"/>.
		'''                  It is expected that this constructor will not
		'''                  be used for other then
		'''                  {@code WINDOW_STATE_CHANGED}
		'''                  {@code WindowEvent}
		'''                  types, because the previous and new window
		'''                  states are meaningless for other event types. </param>
		''' <param name="oldState">  An integer representing the previous window state.
		'''                  See {@code #getOldState()} for allowable values </param>
		''' <param name="newState">  An integer representing the new window state.
		'''                  See {@code #getNewState()} for allowable values </param>
		''' <exception cref="IllegalArgumentException"> if <code>source</code> is null </exception>
		''' <seealso cref= #getWindow() </seealso>
		''' <seealso cref= #getID() </seealso>
		''' <seealso cref= #getOldState() </seealso>
		''' <seealso cref= #getNewState()
		''' @since 1.4 </seealso>
		Public Sub New(ByVal source As java.awt.Window, ByVal id As Integer, ByVal oldState As Integer, ByVal newState As Integer)
			Me.New(source, id, Nothing, oldState, newState)
		End Sub

		''' <summary>
		''' Constructs a <code>WindowEvent</code> object.
		''' <p>This method throws an
		''' <code>IllegalArgumentException</code> if <code>source</code>
		''' is <code>null</code>.
		''' </summary>
		''' <param name="source"> The <code>Window</code> object that originated the event </param>
		''' <param name="id">     An integer indicating the type of event.
		'''                     For information on allowable values, see
		'''                     the class description for <seealso cref="WindowEvent"/>. </param>
		''' <exception cref="IllegalArgumentException"> if <code>source</code> is null </exception>
		''' <seealso cref= #getWindow() </seealso>
		''' <seealso cref= #getID() </seealso>
		Public Sub New(ByVal source As java.awt.Window, ByVal id As Integer)
			Me.New(source, id, Nothing, 0, 0)
		End Sub

		''' <summary>
		''' Returns the originator of the event.
		''' </summary>
		''' <returns> the Window object that originated the event </returns>
		Public Overridable Property window As java.awt.Window
			Get
				Return If(TypeOf source Is java.awt.Window, CType(source, java.awt.Window), Nothing)
			End Get
		End Property

		''' <summary>
		''' Returns the other Window involved in this focus or activation change.
		''' For a WINDOW_ACTIVATED or WINDOW_GAINED_FOCUS event, this is the Window
		''' that lost activation or focus. For a WINDOW_DEACTIVATED or
		''' WINDOW_LOST_FOCUS event, this is the Window that gained activation or
		''' focus. For any other type of WindowEvent, or if the focus or activation
		''' change occurs with a native application, with a Java application in a
		''' different VM or context, or with no other Window, null is returned.
		''' </summary>
		''' <returns> the other Window involved in the focus or activation change, or
		'''         null
		''' @since 1.4 </returns>
		Public Overridable Property oppositeWindow As java.awt.Window
			Get
				If opposite Is Nothing Then Return Nothing
    
				Return If(sun.awt.SunToolkit.targetToAppContext(opposite) = sun.awt.AppContext.appContext, opposite, Nothing)
			End Get
		End Property

		''' <summary>
		''' For <code>WINDOW_STATE_CHANGED</code> events returns the
		''' previous state of the window. The state is
		''' represented as a bitwise mask.
		''' <ul>
		''' <li><code>NORMAL</code>
		''' <br>Indicates that no state bits are set.
		''' <li><code>ICONIFIED</code>
		''' <li><code>MAXIMIZED_HORIZ</code>
		''' <li><code>MAXIMIZED_VERT</code>
		''' <li><code>MAXIMIZED_BOTH</code>
		''' <br>Concatenates <code>MAXIMIZED_HORIZ</code>
		''' and <code>MAXIMIZED_VERT</code>.
		''' </ul>
		''' </summary>
		''' <returns> a bitwise mask of the previous window state </returns>
		''' <seealso cref= java.awt.Frame#getExtendedState()
		''' @since 1.4 </seealso>
		Public Overridable Property oldState As Integer
			Get
				Return oldState
			End Get
		End Property

		''' <summary>
		''' For <code>WINDOW_STATE_CHANGED</code> events returns the
		''' new state of the window. The state is
		''' represented as a bitwise mask.
		''' <ul>
		''' <li><code>NORMAL</code>
		''' <br>Indicates that no state bits are set.
		''' <li><code>ICONIFIED</code>
		''' <li><code>MAXIMIZED_HORIZ</code>
		''' <li><code>MAXIMIZED_VERT</code>
		''' <li><code>MAXIMIZED_BOTH</code>
		''' <br>Concatenates <code>MAXIMIZED_HORIZ</code>
		''' and <code>MAXIMIZED_VERT</code>.
		''' </ul>
		''' </summary>
		''' <returns> a bitwise mask of the new window state </returns>
		''' <seealso cref= java.awt.Frame#getExtendedState()
		''' @since 1.4 </seealso>
		Public Overridable Property newState As Integer
			Get
				Return newState
			End Get
		End Property

		''' <summary>
		''' Returns a parameter string identifying this event.
		''' This method is useful for event-logging and for debugging.
		''' </summary>
		''' <returns> a string identifying the event and its attributes </returns>
		Public Overrides Function paramString() As String
			Dim typeStr As String
			Select Case id
			  Case WINDOW_OPENED
				  typeStr = "WINDOW_OPENED"
			  Case WINDOW_CLOSING
				  typeStr = "WINDOW_CLOSING"
			  Case WINDOW_CLOSED
				  typeStr = "WINDOW_CLOSED"
			  Case WINDOW_ICONIFIED
				  typeStr = "WINDOW_ICONIFIED"
			  Case WINDOW_DEICONIFIED
				  typeStr = "WINDOW_DEICONIFIED"
			  Case WINDOW_ACTIVATED
				  typeStr = "WINDOW_ACTIVATED"
			  Case WINDOW_DEACTIVATED
				  typeStr = "WINDOW_DEACTIVATED"
			  Case WINDOW_GAINED_FOCUS
				  typeStr = "WINDOW_GAINED_FOCUS"
			  Case WINDOW_LOST_FOCUS
				  typeStr = "WINDOW_LOST_FOCUS"
			  Case WINDOW_STATE_CHANGED
				  typeStr = "WINDOW_STATE_CHANGED"
			  Case Else
				  typeStr = "unknown type"
			End Select
			typeStr &= ",opposite=" & oppositeWindow & ",oldState=" & oldState & ",newState=" & newState

			Return typeStr
		End Function
	End Class

End Namespace