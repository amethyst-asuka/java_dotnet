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
	''' Interface to describe a structural piece of a document.  It
	''' is intended to capture the spirit of an SGML element.
	''' 
	''' @author  Timothy Prinzing
	''' </summary>
	Public Interface Element

		''' <summary>
		''' Fetches the document associated with this element.
		''' </summary>
		''' <returns> the document </returns>
		ReadOnly Property document As Document

		''' <summary>
		''' Fetches the parent element.  If the element is a root level
		''' element returns <code>null</code>.
		''' </summary>
		''' <returns> the parent element </returns>
		ReadOnly Property parentElement As Element

		''' <summary>
		''' Fetches the name of the element.  If the element is used to
		''' represent some type of structure, this would be the type
		''' name.
		''' </summary>
		''' <returns> the element name </returns>
		ReadOnly Property name As String

		''' <summary>
		''' Fetches the collection of attributes this element contains.
		''' </summary>
		''' <returns> the attributes for the element </returns>
		ReadOnly Property attributes As AttributeSet

		''' <summary>
		''' Fetches the offset from the beginning of the document
		''' that this element begins at.  If this element has
		''' children, this will be the offset of the first child.
		''' As a document position, there is an implied forward bias.
		''' </summary>
		''' <returns> the starting offset &gt;= 0 and &lt; getEndOffset(); </returns>
		''' <seealso cref= Document </seealso>
		''' <seealso cref= AbstractDocument </seealso>
		ReadOnly Property startOffset As Integer

		''' <summary>
		''' Fetches the offset from the beginning of the document
		''' that this element ends at.  If this element has
		''' children, this will be the end offset of the last child.
		''' As a document position, there is an implied backward bias.
		''' <p>
		''' All the default <code>Document</code> implementations
		''' descend from <code>AbstractDocument</code>.
		''' <code>AbstractDocument</code> models an implied break at the end of
		''' the document. As a result of this, it is possible for this to
		''' return a value greater than the length of the document.
		''' </summary>
		''' <returns> the ending offset &gt; getStartOffset() and
		'''     &lt;= getDocument().getLength() + 1 </returns>
		''' <seealso cref= Document </seealso>
		''' <seealso cref= AbstractDocument </seealso>
		ReadOnly Property endOffset As Integer

		''' <summary>
		''' Gets the child element index closest to the given offset.
		''' The offset is specified relative to the beginning of the
		''' document.  Returns <code>-1</code> if the
		''' <code>Element</code> is a leaf, otherwise returns
		''' the index of the <code>Element</code> that best represents
		''' the given location.  Returns <code>0</code> if the location
		''' is less than the start offset. Returns
		''' <code>getElementCount() - 1</code> if the location is
		''' greater than or equal to the end offset.
		''' </summary>
		''' <param name="offset"> the specified offset &gt;= 0 </param>
		''' <returns> the element index &gt;= 0 </returns>
		Function getElementIndex(ByVal offset As Integer) As Integer

		''' <summary>
		''' Gets the number of child elements contained by this element.
		''' If this element is a leaf, a count of zero is returned.
		''' </summary>
		''' <returns> the number of child elements &gt;= 0 </returns>
		ReadOnly Property elementCount As Integer

		''' <summary>
		''' Fetches the child element at the given index.
		''' </summary>
		''' <param name="index"> the specified index &gt;= 0 </param>
		''' <returns> the child element </returns>
		Function getElement(ByVal index As Integer) As Element

		''' <summary>
		''' Is this element a leaf element? An element that
		''' <i>may</i> have children, even if it currently
		''' has no children, would return <code>false</code>.
		''' </summary>
		''' <returns> true if a leaf element else false </returns>
		ReadOnly Property leaf As Boolean


	End Interface

End Namespace