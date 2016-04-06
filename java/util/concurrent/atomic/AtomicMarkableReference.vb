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
	''' An {@code AtomicMarkableReference} maintains an object reference
	''' along with a mark bit, that can be updated atomically.
	''' 
	''' <p>Implementation note: This implementation maintains markable
	''' references by creating internal objects representing "boxed"
	''' [reference, boolean] pairs.
	''' 
	''' @since 1.5
	''' @author Doug Lea </summary>
	''' @param <V> The type of object referred to by this reference </param>
	Public Class AtomicMarkableReference(Of V)

		Private Class Pair(Of T)
			Friend ReadOnly reference As T
			Friend ReadOnly mark As Boolean
			Private Sub New(  reference As T,   mark As Boolean)
				Me.reference = reference
				Me.mark = mark
			End Sub
			Shared Function [of](Of T)(  reference As T,   mark As Boolean) As Pair(Of T)
				Return New Pair(Of T)(reference, mark)
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private pair_Renamed As Pair(Of V)

		''' <summary>
		''' Creates a new {@code AtomicMarkableReference} with the given
		''' initial values.
		''' </summary>
		''' <param name="initialRef"> the initial reference </param>
		''' <param name="initialMark"> the initial mark </param>
		Public Sub New(  initialRef As V,   initialMark As Boolean)
			pair_Renamed = Pair.of(initialRef, initialMark)
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
		''' Returns the current value of the mark.
		''' </summary>
		''' <returns> the current value of the mark </returns>
		Public Overridable Property marked As Boolean
			Get
				Return pair_Renamed.mark
			End Get
		End Property

		''' <summary>
		''' Returns the current values of both the reference and the mark.
		''' Typical usage is {@code boolean[1] holder; ref = v.get(holder); }.
		''' </summary>
		''' <param name="markHolder"> an array of size of at least one. On return,
		''' {@code markholder[0]} will hold the value of the mark. </param>
		''' <returns> the current value of the reference </returns>
		Public Overridable Function [get](  markHolder As Boolean()) As V
			Dim pair_Renamed As Pair(Of V) = Me.pair_Renamed
			markHolder(0) = pair_Renamed.mark
			Return pair_Renamed.reference
		End Function

		''' <summary>
		''' Atomically sets the value of both the reference and mark
		''' to the given update values if the
		''' current reference is {@code ==} to the expected reference
		''' and the current mark is equal to the expected mark.
		''' 
		''' <p><a href="package-summary.html#weakCompareAndSet">May fail
		''' spuriously and does not provide ordering guarantees</a>, so is
		''' only rarely an appropriate alternative to {@code compareAndSet}.
		''' </summary>
		''' <param name="expectedReference"> the expected value of the reference </param>
		''' <param name="newReference"> the new value for the reference </param>
		''' <param name="expectedMark"> the expected value of the mark </param>
		''' <param name="newMark"> the new value for the mark </param>
		''' <returns> {@code true} if successful </returns>
		Public Overridable Function weakCompareAndSet(  expectedReference As V,   newReference As V,   expectedMark As Boolean,   newMark As Boolean) As Boolean
			Return compareAndSet(expectedReference, newReference, expectedMark, newMark)
		End Function

		''' <summary>
		''' Atomically sets the value of both the reference and mark
		''' to the given update values if the
		''' current reference is {@code ==} to the expected reference
		''' and the current mark is equal to the expected mark.
		''' </summary>
		''' <param name="expectedReference"> the expected value of the reference </param>
		''' <param name="newReference"> the new value for the reference </param>
		''' <param name="expectedMark"> the expected value of the mark </param>
		''' <param name="newMark"> the new value for the mark </param>
		''' <returns> {@code true} if successful </returns>
		Public Overridable Function compareAndSet(  expectedReference As V,   newReference As V,   expectedMark As Boolean,   newMark As Boolean) As Boolean
			Dim current As Pair(Of V) = pair_Renamed
			Return expectedReference Is current.reference AndAlso expectedMark = current.mark AndAlso ((newReference Is current.reference AndAlso newMark = current.mark) OrElse casPair(current, Pair.of(newReference, newMark)))
		End Function

		''' <summary>
		''' Unconditionally sets the value of both the reference and mark.
		''' </summary>
		''' <param name="newReference"> the new value for the reference </param>
		''' <param name="newMark"> the new value for the mark </param>
		Public Overridable Sub [set](  newReference As V,   newMark As Boolean)
			Dim current As Pair(Of V) = pair_Renamed
			If newReference IsNot current.reference OrElse newMark <> current.mark Then Me.pair_Renamed = Pair.of(newReference, newMark)
		End Sub

		''' <summary>
		''' Atomically sets the value of the mark to the given update value
		''' if the current reference is {@code ==} to the expected
		''' reference.  Any given invocation of this operation may fail
		''' (return {@code false}) spuriously, but repeated invocation
		''' when the current value holds the expected value and no other
		''' thread is also attempting to set the value will eventually
		''' succeed.
		''' </summary>
		''' <param name="expectedReference"> the expected value of the reference </param>
		''' <param name="newMark"> the new value for the mark </param>
		''' <returns> {@code true} if successful </returns>
		Public Overridable Function attemptMark(  expectedReference As V,   newMark As Boolean) As Boolean
			Dim current As Pair(Of V) = pair_Renamed
			Return expectedReference Is current.reference AndAlso (newMark = current.mark OrElse casPair(current, Pair.of(expectedReference, newMark)))
		End Function

		' Unsafe mechanics

		Private Shared ReadOnly UNSAFE As sun.misc.Unsafe = sun.misc.Unsafe.unsafe
		Private Shared ReadOnly pairOffset As Long = objectFieldOffset(UNSAFE, "pair", GetType(AtomicMarkableReference))

		Private Function casPair(  cmp As Pair(Of V),   val As Pair(Of V)) As Boolean
			Return UNSAFE.compareAndSwapObject(Me, pairOffset, cmp, val)
		End Function

		Friend Shared Function objectFieldOffset(  UNSAFE As sun.misc.Unsafe,   field As String,   klazz As [Class]) As Long
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