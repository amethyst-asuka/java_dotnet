'
' * Copyright (c) 2004, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Provide access to JAXB xml binding data for a JAXB object.
	''' 
	''' <p>
	''' Intially, the intent of this class is to just conceptualize how
	''' a JAXB application developer can access xml binding information,
	''' independent if binding model is java to schema or schema to java.
	''' Since accessing the XML element name related to a JAXB element is
	''' a highly requested feature, demonstrate access to this
	''' binding information.
	''' 
	''' The factory method to get a <code>JAXBIntrospector</code> instance is
	''' <seealso cref="JAXBContext#createJAXBIntrospector()"/>.
	''' </summary>
	''' <seealso cref= JAXBContext#createJAXBIntrospector()
	''' @since JAXB2.0 </seealso>
	Public MustInherit Class JAXBIntrospector

		''' <summary>
		''' <p>Return true if <code>object</code> represents a JAXB element.</p>
		''' <p>Parameter <code>object</code> is a JAXB element for following cases:
		''' <ol>
		'''   <li>It is an instance of <code>javax.xml.bind.JAXBElement</code>.</li>
		'''   <li>The class of <code>object</code> is annotated with
		'''       <code>&#64XmlRootElement</code>.
		'''   </li>
		''' </ol>
		''' </summary>
		''' <seealso cref= #getElementName(Object) </seealso>
		Public MustOverride Function isElement(ByVal [object] As Object) As Boolean

		''' <summary>
		''' <p>Get xml element qname for <code>jaxbElement</code>.</p>
		''' </summary>
		''' <param name="jaxbElement"> is an object that <seealso cref="#isElement(Object)"/> returned true.
		''' </param>
		''' <returns> xml element qname associated with jaxbElement;
		'''         null if <code>jaxbElement</code> is not a JAXB Element. </returns>
		Public MustOverride Function getElementName(ByVal jaxbElement As Object) As javax.xml.namespace.QName

		''' <summary>
		''' <p>Get the element value of a JAXB element.</p>
		''' 
		''' <p>Convenience method to abstract whether working with either
		'''    a javax.xml.bind.JAXBElement instance or an instance of
		'''    <tt>&#64XmlRootElement</tt> annotated Java class.</p>
		''' </summary>
		''' <param name="jaxbElement">  object that #isElement(Object) returns true.
		''' </param>
		''' <returns> The element value of the <code>jaxbElement</code>. </returns>
		Public Shared Function getValue(ByVal jaxbElement As Object) As Object
			If TypeOf jaxbElement Is JAXBElement Then
				Return CType(jaxbElement, JAXBElement).value
			Else
				' assume that class of this instance is
				' annotated with @XmlRootElement.
				Return jaxbElement
			End If
		End Function
	End Class

End Namespace