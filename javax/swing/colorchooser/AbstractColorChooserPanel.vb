Imports Microsoft.VisualBasic
Imports System
Imports javax.swing

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
	''' This is the abstract superclass for color choosers.  If you want to add
	''' a new color chooser panel into a <code>JColorChooser</code>, subclass
	''' this class.
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
	''' @author Tom Santos
	''' @author Steve Wilson
	''' </summary>
	Public MustInherit Class AbstractColorChooserPanel
		Inherits JPanel

'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'		private final java.beans.PropertyChangeListener enabledListener = New java.beans.PropertyChangeListener()
	'	{
	'		public void propertyChange(PropertyChangeEvent event)
	'		{
	'			Object value = event.getNewValue();
	'			if (value instanceof java.lang.Boolean)
	'			{
	'				setEnabled((java.lang.Boolean) value);
	'			}
	'		}
	'	};

		''' 
		Private chooser As JColorChooser

		''' <summary>
		''' Invoked automatically when the model's state changes.
		''' It is also called by <code>installChooserPanel</code> to allow
		''' you to set up the initial state of your chooser.
		''' Override this method to update your <code>ChooserPanel</code>.
		''' </summary>
		Public MustOverride Sub updateChooser()

		''' <summary>
		''' Builds a new chooser panel.
		''' </summary>
		Protected Friend MustOverride Sub buildChooser()

		''' <summary>
		''' Returns a string containing the display name of the panel. </summary>
		''' <returns> the name of the display panel </returns>
		Public MustOverride ReadOnly Property displayName As String

		''' <summary>
		''' Provides a hint to the look and feel as to the
		''' <code>KeyEvent.VK</code> constant that can be used as a mnemonic to
		''' access the panel. A return value &lt;= 0 indicates there is no mnemonic.
		''' <p>
		''' The return value here is a hint, it is ultimately up to the look
		''' and feel to honor the return value in some meaningful way.
		''' <p>
		''' This implementation returns 0, indicating the
		''' <code>AbstractColorChooserPanel</code> does not support a mnemonic,
		''' subclasses wishing a mnemonic will need to override this.
		''' </summary>
		''' <returns> KeyEvent.VK constant identifying the mnemonic; &lt;= 0 for no
		'''         mnemonic </returns>
		''' <seealso cref= #getDisplayedMnemonicIndex
		''' @since 1.4 </seealso>
		Public Overridable Property mnemonic As Integer
			Get
				Return 0
			End Get
		End Property

		''' <summary>
		''' Provides a hint to the look and feel as to the index of the character in
		''' <code>getDisplayName</code> that should be visually identified as the
		''' mnemonic. The look and feel should only use this if
		''' <code>getMnemonic</code> returns a value &gt; 0.
		''' <p>
		''' The return value here is a hint, it is ultimately up to the look
		''' and feel to honor the return value in some meaningful way. For example,
		''' a look and feel may wish to render each
		''' <code>AbstractColorChooserPanel</code> in a <code>JTabbedPane</code>,
		''' and further use this return value to underline a character in
		''' the <code>getDisplayName</code>.
		''' <p>
		''' This implementation returns -1, indicating the
		''' <code>AbstractColorChooserPanel</code> does not support a mnemonic,
		''' subclasses wishing a mnemonic will need to override this.
		''' </summary>
		''' <returns> Character index to render mnemonic for; -1 to provide no
		'''                   visual identifier for this panel. </returns>
		''' <seealso cref= #getMnemonic
		''' @since 1.4 </seealso>
		Public Overridable Property displayedMnemonicIndex As Integer
			Get
				Return -1
			End Get
		End Property

		''' <summary>
		''' Returns the small display icon for the panel. </summary>
		''' <returns> the small display icon </returns>
		Public MustOverride ReadOnly Property smallDisplayIcon As Icon

		''' <summary>
		''' Returns the large display icon for the panel. </summary>
		''' <returns> the large display icon </returns>
		Public MustOverride ReadOnly Property largeDisplayIcon As Icon

		''' <summary>
		''' Invoked when the panel is added to the chooser.
		''' If you override this, be sure to call <code>super</code>. </summary>
		''' <param name="enclosingChooser">  the panel to be added </param>
		''' <exception cref="RuntimeException">  if the chooser panel has already been
		'''                          installed </exception>
		Public Overridable Sub installChooserPanel(ByVal enclosingChooser As JColorChooser)
			If chooser IsNot Nothing Then Throw New Exception("This chooser panel is already installed")
			chooser = enclosingChooser
			chooser.addPropertyChangeListener("enabled", enabledListener)
			enabled = chooser.enabled
			buildChooser()
			updateChooser()
		End Sub

		''' <summary>
		''' Invoked when the panel is removed from the chooser.
		''' If override this, be sure to call <code>super</code>.
		''' </summary>
	  Public Overridable Sub uninstallChooserPanel(ByVal enclosingChooser As JColorChooser)
			chooser.removePropertyChangeListener("enabled", enabledListener)
			chooser = Nothing
	  End Sub

		''' <summary>
		''' Returns the model that the chooser panel is editing. </summary>
		''' <returns> the <code>ColorSelectionModel</code> model this panel
		'''         is editing </returns>
		Public Overridable Property colorSelectionModel As ColorSelectionModel
			Get
				Return If(Me.chooser IsNot Nothing, Me.chooser.selectionModel, Nothing)
			End Get
		End Property

		''' <summary>
		''' Returns the color that is currently selected. </summary>
		''' <returns> the <code>Color</code> that is selected </returns>
		Protected Friend Overridable Property colorFromModel As Color
			Get
				Dim model As ColorSelectionModel = colorSelectionModel
				Return If(model IsNot Nothing, model.selectedColor, Nothing)
			End Get
		End Property

		Friend Overridable Property selectedColor As Color
			Set(ByVal color As Color)
				Dim model As ColorSelectionModel = colorSelectionModel
				If model IsNot Nothing Then model.selectedColor = color
			End Set
		End Property

		''' <summary>
		''' Draws the panel. </summary>
		''' <param name="g">  the <code>Graphics</code> object </param>
		Public Overrides Sub paint(ByVal g As Graphics)
			MyBase.paint(g)
		End Sub

		''' <summary>
		''' Returns an integer from the defaults table. If <code>key</code> does
		''' not map to a valid <code>Integer</code>, <code>default</code> is
		''' returned.
		''' </summary>
		''' <param name="key">  an <code>Object</code> specifying the int </param>
		''' <param name="defaultValue"> Returned value if <code>key</code> is not available,
		'''                     or is not an Integer </param>
		''' <returns> the int </returns>
		Friend Overridable Function getInt(ByVal key As Object, ByVal defaultValue As Integer) As Integer
			Dim value As Object = UIManager.get(key, locale)

			If TypeOf value Is Integer? Then Return CInt(Fix(value))
			If TypeOf value Is String Then
				Try
					Return Convert.ToInt32(CStr(value))
				Catch nfe As NumberFormatException
				End Try
			End If
			Return defaultValue
		End Function
	End Class

End Namespace