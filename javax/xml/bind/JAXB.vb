Imports System

'
' * Copyright (c) 2006, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.bind


	''' <summary>
	''' Class that defines convenience methods for common, simple use of JAXB.
	''' 
	''' <p>
	''' Methods defined in this class are convenience methods that combine several basic operations
	''' in the <seealso cref="JAXBContext"/>, <seealso cref="Unmarshaller"/>, and <seealso cref="Marshaller"/>.
	''' 
	''' They are designed
	''' to be the prefered methods for developers new to JAXB. They have
	''' the following characterstics:
	''' 
	''' <ol>
	'''  <li>Generally speaking, the performance is not necessarily optimal.
	'''      It is expected that people who need to write performance
	'''      critical code will use the rest of the JAXB API directly.
	'''  <li>Errors that happen during the processing is wrapped into
	'''      <seealso cref="DataBindingException"/> (which will have <seealso cref="JAXBException"/>
	'''      as its <seealso cref="Throwable#getCause() cause"/>. It is expected that
	'''      people who prefer the checked exception would use
	'''      the rest of the JAXB API directly.
	''' </ol>
	''' 
	''' <p>
	''' In addition, the <tt>unmarshal</tt> methods have the following characteristic:
	''' 
	''' <ol>
	'''  <li>Schema validation is not performed on the input XML.
	'''      The processing will try to continue even if there
	'''      are errors in the XML, as much as possible. Only as
	'''      the last resort, this method fails with <seealso cref="DataBindingException"/>.
	''' </ol>
	''' 
	''' <p>
	''' Similarly, the <tt>marshal</tt> methods have the following characteristic:
	''' <ol>
	'''  <li>The processing will try to continue even if the Java object tree
	'''      does not meet the validity requirement. Only as
	'''      the last resort, this method fails with <seealso cref="DataBindingException"/>.
	''' </ol>
	''' 
	''' 
	''' <p>
	''' All the methods on this class require non-null arguments to all parameters.
	''' The <tt>unmarshal</tt> methods either fail with an exception or return
	''' a non-null value.
	''' 
	''' @author Kohsuke Kawaguchi
	''' @since 2.1
	''' </summary>
	Public NotInheritable Class JAXB
		''' <summary>
		''' No instanciation is allowed.
		''' </summary>
		Private Sub New()
		End Sub

		''' <summary>
		''' To improve the performance, we'll cache the last <seealso cref="JAXBContext"/> used.
		''' </summary>
		Private NotInheritable Class Cache
			Friend ReadOnly type As Type
			Friend ReadOnly context As JAXBContext

			Public Sub New(ByVal type As Type)
				Me.type = type
				Me.context = JAXBContext.newInstance(type)
			End Sub
		End Class

		''' <summary>
		''' Cache. We don't want to prevent the <seealso cref="Cache#type"/> from GC-ed,
		''' hence <seealso cref="WeakReference"/>.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private Shared cache As WeakReference(Of Cache)

		''' <summary>
		''' Obtains the <seealso cref="JAXBContext"/> from the given type,
		''' by using the cache if possible.
		''' 
		''' <p>
		''' We don't use locks to control access to <seealso cref="#cache"/>, but this code
		''' should be thread-safe thanks to the immutable <seealso cref="Cache"/> and {@code volatile}.
		''' </summary>
		Private Shared Function getContext(Of T)(ByVal type As Type) As JAXBContext
			Dim c As WeakReference(Of Cache) = cache
			If c IsNot Nothing Then
				Dim d As Cache = c.get()
				If d IsNot Nothing AndAlso d.type Is type Then Return d.context
			End If

			' overwrite the cache
			Dim d As New Cache(type)
			cache = New WeakReference(Of Cache)(d)

			Return d.context
		End Function

		''' <summary>
		''' Reads in a Java object tree from the given XML input.
		''' </summary>
		''' <param name="xml">
		'''      Reads the entire file as XML. </param>
		Public Shared Function unmarshal(Of T)(ByVal xml As java.io.File, ByVal type As Type) As T
			Try
				Dim item As JAXBElement(Of T) = getContext(type).createUnmarshaller().unmarshal(New javax.xml.transform.stream.StreamSource(xml), type)
				Return item.value
			Catch e As JAXBException
				Throw New DataBindingException(e)
			End Try
		End Function

		''' <summary>
		''' Reads in a Java object tree from the given XML input.
		''' </summary>
		''' <param name="xml">
		'''      The resource pointed by the URL is read in its entirety. </param>
		Public Shared Function unmarshal(Of T)(ByVal xml As java.net.URL, ByVal type As Type) As T
			Try
				Dim item As JAXBElement(Of T) = getContext(type).createUnmarshaller().unmarshal(toSource(xml), type)
				Return item.value
			Catch e As JAXBException
				Throw New DataBindingException(e)
			Catch e As java.io.IOException
				Throw New DataBindingException(e)
			End Try
		End Function

		''' <summary>
		''' Reads in a Java object tree from the given XML input.
		''' </summary>
		''' <param name="xml">
		'''      The URI is <seealso cref="URI#toURL() turned into URL"/> and then
		'''      follows the handling of <tt>URL</tt>. </param>
		Public Shared Function unmarshal(Of T)(ByVal xml As java.net.URI, ByVal type As Type) As T
			Try
				Dim item As JAXBElement(Of T) = getContext(type).createUnmarshaller().unmarshal(toSource(xml), type)
				Return item.value
			Catch e As JAXBException
				Throw New DataBindingException(e)
			Catch e As java.io.IOException
				Throw New DataBindingException(e)
			End Try
		End Function

		''' <summary>
		''' Reads in a Java object tree from the given XML input.
		''' </summary>
		''' <param name="xml">
		'''      The string is first interpreted as an absolute <tt>URI</tt>.
		'''      If it's not <seealso cref="URI#isAbsolute() a valid absolute URI"/>,
		'''      then it's interpreted as a <tt>File</tt> </param>
		Public Shared Function unmarshal(Of T)(ByVal xml As String, ByVal type As Type) As T
			Try
				Dim item As JAXBElement(Of T) = getContext(type).createUnmarshaller().unmarshal(toSource(xml), type)
				Return item.value
			Catch e As JAXBException
				Throw New DataBindingException(e)
			Catch e As java.io.IOException
				Throw New DataBindingException(e)
			End Try
		End Function

		''' <summary>
		''' Reads in a Java object tree from the given XML input.
		''' </summary>
		''' <param name="xml">
		'''      The entire stream is read as an XML infoset.
		'''      Upon a successful completion, the stream will be closed by this method. </param>
		Public Shared Function unmarshal(Of T)(ByVal xml As java.io.InputStream, ByVal type As Type) As T
			Try
				Dim item As JAXBElement(Of T) = getContext(type).createUnmarshaller().unmarshal(toSource(xml), type)
				Return item.value
			Catch e As JAXBException
				Throw New DataBindingException(e)
			Catch e As java.io.IOException
				Throw New DataBindingException(e)
			End Try
		End Function

		''' <summary>
		''' Reads in a Java object tree from the given XML input.
		''' </summary>
		''' <param name="xml">
		'''      The character stream is read as an XML infoset.
		'''      The encoding declaration in the XML will be ignored.
		'''      Upon a successful completion, the stream will be closed by this method. </param>
		Public Shared Function unmarshal(Of T)(ByVal xml As java.io.Reader, ByVal type As Type) As T
			Try
				Dim item As JAXBElement(Of T) = getContext(type).createUnmarshaller().unmarshal(toSource(xml), type)
				Return item.value
			Catch e As JAXBException
				Throw New DataBindingException(e)
			Catch e As java.io.IOException
				Throw New DataBindingException(e)
			End Try
		End Function

		''' <summary>
		''' Reads in a Java object tree from the given XML input.
		''' </summary>
		''' <param name="xml">
		'''      The XML infoset that the <seealso cref="Source"/> represents is read. </param>
		Public Shared Function unmarshal(Of T)(ByVal xml As javax.xml.transform.Source, ByVal type As Type) As T
			Try
				Dim item As JAXBElement(Of T) = getContext(type).createUnmarshaller().unmarshal(toSource(xml), type)
				Return item.value
			Catch e As JAXBException
				Throw New DataBindingException(e)
			Catch e As java.io.IOException
				Throw New DataBindingException(e)
			End Try
		End Function



		''' <summary>
		''' Creates <seealso cref="Source"/> from various XML representation.
		''' See <seealso cref="#unmarshal"/> for the conversion rules.
		''' </summary>
		Private Shared Function toSource(ByVal xml As Object) As javax.xml.transform.Source
			If xml Is Nothing Then Throw New System.ArgumentException("no XML is given")

			If TypeOf xml Is String Then
				Try
					xml = New java.net.URI(CStr(xml))
				Catch e As java.net.URISyntaxException
					xml = New File(CStr(xml))
				End Try
			End If
			If TypeOf xml Is File Then
				Dim file As File = CType(xml, File)
				Return New javax.xml.transform.stream.StreamSource(file)
			End If
			If TypeOf xml Is java.net.URI Then
				Dim uri As java.net.URI = CType(xml, java.net.URI)
				xml=uri.toURL()
			End If
			If TypeOf xml Is java.net.URL Then
				Dim url As java.net.URL = CType(xml, java.net.URL)
				Return New javax.xml.transform.stream.StreamSource(url.toExternalForm())
			End If
			If TypeOf xml Is java.io.InputStream Then
				Dim [in] As java.io.InputStream = CType(xml, java.io.InputStream)
				Return New javax.xml.transform.stream.StreamSource([in])
			End If
			If TypeOf xml Is java.io.Reader Then
				Dim r As java.io.Reader = CType(xml, java.io.Reader)
				Return New javax.xml.transform.stream.StreamSource(r)
			End If
			If TypeOf xml Is javax.xml.transform.Source Then Return CType(xml, javax.xml.transform.Source)
			Throw New System.ArgumentException("I don't understand how to handle " & xml.GetType())
		End Function

		''' <summary>
		''' Writes a Java object tree to XML and store it to the specified location.
		''' </summary>
		''' <param name="jaxbObject">
		'''      The Java object to be marshalled into XML. If this object is
		'''      a <seealso cref="JAXBElement"/>, it will provide the root tag name and
		'''      the body. If this object has <seealso cref="XmlRootElement"/>
		'''      on its class definition, that will be used as the root tag name
		'''      and the given object will provide the body. Otherwise,
		'''      the root tag name is <seealso cref="Introspector#decapitalize(String) infered"/> from
		'''      <seealso cref="Class#getSimpleName() the short class name"/>.
		'''      This parameter must not be null.
		''' </param>
		''' <param name="xml">
		'''      XML will be written to this file. If it already exists,
		'''      it will be overwritten.
		''' </param>
		''' <exception cref="DataBindingException">
		'''      If the operation fails, such as due to I/O error, unbindable classes. </exception>
		Public Shared Sub marshal(ByVal jaxbObject As Object, ByVal xml As java.io.File)
			_marshal(jaxbObject,xml)
		End Sub

		''' <summary>
		''' Writes a Java object tree to XML and store it to the specified location.
		''' </summary>
		''' <param name="jaxbObject">
		'''      The Java object to be marshalled into XML. If this object is
		'''      a <seealso cref="JAXBElement"/>, it will provide the root tag name and
		'''      the body. If this object has <seealso cref="XmlRootElement"/>
		'''      on its class definition, that will be used as the root tag name
		'''      and the given object will provide the body. Otherwise,
		'''      the root tag name is <seealso cref="Introspector#decapitalize(String) infered"/> from
		'''      <seealso cref="Class#getSimpleName() the short class name"/>.
		'''      This parameter must not be null.
		''' </param>
		''' <param name="xml">
		'''      The XML will be <seealso cref="URLConnection#getOutputStream() sent"/> to the
		'''      resource pointed by this URL. Note that not all <tt>URL</tt>s support
		'''      such operation, and exact semantics depends on the <tt>URL</tt>
		'''      implementations. In case of <seealso cref="HttpURLConnection HTTP URLs"/>,
		'''      this will perform HTTP POST.
		''' </param>
		''' <exception cref="DataBindingException">
		'''      If the operation fails, such as due to I/O error, unbindable classes. </exception>
		Public Shared Sub marshal(ByVal jaxbObject As Object, ByVal xml As java.net.URL)
			_marshal(jaxbObject,xml)
		End Sub

		''' <summary>
		''' Writes a Java object tree to XML and store it to the specified location.
		''' </summary>
		''' <param name="jaxbObject">
		'''      The Java object to be marshalled into XML. If this object is
		'''      a <seealso cref="JAXBElement"/>, it will provide the root tag name and
		'''      the body. If this object has <seealso cref="XmlRootElement"/>
		'''      on its class definition, that will be used as the root tag name
		'''      and the given object will provide the body. Otherwise,
		'''      the root tag name is <seealso cref="Introspector#decapitalize(String) infered"/> from
		'''      <seealso cref="Class#getSimpleName() the short class name"/>.
		'''      This parameter must not be null.
		''' </param>
		''' <param name="xml">
		'''      The URI is <seealso cref="URI#toURL() turned into URL"/> and then
		'''      follows the handling of <tt>URL</tt>. See above.
		''' </param>
		''' <exception cref="DataBindingException">
		'''      If the operation fails, such as due to I/O error, unbindable classes. </exception>
		Public Shared Sub marshal(ByVal jaxbObject As Object, ByVal xml As java.net.URI)
			_marshal(jaxbObject,xml)
		End Sub

		''' <summary>
		''' Writes a Java object tree to XML and store it to the specified location.
		''' </summary>
		''' <param name="jaxbObject">
		'''      The Java object to be marshalled into XML. If this object is
		'''      a <seealso cref="JAXBElement"/>, it will provide the root tag name and
		'''      the body. If this object has <seealso cref="XmlRootElement"/>
		'''      on its class definition, that will be used as the root tag name
		'''      and the given object will provide the body. Otherwise,
		'''      the root tag name is <seealso cref="Introspector#decapitalize(String) infered"/> from
		'''      <seealso cref="Class#getSimpleName() the short class name"/>.
		'''      This parameter must not be null.
		''' </param>
		''' <param name="xml">
		'''      The string is first interpreted as an absolute <tt>URI</tt>.
		'''      If it's not <seealso cref="URI#isAbsolute() a valid absolute URI"/>,
		'''      then it's interpreted as a <tt>File</tt>
		''' </param>
		''' <exception cref="DataBindingException">
		'''      If the operation fails, such as due to I/O error, unbindable classes. </exception>
		Public Shared Sub marshal(ByVal jaxbObject As Object, ByVal xml As String)
			_marshal(jaxbObject,xml)
		End Sub

		''' <summary>
		''' Writes a Java object tree to XML and store it to the specified location.
		''' </summary>
		''' <param name="jaxbObject">
		'''      The Java object to be marshalled into XML. If this object is
		'''      a <seealso cref="JAXBElement"/>, it will provide the root tag name and
		'''      the body. If this object has <seealso cref="XmlRootElement"/>
		'''      on its class definition, that will be used as the root tag name
		'''      and the given object will provide the body. Otherwise,
		'''      the root tag name is <seealso cref="Introspector#decapitalize(String) infered"/> from
		'''      <seealso cref="Class#getSimpleName() the short class name"/>.
		'''      This parameter must not be null.
		''' </param>
		''' <param name="xml">
		'''      The XML will be sent to the given <seealso cref="OutputStream"/>.
		'''      Upon a successful completion, the stream will be closed by this method.
		''' </param>
		''' <exception cref="DataBindingException">
		'''      If the operation fails, such as due to I/O error, unbindable classes. </exception>
		Public Shared Sub marshal(ByVal jaxbObject As Object, ByVal xml As java.io.OutputStream)
			_marshal(jaxbObject,xml)
		End Sub

		''' <summary>
		''' Writes a Java object tree to XML and store it to the specified location.
		''' </summary>
		''' <param name="jaxbObject">
		'''      The Java object to be marshalled into XML. If this object is
		'''      a <seealso cref="JAXBElement"/>, it will provide the root tag name and
		'''      the body. If this object has <seealso cref="XmlRootElement"/>
		'''      on its class definition, that will be used as the root tag name
		'''      and the given object will provide the body. Otherwise,
		'''      the root tag name is <seealso cref="Introspector#decapitalize(String) infered"/> from
		'''      <seealso cref="Class#getSimpleName() the short class name"/>.
		'''      This parameter must not be null.
		''' </param>
		''' <param name="xml">
		'''      The XML will be sent as a character stream to the given <seealso cref="Writer"/>.
		'''      Upon a successful completion, the stream will be closed by this method.
		''' </param>
		''' <exception cref="DataBindingException">
		'''      If the operation fails, such as due to I/O error, unbindable classes. </exception>
		Public Shared Sub marshal(ByVal jaxbObject As Object, ByVal xml As java.io.Writer)
			_marshal(jaxbObject,xml)
		End Sub

		''' <summary>
		''' Writes a Java object tree to XML and store it to the specified location.
		''' </summary>
		''' <param name="jaxbObject">
		'''      The Java object to be marshalled into XML. If this object is
		'''      a <seealso cref="JAXBElement"/>, it will provide the root tag name and
		'''      the body. If this object has <seealso cref="XmlRootElement"/>
		'''      on its class definition, that will be used as the root tag name
		'''      and the given object will provide the body. Otherwise,
		'''      the root tag name is <seealso cref="Introspector#decapitalize(String) infered"/> from
		'''      <seealso cref="Class#getSimpleName() the short class name"/>.
		'''      This parameter must not be null.
		''' </param>
		''' <param name="xml">
		'''      The XML will be sent to the <seealso cref="Result"/> object.
		''' </param>
		''' <exception cref="DataBindingException">
		'''      If the operation fails, such as due to I/O error, unbindable classes. </exception>
		Public Shared Sub marshal(ByVal jaxbObject As Object, ByVal xml As javax.xml.transform.Result)
			_marshal(jaxbObject,xml)
		End Sub

		''' <summary>
		''' Writes a Java object tree to XML and store it to the specified location.
		''' 
		''' <p>
		''' This method is a convenience method that combines several basic operations
		''' in the <seealso cref="JAXBContext"/> and <seealso cref="Marshaller"/>. This method is designed
		''' to be the prefered method for developers new to JAXB. This method
		''' has the following characterstics:
		''' 
		''' <ol>
		'''  <li>Generally speaking, the performance is not necessarily optimal.
		'''      It is expected that those people who need to write performance
		'''      critical code will use the rest of the JAXB API directly.
		'''  <li>Errors that happen during the processing is wrapped into
		'''      <seealso cref="DataBindingException"/> (which will have <seealso cref="JAXBException"/>
		'''      as its <seealso cref="Throwable#getCause() cause"/>. It is expected that
		'''      those people who prefer the checked exception would use
		'''      the rest of the JAXB API directly.
		''' </ol>
		''' </summary>
		''' <param name="jaxbObject">
		'''      The Java object to be marshalled into XML. If this object is
		'''      a <seealso cref="JAXBElement"/>, it will provide the root tag name and
		'''      the body. If this object has <seealso cref="XmlRootElement"/>
		'''      on its class definition, that will be used as the root tag name
		'''      and the given object will provide the body. Otherwise,
		'''      the root tag name is <seealso cref="Introspector#decapitalize(String) infered"/> from
		'''      <seealso cref="Class#getSimpleName() the short class name"/>.
		'''      This parameter must not be null.
		''' </param>
		''' <param name="xml">
		'''      Represents the receiver of XML. Objects of the following types are allowed.
		''' 
		'''      <table><tr>
		'''          <th>Type</th>
		'''          <th>Operation</th>
		'''      </tr><tr>
		'''          <td><seealso cref="File"/></td>
		'''          <td>XML will be written to this file. If it already exists,
		'''              it will be overwritten.</td>
		'''      </tr><tr>
		'''          <td><seealso cref="URL"/></td>
		'''          <td>The XML will be <seealso cref="URLConnection#getOutputStream() sent"/> to the
		'''              resource pointed by this URL. Note that not all <tt>URL</tt>s support
		'''              such operation, and exact semantics depends on the <tt>URL</tt>
		'''              implementations. In case of <seealso cref="HttpURLConnection HTTP URLs"/>,
		'''              this will perform HTTP POST.</td>
		'''      </tr><tr>
		'''          <td><seealso cref="URI"/></td>
		'''          <td>The URI is <seealso cref="URI#toURL() turned into URL"/> and then
		'''              follows the handling of <tt>URL</tt>. See above.</td>
		'''      </tr><tr>
		'''          <td><seealso cref="String"/></td>
		'''          <td>The string is first interpreted as an absolute <tt>URI</tt>.
		'''              If it's not <seealso cref="URI#isAbsolute() a valid absolute URI"/>,
		'''              then it's interpreted as a <tt>File</tt></td>
		'''      </tr><tr>
		'''          <td><seealso cref="OutputStream"/></td>
		'''          <td>The XML will be sent to the given <seealso cref="OutputStream"/>.
		'''              Upon a successful completion, the stream will be closed by this method.</td>
		'''      </tr><tr>
		'''          <td><seealso cref="Writer"/></td>
		'''          <td>The XML will be sent as a character stream to the given <seealso cref="Writer"/>.
		'''              Upon a successful completion, the stream will be closed by this method.</td>
		'''      </tr><tr>
		'''          <td><seealso cref="Result"/></td>
		'''          <td>The XML will be sent to the <seealso cref="Result"/> object.</td>
		'''      </tr></table>
		''' </param>
		''' <exception cref="DataBindingException">
		'''      If the operation fails, such as due to I/O error, unbindable classes. </exception>
		Private Shared Sub _marshal(ByVal jaxbObject As Object, ByVal xml As Object)
			Try
				Dim ___context As JAXBContext

				If TypeOf jaxbObject Is JAXBElement Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					___context = getContext(CType(jaxbObject, JAXBElement(Of ?)).declaredType)
				Else
					Dim clazz As Type = jaxbObject.GetType()
					Dim r As javax.xml.bind.annotation.XmlRootElement = clazz.getAnnotation(GetType(javax.xml.bind.annotation.XmlRootElement))
					___context = getContext(clazz)
					If r Is Nothing Then jaxbObject = New JAXBElement(New javax.xml.namespace.QName(inferName(clazz)),clazz,jaxbObject)
				End If

				Dim m As Marshaller = ___context.createMarshaller()
				m.propertyrty(Marshaller.JAXB_FORMATTED_OUTPUT,True)
				m.marshal(jaxbObject, toResult(xml))
			Catch e As JAXBException
				Throw New DataBindingException(e)
			Catch e As java.io.IOException
				Throw New DataBindingException(e)
			End Try
		End Sub

		Private Shared Function inferName(ByVal clazz As Type) As String
			Return java.beans.Introspector.decapitalize(clazz.Name)
		End Function

		''' <summary>
		''' Creates <seealso cref="Result"/> from various XML representation.
		''' See <seealso cref="#_marshal(Object,Object)"/> for the conversion rules.
		''' </summary>
		Private Shared Function toResult(ByVal xml As Object) As javax.xml.transform.Result
			If xml Is Nothing Then Throw New System.ArgumentException("no XML is given")

			If TypeOf xml Is String Then
				Try
					xml = New java.net.URI(CStr(xml))
				Catch e As java.net.URISyntaxException
					xml = New File(CStr(xml))
				End Try
			End If
			If TypeOf xml Is File Then
				Dim file As File = CType(xml, File)
				Return New javax.xml.transform.stream.StreamResult(file)
			End If
			If TypeOf xml Is java.net.URI Then
				Dim uri As java.net.URI = CType(xml, java.net.URI)
				xml=uri.toURL()
			End If
			If TypeOf xml Is java.net.URL Then
				Dim url As java.net.URL = CType(xml, java.net.URL)
				Dim con As java.net.URLConnection = url.openConnection()
				con.doOutput = True
				con.doInput = False
				con.connect()
				Return New javax.xml.transform.stream.StreamResult(con.outputStream)
			End If
			If TypeOf xml Is java.io.OutputStream Then
				Dim os As java.io.OutputStream = CType(xml, java.io.OutputStream)
				Return New javax.xml.transform.stream.StreamResult(os)
			End If
			If TypeOf xml Is java.io.Writer Then
				Dim w As java.io.Writer = CType(xml, java.io.Writer)
				Return New javax.xml.transform.stream.StreamResult(w)
			End If
			If TypeOf xml Is javax.xml.transform.Result Then Return CType(xml, javax.xml.transform.Result)
			Throw New System.ArgumentException("I don't understand how to handle " & xml.GetType())
		End Function

	End Class

End Namespace