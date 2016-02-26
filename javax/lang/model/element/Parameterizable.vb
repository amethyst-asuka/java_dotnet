Imports System.Collections.Generic

'
' * Copyright (c) 2009, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.lang.model.element


	''' <summary>
	''' A mixin interface for an element that has type parameters.
	''' 
	''' @author Joseph D. Darcy
	''' @since 1.7
	''' </summary>
	Public Interface Parameterizable
		Inherits Element

		''' <summary>
		''' Returns the formal type parameters of the type element in
		''' declaration order.
		''' </summary>
		''' <returns> the formal type parameters, or an empty list
		''' if there are none </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		ReadOnly Property typeParameters As IList(Of ? As TypeParameterElement)
	End Interface

End Namespace