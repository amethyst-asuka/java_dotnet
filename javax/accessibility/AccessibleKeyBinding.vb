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
	''' The AccessibleKeyBinding interface should be supported by any object
	''' that has a keyboard bindings such as a keyboard mnemonic and/or keyboard
	''' shortcut which can be used to select the object.  This interface provides
	''' the standard mechanism for an assistive technology to determine the
	''' key bindings which exist for this object.
	''' Any object that has such key bindings should support this
	''' interface.
	''' </summary>
	''' <seealso cref= Accessible </seealso>
	''' <seealso cref= Accessible#getAccessibleContext </seealso>
	''' <seealso cref= AccessibleContext
	''' 
	''' @author      Lynn Monsanto
	''' @since 1.4 </seealso>
	Public Interface AccessibleKeyBinding

		''' <summary>
		''' Returns the number of key bindings for this object
		''' </summary>
		''' <returns> the zero-based number of key bindings for this object </returns>
		ReadOnly Property accessibleKeyBindingCount As Integer

		''' <summary>
		''' Returns a key binding for this object.  The value returned is an
		''' java.lang.Object which must be cast to appropriate type depending
		''' on the underlying implementation of the key.
		''' </summary>
		''' <param name="i"> zero-based index of the key bindings </param>
		''' <returns> a javax.lang.Object which specifies the key binding </returns>
		''' <seealso cref= #getAccessibleKeyBindingCount </seealso>
		Function getAccessibleKeyBinding(ByVal i As Integer) As Object
	End Interface

End Namespace