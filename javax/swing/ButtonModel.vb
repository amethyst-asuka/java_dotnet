Imports javax.swing.event

'
' * Copyright (c) 1997, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' State model for buttons.
	''' <p>
	''' This model is used for regular buttons, as well as check boxes
	''' and radio buttons, which are special kinds of buttons. In practice,
	''' a button's UI takes the responsibility of calling methods on its
	''' model to manage the state, as detailed below:
	''' <p>
	''' In simple terms, pressing and releasing the mouse over a regular
	''' button triggers the button and causes and <code>ActionEvent</code>
	''' to be fired. The same behavior can be produced via a keyboard key
	''' defined by the look and feel of the button (typically the SPACE BAR).
	''' Pressing and releasing this key while the button has
	''' focus will give the same results. For check boxes and radio buttons, the
	''' mouse or keyboard equivalent sequence just described causes the button
	''' to become selected.
	''' <p>
	''' In details, the state model for buttons works as follows
	''' when used with the mouse:
	''' <br>
	''' Pressing the mouse on top of a button makes the model both
	''' armed and pressed. As long as the mouse remains down,
	''' the model remains pressed, even if the mouse moves
	''' outside the button. On the contrary, the model is only
	''' armed while the mouse remains pressed within the bounds of
	''' the button (it can move in or out of the button, but the model
	''' is only armed during the portion of time spent within the button).
	''' A button is triggered, and an <code>ActionEvent</code> is fired,
	''' when the mouse is released while the model is armed
	''' - meaning when it is released over top of the button after the mouse
	''' has previously been pressed on that button (and not already released).
	''' Upon mouse release, the model becomes unarmed and unpressed.
	''' <p>
	''' In details, the state model for buttons works as follows
	''' when used with the keyboard:
	''' <br>
	''' Pressing the look and feel defined keyboard key while the button
	''' has focus makes the model both armed and pressed. As long as this key
	''' remains down, the model remains in this state. Releasing the key sets
	''' the model to unarmed and unpressed, triggers the button, and causes an
	''' <code>ActionEvent</code> to be fired.
	''' 
	''' @author Jeff Dinkins
	''' </summary>
	Public Interface ButtonModel
		Inherits ItemSelectable

		''' <summary>
		''' Indicates partial commitment towards triggering the
		''' button.
		''' </summary>
		''' <returns> <code>true</code> if the button is armed,
		'''         and ready to be triggered </returns>
		''' <seealso cref= #setArmed </seealso>
		Property armed As Boolean

		''' <summary>
		''' Indicates if the button has been selected. Only needed for
		''' certain types of buttons - such as radio buttons and check boxes.
		''' </summary>
		''' <returns> <code>true</code> if the button is selected </returns>
		Property selected As Boolean

		''' <summary>
		''' Indicates if the button can be selected or triggered by
		''' an input device, such as a mouse pointer.
		''' </summary>
		''' <returns> <code>true</code> if the button is enabled </returns>
		Property enabled As Boolean

		''' <summary>
		''' Indicates if the button is pressed.
		''' </summary>
		''' <returns> <code>true</code> if the button is pressed </returns>
		Property pressed As Boolean

		''' <summary>
		''' Indicates that the mouse is over the button.
		''' </summary>
		''' <returns> <code>true</code> if the mouse is over the button </returns>
		Property rollover As Boolean






		''' <summary>
		''' Sets the keyboard mnemonic (shortcut key or
		''' accelerator key) for the button.
		''' </summary>
		''' <param name="key"> an int specifying the accelerator key </param>
		Property mnemonic As Integer


		''' <summary>
		''' Sets the action command string that gets sent as part of the
		''' <code>ActionEvent</code> when the button is triggered.
		''' </summary>
		''' <param name="s"> the <code>String</code> that identifies the generated event </param>
		''' <seealso cref= #getActionCommand </seealso>
		''' <seealso cref= java.awt.event.ActionEvent#getActionCommand </seealso>
		Property actionCommand As String


		''' <summary>
		''' Identifies the group the button belongs to --
		''' needed for radio buttons, which are mutually
		''' exclusive within their group.
		''' </summary>
		''' <param name="group"> the <code>ButtonGroup</code> the button belongs to </param>
		WriteOnly Property group As ButtonGroup

		''' <summary>
		''' Adds an <code>ActionListener</code> to the model.
		''' </summary>
		''' <param name="l"> the listener to add </param>
		Sub addActionListener(ByVal l As ActionListener)

		''' <summary>
		''' Removes an <code>ActionListener</code> from the model.
		''' </summary>
		''' <param name="l"> the listener to remove </param>
		Sub removeActionListener(ByVal l As ActionListener)

		''' <summary>
		''' Adds an <code>ItemListener</code> to the model.
		''' </summary>
		''' <param name="l"> the listener to add </param>
		Sub addItemListener(ByVal l As ItemListener)

		''' <summary>
		''' Removes an <code>ItemListener</code> from the model.
		''' </summary>
		''' <param name="l"> the listener to remove </param>
		Sub removeItemListener(ByVal l As ItemListener)

		''' <summary>
		''' Adds a <code>ChangeListener</code> to the model.
		''' </summary>
		''' <param name="l"> the listener to add </param>
		Sub addChangeListener(ByVal l As ChangeListener)

		''' <summary>
		''' Removes a <code>ChangeListener</code> from the model.
		''' </summary>
		''' <param name="l"> the listener to remove </param>
		Sub removeChangeListener(ByVal l As ChangeListener)

	End Interface

End Namespace