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
	''' <p>The ModelMBeanConstructorInfo object describes a constructor of the ModelMBean.
	''' It is a subclass of MBeanConstructorInfo with the addition of an associated Descriptor
	''' and an implementation of the DescriptorAccess interface.</p>
	''' 
	''' <P id="descriptor">
	''' The fields in the descriptor are defined, but not limited to, the following.
	''' Note that when the Type in this table is Number, a String that is the decimal
	''' representation of a Long can also be used.</P>
	''' 
	''' <table border="1" cellpadding="5" summary="ModelMBeanConstructorInfo Fields">
	''' <tr><th>Name</th><th>Type</th><th>Meaning</th></tr>
	''' <tr><td>name</td><td>String</td>
	'''     <td>Constructor name.</td></tr>
	''' <tr><td>descriptorType</td><td>String</td>
	'''     <td>Must be "operation".</td></tr>
	''' <tr><td>role</td><td>String</td>
	'''     <td>Must be "constructor".</td></tr>
	''' <tr><td>displayName</td><td>String</td>
	'''     <td>Human readable name of constructor.</td></tr>
	''' <tr><td>visibility</td><td>Number</td>
	'''     <td>1-4 where 1: always visible 4: rarely visible.</td></tr>
	''' <tr><td>presentationString</td><td>String</td>
	'''     <td>XML formatted string to describe how to present operation</td></tr>
	''' </table>
	''' 
	''' <p>The {@code persistPolicy} and {@code currencyTimeLimit} fields
	''' are meaningless for constructors, but are not considered invalid.</p>
	''' 
	''' <p>The default descriptor will have the {@code name}, {@code
	''' descriptorType}, {@code displayName} and {@code role} fields.
	''' 
	''' <p>The <b>serialVersionUID</b> of this class is <code>3862947819818064362L</code>.
	''' 
	''' @since 1.5
	''' </summary>

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class ModelMBeanConstructorInfo
		Inherits javax.management.MBeanConstructorInfo
		Implements javax.management.DescriptorAccess ' serialVersionUID is not constant

		' Serialization compatibility stuff:
		' Two serial forms are supported in this class. The selected form depends
		' on system property "jmx.serial.form":
		'  - "1.0" for JMX 1.0
		'  - any other value for JMX 1.1 and higher
		'
		' Serial version for old serial form
		Private Const oldSerialVersionUID As Long = -4440125391095574518L
		'
		' Serial version for new serial form
		Private Const newSerialVersionUID As Long = 3862947819818064362L
		'
		' Serializable fields in old serial form
		Private Shared ReadOnly oldSerialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("consDescriptor", GetType(javax.management.Descriptor)), New java.io.ObjectStreamField("currClass", GetType(String)) }
		'
		' Serializable fields in new serial form
		Private Shared ReadOnly newSerialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("consDescriptor", GetType(javax.management.Descriptor)) }
		'
		' Actual serial version and serial form
		Private Shared ReadOnly Shadows serialVersionUID As Long
		''' <summary>
		''' @serialField consDescriptor Descriptor The <seealso cref="Descriptor"/> containing the metadata for this instance
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
			''' @serial The <seealso cref="Descriptor"/> containing the metadata for this instance
			''' </summary>
			Private consDescriptor As javax.management.Descriptor = validDescriptor(Nothing)

			Private Const currClass As String = "ModelMBeanConstructorInfo"


			''' <summary>
			''' Constructs a ModelMBeanConstructorInfo object with a default
			''' descriptor.  The <seealso cref="Descriptor"/> of the constructed
			''' object will include fields contributed by any annotations on
			''' the {@code Constructor} object that contain the {@link
			''' DescriptorKey} meta-annotation.
			''' </summary>
			''' <param name="description"> A human readable description of the constructor. </param>
			''' <param name="constructorMethod"> The java.lang.reflect.Constructor object
			''' describing the MBean constructor. </param>
			Public Sub New(Of T1)(ByVal description As String, ByVal constructorMethod As Constructor(Of T1))
					MyBase.New(description, constructorMethod)
					If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanConstructorInfo).name, "ModelMBeanConstructorInfo(String,Constructor)", "Entry")
					consDescriptor = validDescriptor(Nothing)

					' put getter and setter methods in constructors list
					' create default descriptor

			End Sub

			''' <summary>
			''' Constructs a ModelMBeanConstructorInfo object.  The {@link
			''' Descriptor} of the constructed object will include fields
			''' contributed by any annotations on the {@code Constructor}
			''' object that contain the <seealso cref="DescriptorKey"/>
			''' meta-annotation.
			''' </summary>
			''' <param name="description"> A human readable description of the constructor. </param>
			''' <param name="constructorMethod"> The java.lang.reflect.Constructor object
			''' describing the ModelMBean constructor. </param>
			''' <param name="descriptor"> An instance of Descriptor containing the
			''' appropriate metadata for this instance of the
			''' ModelMBeanConstructorInfo.  If it is null, then a default
			''' descriptor will be created. If the descriptor does not
			''' contain the field "displayName" this field is added in the
			''' descriptor with its default value.
			''' </param>
			''' <exception cref="RuntimeOperationsException"> Wraps an
			''' IllegalArgumentException. The descriptor is invalid, or
			''' descriptor field "name" is not equal to name
			''' parameter, or descriptor field "descriptorType" is
			''' not equal to "operation" or descriptor field "role" is
			''' present but not equal to "constructor". </exception>

			Public Sub New(Of T1)(ByVal description As String, ByVal constructorMethod As Constructor(Of T1), ByVal descriptor As javax.management.Descriptor)

					MyBase.New(description, constructorMethod)
					' put getter and setter methods in constructors list
					If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanConstructorInfo).name, "ModelMBeanConstructorInfo(" & "String,Constructor,Descriptor)", "Entry")
					consDescriptor = validDescriptor(descriptor)
			End Sub
			''' <summary>
			''' Constructs a ModelMBeanConstructorInfo object with a default descriptor.
			''' </summary>
			''' <param name="name"> The name of the constructor. </param>
			''' <param name="description"> A human readable description of the constructor. </param>
			''' <param name="signature"> MBeanParameterInfo object array describing the parameters(arguments) of the constructor. </param>

			Public Sub New(ByVal name As String, ByVal description As String, ByVal signature As javax.management.MBeanParameterInfo())

					MyBase.New(name, description, signature)
					' create default descriptor
					If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanConstructorInfo).name, "ModelMBeanConstructorInfo(" & "String,String,MBeanParameterInfo[])", "Entry")
					consDescriptor = validDescriptor(Nothing)
			End Sub
			''' <summary>
			''' Constructs a ModelMBeanConstructorInfo object.
			''' </summary>
			''' <param name="name"> The name of the constructor. </param>
			''' <param name="description"> A human readable description of the constructor. </param>
			''' <param name="signature"> MBeanParameterInfo objects describing the parameters(arguments) of the constructor. </param>
			''' <param name="descriptor"> An instance of Descriptor containing the appropriate metadata
			'''                   for this instance of the MBeanConstructorInfo. If it is null then a default descriptor will be created.
			''' If the descriptor does not contain the field "displayName" this field
			''' is added in the descriptor with its default value.
			''' </param>
			''' <exception cref="RuntimeOperationsException"> Wraps an
			''' IllegalArgumentException. The descriptor is invalid, or
			''' descriptor field "name" is not equal to name
			''' parameter, or descriptor field "descriptorType" is
			''' not equal to "operation" or descriptor field "role" is
			''' present but not equal to "constructor". </exception>

			Public Sub New(ByVal name As String, ByVal description As String, ByVal signature As javax.management.MBeanParameterInfo(), ByVal descriptor As javax.management.Descriptor)
					MyBase.New(name, description, signature)
					If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanConstructorInfo).name, "ModelMBeanConstructorInfo(" & "String,String,MBeanParameterInfo[],Descriptor)", "Entry")
					consDescriptor = validDescriptor(descriptor)
			End Sub

			''' <summary>
			''' Constructs a new ModelMBeanConstructorInfo object from this ModelMBeanConstructor Object.
			''' </summary>
			''' <param name="old"> the ModelMBeanConstructorInfo to be duplicated
			'''  </param>
			Friend Sub New(ByVal old As ModelMBeanConstructorInfo)
					MyBase.New(old.name, old.description, old.signature)
					If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanConstructorInfo).name, "ModelMBeanConstructorInfo(" & "ModelMBeanConstructorInfo)", "Entry")
					consDescriptor = validDescriptor(consDescriptor)
			End Sub

			''' <summary>
			''' Creates and returns a new ModelMBeanConstructorInfo which is a duplicate of this ModelMBeanConstructorInfo.
			''' 
			''' </summary>
			Public Overrides Function clone() As Object
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanConstructorInfo).name, "clone()", "Entry")
					Return (New ModelMBeanConstructorInfo(Me))
			End Function

			''' <summary>
			''' Returns a copy of the associated Descriptor.
			''' </summary>
			''' <returns> Descriptor associated with the
			''' ModelMBeanConstructorInfo object.
			''' </returns>
			''' <seealso cref= #setDescriptor </seealso>


			Public Property Overrides descriptor As javax.management.Descriptor
				Get
					If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanConstructorInfo).name, "getDescriptor()", "Entry")
					If consDescriptor Is Nothing Then consDescriptor = validDescriptor(Nothing)
					Return (CType(consDescriptor.clone(), javax.management.Descriptor))
				End Get
				Set(ByVal inDescriptor As javax.management.Descriptor)
					If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanConstructorInfo).name, "setDescriptor()", "Entry")
					consDescriptor = validDescriptor(inDescriptor)
				End Set
			End Property

			''' <summary>
			''' Returns a string containing the entire contents of the ModelMBeanConstructorInfo in human readable form.
			''' </summary>
			Public Overrides Function ToString() As String
				If MODELMBEAN_LOGGER.isLoggable(java.util.logging.Level.FINER) Then MODELMBEAN_LOGGER.logp(java.util.logging.Level.FINER, GetType(ModelMBeanConstructorInfo).name, "toString()", "Entry")
					Dim retStr As String = "ModelMBeanConstructorInfo: " & Me.name & " ; Description: " & Me.description & " ; Descriptor: " & Me.descriptor & " ; Signature: "
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
			''' displayName=this.getName(), name=this.getName(), descriptorType="operation",
			''' role="constructor"
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
					clone.fieldeld("role","constructor")
					MODELMBEAN_LOGGER.finer("Defaulting Descriptor role field to ""constructor""")
				End If

				'Checking validity
				If Not clone.valid Then Throw New javax.management.RuntimeOperationsException(New System.ArgumentException("Invalid Descriptor argument"), "The isValid() method of the Descriptor object itself returned false," & "one or more required fields are invalid. Descriptor:" & clone.ToString())
				If (Not name.ToUpper()) = CStr(clone.getFieldValue("name")).ToUpper() Then Throw New javax.management.RuntimeOperationsException(New System.ArgumentException("Invalid Descriptor argument"), "The Descriptor ""name"" field does not match the object described. " & " Expected: " & Me.name & " , was: " & clone.getFieldValue("name"))
				If Not "operation".equalsIgnoreCase(CStr(clone.getFieldValue("descriptorType"))) Then Throw New javax.management.RuntimeOperationsException(New System.ArgumentException("Invalid Descriptor argument"), "The Descriptor ""descriptorType"" field does not match the object described. " & " Expected: ""operation"" ," & " was: " & clone.getFieldValue("descriptorType"))
				If (Not CStr(clone.getFieldValue("role")).ToUpper()) = "constructor".ToUpper() Then Throw New javax.management.RuntimeOperationsException(New System.ArgumentException("Invalid Descriptor argument"), "The Descriptor ""role"" field does not match the object described. " & " Expected: ""constructor"" ," & " was: " & clone.getFieldValue("role"))

				Return clone
			End Function

		''' <summary>
		''' Deserializes a <seealso cref="ModelMBeanConstructorInfo"/> from an <seealso cref="ObjectInputStream"/>.
		''' </summary>
		Private Sub readObject(ByVal [in] As java.io.ObjectInputStream)
		  ' New serial form ignores extra field "currClass"
		  [in].defaultReadObject()
		End Sub


		''' <summary>
		''' Serializes a <seealso cref="ModelMBeanConstructorInfo"/> to an <seealso cref="ObjectOutputStream"/>.
		''' </summary>
		Private Sub writeObject(ByVal out As java.io.ObjectOutputStream)
		  If compat Then
			' Serializes this instance in the old serial form
			'
			Dim fields As java.io.ObjectOutputStream.PutField = out.putFields()
			fields.put("consDescriptor", consDescriptor)
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