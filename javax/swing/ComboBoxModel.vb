'
' * Copyright (c) 1997, 2011, Oracle and/or its affiliates. All rights reserved.
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
	''' A data model for a combo box. This interface extends <code>ListDataModel</code>
	''' and adds the concept of a <i>selected item</i>. The selected item is generally
	''' the item which is visible in the combo box display area.
	''' <p>
	''' The selected item may not necessarily be managed by the underlying
	''' <code>ListModel</code>. This disjoint behavior allows for the temporary
	''' storage and retrieval of a selected item in the model.
	''' </summary>
	''' @param <E> the type of the elements of this model
	''' 
	''' @author Arnaud Weber </param>
	Public Interface ComboBoxModel(Of E)
		Inherits ListModel(Of E)

	  ''' <summary>
	  ''' Set the selected item. The implementation of this  method should notify
	  ''' all registered <code>ListDataListener</code>s that the contents
	  ''' have changed.
	  ''' </summary>
	  ''' <param name="anItem"> the list object to select or <code>null</code>
	  '''        to clear the selection </param>
	  Property selectedItem As Object

	End Interface

End Namespace