Imports Microsoft.VisualBasic
Imports System

'
' * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.management





	''' <summary>
	''' This class represents numbers that are arguments to relational constraints.
	''' A NumericValueExp may be used anywhere a ValueExp is required.
	''' 
	''' <p>The <b>serialVersionUID</b> of this class is <code>-4679739485102359104L</code>.
	''' 
	''' @serial include
	''' 
	''' @since 1.5
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Friend Class NumericValueExp
		Inherits QueryEval
		Implements ValueExp ' serialVersionUID not constant

		' Serialization compatibility stuff:
		' Two serial forms are supported in this class. The selected form depends
		' on system property "jmx.serial.form":
		'  - "1.0" for JMX 1.0
		'  - any other value for JMX 1.1 and higher
		'
		' Serial version for old serial form
		Private Const oldSerialVersionUID As Long = -6227876276058904000L
		'
		' Serial version for new serial form
		Private Const newSerialVersionUID As Long = -4679739485102359104L
		'
		' Serializable fields in old serial form
		Private Shared ReadOnly oldSerialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("longVal", Long.TYPE), New java.io.ObjectStreamField("doubleVal", Double.TYPE), New java.io.ObjectStreamField("valIsLong", Boolean.TYPE) }
		'
		' Serializable fields in new serial form
		Private Shared ReadOnly newSerialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("val", GetType(Number)) }
		'
		' Actual serial version and serial form
		Private Shared ReadOnly serialVersionUID As Long

		''' <summary>
		''' @serialField val Number The numeric value
		''' 
		''' <p>The <b>serialVersionUID</b> of this class is <code>-4679739485102359104L</code>.
		''' </summary>
		Private Shared ReadOnly serialPersistentFields As java.io.ObjectStreamField()
		Private val As Number = 0.0

		Private Shared compat As Boolean = False
		Shared Sub New()
			Try
				Dim act As New com.sun.jmx.mbeanserver.GetPropertyAction("jmx.serial.form")
				Dim form As String = java.security.AccessController.doPrivileged(act)
				compat = (form IsNot Nothing AndAlso form.Equals("1.0"))
			Catch e As Exception
				' OK: exception means no compat with 1.0, too bad
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
		''' Basic constructor.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Creates a new NumericValue representing the numeric literal @{code val}. </summary>
		Friend Sub New(ByVal val As Number)
		  Me.val = val
		End Sub

		''' <summary>
		''' Returns a double numeric value
		''' </summary>
		Public Overridable Function doubleValue() As Double
		  If TypeOf val Is Long? OrElse TypeOf val Is Integer? Then Return CDbl(val)
		  Return val
		End Function

		''' <summary>
		''' Returns a long numeric value
		''' </summary>
		Public Overridable Function longValue() As Long
		  If TypeOf val Is Long? OrElse TypeOf val Is Integer? Then Return val
		  Return CLng(Fix(val))
		End Function

		''' <summary>
		''' Returns true is if the numeric value is a long, false otherwise.
		''' </summary>
		Public Overridable Property [long] As Boolean
			Get
				Return (TypeOf val Is Long? OrElse TypeOf val Is Integer?)
			End Get
		End Property

		''' <summary>
		''' Returns the string representing the object
		''' </summary>
		Public Overrides Function ToString() As String
		  If val Is Nothing Then Return "null"
		  If TypeOf val Is Long? OrElse TypeOf val Is Integer? Then Return Convert.ToString(val)
		  Dim d As Double = val
		  If Double.IsInfinity(d) Then Return If(d > 0, "(1.0 / 0.0)", "(-1.0 / 0.0)")
		  If Double.IsNaN(d) Then Return "(0.0 / 0.0)"
		  Return Convert.ToString(d)
		End Function

		''' <summary>
		''' Applies the ValueExp on a MBean.
		''' </summary>
		''' <param name="name"> The name of the MBean on which the ValueExp will be applied.
		''' </param>
		''' <returns>  The <CODE>ValueExp</CODE>.
		''' </returns>
		''' <exception cref="BadStringOperationException"> </exception>
		''' <exception cref="BadBinaryOpValueExpException"> </exception>
		''' <exception cref="BadAttributeValueExpException"> </exception>
		''' <exception cref="InvalidApplicationException"> </exception>
		Public Overridable Function apply(ByVal name As ObjectName) As ValueExp Implements ValueExp.apply
			Return Me
		End Function

		''' <summary>
		''' Deserializes a <seealso cref="NumericValueExp"/> from an <seealso cref="ObjectInputStream"/>.
		''' </summary>
		Private Sub readObject(ByVal [in] As java.io.ObjectInputStream)
		  If compat Then
			' Read an object serialized in the old serial form
			'
			Dim doubleVal As Double
			Dim longVal As Long
			Dim isLong As Boolean
			Dim fields As java.io.ObjectInputStream.GetField = [in].readFields()
			doubleVal = fields.get("doubleVal", CDbl(0))
			If fields.defaulted("doubleVal") Then Throw New NullPointerException("doubleVal")
			longVal = fields.get("longVal", CLng(0))
			If fields.defaulted("longVal") Then Throw New NullPointerException("longVal")
			isLong = fields.get("valIsLong", False)
			If fields.defaulted("valIsLong") Then Throw New NullPointerException("valIsLong")
			If isLong Then
			  Me.val = longVal
			Else
			  Me.val = doubleVal
			End If
		  Else
			' Read an object serialized in the new serial form
			'
			[in].defaultReadObject()
		  End If
		End Sub


		''' <summary>
		''' Serializes a <seealso cref="NumericValueExp"/> to an <seealso cref="ObjectOutputStream"/>.
		''' </summary>
		Private Sub writeObject(ByVal out As java.io.ObjectOutputStream)
		  If compat Then
			' Serializes this instance in the old serial form
			'
			Dim fields As java.io.ObjectOutputStream.PutField = out.putFields()
			fields.put("doubleVal", doubleValue())
			fields.put("longVal", longValue())
			fields.put("valIsLong", [long])
			out.writeFields()
		  Else
			' Serializes this instance in the new serial form
			'
			out.defaultWriteObject()
		  End If
		End Sub

		<Obsolete> _
		Public Overrides Property mBeanServer Implements ValueExp.setMBeanServer As MBeanServer
			Set(ByVal s As MBeanServer)
				MyBase.mBeanServer = s
			End Set
		End Property

	End Class

End Namespace