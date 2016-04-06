Imports System

'
' * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.awt


	''' <summary>
	''' Signals that an Abstract Window Toolkit exception has occurred.
	''' 
	''' @author      Arthur van Hoff
	''' </summary>
	Public Class AWTException
		Inherits Exception

	'    
	'     * JDK 1.1 serialVersionUID
	'     
		 Private Shadows Const serialVersionUID As Long = -1900414231151323879L

		''' <summary>
		''' Constructs an instance of <code>AWTException</code> with the
		''' specified detail message. A detail message is an
		''' instance of <code>String</code> that describes this particular
		''' exception. </summary>
		''' <param name="msg">     the detail message
		''' @since   JDK1.0 </param>
		Public Sub New(  msg As String)
			MyBase.New(msg)
		End Sub
	End Class

End Namespace