Imports System
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
	''' Represents a program element such as a package, class, or method.
	''' Each element represents a static, language-level construct
	''' (and not, for example, a runtime construct of the virtual machine).
	''' 
	''' <p> Elements should be compared using the <seealso cref="#equals(Object)"/>
	''' method.  There is no guarantee that any particular element will
	''' always be represented by the same object.
	''' 
	''' <p> To implement operations based on the class of an {@code
	''' Element} object, either use a <seealso cref="ElementVisitor visitor"/> or
	''' use the result of the <seealso cref="#getKind"/> method.  Using {@code
	''' instanceof} is <em>not</em> necessarily a reliable idiom for
	''' determining the effective class of an object in this modeling
	''' hierarchy since an implementation may choose to have a single object
	''' implement multiple {@code Element} subinterfaces.
	''' 
	''' @author Joseph D. Darcy
	''' @author Scott Seligman
	''' @author Peter von der Ah&eacute; </summary>
	''' <seealso cref= Elements </seealso>
	''' <seealso cref= TypeMirror
	''' @since 1.6 </seealso>
	Public Interface Element
		Inherits javax.lang.model.AnnotatedConstruct

		''' <summary>
		''' Returns the type defined by this element.
		''' 
		''' <p> A generic element defines a family of types, not just one.
		''' If this is a generic element, a <i>prototypical</i> type is
		''' returned.  This is the element's invocation on the
		''' type variables corresponding to its own formal type parameters.
		''' For example,
		''' for the generic class element {@code C<N extends Number>},
		''' the parameterized type {@code C<N>} is returned.
		''' The <seealso cref="Types"/> utility interface has more general methods
		''' for obtaining the full range of types defined by an element.
		''' </summary>
		''' <seealso cref= Types
		''' </seealso>
		''' <returns> the type defined by this element </returns>
		Function asType() As TypeMirror

		''' <summary>
		''' Returns the {@code kind} of this element.
		''' </summary>
		''' <returns> the kind of this element </returns>
		ReadOnly Property kind As ElementKind

		''' <summary>
		''' Returns the modifiers of this element, excluding annotations.
		''' Implicit modifiers, such as the {@code public} and {@code static}
		''' modifiers of interface members, are included.
		''' </summary>
		''' <returns> the modifiers of this element, or an empty set if there are none </returns>
		ReadOnly Property modifiers As java.util.Set(Of Modifier)

		''' <summary>
		''' Returns the simple (unqualified) name of this element.  The
		''' name of a generic type does not include any reference to its
		''' formal type parameters.
		''' 
		''' For example, the simple name of the type element {@code
		''' java.util.Set<E>} is {@code "Set"}.
		''' 
		''' If this element represents an unnamed {@linkplain
		''' PackageElement#getSimpleName package}, an empty name is
		''' returned.
		''' 
		''' If it represents a {@link ExecutableElement#getSimpleName
		''' constructor}, the name "{@code <init>}" is returned.  If it
		''' represents a {@link ExecutableElement#getSimpleName static
		''' initializer}, the name "{@code <clinit>}" is returned.
		''' 
		''' If it represents an {@link TypeElement#getSimpleName
		''' anonymous class} or {@link ExecutableElement#getSimpleName
		''' instance initializer}, an empty name is returned.
		''' </summary>
		''' <returns> the simple name of this element </returns>
		''' <seealso cref= PackageElement#getSimpleName </seealso>
		''' <seealso cref= ExecutableElement#getSimpleName </seealso>
		''' <seealso cref= TypeElement#getSimpleName </seealso>
		''' <seealso cref= VariableElement#getSimpleName </seealso>
		ReadOnly Property simpleName As Name

		''' <summary>
		''' Returns the innermost element
		''' within which this element is, loosely speaking, enclosed.
		''' <ul>
		''' <li> If this element is one whose declaration is lexically enclosed
		''' immediately within the declaration of another element, that other
		''' element is returned.
		''' 
		''' <li> If this is a {@link TypeElement#getEnclosingElement
		''' top-level type}, its package is returned.
		''' 
		''' <li> If this is a {@linkplain
		''' PackageElement#getEnclosingElement package}, {@code null} is
		''' returned.
		''' 
		''' <li> If this is a {@linkplain
		''' TypeParameterElement#getEnclosingElement type parameter},
		''' {@link TypeParameterElement#getGenericElement the
		''' generic element} of the type parameter is returned.
		''' 
		''' <li> If this is a {@linkplain
		''' VariableElement#getEnclosingElement method or constructor
		''' parameter}, {@link ExecutableElement the executable
		''' element} which declares the parameter is returned.
		''' 
		''' </ul>
		''' </summary>
		''' <returns> the enclosing element, or {@code null} if there is none </returns>
		''' <seealso cref= Elements#getPackageOf </seealso>
		ReadOnly Property enclosingElement As Element

		''' <summary>
		''' Returns the elements that are, loosely speaking, directly
		''' enclosed by this element.
		''' 
		''' A {@link TypeElement#getEnclosedElements class or
		''' interface} is considered to enclose the fields, methods,
		''' constructors, and member types that it directly declares.
		''' 
		''' A <seealso cref="PackageElement#getEnclosedElements package"/>
		''' encloses the top-level classes and interfaces within it, but is
		''' not considered to enclose subpackages.
		''' 
		''' Other kinds of elements are not currently considered to enclose
		''' any elements; however, that may change as this API or the
		''' programming language evolves.
		''' 
		''' <p>Note that elements of certain kinds can be isolated using
		''' methods in <seealso cref="ElementFilter"/>.
		''' </summary>
		''' <returns> the enclosed elements, or an empty list if none </returns>
		''' <seealso cref= PackageElement#getEnclosedElements </seealso>
		''' <seealso cref= TypeElement#getEnclosedElements </seealso>
		''' <seealso cref= Elements#getAllMembers
		''' @jls 8.8.9 Default Constructor
		''' @jls 8.9 Enums </seealso>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		ReadOnly Property enclosedElements As IList(Of ? As Element)

        ''' <summary>
        ''' Returns {@code true} if the argument represents the same
        ''' element as {@code this}, or {@code false} otherwise.
        ''' 
        ''' <p>Note that the identity of an element involves implicit state
        ''' not directly accessible from the element's methods, including
        ''' state about the presence of unrelated types.  Element objects
        ''' created by different implementations of these interfaces should
        ''' <i>not</i> be expected to be equal even if &quot;the same&quot;
        ''' element is being modeled; this is analogous to the inequality
        ''' of {@code Class} objects for the same class file loaded through
        ''' different class loaders.
        ''' </summary>
        ''' <param name="obj">  the object to be compared with this element </param>
        ''' <returns> {@code true} if the specified object represents the same
        '''          element as this </returns>
        Function Equals(ByVal obj As Object) As Boolean

        ''' <summary>
        ''' Obeys the general contract of <seealso cref="Object#hashCode Object.hashCode"/>.
        ''' </summary>
        ''' <seealso cref= #equals </seealso>
        Function GetHashCode() As Integer


        ''' <summary>
        ''' {@inheritDoc}
        ''' 
        ''' <p> To get inherited annotations as well, use {@link
        ''' Elements#getAllAnnotationMirrors(Element)
        ''' getAllAnnotationMirrors}.
        ''' 
        ''' @since 1.6
        ''' </summary>
        'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
        ReadOnly Property annotationMirrors As IList(Of ? As AnnotationMirror)

        ''' <summary>
        ''' {@inheritDoc}
        ''' @since 1.6
        ''' </summary>
        Function getAnnotation(Of A As annotation)(ByVal annotationType As type) As A

        ''' <summary>
        ''' Applies a visitor to this element.
        ''' </summary>
        ''' @param <R> the return type of the visitor's methods </param>
        ''' @param <P> the type of the additional parameter to the visitor's methods </param>
        ''' <param name="v">   the visitor operating on this element </param>
        ''' <param name="p">   additional parameter to the visitor </param>
        ''' <returns> a visitor-specified result </returns>
        Function accept(Of R, P)(ByVal v As ElementVisitor(Of R, P), ByVal params As P) As R
	End Interface

End Namespace