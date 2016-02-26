'
' * Copyright (c) 2005, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' Represents an array type.
	''' A multidimensional array type is represented as an array type
	''' whose component type is also an array type.
	''' 
	''' @author Joseph D. Darcy
	''' @author Scott Seligman
	''' @author Peter von der Ah&eacute;
	''' @since 1.6
	''' </summary>
	Public Interface ArrayType
		Inherits ReferenceType

		''' <summary>
		''' Returns the component type of this array type.
		''' </summary>
		''' <returns> the component type of this array type </returns>
		ReadOnly Property componentType As TypeMirror
	End Interface

End Namespace