'
' * Copyright (c) 2011, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.util.regex


	Friend Enum UnicodeProp

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		ALPHABETIC
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public boolean is(int ch)
	'		{
	'			Return Character.isAlphabetic(ch);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		},

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		LETTER
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public boolean is(int ch)
	'		{
	'			Return Character.isLetter(ch);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		},

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		IDEOGRAPHIC
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public boolean is(int ch)
	'		{
	'			Return Character.isIdeographic(ch);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		},

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		LOWERCASE
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public boolean is(int ch)
	'		{
	'			Return Character.isLowerCase(ch);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		},

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		UPPERCASE
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public boolean is(int ch)
	'		{
	'			Return Character.isUpperCase(ch);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		},

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		TITLECASE
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public boolean is(int ch)
	'		{
	'			Return Character.isTitleCase(ch);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		},

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		WHITE_SPACE
			' \p{Whitespace}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public boolean is(int ch)
	'		{
	'			Return ((((1 << Character.SPACE_SEPARATOR) | (1 << Character.LINE_SEPARATOR) | (1 << Character.PARAGRAPH_SEPARATOR)) >> Character.getType(ch)) & 1) != 0 || (ch >= &H9 && ch <= &Hd) || (ch == &H85);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		},

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		CONTROL
			' \p{gc=Control}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public boolean is(int ch)
	'		{
	'			Return Character.getType(ch) == Character.CONTROL;
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		},

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		PUNCTUATION
			' \p{gc=Punctuation}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public boolean is(int ch)
	'		{
	'			Return ((((1 << Character.CONNECTOR_PUNCTUATION) | (1 << Character.DASH_PUNCTUATION) | (1 << Character.START_PUNCTUATION) | (1 << Character.END_PUNCTUATION) | (1 << Character.OTHER_PUNCTUATION) | (1 << Character.INITIAL_QUOTE_PUNCTUATION) | (1 << Character.FINAL_QUOTE_PUNCTUATION)) >> Character.getType(ch)) & 1) != 0;
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		},

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		HEX_DIGIT
			' \p{gc=Decimal_Number}
			' \p{Hex_Digit}    -> PropList.txt: Hex_Digit
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public boolean is(int ch)
	'		{
	'			Return DIGIT.is(ch) || (ch >= &H30 && ch <= &H39) || (ch >= &H41 && ch <= &H46) || (ch >= &H61 && ch <= &H66) || (ch >= &HFF10 && ch <= &HFF19) || (ch >= &HFF21 && ch <= &HFF26) || (ch >= &HFF41 && ch <= &HFF46);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		},

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		ASSIGNED
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public boolean is(int ch)
	'		{
	'			Return Character.getType(ch) != Character.UNASSIGNED;
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		},

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		NONCHARACTER_CODE_POINT
			' PropList.txt:Noncharacter_Code_Point
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public boolean is(int ch)
	'		{
	'			Return (ch & &Hfffe) == &Hfffe || (ch >= &Hfdd0 && ch <= &Hfdef);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		},

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		DIGIT
			' \p{gc=Decimal_Number}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public boolean is(int ch)
	'		{
	'			Return Character.isDigit(ch);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		},

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		ALNUM
			' \p{alpha}
			' \p{digit}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public boolean is(int ch)
	'		{
	'			Return ALPHABETIC.is(ch) || DIGIT.is(ch);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		},

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		BLANK
			' \p{Whitespace} --
			' [\N{LF} \N{VT} \N{FF} \N{CR} \N{NEL}  -> 0xa, 0xb, 0xc, 0xd, 0x85
			'  \p{gc=Line_Separator}
			'  \p{gc=Paragraph_Separator}]
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public boolean is(int ch)
	'		{
	'			Return Character.getType(ch) == Character.SPACE_SEPARATOR || ch == &H9; ' \N{HT}
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		},

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		GRAPH
			' [^
			'  \p{space}
			'  \p{gc=Control}
			'  \p{gc=Surrogate}
			'  \p{gc=Unassigned}]
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public boolean is(int ch)
	'		{
	'			Return ((((1 << Character.SPACE_SEPARATOR) | (1 << Character.LINE_SEPARATOR) | (1 << Character.PARAGRAPH_SEPARATOR) | (1 << Character.CONTROL) | (1 << Character.SURROGATE) | (1 << Character.UNASSIGNED)) >> Character.getType(ch)) & 1) == 0;
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		},

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		PRINT
			' \p{graph}
			' \p{blank}
			' -- \p{cntrl}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public boolean is(int ch)
	'		{
	'			Return (GRAPH.is(ch) || BLANK.is(ch)) && !CONTROL.is(ch);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		},

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		WORD
			'  \p{alpha}
			'  \p{gc=Mark}
			'  \p{digit}
			'  \p{gc=Connector_Punctuation}
			'  \p{Join_Control}    200C..200D

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public boolean is(int ch)
	'		{
	'			Return ALPHABETIC.is(ch) || ((((1 << Character.NON_SPACING_MARK) | (1 << Character.ENCLOSING_MARK) | (1 << Character.COMBINING_SPACING_MARK) | (1 << Character.DECIMAL_DIGIT_NUMBER) | (1 << Character.CONNECTOR_PUNCTUATION)) >> Character.getType(ch)) & 1) != 0 || JOIN_CONTROL.is(ch);
	'		}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		},

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		JOIN_CONTROL
			'  200C..200D    PropList.txt:Join_Control
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public boolean is(int ch)
	'		{
	'		   Return (ch == &H200C || ch == &H200D);
	'		}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		private final static java.util.HashMap(Of String, String) posix = New java.util.HashMap(Of )();
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		private final static java.util.HashMap(Of String, String) aliases = New java.util.HashMap(Of )();
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		static ImpliedClass()
	'	{
	'		posix.put("ALPHA", "ALPHABETIC");
	'		posix.put("LOWER", "LOWERCASE");
	'		posix.put("UPPER", "UPPERCASE");
	'		posix.put("SPACE", "WHITE_SPACE");
	'		posix.put("PUNCT", "PUNCTUATION");
	'		posix.put("XDIGIT","HEX_DIGIT");
	'		posix.put("ALNUM", "ALNUM");
	'		posix.put("CNTRL", "CONTROL");
	'		posix.put("DIGIT", "DIGIT");
	'		posix.put("BLANK", "BLANK");
	'		posix.put("GRAPH", "GRAPH");
	'		posix.put("PRINT", "PRINT");
	'
	'		aliases.put("WHITESPACE", "WHITE_SPACE");
	'		aliases.put("HEXDIGIT","HEX_DIGIT");
	'		aliases.put("NONCHARACTERCODEPOINT", "NONCHARACTER_CODE_POINT");
	'		aliases.put("JOINCONTROL", "JOIN_CONTROL");
	'	}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public static UnicodeProp forName(String propName)
	'	{
	'		propName = propName.toUpperCase(Locale.ENGLISH);
	'		String alias = aliases.get(propName);
	'		if (alias != Nothing)
	'			propName = alias;
	'		try
	'		{
	'			Return valueOf(propName);
	'		}
	'		catch (IllegalArgumentException x)
	'		{
	'		}
	'		Return Nothing;
	'	}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public static UnicodeProp forPOSIXName(String propName)
	'	{
	'		propName = posix.get(propName.toUpperCase(Locale.ENGLISH));
	'		if (propName == Nothing)
	'			Return Nothing;
	'		Return valueOf(propName);
	'	}

		[public] = int ch

End Namespace