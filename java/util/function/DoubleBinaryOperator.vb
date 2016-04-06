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
	''' Represents an operation upon two {@code double}-valued operands and producing a
	''' {@code double}-valued result.   This is the primitive type specialization of
	''' <seealso cref="BinaryOperator"/> for {@code double}.
	''' 
	''' <p>This is a <a href="package-summary.html">functional interface</a>
	''' whose functional method is <seealso cref="#applyAsDouble(double, double)"/>.
	''' </summary>
	''' <seealso cref= BinaryOperator </seealso>
	''' <seealso cref= DoubleUnaryOperator
	''' @since 1.8 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Interface DoubleBinaryOperator
		''' <summary>
		''' Applies this operator to the given operands.
		''' </summary>
		''' <param name="left"> the first operand </param>
		''' <param name="right"> the second operand </param>
		''' <returns> the operator result </returns>
		Function applyAsDouble(  left As Double,   right As Double) As Double
	End Interface

End Namespace