Imports System
Imports javax.swing.text
Imports javax.swing.event
Imports javax.swing.plaf

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
Namespace javax.swing



	''' <summary>
	''' A text component that can be marked up with attributes that are
	''' represented graphically.
	''' You can find how-to information and examples of using text panes in
	''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/components/text.html">Using Text Components</a>,
	''' a section in <em>The Java Tutorial.</em>
	''' 
	''' <p>
	''' This component models paragraphs
	''' that are composed of runs of character level attributes.  Each
	''' paragraph may have a logical style attached to it which contains
	''' the default attributes to use if not overridden by attributes set
	''' on the paragraph or character run.  Components and images may
	''' be embedded in the flow of text.
	''' 
	''' <dl>
	''' <dt><b><font size=+1>Newlines</font></b>
	''' <dd>
	''' For a discussion on how newlines are handled, see
	''' <a href="text/DefaultEditorKit.html">DefaultEditorKit</a>.
	''' </dl>
	''' 
	''' <p>
	''' <strong>Warning:</strong> Swing is not thread safe. For more
	''' information see <a
	''' href="package-summary.html#threading">Swing's Threading
	''' Policy</a>.
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
	''' @beaninfo
	'''   attribute: isContainer true
	''' description: A text component that can be marked up with attributes that are graphically represented.
	''' 
	''' @author  Timothy Prinzing </summary>
	''' <seealso cref= javax.swing.text.StyledEditorKit </seealso>
	Public Class JTextPane
		Inherits JEditorPane

		''' <summary>
		''' Creates a new <code>JTextPane</code>.  A new instance of
		''' <code>StyledEditorKit</code> is
		''' created and set, and the document model set to <code>null</code>.
		''' </summary>
		Public Sub New()
			MyBase.New()
			Dim ___editorKit As EditorKit = createDefaultEditorKit()
			Dim ___contentType As String = ___editorKit.contentType
			If ___contentType IsNot Nothing AndAlso getEditorKitClassNameForContentType(___contentType) = defaultEditorKitMap.get(___contentType) Then editorKitForContentTypeype(___contentType, ___editorKit)
			editorKit = ___editorKit
		End Sub

		''' <summary>
		''' Creates a new <code>JTextPane</code>, with a specified document model.
		''' A new instance of <code>javax.swing.text.StyledEditorKit</code>
		'''  is created and set.
		''' </summary>
		''' <param name="doc"> the document model </param>
		Public Sub New(ByVal doc As StyledDocument)
			Me.New()
			styledDocument = doc
		End Sub

		''' <summary>
		''' Returns the class ID for the UI.
		''' </summary>
		''' <returns> the string "TextPaneUI"
		''' </returns>
		''' <seealso cref= JComponent#getUIClassID </seealso>
		''' <seealso cref= UIDefaults#getUI </seealso>
		Public Property Overrides uIClassID As String
			Get
				Return uiClassID
			End Get
		End Property

		''' <summary>
		''' Associates the editor with a text document.  This
		''' must be a <code>StyledDocument</code>.
		''' </summary>
		''' <param name="doc">  the document to display/edit </param>
		''' <exception cref="IllegalArgumentException">  if <code>doc</code> can't
		'''   be narrowed to a <code>StyledDocument</code> which is the
		'''   required type of model for this text component </exception>
		Public Overrides Property document As Document
			Set(ByVal doc As Document)
				If TypeOf doc Is StyledDocument Then
					MyBase.document = doc
				Else
					Throw New System.ArgumentException("Model must be StyledDocument")
				End If
			End Set
		End Property

		''' <summary>
		''' Associates the editor with a text document.
		''' The currently registered factory is used to build a view for
		''' the document, which gets displayed by the editor.
		''' </summary>
		''' <param name="doc">  the document to display/edit </param>
		Public Overridable Property styledDocument As StyledDocument
			Set(ByVal doc As StyledDocument)
				MyBase.document = doc
			End Set
			Get
				Return CType(document, StyledDocument)
			End Get
		End Property


		''' <summary>
		''' Replaces the currently selected content with new content
		''' represented by the given string.  If there is no selection
		''' this amounts to an insert of the given text.  If there
		''' is no replacement text this amounts to a removal of the
		''' current selection.  The replacement text will have the
		''' attributes currently defined for input at the point of
		''' insertion.  If the document is not editable, beep and return.
		''' </summary>
		''' <param name="content">  the content to replace the selection with </param>
		Public Overrides Sub replaceSelection(ByVal content As String)
			replaceSelection(content, True)
		End Sub

		Private Sub replaceSelection(ByVal content As String, ByVal checkEditable As Boolean)
			If checkEditable AndAlso (Not editable) Then
				UIManager.lookAndFeel.provideErrorFeedback(JTextPane.this)
				Return
			End If
			Dim doc As Document = styledDocument
			If doc IsNot Nothing Then
				Try
					Dim ___caret As Caret = caret
					Dim composedTextSaved As Boolean = saveComposedText(___caret.dot)
					Dim p0 As Integer = Math.Min(___caret.dot, ___caret.mark)
					Dim p1 As Integer = Math.Max(___caret.dot, ___caret.mark)
					Dim attr As AttributeSet = inputAttributes.copyAttributes()
					If TypeOf doc Is AbstractDocument Then
						CType(doc, AbstractDocument).replace(p0, p1 - p0, content,attr)
					Else
						If p0 <> p1 Then doc.remove(p0, p1 - p0)
						If content IsNot Nothing AndAlso content.Length > 0 Then doc.insertString(p0, content, attr)
					End If
					If composedTextSaved Then restoreComposedText()
				Catch e As BadLocationException
					UIManager.lookAndFeel.provideErrorFeedback(JTextPane.this)
				End Try
			End If
		End Sub

		''' <summary>
		''' Inserts a component into the document as a replacement
		''' for the currently selected content.  If there is no
		''' selection the component is effectively inserted at the
		''' current position of the caret.  This is represented in
		''' the associated document as an attribute of one character
		''' of content.
		''' <p>
		''' The component given is the actual component used by the
		''' JTextPane.  Since components cannot be a child of more than
		''' one container, this method should not be used in situations
		''' where the model is shared by text components.
		''' <p>
		''' The component is placed relative to the text baseline
		''' according to the value returned by
		''' <code>Component.getAlignmentY</code>.  For Swing components
		''' this value can be conveniently set using the method
		''' <code>JComponent.setAlignmentY</code>.  For example, setting
		''' a value of <code>0.75</code> will cause 75 percent of the
		''' component to be above the baseline, and 25 percent of the
		''' component to be below the baseline.
		''' </summary>
		''' <param name="c">    the component to insert </param>
		Public Overridable Sub insertComponent(ByVal c As Component)
			Dim ___inputAttributes As MutableAttributeSet = inputAttributes
			___inputAttributes.removeAttributes(___inputAttributes)
			StyleConstants.componentent(___inputAttributes, c)
			replaceSelection(" ", False)
			___inputAttributes.removeAttributes(___inputAttributes)
		End Sub

		''' <summary>
		''' Inserts an icon into the document as a replacement
		''' for the currently selected content.  If there is no
		''' selection the icon is effectively inserted at the
		''' current position of the caret.  This is represented in
		''' the associated document as an attribute of one character
		''' of content.
		''' </summary>
		''' <param name="g">    the icon to insert </param>
		''' <seealso cref= Icon </seealso>
		Public Overridable Sub insertIcon(ByVal g As Icon)
			Dim ___inputAttributes As MutableAttributeSet = inputAttributes
			___inputAttributes.removeAttributes(___inputAttributes)
			StyleConstants.iconcon(___inputAttributes, g)
			replaceSelection(" ", False)
			___inputAttributes.removeAttributes(___inputAttributes)
		End Sub

		''' <summary>
		''' Adds a new style into the logical style hierarchy.  Style attributes
		''' resolve from bottom up so an attribute specified in a child
		''' will override an attribute specified in the parent.
		''' </summary>
		''' <param name="nm">   the name of the style (must be unique within the
		'''   collection of named styles).  The name may be <code>null</code>
		'''   if the style is unnamed, but the caller is responsible
		'''   for managing the reference returned as an unnamed style can't
		'''   be fetched by name.  An unnamed style may be useful for things
		'''   like character attribute overrides such as found in a style
		'''   run. </param>
		''' <param name="parent"> the parent style.  This may be <code>null</code>
		'''   if unspecified
		'''   attributes need not be resolved in some other style. </param>
		''' <returns> the new <code>Style</code> </returns>
		Public Overridable Function addStyle(ByVal nm As String, ByVal parent As Style) As Style
			Dim doc As StyledDocument = styledDocument
			Return doc.addStyle(nm, parent)
		End Function

		''' <summary>
		''' Removes a named non-<code>null</code> style previously added to
		''' the document.
		''' </summary>
		''' <param name="nm">  the name of the style to remove </param>
		Public Overridable Sub removeStyle(ByVal nm As String)
			Dim doc As StyledDocument = styledDocument
			doc.removeStyle(nm)
		End Sub

		''' <summary>
		''' Fetches a named non-<code>null</code> style previously added.
		''' </summary>
		''' <param name="nm">  the name of the style </param>
		''' <returns> the <code>Style</code> </returns>
		Public Overridable Function getStyle(ByVal nm As String) As Style
			Dim doc As StyledDocument = styledDocument
			Return doc.getStyle(nm)
		End Function

		''' <summary>
		''' Sets the logical style to use for the paragraph at the
		''' current caret position.  If attributes aren't explicitly set
		''' for character and paragraph attributes they will resolve
		''' through the logical style assigned to the paragraph, which
		''' in term may resolve through some hierarchy completely
		''' independent of the element hierarchy in the document.
		''' </summary>
		''' <param name="s">  the logical style to assign to the paragraph,
		'''          or <code>null</code> for no style </param>
		Public Overridable Property logicalStyle As Style
			Set(ByVal s As Style)
				Dim doc As StyledDocument = styledDocument
				doc.logicalStyleyle(caretPosition, s)
			End Set
			Get
				Dim doc As StyledDocument = styledDocument
				Return doc.getLogicalStyle(caretPosition)
			End Get
		End Property


		''' <summary>
		''' Fetches the character attributes in effect at the
		''' current location of the caret, or <code>null</code>.
		''' </summary>
		''' <returns> the attributes, or <code>null</code> </returns>
		Public Overridable Property characterAttributes As AttributeSet
			Get
				Dim doc As StyledDocument = styledDocument
				Dim run As Element = doc.getCharacterElement(caretPosition)
				If run IsNot Nothing Then Return run.attributes
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Applies the given attributes to character
		''' content.  If there is a selection, the attributes
		''' are applied to the selection range.  If there
		''' is no selection, the attributes are applied to
		''' the input attribute set which defines the attributes
		''' for any new text that gets inserted.
		''' </summary>
		''' <param name="attr"> the attributes </param>
		''' <param name="replace"> if true, then replace the existing attributes first </param>
		Public Overridable Sub setCharacterAttributes(ByVal attr As AttributeSet, ByVal replace As Boolean)
			Dim p0 As Integer = selectionStart
			Dim p1 As Integer = selectionEnd
			If p0 <> p1 Then
				Dim doc As StyledDocument = styledDocument
				doc.characterAttributestes(p0, p1 - p0, attr, replace)
			Else
				Dim ___inputAttributes As MutableAttributeSet = inputAttributes
				If replace Then ___inputAttributes.removeAttributes(___inputAttributes)
				___inputAttributes.addAttributes(attr)
			End If
		End Sub

		''' <summary>
		''' Fetches the current paragraph attributes in effect
		''' at the location of the caret, or <code>null</code> if none.
		''' </summary>
		''' <returns> the attributes </returns>
		Public Overridable Property paragraphAttributes As AttributeSet
			Get
				Dim doc As StyledDocument = styledDocument
				Dim paragraph As Element = doc.getParagraphElement(caretPosition)
				If paragraph IsNot Nothing Then Return paragraph.attributes
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Applies the given attributes to paragraphs.  If
		''' there is a selection, the attributes are applied
		''' to the paragraphs that intersect the selection.
		''' If there is no selection, the attributes are applied
		''' to the paragraph at the current caret position.
		''' </summary>
		''' <param name="attr"> the non-<code>null</code> attributes </param>
		''' <param name="replace"> if true, replace the existing attributes first </param>
		Public Overridable Sub setParagraphAttributes(ByVal attr As AttributeSet, ByVal replace As Boolean)
			Dim p0 As Integer = selectionStart
			Dim p1 As Integer = selectionEnd
			Dim doc As StyledDocument = styledDocument
			doc.paragraphAttributestes(p0, p1 - p0, attr, replace)
		End Sub

		''' <summary>
		''' Gets the input attributes for the pane.
		''' </summary>
		''' <returns> the attributes </returns>
		Public Overridable Property inputAttributes As MutableAttributeSet
			Get
				Return styledEditorKit.inputAttributes
			End Get
		End Property

		''' <summary>
		''' Gets the editor kit.
		''' </summary>
		''' <returns> the editor kit </returns>
		Protected Friend Property styledEditorKit As StyledEditorKit
			Get
				Return CType(editorKit, StyledEditorKit)
			End Get
		End Property

		''' <seealso cref= #getUIClassID </seealso>
		''' <seealso cref= #readObject </seealso>
		Private Const uiClassID As String = "TextPaneUI"


		''' <summary>
		''' See <code>readObject</code> and <code>writeObject</code> in
		''' <code>JComponent</code> for more
		''' information about serialization in Swing.
		''' </summary>
		''' <param name="s"> the output stream </param>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			s.defaultWriteObject()
			If uIClassID.Equals(uiClassID) Then
				Dim count As SByte = JComponent.getWriteObjCounter(Me)
				count -= 1
				JComponent.writeObjCounterter(Me, count)
				If count = 0 AndAlso ui IsNot Nothing Then ui.installUI(Me)
			End If
		End Sub


		' --- JEditorPane ------------------------------------

		''' <summary>
		''' Creates the <code>EditorKit</code> to use by default.  This
		''' is implemented to return <code>javax.swing.text.StyledEditorKit</code>.
		''' </summary>
		''' <returns> the editor kit </returns>
		Protected Friend Overrides Function createDefaultEditorKit() As EditorKit
			Return New StyledEditorKit
		End Function

		''' <summary>
		''' Sets the currently installed kit for handling
		''' content.  This is the bound property that
		''' establishes the content type of the editor.
		''' </summary>
		''' <param name="kit"> the desired editor behavior </param>
		''' <exception cref="IllegalArgumentException"> if kit is not a
		'''          <code>StyledEditorKit</code> </exception>
		Public NotOverridable Overrides Property editorKit As EditorKit
			Set(ByVal kit As EditorKit)
				If TypeOf kit Is StyledEditorKit Then
					MyBase.editorKit = kit
				Else
					Throw New System.ArgumentException("Must be StyledEditorKit")
				End If
			End Set
		End Property

		''' <summary>
		''' Returns a string representation of this <code>JTextPane</code>.
		''' This method
		''' is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this <code>JTextPane</code> </returns>
		Protected Friend Overrides Function paramString() As String
			Return MyBase.paramString()
		End Function

	End Class

End Namespace