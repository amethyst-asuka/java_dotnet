Imports javax.swing.text

'
' * Copyright (c) 1998, 2000, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.accessibility



	''' <summary>
	''' <P>The AccessibleHypertext class is the base class for all
	''' classes that present hypertext information on the display.  This class
	''' provides the standard mechanism for an assistive technology to access
	''' that text via its content, attributes, and spatial location.
	''' It also provides standard mechanisms for manipulating hyperlinks.
	''' Applications can determine if an object supports the AccessibleHypertext
	''' interface by first obtaining its AccessibleContext (see <seealso cref="Accessible"/>)
	''' and then calling the <seealso cref="AccessibleContext#getAccessibleText"/>
	''' method of AccessibleContext.  If the return value is a class which extends
	''' AccessibleHypertext, then that object supports AccessibleHypertext.
	''' </summary>
	''' <seealso cref= Accessible </seealso>
	''' <seealso cref= Accessible#getAccessibleContext </seealso>
	''' <seealso cref= AccessibleContext </seealso>
	''' <seealso cref= AccessibleText </seealso>
	''' <seealso cref= AccessibleContext#getAccessibleText
	''' 
	''' @author      Peter Korn </seealso>
	Public Interface AccessibleHypertext
		Inherits AccessibleText

		''' <summary>
		''' Returns the number of links within this hypertext document.
		''' </summary>
		''' <returns> number of links in this hypertext doc. </returns>
		ReadOnly Property linkCount As Integer

		''' <summary>
		''' Returns the nth Link of this Hypertext document.
		''' </summary>
		''' <param name="linkIndex"> within the links of this Hypertext </param>
		''' <returns> Link object encapsulating the nth link(s) </returns>
		Function getLink(ByVal linkIndex As Integer) As AccessibleHyperlink

		''' <summary>
		''' Returns the index into an array of hyperlinks that
		''' is associated with this character index, or -1 if there
		''' is no hyperlink associated with this index.
		''' </summary>
		''' <param name="charIndex"> index within the text </param>
		''' <returns> index into the set of hyperlinks for this hypertext doc. </returns>
		Function getLinkIndex(ByVal charIndex As Integer) As Integer
	End Interface

End Namespace