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

Namespace java.util.concurrent.locks

	''' <summary>
	''' A synchronizer that may be exclusively owned by a thread.  This
	''' class provides a basis for creating locks and related synchronizers
	''' that may entail a notion of ownership.  The
	''' {@code AbstractOwnableSynchronizer} class itself does not manage or
	''' use this information. However, subclasses and tools may use
	''' appropriately maintained values to help control and monitor access
	''' and provide diagnostics.
	''' 
	''' @since 1.6
	''' @author Doug Lea
	''' </summary>
	<Serializable> _
	Public MustInherit Class AbstractOwnableSynchronizer

		''' <summary>
		''' Use serial ID even though all fields transient. </summary>
		Private Const serialVersionUID As Long = 3737899427754241961L

		''' <summary>
		''' Empty constructor for use by subclasses.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' The current owner of exclusive mode synchronization.
		''' </summary>
		<NonSerialized> _
		Private exclusiveOwnerThread As Thread

		''' <summary>
		''' Sets the thread that currently owns exclusive access.
		''' A {@code null} argument indicates that no thread owns access.
		''' This method does not otherwise impose any synchronization or
		''' {@code volatile} field accesses. </summary>
		''' <param name="thread"> the owner thread </param>
		Protected Friend Property exclusiveOwnerThread As Thread
			Set(  thread_Renamed As Thread)
				exclusiveOwnerThread = thread_Renamed
			End Set
			Get
				Return exclusiveOwnerThread
			End Get
		End Property

	End Class

End Namespace