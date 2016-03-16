Imports System
Imports System.Runtime.CompilerServices
Imports System.Threading

'
' * Copyright (c) 1995, 2012, Oracle and/or its affiliates. All rights reserved.
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
	''' A thread group represents a set of threads. In addition, a thread
	''' group can also include other thread groups. The thread groups form
	''' a tree in which every thread group except the initial thread group
	''' has a parent.
	''' <p>
	''' A thread is allowed to access information about its own thread
	''' group, but not to access information about its thread group's
	''' parent thread group or any other thread groups.
	''' 
	''' @author  unascribed
	''' @since   JDK1.0
	''' </summary>
	' The locking strategy for this code is to try to lock only one level of the
	' * tree wherever possible, but otherwise to lock from the bottom up.
	' * That is, from child thread groups to parents.
	' * This has the advantage of limiting the number of locks that need to be held
	' * and in particular avoids having to grab the lock for the root thread group,
	' * (or a global lock) which would be a source of contention on a
	' * multi-processor system with many thread groups.
	' * This policy often leads to taking a snapshot of the state of a thread group
	' * and working off of that snapshot, rather than holding the thread group locked
	' * while we work on the children.
	' 
	Public Class ThreadGroup
		Implements Thread.UncaughtExceptionHandler

		Private ReadOnly parent As ThreadGroup
		Friend name As String
		Friend maxPriority As Integer
		Friend destroyed As Boolean
		Friend daemon As Boolean
		Friend vmAllowSuspension As Boolean

		Friend nUnstartedThreads As Integer = 0
		Friend nthreads As Integer
		Friend threads As Thread()

		Friend ngroups As Integer
		Friend groups As ThreadGroup()

		''' <summary>
		''' Creates an empty Thread group that is not in any Thread group.
		''' This method is used to create the system Thread group.
		''' </summary>
		Private Sub New() ' called from C code
			Me.name = "system"
			Me.maxPriority = Thread.MAX_PRIORITY
			Me.parent = Nothing
		End Sub

		''' <summary>
		''' Constructs a new thread group. The parent of this new group is
		''' the thread group of the currently running thread.
		''' <p>
		''' The <code>checkAccess</code> method of the parent thread group is
		''' called with no arguments; this may result in a security exception.
		''' </summary>
		''' <param name="name">   the name of the new thread group. </param>
		''' <exception cref="SecurityException">  if the current thread cannot create a
		'''               thread in the specified thread group. </exception>
		''' <seealso cref=     java.lang.ThreadGroup#checkAccess()
		''' @since   JDK1.0 </seealso>
		Public Sub New(ByVal name As String)
			Me.New(Thread.CurrentThread.threadGroup, name)
		End Sub

		''' <summary>
		''' Creates a new thread group. The parent of this new group is the
		''' specified thread group.
		''' <p>
		''' The <code>checkAccess</code> method of the parent thread group is
		''' called with no arguments; this may result in a security exception.
		''' </summary>
		''' <param name="parent">   the parent thread group. </param>
		''' <param name="name">     the name of the new thread group. </param>
		''' <exception cref="NullPointerException">  if the thread group argument is
		'''               <code>null</code>. </exception>
		''' <exception cref="SecurityException">  if the current thread cannot create a
		'''               thread in the specified thread group. </exception>
		''' <seealso cref=     java.lang.SecurityException </seealso>
		''' <seealso cref=     java.lang.ThreadGroup#checkAccess()
		''' @since   JDK1.0 </seealso>
		Public Sub New(ByVal parent As ThreadGroup, ByVal name As String)
			Me.New(checkParentAccess(parent), parent, name)
		End Sub

		Private Sub New(ByVal unused As Void, ByVal parent As ThreadGroup, ByVal name As String)
			Me.name = name
			Me.maxPriority = parent.maxPriority
			Me.daemon = parent.daemon
			Me.vmAllowSuspension = parent.vmAllowSuspension
			Me.parent = parent
			parent.add(Me)
		End Sub

	'    
	'     * @throws  NullPointerException  if the parent argument is {@code null}
	'     * @throws  SecurityException     if the current thread cannot create a
	'     *                                thread in the specified thread group.
	'     
		Private Shared Function checkParentAccess(ByVal parent As ThreadGroup) As Void
			parent.checkAccess()
			Return Nothing
		End Function

		''' <summary>
		''' Returns the name of this thread group.
		''' </summary>
		''' <returns>  the name of this thread group.
		''' @since   JDK1.0 </returns>
		Public Property name As String
			Get
				Return name
			End Get
		End Property

		''' <summary>
		''' Returns the parent of this thread group.
		''' <p>
		''' First, if the parent is not <code>null</code>, the
		''' <code>checkAccess</code> method of the parent thread group is
		''' called with no arguments; this may result in a security exception.
		''' </summary>
		''' <returns>  the parent of this thread group. The top-level thread group
		'''          is the only thread group whose parent is <code>null</code>. </returns>
		''' <exception cref="SecurityException">  if the current thread cannot modify
		'''               this thread group. </exception>
		''' <seealso cref=        java.lang.ThreadGroup#checkAccess() </seealso>
		''' <seealso cref=        java.lang.SecurityException </seealso>
		''' <seealso cref=        java.lang.RuntimePermission
		''' @since   JDK1.0 </seealso>
		Public Property parent As ThreadGroup
			Get
				If parent IsNot Nothing Then parent.checkAccess()
				Return parent
			End Get
		End Property

		''' <summary>
		''' Returns the maximum priority of this thread group. Threads that are
		''' part of this group cannot have a higher priority than the maximum
		''' priority.
		''' </summary>
		''' <returns>  the maximum priority that a thread in this thread group
		'''          can have. </returns>
		''' <seealso cref=     #setMaxPriority
		''' @since   JDK1.0 </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Function getMaxPriority() As Integer 'JavaToDotNetTempPropertyGetmaxPriority
		Public Property maxPriority As Integer
			Get
				Return maxPriority
			End Get
			Set(ByVal pri As Integer)
		End Property

		''' <summary>
		''' Tests if this thread group is a daemon thread group. A
		''' daemon thread group is automatically destroyed when its last
		''' thread is stopped or its last thread group is destroyed.
		''' </summary>
		''' <returns>  <code>true</code> if this thread group is a daemon thread group;
		'''          <code>false</code> otherwise.
		''' @since   JDK1.0 </returns>
		Public Property daemon As Boolean
			Get
				Return daemon
			End Get
			Set(ByVal daemon As Boolean)
				checkAccess()
				Me.daemon = daemon
			End Set
		End Property

		''' <summary>
		''' Tests if this thread group has been destroyed.
		''' </summary>
		''' <returns>  true if this object is destroyed
		''' @since   JDK1.1 </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property destroyed As Boolean
			Get
				Return destroyed
			End Get
		End Property


			Dim ngroupsSnapshot As Integer
			Dim groupsSnapshot As ThreadGroup()
			SyncLock Me
				checkAccess()
				If pri < Thread.MIN_PRIORITY OrElse pri > Thread.MAX_PRIORITY Then Return
				maxPriority = If(parent IsNot Nothing, System.Math.Min(pri, parent.maxPriority), pri)
				ngroupsSnapshot = ngroups
				If groups IsNot Nothing Then
					groupsSnapshot = java.util.Arrays.copyOf(groups, ngroupsSnapshot)
				Else
					groupsSnapshot = Nothing
				End If
			End SyncLock
			For i As Integer = 0 To ngroupsSnapshot - 1
				groupsSnapshot(i).maxPriority = pri
			Next i
		End Sub

		''' <summary>
		''' Tests if this thread group is either the thread group
		''' argument or one of its ancestor thread groups.
		''' </summary>
		''' <param name="g">   a thread group. </param>
		''' <returns>  <code>true</code> if this thread group is the thread group
		'''          argument or one of its ancestor thread groups;
		'''          <code>false</code> otherwise.
		''' @since   JDK1.0 </returns>
		Public Function parentOf(ByVal g As ThreadGroup) As Boolean
			Do While g IsNot Nothing
				If g Is Me Then Return True
				g = g.parent
			Loop
			Return False
		End Function

		''' <summary>
		''' Determines if the currently running thread has permission to
		''' modify this thread group.
		''' <p>
		''' If there is a security manager, its <code>checkAccess</code> method
		''' is called with this thread group as its argument. This may result
		''' in throwing a <code>SecurityException</code>.
		''' </summary>
		''' <exception cref="SecurityException">  if the current thread is not allowed to
		'''               access this thread group. </exception>
		''' <seealso cref=        java.lang.SecurityManager#checkAccess(java.lang.ThreadGroup)
		''' @since      JDK1.0 </seealso>
		Public Sub checkAccess()
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkAccess(Me)
		End Sub

		''' <summary>
		''' Returns an estimate of the number of active threads in this thread
		''' group and its subgroups. Recursively iterates over all subgroups in
		''' this thread group.
		''' 
		''' <p> The value returned is only an estimate because the number of
		''' threads may change dynamically while this method traverses internal
		''' data structures, and might be affected by the presence of certain
		''' system threads. This method is intended primarily for debugging
		''' and monitoring purposes.
		''' </summary>
		''' <returns>  an estimate of the number of active threads in this thread
		'''          group and in any other thread group that has this thread
		'''          group as an ancestor
		''' 
		''' @since   JDK1.0 </returns>
		Public Overridable Function activeCount() As Integer
			Dim result As Integer
			' Snapshot sub-group data so we don't hold this lock
			' while our children are computing.
			Dim ngroupsSnapshot As Integer
			Dim groupsSnapshot As ThreadGroup()
			SyncLock Me
				If destroyed Then Return 0
				result = nthreads
				ngroupsSnapshot = ngroups
				If groups IsNot Nothing Then
					groupsSnapshot = java.util.Arrays.copyOf(groups, ngroupsSnapshot)
				Else
					groupsSnapshot = Nothing
				End If
			End SyncLock
			For i As Integer = 0 To ngroupsSnapshot - 1
				result += groupsSnapshot(i).activeCount()
			Next i
			Return result
		End Function

		''' <summary>
		''' Copies into the specified array every active thread in this
		''' thread group and its subgroups.
		''' 
		''' <p> An invocation of this method behaves in exactly the same
		''' way as the invocation
		''' 
		''' <blockquote>
		''' <seealso cref="#enumerate(Thread[], boolean) enumerate"/>{@code (list, true)}
		''' </blockquote>
		''' </summary>
		''' <param name="list">
		'''         an array into which to put the list of threads
		''' </param>
		''' <returns>  the number of threads put into the array
		''' </returns>
		''' <exception cref="SecurityException">
		'''          if <seealso cref="#checkAccess checkAccess"/> determines that
		'''          the current thread cannot access this thread group
		''' 
		''' @since   JDK1.0 </exception>
		Public Overridable Function enumerate(ByVal list As Thread()) As Integer
			checkAccess()
			Return enumerate(list, 0, True)
		End Function

		''' <summary>
		''' Copies into the specified array every active thread in this
		''' thread group. If {@code recurse} is {@code true},
		''' this method recursively enumerates all subgroups of this
		''' thread group and references to every active thread in these
		''' subgroups are also included. If the array is too short to
		''' hold all the threads, the extra threads are silently ignored.
		''' 
		''' <p> An application might use the <seealso cref="#activeCount activeCount"/>
		''' method to get an estimate of how big the array should be, however
		''' <i>if the array is too short to hold all the threads, the extra threads
		''' are silently ignored.</i>  If it is critical to obtain every active
		''' thread in this thread group, the caller should verify that the returned
		''' int value is strictly less than the length of {@code list}.
		''' 
		''' <p> Due to the inherent race condition in this method, it is recommended
		''' that the method only be used for debugging and monitoring purposes.
		''' </summary>
		''' <param name="list">
		'''         an array into which to put the list of threads
		''' </param>
		''' <param name="recurse">
		'''         if {@code true}, recursively enumerate all subgroups of this
		'''         thread group
		''' </param>
		''' <returns>  the number of threads put into the array
		''' </returns>
		''' <exception cref="SecurityException">
		'''          if <seealso cref="#checkAccess checkAccess"/> determines that
		'''          the current thread cannot access this thread group
		''' 
		''' @since   JDK1.0 </exception>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public int enumerate(Thread list() , boolean recurse)
			checkAccess()
			Return enumerate(list, 0, recurse)

		private Integer enumerate(Thread list() , Integer n, Boolean recurse)
			Dim ngroupsSnapshot As Integer = 0
			Dim groupsSnapshot As ThreadGroup() = Nothing
			SyncLock Me
				If destroyed Then Return 0
				Dim nt As Integer = nthreads
				If nt > list.length - n Then nt = list.length - n
				For i As Integer = 0 To nt - 1
					If threads(i).alive Then
						list(n) = threads(i)
						n += 1
					End If
				Next i
				If recurse Then
					ngroupsSnapshot = ngroups
					If groups IsNot Nothing Then
						groupsSnapshot = java.util.Arrays.copyOf(groups, ngroupsSnapshot)
					Else
						groupsSnapshot = Nothing
					End If
				End If
			End SyncLock
			If recurse Then
				For i As Integer = 0 To ngroupsSnapshot - 1
					n = groupsSnapshot(i).enumerate(list, n, True)
				Next i
			End If
			Return n

		''' <summary>
		''' Returns an estimate of the number of active groups in this
		''' thread group and its subgroups. Recursively iterates over
		''' all subgroups in this thread group.
		''' 
		''' <p> The value returned is only an estimate because the number of
		''' thread groups may change dynamically while this method traverses
		''' internal data structures. This method is intended primarily for
		''' debugging and monitoring purposes.
		''' </summary>
		''' <returns>  the number of active thread groups with this thread group as
		'''          an ancestor
		''' 
		''' @since   JDK1.0 </returns>
		public Integer activeGroupCount()
			Dim ngroupsSnapshot As Integer
			Dim groupsSnapshot As ThreadGroup()
			SyncLock Me
				If destroyed Then Return 0
				ngroupsSnapshot = ngroups
				If groups IsNot Nothing Then
					groupsSnapshot = java.util.Arrays.copyOf(groups, ngroupsSnapshot)
				Else
					groupsSnapshot = Nothing
				End If
			End SyncLock
			Dim n As Integer = ngroupsSnapshot
			For i As Integer = 0 To ngroupsSnapshot - 1
				n += groupsSnapshot(i).activeGroupCount()
			Next i
			Return n

		''' <summary>
		''' Copies into the specified array references to every active
		''' subgroup in this thread group and its subgroups.
		''' 
		''' <p> An invocation of this method behaves in exactly the same
		''' way as the invocation
		''' 
		''' <blockquote>
		''' <seealso cref="#enumerate(ThreadGroup[], boolean) enumerate"/>{@code (list, true)}
		''' </blockquote>
		''' </summary>
		''' <param name="list">
		'''         an array into which to put the list of thread groups
		''' </param>
		''' <returns>  the number of thread groups put into the array
		''' </returns>
		''' <exception cref="SecurityException">
		'''          if <seealso cref="#checkAccess checkAccess"/> determines that
		'''          the current thread cannot access this thread group
		''' 
		''' @since   JDK1.0 </exception>
		public Integer enumerate(ThreadGroup list())
			checkAccess()
			Return enumerate(list, 0, True)

		''' <summary>
		''' Copies into the specified array references to every active
		''' subgroup in this thread group. If {@code recurse} is
		''' {@code true}, this method recursively enumerates all subgroups of this
		''' thread group and references to every active thread group in these
		''' subgroups are also included.
		''' 
		''' <p> An application might use the
		''' <seealso cref="#activeGroupCount activeGroupCount"/> method to
		''' get an estimate of how big the array should be, however <i>if the
		''' array is too short to hold all the thread groups, the extra thread
		''' groups are silently ignored.</i>  If it is critical to obtain every
		''' active subgroup in this thread group, the caller should verify that
		''' the returned int value is strictly less than the length of
		''' {@code list}.
		''' 
		''' <p> Due to the inherent race condition in this method, it is recommended
		''' that the method only be used for debugging and monitoring purposes.
		''' </summary>
		''' <param name="list">
		'''         an array into which to put the list of thread groups
		''' </param>
		''' <param name="recurse">
		'''         if {@code true}, recursively enumerate all subgroups
		''' </param>
		''' <returns>  the number of thread groups put into the array
		''' </returns>
		''' <exception cref="SecurityException">
		'''          if <seealso cref="#checkAccess checkAccess"/> determines that
		'''          the current thread cannot access this thread group
		''' 
		''' @since   JDK1.0 </exception>
		public Integer enumerate(ThreadGroup list() , Boolean recurse)
			checkAccess()
			Return enumerate(list, 0, recurse)

		private Integer enumerate(ThreadGroup list() , Integer n, Boolean recurse)
			Dim ngroupsSnapshot As Integer = 0
			Dim groupsSnapshot As ThreadGroup() = Nothing
			SyncLock Me
				If destroyed Then Return 0
				Dim ng As Integer = ngroups
				If ng > list.length - n Then ng = list.length - n
				If ng > 0 Then
					Array.Copy(groups, 0, list, n, ng)
					n += ng
				End If
				If recurse Then
					ngroupsSnapshot = ngroups
					If groups IsNot Nothing Then
						groupsSnapshot = java.util.Arrays.copyOf(groups, ngroupsSnapshot)
					Else
						groupsSnapshot = Nothing
					End If
				End If
			End SyncLock
			If recurse Then
				For i As Integer = 0 To ngroupsSnapshot - 1
					n = groupsSnapshot(i).enumerate(list, n, True)
				Next i
			End If
			Return n

		''' <summary>
		''' Stops all threads in this thread group.
		''' <p>
		''' First, the <code>checkAccess</code> method of this thread group is
		''' called with no arguments; this may result in a security exception.
		''' <p>
		''' This method then calls the <code>stop</code> method on all the
		''' threads in this thread group and in all of its subgroups.
		''' </summary>
		''' <exception cref="SecurityException">  if the current thread is not allowed
		'''               to access this thread group or any of the threads in
		'''               the thread group. </exception>
		''' <seealso cref=        java.lang.SecurityException </seealso>
		''' <seealso cref=        java.lang.Thread#stop() </seealso>
		''' <seealso cref=        java.lang.ThreadGroup#checkAccess()
		''' @since      JDK1.0 </seealso>
		''' @deprecated    This method is inherently unsafe.  See
		'''     <seealso cref="Thread#stop"/> for details. 
		<Obsolete("   This method is inherently unsafe.  See")> _
		public final  Sub  stop()
			If stopOrSuspend(False) Then Thread.CurrentThread.Abort()

		''' <summary>
		''' Interrupts all threads in this thread group.
		''' <p>
		''' First, the <code>checkAccess</code> method of this thread group is
		''' called with no arguments; this may result in a security exception.
		''' <p>
		''' This method then calls the <code>interrupt</code> method on all the
		''' threads in this thread group and in all of its subgroups.
		''' </summary>
		''' <exception cref="SecurityException">  if the current thread is not allowed
		'''               to access this thread group or any of the threads in
		'''               the thread group. </exception>
		''' <seealso cref=        java.lang.Thread#interrupt() </seealso>
		''' <seealso cref=        java.lang.SecurityException </seealso>
		''' <seealso cref=        java.lang.ThreadGroup#checkAccess()
		''' @since      1.2 </seealso>
		public final  Sub  interrupt()
			Dim ngroupsSnapshot As Integer
			Dim groupsSnapshot As ThreadGroup()
			SyncLock Me
				checkAccess()
				For i As Integer = 0 To nthreads - 1
					threads(i).interrupt()
				Next i
				ngroupsSnapshot = ngroups
				If groups IsNot Nothing Then
					groupsSnapshot = java.util.Arrays.copyOf(groups, ngroupsSnapshot)
				Else
					groupsSnapshot = Nothing
				End If
			End SyncLock
			For i As Integer = 0 To ngroupsSnapshot - 1
				groupsSnapshot(i).interrupt()
			Next i

		''' <summary>
		''' Suspends all threads in this thread group.
		''' <p>
		''' First, the <code>checkAccess</code> method of this thread group is
		''' called with no arguments; this may result in a security exception.
		''' <p>
		''' This method then calls the <code>suspend</code> method on all the
		''' threads in this thread group and in all of its subgroups.
		''' </summary>
		''' <exception cref="SecurityException">  if the current thread is not allowed
		'''               to access this thread group or any of the threads in
		'''               the thread group. </exception>
		''' <seealso cref=        java.lang.Thread#suspend() </seealso>
		''' <seealso cref=        java.lang.SecurityException </seealso>
		''' <seealso cref=        java.lang.ThreadGroup#checkAccess()
		''' @since      JDK1.0 </seealso>
		''' @deprecated    This method is inherently deadlock-prone.  See
		'''     <seealso cref="Thread#suspend"/> for details. 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		<Obsolete("   This method is inherently deadlock-prone.  See")> _
		public final  Sub  suspend()
			If stopOrSuspend(True) Then Thread.CurrentThread.Suspend()

		''' <summary>
		''' Helper method: recursively stops or suspends (as directed by the
		''' boolean argument) all of the threads in this thread group and its
		''' subgroups, except the current thread.  This method returns true
		''' if (and only if) the current thread is found to be in this thread
		''' group or one of its subgroups.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		private Boolean stopOrSuspend(Boolean suspend)
			Dim suicide As Boolean = False
			Dim us As Thread = Thread.CurrentThread
			Dim ngroupsSnapshot As Integer
			Dim groupsSnapshot As ThreadGroup() = Nothing
			SyncLock Me
				checkAccess()
				For i As Integer = 0 To nthreads - 1
					If threads(i) Is us Then
						suicide = True
					ElseIf suspend Then
						threads(i).suspend()
					Else
						threads(i).stop()
					End If
				Next i

				ngroupsSnapshot = ngroups
				If groups IsNot Nothing Then groupsSnapshot = java.util.Arrays.copyOf(groups, ngroupsSnapshot)
			End SyncLock
			Dim i As Integer = 0
			Do While i < ngroupsSnapshot
				suicide = groupsSnapshot(i).stopOrSuspend(suspend) OrElse suicide
				i += 1
			Loop

			Return suicide

		''' <summary>
		''' Resumes all threads in this thread group.
		''' <p>
		''' First, the <code>checkAccess</code> method of this thread group is
		''' called with no arguments; this may result in a security exception.
		''' <p>
		''' This method then calls the <code>resume</code> method on all the
		''' threads in this thread group and in all of its sub groups.
		''' </summary>
		''' <exception cref="SecurityException">  if the current thread is not allowed to
		'''               access this thread group or any of the threads in the
		'''               thread group. </exception>
		''' <seealso cref=        java.lang.SecurityException </seealso>
		''' <seealso cref=        java.lang.Thread#resume() </seealso>
		''' <seealso cref=        java.lang.ThreadGroup#checkAccess()
		''' @since      JDK1.0 </seealso>
		''' @deprecated    This method is used solely in conjunction with
		'''      <tt>Thread.suspend</tt> and <tt>ThreadGroup.suspend</tt>,
		'''       both of which have been deprecated, as they are inherently
		'''       deadlock-prone.  See <seealso cref="Thread#suspend"/> for details. 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		<Obsolete("   This method is used solely in conjunction with")> _
		public final  Sub  resume()
			Dim ngroupsSnapshot As Integer
			Dim groupsSnapshot As ThreadGroup()
			SyncLock Me
				checkAccess()
				For i As Integer = 0 To nthreads - 1
					threads(i).resume()
				Next i
				ngroupsSnapshot = ngroups
				If groups IsNot Nothing Then
					groupsSnapshot = java.util.Arrays.copyOf(groups, ngroupsSnapshot)
				Else
					groupsSnapshot = Nothing
				End If
			End SyncLock
			For i As Integer = 0 To ngroupsSnapshot - 1
				groupsSnapshot(i).resume()
			Next i

		''' <summary>
		''' Destroys this thread group and all of its subgroups. This thread
		''' group must be empty, indicating that all threads that had been in
		''' this thread group have since stopped.
		''' <p>
		''' First, the <code>checkAccess</code> method of this thread group is
		''' called with no arguments; this may result in a security exception.
		''' </summary>
		''' <exception cref="IllegalThreadStateException">  if the thread group is not
		'''               empty or if the thread group has already been destroyed. </exception>
		''' <exception cref="SecurityException">  if the current thread cannot modify this
		'''               thread group. </exception>
		''' <seealso cref=        java.lang.ThreadGroup#checkAccess()
		''' @since      JDK1.0 </seealso>
		public final  Sub  destroy()
			Dim ngroupsSnapshot As Integer
			Dim groupsSnapshot As ThreadGroup()
			SyncLock Me
				checkAccess()
				If destroyed OrElse (nthreads > 0) Then Throw New IllegalThreadStateException
				ngroupsSnapshot = ngroups
				If groups IsNot Nothing Then
					groupsSnapshot = java.util.Arrays.copyOf(groups, ngroupsSnapshot)
				Else
					groupsSnapshot = Nothing
				End If
				If parent IsNot Nothing Then
					destroyed = True
					ngroups = 0
					groups = Nothing
					nthreads = 0
					threads = Nothing
				End If
			End SyncLock
			For i As Integer = 0 To ngroupsSnapshot - 1
				groupsSnapshot(i).destroy()
			Next i
			If parent IsNot Nothing Then parent.remove(Me)

		''' <summary>
		''' Adds the specified Thread group to this group. </summary>
		''' <param name="g"> the specified Thread group to be added </param>
		''' <exception cref="IllegalThreadStateException"> If the Thread group has been destroyed. </exception>
		private final  Sub  add(ThreadGroup g)
			SyncLock Me
				If destroyed Then Throw New IllegalThreadStateException
				If groups Is Nothing Then
					groups = New ThreadGroup(3){}
				ElseIf ngroups = groups.Length Then
					groups = java.util.Arrays.copyOf(groups, ngroups * 2)
				End If
				groups(ngroups) = g

				' This is done last so it doesn't matter in case the
				' thread is killed
				ngroups += 1
			End SyncLock

		''' <summary>
		''' Removes the specified Thread group from this group. </summary>
		''' <param name="g"> the Thread group to be removed </param>
		''' <returns> if this Thread has already been destroyed. </returns>
		private  Sub  remove(ThreadGroup g)
			SyncLock Me
				If destroyed Then Return
				Dim i As Integer = 0
				Do While i < ngroups
					If groups(i) Is g Then
						ngroups -= 1
						Array.Copy(groups, i + 1, groups, i, ngroups - i)
						' Zap dangling reference to the dead group so that
						' the garbage collector will collect it.
						groups(ngroups) = Nothing
						Exit Do
					End If
					i += 1
				Loop
				If nthreads = 0 Then notifyAll()
				If daemon AndAlso (nthreads = 0) AndAlso (nUnstartedThreads = 0) AndAlso (ngroups = 0) Then destroy()
			End SyncLock


		''' <summary>
		''' Increments the count of unstarted threads in the thread group.
		''' Unstarted threads are not added to the thread group so that they
		''' can be collected if they are never started, but they must be
		''' counted so that daemon thread groups with unstarted threads in
		''' them are not destroyed.
		''' </summary>
		void addUnstarted()
			SyncLock Me
				If destroyed Then Throw New IllegalThreadStateException
				nUnstartedThreads += 1
			End SyncLock

		''' <summary>
		''' Adds the specified thread to this thread group.
		''' 
		''' <p> Note: This method is called from both library code
		''' and the Virtual Machine. It is called from VM to add
		''' certain system threads to the system thread group.
		''' </summary>
		''' <param name="t">
		'''         the Thread to be added
		''' </param>
		''' <exception cref="IllegalThreadStateException">
		'''          if the Thread group has been destroyed </exception>
		void add(Thread t)
			SyncLock Me
				If destroyed Then Throw New IllegalThreadStateException
				If threads Is Nothing Then
					threads = New Thread(3){}
				ElseIf nthreads = threads.Length Then
					threads = java.util.Arrays.copyOf(threads, nthreads * 2)
				End If
				threads(nthreads) = t

				' This is done last so it doesn't matter in case the
				' thread is killed
				nthreads += 1

				' The thread is now a fully fledged member of the group, even
				' though it may, or may not, have been started yet. It will prevent
				' the group from being destroyed so the unstarted Threads count is
				' decremented.
				nUnstartedThreads -= 1
			End SyncLock

		''' <summary>
		''' Notifies the group that the thread {@code t} has failed
		''' an attempt to start.
		''' 
		''' <p> The state of this thread group is rolled back as if the
		''' attempt to start the thread has never occurred. The thread is again
		''' considered an unstarted member of the thread group, and a subsequent
		''' attempt to start the thread is permitted.
		''' </summary>
		''' <param name="t">
		'''         the Thread whose start method was invoked </param>
		void threadStartFailed(Thread t)
			SyncLock Me
				remove(t)
				nUnstartedThreads += 1
			End SyncLock

		''' <summary>
		''' Notifies the group that the thread {@code t} has terminated.
		''' 
		''' <p> Destroy the group if all of the following conditions are
		''' true: this is a daemon thread group; there are no more alive
		''' or unstarted threads in the group; there are no subgroups in
		''' this thread group.
		''' </summary>
		''' <param name="t">
		'''         the Thread that has terminated </param>
		void threadTerminated(Thread t)
			SyncLock Me
				remove(t)

				If nthreads = 0 Then notifyAll()
				If daemon AndAlso (nthreads = 0) AndAlso (nUnstartedThreads = 0) AndAlso (ngroups = 0) Then destroy()
			End SyncLock

		''' <summary>
		''' Removes the specified Thread from this group. Invoking this method
		''' on a thread group that has been destroyed has no effect.
		''' </summary>
		''' <param name="t">
		'''         the Thread to be removed </param>
		private  Sub  remove(Thread t)
			SyncLock Me
				If destroyed Then Return
				Dim i As Integer = 0
				Do While i < nthreads
					If threads(i) Is t Then
						nthreads -= 1
						Array.Copy(threads, i + 1, threads, i, nthreads - i)
						' Zap dangling reference to the dead thread so that
						' the garbage collector will collect it.
						threads(nthreads) = Nothing
						Exit Do
					End If
					i += 1
				Loop
			End SyncLock

		''' <summary>
		''' Prints information about this thread group to the standard
		''' output. This method is useful only for debugging.
		''' 
		''' @since   JDK1.0
		''' </summary>
		public  Sub  list()
			list(System.out, 0)
		void list(java.io.PrintStream out, Integer indent)
			Dim ngroupsSnapshot As Integer
			Dim groupsSnapshot As ThreadGroup()
			SyncLock Me
				For j As Integer = 0 To indent - 1
					out.print(" ")
				Next j
				out.println(Me)
				indent += 4
				For i As Integer = 0 To nthreads - 1
					For j As Integer = 0 To indent - 1
						out.print(" ")
					Next j
					out.println(threads(i))
				Next i
				ngroupsSnapshot = ngroups
				If groups IsNot Nothing Then
					groupsSnapshot = java.util.Arrays.copyOf(groups, ngroupsSnapshot)
				Else
					groupsSnapshot = Nothing
				End If
			End SyncLock
			For i As Integer = 0 To ngroupsSnapshot - 1
				groupsSnapshot(i).list(out, indent)
			Next i

		''' <summary>
		''' Called by the Java Virtual Machine when a thread in this
		''' thread group stops because of an uncaught exception, and the thread
		''' does not have a specific <seealso cref="Thread.UncaughtExceptionHandler"/>
		''' installed.
		''' <p>
		''' The <code>uncaughtException</code> method of
		''' <code>ThreadGroup</code> does the following:
		''' <ul>
		''' <li>If this thread group has a parent thread group, the
		'''     <code>uncaughtException</code> method of that parent is called
		'''     with the same two arguments.
		''' <li>Otherwise, this method checks to see if there is a
		'''     {@link Thread#getDefaultUncaughtExceptionHandler default
		'''     uncaught exception handler} installed, and if so, its
		'''     <code>uncaughtException</code> method is called with the same
		'''     two arguments.
		''' <li>Otherwise, this method determines if the <code>Throwable</code>
		'''     argument is an instance of <seealso cref="ThreadDeath"/>. If so, nothing
		'''     special is done. Otherwise, a message containing the
		'''     thread's name, as returned from the thread's {@link
		'''     Thread#getName getName} method, and a stack backtrace,
		'''     using the <code>Throwable</code>'s {@link
		'''     Throwable#printStackTrace printStackTrace} method, is
		'''     printed to the <seealso cref="System#err standard error stream"/>.
		''' </ul>
		''' <p>
		''' Applications can override this method in subclasses of
		''' <code>ThreadGroup</code> to provide alternative handling of
		''' uncaught exceptions.
		''' </summary>
		''' <param name="t">   the thread that is about to exit. </param>
		''' <param name="e">   the uncaught exception.
		''' @since   JDK1.0 </param>
		public  Sub  uncaughtException(Thread t, Throwable e)
			If parent IsNot Nothing Then
				parent.uncaughtException(t, e)
			Else
				Dim ueh As Thread.UncaughtExceptionHandler = Thread.defaultUncaughtExceptionHandler
				If ueh IsNot Nothing Then
					ueh.uncaughtException(t, e)
				ElseIf Not(TypeOf e Is ThreadDeath) Then
					Console.Error.Write("Exception in thread """ & t.name & """ ")
					e.printStackTrace(System.err)
				End If
			End If

		''' <summary>
		''' Used by VM to control lowmem implicit suspension.
		''' </summary>
		''' <param name="b"> boolean to allow or disallow suspension </param>
		''' <returns> true on success
		''' @since   JDK1.1 </returns>
		''' @deprecated The definition of this call depends on <seealso cref="#suspend"/>,
		'''             which is deprecated.  Further, the behavior of this call
		'''             was never specified. 
		<Obsolete("The definition of this call depends on <seealso cref="#suspend"/>,")> _
		public Boolean allowThreadSuspension(Boolean b)
			Me.vmAllowSuspension = b
			If Not b Then sun.misc.VM.unsuspendSomeThreads()
			Return True

		''' <summary>
		''' Returns a string representation of this Thread group.
		''' </summary>
		''' <returns>  a string representation of this thread group.
		''' @since   JDK1.0 </returns>
		public String ToString()
			Return Me.GetType().name & "[name=" & name & ",maxpri=" & maxPriority & "]"
	End Class

End Namespace