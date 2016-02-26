Imports System.Collections

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
'
' * $Id: Manifest.java,v 1.7 2005/05/10 16:03:46 mullan Exp $
' 
Namespace javax.xml.crypto.dsig


	''' <summary>
	''' A representation of the XML <code>Manifest</code> element as defined in
	''' the <a href="http://www.w3.org/TR/xmldsig-core/">
	''' W3C Recommendation for XML-Signature Syntax and Processing</a>.
	''' The XML Schema Definition is defined as:
	''' <pre>{@code
	''' <element name="Manifest" type="ds:ManifestType"/>
	'''   <complexType name="ManifestType">
	'''     <sequence>
	'''       <element ref="ds:Reference" maxOccurs="unbounded"/>
	'''     </sequence>
	'''     <attribute name="Id" type="ID" use="optional"/>
	'''   </complexType>
	''' }</pre>
	''' 
	''' A <code>Manifest</code> instance may be created by invoking
	''' one of the <seealso cref="XMLSignatureFactory#newManifest newManifest"/>
	''' methods of the <seealso cref="XMLSignatureFactory"/> class; for example:
	''' 
	''' <pre>
	'''   XMLSignatureFactory factory = XMLSignatureFactory.getInstance("DOM");
	'''   List references = Collections.singletonList(factory.newReference
	'''       ("#reference-1", DigestMethod.SHA1));
	'''   Manifest manifest = factory.newManifest(references, "manifest-1");
	''' </pre>
	''' 
	''' @author Sean Mullan
	''' @author JSR 105 Expert Group
	''' @since 1.6 </summary>
	''' <seealso cref= XMLSignatureFactory#newManifest(List) </seealso>
	''' <seealso cref= XMLSignatureFactory#newManifest(List, String) </seealso>
	Public Interface Manifest
		Inherits javax.xml.crypto.XMLStructure

		''' <summary>
		''' URI that identifies the <code>Manifest</code> element (this can be
		''' specified as the value of the <code>type</code> parameter of the
		''' <seealso cref="Reference"/> class to identify the referent's type).
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final static String TYPE = "http://www.w3.org/2000/09/xmldsig#Manifest";

		''' <summary>
		''' Returns the Id of this <code>Manifest</code>.
		''' </summary>
		''' <returns> the Id  of this <code>Manifest</code> (or <code>null</code>
		'''    if not specified) </returns>
		ReadOnly Property id As String

		''' <summary>
		''' Returns an {@link java.util.Collections#unmodifiableList unmodifiable
		''' list} of one or more <seealso cref="Reference"/>s that are contained in this
		''' <code>Manifest</code>.
		''' </summary>
		''' <returns> an unmodifiable list of one or more <code>Reference</code>s </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		ReadOnly Property references As IList
	End Interface

End Namespace