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
	''' Represents a predicate (boolean-valued function) of two arguments.  This is
	''' the two-arity specialization of <seealso cref="Predicate"/>.
	''' 
	''' <p>This is a <a href="package-summary.html">functional interface</a>
	''' whose functional method is <seealso cref="#test(Object, Object)"/>.
	''' </summary>
	''' @param <T> the type of the first argument to the predicate </param>
	''' @param <U> the type of the second argument the predicate
	''' </param>
	''' <seealso cref= Predicate
	''' @since 1.8 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Interface BiPredicate(Of T, U)

		''' <summary>
		''' Evaluates this predicate on the given arguments.
		''' </summary>
		''' <param name="t"> the first input argument </param>
		''' <param name="u"> the second input argument </param>
		''' <returns> {@code true} if the input arguments match the predicate,
		''' otherwise {@code false} </returns>
		Function test(  t As T,   u As U) As Boolean

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
		default Function [and](Of T1)(  other As BiPredicate(Of T1)) As BiPredicate(Of T, U)
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(other);
			Sub [New](  t As T,   u As U)

		''' <summary>
		''' Returns a predicate that represents the logical negation of this
		''' predicate.
		''' </summary>
		''' <returns> a predicate that represents the logical negation of this
		''' predicate </returns>
		default Function negate() As BiPredicate(Of T, U)
			Sub [New](  t As T,   u As U)

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
		default Function [or](Of T1)(  other As BiPredicate(Of T1)) As BiPredicate(Of T, U)
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(other);
			Sub [New](  t As T,   u As U)
	End Interface

End Namespace