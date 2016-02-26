Imports System

'
' * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' A <code>MetaMessage</code> is a <code><seealso cref="MidiMessage"/></code> that is not meaningful to synthesizers, but
	''' that can be stored in a MIDI file and interpreted by a sequencer program.
	''' (See the discussion in the <code>MidiMessage</code>
	''' class description.)  The Standard MIDI Files specification defines
	''' various types of meta-events, such as sequence number, lyric, cue point,
	''' and set tempo.  There are also meta-events
	''' for such information as lyrics, copyrights, tempo indications, time and key
	''' signatures, markers, etc.  For more information, see the Standard MIDI Files 1.0
	''' specification, which is part of the Complete MIDI 1.0 Detailed Specification
	''' published by the MIDI Manufacturer's Association
	''' (<a href = http://www.midi.org>http://www.midi.org</a>).
	''' 
	''' <p>
	''' When data is being transported using MIDI wire protocol,
	''' a <code><seealso cref="ShortMessage"/></code> with the status value <code>0xFF</code> represents
	''' a system reset message.  In MIDI files, this same status value denotes a <code>MetaMessage</code>.
	''' The types of meta-message are distinguished from each other by the first byte
	''' that follows the status byte <code>0xFF</code>.  The subsequent bytes are data
	''' bytes.  As with system exclusive messages, there are an arbitrary number of
	''' data bytes, depending on the type of <code>MetaMessage</code>.
	''' </summary>
	''' <seealso cref= MetaEventListener
	''' 
	''' @author David Rivas
	''' @author Kara Kytle </seealso>

	Public Class MetaMessage
		Inherits MidiMessage


		' Status byte defines

		''' <summary>
		''' Status byte for <code>MetaMessage</code> (0xFF, or 255), which is used
		''' in MIDI files.  It has the same value as SYSTEM_RESET, which
		''' is used in the real-time "MIDI wire" protocol. </summary>
		''' <seealso cref= MidiMessage#getStatus </seealso>
		Public Const META As Integer = &HFF ' 255

		' Instance variables

		''' <summary>
		''' The length of the actual message in the data array.
		''' This is used to determine how many bytes of the data array
		''' is the message, and how many are the status byte, the
		''' type byte, and the variable-length-int describing the
		''' length of the message.
		''' </summary>
		Private dataLength As Integer = 0


		''' <summary>
		''' Constructs a new <code>MetaMessage</code>. The contents of
		''' the message are not set here; use
		''' <seealso cref="#setMessage(int, byte[], int) setMessage"/>
		''' to set them subsequently.
		''' </summary>
		Public Sub New()
			' Default meta message data: just the META status byte value
			Me.New(New SByte(){CByte(META), 0})
		End Sub

		''' <summary>
		''' Constructs a new {@code MetaMessage} and sets the message parameters.
		''' The contents of the message can be changed by using
		''' the {@code setMessage} method.
		''' </summary>
		''' <param name="type">   meta-message type (must be less than 128) </param>
		''' <param name="data">   the data bytes in the MIDI message </param>
		''' <param name="length"> an amount of bytes in the {@code data} byte array;
		'''     it should be non-negative and less than or equal to
		'''     {@code data.length} </param>
		''' <exception cref="InvalidMidiDataException"> if the parameter values do not specify
		'''     a valid MIDI meta message </exception>
		''' <seealso cref= #setMessage(int, byte[], int) </seealso>
		''' <seealso cref= #getType() </seealso>
		''' <seealso cref= #getData()
		''' @since 1.7 </seealso>
		Public Sub New(ByVal type As Integer, ByVal data As SByte(), ByVal length As Integer)
			MyBase.New(Nothing)
			messageage(type, data, length) ' can throw InvalidMidiDataException
		End Sub


		''' <summary>
		''' Constructs a new <code>MetaMessage</code>. </summary>
		''' <param name="data"> an array of bytes containing the complete message.
		''' The message data may be changed using the <code>setMessage</code>
		''' method. </param>
		''' <seealso cref= #setMessage </seealso>
		Protected Friend Sub New(ByVal data As SByte())
			MyBase.New(data)
			'$$fb 2001-10-06: need to calculate dataLength. Fix for bug #4511796
			If data.Length>=3 Then
				dataLength=data.Length-3
				Dim pos As Integer=2
				Do While pos<data.Length AndAlso (data(pos) And &H80)<>0
					dataLength -= 1
					pos += 1
				Loop
			End If
		End Sub


		''' <summary>
		''' Sets the message parameters for a <code>MetaMessage</code>.
		''' Since only one status byte value, <code>0xFF</code>, is allowed for meta-messages,
		''' it does not need to be specified here.  Calls to <code><seealso cref="MidiMessage#getStatus getStatus"/></code> return
		''' <code>0xFF</code> for all meta-messages.
		''' <p>
		''' The <code>type</code> argument should be a valid value for the byte that
		''' follows the status byte in the <code>MetaMessage</code>.  The <code>data</code> argument
		''' should contain all the subsequent bytes of the <code>MetaMessage</code>.  In other words,
		''' the byte that specifies the type of <code>MetaMessage</code> is not considered a data byte.
		''' </summary>
		''' <param name="type">              meta-message type (must be less than 128) </param>
		''' <param name="data">              the data bytes in the MIDI message </param>
		''' <param name="length">    the number of bytes in the <code>data</code>
		''' byte array </param>
		''' <exception cref="InvalidMidiDataException">  if the
		''' parameter values do not specify a valid MIDI meta message </exception>
		Public Overridable Sub setMessage(ByVal type As Integer, ByVal data As SByte(), ByVal length As Integer)

			If type >= 128 OrElse type < 0 Then Throw New InvalidMidiDataException("Invalid meta event with type " & type)
			If (length > 0 AndAlso length > data.Length) OrElse length < 0 Then Throw New InvalidMidiDataException("length out of bounds: " & length)

			Me.length = 2 + getVarIntLength(length) + length
			Me.dataLength = length
			Me.data = New SByte(Me.length - 1){}
			Me.data(0) = CByte(META) ' status value for MetaMessages (meta events)
			Me.data(1) = CByte(type) ' MetaMessage type
			writeVarInt(Me.data, 2, length) ' write the length as a variable int
			If length > 0 Then Array.Copy(data, 0, Me.data, Me.length - Me.dataLength, Me.dataLength)
		End Sub


		''' <summary>
		''' Obtains the type of the <code>MetaMessage</code>. </summary>
		''' <returns> an integer representing the <code>MetaMessage</code> type </returns>
		Public Overridable Property type As Integer
			Get
				If length>=2 Then Return data(1) And &HFF
				Return 0
			End Get
		End Property



		''' <summary>
		''' Obtains a copy of the data for the meta message.  The returned
		''' array of bytes does not include the status byte or the message
		''' length data.  The length of the data for the meta message is
		''' the length of the array.  Note that the length of the entire
		''' message includes the status byte and the meta message type
		''' byte, and therefore may be longer than the returned array. </summary>
		''' <returns> array containing the meta message data. </returns>
		''' <seealso cref= MidiMessage#getLength </seealso>
		Public Overridable Property data As SByte()
			Get
				Dim returnedArray As SByte() = New SByte(dataLength - 1){}
				Array.Copy(data, (length - dataLength), returnedArray, 0, dataLength)
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

			Dim [event] As New MetaMessage(newData)
			Return [event]
		End Function

		' HELPER METHODS

		Private Function getVarIntLength(ByVal value As Long) As Integer
			Dim ___length As Integer = 0
			Do
				value = value >> 7
				___length += 1
			Loop While value > 0
			Return ___length
		End Function

		Private Const mask As Long = &H7F

		Private Sub writeVarInt(ByVal data As SByte(), ByVal [off] As Integer, ByVal value As Long)
			Dim shift As Integer=63 ' number of bitwise left-shifts of mask
			' first screen out leading zeros
			Do While (shift > 0) AndAlso ((value And (mask << shift)) = 0)
				shift-=7
			Loop
			' then write actual values
			Do While shift > 0
				data([off])=CByte(((value And (mask << shift)) >> shift) Or &H80)
				[off] += 1
				shift-=7
			Loop
			data([off]) = CByte(value And mask)
		End Sub

	End Class

End Namespace