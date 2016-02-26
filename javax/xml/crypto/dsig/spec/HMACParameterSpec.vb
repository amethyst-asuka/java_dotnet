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
' * $Id: HMACParameterSpec.java,v 1.4 2005/05/10 16:40:17 mullan Exp $
' 
Namespace javax.xml.crypto.dsig.spec


	''' <summary>
	''' Parameters for the <a href="http://www.w3.org/TR/xmldsig-core/#sec-MACs">
	''' XML Signature HMAC Algorithm</a>. The parameters include an optional output
	''' length which specifies the MAC truncation length in bits. The resulting
	''' HMAC will be truncated to the specified number of bits. If the parameter is
	''' not specified, then this implies that all the bits of the hash are to be
	''' output. The XML Schema Definition of the <code>HMACOutputLength</code>
	''' element is defined as:
	''' <pre><code>
	''' &lt;element name="HMACOutputLength" minOccurs="0" type="ds:HMACOutputLengthType"/&gt;
	''' &lt;simpleType name="HMACOutputLengthType"&gt;
	'''   &lt;restriction base="integer"/&gt;
	''' &lt;/simpleType&gt;
	''' </code></pre>
	''' 
	''' @author Sean Mullan
	''' @author JSR 105 Expert Group
	''' @since 1.6 </summary>
	''' <seealso cref= SignatureMethod </seealso>
	''' <seealso cref= <a href="http://www.ietf.org/rfc/rfc2104.txt">RFC 2104</a> </seealso>
	Public NotInheritable Class HMACParameterSpec
		Implements SignatureMethodParameterSpec

		Private outputLength As Integer

		''' <summary>
		''' Creates an <code>HMACParameterSpec</code> with the specified truncation
		''' length.
		''' </summary>
		''' <param name="outputLength"> the truncation length in number of bits </param>
		Public Sub New(ByVal outputLength As Integer)
			Me.outputLength = outputLength
		End Sub

		''' <summary>
		''' Returns the truncation length.
		''' </summary>
		''' <returns> the truncation length in number of bits </returns>
		Public Property outputLength As Integer
			Get
				Return outputLength
			End Get
		End Property
	End Class

End Namespace