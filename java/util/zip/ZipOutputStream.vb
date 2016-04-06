Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic

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

Namespace java.util.zip


	''' <summary>
	''' This class implements an output stream filter for writing files in the
	''' ZIP file format. Includes support for both compressed and uncompressed
	''' entries.
	''' 
	''' @author      David Connelly
	''' </summary>
	Public Class ZipOutputStream
		Inherits DeflaterOutputStream
		Implements ZipConstants

		''' <summary>
		''' Whether to use ZIP64 for zip files with more than 64k entries.
		''' Until ZIP64 support in zip implementations is ubiquitous, this
		''' system property allows the creation of zip files which can be
		''' read by legacy zip implementations which tolerate "incorrect"
		''' total entry count fields, such as the ones in jdk6, and even
		''' some in jdk7.
		''' </summary>
		Private Shared ReadOnly inhibitZip64 As Boolean =  java.lang.[Boolean].parseBoolean(java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("jdk.util.zip.inhibitZip64", "false")))

		Private Class XEntry
			Friend ReadOnly entry As ZipEntry
			Friend ReadOnly offset As Long
			Public Sub New(  entry As ZipEntry,   offset As Long)
				Me.entry = entry
				Me.offset = offset
			End Sub
		End Class

		Private current As XEntry
		Private xentries As New List(Of XEntry)
		Private names As New HashSet(Of String)
		Private crc As New CRC32
		Private written As Long = 0
		Private locoff As Long = 0
		Private comment As SByte()
		Private method As Integer = DEFLATED
		Private finished As Boolean

		Private closed As Boolean = False

		Private ReadOnly zc As ZipCoder

		Private Shared Function version(  e As ZipEntry) As Integer
			Select Case e.method
			Case DEFLATED
				Return 20
			Case STORED
				Return 10
			Case Else
				Throw New ZipException("unsupported compression method")
			End Select
		End Function

		''' <summary>
		''' Checks to make sure that this stream has not been closed.
		''' </summary>
		Private Sub ensureOpen()
			If closed Then Throw New java.io.IOException("Stream closed")
		End Sub
		''' <summary>
		''' Compression method for uncompressed (STORED) entries.
		''' </summary>
		Public Const STORED As Integer = ZipEntry.STORED

		''' <summary>
		''' Compression method for compressed (DEFLATED) entries.
		''' </summary>
		Public Const DEFLATED As Integer = ZipEntry.DEFLATED

		''' <summary>
		''' Creates a new ZIP output stream.
		''' 
		''' <p>The UTF-8 <seealso cref="java.nio.charset.Charset charset"/> is used
		''' to encode the entry names and comments.
		''' </summary>
		''' <param name="out"> the actual output stream </param>
		Public Sub New(  out As java.io.OutputStream)
			Me.New(out, java.nio.charset.StandardCharsets.UTF_8)
		End Sub

		''' <summary>
		''' Creates a new ZIP output stream.
		''' </summary>
		''' <param name="out"> the actual output stream
		''' </param>
		''' <param name="charset"> the <seealso cref="java.nio.charset.Charset charset"/>
		'''                to be used to encode the entry names and comments
		''' 
		''' @since 1.7 </param>
		Public Sub New(  out As java.io.OutputStream,   charset As java.nio.charset.Charset)
			MyBase.New(out, New Deflater(Deflater.DEFAULT_COMPRESSION, True))
			If charset Is Nothing Then Throw New NullPointerException("charset is null")
			Me.zc = ZipCoder.get(charset)
			usesDefaultDeflater = True
		End Sub

		''' <summary>
		''' Sets the ZIP file comment. </summary>
		''' <param name="comment"> the comment string </param>
		''' <exception cref="IllegalArgumentException"> if the length of the specified
		'''            ZIP file comment is greater than 0xFFFF bytes </exception>
		Public Overridable Property comment As String
			Set(  comment As String)
				If comment IsNot Nothing Then
					Me.comment = zc.getBytes(comment)
					If Me.comment.Length > &Hffff Then Throw New IllegalArgumentException("ZIP file comment too java.lang.[Long].")
				End If
			End Set
		End Property

		''' <summary>
		''' Sets the default compression method for subsequent entries. This
		''' default will be used whenever the compression method is not specified
		''' for an individual ZIP file entry, and is initially set to DEFLATED. </summary>
		''' <param name="method"> the default compression method </param>
		''' <exception cref="IllegalArgumentException"> if the specified compression method
		'''            is invalid </exception>
		Public Overridable Property method As Integer
			Set(  method As Integer)
				If method <> DEFLATED AndAlso method <> STORED Then Throw New IllegalArgumentException("invalid compression method")
				Me.method = method
			End Set
		End Property

		''' <summary>
		''' Sets the compression level for subsequent entries which are DEFLATED.
		''' The default setting is DEFAULT_COMPRESSION. </summary>
		''' <param name="level"> the compression level (0-9) </param>
		''' <exception cref="IllegalArgumentException"> if the compression level is invalid </exception>
		Public Overridable Property level As Integer
			Set(  level As Integer)
				def.level = level
			End Set
		End Property

		''' <summary>
		''' Begins writing a new ZIP file entry and positions the stream to the
		''' start of the entry data. Closes the current entry if still active.
		''' The default compression method will be used if no compression method
		''' was specified for the entry, and the current time will be used if
		''' the entry has no set modification time. </summary>
		''' <param name="e"> the ZIP entry to be written </param>
		''' <exception cref="ZipException"> if a ZIP format error has occurred </exception>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		Public Overridable Sub putNextEntry(  e As ZipEntry)
			ensureOpen()
			If current IsNot Nothing Then closeEntry() ' close previous entry
			If e.xdostime = -1 Then e.time = System.currentTimeMillis()
			If e.method = -1 Then e.method = method ' use default method
			' store size, compressed size, and crc-32 in LOC header
			e.flag = 0
			Select Case e.method
			Case DEFLATED
				' store size, compressed size, and crc-32 in data descriptor
				' immediately following the compressed entry data
				If e.size = -1 OrElse e.csize = -1 OrElse e.crc = -1 Then e.flag = 8

			Case STORED
				' compressed size, uncompressed size, and crc-32 must all be
				' set for entries using STORED compression method
				If e.size = -1 Then
					e.size = e.csize
				ElseIf e.csize = -1 Then
					e.csize = e.size
				ElseIf e.size <> e.csize Then
					Throw New ZipException("STORED entry where compressed != uncompressed size")
				End If
				If e.size = -1 OrElse e.crc = -1 Then Throw New ZipException("STORED entry missing size, compressed size, or crc-32")
			Case Else
				Throw New ZipException("unsupported compression method")
			End Select
			If Not names.add(e.name) Then Throw New ZipException("duplicate entry: " & e.name)
			If zc.uTF8 Then e.flag = e.flag Or EFS
			current = New XEntry(e, written)
			xentries.add(current)
			writeLOC(current)
		End Sub

		''' <summary>
		''' Closes the current ZIP entry and positions the stream for writing
		''' the next entry. </summary>
		''' <exception cref="ZipException"> if a ZIP format error has occurred </exception>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		Public Overridable Sub closeEntry()
			ensureOpen()
			If current IsNot Nothing Then
				Dim e As ZipEntry = current.entry
				Select Case e.method
				Case DEFLATED
					def.finish()
					Do While Not def.finished()
						deflate()
					Loop
					If (e.flag And 8) = 0 Then
						' verify size, compressed size, and crc-32 settings
						If e.size <> def.bytesRead Then Throw New ZipException("invalid entry size (expected " & e.size & " but got " & def.bytesRead & " bytes)")
						If e.csize <> def.bytesWritten Then Throw New ZipException("invalid entry compressed size (expected " & e.csize & " but got " & def.bytesWritten & " bytes)")
						If e.crc <> crc.value Then Throw New ZipException("invalid entry CRC-32 (expected 0x" & java.lang.[Long].toHexString(e.crc) & " but got 0x" & java.lang.[Long].toHexString(crc.value) & ")")
					Else
						e.size = def.bytesRead
						e.csize = def.bytesWritten
						e.crc = crc.value
						writeEXT(e)
					End If
					def.reset()
					written += e.csize
				Case STORED
					' we already know that both e.size and e.csize are the same
					If e.size <> written - locoff Then Throw New ZipException("invalid entry size (expected " & e.size & " but got " & (written - locoff) & " bytes)")
					If e.crc <> crc.value Then Throw New ZipException("invalid entry crc-32 (expected 0x" & java.lang.[Long].toHexString(e.crc) & " but got 0x" & java.lang.[Long].toHexString(crc.value) & ")")
				Case Else
					Throw New ZipException("invalid compression method")
				End Select
				crc.reset()
				current = Nothing
			End If
		End Sub

		''' <summary>
		''' Writes an array of bytes to the current ZIP entry data. This method
		''' will block until all the bytes are written. </summary>
		''' <param name="b"> the data to be written </param>
		''' <param name="off"> the start offset in the data </param>
		''' <param name="len"> the number of bytes that are written </param>
		''' <exception cref="ZipException"> if a ZIP file error has occurred </exception>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Sub write(  b As SByte(),   [off] As Integer,   len As Integer)
			ensureOpen()
			If [off] < 0 OrElse len < 0 OrElse [off] > b.Length - len Then
				Throw New IndexOutOfBoundsException
			ElseIf len = 0 Then
				Return
			End If

			If current Is Nothing Then Throw New ZipException("no current ZIP entry")
			Dim entry As ZipEntry = current.entry
			Select Case entry.method
			Case DEFLATED
				MyBase.write(b, [off], len)
			Case STORED
				written += len
				If written - locoff > entry.size Then Throw New ZipException("attempt to write past end of STORED entry")
				out.write(b, [off], len)
			Case Else
				Throw New ZipException("invalid compression method")
			End Select
			crc.update(b, [off], len)
		End Sub

		''' <summary>
		''' Finishes writing the contents of the ZIP output stream without closing
		''' the underlying stream. Use this method when applying multiple filters
		''' in succession to the same output stream. </summary>
		''' <exception cref="ZipException"> if a ZIP file error has occurred </exception>
		''' <exception cref="IOException"> if an I/O exception has occurred </exception>
		Public Overrides Sub finish()
			ensureOpen()
			If finished Then Return
			If current IsNot Nothing Then closeEntry()
			' write central directory
			Dim [off] As Long = written
			For Each xentry As XEntry In xentries
				writeCEN(xentry)
			Next xentry
			writeEND([off], written - [off])
			finished = True
		End Sub

		''' <summary>
		''' Closes the ZIP output stream as well as the stream being filtered. </summary>
		''' <exception cref="ZipException"> if a ZIP file error has occurred </exception>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		Public Overrides Sub close()
			If Not closed Then
				MyBase.close()
				closed = True
			End If
		End Sub

	'    
	'     * Writes local file (LOC) header for specified entry.
	'     
		Private Sub writeLOC(  xentry As XEntry)
			Dim e As ZipEntry = xentry.entry
			Dim flag As Integer = e.flag
			Dim hasZip64 As Boolean = False
			Dim elen As Integer = getExtraLen(e.extra)

			writeInt(LOCSIG) ' LOC header signature
			If (flag And 8) = 8 Then
				writeShort(version(e)) ' version needed to extract
				writeShort(flag) ' general purpose bit flag
				writeShort(e.method) ' compression method
				writeInt(e.xdostime) ' last modification time
				' store size, uncompressed size, and crc-32 in data descriptor
				' immediately following compressed entry data
				writeInt(0)
				writeInt(0)
				writeInt(0)
			Else
				If e.csize >= ZIP64_MAGICVAL OrElse e.size >= ZIP64_MAGICVAL Then
					hasZip64 = True
					writeShort(45) ' ver 4.5 for zip64
				Else
					writeShort(version(e)) ' version needed to extract
				End If
				writeShort(flag) ' general purpose bit flag
				writeShort(e.method) ' compression method
				writeInt(e.xdostime) ' last modification time
				writeInt(e.crc) ' crc-32
				If hasZip64 Then
					writeInt(ZIP64_MAGICVAL)
					writeInt(ZIP64_MAGICVAL)
					elen += 20 'headid(2) + size(2) + size(8) + csize(8)
				Else
					writeInt(e.csize) ' compressed size
					writeInt(e.size) ' uncompressed size
				End If
			End If
			Dim nameBytes As SByte() = zc.getBytes(e.name)
			writeShort(nameBytes.Length)

			Dim elenEXTT As Integer = 0 ' info-zip extended timestamp
			Dim flagEXTT As Integer = 0
			If e.mtime IsNot Nothing Then
				elenEXTT += 4
				flagEXTT = flagEXTT Or EXTT_FLAG_LMT
			End If
			If e.atime IsNot Nothing Then
				elenEXTT += 4
				flagEXTT = flagEXTT Or EXTT_FLAG_LAT
			End If
			If e.ctime IsNot Nothing Then
				elenEXTT += 4
				flagEXTT = flagEXTT Or EXTT_FLAT_CT
			End If
			If flagEXTT <> 0 Then elen += (elenEXTT + 5) ' headid(2) + size(2) + flag(1) + data
			writeShort(elen)
			writeBytes(nameBytes, 0, nameBytes.Length)
			If hasZip64 Then
				writeShort(ZIP64_EXTID)
				writeShort(16)
				writeLong(e.size)
				writeLong(e.csize)
			End If
			If flagEXTT <> 0 Then
				writeShort(EXTID_EXTT)
				writeShort(elenEXTT + 1) ' flag + data
				writeByte(flagEXTT)
				If e.mtime IsNot Nothing Then writeInt(fileTimeToUnixTime(e.mtime))
				If e.atime IsNot Nothing Then writeInt(fileTimeToUnixTime(e.atime))
				If e.ctime IsNot Nothing Then writeInt(fileTimeToUnixTime(e.ctime))
			End If
			writeExtra(e.extra)
			locoff = written
		End Sub

	'    
	'     * Writes extra data descriptor (EXT) for specified entry.
	'     
		Private Sub writeEXT(  e As ZipEntry)
			writeInt(EXTSIG) ' EXT header signature
			writeInt(e.crc) ' crc-32
			If e.csize >= ZIP64_MAGICVAL OrElse e.size >= ZIP64_MAGICVAL Then
				writeLong(e.csize)
				writeLong(e.size)
			Else
				writeInt(e.csize) ' compressed size
				writeInt(e.size) ' uncompressed size
			End If
		End Sub

	'    
	'     * Write central directory (CEN) header for specified entry.
	'     * REMIND: add support for file attributes
	'     
		Private Sub writeCEN(  xentry As XEntry)
			Dim e As ZipEntry = xentry.entry
			Dim flag As Integer = e.flag
			Dim version As Integer = version(e)
			Dim csize As Long = e.csize
			Dim size As Long = e.size
			Dim offset As Long = xentry.offset
			Dim elenZIP64 As Integer = 0
			Dim hasZip64 As Boolean = False

			If e.csize >= ZIP64_MAGICVAL Then
				csize = ZIP64_MAGICVAL
				elenZIP64 += 8 ' csize(8)
				hasZip64 = True
			End If
			If e.size >= ZIP64_MAGICVAL Then
				size = ZIP64_MAGICVAL ' size(8)
				elenZIP64 += 8
				hasZip64 = True
			End If
			If xentry.offset >= ZIP64_MAGICVAL Then
				offset = ZIP64_MAGICVAL
				elenZIP64 += 8 ' offset(8)
				hasZip64 = True
			End If
			writeInt(CENSIG) ' CEN header signature
			If hasZip64 Then
				writeShort(45) ' ver 4.5 for zip64
				writeShort(45)
			Else
				writeShort(version) ' version made by
				writeShort(version) ' version needed to extract
			End If
			writeShort(flag) ' general purpose bit flag
			writeShort(e.method) ' compression method
			writeInt(e.xdostime) ' last modification time
			writeInt(e.crc) ' crc-32
			writeInt(csize) ' compressed size
			writeInt(size) ' uncompressed size
			Dim nameBytes As SByte() = zc.getBytes(e.name)
			writeShort(nameBytes.Length)

			Dim elen As Integer = getExtraLen(e.extra)
			If hasZip64 Then elen += (elenZIP64 + 4) ' + headid(2) + datasize(2)
			' cen info-zip extended timestamp only outputs mtime
			' but set the flag for a/ctime, if present in loc
			Dim flagEXTT As Integer = 0
			If e.mtime IsNot Nothing Then
				elen += 4 ' + mtime(4)
				flagEXTT = flagEXTT Or EXTT_FLAG_LMT
			End If
			If e.atime IsNot Nothing Then flagEXTT = flagEXTT Or EXTT_FLAG_LAT
			If e.ctime IsNot Nothing Then flagEXTT = flagEXTT Or EXTT_FLAT_CT
			If flagEXTT <> 0 Then elen += 5 ' headid + sz + flag
			writeShort(elen)
			Dim commentBytes As SByte()
			If e.comment IsNot Nothing Then
				commentBytes = zc.getBytes(e.comment)
				writeShort (System.Math.Min(commentBytes.Length, &Hffff))
			Else
				commentBytes = Nothing
				writeShort(0)
			End If
			writeShort(0) ' starting disk number
			writeShort(0) ' internal file attributes (unused)
			writeInt(0) ' external file attributes (unused)
			writeInt(offset) ' relative offset of local header
			writeBytes(nameBytes, 0, nameBytes.Length)

			' take care of EXTID_ZIP64 and EXTID_EXTT
			If hasZip64 Then
				writeShort(ZIP64_EXTID) ' Zip64 extra
				writeShort(elenZIP64)
				If size = ZIP64_MAGICVAL Then writeLong(e.size)
				If csize = ZIP64_MAGICVAL Then writeLong(e.csize)
				If offset = ZIP64_MAGICVAL Then writeLong(xentry.offset)
			End If
			If flagEXTT <> 0 Then
				writeShort(EXTID_EXTT)
				If e.mtime IsNot Nothing Then
					writeShort(5) ' flag + mtime
					writeByte(flagEXTT)
					writeInt(fileTimeToUnixTime(e.mtime))
				Else
					writeShort(1) ' flag only
					writeByte(flagEXTT)
				End If
			End If
			writeExtra(e.extra)
			If commentBytes IsNot Nothing Then writeBytes(commentBytes, 0, System.Math.Min(commentBytes.Length, &Hffff))
		End Sub

	'    
	'     * Writes end of central directory (END) header.
	'     
		Private Sub writeEND(  [off] As Long,   len As Long)
			Dim hasZip64 As Boolean = False
			Dim xlen As Long = len
			Dim xoff As Long = [off]
			If xlen >= ZIP64_MAGICVAL Then
				xlen = ZIP64_MAGICVAL
				hasZip64 = True
			End If
			If xoff >= ZIP64_MAGICVAL Then
				xoff = ZIP64_MAGICVAL
				hasZip64 = True
			End If
			Dim count As Integer = xentries.size()
			If count >= ZIP64_MAGICCOUNT Then
				hasZip64 = hasZip64 Or Not inhibitZip64
				If hasZip64 Then count = ZIP64_MAGICCOUNT
			End If
			If hasZip64 Then
				Dim off64 As Long = written
				'zip64 end of central directory record
				writeInt(ZIP64_ENDSIG) ' zip64 END record signature
				writeLong(ZIP64_ENDHDR - 12) ' size of zip64 end
				writeShort(45) ' version made by
				writeShort(45) ' version needed to extract
				writeInt(0) ' number of this disk
				writeInt(0) ' central directory start disk
				writeLong(xentries.size()) ' number of directory entires on disk
				writeLong(xentries.size()) ' number of directory entires
				writeLong(len) ' length of central directory
				writeLong([off]) ' offset of central directory

				'zip64 end of central directory locator
				writeInt(ZIP64_LOCSIG) ' zip64 END locator signature
				writeInt(0) ' zip64 END start disk
				writeLong(off64) ' offset of zip64 END
				writeInt(1) ' total number of disks (?)
			End If
			writeInt(ENDSIG) ' END record signature
			writeShort(0) ' number of this disk
			writeShort(0) ' central directory start disk
			writeShort(count) ' number of directory entries on disk
			writeShort(count) ' total number of directory entries
			writeInt(xlen) ' length of central directory
			writeInt(xoff) ' offset of central directory
			If comment IsNot Nothing Then ' zip file comment
				writeShort(comment.Length)
				writeBytes(comment, 0, comment.Length)
			Else
				writeShort(0)
			End If
		End Sub

	'    
	'     * Returns the length of extra data without EXTT and ZIP64.
	'     
		Private Function getExtraLen(  extra As SByte()) As Integer
			If extra Is Nothing Then Return 0
			Dim skipped As Integer = 0
			Dim len As Integer = extra.Length
			Dim [off] As Integer = 0
			Do While [off] + 4 <= len
				Dim tag As Integer = get16(extra, [off])
				Dim sz As Integer = get16(extra, [off] + 2)
				If sz < 0 OrElse ([off] + 4 + sz) > len Then Exit Do
				If tag = EXTID_EXTT OrElse tag = EXTID_ZIP64 Then skipped += (sz + 4)
				[off] += (sz + 4)
			Loop
			Return len - skipped
		End Function

	'    
	'     * Writes extra data without EXTT and ZIP64.
	'     *
	'     * Extra timestamp and ZIP64 data is handled/output separately
	'     * in writeLOC and writeCEN.
	'     
		Private Sub writeExtra(  extra As SByte())
			If extra IsNot Nothing Then
				Dim len As Integer = extra.Length
				Dim [off] As Integer = 0
				Do While [off] + 4 <= len
					Dim tag As Integer = get16(extra, [off])
					Dim sz As Integer = get16(extra, [off] + 2)
					If sz < 0 OrElse ([off] + 4 + sz) > len Then
						writeBytes(extra, [off], len - [off])
						Return
					End If
					If tag <> EXTID_EXTT AndAlso tag <> EXTID_ZIP64 Then writeBytes(extra, [off], sz + 4)
					[off] += (sz + 4)
				Loop
				If [off] < len Then writeBytes(extra, [off], len - [off])
			End If
		End Sub

	'    
	'     * Writes a 8-bit byte to the output stream.
	'     
		Private Sub writeByte(  v As Integer)
			Dim out As java.io.OutputStream = Me.out
			out.write(v And &Hff)
			written += 1
		End Sub

	'    
	'     * Writes a 16-bit short to the output stream in little-endian byte order.
	'     
		Private Sub writeShort(  v As Integer)
			Dim out As java.io.OutputStream = Me.out
			out.write((CInt(CUInt(v) >> 0)) And &Hff)
			out.write((CInt(CUInt(v) >> 8)) And &Hff)
			written += 2
		End Sub

	'    
	'     * Writes a 32-bit int to the output stream in little-endian byte order.
	'     
		Private Sub writeInt(  v As Long)
			Dim out As java.io.OutputStream = Me.out
			out.write(CInt(Fix((CLng(CULng(v) >> 0)) And &Hff)))
			out.write(CInt(Fix((CLng(CULng(v) >> 8)) And &Hff)))
			out.write(CInt(Fix((CLng(CULng(v) >> 16)) And &Hff)))
			out.write(CInt(Fix((CLng(CULng(v) >> 24)) And &Hff)))
			written += 4
		End Sub

	'    
	'     * Writes a 64-bit int to the output stream in little-endian byte order.
	'     
		Private Sub writeLong(  v As Long)
			Dim out As java.io.OutputStream = Me.out
			out.write(CInt(Fix((CLng(CULng(v) >> 0)) And &Hff)))
			out.write(CInt(Fix((CLng(CULng(v) >> 8)) And &Hff)))
			out.write(CInt(Fix((CLng(CULng(v) >> 16)) And &Hff)))
			out.write(CInt(Fix((CLng(CULng(v) >> 24)) And &Hff)))
			out.write(CInt(Fix((CLng(CULng(v) >> 32)) And &Hff)))
			out.write(CInt(Fix((CLng(CULng(v) >> 40)) And &Hff)))
			out.write(CInt(Fix((CLng(CULng(v) >> 48)) And &Hff)))
			out.write(CInt(Fix((CLng(CULng(v) >> 56)) And &Hff)))
			written += 8
		End Sub

	'    
	'     * Writes an array of bytes to the output stream.
	'     
		Private Sub writeBytes(  b As SByte(),   [off] As Integer,   len As Integer)
			MyBase.out.write(b, [off], len)
			written += len
		End Sub
	End Class

End Namespace