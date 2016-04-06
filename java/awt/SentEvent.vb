'
' * Copyright (c) 2000, 2007, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt


	''' <summary>
	''' A wrapping tag for a nested AWTEvent which indicates that the event was
	''' sent from another AppContext. The destination AppContext should handle the
	''' event even if it is currently blocked waiting for a SequencedEvent or
	''' another SentEvent to be handled.
	''' 
	''' @author David Mendenhall
	''' </summary>
	Friend Class SentEvent
		Inherits AWTEvent
		Implements ActiveEvent

	'    
	'     * serialVersionUID
	'     
		Private Const serialVersionUID As Long = -383615247028828931L

		Friend Shared ReadOnly ID As Integer = java.awt.event.FocusEvent.FOCUS_LAST + 2

		Friend dispatched As Boolean
		Private nested As AWTEvent
		Private toNotify As sun.awt.AppContext

		Friend Sub New()
			Me.New(Nothing)
		End Sub
		Friend Sub New(  nested As AWTEvent)
			Me.New(nested, Nothing)
		End Sub
		Friend Sub New(  nested As AWTEvent,   toNotify As sun.awt.AppContext)
			MyBase.New(If(nested IsNot Nothing, nested.source, Toolkit.defaultToolkit), ID)
			Me.nested = nested
			Me.toNotify = toNotify
		End Sub

		Public Overridable Sub dispatch() Implements ActiveEvent.dispatch
			Try
				If nested IsNot Nothing Then Toolkit.eventQueue.dispatchEvent(nested)
			Finally
				dispatched = True
				If toNotify IsNot Nothing Then sun.awt.SunToolkit.postEvent(toNotify, New SentEvent)
				SyncLock Me
					notifyAll()
				End SyncLock
			End Try
		End Sub
		Friend Sub dispose()
			dispatched = True
			If toNotify IsNot Nothing Then sun.awt.SunToolkit.postEvent(toNotify, New SentEvent)
			SyncLock Me
				notifyAll()
			End SyncLock
		End Sub
	End Class

End Namespace