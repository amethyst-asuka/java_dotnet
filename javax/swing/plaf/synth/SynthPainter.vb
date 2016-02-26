'
' * Copyright (c) 2002, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.plaf.synth


	''' <summary>
	''' <code>SynthPainter</code> is used for painting portions of
	''' <code>JComponent</code>s. At a minimum each <code>JComponent</code>
	''' has two paint methods: one for the border and one for the background. Some
	''' <code>JComponent</code>s have more than one <code>Region</code>, and as
	''' a consequence more paint methods.
	''' <p>
	''' Instances of <code>SynthPainter</code> are obtained from the
	''' <seealso cref="javax.swing.plaf.synth.SynthStyle#getPainter"/> method.
	''' <p>
	''' You typically supply a <code>SynthPainter</code> by way of Synth's
	''' <a href="doc-files/synthFileFormat.html">file</a> format. The following
	''' example registers a painter for all <code>JButton</code>s that will
	''' render the image <code>myImage.png</code>:
	''' <pre>
	'''  &lt;style id="buttonStyle"&gt;
	'''    &lt;imagePainter path="myImage.png" sourceInsets="2 2 2 2"
	'''                  paintCenter="true" stretch="true"/&gt;
	'''    &lt;insets top="2" bottom="2" left="2" right="2"/&gt;
	'''  &lt;/style&gt;
	'''  &lt;bind style="buttonStyle" type="REGION" key="button"/&gt;
	''' </pre>
	''' <p>
	''' <code>SynthPainter</code> is abstract in so far as it does no painting,
	''' all the methods
	''' are empty. While none of these methods are typed to throw an exception,
	''' subclasses can assume that valid arguments are passed in, and if not
	''' they can throw a <code>NullPointerException</code> or
	''' <code>IllegalArgumentException</code> in response to invalid arguments.
	''' 
	''' @since 1.5
	''' @author Scott Violet
	''' </summary>
	Public MustInherit Class SynthPainter
		''' <summary>
		''' Used to avoid null painter checks everywhere.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: The following anonymous inner class could not be converted:
'		static SynthPainter NULL_PAINTER = New SynthPainterAnonymousInnerClassHelper();
	'ignore


		''' <summary>
		''' Paints the background of an arrow button. Arrow buttons are created by
		''' some components, such as <code>JScrollBar</code>.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintArrowButtonBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of an arrow button. Arrow buttons are created by
		''' some components, such as <code>JScrollBar</code>.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintArrowButtonBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the foreground of an arrow button. This method is responsible
		''' for drawing a graphical representation of a direction, typically
		''' an arrow. Arrow buttons are created by
		''' some components, such as <code>JScrollBar</code>
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="direction"> One of SwingConstants.NORTH, SwingConstants.SOUTH
		'''                  SwingConstants.EAST or SwingConstants.WEST </param>
		Public Overridable Sub paintArrowButtonForeground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal direction As Integer)
		End Sub

		''' <summary>
		''' Paints the background of a button.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintButtonBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of a button.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintButtonBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of a check box menu item.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintCheckBoxMenuItemBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of a check box menu item.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintCheckBoxMenuItemBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of a check box.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintCheckBoxBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of a check box.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintCheckBoxBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of a color chooser.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintColorChooserBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of a color chooser.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintColorChooserBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of a combo box.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintComboBoxBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of a combo box.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintComboBoxBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of a desktop icon.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintDesktopIconBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of a desktop icon.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintDesktopIconBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of a desktop pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintDesktopPaneBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of a desktop pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintDesktopPaneBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of an editor pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintEditorPaneBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of an editor pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintEditorPaneBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of a file chooser.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintFileChooserBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of a file chooser.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintFileChooserBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of a formatted text field.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintFormattedTextFieldBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of a formatted text field.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintFormattedTextFieldBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of an internal frame title pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintInternalFrameTitlePaneBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of an internal frame title pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintInternalFrameTitlePaneBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of an internal frame.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintInternalFrameBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of an internal frame.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintInternalFrameBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of a label.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintLabelBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of a label.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintLabelBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of a list.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintListBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of a list.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintListBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of a menu bar.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintMenuBarBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of a menu bar.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintMenuBarBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of a menu item.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintMenuItemBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of a menu item.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintMenuItemBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of a menu.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintMenuBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of a menu.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintMenuBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of an option pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintOptionPaneBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of an option pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintOptionPaneBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of a panel.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintPanelBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of a panel.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintPanelBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of a password field.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintPasswordFieldBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of a password field.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintPasswordFieldBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of a popup menu.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintPopupMenuBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of a popup menu.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintPopupMenuBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of a progress bar.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintProgressBarBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of a progress bar. This implementation invokes the
		''' method of the same name without the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> one of <code>JProgressBar.HORIZONTAL</code> or
		'''                    <code>JProgressBar.VERTICAL</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintProgressBarBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintProgressBarBackground(context, g, x, y, w, h)
		End Sub

		''' <summary>
		''' Paints the border of a progress bar.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintProgressBarBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of a progress bar. This implementation invokes the
		''' method of the same name without the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> one of <code>JProgressBar.HORIZONTAL</code> or
		'''                    <code>JProgressBar.VERTICAL</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintProgressBarBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintProgressBarBorder(context, g, x, y, w, h)
		End Sub

		''' <summary>
		''' Paints the foreground of a progress bar. is responsible for
		''' providing an indication of the progress of the progress bar.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> one of <code>JProgressBar.HORIZONTAL</code> or
		'''                    <code>JProgressBar.VERTICAL</code> </param>
		Public Overridable Sub paintProgressBarForeground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
		End Sub

		''' <summary>
		''' Paints the background of a radio button menu item.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintRadioButtonMenuItemBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of a radio button menu item.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintRadioButtonMenuItemBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of a radio button.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintRadioButtonBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of a radio button.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintRadioButtonBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of a root pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintRootPaneBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of a root pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintRootPaneBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of a scrollbar.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintScrollBarBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of a scrollbar. This implementation invokes the
		''' method of the same name without the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> Orientation of the JScrollBar, one of
		'''                    <code>JScrollBar.HORIZONTAL</code> or
		'''                    <code>JScrollBar.VERTICAL</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintScrollBarBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintScrollBarBackground(context, g, x, y, w, h)
		End Sub

		''' <summary>
		''' Paints the border of a scrollbar.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintScrollBarBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of a scrollbar. This implementation invokes the
		''' method of the same name without the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> Orientation of the JScrollBar, one of
		'''                    <code>JScrollBar.HORIZONTAL</code> or
		'''                    <code>JScrollBar.VERTICAL</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintScrollBarBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintScrollBarBorder(context, g, x, y, w, h)
		End Sub

		''' <summary>
		''' Paints the background of the thumb of a scrollbar. The thumb provides
		''' a graphical indication as to how much of the Component is visible in a
		''' <code>JScrollPane</code>.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> Orientation of the JScrollBar, one of
		'''                    <code>JScrollBar.HORIZONTAL</code> or
		'''                    <code>JScrollBar.VERTICAL</code> </param>
		Public Overridable Sub paintScrollBarThumbBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
		End Sub

		''' <summary>
		''' Paints the border of the thumb of a scrollbar. The thumb provides
		''' a graphical indication as to how much of the Component is visible in a
		''' <code>JScrollPane</code>.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> Orientation of the JScrollBar, one of
		'''                    <code>JScrollBar.HORIZONTAL</code> or
		'''                    <code>JScrollBar.VERTICAL</code> </param>
		Public Overridable Sub paintScrollBarThumbBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
		End Sub

		''' <summary>
		''' Paints the background of the track of a scrollbar. The track contains
		''' the thumb.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintScrollBarTrackBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of the track of a scrollbar. The track contains
		''' the thumb. This implementation invokes the method of the same name without
		''' the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> Orientation of the JScrollBar, one of
		'''                    <code>JScrollBar.HORIZONTAL</code> or
		'''                    <code>JScrollBar.VERTICAL</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintScrollBarTrackBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintScrollBarTrackBackground(context, g, x, y, w, h)
		End Sub

		''' <summary>
		''' Paints the border of the track of a scrollbar. The track contains
		''' the thumb.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintScrollBarTrackBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of the track of a scrollbar. The track contains
		''' the thumb. This implementation invokes the method of the same name without
		''' the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> Orientation of the JScrollBar, one of
		'''                    <code>JScrollBar.HORIZONTAL</code> or
		'''                    <code>JScrollBar.VERTICAL</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintScrollBarTrackBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintScrollBarTrackBorder(context, g, x, y, w, h)
		End Sub

		''' <summary>
		''' Paints the background of a scroll pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintScrollPaneBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of a scroll pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintScrollPaneBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of a separator.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintSeparatorBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of a separator. This implementation invokes the
		''' method of the same name without the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> One of <code>JSeparator.HORIZONTAL</code> or
		'''                           <code>JSeparator.VERTICAL</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintSeparatorBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintSeparatorBackground(context, g, x, y, w, h)
		End Sub

		''' <summary>
		''' Paints the border of a separator.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintSeparatorBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of a separator. This implementation invokes the
		''' method of the same name without the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> One of <code>JSeparator.HORIZONTAL</code> or
		'''                           <code>JSeparator.VERTICAL</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintSeparatorBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintSeparatorBorder(context, g, x, y, w, h)
		End Sub

		''' <summary>
		''' Paints the foreground of a separator.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> One of <code>JSeparator.HORIZONTAL</code> or
		'''                           <code>JSeparator.VERTICAL</code> </param>
		Public Overridable Sub paintSeparatorForeground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
		End Sub

		''' <summary>
		''' Paints the background of a slider.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintSliderBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of a slider. This implementation invokes the
		''' method of the same name without the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> One of <code>JSlider.HORIZONTAL</code> or
		'''                           <code>JSlider.VERTICAL</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintSliderBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintSliderBackground(context, g, x, y, w, h)
		End Sub

		''' <summary>
		''' Paints the border of a slider.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintSliderBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of a slider. This implementation invokes the
		''' method of the same name without the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> One of <code>JSlider.HORIZONTAL</code> or
		'''                           <code>JSlider.VERTICAL</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintSliderBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintSliderBorder(context, g, x, y, w, h)
		End Sub

		''' <summary>
		''' Paints the background of the thumb of a slider.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> One of <code>JSlider.HORIZONTAL</code> or
		'''                           <code>JSlider.VERTICAL</code> </param>
		Public Overridable Sub paintSliderThumbBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
		End Sub

		''' <summary>
		''' Paints the border of the thumb of a slider.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> One of <code>JSlider.HORIZONTAL</code> or
		'''                           <code>JSlider.VERTICAL</code> </param>
		Public Overridable Sub paintSliderThumbBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
		End Sub

		''' <summary>
		''' Paints the background of the track of a slider.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintSliderTrackBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of the track of a slider. This implementation invokes
		''' the method of the same name without the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> One of <code>JSlider.HORIZONTAL</code> or
		'''                           <code>JSlider.VERTICAL</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintSliderTrackBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintSliderTrackBackground(context, g, x, y, w, h)
		End Sub

		''' <summary>
		''' Paints the border of the track of a slider.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintSliderTrackBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of the track of a slider. This implementation invokes the
		''' method of the same name without the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> One of <code>JSlider.HORIZONTAL</code> or
		'''                           <code>JSlider.VERTICAL</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintSliderTrackBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintSliderTrackBorder(context, g, x, y, w, h)
		End Sub

		''' <summary>
		''' Paints the background of a spinner.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintSpinnerBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of a spinner.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintSpinnerBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of the divider of a split pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintSplitPaneDividerBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of the divider of a split pane. This implementation
		''' invokes the method of the same name without the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> One of <code>JSplitPane.HORIZONTAL_SPLIT</code> or
		'''                           <code>JSplitPane.VERTICAL_SPLIT</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintSplitPaneDividerBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintSplitPaneDividerBackground(context, g, x, y, w, h)
		End Sub

		''' <summary>
		''' Paints the foreground of the divider of a split pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> One of <code>JSplitPane.HORIZONTAL_SPLIT</code> or
		'''                           <code>JSplitPane.VERTICAL_SPLIT</code> </param>
		Public Overridable Sub paintSplitPaneDividerForeground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
		End Sub

		''' <summary>
		''' Paints the divider, when the user is dragging the divider, of a
		''' split pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> One of <code>JSplitPane.HORIZONTAL_SPLIT</code> or
		'''                           <code>JSplitPane.VERTICAL_SPLIT</code> </param>
		Public Overridable Sub paintSplitPaneDragDivider(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
		End Sub

		''' <summary>
		''' Paints the background of a split pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintSplitPaneBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of a split pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintSplitPaneBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of a tabbed pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTabbedPaneBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of a tabbed pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTabbedPaneBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of the area behind the tabs of a tabbed pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTabbedPaneTabAreaBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of the area behind the tabs of a tabbed pane.
		''' This implementation invokes the method of the same name without the
		''' orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> One of <code>JTabbedPane.TOP</code>,
		'''                    <code>JTabbedPane.LEFT</code>,
		'''                    <code>JTabbedPane.BOTTOM</code>, or
		'''                    <code>JTabbedPane.RIGHT</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintTabbedPaneTabAreaBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintTabbedPaneTabAreaBackground(context, g, x, y, w, h)
		End Sub

		''' <summary>
		''' Paints the border of the area behind the tabs of a tabbed pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTabbedPaneTabAreaBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of the area behind the tabs of a tabbed pane. This
		''' implementation invokes the method of the same name without the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> One of <code>JTabbedPane.TOP</code>,
		'''                    <code>JTabbedPane.LEFT</code>,
		'''                    <code>JTabbedPane.BOTTOM</code>, or
		'''                    <code>JTabbedPane.RIGHT</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintTabbedPaneTabAreaBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintTabbedPaneTabAreaBorder(context, g, x, y, w, h)
		End Sub

		''' <summary>
		''' Paints the background of a tab of a tabbed pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="tabIndex"> Index of tab being painted. </param>
		Public Overridable Sub paintTabbedPaneTabBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal tabIndex As Integer)
		End Sub

		''' <summary>
		''' Paints the background of a tab of a tabbed pane. This implementation
		''' invokes the method of the same name without the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="tabIndex"> Index of tab being painted. </param>
		''' <param name="orientation"> One of <code>JTabbedPane.TOP</code>,
		'''                    <code>JTabbedPane.LEFT</code>,
		'''                    <code>JTabbedPane.BOTTOM</code>, or
		'''                    <code>JTabbedPane.RIGHT</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintTabbedPaneTabBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal tabIndex As Integer, ByVal orientation As Integer)
			paintTabbedPaneTabBackground(context, g, x, y, w, h, tabIndex)
		End Sub

		''' <summary>
		''' Paints the border of a tab of a tabbed pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="tabIndex"> Index of tab being painted. </param>
		Public Overridable Sub paintTabbedPaneTabBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal tabIndex As Integer)
		End Sub

		''' <summary>
		''' Paints the border of a tab of a tabbed pane. This implementation invokes
		''' the method of the same name without the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="tabIndex"> Index of tab being painted. </param>
		''' <param name="orientation"> One of <code>JTabbedPane.TOP</code>,
		'''                    <code>JTabbedPane.LEFT</code>,
		'''                    <code>JTabbedPane.BOTTOM</code>, or
		'''                    <code>JTabbedPane.RIGHT</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintTabbedPaneTabBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal tabIndex As Integer, ByVal orientation As Integer)
			paintTabbedPaneTabBorder(context, g, x, y, w, h, tabIndex)
		End Sub

		''' <summary>
		''' Paints the background of the area that contains the content of the
		''' selected tab of a tabbed pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTabbedPaneContentBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of the area that contains the content of the
		''' selected tab of a tabbed pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTabbedPaneContentBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of the header of a table.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTableHeaderBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of the header of a table.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTableHeaderBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of a table.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTableBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of a table.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTableBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of a text area.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTextAreaBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of a text area.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTextAreaBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of a text pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTextPaneBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of a text pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTextPaneBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of a text field.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTextFieldBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of a text field.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTextFieldBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of a toggle button.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintToggleButtonBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of a toggle button.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintToggleButtonBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of a tool bar.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintToolBarBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of a tool bar. This implementation invokes the
		''' method of the same name without the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> One of <code>JToolBar.HORIZONTAL</code> or
		'''                           <code>JToolBar.VERTICAL</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintToolBarBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintToolBarBackground(context, g, x, y, w, h)
		End Sub

		''' <summary>
		''' Paints the border of a tool bar.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintToolBarBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of a tool bar. This implementation invokes the
		''' method of the same name without the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> One of <code>JToolBar.HORIZONTAL</code> or
		'''                           <code>JToolBar.VERTICAL</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintToolBarBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintToolBarBorder(context, g, x, y, w, h)
		End Sub

		''' <summary>
		''' Paints the background of the tool bar's content area.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintToolBarContentBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of the tool bar's content area. This implementation
		''' invokes the method of the same name without the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> One of <code>JToolBar.HORIZONTAL</code> or
		'''                           <code>JToolBar.VERTICAL</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintToolBarContentBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintToolBarContentBackground(context, g, x, y, w, h)
		End Sub

		''' <summary>
		''' Paints the border of the content area of a tool bar.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintToolBarContentBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of the content area of a tool bar. This implementation
		''' invokes the method of the same name without the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> One of <code>JToolBar.HORIZONTAL</code> or
		'''                           <code>JToolBar.VERTICAL</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintToolBarContentBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintToolBarContentBorder(context, g, x, y, w, h)
		End Sub

		''' <summary>
		''' Paints the background of the window containing the tool bar when it
		''' has been detached from its primary frame.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintToolBarDragWindowBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of the window containing the tool bar when it
		''' has been detached from its primary frame. This implementation invokes the
		''' method of the same name without the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> One of <code>JToolBar.HORIZONTAL</code> or
		'''                           <code>JToolBar.VERTICAL</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintToolBarDragWindowBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintToolBarDragWindowBackground(context, g, x, y, w, h)
		End Sub

		''' <summary>
		''' Paints the border of the window containing the tool bar when it
		''' has been detached from it's primary frame.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintToolBarDragWindowBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of the window containing the tool bar when it
		''' has been detached from it's primary frame. This implementation invokes the
		''' method of the same name without the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> One of <code>JToolBar.HORIZONTAL</code> or
		'''                           <code>JToolBar.VERTICAL</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintToolBarDragWindowBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintToolBarDragWindowBorder(context, g, x, y, w, h)
		End Sub

		''' <summary>
		''' Paints the background of a tool tip.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintToolTipBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of a tool tip.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintToolTipBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of a tree.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTreeBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of a tree.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTreeBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of the row containing a cell in a tree.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTreeCellBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of the row containing a cell in a tree.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTreeCellBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the focus indicator for a cell in a tree when it has focus.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTreeCellFocus(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the background of the viewport.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintViewportBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub

		''' <summary>
		''' Paints the border of a viewport.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintViewportBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
		End Sub
	End Class

End Namespace