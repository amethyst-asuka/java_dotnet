'
' * Copyright (c) 2012, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang.reflect

	''' <summary>
	''' {@code AnnotatedTypeVariable} represents the potentially annotated use of a
	''' type variable, whose declaration may have bounds which themselves represent
	''' annotated uses of types.
	''' 
	''' @since 1.8
	''' </summary>
	Public Interface AnnotatedTypeVariable
		Inherits AnnotatedType

		''' <summary>
		''' Returns the potentially annotated bounds of this type variable.
		''' </summary>
		''' <returns> the potentially annotated bounds of this type variable </returns>
		ReadOnly Property annotatedBounds As AnnotatedType()
	End Interface

End Namespace