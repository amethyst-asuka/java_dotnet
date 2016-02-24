Imports Microsoft.VisualBasic
Imports System.Collections.Generic
Imports System.Runtime.InteropServices

'
' * Copyright (c) 1995, 2015, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.util.zip



	''' <summary>
	''' This class is used to read entries from a zip file.
	''' 
	''' <p> Unless otherwise noted, passing a <tt>null</tt> argument to a constructor
	''' or method in this class will cause a <seealso cref="NullPointerException"/> to be
	''' thrown.
	''' 
	''' @author      David Connelly
	''' </summary>
	Public Class ZipFile
		Implements ZipConstants, java.io.Closeable

		Private jzfile As Long ' address of jzfile data
		Private ReadOnly name As String ' zip file name
		Private ReadOnly total As Integer ' total number of entries
		Private ReadOnly locsig As Boolean ' if zip file starts with LOCSIG (usually true)
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private closeRequested As Boolean = False

		Private Const STORED As Integer = ZipEntry.STORED
		Private Const DEFLATED As Integer = ZipEntry.DEFLATED

		''' <summary>
		''' Mode flag to open a zip file for reading.
		''' </summary>
		Public Const OPEN_READ As Integer = &H1

		''' <summary>
		''' Mode flag to open a zip file and mark it for deletion.  The file will be
		''' deleted some time between the moment that it is opened and the moment
		''' that it is closed, but its contents will remain accessible via the
		''' <tt>ZipFile</tt> object until either the close method is invoked or the
		''' virtual machine exits.
		''' </summary>
		Public Const OPEN_DELETE As Integer = &H4

		Shared Sub New()
			' Zip library is loaded from System.initializeSystemClass 
			initIDs()
			' A system prpperty to disable mmap use to avoid vm crash when
			' in-use zip file is accidently overwritten by others.
			Dim prop As String = sun.misc.VM.getSavedProperty("sun.zip.disableMemoryMapping")
			usemmap = (prop Is Nothing OrElse Not(prop.length() = 0 OrElse prop.equalsIgnoreCase("true")))
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			sun.misc.SharedSecrets.setJavaUtilZipFileAccess(New sun.misc.JavaUtilZipFileAccess()
	'		{
	'				public boolean startsWithLocHeader(ZipFile zip)
	'				{
	'					Return zip.startsWithLocHeader();
	'				}
	'			 }
		   )
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub initIDs()
		End Sub

		Private Shared ReadOnly usemmap As Boolean


		''' <summary>
		''' Opens a zip file for reading.
		''' 
		''' <p>First, if there is a security manager, its <code>checkRead</code>
		''' method is called with the <code>name</code> argument as its argument
		''' to ensure the read is allowed.
		''' 
		''' <p>The UTF-8 <seealso cref="java.nio.charset.Charset charset"/> is used to
		''' decode the entry names and comments.
		''' </summary>
		''' <param name="name"> the name of the zip file </param>
		''' <exception cref="ZipException"> if a ZIP format error has occurred </exception>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		''' <exception cref="SecurityException"> if a security manager exists and its
		'''         <code>checkRead</code> method doesn't allow read access to the file.
		''' </exception>
		''' <seealso cref= SecurityManager#checkRead(java.lang.String) </seealso>
		Public Sub New(ByVal name As String)
			Me.New(New File(name), OPEN_READ)
		End Sub

		''' <summary>
		''' Opens a new <code>ZipFile</code> to read from the specified
		''' <code>File</code> object in the specified mode.  The mode argument
		''' must be either <tt>OPEN_READ</tt> or <tt>OPEN_READ | OPEN_DELETE</tt>.
		''' 
		''' <p>First, if there is a security manager, its <code>checkRead</code>
		''' method is called with the <code>name</code> argument as its argument to
		''' ensure the read is allowed.
		''' 
		''' <p>The UTF-8 <seealso cref="java.nio.charset.Charset charset"/> is used to
		''' decode the entry names and comments
		''' </summary>
		''' <param name="file"> the ZIP file to be opened for reading </param>
		''' <param name="mode"> the mode in which the file is to be opened </param>
		''' <exception cref="ZipException"> if a ZIP format error has occurred </exception>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		''' <exception cref="SecurityException"> if a security manager exists and
		'''         its <code>checkRead</code> method
		'''         doesn't allow read access to the file,
		'''         or its <code>checkDelete</code> method doesn't allow deleting
		'''         the file when the <tt>OPEN_DELETE</tt> flag is set. </exception>
		''' <exception cref="IllegalArgumentException"> if the <tt>mode</tt> argument is invalid </exception>
		''' <seealso cref= SecurityManager#checkRead(java.lang.String)
		''' @since 1.3 </seealso>
		Public Sub New(ByVal file As java.io.File, ByVal mode As Integer)
			Me.New(file, mode, java.nio.charset.StandardCharsets.UTF_8)
		End Sub

		''' <summary>
		''' Opens a ZIP file for reading given the specified File object.
		''' 
		''' <p>The UTF-8 <seealso cref="java.nio.charset.Charset charset"/> is used to
		''' decode the entry names and comments.
		''' </summary>
		''' <param name="file"> the ZIP file to be opened for reading </param>
		''' <exception cref="ZipException"> if a ZIP format error has occurred </exception>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		Public Sub New(ByVal file As java.io.File)
			Me.New(file, OPEN_READ)
		End Sub

		Private zc As ZipCoder

		''' <summary>
		''' Opens a new <code>ZipFile</code> to read from the specified
		''' <code>File</code> object in the specified mode.  The mode argument
		''' must be either <tt>OPEN_READ</tt> or <tt>OPEN_READ | OPEN_DELETE</tt>.
		''' 
		''' <p>First, if there is a security manager, its <code>checkRead</code>
		''' method is called with the <code>name</code> argument as its argument to
		''' ensure the read is allowed.
		''' </summary>
		''' <param name="file"> the ZIP file to be opened for reading </param>
		''' <param name="mode"> the mode in which the file is to be opened </param>
		''' <param name="charset">
		'''        the <seealso cref="java.nio.charset.Charset charset"/> to
		'''        be used to decode the ZIP entry name and comment that are not
		'''        encoded by using UTF-8 encoding (indicated by entry's general
		'''        purpose flag).
		''' </param>
		''' <exception cref="ZipException"> if a ZIP format error has occurred </exception>
		''' <exception cref="IOException"> if an I/O error has occurred
		''' </exception>
		''' <exception cref="SecurityException">
		'''         if a security manager exists and its <code>checkRead</code>
		'''         method doesn't allow read access to the file,or its
		'''         <code>checkDelete</code> method doesn't allow deleting the
		'''         file when the <tt>OPEN_DELETE</tt> flag is set
		''' </exception>
		''' <exception cref="IllegalArgumentException"> if the <tt>mode</tt> argument is invalid
		''' </exception>
		''' <seealso cref= SecurityManager#checkRead(java.lang.String)
		''' 
		''' @since 1.7 </seealso>
		Public Sub New(ByVal file As java.io.File, ByVal mode As Integer, ByVal charset As java.nio.charset.Charset)
			If ((mode And OPEN_READ) = 0) OrElse ((mode And Not(OPEN_READ Or OPEN_DELETE)) <> 0) Then Throw New IllegalArgumentException("Illegal mode: 0x" & Integer.toHexString(mode))
			Dim name_Renamed As String = file.path
			Dim sm As SecurityManager = System.securityManager
			If sm IsNot Nothing Then
				sm.checkRead(name_Renamed)
				If (mode And OPEN_DELETE) <> 0 Then sm.checkDelete(name_Renamed)
			End If
			If charset Is Nothing Then Throw New NullPointerException("charset is null")
			Me.zc = ZipCoder.get(charset)
			Dim t0 As Long = System.nanoTime()
			jzfile = open(name_Renamed, mode, file.lastModified(), usemmap)
			sun.misc.PerfCounter.zipFileOpenTime.addElapsedTimeFrom(t0)
			sun.misc.PerfCounter.zipFileCount.increment()
			Me.name = name_Renamed
			Me.total = getTotal(jzfile)
			Me.locsig = startsWithLOC(jzfile)
		End Sub

		''' <summary>
		''' Opens a zip file for reading.
		''' 
		''' <p>First, if there is a security manager, its <code>checkRead</code>
		''' method is called with the <code>name</code> argument as its argument
		''' to ensure the read is allowed.
		''' </summary>
		''' <param name="name"> the name of the zip file </param>
		''' <param name="charset">
		'''        the <seealso cref="java.nio.charset.Charset charset"/> to
		'''        be used to decode the ZIP entry name and comment that are not
		'''        encoded by using UTF-8 encoding (indicated by entry's general
		'''        purpose flag).
		''' </param>
		''' <exception cref="ZipException"> if a ZIP format error has occurred </exception>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		''' <exception cref="SecurityException">
		'''         if a security manager exists and its <code>checkRead</code>
		'''         method doesn't allow read access to the file
		''' </exception>
		''' <seealso cref= SecurityManager#checkRead(java.lang.String)
		''' 
		''' @since 1.7 </seealso>
		Public Sub New(ByVal name As String, ByVal charset As java.nio.charset.Charset)
			Me.New(New File(name), OPEN_READ, charset)
		End Sub

		''' <summary>
		''' Opens a ZIP file for reading given the specified File object. </summary>
		''' <param name="file"> the ZIP file to be opened for reading </param>
		''' <param name="charset">
		'''        The <seealso cref="java.nio.charset.Charset charset"/> to be
		'''        used to decode the ZIP entry name and comment (ignored if
		'''        the <a href="package-summary.html#lang_encoding"> language
		'''        encoding bit</a> of the ZIP entry's general purpose bit
		'''        flag is set).
		''' </param>
		''' <exception cref="ZipException"> if a ZIP format error has occurred </exception>
		''' <exception cref="IOException"> if an I/O error has occurred
		''' 
		''' @since 1.7 </exception>
		Public Sub New(ByVal file As java.io.File, ByVal charset As java.nio.charset.Charset)
			Me.New(file, OPEN_READ, charset)
		End Sub

		''' <summary>
		''' Returns the zip file comment, or null if none.
		''' </summary>
		''' <returns> the comment string for the zip file, or null if none
		''' </returns>
		''' <exception cref="IllegalStateException"> if the zip file has been closed
		''' 
		''' Since 1.7 </exception>
		Public Overridable Property comment As String
			Get
				SyncLock Me
					ensureOpen()
					Dim bcomm As SByte() = getCommentBytes(jzfile)
					If bcomm Is Nothing Then Return Nothing
					Return zc.ToString(bcomm, bcomm.Length)
				End SyncLock
			End Get
		End Property

		''' <summary>
		''' Returns the zip file entry for the specified name, or null
		''' if not found.
		''' </summary>
		''' <param name="name"> the name of the entry </param>
		''' <returns> the zip file entry, or null if not found </returns>
		''' <exception cref="IllegalStateException"> if the zip file has been closed </exception>
		Public Overridable Function getEntry(ByVal name As String) As ZipEntry
			If name Is Nothing Then Throw New NullPointerException("name")
			Dim jzentry As Long = 0
			SyncLock Me
				ensureOpen()
				jzentry = getEntry(jzfile, zc.getBytes(name), True)
				If jzentry <> 0 Then
					Dim ze As ZipEntry = getZipEntry(name, jzentry)
					freeEntry(jzfile, jzentry)
					Return ze
				End If
			End SyncLock
			Return Nothing
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function getEntry(ByVal jzfile As Long, ByVal name As SByte(), ByVal addSlash As Boolean) As Long
		End Function

		' freeEntry releases the C jzentry struct.
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub freeEntry(ByVal jzfile As Long, ByVal jzentry As Long)
		End Sub

		' the outstanding inputstreams that need to be closed,
		' mapped to the inflater objects they use.
		Private ReadOnly streams As IDictionary(Of java.io.InputStream, Inflater) = New java.util.WeakHashMap(Of java.io.InputStream, Inflater)

		''' <summary>
		''' Returns an input stream for reading the contents of the specified
		''' zip file entry.
		''' 
		''' <p> Closing this ZIP file will, in turn, close all input
		''' streams that have been returned by invocations of this method.
		''' </summary>
		''' <param name="entry"> the zip file entry </param>
		''' <returns> the input stream for reading the contents of the specified
		''' zip file entry. </returns>
		''' <exception cref="ZipException"> if a ZIP format error has occurred </exception>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		''' <exception cref="IllegalStateException"> if the zip file has been closed </exception>
		Public Overridable Function getInputStream(ByVal entry As ZipEntry) As java.io.InputStream
			If entry Is Nothing Then Throw New NullPointerException("entry")
			Dim jzentry As Long = 0
			Dim [in] As ZipFileInputStream = Nothing
			SyncLock Me
				ensureOpen()
				If (Not zc.uTF8) AndAlso (entry.flag And EFS) <> 0 Then
					jzentry = getEntry(jzfile, zc.getBytesUTF8(entry.name), False)
				Else
					jzentry = getEntry(jzfile, zc.getBytes(entry.name), False)
				End If
				If jzentry = 0 Then Return Nothing
				[in] = New ZipFileInputStream(Me, jzentry)

				Select Case getEntryMethod(jzentry)
				Case STORED
					SyncLock streams
						streams([in]) = Nothing
					End SyncLock
					Return [in]
				Case DEFLATED
					' MORE: Compute good size for inflater stream:
					Dim size As Long = getEntrySize(jzentry) + 2 ' Inflater likes a bit of slack
					If size > 65536 Then size = 8192
					If size <= 0 Then size = 4096
					Dim inf As Inflater = inflater
					Dim [is] As java.io.InputStream = New ZipFileInflaterInputStream(Me, [in], inf, CInt(size))
					SyncLock streams
						streams([is]) = inf
					End SyncLock
					Return [is]
				Case Else
					Throw New ZipException("invalid compression method")
				End Select
			End SyncLock
		End Function

		Private Class ZipFileInflaterInputStream
			Inherits InflaterInputStream

			Private ReadOnly outerInstance As ZipFile

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Private closeRequested As Boolean = False
			Private eof As Boolean = False
			Private ReadOnly zfin As ZipFileInputStream

			Friend Sub New(ByVal outerInstance As ZipFile, ByVal zfin As ZipFileInputStream, ByVal inf As Inflater, ByVal size As Integer)
					Me.outerInstance = outerInstance
				MyBase.New(zfin, inf, size)
				Me.zfin = zfin
			End Sub

			Public Overrides Sub close()
				If closeRequested Then Return
				closeRequested = True

				MyBase.close()
				Dim inf As Inflater
				SyncLock outerInstance.streams
					inf = outerInstance.streams.Remove(Me)
				End SyncLock
				If inf IsNot Nothing Then outerInstance.releaseInflater(inf)
			End Sub

			' Override fill() method to provide an extra "dummy" byte
			' at the end of the input stream. This is required when
			' using the "nowrap" Inflater option.
			Protected Friend Overrides Sub fill()
				If eof Then Throw New java.io.EOFException("Unexpected end of ZLIB input stream")
				len = [in].read(buf, 0, buf.Length)
				If len = -1 Then
					buf(0) = 0
					len = 1
					eof = True
				End If
				inf.inputput(buf, 0, len)
			End Sub

			Public Overrides Function available() As Integer
				If closeRequested Then Return 0
				Dim avail As Long = zfin.size() - inf.bytesWritten
				Return (If(avail > (Long) Integer.MaxValue, Integer.MaxValue, CInt(avail)))
			End Function

			Protected Overrides Sub Finalize()
				close()
			End Sub
		End Class

	'    
	'     * Gets an inflater from the list of available inflaters or allocates
	'     * a new one.
	'     
		Private Property inflater As Inflater
			Get
				Dim inf As Inflater
				SyncLock inflaterCache
					inf = inflaterCache.poll()
					Do While Nothing IsNot inf
						If False = inf.ended() Then Return inf
						inf = inflaterCache.poll()
					Loop
				End SyncLock
				Return New Inflater(True)
			End Get
		End Property

	'    
	'     * Releases the specified inflater to the list of available inflaters.
	'     
		Private Sub releaseInflater(ByVal inf As Inflater)
			If False = inf.ended() Then
				inf.reset()
				SyncLock inflaterCache
					inflaterCache.add(inf)
				End SyncLock
			End If
		End Sub

		' List of available Inflater objects for decompression
		Private inflaterCache As java.util.Deque(Of Inflater) = New java.util.ArrayDeque(Of Inflater)

		''' <summary>
		''' Returns the path name of the ZIP file. </summary>
		''' <returns> the path name of the ZIP file </returns>
		Public Overridable Property name As String
			Get
				Return name
			End Get
		End Property

		Private Class ZipEntryIterator
			Implements System.Collections.IEnumerator(Of ZipEntry), IEnumerator(Of ZipEntry)

			Private ReadOnly outerInstance As ZipFile

			Private i As Integer = 0

			Public Sub New(ByVal outerInstance As ZipFile)
					Me.outerInstance = outerInstance
				outerInstance.ensureOpen()
			End Sub

			Public Overridable Function hasMoreElements() As Boolean
				Return hasNext()
			End Function

			Public Overridable Function hasNext() As Boolean
				SyncLock ZipFile.this
					outerInstance.ensureOpen()
					Return i < outerInstance.total
				End SyncLock
			End Function

			Public Overridable Function nextElement() As ZipEntry
				Return [next]()
			End Function

			Public Overridable Function [next]() As ZipEntry
				SyncLock ZipFile.this
					outerInstance.ensureOpen()
					If i >= outerInstance.total Then Throw New java.util.NoSuchElementException
					Dim jzentry As Long = getNextEntry(outerInstance.jzfile, i)
					i += 1
					If jzentry = 0 Then
						Dim message As String
						If outerInstance.closeRequested Then
							message = "ZipFile concurrently closed"
						Else
							message = getZipMessage(outerInstance.jzfile)
						End If
						Throw New ZipError("jzentry == 0" & "," & vbLf & " jzfile = " & outerInstance.jzfile & "," & vbLf & " total = " & outerInstance.total & "," & vbLf & " name = " & outerInstance.name & "," & vbLf & " i = " & i & "," & vbLf & " message = " & message)
					End If
					Dim ze As ZipEntry = outerInstance.getZipEntry(Nothing, jzentry)
					freeEntry(outerInstance.jzfile, jzentry)
					Return ze
				End SyncLock
			End Function
		End Class

		''' <summary>
		''' Returns an enumeration of the ZIP file entries. </summary>
		''' <returns> an enumeration of the ZIP file entries </returns>
		''' <exception cref="IllegalStateException"> if the zip file has been closed </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Function entries() As System.Collections.IEnumerator(Of ? As ZipEntry)
			Return New ZipEntryIterator(Me)
		End Function

		''' <summary>
		''' Return an ordered {@code Stream} over the ZIP file entries.
		''' Entries appear in the {@code Stream} in the order they appear in
		''' the central directory of the ZIP file.
		''' </summary>
		''' <returns> an ordered {@code Stream} of entries in this ZIP file </returns>
		''' <exception cref="IllegalStateException"> if the zip file has been closed
		''' @since 1.8 </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Function stream() As java.util.stream.Stream(Of ? As ZipEntry)
			Return java.util.stream.StreamSupport.stream(java.util.Spliterators.spliterator(New ZipEntryIterator(Me), size(), java.util.Spliterator.ORDERED Or java.util.Spliterator.DISTINCT Or java.util.Spliterator.IMMUTABLE Or java.util.Spliterator.NONNULL), False)
		End Function

		Private Function getZipEntry(ByVal name As String, ByVal jzentry As Long) As ZipEntry
			Dim e As New ZipEntry
			e.flag = getEntryFlag(jzentry) ' get the flag first
			If name IsNot Nothing Then
				e.name = name
			Else
				Dim bname As SByte() = getEntryBytes(jzentry, JZENTRY_NAME)
				If (Not zc.uTF8) AndAlso (e.flag And EFS) <> 0 Then
					e.name = zc.toStringUTF8(bname, bname.Length)
				Else
					e.name = zc.ToString(bname, bname.Length)
				End If
			End If
			e.xdostime = getEntryTime(jzentry)
			e.crc = getEntryCrc(jzentry)
			e.size = getEntrySize(jzentry)
			e.csize = getEntryCSize(jzentry)
			e.method = getEntryMethod(jzentry)
			e.extra0ra0(getEntryBytes(jzentry, JZENTRY_EXTRA), False)
			Dim bcomm As SByte() = getEntryBytes(jzentry, JZENTRY_COMMENT)
			If bcomm Is Nothing Then
				e.comment = Nothing
			Else
				If (Not zc.uTF8) AndAlso (e.flag And EFS) <> 0 Then
					e.comment = zc.toStringUTF8(bcomm, bcomm.Length)
				Else
					e.comment = zc.ToString(bcomm, bcomm.Length)
				End If
			End If
			Return e
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function getNextEntry(ByVal jzfile As Long, ByVal i As Integer) As Long
		End Function

		''' <summary>
		''' Returns the number of entries in the ZIP file. </summary>
		''' <returns> the number of entries in the ZIP file </returns>
		''' <exception cref="IllegalStateException"> if the zip file has been closed </exception>
		Public Overridable Function size() As Integer
			ensureOpen()
			Return total
		End Function

		''' <summary>
		''' Closes the ZIP file.
		''' <p> Closing this ZIP file will close all of the input streams
		''' previously returned by invocations of the {@link #getInputStream
		''' getInputStream} method.
		''' </summary>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		Public Overridable Sub close()
			If closeRequested Then Return
			closeRequested = True

			SyncLock Me
				' Close streams, release their inflaters
				SyncLock streams
					If False = streams.Count = 0 Then
						Dim copy As IDictionary(Of java.io.InputStream, Inflater) = New Dictionary(Of java.io.InputStream, Inflater)(streams)
						streams.Clear()
						For Each e As KeyValuePair(Of java.io.InputStream, Inflater) In copy
							e.Key.close()
							Dim inf As Inflater = e.Value
							If inf IsNot Nothing Then inf.end()
						Next e
					End If
				End SyncLock

				' Release cached inflaters
				Dim inf As Inflater
				SyncLock inflaterCache
					inf = inflaterCache.poll()
					Do While Nothing IsNot inf
						inf.end()
						inf = inflaterCache.poll()
					Loop
				End SyncLock

				If jzfile <> 0 Then
					' Close the zip file
					Dim zf As Long = Me.jzfile
					jzfile = 0

					close(zf)
				End If
			End SyncLock
		End Sub

		''' <summary>
		''' Ensures that the system resources held by this ZipFile object are
		''' released when there are no more references to it.
		''' 
		''' <p>
		''' Since the time when GC would invoke this method is undetermined,
		''' it is strongly recommended that applications invoke the <code>close</code>
		''' method as soon they have finished accessing this <code>ZipFile</code>.
		''' This will prevent holding up system resources for an undetermined
		''' length of time.
		''' </summary>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		''' <seealso cref=    java.util.zip.ZipFile#close() </seealso>
		Protected Overrides Sub Finalize()
			close()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub close(ByVal jzfile As Long)
		End Sub

		Private Sub ensureOpen()
			If closeRequested Then Throw New IllegalStateException("zip file closed")

			If jzfile = 0 Then Throw New IllegalStateException("The object is not initialized.")
		End Sub

		Private Sub ensureOpenOrZipException()
			If closeRequested Then Throw New ZipException("ZipFile closed")
		End Sub

	'    
	'     * Inner class implementing the input stream used to read a
	'     * (possibly compressed) zip file entry.
	'     
	   Private Class ZipFileInputStream
		   Inherits java.io.InputStream

		   Private ReadOnly outerInstance As ZipFile

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Private closeRequested As Boolean = False
			Protected Friend jzentry As Long ' address of jzentry data
			Private pos As Long ' current position within entry data
			Protected Friend [rem] As Long ' number of remaining bytes within entry
			Protected Friend size_Renamed As Long ' uncompressed size of this entry

			Friend Sub New(ByVal outerInstance As ZipFile, ByVal jzentry As Long)
					Me.outerInstance = outerInstance
				pos = 0
				[rem] = getEntryCSize(jzentry)
				size_Renamed = getEntrySize(jzentry)
				Me.jzentry = jzentry
			End Sub

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			public int read(byte b() , int off, int len) throws java.io.IOException
				SyncLock ZipFile.this
					Dim [rem] As Long = Me.rem
					Dim pos As Long = Me.pos
					If [rem] = 0 Then Return -1
					If len <= 0 Then Return 0
					If len > [rem] Then len = CInt([rem])

					outerInstance.ensureOpenOrZipException()
					len = ZipFile.read(outerInstance.jzfile, jzentry, pos, b, off, len)
					If len > 0 Then
						Me.pos = (pos + len)
						Me.rem = ([rem] - len)
					End If
				End SyncLock
				If [rem] = 0 Then close()
				Return len

			public Integer read() throws java.io.IOException
				Dim b As SByte() = New SByte(0){}
				If read(b, 0, 1) = 1 Then
					Return b(0) And &Hff
				Else
					Return -1
				End If

			public Long skip(Long n)
				If n > [rem] Then n = [rem]
				pos += n
				[rem] -= n
				If [rem] = 0 Then close()
				Return n

			public Integer available()
				Return If([rem] > Integer.MaxValue, Integer.MaxValue, CInt([rem]))

			public Long size()
				Return size_Renamed

			public void close()
				If closeRequested Then Return
				closeRequested = True

				[rem] = 0
				SyncLock ZipFile.this
					If jzentry <> 0 AndAlso outerInstance.jzfile <> 0 Then
						freeEntry(outerInstance.jzfile, jzentry)
						jzentry = 0
					End If
				End SyncLock
				SyncLock outerInstance.streams
					outerInstance.streams.Remove(Me)
				End SyncLock

			protected void Finalize()
				close()
	   End Class


		''' <summary>
		''' Returns {@code true} if, and only if, the zip file begins with {@code
		''' LOCSIG}.
		''' </summary>
		private Boolean startsWithLocHeader()
			Return locsig

		private static native Long open(String name, Integer mode, Long lastModified, Boolean usemmap) throws java.io.IOException
		private static native Integer getTotal(Long jzfile)
		private static native Boolean startsWithLOC(Long jzfile)
		private static native Integer read(Long jzfile, Long jzentry, Long pos, SByte() b, Integer off, Integer len)

		' access to the native zentry object
		private static native Long getEntryTime(Long jzentry)
		private static native Long getEntryCrc(Long jzentry)
		private static native Long getEntryCSize(Long jzentry)
		private static native Long getEntrySize(Long jzentry)
		private static native Integer getEntryMethod(Long jzentry)
		private static native Integer getEntryFlag(Long jzentry)
		private static native SByte() getCommentBytes(Long jzfile)

		private static final Integer JZENTRY_NAME = 0
		private static final Integer JZENTRY_EXTRA = 1
		private static final Integer JZENTRY_COMMENT = 2
		private static native SByte() getEntryBytes(Long jzentry, Integer type)

		private static native String getZipMessage(Long jzfile)
	End Class

End Namespace