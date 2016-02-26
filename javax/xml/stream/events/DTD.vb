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
	''' This is the top level interface for events dealing with DTDs
	''' 
	''' @version 1.0
	''' @author Copyright (c) 2009 by Oracle Corporation. All Rights Reserved.
	''' @since 1.6
	''' </summary>
	Public Interface DTD
		Inherits XMLEvent

	  ''' <summary>
	  ''' Returns the entire Document Type Declaration as a string, including
	  ''' the internal DTD subset.
	  ''' This may be null if there is not an internal subset.
	  ''' If it is not null it must return the entire
	  ''' Document Type Declaration which matches the doctypedecl
	  ''' production in the XML 1.0 specification
	  ''' </summary>
	  ReadOnly Property documentTypeDeclaration As String

	  ''' <summary>
	  ''' Returns an implementation defined representation of the DTD.
	  ''' This method may return null if no representation is available.
	  ''' </summary>
	  ReadOnly Property processedDTD As Object

	  ''' <summary>
	  ''' Return a List containing the notations declared in the DTD.
	  ''' This list must contain NotationDeclaration events. </summary>
	  ''' <seealso cref= NotationDeclaration </seealso>
	  ''' <returns> an unordered list of NotationDeclaration events </returns>
	  ReadOnly Property notations As IList

	  ''' <summary>
	  ''' Return a List containing the general entities,
	  ''' both external and internal, declared in the DTD.
	  ''' This list must contain EntityDeclaration events. </summary>
	  ''' <seealso cref= EntityDeclaration </seealso>
	  ''' <returns> an unordered list of EntityDeclaration events </returns>
	  ReadOnly Property entities As IList
	End Interface

End Namespace