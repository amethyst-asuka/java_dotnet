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
	''' Utility methods for operating on program elements.
	''' 
	''' <p><b>Compatibility Note:</b> Methods may be added to this interface
	''' in future releases of the platform.
	''' 
	''' @author Joseph D. Darcy
	''' @author Scott Seligman
	''' @author Peter von der Ah&eacute; </summary>
	''' <seealso cref= javax.annotation.processing.ProcessingEnvironment#getElementUtils
	''' @since 1.6 </seealso>
	Public Interface Elements

		''' <summary>
		''' Returns a package given its fully qualified name.
		''' </summary>
		''' <param name="name">  fully qualified package name, or "" for an unnamed package </param>
		''' <returns> the named package, or {@code null} if it cannot be found </returns>
		Function getPackageElement(ByVal name As CharSequence) As PackageElement

		''' <summary>
		''' Returns a type element given its canonical name.
		''' </summary>
		''' <param name="name">  the canonical name </param>
		''' <returns> the named type element, or {@code null} if it cannot be found </returns>
		Function getTypeElement(ByVal name As CharSequence) As TypeElement

		''' <summary>
		''' Returns the values of an annotation's elements, including defaults.
		''' </summary>
		''' <seealso cref= AnnotationMirror#getElementValues() </seealso>
		''' <param name="a">  annotation to examine </param>
		''' <returns> the values of the annotation's elements, including defaults </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Function getElementValuesWithDefaults(ByVal a As AnnotationMirror) As IDictionary(Of ? As ExecutableElement, ? As AnnotationValue)

		''' <summary>
		''' Returns the text of the documentation (&quot;Javadoc&quot;)
		''' comment of an element.
		''' 
		''' <p> A documentation comment of an element is a comment that
		''' begins with "{@code /**}" , ends with a separate
		''' "<code>*&#47;</code>", and immediately precedes the element,
		''' ignoring white space.  Therefore, a documentation comment
		''' contains at least three"{@code *}" characters.  The text
		''' returned for the documentation comment is a processed form of
		''' the comment as it appears in source code.  The leading "{@code
		''' /**}" and trailing "<code>*&#47;</code>" are removed.  For lines
		''' of the comment starting after the initial "{@code /**}",
		''' leading white space characters are discarded as are any
		''' consecutive "{@code *}" characters appearing after the white
		''' space or starting the line.  The processed lines are then
		''' concatenated together (including line terminators) and
		''' returned.
		''' </summary>
		''' <param name="e">  the element being examined </param>
		''' <returns> the documentation comment of the element, or {@code null}
		'''          if there is none
		''' @jls 3.6 White Space </returns>
		Function getDocComment(ByVal e As Element) As String

		''' <summary>
		''' Returns {@code true} if the element is deprecated, {@code false} otherwise.
		''' </summary>
		''' <param name="e">  the element being examined </param>
		''' <returns> {@code true} if the element is deprecated, {@code false} otherwise </returns>
		Function isDeprecated(ByVal e As Element) As Boolean

		''' <summary>
		''' Returns the <i>binary name</i> of a type element.
		''' </summary>
		''' <param name="type">  the type element being examined </param>
		''' <returns> the binary name
		''' </returns>
		''' <seealso cref= TypeElement#getQualifiedName
		''' @jls 13.1 The Form of a Binary </seealso>
		Function getBinaryName(ByVal type As TypeElement) As Name


		''' <summary>
		''' Returns the package of an element.  The package of a package is
		''' itself.
		''' </summary>
		''' <param name="type"> the element being examined </param>
		''' <returns> the package of an element </returns>
		Function getPackageOf(ByVal type As Element) As PackageElement

		''' <summary>
		''' Returns all members of a type element, whether inherited or
		''' declared directly.  For a class the result also includes its
		''' constructors, but not local or anonymous classes.
		''' 
		''' <p>Note that elements of certain kinds can be isolated using
		''' methods in <seealso cref="ElementFilter"/>.
		''' </summary>
		''' <param name="type">  the type being examined </param>
		''' <returns> all members of the type </returns>
		''' <seealso cref= Element#getEnclosedElements </seealso>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Function getAllMembers(ByVal type As TypeElement) As IList(Of ? As Element)

		''' <summary>
		''' Returns all annotations <i>present</i> on an element, whether
		''' directly present or present via inheritance.
		''' </summary>
		''' <param name="e">  the element being examined </param>
		''' <returns> all annotations of the element </returns>
		''' <seealso cref= Element#getAnnotationMirrors </seealso>
		''' <seealso cref= javax.lang.model.AnnotatedConstruct </seealso>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Function getAllAnnotationMirrors(ByVal e As Element) As IList(Of ? As AnnotationMirror)

		''' <summary>
		''' Tests whether one type, method, or field hides another.
		''' </summary>
		''' <param name="hider">   the first element </param>
		''' <param name="hidden">  the second element </param>
		''' <returns> {@code true} if and only if the first element hides
		'''          the second </returns>
		Function hides(ByVal hider As Element, ByVal hidden As Element) As Boolean

		''' <summary>
		''' Tests whether one method, as a member of a given type,
		''' overrides another method.
		''' When a non-abstract method overrides an abstract one, the
		''' former is also said to <i>implement</i> the latter.
		''' 
		''' <p> In the simplest and most typical usage, the value of the
		''' {@code type} parameter will simply be the class or interface
		''' directly enclosing {@code overrider} (the possibly-overriding
		''' method).  For example, suppose {@code m1} represents the method
		''' {@code String.hashCode} and {@code m2} represents {@code
		''' Object.hashCode}.  We can then ask whether {@code m1} overrides
		''' {@code m2} within the class {@code String} (it does):
		''' 
		''' <blockquote>
		''' {@code assert elements.overrides(m1, m2,
		'''          elements.getTypeElement("java.lang.String")); }
		''' </blockquote>
		''' 
		''' A more interesting case can be illustrated by the following example
		''' in which a method in type {@code A} does not override a
		''' like-named method in type {@code B}:
		''' 
		''' <blockquote>
		''' {@code class A { public void m() {} } }<br>
		''' {@code interface B { void m(); } }<br>
		''' ...<br>
		''' {@code m1 = ...;  // A.m }<br>
		''' {@code m2 = ...;  // B.m }<br>
		''' {@code assert ! elements.overrides(m1, m2,
		'''          elements.getTypeElement("A")); }
		''' </blockquote>
		''' 
		''' When viewed as a member of a third type {@code C}, however,
		''' the method in {@code A} does override the one in {@code B}:
		''' 
		''' <blockquote>
		''' {@code class C extends A implements B {} }<br>
		''' ...<br>
		''' {@code assert elements.overrides(m1, m2,
		'''          elements.getTypeElement("C")); }
		''' </blockquote>
		''' </summary>
		''' <param name="overrider">  the first method, possible overrider </param>
		''' <param name="overridden">  the second method, possibly being overridden </param>
		''' <param name="type">   the type of which the first method is a member </param>
		''' <returns> {@code true} if and only if the first method overrides
		'''          the second
		''' @jls 8.4.8 Inheritance, Overriding, and Hiding
		''' @jls 9.4.1 Inheritance and Overriding </returns>
		Function [overrides](ByVal overrider As ExecutableElement, ByVal overridden As ExecutableElement, ByVal type As TypeElement) As Boolean

		''' <summary>
		''' Returns the text of a <i>constant expression</i> representing a
		''' primitive value or a string.
		''' The text returned is in a form suitable for representing the value
		''' in source code.
		''' </summary>
		''' <param name="value">  a primitive value or string </param>
		''' <returns> the text of a constant expression </returns>
		''' <exception cref="IllegalArgumentException"> if the argument is not a primitive
		'''          value or string
		''' </exception>
		''' <seealso cref= VariableElement#getConstantValue() </seealso>
		Function getConstantExpression(ByVal value As Object) As String

		''' <summary>
		''' Prints a representation of the elements to the given writer in
		''' the specified order.  The main purpose of this method is for
		''' diagnostics.  The exact format of the output is <em>not</em>
		''' specified and is subject to change.
		''' </summary>
		''' <param name="w"> the writer to print the output to </param>
		''' <param name="elements"> the elements to print </param>
		Sub printElements(ByVal w As java.io.Writer, ParamArray ByVal elements As Element())

		''' <summary>
		''' Return a name with the same sequence of characters as the
		''' argument.
		''' </summary>
		''' <param name="cs"> the character sequence to return as a name </param>
		''' <returns> a name with the same sequence of characters as the argument </returns>
		Function getName(ByVal cs As CharSequence) As Name

		''' <summary>
		''' Returns {@code true} if the type element is a functional interface, {@code false} otherwise.
		''' </summary>
		''' <param name="type"> the type element being examined </param>
		''' <returns> {@code true} if the element is a functional interface, {@code false} otherwise
		''' @jls 9.8 Functional Interfaces
		''' @since 1.8 </returns>
		Function isFunctionalInterface(ByVal type As TypeElement) As Boolean
	End Interface

End Namespace