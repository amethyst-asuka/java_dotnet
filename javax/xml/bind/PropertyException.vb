Imports System

'
' * Copyright (c) 2004, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' This exception indicates that an error was encountered while getting or
	''' setting a property.
	''' 
	''' @author <ul><li>Ryan Shoemaker, Sun Microsystems, Inc.</li><li>Kohsuke Kawaguchi, Sun Microsystems, Inc.</li><li>Joe Fialli, Sun Microsystems, Inc.</li></ul> </summary>
	''' <seealso cref= JAXBContext </seealso>
	''' <seealso cref= Validator </seealso>
	''' <seealso cref= Unmarshaller
	''' @since JAXB1.0 </seealso>
	Public Class PropertyException
		Inherits JAXBException

		''' <summary>
		''' Construct a PropertyException with the specified detail message.  The
		''' errorCode and linkedException will default to null.
		''' </summary>
		''' <param name="message"> a description of the exception </param>
		Public Sub New(ByVal message As String)
			MyBase.New(message)
		End Sub

		''' <summary>
		''' Construct a PropertyException with the specified detail message and
		''' vendor specific errorCode.  The linkedException will default to null.
		''' </summary>
		''' <param name="message"> a description of the exception </param>
		''' <param name="errorCode"> a string specifying the vendor specific error code </param>
		Public Sub New(ByVal message As String, ByVal errorCode As String)
			MyBase.New(message, errorCode)
		End Sub

		''' <summary>
		''' Construct a PropertyException with a linkedException.  The detail
		''' message and vendor specific errorCode will default to null.
		''' </summary>
		''' <param name="exception"> the linked exception </param>
		Public Sub New(ByVal exception As Exception)
			MyBase.New(exception)
		End Sub

		''' <summary>
		''' Construct a PropertyException with the specified detail message and
		''' linkedException.  The errorCode will default to null.
		''' </summary>
		''' <param name="message"> a description of the exception </param>
		''' <param name="exception"> the linked exception </param>
		Public Sub New(ByVal message As String, ByVal exception As Exception)
			MyBase.New(message, exception)
		End Sub

		''' <summary>
		''' Construct a PropertyException with the specified detail message, vendor
		''' specific errorCode, and linkedException.
		''' </summary>
		''' <param name="message"> a description of the exception </param>
		''' <param name="errorCode"> a string specifying the vendor specific error code </param>
		''' <param name="exception"> the linked exception </param>
		Public Sub New(ByVal message As String, ByVal errorCode As String, ByVal exception As Exception)
			MyBase.New(message, errorCode, exception)
		End Sub

		''' <summary>
		''' Construct a PropertyException whose message field is set based on the
		''' name of the property and value.toString().
		''' </summary>
		''' <param name="name"> the name of the property related to this exception </param>
		''' <param name="value"> the value of the property related to this exception </param>
		Public Sub New(ByVal name As String, ByVal value As Object)
			MyBase.New(Messages.format(Messages.NAME_VALUE, name, value.ToString()))
		End Sub


	End Class

End Namespace