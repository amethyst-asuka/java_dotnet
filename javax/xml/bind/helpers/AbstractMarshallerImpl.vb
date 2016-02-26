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

	' J2SE1.4 feature
	' import java.nio.charset.Charset;
	' import java.nio.charset.UnsupportedCharsetException;

	''' <summary>
	''' Partial default <tt>Marshaller</tt> implementation.
	''' 
	''' <p>
	''' This class provides a partial default implementation for the
	''' <seealso cref="javax.xml.bind.Marshaller"/> interface.
	''' 
	''' <p>
	''' The only methods that a JAXB Provider has to implement are
	''' <seealso cref="Marshaller#marshal(Object, javax.xml.transform.Result) marshal(Object, javax.xml.transform.Result)"/>,
	''' <seealso cref="Marshaller#marshal(Object, javax.xml.transform.Result) marshal(Object, javax.xml.stream.XMLStreamWriter)"/>, and
	''' <seealso cref="Marshaller#marshal(Object, javax.xml.transform.Result) marshal(Object, javax.xml.stream.XMLEventWriter)"/>.
	''' 
	''' @author <ul><li>Kohsuke Kawaguchi, Sun Microsystems, Inc.</li></ul> </summary>
	''' <seealso cref= javax.xml.bind.Marshaller
	''' @since JAXB1.0 </seealso>
	Public MustInherit Class AbstractMarshallerImpl
		Implements javax.xml.bind.Marshaller

			Public MustOverride Sub afterMarshal(ByVal source As Object)
			Public MustOverride Sub beforeMarshal(ByVal source As Object)
			Public MustOverride WriteOnly Property eventHandler As javax.xml.bind.ValidationEventHandler
			Public MustOverride Function getNode(ByVal contentTree As Object) As org.w3c.dom.Node
			Public MustOverride Sub marshal(ByVal jaxbElement As Object, ByVal writer As javax.xml.stream.XMLEventWriter)
			Public MustOverride Sub marshal(ByVal jaxbElement As Object, ByVal writer As javax.xml.stream.XMLStreamWriter)
			Public MustOverride Sub marshal(ByVal jaxbElement As Object, ByVal node As org.w3c.dom.Node)
			Public MustOverride Sub marshal(ByVal jaxbElement As Object, ByVal handler As org.xml.sax.ContentHandler)
			Public MustOverride Sub marshal(ByVal jaxbElement As Object, ByVal writer As java.io.Writer)
			Public MustOverride Sub marshal(ByVal jaxbElement As Object, ByVal os As java.io.OutputStream)
			Public MustOverride Sub marshal(ByVal jaxbElement As Object, ByVal result As javax.xml.transform.Result)
		''' <summary>
		''' handler that will be used to process errors and warnings during marshal </summary>
		Private eventHandler As javax.xml.bind.ValidationEventHandler = New DefaultValidationEventHandler

		'J2SE1.4 feature
		'private Charset encoding = null;

		''' <summary>
		''' store the value of the encoding property. </summary>
		Private encoding As String = "UTF-8"

		''' <summary>
		''' store the value of the schemaLocation property. </summary>
		Private schemaLocation As String = Nothing

		''' <summary>
		''' store the value of the noNamespaceSchemaLocation property. </summary>
		Private noNSSchemaLocation As String = Nothing

		''' <summary>
		''' store the value of the formattedOutput property. </summary>
		Private formattedOutput As Boolean = False

		''' <summary>
		''' store the value of the fragment property. </summary>
		Private fragment As Boolean = False

		Public Sub marshal(ByVal obj As Object, ByVal os As java.io.OutputStream)

			checkNotNull(obj, "obj", os, "os")
			marshal(obj, New javax.xml.transform.stream.StreamResult(os))
		End Sub

		Public Overridable Sub marshal(ByVal jaxbElement As Object, ByVal output As java.io.File)
			checkNotNull(jaxbElement, "jaxbElement", output, "output")
			Try
				Dim os As java.io.OutputStream = New java.io.BufferedOutputStream(New java.io.FileOutputStream(output))
				Try
					marshal(jaxbElement, New javax.xml.transform.stream.StreamResult(os))
				Finally
					os.close()
				End Try
			Catch e As java.io.IOException
				Throw New javax.xml.bind.JAXBException(e)
			End Try
		End Sub

		Public Sub marshal(ByVal obj As Object, ByVal w As java.io.Writer)

			checkNotNull(obj, "obj", w, "writer")
			marshal(obj, New javax.xml.transform.stream.StreamResult(w))
		End Sub

		Public Sub marshal(ByVal obj As Object, ByVal handler As org.xml.sax.ContentHandler)

			checkNotNull(obj, "obj", handler, "handler")
			marshal(obj, New javax.xml.transform.sax.SAXResult(handler))
		End Sub

		Public Sub marshal(ByVal obj As Object, ByVal node As org.w3c.dom.Node)

			checkNotNull(obj, "obj", node, "node")
			marshal(obj, New javax.xml.transform.dom.DOMResult(node))
		End Sub

		''' <summary>
		''' By default, the getNode method is unsupported and throw
		''' an <seealso cref="java.lang.UnsupportedOperationException"/>.
		''' 
		''' Implementations that choose to support this method must
		''' override this method.
		''' </summary>
		Public Overridable Function getNode(ByVal obj As Object) As org.w3c.dom.Node

			checkNotNull(obj, "obj", Boolean.TRUE, "foo")

			Throw New System.NotSupportedException
		End Function

		''' <summary>
		''' Convenience method for getting the current output encoding.
		''' </summary>
		''' <returns> the current encoding or "UTF-8" if it hasn't been set. </returns>
		Protected Friend Overridable Property encoding As String
			Get
				Return encoding
			End Get
			Set(ByVal encoding As String)
				Me.encoding = encoding
			End Set
		End Property


		''' <summary>
		''' Convenience method for getting the current schemaLocation.
		''' </summary>
		''' <returns> the current schemaLocation or null if it hasn't been set </returns>
		Protected Friend Overridable Property schemaLocation As String
			Get
				Return schemaLocation
			End Get
			Set(ByVal location As String)
				schemaLocation = location
			End Set
		End Property


		''' <summary>
		''' Convenience method for getting the current noNamespaceSchemaLocation.
		''' </summary>
		''' <returns> the current noNamespaceSchemaLocation or null if it hasn't
		''' been set </returns>
		Protected Friend Overridable Property noNSSchemaLocation As String
			Get
				Return noNSSchemaLocation
			End Get
			Set(ByVal location As String)
				noNSSchemaLocation = location
			End Set
		End Property


		''' <summary>
		''' Convenience method for getting the formatted output flag.
		''' </summary>
		''' <returns> the current value of the formatted output flag or false if
		''' it hasn't been set. </returns>
		Protected Friend Overridable Property formattedOutput As Boolean
			Get
				Return formattedOutput
			End Get
			Set(ByVal v As Boolean)
				formattedOutput = v
			End Set
		End Property



		''' <summary>
		''' Convenience method for getting the fragment flag.
		''' </summary>
		''' <returns> the current value of the fragment flag or false if
		''' it hasn't been set. </returns>
		Protected Friend Overridable Property fragment As Boolean
			Get
				Return fragment
			End Get
			Set(ByVal v As Boolean)
				fragment = v
			End Set
		End Property



		Friend Shared aliases As String() = { "UTF-8", "UTF8", "UTF-16", "Unicode", "UTF-16BE", "UnicodeBigUnmarked", "UTF-16LE", "UnicodeLittleUnmarked", "US-ASCII", "ASCII", "TIS-620", "TIS620", "ISO-10646-UCS-2", "Unicode", "EBCDIC-CP-US", "cp037", "EBCDIC-CP-CA", "cp037", "EBCDIC-CP-NL", "cp037", "EBCDIC-CP-WT", "cp037", "EBCDIC-CP-DK", "cp277", "EBCDIC-CP-NO", "cp277", "EBCDIC-CP-FI", "cp278", "EBCDIC-CP-SE", "cp278", "EBCDIC-CP-IT", "cp280", "EBCDIC-CP-ES", "cp284", "EBCDIC-CP-GB", "cp285", "EBCDIC-CP-FR", "cp297", "EBCDIC-CP-AR1", "cp420", "EBCDIC-CP-HE", "cp424", "EBCDIC-CP-BE", "cp500", "EBCDIC-CP-CH", "cp500", "EBCDIC-CP-ROECE", "cp870", "EBCDIC-CP-YU", "cp870", "EBCDIC-CP-IS", "cp871", "EBCDIC-CP-AR2", "cp918" }

		''' <summary>
		''' Gets the corresponding Java encoding name from an IANA name.
		''' 
		''' This method is a helper method for the derived class to convert
		''' encoding names.
		''' </summary>
		''' <exception cref="UnsupportedEncodingException">
		'''      If this implementation couldn't find the Java encoding name. </exception>
		Protected Friend Overridable Function getJavaEncoding(ByVal encoding As String) As String
			Try
				"1".getBytes(encoding)
				Return encoding
			Catch e As java.io.UnsupportedEncodingException
				' try known alias
				For i As Integer = 0 To aliases.Length - 1 Step 2
					If encoding.Equals(aliases(i)) Then
						"1".getBytes(aliases(i+1))
						Return aliases(i+1)
					End If
				Next i

				Throw New java.io.UnsupportedEncodingException(encoding)
			End Try
	'         J2SE1.4 feature
	'        try {
	'            this.encoding = Charset.forName( _encoding );
	'        } catch( UnsupportedCharsetException uce ) {
	'            throw new JAXBException( uce );
	'        }
	'         
		End Function

		''' <summary>
		''' Default implementation of the setProperty method handles
		''' the four defined properties in Marshaller. If a provider
		''' needs to handle additional properties, it should override
		''' this method in a derived class.
		''' </summary>
		Public Overridable Sub setProperty(ByVal name As String, ByVal value As Object)

			If name Is Nothing Then Throw New System.ArgumentException(Messages.format(Messages.MUST_NOT_BE_NULL, "name"))

			' recognize and handle four pre-defined properties.
			If JAXB_ENCODING.Equals(name) Then
				checkString(name, value)
				encoding = CStr(value)
				Return
			End If
			If JAXB_FORMATTED_OUTPUT.Equals(name) Then
				checkBoolean(name, value)
				formattedOutput = CBool(value)
				Return
			End If
			If JAXB_NO_NAMESPACE_SCHEMA_LOCATION.Equals(name) Then
				checkString(name, value)
				noNSSchemaLocation = CStr(value)
				Return
			End If
			If JAXB_SCHEMA_LOCATION.Equals(name) Then
				checkString(name, value)
				schemaLocation = CStr(value)
				Return
			End If
			If JAXB_FRAGMENT.Equals(name) Then
				checkBoolean(name, value)
				fragment = CBool(value)
				Return
			End If

			Throw New javax.xml.bind.PropertyException(name, value)
		End Sub

		''' <summary>
		''' Default implementation of the getProperty method handles
		''' the four defined properties in Marshaller.  If a provider
		''' needs to support additional provider specific properties,
		''' it should override this method in a derived class.
		''' </summary>
		Public Overridable Function getProperty(ByVal name As String) As Object

			If name Is Nothing Then Throw New System.ArgumentException(Messages.format(Messages.MUST_NOT_BE_NULL, "name"))

			' recognize and handle four pre-defined properties.
			If JAXB_ENCODING.Equals(name) Then Return encoding
			If JAXB_FORMATTED_OUTPUT.Equals(name) Then Return If(formattedOutput, Boolean.TRUE, Boolean.FALSE)
			If JAXB_NO_NAMESPACE_SCHEMA_LOCATION.Equals(name) Then Return noNSSchemaLocation
			If JAXB_SCHEMA_LOCATION.Equals(name) Then Return schemaLocation
			If JAXB_FRAGMENT.Equals(name) Then Return If(fragment, Boolean.TRUE, Boolean.FALSE)

			Throw New javax.xml.bind.PropertyException(name)
		End Function
		''' <seealso cref= javax.xml.bind.Marshaller#getEventHandler() </seealso>
		Public Overridable Property eventHandler As javax.xml.bind.ValidationEventHandler
			Get
				Return eventHandler
			End Get
			Set(ByVal handler As javax.xml.bind.ValidationEventHandler)
    
				If handler Is Nothing Then
					eventHandler = New DefaultValidationEventHandler
				Else
					eventHandler = handler
				End If
			End Set
		End Property





	'    
	'     * assert that the given object is a Boolean
	'     
		Private Sub checkBoolean(ByVal name As String, ByVal value As Object)
			If Not(TypeOf value Is Boolean?) Then Throw New javax.xml.bind.PropertyException(Messages.format(Messages.MUST_BE_BOOLEAN, name))
		End Sub

	'    
	'     * assert that the given object is a String
	'     
		Private Sub checkString(ByVal name As String, ByVal value As Object)
			If Not(TypeOf value Is String) Then Throw New javax.xml.bind.PropertyException(Messages.format(Messages.MUST_BE_STRING, name))
		End Sub

	'    
	'     * assert that the parameters are not null
	'     
		Private Sub checkNotNull(ByVal o1 As Object, ByVal o1Name As String, ByVal o2 As Object, ByVal o2Name As String)

			If o1 Is Nothing Then Throw New System.ArgumentException(Messages.format(Messages.MUST_NOT_BE_NULL, o1Name))
			If o2 Is Nothing Then Throw New System.ArgumentException(Messages.format(Messages.MUST_NOT_BE_NULL, o2Name))
		End Sub

		Public Overridable Sub marshal(ByVal obj As Object, ByVal writer As javax.xml.stream.XMLEventWriter)

			Throw New System.NotSupportedException
		End Sub

		Public Overridable Sub marshal(ByVal obj As Object, ByVal writer As javax.xml.stream.XMLStreamWriter)

			Throw New System.NotSupportedException
		End Sub

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

		Public Overridable Property attachmentMarshaller As javax.xml.bind.attachment.AttachmentMarshaller
			Set(ByVal am As javax.xml.bind.attachment.AttachmentMarshaller)
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