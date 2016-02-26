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


Namespace javax.security.cert

	''' <summary>
	''' Certificate Expired Exception. This is thrown whenever the current
	''' {@code Date} or the specified {@code Date} is after the
	''' {@code notAfter} date/time specified in the validity period
	''' of the certificate.
	''' 
	''' <p><em>Note: The classes in the package {@code javax.security.cert}
	''' exist for compatibility with earlier versions of the
	''' Java Secure Sockets Extension (JSSE). New applications should instead
	''' use the standard Java SE certificate classes located in
	''' {@code java.security.cert}.</em></p>
	''' 
	''' @since 1.4
	''' @author Hemma Prafullchandra
	''' </summary>
	Public Class CertificateExpiredException
		Inherits CertificateException

		Private Const serialVersionUID As Long = 5091601212177261883L
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
		Public Sub New(ByVal message As String)
			MyBase.New(message)
		End Sub
	End Class

End Namespace