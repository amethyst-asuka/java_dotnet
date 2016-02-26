Imports System
Imports System.Collections.Generic

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
Namespace javax.swing



	''' <summary>
	''' The default model for combo boxes.
	''' </summary>
	''' @param <E> the type of the elements of this model
	''' 
	''' @author Arnaud Weber
	''' @author Tom Santos </param>

	<Serializable> _
	Public Class DefaultComboBoxModel(Of E)
		Inherits AbstractListModel(Of E)
		Implements MutableComboBoxModel(Of E)

		Friend objects As List(Of E)
		Friend selectedObject As Object

		''' <summary>
		''' Constructs an empty DefaultComboBoxModel object.
		''' </summary>
		Public Sub New()
			objects = New List(Of E)
		End Sub

		''' <summary>
		''' Constructs a DefaultComboBoxModel object initialized with
		''' an array of objects.
		''' </summary>
		''' <param name="items">  an array of Object objects </param>
		Public Sub New(ByVal items As E())
			objects = New List(Of E)(items.Length)

			Dim i, c As Integer
			i=0
			c=items.Length
			Do While i<c
				objects.Add(items(i))
				i += 1
			Loop

			If size > 0 Then selectedObject = getElementAt(0)
		End Sub

		''' <summary>
		''' Constructs a DefaultComboBoxModel object initialized with
		''' a vector.
		''' </summary>
		''' <param name="v">  a Vector object ... </param>
		Public Sub New(ByVal v As List(Of E))
			objects = v

			If size > 0 Then selectedObject = getElementAt(0)
		End Sub

		' implements javax.swing.ComboBoxModel
		''' <summary>
		''' Set the value of the selected item. The selected item may be null.
		''' </summary>
		''' <param name="anObject"> The combo box value or null for no selection. </param>
		Public Overridable Property selectedItem As Object
			Set(ByVal anObject As Object)
				If (selectedObject IsNot Nothing AndAlso (Not selectedObject.Equals(anObject))) OrElse selectedObject Is Nothing AndAlso anObject IsNot Nothing Then
					selectedObject = anObject
					fireContentsChanged(Me, -1, -1)
				End If
			End Set
			Get
				Return selectedObject
			End Get
		End Property

		' implements javax.swing.ComboBoxModel

		' implements javax.swing.ListModel
		Public Overridable Property size As Integer
			Get
				Return objects.Count
			End Get
		End Property

		' implements javax.swing.ListModel
		Public Overridable Function getElementAt(ByVal index As Integer) As E
			If index >= 0 AndAlso index < objects.Count Then
				Return objects(index)
			Else
				Return Nothing
			End If
		End Function

		''' <summary>
		''' Returns the index-position of the specified object in the list.
		''' </summary>
		''' <param name="anObject"> </param>
		''' <returns> an int representing the index position, where 0 is
		'''         the first position </returns>
		Public Overridable Function getIndexOf(ByVal anObject As Object) As Integer
			Return objects.IndexOf(anObject)
		End Function

		' implements javax.swing.MutableComboBoxModel
		Public Overridable Sub addElement(ByVal anObject As E)
			objects.Add(anObject)
			fireIntervalAdded(Me,objects.Count-1, objects.Count-1)
			If objects.Count = 1 AndAlso selectedObject Is Nothing AndAlso anObject IsNot Nothing Then selectedItem = anObject
		End Sub

		' implements javax.swing.MutableComboBoxModel
		Public Overridable Sub insertElementAt(ByVal anObject As E, ByVal index As Integer)
			objects.Insert(index, anObject)
			fireIntervalAdded(Me, index, index)
		End Sub

		' implements javax.swing.MutableComboBoxModel
		Public Overridable Sub removeElementAt(ByVal index As Integer) Implements MutableComboBoxModel(Of E).removeElementAt
			If getElementAt(index) Is selectedObject Then
				If index = 0 Then
					selectedItem = If(size = 1, Nothing, getElementAt(index + 1))
				Else
					selectedItem = getElementAt(index - 1)
				End If
			End If

			objects.RemoveAt(index)

			fireIntervalRemoved(Me, index, index)
		End Sub

		' implements javax.swing.MutableComboBoxModel
		Public Overridable Sub removeElement(ByVal anObject As Object) Implements MutableComboBoxModel(Of E).removeElement
			Dim index As Integer = objects.IndexOf(anObject)
			If index <> -1 Then removeElementAt(index)
		End Sub

		''' <summary>
		''' Empties the list.
		''' </summary>
		Public Overridable Sub removeAllElements()
			If objects.Count > 0 Then
				Dim firstIndex As Integer = 0
				Dim lastIndex As Integer = objects.Count - 1
				objects.Clear()
				selectedObject = Nothing
				fireIntervalRemoved(Me, firstIndex, lastIndex)
			Else
				selectedObject = Nothing
			End If
		End Sub
	End Class

End Namespace