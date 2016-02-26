Imports System.Collections

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
	''' An interface for the end element event.  An EndElement is reported
	''' for each End Tag in the document.
	''' 
	''' @version 1.0
	''' @author Copyright (c) 2009 by Oracle Corporation. All Rights Reserved. </summary>
	''' <seealso cref= XMLEvent
	''' @since 1.6 </seealso>
	Public Interface EndElement
		Inherits XMLEvent

	  ''' <summary>
	  ''' Get the name of this event </summary>
	  ''' <returns> the qualified name of this event </returns>
	  ReadOnly Property name As javax.xml.namespace.QName

	  ''' <summary>
	  ''' Returns an Iterator of namespaces that have gone out
	  ''' of scope.  Returns an empty iterator if no namespaces have gone
	  ''' out of scope. </summary>
	  ''' <returns> an Iterator over Namespace interfaces, or an
	  ''' empty iterator </returns>
	  ReadOnly Property namespaces As IEnumerator

	End Interface

End Namespace