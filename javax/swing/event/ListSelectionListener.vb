'
' * Copyright (c) 1997, 1998, Oracle and/or its affiliates. All rights reserved.
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
	''' The listener that's notified when a lists selection value
	''' changes.
	''' </summary>
	''' <seealso cref= javax.swing.ListSelectionModel
	''' 
	''' @author Hans Muller </seealso>

	Public Interface ListSelectionListener
		Inherits java.util.EventListener

	  ''' <summary>
	  ''' Called whenever the value of the selection changes. </summary>
	  ''' <param name="e"> the event that characterizes the change. </param>
	  Sub valueChanged(ByVal e As ListSelectionEvent)
	End Interface

End Namespace