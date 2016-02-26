'
' * Copyright (c) 1999, 2003, Oracle and/or its affiliates. All rights reserved.
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
	''' A hardware or software device that plays back a MIDI
	''' <code><seealso cref="Sequence sequence"/></code> is known as a <em>sequencer</em>.
	''' A MIDI sequence contains lists of time-stamped MIDI data, such as
	''' might be read from a standard MIDI file.  Most
	''' sequencers also provide functions for creating and editing sequences.
	''' <p>
	''' The <code>Sequencer</code> interface includes methods for the following
	''' basic MIDI sequencer operations:
	''' <ul>
	''' <li>obtaining a sequence from MIDI file data</li>
	''' <li>starting and stopping playback</li>
	''' <li>moving to an arbitrary position in the sequence</li>
	''' <li>changing the tempo (speed) of playback</li>
	''' <li>synchronizing playback to an internal clock or to received MIDI
	''' messages</li>
	''' <li>controlling the timing of another device</li>
	''' </ul>
	''' In addition, the following operations are supported, either directly, or
	''' indirectly through objects that the <code>Sequencer</code> has access to:
	''' <ul>
	''' <li>editing the data by adding or deleting individual MIDI events or entire
	''' tracks</li>
	''' <li>muting or soloing individual tracks in the sequence</li>
	''' <li>notifying listener objects about any meta-events or
	''' control-change events encountered while playing back the sequence.</li>
	''' </ul>
	''' </summary>
	''' <seealso cref= Sequencer.SyncMode </seealso>
	''' <seealso cref= #addMetaEventListener </seealso>
	''' <seealso cref= ControllerEventListener </seealso>
	''' <seealso cref= Receiver </seealso>
	''' <seealso cref= Transmitter </seealso>
	''' <seealso cref= MidiDevice
	''' 
	''' @author Kara Kytle
	''' @author Florian Bomers </seealso>
	Public Interface Sequencer
		Inherits MidiDevice


		''' <summary>
		''' A value indicating that looping should continue
		''' indefinitely rather than complete after a specific
		''' number of loops.
		''' </summary>
		''' <seealso cref= #setLoopCount
		''' @since 1.5 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int LOOP_CONTINUOUSLY = -1;



		''' <summary>
		''' Sets the current sequence on which the sequencer operates.
		''' 
		''' <p>This method can be called even if the
		''' <code>Sequencer</code> is closed.
		''' </summary>
		''' <param name="sequence"> the sequence to be loaded. </param>
		''' <exception cref="InvalidMidiDataException"> if the sequence contains invalid
		''' MIDI data, or is not supported. </exception>
		Property sequence As Sequence


		''' <summary>
		''' Sets the current sequence on which the sequencer operates.
		''' The stream must point to MIDI file data.
		''' 
		''' <p>This method can be called even if the
		''' <code>Sequencer</code> is closed.
		''' </summary>
		''' <param name="stream"> stream containing MIDI file data. </param>
		''' <exception cref="IOException"> if an I/O exception occurs during reading of the stream. </exception>
		''' <exception cref="InvalidMidiDataException"> if invalid data is encountered
		''' in the stream, or the stream is not supported. </exception>
		WriteOnly Property sequence As java.io.InputStream




		''' <summary>
		''' Starts playback of the MIDI data in the currently
		''' loaded sequence.
		''' Playback will begin from the current position.
		''' If the playback position reaches the loop end point,
		''' and the loop count is greater than 0, playback will
		''' resume at the loop start point for the number of
		''' repetitions set with <code>setLoopCount</code>.
		''' After that, or if the loop count is 0, playback will
		''' continue to play to the end of the sequence.
		''' 
		''' <p>The implementation ensures that the synthesizer
		''' is brought to a consistent state when jumping
		''' to the loop start point by sending appropriate
		''' controllers, pitch bend, and program change events.
		''' </summary>
		''' <exception cref="IllegalStateException"> if the <code>Sequencer</code> is
		''' closed.
		''' </exception>
		''' <seealso cref= #setLoopStartPoint </seealso>
		''' <seealso cref= #setLoopEndPoint </seealso>
		''' <seealso cref= #setLoopCount </seealso>
		''' <seealso cref= #stop </seealso>
		Sub start()


		''' <summary>
		''' Stops recording, if active, and playback of the currently loaded sequence,
		''' if any.
		''' </summary>
		''' <exception cref="IllegalStateException"> if the <code>Sequencer</code> is
		''' closed.
		''' </exception>
		''' <seealso cref= #start </seealso>
		''' <seealso cref= #isRunning </seealso>
		Sub [stop]()


		''' <summary>
		''' Indicates whether the Sequencer is currently running.  The default is <code>false</code>.
		''' The Sequencer starts running when either <code><seealso cref="#start"/></code> or <code><seealso cref="#startRecording"/></code>
		''' is called.  <code>isRunning</code> then returns <code>true</code> until playback of the
		''' sequence completes or <code><seealso cref="#stop"/></code> is called. </summary>
		''' <returns> <code>true</code> if the Sequencer is running, otherwise <code>false</code> </returns>
		ReadOnly Property running As Boolean


		''' <summary>
		''' Starts recording and playback of MIDI data.  Data is recorded to all enabled tracks,
		''' on the channel(s) for which they were enabled.  Recording begins at the current position
		''' of the sequencer.   Any events already in the track are overwritten for the duration
		''' of the recording session.  Events from the currently loaded sequence,
		''' if any, are delivered to the sequencer's transmitter(s) along with messages
		''' received during recording.
		''' <p>
		''' Note that tracks are not by default enabled for recording.  In order to record MIDI data,
		''' at least one track must be specifically enabled for recording.
		''' </summary>
		''' <exception cref="IllegalStateException"> if the <code>Sequencer</code> is
		''' closed.
		''' </exception>
		''' <seealso cref= #startRecording </seealso>
		''' <seealso cref= #recordEnable </seealso>
		''' <seealso cref= #recordDisable </seealso>
		Sub startRecording()


		''' <summary>
		''' Stops recording, if active.  Playback of the current sequence continues.
		''' </summary>
		''' <exception cref="IllegalStateException"> if the <code>Sequencer</code> is
		''' closed.
		''' </exception>
		''' <seealso cref= #startRecording </seealso>
		''' <seealso cref= #isRecording </seealso>
		Sub stopRecording()


		''' <summary>
		''' Indicates whether the Sequencer is currently recording.  The default is <code>false</code>.
		''' The Sequencer begins recording when <code><seealso cref="#startRecording"/></code> is called,
		''' and then returns <code>true</code> until <code><seealso cref="#stop"/></code> or <code><seealso cref="#stopRecording"/></code>
		''' is called. </summary>
		''' <returns> <code>true</code> if the Sequencer is recording, otherwise <code>false</code> </returns>
		ReadOnly Property recording As Boolean


		''' <summary>
		''' Prepares the specified track for recording events received on a particular channel.
		''' Once enabled, a track will receive events when recording is active. </summary>
		''' <param name="track"> the track to which events will be recorded </param>
		''' <param name="channel"> the channel on which events will be received.  If -1 is specified
		''' for the channel value, the track will receive data from all channels. </param>
		''' <exception cref="IllegalArgumentException"> thrown if the track is not part of the current
		''' sequence. </exception>
		Sub recordEnable(ByVal track As Track, ByVal channel As Integer)


		''' <summary>
		''' Disables recording to the specified track.  Events will no longer be recorded
		''' into this track. </summary>
		''' <param name="track"> the track to disable for recording, or <code>null</code> to disable
		''' recording for all tracks. </param>
		Sub recordDisable(ByVal track As Track)


		''' <summary>
		''' Obtains the current tempo, expressed in beats per minute.  The
		''' actual tempo of playback is the product of the returned value
		''' and the tempo factor.
		''' </summary>
		''' <returns> the current tempo in beats per minute
		''' </returns>
		''' <seealso cref= #getTempoFactor </seealso>
		''' <seealso cref= #setTempoInBPM(float) </seealso>
		''' <seealso cref= #getTempoInMPQ </seealso>
		Property tempoInBPM As Single




		''' <summary>
		''' Obtains the current tempo, expressed in microseconds per quarter
		''' note.  The actual tempo of playback is the product of the returned
		''' value and the tempo factor.
		''' </summary>
		''' <returns> the current tempo in microseconds per quarter note </returns>
		''' <seealso cref= #getTempoFactor </seealso>
		''' <seealso cref= #setTempoInMPQ(float) </seealso>
		''' <seealso cref= #getTempoInBPM </seealso>
		Property tempoInMPQ As Single




		''' <summary>
		''' Scales the sequencer's actual playback tempo by the factor provided.
		''' The default is 1.0.  A value of 1.0 represents the natural rate (the
		''' tempo specified in the sequence), 2.0 means twice as fast, etc.
		''' The tempo factor does not affect the values returned by
		''' <code><seealso cref="#getTempoInMPQ"/></code> and <code><seealso cref="#getTempoInBPM"/></code>.
		''' Those values indicate the tempo prior to scaling.
		''' <p>
		''' Note that the tempo factor cannot be adjusted when external
		''' synchronization is used.  In that situation,
		''' <code>setTempoFactor</code> always sets the tempo factor to 1.0.
		''' </summary>
		''' <param name="factor"> the requested tempo scalar </param>
		''' <seealso cref= #getTempoFactor </seealso>
		Property tempoFactor As Single




		''' <summary>
		''' Obtains the length of the current sequence, expressed in MIDI ticks,
		''' or 0 if no sequence is set. </summary>
		''' <returns> length of the sequence in ticks </returns>
		ReadOnly Property tickLength As Long


		''' <summary>
		''' Obtains the current position in the sequence, expressed in MIDI
		''' ticks.  (The duration of a tick in seconds is determined both by
		''' the tempo and by the timing resolution stored in the
		''' <code><seealso cref="Sequence"/></code>.)
		''' </summary>
		''' <returns> current tick </returns>
		''' <seealso cref= #setTickPosition </seealso>
		Property tickPosition As Long




		''' <summary>
		''' Obtains the length of the current sequence, expressed in microseconds,
		''' or 0 if no sequence is set. </summary>
		''' <returns> length of the sequence in microseconds. </returns>
		ReadOnly Property microsecondLength As Long


		''' <summary>
		''' Obtains the current position in the sequence, expressed in
		''' microseconds. </summary>
		''' <returns> the current position in microseconds </returns>
		''' <seealso cref= #setMicrosecondPosition </seealso>
		Property microsecondPosition As Long




		''' <summary>
		''' Sets the source of timing information used by this sequencer.
		''' The sequencer synchronizes to the master, which is the internal clock,
		''' MIDI clock, or MIDI time code, depending on the value of
		''' <code>sync</code>.  The <code>sync</code> argument must be one
		''' of the supported modes, as returned by
		''' <code><seealso cref="#getMasterSyncModes"/></code>.
		''' </summary>
		''' <param name="sync"> the desired master synchronization mode
		''' </param>
		''' <seealso cref= SyncMode#INTERNAL_CLOCK </seealso>
		''' <seealso cref= SyncMode#MIDI_SYNC </seealso>
		''' <seealso cref= SyncMode#MIDI_TIME_CODE </seealso>
		''' <seealso cref= #getMasterSyncMode </seealso>
		Property masterSyncMode As SyncMode




		''' <summary>
		''' Obtains the set of master synchronization modes supported by this
		''' sequencer.
		''' </summary>
		''' <returns> the available master synchronization modes
		''' </returns>
		''' <seealso cref= SyncMode#INTERNAL_CLOCK </seealso>
		''' <seealso cref= SyncMode#MIDI_SYNC </seealso>
		''' <seealso cref= SyncMode#MIDI_TIME_CODE </seealso>
		''' <seealso cref= #getMasterSyncMode </seealso>
		''' <seealso cref= #setMasterSyncMode(Sequencer.SyncMode) </seealso>
		ReadOnly Property masterSyncModes As SyncMode()


		''' <summary>
		''' Sets the slave synchronization mode for the sequencer.
		''' This indicates the type of timing information sent by the sequencer
		''' to its receiver.  The <code>sync</code> argument must be one
		''' of the supported modes, as returned by
		''' <code><seealso cref="#getSlaveSyncModes"/></code>.
		''' </summary>
		''' <param name="sync"> the desired slave synchronization mode
		''' </param>
		''' <seealso cref= SyncMode#MIDI_SYNC </seealso>
		''' <seealso cref= SyncMode#MIDI_TIME_CODE </seealso>
		''' <seealso cref= SyncMode#NO_SYNC </seealso>
		''' <seealso cref= #getSlaveSyncModes </seealso>
		Property slaveSyncMode As SyncMode




		''' <summary>
		''' Obtains the set of slave synchronization modes supported by the sequencer.
		''' </summary>
		''' <returns> the available slave synchronization modes
		''' </returns>
		''' <seealso cref= SyncMode#MIDI_SYNC </seealso>
		''' <seealso cref= SyncMode#MIDI_TIME_CODE </seealso>
		''' <seealso cref= SyncMode#NO_SYNC </seealso>
		ReadOnly Property slaveSyncModes As SyncMode()


		''' <summary>
		''' Sets the mute state for a track.  This method may fail for a number
		''' of reasons.  For example, the track number specified may not be valid
		''' for the current sequence, or the sequencer may not support this functionality.
		''' An application which needs to verify whether this operation succeeded should
		''' follow this call with a call to <code><seealso cref="#getTrackMute"/></code>.
		''' </summary>
		''' <param name="track"> the track number.  Tracks in the current sequence are numbered
		''' from 0 to the number of tracks in the sequence minus 1. </param>
		''' <param name="mute"> the new mute state for the track.  <code>true</code> implies the
		''' track should be muted, <code>false</code> implies the track should be unmuted. </param>
		''' <seealso cref= #getSequence </seealso>
		Sub setTrackMute(ByVal track As Integer, ByVal mute As Boolean)


		''' <summary>
		''' Obtains the current mute state for a track.  The default mute
		''' state for all tracks which have not been muted is false.  In any
		''' case where the specified track has not been muted, this method should
		''' return false.  This applies if the sequencer does not support muting
		''' of tracks, and if the specified track index is not valid.
		''' </summary>
		''' <param name="track"> the track number.  Tracks in the current sequence are numbered
		''' from 0 to the number of tracks in the sequence minus 1. </param>
		''' <returns> <code>true</code> if muted, <code>false</code> if not. </returns>
		Function getTrackMute(ByVal track As Integer) As Boolean

		''' <summary>
		''' Sets the solo state for a track.  If <code>solo</code> is <code>true</code>
		''' only this track and other solo'd tracks will sound. If <code>solo</code>
		''' is <code>false</code> then only other solo'd tracks will sound, unless no
		''' tracks are solo'd in which case all un-muted tracks will sound.
		''' <p>
		''' This method may fail for a number
		''' of reasons.  For example, the track number specified may not be valid
		''' for the current sequence, or the sequencer may not support this functionality.
		''' An application which needs to verify whether this operation succeeded should
		''' follow this call with a call to <code><seealso cref="#getTrackSolo"/></code>.
		''' </summary>
		''' <param name="track"> the track number.  Tracks in the current sequence are numbered
		''' from 0 to the number of tracks in the sequence minus 1. </param>
		''' <param name="solo"> the new solo state for the track.  <code>true</code> implies the
		''' track should be solo'd, <code>false</code> implies the track should not be solo'd. </param>
		''' <seealso cref= #getSequence </seealso>
		Sub setTrackSolo(ByVal track As Integer, ByVal solo As Boolean)


		''' <summary>
		''' Obtains the current solo state for a track.  The default mute
		''' state for all tracks which have not been solo'd is false.  In any
		''' case where the specified track has not been solo'd, this method should
		''' return false.  This applies if the sequencer does not support soloing
		''' of tracks, and if the specified track index is not valid.
		''' </summary>
		''' <param name="track"> the track number.  Tracks in the current sequence are numbered
		''' from 0 to the number of tracks in the sequence minus 1. </param>
		''' <returns> <code>true</code> if solo'd, <code>false</code> if not. </returns>
		Function getTrackSolo(ByVal track As Integer) As Boolean


		''' <summary>
		''' Registers a meta-event listener to receive
		''' notification whenever a meta-event is encountered in the sequence
		''' and processed by the sequencer. This method can fail if, for
		''' instance,this class of sequencer does not support meta-event
		''' notification.
		''' </summary>
		''' <param name="listener"> listener to add </param>
		''' <returns> <code>true</code> if the listener was successfully added,
		''' otherwise <code>false</code>
		''' </returns>
		''' <seealso cref= #removeMetaEventListener </seealso>
		''' <seealso cref= MetaEventListener </seealso>
		''' <seealso cref= MetaMessage </seealso>
		Function addMetaEventListener(ByVal listener As MetaEventListener) As Boolean


		''' <summary>
		''' Removes the specified meta-event listener from this sequencer's
		''' list of registered listeners, if in fact the listener is registered.
		''' </summary>
		''' <param name="listener"> the meta-event listener to remove </param>
		''' <seealso cref= #addMetaEventListener </seealso>
		Sub removeMetaEventListener(ByVal listener As MetaEventListener)


		''' <summary>
		''' Registers a controller event listener to receive notification
		''' whenever the sequencer processes a control-change event of the
		''' requested type or types.  The types are specified by the
		''' <code>controllers</code> argument, which should contain an array of
		''' MIDI controller numbers.  (Each number should be between 0 and 127,
		''' inclusive.  See the MIDI 1.0 Specification for the numbers that
		''' correspond to various types of controllers.)
		''' <p>
		''' The returned array contains the MIDI controller
		''' numbers for which the listener will now receive events.
		''' Some sequencers might not support controller event notification, in
		''' which case the array has a length of 0.  Other sequencers might
		''' support notification for some controllers but not all.
		''' This method may be invoked repeatedly.
		''' Each time, the returned array indicates all the controllers
		''' that the listener will be notified about, not only the controllers
		''' requested in that particular invocation.
		''' </summary>
		''' <param name="listener"> the controller event listener to add to the list of
		''' registered listeners </param>
		''' <param name="controllers"> the MIDI controller numbers for which change
		''' notification is requested </param>
		''' <returns> the numbers of all the MIDI controllers whose changes will
		''' now be reported to the specified listener
		''' </returns>
		''' <seealso cref= #removeControllerEventListener </seealso>
		''' <seealso cref= ControllerEventListener </seealso>
		Function addControllerEventListener(ByVal listener As ControllerEventListener, ByVal controllers As Integer()) As Integer()


		''' <summary>
		''' Removes a controller event listener's interest in one or more
		''' types of controller event. The <code>controllers</code> argument
		''' is an array of MIDI numbers corresponding to the  controllers for
		''' which the listener should no longer receive change notifications.
		''' To completely remove this listener from the list of registered
		''' listeners, pass in <code>null</code> for <code>controllers</code>.
		''' The returned array contains the MIDI controller
		''' numbers for which the listener will now receive events.  The
		''' array has a length of 0 if the listener will not receive
		''' change notifications for any controllers.
		''' </summary>
		''' <param name="listener"> old listener </param>
		''' <param name="controllers"> the MIDI controller numbers for which change
		''' notification should be cancelled, or <code>null</code> to cancel
		''' for all controllers </param>
		''' <returns> the numbers of all the MIDI controllers whose changes will
		''' now be reported to the specified listener
		''' </returns>
		''' <seealso cref= #addControllerEventListener </seealso>
		Function removeControllerEventListener(ByVal listener As ControllerEventListener, ByVal controllers As Integer()) As Integer()


		''' <summary>
		''' Sets the first MIDI tick that will be
		''' played in the loop. If the loop count is
		''' greater than 0, playback will jump to this
		''' point when reaching the loop end point.
		''' 
		''' <p>A value of 0 for the starting point means the
		''' beginning of the loaded sequence. The starting
		''' point must be lower than or equal to the ending
		''' point, and it must fall within the size of the
		''' loaded sequence.
		''' 
		''' <p>A sequencer's loop start point defaults to
		''' start of the sequence.
		''' </summary>
		''' <param name="tick"> the loop's starting position,
		'''        in MIDI ticks (zero-based) </param>
		''' <exception cref="IllegalArgumentException"> if the requested
		'''         loop start point cannot be set, usually because
		'''         it falls outside the sequence's
		'''         duration or because the start point is
		'''         after the end point
		''' </exception>
		''' <seealso cref= #setLoopEndPoint </seealso>
		''' <seealso cref= #setLoopCount </seealso>
		''' <seealso cref= #getLoopStartPoint </seealso>
		''' <seealso cref= #start
		''' @since 1.5 </seealso>
		Property loopStartPoint As Long




		''' <summary>
		''' Sets the last MIDI tick that will be played in
		''' the loop. If the loop count is 0, the loop end
		''' point has no effect and playback continues to
		''' play when reaching the loop end point.
		''' 
		''' <p>A value of -1 for the ending point
		''' indicates the last tick of the sequence.
		''' Otherwise, the ending point must be greater
		''' than or equal to the starting point, and it must
		''' fall within the size of the loaded sequence.
		''' 
		''' <p>A sequencer's loop end point defaults to -1,
		''' meaning the end of the sequence.
		''' </summary>
		''' <param name="tick"> the loop's ending position,
		'''        in MIDI ticks (zero-based), or
		'''        -1 to indicate the final tick </param>
		''' <exception cref="IllegalArgumentException"> if the requested
		'''         loop point cannot be set, usually because
		'''         it falls outside the sequence's
		'''         duration or because the ending point is
		'''         before the starting point
		''' </exception>
		''' <seealso cref= #setLoopStartPoint </seealso>
		''' <seealso cref= #setLoopCount </seealso>
		''' <seealso cref= #getLoopEndPoint </seealso>
		''' <seealso cref= #start
		''' @since 1.5 </seealso>
		Property loopEndPoint As Long




		''' <summary>
		''' Sets the number of repetitions of the loop for
		''' playback.
		''' When the playback position reaches the loop end point,
		''' it will loop back to the loop start point
		''' <code>count</code> times, after which playback will
		''' continue to play to the end of the sequence.
		''' <p>
		''' If the current position when this method is invoked
		''' is greater than the loop end point, playback
		''' continues to the end of the sequence without looping,
		''' unless the loop end point is changed subsequently.
		''' <p>
		''' A <code>count</code> value of 0 disables looping:
		''' playback will continue at the loop end point, and it
		''' will not loop back to the loop start point.
		''' This is a sequencer's default.
		''' 
		''' <p>If playback is stopped during looping, the
		''' current loop status is cleared; subsequent start
		''' requests are not affected by an interrupted loop
		''' operation.
		''' </summary>
		''' <param name="count"> the number of times playback should
		'''        loop back from the loop's end position
		'''        to the loop's start position, or
		'''        <code><seealso cref="#LOOP_CONTINUOUSLY"/></code>
		'''        to indicate that looping should
		'''        continue until interrupted
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>count</code> is
		''' negative and not equal to <seealso cref="#LOOP_CONTINUOUSLY"/>
		''' </exception>
		''' <seealso cref= #setLoopStartPoint </seealso>
		''' <seealso cref= #setLoopEndPoint </seealso>
		''' <seealso cref= #getLoopCount </seealso>
		''' <seealso cref= #start
		''' @since 1.5 </seealso>
		Property loopCount As Integer



		''' <summary>
		''' A <code>SyncMode</code> object represents one of the ways in which
		''' a MIDI sequencer's notion of time can be synchronized with a master
		''' or slave device.
		''' If the sequencer is being synchronized to a master, the
		''' sequencer revises its current time in response to messages from
		''' the master.  If the sequencer has a slave, the sequencer
		''' similarly sends messages to control the slave's timing.
		''' <p>
		''' There are three predefined modes that specify possible masters
		''' for a sequencer: <code>INTERNAL_CLOCK</code>,
		''' <code>MIDI_SYNC</code>, and <code>MIDI_TIME_CODE</code>.  The
		''' latter two work if the sequencer receives MIDI messages from
		''' another device.  In these two modes, the sequencer's time gets reset
		''' based on system real-time timing clock messages or MIDI time code
		''' (MTC) messages, respectively.  These two modes can also be used
		''' as slave modes, in which case the sequencer sends the corresponding
		''' types of MIDI messages to its receiver (whether or not the sequencer
		''' is also receiving them from a master).  A fourth mode,
		''' <code>NO_SYNC</code>, is used to indicate that the sequencer should
		''' not control its receiver's timing.
		''' </summary>
		''' <seealso cref= Sequencer#setMasterSyncMode(Sequencer.SyncMode) </seealso>
		''' <seealso cref= Sequencer#setSlaveSyncMode(Sequencer.SyncMode) </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interface inner types are not converted:
'		public static class SyncMode
	'	{
	'
	'		''' <summary>
	'		''' Synchronization mode name.
	'		''' </summary>
	'		private String name;
	'
	'		''' <summary>
	'		''' Constructs a synchronization mode. </summary>
	'		''' <param name="name"> name of the synchronization mode </param>
	'		protected SyncMode(String name)
	'		{
	'
	'			Me.name = name;
	'		}
	'
	'
	'		''' <summary>
	'		''' Determines whether two objects are equal.
	'		''' Returns <code>true</code> if the objects are identical </summary>
	'		''' <param name="obj"> the reference object with which to compare </param>
	'		''' <returns> <code>true</code> if this object is the same as the
	'		''' <code>obj</code> argument, <code>false</code> otherwise </returns>
	'		public final boolean equals(Object obj)
	'		{
	'
	'			Return MyBase.equals(obj);
	'		}
	'
	'
	'		''' <summary>
	'		''' Finalizes the hashcode method.
	'		''' </summary>
	'		public final int hashCode()
	'		{
	'
	'			Return MyBase.hashCode();
	'		}
	'
	'
	'		''' <summary>
	'		''' Provides this synchronization mode's name as the string
	'		''' representation of the mode. </summary>
	'		''' <returns> the name of this synchronization mode </returns>
	'		public final String toString()
	'		{
	'
	'			Return name;
	'		}
	'
	'
	'		''' <summary>
	'		''' A master synchronization mode that makes the sequencer get
	'		''' its timing information from its internal clock.  This is not
	'		''' a legal slave sync mode.
	'		''' </summary>
	'		public static final SyncMode INTERNAL_CLOCK = New SyncMode("Internal Clock");
	'
	'
	'		''' <summary>
	'		''' A master or slave synchronization mode that specifies the
	'		''' use of MIDI clock
	'		''' messages.  If this mode is used as the master sync mode,
	'		''' the sequencer gets its timing information from system real-time
	'		''' MIDI clock messages.  This mode only applies as the master sync
	'		''' mode for sequencers that are also MIDI receivers.  If this is the
	'		''' slave sync mode, the sequencer sends system real-time MIDI clock
	'		''' messages to its receiver.  MIDI clock messages are sent at a rate
	'		''' of 24 per quarter note.
	'		''' </summary>
	'		public static final SyncMode MIDI_SYNC = New SyncMode("MIDI Sync");
	'
	'
	'		''' <summary>
	'		''' A master or slave synchronization mode that specifies the
	'		''' use of MIDI Time Code.
	'		''' If this mode is used as the master sync mode,
	'		''' the sequencer gets its timing information from MIDI Time Code
	'		''' messages.  This mode only applies as the master sync
	'		''' mode to sequencers that are also MIDI receivers.  If this
	'		''' mode is used as the
	'		''' slave sync mode, the sequencer sends MIDI Time Code
	'		''' messages to its receiver.  (See the MIDI 1.0 Detailed
	'		''' Specification for a description of MIDI Time Code.)
	'		''' </summary>
	'		public static final SyncMode MIDI_TIME_CODE = New SyncMode("MIDI Time Code");
	'
	'
	'		''' <summary>
	'		''' A slave synchronization mode indicating that no timing information
	'		''' should be sent to the receiver.  This is not a legal master sync
	'		''' mode.
	'		''' </summary>
	'		public static final SyncMode NO_SYNC = New SyncMode("No Timing");
	'
	'	} ' class SyncMode
	End Interface

End Namespace