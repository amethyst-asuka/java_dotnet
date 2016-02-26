Imports System
Imports System.Collections
Imports System.Text
Imports javax.swing
Imports javax.swing.event
Imports javax.swing.text

'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.text.html

	''' <summary>
	''' Component decorator that implements the view interface
	''' for form elements, &lt;input&gt;, &lt;textarea&gt;,
	''' and &lt;select&gt;.  The model for the component is stored
	''' as an attribute of the the element (using StyleConstants.ModelAttribute),
	''' and is used to build the component of the view.  The type
	''' of the model is assumed to of the type that would be set by
	''' <code>HTMLDocument.HTMLReader.FormAction</code>.  If there are
	''' multiple views mapped over the document, they will share the
	''' embedded component models.
	''' <p>
	''' The following table shows what components get built
	''' by this view.
	''' <table summary="shows what components get built by this view">
	''' <tr>
	'''   <th>Element Type</th>
	'''   <th>Component built</th>
	''' </tr>
	''' <tr>
	'''   <td>input, type button</td>
	'''   <td>JButton</td>
	''' </tr>
	''' <tr>
	'''   <td>input, type checkbox</td>
	'''   <td>JCheckBox</td>
	''' </tr>
	''' <tr>
	'''   <td>input, type image</td>
	'''   <td>JButton</td>
	''' </tr>
	''' <tr>
	'''   <td>input, type password</td>
	'''   <td>JPasswordField</td>
	''' </tr>
	''' <tr>
	'''   <td>input, type radio</td>
	'''   <td>JRadioButton</td>
	''' </tr>
	''' <tr>
	'''   <td>input, type reset</td>
	'''   <td>JButton</td>
	''' </tr>
	''' <tr>
	'''   <td>input, type submit</td>
	'''   <td>JButton</td>
	''' </tr>
	''' <tr>
	'''   <td>input, type text</td>
	'''   <td>JTextField</td>
	''' </tr>
	''' <tr>
	'''   <td>select, size &gt; 1 or multiple attribute defined</td>
	'''   <td>JList in a JScrollPane</td>
	''' </tr>
	''' <tr>
	'''   <td>select, size unspecified or 1</td>
	'''   <td>JComboBox</td>
	''' </tr>
	''' <tr>
	'''   <td>textarea</td>
	'''   <td>JTextArea in a JScrollPane</td>
	''' </tr>
	''' <tr>
	'''   <td>input, type file</td>
	'''   <td>JTextField</td>
	''' </tr>
	''' </table>
	''' 
	''' @author Timothy Prinzing
	''' @author Sunita Mani
	''' </summary>
	Public Class FormView
		Inherits ComponentView
		Implements ActionListener

		''' <summary>
		''' If a value attribute is not specified for a FORM input element
		''' of type "submit", then this default string is used.
		''' </summary>
		''' @deprecated As of 1.3, value now comes from UIManager property
		'''             FormView.submitButtonText 
		<Obsolete("As of 1.3, value now comes from UIManager property")> _
		Public Shared ReadOnly SUBMIT As New String("Submit Query")
		''' <summary>
		''' If a value attribute is not specified for a FORM input element
		''' of type "reset", then this default string is used.
		''' </summary>
		''' @deprecated As of 1.3, value comes from UIManager UIManager property
		'''             FormView.resetButtonText 
		<Obsolete("As of 1.3, value comes from UIManager UIManager property")> _
		Public Shared ReadOnly RESET As New String("Reset")

		''' <summary>
		''' Document attribute name for storing POST data. JEditorPane.getPostData()
		''' uses the same name, should be kept in sync.
		''' </summary>
		Friend Const PostDataProperty As String = "javax.swing.JEditorPane.postdata"

		''' <summary>
		''' Used to indicate if the maximum span should be the same as the
		''' preferred span. This is used so that the Component's size doesn't
		''' change if there is extra room on a line. The first bit is used for
		''' the X direction, and the second for the y direction.
		''' </summary>
		Private maxIsPreferred As Short

		''' <summary>
		''' Creates a new FormView object.
		''' </summary>
		''' <param name="elem"> the element to decorate </param>
		Public Sub New(ByVal elem As Element)
			MyBase.New(elem)
		End Sub

		''' <summary>
		''' Create the component.  This is basically a
		''' big switch statement based upon the tag type
		''' and html attributes of the associated element.
		''' </summary>
		Protected Friend Overrides Function createComponent() As Component
			Dim attr As AttributeSet = element.attributes
			Dim t As HTML.Tag = CType(attr.getAttribute(StyleConstants.NameAttribute), HTML.Tag)
			Dim c As JComponent = Nothing
			Dim model As Object = attr.getAttribute(StyleConstants.ModelAttribute)

			' Remove listeners previously registered in shared model
			' when a new UI component is replaced.  See bug 7189299.
			removeStaleListenerForModel(model)
			If t Is HTML.Tag.INPUT Then
				c = createInputComponent(attr, model)
			ElseIf t Is HTML.Tag.SELECT Then

				If TypeOf model Is OptionListModel Then

					Dim list As New JList(CType(model, ListModel))
					Dim ___size As Integer = HTML.getIntegerAttributeValue(attr, HTML.Attribute.SIZE, 1)
					list.visibleRowCount = ___size
					list.selectionModel = CType(model, ListSelectionModel)
					c = New JScrollPane(list)
				Else
					c = New JComboBox(CType(model, ComboBoxModel))
					maxIsPreferred = 3
				End If
			ElseIf t Is HTML.Tag.TEXTAREA Then
				Dim area As New JTextArea(CType(model, Document))
				Dim rows As Integer = HTML.getIntegerAttributeValue(attr, HTML.Attribute.ROWS, 1)
				area.rows = rows
				Dim cols As Integer = HTML.getIntegerAttributeValue(attr, HTML.Attribute.COLS, 20)
				maxIsPreferred = 3
				area.columns = cols
				c = New JScrollPane(area, JScrollPane.VERTICAL_SCROLLBAR_ALWAYS, JScrollPane.HORIZONTAL_SCROLLBAR_ALWAYS)
			End If

			If c IsNot Nothing Then c.alignmentY = 1.0f
			Return c
		End Function


		''' <summary>
		''' Creates a component for an &lt;INPUT&gt; element based on the
		''' value of the "type" attribute.
		''' </summary>
		''' <param name="set"> of attributes associated with the &lt;INPUT&gt; element. </param>
		''' <param name="model"> the value of the StyleConstants.ModelAttribute </param>
		''' <returns> the component. </returns>
		Private Function createInputComponent(ByVal attr As AttributeSet, ByVal model As Object) As JComponent
			Dim c As JComponent = Nothing
			Dim type As String = CStr(attr.getAttribute(HTML.Attribute.TYPE))

			If type.Equals("submit") OrElse type.Equals("reset") Then
				Dim value As String = CStr(attr.getAttribute(HTML.Attribute.VALUE))
				If value Is Nothing Then
					If type.Equals("submit") Then
						value = UIManager.getString("FormView.submitButtonText")
					Else
						value = UIManager.getString("FormView.resetButtonText")
					End If
				End If
				Dim button As New JButton(value)
				If model IsNot Nothing Then
					button.model = CType(model, ButtonModel)
					button.addActionListener(Me)
				End If
				c = button
				maxIsPreferred = 3
			ElseIf type.Equals("image") Then
				Dim srcAtt As String = CStr(attr.getAttribute(HTML.Attribute.SRC))
				Dim button As JButton
				Try
					Dim base As URL = CType(element.document, HTMLDocument).base
					Dim srcURL As New URL(base, srcAtt)
					Dim icon As Icon = New ImageIcon(srcURL)
					button = New JButton(icon)
				Catch e As MalformedURLException
					button = New JButton(srcAtt)
				End Try
				If model IsNot Nothing Then
					button.model = CType(model, ButtonModel)
					button.addMouseListener(New MouseEventListener(Me))
				End If
				c = button
				maxIsPreferred = 3
			ElseIf type.Equals("checkbox") Then
				c = New JCheckBox
				If model IsNot Nothing Then CType(c, JCheckBox).model = CType(model, JToggleButton.ToggleButtonModel)
				maxIsPreferred = 3
			ElseIf type.Equals("radio") Then
				c = New JRadioButton
				If model IsNot Nothing Then CType(c, JRadioButton).model = CType(model, JToggleButton.ToggleButtonModel)
				maxIsPreferred = 3
			ElseIf type.Equals("text") Then
				Dim ___size As Integer = HTML.getIntegerAttributeValue(attr, HTML.Attribute.SIZE, -1)
				Dim field As JTextField
				If ___size > 0 Then
					field = New JTextField
					field.columns = ___size
				Else
					field = New JTextField
					field.columns = 20
				End If
				c = field
				If model IsNot Nothing Then field.document = CType(model, Document)
				field.addActionListener(Me)
				maxIsPreferred = 3
			ElseIf type.Equals("password") Then
				Dim field As New JPasswordField
				c = field
				If model IsNot Nothing Then field.document = CType(model, Document)
				Dim ___size As Integer = HTML.getIntegerAttributeValue(attr, HTML.Attribute.SIZE, -1)
				field.columns = If(___size > 0, ___size, 20)
				field.addActionListener(Me)
				maxIsPreferred = 3
			ElseIf type.Equals("file") Then
				Dim field As New JTextField
				If model IsNot Nothing Then field.document = CType(model, Document)
				Dim ___size As Integer = HTML.getIntegerAttributeValue(attr, HTML.Attribute.SIZE, -1)
				field.columns = If(___size > 0, ___size, 20)
				Dim browseButton As New JButton(UIManager.getString("FormView.browseFileButtonText"))
				Dim ___box As Box = Box.createHorizontalBox()
				___box.add(field)
				___box.add(Box.createHorizontalStrut(5))
				___box.add(browseButton)
				browseButton.addActionListener(New BrowseFileAction(Me, attr, CType(model, Document)))
				c = ___box
				maxIsPreferred = 3
			End If
			Return c
		End Function

		Private Sub removeStaleListenerForModel(ByVal model As Object)
			If TypeOf model Is DefaultButtonModel Then
				' case of JButton whose model is DefaultButtonModel
				' Need to remove stale ActionListener, ChangeListener and
				' ItemListener that are instance of AbstractButton$Handler.
				Dim buttonModel As DefaultButtonModel = CType(model, DefaultButtonModel)
				Dim listenerClass As String = "javax.swing.AbstractButton$Handler"
				For Each listener As ActionListener In buttonModel.actionListeners
					If listenerClass.Equals(listener.GetType().name) Then buttonModel.removeActionListener(listener)
				Next listener
				For Each listener As ChangeListener In buttonModel.changeListeners
					If listenerClass.Equals(listener.GetType().name) Then buttonModel.removeChangeListener(listener)
				Next listener
				For Each listener As ItemListener In buttonModel.itemListeners
					If listenerClass.Equals(listener.GetType().name) Then buttonModel.removeItemListener(listener)
				Next listener
			ElseIf TypeOf model Is AbstractListModel Then
				' case of JComboBox and JList
				' For JList, the stale ListDataListener is instance
				' BasicListUI$Handler.
				' For JComboBox, there are 2 stale ListDataListeners, which are
				' BasicListUI$Handler and BasicComboBoxUI$Handler.
				Dim listModel As AbstractListModel = CType(model, AbstractListModel)
				Dim listenerClass1 As String = "javax.swing.plaf.basic.BasicListUI$Handler"
				Dim listenerClass2 As String = "javax.swing.plaf.basic.BasicComboBoxUI$Handler"
				For Each listener As ListDataListener In listModel.listDataListeners
					If listenerClass1.Equals(listener.GetType().name) OrElse listenerClass2.Equals(listener.GetType().name) Then listModel.removeListDataListener(listener)
				Next listener
			ElseIf TypeOf model Is AbstractDocument Then
				' case of JPasswordField, JTextField and JTextArea
				' All have 2 stale DocumentListeners.
				Dim listenerClass1 As String = "javax.swing.plaf.basic.BasicTextUI$UpdateHandler"
				Dim listenerClass2 As String = "javax.swing.text.DefaultCaret$Handler"
				Dim docModel As AbstractDocument = CType(model, AbstractDocument)
				For Each listener As DocumentListener In docModel.documentListeners
					If listenerClass1.Equals(listener.GetType().name) OrElse listenerClass2.Equals(listener.GetType().name) Then docModel.removeDocumentListener(listener)
				Next listener
			End If
		End Sub

		''' <summary>
		''' Determines the maximum span for this view along an
		''' axis. For certain components, the maximum and preferred span are the
		''' same. For others this will return the value
		''' returned by Component.getMaximumSize along the
		''' axis of interest.
		''' </summary>
		''' <param name="axis"> may be either View.X_AXIS or View.Y_AXIS </param>
		''' <returns>   the span the view would like to be rendered into &gt;= 0.
		'''           Typically the view is told to render into the span
		'''           that is returned, although there is no guarantee.
		'''           The parent may choose to resize or break the view. </returns>
		''' <exception cref="IllegalArgumentException"> for an invalid axis </exception>
		Public Overrides Function getMaximumSpan(ByVal axis As Integer) As Single
			Select Case axis
			Case View.X_AXIS
				If (maxIsPreferred And 1) = 1 Then
					MyBase.getMaximumSpan(axis)
					Return getPreferredSpan(axis)
				End If
				Return MyBase.getMaximumSpan(axis)
			Case View.Y_AXIS
				If (maxIsPreferred And 2) = 2 Then
					MyBase.getMaximumSpan(axis)
					Return getPreferredSpan(axis)
				End If
				Return MyBase.getMaximumSpan(axis)
			Case Else
			End Select
			Return MyBase.getMaximumSpan(axis)
		End Function


		''' <summary>
		''' Responsible for processing the ActionEvent.
		''' If the element associated with the FormView,
		''' has a type of "submit", "reset", "text" or "password"
		''' then the action is processed.  In the case of a "submit"
		''' the form is submitted.  In the case of a "reset"
		''' the form is reset to its original state.
		''' In the case of "text" or "password", if the
		''' element is the last one of type "text" or "password",
		''' the form is submitted.  Otherwise, focus is transferred
		''' to the next component in the form.
		''' </summary>
		''' <param name="evt"> the ActionEvent. </param>
		Public Overridable Sub actionPerformed(ByVal evt As ActionEvent)
			Dim ___element As Element = element
			Dim dataBuffer As New StringBuilder
			Dim doc As HTMLDocument = CType(document, HTMLDocument)
			Dim attr As AttributeSet = ___element.attributes

			Dim type As String = CStr(attr.getAttribute(HTML.Attribute.TYPE))

			If type.Equals("submit") Then
				getFormData(dataBuffer)
				submitData(dataBuffer.ToString())
			ElseIf type.Equals("reset") Then
				resetForm()
			ElseIf type.Equals("text") OrElse type.Equals("password") Then
				If lastTextOrPasswordField Then
					getFormData(dataBuffer)
					submitData(dataBuffer.ToString())
				Else
					component.transferFocus()
				End If
			End If
		End Sub


		''' <summary>
		''' This method is responsible for submitting the form data.
		''' A thread is forked to undertake the submission.
		''' </summary>
		Protected Friend Overridable Sub submitData(ByVal data As String)
			Dim form As Element = formElement
			Dim attrs As AttributeSet = form.attributes
			Dim doc As HTMLDocument = CType(form.document, HTMLDocument)
			Dim base As URL = doc.base

			Dim target As String = CStr(attrs.getAttribute(HTML.Attribute.TARGET))
			If target Is Nothing Then target = "_self"

			Dim method As String = CStr(attrs.getAttribute(HTML.Attribute.METHOD))
			If method Is Nothing Then method = "GET"
			method = method.ToLower()
			Dim isPostMethod As Boolean = method.Equals("post")
			If isPostMethod Then storePostData(doc, target, data)

			Dim action As String = CStr(attrs.getAttribute(HTML.Attribute.ACTION))
			Dim actionURL As URL
			Try
				actionURL = If(action Is Nothing, New URL(base.protocol, base.host, base.port, base.file), New URL(base, action))
				If Not isPostMethod Then
					Dim query As String = data.ToString()
					actionURL = New URL(actionURL & "?" & query)
				End If
			Catch e As MalformedURLException
				actionURL = Nothing
			End Try
			Dim c As JEditorPane = CType(container, JEditorPane)
			Dim kit As HTMLEditorKit = CType(c.editorKit, HTMLEditorKit)

			Dim formEvent As FormSubmitEvent = Nothing
			If (Not kit.autoFormSubmission) OrElse doc.frameDocument Then
				Dim methodType As FormSubmitEvent.MethodType = If(isPostMethod, FormSubmitEvent.MethodType.POST, FormSubmitEvent.MethodType.GET)
				formEvent = New FormSubmitEvent(FormView.this, HyperlinkEvent.EventType.ACTIVATED, actionURL, form, target, methodType, data)

			End If
			' setPage() may take significant time so schedule it to run later.
			Dim fse As FormSubmitEvent = formEvent
			Dim url As URL = actionURL
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			SwingUtilities.invokeLater(New Runnable()
	'		{
	'			public void run()
	'			{
	'				if (fse != Nothing)
	'				{
	'					c.fireHyperlinkUpdate(fse);
	'				}
	'				else
	'				{
	'					try
	'					{
	'						c.setPage(url);
	'					}
	'					catch (IOException e)
	'					{
	'						UIManager.getLookAndFeel().provideErrorFeedback(c);
	'					}
	'				}
	'			}
	'		});
		End Sub

		Private Sub storePostData(ByVal doc As HTMLDocument, ByVal target As String, ByVal data As String)

	'         POST data is stored into the document property named by constant
	'         * PostDataProperty from where it is later retrieved by method
	'         * JEditorPane.getPostData().  If the current document is in a frame,
	'         * the data is initially put into the toplevel (frameset) document
	'         * property (named <PostDataProperty>.<Target frame name>).  It is the
	'         * responsibility of FrameView which updates the target frame
	'         * to move data from the frameset document property into the frame
	'         * document property.
	'         

			Dim propDoc As Document = doc
			Dim propName As String = PostDataProperty

			If doc.frameDocument Then
				' find the top-most JEditorPane holding the frameset view.
				Dim p As FrameView.FrameEditorPane = CType(container, FrameView.FrameEditorPane)
				Dim v As FrameView = p.frameView
				Dim c As JEditorPane = v.outermostJEditorPane
				If c IsNot Nothing Then
					propDoc = c.document
					propName += ("." & target)
				End If
			End If

			propDoc.putProperty(propName, data)
		End Sub

		''' <summary>
		''' MouseEventListener class to handle form submissions when
		''' an input with type equal to image is clicked on.
		''' A MouseListener is necessary since along with the image
		''' data the coordinates associated with the mouse click
		''' need to be submitted.
		''' </summary>
		Protected Friend Class MouseEventListener
			Inherits MouseAdapter

			Private ReadOnly outerInstance As FormView

			Public Sub New(ByVal outerInstance As FormView)
				Me.outerInstance = outerInstance
			End Sub


			Public Overridable Sub mouseReleased(ByVal evt As MouseEvent)
				Dim imageData As String = outerInstance.getImageData(evt.point)
				outerInstance.imageSubmit(imageData)
			End Sub
		End Class

		''' <summary>
		''' This method is called to submit a form in response
		''' to a click on an image -- an &lt;INPUT&gt; form
		''' element of type "image".
		''' </summary>
		''' <param name="imageData"> the mouse click coordinates. </param>
		Protected Friend Overridable Sub imageSubmit(ByVal imageData As String)

			Dim dataBuffer As New StringBuilder
			Dim elem As Element = element
			Dim hdoc As HTMLDocument = CType(elem.document, HTMLDocument)
			getFormData(dataBuffer)
			If dataBuffer.Length > 0 Then dataBuffer.Append("&"c)
			dataBuffer.Append(imageData)
			submitData(dataBuffer.ToString())
			Return
		End Sub

		''' <summary>
		''' Extracts the value of the name attribute
		''' associated with the input element of type
		''' image.  If name is defined it is encoded using
		''' the URLEncoder.encode() method and the
		''' image data is returned in the following format:
		'''      name + ".x" +"="+ x +"&"+ name +".y"+"="+ y
		''' otherwise,
		'''      "x="+ x +"&y="+ y
		''' </summary>
		''' <param name="point"> associated with the mouse click. </param>
		''' <returns> the image data. </returns>
		Private Function getImageData(ByVal point As Point) As String

			Dim mouseCoords As String = point.x & ":" & point.y
			Dim sep As Integer = mouseCoords.IndexOf(":"c)
			Dim x As String = mouseCoords.Substring(0, sep)
			sep += 1
			Dim y As String = mouseCoords.Substring(sep)
			Dim name As String = CStr(element.attributes.getAttribute(HTML.Attribute.NAME))

			Dim data As String
			If name Is Nothing OrElse name.Equals("") Then
				data = "x=" & x & "&y=" & y
			Else
				name = URLEncoder.encode(name)
				data = name & ".x" & "=" & x & "&" & name & ".y" & "=" & y
			End If
			Return data
		End Function


		''' <summary>
		''' The following methods provide functionality required to
		''' iterate over a the elements of the form and in the case
		''' of a form submission, extract the data from each model
		''' that is associated with each form element, and in the
		''' case of reset, reinitialize the each model to its
		''' initial state.
		''' </summary>


		''' <summary>
		''' Returns the Element representing the <code>FORM</code>.
		''' </summary>
		Private Property formElement As Element
			Get
				Dim elem As Element = element
				Do While elem IsNot Nothing
					If elem.attributes.getAttribute(StyleConstants.NameAttribute) Is HTML.Tag.FORM Then Return elem
					elem = elem.parentElement
				Loop
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Iterates over the
		''' element hierarchy, extracting data from the
		''' models associated with the relevant form elements.
		''' "Relevant" means the form elements that are part
		''' of the same form whose element triggered the submit
		''' action.
		''' </summary>
		''' <param name="buffer">        the buffer that contains that data to submit </param>
		''' <param name="targetElement"> the element that triggered the
		'''                      form submission </param>
		Private Sub getFormData(ByVal buffer As StringBuilder)
			Dim formE As Element = formElement
			If formE IsNot Nothing Then
				Dim it As New ElementIterator(formE)
				Dim [next] As Element

				[next] = it.next()
				Do While [next] IsNot Nothing
					If isControl([next]) Then
						Dim type As String = CStr([next].attributes.getAttribute(HTML.Attribute.TYPE))

						If type IsNot Nothing AndAlso type.Equals("submit") AndAlso [next] IsNot element Then
							' do nothing - this submit is not the trigger
						ElseIf type Is Nothing OrElse (Not type.Equals("image")) Then
							' images only result in data if they triggered
							' the submit and they require that the mouse click
							' coords be appended to the data.  Hence its
							' processing is handled by the view.
							loadElementDataIntoBuffer([next], buffer)
						End If
					End If
					[next] = it.next()
				Loop
			End If
		End Sub

		''' <summary>
		''' Loads the data
		''' associated with the element into the buffer.
		''' The format in which data is appended depends
		''' on the type of the form element.  Essentially
		''' data is loaded in name/value pairs.
		''' 
		''' </summary>
		Private Sub loadElementDataIntoBuffer(ByVal elem As Element, ByVal buffer As StringBuilder)

			Dim attr As AttributeSet = elem.attributes
			Dim name As String = CStr(attr.getAttribute(HTML.Attribute.NAME))
			If name Is Nothing Then Return
			Dim value As String = Nothing
			Dim tag As HTML.Tag = CType(elem.attributes.getAttribute(StyleConstants.NameAttribute), HTML.Tag)

			If tag Is HTML.Tag.INPUT Then
				value = getInputElementData(attr)
			ElseIf tag Is HTML.Tag.TEXTAREA Then
				value = getTextAreaData(attr)
			ElseIf tag Is HTML.Tag.SELECT Then
				loadSelectData(attr, buffer)
			End If

			If name IsNot Nothing AndAlso value IsNot Nothing Then appendBuffer(buffer, name, value)
		End Sub


		''' <summary>
		''' Returns the data associated with an &lt;INPUT&gt; form
		''' element.  The value of "type" attributes is
		''' used to determine the type of the model associated
		''' with the element and then the relevant data is
		''' extracted.
		''' </summary>
		Private Function getInputElementData(ByVal attr As AttributeSet) As String

			Dim model As Object = attr.getAttribute(StyleConstants.ModelAttribute)
			Dim type As String = CStr(attr.getAttribute(HTML.Attribute.TYPE))
			Dim value As String = Nothing

			If type.Equals("text") OrElse type.Equals("password") Then
				Dim doc As Document = CType(model, Document)
				Try
					value = doc.getText(0, doc.length)
				Catch e As BadLocationException
					value = Nothing
				End Try
			ElseIf type.Equals("submit") OrElse type.Equals("hidden") Then
				value = CStr(attr.getAttribute(HTML.Attribute.VALUE))
				If value Is Nothing Then value = ""
			ElseIf type.Equals("radio") OrElse type.Equals("checkbox") Then
				Dim m As ButtonModel = CType(model, ButtonModel)
				If m.selected Then
					value = CStr(attr.getAttribute(HTML.Attribute.VALUE))
					If value Is Nothing Then value = "on"
				End If
			ElseIf type.Equals("file") Then
				Dim doc As Document = CType(model, Document)
				Dim path As String

				Try
					path = doc.getText(0, doc.length)
				Catch e As BadLocationException
					path = Nothing
				End Try
				If path IsNot Nothing AndAlso path.Length > 0 Then value = path
			End If
			Return value
		End Function

		''' <summary>
		''' Returns the data associated with the &lt;TEXTAREA&gt; form
		''' element.  This is done by getting the text stored in the
		''' Document model.
		''' </summary>
		Private Function getTextAreaData(ByVal attr As AttributeSet) As String
			Dim doc As Document = CType(attr.getAttribute(StyleConstants.ModelAttribute), Document)
			Try
				Return doc.getText(0, doc.length)
			Catch e As BadLocationException
				Return Nothing
			End Try
		End Function


		''' <summary>
		''' Loads the buffer with the data associated with the Select
		''' form element.  Basically, only items that are selected
		''' and have their name attribute set are added to the buffer.
		''' </summary>
		Private Sub loadSelectData(ByVal attr As AttributeSet, ByVal buffer As StringBuilder)

			Dim name As String = CStr(attr.getAttribute(HTML.Attribute.NAME))
			If name Is Nothing Then Return
			Dim m As Object = attr.getAttribute(StyleConstants.ModelAttribute)
			If TypeOf m Is OptionListModel Then
				Dim model As OptionListModel(Of [Option]) = CType(m, OptionListModel(Of [Option]))

				For i As Integer = 0 To model.size - 1
					If model.isSelectedIndex(i) Then
						Dim [option] As [Option] = model.getElementAt(i)
						appendBuffer(buffer, name, [option].value)
					End If
				Next i
			ElseIf TypeOf m Is ComboBoxModel Then
				Dim model As ComboBoxModel = CType(m, ComboBoxModel)
				Dim [option] As [Option] = CType(model.selectedItem, [Option])
				If [option] IsNot Nothing Then appendBuffer(buffer, name, [option].value)
			End If
		End Sub

		''' <summary>
		''' Appends name / value pairs into the
		''' buffer.  Both names and values are encoded using the
		''' URLEncoder.encode() method before being added to the
		''' buffer.
		''' </summary>
		Private Sub appendBuffer(ByVal buffer As StringBuilder, ByVal name As String, ByVal value As String)
			If buffer.Length > 0 Then buffer.Append("&"c)
			Dim encodedName As String = URLEncoder.encode(name)
			buffer.Append(encodedName)
			buffer.Append("="c)
			Dim encodedValue As String = URLEncoder.encode(value)
			buffer.Append(encodedValue)
		End Sub

		''' <summary>
		''' Returns true if the Element <code>elem</code> represents a control.
		''' </summary>
		Private Function isControl(ByVal elem As Element) As Boolean
			Return elem.leaf
		End Function

		''' <summary>
		''' Iterates over the element hierarchy to determine if
		''' the element parameter, which is assumed to be an
		''' &lt;INPUT&gt; element of type password or text, is the last
		''' one of either kind, in the form to which it belongs.
		''' </summary>
		Friend Overridable Property lastTextOrPasswordField As Boolean
			Get
				Dim ___parent As Element = formElement
				Dim elem As Element = element
    
				If ___parent IsNot Nothing Then
					Dim it As New ElementIterator(___parent)
					Dim [next] As Element
					Dim found As Boolean = False
    
					[next] = it.next()
					Do While [next] IsNot Nothing
						If [next] Is elem Then
							found = True
						ElseIf found AndAlso isControl([next]) Then
							Dim elemAttr As AttributeSet = [next].attributes
    
							If HTMLDocument.matchNameAttribute(elemAttr, HTML.Tag.INPUT) Then
								Dim type As String = CStr(elemAttr.getAttribute(HTML.Attribute.TYPE))
    
								If "text".Equals(type) OrElse "password".Equals(type) Then Return False
							End If
						End If
						[next] = it.next()
					Loop
				End If
				Return True
			End Get
		End Property

		''' <summary>
		''' Resets the form
		''' to its initial state by reinitializing the models
		''' associated with each form element to their initial
		''' values.
		''' 
		''' param elem the element that triggered the reset
		''' </summary>
		Friend Overridable Sub resetForm()
			Dim ___parent As Element = formElement

			If ___parent IsNot Nothing Then
				Dim it As New ElementIterator(___parent)
				Dim [next] As Element

				[next] = it.next()
				Do While [next] IsNot Nothing
					If isControl([next]) Then
						Dim elemAttr As AttributeSet = [next].attributes
						Dim m As Object = elemAttr.getAttribute(StyleConstants.ModelAttribute)
						If TypeOf m Is TextAreaDocument Then
							Dim doc As TextAreaDocument = CType(m, TextAreaDocument)
							doc.reset()
						ElseIf TypeOf m Is PlainDocument Then
							Try
								Dim doc As PlainDocument = CType(m, PlainDocument)
								doc.remove(0, doc.length)
								If HTMLDocument.matchNameAttribute(elemAttr, HTML.Tag.INPUT) Then
									Dim value As String = CStr(elemAttr.getAttribute(HTML.Attribute.VALUE))
									If value IsNot Nothing Then doc.insertString(0, value, Nothing)
								End If
							Catch e As BadLocationException
							End Try
						ElseIf TypeOf m Is OptionListModel Then
							Dim model As OptionListModel = CType(m, OptionListModel)
							Dim ___size As Integer = model.size
							For i As Integer = 0 To ___size - 1
								model.removeIndexInterval(i, i)
							Next i
							Dim selectionRange As BitArray = model.initialSelection
							For i As Integer = 0 To selectionRange.Count - 1
								If selectionRange.Get(i) Then model.addSelectionInterval(i, i)
							Next i
						ElseIf TypeOf m Is OptionComboBoxModel Then
							Dim model As OptionComboBoxModel = CType(m, OptionComboBoxModel)
							Dim [option] As [Option] = model.initialSelection
							If [option] IsNot Nothing Then model.selectedItem = [option]
						ElseIf TypeOf m Is JToggleButton.ToggleButtonModel Then
							Dim checked As Boolean = (CStr(elemAttr.getAttribute(HTML.Attribute.CHECKED)) IsNot Nothing)
							Dim model As JToggleButton.ToggleButtonModel = CType(m, JToggleButton.ToggleButtonModel)
							model.selected = checked
						End If
					End If
					[next] = it.next()
				Loop
			End If
		End Sub


		''' <summary>
		''' BrowseFileAction is used for input type == file. When the user
		''' clicks the button a JFileChooser is brought up allowing the user
		''' to select a file in the file system. The resulting path to the selected
		''' file is set in the text field (actually an instance of Document).
		''' </summary>
		Private Class BrowseFileAction
			Implements ActionListener

			Private ReadOnly outerInstance As FormView

			Private attrs As AttributeSet
			Private model As Document

			Friend Sub New(ByVal outerInstance As FormView, ByVal attrs As AttributeSet, ByVal model As Document)
					Me.outerInstance = outerInstance
				Me.attrs = attrs
				Me.model = model
			End Sub

			Public Overridable Sub actionPerformed(ByVal ae As ActionEvent)
				' PENDING: When mime support is added to JFileChooser use the
				' accept value of attrs.
				Dim fc As New JFileChooser
				fc.multiSelectionEnabled = False
				If fc.showOpenDialog(outerInstance.container) = JFileChooser.APPROVE_OPTION Then
					Dim selected As File = fc.selectedFile

					If selected IsNot Nothing Then
						Try
							If model.length > 0 Then model.remove(0, model.length)
							model.insertString(0, selected.path, Nothing)
						Catch ble As BadLocationException
						End Try
					End If
				End If
			End Sub
		End Class
	End Class

End Namespace