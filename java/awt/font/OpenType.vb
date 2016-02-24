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

Namespace java.awt.font

	''' <summary>
	''' The <code>OpenType</code> interface represents OpenType and
	''' TrueType fonts.  This interface makes it possible to obtain
	''' <i>sfnt</i> tables from the font.  A particular
	''' <code>Font</code> object can implement this interface.
	''' <p>
	''' For more information on TrueType and OpenType fonts, see the
	''' OpenType specification.
	''' ( <a href="http://www.microsoft.com/typography/otspec/">http://www.microsoft.com/typography/otspec/</a> ).
	''' </summary>
	Public Interface OpenType

	  ' 51 tag types so far 

	  ''' <summary>
	  ''' Character to glyph mapping.  Table tag "cmap" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_CMAP = &H636d6170;

	  ''' <summary>
	  ''' Font header.  Table tag "head" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_HEAD = &H68656164;

	  ''' <summary>
	  ''' Naming table.  Table tag "name" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_NAME = &H6e616d65;

	  ''' <summary>
	  ''' Glyph data.  Table tag "glyf" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_GLYF = &H676c7966;

	  ''' <summary>
	  ''' Maximum profile.  Table tag "maxp" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_MAXP = &H6d617870;

	  ''' <summary>
	  ''' CVT preprogram.  Table tag "prep" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_PREP = &H70726570;

	  ''' <summary>
	  ''' Horizontal metrics.  Table tag "hmtx" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_HMTX = &H686d7478;

	  ''' <summary>
	  ''' Kerning.  Table tag "kern" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_KERN = &H6b65726e;

	  ''' <summary>
	  ''' Horizontal device metrics.  Table tag "hdmx" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_HDMX = &H68646d78;

	  ''' <summary>
	  ''' Index to location.  Table tag "loca" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_LOCA = &H6c6f6361;

	  ''' <summary>
	  ''' PostScript Information.  Table tag "post" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_POST = &H706f7374;

	  ''' <summary>
	  ''' OS/2 and Windows specific metrics.  Table tag "OS/2"
	  ''' in the Open Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_OS2 = &H4f532f32;

	  ''' <summary>
	  ''' Control value table.  Table tag "cvt "
	  ''' in the Open Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_CVT = &H63767420;

	  ''' <summary>
	  ''' Grid-fitting and scan conversion procedure.  Table tag
	  ''' "gasp" in the Open Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_GASP = &H67617370;

	  ''' <summary>
	  ''' Vertical device metrics.  Table tag "VDMX" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_VDMX = &H56444d58;

	  ''' <summary>
	  ''' Vertical metrics.  Table tag "vmtx" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_VMTX = &H766d7478;

	  ''' <summary>
	  ''' Vertical metrics header.  Table tag "vhea" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_VHEA = &H76686561;

	  ''' <summary>
	  ''' Horizontal metrics header.  Table tag "hhea" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_HHEA = &H68686561;

	  ''' <summary>
	  ''' Adobe Type 1 font data.  Table tag "typ1" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_TYP1 = &H74797031;

	  ''' <summary>
	  ''' Baseline table.  Table tag "bsln" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_BSLN = &H62736c6e;

	  ''' <summary>
	  ''' Glyph substitution.  Table tag "GSUB" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_GSUB = &H47535542;

	  ''' <summary>
	  ''' Digital signature.  Table tag "DSIG" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_DSIG = &H44534947;

	  ''' <summary>
	  ''' Font program.   Table tag "fpgm" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_FPGM = &H6670676d;

	  ''' <summary>
	  ''' Font variation.   Table tag "fvar" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_FVAR = &H66766172;

	  ''' <summary>
	  ''' Glyph variation.  Table tag "gvar" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_GVAR = &H67766172;

	  ''' <summary>
	  ''' Compact font format (Type1 font).  Table tag
	  ''' "CFF " in the Open Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_CFF = &H43464620;

	  ''' <summary>
	  ''' Multiple master supplementary data.  Table tag
	  ''' "MMSD" in the Open Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_MMSD = &H4d4d5344;

	  ''' <summary>
	  ''' Multiple master font metrics.  Table tag
	  ''' "MMFX" in the Open Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_MMFX = &H4d4d4658;

	  ''' <summary>
	  ''' Baseline data.  Table tag "BASE" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_BASE = &H42415345;

	  ''' <summary>
	  ''' Glyph definition.  Table tag "GDEF" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_GDEF = &H47444546;

	  ''' <summary>
	  ''' Glyph positioning.  Table tag "GPOS" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_GPOS = &H47504f53;

	  ''' <summary>
	  ''' Justification.  Table tag "JSTF" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_JSTF = &H4a535446;

	  ''' <summary>
	  ''' Embedded bitmap data.  Table tag "EBDT" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_EBDT = &H45424454;

	  ''' <summary>
	  ''' Embedded bitmap location.  Table tag "EBLC" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_EBLC = &H45424c43;

	  ''' <summary>
	  ''' Embedded bitmap scaling.  Table tag "EBSC" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_EBSC = &H45425343;

	  ''' <summary>
	  ''' Linear threshold.  Table tag "LTSH" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_LTSH = &H4c545348;

	  ''' <summary>
	  ''' PCL 5 data.  Table tag "PCLT" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_PCLT = &H50434c54;

	  ''' <summary>
	  ''' Accent attachment.  Table tag "acnt" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_ACNT = &H61636e74;

	  ''' <summary>
	  ''' Axis variation.  Table tag "avar" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_AVAR = &H61766172;

	  ''' <summary>
	  ''' Bitmap data.  Table tag "bdat" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_BDAT = &H62646174;

	  ''' <summary>
	  ''' Bitmap location.  Table tag "bloc" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_BLOC = &H626c6f63;

	   ''' <summary>
	   ''' CVT variation.  Table tag "cvar" in the Open
	   ''' Type Specification.
	   ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_CVAR = &H63766172;

	  ''' <summary>
	  ''' Feature name.  Table tag "feat" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_FEAT = &H66656174;

	  ''' <summary>
	  ''' Font descriptors.  Table tag "fdsc" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_FDSC = &H66647363;

	  ''' <summary>
	  ''' Font metrics.  Table tag "fmtx" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_FMTX = &H666d7478;

	  ''' <summary>
	  ''' Justification.  Table tag "just" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_JUST = &H6a757374;

	  ''' <summary>
	  ''' Ligature caret.   Table tag "lcar" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_LCAR = &H6c636172;

	  ''' <summary>
	  ''' Glyph metamorphosis.  Table tag "mort" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_MORT = &H6d6f7274;

	  ''' <summary>
	  ''' Optical bounds.  Table tag "opbd" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_OPBD = &H6d6f7274;

	  ''' <summary>
	  ''' Glyph properties.  Table tag "prop" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_PROP = &H70726f70;

	  ''' <summary>
	  ''' Tracking.  Table tag "trak" in the Open
	  ''' Type Specification.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public final static int TAG_TRAK = &H7472616b;

	  ''' <summary>
	  ''' Returns the version of the <code>OpenType</code> font.
	  ''' 1.0 is represented as 0x00010000. </summary>
	  ''' <returns> the version of the <code>OpenType</code> font. </returns>
	  ReadOnly Property version As Integer

	  ''' <summary>
	  ''' Returns the table as an array of bytes for a specified tag.
	  ''' Tags for sfnt tables include items like <i>cmap</i>,
	  ''' <i>name</i> and <i>head</i>.  The <code>byte</code> array
	  ''' returned is a copy of the font data in memory. </summary>
	  ''' <param name="sfntTag"> a four-character code as a 32-bit integer </param>
	  ''' <returns> a <code>byte</code> array that is the table that
	  ''' contains the font data corresponding to the specified
	  ''' tag. </returns>
	  Function getFontTable(ByVal sfntTag As Integer) As SByte()

	  ''' <summary>
	  ''' Returns the table as an array of bytes for a specified tag.
	  ''' Tags for sfnt tables include items like <i>cmap</i>,
	  ''' <i>name</i> and <i>head</i>.  The byte array returned is a
	  ''' copy of the font data in memory. </summary>
	  ''' <param name="strSfntTag"> a four-character code as a
	  '''            <code>String</code> </param>
	  ''' <returns> a <code>byte</code> array that is the table that
	  ''' contains the font data corresponding to the specified
	  ''' tag. </returns>
	  Function getFontTable(ByVal strSfntTag As String) As SByte()

	  ''' <summary>
	  ''' Returns a subset of the table as an array of bytes
	  ''' for a specified tag.  Tags for sfnt tables include
	  ''' items like <i>cmap</i>, <i>name</i> and <i>head</i>.
	  ''' The byte array returned is a copy of the font data in
	  ''' memory. </summary>
	  ''' <param name="sfntTag"> a four-character code as a 32-bit integer </param>
	  ''' <param name="offset"> index of first byte to return from table </param>
	  ''' <param name="count"> number of bytes to return from table </param>
	  ''' <returns> a subset of the table corresponding to
	  '''            <code>sfntTag</code> and containing the bytes
	  '''            starting at <code>offset</code> byte and including
	  '''            <code>count</code> bytes. </returns>
	  Function getFontTable(ByVal sfntTag As Integer, ByVal offset As Integer, ByVal count As Integer) As SByte()

	  ''' <summary>
	  ''' Returns a subset of the table as an array of bytes
	  ''' for a specified tag.  Tags for sfnt tables include items
	  ''' like <i>cmap</i>, <i>name</i> and <i>head</i>. The
	  ''' <code>byte</code> array returned is a copy of the font
	  ''' data in memory. </summary>
	  ''' <param name="strSfntTag"> a four-character code as a
	  ''' <code>String</code> </param>
	  ''' <param name="offset"> index of first byte to return from table </param>
	  ''' <param name="count">  number of bytes to return from table </param>
	  ''' <returns> a subset of the table corresponding to
	  '''            <code>strSfntTag</code> and containing the bytes
	  '''            starting at <code>offset</code> byte and including
	  '''            <code>count</code> bytes. </returns>
	  Function getFontTable(ByVal strSfntTag As String, ByVal offset As Integer, ByVal count As Integer) As SByte()

	  ''' <summary>
	  ''' Returns the size of the table for a specified tag. Tags for sfnt
	  ''' tables include items like <i>cmap</i>, <i>name</i> and <i>head</i>. </summary>
	  ''' <param name="sfntTag"> a four-character code as a 32-bit integer </param>
	  ''' <returns> the size of the table corresponding to the specified
	  ''' tag. </returns>
	  Function getFontTableSize(ByVal sfntTag As Integer) As Integer

	  ''' <summary>
	  ''' Returns the size of the table for a specified tag. Tags for sfnt
	  ''' tables include items like <i>cmap</i>, <i>name</i> and <i>head</i>. </summary>
	  ''' <param name="strSfntTag"> a four-character code as a
	  ''' <code>String</code> </param>
	  ''' <returns> the size of the table corresponding to the specified tag. </returns>
	  Function getFontTableSize(ByVal strSfntTag As String) As Integer


	End Interface

End Namespace