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
	''' Abstract base class for most fork-join tasks used to implement stream ops.
	''' Manages splitting logic, tracking of child tasks, and intermediate results.
	''' Each task is associated with a <seealso cref="Spliterator"/> that describes the portion
	''' of the input associated with the subtree rooted at this task.
	''' Tasks may be leaf nodes (which will traverse the elements of
	''' the {@code Spliterator}) or internal nodes (which split the
	''' {@code Spliterator} into multiple child tasks).
	''' 
	''' @implNote
	''' <p>This class is based on <seealso cref="CountedCompleter"/>, a form of fork-join task
	''' where each task has a semaphore-like count of uncompleted children, and the
	''' task is implicitly completed and notified when its last child completes.
	''' Internal node tasks will likely override the {@code onCompletion} method from
	''' {@code CountedCompleter} to merge the results from child tasks into the
	''' current task's result.
	''' 
	''' <p>Splitting and setting up the child task links is done by {@code compute()}
	''' for internal nodes.  At {@code compute()} time for leaf nodes, it is
	''' guaranteed that the parent's child-related fields (including sibling links
	''' for the parent's children) will be set up for all children.
	''' 
	''' <p>For example, a task that performs a reduce would override {@code doLeaf()}
	''' to perform a reduction on that leaf node's chunk using the
	''' {@code Spliterator}, and override {@code onCompletion()} to merge the results
	''' of the child tasks for internal nodes:
	''' 
	''' <pre>{@code
	'''     protected S doLeaf() {
	'''         spliterator.forEach(...);
	'''         return localReductionResult;
	'''     }
	''' 
	'''     public  Sub  onCompletion(CountedCompleter caller) {
	'''         if (!isLeaf()) {
	'''             ReduceTask<P_IN, P_OUT, T, R> child = children;
	'''             R result = child.getLocalResult();
	'''             child = child.nextSibling;
	'''             for (; child != null; child = child.nextSibling)
	'''                 result = combine(result, child.getLocalResult());
	'''             setLocalResult(result);
	'''         }
	'''     }
	''' }</pre>
	''' 
	''' <p>Serialization is not supported as there is no intention to serialize
	''' tasks managed by stream ops.
	''' </summary>
	''' @param <P_IN> Type of elements input to the pipeline </param>
	''' @param <P_OUT> Type of elements output from the pipeline </param>
	''' @param <R> Type of intermediate result, which may be different from operation
	'''        result type </param>
	''' @param <K> Type of parent, child and sibling tasks
	''' @since 1.8 </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Friend MustInherit Class AbstractTask(Of P_IN, P_OUT, R, K As AbstractTask(Of P_IN, P_OUT, R, K))
		Inherits java.util.concurrent.CountedCompleter(Of R)

		''' <summary>
		''' Default target factor of leaf tasks for parallel decomposition.
		''' To allow load balancing, we over-partition, currently to approximately
		''' four tasks per processor, which enables others to help out
		''' if leaf tasks are uneven or some processors are otherwise busy.
		''' </summary>
		Friend Shared ReadOnly LEAF_TARGET As Integer = java.util.concurrent.ForkJoinPool.commonPoolParallelism << 2

		''' <summary>
		''' The pipeline helper, common to all tasks in a computation </summary>
		Protected Friend ReadOnly helper As PipelineHelper(Of P_OUT)

		''' <summary>
		''' The spliterator for the portion of the input associated with the subtree
		''' rooted at this task
		''' </summary>
		Protected Friend spliterator As java.util.Spliterator(Of P_IN)

		''' <summary>
		''' Target leaf size, common to all tasks in a computation </summary>
		Protected Friend targetSize As Long ' may be laziliy initialized

		''' <summary>
		''' The left child.
		''' null if no children
		''' if non-null rightChild is non-null
		''' </summary>
		Protected Friend leftChild As K

		''' <summary>
		''' The right child.
		''' null if no children
		''' if non-null leftChild is non-null
		''' </summary>
		Protected Friend rightChild As K

		''' <summary>
		''' The result of this node, if completed </summary>
		Private localResult As R

		''' <summary>
		''' Constructor for root nodes.
		''' </summary>
		''' <param name="helper"> The {@code PipelineHelper} describing the stream pipeline
		'''               up to this operation </param>
		''' <param name="spliterator"> The {@code Spliterator} describing the source for this
		'''                    pipeline </param>
		Protected Friend Sub New(ByVal helper As PipelineHelper(Of P_OUT), ByVal spliterator As java.util.Spliterator(Of P_IN))
			MyBase.New(Nothing)
			Me.helper = helper
			Me.spliterator = spliterator
			Me.targetSize = 0L
		End Sub

		''' <summary>
		''' Constructor for non-root nodes.
		''' </summary>
		''' <param name="parent"> this node's parent task </param>
		''' <param name="spliterator"> {@code Spliterator} describing the subtree rooted at
		'''        this node, obtained by splitting the parent {@code Spliterator} </param>
		Protected Friend Sub New(ByVal parent As K, ByVal spliterator As java.util.Spliterator(Of P_IN))
			MyBase.New(parent)
			Me.spliterator = spliterator
			Me.helper = parent.helper
			Me.targetSize = parent.targetSize
		End Sub

		''' <summary>
		''' Constructs a new node of type T whose parent is the receiver; must call
		''' the AbstractTask(T, Spliterator) constructor with the receiver and the
		''' provided Spliterator.
		''' </summary>
		''' <param name="spliterator"> {@code Spliterator} describing the subtree rooted at
		'''        this node, obtained by splitting the parent {@code Spliterator} </param>
		''' <returns> newly constructed child node </returns>
		Protected Friend MustOverride Function makeChild(ByVal spliterator As java.util.Spliterator(Of P_IN)) As K

		''' <summary>
		''' Computes the result associated with a leaf node.  Will be called by
		''' {@code compute()} and the result passed to @{code setLocalResult()}
		''' </summary>
		''' <returns> the computed result of a leaf node </returns>
		Protected Friend MustOverride Function doLeaf() As R

		''' <summary>
		''' Returns a suggested target leaf size based on the initial size estimate.
		''' </summary>
		''' <returns> suggested target leaf size </returns>
		Public Shared Function suggestTargetSize(ByVal sizeEstimate As Long) As Long
			Dim est As Long = sizeEstimate \ LEAF_TARGET
			Return If(est > 0L, est, 1L)
		End Function

		''' <summary>
		''' Returns the targetSize, initializing it via the supplied
		''' size estimate if not already initialized.
		''' </summary>
		Protected Friend Function getTargetSize(ByVal sizeEstimate As Long) As Long
			Dim s As Long
				s = targetSize
				If s <> 0 Then
					Return (s)
				Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Return (targetSize = suggestTargetSize(sizeEstimate))
				End If
		End Function

		''' <summary>
		''' Returns the local result, if any. Subclasses should use
		''' <seealso cref="#setLocalResult(Object)"/> and <seealso cref="#getLocalResult()"/> to manage
		''' results.  This returns the local result so that calls from within the
		''' fork-join framework will return the correct result.
		''' </summary>
		''' <returns> local result for this node previously stored with
		''' <seealso cref="#setLocalResult"/> </returns>
		Public Property Overrides rawResult As R
			Get
				Return localResult
			End Get
			Set(ByVal result As R)
				If result IsNot Nothing Then Throw New IllegalStateException
			End Set
		End Property


		''' <summary>
		''' Retrieves a result previously stored with <seealso cref="#setLocalResult"/>
		''' </summary>
		''' <returns> local result for this node previously stored with
		''' <seealso cref="#setLocalResult"/> </returns>
		Protected Friend Overridable Property localResult As R
			Get
				Return localResult
			End Get
			Set(ByVal localResult As R)
				Me.localResult = localResult
			End Set
		End Property


		''' <summary>
		''' Indicates whether this task is a leaf node.  (Only valid after
		''' <seealso cref="#compute"/> has been called on this node).  If the node is not a
		''' leaf node, then children will be non-null and numChildren will be
		''' positive.
		''' </summary>
		''' <returns> {@code true} if this task is a leaf node </returns>
		Protected Friend Overridable Property leaf As Boolean
			Get
				Return leftChild Is Nothing
			End Get
		End Property

		''' <summary>
		''' Indicates whether this task is the root node
		''' </summary>
		''' <returns> {@code true} if this task is the root node. </returns>
		Protected Friend Overridable Property root As Boolean
			Get
				Return parent Is Nothing
			End Get
		End Property

		''' <summary>
		''' Returns the parent of this task, or null if this task is the root
		''' </summary>
		''' <returns> the parent of this task, or null if this task is the root </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Protected Friend Overridable Property parent As K
			Get
				Return CType(completer, K)
			End Get
		End Property

		''' <summary>
		''' Decides whether or not to split a task further or compute it
		''' directly. If computing directly, calls {@code doLeaf} and pass
		''' the result to {@code setRawResult}. Otherwise splits off
		''' subtasks, forking one and continuing as the other.
		''' 
		''' <p> The method is structured to conserve resources across a
		''' range of uses.  The loop continues with one of the child tasks
		''' when split, to avoid deep recursion. To cope with spliterators
		''' that may be systematically biased toward left-heavy or
		''' right-heavy splits, we alternate which child is forked versus
		''' continued in the loop.
		''' </summary>
		Public Overrides Sub compute()
			Dim rs As java.util.Spliterator(Of P_IN) = spliterator, ls As java.util.Spliterator(Of P_IN) ' right, left spliterators
			Dim sizeEstimate As Long = rs.estimateSize()
			Dim sizeThreshold As Long = getTargetSize(sizeEstimate)
			Dim forkRight As Boolean = False
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim task As K = CType(Me, K)
			ls = rs.trySplit()
			Do While sizeEstimate > sizeThreshold AndAlso ls IsNot Nothing
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
				ls = rs.trySplit()
			Loop
			task.localResult = task.doLeaf()
			task.tryComplete()
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' @implNote
		''' Clears spliterator and children fields.  Overriders MUST call
		''' {@code super.onCompletion} as the last thing they do if they want these
		''' cleared.
		''' </summary>
		Public Overrides Sub onCompletion(Of T1)(ByVal caller As java.util.concurrent.CountedCompleter(Of T1))
			spliterator = Nothing
				rightChild = Nothing
				leftChild = rightChild
		End Sub

		''' <summary>
		''' Returns whether this node is a "leftmost" node -- whether the path from
		''' the root to this node involves only traversing leftmost child links.  For
		''' a leaf node, this means it is the first leaf node in the encounter order.
		''' </summary>
		''' <returns> {@code true} if this node is a "leftmost" node </returns>
		Protected Friend Overridable Property leftmostNode As Boolean
			Get
	'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim node As K = CType(Me, K)
				Do While node IsNot Nothing
					Dim parent_Renamed As K = node.parent
					If parent_Renamed IsNot Nothing AndAlso parent_Renamed.leftChild IsNot node Then Return False
					node = parent_Renamed
				Loop
				Return True
			End Get
		End Property
	End Class

End Namespace