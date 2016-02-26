Imports System
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

Namespace javax.sound.sampled


	''' <summary>
	''' <code>AudioFormat</code> is the class that specifies a particular arrangement of data in a sound stream.
	''' By examining the information stored in the audio format, you can discover how to interpret the bits in the
	''' binary sound data.
	''' <p>
	''' Every data line has an audio format associated with its data stream. The audio format of a source (playback) data line indicates
	''' what kind of data the data line expects to receive for output.  For a target (capture) data line, the audio format specifies the kind
	''' of the data that can be read from the line.
	''' Sound files also have audio formats, of course.  The <code><seealso cref="AudioFileFormat"/></code>
	''' class encapsulates an <code>AudioFormat</code> in addition to other,
	''' file-specific information.  Similarly, an <code><seealso cref="AudioInputStream"/></code> has an
	''' <code>AudioFormat</code>.
	''' <p>
	''' The <code>AudioFormat</code> class accommodates a number of common sound-file encoding techniques, including
	''' pulse-code modulation (PCM), mu-law encoding, and a-law encoding.  These encoding techniques are predefined,
	''' but service providers can create new encoding types.
	''' The encoding that a specific format uses is named by its <code>encoding</code> field.
	''' <p>
	''' In addition to the encoding, the audio format includes other properties that further specify the exact
	''' arrangement of the data.
	''' These include the number of channels, sample rate, sample size, byte order, frame rate, and frame size.
	''' Sounds may have different numbers of audio channels: one for mono, two for stereo.
	''' The sample rate measures how many "snapshots" (samples) of the sound pressure are taken per second, per channel.
	''' (If the sound is stereo rather than mono, two samples are actually measured at each instant of time: one for the left channel,
	''' and another for the right channel; however, the sample rate still measures the number per channel, so the rate is the same
	''' regardless of the number of channels.   This is the standard use of the term.)
	''' The sample size indicates how many bits are used to store each snapshot; 8 and 16 are typical values.
	''' For 16-bit samples (or any other sample size larger than a byte),
	''' byte order is important; the bytes in each sample are arranged in
	''' either the "little-endian" or "big-endian" style.
	''' For encodings like PCM, a frame consists of the set of samples for all channels at a given
	''' point in time, and so the size of a frame (in bytes) is always equal to the size of a sample (in bytes) times
	''' the number of channels.  However, with some other sorts of encodings a frame can contain
	''' a bundle of compressed data for a whole series of samples, as well as additional, non-sample
	''' data.  For such encodings, the sample rate and sample size refer to the data after it is decoded into PCM,
	''' and so they are completely different from the frame rate and frame size.
	''' 
	''' <p>An <code>AudioFormat</code> object can include a set of
	''' properties. A property is a pair of key and value: the key
	''' is of type <code>String</code>, the associated property
	''' value is an arbitrary object. Properties specify
	''' additional format specifications, like the bit rate for
	''' compressed formats. Properties are mainly used as a means
	''' to transport additional information of the audio format
	''' to and from the service providers. Therefore, properties
	''' are ignored in the <seealso cref="#matches(AudioFormat)"/> method.
	''' However, methods which rely on the installed service
	''' providers, like {@link AudioSystem#isConversionSupported
	''' (AudioFormat, AudioFormat) isConversionSupported} may consider
	''' properties, depending on the respective service provider
	''' implementation.
	''' 
	''' <p>The following table lists some common properties which
	''' service providers should use, if applicable:
	''' 
	''' <table border=0>
	'''  <caption>Audio Format Properties</caption>
	'''  <tr>
	'''   <th>Property key</th>
	'''   <th>Value type</th>
	'''   <th>Description</th>
	'''  </tr>
	'''  <tr>
	'''   <td>&quot;bitrate&quot;</td>
	'''   <td><seealso cref="java.lang.Integer Integer"/></td>
	'''   <td>average bit rate in bits per second</td>
	'''  </tr>
	'''  <tr>
	'''   <td>&quot;vbr&quot;</td>
	'''   <td><seealso cref="java.lang.Boolean Boolean"/></td>
	'''   <td><code>true</code>, if the file is encoded in variable bit
	'''       rate (VBR)</td>
	'''  </tr>
	'''  <tr>
	'''   <td>&quot;quality&quot;</td>
	'''   <td><seealso cref="java.lang.Integer Integer"/></td>
	'''   <td>encoding/conversion quality, 1..100</td>
	'''  </tr>
	''' </table>
	''' 
	''' <p>Vendors of service providers (plugins) are encouraged
	''' to seek information about other already established
	''' properties in third party plugins, and follow the same
	''' conventions.
	''' 
	''' @author Kara Kytle
	''' @author Florian Bomers </summary>
	''' <seealso cref= DataLine#getFormat </seealso>
	''' <seealso cref= AudioInputStream#getFormat </seealso>
	''' <seealso cref= AudioFileFormat </seealso>
	''' <seealso cref= javax.sound.sampled.spi.FormatConversionProvider
	''' @since 1.3 </seealso>
	Public Class AudioFormat

		' INSTANCE VARIABLES


		''' <summary>
		''' The audio encoding technique used by this format.
		''' </summary>
		Protected Friend encoding As Encoding

		''' <summary>
		''' The number of samples played or recorded per second, for sounds that have this format.
		''' </summary>
		Protected Friend sampleRate As Single

		''' <summary>
		''' The number of bits in each sample of a sound that has this format.
		''' </summary>
		Protected Friend sampleSizeInBits As Integer

		''' <summary>
		''' The number of audio channels in this format (1 for mono, 2 for stereo).
		''' </summary>
		Protected Friend channels As Integer

		''' <summary>
		''' The number of bytes in each frame of a sound that has this format.
		''' </summary>
		Protected Friend frameSize As Integer

		''' <summary>
		''' The number of frames played or recorded per second, for sounds that have this format.
		''' </summary>
		Protected Friend frameRate As Single

		''' <summary>
		''' Indicates whether the audio data is stored in big-endian or little-endian order.
		''' </summary>
		Protected Friend bigEndian As Boolean


		''' <summary>
		''' The set of properties </summary>
		Private ___properties As Dictionary(Of String, Object)


		''' <summary>
		''' Constructs an <code>AudioFormat</code> with the given parameters.
		''' The encoding specifies the convention used to represent the data.
		''' The other parameters are further explained in the {@link AudioFormat
		''' class description}. </summary>
		''' <param name="encoding">                  the audio encoding technique </param>
		''' <param name="sampleRate">                the number of samples per second </param>
		''' <param name="sampleSizeInBits">  the number of bits in each sample </param>
		''' <param name="channels">                  the number of channels (1 for mono, 2 for stereo, and so on) </param>
		''' <param name="frameSize">                 the number of bytes in each frame </param>
		''' <param name="frameRate">                 the number of frames per second </param>
		''' <param name="bigEndian">                 indicates whether the data for a single sample
		'''                                                  is stored in big-endian byte order (<code>false</code>
		'''                                                  means little-endian) </param>
		Public Sub New(ByVal ___encoding As Encoding, ByVal sampleRate As Single, ByVal sampleSizeInBits As Integer, ByVal channels As Integer, ByVal frameSize As Integer, ByVal frameRate As Single, ByVal bigEndian As Boolean)

			Me.encoding = ___encoding
			Me.sampleRate = sampleRate
			Me.sampleSizeInBits = sampleSizeInBits
			Me.channels = channels
			Me.frameSize = frameSize
			Me.frameRate = frameRate
			Me.bigEndian = bigEndian
			Me.___properties = Nothing
		End Sub


		''' <summary>
		''' Constructs an <code>AudioFormat</code> with the given parameters.
		''' The encoding specifies the convention used to represent the data.
		''' The other parameters are further explained in the {@link AudioFormat
		''' class description}. </summary>
		''' <param name="encoding">         the audio encoding technique </param>
		''' <param name="sampleRate">       the number of samples per second </param>
		''' <param name="sampleSizeInBits"> the number of bits in each sample </param>
		''' <param name="channels">         the number of channels (1 for mono, 2 for
		'''                         stereo, and so on) </param>
		''' <param name="frameSize">        the number of bytes in each frame </param>
		''' <param name="frameRate">        the number of frames per second </param>
		''' <param name="bigEndian">        indicates whether the data for a single sample
		'''                         is stored in big-endian byte order
		'''                         (<code>false</code> means little-endian) </param>
		''' <param name="properties">       a <code>Map&lt;String,Object&gt;</code> object
		'''                         containing format properties
		''' 
		''' @since 1.5 </param>
		Public Sub New(ByVal ___encoding As Encoding, ByVal sampleRate As Single, ByVal sampleSizeInBits As Integer, ByVal channels As Integer, ByVal frameSize As Integer, ByVal frameRate As Single, ByVal bigEndian As Boolean, ByVal properties As IDictionary(Of String, Object))
			Me.New(___encoding, sampleRate, sampleSizeInBits, channels, frameSize, frameRate, bigEndian)
			Me.___properties = New Dictionary(Of String, Object)(properties)
		End Sub


		''' <summary>
		''' Constructs an <code>AudioFormat</code> with a linear PCM encoding and
		''' the given parameters.  The frame size is set to the number of bytes
		''' required to contain one sample from each channel, and the frame rate
		''' is set to the sample rate.
		''' </summary>
		''' <param name="sampleRate">                the number of samples per second </param>
		''' <param name="sampleSizeInBits">  the number of bits in each sample </param>
		''' <param name="channels">                  the number of channels (1 for mono, 2 for stereo, and so on) </param>
		''' <param name="signed">                    indicates whether the data is signed or unsigned </param>
		''' <param name="bigEndian">                 indicates whether the data for a single sample
		'''                                                  is stored in big-endian byte order (<code>false</code>
		'''                                                  means little-endian) </param>
		Public Sub New(ByVal sampleRate As Single, ByVal sampleSizeInBits As Integer, ByVal channels As Integer, ByVal signed As Boolean, ByVal bigEndian As Boolean)

			Me.New((If(signed = True, Encoding.PCM_SIGNED, Encoding.PCM_UNSIGNED)), sampleRate, sampleSizeInBits, channels,If(channels = AudioSystem.NOT_SPECIFIED OrElse sampleSizeInBits = AudioSystem.NOT_SPECIFIED, AudioSystem.NOT_SPECIFIED, ((sampleSizeInBits + 7) \ 8) * channels), sampleRate, bigEndian)
		End Sub

		''' <summary>
		''' Obtains the type of encoding for sounds in this format.
		''' </summary>
		''' <returns> the encoding type </returns>
		''' <seealso cref= Encoding#PCM_SIGNED </seealso>
		''' <seealso cref= Encoding#PCM_UNSIGNED </seealso>
		''' <seealso cref= Encoding#ULAW </seealso>
		''' <seealso cref= Encoding#ALAW </seealso>
		Public Overridable Property encoding As Encoding
			Get
    
				Return encoding
			End Get
		End Property

		''' <summary>
		''' Obtains the sample rate.
		''' For compressed formats, the return value is the sample rate of the uncompressed
		''' audio data.
		''' When this AudioFormat is used for queries (e.g. {@link
		''' AudioSystem#isConversionSupported(AudioFormat, AudioFormat)
		''' AudioSystem.isConversionSupported}) or capabilities (e.g. {@link
		''' DataLine.Info#getFormats() DataLine.Info.getFormats}), a sample rate of
		''' <code>AudioSystem.NOT_SPECIFIED</code> means that any sample rate is
		''' acceptable. <code>AudioSystem.NOT_SPECIFIED</code> is also returned when
		''' the sample rate is not defined for this audio format. </summary>
		''' <returns> the number of samples per second,
		''' or <code>AudioSystem.NOT_SPECIFIED</code>
		''' </returns>
		''' <seealso cref= #getFrameRate() </seealso>
		''' <seealso cref= AudioSystem#NOT_SPECIFIED </seealso>
		Public Overridable Property sampleRate As Single
			Get
    
				Return sampleRate
			End Get
		End Property

		''' <summary>
		''' Obtains the size of a sample.
		''' For compressed formats, the return value is the sample size of the
		''' uncompressed audio data.
		''' When this AudioFormat is used for queries (e.g. {@link
		''' AudioSystem#isConversionSupported(AudioFormat, AudioFormat)
		''' AudioSystem.isConversionSupported}) or capabilities (e.g. {@link
		''' DataLine.Info#getFormats() DataLine.Info.getFormats}), a sample size of
		''' <code>AudioSystem.NOT_SPECIFIED</code> means that any sample size is
		''' acceptable. <code>AudioSystem.NOT_SPECIFIED</code> is also returned when
		''' the sample size is not defined for this audio format. </summary>
		''' <returns> the number of bits in each sample,
		''' or <code>AudioSystem.NOT_SPECIFIED</code>
		''' </returns>
		''' <seealso cref= #getFrameSize() </seealso>
		''' <seealso cref= AudioSystem#NOT_SPECIFIED </seealso>
		Public Overridable Property sampleSizeInBits As Integer
			Get
    
				Return sampleSizeInBits
			End Get
		End Property

		''' <summary>
		''' Obtains the number of channels.
		''' When this AudioFormat is used for queries (e.g. {@link
		''' AudioSystem#isConversionSupported(AudioFormat, AudioFormat)
		''' AudioSystem.isConversionSupported}) or capabilities (e.g. {@link
		''' DataLine.Info#getFormats() DataLine.Info.getFormats}), a return value of
		''' <code>AudioSystem.NOT_SPECIFIED</code> means that any (positive) number of channels is
		''' acceptable. </summary>
		''' <returns> The number of channels (1 for mono, 2 for stereo, etc.),
		''' or <code>AudioSystem.NOT_SPECIFIED</code>
		''' </returns>
		''' <seealso cref= AudioSystem#NOT_SPECIFIED </seealso>
		Public Overridable Property channels As Integer
			Get
    
				Return channels
			End Get
		End Property

		''' <summary>
		''' Obtains the frame size in bytes.
		''' When this AudioFormat is used for queries (e.g. {@link
		''' AudioSystem#isConversionSupported(AudioFormat, AudioFormat)
		''' AudioSystem.isConversionSupported}) or capabilities (e.g. {@link
		''' DataLine.Info#getFormats() DataLine.Info.getFormats}), a frame size of
		''' <code>AudioSystem.NOT_SPECIFIED</code> means that any frame size is
		''' acceptable. <code>AudioSystem.NOT_SPECIFIED</code> is also returned when
		''' the frame size is not defined for this audio format. </summary>
		''' <returns> the number of bytes per frame,
		''' or <code>AudioSystem.NOT_SPECIFIED</code>
		''' </returns>
		''' <seealso cref= #getSampleSizeInBits() </seealso>
		''' <seealso cref= AudioSystem#NOT_SPECIFIED </seealso>
		Public Overridable Property frameSize As Integer
			Get
    
				Return frameSize
			End Get
		End Property

		''' <summary>
		''' Obtains the frame rate in frames per second.
		''' When this AudioFormat is used for queries (e.g. {@link
		''' AudioSystem#isConversionSupported(AudioFormat, AudioFormat)
		''' AudioSystem.isConversionSupported}) or capabilities (e.g. {@link
		''' DataLine.Info#getFormats() DataLine.Info.getFormats}), a frame rate of
		''' <code>AudioSystem.NOT_SPECIFIED</code> means that any frame rate is
		''' acceptable. <code>AudioSystem.NOT_SPECIFIED</code> is also returned when
		''' the frame rate is not defined for this audio format. </summary>
		''' <returns> the number of frames per second,
		''' or <code>AudioSystem.NOT_SPECIFIED</code>
		''' </returns>
		''' <seealso cref= #getSampleRate() </seealso>
		''' <seealso cref= AudioSystem#NOT_SPECIFIED </seealso>
		Public Overridable Property frameRate As Single
			Get
    
				Return frameRate
			End Get
		End Property


		''' <summary>
		''' Indicates whether the audio data is stored in big-endian or little-endian
		''' byte order.  If the sample size is not more than one byte, the return value is
		''' irrelevant. </summary>
		''' <returns> <code>true</code> if the data is stored in big-endian byte order,
		''' <code>false</code> if little-endian </returns>
		Public Overridable Property bigEndian As Boolean
			Get
    
				Return bigEndian
			End Get
		End Property


		''' <summary>
		''' Obtain an unmodifiable map of properties.
		''' The concept of properties is further explained in
		''' the <seealso cref="AudioFileFormat class description"/>.
		''' </summary>
		''' <returns> a <code>Map&lt;String,Object&gt;</code> object containing
		'''         all properties. If no properties are recognized, an empty map is
		'''         returned.
		''' </returns>
		''' <seealso cref= #getProperty(String)
		''' @since 1.5 </seealso>
		Public Overridable Function properties() As IDictionary(Of String, Object)
			Dim ret As IDictionary(Of String, Object)
			If ___properties Is Nothing Then
				ret = New Dictionary(Of String, Object)(0)
			Else
				ret = CType(___properties.clone(), IDictionary(Of String, Object))
			End If
			Return CType(java.util.Collections.unmodifiableMap(ret), IDictionary(Of String, Object))
		End Function


		''' <summary>
		''' Obtain the property value specified by the key.
		''' The concept of properties is further explained in
		''' the <seealso cref="AudioFileFormat class description"/>.
		''' 
		''' <p>If the specified property is not defined for a
		''' particular file format, this method returns
		''' <code>null</code>.
		''' </summary>
		''' <param name="key"> the key of the desired property </param>
		''' <returns> the value of the property with the specified key,
		'''         or <code>null</code> if the property does not exist.
		''' </returns>
		''' <seealso cref= #properties()
		''' @since 1.5 </seealso>
		Public Overridable Function getProperty(ByVal key As String) As Object
			If ___properties Is Nothing Then Return Nothing
			Return ___properties(key)
		End Function


		''' <summary>
		''' Indicates whether this format matches the one specified.
		''' To match, two formats must have the same encoding,
		''' and consistent values of the number of channels, sample rate, sample size,
		''' frame rate, and frame size.
		''' The values of the property are consistent if they are equal
		''' or the specified format has the property value
		''' {@code AudioSystem.NOT_SPECIFIED}.
		''' The byte order (big-endian or little-endian) must be the same
		''' if the sample size is greater than one byte.
		''' </summary>
		''' <param name="format"> format to test for match </param>
		''' <returns> {@code true} if this format matches the one specified,
		'''         {@code false} otherwise. </returns>
		Public Overridable Function matches(ByVal format As AudioFormat) As Boolean
			If format.encoding.Equals(encoding) AndAlso (format.channels = AudioSystem.NOT_SPECIFIED OrElse format.channels = channels) AndAlso (format.sampleRate = CSng(AudioSystem.NOT_SPECIFIED) OrElse format.sampleRate = sampleRate) AndAlso (format.sampleSizeInBits = AudioSystem.NOT_SPECIFIED OrElse format.sampleSizeInBits = sampleSizeInBits) AndAlso (format.frameRate = CSng(AudioSystem.NOT_SPECIFIED) OrElse format.frameRate = frameRate) AndAlso (format.frameSize = AudioSystem.NOT_SPECIFIED OrElse format.frameSize = frameSize) AndAlso (sampleSizeInBits <= 8 OrElse format.bigEndian = bigEndian) Then Return True
			Return False
		End Function


		''' <summary>
		''' Returns a string that describes the format, such as:
		''' "PCM SIGNED 22050 Hz 16 bit mono big-endian".  The contents of the string
		''' may vary between implementations of Java Sound.
		''' </summary>
		''' <returns> a string that describes the format parameters </returns>
		Public Overrides Function ToString() As String
			Dim sEncoding As String = ""
			If encoding IsNot Nothing Then sEncoding = encoding.ToString() & " "

			Dim sSampleRate As String
			If sampleRate = CSng(AudioSystem.NOT_SPECIFIED) Then
				sSampleRate = "unknown sample rate, "
			Else
				sSampleRate = "" & sampleRate & " Hz, "
			End If

			Dim sSampleSizeInBits As String
			If sampleSizeInBits = CSng(AudioSystem.NOT_SPECIFIED) Then
				sSampleSizeInBits = "unknown bits per sample, "
			Else
				sSampleSizeInBits = "" & sampleSizeInBits & " bit, "
			End If

			Dim sChannels As String
			If channels = 1 Then
				sChannels = "mono, "
			Else
				If channels = 2 Then
					sChannels = "stereo, "
				Else
					If channels = AudioSystem.NOT_SPECIFIED Then
						sChannels = " unknown number of channels, "
					Else
						sChannels = "" & channels & " channels, "
					End If
				End If
			End If

			Dim sFrameSize As String
			If frameSize = CSng(AudioSystem.NOT_SPECIFIED) Then
				sFrameSize = "unknown frame size, "
			Else
				sFrameSize = "" & frameSize & " bytes/frame, "
			End If

			Dim sFrameRate As String = ""
			If Math.Abs(sampleRate - frameRate) > 0.00001 Then
				If frameRate = CSng(AudioSystem.NOT_SPECIFIED) Then
					sFrameRate = "unknown frame rate, "
				Else
					sFrameRate = frameRate & " frames/second, "
				End If
			End If

			Dim sEndian As String = ""
			If (encoding.Equals(Encoding.PCM_SIGNED) OrElse encoding.Equals(Encoding.PCM_UNSIGNED)) AndAlso ((sampleSizeInBits > 8) OrElse (sampleSizeInBits = AudioSystem.NOT_SPECIFIED)) Then
				If bigEndian Then
					sEndian = "big-endian"
				Else
					sEndian = "little-endian"
				End If
			End If

			Return sEncoding + sSampleRate + sSampleSizeInBits + sChannels + sFrameSize + sFrameRate + sEndian

		End Function

		''' <summary>
		''' The <code>Encoding</code> class  names the  specific type of data representation
		''' used for an audio stream.   The encoding includes aspects of the
		''' sound format other than the number of channels, sample rate, sample size,
		''' frame rate, frame size, and byte order.
		''' <p>
		''' One ubiquitous type of audio encoding is pulse-code modulation (PCM),
		''' which is simply a linear (proportional) representation of the sound
		''' waveform.  With PCM, the number stored in each sample is proportional
		''' to the instantaneous amplitude of the sound pressure at that point in
		''' time.  The numbers may be signed or unsigned integers or floats.
		''' Besides PCM, other encodings include mu-law and a-law, which are nonlinear
		''' mappings of the sound amplitude that are often used for recording speech.
		''' <p>
		''' You can use a predefined encoding by referring to one of the static
		''' objects created by this class, such as PCM_SIGNED or
		''' PCM_UNSIGNED.  Service providers can create new encodings, such as
		''' compressed audio formats, and make
		''' these available through the <code><seealso cref="AudioSystem"/></code> class.
		''' <p>
		''' The <code>Encoding</code> class is static, so that all
		''' <code>AudioFormat</code> objects that have the same encoding will refer
		''' to the same object (rather than different instances of the same class).
		''' This allows matches to be made by checking that two format's encodings
		''' are equal.
		''' </summary>
		''' <seealso cref= AudioFormat </seealso>
		''' <seealso cref= javax.sound.sampled.spi.FormatConversionProvider
		''' 
		''' @author Kara Kytle
		''' @since 1.3 </seealso>
		Public Class Encoding


			' ENCODING DEFINES

			''' <summary>
			''' Specifies signed, linear PCM data.
			''' </summary>
			Public Shared ReadOnly PCM_SIGNED As New Encoding("PCM_SIGNED")

			''' <summary>
			''' Specifies unsigned, linear PCM data.
			''' </summary>
			Public Shared ReadOnly PCM_UNSIGNED As New Encoding("PCM_UNSIGNED")

			''' <summary>
			''' Specifies floating-point PCM data.
			''' 
			''' @since 1.7
			''' </summary>
			Public Shared ReadOnly PCM_FLOAT As New Encoding("PCM_FLOAT")

			''' <summary>
			''' Specifies u-law encoded data.
			''' </summary>
			Public Shared ReadOnly ULAW As New Encoding("ULAW")

			''' <summary>
			''' Specifies a-law encoded data.
			''' </summary>
			Public Shared ReadOnly ALAW As New Encoding("ALAW")


			' INSTANCE VARIABLES

			''' <summary>
			''' Encoding name.
			''' </summary>
			Private name As String


			' CONSTRUCTOR

			''' <summary>
			''' Constructs a new encoding. </summary>
			''' <param name="name">  the name of the new type of encoding </param>
			Public Sub New(ByVal name As String)
				Me.name = name
			End Sub


			' METHODS

			''' <summary>
			''' Finalizes the equals method
			''' </summary>
			Public NotOverridable Overrides Function Equals(ByVal obj As Object) As Boolean
				If ToString() Is Nothing Then Return (obj IsNot Nothing) AndAlso (obj.ToString() Is Nothing)
				If TypeOf obj Is Encoding Then Return ToString().Equals(obj.ToString())
				Return False
			End Function

			''' <summary>
			''' Finalizes the hashCode method
			''' </summary>
			Public NotOverridable Overrides Function GetHashCode() As Integer
				If ToString() Is Nothing Then Return 0
				Return ToString().GetHashCode()
			End Function

			''' <summary>
			''' Provides the <code>String</code> representation of the encoding.  This <code>String</code> is
			''' the same name that was passed to the constructor.  For the predefined encodings, the name
			''' is similar to the encoding's variable (field) name.  For example, <code>PCM_SIGNED.toString()</code> returns
			''' the name "pcm_signed".
			''' </summary>
			''' <returns> the encoding name </returns>
			Public NotOverridable Overrides Function ToString() As String
				Return name
			End Function

		End Class ' class Encoding
	End Class

End Namespace