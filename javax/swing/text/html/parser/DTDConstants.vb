'
' * Copyright (c) 1998, 1999, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.text.html.parser

	''' <summary>
	''' SGML constants used in a DTD. The names of the
	''' constants correspond the the equivalent SGML constructs
	''' as described in "The SGML Handbook" by  Charles F. Goldfarb.
	''' </summary>
	''' <seealso cref= DTD </seealso>
	''' <seealso cref= Element
	''' @author Arthur van Hoff </seealso>
	Public Interface DTDConstants
		' Attribute value types
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int CDATA = 1;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int ENTITY = 2;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int ENTITIES = 3;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int ID = 4;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int IDREF = 5;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int IDREFS = 6;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int NAME = 7;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int NAMES = 8;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int NMTOKEN = 9;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int NMTOKENS = 10;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int NOTATION = 11;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int NUMBER = 12;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int NUMBERS = 13;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int NUTOKEN = 14;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int NUTOKENS = 15;

		' Content model types
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int RCDATA = 16;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int EMPTY = 17;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int MODEL = 18;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int ANY = 19;

		' Attribute value modifiers
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int FIXED = 1;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int REQUIRED = 2;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int CURRENT = 3;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int CONREF = 4;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int IMPLIED = 5;

		' Entity types
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int PUBLIC = 10;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int SDATA = 11;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int PI = 12;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int STARTTAG = 13;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int ENDTAG = 14;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int MS = 15;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int MD = 16;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int SYSTEM = 17;

'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int GENERAL = 1<<16;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int DEFAULT = 1<<17;
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int PARAMETER = 1<<18;
	End Interface

End Namespace