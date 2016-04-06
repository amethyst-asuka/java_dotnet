Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 2007, 2014, Oracle and/or its affiliates. All rights reserved.
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
	''' An exception that indicates an X.509 certificate is revoked. A
	''' {@code CertificateRevokedException} contains additional information
	''' about the revoked certificate, such as the date on which the
	''' certificate was revoked and the reason it was revoked.
	''' 
	''' @author Sean Mullan
	''' @since 1.7 </summary>
	''' <seealso cref= CertPathValidatorException </seealso>
	Public Class CertificateRevokedException
		Inherits CertificateException

		Private Shadows Const serialVersionUID As Long = 7839996631571608627L

		''' <summary>
		''' @serial the date on which the certificate was revoked
		''' </summary>
		Private revocationDate As DateTime?
		''' <summary>
		''' @serial the revocation reason
		''' </summary>
		Private ReadOnly reason As CRLReason
		''' <summary>
		''' @serial the {@code X500Principal} that represents the name of the
		''' authority that signed the certificate's revocation status information
		''' </summary>
		Private ReadOnly authority As javax.security.auth.x500.X500Principal

		<NonSerialized> _
		Private extensions As IDictionary(Of String, Extension)

		''' <summary>
		''' Constructs a {@code CertificateRevokedException} with
		''' the specified revocation date, reason code, authority name, and map
		''' of extensions.
		''' </summary>
		''' <param name="revocationDate"> the date on which the certificate was revoked. The
		'''    date is copied to protect against subsequent modification. </param>
		''' <param name="reason"> the revocation reason </param>
		''' <param name="extensions"> a map of X.509 Extensions. Each key is an OID String
		'''    that maps to the corresponding Extension. The map is copied to
		'''    prevent subsequent modification. </param>
		''' <param name="authority"> the {@code X500Principal} that represents the name
		'''    of the authority that signed the certificate's revocation status
		'''    information </param>
		''' <exception cref="NullPointerException"> if {@code revocationDate},
		'''    {@code reason}, {@code authority}, or
		'''    {@code extensions} is {@code null} </exception>
		Public Sub New(  revocationDate As DateTime?,   reason As CRLReason,   authority As javax.security.auth.x500.X500Principal,   extensions As IDictionary(Of String, Extension))
			If revocationDate Is Nothing OrElse reason Is Nothing OrElse authority Is Nothing OrElse extensions Is Nothing Then Throw New NullPointerException
			Me.revocationDate = New DateTime?(revocationDate.Value.time)
			Me.reason = reason
			Me.authority = authority
			' make sure Map only contains correct types
			Me.extensions = java.util.Collections.checkedMap(New Dictionary(Of ), GetType(String), GetType(Extension))
'JAVA TO VB CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
			Me.extensions.putAll(extensions)
		End Sub

		''' <summary>
		''' Returns the date on which the certificate was revoked. A new copy is
		''' returned each time the method is invoked to protect against subsequent
		''' modification.
		''' </summary>
		''' <returns> the revocation date </returns>
		Public Overridable Property revocationDate As DateTime?
			Get
				Return CDate(revocationDate.Value.clone())
			End Get
		End Property

		''' <summary>
		''' Returns the reason the certificate was revoked.
		''' </summary>
		''' <returns> the revocation reason </returns>
		Public Overridable Property revocationReason As CRLReason
			Get
				Return reason
			End Get
		End Property

		''' <summary>
		''' Returns the name of the authority that signed the certificate's
		''' revocation status information.
		''' </summary>
		''' <returns> the {@code X500Principal} that represents the name of the
		'''     authority that signed the certificate's revocation status information </returns>
		Public Overridable Property authorityName As javax.security.auth.x500.X500Principal
			Get
				Return authority
			End Get
		End Property

		''' <summary>
		''' Returns the invalidity date, as specified in the Invalidity Date
		''' extension of this {@code CertificateRevokedException}. The
		''' invalidity date is the date on which it is known or suspected that the
		''' private key was compromised or that the certificate otherwise became
		''' invalid. This implementation calls {@code getExtensions()} and
		''' checks the returned map for an entry for the Invalidity Date extension
		''' OID ("2.5.29.24"). If found, it returns the invalidity date in the
		''' extension; otherwise null. A new Date object is returned each time the
		''' method is invoked to protect against subsequent modification.
		''' </summary>
		''' <returns> the invalidity date, or {@code null} if not specified </returns>
		Public Overridable Property invalidityDate As DateTime?
			Get
				Dim ext As Extension = extensions("2.5.29.24")
				If ext Is Nothing Then
					Return Nothing
				Else
					Try
						Dim invalidity As DateTime? = sun.security.x509.InvalidityDateExtension.toImpl(ext).get("DATE")
						Return New DateTime?(invalidity.Value.time)
					Catch ioe As java.io.IOException
						Return Nothing
					End Try
				End If
			End Get
		End Property

		''' <summary>
		''' Returns a map of X.509 extensions containing additional information
		''' about the revoked certificate, such as the Invalidity Date
		''' Extension. Each key is an OID String that maps to the corresponding
		''' Extension.
		''' </summary>
		''' <returns> an unmodifiable map of X.509 extensions, or an empty map
		'''    if there are no extensions </returns>
		Public Overridable Property extensions As IDictionary(Of String, Extension)
			Get
				Return java.util.Collections.unmodifiableMap(extensions)
			End Get
		End Property

		Public  Overrides ReadOnly Property  message As String
			Get
				Return "Certificate has been revoked, reason: " & reason & ", revocation date: " & revocationDate & ", authority: " & authority & ", extension OIDs: " & extensions.Keys
			End Get
		End Property

		''' <summary>
		''' Serialize this {@code CertificateRevokedException} instance.
		''' 
		''' @serialData the size of the extensions map (int), followed by all of
		''' the extensions in the map, in no particular order. For each extension,
		''' the following data is emitted: the OID String (Object), the criticality
		''' flag (boolean), the length of the encoded extension value byte array
		''' (int), and the encoded extension value bytes.
		''' </summary>
		Private Sub writeObject(  oos As java.io.ObjectOutputStream)
			' Write out the non-transient fields
			' (revocationDate, reason, authority)
			oos.defaultWriteObject()

			' Write out the size (number of mappings) of the extensions map
			oos.writeInt(extensions.Count)

			' For each extension in the map, the following are emitted (in order):
			' the OID String (Object), the criticality flag (boolean), the length
			' of the encoded extension value byte array (int), and the encoded
			' extension value byte array. The extensions themselves are emitted
			' in no particular order.
			For Each entry As KeyValuePair(Of String, Extension) In extensions
				Dim ext As Extension = entry.Value
				oos.writeObject(ext.id)
				oos.writeBoolean(ext.critical)
				Dim extVal As SByte() = ext.value
				oos.writeInt(extVal.Length)
				oos.write(extVal)
			Next entry
		End Sub

		''' <summary>
		''' Deserialize the {@code CertificateRevokedException} instance.
		''' </summary>
		Private Sub readObject(  ois As java.io.ObjectInputStream)
			' Read in the non-transient fields
			' (revocationDate, reason, authority)
			ois.defaultReadObject()

			' Defensively copy the revocation date
			revocationDate = New DateTime?(revocationDate.Value.time)

			' Read in the size (number of mappings) of the extensions map
			' and create the extensions map
			Dim size As Integer = ois.readInt()
			If size = 0 Then
				extensions = java.util.Collections.emptyMap()
			Else
				extensions = New Dictionary(Of String, Extension)(size)
			End If

			' Read in the extensions and put the mappings in the extensions map
			For i As Integer = 0 To size - 1
				Dim oid As String = CStr(ois.readObject())
				Dim critical As Boolean = ois.readBoolean()
				Dim length As Integer = ois.readInt()
				Dim extVal As SByte() = New SByte(length - 1){}
				ois.readFully(extVal)
				Dim ext As Extension = sun.security.x509.Extension.newExtension(New sun.security.util.ObjectIdentifier(oid), critical, extVal)
				extensions(oid) = ext
			Next i
		End Sub
	End Class

End Namespace