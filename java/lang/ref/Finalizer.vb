Imports System.Threading

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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


	Friend NotInheritable Class Finalizer
		Inherits FinalReference(Of Object)

	' Package-private; must be in
	'                                                          same package as the Reference
	'                                                          class 

		Private Shared queue As New ReferenceQueue(Of Object)
		Private Shared unfinalized As Finalizer = Nothing
		Private Shared ReadOnly lock As New Object

		Private [next] As Finalizer = Nothing, prev As Finalizer = Nothing

		Private Function hasBeenFinalized() As Boolean
			Return ([next] Is Me)
		End Function

		Private Sub add()
			SyncLock lock
				If unfinalized IsNot Nothing Then
					Me.next = unfinalized
					unfinalized.prev = Me
				End If
				unfinalized = Me
			End SyncLock
		End Sub

		Private Sub remove()
			SyncLock lock
				If unfinalized Is Me Then
					If Me.next IsNot Nothing Then
						unfinalized = Me.next
					Else
						unfinalized = Me.prev
					End If
				End If
				If Me.next IsNot Nothing Then Me.next.prev = Me.prev
				If Me.prev IsNot Nothing Then Me.prev.next = Me.next
				Me.next = Me ' Indicates that this has been finalized
				Me.prev = Me
			End SyncLock
		End Sub

		Private Sub New(ByVal finalizee As Object)
			MyBase.New(finalizee, queue)
			add()
		End Sub

		' Invoked by VM 
		Friend Shared Sub register(ByVal finalizee As Object)
			Dim TempFinalizer As Finalizer = New Finalizer(finalizee)
		End Sub

		Private Sub runFinalizer(ByVal jla As sun.misc.JavaLangAccess)
			SyncLock Me
				If hasBeenFinalized() Then Return
				remove()
			End SyncLock
			Try
				Dim finalizee As Object = Me.get()
				If finalizee IsNot Nothing AndAlso Not(TypeOf finalizee Is java.lang.Enum) Then
					jla.invokeFinalize(finalizee)

	'                 Clear stack slot containing this variable, to decrease
	'                   the chances of false retention with a conservative GC 
					finalizee = Nothing
				End If
			Catch x As Throwable
			End Try
			MyBase.clear()
		End Sub

	'     Create a privileged secondary finalizer thread in the system thread
	'       group for the given Runnable, and wait for it to complete.
	'
	'       This method is used by both runFinalization and runFinalizersOnExit.
	'       The former method invokes all pending finalizers, while the latter
	'       invokes all uninvoked finalizers if on-exit finalization has been
	'       enabled.
	'
	'       These two methods could have been implemented by offloading their work
	'       to the regular finalizer thread and waiting for that thread to finish.
	'       The advantage of creating a fresh thread, however, is that it insulates
	'       invokers of these methods from a stalled or deadlocked finalizer thread.
	'     
		Private Shared Sub forkSecondaryFinalizer(ByVal proc As Runnable)
			java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
		End Sub

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As Void
			Dim tg As ThreadGroup = Thread.CurrentThread.threadGroup
			Dim tgn As ThreadGroup = tg
			Do While tgn IsNot Nothing

				tg = tgn
				tgn = tg.parent
			Loop
			Dim sft As New Thread(tg, proc, "Secondary finalizer")
			sft.start()
			Try
				sft.join()
			Catch x As InterruptedException
				' Ignore 
			End Try
			Return Nothing
			End Function
		End Class

		' Called by Runtime.runFinalization() 
		Friend Shared Sub runFinalization()
			If Not sun.misc.VM.booted Then Return

			forkSecondaryFinalizer(New RunnableAnonymousInnerClassHelper
		End Sub

		Private Class RunnableAnonymousInnerClassHelper
			Implements Runnable

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Private running As Boolean
			Public Overridable Sub run() Implements Runnable.run
				If running Then Return
				Dim jla As sun.misc.JavaLangAccess = sun.misc.SharedSecrets.javaLangAccess
				running = True
				Do
					Dim f As Finalizer = CType(outerInstance.queue.poll(), Finalizer)
					If f Is Nothing Then Exit Do
					f.runFinalizer(jla)
				Loop
			End Sub
		End Class

		' Invoked by java.lang.Shutdown 
		Friend Shared Sub runAllFinalizers()
			If Not sun.misc.VM.booted Then Return

			forkSecondaryFinalizer(New RunnableAnonymousInnerClassHelper2
		End Sub

		Private Class RunnableAnonymousInnerClassHelper2
			Implements Runnable

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Private running As Boolean
			Public Overridable Sub run() Implements Runnable.run
				If running Then Return
				Dim jla As sun.misc.JavaLangAccess = sun.misc.SharedSecrets.javaLangAccess
				running = True
				Do
					Dim f As Finalizer
					SyncLock lock
						f = unfinalized
						If f Is Nothing Then Exit Do
						unfinalized = f.next
					End SyncLock
					f.runFinalizer(jla)
				Loop
			End Sub
		End Class

		Private Class FinalizerThread
			Inherits Thread

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Private running As Boolean
			Friend Sub New(ByVal g As ThreadGroup)
				MyBase.New(g, "Finalizer")
			End Sub
			Public Overrides Sub run()
				If running Then Return

				' Finalizer thread starts before System.initializeSystemClass
				' is called.  Wait until JavaLangAccess is available
				Do While Not sun.misc.VM.booted
					' delay until VM completes initialization
					Try
						sun.misc.VM.awaitBooted()
					Catch x As InterruptedException
						' ignore and continue
					End Try
				Loop
				Dim jla As sun.misc.JavaLangAccess = sun.misc.SharedSecrets.javaLangAccess
				running = True
				Do
					Try
						Dim f As Finalizer = CType(outerInstance.queue.remove(), Finalizer)
						f.runFinalizer(jla)
					Catch x As InterruptedException
						' ignore and continue
					End Try
				Loop
			End Sub
		End Class

		Shared Sub New()
			Dim tg As ThreadGroup = Thread.CurrentThread.threadGroup
			Dim tgn As ThreadGroup = tg
			Do While tgn IsNot Nothing

				tg = tgn
				tgn = tg.parent
			Loop
			Dim finalizer_Renamed As Thread = New FinalizerThread(tg)
			finalizer_Renamed.priority = Thread.MAX_PRIORITY - 2
			finalizer_Renamed.daemon = True
			finalizer_Renamed.start()
		End Sub

	End Class

End Namespace