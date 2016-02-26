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
	''' Represents a type variable.
	''' A type variable may be explicitly declared by a
	''' <seealso cref="TypeParameterElement type parameter"/> of a
	''' type, method, or constructor.
	''' A type variable may also be declared implicitly, as by
	''' the capture conversion of a wildcard type argument
	''' (see chapter 5 of
	''' <cite>The Java&trade; Language Specification</cite>).
	''' 
	''' @author Joseph D. Darcy
	''' @author Scott Seligman
	''' @author Peter von der Ah&eacute; </summary>
	''' <seealso cref= TypeParameterElement
	''' @since 1.6 </seealso>
	Public Interface TypeVariable
		Inherits ReferenceType

		''' <summary>
		''' Returns the element corresponding to this type variable.
		''' </summary>
		''' <returns> the element corresponding to this type variable </returns>
		Function asElement() As javax.lang.model.element.Element

		''' <summary>
		''' Returns the upper bound of this type variable.
		''' 
		''' <p> If this type variable was declared with no explicit
		''' upper bounds, the result is {@code java.lang.Object}.
		''' If it was declared with multiple upper bounds,
		''' the result is an <seealso cref="IntersectionType intersection type"/>;
		''' individual bounds can be found by examining the result's
		''' <seealso cref="IntersectionType#getBounds() bounds"/>.
		''' </summary>
		''' <returns> the upper bound of this type variable </returns>
		ReadOnly Property upperBound As TypeMirror

		''' <summary>
		''' Returns the lower bound of this type variable.  While a type
		''' parameter cannot include an explicit lower bound declaration,
		''' capture conversion can produce a type variable with a
		''' non-trivial lower bound.  Type variables otherwise have a
		''' lower bound of <seealso cref="NullType"/>.
		''' </summary>
		''' <returns> the lower bound of this type variable </returns>
		ReadOnly Property lowerBound As TypeMirror
	End Interface

End Namespace