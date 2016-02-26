'
' * Copyright (c) 2006, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.bind


	''' <summary>
	''' <seealso cref="PrivilegedAction"/> that gets the system property value.
	''' @author Kohsuke Kawaguchi
	''' </summary>
	Friend NotInheritable Class GetPropertyAction
		Implements java.security.PrivilegedAction(Of String)

		Private ReadOnly propertyName As String

		Public Sub New(ByVal propertyName As String)
			Me.propertyName = propertyName
		End Sub

		Public Function run() As String
			Return System.getProperty(propertyName)
		End Function
	End Class

End Namespace