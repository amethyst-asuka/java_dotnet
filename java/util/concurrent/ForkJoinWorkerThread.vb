Imports System
Imports System.Threading

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

Namespace java.util.concurrent


	''' <summary>
	''' A thread managed by a <seealso cref="ForkJoinPool"/>, which executes
	''' <seealso cref="ForkJoinTask"/>s.
	''' This class is subclassable solely for the sake of adding
	''' functionality -- there are no overridable methods dealing with
	''' scheduling or execution.  However, you can override initialization
	''' and termination methods surrounding the main task processing loop.
	''' If you do create such a subclass, you will also need to supply a
	''' custom <seealso cref="ForkJoinPool.ForkJoinWorkerThreadFactory"/> to
	''' <seealso cref="ForkJoinPool#ForkJoinPool use it"/> in a {@code ForkJoinPool}.
	''' 
	''' @since 1.7
	''' @author Doug Lea
	''' </summary>
	Public Class ForkJoinWorkerThread
		Inherits Thread

	'    
	'     * ForkJoinWorkerThreads are managed by ForkJoinPools and perform
	'     * ForkJoinTasks. For explanation, see the internal documentation
	'     * of class ForkJoinPool.
	'     *
	'     * This class just maintains links to its pool and WorkQueue.  The
	'     * pool field is set immediately upon construction, but the
	'     * workQueue field is not set until a call to registerWorker
	'     * completes. This leads to a visibility race, that is tolerated
	'     * by requiring that the workQueue field is only accessed by the
	'     * owning thread.
	'     *
	'     * Support for (non-public) subclass InnocuousForkJoinWorkerThread
	'     * requires that we break quite a lot of encapsulation (via Unsafe)
	'     * both here and in the subclass to access and set Thread fields.
	'     

		Friend ReadOnly pool As ForkJoinPool ' the pool this thread works in
		Friend ReadOnly workQueue As ForkJoinPool.WorkQueue ' work-stealing mechanics

		''' <summary>
		''' Creates a ForkJoinWorkerThread operating in the given pool.
		''' </summary>
		''' <param name="pool"> the pool this thread works in </param>
		''' <exception cref="NullPointerException"> if pool is null </exception>
		Protected Friend Sub New(ByVal pool As ForkJoinPool)
			' Use a placeholder until a useful name can be set in registerWorker
			MyBase.New("aForkJoinWorkerThread")
			Me.pool = pool
			Me.workQueue = pool.registerWorker(Me)
		End Sub

		''' <summary>
		''' Version for InnocuousForkJoinWorkerThread
		''' </summary>
		Friend Sub New(ByVal pool As ForkJoinPool, ByVal threadGroup As ThreadGroup, ByVal acc As java.security.AccessControlContext)
			MyBase.New(threadGroup, Nothing, "aForkJoinWorkerThread")
			U.putOrderedObject(Me, INHERITEDACCESSCONTROLCONTEXT, acc)
			eraseThreadLocals() ' clear before registering
			Me.pool = pool
			Me.workQueue = pool.registerWorker(Me)
		End Sub

		''' <summary>
		''' Returns the pool hosting this thread.
		''' </summary>
		''' <returns> the pool </returns>
		Public Overridable Property pool As ForkJoinPool
			Get
				Return pool
			End Get
		End Property

		''' <summary>
		''' Returns the unique index number of this thread in its pool.
		''' The returned value ranges from zero to the maximum number of
		''' threads (minus one) that may exist in the pool, and does not
		''' change during the lifetime of the thread.  This method may be
		''' useful for applications that track status or collect results
		''' per-worker-thread rather than per-task.
		''' </summary>
		''' <returns> the index number </returns>
		Public Overridable Property poolIndex As Integer
			Get
				Return workQueue.poolIndex
			End Get
		End Property

		''' <summary>
		''' Initializes internal state after construction but before
		''' processing any tasks. If you override this method, you must
		''' invoke {@code super.onStart()} at the beginning of the method.
		''' Initialization requires care: Most fields must have legal
		''' default values, to ensure that attempted accesses from other
		''' threads work correctly even before this thread starts
		''' processing tasks.
		''' </summary>
		Protected Friend Overridable Sub onStart()
		End Sub

		''' <summary>
		''' Performs cleanup associated with termination of this worker
		''' thread.  If you override this method, you must invoke
		''' {@code super.onTermination} at the end of the overridden method.
		''' </summary>
		''' <param name="exception"> the exception causing this thread to abort due
		''' to an unrecoverable error, or {@code null} if completed normally </param>
		Protected Friend Overridable Sub onTermination(ByVal exception_Renamed As Throwable)
		End Sub

		''' <summary>
		''' This method is required to be public, but should never be
		''' called explicitly. It performs the main run loop to execute
		''' <seealso cref="ForkJoinTask"/>s.
		''' </summary>
		Public Overrides Sub run()
			If workQueue.array Is Nothing Then ' only run once
				Dim exception_Renamed As Throwable = Nothing
				Try
					onStart()
					pool.runWorker(workQueue)
				Catch ex As Throwable
					exception_Renamed = ex
				Finally
					Try
						onTermination(exception_Renamed)
					Catch ex As Throwable
						If exception_Renamed Is Nothing Then exception_Renamed = ex
					Finally
						pool.deregisterWorker(Me, exception_Renamed)
					End Try
				End Try
			End If
		End Sub

		''' <summary>
		''' Erases ThreadLocals by nulling out Thread maps.
		''' </summary>
		Friend Sub eraseThreadLocals()
			U.putObject(Me, THREADLOCALS, Nothing)
			U.putObject(Me, INHERITABLETHREADLOCALS, Nothing)
		End Sub

		''' <summary>
		''' Non-public hook method for InnocuousForkJoinWorkerThread
		''' </summary>
		Friend Overridable Sub afterTopLevelExec()
		End Sub

		' Set up to allow setting thread fields in constructor
		Private Shared ReadOnly U As sun.misc.Unsafe
		Private Shared ReadOnly THREADLOCALS As Long
		Private Shared ReadOnly INHERITABLETHREADLOCALS As Long
		Private Shared ReadOnly INHERITEDACCESSCONTROLCONTEXT As Long
		Shared Sub New()
			Try
				U = sun.misc.Unsafe.unsafe
				Dim tk As Class = GetType(Thread)
				THREADLOCALS = U.objectFieldOffset(tk.getDeclaredField("threadLocals"))
				INHERITABLETHREADLOCALS = U.objectFieldOffset(tk.getDeclaredField("inheritableThreadLocals"))
				INHERITEDACCESSCONTROLCONTEXT = U.objectFieldOffset(tk.getDeclaredField("inheritedAccessControlContext"))

			Catch e As Exception
				Throw New [Error](e)
			End Try
		End Sub

		''' <summary>
		''' A worker thread that has no permissions, is not a member of any
		''' user-defined ThreadGroup, and erases all ThreadLocals after
		''' running each top-level task.
		''' </summary>
		Friend NotInheritable Class InnocuousForkJoinWorkerThread
			Inherits ForkJoinWorkerThread

			''' <summary>
			''' The ThreadGroup for all InnocuousForkJoinWorkerThreads </summary>
			Private Shared ReadOnly innocuousThreadGroup As ThreadGroup = createThreadGroup()

			''' <summary>
			''' An AccessControlContext supporting no privileges </summary>
			Private Shared ReadOnly INNOCUOUS_ACC As New java.security.AccessControlContext(New java.security.ProtectionDomain() { New java.security.ProtectionDomain(Nothing, Nothing)
					Friend })

			Friend Sub New(ByVal pool As ForkJoinPool)
				MyBase.New(pool, innocuousThreadGroup, INNOCUOUS_ACC)
			End Sub

			Friend Overrides Sub afterTopLevelExec() ' to erase ThreadLocals
				eraseThreadLocals()
			End Sub

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
            Public Overrides Function getContextClassLoader() As ClassLoader ' to always report system loaderJavaToDotNetTempPropertyGetcontextClassLoader
			Public Property Overrides contextClassLoader As ClassLoader
				Get
					Return ClassLoader.systemClassLoader
				End Get
				Set(ByVal cl As ClassLoader)
			End Property

			Public Overrides Property uncaughtExceptionHandler As UncaughtExceptionHandler
				Set(ByVal x As UncaughtExceptionHandler)
				End Set
			End Property
				Throw New SecurityException("setContextClassLoader")
			End Sub

			''' <summary>
			''' Returns a new group with the system ThreadGroup (the
			''' topmost, parent-less group) as parent.  Uses Unsafe to
			''' traverse Thread.group and ThreadGroup.parent fields.
			''' </summary>
			Private Shared Function createThreadGroup() As ThreadGroup
				Try
					Dim u As sun.misc.Unsafe = sun.misc.Unsafe.unsafe
					Dim tk As Class = GetType(Thread)
					Dim gk As Class = GetType(ThreadGroup)
					Dim tg As Long = u.objectFieldOffset(tk.getDeclaredField("group"))
					Dim gp As Long = u.objectFieldOffset(gk.getDeclaredField("parent"))
					Dim group As ThreadGroup = CType(u.getObject(Thread.CurrentThread, tg), ThreadGroup)
					Do While group IsNot Nothing
						Dim parent As ThreadGroup = CType(u.getObject(group, gp), ThreadGroup)
						If parent Is Nothing Then Return New ThreadGroup(group, "InnocuousForkJoinWorkerThreadGroup")
						group = parent
					Loop
				Catch e As Exception
					Throw New [Error](e)
				End Try
				' fall through if null as cannot-happen safeguard
				Throw New [Error]("Cannot create ThreadGroup")
			End Function
		End Class

	End Class

End Namespace