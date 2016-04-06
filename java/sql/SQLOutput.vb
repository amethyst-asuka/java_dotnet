Imports System

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

Namespace java.sql

	''' <summary>
	''' The output stream for writing the attributes of a user-defined
	''' type back to the database.  This interface, used
	''' only for custom mapping, is used by the driver, and its
	''' methods are never directly invoked by a programmer.
	''' <p>When an object of a class implementing the interface
	''' <code>SQLData</code> is passed as an argument to an SQL statement, the
	''' JDBC driver calls the method <code>SQLData.getSQLType</code> to
	''' determine the  kind of SQL
	''' datum being passed to the database.
	''' The driver then creates an instance of <code>SQLOutput</code> and
	''' passes it to the method <code>SQLData.writeSQL</code>.
	''' The method <code>writeSQL</code> in turn calls the
	''' appropriate <code>SQLOutput</code> <i>writer</i> methods
	''' <code>writeBoolean</code>, <code>writeCharacterStream</code>, and so on)
	''' to write data from the <code>SQLData</code> object to
	''' the <code>SQLOutput</code> output stream as the
	''' representation of an SQL user-defined type.
	''' @since 1.2
	''' </summary>

	 Public Interface SQLOutput

	  '================================================================
	  ' Methods for writing attributes to the stream of SQL data.
	  ' These methods correspond to the column-accessor methods of
	  ' java.sql.ResultSet.
	  '================================================================

	  ''' <summary>
	  ''' Writes the next attribute to the stream as a <code>String</code>
	  ''' in the Java programming language.
	  ''' </summary>
	  ''' <param name="x"> the value to pass to the database </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  ''' this method
	  ''' @since 1.2 </exception>
	  Sub writeString(  x As String)

	  ''' <summary>
	  ''' Writes the next attribute to the stream as a Java  java.lang.[Boolean].
	  ''' Writes the next attribute to the stream as a <code>String</code>
	  ''' in the Java programming language.
	  ''' </summary>
	  ''' <param name="x"> the value to pass to the database </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  ''' this method
	  ''' @since 1.2 </exception>
	  Sub writeBoolean(  x As Boolean)

	  ''' <summary>
	  ''' Writes the next attribute to the stream as a Java java.lang.[Byte].
	  ''' Writes the next attribute to the stream as a <code>String</code>
	  ''' in the Java programming language.
	  ''' </summary>
	  ''' <param name="x"> the value to pass to the database </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  ''' this method
	  ''' @since 1.2 </exception>
	  Sub writeByte(  x As SByte)

	  ''' <summary>
	  ''' Writes the next attribute to the stream as a Java  java.lang.[Short].
	  ''' Writes the next attribute to the stream as a <code>String</code>
	  ''' in the Java programming language.
	  ''' </summary>
	  ''' <param name="x"> the value to pass to the database </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  ''' this method
	  ''' @since 1.2 </exception>
	  Sub writeShort(  x As Short)

	  ''' <summary>
	  ''' Writes the next attribute to the stream as a Java int.
	  ''' Writes the next attribute to the stream as a <code>String</code>
	  ''' in the Java programming language.
	  ''' </summary>
	  ''' <param name="x"> the value to pass to the database </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  ''' this method
	  ''' @since 1.2 </exception>
	  Sub writeInt(  x As Integer)

	  ''' <summary>
	  ''' Writes the next attribute to the stream as a Java java.lang.[Long].
	  ''' Writes the next attribute to the stream as a <code>String</code>
	  ''' in the Java programming language.
	  ''' </summary>
	  ''' <param name="x"> the value to pass to the database </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  ''' this method
	  ''' @since 1.2 </exception>
	  Sub writeLong(  x As Long)

	  ''' <summary>
	  ''' Writes the next attribute to the stream as a Java float.
	  ''' Writes the next attribute to the stream as a <code>String</code>
	  ''' in the Java programming language.
	  ''' </summary>
	  ''' <param name="x"> the value to pass to the database </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  ''' this method
	  ''' @since 1.2 </exception>
	  Sub writeFloat(  x As Single)

	  ''' <summary>
	  ''' Writes the next attribute to the stream as a Java java.lang.[Double].
	  ''' Writes the next attribute to the stream as a <code>String</code>
	  ''' in the Java programming language.
	  ''' </summary>
	  ''' <param name="x"> the value to pass to the database </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  ''' this method
	  ''' @since 1.2 </exception>
	  Sub writeDouble(  x As Double)

	  ''' <summary>
	  ''' Writes the next attribute to the stream as a java.math.BigDecimal object.
	  ''' Writes the next attribute to the stream as a <code>String</code>
	  ''' in the Java programming language.
	  ''' </summary>
	  ''' <param name="x"> the value to pass to the database </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  ''' this method
	  ''' @since 1.2 </exception>
	  Sub writeBigDecimal(  x As Decimal)

	  ''' <summary>
	  ''' Writes the next attribute to the stream as an array of bytes.
	  ''' Writes the next attribute to the stream as a <code>String</code>
	  ''' in the Java programming language.
	  ''' </summary>
	  ''' <param name="x"> the value to pass to the database </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  ''' this method
	  ''' @since 1.2 </exception>
	  Sub writeBytes(  x As SByte())

	  ''' <summary>
	  ''' Writes the next attribute to the stream as a java.sql.Date object.
	  ''' Writes the next attribute to the stream as a <code>java.sql.Date</code> object
	  ''' in the Java programming language.
	  ''' </summary>
	  ''' <param name="x"> the value to pass to the database </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  ''' this method
	  ''' @since 1.2 </exception>
	  Sub writeDate(  x As java.sql.Date)

	  ''' <summary>
	  ''' Writes the next attribute to the stream as a java.sql.Time object.
	  ''' Writes the next attribute to the stream as a <code>java.sql.Date</code> object
	  ''' in the Java programming language.
	  ''' </summary>
	  ''' <param name="x"> the value to pass to the database </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  ''' this method
	  ''' @since 1.2 </exception>
	  Sub writeTime(  x As java.sql.Time)

	  ''' <summary>
	  ''' Writes the next attribute to the stream as a java.sql.Timestamp object.
	  ''' Writes the next attribute to the stream as a <code>java.sql.Date</code> object
	  ''' in the Java programming language.
	  ''' </summary>
	  ''' <param name="x"> the value to pass to the database </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  ''' this method
	  ''' @since 1.2 </exception>
	  Sub writeTimestamp(  x As java.sql.Timestamp)

	  ''' <summary>
	  ''' Writes the next attribute to the stream as a stream of Unicode characters.
	  ''' </summary>
	  ''' <param name="x"> the value to pass to the database </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  ''' this method
	  ''' @since 1.2 </exception>
	  Sub writeCharacterStream(  x As java.io.Reader)

	  ''' <summary>
	  ''' Writes the next attribute to the stream as a stream of ASCII characters.
	  ''' </summary>
	  ''' <param name="x"> the value to pass to the database </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  ''' this method
	  ''' @since 1.2 </exception>
	  Sub writeAsciiStream(  x As java.io.InputStream)

	  ''' <summary>
	  ''' Writes the next attribute to the stream as a stream of uninterpreted
	  ''' bytes.
	  ''' </summary>
	  ''' <param name="x"> the value to pass to the database </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  ''' this method
	  ''' @since 1.2 </exception>
	  Sub writeBinaryStream(  x As java.io.InputStream)

	  '================================================================
	  ' Methods for writing items of SQL user-defined types to the stream.
	  ' These methods pass objects to the database as values of SQL
	  ' Structured Types, Distinct Types, Constructed Types, and Locator
	  ' Types.  They decompose the Java object(s) and write leaf data
	  ' items using the methods above.
	  '================================================================

	  ''' <summary>
	  ''' Writes to the stream the data contained in the given
	  ''' <code>SQLData</code> object.
	  ''' When the <code>SQLData</code> object is <code>null</code>, this
	  ''' method writes an SQL <code>NULL</code> to the stream.
	  ''' Otherwise, it calls the <code>SQLData.writeSQL</code>
	  ''' method of the given object, which
	  ''' writes the object's attributes to the stream.
	  ''' The implementation of the method <code>SQLData.writeSQL</code>
	  ''' calls the appropriate <code>SQLOutput</code> writer method(s)
	  ''' for writing each of the object's attributes in order.
	  ''' The attributes must be read from an <code>SQLInput</code>
	  ''' input stream and written to an <code>SQLOutput</code>
	  ''' output stream in the same order in which they were
	  ''' listed in the SQL definition of the user-defined type.
	  ''' </summary>
	  ''' <param name="x"> the object representing data of an SQL structured or
	  ''' distinct type </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  ''' this method
	  ''' @since 1.2 </exception>
	  Sub writeObject(  x As SQLData)

	  ''' <summary>
	  ''' Writes an SQL <code>REF</code> value to the stream.
	  ''' </summary>
	  ''' <param name="x"> a <code>Ref</code> object representing data of an SQL
	  ''' <code>REF</code> value </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  ''' this method
	  ''' @since 1.2 </exception>
	  Sub writeRef(  x As Ref)

	  ''' <summary>
	  ''' Writes an SQL <code>BLOB</code> value to the stream.
	  ''' </summary>
	  ''' <param name="x"> a <code>Blob</code> object representing data of an SQL
	  ''' <code>BLOB</code> value
	  ''' </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  ''' this method
	  ''' @since 1.2 </exception>
	  Sub writeBlob(  x As Blob)

	  ''' <summary>
	  ''' Writes an SQL <code>CLOB</code> value to the stream.
	  ''' </summary>
	  ''' <param name="x"> a <code>Clob</code> object representing data of an SQL
	  ''' <code>CLOB</code> value
	  ''' </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  ''' this method
	  ''' @since 1.2 </exception>
	  Sub writeClob(  x As Clob)

	  ''' <summary>
	  ''' Writes an SQL structured type value to the stream.
	  ''' </summary>
	  ''' <param name="x"> a <code>Struct</code> object representing data of an SQL
	  ''' structured type
	  ''' </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  ''' this method
	  ''' @since 1.2 </exception>
	  Sub writeStruct(  x As Struct)

	  ''' <summary>
	  ''' Writes an SQL <code>ARRAY</code> value to the stream.
	  ''' </summary>
	  ''' <param name="x"> an <code>Array</code> object representing data of an SQL
	  ''' <code>ARRAY</code> type
	  ''' </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  ''' this method
	  ''' @since 1.2 </exception>
	  Sub writeArray(  x As Array)

		 '--------------------------- JDBC 3.0 ------------------------

		 ''' <summary>
		 ''' Writes a SQL <code>DATALINK</code> value to the stream.
		 ''' </summary>
		 ''' <param name="x"> a <code>java.net.URL</code> object representing the data
		 ''' of SQL DATALINK type
		 ''' </param>
		 ''' <exception cref="SQLException"> if a database access error occurs </exception>
		 ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		 ''' this method
		 ''' @since 1.4 </exception>
		 Sub writeURL(  x As java.net.URL)

		 '--------------------------- JDBC 4.0 ------------------------

	  ''' <summary>
	  ''' Writes the next attribute to the stream as a <code>String</code>
	  ''' in the Java programming language. The driver converts this to a
	  ''' SQL <code>NCHAR</code> or
	  ''' <code>NVARCHAR</code> or <code>LONGNVARCHAR</code> value
	  ''' (depending on the argument's
	  ''' size relative to the driver's limits on <code>NVARCHAR</code> values)
	  ''' when it sends it to the stream.
	  ''' </summary>
	  ''' <param name="x"> the value to pass to the database </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  ''' this method
	  ''' @since 1.6 </exception>
	  Sub writeNString(  x As String)

	  ''' <summary>
	  ''' Writes an SQL <code>NCLOB</code> value to the stream.
	  ''' </summary>
	  ''' <param name="x"> a <code>NClob</code> object representing data of an SQL
	  ''' <code>NCLOB</code> value
	  ''' </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  ''' this method
	  ''' @since 1.6 </exception>
	  Sub writeNClob(  x As NClob)


	  ''' <summary>
	  ''' Writes an SQL <code>ROWID</code> value to the stream.
	  ''' </summary>
	  ''' <param name="x"> a <code>RowId</code> object representing data of an SQL
	  ''' <code>ROWID</code> value
	  ''' </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  ''' this method
	  ''' @since 1.6 </exception>
	  Sub writeRowId(  x As RowId)


	  ''' <summary>
	  ''' Writes an SQL <code>XML</code> value to the stream.
	  ''' </summary>
	  ''' <param name="x"> a <code>SQLXML</code> object representing data of an SQL
	  ''' <code>XML</code> value
	  ''' </param>
	  ''' <exception cref="SQLException"> if a database access error occurs,
	  ''' the <code>java.xml.transform.Result</code>,
	  '''  <code>Writer</code> or <code>OutputStream</code> has not been closed for the <code>SQLXML</code> object or
	  '''  if there is an error processing the XML value.  The <code>getCause</code> method
	  '''  of the exception may provide a more detailed exception, for example, if the
	  '''  stream does not contain valid XML. </exception>
	  ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  ''' this method
	  ''' @since 1.6 </exception>
	  Sub writeSQLXML(  x As SQLXML)

	  '--------------------------JDBC 4.2 -----------------------------

	  ''' <summary>
	  ''' Writes to the stream the data contained in the given object. The
	  ''' object will be converted to the specified targetSqlType
	  ''' before being sent to the stream.
	  ''' <p>
	  ''' When the {@code object} is {@code null}, this
	  ''' method writes an SQL {@code NULL} to the stream.
	  ''' <p>
	  ''' If the object has a custom mapping (is of a class implementing the
	  ''' interface {@code SQLData}),
	  ''' the JDBC driver should call the method {@code SQLData.writeSQL} to
	  ''' write it to the SQL data stream.
	  ''' If, on the other hand, the object is of a class implementing
	  ''' {@code Ref}, {@code Blob}, {@code Clob},  {@code NClob},
	  '''  {@code Struct}, {@code java.net.URL},
	  ''' or {@code Array}, the driver should pass it to the database as a
	  ''' value of the corresponding SQL type.
	  ''' <P>
	  ''' The default implementation will throw {@code SQLFeatureNotSupportedException}
	  ''' </summary>
	  ''' <param name="x"> the object containing the input parameter value </param>
	  ''' <param name="targetSqlType"> the SQL type to be sent to the database. </param>
	  ''' <exception cref="SQLException"> if a database access error occurs  or
	  '''            if the Java Object specified by x is an InputStream
	  '''            or Reader object and the value of the scale parameter is less
	  '''            than zero </exception>
	  ''' <exception cref="SQLFeatureNotSupportedException"> if
	  ''' the JDBC driver does not support this data type </exception>
	  ''' <seealso cref= JDBCType </seealso>
	  ''' <seealso cref= SQLType
	  ''' @since 1.8 </seealso>
	  default Sub writeObject(  x As Object,   targetSqlType As SQLType)
			throw Function SQLFeatureNotSupportedException() As New

	 End Interface


End Namespace