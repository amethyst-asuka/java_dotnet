Imports System

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
Namespace java.util


	''' <summary>
	''' A state object for collecting statistics such as count, min, max, sum, and
	''' average.
	''' 
	''' <p>This class is designed to work with (though does not require)
	''' <seealso cref="java.util.stream streams"/>. For example, you can compute
	''' summary statistics on a stream of ints with:
	''' <pre> {@code
	''' IntSummaryStatistics stats = intStream.collect(IntSummaryStatistics::new,
	'''                                                IntSummaryStatistics::accept,
	'''                                                IntSummaryStatistics::combine);
	''' }</pre>
	''' 
	''' <p>{@code IntSummaryStatistics} can be used as a
	''' <seealso cref="java.util.stream.Stream#collect(Collector) reduction"/>
	''' target for a <seealso cref="java.util.stream.Stream stream"/>. For example:
	''' 
	''' <pre> {@code
	''' IntSummaryStatistics stats = people.stream()
	'''                                    .collect(Collectors.summarizingInt(Person::getDependents));
	''' }</pre>
	''' 
	''' This computes, in a single pass, the count of people, as well as the minimum,
	''' maximum, sum, and average of their number of dependents.
	''' 
	''' @implNote This implementation is not thread safe. However, it is safe to use
	''' {@link java.util.stream.Collectors#summarizingInt(java.util.function.ToIntFunction)
	''' Collectors.toIntStatistics()} on a parallel stream, because the parallel
	''' implementation of <seealso cref="java.util.stream.Stream#collect Stream.collect()"/>
	''' provides the necessary partitioning, isolation, and merging of results for
	''' safe and efficient parallel execution.
	''' 
	''' <p>This implementation does not check for overflow of the sum.
	''' @since 1.8
	''' </summary>
	Public Class IntSummaryStatistics
		Implements java.util.function.IntConsumer

		Private count As Long
		Private sum As Long
		Private min As Integer =  java.lang.[Integer].MAX_VALUE
		Private max As Integer =  java.lang.[Integer].MIN_VALUE

		''' <summary>
		''' Construct an empty instance with zero count, zero sum,
		''' {@code  java.lang.[Integer].MAX_VALUE} min, {@code  java.lang.[Integer].MIN_VALUE} max and zero
		''' average.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Records a new value into the summary information
		''' </summary>
		''' <param name="value"> the input value </param>
		Public Overrides Sub accept(ByVal value As Integer)
			count += 1
			sum += value
			min = System.Math.Min(min, value)
			max = System.Math.Max(max, value)
		End Sub

		''' <summary>
		''' Combines the state of another {@code IntSummaryStatistics} into this one.
		''' </summary>
		''' <param name="other"> another {@code IntSummaryStatistics} </param>
		''' <exception cref="NullPointerException"> if {@code other} is null </exception>
		Public Overridable Sub combine(ByVal other As IntSummaryStatistics)
			count += other.count
			sum += other.sum
			min = System.Math.Min(min, other.min)
			max = System.Math.Max(max, other.max)
		End Sub

		''' <summary>
		''' Returns the count of values recorded.
		''' </summary>
		''' <returns> the count of values </returns>
		Public Property count As Long
			Get
				Return count
			End Get
		End Property

		''' <summary>
		''' Returns the sum of values recorded, or zero if no values have been
		''' recorded.
		''' </summary>
		''' <returns> the sum of values, or zero if none </returns>
		Public Property sum As Long
			Get
				Return sum
			End Get
		End Property

		''' <summary>
		''' Returns the minimum value recorded, or {@code  java.lang.[Integer].MAX_VALUE} if no
		''' values have been recorded.
		''' </summary>
		''' <returns> the minimum value, or {@code  java.lang.[Integer].MAX_VALUE} if none </returns>
		Public Property min As Integer
			Get
				Return min
			End Get
		End Property

		''' <summary>
		''' Returns the maximum value recorded, or {@code  java.lang.[Integer].MIN_VALUE} if no
		''' values have been recorded.
		''' </summary>
		''' <returns> the maximum value, or {@code  java.lang.[Integer].MIN_VALUE} if none </returns>
		Public Property max As Integer
			Get
				Return max
			End Get
		End Property

		''' <summary>
		''' Returns the arithmetic mean of values recorded, or zero if no values have been
		''' recorded.
		''' </summary>
		''' <returns> the arithmetic mean of values, or zero if none </returns>
		Public Property average As Double
			Get
				Return If(count > 0, CDbl(sum) / count, 0.0R)
			End Get
		End Property

		Public Overrides Function ToString() As String
		''' <summary>
		''' {@inheritDoc}
		''' 
		''' Returns a non-empty string representation of this object suitable for
		''' debugging. The exact presentation format is unspecified and may vary
		''' between implementations and versions.
		''' </summary>
			Return String.Format("{0}{{count={1:D}, sum={2:D}, min={3:D}, average={4:F}, max={5:D}}}", Me.GetType().Name, count, sum, min, average, max)
		End Function
	End Class

End Namespace