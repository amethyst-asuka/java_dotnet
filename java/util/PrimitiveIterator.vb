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
Namespace java.util


	''' <summary>
	''' A base type for primitive specializations of {@code Iterator}.  Specialized
	''' subtypes are provided for <seealso cref="OfInt int"/>, <seealso cref="OfLong long"/>, and
	''' <seealso cref="OfDouble double"/> values.
	''' 
	''' <p>The specialized subtype default implementations of <seealso cref="Iterator#next"/>
	''' and <seealso cref="Iterator#forEachRemaining(java.util.function.Consumer)"/> box
	''' primitive values to instances of their corresponding wrapper class.  Such
	''' boxing may offset any advantages gained when using the primitive
	''' specializations.  To avoid boxing, the corresponding primitive-based methods
	''' should be used.  For example, <seealso cref="PrimitiveIterator.OfInt#nextInt()"/> and
	''' <seealso cref="PrimitiveIterator.OfInt#forEachRemaining(java.util.function.IntConsumer)"/>
	''' should be used in preference to <seealso cref="PrimitiveIterator.OfInt#next()"/> and
	''' <seealso cref="PrimitiveIterator.OfInt#forEachRemaining(java.util.function.Consumer)"/>.
	''' 
	''' <p>Iteration of primitive values using boxing-based methods
	''' <seealso cref="Iterator#next next()"/> and
	''' <seealso cref="Iterator#forEachRemaining(java.util.function.Consumer) forEachRemaining()"/>,
	''' does not affect the order in which the values, transformed to boxed values,
	''' are encountered.
	''' 
	''' @implNote
	''' If the boolean system property {@code org.openjdk.java.util.stream.tripwire}
	''' is set to {@code true} then diagnostic warnings are reported if boxing of
	''' primitive values occur when operating on primitive subtype specializations.
	''' </summary>
	''' @param <T> the type of elements returned by this PrimitiveIterator.  The
	'''        type must be a wrapper type for a primitive type, such as
	'''        {@code Integer} for the primitive {@code int} type. </param>
	''' @param <T_CONS> the type of primitive consumer.  The type must be a
	'''        primitive specialization of <seealso cref="java.util.function.Consumer"/> for
	'''        {@code T}, such as <seealso cref="java.util.function.IntConsumer"/> for
	'''        {@code Integer}.
	''' 
	''' @since 1.8 </param>
	Public Interface PrimitiveIterator(Of T, T_CONS)
		Inherits [Iterator](Of T)

		''' <summary>
		''' Performs the given action for each remaining element, in the order
		''' elements occur when iterating, until all elements have been processed
		''' or the action throws an exception.  Errors or runtime exceptions
		''' thrown by the action are relayed to the caller.
		''' </summary>
		''' <param name="action"> The action to be performed for each element </param>
		''' <exception cref="NullPointerException"> if the specified action is null </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Sub forEachRemaining(ByVal action As T_CONS)

		''' <summary>
		''' An Iterator specialized for {@code int} values.
		''' @since 1.8
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interface inner types are not converted:
'		public static interface OfInt extends PrimitiveIterator(Of java.lang.Integer, java.util.function.IntConsumer)
	'	{
	'
	'		''' <summary>
	'		''' Returns the next {@code int} element in the iteration.
	'		''' </summary>
	'		''' <returns> the next {@code int} element in the iteration </returns>
	'		''' <exception cref="NoSuchElementException"> if the iteration has no more elements </exception>
	'		int nextInt();
	'
	'		''' <summary>
	'		''' Performs the given action for each remaining element until all elements
	'		''' have been processed or the action throws an exception.  Actions are
	'		''' performed in the order of iteration, if that order is specified.
	'		''' Exceptions thrown by the action are relayed to the caller.
	'		''' 
	'		''' @implSpec
	'		''' <p>The default implementation behaves as if:
	'		''' <pre>{@code
	'		'''     while (hasNext())
	'		'''         action.accept(nextInt());
	'		''' }</pre>
	'		''' </summary>
	'		''' <param name="action"> The action to be performed for each element </param>
	'		''' <exception cref="NullPointerException"> if the specified action is null </exception>
	'		default  Sub  forEachRemaining(IntConsumer action)
	'		{
	'			Objects.requireNonNull(action);
	'			while (hasNext())
	'				action.accept(nextInt());
	'		}
	'
	'		''' <summary>
	'		''' {@inheritDoc}
	'		''' @implSpec
	'		''' The default implementation boxes the result of calling
	'		''' <seealso cref="#nextInt()"/>, and returns that boxed result.
	'		''' </summary>
	'		@Override default java.lang.Integer next()
	'		{
	'			if (Tripwire.ENABLED)
	'				Tripwire.trip(getClass(), "{0} calling PrimitiveIterator.OfInt.nextInt()");
	'			Return nextInt();
	'		}
	'
	'		''' <summary>
	'		''' {@inheritDoc}
	'		''' @implSpec
	'		''' If the action is an instance of {@code IntConsumer} then it is cast
	'		''' to {@code IntConsumer} and passed to <seealso cref="#forEachRemaining"/>;
	'		''' otherwise the action is adapted to an instance of
	'		''' {@code IntConsumer}, by boxing the argument of {@code IntConsumer},
	'		''' and then passed to <seealso cref="#forEachRemaining"/>.
	'		''' </summary>
	'		@Override default  Sub  forEachRemaining(Consumer<? MyBase java.lang.Integer> action)
	'		{
	'			if (action instanceof IntConsumer)
	'			{
	'				forEachRemaining((IntConsumer) action);
	'			}
	'			else
	'			{
	'				' The method reference action::accept is never null
	'				Objects.requireNonNull(action);
	'				if (Tripwire.ENABLED)
	'					Tripwire.trip(getClass(), "{0} calling PrimitiveIterator.OfInt.forEachRemainingInt(action::accept)");
	'				forEachRemaining((IntConsumer) action::accept);
	'			}
	'		}
	'
	'	}

		''' <summary>
		''' An Iterator specialized for {@code long} values.
		''' @since 1.8
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interface inner types are not converted:
'		public static interface OfLong extends PrimitiveIterator(Of java.lang.Long, java.util.function.LongConsumer)
	'	{
	'
	'		''' <summary>
	'		''' Returns the next {@code long} element in the iteration.
	'		''' </summary>
	'		''' <returns> the next {@code long} element in the iteration </returns>
	'		''' <exception cref="NoSuchElementException"> if the iteration has no more elements </exception>
	'		long nextLong();
	'
	'		''' <summary>
	'		''' Performs the given action for each remaining element until all elements
	'		''' have been processed or the action throws an exception.  Actions are
	'		''' performed in the order of iteration, if that order is specified.
	'		''' Exceptions thrown by the action are relayed to the caller.
	'		''' 
	'		''' @implSpec
	'		''' <p>The default implementation behaves as if:
	'		''' <pre>{@code
	'		'''     while (hasNext())
	'		'''         action.accept(nextLong());
	'		''' }</pre>
	'		''' </summary>
	'		''' <param name="action"> The action to be performed for each element </param>
	'		''' <exception cref="NullPointerException"> if the specified action is null </exception>
	'		default  Sub  forEachRemaining(LongConsumer action)
	'		{
	'			Objects.requireNonNull(action);
	'			while (hasNext())
	'				action.accept(nextLong());
	'		}
	'
	'		''' <summary>
	'		''' {@inheritDoc}
	'		''' @implSpec
	'		''' The default implementation boxes the result of calling
	'		''' <seealso cref="#nextLong()"/>, and returns that boxed result.
	'		''' </summary>
	'		@Override default java.lang.Long next()
	'		{
	'			if (Tripwire.ENABLED)
	'				Tripwire.trip(getClass(), "{0} calling PrimitiveIterator.OfLong.nextLong()");
	'			Return nextLong();
	'		}
	'
	'		''' <summary>
	'		''' {@inheritDoc}
	'		''' @implSpec
	'		''' If the action is an instance of {@code LongConsumer} then it is cast
	'		''' to {@code LongConsumer} and passed to <seealso cref="#forEachRemaining"/>;
	'		''' otherwise the action is adapted to an instance of
	'		''' {@code LongConsumer}, by boxing the argument of {@code LongConsumer},
	'		''' and then passed to <seealso cref="#forEachRemaining"/>.
	'		''' </summary>
	'		@Override default  Sub  forEachRemaining(Consumer<? MyBase java.lang.Long> action)
	'		{
	'			if (action instanceof LongConsumer)
	'			{
	'				forEachRemaining((LongConsumer) action);
	'			}
	'			else
	'			{
	'				' The method reference action::accept is never null
	'				Objects.requireNonNull(action);
	'				if (Tripwire.ENABLED)
	'					Tripwire.trip(getClass(), "{0} calling PrimitiveIterator.OfLong.forEachRemainingLong(action::accept)");
	'				forEachRemaining((LongConsumer) action::accept);
	'			}
	'		}
	'	}

		''' <summary>
		''' An Iterator specialized for {@code double} values.
		''' @since 1.8
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interface inner types are not converted:
'		public static interface OfDouble extends PrimitiveIterator(Of java.lang.Double, java.util.function.DoubleConsumer)
	'	{
	'
	'		''' <summary>
	'		''' Returns the next {@code double} element in the iteration.
	'		''' </summary>
	'		''' <returns> the next {@code double} element in the iteration </returns>
	'		''' <exception cref="NoSuchElementException"> if the iteration has no more elements </exception>
	'		double nextDouble();
	'
	'		''' <summary>
	'		''' Performs the given action for each remaining element until all elements
	'		''' have been processed or the action throws an exception.  Actions are
	'		''' performed in the order of iteration, if that order is specified.
	'		''' Exceptions thrown by the action are relayed to the caller.
	'		''' 
	'		''' @implSpec
	'		''' <p>The default implementation behaves as if:
	'		''' <pre>{@code
	'		'''     while (hasNext())
	'		'''         action.accept(nextDouble());
	'		''' }</pre>
	'		''' </summary>
	'		''' <param name="action"> The action to be performed for each element </param>
	'		''' <exception cref="NullPointerException"> if the specified action is null </exception>
	'		default  Sub  forEachRemaining(DoubleConsumer action)
	'		{
	'			Objects.requireNonNull(action);
	'			while (hasNext())
	'				action.accept(nextDouble());
	'		}
	'
	'		''' <summary>
	'		''' {@inheritDoc}
	'		''' @implSpec
	'		''' The default implementation boxes the result of calling
	'		''' <seealso cref="#nextDouble()"/>, and returns that boxed result.
	'		''' </summary>
	'		@Override default java.lang.Double next()
	'		{
	'			if (Tripwire.ENABLED)
	'				Tripwire.trip(getClass(), "{0} calling PrimitiveIterator.OfDouble.nextLong()");
	'			Return nextDouble();
	'		}
	'
	'		''' <summary>
	'		''' {@inheritDoc}
	'		''' @implSpec
	'		''' If the action is an instance of {@code DoubleConsumer} then it is
	'		''' cast to {@code DoubleConsumer} and passed to
	'		''' <seealso cref="#forEachRemaining"/>; otherwise the action is adapted to
	'		''' an instance of {@code DoubleConsumer}, by boxing the argument of
	'		''' {@code DoubleConsumer}, and then passed to
	'		''' <seealso cref="#forEachRemaining"/>.
	'		''' </summary>
	'		@Override default  Sub  forEachRemaining(Consumer<? MyBase java.lang.Double> action)
	'		{
	'			if (action instanceof DoubleConsumer)
	'			{
	'				forEachRemaining((DoubleConsumer) action);
	'			}
	'			else
	'			{
	'				' The method reference action::accept is never null
	'				Objects.requireNonNull(action);
	'				if (Tripwire.ENABLED)
	'					Tripwire.trip(getClass(), "{0} calling PrimitiveIterator.OfDouble.forEachRemainingDouble(action::accept)");
	'				forEachRemaining((DoubleConsumer) action::accept);
	'			}
	'		}
	'	}
	End Interface

End Namespace