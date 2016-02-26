Imports System

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

Namespace javax.security.sasl


	''' <summary>
	''' This callback is used by {@code SaslServer} to determine whether
	''' one entity (identified by an authenticated authentication id)
	''' can act on
	''' behalf of another entity (identified by an authorization id).
	'''  
	''' @since 1.5
	'''  
	''' @author Rosanna Lee
	''' @author Rob Weltman
	''' </summary>
	<Serializable> _
	Public Class AuthorizeCallback
		Implements javax.security.auth.callback.Callback

		''' <summary>
		''' The (authenticated) authentication id to check.
		''' @serial
		''' </summary>
		Private authenticationID As String

		''' <summary>
		''' The authorization id to check.
		''' @serial
		''' </summary>
		Private authorizationID As String

		''' <summary>
		''' The id of the authorized entity. If null, the id of
		''' the authorized entity is authorizationID.
		''' @serial
		''' </summary>
		Private authorizedID As String

		''' <summary>
		''' A flag indicating whether the authentication id is allowed to
		''' act on behalf of the authorization id.
		''' @serial
		''' </summary>
		Private authorized As Boolean

		''' <summary>
		''' Constructs an instance of {@code AuthorizeCallback}.
		''' </summary>
		''' <param name="authnID">   The (authenticated) authentication id. </param>
		''' <param name="authzID">   The authorization id. </param>
		Public Sub New(ByVal authnID As String, ByVal authzID As String)
			authenticationID = authnID
			authorizationID = authzID
		End Sub

		''' <summary>
		''' Returns the authentication id to check. </summary>
		''' <returns> The authentication id to check. </returns>
		Public Overridable Property authenticationID As String
			Get
				Return authenticationID
			End Get
		End Property

		''' <summary>
		''' Returns the authorization id to check. </summary>
		''' <returns> The authentication id to check. </returns>
		Public Overridable Property authorizationID As String
			Get
				Return authorizationID
			End Get
		End Property

		''' <summary>
		''' Determines whether the authentication id is allowed to
		''' act on behalf of the authorization id.
		''' </summary>
		''' <returns> {@code true} if authorization is allowed; {@code false} otherwise </returns>
		''' <seealso cref= #setAuthorized(boolean) </seealso>
		''' <seealso cref= #getAuthorizedID() </seealso>
		Public Overridable Property authorized As Boolean
			Get
				Return authorized
			End Get
			Set(ByVal ok As Boolean)
				authorized = ok
			End Set
		End Property


		''' <summary>
		''' Returns the id of the authorized user. </summary>
		''' <returns> The id of the authorized user. {@code null} means the
		''' authorization failed. </returns>
		''' <seealso cref= #setAuthorized(boolean) </seealso>
		''' <seealso cref= #setAuthorizedID(java.lang.String) </seealso>
		Public Overridable Property authorizedID As String
			Get
				If Not authorized Then Return Nothing
				Return If(authorizedID Is Nothing, authorizationID, authorizedID)
			End Get
			Set(ByVal id As String)
				authorizedID = id
			End Set
		End Property


		Private Const serialVersionUID As Long = -2353344186490470805L
	End Class

End Namespace