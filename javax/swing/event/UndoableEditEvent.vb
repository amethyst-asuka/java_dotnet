Imports javax.swing.undo

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' An event indicating that an operation which can be undone has occurred.
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' 
	''' @author Ray Ryan
	''' </summary>
	Public Class UndoableEditEvent
		Inherits java.util.EventObject

		Private myEdit As UndoableEdit

		''' <summary>
		''' Constructs an UndoableEditEvent object.
		''' </summary>
		''' <param name="source">  the Object that originated the event
		'''                (typically <code>this</code>) </param>
		''' <param name="edit">    an UndoableEdit object </param>
		Public Sub New(ByVal source As Object, ByVal edit As UndoableEdit)
			MyBase.New(source)
			myEdit = edit
		End Sub

		''' <summary>
		''' Returns the edit value.
		''' </summary>
		''' <returns> the UndoableEdit object encapsulating the edit </returns>
		Public Overridable Property edit As UndoableEdit
			Get
				Return myEdit
			End Get
		End Property
	End Class

End Namespace