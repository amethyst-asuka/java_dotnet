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

Namespace org.w3c.dom.ranges

	''' <summary>
	''' Range operations may throw a <code>RangeException</code> as specified in
	''' their method descriptions.
	''' <p>See also the <a href='http://www.w3.org/TR/2000/REC-DOM-Level-2-Traversal-Range-20001113'>Document Object Model (DOM) Level 2 Traversal and Range Specification</a>.
	''' @since DOM Level 2
	''' </summary>
	Public Class RangeException
		Inherits Exception

		Public Sub New(ByVal code As Short, ByVal message As String)
		   MyBase.New(message)
		   Me.code = code
		End Sub
		Public code As Short
		' RangeExceptionCode
		''' <summary>
		''' If the boundary-points of a Range do not meet specific requirements.
		''' </summary>
		Public Const BAD_BOUNDARYPOINTS_ERR As Short = 1
		''' <summary>
		''' If the container of an boundary-point of a Range is being set to either
		''' a node of an invalid type or a node with an ancestor of an invalid
		''' type.
		''' </summary>
		Public Const INVALID_NODE_TYPE_ERR As Short = 2

	End Class

End Namespace