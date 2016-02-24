'
' * Copyright (c) 1998, 2000, Oracle and/or its affiliates. All rights reserved.
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

Namespace org.omg.CORBA.DynAnyPackage

	''' <summary>
	''' @author unattributed
	''' 
	''' Dynamic Any insert operations raise the <code>InvalidValue</code>
	''' exception if the value inserted is not consistent with the type
	''' of the accessed component in the <code>DynAny</code> object.
	''' </summary>
	Public NotInheritable Class InvalidValue
		Inherits org.omg.CORBA.UserException

		''' <summary>
		''' Constructs an <code>InvalidValue</code> object.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs an <code>InvalidValue</code> object. </summary>
		''' <param name="reason">  a <code>String</code> giving more information
		''' regarding the exception. </param>
		Public Sub New(ByVal reason As String)
			MyBase.New(reason)
		End Sub
	End Class

End Namespace