Imports System

'
' * Copyright (c) 1999, 2004, Oracle and/or its affiliates. All rights reserved.
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
	''' A <code>SoundbankResource</code> represents any audio resource stored
	''' in a <code><seealso cref="Soundbank"/></code>.  Common soundbank resources include:
	''' <ul>
	''' <li>Instruments.  An instrument may be specified in a variety of
	''' ways.  However, all soundbanks have some mechanism for defining
	''' instruments.  In doing so, they may reference other resources
	''' stored in the soundbank.  Each instrument has a <code>Patch</code>
	''' which specifies the MIDI program and bank by which it may be
	''' referenced in MIDI messages.  Instrument information may be
	''' stored in <code><seealso cref="Instrument"/></code> objects.
	''' <li>Audio samples.  A sample typically is a sampled audio waveform
	''' which contains a short sound recording whose duration is a fraction of
	''' a second, or at most a few seconds.  These audio samples may be
	''' used by a <code><seealso cref="Synthesizer"/></code> to synthesize sound in response to MIDI
	''' commands, or extracted for use by an application.
	''' (The terminology reflects musicians' use of the word "sample" to refer
	''' collectively to a series of contiguous audio samples or frames, rather than
	''' to a single, instantaneous sample.)
	''' The data class for an audio sample will be an object
	''' that encapsulates the audio sample data itself and information
	''' about how to interpret it (the format of the audio data), such
	''' as an <code><seealso cref="javax.sound.sampled.AudioInputStream"/></code>.     </li>
	''' <li>Embedded sequences.  A sound bank may contain built-in
	''' song data stored in a data object such as a <code><seealso cref="Sequence"/></code>.
	''' </ul>
	''' <p>
	''' Synthesizers that use wavetable synthesis or related
	''' techniques play back the audio in a sample when
	''' synthesizing notes, often when emulating the real-world instrument that
	''' was originally recorded.  However, there is not necessarily a one-to-one
	''' correspondence between the <code>Instruments</code> and samples
	''' in a <code>Soundbank</code>.  A single <code>Instrument</code> can use
	''' multiple SoundbankResources (typically for notes of dissimilar pitch or
	''' brightness).  Also, more than one <code>Instrument</code> can use the same
	''' sample.
	''' 
	''' @author Kara Kytle
	''' </summary>

	Public MustInherit Class SoundbankResource


		''' <summary>
		''' The sound bank that contains the <code>SoundbankResources</code>
		''' </summary>
		Private ReadOnly soundBank As Soundbank


		''' <summary>
		''' The name of the <code>SoundbankResource</code>
		''' </summary>
		Private ReadOnly name As String


		''' <summary>
		''' The class used to represent the sample's data.
		''' </summary>
		Private ReadOnly dataClass As Type


		''' <summary>
		''' The wavetable index.
		''' </summary>
		'private final int index;


		''' <summary>
		''' Constructs a new <code>SoundbankResource</code> from the given sound bank
		''' and wavetable index.  (Setting the <code>SoundbankResource's</code> name,
		''' sampled audio data, and instruments is a subclass responsibility.) </summary>
		''' <param name="soundBank"> the sound bank containing this <code>SoundbankResource</code> </param>
		''' <param name="name"> the name of the sample </param>
		''' <param name="dataClass"> the class used to represent the sample's data
		''' </param>
		''' <seealso cref= #getSoundbank </seealso>
		''' <seealso cref= #getName </seealso>
		''' <seealso cref= #getDataClass </seealso>
		''' <seealso cref= #getData </seealso>
		Protected Friend Sub New(ByVal soundBank As Soundbank, ByVal name As String, ByVal dataClass As Type)

			Me.soundBank = soundBank
			Me.name = name
			Me.dataClass = dataClass
		End Sub


		''' <summary>
		''' Obtains the sound bank that contains this <code>SoundbankResource</code>. </summary>
		''' <returns> the sound bank in which this <code>SoundbankResource</code> is stored </returns>
		Public Overridable Property soundbank As Soundbank
			Get
				Return soundBank
			End Get
		End Property


		''' <summary>
		''' Obtains the name of the resource.  This should generally be a string
		''' descriptive of the resource. </summary>
		''' <returns> the instrument's name </returns>
		Public Overridable Property name As String
			Get
				Return name
			End Get
		End Property


		''' <summary>
		''' Obtains the class used by this sample to represent its data.
		''' The object returned by <code>getData</code> will be of this
		''' class.  If this <code>SoundbankResource</code> object does not support
		''' direct access to its data, returns <code>null</code>. </summary>
		''' <returns> the class used to represent the sample's data, or
		''' null if the data is not accessible </returns>
		Public Overridable Property dataClass As Type
			Get
				Return dataClass
			End Get
		End Property


		''' <summary>
		''' Obtains the sampled audio that is stored in this <code>SoundbankResource</code>.
		''' The type of object returned depends on the implementation of the
		''' concrete class, and may be queried using <code>getDataClass</code>. </summary>
		''' <returns> an object containing the sampled audio data </returns>
		''' <seealso cref= #getDataClass </seealso>
		Public MustOverride ReadOnly Property data As Object


		''' <summary>
		''' Obtains the index of this <code>SoundbankResource</code> into the
		''' <code>Soundbank's</code> set of <code>SoundbankResources</code>. </summary>
		''' <returns> the wavetable index </returns>
		'public int getIndex() {
		'  return index;
		'}


		''' <summary>
		''' Obtains a list of the instruments in the sound bank that use the
		''' <code>SoundbankResource</code> for sound synthesis. </summary>
		''' <returns> an array of <code>Instruments</code> that reference this
		''' <code>SoundbankResource</code>
		''' </returns>
		''' <seealso cref= Instrument#getSamples </seealso>
		'public abstract Instrument[] getInstruments();
	End Class

End Namespace