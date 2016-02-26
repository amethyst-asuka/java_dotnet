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
	''' The <code>Action</code> annotation allows explicit association of a
	''' WS-Addressing <code>Action</code> message addressing property with
	''' <code>input</code>, <code>output</code>, and
	''' <code>fault</code> messages of the mapped WSDL operation.
	''' <p>
	''' This annotation can be specified on each method of a service endpoint interface.
	''' For such a method, the mapped operation in the generated WSDL's
	''' <code>wsam:Action</code> attribute on the WSDL <code>input</code>,
	''' <code>output</code> and <code>fault</code> messages of the WSDL <code>operation</code>
	''' is based upon which attributes of the <code>Action</code> annotation have been specified.
	''' For the exact computation of <code>wsam:Action</code> values for the messages, refer
	''' to the algorithm in the JAX-WS specification.
	''' <p>
	''' <b>Example 1</b>: Specify explicit values for <code>Action</code> message addressing property
	''' for <code>input</code> and <code>output</code> messages.
	''' 
	''' <pre>
	''' &#64;WebService(targetNamespace="http://example.com/numbers")
	''' public class AddNumbersImpl {
	'''     <b>&#64;Action(
	'''         input="http://example.com/inputAction",
	'''         output="http://example.com/outputAction")</b>
	'''     public int addNumbers(int number1, int number2) {
	'''         return number1 + number2;
	'''     }
	''' }
	''' </pre>
	''' 
	''' The generated WSDL looks like:
	''' <pre>
	'''   &lt;definitions targetNamespace="http://example.com/numbers" ...>
	'''     ...
	'''     &lt;portType name="AddNumbersPortType">
	'''       &lt;operation name="AddNumbers">
	'''         &lt;input message="tns:AddNumbersInput" name="foo"
	'''           <b>wsam:Action="http://example.com/inputAction"</b>/>
	'''         &lt;output message="tns:AddNumbersOutput" name="bar"
	'''           <b>wsam:Action="http://example.com/outputAction"</b>/>
	'''       &lt;/operation>
	'''     &lt;/portType>
	'''     ...
	'''   &lt;/definitions>
	''' </pre>
	''' 
	''' <p>
	''' <b>Example 2</b>: Specify explicit value for <code>Action</code> message addressing property
	''' for only the <code>input</code> message. The <code>wsam:Action</code> values for the
	''' WSDL <code>output</code> message are computed using the algorithm in the JAX-WS specification.
	''' 
	''' <pre>
	''' &#64;WebService(targetNamespace="http://example.com/numbers")
	''' public class AddNumbersImpl {
	'''     <b>&#64;Action(input="http://example.com/inputAction")</b>
	'''     public int addNumbers(int number1, int number2) {
	'''         return number1 + number2;
	'''     }
	''' }
	''' </pre>
	''' 
	''' The generated WSDL looks like:
	''' <pre>
	'''   &lt;definitions targetNamespace="http://example.com/numbers" ...>
	'''     ...
	'''     &lt;portType name="AddNumbersPortType">
	'''       &lt;operation name="AddNumbers">
	'''         &lt;input message="tns:AddNumbersInput" name="foo"
	'''           <b>wsam:Action="http://example.com/inputAction"</b> />
	'''         &lt;output message="tns:AddNumbersOutput" name="bar"
	'''           <b>wsam:Action="http://example.com/numbers/AddNumbersPortType/AddNumbersResponse"</b>/>
	'''       &lt;/operation>
	'''     &lt;/portType>
	'''     ...
	'''   &lt;/definitions>
	''' </pre>
	''' 
	''' It is legitimate to specify an explicit value for <code>Action</code> message addressing property for
	''' <code>output</code> message only. In this case, <code>wsam:Action</code> value for the
	''' WSDL <code>input</code> message is computed using the algorithm in the JAX-WS specification.
	''' 
	''' <p>
	''' <b>Example 3</b>: See <seealso cref="FaultAction"/> annotation for an example of
	''' how to specify an explicit value for <code>Action</code> message addressing property for the
	''' <code>fault</code> message.
	''' </summary>
	''' <seealso cref= FaultAction
	''' 
	''' @since JAX-WS 2.1 </seealso>

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(AttributeTargets.Method, AllowMultiple := False, Inherited := False> _
	Public Class Action
		Inherits System.Attribute

		''' <summary>
		''' Explicit value of the WS-Addressing <code>Action</code> message addressing property for the <code>input</code>
		''' message of the operation.
		''' </summary>
		String input() default ""

		''' <summary>
		''' Explicit value of the WS-Addressing <code>Action</code> message addressing property for the <code>output</code>
		''' message of the operation.
		''' </summary>
		String output() default ""

		''' <summary>
		''' Explicit value of the WS-Addressing <code>Action</code> message addressing property for the <code>fault</code>
		''' message(s) of the operation. Each exception that is mapped to a fault and requires an explicit WS-Addressing
		''' <code>Action</code> message addressing property, needs to be specified as a value in this property
		''' using <seealso cref="FaultAction"/> annotation.
		''' </summary>
		FaultAction() fault() default
	End Class

End Namespace