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

Namespace java.rmi.activation


	''' <summary>
	''' The identifier for a registered activation group serves several
	''' purposes: <ul>
	''' <li>identifies the group uniquely within the activation system, and
	''' <li>contains a reference to the group's activation system so that the
	''' group can contact its activation system when necessary.</ul><p>
	''' 
	''' The <code>ActivationGroupID</code> is returned from the call to
	''' <code>ActivationSystem.registerGroup</code> and is used to identify
	''' the group within the activation system. This group id is passed
	''' as one of the arguments to the activation group's special constructor
	''' when an activation group is created/recreated.
	''' 
	''' @author      Ann Wollrath </summary>
	''' <seealso cref=         ActivationGroup </seealso>
	''' <seealso cref=         ActivationGroupDesc
	''' @since       1.2 </seealso>
	<Serializable> _
	Public Class ActivationGroupID
		''' <summary>
		''' @serial The group's activation system.
		''' </summary>
		Private system_Renamed As ActivationSystem

		''' <summary>
		''' @serial The group's unique id.
		''' </summary>
		Private uid As New java.rmi.server.UID

		''' <summary>
		''' indicate compatibility with the Java 2 SDK v1.2 version of class </summary>
		Private Const serialVersionUID As Long = -1648432278909740833L

		''' <summary>
		''' Constructs a unique group id.
		''' </summary>
		''' <param name="system"> the group's activation system </param>
		''' <exception cref="UnsupportedOperationException"> if and only if activation is
		'''         not supported by this implementation
		''' @since 1.2 </exception>
		Public Sub New(ByVal system_Renamed As ActivationSystem)
			Me.system_Renamed = system_Renamed
		End Sub

		''' <summary>
		''' Returns the group's activation system. </summary>
		''' <returns> the group's activation system
		''' @since 1.2 </returns>
		Public Overridable Property system As ActivationSystem
			Get
				Return system_Renamed
			End Get
		End Property

		''' <summary>
		''' Returns a hashcode for the group's identifier.  Two group
		''' identifiers that refer to the same remote group will have the
		''' same hash code.
		''' </summary>
		''' <seealso cref= java.util.Hashtable
		''' @since 1.2 </seealso>
		Public Overrides Function GetHashCode() As Integer
			Return uid.GetHashCode()
		End Function

		''' <summary>
		''' Compares two group identifiers for content equality.
		''' Returns true if both of the following conditions are true:
		''' 1) the unique identifiers are equivalent (by content), and
		''' 2) the activation system specified in each
		'''    refers to the same remote object.
		''' </summary>
		''' <param name="obj">     the Object to compare with </param>
		''' <returns>  true if these Objects are equal; false otherwise. </returns>
		''' <seealso cref=             java.util.Hashtable
		''' @since 1.2 </seealso>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Me Is obj Then
				Return True
			ElseIf TypeOf obj Is ActivationGroupID Then
				Dim id As ActivationGroupID = CType(obj, ActivationGroupID)
				Return (uid.Equals(id.uid) AndAlso system_Renamed.Equals(id.system_Renamed))
			Else
				Return False
			End If
		End Function
	End Class

End Namespace