Imports Microsoft.VisualBasic
Imports javax.accessibility
Imports javax.swing.text
Imports javax.swing.text.html
Imports javax.swing.plaf.basic

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
	''' A display area for a short text string or an image,
	''' or both.
	''' A label does not react to input events.
	''' As a result, it cannot get the keyboard focus.
	''' A label can, however, display a keyboard alternative
	''' as a convenience for a nearby component
	''' that has a keyboard alternative but can't display it.
	''' <p>
	''' A <code>JLabel</code> object can display
	''' either text, an image, or both.
	''' You can specify where in the label's display area
	''' the label's contents are aligned
	''' by setting the vertical and horizontal alignment.
	''' By default, labels are vertically centered
	''' in their display area.
	''' Text-only labels are leading edge aligned, by default;
	''' image-only labels are horizontally centered, by default.
	''' <p>
	''' You can also specify the position of the text
	''' relative to the image.
	''' By default, text is on the trailing edge of the image,
	''' with the text and image vertically aligned.
	''' <p>
	''' A label's leading and trailing edge are determined from the value of its
	''' <seealso cref="java.awt.ComponentOrientation"/> property.  At present, the default
	''' ComponentOrientation setting maps the leading edge to left and the trailing
	''' edge to right.
	''' 
	''' <p>
	''' Finally, you can use the <code>setIconTextGap</code> method
	''' to specify how many pixels
	''' should appear between the text and the image.
	''' The default is 4 pixels.
	''' <p>
	''' See <a href="https://docs.oracle.com/javase/tutorial/uiswing/components/label.html">How to Use Labels</a>
	''' in <em>The Java Tutorial</em>
	''' for further documentation.
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
	''' description: A component that displays a short string and an icon.
	''' 
	''' @author Hans Muller
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class JLabel
		Inherits JComponent
		Implements SwingConstants, Accessible

		''' <seealso cref= #getUIClassID </seealso>
		''' <seealso cref= #readObject </seealso>
		Private Const uiClassID As String = "LabelUI"

		Private mnemonic As Integer = ControlChars.NullChar
		Private mnemonicIndex As Integer = -1

		Private text As String = "" ' "" rather than null, for BeanBox
		Private defaultIcon As Icon = Nothing
		Private disabledIcon As Icon = Nothing
		Private disabledIconSet As Boolean = False

		Private verticalAlignment As Integer = CENTER
		Private horizontalAlignment As Integer = LEADING
		Private verticalTextPosition As Integer = CENTER
		Private horizontalTextPosition As Integer = TRAILING
		Private iconTextGap As Integer = 4

		Protected Friend labelFor As java.awt.Component = Nothing

		''' <summary>
		''' Client property key used to determine what label is labeling the
		''' component.  This is generally not used by labels, but is instead
		''' used by components such as text areas that are being labeled by
		''' labels.  When the labelFor property of a label is set, it will
		''' automatically set the LABELED_BY_PROPERTY of the component being
		''' labelled.
		''' </summary>
		''' <seealso cref= #setLabelFor </seealso>
		Friend Const LABELED_BY_PROPERTY As String = "labeledBy"

		''' <summary>
		''' Creates a <code>JLabel</code> instance with the specified
		''' text, image, and horizontal alignment.
		''' The label is centered vertically in its display area.
		''' The text is on the trailing edge of the image.
		''' </summary>
		''' <param name="text">  The text to be displayed by the label. </param>
		''' <param name="icon">  The image to be displayed by the label. </param>
		''' <param name="horizontalAlignment">  One of the following constants
		'''           defined in <code>SwingConstants</code>:
		'''           <code>LEFT</code>,
		'''           <code>CENTER</code>,
		'''           <code>RIGHT</code>,
		'''           <code>LEADING</code> or
		'''           <code>TRAILING</code>. </param>
		Public Sub New(ByVal text As String, ByVal icon As Icon, ByVal horizontalAlignment As Integer)
			text = text
			icon = icon
			horizontalAlignment = horizontalAlignment
			updateUI()
			alignmentX = LEFT_ALIGNMENT
		End Sub

		''' <summary>
		''' Creates a <code>JLabel</code> instance with the specified
		''' text and horizontal alignment.
		''' The label is centered vertically in its display area.
		''' </summary>
		''' <param name="text">  The text to be displayed by the label. </param>
		''' <param name="horizontalAlignment">  One of the following constants
		'''           defined in <code>SwingConstants</code>:
		'''           <code>LEFT</code>,
		'''           <code>CENTER</code>,
		'''           <code>RIGHT</code>,
		'''           <code>LEADING</code> or
		'''           <code>TRAILING</code>. </param>
		Public Sub New(ByVal text As String, ByVal horizontalAlignment As Integer)
			Me.New(text, Nothing, horizontalAlignment)
		End Sub

		''' <summary>
		''' Creates a <code>JLabel</code> instance with the specified text.
		''' The label is aligned against the leading edge of its display area,
		''' and centered vertically.
		''' </summary>
		''' <param name="text">  The text to be displayed by the label. </param>
		Public Sub New(ByVal text As String)
			Me.New(text, Nothing, LEADING)
		End Sub

		''' <summary>
		''' Creates a <code>JLabel</code> instance with the specified
		''' image and horizontal alignment.
		''' The label is centered vertically in its display area.
		''' </summary>
		''' <param name="image">  The image to be displayed by the label. </param>
		''' <param name="horizontalAlignment">  One of the following constants
		'''           defined in <code>SwingConstants</code>:
		'''           <code>LEFT</code>,
		'''           <code>CENTER</code>,
		'''           <code>RIGHT</code>,
		'''           <code>LEADING</code> or
		'''           <code>TRAILING</code>. </param>
		Public Sub New(ByVal image As Icon, ByVal horizontalAlignment As Integer)
			Me.New(Nothing, image, horizontalAlignment)
		End Sub

		''' <summary>
		''' Creates a <code>JLabel</code> instance with the specified image.
		''' The label is centered vertically and horizontally
		''' in its display area.
		''' </summary>
		''' <param name="image">  The image to be displayed by the label. </param>
		Public Sub New(ByVal image As Icon)
			Me.New(Nothing, image, CENTER)
		End Sub

		''' <summary>
		''' Creates a <code>JLabel</code> instance with
		''' no image and with an empty string for the title.
		''' The label is centered vertically
		''' in its display area.
		''' The label's contents, once set, will be displayed on the leading edge
		''' of the label's display area.
		''' </summary>
		Public Sub New()
			Me.New("", Nothing, LEADING)
		End Sub


		''' <summary>
		''' Returns the L&amp;F object that renders this component.
		''' </summary>
		''' <returns> LabelUI object </returns>
		Public Overridable Property uI As javax.swing.plaf.LabelUI
			Get
				Return CType(ui, javax.swing.plaf.LabelUI)
			End Get
			Set(ByVal ui As javax.swing.plaf.LabelUI)
				MyBase.uI = ui
				' disabled icon is generated by LF so it should be unset here
				If (Not disabledIconSet) AndAlso disabledIcon IsNot Nothing Then disabledIcon = Nothing
			End Set
		End Property




		''' <summary>
		''' Resets the UI property to a value from the current look and feel.
		''' </summary>
		''' <seealso cref= JComponent#updateUI </seealso>
		Public Overrides Sub updateUI()
			uI = CType(UIManager.getUI(Me), javax.swing.plaf.LabelUI)
		End Sub


		''' <summary>
		''' Returns a string that specifies the name of the l&amp;f class
		''' that renders this component.
		''' </summary>
		''' <returns> String "LabelUI"
		''' </returns>
		''' <seealso cref= JComponent#getUIClassID </seealso>
		''' <seealso cref= UIDefaults#getUI </seealso>
		Public Property Overrides uIClassID As String
			Get
				Return uiClassID
			End Get
		End Property


		''' <summary>
		''' Returns the text string that the label displays.
		''' </summary>
		''' <returns> a String </returns>
		''' <seealso cref= #setText </seealso>
		Public Overridable Property text As String
			Get
				Return text
			End Get
			Set(ByVal text As String)
    
				Dim oldAccessibleName As String = Nothing
				If accessibleContext IsNot Nothing Then oldAccessibleName = accessibleContext.accessibleName
    
				Dim oldValue As String = Me.text
				Me.text = text
				firePropertyChange("text", oldValue, text)
    
				displayedMnemonicIndex = SwingUtilities.findDisplayedMnemonicIndex(text, displayedMnemonic)
    
				If (accessibleContext IsNot Nothing) AndAlso (accessibleContext.accessibleName <> oldAccessibleName) Then accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_VISIBLE_DATA_PROPERTY, oldAccessibleName, accessibleContext.accessibleName)
				If text Is Nothing OrElse oldValue Is Nothing OrElse (Not text.Equals(oldValue)) Then
					revalidate()
					repaint()
				End If
			End Set
		End Property




		''' <summary>
		''' Returns the graphic image (glyph, icon) that the label displays.
		''' </summary>
		''' <returns> an Icon </returns>
		''' <seealso cref= #setIcon </seealso>
		Public Overridable Property icon As Icon
			Get
				Return defaultIcon
			End Get
			Set(ByVal icon As Icon)
				Dim oldValue As Icon = defaultIcon
				defaultIcon = icon
    
		'         If the default icon has really changed and we had
		'         * generated the disabled icon for this component
		'         * (in other words, setDisabledIcon() was never called), then
		'         * clear the disabledIcon field.
		'         
				If (defaultIcon IsNot oldValue) AndAlso (Not disabledIconSet) Then disabledIcon = Nothing
    
				firePropertyChange("icon", oldValue, defaultIcon)
    
				If (accessibleContext IsNot Nothing) AndAlso (oldValue IsNot defaultIcon) Then accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_VISIBLE_DATA_PROPERTY, oldValue, defaultIcon)
    
		'         If the default icon has changed and the new one is
		'         * a different size, then revalidate.   Repaint if the
		'         * default icon has changed.
		'         
				If defaultIcon IsNot oldValue Then
					If (defaultIcon Is Nothing) OrElse (oldValue Is Nothing) OrElse (defaultIcon.iconWidth <> oldValue.iconWidth) OrElse (defaultIcon.iconHeight <> oldValue.iconHeight) Then revalidate()
					repaint()
				End If
			End Set
		End Property



		''' <summary>
		''' Returns the icon used by the label when it's disabled.
		''' If no disabled icon has been set this will forward the call to
		''' the look and feel to construct an appropriate disabled Icon.
		''' <p>
		''' Some look and feels might not render the disabled Icon, in which
		''' case they will ignore this.
		''' </summary>
		''' <returns> the <code>disabledIcon</code> property </returns>
		''' <seealso cref= #setDisabledIcon </seealso>
		''' <seealso cref= javax.swing.LookAndFeel#getDisabledIcon </seealso>
		''' <seealso cref= ImageIcon </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Property disabledIcon As Icon
			Get
				If (Not disabledIconSet) AndAlso disabledIcon Is Nothing AndAlso defaultIcon IsNot Nothing Then
					disabledIcon = UIManager.lookAndFeel.getDisabledIcon(Me, defaultIcon)
					If disabledIcon IsNot Nothing Then firePropertyChange("disabledIcon", Nothing, disabledIcon)
				End If
				Return disabledIcon
			End Get
			Set(ByVal disabledIcon As Icon)
				Dim oldValue As Icon = Me.disabledIcon
				Me.disabledIcon = disabledIcon
				disabledIconSet = (disabledIcon IsNot Nothing)
				firePropertyChange("disabledIcon", oldValue, disabledIcon)
				If disabledIcon IsNot oldValue Then
					If disabledIcon Is Nothing OrElse oldValue Is Nothing OrElse disabledIcon.iconWidth <> oldValue.iconWidth OrElse disabledIcon.iconHeight <> oldValue.iconHeight Then revalidate()
					If Not enabled Then repaint()
				End If
			End Set
		End Property




		''' <summary>
		''' Specify a keycode that indicates a mnemonic key.
		''' This property is used when the label is part of a larger component.
		''' If the labelFor property of the label is not null, the label will
		''' call the requestFocus method of the component specified by the
		''' labelFor property when the mnemonic is activated.
		''' </summary>
		''' <seealso cref= #getLabelFor </seealso>
		''' <seealso cref= #setLabelFor
		''' @beaninfo
		'''        bound: true
		'''    attribute: visualUpdate true
		'''  description: The mnemonic keycode. </seealso>
		Public Overridable Property displayedMnemonic As Integer
			Set(ByVal key As Integer)
				Dim oldKey As Integer = mnemonic
				mnemonic = key
				firePropertyChange("displayedMnemonic", oldKey, mnemonic)
    
				displayedMnemonicIndex = SwingUtilities.findDisplayedMnemonicIndex(text, mnemonic)
    
				If key <> oldKey Then
					revalidate()
					repaint()
				End If
			End Set
			Get
				Return mnemonic
			End Get
		End Property


		''' <summary>
		''' Specifies the displayedMnemonic as a char value.
		''' </summary>
		''' <param name="aChar">  a char specifying the mnemonic to display </param>
		''' <seealso cref= #setDisplayedMnemonic(int) </seealso>
		Public Overridable Property displayedMnemonic As Char
			Set(ByVal aChar As Char)
				Dim vk As Integer = java.awt.event.KeyEvent.getExtendedKeyCodeForChar(aChar)
				If vk <> java.awt.event.KeyEvent.VK_UNDEFINED Then displayedMnemonic = vk
			End Set
		End Property



		''' <summary>
		''' Provides a hint to the look and feel as to which character in the
		''' text should be decorated to represent the mnemonic. Not all look and
		''' feels may support this. A value of -1 indicates either there is no
		''' mnemonic, the mnemonic character is not contained in the string, or
		''' the developer does not wish the mnemonic to be displayed.
		''' <p>
		''' The value of this is updated as the properties relating to the
		''' mnemonic change (such as the mnemonic itself, the text...).
		''' You should only ever have to call this if
		''' you do not wish the default character to be underlined. For example, if
		''' the text was 'Save As', with a mnemonic of 'a', and you wanted the 'A'
		''' to be decorated, as 'Save <u>A</u>s', you would have to invoke
		''' <code>setDisplayedMnemonicIndex(5)</code> after invoking
		''' <code>setDisplayedMnemonic(KeyEvent.VK_A)</code>.
		''' 
		''' @since 1.4 </summary>
		''' <param name="index"> Index into the String to underline </param>
		''' <exception cref="IllegalArgumentException"> will be thrown if <code>index</code>
		'''            is &gt;= length of the text, or &lt; -1
		''' 
		''' @beaninfo
		'''        bound: true
		'''    attribute: visualUpdate true
		'''  description: the index into the String to draw the keyboard character
		'''               mnemonic at </exception>
		Public Overridable Property displayedMnemonicIndex As Integer
			Set(ByVal index As Integer)
				Dim oldValue As Integer = mnemonicIndex
				If index = -1 Then
					mnemonicIndex = -1
				Else
					Dim ___text As String = text
					Dim textLength As Integer = If(___text Is Nothing, 0, ___text.Length)
					If index < -1 OrElse index >= textLength Then ' index out of range Throw New System.ArgumentException("index == " & index)
				End If
				mnemonicIndex = index
				firePropertyChange("displayedMnemonicIndex", oldValue, index)
				If index <> oldValue Then
					revalidate()
					repaint()
				End If
			End Set
			Get
				Return mnemonicIndex
			End Get
		End Property


		''' <summary>
		''' Verify that key is a legal value for the horizontalAlignment properties.
		''' </summary>
		''' <param name="key"> the property value to check </param>
		''' <param name="message"> the IllegalArgumentException detail message </param>
		''' <exception cref="IllegalArgumentException"> if key isn't LEFT, CENTER, RIGHT,
		''' LEADING or TRAILING. </exception>
		''' <seealso cref= #setHorizontalTextPosition </seealso>
		''' <seealso cref= #setHorizontalAlignment </seealso>
		Protected Friend Overridable Function checkHorizontalKey(ByVal key As Integer, ByVal message As String) As Integer
			If (key = LEFT) OrElse (key = CENTER) OrElse (key = RIGHT) OrElse (key = LEADING) OrElse (key = TRAILING) Then
				Return key
			Else
				Throw New System.ArgumentException(message)
			End If
		End Function


		''' <summary>
		''' Verify that key is a legal value for the
		''' verticalAlignment or verticalTextPosition properties.
		''' </summary>
		''' <param name="key"> the property value to check </param>
		''' <param name="message"> the IllegalArgumentException detail message </param>
		''' <exception cref="IllegalArgumentException"> if key isn't TOP, CENTER, or BOTTOM. </exception>
		''' <seealso cref= #setVerticalAlignment </seealso>
		''' <seealso cref= #setVerticalTextPosition </seealso>
		Protected Friend Overridable Function checkVerticalKey(ByVal key As Integer, ByVal message As String) As Integer
			If (key = TOP) OrElse (key = CENTER) OrElse (key = BOTTOM) Then
				Return key
			Else
				Throw New System.ArgumentException(message)
			End If
		End Function


		''' <summary>
		''' Returns the amount of space between the text and the icon
		''' displayed in this label.
		''' </summary>
		''' <returns> an int equal to the number of pixels between the text
		'''         and the icon. </returns>
		''' <seealso cref= #setIconTextGap </seealso>
		Public Overridable Property iconTextGap As Integer
			Get
				Return iconTextGap
			End Get
			Set(ByVal iconTextGap As Integer)
				Dim oldValue As Integer = Me.iconTextGap
				Me.iconTextGap = iconTextGap
				firePropertyChange("iconTextGap", oldValue, iconTextGap)
				If iconTextGap <> oldValue Then
					revalidate()
					repaint()
				End If
			End Set
		End Property





		''' <summary>
		''' Returns the alignment of the label's contents along the Y axis.
		''' </summary>
		''' <returns>   The value of the verticalAlignment property, one of the
		'''           following constants defined in <code>SwingConstants</code>:
		'''           <code>TOP</code>,
		'''           <code>CENTER</code>, or
		'''           <code>BOTTOM</code>.
		''' </returns>
		''' <seealso cref= SwingConstants </seealso>
		''' <seealso cref= #setVerticalAlignment </seealso>
		Public Overridable Property verticalAlignment As Integer
			Get
				Return verticalAlignment
			End Get
			Set(ByVal alignment As Integer)
				If alignment = verticalAlignment Then Return
				Dim oldValue As Integer = verticalAlignment
				verticalAlignment = checkVerticalKey(alignment, "verticalAlignment")
				firePropertyChange("verticalAlignment", oldValue, verticalAlignment)
				repaint()
			End Set
		End Property




		''' <summary>
		''' Returns the alignment of the label's contents along the X axis.
		''' </summary>
		''' <returns>   The value of the horizontalAlignment property, one of the
		'''           following constants defined in <code>SwingConstants</code>:
		'''           <code>LEFT</code>,
		'''           <code>CENTER</code>,
		'''           <code>RIGHT</code>,
		'''           <code>LEADING</code> or
		'''           <code>TRAILING</code>.
		''' </returns>
		''' <seealso cref= #setHorizontalAlignment </seealso>
		''' <seealso cref= SwingConstants </seealso>
		Public Overridable Property horizontalAlignment As Integer
			Get
				Return horizontalAlignment
			End Get
			Set(ByVal alignment As Integer)
				If alignment = horizontalAlignment Then Return
				Dim oldValue As Integer = horizontalAlignment
				horizontalAlignment = checkHorizontalKey(alignment, "horizontalAlignment")
				firePropertyChange("horizontalAlignment", oldValue, horizontalAlignment)
				repaint()
			End Set
		End Property



		''' <summary>
		''' Returns the vertical position of the label's text,
		''' relative to its image.
		''' </summary>
		''' <returns>   One of the following constants
		'''           defined in <code>SwingConstants</code>:
		'''           <code>TOP</code>,
		'''           <code>CENTER</code>, or
		'''           <code>BOTTOM</code>.
		''' </returns>
		''' <seealso cref= #setVerticalTextPosition </seealso>
		''' <seealso cref= SwingConstants </seealso>
		Public Overridable Property verticalTextPosition As Integer
			Get
				Return verticalTextPosition
			End Get
			Set(ByVal textPosition As Integer)
				If textPosition = verticalTextPosition Then Return
				Dim old As Integer = verticalTextPosition
				verticalTextPosition = checkVerticalKey(textPosition, "verticalTextPosition")
				firePropertyChange("verticalTextPosition", old, verticalTextPosition)
				revalidate()
				repaint()
			End Set
		End Property




		''' <summary>
		''' Returns the horizontal position of the label's text,
		''' relative to its image.
		''' </summary>
		''' <returns>   One of the following constants
		'''           defined in <code>SwingConstants</code>:
		'''           <code>LEFT</code>,
		'''           <code>CENTER</code>,
		'''           <code>RIGHT</code>,
		'''           <code>LEADING</code> or
		'''           <code>TRAILING</code>.
		''' </returns>
		''' <seealso cref= SwingConstants </seealso>
		Public Overridable Property horizontalTextPosition As Integer
			Get
				Return horizontalTextPosition
			End Get
			Set(ByVal textPosition As Integer)
				Dim old As Integer = horizontalTextPosition
				Me.horizontalTextPosition = checkHorizontalKey(textPosition, "horizontalTextPosition")
				firePropertyChange("horizontalTextPosition", old, horizontalTextPosition)
				revalidate()
				repaint()
			End Set
		End Property




		''' <summary>
		''' This is overridden to return false if the current Icon's Image is
		''' not equal to the passed in Image <code>img</code>.
		''' </summary>
		''' <seealso cref=     java.awt.image.ImageObserver </seealso>
		''' <seealso cref=     java.awt.Component#imageUpdate(java.awt.Image, int, int, int, int, int) </seealso>
		Public Overridable Function imageUpdate(ByVal img As java.awt.Image, ByVal infoflags As Integer, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer) As Boolean
			' Don't use getDisabledIcon, will trigger creation of icon if icon
			' not set.
			If (Not showing) OrElse (Not SwingUtilities.doesIconReferenceImage(icon, img)) AndAlso (Not SwingUtilities.doesIconReferenceImage(disabledIcon, img)) Then Return False
			Return MyBase.imageUpdate(img, infoflags, x, y, w, h)
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


		''' <summary>
		''' Returns a string representation of this JLabel. This method
		''' is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this JLabel. </returns>
		Protected Friend Overrides Function paramString() As String
			Dim textString As String = (If(text IsNot Nothing, text, ""))
			Dim defaultIconString As String = (If((defaultIcon IsNot Nothing) AndAlso (defaultIcon IsNot Me), defaultIcon.ToString(), ""))
			Dim disabledIconString As String = (If((disabledIcon IsNot Nothing) AndAlso (disabledIcon IsNot Me), disabledIcon.ToString(), ""))
			Dim labelForString As String = (If(labelFor IsNot Nothing, labelFor.ToString(), ""))
			Dim verticalAlignmentString As String
			If verticalAlignment = TOP Then
				verticalAlignmentString = "TOP"
			ElseIf verticalAlignment = CENTER Then
				verticalAlignmentString = "CENTER"
			ElseIf verticalAlignment = BOTTOM Then
				verticalAlignmentString = "BOTTOM"
			Else
				verticalAlignmentString = ""
			End If
			Dim horizontalAlignmentString As String
			If horizontalAlignment = LEFT Then
				horizontalAlignmentString = "LEFT"
			ElseIf horizontalAlignment = CENTER Then
				horizontalAlignmentString = "CENTER"
			ElseIf horizontalAlignment = RIGHT Then
				horizontalAlignmentString = "RIGHT"
			ElseIf horizontalAlignment = LEADING Then
				horizontalAlignmentString = "LEADING"
			ElseIf horizontalAlignment = TRAILING Then
				horizontalAlignmentString = "TRAILING"
			Else
				horizontalAlignmentString = ""
			End If
			Dim verticalTextPositionString As String
			If verticalTextPosition = TOP Then
				verticalTextPositionString = "TOP"
			ElseIf verticalTextPosition = CENTER Then
				verticalTextPositionString = "CENTER"
			ElseIf verticalTextPosition = BOTTOM Then
				verticalTextPositionString = "BOTTOM"
			Else
				verticalTextPositionString = ""
			End If
			Dim horizontalTextPositionString As String
			If horizontalTextPosition = LEFT Then
				horizontalTextPositionString = "LEFT"
			ElseIf horizontalTextPosition = CENTER Then
				horizontalTextPositionString = "CENTER"
			ElseIf horizontalTextPosition = RIGHT Then
				horizontalTextPositionString = "RIGHT"
			ElseIf horizontalTextPosition = LEADING Then
				horizontalTextPositionString = "LEADING"
			ElseIf horizontalTextPosition = TRAILING Then
				horizontalTextPositionString = "TRAILING"
			Else
				horizontalTextPositionString = ""
			End If

			Return MyBase.paramString() & ",defaultIcon=" & defaultIconString & ",disabledIcon=" & disabledIconString & ",horizontalAlignment=" & horizontalAlignmentString & ",horizontalTextPosition=" & horizontalTextPositionString & ",iconTextGap=" & iconTextGap & ",labelFor=" & labelForString & ",text=" & textString & ",verticalAlignment=" & verticalAlignmentString & ",verticalTextPosition=" & verticalTextPositionString
		End Function

		''' <summary>
		''' --- Accessibility Support ---
		''' </summary>

		''' <summary>
		''' Get the component this is labelling.
		''' </summary>
		''' <returns> the Component this is labelling.  Can be null if this
		''' does not label a Component.  If the displayedMnemonic
		''' property is set and the labelFor property is also set, the label
		''' will call the requestFocus method of the component specified by the
		''' labelFor property when the mnemonic is activated.
		''' </returns>
		''' <seealso cref= #getDisplayedMnemonic </seealso>
		''' <seealso cref= #setDisplayedMnemonic </seealso>
		Public Overridable Property labelFor As java.awt.Component
			Get
				Return labelFor
			End Get
			Set(ByVal c As java.awt.Component)
				Dim oldC As java.awt.Component = labelFor
				labelFor = c
				firePropertyChange("labelFor", oldC, c)
    
				If TypeOf oldC Is JComponent Then CType(oldC, JComponent).putClientProperty(LABELED_BY_PROPERTY, Nothing)
				If TypeOf c Is JComponent Then CType(c, JComponent).putClientProperty(LABELED_BY_PROPERTY, Me)
			End Set
		End Property


		''' <summary>
		''' Get the AccessibleContext of this object
		''' </summary>
		''' <returns> the AccessibleContext of this object
		''' @beaninfo
		'''       expert: true
		'''  description: The AccessibleContext associated with this Label. </returns>
		Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleJLabel(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' The class used to obtain the accessible role for this object.
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
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Protected Friend Class AccessibleJLabel
			Inherits AccessibleJComponent
			Implements AccessibleText, AccessibleExtendedComponent

			Private ReadOnly outerInstance As JLabel

			Public Sub New(ByVal outerInstance As JLabel)
				Me.outerInstance = outerInstance
			End Sub


			''' <summary>
			''' Get the accessible name of this object.
			''' </summary>
			''' <returns> the localized name of the object -- can be null if this
			''' object does not have a name </returns>
			''' <seealso cref= AccessibleContext#setAccessibleName </seealso>
			Public Overridable Property accessibleName As String
				Get
					Dim name As String = accessibleName
    
					If name Is Nothing Then name = CStr(outerInstance.getClientProperty(AccessibleContext.ACCESSIBLE_NAME_PROPERTY))
					If name Is Nothing Then name = outerInstance.text
					If name Is Nothing Then name = MyBase.accessibleName
					Return name
				End Get
			End Property

			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object </returns>
			''' <seealso cref= AccessibleRole </seealso>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.LABEL
				End Get
			End Property

			''' <summary>
			''' Get the AccessibleIcons associated with this object if one
			''' or more exist.  Otherwise return null.
			''' @since 1.3
			''' </summary>
			Public Overridable Property accessibleIcon As AccessibleIcon()
				Get
					Dim icon As Icon = outerInstance.icon
					If TypeOf icon Is Accessible Then
						Dim ac As AccessibleContext = CType(icon, Accessible).accessibleContext
						If ac IsNot Nothing AndAlso TypeOf ac Is AccessibleIcon Then Return New AccessibleIcon() { CType(ac, AccessibleIcon) }
					End If
					Return Nothing
				End Get
			End Property

			''' <summary>
			''' Get the AccessibleRelationSet associated with this object if one
			''' exists.  Otherwise return null. </summary>
			''' <seealso cref= AccessibleRelation
			''' @since 1.3 </seealso>
			Public Overridable Property accessibleRelationSet As AccessibleRelationSet
				Get
					' Check where the AccessibleContext's relation
					' set already contains a LABEL_FOR relation.
					Dim relationSet As AccessibleRelationSet = MyBase.accessibleRelationSet
    
					If Not relationSet.contains(AccessibleRelation.LABEL_FOR) Then
						Dim c As java.awt.Component = outerInstance.labelFor
						If c IsNot Nothing Then
							Dim relation As New AccessibleRelation(AccessibleRelation.LABEL_FOR)
							relation.target = c
							relationSet.add(relation)
						End If
					End If
					Return relationSet
				End Get
			End Property


			' AccessibleText ---------- 

			Public Overridable Property accessibleText As AccessibleText
				Get
					Dim ___view As View = CType(JLabel.this.getClientProperty("html"), View)
					If ___view IsNot Nothing Then
						Return Me
					Else
						Return Nothing
					End If
				End Get
			End Property

			''' <summary>
			''' Given a point in local coordinates, return the zero-based index
			''' of the character under that Point.  If the point is invalid,
			''' this method returns -1.
			''' </summary>
			''' <param name="p"> the Point in local coordinates </param>
			''' <returns> the zero-based index of the character under Point p; if
			''' Point is invalid returns -1.
			''' @since 1.3 </returns>
			Public Overridable Function getIndexAtPoint(ByVal p As Point) As Integer Implements AccessibleText.getIndexAtPoint
				Dim ___view As View = CType(JLabel.this.getClientProperty("html"), View)
				If ___view IsNot Nothing Then
					Dim r As Rectangle = textRectangle
					If r Is Nothing Then Return -1
					Dim shape As New Rectangle2D.Float(r.x, r.y, r.width, r.height)
					Dim bias As Position.Bias() = New Position.Bias(0){}
					Return ___view.viewToModel(p.x, p.y, shape, bias)
				Else
					Return -1
				End If
			End Function

			''' <summary>
			''' Returns the bounding box of the character at the given
			''' index in the string.  The bounds are returned in local
			''' coordinates. If the index is invalid, <code>null</code> is returned.
			''' </summary>
			''' <param name="i"> the index into the String </param>
			''' <returns> the screen coordinates of the character's bounding box.
			''' If the index is invalid, <code>null</code> is returned.
			''' @since 1.3 </returns>
			Public Overridable Function getCharacterBounds(ByVal i As Integer) As Rectangle Implements AccessibleText.getCharacterBounds
				Dim ___view As View = CType(JLabel.this.getClientProperty("html"), View)
				If ___view IsNot Nothing Then
					Dim r As Rectangle = textRectangle
			If r Is Nothing Then Return Nothing
					Dim shape As New Rectangle2D.Float(r.x, r.y, r.width, r.height)
					Try
						Dim charShape As Shape = ___view.modelToView(i, shape, Position.Bias.Forward)
						Return charShape.bounds
					Catch e As BadLocationException
						Return Nothing
					End Try
				Else
					Return Nothing
				End If
			End Function

			''' <summary>
			''' Return the number of characters (valid indicies)
			''' </summary>
			''' <returns> the number of characters
			''' @since 1.3 </returns>
			Public Overridable Property charCount As Integer Implements AccessibleText.getCharCount
				Get
					Dim ___view As View = CType(JLabel.this.getClientProperty("html"), View)
					If ___view IsNot Nothing Then
						Dim d As Document = ___view.document
						If TypeOf d Is StyledDocument Then
							Dim doc As StyledDocument = CType(d, StyledDocument)
							Return doc.length
						End If
					End If
					Return accessibleContext.accessibleName.length()
				End Get
			End Property

			''' <summary>
			''' Return the zero-based offset of the caret.
			''' 
			''' Note: That to the right of the caret will have the same index
			''' value as the offset (the caret is between two characters). </summary>
			''' <returns> the zero-based offset of the caret.
			''' @since 1.3 </returns>
			Public Overridable Property caretPosition As Integer Implements AccessibleText.getCaretPosition
				Get
					' There is no caret.
					Return -1
				End Get
			End Property

			''' <summary>
			''' Returns the String at a given index.
			''' </summary>
			''' <param name="part"> the AccessibleText.CHARACTER, AccessibleText.WORD,
			''' or AccessibleText.SENTENCE to retrieve </param>
			''' <param name="index"> an index within the text &gt;= 0 </param>
			''' <returns> the letter, word, or sentence,
			'''   null for an invalid index or part
			''' @since 1.3 </returns>
			Public Overridable Function getAtIndex(ByVal part As Integer, ByVal index As Integer) As String Implements AccessibleText.getAtIndex
				If index < 0 OrElse index >= charCount Then Return Nothing
				Select Case part
				Case AccessibleText.CHARACTER
					Try
						Return getText(index, 1)
					Catch e As BadLocationException
						Return Nothing
					End Try
				Case AccessibleText.WORD
					Try
						Dim s As String = getText(0, charCount)
						Dim words As BreakIterator = BreakIterator.getWordInstance(locale)
						words.text = s
						Dim [end] As Integer = words.following(index)
						Return s.Substring(words.previous(), [end] - (words.previous()))
					Catch e As BadLocationException
						Return Nothing
					End Try
				Case AccessibleText.SENTENCE
					Try
						Dim s As String = getText(0, charCount)
						Dim sentence As BreakIterator = BreakIterator.getSentenceInstance(locale)
						sentence.text = s
						Dim [end] As Integer = sentence.following(index)
						Return s.Substring(sentence.previous(), [end] - (sentence.previous()))
					Catch e As BadLocationException
						Return Nothing
					End Try
				Case Else
					Return Nothing
				End Select
			End Function

			''' <summary>
			''' Returns the String after a given index.
			''' </summary>
			''' <param name="part"> the AccessibleText.CHARACTER, AccessibleText.WORD,
			''' or AccessibleText.SENTENCE to retrieve </param>
			''' <param name="index"> an index within the text &gt;= 0 </param>
			''' <returns> the letter, word, or sentence, null for an invalid
			'''  index or part
			''' @since 1.3 </returns>
			Public Overridable Function getAfterIndex(ByVal part As Integer, ByVal index As Integer) As String Implements AccessibleText.getAfterIndex
				If index < 0 OrElse index >= charCount Then Return Nothing
				Select Case part
				Case AccessibleText.CHARACTER
					If index+1 >= charCount Then Return Nothing
					Try
						Return getText(index+1, 1)
					Catch e As BadLocationException
						Return Nothing
					End Try
				Case AccessibleText.WORD
					Try
						Dim s As String = getText(0, charCount)
						Dim words As BreakIterator = BreakIterator.getWordInstance(locale)
						words.text = s
						Dim start As Integer = words.following(index)
						If start = BreakIterator.DONE OrElse start >= s.Length Then Return Nothing
						Dim [end] As Integer = words.following(start)
						If [end] = BreakIterator.DONE OrElse [end] >= s.Length Then Return Nothing
						Return s.Substring(start, [end] - start)
					Catch e As BadLocationException
						Return Nothing
					End Try
				Case AccessibleText.SENTENCE
					Try
						Dim s As String = getText(0, charCount)
						Dim sentence As BreakIterator = BreakIterator.getSentenceInstance(locale)
						sentence.text = s
						Dim start As Integer = sentence.following(index)
						If start = BreakIterator.DONE OrElse start > s.Length Then Return Nothing
						Dim [end] As Integer = sentence.following(start)
						If [end] = BreakIterator.DONE OrElse [end] > s.Length Then Return Nothing
						Return s.Substring(start, [end] - start)
					Catch e As BadLocationException
						Return Nothing
					End Try
				Case Else
					Return Nothing
				End Select
			End Function

			''' <summary>
			''' Returns the String before a given index.
			''' </summary>
			''' <param name="part"> the AccessibleText.CHARACTER, AccessibleText.WORD,
			'''   or AccessibleText.SENTENCE to retrieve </param>
			''' <param name="index"> an index within the text &gt;= 0 </param>
			''' <returns> the letter, word, or sentence, null for an invalid index
			'''  or part
			''' @since 1.3 </returns>
			Public Overridable Function getBeforeIndex(ByVal part As Integer, ByVal index As Integer) As String Implements AccessibleText.getBeforeIndex
				If index < 0 OrElse index > charCount-1 Then Return Nothing
				Select Case part
				Case AccessibleText.CHARACTER
					If index = 0 Then Return Nothing
					Try
						Return getText(index-1, 1)
					Catch e As BadLocationException
						Return Nothing
					End Try
				Case AccessibleText.WORD
					Try
						Dim s As String = getText(0, charCount)
						Dim words As BreakIterator = BreakIterator.getWordInstance(locale)
						words.text = s
						Dim [end] As Integer = words.following(index)
						[end] = words.previous()
						Dim start As Integer = words.previous()
						If start = BreakIterator.DONE Then Return Nothing
						Return s.Substring(start, [end] - start)
					Catch e As BadLocationException
						Return Nothing
					End Try
				Case AccessibleText.SENTENCE
					Try
						Dim s As String = getText(0, charCount)
						Dim sentence As BreakIterator = BreakIterator.getSentenceInstance(locale)
						sentence.text = s
						Dim [end] As Integer = sentence.following(index)
						[end] = sentence.previous()
						Dim start As Integer = sentence.previous()
						If start = BreakIterator.DONE Then Return Nothing
						Return s.Substring(start, [end] - start)
					Catch e As BadLocationException
						Return Nothing
					End Try
				Case Else
					Return Nothing
				End Select
			End Function

			''' <summary>
			''' Return the AttributeSet for a given character at a given index
			''' </summary>
			''' <param name="i"> the zero-based index into the text </param>
			''' <returns> the AttributeSet of the character
			''' @since 1.3 </returns>
			Public Overridable Function getCharacterAttribute(ByVal i As Integer) As AttributeSet Implements AccessibleText.getCharacterAttribute
				Dim ___view As View = CType(JLabel.this.getClientProperty("html"), View)
				If ___view IsNot Nothing Then
					Dim d As Document = ___view.document
					If TypeOf d Is StyledDocument Then
						Dim doc As StyledDocument = CType(d, StyledDocument)
						Dim elem As Element = doc.getCharacterElement(i)
						If elem IsNot Nothing Then Return elem.attributes
					End If
				End If
				Return Nothing
			End Function

			''' <summary>
			''' Returns the start offset within the selected text.
			''' If there is no selection, but there is
			''' a caret, the start and end offsets will be the same.
			''' </summary>
			''' <returns> the index into the text of the start of the selection
			''' @since 1.3 </returns>
			Public Overridable Property selectionStart As Integer Implements AccessibleText.getSelectionStart
				Get
					' Text cannot be selected.
					Return -1
				End Get
			End Property

			''' <summary>
			''' Returns the end offset within the selected text.
			''' If there is no selection, but there is
			''' a caret, the start and end offsets will be the same.
			''' </summary>
			''' <returns> the index into the text of the end of the selection
			''' @since 1.3 </returns>
			Public Overridable Property selectionEnd As Integer Implements AccessibleText.getSelectionEnd
				Get
					' Text cannot be selected.
					Return -1
				End Get
			End Property

			''' <summary>
			''' Returns the portion of the text that is selected.
			''' </summary>
			''' <returns> the String portion of the text that is selected
			''' @since 1.3 </returns>
			Public Overridable Property selectedText As String Implements AccessibleText.getSelectedText
				Get
					' Text cannot be selected.
					Return Nothing
				End Get
			End Property

	'        
	'         * Returns the text substring starting at the specified
	'         * offset with the specified length.
	'         
			Private Function getText(ByVal offset As Integer, ByVal length As Integer) As String

				Dim ___view As View = CType(JLabel.this.getClientProperty("html"), View)
				If ___view IsNot Nothing Then
					Dim d As Document = ___view.document
					If TypeOf d Is StyledDocument Then
						Dim doc As StyledDocument = CType(d, StyledDocument)
						Return doc.getText(offset, length)
					End If
				End If
				Return Nothing
			End Function

	'        
	'         * Returns the bounding rectangle for the component text.
	'         
			Private Property textRectangle As Rectangle
				Get
    
					Dim ___text As String = outerInstance.text
					Dim icon As Icon = If(outerInstance.enabled, outerInstance.icon, outerInstance.disabledIcon)
    
					If (icon Is Nothing) AndAlso (___text Is Nothing) Then Return Nothing
    
					Dim paintIconR As New Rectangle
					Dim paintTextR As New Rectangle
					Dim paintViewR As New Rectangle
					Dim paintViewInsets As New Insets(0, 0, 0, 0)
    
					paintViewInsets = outerInstance.getInsets(paintViewInsets)
					paintViewR.x = paintViewInsets.left
					paintViewR.y = paintViewInsets.top
					paintViewR.width = outerInstance.width - (paintViewInsets.left + paintViewInsets.right)
					paintViewR.height = outerInstance.height - (paintViewInsets.top + paintViewInsets.bottom)
    
					Dim clippedText As String = SwingUtilities.layoutCompoundLabel(CType(JLabel.this, JComponent), outerInstance.getFontMetrics(font), ___text, icon, outerInstance.verticalAlignment, outerInstance.horizontalAlignment, outerInstance.verticalTextPosition, outerInstance.horizontalTextPosition, paintViewR, paintIconR, paintTextR, outerInstance.iconTextGap)
    
					Return paintTextR
				End Get
			End Property

			' ----- AccessibleExtendedComponent

			''' <summary>
			''' Returns the AccessibleExtendedComponent
			''' </summary>
			''' <returns> the AccessibleExtendedComponent </returns>
			Friend Overridable Property accessibleExtendedComponent As AccessibleExtendedComponent
				Get
					Return Me
				End Get
			End Property

			''' <summary>
			''' Returns the tool tip text
			''' </summary>
			''' <returns> the tool tip text, if supported, of the object;
			''' otherwise, null
			''' @since 1.4 </returns>
			Public Overridable Property toolTipText As String Implements AccessibleExtendedComponent.getToolTipText
				Get
					Return outerInstance.toolTipText
				End Get
			End Property

			''' <summary>
			''' Returns the titled border text
			''' </summary>
			''' <returns> the titled border text, if supported, of the object;
			''' otherwise, null
			''' @since 1.4 </returns>
			Public Overridable Property titledBorderText As String Implements AccessibleExtendedComponent.getTitledBorderText
				Get
					Return MyBase.titledBorderText
				End Get
			End Property

			''' <summary>
			''' Returns key bindings associated with this object
			''' </summary>
			''' <returns> the key bindings, if supported, of the object;
			''' otherwise, null </returns>
			''' <seealso cref= AccessibleKeyBinding
			''' @since 1.4 </seealso>
			Public Overridable Property accessibleKeyBinding As AccessibleKeyBinding Implements AccessibleExtendedComponent.getAccessibleKeyBinding
				Get
					Dim mnemonic As Integer = outerInstance.displayedMnemonic
					If mnemonic = 0 Then Return Nothing
					Return New LabelKeyBinding(Me, mnemonic)
				End Get
			End Property

			Friend Class LabelKeyBinding
				Implements AccessibleKeyBinding

				Private ReadOnly outerInstance As JLabel.AccessibleJLabel

				Friend mnemonic As Integer

				Friend Sub New(ByVal outerInstance As JLabel.AccessibleJLabel, ByVal mnemonic As Integer)
						Me.outerInstance = outerInstance
					Me.mnemonic = mnemonic
				End Sub

				''' <summary>
				''' Returns the number of key bindings for this object
				''' </summary>
				''' <returns> the zero-based number of key bindings for this object </returns>
				Public Overridable Property accessibleKeyBindingCount As Integer Implements AccessibleKeyBinding.getAccessibleKeyBindingCount
					Get
						Return 1
					End Get
				End Property

				''' <summary>
				''' Returns a key binding for this object.  The value returned is an
				''' java.lang.Object which must be cast to appropriate type depending
				''' on the underlying implementation of the key.  For example, if the
				''' Object returned is a javax.swing.KeyStroke, the user of this
				''' method should do the following:
				''' <nf><code>
				''' Component c = <get the component that has the key bindings>
				''' AccessibleContext ac = c.getAccessibleContext();
				''' AccessibleKeyBinding akb = ac.getAccessibleKeyBinding();
				''' for (int i = 0; i < akb.getAccessibleKeyBindingCount(); i++) {
				'''     Object o = akb.getAccessibleKeyBinding(i);
				'''     if (o instanceof javax.swing.KeyStroke) {
				'''         javax.swing.KeyStroke keyStroke = (javax.swing.KeyStroke)o;
				'''         <do something with the key binding>
				'''     }
				''' }
				''' </code></nf>
				''' </summary>
				''' <param name="i"> zero-based index of the key bindings </param>
				''' <returns> a javax.lang.Object which specifies the key binding </returns>
				''' <exception cref="IllegalArgumentException"> if the index is
				''' out of bounds </exception>
				''' <seealso cref= #getAccessibleKeyBindingCount </seealso>
				Public Overridable Function getAccessibleKeyBinding(ByVal i As Integer) As Object Implements AccessibleKeyBinding.getAccessibleKeyBinding
					If i <> 0 Then Throw New System.ArgumentException
					Return KeyStroke.getKeyStroke(mnemonic, 0)
				End Function
			End Class

		End Class ' AccessibleJComponent
	End Class

End Namespace