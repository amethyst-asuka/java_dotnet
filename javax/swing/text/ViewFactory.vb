'
' * Copyright (c) 1997, 1998, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.text


	''' <summary>
	''' A factory to create a view of some portion of document subject.
	''' This is intended to enable customization of how views get
	''' mapped over a document model.
	''' 
	''' @author  Timothy Prinzing
	''' </summary>
	Public Interface ViewFactory

		''' <summary>
		''' Creates a view from the given structural element of a
		''' document.
		''' </summary>
		''' <param name="elem">  the piece of the document to build a view of </param>
		''' <returns> the view </returns>
		''' <seealso cref= View </seealso>
		Function create(ByVal elem As Element) As View

	End Interface

End Namespace