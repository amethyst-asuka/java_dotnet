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
	''' A {@code MidiDeviceProvider} is a factory or provider for a particular type
	''' of MIDI device. This mechanism allows the implementation to determine how
	''' resources are managed in the creation and management of a device.
	''' 
	''' @author Kara Kytle
	''' </summary>
	Public MustInherit Class MidiDeviceProvider

		''' <summary>
		''' Indicates whether the device provider supports the device represented by
		''' the specified device info object.
		''' </summary>
		''' <param name="info"> an info object that describes the device for which support
		'''         is queried </param>
		''' <returns> {@code true} if the specified device is supported, otherwise
		'''         {@code false} </returns>
		Public Overridable Function isDeviceSupported(ByVal info As javax.sound.midi.MidiDevice.Info) As Boolean

			Dim infos As javax.sound.midi.MidiDevice.Info() = deviceInfo

			For i As Integer = 0 To infos.Length - 1
				If info.Equals(infos(i)) Then Return True
			Next i
			Return False
		End Function

		''' <summary>
		''' Obtains the set of info objects representing the device or devices
		''' provided by this {@code MidiDeviceProvider}.
		''' </summary>
		''' <returns> set of device info objects </returns>
		Public MustOverride ReadOnly Property deviceInfo As javax.sound.midi.MidiDevice.Info()

		''' <summary>
		''' Obtains an instance of the device represented by the info object.
		''' </summary>
		''' <param name="info"> an info object that describes the desired device </param>
		''' <returns> device instance </returns>
		''' <exception cref="IllegalArgumentException"> if the info object specified does not
		'''         match the info object for a device supported by this
		'''         {@code MidiDeviceProvider} </exception>
		Public MustOverride Function getDevice(ByVal info As javax.sound.midi.MidiDevice.Info) As javax.sound.midi.MidiDevice
	End Class

End Namespace