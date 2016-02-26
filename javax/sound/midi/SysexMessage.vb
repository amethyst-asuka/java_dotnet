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
	''' A <code>SysexMessage</code> object represents a MIDI system exclusive message.
	''' <p>
	''' When a system exclusive message is read from a MIDI file, it always has
	''' a defined length.  Data from a system exclusive message from a MIDI file
	''' should be stored in the data array of a <code>SysexMessage</code> as
	''' follows: the system exclusive message status byte (0xF0 or 0xF7), all
	''' message data bytes, and finally the end-of-exclusive flag (0xF7).
	''' The length reported by the <code>SysexMessage</code> object is therefore
	''' the length of the system exclusive data plus two: one byte for the status
	''' byte and one for the end-of-exclusive flag.
	''' <p>
	''' As dictated by the Standard MIDI Files specification, two status byte values are legal
	''' for a <code>SysexMessage</code> read from a MIDI file:
	''' <ul>
	''' <li>0xF0: System Exclusive message (same as in MIDI wire protocol)</li>
	''' <li>0xF7: Special System Exclusive message</li>
	''' </ul>
	''' <p>
	''' When Java Sound is used to handle system exclusive data that is being received
	''' using MIDI wire protocol, it should place the data in one or more
	''' <code>SysexMessages</code>.  In this case, the length of the system exclusive data
	''' is not known in advance; the end of the system exclusive data is marked by an
	''' end-of-exclusive flag (0xF7) in the MIDI wire byte stream.
	''' <ul>
	''' <li>0xF0: System Exclusive message (same as in MIDI wire protocol)</li>
	''' <li>0xF7: End of Exclusive (EOX)</li>
	''' </ul>
	''' The first <code>SysexMessage</code> object containing data for a particular system
	''' exclusive message should have the status value 0xF0.  If this message contains all
	''' the system exclusive data
	''' for the message, it should end with the status byte 0xF7 (EOX).
	''' Otherwise, additional system exclusive data should be sent in one or more
	''' <code>SysexMessages</code> with a status value of 0xF7.  The <code>SysexMessage</code>
	''' containing the last of the data for the system exclusive message should end with the
	''' value 0xF7 (EOX) to mark the end of the system exclusive message.
	''' <p>
	''' If system exclusive data from <code>SysexMessages</code> objects is being transmitted
	''' using MIDI wire protocol, only the initial 0xF0 status byte, the system exclusive
	''' data itself, and the final 0xF7 (EOX) byte should be propagated; any 0xF7 status
	''' bytes used to indicate that a <code>SysexMessage</code> contains continuing system
	''' exclusive data should not be propagated via MIDI wire protocol.
	''' 
	''' @author David Rivas
	''' @author Kara Kytle
	''' @author Florian Bomers
	''' </summary>
	Public Class SysexMessage
		Inherits MidiMessage


		' Status byte defines


		''' <summary>
		''' Status byte for System Exclusive message (0xF0, or 240). </summary>
		''' <seealso cref= MidiMessage#getStatus </seealso>
		Public Const SYSTEM_EXCLUSIVE As Integer = &HF0 ' 240


		''' <summary>
		''' Status byte for Special System Exclusive message (0xF7, or 247), which is used
		''' in MIDI files.  It has the same value as END_OF_EXCLUSIVE, which
		''' is used in the real-time "MIDI wire" protocol. </summary>
		''' <seealso cref= MidiMessage#getStatus </seealso>
		Public Const SPECIAL_SYSTEM_EXCLUSIVE As Integer = &HF7 ' 247


		' Instance variables


	'    
	'     * The data bytes for this system exclusive message.  These are
	'     * initialized to <code>null</code> and are set explicitly
	'     * by {@link #setMessage(int, byte[], int, long) setMessage}.
	'     
		'protected byte[] data = null;


		''' <summary>
		''' Constructs a new <code>SysexMessage</code>. The
		''' contents of the new message are guaranteed to specify
		''' a valid MIDI message.  Subsequently, you may set the
		''' contents of the message using one of the <code>setMessage</code>
		''' methods. </summary>
		''' <seealso cref= #setMessage </seealso>
		Public Sub New()
			Me.New(New SByte(1){})
			' Default sysex message data: SOX followed by EOX
			data(0) = CByte(SYSTEM_EXCLUSIVE And &HFF)
			data(1) = CByte(ShortMessage.END_OF_EXCLUSIVE And &HFF)
		End Sub

		''' <summary>
		''' Constructs a new {@code SysexMessage} and sets the data for
		''' the message. The first byte of the data array must be a valid system
		''' exclusive status byte (0xF0 or 0xF7).
		''' The contents of the message can be changed by using one of
		''' the {@code setMessage} methods.
		''' </summary>
		''' <param name="data"> the system exclusive message data including the status byte </param>
		''' <param name="length"> the length of the valid message data in the array,
		'''     including the status byte; it should be non-negative and less than
		'''     or equal to {@code data.length} </param>
		''' <exception cref="InvalidMidiDataException"> if the parameter values
		'''     do not specify a valid MIDI meta message. </exception>
		''' <seealso cref= #setMessage(byte[], int) </seealso>
		''' <seealso cref= #setMessage(int, byte[], int) </seealso>
		''' <seealso cref= #getData()
		''' @since 1.7 </seealso>
		Public Sub New(ByVal data As SByte(), ByVal length As Integer)
			MyBase.New(Nothing)
			messageage(data, length)
		End Sub

		''' <summary>
		''' Constructs a new {@code SysexMessage} and sets the data for the message.
		''' The contents of the message can be changed by using one of
		''' the {@code setMessage} methods.
		''' </summary>
		''' <param name="status"> the status byte for the message; it must be a valid system
		'''     exclusive status byte (0xF0 or 0xF7) </param>
		''' <param name="data"> the system exclusive message data (without the status byte) </param>
		''' <param name="length"> the length of the valid message data in the array;
		'''     it should be non-negative and less than or equal to
		'''     {@code data.length} </param>
		''' <exception cref="InvalidMidiDataException"> if the parameter values
		'''     do not specify a valid MIDI meta message. </exception>
		''' <seealso cref= #setMessage(byte[], int) </seealso>
		''' <seealso cref= #setMessage(int, byte[], int) </seealso>
		''' <seealso cref= #getData()
		''' @since 1.7 </seealso>
		Public Sub New(ByVal status As Integer, ByVal data As SByte(), ByVal length As Integer)
			MyBase.New(Nothing)
			messageage(status, data, length)
		End Sub


		''' <summary>
		''' Constructs a new <code>SysexMessage</code>. </summary>
		''' <param name="data"> an array of bytes containing the complete message.
		''' The message data may be changed using the <code>setMessage</code>
		''' method. </param>
		''' <seealso cref= #setMessage </seealso>
		Protected Friend Sub New(ByVal data As SByte())
			MyBase.New(data)
		End Sub


		''' <summary>
		''' Sets the data for the system exclusive message.   The
		''' first byte of the data array must be a valid system
		''' exclusive status byte (0xF0 or 0xF7). </summary>
		''' <param name="data"> the system exclusive message data </param>
		''' <param name="length"> the length of the valid message data in
		''' the array, including the status byte. </param>
		Public Overrides Sub setMessage(ByVal data As SByte(), ByVal length As Integer)
			Dim ___status As Integer = (data(0) And &HFF)
			If (___status <> &HF0) AndAlso (___status <> &HF7) Then Throw New InvalidMidiDataException("Invalid status byte for sysex message: 0x" & Integer.toHexString(___status))
			MyBase.messageage(data, length)
		End Sub


		''' <summary>
		''' Sets the data for the system exclusive message. </summary>
		''' <param name="status"> the status byte for the message (0xF0 or 0xF7) </param>
		''' <param name="data"> the system exclusive message data </param>
		''' <param name="length"> the length of the valid message data in
		''' the array </param>
		''' <exception cref="InvalidMidiDataException"> if the status byte is invalid for a sysex message </exception>
		Public Overridable Sub setMessage(ByVal status As Integer, ByVal data As SByte(), ByVal length As Integer)
			If (status <> &HF0) AndAlso (status <> &HF7) Then Throw New InvalidMidiDataException("Invalid status byte for sysex message: 0x" & Integer.toHexString(status))
			If length < 0 OrElse length > data.Length Then Throw New System.IndexOutOfRangeException("length out of bounds: " & length)
			Me.length = length + 1

			If Me.data Is Nothing OrElse Me.data.Length < Me.length Then Me.data = New SByte(Me.length - 1){}

			Me.data(0) = CByte(status And &HFF)
			If length > 0 Then Array.Copy(data, 0, Me.data, 1, length)
		End Sub


		''' <summary>
		''' Obtains a copy of the data for the system exclusive message.
		''' The returned array of bytes does not include the status byte. </summary>
		''' <returns> array containing the system exclusive message data. </returns>
		Public Overridable Property data As SByte()
			Get
				Dim returnedArray As SByte() = New SByte(length - 2){}
				Array.Copy(data, 1, returnedArray, 0, (length - 1))
				Return returnedArray
			End Get
		End Property


		''' <summary>
		''' Creates a new object of the same class and with the same contents
		''' as this object. </summary>
		''' <returns> a clone of this instance </returns>
		Public Overrides Function clone() As Object
			Dim newData As SByte() = New SByte(length - 1){}
			Array.Copy(data, 0, newData, 0, newData.Length)
			Dim [event] As New SysexMessage(newData)
			Return [event]
		End Function
	End Class

End Namespace