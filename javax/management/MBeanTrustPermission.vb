'
' * Copyright (c) 2002, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.management


	''' <summary>
	''' This permission represents "trust" in a signer or codebase.
	''' <p>
	''' MBeanTrustPermission contains a target name but no actions list.
	''' A single target name, "register", is defined for this permission.
	''' The target "*" is also allowed, permitting "register" and any future
	''' targets that may be defined.
	''' Only the null value or the empty string are allowed for the action
	''' to allow the policy object to create the permissions specified in
	''' the policy file.
	''' <p>
	''' If a signer, or codesource is granted this permission, then it is
	''' considered a trusted source for MBeans. Only MBeans from trusted
	''' sources may be registered in the MBeanServer.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class MBeanTrustPermission
		Inherits java.security.BasicPermission

		Private Const serialVersionUID As Long = -2952178077029018140L

		''' <summary>
		''' <p>Create a new MBeanTrustPermission with the given name.</p>
		'''    <p>This constructor is equivalent to
		'''    <code>MBeanTrustPermission(name,null)</code>.</p> </summary>
		'''    <param name="name"> the name of the permission. It must be
		'''    "register" or "*" for this permission.
		''' </param>
		''' <exception cref="NullPointerException"> if <code>name</code> is <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>name</code> is neither
		''' "register" nor "*". </exception>
		Public Sub New(ByVal name As String)
			Me.New(name, Nothing)
		End Sub

		''' <summary>
		''' <p>Create a new MBeanTrustPermission with the given name.</p> </summary>
		'''    <param name="name"> the name of the permission. It must be
		'''    "register" or "*" for this permission. </param>
		'''    <param name="actions"> the actions for the permission.  It must be
		'''    null or <code>""</code>.
		''' </param>
		''' <exception cref="NullPointerException"> if <code>name</code> is <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>name</code> is neither
		''' "register" nor "*"; or if <code>actions</code> is a non-null
		''' non-empty string. </exception>
		Public Sub New(ByVal name As String, ByVal actions As String)
			MyBase.New(name, actions)
			validate(name,actions)
		End Sub

		Private Shared Sub validate(ByVal name As String, ByVal actions As String)
			' Check that actions is a null empty string 
			If actions IsNot Nothing AndAlso actions.Length > 0 Then Throw New System.ArgumentException("MBeanTrustPermission actions must be null: " & actions)

			If (Not name.Equals("register")) AndAlso (Not name.Equals("*")) Then Throw New System.ArgumentException("MBeanTrustPermission: Unknown target name " & "[" & name & "]")
		End Sub

		Private Sub readObject(ByVal [in] As java.io.ObjectInputStream)

			' Reading private fields of base class
			[in].defaultReadObject()
			Try
				validate(MyBase.name,MyBase.actions)
			Catch e As System.ArgumentException
				Throw New java.io.InvalidObjectException(e.Message)
			End Try
		End Sub
	End Class

End Namespace