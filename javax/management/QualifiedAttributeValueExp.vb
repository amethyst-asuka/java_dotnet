Imports System

'
' * Copyright (c) 1999, 2008, Oracle and/or its affiliates. All rights reserved.
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
	''' <p>Represents attributes used as arguments to relational constraints,
	''' where the attribute must be in an MBean of a specified {@linkplain
	''' MBeanInfo#getClassName() class}. A QualifiedAttributeValueExp may be used
	''' anywhere a ValueExp is required.
	''' 
	''' @serial include
	''' 
	''' @since 1.5
	''' </summary>
	Friend Class QualifiedAttributeValueExp
		Inherits AttributeValueExp


		' Serial version 
		Private Const serialVersionUID As Long = 8832517277410933254L

		''' <summary>
		''' @serial The attribute class name
		''' </summary>
		Private className As String


		''' <summary>
		''' Basic Constructor. </summary>
		''' @deprecated see <seealso cref="AttributeValueExp#AttributeValueExp()"/> 
		<Obsolete("see <seealso cref="AttributeValueExp#AttributeValueExp()"/>")> _
		Public Sub New()
		End Sub

		''' <summary>
		''' Creates a new QualifiedAttributeValueExp representing the specified object
		''' attribute, named attr with class name className.
		''' </summary>
		Public Sub New(ByVal className As String, ByVal attr As String)
			MyBase.New(attr)
			Me.className = className
		End Sub


		''' <summary>
		''' Returns a string representation of the class name of the attribute.
		''' </summary>
		Public Overridable Property attrClassName As String
			Get
				Return className
			End Get
		End Property

		''' <summary>
		''' Applies the QualifiedAttributeValueExp to an MBean.
		''' </summary>
		''' <param name="name"> The name of the MBean on which the QualifiedAttributeValueExp will be applied.
		''' </param>
		''' <returns>  The ValueExp.
		''' </returns>
		''' <exception cref="BadStringOperationException"> </exception>
		''' <exception cref="BadBinaryOpValueExpException"> </exception>
		''' <exception cref="BadAttributeValueExpException"> </exception>
		''' <exception cref="InvalidApplicationException"> </exception>
		Public Overrides Function apply(ByVal name As ObjectName) As ValueExp
			Try
				Dim server As MBeanServer = QueryEval.mBeanServer
				Dim v As String = server.getObjectInstance(name).className

				If v.Equals(className) Then Return MyBase.apply(name)
				Throw New InvalidApplicationException("Class name is " & v & ", should be " & className)

			Catch e As Exception
				Throw New InvalidApplicationException("Qualified attribute: " & e)
	'             Can happen if MBean disappears between the time we
	'               construct the list of MBeans to query and the time we
	'               evaluate the query on this MBean, or if
	'               getObjectInstance throws SecurityException.  
			End Try
		End Function

		''' <summary>
		''' Returns the string representing its value
		''' </summary>
		Public Overrides Function ToString() As String
			If className IsNot Nothing Then
				Return className & "." & MyBase.ToString()
			Else
				Return MyBase.ToString()
			End If
		End Function

	End Class

End Namespace