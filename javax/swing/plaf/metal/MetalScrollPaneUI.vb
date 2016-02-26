Imports System
Imports javax.swing
Imports javax.swing.event
Imports javax.swing.border
Imports javax.swing.plaf
Imports javax.swing.plaf.basic

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

Namespace javax.swing.plaf.metal



	''' <summary>
	''' A Metal L&amp;F implementation of ScrollPaneUI.
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
	Public Class MetalScrollPaneUI
		Inherits BasicScrollPaneUI

		Private scrollBarSwapListener As PropertyChangeListener

		Public Shared Function createUI(ByVal x As JComponent) As ComponentUI
			Return New MetalScrollPaneUI
		End Function

		Public Overrides Sub installUI(ByVal c As JComponent)

			MyBase.installUI(c)

			Dim sp As JScrollPane = CType(c, JScrollPane)
			updateScrollbarsFreeStanding()
		End Sub

		Public Overrides Sub uninstallUI(ByVal c As JComponent)
			MyBase.uninstallUI(c)

			Dim sp As JScrollPane = CType(c, JScrollPane)
			Dim hsb As JScrollBar = sp.horizontalScrollBar
			Dim vsb As JScrollBar = sp.verticalScrollBar
			If hsb IsNot Nothing Then hsb.putClientProperty(MetalScrollBarUI.FREE_STANDING_PROP, Nothing)
			If vsb IsNot Nothing Then vsb.putClientProperty(MetalScrollBarUI.FREE_STANDING_PROP, Nothing)
		End Sub

		Public Overrides Sub installListeners(ByVal scrollPane As JScrollPane)
			MyBase.installListeners(scrollPane)
			scrollBarSwapListener = createScrollBarSwapListener()
			scrollPane.addPropertyChangeListener(scrollBarSwapListener)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub uninstallListeners(ByVal c As JComponent)
			MyBase.uninstallListeners(c)
			c.removePropertyChangeListener(scrollBarSwapListener)
		End Sub

		''' @deprecated - Replaced by <seealso cref="#uninstallListeners(JComponent)"/> 
		<Obsolete("- Replaced by <seealso cref="#uninstallListeners(JComponent)"/>")> _
		Public Overridable Sub uninstallListeners(ByVal scrollPane As JScrollPane)
			MyBase.uninstallListeners(scrollPane)
			scrollPane.removePropertyChangeListener(scrollBarSwapListener)
		End Sub

		''' <summary>
		''' If the border of the scrollpane is an instance of
		''' <code>MetalBorders.ScrollPaneBorder</code>, the client property
		''' <code>FREE_STANDING_PROP</code> of the scrollbars
		''' is set to false, otherwise it is set to true.
		''' </summary>
		Private Sub updateScrollbarsFreeStanding()
			If scrollpane Is Nothing Then Return
			Dim border As Border = scrollpane.border
			Dim value As Object

			If TypeOf border Is MetalBorders.ScrollPaneBorder Then
				value = Boolean.FALSE
			Else
				value = Boolean.TRUE
			End If
			Dim sb As JScrollBar = scrollpane.horizontalScrollBar
			If sb IsNot Nothing Then sb.putClientProperty(MetalScrollBarUI.FREE_STANDING_PROP, value)
			sb = scrollpane.verticalScrollBar
			If sb IsNot Nothing Then sb.putClientProperty(MetalScrollBarUI.FREE_STANDING_PROP, value)
		End Sub

		Protected Friend Overridable Function createScrollBarSwapListener() As PropertyChangeListener
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			Return New PropertyChangeListener()
	'		{
	'			public void propertyChange(PropertyChangeEvent e)
	'			{
	'				  String propertyName = e.getPropertyName();
	'				  if (propertyName.equals("verticalScrollBar") || propertyName.equals("horizontalScrollBar"))
	'				  {
	'					  JScrollBar oldSB = (JScrollBar)e.getOldValue();
	'					  if (oldSB != Nothing)
	'					  {
	'						  oldSB.putClientProperty(MetalScrollBarUI.FREE_STANDING_PROP, Nothing);
	'					  }
	'					  JScrollBar newSB = (JScrollBar)e.getNewValue();
	'					  if (newSB != Nothing)
	'					  {
	'						  newSB.putClientProperty(MetalScrollBarUI.FREE_STANDING_PROP, Boolean.FALSE);
	'					  }
	'				  }
	'				  else if ("border".equals(propertyName))
	'				  {
	'					  updateScrollbarsFreeStanding();
	'				  }
	'		}
	'		};
		End Function

	End Class

End Namespace