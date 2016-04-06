Imports System.Runtime.InteropServices

'
' * Copyright (c) 1999, 2005, Oracle and/or its affiliates. All rights reserved.
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
	''' Package-private utility class containing data structures and logic
	''' governing the virtual-machine shutdown sequence.
	''' 
	''' @author   Mark Reinhold
	''' @since    1.3
	''' </summary>

	Friend Class Shutdown

		' Shutdown state 
		Private Const RUNNING As Integer = 0
		Private Const HOOKS As Integer = 1
		Private Const FINALIZERS As Integer = 2
		Private Shared state As Integer = RUNNING

		' Should we run all finalizers upon exit? 
		Private Shared runFinalizersOnExit As Boolean = False

		' The system shutdown hooks are registered with a predefined slot.
		' The list of shutdown hooks is as follows:
		' (0) Console restore hook
		' (1) Application hooks
		' (2) DeleteOnExit hook
		Private Const MAX_SYSTEM_HOOKS As Integer = 10
		Private Shared ReadOnly hooks As Runnable() = New Runnable(MAX_SYSTEM_HOOKS - 1){}

		' the index of the currently running shutdown hook to the hooks array
		Private Shared currentRunningHook As Integer = 0

		' The preceding static fields are protected by this lock 
		Private Class Lock
		End Class
		Private Shared lock As Object = New Lock

		' Lock object for the native halt method 
		Private Shared haltLock As Object = New Lock

		' Invoked by Runtime.runFinalizersOnExit 
		Friend Shared Property runFinalizersOnExit As Boolean
			Set(  run As Boolean)
				SyncLock lock
					runFinalizersOnExit = run
				End SyncLock
			End Set
		End Property


		''' <summary>
		''' Add a new shutdown hook.  Checks the shutdown state and the hook itself,
		''' but does not do any security checks.
		''' 
		''' The registerShutdownInProgress parameter should be false except
		''' registering the DeleteOnExitHook since the first file may
		''' be added to the delete on exit list by the application shutdown
		''' hooks.
		''' 
		''' @params slot  the slot in the shutdown hook array, whose element
		'''               will be invoked in order during shutdown
		''' @params registerShutdownInProgress true to allow the hook
		'''               to be registered even if the shutdown is in progress.
		''' @params hook  the hook to be registered
		''' 
		''' @throw IllegalStateException
		'''        if registerShutdownInProgress is false and shutdown is in progress; or
		'''        if registerShutdownInProgress is true and the shutdown process
		'''           already passes the given slot
		''' </summary>
		Friend Shared Sub add(  slot As Integer,   registerShutdownInProgress As Boolean,   hook As Runnable)
			SyncLock lock
				If hooks(slot) IsNot Nothing Then Throw New InternalError("Shutdown hook at slot " & slot & " already registered")

				If Not registerShutdownInProgress Then
					If state > RUNNING Then Throw New IllegalStateException("Shutdown in progress")
				Else
					If state > HOOKS OrElse (state = HOOKS AndAlso slot <= currentRunningHook) Then Throw New IllegalStateException("Shutdown in progress")
				End If

				hooks(slot) = hook
			End SyncLock
		End Sub

	'     Run all registered shutdown hooks
	'     
		Private Shared Sub runHooks()
			For i As Integer = 0 To MAX_SYSTEM_HOOKS - 1
				Try
					Dim hook As Runnable
					SyncLock lock
						' acquire the lock to make sure the hook registered during
						' shutdown is visible here.
						currentRunningHook = i
						hook = hooks(i)
					End SyncLock
					If hook IsNot Nothing Then hook.run()
				Catch t As Throwable
					If TypeOf t Is ThreadDeath Then
						Dim td As ThreadDeath = CType(t, ThreadDeath)
						Throw td
					End If
				End Try
			Next i
		End Sub

	'     The halt method is synchronized on the halt lock
	'     * to avoid corruption of the delete-on-shutdown file list.
	'     * It invokes the true native halt method.
	'     
		Friend Shared Sub halt(  status As Integer)
			SyncLock haltLock
				halt0(status)
			End SyncLock
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Sub halt0(  status As Integer)
		End Sub

		' Wormhole for invoking java.lang.ref.Finalizer.runAllFinalizers 
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub runAllFinalizers()
		End Sub


	'     The actual shutdown sequence is defined here.
	'     *
	'     * If it weren't for runFinalizersOnExit, this would be simple -- we'd just
	'     * run the hooks and then halt.  Instead we need to keep track of whether
	'     * we're running hooks or finalizers.  In the latter case a finalizer could
	'     * invoke exit(1) to cause immediate termination, while in the former case
	'     * any further invocations of exit(n), for any n, simply stall.  Note that
	'     * if on-exit finalizers are enabled they're run iff the shutdown is
	'     * initiated by an exit(0); they're never run on exit(n) for n != 0 or in
	'     * response to SIGINT, SIGTERM, etc.
	'     
		Private Shared Sub sequence()
			SyncLock lock
	'             Guard against the possibility of a daemon thread invoking exit
	'             * after DestroyJavaVM initiates the shutdown sequence
	'             
				If state <> HOOKS Then Return
			End SyncLock
			runHooks()
			Dim rfoe As Boolean
			SyncLock lock
				state = FINALIZERS
				rfoe = runFinalizersOnExit
			End SyncLock
			If rfoe Then runAllFinalizers()
		End Sub


	'     Invoked by Runtime.exit, which does all the security checks.
	'     * Also invoked by handlers for system-provided termination events,
	'     * which should pass a nonzero status code.
	'     
		Friend Shared Sub [exit](  status As Integer)
			Dim runMoreFinalizers As Boolean = False
			SyncLock lock
				If status <> 0 Then runFinalizersOnExit = False
				Select Case state
				Case RUNNING ' Initiate shutdown
					state = HOOKS
				Case HOOKS ' Stall and halt
				Case FINALIZERS
					If status <> 0 Then
						' Halt immediately on nonzero status 
						halt(status)
					Else
	'                     Compatibility with old behavior:
	'                     * Run more finalizers and then halt
	'                     
						runMoreFinalizers = runFinalizersOnExit
					End If
				End Select
			End SyncLock
			If runMoreFinalizers Then
				runAllFinalizers()
				halt(status)
			End If
			SyncLock GetType(Shutdown)
	'             Synchronize on the class object, causing any other thread
	'             * that attempts to initiate shutdown to stall indefinitely
	'             
				sequence()
				halt(status)
			End SyncLock
		End Sub


	'     Invoked by the JNI DestroyJavaVM procedure when the last non-daemon
	'     * thread has finished.  Unlike the exit method, this method does not
	'     * actually halt the VM.
	'     
		Shared Sub shutdown()
			SyncLock lock
				Select Case state
				Case RUNNING ' Initiate shutdown
					state = HOOKS
				Case HOOKS, FINALIZERS ' Stall and then return
				End Select
			End SyncLock
			SyncLock GetType(Shutdown)
				sequence()
			End SyncLock
		End Sub

	End Class

End Namespace