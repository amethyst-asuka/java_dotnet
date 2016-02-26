Imports System.Collections.Generic
Imports javax.lang.model.type
Imports javax.lang.model.util

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

Namespace javax.lang.model.element

	''' <summary>
	''' Represents a class or interface program element.  Provides access
	''' to information about the type and its members.  Note that an enum
	''' type is a kind of class and an annotation type is a kind of
	''' interface.
	''' 
	''' <p> <a name="ELEM_VS_TYPE"></a>
	''' While a {@code TypeElement} represents a class or interface
	''' <i>element</i>, a <seealso cref="DeclaredType"/> represents a class
	''' or interface <i>type</i>, the latter being a use
	''' (or <i>invocation</i>) of the former.
	''' The distinction is most apparent with generic types,
	''' for which a single element can define a whole
	''' family of types.  For example, the element
	''' {@code java.util.Set} corresponds to the parameterized types
	''' {@code java.util.Set<String>} and {@code java.util.Set<Number>}
	''' (and many others), and to the raw type {@code java.util.Set}.
	''' 
	''' <p> Each method of this interface that returns a list of elements
	''' will return them in the order that is natural for the underlying
	''' source of program information.  For example, if the underlying
	''' source of information is Java source code, then the elements will be
	''' returned in source code order.
	''' 
	''' @author Joseph D. Darcy
	''' @author Scott Seligman
	''' @author Peter von der Ah&eacute; </summary>
	''' <seealso cref= DeclaredType
	''' @since 1.6 </seealso>
	Public Interface TypeElement
		Inherits Element, Parameterizable, QualifiedNameable

		''' <summary>
		''' Returns the fields, methods, constructors, and member types
		''' that are directly declared in this class or interface.
		''' 
		''' This includes any (implicit) default constructor and
		''' the implicit {@code values} and {@code valueOf} methods of an
		''' enum type.
		''' 
		''' <p> Note that as a particular instance of the {@linkplain
		''' javax.lang.model.element general accuracy requirements} and the
		''' ordering behavior required of this interface, the list of
		''' enclosed elements will be returned in the natural order for the
		''' originating source of information about the type.  For example,
		''' if the information about the type is originating from a source
		''' file, the elements will be returned in source code order.
		''' (However, in that case the the ordering of synthesized
		''' elements, such as a default constructor, is not specified.)
		''' </summary>
		''' <returns> the enclosed elements in proper order, or an empty list if none </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		ReadOnly Property Overrides enclosedElements As IList(Of ? As Element)

		''' <summary>
		''' Returns the <i>nesting kind</i> of this type element.
		''' </summary>
		''' <returns> the nesting kind of this type element </returns>
		ReadOnly Property nestingKind As NestingKind

		''' <summary>
		''' Returns the fully qualified name of this type element.
		''' More precisely, it returns the <i>canonical</i> name.
		''' For local and anonymous classes, which do not have canonical names,
		''' an empty name is returned.
		''' 
		''' <p>The name of a generic type does not include any reference
		''' to its formal type parameters.
		''' For example, the fully qualified name of the interface
		''' {@code java.util.Set<E>} is "{@code java.util.Set}".
		''' Nested types use "{@code .}" as a separator, as in
		''' "{@code java.util.Map.Entry}".
		''' </summary>
		''' <returns> the fully qualified name of this class or interface, or
		''' an empty name if none
		''' </returns>
		''' <seealso cref= Elements#getBinaryName
		''' @jls 6.7 Fully Qualified Names and Canonical Names </seealso>
		ReadOnly Property qualifiedName As Name

		''' <summary>
		''' Returns the simple name of this type element.
		''' 
		''' For an anonymous class, an empty name is returned.
		''' </summary>
		''' <returns> the simple name of this class or interface,
		''' an empty name for an anonymous class
		'''  </returns>
		ReadOnly Property Overrides simpleName As Name

		''' <summary>
		''' Returns the direct superclass of this type element.
		''' If this type element represents an interface or the class
		''' {@code java.lang.Object}, then a <seealso cref="NoType"/>
		''' with kind <seealso cref="TypeKind#NONE NONE"/> is returned.
		''' </summary>
		''' <returns> the direct superclass, or a {@code NoType} if there is none </returns>
		ReadOnly Property superclass As TypeMirror

		''' <summary>
		''' Returns the interface types directly implemented by this class
		''' or extended by this interface.
		''' </summary>
		''' <returns> the interface types directly implemented by this class
		''' or extended by this interface, or an empty list if there are none </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		ReadOnly Property interfaces As IList(Of ? As TypeMirror)

		''' <summary>
		''' Returns the formal type parameters of this type element
		''' in declaration order.
		''' </summary>
		''' <returns> the formal type parameters, or an empty list
		''' if there are none </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		ReadOnly Property typeParameters As IList(Of ? As TypeParameterElement)

		''' <summary>
		''' Returns the package of a top-level type and returns the
		''' immediately lexically enclosing element for a {@linkplain
		''' NestingKind#isNested nested} type.
		''' </summary>
		''' <returns> the package of a top-level type, the immediately
		''' lexically enclosing element for a nested type </returns>
		ReadOnly Property Overrides enclosingElement As Element
	End Interface

End Namespace