Imports javax.swing.table

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.event

	''' <summary>
	''' <B>TableColumnModelEvent</B> is used to notify listeners that a table
	''' column model has changed, such as a column was added, removed, or
	''' moved.
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
	''' @author Alan Chung </summary>
	''' <seealso cref= TableColumnModelListener </seealso>
	Public Class TableColumnModelEvent
		Inherits java.util.EventObject

	'
	'  Instance Variables
	'

		''' <summary>
		''' The index of the column from where it was moved or removed </summary>
		Protected Friend fromIndex As Integer

		''' <summary>
		''' The index of the column to where it was moved or added </summary>
		Protected Friend toIndex As Integer

	'
	' Constructors
	'

		''' <summary>
		''' Constructs a {@code TableColumnModelEvent} object.
		''' </summary>
		''' <param name="source">  the {@code TableColumnModel} that originated the event </param>
		''' <param name="from">    an int specifying the index from where the column was
		'''                moved or removed </param>
		''' <param name="to">      an int specifying the index to where the column was
		'''                moved or added </param>
		''' <seealso cref= #getFromIndex </seealso>
		''' <seealso cref= #getToIndex </seealso>
		Public Sub New(ByVal source As TableColumnModel, ByVal [from] As Integer, ByVal [to] As Integer)
			MyBase.New(source)
			fromIndex = [from]
			toIndex = [to]
		End Sub

	'
	' Querying Methods
	'

		''' <summary>
		''' Returns the fromIndex.  Valid for removed or moved events </summary>
		Public Overridable Property fromIndex As Integer
			Get
				Return fromIndex
			End Get
		End Property

		''' <summary>
		''' Returns the toIndex.  Valid for add and moved events </summary>
		Public Overridable Property toIndex As Integer
			Get
				Return toIndex
			End Get
		End Property
	End Class

End Namespace