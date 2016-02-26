Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Text
Imports javax.swing.event
import static sun.swing.SwingUtilities2.IMPLIED_CR

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
	''' A document that can be marked up with character and paragraph
	''' styles in a manner similar to the Rich Text Format.  The element
	''' structure for this document represents style crossings for
	''' style runs.  These style runs are mapped into a paragraph element
	''' structure (which may reside in some other structure).  The
	''' style runs break at paragraph boundaries since logical styles are
	''' assigned to paragraph boundaries.
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' 
	''' @author  Timothy Prinzing </summary>
	''' <seealso cref=     Document </seealso>
	''' <seealso cref=     AbstractDocument </seealso>
	Public Class DefaultStyledDocument
		Inherits AbstractDocument
		Implements StyledDocument

		''' <summary>
		''' Constructs a styled document.
		''' </summary>
		''' <param name="c">  the container for the content </param>
		''' <param name="styles"> resources and style definitions which may
		'''  be shared across documents </param>
		Public Sub New(ByVal c As Content, ByVal styles As StyleContext)
			MyBase.New(c, styles)
			listeningStyles = New List(Of Style)
			buffer = New ElementBuffer(Me, createDefaultRoot())
			Dim defaultStyle As Style = styles.getStyle(StyleContext.DEFAULT_STYLE)
			logicalStyleyle(0, defaultStyle)
		End Sub

		''' <summary>
		''' Constructs a styled document with the default content
		''' storage implementation and a shared set of styles.
		''' </summary>
		''' <param name="styles"> the styles </param>
		Public Sub New(ByVal styles As StyleContext)
			Me.New(New GapContent(BUFFER_SIZE_DEFAULT), styles)
		End Sub

		''' <summary>
		''' Constructs a default styled document.  This buffers
		''' input content by a size of <em>BUFFER_SIZE_DEFAULT</em>
		''' and has a style context that is scoped by the lifetime
		''' of the document and is not shared with other documents.
		''' </summary>
		Public Sub New()
			Me.New(New GapContent(BUFFER_SIZE_DEFAULT), New StyleContext)
		End Sub

		''' <summary>
		''' Gets the default root element.
		''' </summary>
		''' <returns> the root </returns>
		''' <seealso cref= Document#getDefaultRootElement </seealso>
		Public Property Overrides defaultRootElement As Element Implements Document.getDefaultRootElement
			Get
				Return buffer.rootElement
			End Get
		End Property

		''' <summary>
		''' Initialize the document to reflect the given element
		''' structure (i.e. the structure reported by the
		''' <code>getDefaultRootElement</code> method.  If the
		''' document contained any data it will first be removed.
		''' </summary>
		Protected Friend Overridable Sub create(ByVal data As ElementSpec())
			Try
				If length <> 0 Then remove(0, length)
				writeLock()

				' install the content
				Dim c As Content = content
				Dim n As Integer = data.Length
				Dim sb As New StringBuilder
				For i As Integer = 0 To n - 1
					Dim es As ElementSpec = data(i)
					If es.length > 0 Then sb.Append(es.array, es.offset, es.length)
				Next i
				Dim cEdit As javax.swing.undo.UndoableEdit = c.insertString(0, sb.ToString())

				' build the event and element structure
				Dim ___length As Integer = sb.Length
				Dim evnt As New DefaultDocumentEvent(0, ___length, DocumentEvent.EventType.INSERT)
				evnt.addEdit(cEdit)
				buffer.create(___length, data, evnt)

				' update bidi (possibly)
				MyBase.insertUpdate(evnt, Nothing)

				' notify the listeners
				evnt.end()
				fireInsertUpdate(evnt)
				fireUndoableEditUpdate(New UndoableEditEvent(Me, evnt))
			Catch ble As BadLocationException
				Throw New StateInvariantError("problem initializing")
			Finally
				writeUnlock()
			End Try

		End Sub

		''' <summary>
		''' Inserts new elements in bulk.  This is useful to allow
		''' parsing with the document in an unlocked state and
		''' prepare an element structure modification.  This method
		''' takes an array of tokens that describe how to update an
		''' element structure so the time within a write lock can
		''' be greatly reduced in an asynchronous update situation.
		''' <p>
		''' This method is thread safe, although most Swing methods
		''' are not. Please see
		''' <A HREF="https://docs.oracle.com/javase/tutorial/uiswing/concurrency/index.html">Concurrency
		''' in Swing</A> for more information.
		''' </summary>
		''' <param name="offset"> the starting offset &gt;= 0 </param>
		''' <param name="data"> the element data </param>
		''' <exception cref="BadLocationException"> for an invalid starting offset </exception>
		Protected Friend Overridable Sub insert(ByVal offset As Integer, ByVal data As ElementSpec())
			If data Is Nothing OrElse data.Length = 0 Then Return

			Try
				writeLock()

				' install the content
				Dim c As Content = content
				Dim n As Integer = data.Length
				Dim sb As New StringBuilder
				For i As Integer = 0 To n - 1
					Dim es As ElementSpec = data(i)
					If es.length > 0 Then sb.Append(es.array, es.offset, es.length)
				Next i
				If sb.Length = 0 Then Return
				Dim cEdit As javax.swing.undo.UndoableEdit = c.insertString(offset, sb.ToString())

				' create event and build the element structure
				Dim ___length As Integer = sb.Length
				Dim evnt As New DefaultDocumentEvent(offset, ___length, DocumentEvent.EventType.INSERT)
				evnt.addEdit(cEdit)
				buffer.insert(offset, ___length, data, evnt)

				' update bidi (possibly)
				MyBase.insertUpdate(evnt, Nothing)

				' notify the listeners
				evnt.end()
				fireInsertUpdate(evnt)
				fireUndoableEditUpdate(New UndoableEditEvent(Me, evnt))
			Finally
				writeUnlock()
			End Try
		End Sub

		''' <summary>
		''' Removes an element from this document.
		''' 
		''' <p>The element is removed from its parent element, as well as
		''' the text in the range identified by the element.  If the
		''' element isn't associated with the document, {@code
		''' IllegalArgumentException} is thrown.</p>
		''' 
		''' <p>As empty branch elements are not allowed in the document, if the
		''' element is the sole child, its parent element is removed as well,
		''' recursively.  This means that when replacing all the children of a
		''' particular element, new children should be added <em>before</em>
		''' removing old children.
		''' 
		''' <p>Element removal results in two events being fired, the
		''' {@code DocumentEvent} for changes in element structure and {@code
		''' UndoableEditEvent} for changes in document content.</p>
		''' 
		''' <p>If the element contains end-of-content mark (the last {@code
		''' "\n"} character in document), this character is not removed;
		''' instead, preceding leaf element is extended to cover the
		''' character.  If the last leaf already ends with {@code "\n",} it is
		''' included in content removal.</p>
		''' 
		''' <p>If the element is {@code null,} {@code NullPointerException} is
		''' thrown.  If the element structure would become invalid after the removal,
		''' for example if the element is the document root element, {@code
		''' IllegalArgumentException} is thrown.  If the current element structure is
		''' invalid, {@code IllegalStateException} is thrown.</p>
		''' </summary>
		''' <param name="elem">                      the element to remove </param>
		''' <exception cref="NullPointerException">      if the element is {@code null} </exception>
		''' <exception cref="IllegalArgumentException">  if the element could not be removed </exception>
		''' <exception cref="IllegalStateException">     if the element structure is invalid
		''' 
		''' @since  1.7 </exception>
		Public Overridable Sub removeElement(ByVal elem As Element)
			Try
				writeLock()
				removeElementImpl(elem)
			Finally
				writeUnlock()
			End Try
		End Sub

		Private Sub removeElementImpl(ByVal elem As Element)
			If elem.document IsNot Me Then Throw New System.ArgumentException("element doesn't belong to document")
			Dim parent As BranchElement = CType(elem.parentElement, BranchElement)
			If parent Is Nothing Then Throw New System.ArgumentException("can't remove the root element")

			Dim startOffset As Integer = elem.startOffset
			Dim removeFrom As Integer = startOffset
			Dim endOffset As Integer = elem.endOffset
			Dim removeTo As Integer = endOffset
			Dim lastEndOffset As Integer = length + 1
			Dim ___content As Content = content
			Dim atEnd As Boolean = False
			Dim isComposedText As Boolean = Utilities.isComposedTextElement(elem)

			If endOffset >= lastEndOffset Then
				' element includes the last "\n" character, needs special handling
				If startOffset <= 0 Then Throw New System.ArgumentException("can't remove the whole content")
				removeTo = lastEndOffset - 1 ' last "\n" must not be removed
				Try
					If ___content.getString(startOffset - 1, 1).Chars(0) = ControlChars.Lf Then
						removeFrom -= 1 ' preceding leaf ends with "\n", remove it
					End If ' can't happen
				Catch ble As BadLocationException
					Throw New IllegalStateException(ble)
				End Try
				atEnd = True
			End If
			Dim ___length As Integer = removeTo - removeFrom

			Dim dde As New DefaultDocumentEvent(removeFrom, ___length, DefaultDocumentEvent.EventType.REMOVE)
			Dim ue As javax.swing.undo.UndoableEdit = Nothing
			' do not leave empty branch elements
			Do While parent.elementCount = 1
				elem = parent
				parent = CType(parent.parentElement, BranchElement)
				If parent Is Nothing Then ' shouldn't happen Throw New IllegalStateException("invalid element structure")
			Loop
			Dim removed As Element() = { elem }
			Dim added As Element() = {}
			Dim index As Integer = parent.getElementIndex(startOffset)
			parent.replace(index, 1, added)
			dde.addEdit(New ElementEdit(parent, index, removed, added))
			If ___length > 0 Then
				Try
					ue = ___content.remove(removeFrom, ___length)
					If ue IsNot Nothing Then dde.addEdit(ue)
				Catch ble As BadLocationException
					' can only happen if the element structure is severely broken
					Throw New IllegalStateException(ble)
				End Try
				lastEndOffset -= ___length
			End If

			If atEnd Then
				' preceding leaf element should be extended to cover orphaned "\n"
				Dim prevLeaf As Element = parent.getElement(parent.elementCount - 1)
				Do While (prevLeaf IsNot Nothing) AndAlso Not prevLeaf.leaf
					prevLeaf = prevLeaf.getElement(prevLeaf.elementCount - 1)
				Loop
				If prevLeaf Is Nothing Then ' shouldn't happen Throw New IllegalStateException("invalid element structure")
				Dim prevStartOffset As Integer = prevLeaf.startOffset
				Dim prevParent As BranchElement = CType(prevLeaf.parentElement, BranchElement)
				Dim prevIndex As Integer = prevParent.getElementIndex(prevStartOffset)
				Dim newElem As Element
				newElem = createLeafElement(prevParent, prevLeaf.attributes, prevStartOffset, lastEndOffset)
				Dim prevRemoved As Element() = { prevLeaf }
				Dim prevAdded As Element() = { newElem }
				prevParent.replace(prevIndex, 1, prevAdded)
				dde.addEdit(New ElementEdit(prevParent, prevIndex, prevRemoved, prevAdded))
			End If

			postRemoveUpdate(dde)
			dde.end()
			fireRemoveUpdate(dde)
			If Not(isComposedText AndAlso (ue IsNot Nothing)) Then fireUndoableEditUpdate(New UndoableEditEvent(Me, dde))
		End Sub

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
		Public Overridable Function addStyle(ByVal nm As String, ByVal parent As Style) As Style Implements StyledDocument.addStyle
			Dim styles As StyleContext = CType(attributeContext, StyleContext)
			Return styles.addStyle(nm, parent)
		End Function

		''' <summary>
		''' Removes a named style previously added to the document.
		''' </summary>
		''' <param name="nm">  the name of the style to remove </param>
		Public Overridable Sub removeStyle(ByVal nm As String) Implements StyledDocument.removeStyle
			Dim styles As StyleContext = CType(attributeContext, StyleContext)
			styles.removeStyle(nm)
		End Sub

		''' <summary>
		''' Fetches a named style previously added.
		''' </summary>
		''' <param name="nm">  the name of the style </param>
		''' <returns> the style </returns>
		Public Overridable Function getStyle(ByVal nm As String) As Style Implements StyledDocument.getStyle
			Dim styles As StyleContext = CType(attributeContext, StyleContext)
			Return styles.getStyle(nm)
		End Function


		''' <summary>
		''' Fetches the list of of style names.
		''' </summary>
		''' <returns> all the style names </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Property styleNames As System.Collections.IEnumerator(Of ?)
			Get
				Return CType(attributeContext, StyleContext).styleNames
			End Get
		End Property

		''' <summary>
		''' Sets the logical style to use for the paragraph at the
		''' given position.  If attributes aren't explicitly set
		''' for character and paragraph attributes they will resolve
		''' through the logical style assigned to the paragraph, which
		''' in turn may resolve through some hierarchy completely
		''' independent of the element hierarchy in the document.
		''' <p>
		''' This method is thread safe, although most Swing methods
		''' are not. Please see
		''' <A HREF="https://docs.oracle.com/javase/tutorial/uiswing/concurrency/index.html">Concurrency
		''' in Swing</A> for more information.
		''' </summary>
		''' <param name="pos"> the offset from the start of the document &gt;= 0 </param>
		''' <param name="s">  the logical style to assign to the paragraph, null if none </param>
		Public Overridable Sub setLogicalStyle(ByVal pos As Integer, ByVal s As Style) Implements StyledDocument.setLogicalStyle
			Dim paragraph As Element = getParagraphElement(pos)
			If (paragraph IsNot Nothing) AndAlso (TypeOf paragraph Is AbstractElement) Then
				Try
					writeLock()
					Dim edit As New StyleChangeUndoableEdit(CType(paragraph, AbstractElement), s)
					CType(paragraph, AbstractElement).resolveParent = s
					Dim p0 As Integer = paragraph.startOffset
					Dim p1 As Integer = paragraph.endOffset
					Dim e As New DefaultDocumentEvent(p0, p1 - p0, DocumentEvent.EventType.CHANGE)
					e.addEdit(edit)
					e.end()
					fireChangedUpdate(e)
					fireUndoableEditUpdate(New UndoableEditEvent(Me, e))
				Finally
					writeUnlock()
				End Try
			End If
		End Sub

		''' <summary>
		''' Fetches the logical style assigned to the paragraph
		''' represented by the given position.
		''' </summary>
		''' <param name="p"> the location to translate to a paragraph
		'''  and determine the logical style assigned &gt;= 0.  This
		'''  is an offset from the start of the document. </param>
		''' <returns> the style, null if none </returns>
		Public Overridable Function getLogicalStyle(ByVal p As Integer) As Style Implements StyledDocument.getLogicalStyle
			Dim s As Style = Nothing
			Dim paragraph As Element = getParagraphElement(p)
			If paragraph IsNot Nothing Then
				Dim a As AttributeSet = paragraph.attributes
				Dim parent As AttributeSet = a.resolveParent
				If TypeOf parent Is Style Then s = CType(parent, Style)
			End If
			Return s
		End Function

		''' <summary>
		''' Sets attributes for some part of the document.
		''' A write lock is held by this operation while changes
		''' are being made, and a DocumentEvent is sent to the listeners
		''' after the change has been successfully completed.
		''' <p>
		''' This method is thread safe, although most Swing methods
		''' are not. Please see
		''' <A HREF="https://docs.oracle.com/javase/tutorial/uiswing/concurrency/index.html">Concurrency
		''' in Swing</A> for more information.
		''' </summary>
		''' <param name="offset"> the offset in the document &gt;= 0 </param>
		''' <param name="length"> the length &gt;= 0 </param>
		''' <param name="s"> the attributes </param>
		''' <param name="replace"> true if the previous attributes should be replaced
		'''  before setting the new attributes </param>
		Public Overridable Sub setCharacterAttributes(ByVal offset As Integer, ByVal length As Integer, ByVal s As AttributeSet, ByVal replace As Boolean) Implements StyledDocument.setCharacterAttributes
			If length = 0 Then Return
			Try
				writeLock()
				Dim changes As New DefaultDocumentEvent(offset, length, DocumentEvent.EventType.CHANGE)

				' split elements that need it
				buffer.change(offset, length, changes)

				Dim sCopy As AttributeSet = s.copyAttributes()

				' PENDING(prinz) - this isn't a very efficient way to iterate
				Dim lastEnd As Integer
				Dim pos As Integer = offset
				Do While pos < (offset + length)
					Dim run As Element = getCharacterElement(pos)
					lastEnd = run.endOffset
					If pos = lastEnd Then Exit Do
					Dim attr As MutableAttributeSet = CType(run.attributes, MutableAttributeSet)
					changes.addEdit(New AttributeUndoableEdit(run, sCopy, replace))
					If replace Then attr.removeAttributes(attr)
					attr.addAttributes(s)
					pos = lastEnd
				Loop
				changes.end()
				fireChangedUpdate(changes)
				fireUndoableEditUpdate(New UndoableEditEvent(Me, changes))
			Finally
				writeUnlock()
			End Try

		End Sub

		''' <summary>
		''' Sets attributes for a paragraph.
		''' <p>
		''' This method is thread safe, although most Swing methods
		''' are not. Please see
		''' <A HREF="https://docs.oracle.com/javase/tutorial/uiswing/concurrency/index.html">Concurrency
		''' in Swing</A> for more information.
		''' </summary>
		''' <param name="offset"> the offset into the paragraph &gt;= 0 </param>
		''' <param name="length"> the number of characters affected &gt;= 0 </param>
		''' <param name="s"> the attributes </param>
		''' <param name="replace"> whether to replace existing attributes, or merge them </param>
		Public Overridable Sub setParagraphAttributes(ByVal offset As Integer, ByVal length As Integer, ByVal s As AttributeSet, ByVal replace As Boolean) Implements StyledDocument.setParagraphAttributes
			Try
				writeLock()
				Dim changes As New DefaultDocumentEvent(offset, length, DocumentEvent.EventType.CHANGE)

				Dim sCopy As AttributeSet = s.copyAttributes()

				' PENDING(prinz) - this assumes a particular element structure
				Dim section As Element = defaultRootElement
				Dim index0 As Integer = section.getElementIndex(offset)
				Dim index1 As Integer = section.getElementIndex(offset + (If(length > 0, length - 1, 0)))
				Dim isI18N As Boolean = Boolean.TRUE.Equals(getProperty(I18NProperty))
				Dim hasRuns As Boolean = False
				For i As Integer = index0 To index1
					Dim paragraph As Element = section.getElement(i)
					Dim attr As MutableAttributeSet = CType(paragraph.attributes, MutableAttributeSet)
					changes.addEdit(New AttributeUndoableEdit(paragraph, sCopy, replace))
					If replace Then attr.removeAttributes(attr)
					attr.addAttributes(s)
					If isI18N AndAlso (Not hasRuns) Then hasRuns = (attr.getAttribute(java.awt.font.TextAttribute.RUN_DIRECTION) IsNot Nothing)
				Next i

				If hasRuns Then updateBidi(changes)

				changes.end()
				fireChangedUpdate(changes)
				fireUndoableEditUpdate(New UndoableEditEvent(Me, changes))
			Finally
				writeUnlock()
			End Try
		End Sub

		''' <summary>
		''' Gets the paragraph element at the offset <code>pos</code>.
		''' A paragraph consists of at least one child Element, which is usually
		''' a leaf.
		''' </summary>
		''' <param name="pos"> the starting offset &gt;= 0 </param>
		''' <returns> the element </returns>
		Public Overrides Function getParagraphElement(ByVal pos As Integer) As Element Implements StyledDocument.getParagraphElement
			Dim e As Element
			e = defaultRootElement
			Do While Not e.leaf
				Dim index As Integer = e.getElementIndex(pos)
				e = e.getElement(index)
			Loop
			If e IsNot Nothing Then Return e.parentElement
			Return e
		End Function

		''' <summary>
		''' Gets a character element based on a position.
		''' </summary>
		''' <param name="pos"> the position in the document &gt;= 0 </param>
		''' <returns> the element </returns>
		Public Overridable Function getCharacterElement(ByVal pos As Integer) As Element Implements StyledDocument.getCharacterElement
			Dim e As Element
			e = defaultRootElement
			Do While Not e.leaf
				Dim index As Integer = e.getElementIndex(pos)
				e = e.getElement(index)
			Loop
			Return e
		End Function

		' --- local methods -------------------------------------------------

		''' <summary>
		''' Updates document structure as a result of text insertion.  This
		''' will happen within a write lock.  This implementation simply
		''' parses the inserted content for line breaks and builds up a set
		''' of instructions for the element buffer.
		''' </summary>
		''' <param name="chng"> a description of the document change </param>
		''' <param name="attr"> the attributes </param>
		Protected Friend Overrides Sub insertUpdate(ByVal chng As DefaultDocumentEvent, ByVal attr As AttributeSet)
			Dim offset As Integer = chng.offset
			Dim ___length As Integer = chng.length
			If attr Is Nothing Then attr = SimpleAttributeSet.EMPTY

			' Paragraph attributes should come from point after insertion.
			' You really only notice this when inserting at a paragraph
			' boundary.
			Dim paragraph As Element = getParagraphElement(offset + ___length)
			Dim pattr As AttributeSet = paragraph.attributes
			' Character attributes should come from actual insertion point.
			Dim pParagraph As Element = getParagraphElement(offset)
			Dim run As Element = pParagraph.getElement(pParagraph.getElementIndex(offset))
			Dim endOffset As Integer = offset + ___length
			Dim insertingAtBoundry As Boolean = (run.endOffset = endOffset)
			Dim cattr As AttributeSet = run.attributes

			Try
				Dim s As New Segment
				Dim parseBuffer As New List(Of ElementSpec)
				Dim lastStartSpec As ElementSpec = Nothing
				Dim insertingAfterNewline As Boolean = False
				Dim lastStartDirection As Short = ElementSpec.OriginateDirection
				' Check if the previous character was a newline.
				If offset > 0 Then
					getText(offset - 1, 1, s)
					If s.array(s.offset) = ControlChars.Lf Then
						' Inserting after a newline.
						insertingAfterNewline = True
						lastStartDirection = createSpecsForInsertAfterNewline(paragraph, pParagraph, pattr, parseBuffer, offset, endOffset)
						For counter As Integer = parseBuffer.Count - 1 To 0 Step -1
							Dim spec As ElementSpec = parseBuffer(counter)
							If spec.type = ElementSpec.StartTagType Then
								lastStartSpec = spec
								Exit For
							End If
						Next counter
					End If
				End If
				' If not inserting after a new line, pull the attributes for
				' new paragraphs from the paragraph under the insertion point.
				If Not insertingAfterNewline Then pattr = pParagraph.attributes

				getText(offset, ___length, s)
				Dim txt As Char() = s.array
				Dim n As Integer = s.offset + s.count
				Dim lastOffset As Integer = s.offset

				For i As Integer = s.offset To n - 1
					If txt(i) = ControlChars.Lf Then
						Dim breakOffset As Integer = i + 1
						parseBuffer.Add(New ElementSpec(attr, ElementSpec.ContentType, breakOffset - lastOffset))
						parseBuffer.Add(New ElementSpec(Nothing, ElementSpec.EndTagType))
						lastStartSpec = New ElementSpec(pattr, ElementSpec.StartTagType)
						parseBuffer.Add(lastStartSpec)
						lastOffset = breakOffset
					End If
				Next i
				If lastOffset < n Then parseBuffer.Add(New ElementSpec(attr, ElementSpec.ContentType, n - lastOffset))

				Dim first As ElementSpec = parseBuffer(0)

				Dim docLength As Integer = length

				' Check for join previous of first content.
				If first.type = ElementSpec.ContentType AndAlso cattr.isEqual(attr) Then first.direction = ElementSpec.JoinPreviousDirection

				' Do a join fracture/next for last start spec if necessary.
				If lastStartSpec IsNot Nothing Then
					If insertingAfterNewline Then
						lastStartSpec.direction = lastStartDirection
					' Join to the fracture if NOT inserting at the end
					' (fracture only happens when not inserting at end of
					' paragraph).
					ElseIf pParagraph.endOffset <> endOffset Then
						lastStartSpec.direction = ElementSpec.JoinFractureDirection
					' Join to next if parent of pParagraph has another
					' element after pParagraph, and it isn't a leaf.
					Else
						Dim parent As Element = pParagraph.parentElement
						Dim pParagraphIndex As Integer = parent.getElementIndex(offset)
						If (pParagraphIndex + 1) < parent.elementCount AndAlso (Not parent.getElement(pParagraphIndex + 1).leaf) Then lastStartSpec.direction = ElementSpec.JoinNextDirection
					End If
				End If

				' Do a JoinNext for last spec if it is content, it doesn't
				' already have a direction set, no new paragraphs have been
				' inserted or a new paragraph has been inserted and its join
				' direction isn't originate, and the element at endOffset
				' is a leaf.
				If insertingAtBoundry AndAlso endOffset < docLength Then
					Dim last As ElementSpec = parseBuffer(parseBuffer.Count - 1)
					If last.type = ElementSpec.ContentType AndAlso last.direction <> ElementSpec.JoinPreviousDirection AndAlso ((lastStartSpec Is Nothing AndAlso (paragraph Is pParagraph OrElse insertingAfterNewline)) OrElse (lastStartSpec IsNot Nothing AndAlso lastStartSpec.direction <> ElementSpec.OriginateDirection)) Then
						Dim nextRun As Element = paragraph.getElement(paragraph.getElementIndex(endOffset))
						' Don't try joining to a branch!
						If nextRun.leaf AndAlso attr.isEqual(nextRun.attributes) Then last.direction = ElementSpec.JoinNextDirection
					End If
				' If not inserting at boundary and there is going to be a
				' fracture, then can join next on last content if cattr
				' matches the new attributes.
				ElseIf (Not insertingAtBoundry) AndAlso lastStartSpec IsNot Nothing AndAlso lastStartSpec.direction = ElementSpec.JoinFractureDirection Then
					Dim last As ElementSpec = parseBuffer(parseBuffer.Count - 1)
					If last.type = ElementSpec.ContentType AndAlso last.direction <> ElementSpec.JoinPreviousDirection AndAlso attr.isEqual(cattr) Then last.direction = ElementSpec.JoinNextDirection
				End If

				' Check for the composed text element. If it is, merge the character attributes
				' into this element as well.
				If Utilities.isComposedTextAttributeDefined(attr) Then
					Dim mattr As MutableAttributeSet = CType(attr, MutableAttributeSet)
					mattr.addAttributes(cattr)
					mattr.addAttribute(AbstractDocument.ElementNameAttribute, AbstractDocument.ContentElementName)

					' Assure that the composed text element is named properly
					' and doesn't have the CR attribute defined.
					mattr.addAttribute(StyleConstants.NameAttribute, AbstractDocument.ContentElementName)
					If mattr.isDefined(IMPLIED_CR) Then mattr.removeAttribute(IMPLIED_CR)
				End If

				Dim spec As ElementSpec() = New ElementSpec(parseBuffer.Count - 1){}
				parseBuffer.CopyTo(spec)
				buffer.insert(offset, ___length, spec, chng)
			Catch bl As BadLocationException
			End Try

			MyBase.insertUpdate(chng, attr)
		End Sub

		''' <summary>
		''' This is called by insertUpdate when inserting after a new line.
		''' It generates, in <code>parseBuffer</code>, ElementSpecs that will
		''' position the stack in <code>paragraph</code>.<p>
		''' It returns the direction the last StartSpec should have (this don't
		''' necessarily create the last start spec).
		''' </summary>
		Friend Overridable Function createSpecsForInsertAfterNewline(ByVal paragraph As Element, ByVal pParagraph As Element, ByVal pattr As AttributeSet, ByVal parseBuffer As List(Of ElementSpec), ByVal offset As Integer, ByVal endOffset As Integer) As Short
			' Need to find the common parent of pParagraph and paragraph.
			If paragraph.parentElement Is pParagraph.parentElement Then
				' The simple (and common) case that pParagraph and
				' paragraph have the same parent.
				Dim spec As New ElementSpec(pattr, ElementSpec.EndTagType)
				parseBuffer.Add(spec)
				spec = New ElementSpec(pattr, ElementSpec.StartTagType)
				parseBuffer.Add(spec)
				If pParagraph.endOffset <> endOffset Then Return ElementSpec.JoinFractureDirection

				Dim parent As Element = pParagraph.parentElement
				If (parent.getElementIndex(offset) + 1) < parent.elementCount Then Return ElementSpec.JoinNextDirection
			Else
				' Will only happen for text with more than 2 levels.
				' Find the common parent of a paragraph and pParagraph
				Dim leftParents As New List(Of Element)
				Dim rightParents As New List(Of Element)
				Dim e As Element = pParagraph
				Do While e IsNot Nothing
					leftParents.Add(e)
					e = e.parentElement
				Loop
				e = paragraph
				Dim leftIndex As Integer = -1
				leftIndex = leftParents.IndexOf(e)
				Do While e IsNot Nothing AndAlso leftIndex = -1
					rightParents.Add(e)
					e = e.parentElement
					leftIndex = leftParents.IndexOf(e)
				Loop
				If e IsNot Nothing Then
					' e identifies the common parent.
					' Build the ends.
					For counter As Integer = 0 To leftIndex - 1
						parseBuffer.Add(New ElementSpec(Nothing, ElementSpec.EndTagType))
					Next counter
					' And the starts.
					Dim spec As ElementSpec
					For counter As Integer = rightParents.Count - 1 To 0 Step -1
						spec = New ElementSpec(rightParents(counter).attributes, ElementSpec.StartTagType)
						If counter > 0 Then spec.direction = ElementSpec.JoinNextDirection
						parseBuffer.Add(spec)
					Next counter
					' If there are right parents, then we generated starts
					' down the right subtree and there will be an element to
					' join to.
					If rightParents.Count > 0 Then Return ElementSpec.JoinNextDirection
					' No right subtree, e.getElement(endOffset) is a
					' leaf. There will be a facture.
					Return ElementSpec.JoinFractureDirection
				End If
				' else: Could throw an exception here, but should never get here!
			End If
			Return ElementSpec.OriginateDirection
		End Function

		''' <summary>
		''' Updates document structure as a result of text removal.
		''' </summary>
		''' <param name="chng"> a description of the document change </param>
		Protected Friend Overrides Sub removeUpdate(ByVal chng As DefaultDocumentEvent)
			MyBase.removeUpdate(chng)
			buffer.remove(chng.offset, chng.length, chng)
		End Sub

		''' <summary>
		''' Creates the root element to be used to represent the
		''' default document structure.
		''' </summary>
		''' <returns> the element base </returns>
		Protected Friend Overridable Function createDefaultRoot() As AbstractElement
			' grabs a write-lock for this initialization and
			' abandon it during initialization so in normal
			' operation we can detect an illegitimate attempt
			' to mutate attributes.
			writeLock()
			Dim section As BranchElement = New SectionElement(Me)
			Dim paragraph As New BranchElement(section, Nothing)

			Dim brk As New LeafElement(paragraph, Nothing, 0, 1)
			Dim buff As Element() = New Element(0){}
			buff(0) = brk
			paragraph.replace(0, 0, buff)

			buff(0) = paragraph
			section.replace(0, 0, buff)
			writeUnlock()
			Return section
		End Function

		''' <summary>
		''' Gets the foreground color from an attribute set.
		''' </summary>
		''' <param name="attr"> the attribute set </param>
		''' <returns> the color </returns>
		Public Overridable Function getForeground(ByVal attr As AttributeSet) As java.awt.Color Implements StyledDocument.getForeground
			Dim styles As StyleContext = CType(attributeContext, StyleContext)
			Return styles.getForeground(attr)
		End Function

		''' <summary>
		''' Gets the background color from an attribute set.
		''' </summary>
		''' <param name="attr"> the attribute set </param>
		''' <returns> the color </returns>
		Public Overridable Function getBackground(ByVal attr As AttributeSet) As java.awt.Color Implements StyledDocument.getBackground
			Dim styles As StyleContext = CType(attributeContext, StyleContext)
			Return styles.getBackground(attr)
		End Function

		''' <summary>
		''' Gets the font from an attribute set.
		''' </summary>
		''' <param name="attr"> the attribute set </param>
		''' <returns> the font </returns>
		Public Overridable Function getFont(ByVal attr As AttributeSet) As java.awt.Font Implements StyledDocument.getFont
			Dim styles As StyleContext = CType(attributeContext, StyleContext)
			Return styles.getFont(attr)
		End Function

		''' <summary>
		''' Called when any of this document's styles have changed.
		''' Subclasses may wish to be intelligent about what gets damaged.
		''' </summary>
		''' <param name="style"> The Style that has changed. </param>
		Protected Friend Overridable Sub styleChanged(ByVal style As Style)
			' Only propagate change updated if have content
			If length <> 0 Then
				' lazily create a ChangeUpdateRunnable
				If updateRunnable Is Nothing Then updateRunnable = New ChangeUpdateRunnable(Me)

				' We may get a whole batch of these at once, so only
				' queue the runnable if it is not already pending
				SyncLock updateRunnable
					If Not updateRunnable.isPending Then
						javax.swing.SwingUtilities.invokeLater(updateRunnable)
						updateRunnable.isPending = True
					End If
				End SyncLock
			End If
		End Sub

		''' <summary>
		''' Adds a document listener for notification of any changes.
		''' </summary>
		''' <param name="listener"> the listener </param>
		''' <seealso cref= Document#addDocumentListener </seealso>
		Public Overrides Sub addDocumentListener(ByVal listener As DocumentListener) Implements Document.addDocumentListener
			SyncLock listeningStyles
				Dim oldDLCount As Integer = listenerList.getListenerCount(GetType(DocumentListener))
				MyBase.addDocumentListener(listener)
				If oldDLCount = 0 Then
					If styleContextChangeListener Is Nothing Then styleContextChangeListener = createStyleContextChangeListener()
					If styleContextChangeListener IsNot Nothing Then
						Dim styles As StyleContext = CType(attributeContext, StyleContext)
						Dim staleListeners As IList(Of ChangeListener) = AbstractChangeHandler.getStaleListeners(styleContextChangeListener)
						For Each l As ChangeListener In staleListeners
							styles.removeChangeListener(l)
						Next l
						styles.addChangeListener(styleContextChangeListener)
					End If
					updateStylesListeningTo()
				End If
			End SyncLock
		End Sub

		''' <summary>
		''' Removes a document listener.
		''' </summary>
		''' <param name="listener"> the listener </param>
		''' <seealso cref= Document#removeDocumentListener </seealso>
		Public Overrides Sub removeDocumentListener(ByVal listener As DocumentListener) Implements Document.removeDocumentListener
			SyncLock listeningStyles
				MyBase.removeDocumentListener(listener)
				If listenerList.getListenerCount(GetType(DocumentListener)) = 0 Then
					For counter As Integer = listeningStyles.Count - 1 To 0 Step -1
						listeningStyles(counter).removeChangeListener(styleChangeListener)
					Next counter
					listeningStyles.Clear()
					If styleContextChangeListener IsNot Nothing Then
						Dim styles As StyleContext = CType(attributeContext, StyleContext)
						styles.removeChangeListener(styleContextChangeListener)
					End If
				End If
			End SyncLock
		End Sub

		''' <summary>
		''' Returns a new instance of StyleChangeHandler.
		''' </summary>
		Friend Overridable Function createStyleChangeListener() As ChangeListener
			Return New StyleChangeHandler(Me)
		End Function

		''' <summary>
		''' Returns a new instance of StyleContextChangeHandler.
		''' </summary>
		Friend Overridable Function createStyleContextChangeListener() As ChangeListener
			Return New StyleContextChangeHandler(Me)
		End Function

		''' <summary>
		''' Adds a ChangeListener to new styles, and removes ChangeListener from
		''' old styles.
		''' </summary>
		Friend Overridable Sub updateStylesListeningTo()
			SyncLock listeningStyles
				Dim styles As StyleContext = CType(attributeContext, StyleContext)
				If styleChangeListener Is Nothing Then styleChangeListener = createStyleChangeListener()
				If styleChangeListener IsNot Nothing AndAlso styles IsNot Nothing Then
					Dim ___styleNames As System.Collections.IEnumerator = styles.styleNames
					Dim v As ArrayList = CType(listeningStyles.clone(), ArrayList)
					listeningStyles.Clear()
					Dim staleListeners As IList(Of ChangeListener) = AbstractChangeHandler.getStaleListeners(styleChangeListener)
					Do While ___styleNames.hasMoreElements()
						Dim name As String = CStr(___styleNames.nextElement())
						Dim aStyle As Style = styles.getStyle(name)
						Dim index As Integer = v.IndexOf(aStyle)
						listeningStyles.Add(aStyle)
						If index = -1 Then
							For Each l As ChangeListener In staleListeners
								aStyle.removeChangeListener(l)
							Next l
							aStyle.addChangeListener(styleChangeListener)
						Else
							v.RemoveAt(index)
						End If
					Loop
					For counter As Integer = v.Count - 1 To 0 Step -1
						Dim aStyle As Style = CType(v(counter), Style)
						aStyle.removeChangeListener(styleChangeListener)
					Next counter
					If listeningStyles.Count = 0 Then styleChangeListener = Nothing
				End If
			End SyncLock
		End Sub

		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			listeningStyles = New List(Of Style)
			s.defaultReadObject()
			' Reinstall style listeners.
			If styleContextChangeListener Is Nothing AndAlso listenerList.getListenerCount(GetType(DocumentListener)) > 0 Then
				styleContextChangeListener = createStyleContextChangeListener()
				If styleContextChangeListener IsNot Nothing Then
					Dim styles As StyleContext = CType(attributeContext, StyleContext)
					styles.addChangeListener(styleContextChangeListener)
				End If
				updateStylesListeningTo()
			End If
		End Sub

		' --- member variables -----------------------------------------------------------

		''' <summary>
		''' The default size of the initial content buffer.
		''' </summary>
		Public Const BUFFER_SIZE_DEFAULT As Integer = 4096

		Protected Friend buffer As ElementBuffer

		''' <summary>
		''' Styles listening to. </summary>
		<NonSerialized> _
		Private listeningStyles As List(Of Style)

		''' <summary>
		''' Listens to Styles. </summary>
		<NonSerialized> _
		Private styleChangeListener As ChangeListener

		''' <summary>
		''' Listens to Styles. </summary>
		<NonSerialized> _
		Private styleContextChangeListener As ChangeListener

		''' <summary>
		''' Run to create a change event for the document </summary>
		<NonSerialized> _
		Private updateRunnable As ChangeUpdateRunnable

		''' <summary>
		''' Default root element for a document... maps out the
		''' paragraphs/lines contained.
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
		Protected Friend Class SectionElement
			Inherits BranchElement

			Private ReadOnly outerInstance As DefaultStyledDocument


			''' <summary>
			''' Creates a new SectionElement.
			''' </summary>
			Public Sub New(ByVal outerInstance As DefaultStyledDocument)
					Me.outerInstance = outerInstance
				MyBase.New(Nothing, Nothing)
			End Sub

			''' <summary>
			''' Gets the name of the element.
			''' </summary>
			''' <returns> the name </returns>
			Public Overridable Property name As String
				Get
					Return SectionElementName
				End Get
			End Property
		End Class

		''' <summary>
		''' Specification for building elements.
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
		Public Class ElementSpec

			''' <summary>
			''' A possible value for getType.  This specifies
			''' that this record type is a start tag and
			''' represents markup that specifies the start
			''' of an element.
			''' </summary>
			Public Const StartTagType As Short = 1

			''' <summary>
			''' A possible value for getType.  This specifies
			''' that this record type is a end tag and
			''' represents markup that specifies the end
			''' of an element.
			''' </summary>
			Public Const EndTagType As Short = 2

			''' <summary>
			''' A possible value for getType.  This specifies
			''' that this record type represents content.
			''' </summary>
			Public Const ContentType As Short = 3

			''' <summary>
			''' A possible value for getDirection.  This specifies
			''' that the data associated with this record should
			''' be joined to what precedes it.
			''' </summary>
			Public Const JoinPreviousDirection As Short = 4

			''' <summary>
			''' A possible value for getDirection.  This specifies
			''' that the data associated with this record should
			''' be joined to what follows it.
			''' </summary>
			Public Const JoinNextDirection As Short = 5

			''' <summary>
			''' A possible value for getDirection.  This specifies
			''' that the data associated with this record should
			''' be used to originate a new element.  This would be
			''' the normal value.
			''' </summary>
			Public Const OriginateDirection As Short = 6

			''' <summary>
			''' A possible value for getDirection.  This specifies
			''' that the data associated with this record should
			''' be joined to the fractured element.
			''' </summary>
			Public Const JoinFractureDirection As Short = 7


			''' <summary>
			''' Constructor useful for markup when the markup will not
			''' be stored in the document.
			''' </summary>
			''' <param name="a"> the attributes for the element </param>
			''' <param name="type"> the type of the element (StartTagType, EndTagType,
			'''  ContentType) </param>
			Public Sub New(ByVal a As AttributeSet, ByVal type As Short)
				Me.New(a, type, Nothing, 0, 0)
			End Sub

			''' <summary>
			''' Constructor for parsing inside the document when
			''' the data has already been added, but len information
			''' is needed.
			''' </summary>
			''' <param name="a"> the attributes for the element </param>
			''' <param name="type"> the type of the element (StartTagType, EndTagType,
			'''  ContentType) </param>
			''' <param name="len"> the length &gt;= 0 </param>
			Public Sub New(ByVal a As AttributeSet, ByVal type As Short, ByVal len As Integer)
				Me.New(a, type, Nothing, 0, len)
			End Sub

			''' <summary>
			''' Constructor for creating a spec externally for batch
			''' input of content and markup into the document.
			''' </summary>
			''' <param name="a"> the attributes for the element </param>
			''' <param name="type"> the type of the element (StartTagType, EndTagType,
			'''  ContentType) </param>
			''' <param name="txt"> the text for the element </param>
			''' <param name="offs"> the offset into the text &gt;= 0 </param>
			''' <param name="len"> the length of the text &gt;= 0 </param>
			Public Sub New(ByVal a As AttributeSet, ByVal type As Short, ByVal txt As Char(), ByVal offs As Integer, ByVal len As Integer)
				attr = a
				Me.type = type
				Me.data = txt
				Me.offs = offs
				Me.len = len
				Me.direction = OriginateDirection
			End Sub

			''' <summary>
			''' Sets the element type.
			''' </summary>
			''' <param name="type"> the type of the element (StartTagType, EndTagType,
			'''  ContentType) </param>
			Public Overridable Property type As Short
				Set(ByVal type As Short)
					Me.type = type
				End Set
				Get
					Return type
				End Get
			End Property


			''' <summary>
			''' Sets the direction.
			''' </summary>
			''' <param name="direction"> the direction (JoinPreviousDirection,
			'''   JoinNextDirection) </param>
			Public Overridable Property direction As Short
				Set(ByVal direction As Short)
					Me.direction = direction
				End Set
				Get
					Return direction
				End Get
			End Property


			''' <summary>
			''' Gets the element attributes.
			''' </summary>
			''' <returns> the attribute set </returns>
			Public Overridable Property attributes As AttributeSet
				Get
					Return attr
				End Get
			End Property

			''' <summary>
			''' Gets the array of characters.
			''' </summary>
			''' <returns> the array </returns>
			Public Overridable Property array As Char()
				Get
					Return data
				End Get
			End Property


			''' <summary>
			''' Gets the starting offset.
			''' </summary>
			''' <returns> the offset &gt;= 0 </returns>
			Public Overridable Property offset As Integer
				Get
					Return offs
				End Get
			End Property

			''' <summary>
			''' Gets the length.
			''' </summary>
			''' <returns> the length &gt;= 0 </returns>
			Public Overridable Property length As Integer
				Get
					Return len
				End Get
			End Property

			''' <summary>
			''' Converts the element to a string.
			''' </summary>
			''' <returns> the string </returns>
			Public Overrides Function ToString() As String
				Dim tlbl As String = "??"
				Dim plbl As String = "??"
				Select Case type
				Case StartTagType
					tlbl = "StartTag"
				Case ContentType
					tlbl = "Content"
				Case EndTagType
					tlbl = "EndTag"
				End Select
				Select Case direction
				Case JoinPreviousDirection
					plbl = "JoinPrevious"
				Case JoinNextDirection
					plbl = "JoinNext"
				Case OriginateDirection
					plbl = "Originate"
				Case JoinFractureDirection
					plbl = "Fracture"
				End Select
				Return tlbl & ":" & plbl & ":" & length
			End Function

			Private attr As AttributeSet
			Private len As Integer
			Private type As Short
			Private direction As Short

			Private offs As Integer
			Private data As Char()
		End Class

		''' <summary>
		''' Class to manage changes to the element
		''' hierarchy.
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
		<Serializable> _
		Public Class ElementBuffer
			Private ReadOnly outerInstance As DefaultStyledDocument


			''' <summary>
			''' Creates a new ElementBuffer.
			''' </summary>
			''' <param name="root"> the root element
			''' @since 1.4 </param>
			Public Sub New(ByVal outerInstance As DefaultStyledDocument, ByVal root As Element)
					Me.outerInstance = outerInstance
				Me.root = root
				changes = New List(Of ElemChanges)
				path = New Stack(Of ElemChanges)
			End Sub

			''' <summary>
			''' Gets the root element.
			''' </summary>
			''' <returns> the root element </returns>
			Public Overridable Property rootElement As Element
				Get
					Return root
				End Get
			End Property

			''' <summary>
			''' Inserts new content.
			''' </summary>
			''' <param name="offset"> the starting offset &gt;= 0 </param>
			''' <param name="length"> the length &gt;= 0 </param>
			''' <param name="data"> the data to insert </param>
			''' <param name="de"> the event capturing this edit </param>
			Public Overridable Sub insert(ByVal offset As Integer, ByVal length As Integer, ByVal data As ElementSpec(), ByVal de As DefaultDocumentEvent)
				If length = 0 Then Return
				insertOp = True
				beginEdits(offset, length)
				insertUpdate(data)
				endEdits(de)

				insertOp = False
			End Sub

			Friend Overridable Sub create(ByVal length As Integer, ByVal data As ElementSpec(), ByVal de As DefaultDocumentEvent)
				insertOp = True
				beginEdits(offset, length)

				' PENDING(prinz) this needs to be fixed to create a new
				' root element as well, but requires changes to the
				' DocumentEvent to inform the views that there is a new
				' root element.

				' Recreate the ending fake element to have the correct offsets.
				Dim elem As Element = root
				Dim index As Integer = elem.getElementIndex(0)
				Do While Not elem.leaf
					Dim child As Element = elem.getElement(index)
					push(elem, index)
					elem = child
					index = elem.getElementIndex(0)
				Loop
				Dim ec As ElemChanges = path.Peek()
				Dim child As Element = ec.parent.getElement(ec.index)
				ec.added.Add(outerInstance.createLeafElement(ec.parent, child.attributes, outerInstance.length, child.endOffset))
				ec.removed.Add(child)
				Do While path.Count > 1
					pop()
				Loop

				Dim n As Integer = data.Length

				' Reset the root elements attributes.
				Dim newAttrs As AttributeSet = Nothing
				If n > 0 AndAlso data(0).type = ElementSpec.StartTagType Then newAttrs = data(0).attributes
				If newAttrs Is Nothing Then newAttrs = SimpleAttributeSet.EMPTY
				Dim attr As MutableAttributeSet = CType(root.attributes, MutableAttributeSet)
				de.addEdit(New AttributeUndoableEdit(root, newAttrs, True))
				attr.removeAttributes(attr)
				attr.addAttributes(newAttrs)

				' fold in the specified subtree
				For i As Integer = 1 To n - 1
					insertElement(data(i))
				Next i

				' pop the remaining path
				Do While path.Count <> 0
					pop()
				Loop

				endEdits(de)
				insertOp = False
			End Sub

			''' <summary>
			''' Removes content.
			''' </summary>
			''' <param name="offset"> the starting offset &gt;= 0 </param>
			''' <param name="length"> the length &gt;= 0 </param>
			''' <param name="de"> the event capturing this edit </param>
			Public Overridable Sub remove(ByVal offset As Integer, ByVal length As Integer, ByVal de As DefaultDocumentEvent)
				beginEdits(offset, length)
				removeUpdate()
				endEdits(de)
			End Sub

			''' <summary>
			''' Changes content.
			''' </summary>
			''' <param name="offset"> the starting offset &gt;= 0 </param>
			''' <param name="length"> the length &gt;= 0 </param>
			''' <param name="de"> the event capturing this edit </param>
			Public Overridable Sub change(ByVal offset As Integer, ByVal length As Integer, ByVal de As DefaultDocumentEvent)
				beginEdits(offset, length)
				changeUpdate()
				endEdits(de)
			End Sub

			''' <summary>
			''' Inserts an update into the document.
			''' </summary>
			''' <param name="data"> the elements to insert </param>
			Protected Friend Overridable Sub insertUpdate(ByVal data As ElementSpec())
				' push the path
				Dim elem As Element = root
				Dim index As Integer = elem.getElementIndex(offset)
				Do While Not elem.leaf
					Dim child As Element = elem.getElement(index)
					push(elem, (If(child.leaf, index, index+1)))
					elem = child
					index = elem.getElementIndex(offset)
				Loop

				' Build a copy of the original path.
				insertPath = New ElemChanges(path.Count - 1){}
				path.copyInto(insertPath)

				' Haven't created the fracture yet.
				createdFracture = False

				' Insert the first content.
				Dim i As Integer

				recreateLeafs = False
				If data(0).type = ElementSpec.ContentType Then
					insertFirstContent(data)
					pos += data(0).length
					i = 1
				Else
					fractureDeepestLeaf(data)
					i = 0
				End If

				' fold in the specified subtree
				Dim n As Integer = data.Length
				Do While i < n
					insertElement(data(i))
					i += 1
				Loop

				' Fracture, if we haven't yet.
				If Not createdFracture Then fracture(-1)

				' pop the remaining path
				Do While path.Count <> 0
					pop()
				Loop

				' Offset the last index if necessary.
				If offsetLastIndex AndAlso offsetLastIndexOnReplace Then insertPath(insertPath.Length - 1).index += 1

				' Make sure an edit is going to be created for each of the
				' original path items that have a change.
				For counter As Integer = insertPath.Length - 1 To 0 Step -1
					Dim change As ElemChanges = insertPath(counter)
					If change.parent Is fracturedParent Then change.added.Add(fracturedChild)
					If (change.added.Count > 0 OrElse change.removed.Count > 0) AndAlso (Not changes.Contains(change)) Then changes.Add(change)
				Next counter

				' An insert at 0 with an initial end implies some elements
				' will have no children (the bottomost leaf would have length 0)
				' this will find what element need to be removed and remove it.
				If offset = 0 AndAlso fracturedParent IsNot Nothing AndAlso data(0).type = ElementSpec.EndTagType Then
					Dim counter As Integer = 0
					Do While counter < data.Length AndAlso data(counter).type = ElementSpec.EndTagType
						counter += 1
					Loop
					Dim change As ElemChanges = insertPath(insertPath.Length - counter - 1)
					change.index -= 1
					change.removed.Insert(0, change.parent.getElement(change.index))
				End If
			End Sub

			''' <summary>
			''' Updates the element structure in response to a removal from the
			''' associated sequence in the document.  Any elements consumed by the
			''' span of the removal are removed.
			''' </summary>
			Protected Friend Overridable Sub removeUpdate()
				removeElements(root, offset, offset + length)
			End Sub

			''' <summary>
			''' Updates the element structure in response to a change in the
			''' document.
			''' </summary>
			Protected Friend Overridable Sub changeUpdate()
				Dim didEnd As Boolean = Split(offset, length)
				If Not didEnd Then
					' need to do the other end
					Do While path.Count <> 0
						pop()
					Loop
					Split(offset + length, 0)
				End If
				Do While path.Count <> 0
					pop()
				Loop
			End Sub

			Friend Overridable Function split(ByVal offs As Integer, ByVal len As Integer) As Boolean
				Dim splitEnd As Boolean = False
				' push the path
				Dim e As Element = root
				Dim index As Integer = e.getElementIndex(offs)
				Do While Not e.leaf
					push(e, index)
					e = e.getElement(index)
					index = e.getElementIndex(offs)
				Loop

				Dim ec As ElemChanges = path.Peek()
				Dim child As Element = ec.parent.getElement(ec.index)
				' make sure there is something to do... if the
				' offset is already at a boundary then there is
				' nothing to do.
				If child.startOffset < offs AndAlso offs < child.endOffset Then
					' we need to split, now see if the other end is within
					' the same parent.
					Dim index0 As Integer = ec.index
					Dim index1 As Integer = index0
					If ((offs + len) < ec.parent.endOffset) AndAlso (len <> 0) Then
						' it's a range split in the same parent
						index1 = ec.parent.getElementIndex(offs+len)
						If index1 = index0 Then
							' it's a three-way split
							ec.removed.Add(child)
							e = outerInstance.createLeafElement(ec.parent, child.attributes, child.startOffset, offs)
							ec.added.Add(e)
							e = outerInstance.createLeafElement(ec.parent, child.attributes, offs, offs + len)
							ec.added.Add(e)
							e = outerInstance.createLeafElement(ec.parent, child.attributes, offs + len, child.endOffset)
							ec.added.Add(e)
							Return True
						Else
							child = ec.parent.getElement(index1)
							If (offs + len) = child.startOffset Then index1 = index0
						End If
						splitEnd = True
					End If

					' split the first location
					pos = offs
					child = ec.parent.getElement(index0)
					ec.removed.Add(child)
					e = outerInstance.createLeafElement(ec.parent, child.attributes, child.startOffset, pos)
					ec.added.Add(e)
					e = outerInstance.createLeafElement(ec.parent, child.attributes, pos, child.endOffset)
					ec.added.Add(e)

					' pick up things in the middle
					For i As Integer = index0 + 1 To index1 - 1
						child = ec.parent.getElement(i)
						ec.removed.Add(child)
						ec.added.Add(child)
					Next i

					If index1 <> index0 Then
						child = ec.parent.getElement(index1)
						pos = offs + len
						ec.removed.Add(child)
						e = outerInstance.createLeafElement(ec.parent, child.attributes, child.startOffset, pos)
						ec.added.Add(e)
						e = outerInstance.createLeafElement(ec.parent, child.attributes, pos, child.endOffset)
						ec.added.Add(e)
					End If
				End If
				Return splitEnd
			End Function

			''' <summary>
			''' Creates the UndoableEdit record for the edits made
			''' in the buffer.
			''' </summary>
			Friend Overridable Sub endEdits(ByVal de As DefaultDocumentEvent)
				Dim n As Integer = changes.Count
				For i As Integer = 0 To n - 1
					Dim ec As ElemChanges = changes(i)
					Dim removed As Element() = New Element(ec.removed.Count - 1){}
					ec.removed.CopyTo(removed)
					Dim added As Element() = New Element(ec.added.Count - 1){}
					ec.added.CopyTo(added)
					Dim index As Integer = ec.index
					CType(ec.parent, BranchElement).replace(index, removed.Length, added)
					Dim ee As New ElementEdit(ec.parent, index, removed, added)
					de.addEdit(ee)
				Next i

				changes.Clear()
				path.removeAllElements()

	'            
	'            for (int i = 0; i < n; i++) {
	'                ElemChanges ec = (ElemChanges) changes.elementAt(i);
	'                System.err.print("edited: " + ec.parent + " at: " + ec.index +
	'                    " removed " + ec.removed.size());
	'                if (ec.removed.size() > 0) {
	'                    int r0 = ((Element) ec.removed.firstElement()).getStartOffset();
	'                    int r1 = ((Element) ec.removed.lastElement()).getEndOffset();
	'                    System.err.print("[" + r0 + "," + r1 + "]");
	'                }
	'                System.err.print(" added " + ec.added.size());
	'                if (ec.added.size() > 0) {
	'                    int p0 = ((Element) ec.added.firstElement()).getStartOffset();
	'                    int p1 = ((Element) ec.added.lastElement()).getEndOffset();
	'                    System.err.print("[" + p0 + "," + p1 + "]");
	'                }
	'                System.err.println("");
	'            }
	'            
			End Sub

			''' <summary>
			''' Initialize the buffer
			''' </summary>
			Friend Overridable Sub beginEdits(ByVal offset As Integer, ByVal length As Integer)
				Me.offset = offset
				Me.length = length
				Me.endOffset = offset + length
				pos = offset
				If changes Is Nothing Then
					changes = New List(Of ElemChanges)
				Else
					changes.Clear()
				End If
				If path Is Nothing Then
					path = New Stack(Of ElemChanges)
				Else
					path.removeAllElements()
				End If
				fracturedParent = Nothing
				fracturedChild = Nothing
					offsetLastIndexOnReplace = False
					offsetLastIndex = offsetLastIndexOnReplace
			End Sub

			''' <summary>
			''' Pushes a new element onto the stack that represents
			''' the current path. </summary>
			''' <param name="record"> Whether or not the push should be
			'''  recorded as an element change or not. </param>
			''' <param name="isFracture"> true if pushing on an element that was created
			''' as the result of a fracture. </param>
			Friend Overridable Sub push(ByVal e As Element, ByVal index As Integer, ByVal isFracture As Boolean)
				Dim ec As New ElemChanges(Me, e, index, isFracture)
				path.Push(ec)
			End Sub

			Friend Overridable Sub push(ByVal e As Element, ByVal index As Integer)
				push(e, index, False)
			End Sub

			Friend Overridable Sub pop()
				Dim ec As ElemChanges = path.Peek()
				path.Pop()
				If (ec.added.Count > 0) OrElse (ec.removed.Count > 0) Then
					changes.Add(ec)
				ElseIf path.Count > 0 Then
					Dim e As Element = ec.parent
					If e.elementCount = 0 Then
						' if we pushed a branch element that didn't get
						' used, make sure its not marked as having been added.
						ec = path.Peek()
						ec.added.Remove(e)
					End If
				End If
			End Sub

			''' <summary>
			''' move the current offset forward by n.
			''' </summary>
			Friend Overridable Sub advance(ByVal n As Integer)
				pos += n
			End Sub

			Friend Overridable Sub insertElement(ByVal es As ElementSpec)
				Dim ec As ElemChanges = path.Peek()
				Select Case es.type
				Case ElementSpec.StartTagType
					Select Case es.direction
					Case ElementSpec.JoinNextDirection
						' Don't create a new element, use the existing one
						' at the specified location.
						Dim parent As Element = ec.parent.getElement(ec.index)

						If parent.leaf Then
							' This happens if inserting into a leaf, followed
							' by a join next where next sibling is not a leaf.
							If (ec.index + 1) < ec.parent.elementCount Then
								parent = ec.parent.getElement(ec.index + 1)
							Else
								Throw New StateInvariantError("Join next to leaf")
							End If
						End If
						' Not really a fracture, but need to treat it like
						' one so that content join next will work correctly.
						' We can do this because there will never be a join
						' next followed by a join fracture.
						push(parent, 0, True)
					Case ElementSpec.JoinFractureDirection
						If Not createdFracture Then fracture(path.Count - 1)
						' If parent isn't a fracture, fracture will be
						' fracturedChild.
						If Not ec.isFracture Then
							push(fracturedChild, 0, True)
						Else
							' Parent is a fracture, use 1st element.
							push(ec.parent.getElement(0), 0, True)
						End If
					Case Else
						Dim belem As Element = outerInstance.createBranchElement(ec.parent, es.attributes)
						ec.added.Add(belem)
						push(belem, 0)
					End Select
				Case ElementSpec.EndTagType
					pop()
				Case ElementSpec.ContentType
				  Dim len As Integer = es.length
					If es.direction <> ElementSpec.JoinNextDirection Then
						Dim leaf As Element = outerInstance.createLeafElement(ec.parent, es.attributes, pos, pos + len)
						ec.added.Add(leaf)
					Else
						' JoinNext on tail is only applicable if last element
						' and attributes come from that of first element.
						' With a little extra testing it would be possible
						' to NOT due this again, as more than likely fracture()
						' created this element.
						If Not ec.isFracture Then
							Dim first As Element = Nothing
							If insertPath IsNot Nothing Then
								For counter As Integer = insertPath.Length - 1 To 0 Step -1
									If insertPath(counter) Is ec Then
										If counter <> (insertPath.Length - 1) Then first = ec.parent.getElement(ec.index)
										Exit For
									End If
								Next counter
							End If
							If first Is Nothing Then first = ec.parent.getElement(ec.index + 1)
							Dim leaf As Element = outerInstance.createLeafElement(ec.parent, first.attributes, pos, first.endOffset)
							ec.added.Add(leaf)
							ec.removed.Add(first)
						Else
							' Parent was fractured element.
							Dim first As Element = ec.parent.getElement(0)
							Dim leaf As Element = outerInstance.createLeafElement(ec.parent, first.attributes, pos, first.endOffset)
							ec.added.Add(leaf)
							ec.removed.Add(first)
						End If
					End If
					pos += len
				End Select
			End Sub

			''' <summary>
			''' Remove the elements from <code>elem</code> in range
			''' <code>rmOffs0</code>, <code>rmOffs1</code>. This uses
			''' <code>canJoin</code> and <code>join</code> to handle joining
			''' the endpoints of the insertion.
			''' </summary>
			''' <returns> true if elem will no longer have any elements. </returns>
			Friend Overridable Function removeElements(ByVal elem As Element, ByVal rmOffs0 As Integer, ByVal rmOffs1 As Integer) As Boolean
				If Not elem.leaf Then
					' update path for changes
					Dim index0 As Integer = elem.getElementIndex(rmOffs0)
					Dim index1 As Integer = elem.getElementIndex(rmOffs1)
					push(elem, index0)
					Dim ec As ElemChanges = path.Peek()

					' if the range is contained by one element,
					' we just forward the request
					If index0 = index1 Then
						Dim child0 As Element = elem.getElement(index0)
						If rmOffs0 <= child0.startOffset AndAlso rmOffs1 >= child0.endOffset Then
							' Element totally removed.
							ec.removed.Add(child0)
						ElseIf removeElements(child0, rmOffs0, rmOffs1) Then
							ec.removed.Add(child0)
						End If
					Else
						' the removal range spans elements.  If we can join
						' the two endpoints, do it.  Otherwise we remove the
						' interior and forward to the endpoints.
						Dim child0 As Element = elem.getElement(index0)
						Dim child1 As Element = elem.getElement(index1)
						Dim containsOffs1 As Boolean = (rmOffs1 < elem.endOffset)
						If containsOffs1 AndAlso canJoin(child0, child1) Then
							' remove and join
							For i As Integer = index0 To index1
								ec.removed.Add(elem.getElement(i))
							Next i
							Dim e As Element = join(elem, child0, child1, rmOffs0, rmOffs1)
							ec.added.Add(e)
						Else
							' remove interior and forward
							Dim rmIndex0 As Integer = index0 + 1
							Dim rmIndex1 As Integer = index1 - 1
							If child0.startOffset = rmOffs0 OrElse (index0 = 0 AndAlso child0.startOffset > rmOffs0 AndAlso child0.endOffset <= rmOffs1) Then
								' start element completely consumed
								child0 = Nothing
								rmIndex0 = index0
							End If
							If Not containsOffs1 Then
								child1 = Nothing
								rmIndex1 += 1
							ElseIf child1.startOffset = rmOffs1 Then
								' end element not touched
								child1 = Nothing
							End If
							If rmIndex0 <= rmIndex1 Then ec.index = rmIndex0
							For i As Integer = rmIndex0 To rmIndex1
								ec.removed.Add(elem.getElement(i))
							Next i
							If child0 IsNot Nothing Then
								If removeElements(child0, rmOffs0, rmOffs1) Then
									ec.removed.Insert(0, child0)
									ec.index = index0
								End If
							End If
							If child1 IsNot Nothing Then
								If removeElements(child1, rmOffs0, rmOffs1) Then ec.removed.Add(child1)
							End If
						End If
					End If

					' publish changes
					pop()

					' Return true if we no longer have any children.
					If elem.elementCount = (ec.removed.Count - ec.added.Count) Then Return True
				End If
				Return False
			End Function

			''' <summary>
			''' Can the two given elements be coelesced together
			''' into one element?
			''' </summary>
			Friend Overridable Function canJoin(ByVal e0 As Element, ByVal e1 As Element) As Boolean
				If (e0 Is Nothing) OrElse (e1 Is Nothing) Then Return False
				' Don't join a leaf to a branch.
				Dim leaf0 As Boolean = e0.leaf
				Dim leaf1 As Boolean = e1.leaf
				If leaf0 <> leaf1 Then Return False
				If leaf0 Then Return e0.attributes.isEqual(e1.attributes)
				' Only join non-leafs if the names are equal. This may result
				' in loss of style information, but this is typically acceptable
				' for non-leafs.
				Dim name0 As String = e0.name
				Dim name1 As String = e1.name
				If name0 IsNot Nothing Then Return name0.Equals(name1)
				If name1 IsNot Nothing Then Return name1.Equals(name0)
				' Both names null, treat as equal.
				Return True
			End Function

			''' <summary>
			''' Joins the two elements carving out a hole for the
			''' given removed range.
			''' </summary>
			Friend Overridable Function join(ByVal p As Element, ByVal left As Element, ByVal right As Element, ByVal rmOffs0 As Integer, ByVal rmOffs1 As Integer) As Element
				If left.leaf AndAlso right.leaf Then
					Return outerInstance.createLeafElement(p, left.attributes, left.startOffset, right.endOffset)
				ElseIf ((Not left.leaf)) AndAlso ((Not right.leaf)) Then
					' join two branch elements.  This copies the children before
					' the removal range on the left element, and after the removal
					' range on the right element.  The two elements on the edge
					' are joined if possible and needed.
					Dim [to] As Element = outerInstance.createBranchElement(p, left.attributes)
					Dim ljIndex As Integer = left.getElementIndex(rmOffs0)
					Dim rjIndex As Integer = right.getElementIndex(rmOffs1)
					Dim lj As Element = left.getElement(ljIndex)
					If lj.startOffset >= rmOffs0 Then lj = Nothing
					Dim rj As Element = right.getElement(rjIndex)
					If rj.startOffset = rmOffs1 Then rj = Nothing
					Dim children As New List(Of Element)

					' transfer the left
					For i As Integer = 0 To ljIndex - 1
						children.Add(clone([to], left.getElement(i)))
					Next i

					' transfer the join/middle
					If canJoin(lj, rj) Then
						Dim e As Element = join([to], lj, rj, rmOffs0, rmOffs1)
						children.Add(e)
					Else
						If lj IsNot Nothing Then children.Add(cloneAsNecessary([to], lj, rmOffs0, rmOffs1))
						If rj IsNot Nothing Then children.Add(cloneAsNecessary([to], rj, rmOffs0, rmOffs1))
					End If

					' transfer the right
					Dim n As Integer = right.elementCount
					Dim i As Integer = If(rj Is Nothing, rjIndex, rjIndex + 1)
					Do While i < n
						children.Add(clone([to], right.getElement(i)))
						i += 1
					Loop

					' install the children
					Dim c As Element() = New Element(children.Count - 1){}
					children.CopyTo(c)
					CType([to], BranchElement).replace(0, 0, c)
					Return [to]
				Else
					Throw New StateInvariantError("No support to join leaf element with non-leaf element")
				End If
			End Function

			''' <summary>
			''' Creates a copy of this element, with a different
			''' parent.
			''' </summary>
			''' <param name="parent"> the parent element </param>
			''' <param name="clonee"> the element to be cloned </param>
			''' <returns> the copy </returns>
			Public Overridable Function clone(ByVal parent As Element, ByVal clonee As Element) As Element
				If clonee.leaf Then Return outerInstance.createLeafElement(parent, clonee.attributes, clonee.startOffset, clonee.endOffset)
				Dim e As Element = outerInstance.createBranchElement(parent, clonee.attributes)
				Dim n As Integer = clonee.elementCount
				Dim children As Element() = New Element(n - 1){}
				For i As Integer = 0 To n - 1
					children(i) = clone(e, clonee.getElement(i))
				Next i
				CType(e, BranchElement).replace(0, 0, children)
				Return e
			End Function

			''' <summary>
			''' Creates a copy of this element, with a different
			''' parent. Children of this element included in the
			''' removal range will be discarded.
			''' </summary>
			Friend Overridable Function cloneAsNecessary(ByVal parent As Element, ByVal clonee As Element, ByVal rmOffs0 As Integer, ByVal rmOffs1 As Integer) As Element
				If clonee.leaf Then Return outerInstance.createLeafElement(parent, clonee.attributes, clonee.startOffset, clonee.endOffset)
				Dim e As Element = outerInstance.createBranchElement(parent, clonee.attributes)
				Dim n As Integer = clonee.elementCount
				Dim childrenList As New List(Of Element)(n)
				For i As Integer = 0 To n - 1
					Dim elem As Element = clonee.getElement(i)
					If elem.startOffset < rmOffs0 OrElse elem.endOffset > rmOffs1 Then childrenList.Add(cloneAsNecessary(e, elem, rmOffs0, rmOffs1))
				Next i
				Dim children As Element() = New Element(childrenList.Count - 1){}
				children = childrenList.ToArray(children)
				CType(e, BranchElement).replace(0, 0, children)
				Return e
			End Function

			''' <summary>
			''' Determines if a fracture needs to be performed. A fracture
			''' can be thought of as moving the right part of a tree to a
			''' new location, where the right part is determined by what has
			''' been inserted. <code>depth</code> is used to indicate a
			''' JoinToFracture is needed to an element at a depth
			''' of <code>depth</code>. Where the root is 0, 1 is the children
			''' of the root...
			''' <p>This will invoke <code>fractureFrom</code> if it is determined
			''' a fracture needs to happen.
			''' </summary>
			Friend Overridable Sub fracture(ByVal depth As Integer)
				Dim cLength As Integer = insertPath.Length
				Dim lastIndex As Integer = -1
				Dim needRecreate As Boolean = recreateLeafs
				Dim lastChange As ElemChanges = insertPath(cLength - 1)
				' Use childAltered to determine when a child has been altered,
				' that is the point of insertion is less than the element count.
				Dim childAltered As Boolean = ((lastChange.index + 1) < lastChange.parent.elementCount)
				Dim deepestAlteredIndex As Integer = If(needRecreate, cLength, -1)
				Dim lastAlteredIndex As Integer = cLength - 1

				createdFracture = True
				' Determine where to start recreating from.
				' Start at - 2, as first one is indicated by recreateLeafs and
				' childAltered.
				For counter As Integer = cLength - 2 To 0 Step -1
					Dim change As ElemChanges = insertPath(counter)
					If change.added.Count > 0 OrElse counter = depth Then
						lastIndex = counter
						If (Not needRecreate) AndAlso childAltered Then
							needRecreate = True
							If deepestAlteredIndex = -1 Then deepestAlteredIndex = lastAlteredIndex + 1
						End If
					End If
					If (Not childAltered) AndAlso change.index < change.parent.elementCount Then
						childAltered = True
						lastAlteredIndex = counter
					End If
				Next counter
				If needRecreate Then
					' Recreate all children to right of parent starting
					' at lastIndex.
					If lastIndex = -1 Then lastIndex = cLength - 1
					fractureFrom(insertPath, lastIndex, deepestAlteredIndex)
				End If
			End Sub

			''' <summary>
			''' Recreates the elements to the right of the insertion point.
			''' This starts at <code>startIndex</code> in <code>changed</code>,
			''' and calls duplicate to duplicate existing elements.
			''' This will also duplicate the elements along the insertion
			''' point, until a depth of <code>endFractureIndex</code> is
			''' reached, at which point only the elements to the right of
			''' the insertion point are duplicated.
			''' </summary>
			Friend Overridable Sub fractureFrom(ByVal changed As ElemChanges(), ByVal startIndex As Integer, ByVal endFractureIndex As Integer)
				' Recreate the element representing the inserted index.
				Dim change As ElemChanges = changed(startIndex)
				Dim child As Element
				Dim newChild As Element
				Dim changeLength As Integer = changed.Length

				If (startIndex + 1) = changeLength Then
					child = change.parent.getElement(change.index)
				Else
					child = change.parent.getElement(change.index - 1)
				End If
				If child.leaf Then
					newChild = outerInstance.createLeafElement(change.parent, child.attributes, Math.Max(endOffset, child.startOffset), child.endOffset)
				Else
					newChild = outerInstance.createBranchElement(change.parent, child.attributes)
				End If
				fracturedParent = change.parent
				fracturedChild = newChild

				' Recreate all the elements to the right of the
				' insertion point.
				Dim parent As Element = newChild

				startIndex += 1
				Do While startIndex < endFractureIndex
					Dim isEnd As Boolean = ((startIndex + 1) = endFractureIndex)
					Dim isEndLeaf As Boolean = ((startIndex + 1) = changeLength)

					' Create the newChild, a duplicate of the elment at
					' index. This isn't done if isEnd and offsetLastIndex are true
					' indicating a join previous was done.
					change = changed(startIndex)

					' Determine the child to duplicate, won't have to duplicate
					' if at end of fracture, or offseting index.
					If isEnd Then
						If offsetLastIndex OrElse (Not isEndLeaf) Then
							child = Nothing
						Else
							child = change.parent.getElement(change.index)
						End If
					Else
						child = change.parent.getElement(change.index - 1)
					End If
					' Duplicate it.
					If child IsNot Nothing Then
						If child.leaf Then
							newChild = outerInstance.createLeafElement(parent, child.attributes, Math.Max(endOffset, child.startOffset), child.endOffset)
						Else
							newChild = outerInstance.createBranchElement(parent, child.attributes)
						End If
					Else
						newChild = Nothing
					End If

					' Recreate the remaining children (there may be none).
					Dim kidsToMove As Integer = change.parent.elementCount - change.index
					Dim kids As Element()
					Dim moveStartIndex As Integer
					Dim kidStartIndex As Integer = 1

					If newChild Is Nothing Then
						' Last part of fracture.
						If isEndLeaf Then
							kidsToMove -= 1
							moveStartIndex = change.index + 1
						Else
							moveStartIndex = change.index
						End If
						kidStartIndex = 0
						kids = New Element(kidsToMove - 1){}
					Else
						If Not isEnd Then
							' Branch.
							kidsToMove += 1
							moveStartIndex = change.index
						Else
							' Last leaf, need to recreate part of it.
							moveStartIndex = change.index + 1
						End If
						kids = New Element(kidsToMove - 1){}
						kids(0) = newChild
					End If

					For counter As Integer = kidStartIndex To kidsToMove - 1
						Dim toMove As Element =change.parent.getElement(moveStartIndex)
						moveStartIndex += 1
						kids(counter) = recreateFracturedElement(parent, toMove)
						change.removed.Add(toMove)
					Next counter
					CType(parent, BranchElement).replace(0, 0, kids)
					parent = newChild
					startIndex += 1
				Loop
			End Sub

			''' <summary>
			''' Recreates <code>toDuplicate</code>. This is called when an
			''' element needs to be created as the result of an insertion. This
			''' will recurse and create all the children. This is similar to
			''' <code>clone</code>, but deteremines the offsets differently.
			''' </summary>
			Friend Overridable Function recreateFracturedElement(ByVal parent As Element, ByVal toDuplicate As Element) As Element
				If toDuplicate.leaf Then Return outerInstance.createLeafElement(parent, toDuplicate.attributes, Math.Max(toDuplicate.startOffset, endOffset), toDuplicate.endOffset)
				' Not a leaf
				Dim newParent As Element = outerInstance.createBranchElement(parent, toDuplicate.attributes)
				Dim childCount As Integer = toDuplicate.elementCount
				Dim newKids As Element() = New Element(childCount - 1){}
				For counter As Integer = 0 To childCount - 1
					newKids(counter) = recreateFracturedElement(newParent, toDuplicate.getElement(counter))
				Next counter
				CType(newParent, BranchElement).replace(0, 0, newKids)
				Return newParent
			End Function

			''' <summary>
			''' Splits the bottommost leaf in <code>path</code>.
			''' This is called from insert when the first element is NOT content.
			''' </summary>
			Friend Overridable Sub fractureDeepestLeaf(ByVal specs As ElementSpec())
				' Split the bottommost leaf. It will be recreated elsewhere.
				Dim ec As ElemChanges = path.Peek()
				Dim child As Element = ec.parent.getElement(ec.index)
				' Inserts at offset 0 do not need to recreate child (it would
				' have a length of 0!).
				If offset <> 0 Then
					Dim newChild As Element = outerInstance.createLeafElement(ec.parent, child.attributes, child.startOffset, offset)

					ec.added.Add(newChild)
				End If
				ec.removed.Add(child)
				If child.endOffset <> endOffset Then
					recreateLeafs = True
				Else
					offsetLastIndex = True
				End If
			End Sub

			''' <summary>
			''' Inserts the first content. This needs to be separate to handle
			''' joining.
			''' </summary>
			Friend Overridable Sub insertFirstContent(ByVal specs As ElementSpec())
				Dim firstSpec As ElementSpec = specs(0)
				Dim ec As ElemChanges = path.Peek()
				Dim child As Element = ec.parent.getElement(ec.index)
				Dim firstEndOffset As Integer = offset + firstSpec.length
				Dim isOnlyContent As Boolean = (specs.Length = 1)

				Select Case firstSpec.direction
				Case ElementSpec.JoinPreviousDirection
					If child.endOffset <> firstEndOffset AndAlso (Not isOnlyContent) Then
						' Create the left split part containing new content.
						Dim newE As Element = outerInstance.createLeafElement(ec.parent, child.attributes, child.startOffset, firstEndOffset)
						ec.added.Add(newE)
						ec.removed.Add(child)
						' Remainder will be created later.
						If child.endOffset <> endOffset Then
							recreateLeafs = True
						Else
							offsetLastIndex = True
						End If
					Else
						offsetLastIndex = True
						offsetLastIndexOnReplace = True
					End If
					' else Inserted at end, and is total length.
					' Update index incase something added/removed.
				Case ElementSpec.JoinNextDirection
					If offset <> 0 Then
						' Recreate the first element, its offset will have
						' changed.
						Dim newE As Element = outerInstance.createLeafElement(ec.parent, child.attributes, child.startOffset, offset)
						ec.added.Add(newE)
						' Recreate the second, merge part. We do no checking
						' to see if JoinNextDirection is valid here!
						Dim nextChild As Element = ec.parent.getElement(ec.index + 1)
						If isOnlyContent Then
							newE = outerInstance.createLeafElement(ec.parent, nextChild.attributes, offset, nextChild.endOffset)
						Else
							newE = outerInstance.createLeafElement(ec.parent, nextChild.attributes, offset, firstEndOffset)
						End If
						ec.added.Add(newE)
						ec.removed.Add(child)
						ec.removed.Add(nextChild)
					End If
					' else nothin to do.
					' PENDING: if !isOnlyContent could raise here!
				Case Else
					' Inserted into middle, need to recreate split left
					' new content, and split right.
					If child.startOffset <> offset Then
						Dim newE As Element = outerInstance.createLeafElement(ec.parent, child.attributes, child.startOffset, offset)
						ec.added.Add(newE)
					End If
					ec.removed.Add(child)
					' new content
					Dim newE As Element = outerInstance.createLeafElement(ec.parent, firstSpec.attributes, offset, firstEndOffset)
					ec.added.Add(newE)
					If child.endOffset <> endOffset Then
						' Signals need to recreate right split later.
						recreateLeafs = True
					Else
						offsetLastIndex = True
					End If
				End Select
			End Sub

			Friend root As Element
			<NonSerialized> _
			Friend pos As Integer ' current position
			<NonSerialized> _
			Friend offset As Integer
			<NonSerialized> _
			Friend length As Integer
			<NonSerialized> _
			Friend endOffset As Integer
			<NonSerialized> _
			Friend changes As List(Of ElemChanges)
			<NonSerialized> _
			Friend path As Stack(Of ElemChanges)
			<NonSerialized> _
			Friend insertOp As Boolean

			<NonSerialized> _
			Friend recreateLeafs As Boolean ' For insert.

			''' <summary>
			''' For insert, path to inserted elements. </summary>
			<NonSerialized> _
			Friend insertPath As ElemChanges()
			''' <summary>
			''' Only for insert, set to true when the fracture has been created. </summary>
			<NonSerialized> _
			Friend createdFracture As Boolean
			''' <summary>
			''' Parent that contains the fractured child. </summary>
			<NonSerialized> _
			Friend fracturedParent As Element
			''' <summary>
			''' Fractured child. </summary>
			<NonSerialized> _
			Friend fracturedChild As Element
			''' <summary>
			''' Used to indicate when fracturing that the last leaf should be
			''' skipped. 
			''' </summary>
			<NonSerialized> _
			Friend offsetLastIndex As Boolean
			''' <summary>
			''' Used to indicate that the parent of the deepest leaf should
			''' offset the index by 1 when adding/removing elements in an
			''' insert. 
			''' </summary>
			<NonSerialized> _
			Friend offsetLastIndexOnReplace As Boolean

	'        
	'         * Internal record used to hold element change specifications
	'         
			Friend Class ElemChanges
				Private ReadOnly outerInstance As DefaultStyledDocument.ElementBuffer


				Friend Sub New(ByVal outerInstance As DefaultStyledDocument.ElementBuffer, ByVal parent As Element, ByVal index As Integer, ByVal isFracture As Boolean)
						Me.outerInstance = outerInstance
					Me.parent = parent
					Me.index = index
					Me.isFracture = isFracture
					added = New List(Of Element)
					removed = New List(Of Element)
				End Sub

				Public Overrides Function ToString() As String
					Return "added: " & added & vbLf & "removed: " & removed & vbLf
				End Function

				Friend parent As Element
				Friend index As Integer
				Friend added As List(Of Element)
				Friend removed As List(Of Element)
				Friend isFracture As Boolean
			End Class

		End Class

		''' <summary>
		''' An UndoableEdit used to remember AttributeSet changes to an
		''' Element.
		''' </summary>
		Public Class AttributeUndoableEdit
			Inherits javax.swing.undo.AbstractUndoableEdit

			Public Sub New(ByVal element As Element, ByVal newAttributes As AttributeSet, ByVal isReplacing As Boolean)
				MyBase.New()
				Me.element = element
				Me.newAttributes = newAttributes
				Me.isReplacing = isReplacing
				' If not replacing, it may be more efficient to only copy the
				' changed values...
				copy = element.attributes.copyAttributes()
			End Sub

			''' <summary>
			''' Redoes a change.
			''' </summary>
			''' <exception cref="CannotRedoException"> if the change cannot be redone </exception>
			Public Overrides Sub redo()
				MyBase.redo()
				Dim [as] As MutableAttributeSet = CType(element.attributes, MutableAttributeSet)
				If isReplacing Then [as].removeAttributes([as])
				[as].addAttributes(newAttributes)
			End Sub

			''' <summary>
			''' Undoes a change.
			''' </summary>
			''' <exception cref="CannotUndoException"> if the change cannot be undone </exception>
			Public Overrides Sub undo()
				MyBase.undo()
				Dim [as] As MutableAttributeSet = CType(element.attributes, MutableAttributeSet)
				[as].removeAttributes([as])
				[as].addAttributes(copy)
			End Sub

			' AttributeSet containing additional entries, must be non-mutable!
			Protected Friend newAttributes As AttributeSet
			' Copy of the AttributeSet the Element contained.
			Protected Friend copy As AttributeSet
			' true if all the attributes in the element were removed first.
			Protected Friend isReplacing As Boolean
			' Efected Element.
			Protected Friend element As Element
		End Class

		''' <summary>
		''' UndoableEdit for changing the resolve parent of an Element.
		''' </summary>
		Friend Class StyleChangeUndoableEdit
			Inherits javax.swing.undo.AbstractUndoableEdit

			Public Sub New(ByVal element As AbstractElement, ByVal newStyle As Style)
				MyBase.New()
				Me.element = element
				Me.newStyle = newStyle
				oldStyle = element.resolveParent
			End Sub

			''' <summary>
			''' Redoes a change.
			''' </summary>
			''' <exception cref="CannotRedoException"> if the change cannot be redone </exception>
			Public Overrides Sub redo()
				MyBase.redo()
				element.resolveParent = newStyle
			End Sub

			''' <summary>
			''' Undoes a change.
			''' </summary>
			''' <exception cref="CannotUndoException"> if the change cannot be undone </exception>
			Public Overrides Sub undo()
				MyBase.undo()
				element.resolveParent = oldStyle
			End Sub

			''' <summary>
			''' Element to change resolve parent of. </summary>
			Protected Friend element As AbstractElement
			''' <summary>
			''' New style. </summary>
			Protected Friend newStyle As Style
			''' <summary>
			''' Old style, before setting newStyle. </summary>
			Protected Friend oldStyle As AttributeSet
		End Class

		''' <summary>
		''' Base class for style change handlers with support for stale objects detection.
		''' </summary>
		Friend MustInherit Class AbstractChangeHandler
			Implements ChangeListener

			' This has an implicit reference to the handler object.  
			Private Class DocReference
				Inherits WeakReference(Of DefaultStyledDocument)

				Private ReadOnly outerInstance As DefaultStyledDocument.AbstractChangeHandler


				Friend Sub New(ByVal outerInstance As DefaultStyledDocument.AbstractChangeHandler, ByVal d As DefaultStyledDocument, ByVal q As ReferenceQueue(Of DefaultStyledDocument))
						Me.outerInstance = outerInstance
					MyBase.New(d, q)
				End Sub

				''' <summary>
				''' Return a reference to the style change handler object.
				''' </summary>
				Friend Overridable Property listener As ChangeListener
					Get
						Return AbstractChangeHandler.this
					End Get
				End Property
			End Class

			''' <summary>
			''' Class-specific reference queues. </summary>
			Private Shared ReadOnly queueMap As IDictionary(Of Type, ReferenceQueue(Of DefaultStyledDocument)) = New Dictionary(Of Type, ReferenceQueue(Of DefaultStyledDocument))

			''' <summary>
			''' A weak reference to the document object. </summary>
			Private doc As DocReference

			Friend Sub New(ByVal d As DefaultStyledDocument)
				Dim c As Type = Me.GetType()
				Dim q As ReferenceQueue(Of DefaultStyledDocument)
				SyncLock queueMap
					q = queueMap(c)
					If q Is Nothing Then
						q = New ReferenceQueue(Of DefaultStyledDocument)
						queueMap(c) = q
					End If
				End SyncLock
				doc = New DocReference(Me, d, q)
			End Sub

			''' <summary>
			''' Return a list of stale change listeners.
			''' 
			''' A change listener becomes "stale" when its document is cleaned by GC.
			''' </summary>
			Friend Shared Function getStaleListeners(ByVal l As ChangeListener) As IList(Of ChangeListener)
				Dim ___staleListeners As IList(Of ChangeListener) = New List(Of ChangeListener)
				Dim q As ReferenceQueue(Of DefaultStyledDocument) = queueMap(l.GetType())

				If q IsNot Nothing Then
					Dim r As DocReference
					SyncLock q
						r = CType(q.poll(), DocReference)
						Do While r IsNot Nothing
							___staleListeners.Add(r.listener)
							r = CType(q.poll(), DocReference)
						Loop
					End SyncLock
				End If

				Return ___staleListeners
			End Function

			''' <summary>
			''' The ChangeListener wrapper which guards against dead documents.
			''' </summary>
			Public Overridable Sub stateChanged(ByVal e As ChangeEvent) Implements ChangeListener.stateChanged
				Dim d As DefaultStyledDocument = doc.get()
				If d IsNot Nothing Then fireStateChanged(d, e)
			End Sub

			''' <summary>
			''' Run the actual class-specific stateChanged() method. </summary>
			Friend MustOverride Sub fireStateChanged(ByVal d As DefaultStyledDocument, ByVal e As ChangeEvent)
		End Class

		''' <summary>
		''' Added to all the Styles. When instances of this receive a
		''' stateChanged method, styleChanged is invoked.
		''' </summary>
		Friend Class StyleChangeHandler
			Inherits AbstractChangeHandler

			Friend Sub New(ByVal d As DefaultStyledDocument)
				MyBase.New(d)
			End Sub

			Friend Overrides Sub fireStateChanged(ByVal d As DefaultStyledDocument, ByVal e As ChangeEvent)
				Dim source As Object = e.source
				If TypeOf source Is Style Then
					d.styleChanged(CType(source, Style))
				Else
					d.styleChanged(Nothing)
				End If
			End Sub
		End Class


		''' <summary>
		''' Added to the StyleContext. When the StyleContext changes, this invokes
		''' <code>updateStylesListeningTo</code>.
		''' </summary>
		Friend Class StyleContextChangeHandler
			Inherits AbstractChangeHandler

			Friend Sub New(ByVal d As DefaultStyledDocument)
				MyBase.New(d)
			End Sub

			Friend Overrides Sub fireStateChanged(ByVal d As DefaultStyledDocument, ByVal e As ChangeEvent)
				d.updateStylesListeningTo()
			End Sub
		End Class


		''' <summary>
		''' When run this creates a change event for the complete document
		''' and fires it.
		''' </summary>
		Friend Class ChangeUpdateRunnable
			Implements Runnable

			Private ReadOnly outerInstance As DefaultStyledDocument

			Public Sub New(ByVal outerInstance As DefaultStyledDocument)
				Me.outerInstance = outerInstance
			End Sub

			Friend isPending As Boolean = False

			Public Overridable Sub run()
				SyncLock Me
					isPending = False
				End SyncLock

				Try
					outerInstance.writeLock()
					Dim dde As New DefaultDocumentEvent(0, outerInstance.length, DocumentEvent.EventType.CHANGE)
					dde.end()
					outerInstance.fireChangedUpdate(dde)
				Finally
					outerInstance.writeUnlock()
				End Try
			End Sub
		End Class
	End Class

End Namespace