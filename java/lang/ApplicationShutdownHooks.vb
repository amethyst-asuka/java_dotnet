Imports System.Runtime.CompilerServices

'
' * Copyright (c) 2005, 2010, Oracle and/or its affiliates. All rights reserved.
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


	'
	' * Class to track and run user level shutdown hooks registered through
	' * <tt>{@link Runtime#addShutdownHook Runtime.addShutdownHook}</tt>.
	' *
	' * @see java.lang.Runtime#addShutdownHook
	' * @see java.lang.Runtime#removeShutdownHook
	' 

	Friend Class ApplicationShutdownHooks
		' The set of registered hooks 
		Private Shared hooks As IdentityHashMap(Of Thread, Thread)
		Shared Sub New()
			Try
				Shutdown.add(1, False, New RunnableAnonymousInnerClassHelper ' not registered if shutdown in progress -  shutdown hook invocation order
			   )
				hooks = New IdentityHashMap(Of )
			Catch e As IllegalStateException
				' application shutdown hooks cannot be added if
				' shutdown is in progress.
				hooks = Nothing
			End Try
		End Sub

		Private Class RunnableAnonymousInnerClassHelper
			Implements Runnable

			Public Overridable Sub run() Implements Runnable.run
				runHooks()
			End Sub
		End Class


		Private Sub New()
		End Sub

	'     Add a new shutdown hook.  Checks the shutdown state and the hook itself,
	'     * but does not do any security checks.
	'     
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Shared Sub add(ByVal hook As Thread)
			If hooks Is Nothing Then Throw New IllegalStateException("Shutdown in progress")

			If hook.alive Then Throw New IllegalArgumentException("Hook already running")

			If hooks.containsKey(hook) Then Throw New IllegalArgumentException("Hook previously registered")

			hooks.put(hook, hook)
		End Sub

	'     Remove a previously-registered hook.  Like the add method, this method
	'     * does not do any security checks.
	'     
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Shared Function remove(ByVal hook As Thread) As Boolean
			If hooks Is Nothing Then Throw New IllegalStateException("Shutdown in progress")

			If hook Is Nothing Then Throw New NullPointerException

			Return hooks.remove(hook) IsNot Nothing
		End Function

	'     Iterates over all application hooks creating a new thread for each
	'     * to run in. Hooks are run concurrently and this method waits for
	'     * them to finish.
	'     
		Friend Shared Sub runHooks()
			Dim threads As Collection(Of Thread)
			SyncLock GetType(ApplicationShutdownHooks)
				threads = hooks.Keys
				hooks = Nothing
			End SyncLock

			For Each hook As Thread In threads
				hook.start()
			Next hook
			For Each hook As Thread In threads
				Try
					hook.join()
				Catch x As InterruptedException
				End Try
			Next hook
		End Sub
	End Class

End Namespace