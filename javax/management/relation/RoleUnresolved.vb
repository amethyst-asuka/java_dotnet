Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Text
import static com.sun.jmx.mbeanserver.Util.cast

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
	''' Represents an unresolved role: a role not retrieved from a relation due
	''' to a problem. It provides the role name, value (if problem when trying to
	''' set the role) and an integer defining the problem (constants defined in
	''' RoleStatus).
	''' 
	''' <p>The <b>serialVersionUID</b> of this class is <code>-48350262537070138L</code>.
	''' 
	''' @since 1.5
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<Serializable> _
	Public Class RoleUnresolved ' serialVersionUID not constant

		' Serialization compatibility stuff:
		' Two serial forms are supported in this class. The selected form depends
		' on system property "jmx.serial.form":
		'  - "1.0" for JMX 1.0
		'  - any other value for JMX 1.1 and higher
		'
		' Serial version for old serial form
		Private Const oldSerialVersionUID As Long = -9026457686611660144L
		'
		' Serial version for new serial form
		Private Const newSerialVersionUID As Long = -48350262537070138L
		'
		' Serializable fields in old serial form
		Private Shared ReadOnly oldSerialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("myRoleName", GetType(String)), New java.io.ObjectStreamField("myRoleValue", GetType(ArrayList)), New java.io.ObjectStreamField("myPbType", GetType(Integer)) }
		'
		' Serializable fields in new serial form
		Private Shared ReadOnly newSerialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("roleName", GetType(String)), New java.io.ObjectStreamField("roleValue", GetType(IList)), New java.io.ObjectStreamField("problemType", GetType(Integer)) }
		'
		' Actual serial version and serial form
		Private Shared ReadOnly serialVersionUID As Long
		''' <summary>
		''' @serialField roleName String Role name
		'''  @serialField roleValue List Role value (<seealso cref="List"/> of <seealso cref="ObjectName"/> objects)
		'''  @serialField problemType int Problem type
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
		''' @serial Role name
		''' </summary>
		Private roleName As String = Nothing

		''' <summary>
		''' @serial Role value (<seealso cref="List"/> of <seealso cref="ObjectName"/> objects)
		''' </summary>
		Private roleValue As IList(Of javax.management.ObjectName) = Nothing

		''' <summary>
		''' @serial Problem type
		''' </summary>
		Private problemType As Integer

		'
		' Constructor
		'

		''' <summary>
		''' Constructor.
		''' </summary>
		''' <param name="name">  name of the role </param>
		''' <param name="value">  value of the role (if problem when setting the
		''' role) </param>
		''' <param name="pbType">  type of problem (according to known problem types,
		''' listed as static final members).
		''' </param>
		''' <exception cref="IllegalArgumentException">  if null parameter or incorrect
		''' problem type </exception>
		Public Sub New(ByVal name As String, ByVal value As IList(Of javax.management.ObjectName), ByVal pbType As Integer)

			If name Is Nothing Then
				Dim excMsg As String = "Invalid parameter."
				Throw New System.ArgumentException(excMsg)
			End If

			roleName = name
			roleValue = value
			' Can throw IllegalArgumentException
			problemType = pbType
			Return
		End Sub

		'
		' Accessors
		'

		''' <summary>
		''' Retrieves role name.
		''' </summary>
		''' <returns> the role name.
		''' </returns>
		''' <seealso cref= #setRoleName </seealso>
		Public Overridable Property roleName As String
			Get
				Return roleName
			End Get
			Set(ByVal name As String)
    
				If name Is Nothing Then
					Dim excMsg As String = "Invalid parameter."
					Throw New System.ArgumentException(excMsg)
				End If
    
				roleName = name
				Return
			End Set
		End Property

		''' <summary>
		''' Retrieves role value.
		''' </summary>
		''' <returns> an ArrayList of ObjectName objects, the one provided to be set
		''' in given role. Null if the unresolved role is returned for a read
		''' access.
		''' </returns>
		''' <seealso cref= #setRoleValue </seealso>
		Public Overridable Property roleValue As IList(Of javax.management.ObjectName)
			Get
				Return roleValue
			End Get
			Set(ByVal value As IList(Of javax.management.ObjectName))
    
				If value IsNot Nothing Then
					roleValue = New List(Of javax.management.ObjectName)(value)
				Else
					roleValue = Nothing
				End If
				Return
			End Set
		End Property

		''' <summary>
		''' Retrieves problem type.
		''' </summary>
		''' <returns> an integer corresponding to a problem, those being described as
		''' static final members of current class.
		''' </returns>
		''' <seealso cref= #setProblemType </seealso>
		Public Overridable Property problemType As Integer
			Get
				Return problemType
			End Get
			Set(ByVal pbType As Integer)
    
				If Not(RoleStatus.isRoleStatus(pbType)) Then
					Dim excMsg As String = "Incorrect problem type."
					Throw New System.ArgumentException(excMsg)
				End If
				problemType = pbType
				Return
			End Set
		End Property




		''' <summary>
		''' Clone this object.
		''' </summary>
		''' <returns> an independent clone. </returns>
		Public Overridable Function clone() As Object
			Try
				Return New RoleUnresolved(roleName, roleValue, problemType)
			Catch exc As System.ArgumentException
				Return Nothing ' :)
			End Try
		End Function

		''' <summary>
		''' Return a string describing this object.
		''' </summary>
		''' <returns> a description of this RoleUnresolved object. </returns>
		Public Overrides Function ToString() As String
			Dim result As New StringBuilder
			result.Append("role name: " & roleName)
			If roleValue IsNot Nothing Then
				result.Append("; value: ")
				Dim objNameIter As IEnumerator(Of javax.management.ObjectName) = roleValue.GetEnumerator()
				Do While objNameIter.MoveNext()
					Dim currObjName As javax.management.ObjectName = objNameIter.Current
					result.Append(currObjName.ToString())
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					If objNameIter.hasNext() Then result.Append(", ")
				Loop
			End If
			result.Append("; problem type: " & problemType)
			Return result.ToString()
		End Function

		''' <summary>
		''' Deserializes a <seealso cref="RoleUnresolved"/> from an <seealso cref="ObjectInputStream"/>.
		''' </summary>
		Private Sub readObject(ByVal [in] As java.io.ObjectInputStream)
		  If compat Then
			' Read an object serialized in the old serial form
			'
			Dim fields As java.io.ObjectInputStream.GetField = [in].readFields()
			roleName = CStr(fields.get("myRoleName", Nothing))
			If fields.defaulted("myRoleName") Then Throw New NullPointerException("myRoleName")
			roleValue = cast(fields.get("myRoleValue", Nothing))
			If fields.defaulted("myRoleValue") Then Throw New NullPointerException("myRoleValue")
			problemType = fields.get("myPbType", 0)
			If fields.defaulted("myPbType") Then Throw New NullPointerException("myPbType")
		  Else
			' Read an object serialized in the new serial form
			'
			[in].defaultReadObject()
		  End If
		End Sub


		''' <summary>
		''' Serializes a <seealso cref="RoleUnresolved"/> to an <seealso cref="ObjectOutputStream"/>.
		''' </summary>
		Private Sub writeObject(ByVal out As java.io.ObjectOutputStream)
		  If compat Then
			' Serializes this instance in the old serial form
			'
			Dim fields As java.io.ObjectOutputStream.PutField = out.putFields()
			fields.put("myRoleName", roleName)
			fields.put("myRoleValue", roleValue)
			fields.put("myPbType", problemType)
			out.writeFields()
		  Else
			' Serializes this instance in the new serial form
			'
			out.defaultWriteObject()
		  End If
		End Sub
	End Class

End Namespace