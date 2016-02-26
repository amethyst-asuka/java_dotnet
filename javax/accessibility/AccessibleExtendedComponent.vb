'
' * Copyright (c) 2000, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' The AccessibleExtendedComponent interface should be supported by any object
	''' that is rendered on the screen.  This interface provides the standard
	''' mechanism for an assistive technology to determine the extended
	''' graphical representation of an object.  Applications can determine
	''' if an object supports the AccessibleExtendedComponent interface by first
	''' obtaining its AccessibleContext
	''' and then calling the
	''' <seealso cref="AccessibleContext#getAccessibleComponent"/> method.
	''' If the return value is not null and the type of the return value is
	''' AccessibleExtendedComponent, the object supports this interface.
	''' </summary>
	''' <seealso cref= Accessible </seealso>
	''' <seealso cref= Accessible#getAccessibleContext </seealso>
	''' <seealso cref= AccessibleContext </seealso>
	''' <seealso cref= AccessibleContext#getAccessibleComponent
	''' 
	''' @author      Lynn Monsanto
	''' @since 1.4 </seealso>
	Public Interface AccessibleExtendedComponent
		Inherits AccessibleComponent

		''' <summary>
		''' Returns the tool tip text
		''' </summary>
		''' <returns> the tool tip text, if supported, of the object;
		''' otherwise, null </returns>
		ReadOnly Property toolTipText As String

		''' <summary>
		''' Returns the titled border text
		''' </summary>
		''' <returns> the titled border text, if supported, of the object;
		''' otherwise, null </returns>
		ReadOnly Property titledBorderText As String

		''' <summary>
		''' Returns key bindings associated with this object
		''' </summary>
		''' <returns> the key bindings, if supported, of the object;
		''' otherwise, null </returns>
		''' <seealso cref= AccessibleKeyBinding </seealso>
		ReadOnly Property accessibleKeyBinding As AccessibleKeyBinding
	End Interface

End Namespace