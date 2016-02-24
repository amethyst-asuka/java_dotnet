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
	''' Soft reference objects, which are cleared at the discretion of the garbage
	''' collector in response to memory demand.  Soft references are most often used
	''' to implement memory-sensitive caches.
	''' 
	''' <p> Suppose that the garbage collector determines at a certain point in time
	''' that an object is <a href="package-summary.html#reachability">softly
	''' reachable</a>.  At that time it may choose to clear atomically all soft
	''' references to that object and all soft references to any other
	''' softly-reachable objects from which that object is reachable through a chain
	''' of strong references.  At the same time or at some later time it will
	''' enqueue those newly-cleared soft references that are registered with
	''' reference queues.
	''' 
	''' <p> All soft references to softly-reachable objects are guaranteed to have
	''' been cleared before the virtual machine throws an
	''' <code>OutOfMemoryError</code>.  Otherwise no constraints are placed upon the
	''' time at which a soft reference will be cleared or the order in which a set
	''' of such references to different objects will be cleared.  Virtual machine
	''' implementations are, however, encouraged to bias against clearing
	''' recently-created or recently-used soft references.
	''' 
	''' <p> Direct instances of this class may be used to implement simple caches;
	''' this class or derived subclasses may also be used in larger data structures
	''' to implement more sophisticated caches.  As long as the referent of a soft
	''' reference is strongly reachable, that is, is actually in use, the soft
	''' reference will not be cleared.  Thus a sophisticated cache can, for example,
	''' prevent its most recently used entries from being discarded by keeping
	''' strong referents to those entries, leaving the remaining entries to be
	''' discarded at the discretion of the garbage collector.
	''' 
	''' @author   Mark Reinhold
	''' @since    1.2
	''' </summary>

	Public Class SoftReference(Of T)
		Inherits Reference(Of T)

		''' <summary>
		''' Timestamp clock, updated by the garbage collector
		''' </summary>
		Private Shared clock As Long

		''' <summary>
		''' Timestamp updated by each invocation of the get method.  The VM may use
		''' this field when selecting soft references to be cleared, but it is not
		''' required to do so.
		''' </summary>
		Private timestamp As Long

		''' <summary>
		''' Creates a new soft reference that refers to the given object.  The new
		''' reference is not registered with any queue.
		''' </summary>
		''' <param name="referent"> object the new soft reference will refer to </param>
		Public Sub New(ByVal referent As T)
			MyBase.New(referent)
			Me.timestamp = clock
		End Sub

		''' <summary>
		''' Creates a new soft reference that refers to the given object and is
		''' registered with the given queue.
		''' </summary>
		''' <param name="referent"> object the new soft reference will refer to </param>
		''' <param name="q"> the queue with which the reference is to be registered,
		'''          or <tt>null</tt> if registration is not required
		'''  </param>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Sub New(Of T1)(ByVal referent As T, ByVal q As ReferenceQueue(Of T1))
			MyBase.New(referent, q)
			Me.timestamp = clock
		End Sub

		''' <summary>
		''' Returns this reference object's referent.  If this reference object has
		''' been cleared, either by the program or by the garbage collector, then
		''' this method returns <code>null</code>.
		''' </summary>
		''' <returns>   The object to which this reference refers, or
		'''           <code>null</code> if this reference object has been cleared </returns>
		Public Overridable Function [get]() As T
			Dim o As T = MyBase.get()
			If o IsNot Nothing AndAlso Me.timestamp <> clock Then Me.timestamp = clock
			Return o
		End Function

	End Class

End Namespace