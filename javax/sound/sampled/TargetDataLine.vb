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
	''' A target data line is a type of <code><seealso cref="DataLine"/></code> from which
	''' audio data can be read.  The most common example is a data line that gets
	''' its data from an audio capture device.  (The device is implemented as a
	''' mixer that writes to the target data line.)
	''' <p>
	''' Note that the naming convention for this interface reflects the relationship
	''' between the line and its mixer.  From the perspective of an application,
	''' a target data line may act as a source for audio data.
	''' <p>
	''' The target data line can be obtained from a mixer by invoking the
	''' <code><seealso cref="Mixer#getLine getLine"/></code>
	''' method of <code>Mixer</code> with an appropriate
	''' <code><seealso cref="DataLine.Info"/></code> object.
	''' <p>
	''' The <code>TargetDataLine</code> interface provides a method for reading the
	''' captured data from the target data line's buffer.Applications
	''' that record audio should read data from the target data line quickly enough
	''' to keep the buffer from overflowing, which could cause discontinuities in
	''' the captured data that are perceived as clicks.  Applications can use the
	''' <code><seealso cref="DataLine#available available"/></code> method defined in the
	''' <code>DataLine</code> interface to determine the amount of data currently
	''' queued in the data line's buffer.  If the buffer does overflow,
	''' the oldest queued data is discarded and replaced by new data.
	''' 
	''' @author Kara Kytle </summary>
	''' <seealso cref= Mixer </seealso>
	''' <seealso cref= DataLine </seealso>
	''' <seealso cref= SourceDataLine
	''' @since 1.3 </seealso>
	Public Interface TargetDataLine
		Inherits DataLine


		''' <summary>
		''' Opens the line with the specified format and requested buffer size,
		''' causing the line to acquire any required system resources and become
		''' operational.
		''' <p>
		''' The buffer size is specified in bytes, but must represent an integral
		''' number of sample frames.  Invoking this method with a requested buffer
		''' size that does not meet this requirement may result in an
		''' IllegalArgumentException.  The actual buffer size for the open line may
		''' differ from the requested buffer size.  The value actually set may be
		''' queried by subsequently calling <code><seealso cref="DataLine#getBufferSize"/></code>
		''' <p>
		''' If this operation succeeds, the line is marked as open, and an
		''' <code><seealso cref="LineEvent.Type#OPEN OPEN"/></code> event is dispatched to the
		''' line's listeners.
		''' <p>
		''' Invoking this method on a line that is already open is illegal
		''' and may result in an <code>IllegalStateException</code>.
		''' <p>
		''' Some lines, once closed, cannot be reopened.  Attempts
		''' to reopen such a line will always result in a
		''' <code>LineUnavailableException</code>.
		''' </summary>
		''' <param name="format"> the desired audio format </param>
		''' <param name="bufferSize"> the desired buffer size, in bytes. </param>
		''' <exception cref="LineUnavailableException"> if the line cannot be
		''' opened due to resource restrictions </exception>
		''' <exception cref="IllegalArgumentException"> if the buffer size does not represent
		''' an integral number of sample frames,
		''' or if <code>format</code> is not fully specified or invalid </exception>
		''' <exception cref="IllegalStateException"> if the line is already open </exception>
		''' <exception cref="SecurityException"> if the line cannot be
		''' opened due to security restrictions
		''' </exception>
		''' <seealso cref= #open(AudioFormat) </seealso>
		''' <seealso cref= Line#open </seealso>
		''' <seealso cref= Line#close </seealso>
		''' <seealso cref= Line#isOpen </seealso>
		''' <seealso cref= LineEvent </seealso>
		Sub open(ByVal format As AudioFormat, ByVal bufferSize As Integer)


		''' <summary>
		''' Opens the line with the specified format, causing the line to acquire any
		''' required system resources and become operational.
		''' 
		''' <p>
		''' The implementation chooses a buffer size, which is measured in bytes but
		''' which encompasses an integral number of sample frames.  The buffer size
		''' that the system has chosen may be queried by subsequently calling <code><seealso cref="DataLine#getBufferSize"/></code>
		''' <p>
		''' If this operation succeeds, the line is marked as open, and an
		''' <code><seealso cref="LineEvent.Type#OPEN OPEN"/></code> event is dispatched to the
		''' line's listeners.
		''' <p>
		''' Invoking this method on a line that is already open is illegal
		''' and may result in an <code>IllegalStateException</code>.
		''' <p>
		''' Some lines, once closed, cannot be reopened.  Attempts
		''' to reopen such a line will always result in a
		''' <code>LineUnavailableException</code>.
		''' </summary>
		''' <param name="format"> the desired audio format </param>
		''' <exception cref="LineUnavailableException"> if the line cannot be
		''' opened due to resource restrictions </exception>
		''' <exception cref="IllegalArgumentException"> if <code>format</code>
		''' is not fully specified or invalid </exception>
		''' <exception cref="IllegalStateException"> if the line is already open </exception>
		''' <exception cref="SecurityException"> if the line cannot be
		''' opened due to security restrictions
		''' </exception>
		''' <seealso cref= #open(AudioFormat, int) </seealso>
		''' <seealso cref= Line#open </seealso>
		''' <seealso cref= Line#close </seealso>
		''' <seealso cref= Line#isOpen </seealso>
		''' <seealso cref= LineEvent </seealso>
		Sub open(ByVal format As AudioFormat)


		''' <summary>
		''' Reads audio data from the data line's input buffer.   The requested
		''' number of bytes is read into the specified array, starting at
		''' the specified offset into the array in bytes.  This method blocks until
		''' the requested amount of data has been read.  However, if the data line
		''' is closed, stopped, drained, or flushed before the requested amount has
		''' been read, the method no longer blocks, but returns the number of bytes
		''' read thus far.
		''' <p>
		''' The number of bytes that can be read without blocking can be ascertained
		''' using the <code><seealso cref="DataLine#available available"/></code> method of the
		''' <code>DataLine</code> interface.  (While it is guaranteed that
		''' this number of bytes can be read without blocking, there is no guarantee
		''' that attempts to read additional data will block.)
		''' <p>
		''' The number of bytes to be read must represent an integral number of
		''' sample frames, such that:
		''' <br>
		''' <center><code>[ bytes read ] % [frame size in bytes ] == 0</code></center>
		''' <br>
		''' The return value will always meet this requirement.  A request to read a
		''' number of bytes representing a non-integral number of sample frames cannot
		''' be fulfilled and may result in an IllegalArgumentException.
		''' </summary>
		''' <param name="b"> a byte array that will contain the requested input data when
		''' this method returns </param>
		''' <param name="off"> the offset from the beginning of the array, in bytes </param>
		''' <param name="len"> the requested number of bytes to read </param>
		''' <returns> the number of bytes actually read </returns>
		''' <exception cref="IllegalArgumentException"> if the requested number of bytes does
		''' not represent an integral number of sample frames.
		''' or if <code>len</code> is negative. </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if <code>off</code> is negative,
		''' or <code>off+len</code> is greater than the length of the array
		''' <code>b</code>.
		''' </exception>
		''' <seealso cref= SourceDataLine#write </seealso>
		''' <seealso cref= DataLine#available </seealso>
		Function read(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer) As Integer

		''' <summary>
		''' Obtains the number of sample frames of audio data that can be read from
		''' the target data line without blocking.  Note that the return value
		''' measures sample frames, not bytes. </summary>
		''' <returns> the number of sample frames currently available for reading </returns>
		''' <seealso cref= SourceDataLine#availableWrite </seealso>
		'public int availableRead();
	End Interface

End Namespace