'
' * Copyright (c) 1996, 1997, Oracle and/or its affiliates. All rights reserved.
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
	''' A VetoableChange event gets fired whenever a bean changes a "constrained"
	''' property.  You can register a VetoableChangeListener with a source bean
	''' so as to be notified of any constrained property updates.
	''' </summary>
	Public Interface VetoableChangeListener
		Inherits java.util.EventListener

		''' <summary>
		''' This method gets called when a constrained property is changed.
		''' </summary>
		''' <param name="evt"> a <code>PropertyChangeEvent</code> object describing the
		'''                event source and the property that has changed. </param>
		''' <exception cref="PropertyVetoException"> if the recipient wishes the property
		'''              change to be rolled back. </exception>
		Sub vetoableChange(  evt As PropertyChangeEvent)
	End Interface

End Namespace