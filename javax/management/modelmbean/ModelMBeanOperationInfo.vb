Imports System
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
	''' <p>The ModelMBeanOperationInfo object describes a management operation of
	''' the ModelMBean.  It is a subclass of MBeanOperationInfo with the addition
	''' of an associated Descriptor and an implementation of the DescriptorAccess
	''' interface.</p>
	''' 
	''' <P id="descriptor">
	''' The fields in the descriptor are defined, but not limited to, the following.
	''' Note that when the Type in this table is Number, a String that is the decimal
	''' representation of a Long can also be used.</P>
	''' 
	''' <table border="1" cellpadding="5" summary="ModelMBeanOperationInfo Fields">
	''' <tr><th>Name</th><th>Type</th><th>Meaning</th></tr>
	''' <tr><td>name</td><td>String</td>
	'''     <td>Operation name.</td></tr>
	''' <tr><td>descriptorType</td><td>String</td>
	'''     <td>Must be "operation".</td></tr>
	''' <tr><td>class</td><td>String</td>
	'''     <td>Class where method is defined (fully qualified).</td></tr>
	''' <tr><td>role</td><td>String</td>
	'''     <td>Must be "operation", "getter", or "setter".</td></tr>
	''' <tr><td>targetObject</td><td>Object</td>
	'''     <td>Object on which to execute this method.</td></tr>
	''' <tr><td>targetType</td><td>String</td>
	'''     <td>type of object reference for targetObject. Can be:
	'''         ObjectReference | Handle | EJBHandle | IOR | RMIReference.</td></tr>
	''' <tr><td>value</td><td>Object</td>
	'''     <td>Cached value for operation.</td></tr>
	''' <tr><td>displayName</td><td>String</td>
	'''     <td>Human readable display name of the operation.</td>
	''' <tr><td>currencyTimeLimit</td><td>Number</td>
	'''     <td>How long cached value is valid.</td></tr>
	''' <tr><td>lastUpdatedTimeStamp</td><td>Number</td>
	'''     <td>When cached value was set.</td></tr>
	''' <tr><td>visibility</td><td>Number</td>
	'''     <td>1-4 where 1: always visible 4: rarely visible.</td></tr>
	''' <tr><td>presentationString</td><td>String</td>
	'''     <td>XML formatted string to describe how to present operation</td></tr>
	''' </table>
	''' 
	''' <p>The default descriptor will have name, descriptorType, displayName and
	''' role fields set.  The default value of the name and displayName fields is
	''' the operation name.</p>
	''' 
	''' <p><b>Note:</b> because of inconsistencies in previous versions of
	''' this specification, it is recommended not to use negative or zero
	''' values for <code>currencyTimeLimit</code>.  To indicate that a
	''' cached value is never valid, omit the
	''' <code>currencyTimeLimit</code> field.  To indicate that it is
	''' always valid, use a very large number for this field.</p>
	''' 
	''' <p>The <b>serialVersionUID</b> of this class is <code>6532732096650090465L</code>.
	''' 
	''' @since 1.5
	''' </summary>

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class ModelMBeanOperationInfo
		Inherits javax.management.MBeanOperationInfo
		Implements javax.management.DescriptorAccess ' serialVersionUID is not constant

		' Serialization compatibility stuff:
		' Two serial forms are supported in this class. The selected form depends
		' on system property "jmx.serial.form":
		'  - "1.0" for JMX 1.0
		'  - any other value for JMX 1.1 and higher
		'
		' Serial version for old serial form
		Private Const oldSerialVersionUID As Long = 9087646304346171239L
		'
		' Serial version for new serial form
		Private Const newSerialVersionUID As Long = 6532732096650090465L
		'
		' Serializable fields in old serial form
		Private Shared ReadOnly oldSerialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("operationDescriptor", GetType(javax.management.Descriptor)), New java.io.ObjectStreamField("currClass", GetType(String)) }
		'
		' Serializable fields in new serial form
		Private Shared ReadOnly newSerialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("operationDescriptor", GetType(javax.management.Descriptor)) }
		'
		' Actual serial version and serial form
		Private Shared ReadOnly Shadows serialVersionUID As Long
		''' <summary>
		''' @serialField operationDescriptor Descriptor The descriptor
		''' containing the appropriate metadata for this instance
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
			''' @serial The descriptor containing the appropriate metadata for this instance
			''' </summary>
			Private operationDescriptor As javax.management.Descriptor = validDescriptor(Nothing)

			Private Const currClass As String = "ModelMBeanOperationInfo"

			''' <summary>
			''' Constructs a ModelMBeanOperationInfo object with a default
			''' descriptor. The <seealso cref="Descriptor"/> of the constructed
			''' object will include fields contributed by any annotations
			''' on the {@code Method} object that contain the {@link
			''' DescriptorKey} meta-annotation.
			''' </summary>
			''' <param name="operationMethod"> The java.lang.reflect.Method object
			''' describing the MBean operation. </param>
			''' <param name="description"> A human readable description of the operation. </param>

			Public Sub New(ByVal description As String, ByVal operationMethod As Method)
					MyBase.New(description, operationMethod)
					' create default descriptor
					If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanOperationInfo).name, "ModelMBeanOperationInfo(String,Method)", "Entry")
					operationDescriptor = validDescriptor(Nothing)
			End Sub

			''' <summary>
			''' Constructs a ModelMBeanOperationInfo object. The {@link
			''' Descriptor} of the constructed object will include fields
			''' contributed by any annotations on the {@code Method} object
			''' that contain the <seealso cref="DescriptorKey"/> meta-annotation.
			''' </summary>
			''' <param name="operationMethod"> The java.lang.reflect.Method object
			''' describing the MBean operation. </param>
			''' <param name="description"> A human readable description of the
			''' operation. </param>
			''' <param name="descriptor"> An instance of Descriptor containing the
			''' appropriate metadata for this instance of the
			''' ModelMBeanOperationInfo.  If it is null a default
			''' descriptor will be created. If the descriptor does not
			''' contain the fields
			''' "displayName" or "role", the missing ones are added with
			''' their default values.
			''' </param>
			''' <exception cref="RuntimeOperationsException"> Wraps an
			''' IllegalArgumentException. The descriptor is invalid; or
			''' descriptor field "name" is not equal to
			''' operation name; or descriptor field "DescriptorType" is
			''' not equal to "operation"; or descriptor
			''' optional field "role" is present but not equal to "operation",
			''' "getter", or "setter".
			'''  </exception>

			Public Sub New(ByVal description As String, ByVal operationMethod As Method, ByVal descriptor As javax.management.Descriptor)

					MyBase.New(description, operationMethod)
					If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanOperationInfo).name, "ModelMBeanOperationInfo(String,Method,Descriptor)", "Entry")
					operationDescriptor = validDescriptor(descriptor)
			End Sub

			''' <summary>
			''' Constructs a ModelMBeanOperationInfo object with a default descriptor.
			''' </summary>
			''' <param name="name"> The name of the method. </param>
			''' <param name="description"> A human readable description of the operation. </param>
			''' <param name="signature"> MBeanParameterInfo objects describing the
			''' parameters(arguments) of the method. </param>
			''' <param name="type"> The type of the method's return value. </param>
			''' <param name="impact"> The impact of the method, one of INFO, ACTION,
			''' ACTION_INFO, UNKNOWN. </param>

			Public Sub New(ByVal name As String, ByVal description As String, ByVal signature As javax.management.MBeanParameterInfo(), ByVal type As String, ByVal impact As Integer)

					MyBase.New(name, description, signature, type, impact)
					' create default descriptor
					If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanOperationInfo).name, "ModelMBeanOperationInfo(" & "String,String,MBeanParameterInfo[],String,int)", "Entry")
					operationDescriptor = validDescriptor(Nothing)
			End Sub

			''' <summary>
			''' Constructs a ModelMBeanOperationInfo object.
			''' </summary>
			''' <param name="name"> The name of the method. </param>
			''' <param name="description"> A human readable description of the operation. </param>
			''' <param name="signature"> MBeanParameterInfo objects describing the
			''' parameters(arguments) of the method. </param>
			''' <param name="type"> The type of the method's return value. </param>
			''' <param name="impact"> The impact of the method, one of INFO, ACTION,
			''' ACTION_INFO, UNKNOWN. </param>
			''' <param name="descriptor"> An instance of Descriptor containing the
			''' appropriate metadata for this instance of the
			''' MBeanOperationInfo. If it is null then a default descriptor
			''' will be created.  If the descriptor does not contain
			''' fields "displayName" or "role",
			''' the missing ones are added with their default values.
			''' </param>
			''' <exception cref="RuntimeOperationsException"> Wraps an
			''' IllegalArgumentException. The descriptor is invalid; or
			''' descriptor field "name" is not equal to
			''' operation name; or descriptor field "DescriptorType" is
			''' not equal to "operation"; or descriptor optional
			''' field "role" is present but not equal to "operation", "getter", or
			''' "setter". </exception>

			Public Sub New(ByVal name As String, ByVal description As String, ByVal signature As javax.management.MBeanParameterInfo(), ByVal type As String, ByVal impact As Integer, ByVal descriptor As javax.management.Descriptor)
					MyBase.New(name, description, signature, type, impact)
					If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanOperationInfo).name, "ModelMBeanOperationInfo(String,String," & "MBeanParameterInfo[],String,int,Descriptor)", "Entry")
					operationDescriptor = validDescriptor(descriptor)
			End Sub

			''' <summary>
			''' Constructs a new ModelMBeanOperationInfo object from this
			''' ModelMBeanOperation Object.
			''' </summary>
			''' <param name="inInfo"> the ModelMBeanOperationInfo to be duplicated
			'''  </param>

			Public Sub New(ByVal inInfo As ModelMBeanOperationInfo)
					MyBase.New(inInfo.name, inInfo.description, inInfo.signature, inInfo.returnType, inInfo.impact)
					If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanOperationInfo).name, "ModelMBeanOperationInfo(ModelMBeanOperationInfo)", "Entry")
					Dim newDesc As javax.management.Descriptor = inInfo.descriptor
					operationDescriptor = validDescriptor(newDesc)
			End Sub

			''' <summary>
			''' Creates and returns a new ModelMBeanOperationInfo which is a
			''' duplicate of this ModelMBeanOperationInfo.
			''' 
			''' </summary>

			Public Overrides Function clone() As Object
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanOperationInfo).name, "clone()", "Entry")
					Return (New ModelMBeanOperationInfo(Me))
			End Function

			''' <summary>
			''' Returns a copy of the associated Descriptor of the
			''' ModelMBeanOperationInfo.
			''' </summary>
			''' <returns> Descriptor associated with the
			''' ModelMBeanOperationInfo object.
			''' </returns>
			''' <seealso cref= #setDescriptor </seealso>

			Public Property Overrides descriptor As javax.management.Descriptor
				Get
					If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanOperationInfo).name, "getDescriptor()", "Entry")
					If operationDescriptor Is Nothing Then operationDescriptor = validDescriptor(Nothing)
    
					Return (CType(operationDescriptor.clone(), javax.management.Descriptor))
				End Get
				Set(ByVal inDescriptor As javax.management.Descriptor)
					If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanOperationInfo).name, "setDescriptor(Descriptor)", "Entry")
					operationDescriptor = validDescriptor(inDescriptor)
				End Set
			End Property


			''' <summary>
			''' Returns a string containing the entire contents of the
			''' ModelMBeanOperationInfo in human readable form.
			''' </summary>
			Public Overrides Function ToString() As String
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanOperationInfo).name, "toString()", "Entry")
					Dim retStr As String = "ModelMBeanOperationInfo: " & Me.name & " ; Description: " & Me.description & " ; Descriptor: " & Me.descriptor & " ; ReturnType: " & Me.returnType & " ; Signature: "
					Dim pTypes As javax.management.MBeanParameterInfo() = Me.signature
					For i As Integer = 0 To pTypes.Length - 1
							retStr = retStr + (pTypes(i)).type & ", "
					Next i
					Return retStr
			End Function

			''' <summary>
			''' Clones the passed in Descriptor, sets default values, and checks for validity.
			''' If the Descriptor is invalid (for instance by having the wrong "name"),
			''' this indicates programming error and a RuntimeOperationsException will be thrown.
			''' 
			''' The following fields will be defaulted if they are not already set:
			''' displayName=this.getName(),name=this.getName(),
			''' descriptorType="operation", role="operation"
			''' 
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
					clone.fieldeld("name", Me.name)
					MODELMBEAN_LOGGER.finer("Defaulting Descriptor name to " & Me.name)
				End If
				If defaulted AndAlso clone.getFieldValue("descriptorType") Is Nothing Then
					clone.fieldeld("descriptorType", "operation")
					MODELMBEAN_LOGGER.finer("Defaulting descriptorType to ""operation""")
				End If
				If clone.getFieldValue("displayName") Is Nothing Then
					clone.fieldeld("displayName",Me.name)
					MODELMBEAN_LOGGER.finer("Defaulting Descriptor displayName to " & Me.name)
				End If
				If clone.getFieldValue("role") Is Nothing Then
					clone.fieldeld("role","operation")
					MODELMBEAN_LOGGER.finer("Defaulting Descriptor role field to ""operation""")
				End If

				'Checking validity
				If Not clone.valid Then Throw New javax.management.RuntimeOperationsException(New System.ArgumentException("Invalid Descriptor argument"), "The isValid() method of the Descriptor object itself returned false," & "one or more required fields are invalid. Descriptor:" & clone.ToString())
				If (Not name.ToUpper()) = CStr(clone.getFieldValue("name")).ToUpper() Then Throw New javax.management.RuntimeOperationsException(New System.ArgumentException("Invalid Descriptor argument"), "The Descriptor ""name"" field does not match the object described. " & " Expected: " & Me.name & " , was: " & clone.getFieldValue("name"))
				If Not "operation".equalsIgnoreCase(CStr(clone.getFieldValue("descriptorType"))) Then Throw New javax.management.RuntimeOperationsException(New System.ArgumentException("Invalid Descriptor argument"), "The Descriptor ""descriptorType"" field does not match the object described. " & " Expected: ""operation"" ," & " was: " & clone.getFieldValue("descriptorType"))
				Dim role As String = CStr(clone.getFieldValue("role"))
				If Not(role.ToUpper() = "operation".ToUpper() OrElse role.ToUpper() = "setter".ToUpper() OrElse role.ToUpper() = "getter".ToUpper()) Then Throw New javax.management.RuntimeOperationsException(New System.ArgumentException("Invalid Descriptor argument"), "The Descriptor ""role"" field does not match the object described. " & " Expected: ""operation"", ""setter"", or ""getter"" ," & " was: " & clone.getFieldValue("role"))
				Dim targetValue As Object = clone.getFieldValue("targetType")
				If targetValue IsNot Nothing Then
					If Not(TypeOf targetValue Is String) Then Throw New javax.management.RuntimeOperationsException(New System.ArgumentException("Invalid Descriptor argument"), "The Descriptor field ""targetValue"" is invalid class. " & " Expected: java.lang.String, " & " was: " & targetValue.GetType().name)
				End If
				Return clone
			End Function

		''' <summary>
		''' Deserializes a <seealso cref="ModelMBeanOperationInfo"/> from an <seealso cref="ObjectInputStream"/>.
		''' </summary>
		Private Sub readObject(ByVal [in] As java.io.ObjectInputStream)
		  ' New serial form ignores extra field "currClass"
		  [in].defaultReadObject()
		End Sub


		''' <summary>
		''' Serializes a <seealso cref="ModelMBeanOperationInfo"/> to an <seealso cref="ObjectOutputStream"/>.
		''' </summary>
		Private Sub writeObject(ByVal out As java.io.ObjectOutputStream)
		  If compat Then
			' Serializes this instance in the old serial form
			'
			Dim fields As java.io.ObjectOutputStream.PutField = out.putFields()
			fields.put("operationDescriptor", operationDescriptor)
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