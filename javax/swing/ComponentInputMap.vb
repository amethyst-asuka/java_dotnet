'
' * Copyright (c) 1999, 2011, Oracle and/or its affiliates. All rights reserved.
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
	''' A <code>ComponentInputMap</code> is an <code>InputMap</code>
	''' associated with a particular <code>JComponent</code>.
	''' The component is automatically notified whenever
	''' the <code>ComponentInputMap</code> changes.
	''' <code>ComponentInputMap</code>s are used for
	''' <code>WHEN_IN_FOCUSED_WINDOW</code> bindings.
	''' 
	''' @author Scott Violet
	''' @since 1.3
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class ComponentInputMap
		Inherits InputMap

		''' <summary>
		''' Component binding is created for. </summary>
		Private component As JComponent

		''' <summary>
		''' Creates a <code>ComponentInputMap</code> associated with the
		''' specified component.
		''' </summary>
		''' <param name="component">  a non-null <code>JComponent</code> </param>
		''' <exception cref="IllegalArgumentException">  if <code>component</code> is null </exception>
		Public Sub New(ByVal component As JComponent)
			Me.component = component
			If component Is Nothing Then Throw New System.ArgumentException("ComponentInputMaps must be associated with a non-null JComponent")
		End Sub

		''' <summary>
		''' Sets the parent, which must be a <code>ComponentInputMap</code>
		''' associated with the same component as this
		''' <code>ComponentInputMap</code>.
		''' </summary>
		''' <param name="map">  a <code>ComponentInputMap</code>
		''' </param>
		''' <exception cref="IllegalArgumentException">  if <code>map</code>
		'''         is not a <code>ComponentInputMap</code>
		'''         or is not associated with the same component </exception>
		Public Overrides Property parent As InputMap
			Set(ByVal map As InputMap)
				If parent Is map Then Return
				If map IsNot Nothing AndAlso (Not(TypeOf map Is ComponentInputMap) OrElse CType(map, ComponentInputMap).component IsNot component) Then Throw New System.ArgumentException("ComponentInputMaps must have a parent ComponentInputMap associated with the same component")
				MyBase.parent = map
				component.componentInputMapChanged(Me)
			End Set
		End Property

		''' <summary>
		''' Returns the component the <code>InputMap</code> was created for.
		''' </summary>
		Public Overridable Property component As JComponent
			Get
				Return component
			End Get
		End Property

		''' <summary>
		''' Adds a binding for <code>keyStroke</code> to <code>actionMapKey</code>.
		''' If <code>actionMapKey</code> is null, this removes the current binding
		''' for <code>keyStroke</code>.
		''' </summary>
		Public Overrides Sub put(ByVal ___keyStroke As KeyStroke, ByVal actionMapKey As Object)
			MyBase.put(___keyStroke, actionMapKey)
			If component IsNot Nothing Then component.componentInputMapChanged(Me)
		End Sub

		''' <summary>
		''' Removes the binding for <code>key</code> from this object.
		''' </summary>
		Public Overrides Sub remove(ByVal key As KeyStroke)
			MyBase.remove(key)
			If component IsNot Nothing Then component.componentInputMapChanged(Me)
		End Sub

		''' <summary>
		''' Removes all the mappings from this object.
		''' </summary>
		Public Overrides Sub clear()
			Dim oldSize As Integer = size()
			MyBase.clear()
			If oldSize > 0 AndAlso component IsNot Nothing Then component.componentInputMapChanged(Me)
		End Sub
	End Class

End Namespace