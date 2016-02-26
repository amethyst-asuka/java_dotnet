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

Namespace javax.sound.sampled


	''' <summary>
	''' The <code>Clip</code> interface represents a special kind of data line whose
	''' audio data can be loaded prior to playback, instead of being streamed in
	''' real time.
	''' <p>
	''' Because the data is pre-loaded and has a known length, you can set a clip
	''' to start playing at any position in its audio data.  You can also create a
	''' loop, so that when the clip is played it will cycle repeatedly.  Loops are
	''' specified with a starting and ending sample frame, along with the number of
	''' times that the loop should be played.
	''' <p>
	''' Clips may be obtained from a <code><seealso cref="Mixer"/></code> that supports lines
	''' of this type.  Data is loaded into a clip when it is opened.
	''' <p>
	''' Playback of an audio clip may be started and stopped using the <code>start</code>
	''' and <code>stop</code> methods.  These methods do not reset the media position;
	''' <code>start</code> causes playback to continue from the position where playback
	''' was last stopped.  To restart playback from the beginning of the clip's audio
	''' data, simply follow the invocation of <code><seealso cref="DataLine#stop stop"/></code>
	''' with setFramePosition(0), which rewinds the media to the beginning
	''' of the clip.
	''' 
	''' @author Kara Kytle
	''' @since 1.3
	''' </summary>
	Public Interface Clip
		Inherits DataLine


		''' <summary>
		''' A value indicating that looping should continue indefinitely rather than
		''' complete after a specific number of loops. </summary>
		''' <seealso cref= #loop </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int LOOP_CONTINUOUSLY = -1;

		''' <summary>
		''' Opens the clip, meaning that it should acquire any required
		''' system resources and become operational.  The clip is opened
		''' with the format and audio data indicated.
		''' If this operation succeeds, the line is marked as open and an
		''' <code><seealso cref="LineEvent.Type#OPEN OPEN"/></code> event is dispatched
		''' to the line's listeners.
		''' <p>
		''' Invoking this method on a line which is already open is illegal
		''' and may result in an IllegalStateException.
		''' <p>
		''' Note that some lines, once closed, cannot be reopened.  Attempts
		''' to reopen such a line will always result in a
		''' <code><seealso cref="LineUnavailableException"/></code>.
		''' </summary>
		''' <param name="format"> the format of the supplied audio data </param>
		''' <param name="data"> a byte array containing audio data to load into the clip </param>
		''' <param name="offset"> the point at which to start copying, expressed in
		''' <em>bytes</em> from the beginning of the array </param>
		''' <param name="bufferSize"> the number of <em>bytes</em>
		''' of data to load into the clip from the array. </param>
		''' <exception cref="LineUnavailableException"> if the line cannot be
		''' opened due to resource restrictions </exception>
		''' <exception cref="IllegalArgumentException"> if the buffer size does not represent
		''' an integral number of sample frames,
		''' or if <code>format</code> is not fully specified or invalid </exception>
		''' <exception cref="IllegalStateException"> if the line is already open </exception>
		''' <exception cref="SecurityException"> if the line cannot be
		''' opened due to security restrictions
		''' </exception>
		''' <seealso cref= #close </seealso>
		''' <seealso cref= #isOpen </seealso>
		''' <seealso cref= LineListener </seealso>
		Sub open(ByVal format As AudioFormat, ByVal data As SByte(), ByVal offset As Integer, ByVal bufferSize As Integer)

		''' <summary>
		''' Opens the clip with the format and audio data present in the provided audio
		''' input stream.  Opening a clip means that it should acquire any required
		''' system resources and become operational.  If this operation
		''' input stream.  If this operation
		''' succeeds, the line is marked open and an
		''' <code><seealso cref="LineEvent.Type#OPEN OPEN"/></code> event is dispatched
		''' to the line's listeners.
		''' <p>
		''' Invoking this method on a line which is already open is illegal
		''' and may result in an IllegalStateException.
		''' <p>
		''' Note that some lines, once closed, cannot be reopened.  Attempts
		''' to reopen such a line will always result in a
		''' <code><seealso cref="LineUnavailableException"/></code>.
		''' </summary>
		''' <param name="stream"> an audio input stream from which audio data will be read into
		''' the clip </param>
		''' <exception cref="LineUnavailableException"> if the line cannot be
		''' opened due to resource restrictions </exception>
		''' <exception cref="IOException"> if an I/O exception occurs during reading of
		''' the stream </exception>
		''' <exception cref="IllegalArgumentException"> if the stream's audio format
		''' is not fully specified or invalid </exception>
		''' <exception cref="IllegalStateException"> if the line is already open </exception>
		''' <exception cref="SecurityException"> if the line cannot be
		''' opened due to security restrictions
		''' </exception>
		''' <seealso cref= #close </seealso>
		''' <seealso cref= #isOpen </seealso>
		''' <seealso cref= LineListener </seealso>
		Sub open(ByVal stream As AudioInputStream)

		''' <summary>
		''' Obtains the media length in sample frames. </summary>
		''' <returns> the media length, expressed in sample frames,
		''' or <code>AudioSystem.NOT_SPECIFIED</code> if the line is not open. </returns>
		''' <seealso cref= AudioSystem#NOT_SPECIFIED </seealso>
		ReadOnly Property frameLength As Integer

		''' <summary>
		''' Obtains the media duration in microseconds </summary>
		''' <returns> the media duration, expressed in microseconds,
		''' or <code>AudioSystem.NOT_SPECIFIED</code> if the line is not open. </returns>
		''' <seealso cref= AudioSystem#NOT_SPECIFIED </seealso>
		ReadOnly Property microsecondLength As Long

		''' <summary>
		''' Sets the media position in sample frames.  The position is zero-based;
		''' the first frame is frame number zero.  When the clip begins playing the
		''' next time, it will start by playing the frame at this position.
		''' <p>
		''' To obtain the current position in sample frames, use the
		''' <code><seealso cref="DataLine#getFramePosition getFramePosition"/></code>
		''' method of <code>DataLine</code>.
		''' </summary>
		''' <param name="frames"> the desired new media position, expressed in sample frames </param>
		WriteOnly Property framePosition As Integer

		''' <summary>
		''' Sets the media position in microseconds.  When the clip begins playing the
		''' next time, it will start at this position.
		''' The level of precision is not guaranteed.  For example, an implementation
		''' might calculate the microsecond position from the current frame position
		''' and the audio sample frame rate.  The precision in microseconds would
		''' then be limited to the number of microseconds per sample frame.
		''' <p>
		''' To obtain the current position in microseconds, use the
		''' <code><seealso cref="DataLine#getMicrosecondPosition getMicrosecondPosition"/></code>
		''' method of <code>DataLine</code>.
		''' </summary>
		''' <param name="microseconds"> the desired new media position, expressed in microseconds </param>
		WriteOnly Property microsecondPosition As Long

		''' <summary>
		''' Sets the first and last sample frames that will be played in
		''' the loop.  The ending point must be greater than
		''' or equal to the starting point, and both must fall within the
		''' the size of the loaded media.  A value of 0 for the starting
		''' point means the beginning of the loaded media.  Similarly, a value of -1
		''' for the ending point indicates the last frame of the media. </summary>
		''' <param name="start"> the loop's starting position, in sample frames (zero-based) </param>
		''' <param name="end"> the loop's ending position, in sample frames (zero-based), or
		''' -1 to indicate the final frame </param>
		''' <exception cref="IllegalArgumentException"> if the requested
		''' loop points cannot be set, usually because one or both falls outside
		''' the media's duration or because the ending point is
		''' before the starting point </exception>
		Sub setLoopPoints(ByVal start As Integer, ByVal [end] As Integer)

		''' <summary>
		''' Starts looping playback from the current position.   Playback will
		''' continue to the loop's end point, then loop back to the loop start point
		''' <code>count</code> times, and finally continue playback to the end of
		''' the clip.
		''' <p>
		''' If the current position when this method is invoked is greater than the
		''' loop end point, playback simply continues to the
		''' end of the clip without looping.
		''' <p>
		''' A <code>count</code> value of 0 indicates that any current looping should
		''' cease and playback should continue to the end of the clip.  The behavior
		''' is undefined when this method is invoked with any other value during a
		''' loop operation.
		''' <p>
		''' If playback is stopped during looping, the current loop status is
		''' cleared; the behavior of subsequent loop and start requests is not
		''' affected by an interrupted loop operation.
		''' </summary>
		''' <param name="count"> the number of times playback should loop back from the
		''' loop's end position to the loop's  start position, or
		''' <code><seealso cref="#LOOP_CONTINUOUSLY"/></code> to indicate that looping should
		''' continue until interrupted </param>
		Sub [loop](ByVal count As Integer)
	End Interface

End Namespace