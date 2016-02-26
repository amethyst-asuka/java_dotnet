Imports System
Imports System.Collections.Generic
Imports System.Text

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
	''' An instance of the <code>AudioFileFormat</code> class describes
	''' an audio file, including the file type, the file's length in bytes,
	''' the length in sample frames of the audio data contained in the file,
	''' and the format of the audio data.
	''' <p>
	''' The <code><seealso cref="AudioSystem"/></code> class includes methods for determining the format
	''' of an audio file, obtaining an audio input stream from an audio file, and
	''' writing an audio file from an audio input stream.
	''' 
	''' <p>An <code>AudioFileFormat</code> object can
	''' include a set of properties. A property is a pair of key and value:
	''' the key is of type <code>String</code>, the associated property
	''' value is an arbitrary object.
	''' Properties specify additional informational
	''' meta data (like a author, copyright, or file duration).
	''' Properties are optional information, and file reader and file
	''' writer implementations are not required to provide or
	''' recognize properties.
	''' 
	''' <p>The following table lists some common properties that should
	''' be used in implementations:
	''' 
	''' <table border=1>
	'''  <caption>Audio File Format Properties</caption>
	'''  <tr>
	'''   <th>Property key</th>
	'''   <th>Value type</th>
	'''   <th>Description</th>
	'''  </tr>
	'''  <tr>
	'''   <td>&quot;duration&quot;</td>
	'''   <td><seealso cref="java.lang.Long Long"/></td>
	'''   <td>playback duration of the file in microseconds</td>
	'''  </tr>
	'''  <tr>
	'''   <td>&quot;author&quot;</td>
	'''   <td><seealso cref="java.lang.String String"/></td>
	'''   <td>name of the author of this file</td>
	'''  </tr>
	'''  <tr>
	'''   <td>&quot;title&quot;</td>
	'''   <td><seealso cref="java.lang.String String"/></td>
	'''   <td>title of this file</td>
	'''  </tr>
	'''  <tr>
	'''   <td>&quot;copyright&quot;</td>
	'''   <td><seealso cref="java.lang.String String"/></td>
	'''   <td>copyright message</td>
	'''  </tr>
	'''  <tr>
	'''   <td>&quot;date&quot;</td>
	'''   <td><seealso cref="java.util.Date Date"/></td>
	'''   <td>date of the recording or release</td>
	'''  </tr>
	'''  <tr>
	'''   <td>&quot;comment&quot;</td>
	'''   <td><seealso cref="java.lang.String String"/></td>
	'''   <td>an arbitrary text</td>
	'''  </tr>
	''' </table>
	''' 
	''' 
	''' @author David Rivas
	''' @author Kara Kytle
	''' @author Florian Bomers </summary>
	''' <seealso cref= AudioInputStream
	''' @since 1.3 </seealso>
	Public Class AudioFileFormat


		' INSTANCE VARIABLES


		''' <summary>
		''' File type.
		''' </summary>
		Private type As Type

		''' <summary>
		''' File length in bytes
		''' </summary>
		Private byteLength As Integer

		''' <summary>
		''' Format of the audio data contained in the file.
		''' </summary>
		Private format As AudioFormat

		''' <summary>
		''' Audio data length in sample frames
		''' </summary>
		Private frameLength As Integer


		''' <summary>
		''' The set of properties </summary>
		Private ___properties As Dictionary(Of String, Object)


		''' <summary>
		''' Constructs an audio file format object.
		''' This protected constructor is intended for use by providers of file-reading
		''' services when returning information about an audio file or about supported audio file
		''' formats. </summary>
		''' <param name="type"> the type of the audio file </param>
		''' <param name="byteLength"> the length of the file in bytes, or <code>AudioSystem.NOT_SPECIFIED</code> </param>
		''' <param name="format"> the format of the audio data contained in the file </param>
		''' <param name="frameLength"> the audio data length in sample frames, or <code>AudioSystem.NOT_SPECIFIED</code>
		''' </param>
		''' <seealso cref= #getType </seealso>
		Protected Friend Sub New(ByVal ___type As Type, ByVal byteLength As Integer, ByVal format As AudioFormat, ByVal frameLength As Integer)

			Me.type = ___type
			Me.byteLength = byteLength
			Me.format = format
			Me.frameLength = frameLength
			Me.___properties = Nothing
		End Sub


		''' <summary>
		''' Constructs an audio file format object.
		''' This public constructor may be used by applications to describe the
		''' properties of a requested audio file. </summary>
		''' <param name="type"> the type of the audio file </param>
		''' <param name="format"> the format of the audio data contained in the file </param>
		''' <param name="frameLength"> the audio data length in sample frames, or <code>AudioSystem.NOT_SPECIFIED</code> </param>
		Public Sub New(ByVal ___type As Type, ByVal format As AudioFormat, ByVal frameLength As Integer)


			Me.New(___type,AudioSystem.NOT_SPECIFIED,format,frameLength)
		End Sub

		''' <summary>
		''' Construct an audio file format object with a set of
		''' defined properties.
		''' This public constructor may be used by applications to describe the
		''' properties of a requested audio file. The properties map
		''' will be copied to prevent any changes to it.
		''' </summary>
		''' <param name="type">        the type of the audio file </param>
		''' <param name="format">      the format of the audio data contained in the file </param>
		''' <param name="frameLength"> the audio data length in sample frames, or
		'''                    <code>AudioSystem.NOT_SPECIFIED</code> </param>
		''' <param name="properties">  a <code>Map&lt;String,Object&gt;</code> object
		'''        with properties
		''' 
		''' @since 1.5 </param>
		Public Sub New(ByVal ___type As Type, ByVal format As AudioFormat, ByVal frameLength As Integer, ByVal properties As IDictionary(Of String, Object))
			Me.New(___type,AudioSystem.NOT_SPECIFIED,format,frameLength)
			Me.___properties = New Dictionary(Of String, Object)(properties)
		End Sub


		''' <summary>
		''' Obtains the audio file type, such as <code>WAVE</code> or <code>AU</code>. </summary>
		''' <returns> the audio file type
		''' </returns>
		''' <seealso cref= Type#WAVE </seealso>
		''' <seealso cref= Type#AU </seealso>
		''' <seealso cref= Type#AIFF </seealso>
		''' <seealso cref= Type#AIFC </seealso>
		''' <seealso cref= Type#SND </seealso>
		Public Overridable Property type As Type
			Get
				Return type
			End Get
		End Property

		''' <summary>
		''' Obtains the size in bytes of the entire audio file (not just its audio data). </summary>
		''' <returns> the audio file length in bytes </returns>
		''' <seealso cref= AudioSystem#NOT_SPECIFIED </seealso>
		Public Overridable Property byteLength As Integer
			Get
				Return byteLength
			End Get
		End Property

		''' <summary>
		''' Obtains the format of the audio data contained in the audio file. </summary>
		''' <returns> the audio data format </returns>
		Public Overridable Property format As AudioFormat
			Get
				Return format
			End Get
		End Property

		''' <summary>
		''' Obtains the length of the audio data contained in the file, expressed in sample frames. </summary>
		''' <returns> the number of sample frames of audio data in the file </returns>
		''' <seealso cref= AudioSystem#NOT_SPECIFIED </seealso>
		Public Overridable Property frameLength As Integer
			Get
				Return frameLength
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
		''' Provides a string representation of the file format. </summary>
		''' <returns> the file format as a string </returns>
		Public Overrides Function ToString() As String

			Dim buf As New StringBuilder

			'$$fb2002-11-01: fix for 4672864: AudioFileFormat.toString() throws unexpected NullPointerException
			If type IsNot Nothing Then
				buf.Append(type.ToString() & " (." & type.extension & ") file")
			Else
				buf.Append("unknown file format")
			End If

			If byteLength <> AudioSystem.NOT_SPECIFIED Then buf.Append(", byte length: " & byteLength)

			buf.Append(", data format: " & format)

			If frameLength <> AudioSystem.NOT_SPECIFIED Then buf.Append(", frame length: " & frameLength)

			Return New String(buf)
		End Function


		''' <summary>
		''' An instance of the <code>Type</code> class represents one of the
		''' standard types of audio file.  Static instances are provided for the
		''' common types.
		''' </summary>
		Public Class Type

			' FILE FORMAT TYPE DEFINES

			''' <summary>
			''' Specifies a WAVE file.
			''' </summary>
			Public Shared ReadOnly WAVE As New Type("WAVE", "wav")

			''' <summary>
			''' Specifies an AU file.
			''' </summary>
			Public Shared ReadOnly AU As New Type("AU", "au")

			''' <summary>
			''' Specifies an AIFF file.
			''' </summary>
			Public Shared ReadOnly AIFF As New Type("AIFF", "aif")

			''' <summary>
			''' Specifies an AIFF-C file.
			''' </summary>
			Public Shared ReadOnly AIFC As New Type("AIFF-C", "aifc")

			''' <summary>
			''' Specifies a SND file.
			''' </summary>
			Public Shared ReadOnly SND As New Type("SND", "snd")


			' INSTANCE VARIABLES

			''' <summary>
			''' File type name.
			''' </summary>
			Private ReadOnly name As String

			''' <summary>
			''' File type extension.
			''' </summary>
			Private ReadOnly extension As String


			' CONSTRUCTOR

			''' <summary>
			''' Constructs a file type. </summary>
			''' <param name="name"> the string that names the file type </param>
			''' <param name="extension"> the string that commonly marks the file type
			''' without leading dot. </param>
			Public Sub New(ByVal name As String, ByVal extension As String)

				Me.name = name
				Me.extension = extension
			End Sub


			' METHODS

			''' <summary>
			''' Finalizes the equals method
			''' </summary>
			Public NotOverridable Overrides Function Equals(ByVal obj As Object) As Boolean
				If ToString() Is Nothing Then Return (obj IsNot Nothing) AndAlso (obj.ToString() Is Nothing)
				If TypeOf obj Is Type Then Return ToString().Equals(obj.ToString())
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
			''' Provides the file type's name as the <code>String</code> representation
			''' of the file type. </summary>
			''' <returns> the file type's name </returns>
			Public NotOverridable Overrides Function ToString() As String
				Return name
			End Function

			''' <summary>
			''' Obtains the common file name extension for this file type. </summary>
			''' <returns> file type extension </returns>
			Public Overridable Property extension As String
				Get
					Return extension
				End Get
			End Property

		End Class ' class Type

	End Class ' class AudioFileFormat

End Namespace