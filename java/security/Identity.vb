Imports Microsoft.VisualBasic
Imports System
Imports java.util

'
' * Copyright (c) 1996, 2015, Oracle and/or its affiliates. All rights reserved.
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
    ''' <p>This class represents identities: real-world objects such as people,
    ''' companies or organizations whose identities can be authenticated using
    ''' their public keys. Identities may also be more abstract (or concrete)
    ''' constructs, such as daemon threads or smart cards.
    ''' 
    ''' <p>All Identity objects have a name and a public key. Names are
    ''' immutable. Identities may also be scoped. That is, if an Identity is
    ''' specified to have a particular scope, then the name and public
    ''' key of the Identity are unique within that scope.
    ''' 
    ''' <p>An Identity also has a set of certificates (all certifying its own
    ''' public key). The Principal names specified in these certificates need
    ''' not be the same, only the key.
    ''' 
    ''' <p>An Identity can be subclassed, to include postal and email addresses,
    ''' telephone numbers, images of faces and logos, and so on.
    ''' </summary>
    ''' <seealso cref= IdentityScope </seealso>
    ''' <seealso cref= Signer </seealso>
    ''' <seealso cref= Principal
    ''' 
    ''' @author Benjamin Renaud </seealso>
    ''' @deprecated This class is no longer used. Its functionality has been
    ''' replaced by {@code java.security.KeyStore}, the
    ''' {@code java.security.cert} package, and
    ''' {@code java.security.Principal}. 
    <Obsolete("This class is no longer used. Its functionality has been"), Serializable>
    Public MustInherit Class Identity
        Implements Principal

        'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public abstract Return getPrincipals(Me);
			Public Function [if](subject = ByVal [Nothing] As ) As [MustOverride]
            Public MustOverride Function implies(ByVal subject As javax.security.auth.Subject) As default
			Public MustOverride Function Equals(ByVal another As Object) As Boolean

        ''' <summary>
        ''' use serialVersionUID from JDK 1.1.x for interoperability </summary>
        Private Const serialVersionUID As Long = 3609922007826600659L

        ''' <summary>
        ''' The name for this identity.
        ''' 
        ''' @serial
        ''' </summary>
        Private name As String

        ''' <summary>
        ''' The public key for this identity.
        ''' 
        ''' @serial
        ''' </summary>
        Private publicKey As PublicKey

        ''' <summary>
        ''' Generic, descriptive information about the identity.
        ''' 
        ''' @serial
        ''' </summary>
        Friend info As String = "No further information available."

        ''' <summary>
        ''' The scope of the identity.
        ''' 
        ''' @serial
        ''' </summary>
        Friend scope As IdentityScope

        ''' <summary>
        ''' The certificates for this identity.
        ''' 
        ''' @serial
        ''' </summary>
        Friend certificates_Renamed As Vector(Of Certificate)

        ''' <summary>
        ''' Constructor for serialization only.
        ''' </summary>
        Protected Friend Sub New()
            Me.New("restoring...")
        End Sub

        ''' <summary>
        ''' Constructs an identity with the specified name and scope.
        ''' </summary>
        ''' <param name="name"> the identity name. </param>
        ''' <param name="scope"> the scope of the identity.
        ''' </param>
        ''' <exception cref="KeyManagementException"> if there is already an identity
        ''' with the same name in the scope. </exception>
        Public Sub New(ByVal name As String, ByVal scope As IdentityScope)
            Me.New(name)
            If scope IsNot Nothing Then scope.addIdentity(Me)
            Me.scope = scope
        End Sub

        ''' <summary>
        ''' Constructs an identity with the specified name and no scope.
        ''' </summary>
        ''' <param name="name"> the identity name. </param>
        Public Sub New(ByVal name As String)
            Me.name = name
        End Sub

        ''' <summary>
        ''' Returns this identity's name.
        ''' </summary>
        ''' <returns> the name of this identity. </returns>
        Public Property name As String Implements Principal.getName
            Get
                Return name
            End Get
        End Property

        ''' <summary>
        ''' Returns this identity's scope.
        ''' </summary>
        ''' <returns> the scope of this identity. </returns>
        Public Property scope As IdentityScope
            Get
                Return scope
            End Get
        End Property

        ''' <summary>
        ''' Returns this identity's public key.
        ''' </summary>
        ''' <returns> the public key for this identity.
        ''' </returns>
        ''' <seealso cref= #setPublicKey </seealso>
        Public Overridable Property publicKey As PublicKey
            Get
                Return publicKey
            End Get
            Set(ByVal key As PublicKey)

                check("setIdentityPublicKey")
                Me.publicKey = key
                certificates_Renamed = New Vector(Of Certificate)
            End Set
        End Property

        ''' <summary>
        ''' Sets this identity's public key. The old key and all of this
        ''' identity's certificates are removed by this operation.
        ''' 
        ''' <p>First, if there is a security manager, its {@code checkSecurityAccess}
        ''' method is called with {@code "setIdentityPublicKey"}
        ''' as its argument to see if it's ok to set the public key.
        ''' </summary>
        ''' <param name="key"> the public key for this identity.
        ''' </param>
        ''' <exception cref="KeyManagementException"> if another identity in the
        ''' identity's scope has the same public key, or if another exception occurs.
        ''' </exception>
        ''' <exception cref="SecurityException">  if a security manager exists and its
        ''' {@code checkSecurityAccess} method doesn't allow
        ''' setting the public key.
        ''' </exception>
        ''' <seealso cref= #getPublicKey </seealso>
        ''' <seealso cref= SecurityManager#checkSecurityAccess </seealso>
        ' Should we throw an exception if this is already set? 

        ''' <summary>
        ''' Specifies a general information string for this identity.
        ''' 
        ''' <p>First, if there is a security manager, its {@code checkSecurityAccess}
        ''' method is called with {@code "setIdentityInfo"}
        ''' as its argument to see if it's ok to specify the information string.
        ''' </summary>
        ''' <param name="info"> the information string.
        ''' </param>
        ''' <exception cref="SecurityException">  if a security manager exists and its
        ''' {@code checkSecurityAccess} method doesn't allow
        ''' setting the information string.
        ''' </exception>
        ''' <seealso cref= #getInfo </seealso>
        ''' <seealso cref= SecurityManager#checkSecurityAccess </seealso>
        Public Overridable Property info As String
            Set(ByVal info As String)
                check("setIdentityInfo")
                Me.info = info
            End Set
            Get
                Return info
            End Get
        End Property


        ''' <summary>
        ''' Adds a certificate for this identity. If the identity has a public
        ''' key, the public key in the certificate must be the same, and if
        ''' the identity does not have a public key, the identity's
        ''' public key is set to be that specified in the certificate.
        ''' 
        ''' <p>First, if there is a security manager, its {@code checkSecurityAccess}
        ''' method is called with {@code "addIdentityCertificate"}
        ''' as its argument to see if it's ok to add a certificate.
        ''' </summary>
        ''' <param name="certificate"> the certificate to be added.
        ''' </param>
        ''' <exception cref="KeyManagementException"> if the certificate is not valid,
        ''' if the public key in the certificate being added conflicts with
        ''' this identity's public key, or if another exception occurs.
        ''' </exception>
        ''' <exception cref="SecurityException">  if a security manager exists and its
        ''' {@code checkSecurityAccess} method doesn't allow
        ''' adding a certificate.
        ''' </exception>
        ''' <seealso cref= SecurityManager#checkSecurityAccess </seealso>
        Public Overridable Sub addCertificate(ByVal certificate As Certificate)

            check("addIdentityCertificate")

            If certificates_Renamed Is Nothing Then certificates_Renamed = New Vector(Of Certificate)
            If publicKey IsNot Nothing Then
                If Not keyEquals(publicKey, certificate.publicKey) Then Throw New KeyManagementException("public key different from cert public key")
            Else
                publicKey = certificate.publicKey
            End If
            certificates_Renamed.addElement(certificate)
        End Sub

        Private Function keyEquals(ByVal aKey As PublicKey, ByVal anotherKey As PublicKey) As Boolean
            Dim aKeyFormat As String = aKey.format
            Dim anotherKeyFormat As String = anotherKey.format
            If (aKeyFormat Is Nothing) Xor (anotherKeyFormat Is Nothing) Then Return False
            If aKeyFormat IsNot Nothing AndAlso anotherKeyFormat IsNot Nothing Then
                If Not aKeyFormat.equalsIgnoreCase(anotherKeyFormat) Then Return False
            End If
            Return Array.Equals(aKey.encoded, anotherKey.encoded)
        End Function


        ''' <summary>
        ''' Removes a certificate from this identity.
        ''' 
        ''' <p>First, if there is a security manager, its {@code checkSecurityAccess}
        ''' method is called with {@code "removeIdentityCertificate"}
        ''' as its argument to see if it's ok to remove a certificate.
        ''' </summary>
        ''' <param name="certificate"> the certificate to be removed.
        ''' </param>
        ''' <exception cref="KeyManagementException"> if the certificate is
        ''' missing, or if another exception occurs.
        ''' </exception>
        ''' <exception cref="SecurityException">  if a security manager exists and its
        ''' {@code checkSecurityAccess} method doesn't allow
        ''' removing a certificate.
        ''' </exception>
        ''' <seealso cref= SecurityManager#checkSecurityAccess </seealso>
        Public Overridable Sub removeCertificate(ByVal certificate As Certificate)
            check("removeIdentityCertificate")
            If certificates_Renamed IsNot Nothing Then certificates_Renamed.removeElement(certificate)
        End Sub

        ''' <summary>
        ''' Returns a copy of all the certificates for this identity.
        ''' </summary>
        ''' <returns> a copy of all the certificates for this identity. </returns>
        Public Overridable Function certificates() As Certificate()
            If certificates_Renamed Is Nothing Then Return New Certificate() {}
            Dim len As Integer = certificates_Renamed.size()
            Dim certs As Certificate() = New Certificate(len - 1) {}
            certificates_Renamed.copyInto(certs)
            Return certs
        End Function

        ''' <summary>
        ''' Tests for equality between the specified object and this identity.
        ''' This first tests to see if the entities actually refer to the same
        ''' object, in which case it returns true. Next, it checks to see if
        ''' the entities have the same name and the same scope. If they do,
        ''' the method returns true. Otherwise, it calls
        ''' <seealso cref="#identityEquals(Identity) identityEquals"/>, which subclasses should
        ''' override.
        ''' </summary>
        ''' <param name="identity"> the object to test for equality with this identity.
        ''' </param>
        ''' <returns> true if the objects are considered equal, false otherwise.
        ''' </returns>
        ''' <seealso cref= #identityEquals </seealso>
        Public NotOverridable Overrides Function Equals(ByVal identity As Object) As Boolean

            If identity Is Me Then Return True

            If TypeOf identity Is Identity Then
                Dim i As Identity = CType(identity, Identity)
                If Me.fullName().Equals(i.fullName()) Then
                    Return True
                Else
                    Return identityEquals(i)
                End If
            End If
            Return False
        End Function

        ''' <summary>
        ''' Tests for equality between the specified identity and this identity.
        ''' This method should be overriden by subclasses to test for equality.
        ''' The default behavior is to return true if the names and public keys
        ''' are equal.
        ''' </summary>
        ''' <param name="identity"> the identity to test for equality with this identity.
        ''' </param>
        ''' <returns> true if the identities are considered equal, false
        ''' otherwise.
        ''' </returns>
        ''' <seealso cref= #equals </seealso>
        Protected Friend Overridable Function identityEquals(ByVal identity As Identity) As Boolean
            If Not name.equalsIgnoreCase(identity.name) Then Return False

            If (publicKey Is Nothing) Xor (identity.publicKey Is Nothing) Then Return False

            If publicKey IsNot Nothing AndAlso identity.publicKey IsNot Nothing Then
                If Not publicKey.Equals(identity.publicKey) Then Return False
            End If

            Return True

        End Function

        ''' <summary>
        ''' Returns a parsable name for identity: identityName.scopeName
        ''' </summary>
        Friend Overridable Function fullName() As String
            Dim parsable As String = name
            If scope IsNot Nothing Then parsable &= "." & scope.name
            Return parsable
        End Function

        ''' <summary>
        ''' Returns a short string describing this identity, telling its
        ''' name and its scope (if any).
        ''' 
        ''' <p>First, if there is a security manager, its {@code checkSecurityAccess}
        ''' method is called with {@code "printIdentity"}
        ''' as its argument to see if it's ok to return the string.
        ''' </summary>
        ''' <returns> information about this identity, such as its name and the
        ''' name of its scope (if any).
        ''' </returns>
        ''' <exception cref="SecurityException">  if a security manager exists and its
        ''' {@code checkSecurityAccess} method doesn't allow
        ''' returning a string describing this identity.
        ''' </exception>
        ''' <seealso cref= SecurityManager#checkSecurityAccess </seealso>
        Public Overrides Function ToString() As String
            check("printIdentity")
            Dim printable As String = name
            If scope IsNot Nothing Then printable &= "[" & scope.name & "]"
            Return printable
        End Function

        ''' <summary>
        ''' Returns a string representation of this identity, with
        ''' optionally more details than that provided by the
        ''' {@code toString} method without any arguments.
        ''' 
        ''' <p>First, if there is a security manager, its {@code checkSecurityAccess}
        ''' method is called with {@code "printIdentity"}
        ''' as its argument to see if it's ok to return the string.
        ''' </summary>
        ''' <param name="detailed"> whether or not to provide detailed information.
        ''' </param>
        ''' <returns> information about this identity. If {@code detailed}
        ''' is true, then this method returns more information than that
        ''' provided by the {@code toString} method without any arguments.
        ''' </returns>
        ''' <exception cref="SecurityException">  if a security manager exists and its
        ''' {@code checkSecurityAccess} method doesn't allow
        ''' returning a string describing this identity.
        ''' </exception>
        ''' <seealso cref= #toString </seealso>
        ''' <seealso cref= SecurityManager#checkSecurityAccess </seealso>
        Public Overrides Function ToString(ByVal detailed As Boolean) As String
            Dim out As String = ToString()
            If detailed Then
                out += vbLf
                out += printKeys()
                out += vbLf & printCertificates()
                If info IsNot Nothing Then
                    out += vbLf & vbTab & info
                Else
                    out += vbLf & vbTab & "no additional information available."
                End If
            End If
            Return out
        End Function

        Friend Overridable Function printKeys() As String
            Dim key As String = ""
            If publicKey IsNot Nothing Then
                key = vbTab & "public key initialized"
            Else
                key = vbTab & "no public key"
            End If
            Return key
        End Function

        Friend Overridable Function printCertificates() As String
            Dim out As String = ""
            If certificates_Renamed Is Nothing Then
                Return vbTab & "no certificates"
            Else
                out += vbTab & "certificates: " & vbLf

                Dim i As Integer = 1
                For Each cert As Certificate In certificates_Renamed
                    out += vbTab & "certificate " & i & vbTab & "for  : " & cert.principal & vbLf
                    i += 1
                    out += vbTab & vbTab & vbTab & "from : " & cert.guarantor & vbLf
                Next cert
            End If
            Return out
        End Function

        ''' <summary>
        ''' Returns a hashcode for this identity.
        ''' </summary>
        ''' <returns> a hashcode for this identity. </returns>
        Public Overrides Function GetHashCode() As Integer
            Return name.GetHashCode()
        End Function

        Private Shared Sub check(ByVal directive As String)
            Dim security_Renamed As SecurityManager = System.securityManager
            If security_Renamed IsNot Nothing Then security_Renamed.checkSecurityAccess(directive)
        End Sub
    End Class

End Namespace