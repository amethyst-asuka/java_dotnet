Imports Microsoft.VisualBasic
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
	''' This is the root exception class for all JAXB exceptions.
	''' 
	''' @author <ul><li>Ryan Shoemaker, Sun Microsystems, Inc.</li></ul> </summary>
	''' <seealso cref= JAXBContext </seealso>
	''' <seealso cref= Marshaller </seealso>
	''' <seealso cref= Unmarshaller
	''' @since JAXB1.0 </seealso>
	Public Class JAXBException
		Inherits Exception

		''' <summary>
		''' Vendor specific error code
		''' 
		''' </summary>
		Private errorCode As String

		''' <summary>
		''' Exception reference
		''' 
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private linkedException As Exception

		Friend Const serialVersionUID As Long = -5621384651494307979L

		''' <summary>
		''' Construct a JAXBException with the specified detail message.  The
		''' errorCode and linkedException will default to null.
		''' </summary>
		''' <param name="message"> a description of the exception </param>
		Public Sub New(ByVal message As String)
			Me.New(message, Nothing, Nothing)
		End Sub

		''' <summary>
		''' Construct a JAXBException with the specified detail message and vendor
		''' specific errorCode.  The linkedException will default to null.
		''' </summary>
		''' <param name="message"> a description of the exception </param>
		''' <param name="errorCode"> a string specifying the vendor specific error code </param>
		Public Sub New(ByVal message As String, ByVal errorCode As String)
			Me.New(message, errorCode, Nothing)
		End Sub

		''' <summary>
		''' Construct a JAXBException with a linkedException.  The detail message and
		''' vendor specific errorCode will default to null.
		''' </summary>
		''' <param name="exception"> the linked exception </param>
		Public Sub New(ByVal exception As Exception)
			Me.New(Nothing, Nothing, exception)
		End Sub

		''' <summary>
		''' Construct a JAXBException with the specified detail message and
		''' linkedException.  The errorCode will default to null.
		''' </summary>
		''' <param name="message"> a description of the exception </param>
		''' <param name="exception"> the linked exception </param>
		Public Sub New(ByVal message As String, ByVal exception As Exception)
			Me.New(message, Nothing, exception)
		End Sub

		''' <summary>
		''' Construct a JAXBException with the specified detail message, vendor
		''' specific errorCode, and linkedException.
		''' </summary>
		''' <param name="message"> a description of the exception </param>
		''' <param name="errorCode"> a string specifying the vendor specific error code </param>
		''' <param name="exception"> the linked exception </param>
		Public Sub New(ByVal message As String, ByVal errorCode As String, ByVal exception As Exception)
			MyBase.New(message)
			Me.errorCode = errorCode
			Me.linkedException = exception
		End Sub

		''' <summary>
		''' Get the vendor specific error code
		''' </summary>
		''' <returns> a string specifying the vendor specific error code </returns>
		Public Overridable Property errorCode As String
			Get
				Return Me.errorCode
			End Get
		End Property

		''' <summary>
		''' Get the linked exception
		''' </summary>
		''' <returns> the linked Exception, null if none exists </returns>
		Public Overridable Property linkedException As Exception
			Get
				Return linkedException
			End Get
			Set(ByVal exception As Exception)
				Me.linkedException = exception
			End Set
		End Property


		''' <summary>
		''' Returns a short description of this JAXBException.
		''' 
		''' </summary>
		Public Overrides Function ToString() As String
			Return If(linkedException Is Nothing, MyBase.ToString(), MyBase.ToString() & vbLf & " - with linked exception:" & vbLf & "[" & linkedException.ToString() & "]")
		End Function

		''' <summary>
		''' Prints this JAXBException and its stack trace (including the stack trace
		''' of the linkedException if it is non-null) to the PrintStream.
		''' </summary>
		''' <param name="s"> PrintStream to use for output </param>
		Public Overridable Sub printStackTrace(ByVal s As java.io.PrintStream)
			MyBase.printStackTrace(s)
		End Sub

		''' <summary>
		''' Prints this JAXBException and its stack trace (including the stack trace
		''' of the linkedException if it is non-null) to <tt>System.err</tt>.
		''' 
		''' </summary>
		Public Overridable Sub printStackTrace()
			MyBase.printStackTrace()
		End Sub

		''' <summary>
		''' Prints this JAXBException and its stack trace (including the stack trace
		''' of the linkedException if it is non-null) to the PrintWriter.
		''' </summary>
		''' <param name="s"> PrintWriter to use for output </param>
		Public Overridable Sub printStackTrace(ByVal s As java.io.PrintWriter)
			MyBase.printStackTrace(s)
		End Sub

		Public Property Overrides cause As Exception
			Get
				Return linkedException
			End Get
		End Property
	End Class

End Namespace