'
' * Copyright (c) 1995, 1998, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.awt.peer


	''' <summary>
	''' The peer interface for <seealso cref="TextField"/>.
	''' 
	''' The peer interfaces are intended only for use in porting
	''' the AWT. They are not intended for use by application
	''' developers, and developers should not implement peers
	''' nor invoke any of the peer methods directly on the peer
	''' instances.
	''' </summary>
	Public Interface TextFieldPeer
		Inherits TextComponentPeer

		''' <summary>
		''' Sets the echo character.
		''' </summary>
		''' <param name="echoChar"> the echo character to set
		''' </param>
		''' <seealso cref= TextField#getEchoChar() </seealso>
		WriteOnly Property echoChar As Char

		''' <summary>
		''' Returns the preferred size of the text field with the specified number
		''' of columns.
		''' </summary>
		''' <param name="columns"> the number of columns
		''' </param>
		''' <returns> the preferred size of the text field
		''' </returns>
		''' <seealso cref= TextField#getPreferredSize(int) </seealso>
		Function getPreferredSize(  columns As Integer) As java.awt.Dimension

		''' <summary>
		''' Returns the minimum size of the text field with the specified number
		''' of columns.
		''' </summary>
		''' <param name="columns"> the number of columns
		''' </param>
		''' <returns> the minimum size of the text field
		''' </returns>
		''' <seealso cref= TextField#getMinimumSize(int) </seealso>
		Function getMinimumSize(  columns As Integer) As java.awt.Dimension

	End Interface

End Namespace