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
	''' <p>A (Notification, Listener ID) pair.</p>
	''' <p>This class is used to associate an emitted notification
	'''    with the listener ID to which it is targeted.</p>
	''' 
	''' @since 1.5
	''' </summary>
	<Serializable> _
	Public Class TargetedNotification

		Private Const serialVersionUID As Long = 7676132089779300926L

	' If we replace Integer with int...
	'     /**
	'      * <p>Constructs a <code>TargetedNotification</code> object.  The
	'      * object contains a pair (Notification, Listener ID).
	'      * The Listener ID identifies the client listener to which that
	'      * notification is targeted. The client listener ID is one
	'      * previously returned by the connector server in response to an
	'      * <code>addNotificationListener</code> request.</p>
	'      * @param notification Notification emitted from the MBean server.
	'      * @param listenerID   The ID of the listener to which this
	'      *        notification is targeted.
	'      */
	'     public TargetedNotification(Notification notification,
	'                              int listenerID) {
	'      this.notif = notification;
	'      this.id = listenerID;
	'     }

		''' <summary>
		''' <p>Constructs a <code>TargetedNotification</code> object.  The
		''' object contains a pair (Notification, Listener ID).
		''' The Listener ID identifies the client listener to which that
		''' notification is targeted. The client listener ID is one
		''' previously returned by the connector server in response to an
		''' <code>addNotificationListener</code> request.</p> </summary>
		''' <param name="notification"> Notification emitted from the MBean server. </param>
		''' <param name="listenerID">   The ID of the listener to which this
		'''        notification is targeted. </param>
		''' <exception cref="IllegalArgumentException"> if the <var>listenerID</var>
		'''        or <var>notification</var> is null. </exception>
		Public Sub New(ByVal ___notification As javax.management.Notification, ByVal listenerID As Integer?)
			validate(___notification, listenerID)
			' If we replace integer with int...
			' this(notification,intValue(listenerID));
			Me.notif = ___notification
			Me.id = listenerID
		End Sub

		''' <summary>
		''' <p>The emitted notification.</p>
		''' </summary>
		''' <returns> The notification. </returns>
		Public Overridable Property notification As javax.management.Notification
			Get
				Return notif
			End Get
		End Property

		''' <summary>
		''' <p>The ID of the listener to which the notification is
		'''    targeted.</p>
		''' </summary>
		''' <returns> The listener ID. </returns>
		Public Overridable Property listenerID As Integer?
			Get
				Return id
			End Get
		End Property

		''' <summary>
		''' Returns a textual representation of this Targeted Notification.
		''' </summary>
		''' <returns> a String representation of this Targeted Notification.
		'''  </returns>
		Public Overrides Function ToString() As String
			Return "{" & notif & ", " & id & "}"
		End Function

		''' <summary>
		''' @serial A notification to transmit to the other side. </summary>
		''' <seealso cref= #getNotification()
		'''  </seealso>
		Private notif As javax.management.Notification
		''' <summary>
		''' @serial The ID of the listener to which the notification is
		'''         targeted. </summary>
		''' <seealso cref= #getListenerID()
		'''  </seealso>
		Private id As Integer?
		'private final int id;

	' Needed if we use int instead of Integer...
	'     private static int intValue(Integer id) {
	'      if (id == null) throw new
	'          IllegalArgumentException("Invalid listener ID: null");
	'      return id.intValue();
	'     }

		Private Sub readObject(ByVal ois As java.io.ObjectInputStream)
			ois.defaultReadObject()
			Try
				validate(Me.notif, Me.id)
			Catch e As System.ArgumentException
				Throw New java.io.InvalidObjectException(e.Message)
			End Try
		End Sub

		Private Shared Sub validate(ByVal notif As javax.management.Notification, ByVal id As Integer?)
			If notif Is Nothing Then Throw New System.ArgumentException("Invalid notification: null")
			If id Is Nothing Then Throw New System.ArgumentException("Invalid listener ID: null")
		End Sub
	End Class

End Namespace