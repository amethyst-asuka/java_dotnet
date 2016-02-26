'
' * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' The kind of a type mirror.
	''' 
	''' <p>Note that it is possible additional type kinds will be added to
	''' accommodate new, currently unknown, language structures added to
	''' future versions of the Java&trade; programming language.
	''' 
	''' @author Joseph D. Darcy
	''' @author Scott Seligman
	''' @author Peter von der Ah&eacute; </summary>
	''' <seealso cref= TypeMirror
	''' @since 1.6 </seealso>
	Public Enum TypeKind
		''' <summary>
		''' The primitive type {@code boolean}.
		''' </summary>
		[BOOLEAN]

		''' <summary>
		''' The primitive type {@code byte}.
		''' </summary>
		[BYTE]

		''' <summary>
		''' The primitive type {@code short}.
		''' </summary>
		[SHORT]

		''' <summary>
		''' The primitive type {@code int}.
		''' </summary>
		INT

		''' <summary>
		''' The primitive type {@code long}.
		''' </summary>
		[LONG]

		''' <summary>
		''' The primitive type {@code char}.
		''' </summary>
		[CHAR]

		''' <summary>
		''' The primitive type {@code float}.
		''' </summary>
		FLOAT

		''' <summary>
		''' The primitive type {@code double}.
		''' </summary>
		[DOUBLE]

		''' <summary>
		''' The pseudo-type corresponding to the keyword {@code void}. </summary>
		''' <seealso cref= NoType </seealso>
		VOID

		''' <summary>
		''' A pseudo-type used where no actual type is appropriate. </summary>
		''' <seealso cref= NoType </seealso>
		NONE

		''' <summary>
		''' The null type.
		''' </summary>
		NULL

		''' <summary>
		''' An array type.
		''' </summary>
		ARRAY

		''' <summary>
		''' A class or interface type.
		''' </summary>
		DECLARED

		''' <summary>
		''' A class or interface type that could not be resolved.
		''' </summary>
		[ERROR]

		''' <summary>
		''' A type variable.
		''' </summary>
		TYPEVAR

		''' <summary>
		''' A wildcard type argument.
		''' </summary>
		WILDCARD

		''' <summary>
		''' A pseudo-type corresponding to a package element. </summary>
		''' <seealso cref= NoType </seealso>
		PACKAGE

		''' <summary>
		''' A method, constructor, or initializer.
		''' </summary>
		EXECUTABLE

		''' <summary>
		''' An implementation-reserved type.
		''' This is not the type you are looking for.
		''' </summary>
		OTHER

		''' <summary>
		''' A union type.
		'''  
		''' @since 1.7
		''' </summary>
		UNION

		''' <summary>
		''' An intersection type.
		'''  
		''' @since 1.8
		''' </summary>
		INTERSECTION
	End Enum
End Namespace