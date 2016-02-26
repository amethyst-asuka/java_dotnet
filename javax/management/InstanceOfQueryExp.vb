'
' * Copyright (c) 2005, Oracle and/or its affiliates. All rights reserved.
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
	''' This class is used by the query building mechanism for isInstanceOf expressions.
	''' @serial include
	''' 
	''' @since 1.6
	''' </summary>
	Friend Class InstanceOfQueryExp
		Inherits QueryEval
		Implements QueryExp

		' Serial version 
		Private Const serialVersionUID As Long = -1081892073854801359L

		''' <summary>
		''' @serial The <seealso cref="StringValueExp"/> returning the name of the class
		'''         of which selected MBeans should be instances.
		''' </summary>
		Private classNameValue As StringValueExp

		''' <summary>
		''' Creates a new InstanceOfExp with a specific class name. </summary>
		''' <param name="classNameValue"> The <seealso cref="StringValueExp"/> returning the name of
		'''        the class of which selected MBeans should be instances. </param>
		' We are using StringValueExp here to be consistent with other queries,
		' although we should actually either use a simple string (the classname)
		' or a ValueExp - which would allow more complex queries - like for
		' instance evaluating the class name from an AttributeValueExp.
		' As it stands - using StringValueExp instead of a simple constant string
		' doesn't serve any useful purpose besides offering a consistent
		' look & feel.
		Public Sub New(ByVal classNameValue As StringValueExp)
			If classNameValue Is Nothing Then Throw New System.ArgumentException("Null class name.")

			Me.classNameValue = classNameValue
		End Sub

		''' <summary>
		''' Returns the class name.
		''' @returns The <seealso cref="StringValueExp"/> returning the name of
		'''        the class of which selected MBeans should be instances.
		''' </summary>
		Public Overridable Property classNameValue As StringValueExp
			Get
				Return classNameValue
			End Get
		End Property

		''' <summary>
		''' Applies the InstanceOf on a MBean.
		''' </summary>
		''' <param name="name"> The name of the MBean on which the InstanceOf will be applied.
		''' </param>
		''' <returns>  True if the MBean specified by the name is instance of the class. </returns>
		''' <exception cref="BadAttributeValueExpException"> </exception>
		''' <exception cref="InvalidApplicationException"> </exception>
		''' <exception cref="BadStringOperationException"> </exception>
		''' <exception cref="BadBinaryOpValueExpException"> </exception>
		Public Overridable Function apply(ByVal name As ObjectName) As Boolean Implements QueryExp.apply

			' Get the class name value
			Dim val As StringValueExp
			Try
				val = CType(classNameValue.apply(name), StringValueExp)
			Catch x As ClassCastException
				' Should not happen - unless someone wrongly implemented
				' StringValueExp.apply().
				Dim y As New BadStringOperationException(x.ToString())
				y.initCause(x)
				Throw y
			End Try

			' Test whether the MBean is an instance of that class.
			Try
				Return mBeanServer.isInstanceOf(name, val.value)
			Catch infe As InstanceNotFoundException
				Return False
			End Try
		End Function

		''' <summary>
		''' Returns a string representation of this InstanceOfQueryExp. </summary>
		''' <returns> a string representation of this InstanceOfQueryExp. </returns>
		Public Overrides Function ToString() As String
		   Return "InstanceOf " & classNameValue.ToString()
		End Function
	End Class

End Namespace