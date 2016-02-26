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
	''' <p>{@code MidiDeviceReceiver} is a {@code Receiver} which represents
	''' a MIDI input connector of a {@code MidiDevice}
	''' (see <seealso cref="MidiDevice#getReceiver()"/>).
	''' 
	''' @since 1.7
	''' </summary>
	Public Interface MidiDeviceReceiver
		Inherits Receiver

		''' <summary>
		''' Obtains a MidiDevice object which is an owner of this Receiver. </summary>
		''' <returns> a MidiDevice object which is an owner of this Receiver </returns>
		ReadOnly Property midiDevice As MidiDevice
	End Interface

End Namespace