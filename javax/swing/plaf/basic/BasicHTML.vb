Imports Microsoft.VisualBasic
Imports System
Imports javax.swing
Imports javax.swing.text
Imports javax.swing.text.html

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
Namespace javax.swing.plaf.basic



	''' <summary>
	''' Support for providing html views for the swing components.
	''' This translates a simple html string to a javax.swing.text.View
	''' implementation that can render the html and provide the necessary
	''' layout semantics.
	''' 
	''' @author  Timothy Prinzing
	''' @since 1.3
	''' </summary>
	Public Class BasicHTML

		''' <summary>
		''' Create an html renderer for the given component and
		''' string of html.
		''' </summary>
		Public Shared Function createHTMLView(ByVal c As JComponent, ByVal html As String) As View
			Dim kit As BasicEditorKit = factory
			Dim doc As Document = kit.createDefaultDocument(c.font, c.foreground)
			Dim base As Object = c.getClientProperty(documentBaseKey)
			If TypeOf base Is java.net.URL Then CType(doc, HTMLDocument).base = CType(base, java.net.URL)
			Dim r As Reader = New StringReader(html)
			Try
				kit.read(r, doc, 0)
			Catch e As Exception
			End Try
			Dim f As ViewFactory = kit.viewFactory
			Dim hview As View = f.create(doc.defaultRootElement)
			Dim v As View = New Renderer(c, f, hview)
			Return v
		End Function

		''' <summary>
		''' Returns the baseline for the html renderer.
		''' </summary>
		''' <param name="view"> the View to get the baseline for </param>
		''' <param name="w"> the width to get the baseline for </param>
		''' <param name="h"> the height to get the baseline for </param>
		''' <exception cref="IllegalArgumentException"> if width or height is &lt; 0 </exception>
		''' <returns> baseline or a value &lt; 0 indicating there is no reasonable
		'''                  baseline </returns>
		''' <seealso cref= java.awt.FontMetrics </seealso>
		''' <seealso cref= javax.swing.JComponent#getBaseline(int,int)
		''' @since 1.6 </seealso>
		Public Shared Function getHTMLBaseline(ByVal ___view As View, ByVal w As Integer, ByVal h As Integer) As Integer
			If w < 0 OrElse h < 0 Then Throw New System.ArgumentException("Width and height must be >= 0")
			If TypeOf ___view Is Renderer Then Return getBaseline(___view.getView(0), w, h)
			Return -1
		End Function

		''' <summary>
		''' Gets the baseline for the specified component.  This digs out
		''' the View client property, and if non-null the baseline is calculated
		''' from it.  Otherwise the baseline is the value <code>y + ascent</code>.
		''' </summary>
		Friend Shared Function getBaseline(ByVal c As JComponent, ByVal y As Integer, ByVal ascent As Integer, ByVal w As Integer, ByVal h As Integer) As Integer
			Dim ___view As View = CType(c.getClientProperty(BasicHTML.propertyKey), View)
			If ___view IsNot Nothing Then
				Dim ___baseline As Integer = getHTMLBaseline(___view, w, h)
				If ___baseline < 0 Then Return ___baseline
				Return y + ___baseline
			End If
			Return y + ascent
		End Function

		''' <summary>
		''' Gets the baseline for the specified View.
		''' </summary>
		Friend Shared Function getBaseline(ByVal ___view As View, ByVal w As Integer, ByVal h As Integer) As Integer
			If hasParagraph(___view) Then
				___view.sizeize(w, h)
				Return getBaseline(___view, New Rectangle(0, 0, w, h))
			End If
			Return -1
		End Function

		Private Shared Function getBaseline(ByVal ___view As View, ByVal bounds As Shape) As Integer
			If ___view.viewCount = 0 Then Return -1
			Dim attributes As AttributeSet = ___view.element.attributes
			Dim name As Object = Nothing
			If attributes IsNot Nothing Then name = attributes.getAttribute(StyleConstants.NameAttribute)
			Dim index As Integer = 0
			If name Is HTML.Tag.HTML AndAlso ___view.viewCount > 1 Then index += 1
			bounds = ___view.getChildAllocation(index, bounds)
			If bounds Is Nothing Then Return -1
			Dim child As View = ___view.getView(index)
			If TypeOf ___view Is javax.swing.text.ParagraphView Then
				Dim rect As Rectangle
				If TypeOf bounds Is Rectangle Then
					rect = CType(bounds, Rectangle)
				Else
					rect = bounds.bounds
				End If
				Return rect.y + CInt(Fix(rect.height * child.getAlignment(View.Y_AXIS)))
			End If
			Return getBaseline(child, bounds)
		End Function

		Private Shared Function hasParagraph(ByVal ___view As View) As Boolean
			If TypeOf ___view Is javax.swing.text.ParagraphView Then Return True
			If ___view.viewCount = 0 Then Return False
			Dim attributes As AttributeSet = ___view.element.attributes
			Dim name As Object = Nothing
			If attributes IsNot Nothing Then name = attributes.getAttribute(StyleConstants.NameAttribute)
			Dim index As Integer = 0
			If name Is HTML.Tag.HTML AndAlso ___view.viewCount > 1 Then index = 1
			Return hasParagraph(___view.getView(index))
		End Function

		''' <summary>
		''' Check the given string to see if it should trigger the
		''' html rendering logic in a non-text component that supports
		''' html rendering.
		''' </summary>
		Public Shared Function isHTMLString(ByVal s As String) As Boolean
			If s IsNot Nothing Then
				If (s.Length >= 6) AndAlso (s.Chars(0) = "<"c) AndAlso (s.Chars(5) = ">"c) Then
					Dim tag As String = s.Substring(1, 4)
					Return tag.ToUpper() = propertyKey.ToUpper()
				End If
			End If
			Return False
		End Function

		''' <summary>
		''' Stash the HTML render for the given text into the client
		''' properties of the given JComponent. If the given text is
		''' <em>NOT HTML</em> the property will be cleared of any
		''' renderer.
		''' <p>
		''' This method is useful for ComponentUI implementations
		''' that are static (i.e. shared) and get their state
		''' entirely from the JComponent.
		''' </summary>
		Public Shared Sub updateRenderer(ByVal c As JComponent, ByVal text As String)
			Dim value As View = Nothing
			Dim oldValue As View = CType(c.getClientProperty(BasicHTML.propertyKey), View)
			Dim htmlDisabled As Boolean? = CBool(c.getClientProperty(htmlDisable))
			If htmlDisabled IsNot Boolean.TRUE AndAlso BasicHTML.isHTMLString(text) Then value = BasicHTML.createHTMLView(c, text)
			If value IsNot oldValue AndAlso oldValue IsNot Nothing Then
				For i As Integer = 0 To oldValue.viewCount - 1
					oldValue.getView(i).parent = Nothing
				Next i
			End If
			c.putClientProperty(BasicHTML.propertyKey, value)
		End Sub

		''' <summary>
		''' If this client property of a JComponent is set to Boolean.TRUE
		''' the component's 'text' property is never treated as HTML.
		''' </summary>
		Private Const htmlDisable As String = "html.disable"

		''' <summary>
		''' Key to use for the html renderer when stored as a
		''' client property of a JComponent.
		''' </summary>
		Public Const propertyKey As String = "html"

		''' <summary>
		''' Key stored as a client property to indicate the base that relative
		''' references are resolved against. For example, lets say you keep
		''' your images in the directory resources relative to the code path,
		''' you would use the following the set the base:
		''' <pre>
		'''   jComponent.putClientProperty(documentBaseKey,
		'''                                xxx.class.getResource("resources/"));
		''' </pre>
		''' </summary>
		Public Const documentBaseKey As String = "html.base"

		Friend Property Shared factory As BasicEditorKit
			Get
				If basicHTMLFactory Is Nothing Then
					basicHTMLViewFactory = New BasicHTMLViewFactory
					basicHTMLFactory = New BasicEditorKit
				End If
				Return basicHTMLFactory
			End Get
		End Property

		''' <summary>
		''' The source of the html renderers
		''' </summary>
		Private Shared basicHTMLFactory As BasicEditorKit

		''' <summary>
		''' Creates the Views that visually represent the model.
		''' </summary>
		Private Shared basicHTMLViewFactory As ViewFactory

		''' <summary>
		''' Overrides to the default stylesheet.  Should consider
		''' just creating a completely fresh stylesheet.
		''' </summary>
		Private Shared ReadOnly styleChanges As String = "p { margin-top: 0; margin-bottom: 0; margin-left: 0; margin-right: 0 }" & "body { margin-top: 0; margin-bottom: 0; margin-left: 0; margin-right: 0 }"

		''' <summary>
		''' The views produced for the ComponentUI implementations aren't
		''' going to be edited and don't need full html support.  This kit
		''' alters the HTMLEditorKit to try and trim things down a bit.
		''' It does the following:
		''' <ul>
		''' <li>It doesn't produce Views for things like comments,
		''' head, title, unknown tags, etc.
		''' <li>It installs a different set of css settings from the default
		''' provided by HTMLEditorKit.
		''' </ul>
		''' </summary>
		Friend Class BasicEditorKit
			Inherits HTMLEditorKit

			''' <summary>
			''' Shared base style for all documents created by us use. </summary>
			Private Shared defaultStyles As StyleSheet

			''' <summary>
			''' Overriden to return our own slimmed down style sheet.
			''' </summary>
			Public Property Overrides styleSheet As StyleSheet
				Get
					If defaultStyles Is Nothing Then
						defaultStyles = New StyleSheet
						Dim r As New StringReader(styleChanges)
						Try
							defaultStyles.loadRules(r, Nothing)
						Catch e As Exception
							' don't want to die in static initialization...
							' just display things wrong.
						End Try
						r.close()
						defaultStyles.addStyleSheet(MyBase.styleSheet)
					End If
					Return defaultStyles
				End Get
			End Property

			''' <summary>
			''' Sets the async policy to flush everything in one chunk, and
			''' to not display unknown tags.
			''' </summary>
			Public Overridable Function createDefaultDocument(ByVal defaultFont As Font, ByVal foreground As Color) As Document
				Dim styles As StyleSheet = styleSheet
				Dim ss As New StyleSheet
				ss.addStyleSheet(styles)
				Dim doc As New BasicDocument(ss, defaultFont, foreground)
				doc.asynchronousLoadPriority = Integer.MaxValue
				doc.preservesUnknownTags = False
				Return doc
			End Function

			''' <summary>
			''' Returns the ViewFactory that is used to make sure the Views don't
			''' load in the background.
			''' </summary>
			Public Property Overrides viewFactory As ViewFactory
				Get
					Return basicHTMLViewFactory
				End Get
			End Property
		End Class


		''' <summary>
		''' BasicHTMLViewFactory extends HTMLFactory to force images to be loaded
		''' synchronously.
		''' </summary>
		Friend Class BasicHTMLViewFactory
			Inherits HTMLEditorKit.HTMLFactory

			Public Overridable Function create(ByVal elem As Element) As View
				Dim ___view As View = MyBase.create(elem)

				If TypeOf ___view Is ImageView Then CType(___view, ImageView).loadsSynchronously = True
				Return ___view
			End Function
		End Class


		''' <summary>
		''' The subclass of HTMLDocument that is used as the model. getForeground
		''' is overridden to return the foreground property from the Component this
		''' was created for.
		''' </summary>
		Friend Class BasicDocument
			Inherits HTMLDocument

			''' <summary>
			''' The host, that is where we are rendering. </summary>
			' private JComponent host;

			Friend Sub New(ByVal s As StyleSheet, ByVal defaultFont As Font, ByVal foreground As Color)
				MyBase.New(s)
				preservesUnknownTags = False
				fontAndColorlor(defaultFont, foreground)
			End Sub

			''' <summary>
			''' Sets the default font and default color. These are set by
			''' adding a rule for the body that specifies the font and color.
			''' This allows the html to override these should it wish to have
			''' a custom font or color.
			''' </summary>
			Private Sub setFontAndColor(ByVal font As Font, ByVal fg As Color)
				styleSheet.addRule(sun.swing.SwingUtilities2.0 displayPropertiesToCSS(font,fg))
			End Sub
		End Class


		''' <summary>
		''' Root text view that acts as an HTML renderer.
		''' </summary>
		Friend Class Renderer
			Inherits View

			Friend Sub New(ByVal c As JComponent, ByVal f As ViewFactory, ByVal v As View)
				MyBase.New(Nothing)
				host = c
				factory = f
				view = v
				view.parent = Me
				' initially layout to the preferred size
				sizeize(view.getPreferredSpan(X_AXIS), view.getPreferredSpan(Y_AXIS))
			End Sub

			''' <summary>
			''' Fetches the attributes to use when rendering.  At the root
			''' level there are no attributes.  If an attribute is resolved
			''' up the view hierarchy this is the end of the line.
			''' </summary>
			Public Property Overrides attributes As AttributeSet
				Get
					Return Nothing
				End Get
			End Property

			''' <summary>
			''' Determines the preferred span for this view along an axis.
			''' </summary>
			''' <param name="axis"> may be either X_AXIS or Y_AXIS </param>
			''' <returns> the span the view would like to be rendered into.
			'''         Typically the view is told to render into the span
			'''         that is returned, although there is no guarantee.
			'''         The parent may choose to resize or break the view. </returns>
			Public Overrides Function getPreferredSpan(ByVal axis As Integer) As Single
				If axis = X_AXIS Then Return width
				Return view.getPreferredSpan(axis)
			End Function

			''' <summary>
			''' Determines the minimum span for this view along an axis.
			''' </summary>
			''' <param name="axis"> may be either X_AXIS or Y_AXIS </param>
			''' <returns> the span the view would like to be rendered into.
			'''         Typically the view is told to render into the span
			'''         that is returned, although there is no guarantee.
			'''         The parent may choose to resize or break the view. </returns>
			Public Overrides Function getMinimumSpan(ByVal axis As Integer) As Single
				Return view.getMinimumSpan(axis)
			End Function

			''' <summary>
			''' Determines the maximum span for this view along an axis.
			''' </summary>
			''' <param name="axis"> may be either X_AXIS or Y_AXIS </param>
			''' <returns> the span the view would like to be rendered into.
			'''         Typically the view is told to render into the span
			'''         that is returned, although there is no guarantee.
			'''         The parent may choose to resize or break the view. </returns>
			Public Overrides Function getMaximumSpan(ByVal axis As Integer) As Single
				Return Integer.MaxValue
			End Function

			''' <summary>
			''' Specifies that a preference has changed.
			''' Child views can call this on the parent to indicate that
			''' the preference has changed.  The root view routes this to
			''' invalidate on the hosting component.
			''' <p>
			''' This can be called on a different thread from the
			''' event dispatching thread and is basically unsafe to
			''' propagate into the component.  To make this safe,
			''' the operation is transferred over to the event dispatching
			''' thread for completion.  It is a design goal that all view
			''' methods be safe to call without concern for concurrency,
			''' and this behavior helps make that true.
			''' </summary>
			''' <param name="child"> the child view </param>
			''' <param name="width"> true if the width preference has changed </param>
			''' <param name="height"> true if the height preference has changed </param>
			Public Overrides Sub preferenceChanged(ByVal child As View, ByVal width As Boolean, ByVal height As Boolean)
				host.revalidate()
				host.repaint()
			End Sub

			''' <summary>
			''' Determines the desired alignment for this view along an axis.
			''' </summary>
			''' <param name="axis"> may be either X_AXIS or Y_AXIS </param>
			''' <returns> the desired alignment, where 0.0 indicates the origin
			'''     and 1.0 the full span away from the origin </returns>
			Public Overrides Function getAlignment(ByVal axis As Integer) As Single
				Return view.getAlignment(axis)
			End Function

			''' <summary>
			''' Renders the view.
			''' </summary>
			''' <param name="g"> the graphics context </param>
			''' <param name="allocation"> the region to render into </param>
			Public Overrides Sub paint(ByVal g As Graphics, ByVal allocation As Shape)
				Dim alloc As Rectangle = allocation.bounds
				view.sizeize(alloc.width, alloc.height)
				view.paint(g, allocation)
			End Sub

			''' <summary>
			''' Sets the view parent.
			''' </summary>
			''' <param name="parent"> the parent view </param>
			Public Overrides Property parent As View
				Set(ByVal parent As View)
					Throw New Exception("Can't set parent on root view")
				End Set
			End Property

			''' <summary>
			''' Returns the number of views in this view.  Since
			''' this view simply wraps the root of the view hierarchy
			''' it has exactly one child.
			''' </summary>
			''' <returns> the number of views </returns>
			''' <seealso cref= #getView </seealso>
			Public Property Overrides viewCount As Integer
				Get
					Return 1
				End Get
			End Property

			''' <summary>
			''' Gets the n-th view in this container.
			''' </summary>
			''' <param name="n"> the number of the view to get </param>
			''' <returns> the view </returns>
			Public Overrides Function getView(ByVal n As Integer) As View
				Return view
			End Function

			''' <summary>
			''' Provides a mapping from the document model coordinate space
			''' to the coordinate space of the view mapped to it.
			''' </summary>
			''' <param name="pos"> the position to convert </param>
			''' <param name="a"> the allocated region to render into </param>
			''' <returns> the bounding box of the given position </returns>
			Public Overrides Function modelToView(ByVal pos As Integer, ByVal a As Shape, ByVal b As Position.Bias) As Shape
				Return view.modelToView(pos, a, b)
			End Function

			''' <summary>
			''' Provides a mapping from the document model coordinate space
			''' to the coordinate space of the view mapped to it.
			''' </summary>
			''' <param name="p0"> the position to convert >= 0 </param>
			''' <param name="b0"> the bias toward the previous character or the
			'''  next character represented by p0, in case the
			'''  position is a boundary of two views. </param>
			''' <param name="p1"> the position to convert >= 0 </param>
			''' <param name="b1"> the bias toward the previous character or the
			'''  next character represented by p1, in case the
			'''  position is a boundary of two views. </param>
			''' <param name="a"> the allocated region to render into </param>
			''' <returns> the bounding box of the given position is returned </returns>
			''' <exception cref="BadLocationException">  if the given position does
			'''   not represent a valid location in the associated document </exception>
			''' <exception cref="IllegalArgumentException"> for an invalid bias argument </exception>
			''' <seealso cref= View#viewToModel </seealso>
			Public Overrides Function modelToView(ByVal p0 As Integer, ByVal b0 As Position.Bias, ByVal p1 As Integer, ByVal b1 As Position.Bias, ByVal a As Shape) As Shape
				Return view.modelToView(p0, b0, p1, b1, a)
			End Function

			''' <summary>
			''' Provides a mapping from the view coordinate space to the logical
			''' coordinate space of the model.
			''' </summary>
			''' <param name="x"> x coordinate of the view location to convert </param>
			''' <param name="y"> y coordinate of the view location to convert </param>
			''' <param name="a"> the allocated region to render into </param>
			''' <returns> the location within the model that best represents the
			'''    given point in the view </returns>
			Public Overrides Function viewToModel(ByVal x As Single, ByVal y As Single, ByVal a As Shape, ByVal bias As Position.Bias()) As Integer
				Return view.viewToModel(x, y, a, bias)
			End Function

			''' <summary>
			''' Returns the document model underlying the view.
			''' </summary>
			''' <returns> the model </returns>
			Public Property Overrides document As Document
				Get
					Return view.document
				End Get
			End Property

			''' <summary>
			''' Returns the starting offset into the model for this view.
			''' </summary>
			''' <returns> the starting offset </returns>
			Public Property Overrides startOffset As Integer
				Get
					Return view.startOffset
				End Get
			End Property

			''' <summary>
			''' Returns the ending offset into the model for this view.
			''' </summary>
			''' <returns> the ending offset </returns>
			Public Property Overrides endOffset As Integer
				Get
					Return view.endOffset
				End Get
			End Property

			''' <summary>
			''' Gets the element that this view is mapped to.
			''' </summary>
			''' <returns> the view </returns>
			Public Property Overrides element As Element
				Get
					Return view.element
				End Get
			End Property

			''' <summary>
			''' Sets the view size.
			''' </summary>
			''' <param name="width"> the width </param>
			''' <param name="height"> the height </param>
			Public Overrides Sub setSize(ByVal width As Single, ByVal height As Single)
				Me.width = CInt(Fix(width))
				view.sizeize(width, height)
			End Sub

			''' <summary>
			''' Fetches the container hosting the view.  This is useful for
			''' things like scheduling a repaint, finding out the host
			''' components font, etc.  The default implementation
			''' of this is to forward the query to the parent view.
			''' </summary>
			''' <returns> the container </returns>
			Public Property Overrides container As Container
				Get
					Return host
				End Get
			End Property

			''' <summary>
			''' Fetches the factory to be used for building the
			''' various view fragments that make up the view that
			''' represents the model.  This is what determines
			''' how the model will be represented.  This is implemented
			''' to fetch the factory provided by the associated
			''' EditorKit.
			''' </summary>
			''' <returns> the factory </returns>
			Public Property Overrides viewFactory As ViewFactory
				Get
					Return factory
				End Get
			End Property

			Private width As Integer
			Private view As View
			Private factory As ViewFactory
			Private host As JComponent

		End Class
	End Class

End Namespace