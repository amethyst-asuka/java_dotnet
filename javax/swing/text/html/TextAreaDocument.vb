Imports javax.swing.text

'
' * Copyright (c) 1998, Oracle and/or its affiliates. All rights reserved.
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
	''' TextAreaDocument extends the capabilities of the PlainDocument
	''' to store the data that is initially set in the Document.
	''' This is stored in order to enable an accurate reset of the
	''' state when a reset is requested.
	''' 
	''' @author Sunita Mani
	''' </summary>

	Friend Class TextAreaDocument
		Inherits PlainDocument

		Friend initialText As String


		''' <summary>
		''' Resets the model by removing all the data,
		''' and restoring it to its initial state.
		''' </summary>
		Friend Overridable Sub reset()
			Try
				remove(0, length)
				If initialText IsNot Nothing Then insertString(0, initialText, Nothing)
			Catch e As BadLocationException
			End Try
		End Sub

		''' <summary>
		''' Stores the data that the model is initially
		''' loaded with.
		''' </summary>
		Friend Overridable Sub storeInitialText()
			Try
				initialText = getText(0, length)
			Catch e As BadLocationException
			End Try
		End Sub
	End Class

End Namespace