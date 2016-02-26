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
	''' <p>Represents attributes used as arguments to relational constraints.
	''' Instances of this class are usually obtained using {@link Query#attr(String)
	''' Query.attr}.</p>
	''' 
	''' <p>An <CODE>AttributeValueExp</CODE> may be used anywhere a
	''' <CODE>ValueExp</CODE> is required.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class AttributeValueExp
		Implements ValueExp


		' Serial version 
		Private Const serialVersionUID As Long = -7768025046539163385L

		''' <summary>
		''' @serial The name of the attribute
		''' </summary>
		Private attr As String

		''' <summary>
		''' An <code>AttributeValueExp</code> with a null attribute. </summary>
		''' @deprecated An instance created with this constructor cannot be
		''' used in a query. 
		<Obsolete("An instance created with this constructor cannot be")> _
		Public Sub New()
		End Sub

		''' <summary>
		''' Creates a new <CODE>AttributeValueExp</CODE> representing the
		''' specified object attribute, named attr.
		''' </summary>
		''' <param name="attr"> the name of the attribute whose value is the value
		''' of this <seealso cref="ValueExp"/>. </param>
		Public Sub New(ByVal attr As String)
			Me.attr = attr
		End Sub

		''' <summary>
		''' Returns a string representation of the name of the attribute.
		''' </summary>
		''' <returns> the attribute name. </returns>
		Public Overridable Property attributeName As String
			Get
				Return attr
			End Get
		End Property

		''' <summary>
		''' <p>Applies the <CODE>AttributeValueExp</CODE> on an MBean.
		''' This method calls <seealso cref="#getAttribute getAttribute(name)"/> and wraps
		''' the result as a {@code ValueExp}.  The value returned by
		''' {@code getAttribute} must be a {@code Number}, {@code String},
		''' or {@code Boolean}; otherwise this method throws a
		''' {@code BadAttributeValueExpException}, which will cause
		''' the containing query to be false for this {@code name}.</p>
		''' </summary>
		''' <param name="name"> The name of the MBean on which the <CODE>AttributeValueExp</CODE> will be applied.
		''' </param>
		''' <returns>  The <CODE>ValueExp</CODE>.
		''' </returns>
		''' <exception cref="BadAttributeValueExpException"> </exception>
		''' <exception cref="InvalidApplicationException"> </exception>
		''' <exception cref="BadStringOperationException"> </exception>
		''' <exception cref="BadBinaryOpValueExpException">
		'''  </exception>
		Public Overrides Function apply(ByVal name As ObjectName) As ValueExp Implements ValueExp.apply
			Dim result As Object = getAttribute(name)

			If TypeOf result Is Number Then
				Return New NumericValueExp(CType(result, Number))
			ElseIf TypeOf result Is String Then
				Return New StringValueExp(CStr(result))
			ElseIf TypeOf result Is Boolean? Then
				Return New BooleanValueExp(CBool(result))
			Else
				Throw New BadAttributeValueExpException(result)
			End If
		End Function

		''' <summary>
		''' Returns the string representing its value.
		''' </summary>
		Public Overrides Function ToString() As String
			Return attr
		End Function


		''' <summary>
		''' Sets the MBean server on which the query is to be performed.
		''' </summary>
		''' <param name="s"> The MBean server on which the query is to be performed.
		''' </param>
		''' @deprecated This method has no effect.  The MBean Server used to
		''' obtain an attribute value is <seealso cref="QueryEval#getMBeanServer()"/>. 
	'     There is no need for this method, because if a query is being
	'       evaluted an AttributeValueExp can only appear inside a QueryExp,
	'       and that QueryExp will itself have done setMBeanServer.  
		<Obsolete("This method has no effect.  The MBean Server used to")> _
		Public Overrides Property mBeanServer Implements ValueExp.setMBeanServer As MBeanServer
			Set(ByVal s As MBeanServer)
			End Set
		End Property


		''' <summary>
		''' <p>Return the value of the given attribute in the named MBean.
		''' If the attempt to access the attribute generates an exception,
		''' return null.</p>
		''' 
		''' <p>The MBean Server used is the one returned by {@link
		''' QueryEval#getMBeanServer()}.</p>
		''' </summary>
		''' <param name="name"> the name of the MBean whose attribute is to be returned.
		''' </param>
		''' <returns> the value of the attribute, or null if it could not be
		''' obtained. </returns>
		Protected Friend Overridable Function getAttribute(ByVal name As ObjectName) As Object
			Try
				' Get the value from the MBeanServer

				Dim server As MBeanServer = QueryEval.mBeanServer

				Return server.getAttribute(name, attr)
			Catch re As Exception
				Return Nothing
			End Try
		End Function
	End Class

End Namespace