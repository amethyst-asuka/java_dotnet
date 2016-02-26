Imports System
Imports javax.swing.text

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
Namespace javax.swing.text.html

	''' <summary>
	''' Value for the ListModel used to represent
	''' &lt;option&gt; elements.  This is the object
	''' installed as items of the DefaultComboBoxModel
	''' used to represent the &lt;select&gt; element.
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
	''' @author  Timothy Prinzing
	''' </summary>
	<Serializable> _
	Public Class [Option]

		''' <summary>
		''' Creates a new Option object.
		''' </summary>
		''' <param name="attr"> the attributes associated with the
		'''  option element.  The attributes are copied to
		'''  ensure they won't change. </param>
		Public Sub New(ByVal attr As AttributeSet)
			Me.attr = attr.copyAttributes()
			selected = (attr.getAttribute(HTML.Attribute.SELECTED) IsNot Nothing)
		End Sub

		''' <summary>
		''' Sets the label to be used for the option.
		''' </summary>
		Public Overridable Property label As String
			Set(ByVal label As String)
				Me.label = label
			End Set
			Get
				Return label
			End Get
		End Property


		''' <summary>
		''' Fetch the attributes associated with this option.
		''' </summary>
		Public Overridable Property attributes As AttributeSet
			Get
				Return attr
			End Get
		End Property

		''' <summary>
		''' String representation is the label.
		''' </summary>
		Public Overrides Function ToString() As String
			Return label
		End Function

		''' <summary>
		''' Sets the selected state.
		''' </summary>
		Protected Friend Overridable Property selection As Boolean
			Set(ByVal state As Boolean)
				selected = state
			End Set
		End Property

		''' <summary>
		''' Fetches the selection state associated with this option.
		''' </summary>
		Public Overridable Property selected As Boolean
			Get
				Return selected
			End Get
		End Property

		''' <summary>
		''' Convenience method to return the string associated
		''' with the <code>value</code> attribute.  If the
		''' value has not been specified, the label will be
		''' returned.
		''' </summary>
		Public Overridable Property value As String
			Get
				Dim ___value As String = CStr(attr.getAttribute(HTML.Attribute.VALUE))
				If ___value Is Nothing Then ___value = label
				Return ___value
			End Get
		End Property

		Private selected As Boolean
		Private label As String
		Private attr As AttributeSet
	End Class

End Namespace