Imports Microsoft.VisualBasic
Imports System
Imports javax.swing.text
Imports javax.swing.plaf
Imports javax.accessibility

'
' * Copyright (c) 1997, 2015, Oracle and/or its affiliates. All rights reserved.
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
	''' A <code>JTextArea</code> is a multi-line area that displays plain text.
	''' It is intended to be a lightweight component that provides source
	''' compatibility with the <code>java.awt.TextArea</code> class where it can
	''' reasonably do so.
	''' You can find information and examples of using all the text components in
	''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/components/text.html">Using Text Components</a>,
	''' a section in <em>The Java Tutorial.</em>
	''' 
	''' <p>
	''' This component has capabilities not found in the
	''' <code>java.awt.TextArea</code> class.  The superclass should be
	''' consulted for additional capabilities.
	''' Alternative multi-line text classes with
	''' more capabilities are <code>JTextPane</code> and <code>JEditorPane</code>.
	''' <p>
	''' The <code>java.awt.TextArea</code> internally handles scrolling.
	''' <code>JTextArea</code> is different in that it doesn't manage scrolling,
	''' but implements the swing <code>Scrollable</code> interface.  This allows it
	''' to be placed inside a <code>JScrollPane</code> if scrolling
	''' behavior is desired, and used directly if scrolling is not desired.
	''' <p>
	''' The <code>java.awt.TextArea</code> has the ability to do line wrapping.
	''' This was controlled by the horizontal scrolling policy.  Since
	''' scrolling is not done by <code>JTextArea</code> directly, backward
	''' compatibility must be provided another way.  <code>JTextArea</code> has
	''' a bound property for line wrapping that controls whether or
	''' not it will wrap lines.  By default, the line wrapping property
	''' is set to false (not wrapped).
	''' <p>
	''' <code>java.awt.TextArea</code> has two properties <code>rows</code>
	''' and <code>columns</code> that are used to determine the preferred size.
	''' <code>JTextArea</code> uses these properties to indicate the
	''' preferred size of the viewport when placed inside a <code>JScrollPane</code>
	''' to match the functionality provided by <code>java.awt.TextArea</code>.
	''' <code>JTextArea</code> has a preferred size of what is needed to
	''' display all of the text, so that it functions properly inside of
	''' a <code>JScrollPane</code>.  If the value for <code>rows</code>
	''' or <code>columns</code> is equal to zero,
	''' the preferred size along that axis is used for
	''' the viewport preferred size along the same axis.
	''' <p>
	''' The <code>java.awt.TextArea</code> could be monitored for changes by adding
	''' a <code>TextListener</code> for <code>TextEvent</code>s.
	''' In the <code>JTextComponent</code> based
	''' components, changes are broadcasted from the model via a
	''' <code>DocumentEvent</code> to <code>DocumentListeners</code>.
	''' The <code>DocumentEvent</code> gives
	''' the location of the change and the kind of change if desired.
	''' The code fragment might look something like:
	''' <pre>
	'''    DocumentListener myListener = ??;
	'''    JTextArea myArea = ??;
	'''    myArea.getDocument().addDocumentListener(myListener);
	''' </pre>
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
	'''   attribute: isContainer false
	''' description: A multi-line area that displays plain text.
	''' 
	''' @author  Timothy Prinzing </summary>
	''' <seealso cref= JTextPane </seealso>
	''' <seealso cref= JEditorPane </seealso>
	Public Class JTextArea
		Inherits JTextComponent

		''' <seealso cref= #getUIClassID </seealso>
		''' <seealso cref= #readObject </seealso>
		Private Const uiClassID As String = "TextAreaUI"

		''' <summary>
		''' Constructs a new TextArea.  A default model is set, the initial string
		''' is null, and rows/columns are set to 0.
		''' </summary>
		Public Sub New()
			Me.New(Nothing, Nothing, 0, 0)
		End Sub

		''' <summary>
		''' Constructs a new TextArea with the specified text displayed.
		''' A default model is created and rows/columns are set to 0.
		''' </summary>
		''' <param name="text"> the text to be displayed, or null </param>
		Public Sub New(ByVal text As String)
			Me.New(Nothing, text, 0, 0)
		End Sub

		''' <summary>
		''' Constructs a new empty TextArea with the specified number of
		''' rows and columns.  A default model is created, and the initial
		''' string is null.
		''' </summary>
		''' <param name="rows"> the number of rows &gt;= 0 </param>
		''' <param name="columns"> the number of columns &gt;= 0 </param>
		''' <exception cref="IllegalArgumentException"> if the rows or columns
		'''  arguments are negative. </exception>
		Public Sub New(ByVal rows As Integer, ByVal columns As Integer)
			Me.New(Nothing, Nothing, rows, columns)
		End Sub

		''' <summary>
		''' Constructs a new TextArea with the specified text and number
		''' of rows and columns.  A default model is created.
		''' </summary>
		''' <param name="text"> the text to be displayed, or null </param>
		''' <param name="rows"> the number of rows &gt;= 0 </param>
		''' <param name="columns"> the number of columns &gt;= 0 </param>
		''' <exception cref="IllegalArgumentException"> if the rows or columns
		'''  arguments are negative. </exception>
		Public Sub New(ByVal text As String, ByVal rows As Integer, ByVal columns As Integer)
			Me.New(Nothing, text, rows, columns)
		End Sub

		''' <summary>
		''' Constructs a new JTextArea with the given document model, and defaults
		''' for all of the other arguments (null, 0, 0).
		''' </summary>
		''' <param name="doc">  the model to use </param>
		Public Sub New(ByVal doc As Document)
			Me.New(doc, Nothing, 0, 0)
		End Sub

		''' <summary>
		''' Constructs a new JTextArea with the specified number of rows
		''' and columns, and the given model.  All of the constructors
		''' feed through this constructor.
		''' </summary>
		''' <param name="doc"> the model to use, or create a default one if null </param>
		''' <param name="text"> the text to be displayed, null if none </param>
		''' <param name="rows"> the number of rows &gt;= 0 </param>
		''' <param name="columns"> the number of columns &gt;= 0 </param>
		''' <exception cref="IllegalArgumentException"> if the rows or columns
		'''  arguments are negative. </exception>
		Public Sub New(ByVal doc As Document, ByVal text As String, ByVal rows As Integer, ByVal columns As Integer)
			MyBase.New()
			Me.rows = rows
			Me.columns = columns
			If doc Is Nothing Then doc = createDefaultModel()
			document = doc
			If text IsNot Nothing Then
				text = text
				[select](0, 0)
			End If
			If rows < 0 Then Throw New System.ArgumentException("rows: " & rows)
			If columns < 0 Then Throw New System.ArgumentException("columns: " & columns)
			LookAndFeel.installProperty(Me, "focusTraversalKeysForward", JComponent.managingFocusForwardTraversalKeys)
			LookAndFeel.installProperty(Me, "focusTraversalKeysBackward", JComponent.managingFocusBackwardTraversalKeys)
		End Sub

		''' <summary>
		''' Returns the class ID for the UI.
		''' </summary>
		''' <returns> the ID ("TextAreaUI") </returns>
		''' <seealso cref= JComponent#getUIClassID </seealso>
		''' <seealso cref= UIDefaults#getUI </seealso>
		Public Property Overrides uIClassID As String
			Get
				Return uiClassID
			End Get
		End Property

		''' <summary>
		''' Creates the default implementation of the model
		''' to be used at construction if one isn't explicitly
		''' given.  A new instance of PlainDocument is returned.
		''' </summary>
		''' <returns> the default document model </returns>
		Protected Friend Overridable Function createDefaultModel() As Document
			Return New PlainDocument
		End Function

		''' <summary>
		''' Sets the number of characters to expand tabs to.
		''' This will be multiplied by the maximum advance for
		''' variable width fonts.  A PropertyChange event ("tabSize") is fired
		''' when the tab size changes.
		''' </summary>
		''' <param name="size"> number of characters to expand to </param>
		''' <seealso cref= #getTabSize
		''' @beaninfo
		'''   preferred: true
		'''       bound: true
		''' description: the number of characters to expand tabs to </seealso>
		Public Overridable Property tabSize As Integer
			Set(ByVal size As Integer)
				Dim doc As Document = document
				If doc IsNot Nothing Then
					Dim old As Integer = tabSize
					doc.putProperty(PlainDocument.tabSizeAttribute, Convert.ToInt32(size))
					firePropertyChange("tabSize", old, size)
				End If
			End Set
			Get
				Dim ___size As Integer = 8
				Dim doc As Document = document
				If doc IsNot Nothing Then
					Dim i As Integer? = CInt(Fix(doc.getProperty(PlainDocument.tabSizeAttribute)))
					If i IsNot Nothing Then ___size = i
				End If
				Return ___size
			End Get
		End Property


		''' <summary>
		''' Sets the line-wrapping policy of the text area.  If set
		''' to true the lines will be wrapped if they are too long
		''' to fit within the allocated width.  If set to false,
		''' the lines will always be unwrapped.  A <code>PropertyChange</code>
		''' event ("lineWrap") is fired when the policy is changed.
		''' By default this property is false.
		''' </summary>
		''' <param name="wrap"> indicates if lines should be wrapped </param>
		''' <seealso cref= #getLineWrap
		''' @beaninfo
		'''   preferred: true
		'''       bound: true
		''' description: should lines be wrapped </seealso>
		Public Overridable Property lineWrap As Boolean
			Set(ByVal wrap As Boolean)
				Dim old As Boolean = Me.wrap
				Me.wrap = wrap
				firePropertyChange("lineWrap", old, wrap)
			End Set
			Get
				Return wrap
			End Get
		End Property


		''' <summary>
		''' Sets the style of wrapping used if the text area is wrapping
		''' lines.  If set to true the lines will be wrapped at word
		''' boundaries (whitespace) if they are too long
		''' to fit within the allocated width.  If set to false,
		''' the lines will be wrapped at character boundaries.
		''' By default this property is false.
		''' </summary>
		''' <param name="word"> indicates if word boundaries should be used
		'''   for line wrapping </param>
		''' <seealso cref= #getWrapStyleWord
		''' @beaninfo
		'''   preferred: false
		'''       bound: true
		''' description: should wrapping occur at word boundaries </seealso>
		Public Overridable Property wrapStyleWord As Boolean
			Set(ByVal word As Boolean)
				Dim old As Boolean = Me.word
				Me.word = word
				firePropertyChange("wrapStyleWord", old, word)
			End Set
			Get
				Return word
			End Get
		End Property


		''' <summary>
		''' Translates an offset into the components text to a
		''' line number.
		''' </summary>
		''' <param name="offset"> the offset &gt;= 0 </param>
		''' <returns> the line number &gt;= 0 </returns>
		''' <exception cref="BadLocationException"> thrown if the offset is
		'''   less than zero or greater than the document length. </exception>
		Public Overridable Function getLineOfOffset(ByVal offset As Integer) As Integer
			Dim doc As Document = document
			If offset < 0 Then
				Throw New BadLocationException("Can't translate offset to line", -1)
			ElseIf offset > doc.length Then
				Throw New BadLocationException("Can't translate offset to line", doc.length+1)
			Else
				Dim map As Element = document.defaultRootElement
				Return map.getElementIndex(offset)
			End If
		End Function

		''' <summary>
		''' Determines the number of lines contained in the area.
		''' </summary>
		''' <returns> the number of lines &gt; 0 </returns>
		Public Overridable Property lineCount As Integer
			Get
				Dim map As Element = document.defaultRootElement
				Return map.elementCount
			End Get
		End Property

		''' <summary>
		''' Determines the offset of the start of the given line.
		''' </summary>
		''' <param name="line">  the line number to translate &gt;= 0 </param>
		''' <returns> the offset &gt;= 0 </returns>
		''' <exception cref="BadLocationException"> thrown if the line is
		''' less than zero or greater or equal to the number of
		''' lines contained in the document (as reported by
		''' getLineCount). </exception>
		Public Overridable Function getLineStartOffset(ByVal line As Integer) As Integer
			Dim ___lineCount As Integer = lineCount
			If line < 0 Then
				Throw New BadLocationException("Negative line", -1)
			ElseIf line >= ___lineCount Then
				Throw New BadLocationException("No such line", document.length+1)
			Else
				Dim map As Element = document.defaultRootElement
				Dim lineElem As Element = map.getElement(line)
				Return lineElem.startOffset
			End If
		End Function

		''' <summary>
		''' Determines the offset of the end of the given line.
		''' </summary>
		''' <param name="line">  the line &gt;= 0 </param>
		''' <returns> the offset &gt;= 0 </returns>
		''' <exception cref="BadLocationException"> Thrown if the line is
		''' less than zero or greater or equal to the number of
		''' lines contained in the document (as reported by
		''' getLineCount). </exception>
		Public Overridable Function getLineEndOffset(ByVal line As Integer) As Integer
			Dim ___lineCount As Integer = lineCount
			If line < 0 Then
				Throw New BadLocationException("Negative line", -1)
			ElseIf line >= ___lineCount Then
				Throw New BadLocationException("No such line", document.length+1)
			Else
				Dim map As Element = document.defaultRootElement
				Dim lineElem As Element = map.getElement(line)
				Dim endOffset As Integer = lineElem.endOffset
				' hide the implicit break at the end of the document
				Return (If(line = ___lineCount - 1, (endOffset - 1), endOffset))
			End If
		End Function

		' --- java.awt.TextArea methods ---------------------------------

		''' <summary>
		''' Inserts the specified text at the specified position.  Does nothing
		''' if the model is null or if the text is null or empty.
		''' </summary>
		''' <param name="str"> the text to insert </param>
		''' <param name="pos"> the position at which to insert &gt;= 0 </param>
		''' <exception cref="IllegalArgumentException">  if pos is an
		'''  invalid position in the model </exception>
		''' <seealso cref= TextComponent#setText </seealso>
		''' <seealso cref= #replaceRange </seealso>
		Public Overridable Sub insert(ByVal str As String, ByVal pos As Integer)
			Dim doc As Document = document
			If doc IsNot Nothing Then
				Try
					doc.insertString(pos, str, Nothing)
				Catch e As BadLocationException
					Throw New System.ArgumentException(e.Message)
				End Try
			End If
		End Sub

		''' <summary>
		''' Appends the given text to the end of the document.  Does nothing if
		''' the model is null or the string is null or empty.
		''' </summary>
		''' <param name="str"> the text to insert </param>
		''' <seealso cref= #insert </seealso>
		Public Overridable Sub append(ByVal str As String)
			Dim doc As Document = document
			If doc IsNot Nothing Then
				Try
					doc.insertString(doc.length, str, Nothing)
				Catch e As BadLocationException
				End Try
			End If
		End Sub

		''' <summary>
		''' Replaces text from the indicated start to end position with the
		''' new text specified.  Does nothing if the model is null.  Simply
		''' does a delete if the new string is null or empty.
		''' </summary>
		''' <param name="str"> the text to use as the replacement </param>
		''' <param name="start"> the start position &gt;= 0 </param>
		''' <param name="end"> the end position &gt;= start </param>
		''' <exception cref="IllegalArgumentException">  if part of the range is an
		'''  invalid position in the model </exception>
		''' <seealso cref= #insert </seealso>
		Public Overridable Sub replaceRange(ByVal str As String, ByVal start As Integer, ByVal [end] As Integer)
			If [end] < start Then Throw New System.ArgumentException("end before start")
			Dim doc As Document = document
			If doc IsNot Nothing Then
				Try
					If TypeOf doc Is AbstractDocument Then
						CType(doc, AbstractDocument).replace(start, [end] - start, str, Nothing)
					Else
						doc.remove(start, [end] - start)
						doc.insertString(start, str, Nothing)
					End If
				Catch e As BadLocationException
					Throw New System.ArgumentException(e.Message)
				End Try
			End If
		End Sub

		''' <summary>
		''' Returns the number of rows in the TextArea.
		''' </summary>
		''' <returns> the number of rows &gt;= 0 </returns>
		Public Overridable Property rows As Integer
			Get
				Return rows
			End Get
			Set(ByVal rows As Integer)
				Dim oldVal As Integer = Me.rows
				If rows < 0 Then Throw New System.ArgumentException("rows less than zero.")
				If rows <> oldVal Then
					Me.rows = rows
					invalidate()
				End If
			End Set
		End Property


		''' <summary>
		''' Defines the meaning of the height of a row.  This defaults to
		''' the height of the font.
		''' </summary>
		''' <returns> the height &gt;= 1 </returns>
		Protected Friend Overridable Property rowHeight As Integer
			Get
				If rowHeight = 0 Then
					Dim metrics As FontMetrics = getFontMetrics(font)
					rowHeight = metrics.height
				End If
				Return rowHeight
			End Get
		End Property

		''' <summary>
		''' Returns the number of columns in the TextArea.
		''' </summary>
		''' <returns> number of columns &gt;= 0 </returns>
		Public Overridable Property columns As Integer
			Get
				Return columns
			End Get
			Set(ByVal columns As Integer)
				Dim oldVal As Integer = Me.columns
				If columns < 0 Then Throw New System.ArgumentException("columns less than zero.")
				If columns <> oldVal Then
					Me.columns = columns
					invalidate()
				End If
			End Set
		End Property


		''' <summary>
		''' Gets column width.
		''' The meaning of what a column is can be considered a fairly weak
		''' notion for some fonts.  This method is used to define the width
		''' of a column.  By default this is defined to be the width of the
		''' character <em>m</em> for the font used.  This method can be
		''' redefined to be some alternative amount.
		''' </summary>
		''' <returns> the column width &gt;= 1 </returns>
		Protected Friend Overridable Property columnWidth As Integer
			Get
				If columnWidth = 0 Then
					Dim metrics As FontMetrics = getFontMetrics(font)
					columnWidth = metrics.charWidth("m"c)
				End If
				Return columnWidth
			End Get
		End Property

		' --- Component methods -----------------------------------------

		''' <summary>
		''' Returns the preferred size of the TextArea.  This is the
		''' maximum of the size needed to display the text and the
		''' size requested for the viewport.
		''' </summary>
		''' <returns> the size </returns>
		Public Property Overrides preferredSize As Dimension
			Get
				Dim d As Dimension = MyBase.preferredSize
				d = If(d Is Nothing, New Dimension(400,400), d)
				Dim ___insets As Insets = insets
    
				If columns <> 0 Then d.width = Math.Max(d.width, columns * columnWidth + ___insets.left + ___insets.right)
				If rows <> 0 Then d.height = Math.Max(d.height, rows * rowHeight + ___insets.top + ___insets.bottom)
				Return d
			End Get
		End Property

		''' <summary>
		''' Sets the current font.  This removes cached row height and column
		''' width so the new font will be reflected, and calls revalidate().
		''' </summary>
		''' <param name="f"> the font to use as the current font </param>
		Public Overrides Property font As Font
			Set(ByVal f As Font)
				MyBase.font = f
				rowHeight = 0
				columnWidth = 0
			End Set
		End Property


		''' <summary>
		''' Returns a string representation of this JTextArea. This method
		''' is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this JTextArea. </returns>
		Protected Friend Overrides Function paramString() As String
			Dim wrapString As String = (If(wrap, "true", "false"))
			Dim wordString As String = (If(word, "true", "false"))

			Return MyBase.paramString() & ",colums=" & columns & ",columWidth=" & columnWidth & ",rows=" & rows & ",rowHeight=" & rowHeight & ",word=" & wordString & ",wrap=" & wrapString
		End Function

		' --- Scrollable methods ----------------------------------------

		''' <summary>
		''' Returns true if a viewport should always force the width of this
		''' Scrollable to match the width of the viewport.  This is implemented
		''' to return true if the line wrapping policy is true, and false
		''' if lines are not being wrapped.
		''' </summary>
		''' <returns> true if a viewport should force the Scrollables width
		''' to match its own. </returns>
		Public Property Overrides scrollableTracksViewportWidth As Boolean
			Get
				Return If(wrap, True, MyBase.scrollableTracksViewportWidth)
			End Get
		End Property

		''' <summary>
		''' Returns the preferred size of the viewport if this component
		''' is embedded in a JScrollPane.  This uses the desired column
		''' and row settings if they have been set, otherwise the superclass
		''' behavior is used.
		''' </summary>
		''' <returns> The preferredSize of a JViewport whose view is this Scrollable. </returns>
		''' <seealso cref= JViewport#getPreferredSize </seealso>
		Public Property Overrides preferredScrollableViewportSize As Dimension
			Get
				Dim ___size As Dimension = MyBase.preferredScrollableViewportSize
				___size = If(___size Is Nothing, New Dimension(400,400), ___size)
				Dim ___insets As Insets = insets
    
				___size.width = If(columns = 0, ___size.width, columns * columnWidth + ___insets.left + ___insets.right)
				___size.height = If(rows = 0, ___size.height, rows * rowHeight + ___insets.top + ___insets.bottom)
				Return ___size
			End Get
		End Property

		''' <summary>
		''' Components that display logical rows or columns should compute
		''' the scroll increment that will completely expose one new row
		''' or column, depending on the value of orientation.  This is implemented
		''' to use the values returned by the <code>getRowHeight</code> and
		''' <code>getColumnWidth</code> methods.
		''' <p>
		''' Scrolling containers, like JScrollPane, will use this method
		''' each time the user requests a unit scroll.
		''' </summary>
		''' <param name="visibleRect"> the view area visible within the viewport </param>
		''' <param name="orientation"> Either SwingConstants.VERTICAL or
		'''   SwingConstants.HORIZONTAL. </param>
		''' <param name="direction"> Less than zero to scroll up/left,
		'''   greater than zero for down/right. </param>
		''' <returns> The "unit" increment for scrolling in the specified direction </returns>
		''' <exception cref="IllegalArgumentException"> for an invalid orientation </exception>
		''' <seealso cref= JScrollBar#setUnitIncrement </seealso>
		''' <seealso cref= #getRowHeight </seealso>
		''' <seealso cref= #getColumnWidth </seealso>
		Public Overrides Function getScrollableUnitIncrement(ByVal visibleRect As Rectangle, ByVal orientation As Integer, ByVal direction As Integer) As Integer
			Select Case orientation
			Case SwingConstants.VERTICAL
				Return rowHeight
			Case SwingConstants.HORIZONTAL
				Return columnWidth
			Case Else
				Throw New System.ArgumentException("Invalid orientation: " & orientation)
			End Select
		End Function

		''' <summary>
		''' See readObject() and writeObject() in JComponent for more
		''' information about serialization in Swing.
		''' </summary>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			s.defaultWriteObject()
			If uIClassID.Equals(uiClassID) Then
				Dim count As SByte = JComponent.getWriteObjCounter(Me)
				count -= 1
				JComponent.writeObjCounterter(Me, count)
				If count = 0 AndAlso ui IsNot Nothing Then ui.installUI(Me)
			End If
		End Sub

	'///////////////
	' Accessibility support
	'//////////////


		''' <summary>
		''' Gets the AccessibleContext associated with this JTextArea.
		''' For JTextAreas, the AccessibleContext takes the form of an
		''' AccessibleJTextArea.
		''' A new AccessibleJTextArea instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleJTextArea that serves as the
		'''         AccessibleContext of this JTextArea </returns>
		Public Property Overrides accessibleContext As AccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleJTextArea(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>JTextArea</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to text area user-interface
		''' elements.
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
		Protected Friend Class AccessibleJTextArea
			Inherits AccessibleJTextComponent

			Private ReadOnly outerInstance As JTextArea

			Public Sub New(ByVal outerInstance As JTextArea)
				Me.outerInstance = outerInstance
			End Sub


			''' <summary>
			''' Gets the state set of this object.
			''' </summary>
			''' <returns> an instance of AccessibleStateSet describing the states
			''' of the object </returns>
			''' <seealso cref= AccessibleStateSet </seealso>
			Public Overridable Property accessibleStateSet As AccessibleStateSet
				Get
					Dim states As AccessibleStateSet = MyBase.accessibleStateSet
					states.add(AccessibleState.MULTI_LINE)
					Return states
				End Get
			End Property
		End Class

		' --- variables -------------------------------------------------

		Private rows As Integer
		Private columns As Integer
		Private columnWidth As Integer
		Private rowHeight As Integer
		Private wrap As Boolean
		Private word As Boolean

	End Class

End Namespace