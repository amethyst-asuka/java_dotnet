'
' * Copyright (c) 1999, 2010, Oracle and/or its affiliates. All rights reserved.
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
	''' A <code>Transmitter</code> sends <code><seealso cref="MidiEvent"/></code> objects to one or more
	''' <code><seealso cref="Receiver Receivers"/></code>. Common MIDI transmitters include sequencers
	''' and MIDI input ports.
	''' </summary>
	''' <seealso cref= Receiver
	''' 
	''' @author Kara Kytle </seealso>
	Public Interface Transmitter
		Inherits AutoCloseable


		''' <summary>
		''' Sets the receiver to which this transmitter will deliver MIDI messages.
		''' If a receiver is currently set, it is replaced with this one. </summary>
		''' <param name="receiver"> the desired receiver. </param>
		Property receiver As Receiver




		''' <summary>
		''' Indicates that the application has finished using the transmitter, and
		''' that limited resources it requires may be released or made available.
		''' 
		''' <p>If the creation of this <code>Transmitter</code> resulted in
		''' implicitly opening the underlying device, the device is
		''' implicitly closed by this method. This is true unless the device is
		''' kept open by other <code>Receiver</code> or <code>Transmitter</code>
		''' instances that opened the device implicitly, and unless the device
		''' has been opened explicitly. If the device this
		''' <code>Transmitter</code> is retrieved from is closed explicitly
		''' by calling <seealso cref="MidiDevice#close MidiDevice.close"/>, the
		''' <code>Transmitter</code> is closed, too.  For a detailed
		''' description of open/close behaviour see the class description
		''' of <seealso cref="javax.sound.midi.MidiDevice MidiDevice"/>.
		''' </summary>
		''' <seealso cref= javax.sound.midi.MidiSystem#getTransmitter </seealso>
		Sub close()
	End Interface

End Namespace