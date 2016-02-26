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
	''' This class represents a boolean value. A BooleanValueExp may be
	''' used anywhere a ValueExp is required.
	''' @serial include
	''' 
	''' @since 1.5
	''' </summary>
	Friend Class BooleanValueExp
		Inherits QueryEval
		Implements ValueExp

		' Serial version 
		Private Const serialVersionUID As Long = 7754922052666594581L

		''' <summary>
		''' @serial The boolean value
		''' </summary>
		Private val As Boolean = False


		''' <summary>
		''' Creates a new BooleanValueExp representing the boolean literal {@code val}. </summary>
		Friend Sub New(ByVal val As Boolean)
			Me.val = val
		End Sub

		''' <summary>
		'''Creates a new BooleanValueExp representing the Boolean object {@code val}. </summary>
		Friend Sub New(ByVal val As Boolean?)
			Me.val = val
		End Sub


		''' <summary>
		''' Returns the  Boolean object representing the value of the BooleanValueExp object. </summary>
		Public Overridable Property value As Boolean?
			Get
				Return Convert.ToBoolean(val)
			End Get
		End Property

		''' <summary>
		''' Returns the string representing the object.
		''' </summary>
		Public Overrides Function ToString() As String
			Return Convert.ToString(val)
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

		<Obsolete> _
		Public Overrides Property mBeanServer Implements ValueExp.setMBeanServer As MBeanServer
			Set(ByVal s As MBeanServer)
				MyBase.mBeanServer = s
			End Set
		End Property


	End Class

End Namespace