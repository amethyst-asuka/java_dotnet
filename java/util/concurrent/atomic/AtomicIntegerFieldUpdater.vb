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
	''' A reflection-based utility that enables atomic updates to
	''' designated {@code volatile int} fields of designated classes.
	''' This class is designed for use in atomic data structures in which
	''' several fields of the same node are independently subject to atomic
	''' updates.
	''' 
	''' <p>Note that the guarantees of the {@code compareAndSet}
	''' method in this class are weaker than in other atomic classes.
	''' Because this class cannot ensure that all uses of the field
	''' are appropriate for purposes of atomic access, it can
	''' guarantee atomicity only with respect to other invocations of
	''' {@code compareAndSet} and {@code set} on the same updater.
	''' 
	''' @since 1.5
	''' @author Doug Lea </summary>
	''' @param <T> The type of the object holding the updatable field </param>
	Public MustInherit Class AtomicIntegerFieldUpdater(Of T)
		''' <summary>
		''' Creates and returns an updater for objects with the given field.
		''' The Class argument is needed to check that reflective types and
		''' generic types match.
		''' </summary>
		''' <param name="tclass"> the class of the objects holding the field </param>
		''' <param name="fieldName"> the name of the field to be updated </param>
		''' @param <U> the type of instances of tclass </param>
		''' <returns> the updater </returns>
		''' <exception cref="IllegalArgumentException"> if the field is not a
		''' volatile integer type </exception>
		''' <exception cref="RuntimeException"> with a nested reflection-based
		''' exception if the class does not hold field or is the wrong type,
		''' or the field is inaccessible to the caller according to Java language
		''' access control </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Function newUpdater(Of U)(ByVal tclass As Class, ByVal fieldName As String) As AtomicIntegerFieldUpdater(Of U)
			Return New AtomicIntegerFieldUpdaterImpl(Of U) (tclass, fieldName, sun.reflect.Reflection.callerClass)
		End Function

		''' <summary>
		''' Protected do-nothing constructor for use by subclasses.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Atomically sets the field of the given object managed by this updater
		''' to the given updated value if the current value {@code ==} the
		''' expected value. This method is guaranteed to be atomic with respect to
		''' other calls to {@code compareAndSet} and {@code set}, but not
		''' necessarily with respect to other changes in the field.
		''' </summary>
		''' <param name="obj"> An object whose field to conditionally set </param>
		''' <param name="expect"> the expected value </param>
		''' <param name="update"> the new value </param>
		''' <returns> {@code true} if successful </returns>
		''' <exception cref="ClassCastException"> if {@code obj} is not an instance
		''' of the class possessing the field established in the constructor </exception>
		Public MustOverride Function compareAndSet(ByVal obj As T, ByVal expect As Integer, ByVal update As Integer) As Boolean

		''' <summary>
		''' Atomically sets the field of the given object managed by this updater
		''' to the given updated value if the current value {@code ==} the
		''' expected value. This method is guaranteed to be atomic with respect to
		''' other calls to {@code compareAndSet} and {@code set}, but not
		''' necessarily with respect to other changes in the field.
		''' 
		''' <p><a href="package-summary.html#weakCompareAndSet">May fail
		''' spuriously and does not provide ordering guarantees</a>, so is
		''' only rarely an appropriate alternative to {@code compareAndSet}.
		''' </summary>
		''' <param name="obj"> An object whose field to conditionally set </param>
		''' <param name="expect"> the expected value </param>
		''' <param name="update"> the new value </param>
		''' <returns> {@code true} if successful </returns>
		''' <exception cref="ClassCastException"> if {@code obj} is not an instance
		''' of the class possessing the field established in the constructor </exception>
		Public MustOverride Function weakCompareAndSet(ByVal obj As T, ByVal expect As Integer, ByVal update As Integer) As Boolean

		''' <summary>
		''' Sets the field of the given object managed by this updater to the
		''' given updated value. This operation is guaranteed to act as a volatile
		''' store with respect to subsequent invocations of {@code compareAndSet}.
		''' </summary>
		''' <param name="obj"> An object whose field to set </param>
		''' <param name="newValue"> the new value </param>
		Public MustOverride Sub [set](ByVal obj As T, ByVal newValue As Integer)

		''' <summary>
		''' Eventually sets the field of the given object managed by this
		''' updater to the given updated value.
		''' </summary>
		''' <param name="obj"> An object whose field to set </param>
		''' <param name="newValue"> the new value
		''' @since 1.6 </param>
		Public MustOverride Sub lazySet(ByVal obj As T, ByVal newValue As Integer)

		''' <summary>
		''' Gets the current value held in the field of the given object managed
		''' by this updater.
		''' </summary>
		''' <param name="obj"> An object whose field to get </param>
		''' <returns> the current value </returns>
		Public MustOverride Function [get](ByVal obj As T) As Integer

		''' <summary>
		''' Atomically sets the field of the given object managed by this updater
		''' to the given value and returns the old value.
		''' </summary>
		''' <param name="obj"> An object whose field to get and set </param>
		''' <param name="newValue"> the new value </param>
		''' <returns> the previous value </returns>
		Public Overridable Function getAndSet(ByVal obj As T, ByVal newValue As Integer) As Integer
			Dim prev As Integer
			Do
				prev = [get](obj)
			Loop While Not compareAndSet(obj, prev, newValue)
			Return prev
		End Function

		''' <summary>
		''' Atomically increments by one the current value of the field of the
		''' given object managed by this updater.
		''' </summary>
		''' <param name="obj"> An object whose field to get and set </param>
		''' <returns> the previous value </returns>
		Public Overridable Function getAndIncrement(ByVal obj As T) As Integer
			Dim prev, [next] As Integer
			Do
				prev = [get](obj)
				[next] = prev + 1
			Loop While Not compareAndSet(obj, prev, [next])
			Return prev
		End Function

		''' <summary>
		''' Atomically decrements by one the current value of the field of the
		''' given object managed by this updater.
		''' </summary>
		''' <param name="obj"> An object whose field to get and set </param>
		''' <returns> the previous value </returns>
		Public Overridable Function getAndDecrement(ByVal obj As T) As Integer
			Dim prev, [next] As Integer
			Do
				prev = [get](obj)
				[next] = prev - 1
			Loop While Not compareAndSet(obj, prev, [next])
			Return prev
		End Function

		''' <summary>
		''' Atomically adds the given value to the current value of the field of
		''' the given object managed by this updater.
		''' </summary>
		''' <param name="obj"> An object whose field to get and set </param>
		''' <param name="delta"> the value to add </param>
		''' <returns> the previous value </returns>
		Public Overridable Function getAndAdd(ByVal obj As T, ByVal delta As Integer) As Integer
			Dim prev, [next] As Integer
			Do
				prev = [get](obj)
				[next] = prev + delta
			Loop While Not compareAndSet(obj, prev, [next])
			Return prev
		End Function

		''' <summary>
		''' Atomically increments by one the current value of the field of the
		''' given object managed by this updater.
		''' </summary>
		''' <param name="obj"> An object whose field to get and set </param>
		''' <returns> the updated value </returns>
		Public Overridable Function incrementAndGet(ByVal obj As T) As Integer
			Dim prev, [next] As Integer
			Do
				prev = [get](obj)
				[next] = prev + 1
			Loop While Not compareAndSet(obj, prev, [next])
			Return [next]
		End Function

		''' <summary>
		''' Atomically decrements by one the current value of the field of the
		''' given object managed by this updater.
		''' </summary>
		''' <param name="obj"> An object whose field to get and set </param>
		''' <returns> the updated value </returns>
		Public Overridable Function decrementAndGet(ByVal obj As T) As Integer
			Dim prev, [next] As Integer
			Do
				prev = [get](obj)
				[next] = prev - 1
			Loop While Not compareAndSet(obj, prev, [next])
			Return [next]
		End Function

		''' <summary>
		''' Atomically adds the given value to the current value of the field of
		''' the given object managed by this updater.
		''' </summary>
		''' <param name="obj"> An object whose field to get and set </param>
		''' <param name="delta"> the value to add </param>
		''' <returns> the updated value </returns>
		Public Overridable Function addAndGet(ByVal obj As T, ByVal delta As Integer) As Integer
			Dim prev, [next] As Integer
			Do
				prev = [get](obj)
				[next] = prev + delta
			Loop While Not compareAndSet(obj, prev, [next])
			Return [next]
		End Function

		''' <summary>
		''' Atomically updates the field of the given object managed by this updater
		''' with the results of applying the given function, returning the previous
		''' value. The function should be side-effect-free, since it may be
		''' re-applied when attempted updates fail due to contention among threads.
		''' </summary>
		''' <param name="obj"> An object whose field to get and set </param>
		''' <param name="updateFunction"> a side-effect-free function </param>
		''' <returns> the previous value
		''' @since 1.8 </returns>
		Public Function getAndUpdate(ByVal obj As T, ByVal updateFunction As java.util.function.IntUnaryOperator) As Integer
			Dim prev, [next] As Integer
			Do
				prev = [get](obj)
				[next] = updateFunction.applyAsInt(prev)
			Loop While Not compareAndSet(obj, prev, [next])
			Return prev
		End Function

		''' <summary>
		''' Atomically updates the field of the given object managed by this updater
		''' with the results of applying the given function, returning the updated
		''' value. The function should be side-effect-free, since it may be
		''' re-applied when attempted updates fail due to contention among threads.
		''' </summary>
		''' <param name="obj"> An object whose field to get and set </param>
		''' <param name="updateFunction"> a side-effect-free function </param>
		''' <returns> the updated value
		''' @since 1.8 </returns>
		Public Function updateAndGet(ByVal obj As T, ByVal updateFunction As java.util.function.IntUnaryOperator) As Integer
			Dim prev, [next] As Integer
			Do
				prev = [get](obj)
				[next] = updateFunction.applyAsInt(prev)
			Loop While Not compareAndSet(obj, prev, [next])
			Return [next]
		End Function

		''' <summary>
		''' Atomically updates the field of the given object managed by this
		''' updater with the results of applying the given function to the
		''' current and given values, returning the previous value. The
		''' function should be side-effect-free, since it may be re-applied
		''' when attempted updates fail due to contention among threads.  The
		''' function is applied with the current value as its first argument,
		''' and the given update as the second argument.
		''' </summary>
		''' <param name="obj"> An object whose field to get and set </param>
		''' <param name="x"> the update value </param>
		''' <param name="accumulatorFunction"> a side-effect-free function of two arguments </param>
		''' <returns> the previous value
		''' @since 1.8 </returns>
		Public Function getAndAccumulate(ByVal obj As T, ByVal x As Integer, ByVal accumulatorFunction As java.util.function.IntBinaryOperator) As Integer
			Dim prev, [next] As Integer
			Do
				prev = [get](obj)
				[next] = accumulatorFunction.applyAsInt(prev, x)
			Loop While Not compareAndSet(obj, prev, [next])
			Return prev
		End Function

		''' <summary>
		''' Atomically updates the field of the given object managed by this
		''' updater with the results of applying the given function to the
		''' current and given values, returning the updated value. The
		''' function should be side-effect-free, since it may be re-applied
		''' when attempted updates fail due to contention among threads.  The
		''' function is applied with the current value as its first argument,
		''' and the given update as the second argument.
		''' </summary>
		''' <param name="obj"> An object whose field to get and set </param>
		''' <param name="x"> the update value </param>
		''' <param name="accumulatorFunction"> a side-effect-free function of two arguments </param>
		''' <returns> the updated value
		''' @since 1.8 </returns>
		Public Function accumulateAndGet(ByVal obj As T, ByVal x As Integer, ByVal accumulatorFunction As java.util.function.IntBinaryOperator) As Integer
			Dim prev, [next] As Integer
			Do
				prev = [get](obj)
				[next] = accumulatorFunction.applyAsInt(prev, x)
			Loop While Not compareAndSet(obj, prev, [next])
			Return [next]
		End Function

		''' <summary>
		''' Standard hotspot implementation using intrinsics
		''' </summary>
		Private Class AtomicIntegerFieldUpdaterImpl(Of T)
			Inherits AtomicIntegerFieldUpdater(Of T)

			Private Shared ReadOnly unsafe As sun.misc.Unsafe = sun.misc.Unsafe.unsafe
			Private ReadOnly offset As Long
			Private ReadOnly tclass As Class
			Private ReadOnly cclass As Class

			Friend Sub New(ByVal tclass As Class, ByVal fieldName As String, ByVal caller As Class)
				Dim field As Field
				Dim modifiers As Integer
				Try
					field = java.security.AccessController.doPrivileged(New PrivilegedExceptionActionAnonymousInnerClassHelper(Of T)
					modifiers = field.modifiers
					sun.reflect.misc.ReflectUtil.ensureMemberAccess(caller, tclass, Nothing, modifiers)
					Dim cl As ClassLoader = tclass.classLoader
					Dim ccl As ClassLoader = caller.classLoader
					If (ccl IsNot Nothing) AndAlso (ccl IsNot cl) AndAlso ((cl Is Nothing) OrElse (Not isAncestor(cl, ccl))) Then sun.reflect.misc.ReflectUtil.checkPackageAccess(tclass)
				Catch pae As java.security.PrivilegedActionException
					Throw New RuntimeException(pae.exception)
				Catch ex As Exception
					Throw New RuntimeException(ex)
				End Try

				Dim fieldt As Class = field.type
				If fieldt IsNot GetType(Integer) Then Throw New IllegalArgumentException("Must be integer type")

				If Not Modifier.isVolatile(modifiers) Then Throw New IllegalArgumentException("Must be volatile type")

				Me.cclass = If(Modifier.isProtected(modifiers) AndAlso caller IsNot tclass, caller, Nothing)
				Me.tclass = tclass
				offset = unsafe.objectFieldOffset(field)
			End Sub

			Private Class PrivilegedExceptionActionAnonymousInnerClassHelper(Of T)
				Implements java.security.PrivilegedExceptionAction(Of T)

				Public Overridable Function run() As Field
					Return outerInstance.tclass.getDeclaredField(fieldName)
				End Function
			End Class

			''' <summary>
			''' Returns true if the second classloader can be found in the first
			''' classloader's delegation chain.
			''' Equivalent to the inaccessible: first.isAncestor(second).
			''' </summary>
			Private Shared Function isAncestor(ByVal first As ClassLoader, ByVal second As ClassLoader) As Boolean
				Dim acl As ClassLoader = first
				Do
					acl = acl.parent
					If second Is acl Then Return True
				Loop While acl IsNot Nothing
				Return False
			End Function

			Private Sub fullCheck(ByVal obj As T)
				If Not tclass.isInstance(obj) Then Throw New ClassCastException
				If cclass IsNot Nothing Then ensureProtectedAccess(obj)
			End Sub

			Public Overridable Function compareAndSet(ByVal obj As T, ByVal expect As Integer, ByVal update As Integer) As Boolean
				If obj Is Nothing OrElse obj.GetType() IsNot tclass OrElse cclass IsNot Nothing Then fullCheck(obj)
				Return unsafe.compareAndSwapInt(obj, offset, expect, update)
			End Function

			Public Overridable Function weakCompareAndSet(ByVal obj As T, ByVal expect As Integer, ByVal update As Integer) As Boolean
				If obj Is Nothing OrElse obj.GetType() IsNot tclass OrElse cclass IsNot Nothing Then fullCheck(obj)
				Return unsafe.compareAndSwapInt(obj, offset, expect, update)
			End Function

			Public Overridable Sub [set](ByVal obj As T, ByVal newValue As Integer)
				If obj Is Nothing OrElse obj.GetType() IsNot tclass OrElse cclass IsNot Nothing Then fullCheck(obj)
				unsafe.putIntVolatile(obj, offset, newValue)
			End Sub

			Public Overridable Sub lazySet(ByVal obj As T, ByVal newValue As Integer)
				If obj Is Nothing OrElse obj.GetType() IsNot tclass OrElse cclass IsNot Nothing Then fullCheck(obj)
				unsafe.putOrderedInt(obj, offset, newValue)
			End Sub

			Public Function [get](ByVal obj As T) As Integer
				If obj Is Nothing OrElse obj.GetType() IsNot tclass OrElse cclass IsNot Nothing Then fullCheck(obj)
				Return unsafe.getIntVolatile(obj, offset)
			End Function

			Public Overridable Function getAndSet(ByVal obj As T, ByVal newValue As Integer) As Integer
				If obj Is Nothing OrElse obj.GetType() IsNot tclass OrElse cclass IsNot Nothing Then fullCheck(obj)
				Return unsafe.getAndSetInt(obj, offset, newValue)
			End Function

			Public Overridable Function getAndIncrement(ByVal obj As T) As Integer
				Return getAndAdd(obj, 1)
			End Function

			Public Overridable Function getAndDecrement(ByVal obj As T) As Integer
				Return getAndAdd(obj, -1)
			End Function

			Public Overridable Function getAndAdd(ByVal obj As T, ByVal delta As Integer) As Integer
				If obj Is Nothing OrElse obj.GetType() IsNot tclass OrElse cclass IsNot Nothing Then fullCheck(obj)
				Return unsafe.getAndAddInt(obj, offset, delta)
			End Function

			Public Overridable Function incrementAndGet(ByVal obj As T) As Integer
				Return getAndAdd(obj, 1) + 1
			End Function

			Public Overridable Function decrementAndGet(ByVal obj As T) As Integer
				 Return getAndAdd(obj, -1) - 1
			End Function

			Public Overridable Function addAndGet(ByVal obj As T, ByVal delta As Integer) As Integer
				Return getAndAdd(obj, delta) + delta
			End Function

			Private Sub ensureProtectedAccess(ByVal obj As T)
				If cclass.isInstance(obj) Then Return
				Throw New RuntimeException(New IllegalAccessException("Class " & cclass.name & " can not access a protected member of class " & tclass.name & " using an instance of " & obj.GetType().name)
			   )
			End Sub
		End Class
	End Class

End Namespace