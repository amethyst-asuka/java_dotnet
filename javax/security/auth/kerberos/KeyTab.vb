'
' * Copyright (c) 2011, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.security.auth.kerberos


	''' <summary>
	''' This class encapsulates a keytab file.
	''' <p>
	''' A Kerberos JAAS login module that obtains long term secret keys from a
	''' keytab file should use this class. The login module will store
	''' an instance of this class in the private credential set of a
	''' <seealso cref="javax.security.auth.Subject Subject"/> during the commit phase of the
	''' authentication process.
	''' <p>
	''' If a {@code KeyTab} object is obtained from <seealso cref="#getUnboundInstance()"/>
	''' or <seealso cref="#getUnboundInstance(java.io.File)"/>, it is unbound and thus can be
	''' used by any service principal. Otherwise, if it's obtained from
	''' <seealso cref="#getInstance(KerberosPrincipal)"/> or
	''' <seealso cref="#getInstance(KerberosPrincipal, java.io.File)"/>, it is bound to the
	''' specific service principal and can only be used by it.
	''' <p>
	''' Please note the constructors <seealso cref="#getInstance()"/> and
	''' <seealso cref="#getInstance(java.io.File)"/> were created when there was no support
	''' for unbound keytabs. These methods should not be used anymore. An object
	''' created with either of these methods are considered to be bound to an
	''' unknown principal, which means, its <seealso cref="#isBound()"/> returns true and
	''' <seealso cref="#getPrincipal()"/> returns null.
	''' <p>
	''' It might be necessary for the application to be granted a
	''' {@link javax.security.auth.PrivateCredentialPermission
	''' PrivateCredentialPermission} if it needs to access the KeyTab
	''' instance from a Subject. This permission is not needed when the
	''' application depends on the default JGSS Kerberos mechanism to access the
	''' KeyTab. In that case, however, the application will need an appropriate
	''' <seealso cref="javax.security.auth.kerberos.ServicePermission ServicePermission"/>.
	''' <p>
	''' The keytab file format is described at
	''' <a href="http://www.ioplex.com/utilities/keytab.txt">
	''' http://www.ioplex.com/utilities/keytab.txt</a>.
	''' <p>
	''' @since 1.7
	''' </summary>
	Public NotInheritable Class KeyTab

	'    
	'     * Impl notes:
	'     *
	'     * This class is only a name, a permanent link to the keytab source
	'     * (can be missing). Itself has no content. In order to read content,
	'     * take a snapshot and read from it.
	'     *
	'     * The snapshot is of type sun.security.krb5.internal.ktab.KeyTab, which
	'     * contains the content of the keytab file when the snapshot is taken.
	'     * Itself has no refresh function and mostly an immutable class (except
	'     * for the create/add/save methods only used by the ktab command).
	'     

		' Source, null if using the default one. Note that the default name
		' is maintained in snapshot, this field is never "resolved".
		Private ReadOnly file As java.io.File

		' Bound user: normally from the "principal" value in a JAAS krb5
		' login conf. Will be null if it's "*".
		Private ReadOnly princ As KerberosPrincipal

		Private ReadOnly bound As Boolean

		' Set up JavaxSecurityAuthKerberosAccess in KerberosSecrets
		Shared Sub New()
			sun.security.krb5.KerberosSecrets.javaxSecurityAuthKerberosAccess = New JavaxSecurityAuthKerberosAccessImpl
		End Sub

		Private Sub New(ByVal princ As KerberosPrincipal, ByVal file As java.io.File, ByVal bound As Boolean)
			Me.princ = princ
			Me.file = file
			Me.bound = bound
		End Sub

		''' <summary>
		''' Returns a {@code KeyTab} instance from a {@code File} object
		''' that is bound to an unknown service principal.
		''' <p>
		''' The result of this method is never null. This method only associates
		''' the returned {@code KeyTab} object with the file and does not read it.
		''' <p>
		''' Developers should call <seealso cref="#getInstance(KerberosPrincipal,File)"/>
		''' when the bound service principal is known. </summary>
		''' <param name="file"> the keytab {@code File} object, must not be null </param>
		''' <returns> the keytab instance </returns>
		''' <exception cref="NullPointerException"> if the {@code file} argument is null </exception>
		Public Shared Function getInstance(ByVal file As java.io.File) As KeyTab
			If file Is Nothing Then Throw New NullPointerException("file must be non null")
			Return New KeyTab(Nothing, file, True)
		End Function

		''' <summary>
		''' Returns an unbound {@code KeyTab} instance from a {@code File}
		''' object.
		''' <p>
		''' The result of this method is never null. This method only associates
		''' the returned {@code KeyTab} object with the file and does not read it. </summary>
		''' <param name="file"> the keytab {@code File} object, must not be null </param>
		''' <returns> the keytab instance </returns>
		''' <exception cref="NullPointerException"> if the file argument is null
		''' @since 1.8 </exception>
		Public Shared Function getUnboundInstance(ByVal file As java.io.File) As KeyTab
			If file Is Nothing Then Throw New NullPointerException("file must be non null")
			Return New KeyTab(Nothing, file, False)
		End Function

		''' <summary>
		''' Returns a {@code KeyTab} instance from a {@code File} object
		''' that is bound to the specified service principal.
		''' <p>
		''' The result of this method is never null. This method only associates
		''' the returned {@code KeyTab} object with the file and does not read it. </summary>
		''' <param name="princ"> the bound service principal, must not be null </param>
		''' <param name="file"> the keytab {@code File} object, must not be null </param>
		''' <returns> the keytab instance </returns>
		''' <exception cref="NullPointerException"> if either of the arguments is null
		''' @since 1.8 </exception>
		Public Shared Function getInstance(ByVal princ As KerberosPrincipal, ByVal file As java.io.File) As KeyTab
			If princ Is Nothing Then Throw New NullPointerException("princ must be non null")
			If file Is Nothing Then Throw New NullPointerException("file must be non null")
			Return New KeyTab(princ, file, True)
		End Function

		''' <summary>
		''' Returns the default {@code KeyTab} instance that is bound
		''' to an unknown service principal.
		''' <p>
		''' The result of this method is never null. This method only associates
		''' the returned {@code KeyTab} object with the default keytab file and
		''' does not read it.
		''' <p>
		''' Developers should call <seealso cref="#getInstance(KerberosPrincipal)"/>
		''' when the bound service principal is known. </summary>
		''' <returns> the default keytab instance. </returns>
		Public Property Shared instance As KeyTab
			Get
				Return New KeyTab(Nothing, Nothing, True)
			End Get
		End Property

		''' <summary>
		''' Returns the default unbound {@code KeyTab} instance.
		''' <p>
		''' The result of this method is never null. This method only associates
		''' the returned {@code KeyTab} object with the default keytab file and
		''' does not read it. </summary>
		''' <returns> the default keytab instance
		''' @since 1.8 </returns>
		Public Property Shared unboundInstance As KeyTab
			Get
				Return New KeyTab(Nothing, Nothing, False)
			End Get
		End Property

		''' <summary>
		''' Returns the default {@code KeyTab} instance that is bound
		''' to the specified service principal.
		''' <p>
		''' The result of this method is never null. This method only associates
		''' the returned {@code KeyTab} object with the default keytab file and
		''' does not read it. </summary>
		''' <param name="princ"> the bound service principal, must not be null </param>
		''' <returns> the default keytab instance </returns>
		''' <exception cref="NullPointerException"> if {@code princ} is null
		''' @since 1.8 </exception>
		Public Shared Function getInstance(ByVal princ As KerberosPrincipal) As KeyTab
			If princ Is Nothing Then Throw New NullPointerException("princ must be non null")
			Return New KeyTab(princ, Nothing, True)
		End Function

		' Takes a snapshot of the keytab content. This method is called by
		' JavaxSecurityAuthKerberosAccessImpl so no more private
		Friend Function takeSnapshot() As sun.security.krb5.internal.ktab.KeyTab
			Try
				Return sun.security.krb5.internal.ktab.KeyTab.getInstance(file)
			Catch ace As java.security.AccessControlException
				If file IsNot Nothing Then
					' It's OK to show the name if caller specified it
					Throw ace
				Else
					Dim ace2 As New java.security.AccessControlException("Access to default keytab denied (modified exception)")
					ace2.stackTrace = ace.stackTrace
					Throw ace2
				End If
			End Try
		End Function

		''' <summary>
		''' Returns fresh keys for the given Kerberos principal.
		''' <p>
		''' Implementation of this method should make sure the returned keys match
		''' the latest content of the keytab file. The result is a newly created
		''' copy that can be modified by the caller without modifying the keytab
		''' object. The caller should <seealso cref="KerberosKey#destroy() destroy"/> the
		''' result keys after they are used.
		''' <p>
		''' Please note that the keytab file can be created after the
		''' {@code KeyTab} object is instantiated and its content may change over
		''' time. Therefore, an application should call this method only when it
		''' needs to use the keys. Any previous result from an earlier invocation
		''' could potentially be expired.
		''' <p>
		''' If there is any error (say, I/O error or format error)
		''' during the reading process of the KeyTab file, a saved result should be
		''' returned. If there is no saved result (say, this is the first time this
		''' method is called, or, all previous read attempts failed), an empty array
		''' should be returned. This can make sure the result is not drastically
		''' changed during the (probably slow) update of the keytab file.
		''' <p>
		''' Each time this method is called and the reading of the file succeeds
		''' with no exception (say, I/O error or file format error),
		''' the result should be saved for {@code principal}. The implementation can
		''' also save keys for other principals having keys in the same keytab object
		''' if convenient.
		''' <p>
		''' Any unsupported key read from the keytab is ignored and not included
		''' in the result.
		''' <p>
		''' If this keytab is bound to a specific principal, calling this method on
		''' another principal will return an empty array.
		''' </summary>
		''' <param name="principal"> the Kerberos principal, must not be null. </param>
		''' <returns> the keys (never null, may be empty) </returns>
		''' <exception cref="NullPointerException"> if the {@code principal}
		''' argument is null </exception>
		''' <exception cref="SecurityException"> if a security manager exists and the read
		''' access to the keytab file is not permitted </exception>
		Public Function getKeys(ByVal principal As KerberosPrincipal) As KerberosKey()
			Try
				If princ IsNot Nothing AndAlso (Not principal.Equals(princ)) Then Return New KerberosKey(){}
				Dim pn As New sun.security.krb5.PrincipalName(principal.name)
				Dim ___keys As sun.security.krb5.EncryptionKey() = takeSnapshot().readServiceKeys(pn)
				Dim kks As KerberosKey() = New KerberosKey(___keys.Length - 1){}
				For i As Integer = 0 To kks.Length - 1
					Dim tmp As Integer? = ___keys(i).keyVersionNumber
					kks(i) = New KerberosKey(principal, ___keys(i).bytes, ___keys(i).eType,If(tmp Is Nothing, 0, tmp))
					___keys(i).destroy()
				Next i
				Return kks
			Catch re As sun.security.krb5.RealmException
				Return New KerberosKey(){}
			End Try
		End Function

		Friend Function getEncryptionKeys(ByVal principal As sun.security.krb5.PrincipalName) As sun.security.krb5.EncryptionKey()
			Return takeSnapshot().readServiceKeys(principal)
		End Function

		''' <summary>
		''' Checks if the keytab file exists. Implementation of this method
		''' should make sure that the result matches the latest status of the
		''' keytab file.
		''' <p>
		''' The caller can use the result to determine if it should fallback to
		''' another mechanism to read the keys. </summary>
		''' <returns> true if the keytab file exists; false otherwise. </returns>
		''' <exception cref="SecurityException"> if a security manager exists and the read
		''' access to the keytab file is not permitted </exception>
		Public Function exists() As Boolean
			Return Not takeSnapshot().missing
		End Function

		Public Overrides Function ToString() As String
			Dim s As String = If(file Is Nothing, "Default keytab", file.ToString())
			If Not bound Then
				Return s
			ElseIf princ Is Nothing Then
				Return s & " for someone"
			Else
				Return s & " for " & princ
			End If
		End Function

		''' <summary>
		''' Returns a hashcode for this KeyTab.
		''' </summary>
		''' <returns> a hashCode() for the {@code KeyTab} </returns>
		Public Overrides Function GetHashCode() As Integer
			Return java.util.Objects.hash(file, princ, bound)
		End Function

		''' <summary>
		''' Compares the specified Object with this KeyTab for equality.
		''' Returns true if the given object is also a
		''' {@code KeyTab} and the two
		''' {@code KeyTab} instances are equivalent.
		''' </summary>
		''' <param name="other"> the Object to compare to </param>
		''' <returns> true if the specified object is equal to this KeyTab </returns>
		Public Overrides Function Equals(ByVal other As Object) As Boolean
			If other Is Me Then Return True

			If Not(TypeOf other Is KeyTab) Then Return False

			Dim otherKtab As KeyTab = CType(other, KeyTab)
			Return java.util.Objects.Equals(otherKtab.princ, princ) AndAlso java.util.Objects.Equals(otherKtab.file, file) AndAlso bound = otherKtab.bound
		End Function

		''' <summary>
		''' Returns the service principal this {@code KeyTab} object
		''' is bound to. Returns {@code null} if it's not bound.
		''' <p>
		''' Please note the deprecated constructors create a KeyTab object bound for
		''' some unknown principal. In this case, this method also returns null.
		''' User can call <seealso cref="#isBound()"/> to verify this case. </summary>
		''' <returns> the service principal
		''' @since 1.8 </returns>
		Public Property principal As KerberosPrincipal
			Get
				Return princ
			End Get
		End Property

		''' <summary>
		''' Returns if the keytab is bound to a principal </summary>
		''' <returns> if the keytab is bound to a principal
		''' @since 1.8 </returns>
		Public Property bound As Boolean
			Get
				Return bound
			End Get
		End Property
	End Class

End Namespace