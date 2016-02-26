'
' * Copyright (c) 2003, 2007, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.management.remote


	''' <summary>
	''' <p>Permission required by an authentication identity to perform
	''' operations on behalf of an authorization identity.</p>
	''' 
	''' <p>A SubjectDelegationPermission contains a name (also referred
	''' to as a "target name") but no actions list; you either have the
	''' named permission or you don't.</p>
	''' 
	''' <p>The target name is the name of the authorization principal
	''' classname followed by a period and the authorization principal
	''' name, that is
	''' <code>"<em>PrincipalClassName</em>.<em>PrincipalName</em>"</code>.</p>
	''' 
	''' <p>An asterisk may appear by itself, or if immediately preceded
	''' by a "." may appear at the end of the target name, to signify a
	''' wildcard match.</p>
	''' 
	''' <p>For example, "*", "javax.management.remote.JMXPrincipal.*" and
	''' "javax.management.remote.JMXPrincipal.delegate" are valid target
	''' names. The first one denotes any principal name from any principal
	''' class, the second one denotes any principal name of the concrete
	''' principal class <code>javax.management.remote.JMXPrincipal</code>
	''' and the third one denotes a concrete principal name
	''' <code>delegate</code> of the concrete principal class
	''' <code>javax.management.remote.JMXPrincipal</code>.</p>
	''' 
	''' @since 1.5
	''' </summary>
	Public NotInheritable Class SubjectDelegationPermission
		Inherits java.security.BasicPermission

		Private Const serialVersionUID As Long = 1481618113008682343L

		''' <summary>
		''' Creates a new SubjectDelegationPermission with the specified name.
		''' The name is the symbolic name of the SubjectDelegationPermission.
		''' </summary>
		''' <param name="name"> the name of the SubjectDelegationPermission
		''' </param>
		''' <exception cref="NullPointerException"> if <code>name</code> is
		''' <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>name</code> is empty. </exception>
		Public Sub New(ByVal name As String)
			MyBase.New(name)
		End Sub

		''' <summary>
		''' Creates a new SubjectDelegationPermission object with the
		''' specified name.  The name is the symbolic name of the
		''' SubjectDelegationPermission, and the actions String is
		''' currently unused and must be null.
		''' </summary>
		''' <param name="name"> the name of the SubjectDelegationPermission </param>
		''' <param name="actions"> must be null.
		''' </param>
		''' <exception cref="NullPointerException"> if <code>name</code> is
		''' <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>name</code> is empty
		''' or <code>actions</code> is not null. </exception>
		Public Sub New(ByVal name As String, ByVal actions As String)
			MyBase.New(name, actions)

			If actions IsNot Nothing Then Throw New System.ArgumentException("Non-null actions")
		End Sub
	End Class

End Namespace