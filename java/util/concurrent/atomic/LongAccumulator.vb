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
	''' One or more variables that together maintain a running {@code long}
	''' value updated using a supplied function.  When updates (method
	''' <seealso cref="#accumulate"/>) are contended across threads, the set of variables
	''' may grow dynamically to reduce contention.  Method <seealso cref="#get"/>
	''' (or, equivalently, <seealso cref="#longValue"/>) returns the current value
	''' across the variables maintaining updates.
	''' 
	''' <p>This class is usually preferable to <seealso cref="AtomicLong"/> when
	''' multiple threads update a common value that is used for purposes such
	''' as collecting statistics, not for fine-grained synchronization
	''' control.  Under low update contention, the two classes have similar
	''' characteristics. But under high contention, expected throughput of
	''' this class is significantly higher, at the expense of higher space
	''' consumption.
	''' 
	''' <p>The order of accumulation within or across threads is not
	''' guaranteed and cannot be depended upon, so this class is only
	''' applicable to functions for which the order of accumulation does
	''' not matter. The supplied accumulator function should be
	''' side-effect-free, since it may be re-applied when attempted updates
	''' fail due to contention among threads. The function is applied with
	''' the current value as its first argument, and the given update as
	''' the second argument.  For example, to maintain a running maximum
	''' value, you could supply {@code Long::max} along with {@code
	''' java.lang.[Long].MIN_VALUE} as the identity.
	''' 
	''' <p>Class <seealso cref="LongAdder"/> provides analogs of the functionality of
	''' this class for the common special case of maintaining counts and
	''' sums.  The call {@code new LongAdder()} is equivalent to {@code new
	''' LongAccumulator((x, y) -> x + y, 0L}.
	''' 
	''' <p>This class extends <seealso cref="Number"/>, but does <em>not</em> define
	''' methods such as {@code equals}, {@code hashCode} and {@code
	''' compareTo} because instances are expected to be mutated, and so are
	''' not useful as collection keys.
	''' 
	''' @since 1.8
	''' @author Doug Lea
	''' </summary>
	<Serializable> _
	Public Class LongAccumulator
		Inherits Striped64

		Private Const serialVersionUID As Long = 7249069246863182397L

		Private ReadOnly [function] As java.util.function.LongBinaryOperator
		Private ReadOnly identity As Long

		''' <summary>
		''' Creates a new instance using the given accumulator function
		''' and identity element. </summary>
		''' <param name="accumulatorFunction"> a side-effect-free function of two arguments </param>
		''' <param name="identity"> identity (initial value) for the accumulator function </param>
		Public Sub New(ByVal accumulatorFunction As java.util.function.LongBinaryOperator, ByVal identity As Long)
			Me.function = accumulatorFunction
				Me.identity = identity
				base = Me.identity
		End Sub

		''' <summary>
		''' Updates with the given value.
		''' </summary>
		''' <param name="x"> the value </param>
		Public Overridable Sub accumulate(ByVal x As Long)
			Dim [as] As Cell()
			Dim b, v, r As Long
			Dim m As Integer
			Dim a As Cell
			[as] = cells
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			r = [function].applyAsLong(b = base, x)
			If [as] IsNot Nothing OrElse r <> b AndAlso (Not casBase(b, r)) Then
				Dim uncontended As Boolean = True
				m = [as].Length - 1
				a = [as](probe And m)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				uncontended = (r = [function].applyAsLong(v = a.value, x)) = v OrElse a.cas(v, r)
				If [as] Is Nothing OrElse m < 0 OrElse a Is Nothing OrElse (Not uncontended) Then longAccumulate(x, [function], uncontended)
			End If
		End Sub

		''' <summary>
		''' Returns the current value.  The returned value is <em>NOT</em>
		''' an atomic snapshot; invocation in the absence of concurrent
		''' updates returns an accurate result, but concurrent updates that
		''' occur while the value is being calculated might not be
		''' incorporated.
		''' </summary>
		''' <returns> the current value </returns>
		Public Overridable Function [get]() As Long
			Dim [as] As Cell() = cells
			Dim a As Cell
			Dim result As Long = base
			If [as] IsNot Nothing Then
				For i As Integer = 0 To [as].Length - 1
					a = [as](i)
					If a IsNot Nothing Then result = [function].applyAsLong(result, a.value)
				Next i
			End If
			Return result
		End Function

		''' <summary>
		''' Resets variables maintaining updates to the identity value.
		''' This method may be a useful alternative to creating a new
		''' updater, but is only effective if there are no concurrent
		''' updates.  Because this method is intrinsically racy, it should
		''' only be used when it is known that no threads are concurrently
		''' updating.
		''' </summary>
		Public Overridable Sub reset()
			Dim [as] As Cell() = cells
			Dim a As Cell
			base = identity
			If [as] IsNot Nothing Then
				For i As Integer = 0 To [as].Length - 1
					a = [as](i)
					If a IsNot Nothing Then a.value = identity
				Next i
			End If
		End Sub

		''' <summary>
		''' Equivalent in effect to <seealso cref="#get"/> followed by {@link
		''' #reset}. This method may apply for example during quiescent
		''' points between multithreaded computations.  If there are
		''' updates concurrent with this method, the returned value is
		''' <em>not</em> guaranteed to be the final value occurring before
		''' the reset.
		''' </summary>
		''' <returns> the value before reset </returns>
		Public Overridable Property thenReset As Long
			Get
				Dim [as] As Cell() = cells
				Dim a As Cell
				Dim result As Long = base
				base = identity
				If [as] IsNot Nothing Then
					For i As Integer = 0 To [as].Length - 1
						a = [as](i)
						If a IsNot Nothing Then
							Dim v As Long = a.value
							a.value = identity
							result = [function].applyAsLong(result, v)
						End If
					Next i
				End If
				Return result
			End Get
		End Property

		''' <summary>
		''' Returns the String representation of the current value. </summary>
		''' <returns> the String representation of the current value </returns>
		Public Overrides Function ToString() As String
			Return Convert.ToString([get]())
		End Function

		''' <summary>
		''' Equivalent to <seealso cref="#get"/>.
		''' </summary>
		''' <returns> the current value </returns>
		Public Overrides Function longValue() As Long
			Return [get]()
		End Function

		''' <summary>
		''' Returns the <seealso cref="#get current value"/> as an {@code int}
		''' after a narrowing primitive conversion.
		''' </summary>
		Public Overrides Function intValue() As Integer
			Return CInt([get]())
		End Function

		''' <summary>
		''' Returns the <seealso cref="#get current value"/> as a {@code float}
		''' after a widening primitive conversion.
		''' </summary>
		Public Overrides Function floatValue() As Single
			Return CSng([get]())
		End Function

		''' <summary>
		''' Returns the <seealso cref="#get current value"/> as a {@code double}
		''' after a widening primitive conversion.
		''' </summary>
		Public Overrides Function doubleValue() As Double
			Return CDbl([get]())
		End Function

		''' <summary>
		''' Serialization proxy, used to avoid reference to the non-public
		''' Striped64 superclass in serialized forms.
		''' @serial include
		''' </summary>
		<Serializable> _
		Private Class SerializationProxy
			Private Const serialVersionUID As Long = 7249069246863182397L

			''' <summary>
			''' The current value returned by get().
			''' @serial
			''' </summary>
			Private ReadOnly value As Long
			''' <summary>
			''' The function used for updates.
			''' @serial
			''' </summary>
			Private ReadOnly [function] As java.util.function.LongBinaryOperator
			''' <summary>
			''' The identity value
			''' @serial
			''' </summary>
			Private ReadOnly identity As Long

			Friend Sub New(ByVal a As LongAccumulator)
				[function] = a.function
				identity = a.identity
				value = a.get()
			End Sub

			''' <summary>
			''' Returns a {@code LongAccumulator} object with initial state
			''' held by this proxy.
			''' </summary>
			''' <returns> a {@code LongAccumulator} object with initial state
			''' held by this proxy. </returns>
			Private Function readResolve() As Object
				Dim a As New LongAccumulator([function], identity)
				a.base = value
				Return a
			End Function
		End Class

		''' <summary>
		''' Returns a
		''' <a href="../../../../serialized-form.html#java.util.concurrent.atomic.LongAccumulator.SerializationProxy">
		''' SerializationProxy</a>
		''' representing the state of this instance.
		''' </summary>
		''' <returns> a <seealso cref="SerializationProxy"/>
		''' representing the state of this instance </returns>
		Private Function writeReplace() As Object
			Return New SerializationProxy(Me)
		End Function

		''' <param name="s"> the stream </param>
		''' <exception cref="java.io.InvalidObjectException"> always </exception>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			Throw New java.io.InvalidObjectException("Proxy required")
		End Sub

	End Class

End Namespace