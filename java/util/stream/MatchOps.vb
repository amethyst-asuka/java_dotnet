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
	''' Factory for instances of a short-circuiting {@code TerminalOp} that implement
	''' quantified predicate matching on the elements of a stream. Supported variants
	''' include match-all, match-any, and match-none.
	''' 
	''' @since 1.8
	''' </summary>
	Friend NotInheritable Class MatchOps

		Private Sub New()
		End Sub

		''' <summary>
		''' Enum describing quantified match options -- all match, any match, none
		''' match.
		''' </summary>
		Friend Enum MatchKind
			''' <summary>
			''' Do all elements match the predicate? </summary>
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
			ANY(True, True),

			''' <summary>
			''' Do any elements match the predicate? </summary>
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
			ALL(False, False),

			''' <summary>
			''' Do no elements match the predicate? </summary>
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
			NONE(True, False);

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'			private final boolean stopOnPredicateMatches;
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'			private final boolean shortCircuitResult;

			[private] MatchKind(boolean stopOnPredicateMatches,
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
							  boolean shortCircuitResult)
				[Me].stopOnPredicateMatches = stopOnPredicateMatches
				[Me].shortCircuitResult = shortCircuitResult
		End Enum

		''' <summary>
		''' Constructs a quantified predicate matcher for a Stream.
		''' </summary>
		''' @param <T> the type of stream elements </param>
		''' <param name="predicate"> the {@code Predicate} to apply to stream elements </param>
		''' <param name="matchKind"> the kind of quantified match (all, any, none) </param>
		''' <returns> a {@code TerminalOp} implementing the desired quantified match
		'''         criteria </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Shared Function makeRef(Of T, T1)(  predicate As java.util.function.Predicate(Of T1),   matchKind As MatchKind) As TerminalOp(Of T, Boolean?)
			java.util.Objects.requireNonNull(predicate)
			java.util.Objects.requireNonNull(matchKind)
'JAVA TO VB CONVERTER TODO TASK: Local classes are not converted by Java to VB Converter:
'			class MatchSink extends BooleanTerminalSink(Of T)
	'		{
	'			MatchSink()
	'			{
	'				MyBase(matchKind);
	'			}
	'
	'			@Override public  Sub  accept(T t)
	'			{
	'				if (!stop && predicate.test(t) == matchKind.stopOnPredicateMatches)
	'				{
	'					stop = True;
	'					value = matchKind.shortCircuitResult;
	'				}
	'			}
	'		}

			Return New MatchOp(Of )(StreamShape.REFERENCE, matchKind, MatchSink::New)
		End Function

		''' <summary>
		''' Constructs a quantified predicate matcher for an {@code IntStream}.
		''' </summary>
		''' <param name="predicate"> the {@code Predicate} to apply to stream elements </param>
		''' <param name="matchKind"> the kind of quantified match (all, any, none) </param>
		''' <returns> a {@code TerminalOp} implementing the desired quantified match
		'''         criteria </returns>
		Public Shared Function makeInt(  predicate As java.util.function.IntPredicate,   matchKind As MatchKind) As TerminalOp(Of Integer?, Boolean?)
			java.util.Objects.requireNonNull(predicate)
			java.util.Objects.requireNonNull(matchKind)
'JAVA TO VB CONVERTER TODO TASK: Local classes are not converted by Java to VB Converter:
'			class MatchSink extends BooleanTerminalSink(Of java.lang.Integer) implements Sink.OfInt
	'		{
	'			MatchSink()
	'			{
	'				MyBase(matchKind);
	'			}
	'
	'			@Override public  Sub  accept(int t)
	'			{
	'				if (!stop && predicate.test(t) == matchKind.stopOnPredicateMatches)
	'				{
	'					stop = True;
	'					value = matchKind.shortCircuitResult;
	'				}
	'			}
	'		}

			Return New MatchOp(Of )(StreamShape.INT_VALUE, matchKind, MatchSink::New)
		End Function

		''' <summary>
		''' Constructs a quantified predicate matcher for a {@code LongStream}.
		''' </summary>
		''' <param name="predicate"> the {@code Predicate} to apply to stream elements </param>
		''' <param name="matchKind"> the kind of quantified match (all, any, none) </param>
		''' <returns> a {@code TerminalOp} implementing the desired quantified match
		'''         criteria </returns>
		Public Shared Function makeLong(  predicate As java.util.function.LongPredicate,   matchKind As MatchKind) As TerminalOp(Of Long?, Boolean?)
			java.util.Objects.requireNonNull(predicate)
			java.util.Objects.requireNonNull(matchKind)
'JAVA TO VB CONVERTER TODO TASK: Local classes are not converted by Java to VB Converter:
'			class MatchSink extends BooleanTerminalSink(Of java.lang.Long) implements Sink.OfLong
	'		{
	'
	'			MatchSink()
	'			{
	'				MyBase(matchKind);
	'			}
	'
	'			@Override public  Sub  accept(long t)
	'			{
	'				if (!stop && predicate.test(t) == matchKind.stopOnPredicateMatches)
	'				{
	'					stop = True;
	'					value = matchKind.shortCircuitResult;
	'				}
	'			}
	'		}

			Return New MatchOp(Of )(StreamShape.LONG_VALUE, matchKind, MatchSink::New)
		End Function

		''' <summary>
		''' Constructs a quantified predicate matcher for a {@code DoubleStream}.
		''' </summary>
		''' <param name="predicate"> the {@code Predicate} to apply to stream elements </param>
		''' <param name="matchKind"> the kind of quantified match (all, any, none) </param>
		''' <returns> a {@code TerminalOp} implementing the desired quantified match
		'''         criteria </returns>
		Public Shared Function makeDouble(  predicate As java.util.function.DoublePredicate,   matchKind As MatchKind) As TerminalOp(Of Double?, Boolean?)
			java.util.Objects.requireNonNull(predicate)
			java.util.Objects.requireNonNull(matchKind)
'JAVA TO VB CONVERTER TODO TASK: Local classes are not converted by Java to VB Converter:
'			class MatchSink extends BooleanTerminalSink(Of java.lang.Double) implements Sink.OfDouble
	'		{
	'
	'			MatchSink()
	'			{
	'				MyBase(matchKind);
	'			}
	'
	'			@Override public  Sub  accept(double t)
	'			{
	'				if (!stop && predicate.test(t) == matchKind.stopOnPredicateMatches)
	'				{
	'					stop = True;
	'					value = matchKind.shortCircuitResult;
	'				}
	'			}
	'		}

			Return New MatchOp(Of )(StreamShape.DOUBLE_VALUE, matchKind, MatchSink::New)
		End Function

		''' <summary>
		''' A short-circuiting {@code TerminalOp} that evaluates a predicate on the
		''' elements of a stream and determines whether all, any or none of those
		''' elements match the predicate.
		''' </summary>
		''' @param <T> the output type of the stream pipeline </param>
		Private NotInheritable Class MatchOp(Of T)
			Implements TerminalOp(Of T, Boolean?)

			Private ReadOnly inputShape_Renamed As StreamShape
			Friend ReadOnly matchKind As MatchKind
			Friend ReadOnly sinkSupplier As java.util.function.Supplier(Of BooleanTerminalSink(Of T))

			''' <summary>
			''' Constructs a {@code MatchOp}.
			''' </summary>
			''' <param name="shape"> the output shape of the stream pipeline </param>
			''' <param name="matchKind"> the kind of quantified match (all, any, none) </param>
			''' <param name="sinkSupplier"> {@code Supplier} for a {@code Sink} of the
			'''        appropriate shape which implements the matching operation </param>
			Friend Sub New(  shape As StreamShape,   matchKind As MatchKind,   sinkSupplier As java.util.function.Supplier(Of BooleanTerminalSink(Of T)))
				Me.inputShape_Renamed = shape
				Me.matchKind = matchKind
				Me.sinkSupplier = sinkSupplier
			End Sub

			Public  Overrides ReadOnly Property  opFlags As Integer Implements TerminalOp(Of T, Boolean?).getOpFlags
				Get
					Return StreamOpFlag.IS_SHORT_CIRCUIT Or StreamOpFlag.NOT_ORDERED
				End Get
			End Property

			Public Overrides Function inputShape() As StreamShape Implements TerminalOp(Of T, Boolean?).inputShape
				Return inputShape_Renamed
			End Function

			Public Overrides Function evaluateSequential(Of S)(  helper As PipelineHelper(Of T),   spliterator As java.util.Spliterator(Of S)) As Boolean?
				Return helper.wrapAndCopyInto(sinkSupplier.get(), spliterator).andClearState
			End Function

			Public Overrides Function evaluateParallel(Of S)(  helper As PipelineHelper(Of T),   spliterator As java.util.Spliterator(Of S)) As Boolean?
				' Approach for parallel implementation:
				' - Decompose as per usual
				' - run match on leaf chunks, call result "b"
				' - if b == matchKind.shortCircuitOn, complete early and return b
				' - else if we complete normally, return !shortCircuitOn

				Return (New MatchTask(Of )(Me, helper, spliterator)).invoke()
			End Function
		End Class

		''' <summary>
		''' Boolean specific terminal sink to avoid the boxing costs when returning
		''' results.  Subclasses implement the shape-specific functionality.
		''' </summary>
		''' @param <T> The output type of the stream pipeline </param>
		Private MustInherit Class BooleanTerminalSink(Of T)
			Implements Sink(Of T)

			Friend [stop] As Boolean
			Friend value As Boolean

			Friend Sub New(  matchKind As MatchKind)
				value = Not matchKind.shortCircuitResult
			End Sub

			Public Overridable Property andClearState As Boolean
				Get
					Return value
				End Get
			End Property

			Public Overrides Function cancellationRequested() As Boolean Implements Sink(Of T).cancellationRequested
				Return [stop]
			End Function
		End Class

		''' <summary>
		''' ForkJoinTask implementation to implement a parallel short-circuiting
		''' quantified match
		''' </summary>
		''' @param <P_IN> the type of source elements for the pipeline </param>
		''' @param <P_OUT> the type of output elements for the pipeline </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private NotInheritable Class MatchTask(Of P_IN, P_OUT)
			Inherits AbstractShortCircuitTask(Of P_IN, P_OUT, Boolean?, MatchTask(Of P_IN, P_OUT))

			Private ReadOnly op As MatchOp(Of P_OUT)

			''' <summary>
			''' Constructor for root node
			''' </summary>
			Friend Sub New(  op As MatchOp(Of P_OUT),   helper As PipelineHelper(Of P_OUT),   spliterator As java.util.Spliterator(Of P_IN))
				MyBase.New(helper, spliterator)
				Me.op = op
			End Sub

			''' <summary>
			''' Constructor for non-root node
			''' </summary>
			Friend Sub New(  parent As MatchTask(Of P_IN, P_OUT),   spliterator As java.util.Spliterator(Of P_IN))
				MyBase.New(parent, spliterator)
				Me.op = parent.op
			End Sub

			Protected Friend Overrides Function makeChild(  spliterator As java.util.Spliterator(Of P_IN)) As MatchTask(Of P_IN, P_OUT)
				Return New MatchTask(Of )(Me, spliterator)
			End Function

			Protected Friend Overrides Function doLeaf() As Boolean?
				Dim b As Boolean = helper.wrapAndCopyInto(op.sinkSupplier.get(), spliterator).andClearState
				If b = op.matchKind.shortCircuitResult Then shortCircuit(b)
				Return Nothing
			End Function

			Protected Friend  Overrides ReadOnly Property  emptyResult As Boolean?
				Get
					Return Not op.matchKind.shortCircuitResult
				End Get
			End Property
		End Class
	End Class


End Namespace