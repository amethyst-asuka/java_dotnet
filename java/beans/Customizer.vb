'
' * Copyright (c) 1996, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.beans

	''' <summary>
	''' A customizer class provides a complete custom GUI for customizing
	''' a target Java Bean.
	''' <P>
	''' Each customizer should inherit from the java.awt.Component class so
	''' it can be instantiated inside an AWT dialog or panel.
	''' <P>
	''' Each customizer should have a null constructor.
	''' </summary>

	Public Interface Customizer

		''' <summary>
		''' Set the object to be customized.  This method should be called only
		''' once, before the Customizer has been added to any parent AWT container. </summary>
		''' <param name="bean">  The object to be customized. </param>
		WriteOnly Property [object] As Object

		''' <summary>
		''' Register a listener for the PropertyChange event.  The customizer
		''' should fire a PropertyChange event whenever it changes the target
		''' bean in a way that might require the displayed properties to be
		''' refreshed.
		''' </summary>
		''' <param name="listener">  An object to be invoked when a PropertyChange
		'''          event is fired. </param>
		 Sub addPropertyChangeListener(ByVal listener As PropertyChangeListener)

		''' <summary>
		''' Remove a listener for the PropertyChange event.
		''' </summary>
		''' <param name="listener">  The PropertyChange listener to be removed. </param>
		Sub removePropertyChangeListener(ByVal listener As PropertyChangeListener)

	End Interface

End Namespace