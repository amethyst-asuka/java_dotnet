Imports javax.swing.text
Imports javax.swing
Imports javax.swing.border
Imports javax.swing.event

'
' * Copyright (c) 1998, 2004, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.text.html


	''' <summary>
	''' EditableView sets the view it contains to be visible only when the
	''' JTextComponent the view is contained in is editable. The min/pref/max
	''' size is 0 when not visible.
	''' 
	''' @author  Scott Violet
	''' </summary>
	Friend Class EditableView
		Inherits ComponentView

		Friend Sub New(ByVal e As Element)
			MyBase.New(e)
		End Sub

		Public Overrides Function getMinimumSpan(ByVal axis As Integer) As Single
			If ___isVisible Then Return MyBase.getMinimumSpan(axis)
			Return 0
		End Function

		Public Overrides Function getPreferredSpan(ByVal axis As Integer) As Single
			If ___isVisible Then Return MyBase.getPreferredSpan(axis)
			Return 0
		End Function

		Public Overrides Function getMaximumSpan(ByVal axis As Integer) As Single
			If ___isVisible Then Return MyBase.getMaximumSpan(axis)
			Return 0
		End Function

		Public Overrides Sub paint(ByVal g As Graphics, ByVal allocation As Shape)
			Dim c As Component = component
			Dim host As Container = container

			If TypeOf host Is JTextComponent AndAlso ___isVisible <> CType(host, JTextComponent).editable Then
				___isVisible = CType(host, JTextComponent).editable
				preferenceChanged(Nothing, True, True)
				host.repaint()
			End If
	'        
	'         * Note: we cannot tweak the visible state of the
	'         * component in createComponent() even though it
	'         * gets called after the setParent() call where
	'         * the value of the boolean is set.  This
	'         * because, the setComponentParent() in the
	'         * superclass, always does a setVisible(false)
	'         * after calling createComponent().   We therefore
	'         * use this flag in the paint() method to
	'         * setVisible() to true if required.
	'         
			If ___isVisible Then
				MyBase.paint(g, allocation)
			Else
				sizeize(0, 0)
			End If
			If c IsNot Nothing Then c.focusable = ___isVisible
		End Sub

		Public Overrides Property parent As View
			Set(ByVal parent As View)
				If parent IsNot Nothing Then
					Dim host As Container = parent.container
					If host IsNot Nothing Then
						If TypeOf host Is JTextComponent Then
							___isVisible = CType(host, JTextComponent).editable
						Else
							___isVisible = False
						End If
					End If
				End If
				MyBase.parent = parent
			End Set
		End Property

		''' <returns> true if the Component is visible. </returns>
		Public Property Overrides visible As Boolean
			Get
				Return ___isVisible
			End Get
		End Property

		''' <summary>
		''' Set to true if the component is visible. This is based off the
		''' editability of the container. 
		''' </summary>
		Private ___isVisible As Boolean
	End Class ' End of EditableView

End Namespace