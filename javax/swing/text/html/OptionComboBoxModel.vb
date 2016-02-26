Imports System
Imports javax.swing

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
Namespace javax.swing.text.html



	''' <summary>
	''' OptionComboBoxModel extends the capabilities of the DefaultComboBoxModel,
	''' to store the Option that is initially marked as selected.
	''' This is stored, in order to enable an accurate reset of the
	''' ComboBox that represents the SELECT form element when the
	''' user requests a clear/reset.  Given that a combobox only allow
	''' for one item to be selected, the last OPTION that has the
	''' attribute set wins.
	''' 
	'''  @author Sunita Mani
	''' </summary>

	<Serializable> _
	Friend Class OptionComboBoxModel(Of E)
		Inherits DefaultComboBoxModel(Of E)

		Private selectedOption As [Option] = Nothing

		''' <summary>
		''' Stores the Option that has been marked its
		''' selected attribute set.
		''' </summary>
		Public Overridable Property initialSelection As [Option]
			Set(ByVal [option] As [Option])
				selectedOption = [option]
			End Set
			Get
				Return selectedOption
			End Get
		End Property

	End Class

End Namespace