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

Namespace javax.xml.ws.soap


	''' <summary>
	''' The <code>SOAPFaultException</code> exception represents a
	'''  SOAP 1.1 or 1.2 fault.
	''' 
	'''  <p>A <code>SOAPFaultException</code> wraps a SAAJ <code>SOAPFault</code>
	'''  that manages the SOAP-specific representation of faults.
	'''  The <code>createFault</code> method of
	'''  <code>javax.xml.soap.SOAPFactory</code> may be used to create an instance
	'''  of <code>javax.xml.soap.SOAPFault</code> for use with the
	'''  constructor. <code>SOAPBinding</code> contains an accessor for the
	'''  <code>SOAPFactory</code> used by the binding instance.
	''' 
	'''  <p>Note that the value of <code>getFault</code> is the only part of the
	'''  exception used when searializing a SOAP fault.
	''' 
	'''  <p>Refer to the SOAP specification for a complete
	'''  description of SOAP faults.
	''' </summary>
	'''  <seealso cref= javax.xml.soap.SOAPFault </seealso>
	'''  <seealso cref= javax.xml.ws.soap.SOAPBinding#getSOAPFactory </seealso>
	'''  <seealso cref= javax.xml.ws.ProtocolException
	''' 
	'''  @since JAX-WS 2.0
	'''  </seealso>
	Public Class SOAPFaultException
		Inherits javax.xml.ws.ProtocolException

		Private fault As javax.xml.soap.SOAPFault

		''' <summary>
		''' Constructor for SOAPFaultException </summary>
		'''  <param name="fault">   <code>SOAPFault</code> representing the fault
		''' </param>
		'''  <seealso cref= javax.xml.soap.SOAPFactory#createFault
		'''  </seealso>
		Public Sub New(ByVal fault As javax.xml.soap.SOAPFault)
			MyBase.New(fault.faultString)
			Me.fault = fault
		End Sub

		''' <summary>
		''' Gets the embedded <code>SOAPFault</code> instance.
		''' </summary>
		'''  <returns> <code>javax.xml.soap.SOAPFault</code> SOAP
		'''          fault element
		'''  </returns>
		Public Overridable Property fault As javax.xml.soap.SOAPFault
			Get
				Return Me.fault
			End Get
		End Property
	End Class

End Namespace