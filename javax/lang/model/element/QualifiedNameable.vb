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
	''' A mixin interface for an element that has a qualified name.
	''' 
	''' @author Joseph D. Darcy
	''' @since 1.7
	''' </summary>
	Public Interface QualifiedNameable
		Inherits Element

		''' <summary>
		''' Returns the fully qualified name of an element.
		''' </summary>
		''' <returns> the fully qualified name of an element </returns>
		ReadOnly Property qualifiedName As Name
	End Interface

End Namespace