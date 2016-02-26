'
' * Copyright (c) 1998, 2011, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing

	''' <summary>
	''' A mutable version of <code>ComboBoxModel</code>.
	''' </summary>
	''' @param <E> the type of the elements of this model
	''' 
	''' @author Tom Santos </param>

	Public Interface MutableComboBoxModel(Of E)
		Inherits ComboBoxModel(Of E)

		''' <summary>
		''' Adds an item at the end of the model. The implementation of this method
		''' should notify all registered <code>ListDataListener</code>s that the
		''' item has been added.
		''' </summary>
		''' <param name="item"> the item to be added </param>
		Sub addElement(ByVal item As E)

		''' <summary>
		''' Removes an item from the model. The implementation of this method should
		''' should notify all registered <code>ListDataListener</code>s that the
		''' item has been removed.
		''' </summary>
		''' <param name="obj"> the <code>Object</code> to be removed </param>
		Sub removeElement(ByVal obj As Object)

		''' <summary>
		''' Adds an item at a specific index.  The implementation of this method
		''' should notify all registered <code>ListDataListener</code>s that the
		''' item has been added.
		''' </summary>
		''' <param name="item">  the item to be added </param>
		''' <param name="index">  location to add the object </param>
		Sub insertElementAt(ByVal item As E, ByVal index As Integer)

		''' <summary>
		''' Removes an item at a specific index. The implementation of this method
		''' should notify all registered <code>ListDataListener</code>s that the
		''' item has been removed.
		''' </summary>
		''' <param name="index">  location of the item to be removed </param>
		Sub removeElementAt(ByVal index As Integer)
	End Interface

End Namespace