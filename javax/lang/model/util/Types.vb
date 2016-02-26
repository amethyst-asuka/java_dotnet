Imports System.Collections.Generic
Imports javax.lang.model.element
Imports javax.lang.model.type

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

Namespace javax.lang.model.util

	''' <summary>
	''' Utility methods for operating on types.
	''' 
	''' <p><b>Compatibility Note:</b> Methods may be added to this interface
	''' in future releases of the platform.
	''' 
	''' @author Joseph D. Darcy
	''' @author Scott Seligman
	''' @author Peter von der Ah&eacute; </summary>
	''' <seealso cref= javax.annotation.processing.ProcessingEnvironment#getTypeUtils
	''' @since 1.6 </seealso>
	Public Interface Types

		''' <summary>
		''' Returns the element corresponding to a type.
		''' The type may be a {@code DeclaredType} or {@code TypeVariable}.
		''' Returns {@code null} if the type is not one with a
		''' corresponding element.
		''' </summary>
		''' <param name="t"> the type to map to an element </param>
		''' <returns> the element corresponding to the given type </returns>
		Function asElement(ByVal t As TypeMirror) As Element

		''' <summary>
		''' Tests whether two {@code TypeMirror} objects represent the same type.
		''' 
		''' <p>Caveat: if either of the arguments to this method represents a
		''' wildcard, this method will return false.  As a consequence, a wildcard
		''' is not the same type as itself.  This might be surprising at first,
		''' but makes sense once you consider that an example like this must be
		''' rejected by the compiler:
		''' <pre>
		'''   {@code List<?> list = new ArrayList<Object>();}
		'''   {@code list.add(list.get(0));}
		''' </pre>
		''' 
		''' <p>Since annotations are only meta-data associated with a type,
		''' the set of annotations on either argument is <em>not</em> taken
		''' into account when computing whether or not two {@code
		''' TypeMirror} objects are the same type. In particular, two
		''' {@code TypeMirror} objects can have different annotations and
		''' still be considered the same.
		''' </summary>
		''' <param name="t1">  the first type </param>
		''' <param name="t2">  the second type </param>
		''' <returns> {@code true} if and only if the two types are the same </returns>
		Function isSameType(ByVal t1 As TypeMirror, ByVal t2 As TypeMirror) As Boolean

		''' <summary>
		''' Tests whether one type is a subtype of another.
		''' Any type is considered to be a subtype of itself.
		''' </summary>
		''' <param name="t1">  the first type </param>
		''' <param name="t2">  the second type </param>
		''' <returns> {@code true} if and only if the first type is a subtype
		'''          of the second </returns>
		''' <exception cref="IllegalArgumentException"> if given an executable or package type
		''' @jls 4.10 Subtyping </exception>
		Function isSubtype(ByVal t1 As TypeMirror, ByVal t2 As TypeMirror) As Boolean

		''' <summary>
		''' Tests whether one type is assignable to another.
		''' </summary>
		''' <param name="t1">  the first type </param>
		''' <param name="t2">  the second type </param>
		''' <returns> {@code true} if and only if the first type is assignable
		'''          to the second </returns>
		''' <exception cref="IllegalArgumentException"> if given an executable or package type
		''' @jls 5.2 Assignment Conversion </exception>
		Function isAssignable(ByVal t1 As TypeMirror, ByVal t2 As TypeMirror) As Boolean

		''' <summary>
		''' Tests whether one type argument <i>contains</i> another.
		''' </summary>
		''' <param name="t1">  the first type </param>
		''' <param name="t2">  the second type </param>
		''' <returns> {@code true} if and only if the first type contains the second </returns>
		''' <exception cref="IllegalArgumentException"> if given an executable or package type
		''' @jls 4.5.1.1 Type Argument Containment and Equivalence </exception>
		Function contains(ByVal t1 As TypeMirror, ByVal t2 As TypeMirror) As Boolean

		''' <summary>
		''' Tests whether the signature of one method is a <i>subsignature</i>
		''' of another.
		''' </summary>
		''' <param name="m1">  the first method </param>
		''' <param name="m2">  the second method </param>
		''' <returns> {@code true} if and only if the first signature is a
		'''          subsignature of the second
		''' @jls 8.4.2 Method Signature </returns>
		Function isSubsignature(ByVal m1 As ExecutableType, ByVal m2 As ExecutableType) As Boolean

		''' <summary>
		''' Returns the direct supertypes of a type.  The interface types, if any,
		''' will appear last in the list.
		''' </summary>
		''' <param name="t">  the type being examined </param>
		''' <returns> the direct supertypes, or an empty list if none </returns>
		''' <exception cref="IllegalArgumentException"> if given an executable or package type </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Function directSupertypes(ByVal t As TypeMirror) As IList(Of ? As TypeMirror)

		''' <summary>
		''' Returns the erasure of a type.
		''' </summary>
		''' <param name="t">  the type to be erased </param>
		''' <returns> the erasure of the given type </returns>
		''' <exception cref="IllegalArgumentException"> if given a package type
		''' @jls 4.6 Type Erasure </exception>
		Function erasure(ByVal t As TypeMirror) As TypeMirror

		''' <summary>
		''' Returns the class of a boxed value of a given primitive type.
		''' That is, <i>boxing conversion</i> is applied.
		''' </summary>
		''' <param name="p">  the primitive type to be converted </param>
		''' <returns> the class of a boxed value of type {@code p}
		''' @jls 5.1.7 Boxing Conversion </returns>
		Function boxedClass(ByVal p As PrimitiveType) As TypeElement

		''' <summary>
		''' Returns the type (a primitive type) of unboxed values of a given type.
		''' That is, <i>unboxing conversion</i> is applied.
		''' </summary>
		''' <param name="t">  the type to be unboxed </param>
		''' <returns> the type of an unboxed value of type {@code t} </returns>
		''' <exception cref="IllegalArgumentException"> if the given type has no
		'''          unboxing conversion
		''' @jls 5.1.8 Unboxing Conversion </exception>
		Function unboxedType(ByVal t As TypeMirror) As PrimitiveType

		''' <summary>
		''' Applies capture conversion to a type.
		''' </summary>
		''' <param name="t">  the type to be converted </param>
		''' <returns> the result of applying capture conversion </returns>
		''' <exception cref="IllegalArgumentException"> if given an executable or package type
		''' @jls 5.1.10 Capture Conversion </exception>
		Function capture(ByVal t As TypeMirror) As TypeMirror

		''' <summary>
		''' Returns a primitive type.
		''' </summary>
		''' <param name="kind">  the kind of primitive type to return </param>
		''' <returns> a primitive type </returns>
		''' <exception cref="IllegalArgumentException"> if {@code kind} is not a primitive kind </exception>
		Function getPrimitiveType(ByVal kind As TypeKind) As PrimitiveType

		''' <summary>
		''' Returns the null type.  This is the type of {@code null}.
		''' </summary>
		''' <returns> the null type </returns>
		ReadOnly Property nullType As NullType

		''' <summary>
		''' Returns a pseudo-type used where no actual type is appropriate.
		''' The kind of type to return may be either
		''' <seealso cref="TypeKind#VOID VOID"/> or <seealso cref="TypeKind#NONE NONE"/>.
		''' For packages, use
		''' <seealso cref="Elements#getPackageElement(CharSequence)"/>{@code .asType()}
		''' instead.
		''' </summary>
		''' <param name="kind">  the kind of type to return </param>
		''' <returns> a pseudo-type of kind {@code VOID} or {@code NONE} </returns>
		''' <exception cref="IllegalArgumentException"> if {@code kind} is not valid </exception>
		Function getNoType(ByVal kind As TypeKind) As NoType

		''' <summary>
		''' Returns an array type with the specified component type.
		''' </summary>
		''' <param name="componentType">  the component type </param>
		''' <returns> an array type with the specified component type. </returns>
		''' <exception cref="IllegalArgumentException"> if the component type is not valid for
		'''          an array </exception>
		Function getArrayType(ByVal componentType As TypeMirror) As ArrayType

		''' <summary>
		''' Returns a new wildcard type argument.  Either of the wildcard's
		''' bounds may be specified, or neither, but not both.
		''' </summary>
		''' <param name="extendsBound">  the extends (upper) bound, or {@code null} if none </param>
		''' <param name="superBound">    the super (lower) bound, or {@code null} if none </param>
		''' <returns> a new wildcard </returns>
		''' <exception cref="IllegalArgumentException"> if bounds are not valid </exception>
		Function getWildcardType(ByVal extendsBound As TypeMirror, ByVal superBound As TypeMirror) As WildcardType

		''' <summary>
		''' Returns the type corresponding to a type element and
		''' actual type arguments.
		''' Given the type element for {@code Set} and the type mirror
		''' for {@code String},
		''' for example, this method may be used to get the
		''' parameterized type {@code Set<String>}.
		''' 
		''' <p> The number of type arguments must either equal the
		''' number of the type element's formal type parameters, or must be
		''' zero.  If zero, and if the type element is generic,
		''' then the type element's raw type is returned.
		''' 
		''' <p> If a parameterized type is being returned, its type element
		''' must not be contained within a generic outer class.
		''' The parameterized type {@code Outer<String>.Inner<Number>},
		''' for example, may be constructed by first using this
		''' method to get the type {@code Outer<String>}, and then invoking
		''' <seealso cref="#getDeclaredType(DeclaredType, TypeElement, TypeMirror...)"/>.
		''' </summary>
		''' <param name="typeElem">  the type element </param>
		''' <param name="typeArgs">  the actual type arguments </param>
		''' <returns> the type corresponding to the type element and
		'''          actual type arguments </returns>
		''' <exception cref="IllegalArgumentException"> if too many or too few
		'''          type arguments are given, or if an inappropriate type
		'''          argument or type element is provided </exception>
		Function getDeclaredType(ByVal typeElem As TypeElement, ParamArray ByVal typeArgs As TypeMirror()) As DeclaredType

		''' <summary>
		''' Returns the type corresponding to a type element
		''' and actual type arguments, given a
		''' <seealso cref="DeclaredType#getEnclosingType() containing type"/>
		''' of which it is a member.
		''' The parameterized type {@code Outer<String>.Inner<Number>},
		''' for example, may be constructed by first using
		''' <seealso cref="#getDeclaredType(TypeElement, TypeMirror...)"/>
		''' to get the type {@code Outer<String>}, and then invoking
		''' this method.
		''' 
		''' <p> If the containing type is a parameterized type,
		''' the number of type arguments must equal the
		''' number of {@code typeElem}'s formal type parameters.
		''' If it is not parameterized or if it is {@code null}, this method is
		''' equivalent to {@code getDeclaredType(typeElem, typeArgs)}.
		''' </summary>
		''' <param name="containing">  the containing type, or {@code null} if none </param>
		''' <param name="typeElem">    the type element </param>
		''' <param name="typeArgs">    the actual type arguments </param>
		''' <returns> the type corresponding to the type element and
		'''          actual type arguments, contained within the given type </returns>
		''' <exception cref="IllegalArgumentException"> if too many or too few
		'''          type arguments are given, or if an inappropriate type
		'''          argument, type element, or containing type is provided </exception>
		Function getDeclaredType(ByVal containing As DeclaredType, ByVal typeElem As TypeElement, ParamArray ByVal typeArgs As TypeMirror()) As DeclaredType

		''' <summary>
		''' Returns the type of an element when that element is viewed as
		''' a member of, or otherwise directly contained by, a given type.
		''' For example,
		''' when viewed as a member of the parameterized type {@code Set<String>},
		''' the {@code Set.add} method is an {@code ExecutableType}
		''' whose parameter is of type {@code String}.
		''' </summary>
		''' <param name="containing">  the containing type </param>
		''' <param name="element">     the element </param>
		''' <returns> the type of the element as viewed from the containing type </returns>
		''' <exception cref="IllegalArgumentException"> if the element is not a valid one
		'''          for the given type </exception>
		Function asMemberOf(ByVal containing As DeclaredType, ByVal element As Element) As TypeMirror
	End Interface

End Namespace