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

Namespace javax.sound.sampled



	''' <summary>
	''' Instances of classes that implement the <code>LineListener</code> interface can register to
	''' receive events when a line's status changes.
	''' 
	''' @author Kara Kytle
	''' </summary>
	''' <seealso cref= Line </seealso>
	''' <seealso cref= Line#addLineListener </seealso>
	''' <seealso cref= Line#removeLineListener </seealso>
	''' <seealso cref= LineEvent
	''' 
	''' @since 1.3 </seealso>
	'
	' * Instances of classes that implement the <code>LineListener</code> interface can register to
	' * receive events when a line's status changes.
	' *
	' * @see Line
	' * @see Line#addLineListener
	' * @see Line#removeLineListener
	' * @see LineEvent
	' *
	' * @author Kara Kytle
	' 
	Public Interface LineListener
		Inherits java.util.EventListener

		''' <summary>
		''' Informs the listener that a line's state has changed.  The listener can then invoke
		''' <code>LineEvent</code> methods to obtain information about the event. </summary>
		''' <param name="event"> a line event that describes the change </param>
	'    
	'     * Informs the listener that a line's state has changed.  The listener can then invoke
	'     * <code>LineEvent</code> methods to obtain information about the event.
	'     * @param event a line event that describes the change
	'     
		Sub update(ByVal [event] As LineEvent)

	End Interface ' interface LineListener

End Namespace