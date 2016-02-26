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
	''' A <code>LineUnavailableException</code> is an exception indicating that a
	''' line cannot be opened because it is unavailable.  This situation
	''' arises most commonly when a requested line is already in use
	''' by another application.
	''' 
	''' @author Kara Kytle
	''' @since 1.3
	''' </summary>
	'
	' * A <code>LinenavailableException</code> is an exception indicating that a
	' * line annot be opened because it is unavailable.  This situation
	' * arises most commonly when a line is requested when it is already in use
	' * by another application.
	' *
	' * @author Kara Kytle
	' 

	Public Class LineUnavailableException
		Inherits Exception

		''' <summary>
		''' Constructs a <code>LineUnavailableException</code> that has
		''' <code>null</code> as its error detail message.
		''' </summary>
		Public Sub New()

			MyBase.New()
		End Sub


		''' <summary>
		''' Constructs a <code>LineUnavailableException</code> that has
		''' the specified detail message.
		''' </summary>
		''' <param name="message"> a string containing the error detail message </param>
		Public Sub New(ByVal message As String)

			MyBase.New(message)
		End Sub
	End Class

End Namespace