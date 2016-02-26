Imports System
Imports System.Collections.Generic

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
	''' This class is used to create a multiple-exclusion scope for
	''' a set of buttons. Creating a set of buttons with the
	''' same <code>ButtonGroup</code> object means that
	''' turning "on" one of those buttons
	''' turns off all other buttons in the group.
	''' <p>
	''' A <code>ButtonGroup</code> can be used with
	''' any set of objects that inherit from <code>AbstractButton</code>.
	''' Typically a button group contains instances of
	''' <code>JRadioButton</code>,
	''' <code>JRadioButtonMenuItem</code>,
	''' or <code>JToggleButton</code>.
	''' It wouldn't make sense to put an instance of
	''' <code>JButton</code> or <code>JMenuItem</code>
	''' in a button group
	''' because <code>JButton</code> and <code>JMenuItem</code>
	''' don't implement the selected state.
	''' <p>
	''' Initially, all buttons in the group are unselected.
	''' <p>
	''' For examples and further information on using button groups see
	''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/components/button.html#radiobutton">How to Use Radio Buttons</a>,
	''' a section in <em>The Java Tutorial</em>.
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' 
	''' @author Jeff Dinkins
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<Serializable> _
	Public Class ButtonGroup

		' the list of buttons participating in this group
		Protected Friend buttons As New List(Of AbstractButton)

		''' <summary>
		''' The current selection.
		''' </summary>
		Friend selection As ButtonModel = Nothing

		''' <summary>
		''' Creates a new <code>ButtonGroup</code>.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Adds the button to the group. </summary>
		''' <param name="b"> the button to be added </param>
		Public Overridable Sub add(ByVal b As AbstractButton)
			If b Is Nothing Then Return
			buttons.Add(b)

			If b.selected Then
				If selection Is Nothing Then
					selection = b.model
				Else
					b.selected = False
				End If
			End If

			b.model.group = Me
		End Sub

		''' <summary>
		''' Removes the button from the group. </summary>
		''' <param name="b"> the button to be removed </param>
		Public Overridable Sub remove(ByVal b As AbstractButton)
			If b Is Nothing Then Return
			buttons.Remove(b)
			If b.model Is selection Then selection = Nothing
			b.model.group = Nothing
		End Sub

		''' <summary>
		''' Clears the selection such that none of the buttons
		''' in the <code>ButtonGroup</code> are selected.
		''' 
		''' @since 1.6
		''' </summary>
		Public Overridable Sub clearSelection()
			If selection IsNot Nothing Then
				Dim oldSelection As ButtonModel = selection
				selection = Nothing
				oldSelection.selected = False
			End If
		End Sub

		''' <summary>
		''' Returns all the buttons that are participating in
		''' this group. </summary>
		''' <returns> an <code>Enumeration</code> of the buttons in this group </returns>
		Public Overridable Property elements As System.Collections.IEnumerator(Of AbstractButton)
			Get
				Return buttons.elements()
			End Get
		End Property

		''' <summary>
		''' Returns the model of the selected button. </summary>
		''' <returns> the selected button model </returns>
		Public Overridable Property selection As ButtonModel
			Get
				Return selection
			End Get
		End Property

		''' <summary>
		''' Sets the selected value for the <code>ButtonModel</code>.
		''' Only one button in the group may be selected at a time. </summary>
		''' <param name="m"> the <code>ButtonModel</code> </param>
		''' <param name="b"> <code>true</code> if this button is to be
		'''   selected, otherwise <code>false</code> </param>
		Public Overridable Sub setSelected(ByVal m As ButtonModel, ByVal b As Boolean)
			If b AndAlso m IsNot Nothing AndAlso m IsNot selection Then
				Dim oldSelection As ButtonModel = selection
				selection = m
				If oldSelection IsNot Nothing Then oldSelection.selected = False
				m.selected = True
			End If
		End Sub

		''' <summary>
		''' Returns whether a <code>ButtonModel</code> is selected. </summary>
		''' <returns> <code>true</code> if the button is selected,
		'''   otherwise returns <code>false</code> </returns>
		Public Overridable Function isSelected(ByVal m As ButtonModel) As Boolean
			Return (m Is selection)
		End Function

		''' <summary>
		''' Returns the number of buttons in the group. </summary>
		''' <returns> the button count
		''' @since 1.3 </returns>
		Public Overridable Property buttonCount As Integer
			Get
				If buttons Is Nothing Then
					Return 0
				Else
					Return buttons.Count
				End If
			End Get
		End Property

	End Class

End Namespace