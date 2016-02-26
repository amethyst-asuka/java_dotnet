'
' * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.ws.soap


	''' <summary>
	''' AddressingFeature represents the use of WS-Addressing with either
	''' the SOAP 1.1/HTTP or SOAP 1.2/HTTP binding. Using this feature
	''' with any other binding is undefined.
	''' <p>
	''' This feature can be used during the creation of SEI proxy, and
	''' <seealso cref="javax.xml.ws.Dispatch"/> instances on the client side and <seealso cref="Endpoint"/>
	''' instances on the server side. This feature cannot be used for <seealso cref="Service"/>
	''' instance creation on the client side.
	''' <p>
	''' The following describes the effects of this feature with respect
	''' to be enabled or disabled:
	''' <ul>
	'''  <li> ENABLED: In this Mode, WS-Addressing will be enabled. It means
	'''       the endpoint supports WS-Addressing but does not require its use.
	'''       A sender could send messages with WS-Addressing headers or without
	'''       WS-Addressing headers. But a receiver MUST consume both types of
	'''       messages.
	'''  <li> DISABLED: In this Mode, WS-Addressing will be disabled.
	'''       At runtime, WS-Addressing headers MUST NOT be used by a sender or
	'''       receiver.
	''' </ul>
	''' <p>
	''' If the feature is enabled, the <code>required</code> property determines
	''' whether the endpoint requires WS-Addressing. If it is set true,
	''' WS-Addressing headers MUST be present on incoming and outgoing messages.
	''' By default the <code>required</code> property is <code>false</code>.
	''' 
	''' <p>
	''' If the web service developer has not explicitly enabled this feature,
	''' WSDL's wsam:Addressing policy assertion is used to find
	''' the use of WS-Addressing. By using the feature explicitly, an application
	''' overrides WSDL's indication of the use of WS-Addressing. In some cases,
	''' this is really required. For example, if an application has implemented
	''' WS-Addressing itself, it can use this feature to disable addressing. That
	''' means a JAX-WS implementation doesn't consume or produce WS-Addressing
	''' headers.
	''' 
	''' <p>
	''' If addressing is enabled, a corresponding wsam:Addressing policy assertion
	''' must be generated in the WSDL as per
	''' <a href="http://www.w3.org/TR/ws-addr-metadata/#wspolicyassertions">
	''' 3.1 WS-Policy Assertions</a>
	''' 
	''' <p>
	''' <b>Example 1: </b>Possible Policy Assertion in the generated WSDL for
	''' <code>&#64;Addressing</code>
	''' <pre>
	'''   &lt;wsam:Addressing wsp:Optional="true">
	'''     &lt;wsp:Policy/>
	'''   &lt;/wsam:Addressing>
	''' </pre>
	''' 
	''' <p>
	''' <b>Example 2: </b>Possible Policy Assertion in the generated WSDL for
	''' <code>&#64;Addressing(required=true)</code>
	''' <pre>
	'''   &lt;wsam:Addressing>
	'''     &lt;wsp:Policy/>
	'''   &lt;/wsam:Addressing>
	''' </pre>
	''' 
	''' <p>
	''' <b>Example 3: </b>Possible Policy Assertion in the generated WSDL for
	''' <code>&#64;Addressing(required=true, responses=Responses.ANONYMOUS)</code>
	''' <pre>
	'''   &lt;wsam:Addressing>
	'''      &lt;wsp:Policy>
	'''        &lt;wsam:AnonymousResponses/>
	'''      &lt;/wsp:Policy>
	'''   &lt;/wsam:Addressing>
	''' </pre>
	''' 
	''' <p>
	''' See <a href="http://www.w3.org/TR/2006/REC-ws-addr-core-20060509/">
	''' Web Services Addressing - Core</a>,
	''' <a href="http://www.w3.org/TR/2006/REC-ws-addr-soap-20060509/">
	''' Web Services Addressing 1.0 - SOAP Binding</a>,
	''' and <a href="http://www.w3.org/TR/ws-addr-metadata/">
	''' Web Services Addressing 1.0 - Metadata</a>
	''' for more information on WS-Addressing.
	''' </summary>
	''' <seealso cref= Addressing
	''' @since JAX-WS 2.1 </seealso>

	Public NotInheritable Class AddressingFeature
		Inherits javax.xml.ws.WebServiceFeature

		''' <summary>
		''' Constant value identifying the AddressingFeature
		''' </summary>
		Public Const ID As String = "http://www.w3.org/2005/08/addressing/module"

		''' <summary>
		''' If addressing is enabled, this property determines whether the endpoint
		''' requires WS-Addressing. If required is true, WS-Addressing headers MUST
		''' be present on incoming and outgoing messages.
		''' </summary>
		' should be private final, keeping original modifier due to backwards compatibility
		Protected Friend required As Boolean

		''' <summary>
		''' If addressing is enabled, this property determines if endpoint requires
		''' the use of only anonymous responses, or only non-anonymous responses, or all.
		''' 
		''' <p>
		''' <seealso cref="Responses#ALL"/> supports all response types and this is the default
		''' value.
		''' 
		''' <p>
		''' <seealso cref="Responses#ANONYMOUS"/> requires the use of only anonymous
		''' responses. It will result into wsam:AnonymousResponses nested assertion
		''' as specified in
		''' <a href="http://www.w3.org/TR/ws-addr-metadata/#wspolicyanonresponses">
		''' 3.1.2 AnonymousResponses Assertion</a> in the generated WSDL.
		''' 
		''' <p>
		''' <seealso cref="Responses#NON_ANONYMOUS"/> requires the use of only non-anonymous
		''' responses. It will result into
		''' wsam:NonAnonymousResponses nested assertion as specified in
		''' <a href="http://www.w3.org/TR/ws-addr-metadata/#wspolicynonanonresponses">
		''' 3.1.3 NonAnonymousResponses Assertion</a> in the generated WSDL.
		''' 
		''' @since JAX-WS 2.2
		''' </summary>
		Public Enum Responses
			''' <summary>
			''' Specifies the use of only anonymous
			''' responses. It will result into wsam:AnonymousResponses nested assertion
			''' as specified in
			''' <a href="http://www.w3.org/TR/ws-addr-metadata/#wspolicyanonresponses">
			''' 3.1.2 AnonymousResponses Assertion</a> in the generated WSDL.
			''' </summary>
			ANONYMOUS

			''' <summary>
			''' Specifies the use of only non-anonymous
			''' responses. It will result into
			''' wsam:NonAnonymousResponses nested assertion as specified in
			''' <a href="http://www.w3.org/TR/ws-addr-metadata/#wspolicynonanonresponses">
			''' 3.1.3 NonAnonymousResponses Assertion</a> in the generated WSDL.
			''' </summary>
			NON_ANONYMOUS

			''' <summary>
			''' Supports all response types and this is the default
			''' </summary>
			ALL
		End Enum

		Private ReadOnly responses As Responses

		''' <summary>
		''' Creates and configures an <code>AddressingFeature</code> with the
		''' use of addressing requirements. The created feature enables
		''' ws-addressing i.e. supports ws-addressing but doesn't require
		''' its use. It is also configured to accept all the response types.
		''' </summary>
		Public Sub New()
			Me.New(True, False, Responses.ALL)
		End Sub

		''' <summary>
		''' Creates and configures an <code>AddressingFeature</code> with the
		''' use of addressing requirements. If <code>enabled</code> is true,
		''' it enables ws-addressing i.e. supports ws-addressing but doesn't
		''' require its use. It also configures to accept all the response types.
		''' </summary>
		''' <param name="enabled"> true enables ws-addressing i.e.ws-addressing
		''' is supported but doesn't require its use </param>
		Public Sub New(ByVal enabled As Boolean)
			Me.New(enabled, False, Responses.ALL)
		End Sub

		''' <summary>
		''' Creates and configures an <code>AddressingFeature</code> with the
		''' use of addressing requirements. If <code>enabled</code> and
		''' <code>required</code> are true, it enables ws-addressing and
		''' requires its use. It also configures to accept all the response types.
		''' </summary>
		''' <param name="enabled"> true enables ws-addressing i.e.ws-addressing
		''' is supported but doesn't require its use </param>
		''' <param name="required"> true means requires the use of ws-addressing . </param>
		Public Sub New(ByVal enabled As Boolean, ByVal required As Boolean)
			Me.New(enabled, required, Responses.ALL)
		End Sub

		''' <summary>
		''' Creates and configures an <code>AddressingFeature</code> with the
		''' use of addressing requirements. If <code>enabled</code> and
		''' <code>required</code> are true, it enables ws-addressing and
		''' requires its use. Also, the response types can be configured using
		''' <code>responses</code> parameter.
		''' </summary>
		''' <param name="enabled"> true enables ws-addressing i.e.ws-addressing
		''' is supported but doesn't require its use </param>
		''' <param name="required"> true means requires the use of ws-addressing . </param>
		''' <param name="responses"> specifies what type of responses are required
		''' 
		''' @since JAX-WS 2.2 </param>
		Public Sub New(ByVal enabled As Boolean, ByVal required As Boolean, ByVal responses As Responses)
			Me.enabled = enabled
			Me.required = required
			Me.responses = responses
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Property Overrides iD As String
			Get
				Return ID
			End Get
		End Property

		''' <summary>
		''' If addressing is enabled, this property determines whether the endpoint
		''' requires WS-Addressing. If required is true, WS-Addressing headers MUST
		''' be present on incoming and outgoing messages.
		''' </summary>
		''' <returns> the current required value </returns>
		Public Property required As Boolean
			Get
				Return required
			End Get
		End Property

		''' <summary>
		''' If addressing is enabled, this property determines whether endpoint
		''' requires the use of anonymous responses, or non-anonymous responses,
		''' or all responses.
		''' 
		''' <p> </summary>
		''' <returns> <seealso cref="Responses#ALL"/> when endpoint supports all types of
		''' responses,
		'''         <seealso cref="Responses#ANONYMOUS"/> when endpoint requires the use of
		''' only anonymous responses,
		'''         <seealso cref="Responses#NON_ANONYMOUS"/> when endpoint requires the use
		''' of only non-anonymous responses
		''' 
		''' @since JAX-WS 2.2 </returns>
		Public Property responses As Responses
			Get
				Return responses
			End Get
		End Property

	End Class

End Namespace