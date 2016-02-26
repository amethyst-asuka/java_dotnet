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

Namespace javax.xml.bind.annotation


	''' <summary>
	''' Converts an element (and its descendants)
	''' from/to DOM (or similar) representation.
	''' 
	''' <p>
	''' Implementations of this interface will be used in conjunction with
	''' <seealso cref="XmlAnyElement"/> annotation to map an element of XML into a representation
	''' of infoset such as W3C DOM.
	''' 
	''' <p>
	''' Implementations hide how a portion of XML is converted into/from such
	''' DOM-like representation, allowing JAXB providers to work with arbitrary
	''' such library.
	''' 
	''' <P>
	''' This interface is intended to be implemented by library writers
	''' and consumed by JAXB providers. None of those methods are intended to
	''' be called from applications.
	''' 
	''' @author Kohsuke Kawaguchi
	''' @since JAXB2.0
	''' </summary>
	Public Interface DomHandler(Of ElementT, ResultT As javax.xml.transform.Result)
		''' <summary>
		''' When a JAXB provider needs to unmarshal a part of a document into an
		''' infoset representation, it first calls this method to create a
		''' <seealso cref="Result"/> object.
		''' 
		''' <p>
		''' A JAXB provider will then send a portion of the XML
		''' into the given result. Such a portion always form a subtree
		''' of the whole XML document rooted at an element.
		''' </summary>
		''' <param name="errorHandler">
		'''      if any error happens between the invocation of this method
		'''      and the invocation of <seealso cref="#getElement(Result)"/>, they
		'''      must be reported to this handler.
		''' 
		'''      The caller must provide a non-null error handler.
		''' 
		'''      The <seealso cref="Result"/> object created from this method
		'''      may hold a reference to this error handler.
		''' 
		''' @return
		'''      null if the operation fails. The error must have been reported
		'''      to the error handler. </param>
		Function createUnmarshaller(ByVal errorHandler As javax.xml.bind.ValidationEventHandler) As ResultT

		''' <summary>
		''' Once the portion is sent to the <seealso cref="Result"/>. This method is called
		''' by a JAXB provider to obtain the unmarshalled element representation.
		''' 
		''' <p>
		''' Multiple invocations of this method may return different objects.
		''' This method can be invoked only when the whole sub-tree are fed
		''' to the <seealso cref="Result"/> object.
		''' </summary>
		''' <param name="rt">
		'''      The <seealso cref="Result"/> object created by <seealso cref="#createUnmarshaller(ValidationEventHandler)"/>.
		''' 
		''' @return
		'''      null if the operation fails. The error must have been reported
		'''      to the error handler. </param>
		Function getElement(ByVal rt As ResultT) As ElementT

		''' <summary>
		''' This method is called when a JAXB provider needs to marshal an element
		''' to XML.
		''' 
		''' <p>
		''' If non-null, the returned <seealso cref="Source"/> must contain a whole document
		''' rooted at one element, which will then be weaved into a bigger document
		''' that the JAXB provider is marshalling.
		''' </summary>
		''' <param name="errorHandler">
		'''      Receives any errors happened during the process of converting
		'''      an element into a <seealso cref="Source"/>.
		''' 
		'''      The caller must provide a non-null error handler.
		''' 
		''' @return
		'''      null if there was an error. The error should have been reported
		'''      to the handler. </param>
		Function marshal(ByVal n As ElementT, ByVal errorHandler As javax.xml.bind.ValidationEventHandler) As javax.xml.transform.Source
	End Interface

End Namespace