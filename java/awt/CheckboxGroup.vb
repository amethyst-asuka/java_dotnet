Imports System
Imports System.Runtime.CompilerServices

'
' * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' The <code>CheckboxGroup</code> class is used to group together
	''' a set of <code>Checkbox</code> buttons.
	''' <p>
	''' Exactly one check box button in a <code>CheckboxGroup</code> can
	''' be in the "on" state at any given time. Pushing any
	''' button sets its state to "on" and forces any other button that
	''' is in the "on" state into the "off" state.
	''' <p>
	''' The following code example produces a new check box group,
	''' with three check boxes:
	''' 
	''' <hr><blockquote><pre>
	''' setLayout(new GridLayout(3, 1));
	''' CheckboxGroup cbg = new CheckboxGroup();
	''' add(new Checkbox("one", cbg, true));
	''' add(new Checkbox("two", cbg, false));
	''' add(new Checkbox("three", cbg, false));
	''' </pre></blockquote><hr>
	''' <p>
	''' This image depicts the check box group created by this example:
	''' <p>
	''' <img src="doc-files/CheckboxGroup-1.gif"
	''' alt="Shows three checkboxes, arranged vertically, labeled one, two, and three. Checkbox one is in the on state."
	''' style="float:center; margin: 7px 10px;">
	''' <p>
	''' @author      Sami Shaio </summary>
	''' <seealso cref=         java.awt.Checkbox
	''' @since       JDK1.0 </seealso>
	<Serializable> _
	Public Class CheckboxGroup
		''' <summary>
		''' The current choice.
		''' @serial </summary>
		''' <seealso cref= #getCurrent() </seealso>
		''' <seealso cref= #setCurrent(Checkbox) </seealso>
		Friend selectedCheckbox As Checkbox = Nothing

	'    
	'     * JDK 1.1 serialVersionUID
	'     
		Private Const serialVersionUID As Long = 3729780091441768983L

		''' <summary>
		''' Creates a new instance of <code>CheckboxGroup</code>.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Gets the current choice from this check box group.
		''' The current choice is the check box in this
		''' group that is currently in the "on" state,
		''' or <code>null</code> if all check boxes in the
		''' group are off. </summary>
		''' <returns>   the check box that is currently in the
		'''                 "on" state, or <code>null</code>. </returns>
		''' <seealso cref=      java.awt.Checkbox </seealso>
		''' <seealso cref=      java.awt.CheckboxGroup#setSelectedCheckbox
		''' @since    JDK1.1 </seealso>
		Public Overridable Property selectedCheckbox As Checkbox
			Get
				Return current
			End Get
			Set(ByVal box As Checkbox)
				current = box
			End Set
		End Property

		''' @deprecated As of JDK version 1.1,
		''' replaced by <code>getSelectedCheckbox()</code>. 
		<Obsolete("As of JDK version 1.1,")> _
		Public Overridable Property current As Checkbox
			Get
				Return selectedCheckbox
			End Get
			Set(ByVal box As Checkbox)
				If box IsNot Nothing AndAlso box.group IsNot Me Then Return
				Dim oldChoice As Checkbox = Me.selectedCheckbox
				Me.selectedCheckbox = box
				If oldChoice IsNot Nothing AndAlso oldChoice IsNot box AndAlso oldChoice.group Is Me Then oldChoice.state = False
				If box IsNot Nothing AndAlso oldChoice IsNot box AndAlso (Not box.state) Then box.stateInternal = True
			End Set
		End Property



		''' <summary>
		''' Returns a string representation of this check box group,
		''' including the value of its current selection. </summary>
		''' <returns>    a string representation of this check box group. </returns>
		Public Overrides Function ToString() As String
			Return Me.GetType().name & "[selectedCheckbox=" & selectedCheckbox & "]"
		End Function

	End Class

End Namespace