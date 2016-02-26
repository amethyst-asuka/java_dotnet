Imports System.Collections

'
' * Copyright (c) 2005, 2011, Oracle and/or its affiliates. All rights reserved.
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
'
' * $Id: Reference.java,v 1.9 2005/05/10 16:03:46 mullan Exp $
' 
Namespace javax.xml.crypto.dsig


	''' <summary>
	''' A representation of the <code>Reference</code> element as defined in the
	''' <a href="http://www.w3.org/TR/xmldsig-core/">
	''' W3C Recommendation for XML-Signature Syntax and Processing</a>.
	''' The XML schema is defined as:
	''' <code><pre>
	''' &lt;element name="Reference" type="ds:ReferenceType"/&gt;
	''' &lt;complexType name="ReferenceType"&gt;
	'''   &lt;sequence&gt;
	'''     &lt;element ref="ds:Transforms" minOccurs="0"/&gt;
	'''     &lt;element ref="ds:DigestMethod"/&gt;
	'''     &lt;element ref="ds:DigestValue"/&gt;
	'''   &lt;/sequence&gt;
	'''   &lt;attribute name="Id" type="ID" use="optional"/&gt;
	'''   &lt;attribute name="URI" type="anyURI" use="optional"/&gt;
	'''   &lt;attribute name="Type" type="anyURI" use="optional"/&gt;
	''' &lt;/complexType&gt;
	''' 
	''' &lt;element name="DigestValue" type="ds:DigestValueType"/&gt;
	''' &lt;simpleType name="DigestValueType"&gt;
	'''   &lt;restriction base="base64Binary"/&gt;
	''' &lt;/simpleType&gt;
	''' </pre></code>
	''' 
	''' <p>A <code>Reference</code> instance may be created by invoking one of the
	''' <seealso cref="XMLSignatureFactory#newReference newReference"/> methods of the
	''' <seealso cref="XMLSignatureFactory"/> class; for example:
	''' 
	''' <pre>
	'''   XMLSignatureFactory factory = XMLSignatureFactory.getInstance("DOM");
	'''   Reference ref = factory.newReference
	'''     ("http://www.ietf.org/rfc/rfc3275.txt",
	'''      factory.newDigestMethod(DigestMethod.SHA1, null));
	''' </pre>
	''' 
	''' @author Sean Mullan
	''' @author Erwin van der Koogh
	''' @author JSR 105 Expert Group
	''' @since 1.6 </summary>
	''' <seealso cref= XMLSignatureFactory#newReference(String, DigestMethod) </seealso>
	''' <seealso cref= XMLSignatureFactory#newReference(String, DigestMethod, List, String, String) </seealso>
	Public Interface Reference
		Inherits javax.xml.crypto.URIReference, javax.xml.crypto.XMLStructure

		''' <summary>
		''' Returns an {@link java.util.Collections#unmodifiableList unmodifiable
		''' list} of <seealso cref="Transform"/>s that are contained in this
		''' <code>Reference</code>.
		''' </summary>
		''' <returns> an unmodifiable list of <code>Transform</code>s
		'''    (may be empty but never <code>null</code>) </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		ReadOnly Property transforms As IList

		''' <summary>
		''' Returns the digest method of this <code>Reference</code>.
		''' </summary>
		''' <returns> the digest method </returns>
		ReadOnly Property digestMethod As DigestMethod

		''' <summary>
		''' Returns the optional <code>Id</code> attribute of this
		''' <code>Reference</code>, which permits this reference to be
		''' referenced from elsewhere.
		''' </summary>
		''' <returns> the <code>Id</code> attribute (may be <code>null</code> if not
		'''    specified) </returns>
		ReadOnly Property id As String

		''' <summary>
		''' Returns the digest value of this <code>Reference</code>.
		''' </summary>
		''' <returns> the raw digest value, or <code>null</code> if this reference has
		'''    not been digested yet. Each invocation of this method returns a new
		'''    clone to protect against subsequent modification. </returns>
		ReadOnly Property digestValue As SByte()

		''' <summary>
		''' Returns the calculated digest value of this <code>Reference</code>
		''' after a validation operation. This method is useful for debugging if
		''' the reference fails to validate.
		''' </summary>
		''' <returns> the calculated digest value, or <code>null</code> if this
		'''    reference has not been validated yet. Each invocation of this method
		'''    returns a new clone to protect against subsequent modification. </returns>
		ReadOnly Property calculatedDigestValue As SByte()

		''' <summary>
		''' Validates this reference. This method verifies the digest of this
		''' reference.
		''' 
		''' <p>This method only validates the reference the first time it is
		''' invoked. On subsequent invocations, it returns a cached result.
		''' </summary>
		''' <returns> <code>true</code> if this reference was validated successfully;
		'''    <code>false</code> otherwise </returns>
		''' <param name="validateContext"> the validating context </param>
		''' <exception cref="NullPointerException"> if <code>validateContext</code> is
		'''    <code>null</code> </exception>
		''' <exception cref="XMLSignatureException"> if an unexpected exception occurs while
		'''    validating the reference </exception>
		Function validate(ByVal validateContext As XMLValidateContext) As Boolean

		''' <summary>
		''' Returns the dereferenced data, if
		''' <a href="XMLSignContext.html#Supported Properties">reference caching</a>
		''' is enabled. This is the result of dereferencing the URI of this
		''' reference during a validation or generation operation.
		''' </summary>
		''' <returns> the dereferenced data, or <code>null</code> if reference
		'''    caching is not enabled or this reference has not been generated or
		'''    validated </returns>
		ReadOnly Property dereferencedData As javax.xml.crypto.Data

		''' <summary>
		''' Returns the pre-digested input stream, if
		''' <a href="XMLSignContext.html#Supported Properties">reference caching</a>
		''' is enabled. This is the input to the digest operation during a
		''' validation or signing operation.
		''' </summary>
		''' <returns> an input stream containing the pre-digested input, or
		'''    <code>null</code> if reference caching is not enabled or this
		'''    reference has not been generated or validated </returns>
		ReadOnly Property digestInputStream As java.io.InputStream
	End Interface

End Namespace