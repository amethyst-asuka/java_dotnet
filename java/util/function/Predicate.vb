'
' * Copyright (c) 2010, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.util.function


	''' <summary>
	''' Represents a predicate (boolean-valued function) of one argument.
	''' 
	''' <p>This is a <a href="package-summary.html">functional interface</a>
	''' whose functional method is <seealso cref="#test(Object)"/>.
	''' </summary>
	''' @param <T> the type of the input to the predicate
	''' 
	''' @since 1.8 </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Interface Predicate(Of T)

		''' <summary>
		''' Evaluates this predicate on the given argument.
		''' </summary>
		''' <param name="t"> the input argument </param>
		''' <returns> {@code true} if the input argument matches the predicate,
		''' otherwise {@code false} </returns>
		Function test(ByVal t As T) As Boolean

		''' <summary>
		''' Returns a composed predicate that represents a short-circuiting logical
		''' AND of this predicate and another.  When evaluating the composed
		''' predicate, if this predicate is {@code false}, then the {@code other}
		''' predicate is not evaluated.
		''' 
		''' <p>Any exceptions thrown during evaluation of either predicate are relayed
		''' to the caller; if evaluation of this predicate throws an exception, the
		''' {@code other} predicate will not be evaluated.
		''' </summary>
		''' <param name="other"> a predicate that will be logically-ANDed with this
		'''              predicate </param>
		''' <returns> a composed predicate that represents the short-circuiting logical
		''' AND of this predicate and the {@code other} predicate </returns>
		''' <exception cref="NullPointerException"> if other is null </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		default Function [and](Of T1)(ByVal other As Predicate(Of T1)) As Predicate(Of T)
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(other);
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return (t) -> test(t) && other.test(t);

		''' <summary>
		''' Returns a predicate that represents the logical negation of this
		''' predicate.
		''' </summary>
		''' <returns> a predicate that represents the logical negation of this
		''' predicate </returns>
		default Function negate() As Predicate(Of T)
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return (t) -> !test(t);

		''' <summary>
		''' Returns a composed predicate that represents a short-circuiting logical
		''' OR of this predicate and another.  When evaluating the composed
		''' predicate, if this predicate is {@code true}, then the {@code other}
		''' predicate is not evaluated.
		''' 
		''' <p>Any exceptions thrown during evaluation of either predicate are relayed
		''' to the caller; if evaluation of this predicate throws an exception, the
		''' {@code other} predicate will not be evaluated.
		''' </summary>
		''' <param name="other"> a predicate that will be logically-ORed with this
		'''              predicate </param>
		''' <returns> a composed predicate that represents the short-circuiting logical
		''' OR of this predicate and the {@code other} predicate </returns>
		''' <exception cref="NullPointerException"> if other is null </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		default Function [or](Of T1)(ByVal other As Predicate(Of T1)) As Predicate(Of T)
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(other);
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return (t) -> test(t) || other.test(t);

		''' <summary>
		''' Returns a predicate that tests if two arguments are equal according
		''' to <seealso cref="Objects#equals(Object, Object)"/>.
		''' </summary>
		''' @param <T> the type of arguments to the predicate </param>
		''' <param name="targetRef"> the object reference with which to compare for equality,
		'''               which may be {@code null} </param>
		''' <returns> a predicate that tests if two arguments are equal according
		''' to <seealso cref="Objects#equals(Object, Object)"/> </returns>
		Shared Function isEqual(Of T)(ByVal targetRef As Object) As Predicate(Of T)
			Sub [New](Nothing == ByVal targetRef As )
	End Interface

End Namespace