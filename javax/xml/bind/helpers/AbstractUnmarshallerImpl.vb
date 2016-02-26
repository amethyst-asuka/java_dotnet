Imports System

'
' * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.bind.helpers



	''' <summary>
	''' Partial default <tt>Unmarshaller</tt> implementation.
	''' 
	''' <p>
	''' This class provides a partial default implementation for the
	''' <seealso cref="javax.xml.bind.Unmarshaller"/>interface.
	''' 
	''' <p>
	''' A JAXB Provider has to implement five methods (getUnmarshallerHandler,
	''' unmarshal(Node), unmarshal(XMLReader,InputSource),
	''' unmarshal(XMLStreamReader), and unmarshal(XMLEventReader).
	''' 
	''' @author <ul>
	'''         <li>Kohsuke Kawaguchi, Sun Microsystems, Inc.</li>
	'''         </ul> </summary>
	''' <seealso cref= javax.xml.bind.Unmarshaller
	''' @since JAXB1.0 </seealso>
	Public MustInherit Class AbstractUnmarshallerImpl
		Implements javax.xml.bind.Unmarshaller

			Public MustOverride Sub afterUnmarshal(ByVal target As Object, ByVal parent As Object)
			Public MustOverride Sub beforeUnmarshal(ByVal target As Object, ByVal parent As Object)
			Public MustOverride WriteOnly Property eventHandler As javax.xml.bind.ValidationEventHandler
			Public MustOverride ReadOnly Property unmarshallerHandler As UnmarshallerHandler
			Public MustOverride Function unmarshal(ByVal reader As javax.xml.stream.XMLEventReader, ByVal declaredType As Type) As javax.xml.bind.JAXBElement(Of T)
			Public MustOverride Function unmarshal(ByVal reader As javax.xml.stream.XMLStreamReader, ByVal declaredType As Type) As javax.xml.bind.JAXBElement(Of T)
			Public MustOverride Function unmarshal(ByVal source As javax.xml.transform.Source, ByVal declaredType As Type) As javax.xml.bind.JAXBElement(Of T)
			Public MustOverride Function unmarshal(ByVal node As org.w3c.dom.Node, ByVal declaredType As Type) As javax.xml.bind.JAXBElement(Of T)
			Public MustOverride Function unmarshal(ByVal node As org.w3c.dom.Node) As Object
		''' <summary>
		''' handler that will be used to process errors and warnings during unmarshal </summary>
		Private eventHandler As javax.xml.bind.ValidationEventHandler = New DefaultValidationEventHandler

		''' <summary>
		''' whether or not the unmarshaller will validate </summary>
		Protected Friend validating As Boolean = False

		''' <summary>
		''' XMLReader that will be used to parse a document.
		''' </summary>
		Private reader As org.xml.sax.XMLReader = Nothing

		''' <summary>
		''' Obtains a configured XMLReader.
		''' 
		''' This method is used when the client-specified
		''' <seealso cref="SAXSource"/> object doesn't have XMLReader.
		''' 
		''' <seealso cref="Unmarshaller"/> is not re-entrant, so we will
		''' only use one instance of XMLReader.
		''' </summary>
		Protected Friend Overridable Property xMLReader As org.xml.sax.XMLReader
			Get
				If reader Is Nothing Then
					Try
						Dim parserFactory As javax.xml.parsers.SAXParserFactory
						parserFactory = javax.xml.parsers.SAXParserFactory.newInstance()
						parserFactory.namespaceAware = True
						' there is no point in asking a validation because
						' there is no guarantee that the document will come with
						' a proper schemaLocation.
						parserFactory.validating = False
						reader = parserFactory.newSAXParser().xMLReader
					Catch e As javax.xml.parsers.ParserConfigurationException
						Throw New javax.xml.bind.JAXBException(e)
					Catch e As org.xml.sax.SAXException
						Throw New javax.xml.bind.JAXBException(e)
					End Try
				End If
				Return reader
			End Get
		End Property

		Public Overridable Function unmarshal(ByVal source As javax.xml.transform.Source) As Object
			If source Is Nothing Then Throw New System.ArgumentException(Messages.format(Messages.MUST_NOT_BE_NULL, "source"))

			If TypeOf source Is javax.xml.transform.sax.SAXSource Then Return unmarshal(CType(source, javax.xml.transform.sax.SAXSource))
			If TypeOf source Is javax.xml.transform.stream.StreamSource Then Return unmarshal(streamSourceToInputSource(CType(source, javax.xml.transform.stream.StreamSource)))
			If TypeOf source Is javax.xml.transform.dom.DOMSource Then Return unmarshal(CType(source, javax.xml.transform.dom.DOMSource).node)

			' we don't handle other types of Source
			Throw New System.ArgumentException
		End Function

		' use the client specified XMLReader contained in the SAXSource.
		Private Function unmarshal(ByVal source As javax.xml.transform.sax.SAXSource) As Object

			Dim r As org.xml.sax.XMLReader = source.xMLReader
			If r Is Nothing Then r = xMLReader

			Return unmarshal(r, source.inputSource)
		End Function

		''' <summary>
		''' Unmarshals an object by using the specified XMLReader and the InputSource.
		''' 
		''' The callee should call the setErrorHandler method of the XMLReader
		''' so that errors are passed to the client-specified ValidationEventHandler.
		''' </summary>
		Protected Friend MustOverride Function unmarshal(ByVal reader As org.xml.sax.XMLReader, ByVal source As org.xml.sax.InputSource) As Object

		Public Function unmarshal(ByVal source As org.xml.sax.InputSource) As Object
			If source Is Nothing Then Throw New System.ArgumentException(Messages.format(Messages.MUST_NOT_BE_NULL, "source"))

			Return unmarshal(xMLReader, source)
		End Function


		Private Function unmarshal(ByVal url As String) As Object
			Return unmarshal(New org.xml.sax.InputSource(url))
		End Function

		Public Function unmarshal(ByVal url As java.net.URL) As Object
			If url Is Nothing Then Throw New System.ArgumentException(Messages.format(Messages.MUST_NOT_BE_NULL, "url"))

			Return unmarshal(url.toExternalForm())
		End Function

		Public Function unmarshal(ByVal f As java.io.File) As Object
			If f Is Nothing Then Throw New System.ArgumentException(Messages.format(Messages.MUST_NOT_BE_NULL, "file"))

			Try
				' copied from JAXP
				Dim path As String = f.absolutePath
				If System.IO.Path.DirectorySeparatorChar <> "/"c Then path = path.Replace(System.IO.Path.DirectorySeparatorChar, "/"c)
				If Not path.StartsWith("/") Then path = "/" & path
				If (Not path.EndsWith("/")) AndAlso f.directory Then path = path & "/"
				Return unmarshal(New java.net.URL("file", "", path))
			Catch e As java.net.MalformedURLException
				Throw New System.ArgumentException(e.Message)
			End Try
		End Function

		Public Function unmarshal(ByVal [is] As java.io.InputStream) As Object

			If [is] Is Nothing Then Throw New System.ArgumentException(Messages.format(Messages.MUST_NOT_BE_NULL, "is"))

			Dim isrc As New org.xml.sax.InputSource([is])
			Return unmarshal(isrc)
		End Function

		Public Function unmarshal(ByVal reader As java.io.Reader) As Object
			If reader Is Nothing Then Throw New System.ArgumentException(Messages.format(Messages.MUST_NOT_BE_NULL, "reader"))

			Dim isrc As New org.xml.sax.InputSource(reader)
			Return unmarshal(isrc)
		End Function


		Private Shared Function streamSourceToInputSource(ByVal ss As javax.xml.transform.stream.StreamSource) As org.xml.sax.InputSource
			Dim [is] As New org.xml.sax.InputSource
			[is].systemId = ss.systemId
			[is].byteStream = ss.inputStream
			[is].characterStream = ss.reader

			Return [is]
		End Function


		''' <summary>
		''' Indicates whether or not the Unmarshaller is configured to validate
		''' during unmarshal operations.
		''' <p>
		''' <i><b>Note:</b> I named this method isValidating() to stay in-line
		''' with JAXP, as opposed to naming it getValidating(). </i>
		''' </summary>
		''' <returns> true if the Unmarshaller is configured to validate during
		'''        unmarshal operations, false otherwise </returns>
		''' <exception cref="JAXBException"> if an error occurs while retrieving the validating
		'''        flag </exception>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Function isValidating() As Boolean 'JavaToDotNetTempPropertyGetvalidating
		Public Overridable Property validating As Boolean
			Get
				Return validating
			End Get
			Set(ByVal validating As Boolean)
		End Property

		''' <summary>
		''' Allow an application to register a validation event handler.
		''' <p>
		''' The validation event handler will be called by the JAXB Provider if any
		''' validation errors are encountered during calls to any of the
		''' <tt>unmarshal</tt> methods.  If the client application does not register
		''' a validation event handler before invoking the unmarshal methods, then
		''' all validation events will be silently ignored and may result in
		''' unexpected behaviour.
		''' </summary>
		''' <param name="handler"> the validation event handler </param>
		''' <exception cref="JAXBException"> if an error was encountered while setting the
		'''        event handler </exception>
		Public Overridable Property eventHandler As javax.xml.bind.ValidationEventHandler
			Set(ByVal handler As javax.xml.bind.ValidationEventHandler)
    
				If handler Is Nothing Then
					eventHandler = New DefaultValidationEventHandler
				Else
					eventHandler = handler
				End If
			End Set
			Get
				Return eventHandler
			End Get
		End Property

			Me.validating = validating
		End Sub



		''' <summary>
		''' Creates an UnmarshalException from a SAXException.
		''' 
		''' This is an utility method provided for the derived classes.
		''' 
		''' <p>
		''' When a provider-implemented ContentHandler wants to throw a
		''' JAXBException, it needs to wrap the exception by a SAXException.
		''' If the unmarshaller implementation blindly wrap SAXException
		''' by JAXBException, such an exception will be a JAXBException
		''' wrapped by a SAXException wrapped by another JAXBException.
		''' This is silly.
		''' 
		''' <p>
		''' This method checks the nested exception of SAXException
		''' and reduce those excessive wrapping.
		''' </summary>
		''' <returns> the resulting UnmarshalException </returns>
		Protected Friend Overridable Function createUnmarshalException(ByVal e As org.xml.sax.SAXException) As javax.xml.bind.UnmarshalException
			' check the nested exception to see if it's an UnmarshalException
			Dim nested As Exception = e.exception
			If TypeOf nested Is javax.xml.bind.UnmarshalException Then Return CType(nested, javax.xml.bind.UnmarshalException)

			If TypeOf nested Is Exception Then Throw CType(nested, Exception)


			' otherwise simply wrap it
			If nested IsNot Nothing Then
				Return New javax.xml.bind.UnmarshalException(nested)
			Else
				Return New javax.xml.bind.UnmarshalException(e)
			End If
		End Function

		''' <summary>
		''' Default implementation of the setProperty method always
		''' throws PropertyException since there are no required
		''' properties. If a provider needs to handle additional
		''' properties, it should override this method in a derived class.
		''' </summary>
		Public Overridable Sub setProperty(ByVal name As String, ByVal value As Object)

			If name Is Nothing Then Throw New System.ArgumentException(Messages.format(Messages.MUST_NOT_BE_NULL, "name"))

			Throw New javax.xml.bind.PropertyException(name, value)
		End Sub

		''' <summary>
		''' Default implementation of the getProperty method always
		''' throws PropertyException since there are no required
		''' properties. If a provider needs to handle additional
		''' properties, it should override this method in a derived class.
		''' </summary>
		Public Overridable Function getProperty(ByVal name As String) As Object

			If name Is Nothing Then Throw New System.ArgumentException(Messages.format(Messages.MUST_NOT_BE_NULL, "name"))

			Throw New javax.xml.bind.PropertyException(name)
		End Function

		Public Overridable Function unmarshal(ByVal reader As javax.xml.stream.XMLEventReader) As Object

			Throw New System.NotSupportedException
		End Function

		Public Overridable Function unmarshal(ByVal reader As javax.xml.stream.XMLStreamReader) As Object

			Throw New System.NotSupportedException
		End Function

		Public Overridable Function unmarshal(Of T)(ByVal node As org.w3c.dom.Node, ByVal expectedType As Type) As javax.xml.bind.JAXBElement(Of T)
			Throw New System.NotSupportedException
		End Function

		Public Overridable Function unmarshal(Of T)(ByVal source As javax.xml.transform.Source, ByVal expectedType As Type) As javax.xml.bind.JAXBElement(Of T)
			Throw New System.NotSupportedException
		End Function

		Public Overridable Function unmarshal(Of T)(ByVal reader As javax.xml.stream.XMLStreamReader, ByVal expectedType As Type) As javax.xml.bind.JAXBElement(Of T)
			Throw New System.NotSupportedException
		End Function

		Public Overridable Function unmarshal(Of T)(ByVal reader As javax.xml.stream.XMLEventReader, ByVal expectedType As Type) As javax.xml.bind.JAXBElement(Of T)
			Throw New System.NotSupportedException
		End Function

		Public Overridable Property schema As javax.xml.validation.Schema
			Set(ByVal schema As javax.xml.validation.Schema)
				Throw New System.NotSupportedException
			End Set
			Get
				Throw New System.NotSupportedException
			End Get
		End Property


		Public Overridable Property adapter As javax.xml.bind.annotation.adapters.XmlAdapter
			Set(ByVal adapter As javax.xml.bind.annotation.adapters.XmlAdapter)
				If adapter Is Nothing Then Throw New System.ArgumentException
				adapterter(CType(adapter.GetType(), [Class]),adapter)
			End Set
		End Property

		Public Overridable Sub setAdapter(Of A As javax.xml.bind.annotation.adapters.XmlAdapter)(ByVal type As Type, ByVal adapter As A)
			Throw New System.NotSupportedException
		End Sub

		Public Overridable Function getAdapter(Of A As javax.xml.bind.annotation.adapters.XmlAdapter)(ByVal type As Type) As A
			Throw New System.NotSupportedException
		End Function

		Public Overridable Property attachmentUnmarshaller As javax.xml.bind.attachment.AttachmentUnmarshaller
			Set(ByVal au As javax.xml.bind.attachment.AttachmentUnmarshaller)
				Throw New System.NotSupportedException
			End Set
			Get
				Throw New System.NotSupportedException
			End Get
		End Property


		Public Overridable Property listener As Listener
			Set(ByVal listener As Listener)
				Throw New System.NotSupportedException
			End Set
			Get
				Throw New System.NotSupportedException
			End Get
		End Property

	End Class

End Namespace