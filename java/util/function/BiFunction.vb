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
	''' Represents a function that accepts two arguments and produces a result.
	''' This is the two-arity specialization of <seealso cref="Function"/>.
	''' 
	''' <p>This is a <a href="package-summary.html">functional interface</a>
	''' whose functional method is <seealso cref="#apply(Object, Object)"/>.
	''' </summary>
	''' @param <T> the type of the first argument to the function </param>
	''' @param <U> the type of the second argument to the function </param>
	''' @param <R> the type of the result of the function
	''' </param>
	''' <seealso cref= Function
	''' @since 1.8 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Interface BiFunction(Of T, U, R)

		''' <summary>
		''' Applies this function to the given arguments.
		''' </summary>
		''' <param name="t"> the first function argument </param>
		''' <param name="u"> the second function argument </param>
		''' <returns> the function result </returns>
		Function apply(ByVal t As T, ByVal u As U) As R

		''' <summary>
		''' Returns a composed function that first applies this function to
		''' its input, and then applies the {@code after} function to the result.
		''' If evaluation of either function throws an exception, it is relayed to
		''' the caller of the composed function.
		''' </summary>
		''' @param <V> the type of output of the {@code after} function, and of the
		'''           composed function </param>
		''' <param name="after"> the function to apply after this function is applied </param>
		''' <returns> a composed function that first applies this function and then
		''' applies the {@code after} function </returns>
		''' <exception cref="NullPointerException"> if after is null </exception>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		default <V> BiFunction<T, U, V> andThen(Function<? MyBase R, ? extends V> after)
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(after);
			Sub [New](ByVal t As T, ByVal u As U)
	End Interface

End Namespace