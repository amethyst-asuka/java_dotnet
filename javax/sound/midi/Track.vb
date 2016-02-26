Imports System.Collections

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
	''' A MIDI track is an independent stream of MIDI events (time-stamped MIDI
	''' data) that can be stored along with other tracks in a standard MIDI file.
	''' The MIDI specification allows only 16 channels of MIDI data, but tracks
	''' are a way to get around this limitation.  A MIDI file can contain any number
	''' of tracks, each containing its own stream of up to 16 channels of MIDI data.
	''' <p>
	''' A <code>Track</code> occupies a middle level in the hierarchy of data played
	''' by a <code><seealso cref="Sequencer"/></code>: sequencers play sequences, which contain tracks,
	''' which contain MIDI events.  A sequencer may provide controls that mute
	''' or solo individual tracks.
	''' <p>
	''' The timing information and resolution for a track is controlled by and stored
	''' in the sequence containing the track. A given <code>Track</code>
	''' is considered to belong to the particular <code><seealso cref="Sequence"/></code> that
	''' maintains its timing. For this reason, a new (empty) track is created by calling the
	''' <code><seealso cref="Sequence#createTrack"/></code> method, rather than by directly invoking a
	''' <code>Track</code> constructor.
	''' <p>
	''' The <code>Track</code> class provides methods to edit the track by adding
	''' or removing <code>MidiEvent</code> objects from it.  These operations keep
	''' the event list in the correct time order.  Methods are also
	''' included to obtain the track's size, in terms of either the number of events
	''' it contains or its duration in ticks.
	''' </summary>
	''' <seealso cref= Sequencer#setTrackMute </seealso>
	''' <seealso cref= Sequencer#setTrackSolo
	''' 
	''' @author Kara Kytle
	''' @author Florian Bomers </seealso>
	Public Class Track

		' TODO: use arrays for faster access

		' the list containing the events
		Private eventsList As New ArrayList

		' use a hashset to detect duplicate events in add(MidiEvent)
		Private [set] As New HashSet

		Private eotEvent As MidiEvent


		''' <summary>
		''' Package-private constructor.  Constructs a new, empty Track object,
		''' which initially contains one event, the meta-event End of Track.
		''' </summary>
		Friend Sub New()
			' start with the end of track event
			Dim eot As MetaMessage = New ImmutableEndOfTrack
			eotEvent = New MidiEvent(eot, 0)
			eventsList.Add(eotEvent)
			[set].Add(eotEvent)
		End Sub

		''' <summary>
		''' Adds a new event to the track.  However, if the event is already
		''' contained in the track, it is not added again.  The list of events
		''' is kept in time order, meaning that this event inserted at the
		''' appropriate place in the list, not necessarily at the end.
		''' </summary>
		''' <param name="event"> the event to add </param>
		''' <returns> <code>true</code> if the event did not already exist in the
		''' track and was added, otherwise <code>false</code> </returns>
		Public Overridable Function add(ByVal [event] As MidiEvent) As Boolean
			If [event] Is Nothing Then Return False
			SyncLock eventsList

				If Not [set].Contains([event]) Then
					Dim eventsCount As Integer = eventsList.Count

					' get the last event
					Dim lastEvent As MidiEvent = Nothing
					If eventsCount > 0 Then lastEvent = CType(eventsList(eventsCount - 1), MidiEvent)
					' sanity check that we have a correct end-of-track
					If lastEvent IsNot eotEvent Then
						' if there is no eot event, add our immutable instance again
						If lastEvent IsNot Nothing Then
							' set eotEvent's tick to the last tick of the track
							eotEvent.tick = lastEvent.tick
						Else
							' if the events list is empty, just set the tick to 0
							eotEvent.tick = 0
						End If
						' we needn't check for a duplicate of eotEvent in "eventsList",
						' since then it would appear in the set.
						eventsList.Add(eotEvent)
						[set].Add(eotEvent)
						eventsCount = eventsList.Count
					End If

					' first see if we are trying to add
					' and endoftrack event.
					If com.sun.media.sound.MidiUtils.isMetaEndOfTrack([event].message) Then
						' since end of track event is useful
						' for delays at the end of a track, we want to keep
						' the tick value requested here if it is greater
						' than the one on the eot we are maintaining.
						' Otherwise, we only want a single eot event, so ignore.
						If [event].tick > eotEvent.tick Then eotEvent.tick = [event].tick
						Return True
					End If

					' prevent duplicates
					[set].Add([event])

					' insert event such that events is sorted in increasing
					' tick order
					Dim i As Integer = eventsCount
					Do While i > 0
						If [event].tick >= CType(eventsList(i-1), MidiEvent).tick Then Exit Do
						i -= 1
					Loop
					If i = eventsCount Then
						' we're adding an event after the
						' tick value of our eot, so push the eot out.
						' Always add at the end for better performance:
						' this saves all the checks and arraycopy when inserting

						' overwrite eot with new event
						eventsList(eventsCount - 1) = [event]
						' set new time of eot, if necessary
						If eotEvent.tick < [event].tick Then eotEvent.tick = [event].tick
						' add eot again at the end
						eventsList.Add(eotEvent)
					Else
						eventsList.Insert(i, [event])
					End If
					Return True
				End If
			End SyncLock

			Return False
		End Function


		''' <summary>
		''' Removes the specified event from the track. </summary>
		''' <param name="event"> the event to remove </param>
		''' <returns> <code>true</code> if the event existed in the track and was removed,
		''' otherwise <code>false</code> </returns>
		Public Overridable Function remove(ByVal [event] As MidiEvent) As Boolean

			' this implementation allows removing the EOT event.
			' pretty bad, but would probably be too risky to
			' change behavior now, in case someone does tricks like:
			'
			' while (track.size() > 0) track.remove(track.get(track.size() - 1));

			' also, would it make sense to adjust the EOT's time
			' to the last event, if the last non-EOT event is removed?
			' Or: document that the ticks() length will not be reduced
			' by deleting events (unless the EOT event is removed)
			SyncLock eventsList
				If [set].Remove([event]) Then
					Dim i As Integer = eventsList.IndexOf([event])
					If i >= 0 Then
						eventsList.RemoveAt(i)
						Return True
					End If
				End If
			End SyncLock
			Return False
		End Function


		''' <summary>
		''' Obtains the event at the specified index. </summary>
		''' <param name="index"> the location of the desired event in the event vector </param>
		''' <exception cref="ArrayIndexOutOfBoundsException">  if the
		''' specified index is negative or not less than the current size of
		''' this track. </exception>
		''' <seealso cref= #size </seealso>
		''' <returns> the event at the specified index </returns>
		Public Overridable Function [get](ByVal index As Integer) As MidiEvent
			Try
				SyncLock eventsList
					Return CType(eventsList(index), MidiEvent)
				End SyncLock
			Catch ioobe As System.IndexOutOfRangeException
				Throw New System.IndexOutOfRangeException(ioobe.Message)
			End Try
		End Function


		''' <summary>
		''' Obtains the number of events in this track. </summary>
		''' <returns> the size of the track's event vector </returns>
		Public Overridable Function size() As Integer
			SyncLock eventsList
				Return eventsList.Count
			End SyncLock
		End Function


		''' <summary>
		''' Obtains the length of the track, expressed in MIDI ticks.  (The
		''' duration of a tick in seconds is determined by the timing resolution
		''' of the <code>Sequence</code> containing this track, and also by
		''' the tempo of the music as set by the sequencer.) </summary>
		''' <returns> the duration, in ticks </returns>
		''' <seealso cref= Sequence#Sequence(float, int) </seealso>
		''' <seealso cref= Sequencer#setTempoInBPM(float) </seealso>
		''' <seealso cref= Sequencer#getTickPosition() </seealso>
		Public Overridable Function ticks() As Long
			Dim ret As Long = 0
			SyncLock eventsList
				If eventsList.Count > 0 Then ret = CType(eventsList(eventsList.Count - 1), MidiEvent).tick
			End SyncLock
			Return ret
		End Function

		Private Class ImmutableEndOfTrack
			Inherits MetaMessage

			Private Sub New()
				MyBase.New(New SByte(2){})
				data(0) = CByte(META)
				data(1) = com.sun.media.sound.MidiUtils.META_END_OF_TRACK_TYPE
				data(2) = 0
			End Sub

			Public Overrides Sub setMessage(ByVal type As Integer, ByVal data As SByte(), ByVal length As Integer)
				Throw New InvalidMidiDataException("cannot modify end of track message")
			End Sub
		End Class

	End Class

End Namespace