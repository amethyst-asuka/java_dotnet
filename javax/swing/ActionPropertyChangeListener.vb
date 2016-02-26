Imports System

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
	''' A package-private PropertyChangeListener which listens for
	''' property changes on an Action and updates the properties
	''' of an ActionEvent source.
	''' <p>
	''' Subclasses must override the actionPropertyChanged method,
	''' which is invoked from the propertyChange method as long as
	''' the target is still valid.
	''' </p>
	''' <p>
	''' WARNING WARNING WARNING WARNING WARNING WARNING:<br>
	''' Do NOT create an annonymous inner class that extends this!  If you do
	''' a strong reference will be held to the containing class, which in most
	''' cases defeats the purpose of this class.
	''' </summary>
	''' <param name="T"> the type of JComponent the underlying Action is attached to
	''' 
	''' @author Georges Saab </param>
	''' <seealso cref= AbstractButton </seealso>
	<Serializable> _
	Friend MustInherit Class ActionPropertyChangeListener(Of T As JComponent)
		Implements java.beans.PropertyChangeListener

		Private Shared queue As ReferenceQueue(Of JComponent)

		' WeakReference's aren't serializable.
		<NonSerialized> _
		Private target As OwnedWeakReference(Of T)
		' The Component's that reference an Action do so through a strong
		' reference, so that there is no need to check for serialized.
		Private action As Action

		Private Property Shared queue As ReferenceQueue(Of JComponent)
			Get
				SyncLock GetType(ActionPropertyChangeListener)
					If queue Is Nothing Then queue = New ReferenceQueue(Of JComponent)
				End SyncLock
				Return queue
			End Get
		End Property

		Public Sub New(ByVal c As T, ByVal a As Action)
			MyBase.New()
			target = c
			Me.action = a
		End Sub

		''' <summary>
		''' PropertyChangeListener method.  If the target has been gc'ed this
		''' will remove the <code>PropertyChangeListener</code> from the Action,
		''' otherwise this will invoke actionPropertyChanged.
		''' </summary>
		Public Sub propertyChange(ByVal e As java.beans.PropertyChangeEvent)
			Dim ___target As T = target
			If ___target Is Nothing Then
				action.removePropertyChangeListener(Me)
			Else
				actionPropertyChanged(___target, action, e)
			End If
		End Sub

		''' <summary>
		''' Invoked when a property changes on the Action and the target
		''' still exists.
		''' </summary>
		Protected Friend MustOverride Sub actionPropertyChanged(ByVal target As T, ByVal action As Action, ByVal e As java.beans.PropertyChangeEvent)

		Private Property target As T
			Set(ByVal c As T)
				Dim ___queue As ReferenceQueue(Of JComponent) = queue
				' Check to see whether any old buttons have
				' been enqueued for GC.  If so, look up their
				' PCL instance and remove it from its Action.
	'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim r As OwnedWeakReference(Of ?)
				r = CType(___queue.poll(), OwnedWeakReference)
				Do While r IsNot Nothing
	'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim oldPCL As ActionPropertyChangeListener(Of ?) = r.owner
					Dim oldAction As Action = oldPCL.action
					If oldAction IsNot Nothing Then oldAction.removePropertyChangeListener(oldPCL)
					r = CType(___queue.poll(), OwnedWeakReference)
				Loop
				Me.target = New OwnedWeakReference(Of T)(c, ___queue, Me)
			End Set
			Get
				If target Is Nothing Then Return Nothing
				Return Me.target.get()
			End Get
		End Property


		Public Overridable Property action As Action
			Get
				  Return action
			End Get
		End Property

		Private Sub writeObject(ByVal s As ObjectOutputStream)
			s.defaultWriteObject()
			s.writeObject(target)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Sub readObject(ByVal s As ObjectInputStream)
			s.defaultReadObject()
			Dim ___target As T = CType(s.readObject(), T)
			If ___target IsNot Nothing Then target = ___target
		End Sub


		Private Class OwnedWeakReference(Of U As JComponent)
			Inherits WeakReference(Of U)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Private owner As ActionPropertyChangeListener(Of ?)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1, T2)(ByVal target As U, ByVal queue As ReferenceQueue(Of T1), ByVal owner As ActionPropertyChangeListener(Of T2))
				MyBase.New(target, queue)
				Me.owner = owner
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public Overridable Property owner As ActionPropertyChangeListener(Of ?)
				Get
					Return owner
				End Get
			End Property
		End Class
	End Class

End Namespace