Imports System

'
' * Copyright (c) 1999, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' This class represents the name of the Java implementation class of
	''' the MBean. It is used for performing queries based on the class of
	''' the MBean.
	''' @serial include
	''' 
	''' <p>The <b>serialVersionUID</b> of this class is <code>-1081892073854801359L</code>.
	''' 
	''' @since 1.5
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Friend Class ClassAttributeValueExp
		Inherits AttributeValueExp ' serialVersionUID is not constant

		' Serialization compatibility stuff:
		' Two serial forms are supported in this class. The selected form depends
		' on system property "jmx.serial.form":
		'  - "1.0" for JMX 1.0
		'  - any other value for JMX 1.1 and higher
		'
		' Serial version for old serial form
		Private Const oldSerialVersionUID As Long = -2212731951078526753L
		'
		' Serial version for new serial form
		Private Const newSerialVersionUID As Long = -1081892073854801359L

		Private Shared ReadOnly serialVersionUID As Long
		Shared Sub New()
			Dim compat As Boolean = False
			Try
				Dim act As New com.sun.jmx.mbeanserver.GetPropertyAction("jmx.serial.form")
				Dim form As String = java.security.AccessController.doPrivileged(act)
				compat = (form IsNot Nothing AndAlso form.Equals("1.0"))
			Catch e As Exception
				' OK: exception means no compat with 1.0, too bad
			End Try
			If compat Then
				serialVersionUID = oldSerialVersionUID
			Else
				serialVersionUID = newSerialVersionUID
			End If
		End Sub

		''' <summary>
		''' @serial The name of the attribute
		''' 
		''' <p>The <b>serialVersionUID</b> of this class is <code>-1081892073854801359L</code>.
		''' </summary>
		Private attr As String

		''' <summary>
		''' Basic Constructor.
		''' </summary>
		Public Sub New()
	'         Compatibility: we have an attr field that we must hold on to
	'           for serial compatibility, even though our parent has one too.  
			MyBase.New("Class")
			attr = "Class"
		End Sub


		''' <summary>
		''' Applies the ClassAttributeValueExp on an MBean. Returns the name of
		''' the Java implementation class of the MBean.
		''' </summary>
		''' <param name="name"> The name of the MBean on which the ClassAttributeValueExp will be applied.
		''' </param>
		''' <returns>  The ValueExp.
		''' </returns>
		''' <exception cref="BadAttributeValueExpException"> </exception>
		''' <exception cref="InvalidApplicationException"> </exception>
		Public Overrides Function apply(ByVal name As ObjectName) As ValueExp
			' getAttribute(name);
			Dim result As Object = getValue(name)
			If TypeOf result Is String Then
				Return New StringValueExp(CStr(result))
			Else
				Throw New BadAttributeValueExpException(result)
			End If
		End Function

		''' <summary>
		''' Returns the string "Class" representing its value
		''' </summary>
		Public Overrides Function ToString() As String
			Return attr
		End Function


		Protected Friend Overridable Function getValue(ByVal name As ObjectName) As Object
			Try
				' Get the class of the object
				Dim server As MBeanServer = QueryEval.mBeanServer
				Return server.getObjectInstance(name).className
			Catch re As Exception
				Return Nothing
	'             In principle the MBean does exist because otherwise we
	'               wouldn't be evaluating the query on it.  But it could
	'               potentially have disappeared in between the time we
	'               discovered it and the time the query is evaluated.
	'
	'               Also, the exception could be a SecurityException.
	'
	'               Returning null from here will cause
	'               BadAttributeValueExpException, which will in turn cause
	'               this MBean to be omitted from the query result.  
			End Try
		End Function

	End Class

End Namespace