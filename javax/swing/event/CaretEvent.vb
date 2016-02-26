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
	''' CaretEvent is used to notify interested parties that
	''' the text caret has changed in the event source.
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
	''' @author  Timothy Prinzing
	''' </summary>
	Public MustInherit Class CaretEvent
		Inherits java.util.EventObject

		''' <summary>
		''' Creates a new CaretEvent object.
		''' </summary>
		''' <param name="source"> the object responsible for the event </param>
		Public Sub New(ByVal source As Object)
			MyBase.New(source)
		End Sub

		''' <summary>
		''' Fetches the location of the caret.
		''' </summary>
		''' <returns> the dot &gt;= 0 </returns>
		Public MustOverride ReadOnly Property dot As Integer

		''' <summary>
		''' Fetches the location of other end of a logical
		''' selection.  If there is no selection, this
		''' will be the same as dot.
		''' </summary>
		''' <returns> the mark &gt;= 0 </returns>
		Public MustOverride ReadOnly Property mark As Integer
	End Class

End Namespace