'
' * Copyright (c) 1999, 2010, Oracle and/or its affiliates. All rights reserved.
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
	''' The <code>Line</code> interface represents a mono or multi-channel
	''' audio feed. A line is an element of the digital audio
	''' "pipeline," such as a mixer, an input or output port,
	''' or a data path into or out of a mixer.
	''' <p>
	''' A line can have controls, such as gain, pan, and reverb.
	''' The controls themselves are instances of classes that extend the
	''' base <code><seealso cref="Control"/></code> class.
	''' The <code>Line</code> interface provides two accessor methods for
	''' obtaining the line's controls: <code><seealso cref="#getControls getControls"/></code> returns the
	''' entire set, and <code><seealso cref="#getControl getControl"/></code> returns a single control of
	''' specified type.
	''' <p>
	''' Lines exist in various states at different times.  When a line opens, it reserves system
	''' resources for itself, and when it closes, these resources are freed for
	''' other objects or applications. The <code><seealso cref="#isOpen()"/></code> method lets
	''' you discover whether a line is open or closed.
	''' An open line need not be processing data, however.  Such processing is
	''' typically initiated by subinterface methods such as
	''' <code><seealso cref="SourceDataLine#write SourceDataLine.write"/></code> and
	''' <code><seealso cref="TargetDataLine#read TargetDataLine.read"/></code>.
	''' <p>
	''' You can register an object to receive notifications whenever the line's
	''' state changes.  The object must implement the <code><seealso cref="LineListener"/></code>
	''' interface, which consists of the single method
	''' <code><seealso cref="LineListener#update update"/></code>.
	''' This method will be invoked when a line opens and closes (and, if it's a
	''' <seealso cref="DataLine"/>, when it starts and stops).
	''' <p>
	''' An object can be registered to listen to multiple lines.  The event it
	''' receives in its <code>update</code> method will specify which line created
	''' the event, what type of event it was
	''' (<code>OPEN</code>, <code>CLOSE</code>, <code>START</code>, or <code>STOP</code>),
	''' and how many sample frames the line had processed at the time the event occurred.
	''' <p>
	''' Certain line operations, such as open and close, can generate security
	''' exceptions if invoked by unprivileged code when the line is a shared audio
	''' resource.
	''' 
	''' @author Kara Kytle
	''' </summary>
	''' <seealso cref= LineEvent
	''' @since 1.3 </seealso>
	Public Interface Line
		Inherits AutoCloseable

		''' <summary>
		''' Obtains the <code>Line.Info</code> object describing this
		''' line. </summary>
		''' <returns> description of the line </returns>
		ReadOnly Property lineInfo As Line.Info

		''' <summary>
		''' Opens the line, indicating that it should acquire any required
		''' system resources and become operational.
		''' If this operation
		''' succeeds, the line is marked as open, and an <code>OPEN</code> event is dispatched
		''' to the line's listeners.
		''' <p>
		''' Note that some lines, once closed, cannot be reopened.  Attempts
		''' to reopen such a line will always result in an <code>LineUnavailableException</code>.
		''' <p>
		''' Some types of lines have configurable properties that may affect
		''' resource allocation.   For example, a <code>DataLine</code> must
		''' be opened with a particular format and buffer size.  Such lines
		''' should provide a mechanism for configuring these properties, such
		''' as an additional <code>open</code> method or methods which allow
		''' an application to specify the desired settings.
		''' <p>
		''' This method takes no arguments, and opens the line with the current
		''' settings.  For <code><seealso cref="SourceDataLine"/></code> and
		''' <code><seealso cref="TargetDataLine"/></code> objects, this means that the line is
		''' opened with default settings.  For a <code><seealso cref="Clip"/></code>, however,
		''' the buffer size is determined when data is loaded.  Since this method does not
		''' allow the application to specify any data to load, an IllegalArgumentException
		''' is thrown. Therefore, you should instead use one of the <code>open</code> methods
		''' provided in the <code>Clip</code> interface to load data into the <code>Clip</code>.
		''' <p>
		''' For <code>DataLine</code>'s, if the <code>DataLine.Info</code>
		''' object which was used to retrieve the line, specifies at least
		''' one fully qualified audio format, the last one will be used
		''' as the default format.
		''' </summary>
		''' <exception cref="IllegalArgumentException"> if this method is called on a Clip instance. </exception>
		''' <exception cref="LineUnavailableException"> if the line cannot be
		''' opened due to resource restrictions. </exception>
		''' <exception cref="SecurityException"> if the line cannot be
		''' opened due to security restrictions.
		''' </exception>
		''' <seealso cref= #close </seealso>
		''' <seealso cref= #isOpen </seealso>
		''' <seealso cref= LineEvent </seealso>
		''' <seealso cref= DataLine </seealso>
		''' <seealso cref= Clip#open(AudioFormat, byte[], int, int) </seealso>
		''' <seealso cref= Clip#open(AudioInputStream) </seealso>
		Sub open()


		''' <summary>
		''' Closes the line, indicating that any system resources
		''' in use by the line can be released.  If this operation
		''' succeeds, the line is marked closed and a <code>CLOSE</code> event is dispatched
		''' to the line's listeners. </summary>
		''' <exception cref="SecurityException"> if the line cannot be
		''' closed due to security restrictions.
		''' </exception>
		''' <seealso cref= #open </seealso>
		''' <seealso cref= #isOpen </seealso>
		''' <seealso cref= LineEvent </seealso>
		Sub close()



		''' <summary>
		''' Indicates whether the line is open, meaning that it has reserved
		''' system resources and is operational, although it might not currently be
		''' playing or capturing sound. </summary>
		''' <returns> <code>true</code> if the line is open, otherwise <code>false</code>
		''' </returns>
		''' <seealso cref= #open() </seealso>
		''' <seealso cref= #close() </seealso>
		ReadOnly Property open As Boolean


		''' <summary>
		''' Obtains the set of controls associated with this line.
		''' Some controls may only be available when the line is open.
		''' If there are no controls, this method returns an array of length 0. </summary>
		''' <returns> the array of controls </returns>
		''' <seealso cref= #getControl </seealso>
		ReadOnly Property controls As Control()

		''' <summary>
		''' Indicates whether the line supports a control of the specified type.
		''' Some controls may only be available when the line is open. </summary>
		''' <param name="control"> the type of the control for which support is queried </param>
		''' <returns> <code>true</code> if at least one control of the specified type is
		''' supported, otherwise <code>false</code>. </returns>
		Function isControlSupported(ByVal control As Control.Type) As Boolean


		''' <summary>
		''' Obtains a control of the specified type,
		''' if there is any.
		''' Some controls may only be available when the line is open. </summary>
		''' <param name="control"> the type of the requested control </param>
		''' <returns> a control of the specified type </returns>
		''' <exception cref="IllegalArgumentException"> if a control of the specified type
		''' is not supported </exception>
		''' <seealso cref= #getControls </seealso>
		''' <seealso cref= #isControlSupported(Control.Type control) </seealso>
		Function getControl(ByVal control As Control.Type) As Control


		''' <summary>
		''' Adds a listener to this line.  Whenever the line's status changes, the
		''' listener's <code>update()</code> method is called with a <code>LineEvent</code> object
		''' that describes the change. </summary>
		''' <param name="listener"> the object to add as a listener to this line </param>
		''' <seealso cref= #removeLineListener </seealso>
		''' <seealso cref= LineListener#update </seealso>
		''' <seealso cref= LineEvent </seealso>
		Sub addLineListener(ByVal listener As LineListener)


		''' <summary>
		''' Removes the specified listener from this line's list of listeners. </summary>
		''' <param name="listener"> listener to remove </param>
		''' <seealso cref= #addLineListener </seealso>
		Sub removeLineListener(ByVal listener As LineListener)


		''' <summary>
		''' A <code>Line.Info</code> object contains information about a line.
		''' The only information provided by <code>Line.Info</code> itself
		''' is the Java class of the line.
		''' A subclass of <code>Line.Info</code> adds other kinds of information
		''' about the line.  This additional information depends on which <code>Line</code>
		''' subinterface is implemented by the kind of line that the <code>Line.Info</code>
		''' subclass describes.
		''' <p>
		''' A <code>Line.Info</code> can be retrieved using various methods of
		''' the <code>Line</code>, <code>Mixer</code>, and <code>AudioSystem</code>
		''' interfaces.  Other such methods let you pass a <code>Line.Info</code> as
		''' an argument, to learn whether lines matching the specified configuration
		''' are available and to obtain them.
		''' 
		''' @author Kara Kytle
		''' </summary>
		''' <seealso cref= Line#getLineInfo </seealso>
		''' <seealso cref= Mixer#getSourceLineInfo </seealso>
		''' <seealso cref= Mixer#getTargetLineInfo </seealso>
		''' <seealso cref= Mixer#getLine <code>Mixer.getLine(Line.Info)</code> </seealso>
		''' <seealso cref= Mixer#getSourceLineInfo(Line.Info) <code>Mixer.getSourceLineInfo(Line.Info)</code> </seealso>
		''' <seealso cref= Mixer#getSourceLineInfo(Line.Info) <code>Mixer.getTargetLineInfo(Line.Info)</code> </seealso>
		''' <seealso cref= Mixer#isLineSupported <code>Mixer.isLineSupported(Line.Info)</code> </seealso>
		''' <seealso cref= AudioSystem#getLine <code>AudioSystem.getLine(Line.Info)</code> </seealso>
		''' <seealso cref= AudioSystem#getSourceLineInfo <code>AudioSystem.getSourceLineInfo(Line.Info)</code> </seealso>
		''' <seealso cref= AudioSystem#getTargetLineInfo <code>AudioSystem.getTargetLineInfo(Line.Info)</code> </seealso>
		''' <seealso cref= AudioSystem#isLineSupported <code>AudioSystem.isLineSupported(Line.Info)</code>
		''' @since 1.3 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interface inner types are not converted:
'		public static class Info
	'	{
	'
	'		''' <summary>
	'		''' The class of the line described by the info object.
	'		''' </summary>
	'		private final Class lineClass;
	'
	'
	'		''' <summary>
	'		''' Constructs an info object that describes a line of the specified class.
	'		''' This constructor is typically used by an application to
	'		''' describe a desired line. </summary>
	'		''' <param name="lineClass"> the class of the line that the new Line.Info object describes </param>
	'		public Info(Class lineClass)
	'		{
	'
	'			if (lineClass == Nothing)
	'			{
	'				Me.lineClass = Line.class;
	'			}
	'			else
	'			{
	'				Me.lineClass = lineClass;
	'			}
	'		}
	'
	'
	'
	'		''' <summary>
	'		''' Obtains the class of the line that this Line.Info object describes. </summary>
	'		''' <returns> the described line's class </returns>
	'		public Class getLineClass()
	'		{
	'			Return lineClass;
	'		}
	'
	'
	'		''' <summary>
	'		''' Indicates whether the specified info object matches this one.
	'		''' To match, the specified object must be identical to or
	'		''' a special case of this one.  The specified info object
	'		''' must be either an instance of the same class as this one,
	'		''' or an instance of a sub-type of this one.  In addition, the
	'		''' attributes of the specified object must be compatible with the
	'		''' capabilities of this one.  Specifically, the routing configuration
	'		''' for the specified info object must be compatible with that of this
	'		''' one.
	'		''' Subclasses may add other criteria to determine whether the two objects
	'		''' match.
	'		''' </summary>
	'		''' <param name="info"> the info object which is being compared to this one </param>
	'		''' <returns> <code>true</code> if the specified object matches this one,
	'		''' <code>false</code> otherwise </returns>
	'		public boolean matches(Info info)
	'		{
	'
	'			' $$kk: 08.30.99: is this backwards?
	'			' dataLine.matches(targetDataLine) == true: targetDataLine is always dataLine
	'			' targetDataLine.matches(dataLine) == false
	'			' so if i want to make sure i get a targetDataLine, i need:
	'			' targetDataLine.matches(prospective_match) == true
	'			' => prospective_match may be other things as well, but it is at least a targetDataLine
	'			' targetDataLine defines the requirements which prospective_match must meet.
	'
	'
	'			' "if this Class object represents a declared class, this method returns
	'			' true if the specified Object argument is an instance of the represented
	'			' class (or of any of its subclasses)"
	'			' GainControlClass.isInstance(MyGainObj) => true
	'			' GainControlClass.isInstance(MySpecialGainInterfaceObj) => true
	'
	'			' this_class.isInstance(that_object)       => that object can by cast to this class
	'			'                                                                          => that_object's class may be a subtype of this_class
	'			'                                                                          => that may be more specific (subtype) of this
	'
	'			' "If this Class object represents an interface, this method returns true
	'			' if the class or any superclass of the specified Object argument implements
	'			' this interface"
	'			' GainControlClass.isInstance(MyGainObj) => true
	'			' GainControlClass.isInstance(GenericControlObj) => may be false
	'			' => that may be more specific
	'
	'			if (! (Me.getClass().isInstance(info)))
	'			{
	'				Return False;
	'			}
	'
	'
	'			' this.isAssignableFrom(that)  =>  this is same or super to that
	'			'                                                          =>      this is at least as general as that
	'			'                                                          =>      that may be subtype of this
	'
	'			if (! (getLineClass().isAssignableFrom(info.getLineClass())))
	'			{
	'				Return False;
	'			}
	'
	'			Return True;
	'		}
	'
	'
	'		''' <summary>
	'		''' Obtains a textual description of the line info. </summary>
	'		''' <returns> a string description </returns>
	'		public String toString()
	'		{
	'
	'			String fullPackagePath = "javax.sound.sampled.";
	'			String initialString = New String(getLineClass().toString());
	'			String finalString;
	'
	'			int index = initialString.indexOf(fullPackagePath);
	'
	'			if (index != -1)
	'			{
	'				finalString = initialString.substring(0, index) + initialString.substring((index + fullPackagePath.length()), initialString.length() - ((index + fullPackagePath.length())));
	'			}
	'			else
	'			{
	'				finalString = initialString;
	'			}
	'
	'			Return finalString;
	'		}
	'
	'	} ' class Info

	End Interface ' interface Line

End Namespace