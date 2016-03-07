Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic

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
    ''' An immutable sequence of certificates (a certification path).
    ''' <p>
    ''' This is an abstract class that defines the methods common to all
    ''' {@code CertPath}s. Subclasses can handle different kinds of
    ''' certificates (X.509, PGP, etc.).
    ''' <p>
    ''' All {@code CertPath} objects have a type, a list of
    ''' {@code Certificate}s, and one or more supported encodings. Because the
    ''' {@code CertPath} class is immutable, a {@code CertPath} cannot
    ''' change in any externally visible way after being constructed. This
    ''' stipulation applies to all public fields and methods of this class and any
    ''' added or overridden by subclasses.
    ''' <p>
    ''' The type is a {@code String} that identifies the type of
    ''' {@code Certificate}s in the certification path. For each
    ''' certificate {@code cert} in a certification path {@code certPath},
    ''' {@code cert.getType().equals(certPath.getType())} must be
    ''' {@code true}.
    ''' <p>
    ''' The list of {@code Certificate}s is an ordered {@code List} of
    ''' zero or more {@code Certificate}s. This {@code List} and all
    ''' of the {@code Certificate}s contained in it must be immutable.
    ''' <p>
    ''' Each {@code CertPath} object must support one or more encodings
    ''' so that the object can be translated into a byte array for storage or
    ''' transmission to other parties. Preferably, these encodings should be
    ''' well-documented standards (such as PKCS#7). One of the encodings supported
    ''' by a {@code CertPath} is considered the default encoding. This
    ''' encoding is used if no encoding is explicitly requested (for the
    ''' <seealso cref="#getEncoded() getEncoded()"/> method, for instance).
    ''' <p>
    ''' All {@code CertPath} objects are also {@code Serializable}.
    ''' {@code CertPath} objects are resolved into an alternate
    ''' <seealso cref="CertPathRep CertPathRep"/> object during serialization. This allows
    ''' a {@code CertPath} object to be serialized into an equivalent
    ''' representation regardless of its underlying implementation.
    ''' <p>
    ''' {@code CertPath} objects can be created with a
    ''' {@code CertificateFactory} or they can be returned by other classes,
    ''' such as a {@code CertPathBuilder}.
    ''' <p>
    ''' By convention, X.509 {@code CertPath}s (consisting of
    ''' {@code X509Certificate}s), are ordered starting with the target
    ''' certificate and ending with a certificate issued by the trust anchor. That
    ''' is, the issuer of one certificate is the subject of the following one. The
    ''' certificate representing the <seealso cref="TrustAnchor TrustAnchor"/> should not be
    ''' included in the certification path. Unvalidated X.509 {@code CertPath}s
    ''' may not follow these conventions. PKIX {@code CertPathValidator}s will
    ''' detect any departure from these conventions that cause the certification
    ''' path to be invalid and throw a {@code CertPathValidatorException}.
    ''' 
    ''' <p> Every implementation of the Java platform is required to support the
    ''' following standard {@code CertPath} encodings:
    ''' <ul>
    ''' <li>{@code PKCS7}</li>
    ''' <li>{@code PkiPath}</li>
    ''' </ul>
    ''' These encodings are described in the <a href=
    ''' "{@docRoot}/../technotes/guides/security/StandardNames.html#CertPathEncodings">
    ''' CertPath Encodings section</a> of the
    ''' Java Cryptography Architecture Standard Algorithm Name Documentation.
    ''' Consult the release documentation for your implementation to see if any
    ''' other encodings are supported.
    ''' <p>
    ''' <b>Concurrent Access</b>
    ''' <p>
    ''' All {@code CertPath} objects must be thread-safe. That is, multiple
    ''' threads may concurrently invoke the methods defined in this class on a
    ''' single {@code CertPath} object (or more than one) with no
    ''' ill effects. This is also true for the {@code List} returned by
    ''' {@code CertPath.getCertificates}.
    ''' <p>
    ''' Requiring {@code CertPath} objects to be immutable and thread-safe
    ''' allows them to be passed around to various pieces of code without worrying
    ''' about coordinating access.  Providing this thread-safety is
    ''' generally not difficult, since the {@code CertPath} and
    ''' {@code List} objects in question are immutable.
    ''' </summary>
    ''' <seealso cref= CertificateFactory </seealso>
    ''' <seealso cref= CertPathBuilder
    ''' 
    ''' @author      Yassir Elley
    ''' @since       1.4 </seealso>
    <Serializable>
    Public MustInherit Class CertPath : Inherits java.lang.Object

        Private Const serialVersionUID As Long = 6068470306649138683L

        ''' <summary>
        ''' Creates a {@code CertPath} of the specified type.
        ''' <p>
        ''' This constructor is protected because most users should use a
        ''' {@code CertificateFactory} to create {@code CertPath}s.
        ''' </summary>
        ''' <param name="type"> the standard name of the type of
        ''' {@code Certificate}s in this path </param>
        Protected Friend Sub New(ByVal type As String)
			Me.type = type
		End Sub

		''' <summary>
		''' Returns the type of {@code Certificate}s in this certification
		''' path. This is the same string that would be returned by
		''' <seealso cref="java.security.cert.Certificate#getType() cert.getType()"/>
		''' for all {@code Certificate}s in the certification path.
		''' </summary>
		''' <returns> the type of {@code Certificate}s in this certification
		''' path (never null) </returns>
		Public Overridable Property type As String

        ''' <summary>
        ''' Returns an iteration of the encodings supported by this certification
        ''' path, with the default encoding first. Attempts to modify the returned
        ''' {@code Iterator} via its {@code remove} method result in an
        ''' {@code UnsupportedOperationException}.
        ''' </summary>
        ''' <returns> an {@code Iterator} over the names of the supported
        '''         encodings (as Strings) </returns>
        Public MustOverride ReadOnly Property encodings As IEnumerator(Of String)

		''' <summary>
		''' Compares this certification path for equality with the specified
		''' object. Two {@code CertPath}s are equal if and only if their
		''' types are equal and their certificate {@code List}s (and by
		''' implication the {@code Certificate}s in those {@code List}s)
		''' are equal. A {@code CertPath} is never equal to an object that is
		''' not a {@code CertPath}.
		''' <p>
		''' This algorithm is implemented by this method. If it is overridden,
		''' the behavior specified here must be maintained.
		''' </summary>
		''' <param name="other"> the object to test for equality with this certification path </param>
		''' <returns> true if the specified object is equal to this certification path,
		''' false otherwise </returns>
		Public Overrides Function Equals(ByVal other As Object) As Boolean
			If Me Is other Then Return True
            If Not (TypeOf other Is CertPath) Then Return False

            Dim otherCP As CertPath = CType(other, CertPath)

            If Not otherCP.type.Equals(type) Then Return False

            Dim thisCertList As IList(Of Certificate) = Me.certificates
            Dim otherCertList As IList(Of Certificate) = otherCP.certificates
            Return (thisCertList.Equals(otherCertList))
		End Function

		''' <summary>
		''' Returns the hashcode for this certification path. The hash code of
		''' a certification path is defined to be the result of the following
		''' calculation:
		''' <pre>{@code
		'''  hashCode = path.getType().hashCode();
		'''  hashCode = 31*hashCode + path.getCertificates().hashCode();
		''' }</pre>
		''' This ensures that {@code path1.equals(path2)} implies that
		''' {@code path1.hashCode()==path2.hashCode()} for any two certification
		''' paths, {@code path1} and {@code path2}, as required by the
		''' general contract of {@code Object.hashCode}.
		''' </summary>
		''' <returns> the hashcode value for this certification path </returns>
		Public Overrides Function GetHashCode() As Integer
			Dim hashCode As Integer = type.GetHashCode()
			hashCode = 31*hashCode + certificates.GetHashCode()
			Return hashCode
		End Function

		''' <summary>
		''' Returns a string representation of this certification path.
		''' This calls the {@code toString} method on each of the
		''' {@code Certificate}s in the path.
		''' </summary>
		''' <returns> a string representation of this certification path </returns>
		Public Overrides Function ToString() As String
			Dim sb As New StringBuffer
            Dim stringIterator As IEnumerator(Of Certificate) = certificates.GetEnumerator()

            sb.append(vbLf & type & " Cert Path: length = " & certificates.Count & "." & vbLf)
            sb.append("[" & vbLf)

            Dim i As Integer = 1
			Do While stringIterator.MoveNext()
				sb.append("==========================================" & "===============Certificate " & i & " start." & vbLf)
				Dim stringCert As Certificate = stringIterator.Current
				sb.append(stringCert.ToString())
				sb.append(vbLf & "========================================" & "=================Certificate " & i & " end." & vbLf & vbLf & vbLf)
				i += 1
			Loop

			sb.append(vbLf & "]")
			Return sb.ToString()
		End Function

		''' <summary>
		''' Returns the encoded form of this certification path, using the default
		''' encoding.
		''' </summary>
		''' <returns> the encoded bytes </returns>
		''' <exception cref="CertificateEncodingException"> if an encoding error occurs </exception>
		Public MustOverride ReadOnly Property encoded As SByte()

		''' <summary>
		''' Returns the encoded form of this certification path, using the
		''' specified encoding.
		''' </summary>
		''' <param name="encoding"> the name of the encoding to use </param>
		''' <returns> the encoded bytes </returns>
		''' <exception cref="CertificateEncodingException"> if an encoding error occurs or
		'''   the encoding requested is not supported </exception>
		Public MustOverride Function getEncoded(ByVal encoding As String) As SByte()

        ''' <summary>
        ''' Returns the list of certificates in this certification path.
        ''' The {@code List} returned must be immutable and thread-safe.
        ''' </summary>
        ''' <returns> an immutable {@code List} of {@code Certificate}s
        '''         (may be empty, but not null) </returns>
        Public MustOverride ReadOnly Property certificates As IList(Of Certificate)

        ''' <summary>
        ''' Replaces the {@code CertPath} to be serialized with a
        ''' {@code CertPathRep} object.
        ''' </summary>
        ''' <returns> the {@code CertPathRep} to be serialized
        ''' </returns>
        ''' <exception cref="ObjectStreamException"> if a {@code CertPathRep} object
        ''' representing this certification path could not be created </exception>
        Protected Friend Overridable Function writeReplace() As Object
			Try
				Return New CertPathRep(type, encoded)
			Catch ce As CertificateException
				Dim nse As New java.io.NotSerializableException("java.security.cert.CertPath: " & type)
				nse.initCause(ce)
				Throw nse
			End Try
		End Function

		''' <summary>
		''' Alternate {@code CertPath} class for serialization.
		''' @since 1.4
		''' </summary>
		<Serializable> _
		Protected Friend Class CertPathRep

			Private Const serialVersionUID As Long = 3015633072427920915L

			''' <summary>
			''' The Certificate type </summary>
			Private type As String
			''' <summary>
			''' The encoded form of the cert path </summary>
			Private data As SByte()

			''' <summary>
			''' Creates a {@code CertPathRep} with the specified
			''' type and encoded form of a certification path.
			''' </summary>
			''' <param name="type"> the standard name of a {@code CertPath} type </param>
			''' <param name="data"> the encoded form of the certification path </param>
			Protected Friend Sub New(ByVal type As String, ByVal data As SByte())
				Me.type = type
				Me.data = data
			End Sub

			''' <summary>
			''' Returns a {@code CertPath} constructed from the type and data.
			''' </summary>
			''' <returns> the resolved {@code CertPath} object
			''' </returns>
			''' <exception cref="ObjectStreamException"> if a {@code CertPath} could not
			''' be constructed </exception>
			Protected Friend Overridable Function readResolve() As Object
				Try
					Dim cf As CertificateFactory = CertificateFactory.getInstance(type)
					Return cf.generateCertPath(New java.io.ByteArrayInputStream(data))
				Catch ce As CertificateException
					Dim nse As New java.io.NotSerializableException("java.security.cert.CertPath: " & type)
					nse.initCause(ce)
					Throw nse
				End Try
			End Function
		End Class
	End Class

End Namespace