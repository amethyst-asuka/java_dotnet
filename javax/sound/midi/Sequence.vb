Imports System.Collections.Generic

'
' * Copyright (c) 1999, 2004, Oracle and/or its affiliates. All rights reserved.
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
	''' A <code>Sequence</code> is a data structure containing musical
	''' information (often an entire song or composition) that can be played
	''' back by a <code><seealso cref="Sequencer"/></code> object. Specifically, the
	''' <code>Sequence</code> contains timing
	''' information and one or more tracks.  Each <code><seealso cref="Track track"/></code> consists of a
	''' series of MIDI events (such as note-ons, note-offs, program changes, and meta-events).
	''' The sequence's timing information specifies the type of unit that is used
	''' to time-stamp the events in the sequence.
	''' <p>
	''' A <code>Sequence</code> can be created from a MIDI file by reading the file
	''' into an input stream and invoking one of the <code>getSequence</code> methods of
	''' <seealso cref="MidiSystem"/>.  A sequence can also be built from scratch by adding new
	''' <code>Tracks</code> to an empty <code>Sequence</code>, and adding
	''' <code><seealso cref="MidiEvent"/></code> objects to these <code>Tracks</code>.
	''' </summary>
	''' <seealso cref= Sequencer#setSequence(java.io.InputStream stream) </seealso>
	''' <seealso cref= Sequencer#setSequence(Sequence sequence) </seealso>
	''' <seealso cref= Track#add(MidiEvent) </seealso>
	''' <seealso cref= MidiFileFormat
	''' 
	''' @author Kara Kytle </seealso>
	Public Class Sequence


		' Timing types

		''' <summary>
		''' The tempo-based timing type, for which the resolution is expressed in pulses (ticks) per quarter note. </summary>
		''' <seealso cref= #Sequence(float, int) </seealso>
		Public Const PPQ As Single = 0.0f

		''' <summary>
		''' The SMPTE-based timing type with 24 frames per second (resolution is expressed in ticks per frame). </summary>
		''' <seealso cref= #Sequence(float, int) </seealso>
		Public Const SMPTE_24 As Single = 24.0f

		''' <summary>
		''' The SMPTE-based timing type with 25 frames per second (resolution is expressed in ticks per frame). </summary>
		''' <seealso cref= #Sequence(float, int) </seealso>
		Public Const SMPTE_25 As Single = 25.0f

		''' <summary>
		''' The SMPTE-based timing type with 29.97 frames per second (resolution is expressed in ticks per frame). </summary>
		''' <seealso cref= #Sequence(float, int) </seealso>
		Public Const SMPTE_30DROP As Single = 29.97f

		''' <summary>
		''' The SMPTE-based timing type with 30 frames per second (resolution is expressed in ticks per frame). </summary>
		''' <seealso cref= #Sequence(float, int) </seealso>
		Public Const SMPTE_30 As Single = 30.0f


		' Variables

		''' <summary>
		''' The timing division type of the sequence. </summary>
		''' <seealso cref= #PPQ </seealso>
		''' <seealso cref= #SMPTE_24 </seealso>
		''' <seealso cref= #SMPTE_25 </seealso>
		''' <seealso cref= #SMPTE_30DROP </seealso>
		''' <seealso cref= #SMPTE_30 </seealso>
		''' <seealso cref= #getDivisionType </seealso>
		Protected Friend divisionType As Single

		''' <summary>
		''' The timing resolution of the sequence. </summary>
		''' <seealso cref= #getResolution </seealso>
		Protected Friend resolution As Integer

		''' <summary>
		''' The MIDI tracks in this sequence. </summary>
		''' <seealso cref= #getTracks </seealso>
		Protected Friend tracks As New List(Of Track)


		''' <summary>
		''' Constructs a new MIDI sequence with the specified timing division
		''' type and timing resolution.  The division type must be one of the
		''' recognized MIDI timing types.  For tempo-based timing,
		''' <code>divisionType</code> is PPQ (pulses per quarter note) and
		''' the resolution is specified in ticks per beat.  For SMTPE timing,
		''' <code>divisionType</code> specifies the number of frames per
		''' second and the resolution is specified in ticks per frame.
		''' The sequence will contain no initial tracks.  Tracks may be
		''' added to or removed from the sequence using <code><seealso cref="#createTrack"/></code>
		''' and <code><seealso cref="#deleteTrack"/></code>.
		''' </summary>
		''' <param name="divisionType"> the timing division type (PPQ or one of the SMPTE types) </param>
		''' <param name="resolution"> the timing resolution </param>
		''' <exception cref="InvalidMidiDataException"> if <code>divisionType</code> is not valid
		''' </exception>
		''' <seealso cref= #PPQ </seealso>
		''' <seealso cref= #SMPTE_24 </seealso>
		''' <seealso cref= #SMPTE_25 </seealso>
		''' <seealso cref= #SMPTE_30DROP </seealso>
		''' <seealso cref= #SMPTE_30 </seealso>
		''' <seealso cref= #getDivisionType </seealso>
		''' <seealso cref= #getResolution </seealso>
		''' <seealso cref= #getTracks </seealso>
		Public Sub New(ByVal divisionType As Single, ByVal resolution As Integer)

			If divisionType = PPQ Then
				Me.divisionType = PPQ
			ElseIf divisionType = SMPTE_24 Then
				Me.divisionType = SMPTE_24
			ElseIf divisionType = SMPTE_25 Then
				Me.divisionType = SMPTE_25
			ElseIf divisionType = SMPTE_30DROP Then
				Me.divisionType = SMPTE_30DROP
			ElseIf divisionType = SMPTE_30 Then
				Me.divisionType = SMPTE_30
			Else
				Throw New InvalidMidiDataException("Unsupported division type: " & divisionType)
			End If

			Me.resolution = resolution
		End Sub


		''' <summary>
		''' Constructs a new MIDI sequence with the specified timing division
		''' type, timing resolution, and number of tracks.  The division type must be one of the
		''' recognized MIDI timing types.  For tempo-based timing,
		''' <code>divisionType</code> is PPQ (pulses per quarter note) and
		''' the resolution is specified in ticks per beat.  For SMTPE timing,
		''' <code>divisionType</code> specifies the number of frames per
		''' second and the resolution is specified in ticks per frame.
		''' The sequence will be initialized with the number of tracks specified by
		''' <code>numTracks</code>. These tracks are initially empty (i.e.
		''' they contain only the meta-event End of Track).
		''' The tracks may be retrieved for editing using the <code><seealso cref="#getTracks"/></code>
		''' method.  Additional tracks may be added, or existing tracks removed,
		''' using <code><seealso cref="#createTrack"/></code> and <code><seealso cref="#deleteTrack"/></code>.
		''' </summary>
		''' <param name="divisionType"> the timing division type (PPQ or one of the SMPTE types) </param>
		''' <param name="resolution"> the timing resolution </param>
		''' <param name="numTracks"> the initial number of tracks in the sequence. </param>
		''' <exception cref="InvalidMidiDataException"> if <code>divisionType</code> is not valid
		''' </exception>
		''' <seealso cref= #PPQ </seealso>
		''' <seealso cref= #SMPTE_24 </seealso>
		''' <seealso cref= #SMPTE_25 </seealso>
		''' <seealso cref= #SMPTE_30DROP </seealso>
		''' <seealso cref= #SMPTE_30 </seealso>
		''' <seealso cref= #getDivisionType </seealso>
		''' <seealso cref= #getResolution </seealso>
		Public Sub New(ByVal divisionType As Single, ByVal resolution As Integer, ByVal numTracks As Integer)

			If divisionType = PPQ Then
				Me.divisionType = PPQ
			ElseIf divisionType = SMPTE_24 Then
				Me.divisionType = SMPTE_24
			ElseIf divisionType = SMPTE_25 Then
				Me.divisionType = SMPTE_25
			ElseIf divisionType = SMPTE_30DROP Then
				Me.divisionType = SMPTE_30DROP
			ElseIf divisionType = SMPTE_30 Then
				Me.divisionType = SMPTE_30
			Else
				Throw New InvalidMidiDataException("Unsupported division type: " & divisionType)
			End If

			Me.resolution = resolution

			For i As Integer = 0 To numTracks - 1
				tracks.Add(New Track)
			Next i
		End Sub


		''' <summary>
		''' Obtains the timing division type for this sequence. </summary>
		''' <returns> the division type (PPQ or one of the SMPTE types)
		''' </returns>
		''' <seealso cref= #PPQ </seealso>
		''' <seealso cref= #SMPTE_24 </seealso>
		''' <seealso cref= #SMPTE_25 </seealso>
		''' <seealso cref= #SMPTE_30DROP </seealso>
		''' <seealso cref= #SMPTE_30 </seealso>
		''' <seealso cref= #Sequence(float, int) </seealso>
		''' <seealso cref= MidiFileFormat#getDivisionType() </seealso>
		Public Overridable Property divisionType As Single
			Get
				Return divisionType
			End Get
		End Property


		''' <summary>
		''' Obtains the timing resolution for this sequence.
		''' If the sequence's division type is PPQ, the resolution is specified in ticks per beat.
		''' For SMTPE timing, the resolution is specified in ticks per frame.
		''' </summary>
		''' <returns> the number of ticks per beat (PPQ) or per frame (SMPTE) </returns>
		''' <seealso cref= #getDivisionType </seealso>
		''' <seealso cref= #Sequence(float, int) </seealso>
		''' <seealso cref= MidiFileFormat#getResolution() </seealso>
		Public Overridable Property resolution As Integer
			Get
				Return resolution
			End Get
		End Property


		''' <summary>
		''' Creates a new, initially empty track as part of this sequence.
		''' The track initially contains the meta-event End of Track.
		''' The newly created track is returned.  All tracks in the sequence
		''' may be retrieved using <code><seealso cref="#getTracks"/></code>.  Tracks may be
		''' removed from the sequence using <code><seealso cref="#deleteTrack"/></code>. </summary>
		''' <returns> the newly created track </returns>
		Public Overridable Function createTrack() As Track

			Dim track As New Track
			tracks.Add(track)

			Return track
		End Function


		''' <summary>
		''' Removes the specified track from the sequence. </summary>
		''' <param name="track"> the track to remove </param>
		''' <returns> <code>true</code> if the track existed in the track and was removed,
		''' otherwise <code>false</code>.
		''' </returns>
		''' <seealso cref= #createTrack </seealso>
		''' <seealso cref= #getTracks </seealso>
		Public Overridable Function deleteTrack(ByVal track As Track) As Boolean

			SyncLock tracks

				Return tracks.Remove(track)
			End SyncLock
		End Function


		''' <summary>
		''' Obtains an array containing all the tracks in this sequence.
		''' If the sequence contains no tracks, an array of length 0 is returned. </summary>
		''' <returns> the array of tracks
		''' </returns>
		''' <seealso cref= #createTrack </seealso>
		''' <seealso cref= #deleteTrack </seealso>
		Public Overridable Property tracks As Track()
			Get
    
				Return CType(tracks.ToArray(), Track())
			End Get
		End Property


		''' <summary>
		''' Obtains the duration of this sequence, expressed in microseconds. </summary>
		''' <returns> this sequence's duration in microseconds. </returns>
		Public Overridable Property microsecondLength As Long
			Get
    
				Return com.sun.media.sound.MidiUtils.tick2microsecond(Me, tickLength, Nothing)
			End Get
		End Property


		''' <summary>
		''' Obtains the duration of this sequence, expressed in MIDI ticks.
		''' </summary>
		''' <returns> this sequence's length in ticks
		''' </returns>
		''' <seealso cref= #getMicrosecondLength </seealso>
		Public Overridable Property tickLength As Long
			Get
    
				Dim length As Long = 0
    
				SyncLock tracks
    
					For i As Integer = 0 To tracks.Count - 1
						Dim temp As Long = CType(tracks(i), Track).ticks()
						If temp>length Then length = temp
					Next i
					Return length
				End SyncLock
			End Get
		End Property


		''' <summary>
		''' Obtains a list of patches referenced in this sequence.
		''' This patch list may be used to load the required
		''' <code><seealso cref="Instrument"/></code> objects
		''' into a <code><seealso cref="Synthesizer"/></code>.
		''' </summary>
		''' <returns> an array of <code><seealso cref="Patch"/></code> objects used in this sequence
		''' </returns>
		''' <seealso cref= Synthesizer#loadInstruments(Soundbank, Patch[]) </seealso>
		Public Overridable Property patchList As Patch()
			Get
    
				' $$kk: 04.09.99: need to implement!!
				Return New Patch(){}
			End Get
		End Property
	End Class

End Namespace