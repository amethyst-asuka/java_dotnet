Imports System

'
' * Copyright (c) 2000, 2003, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.util.prefs


	''' <summary>
	''' Thrown to indicate that an operation could not complete because
	''' the input did not conform to the appropriate XML document type
	''' for a collection of preferences, as per the <seealso cref="Preferences"/>
	''' specification.
	''' 
	''' @author  Josh Bloch </summary>
	''' <seealso cref=     Preferences
	''' @since   1.4 </seealso>
	Public Class InvalidPreferencesFormatException
		Inherits Exception

		''' <summary>
		''' Constructs an InvalidPreferencesFormatException with the specified
		''' cause.
		''' </summary>
		''' <param name="cause"> the cause (which is saved for later retrieval by the
		'''         <seealso cref="Throwable#getCause()"/> method). </param>
		Public Sub New(ByVal cause As Throwable)
			MyBase.New(cause)
		End Sub

	   ''' <summary>
	   ''' Constructs an InvalidPreferencesFormatException with the specified
	   ''' detail message.
	   ''' </summary>
	   ''' <param name="message">   the detail message. The detail message is saved for
	   '''          later retrieval by the <seealso cref="Throwable#getMessage()"/> method. </param>
		Public Sub New(ByVal message As String)
			MyBase.New(message)
		End Sub

		''' <summary>
		''' Constructs an InvalidPreferencesFormatException with the specified
		''' detail message and cause.
		''' </summary>
		''' <param name="message">   the detail message. The detail message is saved for
		'''         later retrieval by the <seealso cref="Throwable#getMessage()"/> method. </param>
		''' <param name="cause"> the cause (which is saved for later retrieval by the
		'''         <seealso cref="Throwable#getCause()"/> method). </param>
		Public Sub New(ByVal message As String, ByVal cause As Throwable)
			MyBase.New(message, cause)
		End Sub

		Private Shadows Const serialVersionUID As Long = -791715184232119669L
	End Class

End Namespace