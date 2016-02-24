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
	''' A {@code long} array in which elements may be updated atomically.
	''' See the <seealso cref="java.util.concurrent.atomic"/> package specification
	''' for description of the properties of atomic variables.
	''' @since 1.5
	''' @author Doug Lea
	''' </summary>
	<Serializable> _
	Public Class AtomicLongArray
		Private Const serialVersionUID As Long = -2308431214976778248L

		Private Shared ReadOnly unsafe As sun.misc.Unsafe = sun.misc.Unsafe.unsafe
		Private Shared ReadOnly base As Integer = unsafe.arrayBaseOffset(GetType(Long()))
		Private Shared ReadOnly shift As Integer
		Private ReadOnly array As Long()

		Shared Sub New()
			Dim scale As Integer = unsafe.arrayIndexScale(GetType(Long()))
			If (scale And (scale - 1)) <> 0 Then Throw New [Error]("data type scale not a power of two")
			shift = 31 - Integer.numberOfLeadingZeros(scale)
		End Sub

		Private Function checkedByteOffset(ByVal i As Integer) As Long
			If i < 0 OrElse i >= array.Length Then Throw New IndexOutOfBoundsException("index " & i)

			Return byteOffset(i)
		End Function

		Private Shared Function byteOffset(ByVal i As Integer) As Long
			Return (CLng(i) << shift) + base
		End Function

		''' <summary>
		''' Creates a new AtomicLongArray of the given length, with all
		''' elements initially zero.
		''' </summary>
		''' <param name="length"> the length of the array </param>
		Public Sub New(ByVal length As Integer)
			array = New Long(length - 1){}
		End Sub

		''' <summary>
		''' Creates a new AtomicLongArray with the same length as, and
		''' all elements copied from, the given array.
		''' </summary>
		''' <param name="array"> the array to copy elements from </param>
		''' <exception cref="NullPointerException"> if array is null </exception>
		Public Sub New(ByVal array As Long())
			' Visibility guaranteed by final field guarantees
			Me.array = array.clone()
		End Sub

		''' <summary>
		''' Returns the length of the array.
		''' </summary>
		''' <returns> the length of the array </returns>
		Public Function length() As Integer
			Return array.Length
		End Function

		''' <summary>
		''' Gets the current value at position {@code i}.
		''' </summary>
		''' <param name="i"> the index </param>
		''' <returns> the current value </returns>
		Public Function [get](ByVal i As Integer) As Long
			Return getRaw(checkedByteOffset(i))
		End Function

		Private Function getRaw(ByVal offset As Long) As Long
			Return unsafe.getLongVolatile(array, offset)
		End Function

		''' <summary>
		''' Sets the element at position {@code i} to the given value.
		''' </summary>
		''' <param name="i"> the index </param>
		''' <param name="newValue"> the new value </param>
		Public Sub [set](ByVal i As Integer, ByVal newValue As Long)
			unsafe.putLongVolatile(array, checkedByteOffset(i), newValue)
		End Sub

		''' <summary>
		''' Eventually sets the element at position {@code i} to the given value.
		''' </summary>
		''' <param name="i"> the index </param>
		''' <param name="newValue"> the new value
		''' @since 1.6 </param>
		Public Sub lazySet(ByVal i As Integer, ByVal newValue As Long)
			unsafe.putOrderedLong(array, checkedByteOffset(i), newValue)
		End Sub

		''' <summary>
		''' Atomically sets the element at position {@code i} to the given value
		''' and returns the old value.
		''' </summary>
		''' <param name="i"> the index </param>
		''' <param name="newValue"> the new value </param>
		''' <returns> the previous value </returns>
		Public Function getAndSet(ByVal i As Integer, ByVal newValue As Long) As Long
			Return unsafe.getAndSetLong(array, checkedByteOffset(i), newValue)
		End Function

		''' <summary>
		''' Atomically sets the element at position {@code i} to the given
		''' updated value if the current value {@code ==} the expected value.
		''' </summary>
		''' <param name="i"> the index </param>
		''' <param name="expect"> the expected value </param>
		''' <param name="update"> the new value </param>
		''' <returns> {@code true} if successful. False return indicates that
		''' the actual value was not equal to the expected value. </returns>
		Public Function compareAndSet(ByVal i As Integer, ByVal expect As Long, ByVal update As Long) As Boolean
			Return compareAndSetRaw(checkedByteOffset(i), expect, update)
		End Function

		Private Function compareAndSetRaw(ByVal offset As Long, ByVal expect As Long, ByVal update As Long) As Boolean
			Return unsafe.compareAndSwapLong(array, offset, expect, update)
		End Function

		''' <summary>
		''' Atomically sets the element at position {@code i} to the given
		''' updated value if the current value {@code ==} the expected value.
		''' 
		''' <p><a href="package-summary.html#weakCompareAndSet">May fail
		''' spuriously and does not provide ordering guarantees</a>, so is
		''' only rarely an appropriate alternative to {@code compareAndSet}.
		''' </summary>
		''' <param name="i"> the index </param>
		''' <param name="expect"> the expected value </param>
		''' <param name="update"> the new value </param>
		''' <returns> {@code true} if successful </returns>
		Public Function weakCompareAndSet(ByVal i As Integer, ByVal expect As Long, ByVal update As Long) As Boolean
			Return compareAndSet(i, expect, update)
		End Function

		''' <summary>
		''' Atomically increments by one the element at index {@code i}.
		''' </summary>
		''' <param name="i"> the index </param>
		''' <returns> the previous value </returns>
		Public Function getAndIncrement(ByVal i As Integer) As Long
			Return getAndAdd(i, 1)
		End Function

		''' <summary>
		''' Atomically decrements by one the element at index {@code i}.
		''' </summary>
		''' <param name="i"> the index </param>
		''' <returns> the previous value </returns>
		Public Function getAndDecrement(ByVal i As Integer) As Long
			Return getAndAdd(i, -1)
		End Function

		''' <summary>
		''' Atomically adds the given value to the element at index {@code i}.
		''' </summary>
		''' <param name="i"> the index </param>
		''' <param name="delta"> the value to add </param>
		''' <returns> the previous value </returns>
		Public Function getAndAdd(ByVal i As Integer, ByVal delta As Long) As Long
			Return unsafe.getAndAddLong(array, checkedByteOffset(i), delta)
		End Function

		''' <summary>
		''' Atomically increments by one the element at index {@code i}.
		''' </summary>
		''' <param name="i"> the index </param>
		''' <returns> the updated value </returns>
		Public Function incrementAndGet(ByVal i As Integer) As Long
			Return getAndAdd(i, 1) + 1
		End Function

		''' <summary>
		''' Atomically decrements by one the element at index {@code i}.
		''' </summary>
		''' <param name="i"> the index </param>
		''' <returns> the updated value </returns>
		Public Function decrementAndGet(ByVal i As Integer) As Long
			Return getAndAdd(i, -1) - 1
		End Function

		''' <summary>
		''' Atomically adds the given value to the element at index {@code i}.
		''' </summary>
		''' <param name="i"> the index </param>
		''' <param name="delta"> the value to add </param>
		''' <returns> the updated value </returns>
		Public Overridable Function addAndGet(ByVal i As Integer, ByVal delta As Long) As Long
			Return getAndAdd(i, delta) + delta
		End Function

		''' <summary>
		''' Atomically updates the element at index {@code i} with the results
		''' of applying the given function, returning the previous value. The
		''' function should be side-effect-free, since it may be re-applied
		''' when attempted updates fail due to contention among threads.
		''' </summary>
		''' <param name="i"> the index </param>
		''' <param name="updateFunction"> a side-effect-free function </param>
		''' <returns> the previous value
		''' @since 1.8 </returns>
		Public Function getAndUpdate(ByVal i As Integer, ByVal updateFunction As java.util.function.LongUnaryOperator) As Long
			Dim offset As Long = checkedByteOffset(i)
			Dim prev, [next] As Long
			Do
				prev = getRaw(offset)
				[next] = updateFunction.applyAsLong(prev)
			Loop While Not compareAndSetRaw(offset, prev, [next])
			Return prev
		End Function

		''' <summary>
		''' Atomically updates the element at index {@code i} with the results
		''' of applying the given function, returning the updated value. The
		''' function should be side-effect-free, since it may be re-applied
		''' when attempted updates fail due to contention among threads.
		''' </summary>
		''' <param name="i"> the index </param>
		''' <param name="updateFunction"> a side-effect-free function </param>
		''' <returns> the updated value
		''' @since 1.8 </returns>
		Public Function updateAndGet(ByVal i As Integer, ByVal updateFunction As java.util.function.LongUnaryOperator) As Long
			Dim offset As Long = checkedByteOffset(i)
			Dim prev, [next] As Long
			Do
				prev = getRaw(offset)
				[next] = updateFunction.applyAsLong(prev)
			Loop While Not compareAndSetRaw(offset, prev, [next])
			Return [next]
		End Function

		''' <summary>
		''' Atomically updates the element at index {@code i} with the
		''' results of applying the given function to the current and
		''' given values, returning the previous value. The function should
		''' be side-effect-free, since it may be re-applied when attempted
		''' updates fail due to contention among threads.  The function is
		''' applied with the current value at index {@code i} as its first
		''' argument, and the given update as the second argument.
		''' </summary>
		''' <param name="i"> the index </param>
		''' <param name="x"> the update value </param>
		''' <param name="accumulatorFunction"> a side-effect-free function of two arguments </param>
		''' <returns> the previous value
		''' @since 1.8 </returns>
		Public Function getAndAccumulate(ByVal i As Integer, ByVal x As Long, ByVal accumulatorFunction As java.util.function.LongBinaryOperator) As Long
			Dim offset As Long = checkedByteOffset(i)
			Dim prev, [next] As Long
			Do
				prev = getRaw(offset)
				[next] = accumulatorFunction.applyAsLong(prev, x)
			Loop While Not compareAndSetRaw(offset, prev, [next])
			Return prev
		End Function

		''' <summary>
		''' Atomically updates the element at index {@code i} with the
		''' results of applying the given function to the current and
		''' given values, returning the updated value. The function should
		''' be side-effect-free, since it may be re-applied when attempted
		''' updates fail due to contention among threads.  The function is
		''' applied with the current value at index {@code i} as its first
		''' argument, and the given update as the second argument.
		''' </summary>
		''' <param name="i"> the index </param>
		''' <param name="x"> the update value </param>
		''' <param name="accumulatorFunction"> a side-effect-free function of two arguments </param>
		''' <returns> the updated value
		''' @since 1.8 </returns>
		Public Function accumulateAndGet(ByVal i As Integer, ByVal x As Long, ByVal accumulatorFunction As java.util.function.LongBinaryOperator) As Long
			Dim offset As Long = checkedByteOffset(i)
			Dim prev, [next] As Long
			Do
				prev = getRaw(offset)
				[next] = accumulatorFunction.applyAsLong(prev, x)
			Loop While Not compareAndSetRaw(offset, prev, [next])
			Return [next]
		End Function

		''' <summary>
		''' Returns the String representation of the current values of array. </summary>
		''' <returns> the String representation of the current values of array </returns>
		Public Overrides Function ToString() As String
			Dim iMax As Integer = array.Length - 1
			If iMax = -1 Then Return "[]"

			Dim b As New StringBuilder
			b.append("["c)
			Dim i As Integer = 0
			Do
				b.append(getRaw(byteOffset(i)))
				If i = iMax Then Return b.append("]"c).ToString()
				b.append(","c).append(" "c)
				i += 1
			Loop
		End Function

	End Class

End Namespace