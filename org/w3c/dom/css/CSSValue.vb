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
	'''  The <code>CSSValue</code> interface represents a simple or a complex
	''' value. A <code>CSSValue</code> object only occurs in a context of a CSS
	''' property.
	''' <p>See also the <a href='http://www.w3.org/TR/2000/REC-DOM-Level-2-Style-20001113'>Document Object Model (DOM) Level 2 Style Specification</a>.
	''' @since DOM Level 2
	''' </summary>
	Public Interface CSSValue
		' UnitTypes
		''' <summary>
		''' The value is inherited and the <code>cssText</code> contains "inherit".
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final short CSS_INHERIT = 0;
		''' <summary>
		''' The value is a primitive value and an instance of the
		''' <code>CSSPrimitiveValue</code> interface can be obtained by using
		''' binding-specific casting methods on this instance of the
		''' <code>CSSValue</code> interface.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final short CSS_PRIMITIVE_VALUE = 1;
		''' <summary>
		''' The value is a <code>CSSValue</code> list and an instance of the
		''' <code>CSSValueList</code> interface can be obtained by using
		''' binding-specific casting methods on this instance of the
		''' <code>CSSValue</code> interface.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final short CSS_VALUE_LIST = 2;
		''' <summary>
		''' The value is a custom value.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final short CSS_CUSTOM = 3;

		''' <summary>
		'''  A string representation of the current value.
		''' </summary>
		Property cssText As String

		''' <summary>
		'''  A code defining the type of the value as defined above.
		''' </summary>
		ReadOnly Property cssValueType As Short

	End Interface

End Namespace