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
	''' A {@code SoundbankReader} supplies soundbank file-reading services. Concrete
	''' subclasses of {@code SoundbankReader} parse a given soundbank file, producing
	''' a <seealso cref="javax.sound.midi.Soundbank"/> object that can be loaded into a
	''' <seealso cref="javax.sound.midi.Synthesizer"/>.
	''' 
	''' @since 1.3
	''' @author Kara Kytle
	''' </summary>
	Public MustInherit Class SoundbankReader

		''' <summary>
		''' Obtains a soundbank object from the URL provided.
		''' </summary>
		''' <param name="url"> URL representing the soundbank. </param>
		''' <returns> soundbank object </returns>
		''' <exception cref="InvalidMidiDataException"> if the URL does not point to valid MIDI
		'''         soundbank data recognized by this soundbank reader </exception>
		''' <exception cref="IOException"> if an I/O error occurs </exception>
		Public MustOverride Function getSoundbank(ByVal url As java.net.URL) As javax.sound.midi.Soundbank

		''' <summary>
		''' Obtains a soundbank object from the {@code InputStream} provided.
		''' </summary>
		''' <param name="stream"> {@code InputStream} representing the soundbank </param>
		''' <returns> soundbank object </returns>
		''' <exception cref="InvalidMidiDataException"> if the stream does not point to valid
		'''         MIDI soundbank data recognized by this soundbank reader </exception>
		''' <exception cref="IOException"> if an I/O error occurs </exception>
		Public MustOverride Function getSoundbank(ByVal stream As java.io.InputStream) As javax.sound.midi.Soundbank

		''' <summary>
		''' Obtains a soundbank object from the {@code File} provided.
		''' </summary>
		''' <param name="file"> the {@code File} representing the soundbank </param>
		''' <returns> soundbank object </returns>
		''' <exception cref="InvalidMidiDataException"> if the file does not point to valid MIDI
		'''         soundbank data recognized by this soundbank reader </exception>
		''' <exception cref="IOException"> if an I/O error occurs </exception>
		Public MustOverride Function getSoundbank(ByVal file As java.io.File) As javax.sound.midi.Soundbank
	End Class

End Namespace