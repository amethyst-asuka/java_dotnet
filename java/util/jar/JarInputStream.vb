'
' * Copyright (c) 1997, 2011, Oracle and/or its affiliates. All rights reserved.
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
	''' The <code>JarInputStream</code> class is used to read the contents of
	''' a JAR file from any input stream. It extends the class
	''' <code>java.util.zip.ZipInputStream</code> with support for reading
	''' an optional <code>Manifest</code> entry. The <code>Manifest</code>
	''' can be used to store meta-information about the JAR file and its entries.
	''' 
	''' @author  David Connelly </summary>
	''' <seealso cref=     Manifest </seealso>
	''' <seealso cref=     java.util.zip.ZipInputStream
	''' @since   1.2 </seealso>
	Public Class JarInputStream
		Inherits ZipInputStream

		Private man As Manifest
		Private first As JarEntry
		Private jv As JarVerifier
		Private mev As sun.security.util.ManifestEntryVerifier
		Private ReadOnly doVerify As Boolean
		Private tryManifest As Boolean

		''' <summary>
		''' Creates a new <code>JarInputStream</code> and reads the optional
		''' manifest. If a manifest is present, also attempts to verify
		''' the signatures if the JarInputStream is signed. </summary>
		''' <param name="in"> the actual input stream </param>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		Public Sub New(ByVal [in] As InputStream)
			Me.New([in], True)
		End Sub

		''' <summary>
		''' Creates a new <code>JarInputStream</code> and reads the optional
		''' manifest. If a manifest is present and verify is true, also attempts
		''' to verify the signatures if the JarInputStream is signed.
		''' </summary>
		''' <param name="in"> the actual input stream </param>
		''' <param name="verify"> whether or not to verify the JarInputStream if
		''' it is signed. </param>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		Public Sub New(ByVal [in] As InputStream, ByVal verify As Boolean)
			MyBase.New([in])
			Me.doVerify = verify

			' This implementation assumes the META-INF/MANIFEST.MF entry
			' should be either the first or the second entry (when preceded
			' by the dir META-INF/). It skips the META-INF/ and then
			' "consumes" the MANIFEST.MF to initialize the Manifest object.
			Dim e As JarEntry = CType(MyBase.nextEntry, JarEntry)
			If e IsNot Nothing AndAlso e.name.equalsIgnoreCase("META-INF/") Then e = CType(MyBase.nextEntry, JarEntry)
			first = checkManifest(e)
		End Sub

		Private Function checkManifest(ByVal e As JarEntry) As JarEntry
			If e IsNot Nothing AndAlso JarFile.MANIFEST_NAME.equalsIgnoreCase(e.name) Then
				man = New Manifest
				Dim bytes_Renamed As SByte() = getBytes(New BufferedInputStream(Me))
				man.read(New ByteArrayInputStream(bytes_Renamed))
				closeEntry()
				If doVerify Then
					jv = New JarVerifier(bytes_Renamed)
					mev = New sun.security.util.ManifestEntryVerifier(man)
				End If
				Return CType(MyBase.nextEntry, JarEntry)
			End If
			Return e
		End Function

		Private Function getBytes(ByVal [is] As InputStream) As SByte()
			Dim buffer As SByte() = New SByte(8191){}
			Dim baos As New ByteArrayOutputStream(2048)
			Dim n As Integer
			n = [is].read(buffer, 0, buffer.Length)
			Do While n <> -1
				baos.write(buffer, 0, n)
				n = [is].read(buffer, 0, buffer.Length)
			Loop
			Return baos.toByteArray()
		End Function

		''' <summary>
		''' Returns the <code>Manifest</code> for this JAR file, or
		''' <code>null</code> if none.
		''' </summary>
		''' <returns> the <code>Manifest</code> for this JAR file, or
		'''         <code>null</code> if none. </returns>
		Public Overridable Property manifest As Manifest
			Get
				Return man
			End Get
		End Property

		''' <summary>
		''' Reads the next ZIP file entry and positions the stream at the
		''' beginning of the entry data. If verification has been enabled,
		''' any invalid signature detected while positioning the stream for
		''' the next entry will result in an exception. </summary>
		''' <exception cref="ZipException"> if a ZIP file error has occurred </exception>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		''' <exception cref="SecurityException"> if any of the jar file entries
		'''         are incorrectly signed. </exception>
		Public Property Overrides nextEntry As ZipEntry
			Get
				Dim e As JarEntry
				If first Is Nothing Then
					e = CType(MyBase.nextEntry, JarEntry)
					If tryManifest Then
						e = checkManifest(e)
						tryManifest = False
					End If
				Else
					e = first
					If first.name.equalsIgnoreCase(sun.misc.JarIndex.INDEX_NAME) Then tryManifest = True
					first = Nothing
				End If
				If jv IsNot Nothing AndAlso e IsNot Nothing Then
					' At this point, we might have parsed all the meta-inf
					' entries and have nothing to verify. If we have
					' nothing to verify, get rid of the JarVerifier object.
					If jv.nothingToVerify() = True Then
						jv = Nothing
						mev = Nothing
					Else
						jv.beginEntry(e, mev)
					End If
				End If
				Return e
			End Get
		End Property

		''' <summary>
		''' Reads the next JAR file entry and positions the stream at the
		''' beginning of the entry data. If verification has been enabled,
		''' any invalid signature detected while positioning the stream for
		''' the next entry will result in an exception. </summary>
		''' <returns> the next JAR file entry, or null if there are no more entries </returns>
		''' <exception cref="ZipException"> if a ZIP file error has occurred </exception>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		''' <exception cref="SecurityException"> if any of the jar file entries
		'''         are incorrectly signed. </exception>
		Public Overridable Property nextJarEntry As JarEntry
			Get
				Return CType(nextEntry, JarEntry)
			End Get
		End Property

		''' <summary>
		''' Reads from the current JAR file entry into an array of bytes.
		''' If <code>len</code> is not zero, the method
		''' blocks until some input is available; otherwise, no
		''' bytes are read and <code>0</code> is returned.
		''' If verification has been enabled, any invalid signature
		''' on the current entry will be reported at some point before the
		''' end of the entry is reached. </summary>
		''' <param name="b"> the buffer into which the data is read </param>
		''' <param name="off"> the start offset in the destination array <code>b</code> </param>
		''' <param name="len"> the maximum number of bytes to read </param>
		''' <returns> the actual number of bytes read, or -1 if the end of the
		'''         entry is reached </returns>
		''' <exception cref="NullPointerException"> If <code>b</code> is <code>null</code>. </exception>
		''' <exception cref="IndexOutOfBoundsException"> If <code>off</code> is negative,
		''' <code>len</code> is negative, or <code>len</code> is greater than
		''' <code>b.length - off</code> </exception>
		''' <exception cref="ZipException"> if a ZIP file error has occurred </exception>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		''' <exception cref="SecurityException"> if any of the jar file entries
		'''         are incorrectly signed. </exception>
		Public Overrides Function read(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer) As Integer
			Dim n As Integer
			If first Is Nothing Then
				n = MyBase.read(b, [off], len)
			Else
				n = -1
			End If
			If jv IsNot Nothing Then jv.update(n, b, [off], len, mev)
			Return n
		End Function

		''' <summary>
		''' Creates a new <code>JarEntry</code> (<code>ZipEntry</code>) for the
		''' specified JAR file entry name. The manifest attributes of
		''' the specified JAR file entry name will be copied to the new
		''' <CODE>JarEntry</CODE>.
		''' </summary>
		''' <param name="name"> the name of the JAR/ZIP file entry </param>
		''' <returns> the <code>JarEntry</code> object just created </returns>
		Protected Friend Overrides Function createZipEntry(ByVal name As String) As ZipEntry
			Dim e As New JarEntry(name)
			If man IsNot Nothing Then e.attr = man.getAttributes(name)
			Return e
		End Function
	End Class

End Namespace