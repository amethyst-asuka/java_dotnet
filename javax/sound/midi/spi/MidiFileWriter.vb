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
	''' A {@code MidiFileWriter} supplies MIDI file-writing services. Classes that
	''' implement this interface can write one or more types of MIDI file from a
	''' <seealso cref="Sequence"/> object.
	''' 
	''' @author Kara Kytle
	''' @since 1.3
	''' </summary>
	Public MustInherit Class MidiFileWriter

		''' <summary>
		''' Obtains the set of MIDI file types for which file writing support is
		''' provided by this file writer.
		''' </summary>
		''' <returns> array of file types. If no file types are supported, an array of
		'''         length 0 is returned. </returns>
		Public MustOverride ReadOnly Property midiFileTypes As Integer()

		''' <summary>
		''' Obtains the file types that this file writer can write from the sequence
		''' specified.
		''' </summary>
		''' <param name="sequence"> the sequence for which MIDI file type support is
		'''         queried </param>
		''' <returns> array of file types. If no file types are supported, returns an
		'''         array of length 0. </returns>
		Public MustOverride Function getMidiFileTypes(ByVal sequence As javax.sound.midi.Sequence) As Integer()

		''' <summary>
		''' Indicates whether file writing support for the specified MIDI file type
		''' is provided by this file writer.
		''' </summary>
		''' <param name="fileType"> the file type for which write capabilities are queried </param>
		''' <returns> {@code true} if the file type is supported, otherwise
		'''         {@code false} </returns>
		Public Overridable Function isFileTypeSupported(ByVal fileType As Integer) As Boolean

			Dim types As Integer() = midiFileTypes
			For i As Integer = 0 To types.Length - 1
				If fileType = types(i) Then Return True
			Next i
			Return False
		End Function

		''' <summary>
		''' Indicates whether a MIDI file of the file type specified can be written
		''' from the sequence indicated.
		''' </summary>
		''' <param name="fileType"> the file type for which write capabilities are queried </param>
		''' <param name="sequence"> the sequence for which file writing support is queried </param>
		''' <returns> {@code true} if the file type is supported for this sequence,
		'''         otherwise {@code false} </returns>
		Public Overridable Function isFileTypeSupported(ByVal fileType As Integer, ByVal ___sequence As javax.sound.midi.Sequence) As Boolean

			Dim types As Integer() = getMidiFileTypes(___sequence)
			For i As Integer = 0 To types.Length - 1
				If fileType = types(i) Then Return True
			Next i
			Return False
		End Function

		''' <summary>
		''' Writes a stream of bytes representing a MIDI file of the file type
		''' indicated to the output stream provided.
		''' </summary>
		''' <param name="in"> sequence containing MIDI data to be written to the file </param>
		''' <param name="fileType"> type of the file to be written to the output stream </param>
		''' <param name="out"> stream to which the file data should be written </param>
		''' <returns> the number of bytes written to the output stream </returns>
		''' <exception cref="IOException"> if an I/O exception occurs </exception>
		''' <exception cref="IllegalArgumentException"> if the file type is not supported by
		'''         this file writer </exception>
		''' <seealso cref= #isFileTypeSupported(int, Sequence) </seealso>
		''' <seealso cref= #getMidiFileTypes(Sequence) </seealso>
		Public MustOverride Function write(ByVal [in] As javax.sound.midi.Sequence, ByVal fileType As Integer, ByVal out As java.io.OutputStream) As Integer

		''' <summary>
		''' Writes a stream of bytes representing a MIDI file of the file type
		''' indicated to the external file provided.
		''' </summary>
		''' <param name="in"> sequence containing MIDI data to be written to the external
		'''         file </param>
		''' <param name="fileType"> type of the file to be written to the external file </param>
		''' <param name="out"> external file to which the file data should be written </param>
		''' <returns> the number of bytes written to the file </returns>
		''' <exception cref="IOException"> if an I/O exception occurs </exception>
		''' <exception cref="IllegalArgumentException"> if the file type is not supported by
		'''         this file writer </exception>
		''' <seealso cref= #isFileTypeSupported(int, Sequence) </seealso>
		''' <seealso cref= #getMidiFileTypes(Sequence) </seealso>
		Public MustOverride Function write(ByVal [in] As javax.sound.midi.Sequence, ByVal fileType As Integer, ByVal out As java.io.File) As Integer
	End Class

End Namespace