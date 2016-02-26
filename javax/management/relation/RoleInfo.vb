Imports System
Imports System.Text

'
' * Copyright (c) 2000, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' A RoleInfo object summarises a role in a relation type.
	''' 
	''' <p>The <b>serialVersionUID</b> of this class is <code>2504952983494636987L</code>.
	''' 
	''' @since 1.5
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<Serializable> _
	Public Class RoleInfo ' serialVersionUID not constant

		' Serialization compatibility stuff:
		' Two serial forms are supported in this class. The selected form depends
		' on system property "jmx.serial.form":
		'  - "1.0" for JMX 1.0
		'  - any other value for JMX 1.1 and higher
		'
		' Serial version for old serial form
		Private Const oldSerialVersionUID As Long = 7227256952085334351L
		'
		' Serial version for new serial form
		Private Const newSerialVersionUID As Long = 2504952983494636987L
		'
		' Serializable fields in old serial form
		Private Shared ReadOnly oldSerialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("myName", GetType(String)), New java.io.ObjectStreamField("myIsReadableFlg", GetType(Boolean)), New java.io.ObjectStreamField("myIsWritableFlg", GetType(Boolean)), New java.io.ObjectStreamField("myDescription", GetType(String)), New java.io.ObjectStreamField("myMinDegree", GetType(Integer)), New java.io.ObjectStreamField("myMaxDegree", GetType(Integer)), New java.io.ObjectStreamField("myRefMBeanClassName", GetType(String)) }
		'
		' Serializable fields in new serial form
		Private Shared ReadOnly newSerialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("name", GetType(String)), New java.io.ObjectStreamField("isReadable", GetType(Boolean)), New java.io.ObjectStreamField("isWritable", GetType(Boolean)), New java.io.ObjectStreamField("description", GetType(String)), New java.io.ObjectStreamField("minDegree", GetType(Integer)), New java.io.ObjectStreamField("maxDegree", GetType(Integer)), New java.io.ObjectStreamField("referencedMBeanClassName", GetType(String)) }
		'
		' Actual serial version and serial form
		Private Shared ReadOnly serialVersionUID As Long
		''' <summary>
		''' @serialField name String Role name
		''' @serialField isReadable boolean Read access mode: <code>true</code> if role is readable
		''' @serialField isWritable boolean Write access mode: <code>true</code> if role is writable
		''' @serialField description String Role description
		''' @serialField minDegree int Minimum degree (i.e. minimum number of referenced MBeans in corresponding role)
		''' @serialField maxDegree int Maximum degree (i.e. maximum number of referenced MBeans in corresponding role)
		''' @serialField referencedMBeanClassName String Name of class of MBean(s) expected to be referenced in corresponding role
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
		' Public constants
		'

		''' <summary>
		''' To specify an unlimited cardinality.
		''' </summary>
		Public Const ROLE_CARDINALITY_INFINITY As Integer = -1

		'
		' Private members
		'

		''' <summary>
		''' @serial Role name
		''' </summary>
		Private name As String = Nothing

		''' <summary>
		''' @serial Read access mode: <code>true</code> if role is readable
		''' </summary>
		Private ___isReadable As Boolean

		''' <summary>
		''' @serial Write access mode: <code>true</code> if role is writable
		''' </summary>
		Private ___isWritable As Boolean

		''' <summary>
		''' @serial Role description
		''' </summary>
		Private description As String = Nothing

		''' <summary>
		''' @serial Minimum degree (i.e. minimum number of referenced MBeans in corresponding role)
		''' </summary>
		Private minDegree As Integer

		''' <summary>
		''' @serial Maximum degree (i.e. maximum number of referenced MBeans in corresponding role)
		''' </summary>
		Private maxDegree As Integer

		''' <summary>
		''' @serial Name of class of MBean(s) expected to be referenced in corresponding role
		''' </summary>
		Private referencedMBeanClassName As String = Nothing

		'
		' Constructors
		'

		''' <summary>
		''' Constructor.
		''' </summary>
		''' <param name="roleName">  name of the role. </param>
		''' <param name="mbeanClassName">  name of the class of MBean(s) expected to
		''' be referenced in corresponding role.  If an MBean <em>M</em> is in
		''' this role, then the MBean server must return true for
		''' <seealso cref="MBeanServer#isInstanceOf isInstanceOf(M, mbeanClassName)"/>. </param>
		''' <param name="read">  flag to indicate if the corresponding role
		''' can be read </param>
		''' <param name="write">  flag to indicate if the corresponding role
		''' can be set </param>
		''' <param name="min">  minimum degree for role, i.e. minimum number of
		''' MBeans to provide in corresponding role
		''' Must be less than or equal to <tt>max</tt>.
		''' (ROLE_CARDINALITY_INFINITY for unlimited) </param>
		''' <param name="max">  maximum degree for role, i.e. maximum number of
		''' MBeans to provide in corresponding role
		''' Must be greater than or equal to <tt>min</tt>
		''' (ROLE_CARDINALITY_INFINITY for unlimited) </param>
		''' <param name="descr">  description of the role (can be null)
		''' </param>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		''' <exception cref="InvalidRoleInfoException">  if the minimum degree is
		''' greater than the maximum degree. </exception>
		''' <exception cref="ClassNotFoundException"> As of JMX 1.2, this exception
		''' can no longer be thrown.  It is retained in the declaration of
		''' this class for compatibility with existing code. </exception>
		''' <exception cref="NotCompliantMBeanException">  if the class mbeanClassName
		''' is not a MBean class. </exception>
		Public Sub New(ByVal roleName As String, ByVal mbeanClassName As String, ByVal read As Boolean, ByVal write As Boolean, ByVal min As Integer, ByVal max As Integer, ByVal descr As String)

			init(roleName, mbeanClassName, read, write, min, max, descr)
			Return
		End Sub

		''' <summary>
		''' Constructor.
		''' </summary>
		''' <param name="roleName">  name of the role </param>
		''' <param name="mbeanClassName">  name of the class of MBean(s) expected to
		''' be referenced in corresponding role.  If an MBean <em>M</em> is in
		''' this role, then the MBean server must return true for
		''' <seealso cref="MBeanServer#isInstanceOf isInstanceOf(M, mbeanClassName)"/>. </param>
		''' <param name="read">  flag to indicate if the corresponding role
		''' can be read </param>
		''' <param name="write">  flag to indicate if the corresponding role
		''' can be set
		''' 
		''' <P>Minimum and maximum degrees defaulted to 1.
		''' <P>Description of role defaulted to null.
		''' </param>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		''' <exception cref="ClassNotFoundException"> As of JMX 1.2, this exception
		''' can no longer be thrown.  It is retained in the declaration of
		''' this class for compatibility with existing code. </exception>
		''' <exception cref="NotCompliantMBeanException"> As of JMX 1.2, this
		''' exception can no longer be thrown.  It is retained in the
		''' declaration of this class for compatibility with existing code. </exception>
		Public Sub New(ByVal roleName As String, ByVal mbeanClassName As String, ByVal read As Boolean, ByVal write As Boolean)

			Try
				init(roleName, mbeanClassName, read, write, 1, 1, Nothing)
			Catch exc As InvalidRoleInfoException
				' OK : Can never happen as the minimum
				'      degree equals the maximum degree.
			End Try

			Return
		End Sub

		''' <summary>
		''' Constructor.
		''' </summary>
		''' <param name="roleName">  name of the role </param>
		''' <param name="mbeanClassName">  name of the class of MBean(s) expected to
		''' be referenced in corresponding role.  If an MBean <em>M</em> is in
		''' this role, then the MBean server must return true for
		''' <seealso cref="MBeanServer#isInstanceOf isInstanceOf(M, mbeanClassName)"/>.
		''' 
		''' <P>IsReadable and IsWritable defaulted to true.
		''' <P>Minimum and maximum degrees defaulted to 1.
		''' <P>Description of role defaulted to null.
		''' </param>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		''' <exception cref="ClassNotFoundException"> As of JMX 1.2, this exception
		''' can no longer be thrown.  It is retained in the declaration of
		''' this class for compatibility with existing code. </exception>
		''' <exception cref="NotCompliantMBeanException"> As of JMX 1.2, this
		''' exception can no longer be thrown.  It is retained in the
		''' declaration of this class for compatibility with existing code. </exception>
		Public Sub New(ByVal roleName As String, ByVal mbeanClassName As String)

			Try
				init(roleName, mbeanClassName, True, True, 1, 1, Nothing)
			Catch exc As InvalidRoleInfoException
				' OK : Can never happen as the minimum
				'      degree equals the maximum degree.
			End Try

			Return
		End Sub

		''' <summary>
		''' Copy constructor.
		''' </summary>
		''' <param name="roleInfo"> the <tt>RoleInfo</tt> instance to be copied.
		''' </param>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		Public Sub New(ByVal ___roleInfo As RoleInfo)

			If ___roleInfo Is Nothing Then
				' Revisit [cebro] Localize message
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			Try
				init(___roleInfo.name, ___roleInfo.refMBeanClassName, ___roleInfo.readable, ___roleInfo.writable, ___roleInfo.minDegree, ___roleInfo.maxDegree, ___roleInfo.description)
			Catch exc3 As InvalidRoleInfoException
				' OK : Can never happen as the minimum degree and the maximum
				'      degree were already checked at the time the roleInfo
				'      instance was created.
			End Try
		End Sub

		'
		' Accessors
		'

		''' <summary>
		''' Returns the name of the role.
		''' </summary>
		''' <returns> the name of the role. </returns>
		Public Overridable Property name As String
			Get
				Return name
			End Get
		End Property

		''' <summary>
		''' Returns read access mode for the role (true if it is readable).
		''' </summary>
		''' <returns> true if the role is readable. </returns>
		Public Overridable Property readable As Boolean
			Get
				Return ___isReadable
			End Get
		End Property

		''' <summary>
		''' Returns write access mode for the role (true if it is writable).
		''' </summary>
		''' <returns> true if the role is writable. </returns>
		Public Overridable Property writable As Boolean
			Get
				Return ___isWritable
			End Get
		End Property

		''' <summary>
		''' Returns description text for the role.
		''' </summary>
		''' <returns> the description of the role. </returns>
		Public Overridable Property description As String
			Get
				Return description
			End Get
		End Property

		''' <summary>
		''' Returns minimum degree for corresponding role reference.
		''' </summary>
		''' <returns> the minimum degree. </returns>
		Public Overridable Property minDegree As Integer
			Get
				Return minDegree
			End Get
		End Property

		''' <summary>
		''' Returns maximum degree for corresponding role reference.
		''' </summary>
		''' <returns> the maximum degree. </returns>
		Public Overridable Property maxDegree As Integer
			Get
				Return maxDegree
			End Get
		End Property

		''' <summary>
		''' <p>Returns name of type of MBean expected to be referenced in
		''' corresponding role.</p>
		''' </summary>
		''' <returns> the name of the referenced type. </returns>
		Public Overridable Property refMBeanClassName As String
			Get
				Return referencedMBeanClassName
			End Get
		End Property

		''' <summary>
		''' Returns true if the <tt>value</tt> parameter is greater than or equal to
		''' the expected minimum degree, false otherwise.
		''' </summary>
		''' <param name="value">  the value to be checked
		''' </param>
		''' <returns> true if greater than or equal to minimum degree, false otherwise. </returns>
		Public Overridable Function checkMinDegree(ByVal value As Integer) As Boolean
			If value >= ROLE_CARDINALITY_INFINITY AndAlso (minDegree = ROLE_CARDINALITY_INFINITY OrElse value >= minDegree) Then
				Return True
			Else
				Return False
			End If
		End Function

		''' <summary>
		''' Returns true if the <tt>value</tt> parameter is lower than or equal to
		''' the expected maximum degree, false otherwise.
		''' </summary>
		''' <param name="value">  the value to be checked
		''' </param>
		''' <returns> true if lower than or equal to maximum degree, false otherwise. </returns>
		Public Overridable Function checkMaxDegree(ByVal value As Integer) As Boolean
			If value >= ROLE_CARDINALITY_INFINITY AndAlso (maxDegree = ROLE_CARDINALITY_INFINITY OrElse (value <> ROLE_CARDINALITY_INFINITY AndAlso value <= maxDegree)) Then
				Return True
			Else
				Return False
			End If
		End Function

		''' <summary>
		''' Returns a string describing the role info.
		''' </summary>
		''' <returns> a description of the role info. </returns>
		Public Overrides Function ToString() As String
			Dim result As New StringBuilder
			result.Append("role info name: " & name)
			result.Append("; isReadable: " & ___isReadable)
			result.Append("; isWritable: " & ___isWritable)
			result.Append("; description: " & description)
			result.Append("; minimum degree: " & minDegree)
			result.Append("; maximum degree: " & maxDegree)
			result.Append("; MBean class: " & referencedMBeanClassName)
			Return result.ToString()
		End Function

		'
		' Misc
		'

		' Initialization
		Private Sub init(ByVal roleName As String, ByVal mbeanClassName As String, ByVal read As Boolean, ByVal write As Boolean, ByVal min As Integer, ByVal max As Integer, ByVal descr As String)

			If roleName Is Nothing OrElse mbeanClassName Is Nothing Then
				' Revisit [cebro] Localize message
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			name = roleName
			___isReadable = read
			___isWritable = write
			If descr IsNot Nothing Then description = descr

			Dim invalidRoleInfoFlg As Boolean = False
			Dim excMsgStrB As New StringBuilder
			If max <> ROLE_CARDINALITY_INFINITY AndAlso (min = ROLE_CARDINALITY_INFINITY OrElse min > max) Then
				' Revisit [cebro] Localize message
				excMsgStrB.Append("Minimum degree ")
				excMsgStrB.Append(min)
				excMsgStrB.Append(" is greater than maximum degree ")
				excMsgStrB.Append(max)
				invalidRoleInfoFlg = True

			ElseIf min < ROLE_CARDINALITY_INFINITY OrElse max < ROLE_CARDINALITY_INFINITY Then
				' Revisit [cebro] Localize message
				excMsgStrB.Append("Minimum or maximum degree has an illegal value, must be [0, ROLE_CARDINALITY_INFINITY].")
				invalidRoleInfoFlg = True
			End If
			If invalidRoleInfoFlg Then Throw New InvalidRoleInfoException(excMsgStrB.ToString())
			minDegree = min
			maxDegree = max

			referencedMBeanClassName = mbeanClassName

			Return
		End Sub

		''' <summary>
		''' Deserializes a <seealso cref="RoleInfo"/> from an <seealso cref="ObjectInputStream"/>.
		''' </summary>
		Private Sub readObject(ByVal [in] As java.io.ObjectInputStream)
		  If compat Then
			' Read an object serialized in the old serial form
			'
			Dim fields As java.io.ObjectInputStream.GetField = [in].readFields()
			name = CStr(fields.get("myName", Nothing))
			If fields.defaulted("myName") Then Throw New NullPointerException("myName")
			___isReadable = fields.get("myIsReadableFlg", False)
			If fields.defaulted("myIsReadableFlg") Then Throw New NullPointerException("myIsReadableFlg")
			___isWritable = fields.get("myIsWritableFlg", False)
			If fields.defaulted("myIsWritableFlg") Then Throw New NullPointerException("myIsWritableFlg")
			description = CStr(fields.get("myDescription", Nothing))
			If fields.defaulted("myDescription") Then Throw New NullPointerException("myDescription")
			minDegree = fields.get("myMinDegree", 0)
			If fields.defaulted("myMinDegree") Then Throw New NullPointerException("myMinDegree")
			maxDegree = fields.get("myMaxDegree", 0)
			If fields.defaulted("myMaxDegree") Then Throw New NullPointerException("myMaxDegree")
			referencedMBeanClassName = CStr(fields.get("myRefMBeanClassName", Nothing))
			If fields.defaulted("myRefMBeanClassName") Then Throw New NullPointerException("myRefMBeanClassName")
		  Else
			' Read an object serialized in the new serial form
			'
			[in].defaultReadObject()
		  End If
		End Sub


		''' <summary>
		''' Serializes a <seealso cref="RoleInfo"/> to an <seealso cref="ObjectOutputStream"/>.
		''' </summary>
		Private Sub writeObject(ByVal out As java.io.ObjectOutputStream)
		  If compat Then
			' Serializes this instance in the old serial form
			'
			Dim fields As java.io.ObjectOutputStream.PutField = out.putFields()
			fields.put("myName", name)
			fields.put("myIsReadableFlg", ___isReadable)
			fields.put("myIsWritableFlg", ___isWritable)
			fields.put("myDescription", description)
			fields.put("myMinDegree", minDegree)
			fields.put("myMaxDegree", maxDegree)
			fields.put("myRefMBeanClassName", referencedMBeanClassName)
			out.writeFields()
		  Else
			' Serializes this instance in the new serial form
			'
			out.defaultWriteObject()
		  End If
		End Sub

	End Class

End Namespace