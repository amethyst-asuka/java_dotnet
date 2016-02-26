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

Namespace javax.sound.midi


	''' <summary>
	''' A <code>MidiUnavailableException</code> is thrown when a requested MIDI
	''' component cannot be opened or created because it is unavailable.  This often
	''' occurs when a device is in use by another application.  More generally, it
	''' can occur when there is a finite number of a certain kind of resource that can
	''' be used for some purpose, and all of them are already in use (perhaps all by
	''' this application).  For an example of the latter case, see the
	''' <seealso cref="Transmitter#setReceiver(Receiver) setReceiver"/> method of
	''' <code>Transmitter</code>.
	''' 
	''' @author Kara Kytle
	''' </summary>
	Public Class MidiUnavailableException
		Inherits Exception

		''' <summary>
		''' Constructs a <code>MidiUnavailableException</code> that has
		''' <code>null</code> as its error detail message.
		''' </summary>
		Public Sub New()

			MyBase.New()
		End Sub


		''' <summary>
		'''  Constructs a <code>MidiUnavailableException</code> with the
		''' specified detail message.
		''' </summary>
		''' <param name="message"> the string to display as an error detail message </param>
		Public Sub New(ByVal message As String)

			MyBase.New(message)
		End Sub
	End Class

End Namespace