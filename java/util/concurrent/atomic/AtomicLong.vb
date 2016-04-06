Imports System
Imports System.Runtime.InteropServices

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
	''' A {@code long} value that may be updated atomically.  See the
	''' <seealso cref="java.util.concurrent.atomic"/> package specification for
	''' description of the properties of atomic variables. An
	''' {@code AtomicLong} is used in applications such as atomically
	''' incremented sequence numbers, and cannot be used as a replacement
	''' for a <seealso cref="java.lang.Long"/>. However, this class does extend
	''' {@code Number} to allow uniform access by tools and utilities that
	''' deal with numerically-based classes.
	''' 
	''' @since 1.5
	''' @author Doug Lea
	''' </summary>
	<Serializable> _
	Public Class AtomicLong
		Inherits Number

		Private Const serialVersionUID As Long = 1927816293512124184L

		' setup to use Unsafe.compareAndSwapLong for updates
		Private Shared ReadOnly unsafe As sun.misc.Unsafe = sun.misc.Unsafe.unsafe
		Private Shared ReadOnly valueOffset As Long

		''' <summary>
		''' Records whether the underlying JVM supports lockless
		''' compareAndSwap for longs. While the Unsafe.compareAndSwapLong
		''' method works in either case, some constructions should be
		''' handled at Java level to avoid locking user-visible locks.
		''' </summary>
		Friend Shared ReadOnly VM_SUPPORTS_LONG_CAS As Boolean = VMSupportsCS8()

		''' <summary>
		''' Returns whether underlying JVM supports lockless CompareAndSet
		''' for longs. Called only once and cached in VM_SUPPORTS_LONG_CAS.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function VMSupportsCS8() As Boolean
		End Function

		Shared Sub New()
			Try
				valueOffset = unsafe.objectFieldOffset(GetType(AtomicLong).getDeclaredField("value"))
			Catch ex As Exception
				Throw New [Error](ex)
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private value As Long

		''' <summary>
		''' Creates a new AtomicLong with the given initial value.
		''' </summary>
		''' <param name="initialValue"> the initial value </param>
		Public Sub New(  initialValue As Long)
			value = initialValue
		End Sub

		''' <summary>
		''' Creates a new AtomicLong with initial value {@code 0}.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Gets the current value.
		''' </summary>
		''' <returns> the current value </returns>
		Public Function [get]() As Long
			Return value
		End Function

		''' <summary>
		''' Sets to the given value.
		''' </summary>
		''' <param name="newValue"> the new value </param>
		Public Sub [set](  newValue As Long)
			value = newValue
		End Sub

		''' <summary>
		''' Eventually sets to the given value.
		''' </summary>
		''' <param name="newValue"> the new value
		''' @since 1.6 </param>
		Public Sub lazySet(  newValue As Long)
			unsafe.putOrderedLong(Me, valueOffset, newValue)
		End Sub

		''' <summary>
		''' Atomically sets to the given value and returns the old value.
		''' </summary>
		''' <param name="newValue"> the new value </param>
		''' <returns> the previous value </returns>
		Public Function getAndSet(  newValue As Long) As Long
			Return unsafe.getAndSetLong(Me, valueOffset, newValue)
		End Function

		''' <summary>
		''' Atomically sets the value to the given updated value
		''' if the current value {@code ==} the expected value.
		''' </summary>
		''' <param name="expect"> the expected value </param>
		''' <param name="update"> the new value </param>
		''' <returns> {@code true} if successful. False return indicates that
		''' the actual value was not equal to the expected value. </returns>
		Public Function compareAndSet(  expect As Long,   update As Long) As Boolean
			Return unsafe.compareAndSwapLong(Me, valueOffset, expect, update)
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
		Public Function weakCompareAndSet(  expect As Long,   update As Long) As Boolean
			Return unsafe.compareAndSwapLong(Me, valueOffset, expect, update)
		End Function

		''' <summary>
		''' Atomically increments by one the current value.
		''' </summary>
		''' <returns> the previous value </returns>
		Public Property andIncrement As Long
			Get
				Return unsafe.getAndAddLong(Me, valueOffset, 1L)
			End Get
		End Property

		''' <summary>
		''' Atomically decrements by one the current value.
		''' </summary>
		''' <returns> the previous value </returns>
		Public Property andDecrement As Long
			Get
				Return unsafe.getAndAddLong(Me, valueOffset, -1L)
			End Get
		End Property

		''' <summary>
		''' Atomically adds the given value to the current value.
		''' </summary>
		''' <param name="delta"> the value to add </param>
		''' <returns> the previous value </returns>
		Public Function getAndAdd(  delta As Long) As Long
			Return unsafe.getAndAddLong(Me, valueOffset, delta)
		End Function

		''' <summary>
		''' Atomically increments by one the current value.
		''' </summary>
		''' <returns> the updated value </returns>
		Public Function incrementAndGet() As Long
			Return unsafe.getAndAddLong(Me, valueOffset, 1L) + 1L
		End Function

		''' <summary>
		''' Atomically decrements by one the current value.
		''' </summary>
		''' <returns> the updated value </returns>
		Public Function decrementAndGet() As Long
			Return unsafe.getAndAddLong(Me, valueOffset, -1L) - 1L
		End Function

		''' <summary>
		''' Atomically adds the given value to the current value.
		''' </summary>
		''' <param name="delta"> the value to add </param>
		''' <returns> the updated value </returns>
		Public Function addAndGet(  delta As Long) As Long
			Return unsafe.getAndAddLong(Me, valueOffset, delta) + delta
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
		Public Function getAndUpdate(  updateFunction As java.util.function.LongUnaryOperator) As Long
			Dim prev, [next] As Long
			Do
				prev = [get]()
				[next] = updateFunction.applyAsLong(prev)
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
		Public Function updateAndGet(  updateFunction As java.util.function.LongUnaryOperator) As Long
			Dim prev, [next] As Long
			Do
				prev = [get]()
				[next] = updateFunction.applyAsLong(prev)
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
		Public Function getAndAccumulate(  x As Long,   accumulatorFunction As java.util.function.LongBinaryOperator) As Long
			Dim prev, [next] As Long
			Do
				prev = [get]()
				[next] = accumulatorFunction.applyAsLong(prev, x)
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
		Public Function accumulateAndGet(  x As Long,   accumulatorFunction As java.util.function.LongBinaryOperator) As Long
			Dim prev, [next] As Long
			Do
				prev = [get]()
				[next] = accumulatorFunction.applyAsLong(prev, x)
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
		''' Returns the value of this {@code AtomicLong} as an {@code int}
		''' after a narrowing primitive conversion.
		''' @jls 5.1.3 Narrowing Primitive Conversions
		''' </summary>
		Public Overrides Function intValue() As Integer
			Return CInt([get]())
		End Function

		''' <summary>
		''' Returns the value of this {@code AtomicLong} as a {@code long}.
		''' </summary>
		Public Overrides Function longValue() As Long
			Return [get]()
		End Function

		''' <summary>
		''' Returns the value of this {@code AtomicLong} as a {@code float}
		''' after a widening primitive conversion.
		''' @jls 5.1.2 Widening Primitive Conversions
		''' </summary>
		Public Overrides Function floatValue() As Single
			Return CSng([get]())
		End Function

		''' <summary>
		''' Returns the value of this {@code AtomicLong} as a {@code double}
		''' after a widening primitive conversion.
		''' @jls 5.1.2 Widening Primitive Conversions
		''' </summary>
		Public Overrides Function doubleValue() As Double
			Return CDbl([get]())
		End Function

	End Class

End Namespace