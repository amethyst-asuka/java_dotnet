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
	''' summary statistics on a stream of longs with:
	''' <pre> {@code
	''' LongSummaryStatistics stats = longStream.collect(LongSummaryStatistics::new,
	'''                                                  LongSummaryStatistics::accept,
	'''                                                  LongSummaryStatistics::combine);
	''' }</pre>
	''' 
	''' <p>{@code LongSummaryStatistics} can be used as a
	''' <seealso cref="java.util.stream.Stream#collect(Collector)"/> reduction}
	''' target for a <seealso cref="java.util.stream.Stream stream"/>. For example:
	''' 
	''' <pre> {@code
	''' LongSummaryStatistics stats = people.stream()
	'''                                     .collect(Collectors.summarizingLong(Person::getAge));
	''' }</pre>
	''' 
	''' This computes, in a single pass, the count of people, as well as the minimum,
	''' maximum, sum, and average of their ages.
	''' 
	''' @implNote This implementation is not thread safe. However, it is safe to use
	''' {@link java.util.stream.Collectors#summarizingLong(java.util.function.ToLongFunction)
	''' Collectors.toLongStatistics()} on a parallel stream, because the parallel
	''' implementation of <seealso cref="java.util.stream.Stream#collect Stream.collect()"/>
	''' provides the necessary partitioning, isolation, and merging of results for
	''' safe and efficient parallel execution.
	''' 
	''' <p>This implementation does not check for overflow of the sum.
	''' @since 1.8
	''' </summary>
	Public Class LongSummaryStatistics
		Implements java.util.function.LongConsumer, java.util.function.IntConsumer

		Private count As Long
		Private sum As Long
		Private min As Long = Long.MAX_VALUE
		Private max As Long = Long.MIN_VALUE

		''' <summary>
		''' Construct an empty instance with zero count, zero sum,
		''' {@code Long.MAX_VALUE} min, {@code Long.MIN_VALUE} max and zero
		''' average.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Records a new {@code int} value into the summary information.
		''' </summary>
		''' <param name="value"> the input value </param>
		Public Overrides Sub accept(ByVal value As Integer)
			accept(CLng(value))
		End Sub

		''' <summary>
		''' Records a new {@code long} value into the summary information.
		''' </summary>
		''' <param name="value"> the input value </param>
		Public Overrides Sub accept(ByVal value As Long)
			count += 1
			sum += value
			min = Math.Min(min, value)
			max = Math.Max(max, value)
		End Sub

		''' <summary>
		''' Combines the state of another {@code LongSummaryStatistics} into this
		''' one.
		''' </summary>
		''' <param name="other"> another {@code LongSummaryStatistics} </param>
		''' <exception cref="NullPointerException"> if {@code other} is null </exception>
		Public Overridable Sub combine(ByVal other As LongSummaryStatistics)
			count += other.count
			sum += other.sum
			min = Math.Min(min, other.min)
			max = Math.Max(max, other.max)
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
		''' Returns the minimum value recorded, or {@code Long.MAX_VALUE} if no
		''' values have been recorded.
		''' </summary>
		''' <returns> the minimum value, or {@code Long.MAX_VALUE} if none </returns>
		Public Property min As Long
			Get
				Return min
			End Get
		End Property

		''' <summary>
		''' Returns the maximum value recorded, or {@code Long.MIN_VALUE} if no
		''' values have been recorded
		''' </summary>
		''' <returns> the maximum value, or {@code Long.MIN_VALUE} if none </returns>
		Public Property max As Long
			Get
				Return max
			End Get
		End Property

		''' <summary>
		''' Returns the arithmetic mean of values recorded, or zero if no values have been
		''' recorded.
		''' </summary>
		''' <returns> The arithmetic mean of values, or zero if none </returns>
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