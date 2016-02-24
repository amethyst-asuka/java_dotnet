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
	''' Represents an operation on a single operand that produces a result of the
	''' same type as its operand.  This is a specialization of {@code Function} for
	''' the case where the operand and result are of the same type.
	''' 
	''' <p>This is a <a href="package-summary.html">functional interface</a>
	''' whose functional method is <seealso cref="#apply(Object)"/>.
	''' </summary>
	''' @param <T> the type of the operand and result of the operator
	''' </param>
	''' <seealso cref= Function
	''' @since 1.8 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Interface UnaryOperator(Of T)
		Inherits [Function](Of T, T)

		''' <summary>
		''' Returns a unary operator that always returns its input argument.
		''' </summary>
		''' @param <T> the type of the input and output of the operator </param>
		''' <returns> a unary operator that always returns its input argument </returns>
		Shared Function identity(Of T)() As UnaryOperator(Of T)
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			Return t -> t;
	End Interface

End Namespace