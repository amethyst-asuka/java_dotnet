Imports Microsoft.VisualBasic
Imports System

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
'
' * @author    IBM Corp.
' *
' * Copyright IBM Corp. 1999-2000.  All rights reserved.
' 

Namespace javax.management.modelmbean



	''' <summary>
	''' Exception thrown when an invalid target object type is specified.
	''' 
	''' 
	''' <p>The <b>serialVersionUID</b> of this class is <code>1190536278266811217L</code>.
	''' 
	''' @since 1.5
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class InvalidTargetObjectTypeException
		Inherits Exception ' serialVersionUID not constant

		' Serialization compatibility stuff:
		' Two serial forms are supported in this class. The selected form depends
		' on system property "jmx.serial.form":
		'  - "1.0" for JMX 1.0
		'  - any other value for JMX 1.1 and higher
		'
		' Serial version for old serial form
		Private Const oldSerialVersionUID As Long = 3711724570458346634L
		'
		' Serial version for new serial form
		Private Const newSerialVersionUID As Long = 1190536278266811217L
		'
		' Serializable fields in old serial form
		Private Shared ReadOnly oldSerialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("msgStr", GetType(String)), New java.io.ObjectStreamField("relatedExcept", GetType(Exception)) }
		'
		' Serializable fields in new serial form
		Private Shared ReadOnly newSerialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("exception", GetType(Exception)) }
		'
		' Actual serial version and serial form
		Private Shared ReadOnly serialVersionUID As Long
		''' <summary>
		''' @serialField exception Exception Encapsulated <seealso cref="Exception"/>
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
		''' @serial Encapsulated <seealso cref="Exception"/>
		''' </summary>
		Friend exception As Exception


		''' <summary>
		''' Default constructor.
		''' </summary>
		Public Sub New()
		  MyBase.New("InvalidTargetObjectTypeException: ")
		  exception = Nothing
		End Sub


		''' <summary>
		''' Constructor from a string.
		''' </summary>
		''' <param name="s"> String value that will be incorporated in the message for
		'''    this exception. </param>

		Public Sub New(ByVal s As String)
		  MyBase.New("InvalidTargetObjectTypeException: " & s)
		  exception = Nothing
		End Sub


		''' <summary>
		''' Constructor taking an exception and a string.
		''' </summary>
		''' <param name="e"> Exception that we may have caught to reissue as an
		'''    InvalidTargetObjectTypeException.  The message will be used, and we may want to
		'''    consider overriding the printStackTrace() methods to get data
		'''    pointing back to original throw stack. </param>
		''' <param name="s"> String value that will be incorporated in message for
		'''    this exception. </param>

		Public Sub New(ByVal e As Exception, ByVal s As String)
		  MyBase.New("InvalidTargetObjectTypeException: " & s + (If(e IsNot Nothing, (vbLf & vbTab & " triggered by:" & e.ToString()), "")))
		  exception = e
		End Sub

		''' <summary>
		''' Deserializes an <seealso cref="InvalidTargetObjectTypeException"/> from an <seealso cref="ObjectInputStream"/>.
		''' </summary>
		Private Sub readObject(ByVal [in] As java.io.ObjectInputStream)
		  If compat Then
			' Read an object serialized in the old serial form
			'
			Dim fields As java.io.ObjectInputStream.GetField = [in].readFields()
			exception = CType(fields.get("relatedExcept", Nothing), Exception)
			If fields.defaulted("relatedExcept") Then Throw New NullPointerException("relatedExcept")
		  Else
			' Read an object serialized in the new serial form
			'
			[in].defaultReadObject()
		  End If
		End Sub


		''' <summary>
		''' Serializes an <seealso cref="InvalidTargetObjectTypeException"/> to an <seealso cref="ObjectOutputStream"/>.
		''' </summary>
		Private Sub writeObject(ByVal out As java.io.ObjectOutputStream)
		  If compat Then
			' Serializes this instance in the old serial form
			'
			Dim fields As java.io.ObjectOutputStream.PutField = out.putFields()
			fields.put("relatedExcept", exception)
			fields.put("msgStr", (If(exception IsNot Nothing, exception.Message, "")))
			out.writeFields()
		  Else
			' Serializes this instance in the new serial form
			'
			out.defaultWriteObject()
		  End If
		End Sub
	End Class

End Namespace