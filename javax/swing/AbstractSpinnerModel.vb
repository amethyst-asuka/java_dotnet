Imports System
Imports javax.swing.event

'
' * Copyright (c) 2000, 2008, Oracle and/or its affiliates. All rights reserved.
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
	''' This class provides the ChangeListener part of the
	''' SpinnerModel interface that should be suitable for most concrete SpinnerModel
	''' implementations.  Subclasses must provide an implementation of the
	''' <code>setValue</code>, <code>getValue</code>, <code>getNextValue</code> and
	''' <code>getPreviousValue</code> methods.
	''' </summary>
	''' <seealso cref= JSpinner </seealso>
	''' <seealso cref= SpinnerModel </seealso>
	''' <seealso cref= SpinnerListModel </seealso>
	''' <seealso cref= SpinnerNumberModel </seealso>
	''' <seealso cref= SpinnerDateModel
	''' 
	''' @author Hans Muller
	''' @since 1.4 </seealso>
	<Serializable> _
	Public MustInherit Class AbstractSpinnerModel
		Implements SpinnerModel

			Public MustOverride ReadOnly Property previousValue As Object Implements SpinnerModel.getPreviousValue
			Public MustOverride ReadOnly Property nextValue As Object Implements SpinnerModel.getNextValue
			Public MustOverride Property value Implements SpinnerModel.setValue As Object

		''' <summary>
		''' Only one ChangeEvent is needed per model instance since the
		''' event's only (read-only) state is the source property.  The source
		''' of events generated here is always "this".
		''' </summary>
		<NonSerialized> _
		Private changeEvent As ChangeEvent = Nothing


		''' <summary>
		''' The list of ChangeListeners for this model.  Subclasses may
		''' store their own listeners here.
		''' </summary>
		Protected Friend listenerList As New EventListenerList


		''' <summary>
		''' Adds a ChangeListener to the model's listener list.  The
		''' ChangeListeners must be notified when the models value changes.
		''' </summary>
		''' <param name="l"> the ChangeListener to add </param>
		''' <seealso cref= #removeChangeListener </seealso>
		''' <seealso cref= SpinnerModel#addChangeListener </seealso>
		Public Overridable Sub addChangeListener(ByVal l As ChangeListener) Implements SpinnerModel.addChangeListener
			listenerList.add(GetType(ChangeListener), l)
		End Sub


		''' <summary>
		''' Removes a ChangeListener from the model's listener list.
		''' </summary>
		''' <param name="l"> the ChangeListener to remove </param>
		''' <seealso cref= #addChangeListener </seealso>
		''' <seealso cref= SpinnerModel#removeChangeListener </seealso>
		Public Overridable Sub removeChangeListener(ByVal l As ChangeListener) Implements SpinnerModel.removeChangeListener
			listenerList.remove(GetType(ChangeListener), l)
		End Sub


		''' <summary>
		''' Returns an array of all the <code>ChangeListener</code>s added
		''' to this AbstractSpinnerModel with addChangeListener().
		''' </summary>
		''' <returns> all of the <code>ChangeListener</code>s added or an empty
		'''         array if no listeners have been added
		''' @since 1.4 </returns>
		Public Overridable Property changeListeners As ChangeListener()
			Get
				Return listenerList.getListeners(GetType(ChangeListener))
			End Get
		End Property


		''' <summary>
		''' Run each ChangeListeners stateChanged() method.
		''' </summary>
		''' <seealso cref= #setValue </seealso>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireStateChanged()
			Dim ___listeners As Object() = listenerList.listenerList
			For i As Integer = ___listeners.Length - 2 To 0 Step -2
				If ___listeners(i) Is GetType(ChangeListener) Then
					If changeEvent Is Nothing Then changeEvent = New ChangeEvent(Me)
					CType(___listeners(i+1), ChangeListener).stateChanged(changeEvent)
				End If
			Next i
		End Sub


		''' <summary>
		''' Return an array of all the listeners of the given type that
		''' were added to this model.  For example to find all of the
		''' ChangeListeners added to this model:
		''' <pre>
		''' myAbstractSpinnerModel.getListeners(ChangeListener.class);
		''' </pre>
		''' </summary>
		''' <param name="listenerType"> the type of listeners to return, e.g. ChangeListener.class </param>
		''' <returns> all of the objects receiving <em>listenerType</em> notifications
		'''         from this model </returns>
		Public Overridable Function getListeners(Of T As EventListener)(ByVal listenerType As Type) As T()
			Return listenerList.getListeners(listenerType)
		End Function
	End Class

End Namespace