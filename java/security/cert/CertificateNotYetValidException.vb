'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.security.cert

	''' <summary>
	''' Certificate is not yet valid exception. This is thrown whenever
	''' the current {@code Date} or the specified {@code Date}
	''' is before the {@code notBefore} date/time in the Certificate
	''' validity period.
	''' 
	''' @author Hemma Prafullchandra
	''' </summary>
	Public Class CertificateNotYetValidException
		Inherits CertificateException

		Friend Shadows Const serialVersionUID As Long = 4355919900041064702L

		''' <summary>
		''' Constructs a CertificateNotYetValidException with no detail message. A
		''' detail message is a String that describes this particular
		''' exception.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a CertificateNotYetValidException with the specified detail
		''' message. A detail message is a String that describes this
		''' particular exception.
		''' </summary>
		''' <param name="message"> the detail message. </param>
		Public Sub New(  message As String)
			MyBase.New(message)
		End Sub
	End Class

End Namespace