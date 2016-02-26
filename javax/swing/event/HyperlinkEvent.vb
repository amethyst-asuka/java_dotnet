'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.event



	''' <summary>
	''' HyperlinkEvent is used to notify interested parties that
	''' something has happened with respect to a hypertext link.
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' 
	''' @author  Timothy Prinzing
	''' </summary>
	Public Class HyperlinkEvent
		Inherits java.util.EventObject

		''' <summary>
		''' Creates a new object representing a hypertext link event.
		''' The other constructor is preferred, as it provides more
		''' information if a URL could not be formed.  This constructor
		''' is primarily for backward compatibility.
		''' </summary>
		''' <param name="source"> the object responsible for the event </param>
		''' <param name="type"> the event type </param>
		''' <param name="u"> the affected URL </param>
		Public Sub New(ByVal source As Object, ByVal type As EventType, ByVal u As java.net.URL)
			Me.New(source, type, u, Nothing)
		End Sub

		''' <summary>
		''' Creates a new object representing a hypertext link event.
		''' </summary>
		''' <param name="source"> the object responsible for the event </param>
		''' <param name="type"> the event type </param>
		''' <param name="u"> the affected URL.  This may be null if a valid URL
		'''   could not be created. </param>
		''' <param name="desc"> the description of the link.  This may be useful
		'''   when attempting to form a URL resulted in a MalformedURLException.
		'''   The description provides the text used when attempting to form the
		'''   URL. </param>
		Public Sub New(ByVal source As Object, ByVal type As EventType, ByVal u As java.net.URL, ByVal desc As String)
			Me.New(source, type, u, desc, Nothing)
		End Sub

		''' <summary>
		''' Creates a new object representing a hypertext link event.
		''' </summary>
		''' <param name="source"> the object responsible for the event </param>
		''' <param name="type"> the event type </param>
		''' <param name="u"> the affected URL.  This may be null if a valid URL
		'''   could not be created. </param>
		''' <param name="desc"> the description of the link.  This may be useful
		'''   when attempting to form a URL resulted in a MalformedURLException.
		'''   The description provides the text used when attempting to form the
		'''   URL. </param>
		''' <param name="sourceElement"> Element in the Document representing the
		'''   anchor
		''' @since 1.4 </param>
		Public Sub New(ByVal source As Object, ByVal type As EventType, ByVal u As java.net.URL, ByVal desc As String, ByVal sourceElement As javax.swing.text.Element)
			MyBase.New(source)
			Me.type = type
			Me.u = u
			Me.desc = desc
			Me.sourceElement = sourceElement
		End Sub

		''' <summary>
		''' Creates a new object representing a hypertext link event.
		''' </summary>
		''' <param name="source"> the object responsible for the event </param>
		''' <param name="type"> the event type </param>
		''' <param name="u"> the affected URL.  This may be null if a valid URL
		'''   could not be created. </param>
		''' <param name="desc"> the description of the link.  This may be useful
		'''   when attempting to form a URL resulted in a MalformedURLException.
		'''   The description provides the text used when attempting to form the
		'''   URL. </param>
		''' <param name="sourceElement"> Element in the Document representing the
		'''   anchor </param>
		''' <param name="inputEvent">  InputEvent that triggered the hyperlink event
		''' @since 1.7 </param>
		Public Sub New(ByVal source As Object, ByVal type As EventType, ByVal u As java.net.URL, ByVal desc As String, ByVal sourceElement As javax.swing.text.Element, ByVal inputEvent As java.awt.event.InputEvent)
			MyBase.New(source)
			Me.type = type
			Me.u = u
			Me.desc = desc
			Me.sourceElement = sourceElement
			Me.inputEvent = inputEvent
		End Sub

		''' <summary>
		''' Gets the type of event.
		''' </summary>
		''' <returns> the type </returns>
		Public Overridable Property eventType As EventType
			Get
				Return type
			End Get
		End Property

		''' <summary>
		''' Get the description of the link as a string.
		''' This may be useful if a URL can't be formed
		''' from the description, in which case the associated
		''' URL would be null.
		''' </summary>
		Public Overridable Property description As String
			Get
				Return desc
			End Get
		End Property

		''' <summary>
		''' Gets the URL that the link refers to.
		''' </summary>
		''' <returns> the URL </returns>
		Public Overridable Property uRL As java.net.URL
			Get
				Return u
			End Get
		End Property

		''' <summary>
		''' Returns the <code>Element</code> that corresponds to the source of the
		''' event. This will typically be an <code>Element</code> representing
		''' an anchor. If a constructor that is used that does not specify a source
		''' <code>Element</code>, or null was specified as the source
		''' <code>Element</code>, this will return null.
		''' </summary>
		''' <returns> Element indicating source of event, or null
		''' @since 1.4 </returns>
		Public Overridable Property sourceElement As javax.swing.text.Element
			Get
				Return sourceElement
			End Get
		End Property

		''' <summary>
		''' Returns the {@code InputEvent} that triggered the hyperlink event.
		''' This will typically be a {@code MouseEvent}.  If a constructor is used
		''' that does not specify an {@code InputEvent}, or @{code null}
		''' was specified as the {@code InputEvent}, this returns {@code null}.
		''' </summary>
		''' <returns>  InputEvent that triggered the hyperlink event, or null
		''' @since 1.7 </returns>
		Public Overridable Property inputEvent As java.awt.event.InputEvent
			Get
				Return inputEvent
			End Get
		End Property

		Private type As EventType
		Private u As java.net.URL
		Private desc As String
		Private sourceElement As javax.swing.text.Element
		Private inputEvent As java.awt.event.InputEvent


		''' <summary>
		''' Defines the ENTERED, EXITED, and ACTIVATED event types, along
		''' with their string representations, returned by toString().
		''' </summary>
		Public NotInheritable Class EventType

			Private Sub New(ByVal s As String)
				typeString = s
			End Sub

			''' <summary>
			''' Entered type.
			''' </summary>
			Public Shared ReadOnly ENTERED As New EventType("ENTERED")

			''' <summary>
			''' Exited type.
			''' </summary>
			Public Shared ReadOnly EXITED As New EventType("EXITED")

			''' <summary>
			''' Activated type.
			''' </summary>
			Public Shared ReadOnly ACTIVATED As New EventType("ACTIVATED")

			''' <summary>
			''' Converts the type to a string.
			''' </summary>
			''' <returns> the string </returns>
			Public Overrides Function ToString() As String
				Return typeString
			End Function

			Private typeString As String
		End Class
	End Class

End Namespace