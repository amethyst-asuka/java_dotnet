Imports System

'
' * Copyright (c) 2005, 2010, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.ws


	''' <summary>
	''' The <code>FaultAction</code> annotation is used inside an <seealso cref="Action"/>
	''' annotation to allow an explicit association of a WS-Addressing
	''' <code>Action</code> message addressing property with the <code>fault</code>
	''' messages of the WSDL operation mapped from the exception class.
	''' <p>
	''' The <code>wsam:Action</code> attribute value in the <code>fault</code>
	''' message in the generated WSDL operation mapped for <code>className</code>
	''' class is equal to the corresponding value in the <code>FaultAction</code>.
	''' For the exact computation of <code>wsam:Action</code> values for the
	''' fault messages, refer to the algorithm in the JAX-WS specification.
	''' 
	''' <p>
	''' <b>Example 1</b>: Specify explicit values for <code>Action</code> message addressing
	''' property for the <code>input</code>, <code>output</code> and <code>fault</code> message
	''' if the Java method throws only one service specific exception.
	''' 
	''' <pre>
	''' &#64;WebService(targetNamespace="http://example.com/numbers")
	''' public class AddNumbersImpl {
	'''     &#64;Action(
	'''         fault = {
	'''             <b>&#64;FaultAction(className=AddNumbersException.class, value="http://example.com/faultAction")</b>
	'''         })
	'''     public int addNumbers(int number1, int number2)
	'''         throws AddNumbersException {
	'''         return number1 + number2;
	'''     }
	''' }
	''' </pre>
	''' 
	''' The generated WSDL looks like:
	''' 
	''' <pre>
	'''   &lt;definitions targetNamespace="http://example.com/numbers" ...>
	'''     ...
	'''     &lt;portType name="AddNumbersPortType">
	'''       &lt;operation name="AddNumbers">
	'''         ...
	'''         &lt;fault message="tns:AddNumbersException" name="AddNumbersException"
	'''           <b>wsam:Action="http://example.com/faultAction"</b>/>
	'''       &lt;/operation>
	'''     &lt;/portType>
	'''     ...
	'''   &lt;/definitions>
	''' </pre>
	''' 
	''' <p>
	''' Example 2: Here is an example that shows if the explicit value for <code>Action</code>
	''' message addressing property for the service specific exception is not present.
	''' 
	''' <pre>
	''' &#64;WebService(targetNamespace="http://example.com/numbers")
	''' public class AddNumbersImpl {
	'''     public int addNumbers(int number1, int number2)
	'''         throws AddNumbersException {
	'''         return number1 + number2;
	'''     }
	''' }
	''' </pre>
	''' 
	''' The generated WSDL looks like:
	''' 
	''' <pre>
	'''   &lt;definitions targetNamespace="http://example.com/numbers" ...>
	'''     ...
	'''     &lt;portType name="AddNumbersPortType">
	'''       &lt;operation name="AddNumbers">
	'''         ...
	'''         &lt;fault message="tns:addNumbersFault" name="InvalidNumbers"
	'''           <b>wsam:Action="http://example.com/numbers/AddNumbersPortType/AddNumbers/Fault/AddNumbersException"</b>/>
	'''       &lt;/operation>
	'''     &lt;/portType>
	'''     ...
	'''   &lt;/definitions>
	''' </pre>
	''' 
	''' <p>
	''' Example 3: Here is an example that shows how to specify explicit values for <code>Action</code>
	''' message addressing property if the Java method throws more than one service specific exception.
	''' 
	''' <pre>
	''' &#64;WebService(targetNamespace="http://example.com/numbers")
	''' public class AddNumbersImpl {
	'''     &#64;Action(
	'''         fault = {
	'''             <b>&#64;FaultAction(className=AddNumbersException.class, value="http://example.com/addFaultAction"),
	'''             &#64;FaultAction(className=TooBigNumbersException.class, value="http://example.com/toobigFaultAction")</b>
	'''         })
	'''     public int addNumbers(int number1, int number2)
	'''         throws AddNumbersException, TooBigNumbersException {
	'''         return number1 + number2;
	'''     }
	''' }
	''' </pre>
	''' 
	''' The generated WSDL looks like:
	''' 
	''' <pre>
	'''   &lt;definitions targetNamespace="http://example.com/numbers" ...>
	'''     ...
	'''     &lt;portType name="AddNumbersPortType">
	'''       &lt;operation name="AddNumbers">
	'''         ...
	'''         &lt;fault message="tns:addNumbersFault" name="AddNumbersException"
	'''           <b>wsam:Action="http://example.com/addFaultAction"</b>/>
	'''         &lt;fault message="tns:tooBigNumbersFault" name="TooBigNumbersException"
	'''           <b>wsam:Action="http://example.com/toobigFaultAction"</b>/>
	'''       &lt;/operation>
	'''     &lt;/portType>
	'''     ...
	'''   &lt;/definitions>
	''' </pre>
	''' 
	''' @since JAX-WS 2.1
	''' </summary>

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(AttributeTargets.Method, AllowMultiple := False, Inherited := False> _
	Public Class FaultAction
		Inherits System.Attribute

		''' <summary>
		''' Name of the exception class
		''' </summary>
		Type className()

		''' <summary>
		''' Value of WS-Addressing <code>Action</code> message addressing property for the exception
		''' </summary>
		String value() default ""
	End Class

End Namespace