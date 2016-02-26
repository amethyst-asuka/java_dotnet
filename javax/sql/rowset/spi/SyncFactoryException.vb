'
' * Copyright (c) 2003, 2004, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.sql.rowset.spi


	''' <summary>
	''' Indicates an error with <code>SyncFactory</code> mechanism. A disconnected
	''' RowSet implementation cannot be used  without a <code>SyncProvider</code>
	''' being successfully instantiated
	''' 
	''' @author Jonathan Bruce </summary>
	''' <seealso cref= javax.sql.rowset.spi.SyncFactory </seealso>
	''' <seealso cref= javax.sql.rowset.spi.SyncFactoryException </seealso>
	Public Class SyncFactoryException
		Inherits java.sql.SQLException

		''' <summary>
		''' Creates new <code>SyncFactoryException</code> without detail message.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Constructs an <code>SyncFactoryException</code> with the specified
		''' detail message.
		''' </summary>
		''' <param name="msg"> the detail message. </param>
		Public Sub New(ByVal msg As String)
			MyBase.New(msg)
		End Sub

		Friend Const serialVersionUID As Long = -4354595476433200352L
	End Class

End Namespace