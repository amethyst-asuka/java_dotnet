'
' * Copyright (c) 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Represents an operation that accepts an object-valued and a
	''' {@code double}-valued argument, and returns no result.  This is the
	''' {@code (reference, double)} specialization of <seealso cref="BiConsumer"/>.
	''' Unlike most other functional interfaces, {@code ObjDoubleConsumer} is
	''' expected to operate via side-effects.
	''' 
	''' <p>This is a <a href="package-summary.html">functional interface</a>
	''' whose functional method is <seealso cref="#accept(Object, double)"/>.
	''' </summary>
	''' @param <T> the type of the object argument to the operation
	''' </param>
	''' <seealso cref= BiConsumer
	''' @since 1.8 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Interface ObjDoubleConsumer(Of T)

		''' <summary>
		''' Performs this operation on the given arguments.
		''' </summary>
		''' <param name="t"> the first input argument </param>
		''' <param name="value"> the second input argument </param>
		Sub accept(  t As T,   value As Double)
	End Interface

End Namespace