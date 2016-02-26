'
' * Copyright (c) 1999, 2003, Oracle and/or its affiliates. All rights reserved.
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
	''' Provides definitions of the attribute change notifications sent by MBeans.
	''' <P>
	''' It's up to the MBean owning the attribute of interest to create and send
	''' attribute change notifications when the attribute change occurs.
	''' So the <CODE>NotificationBroadcaster</CODE> interface has to be implemented
	''' by any MBean for which an attribute change is of interest.
	''' <P>
	''' Example:
	''' If an MBean called <CODE>myMbean</CODE> needs to notify registered listeners
	''' when its attribute:
	''' <BLOCKQUOTE><CODE>
	'''      String myString
	''' </CODE></BLOCKQUOTE>
	''' is modified, <CODE>myMbean</CODE> creates and emits the following notification:
	''' <BLOCKQUOTE><CODE>
	''' new AttributeChangeNotification(myMbean, sequenceNumber, timeStamp, msg,
	'''                                 "myString", "String", oldValue, newValue);
	''' </CODE></BLOCKQUOTE>
	''' 
	''' @since 1.5
	''' </summary>
	Public Class AttributeChangeNotification
		Inherits javax.management.Notification

		' Serial version 
		Private Const serialVersionUID As Long = 535176054565814134L

		''' <summary>
		''' Notification type which indicates that the observed MBean attribute value has changed.
		''' <BR>The value of this type string is <CODE>jmx.attribute.change</CODE>.
		''' </summary>
		Public Const ATTRIBUTE_CHANGE As String = "jmx.attribute.change"


		''' <summary>
		''' @serial The MBean attribute name.
		''' </summary>
		Private attributeName As String = Nothing

		''' <summary>
		''' @serial The MBean attribute type.
		''' </summary>
		Private attributeType As String = Nothing

		''' <summary>
		''' @serial The MBean attribute old value.
		''' </summary>
		Private oldValue As Object = Nothing

		''' <summary>
		''' @serial The MBean attribute new value.
		''' </summary>
		Private newValue As Object = Nothing


		''' <summary>
		''' Constructs an attribute change notification object.
		''' In addition to the information common to all notification, the caller must supply the name and type
		''' of the attribute, as well as its old and new values.
		''' </summary>
		''' <param name="source"> The notification producer, that is, the MBean the attribute belongs to. </param>
		''' <param name="sequenceNumber"> The notification sequence number within the source object. </param>
		''' <param name="timeStamp"> The date at which the notification is being sent. </param>
		''' <param name="msg"> A String containing the message of the notification. </param>
		''' <param name="attributeName"> A String giving the name of the attribute. </param>
		''' <param name="attributeType"> A String containing the type of the attribute. </param>
		''' <param name="oldValue"> An object representing value of the attribute before the change. </param>
		''' <param name="newValue"> An object representing value of the attribute after the change. </param>
		Public Sub New(ByVal source As Object, ByVal sequenceNumber As Long, ByVal timeStamp As Long, ByVal msg As String, ByVal attributeName As String, ByVal attributeType As String, ByVal oldValue As Object, ByVal newValue As Object)

			MyBase.New(AttributeChangeNotification.ATTRIBUTE_CHANGE, source, sequenceNumber, timeStamp, msg)
			Me.attributeName = attributeName
			Me.attributeType = attributeType
			Me.oldValue = oldValue
			Me.newValue = newValue
		End Sub


		''' <summary>
		''' Gets the name of the attribute which has changed.
		''' </summary>
		''' <returns> A String containing the name of the attribute. </returns>
		Public Overridable Property attributeName As String
			Get
				Return attributeName
			End Get
		End Property

		''' <summary>
		''' Gets the type of the attribute which has changed.
		''' </summary>
		''' <returns> A String containing the type of the attribute. </returns>
		Public Overridable Property attributeType As String
			Get
				Return attributeType
			End Get
		End Property

		''' <summary>
		''' Gets the old value of the attribute which has changed.
		''' </summary>
		''' <returns> An Object containing the old value of the attribute. </returns>
		Public Overridable Property oldValue As Object
			Get
				Return oldValue
			End Get
		End Property

		''' <summary>
		''' Gets the new value of the attribute which has changed.
		''' </summary>
		''' <returns> An Object containing the new value of the attribute. </returns>
		Public Overridable Property newValue As Object
			Get
				Return newValue
			End Get
		End Property

	End Class

End Namespace