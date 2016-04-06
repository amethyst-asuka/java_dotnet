'
' * Copyright (c) 1996, 2006, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.io

	''' <summary>
	''' Thrown when the Serialization runtime detects one of the following
	''' problems with a Class.
	''' <UL>
	''' <LI> The serial version of the class does not match that of the class
	'''      descriptor read from the stream
	''' <LI> The class contains unknown datatypes
	''' <LI> The class does not have an accessible no-arg constructor
	''' </UL>
	''' 
	''' @author  unascribed
	''' @since   JDK1.1
	''' </summary>
	Public Class InvalidClassException
		Inherits ObjectStreamException

		Private Shadows Const serialVersionUID As Long = -4333316296251054416L

		''' <summary>
		''' Name of the invalid class.
		''' 
		''' @serial Name of the invalid class.
		''' </summary>
		Public classname As String

		''' <summary>
		''' Report an InvalidClassException for the reason specified.
		''' </summary>
		''' <param name="reason">  String describing the reason for the exception. </param>
		Public Sub New(  reason As String)
			MyBase.New(reason)
		End Sub

		''' <summary>
		''' Constructs an InvalidClassException object.
		''' </summary>
		''' <param name="cname">   a String naming the invalid class. </param>
		''' <param name="reason">  a String describing the reason for the exception. </param>
		Public Sub New(  cname As String,   reason As String)
			MyBase.New(reason)
			classname = cname
		End Sub

		''' <summary>
		''' Produce the message and include the classname, if present.
		''' </summary>
		Public  Overrides ReadOnly Property  message As String
			Get
				If classname Is Nothing Then
					Return MyBase.message
				Else
					Return classname & "; " & MyBase.message
				End If
			End Get
		End Property
	End Class

End Namespace