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
	''' to retrieve realm information.
	'''  
	''' @since 1.5
	'''  
	''' @author Rosanna Lee
	''' @author Rob Weltman
	''' </summary>
	Public Class RealmCallback
		Inherits javax.security.auth.callback.TextInputCallback

		''' <summary>
		''' Constructs a {@code RealmCallback} with a prompt.
		''' </summary>
		''' <param name="prompt"> The non-null prompt to use to request the realm information. </param>
		''' <exception cref="IllegalArgumentException"> If {@code prompt} is null or
		''' the empty string. </exception>
		Public Sub New(ByVal prompt As String)
			MyBase.New(prompt)
		End Sub

		''' <summary>
		''' Constructs a {@code RealmCallback} with a prompt and default
		''' realm information.
		''' </summary>
		''' <param name="prompt"> The non-null prompt to use to request the realm information. </param>
		''' <param name="defaultRealmInfo"> The non-null default realm information to use. </param>
		''' <exception cref="IllegalArgumentException"> If {@code prompt} is null or
		''' the empty string,
		''' or if {@code defaultRealm} is empty or null. </exception>
		Public Sub New(ByVal prompt As String, ByVal defaultRealmInfo As String)
			MyBase.New(prompt, defaultRealmInfo)
		End Sub

		Private Const serialVersionUID As Long = -4342673378785456908L
	End Class

End Namespace