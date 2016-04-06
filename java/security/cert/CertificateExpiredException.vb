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
	''' Certificate Expired Exception. This is thrown whenever the current
	''' {@code Date} or the specified {@code Date} is after the
	''' {@code notAfter} date/time specified in the validity period
	''' of the certificate.
	''' 
	''' @author Hemma Prafullchandra
	''' </summary>
	Public Class CertificateExpiredException
		Inherits CertificateException

		Private Shadows Const serialVersionUID As Long = 9071001339691533771L

		''' <summary>
		''' Constructs a CertificateExpiredException with no detail message. A
		''' detail message is a String that describes this particular
		''' exception.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a CertificateExpiredException with the specified detail
		''' message. A detail message is a String that describes this
		''' particular exception.
		''' </summary>
		''' <param name="message"> the detail message. </param>
		Public Sub New(  message As String)
			MyBase.New(message)
		End Sub
	End Class

End Namespace