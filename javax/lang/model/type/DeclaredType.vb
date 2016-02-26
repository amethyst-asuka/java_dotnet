Imports System.Collections.Generic

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
	''' Represents a declared type, either a class type or an interface type.
	''' This includes parameterized types such as {@code java.util.Set<String>}
	''' as well as raw types.
	''' 
	''' <p> While a {@code TypeElement} represents a class or interface
	''' <i>element</i>, a {@code DeclaredType} represents a class
	''' or interface <i>type</i>, the latter being a use
	''' (or <i>invocation</i>) of the former.
	''' See <seealso cref="TypeElement"/> for more on this distinction.
	''' 
	''' <p> The supertypes (both class and interface types) of a declared
	''' type may be found using the {@link
	''' Types#directSupertypes(TypeMirror)} method.  This returns the
	''' supertypes with any type arguments substituted in.
	''' 
	''' @author Joseph D. Darcy
	''' @author Scott Seligman
	''' @author Peter von der Ah&eacute; </summary>
	''' <seealso cref= TypeElement
	''' @since 1.6 </seealso>
	Public Interface DeclaredType
		Inherits ReferenceType

		''' <summary>
		''' Returns the element corresponding to this type.
		''' </summary>
		''' <returns> the element corresponding to this type </returns>
		Function asElement() As javax.lang.model.element.Element

		''' <summary>
		''' Returns the type of the innermost enclosing instance or a
		''' {@code NoType} of kind {@code NONE} if there is no enclosing
		''' instance.  Only types corresponding to inner classes have an
		''' enclosing instance.
		''' </summary>
		''' <returns> a type mirror for the enclosing type
		''' @jls 8.1.3 Inner Classes and Enclosing Instances
		''' @jls 15.9.2 Determining Enclosing Instances </returns>
		ReadOnly Property enclosingType As TypeMirror

		''' <summary>
		''' Returns the actual type arguments of this type.
		''' For a type nested within a parameterized type
		''' (such as {@code Outer<String>.Inner<Number>}), only the type
		''' arguments of the innermost type are included.
		''' </summary>
		''' <returns> the actual type arguments of this type, or an empty list
		'''           if none </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		ReadOnly Property typeArguments As IList(Of ? As TypeMirror)
	End Interface

End Namespace