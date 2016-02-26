Imports System

'
' * Copyright (c) 2005, Oracle and/or its affiliates. All rights reserved.
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
	''' <code>RowSorterEvent</code> provides notification of changes to
	''' a <code>RowSorter</code>.  Two types of notification are possible:
	''' <ul>
	''' <li><code>Type.SORT_ORDER_CHANGED</code>: indicates the sort order has
	'''     changed.  This is typically followed by a notification of:
	''' <li><code>Type.SORTED</code>: indicates the contents of the model have
	'''     been transformed in some way.  For example, the contents may have
	'''     been sorted or filtered.
	''' </ul>
	''' </summary>
	''' <seealso cref= javax.swing.RowSorter
	''' @since 1.6 </seealso>
	Public Class RowSorterEvent
		Inherits java.util.EventObject

		Private type As Type
		Private oldViewToModel As Integer()

		''' <summary>
		''' Enumeration of the types of <code>RowSorterEvent</code>s.
		''' 
		''' @since 1.6
		''' </summary>
		Public Enum Type
			''' <summary>
			''' Indicates the sort order has changed.
			''' </summary>
			SORT_ORDER_CHANGED

			''' <summary>
			''' Indicates the contents have been newly sorted or
			''' transformed in some way.
			''' </summary>
			SORTED
		End Enum

		''' <summary>
		''' Creates a <code>RowSorterEvent</code> of type
		''' <code>SORT_ORDER_CHANGED</code>.
		''' </summary>
		''' <param name="source"> the source of the change </param>
		''' <exception cref="IllegalArgumentException"> if <code>source</code> is
		'''         <code>null</code> </exception>
		Public Sub New(ByVal source As javax.swing.RowSorter)
			Me.New(source, Type.SORT_ORDER_CHANGED, Nothing)
		End Sub

		''' <summary>
		''' Creates a <code>RowSorterEvent</code>.
		''' </summary>
		''' <param name="source"> the source of the change </param>
		''' <param name="type"> the type of event </param>
		''' <param name="previousRowIndexToModel"> the mapping from model indices to
		'''        view indices prior to the sort, may be <code>null</code> </param>
		''' <exception cref="IllegalArgumentException"> if source or <code>type</code> is
		'''         <code>null</code> </exception>
		Public Sub New(ByVal source As javax.swing.RowSorter, ByVal type As Type, ByVal previousRowIndexToModel As Integer())
			MyBase.New(source)
			If type Is Nothing Then Throw New System.ArgumentException("type must be non-null")
			Me.type = type
			Me.oldViewToModel = previousRowIndexToModel
		End Sub

		''' <summary>
		''' Returns the source of the event as a <code>RowSorter</code>.
		''' </summary>
		''' <returns> the source of the event as a <code>RowSorter</code> </returns>
		Public Overridable Property source As javax.swing.RowSorter
			Get
				Return CType(MyBase.source, javax.swing.RowSorter)
			End Get
		End Property

		''' <summary>
		''' Returns the type of event.
		''' </summary>
		''' <returns> the type of event </returns>
		Public Overridable Property type As Type
			Get
				Return type
			End Get
		End Property

		''' <summary>
		''' Returns the location of <code>index</code> in terms of the
		''' model prior to the sort.  This method is only useful for events
		''' of type <code>SORTED</code>.  This method will return -1 if the
		''' index is not valid, or the locations prior to the sort have not
		''' been provided.
		''' </summary>
		''' <param name="index"> the index in terms of the view </param>
		''' <returns> the index in terms of the model prior to the sort, or -1 if
		'''         the location is not valid or the mapping was not provided. </returns>
		Public Overridable Function convertPreviousRowIndexToModel(ByVal index As Integer) As Integer
			If oldViewToModel IsNot Nothing AndAlso index >= 0 AndAlso index < oldViewToModel.Length Then Return oldViewToModel(index)
			Return -1
		End Function

		''' <summary>
		''' Returns the number of rows before the sort.  This method is only
		''' useful for events of type <code>SORTED</code> and if the
		''' last locations have not been provided will return 0.
		''' </summary>
		''' <returns> the number of rows in terms of the view prior to the sort </returns>
		Public Overridable Property previousRowCount As Integer
			Get
				Return If(oldViewToModel Is Nothing, 0, oldViewToModel.Length)
			End Get
		End Property
	End Class

End Namespace