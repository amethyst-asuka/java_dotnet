Imports System

'
' * Copyright (c) 1998, 1999, Oracle and/or its affiliates. All rights reserved.
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

Namespace org.omg.CORBA.portable

	''' <summary>
	''' This class is used for reporting application level exceptions between ORBs and stubs.
	''' </summary>

	Public Class ApplicationException
		Inherits Exception

		''' <summary>
		''' Constructs an ApplicationException from the CORBA repository ID of the exception
		''' and an input stream from which the exception data can be read as its parameters. </summary>
		''' <param name="id"> the repository id of the user exception </param>
		''' <param name="ins"> the stream which contains the user exception data </param>
		Public Sub New(ByVal id As String, ByVal ins As InputStream)
			Me.id = id
			Me.ins = ins
		End Sub

		''' <summary>
		''' Returns the CORBA repository ID of the exception
		''' without removing it from the exceptions input stream. </summary>
		''' <returns> The CORBA repository ID of this exception </returns>
		Public Overridable Property id As String
			Get
				Return id
			End Get
		End Property

		''' <summary>
		''' Returns the input stream from which the exception data can be read as its parameters. </summary>
		''' <returns> The stream which contains the user exception data </returns>
		Public Overridable Property inputStream As InputStream
			Get
				Return ins
			End Get
		End Property

		Private id As String
		Private ins As InputStream
	End Class

End Namespace