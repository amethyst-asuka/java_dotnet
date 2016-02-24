Imports System
Imports System.Diagnostics

'
' * Copyright (c) 2012, 2014, Oracle and/or its affiliates. All rights reserved.
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
	''' Abstract base class for an intermediate pipeline stage or pipeline source
	''' stage implementing whose elements are of type {@code int}.
	''' </summary>
	''' @param <E_IN> type of elements in the upstream source
	''' @since 1.8 </param>
	Friend MustInherit Class IntPipeline(Of E_IN)
		Inherits AbstractPipeline(Of E_IN, Integer?, IntStream)
		Implements IntStream

		''' <summary>
		''' Constructor for the head of a stream pipeline.
		''' </summary>
		''' <param name="source"> {@code Supplier<Spliterator>} describing the stream source </param>
		''' <param name="sourceFlags"> The source flags for the stream source, described in
		'''        <seealso cref="StreamOpFlag"/> </param>
		''' <param name="parallel"> {@code true} if the pipeline is parallel </param>
		Friend Sub New(Of T1 As java.util.Spliterator(Of Integer?)(ByVal source As java.util.function.Supplier(Of T1), ByVal sourceFlags As Integer, ByVal parallel As Boolean)
			MyBase.New(source, sourceFlags, parallel)
		End Sub

		''' <summary>
		''' Constructor for the head of a stream pipeline.
		''' </summary>
		''' <param name="source"> {@code Spliterator} describing the stream source </param>
		''' <param name="sourceFlags"> The source flags for the stream source, described in
		'''        <seealso cref="StreamOpFlag"/> </param>
		''' <param name="parallel"> {@code true} if the pipeline is parallel </param>
		Friend Sub New(ByVal source As java.util.Spliterator(Of Integer?), ByVal sourceFlags As Integer, ByVal parallel As Boolean)
			MyBase.New(source, sourceFlags, parallel)
		End Sub

		''' <summary>
		''' Constructor for appending an intermediate operation onto an existing
		''' pipeline.
		''' </summary>
		''' <param name="upstream"> the upstream element source </param>
		''' <param name="opFlags"> the operation flags for the new operation </param>
		Friend Sub New(Of T1)(ByVal upstream As AbstractPipeline(Of T1), ByVal opFlags As Integer)
			MyBase.New(upstream, opFlags)
		End Sub

		''' <summary>
		''' Adapt a {@code Sink<Integer> to an {@code IntConsumer}, ideally simply
		''' by casting.
		''' </summary>
		Private Shared Function adapt(ByVal sink As Sink(Of Integer?)) As java.util.function.IntConsumer
			If TypeOf sink Is java.util.function.IntConsumer Then
				Return CType(sink, java.util.function.IntConsumer)
			Else
				If Tripwire.ENABLED Then Tripwire.trip(GetType(AbstractPipeline), "using IntStream.adapt(Sink<Integer> s)")
				Return sink::accept
			End If
		End Function

		''' <summary>
		''' Adapt a {@code Spliterator<Integer>} to a {@code Spliterator.OfInt}.
		''' 
		''' @implNote
		''' The implementation attempts to cast to a Spliterator.OfInt, and throws an
		''' exception if this cast is not possible.
		''' </summary>
		Private Shared Function adapt(ByVal s As java.util.Spliterator(Of Integer?)) As java.util.Spliterator.OfInt
			If TypeOf s Is java.util.Spliterator.OfInt Then
				Return CType(s, java.util.Spliterator.OfInt)
			Else
				If Tripwire.ENABLED Then Tripwire.trip(GetType(AbstractPipeline), "using IntStream.adapt(Spliterator<Integer> s)")
				Throw New UnsupportedOperationException("IntStream.adapt(Spliterator<Integer> s)")
			End If
		End Function


		' Shape-specific methods

		Friend Property Overrides outputShape As StreamShape
			Get
				Return StreamShape.INT_VALUE
			End Get
		End Property

		Friend Overrides Function evaluateToNode(Of P_IN)(ByVal helper As PipelineHelper(Of Integer?), ByVal spliterator As java.util.Spliterator(Of P_IN), ByVal flattenTree As Boolean, ByVal generator As java.util.function.IntFunction(Of Integer?())) As Node(Of Integer?)
			Return Nodes.collectInt(helper, spliterator, flattenTree)
		End Function

		Friend Overrides Function wrap(Of P_IN)(ByVal ph As PipelineHelper(Of Integer?), ByVal supplier As java.util.function.Supplier(Of java.util.Spliterator(Of P_IN)), ByVal isParallel As Boolean) As java.util.Spliterator(Of Integer?)
			Return New StreamSpliterators.IntWrappingSpliterator(Of )(ph, supplier, isParallel)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Overrides Function lazySpliterator(Of T1 As java.util.Spliterator(Of Integer?)(ByVal supplier As java.util.function.Supplier(Of T1)) As java.util.Spliterator.OfInt
			Return New StreamSpliterators.DelegatingSpliterator.OfInt(CType(supplier, java.util.function.Supplier(Of java.util.Spliterator.OfInt)))
		End Function

		Friend Overrides Sub forEachWithCancel(ByVal spliterator As java.util.Spliterator(Of Integer?), ByVal sink As Sink(Of Integer?))
			Dim spl As java.util.Spliterator.OfInt = adapt(spliterator)
			Dim adaptedSink As java.util.function.IntConsumer = adapt(sink)
			Do
			Loop While (Not sink.cancellationRequested()) AndAlso spl.tryAdvance(adaptedSink)
		End Sub

		Friend Overrides Function makeNodeBuilder(ByVal exactSizeIfKnown As Long, ByVal generator As java.util.function.IntFunction(Of Integer?())) As Node.Builder(Of Integer?)
			Return Nodes.intBuilder(exactSizeIfKnown)
		End Function


		' IntStream

		Public Overrides Function [iterator]() As java.util.PrimitiveIterator.OfInt Implements IntStream.iterator
			Return java.util.Spliterators.iterator(spliterator())
		End Function

		Public Overrides Function spliterator() As java.util.Spliterator.OfInt Implements IntStream.spliterator
			Return adapt(MyBase.spliterator())
		End Function

		' Stateless intermediate ops from IntStream

		Public Overrides Function asLongStream() As LongStream Implements IntStream.asLongStream
			Return New StatelessOpAnonymousInnerClassHelper(Of E_IN)
		End Function

		Private Class StatelessOpAnonymousInnerClassHelper(Of E_IN)
			Inherits StatelessOp(Of E_IN)

			Friend Overrides Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of Long?)) As Sink(Of Integer?)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return New Sink.ChainedInt<java.lang.Long>(sink)
	'			{
	'				@Override public void accept(int t)
	'				{
	'					downstream.accept((long) t);
	'				}
	'			};
			End Function
		End Class

		Public Overrides Function asDoubleStream() As DoubleStream Implements IntStream.asDoubleStream
			Return New StatelessOpAnonymousInnerClassHelper(Of E_IN)
		End Function

		Private Class StatelessOpAnonymousInnerClassHelper(Of E_IN)
			Inherits StatelessOp(Of E_IN)

			Friend Overrides Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of Double?)) As Sink(Of Integer?)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return New Sink.ChainedInt<java.lang.Double>(sink)
	'			{
	'				@Override public void accept(int t)
	'				{
	'					downstream.accept((double) t);
	'				}
	'			};
			End Function
		End Class

		Public Overrides Function boxed() As Stream(Of Integer?) Implements IntStream.boxed
			Return mapToObj(Integer?::valueOf)
		End Function

		Public Overrides Function map(ByVal mapper As java.util.function.IntUnaryOperator) As IntStream Implements IntStream.map
			java.util.Objects.requireNonNull(mapper)
			Return New StatelessOpAnonymousInnerClassHelper(Of E_IN)
		End Function

		Private Class StatelessOpAnonymousInnerClassHelper(Of E_IN)
			Inherits StatelessOp(Of E_IN)

			Friend Overrides Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of Integer?)) As Sink(Of Integer?)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return New Sink.ChainedInt<java.lang.Integer>(sink)
	'			{
	'				@Override public void accept(int t)
	'				{
	'					downstream.accept(mapper.applyAsInt(t));
	'				}
	'			};
			End Function
		End Class

		Public Overrides Function mapToObj(Of U, T1 As U)(ByVal mapper As java.util.function.IntFunction(Of T1)) As Stream(Of U) Implements IntStream.mapToObj
			java.util.Objects.requireNonNull(mapper)
			Return New StatelessOpAnonymousInnerClassHelper(Of E_IN, E_OUT)
		End Function

		Private Class StatelessOpAnonymousInnerClassHelper(Of E_IN, E_OUT)
			Inherits StatelessOp(Of E_IN, E_OUT)

			Friend Overrides Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of U)) As Sink(Of Integer?)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return New Sink.ChainedInt<U>(sink)
	'			{
	'				@Override public void accept(int t)
	'				{
	'					downstream.accept(mapper.apply(t));
	'				}
	'			};
			End Function
		End Class

		Public Overrides Function mapToLong(ByVal mapper As java.util.function.IntToLongFunction) As LongStream Implements IntStream.mapToLong
			java.util.Objects.requireNonNull(mapper)
			Return New StatelessOpAnonymousInnerClassHelper2(Of E_IN)
		End Function

		Private Class StatelessOpAnonymousInnerClassHelper2(Of E_IN)
			Inherits StatelessOp(Of E_IN)

			Friend Overrides Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of Long?)) As Sink(Of Integer?)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return New Sink.ChainedInt<java.lang.Long>(sink)
	'			{
	'				@Override public void accept(int t)
	'				{
	'					downstream.accept(mapper.applyAsLong(t));
	'				}
	'			};
			End Function
		End Class

		Public Overrides Function mapToDouble(ByVal mapper As java.util.function.IntToDoubleFunction) As DoubleStream Implements IntStream.mapToDouble
			java.util.Objects.requireNonNull(mapper)
			Return New StatelessOpAnonymousInnerClassHelper2(Of E_IN)
		End Function

		Private Class StatelessOpAnonymousInnerClassHelper2(Of E_IN)
			Inherits StatelessOp(Of E_IN)

			Friend Overrides Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of Double?)) As Sink(Of Integer?)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return New Sink.ChainedInt<java.lang.Double>(sink)
	'			{
	'				@Override public void accept(int t)
	'				{
	'					downstream.accept(mapper.applyAsDouble(t));
	'				}
	'			};
			End Function
		End Class

		Public Overrides Function flatMap(Of T1 As IntStream)(ByVal mapper As java.util.function.IntFunction(Of T1)) As IntStream Implements IntStream.flatMap
			Return New StatelessOpAnonymousInnerClassHelper2(Of E_IN)
		End Function

		Private Class StatelessOpAnonymousInnerClassHelper2(Of E_IN)
			Inherits StatelessOp(Of E_IN)

			Friend Overrides Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of Integer?)) As Sink(Of Integer?)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return New Sink.ChainedInt<java.lang.Integer>(sink)
	'			{
	'				@Override public void begin(long size)
	'				{
	'					downstream.begin(-1);
	'				}
	'
	'				@Override public void accept(int t)
	'				{
	'					try (IntStream result = mapper.apply(t))
	'					{
	'						' We can do better that this too; optimize for depth=0 case and just grab spliterator and forEach it
	'						if (result != Nothing)
	'							result.sequential().forEach(i -> downstream.accept(i));
	'					}
	'				}
	'			};
			End Function
		End Class

		Public Overrides Function unordered() As IntStream
			If Not ordered Then Return Me
			Return New StatelessOpAnonymousInnerClassHelper3(Of E_IN)
		End Function

		Private Class StatelessOpAnonymousInnerClassHelper3(Of E_IN)
			Inherits StatelessOp(Of E_IN)

			Friend Overrides Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of Integer?)) As Sink(Of Integer?)
				Return sink
			End Function
		End Class

		Public Overrides Function filter(ByVal predicate As java.util.function.IntPredicate) As IntStream Implements IntStream.filter
			java.util.Objects.requireNonNull(predicate)
			Return New StatelessOpAnonymousInnerClassHelper4(Of E_IN)
		End Function

		Private Class StatelessOpAnonymousInnerClassHelper4(Of E_IN)
			Inherits StatelessOp(Of E_IN)

			Friend Overrides Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of Integer?)) As Sink(Of Integer?)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return New Sink.ChainedInt<java.lang.Integer>(sink)
	'			{
	'				@Override public void begin(long size)
	'				{
	'					downstream.begin(-1);
	'				}
	'
	'				@Override public void accept(int t)
	'				{
	'					if (predicate.test(t))
	'						downstream.accept(t);
	'				}
	'			};
			End Function
		End Class

		Public Overrides Function peek(ByVal action As java.util.function.IntConsumer) As IntStream Implements IntStream.peek
			java.util.Objects.requireNonNull(action)
			Return New StatelessOpAnonymousInnerClassHelper5(Of E_IN)
		End Function

		Private Class StatelessOpAnonymousInnerClassHelper5(Of E_IN)
			Inherits StatelessOp(Of E_IN)

			Friend Overrides Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of Integer?)) As Sink(Of Integer?)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return New Sink.ChainedInt<java.lang.Integer>(sink)
	'			{
	'				@Override public void accept(int t)
	'				{
	'					action.accept(t);
	'					downstream.accept(t);
	'				}
	'			};
			End Function
		End Class

		' Stateful intermediate ops from IntStream

		Public Overrides Function limit(ByVal maxSize As Long) As IntStream Implements IntStream.limit
			If maxSize < 0 Then Throw New IllegalArgumentException(Convert.ToString(maxSize))
			Return SliceOps.makeInt(Me, 0, maxSize)
		End Function

		Public Overrides Function skip(ByVal n As Long) As IntStream Implements IntStream.skip
			If n < 0 Then Throw New IllegalArgumentException(Convert.ToString(n))
			If n = 0 Then
				Return Me
			Else
				Return SliceOps.makeInt(Me, n, -1)
			End If
		End Function

		Public Overrides Function sorted() As IntStream Implements IntStream.sorted
			Return SortedOps.makeInt(Me)
		End Function

		Public Overrides Function distinct() As IntStream Implements IntStream.distinct
			' While functional and quick to implement, this approach is not very efficient.
			' An efficient version requires an int-specific map/set implementation.
			Return boxed().distinct().mapToInt(i -> i)
		End Function

		' Terminal ops from IntStream

		Public Overrides Sub forEach(ByVal action As java.util.function.IntConsumer) Implements IntStream.forEach
			evaluate(ForEachOps.makeInt(action, False))
		End Sub

		Public Overrides Sub forEachOrdered(ByVal action As java.util.function.IntConsumer) Implements IntStream.forEachOrdered
			evaluate(ForEachOps.makeInt(action, True))
		End Sub

		Public Overrides Function sum() As Integer Implements IntStream.sum
			Return reduce(0, Integer?::sum)
		End Function

		Public Overrides Function min() As java.util.OptionalInt Implements IntStream.min
			Return reduce(Math::min)
		End Function

		Public Overrides Function max() As java.util.OptionalInt Implements IntStream.max
			Return reduce(Math::max)
		End Function

		Public Overrides Function count() As Long Implements IntStream.count
			Return mapToLong(e -> 1L).sum()
		End Function

		Public Overrides Function average() As java.util.OptionalDouble Implements IntStream.average
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Dim avg As Long() = collect(() -> New Long(1){}, (ll, i) -> { ll(0); ll(1) += i; }, (ll, rr) -> { ll(0) += rr(0); ll(1) += rr(1); })
				ll(0) += 1
			Return If(avg(0) > 0, java.util.OptionalDouble.of(CDbl(avg(1)) / avg(0)), java.util.OptionalDouble.empty())
		End Function

		Public Overrides Function summaryStatistics() As java.util.IntSummaryStatistics Implements IntStream.summaryStatistics
			Return collect(java.util.IntSummaryStatistics::New, java.util.IntSummaryStatistics::accept, java.util.IntSummaryStatistics::combine)
		End Function

		Public Overrides Function reduce(ByVal identity As Integer, ByVal op As java.util.function.IntBinaryOperator) As Integer Implements IntStream.reduce
			Return evaluate(ReduceOps.makeInt(identity, op))
		End Function

		Public Overrides Function reduce(ByVal op As java.util.function.IntBinaryOperator) As java.util.OptionalInt Implements IntStream.reduce
			Return evaluate(ReduceOps.makeInt(op))
		End Function

		Public Overrides Function collect(Of R)(ByVal supplier As java.util.function.Supplier(Of R), ByVal accumulator As java.util.function.ObjIntConsumer(Of R), ByVal combiner As java.util.function.BiConsumer(Of R, R)) As R Implements IntStream.collect
			Dim [operator] As java.util.function.BinaryOperator(Of R) = (left, right) ->
				combiner.accept(left, right)
				Return left
			Return evaluate(ReduceOps.makeInt(supplier, accumulator, [operator]))
		End Function

		Public Overrides Function anyMatch(ByVal predicate As java.util.function.IntPredicate) As Boolean Implements IntStream.anyMatch
			Return evaluate(MatchOps.makeInt(predicate, MatchOps.MatchKind.ANY))
		End Function

		Public Overrides Function allMatch(ByVal predicate As java.util.function.IntPredicate) As Boolean Implements IntStream.allMatch
			Return evaluate(MatchOps.makeInt(predicate, MatchOps.MatchKind.ALL))
		End Function

		Public Overrides Function noneMatch(ByVal predicate As java.util.function.IntPredicate) As Boolean Implements IntStream.noneMatch
			Return evaluate(MatchOps.makeInt(predicate, MatchOps.MatchKind.NONE))
		End Function

		Public Overrides Function findFirst() As java.util.OptionalInt Implements IntStream.findFirst
			Return evaluate(FindOps.makeInt(True))
		End Function

		Public Overrides Function findAny() As java.util.OptionalInt Implements IntStream.findAny
			Return evaluate(FindOps.makeInt(False))
		End Function

		Public Overrides Function toArray() As Integer() Implements IntStream.toArray
			Return Nodes.flattenInt(CType(evaluateToArrayNode(Integer?() ::New), Node.OfInt)).asPrimitiveArray()
		End Function

		'

		''' <summary>
		''' Source stage of an IntStream.
		''' </summary>
		''' @param <E_IN> type of elements in the upstream source
		''' @since 1.8 </param>
		Friend Class Head(Of E_IN)
			Inherits IntPipeline(Of E_IN)

			''' <summary>
			''' Constructor for the source stage of an IntStream.
			''' </summary>
			''' <param name="source"> {@code Supplier<Spliterator>} describing the stream
			'''               source </param>
			''' <param name="sourceFlags"> the source flags for the stream source, described
			'''                    in <seealso cref="StreamOpFlag"/> </param>
			''' <param name="parallel"> {@code true} if the pipeline is parallel </param>
			Friend Sub New(Of T1 As java.util.Spliterator(Of Integer?)(ByVal source As java.util.function.Supplier(Of T1), ByVal sourceFlags As Integer, ByVal parallel As Boolean)
				MyBase.New(source, sourceFlags, parallel)
			End Sub

			''' <summary>
			''' Constructor for the source stage of an IntStream.
			''' </summary>
			''' <param name="source"> {@code Spliterator} describing the stream source </param>
			''' <param name="sourceFlags"> the source flags for the stream source, described
			'''                    in <seealso cref="StreamOpFlag"/> </param>
			''' <param name="parallel"> {@code true} if the pipeline is parallel </param>
			Friend Sub New(ByVal source As java.util.Spliterator(Of Integer?), ByVal sourceFlags As Integer, ByVal parallel As Boolean)
				MyBase.New(source, sourceFlags, parallel)
			End Sub

			Friend Overrides Function opIsStateful() As Boolean
				Throw New UnsupportedOperationException
			End Function

			Friend Overrides Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of Integer?)) As Sink(Of E_IN)
				Throw New UnsupportedOperationException
			End Function

			' Optimized sequential terminal operations for the head of the pipeline

			Public Overrides Sub forEach(ByVal action As java.util.function.IntConsumer)
				If Not parallel Then
					adapt(sourceStageSpliterator()).forEachRemaining(action)
				Else
					MyBase.forEach(action)
				End If
			End Sub

			Public Overrides Sub forEachOrdered(ByVal action As java.util.function.IntConsumer)
				If Not parallel Then
					adapt(sourceStageSpliterator()).forEachRemaining(action)
				Else
					MyBase.forEachOrdered(action)
				End If
			End Sub
		End Class

		''' <summary>
		''' Base class for a stateless intermediate stage of an IntStream
		''' </summary>
		''' @param <E_IN> type of elements in the upstream source
		''' @since 1.8 </param>
		Friend MustInherit Class StatelessOp(Of E_IN)
			Inherits IntPipeline(Of E_IN)

			''' <summary>
			''' Construct a new IntStream by appending a stateless intermediate
			''' operation to an existing stream. </summary>
			''' <param name="upstream"> The upstream pipeline stage </param>
			''' <param name="inputShape"> The stream shape for the upstream pipeline stage </param>
			''' <param name="opFlags"> Operation flags for the new stage </param>
			Friend Sub New(Of T1)(ByVal upstream As AbstractPipeline(Of T1), ByVal inputShape As StreamShape, ByVal opFlags As Integer)
				MyBase.New(upstream, opFlags)
				Debug.Assert(upstream.outputShape = inputShape)
			End Sub

			Friend Overrides Function opIsStateful() As Boolean
				Return False
			End Function
		End Class

		''' <summary>
		''' Base class for a stateful intermediate stage of an IntStream.
		''' </summary>
		''' @param <E_IN> type of elements in the upstream source
		''' @since 1.8 </param>
		Friend MustInherit Class StatefulOp(Of E_IN)
			Inherits IntPipeline(Of E_IN)

			''' <summary>
			''' Construct a new IntStream by appending a stateful intermediate
			''' operation to an existing stream. </summary>
			''' <param name="upstream"> The upstream pipeline stage </param>
			''' <param name="inputShape"> The stream shape for the upstream pipeline stage </param>
			''' <param name="opFlags"> Operation flags for the new stage </param>
			Friend Sub New(Of T1)(ByVal upstream As AbstractPipeline(Of T1), ByVal inputShape As StreamShape, ByVal opFlags As Integer)
				MyBase.New(upstream, opFlags)
				Debug.Assert(upstream.outputShape = inputShape)
			End Sub

			Friend Overrides Function opIsStateful() As Boolean
				Return True
			End Function

			Friend MustOverride Overrides Function opEvaluateParallel(Of P_IN)(ByVal helper As PipelineHelper(Of Integer?), ByVal spliterator As java.util.Spliterator(Of P_IN), ByVal generator As java.util.function.IntFunction(Of Integer?())) As Node(Of Integer?)
		End Class
	End Class

End Namespace