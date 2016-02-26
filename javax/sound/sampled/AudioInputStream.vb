Imports System

'
' * Copyright (c) 1999, 2005, Oracle and/or its affiliates. All rights reserved.
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
	''' An audio input stream is an input stream with a specified audio format and
	''' length.  The length is expressed in sample frames, not bytes.
	''' Several methods are provided for reading a certain number of bytes from
	''' the stream, or an unspecified number of bytes.
	''' The audio input stream keeps track  of the last byte that was read.
	''' You can skip over an arbitrary number of bytes to get to a later position
	''' for reading. An audio input stream may support marks.  When you set a mark,
	''' the current position is remembered so that you can return to it later.
	''' <p>
	''' The <code>AudioSystem</code> class includes many methods that manipulate
	''' <code>AudioInputStream</code> objects.
	''' For example, the methods let you:
	''' <ul>
	''' <li> obtain an
	''' audio input stream from an external audio file, stream, or URL
	''' <li> write an external file from an audio input stream
	''' <li> convert an audio input stream to a different audio format
	''' </ul>
	''' 
	''' @author David Rivas
	''' @author Kara Kytle
	''' @author Florian Bomers
	''' </summary>
	''' <seealso cref= AudioSystem </seealso>
	''' <seealso cref= Clip#open(AudioInputStream) Clip.open(AudioInputStream)
	''' @since 1.3 </seealso>
	Public Class AudioInputStream
		Inherits java.io.InputStream

		''' <summary>
		''' The <code>InputStream</code> from which this <code>AudioInputStream</code>
		''' object was constructed.
		''' </summary>
		Private stream As java.io.InputStream

		''' <summary>
		''' The format of the audio data contained in the stream.
		''' </summary>
		Protected Friend format As AudioFormat

		''' <summary>
		''' This stream's length, in sample frames.
		''' </summary>
		Protected Friend frameLength As Long

		''' <summary>
		''' The size of each frame, in bytes.
		''' </summary>
		Protected Friend frameSize As Integer

		''' <summary>
		''' The current position in this stream, in sample frames (zero-based).
		''' </summary>
		Protected Friend framePos As Long

		''' <summary>
		''' The position where a mark was set.
		''' </summary>
		Private markpos As Long

		''' <summary>
		''' When the underlying stream could only return
		''' a non-integral number of frames, store
		''' the remainder in a temporary buffer
		''' </summary>
		Private pushBackBuffer As SByte() = Nothing

		''' <summary>
		''' number of valid bytes in the pushBackBuffer
		''' </summary>
		Private pushBackLen As Integer = 0

		''' <summary>
		''' MarkBuffer at mark position
		''' </summary>
		Private markPushBackBuffer As SByte() = Nothing

		''' <summary>
		''' number of valid bytes in the markPushBackBuffer
		''' </summary>
		Private markPushBackLen As Integer = 0


		''' <summary>
		''' Constructs an audio input stream that has the requested format and length in sample frames,
		''' using audio data from the specified input stream. </summary>
		''' <param name="stream"> the stream on which this <code>AudioInputStream</code>
		''' object is based </param>
		''' <param name="format"> the format of this stream's audio data </param>
		''' <param name="length"> the length in sample frames of the data in this stream </param>
		Public Sub New(ByVal stream As java.io.InputStream, ByVal format As AudioFormat, ByVal length As Long)

			MyBase.New()

			Me.format = format
			Me.frameLength = length
			Me.frameSize = format.frameSize

			' any frameSize that is not well-defined will
			' cause that this stream will be read in bytes
			If Me.frameSize = AudioSystem.NOT_SPECIFIED OrElse frameSize <= 0 Then Me.frameSize = 1

			Me.stream = stream
			framePos = 0
			markpos = 0
		End Sub


		''' <summary>
		''' Constructs an audio input stream that reads its data from the target
		''' data line indicated.  The format of the stream is the same as that of
		''' the target data line, and the length is AudioSystem#NOT_SPECIFIED. </summary>
		''' <param name="line"> the target data line from which this stream obtains its data. </param>
		''' <seealso cref= AudioSystem#NOT_SPECIFIED </seealso>
		Public Sub New(ByVal line As TargetDataLine)

			Dim tstream As New TargetDataLineInputStream(Me, line)
			format = line.format
			frameLength = AudioSystem.NOT_SPECIFIED
			frameSize = format.frameSize

			If frameSize = AudioSystem.NOT_SPECIFIED OrElse frameSize <= 0 Then frameSize = 1
			Me.stream = tstream
			framePos = 0
			markpos = 0
		End Sub


		''' <summary>
		''' Obtains the audio format of the sound data in this audio input stream. </summary>
		''' <returns> an audio format object describing this stream's format </returns>
		Public Overridable Property format As AudioFormat
			Get
				Return format
			End Get
		End Property


		''' <summary>
		''' Obtains the length of the stream, expressed in sample frames rather than bytes. </summary>
		''' <returns> the length in sample frames </returns>
		Public Overridable Property frameLength As Long
			Get
				Return frameLength
			End Get
		End Property


		''' <summary>
		''' Reads the next byte of data from the audio input stream.  The audio input
		''' stream's frame size must be one byte, or an <code>IOException</code>
		''' will be thrown.
		''' </summary>
		''' <returns> the next byte of data, or -1 if the end of the stream is reached </returns>
		''' <exception cref="IOException"> if an input or output error occurs </exception>
		''' <seealso cref= #read(byte[], int, int) </seealso>
		''' <seealso cref= #read(byte[]) </seealso>
		''' <seealso cref= #available
		''' <p> </seealso>
		Public Overridable Function read() As Integer
			If frameSize <> 1 Then Throw New java.io.IOException("cannot read a single byte if frame size > 1")

			Dim data As SByte() = New SByte(0){}
			Dim temp As Integer = read(data)
			If temp <= 0 Then Return -1
			Return data(0) And &HFF
		End Function


		''' <summary>
		''' Reads some number of bytes from the audio input stream and stores them into
		''' the buffer array <code>b</code>. The number of bytes actually read is
		''' returned as an integer. This method blocks until input data is
		''' available, the end of the stream is detected, or an exception is thrown.
		''' <p>This method will always read an integral number of frames.
		''' If the length of the array is not an integral number
		''' of frames, a maximum of <code>b.length - (b.length % frameSize)
		''' </code> bytes will be read.
		''' </summary>
		''' <param name="b"> the buffer into which the data is read </param>
		''' <returns> the total number of bytes read into the buffer, or -1 if there
		''' is no more data because the end of the stream has been reached </returns>
		''' <exception cref="IOException"> if an input or output error occurs </exception>
		''' <seealso cref= #read(byte[], int, int) </seealso>
		''' <seealso cref= #read() </seealso>
		''' <seealso cref= #available </seealso>
		Public Overridable Function read(ByVal b As SByte()) As Integer
			Return read(b,0,b.Length)
		End Function


		''' <summary>
		''' Reads up to a specified maximum number of bytes of data from the audio
		''' stream, putting them into the given byte array.
		''' <p>This method will always read an integral number of frames.
		''' If <code>len</code> does not specify an integral number
		''' of frames, a maximum of <code>len - (len % frameSize)
		''' </code> bytes will be read.
		''' </summary>
		''' <param name="b"> the buffer into which the data is read </param>
		''' <param name="off"> the offset, from the beginning of array <code>b</code>, at which
		''' the data will be written </param>
		''' <param name="len"> the maximum number of bytes to read </param>
		''' <returns> the total number of bytes read into the buffer, or -1 if there
		''' is no more data because the end of the stream has been reached </returns>
		''' <exception cref="IOException"> if an input or output error occurs </exception>
		''' <seealso cref= #read(byte[]) </seealso>
		''' <seealso cref= #read() </seealso>
		''' <seealso cref= #skip </seealso>
		''' <seealso cref= #available </seealso>
		Public Overridable Function read(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer) As Integer

			' make sure we don't read fractions of a frame.
			If (len Mod frameSize) <> 0 Then
				len -= (len Mod frameSize)
				If len = 0 Then Return 0
			End If

			If frameLength <> AudioSystem.NOT_SPECIFIED Then
				If framePos >= frameLength Then
					Return -1
				Else

					' don't try to read beyond our own set length in frames
					If (len\frameSize) > (frameLength-framePos) Then len = CInt(frameLength-framePos) * frameSize
				End If
			End If

			Dim bytesRead As Integer = 0
			Dim thisOff As Integer = [off]

			' if we've bytes left from last call to read(),
			' use them first
			If pushBackLen > 0 AndAlso len >= pushBackLen Then
				Array.Copy(pushBackBuffer, 0, b, [off], pushBackLen)
				thisOff += pushBackLen
				len -= pushBackLen
				bytesRead += pushBackLen
				pushBackLen = 0
			End If

			Dim thisBytesRead As Integer = stream.read(b, thisOff, len)
			If thisBytesRead = -1 Then Return -1
			If thisBytesRead > 0 Then bytesRead += thisBytesRead
			If bytesRead > 0 Then
				pushBackLen = bytesRead Mod frameSize
				If pushBackLen > 0 Then
					' copy everything we got from the beginning of the frame
					' to our pushback buffer
					If pushBackBuffer Is Nothing Then pushBackBuffer = New SByte(frameSize - 1){}
					Array.Copy(b, [off] + bytesRead - pushBackLen, pushBackBuffer, 0, pushBackLen)
					bytesRead -= pushBackLen
				End If
				' make sure to update our framePos
				framePos += bytesRead\frameSize
			End If
			Return bytesRead
		End Function


		''' <summary>
		''' Skips over and discards a specified number of bytes from this
		''' audio input stream. </summary>
		''' <param name="n"> the requested number of bytes to be skipped </param>
		''' <returns> the actual number of bytes skipped </returns>
		''' <exception cref="IOException"> if an input or output error occurs </exception>
		''' <seealso cref= #read </seealso>
		''' <seealso cref= #available </seealso>
		Public Overridable Function skip(ByVal n As Long) As Long

			' make sure not to skip fractional frames
			If (n Mod frameSize) <> 0 Then n -= (n Mod frameSize)

			If frameLength <> AudioSystem.NOT_SPECIFIED Then
				' don't skip more than our set length in frames.
				If (n\frameSize) > (frameLength-framePos) Then n = (frameLength-framePos) * frameSize
			End If
			Dim temp As Long = stream.skip(n)

			' if no error, update our position.
			If temp Mod frameSize <> 0 Then Throw New java.io.IOException("Could not skip an integer number of frames.")
			If temp >= 0 Then framePos += temp\frameSize
			Return temp

		End Function


		''' <summary>
		''' Returns the maximum number of bytes that can be read (or skipped over) from this
		''' audio input stream without blocking.  This limit applies only to the next invocation of
		''' a <code>read</code> or <code>skip</code> method for this audio input stream; the limit
		''' can vary each time these methods are invoked.
		''' Depending on the underlying stream,an IOException may be thrown if this
		''' stream is closed. </summary>
		''' <returns> the number of bytes that can be read from this audio input stream without blocking </returns>
		''' <exception cref="IOException"> if an input or output error occurs </exception>
		''' <seealso cref= #read(byte[], int, int) </seealso>
		''' <seealso cref= #read(byte[]) </seealso>
		''' <seealso cref= #read() </seealso>
		''' <seealso cref= #skip </seealso>
		Public Overridable Function available() As Integer

			Dim temp As Integer = stream.available()

			' don't return greater than our set length in frames
			If (frameLength <> AudioSystem.NOT_SPECIFIED) AndAlso ((temp\frameSize) > (frameLength-framePos)) Then
				Return CInt(frameLength-framePos) * frameSize
			Else
				Return temp
			End If
		End Function


		''' <summary>
		''' Closes this audio input stream and releases any system resources associated
		''' with the stream. </summary>
		''' <exception cref="IOException"> if an input or output error occurs </exception>
		Public Overridable Sub close()
			stream.close()
		End Sub


		''' <summary>
		''' Marks the current position in this audio input stream. </summary>
		''' <param name="readlimit"> the maximum number of bytes that can be read before
		''' the mark position becomes invalid. </param>
		''' <seealso cref= #reset </seealso>
		''' <seealso cref= #markSupported </seealso>

		Public Overridable Sub mark(ByVal readlimit As Integer)

			stream.mark(readlimit)
			If markSupported() Then
				markpos = framePos
				' remember the pushback buffer
				markPushBackLen = pushBackLen
				If markPushBackLen > 0 Then
					If markPushBackBuffer Is Nothing Then markPushBackBuffer = New SByte(frameSize - 1){}
					Array.Copy(pushBackBuffer, 0, markPushBackBuffer, 0, markPushBackLen)
				End If
			End If
		End Sub


		''' <summary>
		''' Repositions this audio input stream to the position it had at the time its
		''' <code>mark</code> method was last invoked. </summary>
		''' <exception cref="IOException"> if an input or output error occurs. </exception>
		''' <seealso cref= #mark </seealso>
		''' <seealso cref= #markSupported </seealso>
		Public Overridable Sub reset()

			stream.reset()
			framePos = markpos
			' re-create the pushback buffer
			pushBackLen = markPushBackLen
			If pushBackLen > 0 Then
				If pushBackBuffer Is Nothing Then pushBackBuffer = New SByte(frameSize - 2){}
				Array.Copy(markPushBackBuffer, 0, pushBackBuffer, 0, pushBackLen)
			End If
		End Sub


		''' <summary>
		''' Tests whether this audio input stream supports the <code>mark</code> and
		''' <code>reset</code> methods. </summary>
		''' <returns> <code>true</code> if this stream supports the <code>mark</code>
		''' and <code>reset</code> methods; <code>false</code> otherwise </returns>
		''' <seealso cref= #mark </seealso>
		''' <seealso cref= #reset </seealso>
		Public Overridable Function markSupported() As Boolean

			Return stream.markSupported()
		End Function


		''' <summary>
		''' Private inner class that makes a TargetDataLine look like an InputStream.
		''' </summary>
		Private Class TargetDataLineInputStream
			Inherits java.io.InputStream

			Private ReadOnly outerInstance As AudioInputStream


			''' <summary>
			''' The TargetDataLine on which this TargetDataLineInputStream is based.
			''' </summary>
			Friend line As TargetDataLine


			Friend Sub New(ByVal outerInstance As AudioInputStream, ByVal line As TargetDataLine)
					Me.outerInstance = outerInstance
				MyBase.New()
				Me.line = line
			End Sub


			Public Overridable Function available() As Integer
				Return line.available()
			End Function

			'$$fb 2001-07-16: added this method to correctly close the underlying TargetDataLine.
			' fixes bug 4479984
			Public Overridable Sub close()
				' the line needs to be flushed and stopped to avoid a dead lock...
				' Probably related to bugs 4417527, 4334868, 4383457
				If line.active Then
					line.flush()
					line.stop()
				End If
				line.close()
			End Sub

			Public Overridable Function read() As Integer

				Dim b As SByte() = New SByte(0){}

				Dim value As Integer = read(b, 0, 1)

				If value = -1 Then Return -1

				value = CInt(b(0))

				If line.format.encoding.Equals(AudioFormat.Encoding.PCM_SIGNED) Then value += 128

				Return value
			End Function


			Public Overridable Function read(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer) As Integer
				Try
					Return line.read(b, [off], len)
				Catch e As System.ArgumentException
					Throw New java.io.IOException(e.Message)
				End Try
			End Function
		End Class
	End Class

End Namespace