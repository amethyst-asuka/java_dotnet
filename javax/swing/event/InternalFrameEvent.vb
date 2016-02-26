'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.event


	''' <summary>
	''' An <code>AWTEvent</code> that adds support for
	''' <code>JInternalFrame</code> objects as the event source.  This class has the
	''' same event types as <code>WindowEvent</code>,
	''' although different IDs are used.
	''' Help on handling internal frame events
	''' is in
	''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/events/internalframelistener.html" target="_top">How to Write an Internal Frame Listener</a>,
	''' a section in <em>The Java Tutorial</em>.
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' </summary>
	''' <seealso cref= java.awt.event.WindowEvent </seealso>
	''' <seealso cref= java.awt.event.WindowListener </seealso>
	''' <seealso cref= JInternalFrame </seealso>
	''' <seealso cref= InternalFrameListener
	''' 
	''' @author Thomas Ball </seealso>
	Public Class InternalFrameEvent
		Inherits java.awt.AWTEvent

		''' <summary>
		''' The first number in the range of IDs used for internal frame events.
		''' </summary>
		Public Const INTERNAL_FRAME_FIRST As Integer = 25549

		''' <summary>
		''' The last number in the range of IDs used for internal frame events.
		''' </summary>
		Public Const INTERNAL_FRAME_LAST As Integer = 25555

		''' <summary>
		''' The "window opened" event.  This event is delivered only
		''' the first time the internal frame is made visible.
		''' </summary>
		''' <seealso cref= JInternalFrame#show </seealso>
		Public Const INTERNAL_FRAME_OPENED As Integer = INTERNAL_FRAME_FIRST

		''' <summary>
		''' The "window is closing" event. This event is delivered when
		''' the user attempts to close the internal frame, such as by
		''' clicking the internal frame's close button,
		''' or when a program attempts to close the internal frame
		''' by invoking the <code>setClosed</code> method.
		''' </summary>
		''' <seealso cref= JInternalFrame#setDefaultCloseOperation </seealso>
		''' <seealso cref= JInternalFrame#doDefaultCloseAction </seealso>
		''' <seealso cref= JInternalFrame#setClosed </seealso>
		Public Shared ReadOnly INTERNAL_FRAME_CLOSING As Integer = 1 + INTERNAL_FRAME_FIRST

		''' <summary>
		''' The "window closed" event. This event is delivered after
		''' the internal frame has been closed as the result of a call to
		''' the <code>setClosed</code> or
		''' <code>dispose</code> method.
		''' </summary>
		''' <seealso cref= JInternalFrame#setClosed </seealso>
		''' <seealso cref= JInternalFrame#dispose </seealso>
		Public Shared ReadOnly INTERNAL_FRAME_CLOSED As Integer = 2 + INTERNAL_FRAME_FIRST

		''' <summary>
		''' The "window iconified" event.
		''' This event indicates that the internal frame
		''' was shrunk down to a small icon.
		''' </summary>
		''' <seealso cref= JInternalFrame#setIcon </seealso>
		Public Shared ReadOnly INTERNAL_FRAME_ICONIFIED As Integer = 3 + INTERNAL_FRAME_FIRST

		''' <summary>
		''' The "window deiconified" event type. This event indicates that the
		''' internal frame has been restored to its normal size.
		''' </summary>
		''' <seealso cref= JInternalFrame#setIcon </seealso>
		Public Shared ReadOnly INTERNAL_FRAME_DEICONIFIED As Integer = 4 + INTERNAL_FRAME_FIRST

		''' <summary>
		''' The "window activated" event type. This event indicates that keystrokes
		''' and mouse clicks are directed towards this internal frame.
		''' </summary>
		''' <seealso cref= JInternalFrame#show </seealso>
		''' <seealso cref= JInternalFrame#setSelected </seealso>
		Public Shared ReadOnly INTERNAL_FRAME_ACTIVATED As Integer = 5 + INTERNAL_FRAME_FIRST

		''' <summary>
		''' The "window deactivated" event type. This event indicates that keystrokes
		''' and mouse clicks are no longer directed to the internal frame.
		''' </summary>
		''' <seealso cref= JInternalFrame#setSelected </seealso>
		Public Shared ReadOnly INTERNAL_FRAME_DEACTIVATED As Integer = 6 + INTERNAL_FRAME_FIRST

		''' <summary>
		''' Constructs an <code>InternalFrameEvent</code> object. </summary>
		''' <param name="source"> the <code>JInternalFrame</code> object that originated the event </param>
		''' <param name="id">     an integer indicating the type of event </param>
		Public Sub New(ByVal source As javax.swing.JInternalFrame, ByVal id As Integer)
			MyBase.New(source, id)
		End Sub

		''' <summary>
		''' Returns a parameter string identifying this event.
		''' This method is useful for event logging and for debugging.
		''' </summary>
		''' <returns> a string identifying the event and its attributes </returns>
		Public Overridable Function paramString() As String
			Dim typeStr As String
			Select Case id
			  Case INTERNAL_FRAME_OPENED
				  typeStr = "INTERNAL_FRAME_OPENED"
			  Case INTERNAL_FRAME_CLOSING
				  typeStr = "INTERNAL_FRAME_CLOSING"
			  Case INTERNAL_FRAME_CLOSED
				  typeStr = "INTERNAL_FRAME_CLOSED"
			  Case INTERNAL_FRAME_ICONIFIED
				  typeStr = "INTERNAL_FRAME_ICONIFIED"
			  Case INTERNAL_FRAME_DEICONIFIED
				  typeStr = "INTERNAL_FRAME_DEICONIFIED"
			  Case INTERNAL_FRAME_ACTIVATED
				  typeStr = "INTERNAL_FRAME_ACTIVATED"
			  Case INTERNAL_FRAME_DEACTIVATED
				  typeStr = "INTERNAL_FRAME_DEACTIVATED"
			  Case Else
				  typeStr = "unknown type"
			End Select
			Return typeStr
		End Function


		''' <summary>
		''' Returns the originator of the event.
		''' </summary>
		''' <returns> the <code>JInternalFrame</code> object that originated the event
		''' @since 1.3 </returns>

		Public Overridable Property internalFrame As javax.swing.JInternalFrame
			Get
			  Return If(TypeOf source Is javax.swing.JInternalFrame, CType(source, javax.swing.JInternalFrame), Nothing)
			End Get
		End Property


	End Class

End Namespace