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

Namespace java.util.jar



	''' 
	''' <summary>
	''' @author      Roland Schemers
	''' </summary>
	Friend Class JarVerifier

		' Are we debugging ? 
		Friend Shared ReadOnly debug As sun.security.util.Debug = sun.security.util.Debug.getInstance("jar")

	'     a table mapping names to code signers, for jar entries that have
	'       had their actual hashes verified 
		Private verifiedSigners As Dictionary(Of String, CodeSigner())

	'     a table mapping names to code signers, for jar entries that have
	'       passed the .SF/.DSA/.EC -> MANIFEST check 
		Private sigFileSigners As Dictionary(Of String, CodeSigner())

		' a hash table to hold .SF bytes 
		Private sigFileData As Dictionary(Of String, SByte())

		''' <summary>
		''' "queue" of pending PKCS7 blocks that we couldn't parse
		'''  until we parsed the .SF file 
		''' </summary>
		Private pendingBlocks As List(Of sun.security.util.SignatureFileVerifier)

		' cache of CodeSigner objects 
		Private signerCache As List(Of CodeSigner())

		' Are we parsing a block? 
		Private parsingBlockOrSF As Boolean = False

		' Are we done parsing META-INF entries? 
		Private parsingMeta As Boolean = True

		' Are there are files to verify? 
		Private anyToVerify As Boolean = True

	'     The output stream to use when keeping track of files we are interested
	'       in 
		Private baos As ByteArrayOutputStream

		''' <summary>
		''' The ManifestDigester object </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private manDig As sun.security.util.ManifestDigester

		''' <summary>
		''' the bytes for the manDig object </summary>
		Friend manifestRawBytes As SByte() = Nothing

		''' <summary>
		''' controls eager signature validation </summary>
		Friend eagerValidation As Boolean

		''' <summary>
		''' makes code source singleton instances unique to us </summary>
		Private csdomain As New Object

		''' <summary>
		''' collect -DIGEST-MANIFEST values for blacklist </summary>
		Private manifestDigests As  ArrayList

		Public Sub New(  rawBytes As SByte())
			manifestRawBytes = rawBytes
			sigFileSigners = New Dictionary(Of )
			verifiedSigners = New Dictionary(Of )
			sigFileData = New Dictionary(Of )(11)
			pendingBlocks = New List(Of )
			baos = New ByteArrayOutputStream
			manifestDigests = New List(Of )
		End Sub

		''' <summary>
		''' This method scans to see which entry we're parsing and
		''' keeps various state information depending on what type of
		''' file is being parsed.
		''' </summary>
		Public Overridable Sub beginEntry(  je As JarEntry,   mev As sun.security.util.ManifestEntryVerifier)
			If je Is Nothing Then Return

			If debug IsNot Nothing Then debug.println("beginEntry " & je.name)

			Dim name As String = je.name

	'        
	'         * Assumptions:
	'         * 1. The manifest should be the first entry in the META-INF directory.
	'         * 2. The .SF/.DSA/.EC files follow the manifest, before any normal entries
	'         * 3. Any of the following will throw a SecurityException:
	'         *    a. digest mismatch between a manifest section and
	'         *       the SF section.
	'         *    b. digest mismatch between the actual jar entry and the manifest
	'         

			If parsingMeta Then
				Dim uname As String = name.ToUpper(Locale.ENGLISH)
				If (uname.StartsWith("META-INF/") OrElse uname.StartsWith("/META-INF/")) Then

					If je.directory Then
						mev.entrytry(Nothing, je)
						Return
					End If

					If uname.Equals(JarFile.MANIFEST_NAME) OrElse uname.Equals(sun.misc.JarIndex.INDEX_NAME) Then Return

					If sun.security.util.SignatureFileVerifier.isBlockOrSF(uname) Then
						' We parse only DSA, RSA or EC PKCS7 blocks. 
						parsingBlockOrSF = True
						baos.reset()
						mev.entrytry(Nothing, je)
						Return
					End If

					' If a META-INF entry is not MF or block or SF, they should
					' be normal entries. According to 2 above, no more block or
					' SF will appear. Let's doneWithMeta.
				End If
			End If

			If parsingMeta Then doneWithMeta()

			If je.directory Then
				mev.entrytry(Nothing, je)
				Return
			End If

			' be liberal in what you accept. If the name starts with ./, remove
			' it as we internally canonicalize it with out the ./.
			If name.StartsWith("./") Then name = name.Substring(2)

			' be liberal in what you accept. If the name starts with /, remove
			' it as we internally canonicalize it with out the /.
			If name.StartsWith("/") Then name = name.Substring(1)

			' only set the jev object for entries that have a signature
			' (either verified or not)
			If sigFileSigners.get(name) IsNot Nothing OrElse verifiedSigners.get(name) IsNot Nothing Then
				mev.entrytry(name, je)
				Return
			End If

			' don't compute the digest for this entry
			mev.entrytry(Nothing, je)

			Return
		End Sub

		''' <summary>
		''' update a single java.lang.[Byte].
		''' </summary>

		Public Overridable Sub update(  b As Integer,   mev As sun.security.util.ManifestEntryVerifier)
			If b <> -1 Then
				If parsingBlockOrSF Then
					baos.write(b)
				Else
					mev.update(CByte(b))
				End If
			Else
				processEntry(mev)
			End If
		End Sub

		''' <summary>
		''' update an array of bytes.
		''' </summary>

		Public Overridable Sub update(  n As Integer,   b As SByte(),   [off] As Integer,   len As Integer,   mev As sun.security.util.ManifestEntryVerifier)
			If n <> -1 Then
				If parsingBlockOrSF Then
					baos.write(b, [off], n)
				Else
					mev.update(b, [off], n)
				End If
			Else
				processEntry(mev)
			End If
		End Sub

		''' <summary>
		''' called when we reach the end of entry in one of the read() methods.
		''' </summary>
		Private Sub processEntry(  mev As sun.security.util.ManifestEntryVerifier)
			If Not parsingBlockOrSF Then
				Dim je As JarEntry = mev.entry
				If (je IsNot Nothing) AndAlso (je.signers Is Nothing) Then
					je.signers = mev.verify(verifiedSigners, sigFileSigners)
					je.certs = mapSignersToCertArray(je.signers)
				End If
			Else

				Try
					parsingBlockOrSF = False

					If debug IsNot Nothing Then debug.println("processEntry: processing block")

					Dim uname As String = mev.entry.name.ToUpper(Locale.ENGLISH)

					If uname.EndsWith(".SF") Then
						Dim key As String = uname.Substring(0, uname.length()-3)
						Dim bytes As SByte() = baos.toByteArray()
						' add to sigFileData in case future blocks need it
						sigFileData.put(key, bytes)
						' check pending blocks, we can now process
						' anyone waiting for this .SF file
						Dim it As [Iterator](Of sun.security.util.SignatureFileVerifier) = pendingBlocks.GetEnumerator()
						Do While it.MoveNext()
							Dim sfv As sun.security.util.SignatureFileVerifier = it.Current
							If sfv.needSignatureFile(key) Then
								If debug IsNot Nothing Then debug.println("processEntry: processing pending block")

								sfv.signatureFile = bytes
								sfv.process(sigFileSigners, manifestDigests)
							End If
						Loop
						Return
					End If

					' now we are parsing a signature block file

					Dim key As String = uname.Substring(0, uname.LastIndexOf("."))

					If signerCache Is Nothing Then signerCache = New List(Of )

					If manDig Is Nothing Then
						SyncLock manifestRawBytes
							If manDig Is Nothing Then
								manDig = New sun.security.util.ManifestDigester(manifestRawBytes)
								manifestRawBytes = Nothing
							End If
						End SyncLock
					End If

					Dim sfv As New sun.security.util.SignatureFileVerifier(signerCache, manDig, uname, baos.toByteArray())

					If sfv.needSignatureFileBytes() Then
						' see if we have already parsed an external .SF file
						Dim bytes As SByte() = sigFileData.get(key)

						If bytes Is Nothing Then
							' put this block on queue for later processing
							' since we don't have the .SF bytes yet
							' (uname, block);
							If debug IsNot Nothing Then debug.println("adding pending block")
							pendingBlocks.add(sfv)
							Return
						Else
							sfv.signatureFile = bytes
						End If
					End If
					sfv.process(sigFileSigners, manifestDigests)

				Catch ioe As IOException
					' e.g. sun.security.pkcs.ParsingException
					If debug IsNot Nothing Then debug.println("processEntry caught: " & ioe)
					' ignore and treat as unsigned
				Catch se As SignatureException
					If debug IsNot Nothing Then debug.println("processEntry caught: " & se)
					' ignore and treat as unsigned
				Catch nsae As NoSuchAlgorithmException
					If debug IsNot Nothing Then debug.println("processEntry caught: " & nsae)
					' ignore and treat as unsigned
				Catch ce As java.security.cert.CertificateException
					If debug IsNot Nothing Then debug.println("processEntry caught: " & ce)
					' ignore and treat as unsigned
				End Try
			End If
		End Sub

		''' <summary>
		''' Return an array of java.security.cert.Certificate objects for
		''' the given file in the jar.
		''' @deprecated
		''' </summary>
		<Obsolete> _
		Public Overridable Function getCerts(  name As String) As java.security.cert.Certificate()
			Return mapSignersToCertArray(getCodeSigners(name))
		End Function

		Public Overridable Function getCerts(  jar As JarFile,   entry As JarEntry) As java.security.cert.Certificate()
			Return mapSignersToCertArray(getCodeSigners(jar, entry))
		End Function

		''' <summary>
		''' return an array of CodeSigner objects for
		''' the given file in the jar. this array is not cloned.
		''' 
		''' </summary>
		Public Overridable Function getCodeSigners(  name As String) As CodeSigner()
			Return verifiedSigners.get(name)
		End Function

		Public Overridable Function getCodeSigners(  jar As JarFile,   entry As JarEntry) As CodeSigner()
			Dim name As String = entry.name
			If eagerValidation AndAlso sigFileSigners.get(name) IsNot Nothing Then
	'            
	'             * Force a read of the entry data to generate the
	'             * verification hash.
	'             
				Try
					Dim s As InputStream = jar.getInputStream(entry)
					Dim buffer As SByte() = New SByte(1023){}
					Dim n As Integer = buffer.Length
					Do While n <> -1
						n = s.read(buffer, 0, buffer.Length)
					Loop
					s.close()
				Catch e As IOException
				End Try
			End If
			Return getCodeSigners(name)
		End Function

	'    
	'     * Convert an array of signers into an array of concatenated certificate
	'     * arrays.
	'     
		Private Shared Function mapSignersToCertArray(  signers As CodeSigner()) As java.security.cert.Certificate()

			If signers IsNot Nothing Then
				Dim certChains As New List(Of java.security.cert.Certificate)
				For i As Integer = 0 To signers.Length - 1
					certChains.addAll(signers(i).signerCertPath.certificates)
				Next i

				' Convert into a Certificate[]
				Return certChains.ToArray(New java.security.cert.Certificate(certChains.size() - 1){})
			End If
			Return Nothing
		End Function

		''' <summary>
		''' returns true if there no files to verify.
		''' should only be called after all the META-INF entries
		''' have been processed.
		''' </summary>
		Friend Overridable Function nothingToVerify() As Boolean
			Return (anyToVerify = False)
		End Function

		''' <summary>
		''' called to let us know we have processed all the
		''' META-INF entries, and if we re-read one of them, don't
		''' re-process it. Also gets rid of any data structures
		''' we needed when parsing META-INF entries.
		''' </summary>
		Friend Overridable Sub doneWithMeta()
			parsingMeta = False
			anyToVerify = Not sigFileSigners.empty
			baos = Nothing
			sigFileData = Nothing
			pendingBlocks = Nothing
			signerCache = Nothing
			manDig = Nothing
			' MANIFEST.MF is always treated as signed and verified,
			' move its signers from sigFileSigners to verifiedSigners.
			If sigFileSigners.containsKey(JarFile.MANIFEST_NAME) Then
				Dim codeSigners_Renamed As CodeSigner() = sigFileSigners.remove(JarFile.MANIFEST_NAME)
				verifiedSigners.put(JarFile.MANIFEST_NAME, codeSigners_Renamed)
			End If
		End Sub

		Friend Class VerifierStream
			Inherits java.io.InputStream

			Private [is] As InputStream
			Private jv As JarVerifier
			Private mev As sun.security.util.ManifestEntryVerifier
			Private numLeft As Long

			Friend Sub New(  man As Manifest,   je As JarEntry,   [is] As InputStream,   jv As JarVerifier)
				Me.is = [is]
				Me.jv = jv
				Me.mev = New sun.security.util.ManifestEntryVerifier(man)
				Me.jv.beginEntry(je, mev)
				Me.numLeft = je.size
				If Me.numLeft = 0 Then Me.jv.update(-1, Me.mev)
			End Sub

			Public Overrides Function read() As Integer
				If numLeft > 0 Then
					Dim b As Integer = [is].read()
					jv.update(b, mev)
					numLeft -= 1
					If numLeft = 0 Then jv.update(-1, mev)
					Return b
				Else
					Return -1
				End If
			End Function

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			public int read(byte b() , int off, int len) throws IOException
				If (numLeft > 0) AndAlso (numLeft < len) Then len = CInt(numLeft)

				If numLeft > 0 Then
					Dim n As Integer = [is].read(b, off, len)
					jv.update(n, b, off, len, mev)
					numLeft -= n
					If numLeft = 0 Then jv.update(-1, b, off, len, mev)
					Return n
				Else
					Return -1
				End If

			public  Sub  close() throws IOException
				If [is] IsNot Nothing Then [is].close()
				[is] = Nothing
				mev = Nothing
				jv = Nothing

			public Integer available() throws IOException
				Return [is].available()

		End Class

		' Extended JavaUtilJarAccess CodeSource API Support

		private Map(Of java.net.URL, Map(Of CodeSigner(), CodeSource)) urlToCodeSourceMap = New HashMap(Of )
		private Map(Of CodeSigner(), CodeSource) signerToCodeSource = New HashMap(Of )
		private java.net.URL lastURL
		private Map(Of CodeSigner(), CodeSource) lastURLMap

	'    
	'     * Create a unique mapping from codeSigner cache entries to CodeSource.
	'     * In theory, multiple URLs origins could map to a single locally cached
	'     * and shared JAR file although in practice there will be a single URL in use.
	'     
		private synchronized CodeSource mapSignersToCodeSource(java.net.URL url, CodeSigner() signers)
			Dim map As Map(Of CodeSigner(), CodeSource)
			If url Is lastURL Then
				map = lastURLMap
			Else
				map = urlToCodeSourceMap.get(url)
				If map Is Nothing Then
					map = New HashMap(Of )
					urlToCodeSourceMap.put(url, map)
				End If
				lastURLMap = map
				lastURL = url
			End If
			Dim cs As CodeSource = map.get(signers)
			If cs Is Nothing Then
				cs = New VerifierCodeSource(csdomain, url, signers)
				signerToCodeSource.put(signers, cs)
			End If
			Return cs

		private CodeSource() mapSignersToCodeSources(java.net.URL url, List(Of CodeSigner()) signers, Boolean unsigned)
			Dim sources As List(Of CodeSource) = New List(Of CodeSource)

			For i As Integer = 0 To signers.size() - 1
				sources.add(mapSignersToCodeSource(url, signers.get(i)))
			Next i
			If unsigned Then sources.add(mapSignersToCodeSource(url, Nothing))
			Return sources.ToArray(New CodeSource(sources.size() - 1){})
		private CodeSigner() emptySigner = New CodeSigner(){}

	'    
	'     * Match CodeSource to a CodeSigner[] in the signer cache.
	'     
		private CodeSigner() findMatchingSigners(CodeSource cs)
			If TypeOf cs Is VerifierCodeSource Then
				Dim vcs As VerifierCodeSource = CType(cs, VerifierCodeSource)
				If vcs.isSameDomain(csdomain) Then Return CType(cs, VerifierCodeSource).privateSigners
			End If

	'        
	'         * In practice signers should always be optimized above
	'         * but this handles a CodeSource of any type, just in case.
	'         
			Dim sources As CodeSource() = mapSignersToCodeSources(cs.location, jarCodeSigners, True)
			Dim sourceList As List(Of CodeSource) = New List(Of CodeSource)
			For i As Integer = 0 To sources.Length - 1
				sourceList.add(sources(i))
			Next i
			Dim j As Integer = sourceList.IndexOf(cs)
			If j <> -1 Then
				Dim match As CodeSigner()
				match = CType(sourceList.get(j), VerifierCodeSource).privateSigners
				If match Is Nothing Then match = emptySigner
				Return match
			End If
			Return Nothing

	'    
	'     * Instances of this class hold uncopied references to internal
	'     * signing data that can be compared by object reference identity.
	'     
		private static class VerifierCodeSource extends CodeSource
			private static final Long serialVersionUID = -9047366145967768825L

			Dim vlocation As java.net.URL
			Dim vsigners As CodeSigner()
			Dim vcerts As java.security.cert.Certificate()
			Dim csdomain As Object

			VerifierCodeSource(Object csdomain, java.net.URL location, CodeSigner() signers)
				MyBase(location, signers)
				Me.csdomain = csdomain
				vlocation = location
				vsigners = signers ' from signerCache

			VerifierCodeSource(Object csdomain, java.net.URL location, java.security.cert.Certificate() certs)
				MyBase(location, certs)
				Me.csdomain = csdomain
				vlocation = location
				vcerts = certs ' from signerCache

	'        
	'         * All VerifierCodeSource instances are constructed based on
	'         * singleton signerCache or signerCacheCert entries for each unique signer.
	'         * No CodeSigner<->Certificate[] conversion is required.
	'         * We use these assumptions to optimize equality comparisons.
	'         
			public Boolean Equals(Object obj)
				If obj Is Me Then Return True
				If TypeOf obj Is VerifierCodeSource Then
					Dim that As VerifierCodeSource = CType(obj, VerifierCodeSource)

	'                
	'                 * Only compare against other per-signer singletons constructed
	'                 * on behalf of the same JarFile instance. Otherwise, compare
	'                 * things the slower way.
	'                 
					If isSameDomain(that.csdomain) Then
						If that.vsigners <> Me.vsigners OrElse that.vcerts <> Me.vcerts Then Return False
						If that.vlocation IsNot Nothing Then
							Return that.vlocation.Equals(Me.vlocation)
						ElseIf Me.vlocation IsNot Nothing Then
							Return Me.vlocation.Equals(that.vlocation) ' both null
						Else
							Return True
						End If
					End If
				End If
				Return MyBase.Equals(obj)

			Boolean isSameDomain(Object csdomain)
				Return Me.csdomain Is csdomain

			private CodeSigner() privateSigners
				Return vsigners

			private java.security.cert.Certificate() privateCertificates
				Return vcerts
		private Map(Of String, CodeSigner()) signerMap_Renamed

		private synchronized Map(Of String, CodeSigner()) signerMap()
			If signerMap_Renamed Is Nothing Then
	'            
	'             * Snapshot signer state so it doesn't change on us. We care
	'             * only about the asserted signatures. Verification of
	'             * signature validity happens via the JarEntry apis.
	'             
				signerMap_Renamed = New HashMap(Of )(verifiedSigners.size() + sigFileSigners.size())
				signerMap_Renamed.putAll(verifiedSigners)
				signerMap_Renamed.putAll(sigFileSigners)
			End If
			Return signerMap_Renamed

		Public  Enumeration(Of String) entryNames(JarFile jar, final CodeSource() cs)
			Dim map As Map(Of String, CodeSigner()) = signerMap()
			Dim itor As [Iterator](Of KeyValuePair(Of String, CodeSigner())) = map.entrySet().GetEnumerator()
			Dim matchUnsigned As Boolean = False

	'        
	'         * Grab a single copy of the CodeSigner arrays. Check
	'         * to see if we can optimize CodeSigner equality test.
	'         
			Dim req As List(Of CodeSigner()) = New List(Of CodeSigner())(cs.length)
			For i As Integer = 0 To cs.length - 1
				Dim match As CodeSigner() = findMatchingSigners(cs(i))
				If match IsNot Nothing Then
					If match.Length > 0 Then
						req.add(match)
					Else
						matchUnsigned = True
					End If
				Else
					matchUnsigned = True
				End If
			Next i

			Dim signersReq As List(Of CodeSigner()) = req
			Dim enum2 As Enumeration(Of String) = If(matchUnsigned, unsignedEntryNames(jar), emptyEnumeration)

			Return New EnumerationAnonymousInnerClassHelper(Of E)

	'    
	'     * Like entries() but screens out internal JAR mechanism entries
	'     * and includes signed entries with no ZIP data.
	'     
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		public Enumeration(Of JarEntry) entries2(final JarFile jar, Enumeration(Of ? As java.util.zip.ZipEntry) e)
			Dim map As Map(Of String, CodeSigner()) = New HashMap(Of String, CodeSigner())
			map.putAll(signerMap())
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim enum_ As Enumeration(Of ? As java.util.zip.ZipEntry) = e
			Return New EnumerationAnonymousInnerClassHelper2(Of E)
		private Enumeration(Of String) emptyEnumeration = New EnumerationAnonymousInnerClassHelper3(Of E)

		' true if file is part of the signature mechanism itself
		static Boolean isSigningRelated(String name)
			Return sun.security.util.SignatureFileVerifier.isSigningRelated(name)

		private Enumeration(Of String) unsignedEntryNames(JarFile jar)
			Dim map As Map(Of String, CodeSigner()) = signerMap()
			Dim entries As Enumeration(Of JarEntry) = jar.entries()
			Return New EnumerationAnonymousInnerClassHelper4(Of E)
		private List(Of CodeSigner()) jarCodeSigners

		private synchronized List(Of CodeSigner()) jarCodeSigners
			Dim signers As CodeSigner()
			If jarCodeSigners Is Nothing Then
				Dim [set] As New HashSet(Of CodeSigner())
				[set].addAll(signerMap().values())
				jarCodeSigners = New List(Of )
				jarCodeSigners.addAll([set])
			End If
			Return jarCodeSigners

		Public  CodeSource() getCodeSources(JarFile jar, java.net.URL url)
			Dim hasUnsigned As Boolean = unsignedEntryNames(jar).hasMoreElements()

			Return mapSignersToCodeSources(url, jarCodeSigners, hasUnsigned)

		public CodeSource getCodeSource(java.net.URL url, String name)
			Dim signers As CodeSigner()

			signers = signerMap().get(name)
			Return mapSignersToCodeSource(url, signers)

		public CodeSource getCodeSource(java.net.URL url, JarFile jar, JarEntry je)
			Dim signers As CodeSigner()

			Return mapSignersToCodeSource(url, getCodeSigners(jar, je))

		public  Sub  eagerValidationion(Boolean eager)
			eagerValidation = eager

		Public   ArrayList manifestDigests
			Return Collections.unmodifiableList(manifestDigests)

		static CodeSource getUnsignedCS(java.net.URL url)
			Return New VerifierCodeSource(Nothing, url, CType(Nothing, java.security.cert.Certificate()))
	End Class


	Private Class EnumerationAnonymousInnerClassHelper(Of E)
		Implements Enumeration(Of E)

		Friend name As String

		Public Overridable Function hasMoreElements() As Boolean Implements Enumeration(Of E).hasMoreElements
			If name IsNot Nothing Then Return True

			Do While itor.hasNext()
				Dim e As KeyValuePair(Of String, CodeSigner()) = itor.next()
				If signersReq.contains(e.Value) Then
					name = e.Key
					Return True
				End If
			Loop
			Do While enum2.hasMoreElements()
				name = enum2.nextElement()
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

	Private Class EnumerationAnonymousInnerClassHelper2(Of E)
		Implements Enumeration(Of E)

		Friend signers As Enumeration(Of String) = Nothing
		Friend entry As JarEntry

		Public Overridable Function hasMoreElements() As Boolean Implements Enumeration(Of E).hasMoreElements
			If entry IsNot Nothing Then Return True
			Do While enum_.hasMoreElements()
				Dim ze As java.util.zip.ZipEntry = enum_.nextElement()
				If JarVerifier.isSigningRelated(ze.name) Then Continue Do
				entry = jar.newEntry(ze)
				Return True
			Loop
			If signers Is Nothing Then signers = Collections.enumeration(map.Keys)
			Do While signers.hasMoreElements()
				Dim name As String = signers.nextElement()
				entry = jar.newEntry(New java.util.zip.ZipEntry(name))
				Return True
			Loop

			' Any map entries left?
			Return False
		End Function

		Public Overridable Function nextElement() As JarEntry
			If hasMoreElements() Then
				Dim je As JarEntry = entry
				map.remove(je.name)
				entry = Nothing
				Return je
			End If
			Throw New NoSuchElementException
		End Function
	End Class

	Private Class EnumerationAnonymousInnerClassHelper3(Of E)
		Implements Enumeration(Of E)

		Public Overridable Function hasMoreElements() As Boolean Implements Enumeration(Of E).hasMoreElements
			Return False
		End Function

		Public Overridable Function nextElement() As String
			Throw New NoSuchElementException
		End Function
	End Class

	Private Class EnumerationAnonymousInnerClassHelper4(Of E)
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
				Dim e As java.util.zip.ZipEntry = entries.nextElement()
				value = e.name
				If e.directory OrElse isSigningRelated(value) Then Continue Do
				If map.get(value) Is Nothing Then
					name = value
					Return True
				End If
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
End Namespace