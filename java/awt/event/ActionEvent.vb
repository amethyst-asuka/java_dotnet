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
	''' A semantic event which indicates that a component-defined action occurred.
	''' This high-level event is generated by a component (such as a
	''' <code>Button</code>) when
	''' the component-specific action occurs (such as being pressed).
	''' The event is passed to every <code>ActionListener</code> object
	''' that registered to receive such events using the component's
	''' <code>addActionListener</code> method.
	''' <p>
	''' <b>Note:</b> To invoke an <code>ActionEvent</code> on a
	''' <code>Button</code> using the keyboard, use the Space bar.
	''' <P>
	''' The object that implements the <code>ActionListener</code> interface
	''' gets this <code>ActionEvent</code> when the event occurs. The listener
	''' is therefore spared the details of processing individual mouse movements
	''' and mouse clicks, and can instead process a "meaningful" (semantic)
	''' event like "button pressed".
	''' <p>
	''' An unspecified behavior will be caused if the {@code id} parameter
	''' of any particular {@code ActionEvent} instance is not
	''' in the range from {@code ACTION_FIRST} to {@code ACTION_LAST}.
	''' </summary>
	''' <seealso cref= ActionListener </seealso>
	''' <seealso cref= <a href="https://docs.oracle.com/javase/tutorial/uiswing/events/actionlistener.html">Tutorial: How to Write an Action Listener</a>
	''' 
	''' @author Carl Quinn
	''' @since 1.1 </seealso>
	Public Class ActionEvent
		Inherits java.awt.AWTEvent

		''' <summary>
		''' The shift modifier. An indicator that the shift key was held
		''' down during the event.
		''' </summary>
		Public Shared ReadOnly SHIFT_MASK As Integer = java.awt.Event.SHIFT_MASK

		''' <summary>
		''' The control modifier. An indicator that the control key was held
		''' down during the event.
		''' </summary>
		Public Shared ReadOnly CTRL_MASK As Integer = java.awt.Event.CTRL_MASK

		''' <summary>
		''' The meta modifier. An indicator that the meta key was held
		''' down during the event.
		''' </summary>
		Public Shared ReadOnly META_MASK As Integer = java.awt.Event.META_MASK

		''' <summary>
		''' The alt modifier. An indicator that the alt key was held
		''' down during the event.
		''' </summary>
		Public Shared ReadOnly ALT_MASK As Integer = java.awt.Event.ALT_MASK


		''' <summary>
		''' The first number in the range of ids used for action events.
		''' </summary>
		Public Const ACTION_FIRST As Integer = 1001

		''' <summary>
		''' The last number in the range of ids used for action events.
		''' </summary>
		Public Const ACTION_LAST As Integer = 1001

		''' <summary>
		''' This event id indicates that a meaningful action occurred.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const ACTION_PERFORMED As Integer = ACTION_FIRST 'Event.ACTION_EVENT

		''' <summary>
		''' The nonlocalized string that gives more details
		''' of what actually caused the event.
		''' This information is very specific to the component
		''' that fired it.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getActionCommand </seealso>
		Friend actionCommand As String

		''' <summary>
		''' Timestamp of when this event occurred. Because an ActionEvent is a high-
		''' level, semantic event, the timestamp is typically the same as an
		''' underlying InputEvent.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getWhen </seealso>
		Friend [when] As Long

		''' <summary>
		''' This represents the key modifier that was selected,
		''' and is used to determine the state of the selected key.
		''' If no modifier has been selected it will default to
		''' zero.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getModifiers </seealso>
		Friend modifiers As Integer

	'    
	'     * JDK 1.1 serialVersionUID
	'     
		Private Const serialVersionUID As Long = -7671078796273832149L

		''' <summary>
		''' Constructs an <code>ActionEvent</code> object.
		''' <p>
		''' This method throws an
		''' <code>IllegalArgumentException</code> if <code>source</code>
		''' is <code>null</code>.
		''' A <code>null</code> <code>command</code> string is legal,
		''' but not recommended.
		''' </summary>
		''' <param name="source">  The object that originated the event </param>
		''' <param name="id">      An integer that identifies the event.
		'''                     For information on allowable values, see
		'''                     the class description for <seealso cref="ActionEvent"/> </param>
		''' <param name="command"> A string that may specify a command (possibly one
		'''                of several) associated with the event </param>
		''' <exception cref="IllegalArgumentException"> if <code>source</code> is null </exception>
		''' <seealso cref= #getSource() </seealso>
		''' <seealso cref= #getID() </seealso>
		''' <seealso cref= #getActionCommand() </seealso>
		Public Sub New(ByVal source As Object, ByVal id As Integer, ByVal command As String)
			Me.New(source, id, command, 0)
		End Sub

		''' <summary>
		''' Constructs an <code>ActionEvent</code> object with modifier keys.
		''' <p>
		''' This method throws an
		''' <code>IllegalArgumentException</code> if <code>source</code>
		''' is <code>null</code>.
		''' A <code>null</code> <code>command</code> string is legal,
		''' but not recommended.
		''' </summary>
		''' <param name="source">  The object that originated the event </param>
		''' <param name="id">      An integer that identifies the event.
		'''                     For information on allowable values, see
		'''                     the class description for <seealso cref="ActionEvent"/> </param>
		''' <param name="command"> A string that may specify a command (possibly one
		'''                of several) associated with the event </param>
		''' <param name="modifiers"> The modifier keys down during event
		'''                  (shift, ctrl, alt, meta).
		'''                  Passing negative parameter is not recommended.
		'''                  Zero value means that no modifiers were passed </param>
		''' <exception cref="IllegalArgumentException"> if <code>source</code> is null </exception>
		''' <seealso cref= #getSource() </seealso>
		''' <seealso cref= #getID() </seealso>
		''' <seealso cref= #getActionCommand() </seealso>
		''' <seealso cref= #getModifiers() </seealso>
		Public Sub New(ByVal source As Object, ByVal id As Integer, ByVal command As String, ByVal modifiers As Integer)
			Me.New(source, id, command, 0, modifiers)
		End Sub

		''' <summary>
		''' Constructs an <code>ActionEvent</code> object with the specified
		''' modifier keys and timestamp.
		''' <p>
		''' This method throws an
		''' <code>IllegalArgumentException</code> if <code>source</code>
		''' is <code>null</code>.
		''' A <code>null</code> <code>command</code> string is legal,
		''' but not recommended.
		''' </summary>
		''' <param name="source">    The object that originated the event </param>
		''' <param name="id">      An integer that identifies the event.
		'''                     For information on allowable values, see
		'''                     the class description for <seealso cref="ActionEvent"/> </param>
		''' <param name="command"> A string that may specify a command (possibly one
		'''                of several) associated with the event </param>
		''' <param name="modifiers"> The modifier keys down during event
		'''                  (shift, ctrl, alt, meta).
		'''                  Passing negative parameter is not recommended.
		'''                  Zero value means that no modifiers were passed </param>
		''' <param name="when">   A long that gives the time the event occurred.
		'''               Passing negative or zero value
		'''               is not recommended </param>
		''' <exception cref="IllegalArgumentException"> if <code>source</code> is null </exception>
		''' <seealso cref= #getSource() </seealso>
		''' <seealso cref= #getID() </seealso>
		''' <seealso cref= #getActionCommand() </seealso>
		''' <seealso cref= #getModifiers() </seealso>
		''' <seealso cref= #getWhen()
		''' 
		''' @since 1.4 </seealso>
		Public Sub New(ByVal source As Object, ByVal id As Integer, ByVal command As String, ByVal [when] As Long, ByVal modifiers As Integer)
			MyBase.New(source, id)
			Me.actionCommand = command
			Me.when = [when]
			Me.modifiers = modifiers
		End Sub

		''' <summary>
		''' Returns the command string associated with this action.
		''' This string allows a "modal" component to specify one of several
		''' commands, depending on its state. For example, a single button might
		''' toggle between "show details" and "hide details". The source object
		''' and the event would be the same in each case, but the command string
		''' would identify the intended action.
		''' <p>
		''' Note that if a <code>null</code> command string was passed
		''' to the constructor for this <code>ActionEvent</code>, this
		''' this method returns <code>null</code>.
		''' </summary>
		''' <returns> the string identifying the command for this event </returns>
		Public Overridable Property actionCommand As String
			Get
				Return actionCommand
			End Get
		End Property

		''' <summary>
		''' Returns the timestamp of when this event occurred. Because an
		''' ActionEvent is a high-level, semantic event, the timestamp is typically
		''' the same as an underlying InputEvent.
		''' </summary>
		''' <returns> this event's timestamp
		''' @since 1.4 </returns>
		Public Overridable Property [when] As Long
			Get
				Return [when]
			End Get
		End Property

		''' <summary>
		''' Returns the modifier keys held down during this action event.
		''' </summary>
		''' <returns> the bitwise-or of the modifier constants </returns>
		Public Overridable Property modifiers As Integer
			Get
				Return modifiers
			End Get
		End Property

		''' <summary>
		''' Returns a parameter string identifying this action event.
		''' This method is useful for event-logging and for debugging.
		''' </summary>
		''' <returns> a string identifying the event and its associated command </returns>
		Public Overrides Function paramString() As String
			Dim typeStr As String
			Select Case id
			  Case ACTION_PERFORMED
				  typeStr = "ACTION_PERFORMED"
			  Case Else
				  typeStr = "unknown type"
			End Select
			Return typeStr & ",cmd=" & actionCommand & ",when=" & [when] & ",modifiers=" & KeyEvent.getKeyModifiersText(modifiers)
		End Function
	End Class

End Namespace