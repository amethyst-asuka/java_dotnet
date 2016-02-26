Imports System
Imports System.Runtime.CompilerServices

'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' The <code>NumericShaper</code> class is used to convert Latin-1 (European)
	''' digits to other Unicode decimal digits.  Users of this class will
	''' primarily be people who wish to present data using
	''' national digit shapes, but find it more convenient to represent the
	''' data internally using Latin-1 (European) digits.  This does not
	''' interpret the deprecated numeric shape selector character (U+206E).
	''' <p>
	''' Instances of <code>NumericShaper</code> are typically applied
	''' as attributes to text with the
	''' <seealso cref="TextAttribute#NUMERIC_SHAPING NUMERIC_SHAPING"/> attribute
	''' of the <code>TextAttribute</code> class.
	''' For example, this code snippet causes a <code>TextLayout</code> to
	''' shape European digits to Arabic in an Arabic context:<br>
	''' <blockquote><pre>
	''' Map map = new HashMap();
	''' map.put(TextAttribute.NUMERIC_SHAPING,
	'''     NumericShaper.getContextualShaper(NumericShaper.ARABIC));
	''' FontRenderContext frc = ...;
	''' TextLayout layout = new TextLayout(text, map, frc);
	''' layout.draw(g2d, x, y);
	''' </pre></blockquote>
	''' <br>
	''' It is also possible to perform numeric shaping explicitly using instances
	''' of <code>NumericShaper</code>, as this code snippet demonstrates:<br>
	''' <blockquote><pre>
	''' char[] text = ...;
	''' // shape all EUROPEAN digits (except zero) to ARABIC digits
	''' NumericShaper shaper = NumericShaper.getShaper(NumericShaper.ARABIC);
	''' shaper.shape(text, start, count);
	''' 
	''' // shape European digits to ARABIC digits if preceding text is Arabic, or
	''' // shape European digits to TAMIL digits if preceding text is Tamil, or
	''' // leave European digits alone if there is no preceding text, or
	''' // preceding text is neither Arabic nor Tamil
	''' NumericShaper shaper =
	'''     NumericShaper.getContextualShaper(NumericShaper.ARABIC |
	'''                                         NumericShaper.TAMIL,
	'''                                       NumericShaper.EUROPEAN);
	''' shaper.shape(text, start, count);
	''' </pre></blockquote>
	''' 
	''' <p><b>Bit mask- and enum-based Unicode ranges</b></p>
	''' 
	''' <p>This class supports two different programming interfaces to
	''' represent Unicode ranges for script-specific digits: bit
	''' mask-based ones, such as <seealso cref="#ARABIC NumericShaper.ARABIC"/>, and
	''' enum-based ones, such as <seealso cref="NumericShaper.Range#ARABIC"/>.
	''' Multiple ranges can be specified by ORing bit mask-based constants,
	''' such as:
	''' <blockquote><pre>
	''' NumericShaper.ARABIC | NumericShaper.TAMIL
	''' </pre></blockquote>
	''' or creating a {@code Set} with the <seealso cref="NumericShaper.Range"/>
	''' constants, such as:
	''' <blockquote><pre>
	''' EnumSet.of(NumericShaper.Scirpt.ARABIC, NumericShaper.Range.TAMIL)
	''' </pre></blockquote>
	''' The enum-based ranges are a super set of the bit mask-based ones.
	''' 
	''' <p>If the two interfaces are mixed (including serialization),
	''' Unicode range values are mapped to their counterparts where such
	''' mapping is possible, such as {@code NumericShaper.Range.ARABIC}
	''' from/to {@code NumericShaper.ARABIC}.  If any unmappable range
	''' values are specified, such as {@code NumericShaper.Range.BALINESE},
	''' those ranges are ignored.
	''' 
	''' <p><b>Decimal Digits Precedence</b></p>
	''' 
	''' <p>A Unicode range may have more than one set of decimal digits. If
	''' multiple decimal digits sets are specified for the same Unicode
	''' range, one of the sets will take precedence as follows.
	''' 
	''' <table border=1 cellspacing=3 cellpadding=0 summary="NumericShaper constants precedence.">
	'''    <tr>
	'''       <th class="TableHeadingColor">Unicode Range</th>
	'''       <th class="TableHeadingColor"><code>NumericShaper</code> Constants</th>
	'''       <th class="TableHeadingColor">Precedence</th>
	'''    </tr>
	'''    <tr>
	'''       <td rowspan="2">Arabic</td>
	'''       <td><seealso cref="NumericShaper#ARABIC NumericShaper.ARABIC"/><br>
	'''           <seealso cref="NumericShaper#EASTERN_ARABIC NumericShaper.EASTERN_ARABIC"/></td>
	'''       <td><seealso cref="NumericShaper#EASTERN_ARABIC NumericShaper.EASTERN_ARABIC"/></td>
	'''    </tr>
	'''    <tr>
	'''       <td><seealso cref="NumericShaper.Range#ARABIC"/><br>
	'''           <seealso cref="NumericShaper.Range#EASTERN_ARABIC"/></td>
	'''       <td><seealso cref="NumericShaper.Range#EASTERN_ARABIC"/></td>
	'''    </tr>
	'''    <tr>
	'''       <td>Tai Tham</td>
	'''       <td><seealso cref="NumericShaper.Range#TAI_THAM_HORA"/><br>
	'''           <seealso cref="NumericShaper.Range#TAI_THAM_THAM"/></td>
	'''       <td><seealso cref="NumericShaper.Range#TAI_THAM_THAM"/></td>
	'''    </tr>
	''' </table>
	''' 
	''' @since 1.4
	''' </summary>

	<Serializable> _
	Public NotInheritable Class NumericShaper
		''' <summary>
		''' A {@code NumericShaper.Range} represents a Unicode range of a
		''' script having its own decimal digits. For example, the {@link
		''' NumericShaper.Range#THAI} range has the Thai digits, THAI DIGIT
		''' ZERO (U+0E50) to THAI DIGIT NINE (U+0E59).
		''' 
		''' <p>The <code>Range</code> enum replaces the traditional bit
		''' mask-based values (e.g., <seealso cref="NumericShaper#ARABIC"/>), and
		''' supports more Unicode ranges than the bit mask-based ones. For
		''' example, the following code using the bit mask:
		''' <blockquote><pre>
		''' NumericShaper.getContextualShaper(NumericShaper.ARABIC |
		'''                                     NumericShaper.TAMIL,
		'''                                   NumericShaper.EUROPEAN);
		''' </pre></blockquote>
		''' can be written using this enum as:
		''' <blockquote><pre>
		''' NumericShaper.getContextualShaper(EnumSet.of(
		'''                                     NumericShaper.Range.ARABIC,
		'''                                     NumericShaper.Range.TAMIL),
		'''                                   NumericShaper.Range.EUROPEAN);
		''' </pre></blockquote>
		''' 
		''' @since 1.7
		''' </summary>
		Public Enum Range
			' The order of EUROPEAN to MOGOLIAN must be consistent
			' with the bitmask-based constants.
			''' <summary>
			''' The Latin (European) range with the Latin (ASCII) digits.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
			EUROPEAN(ChrW(&H0030), ChrW(&H0000), ChrW(&H0300)),
			''' <summary>
			''' The Arabic range with the Arabic-Indic digits.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
			ARABIC(ChrW(&H0660), ChrW(&H0600), ChrW(&H0780)),
			''' <summary>
			''' The Arabic range with the Eastern Arabic-Indic digits.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
			EASTERN_ARABIC(ChrW(&H06f0), ChrW(&H0600), ChrW(&H0780)),
			''' <summary>
			''' The Devanagari range with the Devanagari digits.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
			DEVANAGARI(ChrW(&H0966), ChrW(&H0900), ChrW(&H0980)),
			''' <summary>
			''' The Bengali range with the Bengali digits.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
			BENGALI(ChrW(&H09e6), ChrW(&H0980), ChrW(&H0a00)),
			''' <summary>
			''' The Gurmukhi range with the Gurmukhi digits.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
			GURMUKHI(ChrW(&H0a66), ChrW(&H0a00), ChrW(&H0a80)),
			''' <summary>
			''' The Gujarati range with the Gujarati digits.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
			GUJARATI(ChrW(&H0ae6), ChrW(&H0b00), ChrW(&H0b80)),
			''' <summary>
			''' The Oriya range with the Oriya digits.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
			ORIYA(ChrW(&H0b66), ChrW(&H0b00), ChrW(&H0b80)),
			''' <summary>
			''' The Tamil range with the Tamil digits.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
			TAMIL(ChrW(&H0be6), ChrW(&H0b80), ChrW(&H0c00)),
			''' <summary>
			''' The Telugu range with the Telugu digits.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
			TELUGU(ChrW(&H0c66), ChrW(&H0c00), ChrW(&H0c80)),
			''' <summary>
			''' The Kannada range with the Kannada digits.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
			KANNADA(ChrW(&H0ce6), ChrW(&H0c80), ChrW(&H0d00)),
			''' <summary>
			''' The Malayalam range with the Malayalam digits.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
			MALAYALAM(ChrW(&H0d66), ChrW(&H0d00), ChrW(&H0d80)),
			''' <summary>
			''' The Thai range with the Thai digits.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
			THAI(ChrW(&H0e50), ChrW(&H0e00), ChrW(&H0e80)),
			''' <summary>
			''' The Lao range with the Lao digits.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
			LAO(ChrW(&H0ed0), ChrW(&H0e80), ChrW(&H0f00)),
			''' <summary>
			''' The Tibetan range with the Tibetan digits.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
			TIBETAN(ChrW(&H0f20), ChrW(&H0f00), ChrW(&H1000)),
			''' <summary>
			''' The Myanmar range with the Myanmar digits.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
			MYANMAR(ChrW(&H1040), ChrW(&H1000), ChrW(&H1080)),
			''' <summary>
			''' The Ethiopic range with the Ethiopic digits. Ethiopic
			''' does not have a decimal digit 0 so Latin (European) 0 is
			''' used.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			ETHIOPIC(ChrW(&H1369), ChrW(&H1200), ChrW(&H1380))
	'		{
	'			@Override char getNumericBase() { Return 1;
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'			},
			''' <summary>
			''' The Khmer range with the Khmer digits.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
			KHMER(ChrW(&H17e0), ChrW(&H1780), ChrW(&H1800)), MONGOLIAN(ChrW(&H1810), ChrW(&H1800), ChrW(&H1900)), NKO(ChrW(&H07c0), ChrW(&H07c0), ChrW(&H0800)), MYANMAR_SHAN(ChrW(&H1090), ChrW(&H1000), ChrW(&H10a0)), LIMBU(ChrW(&H1946), ChrW(&H1900), ChrW(&H1950)), NEW_TAI_LUE(ChrW(&H19d0), ChrW(&H1980), ChrW(&H19e0)), BALINESE(ChrW(&H1b50), ChrW(&H1b00), ChrW(&H1b80)), SUNDANESE(ChrW(&H1bb0), ChrW(&H1b80), ChrW(&H1bc0)), LEPCHA(ChrW(&H1c40), ChrW(&H1c00), ChrW(&H1c50)), OL_CHIKI(ChrW(&H1c50), ChrW(&H1c50), ChrW(&H1c80)), VAI(ChrW(&Ha620), ChrW(&Ha500), ChrW(&Ha640)), SAURASHTRA(ChrW(&Ha8R0), ChrW(&Ha880), ChrW(&Ha8e0)), KAYAH_LI(ChrW(&Ha900), ChrW(&Ha900), ChrW(&Ha930)), CHAM(ChrW(&Haa50), ChrW(&Haa00), ChrW(&Haa60)), TAI_THAM_HORA(ChrW(&H1a80), ChrW(&H1a20), ChrW(&H1ab0)), TAI_THAM_THAM(ChrW(&H1a90), ChrW(&H1a20), ChrW(&H1ab0)), JAVANESE(ChrW(&Ha9R0), ChrW(&Ha980), ChrW(&Ha9e0)), MEETEI_MAYEK(ChrW(&Habf0), ChrW(&Habc0), ChrW(&Hac00));
			''' <summary>
			''' The Mongolian range with the Mongolian digits.
			''' </summary>
			' The order of EUROPEAN to MOGOLIAN must be consistent
			' with the bitmask-based constants.
			''' <summary>
			''' The N'Ko range with the N'Ko digits.
			''' </summary>
			''' <summary>
			''' The Myanmar range with the Myanmar Shan digits.
			''' </summary>
			''' <summary>
			''' The Limbu range with the Limbu digits.
			''' </summary>
			''' <summary>
			''' The New Tai Lue range with the New Tai Lue digits.
			''' </summary>
			''' <summary>
			''' The Balinese range with the Balinese digits.
			''' </summary>
			''' <summary>
			''' The Sundanese range with the Sundanese digits.
			''' </summary>
			''' <summary>
			''' The Lepcha range with the Lepcha digits.
			''' </summary>
			''' <summary>
			''' The Ol Chiki range with the Ol Chiki digits.
			''' </summary>
			''' <summary>
			''' The Vai range with the Vai digits.
			''' </summary>
			''' <summary>
			''' The Saurashtra range with the Saurashtra digits.
			''' </summary>
			''' <summary>
			''' The Kayah Li range with the Kayah Li digits.
			''' </summary>
			''' <summary>
			''' The Cham range with the Cham digits.
			''' </summary>
			''' <summary>
			''' The Tai Tham Hora range with the Tai Tham Hora digits.
			''' </summary>
			''' <summary>
			''' The Tai Tham Tham range with the Tai Tham Tham digits.
			''' </summary>
			''' <summary>
			''' The Javanese range with the Javanese digits.
			''' </summary>
			''' <summary>
			''' The Meetei Mayek range with the Meetei Mayek digits.
			''' </summary>

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			private static int toRangeIndex(Range script)
	'		{
	'			int index = script.ordinal();
	'			Return index < NUM_KEYS ? index : -1;
	'		}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			private static Range indexToRange(int index)
	'		{
	'			Return index < NUM_KEYS ? Range.values()[index] : Nothing;
	'		}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			private static int toRangeMask(java.util.Set(Of Range) ranges)
	'		{
	'			int m = 0;
	'			for (Range range : ranges)
	'			{
	'				int index = range.ordinal();
	'				if (index < NUM_KEYS)
	'				{
	'					m |= 1 << index;
	'				}
	'			}
	'			Return m;
	'		}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			private static java.util.Set(Of Range) maskToRangeSet(int mask)
	'		{
	'			Set<Range> set = EnumSet.noneOf(Range.class);
	'			Range[] a = Range.values();
	'			for (int i = 0; i < NUM_KEYS; i += 1)
	'			{
	'				if ((mask & (1 << i)) != 0)
	'				{
	'					set.add(a[i]);
	'				}
	'			}
	'			Return set;
	'		}

			' base character of range digits
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'			private final int base;
			' Unicode range
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'			private final int start, end; ' exclusive -  inclusive

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			private Range(int base, int start, int end)
	'		{
	'			Me.base = base - ("0"c + getNumericBase());
	'			Me.start = start;
	'			Me.end = end;
	'		}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			private int getDigitBase()
	'		{
	'			Return base;
	'		}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			char getNumericBase()
	'		{
	'			Return 0;
	'		}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			private boolean inRange(int c)
	'		{
	'			Return start <= c && c < end;
	'		}
		End Enum

		''' <summary>
		''' index of context for contextual shaping - values range from 0 to 18 </summary>
		Private key As Integer

		''' <summary>
		''' flag indicating whether to shape contextually (high bit) and which
		'''  digit ranges to shape (bits 0-18)
		''' </summary>
		Private mask As Integer

		''' <summary>
		''' The context {@code Range} for contextual shaping or the {@code
		''' Range} for non-contextual shaping. {@code null} for the bit
		''' mask-based API.
		''' 
		''' @since 1.7
		''' </summary>
		Private shapingRange As Range

		''' <summary>
		''' {@code Set<Range>} indicating which Unicode ranges to
		''' shape. {@code null} for the bit mask-based API.
		''' </summary>
		<NonSerialized> _
		Private rangeSet As java.util.Set(Of Range)

		''' <summary>
		''' rangeSet.toArray() value. Sorted by Range.base when the number
		''' of elements is greater then BSEARCH_THRESHOLD.
		''' </summary>
		<NonSerialized> _
		Private rangeArray As Range()

		''' <summary>
		''' If more than BSEARCH_THRESHOLD ranges are specified, binary search is used.
		''' </summary>
		Private Const BSEARCH_THRESHOLD As Integer = 3

		Private Const serialVersionUID As Long = -8022764705923730308L

		''' <summary>
		''' Identifies the Latin-1 (European) and extended range, and
		'''  Latin-1 (European) decimal base.
		''' </summary>
		Public Shared ReadOnly EUROPEAN As Integer = 1<<0

		''' <summary>
		''' Identifies the ARABIC range and decimal base. </summary>
		Public Shared ReadOnly ARABIC As Integer = 1<<1

		''' <summary>
		''' Identifies the ARABIC range and ARABIC_EXTENDED decimal base. </summary>
		Public Shared ReadOnly EASTERN_ARABIC As Integer = 1<<2

		''' <summary>
		''' Identifies the DEVANAGARI range and decimal base. </summary>
		Public Shared ReadOnly DEVANAGARI As Integer = 1<<3

		''' <summary>
		''' Identifies the BENGALI range and decimal base. </summary>
		Public Shared ReadOnly BENGALI As Integer = 1<<4

		''' <summary>
		''' Identifies the GURMUKHI range and decimal base. </summary>
		Public Shared ReadOnly GURMUKHI As Integer = 1<<5

		''' <summary>
		''' Identifies the GUJARATI range and decimal base. </summary>
		Public Shared ReadOnly GUJARATI As Integer = 1<<6

		''' <summary>
		''' Identifies the ORIYA range and decimal base. </summary>
		Public Shared ReadOnly ORIYA As Integer = 1<<7

		''' <summary>
		''' Identifies the TAMIL range and decimal base. </summary>
		' TAMIL DIGIT ZERO was added in Unicode 4.1
		Public Shared ReadOnly TAMIL As Integer = 1<<8

		''' <summary>
		''' Identifies the TELUGU range and decimal base. </summary>
		Public Shared ReadOnly TELUGU As Integer = 1<<9

		''' <summary>
		''' Identifies the KANNADA range and decimal base. </summary>
		Public Shared ReadOnly KANNADA As Integer = 1<<10

		''' <summary>
		''' Identifies the MALAYALAM range and decimal base. </summary>
		Public Shared ReadOnly MALAYALAM As Integer = 1<<11

		''' <summary>
		''' Identifies the THAI range and decimal base. </summary>
		Public Shared ReadOnly THAI As Integer = 1<<12

		''' <summary>
		''' Identifies the LAO range and decimal base. </summary>
		Public Shared ReadOnly LAO As Integer = 1<<13

		''' <summary>
		''' Identifies the TIBETAN range and decimal base. </summary>
		Public Shared ReadOnly TIBETAN As Integer = 1<<14

		''' <summary>
		''' Identifies the MYANMAR range and decimal base. </summary>
		Public Shared ReadOnly MYANMAR As Integer = 1<<15

		''' <summary>
		''' Identifies the ETHIOPIC range and decimal base. </summary>
		Public Shared ReadOnly ETHIOPIC As Integer = 1<<16

		''' <summary>
		''' Identifies the KHMER range and decimal base. </summary>
		Public Shared ReadOnly KHMER As Integer = 1<<17

		''' <summary>
		''' Identifies the MONGOLIAN range and decimal base. </summary>
		Public Shared ReadOnly MONGOLIAN As Integer = 1<<18

		''' <summary>
		''' Identifies all ranges, for full contextual shaping.
		''' 
		''' <p>This constant specifies all of the bit mask-based
		''' ranges. Use {@code EmunSet.allOf(NumericShaper.Range.class)} to
		''' specify all of the enum-based ranges.
		''' </summary>
		Public Const ALL_RANGES As Integer = &H7ffff

		Private Const EUROPEAN_KEY As Integer = 0
		Private Const ARABIC_KEY As Integer = 1
		Private Const EASTERN_ARABIC_KEY As Integer = 2
		Private Const DEVANAGARI_KEY As Integer = 3
		Private Const BENGALI_KEY As Integer = 4
		Private Const GURMUKHI_KEY As Integer = 5
		Private Const GUJARATI_KEY As Integer = 6
		Private Const ORIYA_KEY As Integer = 7
		Private Const TAMIL_KEY As Integer = 8
		Private Const TELUGU_KEY As Integer = 9
		Private Const KANNADA_KEY As Integer = 10
		Private Const MALAYALAM_KEY As Integer = 11
		Private Const THAI_KEY As Integer = 12
		Private Const LAO_KEY As Integer = 13
		Private Const TIBETAN_KEY As Integer = 14
		Private Const MYANMAR_KEY As Integer = 15
		Private Const ETHIOPIC_KEY As Integer = 16
		Private Const KHMER_KEY As Integer = 17
		Private Const MONGOLIAN_KEY As Integer = 18

		Private Shared ReadOnly NUM_KEYS As Integer = MONGOLIAN_KEY + 1 ' fixed

		Private Shared ReadOnly CONTEXTUAL_MASK As Integer = 1<<31

		Private Shared ReadOnly bases As Char() = { ChrW(&H0030) - ChrW(&H0030), ChrW(&H0660) - ChrW(&H0030), ChrW(&H06f0) - ChrW(&H0030), ChrW(&H0966) - ChrW(&H0030), ChrW(&H09e6) - ChrW(&H0030), ChrW(&H0a66) - ChrW(&H0030), ChrW(&H0ae6) - ChrW(&H0030), ChrW(&H0b66) - ChrW(&H0030), ChrW(&H0be6) - ChrW(&H0030), ChrW(&H0c66) - ChrW(&H0030), ChrW(&H0ce6) - ChrW(&H0030), ChrW(&H0d66) - ChrW(&H0030), ChrW(&H0e50) - ChrW(&H0030), ChrW(&H0ed0) - ChrW(&H0030), ChrW(&H0f20) - ChrW(&H0030), ChrW(&H1040) - ChrW(&H0030), ChrW(&H1369) - ChrW(&H0031), ChrW(&H17e0) - ChrW(&H0030), ChrW(&H1810) - ChrW(&H0030) }

		' some ranges adjoin or overlap, rethink if we want to do a binary search on this

		Private Shared ReadOnly contexts As Char() = { ChrW(&H0000), ChrW(&H0300), ChrW(&H0600), ChrW(&H0780), ChrW(&H0600), ChrW(&H0780), ChrW(&H0900), ChrW(&H0980), ChrW(&H0980), ChrW(&H0a00), ChrW(&H0a00), ChrW(&H0a80), ChrW(&H0a80), ChrW(&H0b00), ChrW(&H0b00), ChrW(&H0b80), ChrW(&H0b80), ChrW(&H0c00), ChrW(&H0c00), ChrW(&H0c80), ChrW(&H0c80), ChrW(&H0d00), ChrW(&H0d00), ChrW(&H0d80), ChrW(&H0e00), ChrW(&H0e80), ChrW(&H0e80), ChrW(&H0f00), ChrW(&H0f00), ChrW(&H1000), ChrW(&H1000), ChrW(&H1080), ChrW(&H1200), ChrW(&H1380), ChrW(&H1780), ChrW(&H1800), ChrW(&H1800), ChrW(&H1900), ChrW(&Hffff) }

		' assume most characters are near each other so probing the cache is infrequent,
		' and a linear probe is ok.

		Private Shared ctCache As Integer = 0
		Private Shared ctCacheLimit As Integer = contexts.length - 2

		' warning, synchronize access to this as it modifies state
		Private Shared Function getContextKey(ByVal c As Char) As Integer
			If c < contexts(ctCache) Then
				Do While ctCache > 0 AndAlso c < contexts(ctCache)
					ctCache -= 1
				Loop
			ElseIf c >= contexts(ctCache + 1) Then
				Do While ctCache < ctCacheLimit AndAlso c >= contexts(ctCache + 1)
					ctCache += 1
				Loop
			End If

			' if we're not in a known range, then return EUROPEAN as the range key
			Return If((ctCache And &H1) = 0, (ctCache / 2), EUROPEAN_KEY)
		End Function

		' cache for the NumericShaper.Range version
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private currentRange As Range = Range.EUROPEAN

		Private Function rangeForCodePoint(ByVal codepoint As Integer) As Range
			If currentRange.inRange(codepoint) Then Return currentRange

			Dim ranges As Range() = rangeArray
			If ranges.Length > BSEARCH_THRESHOLD Then
				Dim lo As Integer = 0
				Dim hi As Integer = ranges.Length - 1
				Do While lo <= hi
					Dim mid As Integer = (lo + hi) \ 2
					Dim range_Renamed As Range = ranges(mid)
					If codepoint < range_Renamed.start Then
						hi = mid - 1
					ElseIf codepoint >= range_Renamed.end Then
						lo = mid + 1
					Else
						currentRange = range_Renamed
						Return range_Renamed
					End If
				Loop
			Else
				For i As Integer = 0 To ranges.Length - 1
					If ranges(i).inRange(codepoint) Then Return ranges(i)
				Next i
			End If
			Return Range.EUROPEAN
		End Function

	'    
	'     * A range table of strong directional characters (types L, R, AL).
	'     * Even (left) indexes are starts of ranges of non-strong-directional (or undefined)
	'     * characters, odd (right) indexes are starts of ranges of strong directional
	'     * characters.
	'     
		Private Shared strongTable As Integer() = { &H0, &H41, &H5b, &H61, &H7b, &Haa, &Hab, &Hb5, &Hb6, &Hba, &Hbb, &Hc0, &Hd7, &Hd8, &Hf7, &Hf8, &H2b9, &H2bb, &H2c2, &H2d0, &H2d2, &H2e0, &H2e5, &H2ee, &H2ef, &H370, &H374, &H376, &H37e, &H386, &H387, &H388, &H3f6, &H3f7, &H483, &H48a, &H58a, &H5be, &H5bf, &H5c0, &H5c1, &H5c3, &H5c4, &H5c6, &H5c7, &H5d0, &H600, &H608, &H609, &H60b, &H60c, &H60d, &H60e, &H61b, &H64b, &H66d, &H670, &H671, &H6d6, &H6e5, &H6e7, &H6ee, &H6f0, &H6fa, &H711, &H712, &H730, &H74d, &H7a6, &H7b1, &H7eb, &H7f4, &H7f6, &H7fa, &H816, &H81a, &H81b, &H824, &H825, &H828, &H829, &H830, &H859, &H85e, &H8e4, &H903, &H93a, &H93b, &H93c, &H93d, &H941, &H949, &H94d, &H94e, &H951, &H958, &H962, &H964, &H981, &H982, &H9bc, &H9bd, &H9c1, &H9c7, &H9cd, &H9ce, &H9e2, &H9e6, &H9f2, &H9f4, &H9fb, &Ha03, &Ha3c, &Ha3e, &Ha41, &Ha59, &Ha70, &Ha72, &Ha75, &Ha83, &Habc, &Habd, &Hac1, &Hac9, &Hacd, &Had0, &Hae2, &Hae6, &Haf1, &Hb02, &Hb3c, &Hb3R, &Hb3f, &Hb40, &Hb41, &Hb47, &Hb4R, &Hb57, &Hb62, &Hb66, &Hb82, &Hb83, &Hbc0, &Hbc1, &Hbcd, &Hbd0, &Hbf3, &Hc01, &Hc3e, &Hc41, &Hc46, &Hc58, &Hc62, &Hc66, &Hc78, &Hc7f, &Hcbc, &Hcbd, &Hccc, &Hcd5, &Hce2, &Hce6, &Hd41, &Hd46, &Hd4R, &Hd4e, &Hd62, &Hd66, &Hdca, &Hdcf, &Hdd2, &Hdd8, &He31, &He32, &He34, &He40, &He47, &He4f, &Heb1, &Heb2, &Heb4, &Hebd, &Hec8, &Hed0, &Hf18, &Hf1a, &Hf35, &Hf36, &Hf37, &Hf38, &Hf39, &Hf3e, &Hf71, &Hf7f, &Hf80, &Hf85, &Hf86, &Hf88, &Hf8R, &Hfbe, &Hfc6, &Hfc7, &H102d, &H1031, &H1032, &H1038, &H1039, &H103b, &H103d, &H103f, &H1058, &H105a, &H105e, &H1061, &H1071, &H1075, &H1082, &H1083, &H1085, &H1087, &H108d, &H108e, &H109d, &H109e, &H135d, &H1360, &H1390, &H13a0, &H1400, &H1401, &H1680, &H1681, &H169b, &H16a0, &H1712, &H1720, &H1732, &H1735, &H1752, &H1760, &H1772, &H1780, &H17b4, &H17b6, &H17b7, &H17be, &H17c6, &H17c7, &H17c9, &H17d4, &H17db, &H17dc, &H17dd, &H17e0, &H17f0, &H1810, &H18a9, &H18aa, &H1920, &H1923, &H1927, &H1929, &H1932, &H1933, &H1939, &H1946, &H19de, &H1a00, &H1a17, &H1a19, &H1a56, &H1a57, &H1a58, &H1a61, &H1a62, &H1a63, &H1a65, &H1a6R, &H1a73, &H1a80, &H1b00, &H1b04, &H1b34, &H1b35, &H1b36, &H1b3b, &H1b3c, &H1b3R, &H1b42, &H1b43, &H1b6b, &H1b74, &H1b80, &H1b82, &H1ba2, &H1ba6, &H1ba8, &H1baa, &H1bab, &H1bac, &H1be6, &H1be7, &H1be8, &H1bea, &H1bed, &H1bee, &H1bef, &H1bf2, &H1c2c, &H1c34, &H1c36, &H1c3b, &H1cd0, &H1cd3, &H1cd4, &H1ce1, &H1ce2, &H1ce9, &H1ced, &H1cee, &H1cf4, &H1cf5, &H1dc0, &H1e00, &H1fbd, &H1fbe, &H1fbf, &H1fc2, &H1fcd, &H1fd0, &H1fdd, &H1fe0, &H1fed, &H1ff2, &H1ffd, &H200e, &H2010, &H2071, &H2074, &H207f, &H2080, &H2090, &H20a0, &H2102, &H2103, &H2107, &H2108, &H210a, &H2114, &H2115, &H2116, &H2119, &H211e, &H2124, &H2125, &H2126, &H2127, &H2128, &H2129, &H212a, &H212e, &H212f, &H213a, &H213c, &H2140, &H2145, &H214a, &H214e, &H2150, &H2160, &H2189, &H2336, &H237b, &H2395, &H2396, &H249c, &H24ea, &H26ac, &H26ad, &H2800, &H2900, &H2c00, &H2ce5, &H2ceb, &H2cef, &H2cf2, &H2cf9, &H2d00, &H2d7f, &H2d80, &H2de0, &H3005, &H3008, &H3021, &H302a, &H3031, &H3036, &H3038, &H303d, &H3041, &H3099, &H309d, &H30a0, &H30a1, &H30fb, &H30fc, &H31c0, &H31f0, &H321d, &H3220, &H3250, &H3260, &H327c, &H327f, &H32b1, &H32c0, &H32cc, &H32d0, &H3377, &H337b, &H33de, &H33e0, &H33ff, &H3400, &H4dc0, &H4e00, &Ha490, &Ha4R0, &Ha60R, &Ha610, &Ha66f, &Ha680, &Ha69f, &Ha6a0, &Ha6f0, &Ha6f2, &Ha700, &Ha722, &Ha788, &Ha789, &Ha802, &Ha803, &Ha806, &Ha807, &Ha80b, &Ha80c, &Ha825, &Ha827, &Ha828, &Ha830, &Ha838, &Ha840, &Ha874, &Ha880, &Ha8c4, &Ha8ce, &Ha8e0, &Ha8f2, &Ha926, &Ha92e, &Ha947, &Ha952, &Ha980, &Ha983, &Ha9b3, &Ha9b4, &Ha9b6, &Ha9ba, &Ha9bc, &Ha9bd, &Haa29, &Haa2f, &Haa31, &Haa33, &Haa35, &Haa40, &Haa43, &Haa44, &Haa4c, &Haa4R, &Haab0, &Haab1, &Haab2, &Haab5, &Haab7, &Haab9, &Haabe, &Haac0, &Haac1, &Haac2, &Haaec, &Haaee, &Haaf6, &Hab01, &Habe5, &Habe6, &Habe8, &Habe9, &Habed, &Habf0, &Hfb1e, &Hfb1f, &Hfb29, &Hfb2a, &Hfd3e, &Hfd50, &Hfdfd, &Hfe70, &Hfeff, &Hff21, &Hff3b, &Hff41, &Hff5b, &Hff66, &Hffe0, &H10000, &H10101, &H10102, &H10140, &H101d0, &H101fd, &H10280, &H1091f, &H10920, &H10a01, &H10a10, &H10a38, &H10a40, &H10b39, &H10b40, &H10e60, &H11000, &H11001, &H11002, &H11038, &H11047, &H11052, &H11066, &H11080, &H11082, &H110b3, &H110b7, &H110b9, &H110bb, &H11100, &H11103, &H11127, &H1112c, &H1112d, &H11136, &H11180, &H11182, &H111b6, &H111bf, &H116ab, &H116ac, &H116ad, &H116ae, &H116b0, &H116b6, &H116b7, &H116c0, &H16f8f, &H16f93, &H1d167, &H1d16a, &H1d173, &H1d183, &H1d185, &H1d18c, &H1d1aa, &H1d1ae, &H1d200, &H1d360, &H1d6Rb, &H1d6Rc, &H1d715, &H1d716, &H1d74f, &H1d750, &H1d789, &H1d78a, &H1d7c3, &H1d7c4, &H1d7ce, &H1ee00, &H1eef0, &H1f110, &H1f16a, &H1f170, &H1f300, &H1f48c, &H1f48R, &H1f524, &H1f525, &H20000, &He0001, &Hf0000, &H10fffe, &H10ffff }


		' use a binary search with a cache

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private stCache As Integer = 0

		Private Function isStrongDirectional(ByVal c As Char) As Boolean
			Dim cachedIndex As Integer = stCache
			If c < strongTable(cachedIndex) Then
				cachedIndex = search(c, strongTable, 0, cachedIndex)
			ElseIf c >= strongTable(cachedIndex + 1) Then
				cachedIndex = search(c, strongTable, cachedIndex + 1, strongTable.length - cachedIndex - 1)
			End If
			Dim val As Boolean = (cachedIndex And &H1) = 1
			stCache = cachedIndex
			Return val
		End Function

		Private Shared Function getKeyFromMask(ByVal mask As Integer) As Integer
			Dim key As Integer = 0
			Do While key < NUM_KEYS AndAlso ((mask And (1<<key)) = 0)
				key += 1
			Loop
			If key = NUM_KEYS OrElse ((mask And Not(1<<key)) <> 0) Then Throw New IllegalArgumentException("invalid shaper: " & Integer.toHexString(mask))
			Return key
		End Function

		''' <summary>
		''' Returns a shaper for the provided unicode range.  All
		''' Latin-1 (EUROPEAN) digits are converted
		''' to the corresponding decimal unicode digits. </summary>
		''' <param name="singleRange"> the specified Unicode range </param>
		''' <returns> a non-contextual numeric shaper </returns>
		''' <exception cref="IllegalArgumentException"> if the range is not a single range </exception>
		Public Shared Function getShaper(ByVal singleRange As Integer) As NumericShaper
			Dim key As Integer = getKeyFromMask(singleRange)
			Return New NumericShaper(key, singleRange)
		End Function

		''' <summary>
		''' Returns a shaper for the provided Unicode
		''' range. All Latin-1 (EUROPEAN) digits are converted to the
		''' corresponding decimal digits of the specified Unicode range.
		''' </summary>
		''' <param name="singleRange"> the Unicode range given by a {@link
		'''                    NumericShaper.Range} constant. </param>
		''' <returns> a non-contextual {@code NumericShaper}. </returns>
		''' <exception cref="NullPointerException"> if {@code singleRange} is {@code null}
		''' @since 1.7 </exception>
		Public Shared Function getShaper(ByVal singleRange As Range) As NumericShaper
			Return New NumericShaper(singleRange, java.util.EnumSet.of(singleRange))
		End Function

		''' <summary>
		''' Returns a contextual shaper for the provided unicode range(s).
		''' Latin-1 (EUROPEAN) digits are converted to the decimal digits
		''' corresponding to the range of the preceding text, if the
		''' range is one of the provided ranges.  Multiple ranges are
		''' represented by or-ing the values together, such as,
		''' <code>NumericShaper.ARABIC | NumericShaper.THAI</code>.  The
		''' shaper assumes EUROPEAN as the starting context, that is, if
		''' EUROPEAN digits are encountered before any strong directional
		''' text in the string, the context is presumed to be EUROPEAN, and
		''' so the digits will not shape. </summary>
		''' <param name="ranges"> the specified Unicode ranges </param>
		''' <returns> a shaper for the specified ranges </returns>
		Public Shared Function getContextualShaper(ByVal ranges As Integer) As NumericShaper
			ranges = ranges Or CONTEXTUAL_MASK
			Return New NumericShaper(EUROPEAN_KEY, ranges)
		End Function

		''' <summary>
		''' Returns a contextual shaper for the provided Unicode
		''' range(s). The Latin-1 (EUROPEAN) digits are converted to the
		''' decimal digits corresponding to the range of the preceding
		''' text, if the range is one of the provided ranges.
		''' 
		''' <p>The shaper assumes EUROPEAN as the starting context, that
		''' is, if EUROPEAN digits are encountered before any strong
		''' directional text in the string, the context is presumed to be
		''' EUROPEAN, and so the digits will not shape.
		''' </summary>
		''' <param name="ranges"> the specified Unicode ranges </param>
		''' <returns> a contextual shaper for the specified ranges </returns>
		''' <exception cref="NullPointerException"> if {@code ranges} is {@code null}.
		''' @since 1.7 </exception>
		Public Shared Function getContextualShaper(ByVal ranges As java.util.Set(Of Range)) As NumericShaper
			Dim shaper As New NumericShaper(Range.EUROPEAN, ranges)
			shaper.mask = CONTEXTUAL_MASK
			Return shaper
		End Function

		''' <summary>
		''' Returns a contextual shaper for the provided unicode range(s).
		''' Latin-1 (EUROPEAN) digits will be converted to the decimal digits
		''' corresponding to the range of the preceding text, if the
		''' range is one of the provided ranges.  Multiple ranges are
		''' represented by or-ing the values together, for example,
		''' <code>NumericShaper.ARABIC | NumericShaper.THAI</code>.  The
		''' shaper uses defaultContext as the starting context. </summary>
		''' <param name="ranges"> the specified Unicode ranges </param>
		''' <param name="defaultContext"> the starting context, such as
		''' <code>NumericShaper.EUROPEAN</code> </param>
		''' <returns> a shaper for the specified Unicode ranges. </returns>
		''' <exception cref="IllegalArgumentException"> if the specified
		''' <code>defaultContext</code> is not a single valid range. </exception>
		Public Shared Function getContextualShaper(ByVal ranges As Integer, ByVal defaultContext As Integer) As NumericShaper
			Dim key As Integer = getKeyFromMask(defaultContext)
			ranges = ranges Or CONTEXTUAL_MASK
			Return New NumericShaper(key, ranges)
		End Function

		''' <summary>
		''' Returns a contextual shaper for the provided Unicode range(s).
		''' The Latin-1 (EUROPEAN) digits will be converted to the decimal
		''' digits corresponding to the range of the preceding text, if the
		''' range is one of the provided ranges. The shaper uses {@code
		''' defaultContext} as the starting context.
		''' </summary>
		''' <param name="ranges"> the specified Unicode ranges </param>
		''' <param name="defaultContext"> the starting context, such as
		'''                       {@code NumericShaper.Range.EUROPEAN} </param>
		''' <returns> a contextual shaper for the specified Unicode ranges. </returns>
		''' <exception cref="NullPointerException">
		'''         if {@code ranges} or {@code defaultContext} is {@code null}
		''' @since 1.7 </exception>
		Public Shared Function getContextualShaper(ByVal ranges As java.util.Set(Of Range), ByVal defaultContext As Range) As NumericShaper
			If defaultContext Is Nothing Then Throw New NullPointerException
			Dim shaper As New NumericShaper(defaultContext, ranges)
			shaper.mask = CONTEXTUAL_MASK
			Return shaper
		End Function

		''' <summary>
		''' Private constructor.
		''' </summary>
		Private Sub New(ByVal key As Integer, ByVal mask As Integer)
			Me.key = key
			Me.mask = mask
		End Sub

		Private Sub New(ByVal defaultContext As Range, ByVal ranges As java.util.Set(Of Range))
			shapingRange = defaultContext
			rangeSet = java.util.EnumSet.copyOf(ranges) ' throws NPE if ranges is null.

			' Give precedance to EASTERN_ARABIC if both ARABIC and
			' EASTERN_ARABIC are specified.
			If rangeSet.contains(Range.EASTERN_ARABIC) AndAlso rangeSet.contains(Range.ARABIC) Then rangeSet.remove(Range.ARABIC)

			' As well as the above case, give precedance to TAI_THAM_THAM if both
			' TAI_THAM_HORA and TAI_THAM_THAM are specified.
			If rangeSet.contains(Range.TAI_THAM_THAM) AndAlso rangeSet.contains(Range.TAI_THAM_HORA) Then rangeSet.remove(Range.TAI_THAM_HORA)

			rangeArray = rangeSet.ToArray(New Range(rangeSet.size() - 1){})
			If rangeArray.length > BSEARCH_THRESHOLD Then java.util.Arrays.sort(rangeArray, New ComparatorAnonymousInnerClassHelper(Of T)
		End Sub

		Private Class ComparatorAnonymousInnerClassHelper(Of T)
			Implements IComparer(Of T)

			Public Overridable Function compare(ByVal s1 As Range, ByVal s2 As Range) As Integer
				Return If(s1.base > s2.base, 1, If(s1.base = s2.base, 0, -1))
			End Function
		End Class

		''' <summary>
		''' Converts the digits in the text that occur between start and
		''' start + count. </summary>
		''' <param name="text"> an array of characters to convert </param>
		''' <param name="start"> the index into <code>text</code> to start
		'''        converting </param>
		''' <param name="count"> the number of characters in <code>text</code>
		'''        to convert </param>
		''' <exception cref="IndexOutOfBoundsException"> if start or start + count is
		'''        out of bounds </exception>
		''' <exception cref="NullPointerException"> if text is null </exception>
		Public Sub shape(ByVal text As Char(), ByVal start As Integer, ByVal count As Integer)
			checkParams(text, start, count)
			If contextual Then
				If rangeSet Is Nothing Then
					shapeContextually(text, start, count, key)
				Else
					shapeContextually(text, start, count, shapingRange)
				End If
			Else
				shapeNonContextually(text, start, count)
			End If
		End Sub

		''' <summary>
		''' Converts the digits in the text that occur between start and
		''' start + count, using the provided context.
		''' Context is ignored if the shaper is not a contextual shaper. </summary>
		''' <param name="text"> an array of characters </param>
		''' <param name="start"> the index into <code>text</code> to start
		'''        converting </param>
		''' <param name="count"> the number of characters in <code>text</code>
		'''        to convert </param>
		''' <param name="context"> the context to which to convert the
		'''        characters, such as <code>NumericShaper.EUROPEAN</code> </param>
		''' <exception cref="IndexOutOfBoundsException"> if start or start + count is
		'''        out of bounds </exception>
		''' <exception cref="NullPointerException"> if text is null </exception>
		''' <exception cref="IllegalArgumentException"> if this is a contextual shaper
		''' and the specified <code>context</code> is not a single valid
		''' range. </exception>
		Public Sub shape(ByVal text As Char(), ByVal start As Integer, ByVal count As Integer, ByVal context As Integer)
			checkParams(text, start, count)
			If contextual Then
				Dim ctxKey As Integer = getKeyFromMask(context)
				If rangeSet Is Nothing Then
					shapeContextually(text, start, count, ctxKey)
				Else
					shapeContextually(text, start, count, System.Enum.GetValues(GetType(Range))(ctxKey))
				End If
			Else
				shapeNonContextually(text, start, count)
			End If
		End Sub

		''' <summary>
		''' Converts the digits in the text that occur between {@code
		''' start} and {@code start + count}, using the provided {@code
		''' context}. {@code Context} is ignored if the shaper is not a
		''' contextual shaper.
		''' </summary>
		''' <param name="text">  a {@code char} array </param>
		''' <param name="start"> the index into {@code text} to start converting </param>
		''' <param name="count"> the number of {@code char}s in {@code text}
		'''              to convert </param>
		''' <param name="context"> the context to which to convert the characters,
		'''                such as {@code NumericShaper.Range.EUROPEAN} </param>
		''' <exception cref="IndexOutOfBoundsException">
		'''         if {@code start} or {@code start + count} is out of bounds </exception>
		''' <exception cref="NullPointerException">
		'''         if {@code text} or {@code context} is null
		''' @since 1.7 </exception>
		Public Sub shape(ByVal text As Char(), ByVal start As Integer, ByVal count As Integer, ByVal context As Range)
			checkParams(text, start, count)
			If context Is Nothing Then Throw New NullPointerException("context is null")

			If contextual Then
				If rangeSet IsNot Nothing Then
					shapeContextually(text, start, count, context)
				Else
					Dim key As Integer = Range.toRangeIndex(context)
					If key >= 0 Then
						shapeContextually(text, start, count, key)
					Else
						shapeContextually(text, start, count, shapingRange)
					End If
				End If
			Else
				shapeNonContextually(text, start, count)
			End If
		End Sub

		Private Sub checkParams(ByVal text As Char(), ByVal start As Integer, ByVal count As Integer)
			If text Is Nothing Then Throw New NullPointerException("text is null")
			If (start < 0) OrElse (start > text.Length) OrElse ((start + count) < 0) OrElse ((start + count) > text.Length) Then Throw New IndexOutOfBoundsException("bad start or count for text of length " & text.Length)
		End Sub

		''' <summary>
		''' Returns a <code>boolean</code> indicating whether or not
		''' this shaper shapes contextually. </summary>
		''' <returns> <code>true</code> if this shaper is contextual;
		'''         <code>false</code> otherwise. </returns>
		Public Property contextual As Boolean
			Get
				Return (mask And CONTEXTUAL_MASK) <> 0
			End Get
		End Property

		''' <summary>
		''' Returns an <code>int</code> that ORs together the values for
		''' all the ranges that will be shaped.
		''' <p>
		''' For example, to check if a shaper shapes to Arabic, you would use the
		''' following:
		''' <blockquote>
		'''   {@code if ((shaper.getRanges() & shaper.ARABIC) != 0) &#123; ... }
		''' </blockquote>
		''' 
		''' <p>Note that this method supports only the bit mask-based
		''' ranges. Call <seealso cref="#getRangeSet()"/> for the enum-based ranges.
		''' </summary>
		''' <returns> the values for all the ranges to be shaped. </returns>
		Public Property ranges As Integer
			Get
				Return mask And Not CONTEXTUAL_MASK
			End Get
		End Property

		''' <summary>
		''' Returns a {@code Set} representing all the Unicode ranges in
		''' this {@code NumericShaper} that will be shaped.
		''' </summary>
		''' <returns> all the Unicode ranges to be shaped.
		''' @since 1.7 </returns>
		Public Property rangeSet As java.util.Set(Of Range)
			Get
				If rangeSet IsNot Nothing Then Return java.util.EnumSet.copyOf(rangeSet)
				Return Range.maskToRangeSet(mask)
			End Get
		End Property

		''' <summary>
		''' Perform non-contextual shaping.
		''' </summary>
		Private Sub shapeNonContextually(ByVal text As Char(), ByVal start As Integer, ByVal count As Integer)
			Dim base As Integer
			Dim minDigit As Char = "0"c
			If shapingRange IsNot Nothing Then
				base = shapingRange.digitBase
				AscW(minDigit) += shapingRange.numericBase
			Else
				base = bases(key)
				If key = ETHIOPIC_KEY Then minDigit = ChrW(AscW(minDigit) + 1) ' Ethiopic doesn't use decimal zero
			End If
			Dim i As Integer = start
			Dim e As Integer = start + count
			Do While i < e
				Dim c As Char = text(i)
				If c >= minDigit AndAlso c <= ChrW(&H0039) Then text(i) = CChar(AscW(c) + base)
				i += 1
			Loop
		End Sub

		''' <summary>
		''' Perform contextual shaping.
		''' Synchronized to protect caches used in getContextKey.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Sub shapeContextually(ByVal text As Char(), ByVal start As Integer, ByVal count As Integer, ByVal ctxKey As Integer)

			' if we don't support this context, then don't shape
			If (mask And (1<<ctxKey)) = 0 Then ctxKey = EUROPEAN_KEY
			Dim lastkey As Integer = ctxKey

			Dim base As Integer = bases(ctxKey)
			Dim minDigit As Char = If(ctxKey = ETHIOPIC_KEY, "1"c, "0"c) ' Ethiopic doesn't use decimal zero

			SyncLock GetType(NumericShaper)
				Dim i As Integer = start
				Dim e As Integer = start + count
				Do While i < e
					Dim c As Char = text(i)
					If c >= minDigit AndAlso c <= ChrW(&H0039) Then text(i) = CChar(AscW(c) + base)

					If isStrongDirectional(c) Then
						Dim newkey As Integer = getContextKey(c)
						If newkey <> lastkey Then
							lastkey = newkey

							ctxKey = newkey
							If ((mask And EASTERN_ARABIC) <> 0) AndAlso (ctxKey = ARABIC_KEY OrElse ctxKey = EASTERN_ARABIC_KEY) Then
								ctxKey = EASTERN_ARABIC_KEY
							ElseIf ((mask And ARABIC) <> 0) AndAlso (ctxKey = ARABIC_KEY OrElse ctxKey = EASTERN_ARABIC_KEY) Then
								ctxKey = ARABIC_KEY
							ElseIf (mask And (1<<ctxKey)) = 0 Then
								ctxKey = EUROPEAN_KEY
							End If

							base = bases(ctxKey)

							minDigit = If(ctxKey = ETHIOPIC_KEY, "1"c, "0"c) ' Ethiopic doesn't use decimal zero
						End If
					End If
					i += 1
				Loop
			End SyncLock
		End Sub

		Private Sub shapeContextually(ByVal text As Char(), ByVal start As Integer, ByVal count As Integer, ByVal ctxKey As Range)
			' if we don't support the specified context, then don't shape.
			If ctxKey Is Nothing OrElse (Not rangeSet.contains(ctxKey)) Then ctxKey = Range.EUROPEAN

			Dim lastKey As Range = ctxKey
			Dim base As Integer = ctxKey.digitBase
			Dim minDigit As Char = ChrW(AscW("0"c) + ctxKey.numericBase)
			Dim [end] As Integer = start + count
			For i As Integer = start To [end] - 1
				Dim c As Char = text(i)
				If c >= minDigit AndAlso c <= "9"c Then
					text(i) = CChar(AscW(c) + base)
					Continue For
				End If
				If isStrongDirectional(c) Then
					ctxKey = rangeForCodePoint(c)
					If ctxKey <> lastKey Then
						lastKey = ctxKey
						base = ctxKey.digitBase
						minDigit = ChrW(AscW("0"c) + ctxKey.numericBase)
					End If
				End If
			Next i
		End Sub

		''' <summary>
		''' Returns a hash code for this shaper. </summary>
		''' <returns> this shaper's hash code. </returns>
		''' <seealso cref= java.lang.Object#hashCode </seealso>
		Public Overrides Function GetHashCode() As Integer
			Dim hash As Integer = mask
			If rangeSet IsNot Nothing Then
				' Use the CONTEXTUAL_MASK bit only for the enum-based
				' NumericShaper. A deserialized NumericShaper might have
				' bit masks.
				hash = hash And CONTEXTUAL_MASK
				hash = hash Xor rangeSet.GetHashCode()
			End If
			Return hash
		End Function

		''' <summary>
		''' Returns {@code true} if the specified object is an instance of
		''' <code>NumericShaper</code> and shapes identically to this one,
		''' regardless of the range representations, the bit mask or the
		''' enum. For example, the following code produces {@code "true"}.
		''' <blockquote><pre>
		''' NumericShaper ns1 = NumericShaper.getShaper(NumericShaper.ARABIC);
		''' NumericShaper ns2 = NumericShaper.getShaper(NumericShaper.Range.ARABIC);
		''' System.out.println(ns1.equals(ns2));
		''' </pre></blockquote>
		''' </summary>
		''' <param name="o"> the specified object to compare to this
		'''          <code>NumericShaper</code> </param>
		''' <returns> <code>true</code> if <code>o</code> is an instance
		'''         of <code>NumericShaper</code> and shapes in the same way;
		'''         <code>false</code> otherwise. </returns>
		''' <seealso cref= java.lang.Object#equals(java.lang.Object) </seealso>
		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If o IsNot Nothing Then
				Try
					Dim rhs As NumericShaper = CType(o, NumericShaper)
					If rangeSet IsNot Nothing Then
						If rhs.rangeSet IsNot Nothing Then Return contextual = rhs.contextual AndAlso rangeSet.Equals(rhs.rangeSet) AndAlso shapingRange = rhs.shapingRange
						Return contextual = rhs.contextual AndAlso rangeSet.Equals(Range.maskToRangeSet(rhs.mask)) AndAlso shapingRange = Range.indexToRange(rhs.key)
					ElseIf rhs.rangeSet IsNot Nothing Then
						Dim rset As java.util.Set(Of Range) = Range.maskToRangeSet(mask)
						Dim srange As Range = Range.indexToRange(key)
						Return contextual = rhs.contextual AndAlso rset.Equals(rhs.rangeSet) AndAlso srange = rhs.shapingRange
					End If
					Return rhs.mask = mask AndAlso rhs.key = key
				Catch e As  [Class]CastException
				End Try
			End If
			Return False
		End Function

		''' <summary>
		''' Returns a <code>String</code> that describes this shaper. This method
		''' is used for debugging purposes only. </summary>
		''' <returns> a <code>String</code> describing this shaper. </returns>
		Public Overrides Function ToString() As String
			Dim buf As New StringBuilder(MyBase.ToString())

			buf.append("[contextual:").append(contextual)

			Dim keyNames As String() = Nothing
			If contextual Then
				buf.append(", context:")
				buf.append(If(shapingRange Is Nothing, System.Enum.GetValues(GetType(Range))(key), shapingRange))
			End If

			If rangeSet Is Nothing Then
				buf.append(", range(s): ")
				Dim first As Boolean = True
				For i As Integer = 0 To NUM_KEYS - 1
					If (mask And (1 << i)) <> 0 Then
						If first Then
							first = False
						Else
							buf.append(", ")
						End If
						buf.append(System.Enum.GetValues(GetType(Range))(i))
					End If
				Next i
			Else
				buf.append(", range set: ").append(rangeSet)
			End If
			buf.append("]"c)

			Return buf.ToString()
		End Function

		''' <summary>
		''' Returns the index of the high bit in value (assuming le, actually
		''' power of 2 >= value). value must be positive.
		''' </summary>
		Private Shared Function getHighBit(ByVal value As Integer) As Integer
			If value <= 0 Then Return -32

			Dim bit As Integer = 0

			If value >= 1 << 16 Then
				value >>= 16
				bit += 16
			End If

			If value >= 1 << 8 Then
				value >>= 8
				bit += 8
			End If

			If value >= 1 << 4 Then
				value >>= 4
				bit += 4
			End If

			If value >= 1 << 2 Then
				value >>= 2
				bit += 2
			End If

			If value >= 1 << 1 Then bit += 1

			Return bit
		End Function

		''' <summary>
		''' fast binary search over subrange of array.
		''' </summary>
		Private Shared Function search(ByVal value As Integer, ByVal array As Integer(), ByVal start As Integer, ByVal length As Integer) As Integer
			Dim power As Integer = 1 << getHighBit(length)
			Dim extra As Integer = length - power
			Dim probe As Integer = power
			Dim index As Integer = start

			If value >= array(index + extra) Then index += extra

			Do While probe > 1
				probe >>= 1

				If value >= array(index + probe) Then index += probe
			Loop

			Return index
		End Function

		''' <summary>
		''' Converts the {@code NumericShaper.Range} enum-based parameters,
		''' if any, to the bit mask-based counterparts and writes this
		''' object to the {@code stream}. Any enum constants that have no
		''' bit mask-based counterparts are ignored in the conversion.
		''' </summary>
		''' <param name="stream"> the output stream to write to </param>
		''' <exception cref="IOException"> if an I/O error occurs while writing to {@code stream}
		''' @since 1.7 </exception>
		Private Sub writeObject(ByVal stream As java.io.ObjectOutputStream)
			If shapingRange IsNot Nothing Then
				Dim index As Integer = Range.toRangeIndex(shapingRange)
				If index >= 0 Then key = index
			End If
			If rangeSet IsNot Nothing Then mask = mask Or Range.toRangeMask(rangeSet)
			stream.defaultWriteObject()
		End Sub
	End Class

End Namespace