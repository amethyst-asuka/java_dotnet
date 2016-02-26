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
	''' Default implementation of the PrintConversionEvent interface.
	''' 
	''' <p>
	''' JAXB providers are allowed to use whatever class that implements
	''' the ValidationEvent interface. This class is just provided for a
	''' convenience.
	''' 
	''' @author <ul><li>Ryan Shoemaker, Sun Microsystems, Inc.</li></ul> </summary>
	''' <seealso cref= javax.xml.bind.PrintConversionEvent </seealso>
	''' <seealso cref= javax.xml.bind.Validator </seealso>
	''' <seealso cref= javax.xml.bind.ValidationEventHandler </seealso>
	''' <seealso cref= javax.xml.bind.ValidationEvent </seealso>
	''' <seealso cref= javax.xml.bind.ValidationEventLocator
	''' @since JAXB1.0 </seealso>
	Public Class PrintConversionEventImpl
		Inherits ValidationEventImpl
		Implements javax.xml.bind.PrintConversionEvent

		''' <summary>
		''' Create a new PrintConversionEventImpl.
		''' </summary>
		''' <param name="_severity"> The severity value for this event.  Must be one of
		''' ValidationEvent.WARNING, ValidationEvent.ERROR, or
		''' ValidationEvent.FATAL_ERROR </param>
		''' <param name="_message"> The text message for this event - may be null. </param>
		''' <param name="_locator"> The locator object for this event - may be null. </param>
		''' <exception cref="IllegalArgumentException"> if an illegal severity field is supplied </exception>
		Public Sub New(ByVal _severity As Integer, ByVal _message As String, ByVal _locator As javax.xml.bind.ValidationEventLocator)

			MyBase.New(_severity, _message, _locator)
		End Sub

		''' <summary>
		''' Create a new PrintConversionEventImpl.
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

			MyBase.New(_severity, _message, _locator, _linkedException)
		End Sub

	End Class

End Namespace