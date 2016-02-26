Imports System
Imports javax.sql

'
' * Copyright (c) 2003, 2014, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.sql.rowset



	''' <summary>
	''' Provides implementations for the methods that set and get
	''' metadata information about a <code>RowSet</code> object's columns.
	''' A <code>RowSetMetaDataImpl</code> object keeps track of the
	''' number of columns in the rowset and maintains an internal array
	''' of column attributes for each column.
	''' <P>
	''' A <code>RowSet</code> object creates a <code>RowSetMetaDataImpl</code>
	''' object internally in order to set and retrieve information about
	''' its columns.
	''' <P>
	''' NOTE: All metadata in a <code>RowSetMetaDataImpl</code> object
	''' should be considered as unavailable until the <code>RowSet</code> object
	''' that it describes is populated.
	''' Therefore, any <code>RowSetMetaDataImpl</code> method that retrieves information
	''' is defined as having unspecified behavior when it is called
	''' before the <code>RowSet</code> object contains data.
	''' 
	''' @since 1.5
	''' </summary>
	<Serializable> _
	Public Class RowSetMetaDataImpl
		Implements RowSetMetaData

		''' <summary>
		''' The number of columns in the <code>RowSet</code> object that created
		''' this <code>RowSetMetaDataImpl</code> object.
		''' @serial
		''' </summary>
		Private colCount As Integer

		''' <summary>
		''' An array of <code>ColInfo</code> objects used to store information
		''' about each column in the <code>RowSet</code> object for which
		''' this <code>RowSetMetaDataImpl</code> object was created. The first
		''' <code>ColInfo</code> object in this array contains information about
		''' the first column in the <code>RowSet</code> object, the second element
		''' contains information about the second column, and so on.
		''' @serial
		''' </summary>
		Private colInfo As ColInfo()

		''' <summary>
		''' Checks to see that the designated column is a valid column number for
		''' the <code>RowSet</code> object for which this <code>RowSetMetaDataImpl</code>
		''' was created. To be valid, a column number must be greater than
		''' <code>0</code> and less than or equal to the number of columns in a row. </summary>
		''' @throws <code>SQLException</code> with the message "Invalid column index"
		'''        if the given column number is out of the range of valid column
		'''        numbers for the <code>RowSet</code> object </exception>
		Private Sub checkColRange(ByVal col As Integer)
			If col <= 0 OrElse col > colCount Then Throw New SQLException("Invalid column index :" & col)
		End Sub

		''' <summary>
		''' Checks to see that the given SQL type is a valid column type and throws an
		''' <code>SQLException</code> object if it is not.
		''' To be valid, a SQL type must be one of the constant values
		''' in the <code><a href="../../sql/Types.html">java.sql.Types</a></code>
		''' class.
		''' </summary>
		''' <param name="SQLType"> an <code>int</code> defined in the class <code>java.sql.Types</code> </param>
		''' <exception cref="SQLException"> if the given <code>int</code> is not a constant defined in the
		'''         class <code>java.sql.Types</code> </exception>
		Private Sub checkColType(ByVal SQLType As Integer)
			Try
				Dim c As Type = GetType(java.sql.Types)
				Dim publicFields As Field() = c.GetFields()
				Dim fieldValue As Integer = 0
				For i As Integer = 0 To publicFields.Length - 1
					fieldValue = publicFields(i).getInt(c)
					If fieldValue = SQLType Then Return
				Next i
			Catch e As Exception
				Throw New SQLException(e.Message)
			End Try
			Throw New SQLException("Invalid SQL type for column")
		End Sub

		''' <summary>
		''' Sets to the given number the number of columns in the <code>RowSet</code>
		''' object for which this <code>RowSetMetaDataImpl</code> object was created.
		''' </summary>
		''' <param name="columnCount"> an <code>int</code> giving the number of columns in the
		'''        <code>RowSet</code> object </param>
		''' <exception cref="SQLException"> if the given number is equal to or less than zero </exception>
		Public Overridable Property columnCount Implements RowSetMetaData.setColumnCount As Integer
			Set(ByVal columnCount As Integer)
    
				If columnCount <= 0 Then Throw New SQLException("Invalid column count. Cannot be less " & "or equal to zero")
    
			   colCount = columnCount
    
			   ' If the colCount is Integer.MAX_VALUE,
			   ' we do not initialize the colInfo object.
			   ' even if we try to initialize the colCount with
			   ' colCount = Integer.MAx_VALUE-1, the colInfo
			   ' initialization fails throwing an ERROR
			   ' OutOfMemory Exception. So we do not initialize
			   ' colInfo at Integer.MAX_VALUE. This is to pass TCK.
    
			   If Not(colCount = Integer.MaxValue) Then
					colInfo = New ColInfo(colCount){}
    
				   For i As Integer = 1 To colCount
						 colInfo(i) = New ColInfo(Me)
				   Next i
			   End If
    
    
			End Set
			Get
				Return colCount
			End Get
		End Property

		''' <summary>
		''' Sets whether the designated column is automatically
		''' numbered, thus read-only, to the given <code>boolean</code>
		''' value.
		''' </summary>
		''' <param name="columnIndex"> the first column is 1, the second is 2, and so on;
		'''        must be between <code>1</code> and the number of columns
		'''        in the rowset, inclusive </param>
		''' <param name="property"> <code>true</code> if the given column is
		'''                 automatically incremented; <code>false</code>
		'''                 otherwise </param>
		''' <exception cref="SQLException"> if a database access error occurs or
		'''         the given index is out of bounds </exception>
		Public Overridable Sub setAutoIncrement(ByVal columnIndex As Integer, ByVal [property] As Boolean) Implements RowSetMetaData.setAutoIncrement
			checkColRange(columnIndex)
			colInfo(columnIndex).autoIncrement = [property]
		End Sub

		''' <summary>
		''' Sets whether the name of the designated column is case sensitive to
		''' the given <code>boolean</code>.
		''' </summary>
		''' <param name="columnIndex"> the first column is 1, the second is 2, and so on;
		'''        must be between <code>1</code> and the number of columns
		'''        in the rowset, inclusive </param>
		''' <param name="property"> <code>true</code> to indicate that the column
		'''                 name is case sensitive; <code>false</code> otherwise </param>
		''' <exception cref="SQLException"> if a database access error occurs or
		'''         the given column number is out of bounds </exception>
		Public Overridable Sub setCaseSensitive(ByVal columnIndex As Integer, ByVal [property] As Boolean) Implements RowSetMetaData.setCaseSensitive
			checkColRange(columnIndex)
			colInfo(columnIndex).caseSensitive = [property]
		End Sub

		''' <summary>
		''' Sets whether a value stored in the designated column can be used
		''' in a <code>WHERE</code> clause to the given <code>boolean</code> value.
		''' </summary>
		''' <param name="columnIndex"> the first column is 1, the second is 2, and so on;
		'''                    must be between <code>1</code> and the number
		'''                    of columns in the rowset, inclusive </param>
		''' <param name="property"> <code>true</code> to indicate that a column
		'''                 value can be used in a <code>WHERE</code> clause;
		'''                 <code>false</code> otherwise
		''' </param>
		''' <exception cref="SQLException"> if a database access error occurs or
		'''         the given column number is out of bounds </exception>
		Public Overridable Sub setSearchable(ByVal columnIndex As Integer, ByVal [property] As Boolean) Implements RowSetMetaData.setSearchable
			checkColRange(columnIndex)
			colInfo(columnIndex).searchable = [property]
		End Sub

		''' <summary>
		''' Sets whether a value stored in the designated column is a cash
		''' value to the given <code>boolean</code>.
		''' </summary>
		''' <param name="columnIndex"> the first column is 1, the second is 2, and so on;
		''' must be between <code>1</code> and the number of columns,
		''' inclusive between <code>1</code> and the number of columns, inclusive </param>
		''' <param name="property"> true if the value is a cash value; false otherwise. </param>
		''' <exception cref="SQLException"> if a database access error occurs
		'''         or the given column number is out of bounds </exception>
		Public Overridable Sub setCurrency(ByVal columnIndex As Integer, ByVal [property] As Boolean) Implements RowSetMetaData.setCurrency
			checkColRange(columnIndex)
			colInfo(columnIndex).currency = [property]
		End Sub

		''' <summary>
		''' Sets whether a value stored in the designated column can be set
		''' to <code>NULL</code> to the given constant from the interface
		''' <code>ResultSetMetaData</code>.
		''' </summary>
		''' <param name="columnIndex"> the first column is 1, the second is 2, and so on;
		'''        must be between <code>1</code> and the number of columns, inclusive </param>
		''' <param name="property"> one of the following <code>ResultSetMetaData</code> constants:
		'''                 <code>columnNoNulls</code>,
		'''                 <code>columnNullable</code>, or
		'''                 <code>columnNullableUnknown</code>
		''' </param>
		''' <exception cref="SQLException"> if a database access error occurs,
		'''         the given column number is out of bounds, or the value supplied
		'''         for the <i>property</i> parameter is not one of the following
		'''         constants:
		'''           <code>ResultSetMetaData.columnNoNulls</code>,
		'''           <code>ResultSetMetaData.columnNullable</code>, or
		'''           <code>ResultSetMetaData.columnNullableUnknown</code> </exception>
		Public Overridable Sub setNullable(ByVal columnIndex As Integer, ByVal [property] As Integer) Implements RowSetMetaData.setNullable
			If ([property] < ResultSetMetaData.columnNoNulls) OrElse [property] > ResultSetMetaData.columnNullableUnknown Then Throw New SQLException("Invalid nullable constant set. Must be " & "either columnNoNulls, columnNullable or columnNullableUnknown")
			checkColRange(columnIndex)
			colInfo(columnIndex).nullable = [property]
		End Sub

		''' <summary>
		''' Sets whether a value stored in the designated column is a signed
		''' number to the given <code>boolean</code>.
		''' </summary>
		''' <param name="columnIndex"> the first column is 1, the second is 2, and so on;
		'''        must be between <code>1</code> and the number of columns, inclusive </param>
		''' <param name="property"> <code>true</code> to indicate that a column
		'''                 value is a signed number;
		'''                 <code>false</code> to indicate that it is not </param>
		''' <exception cref="SQLException"> if a database access error occurs
		'''         or the given column number is out of bounds </exception>
		Public Overridable Sub setSigned(ByVal columnIndex As Integer, ByVal [property] As Boolean) Implements RowSetMetaData.setSigned
			checkColRange(columnIndex)
			colInfo(columnIndex).signed = [property]
		End Sub

		''' <summary>
		''' Sets the normal maximum number of chars in the designated column
		''' to the given number.
		''' </summary>
		''' <param name="columnIndex"> the first column is 1, the second is 2, and so on;
		'''        must be between <code>1</code> and the number of columns, inclusive </param>
		''' <param name="size"> the maximum size of the column in chars; must be
		'''        <code>0</code> or more </param>
		''' <exception cref="SQLException"> if a database access error occurs,
		'''        the given column number is out of bounds, or <i>size</i> is
		'''        less than <code>0</code> </exception>
		Public Overridable Sub setColumnDisplaySize(ByVal columnIndex As Integer, ByVal size As Integer) Implements RowSetMetaData.setColumnDisplaySize
			If size < 0 Then Throw New SQLException("Invalid column display size. Cannot be less " & "than zero")
			checkColRange(columnIndex)
			colInfo(columnIndex).columnDisplaySize = size
		End Sub

		''' <summary>
		''' Sets the suggested column label for use in printouts and
		''' displays, if any, to <i>label</i>. If <i>label</i> is
		''' <code>null</code>, the column label is set to an empty string
		''' ("").
		''' </summary>
		''' <param name="columnIndex"> the first column is 1, the second is 2, and so on;
		'''        must be between <code>1</code> and the number of columns, inclusive </param>
		''' <param name="label"> the column label to be used in printouts and displays; if the
		'''        column label is <code>null</code>, an empty <code>String</code> is
		'''        set </param>
		''' <exception cref="SQLException"> if a database access error occurs
		'''         or the given column index is out of bounds </exception>
		Public Overridable Sub setColumnLabel(ByVal columnIndex As Integer, ByVal label As String) Implements RowSetMetaData.setColumnLabel
			checkColRange(columnIndex)
			If label IsNot Nothing Then
				colInfo(columnIndex).columnLabel = label
			Else
				colInfo(columnIndex).columnLabel = ""
			End If
		End Sub

		''' <summary>
		''' Sets the column name of the designated column to the given name.
		''' </summary>
		''' <param name="columnIndex"> the first column is 1, the second is 2, and so on;
		'''      must be between <code>1</code> and the number of columns, inclusive </param>
		''' <param name="columnName"> a <code>String</code> object indicating the column name;
		'''      if the given name is <code>null</code>, an empty <code>String</code>
		'''      is set </param>
		''' <exception cref="SQLException"> if a database access error occurs or the given column
		'''      index is out of bounds </exception>
		Public Overridable Sub setColumnName(ByVal columnIndex As Integer, ByVal columnName As String) Implements RowSetMetaData.setColumnName
			checkColRange(columnIndex)
			If columnName IsNot Nothing Then
				colInfo(columnIndex).columnName = columnName
			Else
				colInfo(columnIndex).columnName = ""
			End If
		End Sub

		''' <summary>
		''' Sets the designated column's table's schema name, if any, to
		''' <i>schemaName</i>. If <i>schemaName</i> is <code>null</code>,
		''' the schema name is set to an empty string ("").
		''' </summary>
		''' <param name="columnIndex"> the first column is 1, the second is 2, and so on;
		'''        must be between <code>1</code> and the number of columns, inclusive </param>
		''' <param name="schemaName"> the schema name for the table from which a value in the
		'''        designated column was derived; may be an empty <code>String</code>
		'''        or <code>null</code> </param>
		''' <exception cref="SQLException"> if a database access error occurs
		'''        or the given column number is out of bounds </exception>
		Public Overridable Sub setSchemaName(ByVal columnIndex As Integer, ByVal schemaName As String) Implements RowSetMetaData.setSchemaName
			checkColRange(columnIndex)
			If schemaName IsNot Nothing Then
				colInfo(columnIndex).schemaName = schemaName
			Else
				colInfo(columnIndex).schemaName = ""
			End If
		End Sub

		''' <summary>
		''' Sets the total number of decimal digits in a value stored in the
		''' designated column to the given number.
		''' </summary>
		''' <param name="columnIndex"> the first column is 1, the second is 2, and so on;
		'''        must be between <code>1</code> and the number of columns, inclusive </param>
		''' <param name="precision"> the total number of decimal digits; must be <code>0</code>
		'''        or more </param>
		''' <exception cref="SQLException"> if a database access error occurs,
		'''         <i>columnIndex</i> is out of bounds, or <i>precision</i>
		'''         is less than <code>0</code> </exception>
		Public Overridable Sub setPrecision(ByVal columnIndex As Integer, ByVal precision As Integer) Implements RowSetMetaData.setPrecision

			If precision < 0 Then Throw New SQLException("Invalid precision value. Cannot be less " & "than zero")
			checkColRange(columnIndex)
			colInfo(columnIndex).colPrecision = precision
		End Sub

		''' <summary>
		''' Sets the number of digits to the right of the decimal point in a value
		''' stored in the designated column to the given number.
		''' </summary>
		''' <param name="columnIndex"> the first column is 1, the second is 2, and so on;
		'''        must be between <code>1</code> and the number of columns, inclusive </param>
		''' <param name="scale"> the number of digits to the right of the decimal point; must be
		'''        zero or greater </param>
		''' <exception cref="SQLException"> if a database access error occurs,
		'''         <i>columnIndex</i> is out of bounds, or <i>scale</i>
		'''         is less than <code>0</code> </exception>
		Public Overridable Sub setScale(ByVal columnIndex As Integer, ByVal scale As Integer) Implements RowSetMetaData.setScale
			If scale < 0 Then Throw New SQLException("Invalid scale size. Cannot be less " & "than zero")
			checkColRange(columnIndex)
			colInfo(columnIndex).colScale = scale
		End Sub

		''' <summary>
		''' Sets the name of the table from which the designated column
		''' was derived to the given table name.
		''' </summary>
		''' <param name="columnIndex"> the first column is 1, the second is 2, and so on;
		'''        must be between <code>1</code> and the number of columns, inclusive </param>
		''' <param name="tableName"> the column's table name; may be <code>null</code> or an
		'''        empty string </param>
		''' <exception cref="SQLException"> if a database access error occurs
		'''         or the given column number is out of bounds </exception>
		Public Overridable Sub setTableName(ByVal columnIndex As Integer, ByVal tableName As String) Implements RowSetMetaData.setTableName
			checkColRange(columnIndex)
			If tableName IsNot Nothing Then
				colInfo(columnIndex).tableName = tableName
			Else
				colInfo(columnIndex).tableName = ""
			End If
		End Sub

		''' <summary>
		''' Sets the catalog name of the table from which the designated
		''' column was derived to <i>catalogName</i>. If <i>catalogName</i>
		''' is <code>null</code>, the catalog name is set to an empty string.
		''' </summary>
		''' <param name="columnIndex"> the first column is 1, the second is 2, and so on;
		'''        must be between <code>1</code> and the number of columns, inclusive </param>
		''' <param name="catalogName"> the column's table's catalog name; if the catalogName
		'''        is <code>null</code>, an empty <code>String</code> is set </param>
		''' <exception cref="SQLException"> if a database access error occurs
		'''         or the given column number is out of bounds </exception>
		Public Overridable Sub setCatalogName(ByVal columnIndex As Integer, ByVal catalogName As String) Implements RowSetMetaData.setCatalogName
			checkColRange(columnIndex)
			If catalogName IsNot Nothing Then
				colInfo(columnIndex).catName = catalogName
			Else
				colInfo(columnIndex).catName = ""
			End If
		End Sub

		''' <summary>
		''' Sets the SQL type code for values stored in the designated column
		''' to the given type code from the class <code>java.sql.Types</code>.
		''' </summary>
		''' <param name="columnIndex"> the first column is 1, the second is 2, and so on;
		'''        must be between <code>1</code> and the number of columns, inclusive </param>
		''' <param name="SQLType"> the designated column's SQL type, which must be one of the
		'''                constants in the class <code>java.sql.Types</code> </param>
		''' <exception cref="SQLException"> if a database access error occurs,
		'''         the given column number is out of bounds, or the column type
		'''         specified is not one of the constants in
		'''         <code>java.sql.Types</code> </exception>
		''' <seealso cref= java.sql.Types </seealso>
		Public Overridable Sub setColumnType(ByVal columnIndex As Integer, ByVal SQLType As Integer) Implements RowSetMetaData.setColumnType
			' examine java.sql.Type reflectively, loop on the fields and check
			' this. Separate out into a private method
			checkColType(SQLType)
			checkColRange(columnIndex)
			colInfo(columnIndex).colType = SQLType
		End Sub

		''' <summary>
		''' Sets the type name used by the data source for values stored in the
		''' designated column to the given type name.
		''' </summary>
		''' <param name="columnIndex"> the first column is 1, the second is 2, and so on;
		''' must be between <code>1</code> and the number of columns, inclusive </param>
		''' <param name="typeName"> the data source-specific type name; if <i>typeName</i> is
		'''        <code>null</code>, an empty <code>String</code> is set </param>
		''' <exception cref="SQLException"> if a database access error occurs
		'''         or the given column number is out of bounds </exception>
		Public Overridable Sub setColumnTypeName(ByVal columnIndex As Integer, ByVal typeName As String) Implements RowSetMetaData.setColumnTypeName
			checkColRange(columnIndex)
			If typeName IsNot Nothing Then
				colInfo(columnIndex).colTypeName = typeName
			Else
				colInfo(columnIndex).colTypeName = ""
			End If
		End Sub


		''' <summary>
		''' Retrieves whether a value stored in the designated column is
		''' automatically numbered, and thus readonly.
		''' </summary>
		''' <param name="columnIndex"> the first column is 1, the second is 2, and so on;
		'''         must be between <code>1</code> and the number of columns, inclusive </param>
		''' <returns> <code>true</code> if the column is automatically numbered;
		'''         <code>false</code> otherwise </returns>
		''' <exception cref="SQLException"> if a database access error occurs
		''' or the given column number is out of bounds </exception>
		Public Overridable Function isAutoIncrement(ByVal columnIndex As Integer) As Boolean
			checkColRange(columnIndex)
			Return colInfo(columnIndex).autoIncrement
		End Function

		''' <summary>
		''' Indicates whether the case of the designated column's name
		''' matters.
		''' </summary>
		''' <param name="columnIndex"> the first column is 1, the second is 2, and so on;
		''' must be between <code>1</code> and the number of columns, inclusive </param>
		''' <returns> <code>true</code> if the column name is case sensitive;
		'''          <code>false</code> otherwise </returns>
		''' <exception cref="SQLException"> if a database access error occurs
		''' or the given column number is out of bounds </exception>
		Public Overridable Function isCaseSensitive(ByVal columnIndex As Integer) As Boolean
			checkColRange(columnIndex)
			Return colInfo(columnIndex).caseSensitive
		End Function

		''' <summary>
		''' Indicates whether a value stored in the designated column
		''' can be used in a <code>WHERE</code> clause.
		''' </summary>
		''' <param name="columnIndex"> the first column is 1, the second is 2, and so on;
		'''        must be between <code>1</code> and the number of columns, inclusive </param>
		''' <returns> <code>true</code> if a value in the designated column can be used in a
		'''         <code>WHERE</code> clause; <code>false</code> otherwise </returns>
		''' <exception cref="SQLException"> if a database access error occurs
		'''         or the given column number is out of bounds </exception>
		Public Overridable Function isSearchable(ByVal columnIndex As Integer) As Boolean
			checkColRange(columnIndex)
			Return colInfo(columnIndex).searchable
		End Function

		''' <summary>
		''' Indicates whether a value stored in the designated column
		''' is a cash value.
		''' </summary>
		''' <param name="columnIndex"> the first column is 1, the second is 2, and so on;
		'''        must be between <code>1</code> and the number of columns, inclusive </param>
		''' <returns> <code>true</code> if a value in the designated column is a cash value;
		'''         <code>false</code> otherwise </returns>
		''' <exception cref="SQLException"> if a database access error occurs
		'''         or the given column number is out of bounds </exception>
		Public Overridable Function isCurrency(ByVal columnIndex As Integer) As Boolean
			checkColRange(columnIndex)
			Return colInfo(columnIndex).currency
		End Function

		''' <summary>
		''' Retrieves a constant indicating whether it is possible
		''' to store a <code>NULL</code> value in the designated column.
		''' </summary>
		''' <param name="columnIndex"> the first column is 1, the second is 2, and so on;
		'''        must be between <code>1</code> and the number of columns, inclusive </param>
		''' <returns> a constant from the <code>ResultSetMetaData</code> interface;
		'''         either <code>columnNoNulls</code>,
		'''         <code>columnNullable</code>, or
		'''         <code>columnNullableUnknown</code> </returns>
		''' <exception cref="SQLException"> if a database access error occurs
		'''         or the given column number is out of bounds </exception>
		Public Overridable Function isNullable(ByVal columnIndex As Integer) As Integer
			checkColRange(columnIndex)
			Return colInfo(columnIndex).nullable
		End Function

		''' <summary>
		''' Indicates whether a value stored in the designated column is
		''' a signed number.
		''' </summary>
		''' <param name="columnIndex"> the first column is 1, the second is 2, and so on;
		'''        must be between <code>1</code> and the number of columns, inclusive </param>
		''' <returns> <code>true</code> if a value in the designated column is a signed
		'''         number; <code>false</code> otherwise </returns>
		''' <exception cref="SQLException"> if a database access error occurs
		'''         or the given column number is out of bounds </exception>
		Public Overridable Function isSigned(ByVal columnIndex As Integer) As Boolean
			checkColRange(columnIndex)
			Return colInfo(columnIndex).signed
		End Function

		''' <summary>
		''' Retrieves the normal maximum width in chars of the designated column.
		''' </summary>
		''' <param name="columnIndex"> the first column is 1, the second is 2, and so on;
		'''        must be between <code>1</code> and the number of columns, inclusive </param>
		''' <returns> the maximum number of chars that can be displayed in the designated
		'''         column </returns>
		''' <exception cref="SQLException"> if a database access error occurs
		'''         or the given column number is out of bounds </exception>
		Public Overridable Function getColumnDisplaySize(ByVal columnIndex As Integer) As Integer
			checkColRange(columnIndex)
			Return colInfo(columnIndex).columnDisplaySize
		End Function

		''' <summary>
		''' Retrieves the suggested column title for the designated
		''' column for use in printouts and displays.
		''' </summary>
		''' <param name="columnIndex"> the first column is 1, the second is 2, and so on;
		'''        must be between <code>1</code> and the number of columns, inclusive </param>
		''' <returns> the suggested column name to use in printouts and displays </returns>
		''' <exception cref="SQLException"> if a database access error occurs
		'''         or the given column number is out of bounds </exception>
		Public Overridable Function getColumnLabel(ByVal columnIndex As Integer) As String
			checkColRange(columnIndex)
			Return colInfo(columnIndex).columnLabel
		End Function

		''' <summary>
		''' Retrieves the name of the designated column.
		''' </summary>
		''' <param name="columnIndex"> the first column is 1, the second is 2, and so on;
		''' must be between <code>1</code> and the number of columns, inclusive </param>
		''' <returns> the column name of the designated column </returns>
		''' <exception cref="SQLException"> if a database access error occurs
		''' or the given column number is out of bounds </exception>
		Public Overridable Function getColumnName(ByVal columnIndex As Integer) As String
			checkColRange(columnIndex)
			Return colInfo(columnIndex).columnName
		End Function

		''' <summary>
		''' Retrieves the schema name of the table from which the value
		''' in the designated column was derived.
		''' </summary>
		''' <param name="columnIndex"> the first column is 1, the second is 2, and so on;
		'''         must be between <code>1</code> and the number of columns,
		'''         inclusive </param>
		''' <returns> the schema name or an empty <code>String</code> if no schema
		'''         name is available </returns>
		''' <exception cref="SQLException"> if a database access error occurs
		''' or the given column number is out of bounds </exception>
		Public Overridable Function getSchemaName(ByVal columnIndex As Integer) As String
			checkColRange(columnIndex)
			Dim str As String =""
			If colInfo(columnIndex).schemaName Is Nothing Then
			Else
				  str = colInfo(columnIndex).schemaName
			End If
			Return str
		End Function

		''' <summary>
		''' Retrieves the total number of digits for values stored in
		''' the designated column.
		''' </summary>
		''' <param name="columnIndex"> the first column is 1, the second is 2, and so on;
		''' must be between <code>1</code> and the number of columns, inclusive </param>
		''' <returns> the precision for values stored in the designated column </returns>
		''' <exception cref="SQLException"> if a database access error occurs
		''' or the given column number is out of bounds </exception>
		Public Overridable Function getPrecision(ByVal columnIndex As Integer) As Integer
			checkColRange(columnIndex)
			Return colInfo(columnIndex).colPrecision
		End Function

		''' <summary>
		''' Retrieves the number of digits to the right of the decimal point
		''' for values stored in the designated column.
		''' </summary>
		''' <param name="columnIndex"> the first column is 1, the second is 2, and so on;
		''' must be between <code>1</code> and the number of columns, inclusive </param>
		''' <returns> the scale for values stored in the designated column </returns>
		''' <exception cref="SQLException"> if a database access error occurs
		''' or the given column number is out of bounds </exception>
		Public Overridable Function getScale(ByVal columnIndex As Integer) As Integer
			checkColRange(columnIndex)
			Return colInfo(columnIndex).colScale
		End Function

		''' <summary>
		''' Retrieves the name of the table from which the value
		''' in the designated column was derived.
		''' </summary>
		''' <param name="columnIndex"> the first column is 1, the second is 2, and so on;
		''' must be between <code>1</code> and the number of columns, inclusive </param>
		''' <returns> the table name or an empty <code>String</code> if no table name
		'''         is available </returns>
		''' <exception cref="SQLException"> if a database access error occurs
		''' or the given column number is out of bounds </exception>
		Public Overridable Function getTableName(ByVal columnIndex As Integer) As String
			checkColRange(columnIndex)
			Return colInfo(columnIndex).tableName
		End Function

		''' <summary>
		''' Retrieves the catalog name of the table from which the value
		''' in the designated column was derived.
		''' </summary>
		''' <param name="columnIndex"> the first column is 1, the second is 2, and so on;
		''' must be between <code>1</code> and the number of columns, inclusive </param>
		''' <returns> the catalog name of the column's table or an empty
		'''         <code>String</code> if no catalog name is available </returns>
		''' <exception cref="SQLException"> if a database access error occurs
		''' or the given column number is out of bounds </exception>
		Public Overridable Function getCatalogName(ByVal columnIndex As Integer) As String
			checkColRange(columnIndex)
			Dim str As String =""
			If colInfo(columnIndex).catName Is Nothing Then
			Else
			   str = colInfo(columnIndex).catName
			End If
			Return str
		End Function

		''' <summary>
		''' Retrieves the type code (one of the <code>java.sql.Types</code>
		''' constants) for the SQL type of the value stored in the
		''' designated column.
		''' </summary>
		''' <param name="columnIndex"> the first column is 1, the second is 2, and so on;
		''' must be between <code>1</code> and the number of columns, inclusive </param>
		''' <returns> an <code>int</code> representing the SQL type of values
		''' stored in the designated column </returns>
		''' <exception cref="SQLException"> if a database access error occurs
		''' or the given column number is out of bounds </exception>
		''' <seealso cref= java.sql.Types </seealso>
		Public Overridable Function getColumnType(ByVal columnIndex As Integer) As Integer
			checkColRange(columnIndex)
			Return colInfo(columnIndex).colType
		End Function

		''' <summary>
		''' Retrieves the DBMS-specific type name for values stored in the
		''' designated column.
		''' </summary>
		''' <param name="columnIndex"> the first column is 1, the second is 2, and so on;
		''' must be between <code>1</code> and the number of columns, inclusive </param>
		''' <returns> the type name used by the data source </returns>
		''' <exception cref="SQLException"> if a database access error occurs
		''' or the given column number is out of bounds </exception>
		Public Overridable Function getColumnTypeName(ByVal columnIndex As Integer) As String
			checkColRange(columnIndex)
			Return colInfo(columnIndex).colTypeName
		End Function


		''' <summary>
		''' Indicates whether the designated column is definitely
		''' not writable, thus readonly.
		''' </summary>
		''' <param name="columnIndex"> the first column is 1, the second is 2, and so on;
		''' must be between <code>1</code> and the number of columns, inclusive </param>
		''' <returns> <code>true</code> if this <code>RowSet</code> object is read-Only
		''' and thus not updatable; <code>false</code> otherwise </returns>
		''' <exception cref="SQLException"> if a database access error occurs
		''' or the given column number is out of bounds </exception>
		Public Overridable Function isReadOnly(ByVal columnIndex As Integer) As Boolean
			checkColRange(columnIndex)
			Return colInfo(columnIndex).readOnly
		End Function

		''' <summary>
		''' Indicates whether it is possible for a write operation on
		''' the designated column to succeed. A return value of
		''' <code>true</code> means that a write operation may or may
		''' not succeed.
		''' </summary>
		''' <param name="columnIndex"> the first column is 1, the second is 2, and so on;
		'''         must be between <code>1</code> and the number of columns, inclusive </param>
		''' <returns> <code>true</code> if a write operation on the designated column may
		'''          will succeed; <code>false</code> otherwise </returns>
		''' <exception cref="SQLException"> if a database access error occurs
		'''         or the given column number is out of bounds </exception>
		Public Overridable Function isWritable(ByVal columnIndex As Integer) As Boolean
			checkColRange(columnIndex)
			Return colInfo(columnIndex).writable
		End Function

		''' <summary>
		''' Indicates whether a write operation on the designated column
		''' will definitely succeed.
		''' </summary>
		''' <param name="columnIndex"> the first column is 1, the second is 2, and so on;
		''' must be between <code>1</code> and the number of columns, inclusive </param>
		''' <returns> <code>true</code> if a write operation on the designated column will
		'''         definitely succeed; <code>false</code> otherwise </returns>
		''' <exception cref="SQLException"> if a database access error occurs
		''' or the given column number is out of bounds </exception>
		Public Overridable Function isDefinitelyWritable(ByVal columnIndex As Integer) As Boolean
			checkColRange(columnIndex)
			Return True
		End Function

		''' <summary>
		''' Retrieves the fully-qualified name of the class in the Java
		''' programming language to which a value in the designated column
		''' will be mapped.  For example, if the value is an <code>int</code>,
		''' the class name returned by this method will be
		''' <code>java.lang.Integer</code>.
		''' <P>
		''' If the value in the designated column has a custom mapping,
		''' this method returns the name of the class that implements
		''' <code>SQLData</code>. When the method <code>ResultSet.getObject</code>
		''' is called to retrieve a value from the designated column, it will
		''' create an instance of this class or one of its subclasses.
		''' </summary>
		''' <param name="columnIndex"> the first column is 1, the second is 2, and so on;
		'''        must be between <code>1</code> and the number of columns, inclusive </param>
		''' <returns> the fully-qualified name of the class in the Java programming
		'''        language that would be used by the method <code>RowSet.getObject</code> to
		'''        retrieve the value in the specified column. This is the class
		'''        name used for custom mapping when there is a custom mapping. </returns>
		''' <exception cref="SQLException"> if a database access error occurs
		'''         or the given column number is out of bounds </exception>
		Public Overridable Function getColumnClassName(ByVal columnIndex As Integer) As String
			Dim className As String = GetType(String).name

			Dim sqlType As Integer = getColumnType(columnIndex)

			Select Case sqlType

			Case Types.NUMERIC, Types.DECIMAL
				className = GetType(Decimal).name

			Case Types.BIT
				className = GetType(Boolean).name

			Case Types.TINYINT
				className = GetType(SByte?).name

			Case Types.SMALLINT
				className = GetType(Short).name

			Case Types.INTEGER
				className = GetType(Integer).name

			Case Types.BIGINT
				className = GetType(Long).name

			Case Types.REAL
				className = GetType(Single?).name

			Case Types.FLOAT, Types.DOUBLE
				className = GetType(Double).name

			Case Types.BINARY, Types.VARBINARY, Types.LONGVARBINARY
				className = "byte[]"

			Case Types.DATE
				className = GetType(java.sql.Date).name

			Case Types.TIME
				className = GetType(java.sql.Time).name

			Case Types.TIMESTAMP
				className = GetType(java.sql.Timestamp).name

			Case Types.BLOB
				className = GetType(java.sql.Blob).name

			Case Types.CLOB
				className = GetType(java.sql.Clob).name
			End Select

			Return className
		End Function

		''' <summary>
		''' Returns an object that implements the given interface to allow access to non-standard methods,
		''' or standard methods not exposed by the proxy.
		''' The result may be either the object found to implement the interface or a proxy for that object.
		''' If the receiver implements the interface then that is the object. If the receiver is a wrapper
		''' and the wrapped object implements the interface then that is the object. Otherwise the object is
		'''  the result of calling <code>unwrap</code> recursively on the wrapped object. If the receiver is not a
		''' wrapper and does not implement the interface, then an <code>SQLException</code> is thrown.
		''' </summary>
		''' <param name="iface"> A Class defining an interface that the result must implement. </param>
		''' <returns> an object that implements the interface. May be a proxy for the actual implementing object. </returns>
		''' <exception cref="java.sql.SQLException"> If no object found that implements the interface
		''' @since 1.6 </exception>
		Public Overridable Function unwrap(Of T)(ByVal iface As Type) As T

			If isWrapperFor(iface) Then
				Return iface.cast(Me)
			Else
				Throw New SQLException("unwrap failed for:" & iface)
			End If
		End Function

		''' <summary>
		''' Returns true if this either implements the interface argument or is directly or indirectly a wrapper
		''' for an object that does. Returns false otherwise. If this implements the interface then return true,
		''' else if this is a wrapper then return the result of recursively calling <code>isWrapperFor</code> on the wrapped
		''' object. If this does not implement the interface and is not a wrapper, return false.
		''' This method should be implemented as a low-cost operation compared to <code>unwrap</code> so that
		''' callers can use this method to avoid expensive <code>unwrap</code> calls that may fail. If this method
		''' returns true then calling <code>unwrap</code> with the same argument should succeed.
		''' </summary>
		''' <param name="interfaces"> a Class defining an interface. </param>
		''' <returns> true if this implements the interface or directly or indirectly wraps an object that does. </returns>
		''' <exception cref="java.sql.SQLException">  if an error occurs while determining whether this is a wrapper
		''' for an object with the given interface.
		''' @since 1.6 </exception>
		Public Overridable Function isWrapperFor(ByVal interfaces As Type) As Boolean
			Return interfaces.IsInstanceOfType(Me)
		End Function

		Friend Const serialVersionUID As Long = 6893806403181801867L

		<Serializable> _
		Private Class ColInfo
			Private ReadOnly outerInstance As RowSetMetaDataImpl

			Public Sub New(ByVal outerInstance As RowSetMetaDataImpl)
				Me.outerInstance = outerInstance
			End Sub

			''' <summary>
			''' The field that indicates whether the value in this column is a number
			''' that is incremented automatically, which makes the value read-only.
			''' <code>true</code> means that the value in this column
			''' is automatically numbered; <code>false</code> means that it is not.
			''' 
			''' @serial
			''' </summary>
			Public autoIncrement As Boolean

			''' <summary>
			''' The field that indicates whether the value in this column is case sensitive.
			''' <code>true</code> means that it is; <code>false</code> that it is not.
			''' 
			''' @serial
			''' </summary>
			Public caseSensitive As Boolean

			''' <summary>
			''' The field that indicates whether the value in this column is a cash value
			''' <code>true</code> means that it is; <code>false</code> that it is not.
			''' 
			''' @serial
			''' </summary>
			Public currency As Boolean

			''' <summary>
			''' The field that indicates whether the value in this column is nullable.
			''' The possible values are the <code>ResultSet</code> constants
			''' <code>columnNoNulls</code>, <code>columnNullable</code>, and
			''' <code>columnNullableUnknown</code>.
			''' 
			''' @serial
			''' </summary>
			Public nullable As Integer

			''' <summary>
			''' The field that indicates whether the value in this column is a signed number.
			''' <code>true</code> means that it is; <code>false</code> that it is not.
			''' 
			''' @serial
			''' </summary>
			Public signed As Boolean

			''' <summary>
			''' The field that indicates whether the value in this column can be used in
			''' a <code>WHERE</code> clause.
			''' <code>true</code> means that it can; <code>false</code> that it cannot.
			''' 
			''' @serial
			''' </summary>
			Public searchable As Boolean

			''' <summary>
			''' The field that indicates the normal maximum width in characters for
			''' this column.
			''' 
			''' @serial
			''' </summary>
			Public columnDisplaySize As Integer

			''' <summary>
			''' The field that holds the suggested column title for this column, to be
			''' used in printing and displays.
			''' 
			''' @serial
			''' </summary>
			Public columnLabel As String

			''' <summary>
			''' The field that holds the name of this column.
			''' 
			''' @serial
			''' </summary>
			Public columnName As String

			''' <summary>
			''' The field that holds the schema name for the table from which this column
			''' was derived.
			''' 
			''' @serial
			''' </summary>
			Public schemaName As String

			''' <summary>
			''' The field that holds the precision of the value in this column.  For number
			''' types, the precision is the total number of decimal digits; for character types,
			''' it is the maximum number of characters; for binary types, it is the maximum
			''' length in bytes.
			''' 
			''' @serial
			''' </summary>
			Public colPrecision As Integer

			''' <summary>
			''' The field that holds the scale (number of digits to the right of the decimal
			''' point) of the value in this column.
			''' 
			''' @serial
			''' </summary>
			Public colScale As Integer

			''' <summary>
			''' The field that holds the name of the table from which this column
			''' was derived.  This value may be the empty string if there is no
			''' table name, such as when this column is produced by a join.
			''' 
			''' @serial
			''' </summary>
			Public tableName As String =""

			''' <summary>
			''' The field that holds the catalog name for the table from which this column
			''' was derived.  If the DBMS does not support catalogs, the value may be the
			''' empty string.
			''' 
			''' @serial
			''' </summary>
			Public catName As String

			''' <summary>
			''' The field that holds the type code from the class <code>java.sql.Types</code>
			''' indicating the type of the value in this column.
			''' 
			''' @serial
			''' </summary>
			Public colType As Integer

			''' <summary>
			''' The field that holds the type name used by this particular data source
			''' for the value stored in this column.
			''' 
			''' @serial
			''' </summary>
			Public colTypeName As String

			''' <summary>
			''' The field that holds the updatability boolean per column of a RowSet
			''' 
			''' @serial
			''' </summary>
			Public [readOnly] As Boolean = False

			''' <summary>
			''' The field that hold the writable boolean per column of a RowSet
			''' 
			''' @serial
			''' </summary>
			Public writable As Boolean = True

			Friend Const serialVersionUID As Long = 5490834817919311283L
		End Class
	End Class

End Namespace