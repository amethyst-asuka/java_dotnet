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
	''' A <code>Receiver</code> receives <code><seealso cref="MidiEvent"/></code> objects and
	''' typically does something useful in response, such as interpreting them to
	''' generate sound or raw MIDI output.  Common MIDI receivers include
	''' synthesizers and MIDI Out ports.
	''' </summary>
	''' <seealso cref= MidiDevice </seealso>
	''' <seealso cref= Synthesizer </seealso>
	''' <seealso cref= Transmitter
	''' 
	''' @author Kara Kytle </seealso>
	Public Interface Receiver
		Inherits AutoCloseable


		'$$fb 2002-04-12: fix for 4662090: Contradiction in Receiver specification
		''' <summary>
		''' Sends a MIDI message and time-stamp to this receiver.
		''' If time-stamping is not supported by this receiver, the time-stamp
		''' value should be -1. </summary>
		''' <param name="message"> the MIDI message to send </param>
		''' <param name="timeStamp"> the time-stamp for the message, in microseconds. </param>
		''' <exception cref="IllegalStateException"> if the receiver is closed </exception>
		Sub send(ByVal message As MidiMessage, ByVal timeStamp As Long)

		''' <summary>
		''' Indicates that the application has finished using the receiver, and
		''' that limited resources it requires may be released or made available.
		''' 
		''' <p>If the creation of this <code>Receiver</code> resulted in
		''' implicitly opening the underlying device, the device is
		''' implicitly closed by this method. This is true unless the device is
		''' kept open by other <code>Receiver</code> or <code>Transmitter</code>
		''' instances that opened the device implicitly, and unless the device
		''' has been opened explicitly. If the device this
		''' <code>Receiver</code> is retrieved from is closed explicitly by
		''' calling <seealso cref="MidiDevice#close MidiDevice.close"/>, the
		''' <code>Receiver</code> is closed, too.  For a detailed
		''' description of open/close behaviour see the class description
		''' of <seealso cref="javax.sound.midi.MidiDevice MidiDevice"/>.
		''' </summary>
		''' <seealso cref= javax.sound.midi.MidiSystem#getReceiver </seealso>
		Sub close()
	End Interface

End Namespace