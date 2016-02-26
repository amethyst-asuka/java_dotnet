Imports Microsoft.VisualBasic
Imports System
Imports javax.swing
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

Namespace javax.swing.plaf.basic


	''' <summary>
	''' A Basic L&amp;F implementation of ProgressBarUI.
	''' 
	''' @author Michael C. Albers
	''' @author Kathy Walrath
	''' </summary>
	Public Class BasicProgressBarUI
		Inherits ProgressBarUI

		Private cachedPercent As Integer
		Private cellLength, cellSpacing As Integer
		' The "selectionForeground" is the color of the text when it is painted
		' over a filled area of the progress bar. The "selectionBackground"
		' is for the text over the unfilled progress bar area.
		Private selectionForeground, selectionBackground As Color

		Private animator As Animator

		Protected Friend progressBar As JProgressBar
		Protected Friend changeListener As ChangeListener
		Private handler As Handler

		''' <summary>
		''' The current state of the indeterminate animation's cycle.
		''' 0, the initial value, means paint the first frame.
		''' When the progress bar is indeterminate and showing,
		''' the default animation thread updates this variable
		''' by invoking incrementAnimationIndex()
		''' every repaintInterval milliseconds.
		''' </summary>
		Private animationIndex As Integer = 0

		''' <summary>
		''' The number of frames per cycle. Under the default implementation,
		''' this depends on the cycleTime and repaintInterval.  It
		''' must be an even number for the default painting algorithm.  This
		''' value is set in the initIndeterminateValues method.
		''' </summary>
		Private numFrames As Integer '0 1|numFrames-1 ... numFrames/2

		''' <summary>
		''' Interval (in ms) between repaints of the indeterminate progress bar.
		''' The value of this method is set
		''' (every time the progress bar changes to indeterminate mode)
		''' using the
		''' "ProgressBar.repaintInterval" key in the defaults table.
		''' </summary>
		Private repaintInterval As Integer

		''' <summary>
		''' The number of milliseconds until the animation cycle repeats.
		''' The value of this method is set
		''' (every time the progress bar changes to indeterminate mode)
		''' using the
		''' "ProgressBar.cycleTime" key in the defaults table.
		''' </summary>
		Private cycleTime As Integer 'must be repaintInterval*2*aPositiveInteger

		'performance stuff
		Private Shared ADJUSTTIMER As Boolean = True 'makes a BIG difference;
												   'make this false for
												   'performance tests

		''' <summary>
		''' Used to hold the location and size of the bouncing box (returned
		''' by getBox) to be painted.
		''' 
		''' @since 1.5
		''' </summary>
		Protected Friend boxRect As Rectangle

		''' <summary>
		''' The rectangle to be updated the next time the
		''' animation thread calls repaint.  For bouncing-box
		''' animation this rect should include the union of
		''' the currently displayed box (which needs to be erased)
		''' and the box to be displayed next.
		''' This rectangle's values are set in
		''' the setAnimationIndex method.
		''' </summary>
		Private nextPaintRect As Rectangle

		'cache
		''' <summary>
		''' The component's painting area, not including the border. </summary>
		Private componentInnards As Rectangle 'the current painting area
		Private oldComponentInnards As Rectangle 'used to see if the size changed

		''' <summary>
		''' For bouncing-box animation, the change in position per frame. </summary>
		Private delta As Double = 0.0

		Private maxPosition As Integer = 0 'maximum X (horiz) or Y box location


		Public Shared Function createUI(ByVal x As JComponent) As ComponentUI
			Return New BasicProgressBarUI
		End Function

		Public Overridable Sub installUI(ByVal c As JComponent)
			progressBar = CType(c, JProgressBar)
			installDefaults()
			installListeners()
			If progressBar.indeterminate Then initIndeterminateValues()
		End Sub

		Public Overridable Sub uninstallUI(ByVal c As JComponent)
			If progressBar.indeterminate Then cleanUpIndeterminateValues()
			uninstallDefaults()
			uninstallListeners()
			progressBar = Nothing
		End Sub

		Protected Friend Overridable Sub installDefaults()
			LookAndFeel.installProperty(progressBar, "opaque", Boolean.TRUE)
			LookAndFeel.installBorder(progressBar,"ProgressBar.border")
			LookAndFeel.installColorsAndFont(progressBar, "ProgressBar.background", "ProgressBar.foreground", "ProgressBar.font")
			cellLength = UIManager.getInt("ProgressBar.cellLength")
			If cellLength = 0 Then cellLength = 1
			cellSpacing = UIManager.getInt("ProgressBar.cellSpacing")
			selectionForeground = UIManager.getColor("ProgressBar.selectionForeground")
			selectionBackground = UIManager.getColor("ProgressBar.selectionBackground")
		End Sub

		Protected Friend Overridable Sub uninstallDefaults()
			LookAndFeel.uninstallBorder(progressBar)
		End Sub

		Protected Friend Overridable Sub installListeners()
			'Listen for changes in the progress bar's data.
			changeListener = handler
			progressBar.addChangeListener(changeListener)

			'Listen for changes between determinate and indeterminate state.
			progressBar.addPropertyChangeListener(handler)
		End Sub

		Private Property handler As Handler
			Get
				If handler Is Nothing Then handler = New Handler(Me)
				Return handler
			End Get
		End Property

		''' <summary>
		''' Starts the animation thread, creating and initializing
		''' it if necessary. This method is invoked when an
		''' indeterminate progress bar should start animating.
		''' Reasons for this may include:
		''' <ul>
		'''    <li>The progress bar is determinate and becomes displayable
		'''    <li>The progress bar is displayable and becomes determinate
		'''    <li>The progress bar is displayable and determinate and this
		'''        UI is installed
		''' </ul>
		''' If you implement your own animation thread,
		''' you must override this method.
		''' 
		''' @since 1.4 </summary>
		''' <seealso cref= #stopAnimationTimer </seealso>
		Protected Friend Overridable Sub startAnimationTimer()
			If animator Is Nothing Then animator = New Animator(Me)

			animator.start(repaintInterval)
		End Sub

		''' <summary>
		''' Stops the animation thread.
		''' This method is invoked when the indeterminate
		''' animation should be stopped. Reasons for this may include:
		''' <ul>
		'''    <li>The progress bar changes to determinate
		'''    <li>The progress bar is no longer part of a displayable hierarchy
		'''    <li>This UI in uninstalled
		''' </ul>
		''' If you implement your own animation thread,
		''' you must override this method.
		''' 
		''' @since 1.4 </summary>
		''' <seealso cref= #startAnimationTimer </seealso>
		Protected Friend Overridable Sub stopAnimationTimer()
			If animator IsNot Nothing Then animator.stop()
		End Sub

		''' <summary>
		''' Removes all listeners installed by this object.
		''' </summary>
		Protected Friend Overridable Sub uninstallListeners()
			progressBar.removeChangeListener(changeListener)
			progressBar.removePropertyChangeListener(handler)
			handler = Nothing
		End Sub


		''' <summary>
		''' Returns the baseline.
		''' </summary>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		''' <seealso cref= javax.swing.JComponent#getBaseline(int, int)
		''' @since 1.6 </seealso>
		Public Overridable Function getBaseline(ByVal c As JComponent, ByVal width As Integer, ByVal height As Integer) As Integer
			MyBase.getBaseline(c, width, height)
			If progressBar.stringPainted AndAlso progressBar.orientation = JProgressBar.HORIZONTAL Then
				Dim metrics As FontMetrics = progressBar.getFontMetrics(progressBar.font)
				Dim insets As Insets = progressBar.insets
				Dim y As Integer = insets.top
				height = height - insets.top - insets.bottom
				Return y + (height + metrics.ascent - metrics.leading - metrics.descent) / 2
			End If
			Return -1
		End Function

		''' <summary>
		''' Returns an enum indicating how the baseline of the component
		''' changes as the size changes.
		''' </summary>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		''' <seealso cref= javax.swing.JComponent#getBaseline(int, int)
		''' @since 1.6 </seealso>
		Public Overridable Function getBaselineResizeBehavior(ByVal c As JComponent) As Component.BaselineResizeBehavior
			MyBase.getBaselineResizeBehavior(c)
			If progressBar.stringPainted AndAlso progressBar.orientation = JProgressBar.HORIZONTAL Then Return Component.BaselineResizeBehavior.CENTER_OFFSET
			Return Component.BaselineResizeBehavior.OTHER
		End Function

		' Many of the Basic*UI components have the following methods.
		' This component does not have these methods because *ProgressBarUI
		'  is not a compound component and does not accept input.
		'
		' protected void installComponents()
		' protected void uninstallComponents()
		' protected void installKeyboardActions()
		' protected void uninstallKeyboardActions()

		Protected Friend Overridable Property preferredInnerHorizontal As Dimension
			Get
				Dim horizDim As Dimension = CType(sun.swing.DefaultLookup.get(progressBar, Me, "ProgressBar.horizontalSize"), Dimension)
				If horizDim Is Nothing Then horizDim = New Dimension(146, 12)
				Return horizDim
			End Get
		End Property

		Protected Friend Overridable Property preferredInnerVertical As Dimension
			Get
				Dim vertDim As Dimension = CType(sun.swing.DefaultLookup.get(progressBar, Me, "ProgressBar.verticalSize"), Dimension)
				If vertDim Is Nothing Then vertDim = New Dimension(12, 146)
				Return vertDim
			End Get
		End Property

		''' <summary>
		''' The "selectionForeground" is the color of the text when it is painted
		''' over a filled area of the progress bar.
		''' </summary>
		Protected Friend Overridable Property selectionForeground As Color
			Get
				Return selectionForeground
			End Get
		End Property

		''' <summary>
		''' The "selectionBackground" is the color of the text when it is painted
		''' over an unfilled area of the progress bar.
		''' </summary>
		Protected Friend Overridable Property selectionBackground As Color
			Get
				Return selectionBackground
			End Get
		End Property

		Private Property cachedPercent As Integer
			Get
				Return cachedPercent
			End Get
			Set(ByVal cachedPercent As Integer)
				Me.cachedPercent = cachedPercent
			End Set
		End Property


		''' <summary>
		''' Returns the width (if HORIZONTAL) or height (if VERTICAL)
		''' of each of the individual cells/units to be rendered in the
		''' progress bar. However, for text rendering simplification and
		''' aesthetic considerations, this function will return 1 when
		''' the progress string is being rendered.
		''' </summary>
		''' <returns> the value representing the spacing between cells </returns>
		''' <seealso cref=    #setCellLength </seealso>
		''' <seealso cref=    JProgressBar#isStringPainted </seealso>
		Protected Friend Overridable Property cellLength As Integer
			Get
				If progressBar.stringPainted Then
					Return 1
				Else
					Return cellLength
				End If
			End Get
			Set(ByVal cellLen As Integer)
				Me.cellLength = cellLen
			End Set
		End Property


		''' <summary>
		''' Returns the spacing between each of the cells/units in the
		''' progress bar. However, for text rendering simplification and
		''' aesthetic considerations, this function will return 0 when
		''' the progress string is being rendered.
		''' </summary>
		''' <returns> the value representing the spacing between cells </returns>
		''' <seealso cref=    #setCellSpacing </seealso>
		''' <seealso cref=    JProgressBar#isStringPainted </seealso>
		Protected Friend Overridable Property cellSpacing As Integer
			Get
				If progressBar.stringPainted Then
					Return 0
				Else
					Return cellSpacing
				End If
			End Get
			Set(ByVal cellSpace As Integer)
				Me.cellSpacing = cellSpace
			End Set
		End Property


		''' <summary>
		''' This determines the amount of the progress bar that should be filled
		''' based on the percent done gathered from the model. This is a common
		''' operation so it was abstracted out. It assumes that your progress bar
		''' is linear. That is, if you are making a circular progress indicator,
		''' you will want to override this method.
		''' </summary>
		Protected Friend Overridable Function getAmountFull(ByVal b As Insets, ByVal width As Integer, ByVal height As Integer) As Integer
			Dim ___amountFull As Integer = 0
			Dim model As BoundedRangeModel = progressBar.model

			If (model.maximum - model.minimum) <> 0 Then
				If progressBar.orientation = JProgressBar.HORIZONTAL Then
					___amountFull = CInt(Fix(Math.Round(width * progressBar.percentComplete)))
				Else
					___amountFull = CInt(Fix(Math.Round(height * progressBar.percentComplete)))
				End If
			End If
			Return ___amountFull
		End Function

		''' <summary>
		''' Delegates painting to one of two methods:
		''' paintDeterminate or paintIndeterminate.
		''' </summary>
		Public Overridable Sub paint(ByVal g As Graphics, ByVal c As JComponent)
			If progressBar.indeterminate Then
				paintIndeterminate(g, c)
			Else
				paintDeterminate(g, c)
			End If
		End Sub

		''' <summary>
		''' Stores the position and size of
		''' the bouncing box that would be painted for the current animation index
		''' in <code>r</code> and returns <code>r</code>.
		''' Subclasses that add to the painting performed
		''' in this class's implementation of <code>paintIndeterminate</code> --
		''' to draw an outline around the bouncing box, for example --
		''' can use this method to get the location of the bouncing
		''' box that was just painted.
		''' By overriding this method,
		''' you have complete control over the size and position
		''' of the bouncing box,
		''' without having to reimplement <code>paintIndeterminate</code>.
		''' </summary>
		''' <param name="r">  the Rectangle instance to be modified;
		'''           may be <code>null</code> </param>
		''' <returns>   <code>null</code> if no box should be drawn;
		'''           otherwise, returns the passed-in rectangle
		'''           (if non-null)
		'''           or a new rectangle
		''' </returns>
		''' <seealso cref= #setAnimationIndex
		''' @since 1.4 </seealso>
		Protected Friend Overridable Function getBox(ByVal r As Rectangle) As Rectangle
			Dim currentFrame As Integer = animationIndex
			Dim middleFrame As Integer = numFrames\2

			If sizeChanged() OrElse delta = 0.0 OrElse maxPosition = 0.0 Then updateSizes()

			r = getGenericBox(r)

			If r Is Nothing Then Return Nothing
			If middleFrame <= 0 Then Return Nothing

			'assert currentFrame >= 0 && currentFrame < numFrames
			If progressBar.orientation = JProgressBar.HORIZONTAL Then
				If currentFrame < middleFrame Then
					r.x = componentInnards.x + CInt(Fix(Math.Round(delta * CDbl(currentFrame))))
				Else
					r.x = maxPosition - CInt(Fix(Math.Round(delta * (currentFrame - middleFrame))))
				End If 'VERTICAL indeterminate progress bar
			Else
				If currentFrame < middleFrame Then
					r.y = componentInnards.y + CInt(Fix(Math.Round(delta * currentFrame)))
				Else
					r.y = maxPosition - CInt(Fix(Math.Round(delta * (currentFrame - middleFrame))))
				End If
			End If
			Return r
		End Function

		''' <summary>
		''' Updates delta, max position.
		''' Assumes componentInnards is correct (e.g. call after sizeChanged()).
		''' </summary>
		Private Sub updateSizes()
			Dim length As Integer = 0

			If progressBar.orientation = JProgressBar.HORIZONTAL Then
				length = getBoxLength(componentInnards.width, componentInnards.height)
				maxPosition = componentInnards.x + componentInnards.width - length
 'VERTICAL progress bar
			Else
				length = getBoxLength(componentInnards.height, componentInnards.width)
				maxPosition = componentInnards.y + componentInnards.height - length
			End If

			'If we're doing bouncing-box animation, update delta.
			delta = 2.0 * CDbl(maxPosition)/CDbl(numFrames)
		End Sub

		''' <summary>
		''' Assumes that the component innards, max position, etc. are up-to-date.
		''' </summary>
		Private Function getGenericBox(ByVal r As Rectangle) As Rectangle
			If r Is Nothing Then r = New Rectangle

			If progressBar.orientation = JProgressBar.HORIZONTAL Then
				r.width = getBoxLength(componentInnards.width, componentInnards.height)
				If r.width < 0 Then
					r = Nothing
				Else
					r.height = componentInnards.height
					r.y = componentInnards.y
				End If
			  ' end of HORIZONTAL
 'VERTICAL progress bar
			Else
				r.height = getBoxLength(componentInnards.height, componentInnards.width)
				If r.height < 0 Then
					r = Nothing
				Else
					r.width = componentInnards.width
					r.x = componentInnards.x
				End If
			End If ' end of VERTICAL

			Return r
		End Function

		''' <summary>
		''' Returns the length
		''' of the "bouncing box" to be painted.
		''' This method is invoked by the
		''' default implementation of <code>paintIndeterminate</code>
		''' to get the width (if the progress bar is horizontal)
		''' or height (if vertical) of the box.
		''' For example:
		''' <blockquote>
		''' <pre>
		''' boxRect.width = getBoxLength(componentInnards.width,
		'''                             componentInnards.height);
		''' </pre>
		''' </blockquote>
		''' </summary>
		''' <param name="availableLength">  the amount of space available
		'''                         for the bouncing box to move in;
		'''                         for a horizontal progress bar,
		'''                         for example,
		'''                         this should be
		'''                         the inside width of the progress bar
		'''                         (the component width minus borders) </param>
		''' <param name="otherDimension">   for a horizontal progress bar, this should be
		'''                         the inside height of the progress bar; this
		'''                         value might be used to constrain or determine
		'''                         the return value
		''' </param>
		''' <returns> the size of the box dimension being determined;
		'''         must be no larger than <code>availableLength</code>
		''' </returns>
		''' <seealso cref= javax.swing.SwingUtilities#calculateInnerArea
		''' @since 1.5 </seealso>
		Protected Friend Overridable Function getBoxLength(ByVal availableLength As Integer, ByVal otherDimension As Integer) As Integer
			Return CInt(Fix(Math.Round(availableLength/6.0)))
		End Function

		''' <summary>
		''' All purpose paint method that should do the right thing for all
		''' linear bouncing-box progress bars.
		''' Override this if you are making another kind of
		''' progress bar.
		''' </summary>
		''' <seealso cref= #paintDeterminate
		''' 
		''' @since 1.4 </seealso>
		Protected Friend Overridable Sub paintIndeterminate(ByVal g As Graphics, ByVal c As JComponent)
			If Not(TypeOf g Is Graphics2D) Then Return

			Dim b As Insets = progressBar.insets ' area for border
			Dim barRectWidth As Integer = progressBar.width - (b.right + b.left)
			Dim barRectHeight As Integer = progressBar.height - (b.top + b.bottom)

			If barRectWidth <= 0 OrElse barRectHeight <= 0 Then Return

			Dim g2 As Graphics2D = CType(g, Graphics2D)

			' Paint the bouncing box.
			boxRect = getBox(boxRect)
			If boxRect IsNot Nothing Then
				g2.color = progressBar.foreground
				g2.fillRect(boxRect.x, boxRect.y, boxRect.width, boxRect.height)
			End If

			' Deal with possible text painting
			If progressBar.stringPainted Then
				If progressBar.orientation = JProgressBar.HORIZONTAL Then
					paintString(g2, b.left, b.top, barRectWidth, barRectHeight, boxRect.x, boxRect.width, b)
				Else
					paintString(g2, b.left, b.top, barRectWidth, barRectHeight, boxRect.y, boxRect.height, b)
				End If
			End If
		End Sub


		''' <summary>
		''' All purpose paint method that should do the right thing for almost
		''' all linear, determinate progress bars. By setting a few values in
		''' the defaults
		''' table, things should work just fine to paint your progress bar.
		''' Naturally, override this if you are making a circular or
		''' semi-circular progress bar.
		''' </summary>
		''' <seealso cref= #paintIndeterminate
		''' 
		''' @since 1.4 </seealso>
		Protected Friend Overridable Sub paintDeterminate(ByVal g As Graphics, ByVal c As JComponent)
			If Not(TypeOf g Is Graphics2D) Then Return

			Dim b As Insets = progressBar.insets ' area for border
			Dim barRectWidth As Integer = progressBar.width - (b.right + b.left)
			Dim barRectHeight As Integer = progressBar.height - (b.top + b.bottom)

			If barRectWidth <= 0 OrElse barRectHeight <= 0 Then Return

			Dim ___cellLength As Integer = cellLength
			Dim ___cellSpacing As Integer = cellSpacing
			' amount of progress to draw
			Dim ___amountFull As Integer = getAmountFull(b, barRectWidth, barRectHeight)

			Dim g2 As Graphics2D = CType(g, Graphics2D)
			g2.color = progressBar.foreground

			If progressBar.orientation = JProgressBar.HORIZONTAL Then
				' draw the cells
				If ___cellSpacing = 0 AndAlso ___amountFull > 0 Then
					' draw one big Rect because there is no space between cells
					g2.stroke = New BasicStroke(CSng(barRectHeight), BasicStroke.CAP_BUTT, BasicStroke.JOIN_BEVEL)
				Else
					' draw each individual cell
					g2.stroke = New BasicStroke(CSng(barRectHeight), BasicStroke.CAP_BUTT, BasicStroke.JOIN_BEVEL, 0.0f, New Single() { ___cellLength, ___cellSpacing }, 0.0f)
				End If

				If BasicGraphicsUtils.isLeftToRight(c) Then
					g2.drawLine(b.left, (barRectHeight\2) + b.top, ___amountFull + b.left, (barRectHeight\2) + b.top)
				Else
					g2.drawLine((barRectWidth + b.left), (barRectHeight\2) + b.top, barRectWidth + b.left - ___amountFull, (barRectHeight\2) + b.top)
				End If
 ' VERTICAL
			Else
				' draw the cells
				If ___cellSpacing = 0 AndAlso ___amountFull > 0 Then
					' draw one big Rect because there is no space between cells
					g2.stroke = New BasicStroke(CSng(barRectWidth), BasicStroke.CAP_BUTT, BasicStroke.JOIN_BEVEL)
				Else
					' draw each individual cell
					g2.stroke = New BasicStroke(CSng(barRectWidth), BasicStroke.CAP_BUTT, BasicStroke.JOIN_BEVEL, 0f, New Single() { ___cellLength, ___cellSpacing }, 0f)
				End If

				g2.drawLine(barRectWidth\2 + b.left, b.top + barRectHeight, barRectWidth\2 + b.left, b.top + barRectHeight - ___amountFull)
			End If

			' Deal with possible text painting
			If progressBar.stringPainted Then paintString(g, b.left, b.top, barRectWidth, barRectHeight, ___amountFull, b)
		End Sub


		Protected Friend Overridable Sub paintString(ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer, ByVal amountFull As Integer, ByVal b As Insets)
			If progressBar.orientation = JProgressBar.HORIZONTAL Then
				If BasicGraphicsUtils.isLeftToRight(progressBar) Then
					If progressBar.indeterminate Then
						boxRect = getBox(boxRect)
						paintString(g, x, y, width, height, boxRect.x, boxRect.width, b)
					Else
						paintString(g, x, y, width, height, x, amountFull, b)
					End If
				Else
					paintString(g, x, y, width, height, x + width - amountFull, amountFull, b)
				End If
			Else
				If progressBar.indeterminate Then
					boxRect = getBox(boxRect)
					paintString(g, x, y, width, height, boxRect.y, boxRect.height, b)
				Else
					paintString(g, x, y, width, height, y + height - amountFull, amountFull, b)
				End If
			End If
		End Sub

		''' <summary>
		''' Paints the progress string.
		''' </summary>
		''' <param name="g"> Graphics used for drawing. </param>
		''' <param name="x"> x location of bounding box </param>
		''' <param name="y"> y location of bounding box </param>
		''' <param name="width"> width of bounding box </param>
		''' <param name="height"> height of bounding box </param>
		''' <param name="fillStart"> start location, in x or y depending on orientation,
		'''        of the filled portion of the progress bar. </param>
		''' <param name="amountFull"> size of the fill region, either width or height
		'''        depending upon orientation. </param>
		''' <param name="b"> Insets of the progress bar. </param>
		Private Sub paintString(ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer, ByVal fillStart As Integer, ByVal amountFull As Integer, ByVal b As Insets)
			If Not(TypeOf g Is Graphics2D) Then Return

			Dim g2 As Graphics2D = CType(g, Graphics2D)
			Dim progressString As String = progressBar.string
			g2.font = progressBar.font
			Dim renderLocation As Point = getStringPlacement(g2, progressString, x, y, width, height)
			Dim oldClip As Rectangle = g2.clipBounds

			If progressBar.orientation = JProgressBar.HORIZONTAL Then
				g2.color = selectionBackground
				sun.swing.SwingUtilities2.drawString(progressBar, g2, progressString, renderLocation.x, renderLocation.y)
				g2.color = selectionForeground
				g2.clipRect(fillStart, y, amountFull, height)
				sun.swing.SwingUtilities2.drawString(progressBar, g2, progressString, renderLocation.x, renderLocation.y) ' VERTICAL
			Else
				g2.color = selectionBackground
				Dim rotate As java.awt.geom.AffineTransform = java.awt.geom.AffineTransform.getRotateInstance(Math.PI/2)
				g2.font = progressBar.font.deriveFont(rotate)
				renderLocation = getStringPlacement(g2, progressString, x, y, width, height)
				sun.swing.SwingUtilities2.drawString(progressBar, g2, progressString, renderLocation.x, renderLocation.y)
				g2.color = selectionForeground
				g2.clipRect(x, fillStart, width, amountFull)
				sun.swing.SwingUtilities2.drawString(progressBar, g2, progressString, renderLocation.x, renderLocation.y)
			End If
			g2.clip = oldClip
		End Sub


		''' <summary>
		''' Designate the place where the progress string will be painted.
		''' This implementation places it at the center of the progress
		''' bar (in both x and y). Override this if you want to right,
		''' left, top, or bottom align the progress string or if you need
		''' to nudge it around for any reason.
		''' </summary>
		Protected Friend Overridable Function getStringPlacement(ByVal g As Graphics, ByVal progressString As String, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer) As Point
			Dim fontSizer As FontMetrics = sun.swing.SwingUtilities2.getFontMetrics(progressBar, g, progressBar.font)
			Dim stringWidth As Integer = sun.swing.SwingUtilities2.stringWidth(progressBar, fontSizer, progressString)

			If progressBar.orientation = JProgressBar.HORIZONTAL Then
				Return New Point(x + Math.Round(width\2 - stringWidth\2), y + ((height + fontSizer.ascent - fontSizer.leading - fontSizer.descent) / 2)) ' VERTICAL
			Else
				Return New Point(x + ((width - fontSizer.ascent + fontSizer.leading + fontSizer.descent) / 2), y + Math.Round(height\2 - stringWidth\2))
			End If
		End Function


		Public Overridable Function getPreferredSize(ByVal c As JComponent) As Dimension
			Dim size As Dimension
			Dim border As Insets = progressBar.insets
			Dim fontSizer As FontMetrics = progressBar.getFontMetrics(progressBar.font)

			If progressBar.orientation = JProgressBar.HORIZONTAL Then
				size = New Dimension(preferredInnerHorizontal)
				' Ensure that the progress string will fit
				If progressBar.stringPainted Then
					' I'm doing this for completeness.
					Dim progString As String = progressBar.string
					Dim stringWidth As Integer = sun.swing.SwingUtilities2.stringWidth(progressBar, fontSizer, progString)
					If stringWidth > size.width Then size.width = stringWidth
					' This uses both Height and Descent to be sure that
					' there is more than enough room in the progress bar
					' for everything.
					' This does have a strange dependency on
					' getStringPlacememnt() in a funny way.
					Dim stringHeight As Integer = fontSizer.height + fontSizer.descent
					If stringHeight > size.height Then size.height = stringHeight
				End If
			Else
				size = New Dimension(preferredInnerVertical)
				' Ensure that the progress string will fit.
				If progressBar.stringPainted Then
					Dim progString As String = progressBar.string
					Dim stringHeight As Integer = fontSizer.height + fontSizer.descent
					If stringHeight > size.width Then size.width = stringHeight
					' This is also for completeness.
					Dim stringWidth As Integer = sun.swing.SwingUtilities2.stringWidth(progressBar, fontSizer, progString)
					If stringWidth > size.height Then size.height = stringWidth
				End If
			End If

			size.width += border.left + border.right
			size.height += border.top + border.bottom
			Return size
		End Function

		''' <summary>
		''' The Minimum size for this component is 10. The rationale here
		''' is that there should be at least one pixel per 10 percent.
		''' </summary>
		Public Overridable Function getMinimumSize(ByVal c As JComponent) As Dimension
			Dim pref As Dimension = getPreferredSize(progressBar)
			If progressBar.orientation = JProgressBar.HORIZONTAL Then
				pref.width = 10
			Else
				pref.height = 10
			End If
			Return pref
		End Function

		Public Overridable Function getMaximumSize(ByVal c As JComponent) As Dimension
			Dim pref As Dimension = getPreferredSize(progressBar)
			If progressBar.orientation = JProgressBar.HORIZONTAL Then
				pref.width = Short.MaxValue
			Else
				pref.height = Short.MaxValue
			End If
			Return pref
		End Function

		''' <summary>
		''' Gets the index of the current animation frame.
		''' 
		''' @since 1.4
		''' </summary>
		Protected Friend Overridable Property animationIndex As Integer
			Get
				Return animationIndex
			End Get
			Set(ByVal newValue As Integer)
				If animationIndex <> newValue Then
					If sizeChanged() Then
						animationIndex = newValue
						maxPosition = 0 'needs to be recalculated
						delta = 0.0 'needs to be recalculated
						progressBar.repaint()
						Return
					End If
    
					'Get the previous box drawn.
					nextPaintRect = getBox(nextPaintRect)
    
					'Update the frame number.
					animationIndex = newValue
    
					'Get the next box to draw.
					If nextPaintRect IsNot Nothing Then
						boxRect = getBox(boxRect)
						If boxRect IsNot Nothing Then nextPaintRect.add(boxRect)
					End If 'animationIndex == newValue
				Else
					Return
				End If
    
				If nextPaintRect IsNot Nothing Then
					progressBar.repaint(nextPaintRect)
				Else
					progressBar.repaint()
				End If
			End Set
		End Property

		''' <summary>
		''' Returns the number of frames for the complete animation loop
		''' used by an indeterminate JProgessBar. The progress chunk will go
		''' from one end to the other and back during the entire loop. This
		''' visual behavior may be changed by subclasses in other Look and Feels.
		''' </summary>
		''' <returns> the number of frames
		''' @since 1.6 </returns>
		Protected Friend Property frameCount As Integer
			Get
				Return numFrames
			End Get
		End Property


		Private Function sizeChanged() As Boolean
			If (oldComponentInnards Is Nothing) OrElse (componentInnards Is Nothing) Then Return True

			oldComponentInnards.rect = componentInnards
			componentInnards = SwingUtilities.calculateInnerArea(progressBar, componentInnards)
			Return Not oldComponentInnards.Equals(componentInnards)
		End Function

		''' <summary>
		''' Sets the index of the current animation frame,
		''' to the next valid value,
		''' which results in the progress bar being repainted.
		''' The next valid value is, by default,
		''' the current animation index plus one.
		''' If the new value would be too large,
		''' this method sets the index to 0.
		''' Subclasses might need to override this method
		''' to ensure that the index does not go over
		''' the number of frames needed for the particular
		''' progress bar instance.
		''' This method is invoked by the default animation thread
		''' every <em>X</em> milliseconds,
		''' where <em>X</em> is specified by the "ProgressBar.repaintInterval"
		''' UI default.
		''' </summary>
		''' <seealso cref= #setAnimationIndex
		''' @since 1.4 </seealso>
		Protected Friend Overridable Sub incrementAnimationIndex()
			Dim newValue As Integer = animationIndex + 1

			If newValue < numFrames Then
				animationIndex = newValue
			Else
				animationIndex = 0
			End If
		End Sub

		''' <summary>
		''' Returns the desired number of milliseconds between repaints.
		''' This value is meaningful
		''' only if the progress bar is in indeterminate mode.
		''' The repaint interval determines how often the
		''' default animation thread's timer is fired.
		''' It's also used by the default indeterminate progress bar
		''' painting code when determining
		''' how far to move the bouncing box per frame.
		''' The repaint interval is specified by
		''' the "ProgressBar.repaintInterval" UI default.
		''' </summary>
		''' <returns>  the repaint interval, in milliseconds </returns>
		Private Property repaintInterval As Integer
			Get
				Return repaintInterval
			End Get
		End Property

		Private Function initRepaintInterval() As Integer
			repaintInterval = sun.swing.DefaultLookup.getInt(progressBar, Me, "ProgressBar.repaintInterval", 50)
			Return repaintInterval
		End Function

		''' <summary>
		''' Returns the number of milliseconds per animation cycle.
		''' This value is meaningful
		''' only if the progress bar is in indeterminate mode.
		''' The cycle time is used by the default indeterminate progress bar
		''' painting code when determining
		''' how far to move the bouncing box per frame.
		''' The cycle time is specified by
		''' the "ProgressBar.cycleTime" UI default
		''' and adjusted, if necessary,
		''' by the initIndeterminateDefaults method.
		''' </summary>
		''' <returns>  the cycle time, in milliseconds </returns>
		Private Property cycleTime As Integer
			Get
				Return cycleTime
			End Get
		End Property

		Private Function initCycleTime() As Integer
			cycleTime = sun.swing.DefaultLookup.getInt(progressBar, Me, "ProgressBar.cycleTime", 3000)
			Return cycleTime
		End Function


		''' <summary>
		''' Initialize cycleTime, repaintInterval, numFrames, animationIndex. </summary>
		Private Sub initIndeterminateDefaults()
			initRepaintInterval() 'initialize repaint interval
			initCycleTime() 'initialize cycle length

			' Make sure repaintInterval is reasonable.
			If repaintInterval <= 0 Then repaintInterval = 100

			' Make sure cycleTime is reasonable.
			If repaintInterval > cycleTime Then
				cycleTime = repaintInterval * 20
			Else
				' Force cycleTime to be a even multiple of repaintInterval.
				Dim factor As Integer = CInt(Fix(Math.Ceiling((CDbl(cycleTime)) / (CDbl(repaintInterval)*2))))
				cycleTime = repaintInterval*factor*2
			End If
		End Sub

		''' <summary>
		''' Invoked by PropertyChangeHandler.
		''' 
		'''  NOTE: This might not be invoked until after the first
		'''  paintIndeterminate call.
		''' </summary>
		Private Sub initIndeterminateValues()
			initIndeterminateDefaults()
			'assert cycleTime/repaintInterval is a whole multiple of 2.
			numFrames = cycleTime\repaintInterval
			initAnimationIndex()

			boxRect = New Rectangle
			nextPaintRect = New Rectangle
			componentInnards = New Rectangle
			oldComponentInnards = New Rectangle

			' we only bother installing the HierarchyChangeListener if we
			' are indeterminate
			progressBar.addHierarchyListener(handler)

			' start the animation thread if necessary
			If progressBar.displayable Then startAnimationTimer()
		End Sub

		''' <summary>
		''' Invoked by PropertyChangeHandler. </summary>
		Private Sub cleanUpIndeterminateValues()
			' stop the animation thread if necessary
			If progressBar.displayable Then stopAnimationTimer()

				repaintInterval = 0
				cycleTime = repaintInterval
				animationIndex = 0
				numFrames = animationIndex
			maxPosition = 0
			delta = 0.0

				nextPaintRect = Nothing
				boxRect = nextPaintRect
				oldComponentInnards = Nothing
				componentInnards = oldComponentInnards

			progressBar.removeHierarchyListener(handler)
		End Sub

		' Called from initIndeterminateValues to initialize the animation index.
		' This assumes that numFrames is set to a correct value.
		Private Sub initAnimationIndex()
			If (progressBar.orientation = JProgressBar.HORIZONTAL) AndAlso (BasicGraphicsUtils.isLeftToRight(progressBar)) Then
				' If this is a left-to-right progress bar,
				' start at the first frame.
				animationIndex = 0
			Else
				' If we go right-to-left or vertically, start at the right/bottom.
				animationIndex = numFrames\2
			End If
		End Sub

		'
		' Animation Thread
		'
		''' <summary>
		''' Implements an animation thread that invokes repaint
		''' at a fixed rate.  If ADJUSTTIMER is true, this thread
		''' will continuously adjust the repaint interval to
		''' try to make the actual time between repaints match
		''' the requested rate.
		''' </summary>
		Private Class Animator
			Implements ActionListener

			Private ReadOnly outerInstance As BasicProgressBarUI

			Public Sub New(ByVal outerInstance As BasicProgressBarUI)
				Me.outerInstance = outerInstance
			End Sub

			Private ___timer As Timer
			Private previousDelay As Long 'used to tune the repaint interval
			Private interval As Integer 'the fixed repaint interval
			Private lastCall As Long 'the last time actionPerformed was called
			Private MINIMUM_DELAY As Integer = 5

			''' <summary>
			''' Creates a timer if one doesn't already exist,
			''' then starts the timer thread.
			''' </summary>
			Private Sub start(ByVal interval As Integer)
				previousDelay = interval
				lastCall = 0

				If ___timer Is Nothing Then
					___timer = New Timer(interval, Me)
				Else
					___timer.delay = interval
				End If

				If ADJUSTTIMER Then
					___timer.repeats = False
					___timer.coalesce = False
				End If

				___timer.start()
			End Sub

			''' <summary>
			''' Stops the timer thread.
			''' </summary>
			Private Sub [stop]()
				___timer.stop()
			End Sub

			''' <summary>
			''' Reacts to the timer's action events.
			''' </summary>
			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				If ADJUSTTIMER Then
					Dim time As Long = System.currentTimeMillis()

					If lastCall > 0 Then 'adjust nextDelay
					'XXX maybe should cache this after a while
						'actual = time - lastCall
						'difference = actual - interval
						'nextDelay = previousDelay - difference
						'          = previousDelay - (time - lastCall - interval)
					   Dim nextDelay As Integer = CInt(Fix(previousDelay - time + lastCall + outerInstance.repaintInterval))
						If nextDelay < MINIMUM_DELAY Then nextDelay = MINIMUM_DELAY
						___timer.initialDelay = nextDelay
						previousDelay = nextDelay
					End If
					___timer.start()
					lastCall = time
				End If

				outerInstance.incrementAnimationIndex() 'paint next frame
			End Sub
		End Class


		''' <summary>
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of {@code BasicProgressBarUI}.
		''' </summary>
		Public Class ChangeHandler
			Implements ChangeListener

			Private ReadOnly outerInstance As BasicProgressBarUI

			Public Sub New(ByVal outerInstance As BasicProgressBarUI)
				Me.outerInstance = outerInstance
			End Sub

			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Handler. If you need to add
			' new functionality add it to the Handler, but make sure this
			' class calls into the Handler.
			Public Overridable Sub stateChanged(ByVal e As ChangeEvent) Implements ChangeListener.stateChanged
				outerInstance.handler.stateChanged(e)
			End Sub
		End Class


		Private Class Handler
			Implements ChangeListener, java.beans.PropertyChangeListener, HierarchyListener

			Private ReadOnly outerInstance As BasicProgressBarUI

			Public Sub New(ByVal outerInstance As BasicProgressBarUI)
				Me.outerInstance = outerInstance
			End Sub

			' ChangeListener
			Public Overridable Sub stateChanged(ByVal e As ChangeEvent) Implements ChangeListener.stateChanged
				Dim model As BoundedRangeModel = outerInstance.progressBar.model
				Dim newRange As Integer = model.maximum - model.minimum
				Dim newPercent As Integer
				Dim oldPercent As Integer = outerInstance.cachedPercent

				If newRange > 0 Then
					newPercent = CInt((100 * CLng(model.value)) \ newRange)
				Else
					newPercent = 0
				End If

				If newPercent <> oldPercent Then
					outerInstance.cachedPercent = newPercent
					outerInstance.progressBar.repaint()
				End If
			End Sub

			' PropertyChangeListener
			Public Overridable Sub propertyChange(ByVal e As java.beans.PropertyChangeEvent)
				Dim prop As String = e.propertyName
				If "indeterminate" = prop Then
					If outerInstance.progressBar.indeterminate Then
						outerInstance.initIndeterminateValues()
					Else
						'clean up
						outerInstance.cleanUpIndeterminateValues()
					End If
					outerInstance.progressBar.repaint()
				End If
			End Sub

			' we don't want the animation to keep running if we're not displayable
			Public Overridable Sub hierarchyChanged(ByVal he As HierarchyEvent)
				If (he.changeFlags And HierarchyEvent.DISPLAYABILITY_CHANGED) <> 0 Then
					If outerInstance.progressBar.indeterminate Then
						If outerInstance.progressBar.displayable Then
							outerInstance.startAnimationTimer()
						Else
							outerInstance.stopAnimationTimer()
						End If
					End If
				End If
			End Sub
		End Class
	End Class

End Namespace