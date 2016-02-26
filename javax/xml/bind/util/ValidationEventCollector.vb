Imports System.Collections.Generic

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

Namespace javax.xml.bind.util


	''' <summary>
	''' <seealso cref="javax.xml.bind.ValidationEventHandler ValidationEventHandler"/>
	''' implementation that collects all events.
	''' 
	''' <p>
	''' To use this class, create a new instance and pass it to the setEventHandler
	''' method of the Validator, Unmarshaller, Marshaller class.  After the call to
	''' validate or unmarshal completes, call the getEvents method to retrieve all
	''' the reported errors and warnings.
	''' 
	''' @author <ul><li>Kohsuke Kawaguchi, Sun Microsystems, Inc.</li><li>Ryan Shoemaker, Sun Microsystems, Inc.</li><li>Joe Fialli, Sun Microsystems, Inc.</li></ul> </summary>
	''' <seealso cref= javax.xml.bind.Validator </seealso>
	''' <seealso cref= javax.xml.bind.ValidationEventHandler </seealso>
	''' <seealso cref= javax.xml.bind.ValidationEvent </seealso>
	''' <seealso cref= javax.xml.bind.ValidationEventLocator
	''' @since JAXB1.0 </seealso>
	Public Class ValidationEventCollector
		Implements javax.xml.bind.ValidationEventHandler

		Private ReadOnly events As IList(Of javax.xml.bind.ValidationEvent) = New List(Of javax.xml.bind.ValidationEvent)

		''' <summary>
		''' Return an array of ValidationEvent objects containing a copy of each of
		''' the collected errors and warnings.
		''' 
		''' @return
		'''      a copy of all the collected errors and warnings or an empty array
		'''      if there weren't any
		''' </summary>
		Public Overridable Property events As javax.xml.bind.ValidationEvent()
			Get
				Return events.ToArray()
			End Get
		End Property

		''' <summary>
		''' Clear all collected errors and warnings.
		''' </summary>
		Public Overridable Sub reset()
			events.Clear()
		End Sub

		''' <summary>
		''' Returns true if this event collector contains at least one
		''' ValidationEvent.
		''' </summary>
		''' <returns> true if this event collector contains at least one
		'''         ValidationEvent, false otherwise </returns>
		Public Overridable Function hasEvents() As Boolean
			Return events.Count > 0
		End Function

		Public Overridable Function handleEvent(ByVal [event] As javax.xml.bind.ValidationEvent) As Boolean
			events.Add([event])

			Dim retVal As Boolean = True
			Select Case [event].severity
				Case javax.xml.bind.ValidationEvent.WARNING
					retVal = True ' continue validation
				Case javax.xml.bind.ValidationEvent.ERROR
					retVal = True ' continue validation
				Case javax.xml.bind.ValidationEvent.FATAL_ERROR
					retVal = False ' halt validation
				Case Else
					_assert(False, Messages.format(Messages.UNRECOGNIZED_SEVERITY, [event].severity))
			End Select

			Return retVal
		End Function

		Private Shared Sub _assert(ByVal b As Boolean, ByVal msg As String)
			If Not b Then Throw New InternalError(msg)
		End Sub
	End Class

End Namespace