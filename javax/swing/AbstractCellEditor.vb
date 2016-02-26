Imports System
Imports javax.swing.event

'
' * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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


	''' 
	''' <summary>
	''' A base class for <code>CellEditors</code>, providing default
	''' implementations for the methods in the <code>CellEditor</code>
	''' interface except <code>getCellEditorValue()</code>.
	''' Like the other abstract implementations in Swing, also manages a list
	''' of listeners.
	''' 
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
	''' @author Philip Milne
	''' @since 1.3
	''' </summary>

	<Serializable> _
	Public MustInherit Class AbstractCellEditor
		Implements CellEditor

			Public MustOverride Function isCellEditable(ByVal anEvent As java.util.EventObject) As Boolean Implements CellEditor.isCellEditable
			Public MustOverride ReadOnly Property cellEditorValue As Object Implements CellEditor.getCellEditorValue

		Protected Friend listenerList As New EventListenerList
		<NonSerialized> _
		Protected Friend changeEvent As ChangeEvent = Nothing

		' Force this to be implemented.
		' public Object  getCellEditorValue()

		''' <summary>
		''' Returns true. </summary>
		''' <param name="e">  an event object </param>
		''' <returns> true </returns>
		Public Overridable Function isCellEditable(ByVal e As java.util.EventObject) As Boolean Implements CellEditor.isCellEditable
			Return True
		End Function

		''' <summary>
		''' Returns true. </summary>
		''' <param name="anEvent">  an event object </param>
		''' <returns> true </returns>
		Public Overridable Function shouldSelectCell(ByVal anEvent As java.util.EventObject) As Boolean Implements CellEditor.shouldSelectCell
			Return True
		End Function

		''' <summary>
		''' Calls <code>fireEditingStopped</code> and returns true. </summary>
		''' <returns> true </returns>
		Public Overridable Function stopCellEditing() As Boolean Implements CellEditor.stopCellEditing
			fireEditingStopped()
			Return True
		End Function

		''' <summary>
		''' Calls <code>fireEditingCanceled</code>.
		''' </summary>
		Public Overridable Sub cancelCellEditing() Implements CellEditor.cancelCellEditing
			fireEditingCanceled()
		End Sub

		''' <summary>
		''' Adds a <code>CellEditorListener</code> to the listener list. </summary>
		''' <param name="l">  the new listener to be added </param>
		Public Overridable Sub addCellEditorListener(ByVal l As CellEditorListener) Implements CellEditor.addCellEditorListener
			listenerList.add(GetType(CellEditorListener), l)
		End Sub

		''' <summary>
		''' Removes a <code>CellEditorListener</code> from the listener list. </summary>
		''' <param name="l">  the listener to be removed </param>
		Public Overridable Sub removeCellEditorListener(ByVal l As CellEditorListener) Implements CellEditor.removeCellEditorListener
			listenerList.remove(GetType(CellEditorListener), l)
		End Sub

		''' <summary>
		''' Returns an array of all the <code>CellEditorListener</code>s added
		''' to this AbstractCellEditor with addCellEditorListener().
		''' </summary>
		''' <returns> all of the <code>CellEditorListener</code>s added or an empty
		'''         array if no listeners have been added
		''' @since 1.4 </returns>
		Public Overridable Property cellEditorListeners As CellEditorListener()
			Get
				Return listenerList.getListeners(GetType(CellEditorListener))
			End Get
		End Property

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.  The event instance
		''' is created lazily.
		''' </summary>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireEditingStopped()
			' Guaranteed to return a non-null array
			Dim listeners As Object() = listenerList.listenerList
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = listeners.Length-2 To 0 Step -2
				If listeners(i) Is GetType(CellEditorListener) Then
					' Lazily create the event:
					If changeEvent Is Nothing Then changeEvent = New ChangeEvent(Me)
					CType(listeners(i+1), CellEditorListener).editingStopped(changeEvent)
				End If
			Next i
		End Sub

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.  The event instance
		''' is created lazily.
		''' </summary>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireEditingCanceled()
			' Guaranteed to return a non-null array
			Dim listeners As Object() = listenerList.listenerList
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = listeners.Length-2 To 0 Step -2
				If listeners(i) Is GetType(CellEditorListener) Then
					' Lazily create the event:
					If changeEvent Is Nothing Then changeEvent = New ChangeEvent(Me)
					CType(listeners(i+1), CellEditorListener).editingCanceled(changeEvent)
				End If
			Next i
		End Sub
	End Class

End Namespace