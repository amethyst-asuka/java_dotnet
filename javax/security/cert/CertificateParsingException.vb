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
	''' Certificate Parsing Exception. This is thrown whenever
	''' invalid DER encoded certificate is parsed or unsupported DER features
	''' are found in the Certificate.
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
	Public Class CertificateParsingException
		Inherits CertificateException

		Private Const serialVersionUID As Long = -8449352422951136229L

		''' <summary>
		''' Constructs a CertificateParsingException with no detail message. A
		''' detail message is a String that describes this particular
		''' exception.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a CertificateParsingException with the specified detail
		''' message. A detail message is a String that describes this
		''' particular exception.
		''' </summary>
		''' <param name="message"> the detail message. </param>
		Public Sub New(ByVal message As String)
			MyBase.New(message)
		End Sub
	End Class

End Namespace