Imports javax.lang.model.element

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
	''' Represents a type in the Java programming language.
	''' Types include primitive types, declared types (class and interface types),
	''' array types, type variables, and the null type.
	''' Also represented are wildcard type arguments,
	''' the signature and return types of executables,
	''' and pseudo-types corresponding to packages and to the keyword {@code void}.
	''' 
	''' <p> Types should be compared using the utility methods in {@link
	''' Types}.  There is no guarantee that any particular type will always
	''' be represented by the same object.
	''' 
	''' <p> To implement operations based on the class of an {@code
	''' TypeMirror} object, either use a <seealso cref="TypeVisitor visitor"/>
	''' or use the result of the <seealso cref="#getKind"/> method.  Using {@code
	''' instanceof} is <em>not</em> necessarily a reliable idiom for
	''' determining the effective class of an object in this modeling
	''' hierarchy since an implementation may choose to have a single
	''' object implement multiple {@code TypeMirror} subinterfaces.
	''' 
	''' @author Joseph D. Darcy
	''' @author Scott Seligman
	''' @author Peter von der Ah&eacute; </summary>
	''' <seealso cref= Element </seealso>
	''' <seealso cref= Types
	''' @since 1.6 </seealso>
	Public Interface TypeMirror
		Inherits javax.lang.model.AnnotatedConstruct

		''' <summary>
		''' Returns the {@code kind} of this type.
		''' </summary>
		''' <returns> the kind of this type </returns>
		ReadOnly Property kind As TypeKind

		''' <summary>
		''' Obeys the general contract of <seealso cref="Object#equals Object.equals"/>.
		''' This method does not, however, indicate whether two types represent
		''' the same type.
		''' Semantic comparisons of type equality should instead use
		''' <seealso cref="Types#isSameType(TypeMirror, TypeMirror)"/>.
		''' The results of {@code t1.equals(t2)} and
		''' {@code Types.isSameType(t1, t2)} may differ.
		''' </summary>
		''' <param name="obj">  the object to be compared with this type </param>
		''' <returns> {@code true} if the specified object is equal to this one </returns>
		Function Equals(ByVal obj As Object) As Boolean

		''' <summary>
		''' Obeys the general contract of <seealso cref="Object#hashCode Object.hashCode"/>.
		''' </summary>
		''' <seealso cref= #equals </seealso>
		Function GetHashCode() As Integer

		''' <summary>
		''' Returns an informative string representation of this type.  If
		''' possible, the string should be of a form suitable for
		''' representing this type in source code.  Any names embedded in
		''' the result are qualified if possible.
		''' </summary>
		''' <returns> a string representation of this type </returns>
		Function ToString() As String

		''' <summary>
		''' Applies a visitor to this type.
		''' </summary>
		''' @param <R> the return type of the visitor's methods </param>
		''' @param <P> the type of the additional parameter to the visitor's methods </param>
		''' <param name="v">   the visitor operating on this type </param>
		''' <param name="p">   additional parameter to the visitor </param>
		''' <returns> a visitor-specified result </returns>
		 Function accept(Of R, P)(ByVal v As TypeVisitor(Of R, P), ByVal p As P) As R
	End Interface

End Namespace