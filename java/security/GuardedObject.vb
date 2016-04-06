Imports System

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

Namespace java.security

	''' <summary>
	''' A GuardedObject is an object that is used to protect access to
	''' another object.
	''' 
	''' <p>A GuardedObject encapsulates a target object and a Guard object,
	''' such that access to the target object is possible
	''' only if the Guard object allows it.
	''' Once an object is encapsulated by a GuardedObject,
	''' access to that object is controlled by the {@code getObject}
	''' method, which invokes the
	''' {@code checkGuard} method on the Guard object that is
	''' guarding access. If access is not allowed,
	''' an exception is thrown.
	''' </summary>
	''' <seealso cref= Guard </seealso>
	''' <seealso cref= Permission
	''' 
	''' @author Roland Schemers
	''' @author Li Gong </seealso>

	<Serializable> _
	Public Class GuardedObject

		Private Const serialVersionUID As Long = -5240450096227834308L

		Private object_Renamed As Object ' the object we are guarding
		Private guard As Guard ' the guard

		''' <summary>
		''' Constructs a GuardedObject using the specified object and guard.
		''' If the Guard object is null, then no restrictions will
		''' be placed on who can access the object.
		''' </summary>
		''' <param name="object"> the object to be guarded.
		''' </param>
		''' <param name="guard"> the Guard object that guards access to the object. </param>

		Public Sub New(  [object] As Object,   guard As Guard)
			Me.guard = guard
			Me.object_Renamed = object_Renamed
		End Sub

		''' <summary>
		''' Retrieves the guarded object, or throws an exception if access
		''' to the guarded object is denied by the guard.
		''' </summary>
		''' <returns> the guarded object.
		''' </returns>
		''' <exception cref="SecurityException"> if access to the guarded object is
		''' denied. </exception>
		Public Overridable Property [object] As Object
			Get
				If guard IsNot Nothing Then guard.checkGuard(object_Renamed)
    
				Return object_Renamed
			End Get
		End Property

		''' <summary>
		''' Writes this object out to a stream (i.e., serializes it).
		''' We check the guard if there is one.
		''' </summary>
		Private Sub writeObject(  oos As java.io.ObjectOutputStream)
			If guard IsNot Nothing Then guard.checkGuard(object_Renamed)

			oos.defaultWriteObject()
		End Sub
	End Class

End Namespace