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
	''' Represents a function that accepts a long-valued argument and produces an
	''' int-valued result.  This is the {@code long}-to-{@code int} primitive
	''' specialization for <seealso cref="Function"/>.
	''' 
	''' <p>This is a <a href="package-summary.html">functional interface</a>
	''' whose functional method is <seealso cref="#applyAsInt(long)"/>.
	''' </summary>
	''' <seealso cref= Function
	''' @since 1.8 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Interface LongToIntFunction

		''' <summary>
		''' Applies this function to the given argument.
		''' </summary>
		''' <param name="value"> the function argument </param>
		''' <returns> the function result </returns>
		Function applyAsInt(  value As Long) As Integer
	End Interface

End Namespace