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
	''' Helper class for executing <a href="package-summary.html#StreamOps">
	''' stream pipelines</a>, capturing all of the information about a stream
	''' pipeline (output shape, intermediate operations, stream flags, parallelism,
	''' etc) in one place.
	''' 
	''' <p>
	''' A {@code PipelineHelper} describes the initial segment of a stream pipeline,
	''' including its source, intermediate operations, and may additionally
	''' incorporate information about the terminal (or stateful) operation which
	''' follows the last intermediate operation described by this
	''' {@code PipelineHelper}. The {@code PipelineHelper} is passed to the
	''' <seealso cref="TerminalOp#evaluateParallel(PipelineHelper, java.util.Spliterator)"/>,
	''' <seealso cref="TerminalOp#evaluateSequential(PipelineHelper, java.util.Spliterator)"/>,
	''' and {@link AbstractPipeline#opEvaluateParallel(PipelineHelper, java.util.Spliterator,
	''' java.util.function.IntFunction)}, methods, which can use the
	''' {@code PipelineHelper} to access information about the pipeline such as
	''' head shape, stream flags, and size, and use the helper methods
	''' such as <seealso cref="#wrapAndCopyInto(Sink, Spliterator)"/>,
	''' <seealso cref="#copyInto(Sink, Spliterator)"/>, and <seealso cref="#wrapSink(Sink)"/> to execute
	''' pipeline operations.
	''' </summary>
	''' @param <P_OUT> type of output elements from the pipeline
	''' @since 1.8 </param>
	Friend MustInherit Class PipelineHelper(Of P_OUT)

		''' <summary>
		''' Gets the stream shape for the source of the pipeline segment.
		''' </summary>
		''' <returns> the stream shape for the source of the pipeline segment. </returns>
		Friend MustOverride ReadOnly Property sourceShape As StreamShape

		''' <summary>
		''' Gets the combined stream and operation flags for the output of the described
		''' pipeline.  This will incorporate stream flags from the stream source, all
		''' the intermediate operations and the terminal operation.
		''' </summary>
		''' <returns> the combined stream and operation flags </returns>
		''' <seealso cref= StreamOpFlag </seealso>
		Friend MustOverride ReadOnly Property streamAndOpFlags As Integer

		''' <summary>
		''' Returns the exact output size of the portion of the output resulting from
		''' applying the pipeline stages described by this {@code PipelineHelper} to
		''' the the portion of the input described by the provided
		''' {@code Spliterator}, if known.  If not known or known infinite, will
		''' return {@code -1}.
		''' 
		''' @apiNote
		''' The exact output size is known if the {@code Spliterator} has the
		''' {@code SIZED} characteristic, and the operation flags
		''' <seealso cref="StreamOpFlag#SIZED"/> is known on the combined stream and operation
		''' flags.
		''' </summary>
		''' <param name="spliterator"> the spliterator describing the relevant portion of the
		'''        source data </param>
		''' <returns> the exact size if known, or -1 if infinite or unknown </returns>
		Friend MustOverride Function exactOutputSizeIfKnown(Of P_IN)(  spliterator As java.util.Spliterator(Of P_IN)) As Long

		''' <summary>
		''' Applies the pipeline stages described by this {@code PipelineHelper} to
		''' the provided {@code Spliterator} and send the results to the provided
		''' {@code Sink}.
		''' 
		''' @implSpec
		''' The implementation behaves as if:
		''' <pre>{@code
		'''     intoWrapped(wrapSink(sink), spliterator);
		''' }</pre>
		''' </summary>
		''' <param name="sink"> the {@code Sink} to receive the results </param>
		''' <param name="spliterator"> the spliterator describing the source input to process </param>
		Friend MustOverride Function wrapAndCopyInto(Of P_IN, S As Sink(Of P_OUT))(  sink As S,   spliterator As java.util.Spliterator(Of P_IN)) As S

		''' <summary>
		''' Pushes elements obtained from the {@code Spliterator} into the provided
		''' {@code Sink}.  If the stream pipeline is known to have short-circuiting
		''' stages in it (see <seealso cref="StreamOpFlag#SHORT_CIRCUIT"/>), the
		''' <seealso cref="Sink#cancellationRequested()"/> is checked after each
		''' element, stopping if cancellation is requested.
		''' 
		''' @implSpec
		''' This method conforms to the {@code Sink} protocol of calling
		''' {@code Sink.begin} before pushing elements, via {@code Sink.accept}, and
		''' calling {@code Sink.end} after all elements have been pushed.
		''' </summary>
		''' <param name="wrappedSink"> the destination {@code Sink} </param>
		''' <param name="spliterator"> the source {@code Spliterator} </param>
		Friend MustOverride Sub copyInto(Of P_IN)(  wrappedSink As Sink(Of P_IN),   spliterator As java.util.Spliterator(Of P_IN))

		''' <summary>
		''' Pushes elements obtained from the {@code Spliterator} into the provided
		''' {@code Sink}, checking <seealso cref="Sink#cancellationRequested()"/> after each
		''' element, and stopping if cancellation is requested.
		''' 
		''' @implSpec
		''' This method conforms to the {@code Sink} protocol of calling
		''' {@code Sink.begin} before pushing elements, via {@code Sink.accept}, and
		''' calling {@code Sink.end} after all elements have been pushed or if
		''' cancellation is requested.
		''' </summary>
		''' <param name="wrappedSink"> the destination {@code Sink} </param>
		''' <param name="spliterator"> the source {@code Spliterator} </param>
		Friend MustOverride Sub copyIntoWithCancel(Of P_IN)(  wrappedSink As Sink(Of P_IN),   spliterator As java.util.Spliterator(Of P_IN))

		''' <summary>
		''' Takes a {@code Sink} that accepts elements of the output type of the
		''' {@code PipelineHelper}, and wrap it with a {@code Sink} that accepts
		''' elements of the input type and implements all the intermediate operations
		''' described by this {@code PipelineHelper}, delivering the result into the
		''' provided {@code Sink}.
		''' </summary>
		''' <param name="sink"> the {@code Sink} to receive the results </param>
		''' <returns> a {@code Sink} that implements the pipeline stages and sends
		'''         results to the provided {@code Sink} </returns>
		Friend MustOverride Function wrapSink(Of P_IN)(  sink As Sink(Of P_OUT)) As Sink(Of P_IN)

		''' 
		''' <param name="spliterator"> </param>
		''' @param <P_IN>
		''' @return </param>
		Friend MustOverride Function wrapSpliterator(Of P_IN)(  spliterator As java.util.Spliterator(Of P_IN)) As java.util.Spliterator(Of P_OUT)

		''' <summary>
		''' Constructs a @{link Node.Builder} compatible with the output shape of
		''' this {@code PipelineHelper}.
		''' </summary>
		''' <param name="exactSizeIfKnown"> if >=0 then a builder will be created that has a
		'''        fixed capacity of exactly sizeIfKnown elements; if < 0 then the
		'''        builder has variable capacity.  A fixed capacity builder will fail
		'''        if an element is added after the builder has reached capacity. </param>
		''' <param name="generator"> a factory function for array instances </param>
		''' <returns> a {@code Node.Builder} compatible with the output shape of this
		'''         {@code PipelineHelper} </returns>
		Friend MustOverride Function makeNodeBuilder(  exactSizeIfKnown As Long,   generator As java.util.function.IntFunction(Of P_OUT())) As Node.Builder(Of P_OUT)

		''' <summary>
		''' Collects all output elements resulting from applying the pipeline stages
		''' to the source {@code Spliterator} into a {@code Node}.
		''' 
		''' @implNote
		''' If the pipeline has no intermediate operations and the source is backed
		''' by a {@code Node} then that {@code Node} will be returned (or flattened
		''' and then returned). This reduces copying for a pipeline consisting of a
		''' stateful operation followed by a terminal operation that returns an
		''' array, such as:
		''' <pre>{@code
		'''     stream.sorted().toArray();
		''' }</pre>
		''' </summary>
		''' <param name="spliterator"> the source {@code Spliterator} </param>
		''' <param name="flatten"> if true and the pipeline is a parallel pipeline then the
		'''        {@code Node} returned will contain no children, otherwise the
		'''        {@code Node} may represent the root in a tree that reflects the
		'''        shape of the computation tree. </param>
		''' <param name="generator"> a factory function for array instances </param>
		''' <returns> the {@code Node} containing all output elements </returns>
		Friend MustOverride Function evaluate(Of P_IN)(  spliterator As java.util.Spliterator(Of P_IN),   flatten As Boolean,   generator As java.util.function.IntFunction(Of P_OUT())) As Node(Of P_OUT)
	End Class

End Namespace