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
Namespace java.util.stream


	''' <summary>
	''' A <seealso cref="Sink"/> which accumulates state as elements are accepted, and allows
	''' a result to be retrieved after the computation is finished.
	''' </summary>
	''' @param <T> the type of elements to be accepted </param>
	''' @param <R> the type of the result
	''' 
	''' @since 1.8 </param>
	Friend Interface TerminalSink(Of T, R)
		Inherits Sink(Of T), java.util.function.Supplier(Of R)

	End Interface

End Namespace