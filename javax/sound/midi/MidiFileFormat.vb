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
	''' A <code>MidiFileFormat</code> object encapsulates a MIDI file's
	''' type, as well as its length and timing information.
	''' 
	''' <p>A <code>MidiFileFormat</code> object can
	''' include a set of properties. A property is a pair of key and value:
	''' the key is of type <code>String</code>, the associated property
	''' value is an arbitrary object.
	''' Properties specify additional informational
	''' meta data (like a author, or copyright).
	''' Properties are optional information, and file reader and file
	''' writer implementations are not required to provide or
	''' recognize properties.
	''' 
	''' <p>The following table lists some common properties that should
	''' be used in implementations:
	''' 
	''' <table border=1>
	'''    <caption>MIDI File Format Properties</caption>
	'''  <tr>
	'''   <th>Property key</th>
	'''   <th>Value type</th>
	'''   <th>Description</th>
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
	''' </summary>
	''' <seealso cref= MidiSystem#getMidiFileFormat(java.io.File) </seealso>
	''' <seealso cref= Sequencer#setSequence(java.io.InputStream stream)
	''' 
	''' @author Kara Kytle
	''' @author Florian Bomers </seealso>

	Public Class MidiFileFormat


		''' <summary>
		''' Represents unknown length. </summary>
		''' <seealso cref= #getByteLength </seealso>
		''' <seealso cref= #getMicrosecondLength </seealso>
		Public Const UNKNOWN_LENGTH As Integer = -1


		''' <summary>
		''' The type of MIDI file.
		''' </summary>
		Protected Friend type As Integer

		''' <summary>
		''' The division type of the MIDI file.
		''' </summary>
		''' <seealso cref= Sequence#PPQ </seealso>
		''' <seealso cref= Sequence#SMPTE_24 </seealso>
		''' <seealso cref= Sequence#SMPTE_25 </seealso>
		''' <seealso cref= Sequence#SMPTE_30DROP </seealso>
		''' <seealso cref= Sequence#SMPTE_30 </seealso>
		Protected Friend divisionType As Single

		''' <summary>
		''' The timing resolution of the MIDI file.
		''' </summary>
		Protected Friend resolution As Integer

		''' <summary>
		''' The length of the MIDI file in bytes.
		''' </summary>
		Protected Friend byteLength As Integer

		''' <summary>
		''' The duration of the MIDI file in microseconds.
		''' </summary>
		Protected Friend microsecondLength As Long


		''' <summary>
		''' The set of properties </summary>
		Private ___properties As Dictionary(Of String, Object)


		''' <summary>
		''' Constructs a <code>MidiFileFormat</code>.
		''' </summary>
		''' <param name="type"> the MIDI file type (0, 1, or 2) </param>
		''' <param name="divisionType"> the timing division type (PPQ or one of the SMPTE types) </param>
		''' <param name="resolution"> the timing resolution </param>
		''' <param name="bytes"> the length of the MIDI file in bytes, or UNKNOWN_LENGTH if not known </param>
		''' <param name="microseconds"> the duration of the file in microseconds, or UNKNOWN_LENGTH if not known </param>
		''' <seealso cref= #UNKNOWN_LENGTH </seealso>
		''' <seealso cref= Sequence#PPQ </seealso>
		''' <seealso cref= Sequence#SMPTE_24 </seealso>
		''' <seealso cref= Sequence#SMPTE_25 </seealso>
		''' <seealso cref= Sequence#SMPTE_30DROP </seealso>
		''' <seealso cref= Sequence#SMPTE_30 </seealso>
		Public Sub New(ByVal type As Integer, ByVal divisionType As Single, ByVal resolution As Integer, ByVal bytes As Integer, ByVal microseconds As Long)

			Me.type = type
			Me.divisionType = divisionType
			Me.resolution = resolution
			Me.byteLength = bytes
			Me.microsecondLength = microseconds
			Me.___properties = Nothing
		End Sub


		''' <summary>
		''' Construct a <code>MidiFileFormat</code> with a set of properties.
		''' </summary>
		''' <param name="type">         the MIDI file type (0, 1, or 2) </param>
		''' <param name="divisionType"> the timing division type
		'''      (PPQ or one of the SMPTE types) </param>
		''' <param name="resolution">   the timing resolution </param>
		''' <param name="bytes"> the length of the MIDI file in bytes,
		'''      or UNKNOWN_LENGTH if not known </param>
		''' <param name="microseconds"> the duration of the file in microseconds,
		'''      or UNKNOWN_LENGTH if not known </param>
		''' <param name="properties">  a <code>Map&lt;String,Object&gt;</code> object
		'''        with properties
		''' </param>
		''' <seealso cref= #UNKNOWN_LENGTH </seealso>
		''' <seealso cref= Sequence#PPQ </seealso>
		''' <seealso cref= Sequence#SMPTE_24 </seealso>
		''' <seealso cref= Sequence#SMPTE_25 </seealso>
		''' <seealso cref= Sequence#SMPTE_30DROP </seealso>
		''' <seealso cref= Sequence#SMPTE_30
		''' @since 1.5 </seealso>
		Public Sub New(ByVal type As Integer, ByVal divisionType As Single, ByVal resolution As Integer, ByVal bytes As Integer, ByVal microseconds As Long, ByVal properties As IDictionary(Of String, Object))
			Me.New(type, divisionType, resolution, bytes, microseconds)
			Me.___properties = New Dictionary(Of String, Object)(properties)
		End Sub



		''' <summary>
		''' Obtains the MIDI file type. </summary>
		''' <returns> the file's type (0, 1, or 2) </returns>
		Public Overridable Property type As Integer
			Get
				Return type
			End Get
		End Property

		''' <summary>
		''' Obtains the timing division type for the MIDI file.
		''' </summary>
		''' <returns> the division type (PPQ or one of the SMPTE types)
		''' </returns>
		''' <seealso cref= Sequence#Sequence(float, int) </seealso>
		''' <seealso cref= Sequence#PPQ </seealso>
		''' <seealso cref= Sequence#SMPTE_24 </seealso>
		''' <seealso cref= Sequence#SMPTE_25 </seealso>
		''' <seealso cref= Sequence#SMPTE_30DROP </seealso>
		''' <seealso cref= Sequence#SMPTE_30 </seealso>
		''' <seealso cref= Sequence#getDivisionType() </seealso>
		Public Overridable Property divisionType As Single
			Get
				Return divisionType
			End Get
		End Property


		''' <summary>
		''' Obtains the timing resolution for the MIDI file.
		''' If the division type is PPQ, the resolution is specified in ticks per beat.
		''' For SMTPE timing, the resolution is specified in ticks per frame.
		''' </summary>
		''' <returns> the number of ticks per beat (PPQ) or per frame (SMPTE) </returns>
		''' <seealso cref= #getDivisionType </seealso>
		''' <seealso cref= Sequence#getResolution() </seealso>
		Public Overridable Property resolution As Integer
			Get
				Return resolution
			End Get
		End Property


		''' <summary>
		''' Obtains the length of the MIDI file, expressed in 8-bit bytes. </summary>
		''' <returns> the number of bytes in the file, or UNKNOWN_LENGTH if not known </returns>
		''' <seealso cref= #UNKNOWN_LENGTH </seealso>
		Public Overridable Property byteLength As Integer
			Get
				Return byteLength
			End Get
		End Property

		''' <summary>
		''' Obtains the length of the MIDI file, expressed in microseconds. </summary>
		''' <returns> the file's duration in microseconds, or UNKNOWN_LENGTH if not known </returns>
		''' <seealso cref= Sequence#getMicrosecondLength() </seealso>
		''' <seealso cref= #getByteLength </seealso>
		''' <seealso cref= #UNKNOWN_LENGTH </seealso>
		Public Overridable Property microsecondLength As Long
			Get
				Return microsecondLength
			End Get
		End Property

		''' <summary>
		''' Obtain an unmodifiable map of properties.
		''' The concept of properties is further explained in
		''' the <seealso cref="MidiFileFormat class description"/>.
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
		''' the <seealso cref="MidiFileFormat class description"/>.
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


	End Class

End Namespace