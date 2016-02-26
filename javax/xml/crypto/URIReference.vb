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
' * $Id: URIReference.java,v 1.4 2005/05/10 15:47:42 mullan Exp $
' 
Namespace javax.xml.crypto

	''' <summary>
	''' Identifies a data object via a URI-Reference, as specified by
	''' <a href="http://www.ietf.org/rfc/rfc2396.txt">RFC 2396</a>.
	''' 
	''' <p>Note that some subclasses may not have a <code>type</code> attribute
	''' and for objects of those types, the <seealso cref="#getType"/> method always returns
	''' <code>null</code>.
	''' 
	''' @author Sean Mullan
	''' @author JSR 105 Expert Group
	''' @since 1.6 </summary>
	''' <seealso cref= URIDereferencer </seealso>
	Public Interface URIReference

		''' <summary>
		''' Returns the URI of the referenced data object.
		''' </summary>
		''' <returns> the URI of the data object in RFC 2396 format (may be
		'''    <code>null</code> if not specified) </returns>
		ReadOnly Property uRI As String

		''' <summary>
		''' Returns the type of data referenced by this URI.
		''' </summary>
		''' <returns> the type (a URI) of the data object (may be <code>null</code>
		'''    if not specified) </returns>
		Function [getType]() As String
	End Interface

End Namespace