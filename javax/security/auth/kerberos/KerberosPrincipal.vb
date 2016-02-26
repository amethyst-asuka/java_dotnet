Imports System
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
	''' This class encapsulates a Kerberos principal.
	''' 
	''' @author Mayank Upadhyay
	''' @since 1.4
	''' </summary>

	<Serializable> _
	Public NotInheritable Class KerberosPrincipal
		Implements java.security.Principal

		Private Const serialVersionUID As Long = -7374788026156829911L

		'name types

		''' <summary>
		''' unknown name type.
		''' </summary>

		Public Const KRB_NT_UNKNOWN As Integer = 0

		''' <summary>
		''' user principal name type.
		''' </summary>

		Public Const KRB_NT_PRINCIPAL As Integer = 1

		''' <summary>
		''' service and other unique instance (krbtgt) name type.
		''' </summary>
		Public Const KRB_NT_SRV_INST As Integer = 2

		''' <summary>
		''' service with host name as instance (telnet, rcommands) name type.
		''' </summary>

		Public Const KRB_NT_SRV_HST As Integer = 3

		''' <summary>
		''' service with host as remaining components name type.
		''' </summary>

		Public Const KRB_NT_SRV_XHST As Integer = 4

		''' <summary>
		''' unique ID name type.
		''' </summary>

		Public Const KRB_NT_UID As Integer = 5

		<NonSerialized> _
		Private fullName As String

		<NonSerialized> _
		Private realm As String

		<NonSerialized> _
		Private nameType As Integer


		''' <summary>
		''' Constructs a KerberosPrincipal from the provided string input. The
		''' name type for this  principal defaults to
		''' <seealso cref="#KRB_NT_PRINCIPAL KRB_NT_PRINCIPAL"/>
		''' This string is assumed to contain a name in the format
		''' that is specified in Section 2.1.1. (Kerberos Principal Name Form) of
		''' <a href=http://www.ietf.org/rfc/rfc1964.txt> RFC 1964 </a>
		''' (for example, <i>duke@FOO.COM</i>, where <i>duke</i>
		''' represents a principal, and <i>FOO.COM</i> represents a realm).
		''' 
		''' <p>If the input name does not contain a realm, the default realm
		''' is used. The default realm can be specified either in a Kerberos
		''' configuration file or via the java.security.krb5.realm
		''' system property. For more information,
		''' <a href="../../../../../technotes/guides/security/jgss/tutorials/index.html">
		''' Kerberos Requirements </a>
		''' </summary>
		''' <param name="name"> the principal name </param>
		''' <exception cref="IllegalArgumentException"> if name is improperly
		''' formatted, if name is null, or if name does not contain
		''' the realm to use and the default realm is not specified
		''' in either a Kerberos configuration file or via the
		''' java.security.krb5.realm system property. </exception>
		Public Sub New(ByVal name As String)
			Me.New(name, KRB_NT_PRINCIPAL)
		End Sub

		''' <summary>
		''' Constructs a KerberosPrincipal from the provided string and
		''' name type input.  The string is assumed to contain a name in the
		''' format that is specified in Section 2.1 (Mandatory Name Forms) of
		''' <a href=http://www.ietf.org/rfc/rfc1964.txt>RFC 1964</a>.
		''' Valid name types are specified in Section 6.2 (Principal Names) of
		''' <a href=http://www.ietf.org/rfc/rfc4120.txt>RFC 4120</a>.
		''' The input name must be consistent with the provided name type.
		''' (for example, <i>duke@FOO.COM</i>, is a valid input string for the
		''' name type, KRB_NT_PRINCIPAL where <i>duke</i>
		''' represents a principal, and <i>FOO.COM</i> represents a realm).
		''' 
		''' <p> If the input name does not contain a realm, the default realm
		''' is used. The default realm can be specified either in a Kerberos
		''' configuration file or via the java.security.krb5.realm
		''' system property. For more information, see
		''' <a href="../../../../../technotes/guides/security/jgss/tutorials/index.html">
		''' Kerberos Requirements</a>.
		''' </summary>
		''' <param name="name"> the principal name </param>
		''' <param name="nameType"> the name type of the principal </param>
		''' <exception cref="IllegalArgumentException"> if name is improperly
		''' formatted, if name is null, if the nameType is not supported,
		''' or if name does not contain the realm to use and the default
		''' realm is not specified in either a Kerberos configuration
		''' file or via the java.security.krb5.realm system property. </exception>

		Public Sub New(ByVal name As String, ByVal nameType As Integer)

			Dim krb5Principal As sun.security.krb5.PrincipalName = Nothing

			Try
				' Appends the default realm if it is missing
				krb5Principal = New sun.security.krb5.PrincipalName(name,nameType)
			Catch e As sun.security.krb5.KrbException
				Throw New System.ArgumentException(e.Message)
			End Try

			' A ServicePermission with a principal in the deduced realm and
			' any action must be granted if no realm is provided by caller.
			If krb5Principal.realmDeduced AndAlso (Not sun.security.krb5.Realm.AUTODEDUCEREALM) Then
				Dim sm As SecurityManager = System.securityManager
				If sm IsNot Nothing Then
					Try
						sm.checkPermission(New ServicePermission("@" & krb5Principal.realmAsString, "-"))
					Catch se As SecurityException
						' Swallow the actual exception to hide info
						Throw New SecurityException("Cannot read realm info")
					End Try
				End If
			End If
			Me.nameType = nameType
			fullName = krb5Principal.ToString()
			realm = krb5Principal.realmString
		End Sub
		''' <summary>
		''' Returns the realm component of this Kerberos principal.
		''' </summary>
		''' <returns> the realm component of this Kerberos principal. </returns>
		Public Property realm As String
			Get
				Return realm
			End Get
		End Property

		''' <summary>
		''' Returns a hashcode for this principal. The hash code is defined to
		''' be the result of the following  calculation:
		''' <pre>{@code
		'''  hashCode = getName().hashCode();
		''' }</pre>
		''' </summary>
		''' <returns> a hashCode() for the {@code KerberosPrincipal} </returns>
		Public Overrides Function GetHashCode() As Integer
			Return name.GetHashCode()
		End Function

		''' <summary>
		''' Compares the specified Object with this Principal for equality.
		''' Returns true if the given object is also a
		''' {@code KerberosPrincipal} and the two
		''' {@code KerberosPrincipal} instances are equivalent.
		''' More formally two {@code KerberosPrincipal} instances are equal
		''' if the values returned by {@code getName()} are equal.
		''' </summary>
		''' <param name="other"> the Object to compare to </param>
		''' <returns> true if the Object passed in represents the same principal
		''' as this one, false otherwise. </returns>
		Public Overrides Function Equals(ByVal other As Object) As Boolean

			If other Is Me Then Return True

			If Not(TypeOf other Is KerberosPrincipal) Then Return False
			Dim myFullName As String = name
			Dim otherFullName As String = CType(other, KerberosPrincipal).name
			Return myFullName.Equals(otherFullName)
		End Function

		''' <summary>
		''' Save the KerberosPrincipal object to a stream
		''' 
		''' @serialData this {@code KerberosPrincipal} is serialized
		'''          by writing out the PrincipalName and the
		'''          realm in their DER-encoded form as specified in Section 5.2.2 of
		'''          <a href=http://www.ietf.org/rfc/rfc4120.txt> RFC4120</a>.
		''' </summary>
		Private Sub writeObject(ByVal oos As ObjectOutputStream)

			Dim krb5Principal As sun.security.krb5.PrincipalName
			Try
				krb5Principal = New sun.security.krb5.PrincipalName(fullName, nameType)
				oos.writeObject(krb5Principal.asn1Encode())
				oos.writeObject(krb5Principal.realm.asn1Encode())
			Catch e As Exception
				Throw New IOException(e)
			End Try
		End Sub

		''' <summary>
		''' Reads this object from a stream (i.e., deserializes it)
		''' </summary>
		Private Sub readObject(ByVal ois As ObjectInputStream)
			Dim asn1EncPrincipal As SByte() = CType(ois.readObject(), SByte ())
			Dim encRealm As SByte() = CType(ois.readObject(), SByte ())
			Try
			   Dim realmObject As New sun.security.krb5.Realm(New DerValue(encRealm))
			   Dim krb5Principal As New sun.security.krb5.PrincipalName(New DerValue(asn1EncPrincipal), realmObject)
			   realm = realmObject.ToString()
			   fullName = krb5Principal.ToString()
			   nameType = krb5Principal.nameType
			Catch e As Exception
				Throw New IOException(e)
			End Try
		End Sub

		''' <summary>
		''' The returned string corresponds to the single-string
		''' representation of a Kerberos Principal name as specified in
		''' Section 2.1 of <a href=http://www.ietf.org/rfc/rfc1964.txt>RFC 1964</a>.
		''' </summary>
		''' <returns> the principal name. </returns>
		Public Property name As String
			Get
				Return fullName
			End Get
		End Property

		''' <summary>
		''' Returns the name type of the KerberosPrincipal. Valid name types
		''' are specified in Section 6.2 of
		''' <a href=http://www.ietf.org/rfc/rfc4120.txt> RFC4120</a>.
		''' </summary>
		''' <returns> the name type. </returns>
		Public Property nameType As Integer
			Get
				Return nameType
			End Get
		End Property

		' Inherits javadocs from Object
		Public Overrides Function ToString() As String
			Return name
		End Function
	End Class

End Namespace