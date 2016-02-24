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
	''' Weak reference objects, which do not prevent their referents from being
	''' made finalizable, finalized, and then reclaimed.  Weak references are most
	''' often used to implement canonicalizing mappings.
	''' 
	''' <p> Suppose that the garbage collector determines at a certain point in time
	''' that an object is <a href="package-summary.html#reachability">weakly
	''' reachable</a>.  At that time it will atomically clear all weak references to
	''' that object and all weak references to any other weakly-reachable objects
	''' from which that object is reachable through a chain of strong and soft
	''' references.  At the same time it will declare all of the formerly
	''' weakly-reachable objects to be finalizable.  At the same time or at some
	''' later time it will enqueue those newly-cleared weak references that are
	''' registered with reference queues.
	''' 
	''' @author   Mark Reinhold
	''' @since    1.2
	''' </summary>

	Public Class WeakReference(Of T)
		Inherits Reference(Of T)

		''' <summary>
		''' Creates a new weak reference that refers to the given object.  The new
		''' reference is not registered with any queue.
		''' </summary>
		''' <param name="referent"> object the new weak reference will refer to </param>
		Public Sub New(ByVal referent As T)
			MyBase.New(referent)
		End Sub

		''' <summary>
		''' Creates a new weak reference that refers to the given object and is
		''' registered with the given queue.
		''' </summary>
		''' <param name="referent"> object the new weak reference will refer to </param>
		''' <param name="q"> the queue with which the reference is to be registered,
		'''          or <tt>null</tt> if registration is not required </param>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Sub New(Of T1)(ByVal referent As T, ByVal q As ReferenceQueue(Of T1))
			MyBase.New(referent, q)
		End Sub

	End Class

End Namespace