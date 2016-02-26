Imports System

'
' * Copyright (c) 2004, 2012, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.soap

	''' <summary>
	''' <code>SOAPElementFactory</code> is a factory for XML fragments that
	''' will eventually end up in the SOAP part. These fragments
	''' can be inserted as children of the <code>SOAPHeader</code> or
	''' <code>SOAPBody</code> or <code>SOAPEnvelope</code>.
	''' 
	''' <p>Elements created using this factory do not have the properties
	''' of an element that lives inside a SOAP header document. These
	''' elements are copied into the XML document tree when they are
	''' inserted. </summary>
	''' @deprecated - Use <code>javax.xml.soap.SOAPFactory</code> for creating SOAPElements. 
	''' <seealso cref= javax.xml.soap.SOAPFactory </seealso>
	Public Class SOAPElementFactory

		Private soapFactory As SOAPFactory

		Private Sub New(ByVal soapFactory As SOAPFactory)
			Me.soapFactory = soapFactory
		End Sub

		''' <summary>
		''' Create a <code>SOAPElement</code> object initialized with the
		''' given <code>Name</code> object.
		''' </summary>
		''' <param name="name"> a <code>Name</code> object with the XML name for
		'''             the new element
		''' </param>
		''' <returns> the new <code>SOAPElement</code> object that was
		'''         created
		''' </returns>
		''' <exception cref="SOAPException"> if there is an error in creating the
		'''            <code>SOAPElement</code> object
		''' </exception>
		''' @deprecated Use
		''' javax.xml.soap.SOAPFactory.createElement(javax.xml.soap.Name)
		''' instead
		''' 
		''' <seealso cref= javax.xml.soap.SOAPFactory#createElement(javax.xml.soap.Name) </seealso>
		''' <seealso cref= javax.xml.soap.SOAPFactory#createElement(javax.xml.namespace.QName) </seealso>
		Public Overridable Function create(ByVal name As Name) As SOAPElement
			Return soapFactory.createElement(name)
		End Function

		''' <summary>
		''' Create a <code>SOAPElement</code> object initialized with the
		''' given local name.
		''' </summary>
		''' <param name="localName"> a <code>String</code> giving the local name for
		'''             the new element
		''' </param>
		''' <returns> the new <code>SOAPElement</code> object that was
		'''         created
		''' </returns>
		''' <exception cref="SOAPException"> if there is an error in creating the
		'''            <code>SOAPElement</code> object
		''' </exception>
		''' @deprecated Use
		''' javax.xml.soap.SOAPFactory.createElement(String localName) instead
		''' 
		''' <seealso cref= javax.xml.soap.SOAPFactory#createElement(java.lang.String) </seealso>
		Public Overridable Function create(ByVal localName As String) As SOAPElement
			Return soapFactory.createElement(localName)
		End Function

		''' <summary>
		''' Create a new <code>SOAPElement</code> object with the given
		''' local name, prefix and uri.
		''' </summary>
		''' <param name="localName"> a <code>String</code> giving the local name
		'''                  for the new element </param>
		''' <param name="prefix"> the prefix for this <code>SOAPElement</code> </param>
		''' <param name="uri"> a <code>String</code> giving the URI of the
		'''            namespace to which the new element belongs
		''' </param>
		''' <exception cref="SOAPException"> if there is an error in creating the
		'''            <code>SOAPElement</code> object
		''' </exception>
		''' @deprecated Use
		''' javax.xml.soap.SOAPFactory.createElement(String localName,
		'''                      String prefix,
		'''                      String uri)
		''' instead
		''' 
		''' <seealso cref= javax.xml.soap.SOAPFactory#createElement(java.lang.String, java.lang.String, java.lang.String) </seealso>
		Public Overridable Function create(ByVal localName As String, ByVal prefix As String, ByVal uri As String) As SOAPElement
			Return soapFactory.createElement(localName, prefix, uri)
		End Function

		''' <summary>
		''' Creates a new instance of <code>SOAPElementFactory</code>.
		''' </summary>
		''' <returns> a new instance of a <code>SOAPElementFactory</code>
		''' </returns>
		''' <exception cref="SOAPException"> if there was an error creating the
		'''            default <code>SOAPElementFactory</code> </exception>
		Public Shared Function newInstance() As SOAPElementFactory
			Try
				Return New SOAPElementFactory(SOAPFactory.newInstance())
			Catch ex As Exception
				Throw New SOAPException("Unable to create SOAP Element Factory: " & ex.Message)
			End Try
		End Function
	End Class

End Namespace