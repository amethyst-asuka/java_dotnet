'
' * Copyright (c) 2010, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' <p>{@code MidiDeviceTransmitter} is a {@code Transmitter} which represents
	''' a MIDI input connector of a {@code MidiDevice}
	''' (see <seealso cref="MidiDevice#getTransmitter()"/>).
	''' 
	''' @since 1.7
	''' </summary>
	Public Interface MidiDeviceTransmitter
		Inherits Transmitter

		''' <summary>
		''' Obtains a MidiDevice object which is an owner of this Transmitter. </summary>
		''' <returns> a MidiDevice object which is an owner of this Transmitter </returns>
		ReadOnly Property midiDevice As MidiDevice
	End Interface

End Namespace