Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System.Threading

'
' * Copyright (c) 1999, 2008, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.text


	''' <summary>
	''' A queue of text layout tasks.
	''' 
	''' @author  Timothy Prinzing </summary>
	''' <seealso cref=     AsyncBoxView
	''' @since   1.3 </seealso>
	Public Class LayoutQueue

		Private Shared ReadOnly DEFAULT_QUEUE As New Object

		Private tasks As List(Of Runnable)
		Private worker As Thread

		''' <summary>
		''' Construct a layout queue.
		''' </summary>
		Public Sub New()
			tasks = New List(Of Runnable)
		End Sub

		''' <summary>
		''' Fetch the default layout queue.
		''' </summary>
		Public Property Shared defaultQueue As LayoutQueue
			Get
				Dim ac As sun.awt.AppContext = sun.awt.AppContext.appContext
				SyncLock DEFAULT_QUEUE
					Dim ___defaultQueue As LayoutQueue = CType(ac.get(DEFAULT_QUEUE), LayoutQueue)
					If ___defaultQueue Is Nothing Then
						___defaultQueue = New LayoutQueue
						ac.put(DEFAULT_QUEUE, ___defaultQueue)
					End If
					Return ___defaultQueue
				End SyncLock
			End Get
			Set(ByVal q As LayoutQueue)
				SyncLock DEFAULT_QUEUE
					sun.awt.AppContext.appContext.put(DEFAULT_QUEUE, q)
				End SyncLock
			End Set
		End Property


		''' <summary>
		''' Add a task that is not needed immediately because
		''' the results are not believed to be visible.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addTask(ByVal task As Runnable)
			If worker Is Nothing Then
				worker = New LayoutThread(Me)
				worker.Start()
			End If
			tasks.Add(task)
			notifyAll()
		End Sub

		''' <summary>
		''' Used by the worker thread to get a new task to execute
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Overridable Function waitForWork() As Runnable
			Do While tasks.Count = 0
				Try
					wait()
				Catch ie As InterruptedException
					Return Nothing
				End Try
			Loop
			Dim work As Runnable = tasks(0)
			tasks.RemoveAt(0)
			Return work
		End Function

		''' <summary>
		''' low priority thread to perform layout work forever
		''' </summary>
		Friend Class LayoutThread
			Inherits System.Threading.Thread

			Private ReadOnly outerInstance As LayoutQueue


			Friend Sub New(ByVal outerInstance As LayoutQueue)
					Me.outerInstance = outerInstance
				MyBase.New("text-layout")
				priority = Thread.MIN_PRIORITY
			End Sub

			Public Overridable Sub run()
				Dim work As Runnable
				Do
					work = outerInstance.waitForWork()
					If work IsNot Nothing Then work.run()
				Loop While work IsNot Nothing
			End Sub


		End Class

	End Class

End Namespace