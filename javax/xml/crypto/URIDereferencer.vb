'
' * Copyright (c) 2005, Oracle and/or its affiliates. All rights reserved.
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
' * ===========================================================================
' *
' * (C) Copyright IBM Corp. 2003 All Rights Reserved.
' *
' * ===========================================================================
' 
'
' * $Id: URIDereferencer.java,v 1.5 2005/05/10 15:47:42 mullan Exp $
' 
Namespace javax.xml.crypto

	''' <summary>
	''' A dereferencer of <seealso cref="URIReference"/>s.
	''' <p>
	''' The result of dereferencing a <code>URIReference</code> is either an
	''' instance of <seealso cref="OctetStreamData"/> or <seealso cref="NodeSetData"/>. Unless the
	''' <code>URIReference</code> is a <i>same-document reference</i> as defined
	''' in section 4.2 of the W3C Recommendation for XML-Signature Syntax and
	''' Processing, the result of dereferencing the <code>URIReference</code>
	''' MUST be an <code>OctetStreamData</code>.
	''' 
	''' @author Sean Mullan
	''' @author Joyce Leung
	''' @author JSR 105 Expert Group
	''' @since 1.6 </summary>
	''' <seealso cref= XMLCryptoContext#setURIDereferencer(URIDereferencer) </seealso>
	''' <seealso cref= XMLCryptoContext#getURIDereferencer </seealso>
	Public Interface URIDereferencer

		''' <summary>
		''' Dereferences the specified <code>URIReference</code> and returns the
		''' dereferenced data.
		''' </summary>
		''' <param name="uriReference"> the <code>URIReference</code> </param>
		''' <param name="context"> an <code>XMLCryptoContext</code> that may
		'''    contain additional useful information for dereferencing the URI. This
		'''    implementation should dereference the specified
		'''    <code>URIReference</code> against the context's <code>baseURI</code>
		'''    parameter, if specified. </param>
		''' <returns> the dereferenced data </returns>
		''' <exception cref="NullPointerException"> if <code>uriReference</code> or
		'''    <code>context</code> are <code>null</code> </exception>
		''' <exception cref="URIReferenceException"> if an exception occurs while
		'''    dereferencing the specified <code>uriReference</code> </exception>
		Function dereference(ByVal uriReference As URIReference, ByVal context As XMLCryptoContext) As Data
	End Interface

End Namespace