Imports System
Imports System.Text
Imports javax.swing.colorchooser
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

Namespace javax.swing




	''' <summary>
	''' <code>JColorChooser</code> provides a pane of controls designed to allow
	''' a user to manipulate and select a color.
	''' For information about using color choosers, see
	''' <a
	''' href="https://docs.oracle.com/javase/tutorial/uiswing/components/colorchooser.html">How to Use Color Choosers</a>,
	''' a section in <em>The Java Tutorial</em>.
	''' 
	''' <p>
	''' 
	''' This class provides three levels of API:
	''' <ol>
	''' <li>A static convenience method which shows a modal color-chooser
	''' dialog and returns the color selected by the user.
	''' <li>A static convenience method for creating a color-chooser dialog
	''' where <code>ActionListeners</code> can be specified to be invoked when
	''' the user presses one of the dialog buttons.
	''' <li>The ability to create instances of <code>JColorChooser</code> panes
	''' directly (within any container). <code>PropertyChange</code> listeners
	''' can be added to detect when the current "color" property changes.
	''' </ol>
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
	''' 
	''' @beaninfo
	'''      attribute: isContainer false
	'''    description: A component that supports selecting a Color.
	''' 
	''' 
	''' @author James Gosling
	''' @author Amy Fowler
	''' @author Steve Wilson
	''' </summary>
	Public Class JColorChooser
		Inherits JComponent
		Implements Accessible

		''' <seealso cref= #getUIClassID </seealso>
		''' <seealso cref= #readObject </seealso>
		Private Const uiClassID As String = "ColorChooserUI"

		Private selectionModel As ColorSelectionModel

		Private previewPanel As JComponent = ColorChooserComponentFactory.previewPanel

		Private chooserPanels As AbstractColorChooserPanel() = New AbstractColorChooserPanel(){}

		Private dragEnabled As Boolean

		''' <summary>
		''' The selection model property name.
		''' </summary>
		Public Const SELECTION_MODEL_PROPERTY As String = "selectionModel"

		''' <summary>
		''' The preview panel property name.
		''' </summary>
		Public Const PREVIEW_PANEL_PROPERTY As String = "previewPanel"

		''' <summary>
		''' The chooserPanel array property name.
		''' </summary>
		Public Const CHOOSER_PANELS_PROPERTY As String = "chooserPanels"


		''' <summary>
		''' Shows a modal color-chooser dialog and blocks until the
		''' dialog is hidden.  If the user presses the "OK" button, then
		''' this method hides/disposes the dialog and returns the selected color.
		''' If the user presses the "Cancel" button or closes the dialog without
		''' pressing "OK", then this method hides/disposes the dialog and returns
		''' <code>null</code>.
		''' </summary>
		''' <param name="component">    the parent <code>Component</code> for the dialog </param>
		''' <param name="title">        the String containing the dialog's title </param>
		''' <param name="initialColor"> the initial Color set when the color-chooser is shown </param>
		''' <returns> the selected color or <code>null</code> if the user opted out </returns>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Shared Function showDialog(ByVal component As Component, ByVal title As String, ByVal initialColor As Color) As Color

			Dim pane As New JColorChooser(If(initialColor IsNot Nothing, initialColor, Color.white))

			Dim ok As New ColorTracker(pane)
			Dim dialog As JDialog = createDialog(component, title, True, pane, ok, Nothing)

			dialog.addComponentListener(New ColorChooserDialog.DisposeOnClose)

			dialog.show() ' blocks until user brings dialog down...

			Return ok.color
		End Function


		''' <summary>
		''' Creates and returns a new dialog containing the specified
		''' <code>ColorChooser</code> pane along with "OK", "Cancel", and "Reset"
		''' buttons. If the "OK" or "Cancel" buttons are pressed, the dialog is
		''' automatically hidden (but not disposed).  If the "Reset"
		''' button is pressed, the color-chooser's color will be reset to the
		''' color which was set the last time <code>show</code> was invoked on the
		''' dialog and the dialog will remain showing.
		''' </summary>
		''' <param name="c">              the parent component for the dialog </param>
		''' <param name="title">          the title for the dialog </param>
		''' <param name="modal">          a boolean. When true, the remainder of the program
		'''                       is inactive until the dialog is closed. </param>
		''' <param name="chooserPane">    the color-chooser to be placed inside the dialog </param>
		''' <param name="okListener">     the ActionListener invoked when "OK" is pressed </param>
		''' <param name="cancelListener"> the ActionListener invoked when "Cancel" is pressed </param>
		''' <returns> a new dialog containing the color-chooser pane </returns>
		''' <exception cref="HeadlessException"> if GraphicsEnvironment.isHeadless()
		''' returns true. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Shared Function createDialog(ByVal c As Component, ByVal title As String, ByVal modal As Boolean, ByVal chooserPane As JColorChooser, ByVal okListener As ActionListener, ByVal cancelListener As ActionListener) As JDialog

			Dim window As Window = JOptionPane.getWindowForComponent(c)
			Dim dialog As ColorChooserDialog
			If TypeOf window Is Frame Then
				dialog = New ColorChooserDialog(CType(window, Frame), title, modal, c, chooserPane, okListener, cancelListener)
			Else
				dialog = New ColorChooserDialog(CType(window, Dialog), title, modal, c, chooserPane, okListener, cancelListener)
			End If
			dialog.accessibleContext.accessibleDescription = title
			Return dialog
		End Function

		''' <summary>
		''' Creates a color chooser pane with an initial color of white.
		''' </summary>
		Public Sub New()
			Me.New(Color.white)
		End Sub

		''' <summary>
		''' Creates a color chooser pane with the specified initial color.
		''' </summary>
		''' <param name="initialColor"> the initial color set in the chooser </param>
		Public Sub New(ByVal initialColor As Color)
			Me.New(New DefaultColorSelectionModel(initialColor))

		End Sub

		''' <summary>
		''' Creates a color chooser pane with the specified
		''' <code>ColorSelectionModel</code>.
		''' </summary>
		''' <param name="model"> the <code>ColorSelectionModel</code> to be used </param>
		Public Sub New(ByVal model As ColorSelectionModel)
			selectionModel = model
			updateUI()
			dragEnabled = False
		End Sub

		''' <summary>
		''' Returns the L&amp;F object that renders this component.
		''' </summary>
		''' <returns> the <code>ColorChooserUI</code> object that renders
		'''          this component </returns>
		Public Overridable Property uI As javax.swing.plaf.ColorChooserUI
			Get
				Return CType(ui, javax.swing.plaf.ColorChooserUI)
			End Get
			Set(ByVal ui As javax.swing.plaf.ColorChooserUI)
				MyBase.uI = ui
			End Set
		End Property


		''' <summary>
		''' Notification from the <code>UIManager</code> that the L&amp;F has changed.
		''' Replaces the current UI object with the latest version from the
		''' <code>UIManager</code>.
		''' </summary>
		''' <seealso cref= JComponent#updateUI </seealso>
		Public Overrides Sub updateUI()
			uI = CType(UIManager.getUI(Me), javax.swing.plaf.ColorChooserUI)
		End Sub

		''' <summary>
		''' Returns the name of the L&amp;F class that renders this component.
		''' </summary>
		''' <returns> the string "ColorChooserUI" </returns>
		''' <seealso cref= JComponent#getUIClassID </seealso>
		''' <seealso cref= UIDefaults#getUI </seealso>
		Public Property Overrides uIClassID As String
			Get
				Return uiClassID
			End Get
		End Property

		''' <summary>
		''' Gets the current color value from the color chooser.
		''' By default, this delegates to the model.
		''' </summary>
		''' <returns> the current color value of the color chooser </returns>
		Public Overridable Property color As Color
			Get
				Return selectionModel.selectedColor
			End Get
			Set(ByVal color As Color)
				selectionModel.selectedColor = color
    
			End Set
		End Property


		''' <summary>
		''' Sets the current color of the color chooser to the
		''' specified RGB color.  Note that the values of red, green,
		''' and blue should be between the numbers 0 and 255, inclusive.
		''' </summary>
		''' <param name="r">   an int specifying the amount of Red </param>
		''' <param name="g">   an int specifying the amount of Green </param>
		''' <param name="b">   an int specifying the amount of Blue </param>
		''' <exception cref="IllegalArgumentException"> if r,g,b values are out of range </exception>
		''' <seealso cref= java.awt.Color </seealso>
		Public Overridable Sub setColor(ByVal r As Integer, ByVal g As Integer, ByVal b As Integer)
			color = New Color(r,g,b)
		End Sub

		''' <summary>
		''' Sets the current color of the color chooser to the
		''' specified color.
		''' </summary>
		''' <param name="c"> an integer value that sets the current color in the chooser
		'''          where the low-order 8 bits specify the Blue value,
		'''          the next 8 bits specify the Green value, and the 8 bits
		'''          above that specify the Red value. </param>
		Public Overridable Property color As Integer
			Set(ByVal c As Integer)
				colorlor((c >> 16) And &HFF, (c >> 8) And &HFF, c And &HFF)
			End Set
		End Property

		''' <summary>
		''' Sets the <code>dragEnabled</code> property,
		''' which must be <code>true</code> to enable
		''' automatic drag handling (the first part of drag and drop)
		''' on this component.
		''' The <code>transferHandler</code> property needs to be set
		''' to a non-<code>null</code> value for the drag to do
		''' anything.  The default value of the <code>dragEnabled</code>
		''' property
		''' is <code>false</code>.
		''' 
		''' <p>
		''' 
		''' When automatic drag handling is enabled,
		''' most look and feels begin a drag-and-drop operation
		''' when the user presses the mouse button over the preview panel.
		''' Some look and feels might not support automatic drag and drop;
		''' they will ignore this property.  You can work around such
		''' look and feels by modifying the component
		''' to directly call the <code>exportAsDrag</code> method of a
		''' <code>TransferHandler</code>.
		''' </summary>
		''' <param name="b"> the value to set the <code>dragEnabled</code> property to </param>
		''' <exception cref="HeadlessException"> if
		'''            <code>b</code> is <code>true</code> and
		'''            <code>GraphicsEnvironment.isHeadless()</code>
		'''            returns <code>true</code>
		''' 
		''' @since 1.4
		''' </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= #getDragEnabled </seealso>
		''' <seealso cref= #setTransferHandler </seealso>
		''' <seealso cref= TransferHandler
		''' 
		''' @beaninfo
		'''  description: Determines whether automatic drag handling is enabled.
		'''        bound: false </seealso>
		Public Overridable Property dragEnabled As Boolean
			Set(ByVal b As Boolean)
				If b AndAlso GraphicsEnvironment.headless Then Throw New HeadlessException
				dragEnabled = b
			End Set
			Get
				Return dragEnabled
			End Get
		End Property


		''' <summary>
		''' Sets the current preview panel.
		''' This will fire a <code>PropertyChangeEvent</code> for the property
		''' named "previewPanel".
		''' </summary>
		''' <param name="preview"> the <code>JComponent</code> which displays the current color </param>
		''' <seealso cref= JComponent#addPropertyChangeListener
		''' 
		''' @beaninfo
		'''       bound: true
		'''      hidden: true
		''' description: The UI component which displays the current color. </seealso>
		Public Overridable Property previewPanel As JComponent
			Set(ByVal preview As JComponent)
    
				If previewPanel IsNot preview Then
					Dim oldPreview As JComponent = previewPanel
					previewPanel = preview
					firePropertyChange(JColorChooser.PREVIEW_PANEL_PROPERTY, oldPreview, preview)
				End If
			End Set
			Get
				Return previewPanel
			End Get
		End Property


		''' <summary>
		''' Adds a color chooser panel to the color chooser.
		''' </summary>
		''' <param name="panel"> the <code>AbstractColorChooserPanel</code> to be added </param>
		Public Overridable Sub addChooserPanel(ByVal panel As AbstractColorChooserPanel)
			Dim oldPanels As AbstractColorChooserPanel() = chooserPanels
			Dim newPanels As AbstractColorChooserPanel() = New AbstractColorChooserPanel(oldPanels.Length){}
			Array.Copy(oldPanels, 0, newPanels, 0, oldPanels.Length)
			newPanels(newPanels.Length-1) = panel
			chooserPanels = newPanels
		End Sub

		''' <summary>
		''' Removes the Color Panel specified.
		''' </summary>
		''' <param name="panel">   a string that specifies the panel to be removed </param>
		''' <returns> the color panel </returns>
		''' <exception cref="IllegalArgumentException"> if panel is not in list of
		'''                  known chooser panels </exception>
		Public Overridable Function removeChooserPanel(ByVal panel As AbstractColorChooserPanel) As AbstractColorChooserPanel


			Dim containedAt As Integer = -1

			For i As Integer = 0 To chooserPanels.Length - 1
				If chooserPanels(i) Is panel Then
					containedAt = i
					Exit For
				End If
			Next i
			If containedAt = -1 Then Throw New System.ArgumentException("chooser panel not in this chooser")

			Dim newArray As AbstractColorChooserPanel() = New AbstractColorChooserPanel(chooserPanels.Length-2){}

			If containedAt = chooserPanels.Length-1 Then ' at end
				Array.Copy(chooserPanels, 0, newArray, 0, newArray.Length)
			ElseIf containedAt = 0 Then ' at start
				Array.Copy(chooserPanels, 1, newArray, 0, newArray.Length)
			Else ' in middle
				Array.Copy(chooserPanels, 0, newArray, 0, containedAt)
				Array.Copy(chooserPanels, containedAt+1, newArray, containedAt, (chooserPanels.Length - containedAt - 1))
			End If

			chooserPanels = newArray

			Return panel
		End Function


		''' <summary>
		''' Specifies the Color Panels used to choose a color value.
		''' </summary>
		''' <param name="panels">  an array of <code>AbstractColorChooserPanel</code>
		'''          objects
		''' 
		''' @beaninfo
		'''       bound: true
		'''      hidden: true
		''' description: An array of different chooser types. </param>
		Public Overridable Property chooserPanels As AbstractColorChooserPanel()
			Set(ByVal panels As AbstractColorChooserPanel())
				Dim oldValue As AbstractColorChooserPanel() = chooserPanels
				chooserPanels = panels
				firePropertyChange(CHOOSER_PANELS_PROPERTY, oldValue, panels)
			End Set
			Get
				Return chooserPanels
			End Get
		End Property


		''' <summary>
		''' Returns the data model that handles color selections.
		''' </summary>
		''' <returns> a <code>ColorSelectionModel</code> object </returns>
		Public Overridable Property selectionModel As ColorSelectionModel
			Get
				Return selectionModel
			End Get
			Set(ByVal newModel As ColorSelectionModel)
				Dim oldModel As ColorSelectionModel = selectionModel
				selectionModel = newModel
				firePropertyChange(JColorChooser.SELECTION_MODEL_PROPERTY, oldModel, newModel)
			End Set
		End Property




		''' <summary>
		''' See <code>readObject</code> and <code>writeObject</code> in
		''' <code>JComponent</code> for more
		''' information about serialization in Swing.
		''' </summary>
		Private Sub writeObject(ByVal s As ObjectOutputStream)
			s.defaultWriteObject()
			If uIClassID.Equals(uiClassID) Then
				Dim count As SByte = JComponent.getWriteObjCounter(Me)
				count -= 1
				JComponent.writeObjCounterter(Me, count)
				If count = 0 AndAlso ui IsNot Nothing Then ui.installUI(Me)
			End If
		End Sub


		''' <summary>
		''' Returns a string representation of this <code>JColorChooser</code>.
		''' This method
		''' is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this <code>JColorChooser</code> </returns>
		Protected Friend Overrides Function paramString() As String
			Dim chooserPanelsString As New StringBuilder("")
			For i As Integer = 0 To chooserPanels.Length - 1
				chooserPanelsString.Append("[" & chooserPanels(i).ToString() & "]")
			Next i
			Dim previewPanelString As String = (If(previewPanel IsNot Nothing, previewPanel.ToString(), ""))

			Return MyBase.paramString() & ",chooserPanels=" & chooserPanelsString.ToString() & ",previewPanel=" & previewPanelString
		End Function

	'///////////////
	' Accessibility support
	'//////////////

		Protected Friend ___accessibleContext As AccessibleContext = Nothing

		''' <summary>
		''' Gets the AccessibleContext associated with this JColorChooser.
		''' For color choosers, the AccessibleContext takes the form of an
		''' AccessibleJColorChooser.
		''' A new AccessibleJColorChooser instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleJColorChooser that serves as the
		'''         AccessibleContext of this JColorChooser </returns>
		Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If ___accessibleContext Is Nothing Then ___accessibleContext = New AccessibleJColorChooser(Me)
				Return ___accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>JColorChooser</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to color chooser user-interface
		''' elements.
		''' </summary>
		Protected Friend Class AccessibleJColorChooser
			Inherits AccessibleJComponent

			Private ReadOnly outerInstance As JColorChooser

			Public Sub New(ByVal outerInstance As JColorChooser)
				Me.outerInstance = outerInstance
			End Sub


			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object </returns>
			''' <seealso cref= AccessibleRole </seealso>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.COLOR_CHOOSER
				End Get
			End Property

		End Class ' inner class AccessibleJColorChooser
	End Class


	'
	' * Class which builds a color chooser dialog consisting of
	' * a JColorChooser with "Ok", "Cancel", and "Reset" buttons.
	' *
	' * Note: This needs to be fixed to deal with localization!
	' 
	Friend Class ColorChooserDialog
		Inherits JDialog

		Private initialColor As Color
		Private chooserPane As JColorChooser
		Private cancelButton As JButton

		Public Sub New(ByVal owner As Dialog, ByVal title As String, ByVal modal As Boolean, ByVal c As Component, ByVal chooserPane As JColorChooser, ByVal okListener As ActionListener, ByVal cancelListener As ActionListener)
			MyBase.New(owner, title, modal)
			initColorChooserDialog(c, chooserPane, okListener, cancelListener)
		End Sub

		Public Sub New(ByVal owner As Frame, ByVal title As String, ByVal modal As Boolean, ByVal c As Component, ByVal chooserPane As JColorChooser, ByVal okListener As ActionListener, ByVal cancelListener As ActionListener)
			MyBase.New(owner, title, modal)
			initColorChooserDialog(c, chooserPane, okListener, cancelListener)
		End Sub

		Protected Friend Overridable Sub initColorChooserDialog(ByVal c As Component, ByVal chooserPane As JColorChooser, ByVal okListener As ActionListener, ByVal cancelListener As ActionListener)
			'setResizable(false);

			Me.chooserPane = chooserPane

			Dim locale As Locale = locale
			Dim okString As String = UIManager.getString("ColorChooser.okText", locale)
			Dim cancelString As String = UIManager.getString("ColorChooser.cancelText", locale)
			Dim resetString As String = UIManager.getString("ColorChooser.resetText", locale)

			Dim ___contentPane As Container = contentPane
			___contentPane.layout = New BorderLayout
			___contentPane.add(chooserPane, BorderLayout.CENTER)

	'        
	'         * Create Lower button panel
	'         
			Dim buttonPane As New JPanel
			buttonPane.layout = New FlowLayout(FlowLayout.CENTER)
			Dim okButton As New JButton(okString)
			rootPane.defaultButton = okButton
			okButton.accessibleContext.accessibleDescription = okString
			okButton.actionCommand = "OK"
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			okButton.addActionListener(New ActionListener()
	'		{
	'			public void actionPerformed(ActionEvent e)
	'			{
	'				hide();
	'			}
	'		});
			If okListener IsNot Nothing Then okButton.addActionListener(okListener)
			buttonPane.add(okButton)

			cancelButton = New JButton(cancelString)
			cancelButton.accessibleContext.accessibleDescription = cancelString

			' The following few lines are used to register esc to close the dialog
			Dim cancelKeyAction As Action = New AbstractActionAnonymousInnerClassHelper
			Dim cancelKeyStroke As KeyStroke = KeyStroke.getKeyStroke(KeyEvent.VK_ESCAPE, 0)
			Dim inputMap As InputMap = cancelButton.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW)
			Dim actionMap As ActionMap = cancelButton.actionMap
			If inputMap IsNot Nothing AndAlso actionMap IsNot Nothing Then
				inputMap.put(cancelKeyStroke, "cancel")
				actionMap.put("cancel", cancelKeyAction)
			End If
			' end esc handling

			cancelButton.actionCommand = "cancel"
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			cancelButton.addActionListener(New ActionListener()
	'		{
	'			public void actionPerformed(ActionEvent e)
	'			{
	'				hide();
	'			}
	'		});
			If cancelListener IsNot Nothing Then cancelButton.addActionListener(cancelListener)
			buttonPane.add(cancelButton)

			Dim resetButton As New JButton(resetString)
			resetButton.accessibleContext.setAccessibleDescription(resetString)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			resetButton.addActionListener(New ActionListener()
	'		{
	'		   public void actionPerformed(ActionEvent e)
	'		   {
	'			   reset();
	'		   }
	'		});
			Dim mnemonic As Integer = sun.swing.SwingUtilities2.getUIDefaultsInt("ColorChooser.resetMnemonic", locale, -1)
			If mnemonic <> -1 Then resetButton.setMnemonic(mnemonic)
			buttonPane.add(resetButton)
			___contentPane.add(buttonPane, BorderLayout.SOUTH)

			If JDialog.defaultLookAndFeelDecorated Then
				Dim supportsWindowDecorations As Boolean = UIManager.lookAndFeel.supportsWindowDecorations
				If supportsWindowDecorations Then rootPane.windowDecorationStyle = JRootPane.COLOR_CHOOSER_DIALOG
			End If
			applyComponentOrientation((If(c Is Nothing, rootPane, c)).componentOrientation)

			pack()
			locationRelativeTo = c

			Me.addWindowListener(New Closer(Me))
		End Sub

		Private Class AbstractActionAnonymousInnerClassHelper
			Inherits AbstractAction

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				CType(e.source, AbstractButton).fireActionPerformed(e)
			End Sub
		End Class

		Public Overridable Sub show()
			initialColor = chooserPane.color
			MyBase.show()
		End Sub

		Public Overridable Sub reset()
			chooserPane.color = initialColor
		End Sub

		<Serializable> _
		Friend Class Closer
			Inherits WindowAdapter

			Private ReadOnly outerInstance As ColorChooserDialog

			Public Sub New(ByVal outerInstance As ColorChooserDialog)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub windowClosing(ByVal e As WindowEvent)
				outerInstance.cancelButton.doClick(0)
				Dim w As Window = e.window
				w.hide()
			End Sub
		End Class

		<Serializable> _
		Friend Class DisposeOnClose
			Inherits ComponentAdapter

			Public Overridable Sub componentHidden(ByVal e As ComponentEvent)
				Dim w As Window = CType(e.component, Window)
				w.Dispose()
			End Sub
		End Class

	End Class

	<Serializable> _
	Friend Class ColorTracker
		Implements ActionListener

		Friend chooser As JColorChooser
		Friend color As Color

		Public Sub New(ByVal c As JColorChooser)
			chooser = c
		End Sub

		Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
			color = chooser.color
		End Sub

		Public Overridable Property color As Color
			Get
				Return color
			End Get
		End Property
	End Class

End Namespace