Imports System

'
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

'
' *
' *
' *
' *
' *
' * Written by Doug Lea with assistance from members of JCP JSR-166
' * Expert Group and released to the public domain, as explained at
' * http://creativecommons.org/publicdomain/zero/1.0/
' 

Namespace java.util.concurrent.atomic

	''' <summary>
	''' An {@code int} value that may be updated atomically.  See the
	''' <seealso cref="java.util.concurrent.atomic"/> package specification for
	''' description of the properties of atomic variables. An
	''' {@code AtomicInteger} is used in applications such as atomically
	''' incremented counters, and cannot be used as a replacement for an
	''' <seealso cref="java.lang.Integer"/>. However, this class does extend
	''' {@code Number} to allow uniform access by tools and utilities that
	''' deal with numerically-based classes.
	''' 
	''' @since 1.5
	''' @author Doug Lea
	''' </summary>
	<Serializable> _
	Public Class AtomicInteger
		Inherits Number

		Private Const serialVersionUID As Long = 6214790243416807050L

		' setup to use Unsafe.compareAndSwapInt for updates
		Private Shared ReadOnly unsafe As sun.misc.Unsafe = sun.misc.Unsafe.unsafe
		Private Shared ReadOnly valueOffset As Long

		Shared Sub New()
			Try
				valueOffset = unsafe.objectFieldOffset(GetType(AtomicInteger).getDeclaredField("value"))
			Catch ex As Exception
				Throw New [Error](ex)
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private value As Integer

		''' <summary>
		''' Creates a new AtomicInteger with the given initial value.
		''' </summary>
		''' <param name="initialValue"> the initial value </param>
		Public Sub New(ByVal initialValue As Integer)
			value = initialValue
		End Sub

		''' <summary>
		''' Creates a new AtomicInteger with initial value {@code 0}.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Gets the current value.
		''' </summary>
		''' <returns> the current value </returns>
		Public Function [get]() As Integer
			Return value
		End Function

		''' <summary>
		''' Sets to the given value.
		''' </summary>
		''' <param name="newValue"> the new value </param>
		Public Sub [set](ByVal newValue As Integer)
			value = newValue
		End Sub

		''' <summary>
		''' Eventually sets to the given value.
		''' </summary>
		''' <param name="newValue"> the new value
		''' @since 1.6 </param>
		Public Sub lazySet(ByVal newValue As Integer)
			unsafe.putOrderedInt(Me, valueOffset, newValue)
		End Sub

		''' <summary>
		''' Atomically sets to the given value and returns the old value.
		''' </summary>
		''' <param name="newValue"> the new value </param>
		''' <returns> the previous value </returns>
		Public Function getAndSet(ByVal newValue As Integer) As Integer
			Return unsafe.getAndSetInt(Me, valueOffset, newValue)
		End Function

		''' <summary>
		''' Atomically sets the value to the given updated value
		''' if the current value {@code ==} the expected value.
		''' </summary>
		''' <param name="expect"> the expected value </param>
		''' <param name="update"> the new value </param>
		''' <returns> {@code true} if successful. False return indicates that
		''' the actual value was not equal to the expected value. </returns>
		Public Function compareAndSet(ByVal expect As Integer, ByVal update As Integer) As Boolean
			Return unsafe.compareAndSwapInt(Me, valueOffset, expect, update)
		End Function

		''' <summary>
		''' Atomically sets the value to the given updated value
		''' if the current value {@code ==} the expected value.
		''' 
		''' <p><a href="package-summary.html#weakCompareAndSet">May fail
		''' spuriously and does not provide ordering guarantees</a>, so is
		''' only rarely an appropriate alternative to {@code compareAndSet}.
		''' </summary>
		''' <param name="expect"> the expected value </param>
		''' <param name="update"> the new value </param>
		''' <returns> {@code true} if successful </returns>
		Public Function weakCompareAndSet(ByVal expect As Integer, ByVal update As Integer) As Boolean
			Return unsafe.compareAndSwapInt(Me, valueOffset, expect, update)
		End Function

		''' <summary>
		''' Atomically increments by one the current value.
		''' </summary>
		''' <returns> the previous value </returns>
		Public Property andIncrement As Integer
			Get
				Return unsafe.getAndAddInt(Me, valueOffset, 1)
			End Get
		End Property

		''' <summary>
		''' Atomically decrements by one the current value.
		''' </summary>
		''' <returns> the previous value </returns>
		Public Property andDecrement As Integer
			Get
				Return unsafe.getAndAddInt(Me, valueOffset, -1)
			End Get
		End Property

		''' <summary>
		''' Atomically adds the given value to the current value.
		''' </summary>
		''' <param name="delta"> the value to add </param>
		''' <returns> the previous value </returns>
		Public Function getAndAdd(ByVal delta As Integer) As Integer
			Return unsafe.getAndAddInt(Me, valueOffset, delta)
		End Function

		''' <summary>
		''' Atomically increments by one the current value.
		''' </summary>
		''' <returns> the updated value </returns>
		Public Function incrementAndGet() As Integer
			Return unsafe.getAndAddInt(Me, valueOffset, 1) + 1
		End Function

		''' <summary>
		''' Atomically decrements by one the current value.
		''' </summary>
		''' <returns> the updated value </returns>
		Public Function decrementAndGet() As Integer
			Return unsafe.getAndAddInt(Me, valueOffset, -1) - 1
		End Function

		''' <summary>
		''' Atomically adds the given value to the current value.
		''' </summary>
		''' <param name="delta"> the value to add </param>
		''' <returns> the updated value </returns>
		Public Function addAndGet(ByVal delta As Integer) As Integer
			Return unsafe.getAndAddInt(Me, valueOffset, delta) + delta
		End Function

		''' <summary>
		''' Atomically updates the current value with the results of
		''' applying the given function, returning the previous value. The
		''' function should be side-effect-free, since it may be re-applied
		''' when attempted updates fail due to contention among threads.
		''' </summary>
		''' <param name="updateFunction"> a side-effect-free function </param>
		''' <returns> the previous value
		''' @since 1.8 </returns>
		Public Function getAndUpdate(ByVal updateFunction As java.util.function.IntUnaryOperator) As Integer
			Dim prev, [next] As Integer
			Do
				prev = [get]()
				[next] = updateFunction.applyAsInt(prev)
			Loop While Not compareAndSet(prev, [next])
			Return prev
		End Function

		''' <summary>
		''' Atomically updates the current value with the results of
		''' applying the given function, returning the updated value. The
		''' function should be side-effect-free, since it may be re-applied
		''' when attempted updates fail due to contention among threads.
		''' </summary>
		''' <param name="updateFunction"> a side-effect-free function </param>
		''' <returns> the updated value
		''' @since 1.8 </returns>
		Public Function updateAndGet(ByVal updateFunction As java.util.function.IntUnaryOperator) As Integer
			Dim prev, [next] As Integer
			Do
				prev = [get]()
				[next] = updateFunction.applyAsInt(prev)
			Loop While Not compareAndSet(prev, [next])
			Return [next]
		End Function

		''' <summary>
		''' Atomically updates the current value with the results of
		''' applying the given function to the current and given values,
		''' returning the previous value. The function should be
		''' side-effect-free, since it may be re-applied when attempted
		''' updates fail due to contention among threads.  The function
		''' is applied with the current value as its first argument,
		''' and the given update as the second argument.
		''' </summary>
		''' <param name="x"> the update value </param>
		''' <param name="accumulatorFunction"> a side-effect-free function of two arguments </param>
		''' <returns> the previous value
		''' @since 1.8 </returns>
		Public Function getAndAccumulate(ByVal x As Integer, ByVal accumulatorFunction As java.util.function.IntBinaryOperator) As Integer
			Dim prev, [next] As Integer
			Do
				prev = [get]()
				[next] = accumulatorFunction.applyAsInt(prev, x)
			Loop While Not compareAndSet(prev, [next])
			Return prev
		End Function

		''' <summary>
		''' Atomically updates the current value with the results of
		''' applying the given function to the current and given values,
		''' returning the updated value. The function should be
		''' side-effect-free, since it may be re-applied when attempted
		''' updates fail due to contention among threads.  The function
		''' is applied with the current value as its first argument,
		''' and the given update as the second argument.
		''' </summary>
		''' <param name="x"> the update value </param>
		''' <param name="accumulatorFunction"> a side-effect-free function of two arguments </param>
		''' <returns> the updated value
		''' @since 1.8 </returns>
		Public Function accumulateAndGet(ByVal x As Integer, ByVal accumulatorFunction As java.util.function.IntBinaryOperator) As Integer
			Dim prev, [next] As Integer
			Do
				prev = [get]()
				[next] = accumulatorFunction.applyAsInt(prev, x)
			Loop While Not compareAndSet(prev, [next])
			Return [next]
		End Function

		''' <summary>
		''' Returns the String representation of the current value. </summary>
		''' <returns> the String representation of the current value </returns>
		Public Overrides Function ToString() As String
			Return Convert.ToString([get]())
		End Function

		''' <summary>
		''' Returns the value of this {@code AtomicInteger} as an {@code int}.
		''' </summary>
		Public Overrides Function intValue() As Integer
			Return [get]()
		End Function

		''' <summary>
		''' Returns the value of this {@code AtomicInteger} as a {@code long}
		''' after a widening primitive conversion.
		''' @jls 5.1.2 Widening Primitive Conversions
		''' </summary>
		Public Overrides Function longValue() As Long
			Return CLng([get]())
		End Function

		''' <summary>
		''' Returns the value of this {@code AtomicInteger} as a {@code float}
		''' after a widening primitive conversion.
		''' @jls 5.1.2 Widening Primitive Conversions
		''' </summary>
		Public Overrides Function floatValue() As Single
			Return CSng([get]())
		End Function

		''' <summary>
		''' Returns the value of this {@code AtomicInteger} as a {@code double}
		''' after a widening primitive conversion.
		''' @jls 5.1.2 Widening Primitive Conversions
		''' </summary>
		Public Overrides Function doubleValue() As Double
			Return CDbl([get]())
		End Function

	End Class

End Namespace