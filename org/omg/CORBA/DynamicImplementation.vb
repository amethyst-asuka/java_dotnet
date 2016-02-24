Imports System

'
' * Copyright (c) 1997, 2004, Oracle and/or its affiliates. All rights reserved.
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

Namespace org.omg.CORBA


	''' @deprecated org.omg.CORBA.DynamicImplementation 
	<Obsolete("org.omg.CORBA.DynamicImplementation")> _
	Public Class DynamicImplementation
		Inherits org.omg.CORBA.portable.ObjectImpl
		''' @deprecated Deprecated by Portable Object Adapter 
		<Obsolete("Deprecated by Portable Object Adapter")> _
		Public Overridable Sub invoke(ByVal request As ServerRequest)
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Sub

		Public Overrides Function _ids() As String()
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function
	End Class

End Namespace