Imports System
Imports javax.swing.event

'
' * Copyright (c) 1997, 2012, Oracle and/or its affiliates. All rights reserved.
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
	''' @author Dave Moore
	''' </summary>

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<Serializable> _
	Friend Class AncestorNotifier
		Implements ComponentListener, java.beans.PropertyChangeListener

		<NonSerialized> _
		Friend firstInvisibleAncestor As java.awt.Component
		Friend listenerList As New EventListenerList
		Friend root As JComponent

		Friend Sub New(ByVal root As JComponent)
			Me.root = root
			addListeners(root, True)
		End Sub

		Friend Overridable Sub addAncestorListener(ByVal l As AncestorListener)
			listenerList.add(GetType(AncestorListener), l)
		End Sub

		Friend Overridable Sub removeAncestorListener(ByVal l As AncestorListener)
			listenerList.remove(GetType(AncestorListener), l)
		End Sub

		Friend Overridable Property ancestorListeners As AncestorListener()
			Get
				Return listenerList.getListeners(GetType(AncestorListener))
			End Get
		End Property

		''' <summary>
		''' Notify all listeners that have registered interest for
		''' notification on this event type.  The event instance
		''' is lazily created using the parameters passed into
		''' the fire method. </summary>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireAncestorAdded(ByVal source As JComponent, ByVal id As Integer, ByVal ancestor As java.awt.Container, ByVal ancestorParent As java.awt.Container)
			' Guaranteed to return a non-null array
			Dim listeners As Object() = listenerList.listenerList
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = listeners.Length-2 To 0 Step -2
				If listeners(i) Is GetType(AncestorListener) Then
					' Lazily create the event:
					Dim ___ancestorEvent As New AncestorEvent(source, id, ancestor, ancestorParent)
					CType(listeners(i+1), AncestorListener).ancestorAdded(___ancestorEvent)
				End If
			Next i
		End Sub

		''' <summary>
		''' Notify all listeners that have registered interest for
		''' notification on this event type.  The event instance
		''' is lazily created using the parameters passed into
		''' the fire method. </summary>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireAncestorRemoved(ByVal source As JComponent, ByVal id As Integer, ByVal ancestor As java.awt.Container, ByVal ancestorParent As java.awt.Container)
			' Guaranteed to return a non-null array
			Dim listeners As Object() = listenerList.listenerList
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = listeners.Length-2 To 0 Step -2
				If listeners(i) Is GetType(AncestorListener) Then
					' Lazily create the event:
					Dim ___ancestorEvent As New AncestorEvent(source, id, ancestor, ancestorParent)
					CType(listeners(i+1), AncestorListener).ancestorRemoved(___ancestorEvent)
				End If
			Next i
		End Sub
		''' <summary>
		''' Notify all listeners that have registered interest for
		''' notification on this event type.  The event instance
		''' is lazily created using the parameters passed into
		''' the fire method. </summary>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireAncestorMoved(ByVal source As JComponent, ByVal id As Integer, ByVal ancestor As java.awt.Container, ByVal ancestorParent As java.awt.Container)
			' Guaranteed to return a non-null array
			Dim listeners As Object() = listenerList.listenerList
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = listeners.Length-2 To 0 Step -2
				If listeners(i) Is GetType(AncestorListener) Then
					' Lazily create the event:
					Dim ___ancestorEvent As New AncestorEvent(source, id, ancestor, ancestorParent)
					CType(listeners(i+1), AncestorListener).ancestorMoved(___ancestorEvent)
				End If
			Next i
		End Sub

		Friend Overridable Sub removeAllListeners()
			removeListeners(root)
		End Sub

		Friend Overridable Sub addListeners(ByVal ancestor As java.awt.Component, ByVal addToFirst As Boolean)
			Dim a As java.awt.Component

			firstInvisibleAncestor = Nothing
			a = ancestor
			Do While firstInvisibleAncestor Is Nothing
				If addToFirst OrElse a IsNot ancestor Then
					a.addComponentListener(Me)

					If TypeOf a Is JComponent Then
						Dim jAncestor As JComponent = CType(a, JComponent)

						jAncestor.addPropertyChangeListener(Me)
					End If
				End If
				If (Not a.visible) OrElse a.parent Is Nothing OrElse TypeOf a Is java.awt.Window Then firstInvisibleAncestor = a
				a = a.parent
			Loop
			If TypeOf firstInvisibleAncestor Is java.awt.Window AndAlso firstInvisibleAncestor.visible Then firstInvisibleAncestor = Nothing
		End Sub

		Friend Overridable Sub removeListeners(ByVal ancestor As java.awt.Component)
			Dim a As java.awt.Component
			a = ancestor
			Do While a IsNot Nothing
				a.removeComponentListener(Me)
				If TypeOf a Is JComponent Then
					Dim jAncestor As JComponent = CType(a, JComponent)
					jAncestor.removePropertyChangeListener(Me)
				End If
				If a Is firstInvisibleAncestor OrElse TypeOf a Is java.awt.Window Then Exit Do
				a = a.parent
			Loop
		End Sub

		Public Overridable Sub componentResized(ByVal e As ComponentEvent)
		End Sub

		Public Overridable Sub componentMoved(ByVal e As ComponentEvent)
			Dim source As java.awt.Component = e.component

			fireAncestorMoved(root, AncestorEvent.ANCESTOR_MOVED, CType(source, java.awt.Container), source.parent)
		End Sub

		Public Overridable Sub componentShown(ByVal e As ComponentEvent)
			Dim ancestor As java.awt.Component = e.component

			If ancestor Is firstInvisibleAncestor Then
				addListeners(ancestor, False)
				If firstInvisibleAncestor Is Nothing Then fireAncestorAdded(root, AncestorEvent.ANCESTOR_ADDED, CType(ancestor, java.awt.Container), ancestor.parent)
			End If
		End Sub

		Public Overridable Sub componentHidden(ByVal e As ComponentEvent)
			Dim ancestor As java.awt.Component = e.component
			Dim needsNotify As Boolean = firstInvisibleAncestor Is Nothing

			If Not(TypeOf ancestor Is java.awt.Window) Then removeListeners(ancestor.parent)
			firstInvisibleAncestor = ancestor
			If needsNotify Then fireAncestorRemoved(root, AncestorEvent.ANCESTOR_REMOVED, CType(ancestor, java.awt.Container), ancestor.parent)
		End Sub

		Public Overridable Sub propertyChange(ByVal evt As java.beans.PropertyChangeEvent)
			Dim s As String = evt.propertyName

			If s IsNot Nothing AndAlso (s.Equals("parent") OrElse s.Equals("ancestor")) Then
				Dim component As JComponent = CType(evt.source, JComponent)

				If evt.newValue IsNot Nothing Then
					If component Is firstInvisibleAncestor Then
						addListeners(component, False)
						If firstInvisibleAncestor Is Nothing Then fireAncestorAdded(root, AncestorEvent.ANCESTOR_ADDED, component, component.parent)
					End If
				Else
					Dim needsNotify As Boolean = firstInvisibleAncestor Is Nothing
					Dim oldParent As java.awt.Container = CType(evt.oldValue, java.awt.Container)

					removeListeners(oldParent)
					firstInvisibleAncestor = component
					If needsNotify Then fireAncestorRemoved(root, AncestorEvent.ANCESTOR_REMOVED, component, oldParent)
				End If
			End If
		End Sub
	End Class

End Namespace