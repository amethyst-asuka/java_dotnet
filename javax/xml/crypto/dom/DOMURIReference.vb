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
' * $Id: DOMURIReference.java,v 1.5 2005/05/09 18:33:26 mullan Exp $
' 
Namespace javax.xml.crypto.dom


	''' <summary>
	''' A DOM-specific <seealso cref="URIReference"/>. The purpose of this class is to
	''' provide additional context necessary for resolving XPointer URIs or
	''' same-document references.
	''' 
	''' @author Sean Mullan
	''' @author JSR 105 Expert Group
	''' @since 1.6
	''' </summary>
	Public Interface DOMURIReference
		Inherits javax.xml.crypto.URIReference

		''' <summary>
		''' Returns the here node.
		''' </summary>
		''' <returns> the attribute or processing instruction node or the
		'''    parent element of the text node that directly contains the URI </returns>
		ReadOnly Property here As org.w3c.dom.Node
	End Interface

End Namespace