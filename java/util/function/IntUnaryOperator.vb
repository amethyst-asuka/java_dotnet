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
Namespace java.util.function


	''' <summary>
	''' Represents an operation on a single {@code int}-valued operand that produces
	''' an {@code int}-valued result.  This is the primitive type specialization of
	''' <seealso cref="UnaryOperator"/> for {@code int}.
	''' 
	''' <p>This is a <a href="package-summary.html">functional interface</a>
	''' whose functional method is <seealso cref="#applyAsInt(int)"/>.
	''' </summary>
	''' <seealso cref= UnaryOperator
	''' @since 1.8 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Interface IntUnaryOperator

		''' <summary>
		''' Applies this operator to the given operand.
		''' </summary>
		''' <param name="operand"> the operand </param>
		''' <returns> the operator result </returns>
		Function applyAsInt(  operand As Integer) As Integer

		''' <summary>
		''' Returns a composed operator that first applies the {@code before}
		''' operator to its input, and then applies this operator to the result.
		''' If evaluation of either operator throws an exception, it is relayed to
		''' the caller of the composed operator.
		''' </summary>
		''' <param name="before"> the operator to apply before this operator is applied </param>
		''' <returns> a composed operator that first applies the {@code before}
		''' operator and then applies this operator </returns>
		''' <exception cref="NullPointerException"> if before is null
		''' </exception>
		''' <seealso cref= #andThen(IntUnaryOperator) </seealso>
		default Function compose(  before As IntUnaryOperator) As IntUnaryOperator
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(before);
			Sub [New](  v As Integer)

		''' <summary>
		''' Returns a composed operator that first applies this operator to
		''' its input, and then applies the {@code after} operator to the result.
		''' If evaluation of either operator throws an exception, it is relayed to
		''' the caller of the composed operator.
		''' </summary>
		''' <param name="after"> the operator to apply after this operator is applied </param>
		''' <returns> a composed operator that first applies this operator and then
		''' applies the {@code after} operator </returns>
		''' <exception cref="NullPointerException"> if after is null
		''' </exception>
		''' <seealso cref= #compose(IntUnaryOperator) </seealso>
		default Function andThen(  after As IntUnaryOperator) As IntUnaryOperator
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(after);
			Sub [New](  t As Integer)

		''' <summary>
		''' Returns a unary operator that always returns its input argument.
		''' </summary>
		''' <returns> a unary operator that always returns its input argument </returns>
		Shared Function identity() As IntUnaryOperator
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			Return t -> t;
	End Interface

End Namespace