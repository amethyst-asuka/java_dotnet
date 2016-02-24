Imports System
Imports System.Collections.Generic

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

Namespace java.security



	''' 
	''' <summary>
	''' <p>This class extends the concept of a codebase to
	''' encapsulate not only the location (URL) but also the certificate chains
	''' that were used to verify signed code originating from that location.
	''' 
	''' @author Li Gong
	''' @author Roland Schemers
	''' </summary>

	<Serializable> _
	Public Class CodeSource

		Private Const serialVersionUID As Long = 4977541819976013951L

		''' <summary>
		''' The code location.
		''' 
		''' @serial
		''' </summary>
		Private location As java.net.URL

	'    
	'     * The code signers.
	'     
		<NonSerialized> _
		Private signers As CodeSigner() = Nothing

	'    
	'     * The code signers. Certificate chains are concatenated.
	'     
		<NonSerialized> _
		Private certs As java.security.cert.Certificate() = Nothing

		' cached SocketPermission used for matchLocation
		<NonSerialized> _
		Private sp As java.net.SocketPermission

		' for generating cert paths
		<NonSerialized> _
		Private factory As CertificateFactory = Nothing

		''' <summary>
		''' Constructs a CodeSource and associates it with the specified
		''' location and set of certificates.
		''' </summary>
		''' <param name="url"> the location (URL).
		''' </param>
		''' <param name="certs"> the certificate(s). It may be null. The contents of the
		''' array are copied to protect against subsequent modification. </param>
		Public Sub New(ByVal url As java.net.URL, ByVal certs As java.security.cert.Certificate())
			Me.location = url

			' Copy the supplied certs
			If certs IsNot Nothing Then Me.certs = certs.clone()
		End Sub

		''' <summary>
		''' Constructs a CodeSource and associates it with the specified
		''' location and set of code signers.
		''' </summary>
		''' <param name="url"> the location (URL). </param>
		''' <param name="signers"> the code signers. It may be null. The contents of the
		''' array are copied to protect against subsequent modification.
		''' 
		''' @since 1.5 </param>
		Public Sub New(ByVal url As java.net.URL, ByVal signers As CodeSigner())
			Me.location = url

			' Copy the supplied signers
			If signers IsNot Nothing Then Me.signers = signers.clone()
		End Sub

		''' <summary>
		''' Returns the hash code value for this object.
		''' </summary>
		''' <returns> a hash code value for this object. </returns>
		Public Overrides Function GetHashCode() As Integer
			If location IsNot Nothing Then
				Return location.GetHashCode()
			Else
				Return 0
			End If
		End Function

		''' <summary>
		''' Tests for equality between the specified object and this
		''' object. Two CodeSource objects are considered equal if their
		''' locations are of identical value and if their signer certificate
		''' chains are of identical value. It is not required that
		''' the certificate chains be in the same order.
		''' </summary>
		''' <param name="obj"> the object to test for equality with this object.
		''' </param>
		''' <returns> true if the objects are considered equal, false otherwise. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If obj Is Me Then Return True

			' objects types must be equal
			If Not(TypeOf obj Is CodeSource) Then Return False

			Dim cs As CodeSource = CType(obj, CodeSource)

			' URLs must match
			If location Is Nothing Then
				' if location is null, then cs.location must be null as well
				If cs.location IsNot Nothing Then Return False
			Else
				' if location is not null, then it must equal cs.location
				If Not location.Equals(cs.location) Then Return False
			End If

			' certs must match
			Return matchCerts(cs, True)
		End Function

		''' <summary>
		''' Returns the location associated with this CodeSource.
		''' </summary>
		''' <returns> the location (URL). </returns>
		Public Property location As java.net.URL
			Get
		'         since URL is practically immutable, returning itself is not
		'           a security problem 
				Return Me.location
			End Get
		End Property

		''' <summary>
		''' Returns the certificates associated with this CodeSource.
		''' <p>
		''' If this CodeSource object was created using the
		''' <seealso cref="#CodeSource(URL url, CodeSigner[] signers)"/>
		''' constructor then its certificate chains are extracted and used to
		''' create an array of Certificate objects. Each signer certificate is
		''' followed by its supporting certificate chain (which may be empty).
		''' Each signer certificate and its supporting certificate chain is ordered
		''' bottom-to-top (i.e., with the signer certificate first and the (root)
		''' certificate authority last).
		''' </summary>
		''' <returns> A copy of the certificates array, or null if there is none. </returns>
		Public Property certificates As java.security.cert.Certificate()
			Get
				If certs IsNot Nothing Then
					Return certs.clone()
    
				ElseIf signers IsNot Nothing Then
					' Convert the code signers to certs
					Dim certChains As New List(Of java.security.cert.Certificate)
					For i As Integer = 0 To signers.Length - 1
						certChains.AddRange(signers(i).signerCertPath.certificates)
					Next i
					certs = certChains.ToArray()
					Return certs.clone()
    
				Else
					Return Nothing
				End If
			End Get
		End Property

		''' <summary>
		''' Returns the code signers associated with this CodeSource.
		''' <p>
		''' If this CodeSource object was created using the
		''' <seealso cref="#CodeSource(URL url, java.security.cert.Certificate[] certs)"/>
		''' constructor then its certificate chains are extracted and used to
		''' create an array of CodeSigner objects. Note that only X.509 certificates
		''' are examined - all other certificate types are ignored.
		''' </summary>
		''' <returns> A copy of the code signer array, or null if there is none.
		''' 
		''' @since 1.5 </returns>
		Public Property codeSigners As CodeSigner()
			Get
				If signers IsNot Nothing Then
					Return signers.clone()
    
				ElseIf certs IsNot Nothing Then
					' Convert the certs to code signers
					signers = convertCertArrayToSignerArray(certs)
					Return signers.clone()
    
				Else
					Return Nothing
				End If
			End Get
		End Property

		''' <summary>
		''' Returns true if this CodeSource object "implies" the specified CodeSource.
		''' <p>
		''' More specifically, this method makes the following checks.
		''' If any fail, it returns false. If they all succeed, it returns true.
		''' <ul>
		''' <li> <i>codesource</i> must not be null.
		''' <li> If this object's certificates are not null, then all
		''' of this object's certificates must be present in <i>codesource</i>'s
		''' certificates.
		''' <li> If this object's location (getLocation()) is not null, then the
		''' following checks are made against this object's location and
		''' <i>codesource</i>'s:
		'''   <ul>
		'''     <li>  <i>codesource</i>'s location must not be null.
		''' 
		'''     <li>  If this object's location
		'''           equals <i>codesource</i>'s location, then return true.
		''' 
		'''     <li>  This object's protocol (getLocation().getProtocol()) must be
		'''           equal to <i>codesource</i>'s protocol, ignoring case.
		''' 
		'''     <li>  If this object's host (getLocation().getHost()) is not null,
		'''           then the SocketPermission
		'''           constructed with this object's host must imply the
		'''           SocketPermission constructed with <i>codesource</i>'s host.
		''' 
		'''     <li>  If this object's port (getLocation().getPort()) is not
		'''           equal to -1 (that is, if a port is specified), it must equal
		'''           <i>codesource</i>'s port or default port
		'''           (codesource.getLocation().getDefaultPort()).
		''' 
		'''     <li>  If this object's file (getLocation().getFile()) doesn't equal
		'''           <i>codesource</i>'s file, then the following checks are made:
		'''           If this object's file ends with "/-",
		'''           then <i>codesource</i>'s file must start with this object's
		'''           file (exclusive the trailing "-").
		'''           If this object's file ends with a "/*",
		'''           then <i>codesource</i>'s file must start with this object's
		'''           file and must not have any further "/" separators.
		'''           If this object's file doesn't end with a "/",
		'''           then <i>codesource</i>'s file must match this object's
		'''           file with a '/' appended.
		''' 
		'''     <li>  If this object's reference (getLocation().getRef()) is
		'''           not null, it must equal <i>codesource</i>'s reference.
		''' 
		'''   </ul>
		''' </ul>
		''' <p>
		''' For example, the codesource objects with the following locations
		''' and null certificates all imply
		''' the codesource with the location "http://java.sun.com/classes/foo.jar"
		''' and null certificates:
		''' <pre>
		'''     http:
		'''     http://*.sun.com/classes/*
		'''     http://java.sun.com/classes/-
		'''     http://java.sun.com/classes/foo.jar
		''' </pre>
		''' 
		''' Note that if this CodeSource has a null location and a null
		''' certificate chain, then it implies every other CodeSource.
		''' </summary>
		''' <param name="codesource"> CodeSource to compare against.
		''' </param>
		''' <returns> true if the specified codesource is implied by this codesource,
		''' false if not. </returns>

		Public Overridable Function implies(ByVal codesource As CodeSource) As Boolean
			If codesource Is Nothing Then Return False

			Return matchCerts(codesource, False) AndAlso matchLocation(codesource)
		End Function

		''' <summary>
		''' Returns true if all the certs in this
		''' CodeSource are also in <i>that</i>.
		''' </summary>
		''' <param name="that"> the CodeSource to check against. </param>
		''' <param name="strict"> If true then a strict equality match is performed.
		'''               Otherwise a subset match is performed. </param>
		Private Function matchCerts(ByVal that As CodeSource, ByVal [strict] As Boolean) As Boolean
			Dim match As Boolean

			' match any key
			If certs Is Nothing AndAlso signers Is Nothing Then
				If [strict] Then
					Return (that.certs Is Nothing AndAlso that.signers Is Nothing)
				Else
					Return True
				End If
			' both have signers
			ElseIf signers IsNot Nothing AndAlso that.signers IsNot Nothing Then
				If [strict] AndAlso signers.Length <> that.signers.Length Then Return False
				For i As Integer = 0 To signers.Length - 1
					match = False
					For j As Integer = 0 To that.signers.Length - 1
						If signers(i).Equals(that.signers(j)) Then
							match = True
							Exit For
						End If
					Next j
					If Not match Then Return False
				Next i
				Return True

			' both have certs
			ElseIf certs IsNot Nothing AndAlso that.certs IsNot Nothing Then
				If [strict] AndAlso certs.Length <> that.certs.Length Then Return False
				For i As Integer = 0 To certs.Length - 1
					match = False
					For j As Integer = 0 To that.certs.Length - 1
						If certs(i).Equals(that.certs(j)) Then
							match = True
							Exit For
						End If
					Next j
					If Not match Then Return False
				Next i
				Return True
			End If

			Return False
		End Function


		''' <summary>
		''' Returns true if two CodeSource's have the "same" location.
		''' </summary>
		''' <param name="that"> CodeSource to compare against </param>
		Private Function matchLocation(ByVal that As CodeSource) As Boolean
			If location Is Nothing Then Return True

			If (that Is Nothing) OrElse (that.location Is Nothing) Then Return False

			If location.Equals(that.location) Then Return True

			If Not location.protocol.equalsIgnoreCase(that.location.protocol) Then Return False

			Dim thisPort As Integer = location.port
			If thisPort <> -1 Then
				Dim thatPort As Integer = that.location.port
				Dim port As Integer = If(thatPort <> -1, thatPort, that.location.defaultPort)
				If thisPort <> port Then Return False
			End If

			If location.file.EndsWith("/-") Then
				' Matches the directory and (recursively) all files
				' and subdirectories contained in that directory.
				' For example, "/a/b/-" implies anything that starts with
				' "/a/b/"
				Dim thisPath As String = location.file.Substring(0, location.file.length()-1)
				If Not that.location.file.StartsWith(thisPath) Then Return False
			ElseIf location.file.EndsWith("/*") Then
				' Matches the directory and all the files contained in that
				' directory.
				' For example, "/a/b/*" implies anything that starts with
				' "/a/b/" but has no further slashes
				Dim last As Integer = that.location.file.LastIndexOf("/"c)
				If last = -1 Then Return False
				Dim thisPath As String = location.file.Substring(0, location.file.length()-1)
				Dim thatPath As String = that.location.file.Substring(0, last+1)
				If Not thatPath.Equals(thisPath) Then Return False
			Else
				' Exact matches only.
				' For example, "/a/b" and "/a/b/" both imply "/a/b/"
				If ((Not that.location.file.Equals(location.file))) AndAlso ((Not that.location.file.Equals(location.file & "/"))) Then Return False
			End If

			If location.ref IsNot Nothing AndAlso (Not location.ref.Equals(that.location.ref)) Then Return False

			Dim thisHost As String = location.host
			Dim thatHost As String = that.location.host
			If thisHost IsNot Nothing Then
				If ("".Equals(thisHost) OrElse "localhost".Equals(thisHost)) AndAlso ("".Equals(thatHost) OrElse "localhost".Equals(thatHost)) Then
					' ok
				ElseIf Not thisHost.Equals(thatHost) Then
					If thatHost Is Nothing Then Return False
					If Me.sp Is Nothing Then Me.sp = New java.net.SocketPermission(thisHost, "resolve")
					If that.sp Is Nothing Then that.sp = New java.net.SocketPermission(thatHost, "resolve")
					If Not Me.sp.implies(that.sp) Then Return False
				End If
			End If
			' everything matches
			Return True
		End Function

		''' <summary>
		''' Returns a string describing this CodeSource, telling its
		''' URL and certificates.
		''' </summary>
		''' <returns> information about this CodeSource. </returns>
		Public Overrides Function ToString() As String
			Dim sb As New StringBuilder
			sb.append("(")
			sb.append(Me.location)

			If Me.certs IsNot Nothing AndAlso Me.certs.Length > 0 Then
				For i As Integer = 0 To Me.certs.Length - 1
					sb.append(" " & Me.certs(i))
				Next i

			ElseIf Me.signers IsNot Nothing AndAlso Me.signers.Length > 0 Then
				For i As Integer = 0 To Me.signers.Length - 1
					sb.append(" " & Me.signers(i))
				Next i
			Else
				sb.append(" <no signer certificates>")
			End If
			sb.append(")")
			Return sb.ToString()
		End Function

		''' <summary>
		''' Writes this object out to a stream (i.e., serializes it).
		''' 
		''' @serialData An initial {@code URL} is followed by an
		''' {@code int} indicating the number of certificates to follow
		''' (a value of "zero" denotes that there are no certificates associated
		''' with this object).
		''' Each certificate is written out starting with a {@code String}
		''' denoting the certificate type, followed by an
		''' {@code int} specifying the length of the certificate encoding,
		''' followed by the certificate encoding itself which is written out as an
		''' array of bytes. Finally, if any code signers are present then the array
		''' of code signers is serialized and written out too.
		''' </summary>
		Private Sub writeObject(ByVal oos As java.io.ObjectOutputStream)
			oos.defaultWriteObject() ' location

			' Serialize the array of certs
			If certs Is Nothing OrElse certs.Length = 0 Then
				oos.writeInt(0)
			Else
				' write out the total number of certs
				oos.writeInt(certs.Length)
				' write out each cert, including its type
				For i As Integer = 0 To certs.Length - 1
					Dim cert As java.security.cert.Certificate = certs(i)
					Try
						oos.writeUTF(cert.type)
						Dim encoded As SByte() = cert.encoded
						oos.writeInt(encoded.Length)
						oos.write(encoded)
					Catch cee As CertificateEncodingException
						Throw New java.io.IOException(cee.Message)
					End Try
				Next i
			End If

			' Serialize the array of code signers (if any)
			If signers IsNot Nothing AndAlso signers.Length > 0 Then oos.writeObject(signers)
		End Sub

		''' <summary>
		''' Restores this object from a stream (i.e., deserializes it).
		''' </summary>
		Private Sub readObject(ByVal ois As java.io.ObjectInputStream)
			Dim cf As CertificateFactory
			Dim cfs As Dictionary(Of String, CertificateFactory) = Nothing

			ois.defaultReadObject() ' location

			' process any new-style certs in the stream (if present)
			Dim size As Integer = ois.readInt()
			If size > 0 Then
				' we know of 3 different cert types: X.509, PGP, SDSI, which
				' could all be present in the stream at the same time
				cfs = New Dictionary(Of String, CertificateFactory)(3)
				Me.certs = New java.security.cert.Certificate(size - 1){}
			End If

			For i As Integer = 0 To size - 1
				' read the certificate type, and instantiate a certificate
				' factory of that type (reuse existing factory if possible)
				Dim certType As String = ois.readUTF()
				If cfs.ContainsKey(certType) Then
					' reuse certificate factory
					cf = cfs(certType)
				Else
					' create new certificate factory
					Try
						cf = CertificateFactory.getInstance(certType)
					Catch ce As CertificateException
						Throw New ClassNotFoundException("Certificate factory for " & certType & " not found")
					End Try
					' store the certificate factory so we can reuse it later
					cfs(certType) = cf
				End If
				' parse the certificate
				Dim encoded As SByte() = Nothing
				Try
					encoded = New SByte(ois.readInt() - 1){}
				Catch oome As OutOfMemoryError
					Throw New java.io.IOException("Certificate too big")
				End Try
				ois.readFully(encoded)
				Dim bais As New java.io.ByteArrayInputStream(encoded)
				Try
					Me.certs(i) = cf.generateCertificate(bais)
				Catch ce As CertificateException
					Throw New java.io.IOException(ce.Message)
				End Try
				bais.close()
			Next i

			' Deserialize array of code signers (if any)
			Try
				Me.signers = CType(ois.readObject(), CodeSigner()).clone()
			Catch ioe As java.io.IOException
				' no signers present
			End Try
		End Sub

	'    
	'     * Convert an array of certificates to an array of code signers.
	'     * The array of certificates is a concatenation of certificate chains
	'     * where the initial certificate in each chain is the end-entity cert.
	'     *
	'     * @return An array of code signers or null if none are generated.
	'     
		Private Function convertCertArrayToSignerArray(ByVal certs As java.security.cert.Certificate()) As CodeSigner()

			If certs Is Nothing Then Return Nothing

			Try
				' Initialize certificate factory
				If factory Is Nothing Then factory = CertificateFactory.getInstance("X.509")

				' Iterate through all the certificates
				Dim i As Integer = 0
				Dim signers As IList(Of CodeSigner) = New List(Of CodeSigner)
				Do While i < certs.Length
					Dim certChain As IList(Of java.security.cert.Certificate) = New List(Of java.security.cert.Certificate)
					certChain.Add(certs(i))
					i += 1
					Dim j As Integer = i

					' Extract chain of certificates
					' (loop while certs are not end-entity certs)
					Do While j < certs.Length AndAlso TypeOf certs(j) Is X509Certificate AndAlso CType(certs(j), X509Certificate).basicConstraints <> -1
						certChain.Add(certs(j))
						j += 1
					Loop
					i = j
					Dim certPath As CertPath = factory.generateCertPath(certChain)
					signers.Add(New CodeSigner(certPath, Nothing))
				Loop

				If signers.Count = 0 Then
					Return Nothing
				Else
					Return signers.ToArray()
				End If

			Catch e As CertificateException
				Return Nothing 'TODO - may be better to throw an ex. here
			End Try
		End Function
	End Class

End Namespace