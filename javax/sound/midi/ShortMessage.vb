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
	''' A <code>ShortMessage</code> contains a MIDI message that has at most
	''' two data bytes following its status byte.  The types of MIDI message
	''' that satisfy this criterion are channel voice, channel mode, system common,
	''' and system real-time--in other words, everything except system exclusive
	''' and meta-events.  The <code>ShortMessage</code> class provides methods
	''' for getting and setting the contents of the MIDI message.
	''' <p>
	''' A number of <code>ShortMessage</code> methods have integer parameters by which
	''' you specify a MIDI status or data byte.  If you know the numeric value, you
	''' can express it directly.  For system common and system real-time messages,
	''' you can often use the corresponding fields of <code>ShortMessage</code>, such as
	''' <seealso cref="#SYSTEM_RESET SYSTEM_RESET"/>.  For channel messages,
	''' the upper four bits of the status byte are specified by a command value and
	''' the lower four bits are specified by a MIDI channel number. To
	''' convert incoming MIDI data bytes that are in the form of Java's signed bytes,
	''' you can use the <A HREF="MidiMessage.html#integersVsBytes">conversion code</A>
	''' given in the <code><seealso cref="MidiMessage"/></code> class description.
	''' </summary>
	''' <seealso cref= SysexMessage </seealso>
	''' <seealso cref= MetaMessage
	''' 
	''' @author David Rivas
	''' @author Kara Kytle
	''' @author Florian Bomers </seealso>

	Public Class ShortMessage
		Inherits MidiMessage


		' Status byte defines


		' System common messages

		''' <summary>
		''' Status byte for MIDI Time Code Quarter Frame message (0xF1, or 241). </summary>
		''' <seealso cref= MidiMessage#getStatus </seealso>
		Public Const MIDI_TIME_CODE As Integer = &HF1 ' 241

		''' <summary>
		''' Status byte for Song Position Pointer message (0xF2, or 242). </summary>
		''' <seealso cref= MidiMessage#getStatus </seealso>
		Public Const SONG_POSITION_POINTER As Integer = &HF2 ' 242

		''' <summary>
		''' Status byte for MIDI Song Select message (0xF3, or 243). </summary>
		''' <seealso cref= MidiMessage#getStatus </seealso>
		Public Const SONG_SELECT As Integer = &HF3 ' 243

		''' <summary>
		''' Status byte for Tune Request message (0xF6, or 246). </summary>
		''' <seealso cref= MidiMessage#getStatus </seealso>
		Public Const TUNE_REQUEST As Integer = &HF6 ' 246

		''' <summary>
		''' Status byte for End of System Exclusive message (0xF7, or 247). </summary>
		''' <seealso cref= MidiMessage#getStatus </seealso>
		Public Const END_OF_EXCLUSIVE As Integer = &HF7 ' 247


		' System real-time messages

		''' <summary>
		''' Status byte for Timing Clock message (0xF8, or 248). </summary>
		''' <seealso cref= MidiMessage#getStatus </seealso>
		Public Const TIMING_CLOCK As Integer = &HF8 ' 248

		''' <summary>
		''' Status byte for Start message (0xFA, or 250). </summary>
		''' <seealso cref= MidiMessage#getStatus </seealso>
		Public Const START As Integer = &HFA ' 250

		''' <summary>
		''' Status byte for Continue message (0xFB, or 251). </summary>
		''' <seealso cref= MidiMessage#getStatus </seealso>
		Public Const [CONTINUE] As Integer = &HFB ' 251

		''' <summary>
		''' Status byte for Stop message (0xFC, or 252). </summary>
		''' <seealso cref= MidiMessage#getStatus </seealso>
		Public Const [STOP] As Integer = &HFC '252

		''' <summary>
		''' Status byte for Active Sensing message (0xFE, or 254). </summary>
		''' <seealso cref= MidiMessage#getStatus </seealso>
		Public Const ACTIVE_SENSING As Integer = &HFE ' 254

		''' <summary>
		''' Status byte for System Reset message (0xFF, or 255). </summary>
		''' <seealso cref= MidiMessage#getStatus </seealso>
		Public Const SYSTEM_RESET As Integer = &HFF ' 255


		' Channel voice message upper nibble defines

		''' <summary>
		''' Command value for Note Off message (0x80, or 128)
		''' </summary>
		Public Const NOTE_OFF As Integer = &H80 ' 128

		''' <summary>
		''' Command value for Note On message (0x90, or 144)
		''' </summary>
		Public Const NOTE_ON As Integer = &H90 ' 144

		''' <summary>
		''' Command value for Polyphonic Key Pressure (Aftertouch) message (0xA0, or 160)
		''' </summary>
		Public Const POLY_PRESSURE As Integer = &HA0 ' 160

		''' <summary>
		''' Command value for Control Change message (0xB0, or 176)
		''' </summary>
		Public Const CONTROL_CHANGE As Integer = &HB0 ' 176

		''' <summary>
		''' Command value for Program Change message (0xC0, or 192)
		''' </summary>
		Public Const PROGRAM_CHANGE As Integer = &HC0 ' 192

		''' <summary>
		''' Command value for Channel Pressure (Aftertouch) message (0xD0, or 208)
		''' </summary>
		Public Const CHANNEL_PRESSURE As Integer = &HD0 ' 208

		''' <summary>
		''' Command value for Pitch Bend message (0xE0, or 224)
		''' </summary>
		Public Const PITCH_BEND As Integer = &HE0 ' 224


		' Instance variables

		''' <summary>
		''' Constructs a new <code>ShortMessage</code>.  The
		''' contents of the new message are guaranteed to specify
		''' a valid MIDI message.  Subsequently, you may set the
		''' contents of the message using one of the <code>setMessage</code>
		''' methods. </summary>
		''' <seealso cref= #setMessage </seealso>
		Public Sub New()
			Me.New(New SByte(2){})
			' Default message data: NOTE_ON on Channel 0 with max volume
			data(0) = CByte(NOTE_ON And &HFF)
			data(1) = CByte(64)
			data(2) = CByte(127)
			length = 3
		End Sub

		''' <summary>
		''' Constructs a new {@code ShortMessage} which represents a MIDI
		''' message that takes no data bytes.
		''' The contents of the message can be changed by using one of
		''' the {@code setMessage} methods.
		''' </summary>
		''' <param name="status"> the MIDI status byte </param>
		''' <exception cref="InvalidMidiDataException"> if {@code status} does not specify
		'''     a valid MIDI status byte for a message that requires no data bytes </exception>
		''' <seealso cref= #setMessage(int) </seealso>
		''' <seealso cref= #setMessage(int, int, int) </seealso>
		''' <seealso cref= #setMessage(int, int, int, int) </seealso>
		''' <seealso cref= #getStatus()
		''' @since 1.7 </seealso>
		Public Sub New(ByVal status As Integer)
			MyBase.New(Nothing)
			message = status ' can throw InvalidMidiDataException
		End Sub

		''' <summary>
		''' Constructs a new {@code ShortMessage} which represents a MIDI message
		''' that takes up to two data bytes. If the message only takes one data byte,
		''' the second data byte is ignored. If the message does not take
		''' any data bytes, both data bytes are ignored.
		''' The contents of the message can be changed by using one of
		''' the {@code setMessage} methods.
		''' </summary>
		''' <param name="status">   the MIDI status byte </param>
		''' <param name="data1">    the first data byte </param>
		''' <param name="data2">    the second data byte </param>
		''' <exception cref="InvalidMidiDataException"> if the status byte or all data bytes
		'''     belonging to the message do not specify a valid MIDI message </exception>
		''' <seealso cref= #setMessage(int) </seealso>
		''' <seealso cref= #setMessage(int, int, int) </seealso>
		''' <seealso cref= #setMessage(int, int, int, int) </seealso>
		''' <seealso cref= #getStatus() </seealso>
		''' <seealso cref= #getData1() </seealso>
		''' <seealso cref= #getData2()
		''' @since 1.7 </seealso>
		Public Sub New(ByVal status As Integer, ByVal data1 As Integer, ByVal data2 As Integer)
			MyBase.New(Nothing)
			messageage(status, data1, data2) ' can throw InvalidMidiDataException
		End Sub

		''' <summary>
		''' Constructs a new {@code ShortMessage} which represents a channel
		''' MIDI message that takes up to two data bytes. If the message only takes
		''' one data byte, the second data byte is ignored. If the message does not
		''' take any data bytes, both data bytes are ignored.
		''' The contents of the message can be changed by using one of
		''' the {@code setMessage} methods.
		''' </summary>
		''' <param name="command">  the MIDI command represented by this message </param>
		''' <param name="channel">  the channel associated with the message </param>
		''' <param name="data1">    the first data byte </param>
		''' <param name="data2">    the second data byte </param>
		''' <exception cref="InvalidMidiDataException"> if the command value, channel value
		'''     or all data bytes belonging to the message do not specify
		'''     a valid MIDI message </exception>
		''' <seealso cref= #setMessage(int) </seealso>
		''' <seealso cref= #setMessage(int, int, int) </seealso>
		''' <seealso cref= #setMessage(int, int, int, int) </seealso>
		''' <seealso cref= #getCommand() </seealso>
		''' <seealso cref= #getChannel() </seealso>
		''' <seealso cref= #getData1() </seealso>
		''' <seealso cref= #getData2()
		''' @since 1.7 </seealso>
		Public Sub New(ByVal command As Integer, ByVal channel As Integer, ByVal data1 As Integer, ByVal data2 As Integer)
			MyBase.New(Nothing)
			messageage(command, channel, data1, data2)
		End Sub


		''' <summary>
		''' Constructs a new <code>ShortMessage</code>. </summary>
		''' <param name="data"> an array of bytes containing the complete message.
		''' The message data may be changed using the <code>setMessage</code>
		''' method. </param>
		''' <seealso cref= #setMessage </seealso>
		' $$fb this should throw an Exception in case of an illegal message!
		Protected Friend Sub New(ByVal data As SByte())
			' $$fb this may set an invalid message.
			' Can't correct without compromising compatibility
			MyBase.New(data)
		End Sub


		''' <summary>
		''' Sets the parameters for a MIDI message that takes no data bytes. </summary>
		''' <param name="status">    the MIDI status byte </param>
		''' <exception cref="InvalidMidiDataException"> if <code>status</code> does not
		''' specify a valid MIDI status byte for a message that requires no data bytes. </exception>
		''' <seealso cref= #setMessage(int, int, int) </seealso>
		''' <seealso cref= #setMessage(int, int, int, int) </seealso>
		Public Overridable Property message As Integer
			Set(ByVal status As Integer)
				' check for valid values
				Dim ___dataLength As Integer = getDataLength(status) ' can throw InvalidMidiDataException
				If ___dataLength <> 0 Then Throw New InvalidMidiDataException("Status byte; " & status & " requires " & ___dataLength & " data bytes")
				messageage(status, 0, 0)
			End Set
		End Property


		''' <summary>
		''' Sets the  parameters for a MIDI message that takes one or two data
		''' bytes.  If the message takes only one data byte, the second data
		''' byte is ignored; if the message does not take any data bytes, both
		''' data bytes are ignored.
		''' </summary>
		''' <param name="status">    the MIDI status byte </param>
		''' <param name="data1">             the first data byte </param>
		''' <param name="data2">             the second data byte </param>
		''' <exception cref="InvalidMidiDataException"> if the
		''' the status byte, or all data bytes belonging to the message, do
		''' not specify a valid MIDI message. </exception>
		''' <seealso cref= #setMessage(int, int, int, int) </seealso>
		''' <seealso cref= #setMessage(int) </seealso>
		Public Overridable Sub setMessage(ByVal status As Integer, ByVal data1 As Integer, ByVal data2 As Integer)
			' check for valid values
			Dim ___dataLength As Integer = getDataLength(status) ' can throw InvalidMidiDataException
			If ___dataLength > 0 Then
				If data1 < 0 OrElse data1 > 127 Then Throw New InvalidMidiDataException("data1 out of range: " & data1)
				If ___dataLength > 1 Then
					If data2 < 0 OrElse data2 > 127 Then Throw New InvalidMidiDataException("data2 out of range: " & data2)
				End If
			End If


			' set the length
			length = ___dataLength + 1
			' re-allocate array if ShortMessage(byte[]) constructor gave array with fewer elements
			If data Is Nothing OrElse data.Length < length Then data = New SByte(2){}

			' set the data
			data(0) = CByte(status And &HFF)
			If length > 1 Then
				data(1) = CByte(data1 And &HFF)
				If length > 2 Then data(2) = CByte(data2 And &HFF)
			End If
		End Sub


		''' <summary>
		''' Sets the short message parameters for a  channel message
		''' which takes up to two data bytes.  If the message only
		''' takes one data byte, the second data byte is ignored; if
		''' the message does not take any data bytes, both data bytes
		''' are ignored.
		''' </summary>
		''' <param name="command">   the MIDI command represented by this message </param>
		''' <param name="channel">   the channel associated with the message </param>
		''' <param name="data1">             the first data byte </param>
		''' <param name="data2">             the second data byte </param>
		''' <exception cref="InvalidMidiDataException"> if the
		''' status byte or all data bytes belonging to the message, do
		''' not specify a valid MIDI message
		''' </exception>
		''' <seealso cref= #setMessage(int, int, int) </seealso>
		''' <seealso cref= #setMessage(int) </seealso>
		''' <seealso cref= #getCommand </seealso>
		''' <seealso cref= #getChannel </seealso>
		''' <seealso cref= #getData1 </seealso>
		''' <seealso cref= #getData2 </seealso>
		Public Overridable Sub setMessage(ByVal command As Integer, ByVal channel As Integer, ByVal data1 As Integer, ByVal data2 As Integer)
			' check for valid values
			If command >= &HF0 OrElse command < &H80 Then Throw New InvalidMidiDataException("command out of range: 0x" & Integer.toHexString(command))
			If (channel And &HFFFFFFF0L) <> 0 Then ' <=> (channel<0 || channel>15) Throw New InvalidMidiDataException("channel out of range: " & channel)
			messageage((command And &HF0) Or (channel And &HF), data1, data2)
		End Sub


		''' <summary>
		''' Obtains the MIDI channel associated with this event.  This method
		''' assumes that the event is a MIDI channel message; if not, the return
		''' value will not be meaningful. </summary>
		''' <returns> MIDI channel associated with the message. </returns>
		''' <seealso cref= #setMessage(int, int, int, int) </seealso>
		Public Overridable Property channel As Integer
			Get
				' this returns 0 if an invalid message is set
				Return (status And &HF)
			End Get
		End Property


		''' <summary>
		''' Obtains the MIDI command associated with this event.  This method
		''' assumes that the event is a MIDI channel message; if not, the return
		''' value will not be meaningful. </summary>
		''' <returns> the MIDI command associated with this event </returns>
		''' <seealso cref= #setMessage(int, int, int, int) </seealso>
		Public Overridable Property command As Integer
			Get
				' this returns 0 if an invalid message is set
				Return (status And &HF0)
			End Get
		End Property


		''' <summary>
		''' Obtains the first data byte in the message. </summary>
		''' <returns> the value of the <code>data1</code> field </returns>
		''' <seealso cref= #setMessage(int, int, int) </seealso>
		Public Overridable Property data1 As Integer
			Get
				If length > 1 Then Return (data(1) And &HFF)
				Return 0
			End Get
		End Property


		''' <summary>
		''' Obtains the second data byte in the message. </summary>
		''' <returns> the value of the <code>data2</code> field </returns>
		''' <seealso cref= #setMessage(int, int, int) </seealso>
		Public Overridable Property data2 As Integer
			Get
				If length > 2 Then Return (data(2) And &HFF)
				Return 0
			End Get
		End Property


		''' <summary>
		''' Creates a new object of the same class and with the same contents
		''' as this object. </summary>
		''' <returns> a clone of this instance. </returns>
		Public Overrides Function clone() As Object
			Dim newData As SByte() = New SByte(length - 1){}
			Array.Copy(data, 0, newData, 0, newData.Length)

			Dim msg As New ShortMessage(newData)
			Return msg
		End Function


		''' <summary>
		''' Retrieves the number of data bytes associated with a particular
		''' status byte value. </summary>
		''' <param name="status"> status byte value, which must represent a short MIDI message </param>
		''' <returns> data length in bytes (0, 1, or 2) </returns>
		''' <exception cref="InvalidMidiDataException"> if the
		''' <code>status</code> argument does not represent the status byte for any
		''' short message </exception>
		Protected Friend Function getDataLength(ByVal status As Integer) As Integer
			' system common and system real-time messages
			Select Case status
			Case &HF6, &HF7, &HF8, &HF9, &HFA, &HFB, &HFC, &HFD, &HFE, &HFF ' Tune Request
				' System real-time messages
				Return 0
			Case &HF1, &HF3 ' MTC Quarter Frame
				Return 1
			Case &HF2 ' Song Position Pointer
				Return 2
			Case Else
			End Select

			' channel voice and mode messages
			Select Case status And &HF0
			Case &H80, &H90, &HA0, &HB0, &HE0
				Return 2
			Case &HC0, &HD0
				Return 1
			Case Else
				Throw New InvalidMidiDataException("Invalid status byte: " & status)
			End Select
		End Function
	End Class

End Namespace