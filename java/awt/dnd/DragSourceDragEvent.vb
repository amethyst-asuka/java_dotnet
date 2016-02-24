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

Namespace java.awt.dnd


	''' <summary>
	''' The <code>DragSourceDragEvent</code> is
	''' delivered from the <code>DragSourceContextPeer</code>,
	''' via the <code>DragSourceContext</code>, to the <code>DragSourceListener</code>
	''' registered with that <code>DragSourceContext</code> and with its associated
	''' <code>DragSource</code>.
	''' <p>
	''' The <code>DragSourceDragEvent</code> reports the <i>target drop action</i>
	''' and the <i>user drop action</i> that reflect the current state of
	''' the drag operation.
	''' <p>
	''' <i>Target drop action</i> is one of <code>DnDConstants</code> that represents
	''' the drop action selected by the current drop target if this drop action is
	''' supported by the drag source or <code>DnDConstants.ACTION_NONE</code> if this
	''' drop action is not supported by the drag source.
	''' <p>
	''' <i>User drop action</i> depends on the drop actions supported by the drag
	''' source and the drop action selected by the user. The user can select a drop
	''' action by pressing modifier keys during the drag operation:
	''' <pre>
	'''   Ctrl + Shift -&gt; ACTION_LINK
	'''   Ctrl         -&gt; ACTION_COPY
	'''   Shift        -&gt; ACTION_MOVE
	''' </pre>
	''' If the user selects a drop action, the <i>user drop action</i> is one of
	''' <code>DnDConstants</code> that represents the selected drop action if this
	''' drop action is supported by the drag source or
	''' <code>DnDConstants.ACTION_NONE</code> if this drop action is not supported
	''' by the drag source.
	''' <p>
	''' If the user doesn't select a drop action, the set of
	''' <code>DnDConstants</code> that represents the set of drop actions supported
	''' by the drag source is searched for <code>DnDConstants.ACTION_MOVE</code>,
	''' then for <code>DnDConstants.ACTION_COPY</code>, then for
	''' <code>DnDConstants.ACTION_LINK</code> and the <i>user drop action</i> is the
	''' first constant found. If no constant is found the <i>user drop action</i>
	''' is <code>DnDConstants.ACTION_NONE</code>.
	''' 
	''' @since 1.2
	''' 
	''' </summary>

	Public Class DragSourceDragEvent
		Inherits DragSourceEvent

		Private Const serialVersionUID As Long = 481346297933902471L

		''' <summary>
		''' Constructs a <code>DragSourceDragEvent</code>.
		''' This class is typically
		''' instantiated by the <code>DragSourceContextPeer</code>
		''' rather than directly
		''' by client code.
		''' The coordinates for this <code>DragSourceDragEvent</code>
		''' are not specified, so <code>getLocation</code> will return
		''' <code>null</code> for this event.
		''' <p>
		''' The arguments <code>dropAction</code> and <code>action</code> should
		''' be one of <code>DnDConstants</code> that represents a single action.
		''' The argument <code>modifiers</code> should be either a bitwise mask
		''' of old <code>java.awt.event.InputEvent.*_MASK</code> constants or a
		''' bitwise mask of extended <code>java.awt.event.InputEvent.*_DOWN_MASK</code>
		''' constants.
		''' This constructor does not throw any exception for invalid <code>dropAction</code>,
		''' <code>action</code> and <code>modifiers</code>.
		''' </summary>
		''' <param name="dsc"> the <code>DragSourceContext</code> that is to manage
		'''            notifications for this event. </param>
		''' <param name="dropAction"> the user drop action. </param>
		''' <param name="action"> the target drop action. </param>
		''' <param name="modifiers"> the modifier keys down during event (shift, ctrl,
		'''        alt, meta)
		'''        Either extended _DOWN_MASK or old _MASK modifiers
		'''        should be used, but both models should not be mixed
		'''        in one event. Use of the extended modifiers is
		'''        preferred.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>dsc</code> is <code>null</code>.
		''' </exception>
		''' <seealso cref= java.awt.event.InputEvent </seealso>
		''' <seealso cref= DragSourceEvent#getLocation </seealso>

		Public Sub New(ByVal dsc As DragSourceContext, ByVal dropAction As Integer, ByVal action As Integer, ByVal modifiers As Integer)
			MyBase.New(dsc)

			targetActions = action
			gestureModifiers = modifiers
			Me.dropAction = dropAction
			If (modifiers And Not(JDK_1_3_MODIFIERS Or JDK_1_4_MODIFIERS)) <> 0 Then
				invalidModifiers = True
			ElseIf (gestureModifiers <> 0) AndAlso (gestureModifiersEx = 0) Then
				newModifiersers()
			ElseIf (gestureModifiers = 0) AndAlso (gestureModifiersEx <> 0) Then
				oldModifiersers()
			Else
				invalidModifiers = True
			End If
		End Sub

		''' <summary>
		''' Constructs a <code>DragSourceDragEvent</code> given the specified
		''' <code>DragSourceContext</code>, user drop action, target drop action,
		''' modifiers and coordinates.
		''' <p>
		''' The arguments <code>dropAction</code> and <code>action</code> should
		''' be one of <code>DnDConstants</code> that represents a single action.
		''' The argument <code>modifiers</code> should be either a bitwise mask
		''' of old <code>java.awt.event.InputEvent.*_MASK</code> constants or a
		''' bitwise mask of extended <code>java.awt.event.InputEvent.*_DOWN_MASK</code>
		''' constants.
		''' This constructor does not throw any exception for invalid <code>dropAction</code>,
		''' <code>action</code> and <code>modifiers</code>.
		''' </summary>
		''' <param name="dsc"> the <code>DragSourceContext</code> associated with this
		'''        event. </param>
		''' <param name="dropAction"> the user drop action. </param>
		''' <param name="action"> the target drop action. </param>
		''' <param name="modifiers"> the modifier keys down during event (shift, ctrl,
		'''        alt, meta)
		'''        Either extended _DOWN_MASK or old _MASK modifiers
		'''        should be used, but both models should not be mixed
		'''        in one event. Use of the extended modifiers is
		'''        preferred. </param>
		''' <param name="x">   the horizontal coordinate for the cursor location </param>
		''' <param name="y">   the vertical coordinate for the cursor location
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>dsc</code> is <code>null</code>.
		''' </exception>
		''' <seealso cref= java.awt.event.InputEvent
		''' @since 1.4 </seealso>
		Public Sub New(ByVal dsc As DragSourceContext, ByVal dropAction As Integer, ByVal action As Integer, ByVal modifiers As Integer, ByVal x As Integer, ByVal y As Integer)
			MyBase.New(dsc, x, y)

			targetActions = action
			gestureModifiers = modifiers
			Me.dropAction = dropAction
			If (modifiers And Not(JDK_1_3_MODIFIERS Or JDK_1_4_MODIFIERS)) <> 0 Then
				invalidModifiers = True
			ElseIf (gestureModifiers <> 0) AndAlso (gestureModifiersEx = 0) Then
				newModifiersers()
			ElseIf (gestureModifiers = 0) AndAlso (gestureModifiersEx <> 0) Then
				oldModifiersers()
			Else
				invalidModifiers = True
			End If
		End Sub

		''' <summary>
		''' This method returns the target drop action.
		''' </summary>
		''' <returns> the target drop action. </returns>
		Public Overridable Property targetActions As Integer
			Get
				Return targetActions
			End Get
		End Property


		Private Shared ReadOnly JDK_1_3_MODIFIERS As Integer = java.awt.event.InputEvent.SHIFT_DOWN_MASK - 1
		Private Shared ReadOnly JDK_1_4_MODIFIERS As Integer = ((java.awt.event.InputEvent.ALT_GRAPH_DOWN_MASK << 1) - 1) And Not JDK_1_3_MODIFIERS

		''' <summary>
		''' This method returns an <code>int</code> representing
		''' the current state of the input device modifiers
		''' associated with the user's gesture. Typically these
		''' would be mouse buttons or keyboard modifiers.
		''' <P>
		''' If the <code>modifiers</code> passed to the constructor
		''' are invalid, this method returns them unchanged.
		''' </summary>
		''' <returns> the current state of the input device modifiers </returns>

		Public Overridable Property gestureModifiers As Integer
			Get
				Return If(invalidModifiers, gestureModifiers, gestureModifiers And JDK_1_3_MODIFIERS)
			End Get
		End Property

		''' <summary>
		''' This method returns an <code>int</code> representing
		''' the current state of the input device extended modifiers
		''' associated with the user's gesture.
		''' See <seealso cref="InputEvent#getModifiersEx"/>
		''' <P>
		''' If the <code>modifiers</code> passed to the constructor
		''' are invalid, this method returns them unchanged.
		''' </summary>
		''' <returns> the current state of the input device extended modifiers
		''' @since 1.4 </returns>

		Public Overridable Property gestureModifiersEx As Integer
			Get
				Return If(invalidModifiers, gestureModifiers, gestureModifiers And JDK_1_4_MODIFIERS)
			End Get
		End Property

		''' <summary>
		''' This method returns the user drop action.
		''' </summary>
		''' <returns> the user drop action. </returns>
		Public Overridable Property userAction As Integer
			Get
				Return dropAction
			End Get
		End Property

		''' <summary>
		''' This method returns the logical intersection of
		''' the target drop action and the set of drop actions supported by
		''' the drag source.
		''' </summary>
		''' <returns> the logical intersection of the target drop action and
		'''         the set of drop actions supported by the drag source. </returns>
		Public Overridable Property dropAction As Integer
			Get
				Return targetActions And dragSourceContext.sourceActions
			End Get
		End Property

	'    
	'     * fields
	'     

		''' <summary>
		''' The target drop action.
		''' 
		''' @serial
		''' </summary>
		Private targetActions As Integer = DnDConstants.ACTION_NONE

		''' <summary>
		''' The user drop action.
		''' 
		''' @serial
		''' </summary>
		Private dropAction As Integer = DnDConstants.ACTION_NONE

		''' <summary>
		''' The state of the input device modifiers associated with the user
		''' gesture.
		''' 
		''' @serial
		''' </summary>
		Private gestureModifiers As Integer = 0

		''' <summary>
		''' Indicates whether the <code>gestureModifiers</code> are invalid.
		''' 
		''' @serial
		''' </summary>
		Private invalidModifiers As Boolean

		''' <summary>
		''' Sets new modifiers by the old ones.
		''' The mouse modifiers have higher priority than overlaying key
		''' modifiers.
		''' </summary>
		Private Sub setNewModifiers()
			If (gestureModifiers And java.awt.event.InputEvent.BUTTON1_MASK) <> 0 Then gestureModifiers = gestureModifiers Or java.awt.event.InputEvent.BUTTON1_DOWN_MASK
			If (gestureModifiers And java.awt.event.InputEvent.BUTTON2_MASK) <> 0 Then gestureModifiers = gestureModifiers Or java.awt.event.InputEvent.BUTTON2_DOWN_MASK
			If (gestureModifiers And java.awt.event.InputEvent.BUTTON3_MASK) <> 0 Then gestureModifiers = gestureModifiers Or java.awt.event.InputEvent.BUTTON3_DOWN_MASK
			If (gestureModifiers And java.awt.event.InputEvent.SHIFT_MASK) <> 0 Then gestureModifiers = gestureModifiers Or java.awt.event.InputEvent.SHIFT_DOWN_MASK
			If (gestureModifiers And java.awt.event.InputEvent.CTRL_MASK) <> 0 Then gestureModifiers = gestureModifiers Or java.awt.event.InputEvent.CTRL_DOWN_MASK
			If (gestureModifiers And java.awt.event.InputEvent.ALT_GRAPH_MASK) <> 0 Then gestureModifiers = gestureModifiers Or java.awt.event.InputEvent.ALT_GRAPH_DOWN_MASK
		End Sub

		''' <summary>
		''' Sets old modifiers by the new ones.
		''' </summary>
		Private Sub setOldModifiers()
			If (gestureModifiers And java.awt.event.InputEvent.BUTTON1_DOWN_MASK) <> 0 Then gestureModifiers = gestureModifiers Or java.awt.event.InputEvent.BUTTON1_MASK
			If (gestureModifiers And java.awt.event.InputEvent.BUTTON2_DOWN_MASK) <> 0 Then gestureModifiers = gestureModifiers Or java.awt.event.InputEvent.BUTTON2_MASK
			If (gestureModifiers And java.awt.event.InputEvent.BUTTON3_DOWN_MASK) <> 0 Then gestureModifiers = gestureModifiers Or java.awt.event.InputEvent.BUTTON3_MASK
			If (gestureModifiers And java.awt.event.InputEvent.SHIFT_DOWN_MASK) <> 0 Then gestureModifiers = gestureModifiers Or java.awt.event.InputEvent.SHIFT_MASK
			If (gestureModifiers And java.awt.event.InputEvent.CTRL_DOWN_MASK) <> 0 Then gestureModifiers = gestureModifiers Or java.awt.event.InputEvent.CTRL_MASK
			If (gestureModifiers And java.awt.event.InputEvent.ALT_GRAPH_DOWN_MASK) <> 0 Then gestureModifiers = gestureModifiers Or java.awt.event.InputEvent.ALT_GRAPH_MASK
		End Sub
	End Class

End Namespace