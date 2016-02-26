Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections
Imports System.Collections.Generic
import static com.sun.jmx.mbeanserver.Util.cast
import static com.sun.jmx.defaults.JmxProperties.RELATION_LOGGER

'
' * Copyright (c) 2000, 2008, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.management.relation







	''' <summary>
	''' Filter for <seealso cref="MBeanServerNotification"/>.
	''' This filter filters MBeanServerNotification notifications by
	''' selecting the ObjectNames of interest and the operations (registration,
	''' unregistration, both) of interest (corresponding to notification
	''' types).
	''' 
	''' <p>The <b>serialVersionUID</b> of this class is <code>2605900539589789736L</code>.
	''' 
	''' @since 1.5
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class MBeanServerNotificationFilter
		Inherits javax.management.NotificationFilterSupport ' serialVersionUID must be constant

		' Serialization compatibility stuff:
		' Two serial forms are supported in this class. The selected form depends
		' on system property "jmx.serial.form":
		'  - "1.0" for JMX 1.0
		'  - any other value for JMX 1.1 and higher
		'
		' Serial version for old serial form
		Private Const oldSerialVersionUID As Long = 6001782699077323605L
		'
		' Serial version for new serial form
		Private Const newSerialVersionUID As Long = 2605900539589789736L
		'
		' Serializable fields in old serial form
		Private Shared ReadOnly oldSerialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("mySelectObjNameList", GetType(ArrayList)), New java.io.ObjectStreamField("myDeselectObjNameList", GetType(ArrayList)) }
		'
		' Serializable fields in new serial form
		Private Shared ReadOnly newSerialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("selectedNames", GetType(IList)), New java.io.ObjectStreamField("deselectedNames", GetType(IList)) }
		'
		' Actual serial version and serial form
		Private Shared ReadOnly serialVersionUID As Long
		''' <summary>
		''' @serialField selectedNames List List of <seealso cref="ObjectName"/>s of interest
		'''         <ul>
		'''         <li><code>null</code> means that all <seealso cref="ObjectName"/>s are implicitly selected
		'''         (check for explicit deselections)</li>
		'''         <li>Empty vector means that no <seealso cref="ObjectName"/> is explicitly selected</li>
		'''         </ul>
		''' @serialField deselectedNames List List of <seealso cref="ObjectName"/>s with no interest
		'''         <ul>
		'''         <li><code>null</code> means that all <seealso cref="ObjectName"/>s are implicitly deselected
		'''         (check for explicit selections))</li>
		'''         <li>Empty vector means that no <seealso cref="ObjectName"/> is explicitly deselected</li>
		'''         </ul>
		''' </summary>
		Private Shared ReadOnly serialPersistentFields As java.io.ObjectStreamField()
		Private Shared compat As Boolean = False
		Shared Sub New()
			Try
				Dim act As New com.sun.jmx.mbeanserver.GetPropertyAction("jmx.serial.form")
				Dim form As String = java.security.AccessController.doPrivileged(act)
				compat = (form IsNot Nothing AndAlso form.Equals("1.0"))
			Catch e As Exception
				' OK : Too bad, no compat with 1.0
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

		'
		' Private members
		'

		''' <summary>
		''' @serial List of <seealso cref="ObjectName"/>s of interest
		'''         <ul>
		'''         <li><code>null</code> means that all <seealso cref="ObjectName"/>s are implicitly selected
		'''         (check for explicit deselections)</li>
		'''         <li>Empty vector means that no <seealso cref="ObjectName"/> is explicitly selected</li>
		'''         </ul>
		''' </summary>
		Private selectedNames As IList(Of javax.management.ObjectName) = New List(Of javax.management.ObjectName)

		''' <summary>
		''' @serial List of <seealso cref="ObjectName"/>s with no interest
		'''         <ul>
		'''         <li><code>null</code> means that all <seealso cref="ObjectName"/>s are implicitly deselected
		'''         (check for explicit selections))</li>
		'''         <li>Empty vector means that no <seealso cref="ObjectName"/> is explicitly deselected</li>
		'''         </ul>
		''' </summary>
		Private deselectedNames As IList(Of javax.management.ObjectName) = Nothing

		'
		' Constructor
		'

		''' <summary>
		''' Creates a filter selecting all MBeanServerNotification notifications for
		''' all ObjectNames.
		''' </summary>
		Public Sub New()

			MyBase.New()
			RELATION_LOGGER.entering(GetType(MBeanServerNotificationFilter).name, "MBeanServerNotificationFilter")

			enableType(javax.management.MBeanServerNotification.REGISTRATION_NOTIFICATION)
			enableType(javax.management.MBeanServerNotification.UNREGISTRATION_NOTIFICATION)

			RELATION_LOGGER.exiting(GetType(MBeanServerNotificationFilter).name, "MBeanServerNotificationFilter")
			Return
		End Sub

		'
		' Accessors
		'

		''' <summary>
		''' Disables any MBeanServerNotification (all ObjectNames are
		''' deselected).
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub disableAllObjectNames()

			RELATION_LOGGER.entering(GetType(MBeanServerNotificationFilter).name, "disableAllObjectNames")

			selectedNames = New List(Of javax.management.ObjectName)
			deselectedNames = Nothing

			RELATION_LOGGER.exiting(GetType(MBeanServerNotificationFilter).name, "disableAllObjectNames")
			Return
		End Sub

		''' <summary>
		''' Disables MBeanServerNotifications concerning given ObjectName.
		''' </summary>
		''' <param name="objectName">  ObjectName no longer of interest
		''' </param>
		''' <exception cref="IllegalArgumentException">  if the given ObjectName is null </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub disableObjectName(ByVal ___objectName As javax.management.ObjectName)

			If ___objectName Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(MBeanServerNotificationFilter).name, "disableObjectName", ___objectName)

			' Removes from selected ObjectNames, if present
			If selectedNames IsNot Nothing Then
				If selectedNames.Count <> 0 Then selectedNames.Remove(___objectName)
			End If

			' Adds it in deselected ObjectNames
			If deselectedNames IsNot Nothing Then
				' If all are deselected, no need to do anything :)
				If Not(deselectedNames.Contains(___objectName)) Then deselectedNames.Add(___objectName)
			End If

			RELATION_LOGGER.exiting(GetType(MBeanServerNotificationFilter).name, "disableObjectName")
			Return
		End Sub

		''' <summary>
		''' Enables all MBeanServerNotifications (all ObjectNames are selected).
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub enableAllObjectNames()

			RELATION_LOGGER.entering(GetType(MBeanServerNotificationFilter).name, "enableAllObjectNames")

			selectedNames = Nothing
			deselectedNames = New List(Of javax.management.ObjectName)

			RELATION_LOGGER.exiting(GetType(MBeanServerNotificationFilter).name, "enableAllObjectNames")
			Return
		End Sub

		''' <summary>
		''' Enables MBeanServerNotifications concerning given ObjectName.
		''' </summary>
		''' <param name="objectName">  ObjectName of interest
		''' </param>
		''' <exception cref="IllegalArgumentException">  if the given ObjectName is null </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub enableObjectName(ByVal ___objectName As javax.management.ObjectName)

			If ___objectName Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(MBeanServerNotificationFilter).name, "enableObjectName", ___objectName)

			' Removes from deselected ObjectNames, if present
			If deselectedNames IsNot Nothing Then
				If deselectedNames.Count <> 0 Then deselectedNames.Remove(___objectName)
			End If

			' Adds it in selected ObjectNames
			If selectedNames IsNot Nothing Then
				' If all are selected, no need to do anything :)
				If Not(selectedNames.Contains(___objectName)) Then selectedNames.Add(___objectName)
			End If

			RELATION_LOGGER.exiting(GetType(MBeanServerNotificationFilter).name, "enableObjectName")
			Return
		End Sub

		''' <summary>
		''' Gets all the ObjectNames enabled.
		''' </summary>
		''' <returns> Vector of ObjectNames:
		''' <P>- null means all ObjectNames are implicitly selected, except the
		''' ObjectNames explicitly deselected
		''' <P>- empty means all ObjectNames are deselected, i.e. no ObjectName
		''' selected. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property enabledObjectNames As List(Of javax.management.ObjectName)
			Get
				If selectedNames IsNot Nothing Then
					Return New List(Of javax.management.ObjectName)(selectedNames)
				Else
					Return Nothing
				End If
			End Get
		End Property

		''' <summary>
		''' Gets all the ObjectNames disabled.
		''' </summary>
		''' <returns> Vector of ObjectNames:
		''' <P>- null means all ObjectNames are implicitly deselected, except the
		''' ObjectNames explicitly selected
		''' <P>- empty means all ObjectNames are selected, i.e. no ObjectName
		''' deselected. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property disabledObjectNames As List(Of javax.management.ObjectName)
			Get
				If deselectedNames IsNot Nothing Then
					Return New List(Of javax.management.ObjectName)(deselectedNames)
				Else
					Return Nothing
				End If
			End Get
		End Property

		'
		' NotificationFilter interface
		'

		''' <summary>
		''' Invoked before sending the specified notification to the listener.
		''' <P>If:
		''' <P>- the ObjectName of the concerned MBean is selected (explicitly OR
		''' (implicitly and not explicitly deselected))
		''' <P>AND
		''' <P>- the type of the operation (registration or unregistration) is
		''' selected
		''' <P>then the notification is sent to the listener.
		''' </summary>
		''' <param name="notif">  The notification to be sent.
		''' </param>
		''' <returns> true if the notification has to be sent to the listener, false
		''' otherwise.
		''' </returns>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function isNotificationEnabled(ByVal notif As javax.management.Notification) As Boolean

			If notif Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			RELATION_LOGGER.entering(GetType(MBeanServerNotificationFilter).name, "isNotificationEnabled", notif)

			' Checks the type first
			Dim ntfType As String = notif.type
			Dim ___enabledTypes As List(Of String) = enabledTypes
			If Not(___enabledTypes.Contains(ntfType)) Then
				RELATION_LOGGER.logp(java.util.logging.Level.FINER, GetType(MBeanServerNotificationFilter).name, "isNotificationEnabled", "Type not selected, exiting")
				Return False
			End If

			' We have a MBeanServerNotification: downcasts it
			Dim mbsNtf As javax.management.MBeanServerNotification = CType(notif, javax.management.MBeanServerNotification)

			' Checks the ObjectName
			Dim objName As javax.management.ObjectName = mbsNtf.mBeanName
			' Is it selected?
			Dim isSelectedFlg As Boolean = False
			If selectedNames IsNot Nothing Then
				' Not all are implicitly selected:
				' checks for explicit selection
				If selectedNames.Count = 0 Then
					' All are explicitly not selected
					RELATION_LOGGER.logp(java.util.logging.Level.FINER, GetType(MBeanServerNotificationFilter).name, "isNotificationEnabled", "No ObjectNames selected, exiting")
					Return False
				End If

				isSelectedFlg = selectedNames.Contains(objName)
				If Not isSelectedFlg Then
					' Not in the explicit selected list
					RELATION_LOGGER.logp(java.util.logging.Level.FINER, GetType(MBeanServerNotificationFilter).name, "isNotificationEnabled", "ObjectName not in selected list, exiting")
					Return False
				End If
			End If

			If Not isSelectedFlg Then
				' Not explicitly selected: is it deselected?

				If deselectedNames Is Nothing Then
					' All are implicitly deselected and it is not explicitly
					' selected
					RELATION_LOGGER.logp(java.util.logging.Level.FINER, GetType(MBeanServerNotificationFilter).name, "isNotificationEnabled", "ObjectName not selected, and all " & "names deselected, exiting")
					Return False

				ElseIf deselectedNames.Contains(objName) Then
					' Explicitly deselected
					RELATION_LOGGER.logp(java.util.logging.Level.FINER, GetType(MBeanServerNotificationFilter).name, "isNotificationEnabled", "ObjectName explicitly not selected, exiting")
					Return False
				End If
			End If

			RELATION_LOGGER.logp(java.util.logging.Level.FINER, GetType(MBeanServerNotificationFilter).name, "isNotificationEnabled", "ObjectName selected, exiting")
			Return True
		End Function


		''' <summary>
		''' Deserializes an <seealso cref="MBeanServerNotificationFilter"/> from an <seealso cref="ObjectInputStream"/>.
		''' </summary>
		Private Sub readObject(ByVal [in] As java.io.ObjectInputStream)
		  If compat Then
			' Read an object serialized in the old serial form
			'
			Dim fields As java.io.ObjectInputStream.GetField = [in].readFields()
			selectedNames = cast(fields.get("mySelectObjNameList", Nothing))
			If fields.defaulted("mySelectObjNameList") Then Throw New NullPointerException("mySelectObjNameList")
			deselectedNames = cast(fields.get("myDeselectObjNameList", Nothing))
			If fields.defaulted("myDeselectObjNameList") Then Throw New NullPointerException("myDeselectObjNameList")
		  Else
			' Read an object serialized in the new serial form
			'
			[in].defaultReadObject()
		  End If
		End Sub


		''' <summary>
		''' Serializes an <seealso cref="MBeanServerNotificationFilter"/> to an <seealso cref="ObjectOutputStream"/>.
		''' </summary>
		Private Sub writeObject(ByVal out As java.io.ObjectOutputStream)
		  If compat Then
			' Serializes this instance in the old serial form
			'
			Dim fields As java.io.ObjectOutputStream.PutField = out.putFields()
			fields.put("mySelectObjNameList", selectedNames)
			fields.put("myDeselectObjNameList", deselectedNames)
			out.writeFields()
		  Else
			' Serializes this instance in the new serial form
			'
			out.defaultWriteObject()
		  End If
		End Sub
	End Class

End Namespace