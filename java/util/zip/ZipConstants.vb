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
	' * This interface defines the constants that are used by the classes
	' * which manipulate ZIP files.
	' *
	' * @author      David Connelly
	' 
	Friend Interface ZipConstants
	'    
	'     * Header signatures
	'     
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static long LOCSIG = &H4034b50L; ' "PK\003\004"
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static long EXTSIG = &H8074b50L; ' "PK\007\008"
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static long CENSIG = &H2014b50L; ' "PK\001\002"
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static long ENDSIG = &H6054b50L; ' "PK\005\006"

	'    
	'     * Header sizes in bytes (including signatures)
	'     
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int LOCHDR = 30; ' LOC header size
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int EXTHDR = 16; ' EXT header size
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int CENHDR = 46; ' CEN header size
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int ENDHDR = 22; ' END header size

	'    
	'     * Local file (LOC) header field offsets
	'     
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int LOCVER = 4; ' version needed to extract
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int LOCFLG = 6; ' general purpose bit flag
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int LOCHOW = 8; ' compression method
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int LOCTIM = 10; ' modification time
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int LOCCRC = 14; ' uncompressed file crc-32 value
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int LOCSIZ = 18; ' compressed size
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int LOCLEN = 22; ' uncompressed size
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int LOCNAM = 26; ' filename length
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int LOCEXT = 28; ' extra field length

	'    
	'     * Extra local (EXT) header field offsets
	'     
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int EXTCRC = 4; ' uncompressed file crc-32 value
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int EXTSIZ = 8; ' compressed size
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int EXTLEN = 12; ' uncompressed size

	'    
	'     * Central directory (CEN) header field offsets
	'     
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int CENVEM = 4; ' version made by
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int CENVER = 6; ' version needed to extract
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int CENFLG = 8; ' encrypt, decrypt flags
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int CENHOW = 10; ' compression method
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int CENTIM = 12; ' modification time
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int CENCRC = 16; ' uncompressed file crc-32 value
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int CENSIZ = 20; ' compressed size
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int CENLEN = 24; ' uncompressed size
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int CENNAM = 28; ' filename length
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int CENEXT = 30; ' extra field length
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int CENCOM = 32; ' comment length
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int CENDSK = 34; ' disk number start
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int CENATT = 36; ' internal file attributes
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int CENATX = 38; ' external file attributes
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int CENOFF = 42; ' LOC header offset

	'    
	'     * End of central directory (END) header field offsets
	'     
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int ENDSUB = 8; ' number of entries on this disk
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int ENDTOT = 10; ' total number of entries
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int ENDSIZ = 12; ' central directory size in bytes
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int ENDOFF = 16; ' offset of first CEN header
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final int ENDCOM = 20; ' zip file comment length
	End Interface

End Namespace