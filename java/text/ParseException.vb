Imports System

'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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

'
' * (C) Copyright Taligent, Inc. 1996, 1997 - All Rights Reserved
' * (C) Copyright IBM Corp. 1996 - 1998 - All Rights Reserved
' *
' *   The original version of this source code and documentation is copyrighted
' * and owned by Taligent, Inc., a wholly-owned subsidiary of IBM. These
' * materials are provided under terms of a License Agreement between Taligent
' * and Sun. This technology is protected by multiple US and International
' * patents. This notice and attribution to Taligent may not be removed.
' *   Taligent is a registered trademark of Taligent, Inc.
' *
' 

Namespace java.text

	''' <summary>
	''' Signals that an error has been reached unexpectedly
	''' while parsing. </summary>
	''' <seealso cref= java.lang.Exception </seealso>
	''' <seealso cref= java.text.Format </seealso>
	''' <seealso cref= java.text.FieldPosition
	''' @author      Mark Davis </seealso>
	Public Class ParseException
		Inherits Exception

		Private Shadows Const serialVersionUID As Long = 2703218443322787634L

		''' <summary>
		''' Constructs a ParseException with the specified detail message and
		''' offset.
		''' A detail message is a String that describes this particular exception.
		''' </summary>
		''' <param name="s"> the detail message </param>
		''' <param name="errorOffset"> the position where the error is found while parsing. </param>
		Public Sub New(ByVal s As String, ByVal errorOffset As Integer)
			MyBase.New(s)
			Me.errorOffset = errorOffset
		End Sub

		''' <summary>
		''' Returns the position where the error was found.
		''' </summary>
		''' <returns> the position where the error was found </returns>
		Public Overridable Property errorOffset As Integer
			Get
				Return errorOffset
			End Get
		End Property

		'============ privates ============
		''' <summary>
		''' The zero-based character offset into the string being parsed at which
		''' the error was found during parsing.
		''' @serial
		''' </summary>
		Private errorOffset As Integer
	End Class

End Namespace