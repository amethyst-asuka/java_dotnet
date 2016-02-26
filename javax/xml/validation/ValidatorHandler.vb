'
' * Copyright (c) 2003, 2005, Oracle and/or its affiliates. All rights reserved.
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
	''' Streaming validator that works on SAX stream.
	''' 
	''' <p>
	''' A <seealso cref="ValidatorHandler"/> object is not thread-safe and not reentrant.
	''' In other words, it is the application's responsibility to make
	''' sure that one <seealso cref="ValidatorHandler"/> object is not used from
	''' more than one thread at any given time.
	''' 
	''' <p>
	''' <seealso cref="ValidatorHandler"/> checks if the SAX events follow
	''' the set of constraints described in the associated <seealso cref="Schema"/>,
	''' and additionally it may modify the SAX events (for example
	''' by adding default values, etc.)
	''' 
	''' <p>
	''' <seealso cref="ValidatorHandler"/> extends from <seealso cref="ContentHandler"/>,
	''' but it refines the underlying <seealso cref="ContentHandler"/> in
	''' the following way:
	''' <ol>
	'''  <li>startElement/endElement events must receive non-null String
	'''      for <code>uri</code>, <code>localName</code>, and <code>qname</code>,
	'''      even though SAX allows some of them to be null.
	'''      Similarly, the user-specified <seealso cref="ContentHandler"/> will receive non-null
	'''      Strings for all three parameters.
	''' 
	'''  <li>Applications must ensure that <seealso cref="ValidatorHandler"/>'s
	'''      <seealso cref="ContentHandler#startPrefixMapping(String,String)"/> and
	'''      <seealso cref="ContentHandler#endPrefixMapping(String)"/> are invoked
	'''      properly. Similarly, the user-specified <seealso cref="ContentHandler"/>
	'''      will receive startPrefixMapping/endPrefixMapping events.
	'''      If the <seealso cref="ValidatorHandler"/> introduces additional namespace
	'''      bindings, the user-specified <seealso cref="ContentHandler"/> will receive
	'''      additional startPrefixMapping/endPrefixMapping events.
	''' 
	'''  <li><seealso cref="org.xml.sax.Attributes"/> for the
	'''      <seealso cref="ContentHandler#startElement(String,String,String,Attributes)"/> method
	'''      may or may not include xmlns* attributes.
	''' </ol>
	''' 
	''' <p>
	''' A <seealso cref="ValidatorHandler"/> is automatically reset every time
	''' the startDocument method is invoked.
	''' 
	''' <h2>Recognized Properties and Features</h2>
	''' <p>
	''' This spec defines the following feature that must be recognized
	''' by all <seealso cref="ValidatorHandler"/> implementations.
	''' 
	''' <h3><code>http://xml.org/sax/features/namespace-prefixes</code></h3>
	''' <p>
	''' This feature controls how a <seealso cref="ValidatorHandler"/> introduces
	''' namespace bindings that were not present in the original SAX event
	''' stream.
	''' When this feature is set to true, it must make
	''' sure that the user's <seealso cref="ContentHandler"/> will see
	''' the corresponding <code>xmlns*</code> attribute in
	''' the <seealso cref="org.xml.sax.Attributes"/> object of the
	''' <seealso cref="ContentHandler#startElement(String,String,String,Attributes)"/>
	''' callback. Otherwise, <code>xmlns*</code> attributes must not be
	''' added to <seealso cref="org.xml.sax.Attributes"/> that's passed to the
	''' user-specified <seealso cref="ContentHandler"/>.
	''' <p>
	''' (Note that regardless of this switch, namespace bindings are
	''' always notified to applications through
	''' <seealso cref="ContentHandler#startPrefixMapping(String,String)"/> and
	''' <seealso cref="ContentHandler#endPrefixMapping(String)"/> methods of the
	''' <seealso cref="ContentHandler"/> specified by the user.)
	''' 
	''' <p>
	''' Note that this feature does <em>NOT</em> affect the way
	''' a <seealso cref="ValidatorHandler"/> receives SAX events. It merely
	''' changes the way it augments SAX events.
	''' 
	''' <p>This feature is set to <code>false</code> by default.</p>
	''' 
	''' @author  <a href="mailto:Kohsuke.Kawaguchi@Sun.com">Kohsuke Kawaguchi</a>
	''' @since 1.5
	''' </summary>
	Public MustInherit Class ValidatorHandler
		Implements org.xml.sax.ContentHandler

		''' <summary>
		''' <p>Constructor for derived classes.</p>
		''' 
		''' <p>The constructor does nothing.</p>
		''' 
		''' <p>Derived classes must create <seealso cref="ValidatorHandler"/> objects that have
		''' <code>null</code> <seealso cref="ErrorHandler"/> and
		''' <code>null</code> <seealso cref="LSResourceResolver"/>.</p>
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Sets the <seealso cref="ContentHandler"/> which receives
		''' the augmented validation result.
		''' 
		''' <p>
		''' When a <seealso cref="ContentHandler"/> is specified, a
		''' <seealso cref="ValidatorHandler"/> will work as a filter
		''' and basically copy the incoming events to the
		''' specified <seealso cref="ContentHandler"/>.
		''' 
		''' <p>
		''' In doing so, a <seealso cref="ValidatorHandler"/> may modify
		''' the events, for example by adding defaulted attributes.
		''' 
		''' <p>
		''' A <seealso cref="ValidatorHandler"/> may buffer events to certain
		''' extent, but to allow <seealso cref="ValidatorHandler"/> to be used
		''' by a parser, the following requirement has to be met.
		''' 
		''' <ol>
		'''  <li>When
		'''      <seealso cref="ContentHandler#startElement(String, String, String, Attributes)"/>,
		'''      <seealso cref="ContentHandler#endElement(String, String, String)"/>,
		'''      <seealso cref="ContentHandler#startDocument()"/>, or
		'''      <seealso cref="ContentHandler#endDocument()"/>
		'''      are invoked on a <seealso cref="ValidatorHandler"/>,
		'''      the same method on the user-specified <seealso cref="ContentHandler"/>
		'''      must be invoked for the same event before the callback
		'''      returns.
		'''  <li><seealso cref="ValidatorHandler"/> may not introduce new elements that
		'''      were not present in the input.
		''' 
		'''  <li><seealso cref="ValidatorHandler"/> may not remove attributes that were
		'''      present in the input.
		''' </ol>
		''' 
		''' <p>
		''' When a callback method on the specified <seealso cref="ContentHandler"/>
		''' throws an exception, the same exception object must be thrown
		''' from the <seealso cref="ValidatorHandler"/>. The <seealso cref="ErrorHandler"/>
		''' should not be notified of such an exception.
		''' 
		''' <p>
		''' This method can be called even during a middle of a validation.
		''' </summary>
		''' <param name="receiver">
		'''      A <seealso cref="ContentHandler"/> or a null value. </param>
		Public MustOverride Property contentHandler As org.xml.sax.ContentHandler


		''' <summary>
		''' Sets the <seealso cref="ErrorHandler"/> to receive errors encountered
		''' during the validation.
		''' 
		''' <p>
		''' Error handler can be used to customize the error handling process
		''' during a validation. When an <seealso cref="ErrorHandler"/> is set,
		''' errors found during the validation will be first sent
		''' to the <seealso cref="ErrorHandler"/>.
		''' 
		''' <p>
		''' The error handler can abort further validation immediately
		''' by throwing <seealso cref="org.xml.sax.SAXException"/> from the handler. Or for example
		''' it can print an error to the screen and try to continue the
		''' validation by returning normally from the <seealso cref="ErrorHandler"/>
		''' 
		''' <p>
		''' If any <seealso cref="Throwable"/> is thrown from an <seealso cref="ErrorHandler"/>,
		''' the same <seealso cref="Throwable"/> object will be thrown toward the
		''' root of the call stack.
		''' 
		''' <p>
		''' <seealso cref="ValidatorHandler"/> is not allowed to
		''' throw <seealso cref="org.xml.sax.SAXException"/> without first reporting it to
		''' <seealso cref="ErrorHandler"/>.
		''' 
		''' <p>
		''' When the <seealso cref="ErrorHandler"/> is null, the implementation will
		''' behave as if the following <seealso cref="ErrorHandler"/> is set:
		''' <pre>
		''' class DraconianErrorHandler implements <seealso cref="ErrorHandler"/> {
		'''     public void fatalError( <seealso cref="org.xml.sax.SAXParseException"/> e ) throws <seealso cref="org.xml.sax.SAXException"/> {
		'''         throw e;
		'''     }
		'''     public void error( <seealso cref="org.xml.sax.SAXParseException"/> e ) throws <seealso cref="org.xml.sax.SAXException"/> {
		'''         throw e;
		'''     }
		'''     public void warning( <seealso cref="org.xml.sax.SAXParseException"/> e ) throws <seealso cref="org.xml.sax.SAXException"/> {
		'''         // noop
		'''     }
		''' }
		''' </pre>
		''' 
		''' <p>
		''' When a new <seealso cref="ValidatorHandler"/> object is created, initially
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
		''' <seealso cref="ValidatorHandler"/> uses a <seealso cref="LSResourceResolver"/>
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
		''' then the <seealso cref="ValidatorHandler"/> will abort the parsing and
		''' the caller of the <code>validate</code> method will receive
		''' the same <seealso cref="RuntimeException"/>.
		''' 
		''' <p>
		''' When a new <seealso cref="ValidatorHandler"/> object is created, initially
		''' this field is set to null.
		''' </summary>
		''' <param name="resourceResolver">
		'''      A new resource resolver to be set. This parameter can be null. </param>
		Public MustOverride Property resourceResolver As org.w3c.dom.ls.LSResourceResolver


		''' <summary>
		''' Obtains the <seealso cref="TypeInfoProvider"/> implementation of this
		''' <seealso cref="ValidatorHandler"/>.
		''' 
		''' <p>
		''' The obtained <seealso cref="TypeInfoProvider"/> can be queried during a parse
		''' to access the type information determined by the validator.
		''' 
		''' <p>
		''' Some schema languages do not define the notion of type,
		''' for those languages, this method may not be supported.
		''' However, to be compliant with this specification, implementations
		''' for W3C XML Schema 1.0 must support this operation.
		''' 
		''' @return
		'''      null if the validator / schema language does not support
		'''      the notion of <seealso cref="org.w3c.dom.TypeInfo"/>.
		'''      Otherwise a non-null valid <seealso cref="TypeInfoProvider"/>.
		''' </summary>
		Public MustOverride ReadOnly Property typeInfoProvider As TypeInfoProvider


		''' <summary>
		''' Look up the value of a feature flag.
		''' 
		''' <p>The feature name is any fully-qualified URI.  It is
		''' possible for a <seealso cref="ValidatorHandler"/> to recognize a feature name but
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
		'''   <seealso cref="ValidatorHandler"/> recognizes the feature name but
		'''   cannot determine its value at this time. </exception>
		''' <exception cref="NullPointerException"> When <code>name</code> is <code>null</code>.
		''' </exception>
		''' <seealso cref= #setFeature(String, boolean) </seealso>
		Public Overridable Function getFeature(ByVal name As String) As Boolean

			If name Is Nothing Then Throw New NullPointerException

			Throw New org.xml.sax.SAXNotRecognizedException(name)
		End Function

		''' <summary>
		''' <p>Set a feature for this <code>ValidatorHandler</code>.</p>
		''' 
		''' <p>Feature can be used to control the way a
		''' <seealso cref="ValidatorHandler"/> parses schemas. The feature name is
		''' any fully-qualified URI. It is possible for a
		''' <seealso cref="SchemaFactory"/> to
		''' expose a feature value but to be unable to change the current
		''' value. Some feature values may be immutable or mutable only in
		''' specific contexts, such as before, during, or after a
		''' validation.</p>
		''' 
		''' <p>All implementations are required to support the <seealso cref="javax.xml.XMLConstants#FEATURE_SECURE_PROCESSING"/> feature.
		''' When the feature is:</p>
		''' <ul>
		'''   <li>
		'''     <code>true</code>: the implementation will limit XML processing to conform to implementation limits.
		'''     Examples include enity expansion limits and XML Schema constructs that would consume large amounts of resources.
		'''     If XML processing is limited for security reasons, it will be reported via a call to the registered
		'''    <seealso cref="ErrorHandler#fatalError(SAXParseException exception)"/>.
		'''     See <seealso cref="#setErrorHandler(ErrorHandler errorHandler)"/>.
		'''   </li>
		'''   <li>
		'''     <code>false</code>: the implementation will processing XML according to the XML specifications without
		'''     regard to possible implementation limits.
		'''   </li>
		''' </ul>
		''' </summary>
		''' <param name="name"> The feature name, which is a non-null fully-qualified URI. </param>
		''' <param name="value"> The requested value of the feature (true or false).
		''' </param>
		''' <exception cref="SAXNotRecognizedException"> If the feature
		'''   value can't be assigned or retrieved. </exception>
		''' <exception cref="SAXNotSupportedException"> When the
		'''   <seealso cref="ValidatorHandler"/> recognizes the feature name but
		'''   cannot set the requested value. </exception>
		''' <exception cref="NullPointerException"> When <code>name</code> is <code>null</code>.
		''' </exception>
		''' <seealso cref= #getFeature(String) </seealso>
		Public Overridable Sub setFeature(ByVal name As String, ByVal value As Boolean)

			If name Is Nothing Then Throw New NullPointerException

			Throw New org.xml.sax.SAXNotRecognizedException(name)
		End Sub

		''' <summary>
		''' Set the value of a property.
		''' 
		''' <p>The property name is any fully-qualified URI.  It is
		''' possible for a <seealso cref="ValidatorHandler"/> to recognize a property name but
		''' to be unable to change the current value.
		''' Some property values may be immutable or mutable only
		''' in specific contexts, such as before, during, or after
		''' a validation.</p>
		''' 
		''' <p><seealso cref="ValidatorHandler"/>s are not required to recognize setting
		''' any specific property names.</p>
		''' </summary>
		''' <param name="name"> The property name, which is a non-null fully-qualified URI. </param>
		''' <param name="object"> The requested value for the property.
		''' </param>
		''' <exception cref="SAXNotRecognizedException"> If the property
		'''   value can't be assigned or retrieved. </exception>
		''' <exception cref="SAXNotSupportedException"> When the
		'''   <seealso cref="ValidatorHandler"/> recognizes the property name but
		'''   cannot set the requested value. </exception>
		''' <exception cref="NullPointerException"> When <code>name</code> is <code>null</code>. </exception>
		Public Overridable Sub setProperty(ByVal name As String, ByVal [object] As Object)

			If name Is Nothing Then Throw New NullPointerException

			Throw New org.xml.sax.SAXNotRecognizedException(name)
		End Sub

		''' <summary>
		''' Look up the value of a property.
		''' 
		''' <p>The property name is any fully-qualified URI.  It is
		''' possible for a <seealso cref="ValidatorHandler"/> to recognize a property name but
		''' temporarily be unable to return its value.
		''' Some property values may be available only in specific
		''' contexts, such as before, during, or after a validation.</p>
		''' 
		''' <p><seealso cref="ValidatorHandler"/>s are not required to recognize any specific
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
		''' <exception cref="NullPointerException"> When <code>name</code> is <code>null</code>.
		''' </exception>
		''' <seealso cref= #setProperty(String, Object) </seealso>
		Public Overridable Function getProperty(ByVal name As String) As Object

			If name Is Nothing Then Throw New NullPointerException

			Throw New org.xml.sax.SAXNotRecognizedException(name)
		End Function
	End Class

End Namespace