'
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

'
' *
' *
' *
' *
' *
' * Copyright (c) 2000 World Wide Web Consortium,
' * (Massachusetts Institute of Technology, Institut National de
' * Recherche en Informatique et en Automatique, Keio University). All
' * Rights Reserved. This program is distributed under the W3C's Software
' * Intellectual Property License. This program is distributed in the
' * hope that it will be useful, but WITHOUT ANY WARRANTY; without even
' * the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
' * PURPOSE.
' * See W3C License http://www.w3.org/Consortium/Legal/ for more details.
' 

Namespace org.w3c.dom.css


	''' <summary>
	'''  The <code>CSSRule</code> interface is the abstract base interface for any
	''' type of CSS statement. This includes both rule sets and at-rules. An
	''' implementation is expected to preserve all rules specified in a CSS style
	''' sheet, even if the rule is not recognized by the parser. Unrecognized
	''' rules are represented using the <code>CSSUnknownRule</code> interface.
	''' <p>See also the <a href='http://www.w3.org/TR/2000/REC-DOM-Level-2-Style-20001113'>Document Object Model (DOM) Level 2 Style Specification</a>.
	''' @since DOM Level 2
	''' </summary>
	Public Interface CSSRule
		' RuleType
		''' <summary>
		''' The rule is a <code>CSSUnknownRule</code>.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final short UNKNOWN_RULE = 0;
		''' <summary>
		''' The rule is a <code>CSSStyleRule</code>.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final short STYLE_RULE = 1;
		''' <summary>
		''' The rule is a <code>CSSCharsetRule</code>.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final short CHARSET_RULE = 2;
		''' <summary>
		''' The rule is a <code>CSSImportRule</code>.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final short IMPORT_RULE = 3;
		''' <summary>
		''' The rule is a <code>CSSMediaRule</code>.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final short MEDIA_RULE = 4;
		''' <summary>
		''' The rule is a <code>CSSFontFaceRule</code>.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final short FONT_FACE_RULE = 5;
		''' <summary>
		''' The rule is a <code>CSSPageRule</code>.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final short PAGE_RULE = 6;

		''' <summary>
		'''  The type of the rule, as defined above. The expectation is that
		''' binding-specific casting methods can be used to cast down from an
		''' instance of the <code>CSSRule</code> interface to the specific
		''' derived interface implied by the <code>type</code>.
		''' </summary>
		Function [getType]() As Short

		''' <summary>
		'''  The parsable textual representation of the rule. This reflects the
		''' current state of the rule and not its initial value.
		''' </summary>
		Property cssText As String

		''' <summary>
		'''  The style sheet that contains this rule.
		''' </summary>
		ReadOnly Property parentStyleSheet As CSSStyleSheet

		''' <summary>
		'''  If this rule is contained inside another rule (e.g. a style rule
		''' inside an @media block), this is the containing rule. If this rule is
		''' not nested inside any other rules, this returns <code>null</code>.
		''' </summary>
		ReadOnly Property parentRule As CSSRule

	End Interface

End Namespace