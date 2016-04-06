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
	''' An operation in a stream pipeline that takes a stream as input and produces
	''' a result or side-effect.  A {@code TerminalOp} has an input type and stream
	''' shape, and a result type.  A {@code TerminalOp} also has a set of
	''' <em>operation flags</em> that describes how the operation processes elements
	''' of the stream (such as short-circuiting or respecting encounter order; see
	''' <seealso cref="StreamOpFlag"/>).
	''' 
	''' <p>A {@code TerminalOp} must provide a sequential and parallel implementation
	''' of the operation relative to a given stream source and set of intermediate
	''' operations.
	''' </summary>
	''' @param <E_IN> the type of input elements </param>
	''' @param <R>    the type of the result
	''' @since 1.8 </param>
	Friend Interface TerminalOp(Of E_IN, R)
		''' <summary>
		''' Gets the shape of the input type of this operation.
		''' 
		''' @implSpec The default returns {@code StreamShape.REFERENCE}.
		''' </summary>
		''' <returns> StreamShape of the input type of this operation </returns>
		default Function inputShape() As StreamShape
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			Return StreamShape.REFERENCE;

		''' <summary>
		''' Gets the stream flags of the operation.  Terminal operations may set a
		''' limited subset of the stream flags defined in <seealso cref="StreamOpFlag"/>, and
		''' these flags are combined with the previously combined stream and
		''' intermediate operation flags for the pipeline.
		''' 
		''' @implSpec The default implementation returns zero.
		''' </summary>
		''' <returns> the stream flags for this operation </returns>
		''' <seealso cref= StreamOpFlag </seealso>
		ReadOnly Property default opFlags As Integer
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			Return 0;

		''' <summary>
		''' Performs a parallel evaluation of the operation using the specified
		''' {@code PipelineHelper}, which describes the upstream intermediate
		''' operations.
		''' 
		''' @implSpec The default performs a sequential evaluation of the operation
		''' using the specified {@code PipelineHelper}.
		''' </summary>
		''' <param name="helper"> the pipeline helper </param>
		''' <param name="spliterator"> the source spliterator </param>
		''' <returns> the result of the evaluation </returns>
		default Function evaluateParallel(  helper As PipelineHelper(Of E_IN),   spliterator As java.util.Spliterator(Of P_IN)) As R(Of P_IN)
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			if (Tripwire.ENABLED)
				Sub [New](getClass()    As , "{0} triggering TerminalOp.evaluateParallel serial default"    As )
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Return evaluateSequential(helper, spliterator);

		''' <summary>
		''' Performs a sequential evaluation of the operation using the specified
		''' {@code PipelineHelper}, which describes the upstream intermediate
		''' operations.
		''' </summary>
		''' <param name="helper"> the pipeline helper </param>
		''' <param name="spliterator"> the source spliterator </param>
		''' <returns> the result of the evaluation </returns>
		 Function evaluateSequential(Of P_IN)(  helper As PipelineHelper(Of E_IN),   spliterator As java.util.Spliterator(Of P_IN)) As R
	End Interface

End Namespace