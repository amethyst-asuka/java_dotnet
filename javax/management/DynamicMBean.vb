'
' * Copyright (c) 1999, 2003, Oracle and/or its affiliates. All rights reserved.
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
	''' Defines the methods that should be implemented by
	''' a Dynamic MBean (MBean that exposes a dynamic management interface).
	''' 
	''' @since 1.5
	''' </summary>
	Public Interface DynamicMBean


		''' <summary>
		''' Obtain the value of a specific attribute of the Dynamic MBean.
		''' </summary>
		''' <param name="attribute"> The name of the attribute to be retrieved
		''' </param>
		''' <returns>  The value of the attribute retrieved.
		''' </returns>
		''' <exception cref="AttributeNotFoundException"> </exception>
		''' <exception cref="MBeanException">  Wraps a <CODE>java.lang.Exception</CODE> thrown by the MBean's getter. </exception>
		''' <exception cref="ReflectionException">  Wraps a <CODE>java.lang.Exception</CODE> thrown while trying to invoke the getter.
		''' </exception>
		''' <seealso cref= #setAttribute </seealso>
		Function getAttribute(ByVal attribute As String) As Object

		''' <summary>
		''' Set the value of a specific attribute of the Dynamic MBean.
		''' </summary>
		''' <param name="attribute"> The identification of the attribute to
		''' be set and  the value it is to be set to.
		''' </param>
		''' <exception cref="AttributeNotFoundException"> </exception>
		''' <exception cref="InvalidAttributeValueException"> </exception>
		''' <exception cref="MBeanException"> Wraps a <CODE>java.lang.Exception</CODE> thrown by the MBean's setter. </exception>
		''' <exception cref="ReflectionException"> Wraps a <CODE>java.lang.Exception</CODE> thrown while trying to invoke the MBean's setter.
		''' </exception>
		''' <seealso cref= #getAttribute </seealso>
		WriteOnly Property attribute As Attribute

		''' <summary>
		''' Get the values of several attributes of the Dynamic MBean.
		''' </summary>
		''' <param name="attributes"> A list of the attributes to be retrieved.
		''' </param>
		''' <returns>  The list of attributes retrieved.
		''' </returns>
		''' <seealso cref= #setAttributes </seealso>
		Function getAttributes(ByVal attributes As String()) As AttributeList

		''' <summary>
		''' Sets the values of several attributes of the Dynamic MBean.
		''' </summary>
		''' <param name="attributes"> A list of attributes: The identification of the
		''' attributes to be set and  the values they are to be set to.
		''' </param>
		''' <returns>  The list of attributes that were set, with their new values.
		''' </returns>
		''' <seealso cref= #getAttributes </seealso>
		Function setAttributes(ByVal attributes As AttributeList) As AttributeList

		''' <summary>
		''' Allows an action to be invoked on the Dynamic MBean.
		''' </summary>
		''' <param name="actionName"> The name of the action to be invoked. </param>
		''' <param name="params"> An array containing the parameters to be set when the action is
		''' invoked. </param>
		''' <param name="signature"> An array containing the signature of the action. The class objects will
		''' be loaded through the same class loader as the one used for loading the
		''' MBean on which the action is invoked.
		''' </param>
		''' <returns>  The object returned by the action, which represents the result of
		''' invoking the action on the MBean specified.
		''' </returns>
		''' <exception cref="MBeanException">  Wraps a <CODE>java.lang.Exception</CODE> thrown by the MBean's invoked method. </exception>
		''' <exception cref="ReflectionException">  Wraps a <CODE>java.lang.Exception</CODE> thrown while trying to invoke the method </exception>
		Function invoke(ByVal actionName As String, Object ByVal  As params(), ByVal signature As String()) As Object

		''' <summary>
		''' Provides the exposed attributes and actions of the Dynamic MBean using an MBeanInfo object.
		''' </summary>
		''' <returns>  An instance of <CODE>MBeanInfo</CODE> allowing all attributes and actions
		''' exposed by this Dynamic MBean to be retrieved.
		'''  </returns>
		ReadOnly Property mBeanInfo As MBeanInfo

	End Interface

End Namespace