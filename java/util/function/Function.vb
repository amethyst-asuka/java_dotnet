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
	''' Represents a function that accepts one argument and produces a result.
	''' 
	''' <p>This is a <a href="package-summary.html">functional interface</a>
	''' whose functional method is <seealso cref="#apply(Object)"/>.
	''' </summary>
	''' @param <T> the type of the input to the function </param>
	''' @param <R> the type of the result of the function
	''' 
	''' @since 1.8 </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Interface [Function](Of T, R)

		''' <summary>
		''' Applies this function to the given argument.
		''' </summary>
		''' <param name="t"> the function argument </param>
		''' <returns> the function result </returns>
		Function apply(  t As T) As R

		''' <summary>
		''' Returns a composed function that first applies the {@code before}
		''' function to its input, and then applies this function to the result.
		''' If evaluation of either function throws an exception, it is relayed to
		''' the caller of the composed function.
		''' </summary>
		''' @param <V> the type of input to the {@code before} function, and to the
		'''           composed function </param>
		''' <param name="before"> the function to apply before this function is applied </param>
		''' <returns> a composed function that first applies the {@code before}
		''' function and then applies this function </returns>
		''' <exception cref="NullPointerException"> if before is null
		''' </exception>
		''' <seealso cref= #andThen(Function) </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		default <V> Function<V, R> compose(Function<? MyBase V, ? extends T> before)
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(before);
			Sub [New](  v As V)

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
		''' <exception cref="NullPointerException"> if after is null
		''' </exception>
		''' <seealso cref= #compose(Function) </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		default <V> Function<T, V> andThen(Function<? MyBase R, ? extends V> after)
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			java.util.Objects.requireNonNull(after);
			Sub [New](  t As T)

		''' <summary>
		''' Returns a function that always returns its input argument.
		''' </summary>
		''' @param <T> the type of the input and output objects to the function </param>
		''' <returns> a function that always returns its input argument </returns>
		Shared Function identity(Of T)() As [Function](Of T, T)
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			Return t -> t;
	End Interface

End Namespace