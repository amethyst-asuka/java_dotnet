'
' * Copyright (c) 1997, 1999, Oracle and/or its affiliates. All rights reserved.
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
	''' This AccessibleSelection interface
	''' provides the standard mechanism for an assistive technology to determine
	''' what the current selected children are, as well as modify the selection set.
	''' Any object that has children that can be selected should support
	''' the AccessibleSelection interface.  Applications can determine if an object supports the
	''' AccessibleSelection interface by first obtaining its AccessibleContext (see
	''' <seealso cref="Accessible"/>) and then calling the
	''' <seealso cref="AccessibleContext#getAccessibleSelection"/> method.
	''' If the return value is not null, the object supports this interface.
	''' </summary>
	''' <seealso cref= Accessible </seealso>
	''' <seealso cref= Accessible#getAccessibleContext </seealso>
	''' <seealso cref= AccessibleContext </seealso>
	''' <seealso cref= AccessibleContext#getAccessibleSelection
	''' 
	''' @author      Peter Korn
	''' @author      Hans Muller
	''' @author      Willie Walker </seealso>
	Public Interface AccessibleSelection

		''' <summary>
		''' Returns the number of Accessible children currently selected.
		''' If no children are selected, the return value will be 0.
		''' </summary>
		''' <returns> the number of items currently selected. </returns>
		 ReadOnly Property accessibleSelectionCount As Integer

		''' <summary>
		''' Returns an Accessible representing the specified selected child
		''' of the object.  If there isn't a selection, or there are
		''' fewer children selected than the integer passed in, the return
		''' value will be null.
		''' <p>Note that the index represents the i-th selected child, which
		''' is different from the i-th child.
		''' </summary>
		''' <param name="i"> the zero-based index of selected children </param>
		''' <returns> the i-th selected child </returns>
		''' <seealso cref= #getAccessibleSelectionCount </seealso>
		 Function getAccessibleSelection(ByVal i As Integer) As Accessible

		''' <summary>
		''' Determines if the current child of this object is selected.
		''' </summary>
		''' <returns> true if the current child of this object is selected; else false. </returns>
		''' <param name="i"> the zero-based index of the child in this Accessible object. </param>
		''' <seealso cref= AccessibleContext#getAccessibleChild </seealso>
		 Function isAccessibleChildSelected(ByVal i As Integer) As Boolean

		''' <summary>
		''' Adds the specified Accessible child of the object to the object's
		''' selection.  If the object supports multiple selections,
		''' the specified child is added to any existing selection, otherwise
		''' it replaces any existing selection in the object.  If the
		''' specified child is already selected, this method has no effect.
		''' </summary>
		''' <param name="i"> the zero-based index of the child </param>
		''' <seealso cref= AccessibleContext#getAccessibleChild </seealso>
		 Sub addAccessibleSelection(ByVal i As Integer)

		''' <summary>
		''' Removes the specified child of the object from the object's
		''' selection.  If the specified item isn't currently selected, this
		''' method has no effect.
		''' </summary>
		''' <param name="i"> the zero-based index of the child </param>
		''' <seealso cref= AccessibleContext#getAccessibleChild </seealso>
		 Sub removeAccessibleSelection(ByVal i As Integer)

		''' <summary>
		''' Clears the selection in the object, so that no children in the
		''' object are selected.
		''' </summary>
		 Sub clearAccessibleSelection()

		''' <summary>
		''' Causes every child of the object to be selected
		''' if the object supports multiple selections.
		''' </summary>
		 Sub selectAllAccessibleSelection()
	End Interface

End Namespace