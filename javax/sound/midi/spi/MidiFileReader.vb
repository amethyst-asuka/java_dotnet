'
' * Copyright (c) 1999, 2014, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.sound.midi.spi



	''' <summary>
	''' A {@code MidiFileReader} supplies MIDI file-reading services. Classes
	''' implementing this interface can parse the format information from one or more
	''' types of MIDI file, and can produce a <seealso cref="Sequence"/> object from files of
	''' these types.
	''' 
	''' @author Kara Kytle
	''' @since 1.3
	''' </summary>
	Public MustInherit Class MidiFileReader

		''' <summary>
		''' Obtains the MIDI file format of the input stream provided. The stream
		''' must point to valid MIDI file data. In general, MIDI file readers may
		''' need to read some data from the stream before determining whether they
		''' support it. These parsers must be able to mark the stream, read enough
		''' data to determine whether they support the stream, and, if not, reset the
		''' stream's read pointer to its original position. If the input stream does
		''' not support this, this method may fail with an {@code IOException}.
		''' </summary>
		''' <param name="stream"> the input stream from which file format information
		'''         should be extracted </param>
		''' <returns> a {@code MidiFileFormat} object describing the MIDI file format </returns>
		''' <exception cref="InvalidMidiDataException"> if the stream does not point to valid
		'''         MIDI file data recognized by the system </exception>
		''' <exception cref="IOException"> if an I/O exception occurs </exception>
		''' <seealso cref= InputStream#markSupported </seealso>
		''' <seealso cref= InputStream#mark </seealso>
		Public MustOverride Function getMidiFileFormat(ByVal stream As java.io.InputStream) As javax.sound.midi.MidiFileFormat

		''' <summary>
		''' Obtains the MIDI file format of the URL provided. The URL must point to
		''' valid MIDI file data.
		''' </summary>
		''' <param name="url"> the URL from which file format information should be
		'''         extracted </param>
		''' <returns> a {@code MidiFileFormat} object describing the MIDI file format </returns>
		''' <exception cref="InvalidMidiDataException"> if the URL does not point to valid MIDI
		'''         file data recognized by the system </exception>
		''' <exception cref="IOException"> if an I/O exception occurs </exception>
		Public MustOverride Function getMidiFileFormat(ByVal url As java.net.URL) As javax.sound.midi.MidiFileFormat

		''' <summary>
		''' Obtains the MIDI file format of the {@code File} provided. The
		''' {@code File} must point to valid MIDI file data.
		''' </summary>
		''' <param name="file"> the {@code File} from which file format information should
		'''         be extracted </param>
		''' <returns> a {@code MidiFileFormat} object describing the MIDI file format </returns>
		''' <exception cref="InvalidMidiDataException"> if the {@code File} does not point to
		'''         valid MIDI file data recognized by the system </exception>
		''' <exception cref="IOException"> if an I/O exception occurs </exception>
		Public MustOverride Function getMidiFileFormat(ByVal file As java.io.File) As javax.sound.midi.MidiFileFormat

		''' <summary>
		''' Obtains a MIDI sequence from the input stream provided. The stream must
		''' point to valid MIDI file data. In general, MIDI file readers may need to
		''' read some data from the stream before determining whether they support
		''' it. These parsers must be able to mark the stream, read enough data to
		''' determine whether they support the stream, and, if not, reset the
		''' stream's read pointer to its original position. If the input stream does
		''' not support this, this method may fail with an IOException.
		''' </summary>
		''' <param name="stream"> the input stream from which the {@code Sequence} should
		'''         be constructed </param>
		''' <returns> a {@code Sequence} object based on the MIDI file data contained
		'''         in the input stream. </returns>
		''' <exception cref="InvalidMidiDataException"> if the stream does not point to valid
		'''         MIDI file data recognized by the system </exception>
		''' <exception cref="IOException"> if an I/O exception occurs </exception>
		''' <seealso cref= InputStream#markSupported </seealso>
		''' <seealso cref= InputStream#mark </seealso>
		Public MustOverride Function getSequence(ByVal stream As java.io.InputStream) As javax.sound.midi.Sequence

		''' <summary>
		''' Obtains a MIDI sequence from the URL provided. The URL must point to
		''' valid MIDI file data.
		''' </summary>
		''' <param name="url"> the URL for which the {@code Sequence} should be constructed </param>
		''' <returns> a {@code Sequence} object based on the MIDI file data pointed to
		'''         by the URL </returns>
		''' <exception cref="InvalidMidiDataException"> if the URL does not point to valid MIDI
		'''         file data recognized by the system </exception>
		''' <exception cref="IOException"> if an I/O exception occurs </exception>
		Public MustOverride Function getSequence(ByVal url As java.net.URL) As javax.sound.midi.Sequence

		''' <summary>
		''' Obtains a MIDI sequence from the {@code File} provided. The {@code File}
		''' must point to valid MIDI file data.
		''' </summary>
		''' <param name="file"> the {@code File} from which the {@code Sequence} should be
		'''         constructed </param>
		''' <returns> a {@code Sequence} object based on the MIDI file data pointed to
		'''         by the {@code File} </returns>
		''' <exception cref="InvalidMidiDataException"> if the {@code File} does not point to
		'''         valid MIDI file data recognized by the system </exception>
		''' <exception cref="IOException"> if an I/O exception occurs </exception>
		Public MustOverride Function getSequence(ByVal file As java.io.File) As javax.sound.midi.Sequence
	End Class

End Namespace