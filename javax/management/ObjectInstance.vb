Imports System

'
' * Copyright (c) 1999, 2004, Oracle and/or its affiliates. All rights reserved.
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

	' java import

	' RI import


	''' <summary>
	''' Used to represent the object name of an MBean and its class name.
	''' If the MBean is a Dynamic MBean the class name should be retrieved from
	''' the <CODE>MBeanInfo</CODE> it provides.
	''' 
	''' @since 1.5
	''' </summary>
	<Serializable> _
	Public Class ObjectInstance


		' Serial version 
		Private Const serialVersionUID As Long = -4099952623687795850L

		''' <summary>
		''' @serial Object name.
		''' </summary>
		Private name As javax.management.ObjectName

		''' <summary>
		''' @serial Class name.
		''' </summary>
		Private className As String

		''' <summary>
		''' Allows an object instance to be created given a string representation of
		''' an object name and the full class name, including the package name.
		''' </summary>
		''' <param name="objectName">  A string representation of the object name. </param>
		''' <param name="className"> The full class name, including the package
		''' name, of the object instance.  If the MBean is a Dynamic MBean
		''' the class name corresponds to its {@link
		''' DynamicMBean#getMBeanInfo()
		''' getMBeanInfo()}<code>.getClassName()</code>.
		''' </param>
		''' <exception cref="MalformedObjectNameException"> The string passed as a
		''' parameter does not have the right format.
		'''  </exception>
		Public Sub New(ByVal ___objectName As String, ByVal className As String)
			Me.New(New javax.management.ObjectName(___objectName), className)
		End Sub

		''' <summary>
		''' Allows an object instance to be created given an object name and
		''' the full class name, including the package name.
		''' </summary>
		''' <param name="objectName">  The object name. </param>
		''' <param name="className">  The full class name, including the package
		''' name, of the object instance.  If the MBean is a Dynamic MBean
		''' the class name corresponds to its {@link
		''' DynamicMBean#getMBeanInfo()
		''' getMBeanInfo()}<code>.getClassName()</code>.
		''' If the MBean is a Dynamic MBean the class name should be retrieved
		''' from the <CODE>MBeanInfo</CODE> it provides.
		'''  </param>
		Public Sub New(ByVal ___objectName As javax.management.ObjectName, ByVal className As String)
			If ___objectName.pattern Then
				Dim iae As New System.ArgumentException("Invalid name->" & ___objectName.ToString())
				Throw New RuntimeOperationsException(iae)
			End If
			Me.name= ___objectName
			Me.className= className
		End Sub


		''' <summary>
		''' Compares the current object instance with another object instance.
		''' </summary>
		''' <param name="object">  The object instance that the current object instance is
		'''     to be compared with.
		''' </param>
		''' <returns>  True if the two object instances are equal, otherwise false. </returns>
		Public Overrides Function Equals(ByVal [object] As Object) As Boolean
			If Not(TypeOf [object] Is ObjectInstance) Then Return False
			Dim val As ObjectInstance = CType([object], ObjectInstance)
			If Not name.Equals(val.objectName) Then Return False
			If className Is Nothing Then Return (val.className Is Nothing)
			Return className.Equals(val.className)
		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim classHash As Integer = (If(className Is Nothing, 0, className.GetHashCode()))
			Return name.GetHashCode() Xor classHash
		End Function

		''' <summary>
		''' Returns the object name part.
		''' </summary>
		''' <returns> the object name. </returns>
		Public Overridable Property objectName As javax.management.ObjectName
			Get
				Return name
			End Get
		End Property

		''' <summary>
		''' Returns the class part.
		''' </summary>
		''' <returns> the class name. </returns>
		Public Overridable Property className As String
			Get
				Return className
			End Get
		End Property

		''' <summary>
		''' Returns a string representing this ObjectInstance object. The format of this string
		''' is not specified, but users can expect that two ObjectInstances return the same
		''' string if and only if they are equal.
		''' </summary>
		Public Overrides Function ToString() As String
			Return className & "[" & objectName & "]"
		End Function
	End Class

End Namespace