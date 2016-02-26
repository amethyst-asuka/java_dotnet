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
	''' MenuEvent is used to notify interested parties that
	''' the menu which is the event source has been posted,
	''' selected, or canceled.
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
	''' @author Georges Saab
	''' @author David Karlton
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class MenuEvent
		Inherits java.util.EventObject

		''' <summary>
		''' Constructs a MenuEvent object.
		''' </summary>
		''' <param name="source">  the Object that originated the event
		'''                (typically <code>this</code>) </param>
		Public Sub New(ByVal source As Object)
			MyBase.New(source)
		End Sub
	End Class

End Namespace