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
	''' This exception is thrown when an XML formatted string is being parsed into ModelMBean objects
	''' or when XML formatted strings are being created from ModelMBean objects.
	''' 
	''' It is also used to wrapper exceptions from XML parsers that may be used.
	''' 
	''' <p>The <b>serialVersionUID</b> of this class is <code>3176664577895105181L</code>.
	''' 
	''' @since 1.5
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class XMLParseException
		Inherits Exception
 ' serialVersionUID not constant
		' Serialization compatibility stuff:
		' Two serial forms are supported in this class. The selected form depends
		' on system property "jmx.serial.form":
		'  - "1.0" for JMX 1.0
		'  - any other value for JMX 1.1 and higher
		'
		' Serial version for old serial form
		Private Const oldSerialVersionUID As Long = -7780049316655891976L
		'
		' Serial version for new serial form
		Private Const newSerialVersionUID As Long = 3176664577895105181L
		'
		' Serializable fields in old serial form
		Private Shared ReadOnly oldSerialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("msgStr", GetType(String)) }
		'
		' Serializable fields in new serial form
	  Private Shared ReadOnly newSerialPersistentFields As java.io.ObjectStreamField() = { }
		'
		' Actual serial version and serial form
		Private Shared ReadOnly serialVersionUID As Long
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
		''' Default constructor .
		''' </summary>
		Public Sub New()
		  MyBase.New("XML Parse Exception.")
		End Sub

		''' <summary>
		''' Constructor taking a string.
		''' </summary>
		''' <param name="s"> the detail message. </param>
		Public Sub New(ByVal s As String)
		  MyBase.New("XML Parse Exception: " & s)
		End Sub
		''' <summary>
		''' Constructor taking a string and an exception.
		''' </summary>
		''' <param name="e"> the nested exception. </param>
		''' <param name="s"> the detail message. </param>
		Public Sub New(ByVal e As Exception, ByVal s As String)
		  MyBase.New("XML Parse Exception: " & s & ":" & e.ToString())
		End Sub

		''' <summary>
		''' Deserializes an <seealso cref="XMLParseException"/> from an <seealso cref="ObjectInputStream"/>.
		''' </summary>
		Private Sub readObject(ByVal [in] As java.io.ObjectInputStream)
		  ' New serial form ignores extra field "msgStr"
		  [in].defaultReadObject()
		End Sub


		''' <summary>
		''' Serializes an <seealso cref="XMLParseException"/> to an <seealso cref="ObjectOutputStream"/>.
		''' </summary>
		Private Sub writeObject(ByVal out As java.io.ObjectOutputStream)
		  If compat Then
			' Serializes this instance in the old serial form
			'
			Dim fields As java.io.ObjectOutputStream.PutField = out.putFields()
			fields.put("msgStr", message)
			out.writeFields()
		  Else
			' Serializes this instance in the new serial form
			'
			out.defaultWriteObject()
		  End If
		End Sub
	End Class

End Namespace