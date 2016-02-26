'
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
' * Copyright (c) 2009 by Oracle Corporation. All Rights Reserved.
' 

Namespace javax.xml.stream.events


	''' <summary>
	''' An interface that contains information about a namespace.
	''' Namespaces are accessed from a StartElement.
	''' 
	''' @version 1.0
	''' @author Copyright (c) 2009 by Oracle Corporation. All Rights Reserved. </summary>
	''' <seealso cref= StartElement
	''' @since 1.6 </seealso>
	Public Interface [Namespace]
		Inherits Attribute

	  ''' <summary>
	  ''' Gets the prefix, returns "" if this is a default
	  ''' namespace declaration.
	  ''' </summary>
	  ReadOnly Property prefix As String

	  ''' <summary>
	  ''' Gets the uri bound to the prefix of this namespace
	  ''' </summary>
	  ReadOnly Property namespaceURI As String

	  ''' <summary>
	  ''' returns true if this attribute declares the default namespace
	  ''' </summary>
	  ReadOnly Property defaultNamespaceDeclaration As Boolean
	End Interface

End Namespace