Imports Microsoft.VisualBasic
Imports System
Imports System.Collections

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
	''' The <code>MidiSystem</code> class provides access to the installed MIDI
	''' system resources, including devices such as synthesizers, sequencers, and
	''' MIDI input and output ports.  A typical simple MIDI application might
	''' begin by invoking one or more <code>MidiSystem</code> methods to learn
	''' what devices are installed and to obtain the ones needed in that
	''' application.
	''' <p>
	''' The class also has methods for reading files, streams, and  URLs that
	''' contain standard MIDI file data or soundbanks.  You can query the
	''' <code>MidiSystem</code> for the format of a specified MIDI file.
	''' <p>
	''' You cannot instantiate a <code>MidiSystem</code>; all the methods are
	''' static.
	''' 
	''' <p>Properties can be used to specify default MIDI devices.
	''' Both system properties and a properties file are considered.
	''' The <code>sound.properties</code> properties file is read from
	''' an implementation-specific location (typically it is the <code>lib</code>
	''' directory in the Java installation directory).
	''' If a property exists both as a system property and in the
	''' properties file, the system property takes precedence. If none is
	''' specified, a suitable default is chosen among the available devices.
	''' The syntax of the properties file is specified in
	''' <seealso cref="java.util.Properties#load(InputStream) Properties.load"/>. The
	''' following table lists the available property keys and which methods
	''' consider them:
	''' 
	''' <table border=0>
	'''  <caption>MIDI System Property Keys</caption>
	'''  <tr>
	'''   <th>Property Key</th>
	'''   <th>Interface</th>
	'''   <th>Affected Method</th>
	'''  </tr>
	'''  <tr>
	'''   <td><code>javax.sound.midi.Receiver</code></td>
	'''   <td><seealso cref="Receiver"/></td>
	'''   <td><seealso cref="#getReceiver"/></td>
	'''  </tr>
	'''  <tr>
	'''   <td><code>javax.sound.midi.Sequencer</code></td>
	'''   <td><seealso cref="Sequencer"/></td>
	'''   <td><seealso cref="#getSequencer"/></td>
	'''  </tr>
	'''  <tr>
	'''   <td><code>javax.sound.midi.Synthesizer</code></td>
	'''   <td><seealso cref="Synthesizer"/></td>
	'''   <td><seealso cref="#getSynthesizer"/></td>
	'''  </tr>
	'''  <tr>
	'''   <td><code>javax.sound.midi.Transmitter</code></td>
	'''   <td><seealso cref="Transmitter"/></td>
	'''   <td><seealso cref="#getTransmitter"/></td>
	'''  </tr>
	''' </table>
	''' 
	''' The property value consists of the provider class name
	''' and the device name, separated by the hash mark (&quot;#&quot;).
	''' The provider class name is the fully-qualified
	''' name of a concrete {@link javax.sound.midi.spi.MidiDeviceProvider
	''' MIDI device provider} class. The device name is matched against
	''' the <code>String</code> returned by the <code>getName</code>
	''' method of <code>MidiDevice.Info</code>.
	''' Either the class name, or the device name may be omitted.
	''' If only the class name is specified, the trailing hash mark
	''' is optional.
	''' 
	''' <p>If the provider class is specified, and it can be
	''' successfully retrieved from the installed providers,
	''' the list of
	''' <code>MidiDevice.Info</code> objects is retrieved
	''' from the provider. Otherwise, or when these devices
	''' do not provide a subsequent match, the list is retrieved
	''' from <seealso cref="#getMidiDeviceInfo"/> to contain
	''' all available <code>MidiDevice.Info</code> objects.
	''' 
	''' <p>If a device name is specified, the resulting list of
	''' <code>MidiDevice.Info</code> objects is searched:
	''' the first one with a matching name, and whose
	''' <code>MidiDevice</code> implements the
	''' respective interface, will be returned.
	''' If no matching <code>MidiDevice.Info</code> object
	''' is found, or the device name is not specified,
	''' the first suitable device from the resulting
	''' list will be returned. For Sequencer and Synthesizer,
	''' a device is suitable if it implements the respective
	''' interface; whereas for Receiver and Transmitter, a device is
	''' suitable if it
	''' implements neither Sequencer nor Synthesizer and provides
	''' at least one Receiver or Transmitter, respectively.
	''' 
	''' For example, the property <code>javax.sound.midi.Receiver</code>
	''' with a value
	''' <code>&quot;com.sun.media.sound.MidiProvider#SunMIDI1&quot;</code>
	''' will have the following consequences when
	''' <code>getReceiver</code> is called:
	''' if the class <code>com.sun.media.sound.MidiProvider</code> exists
	''' in the list of installed MIDI device providers,
	''' the first <code>Receiver</code> device with name
	''' <code>&quot;SunMIDI1&quot;</code> will be returned. If it cannot
	''' be found, the first <code>Receiver</code> from that provider
	''' will be returned, regardless of name.
	''' If there is none, the first <code>Receiver</code> with name
	''' <code>&quot;SunMIDI1&quot;</code> in the list of all devices
	''' (as returned by <code>getMidiDeviceInfo</code>) will be returned,
	''' or, if not found, the first <code>Receiver</code> that can
	''' be found in the list of all devices is returned.
	''' If that fails, too, a <code>MidiUnavailableException</code>
	''' is thrown.
	''' 
	''' @author Kara Kytle
	''' @author Florian Bomers
	''' @author Matthias Pfisterer
	''' </summary>
	Public Class MidiSystem

		''' <summary>
		''' Private no-args constructor for ensuring against instantiation.
		''' </summary>
		Private Sub New()
		End Sub


		''' <summary>
		''' Obtains an array of information objects representing
		''' the set of all MIDI devices available on the system.
		''' A returned information object can then be used to obtain the
		''' corresponding device object, by invoking
		''' <seealso cref="#getMidiDevice(MidiDevice.Info) getMidiDevice"/>.
		''' </summary>
		''' <returns> an array of <code>MidiDevice.Info</code> objects, one
		''' for each installed MIDI device.  If no such devices are installed,
		''' an array of length 0 is returned. </returns>
		Public Property Shared midiDeviceInfo As MidiDevice.Info()
			Get
				Dim allInfos As IList = New ArrayList
				Dim ___providers As IList = midiDeviceProviders
    
				For i As Integer = 0 To ___providers.Count - 1
					Dim provider As javax.sound.midi.spi.MidiDeviceProvider = CType(___providers(i), javax.sound.midi.spi.MidiDeviceProvider)
					Dim tmpinfo As MidiDevice.Info() = provider.deviceInfo
					For j As Integer = 0 To tmpinfo.Length - 1
						allInfos.Add(tmpinfo(j))
					Next j
				Next i
				Dim infosArray As MidiDevice.Info() = CType(allInfos.ToArray(GetType(MidiDevice.Info)), MidiDevice.Info())
				Return infosArray
			End Get
		End Property


		''' <summary>
		''' Obtains the requested MIDI device.
		''' </summary>
		''' <param name="info"> a device information object representing the desired device. </param>
		''' <returns> the requested device </returns>
		''' <exception cref="MidiUnavailableException"> if the requested device is not available
		''' due to resource restrictions </exception>
		''' <exception cref="IllegalArgumentException"> if the info object does not represent
		''' a MIDI device installed on the system </exception>
		''' <seealso cref= #getMidiDeviceInfo </seealso>
		Public Shared Function getMidiDevice(ByVal info As MidiDevice.Info) As MidiDevice
			Dim ___providers As IList = midiDeviceProviders

			For i As Integer = 0 To ___providers.Count - 1
				Dim provider As javax.sound.midi.spi.MidiDeviceProvider = CType(___providers(i), javax.sound.midi.spi.MidiDeviceProvider)
				If provider.isDeviceSupported(info) Then
					Dim device As MidiDevice = provider.getDevice(info)
					Return device
				End If
			Next i
			Throw New System.ArgumentException("Requested device not installed: " & info)
		End Function


		''' <summary>
		''' Obtains a MIDI receiver from an external MIDI port
		''' or other default device.
		''' The returned receiver always implements
		''' the {@code MidiDeviceReceiver} interface.
		''' 
		''' <p>If the system property
		''' <code>javax.sound.midi.Receiver</code>
		''' is defined or it is defined in the file &quot;sound.properties&quot;,
		''' it is used to identify the device that provides the default receiver.
		''' For details, refer to the <seealso cref="MidiSystem class description"/>.
		''' 
		''' If a suitable MIDI port is not available, the Receiver is
		''' retrieved from an installed synthesizer.
		''' 
		''' <p>If a native receiver provided by the default device does not implement
		''' the {@code MidiDeviceReceiver} interface, it will be wrapped in a
		''' wrapper class that implements the {@code MidiDeviceReceiver} interface.
		''' The corresponding {@code Receiver} method calls will be forwarded
		''' to the native receiver.
		''' 
		''' <p>If this method returns successfully, the {@link
		''' javax.sound.midi.MidiDevice MidiDevice} the
		''' <code>Receiver</code> belongs to is opened implicitly, if it is
		''' not already open. It is possible to close an implicitly opened
		''' device by calling <seealso cref="javax.sound.midi.Receiver#close close"/>
		''' on the returned <code>Receiver</code>. All open <code>Receiver</code>
		''' instances have to be closed in order to release system resources
		''' hold by the <code>MidiDevice</code>. For a
		''' detailed description of open/close behaviour see the class
		''' description of <seealso cref="javax.sound.midi.MidiDevice MidiDevice"/>.
		''' 
		''' </summary>
		''' <returns> the default MIDI receiver </returns>
		''' <exception cref="MidiUnavailableException"> if the default receiver is not
		'''         available due to resource restrictions,
		'''         or no device providing receivers is installed in the system </exception>
		Public Property Shared receiver As Receiver
			Get
				' may throw MidiUnavailableException
				Dim device As MidiDevice = getDefaultDeviceWrapper(GetType(Receiver))
				Dim ___receiver As Receiver
				If TypeOf device Is com.sun.media.sound.ReferenceCountingDevice Then
					___receiver = CType(device, com.sun.media.sound.ReferenceCountingDevice).receiverReferenceCounting
				Else
					___receiver = device.receiver
				End If
				If Not(TypeOf ___receiver Is MidiDeviceReceiver) Then ___receiver = New com.sun.media.sound.MidiDeviceReceiverEnvelope(device, ___receiver)
				Return ___receiver
			End Get
		End Property


		''' <summary>
		''' Obtains a MIDI transmitter from an external MIDI port
		''' or other default source.
		''' The returned transmitter always implements
		''' the {@code MidiDeviceTransmitter} interface.
		''' 
		''' <p>If the system property
		''' <code>javax.sound.midi.Transmitter</code>
		''' is defined or it is defined in the file &quot;sound.properties&quot;,
		''' it is used to identify the device that provides the default transmitter.
		''' For details, refer to the <seealso cref="MidiSystem class description"/>.
		''' 
		''' <p>If a native transmitter provided by the default device does not implement
		''' the {@code MidiDeviceTransmitter} interface, it will be wrapped in a
		''' wrapper class that implements the {@code MidiDeviceTransmitter} interface.
		''' The corresponding {@code Transmitter} method calls will be forwarded
		''' to the native transmitter.
		''' 
		''' <p>If this method returns successfully, the {@link
		''' javax.sound.midi.MidiDevice MidiDevice} the
		''' <code>Transmitter</code> belongs to is opened implicitly, if it
		''' is not already open. It is possible to close an implicitly
		''' opened device by calling {@link
		''' javax.sound.midi.Transmitter#close close} on the returned
		''' <code>Transmitter</code>. All open <code>Transmitter</code>
		''' instances have to be closed in order to release system resources
		''' hold by the <code>MidiDevice</code>. For a detailed description
		''' of open/close behaviour see the class description of {@link
		''' javax.sound.midi.MidiDevice MidiDevice}.
		''' </summary>
		''' <returns> the default MIDI transmitter </returns>
		''' <exception cref="MidiUnavailableException"> if the default transmitter is not
		'''         available due to resource restrictions,
		'''         or no device providing transmitters is installed in the system </exception>
		Public Property Shared transmitter As Transmitter
			Get
				' may throw MidiUnavailableException
				Dim device As MidiDevice = getDefaultDeviceWrapper(GetType(Transmitter))
				Dim ___transmitter As Transmitter
				If TypeOf device Is com.sun.media.sound.ReferenceCountingDevice Then
					___transmitter = CType(device, com.sun.media.sound.ReferenceCountingDevice).transmitterReferenceCounting
				Else
					___transmitter = device.transmitter
				End If
				If Not(TypeOf ___transmitter Is MidiDeviceTransmitter) Then ___transmitter = New com.sun.media.sound.MidiDeviceTransmitterEnvelope(device, ___transmitter)
				Return ___transmitter
			End Get
		End Property


		''' <summary>
		''' Obtains the default synthesizer.
		''' 
		''' <p>If the system property
		''' <code>javax.sound.midi.Synthesizer</code>
		''' is defined or it is defined in the file &quot;sound.properties&quot;,
		''' it is used to identify the default synthesizer.
		''' For details, refer to the <seealso cref="MidiSystem class description"/>.
		''' </summary>
		''' <returns> the default synthesizer </returns>
		''' <exception cref="MidiUnavailableException"> if the synthesizer is not
		'''         available due to resource restrictions,
		'''         or no synthesizer is installed in the system </exception>
		Public Property Shared synthesizer As Synthesizer
			Get
				' may throw MidiUnavailableException
				Return CType(getDefaultDeviceWrapper(GetType(Synthesizer)), Synthesizer)
			End Get
		End Property


		''' <summary>
		''' Obtains the default <code>Sequencer</code>, connected to
		''' a default device.
		''' The returned <code>Sequencer</code> instance is
		''' connected to the default <code>Synthesizer</code>,
		''' as returned by <seealso cref="#getSynthesizer"/>.
		''' If there is no <code>Synthesizer</code>
		''' available, or the default <code>Synthesizer</code>
		''' cannot be opened, the <code>sequencer</code> is connected
		''' to the default <code>Receiver</code>, as returned
		''' by <seealso cref="#getReceiver"/>.
		''' The connection is made by retrieving a <code>Transmitter</code>
		''' instance from the <code>Sequencer</code> and setting its
		''' <code>Receiver</code>.
		''' Closing and re-opening the sequencer will restore the
		''' connection to the default device.
		''' 
		''' <p>This method is equivalent to calling
		''' <code>getSequencer(true)</code>.
		''' 
		''' <p>If the system property
		''' <code>javax.sound.midi.Sequencer</code>
		''' is defined or it is defined in the file &quot;sound.properties&quot;,
		''' it is used to identify the default sequencer.
		''' For details, refer to the <seealso cref="MidiSystem class description"/>.
		''' </summary>
		''' <returns> the default sequencer, connected to a default Receiver </returns>
		''' <exception cref="MidiUnavailableException"> if the sequencer is not
		'''         available due to resource restrictions,
		'''         or there is no <code>Receiver</code> available by any
		'''         installed <code>MidiDevice</code>,
		'''         or no sequencer is installed in the system. </exception>
		''' <seealso cref= #getSequencer(boolean) </seealso>
		''' <seealso cref= #getSynthesizer </seealso>
		''' <seealso cref= #getReceiver </seealso>
		Public Property Shared sequencer As Sequencer
			Get
				Return getSequencer(True)
			End Get
		End Property



		''' <summary>
		''' Obtains the default <code>Sequencer</code>, optionally
		''' connected to a default device.
		''' 
		''' <p>If <code>connected</code> is true, the returned
		''' <code>Sequencer</code> instance is
		''' connected to the default <code>Synthesizer</code>,
		''' as returned by <seealso cref="#getSynthesizer"/>.
		''' If there is no <code>Synthesizer</code>
		''' available, or the default <code>Synthesizer</code>
		''' cannot be opened, the <code>sequencer</code> is connected
		''' to the default <code>Receiver</code>, as returned
		''' by <seealso cref="#getReceiver"/>.
		''' The connection is made by retrieving a <code>Transmitter</code>
		''' instance from the <code>Sequencer</code> and setting its
		''' <code>Receiver</code>.
		''' Closing and re-opening the sequencer will restore the
		''' connection to the default device.
		''' 
		''' <p>If <code>connected</code> is false, the returned
		''' <code>Sequencer</code> instance is not connected, it
		''' has no open <code>Transmitters</code>. In order to
		''' play the sequencer on a MIDI device, or a <code>Synthesizer</code>,
		''' it is necessary to get a <code>Transmitter</code> and set its
		''' <code>Receiver</code>.
		''' 
		''' <p>If the system property
		''' <code>javax.sound.midi.Sequencer</code>
		''' is defined or it is defined in the file "sound.properties",
		''' it is used to identify the default sequencer.
		''' For details, refer to the <seealso cref="MidiSystem class description"/>.
		''' </summary>
		''' <param name="connected"> whether or not the returned {@code Sequencer}
		''' is connected to the default {@code Synthesizer} </param>
		''' <returns> the default sequencer </returns>
		''' <exception cref="MidiUnavailableException"> if the sequencer is not
		'''         available due to resource restrictions,
		'''         or no sequencer is installed in the system,
		'''         or if <code>connected</code> is true, and there is
		'''         no <code>Receiver</code> available by any installed
		'''         <code>MidiDevice</code> </exception>
		''' <seealso cref= #getSynthesizer </seealso>
		''' <seealso cref= #getReceiver
		''' @since 1.5 </seealso>
		Public Shared Function getSequencer(ByVal connected As Boolean) As Sequencer
			Dim seq As Sequencer = CType(getDefaultDeviceWrapper(GetType(Sequencer)), Sequencer)

			If connected Then
				' IMPORTANT: this code needs to be synch'ed with
				'            all AutoConnectSequencer instances,
				'            (e.g. RealTimeSequencer) because the
				'            same algorithm for synth retrieval
				'            needs to be used!

				Dim rec As Receiver = Nothing
				Dim mue As MidiUnavailableException = Nothing

				' first try to connect to the default synthesizer
				Try
					Dim synth As Synthesizer = synthesizer
					If TypeOf synth Is com.sun.media.sound.ReferenceCountingDevice Then
						rec = CType(synth, com.sun.media.sound.ReferenceCountingDevice).receiverReferenceCounting
					Else
						synth.open()
						Try
							rec = synth.receiver
						Finally
							' make sure that the synth is properly closed
							If rec Is Nothing Then synth.close()
						End Try
					End If
				Catch e As MidiUnavailableException
					' something went wrong with synth
					If TypeOf e Is MidiUnavailableException Then mue = CType(e, MidiUnavailableException)
				End Try
				If rec Is Nothing Then
					' then try to connect to the default Receiver
					Try
						rec = MidiSystem.receiver
					Catch e As Exception
						' something went wrong. Nothing to do then!
						If TypeOf e Is MidiUnavailableException Then mue = CType(e, MidiUnavailableException)
					End Try
				End If
				If rec IsNot Nothing Then
					seq.transmitter.receiver = rec
					If TypeOf seq Is com.sun.media.sound.AutoConnectSequencer Then CType(seq, com.sun.media.sound.AutoConnectSequencer).autoConnect = rec
				Else
					If mue IsNot Nothing Then Throw mue
					Throw New MidiUnavailableException("no receiver available")
				End If
			End If
			Return seq
		End Function




		''' <summary>
		''' Constructs a MIDI sound bank by reading it from the specified stream.
		''' The stream must point to
		''' a valid MIDI soundbank file.  In general, MIDI soundbank providers may
		''' need to read some data from the stream before determining whether they
		''' support it.  These parsers must
		''' be able to mark the stream, read enough data to determine whether they
		''' support the stream, and, if not, reset the stream's read pointer to
		''' its original position.  If the input stream does not support this,
		''' this method may fail with an IOException. </summary>
		''' <param name="stream"> the source of the sound bank data. </param>
		''' <returns> the sound bank </returns>
		''' <exception cref="InvalidMidiDataException"> if the stream does not point to
		''' valid MIDI soundbank data recognized by the system </exception>
		''' <exception cref="IOException"> if an I/O error occurred when loading the soundbank </exception>
		''' <seealso cref= InputStream#markSupported </seealso>
		''' <seealso cref= InputStream#mark </seealso>
		Public Shared Function getSoundbank(ByVal stream As java.io.InputStream) As Soundbank

			Dim sp As javax.sound.midi.spi.SoundbankReader = Nothing
			Dim s As Soundbank = Nothing

			Dim ___providers As IList = soundbankReaders

			For i As Integer = 0 To ___providers.Count - 1
				sp = CType(___providers(i), javax.sound.midi.spi.SoundbankReader)
				s = sp.getSoundbank(stream)

				If s IsNot Nothing Then Return s
			Next i
			Throw New InvalidMidiDataException("cannot get soundbank from stream")

		End Function


		''' <summary>
		''' Constructs a <code>Soundbank</code> by reading it from the specified URL.
		''' The URL must point to a valid MIDI soundbank file.
		''' </summary>
		''' <param name="url"> the source of the sound bank data </param>
		''' <returns> the sound bank </returns>
		''' <exception cref="InvalidMidiDataException"> if the URL does not point to valid MIDI
		''' soundbank data recognized by the system </exception>
		''' <exception cref="IOException"> if an I/O error occurred when loading the soundbank </exception>
		Public Shared Function getSoundbank(ByVal url As java.net.URL) As Soundbank

			Dim sp As javax.sound.midi.spi.SoundbankReader = Nothing
			Dim s As Soundbank = Nothing

			Dim ___providers As IList = soundbankReaders

			For i As Integer = 0 To ___providers.Count - 1
				sp = CType(___providers(i), javax.sound.midi.spi.SoundbankReader)
				s = sp.getSoundbank(url)

				If s IsNot Nothing Then Return s
			Next i
			Throw New InvalidMidiDataException("cannot get soundbank from stream")

		End Function


		''' <summary>
		''' Constructs a <code>Soundbank</code> by reading it from the specified
		''' <code>File</code>.
		''' The <code>File</code> must point to a valid MIDI soundbank file.
		''' </summary>
		''' <param name="file"> the source of the sound bank data </param>
		''' <returns> the sound bank </returns>
		''' <exception cref="InvalidMidiDataException"> if the <code>File</code> does not
		''' point to valid MIDI soundbank data recognized by the system </exception>
		''' <exception cref="IOException"> if an I/O error occurred when loading the soundbank </exception>
		Public Shared Function getSoundbank(ByVal file As java.io.File) As Soundbank

			Dim sp As javax.sound.midi.spi.SoundbankReader = Nothing
			Dim s As Soundbank = Nothing

			Dim ___providers As IList = soundbankReaders

			For i As Integer = 0 To ___providers.Count - 1
				sp = CType(___providers(i), javax.sound.midi.spi.SoundbankReader)
				s = sp.getSoundbank(file)

				If s IsNot Nothing Then Return s
			Next i
			Throw New InvalidMidiDataException("cannot get soundbank from stream")
		End Function



		''' <summary>
		''' Obtains the MIDI file format of the data in the specified input stream.
		''' The stream must point to valid MIDI file data for a file type recognized
		''' by the system.
		''' <p>
		''' This method and/or the code it invokes may need to read some data from
		''' the stream to determine whether its data format is supported.  The
		''' implementation may therefore
		''' need to mark the stream, read enough data to determine whether it is in
		''' a supported format, and reset the stream's read pointer to its original
		''' position.  If the input stream does not permit this set of operations,
		''' this method may fail with an <code>IOException</code>.
		''' <p>
		''' This operation can only succeed for files of a type which can be parsed
		''' by an installed file reader.  It may fail with an InvalidMidiDataException
		''' even for valid files if no compatible file reader is installed.  It
		''' will also fail with an InvalidMidiDataException if a compatible file reader
		''' is installed, but encounters errors while determining the file format.
		''' </summary>
		''' <param name="stream"> the input stream from which file format information
		''' should be extracted </param>
		''' <returns> an <code>MidiFileFormat</code> object describing the MIDI file
		''' format </returns>
		''' <exception cref="InvalidMidiDataException"> if the stream does not point to valid
		''' MIDI file data recognized by the system </exception>
		''' <exception cref="IOException"> if an I/O exception occurs while accessing the
		''' stream </exception>
		''' <seealso cref= #getMidiFileFormat(URL) </seealso>
		''' <seealso cref= #getMidiFileFormat(File) </seealso>
		''' <seealso cref= InputStream#markSupported </seealso>
		''' <seealso cref= InputStream#mark </seealso>
		Public Shared Function getMidiFileFormat(ByVal stream As java.io.InputStream) As MidiFileFormat

			Dim ___providers As IList = midiFileReaders
			Dim format As MidiFileFormat = Nothing

			For i As Integer = 0 To ___providers.Count - 1
				Dim reader As javax.sound.midi.spi.MidiFileReader = CType(___providers(i), javax.sound.midi.spi.MidiFileReader)
				Try
					format = reader.getMidiFileFormat(stream) ' throws IOException
					Exit For
				Catch e As InvalidMidiDataException
					Continue For
				End Try
			Next i

			If format Is Nothing Then
				Throw New InvalidMidiDataException("input stream is not a supported file type")
			Else
				Return format
			End If
		End Function


		''' <summary>
		''' Obtains the MIDI file format of the data in the specified URL.  The URL
		''' must point to valid MIDI file data for a file type recognized
		''' by the system.
		''' <p>
		''' This operation can only succeed for files of a type which can be parsed
		''' by an installed file reader.  It may fail with an InvalidMidiDataException
		''' even for valid files if no compatible file reader is installed.  It
		''' will also fail with an InvalidMidiDataException if a compatible file reader
		''' is installed, but encounters errors while determining the file format.
		''' </summary>
		''' <param name="url"> the URL from which file format information should be
		''' extracted </param>
		''' <returns> a <code>MidiFileFormat</code> object describing the MIDI file
		''' format </returns>
		''' <exception cref="InvalidMidiDataException"> if the URL does not point to valid MIDI
		''' file data recognized by the system </exception>
		''' <exception cref="IOException"> if an I/O exception occurs while accessing the URL
		''' </exception>
		''' <seealso cref= #getMidiFileFormat(InputStream) </seealso>
		''' <seealso cref= #getMidiFileFormat(File) </seealso>
		Public Shared Function getMidiFileFormat(ByVal url As java.net.URL) As MidiFileFormat

			Dim ___providers As IList = midiFileReaders
			Dim format As MidiFileFormat = Nothing

			For i As Integer = 0 To ___providers.Count - 1
				Dim reader As javax.sound.midi.spi.MidiFileReader = CType(___providers(i), javax.sound.midi.spi.MidiFileReader)
				Try
					format = reader.getMidiFileFormat(url) ' throws IOException
					Exit For
				Catch e As InvalidMidiDataException
					Continue For
				End Try
			Next i

			If format Is Nothing Then
				Throw New InvalidMidiDataException("url is not a supported file type")
			Else
				Return format
			End If
		End Function


		''' <summary>
		''' Obtains the MIDI file format of the specified <code>File</code>.  The
		''' <code>File</code> must point to valid MIDI file data for a file type
		''' recognized by the system.
		''' <p>
		''' This operation can only succeed for files of a type which can be parsed
		''' by an installed file reader.  It may fail with an InvalidMidiDataException
		''' even for valid files if no compatible file reader is installed.  It
		''' will also fail with an InvalidMidiDataException if a compatible file reader
		''' is installed, but encounters errors while determining the file format.
		''' </summary>
		''' <param name="file"> the <code>File</code> from which file format information
		''' should be extracted </param>
		''' <returns> a <code>MidiFileFormat</code> object describing the MIDI file
		''' format </returns>
		''' <exception cref="InvalidMidiDataException"> if the <code>File</code> does not point
		'''  to valid MIDI file data recognized by the system </exception>
		''' <exception cref="IOException"> if an I/O exception occurs while accessing the file
		''' </exception>
		''' <seealso cref= #getMidiFileFormat(InputStream) </seealso>
		''' <seealso cref= #getMidiFileFormat(URL) </seealso>
		Public Shared Function getMidiFileFormat(ByVal file As java.io.File) As MidiFileFormat

			Dim ___providers As IList = midiFileReaders
			Dim format As MidiFileFormat = Nothing

			For i As Integer = 0 To ___providers.Count - 1
				Dim reader As javax.sound.midi.spi.MidiFileReader = CType(___providers(i), javax.sound.midi.spi.MidiFileReader)
				Try
					format = reader.getMidiFileFormat(file) ' throws IOException
					Exit For
				Catch e As InvalidMidiDataException
					Continue For
				End Try
			Next i

			If format Is Nothing Then
				Throw New InvalidMidiDataException("file is not a supported file type")
			Else
				Return format
			End If
		End Function


		''' <summary>
		''' Obtains a MIDI sequence from the specified input stream.  The stream must
		''' point to valid MIDI file data for a file type recognized
		''' by the system.
		''' <p>
		''' This method and/or the code it invokes may need to read some data
		''' from the stream to determine whether
		''' its data format is supported.  The implementation may therefore
		''' need to mark the stream, read enough data to determine whether it is in
		''' a supported format, and reset the stream's read pointer to its original
		''' position.  If the input stream does not permit this set of operations,
		''' this method may fail with an <code>IOException</code>.
		''' <p>
		''' This operation can only succeed for files of a type which can be parsed
		''' by an installed file reader.  It may fail with an InvalidMidiDataException
		''' even for valid files if no compatible file reader is installed.  It
		''' will also fail with an InvalidMidiDataException if a compatible file reader
		''' is installed, but encounters errors while constructing the <code>Sequence</code>
		''' object from the file data.
		''' </summary>
		''' <param name="stream"> the input stream from which the <code>Sequence</code>
		''' should be constructed </param>
		''' <returns> a <code>Sequence</code> object based on the MIDI file data
		''' contained in the input stream </returns>
		''' <exception cref="InvalidMidiDataException"> if the stream does not point to
		''' valid MIDI file data recognized by the system </exception>
		''' <exception cref="IOException"> if an I/O exception occurs while accessing the
		''' stream </exception>
		''' <seealso cref= InputStream#markSupported </seealso>
		''' <seealso cref= InputStream#mark </seealso>
		Public Shared Function getSequence(ByVal stream As java.io.InputStream) As Sequence

			Dim ___providers As IList = midiFileReaders
			Dim ___sequence As Sequence = Nothing

			For i As Integer = 0 To ___providers.Count - 1
				Dim reader As javax.sound.midi.spi.MidiFileReader = CType(___providers(i), javax.sound.midi.spi.MidiFileReader)
				Try
					___sequence = reader.getSequence(stream) ' throws IOException
					Exit For
				Catch e As InvalidMidiDataException
					Continue For
				End Try
			Next i

			If ___sequence Is Nothing Then
				Throw New InvalidMidiDataException("could not get sequence from input stream")
			Else
				Return ___sequence
			End If
		End Function


		''' <summary>
		''' Obtains a MIDI sequence from the specified URL.  The URL must
		''' point to valid MIDI file data for a file type recognized
		''' by the system.
		''' <p>
		''' This operation can only succeed for files of a type which can be parsed
		''' by an installed file reader.  It may fail with an InvalidMidiDataException
		''' even for valid files if no compatible file reader is installed.  It
		''' will also fail with an InvalidMidiDataException if a compatible file reader
		''' is installed, but encounters errors while constructing the <code>Sequence</code>
		''' object from the file data.
		''' </summary>
		''' <param name="url"> the URL from which the <code>Sequence</code> should be
		''' constructed </param>
		''' <returns> a <code>Sequence</code> object based on the MIDI file data
		''' pointed to by the URL </returns>
		''' <exception cref="InvalidMidiDataException"> if the URL does not point to valid MIDI
		''' file data recognized by the system </exception>
		''' <exception cref="IOException"> if an I/O exception occurs while accessing the URL </exception>
		Public Shared Function getSequence(ByVal url As java.net.URL) As Sequence

			Dim ___providers As IList = midiFileReaders
			Dim ___sequence As Sequence = Nothing

			For i As Integer = 0 To ___providers.Count - 1
				Dim reader As javax.sound.midi.spi.MidiFileReader = CType(___providers(i), javax.sound.midi.spi.MidiFileReader)
				Try
					___sequence = reader.getSequence(url) ' throws IOException
					Exit For
				Catch e As InvalidMidiDataException
					Continue For
				End Try
			Next i

			If ___sequence Is Nothing Then
				Throw New InvalidMidiDataException("could not get sequence from URL")
			Else
				Return ___sequence
			End If
		End Function


		''' <summary>
		''' Obtains a MIDI sequence from the specified <code>File</code>.
		''' The <code>File</code> must point to valid MIDI file data
		''' for a file type recognized by the system.
		''' <p>
		''' This operation can only succeed for files of a type which can be parsed
		''' by an installed file reader.  It may fail with an InvalidMidiDataException
		''' even for valid files if no compatible file reader is installed.  It
		''' will also fail with an InvalidMidiDataException if a compatible file reader
		''' is installed, but encounters errors while constructing the <code>Sequence</code>
		''' object from the file data.
		''' </summary>
		''' <param name="file"> the <code>File</code> from which the <code>Sequence</code>
		''' should be constructed </param>
		''' <returns> a <code>Sequence</code> object based on the MIDI file data
		''' pointed to by the File </returns>
		''' <exception cref="InvalidMidiDataException"> if the File does not point to valid MIDI
		''' file data recognized by the system </exception>
		''' <exception cref="IOException"> if an I/O exception occurs </exception>
		Public Shared Function getSequence(ByVal file As java.io.File) As Sequence

			Dim ___providers As IList = midiFileReaders
			Dim ___sequence As Sequence = Nothing

			For i As Integer = 0 To ___providers.Count - 1
				Dim reader As javax.sound.midi.spi.MidiFileReader = CType(___providers(i), javax.sound.midi.spi.MidiFileReader)
				Try
					___sequence = reader.getSequence(file) ' throws IOException
					Exit For
				Catch e As InvalidMidiDataException
					Continue For
				End Try
			Next i

			If ___sequence Is Nothing Then
				Throw New InvalidMidiDataException("could not get sequence from file")
			Else
				Return ___sequence
			End If
		End Function


		''' <summary>
		''' Obtains the set of MIDI file types for which file writing support is
		''' provided by the system. </summary>
		''' <returns> array of unique file types.  If no file types are supported,
		''' an array of length 0 is returned. </returns>
		Public Property Shared midiFileTypes As Integer()
			Get
    
				Dim ___providers As IList = midiFileWriters
				Dim allTypes As java.util.Set = New HashSet
    
				' gather from all the providers
    
				For i As Integer = 0 To ___providers.Count - 1
					Dim writer As javax.sound.midi.spi.MidiFileWriter = CType(___providers(i), javax.sound.midi.spi.MidiFileWriter)
					Dim types As Integer() = writer.midiFileTypes
					For j As Integer = 0 To types.Length - 1
						allTypes.add(New Integer?(types(j)))
					Next j
				Next i
				Dim resultTypes As Integer() = New Integer(allTypes.size() - 1){}
				Dim index As Integer = 0
				Dim [iterator] As IEnumerator = allTypes.GetEnumerator()
				Do While [iterator].hasNext()
					Dim [integer] As Integer? = CInt(Fix([iterator].next()))
					resultTypes(index) = [integer]
					index += 1
				Loop
				Return resultTypes
			End Get
		End Property


		''' <summary>
		''' Indicates whether file writing support for the specified MIDI file type
		''' is provided by the system. </summary>
		''' <param name="fileType"> the file type for which write capabilities are queried </param>
		''' <returns> <code>true</code> if the file type is supported,
		''' otherwise <code>false</code> </returns>
		Public Shared Function isFileTypeSupported(ByVal fileType As Integer) As Boolean

			Dim ___providers As IList = midiFileWriters

			For i As Integer = 0 To ___providers.Count - 1
				Dim writer As javax.sound.midi.spi.MidiFileWriter = CType(___providers(i), javax.sound.midi.spi.MidiFileWriter)
				If writer.isFileTypeSupported(fileType) Then Return True
			Next i
			Return False
		End Function


		''' <summary>
		''' Obtains the set of MIDI file types that the system can write from the
		''' sequence specified. </summary>
		''' <param name="sequence"> the sequence for which MIDI file type support
		''' is queried </param>
		''' <returns> the set of unique supported file types.  If no file types are supported,
		''' returns an array of length 0. </returns>
		Public Shared Function getMidiFileTypes(ByVal ___sequence As Sequence) As Integer()

			Dim ___providers As IList = midiFileWriters
			Dim allTypes As java.util.Set = New HashSet

			' gather from all the providers

			For i As Integer = 0 To ___providers.Count - 1
				Dim writer As javax.sound.midi.spi.MidiFileWriter = CType(___providers(i), javax.sound.midi.spi.MidiFileWriter)
				Dim types As Integer() = writer.getMidiFileTypes(___sequence)
				For j As Integer = 0 To types.Length - 1
					allTypes.add(New Integer?(types(j)))
				Next j
			Next i
			Dim resultTypes As Integer() = New Integer(allTypes.size() - 1){}
			Dim index As Integer = 0
			Dim [iterator] As IEnumerator = allTypes.GetEnumerator()
			Do While [iterator].hasNext()
				Dim [integer] As Integer? = CInt(Fix([iterator].next()))
				resultTypes(index) = [integer]
				index += 1
			Loop
			Return resultTypes
		End Function


		''' <summary>
		''' Indicates whether a MIDI file of the file type specified can be written
		''' from the sequence indicated. </summary>
		''' <param name="fileType"> the file type for which write capabilities
		''' are queried </param>
		''' <param name="sequence"> the sequence for which file writing support is queried </param>
		''' <returns> <code>true</code> if the file type is supported for this
		''' sequence, otherwise <code>false</code> </returns>
		Public Shared Function isFileTypeSupported(ByVal fileType As Integer, ByVal ___sequence As Sequence) As Boolean

			Dim ___providers As IList = midiFileWriters

			For i As Integer = 0 To ___providers.Count - 1
				Dim writer As javax.sound.midi.spi.MidiFileWriter = CType(___providers(i), javax.sound.midi.spi.MidiFileWriter)
				If writer.isFileTypeSupported(fileType,___sequence) Then Return True
			Next i
			Return False
		End Function


		''' <summary>
		''' Writes a stream of bytes representing a file of the MIDI file type
		''' indicated to the output stream provided. </summary>
		''' <param name="in"> sequence containing MIDI data to be written to the file </param>
		''' <param name="fileType"> the file type of the file to be written to the output stream </param>
		''' <param name="out"> stream to which the file data should be written </param>
		''' <returns> the number of bytes written to the output stream </returns>
		''' <exception cref="IOException"> if an I/O exception occurs </exception>
		''' <exception cref="IllegalArgumentException"> if the file format is not supported by
		''' the system </exception>
		''' <seealso cref= #isFileTypeSupported(int, Sequence) </seealso>
		''' <seealso cref=     #getMidiFileTypes(Sequence) </seealso>
		Public Shared Function write(ByVal [in] As Sequence, ByVal fileType As Integer, ByVal out As java.io.OutputStream) As Integer

			Dim ___providers As IList = midiFileWriters
			'$$fb 2002-04-17: Fix for 4635287: Standard MidiFileWriter cannot write empty Sequences
			Dim bytesWritten As Integer = -2

			For i As Integer = 0 To ___providers.Count - 1
				Dim writer As javax.sound.midi.spi.MidiFileWriter = CType(___providers(i), javax.sound.midi.spi.MidiFileWriter)
				If writer.isFileTypeSupported(fileType, [in]) Then

					bytesWritten = writer.write([in], fileType, out)
					Exit For
				End If
			Next i
			If bytesWritten = -2 Then Throw New System.ArgumentException("MIDI file type is not supported")
			Return bytesWritten
		End Function


		''' <summary>
		''' Writes a stream of bytes representing a file of the MIDI file type
		''' indicated to the external file provided. </summary>
		''' <param name="in"> sequence containing MIDI data to be written to the file </param>
		''' <param name="type"> the file type of the file to be written to the output stream </param>
		''' <param name="out"> external file to which the file data should be written </param>
		''' <returns> the number of bytes written to the file </returns>
		''' <exception cref="IOException"> if an I/O exception occurs </exception>
		''' <exception cref="IllegalArgumentException"> if the file type is not supported by
		''' the system </exception>
		''' <seealso cref= #isFileTypeSupported(int, Sequence) </seealso>
		''' <seealso cref=     #getMidiFileTypes(Sequence) </seealso>
		Public Shared Function write(ByVal [in] As Sequence, ByVal type As Integer, ByVal out As java.io.File) As Integer

			Dim ___providers As IList = midiFileWriters
			'$$fb 2002-04-17: Fix for 4635287: Standard MidiFileWriter cannot write empty Sequences
			Dim bytesWritten As Integer = -2

			For i As Integer = 0 To ___providers.Count - 1
				Dim writer As javax.sound.midi.spi.MidiFileWriter = CType(___providers(i), javax.sound.midi.spi.MidiFileWriter)
				If writer.isFileTypeSupported(type, [in]) Then

					bytesWritten = writer.write([in], type, out)
					Exit For
				End If
			Next i
			If bytesWritten = -2 Then Throw New System.ArgumentException("MIDI file type is not supported")
			Return bytesWritten
		End Function



		' HELPER METHODS

		Private Property Shared midiDeviceProviders As IList
			Get
				Return getProviders(GetType(javax.sound.midi.spi.MidiDeviceProvider))
			End Get
		End Property


		Private Property Shared soundbankReaders As IList
			Get
				Return getProviders(GetType(javax.sound.midi.spi.SoundbankReader))
			End Get
		End Property


		Private Property Shared midiFileWriters As IList
			Get
				Return getProviders(GetType(javax.sound.midi.spi.MidiFileWriter))
			End Get
		End Property


		Private Property Shared midiFileReaders As IList
			Get
				Return getProviders(GetType(javax.sound.midi.spi.MidiFileReader))
			End Get
		End Property


		''' <summary>
		''' Attempts to locate and return a default MidiDevice of the specified
		''' type.
		''' 
		''' This method wraps <seealso cref="#getDefaultDevice"/>. It catches the
		''' <code>IllegalArgumentException</code> thrown by
		''' <code>getDefaultDevice</code> and instead throws a
		''' <code>MidiUnavailableException</code>, with the catched
		''' exception chained.
		''' </summary>
		''' <param name="deviceClass"> The requested device type, one of Synthesizer.class,
		''' Sequencer.class, Receiver.class or Transmitter.class. </param>
		''' <exception cref="MidiUnavalableException"> on failure. </exception>
		Private Shared Function getDefaultDeviceWrapper(ByVal deviceClass As Type) As MidiDevice
			Try
				Return getDefaultDevice(deviceClass)
			Catch iae As System.ArgumentException
				Dim mae As New MidiUnavailableException
				mae.initCause(iae)
				Throw mae
			End Try
		End Function


		''' <summary>
		''' Attempts to locate and return a default MidiDevice of the specified
		''' type.
		''' </summary>
		''' <param name="deviceClass"> The requested device type, one of Synthesizer.class,
		''' Sequencer.class, Receiver.class or Transmitter.class. </param>
		''' <exception cref="IllegalArgumentException"> on failure. </exception>
		Private Shared Function getDefaultDevice(ByVal deviceClass As Type) As MidiDevice
			Dim ___providers As IList = midiDeviceProviders
			Dim providerClassName As String = com.sun.media.sound.JDK13Services.getDefaultProviderClassName(deviceClass)
			Dim instanceName As String = com.sun.media.sound.JDK13Services.getDefaultInstanceName(deviceClass)
			Dim device As MidiDevice

			If providerClassName IsNot Nothing Then
				Dim defaultProvider As javax.sound.midi.spi.MidiDeviceProvider = getNamedProvider(providerClassName, ___providers)
				If defaultProvider IsNot Nothing Then
					If instanceName IsNot Nothing Then
						device = getNamedDevice(instanceName, defaultProvider, deviceClass)
						If device IsNot Nothing Then Return device
					End If
					device = getFirstDevice(defaultProvider, deviceClass)
					If device IsNot Nothing Then Return device
				End If
			End If

	'         Provider class not specified or cannot be found, or
	'           provider class specified, and no appropriate device available or
	'           provider class and instance specified and instance cannot be found or is not appropriate 
			If instanceName IsNot Nothing Then
				device = getNamedDevice(instanceName, ___providers, deviceClass)
				If device IsNot Nothing Then Return device
			End If

	'         No default are specified, or if something is specified, everything
	'           failed. 
			device = getFirstDevice(___providers, deviceClass)
			If device IsNot Nothing Then Return device
			Throw New System.ArgumentException("Requested device not installed")
		End Function



		''' <summary>
		''' Return a MidiDeviceProcider of a given class from the list of
		'''    MidiDeviceProviders.
		''' </summary>
		'''    <param name="providerClassName"> The class name of the provider to be returned. </param>
		'''    <param name="provider"> The list of MidiDeviceProviders that is searched. </param>
		'''    <returns> A MidiDeviceProvider of the requested class, or null if none
		'''    is found. </returns>
		Private Shared Function getNamedProvider(ByVal providerClassName As String, ByVal providers As IList) As javax.sound.midi.spi.MidiDeviceProvider
			For i As Integer = 0 To providers.Count - 1
				Dim provider As javax.sound.midi.spi.MidiDeviceProvider = CType(providers(i), javax.sound.midi.spi.MidiDeviceProvider)
				If provider.GetType().name.Equals(providerClassName) Then Return provider
			Next i
			Return Nothing
		End Function


		''' <summary>
		''' Return a MidiDevice with a given name from a given MidiDeviceProvider. </summary>
		'''    <param name="deviceName"> The name of the MidiDevice to be returned. </param>
		'''    <param name="provider"> The MidiDeviceProvider to check for MidiDevices. </param>
		'''    <param name="deviceClass"> The requested device type, one of Synthesizer.class,
		'''    Sequencer.class, Receiver.class or Transmitter.class.
		''' </param>
		'''    <returns> A MidiDevice matching the requirements, or null if none is found. </returns>
		Private Shared Function getNamedDevice(ByVal deviceName As String, ByVal provider As javax.sound.midi.spi.MidiDeviceProvider, ByVal deviceClass As Type) As MidiDevice
			Dim device As MidiDevice
			' try to get MIDI port
			device = getNamedDevice(deviceName, provider, deviceClass, False, False)
			If device IsNot Nothing Then Return device

			If deviceClass Is GetType(Receiver) Then
				' try to get Synthesizer
				device = getNamedDevice(deviceName, provider, deviceClass, True, False)
				If device IsNot Nothing Then Return device
			End If

			Return Nothing
		End Function


		''' <summary>
		''' Return a MidiDevice with a given name from a given MidiDeviceProvider. </summary>
		'''  <param name="deviceName"> The name of the MidiDevice to be returned. </param>
		'''  <param name="provider"> The MidiDeviceProvider to check for MidiDevices. </param>
		'''  <param name="deviceClass"> The requested device type, one of Synthesizer.class,
		'''  Sequencer.class, Receiver.class or Transmitter.class.
		''' </param>
		'''  <returns> A MidiDevice matching the requirements, or null if none is found. </returns>
		Private Shared Function getNamedDevice(ByVal deviceName As String, ByVal provider As javax.sound.midi.spi.MidiDeviceProvider, ByVal deviceClass As Type, ByVal allowSynthesizer As Boolean, ByVal allowSequencer As Boolean) As MidiDevice
			Dim infos As MidiDevice.Info() = provider.deviceInfo
			For i As Integer = 0 To infos.Length - 1
				If infos(i).name.Equals(deviceName) Then
					Dim device As MidiDevice = provider.getDevice(infos(i))
					If isAppropriateDevice(device, deviceClass, allowSynthesizer, allowSequencer) Then Return device
				End If
			Next i
			Return Nothing
		End Function


		''' <summary>
		''' Return a MidiDevice with a given name from a list of
		'''    MidiDeviceProviders. </summary>
		'''    <param name="deviceName"> The name of the MidiDevice to be returned. </param>
		'''    <param name="providers"> The List of MidiDeviceProviders to check for
		'''    MidiDevices. </param>
		'''    <param name="deviceClass"> The requested device type, one of Synthesizer.class,
		'''    Sequencer.class, Receiver.class or Transmitter.class. </param>
		'''    <returns> A Mixer matching the requirements, or null if none is found. </returns>
		Private Shared Function getNamedDevice(ByVal deviceName As String, ByVal providers As IList, ByVal deviceClass As Type) As MidiDevice
			Dim device As MidiDevice
			' try to get MIDI port
			device = getNamedDevice(deviceName, providers, deviceClass, False, False)
			If device IsNot Nothing Then Return device

			If deviceClass Is GetType(Receiver) Then
				' try to get Synthesizer
				device = getNamedDevice(deviceName, providers, deviceClass, True, False)
				If device IsNot Nothing Then Return device
			End If

			Return Nothing
		End Function


		''' <summary>
		''' Return a MidiDevice with a given name from a list of
		'''    MidiDeviceProviders. </summary>
		'''    <param name="deviceName"> The name of the MidiDevice to be returned. </param>
		'''    <param name="providers"> The List of MidiDeviceProviders to check for
		'''    MidiDevices. </param>
		'''    <param name="deviceClass"> The requested device type, one of Synthesizer.class,
		'''    Sequencer.class, Receiver.class or Transmitter.class. </param>
		'''    <returns> A Mixer matching the requirements, or null if none is found. </returns>
		Private Shared Function getNamedDevice(ByVal deviceName As String, ByVal providers As IList, ByVal deviceClass As Type, ByVal allowSynthesizer As Boolean, ByVal allowSequencer As Boolean) As MidiDevice
			For i As Integer = 0 To providers.Count - 1
				Dim provider As javax.sound.midi.spi.MidiDeviceProvider = CType(providers(i), javax.sound.midi.spi.MidiDeviceProvider)
				Dim device As MidiDevice = getNamedDevice(deviceName, provider, deviceClass, allowSynthesizer, allowSequencer)
				If device IsNot Nothing Then Return device
			Next i
			Return Nothing
		End Function


		''' <summary>
		''' From a given MidiDeviceProvider, return the first appropriate device. </summary>
		'''    <param name="provider"> The MidiDeviceProvider to check for MidiDevices. </param>
		'''    <param name="deviceClass"> The requested device type, one of Synthesizer.class,
		'''    Sequencer.class, Receiver.class or Transmitter.class. </param>
		'''    <returns> A MidiDevice is considered appropriate, or null if no
		'''    appropriate device is found. </returns>
		Private Shared Function getFirstDevice(ByVal provider As javax.sound.midi.spi.MidiDeviceProvider, ByVal deviceClass As Type) As MidiDevice
			Dim device As MidiDevice
			' try to get MIDI port
			device = getFirstDevice(provider, deviceClass, False, False)
			If device IsNot Nothing Then Return device

			If deviceClass Is GetType(Receiver) Then
				' try to get Synthesizer
				device = getFirstDevice(provider, deviceClass, True, False)
				If device IsNot Nothing Then Return device
			End If

			Return Nothing
		End Function


		''' <summary>
		''' From a given MidiDeviceProvider, return the first appropriate device. </summary>
		'''    <param name="provider"> The MidiDeviceProvider to check for MidiDevices. </param>
		'''    <param name="deviceClass"> The requested device type, one of Synthesizer.class,
		'''    Sequencer.class, Receiver.class or Transmitter.class. </param>
		'''    <returns> A MidiDevice is considered appropriate, or null if no
		'''    appropriate device is found. </returns>
		Private Shared Function getFirstDevice(ByVal provider As javax.sound.midi.spi.MidiDeviceProvider, ByVal deviceClass As Type, ByVal allowSynthesizer As Boolean, ByVal allowSequencer As Boolean) As MidiDevice
			Dim infos As MidiDevice.Info() = provider.deviceInfo
			For j As Integer = 0 To infos.Length - 1
				Dim device As MidiDevice = provider.getDevice(infos(j))
				If isAppropriateDevice(device, deviceClass, allowSynthesizer, allowSequencer) Then Return device
			Next j
			Return Nothing
		End Function


		''' <summary>
		''' From a List of MidiDeviceProviders, return the first appropriate
		'''    MidiDevice. </summary>
		'''    <param name="providers"> The List of MidiDeviceProviders to search. </param>
		'''    <param name="deviceClass"> The requested device type, one of Synthesizer.class,
		'''    Sequencer.class, Receiver.class or Transmitter.class. </param>
		'''    <returns> A MidiDevice that is considered appropriate, or null
		'''    if none is found. </returns>
		Private Shared Function getFirstDevice(ByVal providers As IList, ByVal deviceClass As Type) As MidiDevice
			Dim device As MidiDevice
			' try to get MIDI port
			device = getFirstDevice(providers, deviceClass, False, False)
			If device IsNot Nothing Then Return device

			If deviceClass Is GetType(Receiver) Then
				' try to get Synthesizer
				device = getFirstDevice(providers, deviceClass, True, False)
				If device IsNot Nothing Then Return device
			End If

			Return Nothing
		End Function


		''' <summary>
		''' From a List of MidiDeviceProviders, return the first appropriate
		'''    MidiDevice. </summary>
		'''    <param name="providers"> The List of MidiDeviceProviders to search. </param>
		'''    <param name="deviceClass"> The requested device type, one of Synthesizer.class,
		'''    Sequencer.class, Receiver.class or Transmitter.class. </param>
		'''    <returns> A MidiDevice that is considered appropriate, or null
		'''    if none is found. </returns>
		Private Shared Function getFirstDevice(ByVal providers As IList, ByVal deviceClass As Type, ByVal allowSynthesizer As Boolean, ByVal allowSequencer As Boolean) As MidiDevice
			For i As Integer = 0 To providers.Count - 1
				Dim provider As javax.sound.midi.spi.MidiDeviceProvider = CType(providers(i), javax.sound.midi.spi.MidiDeviceProvider)
				Dim device As MidiDevice = getFirstDevice(provider, deviceClass, allowSynthesizer, allowSequencer)
				If device IsNot Nothing Then Return device
			Next i
			Return Nothing
		End Function


		''' <summary>
		''' Checks if a MidiDevice is appropriate.
		'''    If deviceClass is Synthesizer or Sequencer, a device implementing
		'''    the respective interface is considered appropriate. If deviceClass
		'''    is Receiver or Transmitter, a device is considered appropriate if
		'''    it implements neither Synthesizer nor Transmitter, and if it can
		'''    provide at least one Receiver or Transmitter, respectively.
		''' </summary>
		'''    <param name="device"> the MidiDevice to test </param>
		'''    <param name="allowSynthesizer"> if true, Synthesizers are considered
		'''    appropriate. Otherwise only pure MidiDevices are considered
		'''    appropriate (unless allowSequencer is true). This flag only has an
		'''    effect for deviceClass Receiver and Transmitter. For other device
		'''    classes (Sequencer and Synthesizer), this flag has no effect. </param>
		'''    <param name="allowSequencer"> if true, Sequencers are considered
		'''    appropriate. Otherwise only pure MidiDevices are considered
		'''    appropriate (unless allowSynthesizer is true). This flag only has an
		'''    effect for deviceClass Receiver and Transmitter. For other device
		'''    classes (Sequencer and Synthesizer), this flag has no effect. </param>
		'''    <returns> true if the device is considered appropriate according to the
		'''    rules given above, false otherwise. </returns>
		Private Shared Function isAppropriateDevice(ByVal device As MidiDevice, ByVal deviceClass As Type, ByVal allowSynthesizer As Boolean, ByVal allowSequencer As Boolean) As Boolean
			If deviceClass.IsInstanceOfType(device) Then
				' This clause is for deviceClass being either Synthesizer
				' or Sequencer.
				Return True
			Else
				' Now the case that deviceClass is Transmitter or
				' Receiver. If neither allowSynthesizer nor allowSequencer is
				' true, we require device instances to be
				' neither Synthesizer nor Sequencer, since we only want
				' devices representing MIDI ports.
				' Otherwise, the respective type is accepted, too
				If (Not(TypeOf device Is Sequencer) AndAlso Not(TypeOf device Is Synthesizer)) OrElse ((TypeOf device Is Sequencer) AndAlso allowSequencer) OrElse ((TypeOf device Is Synthesizer) AndAlso allowSynthesizer) Then
					' And of cource, the device has to be able to provide
					' Receivers or Transmitters.
					If (deviceClass Is GetType(Receiver) AndAlso device.maxReceivers <> 0) OrElse (deviceClass Is GetType(Transmitter) AndAlso device.maxTransmitters <> 0) Then Return True
				End If
			End If
			Return False
		End Function


		''' <summary>
		''' Obtains the set of services currently installed on the system
		''' using sun.misc.Service, the SPI mechanism in 1.3. </summary>
		''' <returns> a List of instances of providers for the requested service.
		''' If no providers are available, a List of length 0 will be returned. </returns>
		Private Shared Function getProviders(ByVal providerClass As Type) As IList
			Return com.sun.media.sound.JDK13Services.getProviders(providerClass)
		End Function
	End Class

End Namespace