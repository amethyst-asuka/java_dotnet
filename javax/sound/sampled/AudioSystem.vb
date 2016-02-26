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

Namespace javax.sound.sampled





	' $fb TODO:
	' * - consistent usage of (typed) collections
	' 


	''' <summary>
	''' The <code>AudioSystem</code> class acts as the entry point to the
	''' sampled-audio system resources. This class lets you query and
	''' access the mixers that are installed on the system.
	''' <code>AudioSystem</code> includes a number of
	''' methods for converting audio data between different formats, and for
	''' translating between audio files and streams. It also provides a method
	''' for obtaining a <code><seealso cref="Line"/></code> directly from the
	''' <code>AudioSystem</code> without dealing explicitly
	''' with mixers.
	''' 
	''' <p>Properties can be used to specify the default mixer
	''' for specific line types.
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
	'''  <caption>Audio System Property Keys</caption>
	'''  <tr>
	'''   <th>Property Key</th>
	'''   <th>Interface</th>
	'''   <th>Affected Method(s)</th>
	'''  </tr>
	'''  <tr>
	'''   <td><code>javax.sound.sampled.Clip</code></td>
	'''   <td><seealso cref="Clip"/></td>
	'''   <td><seealso cref="#getLine"/>, <seealso cref="#getClip"/></td>
	'''  </tr>
	'''  <tr>
	'''   <td><code>javax.sound.sampled.Port</code></td>
	'''   <td><seealso cref="Port"/></td>
	'''   <td><seealso cref="#getLine"/></td>
	'''  </tr>
	'''  <tr>
	'''   <td><code>javax.sound.sampled.SourceDataLine</code></td>
	'''   <td><seealso cref="SourceDataLine"/></td>
	'''   <td><seealso cref="#getLine"/>, <seealso cref="#getSourceDataLine"/></td>
	'''  </tr>
	'''  <tr>
	'''   <td><code>javax.sound.sampled.TargetDataLine</code></td>
	'''   <td><seealso cref="TargetDataLine"/></td>
	'''   <td><seealso cref="#getLine"/>, <seealso cref="#getTargetDataLine"/></td>
	'''  </tr>
	''' </table>
	''' 
	''' The property value consists of the provider class name
	''' and the mixer name, separated by the hash mark (&quot;#&quot;).
	''' The provider class name is the fully-qualified
	''' name of a concrete {@link javax.sound.sampled.spi.MixerProvider
	''' mixer provider} class. The mixer name is matched against
	''' the <code>String</code> returned by the <code>getName</code>
	''' method of <code>Mixer.Info</code>.
	''' Either the class name, or the mixer name may be omitted.
	''' If only the class name is specified, the trailing hash mark
	''' is optional.
	''' 
	''' <p>If the provider class is specified, and it can be
	''' successfully retrieved from the installed providers, the list of
	''' <code>Mixer.Info</code> objects is retrieved
	''' from the provider. Otherwise, or when these mixers
	''' do not provide a subsequent match, the list is retrieved
	''' from <seealso cref="#getMixerInfo"/> to contain
	''' all available <code>Mixer.Info</code> objects.
	''' 
	''' <p>If a mixer name is specified, the resulting list of
	''' <code>Mixer.Info</code> objects is searched:
	''' the first one with a matching name, and whose
	''' <code>Mixer</code> provides the
	''' respective line interface, will be returned.
	''' If no matching <code>Mixer.Info</code> object
	''' is found, or the mixer name is not specified,
	''' the first mixer from the resulting
	''' list, which provides the respective line
	''' interface, will be returned.
	''' 
	''' For example, the property <code>javax.sound.sampled.Clip</code>
	''' with a value
	''' <code>&quot;com.sun.media.sound.MixerProvider#SunClip&quot;</code>
	''' will have the following consequences when
	''' <code>getLine</code> is called requesting a <code>Clip</code>
	''' instance:
	''' if the class <code>com.sun.media.sound.MixerProvider</code> exists
	''' in the list of installed mixer providers,
	''' the first <code>Clip</code> from the first mixer with name
	''' <code>&quot;SunClip&quot;</code> will be returned. If it cannot
	''' be found, the first <code>Clip</code> from the first mixer
	''' of the specified provider will be returned, regardless of name.
	''' If there is none, the first <code>Clip</code> from the first
	''' <code>Mixer</code> with name
	''' <code>&quot;SunClip&quot;</code> in the list of all mixers
	''' (as returned by <code>getMixerInfo</code>) will be returned,
	''' or, if not found, the first <code>Clip</code> of the first
	''' <code>Mixer</code>that can be found in the list of all
	''' mixers is returned.
	''' If that fails, too, an <code>IllegalArgumentException</code>
	''' is thrown.
	''' 
	''' @author Kara Kytle
	''' @author Florian Bomers
	''' @author Matthias Pfisterer
	''' @author Kevin P. Smith
	''' </summary>
	''' <seealso cref= AudioFormat </seealso>
	''' <seealso cref= AudioInputStream </seealso>
	''' <seealso cref= Mixer </seealso>
	''' <seealso cref= Line </seealso>
	''' <seealso cref= Line.Info
	''' @since 1.3 </seealso>
	Public Class AudioSystem

		''' <summary>
		''' An integer that stands for an unknown numeric value.
		''' This value is appropriate only for signed quantities that do not
		''' normally take negative values.  Examples include file sizes, frame
		''' sizes, buffer sizes, and sample rates.
		''' A number of Java Sound constructors accept
		''' a value of <code>NOT_SPECIFIED</code> for such parameters.  Other
		''' methods may also accept or return this value, as documented.
		''' </summary>
		Public Const NOT_SPECIFIED As Integer = -1

		''' <summary>
		''' Private no-args constructor for ensuring against instantiation.
		''' </summary>
		Private Sub New()
		End Sub


		''' <summary>
		''' Obtains an array of mixer info objects that represents
		''' the set of audio mixers that are currently installed on the system. </summary>
		''' <returns> an array of info objects for the currently installed mixers.  If no mixers
		''' are available on the system, an array of length 0 is returned. </returns>
		''' <seealso cref= #getMixer </seealso>
		Public Property Shared mixerInfo As Mixer.Info()
			Get
    
				Dim infos As IList = mixerInfoList
				Dim allInfos As Mixer.Info() = CType(infos.ToArray(GetType(Mixer.Info)), Mixer.Info())
				Return allInfos
			End Get
		End Property


		''' <summary>
		''' Obtains the requested audio mixer. </summary>
		''' <param name="info"> a <code>Mixer.Info</code> object representing the desired
		''' mixer, or <code>null</code> for the system default mixer </param>
		''' <returns> the requested mixer </returns>
		''' <exception cref="SecurityException"> if the requested mixer
		''' is unavailable because of security restrictions </exception>
		''' <exception cref="IllegalArgumentException"> if the info object does not represent
		''' a mixer installed on the system </exception>
		''' <seealso cref= #getMixerInfo </seealso>
		Public Shared Function getMixer(ByVal info As Mixer.Info) As Mixer

			Dim ___mixer As Mixer = Nothing
			Dim ___providers As IList = mixerProviders

			For i As Integer = 0 To ___providers.Count - 1

				Try
					Return CType(___providers(i), javax.sound.sampled.spi.MixerProvider).getMixer(info)

				Catch e As System.ArgumentException
				Catch e As NullPointerException
					' $$jb 08.20.99:  If the strings in the info object aren't
					' set, then Netscape (using jdk1.1.5) tends to throw
					' NPE's when doing some string manipulation.  This is
					' probably not the best fix, but is solves the problem
					' of the NPE in Netscape using local classes
					' $$jb 11.01.99: Replacing this patch.
				End Try
			Next i

			'$$fb if looking for default mixer, and not found yet, add a round of looking
			If info Is Nothing Then
				For i As Integer = 0 To ___providers.Count - 1
					Try
						Dim provider As javax.sound.sampled.spi.MixerProvider = CType(___providers(i), javax.sound.sampled.spi.MixerProvider)
						Dim infos As Mixer.Info() = provider.mixerInfo
						' start from 0 to last device (do not reverse this order)
						For ii As Integer = 0 To infos.Length - 1
							Try
								Return provider.getMixer(infos(ii))
							Catch e As System.ArgumentException
								' this is not a good default device :)
							End Try
						Next ii
					Catch e As System.ArgumentException
					Catch e As NullPointerException
					End Try
				Next i
			End If


			Throw New System.ArgumentException("Mixer not supported: " & (If(info IsNot Nothing, info.ToString(), "null")))
		End Function


		'$$fb 2002-11-26: fix for 4757930: DOC: AudioSystem.getTarget/SourceLineInfo() is ambiguous
		''' <summary>
		''' Obtains information about all source lines of a particular type that are supported
		''' by the installed mixers. </summary>
		''' <param name="info"> a <code>Line.Info</code> object that specifies the kind of
		''' lines about which information is requested </param>
		''' <returns> an array of <code>Line.Info</code> objects describing source lines matching
		''' the type requested.  If no matching source lines are supported, an array of length 0
		''' is returned.
		''' </returns>
		''' <seealso cref= Mixer#getSourceLineInfo(Line.Info) </seealso>
		Public Shared Function getSourceLineInfo(ByVal info As Line.Info) As Line.Info()

			Dim vector As New ArrayList
			Dim currentInfoArray As Line.Info()

			Dim ___mixer As Mixer
			Dim fullInfo As Line.Info = Nothing
			Dim infoArray As Mixer.Info() = mixerInfo

			For i As Integer = 0 To infoArray.Length - 1

				___mixer = getMixer(infoArray(i))

				currentInfoArray = ___mixer.getSourceLineInfo(info)
				For j As Integer = 0 To currentInfoArray.Length - 1
					vector.Add(currentInfoArray(j))
				Next j
			Next i

			Dim returnedArray As Line.Info() = New Line.Info(vector.Count - 1){}

			For i As Integer = 0 To returnedArray.Length - 1
				returnedArray(i) = CType(vector(i), Line.Info)
			Next i

			Return returnedArray
		End Function


		''' <summary>
		''' Obtains information about all target lines of a particular type that are supported
		''' by the installed mixers. </summary>
		''' <param name="info"> a <code>Line.Info</code> object that specifies the kind of
		''' lines about which information is requested </param>
		''' <returns> an array of <code>Line.Info</code> objects describing target lines matching
		''' the type requested.  If no matching target lines are supported, an array of length 0
		''' is returned.
		''' </returns>
		''' <seealso cref= Mixer#getTargetLineInfo(Line.Info) </seealso>
		Public Shared Function getTargetLineInfo(ByVal info As Line.Info) As Line.Info()

			Dim vector As New ArrayList
			Dim currentInfoArray As Line.Info()

			Dim ___mixer As Mixer
			Dim fullInfo As Line.Info = Nothing
			Dim infoArray As Mixer.Info() = mixerInfo

			For i As Integer = 0 To infoArray.Length - 1

				___mixer = getMixer(infoArray(i))

				currentInfoArray = ___mixer.getTargetLineInfo(info)
				For j As Integer = 0 To currentInfoArray.Length - 1
					vector.Add(currentInfoArray(j))
				Next j
			Next i

			Dim returnedArray As Line.Info() = New Line.Info(vector.Count - 1){}

			For i As Integer = 0 To returnedArray.Length - 1
				returnedArray(i) = CType(vector(i), Line.Info)
			Next i

			Return returnedArray
		End Function


		''' <summary>
		''' Indicates whether the system supports any lines that match
		''' the specified <code>Line.Info</code> object.  A line is supported if
		''' any installed mixer supports it. </summary>
		''' <param name="info"> a <code>Line.Info</code> object describing the line for which support is queried </param>
		''' <returns> <code>true</code> if at least one matching line is
		''' supported, otherwise <code>false</code>
		''' </returns>
		''' <seealso cref= Mixer#isLineSupported(Line.Info) </seealso>
		Public Shared Function isLineSupported(ByVal info As Line.Info) As Boolean

			Dim ___mixer As Mixer
			Dim infoArray As Mixer.Info() = mixerInfo

			For i As Integer = 0 To infoArray.Length - 1

				If infoArray(i) IsNot Nothing Then
					___mixer = getMixer(infoArray(i))
					If ___mixer.isLineSupported(info) Then Return True
				End If
			Next i

			Return False
		End Function

		''' <summary>
		''' Obtains a line that matches the description in the specified
		''' <code>Line.Info</code> object.
		''' 
		''' <p>If a <code>DataLine</code> is requested, and <code>info</code>
		''' is an instance of <code>DataLine.Info</code> specifying at least
		''' one fully qualified audio format, the last one
		''' will be used as the default format of the returned
		''' <code>DataLine</code>.
		''' 
		''' <p>If system properties
		''' <code>javax.sound.sampled.Clip</code>,
		''' <code>javax.sound.sampled.Port</code>,
		''' <code>javax.sound.sampled.SourceDataLine</code> and
		''' <code>javax.sound.sampled.TargetDataLine</code> are defined
		''' or they are defined in the file &quot;sound.properties&quot;,
		''' they are used to retrieve default lines.
		''' For details, refer to the <seealso cref="AudioSystem class description"/>.
		''' 
		''' If the respective property is not set, or the mixer
		''' requested in the property is not installed or does not provide the
		''' requested line, all installed mixers are queried for the
		''' requested line type. A Line will be returned from the first mixer
		''' providing the requested line type.
		''' </summary>
		''' <param name="info"> a <code>Line.Info</code> object describing the desired kind of line </param>
		''' <returns> a line of the requested kind
		''' </returns>
		''' <exception cref="LineUnavailableException"> if a matching line
		''' is not available due to resource restrictions </exception>
		''' <exception cref="SecurityException"> if a matching line
		''' is not available due to security restrictions </exception>
		''' <exception cref="IllegalArgumentException"> if the system does not
		''' support at least one line matching the specified
		''' <code>Line.Info</code> object
		''' through any installed mixer </exception>
		Public Shared Function getLine(ByVal info As Line.Info) As Line
			Dim lue As LineUnavailableException = Nothing
			Dim ___providers As IList = mixerProviders


			' 1: try from default mixer for this line class
			Try
				Dim ___mixer As Mixer = getDefaultMixer(___providers, info)
				If ___mixer IsNot Nothing AndAlso ___mixer.isLineSupported(info) Then Return ___mixer.getLine(info)
			Catch e As LineUnavailableException
				lue = e
			Catch iae As System.ArgumentException
				' must not happen... but better to catch it here,
				' if plug-ins are badly written
			End Try


			' 2: if that doesn't work, try to find any mixing mixer
			For i As Integer = 0 To ___providers.Count - 1
				Dim provider As javax.sound.sampled.spi.MixerProvider = CType(___providers(i), javax.sound.sampled.spi.MixerProvider)
				Dim infos As Mixer.Info() = provider.mixerInfo

				For j As Integer = 0 To infos.Length - 1
					Try
						Dim ___mixer As Mixer = provider.getMixer(infos(j))
						' see if this is an appropriate mixer which can mix
						If isAppropriateMixer(___mixer, info, True) Then Return ___mixer.getLine(info)
					Catch e As LineUnavailableException
						lue = e
					Catch iae As System.ArgumentException
						' must not happen... but better to catch it here,
						' if plug-ins are badly written
					End Try
				Next j
			Next i


			' 3: if that didn't work, try to find any non-mixing mixer
			For i As Integer = 0 To ___providers.Count - 1
				Dim provider As javax.sound.sampled.spi.MixerProvider = CType(___providers(i), javax.sound.sampled.spi.MixerProvider)
				Dim infos As Mixer.Info() = provider.mixerInfo
				For j As Integer = 0 To infos.Length - 1
					Try
						Dim ___mixer As Mixer = provider.getMixer(infos(j))
						' see if this is an appropriate mixer which can mix
						If isAppropriateMixer(___mixer, info, False) Then Return ___mixer.getLine(info)
					Catch e As LineUnavailableException
						lue = e
					Catch iae As System.ArgumentException
						' must not happen... but better to catch it here,
						' if plug-ins are badly written
					End Try
				Next j
			Next i

			' if this line was supported but was not available, throw the last
			' LineUnavailableException we got (??).
			If lue IsNot Nothing Then Throw lue

			' otherwise, the requested line was not supported, so throw
			' an Illegal argument exception
			Throw New System.ArgumentException("No line matching " & info.ToString() & " is supported.")
		End Function


		''' <summary>
		''' Obtains a clip that can be used for playing back
		''' an audio file or an audio stream. The returned clip
		''' will be provided by the default system mixer, or,
		''' if not possible, by any other mixer installed in the
		''' system that supports a <code>Clip</code>
		''' object.
		''' 
		''' <p>The returned clip must be opened with the
		''' <code>open(AudioFormat)</code> or
		''' <code>open(AudioInputStream)</code> method.
		''' 
		''' <p>This is a high-level method that uses <code>getMixer</code>
		''' and <code>getLine</code> internally.
		''' 
		''' <p>If the system property
		''' <code>javax.sound.sampled.Clip</code>
		''' is defined or it is defined in the file &quot;sound.properties&quot;,
		''' it is used to retrieve the default clip.
		''' For details, refer to the <seealso cref="AudioSystem class description"/>.
		''' </summary>
		''' <returns> the desired clip object
		''' </returns>
		''' <exception cref="LineUnavailableException"> if a clip object
		''' is not available due to resource restrictions </exception>
		''' <exception cref="SecurityException"> if a clip object
		''' is not available due to security restrictions </exception>
		''' <exception cref="IllegalArgumentException"> if the system does not
		''' support at least one clip instance through any installed mixer
		''' </exception>
		''' <seealso cref= #getClip(Mixer.Info)
		''' @since 1.5 </seealso>
		Public Property Shared clip As Clip
			Get
				Dim format As New AudioFormat(AudioFormat.Encoding.PCM_SIGNED, AudioSystem.NOT_SPECIFIED, 16, 2, 4, AudioSystem.NOT_SPECIFIED, True)
				Dim info As New DataLine.Info(GetType(Clip), format)
				Return CType(AudioSystem.getLine(info), Clip)
			End Get
		End Property


		''' <summary>
		''' Obtains a clip from the specified mixer that can be
		''' used for playing back an audio file or an audio stream.
		''' 
		''' <p>The returned clip must be opened with the
		''' <code>open(AudioFormat)</code> or
		''' <code>open(AudioInputStream)</code> method.
		''' 
		''' <p>This is a high-level method that uses <code>getMixer</code>
		''' and <code>getLine</code> internally.
		''' </summary>
		''' <param name="mixerInfo"> a <code>Mixer.Info</code> object representing the
		''' desired mixer, or <code>null</code> for the system default mixer </param>
		''' <returns> a clip object from the specified mixer
		''' </returns>
		''' <exception cref="LineUnavailableException"> if a clip
		''' is not available from this mixer due to resource restrictions </exception>
		''' <exception cref="SecurityException"> if a clip
		''' is not available from this mixer due to security restrictions </exception>
		''' <exception cref="IllegalArgumentException"> if the system does not
		''' support at least one clip through the specified mixer
		''' </exception>
		''' <seealso cref= #getClip()
		''' @since 1.5 </seealso>
		Public Shared Function getClip(ByVal mixerInfo As Mixer.Info) As Clip
			Dim format As New AudioFormat(AudioFormat.Encoding.PCM_SIGNED, AudioSystem.NOT_SPECIFIED, 16, 2, 4, AudioSystem.NOT_SPECIFIED, True)
			Dim info As New DataLine.Info(GetType(Clip), format)
			Dim ___mixer As Mixer = AudioSystem.getMixer(mixerInfo)
			Return CType(___mixer.getLine(info), Clip)
		End Function


		''' <summary>
		''' Obtains a source data line that can be used for playing back
		''' audio data in the format specified by the
		''' <code>AudioFormat</code> object. The returned line
		''' will be provided by the default system mixer, or,
		''' if not possible, by any other mixer installed in the
		''' system that supports a matching
		''' <code>SourceDataLine</code> object.
		''' 
		''' <p>The returned line should be opened with the
		''' <code>open(AudioFormat)</code> or
		''' <code>open(AudioFormat, int)</code> method.
		''' 
		''' <p>This is a high-level method that uses <code>getMixer</code>
		''' and <code>getLine</code> internally.
		''' 
		''' <p>The returned <code>SourceDataLine</code>'s default
		''' audio format will be initialized with <code>format</code>.
		''' 
		''' <p>If the system property
		''' <code>javax.sound.sampled.SourceDataLine</code>
		''' is defined or it is defined in the file &quot;sound.properties&quot;,
		''' it is used to retrieve the default source data line.
		''' For details, refer to the <seealso cref="AudioSystem class description"/>.
		''' </summary>
		''' <param name="format"> an <code>AudioFormat</code> object specifying
		'''        the supported audio format of the returned line,
		'''        or <code>null</code> for any audio format </param>
		''' <returns> the desired <code>SourceDataLine</code> object
		''' </returns>
		''' <exception cref="LineUnavailableException"> if a matching source data line
		'''         is not available due to resource restrictions </exception>
		''' <exception cref="SecurityException"> if a matching source data line
		'''         is not available due to security restrictions </exception>
		''' <exception cref="IllegalArgumentException"> if the system does not
		'''         support at least one source data line supporting the
		'''         specified audio format through any installed mixer
		''' </exception>
		''' <seealso cref= #getSourceDataLine(AudioFormat, Mixer.Info)
		''' @since 1.5 </seealso>
		Public Shared Function getSourceDataLine(ByVal format As AudioFormat) As SourceDataLine
			Dim info As New DataLine.Info(GetType(SourceDataLine), format)
			Return CType(AudioSystem.getLine(info), SourceDataLine)
		End Function


		''' <summary>
		''' Obtains a source data line that can be used for playing back
		''' audio data in the format specified by the
		''' <code>AudioFormat</code> object, provided by the mixer
		''' specified by the <code>Mixer.Info</code> object.
		''' 
		''' <p>The returned line should be opened with the
		''' <code>open(AudioFormat)</code> or
		''' <code>open(AudioFormat, int)</code> method.
		''' 
		''' <p>This is a high-level method that uses <code>getMixer</code>
		''' and <code>getLine</code> internally.
		''' 
		''' <p>The returned <code>SourceDataLine</code>'s default
		''' audio format will be initialized with <code>format</code>.
		''' </summary>
		''' <param name="format"> an <code>AudioFormat</code> object specifying
		'''        the supported audio format of the returned line,
		'''        or <code>null</code> for any audio format </param>
		''' <param name="mixerinfo"> a <code>Mixer.Info</code> object representing
		'''        the desired mixer, or <code>null</code> for the system
		'''        default mixer </param>
		''' <returns> the desired <code>SourceDataLine</code> object
		''' </returns>
		''' <exception cref="LineUnavailableException"> if a matching source data
		'''         line is not available from the specified mixer due
		'''         to resource restrictions </exception>
		''' <exception cref="SecurityException"> if a matching source data line
		'''         is not available from the specified mixer due to
		'''         security restrictions </exception>
		''' <exception cref="IllegalArgumentException"> if the specified mixer does
		'''         not support at least one source data line supporting
		'''         the specified audio format
		''' </exception>
		''' <seealso cref= #getSourceDataLine(AudioFormat)
		''' @since 1.5 </seealso>
		Public Shared Function getSourceDataLine(ByVal format As AudioFormat, ByVal mixerinfo As Mixer.Info) As SourceDataLine
			Dim info As New DataLine.Info(GetType(SourceDataLine), format)
			Dim ___mixer As Mixer = AudioSystem.getMixer(mixerinfo)
			Return CType(___mixer.getLine(info), SourceDataLine)
		End Function


		''' <summary>
		''' Obtains a target data line that can be used for recording
		''' audio data in the format specified by the
		''' <code>AudioFormat</code> object. The returned line
		''' will be provided by the default system mixer, or,
		''' if not possible, by any other mixer installed in the
		''' system that supports a matching
		''' <code>TargetDataLine</code> object.
		''' 
		''' <p>The returned line should be opened with the
		''' <code>open(AudioFormat)</code> or
		''' <code>open(AudioFormat, int)</code> method.
		''' 
		''' <p>This is a high-level method that uses <code>getMixer</code>
		''' and <code>getLine</code> internally.
		''' 
		''' <p>The returned <code>TargetDataLine</code>'s default
		''' audio format will be initialized with <code>format</code>.
		''' 
		''' <p>If the system property
		''' {@code javax.sound.sampled.TargetDataLine}
		''' is defined or it is defined in the file &quot;sound.properties&quot;,
		''' it is used to retrieve the default target data line.
		''' For details, refer to the <seealso cref="AudioSystem class description"/>.
		''' </summary>
		''' <param name="format"> an <code>AudioFormat</code> object specifying
		'''        the supported audio format of the returned line,
		'''        or <code>null</code> for any audio format </param>
		''' <returns> the desired <code>TargetDataLine</code> object
		''' </returns>
		''' <exception cref="LineUnavailableException"> if a matching target data line
		'''         is not available due to resource restrictions </exception>
		''' <exception cref="SecurityException"> if a matching target data line
		'''         is not available due to security restrictions </exception>
		''' <exception cref="IllegalArgumentException"> if the system does not
		'''         support at least one target data line supporting the
		'''         specified audio format through any installed mixer
		''' </exception>
		''' <seealso cref= #getTargetDataLine(AudioFormat, Mixer.Info) </seealso>
		''' <seealso cref= AudioPermission
		''' @since 1.5 </seealso>
		Public Shared Function getTargetDataLine(ByVal format As AudioFormat) As TargetDataLine

			Dim info As New DataLine.Info(GetType(TargetDataLine), format)
			Return CType(AudioSystem.getLine(info), TargetDataLine)
		End Function



		''' <summary>
		''' Obtains a target data line that can be used for recording
		''' audio data in the format specified by the
		''' <code>AudioFormat</code> object, provided by the mixer
		''' specified by the <code>Mixer.Info</code> object.
		''' 
		''' <p>The returned line should be opened with the
		''' <code>open(AudioFormat)</code> or
		''' <code>open(AudioFormat, int)</code> method.
		''' 
		''' <p>This is a high-level method that uses <code>getMixer</code>
		''' and <code>getLine</code> internally.
		''' 
		''' <p>The returned <code>TargetDataLine</code>'s default
		''' audio format will be initialized with <code>format</code>.
		''' </summary>
		''' <param name="format"> an <code>AudioFormat</code> object specifying
		'''        the supported audio format of the returned line,
		'''        or <code>null</code> for any audio format </param>
		''' <param name="mixerinfo"> a <code>Mixer.Info</code> object representing the
		'''        desired mixer, or <code>null</code> for the system default mixer </param>
		''' <returns> the desired <code>TargetDataLine</code> object
		''' </returns>
		''' <exception cref="LineUnavailableException"> if a matching target data
		'''         line is not available from the specified mixer due
		'''         to resource restrictions </exception>
		''' <exception cref="SecurityException"> if a matching target data line
		'''         is not available from the specified mixer due to
		'''         security restrictions </exception>
		''' <exception cref="IllegalArgumentException"> if the specified mixer does
		'''         not support at least one target data line supporting
		'''         the specified audio format
		''' </exception>
		''' <seealso cref= #getTargetDataLine(AudioFormat) </seealso>
		''' <seealso cref= AudioPermission
		''' @since 1.5 </seealso>
		Public Shared Function getTargetDataLine(ByVal format As AudioFormat, ByVal mixerinfo As Mixer.Info) As TargetDataLine

			Dim info As New DataLine.Info(GetType(TargetDataLine), format)
			Dim ___mixer As Mixer = AudioSystem.getMixer(mixerinfo)
			Return CType(___mixer.getLine(info), TargetDataLine)
		End Function


		' $$fb 2002-04-12: fix for 4662082: behavior of AudioSystem.getTargetEncodings() methods doesn't match the spec
		''' <summary>
		''' Obtains the encodings that the system can obtain from an
		''' audio input stream with the specified encoding using the set
		''' of installed format converters. </summary>
		''' <param name="sourceEncoding"> the encoding for which conversion support
		''' is queried </param>
		''' <returns> array of encodings.  If <code>sourceEncoding</code>is not supported,
		''' an array of length 0 is returned. Otherwise, the array will have a length
		''' of at least 1, representing <code>sourceEncoding</code> (no conversion). </returns>
		Public Shared Function getTargetEncodings(ByVal sourceEncoding As AudioFormat.Encoding) As AudioFormat.Encoding()

			Dim codecs As IList = formatConversionProviders
			Dim encodings As New ArrayList

			Dim encs As AudioFormat.Encoding() = Nothing

			' gather from all the codecs
			For i As Integer = 0 To codecs.Count - 1
				Dim codec As javax.sound.sampled.spi.FormatConversionProvider = CType(codecs(i), javax.sound.sampled.spi.FormatConversionProvider)
				If codec.isSourceEncodingSupported(sourceEncoding) Then
					encs = codec.targetEncodings
					For j As Integer = 0 To encs.Length - 1
						encodings.Add(encs(j))
					Next j
				End If
			Next i
			Dim encs2 As AudioFormat.Encoding() = CType(encodings.ToArray(GetType(AudioFormat.Encoding)), AudioFormat.Encoding())
			Return encs2
		End Function



		' $$fb 2002-04-12: fix for 4662082: behavior of AudioSystem.getTargetEncodings() methods doesn't match the spec
		''' <summary>
		''' Obtains the encodings that the system can obtain from an
		''' audio input stream with the specified format using the set
		''' of installed format converters. </summary>
		''' <param name="sourceFormat"> the audio format for which conversion
		''' is queried </param>
		''' <returns> array of encodings. If <code>sourceFormat</code>is not supported,
		''' an array of length 0 is returned. Otherwise, the array will have a length
		''' of at least 1, representing the encoding of <code>sourceFormat</code> (no conversion). </returns>
		Public Shared Function getTargetEncodings(ByVal sourceFormat As AudioFormat) As AudioFormat.Encoding()


			Dim codecs As IList = formatConversionProviders
			Dim encodings As New ArrayList

			Dim size As Integer = 0
			Dim index As Integer = 0
			Dim encs As AudioFormat.Encoding() = Nothing

			' gather from all the codecs

			For i As Integer = 0 To codecs.Count - 1
				encs = CType(codecs(i), javax.sound.sampled.spi.FormatConversionProvider).getTargetEncodings(sourceFormat)
				size += encs.Length
				encodings.Add(encs)
			Next i

			' now build a new array

			Dim encs2 As AudioFormat.Encoding() = New AudioFormat.Encoding(size - 1){}
			For i As Integer = 0 To encodings.Count - 1
				encs = CType(encodings(i), AudioFormat.Encoding ())
				For j As Integer = 0 To encs.Length - 1
					encs2(index) = encs(j)
					index += 1
				Next j
			Next i
			Return encs2
		End Function


		''' <summary>
		''' Indicates whether an audio input stream of the specified encoding
		''' can be obtained from an audio input stream that has the specified
		''' format. </summary>
		''' <param name="targetEncoding"> the desired encoding after conversion </param>
		''' <param name="sourceFormat"> the audio format before conversion </param>
		''' <returns> <code>true</code> if the conversion is supported,
		''' otherwise <code>false</code> </returns>
		Public Shared Function isConversionSupported(ByVal targetEncoding As AudioFormat.Encoding, ByVal sourceFormat As AudioFormat) As Boolean


			Dim codecs As IList = formatConversionProviders

			For i As Integer = 0 To codecs.Count - 1
				Dim codec As javax.sound.sampled.spi.FormatConversionProvider = CType(codecs(i), javax.sound.sampled.spi.FormatConversionProvider)
				If codec.isConversionSupported(targetEncoding,sourceFormat) Then Return True
			Next i
			Return False
		End Function


		''' <summary>
		''' Obtains an audio input stream of the indicated encoding, by converting the
		''' provided audio input stream. </summary>
		''' <param name="targetEncoding"> the desired encoding after conversion </param>
		''' <param name="sourceStream"> the stream to be converted </param>
		''' <returns> an audio input stream of the indicated encoding </returns>
		''' <exception cref="IllegalArgumentException"> if the conversion is not supported </exception>
		''' <seealso cref= #getTargetEncodings(AudioFormat.Encoding) </seealso>
		''' <seealso cref= #getTargetEncodings(AudioFormat) </seealso>
		''' <seealso cref= #isConversionSupported(AudioFormat.Encoding, AudioFormat) </seealso>
		''' <seealso cref= #getAudioInputStream(AudioFormat, AudioInputStream) </seealso>
		Public Shared Function getAudioInputStream(ByVal targetEncoding As AudioFormat.Encoding, ByVal sourceStream As AudioInputStream) As AudioInputStream

			Dim codecs As IList = formatConversionProviders

			For i As Integer = 0 To codecs.Count - 1
				Dim codec As javax.sound.sampled.spi.FormatConversionProvider = CType(codecs(i), javax.sound.sampled.spi.FormatConversionProvider)
				If codec.isConversionSupported(targetEncoding, sourceStream.format) Then Return codec.getAudioInputStream(targetEncoding, sourceStream)
			Next i
			' we ran out of options, throw an exception
			Throw New System.ArgumentException("Unsupported conversion: " & targetEncoding & " from " & sourceStream.format)
		End Function


		''' <summary>
		''' Obtains the formats that have a particular encoding and that the system can
		''' obtain from a stream of the specified format using the set of
		''' installed format converters. </summary>
		''' <param name="targetEncoding"> the desired encoding after conversion </param>
		''' <param name="sourceFormat"> the audio format before conversion </param>
		''' <returns> array of formats.  If no formats of the specified
		''' encoding are supported, an array of length 0 is returned. </returns>
		Public Shared Function getTargetFormats(ByVal targetEncoding As AudioFormat.Encoding, ByVal sourceFormat As AudioFormat) As AudioFormat()

			Dim codecs As IList = formatConversionProviders
			Dim formats As New ArrayList

			Dim size As Integer = 0
			Dim index As Integer = 0
			Dim fmts As AudioFormat() = Nothing

			' gather from all the codecs

			For i As Integer = 0 To codecs.Count - 1
				Dim codec As javax.sound.sampled.spi.FormatConversionProvider = CType(codecs(i), javax.sound.sampled.spi.FormatConversionProvider)
				fmts = codec.getTargetFormats(targetEncoding, sourceFormat)
				size += fmts.Length
				formats.Add(fmts)
			Next i

			' now build a new array

			Dim fmts2 As AudioFormat() = New AudioFormat(size - 1){}
			For i As Integer = 0 To formats.Count - 1
				fmts = CType(formats(i), AudioFormat ())
				For j As Integer = 0 To fmts.Length - 1
					fmts2(index) = fmts(j)
					index += 1
				Next j
			Next i
			Return fmts2
		End Function


		''' <summary>
		''' Indicates whether an audio input stream of a specified format
		''' can be obtained from an audio input stream of another specified format. </summary>
		''' <param name="targetFormat"> the desired audio format after conversion </param>
		''' <param name="sourceFormat"> the audio format before conversion </param>
		''' <returns> <code>true</code> if the conversion is supported,
		''' otherwise <code>false</code> </returns>

		Public Shared Function isConversionSupported(ByVal targetFormat As AudioFormat, ByVal sourceFormat As AudioFormat) As Boolean

			Dim codecs As IList = formatConversionProviders

			For i As Integer = 0 To codecs.Count - 1
				Dim codec As javax.sound.sampled.spi.FormatConversionProvider = CType(codecs(i), javax.sound.sampled.spi.FormatConversionProvider)
				If codec.isConversionSupported(targetFormat, sourceFormat) Then Return True
			Next i
			Return False
		End Function


		''' <summary>
		''' Obtains an audio input stream of the indicated format, by converting the
		''' provided audio input stream. </summary>
		''' <param name="targetFormat"> the desired audio format after conversion </param>
		''' <param name="sourceStream"> the stream to be converted </param>
		''' <returns> an audio input stream of the indicated format </returns>
		''' <exception cref="IllegalArgumentException"> if the conversion is not supported
		''' #see #getTargetEncodings(AudioFormat) </exception>
		''' <seealso cref= #getTargetFormats(AudioFormat.Encoding, AudioFormat) </seealso>
		''' <seealso cref= #isConversionSupported(AudioFormat, AudioFormat) </seealso>
		''' <seealso cref= #getAudioInputStream(AudioFormat.Encoding, AudioInputStream) </seealso>
		Public Shared Function getAudioInputStream(ByVal targetFormat As AudioFormat, ByVal sourceStream As AudioInputStream) As AudioInputStream

			If sourceStream.format.matches(targetFormat) Then Return sourceStream

			Dim codecs As IList = formatConversionProviders

			For i As Integer = 0 To codecs.Count - 1
				Dim codec As javax.sound.sampled.spi.FormatConversionProvider = CType(codecs(i), javax.sound.sampled.spi.FormatConversionProvider)
				If codec.isConversionSupported(targetFormat,sourceStream.format) Then Return codec.getAudioInputStream(targetFormat,sourceStream)
			Next i

			' we ran out of options...
			Throw New System.ArgumentException("Unsupported conversion: " & targetFormat & " from " & sourceStream.format)
		End Function


		''' <summary>
		''' Obtains the audio file format of the provided input stream.  The stream must
		''' point to valid audio file data.  The implementation of this method may require
		''' multiple parsers to examine the stream to determine whether they support it.
		''' These parsers must be able to mark the stream, read enough data to determine whether they
		''' support the stream, and, if not, reset the stream's read pointer to its original
		''' position.  If the input stream does not support these operations, this method may fail
		''' with an <code>IOException</code>. </summary>
		''' <param name="stream"> the input stream from which file format information should be
		''' extracted </param>
		''' <returns> an <code>AudioFileFormat</code> object describing the stream's audio file format </returns>
		''' <exception cref="UnsupportedAudioFileException"> if the stream does not point to valid audio
		''' file data recognized by the system </exception>
		''' <exception cref="IOException"> if an input/output exception occurs </exception>
		''' <seealso cref= InputStream#markSupported </seealso>
		''' <seealso cref= InputStream#mark </seealso>
		Public Shared Function getAudioFileFormat(ByVal stream As java.io.InputStream) As AudioFileFormat

			Dim ___providers As IList = audioFileReaders
			Dim format As AudioFileFormat = Nothing

			For i As Integer = 0 To ___providers.Count - 1
				Dim reader As javax.sound.sampled.spi.AudioFileReader = CType(___providers(i), javax.sound.sampled.spi.AudioFileReader)
				Try
					format = reader.getAudioFileFormat(stream) ' throws IOException
					Exit For
				Catch e As UnsupportedAudioFileException
					Continue For
				End Try
			Next i

			If format Is Nothing Then
				Throw New UnsupportedAudioFileException("file is not a supported file type")
			Else
				Return format
			End If
		End Function

		''' <summary>
		''' Obtains the audio file format of the specified URL.  The URL must
		''' point to valid audio file data. </summary>
		''' <param name="url"> the URL from which file format information should be
		''' extracted </param>
		''' <returns> an <code>AudioFileFormat</code> object describing the audio file format </returns>
		''' <exception cref="UnsupportedAudioFileException"> if the URL does not point to valid audio
		''' file data recognized by the system </exception>
		''' <exception cref="IOException"> if an input/output exception occurs </exception>
		Public Shared Function getAudioFileFormat(ByVal url As java.net.URL) As AudioFileFormat

			Dim ___providers As IList = audioFileReaders
			Dim format As AudioFileFormat = Nothing

			For i As Integer = 0 To ___providers.Count - 1
				Dim reader As javax.sound.sampled.spi.AudioFileReader = CType(___providers(i), javax.sound.sampled.spi.AudioFileReader)
				Try
					format = reader.getAudioFileFormat(url) ' throws IOException
					Exit For
				Catch e As UnsupportedAudioFileException
					Continue For
				End Try
			Next i

			If format Is Nothing Then
				Throw New UnsupportedAudioFileException("file is not a supported file type")
			Else
				Return format
			End If
		End Function

		''' <summary>
		''' Obtains the audio file format of the specified <code>File</code>.  The <code>File</code> must
		''' point to valid audio file data. </summary>
		''' <param name="file"> the <code>File</code> from which file format information should be
		''' extracted </param>
		''' <returns> an <code>AudioFileFormat</code> object describing the audio file format </returns>
		''' <exception cref="UnsupportedAudioFileException"> if the <code>File</code> does not point to valid audio
		''' file data recognized by the system </exception>
		''' <exception cref="IOException"> if an I/O exception occurs </exception>
		Public Shared Function getAudioFileFormat(ByVal file As java.io.File) As AudioFileFormat

			Dim ___providers As IList = audioFileReaders
			Dim format As AudioFileFormat = Nothing

			For i As Integer = 0 To ___providers.Count - 1
				Dim reader As javax.sound.sampled.spi.AudioFileReader = CType(___providers(i), javax.sound.sampled.spi.AudioFileReader)
				Try
					format = reader.getAudioFileFormat(file) ' throws IOException
					Exit For
				Catch e As UnsupportedAudioFileException
					Continue For
				End Try
			Next i

			If format Is Nothing Then
				Throw New UnsupportedAudioFileException("file is not a supported file type")
			Else
				Return format
			End If
		End Function


		''' <summary>
		''' Obtains an audio input stream from the provided input stream.  The stream must
		''' point to valid audio file data.  The implementation of this method may
		''' require multiple parsers to
		''' examine the stream to determine whether they support it.  These parsers must
		''' be able to mark the stream, read enough data to determine whether they
		''' support the stream, and, if not, reset the stream's read pointer to its original
		''' position.  If the input stream does not support these operation, this method may fail
		''' with an <code>IOException</code>. </summary>
		''' <param name="stream"> the input stream from which the <code>AudioInputStream</code> should be
		''' constructed </param>
		''' <returns> an <code>AudioInputStream</code> object based on the audio file data contained
		''' in the input stream. </returns>
		''' <exception cref="UnsupportedAudioFileException"> if the stream does not point to valid audio
		''' file data recognized by the system </exception>
		''' <exception cref="IOException"> if an I/O exception occurs </exception>
		''' <seealso cref= InputStream#markSupported </seealso>
		''' <seealso cref= InputStream#mark </seealso>
		Public Shared Function getAudioInputStream(ByVal stream As java.io.InputStream) As AudioInputStream

			Dim ___providers As IList = audioFileReaders
			Dim audioStream As AudioInputStream = Nothing

			For i As Integer = 0 To ___providers.Count - 1
				Dim reader As javax.sound.sampled.spi.AudioFileReader = CType(___providers(i), javax.sound.sampled.spi.AudioFileReader)
				Try
					audioStream = reader.getAudioInputStream(stream) ' throws IOException
					Exit For
				Catch e As UnsupportedAudioFileException
					Continue For
				End Try
			Next i

			If audioStream Is Nothing Then
				Throw New UnsupportedAudioFileException("could not get audio input stream from input stream")
			Else
				Return audioStream
			End If
		End Function

		''' <summary>
		''' Obtains an audio input stream from the URL provided.  The URL must
		''' point to valid audio file data. </summary>
		''' <param name="url"> the URL for which the <code>AudioInputStream</code> should be
		''' constructed </param>
		''' <returns> an <code>AudioInputStream</code> object based on the audio file data pointed
		''' to by the URL </returns>
		''' <exception cref="UnsupportedAudioFileException"> if the URL does not point to valid audio
		''' file data recognized by the system </exception>
		''' <exception cref="IOException"> if an I/O exception occurs </exception>
		Public Shared Function getAudioInputStream(ByVal url As java.net.URL) As AudioInputStream

			Dim ___providers As IList = audioFileReaders
			Dim audioStream As AudioInputStream = Nothing

			For i As Integer = 0 To ___providers.Count - 1
				Dim reader As javax.sound.sampled.spi.AudioFileReader = CType(___providers(i), javax.sound.sampled.spi.AudioFileReader)
				Try
					audioStream = reader.getAudioInputStream(url) ' throws IOException
					Exit For
				Catch e As UnsupportedAudioFileException
					Continue For
				End Try
			Next i

			If audioStream Is Nothing Then
				Throw New UnsupportedAudioFileException("could not get audio input stream from input URL")
			Else
				Return audioStream
			End If
		End Function

		''' <summary>
		''' Obtains an audio input stream from the provided <code>File</code>.  The <code>File</code> must
		''' point to valid audio file data. </summary>
		''' <param name="file"> the <code>File</code> for which the <code>AudioInputStream</code> should be
		''' constructed </param>
		''' <returns> an <code>AudioInputStream</code> object based on the audio file data pointed
		''' to by the <code>File</code> </returns>
		''' <exception cref="UnsupportedAudioFileException"> if the <code>File</code> does not point to valid audio
		''' file data recognized by the system </exception>
		''' <exception cref="IOException"> if an I/O exception occurs </exception>
		Public Shared Function getAudioInputStream(ByVal file As java.io.File) As AudioInputStream

			Dim ___providers As IList = audioFileReaders
			Dim audioStream As AudioInputStream = Nothing

			For i As Integer = 0 To ___providers.Count - 1
				Dim reader As javax.sound.sampled.spi.AudioFileReader = CType(___providers(i), javax.sound.sampled.spi.AudioFileReader)
				Try
					audioStream = reader.getAudioInputStream(file) ' throws IOException
					Exit For
				Catch e As UnsupportedAudioFileException
					Continue For
				End Try
			Next i

			If audioStream Is Nothing Then
				Throw New UnsupportedAudioFileException("could not get audio input stream from input file")
			Else
				Return audioStream
			End If
		End Function


		''' <summary>
		''' Obtains the file types for which file writing support is provided by the system. </summary>
		''' <returns> array of unique file types.  If no file types are supported,
		''' an array of length 0 is returned. </returns>
		Public Property Shared audioFileTypes As AudioFileFormat.Type()
			Get
				Dim ___providers As IList = audioFileWriters
				Dim returnTypesSet As java.util.Set = New HashSet
    
				For i As Integer = 0 To ___providers.Count - 1
					Dim writer As javax.sound.sampled.spi.AudioFileWriter = CType(___providers(i), javax.sound.sampled.spi.AudioFileWriter)
					Dim fileTypes As AudioFileFormat.Type() = writer.audioFileTypes
					For j As Integer = 0 To fileTypes.Length - 1
						returnTypesSet.add(fileTypes(j))
					Next j
				Next i
				Dim returnTypes As AudioFileFormat.Type() = CType(returnTypesSet.ToArray(New AudioFileFormat.Type(){}), AudioFileFormat.Type())
				Return returnTypes
			End Get
		End Property


		''' <summary>
		''' Indicates whether file writing support for the specified file type is provided
		''' by the system. </summary>
		''' <param name="fileType"> the file type for which write capabilities are queried </param>
		''' <returns> <code>true</code> if the file type is supported,
		''' otherwise <code>false</code> </returns>
		Public Shared Function isFileTypeSupported(ByVal fileType As AudioFileFormat.Type) As Boolean

			Dim ___providers As IList = audioFileWriters

			For i As Integer = 0 To ___providers.Count - 1
				Dim writer As javax.sound.sampled.spi.AudioFileWriter = CType(___providers(i), javax.sound.sampled.spi.AudioFileWriter)
				If writer.isFileTypeSupported(fileType) Then Return True
			Next i
			Return False
		End Function


		''' <summary>
		''' Obtains the file types that the system can write from the
		''' audio input stream specified. </summary>
		''' <param name="stream"> the audio input stream for which audio file type support
		''' is queried </param>
		''' <returns> array of file types.  If no file types are supported,
		''' an array of length 0 is returned. </returns>
		Public Shared Function getAudioFileTypes(ByVal stream As AudioInputStream) As AudioFileFormat.Type()
			Dim ___providers As IList = audioFileWriters
			Dim returnTypesSet As java.util.Set = New HashSet

			For i As Integer = 0 To ___providers.Count - 1
				Dim writer As javax.sound.sampled.spi.AudioFileWriter = CType(___providers(i), javax.sound.sampled.spi.AudioFileWriter)
				Dim fileTypes As AudioFileFormat.Type() = writer.getAudioFileTypes(stream)
				For j As Integer = 0 To fileTypes.Length - 1
					returnTypesSet.add(fileTypes(j))
				Next j
			Next i
			Dim returnTypes As AudioFileFormat.Type() = CType(returnTypesSet.ToArray(New AudioFileFormat.Type(){}), AudioFileFormat.Type())
			Return returnTypes
		End Function


		''' <summary>
		''' Indicates whether an audio file of the specified file type can be written
		''' from the indicated audio input stream. </summary>
		''' <param name="fileType"> the file type for which write capabilities are queried </param>
		''' <param name="stream"> the stream for which file-writing support is queried </param>
		''' <returns> <code>true</code> if the file type is supported for this audio input stream,
		''' otherwise <code>false</code> </returns>
		Public Shared Function isFileTypeSupported(ByVal fileType As AudioFileFormat.Type, ByVal stream As AudioInputStream) As Boolean

			Dim ___providers As IList = audioFileWriters

			For i As Integer = 0 To ___providers.Count - 1
				Dim writer As javax.sound.sampled.spi.AudioFileWriter = CType(___providers(i), javax.sound.sampled.spi.AudioFileWriter)
				If writer.isFileTypeSupported(fileType, stream) Then Return True
			Next i
			Return False
		End Function


		''' <summary>
		''' Writes a stream of bytes representing an audio file of the specified file type
		''' to the output stream provided.  Some file types require that
		''' the length be written into the file header; such files cannot be written from
		''' start to finish unless the length is known in advance.  An attempt
		''' to write a file of such a type will fail with an IOException if the length in
		''' the audio file type is <code>AudioSystem.NOT_SPECIFIED</code>.
		''' </summary>
		''' <param name="stream"> the audio input stream containing audio data to be
		''' written to the file </param>
		''' <param name="fileType"> the kind of audio file to write </param>
		''' <param name="out"> the stream to which the file data should be written </param>
		''' <returns> the number of bytes written to the output stream </returns>
		''' <exception cref="IOException"> if an input/output exception occurs </exception>
		''' <exception cref="IllegalArgumentException"> if the file type is not supported by
		''' the system </exception>
		''' <seealso cref= #isFileTypeSupported </seealso>
		''' <seealso cref=     #getAudioFileTypes </seealso>
		Public Shared Function write(ByVal stream As AudioInputStream, ByVal fileType As AudioFileFormat.Type, ByVal out As java.io.OutputStream) As Integer

			Dim ___providers As IList = audioFileWriters
			Dim bytesWritten As Integer = 0
			Dim flag As Boolean = False

			For i As Integer = 0 To ___providers.Count - 1
				Dim writer As javax.sound.sampled.spi.AudioFileWriter = CType(___providers(i), javax.sound.sampled.spi.AudioFileWriter)
				Try
					bytesWritten = writer.write(stream, fileType, out) ' throws IOException
					flag = True
					Exit For
				Catch e As System.ArgumentException
					' thrown if this provider cannot write the sequence, try the next
					Continue For
				End Try
			Next i
			If Not flag Then
				Throw New System.ArgumentException("could not write audio file: file type not supported: " & fileType)
			Else
				Return bytesWritten
			End If
		End Function


		''' <summary>
		''' Writes a stream of bytes representing an audio file of the specified file type
		''' to the external file provided. </summary>
		''' <param name="stream"> the audio input stream containing audio data to be
		''' written to the file </param>
		''' <param name="fileType"> the kind of audio file to write </param>
		''' <param name="out"> the external file to which the file data should be written </param>
		''' <returns> the number of bytes written to the file </returns>
		''' <exception cref="IOException"> if an I/O exception occurs </exception>
		''' <exception cref="IllegalArgumentException"> if the file type is not supported by
		''' the system </exception>
		''' <seealso cref= #isFileTypeSupported </seealso>
		''' <seealso cref=     #getAudioFileTypes </seealso>
		Public Shared Function write(ByVal stream As AudioInputStream, ByVal fileType As AudioFileFormat.Type, ByVal out As java.io.File) As Integer

			Dim ___providers As IList = audioFileWriters
			Dim bytesWritten As Integer = 0
			Dim flag As Boolean = False

			For i As Integer = 0 To ___providers.Count - 1
				Dim writer As javax.sound.sampled.spi.AudioFileWriter = CType(___providers(i), javax.sound.sampled.spi.AudioFileWriter)
				Try
					bytesWritten = writer.write(stream, fileType, out) ' throws IOException
					flag = True
					Exit For
				Catch e As System.ArgumentException
					' thrown if this provider cannot write the sequence, try the next
					Continue For
				End Try
			Next i
			If Not flag Then
				Throw New System.ArgumentException("could not write audio file: file type not supported: " & fileType)
			Else
				Return bytesWritten
			End If
		End Function


		' METHODS FOR INTERNAL IMPLEMENTATION USE

		''' <summary>
		''' Obtains the set of MixerProviders on the system.
		''' </summary>
		Private Property Shared mixerProviders As IList
			Get
				Return getProviders(GetType(javax.sound.sampled.spi.MixerProvider))
			End Get
		End Property


		''' <summary>
		''' Obtains the set of format converters (codecs, transcoders, etc.)
		''' that are currently installed on the system. </summary>
		''' <returns> an array of
		''' {@link javax.sound.sampled.spi.FormatConversionProvider
		''' FormatConversionProvider}
		''' objects representing the available format converters.  If no format
		''' converters readers are available on the system, an array of length 0 is
		''' returned. </returns>
		Private Property Shared formatConversionProviders As IList
			Get
				Return getProviders(GetType(javax.sound.sampled.spi.FormatConversionProvider))
			End Get
		End Property


		''' <summary>
		''' Obtains the set of audio file readers that are currently installed on the system. </summary>
		''' <returns> a List of
		''' {@link javax.sound.sampled.spi.AudioFileReader
		''' AudioFileReader}
		''' objects representing the installed audio file readers.  If no audio file
		''' readers are available on the system, an empty List is returned. </returns>
		Private Property Shared audioFileReaders As IList
			Get
				Return getProviders(GetType(javax.sound.sampled.spi.AudioFileReader))
			End Get
		End Property


		''' <summary>
		''' Obtains the set of audio file writers that are currently installed on the system. </summary>
		''' <returns> a List of
		''' <seealso cref="javax.sound.samples.spi.AudioFileWriter AudioFileWriter"/>
		''' objects representing the available audio file writers.  If no audio file
		''' writers are available on the system, an empty List is returned. </returns>
		Private Property Shared audioFileWriters As IList
			Get
				Return getProviders(GetType(javax.sound.sampled.spi.AudioFileWriter))
			End Get
		End Property



		''' <summary>
		''' Attempts to locate and return a default Mixer that provides lines
		''' of the specified type.
		''' </summary>
		''' <param name="providers"> the installed mixer providers </param>
		''' <param name="info"> The requested line type
		''' TargetDataLine.class, Clip.class or Port.class. </param>
		''' <returns> a Mixer that matches the requirements, or null if no default mixer found </returns>
		Private Shared Function getDefaultMixer(ByVal providers As IList, ByVal info As Line.Info) As Mixer
			Dim lineClass As Type = info.lineClass
			Dim providerClassName As String = com.sun.media.sound.JDK13Services.getDefaultProviderClassName(lineClass)
			Dim instanceName As String = com.sun.media.sound.JDK13Services.getDefaultInstanceName(lineClass)
			Dim ___mixer As Mixer

			If providerClassName IsNot Nothing Then
				Dim defaultProvider As javax.sound.sampled.spi.MixerProvider = getNamedProvider(providerClassName, providers)
				If defaultProvider IsNot Nothing Then
					If instanceName IsNot Nothing Then
						___mixer = getNamedMixer(instanceName, defaultProvider, info)
						If ___mixer IsNot Nothing Then Return ___mixer
					Else
						___mixer = getFirstMixer(defaultProvider, info, False) ' mixing not required
						If ___mixer IsNot Nothing Then Return ___mixer
					End If

				End If
			End If

	'         Provider class not specified or
	'           provider class cannot be found, or
	'           provider class and instance specified and instance cannot be found or is not appropriate 
			If instanceName IsNot Nothing Then
				___mixer = getNamedMixer(instanceName, providers, info)
				If ___mixer IsNot Nothing Then Return ___mixer
			End If


	'         No default are specified, or if something is specified, everything
	'           failed. 
			Return Nothing
		End Function



		''' <summary>
		''' Return a MixerProvider of a given class from the list of
		'''    MixerProviders.
		''' 
		'''    This method never requires the returned Mixer to do mixing. </summary>
		'''    <param name="providerClassName"> The class name of the provider to be returned. </param>
		'''    <param name="providers"> The list of MixerProviders that is searched. </param>
		'''    <returns> A MixerProvider of the requested class, or null if none is
		'''    found. </returns>
		Private Shared Function getNamedProvider(ByVal providerClassName As String, ByVal providers As IList) As javax.sound.sampled.spi.MixerProvider
			For i As Integer = 0 To providers.Count - 1
				Dim provider As javax.sound.sampled.spi.MixerProvider = CType(providers(i), javax.sound.sampled.spi.MixerProvider)
				If provider.GetType().name.Equals(providerClassName) Then Return provider
			Next i
			Return Nothing
		End Function


		''' <summary>
		''' Return a Mixer with a given name from a given MixerProvider.
		'''  This method never requires the returned Mixer to do mixing. </summary>
		'''  <param name="mixerName"> The name of the Mixer to be returned. </param>
		'''  <param name="provider"> The MixerProvider to check for Mixers. </param>
		'''  <param name="info"> The type of line the returned Mixer is required to
		'''  support.
		''' </param>
		'''  <returns> A Mixer matching the requirements, or null if none is found. </returns>
		Private Shared Function getNamedMixer(ByVal mixerName As String, ByVal provider As javax.sound.sampled.spi.MixerProvider, ByVal info As Line.Info) As Mixer
			Dim infos As Mixer.Info() = provider.mixerInfo
			For i As Integer = 0 To infos.Length - 1
				If infos(i).name.Equals(mixerName) Then
					Dim ___mixer As Mixer = provider.getMixer(infos(i))
					If isAppropriateMixer(___mixer, info, False) Then Return ___mixer
				End If
			Next i
			Return Nothing
		End Function


		''' <summary>
		''' From a List of MixerProviders, return a Mixer with a given name.
		'''    This method never requires the returned Mixer to do mixing. </summary>
		'''    <param name="mixerName"> The name of the Mixer to be returned. </param>
		'''    <param name="providers"> The List of MixerProviders to check for Mixers. </param>
		'''    <param name="info"> The type of line the returned Mixer is required to
		'''    support. </param>
		'''    <returns> A Mixer matching the requirements, or null if none is found. </returns>
		Private Shared Function getNamedMixer(ByVal mixerName As String, ByVal providers As IList, ByVal info As Line.Info) As Mixer
			For i As Integer = 0 To providers.Count - 1
				Dim provider As javax.sound.sampled.spi.MixerProvider = CType(providers(i), javax.sound.sampled.spi.MixerProvider)
				Dim ___mixer As Mixer = getNamedMixer(mixerName, provider, info)
				If ___mixer IsNot Nothing Then Return ___mixer
			Next i
			Return Nothing
		End Function


		''' <summary>
		''' From a given MixerProvider, return the first appropriate Mixer. </summary>
		'''    <param name="provider"> The MixerProvider to check for Mixers. </param>
		'''    <param name="info"> The type of line the returned Mixer is required to
		'''    support. </param>
		'''    <param name="isMixingRequired"> If true, only Mixers that support mixing are
		'''    returned for line types of SourceDataLine and Clip.
		''' </param>
		'''    <returns> A Mixer that is considered appropriate, or null
		'''    if none is found. </returns>
		Private Shared Function getFirstMixer(ByVal provider As javax.sound.sampled.spi.MixerProvider, ByVal info As Line.Info, ByVal isMixingRequired As Boolean) As Mixer
			Dim infos As Mixer.Info() = provider.mixerInfo
			For j As Integer = 0 To infos.Length - 1
				Dim ___mixer As Mixer = provider.getMixer(infos(j))
				If isAppropriateMixer(___mixer, info, isMixingRequired) Then Return ___mixer
			Next j
			Return Nothing
		End Function


		''' <summary>
		''' Checks if a Mixer is appropriate.
		'''    A Mixer is considered appropriate if it support the given line type.
		'''    If isMixingRequired is true and the line type is an output one
		'''    (SourceDataLine, Clip), the mixer is appropriate if it supports
		'''    at least 2 (concurrent) lines of the given type.
		''' </summary>
		'''    <returns> true if the mixer is considered appropriate according to the
		'''    rules given above, false otherwise. </returns>
		Private Shared Function isAppropriateMixer(ByVal mixer As Mixer, ByVal lineInfo As Line.Info, ByVal isMixingRequired As Boolean) As Boolean
			If Not mixer.isLineSupported(lineInfo) Then Return False
			Dim lineClass As Type = lineInfo.lineClass
			If isMixingRequired AndAlso (lineClass.IsSubclassOf(GetType(SourceDataLine)) OrElse lineClass.IsSubclassOf(GetType(Clip))) Then
				Dim maxLines As Integer = mixer.getMaxLines(lineInfo)
				Return ((maxLines = NOT_SPECIFIED) OrElse (maxLines > 1))
			End If
			Return True
		End Function



		''' <summary>
		''' Like getMixerInfo, but return List
		''' </summary>
		Private Property Shared mixerInfoList As IList
			Get
				Dim ___providers As IList = mixerProviders
				Return getMixerInfoList(___providers)
			End Get
		End Property


		''' <summary>
		''' Like getMixerInfo, but return List
		''' </summary>
		Private Shared Function getMixerInfoList(ByVal providers As IList) As IList
			Dim infos As IList = New ArrayList

			Dim someInfos As Mixer.Info() ' per-mixer
			Dim allInfos As Mixer.Info() ' for all mixers

			For i As Integer = 0 To providers.Count - 1
				someInfos = CType(CType(providers(i), javax.sound.sampled.spi.MixerProvider).mixerInfo, Mixer.Info())

				For j As Integer = 0 To someInfos.Length - 1
					infos.Add(someInfos(j))
				Next j
			Next i

			Return infos
		End Function


		''' <summary>
		''' Obtains the set of services currently installed on the system
		''' using sun.misc.Service, the SPI mechanism in 1.3. </summary>
		''' <returns> a List of instances of providers for the requested service.
		''' If no providers are available, a vector of length 0 will be returned. </returns>
		Private Shared Function getProviders(ByVal providerClass As Type) As IList
			Return com.sun.media.sound.JDK13Services.getProviders(providerClass)
		End Function
	End Class

End Namespace