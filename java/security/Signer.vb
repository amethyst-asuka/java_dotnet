Imports Microsoft.VisualBasic
Imports System

'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' This class is used to represent an Identity that can also digitally
	''' sign data.
	''' 
	''' <p>The management of a signer's private keys is an important and
	''' sensitive issue that should be handled by subclasses as appropriate
	''' to their intended use.
	''' </summary>
	''' <seealso cref= Identity
	''' 
	''' @author Benjamin Renaud
	''' </seealso>
	''' @deprecated This class is no longer used. Its functionality has been
	''' replaced by {@code java.security.KeyStore}, the
	''' {@code java.security.cert} package, and
	''' {@code java.security.Principal}. 
	<Obsolete("This class is no longer used. Its functionality has been")> _
	Public MustInherit Class Signer
		Inherits Identity

		Private Const serialVersionUID As Long = -1763464102261361480L

		''' <summary>
		''' The signer's private key.
		''' 
		''' @serial
		''' </summary>
		Private privateKey As PrivateKey

		''' <summary>
		''' Creates a signer. This constructor should only be used for
		''' serialization.
		''' </summary>
		Protected Friend Sub New()
			MyBase.New()
		End Sub


		''' <summary>
		''' Creates a signer with the specified identity name.
		''' </summary>
		''' <param name="name"> the identity name. </param>
		Public Sub New(  name As String)
			MyBase.New(name)
		End Sub

		''' <summary>
		''' Creates a signer with the specified identity name and scope.
		''' </summary>
		''' <param name="name"> the identity name.
		''' </param>
		''' <param name="scope"> the scope of the identity.
		''' </param>
		''' <exception cref="KeyManagementException"> if there is already an identity
		''' with the same name in the scope. </exception>
		Public Sub New(  name As String,   scope As IdentityScope)
			MyBase.New(name, scope)
		End Sub

		''' <summary>
		''' Returns this signer's private key.
		''' 
		''' <p>First, if there is a security manager, its {@code checkSecurityAccess}
		''' method is called with {@code "getSignerPrivateKey"}
		''' as its argument to see if it's ok to return the private key.
		''' </summary>
		''' <returns> this signer's private key, or null if the private key has
		''' not yet been set.
		''' </returns>
		''' <exception cref="SecurityException">  if a security manager exists and its
		''' {@code checkSecurityAccess} method doesn't allow
		''' returning the private key.
		''' </exception>
		''' <seealso cref= SecurityManager#checkSecurityAccess </seealso>
		Public Overridable Property privateKey As PrivateKey
			Get
				check("getSignerPrivateKey")
				Return privateKey
			End Get
		End Property

	   ''' <summary>
	   ''' Sets the key pair (public key and private key) for this signer.
	   '''  
	   ''' <p>First, if there is a security manager, its {@code checkSecurityAccess}
	   ''' method is called with {@code "setSignerKeyPair"}
	   ''' as its argument to see if it's ok to set the key pair.
	   ''' </summary>
	   ''' <param name="pair"> an initialized key pair.
	   ''' </param>
	   ''' <exception cref="InvalidParameterException"> if the key pair is not
	   ''' properly initialized. </exception>
	   ''' <exception cref="KeyException"> if the key pair cannot be set for any
	   ''' other reason. </exception>
	   ''' <exception cref="SecurityException">  if a security manager exists and its
	   ''' {@code checkSecurityAccess} method doesn't allow
	   ''' setting the key pair.
	   ''' </exception>
	   ''' <seealso cref= SecurityManager#checkSecurityAccess </seealso>
		Public Property keyPair As KeyPair
			Set(  pair As KeyPair)
				check("setSignerKeyPair")
				Dim pub As PublicKey = pair.public
				Dim priv As PrivateKey = pair.private
    
				If pub Is Nothing OrElse priv Is Nothing Then Throw New InvalidParameterException
				Try
					AccessController.doPrivileged(New PrivilegedExceptionActionAnonymousInnerClassHelper(Of T)
				Catch pae As PrivilegedActionException
					Throw CType(pae.exception, KeyManagementException)
				End Try
				privateKey = priv
			End Set
		End Property

		Private Class PrivilegedExceptionActionAnonymousInnerClassHelper(Of T)
			Implements PrivilegedExceptionAction(Of T)

			Public Overridable Function run() As Void
				outerInstance.publicKey = pub
				Return Nothing
			End Function
		End Class

		Friend Overrides Function printKeys() As String
			Dim keys As String = ""
			Dim publicKey_Renamed As PublicKey = publicKey
			If publicKey_Renamed IsNot Nothing AndAlso privateKey IsNot Nothing Then
				keys = vbTab & "public and private keys initialized"

			Else
				keys = vbTab & "no keys"
			End If
			Return keys
		End Function

		''' <summary>
		''' Returns a string of information about the signer.
		''' </summary>
		''' <returns> a string of information about the signer. </returns>
		Public Overrides Function ToString() As String
			Return "[Signer]" & MyBase.ToString()
		End Function

		Private Shared Sub check(  directive As String)
			Dim security_Renamed As SecurityManager = System.securityManager
			If security_Renamed IsNot Nothing Then security_Renamed.checkSecurityAccess(directive)
		End Sub

	End Class

End Namespace