'
' * Copyright (c) 1999, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' The AccessibleIcon interface should be supported by any object
	''' that has an associated icon (e.g., buttons). This interface
	''' provides the standard mechanism for an assistive technology
	''' to get descriptive information about icons.
	''' Applications can determine
	''' if an object supports the AccessibleIcon interface by first
	''' obtaining its AccessibleContext (see
	''' <seealso cref="Accessible"/>) and then calling the
	''' <seealso cref="AccessibleContext#getAccessibleIcon"/> method.
	''' If the return value is not null, the object supports this interface.
	''' </summary>
	''' <seealso cref= Accessible </seealso>
	''' <seealso cref= AccessibleContext
	''' 
	''' @author      Lynn Monsanto
	''' @since 1.3 </seealso>
	Public Interface AccessibleIcon

		''' <summary>
		''' Gets the description of the icon.  This is meant to be a brief
		''' textual description of the object.  For example, it might be
		''' presented to a blind user to give an indication of the purpose
		''' of the icon.
		''' </summary>
		''' <returns> the description of the icon </returns>
		Property accessibleIconDescription As String


		''' <summary>
		''' Gets the width of the icon
		''' </summary>
		''' <returns> the width of the icon. </returns>
		ReadOnly Property accessibleIconWidth As Integer

		''' <summary>
		''' Gets the height of the icon
		''' </summary>
		''' <returns> the height of the icon. </returns>
		ReadOnly Property accessibleIconHeight As Integer

	End Interface

End Namespace