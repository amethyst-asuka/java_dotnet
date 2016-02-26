Imports javax.swing
Imports javax.swing.event
Imports javax.swing.border
Imports javax.swing.plaf.basic
Imports javax.swing.plaf

'
' * Copyright (c) 1998, 2009, Oracle and/or its affiliates. All rights reserved.
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
	''' Metal implementation of JInternalFrame.
	''' <p>
	''' 
	''' @author Steve Wilson
	''' </summary>
	Public Class MetalInternalFrameUI
		Inherits BasicInternalFrameUI

	  Private Shared ReadOnly metalPropertyChangeListener As java.beans.PropertyChangeListener = New MetalPropertyChangeHandler

	  Private Shared ReadOnly handyEmptyBorder As Border = New EmptyBorder(0,0,0,0)

	  Protected Friend Shared IS_PALETTE As String = "JInternalFrame.isPalette"
	  Private Shared IS_PALETTE_KEY As String = "JInternalFrame.isPalette"
	  Private Shared FRAME_TYPE As String = "JInternalFrame.frameType"
	  Private Shared NORMAL_FRAME As String = "normal"
	  Private Shared PALETTE_FRAME As String = "palette"
	  Private Shared OPTION_DIALOG As String = "optionDialog"

	  Public Sub New(ByVal b As JInternalFrame)
		MyBase.New(b)
	  End Sub

	  Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
		  Return New MetalInternalFrameUI(CType(c, JInternalFrame))
	  End Function

	  Public Overrides Sub installUI(ByVal c As JComponent)
		MyBase.installUI(c)

		Dim paletteProp As Object = c.getClientProperty(IS_PALETTE_KEY)
		If paletteProp IsNot Nothing Then palette = CBool(paletteProp)

		Dim content As Container = frame.contentPane
		stripContentBorder(content)
		'c.setOpaque(false);
	  End Sub

	  Public Overrides Sub uninstallUI(ByVal c As JComponent)
		  frame = CType(c, JInternalFrame)

		  Dim cont As Container = CType(c, JInternalFrame).contentPane
		  If TypeOf cont Is JComponent Then
			Dim content As JComponent = CType(cont, JComponent)
			If content.border Is handyEmptyBorder Then content.border = Nothing
		  End If
		  MyBase.uninstallUI(c)
	  End Sub

		Protected Friend Overrides Sub installListeners()
			MyBase.installListeners()
			frame.addPropertyChangeListener(metalPropertyChangeListener)
		End Sub

		Protected Friend Overrides Sub uninstallListeners()
			frame.removePropertyChangeListener(metalPropertyChangeListener)
			MyBase.uninstallListeners()
		End Sub

	  Protected Friend Overrides Sub installKeyboardActions()
		  MyBase.installKeyboardActions()
		  Dim map As ActionMap = SwingUtilities.getUIActionMap(frame)
		  If map IsNot Nothing Then map.remove("showSystemMenu")
	  End Sub

	  Protected Friend Overrides Sub uninstallKeyboardActions()
		  MyBase.uninstallKeyboardActions()
	  End Sub

		Protected Friend Overrides Sub uninstallComponents()
			titlePane = Nothing
			MyBase.uninstallComponents()
		End Sub

	  Private Sub stripContentBorder(ByVal c As Object)
			If TypeOf c Is JComponent Then
				Dim contentComp As JComponent = CType(c, JComponent)
				Dim contentBorder As Border = contentComp.border
				If contentBorder Is Nothing OrElse TypeOf contentBorder Is UIResource Then contentComp.border = handyEmptyBorder
			End If
	  End Sub


	  Protected Friend Overrides Function createNorthPane(ByVal w As JInternalFrame) As JComponent
		  Return New MetalInternalFrameTitlePane(w)
	  End Function


	  Private Property frameType As String
		  Set(ByVal frameType As String)
			  If frameType.Equals(OPTION_DIALOG) Then
				  LookAndFeel.installBorder(frame, "InternalFrame.optionDialogBorder")
				  CType(titlePane, MetalInternalFrameTitlePane).palette = False
			  ElseIf frameType.Equals(PALETTE_FRAME) Then
				  LookAndFeel.installBorder(frame, "InternalFrame.paletteBorder")
				  CType(titlePane, MetalInternalFrameTitlePane).palette = True
			  Else
				  LookAndFeel.installBorder(frame, "InternalFrame.border")
				  CType(titlePane, MetalInternalFrameTitlePane).palette = False
			  End If
		  End Set
	  End Property

	  ' this should be deprecated - jcs
	  Public Overridable Property palette As Boolean
		  Set(ByVal isPalette As Boolean)
			If isPalette Then
				LookAndFeel.installBorder(frame, "InternalFrame.paletteBorder")
			Else
				LookAndFeel.installBorder(frame, "InternalFrame.border")
			End If
			CType(titlePane, MetalInternalFrameTitlePane).palette = isPalette
    
		  End Set
	  End Property

	  Private Class MetalPropertyChangeHandler
		  Implements java.beans.PropertyChangeListener

		  Public Overridable Sub propertyChange(ByVal e As java.beans.PropertyChangeEvent)
			  Dim name As String = e.propertyName
			  Dim jif As JInternalFrame = CType(e.source, JInternalFrame)

			  If Not(TypeOf jif.uI Is MetalInternalFrameUI) Then Return

			  Dim ui As MetalInternalFrameUI = CType(jif.uI, MetalInternalFrameUI)

			  If name.Equals(FRAME_TYPE) Then
				  If TypeOf e.newValue Is String Then ui.frameType = CStr(e.newValue)
			  ElseIf name.Equals(IS_PALETTE_KEY) Then
				  If e.newValue IsNot Nothing Then
					  ui.palette = CBool(e.newValue)
				  Else
					  ui.palette = False
				  End If
			  ElseIf name.Equals(JInternalFrame.CONTENT_PANE_PROPERTY) Then
				  ui.stripContentBorder(e.newValue)
			  End If
		  End Sub
	  End Class ' end class MetalPropertyChangeHandler


		Private Class BorderListener1
			Inherits BorderListener
			Implements SwingConstants

			Private ReadOnly outerInstance As MetalInternalFrameUI

			Public Sub New(ByVal outerInstance As MetalInternalFrameUI)
				Me.outerInstance = outerInstance
			End Sub


			Friend Overridable Property iconBounds As Rectangle
				Get
					Dim leftToRight As Boolean = MetalUtils.isLeftToRight(outerInstance.frame)
					Dim xOffset As Integer = If(leftToRight, 5, outerInstance.titlePane.width - 5)
					Dim rect As Rectangle = Nothing
    
					Dim icon As Icon = outerInstance.frame.frameIcon
					If icon IsNot Nothing Then
						If Not leftToRight Then xOffset -= icon.iconWidth
						Dim iconY As Integer = ((outerInstance.titlePane.height \ 2) - (icon.iconHeight \2))
						rect = New Rectangle(xOffset, iconY, icon.iconWidth, icon.iconHeight)
					End If
					Return rect
				End Get
			End Property

			Public Overridable Sub mouseClicked(ByVal e As MouseEvent)
				If e.clickCount = 2 AndAlso e.source Is outerInstance.northPane AndAlso outerInstance.frame.closable AndAlso (Not outerInstance.frame.icon) Then
					Dim rect As Rectangle = iconBounds
					If (rect IsNot Nothing) AndAlso rect.contains(e.x, e.y) Then
						outerInstance.frame.doDefaultCloseAction()
					Else
						MyBase.mouseClicked(e)
					End If
				Else
					MyBase.mouseClicked(e)
				End If
			End Sub
		End Class '/ End BorderListener Class


		''' <summary>
		''' Returns the <code>MouseInputAdapter</code> that will be installed
		''' on the TitlePane.
		''' </summary>
		''' <param name="w"> the <code>JInternalFrame</code> </param>
		''' <returns> the <code>MouseInputAdapter</code> that will be installed
		''' on the TitlePane.
		''' @since 1.6 </returns>
		Protected Friend Overrides Function createBorderListener(ByVal w As JInternalFrame) As MouseInputAdapter
			Return New BorderListener1(Me)
		End Function
	End Class

End Namespace