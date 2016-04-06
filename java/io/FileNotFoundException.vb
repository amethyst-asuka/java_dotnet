'
' * Copyright (c) 1994, 2008, Oracle and/or its affiliates. All rights reserved.
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
	''' Signals that an attempt to open the file denoted by a specified pathname
	''' has failed.
	''' 
	''' <p> This exception will be thrown by the <seealso cref="FileInputStream"/>, {@link
	''' FileOutputStream}, and <seealso cref="RandomAccessFile"/> constructors when a file
	''' with the specified pathname does not exist.  It will also be thrown by these
	''' constructors if the file does exist but for some reason is inaccessible, for
	''' example when an attempt is made to open a read-only file for writing.
	''' 
	''' @author  unascribed
	''' @since   JDK1.0
	''' </summary>

	Public Class FileNotFoundException
		Inherits IOException

		Private Shadows Const serialVersionUID As Long = -897856973823710492L

		''' <summary>
		''' Constructs a <code>FileNotFoundException</code> with
		''' <code>null</code> as its error detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a <code>FileNotFoundException</code> with the
		''' specified detail message. The string <code>s</code> can be
		''' retrieved later by the
		''' <code><seealso cref="java.lang.Throwable#getMessage"/></code>
		''' method of class <code>java.lang.Throwable</code>.
		''' </summary>
		''' <param name="s">   the detail message. </param>
		Public Sub New(  s As String)
			MyBase.New(s)
		End Sub

		''' <summary>
		''' Constructs a <code>FileNotFoundException</code> with a detail message
		''' consisting of the given pathname string followed by the given reason
		''' string.  If the <code>reason</code> argument is <code>null</code> then
		''' it will be omitted.  This private constructor is invoked only by native
		''' I/O methods.
		''' 
		''' @since 1.2
		''' </summary>
		Private Sub New(  path As String,   reason As String)
			MyBase.New(path + (If(reason Is Nothing, "", " (" & reason & ")")))
		End Sub

	End Class

End Namespace