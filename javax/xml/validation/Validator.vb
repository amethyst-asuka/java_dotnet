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

Namespace javax.xml.validation




	''' <summary>
	''' <p>A processor that checks an XML document against <seealso cref="Schema"/>.</p>
	''' 
	''' <p>
	''' A validator object is not thread-safe and not reentrant.
	''' In other words, it is the application's responsibility to make
	''' sure that one <seealso cref="Validator"/> object is not used from
	''' more than one thread at any given time, and while the <code>validate</code>
	''' method is invoked, applications may not recursively call
	''' the <code>validate</code> method.
	''' <p>
	''' 
	''' 
	''' @author  <a href="mailto:Kohsuke.Kawaguchi@Sun.com">Kohsuke Kawaguchi</a>
	''' @since 1.5
	''' </summary>
	Public MustInherit Class Validator

		''' <summary>
		''' Constructor for derived classes.
		''' 
		''' <p>The constructor does nothing.</p>
		''' 
		''' <p>Derived classes must create <seealso cref="Validator"/> objects that have
		''' <code>null</code> <seealso cref="ErrorHandler"/> and
		''' <code>null</code> <seealso cref="LSResourceResolver"/>.
		''' </p>
		''' </summary>
		Protected Friend Sub New()
		End Sub

			''' <summary>
			''' <p>Reset this <code>Validator</code> to its original configuration.</p>
			''' 
			''' <p><code>Validator</code> is reset to the same state as when it was created with
			''' <seealso cref="Schema#newValidator()"/>.
			''' <code>reset()</code> is designed to allow the reuse of existing <code>Validator</code>s
			''' thus saving resources associated with the creation of new <code>Validator</code>s.</p>
			''' 
			''' <p>The reset <code>Validator</code> is not guaranteed to have the same <seealso cref="LSResourceResolver"/> or <seealso cref="ErrorHandler"/>
			''' <code>Object</code>s, e.g. <seealso cref="Object#equals(Object obj)"/>.  It is guaranteed to have a functionally equal
			''' <code>LSResourceResolver</code> and <code>ErrorHandler</code>.</p>
			''' </summary>
			Public MustOverride Sub reset()

		''' <summary>
		''' Validates the specified input.
		''' 
		''' <p>This is just a convenience method for
		''' <seealso cref="#validate(Source source, Result result)"/>
		''' with <code>result</code> of <code>null</code>.</p>
		''' </summary>
		''' <param name="source">
		'''      XML to be validated. Must be an XML document or
		'''      XML element and must not be null. For backwards compatibility,
		'''      the results of attempting to validate anything other than
		'''      a document or element are implementation-dependent.
		'''      Implementations must either recognize and process the input
		'''      or throw an IllegalArgumentException.
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''      If the <code>Source</code>
		'''      is an XML artifact that the implementation cannot
		'''      validate (for example, a processing instruction).
		''' </exception>
		''' <exception cref="SAXException">
		'''      If the <seealso cref="ErrorHandler"/> throws a <seealso cref="SAXException"/> or
		'''      if a fatal error is found and the <seealso cref="ErrorHandler"/> returns
		'''      normally.
		''' </exception>
		''' <exception cref="IOException">
		'''      If the validator is processing a
		'''      <seealso cref="javax.xml.transform.sax.SAXSource"/> and the
		'''      underlying <seealso cref="org.xml.sax.XMLReader"/> throws an
		'''      <seealso cref="IOException"/>.
		''' 
		''' </exception>
		''' <exception cref="NullPointerException"> If <code>source</code> is
		'''   <code>null</code>.
		''' </exception>
		''' <seealso cref= #validate(Source source, Result result) </seealso>
		Public Overridable Sub validate(ByVal source As javax.xml.transform.Source)

			validate(source, Nothing)
		End Sub

		''' <summary>
		''' <p>Validates the specified input and send the augmented validation
		''' result to the specified output.</p>
		''' 
		''' <p>This method places the following restrictions on the types of
		''' the <seealso cref="Source"/>/<seealso cref="Result"/> accepted.</p>
		''' 
		''' <table border=1>
		''' <thead>
		'''  <tr>
		'''   <th colspan="5"><code>Source</code> / <code>Result</code> Accepted</th>
		'''  </tr>
		'''  <tr>
		'''   <th></th>
		'''   <th><seealso cref="javax.xml.transform.stream.StreamSource"/></th>
		'''   <th><seealso cref="javax.xml.transform.sax.SAXSource"/></th>
		'''   <th><seealso cref="javax.xml.transform.dom.DOMSource"/></th>
		'''   <th><seealso cref="javax.xml.transform.stax.StAXSource"/></th>
		'''  </tr>
		''' </thead>
		''' <tbody align="center">
		'''  <tr>
		'''   <td><code>null</code></td>
		'''   <td>OK</td>
		'''   <td>OK</td>
		'''   <td>OK</td>
		'''   <td>OK</td>
		'''  </tr>
		'''  <tr>
		'''   <th><seealso cref="javax.xml.transform.stream.StreamResult"/></th>
		'''   <td>OK</td>
		'''   <td><code>IllegalArgumentException</code></td>
		'''   <td><code>IllegalArgumentException</code></td>
		'''   <td><code>IllegalArgumentException</code></td>
		'''  </tr>
		'''  <tr>
		'''   <th><seealso cref="javax.xml.transform.sax.SAXResult"/></th>
		'''   <td><code>IllegalArgumentException</code></td>
		'''   <td>OK</td>
		'''   <td><code>IllegalArgumentException</code></td>
		'''   <td><code>IllegalArgumentException</code></td>
		'''  </tr>
		'''  <tr>
		'''   <th><seealso cref="javax.xml.transform.dom.DOMResult"/></th>
		'''   <td><code>IllegalArgumentException</code></td>
		'''   <td><code>IllegalArgumentException</code></td>
		'''   <td>OK</td>
		'''   <td><code>IllegalArgumentException</code></td>
		'''  </tr>
		'''  <tr>
		'''   <th><seealso cref="javax.xml.transform.stax.StAXResult"/></th>
		'''   <td><code>IllegalArgumentException</code></td>
		'''   <td><code>IllegalArgumentException</code></td>
		'''   <td><code>IllegalArgumentException</code></td>
		'''   <td>OK</td>
		'''  </tr>
		''' </tbody>
		''' </table>
		''' 
		''' <p>To validate one <code>Source</code> into another kind of
		''' <code>Result</code>, use the identity transformer (see
		''' <seealso cref="javax.xml.transform.TransformerFactory#newTransformer()"/>).</p>
		''' 
		''' <p>Errors found during the validation is sent to the specified
		''' <seealso cref="ErrorHandler"/>.</p>
		''' 
		''' <p>If a document is valid, or if a document contains some errors
		''' but none of them were fatal and the <code>ErrorHandler</code> didn't
		''' throw any exception, then the method returns normally.</p>
		''' </summary>
		''' <param name="source">
		'''      XML to be validated. Must be an XML document or
		'''      XML element and must not be null. For backwards compatibility,
		'''      the results of attempting to validate anything other than
		'''      a document or element are implementation-dependent.
		'''      Implementations must either recognize and process the input
		'''      or throw an IllegalArgumentException.
		''' </param>
		''' <param name="result">
		'''      The <code>Result</code> object that receives (possibly augmented)
		'''      XML. This parameter can be null if the caller is not interested
		'''      in it.
		''' 
		'''      Note that when a <code>DOMResult</code> is used,
		'''      a validator might just pass the same DOM node from
		'''      <code>DOMSource</code> to <code>DOMResult</code>
		'''      (in which case <code>source.getNode()==result.getNode()</code>),
		'''      it might copy the entire DOM tree, or it might alter the
		'''      node given by the source.
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''      If the <code>Result</code> type doesn't match the
		'''      <code>Source</code> type of if the <code>Source</code>
		'''      is an XML artifact that the implementation cannot
		'''      validate (for example, a processing instruction). </exception>
		''' <exception cref="SAXException">
		'''      If the <code>ErrorHandler</code> throws a
		'''      <code>SAXException</code> or
		'''      if a fatal error is found and the <code>ErrorHandler</code> returns
		'''      normally. </exception>
		''' <exception cref="IOException">
		'''      If the validator is processing a
		'''      <code>SAXSource</code> and the
		'''      underlying <seealso cref="org.xml.sax.XMLReader"/> throws an
		'''      <code>IOException</code>. </exception>
		''' <exception cref="NullPointerException">
		'''      If the <code>source</code> parameter is <code>null</code>.
		''' </exception>
		''' <seealso cref= #validate(Source source) </seealso>
		Public MustOverride Sub validate(ByVal source As javax.xml.transform.Source, ByVal result As javax.xml.transform.Result)

		''' <summary>
		''' Sets the <seealso cref="ErrorHandler"/> to receive errors encountered
		''' during the <code>validate</code> method invocation.
		''' 
		''' <p>
		''' Error handler can be used to customize the error handling process
		''' during a validation. When an <seealso cref="ErrorHandler"/> is set,
		''' errors found during the validation will be first sent
		''' to the <seealso cref="ErrorHandler"/>.
		''' 
		''' <p>
		''' The error handler can abort further validation immediately
		''' by throwing <seealso cref="SAXException"/> from the handler. Or for example
		''' it can print an error to the screen and try to continue the
		''' validation by returning normally from the <seealso cref="ErrorHandler"/>
		''' 
		''' <p>
		''' If any <seealso cref="Throwable"/> is thrown from an <seealso cref="ErrorHandler"/>,
		''' the caller of the <code>validate</code> method will be thrown
		''' the same <seealso cref="Throwable"/> object.
		''' 
		''' <p>
		''' <seealso cref="Validator"/> is not allowed to
		''' throw <seealso cref="SAXException"/> without first reporting it to
		''' <seealso cref="ErrorHandler"/>.
		''' 
		''' <p>
		''' When the <seealso cref="ErrorHandler"/> is null, the implementation will
		''' behave as if the following <seealso cref="ErrorHandler"/> is set:
		''' <pre>
		''' class DraconianErrorHandler implements <seealso cref="ErrorHandler"/> {
		'''     public void fatalError( <seealso cref="org.xml.sax.SAXParseException"/> e ) throws <seealso cref="SAXException"/> {
		'''         throw e;
		'''     }
		'''     public void error( <seealso cref="org.xml.sax.SAXParseException"/> e ) throws <seealso cref="SAXException"/> {
		'''         throw e;
		'''     }
		'''     public void warning( <seealso cref="org.xml.sax.SAXParseException"/> e ) throws <seealso cref="SAXException"/> {
		'''         // noop
		'''     }
		''' }
		''' </pre>
		''' 
		''' <p>
		''' When a new <seealso cref="Validator"/> object is created, initially
		''' this field is set to null.
		''' </summary>
		''' <param name="errorHandler">
		'''      A new error handler to be set. This parameter can be null. </param>
		Public MustOverride Property errorHandler As org.xml.sax.ErrorHandler


		''' <summary>
		''' Sets the <seealso cref="LSResourceResolver"/> to customize
		''' resource resolution while in a validation episode.
		''' 
		''' <p>
		''' <seealso cref="Validator"/> uses a <seealso cref="LSResourceResolver"/>
		''' when it needs to locate external resources while a validation,
		''' although exactly what constitutes "locating external resources" is
		''' up to each schema language.
		''' 
		''' <p>
		''' When the <seealso cref="LSResourceResolver"/> is null, the implementation will
		''' behave as if the following <seealso cref="LSResourceResolver"/> is set:
		''' <pre>
		''' class DumbLSResourceResolver implements <seealso cref="LSResourceResolver"/> {
		'''     public <seealso cref="org.w3c.dom.ls.LSInput"/> resolveResource(
		'''         String publicId, String systemId, String baseURI) {
		''' 
		'''         return null; // always return null
		'''     }
		''' }
		''' </pre>
		''' 
		''' <p>
		''' If a <seealso cref="LSResourceResolver"/> throws a <seealso cref="RuntimeException"/>
		'''  (or instances of its derived classes),
		''' then the <seealso cref="Validator"/> will abort the parsing and
		''' the caller of the <code>validate</code> method will receive
		''' the same <seealso cref="RuntimeException"/>.
		''' 
		''' <p>
		''' When a new <seealso cref="Validator"/> object is created, initially
		''' this field is set to null.
		''' </summary>
		''' <param name="resourceResolver">
		'''      A new resource resolver to be set. This parameter can be null. </param>
		Public MustOverride Property resourceResolver As org.w3c.dom.ls.LSResourceResolver




		''' <summary>
		''' Look up the value of a feature flag.
		''' 
		''' <p>The feature name is any fully-qualified URI.  It is
		''' possible for a <seealso cref="Validator"/> to recognize a feature name but
		''' temporarily be unable to return its value.
		''' Some feature values may be available only in specific
		''' contexts, such as before, during, or after a validation.
		''' 
		''' <p>Implementors are free (and encouraged) to invent their own features,
		''' using names built on their own URIs.</p>
		''' </summary>
		''' <param name="name"> The feature name, which is a non-null fully-qualified URI.
		''' </param>
		''' <returns> The current value of the feature (true or false).
		''' </returns>
		''' <exception cref="SAXNotRecognizedException"> If the feature
		'''   value can't be assigned or retrieved. </exception>
		''' <exception cref="SAXNotSupportedException"> When the
		'''   <seealso cref="Validator"/> recognizes the feature name but
		'''   cannot determine its value at this time. </exception>
		''' <exception cref="NullPointerException">
		'''   When the name parameter is null.
		''' </exception>
		''' <seealso cref= #setFeature(String, boolean) </seealso>
		Public Overridable Function getFeature(ByVal name As String) As Boolean

			If name Is Nothing Then Throw New NullPointerException("the name parameter is null")

			Throw New org.xml.sax.SAXNotRecognizedException(name)
		End Function

		''' <summary>
		''' Set the value of a feature flag.
		''' 
		''' <p>
		''' Feature can be used to control the way a <seealso cref="Validator"/>
		''' parses schemas, although <seealso cref="Validator"/>s are not required
		''' to recognize any specific feature names.</p>
		''' 
		''' <p>The feature name is any fully-qualified URI.  It is
		''' possible for a <seealso cref="Validator"/> to expose a feature value but
		''' to be unable to change the current value.
		''' Some feature values may be immutable or mutable only
		''' in specific contexts, such as before, during, or after
		''' a validation.</p>
		''' </summary>
		''' <param name="name"> The feature name, which is a non-null fully-qualified URI. </param>
		''' <param name="value"> The requested value of the feature (true or false).
		''' </param>
		''' <exception cref="SAXNotRecognizedException"> If the feature
		'''   value can't be assigned or retrieved. </exception>
		''' <exception cref="SAXNotSupportedException"> When the
		'''   <seealso cref="Validator"/> recognizes the feature name but
		'''   cannot set the requested value. </exception>
		''' <exception cref="NullPointerException">
		'''   When the name parameter is null.
		''' </exception>
		''' <seealso cref= #getFeature(String) </seealso>
		Public Overridable Sub setFeature(ByVal name As String, ByVal value As Boolean)

			If name Is Nothing Then Throw New NullPointerException("the name parameter is null")

			Throw New org.xml.sax.SAXNotRecognizedException(name)
		End Sub

		''' <summary>
		''' Set the value of a property.
		''' 
		''' <p>The property name is any fully-qualified URI.  It is
		''' possible for a <seealso cref="Validator"/> to recognize a property name but
		''' to be unable to change the current value.
		''' Some property values may be immutable or mutable only
		''' in specific contexts, such as before, during, or after
		''' a validation.</p>
		''' 
		''' <p>
		''' All implementations that implement JAXP 1.5 or newer are required to
		''' support the <seealso cref="javax.xml.XMLConstants#ACCESS_EXTERNAL_DTD"/> and
		''' <seealso cref="javax.xml.XMLConstants#ACCESS_EXTERNAL_SCHEMA"/> properties.
		''' </p>
		''' <ul>
		'''   <li>
		'''      <p>Access to external DTDs in source or Schema file is restricted to
		'''      the protocols specified by the <seealso cref="javax.xml.XMLConstants#ACCESS_EXTERNAL_DTD"/>
		'''      property.  If access is denied during validation due to the restriction
		'''      of this property, <seealso cref="org.xml.sax.SAXException"/> will be thrown by the
		'''      <seealso cref="#validate(Source)"/> method.</p>
		''' 
		'''      <p>Access to external reference set by the schemaLocation attribute is
		'''      restricted to the protocols specified by the
		'''      <seealso cref="javax.xml.XMLConstants#ACCESS_EXTERNAL_SCHEMA"/> property.
		'''      If access is denied during validation due to the restriction of this property,
		'''      <seealso cref="org.xml.sax.SAXException"/> will be thrown by the
		'''      <seealso cref="#validate(Source)"/> method.</p>
		'''   </li>
		''' </ul>
		''' </summary>
		''' <param name="name"> The property name, which is a non-null fully-qualified URI. </param>
		''' <param name="object"> The requested value for the property.
		''' </param>
		''' <exception cref="SAXNotRecognizedException"> If the property
		'''   value can't be assigned or retrieved. </exception>
		''' <exception cref="SAXNotSupportedException"> When the
		'''   <seealso cref="Validator"/> recognizes the property name but
		'''   cannot set the requested value. </exception>
		''' <exception cref="NullPointerException">
		'''   When the name parameter is null. </exception>
		Public Overridable Sub setProperty(ByVal name As String, ByVal [object] As Object)

			If name Is Nothing Then Throw New NullPointerException("the name parameter is null")

			Throw New org.xml.sax.SAXNotRecognizedException(name)
		End Sub

		''' <summary>
		''' Look up the value of a property.
		''' 
		''' <p>The property name is any fully-qualified URI.  It is
		''' possible for a <seealso cref="Validator"/> to recognize a property name but
		''' temporarily be unable to return its value.
		''' Some property values may be available only in specific
		''' contexts, such as before, during, or after a validation.</p>
		''' 
		''' <p><seealso cref="Validator"/>s are not required to recognize any specific
		''' property names.</p>
		''' 
		''' <p>Implementors are free (and encouraged) to invent their own properties,
		''' using names built on their own URIs.</p>
		''' </summary>
		''' <param name="name"> The property name, which is a non-null fully-qualified URI.
		''' </param>
		''' <returns> The current value of the property.
		''' </returns>
		''' <exception cref="SAXNotRecognizedException"> If the property
		'''   value can't be assigned or retrieved. </exception>
		''' <exception cref="SAXNotSupportedException"> When the
		'''   XMLReader recognizes the property name but
		'''   cannot determine its value at this time. </exception>
		''' <exception cref="NullPointerException">
		'''   When the name parameter is null.
		''' </exception>
		''' <seealso cref= #setProperty(String, Object) </seealso>
		Public Overridable Function getProperty(ByVal name As String) As Object

			If name Is Nothing Then Throw New NullPointerException("the name parameter is null")

			Throw New org.xml.sax.SAXNotRecognizedException(name)
		End Function
	End Class

End Namespace