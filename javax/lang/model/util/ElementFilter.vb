Imports System
Imports System.Collections.Generic
Imports javax.lang.model.element

'
' * Copyright (c) 2005, 2012, Oracle and/or its affiliates. All rights reserved.
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
	''' Filters for selecting just the elements of interest from a
	''' collection of elements.  The returned sets and lists are new
	''' collections and do use the argument as a backing store.  The
	''' methods in this class do not make any attempts to guard against
	''' concurrent modifications of the arguments.  The returned sets and
	''' lists are mutable but unsafe for concurrent access.  A returned set
	''' has the same iteration order as the argument set to a method.
	''' 
	''' <p>If iterables and sets containing {@code null} are passed as
	''' arguments to methods in this class, a {@code NullPointerException}
	''' will be thrown.
	''' 
	''' <p>Note that a <i>static import</i> statement can make the text of
	''' calls to the methods in this class more concise; for example:
	''' 
	''' <blockquote><pre>
	'''     import static javax.lang.model.util.ElementFilter.*;
	'''     ...
	'''         {@code List<VariableElement>} fs = fieldsIn(someClass.getEnclosedElements());
	''' </pre></blockquote>
	''' 
	''' @author Joseph D. Darcy
	''' @author Scott Seligman
	''' @author Peter von der Ah&eacute;
	''' @author Martin Buchholz
	''' @since 1.6
	''' </summary>
	Public Class ElementFilter
		Private Sub New() ' Do not instantiate.
		End Sub

		Private Shared ReadOnly CONSTRUCTOR_KIND As java.util.Set(Of ElementKind) = java.util.Collections.unmodifiableSet(java.util.EnumSet.of(ElementKind.CONSTRUCTOR))

		Private Shared ReadOnly FIELD_KINDS As java.util.Set(Of ElementKind) = java.util.Collections.unmodifiableSet(java.util.EnumSet.of(ElementKind.FIELD, ElementKind.ENUM_CONSTANT))
		Private Shared ReadOnly METHOD_KIND As java.util.Set(Of ElementKind) = java.util.Collections.unmodifiableSet(java.util.EnumSet.of(ElementKind.METHOD))

		Private Shared ReadOnly PACKAGE_KIND As java.util.Set(Of ElementKind) = java.util.Collections.unmodifiableSet(java.util.EnumSet.of(ElementKind.PACKAGE))

		Private Shared ReadOnly TYPE_KINDS As java.util.Set(Of ElementKind) = java.util.Collections.unmodifiableSet(java.util.EnumSet.of(ElementKind.CLASS, ElementKind.ENUM, ElementKind.INTERFACE, ElementKind.ANNOTATION_TYPE))
		''' <summary>
		''' Returns a list of fields in {@code elements}. </summary>
		''' <returns> a list of fields in {@code elements} </returns>
		''' <param name="elements"> the elements to filter </param>
		Public Shared Function fieldsIn(Of T1 As Element)(ByVal elements As IEnumerable(Of T1)) As IList(Of VariableElement)
			Return listFilter(elements, FIELD_KINDS, GetType(VariableElement))
		End Function

		''' <summary>
		''' Returns a set of fields in {@code elements}. </summary>
		''' <returns> a set of fields in {@code elements} </returns>
		''' <param name="elements"> the elements to filter </param>
		Public Shared Function fieldsIn(Of T1 As Element)(ByVal elements As java.util.Set(Of T1)) As java.util.Set(Of VariableElement)
			Return filterter(elements, FIELD_KINDS, GetType(VariableElement))
		End Function

		''' <summary>
		''' Returns a list of constructors in {@code elements}. </summary>
		''' <returns> a list of constructors in {@code elements} </returns>
		''' <param name="elements"> the elements to filter </param>
		Public Shared Function constructorsIn(Of T1 As Element)(ByVal elements As IEnumerable(Of T1)) As IList(Of ExecutableElement)
			Return listFilter(elements, CONSTRUCTOR_KIND, GetType(ExecutableElement))
		End Function

		''' <summary>
		''' Returns a set of constructors in {@code elements}. </summary>
		''' <returns> a set of constructors in {@code elements} </returns>
		''' <param name="elements"> the elements to filter </param>
		Public Shared Function constructorsIn(Of T1 As Element)(ByVal elements As java.util.Set(Of T1)) As java.util.Set(Of ExecutableElement)
			Return filterter(elements, CONSTRUCTOR_KIND, GetType(ExecutableElement))
		End Function

		''' <summary>
		''' Returns a list of methods in {@code elements}. </summary>
		''' <returns> a list of methods in {@code elements} </returns>
		''' <param name="elements"> the elements to filter </param>
		Public Shared Function methodsIn(Of T1 As Element)(ByVal elements As IEnumerable(Of T1)) As IList(Of ExecutableElement)
			Return listFilter(elements, METHOD_KIND, GetType(ExecutableElement))
		End Function

		''' <summary>
		''' Returns a set of methods in {@code elements}. </summary>
		''' <returns> a set of methods in {@code elements} </returns>
		''' <param name="elements"> the elements to filter </param>
		Public Shared Function methodsIn(Of T1 As Element)(ByVal elements As java.util.Set(Of T1)) As java.util.Set(Of ExecutableElement)
			Return filterter(elements, METHOD_KIND, GetType(ExecutableElement))
		End Function

		''' <summary>
		''' Returns a list of types in {@code elements}. </summary>
		''' <returns> a list of types in {@code elements} </returns>
		''' <param name="elements"> the elements to filter </param>
		Public Shared Function typesIn(Of T1 As Element)(ByVal elements As IEnumerable(Of T1)) As IList(Of TypeElement)
			Return listFilter(elements, TYPE_KINDS, GetType(TypeElement))
		End Function

		''' <summary>
		''' Returns a set of types in {@code elements}. </summary>
		''' <returns> a set of types in {@code elements} </returns>
		''' <param name="elements"> the elements to filter </param>
		Public Shared Function typesIn(Of T1 As Element)(ByVal elements As java.util.Set(Of T1)) As java.util.Set(Of TypeElement)
			Return filterter(elements, TYPE_KINDS, GetType(TypeElement))
		End Function

		''' <summary>
		''' Returns a list of packages in {@code elements}. </summary>
		''' <returns> a list of packages in {@code elements} </returns>
		''' <param name="elements"> the elements to filter </param>
		Public Shared Function packagesIn(Of T1 As Element)(ByVal elements As IEnumerable(Of T1)) As IList(Of PackageElement)
			Return listFilter(elements, PACKAGE_KIND, GetType(PackageElement))
		End Function

		''' <summary>
		''' Returns a set of packages in {@code elements}. </summary>
		''' <returns> a set of packages in {@code elements} </returns>
		''' <param name="elements"> the elements to filter </param>
		Public Shared Function packagesIn(Of T1 As Element)(ByVal elements As java.util.Set(Of T1)) As java.util.Set(Of PackageElement)
			Return filterter(elements, PACKAGE_KIND, GetType(PackageElement))
		End Function

		' Assumes targetKinds and E are sensible.
		Private Shared Function listFilter(Of E As Element, T1 As Element)(ByVal elements As IEnumerable(Of T1), ByVal targetKinds As java.util.Set(Of ElementKind), ByVal clazz As Type) As IList(Of E)
			Dim list As IList(Of E) = New List(Of E)
			For Each e As Element In elements
				If targetKinds.contains(e.kind) Then list.Add(clazz.cast(e))
			Next e
			Return list
		End Function

		' Assumes targetKinds and E are sensible.
		Private Shared Function setFilter(Of E As Element, T1 As Element)(ByVal elements As java.util.Set(Of T1), ByVal targetKinds As java.util.Set(Of ElementKind), ByVal clazz As Type) As java.util.Set(Of E)
			' Return set preserving iteration order of input set.
			Dim [set] As java.util.Set(Of E) = New java.util.LinkedHashSet(Of E)
			For Each e As Element In elements
				If targetKinds.contains(e.kind) Then [set].add(clazz.cast(e))
			Next e
			Return [set]
		End Function
	End Class

End Namespace