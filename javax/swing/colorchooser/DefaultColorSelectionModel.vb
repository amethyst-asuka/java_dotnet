Imports System
Imports javax.swing
Imports javax.swing.event

'
' * Copyright (c) 1998, 2001, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.colorchooser


	''' <summary>
	''' A generic implementation of <code>ColorSelectionModel</code>.
	''' 
	''' @author Steve Wilson
	''' </summary>
	''' <seealso cref= java.awt.Color </seealso>
	<Serializable> _
	Public Class DefaultColorSelectionModel
		Implements ColorSelectionModel

		''' <summary>
		''' Only one <code>ChangeEvent</code> is needed per model instance
		''' since the event's only (read-only) state is the source property.
		''' The source of events generated here is always "this".
		''' </summary>
		<NonSerialized> _
		Protected Friend changeEvent As ChangeEvent = Nothing

		Protected Friend listenerList As New EventListenerList

		Private selectedColor As java.awt.Color

		''' <summary>
		''' Creates a <code>DefaultColorSelectionModel</code> with the
		''' current color set to <code>Color.white</code>.  This is
		''' the default constructor.
		''' </summary>
		Public Sub New()
			selectedColor = java.awt.Color.white
		End Sub

		''' <summary>
		''' Creates a <code>DefaultColorSelectionModel</code> with the
		''' current color set to <code>color</code>, which should be
		''' non-<code>null</code>.  Note that setting the color to
		''' <code>null</code> is undefined and may have unpredictable
		''' results.
		''' </summary>
		''' <param name="color"> the new <code>Color</code> </param>
		Public Sub New(ByVal color As java.awt.Color)
			selectedColor = color
		End Sub

		''' <summary>
		''' Returns the selected <code>Color</code> which should be
		''' non-<code>null</code>.
		''' </summary>
		''' <returns> the selected <code>Color</code> </returns>
		Public Overridable Property selectedColor As java.awt.Color Implements ColorSelectionModel.getSelectedColor
			Get
				Return selectedColor
			End Get
			Set(ByVal color As java.awt.Color)
				If color IsNot Nothing AndAlso (Not selectedColor.Equals(color)) Then
					selectedColor = color
					fireStateChanged()
				End If
			End Set
		End Property



		''' <summary>
		''' Adds a <code>ChangeListener</code> to the model.
		''' </summary>
		''' <param name="l"> the <code>ChangeListener</code> to be added </param>
		Public Overridable Sub addChangeListener(ByVal l As ChangeListener) Implements ColorSelectionModel.addChangeListener
			listenerList.add(GetType(ChangeListener), l)
		End Sub

		''' <summary>
		''' Removes a <code>ChangeListener</code> from the model. </summary>
		''' <param name="l"> the <code>ChangeListener</code> to be removed </param>
		Public Overridable Sub removeChangeListener(ByVal l As ChangeListener) Implements ColorSelectionModel.removeChangeListener
			listenerList.remove(GetType(ChangeListener), l)
		End Sub

		''' <summary>
		''' Returns an array of all the <code>ChangeListener</code>s added
		''' to this <code>DefaultColorSelectionModel</code> with
		''' <code>addChangeListener</code>.
		''' </summary>
		''' <returns> all of the <code>ChangeListener</code>s added, or an empty
		'''         array if no listeners have been added
		''' @since 1.4 </returns>
		Public Overridable Property changeListeners As ChangeListener()
			Get
				Return listenerList.getListeners(GetType(ChangeListener))
			End Get
		End Property

		''' <summary>
		''' Runs each <code>ChangeListener</code>'s
		''' <code>stateChanged</code> method.
		''' </summary>
		''' <!-- <seealso cref= #setRangeProperties    //bad link--> </seealso>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireStateChanged()
			Dim listeners As Object() = listenerList.listenerList
			For i As Integer = listeners.Length - 2 To 0 Step -2
				If listeners(i) Is GetType(ChangeListener) Then
					If changeEvent Is Nothing Then changeEvent = New ChangeEvent(Me)
					CType(listeners(i+1), ChangeListener).stateChanged(changeEvent)
				End If
			Next i
		End Sub

	End Class

End Namespace