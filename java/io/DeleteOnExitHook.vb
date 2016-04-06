Imports System.Runtime.CompilerServices
Imports System.Collections.Generic

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
Namespace java.io


	''' <summary>
	''' This class holds a set of filenames to be deleted on VM exit through a shutdown hook.
	''' A set is used both to prevent double-insertion of the same file as well as offer
	''' quick removal.
	''' </summary>

	Friend Class DeleteOnExitHook
		Private Shared files As New LinkedHashSet(Of String)
		Shared Sub New()
			' DeleteOnExitHook must be the last shutdown hook to be invoked.
			' Application shutdown hooks may add the first file to the
			' delete on exit list and cause the DeleteOnExitHook to be
			' registered during shutdown in progress. So set the
			' registerShutdownInProgress parameter to true.
			sun.misc.SharedSecrets.javaLangAccess.registerShutdownHook(2, True, New RunnableAnonymousInnerClassHelper ' register even if shutdown in progress -  Shutdown hook invocation order
		   )
		End Sub

		Private Class RunnableAnonymousInnerClassHelper
			Implements Runnable

			Public Overridable Sub run() Implements Runnable.run
			   runHooks()
			End Sub
		End Class

		Private Sub New()
		End Sub

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Shared Sub add(  file_Renamed As String)
			If files Is Nothing Then Throw New IllegalStateException("Shutdown in progress")

			files.Add(file_Renamed)
		End Sub

		Friend Shared Sub runHooks()
			Dim theFiles As LinkedHashSet(Of String)

			SyncLock GetType(DeleteOnExitHook)
				theFiles = files
				files = Nothing
			End SyncLock

			Dim toBeDeleted As New List(Of String)(theFiles)

			' reverse the list to maintain previous jdk deletion order.
			' Last in first deleted.
			Collections.reverse(toBeDeleted)
			For Each filename As String In toBeDeleted
				If System.IO.Directory.Exists(filename) Then System.IO.Directory.Delete(filename, True) Else System.IO.File.Delete(filename)
			Next filename
		End Sub
	End Class

End Namespace