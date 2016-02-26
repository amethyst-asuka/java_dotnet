'
' * Copyright (c) 1997, 2001, Oracle and/or its affiliates. All rights reserved.
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
	''' This interface defines the methods components like JList use
	''' to get the value of each cell in a list and the length of the list.
	''' Logically the model is a vector, indices vary from 0 to
	''' ListDataModel.getSize() - 1.  Any change to the contents or
	''' length of the data model must be reported to all of the
	''' ListDataListeners.
	''' </summary>
	''' @param <E> the type of the elements of this model
	''' 
	''' @author Hans Muller </param>
	''' <seealso cref= JList </seealso>
	Public Interface ListModel(Of E)
	  ''' <summary>
	  ''' Returns the length of the list. </summary>
	  ''' <returns> the length of the list </returns>
	  ReadOnly Property size As Integer

	  ''' <summary>
	  ''' Returns the value at the specified index. </summary>
	  ''' <param name="index"> the requested index </param>
	  ''' <returns> the value at <code>index</code> </returns>
	  Function getElementAt(ByVal index As Integer) As E

	  ''' <summary>
	  ''' Adds a listener to the list that's notified each time a change
	  ''' to the data model occurs. </summary>
	  ''' <param name="l"> the <code>ListDataListener</code> to be added </param>
	  Sub addListDataListener(ByVal l As javax.swing.event.ListDataListener)

	  ''' <summary>
	  ''' Removes a listener from the list that's notified each time a
	  ''' change to the data model occurs. </summary>
	  ''' <param name="l"> the <code>ListDataListener</code> to be removed </param>
	  Sub removeListDataListener(ByVal l As javax.swing.event.ListDataListener)
	End Interface

End Namespace