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
' * Copyright (c) 2004 World Wide Web Consortium,
' *
' * (Massachusetts Institute of Technology, European Research Consortium for
' * Informatics and Mathematics, Keio University). All Rights Reserved. This
' * work is distributed under the W3C(r) Software License [1] in the hope that
' * it will be useful, but WITHOUT ANY WARRANTY; without even the implied
' * warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
' *
' * [1] http://www.w3.org/Consortium/Legal/2002/copyright-software-20021231
' 

Namespace org.w3c.dom

	''' <summary>
	''' <code>DOMError</code> is an interface that describes an error.
	''' <p>See also the <a href='http://www.w3.org/TR/2004/REC-DOM-Level-3-Core-20040407'>Document Object Model (DOM) Level 3 Core Specification</a>.
	''' @since DOM Level 3
	''' </summary>
	Public Interface DOMError
		' ErrorSeverity
		''' <summary>
		''' The severity of the error described by the <code>DOMError</code> is
		''' warning. A <code>SEVERITY_WARNING</code> will not cause the
		''' processing to stop, unless <code>DOMErrorHandler.handleError()</code>
		''' returns <code>false</code>.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final short SEVERITY_WARNING = 1;
		''' <summary>
		''' The severity of the error described by the <code>DOMError</code> is
		''' error. A <code>SEVERITY_ERROR</code> may not cause the processing to
		''' stop if the error can be recovered, unless
		''' <code>DOMErrorHandler.handleError()</code> returns <code>false</code>.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final short SEVERITY_ERROR = 2;
		''' <summary>
		''' The severity of the error described by the <code>DOMError</code> is
		''' fatal error. A <code>SEVERITY_FATAL_ERROR</code> will cause the
		''' normal processing to stop. The return value of
		''' <code>DOMErrorHandler.handleError()</code> is ignored unless the
		''' implementation chooses to continue, in which case the behavior
		''' becomes undefined.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final short SEVERITY_FATAL_ERROR = 3;

		''' <summary>
		''' The severity of the error, either <code>SEVERITY_WARNING</code>,
		''' <code>SEVERITY_ERROR</code>, or <code>SEVERITY_FATAL_ERROR</code>.
		''' </summary>
		ReadOnly Property severity As Short

		''' <summary>
		''' An implementation specific string describing the error that occurred.
		''' </summary>
		ReadOnly Property message As String

		''' <summary>
		'''  A <code>DOMString</code> indicating which related data is expected in
		''' <code>relatedData</code>. Users should refer to the specification of
		''' the error in order to find its <code>DOMString</code> type and
		''' <code>relatedData</code> definitions if any.
		''' <p ><b>Note:</b>  As an example,
		''' <code>Document.normalizeDocument()</code> does generate warnings when
		''' the "split-cdata-sections" parameter is in use. Therefore, the method
		''' generates a <code>SEVERITY_WARNING</code> with <code>type</code>
		''' <code>"cdata-sections-splitted"</code> and the first
		''' <code>CDATASection</code> node in document order resulting from the
		''' split is returned by the <code>relatedData</code> attribute.
		''' </summary>
		Function [getType]() As String

		''' <summary>
		''' The related platform dependent exception if any.
		''' </summary>
		ReadOnly Property relatedException As Object

		''' <summary>
		'''  The related <code>DOMError.type</code> dependent data if any.
		''' </summary>
		ReadOnly Property relatedData As Object

		''' <summary>
		''' The location of the error.
		''' </summary>
		ReadOnly Property location As DOMLocator

	End Interface

End Namespace