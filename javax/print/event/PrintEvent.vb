'
' * Copyright (c) 2000, 2003, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.print.event

	''' 
	''' <summary>
	''' Class PrintEvent is the super class of all Print Service API events.
	''' </summary>

	Public Class PrintEvent
		Inherits java.util.EventObject

		Private Const serialVersionUID As Long = 2286914924430763847L

		''' <summary>
		''' Constructs a PrintEvent object. </summary>
		''' <param name="source"> is the source of the event </param>
		''' <exception cref="IllegalArgumentException"> if <code>source</code> is
		'''         <code>null</code>. </exception>
		Public Sub New(ByVal source As Object)
			MyBase.New(source)
		End Sub

		''' <returns> a message describing the event </returns>
		Public Overrides Function ToString() As String
			Return ("PrintEvent on " & source.ToString())
		End Function

	End Class

End Namespace