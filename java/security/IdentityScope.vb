Imports System
Imports System.Collections.Generic

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
	''' <p>This class represents a scope for identities. It is an Identity
	''' itself, and therefore has a name and can have a scope. It can also
	''' optionally have a public key and associated certificates.
	''' 
	''' <p>An IdentityScope can contain Identity objects of all kinds, including
	''' Signers. All types of Identity objects can be retrieved, added, and
	''' removed using the same methods. Note that it is possible, and in fact
	''' expected, that different types of identity scopes will
	''' apply different policies for their various operations on the
	''' various types of Identities.
	''' 
	''' <p>There is a one-to-one mapping between keys and identities, and
	''' there can only be one copy of one key per scope. For example, suppose
	''' <b>Acme Software, Inc</b> is a software publisher known to a user.
	''' Suppose it is an Identity, that is, it has a public key, and a set of
	''' associated certificates. It is named in the scope using the name
	''' "Acme Software". No other named Identity in the scope has the same
	''' public  key. Of course, none has the same name as well.
	''' </summary>
	''' <seealso cref= Identity </seealso>
	''' <seealso cref= Signer </seealso>
	''' <seealso cref= Principal </seealso>
	''' <seealso cref= Key
	''' 
	''' @author Benjamin Renaud
	''' </seealso>
	''' @deprecated This class is no longer used. Its functionality has been
	''' replaced by {@code java.security.KeyStore}, the
	''' {@code java.security.cert} package, and
	''' {@code java.security.Principal}. 
	<Obsolete("This class is no longer used. Its functionality has been")> _
	Public MustInherit Class IdentityScope
		Inherits Identity

		Private Const serialVersionUID As Long = -2337346281189773310L

		' The system's scope 
		Private Shared Shadows scope As IdentityScope

		' initialize the system scope
		Private Shared Sub initializeSystemScope()

			Dim classname As String = AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)

			If classname Is Nothing Then
				Return

			Else

				Try
					Type.GetType(classname)
				Catch e As ClassNotFoundException
					'Security.error("unable to establish a system scope from " +
					'             classname);
					Console.WriteLine(e.ToString())
					Console.Write(e.StackTrace)
				End Try
			End If
		End Sub

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements PrivilegedAction(Of T)

			Public Overridable Function run() As String
				Return Security.getProperty("system.scope")
			End Function
		End Class

		''' <summary>
		''' This constructor is used for serialization only and should not
		''' be used by subclasses.
		''' </summary>
		Protected Friend Sub New()
			Me.New("restoring...")
		End Sub

		''' <summary>
		''' Constructs a new identity scope with the specified name.
		''' </summary>
		''' <param name="name"> the scope name. </param>
		Public Sub New(ByVal name As String)
			MyBase.New(name)
		End Sub

		''' <summary>
		''' Constructs a new identity scope with the specified name and scope.
		''' </summary>
		''' <param name="name"> the scope name. </param>
		''' <param name="scope"> the scope for the new identity scope.
		''' </param>
		''' <exception cref="KeyManagementException"> if there is already an identity
		''' with the same name in the scope. </exception>
		Public Sub New(ByVal name As String, ByVal scope As IdentityScope)
			MyBase.New(name, scope)
		End Sub

		''' <summary>
		''' Returns the system's identity scope.
		''' </summary>
		''' <returns> the system's identity scope, or {@code null} if none has been
		'''         set.
		''' </returns>
		''' <seealso cref= #setSystemScope </seealso>
		Public Property Shared systemScope As IdentityScope
			Get
				If scope Is Nothing Then initializeSystemScope()
				Return scope
			End Get
			Set(ByVal scope As IdentityScope)
				check("setSystemScope")
				IdentityScope.scope = scope
			End Set
		End Property



		''' <summary>
		''' Returns the number of identities within this identity scope.
		''' </summary>
		''' <returns> the number of identities within this identity scope. </returns>
		Public MustOverride Function size() As Integer

		''' <summary>
		''' Returns the identity in this scope with the specified name (if any).
		''' </summary>
		''' <param name="name"> the name of the identity to be retrieved.
		''' </param>
		''' <returns> the identity named {@code name}, or null if there are
		''' no identities named {@code name} in this scope. </returns>
		Public MustOverride Function getIdentity(ByVal name As String) As Identity

		''' <summary>
		''' Retrieves the identity whose name is the same as that of the
		''' specified principal. (Note: Identity implements Principal.)
		''' </summary>
		''' <param name="principal"> the principal corresponding to the identity
		''' to be retrieved.
		''' </param>
		''' <returns> the identity whose name is the same as that of the
		''' principal, or null if there are no identities of the same name
		''' in this scope. </returns>
		Public Overridable Function getIdentity(ByVal principal As Principal) As Identity
			Return getIdentity(principal.name)
		End Function

		''' <summary>
		''' Retrieves the identity with the specified public key.
		''' </summary>
		''' <param name="key"> the public key for the identity to be returned.
		''' </param>
		''' <returns> the identity with the given key, or null if there are
		''' no identities in this scope with that key. </returns>
		Public MustOverride Function getIdentity(ByVal key As PublicKey) As Identity

		''' <summary>
		''' Adds an identity to this identity scope.
		''' </summary>
		''' <param name="identity"> the identity to be added.
		''' </param>
		''' <exception cref="KeyManagementException"> if the identity is not
		''' valid, a name conflict occurs, another identity has the same
		''' public key as the identity being added, or another exception
		''' occurs.  </exception>
		Public MustOverride Sub addIdentity(ByVal identity As Identity)

		''' <summary>
		''' Removes an identity from this identity scope.
		''' </summary>
		''' <param name="identity"> the identity to be removed.
		''' </param>
		''' <exception cref="KeyManagementException"> if the identity is missing,
		''' or another exception occurs. </exception>
		Public MustOverride Sub removeIdentity(ByVal identity As Identity)

		''' <summary>
		''' Returns an enumeration of all identities in this identity scope.
		''' </summary>
		''' <returns> an enumeration of all identities in this identity scope. </returns>
		Public MustOverride Function identities() As System.Collections.IEnumerator(Of Identity)

		''' <summary>
		''' Returns a string representation of this identity scope, including
		''' its name, its scope name, and the number of identities in this
		''' identity scope.
		''' </summary>
		''' <returns> a string representation of this identity scope. </returns>
		Public Overrides Function ToString() As String
			Return MyBase.ToString() & "[" & size() & "]"
		End Function

		Private Shared Sub check(ByVal directive As String)
			Dim security_Renamed As SecurityManager = System.securityManager
			If security_Renamed IsNot Nothing Then security_Renamed.checkSecurityAccess(directive)
		End Sub

	End Class

End Namespace