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

Namespace javax.xml.bind

	''' <summary>
	''' This exception indicates that an error has occurred while performing
	''' a marshal operation that the provider is unable to recover from.
	''' 
	''' <p>
	''' The <tt>ValidationEventHandler</tt> can cause this exception to be thrown
	''' during the marshal operations.  See
	''' {@link ValidationEventHandler#handleEvent(ValidationEvent)
	''' ValidationEventHandler.handleEvent(ValidationEvent)}.
	''' 
	''' @author <ul><li>Ryan Shoemaker, Sun Microsystems, Inc.</li></ul> </summary>
	''' <seealso cref= JAXBException </seealso>
	''' <seealso cref= Marshaller
	''' @since JAXB1.0 </seealso>
	Public Class MarshalException
		Inherits JAXBException

		''' <summary>
		''' Construct a MarshalException with the specified detail message.  The
		''' errorCode and linkedException will default to null.
		''' </summary>
		''' <param name="message"> a description of the exception </param>
		Public Sub New(ByVal message As String)
			Me.New(message, Nothing, Nothing)
		End Sub

		''' <summary>
		''' Construct a MarshalException with the specified detail message and vendor
		''' specific errorCode.  The linkedException will default to null.
		''' </summary>
		''' <param name="message"> a description of the exception </param>
		''' <param name="errorCode"> a string specifying the vendor specific error code </param>
		Public Sub New(ByVal message As String, ByVal errorCode As String)
			Me.New(message, errorCode, Nothing)
		End Sub

		''' <summary>
		''' Construct a MarshalException with a linkedException.  The detail message and
		''' vendor specific errorCode will default to null.
		''' </summary>
		''' <param name="exception"> the linked exception </param>
		Public Sub New(ByVal exception As Exception)
			Me.New(Nothing, Nothing, exception)
		End Sub

		''' <summary>
		''' Construct a MarshalException with the specified detail message and
		''' linkedException.  The errorCode will default to null.
		''' </summary>
		''' <param name="message"> a description of the exception </param>
		''' <param name="exception"> the linked exception </param>
		Public Sub New(ByVal message As String, ByVal exception As Exception)
			Me.New(message, Nothing, exception)
		End Sub

		''' <summary>
		''' Construct a MarshalException with the specified detail message, vendor
		''' specific errorCode, and linkedException.
		''' </summary>
		''' <param name="message"> a description of the exception </param>
		''' <param name="errorCode"> a string specifying the vendor specific error code </param>
		''' <param name="exception"> the linked exception </param>
		Public Sub New(ByVal message As String, ByVal errorCode As String, ByVal exception As Exception)
			MyBase.New(message, errorCode, exception)
		End Sub

	End Class

End Namespace