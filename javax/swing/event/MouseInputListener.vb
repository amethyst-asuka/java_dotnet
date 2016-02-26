'
' * Copyright (c) 1998, 2006, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.event


	''' <summary>
	''' A listener implementing all the methods in both the {@code MouseListener} and
	''' {@code MouseMotionListener} interfaces.
	''' </summary>
	''' <seealso cref= MouseInputAdapter
	''' @author Philip Milne </seealso>

	Public Interface MouseInputListener
		Inherits java.awt.event.MouseListener, java.awt.event.MouseMotionListener

	End Interface

End Namespace