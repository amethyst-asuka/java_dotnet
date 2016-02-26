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

Namespace javax.accessibility

	''' <summary>
	''' Interface Accessible is the main interface for the accessibility package.
	''' All components that support
	''' the accessibility package must implement this interface.
	''' It contains a single method, <seealso cref="#getAccessibleContext"/>, which
	''' returns an instance of the class <seealso cref="AccessibleContext"/>.
	''' 
	''' @author      Peter Korn
	''' @author      Hans Muller
	''' @author      Willie Walker
	''' </summary>
	Public Interface Accessible

		''' <summary>
		''' Returns the AccessibleContext associated with this object.  In most
		''' cases, the return value should not be null if the object implements
		''' interface Accessible.  If a component developer creates a subclass
		''' of an object that implements Accessible, and that subclass
		''' is not Accessible, the developer should override the
		''' getAccessibleContext method to return null. </summary>
		''' <returns> the AccessibleContext associated with this object </returns>
		ReadOnly Property accessibleContext As AccessibleContext
	End Interface

End Namespace