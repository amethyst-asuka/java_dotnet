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
	''' A basic event handler interface for validation errors.
	''' 
	''' <p>
	''' If an application needs to implement customized event handling, it must
	''' implement this interface and then register it with either the
	''' <seealso cref="Unmarshaller#setEventHandler(ValidationEventHandler) Unmarshaller"/>,
	''' the <seealso cref="Validator#setEventHandler(ValidationEventHandler) Validator"/>, or
	''' the <seealso cref="Marshaller#setEventHandler(ValidationEventHandler) Marshaller"/>.
	''' The JAXB Provider will then report validation errors and warnings encountered
	''' during the unmarshal, marshal, and validate operations to these event
	''' handlers.
	''' 
	''' <p>
	''' If the <tt>handleEvent</tt> method throws an unchecked runtime exception,
	''' the JAXB Provider must treat that as if the method returned false, effectively
	''' terminating whatever operation was in progress at the time (unmarshal,
	''' validate, or marshal).
	''' 
	''' <p>
	''' Modifying the Java content tree within your event handler is undefined
	''' by the specification and may result in unexpected behaviour.
	''' 
	''' <p>
	''' Failing to return false from the <tt>handleEvent</tt> method after
	''' encountering a fatal error is undefined by the specification and may result
	''' in unexpected behavior.
	''' 
	''' <p>
	''' <b>Default Event Handler</b>
	''' <blockquote>
	'''    See: <a href="Validator.html#defaulthandler">Validator javadocs</a>
	''' </blockquote>
	''' 
	''' @author <ul><li>Ryan Shoemaker, Sun Microsystems, Inc.</li><li>Kohsuke Kawaguchi, Sun Microsystems, Inc.</li><li>Joe Fialli, Sun Microsystems, Inc.</li></ul> </summary>
	''' <seealso cref= Unmarshaller </seealso>
	''' <seealso cref= Validator </seealso>
	''' <seealso cref= Marshaller </seealso>
	''' <seealso cref= ValidationEvent </seealso>
	''' <seealso cref= javax.xml.bind.util.ValidationEventCollector
	''' @since JAXB1.0 </seealso>
	Public Interface ValidationEventHandler
		''' <summary>
		''' Receive notification of a validation warning or error.
		''' 
		''' The ValidationEvent will have a
		''' <seealso cref="ValidationEventLocator ValidationEventLocator"/> embedded in it that
		''' indicates where the error or warning occurred.
		''' 
		''' <p>
		''' If an unchecked runtime exception is thrown from this method, the JAXB
		''' provider will treat it as if the method returned false and interrupt
		''' the current unmarshal, validate, or marshal operation.
		''' </summary>
		''' <param name="event"> the encapsulated validation event information.  It is a
		''' provider error if this parameter is null. </param>
		''' <returns> true if the JAXB Provider should attempt to continue the current
		'''         unmarshal, validate, or marshal operation after handling this
		'''         warning/error, false if the provider should terminate the current
		'''         operation with the appropriate <tt>UnmarshalException</tt>,
		'''         <tt>ValidationException</tt>, or <tt>MarshalException</tt>. </returns>
		''' <exception cref="IllegalArgumentException"> if the event object is null. </exception>
		Function handleEvent(ByVal [event] As ValidationEvent) As Boolean

	End Interface

End Namespace