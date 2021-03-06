Imports System
Imports System.Diagnostics
Imports System.Text

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

Namespace javax.xml.bind.helpers



	''' <summary>
	''' <p>
	''' JAXB 1.0 only default validation event handler. This is the default
	''' handler for all objects created from a JAXBContext that is managing
	''' schema-derived code generated by a JAXB 1.0 binding compiler.
	''' 
	''' <p>
	''' This handler causes the unmarshal and validate operations to fail on the first
	''' error or fatal error.
	''' 
	''' <p>
	''' This handler is not the default handler for JAXB mapped classes following
	''' JAXB 2.0 or later versions. Default validation event handling has changed
	''' and is specified in  <seealso cref="javax.xml.bind.Unmarshaller"/> and
	''' <seealso cref="javax.xml.bind.Marshaller"/>.
	''' 
	''' @author <ul><li>Ryan Shoemaker, Sun Microsystems, Inc.</li></ul> </summary>
	''' <seealso cref= javax.xml.bind.Unmarshaller </seealso>
	''' <seealso cref= javax.xml.bind.Validator </seealso>
	''' <seealso cref= javax.xml.bind.ValidationEventHandler
	''' @since JAXB1.0 </seealso>
	Public Class DefaultValidationEventHandler
		Implements javax.xml.bind.ValidationEventHandler

		Public Overridable Function handleEvent(ByVal [event] As javax.xml.bind.ValidationEvent) As Boolean

			If [event] Is Nothing Then Throw New System.ArgumentException

			' calculate the severity prefix and return value
			Dim severity As String = Nothing
			Dim retVal As Boolean = False
			Select Case [event].severity
				Case javax.xml.bind.ValidationEvent.WARNING
					severity = Messages.format(Messages.WARNING)
					retVal = True ' continue after warnings
				Case javax.xml.bind.ValidationEvent.ERROR
					severity = Messages.format(Messages.ERROR)
					retVal = False ' terminate after errors
				Case javax.xml.bind.ValidationEvent.FATAL_ERROR
					severity = Messages.format(Messages.FATAL_ERROR)
					retVal = False ' terminate after fatal errors
				Case Else
					Debug.Assert(False, Messages.format(Messages.UNRECOGNIZED_SEVERITY, [event].severity))
			End Select

			' calculate the location message
			Dim ___location As String = getLocation([event])

			Console.WriteLine(Messages.format(Messages.SEVERITY_MESSAGE, severity, [event].message, ___location))

			' fail on the first error or fatal error
			Return retVal
		End Function

		''' <summary>
		''' Calculate a location message for the event
		''' 
		''' </summary>
		Private Function getLocation(ByVal [event] As javax.xml.bind.ValidationEvent) As String
			Dim msg As New StringBuilder

			Dim locator As javax.xml.bind.ValidationEventLocator = [event].locator

			If locator IsNot Nothing Then

				Dim url As java.net.URL = locator.uRL
				Dim obj As Object = locator.object
				Dim node As org.w3c.dom.Node = locator.node
				Dim line As Integer = locator.lineNumber

				If url IsNot Nothing OrElse line<>-1 Then
					msg.Append("line " & line)
					If url IsNot Nothing Then msg.Append(" of " & url)
				ElseIf obj IsNot Nothing Then
					msg.Append(" obj: " & obj.ToString())
				ElseIf node IsNot Nothing Then
					msg.Append(" node: " & node.ToString())
				End If
			Else
				msg.Append(Messages.format(Messages.LOCATION_UNAVAILABLE))
			End If

			Return msg.ToString()
		End Function
	End Class

End Namespace