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
	''' designated {@code volatile} reference fields of designated
	''' classes.  This class is designed for use in atomic data structures
	''' in which several reference fields of the same node are
	''' independently subject to atomic updates. For example, a tree node
	''' might be declared as
	''' 
	'''  <pre> {@code
	''' class Node {
	'''   private volatile Node left, right;
	''' 
	'''   private static final AtomicReferenceFieldUpdater<Node, Node> leftUpdater =
	'''     AtomicReferenceFieldUpdater.newUpdater(Node.class, Node.class, "left");
	'''   private static AtomicReferenceFieldUpdater<Node, Node> rightUpdater =
	'''     AtomicReferenceFieldUpdater.newUpdater(Node.class, Node.class, "right");
	''' 
	'''   Node getLeft() { return left;  }
	'''   boolean compareAndSetLeft(Node expect, Node update) {
	'''     return leftUpdater.compareAndSet(this, expect, update);
	'''   }
	'''   // ... and so on
	''' }}</pre>
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
	''' @param <V> The type of the field </param>
	Public MustInherit Class AtomicReferenceFieldUpdater(Of T, V)

		''' <summary>
		''' Creates and returns an updater for objects with the given field.
		''' The Class arguments are needed to check that reflective types and
		''' generic types match.
		''' </summary>
		''' <param name="tclass"> the class of the objects holding the field </param>
		''' <param name="vclass"> the class of the field </param>
		''' <param name="fieldName"> the name of the field to be updated </param>
		''' @param <U> the type of instances of tclass </param>
		''' @param <W> the type of instances of vclass </param>
		''' <returns> the updater </returns>
		''' <exception cref="ClassCastException"> if the field is of the wrong type </exception>
		''' <exception cref="IllegalArgumentException"> if the field is not volatile </exception>
		''' <exception cref="RuntimeException"> with a nested reflection-based
		''' exception if the class does not hold field or is the wrong type,
		''' or the field is inaccessible to the caller according to Java language
		''' access control </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Function newUpdater(Of U, W)(ByVal tclass As [Class], ByVal vclass As [Class], ByVal fieldName As String) As AtomicReferenceFieldUpdater(Of U, W)
			Return New AtomicReferenceFieldUpdaterImpl(Of U, W) (tclass, vclass, fieldName, sun.reflect.Reflection.callerClass)
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
		Public MustOverride Function compareAndSet(ByVal obj As T, ByVal expect As V, ByVal update As V) As Boolean

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
		Public MustOverride Function weakCompareAndSet(ByVal obj As T, ByVal expect As V, ByVal update As V) As Boolean

		''' <summary>
		''' Sets the field of the given object managed by this updater to the
		''' given updated value. This operation is guaranteed to act as a volatile
		''' store with respect to subsequent invocations of {@code compareAndSet}.
		''' </summary>
		''' <param name="obj"> An object whose field to set </param>
		''' <param name="newValue"> the new value </param>
		Public MustOverride Sub [set](ByVal obj As T, ByVal newValue As V)

		''' <summary>
		''' Eventually sets the field of the given object managed by this
		''' updater to the given updated value.
		''' </summary>
		''' <param name="obj"> An object whose field to set </param>
		''' <param name="newValue"> the new value
		''' @since 1.6 </param>
		Public MustOverride Sub lazySet(ByVal obj As T, ByVal newValue As V)

		''' <summary>
		''' Gets the current value held in the field of the given object managed
		''' by this updater.
		''' </summary>
		''' <param name="obj"> An object whose field to get </param>
		''' <returns> the current value </returns>
		Public MustOverride Function [get](ByVal obj As T) As V

		''' <summary>
		''' Atomically sets the field of the given object managed by this updater
		''' to the given value and returns the old value.
		''' </summary>
		''' <param name="obj"> An object whose field to get and set </param>
		''' <param name="newValue"> the new value </param>
		''' <returns> the previous value </returns>
		Public Overridable Function getAndSet(ByVal obj As T, ByVal newValue As V) As V
			Dim prev As V
			Do
				prev = [get](obj)
			Loop While Not compareAndSet(obj, prev, newValue)
			Return prev
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
		Public Function getAndUpdate(ByVal obj As T, ByVal updateFunction As java.util.function.UnaryOperator(Of V)) As V
			Dim prev, [next] As V
			Do
				prev = [get](obj)
				[next] = updateFunction.apply(prev)
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
		Public Function updateAndGet(ByVal obj As T, ByVal updateFunction As java.util.function.UnaryOperator(Of V)) As V
			Dim prev, [next] As V
			Do
				prev = [get](obj)
				[next] = updateFunction.apply(prev)
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
		Public Function getAndAccumulate(ByVal obj As T, ByVal x As V, ByVal accumulatorFunction As java.util.function.BinaryOperator(Of V)) As V
			Dim prev, [next] As V
			Do
				prev = [get](obj)
				[next] = accumulatorFunction.apply(prev, x)
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
		Public Function accumulateAndGet(ByVal obj As T, ByVal x As V, ByVal accumulatorFunction As java.util.function.BinaryOperator(Of V)) As V
			Dim prev, [next] As V
			Do
				prev = [get](obj)
				[next] = accumulatorFunction.apply(prev, x)
			Loop While Not compareAndSet(obj, prev, [next])
			Return [next]
		End Function

		Private NotInheritable Class AtomicReferenceFieldUpdaterImpl(Of T, V)
			Inherits AtomicReferenceFieldUpdater(Of T, V)

			Private Shared ReadOnly unsafe As sun.misc.Unsafe = sun.misc.Unsafe.unsafe
			Private ReadOnly offset As Long
			Private ReadOnly tclass As  [Class]
			Private ReadOnly vclass As  [Class]
			Private ReadOnly cclass As  [Class]

	'        
	'         * Internal type checks within all update methods contain
	'         * internal inlined optimizations checking for the common
	'         * cases where the class is final (in which case a simple
	'         * getClass comparison suffices) or is of type Object (in
	'         * which case no check is needed because all objects are
	'         * instances of Object). The Object case is handled simply by
	'         * setting vclass to null in constructor.  The targetCheck and
	'         * updateCheck methods are invoked when these faster
	'         * screenings fail.
	'         

			Friend Sub New(ByVal tclass As [Class], ByVal vclass As [Class], ByVal fieldName As String, ByVal caller As [Class])
				Dim field As Field
				Dim fieldClass As  [Class]
				Dim modifiers As Integer
				Try
					field = java.security.AccessController.doPrivileged(New PrivilegedExceptionActionAnonymousInnerClassHelper(Of T)
					modifiers = field.modifiers
					sun.reflect.misc.ReflectUtil.ensureMemberAccess(caller, tclass, Nothing, modifiers)
					Dim cl As  ClassLoader = tclass.classLoader
					Dim ccl As  ClassLoader = caller.classLoader
					If (ccl IsNot Nothing) AndAlso (ccl IsNot cl) AndAlso ((cl Is Nothing) OrElse (Not isAncestor(cl, ccl))) Then sun.reflect.misc.ReflectUtil.checkPackageAccess(tclass)
					fieldClass = field.type
				Catch pae As java.security.PrivilegedActionException
					Throw New RuntimeException(pae.exception)
				Catch ex As Exception
					Throw New RuntimeException(ex)
				End Try

				If vclass IsNot fieldClass Then Throw New ClassCastException
				If vclass.primitive Then Throw New IllegalArgumentException("Must be reference type")

				If Not Modifier.isVolatile(modifiers) Then Throw New IllegalArgumentException("Must be volatile type")

				Me.cclass = If(Modifier.isProtected(modifiers) AndAlso caller IsNot tclass, caller, Nothing)
				Me.tclass = tclass
				If vclass Is GetType(Object) Then
					Me.vclass = Nothing
				Else
					Me.vclass = vclass
				End If
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
			Private Shared Function isAncestor(ByVal first As  ClassLoader, ByVal second As  ClassLoader) As Boolean
				Dim acl As  ClassLoader = first
				Do
					acl = acl.parent
					If second Is acl Then Return True
				Loop While acl IsNot Nothing
				Return False
			End Function

			Friend Sub targetCheck(ByVal obj As T)
				If Not tclass.isInstance(obj) Then Throw New ClassCastException
				If cclass IsNot Nothing Then ensureProtectedAccess(obj)
			End Sub

			Friend Sub updateCheck(ByVal obj As T, ByVal update As V)
				If (Not tclass.isInstance(obj)) OrElse (update IsNot Nothing AndAlso vclass IsNot Nothing AndAlso (Not vclass.isInstance(update))) Then Throw New ClassCastException
				If cclass IsNot Nothing Then ensureProtectedAccess(obj)
			End Sub

			Public Function compareAndSet(ByVal obj As T, ByVal expect As V, ByVal update As V) As Boolean
				If obj Is Nothing OrElse obj.GetType() IsNot tclass OrElse cclass IsNot Nothing OrElse (update IsNot Nothing AndAlso vclass IsNot Nothing AndAlso vclass IsNot update.GetType()) Then updateCheck(obj, update)
				Return unsafe.compareAndSwapObject(obj, offset, expect, update)
			End Function

			Public Function weakCompareAndSet(ByVal obj As T, ByVal expect As V, ByVal update As V) As Boolean
				' same implementation as strong form for now
				If obj Is Nothing OrElse obj.GetType() IsNot tclass OrElse cclass IsNot Nothing OrElse (update IsNot Nothing AndAlso vclass IsNot Nothing AndAlso vclass IsNot update.GetType()) Then updateCheck(obj, update)
				Return unsafe.compareAndSwapObject(obj, offset, expect, update)
			End Function

			Public Sub [set](ByVal obj As T, ByVal newValue As V)
				If obj Is Nothing OrElse obj.GetType() IsNot tclass OrElse cclass IsNot Nothing OrElse (newValue IsNot Nothing AndAlso vclass IsNot Nothing AndAlso vclass IsNot newValue.GetType()) Then updateCheck(obj, newValue)
				unsafe.putObjectVolatile(obj, offset, newValue)
			End Sub

			Public Sub lazySet(ByVal obj As T, ByVal newValue As V)
				If obj Is Nothing OrElse obj.GetType() IsNot tclass OrElse cclass IsNot Nothing OrElse (newValue IsNot Nothing AndAlso vclass IsNot Nothing AndAlso vclass IsNot newValue.GetType()) Then updateCheck(obj, newValue)
				unsafe.putOrderedObject(obj, offset, newValue)
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Function [get](ByVal obj As T) As V
				If obj Is Nothing OrElse obj.GetType() IsNot tclass OrElse cclass IsNot Nothing Then targetCheck(obj)
				Return CType(unsafe.getObjectVolatile(obj, offset), V)
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Function getAndSet(ByVal obj As T, ByVal newValue As V) As V
				If obj Is Nothing OrElse obj.GetType() IsNot tclass OrElse cclass IsNot Nothing OrElse (newValue IsNot Nothing AndAlso vclass IsNot Nothing AndAlso vclass IsNot newValue.GetType()) Then updateCheck(obj, newValue)
				Return CType(unsafe.getAndSetObject(obj, offset, newValue), V)
			End Function

			Private Sub ensureProtectedAccess(ByVal obj As T)
				If cclass.isInstance(obj) Then Return
				Throw New RuntimeException(New IllegalAccessException("Class " & cclass.name & " can not access a protected member of class " & tclass.name & " using an instance of " & obj.GetType().name)
			   )
			End Sub
		End Class
	End Class

End Namespace