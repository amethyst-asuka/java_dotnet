Imports System
Imports System.Diagnostics

'
' * Copyright (c) 2013, 2014, Oracle and/or its affiliates. All rights reserved.
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
	''' stage implementing whose elements are of type {@code double}.
	''' </summary>
	''' @param <E_IN> type of elements in the upstream source
	''' 
	''' @since 1.8 </param>
	Friend MustInherit Class DoublePipeline(Of E_IN)
		Inherits AbstractPipeline(Of E_IN, Double?, DoubleStream)
		Implements DoubleStream

		''' <summary>
		''' Constructor for the head of a stream pipeline.
		''' </summary>
		''' <param name="source"> {@code Supplier<Spliterator>} describing the stream source </param>
		''' <param name="sourceFlags"> the source flags for the stream source, described in
		''' <seealso cref="StreamOpFlag"/> </param>
		Friend Sub New(Of T1 As java.util.Spliterator(Of Double?)(ByVal source As java.util.function.Supplier(Of T1), ByVal sourceFlags As Integer, ByVal parallel As Boolean)
			MyBase.New(source, sourceFlags, parallel)
		End Sub

		''' <summary>
		''' Constructor for the head of a stream pipeline.
		''' </summary>
		''' <param name="source"> {@code Spliterator} describing the stream source </param>
		''' <param name="sourceFlags"> the source flags for the stream source, described in
		''' <seealso cref="StreamOpFlag"/> </param>
		Friend Sub New(ByVal source As java.util.Spliterator(Of Double?), ByVal sourceFlags As Integer, ByVal parallel As Boolean)
			MyBase.New(source, sourceFlags, parallel)
		End Sub

		''' <summary>
		''' Constructor for appending an intermediate operation onto an existing
		''' pipeline.
		''' </summary>
		''' <param name="upstream"> the upstream element source. </param>
		''' <param name="opFlags"> the operation flags </param>
		Friend Sub New(Of T1)(ByVal upstream As AbstractPipeline(Of T1), ByVal opFlags As Integer)
			MyBase.New(upstream, opFlags)
		End Sub

		''' <summary>
		''' Adapt a {@code Sink<Double> to a {@code DoubleConsumer}, ideally simply
		''' by casting.
		''' </summary>
		Private Shared Function adapt(ByVal sink As Sink(Of Double?)) As java.util.function.DoubleConsumer
			If TypeOf sink Is java.util.function.DoubleConsumer Then
				Return CType(sink, java.util.function.DoubleConsumer)
			Else
				If Tripwire.ENABLED Then Tripwire.trip(GetType(AbstractPipeline), "using DoubleStream.adapt(Sink<Double> s)")
				Return sink::accept
			End If
		End Function

		''' <summary>
		''' Adapt a {@code Spliterator<Double>} to a {@code Spliterator.OfDouble}.
		''' 
		''' @implNote
		''' The implementation attempts to cast to a Spliterator.OfDouble, and throws
		''' an exception if this cast is not possible.
		''' </summary>
		Private Shared Function adapt(ByVal s As java.util.Spliterator(Of Double?)) As java.util.Spliterator.OfDouble
			If TypeOf s Is java.util.Spliterator.OfDouble Then
				Return CType(s, java.util.Spliterator.OfDouble)
			Else
				If Tripwire.ENABLED Then Tripwire.trip(GetType(AbstractPipeline), "using DoubleStream.adapt(Spliterator<Double> s)")
				Throw New UnsupportedOperationException("DoubleStream.adapt(Spliterator<Double> s)")
			End If
		End Function


		' Shape-specific methods

		Friend Property Overrides outputShape As StreamShape
			Get
				Return StreamShape.DOUBLE_VALUE
			End Get
		End Property

		Friend Overrides Function evaluateToNode(Of P_IN)(ByVal helper As PipelineHelper(Of Double?), ByVal spliterator As java.util.Spliterator(Of P_IN), ByVal flattenTree As Boolean, ByVal generator As java.util.function.IntFunction(Of Double?())) As Node(Of Double?)
			Return Nodes.collectDouble(helper, spliterator, flattenTree)
		End Function

		Friend Overrides Function wrap(Of P_IN)(ByVal ph As PipelineHelper(Of Double?), ByVal supplier As java.util.function.Supplier(Of java.util.Spliterator(Of P_IN)), ByVal isParallel As Boolean) As java.util.Spliterator(Of Double?)
			Return New StreamSpliterators.DoubleWrappingSpliterator(Of )(ph, supplier, isParallel)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Overrides Function lazySpliterator(Of T1 As java.util.Spliterator(Of Double?)(ByVal supplier As java.util.function.Supplier(Of T1)) As java.util.Spliterator.OfDouble
			Return New StreamSpliterators.DelegatingSpliterator.OfDouble(CType(supplier, java.util.function.Supplier(Of java.util.Spliterator.OfDouble)))
		End Function

		Friend Overrides Sub forEachWithCancel(ByVal spliterator As java.util.Spliterator(Of Double?), ByVal sink As Sink(Of Double?))
			Dim spl As java.util.Spliterator.OfDouble = adapt(spliterator)
			Dim adaptedSink As java.util.function.DoubleConsumer = adapt(sink)
			Do
			Loop While (Not sink.cancellationRequested()) AndAlso spl.tryAdvance(adaptedSink)
		End Sub

		Friend Overrides Function makeNodeBuilder(ByVal exactSizeIfKnown As Long, ByVal generator As java.util.function.IntFunction(Of Double?())) As Node.Builder(Of Double?)
			Return Nodes.doubleBuilder(exactSizeIfKnown)
		End Function


		' DoubleStream

		Public Overrides Function [iterator]() As java.util.PrimitiveIterator.OfDouble Implements DoubleStream.iterator
			Return java.util.Spliterators.iterator(spliterator())
		End Function

		Public Overrides Function spliterator() As java.util.Spliterator.OfDouble Implements DoubleStream.spliterator
			Return adapt(MyBase.spliterator())
		End Function

		' Stateless intermediate ops from DoubleStream

		Public Overrides Function boxed() As Stream(Of Double?) Implements DoubleStream.boxed
			Return mapToObj(Double?::valueOf)
		End Function

		Public Overrides Function map(ByVal mapper As java.util.function.DoubleUnaryOperator) As DoubleStream Implements DoubleStream.map
			java.util.Objects.requireNonNull(mapper)
			Return New StatelessOpAnonymousInnerClassHelper(Of E_IN)
		End Function

		Private Class StatelessOpAnonymousInnerClassHelper(Of E_IN)
			Inherits StatelessOp(Of E_IN)

			Friend Overrides Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of Double?)) As Sink(Of Double?)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return New Sink.ChainedDouble<java.lang.Double>(sink)
	'			{
	'				@Override public  Sub  accept(double t)
	'				{
	'					downstream.accept(mapper.applyAsDouble(t));
	'				}
	'			};
			End Function
		End Class

		Public Overrides Function mapToObj(Of U, T1 As U)(ByVal mapper As java.util.function.DoubleFunction(Of T1)) As Stream(Of U) Implements DoubleStream.mapToObj
			java.util.Objects.requireNonNull(mapper)
			Return New StatelessOpAnonymousInnerClassHelper(Of E_IN, E_OUT)
		End Function

		Private Class StatelessOpAnonymousInnerClassHelper(Of E_IN, E_OUT)
			Inherits StatelessOp(Of E_IN, E_OUT)

			Friend Overrides Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of U)) As Sink(Of Double?)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return New Sink.ChainedDouble<U>(sink)
	'			{
	'				@Override public  Sub  accept(double t)
	'				{
	'					downstream.accept(mapper.apply(t));
	'				}
	'			};
			End Function
		End Class

		Public Overrides Function mapToInt(ByVal mapper As java.util.function.DoubleToIntFunction) As IntStream Implements DoubleStream.mapToInt
			java.util.Objects.requireNonNull(mapper)
			Return New StatelessOpAnonymousInnerClassHelper(Of E_IN)
		End Function

		Private Class StatelessOpAnonymousInnerClassHelper(Of E_IN)
			Inherits StatelessOp(Of E_IN)

			Friend Overrides Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of Integer?)) As Sink(Of Double?)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return New Sink.ChainedDouble<java.lang.Integer>(sink)
	'			{
	'				@Override public  Sub  accept(double t)
	'				{
	'					downstream.accept(mapper.applyAsInt(t));
	'				}
	'			};
			End Function
		End Class

		Public Overrides Function mapToLong(ByVal mapper As java.util.function.DoubleToLongFunction) As LongStream Implements DoubleStream.mapToLong
			java.util.Objects.requireNonNull(mapper)
			Return New StatelessOpAnonymousInnerClassHelper(Of E_IN)
		End Function

		Private Class StatelessOpAnonymousInnerClassHelper(Of E_IN)
			Inherits StatelessOp(Of E_IN)

			Friend Overrides Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of Long?)) As Sink(Of Double?)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return New Sink.ChainedDouble<java.lang.Long>(sink)
	'			{
	'				@Override public  Sub  accept(double t)
	'				{
	'					downstream.accept(mapper.applyAsLong(t));
	'				}
	'			};
			End Function
		End Class

		Public Overrides Function flatMap(Of T1 As DoubleStream)(ByVal mapper As java.util.function.DoubleFunction(Of T1)) As DoubleStream Implements DoubleStream.flatMap
			Return New StatelessOpAnonymousInnerClassHelper2(Of E_IN)
		End Function

		Private Class StatelessOpAnonymousInnerClassHelper2(Of E_IN)
			Inherits StatelessOp(Of E_IN)

			Friend Overrides Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of Double?)) As Sink(Of Double?)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return New Sink.ChainedDouble<java.lang.Double>(sink)
	'			{
	'				@Override public  Sub  begin(long size)
	'				{
	'					downstream.begin(-1);
	'				}
	'
	'				@Override public  Sub  accept(double t)
	'				{
	'					try (DoubleStream result = mapper.apply(t))
	'					{
	'						' We can do better that this too; optimize for depth=0 case and just grab spliterator and forEach it
	'						if (result != Nothing)
	'							result.sequential().forEach(i -> downstream.accept(i));
	'					}
	'				}
	'			};
			End Function
		End Class

		Public Overrides Function unordered() As DoubleStream
			If Not ordered Then Return Me
			Return New StatelessOpAnonymousInnerClassHelper3(Of E_IN)
		End Function

		Private Class StatelessOpAnonymousInnerClassHelper3(Of E_IN)
			Inherits StatelessOp(Of E_IN)

			Friend Overrides Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of Double?)) As Sink(Of Double?)
				Return sink
			End Function
		End Class

		Public Overrides Function filter(ByVal predicate As java.util.function.DoublePredicate) As DoubleStream Implements DoubleStream.filter
			java.util.Objects.requireNonNull(predicate)
			Return New StatelessOpAnonymousInnerClassHelper4(Of E_IN)
		End Function

		Private Class StatelessOpAnonymousInnerClassHelper4(Of E_IN)
			Inherits StatelessOp(Of E_IN)

			Friend Overrides Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of Double?)) As Sink(Of Double?)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return New Sink.ChainedDouble<java.lang.Double>(sink)
	'			{
	'				@Override public  Sub  begin(long size)
	'				{
	'					downstream.begin(-1);
	'				}
	'
	'				@Override public  Sub  accept(double t)
	'				{
	'					if (predicate.test(t))
	'						downstream.accept(t);
	'				}
	'			};
			End Function
		End Class

		Public Overrides Function peek(ByVal action As java.util.function.DoubleConsumer) As DoubleStream Implements DoubleStream.peek
			java.util.Objects.requireNonNull(action)
			Return New StatelessOpAnonymousInnerClassHelper5(Of E_IN)
		End Function

		Private Class StatelessOpAnonymousInnerClassHelper5(Of E_IN)
			Inherits StatelessOp(Of E_IN)

			Friend Overrides Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of Double?)) As Sink(Of Double?)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return New Sink.ChainedDouble<java.lang.Double>(sink)
	'			{
	'				@Override public  Sub  accept(double t)
	'				{
	'					action.accept(t);
	'					downstream.accept(t);
	'				}
	'			};
			End Function
		End Class

		' Stateful intermediate ops from DoubleStream

		Public Overrides Function limit(ByVal maxSize As Long) As DoubleStream Implements DoubleStream.limit
			If maxSize < 0 Then Throw New IllegalArgumentException(Convert.ToString(maxSize))
			Return SliceOps.makeDouble(Me, CLng(0), maxSize)
		End Function

		Public Overrides Function skip(ByVal n As Long) As DoubleStream Implements DoubleStream.skip
			If n < 0 Then Throw New IllegalArgumentException(Convert.ToString(n))
			If n = 0 Then
				Return Me
			Else
				Dim limit As Long = -1
				Return SliceOps.makeDouble(Me, n, limit)
			End If
		End Function

		Public Overrides Function sorted() As DoubleStream Implements DoubleStream.sorted
			Return SortedOps.makeDouble(Me)
		End Function

		Public Overrides Function distinct() As DoubleStream Implements DoubleStream.distinct
			' While functional and quick to implement, this approach is not very efficient.
			' An efficient version requires a double-specific map/set implementation.
			Return boxed().distinct().mapToDouble(i -> (Double) i)
		End Function

		' Terminal ops from DoubleStream

		Public Overrides Sub forEach(ByVal consumer As java.util.function.DoubleConsumer) Implements DoubleStream.forEach
			evaluate(ForEachOps.makeDouble(consumer, False))
		End Sub

		Public Overrides Sub forEachOrdered(ByVal consumer As java.util.function.DoubleConsumer) Implements DoubleStream.forEachOrdered
			evaluate(ForEachOps.makeDouble(consumer, True))
		End Sub

		Public Overrides Function sum() As Double Implements DoubleStream.sum
	'        
	'         * In the arrays allocated for the collect operation, index 0
	'         * holds the high-order bits of the running sum, index 1 holds
	'         * the low-order bits of the sum computed via compensated
	'         * summation, and index 2 holds the simple sum used to compute
	'         * the proper result if the stream contains infinite values of
	'         * the same sign.
	'         
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Dim summation As Double() = collect(() -> New Double(2){}, (ll, d) -> { Collectors.sumWithCompensation(ll, d); ll(2) += d; }, (ll, rr) -> { Collectors.sumWithCompensation(ll, rr(0)); Collectors.sumWithCompensation(ll, rr(1)); ll(2) += rr(2); })

			Return Collectors.computeFinalSum(summation)
		End Function

		Public Overrides Function min() As java.util.OptionalDouble Implements DoubleStream.min
			Return reduce(Math::min)
		End Function

		Public Overrides Function max() As java.util.OptionalDouble Implements DoubleStream.max
			Return reduce(Math::max)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' @implNote The {@code double} format can represent all
		''' consecutive integers in the range -2<sup>53</sup> to
		''' 2<sup>53</sup>. If the pipeline has more than 2<sup>53</sup>
		''' values, the divisor in the average computation will saturate at
		''' 2<sup>53</sup>, leading to additional numerical errors.
		''' </summary>
		Public Overrides Function average() As java.util.OptionalDouble Implements DoubleStream.average
	'        
	'         * In the arrays allocated for the collect operation, index 0
	'         * holds the high-order bits of the running sum, index 1 holds
	'         * the low-order bits of the sum computed via compensated
	'         * summation, index 2 holds the number of values seen, index 3
	'         * holds the simple sum.
	'         
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Dim avg As Double() = collect(() -> New Double(3){}, (ll, d) -> { ll(2); Collectors.sumWithCompensation(ll, d); ll(3) += d; }, (ll, rr) -> { Collectors.sumWithCompensation(ll, rr(0)); Collectors.sumWithCompensation(ll, rr(1)); ll(2) += rr(2); ll(3) += rr(3); })
				ll(2) += 1
			Return If(avg(2) > 0, java.util.OptionalDouble.of(Collectors.computeFinalSum(avg) / avg(2)), java.util.OptionalDouble.empty())
		End Function

		Public Overrides Function count() As Long Implements DoubleStream.count
			Return mapToLong(e -> 1L).sum()
		End Function

		Public Overrides Function summaryStatistics() As java.util.DoubleSummaryStatistics Implements DoubleStream.summaryStatistics
			Return collect(java.util.DoubleSummaryStatistics::New, java.util.DoubleSummaryStatistics::accept, java.util.DoubleSummaryStatistics::combine)
		End Function

		Public Overrides Function reduce(ByVal identity As Double, ByVal op As java.util.function.DoubleBinaryOperator) As Double Implements DoubleStream.reduce
			Return evaluate(ReduceOps.makeDouble(identity, op))
		End Function

		Public Overrides Function reduce(ByVal op As java.util.function.DoubleBinaryOperator) As java.util.OptionalDouble Implements DoubleStream.reduce
			Return evaluate(ReduceOps.makeDouble(op))
		End Function

		Public Overrides Function collect(Of R)(ByVal supplier As java.util.function.Supplier(Of R), ByVal accumulator As java.util.function.ObjDoubleConsumer(Of R), ByVal combiner As java.util.function.BiConsumer(Of R, R)) As R Implements DoubleStream.collect
			Dim [operator] As java.util.function.BinaryOperator(Of R) = (left, right) ->
				combiner.accept(left, right)
				Return left
			Return evaluate(ReduceOps.makeDouble(supplier, accumulator, [operator]))
		End Function

		Public Overrides Function anyMatch(ByVal predicate As java.util.function.DoublePredicate) As Boolean Implements DoubleStream.anyMatch
			Return evaluate(MatchOps.makeDouble(predicate, MatchOps.MatchKind.ANY))
		End Function

		Public Overrides Function allMatch(ByVal predicate As java.util.function.DoublePredicate) As Boolean Implements DoubleStream.allMatch
			Return evaluate(MatchOps.makeDouble(predicate, MatchOps.MatchKind.ALL))
		End Function

		Public Overrides Function noneMatch(ByVal predicate As java.util.function.DoublePredicate) As Boolean Implements DoubleStream.noneMatch
			Return evaluate(MatchOps.makeDouble(predicate, MatchOps.MatchKind.NONE))
		End Function

		Public Overrides Function findFirst() As java.util.OptionalDouble Implements DoubleStream.findFirst
			Return evaluate(FindOps.makeDouble(True))
		End Function

		Public Overrides Function findAny() As java.util.OptionalDouble Implements DoubleStream.findAny
			Return evaluate(FindOps.makeDouble(False))
		End Function

		Public Overrides Function toArray() As Double() Implements DoubleStream.toArray
			Return Nodes.flattenDouble(CType(evaluateToArrayNode(Double?() ::New), Node.OfDouble)).asPrimitiveArray()
		End Function

		'

		''' <summary>
		''' Source stage of a DoubleStream
		''' </summary>
		''' @param <E_IN> type of elements in the upstream source </param>
		Friend Class Head(Of E_IN)
			Inherits DoublePipeline(Of E_IN)

			''' <summary>
			''' Constructor for the source stage of a DoubleStream.
			''' </summary>
			''' <param name="source"> {@code Supplier<Spliterator>} describing the stream
			'''               source </param>
			''' <param name="sourceFlags"> the source flags for the stream source, described
			'''                    in <seealso cref="StreamOpFlag"/> </param>
			''' <param name="parallel"> {@code true} if the pipeline is parallel </param>
			Friend Sub New(Of T1 As java.util.Spliterator(Of Double?)(ByVal source As java.util.function.Supplier(Of T1), ByVal sourceFlags As Integer, ByVal parallel As Boolean)
				MyBase.New(source, sourceFlags, parallel)
			End Sub

			''' <summary>
			''' Constructor for the source stage of a DoubleStream.
			''' </summary>
			''' <param name="source"> {@code Spliterator} describing the stream source </param>
			''' <param name="sourceFlags"> the source flags for the stream source, described
			'''                    in <seealso cref="StreamOpFlag"/> </param>
			''' <param name="parallel"> {@code true} if the pipeline is parallel </param>
			Friend Sub New(ByVal source As java.util.Spliterator(Of Double?), ByVal sourceFlags As Integer, ByVal parallel As Boolean)
				MyBase.New(source, sourceFlags, parallel)
			End Sub

			Friend Overrides Function opIsStateful() As Boolean
				Throw New UnsupportedOperationException
			End Function

			Friend Overrides Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of Double?)) As Sink(Of E_IN)
				Throw New UnsupportedOperationException
			End Function

			' Optimized sequential terminal operations for the head of the pipeline

			Public Overrides Sub forEach(ByVal consumer As java.util.function.DoubleConsumer)
				If Not parallel Then
					adapt(sourceStageSpliterator()).forEachRemaining(consumer)
				Else
					MyBase.forEach(consumer)
				End If
			End Sub

			Public Overrides Sub forEachOrdered(ByVal consumer As java.util.function.DoubleConsumer)
				If Not parallel Then
					adapt(sourceStageSpliterator()).forEachRemaining(consumer)
				Else
					MyBase.forEachOrdered(consumer)
				End If
			End Sub

		End Class

		''' <summary>
		''' Base class for a stateless intermediate stage of a DoubleStream.
		''' </summary>
		''' @param <E_IN> type of elements in the upstream source
		''' @since 1.8 </param>
		Friend MustInherit Class StatelessOp(Of E_IN)
			Inherits DoublePipeline(Of E_IN)

			''' <summary>
			''' Construct a new DoubleStream by appending a stateless intermediate
			''' operation to an existing stream.
			''' </summary>
			''' <param name="upstream"> the upstream pipeline stage </param>
			''' <param name="inputShape"> the stream shape for the upstream pipeline stage </param>
			''' <param name="opFlags"> operation flags for the new stage </param>
			Friend Sub New(Of T1)(ByVal upstream As AbstractPipeline(Of T1), ByVal inputShape As StreamShape, ByVal opFlags As Integer)
				MyBase.New(upstream, opFlags)
				Debug.Assert(upstream.outputShape = inputShape)
			End Sub

			Friend Overrides Function opIsStateful() As Boolean
				Return False
			End Function
		End Class

		''' <summary>
		''' Base class for a stateful intermediate stage of a DoubleStream.
		''' </summary>
		''' @param <E_IN> type of elements in the upstream source
		''' @since 1.8 </param>
		Friend MustInherit Class StatefulOp(Of E_IN)
			Inherits DoublePipeline(Of E_IN)

			''' <summary>
			''' Construct a new DoubleStream by appending a stateful intermediate
			''' operation to an existing stream.
			''' </summary>
			''' <param name="upstream"> the upstream pipeline stage </param>
			''' <param name="inputShape"> the stream shape for the upstream pipeline stage </param>
			''' <param name="opFlags"> operation flags for the new stage </param>
			Friend Sub New(Of T1)(ByVal upstream As AbstractPipeline(Of T1), ByVal inputShape As StreamShape, ByVal opFlags As Integer)
				MyBase.New(upstream, opFlags)
				Debug.Assert(upstream.outputShape = inputShape)
			End Sub

			Friend Overrides Function opIsStateful() As Boolean
				Return True
			End Function

			Friend MustOverride Overrides Function opEvaluateParallel(Of P_IN)(ByVal helper As PipelineHelper(Of Double?), ByVal spliterator As java.util.Spliterator(Of P_IN), ByVal generator As java.util.function.IntFunction(Of Double?())) As Node(Of Double?)
		End Class
	End Class

End Namespace