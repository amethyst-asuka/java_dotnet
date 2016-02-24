'
' * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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

	'
	' * This class defines the constants that are used by the classes
	' * which manipulate Zip64 files.
	' 

	Friend Class ZipConstants64

	'    
	'     * ZIP64 constants
	'     
		Friend Const ZIP64_ENDSIG As Long = &H6064b50L ' "PK\006\006"
		Friend Const ZIP64_LOCSIG As Long = &H7064b50L ' "PK\006\007"
		Friend Const ZIP64_ENDHDR As Integer = 56 ' ZIP64 end header size
		Friend Const ZIP64_LOCHDR As Integer = 20 ' ZIP64 end loc header size
		Friend Const ZIP64_EXTHDR As Integer = 24 ' EXT header size
		Friend Const ZIP64_EXTID As Integer = &H1 ' Extra field Zip64 header ID

		Friend Const ZIP64_MAGICCOUNT As Integer = &HFFFF
		Friend Const ZIP64_MAGICVAL As Long = &HFFFFFFFFL

	'    
	'     * Zip64 End of central directory (END) header field offsets
	'     
		Friend Const ZIP64_ENDLEN As Integer = 4 ' size of zip64 end of central dir
		Friend Const ZIP64_ENDVEM As Integer = 12 ' version made by
		Friend Const ZIP64_ENDVER As Integer = 14 ' version needed to extract
		Friend Const ZIP64_ENDNMD As Integer = 16 ' number of this disk
		Friend Const ZIP64_ENDDSK As Integer = 20 ' disk number of start
		Friend Const ZIP64_ENDTOD As Integer = 24 ' total number of entries on this disk
		Friend Const ZIP64_ENDTOT As Integer = 32 ' total number of entries
		Friend Const ZIP64_ENDSIZ As Integer = 40 ' central directory size in bytes
		Friend Const ZIP64_ENDOFF As Integer = 48 ' offset of first CEN header
		Friend Const ZIP64_ENDEXT As Integer = 56 ' zip64 extensible data sector

	'    
	'     * Zip64 End of central directory locator field offsets
	'     
		Friend Const ZIP64_LOCDSK As Integer = 4 ' disk number start
		Friend Const ZIP64_LOCOFF As Integer = 8 ' offset of zip64 end
		Friend Const ZIP64_LOCTOT As Integer = 16 ' total number of disks

	'    
	'     * Zip64 Extra local (EXT) header field offsets
	'     
		Friend Const ZIP64_EXTCRC As Integer = 4 ' uncompressed file crc-32 value
		Friend Const ZIP64_EXTSIZ As Integer = 8 ' compressed size, 8-byte
		Friend Const ZIP64_EXTLEN As Integer = 16 ' uncompressed size, 8-byte

	'    
	'     * Language encoding flag EFS
	'     
		Friend Const EFS As Integer = &H800 ' If this bit is set the filename and
											' comment fields for this file must be
											' encoded using UTF-8.

	'    
	'     * Constants below are defined here (instead of in ZipConstants)
	'     * to avoid being exposed as public fields of ZipFile, ZipEntry,
	'     * ZipInputStream and ZipOutputstream.
	'     

	'    
	'     * Extra field header ID
	'     
		Friend Const EXTID_ZIP64 As Integer = &H1 ' Zip64
		Friend Const EXTID_NTFS As Integer = &Ha ' NTFS
		Friend Const EXTID_UNIX As Integer = &Hd ' UNIX
		Friend Const EXTID_EXTT As Integer = &H5455 ' Info-ZIP Extended Timestamp

	'    
	'     * EXTT timestamp flags
	'     
		Friend Const EXTT_FLAG_LMT As Integer = &H1 ' LastModifiedTime
		Friend Const EXTT_FLAG_LAT As Integer = &H2 ' LastAccessTime
		Friend Const EXTT_FLAT_CT As Integer = &H4 ' CreationTime

		Private Sub New()
		End Sub
	End Class

End Namespace