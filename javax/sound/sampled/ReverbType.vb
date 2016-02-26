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

Namespace javax.sound.sampled


	''' <summary>
	''' The <code>ReverbType</code> class provides methods for
	''' accessing various reverberation settings to be applied to
	''' an audio signal.
	''' <p>
	''' Reverberation simulates the reflection of sound off of
	''' the walls, ceiling, and floor of a room.  Depending on
	''' the size of the room, and how absorbent or reflective the materials in the
	''' room's surfaces are, the sound might bounce around for a
	''' long time before dying away.
	''' <p>
	''' The reverberation parameters provided by <code>ReverbType</code> consist
	''' of the delay time and intensity of early reflections, the delay time and
	''' intensity of late reflections, and an overall decay time.
	''' Early reflections are the initial individual low-order reflections of the
	''' direct signal off the surfaces in the room.
	''' The late Reflections are the dense, high-order reflections that characterize
	''' the room's reverberation.
	''' The delay times for the start of these two reflection types give the listener
	''' a sense of the overall size and complexity of the room's shape and contents.
	''' The larger the room, the longer the reflection delay times.
	''' The early and late reflections' intensities define the gain (in decibels) of the reflected
	''' signals as compared to the direct signal.  These intensities give the
	''' listener an impression of the absorptive nature of the surfaces and objects
	''' in the room.
	''' The decay time defines how long the reverberation takes to exponentially
	''' decay until it is no longer perceptible ("effective zero").
	''' The larger and less absorbent the surfaces, the longer the decay time.
	''' <p>
	''' The set of parameters defined here may not include all aspects of reverberation
	''' as specified by some systems.  For example, the Midi Manufacturer's Association
	''' (MMA) has an Interactive Audio Special Interest Group (IASIG), which has a
	''' 3-D Working Group that has defined a Level 2 Spec (I3DL2).  I3DL2
	''' supports filtering of reverberation and
	''' control of reverb density.  These properties are not included in the JavaSound 1.0
	''' definition of a reverb control.  In such a case, the implementing system
	''' should either extend the defined reverb control to include additional
	''' parameters, or else interpret the system's additional capabilities in a way that fits
	''' the model described here.
	''' <p>
	''' If implementing JavaSound on a I3DL2-compliant device:
	''' <ul>
	''' <li>Filtering is disabled (high-frequency attenuations are set to 0.0 dB)
	''' <li>Density parameters are set to midway between minimum and maximum
	''' </ul>
	''' <p>
	''' The following table shows what parameter values an implementation might use for a
	''' representative set of reverberation settings.
	''' <p>
	''' 
	''' <b>Reverberation Types and Parameters</b>
	''' <p>
	''' <table border=1 cellpadding=5 summary="reverb types and params: decay time, late intensity, late delay, early intensity, and early delay">
	''' 
	''' <tr>
	'''  <th>Type</th>
	'''  <th>Decay Time (ms)</th>
	'''  <th>Late Intensity (dB)</th>
	'''  <th>Late Delay (ms)</th>
	'''  <th>Early Intensity (dB)</th>
	'''  <th>Early Delay(ms)</th>
	''' </tr>
	''' 
	''' <tr>
	'''  <td>Cavern</td>
	'''  <td>2250</td>
	'''  <td>-2.0</td>
	'''  <td>41.3</td>
	'''  <td>-1.4</td>
	'''  <td>10.3</td>
	''' </tr>
	''' 
	''' <tr>
	'''  <td>Dungeon</td>
	'''  <td>1600</td>
	'''  <td>-1.0</td>
	'''  <td>10.3</td>
	'''  <td>-0.7</td>
	'''  <td>2.6</td>
	''' </tr>
	''' 
	''' <tr>
	'''  <td>Garage</td>
	'''  <td>900</td>
	'''  <td>-6.0</td>
	'''  <td>14.7</td>
	'''  <td>-4.0</td>
	'''  <td>3.9</td>
	''' </tr>
	''' 
	''' <tr>
	'''  <td>Acoustic Lab</td>
	'''  <td>280</td>
	'''  <td>-3.0</td>
	'''  <td>8.0</td>
	'''  <td>-2.0</td>
	'''  <td>2.0</td>
	''' </tr>
	''' 
	''' <tr>
	'''  <td>Closet</td>
	'''  <td>150</td>
	'''  <td>-10.0</td>
	'''  <td>2.5</td>
	'''  <td>-7.0</td>
	'''  <td>0.6</td>
	''' </tr>
	''' 
	''' </table>
	''' 
	''' @author Kara Kytle
	''' @since 1.3
	''' </summary>
	Public Class ReverbType

		''' <summary>
		''' Descriptive name of the reverb type..
		''' </summary>
		Private name As String

		''' <summary>
		''' Early reflection delay in microseconds.
		''' </summary>
		Private earlyReflectionDelay As Integer

		''' <summary>
		''' Early reflection intensity.
		''' </summary>
		Private earlyReflectionIntensity As Single

		''' <summary>
		''' Late reflection delay in microseconds.
		''' </summary>
		Private lateReflectionDelay As Integer

		''' <summary>
		''' Late reflection intensity.
		''' </summary>
		Private lateReflectionIntensity As Single

		''' <summary>
		''' Total decay time
		''' </summary>
		Private decayTime As Integer


		''' <summary>
		''' Constructs a new reverb type that has the specified reverberation
		''' parameter values. </summary>
		''' <param name="name"> the name of the new reverb type, or a zero-length <code>String</code> </param>
		''' <param name="earlyReflectionDelay"> the new type's early reflection delay time in microseconds </param>
		''' <param name="earlyReflectionIntensity"> the new type's early reflection intensity in dB </param>
		''' <param name="lateReflectionDelay"> the new type's late reflection delay time in microseconds </param>
		''' <param name="lateReflectionIntensity"> the new type's late reflection intensity in dB </param>
		''' <param name="decayTime"> the new type's decay time in microseconds </param>
		Protected Friend Sub New(ByVal name As String, ByVal earlyReflectionDelay As Integer, ByVal earlyReflectionIntensity As Single, ByVal lateReflectionDelay As Integer, ByVal lateReflectionIntensity As Single, ByVal decayTime As Integer)

			Me.name = name
			Me.earlyReflectionDelay = earlyReflectionDelay
			Me.earlyReflectionIntensity = earlyReflectionIntensity
			Me.lateReflectionDelay = lateReflectionDelay
			Me.lateReflectionIntensity = lateReflectionIntensity
			Me.decayTime = decayTime
		End Sub


		''' <summary>
		''' Obtains the name of this reverb type. </summary>
		''' <returns> the name of this reverb type
		''' @since 1.5 </returns>
		Public Overridable Property name As String
			Get
					Return name
			End Get
		End Property


		''' <summary>
		''' Returns the early reflection delay time in microseconds.
		''' This is the amount of time between when the direct signal is
		''' heard and when the first early reflections are heard. </summary>
		''' <returns>  early reflection delay time for this reverb type, in microseconds </returns>
		Public Property earlyReflectionDelay As Integer
			Get
				Return earlyReflectionDelay
			End Get
		End Property


		''' <summary>
		''' Returns the early reflection intensity in decibels.
		''' This is the amplitude attenuation of the first early reflections
		''' relative to the direct signal. </summary>
		''' <returns>  early reflection intensity for this reverb type, in dB </returns>
		Public Property earlyReflectionIntensity As Single
			Get
				Return earlyReflectionIntensity
			End Get
		End Property


		''' <summary>
		''' Returns the late reflection delay time in microseconds.
		''' This is the amount of time between when the first early reflections
		''' are heard and when the first late reflections are heard. </summary>
		''' <returns>  late reflection delay time for this reverb type, in microseconds </returns>
		Public Property lateReflectionDelay As Integer
			Get
				Return lateReflectionDelay
			End Get
		End Property


		''' <summary>
		''' Returns the late reflection intensity in decibels.
		''' This is the amplitude attenuation of the first late reflections
		''' relative to the direct signal. </summary>
		''' <returns>  late reflection intensity for this reverb type, in dB </returns>
		Public Property lateReflectionIntensity As Single
			Get
				Return lateReflectionIntensity
			End Get
		End Property


		''' <summary>
		''' Obtains the decay time, which is the amount of time over which the
		''' late reflections attenuate to effective zero.  The effective zero
		''' value is implementation-dependent. </summary>
		''' <returns>  the decay time of the late reflections, in microseconds </returns>
		Public Property decayTime As Integer
			Get
				Return decayTime
			End Get
		End Property


		''' <summary>
		''' Indicates whether the specified object is equal to this reverb type,
		''' returning <code>true</code> if the objects are identical. </summary>
		''' <param name="obj"> the reference object with which to compare </param>
		''' <returns> <code>true</code> if this reverb type is the same as
		''' <code>obj</code>; <code>false</code> otherwise </returns>
		Public NotOverridable Overrides Function Equals(ByVal obj As Object) As Boolean
			Return MyBase.Equals(obj)
		End Function


		''' <summary>
		''' Finalizes the hashcode method.
		''' </summary>
		Public NotOverridable Overrides Function GetHashCode() As Integer
			Return MyBase.GetHashCode()
		End Function


		''' <summary>
		''' Provides a <code>String</code> representation of the reverb type,
		''' including its name and its parameter settings.
		''' The exact contents of the string may vary between implementations of
		''' Java Sound. </summary>
		''' <returns> reverberation type name and description </returns>
		Public NotOverridable Overrides Function ToString() As String

			'$$fb2001-07-20: fix for bug 4385060: The "name" attribute of class "ReverbType" is not accessible.
			'return (super.toString() + ", early reflection delay " + earlyReflectionDelay +
			Return (name & ", early reflection delay " & earlyReflectionDelay & " ns, early reflection intensity " & earlyReflectionIntensity & " dB, late deflection delay " & lateReflectionDelay & " ns, late reflection intensity " & lateReflectionIntensity & " dB, decay time " & decayTime)
		End Function

	End Class ' class ReverbType

End Namespace