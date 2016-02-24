Imports System

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
' * Copyright (c) 2002 World Wide Web Consortium,
' * (Massachusetts Institute of Technology, Institut National de
' * Recherche en Informatique et en Automatique, Keio University). All
' * Rights Reserved. This program is distributed under the W3C's Software
' * Intellectual Property License. This program is distributed in the
' * hope that it will be useful, but WITHOUT ANY WARRANTY; without even
' * the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
' * PURPOSE.
' * See W3C License http://www.w3.org/Consortium/Legal/ for more details.
' 

Namespace org.w3c.dom.xpath

	''' <summary>
	''' A new exception has been created for exceptions specific to these XPath
	''' interfaces.
	''' <p>See also the <a href='http://www.w3.org/2002/08/WD-DOM-Level-3-XPath-20020820'>Document Object Model (DOM) Level 3 XPath Specification</a>.
	''' </summary>
	Public Class XPathException
		Inherits Exception

		Public Sub New(ByVal code As Short, ByVal message As String)
		   MyBase.New(message)
		   Me.code = code
		End Sub
		Public code As Short
		' XPathExceptionCode
		''' <summary>
		''' If the expression has a syntax error or otherwise is not a legal
		''' expression according to the rules of the specific
		''' <code>XPathEvaluator</code> or contains specialized extension
		''' functions or variables not supported by this implementation.
		''' </summary>
		Public Const INVALID_EXPRESSION_ERR As Short = 1
		''' <summary>
		''' If the expression cannot be converted to return the specified type.
		''' </summary>
		Public Const TYPE_ERR As Short = 2

	End Class

End Namespace