Imports System
Imports System.Text
import static com.sun.jmx.defaults.JmxProperties.MODELMBEAN_LOGGER

'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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
'
' * @author    IBM Corp.
' *
' * Copyright IBM Corp. 1999-2000.  All rights reserved.
' 

Namespace javax.management.modelmbean




	''' <summary>
	''' <p>The ModelMBeanNotificationInfo object describes a notification emitted
	''' by a ModelMBean.
	''' It is a subclass of MBeanNotificationInfo with the addition of an
	''' associated Descriptor and an implementation of the Descriptor interface.</p>
	''' 
	''' <P id="descriptor">
	''' The fields in the descriptor are defined, but not limited to, the following.
	''' Note that when the Type in this table is Number, a String that is the decimal
	''' representation of a Long can also be used.</P>
	''' 
	''' <table border="1" cellpadding="5" summary="ModelMBeanNotificationInfo Fields">
	''' <tr><th>Name</th><th>Type</th><th>Meaning</th></tr>
	''' <tr><td>name</td><td>String</td>
	'''     <td>Notification name.</td></tr>
	''' <tr><td>descriptorType</td><td>String</td>
	'''     <td>Must be "notification".</td></tr>
	''' <tr><td>severity</td><td>Number</td>
	'''     <td>0-6 where 0: unknown; 1: non-recoverable;
	'''         2: critical, failure; 3: major, severe;
	'''         4: minor, marginal, error; 5: warning;
	'''         6: normal, cleared, informative</td></tr>
	''' <tr><td>messageID</td><td>String</td>
	'''     <td>Unique key for message text (to allow translation, analysis).</td></tr>
	''' <tr><td>messageText</td><td>String</td>
	'''     <td>Text of notification.</td></tr>
	''' <tr><td>log</td><td>String</td>
	'''     <td>T - log message, F - do not log message.</td></tr>
	''' <tr><td>logfile</td><td>String</td>
	'''     <td>fully qualified file name appropriate for operating system.</td></tr>
	''' <tr><td>visibility</td><td>Number</td>
	'''     <td>1-4 where 1: always visible 4: rarely visible.</td></tr>
	''' <tr><td>presentationString</td><td>String</td>
	'''     <td>XML formatted string to allow presentation of data.</td></tr>
	''' </table>
	''' 
	''' <p>The default descriptor contains the name, descriptorType,
	''' displayName and severity(=6) fields.  The default value of the name
	''' and displayName fields is the name of the Notification class (as
	''' specified by the <code>name</code> parameter of the
	''' ModelMBeanNotificationInfo constructor).</p>
	''' 
	''' <p>The <b>serialVersionUID</b> of this class is <code>-7445681389570207141L</code>.
	''' 
	''' @since 1.5
	''' </summary>

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class ModelMBeanNotificationInfo
		Inherits javax.management.MBeanNotificationInfo
		Implements javax.management.DescriptorAccess ' serialVersionUID is not constant

		' Serialization compatibility stuff:
		' Two serial forms are supported in this class. The selected form
		' depends on system property "jmx.serial.form":
		'  - "1.0" for JMX 1.0
		'  - any other value for JMX 1.1 and higher
		'
		' Serial version for old serial form
		Private Const oldSerialVersionUID As Long = -5211564525059047097L
		'
		' Serial version for new serial form
		Private Const newSerialVersionUID As Long = -7445681389570207141L
		'
		' Serializable fields in old serial form
		Private Shared ReadOnly oldSerialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("notificationDescriptor", GetType(javax.management.Descriptor)), New java.io.ObjectStreamField("currClass", GetType(String)) }
		'
		' Serializable fields in new serial form
		Private Shared ReadOnly newSerialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("notificationDescriptor", GetType(javax.management.Descriptor)) }
		'
		' Actual serial version and serial form
		Private Shared ReadOnly Shadows serialVersionUID As Long
		''' <summary>
		''' @serialField notificationDescriptor Descriptor The descriptor
		'''   containing the appropriate metadata for this instance
		''' </summary>
		Private Shared ReadOnly serialPersistentFields As java.io.ObjectStreamField()
		Private Shared compat As Boolean = False
		Shared Sub New()
			Try
				Dim act As New com.sun.jmx.mbeanserver.GetPropertyAction("jmx.serial.form")
				Dim form As String = java.security.AccessController.doPrivileged(act)
				compat = (form IsNot Nothing AndAlso form.Equals("1.0"))
			Catch e As Exception
				' OK: No compat with 1.0
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
		''' @serial The descriptor containing the appropriate metadata for
		'''         this instance
		''' </summary>
		Private notificationDescriptor As javax.management.Descriptor

		Private Const currClass As String = "ModelMBeanNotificationInfo"

		''' <summary>
		''' Constructs a ModelMBeanNotificationInfo object with a default
		''' descriptor.
		''' </summary>
		''' <param name="notifTypes"> The array of strings (in dot notation) containing
		'''     the notification types that may be emitted. </param>
		''' <param name="name"> The name of the Notification class. </param>
		''' <param name="description"> A human readable description of the
		'''     Notification. Optional.
		'''  </param>
		Public Sub New(ByVal notifTypes As String(), ByVal name As String, ByVal description As String)
			Me.New(notifTypes,name,description,Nothing)
		End Sub

		''' <summary>
		''' Constructs a ModelMBeanNotificationInfo object.
		''' </summary>
		''' <param name="notifTypes"> The array of strings (in dot notation)
		'''        containing the notification types that may be emitted. </param>
		''' <param name="name"> The name of the Notification class. </param>
		''' <param name="description"> A human readable description of the Notification.
		'''        Optional. </param>
		''' <param name="descriptor"> An instance of Descriptor containing the
		'''        appropriate metadata for this instance of the
		'''        MBeanNotificationInfo. If it is null a default descriptor
		'''        will be created. If the descriptor does not contain the
		'''        fields "displayName" or "severity",
		'''        the missing ones are added with their default values.
		''' </param>
		''' <exception cref="RuntimeOperationsException"> Wraps an
		'''    <seealso cref="IllegalArgumentException"/>. The descriptor is invalid, or
		'''    descriptor field "name" is not equal to parameter name, or
		'''    descriptor field "descriptorType" is not equal to "notification".
		''' 
		'''  </exception>
		Public Sub New(ByVal notifTypes As String(), ByVal name As String, ByVal description As String, ByVal descriptor As javax.management.Descriptor)
			MyBase.New(notifTypes, name, description)
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanNotificationInfo).name, "ModelMBeanNotificationInfo", "Entry")
			notificationDescriptor = validDescriptor(descriptor)
		End Sub

		''' <summary>
		''' Constructs a new ModelMBeanNotificationInfo object from this
		''' ModelMBeanNotfication Object.
		''' </summary>
		''' <param name="inInfo"> the ModelMBeanNotificationInfo to be duplicated
		''' 
		'''  </param>
		Public Sub New(ByVal inInfo As ModelMBeanNotificationInfo)
			Me.New(inInfo.notifTypes, inInfo.name, inInfo.description,inInfo.descriptor)
		End Sub

		''' <summary>
		''' Creates and returns a new ModelMBeanNotificationInfo which is a
		''' duplicate of this ModelMBeanNotificationInfo.
		''' 
		''' </summary>
		Public Overrides Function clone() As Object
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanNotificationInfo).name, "clone()", "Entry")
			Return (New ModelMBeanNotificationInfo(Me))
		End Function

		''' <summary>
		''' Returns a copy of the associated Descriptor for the
		''' ModelMBeanNotificationInfo.
		''' </summary>
		''' <returns> Descriptor associated with the
		''' ModelMBeanNotificationInfo object.
		''' </returns>
		''' <seealso cref= #setDescriptor
		'''  </seealso>
		Public Property Overrides descriptor As javax.management.Descriptor
			Get
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanNotificationInfo).name, "getDescriptor()", "Entry")
    
				If notificationDescriptor Is Nothing Then
					' Dead code. Should never happen.
					If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanNotificationInfo).name, "getDescriptor()", "Descriptor value is null, " & "setting descriptor to default values")
					notificationDescriptor = validDescriptor(Nothing)
				End If
    
				Return (CType(notificationDescriptor.clone(), javax.management.Descriptor))
			End Get
			Set(ByVal inDescriptor As javax.management.Descriptor)
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanNotificationInfo).name, "setDescriptor(Descriptor)", "Entry")
				notificationDescriptor = validDescriptor(inDescriptor)
			End Set
		End Property


		''' <summary>
		''' Returns a human readable string containing
		''' ModelMBeanNotificationInfo.
		''' </summary>
		''' <returns> a string describing this object.
		'''  </returns>
		Public Overrides Function ToString() As String
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanNotificationInfo).name, "toString()", "Entry")

			Dim retStr As New StringBuilder

			retStr.Append("ModelMBeanNotificationInfo: ").append(Me.name)

			retStr.Append(" ; Description: ").append(Me.description)

			retStr.Append(" ; Descriptor: ").append(Me.descriptor)

			retStr.Append(" ; Types: ")
			Dim nTypes As String() = Me.notifTypes
			For i As Integer = 0 To nTypes.Length - 1
				If i > 0 Then retStr.Append(", ")
				retStr.Append(nTypes(i))
			Next i
			Return retStr.ToString()
		End Function


		''' <summary>
		''' Clones the passed in Descriptor, sets default values, and checks for validity.
		''' If the Descriptor is invalid (for instance by having the wrong "name"),
		''' this indicates programming error and a RuntimeOperationsException will be thrown.
		''' 
		''' The following fields will be defaulted if they are not already set:
		''' descriptorType="notification",displayName=this.getName(),
		''' name=this.getName(),severity="6"
		''' 
		''' </summary>
		''' <param name="in"> Descriptor to be checked, or null which is equivalent to an
		''' empty Descriptor. </param>
		''' <exception cref="RuntimeOperationsException"> if Descriptor is invalid </exception>
		Private Function validDescriptor(ByVal [in] As javax.management.Descriptor) As javax.management.Descriptor
			Dim clone As javax.management.Descriptor
			Dim defaulted As Boolean = ([in] Is Nothing)
			If defaulted Then
				clone = New DescriptorSupport
				MODELMBEAN_LOGGER.finer("Null Descriptor, creating new.")
			Else
				clone = CType([in].clone(), javax.management.Descriptor)
			End If

			'Setting defaults.
			If defaulted AndAlso clone.getFieldValue("name") Is Nothing Then
				clone.fieldeld("name", Me.name)
				MODELMBEAN_LOGGER.finer("Defaulting Descriptor name to " & Me.name)
			End If
			If defaulted AndAlso clone.getFieldValue("descriptorType") Is Nothing Then
				clone.fieldeld("descriptorType", "notification")
				MODELMBEAN_LOGGER.finer("Defaulting descriptorType to ""notification""")
			End If
			If clone.getFieldValue("displayName") Is Nothing Then
				clone.fieldeld("displayName",Me.name)
				MODELMBEAN_LOGGER.finer("Defaulting Descriptor displayName to " & Me.name)
			End If
			If clone.getFieldValue("severity") Is Nothing Then
				clone.fieldeld("severity", "6")
				MODELMBEAN_LOGGER.finer("Defaulting Descriptor severity field to 6")
			End If

			'Checking validity
			If Not clone.valid Then Throw New javax.management.RuntimeOperationsException(New System.ArgumentException("Invalid Descriptor argument"), "The isValid() method of the Descriptor object itself returned false," & "one or more required fields are invalid. Descriptor:" & clone.ToString())
			If (Not name.ToUpper()) = CStr(clone.getFieldValue("name")).ToUpper() Then Throw New javax.management.RuntimeOperationsException(New System.ArgumentException("Invalid Descriptor argument"), "The Descriptor ""name"" field does not match the object described. " & " Expected: " & Me.name & " , was: " & clone.getFieldValue("name"))
			If Not "notification".equalsIgnoreCase(CStr(clone.getFieldValue("descriptorType"))) Then Throw New javax.management.RuntimeOperationsException(New System.ArgumentException("Invalid Descriptor argument"), "The Descriptor ""descriptorType"" field does not match the object described. " & " Expected: ""notification"" ," & " was: " & clone.getFieldValue("descriptorType"))

			Return clone
		End Function


		''' <summary>
		''' Deserializes a <seealso cref="ModelMBeanNotificationInfo"/> from an
		''' <seealso cref="ObjectInputStream"/>.
		''' 
		''' </summary>
		Private Sub readObject(ByVal [in] As java.io.ObjectInputStream)
			' New serial form ignores extra field "currClass"
			[in].defaultReadObject()
		End Sub


		''' <summary>
		''' Serializes a <seealso cref="ModelMBeanNotificationInfo"/> to an
		''' <seealso cref="ObjectOutputStream"/>.
		''' 
		''' </summary>
		Private Sub writeObject(ByVal out As java.io.ObjectOutputStream)
			If compat Then
				' Serializes this instance in the old serial form
				'
				Dim fields As java.io.ObjectOutputStream.PutField = out.putFields()
				fields.put("notificationDescriptor", notificationDescriptor)
				fields.put("currClass", currClass)
				out.writeFields()
			Else
				' Serializes this instance in the new serial form
				'
				out.defaultWriteObject()
			End If
		End Sub

	End Class

End Namespace