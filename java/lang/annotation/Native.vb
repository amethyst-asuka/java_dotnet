'
' * Copyright (c) 2012, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang.annotation


	''' <summary>
	''' Indicates that a field defining a constant value may be referenced
	''' from native code.
	''' 
	''' The annotation may be used as a hint by tools that generate native
	''' header files to determine whether a header file is required, and
	''' if so, what declarations it should contain.
	''' 
	''' @since 1.8
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(AttributeTargets.Field, AllowMultiple := False, Inherited := False> _
	Public Class Native
		Inherits System.Attribute

	End Class

End Namespace