Imports javax.swing.event

'
' * Copyright (c) 1997, 2002, Oracle and/or its affiliates. All rights reserved.
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
	''' A model that supports at most one indexed selection.
	''' 
	''' @author Dave Moore
	''' </summary>
	Public Interface SingleSelectionModel
		''' <summary>
		''' Returns the model's selection.
		''' </summary>
		''' <returns>  the model's selection, or -1 if there is no selection </returns>
		''' <seealso cref=     #setSelectedIndex </seealso>
		Property selectedIndex As Integer


		''' <summary>
		''' Clears the selection (to -1).
		''' </summary>
		Sub clearSelection()

		''' <summary>
		''' Returns true if the selection model currently has a selected value. </summary>
		''' <returns> true if a value is currently selected </returns>
		ReadOnly Property selected As Boolean

		''' <summary>
		''' Adds <I>listener</I> as a listener to changes in the model. </summary>
		''' <param name="listener"> the ChangeListener to add </param>
		Sub addChangeListener(ByVal listener As ChangeListener)

		''' <summary>
		''' Removes <I>listener</I> as a listener to changes in the model. </summary>
		''' <param name="listener"> the ChangeListener to remove </param>
		Sub removeChangeListener(ByVal listener As ChangeListener)
	End Interface

End Namespace