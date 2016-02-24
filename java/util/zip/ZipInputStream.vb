Imports Microsoft.VisualBasic
Imports System

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
	''' This class implements an input stream filter for reading files in the
	''' ZIP file format. Includes support for both compressed and uncompressed
	''' entries.
	''' 
	''' @author      David Connelly
	''' </summary>
	Public Class ZipInputStream
		Inherits InflaterInputStream
		Implements ZipConstants

		Private entry As ZipEntry
		Private flag As Integer
		Private crc As New CRC32
		Private remaining As Long
		Private tmpbuf As SByte() = New SByte(511){}

		Private Const STORED As Integer = ZipEntry.STORED
		Private Const DEFLATED As Integer = ZipEntry.DEFLATED

		Private closed As Boolean = False
		' this flag is set to true after EOF has reached for
		' one entry
		Private entryEOF As Boolean = False

		Private zc As ZipCoder

		''' <summary>
		''' Check to make sure that this stream has not been closed
		''' </summary>
		Private Sub ensureOpen()
			If closed Then Throw New java.io.IOException("Stream closed")
		End Sub

		''' <summary>
		''' Creates a new ZIP input stream.
		''' 
		''' <p>The UTF-8 <seealso cref="java.nio.charset.Charset charset"/> is used to
		''' decode the entry names.
		''' </summary>
		''' <param name="in"> the actual input stream </param>
		Public Sub New(ByVal [in] As java.io.InputStream)
			Me.New([in], java.nio.charset.StandardCharsets.UTF_8)
		End Sub

		''' <summary>
		''' Creates a new ZIP input stream.
		''' </summary>
		''' <param name="in"> the actual input stream
		''' </param>
		''' <param name="charset">
		'''        The <seealso cref="java.nio.charset.Charset charset"/> to be
		'''        used to decode the ZIP entry name (ignored if the
		'''        <a href="package-summary.html#lang_encoding"> language
		'''        encoding bit</a> of the ZIP entry's general purpose bit
		'''        flag is set).
		''' 
		''' @since 1.7 </param>
		Public Sub New(ByVal [in] As java.io.InputStream, ByVal charset As java.nio.charset.Charset)
			MyBase.New(New java.io.PushbackInputStream([in], 512), New Inflater(True), 512)
			usesDefaultInflater = True
			If [in] Is Nothing Then Throw New NullPointerException("in is null")
			If charset Is Nothing Then Throw New NullPointerException("charset is null")
			Me.zc = ZipCoder.get(charset)
		End Sub

		''' <summary>
		''' Reads the next ZIP file entry and positions the stream at the
		''' beginning of the entry data. </summary>
		''' <returns> the next ZIP file entry, or null if there are no more entries </returns>
		''' <exception cref="ZipException"> if a ZIP file error has occurred </exception>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		Public Overridable Property nextEntry As ZipEntry
			Get
				ensureOpen()
				If entry IsNot Nothing Then closeEntry()
				crc.reset()
				inf.reset()
				entry = readLOC()
				If entry Is Nothing Then Return Nothing
				If entry.method = STORED Then remaining = entry.size
				entryEOF = False
				Return entry
			End Get
		End Property

		''' <summary>
		''' Closes the current ZIP entry and positions the stream for reading the
		''' next entry. </summary>
		''' <exception cref="ZipException"> if a ZIP file error has occurred </exception>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		Public Overridable Sub closeEntry()
			ensureOpen()
			Do While read(tmpbuf, 0, tmpbuf.Length) <> -1

			Loop
			entryEOF = True
		End Sub

		''' <summary>
		''' Returns 0 after EOF has reached for the current entry data,
		''' otherwise always return 1.
		''' <p>
		''' Programs should not count on this method to return the actual number
		''' of bytes that could be read without blocking.
		''' </summary>
		''' <returns>     1 before EOF and 0 after EOF has reached for current entry. </returns>
		''' <exception cref="IOException">  if an I/O error occurs.
		'''  </exception>
		Public Overrides Function available() As Integer
			ensureOpen()
			If entryEOF Then
				Return 0
			Else
				Return 1
			End If
		End Function

		''' <summary>
		''' Reads from the current ZIP entry into an array of bytes.
		''' If <code>len</code> is not zero, the method
		''' blocks until some input is available; otherwise, no
		''' bytes are read and <code>0</code> is returned. </summary>
		''' <param name="b"> the buffer into which the data is read </param>
		''' <param name="off"> the start offset in the destination array <code>b</code> </param>
		''' <param name="len"> the maximum number of bytes read </param>
		''' <returns> the actual number of bytes read, or -1 if the end of the
		'''         entry is reached </returns>
		''' <exception cref="NullPointerException"> if <code>b</code> is <code>null</code>. </exception>
		''' <exception cref="IndexOutOfBoundsException"> if <code>off</code> is negative,
		''' <code>len</code> is negative, or <code>len</code> is greater than
		''' <code>b.length - off</code> </exception>
		''' <exception cref="ZipException"> if a ZIP file error has occurred </exception>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		Public Overrides Function read(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer) As Integer
			ensureOpen()
			If [off] < 0 OrElse len < 0 OrElse [off] > b.Length - len Then
				Throw New IndexOutOfBoundsException
			ElseIf len = 0 Then
				Return 0
			End If

			If entry Is Nothing Then Return -1
			Select Case entry.method
			Case DEFLATED
				len = MyBase.read(b, [off], len)
				If len = -1 Then
					readEnd(entry)
					entryEOF = True
					entry = Nothing
				Else
					crc.update(b, [off], len)
				End If
				Return len
			Case STORED
				If remaining <= 0 Then
					entryEOF = True
					entry = Nothing
					Return -1
				End If
				If len > remaining Then len = CInt(remaining)
				len = [in].read(b, [off], len)
				If len = -1 Then Throw New ZipException("unexpected EOF")
				crc.update(b, [off], len)
				remaining -= len
				If remaining = 0 AndAlso entry.crc <> crc.value Then Throw New ZipException("invalid entry CRC (expected 0x" & Long.toHexString(entry.crc) & " but got 0x" & Long.toHexString(crc.value) & ")")
				Return len
			Case Else
				Throw New ZipException("invalid compression method")
			End Select
		End Function

		''' <summary>
		''' Skips specified number of bytes in the current ZIP entry. </summary>
		''' <param name="n"> the number of bytes to skip </param>
		''' <returns> the actual number of bytes skipped </returns>
		''' <exception cref="ZipException"> if a ZIP file error has occurred </exception>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		''' <exception cref="IllegalArgumentException"> if {@code n < 0} </exception>
		Public Overrides Function skip(ByVal n As Long) As Long
			If n < 0 Then Throw New IllegalArgumentException("negative skip length")
			ensureOpen()
			Dim max As Integer = CInt(Fix(Math.Min(n, Integer.MaxValue)))
			Dim total As Integer = 0
			Do While total < max
				Dim len As Integer = max - total
				If len > tmpbuf.Length Then len = tmpbuf.Length
				len = read(tmpbuf, 0, len)
				If len = -1 Then
					entryEOF = True
					Exit Do
				End If
				total += len
			Loop
			Return total
		End Function

		''' <summary>
		''' Closes this input stream and releases any system resources associated
		''' with the stream. </summary>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		Public Overrides Sub close()
			If Not closed Then
				MyBase.close()
				closed = True
			End If
		End Sub

		Private b As SByte() = New SByte(255){}

	'    
	'     * Reads local file (LOC) header for next entry.
	'     
		Private Function readLOC() As ZipEntry
			Try
				readFully(tmpbuf, 0, LOCHDR)
			Catch e As java.io.EOFException
				Return Nothing
			End Try
			If get32(tmpbuf, 0) <> LOCSIG Then Return Nothing
			' get flag first, we need check EFS.
			flag = get16(tmpbuf, LOCFLG)
			' get the entry name and create the ZipEntry first
			Dim len As Integer = get16(tmpbuf, LOCNAM)
			Dim blen As Integer = b.Length
			If len > blen Then
				Do
					blen = blen * 2
				Loop While len > blen
				b = New SByte(blen - 1){}
			End If
			readFully(b, 0, len)
			' Force to use UTF-8 if the EFS bit is ON, even the cs is NOT UTF-8
			Dim e As ZipEntry = createZipEntry(If((flag And EFS) <> 0, zc.toStringUTF8(b, len), zc.ToString(b, len)))
			' now get the remaining fields for the entry
			If (flag And 1) = 1 Then Throw New ZipException("encrypted ZIP entry not supported")
			e.method = get16(tmpbuf, LOCHOW)
			e.xdostime = get32(tmpbuf, LOCTIM)
			If (flag And 8) = 8 Then
				' "Data Descriptor" present 
				If e.method <> DEFLATED Then Throw New ZipException("only DEFLATED entries can have EXT descriptor")
			Else
				e.crc = get32(tmpbuf, LOCCRC)
				e.csize = get32(tmpbuf, LOCSIZ)
				e.size = get32(tmpbuf, LOCLEN)
			End If
			len = get16(tmpbuf, LOCEXT)
			If len > 0 Then
				Dim extra As SByte() = New SByte(len - 1){}
				readFully(extra, 0, len)
				e.extra0ra0(extra, e.csize = ZIP64_MAGICVAL OrElse e.size = ZIP64_MAGICVAL)
			End If
			Return e
		End Function

		''' <summary>
		''' Creates a new <code>ZipEntry</code> object for the specified
		''' entry name.
		''' </summary>
		''' <param name="name"> the ZIP file entry name </param>
		''' <returns> the ZipEntry just created </returns>
		Protected Friend Overridable Function createZipEntry(ByVal name As String) As ZipEntry
			Return New ZipEntry(name)
		End Function

	'    
	'     * Reads end of deflated entry as well as EXT descriptor if present.
	'     
		Private Sub readEnd(ByVal e As ZipEntry)
			Dim n As Integer = inf.remaining
			If n > 0 Then CType([in], java.io.PushbackInputStream).unread(buf, len - n, n)
			If (flag And 8) = 8 Then
				' "Data Descriptor" present 
				If inf.bytesWritten > ZIP64_MAGICVAL OrElse inf.bytesRead > ZIP64_MAGICVAL Then
					' ZIP64 format
					readFully(tmpbuf, 0, ZIP64_EXTHDR)
					Dim sig As Long = get32(tmpbuf, 0)
					If sig <> EXTSIG Then ' no EXTSIG present
						e.crc = sig
						e.csize = get64(tmpbuf, ZIP64_EXTSIZ - ZIP64_EXTCRC)
						e.size = get64(tmpbuf, ZIP64_EXTLEN - ZIP64_EXTCRC)
						CType([in], java.io.PushbackInputStream).unread(tmpbuf, ZIP64_EXTHDR - ZIP64_EXTCRC - 1, ZIP64_EXTCRC)
					Else
						e.crc = get32(tmpbuf, ZIP64_EXTCRC)
						e.csize = get64(tmpbuf, ZIP64_EXTSIZ)
						e.size = get64(tmpbuf, ZIP64_EXTLEN)
					End If
				Else
					readFully(tmpbuf, 0, EXTHDR)
					Dim sig As Long = get32(tmpbuf, 0)
					If sig <> EXTSIG Then ' no EXTSIG present
						e.crc = sig
						e.csize = get32(tmpbuf, EXTSIZ - EXTCRC)
						e.size = get32(tmpbuf, EXTLEN - EXTCRC)
						CType([in], java.io.PushbackInputStream).unread(tmpbuf, EXTHDR - EXTCRC - 1, EXTCRC)
					Else
						e.crc = get32(tmpbuf, EXTCRC)
						e.csize = get32(tmpbuf, EXTSIZ)
						e.size = get32(tmpbuf, EXTLEN)
					End If
				End If
			End If
			If e.size <> inf.bytesWritten Then Throw New ZipException("invalid entry size (expected " & e.size & " but got " & inf.bytesWritten & " bytes)")
			If e.csize <> inf.bytesRead Then Throw New ZipException("invalid entry compressed size (expected " & e.csize & " but got " & inf.bytesRead & " bytes)")
			If e.crc <> crc.value Then Throw New ZipException("invalid entry CRC (expected 0x" & Long.toHexString(e.crc) & " but got 0x" & Long.toHexString(crc.value) & ")")
		End Sub

	'    
	'     * Reads bytes, blocking until all bytes are read.
	'     
		Private Sub readFully(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer)
			Do While len > 0
				Dim n As Integer = [in].read(b, [off], len)
				If n = -1 Then Throw New java.io.EOFException
				[off] += n
				len -= n
			Loop
		End Sub

	End Class

End Namespace