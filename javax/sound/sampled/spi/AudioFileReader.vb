'
' * Copyright (c) 1999, 2002, Oracle and/or its affiliates. All rights reserved.
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
	''' Provider for audio file reading services.  Classes providing concrete
	''' implementations can parse the format information from one or more types of
	''' audio file, and can produce audio input streams from files of these types.
	''' 
	''' @author Kara Kytle
	''' @since 1.3
	''' </summary>
	Public MustInherit Class AudioFileReader

		''' <summary>
		''' Obtains the audio file format of the input stream provided.  The stream must
		''' point to valid audio file data.  In general, audio file readers may
		''' need to read some data from the stream before determining whether they
		''' support it.  These parsers must
		''' be able to mark the stream, read enough data to determine whether they
		''' support the stream, and, if not, reset the stream's read pointer to its original
		''' position.  If the input stream does not support this, this method may fail
		''' with an <code>IOException</code>. </summary>
		''' <param name="stream"> the input stream from which file format information should be
		''' extracted </param>
		''' <returns> an <code>AudioFileFormat</code> object describing the audio file format </returns>
		''' <exception cref="UnsupportedAudioFileException"> if the stream does not point to valid audio
		''' file data recognized by the system </exception>
		''' <exception cref="IOException"> if an I/O exception occurs </exception>
		''' <seealso cref= InputStream#markSupported </seealso>
		''' <seealso cref= InputStream#mark </seealso>
		Public MustOverride Function getAudioFileFormat(ByVal stream As java.io.InputStream) As javax.sound.sampled.AudioFileFormat

		''' <summary>
		''' Obtains the audio file format of the URL provided.  The URL must
		''' point to valid audio file data. </summary>
		''' <param name="url"> the URL from which file format information should be
		''' extracted </param>
		''' <returns> an <code>AudioFileFormat</code> object describing the audio file format </returns>
		''' <exception cref="UnsupportedAudioFileException"> if the URL does not point to valid audio
		''' file data recognized by the system </exception>
		''' <exception cref="IOException"> if an I/O exception occurs </exception>
		Public MustOverride Function getAudioFileFormat(ByVal url As java.net.URL) As javax.sound.sampled.AudioFileFormat

		''' <summary>
		''' Obtains the audio file format of the <code>File</code> provided.  The <code>File</code> must
		''' point to valid audio file data. </summary>
		''' <param name="file"> the <code>File</code> from which file format information should be
		''' extracted </param>
		''' <returns> an <code>AudioFileFormat</code> object describing the audio file format </returns>
		''' <exception cref="UnsupportedAudioFileException"> if the <code>File</code> does not point to valid audio
		''' file data recognized by the system </exception>
		''' <exception cref="IOException"> if an I/O exception occurs </exception>
		Public MustOverride Function getAudioFileFormat(ByVal file As java.io.File) As javax.sound.sampled.AudioFileFormat

		''' <summary>
		''' Obtains an audio input stream from the input stream provided.  The stream must
		''' point to valid audio file data.  In general, audio file readers may
		''' need to read some data from the stream before determining whether they
		''' support it.  These parsers must
		''' be able to mark the stream, read enough data to determine whether they
		''' support the stream, and, if not, reset the stream's read pointer to its original
		''' position.  If the input stream does not support this, this method may fail
		''' with an <code>IOException</code>. </summary>
		''' <param name="stream"> the input stream from which the <code>AudioInputStream</code> should be
		''' constructed </param>
		''' <returns> an <code>AudioInputStream</code> object based on the audio file data contained
		''' in the input stream. </returns>
		''' <exception cref="UnsupportedAudioFileException"> if the stream does not point to valid audio
		''' file data recognized by the system </exception>
		''' <exception cref="IOException"> if an I/O exception occurs </exception>
		''' <seealso cref= InputStream#markSupported </seealso>
		''' <seealso cref= InputStream#mark </seealso>
		Public MustOverride Function getAudioInputStream(ByVal stream As java.io.InputStream) As javax.sound.sampled.AudioInputStream

		''' <summary>
		''' Obtains an audio input stream from the URL provided.  The URL must
		''' point to valid audio file data. </summary>
		''' <param name="url"> the URL for which the <code>AudioInputStream</code> should be
		''' constructed </param>
		''' <returns> an <code>AudioInputStream</code> object based on the audio file data pointed
		''' to by the URL </returns>
		''' <exception cref="UnsupportedAudioFileException"> if the URL does not point to valid audio
		''' file data recognized by the system </exception>
		''' <exception cref="IOException"> if an I/O exception occurs </exception>
		Public MustOverride Function getAudioInputStream(ByVal url As java.net.URL) As javax.sound.sampled.AudioInputStream

		''' <summary>
		''' Obtains an audio input stream from the <code>File</code> provided.  The <code>File</code> must
		''' point to valid audio file data. </summary>
		''' <param name="file"> the <code>File</code> for which the <code>AudioInputStream</code> should be
		''' constructed </param>
		''' <returns> an <code>AudioInputStream</code> object based on the audio file data pointed
		''' to by the File </returns>
		''' <exception cref="UnsupportedAudioFileException"> if the <code>File</code> does not point to valid audio
		''' file data recognized by the system </exception>
		''' <exception cref="IOException"> if an I/O exception occurs </exception>
		Public MustOverride Function getAudioInputStream(ByVal file As java.io.File) As javax.sound.sampled.AudioInputStream
	End Class

End Namespace