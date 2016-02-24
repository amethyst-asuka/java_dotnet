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

Namespace java.lang


	''' <summary>
	''' An informative annotation type used to indicate that an interface
	''' type declaration is intended to be a <i>functional interface</i> as
	''' defined by the Java Language Specification.
	''' 
	''' Conceptually, a functional interface has exactly one abstract
	''' method.  Since {@link java.lang.reflect.Method#isDefault()
	''' default methods} have an implementation, they are not abstract.  If
	''' an interface declares an abstract method overriding one of the
	''' public methods of {@code java.lang.Object}, that also does
	''' <em>not</em> count toward the interface's abstract method count
	''' since any implementation of the interface will have an
	''' implementation from {@code java.lang.Object} or elsewhere.
	''' 
	''' <p>Note that instances of functional interfaces can be created with
	''' lambda expressions, method references, or constructor references.
	''' 
	''' <p>If a type is annotated with this annotation type, compilers are
	''' required to generate an error message unless:
	''' 
	''' <ul>
	''' <li> The type is an interface type and not an annotation type, enum, or class.
	''' <li> The annotated type satisfies the requirements of a functional interface.
	''' </ul>
	''' 
	''' <p>However, the compiler will treat any interface meeting the
	''' definition of a functional interface as a functional interface
	''' regardless of whether or not a {@code FunctionalInterface}
	''' annotation is present on the interface declaration.
	''' 
	''' @jls 4.3.2. The Class Object
	''' @jls 9.8 Functional Interfaces
	''' @jls 9.4.3 Interface Method Body
	''' @since 1.8
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(AttributeTargets.Class, AllowMultiple := False, Inherited := False> _
	Public Class FunctionalInterface
		Inherits System.Attribute

	End Class

End Namespace