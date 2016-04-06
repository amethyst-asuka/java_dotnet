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

Namespace java.util.jar


	''' <summary>
	''' This class is used to represent a JAR file entry.
	''' </summary>
	Public Class JarEntry
		Inherits java.util.zip.ZipEntry

		Friend attr As Attributes
		Friend certs As java.security.cert.Certificate()
		Friend signers As java.security.CodeSigner()

		''' <summary>
		''' Creates a new <code>JarEntry</code> for the specified JAR file
		''' entry name.
		''' </summary>
		''' <param name="name"> the JAR file entry name </param>
		''' <exception cref="NullPointerException"> if the entry name is <code>null</code> </exception>
		''' <exception cref="IllegalArgumentException"> if the entry name is longer than
		'''            0xFFFF bytes. </exception>
		Public Sub New(  name As String)
			MyBase.New(name)
		End Sub

		''' <summary>
		''' Creates a new <code>JarEntry</code> with fields taken from the
		''' specified <code>ZipEntry</code> object. </summary>
		''' <param name="ze"> the <code>ZipEntry</code> object to create the
		'''           <code>JarEntry</code> from </param>
		Public Sub New(  ze As java.util.zip.ZipEntry)
			MyBase.New(ze)
		End Sub

		''' <summary>
		''' Creates a new <code>JarEntry</code> with fields taken from the
		''' specified <code>JarEntry</code> object.
		''' </summary>
		''' <param name="je"> the <code>JarEntry</code> to copy </param>
		Public Sub New(  je As JarEntry)
			Me.New(CType(je, java.util.zip.ZipEntry))
			Me.attr = je.attr
			Me.certs = je.certs
			Me.signers = je.signers
		End Sub

		''' <summary>
		''' Returns the <code>Manifest</code> <code>Attributes</code> for this
		''' entry, or <code>null</code> if none.
		''' </summary>
		''' <returns> the <code>Manifest</code> <code>Attributes</code> for this
		''' entry, or <code>null</code> if none </returns>
		''' <exception cref="IOException">  if an I/O error has occurred </exception>
		Public Overridable Property attributes As Attributes
			Get
				Return attr
			End Get
		End Property

		''' <summary>
		''' Returns the <code>Certificate</code> objects for this entry, or
		''' <code>null</code> if none. This method can only be called once
		''' the <code>JarEntry</code> has been completely verified by reading
		''' from the entry input stream until the end of the stream has been
		''' reached. Otherwise, this method will return <code>null</code>.
		''' 
		''' <p>The returned certificate array comprises all the signer certificates
		''' that were used to verify this entry. Each signer certificate is
		''' followed by its supporting certificate chain (which may be empty).
		''' Each signer certificate and its supporting certificate chain are ordered
		''' bottom-to-top (i.e., with the signer certificate first and the (root)
		''' certificate authority last).
		''' </summary>
		''' <returns> the <code>Certificate</code> objects for this entry, or
		''' <code>null</code> if none. </returns>
		Public Overridable Property certificates As java.security.cert.Certificate()
			Get
				Return If(certs Is Nothing, Nothing, certs.clone())
			End Get
		End Property

		''' <summary>
		''' Returns the <code>CodeSigner</code> objects for this entry, or
		''' <code>null</code> if none. This method can only be called once
		''' the <code>JarEntry</code> has been completely verified by reading
		''' from the entry input stream until the end of the stream has been
		''' reached. Otherwise, this method will return <code>null</code>.
		''' 
		''' <p>The returned array comprises all the code signers that have signed
		''' this entry.
		''' </summary>
		''' <returns> the <code>CodeSigner</code> objects for this entry, or
		''' <code>null</code> if none.
		''' 
		''' @since 1.5 </returns>
		Public Overridable Property codeSigners As java.security.CodeSigner()
			Get
				Return If(signers Is Nothing, Nothing, signers.clone())
			End Get
		End Property
	End Class

End Namespace