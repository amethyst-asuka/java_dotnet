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

Namespace javax.sound.sampled.spi


	''' <summary>
	''' A provider or factory for a particular mixer type.
	''' This mechanism allows the implementation to determine
	''' how resources are managed in creation / management of
	''' a mixer.
	''' 
	''' @author Kara Kytle
	''' @since 1.3
	''' </summary>
	Public MustInherit Class MixerProvider


		''' <summary>
		''' Indicates whether the mixer provider supports the mixer represented by
		''' the specified mixer info object.
		''' <p>
		''' The full set of mixer info objects that represent the mixers supported
		''' by this {@code MixerProvider} may be obtained
		''' through the {@code getMixerInfo} method.
		''' </summary>
		''' <param name="info"> an info object that describes the mixer for which support is queried </param>
		''' <returns> {@code true} if the specified mixer is supported,
		'''     otherwise {@code false} </returns>
		''' <seealso cref= #getMixerInfo() </seealso>
		Public Overridable Function isMixerSupported(ByVal info As javax.sound.sampled.Mixer.Info) As Boolean

			Dim infos As javax.sound.sampled.Mixer.Info() = mixerInfo

			For i As Integer = 0 To infos.Length - 1
				If info.Equals(infos(i)) Then Return True
			Next i
			Return False
		End Function


		''' <summary>
		''' Obtains the set of info objects representing the mixer
		''' or mixers provided by this MixerProvider.
		''' <p>
		''' The {@code isMixerSupported} method returns {@code true}
		''' for all the info objects returned by this method.
		''' The corresponding mixer instances for the info objects
		''' are returned by the {@code getMixer} method.
		''' </summary>
		''' <returns> a set of mixer info objects </returns>
		''' <seealso cref= #getMixer(javax.sound.sampled.Mixer.Info) getMixer(Mixer.Info) </seealso>
		''' <seealso cref= #isMixerSupported(javax.sound.sampled.Mixer.Info) isMixerSupported(Mixer.Info) </seealso>
		Public MustOverride ReadOnly Property mixerInfo As javax.sound.sampled.Mixer.Info()


		''' <summary>
		''' Obtains an instance of the mixer represented by the info object.
		''' <p>
		''' The full set of the mixer info objects that represent the mixers
		''' supported by this {@code MixerProvider} may be obtained
		''' through the {@code getMixerInfo} method.
		''' Use the {@code isMixerSupported} method to test whether
		''' this {@code MixerProvider} supports a particular mixer.
		''' </summary>
		''' <param name="info"> an info object that describes the desired mixer </param>
		''' <returns> mixer instance </returns>
		''' <exception cref="IllegalArgumentException"> if the info object specified does not
		'''     match the info object for a mixer supported by this MixerProvider. </exception>
		''' <seealso cref= #getMixerInfo() </seealso>
		''' <seealso cref= #isMixerSupported(javax.sound.sampled.Mixer.Info) isMixerSupported(Mixer.Info) </seealso>
		Public MustOverride Function getMixer(ByVal info As javax.sound.sampled.Mixer.Info) As javax.sound.sampled.Mixer
	End Class

End Namespace