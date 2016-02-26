Imports System

'
' * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.management.remote


	''' <summary>
	''' <p>Result of a query for buffered notifications.  Notifications in
	''' a notification buffer have positive, monotonically increasing
	''' sequence numbers.  The result of a notification query contains the
	''' following elements:</p>
	''' 
	''' <ul>
	''' 
	''' <li>The sequence number of the earliest notification still in
	''' the buffer.
	''' 
	''' <li>The sequence number of the next notification available for
	''' querying.  This will be the starting sequence number for the next
	''' notification query.
	''' 
	''' <li>An array of (Notification,listenerID) pairs corresponding to
	''' the returned notifications and the listeners they correspond to.
	''' 
	''' </ul>
	''' 
	''' <p>It is possible for the <code>nextSequenceNumber</code> to be less
	''' than the <code>earliestSequenceNumber</code>.  This signifies that
	''' notifications between the two might have been lost.</p>
	''' 
	''' @since 1.5
	''' </summary>
	<Serializable> _
	Public Class NotificationResult

		Private Const serialVersionUID As Long = 1191800228721395279L

		''' <summary>
		''' <p>Constructs a notification query result.</p>
		''' </summary>
		''' <param name="earliestSequenceNumber"> the sequence number of the
		''' earliest notification still in the buffer. </param>
		''' <param name="nextSequenceNumber"> the sequence number of the next
		''' notification available for querying. </param>
		''' <param name="targetedNotifications"> the notifications resulting from
		''' the query, and the listeners they correspond to.  This array
		''' can be empty.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if
		''' <code>targetedNotifications</code> is null or if
		''' <code>earliestSequenceNumber</code> or
		''' <code>nextSequenceNumber</code> is negative. </exception>
		Public Sub New(ByVal earliestSequenceNumber As Long, ByVal nextSequenceNumber As Long, ByVal targetedNotifications As TargetedNotification())
			validate(targetedNotifications, earliestSequenceNumber, nextSequenceNumber)
			Me.earliestSequenceNumber = earliestSequenceNumber
			Me.nextSequenceNumber = nextSequenceNumber
			Me.targetedNotifications = (If(targetedNotifications.Length = 0, targetedNotifications, targetedNotifications.clone()))
		End Sub

		''' <summary>
		''' Returns the sequence number of the earliest notification still
		''' in the buffer.
		''' </summary>
		''' <returns> the sequence number of the earliest notification still
		''' in the buffer. </returns>
		Public Overridable Property earliestSequenceNumber As Long
			Get
				Return earliestSequenceNumber
			End Get
		End Property

		''' <summary>
		''' Returns the sequence number of the next notification available
		''' for querying.
		''' </summary>
		''' <returns> the sequence number of the next notification available
		''' for querying. </returns>
		Public Overridable Property nextSequenceNumber As Long
			Get
				Return nextSequenceNumber
			End Get
		End Property

		''' <summary>
		''' Returns the notifications resulting from the query, and the
		''' listeners they correspond to.
		''' </summary>
		''' <returns> the notifications resulting from the query, and the
		''' listeners they correspond to.  This array can be empty. </returns>
		Public Overridable Property targetedNotifications As TargetedNotification()
			Get
				Return If(targetedNotifications.Length = 0, targetedNotifications, targetedNotifications.clone())
			End Get
		End Property

		''' <summary>
		''' Returns a string representation of the object.  The result
		''' should be a concise but informative representation that is easy
		''' for a person to read.
		''' </summary>
		''' <returns> a string representation of the object. </returns>
		Public Overrides Function ToString() As String
			Return "NotificationResult: earliest=" & earliestSequenceNumber & "; next=" & nextSequenceNumber & "; nnotifs=" & targetedNotifications.Length
		End Function

		Private Sub readObject(ByVal ois As java.io.ObjectInputStream)
			ois.defaultReadObject()
			Try
				validate(Me.targetedNotifications, Me.earliestSequenceNumber, Me.nextSequenceNumber)

				Me.targetedNotifications = If(Me.targetedNotifications.Length = 0, Me.targetedNotifications, Me.targetedNotifications.clone())
			Catch e As System.ArgumentException
				Throw New java.io.InvalidObjectException(e.Message)
			End Try
		End Sub

		Private earliestSequenceNumber As Long
		Private nextSequenceNumber As Long
		Private targetedNotifications As TargetedNotification()

		Private Shared Sub validate(ByVal targetedNotifications As TargetedNotification(), ByVal earliestSequenceNumber As Long, ByVal nextSequenceNumber As Long)
			If targetedNotifications Is Nothing Then
				Const msg As String = "Notifications null"
				Throw New System.ArgumentException(msg)
			End If

			If earliestSequenceNumber < 0 OrElse nextSequenceNumber < 0 Then Throw New System.ArgumentException("Bad sequence numbers")
	'         We used to check nextSequenceNumber >= earliestSequenceNumber
	'           here.  But in fact the opposite can legitimately be true if
	'           notifications have been lost.  
		End Sub
	End Class

End Namespace