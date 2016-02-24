'
' * Copyright (c) 2010, Oracle and/or its affiliates. All rights reserved.
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

'
' *******************************************************************************
' * Copyright (C) 2009-2010, International Business Machines Corporation and    *
' * others. All Rights Reserved.                                                *
' *******************************************************************************
' 

Namespace java.util

	''' <summary>
	''' Thrown by methods in <seealso cref="Locale"/> and <seealso cref="Locale.Builder"/> to
	''' indicate that an argument is not a well-formed BCP 47 tag.
	''' </summary>
	''' <seealso cref= Locale
	''' @since 1.7 </seealso>
	Public Class IllformedLocaleException
		Inherits RuntimeException

		Private Shadows Const serialVersionUID As Long = -5245986824925681401L

		Private _errIdx As Integer = -1

		''' <summary>
		''' Constructs a new <code>IllformedLocaleException</code> with no
		''' detail message and -1 as the error index.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a new <code>IllformedLocaleException</code> with the
		''' given message and -1 as the error index.
		''' </summary>
		''' <param name="message"> the message </param>
		Public Sub New(ByVal message As String)
			MyBase.New(message)
		End Sub

		''' <summary>
		''' Constructs a new <code>IllformedLocaleException</code> with the
		''' given message and the error index.  The error index is the approximate
		''' offset from the start of the ill-formed value to the point where the
		''' parse first detected an error.  A negative error index value indicates
		''' either the error index is not applicable or unknown.
		''' </summary>
		''' <param name="message"> the message </param>
		''' <param name="errorIndex"> the index </param>
		Public Sub New(ByVal message As String, ByVal errorIndex As Integer)
			MyBase.New(message + (If(errorIndex < 0, "", " [at index " & errorIndex & "]")))
			_errIdx = errorIndex
		End Sub

		''' <summary>
		''' Returns the index where the error was found. A negative value indicates
		''' either the error index is not applicable or unknown.
		''' </summary>
		''' <returns> the error index </returns>
		Public Overridable Property errorIndex As Integer
			Get
				Return _errIdx
			End Get
		End Property
	End Class

End Namespace