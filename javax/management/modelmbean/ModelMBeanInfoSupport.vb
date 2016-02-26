Imports Microsoft.VisualBasic
Imports System
Imports System.Text
import static com.sun.jmx.defaults.JmxProperties.MODELMBEAN_LOGGER

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
'
' * @author    IBM Corp.
' *
' * Copyright IBM Corp. 1999-2000.  All rights reserved.
' 

Namespace javax.management.modelmbean




	''' <summary>
	''' This class represents the meta data for ModelMBeans.  Descriptors have been
	''' added on the meta data objects.
	''' <P>
	''' Java resources wishing to be manageable instantiate the ModelMBean using the
	''' MBeanServer's createMBean method.  The resource then sets the ModelMBeanInfo
	''' and Descriptors for the ModelMBean instance. The attributes and operations
	''' exposed via the ModelMBeanInfo for the ModelMBean are accessible
	''' from MBeans, connectors/adaptors like other MBeans. Through the Descriptors,
	''' values and methods in the managed application can be defined and mapped to
	''' attributes and operations of the ModelMBean.
	''' This mapping can be defined during development in a file or dynamically and
	''' programmatically at runtime.
	''' <P>
	''' Every ModelMBean which is instantiated in the MBeanServer becomes manageable:
	''' its attributes and operations
	''' become remotely accessible through the connectors/adaptors connected to that
	''' MBeanServer.
	''' A Java object cannot be registered in the MBeanServer unless it is a JMX
	''' compliant MBean.
	''' By instantiating a ModelMBean, resources are guaranteed that the MBean is
	''' valid.
	''' 
	''' MBeanException and RuntimeOperationsException must be thrown on every public
	''' method.  This allows for wrapping exceptions from distributed
	''' communications (RMI, EJB, etc.)
	''' 
	''' <p>The <b>serialVersionUID</b> of this class is
	''' <code>-1935722590756516193L</code>.
	''' 
	''' @since 1.5
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class ModelMBeanInfoSupport
		Inherits javax.management.MBeanInfo
		Implements ModelMBeanInfo

		' Serialization compatibility stuff:
		' Two serial forms are supported in this class. The selected form depends
		' on system property "jmx.serial.form":
		'  - "1.0" for JMX 1.0
		'  - any other value for JMX 1.1 and higher
		'
		' Serial version for old serial form
		Private Const oldSerialVersionUID As Long = -3944083498453227709L
		'
		' Serial version for new serial form
		Private Const newSerialVersionUID As Long = -1935722590756516193L
		'
		' Serializable fields in old serial form
		Private Shared ReadOnly oldSerialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("modelMBeanDescriptor", GetType(javax.management.Descriptor)), New java.io.ObjectStreamField("mmbAttributes", GetType(javax.management.MBeanAttributeInfo())), New java.io.ObjectStreamField("mmbConstructors", GetType(javax.management.MBeanConstructorInfo())), New java.io.ObjectStreamField("mmbNotifications", GetType(javax.management.MBeanNotificationInfo())), New java.io.ObjectStreamField("mmbOperations", GetType(javax.management.MBeanOperationInfo())), New java.io.ObjectStreamField("currClass", GetType(String)) }
		'
		' Serializable fields in new serial form
		Private Shared ReadOnly newSerialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("modelMBeanDescriptor", GetType(javax.management.Descriptor)), New java.io.ObjectStreamField("modelMBeanAttributes", GetType(javax.management.MBeanAttributeInfo())), New java.io.ObjectStreamField("modelMBeanConstructors", GetType(javax.management.MBeanConstructorInfo())), New java.io.ObjectStreamField("modelMBeanNotifications", GetType(javax.management.MBeanNotificationInfo())), New java.io.ObjectStreamField("modelMBeanOperations", GetType(javax.management.MBeanOperationInfo())) }
		'
		' Actual serial version and serial form
		Private Shared ReadOnly Shadows serialVersionUID As Long
		''' <summary>
		''' @serialField modelMBeanDescriptor Descriptor The descriptor containing
		'''              MBean wide policy
		''' @serialField modelMBeanAttributes ModelMBeanAttributeInfo[] The array of
		'''              <seealso cref="ModelMBeanAttributeInfo"/> objects which
		'''              have descriptors
		''' @serialField modelMBeanConstructors MBeanConstructorInfo[] The array of
		'''              <seealso cref="ModelMBeanConstructorInfo"/> objects which
		'''              have descriptors
		''' @serialField modelMBeanNotifications MBeanNotificationInfo[] The array of
		'''              <seealso cref="ModelMBeanNotificationInfo"/> objects which
		'''              have descriptors
		''' @serialField modelMBeanOperations MBeanOperationInfo[] The array of
		'''              <seealso cref="ModelMBeanOperationInfo"/> objects which
		'''              have descriptors
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
		''' @serial The descriptor containing MBean wide policy
		''' </summary>
		Private modelMBeanDescriptor As javax.management.Descriptor = Nothing

	'     The following fields always have the same values as the
	'       fields inherited from MBeanInfo and are retained only for
	'       compatibility.  By rewriting the serialization code we could
	'       get rid of them.
	'
	'       These fields can't be final because they are assigned to by
	'       readObject().  

		''' <summary>
		''' @serial The array of <seealso cref="ModelMBeanAttributeInfo"/> objects which
		'''         have descriptors
		''' </summary>
		Private modelMBeanAttributes As javax.management.MBeanAttributeInfo()

		''' <summary>
		''' @serial The array of <seealso cref="ModelMBeanConstructorInfo"/> objects which
		'''         have descriptors
		''' </summary>
		Private modelMBeanConstructors As javax.management.MBeanConstructorInfo()

		''' <summary>
		''' @serial The array of <seealso cref="ModelMBeanNotificationInfo"/> objects which
		'''         have descriptors
		''' </summary>
		Private modelMBeanNotifications As javax.management.MBeanNotificationInfo()

		''' <summary>
		''' @serial The array of <seealso cref="ModelMBeanOperationInfo"/> objects which
		'''         have descriptors
		''' </summary>
		Private modelMBeanOperations As javax.management.MBeanOperationInfo()

		Private Const ATTR As String = "attribute"
		Private Const OPER As String = "operation"
		Private Const NOTF As String = "notification"
		Private Const CONS As String = "constructor"
		Private Const MMB As String = "mbean"
		Private Const ALL As String = "all"
		Private Const currClass As String = "ModelMBeanInfoSupport"

		''' <summary>
		''' Constructs a ModelMBeanInfoSupport which is a duplicate of the given
		''' ModelMBeanInfo.  The returned object is a shallow copy of the given
		''' object.  Neither the Descriptor nor the contained arrays
		''' ({@code ModelMBeanAttributeInfo[]} etc) are cloned.  This method is
		''' chiefly of interest to modify the Descriptor of the returned instance
		''' via <seealso cref="#setDescriptor setDescriptor"/> without affecting the
		''' Descriptor of the original object.
		''' </summary>
		''' <param name="mbi"> the ModelMBeanInfo instance from which the ModelMBeanInfo
		''' being created is initialized. </param>
		Public Sub New(ByVal mbi As ModelMBeanInfo)
			MyBase.New(mbi.className, mbi.description, mbi.attributes, mbi.constructors, mbi.operations, mbi.notifications)

			modelMBeanAttributes = mbi.attributes
			modelMBeanConstructors = mbi.constructors
			modelMBeanOperations = mbi.operations
			modelMBeanNotifications = mbi.notifications

			Try
				Dim ___mbeandescriptor As javax.management.Descriptor = mbi.mBeanDescriptor
				modelMBeanDescriptor = validDescriptor(___mbeandescriptor)
			Catch mbe As javax.management.MBeanException
				modelMBeanDescriptor = validDescriptor(Nothing)
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanInfoSupport).name, "ModelMBeanInfo(ModelMBeanInfo)", "Could not get a valid modelMBeanDescriptor, " & "setting a default Descriptor")
			End Try

			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanInfoSupport).name, "ModelMBeanInfo(ModelMBeanInfo)", "Exit")
		End Sub

		''' <summary>
		''' Creates a ModelMBeanInfoSupport with the provided information,
		''' but the descriptor is a default.
		''' The default descriptor is: name=className, descriptorType="mbean",
		''' displayName=className, persistPolicy="never", log="F", visibility="1"
		''' </summary>
		''' <param name="className"> classname of the MBean </param>
		''' <param name="description"> human readable description of the
		''' ModelMBean </param>
		''' <param name="attributes"> array of ModelMBeanAttributeInfo objects
		''' which have descriptors </param>
		''' <param name="constructors"> array of ModelMBeanConstructorInfo
		''' objects which have descriptors </param>
		''' <param name="operations"> array of ModelMBeanOperationInfo objects
		''' which have descriptors </param>
		''' <param name="notifications"> array of ModelMBeanNotificationInfo
		''' objects which have descriptors </param>
		Public Sub New(ByVal className As String, ByVal description As String, ByVal attributes As ModelMBeanAttributeInfo(), ByVal constructors As ModelMBeanConstructorInfo(), ByVal operations As ModelMBeanOperationInfo(), ByVal notifications As ModelMBeanNotificationInfo())
			Me.New(className, description, attributes, constructors, operations, notifications, Nothing)
		End Sub

		''' <summary>
		''' Creates a ModelMBeanInfoSupport with the provided information
		''' and the descriptor given in parameter.
		''' </summary>
		''' <param name="className"> classname of the MBean </param>
		''' <param name="description"> human readable description of the
		''' ModelMBean </param>
		''' <param name="attributes"> array of ModelMBeanAttributeInfo objects
		''' which have descriptors </param>
		''' <param name="constructors"> array of ModelMBeanConstructorInfo
		''' objects which have descriptor </param>
		''' <param name="operations"> array of ModelMBeanOperationInfo objects
		''' which have descriptor </param>
		''' <param name="notifications"> array of ModelMBeanNotificationInfo
		''' objects which have descriptor </param>
		''' <param name="mbeandescriptor"> descriptor to be used as the
		''' MBeanDescriptor containing MBean wide policy. If the
		''' descriptor is null, a default descriptor will be constructed.
		''' The default descriptor is:
		''' name=className, descriptorType="mbean", displayName=className,
		''' persistPolicy="never", log="F", visibility="1".  If the descriptor
		''' does not contain all of these fields, the missing ones are
		''' added with these default values.
		''' </param>
		''' <exception cref="RuntimeOperationsException"> Wraps an
		''' IllegalArgumentException for invalid descriptor passed in
		''' parameter.  (see {@link #getMBeanDescriptor
		''' getMBeanDescriptor} for the definition of a valid MBean
		''' descriptor.) </exception>

		Public Sub New(ByVal className As String, ByVal description As String, ByVal attributes As ModelMBeanAttributeInfo(), ByVal constructors As ModelMBeanConstructorInfo(), ByVal operations As ModelMBeanOperationInfo(), ByVal notifications As ModelMBeanNotificationInfo(), ByVal mbeandescriptor As javax.management.Descriptor)
			MyBase.New(className, description,If(attributes IsNot Nothing, attributes, NO_ATTRIBUTES),If(constructors IsNot Nothing, constructors, NO_CONSTRUCTORS),If(operations IsNot Nothing, operations, NO_OPERATIONS),If(notifications IsNot Nothing, notifications, NO_NOTIFICATIONS))
	'         The values saved here are possibly null, but we
	'           check this everywhere they are referenced.  If at
	'           some stage we replace null with an empty array
	'           here, as we do in the superclass constructor
	'           parameters, then we must also do this in
	'           readObject().  
			modelMBeanAttributes = attributes
			modelMBeanConstructors = constructors
			modelMBeanOperations = operations
			modelMBeanNotifications = notifications
			modelMBeanDescriptor = validDescriptor(mbeandescriptor)
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanInfoSupport).name, "ModelMBeanInfoSupport(String,String,ModelMBeanAttributeInfo[]," & "ModelMBeanConstructorInfo[],ModelMBeanOperationInfo[]," & "ModelMBeanNotificationInfo[],Descriptor)", "Exit")
		End Sub

		Private Shared ReadOnly NO_ATTRIBUTES As ModelMBeanAttributeInfo() = New ModelMBeanAttributeInfo(){}
		Private Shared ReadOnly NO_CONSTRUCTORS As ModelMBeanConstructorInfo() = New ModelMBeanConstructorInfo(){}
		Private Shared ReadOnly NO_NOTIFICATIONS As ModelMBeanNotificationInfo() = New ModelMBeanNotificationInfo(){}
		Private Shared ReadOnly NO_OPERATIONS As ModelMBeanOperationInfo() = New ModelMBeanOperationInfo(){}

		' Java doc inherited from MOdelMBeanInfo interface

		''' <summary>
		''' Returns a shallow clone of this instance.  Neither the Descriptor nor
		''' the contained arrays ({@code ModelMBeanAttributeInfo[]} etc) are
		''' cloned.  This method is chiefly of interest to modify the Descriptor
		''' of the clone via <seealso cref="#setDescriptor setDescriptor"/> without affecting
		''' the Descriptor of the original object.
		''' </summary>
		''' <returns> a shallow clone of this instance. </returns>
		Public Overrides Function clone() As Object
			Return (New ModelMBeanInfoSupport(Me))
		End Function


		Public Overridable Function getDescriptors(ByVal inDescriptorType As String) As javax.management.Descriptor() Implements ModelMBeanInfo.getDescriptors
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanInfoSupport).name, "getDescriptors(String)", "Entry")

			If (inDescriptorType Is Nothing) OrElse (inDescriptorType.Equals("")) Then inDescriptorType = "all"

			' if no descriptors of that type, will return empty array
			'
			Dim retList As javax.management.Descriptor()

			If inDescriptorType.ToUpper() = MMB.ToUpper() Then
				retList = New javax.management.Descriptor() {modelMBeanDescriptor}
			ElseIf inDescriptorType.ToUpper() = ATTR.ToUpper() Then
				Dim attrList As javax.management.MBeanAttributeInfo() = modelMBeanAttributes
				Dim numAttrs As Integer = 0
				If attrList IsNot Nothing Then numAttrs = attrList.Length

				retList = New javax.management.Descriptor(numAttrs - 1){}
				For i As Integer = 0 To numAttrs - 1
					retList(i) = (CType(attrList(i), ModelMBeanAttributeInfo).descriptor)
				Next i
			ElseIf inDescriptorType.ToUpper() = OPER.ToUpper() Then
				Dim operList As javax.management.MBeanOperationInfo() = modelMBeanOperations
				Dim numOpers As Integer = 0
				If operList IsNot Nothing Then numOpers = operList.Length

				retList = New javax.management.Descriptor(numOpers - 1){}
				For i As Integer = 0 To numOpers - 1
					retList(i) = (CType(operList(i), ModelMBeanOperationInfo).descriptor)
				Next i
			ElseIf inDescriptorType.ToUpper() = CONS.ToUpper() Then
				Dim consList As javax.management.MBeanConstructorInfo() = modelMBeanConstructors
				Dim numCons As Integer = 0
				If consList IsNot Nothing Then numCons = consList.Length

				retList = New javax.management.Descriptor(numCons - 1){}
				For i As Integer = 0 To numCons - 1
					retList(i) = (CType(consList(i), ModelMBeanConstructorInfo).descriptor)
				Next i
			ElseIf inDescriptorType.ToUpper() = NOTF.ToUpper() Then
				Dim notifList As javax.management.MBeanNotificationInfo() = modelMBeanNotifications
				Dim numNotifs As Integer = 0
				If notifList IsNot Nothing Then numNotifs = notifList.Length

				retList = New javax.management.Descriptor(numNotifs - 1){}
				For i As Integer = 0 To numNotifs - 1
					retList(i) = (CType(notifList(i), ModelMBeanNotificationInfo).descriptor)
				Next i
			ElseIf inDescriptorType.ToUpper() = ALL.ToUpper() Then

				Dim attrList As javax.management.MBeanAttributeInfo() = modelMBeanAttributes
				Dim numAttrs As Integer = 0
				If attrList IsNot Nothing Then numAttrs = attrList.Length

				Dim operList As javax.management.MBeanOperationInfo() = modelMBeanOperations
				Dim numOpers As Integer = 0
				If operList IsNot Nothing Then numOpers = operList.Length

				Dim consList As javax.management.MBeanConstructorInfo() = modelMBeanConstructors
				Dim numCons As Integer = 0
				If consList IsNot Nothing Then numCons = consList.Length

				Dim notifList As javax.management.MBeanNotificationInfo() = modelMBeanNotifications
				Dim numNotifs As Integer = 0
				If notifList IsNot Nothing Then numNotifs = notifList.Length

				Dim count As Integer = numAttrs + numCons + numOpers + numNotifs + 1
				retList = New javax.management.Descriptor(count - 1){}

				retList(count-1) = modelMBeanDescriptor

				Dim j As Integer=0
				For i As Integer = 0 To numAttrs - 1
					retList(j) = (CType(attrList(i), ModelMBeanAttributeInfo).descriptor)
					j += 1
				Next i
				For i As Integer = 0 To numCons - 1
					retList(j) = (CType(consList(i), ModelMBeanConstructorInfo).descriptor)
					j += 1
				Next i
				For i As Integer = 0 To numOpers - 1
					retList(j) = (CType(operList(i), ModelMBeanOperationInfo).descriptor)
					j += 1
				Next i
				For i As Integer = 0 To numNotifs - 1
					retList(j) = (CType(notifList(i), ModelMBeanNotificationInfo).descriptor)
					j += 1
				Next i
			Else
				Dim iae As New System.ArgumentException("Descriptor Type is invalid")
				Dim msg As String = "Exception occurred trying to find" & " the descriptors of the MBean"
				Throw New javax.management.RuntimeOperationsException(iae,msg)
			End If
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanInfoSupport).name, "getDescriptors(String)", "Exit")

			Return retList
		End Function


		Public Overridable Property descriptors Implements ModelMBeanInfo.setDescriptors As javax.management.Descriptor()
			Set(ByVal inDescriptors As javax.management.Descriptor())
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanInfoSupport).name, "setDescriptors(Descriptor[])", "Entry")
				If inDescriptors Is Nothing Then Throw New javax.management.RuntimeOperationsException(New System.ArgumentException("Descriptor list is invalid"), "Exception occurred trying to set the descriptors " & "of the MBeanInfo")
				If inDescriptors.Length = 0 Then ' empty list, no-op Return
				For j As Integer = 0 To inDescriptors.Length - 1
					descriptortor(inDescriptors(j),Nothing)
				Next j
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanInfoSupport).name, "setDescriptors(Descriptor[])", "Exit")
    
			End Set
		End Property


		''' <summary>
		''' Returns a Descriptor requested by name.
		''' </summary>
		''' <param name="inDescriptorName"> The name of the descriptor.
		''' </param>
		''' <returns> Descriptor containing a descriptor for the ModelMBean with the
		'''         same name. If no descriptor is found, null is returned.
		''' </returns>
		''' <exception cref="MBeanException"> Wraps a distributed communication Exception. </exception>
		''' <exception cref="RuntimeOperationsException"> Wraps an IllegalArgumentException
		'''            for null name.
		''' </exception>
		''' <seealso cref= #setDescriptor </seealso>

		Public Overridable Function getDescriptor(ByVal inDescriptorName As String) As javax.management.Descriptor
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanInfoSupport).name, "getDescriptor(String)", "Entry")
			Return (getDescriptor(inDescriptorName, Nothing))
		End Function


		Public Overridable Function getDescriptor(ByVal inDescriptorName As String, ByVal inDescriptorType As String) As javax.management.Descriptor Implements ModelMBeanInfo.getDescriptor
			If inDescriptorName Is Nothing Then Throw New javax.management.RuntimeOperationsException(New System.ArgumentException("Descriptor is invalid"), "Exception occurred trying to set the descriptors of " & "the MBeanInfo")

			If MMB.ToUpper() = inDescriptorType.ToUpper() Then Return CType(modelMBeanDescriptor.clone(), javax.management.Descriptor)

	'             The logic here is a bit convoluted, because we are
	'               dealing with two possible cases, depending on whether
	'               inDescriptorType is null.  If it's not null, then only
	'               one of the following ifs will run, and it will either
	'               return a descriptor or null.  If inDescriptorType is
	'               null, then all of the following ifs will run until one
	'               of them finds a descriptor.  
			If ATTR.ToUpper() = inDescriptorType.ToUpper() OrElse inDescriptorType Is Nothing Then
				Dim ___attr As ModelMBeanAttributeInfo = getAttribute(inDescriptorName)
				If ___attr IsNot Nothing Then Return ___attr.descriptor
				If inDescriptorType IsNot Nothing Then Return Nothing
			End If
			If OPER.ToUpper() = inDescriptorType.ToUpper() OrElse inDescriptorType Is Nothing Then
				Dim ___oper As ModelMBeanOperationInfo = getOperation(inDescriptorName)
				If ___oper IsNot Nothing Then Return ___oper.descriptor
				If inDescriptorType IsNot Nothing Then Return Nothing
			End If
			If CONS.ToUpper() = inDescriptorType.ToUpper() OrElse inDescriptorType Is Nothing Then
				Dim ___oper As ModelMBeanConstructorInfo = getConstructor(inDescriptorName)
				If ___oper IsNot Nothing Then Return ___oper.descriptor
				If inDescriptorType IsNot Nothing Then Return Nothing
			End If
			If NOTF.ToUpper() = inDescriptorType.ToUpper() OrElse inDescriptorType Is Nothing Then
				Dim notif As ModelMBeanNotificationInfo = getNotification(inDescriptorName)
				If notif IsNot Nothing Then Return notif.descriptor
				If inDescriptorType IsNot Nothing Then Return Nothing
			End If
			If inDescriptorType Is Nothing Then Return Nothing
			Throw New javax.management.RuntimeOperationsException(New System.ArgumentException("Descriptor Type is invalid"), "Exception occurred trying to find the descriptors of the MBean")

		End Function



		Public Overridable Sub setDescriptor(ByVal inDescriptor As javax.management.Descriptor, ByVal inDescriptorType As String) Implements ModelMBeanInfo.setDescriptor
			Const excMsg As String = "Exception occurred trying to set the descriptors of the MBean"
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanInfoSupport).name, "setDescriptor(Descriptor,String)", "Entry")

			If inDescriptor Is Nothing Then inDescriptor = New DescriptorSupport

			If (inDescriptorType Is Nothing) OrElse (inDescriptorType.Equals("")) Then
				inDescriptorType = CStr(inDescriptor.getFieldValue("descriptorType"))

				If inDescriptorType Is Nothing Then
					   MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanInfoSupport).name, "setDescriptor(Descriptor,String)", "descriptorType null in both String parameter and Descriptor, defaulting to " & MMB)
					inDescriptorType = MMB
				End If
			End If

			Dim inDescriptorName As String = CStr(inDescriptor.getFieldValue("name"))
			If inDescriptorName Is Nothing Then
				MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanInfoSupport).name, "setDescriptor(Descriptor,String)", "descriptor name null, defaulting to " & Me.className)
				inDescriptorName = Me.className
			End If
			Dim found As Boolean = False
			If inDescriptorType.ToUpper() = MMB.ToUpper() Then
				mBeanDescriptor = inDescriptor
				found = True
			ElseIf inDescriptorType.ToUpper() = ATTR.ToUpper() Then
				Dim attrList As javax.management.MBeanAttributeInfo() = modelMBeanAttributes
				Dim numAttrs As Integer = 0
				If attrList IsNot Nothing Then numAttrs = attrList.Length

				For i As Integer = 0 To numAttrs - 1
					If inDescriptorName.Equals(attrList(i).name) Then
						found = True
						Dim mmbai As ModelMBeanAttributeInfo = CType(attrList(i), ModelMBeanAttributeInfo)
						mmbai.descriptor = inDescriptor
						If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then
							Dim strb As (New StringBuilder).Append("Setting descriptor to ").append(inDescriptor).append(vbTab & vbLf & " local: AttributeInfo descriptor is ").append(mmbai.descriptor).append(vbTab & vbLf & " modelMBeanInfo: AttributeInfo descriptor is ").append(Me.getDescriptor(inDescriptorName,"attribute"))
							MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanInfoSupport).name, "setDescriptor(Descriptor,String)", strb.ToString())
						End If
					End If
				Next i
			ElseIf inDescriptorType.ToUpper() = OPER.ToUpper() Then
				Dim operList As javax.management.MBeanOperationInfo() = modelMBeanOperations
				Dim numOpers As Integer = 0
				If operList IsNot Nothing Then numOpers = operList.Length

				For i As Integer = 0 To numOpers - 1
					If inDescriptorName.Equals(operList(i).name) Then
						found = True
						Dim mmboi As ModelMBeanOperationInfo = CType(operList(i), ModelMBeanOperationInfo)
						mmboi.descriptor = inDescriptor
					End If
				Next i
			ElseIf inDescriptorType.ToUpper() = CONS.ToUpper() Then
				Dim consList As javax.management.MBeanConstructorInfo() = modelMBeanConstructors
				Dim numCons As Integer = 0
				If consList IsNot Nothing Then numCons = consList.Length

				For i As Integer = 0 To numCons - 1
					If inDescriptorName.Equals(consList(i).name) Then
						found = True
						Dim mmbci As ModelMBeanConstructorInfo = CType(consList(i), ModelMBeanConstructorInfo)
						mmbci.descriptor = inDescriptor
					End If
				Next i
			ElseIf inDescriptorType.ToUpper() = NOTF.ToUpper() Then
				Dim notifList As javax.management.MBeanNotificationInfo() = modelMBeanNotifications
				Dim numNotifs As Integer = 0
				If notifList IsNot Nothing Then numNotifs = notifList.Length

				For i As Integer = 0 To numNotifs - 1
					If inDescriptorName.Equals(notifList(i).name) Then
						found = True
						Dim mmbni As ModelMBeanNotificationInfo = CType(notifList(i), ModelMBeanNotificationInfo)
						mmbni.descriptor = inDescriptor
					End If
				Next i
			Else
				Dim iae As Exception = New System.ArgumentException("Invalid descriptor type: " & inDescriptorType)
				Throw New javax.management.RuntimeOperationsException(iae, excMsg)
			End If

			If Not found Then
				Dim iae As Exception = New System.ArgumentException("Descriptor name is invalid: " & "type=" & inDescriptorType & "; name=" & inDescriptorName)
				Throw New javax.management.RuntimeOperationsException(iae, excMsg)
			End If
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanInfoSupport).name, "setDescriptor(Descriptor,String)", "Exit")

		End Sub


		Public Overridable Function getAttribute(ByVal inName As String) As ModelMBeanAttributeInfo Implements ModelMBeanInfo.getAttribute
			Dim retInfo As ModelMBeanAttributeInfo = Nothing
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanInfoSupport).name, "getAttribute(String)", "Entry")
			If inName Is Nothing Then Throw New javax.management.RuntimeOperationsException(New System.ArgumentException("Attribute Name is null"), "Exception occurred trying to get the " & "ModelMBeanAttributeInfo of the MBean")
			Dim attrList As javax.management.MBeanAttributeInfo() = modelMBeanAttributes
			Dim numAttrs As Integer = 0
			If attrList IsNot Nothing Then numAttrs = attrList.Length

			Dim i As Integer=0
			Do While (i < numAttrs) AndAlso (retInfo Is Nothing)
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then
					Dim strb As (New StringBuilder).Append(vbTab & vbLf & " this.getAttributes() MBeanAttributeInfo Array ").append(i).append(":").append(CType(attrList(i), ModelMBeanAttributeInfo).descriptor).append(vbTab & vbLf & " this.modelMBeanAttributes MBeanAttributeInfo Array ").append(i).append(":").append(CType(modelMBeanAttributes(i), ModelMBeanAttributeInfo).descriptor)
					MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanInfoSupport).name, "getAttribute(String)", strb.ToString())
				End If
				If inName.Equals(attrList(i).name) Then retInfo = (CType(attrList(i).clone(), ModelMBeanAttributeInfo))
				i += 1
			Loop
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanInfoSupport).name, "getAttribute(String)", "Exit")

			Return retInfo
		End Function



		Public Overridable Function getOperation(ByVal inName As String) As ModelMBeanOperationInfo Implements ModelMBeanInfo.getOperation
			Dim retInfo As ModelMBeanOperationInfo = Nothing
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanInfoSupport).name, "getOperation(String)", "Entry")
			If inName Is Nothing Then Throw New javax.management.RuntimeOperationsException(New System.ArgumentException("inName is null"), "Exception occurred trying to get the " & "ModelMBeanOperationInfo of the MBean")
			Dim operList As javax.management.MBeanOperationInfo() = modelMBeanOperations 'this.getOperations();
			Dim numOpers As Integer = 0
			If operList IsNot Nothing Then numOpers = operList.Length

			Dim i As Integer=0
			Do While (i < numOpers) AndAlso (retInfo Is Nothing)
				If inName.Equals(operList(i).name) Then retInfo = (CType(operList(i).clone(), ModelMBeanOperationInfo))
				i += 1
			Loop
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanInfoSupport).name, "getOperation(String)", "Exit")

			Return retInfo
		End Function

		''' <summary>
		''' Returns the ModelMBeanConstructorInfo requested by name.
		''' If no ModelMBeanConstructorInfo exists for this name null is returned.
		''' </summary>
		''' <param name="inName"> the name of the constructor.
		''' </param>
		''' <returns> the constructor info for the named constructor, or null
		''' if there is none.
		''' </returns>
		''' <exception cref="MBeanException"> Wraps a distributed communication Exception. </exception>
		''' <exception cref="RuntimeOperationsException"> Wraps an IllegalArgumentException
		'''            for a null constructor name. </exception>

		Public Overridable Function getConstructor(ByVal inName As String) As ModelMBeanConstructorInfo
			Dim retInfo As ModelMBeanConstructorInfo = Nothing
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanInfoSupport).name, "getConstructor(String)", "Entry")
			If inName Is Nothing Then Throw New javax.management.RuntimeOperationsException(New System.ArgumentException("Constructor name is null"), "Exception occurred trying to get the " & "ModelMBeanConstructorInfo of the MBean")
			Dim consList As javax.management.MBeanConstructorInfo() = modelMBeanConstructors 'this.getConstructors();
			Dim numCons As Integer = 0
			If consList IsNot Nothing Then numCons = consList.Length

			Dim i As Integer=0
			Do While (i < numCons) AndAlso (retInfo Is Nothing)
				If inName.Equals(consList(i).name) Then retInfo = (CType(consList(i).clone(), ModelMBeanConstructorInfo))
				i += 1
			Loop
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanInfoSupport).name, "getConstructor(String)", "Exit")

			Return retInfo
		End Function


		Public Overridable Function getNotification(ByVal inName As String) As ModelMBeanNotificationInfo Implements ModelMBeanInfo.getNotification
			Dim retInfo As ModelMBeanNotificationInfo = Nothing
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanInfoSupport).name, "getNotification(String)", "Entry")
			If inName Is Nothing Then Throw New javax.management.RuntimeOperationsException(New System.ArgumentException("Notification name is null"), "Exception occurred trying to get the " & "ModelMBeanNotificationInfo of the MBean")
			Dim notifList As javax.management.MBeanNotificationInfo() = modelMBeanNotifications 'this.getNotifications();
			Dim numNotifs As Integer = 0
			If notifList IsNot Nothing Then numNotifs = notifList.Length

			Dim i As Integer=0
			Do While (i < numNotifs) AndAlso (retInfo Is Nothing)
				If inName.Equals(notifList(i).name) Then retInfo = (CType(notifList(i).clone(), ModelMBeanNotificationInfo))
				i += 1
			Loop
			If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanInfoSupport).name, "getNotification(String)", "Exit")

			Return retInfo
		End Function


		' We override MBeanInfo.getDescriptor() to return our descriptor. 
		''' <summary>
		''' @since 1.6
		''' </summary>
		Public Property Overrides descriptor As javax.management.Descriptor
			Get
				Return mBeanDescriptorNoException
			End Get
		End Property

		Public Overridable Property mBeanDescriptor As javax.management.Descriptor Implements ModelMBeanInfo.getMBeanDescriptor
			Get
				Return mBeanDescriptorNoException
			End Get
			Set(ByVal inMBeanDescriptor As javax.management.Descriptor)
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanInfoSupport).name, "setMBeanDescriptor(Descriptor)", "Entry")
				modelMBeanDescriptor = validDescriptor(inMBeanDescriptor)
			End Set
		End Property

		Private Property mBeanDescriptorNoException As javax.management.Descriptor
			Get
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanInfoSupport).name, "getMBeanDescriptorNoException()", "Entry")
    
				If modelMBeanDescriptor Is Nothing Then modelMBeanDescriptor = validDescriptor(Nothing)
    
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanInfoSupport).name, "getMBeanDescriptorNoException()", "Exit, returning: " & modelMBeanDescriptor)
				Return CType(modelMBeanDescriptor.clone(), javax.management.Descriptor)
			End Get
		End Property



		''' <summary>
		''' Clones the passed in Descriptor, sets default values, and checks for validity.
		''' If the Descriptor is invalid (for instance by having the wrong "name"),
		''' this indicates programming error and a RuntimeOperationsException will be thrown.
		''' 
		''' The following fields will be defaulted if they are not already set:
		''' displayName=className,name=className,descriptorType="mbean",
		''' persistPolicy="never", log="F", visibility="1"
		''' </summary>
		''' <param name="in"> Descriptor to be checked, or null which is equivalent to
		''' an empty Descriptor. </param>
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
				clone.fieldeld("name", Me.className)
				MODELMBEAN_LOGGER.finer("Defaulting Descriptor name to " & Me.className)
			End If
			If defaulted AndAlso clone.getFieldValue("descriptorType") Is Nothing Then
				clone.fieldeld("descriptorType", MMB)
				MODELMBEAN_LOGGER.finer("Defaulting descriptorType to """ & MMB & """")
			End If
			If clone.getFieldValue("displayName") Is Nothing Then
				clone.fieldeld("displayName",Me.className)
				MODELMBEAN_LOGGER.finer("Defaulting Descriptor displayName to " & Me.className)
			End If
			If clone.getFieldValue("persistPolicy") Is Nothing Then
				clone.fieldeld("persistPolicy","never")
				MODELMBEAN_LOGGER.finer("Defaulting Descriptor persistPolicy to ""never""")
			End If
			If clone.getFieldValue("log") Is Nothing Then
				clone.fieldeld("log","F")
				MODELMBEAN_LOGGER.finer("Defaulting Descriptor ""log"" field to ""F""")
			End If
			If clone.getFieldValue("visibility") Is Nothing Then
				clone.fieldeld("visibility","1")
				MODELMBEAN_LOGGER.finer("Defaulting Descriptor visibility to 1")
			End If

			'Checking validity
			If Not clone.valid Then Throw New javax.management.RuntimeOperationsException(New System.ArgumentException("Invalid Descriptor argument"), "The isValid() method of the Descriptor object itself returned false," & "one or more required fields are invalid. Descriptor:" & clone.ToString())

			If (Not CStr(clone.getFieldValue("descriptorType")).ToUpper()) = MMB.ToUpper() Then Throw New javax.management.RuntimeOperationsException(New System.ArgumentException("Invalid Descriptor argument"), "The Descriptor ""descriptorType"" field does not match the object described. " & " Expected: " & MMB & " , was: " & clone.getFieldValue("descriptorType"))

			Return clone
		End Function




		''' <summary>
		''' Deserializes a <seealso cref="ModelMBeanInfoSupport"/> from an <seealso cref="ObjectInputStream"/>.
		''' </summary>
		Private Sub readObject(ByVal [in] As java.io.ObjectInputStream)
			If compat Then
				' Read an object serialized in the old serial form
				'
				Dim fields As java.io.ObjectInputStream.GetField = [in].readFields()
				modelMBeanDescriptor = CType(fields.get("modelMBeanDescriptor", Nothing), javax.management.Descriptor)
				If fields.defaulted("modelMBeanDescriptor") Then Throw New NullPointerException("modelMBeanDescriptor")
				modelMBeanAttributes = CType(fields.get("mmbAttributes", Nothing), javax.management.MBeanAttributeInfo())
				If fields.defaulted("mmbAttributes") Then Throw New NullPointerException("mmbAttributes")
				modelMBeanConstructors = CType(fields.get("mmbConstructors", Nothing), javax.management.MBeanConstructorInfo())
				If fields.defaulted("mmbConstructors") Then Throw New NullPointerException("mmbConstructors")
				modelMBeanNotifications = CType(fields.get("mmbNotifications", Nothing), javax.management.MBeanNotificationInfo())
				If fields.defaulted("mmbNotifications") Then Throw New NullPointerException("mmbNotifications")
				modelMBeanOperations = CType(fields.get("mmbOperations", Nothing), javax.management.MBeanOperationInfo())
				If fields.defaulted("mmbOperations") Then Throw New NullPointerException("mmbOperations")
			Else
				' Read an object serialized in the new serial form
				'
				[in].defaultReadObject()
			End If
		End Sub


		''' <summary>
		''' Serializes a <seealso cref="ModelMBeanInfoSupport"/> to an <seealso cref="ObjectOutputStream"/>.
		''' </summary>
		Private Sub writeObject(ByVal out As java.io.ObjectOutputStream)
			If compat Then
				' Serializes this instance in the old serial form
				'
				Dim fields As java.io.ObjectOutputStream.PutField = out.putFields()
				fields.put("modelMBeanDescriptor", modelMBeanDescriptor)
				fields.put("mmbAttributes", modelMBeanAttributes)
				fields.put("mmbConstructors", modelMBeanConstructors)
				fields.put("mmbNotifications", modelMBeanNotifications)
				fields.put("mmbOperations", modelMBeanOperations)
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