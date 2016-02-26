Imports System

'
' * Copyright (c) 1999, 2002, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.sound.sampled

	''' <summary>
	''' An <code>UnsupportedAudioFileException</code> is an exception indicating that an
	''' operation failed because a file did not contain valid data of a recognized file
	''' type and format.
	''' 
	''' @author Kara Kytle
	''' @since 1.3
	''' </summary>
	'
	' * An <code>UnsupportedAudioFileException</code> is an exception indicating that an
	' * operation failed because a file did not contain valid data of a recognized file
	' * type and format.
	' *
	' * @author Kara Kytle
	' 

	Public Class UnsupportedAudioFileException
		Inherits Exception

		''' <summary>
		''' Constructs a <code>UnsupportedAudioFileException</code> that has
		''' <code>null</code> as its error detail message.
		''' </summary>
		Public Sub New()

			MyBase.New()
		End Sub


		''' <summary>
		''' Constructs a <code>UnsupportedAudioFileException</code> that has
		''' the specified detail message.
		''' </summary>
		''' <param name="message"> a string containing the error detail message </param>
		Public Sub New(ByVal message As String)

			MyBase.New(message)
		End Sub
	End Class

End Namespace