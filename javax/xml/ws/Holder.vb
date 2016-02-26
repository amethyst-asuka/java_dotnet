Imports System

'
' * Copyright (c) 2005, 2010, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.ws


	''' <summary>
	''' Holds a value of type <code>T</code>.
	''' 
	''' @since JAX-WS 2.0
	''' </summary>
	<Serializable> _
	Public NotInheritable Class Holder(Of T)

		Private Const serialVersionUID As Long = 2623699057546497185L

		''' <summary>
		''' The value contained in the holder.
		''' </summary>
		Public value As T

		''' <summary>
		''' Creates a new holder with a <code>null</code> value.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Create a new holder with the specified value.
		''' </summary>
		''' <param name="value"> The value to be stored in the holder. </param>
		Public Sub New(ByVal value As T)
			Me.value = value
		End Sub
	End Class

End Namespace