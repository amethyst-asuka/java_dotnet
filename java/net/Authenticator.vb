Imports System.Runtime.CompilerServices

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

Namespace java.net

	''' <summary>
	''' The class Authenticator represents an object that knows how to obtain
	''' authentication for a network connection.  Usually, it will do this
	''' by prompting the user for information.
	''' <p>
	''' Applications use this class by overriding {@link
	''' #getPasswordAuthentication()} in a sub-class. This method will
	''' typically use the various getXXX() accessor methods to get information
	''' about the entity requesting authentication. It must then acquire a
	''' username and password either by interacting with the user or through
	''' some other non-interactive means. The credentials are then returned
	''' as a <seealso cref="PasswordAuthentication"/> return value.
	''' <p>
	''' An instance of this concrete sub-class is then registered
	''' with the system by calling <seealso cref="#setDefault(Authenticator)"/>.
	''' When authentication is required, the system will invoke one of the
	''' requestPasswordAuthentication() methods which in turn will call the
	''' getPasswordAuthentication() method of the registered object.
	''' <p>
	''' All methods that request authentication have a default implementation
	''' that fails.
	''' </summary>
	''' <seealso cref= java.net.Authenticator#setDefault(java.net.Authenticator) </seealso>
	''' <seealso cref= java.net.Authenticator#getPasswordAuthentication()
	''' 
	''' @author  Bill Foote
	''' @since   1.2 </seealso>

	' There are no abstract methods, but to be useful the user must
	' subclass.
	Public MustInherit Class Authenticator

		' The system-wide authenticator object.  See setDefault().
		Private Shared theAuthenticator As Authenticator

		Private requestingHost As String
		Private requestingSite As InetAddress
		Private requestingPort As Integer
		Private requestingProtocol As String
		Private requestingPrompt As String
		Private requestingScheme As String
		Private requestingURL As URL
		Private requestingAuthType As RequestorType

		''' <summary>
		''' The type of the entity requesting authentication.
		''' 
		''' @since 1.5
		''' </summary>
		Public Enum RequestorType
			''' <summary>
			''' Entity requesting authentication is a HTTP proxy server.
			''' </summary>
			PROXY
			''' <summary>
			''' Entity requesting authentication is a HTTP origin server.
			''' </summary>
			SERVER
		End Enum

		Private Sub reset()
			requestingHost = Nothing
			requestingSite = Nothing
			requestingPort = -1
			requestingProtocol = Nothing
			requestingPrompt = Nothing
			requestingScheme = Nothing
			requestingURL = Nothing
			requestingAuthType = RequestorType.SERVER
		End Sub


		''' <summary>
		''' Sets the authenticator that will be used by the networking code
		''' when a proxy or an HTTP server asks for authentication.
		''' <p>
		''' First, if there is a security manager, its {@code checkPermission}
		''' method is called with a
		''' {@code NetPermission("setDefaultAuthenticator")} permission.
		''' This may result in a java.lang.SecurityException.
		''' </summary>
		''' <param name="a">       The authenticator to be set. If a is {@code null} then
		'''                  any previously set authenticator is removed.
		''' </param>
		''' <exception cref="SecurityException">
		'''        if a security manager exists and its
		'''        {@code checkPermission} method doesn't allow
		'''        setting the default authenticator.
		''' </exception>
		''' <seealso cref= SecurityManager#checkPermission </seealso>
		''' <seealso cref= java.net.NetPermission </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Shared Property [default] As Authenticator
			Set(  a As Authenticator)
				Dim sm As SecurityManager = System.securityManager
				If sm IsNot Nothing Then
					Dim defaultPermissionion As New NetPermission("setDefaultAuthenticator")
					sm.checkPermission(defaultPermissionion)
				End If
    
				theAuthenticator = a
			End Set
		End Property

		''' <summary>
		''' Ask the authenticator that has been registered with the system
		''' for a password.
		''' <p>
		''' First, if there is a security manager, its {@code checkPermission}
		''' method is called with a
		''' {@code NetPermission("requestPasswordAuthentication")} permission.
		''' This may result in a java.lang.SecurityException.
		''' </summary>
		''' <param name="addr"> The InetAddress of the site requesting authorization,
		'''             or null if not known. </param>
		''' <param name="port"> the port for the requested connection </param>
		''' <param name="protocol"> The protocol that's requesting the connection
		'''          (<seealso cref="java.net.Authenticator#getRequestingProtocol()"/>) </param>
		''' <param name="prompt"> A prompt string for the user </param>
		''' <param name="scheme"> The authentication scheme
		''' </param>
		''' <returns> The username/password, or null if one can't be gotten.
		''' </returns>
		''' <exception cref="SecurityException">
		'''        if a security manager exists and its
		'''        {@code checkPermission} method doesn't allow
		'''        the password authentication request.
		''' </exception>
		''' <seealso cref= SecurityManager#checkPermission </seealso>
		''' <seealso cref= java.net.NetPermission </seealso>
		Public Shared Function requestPasswordAuthentication(  addr As InetAddress,   port As Integer,   protocol As String,   prompt As String,   scheme As String) As PasswordAuthentication

			Dim sm As SecurityManager = System.securityManager
			If sm IsNot Nothing Then
				Dim requestPermission As New NetPermission("requestPasswordAuthentication")
				sm.checkPermission(requestPermission)
			End If

			Dim a As Authenticator = theAuthenticator
			If a Is Nothing Then
				Return Nothing
			Else
				SyncLock a
					a.reset()
					a.requestingSite = addr
					a.requestingPort = port
					a.requestingProtocol = protocol
					a.requestingPrompt = prompt
					a.requestingScheme = scheme
					Return a.passwordAuthentication
				End SyncLock
			End If
		End Function

		''' <summary>
		''' Ask the authenticator that has been registered with the system
		''' for a password. This is the preferred method for requesting a password
		''' because the hostname can be provided in cases where the InetAddress
		''' is not available.
		''' <p>
		''' First, if there is a security manager, its {@code checkPermission}
		''' method is called with a
		''' {@code NetPermission("requestPasswordAuthentication")} permission.
		''' This may result in a java.lang.SecurityException.
		''' </summary>
		''' <param name="host"> The hostname of the site requesting authentication. </param>
		''' <param name="addr"> The InetAddress of the site requesting authentication,
		'''             or null if not known. </param>
		''' <param name="port"> the port for the requested connection. </param>
		''' <param name="protocol"> The protocol that's requesting the connection
		'''          (<seealso cref="java.net.Authenticator#getRequestingProtocol()"/>) </param>
		''' <param name="prompt"> A prompt string for the user which identifies the authentication realm. </param>
		''' <param name="scheme"> The authentication scheme
		''' </param>
		''' <returns> The username/password, or null if one can't be gotten.
		''' </returns>
		''' <exception cref="SecurityException">
		'''        if a security manager exists and its
		'''        {@code checkPermission} method doesn't allow
		'''        the password authentication request.
		''' </exception>
		''' <seealso cref= SecurityManager#checkPermission </seealso>
		''' <seealso cref= java.net.NetPermission
		''' @since 1.4 </seealso>
		Public Shared Function requestPasswordAuthentication(  host As String,   addr As InetAddress,   port As Integer,   protocol As String,   prompt As String,   scheme As String) As PasswordAuthentication

			Dim sm As SecurityManager = System.securityManager
			If sm IsNot Nothing Then
				Dim requestPermission As New NetPermission("requestPasswordAuthentication")
				sm.checkPermission(requestPermission)
			End If

			Dim a As Authenticator = theAuthenticator
			If a Is Nothing Then
				Return Nothing
			Else
				SyncLock a
					a.reset()
					a.requestingHost = host
					a.requestingSite = addr
					a.requestingPort = port
					a.requestingProtocol = protocol
					a.requestingPrompt = prompt
					a.requestingScheme = scheme
					Return a.passwordAuthentication
				End SyncLock
			End If
		End Function

		''' <summary>
		''' Ask the authenticator that has been registered with the system
		''' for a password.
		''' <p>
		''' First, if there is a security manager, its {@code checkPermission}
		''' method is called with a
		''' {@code NetPermission("requestPasswordAuthentication")} permission.
		''' This may result in a java.lang.SecurityException.
		''' </summary>
		''' <param name="host"> The hostname of the site requesting authentication. </param>
		''' <param name="addr"> The InetAddress of the site requesting authorization,
		'''             or null if not known. </param>
		''' <param name="port"> the port for the requested connection </param>
		''' <param name="protocol"> The protocol that's requesting the connection
		'''          (<seealso cref="java.net.Authenticator#getRequestingProtocol()"/>) </param>
		''' <param name="prompt"> A prompt string for the user </param>
		''' <param name="scheme"> The authentication scheme </param>
		''' <param name="url"> The requesting URL that caused the authentication </param>
		''' <param name="reqType"> The type (server or proxy) of the entity requesting
		'''              authentication.
		''' </param>
		''' <returns> The username/password, or null if one can't be gotten.
		''' </returns>
		''' <exception cref="SecurityException">
		'''        if a security manager exists and its
		'''        {@code checkPermission} method doesn't allow
		'''        the password authentication request.
		''' </exception>
		''' <seealso cref= SecurityManager#checkPermission </seealso>
		''' <seealso cref= java.net.NetPermission
		''' 
		''' @since 1.5 </seealso>
		Public Shared Function requestPasswordAuthentication(  host As String,   addr As InetAddress,   port As Integer,   protocol As String,   prompt As String,   scheme As String,   url As URL,   reqType As RequestorType) As PasswordAuthentication

			Dim sm As SecurityManager = System.securityManager
			If sm IsNot Nothing Then
				Dim requestPermission As New NetPermission("requestPasswordAuthentication")
				sm.checkPermission(requestPermission)
			End If

			Dim a As Authenticator = theAuthenticator
			If a Is Nothing Then
				Return Nothing
			Else
				SyncLock a
					a.reset()
					a.requestingHost = host
					a.requestingSite = addr
					a.requestingPort = port
					a.requestingProtocol = protocol
					a.requestingPrompt = prompt
					a.requestingScheme = scheme
					a.requestingURL = url
					a.requestingAuthType = reqType
					Return a.passwordAuthentication
				End SyncLock
			End If
		End Function

		''' <summary>
		''' Gets the {@code hostname} of the
		''' site or proxy requesting authentication, or {@code null}
		''' if not available.
		''' </summary>
		''' <returns> the hostname of the connection requiring authentication, or null
		'''          if it's not available.
		''' @since 1.4 </returns>
		Protected Friend Property requestingHost As String
			Get
				Return requestingHost
			End Get
		End Property

		''' <summary>
		''' Gets the {@code InetAddress} of the
		''' site requesting authorization, or {@code null}
		''' if not available.
		''' </summary>
		''' <returns> the InetAddress of the site requesting authorization, or null
		'''          if it's not available. </returns>
		Protected Friend Property requestingSite As InetAddress
			Get
				Return requestingSite
			End Get
		End Property

		''' <summary>
		''' Gets the port number for the requested connection. </summary>
		''' <returns> an {@code int} indicating the
		''' port for the requested connection. </returns>
		Protected Friend Property requestingPort As Integer
			Get
				Return requestingPort
			End Get
		End Property

		''' <summary>
		''' Give the protocol that's requesting the connection.  Often this
		''' will be based on a URL, but in a future JDK it could be, for
		''' example, "SOCKS" for a password-protected SOCKS5 firewall.
		''' </summary>
		''' <returns> the protocol, optionally followed by "/version", where
		'''          version is a version number.
		''' </returns>
		''' <seealso cref= java.net.URL#getProtocol() </seealso>
		Protected Friend Property requestingProtocol As String
			Get
				Return requestingProtocol
			End Get
		End Property

		''' <summary>
		''' Gets the prompt string given by the requestor.
		''' </summary>
		''' <returns> the prompt string given by the requestor (realm for
		'''          http requests) </returns>
		Protected Friend Property requestingPrompt As String
			Get
				Return requestingPrompt
			End Get
		End Property

		''' <summary>
		''' Gets the scheme of the requestor (the HTTP scheme
		''' for an HTTP firewall, for example).
		''' </summary>
		''' <returns> the scheme of the requestor
		'''  </returns>
		Protected Friend Property requestingScheme As String
			Get
				Return requestingScheme
			End Get
		End Property

		''' <summary>
		''' Called when password authorization is needed.  Subclasses should
		''' override the default implementation, which returns null. </summary>
		''' <returns> The PasswordAuthentication collected from the
		'''          user, or null if none is provided. </returns>
		Protected Friend Overridable Property passwordAuthentication As PasswordAuthentication
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Returns the URL that resulted in this
		''' request for authentication.
		''' 
		''' @since 1.5
		''' </summary>
		''' <returns> the requesting URL
		'''  </returns>
		Protected Friend Overridable Property requestingURL As URL
			Get
				Return requestingURL
			End Get
		End Property

		''' <summary>
		''' Returns whether the requestor is a Proxy or a Server.
		''' 
		''' @since 1.5
		''' </summary>
		''' <returns> the authentication type of the requestor
		'''  </returns>
		Protected Friend Overridable Property requestorType As RequestorType
			Get
				Return requestingAuthType
			End Get
		End Property
	End Class

End Namespace