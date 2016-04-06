'
' * Copyright (c) 1995, 2007, Oracle and/or its affiliates. All rights reserved.
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
	''' The peer interface for <seealso cref="Choice"/>.
	''' 
	''' The peer interfaces are intended only for use in porting
	''' the AWT. They are not intended for use by application
	''' developers, and developers should not implement peers
	''' nor invoke any of the peer methods directly on the peer
	''' instances.
	''' </summary>
	Public Interface ChoicePeer
		Inherits ComponentPeer

		''' <summary>
		''' Adds an item with the string {@code item} to the combo box list
		''' at index {@code index}.
		''' </summary>
		''' <param name="item"> the label to be added to the list </param>
		''' <param name="index"> the index where to add the item
		''' </param>
		''' <seealso cref= Choice#add(String) </seealso>
		Sub add(  item As String,   index As Integer)

		''' <summary>
		''' Removes the item at index {@code index} from the combo box list.
		''' </summary>
		''' <param name="index"> the index where to remove the item
		''' </param>
		''' <seealso cref= Choice#remove(int) </seealso>
		Sub remove(  index As Integer)

		''' <summary>
		''' Removes all items from the combo box list.
		''' </summary>
		''' <seealso cref= Choice#removeAll() </seealso>
		Sub removeAll()

		''' <summary>
		''' Selects the item at index {@code index}.
		''' </summary>
		''' <param name="index"> the index which should be selected
		''' </param>
		''' <seealso cref= Choice#select(int) </seealso>
		Sub [select](  index As Integer)

	End Interface

End Namespace