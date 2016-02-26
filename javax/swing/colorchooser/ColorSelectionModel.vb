Imports javax.swing
Imports javax.swing.event

'
' * Copyright (c) 1998, 2001, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.colorchooser


	''' <summary>
	''' A model that supports selecting a <code>Color</code>.
	''' 
	''' @author Steve Wilson
	''' </summary>
	''' <seealso cref= java.awt.Color </seealso>
	Public Interface ColorSelectionModel
		''' <summary>
		''' Returns the selected <code>Color</code> which should be
		''' non-<code>null</code>.
		''' </summary>
		''' <returns>  the selected <code>Color</code> </returns>
		''' <seealso cref=     #setSelectedColor </seealso>
		Property selectedColor As java.awt.Color


		''' <summary>
		''' Adds <code>listener</code> as a listener to changes in the model. </summary>
		''' <param name="listener"> the <code>ChangeListener</code> to be added </param>
		Sub addChangeListener(ByVal listener As ChangeListener)

		''' <summary>
		''' Removes <code>listener</code> as a listener to changes in the model. </summary>
		''' <param name="listener"> the <code>ChangeListener</code> to be removed </param>
		Sub removeChangeListener(ByVal listener As ChangeListener)
	End Interface

End Namespace