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

Namespace javax.xml.bind

	''' <summary>
	''' As of JAXB 2.0, this class is deprecated and optional.
	''' <p>
	''' The <tt>Validator</tt> class is responsible for controlling the validation
	''' of content trees during runtime.
	''' 
	''' <p>
	''' <a name="validationtypes"></a>
	''' <b>Three Forms of Validation</b><br>
	''' <blockquote>
	'''    <dl>
	'''        <dt><b>Unmarshal-Time Validation</b></dt>
	'''        <dd>This form of validation enables a client application to receive
	'''            information about validation errors and warnings detected while
	'''            unmarshalling XML data into a Java content tree and is completely
	'''            orthogonal to the other types of validation.  To enable or disable
	'''            it, see the javadoc for
	'''            <seealso cref="Unmarshaller#setValidating(boolean) Unmarshaller.setValidating"/>.
	'''            All JAXB 1.0 Providers are required to support this operation.
	'''        </dd>
	''' 
	'''        <dt><b>On-Demand Validation</b></dt>
	'''        <dd> This form of validation enables a client application to receive
	'''             information about validation errors and warnings detected in the
	'''             Java content tree.  At any point, client applications can call
	'''             the <seealso cref="Validator#validate(Object) Validator.validate"/> method
	'''             on the Java content tree (or any sub-tree of it).  All JAXB 1.0
	'''             Providers are required to support this operation.
	'''        </dd>
	''' 
	'''        <dt><b>Fail-Fast Validation</b></dt>
	'''        <dd> This form of validation enables a client application to receive
	'''             immediate feedback about modifications to the Java content tree
	'''             that violate type constraints on Java Properties as defined in
	'''             the specification.  JAXB Providers are not required support
	'''             this type of validation.  Of the JAXB Providers that do support
	'''             this type of validation, some may require you to decide at schema
	'''             compile time whether or not a client application will be allowed
	'''             to request fail-fast validation at runtime.
	'''        </dd>
	'''    </dl>
	''' </blockquote>
	''' 
	''' <p>
	''' The <tt>Validator</tt> class is responsible for managing On-Demand Validation.
	''' The <tt>Unmarshaller</tt> class is responsible for managing Unmarshal-Time
	''' Validation during the unmarshal operations.  Although there is no formal
	''' method of enabling validation during the marshal operations, the
	''' <tt>Marshaller</tt> may detect errors, which will be reported to the
	''' <tt>ValidationEventHandler</tt> registered on it.
	''' 
	''' <p>
	''' <a name="defaulthandler"></a>
	''' <b>Using the Default EventHandler</b><br>
	''' <blockquote>
	'''   If the client application does not set an event handler on their
	'''   <tt>Validator</tt>, <tt>Unmarshaller</tt>, or <tt>Marshaller</tt> prior to
	'''   calling the validate, unmarshal, or marshal methods, then a default event
	'''   handler will receive notification of any errors or warnings encountered.
	'''   The default event handler will cause the current operation to halt after
	'''   encountering the first error or fatal error (but will attempt to continue
	'''   after receiving warnings).
	''' </blockquote>
	''' 
	''' <p>
	''' <a name="handlingevents"></a>
	''' <b>Handling Validation Events</b><br>
	''' <blockquote>
	'''   There are three ways to handle events encountered during the unmarshal,
	'''   validate, and marshal operations:
	'''    <dl>
	'''        <dt>Use the default event handler</dt>
	'''        <dd>The default event handler will be used if you do not specify one
	'''            via the <tt>setEventHandler</tt> API's on <tt>Validator</tt>,
	'''            <tt>Unmarshaller</tt>, or <tt>Marshaller</tt>.
	'''        </dd>
	''' 
	'''        <dt>Implement and register a custom event handler</dt>
	'''        <dd>Client applications that require sophisticated event processing
	'''            can implement the <tt>ValidationEventHandler</tt> interface and
	'''            register it with the <tt>Unmarshaller</tt> and/or
	'''            <tt>Validator</tt>.
	'''        </dd>
	''' 
	'''        <dt>Use the <seealso cref="javax.xml.bind.util.ValidationEventCollector ValidationEventCollector"/>
	'''            utility</dt>
	'''        <dd>For convenience, a specialized event handler is provided that
	'''            simply collects any <tt>ValidationEvent</tt> objects created
	'''            during the unmarshal, validate, and marshal operations and
	'''            returns them to the client application as a
	'''            <tt>java.util.Collection</tt>.
	'''        </dd>
	'''    </dl>
	''' </blockquote>
	''' 
	''' <p>
	''' <b>Validation and Well-Formedness</b><br>
	''' <blockquote>
	''' <p>
	''' Validation events are handled differently depending on how the client
	''' application is configured to process them as described in the previous
	''' section.  However, there are certain cases where a JAXB Provider indicates
	''' that it is no longer able to reliably detect and report errors.  In these
	''' cases, the JAXB Provider will set the severity of the ValidationEvent to
	''' FATAL_ERROR to indicate that the unmarshal, validate, or marshal operations
	''' should be terminated.  The default event handler and
	''' <tt>ValidationEventCollector</tt> utility class must terminate processing
	''' after being notified of a fatal error.  Client applications that supply their
	''' own <tt>ValidationEventHandler</tt> should also terminate processing after
	''' being notified of a fatal error.  If not, unexpected behaviour may occur.
	''' </blockquote>
	''' 
	''' <p>
	''' <a name="supportedProps"></a>
	''' <b>Supported Properties</b><br>
	''' <blockquote>
	''' <p>
	''' There currently are not any properties required to be supported by all
	''' JAXB Providers on Validator.  However, some providers may support
	''' their own set of provider specific properties.
	''' </blockquote>
	''' 
	''' 
	''' @author <ul><li>Ryan Shoemaker, Sun Microsystems, Inc.</li><li>Kohsuke Kawaguchi, Sun Microsystems, Inc.</li><li>Joe Fialli, Sun Microsystems, Inc.</li></ul> </summary>
	''' <seealso cref= JAXBContext </seealso>
	''' <seealso cref= Unmarshaller </seealso>
	''' <seealso cref= ValidationEventHandler </seealso>
	''' <seealso cref= ValidationEvent </seealso>
	''' <seealso cref= javax.xml.bind.util.ValidationEventCollector
	''' @since JAXB1.0 </seealso>
	''' @deprecated since JAXB 2.0 
	Public Interface Validator

		''' <summary>
		''' Allow an application to register a validation event handler.
		''' <p>
		''' The validation event handler will be called by the JAXB Provider if any
		''' validation errors are encountered during calls to
		''' <seealso cref="#validate(Object) validate"/>.  If the client application does not
		''' register a validation event handler before invoking the validate method,
		''' then validation events will be handled by the default event handler which
		''' will terminate the validate operation after the first error or fatal error
		''' is encountered.
		''' <p>
		''' Calling this method with a null parameter will cause the Validator
		''' to revert back to the default default event handler.
		''' </summary>
		''' <param name="handler"> the validation event handler </param>
		''' <exception cref="JAXBException"> if an error was encountered while setting the
		'''         event handler </exception>
		''' @deprecated since JAXB2.0 
		Property eventHandler As ValidationEventHandler


		''' <summary>
		''' Validate the Java content tree starting at <tt>subrootObj</tt>.
		''' <p>
		''' Client applications can use this method to validate Java content trees
		''' on-demand at runtime.  This method can be used to validate any arbitrary
		''' subtree of the Java content tree.  Global constraint checking <b>will not
		''' </b> be performed as part of this operation (i.e. ID/IDREF constraints).
		''' </summary>
		''' <param name="subrootObj"> the obj to begin validation at </param>
		''' <exception cref="JAXBException"> if any unexpected problem occurs during validation </exception>
		''' <exception cref="ValidationException">
		'''     If the <seealso cref="ValidationEventHandler ValidationEventHandler"/>
		'''     returns false from its <tt>handleEvent</tt> method or the
		'''     <tt>Validator</tt> is unable to validate the content tree rooted
		'''     at <tt>subrootObj</tt> </exception>
		''' <exception cref="IllegalArgumentException">
		'''      If the subrootObj parameter is null </exception>
		''' <returns> true if the subtree rooted at <tt>subrootObj</tt> is valid, false
		'''         otherwise </returns>
		''' @deprecated since JAXB2.0 
		Function validate(ByVal subrootObj As Object) As Boolean

		''' <summary>
		''' Validate the Java content tree rooted at <tt>rootObj</tt>.
		''' <p>
		''' Client applications can use this method to validate Java content trees
		''' on-demand at runtime.  This method is used to validate an entire Java
		''' content tree.  Global constraint checking <b>will</b> be performed as
		''' part of this operation (i.e. ID/IDREF constraints).
		''' </summary>
		''' <param name="rootObj"> the root obj to begin validation at </param>
		''' <exception cref="JAXBException"> if any unexpected problem occurs during validation </exception>
		''' <exception cref="ValidationException">
		'''     If the <seealso cref="ValidationEventHandler ValidationEventHandler"/>
		'''     returns false from its <tt>handleEvent</tt> method or the
		'''     <tt>Validator</tt> is unable to validate the content tree rooted
		'''     at <tt>rootObj</tt> </exception>
		''' <exception cref="IllegalArgumentException">
		'''      If the rootObj parameter is null </exception>
		''' <returns> true if the tree rooted at <tt>rootObj</tt> is valid, false
		'''         otherwise </returns>
		''' @deprecated since JAXB2.0 
		Function validateRoot(ByVal rootObj As Object) As Boolean

		''' <summary>
		''' Set the particular property in the underlying implementation of
		''' <tt>Validator</tt>.  This method can only be used to set one of
		''' the standard JAXB defined properties above or a provider specific
		''' property.  Attempting to set an undefined property will result in
		''' a PropertyException being thrown.  See <a href="#supportedProps">
		''' Supported Properties</a>.
		''' </summary>
		''' <param name="name"> the name of the property to be set. This value can either
		'''              be specified using one of the constant fields or a user
		'''              supplied string. </param>
		''' <param name="value"> the value of the property to be set
		''' </param>
		''' <exception cref="PropertyException"> when there is an error processing the given
		'''                            property or value </exception>
		''' <exception cref="IllegalArgumentException">
		'''      If the name parameter is null </exception>
		''' @deprecated since JAXB2.0 
		Sub setProperty(ByVal name As String, ByVal value As Object)

		''' <summary>
		''' Get the particular property in the underlying implementation of
		''' <tt>Validator</tt>.  This method can only be used to get one of
		''' the standard JAXB defined properties above or a provider specific
		''' property.  Attempting to get an undefined property will result in
		''' a PropertyException being thrown.  See <a href="#supportedProps">
		''' Supported Properties</a>.
		''' </summary>
		''' <param name="name"> the name of the property to retrieve </param>
		''' <returns> the value of the requested property
		''' </returns>
		''' <exception cref="PropertyException">
		'''      when there is an error retrieving the given property or value
		'''      property name </exception>
		''' <exception cref="IllegalArgumentException">
		'''      If the name parameter is null </exception>
		''' @deprecated since JAXB2.0 
		Function getProperty(ByVal name As String) As Object

	End Interface

End Namespace