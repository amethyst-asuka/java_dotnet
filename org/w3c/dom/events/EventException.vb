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

Namespace org.w3c.dom.events

	''' <summary>
	'''  Event operations may throw an <code>EventException</code> as specified in
	''' their method descriptions.
	''' <p>See also the <a href='http://www.w3.org/TR/2000/REC-DOM-Level-2-Events-20001113'>Document Object Model (DOM) Level 2 Events Specification</a>.
	''' @since DOM Level 2
	''' </summary>
	Public Class EventException
		Inherits Exception

		Public Sub New(ByVal code As Short, ByVal message As String)
		   MyBase.New(message)
		   Me.code = code
		End Sub
		Public code As Short
		' EventExceptionCode
		''' <summary>
		'''  If the <code>Event</code>'s type was not specified by initializing the
		''' event before the method was called. Specification of the Event's type
		''' as <code>null</code> or an empty string will also trigger this
		''' exception.
		''' </summary>
		Public Const UNSPECIFIED_EVENT_TYPE_ERR As Short = 0

	End Class

End Namespace