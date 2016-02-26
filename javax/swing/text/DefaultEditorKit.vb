Imports Microsoft.VisualBasic
Imports System

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
	''' This is the set of things needed by a text component
	''' to be a reasonably functioning editor for some <em>type</em>
	''' of text document.  This implementation provides a default
	''' implementation which treats text as plain text and
	''' provides a minimal set of actions for a simple editor.
	''' 
	''' <dl>
	''' <dt><b><font size=+1>Newlines</font></b>
	''' <dd>
	''' There are two properties which deal with newlines.  The
	''' system property, <code>line.separator</code>, is defined to be
	''' platform-dependent, either "\n", "\r", or "\r\n".  There is also
	''' a property defined in <code>DefaultEditorKit</code>, called
	''' <a href=#EndOfLineStringProperty><code>EndOfLineStringProperty</code></a>,
	''' which is defined automatically when a document is loaded, to be
	''' the first occurrence of any of the newline characters.
	''' When a document is loaded, <code>EndOfLineStringProperty</code>
	''' is set appropriately, and when the document is written back out, the
	''' <code>EndOfLineStringProperty</code> is used.  But while the document
	''' is in memory, the "\n" character is used to define a
	''' newline, regardless of how the newline is defined when
	''' the document is on disk.  Therefore, for searching purposes,
	''' "\n" should always be used.  When a new document is created,
	''' and the <code>EndOfLineStringProperty</code> has not been defined,
	''' it will use the System property when writing out the
	''' document.
	''' <p>Note that <code>EndOfLineStringProperty</code> is set
	''' on the <code>Document</code> using the <code>get/putProperty</code>
	''' methods.  Subclasses may override this behavior.
	''' 
	''' </dl>
	''' 
	''' @author  Timothy Prinzing
	''' </summary>
	Public Class DefaultEditorKit
		Inherits EditorKit

		''' <summary>
		''' default constructor for DefaultEditorKit
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Gets the MIME type of the data that this
		''' kit represents support for.  The default
		''' is <code>text/plain</code>.
		''' </summary>
		''' <returns> the type </returns>
		Public Property Overrides contentType As String
			Get
				Return "text/plain"
			End Get
		End Property

		''' <summary>
		''' Fetches a factory that is suitable for producing
		''' views of any models that are produced by this
		''' kit.  The default is to have the UI produce the
		''' factory, so this method has no implementation.
		''' </summary>
		''' <returns> the view factory </returns>
		Public Property Overrides viewFactory As ViewFactory
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Fetches the set of commands that can be used
		''' on a text component that is using a model and
		''' view produced by this kit.
		''' </summary>
		''' <returns> the command list </returns>
		Public Property Overrides actions As javax.swing.Action()
			Get
				Return defaultActions
			End Get
		End Property

		''' <summary>
		''' Fetches a caret that can navigate through views
		''' produced by the associated ViewFactory.
		''' </summary>
		''' <returns> the caret </returns>
		Public Overrides Function createCaret() As Caret
			Return Nothing
		End Function

		''' <summary>
		''' Creates an uninitialized text storage model (PlainDocument)
		''' that is appropriate for this type of editor.
		''' </summary>
		''' <returns> the model </returns>
		Public Overrides Function createDefaultDocument() As Document
			Return New PlainDocument
		End Function

		''' <summary>
		''' Inserts content from the given stream which is expected
		''' to be in a format appropriate for this kind of content
		''' handler.
		''' </summary>
		''' <param name="in">  The stream to read from </param>
		''' <param name="doc"> The destination for the insertion. </param>
		''' <param name="pos"> The location in the document to place the
		'''   content &gt;=0. </param>
		''' <exception cref="IOException"> on any I/O error </exception>
		''' <exception cref="BadLocationException"> if pos represents an invalid
		'''   location within the document. </exception>
		Public Overrides Sub read(ByVal [in] As InputStream, ByVal doc As Document, ByVal pos As Integer)

			read(New InputStreamReader([in]), doc, pos)
		End Sub

		''' <summary>
		''' Writes content from a document to the given stream
		''' in a format appropriate for this kind of content handler.
		''' </summary>
		''' <param name="out"> The stream to write to </param>
		''' <param name="doc"> The source for the write. </param>
		''' <param name="pos"> The location in the document to fetch the
		'''   content &gt;=0. </param>
		''' <param name="len"> The amount to write out &gt;=0. </param>
		''' <exception cref="IOException"> on any I/O error </exception>
		''' <exception cref="BadLocationException"> if pos represents an invalid
		'''   location within the document. </exception>
		Public Overrides Sub write(ByVal out As OutputStream, ByVal doc As Document, ByVal pos As Integer, ByVal len As Integer)
			Dim osw As New OutputStreamWriter(out)

			write(osw, doc, pos, len)
			osw.flush()
		End Sub

		''' <summary>
		''' Gets the input attributes for the pane. This method exists for
		''' the benefit of StyledEditorKit so that the read method will
		''' pick up the correct attributes to apply to inserted text.
		''' This class's implementation simply returns null.
		''' </summary>
		''' <returns> null </returns>
		Friend Overridable Property inputAttributes As MutableAttributeSet
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Inserts content from the given stream, which will be
		''' treated as plain text.
		''' </summary>
		''' <param name="in">  The stream to read from </param>
		''' <param name="doc"> The destination for the insertion. </param>
		''' <param name="pos"> The location in the document to place the
		'''   content &gt;=0. </param>
		''' <exception cref="IOException"> on any I/O error </exception>
		''' <exception cref="BadLocationException"> if pos represents an invalid
		'''   location within the document. </exception>
		Public Overrides Sub read(ByVal [in] As Reader, ByVal doc As Document, ByVal pos As Integer)

			Dim buff As Char() = New Char(4095){}
			Dim nch As Integer
			Dim lastWasCR As Boolean = False
			Dim isCRLF As Boolean = False
			Dim isCR As Boolean = False
			Dim last As Integer
			Dim wasEmpty As Boolean = (doc.length = 0)
			Dim attr As AttributeSet = inputAttributes

			' Read in a block at a time, mapping \r\n to \n, as well as single
			' \r's to \n's. If a \r\n is encountered, \r\n will be set as the
			' newline string for the document, if \r is encountered it will
			' be set as the newline character, otherwise the newline property
			' for the document will be removed.
			nch = [in].read(buff, 0, buff.Length)
			Do While nch <> -1
				last = 0
				For counter As Integer = 0 To nch - 1
					Select Case buff(counter)
					Case ControlChars.Cr
						If lastWasCR Then
							isCR = True
							If counter = 0 Then
								doc.insertString(pos, vbLf, attr)
								pos += 1
							Else
								buff(counter - 1) = ControlChars.Lf
							End If
						Else
							lastWasCR = True
						End If
					Case ControlChars.Lf
						If lastWasCR Then
							If counter > (last + 1) Then
								doc.insertString(pos, New String(buff, last, counter - last - 1), attr)
								pos += (counter - last - 1)
							End If
							' else nothing to do, can skip \r, next write will
							' write \n
							lastWasCR = False
							last = counter
							isCRLF = True
						End If
					Case Else
						If lastWasCR Then
							isCR = True
							If counter = 0 Then
								doc.insertString(pos, vbLf, attr)
								pos += 1
							Else
								buff(counter - 1) = ControlChars.Lf
							End If
							lastWasCR = False
						End If
					End Select
				Next counter
				If last < nch Then
					If lastWasCR Then
						If last < (nch - 1) Then
							doc.insertString(pos, New String(buff, last, nch - last - 1), attr)
							pos += (nch - last - 1)
						End If
					Else
						doc.insertString(pos, New String(buff, last, nch - last), attr)
						pos += (nch - last)
					End If
				End If
				nch = [in].read(buff, 0, buff.Length)
			Loop
			If lastWasCR Then
				doc.insertString(pos, vbLf, attr)
				isCR = True
			End If
			If wasEmpty Then
				If isCRLF Then
					doc.putProperty(EndOfLineStringProperty, vbCrLf)
				ElseIf isCR Then
					doc.putProperty(EndOfLineStringProperty, vbCr)
				Else
					doc.putProperty(EndOfLineStringProperty, vbLf)
				End If
			End If
		End Sub

		''' <summary>
		''' Writes content from a document to the given stream
		''' as plain text.
		''' </summary>
		''' <param name="out">  The stream to write to </param>
		''' <param name="doc"> The source for the write. </param>
		''' <param name="pos"> The location in the document to fetch the
		'''   content from &gt;=0. </param>
		''' <param name="len"> The amount to write out &gt;=0. </param>
		''' <exception cref="IOException"> on any I/O error </exception>
		''' <exception cref="BadLocationException"> if pos is not within 0 and
		'''   the length of the document. </exception>
		Public Overrides Sub write(ByVal out As Writer, ByVal doc As Document, ByVal pos As Integer, ByVal len As Integer)

			If (pos < 0) OrElse ((pos + len) > doc.length) Then Throw New BadLocationException("DefaultEditorKit.write", pos)
			Dim data As New Segment
			Dim nleft As Integer = len
			Dim offs As Integer = pos
			Dim endOfLineProperty As Object = doc.getProperty(EndOfLineStringProperty)
			If endOfLineProperty Is Nothing Then
				Try
					endOfLineProperty = System.getProperty("line.separator")
				Catch se As SecurityException
				End Try
			End If
			Dim endOfLine As String
			If TypeOf endOfLineProperty Is String Then
				endOfLine = CStr(endOfLineProperty)
			Else
				endOfLine = Nothing
			End If
			If endOfLineProperty IsNot Nothing AndAlso (Not endOfLine.Equals(vbLf)) Then
				' There is an end of line string that isn't \n, have to iterate
				' through and find all \n's and translate to end of line string.
				Do While nleft > 0
					Dim n As Integer = Math.Min(nleft, 4096)
					doc.getText(offs, n, data)
					Dim last As Integer = data.offset
					Dim array As Char() = data.array
					Dim maxCounter As Integer = last + data.count
					For counter As Integer = last To maxCounter - 1
						If array(counter) = ControlChars.Lf Then
							If counter > last Then out.write(array, last, counter - last)
							out.write(endOfLine)
							last = counter + 1
						End If
					Next counter
					If maxCounter > last Then out.write(array, last, maxCounter - last)
					offs += n
					nleft -= n
				Loop
			Else
				' Just write out text, will already have \n, no mapping to
				' do.
				Do While nleft > 0
					Dim n As Integer = Math.Min(nleft, 4096)
					doc.getText(offs, n, data)
					out.write(data.array, data.offset, data.count)
					offs += n
					nleft -= n
				Loop
			End If
			out.flush()
		End Sub


		''' <summary>
		''' When reading a document if a CRLF is encountered a property
		''' with this name is added and the value will be "\r\n".
		''' </summary>
		Public Const EndOfLineStringProperty As String = "__EndOfLine__"

		' --- names of well-known actions ---------------------------

		''' <summary>
		''' Name of the action to place content into the associated
		''' document.  If there is a selection, it is removed before
		''' the new content is added. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const insertContentAction As String = "insert-content"

		''' <summary>
		''' Name of the action to place a line/paragraph break into
		''' the document.  If there is a selection, it is removed before
		''' the break is added. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const insertBreakAction As String = "insert-break"

		''' <summary>
		''' Name of the action to place a tab character into
		''' the document.  If there is a selection, it is removed before
		''' the tab is added. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const insertTabAction As String = "insert-tab"

		''' <summary>
		''' Name of the action to delete the character of content that
		''' precedes the current caret position. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const deletePrevCharAction As String = "delete-previous"

		''' <summary>
		''' Name of the action to delete the character of content that
		''' follows the current caret position. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const deleteNextCharAction As String = "delete-next"

		''' <summary>
		''' Name of the action to delete the word that
		''' follows the beginning of the selection. </summary>
		''' <seealso cref= #getActions </seealso>
		''' <seealso cref= JTextComponent#getSelectionStart
		''' @since 1.6 </seealso>
		Public Const deleteNextWordAction As String = "delete-next-word"

		''' <summary>
		''' Name of the action to delete the word that
		''' precedes the beginning of the selection. </summary>
		''' <seealso cref= #getActions </seealso>
		''' <seealso cref= JTextComponent#getSelectionStart
		''' @since 1.6 </seealso>
		Public Const deletePrevWordAction As String = "delete-previous-word"

		''' <summary>
		''' Name of the action to set the editor into read-only
		''' mode. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const readOnlyAction As String = "set-read-only"

		''' <summary>
		''' Name of the action to set the editor into writeable
		''' mode. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const writableAction As String = "set-writable"

		''' <summary>
		''' Name of the action to cut the selected region
		''' and place the contents into the system clipboard. </summary>
		''' <seealso cref= JTextComponent#cut </seealso>
		''' <seealso cref= #getActions </seealso>
		Public Const cutAction As String = "cut-to-clipboard"

		''' <summary>
		''' Name of the action to copy the selected region
		''' and place the contents into the system clipboard. </summary>
		''' <seealso cref= JTextComponent#copy </seealso>
		''' <seealso cref= #getActions </seealso>
		Public Const copyAction As String = "copy-to-clipboard"

		''' <summary>
		''' Name of the action to paste the contents of the
		''' system clipboard into the selected region, or before the
		''' caret if nothing is selected. </summary>
		''' <seealso cref= JTextComponent#paste </seealso>
		''' <seealso cref= #getActions </seealso>
		Public Const pasteAction As String = "paste-from-clipboard"

		''' <summary>
		''' Name of the action to create a beep. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const beepAction As String = "beep"

		''' <summary>
		''' Name of the action to page up vertically. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const pageUpAction As String = "page-up"

		''' <summary>
		''' Name of the action to page down vertically. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const pageDownAction As String = "page-down"

		''' <summary>
		''' Name of the action to page up vertically, and move the
		''' selection. </summary>
		''' <seealso cref= #getActions </seealso>
		'public
	 Friend Const selectionPageUpAction As String = "selection-page-up"

		''' <summary>
		''' Name of the action to page down vertically, and move the
		''' selection. </summary>
		''' <seealso cref= #getActions </seealso>
		'public
	 Friend Const selectionPageDownAction As String = "selection-page-down"

		''' <summary>
		''' Name of the action to page left horizontally, and move the
		''' selection. </summary>
		''' <seealso cref= #getActions </seealso>
		'public
	 Friend Const selectionPageLeftAction As String = "selection-page-left"

		''' <summary>
		''' Name of the action to page right horizontally, and move the
		''' selection. </summary>
		''' <seealso cref= #getActions </seealso>
		'public
	 Friend Const selectionPageRightAction As String = "selection-page-right"

		''' <summary>
		''' Name of the Action for moving the caret
		''' logically forward one position. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const forwardAction As String = "caret-forward"

		''' <summary>
		''' Name of the Action for moving the caret
		''' logically backward one position. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const backwardAction As String = "caret-backward"

		''' <summary>
		''' Name of the Action for extending the selection
		''' by moving the caret logically forward one position. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const selectionForwardAction As String = "selection-forward"

		''' <summary>
		''' Name of the Action for extending the selection
		''' by moving the caret logically backward one position. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const selectionBackwardAction As String = "selection-backward"

		''' <summary>
		''' Name of the Action for moving the caret
		''' logically upward one position. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const upAction As String = "caret-up"

		''' <summary>
		''' Name of the Action for moving the caret
		''' logically downward one position. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const downAction As String = "caret-down"

		''' <summary>
		''' Name of the Action for moving the caret
		''' logically upward one position, extending the selection. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const selectionUpAction As String = "selection-up"

		''' <summary>
		''' Name of the Action for moving the caret
		''' logically downward one position, extending the selection. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const selectionDownAction As String = "selection-down"

		''' <summary>
		''' Name of the <code>Action</code> for moving the caret
		''' to the beginning of a word. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const beginWordAction As String = "caret-begin-word"

		''' <summary>
		''' Name of the Action for moving the caret
		''' to the end of a word. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const endWordAction As String = "caret-end-word"

		''' <summary>
		''' Name of the <code>Action</code> for moving the caret
		''' to the beginning of a word, extending the selection. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const selectionBeginWordAction As String = "selection-begin-word"

		''' <summary>
		''' Name of the Action for moving the caret
		''' to the end of a word, extending the selection. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const selectionEndWordAction As String = "selection-end-word"

		''' <summary>
		''' Name of the <code>Action</code> for moving the caret to the
		''' beginning of the previous word. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const previousWordAction As String = "caret-previous-word"

		''' <summary>
		''' Name of the <code>Action</code> for moving the caret to the
		''' beginning of the next word. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const nextWordAction As String = "caret-next-word"

		''' <summary>
		''' Name of the <code>Action</code> for moving the selection to the
		''' beginning of the previous word, extending the selection. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const selectionPreviousWordAction As String = "selection-previous-word"

		''' <summary>
		''' Name of the <code>Action</code> for moving the selection to the
		''' beginning of the next word, extending the selection. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const selectionNextWordAction As String = "selection-next-word"

		''' <summary>
		''' Name of the <code>Action</code> for moving the caret
		''' to the beginning of a line. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const beginLineAction As String = "caret-begin-line"

		''' <summary>
		''' Name of the <code>Action</code> for moving the caret
		''' to the end of a line. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const endLineAction As String = "caret-end-line"

		''' <summary>
		''' Name of the <code>Action</code> for moving the caret
		''' to the beginning of a line, extending the selection. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const selectionBeginLineAction As String = "selection-begin-line"

		''' <summary>
		''' Name of the <code>Action</code> for moving the caret
		''' to the end of a line, extending the selection. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const selectionEndLineAction As String = "selection-end-line"

		''' <summary>
		''' Name of the <code>Action</code> for moving the caret
		''' to the beginning of a paragraph. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const beginParagraphAction As String = "caret-begin-paragraph"

		''' <summary>
		''' Name of the <code>Action</code> for moving the caret
		''' to the end of a paragraph. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const endParagraphAction As String = "caret-end-paragraph"

		''' <summary>
		''' Name of the <code>Action</code> for moving the caret
		''' to the beginning of a paragraph, extending the selection. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const selectionBeginParagraphAction As String = "selection-begin-paragraph"

		''' <summary>
		''' Name of the <code>Action</code> for moving the caret
		''' to the end of a paragraph, extending the selection. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const selectionEndParagraphAction As String = "selection-end-paragraph"

		''' <summary>
		''' Name of the <code>Action</code> for moving the caret
		''' to the beginning of the document. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const beginAction As String = "caret-begin"

		''' <summary>
		''' Name of the <code>Action</code> for moving the caret
		''' to the end of the document. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const endAction As String = "caret-end"

		''' <summary>
		''' Name of the <code>Action</code> for moving the caret
		''' to the beginning of the document. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const selectionBeginAction As String = "selection-begin"

		''' <summary>
		''' Name of the Action for moving the caret
		''' to the end of the document. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const selectionEndAction As String = "selection-end"

		''' <summary>
		''' Name of the Action for selecting a word around the caret. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const selectWordAction As String = "select-word"

		''' <summary>
		''' Name of the Action for selecting a line around the caret. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const selectLineAction As String = "select-line"

		''' <summary>
		''' Name of the Action for selecting a paragraph around the caret. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const selectParagraphAction As String = "select-paragraph"

		''' <summary>
		''' Name of the Action for selecting the entire document </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const selectAllAction As String = "select-all"

		''' <summary>
		''' Name of the Action for removing selection </summary>
		''' <seealso cref= #getActions </seealso>
		'public
	 Friend Const unselectAction As String = "unselect"

		''' <summary>
		''' Name of the Action for toggling the component's orientation. </summary>
		''' <seealso cref= #getActions </seealso>
		'public
	 Friend Const toggleComponentOrientationAction As String = "toggle-componentOrientation"

		''' <summary>
		''' Name of the action that is executed by default if
		''' a <em>key typed event</em> is received and there
		''' is no keymap entry. </summary>
		''' <seealso cref= #getActions </seealso>
		Public Const defaultKeyTypedAction As String = "default-typed"

		' --- Action implementations ---------------------------------

		Private Shared ReadOnly defaultActions As javax.swing.Action() = { New InsertContentAction, New DeletePrevCharAction, New DeleteNextCharAction, New ReadOnlyAction, New DeleteWordAction(deletePrevWordAction), New DeleteWordAction(deleteNextWordAction), New WritableAction, New CutAction, New CopyAction, New PasteAction, New VerticalPageAction(pageUpAction, -1, False), New VerticalPageAction(pageDownAction, 1, False), New VerticalPageAction(selectionPageUpAction, -1, True), New VerticalPageAction(selectionPageDownAction, 1, True), New PageAction(selectionPageLeftAction, True, True), New PageAction(selectionPageRightAction, False, True), New InsertBreakAction, New BeepAction, New NextVisualPositionAction(forwardAction, False, javax.swing.SwingConstants.EAST), New NextVisualPositionAction(backwardAction, False, javax.swing.SwingConstants.WEST), New NextVisualPositionAction(selectionForwardAction, True, javax.swing.SwingConstants.EAST), New NextVisualPositionAction(selectionBackwardAction, True, javax.swing.SwingConstants.WEST), New NextVisualPositionAction(upAction, False, javax.swing.SwingConstants.NORTH), New NextVisualPositionAction(downAction, False, javax.swing.SwingConstants.SOUTH), New NextVisualPositionAction(selectionUpAction, True, javax.swing.SwingConstants.NORTH), New NextVisualPositionAction(selectionDownAction, True, javax.swing.SwingConstants.SOUTH), New BeginWordAction(beginWordAction, False), New EndWordAction(endWordAction, False), New BeginWordAction(selectionBeginWordAction, True), New EndWordAction(selectionEndWordAction, True), New PreviousWordAction(previousWordAction, False), New NextWordAction(nextWordAction, False), New PreviousWordAction(selectionPreviousWordAction, True), New NextWordAction(selectionNextWordAction, True), New BeginLineAction(beginLineAction, False), New EndLineAction(endLineAction, False), New BeginLineAction(selectionBeginLineAction, True), New EndLineAction(selectionEndLineAction, True), New BeginParagraphAction(beginParagraphAction, False), New EndParagraphAction(endParagraphAction, False), New BeginParagraphAction(selectionBeginParagraphAction, True), New EndParagraphAction(selectionEndParagraphAction, True), New BeginAction(beginAction, False), New EndAction(endAction, False), New BeginAction(selectionBeginAction, True), New EndAction(selectionEndAction, True), New DefaultKeyTypedAction, New InsertTabAction, New SelectWordAction, New SelectLineAction, New SelectParagraphAction, New SelectAllAction, New UnselectAction, New ToggleComponentOrientationAction, New DumpModelAction }

		''' <summary>
		''' The action that is executed by default if
		''' a <em>key typed event</em> is received and there
		''' is no keymap entry.  There is a variation across
		''' different VM's in what gets sent as a <em>key typed</em>
		''' event, and this action tries to filter out the undesired
		''' events.  This filters the control characters and those
		''' with the ALT modifier.  It allows Control-Alt sequences
		''' through as these form legitimate unicode characters on
		''' some PC keyboards.
		''' <p>
		''' If the event doesn't get filtered, it will try to insert
		''' content into the text editor.  The content is fetched
		''' from the command string of the ActionEvent.  The text
		''' entry is done through the <code>replaceSelection</code>
		''' method on the target text component.  This is the
		''' action that will be fired for most text entry tasks.
		''' <p>
		''' <strong>Warning:</strong>
		''' Serialized objects of this class will not be compatible with
		''' future Swing releases. The current serialization support is
		''' appropriate for short term storage or RMI between applications running
		''' the same version of Swing.  As of 1.4, support for long term storage
		''' of all JavaBeans&trade;
		''' has been added to the <code>java.beans</code> package.
		''' Please see <seealso cref="java.beans.XMLEncoder"/>.
		''' </summary>
		''' <seealso cref= DefaultEditorKit#defaultKeyTypedAction </seealso>
		''' <seealso cref= DefaultEditorKit#getActions </seealso>
		''' <seealso cref= Keymap#setDefaultAction </seealso>
		''' <seealso cref= Keymap#getDefaultAction </seealso>
		Public Class DefaultKeyTypedAction
			Inherits TextAction

			''' <summary>
			''' Creates this object with the appropriate identifier.
			''' </summary>
			Public Sub New()
				MyBase.New(defaultKeyTypedAction)
			End Sub

			''' <summary>
			''' The operation to perform when this action is triggered.
			''' </summary>
			''' <param name="e"> the action event </param>
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				Dim target As JTextComponent = getTextComponent(e)
				If (target IsNot Nothing) AndAlso (e IsNot Nothing) Then
					If ((Not target.editable)) OrElse ((Not target.enabled)) Then Return
					Dim content As String = e.actionCommand
					Dim [mod] As Integer = e.modifiers
					If (content IsNot Nothing) AndAlso (content.Length > 0) Then
						Dim isPrintableMask As Boolean = True
						Dim tk As Toolkit = Toolkit.defaultToolkit
						If TypeOf tk Is sun.awt.SunToolkit Then isPrintableMask = CType(tk, sun.awt.SunToolkit).isPrintableCharacterModifiersMask([mod])

						If isPrintableMask Then
							Dim c As Char = content.Chars(0)
							If (c >= &H20) AndAlso (AscW(c) <> &H7F) Then target.replaceSelection(content)
						End If
					End If
				End If
			End Sub
		End Class

		''' <summary>
		''' Places content into the associated document.
		''' If there is a selection, it is removed before
		''' the new content is added.
		''' <p>
		''' <strong>Warning:</strong>
		''' Serialized objects of this class will not be compatible with
		''' future Swing releases. The current serialization support is
		''' appropriate for short term storage or RMI between applications running
		''' the same version of Swing.  As of 1.4, support for long term storage
		''' of all JavaBeans&trade;
		''' has been added to the <code>java.beans</code> package.
		''' Please see <seealso cref="java.beans.XMLEncoder"/>.
		''' </summary>
		''' <seealso cref= DefaultEditorKit#insertContentAction </seealso>
		''' <seealso cref= DefaultEditorKit#getActions </seealso>
		Public Class InsertContentAction
			Inherits TextAction

			''' <summary>
			''' Creates this object with the appropriate identifier.
			''' </summary>
			Public Sub New()
				MyBase.New(insertContentAction)
			End Sub

			''' <summary>
			''' The operation to perform when this action is triggered.
			''' </summary>
			''' <param name="e"> the action event </param>
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				Dim target As JTextComponent = getTextComponent(e)
				If (target IsNot Nothing) AndAlso (e IsNot Nothing) Then
					If ((Not target.editable)) OrElse ((Not target.enabled)) Then
						javax.swing.UIManager.lookAndFeel.provideErrorFeedback(target)
						Return
					End If
					Dim content As String = e.actionCommand
					If content IsNot Nothing Then
						target.replaceSelection(content)
					Else
						javax.swing.UIManager.lookAndFeel.provideErrorFeedback(target)
					End If
				End If
			End Sub
		End Class

		''' <summary>
		''' Places a line/paragraph break into the document.
		''' If there is a selection, it is removed before
		''' the break is added.
		''' <p>
		''' <strong>Warning:</strong>
		''' Serialized objects of this class will not be compatible with
		''' future Swing releases. The current serialization support is
		''' appropriate for short term storage or RMI between applications running
		''' the same version of Swing.  As of 1.4, support for long term storage
		''' of all JavaBeans&trade;
		''' has been added to the <code>java.beans</code> package.
		''' Please see <seealso cref="java.beans.XMLEncoder"/>.
		''' </summary>
		''' <seealso cref= DefaultEditorKit#insertBreakAction </seealso>
		''' <seealso cref= DefaultEditorKit#getActions </seealso>
		Public Class InsertBreakAction
			Inherits TextAction

			''' <summary>
			''' Creates this object with the appropriate identifier.
			''' </summary>
			Public Sub New()
				MyBase.New(insertBreakAction)
			End Sub

			''' <summary>
			''' The operation to perform when this action is triggered.
			''' </summary>
			''' <param name="e"> the action event </param>
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				Dim target As JTextComponent = getTextComponent(e)
				If target IsNot Nothing Then
					If ((Not target.editable)) OrElse ((Not target.enabled)) Then
						javax.swing.UIManager.lookAndFeel.provideErrorFeedback(target)
						Return
					End If
					target.replaceSelection(vbLf)
				End If
			End Sub
		End Class

		''' <summary>
		''' Places a tab character into the document. If there
		''' is a selection, it is removed before the tab is added.
		''' <p>
		''' <strong>Warning:</strong>
		''' Serialized objects of this class will not be compatible with
		''' future Swing releases. The current serialization support is
		''' appropriate for short term storage or RMI between applications running
		''' the same version of Swing.  As of 1.4, support for long term storage
		''' of all JavaBeans&trade;
		''' has been added to the <code>java.beans</code> package.
		''' Please see <seealso cref="java.beans.XMLEncoder"/>.
		''' </summary>
		''' <seealso cref= DefaultEditorKit#insertTabAction </seealso>
		''' <seealso cref= DefaultEditorKit#getActions </seealso>
		Public Class InsertTabAction
			Inherits TextAction

			''' <summary>
			''' Creates this object with the appropriate identifier.
			''' </summary>
			Public Sub New()
				MyBase.New(insertTabAction)
			End Sub

			''' <summary>
			''' The operation to perform when this action is triggered.
			''' </summary>
			''' <param name="e"> the action event </param>
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				Dim target As JTextComponent = getTextComponent(e)
				If target IsNot Nothing Then
					If ((Not target.editable)) OrElse ((Not target.enabled)) Then
						javax.swing.UIManager.lookAndFeel.provideErrorFeedback(target)
						Return
					End If
					target.replaceSelection(vbTab)
				End If
			End Sub
		End Class

	'    
	'     * Deletes the character of content that precedes the
	'     * current caret position.
	'     * @see DefaultEditorKit#deletePrevCharAction
	'     * @see DefaultEditorKit#getActions
	'     
		Friend Class DeletePrevCharAction
			Inherits TextAction

			''' <summary>
			''' Creates this object with the appropriate identifier.
			''' </summary>
			Friend Sub New()
				MyBase.New(deletePrevCharAction)
			End Sub

			''' <summary>
			''' The operation to perform when this action is triggered.
			''' </summary>
			''' <param name="e"> the action event </param>
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				Dim target As JTextComponent = getTextComponent(e)
				Dim beep As Boolean = True
				If (target IsNot Nothing) AndAlso (target.editable) Then
					Try
						Dim doc As Document = target.document
						Dim caret As Caret = target.caret
						Dim dot As Integer = caret.dot
						Dim mark As Integer = caret.mark
						If dot <> mark Then
							doc.remove(Math.Min(dot, mark), Math.Abs(dot - mark))
							beep = False
						ElseIf dot > 0 Then
							Dim delChars As Integer = 1

							If dot > 1 Then
								Dim dotChars As String = doc.getText(dot - 2, 2)
								Dim c0 As Char = dotChars.Chars(0)
								Dim c1 As Char = dotChars.Chars(1)

								If c0 >= ChrW(&HD800) AndAlso c0 <= ChrW(&HDBFF) AndAlso c1 >= ChrW(&HDC00) AndAlso c1 <= ChrW(&HDFFF) Then delChars = 2
							End If

							doc.remove(dot - delChars, delChars)
							beep = False
						End If
					Catch bl As BadLocationException
					End Try
				End If
				If beep Then javax.swing.UIManager.lookAndFeel.provideErrorFeedback(target)
			End Sub
		End Class

	'    
	'     * Deletes the character of content that follows the
	'     * current caret position.
	'     * @see DefaultEditorKit#deleteNextCharAction
	'     * @see DefaultEditorKit#getActions
	'     
		Friend Class DeleteNextCharAction
			Inherits TextAction

			' Create this object with the appropriate identifier. 
			Friend Sub New()
				MyBase.New(deleteNextCharAction)
			End Sub

			''' <summary>
			''' The operation to perform when this action is triggered. </summary>
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				Dim target As JTextComponent = getTextComponent(e)
				Dim beep As Boolean = True
				If (target IsNot Nothing) AndAlso (target.editable) Then
					Try
						Dim doc As Document = target.document
						Dim caret As Caret = target.caret
						Dim dot As Integer = caret.dot
						Dim mark As Integer = caret.mark
						If dot <> mark Then
							doc.remove(Math.Min(dot, mark), Math.Abs(dot - mark))
							beep = False
						ElseIf dot < doc.length Then
							Dim delChars As Integer = 1

							If dot < doc.length - 1 Then
								Dim dotChars As String = doc.getText(dot, 2)
								Dim c0 As Char = dotChars.Chars(0)
								Dim c1 As Char = dotChars.Chars(1)

								If c0 >= ChrW(&HD800) AndAlso c0 <= ChrW(&HDBFF) AndAlso c1 >= ChrW(&HDC00) AndAlso c1 <= ChrW(&HDFFF) Then delChars = 2
							End If

							doc.remove(dot, delChars)
							beep = False
						End If
					Catch bl As BadLocationException
					End Try
				End If
				If beep Then javax.swing.UIManager.lookAndFeel.provideErrorFeedback(target)
			End Sub
		End Class


	'    
	'     * Deletes the word that precedes/follows the beginning of the selection.
	'     * @see DefaultEditorKit#getActions
	'     
		Friend Class DeleteWordAction
			Inherits TextAction

			Friend Sub New(ByVal name As String)
				MyBase.New(name)
				assert(name = deletePrevWordAction) OrElse (name = deleteNextWordAction)
			End Sub
			''' <summary>
			''' The operation to perform when this action is triggered.
			''' </summary>
			''' <param name="e"> the action event </param>
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				Dim target As JTextComponent = getTextComponent(e)
				If (target IsNot Nothing) AndAlso (e IsNot Nothing) Then
					If ((Not target.editable)) OrElse ((Not target.enabled)) Then
						javax.swing.UIManager.lookAndFeel.provideErrorFeedback(target)
						Return
					End If
					Dim beep As Boolean = True
					Try
						Dim start As Integer = target.selectionStart
						Dim line As Element = Utilities.getParagraphElement(target, start)
						Dim [end] As Integer
						If deleteNextWordAction Is getValue(javax.swing.Action.NAME) Then
							[end] = Utilities.getNextWordInParagraph(target, line, start, False)
							If [end] = java.text.BreakIterator.DONE Then
								'last word in the paragraph
								Dim endOfLine As Integer = line.endOffset
								If start = endOfLine - 1 Then
									'for last position remove last \n
									[end] = endOfLine
								Else
									'remove to the end of the paragraph
									[end] = endOfLine - 1
								End If
							End If
						Else
							[end] = Utilities.getPrevWordInParagraph(target, line, start)
							If [end] = java.text.BreakIterator.DONE Then
								'there is no previous word in the paragraph
								Dim startOfLine As Integer = line.startOffset
								If start = startOfLine Then
									'for first position remove previous \n
									[end] = startOfLine - 1
								Else
									'remove to the start of the paragraph
									[end] = startOfLine
								End If
							End If
						End If
						Dim offs As Integer = Math.Min(start, [end])
						Dim len As Integer = Math.Abs([end] - start)
						If offs >= 0 Then
							target.document.remove(offs, len)
							beep = False
						End If
					Catch ignore As BadLocationException
					End Try
					If beep Then javax.swing.UIManager.lookAndFeel.provideErrorFeedback(target)
				End If
			End Sub
		End Class


	'    
	'     * Sets the editor into read-only mode.
	'     * @see DefaultEditorKit#readOnlyAction
	'     * @see DefaultEditorKit#getActions
	'     
		Friend Class ReadOnlyAction
			Inherits TextAction

			' Create this object with the appropriate identifier. 
			Friend Sub New()
				MyBase.New(readOnlyAction)
			End Sub

			''' <summary>
			''' The operation to perform when this action is triggered.
			''' </summary>
			''' <param name="e"> the action event </param>
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				Dim target As JTextComponent = getTextComponent(e)
				If target IsNot Nothing Then target.editable = False
			End Sub
		End Class

	'    
	'     * Sets the editor into writeable mode.
	'     * @see DefaultEditorKit#writableAction
	'     * @see DefaultEditorKit#getActions
	'     
		Friend Class WritableAction
			Inherits TextAction

			' Create this object with the appropriate identifier. 
			Friend Sub New()
				MyBase.New(writableAction)
			End Sub

			''' <summary>
			''' The operation to perform when this action is triggered.
			''' </summary>
			''' <param name="e"> the action event </param>
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				Dim target As JTextComponent = getTextComponent(e)
				If target IsNot Nothing Then target.editable = True
			End Sub
		End Class

		''' <summary>
		''' Cuts the selected region and place its contents
		''' into the system clipboard.
		''' <p>
		''' <strong>Warning:</strong>
		''' Serialized objects of this class will not be compatible with
		''' future Swing releases. The current serialization support is
		''' appropriate for short term storage or RMI between applications running
		''' the same version of Swing.  As of 1.4, support for long term storage
		''' of all JavaBeans&trade;
		''' has been added to the <code>java.beans</code> package.
		''' Please see <seealso cref="java.beans.XMLEncoder"/>.
		''' </summary>
		''' <seealso cref= DefaultEditorKit#cutAction </seealso>
		''' <seealso cref= DefaultEditorKit#getActions </seealso>
		Public Class CutAction
			Inherits TextAction

			''' <summary>
			''' Create this object with the appropriate identifier. </summary>
			Public Sub New()
				MyBase.New(cutAction)
			End Sub

			''' <summary>
			''' The operation to perform when this action is triggered.
			''' </summary>
			''' <param name="e"> the action event </param>
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				Dim target As JTextComponent = getTextComponent(e)
				If target IsNot Nothing Then target.cut()
			End Sub
		End Class

		''' <summary>
		''' Copies the selected region and place its contents
		''' into the system clipboard.
		''' <p>
		''' <strong>Warning:</strong>
		''' Serialized objects of this class will not be compatible with
		''' future Swing releases. The current serialization support is
		''' appropriate for short term storage or RMI between applications running
		''' the same version of Swing.  As of 1.4, support for long term storage
		''' of all JavaBeans&trade;
		''' has been added to the <code>java.beans</code> package.
		''' Please see <seealso cref="java.beans.XMLEncoder"/>.
		''' </summary>
		''' <seealso cref= DefaultEditorKit#copyAction </seealso>
		''' <seealso cref= DefaultEditorKit#getActions </seealso>
		Public Class CopyAction
			Inherits TextAction

			''' <summary>
			''' Create this object with the appropriate identifier. </summary>
			Public Sub New()
				MyBase.New(copyAction)
			End Sub

			''' <summary>
			''' The operation to perform when this action is triggered.
			''' </summary>
			''' <param name="e"> the action event </param>
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				Dim target As JTextComponent = getTextComponent(e)
				If target IsNot Nothing Then target.copy()
			End Sub
		End Class

		''' <summary>
		''' Pastes the contents of the system clipboard into the
		''' selected region, or before the caret if nothing is
		''' selected.
		''' <p>
		''' <strong>Warning:</strong>
		''' Serialized objects of this class will not be compatible with
		''' future Swing releases. The current serialization support is
		''' appropriate for short term storage or RMI between applications running
		''' the same version of Swing.  As of 1.4, support for long term storage
		''' of all JavaBeans&trade;
		''' has been added to the <code>java.beans</code> package.
		''' Please see <seealso cref="java.beans.XMLEncoder"/>.
		''' </summary>
		''' <seealso cref= DefaultEditorKit#pasteAction </seealso>
		''' <seealso cref= DefaultEditorKit#getActions </seealso>
		Public Class PasteAction
			Inherits TextAction

			''' <summary>
			''' Create this object with the appropriate identifier. </summary>
			Public Sub New()
				MyBase.New(pasteAction)
			End Sub

			''' <summary>
			''' The operation to perform when this action is triggered.
			''' </summary>
			''' <param name="e"> the action event </param>
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				Dim target As JTextComponent = getTextComponent(e)
				If target IsNot Nothing Then target.paste()
			End Sub
		End Class

		''' <summary>
		''' Creates a beep.
		''' <p>
		''' <strong>Warning:</strong>
		''' Serialized objects of this class will not be compatible with
		''' future Swing releases. The current serialization support is
		''' appropriate for short term storage or RMI between applications running
		''' the same version of Swing.  As of 1.4, support for long term storage
		''' of all JavaBeans&trade;
		''' has been added to the <code>java.beans</code> package.
		''' Please see <seealso cref="java.beans.XMLEncoder"/>.
		''' </summary>
		''' <seealso cref= DefaultEditorKit#beepAction </seealso>
		''' <seealso cref= DefaultEditorKit#getActions </seealso>
		Public Class BeepAction
			Inherits TextAction

			''' <summary>
			''' Create this object with the appropriate identifier. </summary>
			Public Sub New()
				MyBase.New(beepAction)
			End Sub

			''' <summary>
			''' The operation to perform when this action is triggered.
			''' </summary>
			''' <param name="e"> the action event </param>
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				Dim target As JTextComponent = getTextComponent(e)
				javax.swing.UIManager.lookAndFeel.provideErrorFeedback(target)
			End Sub
		End Class

		''' <summary>
		''' Scrolls up/down vertically.  The select version of this action extends
		''' the selection, instead of simply moving the caret.
		''' </summary>
		''' <seealso cref= DefaultEditorKit#pageUpAction </seealso>
		''' <seealso cref= DefaultEditorKit#pageDownAction </seealso>
		''' <seealso cref= DefaultEditorKit#getActions </seealso>
		Friend Class VerticalPageAction
			Inherits TextAction

			''' <summary>
			''' Create this object with the appropriate identifier. </summary>
			Public Sub New(ByVal nm As String, ByVal direction As Integer, ByVal [select] As Boolean)
				MyBase.New(nm)
				Me.select = [select]
				Me.direction = direction
			End Sub

			''' <summary>
			''' The operation to perform when this action is triggered. </summary>
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				Dim target As JTextComponent = getTextComponent(e)
				If target IsNot Nothing Then
					Dim visible As Rectangle = target.visibleRect
					Dim newVis As New Rectangle(visible)
					Dim selectedIndex As Integer = target.caretPosition
					Dim scrollAmount As Integer = direction * target.getScrollableBlockIncrement(visible, javax.swing.SwingConstants.VERTICAL, direction)
					Dim initialY As Integer = visible.y
					Dim caret As Caret = target.caret
					Dim magicPosition As Point = caret.magicCaretPosition

					If selectedIndex <> -1 Then
						Try
							Dim dotBounds As Rectangle = target.modelToView(selectedIndex)
							Dim x As Integer = If(magicPosition IsNot Nothing, magicPosition.x, dotBounds.x)
							Dim h As Integer = dotBounds.height
							If h > 0 Then scrollAmount = scrollAmount \ h * h
							newVis.y = constrainY(target, initialY + scrollAmount, visible.height)

							Dim newIndex As Integer

							If visible.contains(dotBounds.x, dotBounds.y) Then
								' Dot is currently visible, base the new
								' location off the old, or
								newIndex = target.viewToModel(New Point(x, constrainY(target, dotBounds.y + scrollAmount, 0)))
							Else
								' Dot isn't visible, choose the top or the bottom
								' for the new location.
								If direction = -1 Then
									newIndex = target.viewToModel(New Point(x, newVis.y))
								Else
									newIndex = target.viewToModel(New Point(x, newVis.y + visible.height))
								End If
							End If
							newIndex = constrainOffset(target, newIndex)
							If newIndex <> selectedIndex Then
								' Make sure the new visible location contains
								' the location of dot, otherwise Caret will
								' cause an additional scroll.
								Dim newY As Integer = getAdjustedY(target, newVis, newIndex)

								If direction = -1 AndAlso newY <= initialY OrElse direction = 1 AndAlso newY >= initialY Then
									' Change index and correct newVis.y only if won't cause scrolling upward
									newVis.y = newY

									If [select] Then
										target.moveCaretPosition(newIndex)
									Else
										target.caretPosition = newIndex
									End If
								End If
							End If
						Catch ble As BadLocationException
						End Try
					Else
						newVis.y = constrainY(target, initialY + scrollAmount, visible.height)
					End If
					If magicPosition IsNot Nothing Then caret.magicCaretPosition = magicPosition
					target.scrollRectToVisible(newVis)
				End If
			End Sub

			''' <summary>
			''' Makes sure <code>y</code> is a valid location in
			''' <code>target</code>.
			''' </summary>
			Private Function constrainY(ByVal target As JTextComponent, ByVal y As Integer, ByVal vis As Integer) As Integer
				If y < 0 Then
					y = 0
				ElseIf y + vis > target.height Then
					y = Math.Max(0, target.height - vis)
				End If
				Return y
			End Function

			''' <summary>
			''' Ensures that <code>offset</code> is a valid offset into the
			''' model for <code>text</code>.
			''' </summary>
			Private Function constrainOffset(ByVal text As JTextComponent, ByVal offset As Integer) As Integer
				Dim doc As Document = text.document

				If (offset <> 0) AndAlso (offset > doc.length) Then offset = doc.length
				If offset < 0 Then offset = 0
				Return offset
			End Function

			''' <summary>
			''' Returns adjustsed {@code y} position that indicates the location to scroll to
			''' after selecting <code>index</code>.
			''' </summary>
			Private Function getAdjustedY(ByVal text As JTextComponent, ByVal visible As Rectangle, ByVal index As Integer) As Integer
				Dim result As Integer = visible.y

				Try
					Dim dotBounds As Rectangle = text.modelToView(index)

					If dotBounds.y < visible.y Then
						result = dotBounds.y
					Else
						If (dotBounds.y > visible.y + visible.height) OrElse (dotBounds.y + dotBounds.height > visible.y + visible.height) Then result = dotBounds.y + dotBounds.height - visible.height
					End If
				Catch ble As BadLocationException
				End Try

				Return result
			End Function

			''' <summary>
			''' Adjusts the Rectangle to contain the bounds of the character at
			''' <code>index</code> in response to a page up.
			''' </summary>
			Private [select] As Boolean

			''' <summary>
			''' Direction to scroll, 1 is down, -1 is up.
			''' </summary>
			Private direction As Integer
		End Class


		''' <summary>
		''' Pages one view to the left or right.
		''' </summary>
		Friend Class PageAction
			Inherits TextAction

			''' <summary>
			''' Create this object with the appropriate identifier. </summary>
			Public Sub New(ByVal nm As String, ByVal left As Boolean, ByVal [select] As Boolean)
				MyBase.New(nm)
				Me.select = [select]
				Me.left = left
			End Sub

			''' <summary>
			''' The operation to perform when this action is triggered. </summary>
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				Dim target As JTextComponent = getTextComponent(e)
				If target IsNot Nothing Then
					Dim selectedIndex As Integer
					Dim visible As New Rectangle
					target.computeVisibleRect(visible)
					If left Then
						visible.x = Math.Max(0, visible.x - visible.width)
					Else
						visible.x += visible.width
					End If

					selectedIndex = target.caretPosition
					If selectedIndex <> -1 Then
						If left Then
							selectedIndex = target.viewToModel(New Point(visible.x, visible.y))
						Else
							selectedIndex = target.viewToModel(New Point(visible.x + visible.width - 1, visible.y + visible.height - 1))
						End If
						Dim doc As Document = target.document
						If (selectedIndex <> 0) AndAlso (selectedIndex > (doc.length-1)) Then
							selectedIndex = doc.length-1
						ElseIf selectedIndex < 0 Then
							selectedIndex = 0
						End If
						If [select] Then
							target.moveCaretPosition(selectedIndex)
						Else
							target.caretPosition = selectedIndex
						End If
					End If
				End If
			End Sub

			Private [select] As Boolean
			Private left As Boolean
		End Class

		Friend Class DumpModelAction
			Inherits TextAction

			Friend Sub New()
				MyBase.New("dump-model")
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				Dim target As JTextComponent = getTextComponent(e)
				If target IsNot Nothing Then
					Dim d As Document = target.document
					If TypeOf d Is AbstractDocument Then CType(d, AbstractDocument).dump(System.err)
				End If
			End Sub
		End Class

	'    
	'     * Action to move the selection by way of the
	'     * getNextVisualPositionFrom method. Constructor indicates direction
	'     * to use.
	'     
		Friend Class NextVisualPositionAction
			Inherits TextAction

			''' <summary>
			''' Create this action with the appropriate identifier. </summary>
			''' <param name="nm">  the name of the action, Action.NAME. </param>
			''' <param name="select"> whether to extend the selection when
			'''  changing the caret position. </param>
			Friend Sub New(ByVal nm As String, ByVal [select] As Boolean, ByVal direction As Integer)
				MyBase.New(nm)
				Me.select = [select]
				Me.direction = direction
			End Sub

			''' <summary>
			''' The operation to perform when this action is triggered. </summary>
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				Dim target As JTextComponent = getTextComponent(e)
				If target IsNot Nothing Then
					Dim caret As Caret = target.caret
					Dim bidiCaret As DefaultCaret = If(TypeOf caret Is DefaultCaret, CType(caret, DefaultCaret), Nothing)
					Dim dot As Integer = caret.dot
					Dim bias As Position.Bias() = New Position.Bias(0){}
					Dim magicPosition As Point = caret.magicCaretPosition

					Try
						If magicPosition Is Nothing AndAlso (direction = javax.swing.SwingConstants.NORTH OrElse direction = javax.swing.SwingConstants.SOUTH) Then
							Dim r As Rectangle = If(bidiCaret IsNot Nothing, target.uI.modelToView(target, dot, bidiCaret.dotBias), target.modelToView(dot))
							magicPosition = New Point(r.x, r.y)
						End If

						Dim filter As NavigationFilter = target.navigationFilter

						If filter IsNot Nothing Then
							dot = filter.getNextVisualPositionFrom(target, dot,If(bidiCaret IsNot Nothing, bidiCaret.dotBias, Position.Bias.Forward), direction, bias)
						Else
							dot = target.uI.getNextVisualPositionFrom(target, dot,If(bidiCaret IsNot Nothing, bidiCaret.dotBias, Position.Bias.Forward), direction, bias)
						End If
						If bias(0) Is Nothing Then bias(0) = Position.Bias.Forward
						If bidiCaret IsNot Nothing Then
							If [select] Then
								bidiCaret.moveDot(dot, bias(0))
							Else
								bidiCaret.dotDot(dot, bias(0))
							End If
						Else
							If [select] Then
								caret.moveDot(dot)
							Else
								caret.dot = dot
							End If
						End If
						If magicPosition IsNot Nothing AndAlso (direction = javax.swing.SwingConstants.NORTH OrElse direction = javax.swing.SwingConstants.SOUTH) Then target.caret.magicCaretPosition = magicPosition
					Catch ex As BadLocationException
					End Try
				End If
			End Sub

			Private [select] As Boolean
			Private direction As Integer
		End Class

	'    
	'     * Position the caret to the beginning of the word.
	'     * @see DefaultEditorKit#beginWordAction
	'     * @see DefaultEditorKit#selectBeginWordAction
	'     * @see DefaultEditorKit#getActions
	'     
		Friend Class BeginWordAction
			Inherits TextAction

			''' <summary>
			''' Create this action with the appropriate identifier. </summary>
			''' <param name="nm">  the name of the action, Action.NAME. </param>
			''' <param name="select"> whether to extend the selection when
			'''  changing the caret position. </param>
			Friend Sub New(ByVal nm As String, ByVal [select] As Boolean)
				MyBase.New(nm)
				Me.select = [select]
			End Sub

			''' <summary>
			''' The operation to perform when this action is triggered. </summary>
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				Dim target As JTextComponent = getTextComponent(e)
				If target IsNot Nothing Then
					Try
						Dim offs As Integer = target.caretPosition
						Dim begOffs As Integer = Utilities.getWordStart(target, offs)
						If [select] Then
							target.moveCaretPosition(begOffs)
						Else
							target.caretPosition = begOffs
						End If
					Catch bl As BadLocationException
						javax.swing.UIManager.lookAndFeel.provideErrorFeedback(target)
					End Try
				End If
			End Sub

			Private [select] As Boolean
		End Class

	'    
	'     * Position the caret to the end of the word.
	'     * @see DefaultEditorKit#endWordAction
	'     * @see DefaultEditorKit#selectEndWordAction
	'     * @see DefaultEditorKit#getActions
	'     
		Friend Class EndWordAction
			Inherits TextAction

			''' <summary>
			''' Create this action with the appropriate identifier. </summary>
			''' <param name="nm">  the name of the action, Action.NAME. </param>
			''' <param name="select"> whether to extend the selection when
			'''  changing the caret position. </param>
			Friend Sub New(ByVal nm As String, ByVal [select] As Boolean)
				MyBase.New(nm)
				Me.select = [select]
			End Sub

			''' <summary>
			''' The operation to perform when this action is triggered. </summary>
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				Dim target As JTextComponent = getTextComponent(e)
				If target IsNot Nothing Then
					Try
						Dim offs As Integer = target.caretPosition
						Dim endOffs As Integer = Utilities.getWordEnd(target, offs)
						If [select] Then
							target.moveCaretPosition(endOffs)
						Else
							target.caretPosition = endOffs
						End If
					Catch bl As BadLocationException
						javax.swing.UIManager.lookAndFeel.provideErrorFeedback(target)
					End Try
				End If
			End Sub

			Private [select] As Boolean
		End Class

	'    
	'     * Position the caret to the beginning of the previous word.
	'     * @see DefaultEditorKit#previousWordAction
	'     * @see DefaultEditorKit#selectPreviousWordAction
	'     * @see DefaultEditorKit#getActions
	'     
		Friend Class PreviousWordAction
			Inherits TextAction

			''' <summary>
			''' Create this action with the appropriate identifier. </summary>
			''' <param name="nm">  the name of the action, Action.NAME. </param>
			''' <param name="select"> whether to extend the selection when
			'''  changing the caret position. </param>
			Friend Sub New(ByVal nm As String, ByVal [select] As Boolean)
				MyBase.New(nm)
				Me.select = [select]
			End Sub

			''' <summary>
			''' The operation to perform when this action is triggered. </summary>
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				Dim target As JTextComponent = getTextComponent(e)
				If target IsNot Nothing Then
					Dim offs As Integer = target.caretPosition
					Dim failed As Boolean = False
					Try
						Dim curPara As Element = Utilities.getParagraphElement(target, offs)
						offs = Utilities.getPreviousWord(target, offs)
						If offs < curPara.startOffset Then offs = Utilities.getParagraphElement(target, offs).endOffset - 1
					Catch bl As BadLocationException
						If offs <> 0 Then
							offs = 0
						Else
							failed = True
						End If
					End Try
					If Not failed Then
						If [select] Then
							target.moveCaretPosition(offs)
						Else
							target.caretPosition = offs
						End If
					Else
						javax.swing.UIManager.lookAndFeel.provideErrorFeedback(target)
					End If
				End If
			End Sub

			Private [select] As Boolean
		End Class

	'    
	'     * Position the caret to the next of the word.
	'     * @see DefaultEditorKit#nextWordAction
	'     * @see DefaultEditorKit#selectNextWordAction
	'     * @see DefaultEditorKit#getActions
	'     
		Friend Class NextWordAction
			Inherits TextAction

			''' <summary>
			''' Create this action with the appropriate identifier. </summary>
			''' <param name="nm">  the name of the action, Action.NAME. </param>
			''' <param name="select"> whether to extend the selection when
			'''  changing the caret position. </param>
			Friend Sub New(ByVal nm As String, ByVal [select] As Boolean)
				MyBase.New(nm)
				Me.select = [select]
			End Sub

			''' <summary>
			''' The operation to perform when this action is triggered. </summary>
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				Dim target As JTextComponent = getTextComponent(e)
				If target IsNot Nothing Then
					Dim offs As Integer = target.caretPosition
					Dim failed As Boolean = False
					Dim oldOffs As Integer = offs
					Dim curPara As Element = Utilities.getParagraphElement(target, offs)
					Try
						offs = Utilities.getNextWord(target, offs)
						If offs >= curPara.endOffset AndAlso oldOffs <> curPara.endOffset - 1 Then offs = curPara.endOffset - 1
					Catch bl As BadLocationException
						Dim [end] As Integer = target.document.length
						If offs <> [end] Then
							If oldOffs <> curPara.endOffset - 1 Then
								offs = curPara.endOffset - 1
							Else
							offs = [end]
							End If
						Else
							failed = True
						End If
					End Try
					If Not failed Then
						If [select] Then
							target.moveCaretPosition(offs)
						Else
							target.caretPosition = offs
						End If
					Else
						javax.swing.UIManager.lookAndFeel.provideErrorFeedback(target)
					End If
				End If
			End Sub

			Private [select] As Boolean
		End Class

	'    
	'     * Position the caret to the beginning of the line.
	'     * @see DefaultEditorKit#beginLineAction
	'     * @see DefaultEditorKit#selectBeginLineAction
	'     * @see DefaultEditorKit#getActions
	'     
		Friend Class BeginLineAction
			Inherits TextAction

			''' <summary>
			''' Create this action with the appropriate identifier. </summary>
			''' <param name="nm">  the name of the action, Action.NAME. </param>
			''' <param name="select"> whether to extend the selection when
			'''  changing the caret position. </param>
			Friend Sub New(ByVal nm As String, ByVal [select] As Boolean)
				MyBase.New(nm)
				Me.select = [select]
			End Sub

			''' <summary>
			''' The operation to perform when this action is triggered. </summary>
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				Dim target As JTextComponent = getTextComponent(e)
				If target IsNot Nothing Then
					Try
						Dim offs As Integer = target.caretPosition
						Dim begOffs As Integer = Utilities.getRowStart(target, offs)
						If [select] Then
							target.moveCaretPosition(begOffs)
						Else
							target.caretPosition = begOffs
						End If
					Catch bl As BadLocationException
						javax.swing.UIManager.lookAndFeel.provideErrorFeedback(target)
					End Try
				End If
			End Sub

			Private [select] As Boolean
		End Class

	'    
	'     * Position the caret to the end of the line.
	'     * @see DefaultEditorKit#endLineAction
	'     * @see DefaultEditorKit#selectEndLineAction
	'     * @see DefaultEditorKit#getActions
	'     
		Friend Class EndLineAction
			Inherits TextAction

			''' <summary>
			''' Create this action with the appropriate identifier. </summary>
			''' <param name="nm">  the name of the action, Action.NAME. </param>
			''' <param name="select"> whether to extend the selection when
			'''  changing the caret position. </param>
			Friend Sub New(ByVal nm As String, ByVal [select] As Boolean)
				MyBase.New(nm)
				Me.select = [select]
			End Sub

			''' <summary>
			''' The operation to perform when this action is triggered. </summary>
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				Dim target As JTextComponent = getTextComponent(e)
				If target IsNot Nothing Then
					Try
						Dim offs As Integer = target.caretPosition
						Dim endOffs As Integer = Utilities.getRowEnd(target, offs)
						If [select] Then
							target.moveCaretPosition(endOffs)
						Else
							target.caretPosition = endOffs
						End If
					Catch bl As BadLocationException
						javax.swing.UIManager.lookAndFeel.provideErrorFeedback(target)
					End Try
				End If
			End Sub

			Private [select] As Boolean
		End Class

	'    
	'     * Position the caret to the beginning of the paragraph.
	'     * @see DefaultEditorKit#beginParagraphAction
	'     * @see DefaultEditorKit#selectBeginParagraphAction
	'     * @see DefaultEditorKit#getActions
	'     
		Friend Class BeginParagraphAction
			Inherits TextAction

			''' <summary>
			''' Create this action with the appropriate identifier. </summary>
			''' <param name="nm">  the name of the action, Action.NAME. </param>
			''' <param name="select"> whether to extend the selection when
			'''  changing the caret position. </param>
			Friend Sub New(ByVal nm As String, ByVal [select] As Boolean)
				MyBase.New(nm)
				Me.select = [select]
			End Sub

			''' <summary>
			''' The operation to perform when this action is triggered. </summary>
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				Dim target As JTextComponent = getTextComponent(e)
				If target IsNot Nothing Then
					Dim offs As Integer = target.caretPosition
					Dim elem As Element = Utilities.getParagraphElement(target, offs)
					offs = elem.startOffset
					If [select] Then
						target.moveCaretPosition(offs)
					Else
						target.caretPosition = offs
					End If
				End If
			End Sub

			Private [select] As Boolean
		End Class

	'    
	'     * Position the caret to the end of the paragraph.
	'     * @see DefaultEditorKit#endParagraphAction
	'     * @see DefaultEditorKit#selectEndParagraphAction
	'     * @see DefaultEditorKit#getActions
	'     
		Friend Class EndParagraphAction
			Inherits TextAction

			''' <summary>
			''' Create this action with the appropriate identifier. </summary>
			''' <param name="nm">  the name of the action, Action.NAME. </param>
			''' <param name="select"> whether to extend the selection when
			'''  changing the caret position. </param>
			Friend Sub New(ByVal nm As String, ByVal [select] As Boolean)
				MyBase.New(nm)
				Me.select = [select]
			End Sub

			''' <summary>
			''' The operation to perform when this action is triggered. </summary>
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				Dim target As JTextComponent = getTextComponent(e)
				If target IsNot Nothing Then
					Dim offs As Integer = target.caretPosition
					Dim elem As Element = Utilities.getParagraphElement(target, offs)
					offs = Math.Min(target.document.length, elem.endOffset)
					If [select] Then
						target.moveCaretPosition(offs)
					Else
						target.caretPosition = offs
					End If
				End If
			End Sub

			Private [select] As Boolean
		End Class

	'    
	'     * Move the caret to the beginning of the document.
	'     * @see DefaultEditorKit#beginAction
	'     * @see DefaultEditorKit#getActions
	'     
		Friend Class BeginAction
			Inherits TextAction

			' Create this object with the appropriate identifier. 
			Friend Sub New(ByVal nm As String, ByVal [select] As Boolean)
				MyBase.New(nm)
				Me.select = [select]
			End Sub

			''' <summary>
			''' The operation to perform when this action is triggered. </summary>
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				Dim target As JTextComponent = getTextComponent(e)
				If target IsNot Nothing Then
					If [select] Then
						target.moveCaretPosition(0)
					Else
						target.caretPosition = 0
					End If
				End If
			End Sub

			Private [select] As Boolean
		End Class

	'    
	'     * Move the caret to the end of the document.
	'     * @see DefaultEditorKit#endAction
	'     * @see DefaultEditorKit#getActions
	'     
		Friend Class EndAction
			Inherits TextAction

			' Create this object with the appropriate identifier. 
			Friend Sub New(ByVal nm As String, ByVal [select] As Boolean)
				MyBase.New(nm)
				Me.select = [select]
			End Sub

			''' <summary>
			''' The operation to perform when this action is triggered. </summary>
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				Dim target As JTextComponent = getTextComponent(e)
				If target IsNot Nothing Then
					Dim doc As Document = target.document
					Dim dot As Integer = doc.length
					If [select] Then
						target.moveCaretPosition(dot)
					Else
						target.caretPosition = dot
					End If
				End If
			End Sub

			Private [select] As Boolean
		End Class

	'    
	'     * Select the word around the caret
	'     * @see DefaultEditorKit#endAction
	'     * @see DefaultEditorKit#getActions
	'     
		Friend Class SelectWordAction
			Inherits TextAction

			''' <summary>
			''' Create this action with the appropriate identifier. </summary>
			''' <param name="nm">  the name of the action, Action.NAME. </param>
			''' <param name="select"> whether to extend the selection when
			'''  changing the caret position. </param>
			Friend Sub New()
				MyBase.New(selectWordAction)
				start = New BeginWordAction("pigdog", False)
				[end] = New EndWordAction("pigdog", True)
			End Sub

			''' <summary>
			''' The operation to perform when this action is triggered. </summary>
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				start.actionPerformed(e)
				[end].actionPerformed(e)
			End Sub

			Private start As javax.swing.Action
			Private [end] As javax.swing.Action
		End Class

	'    
	'     * Select the line around the caret
	'     * @see DefaultEditorKit#endAction
	'     * @see DefaultEditorKit#getActions
	'     
		Friend Class SelectLineAction
			Inherits TextAction

			''' <summary>
			''' Create this action with the appropriate identifier. </summary>
			''' <param name="nm">  the name of the action, Action.NAME. </param>
			''' <param name="select"> whether to extend the selection when
			'''  changing the caret position. </param>
			Friend Sub New()
				MyBase.New(selectLineAction)
				start = New BeginLineAction("pigdog", False)
				[end] = New EndLineAction("pigdog", True)
			End Sub

			''' <summary>
			''' The operation to perform when this action is triggered. </summary>
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				start.actionPerformed(e)
				[end].actionPerformed(e)
			End Sub

			Private start As javax.swing.Action
			Private [end] As javax.swing.Action
		End Class

	'    
	'     * Select the paragraph around the caret
	'     * @see DefaultEditorKit#endAction
	'     * @see DefaultEditorKit#getActions
	'     
		Friend Class SelectParagraphAction
			Inherits TextAction

			''' <summary>
			''' Create this action with the appropriate identifier. </summary>
			''' <param name="nm">  the name of the action, Action.NAME. </param>
			''' <param name="select"> whether to extend the selection when
			'''  changing the caret position. </param>
			Friend Sub New()
				MyBase.New(selectParagraphAction)
				start = New BeginParagraphAction("pigdog", False)
				[end] = New EndParagraphAction("pigdog", True)
			End Sub

			''' <summary>
			''' The operation to perform when this action is triggered. </summary>
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				start.actionPerformed(e)
				[end].actionPerformed(e)
			End Sub

			Private start As javax.swing.Action
			Private [end] As javax.swing.Action
		End Class

	'    
	'     * Select the entire document
	'     * @see DefaultEditorKit#endAction
	'     * @see DefaultEditorKit#getActions
	'     
		Friend Class SelectAllAction
			Inherits TextAction

			''' <summary>
			''' Create this action with the appropriate identifier. </summary>
			''' <param name="nm">  the name of the action, Action.NAME. </param>
			''' <param name="select"> whether to extend the selection when
			'''  changing the caret position. </param>
			Friend Sub New()
				MyBase.New(selectAllAction)
			End Sub

			''' <summary>
			''' The operation to perform when this action is triggered. </summary>
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				Dim target As JTextComponent = getTextComponent(e)
				If target IsNot Nothing Then
					Dim doc As Document = target.document
					target.caretPosition = 0
					target.moveCaretPosition(doc.length)
				End If
			End Sub

		End Class

	'    
	'     * Remove the selection, if any.
	'     * @see DefaultEditorKit#unselectAction
	'     * @see DefaultEditorKit#getActions
	'     
		Friend Class UnselectAction
			Inherits TextAction

			''' <summary>
			''' Create this action with the appropriate identifier.
			''' </summary>
			Friend Sub New()
				MyBase.New(unselectAction)
			End Sub

			''' <summary>
			''' The operation to perform when this action is triggered. </summary>
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				Dim target As JTextComponent = getTextComponent(e)
				If target IsNot Nothing Then target.caretPosition = target.caretPosition
			End Sub

		End Class

	'    
	'     * Toggles the ComponentOrientation of the text component.
	'     * @see DefaultEditorKit#toggleComponentOrientationAction
	'     * @see DefaultEditorKit#getActions
	'     
		Friend Class ToggleComponentOrientationAction
			Inherits TextAction

			''' <summary>
			''' Create this action with the appropriate identifier.
			''' </summary>
			Friend Sub New()
				MyBase.New(toggleComponentOrientationAction)
			End Sub

			''' <summary>
			''' The operation to perform when this action is triggered. </summary>
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				Dim target As JTextComponent = getTextComponent(e)
				If target IsNot Nothing Then
					Dim last As ComponentOrientation = target.componentOrientation
					Dim [next] As ComponentOrientation
					If last Is ComponentOrientation.RIGHT_TO_LEFT Then
						[next] = ComponentOrientation.LEFT_TO_RIGHT
					Else
						[next] = ComponentOrientation.RIGHT_TO_LEFT
					End If
					target.componentOrientation = [next]
					target.repaint()
				End If
			End Sub
		End Class

	End Class

End Namespace