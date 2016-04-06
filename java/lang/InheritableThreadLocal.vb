'
' * Copyright (c) 1998, 2012, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang

	''' <summary>
	''' This class extends <tt>ThreadLocal</tt> to provide inheritance of values
	''' from parent thread to child thread: when a child thread is created, the
	''' child receives initial values for all inheritable thread-local variables
	''' for which the parent has values.  Normally the child's values will be
	''' identical to the parent's; however, the child's value can be made an
	''' arbitrary function of the parent's by overriding the <tt>childValue</tt>
	''' method in this class.
	''' 
	''' <p>Inheritable thread-local variables are used in preference to
	''' ordinary thread-local variables when the per-thread-attribute being
	''' maintained in the variable (e.g., User ID, Transaction ID) must be
	''' automatically transmitted to any child threads that are created.
	''' 
	''' @author  Josh Bloch and Doug Lea </summary>
	''' <seealso cref=     ThreadLocal
	''' @since   1.2 </seealso>

	Public Class InheritableThreadLocal(Of T)
		Inherits ThreadLocal(Of T)

		''' <summary>
		''' Computes the child's initial value for this inheritable thread-local
		''' variable as a function of the parent's value at the time the child
		''' thread is created.  This method is called from within the parent
		''' thread before the child is started.
		''' <p>
		''' This method merely returns its input argument, and should be overridden
		''' if a different behavior is desired.
		''' </summary>
		''' <param name="parentValue"> the parent thread's value </param>
		''' <returns> the child thread's initial value </returns>
		Protected Friend Overridable Function childValue(  parentValue As T) As T
			Return parentValue
		End Function

		''' <summary>
		''' Get the map associated with a ThreadLocal.
		''' </summary>
		''' <param name="t"> the current thread </param>
		Friend Overridable Function getMap(  t As Thread) As ThreadLocalMap
		   Return t.inheritableThreadLocals
		End Function

		''' <summary>
		''' Create the map associated with a ThreadLocal.
		''' </summary>
		''' <param name="t"> the current thread </param>
		''' <param name="firstValue"> value for the initial entry of the table. </param>
		Friend Overridable Sub createMap(  t As Thread,   firstValue As T)
			t.inheritableThreadLocals = New ThreadLocalMap(Me, firstValue)
		End Sub
	End Class

End Namespace