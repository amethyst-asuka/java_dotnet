'
' * Copyright (c) 1996, 2005, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.sql

	''' <summary>
	''' An object that can be used to get information about the types
	''' and properties of the columns in a <code>ResultSet</code> object.
	''' The following code fragment creates the <code>ResultSet</code> object rs,
	''' creates the <code>ResultSetMetaData</code> object rsmd, and uses rsmd
	''' to find out how many columns rs has and whether the first column in rs
	''' can be used in a <code>WHERE</code> clause.
	''' <PRE>
	''' 
	'''     ResultSet rs = stmt.executeQuery("SELECT a, b, c FROM TABLE2");
	'''     ResultSetMetaData rsmd = rs.getMetaData();
	'''     int numberOfColumns = rsmd.getColumnCount();
	'''     boolean b = rsmd.isSearchable(1);
	''' 
	''' </PRE>
	''' </summary>

	Public Interface ResultSetMetaData
		Inherits Wrapper

		''' <summary>
		''' Returns the number of columns in this <code>ResultSet</code> object.
		''' </summary>
		''' <returns> the number of columns </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		ReadOnly Property columnCount As Integer

		''' <summary>
		''' Indicates whether the designated column is automatically numbered.
		''' </summary>
		''' <param name="column"> the first column is 1, the second is 2, ... </param>
		''' <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		Function isAutoIncrement(  column As Integer) As Boolean

		''' <summary>
		''' Indicates whether a column's case matters.
		''' </summary>
		''' <param name="column"> the first column is 1, the second is 2, ... </param>
		''' <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		Function isCaseSensitive(  column As Integer) As Boolean

		''' <summary>
		''' Indicates whether the designated column can be used in a where clause.
		''' </summary>
		''' <param name="column"> the first column is 1, the second is 2, ... </param>
		''' <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		Function isSearchable(  column As Integer) As Boolean

		''' <summary>
		''' Indicates whether the designated column is a cash value.
		''' </summary>
		''' <param name="column"> the first column is 1, the second is 2, ... </param>
		''' <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		Function isCurrency(  column As Integer) As Boolean

		''' <summary>
		''' Indicates the nullability of values in the designated column.
		''' </summary>
		''' <param name="column"> the first column is 1, the second is 2, ... </param>
		''' <returns> the nullability status of the given column; one of <code>columnNoNulls</code>,
		'''          <code>columnNullable</code> or <code>columnNullableUnknown</code> </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		Function isNullable(  column As Integer) As Integer

		''' <summary>
		''' The constant indicating that a
		''' column does not allow <code>NULL</code> values.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int columnNoNulls = 0;

		''' <summary>
		''' The constant indicating that a
		''' column allows <code>NULL</code> values.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int columnNullable = 1;

		''' <summary>
		''' The constant indicating that the
		''' nullability of a column's values is unknown.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int columnNullableUnknown = 2;

		''' <summary>
		''' Indicates whether values in the designated column are signed numbers.
		''' </summary>
		''' <param name="column"> the first column is 1, the second is 2, ... </param>
		''' <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		Function isSigned(  column As Integer) As Boolean

		''' <summary>
		''' Indicates the designated column's normal maximum width in characters.
		''' </summary>
		''' <param name="column"> the first column is 1, the second is 2, ... </param>
		''' <returns> the normal maximum number of characters allowed as the width
		'''          of the designated column </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		Function getColumnDisplaySize(  column As Integer) As Integer

		''' <summary>
		''' Gets the designated column's suggested title for use in printouts and
		''' displays. The suggested title is usually specified by the SQL <code>AS</code>
		''' clause.  If a SQL <code>AS</code> is not specified, the value returned from
		''' <code>getColumnLabel</code> will be the same as the value returned by the
		''' <code>getColumnName</code> method.
		''' </summary>
		''' <param name="column"> the first column is 1, the second is 2, ... </param>
		''' <returns> the suggested column title </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		Function getColumnLabel(  column As Integer) As String

		''' <summary>
		''' Get the designated column's name.
		''' </summary>
		''' <param name="column"> the first column is 1, the second is 2, ... </param>
		''' <returns> column name </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		Function getColumnName(  column As Integer) As String

		''' <summary>
		''' Get the designated column's table's schema.
		''' </summary>
		''' <param name="column"> the first column is 1, the second is 2, ... </param>
		''' <returns> schema name or "" if not applicable </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		Function getSchemaName(  column As Integer) As String

		''' <summary>
		''' Get the designated column's specified column size.
		''' For numeric data, this is the maximum precision.  For character data, this is the length in characters.
		''' For datetime datatypes, this is the length in characters of the String representation (assuming the
		''' maximum allowed precision of the fractional seconds component). For binary data, this is the length in bytes.  For the ROWID datatype,
		''' this is the length in bytes. 0 is returned for data types where the
		''' column size is not applicable.
		''' </summary>
		''' <param name="column"> the first column is 1, the second is 2, ... </param>
		''' <returns> precision </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		Function getPrecision(  column As Integer) As Integer

		''' <summary>
		''' Gets the designated column's number of digits to right of the decimal point.
		''' 0 is returned for data types where the scale is not applicable.
		''' </summary>
		''' <param name="column"> the first column is 1, the second is 2, ... </param>
		''' <returns> scale </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		Function getScale(  column As Integer) As Integer

		''' <summary>
		''' Gets the designated column's table name.
		''' </summary>
		''' <param name="column"> the first column is 1, the second is 2, ... </param>
		''' <returns> table name or "" if not applicable </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		Function getTableName(  column As Integer) As String

		''' <summary>
		''' Gets the designated column's table's catalog name.
		''' </summary>
		''' <param name="column"> the first column is 1, the second is 2, ... </param>
		''' <returns> the name of the catalog for the table in which the given column
		'''          appears or "" if not applicable </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		Function getCatalogName(  column As Integer) As String

		''' <summary>
		''' Retrieves the designated column's SQL type.
		''' </summary>
		''' <param name="column"> the first column is 1, the second is 2, ... </param>
		''' <returns> SQL type from java.sql.Types </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		''' <seealso cref= Types </seealso>
		Function getColumnType(  column As Integer) As Integer

		''' <summary>
		''' Retrieves the designated column's database-specific type name.
		''' </summary>
		''' <param name="column"> the first column is 1, the second is 2, ... </param>
		''' <returns> type name used by the database. If the column type is
		''' a user-defined type, then a fully-qualified type name is returned. </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		Function getColumnTypeName(  column As Integer) As String

		''' <summary>
		''' Indicates whether the designated column is definitely not writable.
		''' </summary>
		''' <param name="column"> the first column is 1, the second is 2, ... </param>
		''' <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		Function isReadOnly(  column As Integer) As Boolean

		''' <summary>
		''' Indicates whether it is possible for a write on the designated column to succeed.
		''' </summary>
		''' <param name="column"> the first column is 1, the second is 2, ... </param>
		''' <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		Function isWritable(  column As Integer) As Boolean

		''' <summary>
		''' Indicates whether a write on the designated column will definitely succeed.
		''' </summary>
		''' <param name="column"> the first column is 1, the second is 2, ... </param>
		''' <returns> <code>true</code> if so; <code>false</code> otherwise </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		Function isDefinitelyWritable(  column As Integer) As Boolean

		'--------------------------JDBC 2.0-----------------------------------

		''' <summary>
		''' <p>Returns the fully-qualified name of the Java class whose instances
		''' are manufactured if the method <code>ResultSet.getObject</code>
		''' is called to retrieve a value
		''' from the column.  <code>ResultSet.getObject</code> may return a subclass of the
		''' class returned by this method.
		''' </summary>
		''' <param name="column"> the first column is 1, the second is 2, ... </param>
		''' <returns> the fully-qualified name of the class in the Java programming
		'''         language that would be used by the method
		''' <code>ResultSet.getObject</code> to retrieve the value in the specified
		''' column. This is the class name used for custom mapping. </returns>
		''' <exception cref="SQLException"> if a database access error occurs
		''' @since 1.2 </exception>
		Function getColumnClassName(  column As Integer) As String
	End Interface

End Namespace