Imports javax.swing.text

'
' * Copyright (c) 2000, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' <P>The AccessibleEditableText interface should be implemented by all
	''' classes that present editable textual information on the display.
	''' Along with the AccessibleText interface, this interface provides
	''' the standard mechanism for an assistive technology to access
	''' that text via its content, attributes, and spatial location.
	''' Applications can determine if an object supports the AccessibleEditableText
	''' interface by first obtaining its AccessibleContext (see <seealso cref="Accessible"/>)
	''' and then calling the <seealso cref="AccessibleContext#getAccessibleEditableText"/>
	''' method of AccessibleContext.  If the return value is not null, the object
	''' supports this interface.
	''' </summary>
	''' <seealso cref= Accessible </seealso>
	''' <seealso cref= Accessible#getAccessibleContext </seealso>
	''' <seealso cref= AccessibleContext </seealso>
	''' <seealso cref= AccessibleContext#getAccessibleText </seealso>
	''' <seealso cref= AccessibleContext#getAccessibleEditableText
	''' 
	''' @author      Lynn Monsanto
	''' @since 1.4 </seealso>

	Public Interface AccessibleEditableText
		Inherits AccessibleText

		''' <summary>
		''' Sets the text contents to the specified string.
		''' </summary>
		''' <param name="s"> the string to set the text contents </param>
		WriteOnly Property textContents As String

		''' <summary>
		''' Inserts the specified string at the given index/
		''' </summary>
		''' <param name="index"> the index in the text where the string will
		''' be inserted </param>
		''' <param name="s"> the string to insert in the text </param>
		Sub insertTextAtIndex(ByVal index As Integer, ByVal s As String)

		''' <summary>
		''' Returns the text string between two indices.
		''' </summary>
		''' <param name="startIndex"> the starting index in the text </param>
		''' <param name="endIndex"> the ending index in the text </param>
		''' <returns> the text string between the indices </returns>
		Function getTextRange(ByVal startIndex As Integer, ByVal endIndex As Integer) As String

		''' <summary>
		''' Deletes the text between two indices
		''' </summary>
		''' <param name="startIndex"> the starting index in the text </param>
		''' <param name="endIndex"> the ending index in the text </param>
		Sub delete(ByVal startIndex As Integer, ByVal endIndex As Integer)

		''' <summary>
		''' Cuts the text between two indices into the system clipboard.
		''' </summary>
		''' <param name="startIndex"> the starting index in the text </param>
		''' <param name="endIndex"> the ending index in the text </param>
		Sub cut(ByVal startIndex As Integer, ByVal endIndex As Integer)

		''' <summary>
		''' Pastes the text from the system clipboard into the text
		''' starting at the specified index.
		''' </summary>
		''' <param name="startIndex"> the starting index in the text </param>
		Sub paste(ByVal startIndex As Integer)

		''' <summary>
		''' Replaces the text between two indices with the specified
		''' string.
		''' </summary>
		''' <param name="startIndex"> the starting index in the text </param>
		''' <param name="endIndex"> the ending index in the text </param>
		''' <param name="s"> the string to replace the text between two indices </param>
		Sub replaceText(ByVal startIndex As Integer, ByVal endIndex As Integer, ByVal s As String)

		''' <summary>
		''' Selects the text between two indices.
		''' </summary>
		''' <param name="startIndex"> the starting index in the text </param>
		''' <param name="endIndex"> the ending index in the text </param>
		Sub selectText(ByVal startIndex As Integer, ByVal endIndex As Integer)

		''' <summary>
		''' Sets attributes for the text between two indices.
		''' </summary>
		''' <param name="startIndex"> the starting index in the text </param>
		''' <param name="endIndex"> the ending index in the text </param>
		''' <param name="as"> the attribute set </param>
		''' <seealso cref= AttributeSet </seealso>
		Sub setAttributes(ByVal startIndex As Integer, ByVal endIndex As Integer, ByVal [as] As AttributeSet)

	End Interface

End Namespace