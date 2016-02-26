'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' A <code>Soundbank</code> contains a set of <code>Instruments</code>
	''' that can be loaded into a <code>Synthesizer</code>.
	''' Note that a Java Sound <code>Soundbank</code> is different from a MIDI bank.
	''' MIDI permits up to 16383 banks, each containing up to 128 instruments
	''' (also sometimes called programs, patches, or timbres).
	''' However, a <code>Soundbank</code> can contain 16383 times 128 instruments,
	''' because the instruments within a <code>Soundbank</code> are indexed by both
	''' a MIDI program number and a MIDI bank number (via a <code>Patch</code>
	''' object). Thus, a <code>Soundbank</code> can be thought of as a collection
	''' of MIDI banks.
	''' <p>
	''' <code>Soundbank</code> includes methods that return <code>String</code>
	''' objects containing the sound bank's name, manufacturer, version number, and
	''' description.  The precise content and format of these strings is left
	''' to the implementor.
	''' <p>
	''' Different synthesizers use a variety of synthesis techniques.  A common
	''' one is wavetable synthesis, in which a segment of recorded sound is
	''' played back, often with looping and pitch change.  The Downloadable Sound
	''' (DLS) format uses segments of recorded sound, as does the Headspace Engine.
	''' <code>Soundbanks</code> and <code>Instruments</code> that are based on
	''' wavetable synthesis (or other uses of stored sound recordings) should
	''' typically implement the <code>getResources()</code>
	''' method to provide access to these recorded segments.  This is optional,
	''' however; the method can return an zero-length array if the synthesis technique
	''' doesn't use sampled sound (FM synthesis and physical modeling are examples
	''' of such techniques), or if it does but the implementor chooses not to make the
	''' samples accessible.
	''' </summary>
	''' <seealso cref= Synthesizer#getDefaultSoundbank </seealso>
	''' <seealso cref= Synthesizer#isSoundbankSupported </seealso>
	''' <seealso cref= Synthesizer#loadInstruments(Soundbank, Patch[]) </seealso>
	''' <seealso cref= Patch </seealso>
	''' <seealso cref= Instrument </seealso>
	''' <seealso cref= SoundbankResource
	''' 
	''' @author David Rivas
	''' @author Kara Kytle </seealso>

	Public Interface Soundbank


		''' <summary>
		''' Obtains the name of the sound bank. </summary>
		''' <returns> a <code>String</code> naming the sound bank </returns>
		ReadOnly Property name As String

		''' <summary>
		''' Obtains the version string for the sound bank. </summary>
		''' <returns> a <code>String</code> that indicates the sound bank's version </returns>
		ReadOnly Property version As String

		''' <summary>
		''' Obtains a <code>string</code> naming the company that provides the
		''' sound bank </summary>
		''' <returns> the vendor string </returns>
		ReadOnly Property vendor As String

		''' <summary>
		''' Obtains a textual description of the sound bank, suitable for display. </summary>
		''' <returns> a <code>String</code> that describes the sound bank </returns>
		ReadOnly Property description As String


		''' <summary>
		''' Extracts a list of non-Instrument resources contained in the sound bank. </summary>
		''' <returns> an array of resources, excluding instruments.  If the sound bank contains
		''' no resources (other than instruments), returns an array of length 0. </returns>
		ReadOnly Property resources As SoundbankResource()


		''' <summary>
		''' Obtains a list of instruments contained in this sound bank. </summary>
		''' <returns> an array of the <code>Instruments</code> in this
		''' <code>SoundBank</code>
		''' If the sound bank contains no instruments, returns an array of length 0.
		''' </returns>
		''' <seealso cref= Synthesizer#getLoadedInstruments </seealso>
		''' <seealso cref= #getInstrument(Patch) </seealso>
		ReadOnly Property instruments As Instrument()

		''' <summary>
		''' Obtains an <code>Instrument</code> from the given <code>Patch</code>. </summary>
		''' <param name="patch"> a <code>Patch</code> object specifying the bank index
		''' and program change number </param>
		''' <returns> the requested instrument, or <code>null</code> if the
		''' sound bank doesn't contain that instrument
		''' </returns>
		''' <seealso cref= #getInstruments </seealso>
		''' <seealso cref= Synthesizer#loadInstruments(Soundbank, Patch[]) </seealso>
		Function getInstrument(ByVal patch As Patch) As Instrument


	End Interface

End Namespace