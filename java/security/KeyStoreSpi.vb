Imports javax.security.auth.callback

'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.security




	''' <summary>
	''' This class defines the <i>Service Provider Interface</i> (<b>SPI</b>)
	''' for the {@code KeyStore} class.
	''' All the abstract methods in this class must be implemented by each
	''' cryptographic service provider who wishes to supply the implementation
	''' of a keystore for a particular keystore type.
	''' 
	''' @author Jan Luehe
	''' 
	''' </summary>
	''' <seealso cref= KeyStore
	''' 
	''' @since 1.2 </seealso>

	Public MustInherit Class KeyStoreSpi

		''' <summary>
		''' Returns the key associated with the given alias, using the given
		''' password to recover it.  The key must have been associated with
		''' the alias by a call to {@code setKeyEntry},
		''' or by a call to {@code setEntry} with a
		''' {@code PrivateKeyEntry} or {@code SecretKeyEntry}.
		''' </summary>
		''' <param name="alias"> the alias name </param>
		''' <param name="password"> the password for recovering the key
		''' </param>
		''' <returns> the requested key, or null if the given alias does not exist
		''' or does not identify a key-related entry.
		''' </returns>
		''' <exception cref="NoSuchAlgorithmException"> if the algorithm for recovering the
		''' key cannot be found </exception>
		''' <exception cref="UnrecoverableKeyException"> if the key cannot be recovered
		''' (e.g., the given password is wrong). </exception>
		Public MustOverride Function engineGetKey(  [alias] As String,   password As Char()) As Key

		''' <summary>
		''' Returns the certificate chain associated with the given alias.
		''' The certificate chain must have been associated with the alias
		''' by a call to {@code setKeyEntry},
		''' or by a call to {@code setEntry} with a
		''' {@code PrivateKeyEntry}.
		''' </summary>
		''' <param name="alias"> the alias name
		''' </param>
		''' <returns> the certificate chain (ordered with the user's certificate first
		''' and the root certificate authority last), or null if the given alias
		''' does not exist or does not contain a certificate chain </returns>
		Public MustOverride Function engineGetCertificateChain(  [alias] As String) As java.security.cert.Certificate()

		''' <summary>
		''' Returns the certificate associated with the given alias.
		''' 
		''' <p> If the given alias name identifies an entry
		''' created by a call to {@code setCertificateEntry},
		''' or created by a call to {@code setEntry} with a
		''' {@code TrustedCertificateEntry},
		''' then the trusted certificate contained in that entry is returned.
		''' 
		''' <p> If the given alias name identifies an entry
		''' created by a call to {@code setKeyEntry},
		''' or created by a call to {@code setEntry} with a
		''' {@code PrivateKeyEntry},
		''' then the first element of the certificate chain in that entry
		''' (if a chain exists) is returned.
		''' </summary>
		''' <param name="alias"> the alias name
		''' </param>
		''' <returns> the certificate, or null if the given alias does not exist or
		''' does not contain a certificate. </returns>
		Public MustOverride Function engineGetCertificate(  [alias] As String) As java.security.cert.Certificate

		''' <summary>
		''' Returns the creation date of the entry identified by the given alias.
		''' </summary>
		''' <param name="alias"> the alias name
		''' </param>
		''' <returns> the creation date of this entry, or null if the given alias does
		''' not exist </returns>
		Public MustOverride Function engineGetCreationDate(  [alias] As String) As Date

		''' <summary>
		''' Assigns the given key to the given alias, protecting it with the given
		''' password.
		''' 
		''' <p>If the given key is of type {@code java.security.PrivateKey},
		''' it must be accompanied by a certificate chain certifying the
		''' corresponding public key.
		''' 
		''' <p>If the given alias already exists, the keystore information
		''' associated with it is overridden by the given key (and possibly
		''' certificate chain).
		''' </summary>
		''' <param name="alias"> the alias name </param>
		''' <param name="key"> the key to be associated with the alias </param>
		''' <param name="password"> the password to protect the key </param>
		''' <param name="chain"> the certificate chain for the corresponding public
		''' key (only required if the given key is of type
		''' {@code java.security.PrivateKey}).
		''' </param>
		''' <exception cref="KeyStoreException"> if the given key cannot be protected, or
		''' this operation fails for some other reason </exception>
		Public MustOverride Sub engineSetKeyEntry(  [alias] As String,   key As Key,   password As Char(),   chain As java.security.cert.Certificate())

		''' <summary>
		''' Assigns the given key (that has already been protected) to the given
		''' alias.
		''' 
		''' <p>If the protected key is of type
		''' {@code java.security.PrivateKey},
		''' it must be accompanied by a certificate chain certifying the
		''' corresponding public key.
		''' 
		''' <p>If the given alias already exists, the keystore information
		''' associated with it is overridden by the given key (and possibly
		''' certificate chain).
		''' </summary>
		''' <param name="alias"> the alias name </param>
		''' <param name="key"> the key (in protected format) to be associated with the alias </param>
		''' <param name="chain"> the certificate chain for the corresponding public
		''' key (only useful if the protected key is of type
		''' {@code java.security.PrivateKey}).
		''' </param>
		''' <exception cref="KeyStoreException"> if this operation fails. </exception>
		Public MustOverride Sub engineSetKeyEntry(  [alias] As String,   key As SByte(),   chain As java.security.cert.Certificate())

		''' <summary>
		''' Assigns the given certificate to the given alias.
		''' 
		''' <p> If the given alias identifies an existing entry
		''' created by a call to {@code setCertificateEntry},
		''' or created by a call to {@code setEntry} with a
		''' {@code TrustedCertificateEntry},
		''' the trusted certificate in the existing entry
		''' is overridden by the given certificate.
		''' </summary>
		''' <param name="alias"> the alias name </param>
		''' <param name="cert"> the certificate
		''' </param>
		''' <exception cref="KeyStoreException"> if the given alias already exists and does
		''' not identify an entry containing a trusted certificate,
		''' or this operation fails for some other reason. </exception>
		Public MustOverride Sub engineSetCertificateEntry(  [alias] As String,   cert As java.security.cert.Certificate)

		''' <summary>
		''' Deletes the entry identified by the given alias from this keystore.
		''' </summary>
		''' <param name="alias"> the alias name
		''' </param>
		''' <exception cref="KeyStoreException"> if the entry cannot be removed. </exception>
		Public MustOverride Sub engineDeleteEntry(  [alias] As String)

		''' <summary>
		''' Lists all the alias names of this keystore.
		''' </summary>
		''' <returns> enumeration of the alias names </returns>
		Public MustOverride Function engineAliases() As Enumeration(Of String)

		''' <summary>
		''' Checks if the given alias exists in this keystore.
		''' </summary>
		''' <param name="alias"> the alias name
		''' </param>
		''' <returns> true if the alias exists, false otherwise </returns>
		Public MustOverride Function engineContainsAlias(  [alias] As String) As Boolean

		''' <summary>
		''' Retrieves the number of entries in this keystore.
		''' </summary>
		''' <returns> the number of entries in this keystore </returns>
		Public MustOverride Function engineSize() As Integer

		''' <summary>
		''' Returns true if the entry identified by the given alias
		''' was created by a call to {@code setKeyEntry},
		''' or created by a call to {@code setEntry} with a
		''' {@code PrivateKeyEntry} or a {@code SecretKeyEntry}.
		''' </summary>
		''' <param name="alias"> the alias for the keystore entry to be checked
		''' </param>
		''' <returns> true if the entry identified by the given alias is a
		''' key-related, false otherwise. </returns>
		Public MustOverride Function engineIsKeyEntry(  [alias] As String) As Boolean

		''' <summary>
		''' Returns true if the entry identified by the given alias
		''' was created by a call to {@code setCertificateEntry},
		''' or created by a call to {@code setEntry} with a
		''' {@code TrustedCertificateEntry}.
		''' </summary>
		''' <param name="alias"> the alias for the keystore entry to be checked
		''' </param>
		''' <returns> true if the entry identified by the given alias contains a
		''' trusted certificate, false otherwise. </returns>
		Public MustOverride Function engineIsCertificateEntry(  [alias] As String) As Boolean

		''' <summary>
		''' Returns the (alias) name of the first keystore entry whose certificate
		''' matches the given certificate.
		''' 
		''' <p>This method attempts to match the given certificate with each
		''' keystore entry. If the entry being considered was
		''' created by a call to {@code setCertificateEntry},
		''' or created by a call to {@code setEntry} with a
		''' {@code TrustedCertificateEntry},
		''' then the given certificate is compared to that entry's certificate.
		''' 
		''' <p> If the entry being considered was
		''' created by a call to {@code setKeyEntry},
		''' or created by a call to {@code setEntry} with a
		''' {@code PrivateKeyEntry},
		''' then the given certificate is compared to the first
		''' element of that entry's certificate chain.
		''' </summary>
		''' <param name="cert"> the certificate to match with.
		''' </param>
		''' <returns> the alias name of the first entry with matching certificate,
		''' or null if no such entry exists in this keystore. </returns>
		Public MustOverride Function engineGetCertificateAlias(  cert As java.security.cert.Certificate) As String

		''' <summary>
		''' Stores this keystore to the given output stream, and protects its
		''' integrity with the given password.
		''' </summary>
		''' <param name="stream"> the output stream to which this keystore is written. </param>
		''' <param name="password"> the password to generate the keystore integrity check
		''' </param>
		''' <exception cref="IOException"> if there was an I/O problem with data </exception>
		''' <exception cref="NoSuchAlgorithmException"> if the appropriate data integrity
		''' algorithm could not be found </exception>
		''' <exception cref="CertificateException"> if any of the certificates included in
		''' the keystore data could not be stored </exception>
		Public MustOverride Sub engineStore(  stream As OutputStream,   password As Char())

		''' <summary>
		''' Stores this keystore using the given
		''' {@code KeyStore.LoadStoreParmeter}.
		''' </summary>
		''' <param name="param"> the {@code KeyStore.LoadStoreParmeter}
		'''          that specifies how to store the keystore,
		'''          which may be {@code null}
		''' </param>
		''' <exception cref="IllegalArgumentException"> if the given
		'''          {@code KeyStore.LoadStoreParmeter}
		'''          input is not recognized </exception>
		''' <exception cref="IOException"> if there was an I/O problem with data </exception>
		''' <exception cref="NoSuchAlgorithmException"> if the appropriate data integrity
		'''          algorithm could not be found </exception>
		''' <exception cref="CertificateException"> if any of the certificates included in
		'''          the keystore data could not be stored
		''' 
		''' @since 1.5 </exception>
		Public Overridable Sub engineStore(  param As KeyStore.LoadStoreParameter)
			Throw New UnsupportedOperationException
		End Sub

		''' <summary>
		''' Loads the keystore from the given input stream.
		''' 
		''' <p>A password may be given to unlock the keystore
		''' (e.g. the keystore resides on a hardware token device),
		''' or to check the integrity of the keystore data.
		''' If a password is not given for integrity checking,
		''' then integrity checking is not performed.
		''' </summary>
		''' <param name="stream"> the input stream from which the keystore is loaded,
		''' or {@code null} </param>
		''' <param name="password"> the password used to check the integrity of
		''' the keystore, the password used to unlock the keystore,
		''' or {@code null}
		''' </param>
		''' <exception cref="IOException"> if there is an I/O or format problem with the
		''' keystore data, if a password is required but not given,
		''' or if the given password was incorrect. If the error is due to a
		''' wrong password, the <seealso cref="Throwable#getCause cause"/> of the
		''' {@code IOException} should be an
		''' {@code UnrecoverableKeyException} </exception>
		''' <exception cref="NoSuchAlgorithmException"> if the algorithm used to check
		''' the integrity of the keystore cannot be found </exception>
		''' <exception cref="CertificateException"> if any of the certificates in the
		''' keystore could not be loaded </exception>
		Public MustOverride Sub engineLoad(  stream As InputStream,   password As Char())

		''' <summary>
		''' Loads the keystore using the given
		''' {@code KeyStore.LoadStoreParameter}.
		''' 
		''' <p> Note that if this KeyStore has already been loaded, it is
		''' reinitialized and loaded again from the given parameter.
		''' </summary>
		''' <param name="param"> the {@code KeyStore.LoadStoreParameter}
		'''          that specifies how to load the keystore,
		'''          which may be {@code null}
		''' </param>
		''' <exception cref="IllegalArgumentException"> if the given
		'''          {@code KeyStore.LoadStoreParameter}
		'''          input is not recognized </exception>
		''' <exception cref="IOException"> if there is an I/O or format problem with the
		'''          keystore data. If the error is due to an incorrect
		'''         {@code ProtectionParameter} (e.g. wrong password)
		'''         the <seealso cref="Throwable#getCause cause"/> of the
		'''         {@code IOException} should be an
		'''         {@code UnrecoverableKeyException} </exception>
		''' <exception cref="NoSuchAlgorithmException"> if the algorithm used to check
		'''          the integrity of the keystore cannot be found </exception>
		''' <exception cref="CertificateException"> if any of the certificates in the
		'''          keystore could not be loaded
		''' 
		''' @since 1.5 </exception>
		Public Overridable Sub engineLoad(  param As KeyStore.LoadStoreParameter)

			If param Is Nothing Then
				engineLoad(CType(Nothing, InputStream), CType(Nothing, Char()))
				Return
			End If

			If TypeOf param Is KeyStore.SimpleLoadStoreParameter Then
				Dim protection As ProtectionParameter = param.protectionParameter
				Dim password As Char()
				If TypeOf protection Is PasswordProtection Then
					password = CType(protection, PasswordProtection).password
				ElseIf TypeOf protection Is CallbackHandlerProtection Then
					Dim handler As CallbackHandler = CType(protection, CallbackHandlerProtection).callbackHandler
					Dim callback As New PasswordCallback("Password: ", False)
					Try
						handler.handle(New Callback() {callback})
					Catch e As UnsupportedCallbackException
						Throw New NoSuchAlgorithmException("Could not obtain password", e)
					End Try
					password = callback.password
					callback.clearPassword()
					If password Is Nothing Then Throw New NoSuchAlgorithmException("No password provided")
				Else
					Throw New NoSuchAlgorithmException("ProtectionParameter must" & " be PasswordProtection or CallbackHandlerProtection")
				End If
				engineLoad(Nothing, password)
				Return
			End If

			Throw New UnsupportedOperationException
		End Sub

		''' <summary>
		''' Gets a {@code KeyStore.Entry} for the specified alias
		''' with the specified protection parameter.
		''' </summary>
		''' <param name="alias"> get the {@code KeyStore.Entry} for this alias </param>
		''' <param name="protParam"> the {@code ProtectionParameter}
		'''          used to protect the {@code Entry},
		'''          which may be {@code null}
		''' </param>
		''' <returns> the {@code KeyStore.Entry} for the specified alias,
		'''          or {@code null} if there is no such entry
		''' </returns>
		''' <exception cref="KeyStoreException"> if the operation failed </exception>
		''' <exception cref="NoSuchAlgorithmException"> if the algorithm for recovering the
		'''          entry cannot be found </exception>
		''' <exception cref="UnrecoverableEntryException"> if the specified
		'''          {@code protParam} were insufficient or invalid </exception>
		''' <exception cref="UnrecoverableKeyException"> if the entry is a
		'''          {@code PrivateKeyEntry} or {@code SecretKeyEntry}
		'''          and the specified {@code protParam} does not contain
		'''          the information needed to recover the key (e.g. wrong password)
		''' 
		''' @since 1.5 </exception>
		Public Overridable Function engineGetEntry(  [alias] As String,   protParam As KeyStore.ProtectionParameter) As KeyStore.Entry

			If Not engineContainsAlias([alias]) Then Return Nothing

			If protParam Is Nothing Then
				If engineIsCertificateEntry([alias]) Then
					Return New KeyStore.TrustedCertificateEntry(engineGetCertificate([alias]))
				Else
					Throw New UnrecoverableKeyException("requested entry requires a password")
				End If
			End If

			If TypeOf protParam Is KeyStore.PasswordProtection Then
				If engineIsCertificateEntry([alias]) Then
					Throw New UnsupportedOperationException("trusted certificate entries are not password-protected")
				ElseIf engineIsKeyEntry([alias]) Then
					Dim pp As KeyStore.PasswordProtection = CType(protParam, KeyStore.PasswordProtection)
					Dim password As Char() = pp.password

					Dim key As Key = engineGetKey([alias], password)
					If TypeOf key Is PrivateKey Then
						Dim chain As java.security.cert.Certificate() = engineGetCertificateChain([alias])
						Return New KeyStore.PrivateKeyEntry(CType(key, PrivateKey), chain)
					ElseIf TypeOf key Is javax.crypto.SecretKey Then
						Return New KeyStore.SecretKeyEntry(CType(key, javax.crypto.SecretKey))
					End If
				End If
			End If

			Throw New UnsupportedOperationException
		End Function

		''' <summary>
		''' Saves a {@code KeyStore.Entry} under the specified alias.
		''' The specified protection parameter is used to protect the
		''' {@code Entry}.
		''' 
		''' <p> If an entry already exists for the specified alias,
		''' it is overridden.
		''' </summary>
		''' <param name="alias"> save the {@code KeyStore.Entry} under this alias </param>
		''' <param name="entry"> the {@code Entry} to save </param>
		''' <param name="protParam"> the {@code ProtectionParameter}
		'''          used to protect the {@code Entry},
		'''          which may be {@code null}
		''' </param>
		''' <exception cref="KeyStoreException"> if this operation fails
		''' 
		''' @since 1.5 </exception>
		Public Overridable Sub engineSetEntry(  [alias] As String,   entry As KeyStore.Entry,   protParam As KeyStore.ProtectionParameter)

			' get password
			If protParam IsNot Nothing AndAlso Not(TypeOf protParam Is KeyStore.PasswordProtection) Then Throw New KeyStoreException("unsupported protection parameter")
			Dim pProtect As KeyStore.PasswordProtection = Nothing
			If protParam IsNot Nothing Then pProtect = CType(protParam, KeyStore.PasswordProtection)

			' set entry
			If TypeOf entry Is KeyStore.TrustedCertificateEntry Then
				If protParam IsNot Nothing AndAlso pProtect.password IsNot Nothing Then
					' pre-1.5 style setCertificateEntry did not allow password
					Throw New KeyStoreException("trusted certificate entries are not password-protected")
				Else
					Dim tce As KeyStore.TrustedCertificateEntry = CType(entry, KeyStore.TrustedCertificateEntry)
					engineSetCertificateEntry([alias], tce.trustedCertificate)
					Return
				End If
			ElseIf TypeOf entry Is KeyStore.PrivateKeyEntry Then
				If pProtect Is Nothing OrElse pProtect.password Is Nothing Then
					' pre-1.5 style setKeyEntry required password
					Throw New KeyStoreException("non-null password required to create PrivateKeyEntry")
				Else
					engineSetKeyEntry([alias], CType(entry, KeyStore.PrivateKeyEntry).privateKey, pProtect.password, CType(entry, KeyStore.PrivateKeyEntry).certificateChain)
					Return
				End If
			ElseIf TypeOf entry Is KeyStore.SecretKeyEntry Then
				If pProtect Is Nothing OrElse pProtect.password Is Nothing Then
					' pre-1.5 style setKeyEntry required password
					Throw New KeyStoreException("non-null password required to create SecretKeyEntry")
				Else
					engineSetKeyEntry([alias], CType(entry, KeyStore.SecretKeyEntry).secretKey, pProtect.password, CType(Nothing, java.security.cert.Certificate()))
					Return
				End If
			End If

			Throw New KeyStoreException("unsupported entry type: " & entry.GetType().name)
		End Sub

		''' <summary>
		''' Determines if the keystore {@code Entry} for the specified
		''' {@code alias} is an instance or subclass of the specified
		''' {@code entryClass}.
		''' </summary>
		''' <param name="alias"> the alias name </param>
		''' <param name="entryClass"> the entry class
		''' </param>
		''' <returns> true if the keystore {@code Entry} for the specified
		'''          {@code alias} is an instance or subclass of the
		'''          specified {@code entryClass}, false otherwise
		''' 
		''' @since 1.5 </returns>
		Public Overridable Function engineEntryInstanceOf(  [alias] As String,   entryClass As [Class]) As Boolean
			If entryClass Is GetType(KeyStore.TrustedCertificateEntry) Then Return engineIsCertificateEntry([alias])
			If entryClass Is GetType(KeyStore.PrivateKeyEntry) Then Return engineIsKeyEntry([alias]) AndAlso engineGetCertificate([alias]) IsNot Nothing
			If entryClass Is GetType(KeyStore.SecretKeyEntry) Then Return engineIsKeyEntry([alias]) AndAlso engineGetCertificate([alias]) Is Nothing
			Return False
		End Function
	End Class

End Namespace