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
	''' Represents strings that are arguments to relational constraints.
	''' A <CODE>StringValueExp</CODE> may be used anywhere a <CODE>ValueExp</CODE> is required.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class StringValueExp
		Implements ValueExp

		' Serial version 
		Private Const serialVersionUID As Long = -3256390509806284044L

		''' <summary>
		''' @serial The string literal
		''' </summary>
		Private val As String

		''' <summary>
		''' Basic constructor.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Creates a new <CODE>StringValueExp</CODE> representing the
		''' given string.
		''' </summary>
		''' <param name="val"> the string that will be the value of this expression </param>
		Public Sub New(ByVal val As String)
			Me.val = val
		End Sub

		''' <summary>
		''' Returns the string represented by the
		''' <CODE>StringValueExp</CODE> instance.
		''' </summary>
		''' <returns> the string. </returns>
		Public Overridable Property value As String
			Get
				Return val
			End Get
		End Property

		''' <summary>
		''' Returns the string representing the object.
		''' </summary>
		Public Overrides Function ToString() As String
			Return "'" & val.Replace("'", "''") & "'"
		End Function


		''' <summary>
		''' Sets the MBean server on which the query is to be performed.
		''' </summary>
		''' <param name="s"> The MBean server on which the query is to be performed. </param>
	'     There is no need for this method, because if a query is being
	'       evaluated a StringValueExp can only appear inside a QueryExp,
	'       and that QueryExp will itself have done setMBeanServer.  
		<Obsolete> _
		Public Overridable Property mBeanServer Implements ValueExp.setMBeanServer As MBeanServer
			Set(ByVal s As MBeanServer)
			End Set
		End Property

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
	End Class

End Namespace