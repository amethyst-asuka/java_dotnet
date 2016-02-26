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
	''' The AccessibleTableModelChange interface describes a change to
	''' the table model.  The attributes of the model change can be
	''' obtained by the following methods:
	''' <ul>
	''' <li> public int getType()
	''' <li> public int getFirstRow();
	''' <li> public int getLastRow();
	''' <li> public int getFirstColumn();
	''' <li> public int getLastColumn();
	''' </ul>
	''' The model change type returned by getType() will be one of:
	''' <ul>
	''' <li> INSERT - one or more rows and/or columns have been inserted
	''' <li> UPDATE - some of the table data has changed
	''' <li> DELETE - one or more rows and/or columns have been deleted
	''' </ul>
	''' The affected area of the table can be determined by the other
	''' four methods which specify ranges of rows and columns
	''' </summary>
	''' <seealso cref= Accessible </seealso>
	''' <seealso cref= Accessible#getAccessibleContext </seealso>
	''' <seealso cref= AccessibleContext </seealso>
	''' <seealso cref= AccessibleContext#getAccessibleTable
	''' 
	''' @author      Lynn Monsanto
	''' @since 1.3 </seealso>
	Public Interface AccessibleTableModelChange

		''' <summary>
		''' Identifies the insertion of new rows and/or columns.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int INSERT = 1;

		''' <summary>
		''' Identifies a change to existing data.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int UPDATE = 0;

		''' <summary>
		''' Identifies the deletion of rows and/or columns.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int DELETE = -1;

		''' <summary>
		'''  Returns the type of event. </summary>
		'''  <returns> the type of event </returns>
		'''  <seealso cref= #INSERT </seealso>
		'''  <seealso cref= #UPDATE </seealso>
		'''  <seealso cref= #DELETE </seealso>
		Function [getType]() As Integer

		''' <summary>
		''' Returns the first row that changed. </summary>
		''' <returns> the first row that changed </returns>
		ReadOnly Property firstRow As Integer

		''' <summary>
		''' Returns the last row that changed. </summary>
		''' <returns> the last row that changed </returns>
		ReadOnly Property lastRow As Integer

		''' <summary>
		''' Returns the first column that changed. </summary>
		''' <returns> the first column that changed </returns>
		ReadOnly Property firstColumn As Integer

		''' <summary>
		''' Returns the last column that changed. </summary>
		''' <returns> the last column that changed </returns>
		ReadOnly Property lastColumn As Integer
	End Interface

End Namespace