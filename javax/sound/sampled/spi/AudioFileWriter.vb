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

Namespace javax.sound.sampled.spi




	''' <summary>
	''' Provider for audio file writing services.  Classes providing concrete
	''' implementations can write one or more types of audio file from an audio
	''' stream.
	''' 
	''' @author Kara Kytle
	''' @since 1.3
	''' </summary>
	Public MustInherit Class AudioFileWriter

		''' <summary>
		''' Obtains the file types for which file writing support is provided by this
		''' audio file writer. </summary>
		''' <returns> array of file types.  If no file types are supported,
		''' an array of length 0 is returned. </returns>
		Public MustOverride ReadOnly Property audioFileTypes As javax.sound.sampled.AudioFileFormat.Type()


		''' <summary>
		''' Indicates whether file writing support for the specified file type is provided
		''' by this audio file writer. </summary>
		''' <param name="fileType"> the file type for which write capabilities are queried </param>
		''' <returns> <code>true</code> if the file type is supported,
		''' otherwise <code>false</code> </returns>
		Public Overridable Function isFileTypeSupported(ByVal fileType As javax.sound.sampled.AudioFileFormat.Type) As Boolean

			Dim types As javax.sound.sampled.AudioFileFormat.Type() = audioFileTypes

			For i As Integer = 0 To types.Length - 1
				If fileType.Equals(types(i)) Then Return True
			Next i
			Return False
		End Function


		''' <summary>
		''' Obtains the file types that this audio file writer can write from the
		''' audio input stream specified. </summary>
		''' <param name="stream"> the audio input stream for which audio file type support
		''' is queried </param>
		''' <returns> array of file types.  If no file types are supported,
		''' an array of length 0 is returned. </returns>
		Public MustOverride Function getAudioFileTypes(ByVal stream As javax.sound.sampled.AudioInputStream) As javax.sound.sampled.AudioFileFormat.Type()


		''' <summary>
		''' Indicates whether an audio file of the type specified can be written
		''' from the audio input stream indicated. </summary>
		''' <param name="fileType"> file type for which write capabilities are queried </param>
		''' <param name="stream"> for which file writing support is queried </param>
		''' <returns> <code>true</code> if the file type is supported for this audio input stream,
		''' otherwise <code>false</code> </returns>
		Public Overridable Function isFileTypeSupported(ByVal fileType As javax.sound.sampled.AudioFileFormat.Type, ByVal stream As javax.sound.sampled.AudioInputStream) As Boolean

			Dim types As javax.sound.sampled.AudioFileFormat.Type() = getAudioFileTypes(stream)

			For i As Integer = 0 To types.Length - 1
				If fileType.Equals(types(i)) Then Return True
			Next i
			Return False
		End Function


		''' <summary>
		''' Writes a stream of bytes representing an audio file of the file type
		''' indicated to the output stream provided.  Some file types require that
		''' the length be written into the file header, and cannot be written from
		''' start to finish unless the length is known in advance.  An attempt
		''' to write such a file type will fail with an IOException if the length in
		''' the audio file format is
		''' <seealso cref="javax.sound.sampled.AudioSystem#NOT_SPECIFIED AudioSystem.NOT_SPECIFIED"/>. </summary>
		''' <param name="stream"> the audio input stream containing audio data to be
		''' written to the output stream </param>
		''' <param name="fileType"> file type to be written to the output stream </param>
		''' <param name="out"> stream to which the file data should be written </param>
		''' <returns> the number of bytes written to the output stream </returns>
		''' <exception cref="IOException"> if an I/O exception occurs </exception>
		''' <exception cref="IllegalArgumentException"> if the file type is not supported by
		''' the system </exception>
		''' <seealso cref= #isFileTypeSupported(AudioFileFormat.Type, AudioInputStream) </seealso>
		''' <seealso cref= #getAudioFileTypes </seealso>
		Public MustOverride Function write(ByVal stream As javax.sound.sampled.AudioInputStream, ByVal fileType As javax.sound.sampled.AudioFileFormat.Type, ByVal out As java.io.OutputStream) As Integer


		''' <summary>
		''' Writes a stream of bytes representing an audio file of the file format
		''' indicated to the external file provided. </summary>
		''' <param name="stream"> the audio input stream containing audio data to be
		''' written to the file </param>
		''' <param name="fileType"> file type to be written to the file </param>
		''' <param name="out"> external file to which the file data should be written </param>
		''' <returns> the number of bytes written to the file </returns>
		''' <exception cref="IOException"> if an I/O exception occurs </exception>
		''' <exception cref="IllegalArgumentException"> if the file format is not supported by
		''' the system </exception>
		''' <seealso cref= #isFileTypeSupported </seealso>
		''' <seealso cref= #getAudioFileTypes </seealso>
		Public MustOverride Function write(ByVal stream As javax.sound.sampled.AudioInputStream, ByVal fileType As javax.sound.sampled.AudioFileFormat.Type, ByVal out As java.io.File) As Integer


	End Class

End Namespace