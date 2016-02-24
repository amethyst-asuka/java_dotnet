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
	''' An object reference that may be updated atomically. See the {@link
	''' java.util.concurrent.atomic} package specification for description
	''' of the properties of atomic variables.
	''' @since 1.5
	''' @author Doug Lea </summary>
	''' @param <V> The type of object referred to by this reference </param>
	<Serializable> _
	Public Class AtomicReference(Of V)
		Private Const serialVersionUID As Long = -1848883965231344442L

		Private Shared ReadOnly unsafe As sun.misc.Unsafe = sun.misc.Unsafe.unsafe
		Private Shared ReadOnly valueOffset As Long

		Shared Sub New()
			Try
				valueOffset = unsafe.objectFieldOffset(GetType(AtomicReference).getDeclaredField("value"))
			Catch ex As Exception
				Throw New [Error](ex)
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private value As V

		''' <summary>
		''' Creates a new AtomicReference with the given initial value.
		''' </summary>
		''' <param name="initialValue"> the initial value </param>
		Public Sub New(ByVal initialValue As V)
			value = initialValue
		End Sub

		''' <summary>
		''' Creates a new AtomicReference with null initial value.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Gets the current value.
		''' </summary>
		''' <returns> the current value </returns>
		Public Function [get]() As V
			Return value
		End Function

		''' <summary>
		''' Sets to the given value.
		''' </summary>
		''' <param name="newValue"> the new value </param>
		Public Sub [set](ByVal newValue As V)
			value = newValue
		End Sub

		''' <summary>
		''' Eventually sets to the given value.
		''' </summary>
		''' <param name="newValue"> the new value
		''' @since 1.6 </param>
		Public Sub lazySet(ByVal newValue As V)
			unsafe.putOrderedObject(Me, valueOffset, newValue)
		End Sub

		''' <summary>
		''' Atomically sets the value to the given updated value
		''' if the current value {@code ==} the expected value. </summary>
		''' <param name="expect"> the expected value </param>
		''' <param name="update"> the new value </param>
		''' <returns> {@code true} if successful. False return indicates that
		''' the actual value was not equal to the expected value. </returns>
		Public Function compareAndSet(ByVal expect As V, ByVal update As V) As Boolean
			Return unsafe.compareAndSwapObject(Me, valueOffset, expect, update)
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
		Public Function weakCompareAndSet(ByVal expect As V, ByVal update As V) As Boolean
			Return unsafe.compareAndSwapObject(Me, valueOffset, expect, update)
		End Function

		''' <summary>
		''' Atomically sets to the given value and returns the old value.
		''' </summary>
		''' <param name="newValue"> the new value </param>
		''' <returns> the previous value </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Function getAndSet(ByVal newValue As V) As V
			Return CType(unsafe.getAndSetObject(Me, valueOffset, newValue), V)
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
		Public Function getAndUpdate(ByVal updateFunction As java.util.function.UnaryOperator(Of V)) As V
			Dim prev, [next] As V
			Do
				prev = [get]()
				[next] = updateFunction.apply(prev)
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
		Public Function updateAndGet(ByVal updateFunction As java.util.function.UnaryOperator(Of V)) As V
			Dim prev, [next] As V
			Do
				prev = [get]()
				[next] = updateFunction.apply(prev)
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
		Public Function getAndAccumulate(ByVal x As V, ByVal accumulatorFunction As java.util.function.BinaryOperator(Of V)) As V
			Dim prev, [next] As V
			Do
				prev = [get]()
				[next] = accumulatorFunction.apply(prev, x)
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
		Public Function accumulateAndGet(ByVal x As V, ByVal accumulatorFunction As java.util.function.BinaryOperator(Of V)) As V
			Dim prev, [next] As V
			Do
				prev = [get]()
				[next] = accumulatorFunction.apply(prev, x)
			Loop While Not compareAndSet(prev, [next])
			Return [next]
		End Function

		''' <summary>
		''' Returns the String representation of the current value. </summary>
		''' <returns> the String representation of the current value </returns>
		Public Overrides Function ToString() As String
			Return Convert.ToString([get]())
		End Function

	End Class

End Namespace