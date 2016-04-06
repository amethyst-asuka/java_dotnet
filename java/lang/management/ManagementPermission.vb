'
' * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang.management

	''' <summary>
	''' The permission which the SecurityManager will check when code
	''' that is running with a SecurityManager calls methods defined
	''' in the management interface for the Java platform.
	''' <P>
	''' The following table
	''' provides a summary description of what the permission allows,
	''' and discusses the risks of granting code the permission.
	''' 
	''' <table border=1 cellpadding=5 summary="Table shows permission target name, what the permission allows, and associated risks">
	''' <tr>
	''' <th>Permission Target Name</th>
	''' <th>What the Permission Allows</th>
	''' <th>Risks of Allowing this Permission</th>
	''' </tr>
	''' 
	''' <tr>
	'''   <td>control</td>
	'''   <td>Ability to control the runtime characteristics of the Java virtual
	'''       machine, for example, enabling and disabling the verbose output for
	'''       the class loading or memory system, setting the threshold of a memory
	'''       pool, and enabling and disabling the thread contention monitoring
	'''       support. Some actions controlled by this permission can disclose
	'''       information about the running application, like the -verbose:class
	'''       flag.
	'''   </td>
	'''   <td>This allows an attacker to control the runtime characteristics
	'''       of the Java virtual machine and cause the system to misbehave. An
	'''       attacker can also access some information related to the running
	'''       application.
	'''   </td>
	''' </tr>
	''' <tr>
	'''   <td>monitor</td>
	'''   <td>Ability to retrieve runtime information about
	'''       the Java virtual machine such as thread
	'''       stack trace, a list of all loaded class names, and input arguments
	'''       to the Java virtual machine.</td>
	'''   <td>This allows malicious code to monitor runtime information and
	'''       uncover vulnerabilities.</td>
	''' </tr>
	''' 
	''' </table>
	''' 
	''' <p>
	''' Programmers do not normally create ManagementPermission objects directly.
	''' Instead they are created by the security policy code based on reading
	''' the security policy file.
	''' 
	''' @author  Mandy Chung
	''' @since   1.5
	''' </summary>
	''' <seealso cref= java.security.BasicPermission </seealso>
	''' <seealso cref= java.security.Permission </seealso>
	''' <seealso cref= java.security.Permissions </seealso>
	''' <seealso cref= java.security.PermissionCollection </seealso>
	''' <seealso cref= java.lang.SecurityManager
	'''  </seealso>

	Public NotInheritable Class ManagementPermission
		Inherits java.security.BasicPermission

		Private Const serialVersionUID As Long = 1897496590799378737L

		''' <summary>
		''' Constructs a ManagementPermission with the specified name.
		''' </summary>
		''' <param name="name"> Permission name. Must be either "monitor" or "control".
		''' </param>
		''' <exception cref="NullPointerException"> if <code>name</code> is <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>name</code> is empty or invalid. </exception>
		Public Sub New(  name As String)
			MyBase.New(name)
			If (Not name.Equals("control")) AndAlso (Not name.Equals("monitor")) Then Throw New IllegalArgumentException("name: " & name)
		End Sub

		''' <summary>
		''' Constructs a new ManagementPermission object.
		''' </summary>
		''' <param name="name"> Permission name. Must be either "monitor" or "control". </param>
		''' <param name="actions"> Must be either null or the empty string.
		''' </param>
		''' <exception cref="NullPointerException"> if <code>name</code> is <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>name</code> is empty or
		''' if arguments are invalid. </exception>
		Public Sub New(  name As String,   actions As String)
			MyBase.New(name)
			If (Not name.Equals("control")) AndAlso (Not name.Equals("monitor")) Then Throw New IllegalArgumentException("name: " & name)
			If actions IsNot Nothing AndAlso actions.length() > 0 Then Throw New IllegalArgumentException("actions: " & actions)
		End Sub
	End Class

End Namespace