'
' * Copyright (c) 2005, 2011, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.ws


	''' <summary>
	''' This class defines web service permissions.
	''' <p>
	''' Web service Permissions are identified by name (also referred to as
	''' a "target name") alone. There are no actions associated
	''' with them.
	''' <p>
	''' The following permission target name is defined:
	''' <p>
	''' <dl>
	'''   <dt>publishEndpoint
	''' </dl>
	''' <p>
	''' The <code>publishEndpoint</code> permission allows publishing a
	''' web service endpoint using the <code>publish</code> methods
	''' defined by the <code>javax.xml.ws.Endpoint</code> class.
	''' <p>
	''' Granting <code>publishEndpoint</code> allows the application to be
	''' exposed as a network service. Depending on the security of the runtime and
	''' the security of the application, this may introduce a security hole that
	''' is remotely exploitable.
	''' </summary>
	''' <seealso cref= javax.xml.ws.Endpoint </seealso>
	''' <seealso cref= java.security.BasicPermission </seealso>
	''' <seealso cref= java.security.Permission </seealso>
	''' <seealso cref= java.security.Permissions </seealso>
	''' <seealso cref= java.lang.SecurityManager </seealso>
	''' <seealso cref= java.net.SocketPermission </seealso>
	Public NotInheritable Class WebServicePermission
		Inherits java.security.BasicPermission

		Private Const serialVersionUID As Long = -146474640053770988L

		''' <summary>
		''' Creates a new permission with the specified name.
		''' </summary>
		''' <param name="name"> the name of the <code>WebServicePermission</code> </param>
		Public Sub New(ByVal name As String)
			MyBase.New(name)
		End Sub

		''' <summary>
		''' Creates a new permission with the specified name and actions.
		''' 
		''' The <code>actions</code> parameter is currently unused and
		''' it should be <code>null</code>.
		''' </summary>
		''' <param name="name"> the name of the <code>WebServicePermission</code> </param>
		''' <param name="actions"> should be <code>null</code> </param>
		Public Sub New(ByVal name As String, ByVal actions As String)
			MyBase.New(name, actions)
		End Sub

	End Class

End Namespace