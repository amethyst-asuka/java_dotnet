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

Namespace javax.sound.sampled.spi



	''' <summary>
	''' A format conversion provider provides format conversion services
	''' from one or more input formats to one or more output formats.
	''' Converters include codecs, which encode and/or decode audio data,
	''' as well as transcoders, etc.  Format converters provide methods for
	''' determining what conversions are supported and for obtaining an audio
	''' stream from which converted data can be read.
	''' <p>
	''' The source format represents the format of the incoming
	''' audio data, which will be converted.
	''' <p>
	''' The target format represents the format of the processed, converted
	''' audio data.  This is the format of the data that can be read from
	''' the stream returned by one of the <code>getAudioInputStream</code> methods.
	''' 
	''' @author Kara Kytle
	''' @since 1.3
	''' </summary>
	Public MustInherit Class FormatConversionProvider


		' NEW METHODS

		''' <summary>
		''' Obtains the set of source format encodings from which format
		''' conversion services are provided by this provider. </summary>
		''' <returns> array of source format encodings. If for some reason provider
		''' does not provide any conversion services, an array of length 0 is
		''' returned. </returns>
		Public MustOverride ReadOnly Property sourceEncodings As javax.sound.sampled.AudioFormat.Encoding()


		''' <summary>
		''' Obtains the set of target format encodings to which format
		''' conversion services are provided by this provider. </summary>
		''' <returns> array of target format encodings. If for some reason provider
		''' does not provide any conversion services, an array of length 0 is
		''' returned. </returns>
		Public MustOverride ReadOnly Property targetEncodings As javax.sound.sampled.AudioFormat.Encoding()


		''' <summary>
		''' Indicates whether the format converter supports conversion from the
		''' specified source format encoding. </summary>
		''' <param name="sourceEncoding"> the source format encoding for which support is queried </param>
		''' <returns> <code>true</code> if the encoding is supported, otherwise <code>false</code> </returns>
		Public Overridable Function isSourceEncodingSupported(ByVal sourceEncoding As javax.sound.sampled.AudioFormat.Encoding) As Boolean

			Dim ___sourceEncodings As javax.sound.sampled.AudioFormat.Encoding() = sourceEncodings

			For i As Integer = 0 To ___sourceEncodings.Length - 1
				If sourceEncoding.Equals(___sourceEncodings(i)) Then Return True
			Next i
			Return False
		End Function


		''' <summary>
		''' Indicates whether the format converter supports conversion to the
		''' specified target format encoding. </summary>
		''' <param name="targetEncoding"> the target format encoding for which support is queried </param>
		''' <returns> <code>true</code> if the encoding is supported, otherwise <code>false</code> </returns>
		Public Overridable Function isTargetEncodingSupported(ByVal targetEncoding As javax.sound.sampled.AudioFormat.Encoding) As Boolean

			Dim ___targetEncodings As javax.sound.sampled.AudioFormat.Encoding() = targetEncodings

			For i As Integer = 0 To ___targetEncodings.Length - 1
				If targetEncoding.Equals(___targetEncodings(i)) Then Return True
			Next i
			Return False
		End Function


		''' <summary>
		''' Obtains the set of target format encodings supported by the format converter
		''' given a particular source format.
		''' If no target format encodings are supported for this source format,
		''' an array of length 0 is returned. </summary>
		''' <param name="sourceFormat"> format of the incoming data </param>
		''' <returns> array of supported target format encodings. </returns>
		Public MustOverride Function getTargetEncodings(ByVal sourceFormat As javax.sound.sampled.AudioFormat) As javax.sound.sampled.AudioFormat.Encoding()


		''' <summary>
		''' Indicates whether the format converter supports conversion to a particular encoding
		''' from a particular format. </summary>
		''' <param name="targetEncoding"> desired encoding of the outgoing data </param>
		''' <param name="sourceFormat"> format of the incoming data </param>
		''' <returns> <code>true</code> if the conversion is supported, otherwise <code>false</code> </returns>
		Public Overridable Function isConversionSupported(ByVal targetEncoding As javax.sound.sampled.AudioFormat.Encoding, ByVal sourceFormat As javax.sound.sampled.AudioFormat) As Boolean

			Dim ___targetEncodings As javax.sound.sampled.AudioFormat.Encoding() = getTargetEncodings(sourceFormat)

			For i As Integer = 0 To ___targetEncodings.Length - 1
				If targetEncoding.Equals(___targetEncodings(i)) Then Return True
			Next i
			Return False
		End Function


		''' <summary>
		''' Obtains the set of target formats with the encoding specified
		''' supported by the format converter
		''' If no target formats with the specified encoding are supported
		''' for this source format, an array of length 0 is returned. </summary>
		''' <param name="targetEncoding"> desired encoding of the stream after processing </param>
		''' <param name="sourceFormat"> format of the incoming data </param>
		''' <returns> array of supported target formats. </returns>
		Public MustOverride Function getTargetFormats(ByVal targetEncoding As javax.sound.sampled.AudioFormat.Encoding, ByVal sourceFormat As javax.sound.sampled.AudioFormat) As javax.sound.sampled.AudioFormat()


		''' <summary>
		''' Indicates whether the format converter supports conversion to one
		''' particular format from another. </summary>
		''' <param name="targetFormat"> desired format of outgoing data </param>
		''' <param name="sourceFormat"> format of the incoming data </param>
		''' <returns> <code>true</code> if the conversion is supported, otherwise <code>false</code> </returns>
		Public Overridable Function isConversionSupported(ByVal targetFormat As javax.sound.sampled.AudioFormat, ByVal sourceFormat As javax.sound.sampled.AudioFormat) As Boolean

			Dim ___targetFormats As javax.sound.sampled.AudioFormat() = getTargetFormats(targetFormat.encoding, sourceFormat)

			For i As Integer = 0 To ___targetFormats.Length - 1
				If targetFormat.matches(___targetFormats(i)) Then Return True
			Next i
			Return False
		End Function


		''' <summary>
		''' Obtains an audio input stream with the specified encoding from the given audio
		''' input stream. </summary>
		''' <param name="targetEncoding"> desired encoding of the stream after processing </param>
		''' <param name="sourceStream"> stream from which data to be processed should be read </param>
		''' <returns> stream from which processed data with the specified target encoding may be read </returns>
		''' <exception cref="IllegalArgumentException"> if the format combination supplied is
		''' not supported. </exception>
		Public MustOverride Function getAudioInputStream(ByVal targetEncoding As javax.sound.sampled.AudioFormat.Encoding, ByVal sourceStream As javax.sound.sampled.AudioInputStream) As javax.sound.sampled.AudioInputStream


		''' <summary>
		''' Obtains an audio input stream with the specified format from the given audio
		''' input stream. </summary>
		''' <param name="targetFormat"> desired data format of the stream after processing </param>
		''' <param name="sourceStream"> stream from which data to be processed should be read </param>
		''' <returns> stream from which processed data with the specified format may be read </returns>
		''' <exception cref="IllegalArgumentException"> if the format combination supplied is
		''' not supported. </exception>
		Public MustOverride Function getAudioInputStream(ByVal targetFormat As javax.sound.sampled.AudioFormat, ByVal sourceStream As javax.sound.sampled.AudioInputStream) As javax.sound.sampled.AudioInputStream

	End Class

End Namespace