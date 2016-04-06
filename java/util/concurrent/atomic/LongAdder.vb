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
	''' One or more variables that together maintain an initially zero
	''' {@code long} sum.  When updates (method <seealso cref="#add"/>) are contended
	''' across threads, the set of variables may grow dynamically to reduce
	''' contention. Method <seealso cref="#sum"/> (or, equivalently, {@link
	''' #longValue}) returns the current total combined across the
	''' variables maintaining the sum.
	''' 
	''' <p>This class is usually preferable to <seealso cref="AtomicLong"/> when
	''' multiple threads update a common sum that is used for purposes such
	''' as collecting statistics, not for fine-grained synchronization
	''' control.  Under low update contention, the two classes have similar
	''' characteristics. But under high contention, expected throughput of
	''' this class is significantly higher, at the expense of higher space
	''' consumption.
	''' 
	''' <p>LongAdders can be used with a {@link
	''' java.util.concurrent.ConcurrentHashMap} to maintain a scalable
	''' frequency map (a form of histogram or multiset). For example, to
	''' add a count to a {@code ConcurrentHashMap<String,LongAdder> freqs},
	''' initializing if not already present, you can use {@code
	''' freqs.computeIfAbsent(k -> new LongAdder()).increment();}
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
	Public Class LongAdder
		Inherits Striped64

		Private Const serialVersionUID As Long = 7249069246863182397L

		''' <summary>
		''' Creates a new adder with initial sum of zero.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Adds the given value.
		''' </summary>
		''' <param name="x"> the value to add </param>
		Public Overridable Sub add(  x As Long)
			Dim [as] As Cell()
			Dim b, v As Long
			Dim m As Integer
			Dim a As Cell
			[as] = cells
			b = base, b + x
			If [as] IsNot Nothing OrElse (Not casBaseb) Then
				Dim uncontended As Boolean = True
				m = [as].Length - 1
				a = [as](probe And m)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				uncontended = a.cas(v = a.value, v + x)
				If [as] Is Nothing OrElse m < 0 OrElse a Is Nothing OrElse (Not uncontended) Then longAccumulate(x, Nothing, uncontended)
			End If
		End Sub

		''' <summary>
		''' Equivalent to {@code add(1)}.
		''' </summary>
		Public Overridable Sub increment()
			add(1L)
		End Sub

		''' <summary>
		''' Equivalent to {@code add(-1)}.
		''' </summary>
		Public Overridable Sub decrement()
			add(-1L)
		End Sub

		''' <summary>
		''' Returns the current sum.  The returned value is <em>NOT</em> an
		''' atomic snapshot; invocation in the absence of concurrent
		''' updates returns an accurate result, but concurrent updates that
		''' occur while the sum is being calculated might not be
		''' incorporated.
		''' </summary>
		''' <returns> the sum </returns>
		Public Overridable Function sum() As Long
			Dim [as] As Cell() = cells
			Dim a As Cell
			Dim sum_Renamed As Long = base
			If [as] IsNot Nothing Then
				For i As Integer = 0 To [as].Length - 1
					a = [as](i)
					If a IsNot Nothing Then sum_Renamed += a.value
				Next i
			End If
			Return sum_Renamed
		End Function

		''' <summary>
		''' Resets variables maintaining the sum to zero.  This method may
		''' be a useful alternative to creating a new adder, but is only
		''' effective if there are no concurrent updates.  Because this
		''' method is intrinsically racy, it should only be used when it is
		''' known that no threads are concurrently updating.
		''' </summary>
		Public Overridable Sub reset()
			Dim [as] As Cell() = cells
			Dim a As Cell
			base = 0L
			If [as] IsNot Nothing Then
				For i As Integer = 0 To [as].Length - 1
					a = [as](i)
					If a IsNot Nothing Then a.value = 0L
				Next i
			End If
		End Sub

		''' <summary>
		''' Equivalent in effect to <seealso cref="#sum"/> followed by {@link
		''' #reset}. This method may apply for example during quiescent
		''' points between multithreaded computations.  If there are
		''' updates concurrent with this method, the returned value is
		''' <em>not</em> guaranteed to be the final value occurring before
		''' the reset.
		''' </summary>
		''' <returns> the sum </returns>
		Public Overridable Function sumThenReset() As Long
			Dim [as] As Cell() = cells
			Dim a As Cell
			Dim sum As Long = base
			base = 0L
			If [as] IsNot Nothing Then
				For i As Integer = 0 To [as].Length - 1
					a = [as](i)
					If a IsNot Nothing Then
						sum += a.value
						a.value = 0L
					End If
				Next i
			End If
			Return sum
		End Function

		''' <summary>
		''' Returns the String representation of the <seealso cref="#sum"/>. </summary>
		''' <returns> the String representation of the <seealso cref="#sum"/> </returns>
		Public Overrides Function ToString() As String
			Return Convert.ToString(sum())
		End Function

		''' <summary>
		''' Equivalent to <seealso cref="#sum"/>.
		''' </summary>
		''' <returns> the sum </returns>
		Public Overrides Function longValue() As Long
			Return sum()
		End Function

		''' <summary>
		''' Returns the <seealso cref="#sum"/> as an {@code int} after a narrowing
		''' primitive conversion.
		''' </summary>
		Public Overrides Function intValue() As Integer
			Return CInt(sum())
		End Function

		''' <summary>
		''' Returns the <seealso cref="#sum"/> as a {@code float}
		''' after a widening primitive conversion.
		''' </summary>
		Public Overrides Function floatValue() As Single
			Return CSng(sum())
		End Function

		''' <summary>
		''' Returns the <seealso cref="#sum"/> as a {@code double} after a widening
		''' primitive conversion.
		''' </summary>
		Public Overrides Function doubleValue() As Double
			Return CDbl(sum())
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
			''' The current value returned by sum().
			''' @serial
			''' </summary>
			Private ReadOnly value As Long

			Friend Sub New(  a As LongAdder)
				value = a.sum()
			End Sub

			''' <summary>
			''' Return a {@code LongAdder} object with initial state
			''' held by this proxy.
			''' </summary>
			''' <returns> a {@code LongAdder} object with initial state
			''' held by this proxy. </returns>
			Private Function readResolve() As Object
				Dim a As New LongAdder
				a.base = value
				Return a
			End Function
		End Class

		''' <summary>
		''' Returns a
		''' <a href="../../../../serialized-form.html#java.util.concurrent.atomic.LongAdder.SerializationProxy">
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
		Private Sub readObject(  s As java.io.ObjectInputStream)
			Throw New java.io.InvalidObjectException("Proxy required")
		End Sub

	End Class

End Namespace