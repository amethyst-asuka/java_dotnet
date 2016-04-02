Imports System.Diagnostics

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
	''' Abstract base class for "pipeline" classes, which are the core
	''' implementations of the Stream interface and its primitive specializations.
	''' Manages construction and evaluation of stream pipelines.
	''' 
	''' <p>An {@code AbstractPipeline} represents an initial portion of a stream
	''' pipeline, encapsulating a stream source and zero or more intermediate
	''' operations.  The individual {@code AbstractPipeline} objects are often
	''' referred to as <em>stages</em>, where each stage describes either the stream
	''' source or an intermediate operation.
	''' 
	''' <p>A concrete intermediate stage is generally built from an
	''' {@code AbstractPipeline}, a shape-specific pipeline class which extends it
	''' (e.g., {@code IntPipeline}) which is also abstract, and an operation-specific
	''' concrete class which extends that.  {@code AbstractPipeline} contains most of
	''' the mechanics of evaluating the pipeline, and implements methods that will be
	''' used by the operation; the shape-specific classes add helper methods for
	''' dealing with collection of results into the appropriate shape-specific
	''' containers.
	''' 
	''' <p>After chaining a new intermediate operation, or executing a terminal
	''' operation, the stream is considered to be consumed, and no more intermediate
	''' or terminal operations are permitted on this stream instance.
	''' 
	''' @implNote
	''' <p>For sequential streams, and parallel streams without
	''' <a href="package-summary.html#StreamOps">stateful intermediate
	''' operations</a>, parallel streams, pipeline evaluation is done in a single
	''' pass that "jams" all the operations together.  For parallel streams with
	''' stateful operations, execution is divided into segments, where each
	''' stateful operations marks the end of a segment, and each segment is
	''' evaluated separately and the result used as the input to the next
	''' segment.  In all cases, the source data is not consumed until a terminal
	''' operation begins.
	''' </summary>
	''' @param <E_IN>  type of input elements </param>
	''' @param <E_OUT> type of output elements </param>
	''' @param <S> type of the subclass implementing {@code BaseStream}
	''' @since 1.8 </param>
	Friend MustInherit Class AbstractPipeline(Of E_IN, E_OUT, S As BaseStream(Of E_OUT, S))
		Inherits PipelineHelper(Of E_OUT)
		Implements BaseStream(Of E_OUT, S)

		Private Const MSG_STREAM_LINKED As String = "stream has already been operated upon or closed"
		Private Const MSG_CONSUMED As String = "source already consumed or closed"

		''' <summary>
		''' Backlink to the head of the pipeline chain (self if this is the source
		''' stage).
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private ReadOnly sourceStage As AbstractPipeline

		''' <summary>
		''' The "upstream" pipeline, or null if this is the source stage.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private ReadOnly previousStage As AbstractPipeline

		''' <summary>
		''' The operation flags for the intermediate operation represented by this
		''' pipeline object.
		''' </summary>
		Protected Friend ReadOnly sourceOrOpFlags As Integer

		''' <summary>
		''' The next stage in the pipeline, or null if this is the last stage.
		''' Effectively final at the point of linking to the next pipeline.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private nextStage As AbstractPipeline

		''' <summary>
		''' The number of intermediate operations between this pipeline object
		''' and the stream source if sequential, or the previous stateful if parallel.
		''' Valid at the point of pipeline preparation for evaluation.
		''' </summary>
		Private depth As Integer

		''' <summary>
		''' The combined source and operation flags for the source and all operations
		''' up to and including the operation represented by this pipeline object.
		''' Valid at the point of pipeline preparation for evaluation.
		''' </summary>
		Private combinedFlags As Integer

		''' <summary>
		''' The source spliterator. Only valid for the head pipeline.
		''' Before the pipeline is consumed if non-null then {@code sourceSupplier}
		''' must be null. After the pipeline is consumed if non-null then is set to
		''' null.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private sourceSpliterator_Renamed As java.util.Spliterator(Of ?)

		''' <summary>
		''' The source supplier. Only valid for the head pipeline. Before the
		''' pipeline is consumed if non-null then {@code sourceSpliterator} must be
		''' null. After the pipeline is consumed if non-null then is set to null.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private sourceSupplier As java.util.function.Supplier(Of ? As java.util.Spliterator(Of ?))

		''' <summary>
		''' True if this pipeline has been linked or consumed
		''' </summary>
		Private linkedOrConsumed As Boolean

		''' <summary>
		''' True if there are any stateful ops in the pipeline; only valid for the
		''' source stage.
		''' </summary>
		Private sourceAnyStateful As Boolean

		Private sourceCloseAction As Runnable

		''' <summary>
		''' True if pipeline is parallel, otherwise the pipeline is sequential; only
		''' valid for the source stage.
		''' </summary>
		Private parallel_Renamed As Boolean

		''' <summary>
		''' Constructor for the head of a stream pipeline.
		''' </summary>
		''' <param name="source"> {@code Supplier<Spliterator>} describing the stream source </param>
		''' <param name="sourceFlags"> The source flags for the stream source, described in
		''' <seealso cref="StreamOpFlag"/> </param>
		''' <param name="parallel"> True if the pipeline is parallel </param>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Friend Sub New(Of T1 As java.util.Spliterator(Of ?)(ByVal source As java.util.function.Supplier(Of T1), ByVal sourceFlags As Integer, ByVal parallel As Boolean)
			Me.previousStage = Nothing
			Me.sourceSupplier = source
			Me.sourceStage = Me
			Me.sourceOrOpFlags = sourceFlags And StreamOpFlag.STREAM_MASK
			' The following is an optimization of:
			' StreamOpFlag.combineOpFlags(sourceOrOpFlags, StreamOpFlag.INITIAL_OPS_VALUE);
			Me.combinedFlags = (Not(sourceOrOpFlags << 1)) And StreamOpFlag.INITIAL_OPS_VALUE
			Me.depth = 0
			Me.parallel_Renamed = parallel
		End Sub

		''' <summary>
		''' Constructor for the head of a stream pipeline.
		''' </summary>
		''' <param name="source"> {@code Spliterator} describing the stream source </param>
		''' <param name="sourceFlags"> the source flags for the stream source, described in
		''' <seealso cref="StreamOpFlag"/> </param>
		''' <param name="parallel"> {@code true} if the pipeline is parallel </param>
		Friend Sub New(Of T1)(ByVal source As java.util.Spliterator(Of T1), ByVal sourceFlags As Integer, ByVal parallel As Boolean)
			Me.previousStage = Nothing
			Me.sourceSpliterator_Renamed = source
			Me.sourceStage = Me
			Me.sourceOrOpFlags = sourceFlags And StreamOpFlag.STREAM_MASK
			' The following is an optimization of:
			' StreamOpFlag.combineOpFlags(sourceOrOpFlags, StreamOpFlag.INITIAL_OPS_VALUE);
			Me.combinedFlags = (Not(sourceOrOpFlags << 1)) And StreamOpFlag.INITIAL_OPS_VALUE
			Me.depth = 0
			Me.parallel_Renamed = parallel
		End Sub

		''' <summary>
		''' Constructor for appending an intermediate operation stage onto an
		''' existing pipeline.
		''' </summary>
		''' <param name="previousStage"> the upstream pipeline stage </param>
		''' <param name="opFlags"> the operation flags for the new stage, described in
		''' <seealso cref="StreamOpFlag"/> </param>
		Friend Sub New(Of T1)(ByVal previousStage As AbstractPipeline(Of T1), ByVal opFlags As Integer)
			If previousStage.linkedOrConsumed Then Throw New IllegalStateException(MSG_STREAM_LINKED)
			previousStage.linkedOrConsumed = True
			previousStage.nextStage = Me

			Me.previousStage = previousStage
			Me.sourceOrOpFlags = opFlags And StreamOpFlag.OP_MASK
			Me.combinedFlags = StreamOpFlag.combineOpFlags(opFlags, previousStage.combinedFlags)
			Me.sourceStage = previousStage.sourceStage
			If opIsStateful() Then sourceStage.sourceAnyStateful = True
			Me.depth = previousStage.depth + 1
		End Sub


		' Terminal evaluation methods

		''' <summary>
		''' Evaluate the pipeline with a terminal operation to produce a result.
		''' </summary>
		''' @param <R> the type of result </param>
		''' <param name="terminalOp"> the terminal operation to be applied to the pipeline. </param>
		''' <returns> the result </returns>
		Friend Function evaluate(Of R)(ByVal terminalOp As TerminalOp(Of E_OUT, R)) As R
			Debug.Assert(outputShape Is terminalOp.inputShape())
			If linkedOrConsumed Then Throw New IllegalStateException(MSG_STREAM_LINKED)
			linkedOrConsumed = True

			Return If(parallel, terminalOp.evaluateParallel(Me, sourceSpliterator(terminalOp.opFlags)), terminalOp.evaluateSequential(Me, sourceSpliterator(terminalOp.opFlags)))
		End Function

		''' <summary>
		''' Collect the elements output from the pipeline stage.
		''' </summary>
		''' <param name="generator"> the array generator to be used to create array instances </param>
		''' <returns> a flat array-backed Node that holds the collected output elements </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Function evaluateToArrayNode(ByVal generator As java.util.function.IntFunction(Of E_OUT())) As Node(Of E_OUT)
			If linkedOrConsumed Then Throw New IllegalStateException(MSG_STREAM_LINKED)
			linkedOrConsumed = True

			' If the last intermediate operation is stateful then
			' evaluate directly to avoid an extra collection step
			If parallel AndAlso previousStage IsNot Nothing AndAlso opIsStateful() Then
				' Set the depth of this, last, pipeline stage to zero to slice the
				' pipeline such that this operation will not be included in the
				' upstream slice and upstream operations will not be included
				' in this slice
				depth = 0
				Return opEvaluateParallel(previousStage, previousStage.sourceSpliterator(0), generator)
			Else
				Return evaluate(sourceSpliterator(0), True, generator)
			End If
		End Function

		''' <summary>
		''' Gets the source stage spliterator if this pipeline stage is the source
		''' stage.  The pipeline is consumed after this method is called and
		''' returns successfully.
		''' </summary>
		''' <returns> the source stage spliterator </returns>
		''' <exception cref="IllegalStateException"> if this pipeline stage is not the source
		'''         stage. </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Function sourceStageSpliterator() As java.util.Spliterator(Of E_OUT)
			If Me IsNot sourceStage Then Throw New IllegalStateException

			If linkedOrConsumed Then Throw New IllegalStateException(MSG_STREAM_LINKED)
			linkedOrConsumed = True

			If sourceStage.sourceSpliterator_Renamed IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim s As java.util.Spliterator(Of E_OUT) = sourceStage.sourceSpliterator_Renamed
				sourceStage.sourceSpliterator_Renamed = Nothing
				Return s
			ElseIf sourceStage.sourceSupplier IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim s As java.util.Spliterator(Of E_OUT) = CType(sourceStage.sourceSupplier.get(), java.util.Spliterator(Of E_OUT))
				sourceStage.sourceSupplier = Nothing
				Return s
			Else
				Throw New IllegalStateException(MSG_CONSUMED)
			End If
		End Function

		' BaseStream

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overrides Function sequential() As S Implements BaseStream(Of E_OUT, S).sequential
			sourceStage.parallel_Renamed = False
			Return CType(Me, S)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overrides Function parallel() As S Implements BaseStream(Of E_OUT, S).parallel
			sourceStage.parallel_Renamed = True
			Return CType(Me, S)
		End Function

		Public Overrides Sub close() Implements BaseStream(Of E_OUT, S).close
			linkedOrConsumed = True
			sourceSupplier = Nothing
			sourceSpliterator_Renamed = Nothing
			If sourceStage.sourceCloseAction IsNot Nothing Then
				Dim closeAction As Runnable = sourceStage.sourceCloseAction
				sourceStage.sourceCloseAction = Nothing
				closeAction.run()
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overrides Function onClose(ByVal closeHandler As Runnable) As S Implements BaseStream(Of E_OUT, S).onClose
			Dim existingHandler As Runnable = sourceStage.sourceCloseAction
			sourceStage.sourceCloseAction = If(existingHandler Is Nothing, closeHandler, Streams.composeWithExceptions(existingHandler, closeHandler))
			Return CType(Me, S)
		End Function

		' Primitive specialization use co-variant overrides, hence is not final
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overrides Function spliterator() As java.util.Spliterator(Of E_OUT) Implements BaseStream(Of E_OUT, S).spliterator
			If linkedOrConsumed Then Throw New IllegalStateException(MSG_STREAM_LINKED)
			linkedOrConsumed = True

			If Me Is sourceStage Then
				If sourceStage.sourceSpliterator_Renamed IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					Dim s As java.util.Spliterator(Of E_OUT) = CType(sourceStage.sourceSpliterator_Renamed, java.util.Spliterator(Of E_OUT))
					sourceStage.sourceSpliterator_Renamed = Nothing
					Return s
				ElseIf sourceStage.sourceSupplier IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					Dim s As java.util.function.Supplier(Of java.util.Spliterator(Of E_OUT)) = CType(sourceStage.sourceSupplier, java.util.function.Supplier(Of java.util.Spliterator(Of E_OUT)))
					sourceStage.sourceSupplier = Nothing
					Return lazySpliterator(s)
				Else
					Throw New IllegalStateException(MSG_CONSUMED)
				End If
			Else
				Return wrap(Me, () -> sourceSpliterator(0), parallel)
			End If
		End Function

		Public  Overrides ReadOnly Property  parallel As Boolean Implements BaseStream(Of E_OUT, S).isParallel
			Get
				Return sourceStage.parallel_Renamed
			End Get
		End Property


		''' <summary>
		''' Returns the composition of stream flags of the stream source and all
		''' intermediate operations.
		''' </summary>
		''' <returns> the composition of stream flags of the stream source and all
		'''         intermediate operations </returns>
		''' <seealso cref= StreamOpFlag </seealso>
		Friend Property streamFlags As Integer
			Get
				Return StreamOpFlag.toStreamFlags(combinedFlags)
			End Get
		End Property

		''' <summary>
		''' Get the source spliterator for this pipeline stage.  For a sequential or
		''' stateless parallel pipeline, this is the source spliterator.  For a
		''' stateful parallel pipeline, this is a spliterator describing the results
		''' of all computations up to and including the most recent stateful
		''' operation.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private Function sourceSpliterator(ByVal terminalFlags As Integer) As java.util.Spliterator(Of ?)
			' Get the source spliterator of the pipeline
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim spliterator As java.util.Spliterator(Of ?) = Nothing
			If sourceStage.sourceSpliterator_Renamed IsNot Nothing Then
				spliterator = sourceStage.sourceSpliterator_Renamed
				sourceStage.sourceSpliterator_Renamed = Nothing
			ElseIf sourceStage.sourceSupplier IsNot Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				spliterator = CType(sourceStage.sourceSupplier.get(), java.util.Spliterator(Of ?))
				sourceStage.sourceSupplier = Nothing
			Else
				Throw New IllegalStateException(MSG_CONSUMED)
			End If

			If parallel AndAlso sourceStage.sourceAnyStateful Then
				' Adapt the source spliterator, evaluating each stateful op
				' in the pipeline up to and including this pipeline stage.
				' The depth and flags of each pipeline stage are adjusted accordingly.
				Dim depth As Integer = 1
				SuppressWarnings("rawtypes") AbstractPipeline u = sourceStage
				SuppressWarnings("rawtypes") AbstractPipeline p = sourceStage.nextStage
				SuppressWarnings("rawtypes") AbstractPipeline e = Me
				Do While u IsNot e

					Dim thisOpFlags As Integer = p.sourceOrOpFlags
					If p.opIsStateful() Then
						depth = 0

						If StreamOpFlag.SHORT_CIRCUIT.isKnown(thisOpFlags) Then thisOpFlags = thisOpFlags And Not StreamOpFlag.IS_SHORT_CIRCUIT

						spliterator = p.opEvaluateParallelLazy(u, spliterator)

						' Inject or clear SIZED on the source pipeline stage
						' based on the stage's spliterator
						thisOpFlags = If(spliterator.hasCharacteristics(java.util.Spliterator.SIZED), (thisOpFlags And (Not StreamOpFlag.NOT_SIZED)) Or StreamOpFlag.IS_SIZED, (thisOpFlags And (Not StreamOpFlag.IS_SIZED)) Or StreamOpFlag.NOT_SIZED)
					End If
					p.depth = depth
					depth += 1
					p.combinedFlags = StreamOpFlag.combineOpFlags(thisOpFlags, u.combinedFlags)
					u = p
					p = p.nextStage
				Loop
			End If

			If terminalFlags <> 0 Then combinedFlags = StreamOpFlag.combineOpFlags(terminalFlags, combinedFlags)

			Return spliterator
		End Function

		' PipelineHelper

		Friend  Overrides ReadOnly Property  sourceShape As StreamShape
			Get
	'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim p As AbstractPipeline = AbstractPipeline.this
				Do While p.depth > 0
					p = p.previousStage
				Loop
				Return p.outputShape
			End Get
		End Property

		Friend Overrides Function exactOutputSizeIfKnown(Of P_IN)(ByVal spliterator As java.util.Spliterator(Of P_IN)) As Long
			Return If(StreamOpFlag.SIZED.isKnown(streamAndOpFlags), spliterator.exactSizeIfKnown, -1)
		End Function

		Friend Overrides Function wrapAndCopyInto(Of P_IN, S As Sink(Of E_OUT))(ByVal sink As S, ByVal spliterator As java.util.Spliterator(Of P_IN)) As S
			copyInto(wrapSink(java.util.Objects.requireNonNull(sink)), spliterator)
			Return sink
		End Function

		Friend Overrides Sub copyInto(Of P_IN)(ByVal wrappedSink As Sink(Of P_IN), ByVal spliterator As java.util.Spliterator(Of P_IN))
			java.util.Objects.requireNonNull(wrappedSink)

			If Not StreamOpFlag.SHORT_CIRCUIT.isKnown(streamAndOpFlags) Then
				wrappedSink.begin(spliterator.exactSizeIfKnown)
				spliterator.forEachRemaining(wrappedSink)
				wrappedSink.end()
			Else
				copyIntoWithCancel(wrappedSink, spliterator)
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Overrides Sub copyIntoWithCancel(Of P_IN)(ByVal wrappedSink As Sink(Of P_IN), ByVal spliterator As java.util.Spliterator(Of P_IN))
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim p As AbstractPipeline = AbstractPipeline.this
			Do While p.depth > 0
				p = p.previousStage
			Loop
			wrappedSink.begin(spliterator.exactSizeIfKnown)
			p.forEachWithCancel(spliterator, wrappedSink)
			wrappedSink.end()
		End Sub

		Friend  Overrides ReadOnly Property  streamAndOpFlags As Integer
			Get
				Return combinedFlags
			End Get
		End Property

		Friend Property ordered As Boolean
			Get
				Return StreamOpFlag.ORDERED.isKnown(combinedFlags)
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Overrides Function wrapSink(Of P_IN)(ByVal sink As Sink(Of E_OUT)) As Sink(Of P_IN)
			java.util.Objects.requireNonNull(sink)

			SuppressWarnings("rawtypes") AbstractPipeline p=AbstractPipeline.this
			Do While p.depth > 0
				sink = p.opWrapSink(p.previousStage.combinedFlags, sink)
				p=p.previousStage
			Loop
			Return CType(sink, Sink(Of P_IN))
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Overrides Function wrapSpliterator(Of P_IN)(ByVal sourceSpliterator As java.util.Spliterator(Of P_IN)) As java.util.Spliterator(Of E_OUT)
			If depth = 0 Then
				Return CType(sourceSpliterator, java.util.Spliterator(Of E_OUT))
			Else
				Return wrap(Me, () -> sourceSpliterator, parallel)
			End If
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Overrides Function evaluate(Of P_IN)(ByVal spliterator As java.util.Spliterator(Of P_IN), ByVal flatten As Boolean, ByVal generator As java.util.function.IntFunction(Of E_OUT())) As Node(Of E_OUT)
			If parallel Then
				' @@@ Optimize if op of this pipeline stage is a stateful op
				Return evaluateToNode(Me, spliterator, flatten, generator)
			Else
				Dim nb As Node.Builder(Of E_OUT) = makeNodeBuilder(exactOutputSizeIfKnown(spliterator), generator)
				Return wrapAndCopyInto(nb, spliterator).build()
			End If
		End Function


		' Shape-specific abstract methods, implemented by XxxPipeline classes

		''' <summary>
		''' Get the output shape of the pipeline.  If the pipeline is the head,
		''' then it's output shape corresponds to the shape of the source.
		''' Otherwise, it's output shape corresponds to the output shape of the
		''' associated operation.
		''' </summary>
		''' <returns> the output shape </returns>
		Friend MustOverride ReadOnly Property outputShape As StreamShape

		''' <summary>
		''' Collect elements output from a pipeline into a Node that holds elements
		''' of this shape.
		''' </summary>
		''' <param name="helper"> the pipeline helper describing the pipeline stages </param>
		''' <param name="spliterator"> the source spliterator </param>
		''' <param name="flattenTree"> true if the returned node should be flattened </param>
		''' <param name="generator"> the array generator </param>
		''' <returns> a Node holding the output of the pipeline </returns>
		Friend MustOverride Function evaluateToNode(Of P_IN)(ByVal helper As PipelineHelper(Of E_OUT), ByVal spliterator As java.util.Spliterator(Of P_IN), ByVal flattenTree As Boolean, ByVal generator As java.util.function.IntFunction(Of E_OUT())) As Node(Of E_OUT)

		''' <summary>
		''' Create a spliterator that wraps a source spliterator, compatible with
		''' this stream shape, and operations associated with a {@link
		''' PipelineHelper}.
		''' </summary>
		''' <param name="ph"> the pipeline helper describing the pipeline stages </param>
		''' <param name="supplier"> the supplier of a spliterator </param>
		''' <returns> a wrapping spliterator compatible with this shape </returns>
		Friend MustOverride Function wrap(Of P_IN)(ByVal ph As PipelineHelper(Of E_OUT), ByVal supplier As java.util.function.Supplier(Of java.util.Spliterator(Of P_IN)), ByVal isParallel As Boolean) As java.util.Spliterator(Of E_OUT)

		''' <summary>
		''' Create a lazy spliterator that wraps and obtains the supplied the
		''' spliterator when a method is invoked on the lazy spliterator. </summary>
		''' <param name="supplier"> the supplier of a spliterator </param>
		Friend MustOverride Function lazySpliterator(Of T1 As java.util.Spliterator(Of E_OUT)(ByVal supplier As java.util.function.Supplier(Of T1)) As java.util.Spliterator(Of E_OUT)

		''' <summary>
		''' Traverse the elements of a spliterator compatible with this stream shape,
		''' pushing those elements into a sink.   If the sink requests cancellation,
		''' no further elements will be pulled or pushed.
		''' </summary>
		''' <param name="spliterator"> the spliterator to pull elements from </param>
		''' <param name="sink"> the sink to push elements to </param>
		Friend MustOverride Sub forEachWithCancel(ByVal spliterator As java.util.Spliterator(Of E_OUT), ByVal sink As Sink(Of E_OUT))

		''' <summary>
		''' Make a node builder compatible with this stream shape.
		''' </summary>
		''' <param name="exactSizeIfKnown"> if {@literal >=0}, then a node builder will be
		''' created that has a fixed capacity of at most sizeIfKnown elements. If
		''' {@literal < 0}, then the node builder has an unfixed capacity. A fixed
		''' capacity node builder will throw exceptions if an element is added after
		''' builder has reached capacity, or is built before the builder has reached
		''' capacity.
		''' </param>
		''' <param name="generator"> the array generator to be used to create instances of a
		''' T[] array. For implementations supporting primitive nodes, this parameter
		''' may be ignored. </param>
		''' <returns> a node builder </returns>
		Friend MustOverride Overrides Function makeNodeBuilder(ByVal exactSizeIfKnown As Long, ByVal generator As java.util.function.IntFunction(Of E_OUT())) As Node.Builder(Of E_OUT)


		' Op-specific abstract methods, implemented by the operation class

		''' <summary>
		''' Returns whether this operation is stateful or not.  If it is stateful,
		''' then the method
		''' <seealso cref="#opEvaluateParallel(PipelineHelper, java.util.Spliterator, java.util.function.IntFunction)"/>
		''' must be overridden.
		''' </summary>
		''' <returns> {@code true} if this operation is stateful </returns>
		Friend MustOverride Function opIsStateful() As Boolean

		''' <summary>
		''' Accepts a {@code Sink} which will receive the results of this operation,
		''' and return a {@code Sink} which accepts elements of the input type of
		''' this operation and which performs the operation, passing the results to
		''' the provided {@code Sink}.
		''' 
		''' @apiNote
		''' The implementation may use the {@code flags} parameter to optimize the
		''' sink wrapping.  For example, if the input is already {@code DISTINCT},
		''' the implementation for the {@code Stream#distinct()} method could just
		''' return the sink it was passed.
		''' </summary>
		''' <param name="flags"> The combined stream and operation flags up to, but not
		'''        including, this operation </param>
		''' <param name="sink"> sink to which elements should be sent after processing </param>
		''' <returns> a sink which accepts elements, perform the operation upon
		'''         each element, and passes the results (if any) to the provided
		'''         {@code Sink}. </returns>
		Friend MustOverride Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of E_OUT)) As Sink(Of E_IN)

		''' <summary>
		''' Performs a parallel evaluation of the operation using the specified
		''' {@code PipelineHelper} which describes the upstream intermediate
		''' operations.  Only called on stateful operations.  If {@link
		''' #opIsStateful()} returns true then implementations must override the
		''' default implementation.
		''' 
		''' @implSpec The default implementation always throw
		''' {@code UnsupportedOperationException}.
		''' </summary>
		''' <param name="helper"> the pipeline helper describing the pipeline stages </param>
		''' <param name="spliterator"> the source {@code Spliterator} </param>
		''' <param name="generator"> the array generator </param>
		''' <returns> a {@code Node} describing the result of the evaluation </returns>
		 Friend Overridable Function opEvaluateParallel(Of P_IN)(ByVal helper As PipelineHelper(Of E_OUT), ByVal spliterator As java.util.Spliterator(Of P_IN), ByVal generator As java.util.function.IntFunction(Of E_OUT())) As Node(Of E_OUT)
			Throw New UnsupportedOperationException("Parallel evaluation is not supported")
		 End Function

		''' <summary>
		''' Returns a {@code Spliterator} describing a parallel evaluation of the
		''' operation, using the specified {@code PipelineHelper} which describes the
		''' upstream intermediate operations.  Only called on stateful operations.
		''' It is not necessary (though acceptable) to do a full computation of the
		''' result here; it is preferable, if possible, to describe the result via a
		''' lazily evaluated spliterator.
		''' 
		''' @implSpec The default implementation behaves as if:
		''' <pre>{@code
		'''     return evaluateParallel(helper, i -> (E_OUT[]) new
		''' Object[i]).spliterator();
		''' }</pre>
		''' and is suitable for implementations that cannot do better than a full
		''' synchronous evaluation.
		''' </summary>
		''' <param name="helper"> the pipeline helper </param>
		''' <param name="spliterator"> the source {@code Spliterator} </param>
		''' <returns> a {@code Spliterator} describing the result of the evaluation </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		 Friend Overridable Function opEvaluateParallelLazy(Of P_IN)(ByVal helper As PipelineHelper(Of E_OUT), ByVal spliterator As java.util.Spliterator(Of P_IN)) As java.util.Spliterator(Of E_OUT)
			Return opEvaluateParallel(helper, spliterator, i -> (E_OUT()) New Object(i - 1){}).spliterator()
		 End Function
	End Class

End Namespace