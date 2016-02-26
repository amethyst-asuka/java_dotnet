'
' * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Class AccessibleTable describes a user-interface component that
	''' presents data in a two-dimensional table format.
	''' 
	''' @author      Lynn Monsanto
	''' @since 1.3
	''' </summary>
	Public Interface AccessibleTable

		''' <summary>
		''' Returns the caption for the table.
		''' </summary>
		''' <returns> the caption for the table </returns>
		Property accessibleCaption As Accessible


		''' <summary>
		''' Returns the summary description of the table.
		''' </summary>
		''' <returns> the summary description of the table </returns>
		Property accessibleSummary As Accessible


		''' <summary>
		''' Returns the number of rows in the table.
		''' </summary>
		''' <returns> the number of rows in the table </returns>
		ReadOnly Property accessibleRowCount As Integer

		''' <summary>
		''' Returns the number of columns in the table.
		''' </summary>
		''' <returns> the number of columns in the table </returns>
		ReadOnly Property accessibleColumnCount As Integer

		''' <summary>
		''' Returns the Accessible at a specified row and column
		''' in the table.
		''' </summary>
		''' <param name="r"> zero-based row of the table </param>
		''' <param name="c"> zero-based column of the table </param>
		''' <returns> the Accessible at the specified row and column </returns>
		Function getAccessibleAt(ByVal r As Integer, ByVal c As Integer) As Accessible

		''' <summary>
		''' Returns the number of rows occupied by the Accessible at
		''' a specified row and column in the table.
		''' </summary>
		''' <param name="r"> zero-based row of the table </param>
		''' <param name="c"> zero-based column of the table </param>
		''' <returns> the number of rows occupied by the Accessible at a
		''' given specified (row, column) </returns>
		Function getAccessibleRowExtentAt(ByVal r As Integer, ByVal c As Integer) As Integer

		''' <summary>
		''' Returns the number of columns occupied by the Accessible at
		''' a specified row and column in the table.
		''' </summary>
		''' <param name="r"> zero-based row of the table </param>
		''' <param name="c"> zero-based column of the table </param>
		''' <returns> the number of columns occupied by the Accessible at a
		''' given specified row and column </returns>
		Function getAccessibleColumnExtentAt(ByVal r As Integer, ByVal c As Integer) As Integer

		''' <summary>
		''' Returns the row headers as an AccessibleTable.
		''' </summary>
		''' <returns> an AccessibleTable representing the row
		''' headers </returns>
		Property accessibleRowHeader As AccessibleTable


		''' <summary>
		''' Returns the column headers as an AccessibleTable.
		''' </summary>
		''' <returns> an AccessibleTable representing the column
		''' headers </returns>
		Property accessibleColumnHeader As AccessibleTable


		''' <summary>
		''' Returns the description of the specified row in the table.
		''' </summary>
		''' <param name="r"> zero-based row of the table </param>
		''' <returns> the description of the row </returns>
		Function getAccessibleRowDescription(ByVal r As Integer) As Accessible

		''' <summary>
		''' Sets the description text of the specified row of the table.
		''' </summary>
		''' <param name="r"> zero-based row of the table </param>
		''' <param name="a"> the description of the row </param>
		Sub setAccessibleRowDescription(ByVal r As Integer, ByVal a As Accessible)

		''' <summary>
		''' Returns the description text of the specified column in the table.
		''' </summary>
		''' <param name="c"> zero-based column of the table </param>
		''' <returns> the text description of the column </returns>
		Function getAccessibleColumnDescription(ByVal c As Integer) As Accessible

		''' <summary>
		''' Sets the description text of the specified column in the table.
		''' </summary>
		''' <param name="c"> zero-based column of the table </param>
		''' <param name="a"> the text description of the column </param>
		Sub setAccessibleColumnDescription(ByVal c As Integer, ByVal a As Accessible)

		''' <summary>
		''' Returns a boolean value indicating whether the accessible at
		''' a specified row and column is selected.
		''' </summary>
		''' <param name="r"> zero-based row of the table </param>
		''' <param name="c"> zero-based column of the table </param>
		''' <returns> the boolean value true if the accessible at the
		''' row and column is selected. Otherwise, the boolean value
		''' false </returns>
		Function isAccessibleSelected(ByVal r As Integer, ByVal c As Integer) As Boolean

		''' <summary>
		''' Returns a boolean value indicating whether the specified row
		''' is selected.
		''' </summary>
		''' <param name="r"> zero-based row of the table </param>
		''' <returns> the boolean value true if the specified row is selected.
		''' Otherwise, false. </returns>
		Function isAccessibleRowSelected(ByVal r As Integer) As Boolean

		''' <summary>
		''' Returns a boolean value indicating whether the specified column
		''' is selected.
		''' </summary>
		''' <param name="c"> zero-based column of the table </param>
		''' <returns> the boolean value true if the specified column is selected.
		''' Otherwise, false. </returns>
		Function isAccessibleColumnSelected(ByVal c As Integer) As Boolean

		''' <summary>
		''' Returns the selected rows in a table.
		''' </summary>
		''' <returns> an array of selected rows where each element is a
		''' zero-based row of the table </returns>
		ReadOnly Property selectedAccessibleRows As Integer()

		''' <summary>
		''' Returns the selected columns in a table.
		''' </summary>
		''' <returns> an array of selected columns where each element is a
		''' zero-based column of the table </returns>
		ReadOnly Property selectedAccessibleColumns As Integer()
	End Interface

End Namespace