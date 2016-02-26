Imports System
Imports javax.swing
Imports javax.swing.text
Imports javax.swing.event

'
' * Copyright (c) 1998, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' Implements a FrameView, intended to support the HTML
	''' &lt;FRAME&gt; tag.  Supports the frameborder, scrolling,
	''' marginwidth and marginheight attributes.
	''' 
	''' @author    Sunita Mani
	''' </summary>

	Friend Class FrameView
		Inherits ComponentView
		Implements HyperlinkListener


		Friend htmlPane As JEditorPane
		Friend scroller As JScrollPane
		Friend editable As Boolean
		Friend width As Single
		Friend height As Single
		Friend src As URL
		''' <summary>
		''' Set to true when the component has been created. </summary>
		Private createdComponent As Boolean

		''' <summary>
		''' Creates a new Frame.
		''' </summary>
		''' <param name="elem"> the element to represent. </param>
		Public Sub New(ByVal elem As Element)
			MyBase.New(elem)
		End Sub

		Protected Friend Overrides Function createComponent() As Component

			Dim elem As Element = element
			Dim ___attributes As AttributeSet = elem.attributes
			Dim srcAtt As String = CStr(___attributes.getAttribute(HTML.Attribute.SRC))

			If (srcAtt IsNot Nothing) AndAlso ((Not srcAtt.Equals(""))) Then
				Try
					Dim base As URL = CType(elem.document, HTMLDocument).base
					src = New URL(base, srcAtt)
					htmlPane = New FrameEditorPane(Me)
					htmlPane.addHyperlinkListener(Me)
					Dim host As JEditorPane = hostPane
					Dim isAutoFormSubmission As Boolean = True
					If host IsNot Nothing Then
						htmlPane.editable = host.editable
						Dim charset As String = CStr(host.getClientProperty("charset"))
						If charset IsNot Nothing Then htmlPane.putClientProperty("charset", charset)
						Dim hostKit As HTMLEditorKit = CType(host.editorKit, HTMLEditorKit)
						If hostKit IsNot Nothing Then isAutoFormSubmission = hostKit.autoFormSubmission
					End If
					htmlPane.page = src
					Dim kit As HTMLEditorKit = CType(htmlPane.editorKit, HTMLEditorKit)
					If kit IsNot Nothing Then kit.autoFormSubmission = isAutoFormSubmission

					Dim doc As Document = htmlPane.document
					If TypeOf doc Is HTMLDocument Then CType(doc, HTMLDocument).frameDocumentState = True
					margingin()
					createScrollPane()
					borderder()
				Catch e As MalformedURLException
					Console.WriteLine(e.ToString())
					Console.Write(e.StackTrace)
				Catch e1 As IOException
					Console.WriteLine(e1.ToString())
					Console.Write(e1.StackTrace)
				End Try
			End If
			createdComponent = True
			Return scroller
		End Function

		Friend Overridable Property hostPane As JEditorPane
			Get
				Dim c As Container = container
				Do While (c IsNot Nothing) AndAlso Not(TypeOf c Is JEditorPane)
					c = c.parent
				Loop
				Return CType(c, JEditorPane)
			End Get
		End Property


		''' <summary>
		''' Sets the parent view for the FrameView.
		''' Also determines if the FrameView should be editable
		''' or not based on whether the JTextComponent that
		''' contains it is editable.
		''' </summary>
		''' <param name="parent"> View </param>
		Public Overrides Property parent As View
			Set(ByVal parent As View)
				If parent IsNot Nothing Then
					Dim t As JTextComponent = CType(parent.container, JTextComponent)
					editable = t.editable
				End If
				MyBase.parent = parent
			End Set
		End Property


		''' <summary>
		''' Also determines if the FrameView should be editable
		''' or not based on whether the JTextComponent that
		''' contains it is editable. And then proceeds to call
		''' the superclass to do the paint().
		''' </summary>
		''' <param name="parent"> View </param>
		''' <seealso cref= text.ComponentView#paint </seealso>
		Public Overrides Sub paint(ByVal g As Graphics, ByVal allocation As Shape)

			Dim host As Container = container
			If host IsNot Nothing AndAlso htmlPane IsNot Nothing AndAlso htmlPane.editable <> CType(host, JTextComponent).editable Then
				editable = CType(host, JTextComponent).editable
				htmlPane.editable = editable
			End If
			MyBase.paint(g, allocation)
		End Sub


		''' <summary>
		''' If the marginwidth or marginheight attributes have been specified,
		''' then the JEditorPane's margin's are set to the new values.
		''' </summary>
		Private Sub setMargin()
			Dim ___margin As Integer = 0
			Dim [in] As Insets = htmlPane.margin
			Dim newInsets As Insets
			Dim modified As Boolean = False
			Dim ___attributes As AttributeSet = element.attributes
			Dim marginStr As String = CStr(___attributes.getAttribute(HTML.Attribute.MARGINWIDTH))
			If [in] IsNot Nothing Then
				newInsets = New Insets([in].top, [in].left, [in].right, [in].bottom)
			Else
				newInsets = New Insets(0,0,0,0)
			End If
			If marginStr IsNot Nothing Then
				___margin = Convert.ToInt32(marginStr)
				If ___margin > 0 Then
					newInsets.left = ___margin
					newInsets.right = ___margin
					modified = True
				End If
			End If
			marginStr = CStr(___attributes.getAttribute(HTML.Attribute.MARGINHEIGHT))
			If marginStr IsNot Nothing Then
				___margin = Convert.ToInt32(marginStr)
				If ___margin > 0 Then
					newInsets.top = ___margin
					newInsets.bottom = ___margin
					modified = True
				End If
			End If
			If modified Then htmlPane.margin = newInsets
		End Sub

		''' <summary>
		''' If the frameborder attribute has been specified, either in the frame,
		''' or by the frames enclosing frameset, the JScrollPane's setBorder()
		''' method is invoked to achieve the desired look.
		''' </summary>
		Private Sub setBorder()

			Dim ___attributes As AttributeSet = element.attributes
			Dim frameBorder As String = CStr(___attributes.getAttribute(HTML.Attribute.FRAMEBORDER))
			If (frameBorder IsNot Nothing) AndAlso (frameBorder.Equals("no") OrElse frameBorder.Equals("0")) Then scroller.border = Nothing
		End Sub


		''' <summary>
		''' This method creates the JScrollPane.  The scrollbar policy is determined by
		''' the scrolling attribute.  If not defined, the default is "auto" which
		''' maps to the scrollbar's being displayed as needed.
		''' </summary>
		Private Sub createScrollPane()
			Dim ___attributes As AttributeSet = element.attributes
			Dim scrolling As String = CStr(___attributes.getAttribute(HTML.Attribute.SCROLLING))
			If scrolling Is Nothing Then scrolling = "auto"

			If Not scrolling.Equals("no") Then
				If scrolling.Equals("yes") Then
					scroller = New JScrollPane(JScrollPane.VERTICAL_SCROLLBAR_ALWAYS, JScrollPane.HORIZONTAL_SCROLLBAR_ALWAYS)
				Else
					' scrollbars will be displayed if needed
					'
					scroller = New JScrollPane
				End If
			Else
				scroller = New JScrollPane(JScrollPane.VERTICAL_SCROLLBAR_NEVER, JScrollPane.HORIZONTAL_SCROLLBAR_NEVER)
			End If

			Dim vp As JViewport = scroller.viewport
			vp.add(htmlPane)
			vp.backingStoreEnabled = True
			scroller.minimumSize = New Dimension(5,5)
			scroller.maximumSize = New Dimension(Integer.MaxValue, Integer.MaxValue)
		End Sub


		''' <summary>
		''' Finds the outermost FrameSetView.  It then
		''' returns that FrameSetView's container.
		''' </summary>
		Friend Overridable Property outermostJEditorPane As JEditorPane
			Get
    
				Dim ___parent As View = parent
				Dim frameSetView As FrameSetView = Nothing
				Do While ___parent IsNot Nothing
					If TypeOf ___parent Is FrameSetView Then frameSetView = CType(___parent, FrameSetView)
					___parent = ___parent.parent
				Loop
				If frameSetView IsNot Nothing Then Return CType(frameSetView.container, JEditorPane)
				Return Nothing
			End Get
		End Property


		''' <summary>
		''' Returns true if this frame is contained within
		''' a nested frameset.
		''' </summary>
		Private Function inNestedFrameSet() As Boolean
			Dim ___parent As FrameSetView = CType(parent, FrameSetView)
			Return (TypeOf ___parent.parent Is FrameSetView)
		End Function


		''' <summary>
		''' Notification of a change relative to a
		''' hyperlink. This method searches for the outermost
		''' JEditorPane, and then fires an HTMLFrameHyperlinkEvent
		''' to that frame.  In addition, if the target is _parent,
		''' and there is not nested framesets then the target is
		''' reset to _top.  If the target is _top, in addition to
		''' firing the event to the outermost JEditorPane, this
		''' method also invokes the setPage() method and explicitly
		''' replaces the current document with the destination url.
		''' </summary>
		''' <param name="HyperlinkEvent"> </param>
		Public Overridable Sub hyperlinkUpdate(ByVal evt As HyperlinkEvent) Implements HyperlinkListener.hyperlinkUpdate

			Dim c As JEditorPane = outermostJEditorPane
			If c Is Nothing Then Return

			If Not(TypeOf evt Is HTMLFrameHyperlinkEvent) Then
				c.fireHyperlinkUpdate(evt)
				Return
			End If

			Dim e As HTMLFrameHyperlinkEvent = CType(evt, HTMLFrameHyperlinkEvent)

			If e.eventType Is HyperlinkEvent.EventType.ACTIVATED Then
				Dim target As String = e.target
				Dim postTarget As String = target

				If target.Equals("_parent") AndAlso (Not inNestedFrameSet()) Then target = "_top"

				If TypeOf evt Is FormSubmitEvent Then
					Dim kit As HTMLEditorKit = CType(c.editorKit, HTMLEditorKit)
					If kit IsNot Nothing AndAlso kit.autoFormSubmission Then
						If target.Equals("_top") Then
							Try
								movePostData(c, postTarget)
								c.page = e.uRL
							Catch ex As IOException
								' Need a way to handle exceptions
							End Try
						Else
							Dim doc As HTMLDocument = CType(c.document, HTMLDocument)
							doc.processHTMLFrameHyperlinkEvent(e)
						End If
					Else
						c.fireHyperlinkUpdate(evt)
					End If
					Return
				End If

				If target.Equals("_top") Then
					Try
						c.page = e.uRL
					Catch ex As IOException
						' Need a way to handle exceptions
						' ex.printStackTrace();
					End Try
				End If
				If Not c.editable Then c.fireHyperlinkUpdate(New HTMLFrameHyperlinkEvent(c, e.eventType, e.uRL, e.description, element, e.inputEvent, target))
			End If
		End Sub

		''' <summary>
		''' Gives notification from the document that attributes were changed
		''' in a location that this view is responsible for.  Currently this view
		''' handles changes to its SRC attribute.
		''' </summary>
		''' <param name="e"> the change information from the associated document </param>
		''' <param name="a"> the current allocation of the view </param>
		''' <param name="f"> the factory to use to rebuild if the view has children
		'''  </param>
		Public Overrides Sub changedUpdate(ByVal e As DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)

			Dim elem As Element = element
			Dim ___attributes As AttributeSet = elem.attributes

			Dim oldPage As URL = src

			Dim srcAtt As String = CStr(___attributes.getAttribute(HTML.Attribute.SRC))
			Dim base As URL = CType(elem.document, HTMLDocument).base
			Try
				If Not createdComponent Then Return

				Dim postData As Object = movePostData(htmlPane, Nothing)
				src = New URL(base, srcAtt)
				If oldPage.Equals(src) AndAlso (src.ref Is Nothing) AndAlso (postData Is Nothing) Then Return

				htmlPane.page = src
				Dim newDoc As Document = htmlPane.document
				If TypeOf newDoc Is HTMLDocument Then CType(newDoc, HTMLDocument).frameDocumentState = True
			Catch e1 As MalformedURLException
				' Need a way to handle exceptions
				'e1.printStackTrace();
			Catch e2 As IOException
				' Need a way to handle exceptions
				'e2.printStackTrace();
			End Try
		End Sub

		''' <summary>
		''' Move POST data from temporary storage into the target document property.
		''' </summary>
		''' <returns> the POST data or null if no data found </returns>
		Private Function movePostData(ByVal targetPane As JEditorPane, ByVal frameName As String) As Object
			Dim postData As Object = Nothing
			Dim p As JEditorPane = outermostJEditorPane
			If p IsNot Nothing Then
				If frameName Is Nothing Then frameName = CStr(element.attributes.getAttribute(HTML.Attribute.NAME))
				If frameName IsNot Nothing Then
					Dim propName As String = FormView.PostDataProperty & "." & frameName
					Dim d As Document = p.document
					postData = d.getProperty(propName)
					If postData IsNot Nothing Then
						targetPane.document.putProperty(FormView.PostDataProperty, postData)
						d.putProperty(propName, Nothing)
					End If
				End If
			End If

			Return postData
		End Function

		''' <summary>
		''' Determines the minimum span for this view along an
		''' axis.
		''' </summary>
		''' <param name="axis"> may be either <code>View.X_AXIS</code> or
		'''  <code>View.Y_AXIS</code> </param>
		''' <returns> the preferred span; given that we do not
		''' support resizing of frames, the minimum span returned
		''' is the same as the preferred span
		'''  </returns>
		Public Overrides Function getMinimumSpan(ByVal axis As Integer) As Single
		  Return 5
		End Function

		''' <summary>
		''' Determines the maximum span for this view along an
		''' axis.
		''' </summary>
		''' <param name="axis"> may be either <code>View.X_AXIS</code> or
		'''  <code>View.Y_AXIS</code> </param>
		''' <returns> the preferred span; given that we do not
		''' support resizing of frames, the maximum span returned
		''' is the same as the preferred span
		'''  </returns>
		Public Overrides Function getMaximumSpan(ByVal axis As Integer) As Single
			Return Integer.MaxValue
		End Function

		''' <summary>
		''' Editor pane rendering frame of HTML document
		'''  It uses the same editor kits classes as outermost JEditorPane
		''' </summary>
		Friend Class FrameEditorPane
			Inherits JEditorPane
			Implements sun.swing.text.html.FrameEditorPaneTag

			Private ReadOnly outerInstance As FrameView

			Public Sub New(ByVal outerInstance As FrameView)
				Me.outerInstance = outerInstance
			End Sub

			Public Overrides Function getEditorKitForContentType(ByVal type As String) As EditorKit
				Dim ___editorKit As EditorKit = MyBase.getEditorKitForContentType(type)
				Dim outerMostJEditorPane As JEditorPane = Nothing
				outerMostJEditorPane = outerInstance.outermostJEditorPane
				If outerMostJEditorPane IsNot Nothing Then
					Dim inheritedEditorKit As EditorKit = outerMostJEditorPane.getEditorKitForContentType(type)
					If Not ___editorKit.GetType().Equals(inheritedEditorKit.GetType()) Then
						___editorKit = CType(inheritedEditorKit.clone(), EditorKit)
						editorKitForContentTypeype(type, ___editorKit)
					End If
				End If
				Return ___editorKit
			End Function

			Friend Overridable Property frameView As FrameView
				Get
					Return FrameView.this
				End Get
			End Property
		End Class
	End Class

End Namespace