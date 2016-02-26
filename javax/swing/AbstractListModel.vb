Imports System
Imports javax.swing.event

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
	''' The abstract definition for the data model that provides
	''' a <code>List</code> with its contents.
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' </summary>
	''' @param <E> the type of the elements of this model
	''' 
	''' @author Hans Muller </param>
	<Serializable> _
	Public MustInherit Class AbstractListModel(Of E)
		Implements ListModel(Of E)

		Protected Friend listenerList As New EventListenerList


		''' <summary>
		''' Adds a listener to the list that's notified each time a change
		''' to the data model occurs.
		''' </summary>
		''' <param name="l"> the <code>ListDataListener</code> to be added </param>
		Public Overridable Sub addListDataListener(ByVal l As ListDataListener)
			listenerList.add(GetType(ListDataListener), l)
		End Sub


		''' <summary>
		''' Removes a listener from the list that's notified each time a
		''' change to the data model occurs.
		''' </summary>
		''' <param name="l"> the <code>ListDataListener</code> to be removed </param>
		Public Overridable Sub removeListDataListener(ByVal l As ListDataListener)
			listenerList.remove(GetType(ListDataListener), l)
		End Sub


		''' <summary>
		''' Returns an array of all the list data listeners
		''' registered on this <code>AbstractListModel</code>.
		''' </summary>
		''' <returns> all of this model's <code>ListDataListener</code>s,
		'''         or an empty array if no list data listeners
		'''         are currently registered
		''' </returns>
		''' <seealso cref= #addListDataListener </seealso>
		''' <seealso cref= #removeListDataListener
		''' 
		''' @since 1.4 </seealso>
		Public Overridable Property listDataListeners As ListDataListener()
			Get
				Return listenerList.getListeners(GetType(ListDataListener))
			End Get
		End Property


		''' <summary>
		''' <code>AbstractListModel</code> subclasses must call this method
		''' <b>after</b>
		''' one or more elements of the list change.  The changed elements
		''' are specified by the closed interval index0, index1 -- the endpoints
		''' are included.  Note that
		''' index0 need not be less than or equal to index1.
		''' </summary>
		''' <param name="source"> the <code>ListModel</code> that changed, typically "this" </param>
		''' <param name="index0"> one end of the new interval </param>
		''' <param name="index1"> the other end of the new interval </param>
		''' <seealso cref= EventListenerList </seealso>
		''' <seealso cref= DefaultListModel </seealso>
		Protected Friend Overridable Sub fireContentsChanged(ByVal source As Object, ByVal index0 As Integer, ByVal index1 As Integer)
			Dim ___listeners As Object() = listenerList.listenerList
			Dim e As ListDataEvent = Nothing

			For i As Integer = ___listeners.Length - 2 To 0 Step -2
				If ___listeners(i) Is GetType(ListDataListener) Then
					If e Is Nothing Then e = New ListDataEvent(source, ListDataEvent.CONTENTS_CHANGED, index0, index1)
					CType(___listeners(i+1), ListDataListener).contentsChanged(e)
				End If
			Next i
		End Sub


		''' <summary>
		''' <code>AbstractListModel</code> subclasses must call this method
		''' <b>after</b>
		''' one or more elements are added to the model.  The new elements
		''' are specified by a closed interval index0, index1 -- the enpoints
		''' are included.  Note that
		''' index0 need not be less than or equal to index1.
		''' </summary>
		''' <param name="source"> the <code>ListModel</code> that changed, typically "this" </param>
		''' <param name="index0"> one end of the new interval </param>
		''' <param name="index1"> the other end of the new interval </param>
		''' <seealso cref= EventListenerList </seealso>
		''' <seealso cref= DefaultListModel </seealso>
		Protected Friend Overridable Sub fireIntervalAdded(ByVal source As Object, ByVal index0 As Integer, ByVal index1 As Integer)
			Dim ___listeners As Object() = listenerList.listenerList
			Dim e As ListDataEvent = Nothing

			For i As Integer = ___listeners.Length - 2 To 0 Step -2
				If ___listeners(i) Is GetType(ListDataListener) Then
					If e Is Nothing Then e = New ListDataEvent(source, ListDataEvent.INTERVAL_ADDED, index0, index1)
					CType(___listeners(i+1), ListDataListener).intervalAdded(e)
				End If
			Next i
		End Sub


		''' <summary>
		''' <code>AbstractListModel</code> subclasses must call this method
		''' <b>after</b> one or more elements are removed from the model.
		''' <code>index0</code> and <code>index1</code> are the end points
		''' of the interval that's been removed.  Note that <code>index0</code>
		''' need not be less than or equal to <code>index1</code>.
		''' </summary>
		''' <param name="source"> the <code>ListModel</code> that changed, typically "this" </param>
		''' <param name="index0"> one end of the removed interval,
		'''               including <code>index0</code> </param>
		''' <param name="index1"> the other end of the removed interval,
		'''               including <code>index1</code> </param>
		''' <seealso cref= EventListenerList </seealso>
		''' <seealso cref= DefaultListModel </seealso>
		Protected Friend Overridable Sub fireIntervalRemoved(ByVal source As Object, ByVal index0 As Integer, ByVal index1 As Integer)
			Dim ___listeners As Object() = listenerList.listenerList
			Dim e As ListDataEvent = Nothing

			For i As Integer = ___listeners.Length - 2 To 0 Step -2
				If ___listeners(i) Is GetType(ListDataListener) Then
					If e Is Nothing Then e = New ListDataEvent(source, ListDataEvent.INTERVAL_REMOVED, index0, index1)
					CType(___listeners(i+1), ListDataListener).intervalRemoved(e)
				End If
			Next i
		End Sub

		''' <summary>
		''' Returns an array of all the objects currently registered as
		''' <code><em>Foo</em>Listener</code>s
		''' upon this model.
		''' <code><em>Foo</em>Listener</code>s
		''' are registered using the <code>add<em>Foo</em>Listener</code> method.
		''' <p>
		''' You can specify the <code>listenerType</code> argument
		''' with a class literal, such as <code><em>Foo</em>Listener.class</code>.
		''' For example, you can query a list model
		''' <code>m</code>
		''' for its list data listeners
		''' with the following code:
		''' 
		''' <pre>ListDataListener[] ldls = (ListDataListener[])(m.getListeners(ListDataListener.class));</pre>
		''' 
		''' If no such listeners exist,
		''' this method returns an empty array.
		''' </summary>
		''' <param name="listenerType">  the type of listeners requested;
		'''          this parameter should specify an interface
		'''          that descends from <code>java.util.EventListener</code> </param>
		''' <returns> an array of all objects registered as
		'''          <code><em>Foo</em>Listener</code>s
		'''          on this model,
		'''          or an empty array if no such
		'''          listeners have been added </returns>
		''' <exception cref="ClassCastException"> if <code>listenerType</code> doesn't
		'''          specify a class or interface that implements
		'''          <code>java.util.EventListener</code>
		''' </exception>
		''' <seealso cref= #getListDataListeners
		''' 
		''' @since 1.3 </seealso>
		Public Overridable Function getListeners(Of T As java.util.EventListener)(ByVal listenerType As Type) As T()
			Return listenerList.getListeners(listenerType)
		End Function
	End Class

End Namespace