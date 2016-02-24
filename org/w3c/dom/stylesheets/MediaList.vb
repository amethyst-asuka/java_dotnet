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
' *
' *
' *
' *
' *
' * Copyright (c) 2000 World Wide Web Consortium,
' * (Massachusetts Institute of Technology, Institut National de
' * Recherche en Informatique et en Automatique, Keio University). All
' * Rights Reserved. This program is distributed under the W3C's Software
' * Intellectual Property License. This program is distributed in the
' * hope that it will be useful, but WITHOUT ANY WARRANTY; without even
' * the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
' * PURPOSE.
' * See W3C License http://www.w3.org/Consortium/Legal/ for more details.
' 

Namespace org.w3c.dom.stylesheets


	''' <summary>
	'''  The <code>MediaList</code> interface provides the abstraction of an
	''' ordered collection of media, without defining or constraining how this
	''' collection is implemented. An empty list is the same as a list that
	''' contains the medium <code>"all"</code>.
	''' <p> The items in the <code>MediaList</code> are accessible via an integral
	''' index, starting from 0.
	''' <p>See also the <a href='http://www.w3.org/TR/2000/REC-DOM-Level-2-Style-20001113'>Document Object Model (DOM) Level 2 Style Specification</a>.
	''' @since DOM Level 2
	''' </summary>
	Public Interface MediaList
		''' <summary>
		'''  The parsable textual representation of the media list. This is a
		''' comma-separated list of media.
		''' </summary>
		Property mediaText As String

		''' <summary>
		'''  The number of media in the list. The range of valid media is
		''' <code>0</code> to <code>length-1</code> inclusive.
		''' </summary>
		ReadOnly Property length As Integer

		''' <summary>
		'''  Returns the <code>index</code>th in the list. If <code>index</code> is
		''' greater than or equal to the number of media in the list, this
		''' returns <code>null</code>. </summary>
		''' <param name="index">  Index into the collection. </param>
		''' <returns>  The medium at the <code>index</code>th position in the
		'''   <code>MediaList</code>, or <code>null</code> if that is not a valid
		'''   index. </returns>
		Function item(ByVal index As Integer) As String

		''' <summary>
		'''  Deletes the medium indicated by <code>oldMedium</code> from the list. </summary>
		''' <param name="oldMedium"> The medium to delete in the media list. </param>
		''' <exception cref="DOMException">
		'''    NO_MODIFICATION_ALLOWED_ERR: Raised if this list is readonly.
		'''   <br> NOT_FOUND_ERR: Raised if <code>oldMedium</code> is not in the
		'''   list. </exception>
		Sub deleteMedium(ByVal oldMedium As String)

		''' <summary>
		'''  Adds the medium <code>newMedium</code> to the end of the list. If the
		''' <code>newMedium</code> is already used, it is first removed. </summary>
		''' <param name="newMedium"> The new medium to add. </param>
		''' <exception cref="DOMException">
		'''    INVALID_CHARACTER_ERR: If the medium contains characters that are
		'''   invalid in the underlying style language.
		'''   <br> NO_MODIFICATION_ALLOWED_ERR: Raised if this list is readonly. </exception>
		Sub appendMedium(ByVal newMedium As String)

	End Interface

End Namespace