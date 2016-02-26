Imports System
Imports javax.swing
Imports javax.swing.text
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
Namespace javax.swing.text.html

	''' <summary>
	''' View of an Image, intended to support the HTML &lt;IMG&gt; tag.
	''' Supports scaling via the HEIGHT and WIDTH attributes of the tag.
	''' If the image is unable to be loaded any text specified via the
	''' <code>ALT</code> attribute will be rendered.
	''' <p>
	''' While this class has been part of swing for a while now, it is public
	''' as of 1.4.
	''' 
	''' @author  Scott Violet </summary>
	''' <seealso cref= IconView
	''' @since 1.4 </seealso>
	Public Class ImageView
		Inherits View

		''' <summary>
		''' If true, when some of the bits are available a repaint is done.
		''' <p>
		''' This is set to false as swing does not offer a repaint that takes a
		''' delay. If this were true, a bunch of immediate repaints would get
		''' generated that end up significantly delaying the loading of the image
		''' (or anything else going on for that matter).
		''' </summary>
		Private Shared sIsInc As Boolean = False
		''' <summary>
		''' Repaint delay when some of the bits are available.
		''' </summary>
		Private Shared sIncRate As Integer = 100
		''' <summary>
		''' Property name for pending image icon
		''' </summary>
		Private Const PENDING_IMAGE As String = "html.pendingImage"
		''' <summary>
		''' Property name for missing image icon
		''' </summary>
		Private Const MISSING_IMAGE As String = "html.missingImage"

		''' <summary>
		''' Document property for image cache.
		''' </summary>
		Private Const IMAGE_CACHE_PROPERTY As String = "imageCache"

		' Height/width to use before we know the real size, these should at least
		' the size of <code>sMissingImageIcon</code> and
		' <code>sPendingImageIcon</code>
		Private Const DEFAULT_WIDTH As Integer = 38
		Private Const DEFAULT_HEIGHT As Integer= 38

		''' <summary>
		''' Default border to use if one is not specified.
		''' </summary>
		Private Const DEFAULT_BORDER As Integer = 2

		' Bitmask values
		Private Const LOADING_FLAG As Integer = 1
		Private Const LINK_FLAG As Integer = 2
		Private Const WIDTH_FLAG As Integer = 4
		Private Const HEIGHT_FLAG As Integer = 8
		Private Const RELOAD_FLAG As Integer = 16
		Private Const RELOAD_IMAGE_FLAG As Integer = 32
		Private Const SYNC_LOAD_FLAG As Integer = 64

		Private attr As AttributeSet
		Private image As Image
		Private disabledImage As Image
		Private width As Integer
		Private height As Integer
		''' <summary>
		''' Bitmask containing some of the above bitmask values. Because the
		''' image loading notification can happen on another thread access to
		''' this is synchronized (at least for modifying it). 
		''' </summary>
		Private state As Integer
		Private container As Container
		Private fBounds As Rectangle
		Private borderColor As Color
		' Size of the border, the insets contains this valid. For example, if
		' the HSPACE attribute was 4 and BORDER 2, leftInset would be 6.
		Private borderSize As Short
		' Insets, obtained from the painter.
		Private leftInset As Short
		Private rightInset As Short
		Private topInset As Short
		Private bottomInset As Short
		''' <summary>
		''' We don't directly implement ImageObserver, instead we use an instance
		''' that calls back to us.
		''' </summary>
		Private imageObserver As java.awt.image.ImageObserver
		''' <summary>
		''' Used for alt text. Will be non-null if the image couldn't be found,
		''' and there is valid alt text.
		''' </summary>
		Private altView As View
		''' <summary>
		''' Alignment along the vertical (Y) axis. </summary>
		Private vAlign As Single



		''' <summary>
		''' Creates a new view that represents an IMG element.
		''' </summary>
		''' <param name="elem"> the element to create a view for </param>
		Public Sub New(ByVal elem As Element)
			MyBase.New(elem)
			fBounds = New Rectangle
			imageObserver = New ImageHandler(Me)
			state = RELOAD_FLAG Or RELOAD_IMAGE_FLAG
		End Sub

		''' <summary>
		''' Returns the text to display if the image can't be loaded. This is
		''' obtained from the Elements attribute set with the attribute name
		''' <code>HTML.Attribute.ALT</code>.
		''' </summary>
		Public Overridable Property altText As String
			Get
				Return CStr(element.attributes.getAttribute(HTML.Attribute.ALT))
			End Get
		End Property

		''' <summary>
		''' Return a URL for the image source,
		''' or null if it could not be determined.
		''' </summary>
		Public Overridable Property imageURL As URL
			Get
				Dim src As String = CStr(element.attributes.getAttribute(HTML.Attribute.SRC))
				If src Is Nothing Then Return Nothing
    
				Dim reference As URL = CType(document, HTMLDocument).base
				Try
					Dim u As New URL(reference,src)
					Return u
				Catch e As MalformedURLException
					Return Nothing
				End Try
			End Get
		End Property

		''' <summary>
		''' Returns the icon to use if the image couldn't be found.
		''' </summary>
		Public Overridable Property noImageIcon As Icon
			Get
				Return CType(UIManager.lookAndFeelDefaults(MISSING_IMAGE), Icon)
			End Get
		End Property

		''' <summary>
		''' Returns the icon to use while in the process of loading the image.
		''' </summary>
		Public Overridable Property loadingImageIcon As Icon
			Get
				Return CType(UIManager.lookAndFeelDefaults(PENDING_IMAGE), Icon)
			End Get
		End Property

		''' <summary>
		''' Returns the image to render.
		''' </summary>
		Public Overridable Property image As Image
			Get
				sync()
				Return image
			End Get
		End Property

		Private Function getImage(ByVal enabled As Boolean) As Image
			Dim img As Image = image
			If Not enabled Then
				If disabledImage Is Nothing Then disabledImage = GrayFilter.createDisabledImage(img)
				img = disabledImage
			End If
			Return img
		End Function

		''' <summary>
		''' Sets how the image is loaded. If <code>newValue</code> is true,
		''' the image we be loaded when first asked for, otherwise it will
		''' be loaded asynchronously. The default is to not load synchronously,
		''' that is to load the image asynchronously.
		''' </summary>
		Public Overridable Property loadsSynchronously As Boolean
			Set(ByVal newValue As Boolean)
				SyncLock Me
					If newValue Then
						state = state Or SYNC_LOAD_FLAG
					Else
						state = (state Or SYNC_LOAD_FLAG) Xor SYNC_LOAD_FLAG
					End If
				End SyncLock
			End Set
			Get
				Return ((state And SYNC_LOAD_FLAG) <> 0)
			End Get
		End Property


		''' <summary>
		''' Convenience method to get the StyleSheet.
		''' </summary>
		Protected Friend Overridable Property styleSheet As StyleSheet
			Get
				Dim doc As HTMLDocument = CType(document, HTMLDocument)
				Return doc.styleSheet
			End Get
		End Property

		''' <summary>
		''' Fetches the attributes to use when rendering.  This is
		''' implemented to multiplex the attributes specified in the
		''' model with a StyleSheet.
		''' </summary>
		Public Property Overrides attributes As AttributeSet
			Get
				sync()
				Return attr
			End Get
		End Property

		''' <summary>
		''' For images the tooltip text comes from text specified with the
		''' <code>ALT</code> attribute. This is overriden to return
		''' <code>getAltText</code>.
		''' </summary>
		''' <seealso cref= JTextComponent#getToolTipText </seealso>
		Public Overrides Function getToolTipText(ByVal x As Single, ByVal y As Single, ByVal allocation As Shape) As String
			Return altText
		End Function

		''' <summary>
		''' Update any cached values that come from attributes.
		''' </summary>
		Protected Friend Overridable Sub setPropertiesFromAttributes()
			Dim sheet As StyleSheet = styleSheet
			Me.attr = sheet.getViewAttributes(Me)

			' Gutters
			borderSize = CShort(getIntAttr(HTML.Attribute.BORDER,If(link, DEFAULT_BORDER, 0)))

				rightInset = CShort(getIntAttr(HTML.Attribute.HSPACE, 0) + borderSize)
				leftInset = rightInset
				bottomInset = CShort(getIntAttr(HTML.Attribute.VSPACE, 0) + borderSize)
				topInset = bottomInset

			borderColor = CType(document, StyledDocument).getForeground(attributes)

			Dim attr As AttributeSet = element.attributes

			' Alignment.
			' PENDING: This needs to be changed to support the CSS versions
			' when conversion from ALIGN to VERTICAL_ALIGN is complete.
			Dim ___alignment As Object = attr.getAttribute(HTML.Attribute.ALIGN)

			vAlign = 1.0f
			If ___alignment IsNot Nothing Then
				___alignment = ___alignment.ToString()
				If "top".Equals(___alignment) Then
					vAlign = 0f
				ElseIf "middle".Equals(___alignment) Then
					vAlign =.5f
				End If
			End If

			Dim anchorAttr As AttributeSet = CType(attr.getAttribute(HTML.Tag.A), AttributeSet)
			If anchorAttr IsNot Nothing AndAlso anchorAttr.isDefined(HTML.Attribute.HREF) Then
				SyncLock Me
					state = state Or LINK_FLAG
				End SyncLock
			Else
				SyncLock Me
					state = (state Or LINK_FLAG) Xor LINK_FLAG
				End SyncLock
			End If
		End Sub

		''' <summary>
		''' Establishes the parent view for this view.
		''' Seize this moment to cache the AWT Container I'm in.
		''' </summary>
		Public Overrides Property parent As View
			Set(ByVal parent As View)
				Dim oldParent As View = parent
				MyBase.parent = parent
				container = If(parent IsNot Nothing, container, Nothing)
				If oldParent IsNot parent Then
					SyncLock Me
						state = state Or RELOAD_FLAG
					End SyncLock
				End If
			End Set
		End Property

		''' <summary>
		''' Invoked when the Elements attributes have changed. Recreates the image.
		''' </summary>
		Public Overrides Sub changedUpdate(ByVal e As DocumentEvent, ByVal a As Shape, ByVal f As ViewFactory)
			MyBase.changedUpdate(e,a,f)

			SyncLock Me
				state = state Or RELOAD_FLAG Or RELOAD_IMAGE_FLAG
			End SyncLock

			' Assume the worst.
			preferenceChanged(Nothing, True, True)
		End Sub

		''' <summary>
		''' Paints the View.
		''' </summary>
		''' <param name="g"> the rendering surface to use </param>
		''' <param name="a"> the allocated region to render into </param>
		''' <seealso cref= View#paint </seealso>
		Public Overrides Sub paint(ByVal g As Graphics, ByVal a As Shape)
			sync()

			Dim rect As Rectangle = If(TypeOf a Is Rectangle, CType(a, Rectangle), a.bounds)
			Dim clip As Rectangle = g.clipBounds

			fBounds.bounds = rect
			paintHighlights(g, a)
			paintBorder(g, rect)
			If clip IsNot Nothing Then g.clipRect(rect.x + leftInset, rect.y + topInset, rect.width - leftInset - rightInset, rect.height - topInset - bottomInset)

			Dim host As Container = container
			Dim img As Image = getImage(host Is Nothing OrElse host.enabled)
			If img IsNot Nothing Then
				If Not hasPixels(img) Then
					' No pixels yet, use the default
					Dim icon As Icon = loadingImageIcon
					If icon IsNot Nothing Then icon.paintIcon(host, g, rect.x + leftInset, rect.y + topInset)
				Else
					' Draw the image
					g.drawImage(img, rect.x + leftInset, rect.y + topInset, width, height, imageObserver)
				End If
			Else
				Dim icon As Icon = noImageIcon
				If icon IsNot Nothing Then icon.paintIcon(host, g, rect.x + leftInset, rect.y + topInset)
				Dim ___view As View = altView
				' Paint the view representing the alt text, if its non-null
				If ___view IsNot Nothing AndAlso ((state And WIDTH_FLAG) = 0 OrElse width > DEFAULT_WIDTH) Then
					' Assume layout along the y direction
					Dim altRect As New Rectangle(rect.x + leftInset + DEFAULT_WIDTH, rect.y + topInset, rect.width - leftInset - rightInset - DEFAULT_WIDTH, rect.height - topInset - bottomInset)

					___view.paint(g, altRect)
				End If
			End If
			If clip IsNot Nothing Then g.cliplip(clip.x, clip.y, clip.width, clip.height)
		End Sub

		Private Sub paintHighlights(ByVal g As Graphics, ByVal shape As Shape)
			If TypeOf container Is JTextComponent Then
				Dim tc As JTextComponent = CType(container, JTextComponent)
				Dim h As Highlighter = tc.highlighter
				If TypeOf h Is LayeredHighlighter Then CType(h, LayeredHighlighter).paintLayeredHighlights(g, startOffset, endOffset, shape, tc, Me)
			End If
		End Sub

		Private Sub paintBorder(ByVal g As Graphics, ByVal rect As Rectangle)
			Dim color As Color = borderColor

			If (borderSize > 0 OrElse image Is Nothing) AndAlso color IsNot Nothing Then
				Dim xOffset As Integer = leftInset - borderSize
				Dim yOffset As Integer = topInset - borderSize
				g.color = color
				Dim n As Integer = If(image Is Nothing, 1, borderSize)
				For counter As Integer = 0 To n - 1
					g.drawRect(rect.x + xOffset + counter, rect.y + yOffset + counter, rect.width - counter - counter - xOffset -xOffset-1, rect.height - counter - counter -yOffset-yOffset-1)
				Next counter
			End If
		End Sub

		''' <summary>
		''' Determines the preferred span for this view along an
		''' axis.
		''' </summary>
		''' <param name="axis"> may be either X_AXIS or Y_AXIS </param>
		''' <returns>   the span the view would like to be rendered into;
		'''           typically the view is told to render into the span
		'''           that is returned, although there is no guarantee;
		'''           the parent may choose to resize or break the view </returns>
		Public Overrides Function getPreferredSpan(ByVal axis As Integer) As Single
			sync()

			' If the attributes specified a width/height, always use it!
			If axis = View.X_AXIS AndAlso (state And WIDTH_FLAG) = WIDTH_FLAG Then
				getPreferredSpanFromAltView(axis)
				Return width + leftInset + rightInset
			End If
			If axis = View.Y_AXIS AndAlso (state And HEIGHT_FLAG) = HEIGHT_FLAG Then
				getPreferredSpanFromAltView(axis)
				Return height + topInset + bottomInset
			End If

			Dim ___image As Image = image

			If ___image IsNot Nothing Then
				Select Case axis
				Case View.X_AXIS
					Return width + leftInset + rightInset
				Case View.Y_AXIS
					Return height + topInset + bottomInset
				Case Else
					Throw New System.ArgumentException("Invalid axis: " & axis)
				End Select
			Else
				Dim ___view As View = altView
				Dim retValue As Single = 0f

				If ___view IsNot Nothing Then retValue = ___view.getPreferredSpan(axis)
				Select Case axis
				Case View.X_AXIS
					Return retValue + CSng(width + leftInset + rightInset)
				Case View.Y_AXIS
					Return retValue + CSng(height + topInset + bottomInset)
				Case Else
					Throw New System.ArgumentException("Invalid axis: " & axis)
				End Select
			End If
		End Function

		''' <summary>
		''' Determines the desired alignment for this view along an
		''' axis.  This is implemented to give the alignment to the
		''' bottom of the icon along the y axis, and the default
		''' along the x axis.
		''' </summary>
		''' <param name="axis"> may be either X_AXIS or Y_AXIS </param>
		''' <returns> the desired alignment; this should be a value
		'''   between 0.0 and 1.0 where 0 indicates alignment at the
		'''   origin and 1.0 indicates alignment to the full span
		'''   away from the origin; an alignment of 0.5 would be the
		'''   center of the view </returns>
		Public Overrides Function getAlignment(ByVal axis As Integer) As Single
			Select Case axis
			Case View.Y_AXIS
				Return vAlign
			Case Else
				Return MyBase.getAlignment(axis)
			End Select
		End Function

		''' <summary>
		''' Provides a mapping from the document model coordinate space
		''' to the coordinate space of the view mapped to it.
		''' </summary>
		''' <param name="pos"> the position to convert </param>
		''' <param name="a"> the allocated region to render into </param>
		''' <returns> the bounding box of the given position </returns>
		''' <exception cref="BadLocationException">  if the given position does not represent a
		'''   valid location in the associated document </exception>
		''' <seealso cref= View#modelToView </seealso>
		Public Overrides Function modelToView(ByVal pos As Integer, ByVal a As Shape, ByVal b As Position.Bias) As Shape
			Dim p0 As Integer = startOffset
			Dim p1 As Integer = endOffset
			If (pos >= p0) AndAlso (pos <= p1) Then
				Dim r As Rectangle = a.bounds
				If pos = p1 Then r.x += r.width
				r.width = 0
				Return r
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Provides a mapping from the view coordinate space to the logical
		''' coordinate space of the model.
		''' </summary>
		''' <param name="x"> the X coordinate </param>
		''' <param name="y"> the Y coordinate </param>
		''' <param name="a"> the allocated region to render into </param>
		''' <returns> the location within the model that best represents the
		'''  given point of view </returns>
		''' <seealso cref= View#viewToModel </seealso>
		Public Overrides Function viewToModel(ByVal x As Single, ByVal y As Single, ByVal a As Shape, ByVal bias As Position.Bias()) As Integer
			Dim alloc As Rectangle = CType(a, Rectangle)
			If x < alloc.x + alloc.width Then
				bias(0) = Position.Bias.Forward
				Return startOffset
			End If
			bias(0) = Position.Bias.Backward
			Return endOffset
		End Function

		''' <summary>
		''' Sets the size of the view.  This should cause
		''' layout of the view if it has any layout duties.
		''' </summary>
		''' <param name="width"> the width &gt;= 0 </param>
		''' <param name="height"> the height &gt;= 0 </param>
		Public Overrides Sub setSize(ByVal width As Single, ByVal height As Single)
			sync()

			If image Is Nothing Then
				Dim ___view As View = altView

				If ___view IsNot Nothing Then ___view.sizeize(Math.Max(0f, width - CSng(DEFAULT_WIDTH + leftInset + rightInset)), Math.Max(0f, height - CSng(topInset + bottomInset)))
			End If
		End Sub

		''' <summary>
		''' Returns true if this image within a link?
		''' </summary>
		Private Property link As Boolean
			Get
				Return ((state And LINK_FLAG) = LINK_FLAG)
			End Get
		End Property

		''' <summary>
		''' Returns true if the passed in image has a non-zero width and height.
		''' </summary>
		Private Function hasPixels(ByVal image As Image) As Boolean
			Return image IsNot Nothing AndAlso (image.getHeight(imageObserver) > 0) AndAlso (image.getWidth(imageObserver) > 0)
		End Function

		''' <summary>
		''' Returns the preferred span of the View used to display the alt text,
		''' or 0 if the view does not exist.
		''' </summary>
		Private Function getPreferredSpanFromAltView(ByVal axis As Integer) As Single
			If image Is Nothing Then
				Dim ___view As View = altView

				If ___view IsNot Nothing Then Return ___view.getPreferredSpan(axis)
			End If
			Return 0f
		End Function

		''' <summary>
		''' Request that this view be repainted.
		''' Assumes the view is still at its last-drawn location.
		''' </summary>
		Private Sub repaint(ByVal delay As Long)
			If container IsNot Nothing AndAlso fBounds IsNot Nothing Then container.repaint(delay, fBounds.x, fBounds.y, fBounds.width, fBounds.height)
		End Sub

		''' <summary>
		''' Convenience method for getting an integer attribute from the elements
		''' AttributeSet.
		''' </summary>
		Private Function getIntAttr(ByVal name As HTML.Attribute, ByVal deflt As Integer) As Integer
			Dim attr As AttributeSet = element.attributes
			If attr.isDefined(name) Then ' does not check parents!
				Dim i As Integer
				Dim val As String = CStr(attr.getAttribute(name))
				If val Is Nothing Then
					i = deflt
				Else
					Try
						i = Math.Max(0, Convert.ToInt32(val))
					Catch x As NumberFormatException
						i = deflt
					End Try
				End If
				Return i
			Else
				Return deflt
			End If
		End Function

		''' <summary>
		''' Makes sure the necessary properties and image is loaded.
		''' </summary>
		Private Sub sync()
			Dim s As Integer = state
			If (s And RELOAD_IMAGE_FLAG) <> 0 Then refreshImage()
			s = state
			If (s And RELOAD_FLAG) <> 0 Then
				SyncLock Me
					state = (state Or RELOAD_FLAG) Xor RELOAD_FLAG
				End SyncLock
				propertiesFromAttributestes()
			End If
		End Sub

		''' <summary>
		''' Loads the image and updates the size accordingly. This should be
		''' invoked instead of invoking <code>loadImage</code> or
		''' <code>updateImageSize</code> directly.
		''' </summary>
		Private Sub refreshImage()
			SyncLock Me
				' clear out width/height/realoadimage flag and set loading flag
				state = (state Or LOADING_FLAG Or RELOAD_IMAGE_FLAG Or WIDTH_FLAG Or HEIGHT_FLAG) Xor (WIDTH_FLAG Or HEIGHT_FLAG Or RELOAD_IMAGE_FLAG)
				image = Nothing
					height = 0
					width = height
			End SyncLock

			Try
				' Load the image
				loadImage()

				' And update the size params
				updateImageSize()
			Finally
				SyncLock Me
					' Clear out state in case someone threw an exception.
					state = (state Or LOADING_FLAG) Xor LOADING_FLAG
				End SyncLock
			End Try
		End Sub

		''' <summary>
		''' Loads the image from the URL <code>getImageURL</code>. This should
		''' only be invoked from <code>refreshImage</code>.
		''' </summary>
		Private Sub loadImage()
			Dim src As URL = imageURL
			Dim newImage As Image = Nothing
			If src IsNot Nothing Then
				Dim cache As java.util.Dictionary = CType(document.getProperty(IMAGE_CACHE_PROPERTY), java.util.Dictionary)
				If cache IsNot Nothing Then
					newImage = CType(cache.get(src), Image)
				Else
					newImage = Toolkit.defaultToolkit.createImage(src)
					If newImage IsNot Nothing AndAlso loadsSynchronously Then
						' Force the image to be loaded by using an ImageIcon.
						Dim ii As New ImageIcon
						ii.image = newImage
					End If
				End If
			End If
			image = newImage
		End Sub

		''' <summary>
		''' Recreates and reloads the image.  This should
		''' only be invoked from <code>refreshImage</code>.
		''' </summary>
		Private Sub updateImageSize()
			Dim newWidth As Integer = 0
			Dim newHeight As Integer = 0
			Dim newState As Integer = 0
			Dim newImage As Image = image

			If newImage IsNot Nothing Then
				Dim elem As Element = element
				Dim attr As AttributeSet = elem.attributes

				' Get the width/height and set the state ivar before calling
				' anything that might cause the image to be loaded, and thus the
				' ImageHandler to be called.
				newWidth = getIntAttr(HTML.Attribute.WIDTH, -1)
				If newWidth > 0 Then newState = newState Or WIDTH_FLAG
				newHeight = getIntAttr(HTML.Attribute.HEIGHT, -1)
				If newHeight > 0 Then newState = newState Or HEIGHT_FLAG

				If newWidth <= 0 Then
					newWidth = newImage.getWidth(imageObserver)
					If newWidth <= 0 Then newWidth = DEFAULT_WIDTH
				End If

				If newHeight <= 0 Then
					newHeight = newImage.getHeight(imageObserver)
					If newHeight <= 0 Then newHeight = DEFAULT_HEIGHT
				End If

				' Make sure the image starts loading:
				If (newState And (WIDTH_FLAG Or HEIGHT_FLAG)) <> 0 Then
					Toolkit.defaultToolkit.prepareImage(newImage, newWidth, newHeight, imageObserver)
				Else
					Toolkit.defaultToolkit.prepareImage(newImage, -1, -1, imageObserver)
				End If

				Dim createText As Boolean = False
				SyncLock Me
					' If imageloading failed, other thread may have called
					' ImageLoader which will null out image, hence we check
					' for it.
					If image IsNot Nothing Then
						If (newState And WIDTH_FLAG) = WIDTH_FLAG OrElse width = 0 Then width = newWidth
						If (newState And HEIGHT_FLAG) = HEIGHT_FLAG OrElse height = 0 Then height = newHeight
					Else
						createText = True
						If (newState And WIDTH_FLAG) = WIDTH_FLAG Then width = newWidth
						If (newState And HEIGHT_FLAG) = HEIGHT_FLAG Then height = newHeight
					End If
					state = state Or newState
					state = (state Or LOADING_FLAG) Xor LOADING_FLAG
				End SyncLock
				If createText Then updateAltTextView()
			Else
					height = DEFAULT_HEIGHT
					width = height
				updateAltTextView()
			End If
		End Sub

		''' <summary>
		''' Updates the view representing the alt text.
		''' </summary>
		Private Sub updateAltTextView()
			Dim text As String = altText

			If text IsNot Nothing Then
				Dim newView As ImageLabelView

				newView = New ImageLabelView(Me, element, text)
				SyncLock Me
					altView = newView
				End SyncLock
			End If
		End Sub

		''' <summary>
		''' Returns the view to use for alternate text. This may be null.
		''' </summary>
		Private Property altView As View
			Get
				Dim ___view As View
    
				SyncLock Me
					___view = altView
				End SyncLock
				If ___view IsNot Nothing AndAlso ___view.parent Is Nothing Then ___view.parent = parent
				Return ___view
			End Get
		End Property

		''' <summary>
		''' Invokes <code>preferenceChanged</code> on the event displatching
		''' thread.
		''' </summary>
		Private Sub safePreferenceChanged()
			If SwingUtilities.eventDispatchThread Then
				Dim doc As Document = document
				If TypeOf doc Is AbstractDocument Then CType(doc, AbstractDocument).readLock()
				preferenceChanged(Nothing, True, True)
				If TypeOf doc Is AbstractDocument Then CType(doc, AbstractDocument).readUnlock()
			Else
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				SwingUtilities.invokeLater(New Runnable()
	'			{
	'					public void run()
	'					{
	'						safePreferenceChanged();
	'					}
	'				});
			End If
		End Sub

		''' <summary>
		''' ImageHandler implements the ImageObserver to correctly update the
		''' display as new parts of the image become available.
		''' </summary>
		Private Class ImageHandler
			Implements java.awt.image.ImageObserver

			Private ReadOnly outerInstance As ImageView

			Public Sub New(ByVal outerInstance As ImageView)
				Me.outerInstance = outerInstance
			End Sub

			' This can come on any thread. If we are in the process of reloading
			' the image and determining our state (loading == true) we don't fire
			' preference changed, or repaint, we just reset the fWidth/fHeight as
			' necessary and return. This is ok as we know when loading finishes
			' it will pick up the new height/width, if necessary.
			Public Overridable Function imageUpdate(ByVal img As Image, ByVal flags As Integer, ByVal x As Integer, ByVal y As Integer, ByVal newWidth As Integer, ByVal newHeight As Integer) As Boolean
				If img IsNot outerInstance.image AndAlso img IsNot outerInstance.disabledImage OrElse outerInstance.image Is Nothing OrElse outerInstance.parent Is Nothing Then Return False

				' Bail out if there was an error:
				If (flags And (ABORT Or ERROR)) <> 0 Then
					outerInstance.repaint(0)
					SyncLock ImageView.this
						If outerInstance.image Is img Then
							' Be sure image hasn't changed since we don't
							' initialy synchronize
							outerInstance.image = Nothing
							If (outerInstance.state And WIDTH_FLAG) <> WIDTH_FLAG Then outerInstance.width = DEFAULT_WIDTH
							If (outerInstance.state And HEIGHT_FLAG) <> HEIGHT_FLAG Then outerInstance.height = DEFAULT_HEIGHT
						Else
							outerInstance.disabledImage = Nothing
						End If
						If (outerInstance.state And LOADING_FLAG) = LOADING_FLAG Then Return False
					End SyncLock
					outerInstance.updateAltTextView()
					outerInstance.safePreferenceChanged()
					Return False
				End If

				If outerInstance.image Is img Then
					' Resize image if necessary:
					Dim changed As Short = 0
					If (flags And java.awt.image.ImageObserver.HEIGHT) <> 0 AndAlso (Not outerInstance.element.attributes.isDefined(HTML.Attribute.HEIGHT)) Then changed = changed Or 1
					If (flags And java.awt.image.ImageObserver.WIDTH) <> 0 AndAlso (Not outerInstance.element.attributes.isDefined(HTML.Attribute.WIDTH)) Then changed = changed Or 2

					SyncLock ImageView.this
						If (changed And 1) = 1 AndAlso (outerInstance.state And WIDTH_FLAG) = 0 Then outerInstance.width = newWidth
						If (changed And 2) = 2 AndAlso (outerInstance.state And HEIGHT_FLAG) = 0 Then outerInstance.height = newHeight
						If (outerInstance.state And LOADING_FLAG) = LOADING_FLAG Then Return True
					End SyncLock
					If changed <> 0 Then
						' May need to resize myself, asynchronously:
						outerInstance.safePreferenceChanged()
						Return True
					End If
				End If

				' Repaint when done or when new pixels arrive:
				If (flags And (FRAMEBITS Or ALLBITS)) <> 0 Then
					outerInstance.repaint(0)
				ElseIf (flags And SOMEBITS) <> 0 AndAlso sIsInc Then
					outerInstance.repaint(sIncRate)
				End If
				Return ((flags And ALLBITS) = 0)
			End Function
		End Class


		''' <summary>
		''' ImageLabelView is used if the image can't be loaded, and
		''' the attribute specified an alt attribute. It overriden a handle of
		''' methods as the text is hardcoded and does not come from the document.
		''' </summary>
		Private Class ImageLabelView
			Inherits InlineView

			Private ReadOnly outerInstance As ImageView

			Private segment As Segment
			Private fg As Color

			Friend Sub New(ByVal outerInstance As ImageView, ByVal e As Element, ByVal text As String)
					Me.outerInstance = outerInstance
				MyBase.New(e)
				reset(text)
			End Sub

			Public Overridable Sub reset(ByVal text As String)
				segment = New Segment(text.ToCharArray(), 0, text.Length)
			End Sub

			Public Overrides Sub paint(ByVal g As Graphics, ByVal a As Shape)
				' Don't use supers paint, otherwise selection will be wrong
				' as our start/end offsets are fake.
				Dim painter As GlyphPainter = glyphPainter

				If painter IsNot Nothing Then
					g.color = foreground
					painter.paint(Me, g, a, startOffset, endOffset)
				End If
			End Sub

			Public Overrides Function getText(ByVal p0 As Integer, ByVal p1 As Integer) As Segment
				If p0 < 0 OrElse p1 > segment.array.Length Then Throw New Exception("ImageLabelView: Stale view")
				segment.offset = p0
				segment.count = p1 - p0
				Return segment
			End Function

			Public Property Overrides startOffset As Integer
				Get
					Return 0
				End Get
			End Property

			Public Property Overrides endOffset As Integer
				Get
					Return segment.array.Length
				End Get
			End Property

			Public Overrides Function breakView(ByVal axis As Integer, ByVal p0 As Integer, ByVal pos As Single, ByVal len As Single) As View
				' Don't allow a break
				Return Me
			End Function

			Public Property Overrides foreground As Color
				Get
					Dim ___parent As View
					___parent = parent
					If fg Is Nothing AndAlso ___parent IsNot Nothing Then
						Dim doc As Document = document
						Dim attr As AttributeSet = ___parent.attributes
    
						If attr IsNot Nothing AndAlso (TypeOf doc Is StyledDocument) Then fg = CType(doc, StyledDocument).getForeground(attr)
					End If
					Return fg
				End Get
			End Property
		End Class
	End Class

End Namespace