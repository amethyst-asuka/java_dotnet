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
	''' An array of object references in which elements may be updated
	''' atomically.  See the <seealso cref="java.util.concurrent.atomic"/> package
	''' specification for description of the properties of atomic
	''' variables.
	''' @since 1.5
	''' @author Doug Lea </summary>
	''' @param <E> The base class of elements held in this array </param>
	<Serializable> _
	Public Class AtomicReferenceArray(Of E)
		Private Const serialVersionUID As Long = -6209656149925076980L

		Private Shared ReadOnly unsafe As sun.misc.Unsafe
		Private Shared ReadOnly base As Integer
		Private Shared ReadOnly shift As Integer
		Private Shared ReadOnly arrayFieldOffset As Long
		Private ReadOnly array As Object() ' must have exact type Object[]

		Shared Sub New()
			Try
				unsafe = sun.misc.Unsafe.unsafe
				arrayFieldOffset = unsafe.objectFieldOffset(GetType(AtomicReferenceArray).getDeclaredField("array"))
				base = unsafe.arrayBaseOffset(GetType(Object()))
				Dim scale As Integer = unsafe.arrayIndexScale(GetType(Object()))
				If (scale And (scale - 1)) <> 0 Then Throw New [Error]("data type scale not a power of two")
				shift = 31 -  java.lang.[Integer].numberOfLeadingZeros(scale)
			Catch e As Exception
				Throw New [Error](e)
			End Try
		End Sub

		Private Function checkedByteOffset(  i As Integer) As Long
			If i < 0 OrElse i >= array.Length Then Throw New IndexOutOfBoundsException("index " & i)

			Return byteOffset(i)
		End Function

		Private Shared Function byteOffset(  i As Integer) As Long
			Return (CLng(i) << shift) + base
		End Function

		''' <summary>
		''' Creates a new AtomicReferenceArray of the given length, with all
		''' elements initially null.
		''' </summary>
		''' <param name="length"> the length of the array </param>
		Public Sub New(  length As Integer)
			array = New Object(length - 1){}
		End Sub

		''' <summary>
		''' Creates a new AtomicReferenceArray with the same length as, and
		''' all elements copied from, the given array.
		''' </summary>
		''' <param name="array"> the array to copy elements from </param>
		''' <exception cref="NullPointerException"> if array is null </exception>
		Public Sub New(  array As E())
			' Visibility guaranteed by final field guarantees
			Me.array = java.util.Arrays.copyOf(array, array.Length, GetType(Object()))
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
		Public Function [get](  i As Integer) As E
			Return getRaw(checkedByteOffset(i))
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Function getRaw(  offset As Long) As E
			Return CType(unsafe.getObjectVolatile(array, offset), E)
		End Function

		''' <summary>
		''' Sets the element at position {@code i} to the given value.
		''' </summary>
		''' <param name="i"> the index </param>
		''' <param name="newValue"> the new value </param>
		Public Sub [set](  i As Integer,   newValue As E)
			unsafe.putObjectVolatile(array, checkedByteOffset(i), newValue)
		End Sub

		''' <summary>
		''' Eventually sets the element at position {@code i} to the given value.
		''' </summary>
		''' <param name="i"> the index </param>
		''' <param name="newValue"> the new value
		''' @since 1.6 </param>
		Public Sub lazySet(  i As Integer,   newValue As E)
			unsafe.putOrderedObject(array, checkedByteOffset(i), newValue)
		End Sub

		''' <summary>
		''' Atomically sets the element at position {@code i} to the given
		''' value and returns the old value.
		''' </summary>
		''' <param name="i"> the index </param>
		''' <param name="newValue"> the new value </param>
		''' <returns> the previous value </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Function getAndSet(  i As Integer,   newValue As E) As E
			Return CType(unsafe.getAndSetObject(array, checkedByteOffset(i), newValue), E)
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
		Public Function compareAndSet(  i As Integer,   expect As E,   update As E) As Boolean
			Return compareAndSetRaw(checkedByteOffset(i), expect, update)
		End Function

		Private Function compareAndSetRaw(  offset As Long,   expect As E,   update As E) As Boolean
			Return unsafe.compareAndSwapObject(array, offset, expect, update)
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
		Public Function weakCompareAndSet(  i As Integer,   expect As E,   update As E) As Boolean
			Return compareAndSet(i, expect, update)
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
		Public Function getAndUpdate(  i As Integer,   updateFunction As java.util.function.UnaryOperator(Of E)) As E
			Dim offset As Long = checkedByteOffset(i)
			Dim prev, [next] As E
			Do
				prev = getRaw(offset)
				[next] = updateFunction.apply(prev)
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
		Public Function updateAndGet(  i As Integer,   updateFunction As java.util.function.UnaryOperator(Of E)) As E
			Dim offset As Long = checkedByteOffset(i)
			Dim prev, [next] As E
			Do
				prev = getRaw(offset)
				[next] = updateFunction.apply(prev)
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
		Public Function getAndAccumulate(  i As Integer,   x As E,   accumulatorFunction As java.util.function.BinaryOperator(Of E)) As E
			Dim offset As Long = checkedByteOffset(i)
			Dim prev, [next] As E
			Do
				prev = getRaw(offset)
				[next] = accumulatorFunction.apply(prev, x)
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
		Public Function accumulateAndGet(  i As Integer,   x As E,   accumulatorFunction As java.util.function.BinaryOperator(Of E)) As E
			Dim offset As Long = checkedByteOffset(i)
			Dim prev, [next] As E
			Do
				prev = getRaw(offset)
				[next] = accumulatorFunction.apply(prev, x)
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

		''' <summary>
		''' Reconstitutes the instance from a stream (that is, deserializes it).
		''' </summary>
		Private Sub readObject(  s As java.io.ObjectInputStream)
			' Note: This must be changed if any additional fields are defined
			Dim a As Object = s.readFields().get("array", Nothing)
			If a Is Nothing OrElse (Not a.GetType().IsArray) Then Throw New java.io.InvalidObjectException("Not array type")
			If a.GetType() IsNot GetType(Object()) Then a = java.util.Arrays.copyOf(CType(a, Object()), Array.getLength(a), GetType(Object()))
			unsafe.putObjectVolatile(Me, arrayFieldOffset, a)
		End Sub

	End Class

End Namespace