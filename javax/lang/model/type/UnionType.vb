Imports System.Collections.Generic

'
' * Copyright (c) 2010, 2011, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.lang.model.type


	''' <summary>
	''' Represents a union type.
	''' 
	''' As of the {@link javax.lang.model.SourceVersion#RELEASE_7
	''' RELEASE_7} source version, union types can appear as the type
	''' of a multi-catch exception parameter.
	''' 
	''' @since 1.7
	''' </summary>
	Public Interface UnionType
		Inherits TypeMirror

		''' <summary>
		''' Return the alternatives comprising this union type.
		''' </summary>
		''' <returns> the alternatives comprising this union type. </returns>
		ReadOnly Property alternatives As IList(Of TypeMirror)
	End Interface

End Namespace