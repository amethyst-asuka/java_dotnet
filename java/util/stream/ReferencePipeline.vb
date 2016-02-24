Imports System
Imports System.Diagnostics
Imports System.Collections.Generic

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
	''' Abstract base class for an intermediate pipeline stage or pipeline source
	''' stage implementing whose elements are of type {@code U}.
	''' </summary>
	''' @param <P_IN> type of elements in the upstream source </param>
	''' @param <P_OUT> type of elements in produced by this stage
	''' 
	''' @since 1.8 </param>
	Friend MustInherit Class ReferencePipeline(Of P_IN, P_OUT)
		Inherits AbstractPipeline(Of P_IN, P_OUT, Stream(Of P_OUT))
		Implements Stream(Of P_OUT)

		''' <summary>
		''' Constructor for the head of a stream pipeline.
		''' </summary>
		''' <param name="source"> {@code Supplier<Spliterator>} describing the stream source </param>
		''' <param name="sourceFlags"> the source flags for the stream source, described in
		'''        <seealso cref="StreamOpFlag"/> </param>
		''' <param name="parallel"> {@code true} if the pipeline is parallel </param>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Friend Sub New(Of T1 As java.util.Spliterator(Of ?)(ByVal source As java.util.function.Supplier(Of T1), ByVal sourceFlags As Integer, ByVal parallel As Boolean)
			MyBase.New(source, sourceFlags, parallel)
		End Sub

		''' <summary>
		''' Constructor for the head of a stream pipeline.
		''' </summary>
		''' <param name="source"> {@code Spliterator} describing the stream source </param>
		''' <param name="sourceFlags"> The source flags for the stream source, described in
		'''        <seealso cref="StreamOpFlag"/> </param>
		''' <param name="parallel"> {@code true} if the pipeline is parallel </param>
		Friend Sub New(Of T1)(ByVal source As java.util.Spliterator(Of T1), ByVal sourceFlags As Integer, ByVal parallel As Boolean)
			MyBase.New(source, sourceFlags, parallel)
		End Sub

		''' <summary>
		''' Constructor for appending an intermediate operation onto an existing
		''' pipeline.
		''' </summary>
		''' <param name="upstream"> the upstream element source. </param>
		Friend Sub New(Of T1)(ByVal upstream As AbstractPipeline(Of T1), ByVal opFlags As Integer)
			MyBase.New(upstream, opFlags)
		End Sub

		' Shape-specific methods

		Friend Property Overrides outputShape As StreamShape
			Get
				Return StreamShape.REFERENCE
			End Get
		End Property

		Friend Overrides Function evaluateToNode(Of P_IN)(ByVal helper As PipelineHelper(Of P_OUT), ByVal spliterator As java.util.Spliterator(Of P_IN), ByVal flattenTree As Boolean, ByVal generator As java.util.function.IntFunction(Of P_OUT())) As Node(Of P_OUT)
			Return Nodes.collect(helper, spliterator, flattenTree, generator)
		End Function

		Friend Overrides Function wrap(Of P_IN)(ByVal ph As PipelineHelper(Of P_OUT), ByVal supplier As java.util.function.Supplier(Of java.util.Spliterator(Of P_IN)), ByVal isParallel As Boolean) As java.util.Spliterator(Of P_OUT)
			Return New StreamSpliterators.WrappingSpliterator(Of )(ph, supplier, isParallel)
		End Function

		Friend Overrides Function lazySpliterator(Of T1 As java.util.Spliterator(Of P_OUT)(ByVal supplier As java.util.function.Supplier(Of T1)) As java.util.Spliterator(Of P_OUT)
			Return New StreamSpliterators.DelegatingSpliterator(Of )(supplier)
		End Function

		Friend Overrides Sub forEachWithCancel(ByVal spliterator As java.util.Spliterator(Of P_OUT), ByVal sink As Sink(Of P_OUT))
			Do
			Loop While (Not sink.cancellationRequested()) AndAlso spliterator.tryAdvance(sink)
		End Sub

		Friend Overrides Function makeNodeBuilder(ByVal exactSizeIfKnown As Long, ByVal generator As java.util.function.IntFunction(Of P_OUT())) As Node.Builder(Of P_OUT)
			Return Nodes.builder(exactSizeIfKnown, generator)
		End Function


		' BaseStream

		Public Overrides Function [iterator]() As IEnumerator(Of P_OUT)
			Return java.util.Spliterators.iterator(spliterator())
		End Function


		' Stream

		' Stateless intermediate operations from Stream

		Public Overrides Function unordered() As Stream(Of P_OUT)
			If Not ordered Then Return Me
			Return New StatelessOpAnonymousInnerClassHelper(Of E_IN, E_OUT)
		End Function

		Private Class StatelessOpAnonymousInnerClassHelper(Of E_IN, E_OUT)
			Inherits StatelessOp(Of E_IN, E_OUT)

			Friend Overrides Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of P_OUT)) As Sink(Of P_OUT)
				Return sink
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overrides Function filter(Of T1)(ByVal predicate As java.util.function.Predicate(Of T1)) As Stream(Of P_OUT) Implements Stream(Of P_OUT).filter
			java.util.Objects.requireNonNull(predicate)
			Return New StatelessOpAnonymousInnerClassHelper2(Of E_IN, E_OUT)
		End Function

		Private Class StatelessOpAnonymousInnerClassHelper2(Of E_IN, E_OUT)
			Inherits StatelessOp(Of E_IN, E_OUT)

			Friend Overrides Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of P_OUT)) As Sink(Of P_OUT)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return New Sink.ChainedReference<P_OUT, P_OUT>(sink)
	'			{
	'				@Override public void begin(long size)
	'				{
	'					downstream.begin(-1);
	'				}
	'
	'				@Override public void accept(P_OUT u)
	'				{
	'					if (predicate.test(u))
	'						downstream.accept(u);
	'				}
	'			};
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overrides Function map(Of R, T1 As R)(ByVal mapper As java.util.function.Function(Of T1)) As Stream(Of R) Implements Stream(Of P_OUT).map
			java.util.Objects.requireNonNull(mapper)
			Return New StatelessOpAnonymousInnerClassHelper3(Of E_IN, E_OUT)
		End Function

		Private Class StatelessOpAnonymousInnerClassHelper3(Of E_IN, E_OUT)
			Inherits StatelessOp(Of E_IN, E_OUT)

			Friend Overrides Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of R)) As Sink(Of P_OUT)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return New Sink.ChainedReference<P_OUT, R>(sink)
	'			{
	'				@Override public void accept(P_OUT u)
	'				{
	'					downstream.accept(mapper.apply(u));
	'				}
	'			};
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overrides Function mapToInt(Of T1)(ByVal mapper As java.util.function.ToIntFunction(Of T1)) As IntStream Implements Stream(Of P_OUT).mapToInt
			java.util.Objects.requireNonNull(mapper)
			Return New StatelessOpAnonymousInnerClassHelper(Of E_IN)
		End Function

		Private Class StatelessOpAnonymousInnerClassHelper(Of E_IN)
			Inherits StatelessOp(Of E_IN)

			Friend Overrides Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of Integer?)) As Sink(Of P_OUT)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return New Sink.ChainedReference<P_OUT, java.lang.Integer>(sink)
	'			{
	'				@Override public void accept(P_OUT u)
	'				{
	'					downstream.accept(mapper.applyAsInt(u));
	'				}
	'			};
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overrides Function mapToLong(Of T1)(ByVal mapper As java.util.function.ToLongFunction(Of T1)) As LongStream Implements Stream(Of P_OUT).mapToLong
			java.util.Objects.requireNonNull(mapper)
			Return New StatelessOpAnonymousInnerClassHelper(Of E_IN)
		End Function

		Private Class StatelessOpAnonymousInnerClassHelper(Of E_IN)
			Inherits StatelessOp(Of E_IN)

			Friend Overrides Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of Long?)) As Sink(Of P_OUT)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return New Sink.ChainedReference<P_OUT, java.lang.Long>(sink)
	'			{
	'				@Override public void accept(P_OUT u)
	'				{
	'					downstream.accept(mapper.applyAsLong(u));
	'				}
	'			};
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overrides Function mapToDouble(Of T1)(ByVal mapper As java.util.function.ToDoubleFunction(Of T1)) As DoubleStream Implements Stream(Of P_OUT).mapToDouble
			java.util.Objects.requireNonNull(mapper)
			Return New StatelessOpAnonymousInnerClassHelper(Of E_IN)
		End Function

		Private Class StatelessOpAnonymousInnerClassHelper(Of E_IN)
			Inherits StatelessOp(Of E_IN)

			Friend Overrides Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of Double?)) As Sink(Of P_OUT)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return New Sink.ChainedReference<P_OUT, java.lang.Double>(sink)
	'			{
	'				@Override public void accept(P_OUT u)
	'				{
	'					downstream.accept(mapper.applyAsDouble(u));
	'				}
	'			};
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overrides Function flatMap(Of R, T1 As Stream(Of ? As R)(ByVal mapper As java.util.function.Function(Of T1)) As Stream(Of R) Implements Stream(Of P_OUT).flatMap
			java.util.Objects.requireNonNull(mapper)
			' We can do better than this, by polling cancellationRequested when stream is infinite
			Return New StatelessOpAnonymousInnerClassHelper4(Of E_IN, E_OUT)
		End Function

		Private Class StatelessOpAnonymousInnerClassHelper4(Of E_IN, E_OUT)
			Inherits StatelessOp(Of E_IN, E_OUT)

			Friend Overrides Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of R)) As Sink(Of P_OUT)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return New Sink.ChainedReference<P_OUT, R>(sink)
	'			{
	'				@Override public void begin(long size)
	'				{
	'					downstream.begin(-1);
	'				}
	'
	'				@Override public void accept(P_OUT u)
	'				{
	'					try (Stream<? extends R> result = mapper.apply(u))
	'					{
	'						' We can do better that this too; optimize for depth=0 case and just grab spliterator and forEach it
	'						if (result != Nothing)
	'							result.sequential().forEach(downstream);
	'					}
	'				}
	'			};
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overrides Function flatMapToInt(Of T1 As IntStream)(ByVal mapper As java.util.function.Function(Of T1)) As IntStream Implements Stream(Of P_OUT).flatMapToInt
			java.util.Objects.requireNonNull(mapper)
			' We can do better than this, by polling cancellationRequested when stream is infinite
			Return New StatelessOpAnonymousInnerClassHelper2(Of E_IN)
		End Function

		Private Class StatelessOpAnonymousInnerClassHelper2(Of E_IN)
			Inherits StatelessOp(Of E_IN)

			Friend Overrides Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of Integer?)) As Sink(Of P_OUT)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return New Sink.ChainedReference<P_OUT, java.lang.Integer>(sink)
	'			{
	'				IntConsumer downstreamAsInt = downstream::accept;
	'				@Override public void begin(long size)
	'				{
	'					downstream.begin(-1);
	'				}
	'
	'				@Override public void accept(P_OUT u)
	'				{
	'					try (IntStream result = mapper.apply(u))
	'					{
	'						' We can do better that this too; optimize for depth=0 case and just grab spliterator and forEach it
	'						if (result != Nothing)
	'							result.sequential().forEach(downstreamAsInt);
	'					}
	'				}
	'			};
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overrides Function flatMapToDouble(Of T1 As DoubleStream)(ByVal mapper As java.util.function.Function(Of T1)) As DoubleStream Implements Stream(Of P_OUT).flatMapToDouble
			java.util.Objects.requireNonNull(mapper)
			' We can do better than this, by polling cancellationRequested when stream is infinite
			Return New StatelessOpAnonymousInnerClassHelper2(Of E_IN)
		End Function

		Private Class StatelessOpAnonymousInnerClassHelper2(Of E_IN)
			Inherits StatelessOp(Of E_IN)

			Friend Overrides Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of Double?)) As Sink(Of P_OUT)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return New Sink.ChainedReference<P_OUT, java.lang.Double>(sink)
	'			{
	'				DoubleConsumer downstreamAsDouble = downstream::accept;
	'				@Override public void begin(long size)
	'				{
	'					downstream.begin(-1);
	'				}
	'
	'				@Override public void accept(P_OUT u)
	'				{
	'					try (DoubleStream result = mapper.apply(u))
	'					{
	'						' We can do better that this too; optimize for depth=0 case and just grab spliterator and forEach it
	'						if (result != Nothing)
	'							result.sequential().forEach(downstreamAsDouble);
	'					}
	'				}
	'			};
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overrides Function flatMapToLong(Of T1 As LongStream)(ByVal mapper As java.util.function.Function(Of T1)) As LongStream Implements Stream(Of P_OUT).flatMapToLong
			java.util.Objects.requireNonNull(mapper)
			' We can do better than this, by polling cancellationRequested when stream is infinite
			Return New StatelessOpAnonymousInnerClassHelper2(Of E_IN)
		End Function

		Private Class StatelessOpAnonymousInnerClassHelper2(Of E_IN)
			Inherits StatelessOp(Of E_IN)

			Friend Overrides Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of Long?)) As Sink(Of P_OUT)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return New Sink.ChainedReference<P_OUT, java.lang.Long>(sink)
	'			{
	'				LongConsumer downstreamAsLong = downstream::accept;
	'				@Override public void begin(long size)
	'				{
	'					downstream.begin(-1);
	'				}
	'
	'				@Override public void accept(P_OUT u)
	'				{
	'					try (LongStream result = mapper.apply(u))
	'					{
	'						' We can do better that this too; optimize for depth=0 case and just grab spliterator and forEach it
	'						if (result != Nothing)
	'							result.sequential().forEach(downstreamAsLong);
	'					}
	'				}
	'			};
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overrides Function peek(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) As Stream(Of P_OUT) Implements Stream(Of P_OUT).peek
			java.util.Objects.requireNonNull(action)
			Return New StatelessOpAnonymousInnerClassHelper5(Of E_IN, E_OUT)
		End Function

		Private Class StatelessOpAnonymousInnerClassHelper5(Of E_IN, E_OUT)
			Inherits StatelessOp(Of E_IN, E_OUT)

			Friend Overrides Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of P_OUT)) As Sink(Of P_OUT)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return New Sink.ChainedReference<P_OUT, P_OUT>(sink)
	'			{
	'				@Override public void accept(P_OUT u)
	'				{
	'					action.accept(u);
	'					downstream.accept(u);
	'				}
	'			};
			End Function
		End Class

		' Stateful intermediate operations from Stream

		Public Overrides Function distinct() As Stream(Of P_OUT) Implements Stream(Of P_OUT).distinct
			Return DistinctOps.makeRef(Me)
		End Function

		Public Overrides Function sorted() As Stream(Of P_OUT) Implements Stream(Of P_OUT).sorted
			Return SortedOps.makeRef(Me)
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overrides Function sorted(Of T1)(ByVal comparator As IComparer(Of T1)) As Stream(Of P_OUT) Implements Stream(Of P_OUT).sorted
			Return SortedOps.makeRef(Me, comparator)
		End Function

		Public Overrides Function limit(ByVal maxSize As Long) As Stream(Of P_OUT) Implements Stream(Of P_OUT).limit
			If maxSize < 0 Then Throw New IllegalArgumentException(Convert.ToString(maxSize))
			Return SliceOps.makeRef(Me, 0, maxSize)
		End Function

		Public Overrides Function skip(ByVal n As Long) As Stream(Of P_OUT) Implements Stream(Of P_OUT).skip
			If n < 0 Then Throw New IllegalArgumentException(Convert.ToString(n))
			If n = 0 Then
				Return Me
			Else
				Return SliceOps.makeRef(Me, n, -1)
			End If
		End Function

		' Terminal operations from Stream

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overrides Sub forEach(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) Implements Stream(Of P_OUT).forEach
			evaluate(ForEachOps.makeRef(action, False))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overrides Sub forEachOrdered(Of T1)(ByVal action As java.util.function.Consumer(Of T1)) Implements Stream(Of P_OUT).forEachOrdered
			evaluate(ForEachOps.makeRef(action, True))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overrides Function toArray(Of A)(ByVal generator As java.util.function.IntFunction(Of A())) As A() Implements Stream(Of P_OUT).toArray
			' Since A has no relation to U (not possible to declare that A is an upper bound of U)
			' there will be no static type checking.
			' Therefore use a raw type and assume A == U rather than propagating the separation of A and U
			' throughout the code-base.
			' The runtime type of U is never checked for equality with the component type of the runtime type of A[].
			' Runtime checking will be performed when an element is stored in A[], thus if A is not a
			' super type of U an ArrayStoreException will be thrown.
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim rawGenerator As java.util.function.IntFunction = CType(generator, java.util.function.IntFunction)
			Return CType(Nodes.flatten(evaluateToArrayNode(rawGenerator), rawGenerator).asArray(rawGenerator), A())
		End Function

		Public Overrides Function toArray() As Object() Implements Stream(Of P_OUT).toArray
			Return ToArray(Object() ::New)
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overrides Function anyMatch(Of T1)(ByVal predicate As java.util.function.Predicate(Of T1)) As Boolean Implements Stream(Of P_OUT).anyMatch
			Return evaluate(MatchOps.makeRef(predicate, MatchOps.MatchKind.ANY))
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overrides Function allMatch(Of T1)(ByVal predicate As java.util.function.Predicate(Of T1)) As Boolean Implements Stream(Of P_OUT).allMatch
			Return evaluate(MatchOps.makeRef(predicate, MatchOps.MatchKind.ALL))
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overrides Function noneMatch(Of T1)(ByVal predicate As java.util.function.Predicate(Of T1)) As Boolean Implements Stream(Of P_OUT).noneMatch
			Return evaluate(MatchOps.makeRef(predicate, MatchOps.MatchKind.NONE))
		End Function

		Public Overrides Function findFirst() As java.util.Optional(Of P_OUT) Implements Stream(Of P_OUT).findFirst
			Return evaluate(FindOps.makeRef(True))
		End Function

		Public Overrides Function findAny() As java.util.Optional(Of P_OUT) Implements Stream(Of P_OUT).findAny
			Return evaluate(FindOps.makeRef(False))
		End Function

		Public Overrides Function reduce(ByVal identity As P_OUT, ByVal accumulator As java.util.function.BinaryOperator(Of P_OUT)) As P_OUT
			Return evaluate(ReduceOps.makeRef(identity, accumulator, accumulator))
		End Function

		Public Overrides Function reduce(ByVal accumulator As java.util.function.BinaryOperator(Of P_OUT)) As java.util.Optional(Of P_OUT) Implements Stream(Of P_OUT).reduce
			Return evaluate(ReduceOps.makeRef(accumulator))
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overrides Function reduce(Of R, T1)(ByVal identity As R, ByVal accumulator As java.util.function.BiFunction(Of T1), ByVal combiner As java.util.function.BinaryOperator(Of R)) As R
			Return evaluate(ReduceOps.makeRef(identity, accumulator, combiner))
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overrides Function collect(Of R, A, T1)(ByVal collector As Collector(Of T1)) As R Implements Stream(Of P_OUT).collect
			Dim container As A
			If parallel AndAlso (collector.characteristics().contains(Collector.Characteristics.CONCURRENT)) AndAlso ((Not ordered) OrElse collector.characteristics().contains(Collector.Characteristics.UNORDERED)) Then
				container = collector.supplier().get()
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim accumulator As java.util.function.BiConsumer(Of A, ?) = collector.accumulator()
				forEach(u -> accumulator.accept(container, u))
			Else
				container = evaluate(ReduceOps.makeRef(collector))
			End If
			Return If(collector.characteristics().contains(Collector.Characteristics.IDENTITY_FINISH), CType(container, R), collector.finisher().apply(container))
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overrides Function collect(Of R, T1)(ByVal supplier As java.util.function.Supplier(Of R), ByVal accumulator As java.util.function.BiConsumer(Of T1), ByVal combiner As java.util.function.BiConsumer(Of R, R)) As R Implements Stream(Of P_OUT).collect
			Return evaluate(ReduceOps.makeRef(supplier, accumulator, combiner))
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overrides Function max(Of T1)(ByVal comparator As IComparer(Of T1)) As java.util.Optional(Of P_OUT) Implements Stream(Of P_OUT).max
			Return reduce(java.util.function.BinaryOperator.maxBy(comparator))
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overrides Function min(Of T1)(ByVal comparator As IComparer(Of T1)) As java.util.Optional(Of P_OUT) Implements Stream(Of P_OUT).min
			Return reduce(java.util.function.BinaryOperator.minBy(comparator))

		End Function

		Public Overrides Function count() As Long Implements Stream(Of P_OUT).count
			Return mapToLong(e -> 1L).sum()
		End Function


		'

		''' <summary>
		''' Source stage of a ReferencePipeline.
		''' </summary>
		''' @param <E_IN> type of elements in the upstream source </param>
		''' @param <E_OUT> type of elements in produced by this stage
		''' @since 1.8 </param>
		Friend Class Head(Of E_IN, E_OUT)
			Inherits ReferencePipeline(Of E_IN, E_OUT)

			''' <summary>
			''' Constructor for the source stage of a Stream.
			''' </summary>
			''' <param name="source"> {@code Supplier<Spliterator>} describing the stream
			'''               source </param>
			''' <param name="sourceFlags"> the source flags for the stream source, described
			'''                    in <seealso cref="StreamOpFlag"/> </param>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend Sub New(Of T1 As java.util.Spliterator(Of ?)(ByVal source As java.util.function.Supplier(Of T1), ByVal sourceFlags As Integer, ByVal parallel As Boolean)
				MyBase.New(source, sourceFlags, parallel)
			End Sub

			''' <summary>
			''' Constructor for the source stage of a Stream.
			''' </summary>
			''' <param name="source"> {@code Spliterator} describing the stream source </param>
			''' <param name="sourceFlags"> the source flags for the stream source, described
			'''                    in <seealso cref="StreamOpFlag"/> </param>
			Friend Sub New(Of T1)(ByVal source As java.util.Spliterator(Of T1), ByVal sourceFlags As Integer, ByVal parallel As Boolean)
				MyBase.New(source, sourceFlags, parallel)
			End Sub

			Friend Overrides Function opIsStateful() As Boolean
				Throw New UnsupportedOperationException
			End Function

			Friend Overrides Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of E_OUT)) As Sink(Of E_IN)
				Throw New UnsupportedOperationException
			End Function

			' Optimized sequential terminal operations for the head of the pipeline

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub forEach(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
				If Not parallel Then
					sourceStageSpliterator().forEachRemaining(action)
				Else
					MyBase.forEach(action)
				End If
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub forEachOrdered(Of T1)(ByVal action As java.util.function.Consumer(Of T1))
				If Not parallel Then
					sourceStageSpliterator().forEachRemaining(action)
				Else
					MyBase.forEachOrdered(action)
				End If
			End Sub
		End Class

		''' <summary>
		''' Base class for a stateless intermediate stage of a Stream.
		''' </summary>
		''' @param <E_IN> type of elements in the upstream source </param>
		''' @param <E_OUT> type of elements in produced by this stage
		''' @since 1.8 </param>
		Friend MustInherit Class StatelessOp(Of E_IN, E_OUT)
			Inherits ReferencePipeline(Of E_IN, E_OUT)

			''' <summary>
			''' Construct a new Stream by appending a stateless intermediate
			''' operation to an existing stream.
			''' </summary>
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
		''' Base class for a stateful intermediate stage of a Stream.
		''' </summary>
		''' @param <E_IN> type of elements in the upstream source </param>
		''' @param <E_OUT> type of elements in produced by this stage
		''' @since 1.8 </param>
		Friend MustInherit Class StatefulOp(Of E_IN, E_OUT)
			Inherits ReferencePipeline(Of E_IN, E_OUT)

			''' <summary>
			''' Construct a new Stream by appending a stateful intermediate operation
			''' to an existing stream. </summary>
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

			Friend MustOverride Overrides Function opEvaluateParallel(Of P_IN)(ByVal helper As PipelineHelper(Of E_OUT), ByVal spliterator As java.util.Spliterator(Of P_IN), ByVal generator As java.util.function.IntFunction(Of E_OUT())) As Node(Of E_OUT)
		End Class
	End Class

End Namespace