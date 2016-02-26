Imports javax.swing.text

'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' HTMLFrameHyperlinkEvent is used to notify interested
	''' parties that link was activated in a frame.
	''' 
	''' @author Sunita Mani
	''' </summary>

	Public Class HTMLFrameHyperlinkEvent
		Inherits javax.swing.event.HyperlinkEvent

		''' <summary>
		''' Creates a new object representing a html frame
		''' hypertext link event.
		''' </summary>
		''' <param name="source"> the object responsible for the event </param>
		''' <param name="type"> the event type </param>
		''' <param name="targetURL"> the affected URL </param>
		''' <param name="targetFrame"> the Frame to display the document in </param>
		Public Sub New(ByVal source As Object, ByVal type As EventType, ByVal targetURL As java.net.URL, ByVal targetFrame As String)
			MyBase.New(source, type, targetURL)
			Me.targetFrame = targetFrame
		End Sub


		''' <summary>
		''' Creates a new object representing a hypertext link event.
		''' </summary>
		''' <param name="source"> the object responsible for the event </param>
		''' <param name="type"> the event type </param>
		''' <param name="targetURL"> the affected URL </param>
		''' <param name="desc"> a description </param>
		''' <param name="targetFrame"> the Frame to display the document in </param>
		Public Sub New(ByVal source As Object, ByVal type As EventType, ByVal targetURL As java.net.URL, ByVal desc As String, ByVal targetFrame As String)
			MyBase.New(source, type, targetURL, desc)
			Me.targetFrame = targetFrame
		End Sub

		''' <summary>
		''' Creates a new object representing a hypertext link event.
		''' </summary>
		''' <param name="source"> the object responsible for the event </param>
		''' <param name="type"> the event type </param>
		''' <param name="targetURL"> the affected URL </param>
		''' <param name="sourceElement"> the element that corresponds to the source
		'''                      of the event </param>
		''' <param name="targetFrame"> the Frame to display the document in </param>
		Public Sub New(ByVal source As Object, ByVal type As EventType, ByVal targetURL As java.net.URL, ByVal sourceElement As Element, ByVal targetFrame As String)
			MyBase.New(source, type, targetURL, Nothing, sourceElement)
			Me.targetFrame = targetFrame
		End Sub


		''' <summary>
		''' Creates a new object representing a hypertext link event.
		''' </summary>
		''' <param name="source"> the object responsible for the event </param>
		''' <param name="type"> the event type </param>
		''' <param name="targetURL"> the affected URL </param>
		''' <param name="desc"> a description </param>
		''' <param name="sourceElement"> the element that corresponds to the source
		'''                      of the event </param>
		''' <param name="targetFrame"> the Frame to display the document in </param>
		Public Sub New(ByVal source As Object, ByVal type As EventType, ByVal targetURL As java.net.URL, ByVal desc As String, ByVal sourceElement As Element, ByVal targetFrame As String)
			MyBase.New(source, type, targetURL, desc, sourceElement)
			Me.targetFrame = targetFrame
		End Sub

		''' <summary>
		''' Creates a new object representing a hypertext link event.
		''' </summary>
		''' <param name="source"> the object responsible for the event </param>
		''' <param name="type"> the event type </param>
		''' <param name="targetURL"> the affected URL </param>
		''' <param name="desc"> a description </param>
		''' <param name="sourceElement"> the element that corresponds to the source
		'''                      of the event </param>
		''' <param name="inputEvent">  InputEvent that triggered the hyperlink event </param>
		''' <param name="targetFrame"> the Frame to display the document in
		''' @since 1.7 </param>
		Public Sub New(ByVal source As Object, ByVal type As EventType, ByVal targetURL As java.net.URL, ByVal desc As String, ByVal sourceElement As Element, ByVal inputEvent As java.awt.event.InputEvent, ByVal targetFrame As String)
			MyBase.New(source, type, targetURL, desc, sourceElement, inputEvent)
			Me.targetFrame = targetFrame
		End Sub

		''' <summary>
		''' returns the target for the link.
		''' </summary>
		Public Overridable Property target As String
			Get
				Return targetFrame
			End Get
		End Property

		Private targetFrame As String
	End Class

End Namespace