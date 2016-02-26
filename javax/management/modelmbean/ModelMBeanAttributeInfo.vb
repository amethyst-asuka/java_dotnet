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
	''' <p>The ModelMBeanAttributeInfo object describes an attribute of the ModelMBean.
	''' It is a subclass of MBeanAttributeInfo with the addition of an associated Descriptor
	''' and an implementation of the DescriptorAccess interface.</p>
	''' 
	''' <P id="descriptor">
	''' The fields in the descriptor are defined, but not limited to, the following.
	''' Note that when the Type in this table is Number, a String that is the decimal
	''' representation of a Long can also be used.</P>
	''' 
	''' <table border="1" cellpadding="5" summary="ModelMBeanAttributeInfo Fields">
	''' <tr><th>Name</th><th>Type</th><th>Meaning</th></tr>
	''' <tr><td>name</td><td>String</td>
	'''     <td>Attribute name.</td></tr>
	''' <tr><td>descriptorType</td><td>String</td>
	'''     <td>Must be "attribute".</td></tr>
	''' <tr id="value-field"><td>value</td><td>Object</td>
	'''     <td>Current (cached) value for attribute.</td></tr>
	''' <tr><td>default</td><td>Object</td>
	'''     <td>Default value for attribute.</td></tr>
	''' <tr><td>displayName</td><td>String</td>
	'''     <td>Name of attribute to be used in displays.</td></tr>
	''' <tr><td>getMethod</td><td>String</td>
	'''     <td>Name of operation descriptor for get method.</td></tr>
	''' <tr><td>setMethod</td><td>String</td>
	'''     <td>Name of operation descriptor for set method.</td></tr>
	''' <tr><td>protocolMap</td><td>Descriptor</td>
	'''     <td>See the section "Protocol Map Support" in the JMX specification
	'''         document.  Mappings must be appropriate for the attribute and entries
	'''         can be updated or augmented at runtime.</td></tr>
	''' <tr><td>persistPolicy</td><td>String</td>
	'''     <td>One of: OnUpdate|OnTimer|NoMoreOftenThan|OnUnregister|Always|Never.
	'''         See the section "MBean Descriptor Fields" in the JMX specification
	'''         document.</td></tr>
	''' <tr><td>persistPeriod</td><td>Number</td>
	'''     <td>Frequency of persist cycle in seconds. Used when persistPolicy is
	'''         "OnTimer" or "NoMoreOftenThan".</td></tr>
	''' <tr><td>currencyTimeLimit</td><td>Number</td>
	'''     <td>How long <a href="#value=field">value</a> is valid: &lt;0 never,
	'''         =0 always, &gt;0 seconds.</td></tr>
	''' <tr><td>lastUpdatedTimeStamp</td><td>Number</td>
	'''     <td>When <a href="#value-field">value</a> was set.</td></tr>
	''' <tr><td>visibility</td><td>Number</td>
	'''     <td>1-4 where 1: always visible, 4: rarely visible.</td></tr>
	''' <tr><td>presentationString</td><td>String</td>
	'''     <td>XML formatted string to allow presentation of data.</td></tr>
	''' </table>
	''' 
	''' <p>The default descriptor contains the name, descriptorType and displayName
	''' fields.  The default value of the name and displayName fields is the name of
	''' the attribute.</p>
	''' 
	''' <p><b>Note:</b> because of inconsistencies in previous versions of
	''' this specification, it is recommended not to use negative or zero
	''' values for <code>currencyTimeLimit</code>.  To indicate that a
	''' cached value is never valid, omit the
	''' <code>currencyTimeLimit</code> field.  To indicate that it is
	''' always valid, use a very large number for this field.</p>
	''' 
	''' <p>The <b>serialVersionUID</b> of this class is <code>6181543027787327345L</code>.
	''' 
	''' @since 1.5
	''' </summary>

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class ModelMBeanAttributeInfo
		Inherits javax.management.MBeanAttributeInfo
		Implements javax.management.DescriptorAccess ' serialVersionUID is not constant

		' Serialization compatibility stuff:
		' Two serial forms are supported in this class. The selected form depends
		' on system property "jmx.serial.form":
		'  - "1.0" for JMX 1.0
		'  - any other value for JMX 1.1 and higher
		'
		' Serial version for old serial form
		Private Const oldSerialVersionUID As Long = 7098036920755973145L
		'
		' Serial version for new serial form
		Private Const newSerialVersionUID As Long = 6181543027787327345L
		'
		' Serializable fields in old serial form
		Private Shared ReadOnly oldSerialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("attrDescriptor", GetType(javax.management.Descriptor)), New java.io.ObjectStreamField("currClass", GetType(String)) }
		'
		' Serializable fields in new serial form
		Private Shared ReadOnly newSerialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("attrDescriptor", GetType(javax.management.Descriptor)) }
		'
		' Actual serial version and serial form
		Private Shared ReadOnly Shadows serialVersionUID As Long
		''' <summary>
		''' @serialField attrDescriptor Descriptor The <seealso cref="Descriptor"/>
		''' containing the metadata corresponding to this attribute
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
			''' @serial The <seealso cref="Descriptor"/> containing the metadata corresponding to
			''' this attribute
			''' </summary>
			Private attrDescriptor As javax.management.Descriptor = validDescriptor(Nothing)

			Private Const currClass As String = "ModelMBeanAttributeInfo"

			''' <summary>
			''' Constructs a ModelMBeanAttributeInfo object with a default
			''' descriptor. The <seealso cref="Descriptor"/> of the constructed
			''' object will include fields contributed by any annotations
			''' on the {@code Method} objects that contain the {@link
			''' DescriptorKey} meta-annotation.
			''' </summary>
			''' <param name="name"> The name of the attribute. </param>
			''' <param name="description"> A human readable description of the attribute. Optional. </param>
			''' <param name="getter"> The method used for reading the attribute value.
			'''          May be null if the property is write-only. </param>
			''' <param name="setter"> The method used for writing the attribute value.
			'''          May be null if the attribute is read-only. </param>
			''' <exception cref="javax.management.IntrospectionException"> There is a consistency
			''' problem in the definition of this attribute.
			'''  </exception>

			Public Sub New(ByVal name As String, ByVal description As String, ByVal getter As Method, ByVal setter As Method)
					MyBase.New(name, description, getter, setter)

					If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanAttributeInfo).name, "ModelMBeanAttributeInfo(" & "String,String,Method,Method)", "Entry", name)

					attrDescriptor = validDescriptor(Nothing)
					' put getter and setter methods in operations list
					' create default descriptor

			End Sub

			''' <summary>
			''' Constructs a ModelMBeanAttributeInfo object.  The {@link
			''' Descriptor} of the constructed object will include fields
			''' contributed by any annotations on the {@code Method}
			''' objects that contain the <seealso cref="DescriptorKey"/>
			''' meta-annotation.
			''' </summary>
			''' <param name="name"> The name of the attribute. </param>
			''' <param name="description"> A human readable description of the attribute. Optional. </param>
			''' <param name="getter"> The method used for reading the attribute value.
			'''          May be null if the property is write-only. </param>
			''' <param name="setter"> The method used for writing the attribute value.
			'''          May be null if the attribute is read-only. </param>
			''' <param name="descriptor"> An instance of Descriptor containing the
			''' appropriate metadata for this instance of the Attribute. If
			''' it is null, then a default descriptor will be created.  If
			''' the descriptor does not contain the field "displayName" this field is added
			''' in the descriptor with its default value. </param>
			''' <exception cref="javax.management.IntrospectionException"> There is a consistency
			''' problem in the definition of this attribute. </exception>
			''' <exception cref="RuntimeOperationsException"> Wraps an
			''' IllegalArgumentException. The descriptor is invalid, or descriptor
			''' field "name" is not equal to name parameter, or descriptor field
			''' "descriptorType" is not equal to "attribute".
			'''  </exception>

			Public Sub New(ByVal name As String, ByVal description As String, ByVal getter As Method, ByVal setter As Method, ByVal descriptor As javax.management.Descriptor)

					MyBase.New(name, description, getter, setter)
					' put getter and setter methods in operations list
					If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanAttributeInfo).name, "ModelMBeanAttributeInfo(" & "String,String,Method,Method,Descriptor)", "Entry", name)
					attrDescriptor = validDescriptor(descriptor)
			End Sub

			''' <summary>
			''' Constructs a ModelMBeanAttributeInfo object with a default descriptor.
			''' </summary>
			''' <param name="name"> The name of the attribute </param>
			''' <param name="type"> The type or class name of the attribute </param>
			''' <param name="description"> A human readable description of the attribute. </param>
			''' <param name="isReadable"> True if the attribute has a getter method, false otherwise. </param>
			''' <param name="isWritable"> True if the attribute has a setter method, false otherwise. </param>
			''' <param name="isIs"> True if the attribute has an "is" getter, false otherwise.
			'''  </param>
			Public Sub New(ByVal name As String, ByVal type As String, ByVal description As String, ByVal isReadable As Boolean, ByVal isWritable As Boolean, ByVal isIs As Boolean)

					MyBase.New(name, type, description, isReadable, isWritable, isIs)
					' create default descriptor
					If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanAttributeInfo).name, "ModelMBeanAttributeInfo(" & "String,String,String,boolean,boolean,boolean)", "Entry", name)
					attrDescriptor = validDescriptor(Nothing)
			End Sub

			''' <summary>
			''' Constructs a ModelMBeanAttributeInfo object.
			''' </summary>
			''' <param name="name"> The name of the attribute </param>
			''' <param name="type"> The type or class name of the attribute </param>
			''' <param name="description"> A human readable description of the attribute. </param>
			''' <param name="isReadable"> True if the attribute has a getter method, false otherwise. </param>
			''' <param name="isWritable"> True if the attribute has a setter method, false otherwise. </param>
			''' <param name="isIs"> True if the attribute has an "is" getter, false otherwise. </param>
			''' <param name="descriptor"> An instance of Descriptor containing the
			''' appropriate metadata for this instance of the Attribute. If
			''' it is null then a default descriptor will be created.  If
			''' the descriptor does not contain the field "displayName" this field
			''' is added in the descriptor with its default value. </param>
			''' <exception cref="RuntimeOperationsException"> Wraps an
			''' IllegalArgumentException. The descriptor is invalid, or descriptor
			''' field "name" is not equal to name parameter, or descriptor field
			''' "descriptorType" is not equal to "attribute".
			'''  </exception>
			Public Sub New(ByVal name As String, ByVal type As String, ByVal description As String, ByVal isReadable As Boolean, ByVal isWritable As Boolean, ByVal isIs As Boolean, ByVal descriptor As javax.management.Descriptor)
					MyBase.New(name, type, description, isReadable, isWritable, isIs)
					If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanAttributeInfo).name, "ModelMBeanAttributeInfo(String,String,String," & "boolean,boolean,boolean,Descriptor)", "Entry", name)
					attrDescriptor = validDescriptor(descriptor)
			End Sub

			''' <summary>
			''' Constructs a new ModelMBeanAttributeInfo object from this
			''' ModelMBeanAttributeInfo Object.  A default descriptor will
			''' be created.
			''' </summary>
			''' <param name="inInfo"> the ModelMBeanAttributeInfo to be duplicated </param>

			Public Sub New(ByVal inInfo As ModelMBeanAttributeInfo)
					MyBase.New(inInfo.name, inInfo.type, inInfo.description, inInfo.readable, inInfo.writable, inInfo.is)
					If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanAttributeInfo).name, "ModelMBeanAttributeInfo(ModelMBeanAttributeInfo)", "Entry")
					Dim newDesc As javax.management.Descriptor = inInfo.descriptor
					attrDescriptor = validDescriptor(newDesc)
			End Sub

			''' <summary>
			''' Gets a copy of the associated Descriptor for the
			''' ModelMBeanAttributeInfo.
			''' </summary>
			''' <returns> Descriptor associated with the
			''' ModelMBeanAttributeInfo object.
			''' </returns>
			''' <seealso cref= #setDescriptor </seealso>

			Public Property Overrides descriptor As javax.management.Descriptor
				Get
					If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanAttributeInfo).name, "getDescriptor()", "Entry")
						If attrDescriptor Is Nothing Then attrDescriptor = validDescriptor(Nothing)
						Return (CType(attrDescriptor.clone(), javax.management.Descriptor))
				End Get
				Set(ByVal inDescriptor As javax.management.Descriptor)
					attrDescriptor = validDescriptor(inDescriptor)
				End Set
			End Property



			''' <summary>
			''' Creates and returns a new ModelMBeanAttributeInfo which is a duplicate of this ModelMBeanAttributeInfo.
			''' </summary>
			''' <exception cref="RuntimeOperationsException"> for illegal value for
			''' field Names or field Values.  If the descriptor construction
			''' fails for any reason, this exception will be thrown. </exception>

			Public Overrides Function clone() As Object
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanAttributeInfo).name, "clone()", "Entry")
					Return (New ModelMBeanAttributeInfo(Me))
			End Function

			''' <summary>
			''' Returns a human-readable version of the
			''' ModelMBeanAttributeInfo instance.
			''' </summary>
			Public Overrides Function ToString() As String
				Return "ModelMBeanAttributeInfo: " & Me.name & " ; Description: " & Me.description & " ; Types: " & Me.type & " ; isReadable: " & Me.readable & " ; isWritable: " & Me.writable & " ; Descriptor: " & Me.descriptor
			End Function


			''' <summary>
			''' Clones the passed in Descriptor, sets default values, and checks for validity.
			''' If the Descriptor is invalid (for instance by having the wrong "name"),
			''' this indicates programming error and a RuntimeOperationsException will be thrown.
			''' 
			''' The following fields will be defaulted if they are not already set:
			''' displayName=this.getName(),name=this.getName(),descriptorType = "attribute"
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
					clone.fieldeld("descriptorType", "attribute")
					MODELMBEAN_LOGGER.finer("Defaulting descriptorType to ""attribute""")
				End If
				If clone.getFieldValue("displayName") Is Nothing Then
					clone.fieldeld("displayName",Me.name)
					MODELMBEAN_LOGGER.finer("Defaulting Descriptor displayName to " & Me.name)
				End If

				'Checking validity
				If Not clone.valid Then Throw New javax.management.RuntimeOperationsException(New System.ArgumentException("Invalid Descriptor argument"), "The isValid() method of the Descriptor object itself returned false," & "one or more required fields are invalid. Descriptor:" & clone.ToString())
				If (Not name.ToUpper()) = CStr(clone.getFieldValue("name")).ToUpper() Then Throw New javax.management.RuntimeOperationsException(New System.ArgumentException("Invalid Descriptor argument"), "The Descriptor ""name"" field does not match the object described. " & " Expected: " & Me.name & " , was: " & clone.getFieldValue("name"))

				If Not "attribute".equalsIgnoreCase(CStr(clone.getFieldValue("descriptorType"))) Then Throw New javax.management.RuntimeOperationsException(New System.ArgumentException("Invalid Descriptor argument"), "The Descriptor ""descriptorType"" field does not match the object described. " & " Expected: ""attribute"" ," & " was: " & clone.getFieldValue("descriptorType"))

				Return clone
			End Function


		''' <summary>
		''' Deserializes a <seealso cref="ModelMBeanAttributeInfo"/> from an <seealso cref="ObjectInputStream"/>.
		''' </summary>
		Private Sub readObject(ByVal [in] As java.io.ObjectInputStream)
		  ' New serial form ignores extra field "currClass"
		  [in].defaultReadObject()
		End Sub


		''' <summary>
		''' Serializes a <seealso cref="ModelMBeanAttributeInfo"/> to an <seealso cref="ObjectOutputStream"/>.
		''' </summary>
		Private Sub writeObject(ByVal out As java.io.ObjectOutputStream)
		  If compat Then
			' Serializes this instance in the old serial form
			'
			Dim fields As java.io.ObjectOutputStream.PutField = out.putFields()
			fields.put("attrDescriptor", attrDescriptor)
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