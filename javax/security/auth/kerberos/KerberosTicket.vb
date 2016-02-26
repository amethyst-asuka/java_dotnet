Imports Microsoft.VisualBasic
Imports System
Imports System.Text
Imports sun.security.util

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

Namespace javax.security.auth.kerberos

	''' <summary>
	''' This class encapsulates a Kerberos ticket and associated
	''' information as viewed from the client's point of view. It captures all
	''' information that the Key Distribution Center (KDC) sends to the client
	''' in the reply message KDC-REP defined in the Kerberos Protocol
	''' Specification (<a href=http://www.ietf.org/rfc/rfc4120.txt>RFC 4120</a>).
	''' <p>
	''' All Kerberos JAAS login modules that authenticate a user to a KDC should
	''' use this class. Where available, the login module might even read this
	''' information from a ticket cache in the operating system instead of
	''' directly communicating with the KDC. During the commit phase of the JAAS
	''' authentication process, the JAAS login module should instantiate this
	''' class and store the instance in the private credential set of a
	''' <seealso cref="javax.security.auth.Subject Subject"/>.<p>
	''' 
	''' It might be necessary for the application to be granted a
	''' {@link javax.security.auth.PrivateCredentialPermission
	''' PrivateCredentialPermission} if it needs to access a KerberosTicket
	''' instance from a Subject. This permission is not needed when the
	''' application depends on the default JGSS Kerberos mechanism to access the
	''' KerberosTicket. In that case, however, the application will need an
	''' appropriate
	''' <seealso cref="javax.security.auth.kerberos.ServicePermission ServicePermission"/>.
	''' <p>
	''' Note that this class is applicable to both ticket granting tickets and
	''' other regular service tickets. A ticket granting ticket is just a
	''' special case of a more generalized service ticket.
	''' </summary>
	''' <seealso cref= javax.security.auth.Subject </seealso>
	''' <seealso cref= javax.security.auth.PrivateCredentialPermission </seealso>
	''' <seealso cref= javax.security.auth.login.LoginContext </seealso>
	''' <seealso cref= org.ietf.jgss.GSSCredential </seealso>
	''' <seealso cref= org.ietf.jgss.GSSManager
	''' 
	''' @author Mayank Upadhyay
	''' @since 1.4 </seealso>
	<Serializable> _
	Public Class KerberosTicket
		Implements javax.security.auth.Destroyable, javax.security.auth.Refreshable

		Private Const serialVersionUID As Long = 7395334370157380539L

		' XXX Make these flag indices public
		Private Const FORWARDABLE_TICKET_FLAG As Integer = 1
		Private Const FORWARDED_TICKET_FLAG As Integer = 2
		Private Const PROXIABLE_TICKET_FLAG As Integer = 3
		Private Const PROXY_TICKET_FLAG As Integer = 4
		Private Const POSTDATED_TICKET_FLAG As Integer = 6
		Private Const RENEWABLE_TICKET_FLAG As Integer = 8
		Private Const INITIAL_TICKET_FLAG As Integer = 9

		Private Const NUM_FLAGS As Integer = 32

		''' 
		''' <summary>
		''' ASN.1 DER Encoding of the Ticket as defined in the
		''' Kerberos Protocol Specification RFC4120.
		''' 
		''' @serial
		''' </summary>

		Private asn1Encoding As SByte()

		''' <summary>
		''' {@code KeyImpl} is serialized by writing out the ASN1 Encoded bytes
		''' of the encryption key. The ASN1 encoding is defined in RFC4120 and as
		''' follows:
		''' <pre>
		''' EncryptionKey   ::= SEQUENCE {
		'''          keytype    [0] Int32 -- actually encryption type --,
		'''          keyvalue   [1] OCTET STRING
		''' }
		''' </pre>
		''' 
		''' @serial
		''' </summary>

		Private sessionKey As KeyImpl

		''' 
		''' <summary>
		''' Ticket Flags as defined in the Kerberos Protocol Specification RFC4120.
		''' 
		''' @serial
		''' </summary>

		Private flags As Boolean()

		''' 
		''' <summary>
		''' Time of initial authentication
		''' 
		''' @serial
		''' </summary>

		Private authTime As DateTime

		''' 
		''' <summary>
		''' Time after which the ticket is valid.
		''' @serial
		''' </summary>
		Private startTime As DateTime

		''' 
		''' <summary>
		''' Time after which the ticket will not be honored. (its expiration time).
		''' 
		''' @serial
		''' </summary>

		Private endTime As DateTime

		''' 
		''' <summary>
		''' For renewable Tickets it indicates the maximum endtime that may be
		''' included in a renewal. It can be thought of as the absolute expiration
		''' time for the ticket, including all renewals. This field may be null
		''' for tickets that are not renewable.
		''' 
		''' @serial
		''' </summary>

		Private renewTill As DateTime

		''' 
		''' <summary>
		''' Client that owns the service ticket
		''' 
		''' @serial
		''' </summary>

		Private client As KerberosPrincipal

		''' 
		''' <summary>
		''' The service for which the ticket was issued.
		''' 
		''' @serial
		''' </summary>

		Private server As KerberosPrincipal

		''' 
		''' <summary>
		''' The addresses from where the ticket may be used by the client.
		''' This field may be null when the ticket is usable from any address.
		''' 
		''' @serial
		''' </summary>


		Private clientAddresses As java.net.InetAddress()

		<NonSerialized> _
		Private destroyed As Boolean = False

		''' <summary>
		''' Constructs a KerberosTicket using credentials information that a
		''' client either receives from a KDC or reads from a cache.
		''' </summary>
		''' <param name="asn1Encoding"> the ASN.1 encoding of the ticket as defined by
		''' the Kerberos protocol specification. </param>
		''' <param name="client"> the client that owns this service
		''' ticket </param>
		''' <param name="server"> the service that this ticket is for </param>
		''' <param name="sessionKey"> the raw bytes for the session key that must be
		''' used to encrypt the authenticator that will be sent to the server </param>
		''' <param name="keyType"> the key type for the session key as defined by the
		''' Kerberos protocol specification. </param>
		''' <param name="flags"> the ticket flags. Each element in this array indicates
		''' the value for the corresponding bit in the ASN.1 BitString that
		''' represents the ticket flags. If the number of elements in this array
		''' is less than the number of flags used by the Kerberos protocol,
		''' then the missing flags will be filled in with false. </param>
		''' <param name="authTime"> the time of initial authentication for the client </param>
		''' <param name="startTime"> the time after which the ticket will be valid. This
		''' may be null in which case the value of authTime is treated as the
		''' startTime. </param>
		''' <param name="endTime"> the time after which the ticket will no longer be
		''' valid </param>
		''' <param name="renewTill"> an absolute expiration time for the ticket,
		''' including all renewal that might be possible. This field may be null
		''' for tickets that are not renewable. </param>
		''' <param name="clientAddresses"> the addresses from where the ticket may be
		''' used by the client. This field may be null when the ticket is usable
		''' from any address. </param>
		Public Sub New(ByVal asn1Encoding As SByte(), ByVal client As KerberosPrincipal, ByVal server As KerberosPrincipal, ByVal sessionKey As SByte(), ByVal keyType As Integer, ByVal flags As Boolean(), ByVal authTime As DateTime, ByVal startTime As DateTime, ByVal endTime As DateTime, ByVal renewTill As DateTime, ByVal clientAddresses As java.net.InetAddress())

			init(asn1Encoding, client, server, sessionKey, keyType, flags, authTime, startTime, endTime, renewTill, clientAddresses)
		End Sub

		Private Sub init(ByVal asn1Encoding As SByte(), ByVal client As KerberosPrincipal, ByVal server As KerberosPrincipal, ByVal sessionKey As SByte(), ByVal keyType As Integer, ByVal flags As Boolean(), ByVal authTime As DateTime, ByVal startTime As DateTime, ByVal endTime As DateTime, ByVal renewTill As DateTime, ByVal clientAddresses As java.net.InetAddress())
			If sessionKey Is Nothing Then Throw New System.ArgumentException("Session key for ticket" & " cannot be null")
			init(asn1Encoding, client, server, New KeyImpl(sessionKey, keyType), flags, authTime, startTime, endTime, renewTill, clientAddresses)
		End Sub

		Private Sub init(ByVal asn1Encoding As SByte(), ByVal client As KerberosPrincipal, ByVal server As KerberosPrincipal, ByVal sessionKey As KeyImpl, ByVal flags As Boolean(), ByVal authTime As DateTime, ByVal startTime As DateTime, ByVal endTime As DateTime, ByVal renewTill As DateTime, ByVal clientAddresses As java.net.InetAddress())
			If asn1Encoding Is Nothing Then Throw New System.ArgumentException("ASN.1 encoding of ticket" & " cannot be null")
			Me.asn1Encoding = asn1Encoding.clone()

			If client Is Nothing Then Throw New System.ArgumentException("Client name in ticket" & " cannot be null")
			Me.client = client

			If server Is Nothing Then Throw New System.ArgumentException("Server name in ticket" & " cannot be null")
			Me.server = server

			' Caller needs to make sure `sessionKey` will not be null
			Me.sessionKey = sessionKey

			If flags IsNot Nothing Then
			   If flags.Length >= NUM_FLAGS Then
					Me.flags = flags.clone()
			   Else
					Me.flags = New Boolean(NUM_FLAGS - 1){}
					' Fill in whatever we have
					For i As Integer = 0 To flags.Length - 1
						Me.flags(i) = flags(i)
					Next i
			   End If
			Else
			   Me.flags = New Boolean(NUM_FLAGS - 1){}
			End If

			If Me.flags(RENEWABLE_TICKET_FLAG) Then
			   If renewTill Is Nothing Then Throw New System.ArgumentException("The renewable period " & "end time cannot be null for renewable tickets.")

			   Me.renewTill = New DateTime(renewTill)
			End If

			If authTime IsNot Nothing Then Me.authTime = New DateTime(authTime)
			If startTime IsNot Nothing Then
				Me.startTime = New DateTime(startTime)
			Else
				Me.startTime = Me.authTime
			End If

			If endTime Is Nothing Then Throw New System.ArgumentException("End time for ticket validity" & " cannot be null")
			Me.endTime = New DateTime(endTime)

			If clientAddresses IsNot Nothing Then Me.clientAddresses = clientAddresses.clone()
		End Sub

		''' <summary>
		''' Returns the client principal associated with this ticket.
		''' </summary>
		''' <returns> the client principal. </returns>
		Public Property client As KerberosPrincipal
			Get
				Return client
			End Get
		End Property

		''' <summary>
		''' Returns the service principal associated with this ticket.
		''' </summary>
		''' <returns> the service principal. </returns>
		Public Property server As KerberosPrincipal
			Get
				Return server
			End Get
		End Property

		''' <summary>
		''' Returns the session key associated with this ticket.
		''' </summary>
		''' <returns> the session key. </returns>
		Public Property sessionKey As javax.crypto.SecretKey
			Get
				If destroyed Then Throw New IllegalStateException("This ticket is no longer valid")
				Return sessionKey
			End Get
		End Property

		''' <summary>
		''' Returns the key type of the session key associated with this
		''' ticket as defined by the Kerberos Protocol Specification.
		''' </summary>
		''' <returns> the key type of the session key associated with this
		''' ticket.
		''' </returns>
		''' <seealso cref= #getSessionKey() </seealso>
		Public Property sessionKeyType As Integer
			Get
				If destroyed Then Throw New IllegalStateException("This ticket is no longer valid")
				Return sessionKey.keyType
			End Get
		End Property

		''' <summary>
		''' Determines if this ticket is forwardable.
		''' </summary>
		''' <returns> true if this ticket is forwardable, false if not. </returns>
		Public Property forwardable As Boolean
			Get
				Return flags(FORWARDABLE_TICKET_FLAG)
			End Get
		End Property

		''' <summary>
		''' Determines if this ticket had been forwarded or was issued based on
		''' authentication involving a forwarded ticket-granting ticket.
		''' </summary>
		''' <returns> true if this ticket had been forwarded or was issued based on
		''' authentication involving a forwarded ticket-granting ticket,
		''' false otherwise. </returns>
		Public Property forwarded As Boolean
			Get
				Return flags(FORWARDED_TICKET_FLAG)
			End Get
		End Property

		''' <summary>
		''' Determines if this ticket is proxiable.
		''' </summary>
		''' <returns> true if this ticket is proxiable, false if not. </returns>
		Public Property proxiable As Boolean
			Get
				Return flags(PROXIABLE_TICKET_FLAG)
			End Get
		End Property

		''' <summary>
		''' Determines is this ticket is a proxy-ticket.
		''' </summary>
		''' <returns> true if this ticket is a proxy-ticket, false if not. </returns>
		Public Property proxy As Boolean
			Get
				Return flags(PROXY_TICKET_FLAG)
			End Get
		End Property


		''' <summary>
		''' Determines is this ticket is post-dated.
		''' </summary>
		''' <returns> true if this ticket is post-dated, false if not. </returns>
		Public Property postdated As Boolean
			Get
				Return flags(POSTDATED_TICKET_FLAG)
			End Get
		End Property

		''' <summary>
		''' Determines is this ticket is renewable. If so, the {@link #refresh()
		''' refresh} method can be called, assuming the validity period for
		''' renewing is not already over.
		''' </summary>
		''' <returns> true if this ticket is renewable, false if not. </returns>
		Public Property renewable As Boolean
			Get
				Return flags(RENEWABLE_TICKET_FLAG)
			End Get
		End Property

		''' <summary>
		''' Determines if this ticket was issued using the Kerberos AS-Exchange
		''' protocol, and not issued based on some ticket-granting ticket.
		''' </summary>
		''' <returns> true if this ticket was issued using the Kerberos AS-Exchange
		''' protocol, false if not. </returns>
		Public Property initial As Boolean
			Get
				Return flags(INITIAL_TICKET_FLAG)
			End Get
		End Property

		''' <summary>
		''' Returns the flags associated with this ticket. Each element in the
		''' returned array indicates the value for the corresponding bit in the
		''' ASN.1 BitString that represents the ticket flags.
		''' </summary>
		''' <returns> the flags associated with this ticket. </returns>
		Public Property flags As Boolean()
			Get
				Return (If(flags Is Nothing, Nothing, flags.clone()))
			End Get
		End Property

		''' <summary>
		''' Returns the time that the client was authenticated.
		''' </summary>
		''' <returns> the time that the client was authenticated
		'''         or null if not set. </returns>
		Public Property authTime As DateTime
			Get
				Return If(authTime Is Nothing, Nothing, CDate(authTime.clone()))
			End Get
		End Property

		''' <summary>
		''' Returns the start time for this ticket's validity period.
		''' </summary>
		''' <returns> the start time for this ticket's validity period
		'''         or null if not set. </returns>
		Public Property startTime As DateTime
			Get
				Return If(startTime Is Nothing, Nothing, CDate(startTime.clone()))
			End Get
		End Property

		''' <summary>
		''' Returns the expiration time for this ticket's validity period.
		''' </summary>
		''' <returns> the expiration time for this ticket's validity period. </returns>
		Public Property endTime As DateTime
			Get
				Return CDate(endTime.clone())
			End Get
		End Property

		''' <summary>
		''' Returns the latest expiration time for this ticket, including all
		''' renewals. This will return a null value for non-renewable tickets.
		''' </summary>
		''' <returns> the latest expiration time for this ticket. </returns>
		Public Property renewTill As DateTime
			Get
				Return If(renewTill Is Nothing, Nothing, CDate(renewTill.clone()))
			End Get
		End Property

		''' <summary>
		''' Returns a list of addresses from where the ticket can be used.
		''' </summary>
		''' <returns> ths list of addresses or null, if the field was not
		''' provided. </returns>
		Public Property clientAddresses As java.net.InetAddress()
			Get
				Return If(clientAddresses Is Nothing, Nothing, clientAddresses.clone())
			End Get
		End Property

		''' <summary>
		''' Returns an ASN.1 encoding of the entire ticket.
		''' </summary>
		''' <returns> an ASN.1 encoding of the entire ticket. </returns>
		Public Property encoded As SByte()
			Get
				If destroyed Then Throw New IllegalStateException("This ticket is no longer valid")
				Return asn1Encoding.clone()
			End Get
		End Property

		''' <summary>
		''' Determines if this ticket is still current. </summary>
		Public Overridable Property current As Boolean
			Get
				Return (System.currentTimeMillis() <= endTime)
			End Get
		End Property

		''' <summary>
		''' Extends the validity period of this ticket. The ticket will contain
		''' a new session key if the refresh operation succeeds. The refresh
		''' operation will fail if the ticket is not renewable or the latest
		''' allowable renew time has passed. Any other error returned by the
		''' KDC will also cause this method to fail.
		''' 
		''' Note: This method is not synchronized with the the accessor
		''' methods of this object. Hence callers need to be aware of multiple
		''' threads that might access this and try to renew it at the same
		''' time.
		''' </summary>
		''' <exception cref="RefreshFailedException"> if the ticket is not renewable, or
		''' the latest allowable renew time has passed, or the KDC returns some
		''' error.
		''' </exception>
		''' <seealso cref= #isRenewable() </seealso>
		''' <seealso cref= #getRenewTill() </seealso>
		Public Overridable Sub refresh()

			If destroyed Then Throw New javax.security.auth.RefreshFailedException("A destroyed ticket " & "cannot be renewd.")

			If Not renewable Then Throw New javax.security.auth.RefreshFailedException("This ticket is not renewable")

			If System.currentTimeMillis() > renewTill Then Throw New javax.security.auth.RefreshFailedException("This ticket is past " & "its last renewal time.")
			Dim e As Exception = Nothing
			Dim krb5Creds As sun.security.krb5.Credentials = Nothing

			Try
				krb5Creds = New sun.security.krb5.Credentials(asn1Encoding, client.ToString(), server.ToString(), sessionKey.encoded, sessionKey.keyType, flags, authTime, startTime, endTime, renewTill, clientAddresses)
				krb5Creds = krb5Creds.renew()
			Catch krbException As sun.security.krb5.KrbException
				e = krbException
			Catch ioException As java.io.IOException
				e = ioException
			End Try

			If e IsNot Nothing Then
				Dim rfException As New javax.security.auth.RefreshFailedException("Failed to renew Kerberos Ticket " & "for client " & client & " and server " & server & " - " & e.Message)
				rfException.initCause(e)
				Throw rfException
			End If

	'        
	'         * In case multiple threads try to refresh it at the same time.
	'         
			SyncLock Me
				Try
					Me.destroy()
				Catch dfException As javax.security.auth.DestroyFailedException
					' Squelch it since we don't care about the old ticket.
				End Try
				init(krb5Creds.encoded, New KerberosPrincipal(krb5Creds.client.name), New KerberosPrincipal(krb5Creds.server.name, KerberosPrincipal.KRB_NT_SRV_INST), krb5Creds.sessionKey.bytes, krb5Creds.sessionKey.eType, krb5Creds.flags, krb5Creds.authTime, krb5Creds.startTime, krb5Creds.endTime, krb5Creds.renewTill, krb5Creds.clientAddresses)
				destroyed = False
			End SyncLock
		End Sub

		''' <summary>
		''' Destroys the ticket and destroys any sensitive information stored in
		''' it.
		''' </summary>
		Public Overridable Sub destroy()
			If Not destroyed Then
				java.util.Arrays.fill(asn1Encoding, CByte(0))
				client = Nothing
				server = Nothing
				sessionKey.destroy()
				flags = Nothing
				authTime = Nothing
				startTime = Nothing
				endTime = Nothing
				renewTill = Nothing
				clientAddresses = Nothing
				destroyed = True
			End If
		End Sub

		''' <summary>
		''' Determines if this ticket has been destroyed.
		''' </summary>
		Public Overridable Property destroyed As Boolean
			Get
				Return destroyed
			End Get
		End Property

		Public Overrides Function ToString() As String
			If destroyed Then Throw New IllegalStateException("This ticket is no longer valid")
			Dim caddrBuf As New StringBuilder
			If clientAddresses IsNot Nothing Then
				For i As Integer = 0 To clientAddresses.Length - 1
					caddrBuf.Append("clientAddresses[" & i & "] = " & clientAddresses(i).ToString())
				Next i
			End If
			Return ("Ticket (hex) = " & vbLf & (New sun.misc.HexDumpEncoder).encodeBuffer(asn1Encoding) & vbLf & "Client Principal = " & client.ToString() & vbLf & "Server Principal = " & server.ToString() & vbLf & "Session Key = " & sessionKey.ToString() & vbLf & "Forwardable Ticket " & flags(FORWARDABLE_TICKET_FLAG) & vbLf & "Forwarded Ticket " & flags(FORWARDED_TICKET_FLAG) & vbLf & "Proxiable Ticket " & flags(PROXIABLE_TICKET_FLAG) & vbLf & "Proxy Ticket " & flags(PROXY_TICKET_FLAG) & vbLf & "Postdated Ticket " & flags(POSTDATED_TICKET_FLAG) & vbLf & "Renewable Ticket " & flags(RENEWABLE_TICKET_FLAG) & vbLf & "Initial Ticket " & flags(RENEWABLE_TICKET_FLAG) & vbLf & "Auth Time = " & Convert.ToString(authTime) & vbLf & "Start Time = " & Convert.ToString(startTime) & vbLf & "End Time = " & endTime.ToString() & vbLf & "Renew Till = " & Convert.ToString(renewTill) & vbLf & "Client Addresses " & (If(clientAddresses Is Nothing, " Null ", caddrBuf.ToString() & vbLf)))
		End Function

		''' <summary>
		''' Returns a hashcode for this KerberosTicket.
		''' </summary>
		''' <returns> a hashCode() for the {@code KerberosTicket}
		''' @since 1.6 </returns>
		Public Overrides Function GetHashCode() As Integer
			Dim result As Integer = 17
			If destroyed Then Return result
			result = result * 37 + java.util.Arrays.hashCode(encoded)
			result = result * 37 + endTime.GetHashCode()
			result = result * 37 + client.GetHashCode()
			result = result * 37 + server.GetHashCode()
			result = result * 37 + sessionKey.GetHashCode()

			' authTime may be null
			If authTime IsNot Nothing Then result = result * 37 + authTime.GetHashCode()

			' startTime may be null
			If startTime IsNot Nothing Then result = result * 37 + startTime.GetHashCode()

			' renewTill may be null
			If renewTill IsNot Nothing Then result = result * 37 + renewTill.GetHashCode()

			' clientAddress may be null, the array's hashCode is 0
			result = result * 37 + java.util.Arrays.hashCode(clientAddresses)
			Return result * 37 + java.util.Arrays.hashCode(flags)
		End Function

		''' <summary>
		''' Compares the specified Object with this KerberosTicket for equality.
		''' Returns true if the given object is also a
		''' {@code KerberosTicket} and the two
		''' {@code KerberosTicket} instances are equivalent.
		''' </summary>
		''' <param name="other"> the Object to compare to </param>
		''' <returns> true if the specified object is equal to this KerberosTicket,
		''' false otherwise. NOTE: Returns false if either of the KerberosTicket
		''' objects has been destroyed.
		''' @since 1.6 </returns>
		Public Overrides Function Equals(ByVal other As Object) As Boolean

			If other Is Me Then Return True

			If Not(TypeOf other Is KerberosTicket) Then Return False

			Dim otherTicket As KerberosTicket = (CType(other, KerberosTicket))
			If destroyed OrElse otherTicket.destroyed Then Return False

			If (Not java.util.Arrays.Equals(encoded, otherTicket.encoded)) OrElse (Not endTime.Equals(otherTicket.endTime)) OrElse (Not server.Equals(otherTicket.server)) OrElse (Not client.Equals(otherTicket.client)) OrElse (Not sessionKey.Equals(otherTicket.sessionKey)) OrElse (Not java.util.Arrays.Equals(clientAddresses, otherTicket.clientAddresses)) OrElse (Not java.util.Arrays.Equals(flags, otherTicket.flags)) Then Return False

			' authTime may be null
			If authTime Is Nothing Then
				If otherTicket.authTime IsNot Nothing Then Return False
			Else
				If Not authTime.Equals(otherTicket.authTime) Then Return False
			End If

			' startTime may be null
			If startTime Is Nothing Then
				If otherTicket.startTime IsNot Nothing Then Return False
			Else
				If Not startTime.Equals(otherTicket.startTime) Then Return False
			End If

			If renewTill Is Nothing Then
				If otherTicket.renewTill IsNot Nothing Then Return False
			Else
				If Not renewTill.Equals(otherTicket.renewTill) Then Return False
			End If

			Return True
		End Function

		Private Sub readObject(ByVal s As ObjectInputStream)
			s.defaultReadObject()
			If sessionKey Is Nothing Then Throw New InvalidObjectException("Session key cannot be null")
			Try
				init(asn1Encoding, client, server, sessionKey, flags, authTime, startTime, endTime, renewTill, clientAddresses)
			Catch iae As System.ArgumentException
				Throw CType((New InvalidObjectException(iae.Message)).initCause(iae), InvalidObjectException)
			End Try
		End Sub
	End Class

End Namespace