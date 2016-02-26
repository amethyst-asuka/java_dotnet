Imports javax.swing.text

'
' * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.text.html


	''' <summary>
	''' FormSubmitEvent is used to notify interested
	''' parties that a form was submitted.
	''' 
	''' @since 1.5
	''' @author    Denis Sharypov
	''' </summary>

	Public Class FormSubmitEvent
		Inherits HTMLFrameHyperlinkEvent

		''' <summary>
		''' Represents an HTML form method type.
		''' <UL>
		''' <LI><code>GET</code> corresponds to the GET form method</LI>
		''' <LI><code>POST</code> corresponds to the POST from method</LI>
		''' </UL>
		''' @since 1.5
		''' </summary>
		Public Enum MethodType
			[GET]
			POST
		End Enum

		''' <summary>
		''' Creates a new object representing an html form submit event.
		''' </summary>
		''' <param name="source"> the object responsible for the event </param>
		''' <param name="type"> the event type </param>
		''' <param name="actionURL"> the form action URL </param>
		''' <param name="sourceElement"> the element that corresponds to the source
		'''                      of the event </param>
		''' <param name="targetFrame"> the Frame to display the document in </param>
		''' <param name="method"> the form method type </param>
		''' <param name="data"> the form submission data </param>
		Friend Sub New(ByVal source As Object, ByVal type As EventType, ByVal targetURL As java.net.URL, ByVal sourceElement As Element, ByVal targetFrame As String, ByVal method As MethodType, ByVal data As String)
			MyBase.New(source, type, targetURL, sourceElement, targetFrame)
			Me.method = method
			Me.data = data
		End Sub


		''' <summary>
		''' Gets the form method type.
		''' </summary>
		''' <returns> the form method type, either
		''' <code>Method.GET</code> or <code>Method.POST</code>. </returns>
		Public Overridable Property method As MethodType
			Get
				Return method
			End Get
		End Property

		''' <summary>
		''' Gets the form submission data.
		''' </summary>
		''' <returns> the string representing the form submission data. </returns>
		Public Overridable Property data As String
			Get
				Return data
			End Get
		End Property

		Private method As MethodType
		Private data As String
	End Class

End Namespace