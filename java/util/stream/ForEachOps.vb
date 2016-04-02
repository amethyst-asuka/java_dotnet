Imports System
Imports System.Collections.Concurrent

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
	''' Factory for creating instances of {@code TerminalOp} that perform an
	''' action for every element of a stream.  Supported variants include unordered
	''' traversal (elements are provided to the {@code Consumer} as soon as they are
	''' available), and ordered traversal (elements are provided to the
	''' {@code Consumer} in encounter order.)
	''' 
	''' <p>Elements are provided to the {@code Consumer} on whatever thread and
	''' whatever order they become available.  For ordered traversals, it is
	''' guaranteed that processing an element <em>happens-before</em> processing
	''' subsequent elements in the encounter order.
	''' 
	''' <p>Exceptions occurring as a result of sending an element to the
	''' {@code Consumer} will be relayed to the caller and traversal will be
	''' prematurely terminated.
	''' 
	''' @since 1.8
	''' </summary>
	Friend NotInheritable Class ForEachOps

		Private Sub New()
		End Sub

		''' <summary>
		''' Constructs a {@code TerminalOp} that perform an action for every element
		''' of a stream.
		''' </summary>
		''' <param name="action"> the {@code Consumer} that receives all elements of a
		'''        stream </param>
		''' <param name="ordered"> whether an ordered traversal is requested </param>
		''' @param <T> the type of the stream elements </param>
		''' <returns> the {@code TerminalOp} instance </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Shared Function makeRef(Of T, T1)(ByVal action As java.util.function.Consumer(Of T1), ByVal ordered As Boolean) As TerminalOp(Of T, Void)
			java.util.Objects.requireNonNull(action)
			Return New ForEachOp.OfRef(Of )(action, ordered)
		End Function

		''' <summary>
		''' Constructs a {@code TerminalOp} that perform an action for every element
		''' of an {@code IntStream}.
		''' </summary>
		''' <param name="action"> the {@code IntConsumer} that receives all elements of a
		'''        stream </param>
		''' <param name="ordered"> whether an ordered traversal is requested </param>
		''' <returns> the {@code TerminalOp} instance </returns>
		Public Shared Function makeInt(ByVal action As java.util.function.IntConsumer, ByVal ordered As Boolean) As TerminalOp(Of Integer?, Void)
			java.util.Objects.requireNonNull(action)
			Return New ForEachOp.OfInt(action, ordered)
		End Function

		''' <summary>
		''' Constructs a {@code TerminalOp} that perform an action for every element
		''' of a {@code LongStream}.
		''' </summary>
		''' <param name="action"> the {@code LongConsumer} that receives all elements of a
		'''        stream </param>
		''' <param name="ordered"> whether an ordered traversal is requested </param>
		''' <returns> the {@code TerminalOp} instance </returns>
		Public Shared Function makeLong(ByVal action As java.util.function.LongConsumer, ByVal ordered As Boolean) As TerminalOp(Of Long?, Void)
			java.util.Objects.requireNonNull(action)
			Return New ForEachOp.OfLong(action, ordered)
		End Function

		''' <summary>
		''' Constructs a {@code TerminalOp} that perform an action for every element
		''' of a {@code DoubleStream}.
		''' </summary>
		''' <param name="action"> the {@code DoubleConsumer} that receives all elements of
		'''        a stream </param>
		''' <param name="ordered"> whether an ordered traversal is requested </param>
		''' <returns> the {@code TerminalOp} instance </returns>
		Public Shared Function makeDouble(ByVal action As java.util.function.DoubleConsumer, ByVal ordered As Boolean) As TerminalOp(Of Double?, Void)
			java.util.Objects.requireNonNull(action)
			Return New ForEachOp.OfDouble(action, ordered)
		End Function

		''' <summary>
		''' A {@code TerminalOp} that evaluates a stream pipeline and sends the
		''' output to itself as a {@code TerminalSink}.  Elements will be sent in
		''' whatever thread they become available.  If the traversal is unordered,
		''' they will be sent independent of the stream's encounter order.
		''' 
		''' <p>This terminal operation is stateless.  For parallel evaluation, each
		''' leaf instance of a {@code ForEachTask} will send elements to the same
		''' {@code TerminalSink} reference that is an instance of this class.
		''' </summary>
		''' @param <T> the output type of the stream pipeline </param>
		Friend MustInherit Class ForEachOp(Of T)
			Implements TerminalOp(Of T, Void), TerminalSink(Of T, Void)

			Private ReadOnly ordered As Boolean

			Protected Friend Sub New(ByVal ordered As Boolean)
				Me.ordered = ordered
			End Sub

			' TerminalOp

			Public  Overrides ReadOnly Property  opFlags As Integer Implements TerminalOp(Of T, Void).getOpFlags
				Get
					Return If(ordered, 0, StreamOpFlag.NOT_ORDERED)
				End Get
			End Property

			Public Overrides Function evaluateSequential(Of S)(ByVal helper As PipelineHelper(Of T), ByVal spliterator As java.util.Spliterator(Of S)) As Void
				Return helper.wrapAndCopyInto(Me, spliterator).get()
			End Function

			Public Overrides Function evaluateParallel(Of S)(ByVal helper As PipelineHelper(Of T), ByVal spliterator As java.util.Spliterator(Of S)) As Void
				If ordered Then
					CType(New ForEachOrderedTask(Of )(helper, spliterator, Me), ForEachOrderedTask(Of )).invoke()
				Else
					CType(New ForEachTask(Of )(helper, spliterator, helper.wrapSink(Me)), ForEachTask(Of )).invoke()
				End If
				Return Nothing
			End Function

			' TerminalSink

			Public Overrides Function [get]() As Void
				Return Nothing
			End Function

			' Implementations

			''' <summary>
			''' Implementation class for reference streams </summary>
			Friend NotInheritable Class OfRef(Of T)
				Inherits ForEachOp(Of T)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Friend ReadOnly consumer As java.util.function.Consumer(Of ?)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
				Friend Sub New(Of T1)(ByVal consumer As java.util.function.Consumer(Of T1), ByVal ordered As Boolean)
					MyBase.New(ordered)
					Me.consumer = consumer
				End Sub

				Public Overrides Sub accept(ByVal t As T)
					consumer.accept(t)
				End Sub
			End Class

			''' <summary>
			''' Implementation class for {@code IntStream} </summary>
			Friend NotInheritable Class OfInt
				Inherits ForEachOp(Of Integer?)
				Implements Sink.OfInt

				Friend ReadOnly consumer As java.util.function.IntConsumer

				Friend Sub New(ByVal consumer As java.util.function.IntConsumer, ByVal ordered As Boolean)
					MyBase.New(ordered)
					Me.consumer = consumer
				End Sub

				Public Overrides Function inputShape() As StreamShape
					Return StreamShape.INT_VALUE
				End Function

				Public Overrides Sub accept(ByVal t As Integer)
					consumer.accept(t)
				End Sub
			End Class

			''' <summary>
			''' Implementation class for {@code LongStream} </summary>
			Friend NotInheritable Class OfLong
				Inherits ForEachOp(Of Long?)
				Implements Sink.OfLong

				Friend ReadOnly consumer As java.util.function.LongConsumer

				Friend Sub New(ByVal consumer As java.util.function.LongConsumer, ByVal ordered As Boolean)
					MyBase.New(ordered)
					Me.consumer = consumer
				End Sub

				Public Overrides Function inputShape() As StreamShape
					Return StreamShape.LONG_VALUE
				End Function

				Public Overrides Sub accept(ByVal t As Long)
					consumer.accept(t)
				End Sub
			End Class

			''' <summary>
			''' Implementation class for {@code DoubleStream} </summary>
			Friend NotInheritable Class OfDouble
				Inherits ForEachOp(Of Double?)
				Implements Sink.OfDouble

				Friend ReadOnly consumer As java.util.function.DoubleConsumer

				Friend Sub New(ByVal consumer As java.util.function.DoubleConsumer, ByVal ordered As Boolean)
					MyBase.New(ordered)
					Me.consumer = consumer
				End Sub

				Public Overrides Function inputShape() As StreamShape
					Return StreamShape.DOUBLE_VALUE
				End Function

				Public Overrides Sub accept(ByVal t As Double)
					consumer.accept(t)
				End Sub
			End Class
		End Class

		''' <summary>
		''' A {@code ForkJoinTask} for performing a parallel for-each operation </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class ForEachTask(Of S, T)
			Inherits java.util.concurrent.CountedCompleter(Of Void)

			Private spliterator As java.util.Spliterator(Of S)
			Private ReadOnly sink As Sink(Of S)
			Private ReadOnly helper As PipelineHelper(Of T)
			Private targetSize As Long

			Friend Sub New(ByVal helper As PipelineHelper(Of T), ByVal spliterator As java.util.Spliterator(Of S), ByVal sink As Sink(Of S))
				MyBase.New(Nothing)
				Me.sink = sink
				Me.helper = helper
				Me.spliterator = spliterator
				Me.targetSize = 0L
			End Sub

			Friend Sub New(ByVal parent As ForEachTask(Of S, T), ByVal spliterator As java.util.Spliterator(Of S))
				MyBase.New(parent)
				Me.spliterator = spliterator
				Me.sink = parent.sink
				Me.targetSize = parent.targetSize
				Me.helper = parent.helper
			End Sub

			' Similar to AbstractTask but doesn't need to track child tasks
			Public Sub compute()
				Dim rightSplit As java.util.Spliterator(Of S) = spliterator, leftSplit As java.util.Spliterator(Of S)
				Dim sizeEstimate As Long = rightSplit.estimateSize(), sizeThreshold As Long
				sizeThreshold = targetSize
				If sizeThreshold = 0L Then
						sizeThreshold = AbstractTask.suggestTargetSize(sizeEstimate)
						targetSize = sizeThreshold
				End If
				Dim isShortCircuit As Boolean = StreamOpFlag.SHORT_CIRCUIT.isKnown(helper.streamAndOpFlags)
				Dim forkRight As Boolean = False
				Dim taskSink As Sink(Of S) = sink
				Dim task As ForEachTask(Of S, T) = Me
				Do While (Not isShortCircuit) OrElse Not taskSink.cancellationRequested()
					leftSplit = rightSplit.trySplit()
					If sizeEstimate <= sizeThreshold OrElse leftSplit Is Nothing Then
						task.helper.copyInto(taskSink, rightSplit)
						Exit Do
					End If
					Dim leftTask As New ForEachTask(Of S, T)(task, leftSplit)
					task.addToPendingCount(1)
					Dim taskToFork As ForEachTask(Of S, T)
					If forkRight Then
						forkRight = False
						rightSplit = leftSplit
						taskToFork = task
						task = leftTask
					Else
						forkRight = True
						taskToFork = leftTask
					End If
					taskToFork.fork()
					sizeEstimate = rightSplit.estimateSize()
				Loop
				task.spliterator = Nothing
				task.propagateCompletion()
			End Sub
		End Class

		''' <summary>
		''' A {@code ForkJoinTask} for performing a parallel for-each operation
		''' which visits the elements in encounter order
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend NotInheritable Class ForEachOrderedTask(Of S, T)
			Inherits java.util.concurrent.CountedCompleter(Of Void)

	'        
	'         * Our goal is to ensure that the elements associated with a task are
	'         * processed according to an in-order traversal of the computation tree.
	'         * We use completion counts for representing these dependencies, so that
	'         * a task does not complete until all the tasks preceding it in this
	'         * order complete.  We use the "completion map" to associate the next
	'         * task in this order for any left child.  We increase the pending count
	'         * of any node on the right side of such a mapping by one to indicate
	'         * its dependency, and when a node on the left side of such a mapping
	'         * completes, it decrements the pending count of its corresponding right
	'         * side.  As the computation tree is expanded by splitting, we must
	'         * atomically update the mappings to maintain the invariant that the
	'         * completion map maps left children to the next node in the in-order
	'         * traversal.
	'         *
	'         * Take, for example, the following computation tree of tasks:
	'         *
	'         *       a
	'         *      / \
	'         *     b   c
	'         *    / \ / \
	'         *   d  e f  g
	'         *
	'         * The complete map will contain (not necessarily all at the same time)
	'         * the following associations:
	'         *
	'         *   d -> e
	'         *   b -> f
	'         *   f -> g
	'         *
	'         * Tasks e, f, g will have their pending counts increased by 1.
	'         *
	'         * The following relationships hold:
	'         *
	'         *   - completion of d "happens-before" e;
	'         *   - completion of d and e "happens-before b;
	'         *   - completion of b "happens-before" f; and
	'         *   - completion of f "happens-before" g
	'         *
	'         * Thus overall the "happens-before" relationship holds for the
	'         * reporting of elements, covered by tasks d, e, f and g, as specified
	'         * by the forEachOrdered operation.
	'         

			Private ReadOnly helper As PipelineHelper(Of T)
			Private spliterator As java.util.Spliterator(Of S)
			Private ReadOnly targetSize As Long
			Private ReadOnly completionMap As ConcurrentDictionary(Of ForEachOrderedTask(Of S, T), ForEachOrderedTask(Of S, T))
			Private ReadOnly action As Sink(Of T)
			Private ReadOnly leftPredecessor As ForEachOrderedTask(Of S, T)
			Private node As Node(Of T)

			Protected Friend Sub New(ByVal helper As PipelineHelper(Of T), ByVal spliterator As java.util.Spliterator(Of S), ByVal action As Sink(Of T))
				MyBase.New(Nothing)
				Me.helper = helper
				Me.spliterator = spliterator
				Me.targetSize = AbstractTask.suggestTargetSize(spliterator.estimateSize())
				' Size map to avoid concurrent re-sizes
				Me.completionMap = New ConcurrentDictionary(Of ) (System.Math.Max(16, AbstractTask.LEAF_TARGET << 1))
				Me.action = action
				Me.leftPredecessor = Nothing
			End Sub

			Friend Sub New(ByVal parent As ForEachOrderedTask(Of S, T), ByVal spliterator As java.util.Spliterator(Of S), ByVal leftPredecessor As ForEachOrderedTask(Of S, T))
				MyBase.New(parent)
				Me.helper = parent.helper
				Me.spliterator = spliterator
				Me.targetSize = parent.targetSize
				Me.completionMap = parent.completionMap
				Me.action = parent.action
				Me.leftPredecessor = leftPredecessor
			End Sub

			Public Overrides Sub compute()
				doCompute(Me)
			End Sub

			Private Shared Sub doCompute(Of S, T)(ByVal task As ForEachOrderedTask(Of S, T))
				Dim rightSplit As java.util.Spliterator(Of S) = task.spliterator, leftSplit As java.util.Spliterator(Of S)
				Dim sizeThreshold As Long = task.targetSize
				Dim forkRight As Boolean = False
				leftSplit = rightSplit.trySplit()
				Do While rightSplit.estimateSize() > sizeThreshold AndAlso leftSplit IsNot Nothing
					Dim leftChild As New ForEachOrderedTask(Of S, T)(task, leftSplit, task.leftPredecessor)
					Dim rightChild As New ForEachOrderedTask(Of S, T)(task, rightSplit, leftChild)

					' Fork the parent task
					' Completion of the left and right children "happens-before"
					' completion of the parent
					task.addToPendingCount(1)
					' Completion of the left child "happens-before" completion of
					' the right child
					rightChild.addToPendingCount(1)
					task.completionMap.put(leftChild, rightChild)

					' If task is not on the left spine
					If task.leftPredecessor IsNot Nothing Then
	'                    
	'                     * Completion of left-predecessor, or left subtree,
	'                     * "happens-before" completion of left-most leaf node of
	'                     * right subtree.
	'                     * The left child's pending count needs to be updated before
	'                     * it is associated in the completion map, otherwise the
	'                     * left child can complete prematurely and violate the
	'                     * "happens-before" constraint.
	'                     
						leftChild.addToPendingCount(1)
						' Update association of left-predecessor to left-most
						' leaf node of right subtree
						If task.completionMap.replace(task.leftPredecessor, task, leftChild) Then
							' If replaced, adjust the pending count of the parent
							' to complete when its children complete
							task.addToPendingCount(-1)
						Else
							' Left-predecessor has already completed, parent's
							' pending count is adjusted by left-predecessor;
							' left child is ready to complete
							leftChild.addToPendingCount(-1)
						End If
					End If

					Dim taskToFork As ForEachOrderedTask(Of S, T)
					If forkRight Then
						forkRight = False
						rightSplit = leftSplit
						task = leftChild
						taskToFork = rightChild
					Else
						forkRight = True
						task = rightChild
						taskToFork = leftChild
					End If
					taskToFork.fork()
					leftSplit = rightSplit.trySplit()
				Loop

	'            
	'             * Task's pending count is either 0 or 1.  If 1 then the completion
	'             * map will contain a value that is task, and two calls to
	'             * tryComplete are required for completion, one below and one
	'             * triggered by the completion of task's left-predecessor in
	'             * onCompletion.  Therefore there is no data race within the if
	'             * block.
	'             
				If task.pendingCount > 0 Then
					' Cannot complete just yet so buffer elements into a Node
					' for use when completion occurs
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					Dim generator As java.util.function.IntFunction(Of T()) = size -> (T()) New Object(size - 1){}
					Dim nb As Node.Builder(Of T) = task.helper.makeNodeBuilder(task.helper.exactOutputSizeIfKnown(rightSplit), generator)
					task.node = task.helper.wrapAndCopyInto(nb, rightSplit).build()
					task.spliterator = Nothing
				End If
				task.tryComplete()
			End Sub

			Public Overrides Sub onCompletion(Of T1)(ByVal caller As java.util.concurrent.CountedCompleter(Of T1))
				If node IsNot Nothing Then
					' Dump buffered elements from this leaf into the sink
					node.forEach(action)
					node = Nothing
				ElseIf spliterator IsNot Nothing Then
					' Dump elements output from this leaf's pipeline into the sink
					helper.wrapAndCopyInto(action, spliterator)
					spliterator = Nothing
				End If

				' The completion of this task *and* the dumping of elements
				' "happens-before" completion of the associated left-most leaf task
				' of right subtree (if any, which can be this task's right sibling)
				'
				Dim leftDescendant As ForEachOrderedTask(Of S, T) = completionMap.Remove(Me)
				If leftDescendant IsNot Nothing Then leftDescendant.tryComplete()
			End Sub
		End Class
	End Class

End Namespace