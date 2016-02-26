Imports javax.swing.text

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

Namespace javax.accessibility



	''' <summary>
	''' <P>The AccessibleText interface should be implemented by all
	''' classes that present textual information on the display.  This interface
	''' provides the standard mechanism for an assistive technology to access
	''' that text via its content, attributes, and spatial location.
	''' Applications can determine if an object supports the AccessibleText
	''' interface by first obtaining its AccessibleContext (see <seealso cref="Accessible"/>)
	''' and then calling the <seealso cref="AccessibleContext#getAccessibleText"/> method of
	''' AccessibleContext.  If the return value is not null, the object supports this
	''' interface.
	''' </summary>
	''' <seealso cref= Accessible </seealso>
	''' <seealso cref= Accessible#getAccessibleContext </seealso>
	''' <seealso cref= AccessibleContext </seealso>
	''' <seealso cref= AccessibleContext#getAccessibleText
	''' 
	''' @author      Peter Korn </seealso>
	Public Interface AccessibleText

		''' <summary>
		''' Constant used to indicate that the part of the text that should be
		''' retrieved is a character.
		''' </summary>
		''' <seealso cref= #getAtIndex </seealso>
		''' <seealso cref= #getAfterIndex </seealso>
		''' <seealso cref= #getBeforeIndex </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int CHARACTER = 1;

		''' <summary>
		''' Constant used to indicate that the part of the text that should be
		''' retrieved is a word.
		''' </summary>
		''' <seealso cref= #getAtIndex </seealso>
		''' <seealso cref= #getAfterIndex </seealso>
		''' <seealso cref= #getBeforeIndex </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int WORD = 2;

		''' <summary>
		''' Constant used to indicate that the part of the text that should be
		''' retrieved is a sentence.
		''' 
		''' A sentence is a string of words which expresses an assertion,
		''' a question, a command, a wish, an exclamation, or the performance
		''' of an action. In English locales, the string usually begins with
		''' a capital letter and concludes with appropriate end punctuation;
		''' such as a period, question or exclamation mark. Other locales may
		''' use different capitalization and/or punctuation.
		''' </summary>
		''' <seealso cref= #getAtIndex </seealso>
		''' <seealso cref= #getAfterIndex </seealso>
		''' <seealso cref= #getBeforeIndex </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int SENTENCE = 3;

		''' <summary>
		''' Given a point in local coordinates, return the zero-based index
		''' of the character under that Point.  If the point is invalid,
		''' this method returns -1.
		''' </summary>
		''' <param name="p"> the Point in local coordinates </param>
		''' <returns> the zero-based index of the character under Point p; if
		''' Point is invalid return -1. </returns>
		Function getIndexAtPoint(ByVal p As Point) As Integer

		''' <summary>
		''' Determines the bounding box of the character at the given
		''' index into the string.  The bounds are returned in local
		''' coordinates.  If the index is invalid an empty rectangle is returned.
		''' </summary>
		''' <param name="i"> the index into the String </param>
		''' <returns> the screen coordinates of the character's bounding box,
		''' if index is invalid return an empty rectangle. </returns>
		Function getCharacterBounds(ByVal i As Integer) As Rectangle

		''' <summary>
		''' Returns the number of characters (valid indicies)
		''' </summary>
		''' <returns> the number of characters </returns>
		ReadOnly Property charCount As Integer

		''' <summary>
		''' Returns the zero-based offset of the caret.
		''' 
		''' Note: That to the right of the caret will have the same index
		''' value as the offset (the caret is between two characters). </summary>
		''' <returns> the zero-based offset of the caret. </returns>
		ReadOnly Property caretPosition As Integer

		''' <summary>
		''' Returns the String at a given index.
		''' </summary>
		''' <param name="part"> the CHARACTER, WORD, or SENTENCE to retrieve </param>
		''' <param name="index"> an index within the text </param>
		''' <returns> the letter, word, or sentence </returns>
		Function getAtIndex(ByVal part As Integer, ByVal index As Integer) As String

		''' <summary>
		''' Returns the String after a given index.
		''' </summary>
		''' <param name="part"> the CHARACTER, WORD, or SENTENCE to retrieve </param>
		''' <param name="index"> an index within the text </param>
		''' <returns> the letter, word, or sentence </returns>
		Function getAfterIndex(ByVal part As Integer, ByVal index As Integer) As String

		''' <summary>
		''' Returns the String before a given index.
		''' </summary>
		''' <param name="part"> the CHARACTER, WORD, or SENTENCE to retrieve </param>
		''' <param name="index"> an index within the text </param>
		''' <returns> the letter, word, or sentence </returns>
		Function getBeforeIndex(ByVal part As Integer, ByVal index As Integer) As String

		''' <summary>
		''' Returns the AttributeSet for a given character at a given index
		''' </summary>
		''' <param name="i"> the zero-based index into the text </param>
		''' <returns> the AttributeSet of the character </returns>
		Function getCharacterAttribute(ByVal i As Integer) As AttributeSet

		''' <summary>
		''' Returns the start offset within the selected text.
		''' If there is no selection, but there is
		''' a caret, the start and end offsets will be the same.
		''' </summary>
		''' <returns> the index into the text of the start of the selection </returns>
		ReadOnly Property selectionStart As Integer

		''' <summary>
		''' Returns the end offset within the selected text.
		''' If there is no selection, but there is
		''' a caret, the start and end offsets will be the same.
		''' </summary>
		''' <returns> the index into the text of the end of the selection </returns>
		ReadOnly Property selectionEnd As Integer

		''' <summary>
		''' Returns the portion of the text that is selected.
		''' </summary>
		''' <returns> the String portion of the text that is selected </returns>
		ReadOnly Property selectedText As String
	End Interface

End Namespace