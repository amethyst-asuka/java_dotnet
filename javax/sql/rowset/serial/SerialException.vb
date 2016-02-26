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

Namespace javax.sql.rowset.serial


	''' <summary>
	''' Indicates and an error with the serialization or de-serialization of
	''' SQL types such as <code>BLOB, CLOB, STRUCT or ARRAY</code> in
	''' addition to SQL types such as <code>DATALINK and JAVAOBJECT</code>
	''' 
	''' </summary>
	Public Class SerialException
		Inherits java.sql.SQLException

		''' <summary>
		''' Creates a new <code>SerialException</code> without a
		''' message.
		''' </summary>
		 Public Sub New()
		 End Sub

		''' <summary>
		''' Creates a new <code>SerialException</code> with the
		''' specified message.
		''' </summary>
		''' <param name="msg"> the detail message </param>
		Public Sub New(ByVal msg As String)
			MyBase.New(msg)
		End Sub

		Friend Const serialVersionUID As Long = -489794565168592690L
	End Class

End Namespace