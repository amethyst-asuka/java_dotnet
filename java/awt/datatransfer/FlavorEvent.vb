'
' * Copyright (c) 2003, 2006, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.awt.datatransfer



	''' <summary>
	''' <code>FlavorEvent</code> is used to notify interested parties
	''' that available <seealso cref="DataFlavor"/>s have changed in the
	''' <seealso cref="Clipboard"/> (the event source).
	''' </summary>
	''' <seealso cref= FlavorListener
	''' 
	''' @author Alexander Gerasimov
	''' @since 1.5 </seealso>
	Public Class FlavorEvent
		Inherits java.util.EventObject

		''' <summary>
		''' Constructs a <code>FlavorEvent</code> object.
		''' </summary>
		''' <param name="source">  the <code>Clipboard</code> that is the source of the event
		''' </param>
		''' <exception cref="IllegalArgumentException"> if the {@code source} is {@code null} </exception>
		Public Sub New(  source As Clipboard)
			MyBase.New(source)
		End Sub
	End Class

End Namespace