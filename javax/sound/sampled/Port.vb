'
' * Copyright (c) 1999, 2004, Oracle and/or its affiliates. All rights reserved.
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
	''' Ports are simple lines for input or output of audio to or from audio devices.
	''' Common examples of ports that act as source lines (mixer inputs) include the microphone,
	''' line input, and CD-ROM drive.  Ports that act as target lines (mixer outputs) include the
	''' speaker, headphone, and line output.  You can access port using a <code><seealso cref="Port.Info"/></code>
	''' object.
	''' 
	''' @author Kara Kytle
	''' @since 1.3
	''' </summary>
	Public Interface Port
		Inherits Line


		' INNER CLASSES


		''' <summary>
		''' The <code>Port.Info</code> class extends <code><seealso cref="Line.Info"/></code>
		''' with additional information specific to ports, including the port's name
		''' and whether it is a source or a target for its mixer.
		''' By definition, a port acts as either a source or a target to its mixer,
		''' but not both.  (Audio input ports are sources; audio output ports are targets.)
		''' <p>
		''' To learn what ports are available, you can retrieve port info objects through the
		''' <code><seealso cref="Mixer#getSourceLineInfo getSourceLineInfo"/></code> and
		''' <code><seealso cref="Mixer#getTargetLineInfo getTargetLineInfo"/></code>
		''' methods of the <code>Mixer</code> interface.  Instances of the
		''' <code>Port.Info</code> class may also be constructed and used to obtain
		''' lines matching the parameters specified in the <code>Port.Info</code> object.
		''' 
		''' @author Kara Kytle
		''' @since 1.3
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interface inner types are not converted:
'		public static class Info extends Line.Info
	'	{
	'
	'
	'		' AUDIO PORT TYPE DEFINES
	'
	'
	'		' SOURCE PORTS
	'
	'		''' <summary>
	'		''' A type of port that gets audio from a built-in microphone or a microphone jack.
	'		''' </summary>
	'		public static final Info MICROPHONE = New Info(Port.class,"MICROPHONE", True);
	'
	'		''' <summary>
	'		''' A type of port that gets audio from a line-level audio input jack.
	'		''' </summary>
	'		public static final Info LINE_IN = New Info(Port.class,"LINE_IN", True);
	'
	'		''' <summary>
	'		''' A type of port that gets audio from a CD-ROM drive.
	'		''' </summary>
	'		public static final Info COMPACT_DISC = New Info(Port.class,"COMPACT_DISC", True);
	'
	'
	'		' TARGET PORTS
	'
	'		''' <summary>
	'		''' A type of port that sends audio to a built-in speaker or a speaker jack.
	'		''' </summary>
	'		public static final Info SPEAKER = New Info(Port.class,"SPEAKER", False);
	'
	'		''' <summary>
	'		''' A type of port that sends audio to a headphone jack.
	'		''' </summary>
	'		public static final Info HEADPHONE = New Info(Port.class,"HEADPHONE", False);
	'
	'		''' <summary>
	'		''' A type of port that sends audio to a line-level audio output jack.
	'		''' </summary>
	'		public static final Info LINE_OUT = New Info(Port.class,"LINE_OUT", False);
	'
	'
	'		' FUTURE DIRECTIONS...
	'
	'		' telephone
	'		' DAT
	'		' DVD
	'
	'
	'		' INSTANCE VARIABLES
	'
	'		private String name;
	'		private boolean isSource;
	'
	'
	'		' CONSTRUCTOR
	'
	'		''' <summary>
	'		''' Constructs a port's info object from the information given.
	'		''' This constructor is typically used by an implementation
	'		''' of Java Sound to describe a supported line.
	'		''' </summary>
	'		''' <param name="lineClass"> the class of the port described by the info object. </param>
	'		''' <param name="name"> the string that names the port </param>
	'		''' <param name="isSource"> <code>true</code> if the port is a source port (such
	'		''' as a microphone), <code>false</code> if the port is a target port
	'		''' (such as a speaker). </param>
	'		public Info(Class lineClass, String name, boolean isSource)
	'		{
	'
	'			MyBase(lineClass);
	'			Me.name = name;
	'			Me.isSource = isSource;
	'		}
	'
	'
	'		' METHODS
	'
	'		''' <summary>
	'		''' Obtains the name of the port. </summary>
	'		''' <returns> the string that names the port </returns>
	'		public String getName()
	'		{
	'			Return name;
	'		}
	'
	'		''' <summary>
	'		''' Indicates whether the port is a source or a target for its mixer. </summary>
	'		''' <returns> <code>true</code> if the port is a source port (such
	'		''' as a microphone), <code>false</code> if the port is a target port
	'		''' (such as a speaker). </returns>
	'		public boolean isSource()
	'		{
	'			Return isSource;
	'		}
	'
	'		''' <summary>
	'		''' Indicates whether this info object specified matches this one.
	'		''' To match, the match requirements of the superclass must be
	'		''' met and the types must be equal. </summary>
	'		''' <param name="info"> the info object for which the match is queried </param>
	'		public boolean matches(Line.Info info)
	'		{
	'
	'			if (! (MyBase.matches(info)))
	'			{
	'				Return False;
	'			}
	'
	'			if (!(name.equals(((Info)info).getName())))
	'			{
	'				Return False;
	'			}
	'
	'			if (! (isSource == ((Info)info).isSource()))
	'			{
	'				Return False;
	'			}
	'
	'			Return True;
	'		}
	'
	'
	'		''' <summary>
	'		''' Finalizes the equals method
	'		''' </summary>
	'		public final boolean equals(Object obj)
	'		{
	'			Return MyBase.equals(obj);
	'		}
	'
	'		''' <summary>
	'		''' Finalizes the hashCode method
	'		''' </summary>
	'		public final int hashCode()
	'		{
	'			Return MyBase.hashCode();
	'		}
	'
	'
	'
	'		''' <summary>
	'		''' Provides a <code>String</code> representation
	'		''' of the port. </summary>
	'		''' <returns>  a string that describes the port </returns>
	'		public final String toString()
	'		{
	'			Return (name + ((isSource == True) ? " source" : " target") + " port");
	'		}
	'
	'	} ' class Info
	End Interface

End Namespace