Imports System

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
	''' Default implementation of the ValidationEvent interface.
	''' 
	''' <p>
	''' JAXB providers are allowed to use whatever class that implements
	''' the ValidationEvent interface. This class is just provided for a
	''' convenience.
	''' 
	''' @author <ul><li>Kohsuke Kawaguchi, Sun Microsystems, Inc.</li></ul> </summary>
	''' <seealso cref= javax.xml.bind.Validator </seealso>
	''' <seealso cref= javax.xml.bind.ValidationEventHandler </seealso>
	''' <seealso cref= javax.xml.bind.ValidationEvent </seealso>
	''' <seealso cref= javax.xml.bind.ValidationEventLocator
	''' @since JAXB1.0 </seealso>
	Public Class ValidationEventImpl
		Implements javax.xml.bind.ValidationEvent

		''' <summary>
		''' Create a new ValidationEventImpl.
		''' </summary>
		''' <param name="_severity"> The severity value for this event.  Must be one of
		''' ValidationEvent.WARNING, ValidationEvent.ERROR, or
		''' ValidationEvent.FATAL_ERROR </param>
		''' <param name="_message"> The text message for this event - may be null. </param>
		''' <param name="_locator"> The locator object for this event - may be null. </param>
		''' <exception cref="IllegalArgumentException"> if an illegal severity field is supplied </exception>
		Public Sub New(ByVal _severity As Integer, ByVal _message As String, ByVal _locator As javax.xml.bind.ValidationEventLocator)

			Me.New(_severity,_message,_locator,Nothing)
		End Sub

		''' <summary>
		''' Create a new ValidationEventImpl.
		''' </summary>
		''' <param name="_severity"> The severity value for this event.  Must be one of
		''' ValidationEvent.WARNING, ValidationEvent.ERROR, or
		''' ValidationEvent.FATAL_ERROR </param>
		''' <param name="_message"> The text message for this event - may be null. </param>
		''' <param name="_locator"> The locator object for this event - may be null. </param>
		''' <param name="_linkedException"> An optional linked exception that may provide
		''' additional information about the event - may be null. </param>
		''' <exception cref="IllegalArgumentException"> if an illegal severity field is supplied </exception>
		Public Sub New(ByVal _severity As Integer, ByVal _message As String, ByVal _locator As javax.xml.bind.ValidationEventLocator, ByVal _linkedException As Exception)

			severity = _severity
			Me.message = _message
			Me.locator = _locator
			Me.linkedException = _linkedException
		End Sub

		Private severity As Integer
		Private message As String
		Private linkedException As Exception
		Private locator As javax.xml.bind.ValidationEventLocator

		Public Overridable Property severity As Integer
			Get
				Return severity
			End Get
			Set(ByVal _severity As Integer)
    
				If _severity <> javax.xml.bind.ValidationEvent.WARNING AndAlso _severity <> javax.xml.bind.ValidationEvent.ERROR AndAlso _severity <> javax.xml.bind.ValidationEvent.FATAL_ERROR Then Throw New System.ArgumentException(Messages.format(Messages.ILLEGAL_SEVERITY))
    
				Me.severity = _severity
			End Set
		End Property



		Public Overridable Property message As String
			Get
				Return message
			End Get
			Set(ByVal _message As String)
				Me.message = _message
			End Set
		End Property

		Public Overridable Property linkedException As Exception
			Get
				Return linkedException
			End Get
			Set(ByVal _linkedException As Exception)
				Me.linkedException = _linkedException
			End Set
		End Property

		Public Overridable Property locator As javax.xml.bind.ValidationEventLocator
			Get
				Return locator
			End Get
			Set(ByVal _locator As javax.xml.bind.ValidationEventLocator)
				Me.locator = _locator
			End Set
		End Property

		''' <summary>
		''' Returns a string representation of this object in a format
		''' helpful to debugging.
		''' </summary>
		''' <seealso cref= Object#equals(Object) </seealso>
		Public Overrides Function ToString() As String
			Dim s As String
			Select Case severity
			Case WARNING
				s="WARNING"
			Case ERROR
				s="ERROR"
			Case FATAL_ERROR
				s="FATAL_ERROR"
			Case Else
				s=Convert.ToString(severity)
			End Select
			Return java.text.MessageFormat.format("[severity={0},message={1},locator={2}]", New Object(){ s, message, locator })
		End Function
	End Class

End Namespace