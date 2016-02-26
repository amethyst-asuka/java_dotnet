Imports System

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

Namespace javax.management




	''' <summary>
	''' <p>The Notification class represents a notification emitted by an
	''' MBean.  It contains a reference to the source MBean: if the
	''' notification has been forwarded through the MBean server, and the
	''' original source of the notification was a reference to the emitting
	''' MBean object, then the MBean server replaces it by the MBean's
	''' ObjectName.  If the listener has registered directly with the
	''' MBean, this is either the object name or a direct reference to the
	''' MBean.</p>
	''' 
	''' <p>It is strongly recommended that notification senders use the
	''' object name rather than a reference to the MBean object as the
	''' source.</p>
	''' 
	''' <p>The <b>serialVersionUID</b> of this class is <code>-7516092053498031989L</code>.
	''' 
	''' @since 1.5
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class Notification
		Inherits java.util.EventObject ' serialVersionUID is not constant

		' Serialization compatibility stuff:
		' Two serial forms are supported in this class. The selected form depends
		' on system property "jmx.serial.form":
		'  - "1.0" for JMX 1.0
		'  - any other value for JMX 1.1 and higher
		'
		' Serial version for old serial form
		Private Const oldSerialVersionUID As Long = 1716977971058914352L
		'
		' Serial version for new serial form
		Private Const newSerialVersionUID As Long = -7516092053498031989L
		'
		' Serializable fields in old serial form
		Private Shared ReadOnly oldSerialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("message", GetType(String)), New java.io.ObjectStreamField("sequenceNumber", Long.TYPE), New java.io.ObjectStreamField("source", GetType(Object)), New java.io.ObjectStreamField("sourceObjectName", GetType(ObjectName)), New java.io.ObjectStreamField("timeStamp", Long.TYPE), New java.io.ObjectStreamField("type", GetType(String)), New java.io.ObjectStreamField("userData", GetType(Object)) }
		'
		' Serializable fields in new serial form
		Private Shared ReadOnly newSerialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("message", GetType(String)), New java.io.ObjectStreamField("sequenceNumber", Long.TYPE), New java.io.ObjectStreamField("source", GetType(Object)), New java.io.ObjectStreamField("timeStamp", Long.TYPE), New java.io.ObjectStreamField("type", GetType(String)), New java.io.ObjectStreamField("userData", GetType(Object)) }
		'
		' Actual serial version and serial form
		Private Shared ReadOnly serialVersionUID As Long
		''' <summary>
		''' @serialField type String The notification type.
		'''              A string expressed in a dot notation similar to Java properties.
		'''              An example of a notification type is network.alarm.router
		''' @serialField sequenceNumber long The notification sequence number.
		'''              A serial number which identify particular instance
		'''              of notification in the context of the notification source.
		''' @serialField timeStamp long The notification timestamp.
		'''              Indicating when the notification was generated
		''' @serialField userData Object The notification user data.
		'''              Used for whatever other data the notification
		'''              source wishes to communicate to its consumers
		''' @serialField message String The notification message.
		''' @serialField source Object The object on which the notification initially occurred.
		''' </summary>
		Private Shared ReadOnly serialPersistentFields As java.io.ObjectStreamField()
		Private Shared compat As Boolean = False
		Shared Sub New()
			Try
				Dim act As New com.sun.jmx.mbeanserver.GetPropertyAction("jmx.serial.form")
				Dim form As String = java.security.AccessController.doPrivileged(act)
				compat = (form IsNot Nothing AndAlso form.Equals("1.0"))
			Catch e As Exception
				' OK: exception means no compat with 1.0, too bad
			End Try
			If compat Then
				serialPersistentFields = oldSerialPersistentFields
				serialVersionUID = oldSerialVersionUID
			Else
				serialPersistentFields = newSerialPersistentFields
				serialVersionUID = newSerialVersionUID
			End If
		End Sub
		'
		' END Serialization compatibility stuff

		''' <summary>
		''' @serial The notification type.
		'''         A string expressed in a dot notation similar to Java properties.
		'''         An example of a notification type is network.alarm.router
		''' </summary>
		Private type As String

		''' <summary>
		''' @serial The notification sequence number.
		'''         A serial number which identify particular instance
		'''         of notification in the context of the notification source.
		''' </summary>
		Private sequenceNumber As Long

		''' <summary>
		''' @serial The notification timestamp.
		'''         Indicating when the notification was generated
		''' </summary>
		Private timeStamp As Long

		''' <summary>
		''' @serial The notification user data.
		'''         Used for whatever other data the notification
		'''         source wishes to communicate to its consumers
		''' </summary>
		Private userData As Object = Nothing

		''' <summary>
		''' @serial The notification message.
		''' </summary>
		Private message As String = ""

		''' <summary>
		''' <p>This field hides the <seealso cref="EventObject#source"/> field in the
		''' parent class to make it non-transient and therefore part of the
		''' serialized form.</p>
		''' 
		''' @serial The object on which the notification initially occurred.
		''' </summary>
		Protected Friend source As Object = Nothing


		''' <summary>
		''' Creates a Notification object.
		''' The notification timeStamp is set to the current date.
		''' </summary>
		''' <param name="type"> The notification type. </param>
		''' <param name="source"> The notification source. </param>
		''' <param name="sequenceNumber"> The notification sequence number within the source object.
		'''  </param>
		Public Sub New(ByVal type As String, ByVal source As Object, ByVal sequenceNumber As Long)
			MyBase.New(source)
			Me.source = source
			Me.type = type
			Me.sequenceNumber = sequenceNumber
			Me.timeStamp = (DateTime.Now).time
		End Sub

		''' <summary>
		''' Creates a Notification object.
		''' The notification timeStamp is set to the current date.
		''' </summary>
		''' <param name="type"> The notification type. </param>
		''' <param name="source"> The notification source. </param>
		''' <param name="sequenceNumber"> The notification sequence number within the source object. </param>
		''' <param name="message"> The detailed message.
		'''  </param>
		Public Sub New(ByVal type As String, ByVal source As Object, ByVal sequenceNumber As Long, ByVal message As String)
			MyBase.New(source)
			Me.source = source
			Me.type = type
			Me.sequenceNumber = sequenceNumber
			Me.timeStamp = (DateTime.Now).time
			Me.message = message
		End Sub

		''' <summary>
		''' Creates a Notification object.
		''' </summary>
		''' <param name="type"> The notification type. </param>
		''' <param name="source"> The notification source. </param>
		''' <param name="sequenceNumber"> The notification sequence number within the source object. </param>
		''' <param name="timeStamp"> The notification emission date.
		'''  </param>
		Public Sub New(ByVal type As String, ByVal source As Object, ByVal sequenceNumber As Long, ByVal timeStamp As Long)
			MyBase.New(source)
			Me.source = source
			Me.type = type
			Me.sequenceNumber = sequenceNumber
			Me.timeStamp = timeStamp
		End Sub

		''' <summary>
		''' Creates a Notification object.
		''' </summary>
		''' <param name="type"> The notification type. </param>
		''' <param name="source"> The notification source. </param>
		''' <param name="sequenceNumber"> The notification sequence number within the source object. </param>
		''' <param name="timeStamp"> The notification emission date. </param>
		''' <param name="message"> The detailed message.
		'''  </param>
		Public Sub New(ByVal type As String, ByVal source As Object, ByVal sequenceNumber As Long, ByVal timeStamp As Long, ByVal message As String)
			MyBase.New(source)
			Me.source = source
			Me.type = type
			Me.sequenceNumber = sequenceNumber
			Me.timeStamp = timeStamp
			Me.message = message
		End Sub

		''' <summary>
		''' Sets the source.
		''' </summary>
		''' <param name="source"> the new source for this object.
		''' </param>
		''' <seealso cref= EventObject#getSource </seealso>
		Public Overridable Property source As Object
			Set(ByVal source As Object)
				MyBase.source = source
				Me.source = source
			End Set
		End Property

		''' <summary>
		''' Get the notification sequence number.
		''' </summary>
		''' <returns> The notification sequence number within the source object. It's a serial number
		''' identifying a particular instance of notification in the context of the notification source.
		''' The notification model does not assume that notifications will be received in the same order
		''' that they are sent. The sequence number helps listeners to sort received notifications.
		''' </returns>
		''' <seealso cref= #setSequenceNumber </seealso>
		Public Overridable Property sequenceNumber As Long
			Get
				Return sequenceNumber
			End Get
			Set(ByVal sequenceNumber As Long)
				Me.sequenceNumber = sequenceNumber
			End Set
		End Property


		''' <summary>
		''' Get the notification type.
		''' </summary>
		''' <returns> The notification type. It's a string expressed in a dot notation
		''' similar to Java properties. It is recommended that the notification type
		''' should follow the reverse-domain-name convention used by Java package
		''' names.  An example of a notification type is com.example.alarm.router. </returns>
		Public Overridable Property type As String
			Get
				Return type
			End Get
		End Property

		''' <summary>
		''' Get the notification timestamp.
		''' </summary>
		''' <returns> The notification timestamp.
		''' </returns>
		''' <seealso cref= #setTimeStamp </seealso>
		Public Overridable Property timeStamp As Long
			Get
				Return timeStamp
			End Get
			Set(ByVal timeStamp As Long)
				Me.timeStamp = timeStamp
			End Set
		End Property


		''' <summary>
		''' Get the notification message.
		''' </summary>
		''' <returns> The message string of this notification object.
		'''  </returns>
		Public Overridable Property message As String
			Get
				Return message
			End Get
		End Property

		''' <summary>
		''' Get the user data.
		''' </summary>
		''' <returns> The user data object. It is used for whatever data
		''' the notification source wishes to communicate to its consumers.
		''' </returns>
		''' <seealso cref= #setUserData </seealso>
		Public Overridable Property userData As Object
			Get
				Return userData
			End Get
			Set(ByVal userData As Object)
    
				Me.userData = userData
			End Set
		End Property


		''' <summary>
		''' Returns a String representation of this notification.
		''' </summary>
		''' <returns> A String representation of this notification. </returns>
		Public Overrides Function ToString() As String
			Return MyBase.ToString() & "[type=" & type & "][message=" & message & "]"
		End Function

		''' <summary>
		''' Deserializes a <seealso cref="Notification"/> from an <seealso cref="ObjectInputStream"/>.
		''' </summary>
		Private Sub readObject(ByVal [in] As java.io.ObjectInputStream)
		  ' New serial form ignores extra field "sourceObjectName"
		  [in].defaultReadObject()
		  MyBase.source = source
		End Sub


		''' <summary>
		''' Serializes a <seealso cref="Notification"/> to an <seealso cref="ObjectOutputStream"/>.
		''' </summary>
		Private Sub writeObject(ByVal out As java.io.ObjectOutputStream)
			If compat Then
				' Serializes this instance in the old serial form
				'
				Dim fields As java.io.ObjectOutputStream.PutField = out.putFields()
				fields.put("type", type)
				fields.put("sequenceNumber", sequenceNumber)
				fields.put("timeStamp", timeStamp)
				fields.put("userData", userData)
				fields.put("message", message)
				fields.put("source", source)
				out.writeFields()
			Else
				' Serializes this instance in the new serial form
				'
				out.defaultWriteObject()
			End If
		End Sub
	End Class

End Namespace