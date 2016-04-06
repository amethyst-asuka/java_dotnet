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
	''' A {@code boolean} value that may be updated atomically. See the
	''' <seealso cref="java.util.concurrent.atomic"/> package specification for
	''' description of the properties of atomic variables. An
	''' {@code AtomicBoolean} is used in applications such as atomically
	''' updated flags, and cannot be used as a replacement for a
	''' <seealso cref="java.lang.Boolean"/>.
	''' 
	''' @since 1.5
	''' @author Doug Lea
	''' </summary>
	<Serializable> _
	Public Class AtomicBoolean
		Private Const serialVersionUID As Long = 4654671469794556979L
		' setup to use Unsafe.compareAndSwapInt for updates
		Private Shared ReadOnly unsafe As sun.misc.Unsafe = sun.misc.Unsafe.unsafe
		Private Shared ReadOnly valueOffset As Long

		Shared Sub New()
			Try
				valueOffset = unsafe.objectFieldOffset(GetType(AtomicBoolean).getDeclaredField("value"))
			Catch ex As Exception
				Throw New [Error](ex)
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private value As Integer

		''' <summary>
		''' Creates a new {@code AtomicBoolean} with the given initial value.
		''' </summary>
		''' <param name="initialValue"> the initial value </param>
		Public Sub New(  initialValue As Boolean)
			value = If(initialValue, 1, 0)
		End Sub

		''' <summary>
		''' Creates a new {@code AtomicBoolean} with initial value {@code false}.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Returns the current value.
		''' </summary>
		''' <returns> the current value </returns>
		Public Function [get]() As Boolean
			Return value <> 0
		End Function

		''' <summary>
		''' Atomically sets the value to the given updated value
		''' if the current value {@code ==} the expected value.
		''' </summary>
		''' <param name="expect"> the expected value </param>
		''' <param name="update"> the new value </param>
		''' <returns> {@code true} if successful. False return indicates that
		''' the actual value was not equal to the expected value. </returns>
		Public Function compareAndSet(  expect As Boolean,   update As Boolean) As Boolean
			Dim e As Integer = If(expect, 1, 0)
			Dim u As Integer = If(update, 1, 0)
			Return unsafe.compareAndSwapInt(Me, valueOffset, e, u)
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
		Public Overridable Function weakCompareAndSet(  expect As Boolean,   update As Boolean) As Boolean
			Dim e As Integer = If(expect, 1, 0)
			Dim u As Integer = If(update, 1, 0)
			Return unsafe.compareAndSwapInt(Me, valueOffset, e, u)
		End Function

		''' <summary>
		''' Unconditionally sets to the given value.
		''' </summary>
		''' <param name="newValue"> the new value </param>
		Public Sub [set](  newValue As Boolean)
			value = If(newValue, 1, 0)
		End Sub

		''' <summary>
		''' Eventually sets to the given value.
		''' </summary>
		''' <param name="newValue"> the new value
		''' @since 1.6 </param>
		Public Sub lazySet(  newValue As Boolean)
			Dim v As Integer = If(newValue, 1, 0)
			unsafe.putOrderedInt(Me, valueOffset, v)
		End Sub

		''' <summary>
		''' Atomically sets to the given value and returns the previous value.
		''' </summary>
		''' <param name="newValue"> the new value </param>
		''' <returns> the previous value </returns>
		Public Function getAndSet(  newValue As Boolean) As Boolean
			Dim prev As Boolean
			Do
				prev = [get]()
			Loop While Not compareAndSet(prev, newValue)
			Return prev
		End Function

		''' <summary>
		''' Returns the String representation of the current value. </summary>
		''' <returns> the String representation of the current value </returns>
		Public Overrides Function ToString() As String
			Return Convert.ToString([get]())
		End Function

	End Class

End Namespace