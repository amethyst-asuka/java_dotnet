'
' * Copyright (c) 2005, 2006, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.plaf.nimbus



	''' <summary>
	''' TableScrollPaneCorner - A simple component that paints itself using the table
	''' header background painter. It is used to fill the top right corner of
	''' scrollpane.
	''' 
	''' @author Created by Jasper Potts (Jan 28, 2008)
	''' </summary>
	Friend Class TableScrollPaneCorner
		Inherits javax.swing.JComponent
		Implements javax.swing.plaf.UIResource

		''' <summary>
		''' Paint the component using the Nimbus Table Header Background Painter
		''' </summary>
		Protected Friend Overrides Sub paintComponent(ByVal g As java.awt.Graphics)
			Dim painter As javax.swing.Painter = CType(javax.swing.UIManager.get("TableHeader:""TableHeader.renderer""[Enabled].backgroundPainter"), javax.swing.Painter)
			If painter IsNot Nothing Then
				If TypeOf g Is java.awt.Graphics2D Then
					painter.paint(CType(g, java.awt.Graphics2D),Me,width+1,height)
				Else
					' paint using image to not Graphics2D to support
					' Java 1.1 printing API
					Dim img As New java.awt.image.BufferedImage(width,height, java.awt.image.BufferedImage.TYPE_INT_ARGB)
					Dim g2 As java.awt.Graphics2D = CType(img.graphics, java.awt.Graphics2D)
					painter.paint(g2,Me,width+1,height)
					g2.Dispose()
					g.drawImage(img,0,0,Nothing)
					img = Nothing
				End If
			End If
		End Sub
	End Class

End Namespace