'
' * Copyright (c) 1997, 2003, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang.ref


	''' <summary>
	''' Phantom reference objects, which are enqueued after the collector
	''' determines that their referents may otherwise be reclaimed.  Phantom
	''' references are most often used for scheduling pre-mortem cleanup actions in
	''' a more flexible way than is possible with the Java finalization mechanism.
	''' 
	''' <p> If the garbage collector determines at a certain point in time that the
	''' referent of a phantom reference is <a
	''' href="package-summary.html#reachability">phantom reachable</a>, then at that
	''' time or at some later time it will enqueue the reference.
	''' 
	''' <p> In order to ensure that a reclaimable object remains so, the referent of
	''' a phantom reference may not be retrieved: The <code>get</code> method of a
	''' phantom reference always returns <code>null</code>.
	''' 
	''' <p> Unlike soft and weak references, phantom references are not
	''' automatically cleared by the garbage collector as they are enqueued.  An
	''' object that is reachable via phantom references will remain so until all
	''' such references are cleared or themselves become unreachable.
	''' 
	''' @author   Mark Reinhold
	''' @since    1.2
	''' </summary>

	Public Class PhantomReference(Of T)
		Inherits Reference(Of T)

		''' <summary>
		''' Returns this reference object's referent.  Because the referent of a
		''' phantom reference is always inaccessible, this method always returns
		''' <code>null</code>.
		''' </summary>
		''' <returns>  <code>null</code> </returns>
		Public Overridable Function [get]() As T
			Return Nothing
		End Function

		''' <summary>
		''' Creates a new phantom reference that refers to the given object and
		''' is registered with the given queue.
		''' 
		''' <p> It is possible to create a phantom reference with a <tt>null</tt>
		''' queue, but such a reference is completely useless: Its <tt>get</tt>
		''' method will always return null and, since it does not have a queue, it
		''' will never be enqueued.
		''' </summary>
		''' <param name="referent"> the object the new phantom reference will refer to </param>
		''' <param name="q"> the queue with which the reference is to be registered,
		'''          or <tt>null</tt> if registration is not required </param>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Sub New(Of T1)(ByVal referent As T, ByVal q As ReferenceQueue(Of T1))
			MyBase.New(referent, q)
		End Sub

	End Class

End Namespace