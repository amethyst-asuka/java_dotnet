Imports System.Collections.Generic
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

Namespace javax.lang.model.element

	''' <summary>
	''' Represents a method, constructor, or initializer (static or
	''' instance) of a class or interface, including annotation type
	''' elements.
	''' 
	''' @author Joseph D. Darcy
	''' @author Scott Seligman
	''' @author Peter von der Ah&eacute; </summary>
	''' <seealso cref= ExecutableType
	''' @since 1.6 </seealso>
	Public Interface ExecutableElement
		Inherits Element, Parameterizable

		''' <summary>
		''' Returns the formal type parameters of this executable
		''' in declaration order.
		''' </summary>
		''' <returns> the formal type parameters, or an empty list
		''' if there are none </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		ReadOnly Property typeParameters As IList(Of ? As TypeParameterElement)

		''' <summary>
		''' Returns the return type of this executable.
		''' Returns a <seealso cref="NoType"/> with kind <seealso cref="TypeKind#VOID VOID"/>
		''' if this executable is not a method, or is a method that does not
		''' return a value.
		''' </summary>
		''' <returns> the return type of this executable </returns>
		ReadOnly Property returnType As TypeMirror

		''' <summary>
		''' Returns the formal parameters of this executable.
		''' They are returned in declaration order.
		''' </summary>
		''' <returns> the formal parameters,
		''' or an empty list if there are none </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		ReadOnly Property parameters As IList(Of ? As VariableElement)

		''' <summary>
		''' Returns the receiver type of this executable,
		''' or <seealso cref="javax.lang.model.type.NoType NoType"/> with
		''' kind <seealso cref="javax.lang.model.type.TypeKind#NONE NONE"/>
		''' if the executable has no receiver type.
		''' 
		''' An executable which is an instance method, or a constructor of an
		''' inner class, has a receiver type derived from the {@linkplain
		''' #getEnclosingElement declaring type}.
		''' 
		''' An executable which is a static method, or a constructor of a
		''' non-inner class, or an initializer (static or instance), has no
		''' receiver type.
		''' </summary>
		''' <returns> the receiver type of this executable
		''' @since 1.8 </returns>
		ReadOnly Property receiverType As TypeMirror

		''' <summary>
		''' Returns {@code true} if this method or constructor accepts a variable
		''' number of arguments and returns {@code false} otherwise.
		''' </summary>
		''' <returns> {@code true} if this method or constructor accepts a variable
		''' number of arguments and {@code false} otherwise </returns>
		ReadOnly Property varArgs As Boolean

		''' <summary>
		''' Returns {@code true} if this method is a default method and
		''' returns {@code false} otherwise.
		''' </summary>
		''' <returns> {@code true} if this method is a default method and
		''' {@code false} otherwise
		''' @since 1.8 </returns>
		ReadOnly Property [default] As Boolean

		''' <summary>
		''' Returns the exceptions and other throwables listed in this
		''' method or constructor's {@code throws} clause in declaration
		''' order.
		''' </summary>
		''' <returns> the exceptions and other throwables listed in the
		''' {@code throws} clause, or an empty list if there are none </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		ReadOnly Property thrownTypes As IList(Of ? As TypeMirror)

		''' <summary>
		''' Returns the default value if this executable is an annotation
		''' type element.  Returns {@code null} if this method is not an
		''' annotation type element, or if it is an annotation type element
		''' with no default value.
		''' </summary>
		''' <returns> the default value, or {@code null} if none </returns>
		ReadOnly Property defaultValue As AnnotationValue

        ''' <summary>
        ''' Returns the simple name of a constructor, method, or
        ''' initializer.  For a constructor, the name {@code "<init>"} is
        ''' returned, for a static initializer, the name {@code "<clinit>"}
        ''' is returned, and for an anonymous class or instance
        ''' initializer, an empty name is returned.
        ''' </summary>
        ''' <returns> the simple name of a constructor, method, or
        ''' initializer </returns>
        ReadOnly Property simpleName As Name
    End Interface

End Namespace