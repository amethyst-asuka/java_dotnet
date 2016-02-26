'
' * Copyright (c) 2001, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.accessibility

	''' <summary>
	''' Class AccessibleExtendedTable provides extended information about
	''' a user-interface component that presents data in a two-dimensional
	''' table format.
	''' Applications can determine if an object supports the
	''' AccessibleExtendedTable interface by first obtaining its
	''' AccessibleContext and then calling the
	''' <seealso cref="AccessibleContext#getAccessibleTable"/> method.
	''' If the return value is not null and the type of the return value is
	''' AccessibleExtendedTable, the object supports this interface.
	'''  
	''' @author      Lynn Monsanto
	''' @since 1.4
	''' </summary>
	Public Interface AccessibleExtendedTable
		Inherits AccessibleTable

		 ''' <summary>
		 ''' Returns the row number of an index in the table.
		 ''' </summary>
		 ''' <param name="index"> the zero-based index in the table.  The index is
		 ''' the table cell offset from row == 0 and column == 0. </param>
		 ''' <returns> the zero-based row of the table if one exists;
		 ''' otherwise -1. </returns>
		 Function getAccessibleRow(ByVal index As Integer) As Integer

		 ''' <summary>
		 ''' Returns the column number of an index in the table.
		 ''' </summary>
		 ''' <param name="index"> the zero-based index in the table.  The index is
		 ''' the table cell offset from row == 0 and column == 0. </param>
		 ''' <returns> the zero-based column of the table if one exists;
		 ''' otherwise -1. </returns>
		 Function getAccessibleColumn(ByVal index As Integer) As Integer

		''' <summary>
		''' Returns the index at a row and column in the table.
		''' </summary>
		''' <param name="r"> zero-based row of the table </param>
		''' <param name="c"> zero-based column of the table </param>
		''' <returns> the zero-based index in the table if one exists;
		''' otherwise -1.  The index is  the table cell offset from
		''' row == 0 and column == 0. </returns>
		 Function getAccessibleIndex(ByVal r As Integer, ByVal c As Integer) As Integer
	End Interface

End Namespace