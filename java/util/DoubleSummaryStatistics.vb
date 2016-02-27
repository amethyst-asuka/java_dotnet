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
	''' summary statistics on a stream of doubles with:
	''' <pre> {@code
	''' DoubleSummaryStatistics stats = doubleStream.collect(DoubleSummaryStatistics::new,
	'''                                                      DoubleSummaryStatistics::accept,
	'''                                                      DoubleSummaryStatistics::combine);
	''' }</pre>
	''' 
	''' <p>{@code DoubleSummaryStatistics} can be used as a
	''' <seealso cref="java.util.stream.Stream#collect(Collector) reduction"/>
	''' target for a <seealso cref="java.util.stream.Stream stream"/>. For example:
	''' 
	''' <pre> {@code
	''' DoubleSummaryStatistics stats = people.stream()
	'''     .collect(Collectors.summarizingDouble(Person::getWeight));
	''' }</pre>
	''' 
	''' This computes, in a single pass, the count of people, as well as the minimum,
	''' maximum, sum, and average of their weights.
	''' 
	''' @implNote This implementation is not thread safe. However, it is safe to use
	''' {@link java.util.stream.Collectors#summarizingDouble(java.util.function.ToDoubleFunction)
	''' Collectors.toDoubleStatistics()} on a parallel stream, because the parallel
	''' implementation of <seealso cref="java.util.stream.Stream#collect Stream.collect()"/>
	''' provides the necessary partitioning, isolation, and merging of results for
	''' safe and efficient parallel execution.
	''' @since 1.8
	''' </summary>
	Public Class DoubleSummaryStatistics
		Implements java.util.function.DoubleConsumer

		Private count As Long
		Private sum As Double
		Private sumCompensation As Double ' Low order bits of sum
		Private simpleSum As Double ' Used to compute right sum for non-finite inputs
		Private min As Double = java.lang.[Double].POSITIVE_INFINITY
		Private max As Double = java.lang.[Double].NEGATIVE_INFINITY

		''' <summary>
		''' Construct an empty instance with zero count, zero sum,
		''' {@code java.lang.[Double].POSITIVE_INFINITY} min, {@code java.lang.[Double].NEGATIVE_INFINITY}
		''' max and zero average.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Records another value into the summary information.
		''' </summary>
		''' <param name="value"> the input value </param>
		Public Overrides Sub accept(ByVal value As Double)
			count += 1
			simpleSum += value
			sumWithCompensation(value)
			min = System.Math.Min(min, value)
			max = System.Math.Max(max, value)
		End Sub

		''' <summary>
		''' Combines the state of another {@code DoubleSummaryStatistics} into this
		''' one.
		''' </summary>
		''' <param name="other"> another {@code DoubleSummaryStatistics} </param>
		''' <exception cref="NullPointerException"> if {@code other} is null </exception>
		Public Overridable Sub combine(ByVal other As DoubleSummaryStatistics)
			count += other.count
			simpleSum += other.simpleSum
			sumWithCompensation(other.sum)
			sumWithCompensation(other.sumCompensation)
			min = System.Math.Min(min, other.min)
			max = System.Math.Max(max, other.max)
		End Sub

		''' <summary>
		''' Incorporate a new double value using Kahan summation /
		''' compensated summation.
		''' </summary>
		Private Sub sumWithCompensation(ByVal value As Double)
			Dim tmp As Double = value - sumCompensation
			Dim velvel As Double = sum + tmp ' Little wolf of rounding error
			sumCompensation = (velvel - sum) - tmp
			sum = velvel
		End Sub

		''' <summary>
		''' Return the count of values recorded.
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
		''' 
		''' If any recorded value is a NaN or the sum is at any point a NaN
		''' then the sum will be NaN.
		''' 
		''' <p> The value of a floating-point sum is a function both of the
		''' input values as well as the order of addition operations. The
		''' order of addition operations of this method is intentionally
		''' not defined to allow for implementation flexibility to improve
		''' the speed and accuracy of the computed result.
		''' 
		''' In particular, this method may be implemented using compensated
		''' summation or other technique to reduce the error bound in the
		''' numerical sum compared to a simple summation of {@code double}
		''' values.
		''' 
		''' @apiNote Values sorted by increasing absolute magnitude tend to yield
		''' more accurate results.
		''' </summary>
		''' <returns> the sum of values, or zero if none </returns>
		Public Property sum As Double
			Get
				' Better error bounds to add both terms as the final sum
				Dim tmp As Double = sum + sumCompensation
				If java.lang.[Double].IsNaN(tmp) AndAlso java.lang.[Double].IsInfinity(simpleSum) Then
					' If the compensated sum is spuriously NaN from
					' accumulating one or more same-signed infinite values,
					' return the correctly-signed infinity stored in
					' simpleSum.
					Return simpleSum
				Else
					Return tmp
				End If
			End Get
		End Property

		''' <summary>
		''' Returns the minimum recorded value, {@code java.lang.[Double].NaN} if any recorded
		''' value was NaN or {@code java.lang.[Double].POSITIVE_INFINITY} if no values were
		''' recorded. Unlike the numerical comparison operators, this method
		''' considers negative zero to be strictly smaller than positive zero.
		''' </summary>
		''' <returns> the minimum recorded value, {@code java.lang.[Double].NaN} if any recorded
		''' value was NaN or {@code java.lang.[Double].POSITIVE_INFINITY} if no values were
		''' recorded </returns>
		Public Property min As Double
			Get
				Return min
			End Get
		End Property

		''' <summary>
		''' Returns the maximum recorded value, {@code java.lang.[Double].NaN} if any recorded
		''' value was NaN or {@code java.lang.[Double].NEGATIVE_INFINITY} if no values were
		''' recorded. Unlike the numerical comparison operators, this method
		''' considers negative zero to be strictly smaller than positive zero.
		''' </summary>
		''' <returns> the maximum recorded value, {@code java.lang.[Double].NaN} if any recorded
		''' value was NaN or {@code java.lang.[Double].NEGATIVE_INFINITY} if no values were
		''' recorded </returns>
		Public Property max As Double
			Get
				Return max
			End Get
		End Property

		''' <summary>
		''' Returns the arithmetic mean of values recorded, or zero if no
		''' values have been recorded.
		''' 
		''' If any recorded value is a NaN or the sum is at any point a NaN
		''' then the average will be code NaN.
		''' 
		''' <p>The average returned can vary depending upon the order in
		''' which values are recorded.
		''' 
		''' This method may be implemented using compensated summation or
		''' other technique to reduce the error bound in the {@link #getSum
		''' numerical sum} used to compute the average.
		''' 
		''' @apiNote Values sorted by increasing absolute magnitude tend to yield
		''' more accurate results.
		''' </summary>
		''' <returns> the arithmetic mean of values, or zero if none </returns>
		Public Property average As Double
			Get
				Return If(count > 0, sum / count, 0.0R)
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' Returns a non-empty string representation of this object suitable for
		''' debugging. The exact presentation format is unspecified and may vary
		''' between implementations and versions.
		''' </summary>
		Public Overrides Function ToString() As String
			Return String.Format("{0}{{count={1:D}, sum={2:F}, min={3:F}, average={4:F}, max={5:F}}}", Me.GetType().Name, count, sum, min, average, max)
		End Function
	End Class

End Namespace