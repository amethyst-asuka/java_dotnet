'
' * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' A <code>Synthesizer</code> generates sound.  This usually happens when one of
	''' the <code>Synthesizer</code>'s <seealso cref="MidiChannel"/> objects receives a
	''' <seealso cref="MidiChannel#noteOn(int, int) noteOn"/> message, either
	''' directly or via the <code>Synthesizer</code> object.
	''' Many <code>Synthesizer</code>s support <code>Receivers</code>, through which
	''' MIDI events can be delivered to the <code>Synthesizer</code>.
	''' In such cases, the <code>Synthesizer</code> typically responds by sending
	''' a corresponding message to the appropriate <code>MidiChannel</code>, or by
	''' processing the event itself if the event isn't one of the MIDI channel
	''' messages.
	''' <p>
	''' The <code>Synthesizer</code> interface includes methods for loading and
	''' unloading instruments from soundbanks.  An instrument is a specification for synthesizing a
	''' certain type of sound, whether that sound emulates a traditional instrument or is
	''' some kind of sound effect or other imaginary sound. A soundbank is a collection of instruments, organized
	''' by bank and program number (via the instrument's <code>Patch</code> object).
	''' Different <code>Synthesizer</code> classes might implement different sound-synthesis
	''' techniques, meaning that some instruments and not others might be compatible with a
	''' given synthesizer.
	''' Also, synthesizers may have a limited amount of memory for instruments, meaning
	''' that not every soundbank and instrument can be used by every synthesizer, even if
	''' the synthesis technique is compatible.
	''' To see whether the instruments from
	''' a certain soundbank can be played by a given synthesizer, invoke the
	''' <seealso cref="#isSoundbankSupported(Soundbank) isSoundbankSupported"/> method of
	''' <code>Synthesizer</code>.
	''' <p>
	''' "Loading" an instrument means that that instrument becomes available for
	''' synthesizing notes.  The instrument is loaded into the bank and
	''' program location specified by its <code>Patch</code> object.  Loading does
	''' not necessarily mean that subsequently played notes will immediately have
	''' the sound of this newly loaded instrument.  For the instrument to play notes,
	''' one of the synthesizer's <code>MidiChannel</code> objects must receive (or have received)
	''' a program-change message that causes that particular instrument's
	''' bank and program number to be selected.
	''' </summary>
	''' <seealso cref= MidiSystem#getSynthesizer </seealso>
	''' <seealso cref= Soundbank </seealso>
	''' <seealso cref= Instrument </seealso>
	''' <seealso cref= MidiChannel#programChange(int, int) </seealso>
	''' <seealso cref= Receiver </seealso>
	''' <seealso cref= Transmitter </seealso>
	''' <seealso cref= MidiDevice
	''' 
	''' @author Kara Kytle </seealso>
	Public Interface Synthesizer
		Inherits MidiDevice


		' SYNTHESIZER METHODS


		''' <summary>
		''' Obtains the maximum number of notes that this synthesizer can sound simultaneously. </summary>
		''' <returns> the maximum number of simultaneous notes </returns>
		''' <seealso cref= #getVoiceStatus </seealso>
		ReadOnly Property maxPolyphony As Integer


		''' <summary>
		''' Obtains the processing latency incurred by this synthesizer, expressed in
		''' microseconds.  This latency measures the worst-case delay between the
		''' time a MIDI message is delivered to the synthesizer and the time that the
		''' synthesizer actually produces the corresponding result.
		''' <p>
		''' Although the latency is expressed in microseconds, a synthesizer's actual measured
		''' delay may vary over a wider range than this resolution suggests.  For example,
		''' a synthesizer might have a worst-case delay of a few milliseconds or more.
		''' </summary>
		''' <returns> the worst-case delay, in microseconds </returns>
		ReadOnly Property latency As Long


		''' <summary>
		''' Obtains the set of MIDI channels controlled by this synthesizer.  Each
		''' non-null element in the returned array is a <code>MidiChannel</code> that
		''' receives the MIDI messages sent on that channel number.
		''' <p>
		''' The MIDI 1.0 specification provides for 16 channels, so this
		''' method returns an array of at least 16 elements.  However, if this synthesizer
		''' doesn't make use of all 16 channels, some of the elements of the array
		''' might be <code>null</code>, so you should check each element
		''' before using it. </summary>
		''' <returns> an array of the <code>MidiChannel</code> objects managed by this
		''' <code>Synthesizer</code>.  Some of the array elements may be <code>null</code>. </returns>
		ReadOnly Property channels As MidiChannel()


		''' <summary>
		''' Obtains the current status of the voices produced by this synthesizer.
		''' If this class of <code>Synthesizer</code> does not provide voice
		''' information, the returned array will always be of length 0.  Otherwise,
		''' its length is always equal to the total number of voices, as returned by
		''' <code>getMaxPolyphony()</code>.  (See the <code>VoiceStatus</code> class
		''' description for an explanation of synthesizer voices.)
		''' </summary>
		''' <returns> an array of <code>VoiceStatus</code> objects that supply
		''' information about the corresponding synthesizer voices </returns>
		''' <seealso cref= #getMaxPolyphony </seealso>
		''' <seealso cref= VoiceStatus </seealso>
		ReadOnly Property voiceStatus As VoiceStatus()


		''' <summary>
		''' Informs the caller whether this synthesizer is capable of loading
		''' instruments from the specified soundbank.
		''' If the soundbank is unsupported, any attempts to load instruments from
		''' it will result in an <code>IllegalArgumentException</code>. </summary>
		''' <param name="soundbank"> soundbank for which support is queried </param>
		''' <returns> <code>true</code> if the soundbank is supported, otherwise <code>false</code> </returns>
		''' <seealso cref= #loadInstruments </seealso>
		''' <seealso cref= #loadAllInstruments </seealso>
		''' <seealso cref= #unloadInstruments </seealso>
		''' <seealso cref= #unloadAllInstruments </seealso>
		''' <seealso cref= #getDefaultSoundbank </seealso>
		Function isSoundbankSupported(ByVal soundbank As Soundbank) As Boolean


		''' <summary>
		''' Makes a particular instrument available for synthesis.  This instrument
		''' is loaded into the patch location specified by its <code>Patch</code>
		''' object, so that if a program-change message is
		''' received (or has been received) that causes that patch to be selected,
		''' subsequent notes will be played using the sound of
		''' <code>instrument</code>.  If the specified instrument is already loaded,
		''' this method does nothing and returns <code>true</code>.
		''' <p>
		''' The instrument must be part of a soundbank
		''' that this <code>Synthesizer</code> supports.  (To make sure, you can use
		''' the <code>getSoundbank</code> method of <code>Instrument</code> and the
		''' <code>isSoundbankSupported</code> method of <code>Synthesizer</code>.) </summary>
		''' <param name="instrument"> instrument to load </param>
		''' <returns> <code>true</code> if the instrument is successfully loaded (or
		''' already had been), <code>false</code> if the instrument could not be
		''' loaded (for example, if the synthesizer has insufficient
		''' memory to load it) </returns>
		''' <exception cref="IllegalArgumentException"> if this
		''' <code>Synthesizer</code> doesn't support the specified instrument's
		''' soundbank </exception>
		''' <seealso cref= #unloadInstrument </seealso>
		''' <seealso cref= #loadInstruments </seealso>
		''' <seealso cref= #loadAllInstruments </seealso>
		''' <seealso cref= #remapInstrument </seealso>
		''' <seealso cref= SoundbankResource#getSoundbank </seealso>
		''' <seealso cref= MidiChannel#programChange(int, int) </seealso>
		Function loadInstrument(ByVal instrument As Instrument) As Boolean


		''' <summary>
		''' Unloads a particular instrument. </summary>
		''' <param name="instrument"> instrument to unload </param>
		''' <exception cref="IllegalArgumentException"> if this
		''' <code>Synthesizer</code> doesn't support the specified instrument's
		''' soundbank </exception>
		''' <seealso cref= #loadInstrument </seealso>
		''' <seealso cref= #unloadInstruments </seealso>
		''' <seealso cref= #unloadAllInstruments </seealso>
		''' <seealso cref= #getLoadedInstruments </seealso>
		''' <seealso cref= #remapInstrument </seealso>
		Sub unloadInstrument(ByVal instrument As Instrument)


		''' <summary>
		''' Remaps an instrument. Instrument <code>to</code> takes the
		''' place of instrument <code>from</code>.<br>
		''' For example, if <code>from</code> was located at bank number 2,
		''' program number 11, remapping causes that bank and program location
		''' to be occupied instead by <code>to</code>.<br>
		''' If the function succeeds,  instrument <code>from</code> is unloaded.
		''' <p>To cancel the remapping reload instrument <code>from</code> by
		''' invoking one of <seealso cref="#loadInstrument"/>, <seealso cref="#loadInstruments"/>
		''' or <seealso cref="#loadAllInstruments"/>.
		''' </summary>
		''' <param name="from"> the <code>Instrument</code> object to be replaced </param>
		''' <param name="to"> the <code>Instrument</code> object to be used in place
		''' of the old instrument, it should be loaded into the synthesizer </param>
		''' <returns> <code>true</code> if the instrument successfully remapped,
		''' <code>false</code> if feature is not implemented by synthesizer </returns>
		''' <exception cref="IllegalArgumentException"> if instrument
		''' <code>from</code> or instrument <code>to</code> aren't supported by
		''' synthesizer or if instrument <code>to</code> is not loaded </exception>
		''' <exception cref="NullPointerException"> if <code>from</code> or
		''' <code>to</code> parameters have null value </exception>
		''' <seealso cref= #loadInstrument </seealso>
		''' <seealso cref= #loadInstruments </seealso>
		''' <seealso cref= #loadAllInstruments </seealso>
		Function remapInstrument(ByVal [from] As Instrument, ByVal [to] As Instrument) As Boolean


		''' <summary>
		''' Obtains the default soundbank for the synthesizer, if one exists.
		''' (Some synthesizers provide a default or built-in soundbank.)
		''' If a synthesizer doesn't have a default soundbank, instruments must
		''' be loaded explicitly from an external soundbank. </summary>
		''' <returns> default soundbank, or <code>null</code> if one does not exist. </returns>
		''' <seealso cref= #isSoundbankSupported </seealso>
		ReadOnly Property defaultSoundbank As Soundbank


		''' <summary>
		''' Obtains a list of instruments that come with the synthesizer.  These
		''' instruments might be built into the synthesizer, or they might be
		''' part of a default soundbank provided with the synthesizer, etc.
		''' <p>
		''' Note that you don't use this method  to find out which instruments are
		''' currently loaded onto the synthesizer; for that purpose, you use
		''' <code>getLoadedInstruments()</code>.
		''' Nor does the method indicate all the instruments that can be loaded onto
		''' the synthesizer; it only indicates the subset that come with the synthesizer.
		''' To learn whether another instrument can be loaded, you can invoke
		''' <code>isSoundbankSupported()</code>, and if the instrument's
		''' <code>Soundbank</code> is supported, you can try loading the instrument.
		''' </summary>
		''' <returns> list of available instruments. If the synthesizer
		''' has no instruments coming with it, an array of length 0 is returned. </returns>
		''' <seealso cref= #getLoadedInstruments </seealso>
		''' <seealso cref= #isSoundbankSupported(Soundbank) </seealso>
		''' <seealso cref= #loadInstrument </seealso>
		ReadOnly Property availableInstruments As Instrument()


		''' <summary>
		''' Obtains a list of the instruments that are currently loaded onto this
		''' <code>Synthesizer</code>. </summary>
		''' <returns> a list of currently loaded instruments </returns>
		''' <seealso cref= #loadInstrument </seealso>
		''' <seealso cref= #getAvailableInstruments </seealso>
		''' <seealso cref= Soundbank#getInstruments </seealso>
		ReadOnly Property loadedInstruments As Instrument()


		''' <summary>
		''' Loads onto the <code>Synthesizer</code> all instruments contained
		''' in the specified <code>Soundbank</code>. </summary>
		''' <param name="soundbank"> the <code>Soundbank</code> whose are instruments are
		''' to be loaded </param>
		''' <returns> <code>true</code> if the instruments are all successfully loaded (or
		''' already had been), <code>false</code> if any instrument could not be
		''' loaded (for example, if the <code>Synthesizer</code> had insufficient memory) </returns>
		''' <exception cref="IllegalArgumentException"> if the requested soundbank is
		''' incompatible with this synthesizer. </exception>
		''' <seealso cref= #isSoundbankSupported </seealso>
		''' <seealso cref= #loadInstrument </seealso>
		''' <seealso cref= #loadInstruments </seealso>
		Function loadAllInstruments(ByVal soundbank As Soundbank) As Boolean



		''' <summary>
		''' Unloads all instruments contained in the specified <code>Soundbank</code>. </summary>
		''' <param name="soundbank"> soundbank containing instruments to unload </param>
		''' <exception cref="IllegalArgumentException"> thrown if the soundbank is not supported. </exception>
		''' <seealso cref= #isSoundbankSupported </seealso>
		''' <seealso cref= #unloadInstrument </seealso>
		''' <seealso cref= #unloadInstruments </seealso>
		Sub unloadAllInstruments(ByVal soundbank As Soundbank)


		''' <summary>
		''' Loads the instruments referenced by the specified patches, from the
		''' specified <code>Soundbank</code>.  Each of the <code>Patch</code> objects
		''' indicates a bank and program number; the <code>Instrument</code> that
		''' has the matching <code>Patch</code> is loaded into that bank and program
		''' location. </summary>
		''' <param name="soundbank"> the <code>Soundbank</code> containing the instruments to load </param>
		''' <param name="patchList"> list of patches for which instruments should be loaded </param>
		''' <returns> <code>true</code> if the instruments are all successfully loaded (or
		''' already had been), <code>false</code> if any instrument could not be
		''' loaded (for example, if the <code>Synthesizer</code> had insufficient memory) </returns>
		''' <exception cref="IllegalArgumentException"> thrown if the soundbank is not supported. </exception>
		''' <seealso cref= #isSoundbankSupported </seealso>
		''' <seealso cref= Instrument#getPatch </seealso>
		''' <seealso cref= #loadAllInstruments </seealso>
		''' <seealso cref= #loadInstrument </seealso>
		''' <seealso cref= Soundbank#getInstrument(Patch) </seealso>
		''' <seealso cref= Sequence#getPatchList() </seealso>
		Function loadInstruments(ByVal soundbank As Soundbank, ByVal patchList As Patch()) As Boolean

		''' <summary>
		''' Unloads the instruments referenced by the specified patches, from the MIDI sound bank specified. </summary>
		''' <param name="soundbank"> soundbank containing instruments to unload </param>
		''' <param name="patchList"> list of patches for which instruments should be unloaded </param>
		''' <exception cref="IllegalArgumentException"> thrown if the soundbank is not supported.
		''' </exception>
		''' <seealso cref= #unloadInstrument </seealso>
		''' <seealso cref= #unloadAllInstruments </seealso>
		''' <seealso cref= #isSoundbankSupported </seealso>
		''' <seealso cref= Instrument#getPatch </seealso>
		''' <seealso cref= #loadInstruments </seealso>
		Sub unloadInstruments(ByVal soundbank As Soundbank, ByVal patchList As Patch())


		' RECEIVER METHODS

		''' <summary>
		''' Obtains the name of the receiver. </summary>
		''' <returns> receiver name </returns>
		'  public abstract String getName();


		''' <summary>
		''' Opens the receiver. </summary>
		''' <exception cref="MidiUnavailableException"> if the receiver is cannot be opened,
		''' usually because the MIDI device is in use by another application. </exception>
		''' <exception cref="SecurityException"> if the receiver cannot be opened due to security
		''' restrictions. </exception>
		'  public abstract void open() throws MidiUnavailableException, SecurityException;


		''' <summary>
		''' Closes the receiver.
		''' </summary>
		'  public abstract void close();


		''' <summary>
		''' Sends a MIDI event to the receiver. </summary>
		''' <param name="event"> event to send. </param>
		''' <exception cref="IllegalStateException"> if the receiver is not open. </exception>
		'  public void send(MidiEvent event) throws IllegalStateException {
		'
		'  }


		''' <summary>
		''' Obtains the set of controls supported by the
		''' element.  If no controls are supported, returns an
		''' array of length 0. </summary>
		''' <returns> set of controls </returns>
		' $$kk: 03.04.99: josh bloch recommends getting rid of this:
		' what can you really do with a set of untyped controls??
		' $$kk: 03.05.99: i am putting this back in.  for one thing,
		' you can check the length and know whether you should keep
		' looking....
		' public Control[] getControls();

		''' <summary>
		''' Obtains the specified control. </summary>
		''' <param name="controlClass"> class of the requested control </param>
		''' <returns> requested control object, or null if the
		''' control is not supported. </returns>
		' public Control getControl(Class controlClass);
	End Interface

End Namespace