Imports Microsoft.VisualBasic
Imports System
Imports javax.swing.event

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
	''' implementation which treats text as styled text and
	''' provides a minimal set of actions for editing styled text.
	''' 
	''' @author  Timothy Prinzing
	''' </summary>
	Public Class StyledEditorKit
		Inherits DefaultEditorKit

		''' <summary>
		''' Creates a new EditorKit used for styled documents.
		''' </summary>
		Public Sub New()
			createInputAttributeUpdated()
			createInputAttributes()
		End Sub

		''' <summary>
		''' Gets the input attributes for the pane.  When
		''' the caret moves and there is no selection, the
		''' input attributes are automatically mutated to
		''' reflect the character attributes of the current
		''' caret location.  The styled editing actions
		''' use the input attributes to carry out their
		''' actions.
		''' </summary>
		''' <returns> the attribute set </returns>
		Public Property Overrides inputAttributes As MutableAttributeSet
			Get
				Return inputAttributes
			End Get
		End Property

		''' <summary>
		''' Fetches the element representing the current
		''' run of character attributes for the caret.
		''' </summary>
		''' <returns> the element </returns>
		Public Overridable Property characterAttributeRun As Element
			Get
				Return currentRun
			End Get
		End Property

		' --- EditorKit methods ---------------------------

		''' <summary>
		''' Fetches the command list for the editor.  This is
		''' the list of commands supported by the superclass
		''' augmented by the collection of commands defined
		''' locally for style operations.
		''' </summary>
		''' <returns> the command list </returns>
		Public Property Overrides actions As javax.swing.Action()
			Get
				Return TextAction.augmentList(MyBase.actions, Me.defaultActions)
			End Get
		End Property

		''' <summary>
		''' Creates an uninitialized text storage model
		''' that is appropriate for this type of editor.
		''' </summary>
		''' <returns> the model </returns>
		Public Overrides Function createDefaultDocument() As Document
			Return New DefaultStyledDocument
		End Function

		''' <summary>
		''' Called when the kit is being installed into
		''' a JEditorPane.
		''' </summary>
		''' <param name="c"> the JEditorPane </param>
		Public Overrides Sub install(ByVal c As javax.swing.JEditorPane)
			c.addCaretListener(inputAttributeUpdater)
			c.addPropertyChangeListener(inputAttributeUpdater)
			Dim caret As Caret = c.caret
			If caret IsNot Nothing Then inputAttributeUpdater.updateInputAttributes(caret.dot, caret.mark, c)
		End Sub

		''' <summary>
		''' Called when the kit is being removed from the
		''' JEditorPane.  This is used to unregister any
		''' listeners that were attached.
		''' </summary>
		''' <param name="c"> the JEditorPane </param>
		Public Overrides Sub deinstall(ByVal c As javax.swing.JEditorPane)
			c.removeCaretListener(inputAttributeUpdater)
			c.removePropertyChangeListener(inputAttributeUpdater)

			' remove references to current document so it can be collected.
			currentRun = Nothing
			currentParagraph = Nothing
		End Sub

	   ''' <summary>
	   ''' Fetches a factory that is suitable for producing
	   ''' views of any models that are produced by this
	   ''' kit.  This is implemented to return View implementations
	   ''' for the following kinds of elements:
	   ''' <ul>
	   ''' <li>AbstractDocument.ContentElementName
	   ''' <li>AbstractDocument.ParagraphElementName
	   ''' <li>AbstractDocument.SectionElementName
	   ''' <li>StyleConstants.ComponentElementName
	   ''' <li>StyleConstants.IconElementName
	   ''' </ul>
	   ''' </summary>
	   ''' <returns> the factory </returns>
		Public Property Overrides viewFactory As ViewFactory
			Get
				Return defaultFactory
			End Get
		End Property

		''' <summary>
		''' Creates a copy of the editor kit.
		''' </summary>
		''' <returns> the copy </returns>
		Public Overrides Function clone() As Object
			Dim o As StyledEditorKit = CType(MyBase.clone(), StyledEditorKit)
				o.currentParagraph = Nothing
				o.currentRun = o.currentParagraph
			o.createInputAttributeUpdated()
			o.createInputAttributes()
			Return o
		End Function

		''' <summary>
		''' Creates the AttributeSet used for the selection.
		''' </summary>
		Private Sub createInputAttributes()
			inputAttributes = New SimpleAttributeSetAnonymousInnerClassHelper
		End Sub

		Private Class SimpleAttributeSetAnonymousInnerClassHelper
			Inherits SimpleAttributeSet

			Public Property Overrides resolveParent As AttributeSet
				Get
					Return If(outerInstance.currentParagraph IsNot Nothing, outerInstance.currentParagraph.attributes, Nothing)
				End Get
			End Property

			Public Overrides Function clone() As Object
				Return New SimpleAttributeSet(Me)
			End Function
		End Class

		''' <summary>
		''' Creates a new <code>AttributeTracker</code>.
		''' </summary>
		Private Sub createInputAttributeUpdated()
			inputAttributeUpdater = New AttributeTracker(Me)
		End Sub


		Private Shared ReadOnly defaultFactory As ViewFactory = New StyledViewFactory

		Friend currentRun As Element
		Friend currentParagraph As Element

		''' <summary>
		''' This is the set of attributes used to store the
		''' input attributes.
		''' </summary>
		Friend inputAttributes As MutableAttributeSet

		''' <summary>
		''' This listener will be attached to the caret of
		''' the text component that the EditorKit gets installed
		''' into.  This should keep the input attributes updated
		''' for use by the styled actions.
		''' </summary>
		Private inputAttributeUpdater As AttributeTracker

		''' <summary>
		''' Tracks caret movement and keeps the input attributes set
		''' to reflect the current set of attribute definitions at the
		''' caret position.
		''' <p>This implements PropertyChangeListener to update the
		''' input attributes when the Document changes, as if the Document
		''' changes the attributes will almost certainly change.
		''' </summary>
		<Serializable> _
		Friend Class AttributeTracker
			Implements CaretListener, java.beans.PropertyChangeListener

			Private ReadOnly outerInstance As StyledEditorKit

			Public Sub New(ByVal outerInstance As StyledEditorKit)
				Me.outerInstance = outerInstance
			End Sub


			''' <summary>
			''' Updates the attributes. <code>dot</code> and <code>mark</code>
			''' mark give the positions of the selection in <code>c</code>.
			''' </summary>
			Friend Overridable Sub updateInputAttributes(ByVal dot As Integer, ByVal mark As Integer, ByVal c As JTextComponent)
				' EditorKit might not have installed the StyledDocument yet.
				Dim aDoc As Document = c.document
				If Not(TypeOf aDoc Is StyledDocument) Then Return
				Dim start As Integer = Math.Min(dot, mark)
				' record current character attributes.
				Dim doc As StyledDocument = CType(aDoc, StyledDocument)
				' If nothing is selected, get the attributes from the character
				' before the start of the selection, otherwise get the attributes
				' from the character element at the start of the selection.
				Dim run As Element
				outerInstance.currentParagraph = doc.getParagraphElement(start)
				If outerInstance.currentParagraph.startOffset = start OrElse dot <> mark Then
					' Get the attributes from the character at the selection
					' if in a different paragrah!
					run = doc.getCharacterElement(start)
				Else
					run = doc.getCharacterElement(Math.Max(start-1, 0))
				End If
				If run IsNot outerInstance.currentRun Then
	'                    
	'                     * PENDING(prinz) All attributes that represent a single
	'                     * glyph position and can't be inserted into should be
	'                     * removed from the input attributes... this requires
	'                     * mixing in an interface to indicate that condition.
	'                     * When we can add things again this logic needs to be
	'                     * improved!!
	'                     
					outerInstance.currentRun = run
					outerInstance.createInputAttributes(outerInstance.currentRun, outerInstance.inputAttributes)
				End If
			End Sub

			Public Overridable Sub propertyChange(ByVal evt As java.beans.PropertyChangeEvent)
				Dim newValue As Object = evt.newValue
				Dim source As Object = evt.source

				If (TypeOf source Is JTextComponent) AndAlso (TypeOf newValue Is Document) Then updateInputAttributes(0, 0, CType(source, JTextComponent))
			End Sub

			Public Overridable Sub caretUpdate(ByVal e As CaretEvent) Implements CaretListener.caretUpdate
				updateInputAttributes(e.dot, e.mark, CType(e.source, JTextComponent))
			End Sub
		End Class

		''' <summary>
		''' Copies the key/values in <code>element</code>s AttributeSet into
		''' <code>set</code>. This does not copy component, icon, or element
		''' names attributes. Subclasses may wish to refine what is and what
		''' isn't copied here. But be sure to first remove all the attributes that
		''' are in <code>set</code>.<p>
		''' This is called anytime the caret moves over a different location.
		''' 
		''' </summary>
		Protected Friend Overridable Sub createInputAttributes(ByVal element As Element, ByVal [set] As MutableAttributeSet)
			If element.attributes.attributeCount > 0 OrElse element.endOffset - element.startOffset > 1 OrElse element.endOffset < element.document.length Then
				[set].removeAttributes([set])
				[set].addAttributes(element.attributes)
				[set].removeAttribute(StyleConstants.ComponentAttribute)
				[set].removeAttribute(StyleConstants.IconAttribute)
				[set].removeAttribute(AbstractDocument.ElementNameAttribute)
				[set].removeAttribute(StyleConstants.ComposedTextAttribute)
			End If
		End Sub

		' ---- default ViewFactory implementation ---------------------

		Friend Class StyledViewFactory
			Implements ViewFactory

			Public Overridable Function create(ByVal elem As Element) As View Implements ViewFactory.create
				Dim kind As String = elem.name
				If kind IsNot Nothing Then
					If kind.Equals(AbstractDocument.ContentElementName) Then
						Return New LabelView(elem)
					ElseIf kind.Equals(AbstractDocument.ParagraphElementName) Then
						Return New ParagraphView(elem)
					ElseIf kind.Equals(AbstractDocument.SectionElementName) Then
						Return New BoxView(elem, View.Y_AXIS)
					ElseIf kind.Equals(StyleConstants.ComponentElementName) Then
						Return New ComponentView(elem)
					ElseIf kind.Equals(StyleConstants.IconElementName) Then
						Return New IconView(elem)
					End If
				End If

				' default to text display
				Return New LabelView(elem)
			End Function

		End Class

		' --- Action implementations ---------------------------------

		Private Shared ReadOnly defaultActions As javax.swing.Action() = { New FontFamilyAction("font-family-SansSerif", "SansSerif"), New FontFamilyAction("font-family-Monospaced", "Monospaced"), New FontFamilyAction("font-family-Serif", "Serif"), New FontSizeAction("font-size-8", 8), New FontSizeAction("font-size-10", 10), New FontSizeAction("font-size-12", 12), New FontSizeAction("font-size-14", 14), New FontSizeAction("font-size-16", 16), New FontSizeAction("font-size-18", 18), New FontSizeAction("font-size-24", 24), New FontSizeAction("font-size-36", 36), New FontSizeAction("font-size-48", 48), New AlignmentAction("left-justify", StyleConstants.ALIGN_LEFT), New AlignmentAction("center-justify", StyleConstants.ALIGN_CENTER), New AlignmentAction("right-justify", StyleConstants.ALIGN_RIGHT), New BoldAction, New ItalicAction, New StyledInsertBreakAction, New UnderlineAction }

		''' <summary>
		''' An action that assumes it's being fired on a JEditorPane
		''' with a StyledEditorKit (or subclass) installed.  This has
		''' some convenience methods for causing character or paragraph
		''' level attribute changes.  The convenience methods will
		''' throw an IllegalArgumentException if the assumption of
		''' a StyledDocument, a JEditorPane, or a StyledEditorKit
		''' fail to be true.
		''' <p>
		''' The component that gets acted upon by the action
		''' will be the source of the ActionEvent if the source
		''' can be narrowed to a JEditorPane type.  If the source
		''' can't be narrowed, the most recently focused text
		''' component is changed.  If neither of these are the
		''' case, the action cannot be performed.
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
		Public MustInherit Class StyledTextAction
			Inherits TextAction

			''' <summary>
			''' Creates a new StyledTextAction from a string action name.
			''' </summary>
			''' <param name="nm"> the name of the action </param>
			Public Sub New(ByVal nm As String)
				MyBase.New(nm)
			End Sub

			''' <summary>
			''' Gets the target editor for an action.
			''' </summary>
			''' <param name="e"> the action event </param>
			''' <returns> the editor </returns>
			Protected Friend Function getEditor(ByVal e As java.awt.event.ActionEvent) As javax.swing.JEditorPane
				Dim tcomp As JTextComponent = getTextComponent(e)
				If TypeOf tcomp Is javax.swing.JEditorPane Then Return CType(tcomp, javax.swing.JEditorPane)
				Return Nothing
			End Function

			''' <summary>
			''' Gets the document associated with an editor pane.
			''' </summary>
			''' <param name="e"> the editor </param>
			''' <returns> the document </returns>
			''' <exception cref="IllegalArgumentException"> for the wrong document type </exception>
			Protected Friend Function getStyledDocument(ByVal e As javax.swing.JEditorPane) As StyledDocument
				Dim d As Document = e.document
				If TypeOf d Is StyledDocument Then Return CType(d, StyledDocument)
				Throw New System.ArgumentException("document must be StyledDocument")
			End Function

			''' <summary>
			''' Gets the editor kit associated with an editor pane.
			''' </summary>
			''' <param name="e"> the editor pane </param>
			''' <returns> the kit </returns>
			''' <exception cref="IllegalArgumentException"> for the wrong document type </exception>
			Protected Friend Function getStyledEditorKit(ByVal e As javax.swing.JEditorPane) As StyledEditorKit
				Dim k As EditorKit = e.editorKit
				If TypeOf k Is StyledEditorKit Then Return CType(k, StyledEditorKit)
				Throw New System.ArgumentException("EditorKit must be StyledEditorKit")
			End Function

			''' <summary>
			''' Applies the given attributes to character
			''' content.  If there is a selection, the attributes
			''' are applied to the selection range.  If there
			''' is no selection, the attributes are applied to
			''' the input attribute set which defines the attributes
			''' for any new text that gets inserted.
			''' </summary>
			''' <param name="editor"> the editor </param>
			''' <param name="attr"> the attributes </param>
			''' <param name="replace">   if true, then replace the existing attributes first </param>
			Protected Friend Sub setCharacterAttributes(ByVal editor As javax.swing.JEditorPane, ByVal attr As AttributeSet, ByVal replace As Boolean)
				Dim p0 As Integer = editor.selectionStart
				Dim p1 As Integer = editor.selectionEnd
				If p0 <> p1 Then
					Dim doc As StyledDocument = getStyledDocument(editor)
					doc.characterAttributestes(p0, p1 - p0, attr, replace)
				End If
				Dim k As StyledEditorKit = getStyledEditorKit(editor)
				Dim inputAttributes As MutableAttributeSet = k.inputAttributes
				If replace Then inputAttributes.removeAttributes(inputAttributes)
				inputAttributes.addAttributes(attr)
			End Sub

			''' <summary>
			''' Applies the given attributes to paragraphs.  If
			''' there is a selection, the attributes are applied
			''' to the paragraphs that intersect the selection.
			''' if there is no selection, the attributes are applied
			''' to the paragraph at the current caret position.
			''' </summary>
			''' <param name="editor"> the editor </param>
			''' <param name="attr"> the attributes </param>
			''' <param name="replace">   if true, replace the existing attributes first </param>
			Protected Friend Sub setParagraphAttributes(ByVal editor As javax.swing.JEditorPane, ByVal attr As AttributeSet, ByVal replace As Boolean)
				Dim p0 As Integer = editor.selectionStart
				Dim p1 As Integer = editor.selectionEnd
				Dim doc As StyledDocument = getStyledDocument(editor)
				doc.paragraphAttributestes(p0, p1 - p0, attr, replace)
			End Sub

		End Class

		''' <summary>
		''' An action to set the font family in the associated
		''' JEditorPane.  This will use the family specified as
		''' the command string on the ActionEvent if there is one,
		''' otherwise the family that was initialized with will be used.
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
		Public Class FontFamilyAction
			Inherits StyledTextAction

			''' <summary>
			''' Creates a new FontFamilyAction.
			''' </summary>
			''' <param name="nm"> the action name </param>
			''' <param name="family"> the font family </param>
			Public Sub New(ByVal nm As String, ByVal family As String)
				MyBase.New(nm)
				Me.family = family
			End Sub

			''' <summary>
			''' Sets the font family.
			''' </summary>
			''' <param name="e"> the event </param>
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				Dim ___editor As javax.swing.JEditorPane = getEditor(e)
				If ___editor IsNot Nothing Then
					Dim family As String = Me.family
					If (e IsNot Nothing) AndAlso (e.source Is ___editor) Then
						Dim s As String = e.actionCommand
						If s IsNot Nothing Then family = s
					End If
					If family IsNot Nothing Then
						Dim attr As MutableAttributeSet = New SimpleAttributeSet
						StyleConstants.fontFamilyily(attr, family)
						characterAttributestes(___editor, attr, False)
					Else
						javax.swing.UIManager.lookAndFeel.provideErrorFeedback(___editor)
					End If
				End If
			End Sub

			Private family As String
		End Class

		''' <summary>
		''' An action to set the font size in the associated
		''' JEditorPane.  This will use the size specified as
		''' the command string on the ActionEvent if there is one,
		''' otherwise the size that was initialized with will be used.
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
		Public Class FontSizeAction
			Inherits StyledTextAction

			''' <summary>
			''' Creates a new FontSizeAction.
			''' </summary>
			''' <param name="nm"> the action name </param>
			''' <param name="size"> the font size </param>
			Public Sub New(ByVal nm As String, ByVal size As Integer)
				MyBase.New(nm)
				Me.size = size
			End Sub

			''' <summary>
			''' Sets the font size.
			''' </summary>
			''' <param name="e"> the action event </param>
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				Dim ___editor As javax.swing.JEditorPane = getEditor(e)
				If ___editor IsNot Nothing Then
					Dim size As Integer = Me.size
					If (e IsNot Nothing) AndAlso (e.source Is ___editor) Then
						Dim s As String = e.actionCommand
						Try
							size = Convert.ToInt32(s, 10)
						Catch nfe As NumberFormatException
						End Try
					End If
					If size <> 0 Then
						Dim attr As MutableAttributeSet = New SimpleAttributeSet
						StyleConstants.fontSizeize(attr, size)
						characterAttributestes(___editor, attr, False)
					Else
						javax.swing.UIManager.lookAndFeel.provideErrorFeedback(___editor)
					End If
				End If
			End Sub

			Private size As Integer
		End Class

		''' <summary>
		''' An action to set foreground color.  This sets the
		''' <code>StyleConstants.Foreground</code> attribute for the
		''' currently selected range of the target JEditorPane.
		''' This is done by calling
		''' <code>StyledDocument.setCharacterAttributes</code>
		''' on the styled document associated with the target
		''' JEditorPane.
		''' <p>
		''' If the target text component is specified as the
		''' source of the ActionEvent and there is a command string,
		''' the command string will be interpreted as the foreground
		''' color.  It will be interpreted by called
		''' <code>Color.decode</code>, and should therefore be
		''' legal input for that method.
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
		Public Class ForegroundAction
			Inherits StyledTextAction

			''' <summary>
			''' Creates a new ForegroundAction.
			''' </summary>
			''' <param name="nm"> the action name </param>
			''' <param name="fg"> the foreground color </param>
			Public Sub New(ByVal nm As String, ByVal fg As Color)
				MyBase.New(nm)
				Me.fg = fg
			End Sub

			''' <summary>
			''' Sets the foreground color.
			''' </summary>
			''' <param name="e"> the action event </param>
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				Dim ___editor As javax.swing.JEditorPane = getEditor(e)
				If ___editor IsNot Nothing Then
					Dim fg As Color = Me.fg
					If (e IsNot Nothing) AndAlso (e.source Is ___editor) Then
						Dim s As String = e.actionCommand
						Try
							fg = Color.decode(s)
						Catch nfe As NumberFormatException
						End Try
					End If
					If fg IsNot Nothing Then
						Dim attr As MutableAttributeSet = New SimpleAttributeSet
						StyleConstants.foregroundund(attr, fg)
						characterAttributestes(___editor, attr, False)
					Else
						javax.swing.UIManager.lookAndFeel.provideErrorFeedback(___editor)
					End If
				End If
			End Sub

			Private fg As Color
		End Class

		''' <summary>
		''' An action to set paragraph alignment.  This sets the
		''' <code>StyleConstants.Alignment</code> attribute for the
		''' currently selected range of the target JEditorPane.
		''' This is done by calling
		''' <code>StyledDocument.setParagraphAttributes</code>
		''' on the styled document associated with the target
		''' JEditorPane.
		''' <p>
		''' If the target text component is specified as the
		''' source of the ActionEvent and there is a command string,
		''' the command string will be interpreted as an integer
		''' that should be one of the legal values for the
		''' <code>StyleConstants.Alignment</code> attribute.
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
		Public Class AlignmentAction
			Inherits StyledTextAction

			''' <summary>
			''' Creates a new AlignmentAction.
			''' </summary>
			''' <param name="nm"> the action name </param>
			''' <param name="a"> the alignment &gt;= 0 </param>
			Public Sub New(ByVal nm As String, ByVal a As Integer)
				MyBase.New(nm)
				Me.a = a
			End Sub

			''' <summary>
			''' Sets the alignment.
			''' </summary>
			''' <param name="e"> the action event </param>
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				Dim ___editor As javax.swing.JEditorPane = getEditor(e)
				If ___editor IsNot Nothing Then
					Dim a As Integer = Me.a
					If (e IsNot Nothing) AndAlso (e.source Is ___editor) Then
						Dim s As String = e.actionCommand
						Try
							a = Convert.ToInt32(s, 10)
						Catch nfe As NumberFormatException
						End Try
					End If
					Dim attr As MutableAttributeSet = New SimpleAttributeSet
					StyleConstants.alignmentent(attr, a)
					paragraphAttributestes(___editor, attr, False)
				End If
			End Sub

			Private a As Integer
		End Class

		''' <summary>
		''' An action to toggle the bold attribute.
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
		Public Class BoldAction
			Inherits StyledTextAction

			''' <summary>
			''' Constructs a new BoldAction.
			''' </summary>
			Public Sub New()
				MyBase.New("font-bold")
			End Sub

			''' <summary>
			''' Toggles the bold attribute.
			''' </summary>
			''' <param name="e"> the action event </param>
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				Dim ___editor As javax.swing.JEditorPane = getEditor(e)
				If ___editor IsNot Nothing Then
					Dim kit As StyledEditorKit = getStyledEditorKit(___editor)
					Dim attr As MutableAttributeSet = kit.inputAttributes
					Dim bold As Boolean = If(StyleConstants.isBold(attr), False, True)
					Dim sas As New SimpleAttributeSet
					StyleConstants.boldold(sas, bold)
					characterAttributestes(___editor, sas, False)
				End If
			End Sub
		End Class

		''' <summary>
		''' An action to toggle the italic attribute.
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
		Public Class ItalicAction
			Inherits StyledTextAction

			''' <summary>
			''' Constructs a new ItalicAction.
			''' </summary>
			Public Sub New()
				MyBase.New("font-italic")
			End Sub

			''' <summary>
			''' Toggles the italic attribute.
			''' </summary>
			''' <param name="e"> the action event </param>
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				Dim ___editor As javax.swing.JEditorPane = getEditor(e)
				If ___editor IsNot Nothing Then
					Dim kit As StyledEditorKit = getStyledEditorKit(___editor)
					Dim attr As MutableAttributeSet = kit.inputAttributes
					Dim italic As Boolean = If(StyleConstants.isItalic(attr), False, True)
					Dim sas As New SimpleAttributeSet
					StyleConstants.italiclic(sas, italic)
					characterAttributestes(___editor, sas, False)
				End If
			End Sub
		End Class

		''' <summary>
		''' An action to toggle the underline attribute.
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
		Public Class UnderlineAction
			Inherits StyledTextAction

			''' <summary>
			''' Constructs a new UnderlineAction.
			''' </summary>
			Public Sub New()
				MyBase.New("font-underline")
			End Sub

			''' <summary>
			''' Toggles the Underline attribute.
			''' </summary>
			''' <param name="e"> the action event </param>
			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				Dim ___editor As javax.swing.JEditorPane = getEditor(e)
				If ___editor IsNot Nothing Then
					Dim kit As StyledEditorKit = getStyledEditorKit(___editor)
					Dim attr As MutableAttributeSet = kit.inputAttributes
					Dim underline As Boolean = If(StyleConstants.isUnderline(attr), False, True)
					Dim sas As New SimpleAttributeSet
					StyleConstants.underlineine(sas, underline)
					characterAttributestes(___editor, sas, False)
				End If
			End Sub
		End Class


		''' <summary>
		''' StyledInsertBreakAction has similar behavior to that of
		''' <code>DefaultEditorKit.InsertBreakAction</code>. That is when
		''' its <code>actionPerformed</code> method is invoked, a newline
		''' is inserted. Beyond that, this will reset the input attributes to
		''' what they were before the newline was inserted.
		''' </summary>
		Friend Class StyledInsertBreakAction
			Inherits StyledTextAction

			Private tempSet As SimpleAttributeSet

			Friend Sub New()
				MyBase.New(insertBreakAction)
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As java.awt.event.ActionEvent)
				Dim target As javax.swing.JEditorPane = getEditor(e)

				If target IsNot Nothing Then
					If ((Not target.editable)) OrElse ((Not target.enabled)) Then
						javax.swing.UIManager.lookAndFeel.provideErrorFeedback(target)
						Return
					End If
					Dim sek As StyledEditorKit = getStyledEditorKit(target)

					If tempSet IsNot Nothing Then
						tempSet.removeAttributes(tempSet)
					Else
						tempSet = New SimpleAttributeSet
					End If
					tempSet.addAttributes(sek.inputAttributes)
					target.replaceSelection(vbLf)

					Dim ia As MutableAttributeSet = sek.inputAttributes

					ia.removeAttributes(ia)
					ia.addAttributes(tempSet)
					tempSet.removeAttributes(tempSet)
				Else
					' See if we are in a JTextComponent.
					Dim text As JTextComponent = getTextComponent(e)

					If text IsNot Nothing Then
						If ((Not text.editable)) OrElse ((Not text.enabled)) Then
							javax.swing.UIManager.lookAndFeel.provideErrorFeedback(target)
							Return
						End If
						text.replaceSelection(vbLf)
					End If
				End If
			End Sub
		End Class
	End Class

End Namespace