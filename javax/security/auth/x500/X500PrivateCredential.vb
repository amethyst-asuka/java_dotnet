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

Namespace javax.security.auth.x500


	''' <summary>
	''' <p> This class represents an {@code X500PrivateCredential}.
	''' It associates an X.509 certificate, corresponding private key and the
	''' KeyStore alias used to reference that exact key pair in the KeyStore.
	''' This enables looking up the private credentials for an X.500 principal
	''' in a subject.
	''' 
	''' </summary>
	Public NotInheritable Class X500PrivateCredential
		Implements javax.security.auth.Destroyable

		Private cert As java.security.cert.X509Certificate
		Private key As java.security.PrivateKey
		Private [alias] As String

		''' <summary>
		''' Creates an X500PrivateCredential that associates an X.509 certificate,
		''' a private key and the KeyStore alias.
		''' <p> </summary>
		''' <param name="cert"> X509Certificate </param>
		''' <param name="key">  PrivateKey for the certificate </param>
		''' <exception cref="IllegalArgumentException"> if either {@code cert} or
		''' {@code key} is null
		'''  </exception>

		Public Sub New(ByVal cert As java.security.cert.X509Certificate, ByVal key As java.security.PrivateKey)
			If cert Is Nothing OrElse key Is Nothing Then Throw New System.ArgumentException
			Me.cert = cert
			Me.key = key
			Me.alias=Nothing
		End Sub

		''' <summary>
		''' Creates an X500PrivateCredential that associates an X.509 certificate,
		''' a private key and the KeyStore alias.
		''' <p> </summary>
		''' <param name="cert"> X509Certificate </param>
		''' <param name="key">  PrivateKey for the certificate </param>
		''' <param name="alias"> KeyStore alias </param>
		''' <exception cref="IllegalArgumentException"> if either {@code cert},
		''' {@code key} or {@code alias} is null
		'''  </exception>
		Public Sub New(ByVal cert As java.security.cert.X509Certificate, ByVal key As java.security.PrivateKey, ByVal [alias] As String)
			If cert Is Nothing OrElse key Is Nothing OrElse [alias] Is Nothing Then Throw New System.ArgumentException
			Me.cert = cert
			Me.key = key
			Me.alias=[alias]
		End Sub

		''' <summary>
		''' Returns the X.509 certificate.
		''' <p> </summary>
		''' <returns> the X509Certificate </returns>

		Public Property certificate As java.security.cert.X509Certificate
			Get
				Return cert
			End Get
		End Property

		''' <summary>
		''' Returns the PrivateKey.
		''' <p> </summary>
		''' <returns> the PrivateKey </returns>
		Public Property privateKey As java.security.PrivateKey
			Get
				Return key
			End Get
		End Property

		''' <summary>
		''' Returns the KeyStore alias.
		''' <p> </summary>
		''' <returns> the KeyStore alias </returns>

		Public Property [alias] As String
			Get
				Return [alias]
			End Get
		End Property

		''' <summary>
		''' Clears the references to the X.509 certificate, private key and the
		''' KeyStore alias in this object.
		''' </summary>

		Public Sub destroy()
			cert = Nothing
			key = Nothing
			[alias] =Nothing
		End Sub

		''' <summary>
		''' Determines if the references to the X.509 certificate and private key
		''' in this object have been cleared.
		''' <p> </summary>
		''' <returns> true if X509Certificate and the PrivateKey are null
		'''  </returns>
		Public Property destroyed As Boolean
			Get
				Return cert Is Nothing AndAlso key Is Nothing AndAlso [alias] Is Nothing
			End Get
		End Property
	End Class

End Namespace