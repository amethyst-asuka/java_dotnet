Imports System

'
' * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.security.auth.callback


	''' <summary>
	''' <p> Underlying security services instantiate and pass a
	''' {@code LanguageCallback} to the {@code handle}
	''' method of a {@code CallbackHandler} to retrieve the {@code Locale}
	''' used for localizing text.
	''' </summary>
	''' <seealso cref= javax.security.auth.callback.CallbackHandler </seealso>
	<Serializable> _
	Public Class LanguageCallback
		Implements Callback

		Private Const serialVersionUID As Long = 2019050433478903213L

		''' <summary>
		''' @serial
		''' @since 1.4
		''' </summary>
		Private locale As java.util.Locale

		''' <summary>
		''' Construct a {@code LanguageCallback}.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Set the retrieved {@code Locale}.
		''' 
		''' <p>
		''' </summary>
		''' <param name="locale"> the retrieved {@code Locale}.
		''' </param>
		''' <seealso cref= #getLocale </seealso>
		Public Overridable Property locale As java.util.Locale
			Set(ByVal locale As java.util.Locale)
				Me.locale = locale
			End Set
			Get
				Return locale
			End Get
		End Property

	End Class

End Namespace