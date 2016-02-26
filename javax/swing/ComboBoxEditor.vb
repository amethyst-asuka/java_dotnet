'
' * Copyright (c) 1997, 1998, Oracle and/or its affiliates. All rights reserved.
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
	''' The editor component used for JComboBox components.
	''' 
	''' @author Arnaud Weber
	''' </summary>
	Public Interface ComboBoxEditor

	  ''' <summary>
	  ''' Return the component that should be added to the tree hierarchy for
	  ''' this editor
	  ''' </summary>
	  ReadOnly Property editorComponent As Component

	  ''' <summary>
	  ''' Set the item that should be edited. Cancel any editing if necessary * </summary>
	  Property item As Object


	  ''' <summary>
	  ''' Ask the editor to start editing and to select everything * </summary>
	  Sub selectAll()

	  ''' <summary>
	  ''' Add an ActionListener. An action event is generated when the edited item changes * </summary>
	  Sub addActionListener(ByVal l As ActionListener)

	  ''' <summary>
	  ''' Remove an ActionListener * </summary>
	  Sub removeActionListener(ByVal l As ActionListener)
	End Interface

End Namespace