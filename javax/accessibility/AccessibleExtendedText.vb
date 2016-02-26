Imports javax.swing.text

'
' * Copyright (c) 2003, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' <P>The AccessibleExtendedText interface contains additional methods
	''' not provided by the AccessibleText interface
	''' 
	''' Applications can determine if an object supports the AccessibleExtendedText
	''' interface by first obtaining its AccessibleContext (see <seealso cref="Accessible"/>)
	''' and then calling the <seealso cref="AccessibleContext#getAccessibleText"/> method of
	''' AccessibleContext.  If the return value is an instance of
	''' AccessibleExtendedText, the object supports this interface.
	''' </summary>
	''' <seealso cref= Accessible </seealso>
	''' <seealso cref= Accessible#getAccessibleContext </seealso>
	''' <seealso cref= AccessibleContext </seealso>
	''' <seealso cref= AccessibleContext#getAccessibleText
	''' 
	''' @author       Peter Korn
	''' @author       Lynn Monsanto
	''' @since 1.5 </seealso>
	Public Interface AccessibleExtendedText

		''' <summary>
		''' Constant used to indicate that the part of the text that should be
		''' retrieved is a line of text.
		''' </summary>
		''' <seealso cref= AccessibleText#getAtIndex </seealso>
		''' <seealso cref= AccessibleText#getAfterIndex </seealso>
		''' <seealso cref= AccessibleText#getBeforeIndex </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int LINE = 4; ' BugID: 4849720

		''' <summary>
		''' Constant used to indicate that the part of the text that should be
		''' retrieved is contiguous text with the same text attributes.
		''' </summary>
		''' <seealso cref= AccessibleText#getAtIndex </seealso>
		''' <seealso cref= AccessibleText#getAfterIndex </seealso>
		''' <seealso cref= AccessibleText#getBeforeIndex </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int ATTRIBUTE_RUN = 5; ' BugID: 4849720

		''' <summary>
		''' Returns the text between two indices
		''' </summary>
		''' <param name="startIndex"> the start index in the text </param>
		''' <param name="endIndex"> the end index in the text </param>
		''' <returns> the text string if the indices are valid.
		''' Otherwise, null is returned. </returns>
		Function getTextRange(ByVal startIndex As Integer, ByVal endIndex As Integer) As String

		''' <summary>
		''' Returns the <code>AccessibleTextSequence</code> at a given index.
		''' </summary>
		''' <param name="part"> the <code>CHARACTER</code>, <code>WORD</code>,
		''' <code>SENTENCE</code>, <code>LINE</code> or <code>ATTRIBUTE_RUN</code>
		''' to retrieve </param>
		''' <param name="index"> an index within the text </param>
		''' <returns> an <code>AccessibleTextSequence</code> specifying the text
		''' if part and index are valid.  Otherwise, null is returned.
		''' </returns>
		''' <seealso cref= AccessibleText#CHARACTER </seealso>
		''' <seealso cref= AccessibleText#WORD </seealso>
		''' <seealso cref= AccessibleText#SENTENCE </seealso>
		Function getTextSequenceAt(ByVal part As Integer, ByVal index As Integer) As AccessibleTextSequence

		''' <summary>
		''' Returns the <code>AccessibleTextSequence</code> after a given index.
		''' </summary>
		''' <param name="part"> the <code>CHARACTER</code>, <code>WORD</code>,
		''' <code>SENTENCE</code>, <code>LINE</code> or <code>ATTRIBUTE_RUN</code>
		''' to retrieve </param>
		''' <param name="index"> an index within the text </param>
		''' <returns> an <code>AccessibleTextSequence</code> specifying the text
		''' if part and index are valid.  Otherwise, null is returned.
		''' </returns>
		''' <seealso cref= AccessibleText#CHARACTER </seealso>
		''' <seealso cref= AccessibleText#WORD </seealso>
		''' <seealso cref= AccessibleText#SENTENCE </seealso>
		Function getTextSequenceAfter(ByVal part As Integer, ByVal index As Integer) As AccessibleTextSequence

		''' <summary>
		''' Returns the <code>AccessibleTextSequence</code> before a given index.
		''' </summary>
		''' <param name="part"> the <code>CHARACTER</code>, <code>WORD</code>,
		''' <code>SENTENCE</code>, <code>LINE</code> or <code>ATTRIBUTE_RUN</code>
		''' to retrieve </param>
		''' <param name="index"> an index within the text </param>
		''' <returns> an <code>AccessibleTextSequence</code> specifying the text
		''' if part and index are valid.  Otherwise, null is returned.
		''' </returns>
		''' <seealso cref= AccessibleText#CHARACTER </seealso>
		''' <seealso cref= AccessibleText#WORD </seealso>
		''' <seealso cref= AccessibleText#SENTENCE </seealso>
		Function getTextSequenceBefore(ByVal part As Integer, ByVal index As Integer) As AccessibleTextSequence

		''' <summary>
		''' Returns the bounding rectangle of the text between two indices.
		''' </summary>
		''' <param name="startIndex"> the start index in the text </param>
		''' <param name="endIndex"> the end index in the text </param>
		''' <returns> the bounding rectangle of the text if the indices are valid.
		''' Otherwise, null is returned. </returns>
		Function getTextBounds(ByVal startIndex As Integer, ByVal endIndex As Integer) As Rectangle
	End Interface

End Namespace