Imports System

'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' <code>MidiMessage</code> is the base class for MIDI messages.  They include
	''' not only the standard MIDI messages that a synthesizer can respond to, but also
	''' "meta-events" that can be used by sequencer programs.  There are meta-events
	''' for such information as lyrics, copyrights, tempo indications, time and key
	''' signatures, markers, etc.  For more information, see the Standard MIDI Files 1.0
	''' specification, which is part of the Complete MIDI 1.0 Detailed Specification
	''' published by the MIDI Manufacturer's Association
	''' (<a href = http://www.midi.org>http://www.midi.org</a>).
	''' <p>
	''' The base <code>MidiMessage</code> class provides access to three types of
	''' information about a MIDI message:
	''' <ul>
	''' <li>The messages's status byte</li>
	''' <li>The total length of the message in bytes (the status byte plus any data bytes)</li>
	''' <li>A byte array containing the complete message</li>
	''' </ul>
	''' 
	''' <code>MidiMessage</code> includes methods to get, but not set, these values.
	''' Setting them is a subclass responsibility.
	''' <p>
	''' <a name="integersVsBytes"></a>
	''' The MIDI standard expresses MIDI data in bytes.  However, because
	''' Java<sup>TM</sup> uses signed bytes, the Java Sound API uses integers
	''' instead of bytes when expressing MIDI data.  For example, the
	''' <seealso cref="#getStatus()"/> method of
	''' <code>MidiMessage</code> returns MIDI status bytes as integers.  If you are
	''' processing MIDI data that originated outside Java Sound and now
	''' is encoded as signed bytes, the bytes can
	''' can be converted to integers using this conversion:
	''' <center>{@code int i = (int)(byte & 0xFF)}</center>
	''' <p>
	''' If you simply need to pass a known MIDI byte value as a method parameter,
	''' it can be expressed directly as an integer, using (for example) decimal or
	''' hexadecimal notation.  For instance, to pass the "active sensing" status byte
	''' as the first argument to ShortMessage's
	''' <seealso cref="ShortMessage#setMessage(int) setMessage(int)"/>
	''' method, you can express it as 254 or 0xFE.
	''' </summary>
	''' <seealso cref= Track </seealso>
	''' <seealso cref= Sequence </seealso>
	''' <seealso cref= Receiver
	''' 
	''' @author David Rivas
	''' @author Kara Kytle </seealso>

	Public MustInherit Class MidiMessage
		Implements ICloneable

		' Instance variables

		''' <summary>
		''' The MIDI message data.  The first byte is the status
		''' byte for the message; subsequent bytes up to the length
		''' of the message are data bytes for this message. </summary>
		''' <seealso cref= #getLength </seealso>
		Protected Friend data As SByte()


		''' <summary>
		''' The number of bytes in the MIDI message, including the
		''' status byte and any data bytes. </summary>
		''' <seealso cref= #getLength </seealso>
		Protected Friend length As Integer = 0


		''' <summary>
		''' Constructs a new <code>MidiMessage</code>.  This protected
		''' constructor is called by concrete subclasses, which should
		''' ensure that the data array specifies a complete, valid MIDI
		''' message.
		''' </summary>
		''' <param name="data"> an array of bytes containing the complete message.
		''' The message data may be changed using the <code>setMessage</code>
		''' method.
		''' </param>
		''' <seealso cref= #setMessage </seealso>
		Protected Friend Sub New(ByVal data As SByte())
			Me.data = data
			If data IsNot Nothing Then Me.length = data.Length
		End Sub


		''' <summary>
		''' Sets the data for the MIDI message.   This protected
		''' method is called by concrete subclasses, which should
		''' ensure that the data array specifies a complete, valid MIDI
		''' message.
		''' </summary>
		''' <param name="data"> the data bytes in the MIDI message </param>
		''' <param name="length"> the number of bytes in the data byte array </param>
		''' <exception cref="InvalidMidiDataException"> if the parameter values do not specify a valid MIDI meta message </exception>
		Protected Friend Overridable Sub setMessage(ByVal data As SByte(), ByVal length As Integer)
			If length < 0 OrElse (length > 0 AndAlso length > data.Length) Then Throw New System.IndexOutOfRangeException("length out of bounds: " & length)
			Me.length = length

			If Me.data Is Nothing OrElse Me.data.Length < Me.length Then Me.data = New SByte(Me.length - 1){}
			Array.Copy(data, 0, Me.data, 0, length)
		End Sub


		''' <summary>
		''' Obtains the MIDI message data.  The first byte of the returned byte
		''' array is the status byte of the message.  Any subsequent bytes up to
		''' the length of the message are data bytes.  The byte array may have a
		''' length which is greater than that of the actual message; the total
		''' length of the message in bytes is reported by the <code><seealso cref="#getLength"/></code>
		''' method.
		''' </summary>
		''' <returns> the byte array containing the complete <code>MidiMessage</code> data </returns>
		Public Overridable Property message As SByte()
			Get
				Dim returnedArray As SByte() = New SByte(length - 1){}
				Array.Copy(data, 0, returnedArray, 0, length)
				Return returnedArray
			End Get
		End Property


		''' <summary>
		''' Obtains the status byte for the MIDI message.  The status "byte" is
		''' represented as an integer; see the
		''' <a href="#integersVsBytes">discussion</a> in the
		''' <code>MidiMessage</code> class description.
		''' </summary>
		''' <returns> the integer representation of this event's status byte </returns>
		Public Overridable Property status As Integer
			Get
				If length > 0 Then Return (data(0) And &HFF)
				Return 0
			End Get
		End Property


		''' <summary>
		''' Obtains the total length of the MIDI message in bytes.  A
		''' MIDI message consists of one status byte and zero or more
		''' data bytes.  The return value ranges from 1 for system real-time messages,
		''' to 2 or 3 for channel messages, to any value for meta and system
		''' exclusive messages.
		''' </summary>
		''' <returns> the length of the message in bytes </returns>
		Public Overridable Property length As Integer
			Get
				Return length
			End Get
		End Property


		''' <summary>
		''' Creates a new object of the same class and with the same contents
		''' as this object. </summary>
		''' <returns> a clone of this instance. </returns>
		Public MustOverride Function clone() As Object
	End Class

End Namespace