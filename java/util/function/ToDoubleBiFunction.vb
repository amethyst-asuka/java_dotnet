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
	''' Represents a function that accepts two arguments and produces a double-valued
	''' result.  This is the {@code double}-producing primitive specialization for
	''' <seealso cref="BiFunction"/>.
	''' 
	''' <p>This is a <a href="package-summary.html">functional interface</a>
	''' whose functional method is <seealso cref="#applyAsDouble(Object, Object)"/>.
	''' </summary>
	''' @param <T> the type of the first argument to the function </param>
	''' @param <U> the type of the second argument to the function
	''' </param>
	''' <seealso cref= BiFunction
	''' @since 1.8 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Interface ToDoubleBiFunction(Of T, U)

		''' <summary>
		''' Applies this function to the given arguments.
		''' </summary>
		''' <param name="t"> the first function argument </param>
		''' <param name="u"> the second function argument </param>
		''' <returns> the function result </returns>
		Function applyAsDouble(ByVal t As T, ByVal u As U) As Double
	End Interface

End Namespace