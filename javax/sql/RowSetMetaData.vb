'
' * Copyright (c) 2000, 2005, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.sql


	''' <summary>
	''' An object that contains information about the columns in a
	''' <code>RowSet</code> object.  This interface is
	''' an extension of the <code>ResultSetMetaData</code> interface with
	''' methods for setting the values in a <code>RowSetMetaData</code> object.
	''' When a <code>RowSetReader</code> object reads data into a <code>RowSet</code>
	''' object, it creates a <code>RowSetMetaData</code> object and initializes it
	''' using the methods in the <code>RowSetMetaData</code> interface.  Then the
	''' reader passes the <code>RowSetMetaData</code> object to the rowset.
	''' <P>
	''' The methods in this interface are invoked internally when an application
	''' calls the method <code>RowSet.execute</code>; an application
	''' programmer would not use them directly.
	''' 
	''' @since 1.4
	''' </summary>

	Public Interface RowSetMetaData
		Inherits ResultSetMetaData

	  ''' <summary>
	  ''' Sets the number of columns in the <code>RowSet</code> object to
	  ''' the given number.
	  ''' </summary>
	  ''' <param name="columnCount"> the number of columns in the <code>RowSet</code> object </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  WriteOnly Property columnCount As Integer

	  ''' <summary>
	  ''' Sets whether the designated column is automatically numbered,
	  ''' The default is for a <code>RowSet</code> object's
	  ''' columns not to be automatically numbered.
	  ''' </summary>
	  ''' <param name="columnIndex"> the first column is 1, the second is 2, ... </param>
	  ''' <param name="property"> <code>true</code> if the column is automatically
	  '''                 numbered; <code>false</code> if it is not
	  ''' </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  Sub setAutoIncrement(ByVal columnIndex As Integer, ByVal [property] As Boolean)

	  ''' <summary>
	  ''' Sets whether the designated column is case sensitive.
	  ''' The default is <code>false</code>.
	  ''' </summary>
	  ''' <param name="columnIndex"> the first column is 1, the second is 2, ... </param>
	  ''' <param name="property"> <code>true</code> if the column is case sensitive;
	  '''                 <code>false</code> if it is not
	  ''' </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  Sub setCaseSensitive(ByVal columnIndex As Integer, ByVal [property] As Boolean)

	  ''' <summary>
	  ''' Sets whether the designated column can be used in a where clause.
	  ''' The default is <code>false</code>.
	  ''' </summary>
	  ''' <param name="columnIndex"> the first column is 1, the second is 2, ... </param>
	  ''' <param name="property"> <code>true</code> if the column can be used in a
	  '''                 <code>WHERE</code> clause; <code>false</code> if it cannot
	  ''' </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  Sub setSearchable(ByVal columnIndex As Integer, ByVal [property] As Boolean)

	  ''' <summary>
	  ''' Sets whether the designated column is a cash value.
	  ''' The default is <code>false</code>.
	  ''' </summary>
	  ''' <param name="columnIndex"> the first column is 1, the second is 2, ... </param>
	  ''' <param name="property"> <code>true</code> if the column is a cash value;
	  '''                 <code>false</code> if it is not
	  ''' </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  Sub setCurrency(ByVal columnIndex As Integer, ByVal [property] As Boolean)

	  ''' <summary>
	  ''' Sets whether the designated column's value can be set to
	  ''' <code>NULL</code>.
	  ''' The default is <code>ResultSetMetaData.columnNullableUnknown</code>
	  ''' </summary>
	  ''' <param name="columnIndex"> the first column is 1, the second is 2, ... </param>
	  ''' <param name="property"> one of the following constants:
	  '''                 <code>ResultSetMetaData.columnNoNulls</code>,
	  '''                 <code>ResultSetMetaData.columnNullable</code>, or
	  '''                 <code>ResultSetMetaData.columnNullableUnknown</code>
	  ''' </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  Sub setNullable(ByVal columnIndex As Integer, ByVal [property] As Integer)

	  ''' <summary>
	  ''' Sets whether the designated column is a signed number.
	  ''' The default is <code>false</code>.
	  ''' </summary>
	  ''' <param name="columnIndex"> the first column is 1, the second is 2, ... </param>
	  ''' <param name="property"> <code>true</code> if the column is a signed number;
	  '''                 <code>false</code> if it is not
	  ''' </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  Sub setSigned(ByVal columnIndex As Integer, ByVal [property] As Boolean)

	  ''' <summary>
	  ''' Sets the designated column's normal maximum width in chars to the
	  ''' given <code>int</code>.
	  ''' </summary>
	  ''' <param name="columnIndex"> the first column is 1, the second is 2, ... </param>
	  ''' <param name="size"> the normal maximum number of characters for
	  '''           the designated column
	  ''' </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  Sub setColumnDisplaySize(ByVal columnIndex As Integer, ByVal size As Integer)

	  ''' <summary>
	  ''' Sets the suggested column title for use in printouts and
	  ''' displays, if any, to the given <code>String</code>.
	  ''' </summary>
	  ''' <param name="columnIndex"> the first column is 1, the second is 2, ... </param>
	  ''' <param name="label"> the column title </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  Sub setColumnLabel(ByVal columnIndex As Integer, ByVal label As String)

	  ''' <summary>
	  ''' Sets the name of the designated column to the given <code>String</code>.
	  ''' </summary>
	  ''' <param name="columnIndex"> the first column is 1, the second is 2, ... </param>
	  ''' <param name="columnName"> the designated column's name </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  Sub setColumnName(ByVal columnIndex As Integer, ByVal columnName As String)

	  ''' <summary>
	  ''' Sets the name of the designated column's table's schema, if any, to
	  ''' the given <code>String</code>.
	  ''' </summary>
	  ''' <param name="columnIndex"> the first column is 1, the second is 2, ... </param>
	  ''' <param name="schemaName"> the schema name </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  Sub setSchemaName(ByVal columnIndex As Integer, ByVal schemaName As String)

	  ''' <summary>
	  ''' Sets the designated column's number of decimal digits to the
	  ''' given <code>int</code>.
	  ''' </summary>
	  ''' <param name="columnIndex"> the first column is 1, the second is 2, ... </param>
	  ''' <param name="precision"> the total number of decimal digits </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  Sub setPrecision(ByVal columnIndex As Integer, ByVal precision As Integer)

	  ''' <summary>
	  ''' Sets the designated column's number of digits to the
	  ''' right of the decimal point to the given <code>int</code>.
	  ''' </summary>
	  ''' <param name="columnIndex"> the first column is 1, the second is 2, ... </param>
	  ''' <param name="scale"> the number of digits to right of decimal point </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  Sub setScale(ByVal columnIndex As Integer, ByVal scale As Integer)

	  ''' <summary>
	  ''' Sets the designated column's table name, if any, to the given
	  ''' <code>String</code>.
	  ''' </summary>
	  ''' <param name="columnIndex"> the first column is 1, the second is 2, ... </param>
	  ''' <param name="tableName"> the column's table name </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  Sub setTableName(ByVal columnIndex As Integer, ByVal tableName As String)

	  ''' <summary>
	  ''' Sets the designated column's table's catalog name, if any, to the given
	  ''' <code>String</code>.
	  ''' </summary>
	  ''' <param name="columnIndex"> the first column is 1, the second is 2, ... </param>
	  ''' <param name="catalogName"> the column's catalog name </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  Sub setCatalogName(ByVal columnIndex As Integer, ByVal catalogName As String)

	  ''' <summary>
	  ''' Sets the designated column's SQL type to the one given.
	  ''' </summary>
	  ''' <param name="columnIndex"> the first column is 1, the second is 2, ... </param>
	  ''' <param name="SQLType"> the column's SQL type </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  ''' <seealso cref= Types </seealso>
	  Sub setColumnType(ByVal columnIndex As Integer, ByVal SQLType As Integer)

	  ''' <summary>
	  ''' Sets the designated column's type name that is specific to the
	  ''' data source, if any, to the given <code>String</code>.
	  ''' </summary>
	  ''' <param name="columnIndex"> the first column is 1, the second is 2, ... </param>
	  ''' <param name="typeName"> data source specific type name. </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  Sub setColumnTypeName(ByVal columnIndex As Integer, ByVal typeName As String)

	End Interface

End Namespace