Imports System

'
' * Copyright (c) 1996, 2000, Oracle and/or its affiliates. All rights reserved.
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
	''' Signals that the requested data is not supported in this flavor. </summary>
	''' <seealso cref= Transferable#getTransferData
	''' 
	''' @author      Amy Fowler </seealso>
	Public Class UnsupportedFlavorException
		Inherits Exception

	'    
	'     * JDK 1.1 serialVersionUID
	'     
		Private Shadows Const serialVersionUID As Long = 5383814944251665601L

		''' <summary>
		''' Constructs an UnsupportedFlavorException.
		''' </summary>
		''' <param name="flavor"> the flavor object which caused the exception. May
		'''        be <code>null</code>. </param>
		Public Sub New(  flavor As DataFlavor)
			MyBase.New(If(flavor IsNot Nothing, flavor.humanPresentableName, Nothing))
		End Sub
	End Class

End Namespace