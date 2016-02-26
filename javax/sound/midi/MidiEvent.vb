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
	''' MIDI events contain a MIDI message and a corresponding time-stamp
	''' expressed in ticks, and can represent the MIDI event information
	''' stored in a MIDI file or a <code><seealso cref="Sequence"/></code> object.  The
	''' duration of a tick is specified by the timing information contained
	''' in the MIDI file or <code>Sequence</code> object.
	''' <p>
	''' In Java Sound, <code>MidiEvent</code> objects are typically contained in a
	''' <code><seealso cref="Track"/></code>, and <code>Tracks</code> are likewise
	''' contained in a <code>Sequence</code>.
	''' 
	''' 
	''' @author David Rivas
	''' @author Kara Kytle
	''' </summary>
	Public Class MidiEvent


		' Instance variables

		''' <summary>
		''' The MIDI message for this event.
		''' </summary>
		Private ReadOnly message As MidiMessage


		''' <summary>
		''' The tick value for this event.
		''' </summary>
		Private tick As Long


		''' <summary>
		''' Constructs a new <code>MidiEvent</code>. </summary>
		''' <param name="message"> the MIDI message contained in the event </param>
		''' <param name="tick"> the time-stamp for the event, in MIDI ticks </param>
		Public Sub New(ByVal message As MidiMessage, ByVal tick As Long)

			Me.message = message
			Me.tick = tick
		End Sub

		''' <summary>
		''' Obtains the MIDI message contained in the event. </summary>
		''' <returns> the MIDI message </returns>
		Public Overridable Property message As MidiMessage
			Get
				Return message
			End Get
		End Property


		''' <summary>
		''' Sets the time-stamp for the event, in MIDI ticks </summary>
		''' <param name="tick"> the new time-stamp, in MIDI ticks </param>
		Public Overridable Property tick As Long
			Set(ByVal tick As Long)
				Me.tick = tick
			End Set
			Get
				Return tick
			End Get
		End Property


	End Class

End Namespace