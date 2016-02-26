'
' * Copyright (c) 2006, 2012, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.annotation.processing

	''' <summary>
	''' Utility class for assembling <seealso cref="Completion"/> objects.
	''' 
	''' @author Joseph D. Darcy
	''' @author Scott Seligman
	''' @author Peter von der Ah&eacute;
	''' @since 1.6
	''' </summary>
	Public Class Completions
		' No instances for you.
		Private Sub New()
		End Sub

		Private Class SimpleCompletion
			Implements Completion

			Private value As String
			Private message As String

			Friend Sub New(ByVal value As String, ByVal message As String)
				If value Is Nothing OrElse message Is Nothing Then Throw New NullPointerException("Null completion strings not accepted.")
				Me.value = value
				Me.message = message
			End Sub

			Public Overridable Property value As String Implements Completion.getValue
				Get
					Return value
				End Get
			End Property


			Public Overridable Property message As String Implements Completion.getMessage
				Get
					Return message
				End Get
			End Property

			Public Overrides Function ToString() As String
				Return "[""" & value & """, """ & message & """]"
			End Function
			' Default equals and hashCode are fine.
		End Class

		''' <summary>
		''' Returns a completion of the value and message.
		''' </summary>
		''' <param name="value"> the text of the completion </param>
		''' <param name="message"> a message about the completion </param>
		''' <returns> a completion of the provided value and message </returns>
		Public Shared Function [of](ByVal value As String, ByVal message As String) As Completion
			Return New SimpleCompletion(value, message)
		End Function

		''' <summary>
		''' Returns a completion of the value and an empty message
		''' </summary>
		''' <param name="value"> the text of the completion </param>
		''' <returns> a completion of the value and an empty message </returns>
		Public Shared Function [of](ByVal value As String) As Completion
			Return New SimpleCompletion(value, "")
		End Function
	End Class

End Namespace