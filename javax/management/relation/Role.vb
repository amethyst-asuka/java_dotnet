Imports Microsoft.VisualBasic
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
	''' Represents a role: includes a role name and referenced MBeans (via their
	''' ObjectNames). The role value is always represented as an ArrayList
	''' collection (of ObjectNames) to homogenize the access.
	''' 
	''' <p>The <b>serialVersionUID</b> of this class is <code>-279985518429862552L</code>.
	''' 
	''' @since 1.5
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<Serializable> _
	Public Class Role ' serialVersionUID not constant

		' Serialization compatibility stuff:
		' Two serial forms are supported in this class. The selected form depends
		' on system property "jmx.serial.form":
		'  - "1.0" for JMX 1.0
		'  - any other value for JMX 1.1 and higher
		'
		' Serial version for old serial form
		Private Const oldSerialVersionUID As Long = -1959486389343113026L
		'
		' Serial version for new serial form
		Private Const newSerialVersionUID As Long = -279985518429862552L
		'
		' Serializable fields in old serial form
		Private Shared ReadOnly oldSerialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("myName", GetType(String)), New java.io.ObjectStreamField("myObjNameList", GetType(ArrayList)) }
		'
		' Serializable fields in new serial form
		Private Shared ReadOnly newSerialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("name", GetType(String)), New java.io.ObjectStreamField("objectNameList", GetType(IList)) }
		'
		' Actual serial version and serial form
		Private Shared ReadOnly serialVersionUID As Long
		''' <summary>
		''' @serialField name String Role name
		''' @serialField objectNameList List <seealso cref="List"/> of <seealso cref="ObjectName"/>s of referenced MBeans
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
		Private name As String = Nothing

		''' <summary>
		''' @serial <seealso cref="List"/> of <seealso cref="ObjectName"/>s of referenced MBeans
		''' </summary>
		Private objectNameList As IList(Of javax.management.ObjectName) = New List(Of javax.management.ObjectName)

		'
		' Constructors
		'

		''' <summary>
		''' <p>Make a new Role object.
		''' No check is made that the ObjectNames in the role value exist in
		''' an MBean server.  That check will be made when the role is set
		''' in a relation.
		''' </summary>
		''' <param name="roleName">  role name </param>
		''' <param name="roleValue">  role value (List of ObjectName objects)
		''' </param>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		Public Sub New(ByVal roleName As String, ByVal roleValue As IList(Of javax.management.ObjectName))

			If roleName Is Nothing OrElse roleValue Is Nothing Then
				Dim excMsg As String = "Invalid parameter"
				Throw New System.ArgumentException(excMsg)
			End If

			roleName = roleName
			roleValue = roleValue

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
				Return name
			End Get
			Set(ByVal roleName As String)
    
				If roleName Is Nothing Then
					Dim excMsg As String = "Invalid parameter."
					Throw New System.ArgumentException(excMsg)
				End If
    
				name = roleName
				Return
			End Set
		End Property

		''' <summary>
		''' Retrieves role value.
		''' </summary>
		''' <returns> ArrayList of ObjectName objects for referenced MBeans.
		''' </returns>
		''' <seealso cref= #setRoleValue </seealso>
		Public Overridable Property roleValue As IList(Of javax.management.ObjectName)
			Get
				Return objectNameList
			End Get
			Set(ByVal roleValue As IList(Of javax.management.ObjectName))
    
				If roleValue Is Nothing Then
					Dim excMsg As String = "Invalid parameter."
					Throw New System.ArgumentException(excMsg)
				End If
    
				objectNameList = New List(Of javax.management.ObjectName)(roleValue)
				Return
			End Set
		End Property



		''' <summary>
		''' Returns a string describing the role.
		''' </summary>
		''' <returns> the description of the role. </returns>
		Public Overrides Function ToString() As String
			Dim result As New StringBuilder
			result.Append("role name: " & name & "; role value: ")
			Dim objNameIter As IEnumerator(Of javax.management.ObjectName) = objectNameList.GetEnumerator()
			Do While objNameIter.MoveNext()
				Dim currObjName As javax.management.ObjectName = objNameIter.Current
				result.Append(currObjName.ToString())
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				If objNameIter.hasNext() Then result.Append(", ")
			Loop
			Return result.ToString()
		End Function

		'
		' Misc
		'

		''' <summary>
		''' Clone the role object.
		''' </summary>
		''' <returns> a Role that is an independent copy of the current Role object. </returns>
		Public Overridable Function clone() As Object

			Try
				Return New Role(name, objectNameList)
			Catch exc As System.ArgumentException
				Return Nothing ' can't happen
			End Try
		End Function

		''' <summary>
		''' Returns a string for the given role value.
		''' </summary>
		''' <param name="roleValue">  List of ObjectName objects
		''' </param>
		''' <returns> A String consisting of the ObjectNames separated by
		''' newlines (\n).
		''' </returns>
		''' <exception cref="IllegalArgumentException">  if null parameter </exception>
		Public Shared Function roleValueToString(ByVal roleValue As IList(Of javax.management.ObjectName)) As String

			If roleValue Is Nothing Then
				Dim excMsg As String = "Invalid parameter"
				Throw New System.ArgumentException(excMsg)
			End If

			Dim result As New StringBuilder
			For Each currObjName As javax.management.ObjectName In roleValue
				If result.Length > 0 Then result.Append(vbLf)
				result.Append(currObjName.ToString())
			Next currObjName
			Return result.ToString()
		End Function

		''' <summary>
		''' Deserializes a <seealso cref="Role"/> from an <seealso cref="ObjectInputStream"/>.
		''' </summary>
		Private Sub readObject(ByVal [in] As java.io.ObjectInputStream)
		  If compat Then
			' Read an object serialized in the old serial form
			'
			Dim fields As java.io.ObjectInputStream.GetField = [in].readFields()
			name = CStr(fields.get("myName", Nothing))
			If fields.defaulted("myName") Then Throw New NullPointerException("myName")
			objectNameList = cast(fields.get("myObjNameList", Nothing))
			If fields.defaulted("myObjNameList") Then Throw New NullPointerException("myObjNameList")
		  Else
			' Read an object serialized in the new serial form
			'
			[in].defaultReadObject()
		  End If
		End Sub


		''' <summary>
		''' Serializes a <seealso cref="Role"/> to an <seealso cref="ObjectOutputStream"/>.
		''' </summary>
		Private Sub writeObject(ByVal out As java.io.ObjectOutputStream)
		  If compat Then
			' Serializes this instance in the old serial form
			'
			Dim fields As java.io.ObjectOutputStream.PutField = out.putFields()
			fields.put("myName", name)
			fields.put("myObjNameList", objectNameList)
			out.writeFields()
		  Else
			' Serializes this instance in the new serial form
			'
			out.defaultWriteObject()
		  End If
		End Sub
	End Class

End Namespace