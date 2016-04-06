Imports Microsoft.VisualBasic
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
	''' {@code double} sum.  When updates (method <seealso cref="#add"/>) are
	''' contended across threads, the set of variables may grow dynamically
	''' to reduce contention.  Method <seealso cref="#sum"/> (or, equivalently {@link
	''' #doubleValue}) returns the current total combined across the
	''' variables maintaining the sum. The order of accumulation within or
	''' across threads is not guaranteed. Thus, this class may not be
	''' applicable if numerical stability is required, especially when
	''' combining values of substantially different orders of magnitude.
	''' 
	''' <p>This class is usually preferable to alternatives when multiple
	''' threads update a common value that is used for purposes such as
	''' summary statistics that are frequently updated but less frequently
	''' read.
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
	Public Class DoubleAdder
		Inherits Striped64

		Private Const serialVersionUID As Long = 7249069246863182397L

	'    
	'     * Note that we must use "long" for underlying representations,
	'     * because there is no compareAndSet for double, due to the fact
	'     * that the bitwise equals used in any CAS implementation is not
	'     * the same as double-precision equals.  However, we use CAS only
	'     * to detect and alleviate contention, for which bitwise equals
	'     * works best anyway. In principle, the long/double conversions
	'     * used here should be essentially free on most platforms since
	'     * they just re-interpret bits.
	'     

		''' <summary>
		''' Creates a new adder with initial sum of zero.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Adds the given value.
		''' </summary>
		''' <param name="x"> the value to add </param>
		Public Overridable Sub add(  x As Double)
			Dim [as] As Cell()
			Dim b, v As Long
			Dim m As Integer
			Dim a As Cell
			[as] = cells
			b = base, java.lang.[Double].doubleToRawLongBits(Double.longBitsToDouble(b) + x)
			If [as] IsNot Nothing OrElse (Not casBaseb) Then
				Dim uncontended As Boolean = True
				m = [as].Length - 1
				a = [as](probe And m)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				uncontended = a.cas(v = a.value, java.lang.[Double].doubleToRawLongBits(Double.longBitsToDouble(v) + x))
				If [as] Is Nothing OrElse m < 0 OrElse a Is Nothing OrElse (Not uncontended) Then doubleAccumulate(x, Nothing, uncontended)
			End If
		End Sub

		''' <summary>
		''' Returns the current sum.  The returned value is <em>NOT</em> an
		''' atomic snapshot; invocation in the absence of concurrent
		''' updates returns an accurate result, but concurrent updates that
		''' occur while the sum is being calculated might not be
		''' incorporated.  Also, because floating-point arithmetic is not
		''' strictly associative, the returned result need not be identical
		''' to the value that would be obtained in a sequential series of
		''' updates to a single variable.
		''' </summary>
		''' <returns> the sum </returns>
		Public Overridable Function sum() As Double
			Dim [as] As Cell() = cells
			Dim a As Cell
			Dim sum_Renamed As Double = java.lang.[Double].longBitsToDouble(base)
			If [as] IsNot Nothing Then
				For i As Integer = 0 To [as].Length - 1
					a = [as](i)
					If a IsNot Nothing Then sum_Renamed += java.lang.[Double].longBitsToDouble(a.value)
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
			base = 0L ' relies on fact that double 0 must have same rep as long
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
		Public Overridable Function sumThenReset() As Double
			Dim [as] As Cell() = cells
			Dim a As Cell
			Dim sum As Double = java.lang.[Double].longBitsToDouble(base)
			base = 0L
			If [as] IsNot Nothing Then
				For i As Integer = 0 To [as].Length - 1
					a = [as](i)
					If a IsNot Nothing Then
						Dim v As Long = a.value
						a.value = 0L
						sum += java.lang.[Double].longBitsToDouble(v)
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
		Public Overrides Function doubleValue() As Double
			Return sum()
		End Function

		''' <summary>
		''' Returns the <seealso cref="#sum"/> as a {@code long} after a
		''' narrowing primitive conversion.
		''' </summary>
		Public Overrides Function longValue() As Long
			Return CLng(Fix(sum()))
		End Function

		''' <summary>
		''' Returns the <seealso cref="#sum"/> as an {@code int} after a
		''' narrowing primitive conversion.
		''' </summary>
		Public Overrides Function intValue() As Integer
			Return CInt(Fix(sum()))
		End Function

		''' <summary>
		''' Returns the <seealso cref="#sum"/> as a {@code float}
		''' after a narrowing primitive conversion.
		''' </summary>
		Public Overrides Function floatValue() As Single
			Return CSng(sum())
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
			Private ReadOnly value As Double

			Friend Sub New(  a As DoubleAdder)
				value = a.sum()
			End Sub

			''' <summary>
			''' Returns a {@code DoubleAdder} object with initial state
			''' held by this proxy.
			''' </summary>
			''' <returns> a {@code DoubleAdder} object with initial state
			''' held by this proxy. </returns>
			Private Function readResolve() As Object
				Dim a As New DoubleAdder
				a.base = java.lang.[Double].doubleToRawLongBits(value)
				Return a
			End Function
		End Class

		''' <summary>
		''' Returns a
		''' <a href="../../../../serialized-form.html#java.util.concurrent.atomic.DoubleAdder.SerializationProxy">
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