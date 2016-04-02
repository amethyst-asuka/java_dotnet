Imports System
Imports System.Diagnostics
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
	''' Spliterator implementations for wrapping and delegating spliterators, used
	''' in the implementation of the <seealso cref="Stream#spliterator()"/> method.
	''' 
	''' @since 1.8
	''' </summary>
	Friend Class StreamSpliterators

		''' <summary>
		''' Abstract wrapping spliterator that binds to the spliterator of a
		''' pipeline helper on first operation.
		''' 
		''' <p>This spliterator is not late-binding and will bind to the source
		''' spliterator when first operated on.
		''' 
		''' <p>A wrapping spliterator produced from a sequential stream
		''' cannot be split if there are stateful operations present.
		''' </summary>
		Private MustInherit Class AbstractWrappingSpliterator(Of P_IN, P_OUT, T_BUFFER As AbstractSpinedBuffer)
			Implements java.util.Spliterator(Of P_OUT)

			' @@@ Detect if stateful operations are present or not
			'     If not then can split otherwise cannot

			''' <summary>
			''' True if this spliterator supports splitting
			''' </summary>
			Friend ReadOnly isParallel As Boolean

			Friend ReadOnly ph As PipelineHelper(Of P_OUT)

			''' <summary>
			''' Supplier for the source spliterator.  Client provides either a
			''' spliterator or a supplier.
			''' </summary>
			Private spliteratorSupplier As java.util.function.Supplier(Of java.util.Spliterator(Of P_IN))

			''' <summary>
			''' Source spliterator.  Either provided from client or obtained from
			''' supplier.
			''' </summary>
			Friend spliterator As java.util.Spliterator(Of P_IN)

			''' <summary>
			''' Sink chain for the downstream stages of the pipeline, ultimately
			''' leading to the buffer. Used during partial traversal.
			''' </summary>
			Friend bufferSink As Sink(Of P_IN)

			''' <summary>
			''' A function that advances one element of the spliterator, pushing
			''' it to bufferSink.  Returns whether any elements were processed.
			''' Used during partial traversal.
			''' </summary>
			Friend pusher As java.util.function.BooleanSupplier

			''' <summary>
			''' Next element to consume from the buffer, used during partial traversal </summary>
			Friend nextToConsume As Long

			''' <summary>
			''' Buffer into which elements are pushed.  Used during partial traversal. </summary>
			Friend buffer As T_BUFFER

			''' <summary>
			''' True if full traversal has occurred (with possible cancelation).
			''' If doing a partial traversal, there may be still elements in buffer.
			''' </summary>
			Friend finished As Boolean

			''' <summary>
			''' Construct an AbstractWrappingSpliterator from a
			''' {@code Supplier<Spliterator>}.
			''' </summary>
			Friend Sub New(ByVal ph As PipelineHelper(Of P_OUT), ByVal spliteratorSupplier As java.util.function.Supplier(Of java.util.Spliterator(Of P_IN)), ByVal parallel As Boolean)
				Me.ph = ph
				Me.spliteratorSupplier = spliteratorSupplier
				Me.spliterator = Nothing
				Me.isParallel = parallel
			End Sub

			''' <summary>
			''' Construct an AbstractWrappingSpliterator from a
			''' {@code Spliterator}.
			''' </summary>
			Friend Sub New(ByVal ph As PipelineHelper(Of P_OUT), ByVal spliterator As java.util.Spliterator(Of P_IN), ByVal parallel As Boolean)
				Me.ph = ph
				Me.spliteratorSupplier = Nothing
				Me.spliterator = spliterator
				Me.isParallel = parallel
			End Sub

			''' <summary>
			''' Called before advancing to set up spliterator, if needed.
			''' </summary>
			Friend Sub init()
				If spliterator Is Nothing Then
					spliterator = spliteratorSupplier.get()
					spliteratorSupplier = Nothing
				End If
			End Sub

			''' <summary>
			''' Get an element from the source, pushing it into the sink chain,
			''' setting up the buffer if needed </summary>
			''' <returns> whether there are elements to consume from the buffer </returns>
			Friend Function doAdvance() As Boolean
				If buffer Is Nothing Then
					If finished Then Return False

					init()
					initPartialTraversalState()
					nextToConsume = 0
					bufferSink.begin(spliterator.exactSizeIfKnown)
					Return fillBuffer()
				Else
					nextToConsume += 1
					Dim hasNext As Boolean = nextToConsume < buffer.count()
					If Not hasNext Then
						nextToConsume = 0
						buffer.clear()
						hasNext = fillBuffer()
					End If
					Return hasNext
				End If
			End Function

			''' <summary>
			''' Invokes the shape-specific constructor with the provided arguments
			''' and returns the result.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend MustOverride Function wrap(ByVal s As java.util.Spliterator(Of P_IN)) As AbstractWrappingSpliterator(Of P_IN, P_OUT, ?)

			''' <summary>
			''' Initializes buffer, sink chain, and pusher for a shape-specific
			''' implementation.
			''' </summary>
			Friend MustOverride Sub initPartialTraversalState()

			Public Overrides Function trySplit() As java.util.Spliterator(Of P_OUT)
				If isParallel AndAlso (Not finished) Then
					init()

					Dim Split As java.util.Spliterator(Of P_IN) = spliterator.trySplit()
					Return If(Split Is Nothing, Nothing, wrap(Split))
				Else
					Return Nothing
				End If
			End Function

			''' <summary>
			''' If the buffer is empty, push elements into the sink chain until
			''' the source is empty or cancellation is requested. </summary>
			''' <returns> whether there are elements to consume from the buffer </returns>
			Private Function fillBuffer() As Boolean
				Do While buffer.count() = 0
					If bufferSink.cancellationRequested() OrElse (Not pusher.asBoolean) Then
						If finished Then
							Return False
						Else
							bufferSink.end() ' might trigger more elements
							finished = True
						End If
					End If
				Loop
				Return True
			End Function

			Public Overrides Function estimateSize() As Long
				init()
				' Use the estimate of the wrapped spliterator
				' Note this may not be accurate if there are filter/flatMap
				' operations filtering or adding elements to the stream
				Return spliterator.estimateSize()
			End Function

			Public  Overrides ReadOnly Property  exactSizeIfKnown As Long
				Get
					init()
					Return If(StreamOpFlag.SIZED.isKnown(ph.streamAndOpFlags), spliterator.exactSizeIfKnown, -1)
				End Get
			End Property

			Public Overrides Function characteristics() As Integer
				init()

				' Get the characteristics from the pipeline
				Dim c As Integer = StreamOpFlag.toCharacteristics(StreamOpFlag.toStreamFlags(ph.streamAndOpFlags))

				' Mask off the size and uniform characteristics and replace with
				' those of the spliterator
				' Note that a non-uniform spliterator can change from something
				' with an exact size to an estimate for a sub-split, for example
				' with HashSet where the size is known at the top level spliterator
				' but for sub-splits only an estimate is known
				If (c And java.util.Spliterator.SIZED) <> 0 Then
					c = c And Not(java.util.Spliterator.SIZED Or java.util.Spliterator.SUBSIZED)
					c = c Or (spliterator.characteristics() And (java.util.Spliterator.SIZED Or java.util.Spliterator.SUBSIZED))
				End If

				Return c
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public  Overrides ReadOnly Property  comparator As IComparer(Of ?)
				Get
					If Not hasCharacteristics(SORTED) Then Throw New IllegalStateException
					Return Nothing
				End Get
			End Property

			Public NotOverridable Overrides Function ToString() As String
				Return String.Format("{0}[{1}]", Me.GetType().name, spliterator)
			End Function
		End Class

		Friend NotInheritable Class WrappingSpliterator(Of P_IN, P_OUT)
			Inherits AbstractWrappingSpliterator(Of P_IN, P_OUT, SpinedBuffer(Of P_OUT))

			Friend Sub New(ByVal ph As PipelineHelper(Of P_OUT), ByVal supplier As java.util.function.Supplier(Of java.util.Spliterator(Of P_IN)), ByVal parallel As Boolean)
				MyBase.New(ph, supplier, parallel)
			End Sub

			Friend Sub New(ByVal ph As PipelineHelper(Of P_OUT), ByVal spliterator As java.util.Spliterator(Of P_IN), ByVal parallel As Boolean)
				MyBase.New(ph, spliterator, parallel)
			End Sub

			Friend Overrides Function wrap(ByVal s As java.util.Spliterator(Of P_IN)) As WrappingSpliterator(Of P_IN, P_OUT)
				Return New WrappingSpliterator(Of )(ph, s, isParallel)
			End Function

			Friend Overrides Sub initPartialTraversalState()
				Dim b As New SpinedBuffer(Of P_OUT)
				buffer = b
				bufferSink = ph.wrapSink(b::accept)
				pusher = () -> spliterator.tryAdvance(bufferSink)
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Function tryAdvance(Of T1)(ByVal consumer As java.util.function.Consumer(Of T1)) As Boolean
				java.util.Objects.requireNonNull(consumer)
				Dim hasNext As Boolean = doAdvance()
				If hasNext Then consumer.accept(buffer.get(nextToConsume))
				Return hasNext
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub forEachRemaining(Of T1)(ByVal consumer As java.util.function.Consumer(Of T1))
				If buffer Is Nothing AndAlso (Not finished) Then
					java.util.Objects.requireNonNull(consumer)
					init()

					ph.wrapAndCopyInto(CType(consumer, Sink(Of P_OUT))::accept, spliterator)
					finished = True
				Else
					Do
					Loop While tryAdvance(consumer)
				End If
			End Sub
		End Class

		Friend NotInheritable Class IntWrappingSpliterator(Of P_IN)
			Inherits AbstractWrappingSpliterator(Of P_IN, Integer?, SpinedBuffer.OfInt)
			Implements java.util.Spliterator.OfInt

			Friend Sub New(ByVal ph As PipelineHelper(Of Integer?), ByVal supplier As java.util.function.Supplier(Of java.util.Spliterator(Of P_IN)), ByVal parallel As Boolean)
				MyBase.New(ph, supplier, parallel)
			End Sub

			Friend Sub New(ByVal ph As PipelineHelper(Of Integer?), ByVal spliterator As java.util.Spliterator(Of P_IN), ByVal parallel As Boolean)
				MyBase.New(ph, spliterator, parallel)
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend Overrides Function wrap(ByVal s As java.util.Spliterator(Of P_IN)) As AbstractWrappingSpliterator(Of P_IN, Integer?, ?)
				Return New IntWrappingSpliterator(Of )(ph, s, isParallel)
			End Function

			Friend Overrides Sub initPartialTraversalState()
				Dim b As New SpinedBuffer.OfInt
				buffer = b
				bufferSink = ph.wrapSink(CType(b, Sink.OfInt)::accept)
				pusher = () -> spliterator.tryAdvance(bufferSink)
			End Sub

			Public Overrides Function trySplit() As java.util.Spliterator.OfInt
				Return CType(MyBase.trySplit(), java.util.Spliterator.OfInt)
			End Function

			Public Overrides Function tryAdvance(ByVal consumer As java.util.function.IntConsumer) As Boolean
				java.util.Objects.requireNonNull(consumer)
				Dim hasNext As Boolean = doAdvance()
				If hasNext Then consumer.accept(buffer.get(nextToConsume))
				Return hasNext
			End Function

			Public Overrides Sub forEachRemaining(ByVal consumer As java.util.function.IntConsumer)
				If buffer Is Nothing AndAlso (Not finished) Then
					java.util.Objects.requireNonNull(consumer)
					init()

					ph.wrapAndCopyInto(CType(consumer, Sink.OfInt)::accept, spliterator)
					finished = True
				Else
					Do
					Loop While tryAdvance(consumer)
				End If
			End Sub
		End Class

		Friend NotInheritable Class LongWrappingSpliterator(Of P_IN)
			Inherits AbstractWrappingSpliterator(Of P_IN, Long?, SpinedBuffer.OfLong)
			Implements java.util.Spliterator.OfLong

			Friend Sub New(ByVal ph As PipelineHelper(Of Long?), ByVal supplier As java.util.function.Supplier(Of java.util.Spliterator(Of P_IN)), ByVal parallel As Boolean)
				MyBase.New(ph, supplier, parallel)
			End Sub

			Friend Sub New(ByVal ph As PipelineHelper(Of Long?), ByVal spliterator As java.util.Spliterator(Of P_IN), ByVal parallel As Boolean)
				MyBase.New(ph, spliterator, parallel)
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend Overrides Function wrap(ByVal s As java.util.Spliterator(Of P_IN)) As AbstractWrappingSpliterator(Of P_IN, Long?, ?)
				Return New LongWrappingSpliterator(Of )(ph, s, isParallel)
			End Function

			Friend Overrides Sub initPartialTraversalState()
				Dim b As New SpinedBuffer.OfLong
				buffer = b
				bufferSink = ph.wrapSink(CType(b, Sink.OfLong)::accept)
				pusher = () -> spliterator.tryAdvance(bufferSink)
			End Sub

			Public Overrides Function trySplit() As java.util.Spliterator.OfLong
				Return CType(MyBase.trySplit(), java.util.Spliterator.OfLong)
			End Function

			Public Overrides Function tryAdvance(ByVal consumer As java.util.function.LongConsumer) As Boolean
				java.util.Objects.requireNonNull(consumer)
				Dim hasNext As Boolean = doAdvance()
				If hasNext Then consumer.accept(buffer.get(nextToConsume))
				Return hasNext
			End Function

			Public Overrides Sub forEachRemaining(ByVal consumer As java.util.function.LongConsumer)
				If buffer Is Nothing AndAlso (Not finished) Then
					java.util.Objects.requireNonNull(consumer)
					init()

					ph.wrapAndCopyInto(CType(consumer, Sink.OfLong)::accept, spliterator)
					finished = True
				Else
					Do
					Loop While tryAdvance(consumer)
				End If
			End Sub
		End Class

		Friend NotInheritable Class DoubleWrappingSpliterator(Of P_IN)
			Inherits AbstractWrappingSpliterator(Of P_IN, Double?, SpinedBuffer.OfDouble)
			Implements java.util.Spliterator.OfDouble

			Friend Sub New(ByVal ph As PipelineHelper(Of Double?), ByVal supplier As java.util.function.Supplier(Of java.util.Spliterator(Of P_IN)), ByVal parallel As Boolean)
				MyBase.New(ph, supplier, parallel)
			End Sub

			Friend Sub New(ByVal ph As PipelineHelper(Of Double?), ByVal spliterator As java.util.Spliterator(Of P_IN), ByVal parallel As Boolean)
				MyBase.New(ph, spliterator, parallel)
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend Overrides Function wrap(ByVal s As java.util.Spliterator(Of P_IN)) As AbstractWrappingSpliterator(Of P_IN, Double?, ?)
				Return New DoubleWrappingSpliterator(Of )(ph, s, isParallel)
			End Function

			Friend Overrides Sub initPartialTraversalState()
				Dim b As New SpinedBuffer.OfDouble
				buffer = b
				bufferSink = ph.wrapSink(CType(b, Sink.OfDouble)::accept)
				pusher = () -> spliterator.tryAdvance(bufferSink)
			End Sub

			Public Overrides Function trySplit() As java.util.Spliterator.OfDouble
				Return CType(MyBase.trySplit(), java.util.Spliterator.OfDouble)
			End Function

			Public Overrides Function tryAdvance(ByVal consumer As java.util.function.DoubleConsumer) As Boolean
				java.util.Objects.requireNonNull(consumer)
				Dim hasNext As Boolean = doAdvance()
				If hasNext Then consumer.accept(buffer.get(nextToConsume))
				Return hasNext
			End Function

			Public Overrides Sub forEachRemaining(ByVal consumer As java.util.function.DoubleConsumer)
				If buffer Is Nothing AndAlso (Not finished) Then
					java.util.Objects.requireNonNull(consumer)
					init()

					ph.wrapAndCopyInto(CType(consumer, Sink.OfDouble)::accept, spliterator)
					finished = True
				Else
					Do
					Loop While tryAdvance(consumer)
				End If
			End Sub
		End Class

		''' <summary>
		''' Spliterator implementation that delegates to an underlying spliterator,
		''' acquiring the spliterator from a {@code Supplier<Spliterator>} on the
		''' first call to any spliterator method. </summary>
		''' @param <T> </param>
		Friend Class DelegatingSpliterator(Of T, T_SPLITR As java.util.Spliterator(Of T))
			Implements java.util.Spliterator(Of T)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Private ReadOnly supplier As java.util.function.Supplier(Of ? As T_SPLITR)

			Private s As T_SPLITR

			Friend Sub New(Of T1 As T_SPLITR)(ByVal supplier As java.util.function.Supplier(Of T1))
				Me.supplier = supplier
			End Sub

			Friend Overridable Function [get]() As T_SPLITR
				If s Is Nothing Then s = supplier.get()
				Return s
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Overrides Function trySplit() As T_SPLITR
				Return CType([get]().trySplit(), T_SPLITR)
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Function tryAdvance(Of T1)(ByVal consumer As java.util.function.Consumer(Of T1)) As Boolean
				Return [get]().tryAdvance(consumer)
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub forEachRemaining(Of T1)(ByVal consumer As java.util.function.Consumer(Of T1))
				[get]().forEachRemaining(consumer)
			End Sub

			Public Overrides Function estimateSize() As Long
				Return [get]().estimateSize()
			End Function

			Public Overrides Function characteristics() As Integer
				Return [get]().characteristics()
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public  Overrides ReadOnly Property  comparator As IComparer(Of ?)
				Get
					Return [get]().comparator
				End Get
			End Property

			Public  Overrides ReadOnly Property  exactSizeIfKnown As Long
				Get
					Return [get]().exactSizeIfKnown
				End Get
			End Property

			Public Overrides Function ToString() As String
				Return Me.GetType().name & "[" & [get]() & "]"
			End Function

			Friend Class OfPrimitive(Of T, T_CONS, T_SPLITR As java.util.Spliterator.OfPrimitive(Of T, T_CONS, T_SPLITR))
				Inherits DelegatingSpliterator(Of T, T_SPLITR)
				Implements java.util.Spliterator.OfPrimitive(Of T, T_CONS, T_SPLITR)

				Friend Sub New(Of T1 As T_SPLITR)(ByVal supplier As java.util.function.Supplier(Of T1))
					MyBase.New(supplier)
				End Sub

				Public Overrides Function tryAdvance(ByVal consumer As T_CONS) As Boolean
					Return outerInstance.get().tryAdvance(consumer)
				End Function

				Public Overrides Sub forEachRemaining(ByVal consumer As T_CONS)
					outerInstance.get().forEachRemaining(consumer)
				End Sub
			End Class

			Friend NotInheritable Class OfInt
				Inherits OfPrimitive(Of Integer?, java.util.function.IntConsumer, java.util.Spliterator.OfInt)
				Implements java.util.Spliterator.OfInt

				Friend Sub New(ByVal supplier As java.util.function.Supplier(Of java.util.Spliterator.OfInt))
					MyBase.New(supplier)
				End Sub
			End Class

			Friend NotInheritable Class OfLong
				Inherits OfPrimitive(Of Long?, java.util.function.LongConsumer, java.util.Spliterator.OfLong)
				Implements java.util.Spliterator.OfLong

				Friend Sub New(ByVal supplier As java.util.function.Supplier(Of java.util.Spliterator.OfLong))
					MyBase.New(supplier)
				End Sub
			End Class

			Friend NotInheritable Class OfDouble
				Inherits OfPrimitive(Of Double?, java.util.function.DoubleConsumer, java.util.Spliterator.OfDouble)
				Implements java.util.Spliterator.OfDouble

				Friend Sub New(ByVal supplier As java.util.function.Supplier(Of java.util.Spliterator.OfDouble))
					MyBase.New(supplier)
				End Sub
			End Class
		End Class

		''' <summary>
		''' A slice Spliterator from a source Spliterator that reports
		''' {@code SUBSIZED}.
		''' 
		''' </summary>
		Friend MustInherit Class SliceSpliterator(Of T, T_SPLITR As java.util.Spliterator(Of T))
			' The start index of the slice
			Friend ReadOnly sliceOrigin As Long
			' One past the last index of the slice
			Friend ReadOnly sliceFence As Long

			' The spliterator to slice
			Friend s As T_SPLITR
			' current (absolute) index, modified on advance/split
			Friend index As Long
			' one past last (absolute) index or sliceFence, which ever is smaller
			Friend fence As Long

			Friend Sub New(ByVal s As T_SPLITR, ByVal sliceOrigin As Long, ByVal sliceFence As Long, ByVal origin As Long, ByVal fence As Long)
				Debug.Assert(s.hasCharacteristics(java.util.Spliterator.SUBSIZED))
				Me.s = s
				Me.sliceOrigin = sliceOrigin
				Me.sliceFence = sliceFence
				Me.index = origin
				Me.fence = fence
			End Sub

			Protected Friend MustOverride Function makeSpliterator(ByVal s As T_SPLITR, ByVal sliceOrigin As Long, ByVal sliceFence As Long, ByVal origin As Long, ByVal fence As Long) As T_SPLITR

			Public Overridable Function trySplit() As T_SPLITR
				If sliceOrigin >= fence Then Return Nothing

				If index >= fence Then Return Nothing

				' Keep splitting until the left and right splits intersect with the slice
				' thereby ensuring the size estimate decreases.
				' This also avoids creating empty spliterators which can result in
				' existing and additionally created F/J tasks that perform
				' redundant work on no elements.
				Do
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					Dim leftSplit As T_SPLITR = CType(s.trySplit(), T_SPLITR)
					If leftSplit Is Nothing Then Return Nothing

					Dim leftSplitFenceUnbounded As Long = index + leftSplit.estimateSize()
					Dim leftSplitFence As Long = System.Math.Min(leftSplitFenceUnbounded, sliceFence)
					If sliceOrigin >= leftSplitFence Then
						' The left split does not intersect with, and is to the left of, the slice
						' The right split does intersect
						' Discard the left split and split further with the right split
						index = leftSplitFence
					ElseIf leftSplitFence >= sliceFence Then
						' The right split does not intersect with, and is to the right of, the slice
						' The left split does intersect
						' Discard the right split and split further with the left split
						s = leftSplit
						fence = leftSplitFence
					ElseIf index >= sliceOrigin AndAlso leftSplitFenceUnbounded <= sliceFence Then
						' The left split is contained within the slice, return the underlying left split
						' Right split is contained within or intersects with the slice
						index = leftSplitFence
						Return leftSplit
					Else
						' The left split intersects with the slice
						' Right split is contained within or intersects with the slice
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						Return makeSpliterator(leftSplit, sliceOrigin, sliceFence, index, index = leftSplitFence)
					End If
				Loop
			End Function

			Public Overridable Function estimateSize() As Long
				Return If(sliceOrigin < fence, fence - System.Math.Max(sliceOrigin, index), 0)
			End Function

			Public Overridable Function characteristics() As Integer
				Return s.characteristics()
			End Function

			Friend NotInheritable Class OfRef(Of T)
				Inherits SliceSpliterator(Of T, java.util.Spliterator(Of T))
				Implements java.util.Spliterator(Of T)

				Friend Sub New(ByVal s As java.util.Spliterator(Of T), ByVal sliceOrigin As Long, ByVal sliceFence As Long)
					Me.New(s, sliceOrigin, sliceFence, 0, System.Math.Min(s.estimateSize(), sliceFence))
				End Sub

				Private Sub New(ByVal s As java.util.Spliterator(Of T), ByVal sliceOrigin As Long, ByVal sliceFence As Long, ByVal origin As Long, ByVal fence As Long)
					MyBase.New(s, sliceOrigin, sliceFence, origin, fence)
				End Sub

				Protected Friend Overrides Function makeSpliterator(ByVal s As java.util.Spliterator(Of T), ByVal sliceOrigin As Long, ByVal sliceFence As Long, ByVal origin As Long, ByVal fence As Long) As java.util.Spliterator(Of T)
					Return New OfRef(Of )(s, sliceOrigin, sliceFence, origin, fence)
				End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
				Public Overrides Function tryAdvance(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) As Boolean
					java.util.Objects.requireNonNull(action)

					If sliceOrigin >= fence Then Return False

					Do While sliceOrigin > index
						s.tryAdvance(e -> {})
						index += 1
					Loop

					If index >= fence Then Return False

					index += 1
					Return s.tryAdvance(action)
				End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
				Public Overrides Sub forEachRemaining(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
					java.util.Objects.requireNonNull(action)

					If sliceOrigin >= fence Then Return

					If index >= fence Then Return

					If index >= sliceOrigin AndAlso (index + s.estimateSize()) <= sliceFence Then
						' The spliterator is contained within the slice
						s.forEachRemaining(action)
						index = fence
					Else
						' The spliterator intersects with the slice
						Do While sliceOrigin > index
							s.tryAdvance(e -> {})
							index += 1
						Loop
						' Traverse elements up to the fence
						Do While index < fence
							s.tryAdvance(action)
							index += 1
						Loop
					End If
				End Sub
			End Class

			Friend MustInherit Class OfPrimitive(Of T, T_SPLITR As java.util.Spliterator.OfPrimitive(Of T, T_CONS, T_SPLITR), T_CONS)
				Inherits SliceSpliterator(Of T, T_SPLITR)
				Implements java.util.Spliterator.OfPrimitive(Of T, T_CONS, T_SPLITR)

				Friend Sub New(ByVal s As T_SPLITR, ByVal sliceOrigin As Long, ByVal sliceFence As Long)
					Me.New(s, sliceOrigin, sliceFence, 0, System.Math.Min(s.estimateSize(), sliceFence))
				End Sub

				Private Sub New(ByVal s As T_SPLITR, ByVal sliceOrigin As Long, ByVal sliceFence As Long, ByVal origin As Long, ByVal fence As Long)
					MyBase.New(s, sliceOrigin, sliceFence, origin, fence)
				End Sub

				Public Overrides Function tryAdvance(ByVal action As T_CONS) As Boolean
					java.util.Objects.requireNonNull(action)

					If outerInstance.sliceOrigin >= outerInstance.fence Then Return False

					Do While outerInstance.sliceOrigin > outerInstance.index
						outerInstance.s.tryAdvance(emptyConsumer())
						outerInstance.index += 1
					Loop

					If outerInstance.index >= outerInstance.fence Then Return False

					outerInstance.index += 1
					Return outerInstance.s.tryAdvance(action)
				End Function

				Public Overrides Sub forEachRemaining(ByVal action As T_CONS)
					java.util.Objects.requireNonNull(action)

					If outerInstance.sliceOrigin >= outerInstance.fence Then Return

					If outerInstance.index >= outerInstance.fence Then Return

					If outerInstance.index >= outerInstance.sliceOrigin AndAlso (outerInstance.index + outerInstance.s.estimateSize()) <= outerInstance.sliceFence Then
						' The spliterator is contained within the slice
						outerInstance.s.forEachRemaining(action)
						outerInstance.index = outerInstance.fence
					Else
						' The spliterator intersects with the slice
						Do While outerInstance.sliceOrigin > outerInstance.index
							outerInstance.s.tryAdvance(emptyConsumer())
							outerInstance.index += 1
						Loop
						' Traverse elements up to the fence
						Do While outerInstance.index < outerInstance.fence
							outerInstance.s.tryAdvance(action)
							outerInstance.index += 1
						Loop
					End If
				End Sub

				Protected Friend MustOverride Function emptyConsumer() As T_CONS
			End Class

			Friend NotInheritable Class OfInt
				Inherits OfPrimitive(Of Integer?, java.util.Spliterator.OfInt, java.util.function.IntConsumer)
				Implements java.util.Spliterator.OfInt

				Friend Sub New(ByVal s As java.util.Spliterator.OfInt, ByVal sliceOrigin As Long, ByVal sliceFence As Long)
					MyBase.New(s, sliceOrigin, sliceFence)
				End Sub

				Friend Sub New(ByVal s As java.util.Spliterator.OfInt, ByVal sliceOrigin As Long, ByVal sliceFence As Long, ByVal origin As Long, ByVal fence As Long)
					MyBase.New(s, sliceOrigin, sliceFence, origin, fence)
				End Sub

				Protected Friend Overrides Function makeSpliterator(ByVal s As java.util.Spliterator.OfInt, ByVal sliceOrigin As Long, ByVal sliceFence As Long, ByVal origin As Long, ByVal fence As Long) As java.util.Spliterator.OfInt
					Return New SliceSpliterator.OfInt(s, sliceOrigin, sliceFence, origin, fence)
				End Function

				Protected Friend Overrides Function emptyConsumer() As java.util.function.IntConsumer
					Return e ->
				End Function
			End Class

			Friend NotInheritable Class OfLong
				Inherits OfPrimitive(Of Long?, java.util.Spliterator.OfLong, java.util.function.LongConsumer)
				Implements java.util.Spliterator.OfLong

				Friend Sub New(ByVal s As java.util.Spliterator.OfLong, ByVal sliceOrigin As Long, ByVal sliceFence As Long)
					MyBase.New(s, sliceOrigin, sliceFence)
				End Sub

				Friend Sub New(ByVal s As java.util.Spliterator.OfLong, ByVal sliceOrigin As Long, ByVal sliceFence As Long, ByVal origin As Long, ByVal fence As Long)
					MyBase.New(s, sliceOrigin, sliceFence, origin, fence)
				End Sub

				Protected Friend Overrides Function makeSpliterator(ByVal s As java.util.Spliterator.OfLong, ByVal sliceOrigin As Long, ByVal sliceFence As Long, ByVal origin As Long, ByVal fence As Long) As java.util.Spliterator.OfLong
					Return New SliceSpliterator.OfLong(s, sliceOrigin, sliceFence, origin, fence)
				End Function

				Protected Friend Overrides Function emptyConsumer() As java.util.function.LongConsumer
					Return e ->
				End Function
			End Class

			Friend NotInheritable Class OfDouble
				Inherits OfPrimitive(Of Double?, java.util.Spliterator.OfDouble, java.util.function.DoubleConsumer)
				Implements java.util.Spliterator.OfDouble

				Friend Sub New(ByVal s As java.util.Spliterator.OfDouble, ByVal sliceOrigin As Long, ByVal sliceFence As Long)
					MyBase.New(s, sliceOrigin, sliceFence)
				End Sub

				Friend Sub New(ByVal s As java.util.Spliterator.OfDouble, ByVal sliceOrigin As Long, ByVal sliceFence As Long, ByVal origin As Long, ByVal fence As Long)
					MyBase.New(s, sliceOrigin, sliceFence, origin, fence)
				End Sub

				Protected Friend Overrides Function makeSpliterator(ByVal s As java.util.Spliterator.OfDouble, ByVal sliceOrigin As Long, ByVal sliceFence As Long, ByVal origin As Long, ByVal fence As Long) As java.util.Spliterator.OfDouble
					Return New SliceSpliterator.OfDouble(s, sliceOrigin, sliceFence, origin, fence)
				End Function

				Protected Friend Overrides Function emptyConsumer() As java.util.function.DoubleConsumer
					Return e ->
				End Function
			End Class
		End Class

		''' <summary>
		''' A slice Spliterator that does not preserve order, if any, of a source
		''' Spliterator.
		''' 
		''' Note: The source spliterator may report {@code ORDERED} since that
		''' spliterator be the result of a previous pipeline stage that was
		''' collected to a {@code Node}. It is the order of the pipeline stage
		''' that governs whether the this slice spliterator is to be used or not.
		''' </summary>
		Friend MustInherit Class UnorderedSliceSpliterator(Of T, T_SPLITR As java.util.Spliterator(Of T))
			Friend Shared ReadOnly CHUNK_SIZE As Integer = 1 << 7

			' The spliterator to slice
			Protected Friend ReadOnly s As T_SPLITR
			Protected Friend ReadOnly unlimited As Boolean
			Private ReadOnly skipThreshold As Long
			Private ReadOnly permits As java.util.concurrent.atomic.AtomicLong

			Friend Sub New(ByVal s As T_SPLITR, ByVal skip As Long, ByVal limit As Long)
				Me.s = s
				Me.unlimited = limit < 0
				Me.skipThreshold = If(limit >= 0, limit, 0)
				Me.permits = New java.util.concurrent.atomic.AtomicLong(If(limit >= 0, skip + limit, skip))
			End Sub

			Friend Sub New(ByVal s As T_SPLITR, ByVal parent As UnorderedSliceSpliterator(Of T, T_SPLITR))
				Me.s = s
				Me.unlimited = parent.unlimited
				Me.permits = parent.permits
				Me.skipThreshold = parent.skipThreshold
			End Sub

			''' <summary>
			''' Acquire permission to skip or process elements.  The caller must
			''' first acquire the elements, then consult this method for guidance
			''' as to what to do with the data.
			''' 
			''' <p>We use an {@code AtomicLong} to atomically maintain a counter,
			''' which is initialized as skip+limit if we are limiting, or skip only
			''' if we are not limiting.  The user should consult the method
			''' {@code checkPermits()} before acquiring data elements.
			''' </summary>
			''' <param name="numElements"> the number of elements the caller has in hand </param>
			''' <returns> the number of elements that should be processed; any
			''' remaining elements should be discarded. </returns>
			Protected Friend Function acquirePermits(ByVal numElements As Long) As Long
				Dim remainingPermits As Long
				Dim grabbing As Long
				' permits never increase, and don't decrease below zero
				Debug.Assert(numElements > 0)
				Do
					remainingPermits = permits.get()
					If remainingPermits = 0 Then Return If(unlimited, numElements, 0)
					grabbing = System.Math.Min(remainingPermits, numElements)
				Loop While grabbing > 0 AndAlso Not permits.compareAndSet(remainingPermits, remainingPermits - grabbing)

				If unlimited Then
					Return System.Math.Max(numElements - grabbing, 0)
				ElseIf remainingPermits > skipThreshold Then
					Return System.Math.Max(grabbing - (remainingPermits - skipThreshold), 0)
				Else
					Return grabbing
				End If
			End Function

			Friend Enum PermitStatus
				NO_MORE
				MAYBE_MORE
				UNLIMITED
			End Enum

			''' <summary>
			''' Call to check if permits might be available before acquiring data </summary>
			Protected Friend Function permitStatus() As PermitStatus
				If permits.get() > 0 Then
					Return PermitStatus.MAYBE_MORE
				Else
					Return If(unlimited, PermitStatus.UNLIMITED, PermitStatus.NO_MORE)
				End If
			End Function

			Public Function trySplit() As T_SPLITR
				' Stop splitting when there are no more limit permits
				If permits.get() = 0 Then Return Nothing
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim Split As T_SPLITR = CType(s.trySplit(), T_SPLITR)
				Return If(Split Is Nothing, Nothing, makeSpliterator(Split))
			End Function

			Protected Friend MustOverride Function makeSpliterator(ByVal s As T_SPLITR) As T_SPLITR

			Public Function estimateSize() As Long
				Return s.estimateSize()
			End Function

			Public Function characteristics() As Integer
				Return s.characteristics() And Not(java.util.Spliterator.SIZED Or java.util.Spliterator.SUBSIZED Or java.util.Spliterator.ORDERED)
			End Function

			Friend NotInheritable Class OfRef(Of T)
				Inherits UnorderedSliceSpliterator(Of T, java.util.Spliterator(Of T))
				Implements java.util.Spliterator(Of T), java.util.function.Consumer(Of T)

				Friend tmpSlot As T

				Friend Sub New(ByVal s As java.util.Spliterator(Of T), ByVal skip As Long, ByVal limit As Long)
					MyBase.New(s, skip, limit)
				End Sub

				Friend Sub New(ByVal s As java.util.Spliterator(Of T), ByVal parent As OfRef(Of T))
					MyBase.New(s, parent)
				End Sub

				Public Overrides Sub accept(ByVal t As T)
					tmpSlot = t
				End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
				Public Overrides Function tryAdvance(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) As Boolean
					java.util.Objects.requireNonNull(action)

					Do While permitStatus() <> PermitStatus.NO_MORE
						If Not s.tryAdvance(Me) Then
							Return False
						ElseIf acquirePermits(1) = 1 Then
							action.accept(tmpSlot)
							tmpSlot = Nothing
							Return True
						End If
					Loop
					Return False
				End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
				Public Overrides Sub forEachRemaining(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
					java.util.Objects.requireNonNull(action)

					Dim sb As ArrayBuffer.OfRef(Of T) = Nothing
					Dim permitStatus As PermitStatus
					permitStatus = permitStatus()
					Do While permitStatus <> PermitStatus.NO_MORE
						If permitStatus = PermitStatus.MAYBE_MORE Then
							' Optimistically traverse elements up to a threshold of CHUNK_SIZE
							If sb Is Nothing Then
								sb = New ArrayBuffer.OfRef(Of )(CHUNK_SIZE)
							Else
								sb.reset()
							End If
							Dim permitsRequested As Long = 0
							Do
								permitsRequested += 1
							Loop While s.tryAdvance(sb) AndAlso permitsRequested < CHUNK_SIZE
							If permitsRequested = 0 Then Return
							sb.forEach(action, acquirePermits(permitsRequested))
						Else
							' Must be UNLIMITED; let 'er rip
							s.forEachRemaining(action)
							Return
						End If
						permitStatus = permitStatus()
					Loop
				End Sub

				Protected Friend Overrides Function makeSpliterator(ByVal s As java.util.Spliterator(Of T)) As java.util.Spliterator(Of T)
					Return New UnorderedSliceSpliterator.OfRef(Of )(s, Me)
				End Function
			End Class

			''' <summary>
			''' Concrete sub-types must also be an instance of type {@code T_CONS}.
			''' </summary>
			''' @param <T_BUFF> the type of the spined buffer. Must also be a type of
			'''        {@code T_CONS}. </param>
			Friend MustInherit Class OfPrimitive(Of T, T_CONS, T_BUFF As ArrayBuffer.OfPrimitive(Of T_CONS), T_SPLITR As java.util.Spliterator.OfPrimitive(Of T, T_CONS, T_SPLITR))
				Inherits UnorderedSliceSpliterator(Of T, T_SPLITR)
				Implements java.util.Spliterator.OfPrimitive(Of T, T_CONS, T_SPLITR)

				Friend Sub New(ByVal s As T_SPLITR, ByVal skip As Long, ByVal limit As Long)
					MyBase.New(s, skip, limit)
				End Sub

				Friend Sub New(ByVal s As T_SPLITR, ByVal parent As UnorderedSliceSpliterator.OfPrimitive(Of T, T_CONS, T_BUFF, T_SPLITR))
					MyBase.New(s, parent)
				End Sub

				Public Overrides Function tryAdvance(ByVal action As T_CONS) As Boolean
					java.util.Objects.requireNonNull(action)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					Dim consumer As T_CONS = CType(Me, T_CONS)

					Do While outerInstance.permitStatus() <> PermitStatus.NO_MORE
						If Not outerInstance.s.tryAdvance(consumer) Then
							Return False
						ElseIf outerInstance.acquirePermits(1) = 1 Then
							acceptConsumed(action)
							Return True
						End If
					Loop
					Return False
				End Function

				Protected Friend MustOverride Sub acceptConsumed(ByVal action As T_CONS)

				Public Overrides Sub forEachRemaining(ByVal action As T_CONS)
					java.util.Objects.requireNonNull(action)

					Dim sb As T_BUFF = Nothing
					Dim permitStatus As PermitStatus
					permitStatus = outerInstance.permitStatus()
					Do While permitStatus <> PermitStatus.NO_MORE
						If permitStatus = PermitStatus.MAYBE_MORE Then
							' Optimistically traverse elements up to a threshold of CHUNK_SIZE
							If sb Is Nothing Then
								sb = bufferCreate(CHUNK_SIZE)
							Else
								sb.reset()
							End If
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
							Dim sbc As T_CONS = CType(sb, T_CONS)
							Dim permitsRequested As Long = 0
							Do
								permitsRequested += 1
							Loop While outerInstance.s.tryAdvance(sbc) AndAlso permitsRequested < CHUNK_SIZE
							If permitsRequested = 0 Then Return
							sb.forEach(action, outerInstance.acquirePermits(permitsRequested))
						Else
							' Must be UNLIMITED; let 'er rip
							outerInstance.s.forEachRemaining(action)
							Return
						End If
						permitStatus = outerInstance.permitStatus()
					Loop
				End Sub

				Protected Friend MustOverride Function bufferCreate(ByVal initialCapacity As Integer) As T_BUFF
			End Class

			Friend NotInheritable Class OfInt
				Inherits OfPrimitive(Of Integer?, java.util.function.IntConsumer, ArrayBuffer.OfInt, java.util.Spliterator.OfInt)
				Implements java.util.Spliterator.OfInt, java.util.function.IntConsumer

				Friend tmpValue As Integer

				Friend Sub New(ByVal s As java.util.Spliterator.OfInt, ByVal skip As Long, ByVal limit As Long)
					MyBase.New(s, skip, limit)
				End Sub

				Friend Sub New(ByVal s As java.util.Spliterator.OfInt, ByVal parent As UnorderedSliceSpliterator.OfInt)
					MyBase.New(s, parent)
				End Sub

				Public Overrides Sub accept(ByVal value As Integer)
					tmpValue = value
				End Sub

				Protected Friend Overrides Sub acceptConsumed(ByVal action As java.util.function.IntConsumer)
					action.accept(tmpValue)
				End Sub

				Protected Friend Overrides Function bufferCreate(ByVal initialCapacity As Integer) As ArrayBuffer.OfInt
					Return New ArrayBuffer.OfInt(initialCapacity)
				End Function

				Protected Friend Overrides Function makeSpliterator(ByVal s As java.util.Spliterator.OfInt) As java.util.Spliterator.OfInt
					Return New UnorderedSliceSpliterator.OfInt(s, Me)
				End Function
			End Class

			Friend NotInheritable Class OfLong
				Inherits OfPrimitive(Of Long?, java.util.function.LongConsumer, ArrayBuffer.OfLong, java.util.Spliterator.OfLong)
				Implements java.util.Spliterator.OfLong, java.util.function.LongConsumer

				Friend tmpValue As Long

				Friend Sub New(ByVal s As java.util.Spliterator.OfLong, ByVal skip As Long, ByVal limit As Long)
					MyBase.New(s, skip, limit)
				End Sub

				Friend Sub New(ByVal s As java.util.Spliterator.OfLong, ByVal parent As UnorderedSliceSpliterator.OfLong)
					MyBase.New(s, parent)
				End Sub

				Public Overrides Sub accept(ByVal value As Long)
					tmpValue = value
				End Sub

				Protected Friend Overrides Sub acceptConsumed(ByVal action As java.util.function.LongConsumer)
					action.accept(tmpValue)
				End Sub

				Protected Friend Overrides Function bufferCreate(ByVal initialCapacity As Integer) As ArrayBuffer.OfLong
					Return New ArrayBuffer.OfLong(initialCapacity)
				End Function

				Protected Friend Overrides Function makeSpliterator(ByVal s As java.util.Spliterator.OfLong) As java.util.Spliterator.OfLong
					Return New UnorderedSliceSpliterator.OfLong(s, Me)
				End Function
			End Class

			Friend NotInheritable Class OfDouble
				Inherits OfPrimitive(Of Double?, java.util.function.DoubleConsumer, ArrayBuffer.OfDouble, java.util.Spliterator.OfDouble)
				Implements java.util.Spliterator.OfDouble, java.util.function.DoubleConsumer

				Friend tmpValue As Double

				Friend Sub New(ByVal s As java.util.Spliterator.OfDouble, ByVal skip As Long, ByVal limit As Long)
					MyBase.New(s, skip, limit)
				End Sub

				Friend Sub New(ByVal s As java.util.Spliterator.OfDouble, ByVal parent As UnorderedSliceSpliterator.OfDouble)
					MyBase.New(s, parent)
				End Sub

				Public Overrides Sub accept(ByVal value As Double)
					tmpValue = value
				End Sub

				Protected Friend Overrides Sub acceptConsumed(ByVal action As java.util.function.DoubleConsumer)
					action.accept(tmpValue)
				End Sub

				Protected Friend Overrides Function bufferCreate(ByVal initialCapacity As Integer) As ArrayBuffer.OfDouble
					Return New ArrayBuffer.OfDouble(initialCapacity)
				End Function

				Protected Friend Overrides Function makeSpliterator(ByVal s As java.util.Spliterator.OfDouble) As java.util.Spliterator.OfDouble
					Return New UnorderedSliceSpliterator.OfDouble(s, Me)
				End Function
			End Class
		End Class

		''' <summary>
		''' A wrapping spliterator that only reports distinct elements of the
		''' underlying spliterator. Does not preserve size and encounter order.
		''' </summary>
		Friend NotInheritable Class DistinctSpliterator(Of T)
			Implements java.util.Spliterator(Of T), java.util.function.Consumer(Of T)

			' The value to represent null in the ConcurrentHashMap
			Private Shared ReadOnly NULL_VALUE As New Object

			' The underlying spliterator
			Private ReadOnly s As java.util.Spliterator(Of T)

			' ConcurrentHashMap holding distinct elements as keys
			Private ReadOnly seen As ConcurrentDictionary(Of T, Boolean?)

			' Temporary element, only used with tryAdvance
			Private tmpSlot As T

			Friend Sub New(ByVal s As java.util.Spliterator(Of T))
				Me.New(s, New ConcurrentDictionary(Of ))
			End Sub

			Private Sub New(ByVal s As java.util.Spliterator(Of T), ByVal seen As ConcurrentDictionary(Of T, Boolean?))
				Me.s = s
				Me.seen = seen
			End Sub

			Public Overrides Sub accept(ByVal t As T)
				Me.tmpSlot = t
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Private Function mapNull(ByVal t As T) As T
				Return If(t IsNot Nothing, t, CType(NULL_VALUE, T))
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Function tryAdvance(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) As Boolean
				Do While s.tryAdvance(Me)
					If seen.GetOrAdd(mapNull(tmpSlot),  java.lang.[Boolean].TRUE) Is Nothing Then
						action.accept(tmpSlot)
						tmpSlot = Nothing
						Return True
					End If
				Loop
				Return False
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub forEachRemaining(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
				s.forEachRemaining(t -> { if(seen.GetOrAdd(mapNull(t),  java.lang.[Boolean].TRUE) Is Nothing) { action.accept(t); } })
			End Sub

			Public Overrides Function trySplit() As java.util.Spliterator(Of T)
				Dim Split As java.util.Spliterator(Of T) = s.trySplit()
				Return If(Split IsNot Nothing, New DistinctSpliterator(Of )(Split, seen), Nothing)
			End Function

			Public Overrides Function estimateSize() As Long
				Return s.estimateSize()
			End Function

			Public Overrides Function characteristics() As Integer
				Return (s.characteristics() And Not(java.util.Spliterator.SIZED Or java.util.Spliterator.SUBSIZED Or java.util.Spliterator.SORTED Or java.util.Spliterator.ORDERED)) Or java.util.Spliterator.DISTINCT
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public  Overrides ReadOnly Property  comparator As IComparer(Of ?)
				Get
					Return s.comparator
				End Get
			End Property
		End Class

		''' <summary>
		''' A Spliterator that infinitely supplies elements in no particular order.
		''' 
		''' <p>Splitting divides the estimated size in two and stops when the
		''' estimate size is 0.
		''' 
		''' <p>The {@code forEachRemaining} method if invoked will never terminate.
		''' The {@code tryAdvance} method always returns true.
		''' 
		''' </summary>
		Friend MustInherit Class InfiniteSupplyingSpliterator(Of T)
			Implements java.util.Spliterator(Of T)

			Friend estimate As Long

			Protected Friend Sub New(ByVal estimate As Long)
				Me.estimate = estimate
			End Sub

			Public Overrides Function estimateSize() As Long
				Return estimate
			End Function

			Public Overrides Function characteristics() As Integer
				Return IMMUTABLE
			End Function

			Friend NotInheritable Class OfRef(Of T)
				Inherits InfiniteSupplyingSpliterator(Of T)

				Friend ReadOnly s As java.util.function.Supplier(Of T)

				Friend Sub New(ByVal size As Long, ByVal s As java.util.function.Supplier(Of T))
					MyBase.New(size)
					Me.s = s
				End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
				Public Overrides Function tryAdvance(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) As Boolean
					java.util.Objects.requireNonNull(action)

					action.accept(s.get())
					Return True
				End Function

				Public Overrides Function trySplit() As java.util.Spliterator(Of T)
					If estimate = 0 Then Return Nothing
					Return New InfiniteSupplyingSpliterator.OfRef(Of )(estimate >>>= 1, s)
				End Function
			End Class

			Friend NotInheritable Class OfInt
				Inherits InfiniteSupplyingSpliterator(Of Integer?)
				Implements java.util.Spliterator.OfInt

				Friend ReadOnly s As java.util.function.IntSupplier

				Friend Sub New(ByVal size As Long, ByVal s As java.util.function.IntSupplier)
					MyBase.New(size)
					Me.s = s
				End Sub

				Public Overrides Function tryAdvance(ByVal action As java.util.function.IntConsumer) As Boolean
					java.util.Objects.requireNonNull(action)

					action.accept(s.asInt)
					Return True
				End Function

				Public Overrides Function trySplit() As java.util.Spliterator.OfInt
					If estimate = 0 Then Return Nothing
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Return New InfiniteSupplyingSpliterator.OfInt(estimate = CLng(CULng(estimate) >> 1), s)
				End Function
			End Class

			Friend NotInheritable Class OfLong
				Inherits InfiniteSupplyingSpliterator(Of Long?)
				Implements java.util.Spliterator.OfLong

				Friend ReadOnly s As java.util.function.LongSupplier

				Friend Sub New(ByVal size As Long, ByVal s As java.util.function.LongSupplier)
					MyBase.New(size)
					Me.s = s
				End Sub

				Public Overrides Function tryAdvance(ByVal action As java.util.function.LongConsumer) As Boolean
					java.util.Objects.requireNonNull(action)

					action.accept(s.asLong)
					Return True
				End Function

				Public Overrides Function trySplit() As java.util.Spliterator.OfLong
					If estimate = 0 Then Return Nothing
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Return New InfiniteSupplyingSpliterator.OfLong(estimate = CLng(CULng(estimate) >> 1), s)
				End Function
			End Class

			Friend NotInheritable Class OfDouble
				Inherits InfiniteSupplyingSpliterator(Of Double?)
				Implements java.util.Spliterator.OfDouble

				Friend ReadOnly s As java.util.function.DoubleSupplier

				Friend Sub New(ByVal size As Long, ByVal s As java.util.function.DoubleSupplier)
					MyBase.New(size)
					Me.s = s
				End Sub

				Public Overrides Function tryAdvance(ByVal action As java.util.function.DoubleConsumer) As Boolean
					java.util.Objects.requireNonNull(action)

					action.accept(s.asDouble)
					Return True
				End Function

				Public Overrides Function trySplit() As java.util.Spliterator.OfDouble
					If estimate = 0 Then Return Nothing
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Return New InfiniteSupplyingSpliterator.OfDouble(estimate = CLng(CULng(estimate) >> 1), s)
				End Function
			End Class
		End Class

		' @@@ Consolidate with Node.Builder
		Friend MustInherit Class ArrayBuffer
			Friend index As Integer

			Friend Overridable Sub reset()
				index = 0
			End Sub

			Friend NotInheritable Class OfRef(Of T)
				Inherits ArrayBuffer
				Implements java.util.function.Consumer(Of T)

				Friend ReadOnly array As Object()

				Friend Sub New(ByVal size As Integer)
					Me.array = New Object(size - 1){}
				End Sub

				Public Overrides Sub accept(ByVal t As T)
					array(index) = t
					index += 1
				End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
				Public Sub forEach(Of T1)(ByVal action As java.util.function.Consumer(Of T1), ByVal fence As Long)
					For i As Integer = 0 To fence - 1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
						Dim t As T = CType(array(i), T)
						action.accept(t)
					Next i
				End Sub
			End Class

			Friend MustInherit Class OfPrimitive(Of T_CONS)
				Inherits ArrayBuffer

				Friend Shadows index As Integer

				Friend Overrides Sub reset()
					index = 0
				End Sub

				Friend MustOverride Sub forEach(ByVal action As T_CONS, ByVal fence As Long)
			End Class

			Friend NotInheritable Class OfInt
				Inherits OfPrimitive(Of java.util.function.IntConsumer)
				Implements java.util.function.IntConsumer

				Friend ReadOnly array As Integer()

				Friend Sub New(ByVal size As Integer)
					Me.array = New Integer(size - 1){}
				End Sub

				Public Overrides Sub accept(ByVal t As Integer)
					array(index) = t
					index += 1
				End Sub

				Public Overrides Sub forEach(ByVal action As java.util.function.IntConsumer, ByVal fence As Long)
					For i As Integer = 0 To fence - 1
						action.accept(array(i))
					Next i
				End Sub
			End Class

			Friend NotInheritable Class OfLong
				Inherits OfPrimitive(Of java.util.function.LongConsumer)
				Implements java.util.function.LongConsumer

				Friend ReadOnly array As Long()

				Friend Sub New(ByVal size As Integer)
					Me.array = New Long(size - 1){}
				End Sub

				Public Overrides Sub accept(ByVal t As Long)
					array(index) = t
					index += 1
				End Sub

				Public Overrides Sub forEach(ByVal action As java.util.function.LongConsumer, ByVal fence As Long)
					For i As Integer = 0 To fence - 1
						action.accept(array(i))
					Next i
				End Sub
			End Class

			Friend NotInheritable Class OfDouble
				Inherits OfPrimitive(Of java.util.function.DoubleConsumer)
				Implements java.util.function.DoubleConsumer

				Friend ReadOnly array As Double()

				Friend Sub New(ByVal size As Integer)
					Me.array = New Double(size - 1){}
				End Sub

				Public Overrides Sub accept(ByVal t As Double)
					array(index) = t
					index += 1
				End Sub

				Friend Overrides Sub forEach(ByVal action As java.util.function.DoubleConsumer, ByVal fence As Long)
					For i As Integer = 0 To fence - 1
						action.accept(array(i))
					Next i
				End Sub
			End Class
		End Class
	End Class


End Namespace