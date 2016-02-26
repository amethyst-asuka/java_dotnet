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
	''' An {@code AtomicStampedReference} maintains an object reference
	''' along with an integer "stamp", that can be updated atomically.
	''' 
	''' <p>Implementation note: This implementation maintains stamped
	''' references by creating internal objects representing "boxed"
	''' [reference, integer] pairs.
	''' 
	''' @since 1.5
	''' @author Doug Lea </summary>
	''' @param <V> The type of object referred to by this reference </param>
	Public Class AtomicStampedReference(Of V)

		Private Class Pair(Of T)
			Friend ReadOnly reference As T
			Friend ReadOnly stamp As Integer
			Private Sub New(ByVal reference As T, ByVal stamp As Integer)
				Me.reference = reference
				Me.stamp = stamp
			End Sub
			Shared Function [of](Of T)(ByVal reference As T, ByVal stamp As Integer) As Pair(Of T)
				Return New Pair(Of T)(reference, stamp)
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private pair_Renamed As Pair(Of V)

		''' <summary>
		''' Creates a new {@code AtomicStampedReference} with the given
		''' initial values.
		''' </summary>
		''' <param name="initialRef"> the initial reference </param>
		''' <param name="initialStamp"> the initial stamp </param>
		Public Sub New(ByVal initialRef As V, ByVal initialStamp As Integer)
			pair_Renamed = Pair.of(initialRef, initialStamp)
		End Sub

		''' <summary>
		''' Returns the current value of the reference.
		''' </summary>
		''' <returns> the current value of the reference </returns>
		Public Overridable Property reference As V
			Get
				Return pair_Renamed.reference
			End Get
		End Property

		''' <summary>
		''' Returns the current value of the stamp.
		''' </summary>
		''' <returns> the current value of the stamp </returns>
		Public Overridable Property stamp As Integer
			Get
				Return pair_Renamed.stamp
			End Get
		End Property

		''' <summary>
		''' Returns the current values of both the reference and the stamp.
		''' Typical usage is {@code int[1] holder; ref = v.get(holder); }.
		''' </summary>
		''' <param name="stampHolder"> an array of size of at least one.  On return,
		''' {@code stampholder[0]} will hold the value of the stamp. </param>
		''' <returns> the current value of the reference </returns>
		Public Overridable Function [get](ByVal stampHolder As Integer()) As V
			Dim pair_Renamed As Pair(Of V) = Me.pair_Renamed
			stampHolder(0) = pair_Renamed.stamp
			Return pair_Renamed.reference
		End Function

		''' <summary>
		''' Atomically sets the value of both the reference and stamp
		''' to the given update values if the
		''' current reference is {@code ==} to the expected reference
		''' and the current stamp is equal to the expected stamp.
		''' 
		''' <p><a href="package-summary.html#weakCompareAndSet">May fail
		''' spuriously and does not provide ordering guarantees</a>, so is
		''' only rarely an appropriate alternative to {@code compareAndSet}.
		''' </summary>
		''' <param name="expectedReference"> the expected value of the reference </param>
		''' <param name="newReference"> the new value for the reference </param>
		''' <param name="expectedStamp"> the expected value of the stamp </param>
		''' <param name="newStamp"> the new value for the stamp </param>
		''' <returns> {@code true} if successful </returns>
		Public Overridable Function weakCompareAndSet(ByVal expectedReference As V, ByVal newReference As V, ByVal expectedStamp As Integer, ByVal newStamp As Integer) As Boolean
			Return compareAndSet(expectedReference, newReference, expectedStamp, newStamp)
		End Function

		''' <summary>
		''' Atomically sets the value of both the reference and stamp
		''' to the given update values if the
		''' current reference is {@code ==} to the expected reference
		''' and the current stamp is equal to the expected stamp.
		''' </summary>
		''' <param name="expectedReference"> the expected value of the reference </param>
		''' <param name="newReference"> the new value for the reference </param>
		''' <param name="expectedStamp"> the expected value of the stamp </param>
		''' <param name="newStamp"> the new value for the stamp </param>
		''' <returns> {@code true} if successful </returns>
		Public Overridable Function compareAndSet(ByVal expectedReference As V, ByVal newReference As V, ByVal expectedStamp As Integer, ByVal newStamp As Integer) As Boolean
			Dim current As Pair(Of V) = pair_Renamed
			Return expectedReference Is current.reference AndAlso expectedStamp = current.stamp AndAlso ((newReference Is current.reference AndAlso newStamp = current.stamp) OrElse casPair(current, Pair.of(newReference, newStamp)))
		End Function

		''' <summary>
		''' Unconditionally sets the value of both the reference and stamp.
		''' </summary>
		''' <param name="newReference"> the new value for the reference </param>
		''' <param name="newStamp"> the new value for the stamp </param>
		Public Overridable Sub [set](ByVal newReference As V, ByVal newStamp As Integer)
			Dim current As Pair(Of V) = pair_Renamed
			If newReference IsNot current.reference OrElse newStamp <> current.stamp Then Me.pair_Renamed = Pair.of(newReference, newStamp)
		End Sub

		''' <summary>
		''' Atomically sets the value of the stamp to the given update value
		''' if the current reference is {@code ==} to the expected
		''' reference.  Any given invocation of this operation may fail
		''' (return {@code false}) spuriously, but repeated invocation
		''' when the current value holds the expected value and no other
		''' thread is also attempting to set the value will eventually
		''' succeed.
		''' </summary>
		''' <param name="expectedReference"> the expected value of the reference </param>
		''' <param name="newStamp"> the new value for the stamp </param>
		''' <returns> {@code true} if successful </returns>
		Public Overridable Function attemptStamp(ByVal expectedReference As V, ByVal newStamp As Integer) As Boolean
			Dim current As Pair(Of V) = pair_Renamed
			Return expectedReference Is current.reference AndAlso (newStamp = current.stamp OrElse casPair(current, Pair.of(expectedReference, newStamp)))
		End Function

		' Unsafe mechanics

		Private Shared ReadOnly UNSAFE As sun.misc.Unsafe = sun.misc.Unsafe.unsafe
		Private Shared ReadOnly pairOffset As Long = objectFieldOffset(UNSAFE, "pair", GetType(AtomicStampedReference))

		Private Function casPair(ByVal cmp As Pair(Of V), ByVal val As Pair(Of V)) As Boolean
			Return UNSAFE.compareAndSwapObject(Me, pairOffset, cmp, val)
		End Function

		Friend Shared Function objectFieldOffset(ByVal UNSAFE As sun.misc.Unsafe, ByVal field As String, ByVal klazz As [Class]) As Long
			Try
				Return UNSAFE.objectFieldOffset(klazz.getDeclaredField(field))
			Catch e As NoSuchFieldException
				' Convert Exception to corresponding Error
				Dim error_Renamed As New NoSuchFieldError(field)
				error_Renamed.initCause(e)
				Throw error_Renamed
			End Try
		End Function
	End Class

End Namespace