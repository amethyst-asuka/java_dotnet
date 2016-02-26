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
	''' This feature represents the use of MTOM with a
	''' web service.
	''' 
	''' This feature can be used during the creation of SEI proxy, and
	''' <seealso cref="javax.xml.ws.Dispatch"/> instances on the client side and <seealso cref="Endpoint"/>
	''' instances on the server side. This feature cannot be used for <seealso cref="Service"/>
	''' instance creation on the client side.
	''' 
	''' <p>
	''' The following describes the affects of this feature with respect
	''' to being enabled or disabled:
	''' <ul>
	'''  <li> ENABLED: In this Mode, MTOM will be enabled. A receiver MUST accept
	''' both a non-optimized and an optimized message, and a sender MAY send an
	''' optimized message, or a non-optimized message. The heuristics used by a
	''' sender to determine whether to use optimization or not are
	''' implementation-specific.
	'''  <li> DISABLED: In this Mode, MTOM will be disabled
	''' </ul>
	''' <p>
	''' The <seealso cref="#threshold"/> property can be used to set the threshold
	''' value used to determine when binary data should be XOP encoded.
	''' 
	''' @since JAX-WS 2.1
	''' </summary>
	Public NotInheritable Class MTOMFeature
		Inherits javax.xml.ws.WebServiceFeature

		''' <summary>
		''' Constant value identifying the MTOMFeature
		''' </summary>
		Public Const ID As String = "http://www.w3.org/2004/08/soap/features/http-optimization"


		''' <summary>
		''' Property for MTOM threshold value. This property serves as a hint when
		''' MTOM is enabled, binary data above this size in bytes SHOULD be sent
		''' as attachment.
		''' The value of this property MUST always be >= 0. Default value is 0.
		''' </summary>
		' should be changed to private final, keeping original modifier to keep backwards compatibility
		Protected Friend threshold As Integer


		''' <summary>
		''' Create an <code>MTOMFeature</code>.
		''' The instance created will be enabled.
		''' </summary>
		Public Sub New()
			Me.enabled = True
			Me.threshold = 0
		End Sub

		''' <summary>
		''' Creates an <code>MTOMFeature</code>.
		''' </summary>
		''' <param name="enabled"> specifies if this feature should be enabled or not </param>
		Public Sub New(ByVal enabled As Boolean)
			Me.enabled = enabled
			Me.threshold = 0
		End Sub


		''' <summary>
		''' Creates an <code>MTOMFeature</code>.
		''' The instance created will be enabled.
		''' </summary>
		''' <param name="threshold"> the size in bytes that binary data SHOULD be before
		''' being sent as an attachment.
		''' </param>
		''' <exception cref="WebServiceException"> if threshold is < 0 </exception>
		Public Sub New(ByVal threshold As Integer)
			If threshold < 0 Then Throw New javax.xml.ws.WebServiceException("MTOMFeature.threshold must be >= 0, actual value: " & threshold)
			Me.enabled = True
			Me.threshold = threshold
		End Sub

		''' <summary>
		''' Creates an <code>MTOMFeature</code>.
		''' </summary>
		''' <param name="enabled"> specifies if this feature should be enabled or not </param>
		''' <param name="threshold"> the size in bytes that binary data SHOULD be before
		''' being sent as an attachment.
		''' </param>
		''' <exception cref="WebServiceException"> if threshold is < 0 </exception>
		Public Sub New(ByVal enabled As Boolean, ByVal threshold As Integer)
			If threshold < 0 Then Throw New javax.xml.ws.WebServiceException("MTOMFeature.threshold must be >= 0, actual value: " & threshold)
			Me.enabled = enabled
			Me.threshold = threshold
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
		''' Gets the threshold value used to determine when binary data
		''' should be sent as an attachment.
		''' </summary>
		''' <returns> the current threshold size in bytes </returns>
		Public Property threshold As Integer
			Get
				Return threshold
			End Get
		End Property
	End Class

End Namespace