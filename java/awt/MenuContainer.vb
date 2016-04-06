Imports System

'
' * Copyright (c) 1995, 2004, Oracle and/or its affiliates. All rights reserved.
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
	''' The super class of all menu related containers.
	''' 
	''' @author      Arthur van Hoff
	''' </summary>

	Public Interface MenuContainer
		ReadOnly Property font As Font
		Sub remove(  comp As MenuComponent)

		''' @deprecated As of JDK version 1.1
		''' replaced by dispatchEvent(AWTEvent). 
		<Obsolete("As of JDK version 1.1")> _
		Function postEvent(  evt As [Event]) As Boolean
	End Interface

End Namespace