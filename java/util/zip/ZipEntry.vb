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
	''' This class is used to represent a ZIP file entry.
	''' 
	''' @author      David Connelly
	''' </summary>
	Public Class ZipEntry
		Implements ZipConstants, Cloneable

		Friend name As String ' entry name
		Friend xdostime As Long = -1 ' last modification time (in extended DOS time,
							' where milliseconds lost in conversion might
							' be encoded into the upper half)
		Friend mtime As java.nio.file.attribute.FileTime ' last modification time, from extra field data
		Friend atime As java.nio.file.attribute.FileTime ' last access time, from extra field data
		Friend ctime As java.nio.file.attribute.FileTime ' creation time, from extra field data
		Friend crc As Long = -1 ' crc-32 of entry data
		Friend size As Long = -1 ' uncompressed size of entry data
		Friend csize As Long = -1 ' compressed size of entry data
		Friend method As Integer = -1 ' compression method
		Friend flag As Integer = 0 ' general purpose flag
		Friend extra As SByte() ' optional extra field data for entry
		Friend comment As String ' optional comment string for entry

		''' <summary>
		''' Compression method for uncompressed entries.
		''' </summary>
		Public Const STORED As Integer = 0

		''' <summary>
		''' Compression method for compressed (deflated) entries.
		''' </summary>
		Public Const DEFLATED As Integer = 8

		''' <summary>
		''' DOS time constant for representing timestamps before 1980.
		''' </summary>
		Friend Shared ReadOnly DOSTIME_BEFORE_1980 As Long = (1 << 21) Or (1 << 16)

		''' <summary>
		''' Approximately 128 years, in milliseconds (ignoring leap years etc).
		''' 
		''' This establish an approximate high-bound value for DOS times in
		''' milliseconds since epoch, used to enable an efficient but
		''' sufficient bounds check to avoid generating extended last modified
		''' time entries.
		''' 
		''' Calculating the exact number is locale dependent, would require loading
		''' TimeZone data eagerly, and would make little practical sense. Since DOS
		''' times theoretically go to 2107 - with compatibility not guaranteed
		''' after 2099 - setting this to a time that is before but near 2099
		''' should be sufficient.
		''' </summary>
		Private Shared ReadOnly UPPER_DOSTIME_BOUND As Long = 128L * 365 * 24 * 60 * 60 * 1000

		''' <summary>
		''' Creates a new zip entry with the specified name.
		''' </summary>
		''' <param name="name">
		'''         The entry name
		''' </param>
		''' <exception cref="NullPointerException"> if the entry name is null </exception>
		''' <exception cref="IllegalArgumentException"> if the entry name is longer than
		'''         0xFFFF bytes </exception>
		Public Sub New(ByVal name As String)
			java.util.Objects.requireNonNull(name, "name")
			If name.length() > &HFFFF Then Throw New IllegalArgumentException("entry name too long")
			Me.name = name
		End Sub

		''' <summary>
		''' Creates a new zip entry with fields taken from the specified
		''' zip entry.
		''' </summary>
		''' <param name="e">
		'''         A zip Entry object
		''' </param>
		''' <exception cref="NullPointerException"> if the entry object is null </exception>
		Public Sub New(ByVal e As ZipEntry)
			java.util.Objects.requireNonNull(e, "entry")
			name = e.name
			xdostime = e.xdostime
			mtime = e.mtime
			atime = e.atime
			ctime = e.ctime
			crc = e.crc
			size = e.size
			csize = e.csize
			method = e.method
			flag = e.flag
			extra = e.extra
			comment = e.comment
		End Sub

		''' <summary>
		''' Creates a new un-initialized zip entry
		''' </summary>
		Friend Sub New()
		End Sub

		''' <summary>
		''' Returns the name of the entry. </summary>
		''' <returns> the name of the entry </returns>
		Public Overridable Property name As String
			Get
				Return name
			End Get
		End Property

		''' <summary>
		''' Sets the last modification time of the entry.
		''' 
		''' <p> If the entry is output to a ZIP file or ZIP file formatted
		''' output stream the last modification time set by this method will
		''' be stored into the {@code date and time fields} of the zip file
		''' entry and encoded in standard {@code MS-DOS date and time format}.
		''' The <seealso cref="java.util.TimeZone#getDefault() default TimeZone"/> is
		''' used to convert the epoch time to the MS-DOS data and time.
		''' </summary>
		''' <param name="time">
		'''         The last modification time of the entry in milliseconds
		'''         since the epoch
		''' </param>
		''' <seealso cref= #getTime() </seealso>
		''' <seealso cref= #getLastModifiedTime() </seealso>
		Public Overridable Property time As Long
			Set(ByVal time As Long)
				Me.xdostime = javaToExtendedDosTime(time)
				' Avoid setting the mtime field if time is in the valid
				' range for a DOS time
				If xdostime <> DOSTIME_BEFORE_1980 AndAlso time <= UPPER_DOSTIME_BOUND Then
					Me.mtime = Nothing
				Else
					Me.mtime = java.nio.file.attribute.FileTime.from(time, java.util.concurrent.TimeUnit.MILLISECONDS)
				End If
			End Set
			Get
				If mtime IsNot Nothing Then Return mtime.toMillis()
				Return If(xdostime <> -1, extendedDosToJavaTime(xdostime), -1)
			End Get
		End Property


		''' <summary>
		''' Sets the last modification time of the entry.
		''' 
		''' <p> When output to a ZIP file or ZIP file formatted output stream
		''' the last modification time set by this method will be stored into
		''' zip file entry's {@code date and time fields} in {@code standard
		''' MS-DOS date and time format}), and the extended timestamp fields
		''' in {@code optional extra data} in UTC time.
		''' </summary>
		''' <param name="time">
		'''         The last modification time of the entry </param>
		''' <returns> This zip entry
		''' </returns>
		''' <exception cref="NullPointerException"> if the {@code time} is null
		''' </exception>
		''' <seealso cref= #getLastModifiedTime()
		''' @since 1.8 </seealso>
		Public Overridable Function setLastModifiedTime(ByVal time As java.nio.file.attribute.FileTime) As ZipEntry
			Me.mtime = java.util.Objects.requireNonNull(time, "lastModifiedTime")
			Me.xdostime = javaToExtendedDosTime(time.to(java.util.concurrent.TimeUnit.MILLISECONDS))
			Return Me
		End Function

		''' <summary>
		''' Returns the last modification time of the entry.
		''' 
		''' <p> If the entry is read from a ZIP file or ZIP file formatted
		''' input stream, this is the last modification time from the zip
		''' file entry's {@code optional extra data} if the extended timestamp
		''' fields are present. Otherwise the last modification time is read
		''' from the entry's {@code date and time fields}, the {@link
		''' java.util.TimeZone#getDefault() default TimeZone} is used to convert
		''' the standard MS-DOS formatted date and time to the epoch time.
		''' </summary>
		''' <returns> The last modification time of the entry, null if not specified
		''' </returns>
		''' <seealso cref= #setLastModifiedTime(FileTime)
		''' @since 1.8 </seealso>
		Public Overridable Property lastModifiedTime As java.nio.file.attribute.FileTime
			Get
				If mtime IsNot Nothing Then Return mtime
				If xdostime = -1 Then Return Nothing
				Return java.nio.file.attribute.FileTime.from(time, java.util.concurrent.TimeUnit.MILLISECONDS)
			End Get
		End Property

		''' <summary>
		''' Sets the last access time of the entry.
		''' 
		''' <p> If set, the last access time will be stored into the extended
		''' timestamp fields of entry's {@code optional extra data}, when output
		''' to a ZIP file or ZIP file formatted stream.
		''' </summary>
		''' <param name="time">
		'''         The last access time of the entry </param>
		''' <returns> This zip entry
		''' </returns>
		''' <exception cref="NullPointerException"> if the {@code time} is null
		''' </exception>
		''' <seealso cref= #getLastAccessTime()
		''' @since 1.8 </seealso>
		Public Overridable Function setLastAccessTime(ByVal time As java.nio.file.attribute.FileTime) As ZipEntry
			Me.atime = java.util.Objects.requireNonNull(time, "lastAccessTime")
			Return Me
		End Function

		''' <summary>
		''' Returns the last access time of the entry.
		''' 
		''' <p> The last access time is from the extended timestamp fields
		''' of entry's {@code optional extra data} when read from a ZIP file
		''' or ZIP file formatted stream.
		''' </summary>
		''' <returns> The last access time of the entry, null if not specified
		''' </returns>
		''' <seealso cref= #setLastAccessTime(FileTime)
		''' @since 1.8 </seealso>
		Public Overridable Property lastAccessTime As java.nio.file.attribute.FileTime
			Get
				Return atime
			End Get
		End Property

		''' <summary>
		''' Sets the creation time of the entry.
		''' 
		''' <p> If set, the creation time will be stored into the extended
		''' timestamp fields of entry's {@code optional extra data}, when
		''' output to a ZIP file or ZIP file formatted stream.
		''' </summary>
		''' <param name="time">
		'''         The creation time of the entry </param>
		''' <returns> This zip entry
		''' </returns>
		''' <exception cref="NullPointerException"> if the {@code time} is null
		''' </exception>
		''' <seealso cref= #getCreationTime()
		''' @since 1.8 </seealso>
		Public Overridable Function setCreationTime(ByVal time As java.nio.file.attribute.FileTime) As ZipEntry
			Me.ctime = java.util.Objects.requireNonNull(time, "creationTime")
			Return Me
		End Function

		''' <summary>
		''' Returns the creation time of the entry.
		''' 
		''' <p> The creation time is from the extended timestamp fields of
		''' entry's {@code optional extra data} when read from a ZIP file
		''' or ZIP file formatted stream.
		''' </summary>
		''' <returns> the creation time of the entry, null if not specified </returns>
		''' <seealso cref= #setCreationTime(FileTime)
		''' @since 1.8 </seealso>
		Public Overridable Property creationTime As java.nio.file.attribute.FileTime
			Get
				Return ctime
			End Get
		End Property

		''' <summary>
		''' Sets the uncompressed size of the entry data.
		''' </summary>
		''' <param name="size"> the uncompressed size in bytes
		''' </param>
		''' <exception cref="IllegalArgumentException"> if the specified size is less
		'''         than 0, is greater than 0xFFFFFFFF when
		'''         <a href="package-summary.html#zip64">ZIP64 format</a> is not supported,
		'''         or is less than 0 when ZIP64 is supported </exception>
		''' <seealso cref= #getSize() </seealso>
		Public Overridable Property size As Long
			Set(ByVal size As Long)
				If size < 0 Then Throw New IllegalArgumentException("invalid entry size")
				Me.size = size
			End Set
			Get
				Return size
			End Get
		End Property


		''' <summary>
		''' Returns the size of the compressed entry data.
		''' 
		''' <p> In the case of a stored entry, the compressed size will be the same
		''' as the uncompressed size of the entry.
		''' </summary>
		''' <returns> the size of the compressed entry data, or -1 if not known </returns>
		''' <seealso cref= #setCompressedSize(long) </seealso>
		Public Overridable Property compressedSize As Long
			Get
				Return csize
			End Get
			Set(ByVal csize As Long)
				Me.csize = csize
			End Set
		End Property


		''' <summary>
		''' Sets the CRC-32 checksum of the uncompressed entry data.
		''' </summary>
		''' <param name="crc"> the CRC-32 value
		''' </param>
		''' <exception cref="IllegalArgumentException"> if the specified CRC-32 value is
		'''         less than 0 or greater than 0xFFFFFFFF </exception>
		''' <seealso cref= #getCrc() </seealso>
		Public Overridable Property crc As Long
			Set(ByVal crc As Long)
				If crc < 0 OrElse crc > &HFFFFFFFFL Then Throw New IllegalArgumentException("invalid entry crc-32")
				Me.crc = crc
			End Set
			Get
				Return crc
			End Get
		End Property


		''' <summary>
		''' Sets the compression method for the entry.
		''' </summary>
		''' <param name="method"> the compression method, either STORED or DEFLATED
		''' </param>
		''' <exception cref="IllegalArgumentException"> if the specified compression
		'''          method is invalid </exception>
		''' <seealso cref= #getMethod() </seealso>
		Public Overridable Property method As Integer
			Set(ByVal method As Integer)
				If method <> STORED AndAlso method <> DEFLATED Then Throw New IllegalArgumentException("invalid compression method")
				Me.method = method
			End Set
			Get
				Return method
			End Get
		End Property


		''' <summary>
		''' Sets the optional extra field data for the entry.
		''' 
		''' <p> Invoking this method may change this entry's last modification
		''' time, last access time and creation time, if the {@code extra} field
		''' data includes the extensible timestamp fields, such as {@code NTFS tag
		''' 0x0001} or {@code Info-ZIP Extended Timestamp}, as specified in
		''' <a href="http://www.info-zip.org/doc/appnote-19970311-iz.zip">Info-ZIP
		''' Application Note 970311</a>.
		''' </summary>
		''' <param name="extra">
		'''         The extra field data bytes
		''' </param>
		''' <exception cref="IllegalArgumentException"> if the length of the specified
		'''         extra field data is greater than 0xFFFF bytes
		''' </exception>
		''' <seealso cref= #getExtra() </seealso>
		Public Overridable Property extra As SByte()
			Set(ByVal extra As SByte())
				extra0ra0(extra, False)
			End Set
			Get
				Return extra
			End Get
		End Property

		''' <summary>
		''' Sets the optional extra field data for the entry.
		''' </summary>
		''' <param name="extra">
		'''        the extra field data bytes </param>
		''' <param name="doZIP64">
		'''        if true, set size and csize from ZIP64 fields if present </param>
		Friend Overridable Sub setExtra0(ByVal extra As SByte(), ByVal doZIP64 As Boolean)
			If extra IsNot Nothing Then
				If extra.Length > &HFFFF Then Throw New IllegalArgumentException("invalid extra field length")
				' extra fields are in "HeaderID(2)DataSize(2)Data... format
				Dim [off] As Integer = 0
				Dim len As Integer = extra.Length
				Do While [off] + 4 < len
					Dim tag As Integer = get16(extra, [off])
					Dim sz As Integer = get16(extra, [off] + 2)
					[off] += 4
					If [off] + sz > len Then ' invalid data Exit Do
					Select Case tag
					Case EXTID_ZIP64
						If doZIP64 Then
							' LOC extra zip64 entry MUST include BOTH original
							' and compressed file size fields.
							' If invalid zip64 extra fields, simply skip. Even
							' it's rare, it's possible the entry size happens to
							' be the magic value and it "accidently" has some
							' bytes in extra match the id.
							If sz >= 16 Then
								size = get64(extra, [off])
								csize = get64(extra, [off] + 8)
							End If
						End If
					Case EXTID_NTFS
						If sz < 32 Then ' reserved  4 bytes + tag 2 bytes + size 2 bytes Exit Select ' m[a|c]time 24 bytes
						Dim pos As Integer = [off] + 4 ' reserved 4 bytes
						If get16(extra, pos) <> &H1 OrElse get16(extra, pos + 2) <> 24 Then Exit Select
						mtime = winTimeToFileTime(get64(extra, pos + 4))
						atime = winTimeToFileTime(get64(extra, pos + 12))
						ctime = winTimeToFileTime(get64(extra, pos + 20))
					Case EXTID_EXTT
						Dim flag As Integer = java.lang.[Byte].toUnsignedInt(extra([off]))
						Dim sz0 As Integer = 1
						' The CEN-header extra field contains the modification
						' time only, or no timestamp at all. 'sz' is used to
						' flag its presence or absence. But if mtime is present
						' in LOC it must be present in CEN as well.
						If (flag And &H1) <> 0 AndAlso (sz0 + 4) <= sz Then
							mtime = unixTimeToFileTime(get32(extra, [off] + sz0))
							sz0 += 4
						End If
						If (flag And &H2) <> 0 AndAlso (sz0 + 4) <= sz Then
							atime = unixTimeToFileTime(get32(extra, [off] + sz0))
							sz0 += 4
						End If
						If (flag And &H4) <> 0 AndAlso (sz0 + 4) <= sz Then
							ctime = unixTimeToFileTime(get32(extra, [off] + sz0))
							sz0 += 4
						End If
					 Case Else
					End Select
					[off] += sz
				Loop
			End If
			Me.extra = extra
		End Sub


		''' <summary>
		''' Sets the optional comment string for the entry.
		''' 
		''' <p>ZIP entry comments have maximum length of 0xffff. If the length of the
		''' specified comment string is greater than 0xFFFF bytes after encoding, only
		''' the first 0xFFFF bytes are output to the ZIP file entry.
		''' </summary>
		''' <param name="comment"> the comment string
		''' </param>
		''' <seealso cref= #getComment() </seealso>
		Public Overridable Property comment As String
			Set(ByVal comment As String)
				Me.comment = comment
			End Set
			Get
				Return comment
			End Get
		End Property


		''' <summary>
		''' Returns true if this is a directory entry. A directory entry is
		''' defined to be one whose name ends with a '/'. </summary>
		''' <returns> true if this is a directory entry </returns>
		Public Overridable Property directory As Boolean
			Get
				Return name.EndsWith("/")
			End Get
		End Property

		''' <summary>
		''' Returns a string representation of the ZIP entry.
		''' </summary>
		Public Overrides Function ToString() As String
			Return name
		End Function

		''' <summary>
		''' Returns the hash code value for this entry.
		''' </summary>
		Public Overrides Function GetHashCode() As Integer
			Return name.GetHashCode()
		End Function

		''' <summary>
		''' Returns a copy of this entry.
		''' </summary>
		Public Overridable Function clone() As Object
			Try
				Dim e As ZipEntry = CType(MyBase.clone(), ZipEntry)
				e.extra = If(extra Is Nothing, Nothing, extra.clone())
				Return e
			Catch e As CloneNotSupportedException
				' This should never happen, since we are Cloneable
				Throw New InternalError(e)
			End Try
		End Function
	End Class

End Namespace