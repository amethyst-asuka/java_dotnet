'
' * Copyright (c) 1997, 1998, Oracle and/or its affiliates. All rights reserved.
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
	''' A small fixed size picture, typically used to decorate components.
	''' </summary>
	''' <seealso cref= ImageIcon </seealso>

	Public Interface Icon
		''' <summary>
		''' Draw the icon at the specified location.  Icon implementations
		''' may use the Component argument to get properties useful for
		''' painting, e.g. the foreground or background color.
		''' </summary>
		Sub paintIcon(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer)

		''' <summary>
		''' Returns the icon's width.
		''' </summary>
		''' <returns> an int specifying the fixed width of the icon. </returns>
		ReadOnly Property iconWidth As Integer

		''' <summary>
		''' Returns the icon's height.
		''' </summary>
		''' <returns> an int specifying the fixed height of the icon. </returns>
		ReadOnly Property iconHeight As Integer
	End Interface

End Namespace