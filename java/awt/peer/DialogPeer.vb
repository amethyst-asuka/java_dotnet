Imports System.Collections.Generic

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
	''' The peer interface for <seealso cref="Dialog"/>. This adds a couple of dialog specific
	''' features to the <seealso cref="WindowPeer"/> interface.
	''' 
	''' The peer interfaces are intended only for use in porting
	''' the AWT. They are not intended for use by application
	''' developers, and developers should not implement peers
	''' nor invoke any of the peer methods directly on the peer
	''' instances.
	''' </summary>
	Public Interface DialogPeer
		Inherits WindowPeer

		''' <summary>
		''' Sets the title on the dialog window.
		''' </summary>
		''' <param name="title"> the title to set
		''' </param>
		''' <seealso cref= Dialog#setTitle(String) </seealso>
		WriteOnly Property title As String

		''' <summary>
		''' Sets if the dialog should be resizable or not.
		''' </summary>
		''' <param name="resizeable"> {@code true} when the dialog should be resizable,
		'''        {@code false} if not
		''' </param>
		''' <seealso cref= Dialog#setResizable(boolean) </seealso>
		WriteOnly Property resizable As Boolean

		''' <summary>
		''' Block the specified windows. This is used for modal dialogs.
		''' </summary>
		''' <param name="windows"> the windows to block
		''' </param>
		''' <seealso cref= Dialog#modalShow() </seealso>
		''' <seealso cref= Dialog#blockWindows() </seealso>
		Sub blockWindows(  windows As IList(Of Window))
	End Interface

End Namespace