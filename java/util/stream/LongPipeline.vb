Imports System
Imports System.Diagnostics

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
Namespace java.util.stream


	''' <summary>
	''' Abstract base class for an intermediate pipeline stage or pipeline source
	''' stage implementing whose elements are of type {@code long}.
	''' </summary>
	''' @param <E_IN> type of elements in the upstream source
	''' @since 1.8 </param>
	Friend MustInherit Class LongPipeline(Of E_IN)
		Inherits AbstractPipeline(Of E_IN, Long?, LongStream)
		Implements LongStream

		''' <summary>
		''' Constructor for the head of a stream pipeline.
		''' </summary>
		''' <param name="source"> {@code Supplier<Spliterator>} describing the stream source </param>
		''' <param name="sourceFlags"> the source flags for the stream source, described in
		'''        <seealso cref="StreamOpFlag"/> </param>
		''' <param name="parallel"> {@code true} if the pipeline is parallel </param>
		Friend Sub New(Of T1 As java.util.Spliterator(Of Long?)(  source As java.util.function.Supplier(Of T1),   sourceFlags As Integer,   parallel As Boolean)
			MyBase.New(source, sourceFlags, parallel)
		End Sub

		''' <summary>
		''' Constructor for the head of a stream pipeline.
		''' </summary>
		''' <param name="source"> {@code Spliterator} describing the stream source </param>
		''' <param name="sourceFlags"> the source flags for the stream source, described in
		'''        <seealso cref="StreamOpFlag"/> </param>
		''' <param name="parallel"> {@code true} if the pipeline is parallel </param>
		Friend Sub New(  source As java.util.Spliterator(Of Long?),   sourceFlags As Integer,   parallel As Boolean)
			MyBase.New(source, sourceFlags, parallel)
		End Sub

		''' <summary>
		''' Constructor for appending an intermediate operation onto an existing pipeline.
		''' </summary>
		''' <param name="upstream"> the upstream element source. </param>
		''' <param name="opFlags"> the operation flags </param>
		Friend Sub New(Of T1)(  upstream As AbstractPipeline(Of T1),   opFlags As Integer)
			MyBase.New(upstream, opFlags)
		End Sub

		''' <summary>
		''' Adapt a {@code Sink<Long> to an {@code LongConsumer}, ideally simply
		''' by casting.
		''' </summary>
		Private Shared Function adapt(  sink As Sink(Of Long?)) As java.util.function.LongConsumer
			If TypeOf sink Is java.util.function.LongConsumer Then
				Return CType(sink, java.util.function.LongConsumer)
			Else
				If Tripwire.ENABLED Then Tripwire.trip(GetType(AbstractPipeline), "using LongStream.adapt(Sink<Long> s)")
				Return sink::accept
			End If
		End Function

		''' <summary>
		''' Adapt a {@code Spliterator<Long>} to a {@code Spliterator.OfLong}.
		''' 
		''' @implNote
		''' The implementation attempts to cast to a Spliterator.OfLong, and throws
		''' an exception if this cast is not possible.
		''' </summary>
		Private Shared Function adapt(  s As java.util.Spliterator(Of Long?)) As java.util.Spliterator.OfLong
			If TypeOf s Is java.util.Spliterator.OfLong Then
				Return CType(s, java.util.Spliterator.OfLong)
			Else
				If Tripwire.ENABLED Then Tripwire.trip(GetType(AbstractPipeline), "using LongStream.adapt(Spliterator<Long> s)")
				Throw New UnsupportedOperationException("LongStream.adapt(Spliterator<Long> s)")
			End If
		End Function


		' Shape-specific methods

		Friend  Overrides ReadOnly Property  outputShape As StreamShape
			Get
				Return StreamShape.LONG_VALUE
			End Get
		End Property

		Friend Overrides Function evaluateToNode(Of P_IN)(  helper As PipelineHelper(Of Long?),   spliterator As java.util.Spliterator(Of P_IN),   flattenTree As Boolean,   generator As java.util.function.IntFunction(Of Long?())) As Node(Of Long?)
			Return Nodes.collectLong(helper, spliterator, flattenTree)
		End Function

		Friend Overrides Function wrap(Of P_IN)(  ph As PipelineHelper(Of Long?),   supplier As java.util.function.Supplier(Of java.util.Spliterator(Of P_IN)),   isParallel As Boolean) As java.util.Spliterator(Of Long?)
			Return New StreamSpliterators.LongWrappingSpliterator(Of )(ph, supplier, isParallel)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Overrides Function lazySpliterator(Of T1 As java.util.Spliterator(Of Long?)(  supplier As java.util.function.Supplier(Of T1)) As java.util.Spliterator.OfLong
			Return New StreamSpliterators.DelegatingSpliterator.OfLong(CType(supplier, java.util.function.Supplier(Of java.util.Spliterator.OfLong)))
		End Function

		Friend Overrides Sub forEachWithCancel(  spliterator As java.util.Spliterator(Of Long?),   sink As Sink(Of Long?))
			Dim spl As java.util.Spliterator.OfLong = adapt(spliterator)
			Dim adaptedSink As java.util.function.LongConsumer = adapt(sink)
			Do
			Loop While (Not sink.cancellationRequested()) AndAlso spl.tryAdvance(adaptedSink)
		End Sub

		Friend Overrides Function makeNodeBuilder(  exactSizeIfKnown As Long,   generator As java.util.function.IntFunction(Of Long?())) As Node.Builder(Of Long?)
			Return Nodes.longBuilder(exactSizeIfKnown)
		End Function


		' LongStream

		Public Overrides Function [iterator]() As java.util.PrimitiveIterator.OfLong Implements LongStream.iterator
			Return java.util.Spliterators.iterator(spliterator())
		End Function

		Public Overrides Function spliterator() As java.util.Spliterator.OfLong Implements LongStream.spliterator
			Return adapt(MyBase.spliterator())
		End Function

		' Stateless intermediate ops from LongStream

		Public Overrides Function asDoubleStream() As DoubleStream Implements LongStream.asDoubleStream
			Return New StatelessOpAnonymousInnerClassHelper(Of E_IN)
		End Function

		Private Class StatelessOpAnonymousInnerClassHelper(Of E_IN)
			Inherits StatelessOp(Of E_IN)

			Friend Overrides Function opWrapSink(  flags As Integer,   sink As Sink(Of Double?)) As Sink(Of Long?)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return New Sink.ChainedLong<java.lang.Double>(sink)
	'			{
	'				@Override public  Sub  accept(long t)
	'				{
	'					downstream.accept((double) t);
	'				}
	'			};
			End Function
		End Class

		Public Overrides Function boxed() As Stream(Of Long?) Implements LongStream.boxed
			Return mapToObj(Long?::valueOf)
		End Function

		Public Overrides Function map(  mapper As java.util.function.LongUnaryOperator) As LongStream Implements LongStream.map
			java.util.Objects.requireNonNull(mapper)
			Return New StatelessOpAnonymousInnerClassHelper(Of E_IN)
		End Function

		Private Class StatelessOpAnonymousInnerClassHelper(Of E_IN)
			Inherits StatelessOp(Of E_IN)

			Friend Overrides Function opWrapSink(  flags As Integer,   sink As Sink(Of Long?)) As Sink(Of Long?)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return New Sink.ChainedLong<java.lang.Long>(sink)
	'			{
	'				@Override public  Sub  accept(long t)
	'				{
	'					downstream.accept(mapper.applyAsLong(t));
	'				}
	'			};
			End Function
		End Class

		Public Overrides Function mapToObj(Of U, T1 As U)(  mapper As java.util.function.LongFunction(Of T1)) As Stream(Of U) Implements LongStream.mapToObj
			java.util.Objects.requireNonNull(mapper)
			Return New StatelessOpAnonymousInnerClassHelper(Of E_IN, E_OUT)
		End Function

		Private Class StatelessOpAnonymousInnerClassHelper(Of E_IN, E_OUT)
			Inherits StatelessOp(Of E_IN, E_OUT)

			Friend Overrides Function opWrapSink(  flags As Integer,   sink As Sink(Of U)) As Sink(Of Long?)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return New Sink.ChainedLong<U>(sink)
	'			{
	'				@Override public  Sub  accept(long t)
	'				{
	'					downstream.accept(mapper.apply(t));
	'				}
	'			};
			End Function
		End Class

		Public Overrides Function mapToInt(  mapper As java.util.function.LongToIntFunction) As IntStream Implements LongStream.mapToInt
			java.util.Objects.requireNonNull(mapper)
			Return New StatelessOpAnonymousInnerClassHelper(Of E_IN)
		End Function

		Private Class StatelessOpAnonymousInnerClassHelper(Of E_IN)
			Inherits StatelessOp(Of E_IN)

			Friend Overrides Function opWrapSink(  flags As Integer,   sink As Sink(Of Integer?)) As Sink(Of Long?)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return New Sink.ChainedLong<java.lang.Integer>(sink)
	'			{
	'				@Override public  Sub  accept(long t)
	'				{
	'					downstream.accept(mapper.applyAsInt(t));
	'				}
	'			};
			End Function
		End Class

		Public Overrides Function mapToDouble(  mapper As java.util.function.LongToDoubleFunction) As DoubleStream Implements LongStream.mapToDouble
			java.util.Objects.requireNonNull(mapper)
			Return New StatelessOpAnonymousInnerClassHelper2(Of E_IN)
		End Function

		Private Class StatelessOpAnonymousInnerClassHelper2(Of E_IN)
			Inherits StatelessOp(Of E_IN)

			Friend Overrides Function opWrapSink(  flags As Integer,   sink As Sink(Of Double?)) As Sink(Of Long?)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return New Sink.ChainedLong<java.lang.Double>(sink)
	'			{
	'				@Override public  Sub  accept(long t)
	'				{
	'					downstream.accept(mapper.applyAsDouble(t));
	'				}
	'			};
			End Function
		End Class

		Public Overrides Function flatMap(Of T1 As LongStream)(  mapper As java.util.function.LongFunction(Of T1)) As LongStream Implements LongStream.flatMap
			Return New StatelessOpAnonymousInnerClassHelper2(Of E_IN)
		End Function

		Private Class StatelessOpAnonymousInnerClassHelper2(Of E_IN)
			Inherits StatelessOp(Of E_IN)

			Friend Overrides Function opWrapSink(  flags As Integer,   sink As Sink(Of Long?)) As Sink(Of Long?)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return New Sink.ChainedLong<java.lang.Long>(sink)
	'			{
	'				@Override public  Sub  begin(long size)
	'				{
	'					downstream.begin(-1);
	'				}
	'
	'				@Override public  Sub  accept(long t)
	'				{
	'					try (LongStream result = mapper.apply(t))
	'					{
	'						' We can do better that this too; optimize for depth=0 case and just grab spliterator and forEach it
	'						if (result != Nothing)
	'							result.sequential().forEach(i -> downstream.accept(i));
	'					}
	'				}
	'			};
			End Function
		End Class

		Public Overrides Function unordered() As LongStream
			If Not ordered Then Return Me
			Return New StatelessOpAnonymousInnerClassHelper3(Of E_IN)
		End Function

		Private Class StatelessOpAnonymousInnerClassHelper3(Of E_IN)
			Inherits StatelessOp(Of E_IN)

			Friend Overrides Function opWrapSink(  flags As Integer,   sink As Sink(Of Long?)) As Sink(Of Long?)
				Return sink
			End Function
		End Class

		Public Overrides Function filter(  predicate As java.util.function.LongPredicate) As LongStream Implements LongStream.filter
			java.util.Objects.requireNonNull(predicate)
			Return New StatelessOpAnonymousInnerClassHelper4(Of E_IN)
		End Function

		Private Class StatelessOpAnonymousInnerClassHelper4(Of E_IN)
			Inherits StatelessOp(Of E_IN)

			Friend Overrides Function opWrapSink(  flags As Integer,   sink As Sink(Of Long?)) As Sink(Of Long?)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return New Sink.ChainedLong<java.lang.Long>(sink)
	'			{
	'				@Override public  Sub  begin(long size)
	'				{
	'					downstream.begin(-1);
	'				}
	'
	'				@Override public  Sub  accept(long t)
	'				{
	'					if (predicate.test(t))
	'						downstream.accept(t);
	'				}
	'			};
			End Function
		End Class

		Public Overrides Function peek(  action As java.util.function.LongConsumer) As LongStream Implements LongStream.peek
			java.util.Objects.requireNonNull(action)
			Return New StatelessOpAnonymousInnerClassHelper5(Of E_IN)
		End Function

		Private Class StatelessOpAnonymousInnerClassHelper5(Of E_IN)
			Inherits StatelessOp(Of E_IN)

			Friend Overrides Function opWrapSink(  flags As Integer,   sink As Sink(Of Long?)) As Sink(Of Long?)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return New Sink.ChainedLong<java.lang.Long>(sink)
	'			{
	'				@Override public  Sub  accept(long t)
	'				{
	'					action.accept(t);
	'					downstream.accept(t);
	'				}
	'			};
			End Function
		End Class

		' Stateful intermediate ops from LongStream

		Public Overrides Function limit(  maxSize As Long) As LongStream Implements LongStream.limit
			If maxSize < 0 Then Throw New IllegalArgumentException(Convert.ToString(maxSize))
			Return SliceOps.makeLong(Me, 0, maxSize)
		End Function

		Public Overrides Function skip(  n As Long) As LongStream Implements LongStream.skip
			If n < 0 Then Throw New IllegalArgumentException(Convert.ToString(n))
			If n = 0 Then
				Return Me
			Else
				Return SliceOps.makeLong(Me, n, -1)
			End If
		End Function

		Public Overrides Function sorted() As LongStream Implements LongStream.sorted
			Return SortedOps.makeLong(Me)
		End Function

		Public Overrides Function distinct() As LongStream Implements LongStream.distinct
			' While functional and quick to implement, this approach is not very efficient.
			' An efficient version requires a long-specific map/set implementation.
			Return boxed().distinct().mapToLong(i -> (Long) i)
		End Function

		' Terminal ops from LongStream

		Public Overrides Sub forEach(  action As java.util.function.LongConsumer) Implements LongStream.forEach
			evaluate(ForEachOps.makeLong(action, False))
		End Sub

		Public Overrides Sub forEachOrdered(  action As java.util.function.LongConsumer) Implements LongStream.forEachOrdered
			evaluate(ForEachOps.makeLong(action, True))
		End Sub

		Public Overrides Function sum() As Long Implements LongStream.sum
			' use better algorithm to compensate for intermediate overflow?
			Return reduce(0, Long?::sum)
		End Function

		Public Overrides Function min() As java.util.OptionalLong Implements LongStream.min
			Return reduce(Math::min)
		End Function

		Public Overrides Function max() As java.util.OptionalLong Implements LongStream.max
			Return reduce(Math::max)
		End Function

		Public Overrides Function average() As java.util.OptionalDouble Implements LongStream.average
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Dim avg As Long() = collect(() -> New Long(1){}, (ll, i) -> { ll(0); ll(1) += i; }, (ll, rr) -> { ll(0) += rr(0); ll(1) += rr(1); })
				ll(0) += 1
			Return If(avg(0) > 0, java.util.OptionalDouble.of(CDbl(avg(1)) / avg(0)), java.util.OptionalDouble.empty())
		End Function

		Public Overrides Function count() As Long Implements LongStream.count
			Return map(e -> 1L).sum()
		End Function

		Public Overrides Function summaryStatistics() As java.util.LongSummaryStatistics Implements LongStream.summaryStatistics
			Return collect(java.util.LongSummaryStatistics::New, java.util.LongSummaryStatistics::accept, java.util.LongSummaryStatistics::combine)
		End Function

		Public Overrides Function reduce(  identity As Long,   op As java.util.function.LongBinaryOperator) As Long Implements LongStream.reduce
			Return evaluate(ReduceOps.makeLong(identity, op))
		End Function

		Public Overrides Function reduce(  op As java.util.function.LongBinaryOperator) As java.util.OptionalLong Implements LongStream.reduce
			Return evaluate(ReduceOps.makeLong(op))
		End Function

		Public Overrides Function collect(Of R)(  supplier As java.util.function.Supplier(Of R),   accumulator As java.util.function.ObjLongConsumer(Of R),   combiner As java.util.function.BiConsumer(Of R, R)) As R Implements LongStream.collect
			Dim [operator] As java.util.function.BinaryOperator(Of R) = (left, right) ->
				combiner.accept(left, right)
				Return left
			Return evaluate(ReduceOps.makeLong(supplier, accumulator, [operator]))
		End Function

		Public Overrides Function anyMatch(  predicate As java.util.function.LongPredicate) As Boolean Implements LongStream.anyMatch
			Return evaluate(MatchOps.makeLong(predicate, MatchOps.MatchKind.ANY))
		End Function

		Public Overrides Function allMatch(  predicate As java.util.function.LongPredicate) As Boolean Implements LongStream.allMatch
			Return evaluate(MatchOps.makeLong(predicate, MatchOps.MatchKind.ALL))
		End Function

		Public Overrides Function noneMatch(  predicate As java.util.function.LongPredicate) As Boolean Implements LongStream.noneMatch
			Return evaluate(MatchOps.makeLong(predicate, MatchOps.MatchKind.NONE))
		End Function

		Public Overrides Function findFirst() As java.util.OptionalLong Implements LongStream.findFirst
			Return evaluate(FindOps.makeLong(True))
		End Function

		Public Overrides Function findAny() As java.util.OptionalLong Implements LongStream.findAny
			Return evaluate(FindOps.makeLong(False))
		End Function

		Public Overrides Function toArray() As Long() Implements LongStream.toArray
			Return Nodes.flattenLong(CType(evaluateToArrayNode(Long?() ::New), Node.OfLong)).asPrimitiveArray()
		End Function


		'

		''' <summary>
		''' Source stage of a LongPipeline.
		''' </summary>
		''' @param <E_IN> type of elements in the upstream source
		''' @since 1.8 </param>
		Friend Class Head(Of E_IN)
			Inherits LongPipeline(Of E_IN)

			''' <summary>
			''' Constructor for the source stage of a LongStream.
			''' </summary>
			''' <param name="source"> {@code Supplier<Spliterator>} describing the stream
			'''               source </param>
			''' <param name="sourceFlags"> the source flags for the stream source, described
			'''                    in <seealso cref="StreamOpFlag"/> </param>
			''' <param name="parallel"> {@code true} if the pipeline is parallel </param>
			Friend Sub New(Of T1 As java.util.Spliterator(Of Long?)(  source As java.util.function.Supplier(Of T1),   sourceFlags As Integer,   parallel As Boolean)
				MyBase.New(source, sourceFlags, parallel)
			End Sub

			''' <summary>
			''' Constructor for the source stage of a LongStream.
			''' </summary>
			''' <param name="source"> {@code Spliterator} describing the stream source </param>
			''' <param name="sourceFlags"> the source flags for the stream source, described
			'''                    in <seealso cref="StreamOpFlag"/> </param>
			''' <param name="parallel"> {@code true} if the pipeline is parallel </param>
			Friend Sub New(  source As java.util.Spliterator(Of Long?),   sourceFlags As Integer,   parallel As Boolean)
				MyBase.New(source, sourceFlags, parallel)
			End Sub

			Friend Overrides Function opIsStateful() As Boolean
				Throw New UnsupportedOperationException
			End Function

			Friend Overrides Function opWrapSink(  flags As Integer,   sink As Sink(Of Long?)) As Sink(Of E_IN)
				Throw New UnsupportedOperationException
			End Function

			' Optimized sequential terminal operations for the head of the pipeline

			Public Overrides Sub forEach(  action As java.util.function.LongConsumer)
				If Not parallel Then
					adapt(sourceStageSpliterator()).forEachRemaining(action)
				Else
					MyBase.forEach(action)
				End If
			End Sub

			Public Overrides Sub forEachOrdered(  action As java.util.function.LongConsumer)
				If Not parallel Then
					adapt(sourceStageSpliterator()).forEachRemaining(action)
				Else
					MyBase.forEachOrdered(action)
				End If
			End Sub
		End Class

		''' <summary>
		''' Base class for a stateless intermediate stage of a LongStream.
		''' </summary>
		''' @param <E_IN> type of elements in the upstream source
		''' @since 1.8 </param>
		Friend MustInherit Class StatelessOp(Of E_IN)
			Inherits LongPipeline(Of E_IN)

			''' <summary>
			''' Construct a new LongStream by appending a stateless intermediate
			''' operation to an existing stream. </summary>
			''' <param name="upstream"> The upstream pipeline stage </param>
			''' <param name="inputShape"> The stream shape for the upstream pipeline stage </param>
			''' <param name="opFlags"> Operation flags for the new stage </param>
			Friend Sub New(Of T1)(  upstream As AbstractPipeline(Of T1),   inputShape As StreamShape,   opFlags As Integer)
				MyBase.New(upstream, opFlags)
				Debug.Assert(upstream.outputShape = inputShape)
			End Sub

			Friend Overrides Function opIsStateful() As Boolean
				Return False
			End Function
		End Class

		''' <summary>
		''' Base class for a stateful intermediate stage of a LongStream.
		''' </summary>
		''' @param <E_IN> type of elements in the upstream source
		''' @since 1.8 </param>
		Friend MustInherit Class StatefulOp(Of E_IN)
			Inherits LongPipeline(Of E_IN)

			''' <summary>
			''' Construct a new LongStream by appending a stateful intermediate
			''' operation to an existing stream. </summary>
			''' <param name="upstream"> The upstream pipeline stage </param>
			''' <param name="inputShape"> The stream shape for the upstream pipeline stage </param>
			''' <param name="opFlags"> Operation flags for the new stage </param>
			Friend Sub New(Of T1)(  upstream As AbstractPipeline(Of T1),   inputShape As StreamShape,   opFlags As Integer)
				MyBase.New(upstream, opFlags)
				Debug.Assert(upstream.outputShape = inputShape)
			End Sub

			Friend Overrides Function opIsStateful() As Boolean
				Return True
			End Function

			Friend MustOverride Overrides Function opEvaluateParallel(Of P_IN)(  helper As PipelineHelper(Of Long?),   spliterator As java.util.Spliterator(Of P_IN),   generator As java.util.function.IntFunction(Of Long?())) As Node(Of Long?)
		End Class
	End Class

End Namespace