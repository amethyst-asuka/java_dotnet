'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Interface for a generic styled document.
	''' 
	''' @author  Timothy Prinzing
	''' </summary>
	Public Interface StyledDocument
		Inherits Document

		''' <summary>
		''' Adds a new style into the logical style hierarchy.  Style attributes
		''' resolve from bottom up so an attribute specified in a child
		''' will override an attribute specified in the parent.
		''' </summary>
		''' <param name="nm">   the name of the style (must be unique within the
		'''   collection of named styles).  The name may be null if the style
		'''   is unnamed, but the caller is responsible
		'''   for managing the reference returned as an unnamed style can't
		'''   be fetched by name.  An unnamed style may be useful for things
		'''   like character attribute overrides such as found in a style
		'''   run. </param>
		''' <param name="parent"> the parent style.  This may be null if unspecified
		'''   attributes need not be resolved in some other style. </param>
		''' <returns> the style </returns>
		Function addStyle(ByVal nm As String, ByVal parent As Style) As Style

		''' <summary>
		''' Removes a named style previously added to the document.
		''' </summary>
		''' <param name="nm">  the name of the style to remove </param>
		Sub removeStyle(ByVal nm As String)

		''' <summary>
		''' Fetches a named style previously added.
		''' </summary>
		''' <param name="nm">  the name of the style </param>
		''' <returns> the style </returns>
		Function getStyle(ByVal nm As String) As Style

		''' <summary>
		''' Changes the content element attributes used for the given range of
		''' existing content in the document.  All of the attributes
		''' defined in the given Attributes argument are applied to the
		''' given range.  This method can be used to completely remove
		''' all content level attributes for the given range by
		''' giving an Attributes argument that has no attributes defined
		''' and setting replace to true.
		''' </summary>
		''' <param name="offset"> the start of the change &gt;= 0 </param>
		''' <param name="length"> the length of the change &gt;= 0 </param>
		''' <param name="s">    the non-null attributes to change to.  Any attributes
		'''  defined will be applied to the text for the given range. </param>
		''' <param name="replace"> indicates whether or not the previous
		'''  attributes should be cleared before the new attributes
		'''  as set.  If true, the operation will replace the
		'''  previous attributes entirely.  If false, the new
		'''  attributes will be merged with the previous attributes. </param>
		Sub setCharacterAttributes(ByVal offset As Integer, ByVal length As Integer, ByVal s As AttributeSet, ByVal replace As Boolean)

		''' <summary>
		''' Sets paragraph attributes.
		''' </summary>
		''' <param name="offset"> the start of the change &gt;= 0 </param>
		''' <param name="length"> the length of the change &gt;= 0 </param>
		''' <param name="s">    the non-null attributes to change to.  Any attributes
		'''  defined will be applied to the text for the given range. </param>
		''' <param name="replace"> indicates whether or not the previous
		'''  attributes should be cleared before the new attributes
		'''  are set.  If true, the operation will replace the
		'''  previous attributes entirely.  If false, the new
		'''  attributes will be merged with the previous attributes. </param>
		Sub setParagraphAttributes(ByVal offset As Integer, ByVal length As Integer, ByVal s As AttributeSet, ByVal replace As Boolean)

		''' <summary>
		''' Sets the logical style to use for the paragraph at the
		''' given position.  If attributes aren't explicitly set
		''' for character and paragraph attributes they will resolve
		''' through the logical style assigned to the paragraph, which
		''' in turn may resolve through some hierarchy completely
		''' independent of the element hierarchy in the document.
		''' </summary>
		''' <param name="pos"> the starting position &gt;= 0 </param>
		''' <param name="s"> the style to set </param>
		Sub setLogicalStyle(ByVal pos As Integer, ByVal s As Style)

		''' <summary>
		''' Gets a logical style for a given position in a paragraph.
		''' </summary>
		''' <param name="p"> the position &gt;= 0 </param>
		''' <returns> the style </returns>
		Function getLogicalStyle(ByVal p As Integer) As Style

		''' <summary>
		''' Gets the element that represents the paragraph that
		''' encloses the given offset within the document.
		''' </summary>
		''' <param name="pos"> the offset &gt;= 0 </param>
		''' <returns> the element </returns>
		Function getParagraphElement(ByVal pos As Integer) As Element

		''' <summary>
		''' Gets the element that represents the character that
		''' is at the given offset within the document.
		''' </summary>
		''' <param name="pos"> the offset &gt;= 0 </param>
		''' <returns> the element </returns>
		Function getCharacterElement(ByVal pos As Integer) As Element


		''' <summary>
		''' Takes a set of attributes and turn it into a foreground color
		''' specification.  This might be used to specify things
		''' like brighter, more hue, etc.
		''' </summary>
		''' <param name="attr"> the set of attributes </param>
		''' <returns> the color </returns>
		Function getForeground(ByVal attr As AttributeSet) As java.awt.Color

		''' <summary>
		''' Takes a set of attributes and turn it into a background color
		''' specification.  This might be used to specify things
		''' like brighter, more hue, etc.
		''' </summary>
		''' <param name="attr"> the set of attributes </param>
		''' <returns> the color </returns>
		Function getBackground(ByVal attr As AttributeSet) As java.awt.Color

		''' <summary>
		''' Takes a set of attributes and turn it into a font
		''' specification.  This can be used to turn things like
		''' family, style, size, etc into a font that is available
		''' on the system the document is currently being used on.
		''' </summary>
		''' <param name="attr"> the set of attributes </param>
		''' <returns> the font </returns>
		Function getFont(ByVal attr As AttributeSet) As java.awt.Font

	End Interface

End Namespace