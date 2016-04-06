'
' * Copyright (c) 2012, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.util.stream


	''' <summary>
	''' Abstract class for fork-join tasks used to implement short-circuiting
	''' stream ops, which can produce a result without processing all elements of the
	''' stream.
	''' </summary>
	''' @param <P_IN> type of input elements to the pipeline </param>
	''' @param <P_OUT> type of output elements from the pipeline </param>
	''' @param <R> type of intermediate result, may be different from operation
	'''        result type </param>
	''' @param <K> type of child and sibling tasks
	''' @since 1.8 </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Friend MustInherit Class AbstractShortCircuitTask(Of P_IN, P_OUT, R, K As AbstractShortCircuitTask(Of P_IN, P_OUT, R, K))
		Inherits AbstractTask(Of P_IN, P_OUT, R, K)

		''' <summary>
		''' The result for this computation; this is shared among all tasks and set
		''' exactly once
		''' </summary>
		Protected Friend ReadOnly sharedResult As java.util.concurrent.atomic.AtomicReference(Of R)

		''' <summary>
		''' Indicates whether this task has been canceled.  Tasks may cancel other
		''' tasks in the computation under various conditions, such as in a
		''' find-first operation, a task that finds a value will cancel all tasks
		''' that are later in the encounter order.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Protected Friend canceled As Boolean

		''' <summary>
		''' Constructor for root tasks.
		''' </summary>
		''' <param name="helper"> the {@code PipelineHelper} describing the stream pipeline
		'''               up to this operation </param>
		''' <param name="spliterator"> the {@code Spliterator} describing the source for this
		'''                    pipeline </param>
		Protected Friend Sub New(  helper As PipelineHelper(Of P_OUT),   spliterator As java.util.Spliterator(Of P_IN))
			MyBase.New(helper, spliterator)
			sharedResult = New java.util.concurrent.atomic.AtomicReference(Of )(Nothing)
		End Sub

		''' <summary>
		''' Constructor for non-root nodes.
		''' </summary>
		''' <param name="parent"> parent task in the computation tree </param>
		''' <param name="spliterator"> the {@code Spliterator} for the portion of the
		'''                    computation tree described by this task </param>
		Protected Friend Sub New(  parent As K,   spliterator As java.util.Spliterator(Of P_IN))
			MyBase.New(parent, spliterator)
			sharedResult = parent.sharedResult
		End Sub

		''' <summary>
		''' Returns the value indicating the computation completed with no task
		''' finding a short-circuitable result.  For example, for a "find" operation,
		''' this might be null or an empty {@code Optional}.
		''' </summary>
		''' <returns> the result to return when no task finds a result </returns>
		Protected Friend MustOverride ReadOnly Property emptyResult As R

		''' <summary>
		''' Overrides AbstractTask version to include checks for early
		''' exits while splitting or computing.
		''' </summary>
		Public Overrides Sub compute()
			Dim rs As java.util.Spliterator(Of P_IN) = spliterator, ls As java.util.Spliterator(Of P_IN)
			Dim sizeEstimate As Long = rs.estimateSize()
			Dim sizeThreshold As Long = getTargetSize(sizeEstimate)
			Dim forkRight As Boolean = False
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim task As K = CType(Me, K)
			Dim sr As java.util.concurrent.atomic.AtomicReference(Of R) = sharedResult
			Dim result As R
			result = sr.get()
			Do While result Is Nothing
				If task.taskCanceled() Then
					result = task.emptyResult
					Exit Do
				End If
				ls = rs.trySplit()
				If sizeEstimate <= sizeThreshold OrElse ls Is Nothing Then
					result = task.doLeaf()
					Exit Do
				End If
				Dim leftChild, rightChild, taskToFork As K
					leftChild = task.makeChild(ls)
					task.leftChild = leftChild
					rightChild = task.makeChild(rs)
					task.rightChild = rightChild
				task.pendingCount = 1
				If forkRight Then
					forkRight = False
					rs = ls
					task = leftChild
					taskToFork = rightChild
				Else
					forkRight = True
					task = rightChild
					taskToFork = leftChild
				End If
				taskToFork.fork()
				sizeEstimate = rs.estimateSize()
				result = sr.get()
			Loop
			task.localResult = result
			task.tryComplete()
		End Sub


		''' <summary>
		''' Declares that a globally valid result has been found.  If another task has
		''' not already found the answer, the result is installed in
		''' {@code sharedResult}.  The {@code compute()} method will check
		''' {@code sharedResult} before proceeding with computation, so this causes
		''' the computation to terminate early.
		''' </summary>
		''' <param name="result"> the result found </param>
		Protected Friend Overridable Sub shortCircuit(  result As R)
			If result IsNot Nothing Then sharedResult.compareAndSet(Nothing, result)
		End Sub

		''' <summary>
		''' Sets a local result for this task.  If this task is the root, set the
		''' shared result instead (if not already set).
		''' </summary>
		''' <param name="localResult"> The result to set for this task </param>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Protected Friend Overrides Sub setLocalResult(  localResult As R) 'JavaToDotNetTempPropertySetlocalResult
		Protected Friend Overrides Property localResult As R
			Set(  localResult As R)
				If root Then
					If localResult IsNot Nothing Then sharedResult.compareAndSet(Nothing, localResult)
				Else
					MyBase.localResult = localResult
				End If
			End Set
			Get
		End Property

		''' <summary>
		''' Retrieves the local result for this task
		''' </summary>
		Public  Overrides ReadOnly Property  rawResult As R
			Get
				Return localResult
			End Get
		End Property

			If root Then
				Dim answer As R = sharedResult.get()
				Return If(answer Is Nothing, emptyResult, answer)
			Else
				Return MyBase.localResult
			End If
		End Function

		''' <summary>
		''' Mark this task as canceled
		''' </summary>
		Protected Friend Overridable Sub cancel()
			canceled = True
		End Sub

		''' <summary>
		''' Queries whether this task is canceled.  A task is considered canceled if
		''' it or any of its parents have been canceled.
		''' </summary>
		''' <returns> {@code true} if this task or any parent is canceled. </returns>
		Protected Friend Overridable Function taskCanceled() As Boolean
			Dim cancel As Boolean = canceled
			If Not cancel Then
				Dim parent As K = parent
				Do While (Not cancel) AndAlso parent IsNot Nothing
					cancel = parent.canceled
					parent = parent.parent
				Loop
			End If

			Return cancel
		End Function

		''' <summary>
		''' Cancels all tasks which succeed this one in the encounter order.  This
		''' includes canceling all the current task's right sibling, as well as the
		''' later right siblings of all its parents.
		''' </summary>
		Protected Friend Overridable Sub cancelLaterNodes()
			' Go up the tree, cancel right siblings of this node and all parents
			SuppressWarnings("unchecked") K parent = parent
			SuppressWarnings("unchecked") K node = CType(Me, K)
			Do While parent IsNot Nothing
				' If node is a left child of parent, then has a right sibling
				If parent.leftChild Is node Then
					Dim rightSibling As K = parent.rightChild
					If Not rightSibling.canceled Then rightSibling.cancel()
				End If
				node = parent
				parent = parent.parent
			Loop
		End Sub
	End Class

End Namespace