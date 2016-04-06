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
	''' Represents an operation that accepts a single input argument and returns no
	''' result. Unlike most other functional interfaces, {@code Consumer} is expected
	''' to operate via side-effects.
	''' 
	''' <p>This is a <a href="package-summary.html">functional interface</a>
	''' whose functional method is <seealso cref="#accept(Object)"/>.
	''' </summary>
	''' @param <T> the type of the input to the operation
	''' 
	''' @since 1.8 </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Interface Consumer(Of T)

		''' <summary>
		''' Performs this operation on the given argument.
		''' </summary>
		''' <param name="t"> the input argument </param>
		Sub accept(  t As T)

		''' <summary>
		''' Returns a composed {@code Consumer} that performs, in sequence, this
		''' operation followed by the {@code after} operation. If performing either
		''' operation throws an exception, it is relayed to the caller of the
		''' composed operation.  If performing this operation throws an exception,
		''' the {@code after} operation will not be performed.
		''' </summary>
		''' <param name="after"> the operation to perform after this operation </param>
		''' <returns> a composed {@code Consumer} that performs in sequence this
		''' operation followed by the {@code after} operation </returns>
		''' <exception cref="NullPointerException"> if {@code after} is null </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		default Function andThen(Of T1)(  after As Consumer(Of T1)) As Consumer(Of T)
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(after);
			Sub [New](  t As T)
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				accept(t);
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				after.accept(t);
	End Interface

End Namespace