'
' * Copyright (c) 1999, 2002, Oracle and/or its affiliates. All rights reserved.
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
	''' The <code>ControllerEventListener</code> interface should be implemented
	''' by classes whose instances need to be notified when a <code>Sequencer</code>
	''' has processed a requested type of MIDI control-change event.
	''' To register a <code>ControllerEventListener</code> object to receive such
	''' notifications, invoke the
	''' {@link Sequencer#addControllerEventListener(ControllerEventListener, int[])
	''' addControllerEventListener} method of <code>Sequencer</code>,
	''' specifying the types of MIDI controllers about which you are interested in
	''' getting control-change notifications.
	''' </summary>
	''' <seealso cref= MidiChannel#controlChange(int, int)
	''' 
	''' @author Kara Kytle </seealso>
	Public Interface ControllerEventListener
		Inherits java.util.EventListener

		''' <summary>
		''' Invoked when a <code>Sequencer</code> has encountered and processed
		''' a control-change event of interest to this listener.  The event passed
		''' in is a <code>ShortMessage</code> whose first data byte indicates
		''' the controller number and whose second data byte is the value to which
		''' the controller was set.
		''' </summary>
		''' <param name="event"> the control-change event that the sequencer encountered in
		''' the sequence it is processing
		''' </param>
		''' <seealso cref= Sequencer#addControllerEventListener(ControllerEventListener, int[]) </seealso>
		''' <seealso cref= MidiChannel#controlChange(int, int) </seealso>
		''' <seealso cref= ShortMessage#getData1 </seealso>
		''' <seealso cref= ShortMessage#getData2 </seealso>
		Sub controlChange(ByVal [event] As ShortMessage)
	End Interface

End Namespace