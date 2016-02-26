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
	''' This exception indicates that a violation of a dynamically checked type
	''' constraint was detected.
	''' 
	''' <p>
	''' This exception can be thrown by the generated setter methods of the schema
	''' derived Java content classes.  However, since fail-fast validation is
	''' an optional feature for JAXB Providers to support, not all setter methods
	''' will throw this exception when a type constraint is violated.
	''' 
	''' <p>
	''' If this exception is throw while invoking a fail-fast setter, the value of
	''' the property is guaranteed to remain unchanged, as if the setter were never
	''' called.
	''' 
	''' @author <ul><li>Ryan Shoemaker, Sun Microsystems, Inc.</li><li>Joe Fialli, Sun Microsystems, Inc.</li></ul> </summary>
	''' <seealso cref= ValidationEvent
	''' @since JAXB1.0 </seealso>

	Public Class TypeConstraintException
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

		Friend Const serialVersionUID As Long = -3059799699420143848L

		''' <summary>
		''' Construct a TypeConstraintException with the specified detail message.  The
		''' errorCode and linkedException will default to null.
		''' </summary>
		''' <param name="message"> a description of the exception </param>
		Public Sub New(ByVal message As String)
			Me.New(message, Nothing, Nothing)
		End Sub

		''' <summary>
		''' Construct a TypeConstraintException with the specified detail message and vendor
		''' specific errorCode.  The linkedException will default to null.
		''' </summary>
		''' <param name="message"> a description of the exception </param>
		''' <param name="errorCode"> a string specifying the vendor specific error code </param>
		Public Sub New(ByVal message As String, ByVal errorCode As String)
			Me.New(message, errorCode, Nothing)
		End Sub

		''' <summary>
		''' Construct a TypeConstraintException with a linkedException.  The detail message and
		''' vendor specific errorCode will default to null.
		''' </summary>
		''' <param name="exception"> the linked exception </param>
		Public Sub New(ByVal exception As Exception)
			Me.New(Nothing, Nothing, exception)
		End Sub

		''' <summary>
		''' Construct a TypeConstraintException with the specified detail message and
		''' linkedException.  The errorCode will default to null.
		''' </summary>
		''' <param name="message"> a description of the exception </param>
		''' <param name="exception"> the linked exception </param>
		Public Sub New(ByVal message As String, ByVal exception As Exception)
			Me.New(message, Nothing, exception)
		End Sub

		''' <summary>
		''' Construct a TypeConstraintException with the specified detail message,
		''' vendor specific errorCode, and linkedException.
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
		''' Returns a short description of this TypeConstraintException.
		''' 
		''' </summary>
		Public Overrides Function ToString() As String
			Return If(linkedException Is Nothing, MyBase.ToString(), MyBase.ToString() & vbLf & " - with linked exception:" & vbLf & "[" & linkedException.ToString() & "]")
		End Function

		''' <summary>
		''' Prints this TypeConstraintException and its stack trace (including the stack trace
		''' of the linkedException if it is non-null) to the PrintStream.
		''' </summary>
		''' <param name="s"> PrintStream to use for output </param>
		Public Overridable Sub printStackTrace(ByVal s As java.io.PrintStream)
			If linkedException IsNot Nothing Then
			  linkedException.printStackTrace(s)
			  s.println("--------------- linked to ------------------")
			End If

			MyBase.printStackTrace(s)
		End Sub

		''' <summary>
		''' Prints this TypeConstraintException and its stack trace (including the stack trace
		''' of the linkedException if it is non-null) to <tt>System.err</tt>.
		''' 
		''' </summary>
		Public Overridable Sub printStackTrace()
			printStackTrace(System.err)
		End Sub

	End Class

End Namespace