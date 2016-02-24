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
	''' Factory for creating instances of {@code TerminalOp} that implement
	''' reductions.
	''' 
	''' @since 1.8
	''' </summary>
	Friend NotInheritable Class ReduceOps

		Private Sub New()
		End Sub

		''' <summary>
		''' Constructs a {@code TerminalOp} that implements a functional reduce on
		''' reference values.
		''' </summary>
		''' @param <T> the type of the input elements </param>
		''' @param <U> the type of the result </param>
		''' <param name="seed"> the identity element for the reduction </param>
		''' <param name="reducer"> the accumulating function that incorporates an additional
		'''        input element into the result </param>
		''' <param name="combiner"> the combining function that combines two intermediate
		'''        results </param>
		''' <returns> a {@code TerminalOp} implementing the reduction </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Shared Function makeRef(Of T, U, T1)(ByVal seed As U, ByVal reducer As java.util.function.BiFunction(Of T1), ByVal combiner As java.util.function.BinaryOperator(Of U)) As TerminalOp(Of T, U)
			java.util.Objects.requireNonNull(reducer)
			java.util.Objects.requireNonNull(combiner)
'JAVA TO VB CONVERTER TODO TASK: Local classes are not converted by Java to VB Converter:
'			class ReducingSink extends Box(Of U) implements AccumulatingSink(Of T, U, ReducingSink)
	'		{
	'			@Override public void begin(long size)
	'			{
	'				state = seed;
	'			}
	'
	'			@Override public void accept(T t)
	'			{
	'				state = reducer.apply(state, t);
	'			}
	'
	'			@Override public void combine(ReducingSink other)
	'			{
	'				state = combiner.apply(state, other.state);
	'			}
	'		}
			Return New ReduceOpAnonymousInnerClassHelper(Of T, R, S)
		End Function

		Private Class ReduceOpAnonymousInnerClassHelper(Of T, R, S)
			Inherits ReduceOp(Of T, R, S)

			Public Overrides Function makeSink() As ReducingSink
				Return New ReducingSink
			End Function
		End Class

		''' <summary>
		''' Constructs a {@code TerminalOp} that implements a functional reduce on
		''' reference values producing an optional reference result.
		''' </summary>
		''' @param <T> The type of the input elements, and the type of the result </param>
		''' <param name="operator"> The reducing function </param>
		''' <returns> A {@code TerminalOp} implementing the reduction </returns>
		Public Shared Function makeRef(Of T)(ByVal [operator] As java.util.function.BinaryOperator(Of T)) As TerminalOp(Of T, java.util.Optional(Of T))
			java.util.Objects.requireNonNull([operator])
'JAVA TO VB CONVERTER TODO TASK: Local classes are not converted by Java to VB Converter:
'			class ReducingSink implements AccumulatingSink(Of T, java.util.Optional(Of T), ReducingSink)
	'		{
	'			private boolean empty;
	'			private T state;
	'
	'			public void begin(long size)
	'			{
	'				empty = True;
	'				state = Nothing;
	'			}
	'
	'			@Override public void accept(T t)
	'			{
	'				if (empty)
	'				{
	'					empty = False;
	'					state = t;
	'				}
	'				else
	'				{
	'					state = operator.apply(state, t);
	'				}
	'			}
	'
	'			@Override public Optional<T> get()
	'			{
	'				Return empty ? Optional.empty() : Optional.of(state);
	'			}
	'
	'			@Override public void combine(ReducingSink other)
	'			{
	'				if (!other.empty)
	'					accept(other.state);
	'			}
	'		}
			Return New ReduceOpAnonymousInnerClassHelper2(Of T, R, S)
		End Function

		Private Class ReduceOpAnonymousInnerClassHelper2(Of T, R, S)
			Inherits ReduceOp(Of T, R, S)

			Public Overrides Function makeSink() As ReducingSink
				Return New ReducingSink
			End Function
		End Class

		''' <summary>
		''' Constructs a {@code TerminalOp} that implements a mutable reduce on
		''' reference values.
		''' </summary>
		''' @param <T> the type of the input elements </param>
		''' @param <I> the type of the intermediate reduction result </param>
		''' <param name="collector"> a {@code Collector} defining the reduction </param>
		''' <returns> a {@code ReduceOp} implementing the reduction </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Shared Function makeRef(Of T, I, T1)(ByVal collector As Collector(Of T1)) As TerminalOp(Of T, I)
			Dim supplier As java.util.function.Supplier(Of I) = java.util.Objects.requireNonNull(collector).supplier()
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim accumulator As java.util.function.BiConsumer(Of I, ?) = collector.accumulator()
			Dim combiner As java.util.function.BinaryOperator(Of I) = collector.combiner()
'JAVA TO VB CONVERTER TODO TASK: Local classes are not converted by Java to VB Converter:
'			class ReducingSink extends Box(Of I) implements AccumulatingSink(Of T, I, ReducingSink)
	'		{
	'			@Override public void begin(long size)
	'			{
	'				state = supplier.get();
	'			}
	'
	'			@Override public void accept(T t)
	'			{
	'				accumulator.accept(state, t);
	'			}
	'
	'			@Override public void combine(ReducingSink other)
	'			{
	'				state = combiner.apply(state, other.state);
	'			}
	'		}
			Return New ReduceOpAnonymousInnerClassHelper3(Of T, R, S)
		End Function

		Private Class ReduceOpAnonymousInnerClassHelper3(Of T, R, S)
			Inherits ReduceOp(Of T, R, S)

			Public Overrides Function makeSink() As ReducingSink
				Return New ReducingSink
			End Function

			Public Property Overrides opFlags As Integer
				Get
					Return If(collector.characteristics().contains(Collector.Characteristics.UNORDERED), StreamOpFlag.NOT_ORDERED, 0)
				End Get
			End Property
		End Class

		''' <summary>
		''' Constructs a {@code TerminalOp} that implements a mutable reduce on
		''' reference values.
		''' </summary>
		''' @param <T> the type of the input elements </param>
		''' @param <R> the type of the result </param>
		''' <param name="seedFactory"> a factory to produce a new base accumulator </param>
		''' <param name="accumulator"> a function to incorporate an element into an
		'''        accumulator </param>
		''' <param name="reducer"> a function to combine an accumulator into another </param>
		''' <returns> a {@code TerminalOp} implementing the reduction </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Shared Function makeRef(Of T, R, T1)(ByVal seedFactory As java.util.function.Supplier(Of R), ByVal accumulator As java.util.function.BiConsumer(Of T1), ByVal reducer As java.util.function.BiConsumer(Of R, R)) As TerminalOp(Of T, R)
			java.util.Objects.requireNonNull(seedFactory)
			java.util.Objects.requireNonNull(accumulator)
			java.util.Objects.requireNonNull(reducer)
'JAVA TO VB CONVERTER TODO TASK: Local classes are not converted by Java to VB Converter:
'			class ReducingSink extends Box(Of R) implements AccumulatingSink(Of T, R, ReducingSink)
	'		{
	'			@Override public void begin(long size)
	'			{
	'				state = seedFactory.get();
	'			}
	'
	'			@Override public void accept(T t)
	'			{
	'				accumulator.accept(state, t);
	'			}
	'
	'			@Override public void combine(ReducingSink other)
	'			{
	'				reducer.accept(state, other.state);
	'			}
	'		}
			Return New ReduceOpAnonymousInnerClassHelper4(Of T, R, S)
		End Function

		Private Class ReduceOpAnonymousInnerClassHelper4(Of T, R, S)
			Inherits ReduceOp(Of T, R, S)

			Public Overrides Function makeSink() As ReducingSink
				Return New ReducingSink
			End Function
		End Class

		''' <summary>
		''' Constructs a {@code TerminalOp} that implements a functional reduce on
		''' {@code int} values.
		''' </summary>
		''' <param name="identity"> the identity for the combining function </param>
		''' <param name="operator"> the combining function </param>
		''' <returns> a {@code TerminalOp} implementing the reduction </returns>
		Public Shared Function makeInt(ByVal identity As Integer, ByVal [operator] As java.util.function.IntBinaryOperator) As TerminalOp(Of Integer?, Integer?)
			java.util.Objects.requireNonNull([operator])
'JAVA TO VB CONVERTER TODO TASK: Local classes are not converted by Java to VB Converter:
'			class ReducingSink implements AccumulatingSink(Of java.lang.Integer, java.lang.Integer, ReducingSink), Sink.OfInt
	'		{
	'			private int state;
	'
	'			@Override public void begin(long size)
	'			{
	'				state = identity;
	'			}
	'
	'			@Override public void accept(int t)
	'			{
	'				state = operator.applyAsInt(state, t);
	'			}
	'
	'			@Override public java.lang.Integer get()
	'			{
	'				Return state;
	'			}
	'
	'			@Override public void combine(ReducingSink other)
	'			{
	'				accept(other.state);
	'			}
	'		}
			Return New ReduceOpAnonymousInnerClassHelper5(Of T, R, S)
		End Function

		Private Class ReduceOpAnonymousInnerClassHelper5(Of T, R, S)
			Inherits ReduceOp(Of T, R, S)

			Public Overrides Function makeSink() As ReducingSink
				Return New ReducingSink
			End Function
		End Class

		''' <summary>
		''' Constructs a {@code TerminalOp} that implements a functional reduce on
		''' {@code int} values, producing an optional integer result.
		''' </summary>
		''' <param name="operator"> the combining function </param>
		''' <returns> a {@code TerminalOp} implementing the reduction </returns>
		Public Shared Function makeInt(ByVal [operator] As java.util.function.IntBinaryOperator) As TerminalOp(Of Integer?, java.util.OptionalInt)
			java.util.Objects.requireNonNull([operator])
'JAVA TO VB CONVERTER TODO TASK: Local classes are not converted by Java to VB Converter:
'			class ReducingSink implements AccumulatingSink(Of java.lang.Integer, java.util.OptionalInt, ReducingSink), Sink.OfInt
	'		{
	'			private boolean empty;
	'			private int state;
	'
	'			public void begin(long size)
	'			{
	'				empty = True;
	'				state = 0;
	'			}
	'
	'			@Override public void accept(int t)
	'			{
	'				if (empty)
	'				{
	'					empty = False;
	'					state = t;
	'				}
	'				else
	'				{
	'					state = operator.applyAsInt(state, t);
	'				}
	'			}
	'
	'			@Override public OptionalInt get()
	'			{
	'				Return empty ? OptionalInt.empty() : OptionalInt.of(state);
	'			}
	'
	'			@Override public void combine(ReducingSink other)
	'			{
	'				if (!other.empty)
	'					accept(other.state);
	'			}
	'		}
			Return New ReduceOpAnonymousInnerClassHelper6(Of T, R, S)
		End Function

		Private Class ReduceOpAnonymousInnerClassHelper6(Of T, R, S)
			Inherits ReduceOp(Of T, R, S)

			Public Overrides Function makeSink() As ReducingSink
				Return New ReducingSink
			End Function
		End Class

		''' <summary>
		''' Constructs a {@code TerminalOp} that implements a mutable reduce on
		''' {@code int} values.
		''' </summary>
		''' @param <R> The type of the result </param>
		''' <param name="supplier"> a factory to produce a new accumulator of the result type </param>
		''' <param name="accumulator"> a function to incorporate an int into an
		'''        accumulator </param>
		''' <param name="combiner"> a function to combine an accumulator into another </param>
		''' <returns> A {@code ReduceOp} implementing the reduction </returns>
		Public Shared Function makeInt(Of R)(ByVal supplier As java.util.function.Supplier(Of R), ByVal accumulator As java.util.function.ObjIntConsumer(Of R), ByVal combiner As java.util.function.BinaryOperator(Of R)) As TerminalOp(Of Integer?, R)
			java.util.Objects.requireNonNull(supplier)
			java.util.Objects.requireNonNull(accumulator)
			java.util.Objects.requireNonNull(combiner)
'JAVA TO VB CONVERTER TODO TASK: Local classes are not converted by Java to VB Converter:
'			class ReducingSink extends Box(Of R) implements AccumulatingSink(Of java.lang.Integer, R, ReducingSink), Sink.OfInt
	'		{
	'			@Override public void begin(long size)
	'			{
	'				state = supplier.get();
	'			}
	'
	'			@Override public void accept(int t)
	'			{
	'				accumulator.accept(state, t);
	'			}
	'
	'			@Override public void combine(ReducingSink other)
	'			{
	'				state = combiner.apply(state, other.state);
	'			}
	'		}
			Return New ReduceOpAnonymousInnerClassHelper7(Of T, R, S)
		End Function

		Private Class ReduceOpAnonymousInnerClassHelper7(Of T, R, S)
			Inherits ReduceOp(Of T, R, S)

			Public Overrides Function makeSink() As ReducingSink
				Return New ReducingSink
			End Function
		End Class

		''' <summary>
		''' Constructs a {@code TerminalOp} that implements a functional reduce on
		''' {@code long} values.
		''' </summary>
		''' <param name="identity"> the identity for the combining function </param>
		''' <param name="operator"> the combining function </param>
		''' <returns> a {@code TerminalOp} implementing the reduction </returns>
		Public Shared Function makeLong(ByVal identity As Long, ByVal [operator] As java.util.function.LongBinaryOperator) As TerminalOp(Of Long?, Long?)
			java.util.Objects.requireNonNull([operator])
'JAVA TO VB CONVERTER TODO TASK: Local classes are not converted by Java to VB Converter:
'			class ReducingSink implements AccumulatingSink(Of java.lang.Long, java.lang.Long, ReducingSink), Sink.OfLong
	'		{
	'			private long state;
	'
	'			@Override public void begin(long size)
	'			{
	'				state = identity;
	'			}
	'
	'			@Override public void accept(long t)
	'			{
	'				state = operator.applyAsLong(state, t);
	'			}
	'
	'			@Override public java.lang.Long get()
	'			{
	'				Return state;
	'			}
	'
	'			@Override public void combine(ReducingSink other)
	'			{
	'				accept(other.state);
	'			}
	'		}
			Return New ReduceOpAnonymousInnerClassHelper8(Of T, R, S)
		End Function

		Private Class ReduceOpAnonymousInnerClassHelper8(Of T, R, S)
			Inherits ReduceOp(Of T, R, S)

			Public Overrides Function makeSink() As ReducingSink
				Return New ReducingSink
			End Function
		End Class

		''' <summary>
		''' Constructs a {@code TerminalOp} that implements a functional reduce on
		''' {@code long} values, producing an optional long result.
		''' </summary>
		''' <param name="operator"> the combining function </param>
		''' <returns> a {@code TerminalOp} implementing the reduction </returns>
		Public Shared Function makeLong(ByVal [operator] As java.util.function.LongBinaryOperator) As TerminalOp(Of Long?, java.util.OptionalLong)
			java.util.Objects.requireNonNull([operator])
'JAVA TO VB CONVERTER TODO TASK: Local classes are not converted by Java to VB Converter:
'			class ReducingSink implements AccumulatingSink(Of java.lang.Long, java.util.OptionalLong, ReducingSink), Sink.OfLong
	'		{
	'			private boolean empty;
	'			private long state;
	'
	'			public void begin(long size)
	'			{
	'				empty = True;
	'				state = 0;
	'			}
	'
	'			@Override public void accept(long t)
	'			{
	'				if (empty)
	'				{
	'					empty = False;
	'					state = t;
	'				}
	'				else
	'				{
	'					state = operator.applyAsLong(state, t);
	'				}
	'			}
	'
	'			@Override public OptionalLong get()
	'			{
	'				Return empty ? OptionalLong.empty() : OptionalLong.of(state);
	'			}
	'
	'			@Override public void combine(ReducingSink other)
	'			{
	'				if (!other.empty)
	'					accept(other.state);
	'			}
	'		}
			Return New ReduceOpAnonymousInnerClassHelper9(Of T, R, S)
		End Function

		Private Class ReduceOpAnonymousInnerClassHelper9(Of T, R, S)
			Inherits ReduceOp(Of T, R, S)

			Public Overrides Function makeSink() As ReducingSink
				Return New ReducingSink
			End Function
		End Class

		''' <summary>
		''' Constructs a {@code TerminalOp} that implements a mutable reduce on
		''' {@code long} values.
		''' </summary>
		''' @param <R> the type of the result </param>
		''' <param name="supplier"> a factory to produce a new accumulator of the result type </param>
		''' <param name="accumulator"> a function to incorporate an int into an
		'''        accumulator </param>
		''' <param name="combiner"> a function to combine an accumulator into another </param>
		''' <returns> a {@code TerminalOp} implementing the reduction </returns>
		Public Shared Function makeLong(Of R)(ByVal supplier As java.util.function.Supplier(Of R), ByVal accumulator As java.util.function.ObjLongConsumer(Of R), ByVal combiner As java.util.function.BinaryOperator(Of R)) As TerminalOp(Of Long?, R)
			java.util.Objects.requireNonNull(supplier)
			java.util.Objects.requireNonNull(accumulator)
			java.util.Objects.requireNonNull(combiner)
'JAVA TO VB CONVERTER TODO TASK: Local classes are not converted by Java to VB Converter:
'			class ReducingSink extends Box(Of R) implements AccumulatingSink(Of java.lang.Long, R, ReducingSink), Sink.OfLong
	'		{
	'			@Override public void begin(long size)
	'			{
	'				state = supplier.get();
	'			}
	'
	'			@Override public void accept(long t)
	'			{
	'				accumulator.accept(state, t);
	'			}
	'
	'			@Override public void combine(ReducingSink other)
	'			{
	'				state = combiner.apply(state, other.state);
	'			}
	'		}
			Return New ReduceOpAnonymousInnerClassHelper10(Of T, R, S)
		End Function

		Private Class ReduceOpAnonymousInnerClassHelper10(Of T, R, S)
			Inherits ReduceOp(Of T, R, S)

			Public Overrides Function makeSink() As ReducingSink
				Return New ReducingSink
			End Function
		End Class

		''' <summary>
		''' Constructs a {@code TerminalOp} that implements a functional reduce on
		''' {@code double} values.
		''' </summary>
		''' <param name="identity"> the identity for the combining function </param>
		''' <param name="operator"> the combining function </param>
		''' <returns> a {@code TerminalOp} implementing the reduction </returns>
		Public Shared Function makeDouble(ByVal identity As Double, ByVal [operator] As java.util.function.DoubleBinaryOperator) As TerminalOp(Of Double?, Double?)
			java.util.Objects.requireNonNull([operator])
'JAVA TO VB CONVERTER TODO TASK: Local classes are not converted by Java to VB Converter:
'			class ReducingSink implements AccumulatingSink(Of java.lang.Double, java.lang.Double, ReducingSink), Sink.OfDouble
	'		{
	'			private double state;
	'
	'			@Override public void begin(long size)
	'			{
	'				state = identity;
	'			}
	'
	'			@Override public void accept(double t)
	'			{
	'				state = operator.applyAsDouble(state, t);
	'			}
	'
	'			@Override public java.lang.Double get()
	'			{
	'				Return state;
	'			}
	'
	'			@Override public void combine(ReducingSink other)
	'			{
	'				accept(other.state);
	'			}
	'		}
			Return New ReduceOpAnonymousInnerClassHelper11(Of T, R, S)
		End Function

		Private Class ReduceOpAnonymousInnerClassHelper11(Of T, R, S)
			Inherits ReduceOp(Of T, R, S)

			Public Overrides Function makeSink() As ReducingSink
				Return New ReducingSink
			End Function
		End Class

		''' <summary>
		''' Constructs a {@code TerminalOp} that implements a functional reduce on
		''' {@code double} values, producing an optional double result.
		''' </summary>
		''' <param name="operator"> the combining function </param>
		''' <returns> a {@code TerminalOp} implementing the reduction </returns>
		Public Shared Function makeDouble(ByVal [operator] As java.util.function.DoubleBinaryOperator) As TerminalOp(Of Double?, java.util.OptionalDouble)
			java.util.Objects.requireNonNull([operator])
'JAVA TO VB CONVERTER TODO TASK: Local classes are not converted by Java to VB Converter:
'			class ReducingSink implements AccumulatingSink(Of java.lang.Double, java.util.OptionalDouble, ReducingSink), Sink.OfDouble
	'		{
	'			private boolean empty;
	'			private double state;
	'
	'			public void begin(long size)
	'			{
	'				empty = True;
	'				state = 0;
	'			}
	'
	'			@Override public void accept(double t)
	'			{
	'				if (empty)
	'				{
	'					empty = False;
	'					state = t;
	'				}
	'				else
	'				{
	'					state = operator.applyAsDouble(state, t);
	'				}
	'			}
	'
	'			@Override public OptionalDouble get()
	'			{
	'				Return empty ? OptionalDouble.empty() : OptionalDouble.of(state);
	'			}
	'
	'			@Override public void combine(ReducingSink other)
	'			{
	'				if (!other.empty)
	'					accept(other.state);
	'			}
	'		}
			Return New ReduceOpAnonymousInnerClassHelper12(Of T, R, S)
		End Function

		Private Class ReduceOpAnonymousInnerClassHelper12(Of T, R, S)
			Inherits ReduceOp(Of T, R, S)

			Public Overrides Function makeSink() As ReducingSink
				Return New ReducingSink
			End Function
		End Class

		''' <summary>
		''' Constructs a {@code TerminalOp} that implements a mutable reduce on
		''' {@code double} values.
		''' </summary>
		''' @param <R> the type of the result </param>
		''' <param name="supplier"> a factory to produce a new accumulator of the result type </param>
		''' <param name="accumulator"> a function to incorporate an int into an
		'''        accumulator </param>
		''' <param name="combiner"> a function to combine an accumulator into another </param>
		''' <returns> a {@code TerminalOp} implementing the reduction </returns>
		Public Shared Function makeDouble(Of R)(ByVal supplier As java.util.function.Supplier(Of R), ByVal accumulator As java.util.function.ObjDoubleConsumer(Of R), ByVal combiner As java.util.function.BinaryOperator(Of R)) As TerminalOp(Of Double?, R)
			java.util.Objects.requireNonNull(supplier)
			java.util.Objects.requireNonNull(accumulator)
			java.util.Objects.requireNonNull(combiner)
'JAVA TO VB CONVERTER TODO TASK: Local classes are not converted by Java to VB Converter:
'			class ReducingSink extends Box(Of R) implements AccumulatingSink(Of java.lang.Double, R, ReducingSink), Sink.OfDouble
	'		{
	'			@Override public void begin(long size)
	'			{
	'				state = supplier.get();
	'			}
	'
	'			@Override public void accept(double t)
	'			{
	'				accumulator.accept(state, t);
	'			}
	'
	'			@Override public void combine(ReducingSink other)
	'			{
	'				state = combiner.apply(state, other.state);
	'			}
	'		}
			Return New ReduceOpAnonymousInnerClassHelper13(Of T, R, S)
		End Function

		Private Class ReduceOpAnonymousInnerClassHelper13(Of T, R, S)
			Inherits ReduceOp(Of T, R, S)

			Public Overrides Function makeSink() As ReducingSink
				Return New ReducingSink
			End Function
		End Class

		''' <summary>
		''' A type of {@code TerminalSink} that implements an associative reducing
		''' operation on elements of type {@code T} and producing a result of type
		''' {@code R}.
		''' </summary>
		''' @param <T> the type of input element to the combining operation </param>
		''' @param <R> the result type </param>
		''' @param <K> the type of the {@code AccumulatingSink}. </param>
		Private Interface AccumulatingSink(Of T, R, K As AccumulatingSink(Of T, R, K))
			Inherits TerminalSink(Of T, R)

			Sub combine(ByVal other As K)
		End Interface

		''' <summary>
		''' State box for a single state element, used as a base class for
		''' {@code AccumulatingSink} instances
		''' </summary>
		''' @param <U> The type of the state element </param>
		Private MustInherit Class Box(Of U)
			Friend state As U

			Friend Sub New() ' Avoid creation of special accessor
			End Sub

			Public Overridable Function [get]() As U
				Return state
			End Function
		End Class

		''' <summary>
		''' A {@code TerminalOp} that evaluates a stream pipeline and sends the
		''' output into an {@code AccumulatingSink}, which performs a reduce
		''' operation. The {@code AccumulatingSink} must represent an associative
		''' reducing operation.
		''' </summary>
		''' @param <T> the output type of the stream pipeline </param>
		''' @param <R> the result type of the reducing operation </param>
		''' @param <S> the type of the {@code AccumulatingSink} </param>
		Private MustInherit Class ReduceOp(Of T, R, S As AccumulatingSink(Of T, R, S))
			Implements TerminalOp(Of T, R)

			Private ReadOnly inputShape_Renamed As StreamShape

			''' <summary>
			''' Create a {@code ReduceOp} of the specified stream shape which uses
			''' the specified {@code Supplier} to create accumulating sinks.
			''' </summary>
			''' <param name="shape"> The shape of the stream pipeline </param>
			Friend Sub New(ByVal shape As StreamShape)
				inputShape_Renamed = shape
			End Sub

			Public MustOverride Function makeSink() As S

			Public Overrides Function inputShape() As StreamShape Implements TerminalOp(Of T, R).inputShape
				Return inputShape_Renamed
			End Function

			Public Overrides Function evaluateSequential(Of P_IN)(ByVal helper As PipelineHelper(Of T), ByVal spliterator As java.util.Spliterator(Of P_IN)) As R Implements TerminalOp(Of T, R).evaluateSequential
				Return helper.wrapAndCopyInto(makeSink(), spliterator).get()
			End Function

			Public Overrides Function evaluateParallel(Of P_IN)(ByVal helper As PipelineHelper(Of T), ByVal spliterator As java.util.Spliterator(Of P_IN)) As R Implements TerminalOp(Of T, R).evaluateParallel
				Return (New ReduceTask(Of )(Me, helper, spliterator)).invoke().get()
			End Function
		End Class

		''' <summary>
		''' A {@code ForkJoinTask} for performing a parallel reduce operation.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private NotInheritable Class ReduceTask(Of P_IN, P_OUT, R, S As AccumulatingSink(Of P_OUT, R, S))
			Inherits AbstractTask(Of P_IN, P_OUT, S, ReduceTask(Of P_IN, P_OUT, R, S))

			Private ReadOnly op As ReduceOp(Of P_OUT, R, S)

			Friend Sub New(ByVal op As ReduceOp(Of P_OUT, R, S), ByVal helper As PipelineHelper(Of P_OUT), ByVal spliterator As java.util.Spliterator(Of P_IN))
				MyBase.New(helper, spliterator)
				Me.op = op
			End Sub

			Friend Sub New(ByVal parent As ReduceTask(Of P_IN, P_OUT, R, S), ByVal spliterator As java.util.Spliterator(Of P_IN))
				MyBase.New(parent, spliterator)
				Me.op = parent.op
			End Sub

			Protected Friend Overrides Function makeChild(ByVal spliterator As java.util.Spliterator(Of P_IN)) As ReduceTask(Of P_IN, P_OUT, R, S)
				Return New ReduceTask(Of )(Me, spliterator)
			End Function

			Protected Friend Overrides Function doLeaf() As S
				Return helper.wrapAndCopyInto(op.makeSink(), spliterator)
			End Function

			Public Overrides Sub onCompletion(Of T1)(ByVal caller As java.util.concurrent.CountedCompleter(Of T1))
				If Not leaf Then
					Dim leftResult As S = leftChild.localResult
					leftResult.combine(rightChild.localResult)
					localResult = leftResult
				End If
				' GC spliterator, left and right child
				MyBase.onCompletion(caller)
			End Sub
		End Class
	End Class

End Namespace