'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.plaf.basic



	''' <summary>
	''' The interface which defines the methods required for the implementation of the popup
	''' portion of a combo box.
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' 
	''' @author Tom Santos
	''' </summary>
	Public Interface ComboPopup
		''' <summary>
		''' Shows the popup
		''' </summary>
		Sub show()

		''' <summary>
		''' Hides the popup
		''' </summary>
		Sub hide()

		''' <summary>
		''' Returns true if the popup is visible (currently being displayed).
		''' </summary>
		''' <returns> <code>true</code> if the component is visible; <code>false</code> otherwise. </returns>
		ReadOnly Property visible As Boolean

		''' <summary>
		''' Returns the list that is being used to draw the items in the combo box.
		''' This method is highly implementation specific and should not be used
		''' for general list manipulation.
		''' </summary>
		ReadOnly Property list As javax.swing.JList

		''' <summary>
		''' Returns a mouse listener that will be added to the combo box or null.
		''' If this method returns null then it will not be added to the combo box.
		''' </summary>
		''' <returns> a <code>MouseListener</code> or null </returns>
		ReadOnly Property mouseListener As java.awt.event.MouseListener

		''' <summary>
		''' Returns a mouse motion listener that will be added to the combo box or null.
		''' If this method returns null then it will not be added to the combo box.
		''' </summary>
		''' <returns> a <code>MouseMotionListener</code> or null </returns>
		ReadOnly Property mouseMotionListener As java.awt.event.MouseMotionListener

		''' <summary>
		''' Returns a key listener that will be added to the combo box or null.
		''' If this method returns null then it will not be added to the combo box.
		''' </summary>
		ReadOnly Property keyListener As java.awt.event.KeyListener

		''' <summary>
		''' Called to inform the ComboPopup that the UI is uninstalling.
		''' If the ComboPopup added any listeners in the component, it should remove them here.
		''' </summary>
		Sub uninstallingUI()
	End Interface

End Namespace