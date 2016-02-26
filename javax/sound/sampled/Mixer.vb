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
	''' A mixer is an audio device with one or more lines.  It need not be
	''' designed for mixing audio signals.  A mixer that actually mixes audio
	''' has multiple input (source) lines and at least one output (target) line.
	''' The former are often instances of classes that implement
	''' <code><seealso cref="SourceDataLine"/></code>,
	''' and the latter, <code><seealso cref="TargetDataLine"/></code>.  <code><seealso cref="Port"/></code>
	''' objects, too, are either source lines or target lines.
	''' A mixer can accept prerecorded, loopable sound as input, by having
	''' some of its source lines be instances of objects that implement the
	''' <code><seealso cref="Clip"/></code> interface.
	''' <p>
	''' Through methods of the <code>Line</code> interface, which <code>Mixer</code> extends,
	''' a mixer might provide a set of controls that are global to the mixer.  For example,
	''' the mixer can have a master gain control.  These global controls are distinct
	''' from the controls belonging to each of the mixer's individual lines.
	''' <p>
	''' Some mixers, especially
	''' those with internal digital mixing capabilities, may provide
	''' additional capabilities by implementing the <code>DataLine</code> interface.
	''' <p>
	''' A mixer can support synchronization of its lines.  When one line in
	''' a synchronized group is started or stopped, the other lines in the group
	''' automatically start or stop simultaneously with the explicitly affected one.
	''' 
	''' @author Kara Kytle
	''' @since 1.3
	''' </summary>
	Public Interface Mixer
		Inherits Line

		''' <summary>
		''' Obtains information about this mixer, including the product's name,
		''' version, vendor, etc. </summary>
		''' <returns> a mixer info object that describes this mixer </returns>
		''' <seealso cref= Mixer.Info </seealso>
		ReadOnly Property mixerInfo As Info


		''' <summary>
		''' Obtains information about the set of source lines supported
		''' by this mixer.
		''' Some source lines may only be available when this mixer is open. </summary>
		''' <returns> array of <code>Line.Info</code> objects representing source lines
		''' for this mixer.  If no source lines are supported,
		''' an array of length 0 is returned. </returns>
		ReadOnly Property sourceLineInfo As Line.Info()

		''' <summary>
		''' Obtains information about the set of target lines supported
		''' by this mixer.
		''' Some target lines may only be available when this mixer is open. </summary>
		''' <returns> array of <code>Line.Info</code> objects representing target lines
		''' for this mixer.  If no target lines are supported,
		''' an array of length 0 is returned. </returns>
		ReadOnly Property targetLineInfo As Line.Info()


		''' <summary>
		''' Obtains information about source lines of a particular type supported
		''' by the mixer.
		''' Some source lines may only be available when this mixer is open. </summary>
		''' <param name="info"> a <code>Line.Info</code> object describing lines about which information
		''' is queried </param>
		''' <returns> an array of <code>Line.Info</code> objects describing source lines matching
		''' the type requested.  If no matching source lines are supported, an array of length 0
		''' is returned. </returns>
		Function getSourceLineInfo(ByVal info As Line.Info) As Line.Info()


		''' <summary>
		''' Obtains information about target lines of a particular type supported
		''' by the mixer.
		''' Some target lines may only be available when this mixer is open. </summary>
		''' <param name="info"> a <code>Line.Info</code> object describing lines about which information
		''' is queried </param>
		''' <returns> an array of <code>Line.Info</code> objects describing target lines matching
		''' the type requested.  If no matching target lines are supported, an array of length 0
		''' is returned. </returns>
		Function getTargetLineInfo(ByVal info As Line.Info) As Line.Info()


		''' <summary>
		''' Indicates whether the mixer supports a line (or lines) that match
		''' the specified <code>Line.Info</code> object.
		''' Some lines may only be supported when this mixer is open. </summary>
		''' <param name="info"> describes the line for which support is queried </param>
		''' <returns> <code>true</code> if at least one matching line is
		''' supported, <code>false</code> otherwise </returns>
		Function isLineSupported(ByVal info As Line.Info) As Boolean

		''' <summary>
		''' Obtains a line that is available for use and that matches the description
		''' in the specified <code>Line.Info</code> object.
		''' 
		''' <p>If a <code>DataLine</code> is requested, and <code>info</code>
		''' is an instance of <code>DataLine.Info</code> specifying at
		''' least one fully qualified audio format, the last one
		''' will be used as the default format of the returned
		''' <code>DataLine</code>.
		''' </summary>
		''' <param name="info"> describes the desired line </param>
		''' <returns> a line that is available for use and that matches the description
		''' in the specified {@code Line.Info} object </returns>
		''' <exception cref="LineUnavailableException"> if a matching line
		''' is not available due to resource restrictions </exception>
		''' <exception cref="IllegalArgumentException"> if this mixer does
		''' not support any lines matching the description </exception>
		''' <exception cref="SecurityException"> if a matching line
		''' is not available due to security restrictions </exception>
		Function getLine(ByVal info As Line.Info) As Line

		'$$fb 2002-04-12: fix for 4667258: behavior of Mixer.getMaxLines(Line.Info) method doesn't match the spec
		''' <summary>
		''' Obtains the approximate maximum number of lines of the requested type that can be open
		''' simultaneously on the mixer.
		''' 
		''' Certain types of mixers do not have a hard bound and may allow opening more lines.
		''' Since certain lines are a shared resource, a mixer may not be able to open the maximum
		''' number of lines if another process has opened lines of this mixer.
		''' 
		''' The requested type is any line that matches the description in
		''' the provided <code>Line.Info</code> object.  For example, if the info
		''' object represents a speaker
		''' port, and the mixer supports exactly one speaker port, this method
		''' should return 1.  If the info object represents a source data line
		''' and the mixer supports the use of 32 source data lines simultaneously,
		''' the return value should be 32.
		''' If there is no limit, this function returns <code>AudioSystem.NOT_SPECIFIED</code>. </summary>
		''' <param name="info"> a <code>Line.Info</code> that describes the line for which
		''' the number of supported instances is queried </param>
		''' <returns> the maximum number of matching lines supported, or <code>AudioSystem.NOT_SPECIFIED</code> </returns>
		Function getMaxLines(ByVal info As Line.Info) As Integer


		''' <summary>
		''' Obtains the set of all source lines currently open to this mixer.
		''' </summary>
		''' <returns> the source lines currently open to the mixer.
		''' If no source lines are currently open to this mixer,  an
		''' array of length 0 is returned. </returns>
		''' <exception cref="SecurityException"> if the matching lines
		''' are not available due to security restrictions </exception>
		ReadOnly Property sourceLines As Line()

		''' <summary>
		''' Obtains the set of all target lines currently open from this mixer.
		''' </summary>
		''' <returns> target lines currently open from the mixer.
		''' If no target lines are currently open from this mixer, an
		''' array of length 0 is returned. </returns>
		''' <exception cref="SecurityException"> if the matching lines
		''' are not available due to security restrictions </exception>
		ReadOnly Property targetLines As Line()

		''' <summary>
		''' Synchronizes two or more lines.  Any subsequent command that starts or stops
		''' audio playback or capture for one of these lines will exert the
		''' same effect on the other lines in the group, so that they start or stop playing or
		''' capturing data simultaneously.
		''' </summary>
		''' <param name="lines"> the lines that should be synchronized </param>
		''' <param name="maintainSync"> <code>true</code> if the synchronization
		''' must be precisely maintained (i.e., the synchronization must be sample-accurate)
		''' at all times during operation of the lines , or <code>false</code>
		''' if precise synchronization is required only during start and stop operations
		''' </param>
		''' <exception cref="IllegalArgumentException"> if the lines cannot be synchronized.
		''' This may occur if the lines are of different types or have different
		''' formats for which this mixer does not support synchronization, or if
		''' all lines specified do not belong to this mixer. </exception>
		Sub synchronize(ByVal lines As Line(), ByVal maintainSync As Boolean)

		''' <summary>
		''' Releases synchronization for the specified lines.  The array must
		''' be identical to one for which synchronization has already been
		''' established; otherwise an exception may be thrown.  However, <code>null</code>
		''' may be specified, in which case all currently synchronized lines that belong
		''' to this mixer are unsynchronized. </summary>
		''' <param name="lines"> the synchronized lines for which synchronization should be
		''' released, or <code>null</code> for all this mixer's synchronized lines
		''' </param>
		''' <exception cref="IllegalArgumentException"> if the lines cannot be unsynchronized.
		''' This may occur if the argument specified does not exactly match a set
		''' of lines for which synchronization has already been established. </exception>
		Sub unsynchronize(ByVal lines As Line())


		''' <summary>
		''' Reports whether this mixer supports synchronization of the specified set of lines.
		''' </summary>
		''' <param name="lines"> the set of lines for which synchronization support is queried </param>
		''' <param name="maintainSync"> <code>true</code> if the synchronization
		''' must be precisely maintained (i.e., the synchronization must be sample-accurate)
		''' at all times during operation of the lines , or <code>false</code>
		''' if precise synchronization is required only during start and stop operations
		''' </param>
		''' <returns> <code>true</code> if the lines can be synchronized, <code>false</code>
		''' otherwise </returns>
		Function isSynchronizationSupported(ByVal lines As Line(), ByVal maintainSync As Boolean) As Boolean


		''' <summary>
		''' The <code>Mixer.Info</code> class represents information about an audio mixer,
		''' including the product's name, version, and vendor, along with a textual
		''' description.  This information may be retrieved through the
		''' <seealso cref="Mixer#getMixerInfo() getMixerInfo"/>
		''' method of the <code>Mixer</code> interface.
		''' 
		''' @author Kara Kytle
		''' @since 1.3
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interface inner types are not converted:
'		public static class Info
	'	{
	'
	'		''' <summary>
	'		''' Mixer name.
	'		''' </summary>
	'		private final String name;
	'
	'		''' <summary>
	'		''' Mixer vendor.
	'		''' </summary>
	'		private final String vendor;
	'
	'		''' <summary>
	'		''' Mixer description.
	'		''' </summary>
	'		private final String description;
	'
	'		''' <summary>
	'		''' Mixer version.
	'		''' </summary>
	'		private final String version;
	'
	'		''' <summary>
	'		''' Constructs a mixer's info object, passing it the given
	'		''' textual information. </summary>
	'		''' <param name="name"> the name of the mixer </param>
	'		''' <param name="vendor"> the company who manufactures or creates the hardware
	'		''' or software mixer </param>
	'		''' <param name="description"> descriptive text about the mixer </param>
	'		''' <param name="version"> version information for the mixer </param>
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
	'		''' Indicates whether two info objects are equal, returning <code>true</code> if
	'		''' they are identical. </summary>
	'		''' <param name="obj"> the reference object with which to compare this info
	'		''' object </param>
	'		''' <returns> <code>true</code> if this info object is the same as the
	'		''' <code>obj</code> argument; <code>false</code> otherwise </returns>
	'		public final boolean equals(Object obj)
	'		{
	'			Return MyBase.equals(obj);
	'		}
	'
	'		''' <summary>
	'		''' Finalizes the hashcode method.
	'		''' </summary>
	'		''' <returns> the hashcode for this object </returns>
	'		public final int hashCode()
	'		{
	'			Return MyBase.hashCode();
	'		}
	'
	'		''' <summary>
	'		''' Obtains the name of the mixer. </summary>
	'		''' <returns> a string that names the mixer </returns>
	'		public final String getName()
	'		{
	'			Return name;
	'		}
	'
	'		''' <summary>
	'		''' Obtains the vendor of the mixer. </summary>
	'		''' <returns> a string that names the mixer's vendor </returns>
	'		public final String getVendor()
	'		{
	'			Return vendor;
	'		}
	'
	'		''' <summary>
	'		''' Obtains the description of the mixer. </summary>
	'		''' <returns> a textual description of the mixer </returns>
	'		public final String getDescription()
	'		{
	'			Return description;
	'		}
	'
	'		''' <summary>
	'		''' Obtains the version of the mixer. </summary>
	'		''' <returns> textual version information for the mixer </returns>
	'		public final String getVersion()
	'		{
	'			Return version;
	'		}
	'
	'		''' <summary>
	'		''' Provides a string representation of the mixer info. </summary>
	'		''' <returns> a string describing the info object </returns>
	'		public final String toString()
	'		{
	'			Return (name + ", version " + version);
	'		}
	'	} ' class Info
	End Interface

End Namespace