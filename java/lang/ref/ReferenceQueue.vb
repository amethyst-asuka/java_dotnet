Imports System.Diagnostics

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
    ''' Reference queues, to which registered reference objects are appended by the
    ''' garbage collector after the appropriate reachability changes are detected.
    ''' 
    ''' @author   Mark Reinhold
    ''' @since    1.2
    ''' </summary>

    Public Class ReferenceQueue(Of T)

        ''' <summary>
        ''' Constructs a new reference-object queue.
        ''' </summary>
        Public Sub New()
        End Sub

        Private Class Null(Of S)
            Inherits ReferenceQueue(Of S)

            Friend Overridable Function enqueue(Of T1 As S)(ByVal r As Reference(Of T1)) As Boolean
                Return False
            End Function
        End Class

        Friend Shared NULL As ReferenceQueue(Of Object) = New Null(Of Object)
        Friend Shared ENQUEUED As ReferenceQueue(Of Object) = New Null(Of Object)

        Private Class Lock : Inherits java.lang.Object
        End Class
        Private _lock As New Lock
        Private head As Reference(Of T) = Nothing
        Private queueLength As Long = 0

        Friend Overridable Function enqueue(ByVal r As Reference(Of T)) As Boolean ' Called only by Reference class
            SyncLock _lock
                ' Check that since getting the lock this reference hasn't already been
                ' enqueued (and even then removed)
                'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
                Dim queue As ReferenceQueue(Of T) = r.queue
                If (queue Is NULL) OrElse (queue Is ENQUEUED) Then Return False
                Debug.Assert(queue Is Me)
                r.queue = ENQUEUED
                r.next = If(head Is Nothing, r, head)
                head = r
                queueLength += 1
                If TypeOf r Is FinalReference Then sun.misc.VM.addFinalRefCount(1)
                _lock.notifyAll()
                Return True
            End SyncLock
        End Function

        'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
        Private Function reallyPoll() As Reference(Of T) ' Must hold lock
            'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
            Dim r As Reference(Of T) = head
            If r IsNot Nothing Then
                head = If(r.next Is r, Nothing, r.next) ' Unchecked due to the next field having a raw type in Reference
                r.queue = NULL
                r.next = r
                queueLength -= 1
                If TypeOf r Is FinalReference Then sun.misc.VM.addFinalRefCount(-1)
                Return r
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' Polls this queue to see if a reference object is available.  If one is
        ''' available without further delay then it is removed from the queue and
        ''' returned.  Otherwise this method immediately returns <tt>null</tt>.
        ''' </summary>
        ''' <returns>  A reference object, if one was immediately available,
        '''          otherwise <code>null</code> </returns>
        'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
        Public Overridable Function poll() As Reference(Of T)
            If head Is Nothing Then Return Nothing
            SyncLock _lock
                Return reallyPoll()
            End SyncLock
        End Function

        ''' <summary>
        ''' Removes the next reference object in this queue, blocking until either
        ''' one becomes available or the given timeout period expires.
        ''' 
        ''' <p> This method does not offer real-time guarantees: It schedules the
        ''' timeout as if by invoking the <seealso cref="Object#wait(long)"/> method.
        ''' </summary>
        ''' <param name="timeout">  If positive, block for up to <code>timeout</code>
        '''                  milliseconds while waiting for a reference to be
        '''                  added to this queue.  If zero, block indefinitely.
        ''' </param>
        ''' <returns>  A reference object, if one was available within the specified
        '''          timeout period, otherwise <code>null</code>
        ''' </returns>
        ''' <exception cref="IllegalArgumentException">
        '''          If the value of the timeout argument is negative
        ''' </exception>
        ''' <exception cref="InterruptedException">
        '''          If the timeout wait is interrupted </exception>
        'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
        Public Overridable Function remove(ByVal timeout As Long) As Reference(Of T)
            If timeout < 0 Then Throw New IllegalArgumentException("Negative timeout value")
            SyncLock _lock
                'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
                Dim r As Reference(Of T) = reallyPoll()
                If r IsNot Nothing Then Return r
                Dim start As Long = If(timeout = 0, 0, System.nanoTime())
                Do
                    _lock.wait(timeout)
                    r = reallyPoll()
                    If r IsNot Nothing Then Return r
                    If timeout <> 0 Then
                        Dim [end] As Long = System.nanoTime()
                        timeout -= ([end] - start) \ 1000000
                        If timeout <= 0 Then Return Nothing
                        start = [end]
                    End If
                Loop
            End SyncLock
        End Function

        ''' <summary>
        ''' Removes the next reference object in this queue, blocking until one
        ''' becomes available.
        ''' </summary>
        ''' <returns> A reference object, blocking until one becomes available </returns>
        ''' <exception cref="InterruptedException">  If the wait is interrupted </exception>
        Public Overridable Function remove() As Reference(Of T)
            Return remove(0)
        End Function
    End Class

End Namespace