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
	''' Represents a predicate (boolean-valued function) of one {@code double}-valued
	''' argument. This is the {@code double}-consuming primitive type specialization
	''' of <seealso cref="Predicate"/>.
	''' 
	''' <p>This is a <a href="package-summary.html">functional interface</a>
	''' whose functional method is <seealso cref="#test(double)"/>.
	''' </summary>
	''' <seealso cref= Predicate
	''' @since 1.8 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Interface DoublePredicate

		''' <summary>
		''' Evaluates this predicate on the given argument.
		''' </summary>
		''' <param name="value"> the input argument </param>
		''' <returns> {@code true} if the input argument matches the predicate,
		''' otherwise {@code false} </returns>
		Function test(  value As Double) As Boolean

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
		default Function [and](  other As DoublePredicate) As DoublePredicate
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(other);
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return (value) -> test(value) && other.test(value);

		''' <summary>
		''' Returns a predicate that represents the logical negation of this
		''' predicate.
		''' </summary>
		''' <returns> a predicate that represents the logical negation of this
		''' predicate </returns>
		default Function negate() As DoublePredicate
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return (value) -> !test(value);

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
		default Function [or](  other As DoublePredicate) As DoublePredicate
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(other);
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return (value) -> test(value) || other.test(value);
	End Interface

End Namespace