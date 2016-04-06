Imports Microsoft.VisualBasic

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

Namespace java.security.cert

	''' <summary>
	''' Parameters used as input for the LDAP {@code CertStore} algorithm.
	''' <p>
	''' This class is used to provide necessary configuration parameters (server
	''' name and port number) to implementations of the LDAP {@code CertStore}
	''' algorithm.
	''' <p>
	''' <b>Concurrent Access</b>
	''' <p>
	''' Unless otherwise specified, the methods defined in this class are not
	''' thread-safe. Multiple threads that need to access a single
	''' object concurrently should synchronize amongst themselves and
	''' provide the necessary locking. Multiple threads each manipulating
	''' separate objects need not synchronize.
	''' 
	''' @since       1.4
	''' @author      Steve Hanna </summary>
	''' <seealso cref=         CertStore </seealso>
	Public Class LDAPCertStoreParameters
		Implements CertStoreParameters

		Private Const LDAP_DEFAULT_PORT As Integer = 389

		''' <summary>
		''' the port number of the LDAP server
		''' </summary>
		Private port As Integer

		''' <summary>
		''' the DNS name of the LDAP server
		''' </summary>
		Private serverName As String

		''' <summary>
		''' Creates an instance of {@code LDAPCertStoreParameters} with the
		''' specified parameter values.
		''' </summary>
		''' <param name="serverName"> the DNS name of the LDAP server </param>
		''' <param name="port"> the port number of the LDAP server </param>
		''' <exception cref="NullPointerException"> if {@code serverName} is
		''' {@code null} </exception>
		Public Sub New(  serverName As String,   port As Integer)
			If serverName Is Nothing Then Throw New NullPointerException
			Me.serverName = serverName
			Me.port = port
		End Sub

		''' <summary>
		''' Creates an instance of {@code LDAPCertStoreParameters} with the
		''' specified server name and a default port of 389.
		''' </summary>
		''' <param name="serverName"> the DNS name of the LDAP server </param>
		''' <exception cref="NullPointerException"> if {@code serverName} is
		''' {@code null} </exception>
		Public Sub New(  serverName As String)
			Me.New(serverName, LDAP_DEFAULT_PORT)
		End Sub

		''' <summary>
		''' Creates an instance of {@code LDAPCertStoreParameters} with the
		''' default parameter values (server name "localhost", port 389).
		''' </summary>
		Public Sub New()
			Me.New("localhost", LDAP_DEFAULT_PORT)
		End Sub

		''' <summary>
		''' Returns the DNS name of the LDAP server.
		''' </summary>
		''' <returns> the name (not {@code null}) </returns>
		Public Overridable Property serverName As String
			Get
				Return serverName
			End Get
		End Property

		''' <summary>
		''' Returns the port number of the LDAP server.
		''' </summary>
		''' <returns> the port number </returns>
		Public Overridable Property port As Integer
			Get
				Return port
			End Get
		End Property

		''' <summary>
		''' Returns a copy of this object. Changes to the copy will not affect
		''' the original and vice versa.
		''' <p>
		''' Note: this method currently performs a shallow copy of the object
		''' (simply calls {@code Object.clone()}). This may be changed in a
		''' future revision to perform a deep copy if new parameters are added
		''' that should not be shared.
		''' </summary>
		''' <returns> the copy </returns>
		Public Overridable Function clone() As Object Implements CertStoreParameters.clone
			Try
				Return MyBase.clone()
			Catch e As CloneNotSupportedException
				' Cannot happen 
				Throw New InternalError(e.ToString(), e)
			End Try
		End Function

		''' <summary>
		''' Returns a formatted string describing the parameters.
		''' </summary>
		''' <returns> a formatted string describing the parameters </returns>
		Public Overrides Function ToString() As String
			Dim sb As New StringBuffer
			sb.append("LDAPCertStoreParameters: [" & vbLf)

			sb.append("  serverName: " & serverName & vbLf)
			sb.append("  port: " & port & vbLf)
			sb.append("]")
			Return sb.ToString()
		End Function
	End Class

End Namespace