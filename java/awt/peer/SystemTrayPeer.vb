'
' * Copyright (c) 2005, 2007, Oracle and/or its affiliates. All rights reserved.
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
	''' The peer interface for <seealso cref="SystemTray"/>. This doesn't need to be
	''' implemented if <seealso cref="SystemTray#isSupported()"/> returns false.
	''' </summary>
	Public Interface SystemTrayPeer

		''' <summary>
		''' Returns the size of the system tray icon.
		''' </summary>
		''' <returns> the size of the system tray icon
		''' </returns>
		''' <seealso cref= SystemTray#getTrayIconSize() </seealso>
		ReadOnly Property trayIconSize As java.awt.Dimension
	End Interface

End Namespace