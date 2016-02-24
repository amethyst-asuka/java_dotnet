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
	''' Represents an operation that accepts two input arguments and returns no
	''' result.  This is the two-arity specialization of <seealso cref="Consumer"/>.
	''' Unlike most other functional interfaces, {@code BiConsumer} is expected
	''' to operate via side-effects.
	''' 
	''' <p>This is a <a href="package-summary.html">functional interface</a>
	''' whose functional method is <seealso cref="#accept(Object, Object)"/>.
	''' </summary>
	''' @param <T> the type of the first argument to the operation </param>
	''' @param <U> the type of the second argument to the operation
	''' </param>
	''' <seealso cref= Consumer
	''' @since 1.8 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Interface BiConsumer(Of T, U)

		''' <summary>
		''' Performs this operation on the given arguments.
		''' </summary>
		''' <param name="t"> the first input argument </param>
		''' <param name="u"> the second input argument </param>
		Sub accept(ByVal t As T, ByVal u As U)

		''' <summary>
		''' Returns a composed {@code BiConsumer} that performs, in sequence, this
		''' operation followed by the {@code after} operation. If performing either
		''' operation throws an exception, it is relayed to the caller of the
		''' composed operation.  If performing this operation throws an exception,
		''' the {@code after} operation will not be performed.
		''' </summary>
		''' <param name="after"> the operation to perform after this operation </param>
		''' <returns> a composed {@code BiConsumer} that performs in sequence this
		''' operation followed by the {@code after} operation </returns>
		''' <exception cref="NullPointerException"> if {@code after} is null </exception>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		default Function andThen(Of T1)(ByVal after As BiConsumer(Of T1)) As BiConsumer(Of T, U)
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(after);

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return (l, r) ->
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				accept(l, r);
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
				after.accept(l, r);
	End Interface

End Namespace