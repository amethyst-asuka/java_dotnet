Imports System
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


    ''' <summary>
    ''' Abstract base class for reference objects.  This class defines the
    ''' operations common to all reference objects.  Because reference objects are
    ''' implemented in close cooperation with the garbage collector, this class may
    ''' not be subclassed directly.
    ''' 
    ''' @author   Mark Reinhold
    ''' @since    1.2
    ''' </summary>

    Public MustInherit Class Reference(Of T) : Inherits java.lang.Object

        '     A Reference instance is in one of four possible internal states:
        '     *
        '     *     Active: Subject to special treatment by the garbage collector.  Some
        '     *     time after the collector detects that the reachability of the
        '     *     referent has changed to the appropriate state, it changes the
        '     *     instance's state to either Pending or Inactive, depending upon
        '     *     whether or not the instance was registered with a queue when it was
        '     *     created.  In the former case it also adds the instance to the
        '     *     pending-Reference list.  Newly-created instances are Active.
        '     *
        '     *     Pending: An element of the pending-Reference list, waiting to be
        '     *     enqueued by the Reference-handler thread.  Unregistered instances
        '     *     are never in this state.
        '     *
        '     *     Enqueued: An element of the queue with which the instance was
        '     *     registered when it was created.  When an instance is removed from
        '     *     its ReferenceQueue, it is made Inactive.  Unregistered instances are
        '     *     never in this state.
        '     *
        '     *     Inactive: Nothing more to do.  Once an instance becomes Inactive its
        '     *     state will never change again.
        '     *
        '     * The state is encoded in the queue and next fields as follows:
        '     *
        '     *     Active: queue = ReferenceQueue with which instance is registered, or
        '     *     ReferenceQueue.NULL if it was not registered with a queue; next =
        '     *     null.
        '     *
        '     *     Pending: queue = ReferenceQueue with which instance is registered;
        '     *     next = this
        '     *
        '     *     Enqueued: queue = ReferenceQueue.ENQUEUED; next = Following instance
        '     *     in queue, or this if at end of list.
        '     *
        '     *     Inactive: queue = ReferenceQueue.NULL; next = this.
        '     *
        '     * With this scheme the collector need only examine the next field in order
        '     * to determine whether a Reference instance requires special treatment: If
        '     * the next field is null then the instance is active; if it is non-null,
        '     * then the collector should treat the instance normally.
        '     *
        '     * To ensure that a concurrent collector can discover active Reference
        '     * objects without interfering with application threads that may apply
        '     * the enqueue() method to those objects, collectors should link
        '     * discovered objects through the discovered field. The discovered
        '     * field is also used for linking Reference objects in the pending list.
        '     

        Private referent As T ' Treated specially by GC

        'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
        'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
        'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
        Friend queue As ReferenceQueue(Of T)

        '     When active:   NULL
        '     *     pending:   this
        '     *    Enqueued:   next reference in queue (or this if last)
        '     *    Inactive:   this
        '     
        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        Friend [next] As Reference

        '     When active:   next element in a discovered reference list maintained by GC (or this if last)
        '     *     pending:   next element in the pending list (or null if last)
        '     *   otherwise:   NULL
        '     
        <NonSerialized>
        Private discovered As Reference(Of T) ' used by VM


        '     Object used to synchronize with the garbage collector.  The collector
        '     * must acquire this lock at the beginning of each collection cycle.  It is
        '     * therefore critical that any code holding this lock complete as quickly
        '     * as possible, allocate no new objects, and avoid calling user code.
        '     
        Private Class Lock
        End Class
        Private Shared lock As New Lock


        '     List of References waiting to be enqueued.  The collector adds
        '     * References to this list, while the Reference-handler thread removes
        '     * them.  This list is protected by the above lock object. The
        '     * list uses the discovered field to link its elements.
        '     
        Private Shared pending As Reference(Of Object) = Nothing

        '     High-priority thread to enqueue pending References
        '     
        Private Class ReferenceHandler
            Inherits Thread

            Friend Sub New(ByVal g As ThreadGroup, ByVal name As String)
                MyBase.New(g, name)
            End Sub

            Public Overrides Sub run()
                Do
                    Dim r As Reference(Of Object)
                    SyncLock lock
                        If pending IsNot Nothing Then
                            r = pending
                            pending = r.discovered
                            r.discovered = Nothing
                        Else
                            ' The waiting on the lock may cause an OOME because it may try to allocate
                            ' exception objects, so also catch OOME here to avoid silent exit of the
                            ' reference handler thread.
                            '
                            ' Explicitly define the order of the two exceptions we catch here
                            ' when waiting for the lock.
                            '
                            ' We do not want to try to potentially load the InterruptedException class
                            ' (which would be done if this was its first use, and InterruptedException
                            ' were checked first) in this situation.
                            '
                            ' This may lead to the VM not ever trying to load the InterruptedException
                            ' class again.
                            Try
                                Try
                                    lock.wait()
                                Catch x As OutOfMemoryError
                                End Try
                            Catch x As InterruptedException
                            End Try
                            Continue Do
                        End If
                    End SyncLock

                    ' Fast path for cleaners
                    If TypeOf r Is sun.misc.Cleaner Then
                        CType(r, sun.misc.Cleaner).clean()
                        Continue Do
                    End If

                    Dim q As ReferenceQueue(Of Object) = r.queue
                    If q IsNot ReferenceQueue.NULL Then q.enqueue(r)
                Loop
            End Sub
        End Class

        Shared Sub New()
            Dim tg As ThreadGroup = Thread.currentThread.threadGroup
            Dim tgn As ThreadGroup = tg
            Do While tgn IsNot Nothing

                tg = tgn
                tgn = tg.parent
            Loop
            Dim handler As Thread = New ReferenceHandler(tg, "Reference Handler")
            '         If there were a special system-only priority greater than
            '         * MAX_PRIORITY, it would be used here
            '         
            handler.priority = Thread.MAX_PRIORITY
            handler.daemon = True
            handler.start()
        End Sub


        ' -- Referent accessor and setters -- 

        ''' <summary>
        ''' Returns this reference object's referent.  If this reference object has
        ''' been cleared, either by the program or by the garbage collector, then
        ''' this method returns <code>null</code>.
        ''' </summary>
        ''' <returns>   The object to which this reference refers, or
        '''           <code>null</code> if this reference object has been cleared </returns>
        Public Overridable Function [get]() As T
            Return Me.referent
        End Function

        ''' <summary>
        ''' Clears this reference object.  Invoking this method will not cause this
        ''' object to be enqueued.
        ''' 
        ''' <p> This method is invoked only by Java code; when the garbage collector
        ''' clears references it does so directly, without invoking this method.
        ''' </summary>
        Public Overridable Sub clear()
            Me.referent = Nothing
        End Sub


        ' -- Queue operations -- 

        ''' <summary>
        ''' Tells whether or not this reference object has been enqueued, either by
        ''' the program or by the garbage collector.  If this reference object was
        ''' not registered with a queue when it was created, then this method will
        ''' always return <code>false</code>.
        ''' </summary>
        ''' <returns>   <code>true</code> if and only if this reference object has
        '''           been enqueued </returns>
        Public Overridable Property enqueued As Boolean
            Get
                Return (Me.queue Is ReferenceQueue.ENQUEUED)
            End Get
        End Property

        ''' <summary>
        ''' Adds this reference object to the queue with which it is registered,
        ''' if any.
        ''' 
        ''' <p> This method is invoked only by Java code; when the garbage collector
        ''' enqueues references it does so directly, without invoking this method.
        ''' </summary>
        ''' <returns>   <code>true</code> if this reference object was successfully
        '''           enqueued; <code>false</code> if it was already enqueued or if
        '''           it was not registered with a queue when it was created </returns>
        Public Overridable Function enqueue() As Boolean
            Return Me.queue.enqueue(Me)
        End Function


        ' -- Constructors -- 

        Friend Sub New(ByVal referent As T)
            Me.New(referent, Nothing)
        End Sub

        'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
        Friend Sub New(ByVal referent As T, ByVal queue As ReferenceQueue(Of T))
            Me.referent = referent
            Me.queue = If(queue Is Nothing, ReferenceQueue.NULL, queue)
        End Sub

    End Class

End Namespace