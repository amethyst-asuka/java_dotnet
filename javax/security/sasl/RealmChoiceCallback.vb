'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.security.sasl


	''' <summary>
	''' This callback is used by {@code SaslClient} and {@code SaslServer}
	''' to obtain a realm given a list of realm choices.
	'''  
	''' @since 1.5
	'''  
	''' @author Rosanna Lee
	''' @author Rob Weltman
	''' </summary>
	Public Class RealmChoiceCallback
		Inherits javax.security.auth.callback.ChoiceCallback

		''' <summary>
		''' Constructs a {@code RealmChoiceCallback} with a prompt, a list of
		''' choices and a default choice.
		''' </summary>
		''' <param name="prompt"> the non-null prompt to use to request the realm. </param>
		''' <param name="choices"> the non-null list of realms to choose from. </param>
		''' <param name="defaultChoice"> the choice to be used as the default choice
		''' when the list of choices is displayed. It is an index into
		''' the {@code choices} array. </param>
		''' <param name="multiple"> true if multiple choices allowed; false otherwise </param>
		''' <exception cref="IllegalArgumentException"> If {@code prompt} is null or the empty string,
		''' if {@code choices} has a length of 0, if any element from
		''' {@code choices} is null or empty, or if {@code defaultChoice}
		''' does not fall within the array boundary of {@code choices} </exception>
		Public Sub New(ByVal prompt As String, ByVal choices As String(), ByVal defaultChoice As Integer, ByVal multiple As Boolean)
			MyBase.New(prompt, choices, defaultChoice, multiple)
		End Sub

		Private Const serialVersionUID As Long = -8588141348846281332L
	End Class

End Namespace