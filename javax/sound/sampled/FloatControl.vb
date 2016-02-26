Imports System

'
' * Copyright (c) 1999, 2003, Oracle and/or its affiliates. All rights reserved.
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
	''' A <code>FloatControl</code> object provides control over a range of
	''' floating-point values.  Float controls are often
	''' represented in graphical user interfaces by continuously
	''' adjustable objects such as sliders or rotary knobs.  Concrete subclasses
	''' of <code>FloatControl</code> implement controls, such as gain and pan, that
	''' affect a line's audio signal in some way that an application can manipulate.
	''' The <code><seealso cref="FloatControl.Type"/></code>
	''' inner class provides static instances of types that are used to
	''' identify some common kinds of float control.
	''' <p>
	''' The <code>FloatControl</code> abstract class provides methods to set and get
	''' the control's current floating-point value.  Other methods obtain the possible
	''' range of values and the control's resolution (the smallest increment between
	''' returned values).  Some float controls allow ramping to a
	''' new value over a specified period of time.  <code>FloatControl</code> also
	''' includes methods that return string labels for the minimum, maximum, and midpoint
	''' positions of the control.
	''' </summary>
	''' <seealso cref= Line#getControls </seealso>
	''' <seealso cref= Line#isControlSupported
	''' 
	''' @author David Rivas
	''' @author Kara Kytle
	''' @since 1.3 </seealso>
	Public MustInherit Class FloatControl
		Inherits Control


		' INSTANCE VARIABLES


		' FINAL VARIABLES

		''' <summary>
		''' The minimum supported value.
		''' </summary>
		Private minimum As Single

		''' <summary>
		''' The maximum supported value.
		''' </summary>
		Private maximum As Single

		''' <summary>
		''' The control's precision.
		''' </summary>
		Private precision As Single

		''' <summary>
		''' The smallest time increment in which a value change
		''' can be effected during a value shift, in microseconds.
		''' </summary>
		Private updatePeriod As Integer


		''' <summary>
		''' A label for the units in which the control values are expressed,
		''' such as "dB" for decibels.
		''' </summary>
		Private ReadOnly units As String

		''' <summary>
		''' A label for the minimum value, such as "Left."
		''' </summary>
		Private ReadOnly minLabel As String

		''' <summary>
		''' A label for the maximum value, such as "Right."
		''' </summary>
		Private ReadOnly maxLabel As String

		''' <summary>
		''' A label for the mid-point value, such as "Center."
		''' </summary>
		Private ReadOnly midLabel As String


		' STATE VARIABLES

		''' <summary>
		''' The current value.
		''' </summary>
		Private value As Single



		' CONSTRUCTORS


		''' <summary>
		''' Constructs a new float control object with the given parameters
		''' </summary>
		''' <param name="type"> the kind of control represented by this float control object </param>
		''' <param name="minimum"> the smallest value permitted for the control </param>
		''' <param name="maximum"> the largest value permitted for the control </param>
		''' <param name="precision"> the resolution or granularity of the control.
		''' This is the size of the increment between discrete valid values. </param>
		''' <param name="updatePeriod"> the smallest time interval, in microseconds, over which the control
		''' can change from one discrete value to the next during a <seealso cref="#shift(float,float,int) shift"/> </param>
		''' <param name="initialValue"> the value that the control starts with when constructed </param>
		''' <param name="units"> the label for the units in which the control's values are expressed,
		''' such as "dB" or "frames per second" </param>
		''' <param name="minLabel"> the label for the minimum value, such as "Left" or "Off" </param>
		''' <param name="midLabel"> the label for the midpoint value, such as "Center" or "Default" </param>
		''' <param name="maxLabel"> the label for the maximum value, such as "Right" or "Full"
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code minimum} is greater
		'''     than {@code maximum} or {@code initialValue} does not fall
		'''     within the allowable range </exception>
		Protected Friend Sub New(ByVal type As Type, ByVal minimum As Single, ByVal maximum As Single, ByVal precision As Single, ByVal updatePeriod As Integer, ByVal initialValue As Single, ByVal units As String, ByVal minLabel As String, ByVal midLabel As String, ByVal maxLabel As String)

			MyBase.New(type)

			If minimum > maximum Then Throw New System.ArgumentException("Minimum value " & minimum & " exceeds maximum value " & maximum & ".")
			If initialValue < minimum Then Throw New System.ArgumentException("Initial value " & initialValue & " smaller than allowable minimum value " & minimum & ".")
			If initialValue > maximum Then Throw New System.ArgumentException("Initial value " & initialValue & " exceeds allowable maximum value " & maximum & ".")


			Me.minimum = minimum
			Me.maximum = maximum

			Me.precision = precision
			Me.updatePeriod = updatePeriod
			Me.value = initialValue

			Me.units = units
			Me.minLabel = (If(minLabel Is Nothing, "", minLabel))
			Me.midLabel = (If(midLabel Is Nothing, "", midLabel))
			Me.maxLabel = (If(maxLabel Is Nothing, "", maxLabel))
		End Sub


		''' <summary>
		''' Constructs a new float control object with the given parameters.
		''' The labels for the minimum, maximum, and mid-point values are set
		''' to zero-length strings.
		''' </summary>
		''' <param name="type"> the kind of control represented by this float control object </param>
		''' <param name="minimum"> the smallest value permitted for the control </param>
		''' <param name="maximum"> the largest value permitted for the control </param>
		''' <param name="precision"> the resolution or granularity of the control.
		''' This is the size of the increment between discrete valid values. </param>
		''' <param name="updatePeriod"> the smallest time interval, in microseconds, over which the control
		''' can change from one discrete value to the next during a <seealso cref="#shift(float,float,int) shift"/> </param>
		''' <param name="initialValue"> the value that the control starts with when constructed </param>
		''' <param name="units"> the label for the units in which the control's values are expressed,
		''' such as "dB" or "frames per second"
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code minimum} is greater
		'''     than {@code maximum} or {@code initialValue} does not fall
		'''     within the allowable range </exception>
		Protected Friend Sub New(ByVal type As Type, ByVal minimum As Single, ByVal maximum As Single, ByVal precision As Single, ByVal updatePeriod As Integer, ByVal initialValue As Single, ByVal units As String)
			Me.New(type, minimum, maximum, precision, updatePeriod, initialValue, units, "", "", "")
		End Sub



		' METHODS


		''' <summary>
		''' Sets the current value for the control.  The default implementation
		''' simply sets the value as indicated.  If the value indicated is greater
		''' than the maximum value, or smaller than the minimum value, an
		''' IllegalArgumentException is thrown.
		''' Some controls require that their line be open before they can be affected
		''' by setting a value. </summary>
		''' <param name="newValue"> desired new value </param>
		''' <exception cref="IllegalArgumentException"> if the value indicated does not fall
		''' within the allowable range </exception>
		Public Overridable Property value As Single
			Set(ByVal newValue As Single)
    
				If newValue > maximum Then Throw New System.ArgumentException("Requested value " & newValue & " exceeds allowable maximum value " & maximum & ".")
    
				If newValue < minimum Then Throw New System.ArgumentException("Requested value " & newValue & " smaller than allowable minimum value " & minimum & ".")
    
				value = newValue
			End Set
			Get
				Return value
			End Get
		End Property




		''' <summary>
		''' Obtains the maximum value permitted. </summary>
		''' <returns> the maximum allowable value </returns>
		Public Overridable Property maximum As Single
			Get
				Return maximum
			End Get
		End Property


		''' <summary>
		''' Obtains the minimum value permitted. </summary>
		''' <returns> the minimum allowable value </returns>
		Public Overridable Property minimum As Single
			Get
				Return minimum
			End Get
		End Property


		''' <summary>
		''' Obtains the label for the units in which the control's values are expressed,
		''' such as "dB" or "frames per second." </summary>
		''' <returns> the units label, or a zero-length string if no label </returns>
		Public Overridable Property units As String
			Get
				Return units
			End Get
		End Property


		''' <summary>
		''' Obtains the label for the minimum value, such as "Left" or "Off." </summary>
		''' <returns> the minimum value label, or a zero-length string if no label      * has been set </returns>
		Public Overridable Property minLabel As String
			Get
				Return minLabel
			End Get
		End Property


		''' <summary>
		''' Obtains the label for the mid-point value, such as "Center" or "Default." </summary>
		''' <returns> the mid-point value label, or a zero-length string if no label    * has been set </returns>
		Public Overridable Property midLabel As String
			Get
				Return midLabel
			End Get
		End Property


		''' <summary>
		''' Obtains the label for the maximum value, such as "Right" or "Full." </summary>
		''' <returns> the maximum value label, or a zero-length string if no label      * has been set </returns>
		Public Overridable Property maxLabel As String
			Get
				Return maxLabel
			End Get
		End Property


		''' <summary>
		''' Obtains the resolution or granularity of the control, in the units
		''' that the control measures.
		''' The precision is the size of the increment between discrete valid values
		''' for this control, over the set of supported floating-point values. </summary>
		''' <returns> the control's precision </returns>
		Public Overridable Property precision As Single
			Get
				Return precision
			End Get
		End Property


		''' <summary>
		''' Obtains the smallest time interval, in microseconds, over which the control's value can
		''' change during a shift.  The update period is the inverse of the frequency with which
		''' the control updates its value during a shift.  If the implementation does not support value shifting over
		''' time, it should set the control's value to the final value immediately
		''' and return -1 from this method.
		''' </summary>
		''' <returns> update period in microseconds, or -1 if shifting over time is unsupported </returns>
		''' <seealso cref= #shift </seealso>
		Public Overridable Property updatePeriod As Integer
			Get
				Return updatePeriod
			End Get
		End Property


		''' <summary>
		''' Changes the control value from the initial value to the final
		''' value linearly over the specified time period, specified in microseconds.
		''' This method returns without blocking; it does not wait for the shift
		''' to complete.  An implementation should complete the operation within the time
		''' specified.  The default implementation simply changes the value
		''' to the final value immediately.
		''' </summary>
		''' <param name="from"> initial value at the beginning of the shift </param>
		''' <param name="to"> final value after the shift </param>
		''' <param name="microseconds"> maximum duration of the shift in microseconds
		''' </param>
		''' <exception cref="IllegalArgumentException"> if either {@code from} or {@code to}
		'''     value does not fall within the allowable range
		''' </exception>
		''' <seealso cref= #getUpdatePeriod </seealso>
		Public Overridable Sub shift(ByVal [from] As Single, ByVal [to] As Single, ByVal microseconds As Integer)
			' test "from" value, "to" value will be tested by setValue()
			If [from] < minimum Then Throw New System.ArgumentException("Requested value " & [from] & " smaller than allowable minimum value " & minimum & ".")
			If [from] > maximum Then Throw New System.ArgumentException("Requested value " & [from] & " exceeds allowable maximum value " & maximum & ".")
			value = [to]
		End Sub


		' ABSTRACT METHOD IMPLEMENTATIONS: CONTROL


		''' <summary>
		''' Provides a string representation of the control </summary>
		''' <returns> a string description </returns>
		Public Overrides Function ToString() As String
			Return New String(type & " with current value: " & value & " " & units & " (range: " & minimum & " - " & maximum & ")")
		End Function


		' INNER CLASSES


		''' <summary>
		''' An instance of the <code>FloatControl.Type</code> inner class identifies one kind of
		''' float control.  Static instances are provided for the
		''' common types.
		''' 
		''' @author Kara Kytle
		''' @since 1.3
		''' </summary>
		Public Class Type
			Inherits Control.Type


			' TYPE DEFINES


			' GAIN TYPES

			''' <summary>
			''' Represents a control for the overall gain on a line.
			''' <p>
			''' Gain is a quantity in decibels (dB) that is added to the intrinsic
			''' decibel level of the audio signal--that is, the level of
			''' the signal before it is altered by the gain control.  A positive
			''' gain amplifies (boosts) the signal's volume, and a negative gain
			''' attenuates (cuts) it.
			''' The gain setting defaults to a value of 0.0 dB, meaning the signal's
			''' loudness is unaffected.   Note that gain measures dB, not amplitude.
			''' The relationship between a gain in decibels and the corresponding
			''' linear amplitude multiplier is:
			''' 
			''' <CENTER><CODE> linearScalar = pow(10.0, gainDB/20.0) </CODE></CENTER>
			''' <p>
			''' The <code>FloatControl</code> class has methods to impose a maximum and
			''' minimum allowable value for gain.  However, because an audio signal might
			''' already be at a high amplitude, the maximum setting does not guarantee
			''' that the signal will be undistorted when the gain is applied to it (unless
			''' the maximum is zero or negative). To avoid numeric overflow from excessively
			''' large gain settings, a gain control can implement
			''' clipping, meaning that the signal's amplitude will be limited to the maximum
			''' value representable by its audio format, instead of wrapping around.
			''' <p>
			''' These comments apply to gain controls in general, not just master gain controls.
			''' A line can have more than one gain control.  For example, a mixer (which is
			''' itself a line) might have a master gain control, an auxiliary return control,
			''' a reverb return control, and, on each of its source lines, an individual aux
			''' send and reverb send.
			''' </summary>
			''' <seealso cref= #AUX_SEND </seealso>
			''' <seealso cref= #AUX_RETURN </seealso>
			''' <seealso cref= #REVERB_SEND </seealso>
			''' <seealso cref= #REVERB_RETURN </seealso>
			''' <seealso cref= #VOLUME </seealso>
			Public Shared ReadOnly MASTER_GAIN As New Type("Master Gain")

			''' <summary>
			''' Represents a control for the auxiliary send gain on a line.
			''' </summary>
			''' <seealso cref= #MASTER_GAIN </seealso>
			''' <seealso cref= #AUX_RETURN </seealso>
			Public Shared ReadOnly AUX_SEND As New Type("AUX Send")

			''' <summary>
			''' Represents a control for the auxiliary return gain on a line.
			''' </summary>
			''' <seealso cref= #MASTER_GAIN </seealso>
			''' <seealso cref= #AUX_SEND </seealso>
			Public Shared ReadOnly AUX_RETURN As New Type("AUX Return")

			''' <summary>
			''' Represents a control for the pre-reverb gain on a line.
			''' This control may be used to affect how much
			''' of a line's signal is directed to a mixer's internal reverberation unit.
			''' </summary>
			''' <seealso cref= #MASTER_GAIN </seealso>
			''' <seealso cref= #REVERB_RETURN </seealso>
			''' <seealso cref= EnumControl.Type#REVERB </seealso>
			Public Shared ReadOnly REVERB_SEND As New Type("Reverb Send")

			''' <summary>
			''' Represents a control for the post-reverb gain on a line.
			''' This control may be used to control the relative amplitude
			''' of the signal returned from an internal reverberation unit.
			''' </summary>
			''' <seealso cref= #MASTER_GAIN </seealso>
			''' <seealso cref= #REVERB_SEND </seealso>
			Public Shared ReadOnly REVERB_RETURN As New Type("Reverb Return")


			' VOLUME

			''' <summary>
			''' Represents a control for the volume on a line.
			''' </summary>
	'        
	'         * $$kk: 08.30.99: ISSUE: what units?  linear or dB?
	'         
			Public Shared ReadOnly VOLUME As New Type("Volume")


			' PAN

			''' <summary>
			''' Represents a control for the relative pan (left-right positioning)
			''' of the signal.  The signal may be mono; the pan setting affects how
			''' it is distributed by the mixer in a stereo mix.  The valid range of values is -1.0
			''' (left channel only) to 1.0 (right channel
			''' only).  The default is 0.0 (centered).
			''' </summary>
			''' <seealso cref= #BALANCE </seealso>
			Public Shared ReadOnly PAN As New Type("Pan")


			' BALANCE

			''' <summary>
			''' Represents a control for the relative balance of a stereo signal
			''' between two stereo speakers.  The valid range of values is -1.0 (left channel only) to 1.0 (right channel
			''' only).  The default is 0.0 (centered).
			''' </summary>
			''' <seealso cref= #PAN </seealso>
			Public Shared ReadOnly BALANCE As New Type("Balance")


			' SAMPLE RATE

			''' <summary>
			''' Represents a control that changes the sample rate of audio playback.  The net effect
			''' of changing the sample rate depends on the relationship between
			''' the media's natural rate and the rate that is set via this control.
			''' The natural rate is the sample rate that is specified in the data line's
			''' <code>AudioFormat</code> object.  For example, if the natural rate
			''' of the media is 11025 samples per second and the sample rate is set
			''' to 22050 samples per second, the media will play back at twice the
			''' normal speed.
			''' <p>
			''' Changing the sample rate with this control does not affect the data line's
			''' audio format.  Also note that whenever you change a sound's sample rate, a
			''' change in the sound's pitch results.  For example, doubling the sample
			''' rate has the effect of doubling the frequencies in the sound's spectrum,
			''' which raises the pitch by an octave.
			''' </summary>
			Public Shared ReadOnly SAMPLE_RATE As New Type("Sample Rate")


			' CONSTRUCTOR

			''' <summary>
			''' Constructs a new float control type. </summary>
			''' <param name="name">  the name of the new float control type </param>
			Protected Friend Sub New(ByVal name As String)
				MyBase.New(name)
			End Sub

		End Class ' class Type

	End Class ' class FloatControl

End Namespace