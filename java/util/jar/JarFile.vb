Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System.Runtime.InteropServices

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
	''' The <code>JarFile</code> class is used to read the contents of a jar file
	''' from any file that can be opened with <code>java.io.RandomAccessFile</code>.
	''' It extends the class <code>java.util.zip.ZipFile</code> with support
	''' for reading an optional <code>Manifest</code> entry. The
	''' <code>Manifest</code> can be used to specify meta-information about the
	''' jar file and its entries.
	''' 
	''' <p> Unless otherwise noted, passing a <tt>null</tt> argument to a constructor
	''' or method in this class will cause a <seealso cref="NullPointerException"/> to be
	''' thrown.
	''' 
	''' If the verify flag is on when opening a signed jar file, the content of the
	''' file is verified against its signature embedded inside the file. Please note
	''' that the verification process does not include validating the signer's
	''' certificate. A caller should inspect the return value of
	''' <seealso cref="JarEntry#getCodeSigners()"/> to further determine if the signature
	''' can be trusted.
	''' 
	''' @author  David Connelly </summary>
	''' <seealso cref=     Manifest </seealso>
	''' <seealso cref=     java.util.zip.ZipFile </seealso>
	''' <seealso cref=     java.util.jar.JarEntry
	''' @since   1.2 </seealso>
	Public Class JarFile
		Inherits ZipFile

		Private manRef As SoftReference(Of Manifest)
		Private manEntry As JarEntry
		Private jv As JarVerifier
		Private jvInitialized As Boolean
		Private verify As Boolean

		' indicates if Class-Path attribute present (only valid if hasCheckedSpecialAttributes true)
		Private hasClassPathAttribute_Renamed As Boolean
		' true if manifest checked for special attributes
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private hasCheckedSpecialAttributes As Boolean

		' Set up JavaUtilJarAccess in SharedSecrets
		Shared Sub New()
			sun.misc.SharedSecrets.javaUtilJarAccess = New JavaUtilJarAccessImpl
			CLASSPATH_LASTOCC = New Integer(127){}
			CLASSPATH_OPTOSFT = New Integer(9){}
			CLASSPATH_LASTOCC(AscW("c"c)) = 1
			CLASSPATH_LASTOCC(AscW("l"c)) = 2
			CLASSPATH_LASTOCC(AscW("s"c)) = 5
			CLASSPATH_LASTOCC(AscW("-"c)) = 6
			CLASSPATH_LASTOCC(AscW("p"c)) = 7
			CLASSPATH_LASTOCC(AscW("a"c)) = 8
			CLASSPATH_LASTOCC(AscW("t"c)) = 9
			CLASSPATH_LASTOCC(AscW("h"c)) = 10
			For i As Integer = 0 To 8
				CLASSPATH_OPTOSFT(i) = 10
			Next i
			CLASSPATH_OPTOSFT(9)=1
		End Sub

		''' <summary>
		''' The JAR manifest file name.
		''' </summary>
		Public Const MANIFEST_NAME As String = "META-INF/MANIFEST.MF"

		''' <summary>
		''' Creates a new <code>JarFile</code> to read from the specified
		''' file <code>name</code>. The <code>JarFile</code> will be verified if
		''' it is signed. </summary>
		''' <param name="name"> the name of the jar file to be opened for reading </param>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		''' <exception cref="SecurityException"> if access to the file is denied
		'''         by the SecurityManager </exception>
		Public Sub New(ByVal name As String)
			Me.New(New File(name), True, ZipFile.OPEN_READ)
		End Sub

		''' <summary>
		''' Creates a new <code>JarFile</code> to read from the specified
		''' file <code>name</code>. </summary>
		''' <param name="name"> the name of the jar file to be opened for reading </param>
		''' <param name="verify"> whether or not to verify the jar file if
		''' it is signed. </param>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		''' <exception cref="SecurityException"> if access to the file is denied
		'''         by the SecurityManager </exception>
		Public Sub New(ByVal name As String, ByVal verify As Boolean)
			Me.New(New File(name), verify, ZipFile.OPEN_READ)
		End Sub

		''' <summary>
		''' Creates a new <code>JarFile</code> to read from the specified
		''' <code>File</code> object. The <code>JarFile</code> will be verified if
		''' it is signed. </summary>
		''' <param name="file"> the jar file to be opened for reading </param>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		''' <exception cref="SecurityException"> if access to the file is denied
		'''         by the SecurityManager </exception>
		Public Sub New(ByVal file_Renamed As File)
			Me.New(file_Renamed, True, ZipFile.OPEN_READ)
		End Sub


		''' <summary>
		''' Creates a new <code>JarFile</code> to read from the specified
		''' <code>File</code> object. </summary>
		''' <param name="file"> the jar file to be opened for reading </param>
		''' <param name="verify"> whether or not to verify the jar file if
		''' it is signed. </param>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		''' <exception cref="SecurityException"> if access to the file is denied
		'''         by the SecurityManager. </exception>
		Public Sub New(ByVal file_Renamed As File, ByVal verify As Boolean)
			Me.New(file_Renamed, verify, ZipFile.OPEN_READ)
		End Sub


		''' <summary>
		''' Creates a new <code>JarFile</code> to read from the specified
		''' <code>File</code> object in the specified mode.  The mode argument
		''' must be either <tt>OPEN_READ</tt> or <tt>OPEN_READ | OPEN_DELETE</tt>.
		''' </summary>
		''' <param name="file"> the jar file to be opened for reading </param>
		''' <param name="verify"> whether or not to verify the jar file if
		''' it is signed. </param>
		''' <param name="mode"> the mode in which the file is to be opened </param>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		''' <exception cref="IllegalArgumentException">
		'''         if the <tt>mode</tt> argument is invalid </exception>
		''' <exception cref="SecurityException"> if access to the file is denied
		'''         by the SecurityManager
		''' @since 1.3 </exception>
		Public Sub New(ByVal file_Renamed As File, ByVal verify As Boolean, ByVal mode As Integer)
			MyBase.New(file_Renamed, mode)
			Me.verify = verify
		End Sub

		''' <summary>
		''' Returns the jar file manifest, or <code>null</code> if none.
		''' </summary>
		''' <returns> the jar file manifest, or <code>null</code> if none
		''' </returns>
		''' <exception cref="IllegalStateException">
		'''         may be thrown if the jar file has been closed </exception>
		''' <exception cref="IOException">  if an I/O error has occurred </exception>
		Public Overridable Property manifest As Manifest
			Get
				Return manifestFromReference
			End Get
		End Property

		Private Property manifestFromReference As Manifest
			Get
				Dim man As Manifest = If(manRef IsNot Nothing, manRef.get(), Nothing)
    
				If man Is Nothing Then
    
					Dim manEntry_Renamed As JarEntry = manEntry
    
					' If found then load the manifest
					If manEntry_Renamed IsNot Nothing Then
						If verify Then
							Dim b As SByte() = getBytes(manEntry_Renamed)
							man = New Manifest(New ByteArrayInputStream(b))
							If Not jvInitialized Then jv = New JarVerifier(b)
						Else
							man = New Manifest(MyBase.getInputStream(manEntry_Renamed))
						End If
						manRef = New SoftReference(Of )(man)
					End If
				End If
				Return man
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Function getMetaInfEntryNames() As String()
		End Function

		''' <summary>
		''' Returns the <code>JarEntry</code> for the given entry name or
		''' <code>null</code> if not found.
		''' </summary>
		''' <param name="name"> the jar file entry name </param>
		''' <returns> the <code>JarEntry</code> for the given entry name or
		'''         <code>null</code> if not found.
		''' </returns>
		''' <exception cref="IllegalStateException">
		'''         may be thrown if the jar file has been closed
		''' </exception>
		''' <seealso cref= java.util.jar.JarEntry </seealso>
		Public Overridable Function getJarEntry(ByVal name As String) As JarEntry
			Return CType(getEntry(name), JarEntry)
		End Function

		''' <summary>
		''' Returns the <code>ZipEntry</code> for the given entry name or
		''' <code>null</code> if not found.
		''' </summary>
		''' <param name="name"> the jar file entry name </param>
		''' <returns> the <code>ZipEntry</code> for the given entry name or
		'''         <code>null</code> if not found
		''' </returns>
		''' <exception cref="IllegalStateException">
		'''         may be thrown if the jar file has been closed
		''' </exception>
		''' <seealso cref= java.util.zip.ZipEntry </seealso>
		Public Overrides Function getEntry(ByVal name As String) As ZipEntry
			Dim ze As ZipEntry = MyBase.getEntry(name)
			If ze IsNot Nothing Then Return New JarFileEntry(Me, ze)
			Return Nothing
		End Function

		Private Class JarEntryIterator
			Implements Enumeration(Of JarEntry), Iterator(Of JarEntry)

			Private ReadOnly outerInstance As JarFile

			Public Sub New(ByVal outerInstance As JarFile)
				Me.outerInstance = outerInstance
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly e As Enumeration(Of ? As ZipEntry) = outerInstance.entries()

			Public Overridable Function hasNext() As Boolean Implements Iterator(Of JarEntry).hasNext
				Return e.hasMoreElements()
			End Function

			Public Overridable Function [next]() As JarEntry
				Dim ze As ZipEntry = e.nextElement()
				Return New JarFileEntry(ze)
			End Function

			Public Overridable Function hasMoreElements() As Boolean Implements Enumeration(Of JarEntry).hasMoreElements
				Return hasNext()
			End Function

			Public Overridable Function nextElement() As JarEntry
				Return [next]()
			End Function
		End Class

		''' <summary>
		''' Returns an enumeration of the zip file entries.
		''' </summary>
		Public Overrides Function entries() As Enumeration(Of JarEntry)
			Return New JarEntryIterator(Me)
		End Function

		Public Overrides Function stream() As java.util.stream.Stream(Of JarEntry)
			Return java.util.stream.StreamSupport.stream(Spliterators.spliterator(New JarEntryIterator(Me), size(), Spliterator.ORDERED Or Spliterator.DISTINCT Or Spliterator.IMMUTABLE Or Spliterator.NONNULL), False)
		End Function

		Private Class JarFileEntry
			Inherits JarEntry

			Private ReadOnly outerInstance As JarFile

			Friend Sub New(ByVal outerInstance As JarFile, ByVal ze As ZipEntry)
					Me.outerInstance = outerInstance
				MyBase.New(ze)
			End Sub
			Public Property Overrides attributes As Attributes
				Get
					Dim man As Manifest = outerInstance.manifest
					If man IsNot Nothing Then
						Return man.getAttributes(name)
					Else
						Return Nothing
					End If
				End Get
			End Property
			Public Property Overrides certificates As java.security.cert.Certificate()
				Get
					Try
						outerInstance.maybeInstantiateVerifier()
					Catch e As IOException
						Throw New RuntimeException(e)
					End Try
					If certs Is Nothing AndAlso outerInstance.jv IsNot Nothing Then certs = outerInstance.jv.getCerts(JarFile.this, Me)
					Return If(certs Is Nothing, Nothing, certs.clone())
				End Get
			End Property
			Public Property Overrides codeSigners As java.security.CodeSigner()
				Get
					Try
						outerInstance.maybeInstantiateVerifier()
					Catch e As IOException
						Throw New RuntimeException(e)
					End Try
					If signers Is Nothing AndAlso outerInstance.jv IsNot Nothing Then signers = outerInstance.jv.getCodeSigners(JarFile.this, Me)
					Return If(signers Is Nothing, Nothing, signers.clone())
				End Get
			End Property
		End Class

	'    
	'     * Ensures that the JarVerifier has been created if one is
	'     * necessary (i.e., the jar appears to be signed.) This is done as
	'     * a quick check to avoid processing of the manifest for unsigned
	'     * jars.
	'     
		Private Sub maybeInstantiateVerifier()
			If jv IsNot Nothing Then Return

			If verify Then
				Dim names As String() = metaInfEntryNames
				If names IsNot Nothing Then
					For i As Integer = 0 To names.Length - 1
						Dim name_Renamed As String = names(i).ToUpper(Locale.ENGLISH)
						If name_Renamed.EndsWith(".DSA") OrElse name_Renamed.EndsWith(".RSA") OrElse name_Renamed.EndsWith(".EC") OrElse name_Renamed.EndsWith(".SF") Then
							' Assume since we found a signature-related file
							' that the jar is signed and that we therefore
							' need a JarVerifier and Manifest
							manifest
							Return
						End If
					Next i
				End If
				' No signature-related files; don't instantiate a
				' verifier
				verify = False
			End If
		End Sub


	'    
	'     * Initializes the verifier object by reading all the manifest
	'     * entries and passing them to the verifier.
	'     
		Private Sub initializeVerifier()
			Dim mev As sun.security.util.ManifestEntryVerifier = Nothing

			' Verify "META-INF/" entries...
			Try
				Dim names As String() = metaInfEntryNames
				If names IsNot Nothing Then
					For i As Integer = 0 To names.Length - 1
						Dim uname As String = names(i).ToUpper(Locale.ENGLISH)
						If MANIFEST_NAME.Equals(uname) OrElse sun.security.util.SignatureFileVerifier.isBlockOrSF(uname) Then
							Dim e As JarEntry = getJarEntry(names(i))
							If e Is Nothing Then Throw New JarException("corrupted jar file")
							If mev Is Nothing Then mev = New sun.security.util.ManifestEntryVerifier(manifestFromReference)
							Dim b As SByte() = getBytes(e)
							If b IsNot Nothing AndAlso b.Length > 0 Then
								jv.beginEntry(e, mev)
								jv.update(b.Length, b, 0, b.Length, mev)
								jv.update(-1, Nothing, 0, 0, mev)
							End If
						End If
					Next i
				End If
			Catch ex As IOException
				' if we had an error parsing any blocks, just
				' treat the jar file as being unsigned
				jv = Nothing
				verify = False
				If JarVerifier.debug IsNot Nothing Then
					JarVerifier.debug.println("jarfile parsing error!")
					Console.WriteLine(ex.ToString())
					Console.Write(ex.StackTrace)
				End If
			End Try

			' if after initializing the verifier we have nothing
			' signed, we null it out.

			If jv IsNot Nothing Then

				jv.doneWithMeta()
				If JarVerifier.debug IsNot Nothing Then JarVerifier.debug.println("done with meta!")

				If jv.nothingToVerify() Then
					If JarVerifier.debug IsNot Nothing Then JarVerifier.debug.println("nothing to verify!")
					jv = Nothing
					verify = False
				End If
			End If
		End Sub

	'    
	'     * Reads all the bytes for a given entry. Used to process the
	'     * META-INF files.
	'     
		Private Function getBytes(ByVal ze As ZipEntry) As SByte()
			Using [is] As InputStream = MyBase.getInputStream(ze)
				Return sun.misc.IOUtils.readFully([is], CInt(ze.size), True)
			End Using
		End Function

		''' <summary>
		''' Returns an input stream for reading the contents of the specified
		''' zip file entry. </summary>
		''' <param name="ze"> the zip file entry </param>
		''' <returns> an input stream for reading the contents of the specified
		'''         zip file entry </returns>
		''' <exception cref="ZipException"> if a zip file format error has occurred </exception>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		''' <exception cref="SecurityException"> if any of the jar file entries
		'''         are incorrectly signed. </exception>
		''' <exception cref="IllegalStateException">
		'''         may be thrown if the jar file has been closed </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function getInputStream(ByVal ze As ZipEntry) As InputStream
			maybeInstantiateVerifier()
			If jv Is Nothing Then Return MyBase.getInputStream(ze)
			If Not jvInitialized Then
				initializeVerifier()
				jvInitialized = True
				' could be set to null after a call to
				' initializeVerifier if we have nothing to
				' verify
				If jv Is Nothing Then Return MyBase.getInputStream(ze)
			End If

			' wrap a verifier stream around the real stream
			Return New JarVerifier.VerifierStream(manifestFromReference,If(TypeOf ze Is JarFileEntry, CType(ze, JarEntry), getJarEntry(ze.name)), MyBase.getInputStream(ze), jv)
		End Function

		' Statics for hand-coded Boyer-Moore search
		Private Shared ReadOnly CLASSPATH_CHARS As Char() = {"c"c,"l"c,"a"c,"s"c,"s"c,"-"c,"p"c,"a"c,"t"c,"h"c}
		' The bad character shift for "class-path"
		Private Shared ReadOnly CLASSPATH_LASTOCC As Integer()
		' The good suffix shift for "class-path"
		Private Shared ReadOnly CLASSPATH_OPTOSFT As Integer()


		Private Property manEntry As JarEntry
			Get
				If manEntry Is Nothing Then
					' First look up manifest entry using standard name
					manEntry = getJarEntry(MANIFEST_NAME)
					If manEntry Is Nothing Then
						' If not found, then iterate through all the "META-INF/"
						' entries to find a match.
						Dim names As String() = metaInfEntryNames
						If names IsNot Nothing Then
							For i As Integer = 0 To names.Length - 1
								If MANIFEST_NAME.Equals(names(i).ToUpper(Locale.ENGLISH)) Then
									manEntry = getJarEntry(names(i))
									Exit For
								End If
							Next i
						End If
					End If
				End If
				Return manEntry
			End Get
		End Property

	   ''' <summary>
	   ''' Returns {@code true} iff this JAR file has a manifest with the
	   ''' Class-Path attribute
	   ''' </summary>
		Friend Overridable Function hasClassPathAttribute() As Boolean
			checkForSpecialAttributes()
			Return hasClassPathAttribute_Renamed
		End Function

		''' <summary>
		''' Returns true if the pattern {@code src} is found in {@code b}.
		''' The {@code lastOcc} and {@code optoSft} arrays are the precomputed
		''' bad character and good suffix shifts.
		''' </summary>
		Private Function match(ByVal src As Char(), ByVal b As SByte(), ByVal lastOcc As Integer(), ByVal optoSft As Integer()) As Boolean
			Dim len As Integer = src.Length
			Dim last As Integer = b.Length - len
			Dim i As Integer = 0
			next:
			Do While i<=last
				For j As Integer = (len-1) To 0 Step -1
					Dim c As Char = ChrW(b(i+j))
					c = If(((AscW(c)-AscW("A"c)) Or (AscW("Z"c)-AscW(c))) >= 0, CChar(AscW(c) + 32), c)
					If c <> src(j) Then
						i += Math.Max(j + 1 - lastOcc(AscW(c) And &H7F), optoSft(j))
						GoTo next
					End If
				Next j
				Return True
			Loop
			Return False
		End Function

		''' <summary>
		''' On first invocation, check if the JAR file has the Class-Path
		''' attribute. A no-op on subsequent calls.
		''' </summary>
		Private Sub checkForSpecialAttributes()
			If hasCheckedSpecialAttributes Then Return
			If Not knownNotToHaveSpecialAttributes Then
				Dim manEntry_Renamed As JarEntry = manEntry
				If manEntry_Renamed IsNot Nothing Then
					Dim b As SByte() = getBytes(manEntry_Renamed)
					If match(CLASSPATH_CHARS, b, CLASSPATH_LASTOCC, CLASSPATH_OPTOSFT) Then hasClassPathAttribute_Renamed = True
				End If
			End If
			hasCheckedSpecialAttributes = True
		End Sub

		Private Shared javaHome As String
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private Shared jarNames As String()
		Private Property knownNotToHaveSpecialAttributes As Boolean
			Get
				' Optimize away even scanning of manifest for jar files we
				' deliver which don't have a class-path attribute. If one of
				' these jars is changed to include such an attribute this code
				' must be changed.
				If javaHome Is Nothing Then javaHome = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("java.home"))
				If jarNames Is Nothing Then
					Dim names As String() = New String(10){}
					Dim fileSep As String = File.separator
					Dim i As Integer = 0
					names(i) = fileSep & "rt.jar"
					i += 1
					names(i) = fileSep & "jsse.jar"
					i += 1
					names(i) = fileSep & "jce.jar"
					i += 1
					names(i) = fileSep & "charsets.jar"
					i += 1
					names(i) = fileSep & "dnsns.jar"
					i += 1
					names(i) = fileSep & "zipfs.jar"
					i += 1
					names(i) = fileSep & "localedata.jar"
					i += 1
						fileSep = "cldrdata.jar"
						names(i) = fileSep
					i += 1
					names(i) = fileSep & "sunjce_provider.jar"
					i += 1
					names(i) = fileSep & "sunpkcs11.jar"
					i += 1
					names(i) = fileSep & "sunec.jar"
					i += 1
					jarNames = names
				End If
    
				Dim name_Renamed As String = name
				Dim localJavaHome As String = javaHome
				If name_Renamed.StartsWith(localJavaHome) Then
					Dim names As String() = jarNames
					For i As Integer = 0 To names.Length - 1
						If name_Renamed.EndsWith(names(i)) Then Return True
					Next i
				End If
				Return False
			End Get
		End Property

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Sub ensureInitialization()
			Try
				maybeInstantiateVerifier()
			Catch e As IOException
				Throw New RuntimeException(e)
			End Try
			If jv IsNot Nothing AndAlso (Not jvInitialized) Then
				initializeVerifier()
				jvInitialized = True
			End If
		End Sub

		Friend Overridable Function newEntry(ByVal ze As ZipEntry) As JarEntry
			Return New JarFileEntry(Me, ze)
		End Function

		Friend Overridable Function entryNames(ByVal cs As java.security.CodeSource()) As Enumeration(Of String)
			ensureInitialization()
			If jv IsNot Nothing Then Return jv.entryNames(Me, cs)

	'        
	'         * JAR file has no signed content. Is there a non-signing
	'         * code source?
	'         
			Dim includeUnsigned As Boolean = False
			For i As Integer = 0 To cs.Length - 1
				If cs(i).codeSigners Is Nothing Then
					includeUnsigned = True
					Exit For
				End If
			Next i
			If includeUnsigned Then
				Return unsignedEntryNames()
			Else
				Return New EnumerationAnonymousInnerClassHelper(Of E)
			End If
		End Function

		Private Class EnumerationAnonymousInnerClassHelper(Of E)
			Implements Enumeration(Of E)

			Public Overridable Function hasMoreElements() As Boolean Implements Enumeration(Of E).hasMoreElements
				Return False
			End Function

			Public Overridable Function nextElement() As String
				Throw New NoSuchElementException
			End Function
		End Class

		''' <summary>
		''' Returns an enumeration of the zip file entries
		''' excluding internal JAR mechanism entries and including
		''' signed entries missing from the ZIP directory.
		''' </summary>
		Friend Overridable Function entries2() As Enumeration(Of JarEntry)
			ensureInitialization()
			If jv IsNot Nothing Then Return jv.entries2(Me, MyBase.entries())

			' screen out entries which are never signed
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim enum_ As Enumeration(Of ? As ZipEntry) = MyBase.entries()
			Return New EnumerationAnonymousInnerClassHelper2(Of E)
		End Function

		Private Class EnumerationAnonymousInnerClassHelper2(Of E)
			Implements Enumeration(Of E)

			Friend entry As ZipEntry

			Public Overridable Function hasMoreElements() As Boolean Implements Enumeration(Of E).hasMoreElements
				If entry IsNot Nothing Then Return True
				Do While enum_.hasMoreElements()
					Dim ze As ZipEntry = enum_.nextElement()
					If JarVerifier.isSigningRelated(ze.name) Then Continue Do
					entry = ze
					Return True
				Loop
				Return False
			End Function

			Public Overridable Function nextElement() As JarFileEntry
				If hasMoreElements() Then
					Dim ze As ZipEntry = entry
					entry = Nothing
					Return New JarFileEntry(ze)
				End If
				Throw New NoSuchElementException
			End Function
		End Class

		Friend Overridable Function getCodeSources(ByVal url As java.net.URL) As java.security.CodeSource()
			ensureInitialization()
			If jv IsNot Nothing Then Return jv.getCodeSources(Me, url)

	'        
	'         * JAR file has no signed content. Is there a non-signing
	'         * code source?
	'         
			Dim unsigned As Enumeration(Of String) = unsignedEntryNames()
			If unsigned.hasMoreElements() Then
				Return New java.security.CodeSource(){JarVerifier.getUnsignedCS(url)}
			Else
				Return Nothing
			End If
		End Function

		Private Function unsignedEntryNames() As Enumeration(Of String)
			Dim entries As Enumeration(Of JarEntry) = entries()
			Return New EnumerationAnonymousInnerClassHelper3(Of E)
		End Function

		Private Class EnumerationAnonymousInnerClassHelper3(Of E)
			Implements Enumeration(Of E)

			Friend name As String

		'            
		'             * Grab entries from ZIP directory but screen out
		'             * metadata.
		'             
			Public Overridable Function hasMoreElements() As Boolean Implements Enumeration(Of E).hasMoreElements
				If name IsNot Nothing Then Return True
				Do While entries.hasMoreElements()
					Dim value As String
					Dim e As ZipEntry = entries.nextElement()
					value = e.name
					If e.directory OrElse JarVerifier.isSigningRelated(value) Then Continue Do
					name = value
					Return True
				Loop
				Return False
			End Function

			Public Overridable Function nextElement() As String
				If hasMoreElements() Then
					Dim value As String = name
					name = Nothing
					Return value
				End If
				Throw New NoSuchElementException
			End Function
		End Class

		Friend Overridable Function getCodeSource(ByVal url As java.net.URL, ByVal name As String) As java.security.CodeSource
			ensureInitialization()
			If jv IsNot Nothing Then
				If jv.eagerValidation Then
					Dim cs As java.security.CodeSource = Nothing
					Dim je As JarEntry = getJarEntry(name)
					If je IsNot Nothing Then
						cs = jv.getCodeSource(url, Me, je)
					Else
						cs = jv.getCodeSource(url, name)
					End If
					Return cs
				Else
					Return jv.getCodeSource(url, name)
				End If
			End If

			Return JarVerifier.getUnsignedCS(url)
		End Function

		Friend Overridable Property eagerValidation As Boolean
			Set(ByVal eager As Boolean)
				Try
					maybeInstantiateVerifier()
				Catch e As IOException
					Throw New RuntimeException(e)
				End Try
				If jv IsNot Nothing Then jv.eagerValidation = eager
			End Set
		End Property

		Friend Overridable Property manifestDigests As List(Of Object)
			Get
				ensureInitialization()
				If jv IsNot Nothing Then Return jv.manifestDigests
				Return New List(Of Object)
			End Get
		End Property
	End Class

End Namespace