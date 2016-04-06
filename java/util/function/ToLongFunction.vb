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
	''' Represents a function that produces a long-valued result.  This is the
	''' {@code long}-producing primitive specialization for <seealso cref="Function"/>.
	''' 
	''' <p>This is a <a href="package-summary.html">functional interface</a>
	''' whose functional method is <seealso cref="#applyAsLong(Object)"/>.
	''' </summary>
	''' @param <T> the type of the input to the function
	''' </param>
	''' <seealso cref= Function
	''' @since 1.8 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Interface ToLongFunction(Of T)

		''' <summary>
		''' Applies this function to the given argument.
		''' </summary>
		''' <param name="value"> the function argument </param>
		''' <returns> the function result </returns>
		Function applyAsLong(  value As T) As Long
	End Interface

End Namespace