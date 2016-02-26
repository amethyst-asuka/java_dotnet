Imports System
Imports javax.swing
Imports javax.swing.border
Imports javax.swing.event
Imports javax.accessibility

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

Namespace javax.swing.colorchooser


	''' <summary>
	''' The standard color swatch chooser.
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
	''' @author Steve Wilson
	''' </summary>
	Friend Class DefaultSwatchChooserPanel
		Inherits AbstractColorChooserPanel

		Friend swatchPanel As SwatchPanel
		Friend recentSwatchPanel As RecentSwatchPanel
		Friend mainSwatchListener As MouseListener
		Friend recentSwatchListener As MouseListener
		Private mainSwatchKeyListener As KeyListener
		Private recentSwatchKeyListener As KeyListener

		Public Sub New()
			MyBase.New()
			inheritsPopupMenu = True
		End Sub

		Public Property Overrides displayName As String
			Get
				Return UIManager.getString("ColorChooser.swatchesNameText", locale)
			End Get
		End Property

		''' <summary>
		''' Provides a hint to the look and feel as to the
		''' <code>KeyEvent.VK</code> constant that can be used as a mnemonic to
		''' access the panel. A return value <= 0 indicates there is no mnemonic.
		''' <p>
		''' The return value here is a hint, it is ultimately up to the look
		''' and feel to honor the return value in some meaningful way.
		''' <p>
		''' This implementation looks up the value from the default
		''' <code>ColorChooser.swatchesMnemonic</code>, or if it
		''' isn't available (or not an <code>Integer</code>) returns -1.
		''' The lookup for the default is done through the <code>UIManager</code>:
		''' <code>UIManager.get("ColorChooser.swatchesMnemonic");</code>.
		''' </summary>
		''' <returns> KeyEvent.VK constant identifying the mnemonic; <= 0 for no
		'''         mnemonic </returns>
		''' <seealso cref= #getDisplayedMnemonicIndex
		''' @since 1.4 </seealso>
		Public Property Overrides mnemonic As Integer
			Get
				Return getInt("ColorChooser.swatchesMnemonic", -1)
			End Get
		End Property

		''' <summary>
		''' Provides a hint to the look and feel as to the index of the character in
		''' <code>getDisplayName</code> that should be visually identified as the
		''' mnemonic. The look and feel should only use this if
		''' <code>getMnemonic</code> returns a value > 0.
		''' <p>
		''' The return value here is a hint, it is ultimately up to the look
		''' and feel to honor the return value in some meaningful way. For example,
		''' a look and feel may wish to render each
		''' <code>AbstractColorChooserPanel</code> in a <code>JTabbedPane</code>,
		''' and further use this return value to underline a character in
		''' the <code>getDisplayName</code>.
		''' <p>
		''' This implementation looks up the value from the default
		''' <code>ColorChooser.rgbDisplayedMnemonicIndex</code>, or if it
		''' isn't available (or not an <code>Integer</code>) returns -1.
		''' The lookup for the default is done through the <code>UIManager</code>:
		''' <code>UIManager.get("ColorChooser.swatchesDisplayedMnemonicIndex");</code>.
		''' </summary>
		''' <returns> Character index to render mnemonic for; -1 to provide no
		'''                   visual identifier for this panel. </returns>
		''' <seealso cref= #getMnemonic
		''' @since 1.4 </seealso>
		Public Property Overrides displayedMnemonicIndex As Integer
			Get
				Return getInt("ColorChooser.swatchesDisplayedMnemonicIndex", -1)
			End Get
		End Property

		Public Property Overrides smallDisplayIcon As Icon
			Get
				Return Nothing
			End Get
		End Property

		Public Property Overrides largeDisplayIcon As Icon
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' The background color, foreground color, and font are already set to the
		''' defaults from the defaults table before this method is called.
		''' </summary>
		Public Overrides Sub installChooserPanel(ByVal enclosingChooser As JColorChooser)
			MyBase.installChooserPanel(enclosingChooser)
		End Sub

		Protected Friend Overrides Sub buildChooser()

			Dim recentStr As String = UIManager.getString("ColorChooser.swatchesRecentText", locale)

			Dim gb As New GridBagLayout
			Dim gbc As New GridBagConstraints
			Dim superHolder As New JPanel(gb)

			swatchPanel = New MainSwatchPanel
			swatchPanel.putClientProperty(AccessibleContext.ACCESSIBLE_NAME_PROPERTY, displayName)
			swatchPanel.inheritsPopupMenu = True

			recentSwatchPanel = New RecentSwatchPanel
			recentSwatchPanel.putClientProperty(AccessibleContext.ACCESSIBLE_NAME_PROPERTY, recentStr)

			mainSwatchKeyListener = New MainSwatchKeyListener(Me)
			mainSwatchListener = New MainSwatchListener(Me)
			swatchPanel.addMouseListener(mainSwatchListener)
			swatchPanel.addKeyListener(mainSwatchKeyListener)
			recentSwatchListener = New RecentSwatchListener(Me)
			recentSwatchKeyListener = New RecentSwatchKeyListener(Me)
			recentSwatchPanel.addMouseListener(recentSwatchListener)
			recentSwatchPanel.addKeyListener(recentSwatchKeyListener)

			Dim mainHolder As New JPanel(New BorderLayout)
			Dim ___border As Border = New CompoundBorder(New LineBorder(Color.black), New LineBorder(Color.white))
			mainHolder.border = ___border
			mainHolder.add(swatchPanel, BorderLayout.CENTER)

			gbc.anchor = GridBagConstraints.LAST_LINE_START
			gbc.gridwidth = 1
			gbc.gridheight = 2
			Dim oldInsets As Insets = gbc.insets
			gbc.insets = New Insets(0, 0, 0, 10)
			superHolder.add(mainHolder, gbc)
			gbc.insets = oldInsets

			recentSwatchPanel.inheritsPopupMenu = True
			Dim recentHolder As New JPanel(New BorderLayout)
			recentHolder.border = ___border
			recentHolder.inheritsPopupMenu = True
			recentHolder.add(recentSwatchPanel, BorderLayout.CENTER)

			Dim l As New JLabel(recentStr)
			l.labelFor = recentSwatchPanel

			gbc.gridwidth = GridBagConstraints.REMAINDER
			gbc.gridheight = 1
			gbc.weighty = 1.0
			superHolder.add(l, gbc)

			gbc.weighty = 0
			gbc.gridheight = GridBagConstraints.REMAINDER
			gbc.insets = New Insets(0, 0, 0, 2)
			superHolder.add(recentHolder, gbc)
			superHolder.inheritsPopupMenu = True

			add(superHolder)
		End Sub

		Public Overrides Sub uninstallChooserPanel(ByVal enclosingChooser As JColorChooser)
			MyBase.uninstallChooserPanel(enclosingChooser)
			swatchPanel.removeMouseListener(mainSwatchListener)
			swatchPanel.removeKeyListener(mainSwatchKeyListener)
			recentSwatchPanel.removeMouseListener(recentSwatchListener)
			recentSwatchPanel.removeKeyListener(recentSwatchKeyListener)

			swatchPanel = Nothing
			recentSwatchPanel = Nothing
			mainSwatchListener = Nothing
			mainSwatchKeyListener = Nothing
			recentSwatchListener = Nothing
			recentSwatchKeyListener = Nothing

			removeAll() ' strip out all the sub-components
		End Sub

		Public Overrides Sub updateChooser()

		End Sub


		Private Class RecentSwatchKeyListener
			Inherits KeyAdapter

			Private ReadOnly outerInstance As DefaultSwatchChooserPanel

			Public Sub New(ByVal outerInstance As DefaultSwatchChooserPanel)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub keyPressed(ByVal e As KeyEvent)
				If KeyEvent.VK_SPACE = e.keyCode Then
					Dim color As Color = outerInstance.recentSwatchPanel.selectedColor
					outerInstance.selectedColor = color
				End If
			End Sub
		End Class

		Private Class MainSwatchKeyListener
			Inherits KeyAdapter

			Private ReadOnly outerInstance As DefaultSwatchChooserPanel

			Public Sub New(ByVal outerInstance As DefaultSwatchChooserPanel)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub keyPressed(ByVal e As KeyEvent)
				If KeyEvent.VK_SPACE = e.keyCode Then
					Dim color As Color = outerInstance.swatchPanel.selectedColor
					outerInstance.selectedColor = color
					outerInstance.recentSwatchPanel.mostRecentColor = color
				End If
			End Sub
		End Class

		<Serializable> _
		Friend Class RecentSwatchListener
			Inherits MouseAdapter

			Private ReadOnly outerInstance As DefaultSwatchChooserPanel

			Public Sub New(ByVal outerInstance As DefaultSwatchChooserPanel)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub mousePressed(ByVal e As MouseEvent)
				If enabled Then
					Dim color As Color = outerInstance.recentSwatchPanel.getColorForLocation(e.x, e.y)
					outerInstance.recentSwatchPanel.selectedColorFromLocationion(e.x, e.y)
					outerInstance.selectedColor = color
					outerInstance.recentSwatchPanel.requestFocusInWindow()
				End If
			End Sub
		End Class

		<Serializable> _
		Friend Class MainSwatchListener
			Inherits MouseAdapter

			Private ReadOnly outerInstance As DefaultSwatchChooserPanel

			Public Sub New(ByVal outerInstance As DefaultSwatchChooserPanel)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub mousePressed(ByVal e As MouseEvent)
				If enabled Then
					Dim color As Color = outerInstance.swatchPanel.getColorForLocation(e.x, e.y)
					outerInstance.selectedColor = color
					outerInstance.swatchPanel.selectedColorFromLocationion(e.x, e.y)
					outerInstance.recentSwatchPanel.mostRecentColor = color
					outerInstance.swatchPanel.requestFocusInWindow()
				End If
			End Sub
		End Class

	End Class



	Friend Class SwatchPanel
		Inherits JPanel

		Protected Friend colors As Color()
		Protected Friend swatchSize As Dimension
		Protected Friend numSwatches As Dimension
		Protected Friend gap As Dimension

		Private selRow As Integer
		Private selCol As Integer

		Public Sub New()
			initValues()
			initColors()
			toolTipText = "" ' register for events
			opaque = True
			background = Color.white
			focusable = True
			inheritsPopupMenu = True

'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			addFocusListener(New FocusAdapter()
	'		{
	'			public void focusGained(FocusEvent e)
	'			{
	'				repaint();
	'			}
	'
	'			public void focusLost(FocusEvent e)
	'			{
	'				repaint();
	'			}
	'		});

'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			addKeyListener(New KeyAdapter()
	'		{
	'			public void keyPressed(KeyEvent e)
	'			{
	'				int typed = e.getKeyCode();
	'				switch (typed)
	'				{
	'					case KeyEvent.VK_UP:
	'						if (selRow > 0)
	'						{
	'							selRow -= 1;
	'							repaint();
	'						}
	'						break;
	'					case KeyEvent.VK_DOWN:
	'						if (selRow < numSwatches.height - 1)
	'						{
	'							selRow += 1;
	'							repaint();
	'						}
	'						break;
	'					case KeyEvent.VK_LEFT:
	'						if (selCol > 0 && outerInstance.getComponentOrientation().isLeftToRight())
	'						{
	'							selCol -= 1;
	'							repaint();
	'						}
	'						else if (selCol < numSwatches.width - 1 && !outerInstance.getComponentOrientation().isLeftToRight())
	'						{
	'							selCol += 1;
	'							repaint();
	'						}
	'						break;
	'					case KeyEvent.VK_RIGHT:
	'						if (selCol < numSwatches.width - 1 && outerInstance.getComponentOrientation().isLeftToRight())
	'						{
	'							selCol += 1;
	'							repaint();
	'						}
	'						else if (selCol > 0 && !outerInstance.getComponentOrientation().isLeftToRight())
	'						{
	'							selCol -= 1;
	'							repaint();
	'						}
	'						break;
	'					case KeyEvent.VK_HOME:
	'						selCol = 0;
	'						selRow = 0;
	'						repaint();
	'						break;
	'					case KeyEvent.VK_END:
	'						selCol = numSwatches.width - 1;
	'						selRow = numSwatches.height - 1;
	'						repaint();
	'						break;
	'				}
	'			}
	'		});
		End Sub

		Public Overridable Property selectedColor As Color
			Get
				Return getColorForCell(selCol, selRow)
			End Get
		End Property

		Protected Friend Overridable Sub initValues()

		End Sub

		Public Overrides Sub paintComponent(ByVal g As Graphics)
			 g.color = background
			 g.fillRect(0,0,width, height)
			 For row As Integer = 0 To numSwatches.height - 1
				Dim ___y As Integer = row * (swatchSize.height + gap.height)
				For column As Integer = 0 To numSwatches.width - 1
					Dim c As Color = getColorForCell(column, row)
					g.color = c
					Dim ___x As Integer
					If Not Me.componentOrientation.leftToRight Then
						___x = (numSwatches.width - column - 1) * (swatchSize.width + gap.width)
					Else
						___x = column * (swatchSize.width + gap.width)
					End If
					g.fillRect(___x, ___y, swatchSize.width, swatchSize.height)
					g.color = Color.black
					g.drawLine(___x+swatchSize.width-1, ___y, ___x+swatchSize.width-1, ___y+swatchSize.height-1)
					g.drawLine(___x, ___y+swatchSize.height-1, ___x+swatchSize.width-1, ___y+swatchSize.height-1)

					If selRow = row AndAlso selCol = column AndAlso Me.focusOwner Then
						Dim c2 As New Color(If(c.red < 125, 255, 0),If(c.green < 125, 255, 0),If(c.blue < 125, 255, 0))
						g.color = c2

						g.drawLine(___x, ___y, ___x + swatchSize.width - 1, ___y)
						g.drawLine(___x, ___y, ___x, ___y + swatchSize.height - 1)
						g.drawLine(___x + swatchSize.width - 1, ___y, ___x + swatchSize.width - 1, ___y + swatchSize.height - 1)
						g.drawLine(___x, ___y + swatchSize.height - 1, ___x + swatchSize.width - 1, ___y + swatchSize.height - 1)
						g.drawLine(___x, ___y, ___x + swatchSize.width - 1, ___y + swatchSize.height - 1)
						g.drawLine(___x, ___y + swatchSize.height - 1, ___x + swatchSize.width - 1, ___y)
					End If
				Next column
			 Next row
		End Sub

		Public Property Overrides preferredSize As Dimension
			Get
				Dim ___x As Integer = numSwatches.width * (swatchSize.width + gap.width) - 1
				Dim ___y As Integer = numSwatches.height * (swatchSize.height + gap.height) - 1
				Return New Dimension(___x, ___y)
			End Get
		End Property

		Protected Friend Overridable Sub initColors()


		End Sub

		Public Overrides Function getToolTipText(ByVal e As MouseEvent) As String
			Dim color As Color = getColorForLocation(e.x, e.y)
			Return color.red & ", " & color.green & ", " & color.blue
		End Function

		Public Overridable Sub setSelectedColorFromLocation(ByVal x As Integer, ByVal y As Integer)
			If Not Me.componentOrientation.leftToRight Then
				selCol = numSwatches.width - x / (swatchSize.width + gap.width) - 1
			Else
				selCol = x / (swatchSize.width + gap.width)
			End If
			selRow = y / (swatchSize.height + gap.height)
			repaint()
		End Sub

		Public Overridable Function getColorForLocation(ByVal x As Integer, ByVal y As Integer) As Color
			Dim column As Integer
			If Not Me.componentOrientation.leftToRight Then
				column = numSwatches.width - x / (swatchSize.width + gap.width) - 1
			Else
				column = x / (swatchSize.width + gap.width)
			End If
			Dim row As Integer = y / (swatchSize.height + gap.height)
			Return getColorForCell(column, row)
		End Function

		Private Function getColorForCell(ByVal column As Integer, ByVal row As Integer) As Color
			Return colors((row * numSwatches.width) + column) ' (STEVE) - change data orientation here
		End Function




	End Class

	Friend Class RecentSwatchPanel
		Inherits SwatchPanel

		Protected Friend Overrides Sub initValues()
			swatchSize = UIManager.getDimension("ColorChooser.swatchesRecentSwatchSize", locale)
			numSwatches = New Dimension(5, 7)
			gap = New Dimension(1, 1)
		End Sub


		Protected Friend Overrides Sub initColors()
			Dim defaultRecentColor As Color = UIManager.getColor("ColorChooser.swatchesDefaultRecentColor", locale)
			Dim numColors As Integer = numSwatches.width * numSwatches.height

			colors = New Color(numColors - 1){}
			For i As Integer = 0 To numColors - 1
				colors(i) = defaultRecentColor
			Next i
		End Sub

		Public Overridable Property mostRecentColor As Color
			Set(ByVal c As Color)
    
				Array.Copy(colors, 0, colors, 1, colors.Length-1)
				colors(0) = c
				repaint()
			End Set
		End Property

	End Class

	Friend Class MainSwatchPanel
		Inherits SwatchPanel


		Protected Friend Overrides Sub initValues()
			swatchSize = UIManager.getDimension("ColorChooser.swatchesSwatchSize", locale)
			numSwatches = New Dimension(31, 9)
			gap = New Dimension(1, 1)
		End Sub

		Protected Friend Overrides Sub initColors()
			Dim rawValues As Integer() = initRawValues()
			Dim numColors As Integer = rawValues.Length \ 3

			colors = New Color(numColors - 1){}
			For i As Integer = 0 To numColors - 1
				colors(i) = New Color(rawValues((i*3)), rawValues((i*3)+1), rawValues((i*3)+2))
			Next i
		End Sub

		Private Function initRawValues() As Integer()

			Dim rawValues As Integer() = { 255, 255, 255, 204, 255, 255, 204, 204, 255, 204, 204, 255, 204, 204, 255, 204, 204, 255, 204, 204, 255, 204, 204, 255, 204, 204, 255, 204, 204, 255, 204, 204, 255, 255, 204, 255, 255, 204, 204, 255, 204, 204, 255, 204, 204, 255, 204, 204, 255, 204, 204, 255, 204, 204, 255, 204, 204, 255, 204, 204, 255, 204, 204, 255, 255, 204, 204, 255, 204, 204, 255, 204, 204, 255, 204, 204, 255, 204, 204, 255, 204, 204, 255, 204, 204, 255, 204, 204, 255, 204, 204, 255, 204, 204, 204, 204, 153, 255, 255, 153, 204, 255, 153, 153, 255, 153, 153, 255, 153, 153, 255, 153, 153, 255, 153, 153, 255, 153, 153, 255, 153, 153, 255, 204, 153, 255, 255, 153, 255, 255, 153, 204, 255, 153, 153, 255, 153, 153, 255, 153, 153, 255, 153, 153, 255, 153, 153, 255, 153, 153, 255, 153, 153, 255, 204, 153, 255, 255, 153, 204, 255, 153, 153, 255, 153, 153, 255, 153, 153, 255, 153, 153, 255, 153, 153, 255, 153, 153, 255, 153, 153, 255, 153, 153, 255, 204, 204, 204, 204, 102, 255, 255, 102, 204, 255, 102, 153, 255, 102, 102, 255, 102, 102, 255, 102, 102, 255, 102, 102, 255, 102, 102, 255, 153, 102, 255, 204, 102, 255, 255, 102, 255, 255, 102, 204, 255, 102, 153, 255, 102, 102, 255, 102, 102, 255, 102, 102, 255, 102, 102, 255, 102, 102, 255, 153, 102, 255, 204, 102, 255, 255, 102, 204, 255, 102, 153, 255, 102, 102, 255, 102, 102, 255, 102, 102, 255, 102, 102, 255, 102, 102, 255, 102, 102, 255, 153, 102, 255, 204, 153, 153, 153, 51, 255, 255, 51, 204, 255, 51, 153, 255, 51, 102, 255, 51, 51, 255, 51, 51, 255, 51, 51, 255, 102, 51, 255, 153, 51, 255, 204, 51, 255, 255, 51, 255, 255, 51, 204, 255, 51, 153, 255, 51, 102, 255, 51, 51, 255, 51, 51, 255, 51, 51, 255, 102, 51, 255, 153, 51, 255, 204, 51, 255, 255, 51, 204, 255, 51, 153, 255, 51, 102, 255, 51, 51, 255, 51, 51, 255, 51, 51, 255, 51, 51, 255, 102, 51, 255, 153, 51, 255, 204, 153, 153, 153, 0, 255, 255, 0, 204, 255, 0, 153, 255, 0, 102, 255, 0, 51, 255, 0, 0, 255, 51, 0, 255, 102, 0, 255, 153, 0, 255, 204, 0, 255, 255, 0, 255, 255, 0, 204, 255, 0, 153, 255, 0, 102, 255, 0, 51, 255, 0, 0, 255, 51, 0, 255, 102, 0, 255, 153, 0, 255, 204, 0, 255, 255, 0, 204, 255, 0, 153, 255, 0, 102, 255, 0, 51, 255, 0, 0, 255, 0, 0, 255, 51, 0, 255, 102, 0, 255, 153, 0, 255, 204, 102, 102, 102, 0, 204, 204, 0, 204, 204, 0, 153, 204, 0, 102, 204, 0, 51, 204, 0, 0, 204, 51, 0, 204, 102, 0, 204, 153, 0, 204, 204, 0, 204, 204, 0, 204, 204, 0, 204, 204, 0, 153, 204, 0, 102, 204, 0, 51, 204, 0, 0, 204, 51, 0, 204, 102, 0, 204, 153, 0, 204, 204, 0, 204, 204, 0, 204, 204, 0, 153, 204, 0, 102, 204, 0, 51, 204, 0, 0, 204, 0, 0, 204, 51, 0, 204, 102, 0, 204, 153, 0, 204, 204, 102, 102, 102, 0, 153, 153, 0, 153, 153, 0, 153, 153, 0, 102, 153, 0, 51, 153, 0, 0, 153, 51, 0, 153, 102, 0, 153, 153, 0, 153, 153, 0, 153, 153, 0, 153, 153, 0, 153, 153, 0, 153, 153, 0, 102, 153, 0, 51, 153, 0, 0, 153, 51, 0, 153, 102, 0, 153, 153, 0, 153, 153, 0, 153, 153, 0, 153, 153, 0, 153, 153, 0, 102, 153, 0, 51, 153, 0, 0, 153, 0, 0, 153, 51, 0, 153, 102, 0, 153, 153, 0, 153, 153, 51, 51, 51, 0, 102, 102, 0, 102, 102, 0, 102, 102, 0, 102, 102, 0, 51, 102, 0, 0, 102, 51, 0, 102, 102, 0, 102, 102, 0, 102, 102, 0, 102, 102, 0, 102, 102, 0, 102, 102, 0, 102, 102, 0, 102, 102, 0, 51, 102, 0, 0, 102, 51, 0, 102, 102, 0, 102, 102, 0, 102, 102, 0, 102, 102, 0, 102, 102, 0, 102, 102, 0, 102, 102, 0, 51, 102, 0, 0, 102, 0, 0, 102, 51, 0, 102, 102, 0, 102, 102, 0, 102, 102, 0, 0, 0, 0, 51, 51, 0, 51, 51, 0, 51, 51, 0, 51, 51, 0, 51, 51, 0, 0, 51, 51, 0, 51, 51, 0, 51, 51, 0, 51, 51, 0, 51, 51, 0, 51, 51, 0, 51, 51, 0, 51, 51, 0, 51, 51, 0, 51, 51, 0, 0, 51, 51, 0, 51, 51, 0, 51, 51, 0, 51, 51, 0, 51, 51, 0, 51, 51, 0, 51, 51, 0, 51, 51, 0, 0, 51, 0, 0, 51, 51, 0, 51, 51, 0, 51, 51, 0, 51, 51, 51, 51, 51 }
			Return rawValues
		End Function
	End Class

End Namespace