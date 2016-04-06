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
	''' Represents an operation upon two operands of the same type, producing a result
	''' of the same type as the operands.  This is a specialization of
	''' <seealso cref="BiFunction"/> for the case where the operands and the result are all of
	''' the same type.
	''' 
	''' <p>This is a <a href="package-summary.html">functional interface</a>
	''' whose functional method is <seealso cref="#apply(Object, Object)"/>.
	''' </summary>
	''' @param <T> the type of the operands and result of the operator
	''' </param>
	''' <seealso cref= BiFunction </seealso>
	''' <seealso cref= UnaryOperator
	''' @since 1.8 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Interface BinaryOperator(Of T)
		Inherits BiFunction(Of T, T, T)

		''' <summary>
		''' Returns a <seealso cref="BinaryOperator"/> which returns the lesser of two elements
		''' according to the specified {@code Comparator}.
		''' </summary>
		''' @param <T> the type of the input arguments of the comparator </param>
		''' <param name="comparator"> a {@code Comparator} for comparing the two values </param>
		''' <returns> a {@code BinaryOperator} which returns the lesser of its operands,
		'''         according to the supplied {@code Comparator} </returns>
		''' <exception cref="NullPointerException"> if the argument is null </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Shared Function minBy(Of T, T1)(  comparator As IComparer(Of T1)) As BinaryOperator(Of T)
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(comparator);
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return (a, b) -> comparator.compare(a, b) <= 0 ? a : b;

		''' <summary>
		''' Returns a <seealso cref="BinaryOperator"/> which returns the greater of two elements
		''' according to the specified {@code Comparator}.
		''' </summary>
		''' @param <T> the type of the input arguments of the comparator </param>
		''' <param name="comparator"> a {@code Comparator} for comparing the two values </param>
		''' <returns> a {@code BinaryOperator} which returns the greater of its operands,
		'''         according to the supplied {@code Comparator} </returns>
		''' <exception cref="NullPointerException"> if the argument is null </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Shared Function maxBy(Of T, T1)(  comparator As IComparer(Of T1)) As BinaryOperator(Of T)
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(comparator);
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return (a, b) -> comparator.compare(a, b) >= 0 ? a : b;
	End Interface

End Namespace