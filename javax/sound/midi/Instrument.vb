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
	''' An instrument is a sound-synthesis algorithm with certain parameter
	''' settings, usually designed to emulate a specific real-world
	''' musical instrument or to achieve a specific sort of sound effect.
	''' Instruments are typically stored in collections called soundbanks.
	''' Before the instrument can be used to play notes, it must first be loaded
	''' onto a synthesizer, and then it must be selected for use on
	''' one or more channels, via a program-change command.  MIDI notes
	''' that are subsequently received on those channels will be played using
	''' the sound of the selected instrument.
	''' </summary>
	''' <seealso cref= Soundbank </seealso>
	''' <seealso cref= Soundbank#getInstruments </seealso>
	''' <seealso cref= Patch </seealso>
	''' <seealso cref= Synthesizer#loadInstrument(Instrument) </seealso>
	''' <seealso cref= MidiChannel#programChange(int, int)
	''' @author Kara Kytle </seealso>

	Public MustInherit Class Instrument
		Inherits SoundbankResource


		''' <summary>
		''' Instrument patch
		''' </summary>
		Private ReadOnly patch As Patch


		''' <summary>
		''' Constructs a new MIDI instrument from the specified <code>Patch</code>.
		''' When a subsequent request is made to load the
		''' instrument, the sound bank will search its contents for this instrument's <code>Patch</code>,
		''' and the instrument will be loaded into the synthesizer at the
		''' bank and program location indicated by the <code>Patch</code> object. </summary>
		''' <param name="soundbank"> sound bank containing the instrument </param>
		''' <param name="patch"> the patch of this instrument </param>
		''' <param name="name"> the name of this instrument </param>
		''' <param name="dataClass"> the class used to represent the sample's data.
		''' </param>
		''' <seealso cref= Synthesizer#loadInstrument(Instrument) </seealso>
		Protected Friend Sub New(ByVal soundbank As Soundbank, ByVal patch As Patch, ByVal name As String, ByVal dataClass As Type)

			MyBase.New(soundbank, name, dataClass)
			Me.patch = patch
		End Sub


		''' <summary>
		''' Obtains the <code>Patch</code> object that indicates the bank and program
		''' numbers where this instrument is to be stored in the synthesizer. </summary>
		''' <returns> this instrument's patch </returns>
		Public Overridable Property patch As Patch
			Get
				Return patch
			End Get
		End Property
	End Class

End Namespace