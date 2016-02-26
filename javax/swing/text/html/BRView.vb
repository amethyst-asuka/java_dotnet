Imports javax.swing.text

'
' * Copyright (c) 1998, 2004, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.text.html

	''' <summary>
	''' Processes the &lt;BR&gt; tag.  In other words, forces a line break.
	''' 
	''' @author Sunita Mani
	''' </summary>
	Friend Class BRView
		Inherits InlineView

		''' <summary>
		''' Creates a new view that represents a &lt;BR&gt; element.
		''' </summary>
		''' <param name="elem"> the element to create a view for </param>
		Public Sub New(ByVal elem As Element)
			MyBase.New(elem)
		End Sub

		''' <summary>
		''' Forces a line break.
		''' </summary>
		''' <returns> View.ForcedBreakWeight </returns>
		Public Overrides Function getBreakWeight(ByVal axis As Integer, ByVal pos As Single, ByVal len As Single) As Integer
			If axis = X_AXIS Then
				Return ForcedBreakWeight
			Else
				Return MyBase.getBreakWeight(axis, pos, len)
			End If
		End Function
	End Class

End Namespace