Imports System.Collections.Generic

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
	 ''' <code>MidiDevice</code> is the base interface for all MIDI devices.
	 ''' Common devices include synthesizers, sequencers, MIDI input ports, and MIDI
	 ''' output ports.
	 ''' 
	 ''' <p>A <code>MidiDevice</code> can be a transmitter or a receiver of
	 ''' MIDI events, or both. Therefore, it can provide <seealso cref="Transmitter"/>
	 ''' or <seealso cref="Receiver"/> instances (or both). Typically, MIDI IN ports
	 ''' provide transmitters, MIDI OUT ports and synthesizers provide
	 ''' receivers. A Sequencer typically provides transmitters for playback
	 ''' and receivers for recording.
	 ''' 
	 ''' <p>A <code>MidiDevice</code> can be opened and closed explicitly as
	 ''' well as implicitly. Explicit opening is accomplished by calling
	 ''' <seealso cref="#open"/>, explicit closing is done by calling {@link
	 ''' #close} on the <code>MidiDevice</code> instance.
	 ''' If an application opens a <code>MidiDevice</code>
	 ''' explicitly, it has to close it explicitly to free system resources
	 ''' and enable the application to exit cleanly. Implicit opening is
	 ''' done by calling {@link javax.sound.midi.MidiSystem#getReceiver
	 ''' MidiSystem.getReceiver} and {@link
	 ''' javax.sound.midi.MidiSystem#getTransmitter
	 ''' MidiSystem.getTransmitter}. The <code>MidiDevice</code> used by
	 ''' <code>MidiSystem.getReceiver</code> and
	 ''' <code>MidiSystem.getTransmitter</code> is implementation-dependant
	 ''' unless the properties <code>javax.sound.midi.Receiver</code>
	 ''' and <code>javax.sound.midi.Transmitter</code> are used (see the
	 ''' description of properties to select default providers in
	 ''' <seealso cref="javax.sound.midi.MidiSystem"/>). A <code>MidiDevice</code>
	 ''' that was opened implicitly, is closed implicitly by closing the
	 ''' <code>Receiver</code> or <code>Transmitter</code> that resulted in
	 ''' opening it. If more than one implicitly opening
	 ''' <code>Receiver</code> or <code>Transmitter</code> were obtained by
	 ''' the application, the device is closed after the last
	 ''' <code>Receiver</code> or <code>Transmitter</code> has been
	 ''' closed. On the other hand, calling <code>getReceiver</code> or
	 ''' <code>getTransmitter</code> on the device instance directly does
	 ''' not open the device implicitly. Closing these
	 ''' <code>Transmitter</code>s and <code>Receiver</code>s does not close
	 ''' the device implicitly. To use a device with <code>Receiver</code>s
	 ''' or <code>Transmitter</code>s obtained this way, the device has to
	 ''' be opened and closed explicitly.
	 ''' 
	 ''' <p>If implicit and explicit opening and closing are mixed on the
	 ''' same <code>MidiDevice</code> instance, the following rules apply:
	 ''' 
	 ''' <ul>
	 ''' <li>After an explicit open (either before or after implicit
	 ''' opens), the device will not be closed by implicit closing. The only
	 ''' way to close an explicitly opened device is an explicit close.</li>
	 ''' 
	 ''' <li>An explicit close always closes the device, even if it also has
	 ''' been opened implicitly. A subsequent implicit close has no further
	 ''' effect.</li>
	 ''' </ul>
	 ''' 
	 ''' To detect if a MidiDevice represents a hardware MIDI port, the
	 ''' following programming technique can be used:
	 ''' 
	 ''' <pre>{@code
	 ''' MidiDevice device = ...;
	 ''' if ( ! (device instanceof Sequencer) && ! (device instanceof Synthesizer)) {
	 '''   // we're now sure that device represents a MIDI port
	 '''   // ...
	 ''' }
	 ''' }</pre>
	 ''' 
	 ''' <p>
	 ''' A <code>MidiDevice</code> includes a <code><seealso cref="MidiDevice.Info"/></code> object
	 ''' to provide manufacturer information and so on.
	 ''' </summary>
	 ''' <seealso cref= Synthesizer </seealso>
	 ''' <seealso cref= Sequencer </seealso>
	 ''' <seealso cref= Receiver </seealso>
	 ''' <seealso cref= Transmitter
	 ''' 
	 ''' @author Kara Kytle
	 ''' @author Florian Bomers </seealso>

	Public Interface MidiDevice
		Inherits AutoCloseable


		''' <summary>
		''' Obtains information about the device, including its Java class and
		''' <code>Strings</code> containing its name, vendor, and description.
		''' </summary>
		''' <returns> device info </returns>
		ReadOnly Property deviceInfo As Info


		''' <summary>
		''' Opens the device, indicating that it should now acquire any
		''' system resources it requires and become operational.
		''' 
		''' <p>An application opening a device explicitly with this call
		''' has to close the device by calling <seealso cref="#close"/>. This is
		''' necessary to release system resources and allow applications to
		''' exit cleanly.
		''' 
		''' <p>
		''' Note that some devices, once closed, cannot be reopened.  Attempts
		''' to reopen such a device will always result in a MidiUnavailableException.
		''' </summary>
		''' <exception cref="MidiUnavailableException"> thrown if the device cannot be
		''' opened due to resource restrictions. </exception>
		''' <exception cref="SecurityException"> thrown if the device cannot be
		''' opened due to security restrictions.
		''' </exception>
		''' <seealso cref= #close </seealso>
		''' <seealso cref= #isOpen </seealso>
		Sub open()


		''' <summary>
		''' Closes the device, indicating that the device should now release
		''' any system resources it is using.
		''' 
		''' <p>All <code>Receiver</code> and <code>Transmitter</code> instances
		''' open from this device are closed. This includes instances retrieved
		''' via <code>MidiSystem</code>.
		''' </summary>
		''' <seealso cref= #open </seealso>
		''' <seealso cref= #isOpen </seealso>
		Sub close()


		''' <summary>
		''' Reports whether the device is open.
		''' </summary>
		''' <returns> <code>true</code> if the device is open, otherwise
		''' <code>false</code> </returns>
		''' <seealso cref= #open </seealso>
		''' <seealso cref= #close </seealso>
		ReadOnly Property open As Boolean


		''' <summary>
		''' Obtains the current time-stamp of the device, in microseconds.
		''' If a device supports time-stamps, it should start counting at
		''' 0 when the device is opened and continue incrementing its
		''' time-stamp in microseconds until the device is closed.
		''' If it does not support time-stamps, it should always return
		''' -1. </summary>
		''' <returns> the current time-stamp of the device in microseconds,
		''' or -1 if time-stamping is not supported by the device. </returns>
		ReadOnly Property microsecondPosition As Long


		''' <summary>
		''' Obtains the maximum number of MIDI IN connections available on this
		''' MIDI device for receiving MIDI data. </summary>
		''' <returns> maximum number of MIDI IN connections,
		''' or -1 if an unlimited number of connections is available. </returns>
		ReadOnly Property maxReceivers As Integer


		''' <summary>
		''' Obtains the maximum number of MIDI OUT connections available on this
		''' MIDI device for transmitting MIDI data. </summary>
		''' <returns> maximum number of MIDI OUT connections,
		''' or -1 if an unlimited number of connections is available. </returns>
		ReadOnly Property maxTransmitters As Integer


		''' <summary>
		''' Obtains a MIDI IN receiver through which the MIDI device may receive
		''' MIDI data.  The returned receiver must be closed when the application
		''' has finished using it.
		''' 
		''' <p>Usually the returned receiver implements
		''' the {@code MidiDeviceReceiver} interface.
		''' 
		''' <p>Obtaining a <code>Receiver</code> with this method does not
		''' open the device. To be able to use the device, it has to be
		''' opened explicitly by calling <seealso cref="#open"/>. Also, closing the
		''' <code>Receiver</code> does not close the device. It has to be
		''' closed explicitly by calling <seealso cref="#close"/>.
		''' </summary>
		''' <returns> a receiver for the device. </returns>
		''' <exception cref="MidiUnavailableException"> thrown if a receiver is not available
		''' due to resource restrictions </exception>
		''' <seealso cref= Receiver#close() </seealso>
		ReadOnly Property receiver As Receiver


		''' <summary>
		''' Returns all currently active, non-closed receivers
		''' connected with this MidiDevice.
		''' A receiver can be removed
		''' from the device by closing it.
		''' 
		''' <p>Usually the returned receivers implement
		''' the {@code MidiDeviceReceiver} interface.
		''' </summary>
		''' <returns> an unmodifiable list of the open receivers
		''' @since 1.5 </returns>
		ReadOnly Property receivers As IList(Of Receiver)


		''' <summary>
		''' Obtains a MIDI OUT connection from which the MIDI device will transmit
		''' MIDI data  The returned transmitter must be closed when the application
		''' has finished using it.
		''' 
		''' <p>Usually the returned transmitter implements
		''' the {@code MidiDeviceTransmitter} interface.
		''' 
		''' <p>Obtaining a <code>Transmitter</code> with this method does not
		''' open the device. To be able to use the device, it has to be
		''' opened explicitly by calling <seealso cref="#open"/>. Also, closing the
		''' <code>Transmitter</code> does not close the device. It has to be
		''' closed explicitly by calling <seealso cref="#close"/>.
		''' </summary>
		''' <returns> a MIDI OUT transmitter for the device. </returns>
		''' <exception cref="MidiUnavailableException"> thrown if a transmitter is not available
		''' due to resource restrictions </exception>
		''' <seealso cref= Transmitter#close() </seealso>
		ReadOnly Property transmitter As Transmitter


		''' <summary>
		''' Returns all currently active, non-closed transmitters
		''' connected with this MidiDevice.
		''' A transmitter can be removed
		''' from the device by closing it.
		''' 
		''' <p>Usually the returned transmitters implement
		''' the {@code MidiDeviceTransmitter} interface.
		''' </summary>
		''' <returns> an unmodifiable list of the open transmitters
		''' @since 1.5 </returns>
		ReadOnly Property transmitters As IList(Of Transmitter)



		''' <summary>
		''' A <code>MidiDevice.Info</code> object contains assorted
		''' data about a <code><seealso cref="MidiDevice"/></code>, including its
		''' name, the company who created it, and descriptive text.
		''' </summary>
		''' <seealso cref= MidiDevice#getDeviceInfo </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interface inner types are not converted:
'		public static class Info
	'	{
	'
	'		''' <summary>
	'		''' The device's name.
	'		''' </summary>
	'		private String name;
	'
	'		''' <summary>
	'		''' The name of the company who provides the device.
	'		''' </summary>
	'		private String vendor;
	'
	'		''' <summary>
	'		''' A description of the device.
	'		''' </summary>
	'		private String description;
	'
	'		''' <summary>
	'		''' Device version.
	'		''' </summary>
	'		private String version;
	'
	'
	'		''' <summary>
	'		''' Constructs a device info object.
	'		''' </summary>
	'		''' <param name="name"> the name of the device </param>
	'		''' <param name="vendor"> the name of the company who provides the device </param>
	'		''' <param name="description"> a description of the device </param>
	'		''' <param name="version"> version information for the device </param>
	'		protected Info(String name, String vendor, String description, String version)
	'		{
	'
	'			Me.name = name;
	'			Me.vendor = vendor;
	'			Me.description = description;
	'			Me.version = version;
	'		}
	'
	'
	'		''' <summary>
	'		''' Reports whether two objects are equal.
	'		''' Returns <code>true</code> if the objects are identical. </summary>
	'		''' <param name="obj"> the reference object with which to compare this
	'		''' object </param>
	'		''' <returns> <code>true</code> if this object is the same as the
	'		''' <code>obj</code> argument; <code>false</code> otherwise </returns>
	'		public final boolean equals(Object obj)
	'		{
	'			Return MyBase.equals(obj);
	'		}
	'
	'
	'		''' <summary>
	'		''' Finalizes the hashcode method.
	'		''' </summary>
	'		public final int hashCode()
	'		{
	'			Return MyBase.hashCode();
	'		}
	'
	'
	'		''' <summary>
	'		''' Obtains the name of the device.
	'		''' </summary>
	'		''' <returns> a string containing the device's name </returns>
	'		public final String getName()
	'		{
	'			Return name;
	'		}
	'
	'
	'		''' <summary>
	'		''' Obtains the name of the company who supplies the device. </summary>
	'		''' <returns> device the vendor's name </returns>
	'		public final String getVendor()
	'		{
	'			Return vendor;
	'		}
	'
	'
	'		''' <summary>
	'		''' Obtains the description of the device. </summary>
	'		''' <returns> a description of the device </returns>
	'		public final String getDescription()
	'		{
	'			Return description;
	'		}
	'
	'
	'		''' <summary>
	'		''' Obtains the version of the device. </summary>
	'		''' <returns> textual version information for the device. </returns>
	'		public final String getVersion()
	'		{
	'			Return version;
	'		}
	'
	'
	'		''' <summary>
	'		''' Provides a string representation of the device information.
	'		''' </summary>
	'		''' <returns> a description of the info object </returns>
	'		public final String toString()
	'		{
	'			Return name;
	'		}
	'	} ' class Info


	End Interface

End Namespace