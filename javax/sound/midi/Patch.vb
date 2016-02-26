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

Namespace javax.sound.midi


	''' <summary>
	''' A <code>Patch</code> object represents a location, on a MIDI
	''' synthesizer, into which a single instrument is stored (loaded).
	''' Every <code>Instrument</code> object has its own <code>Patch</code>
	''' object that specifies the memory location
	''' into which that instrument should be loaded. The
	''' location is specified abstractly by a bank index and a program number (not by
	''' any scheme that directly refers to a specific address or offset in RAM).
	''' This is a hierarchical indexing scheme: MIDI provides for up to 16384 banks,
	''' each of which contains up to 128 program locations.  For example, a
	''' minimal sort of synthesizer might have only one bank of instruments, and
	''' only 32 instruments (programs) in that bank.
	''' <p>
	''' To select what instrument should play the notes on a particular MIDI
	''' channel, two kinds of MIDI message are used that specify a patch location:
	''' a bank-select command, and a program-change channel command.  The Java Sound
	''' equivalent is the
	''' <seealso cref="MidiChannel#programChange(int, int) programChange(int, int)"/>
	''' method of <code>MidiChannel</code>.
	''' </summary>
	''' <seealso cref= Instrument </seealso>
	''' <seealso cref= Instrument#getPatch() </seealso>
	''' <seealso cref= MidiChannel#programChange(int, int) </seealso>
	''' <seealso cref= Synthesizer#loadInstruments(Soundbank, Patch[]) </seealso>
	''' <seealso cref= Soundbank </seealso>
	''' <seealso cref= Sequence#getPatchList()
	''' 
	''' @author Kara Kytle </seealso>

	Public Class Patch


		''' <summary>
		''' Bank index
		''' </summary>
		Private ReadOnly bank As Integer


		''' <summary>
		''' Program change number
		''' </summary>
		Private ReadOnly program As Integer


		''' <summary>
		''' Constructs a new patch object from the specified bank and program
		''' numbers. </summary>
		''' <param name="bank"> the bank index (in the range from 0 to 16383) </param>
		''' <param name="program"> the program index (in the range from 0 to 127) </param>
		Public Sub New(ByVal bank As Integer, ByVal program As Integer)

			Me.bank = bank
			Me.program = program
		End Sub


		''' <summary>
		''' Returns the number of the bank that contains the instrument
		''' whose location this <code>Patch</code> specifies. </summary>
		''' <returns> the bank number, whose range is from 0 to 16383 </returns>
		''' <seealso cref= MidiChannel#programChange(int, int) </seealso>
		Public Overridable Property bank As Integer
			Get
    
				Return bank
			End Get
		End Property


		''' <summary>
		''' Returns the index, within
		''' a bank, of the instrument whose location this <code>Patch</code> specifies. </summary>
		''' <returns> the instrument's program number, whose range is from 0 to 127
		''' </returns>
		''' <seealso cref= MidiChannel#getProgram </seealso>
		''' <seealso cref= MidiChannel#programChange(int) </seealso>
		''' <seealso cref= MidiChannel#programChange(int, int) </seealso>
		Public Overridable Property program As Integer
			Get
    
				Return program
			End Get
		End Property
	End Class

End Namespace