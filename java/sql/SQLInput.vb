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
	''' An input stream that contains a stream of values representing an
	''' instance of an SQL structured type or an SQL distinct type.
	''' This interface, used only for custom mapping, is used by the driver
	''' behind the scenes, and a programmer never directly invokes
	''' <code>SQLInput</code> methods. The <i>reader</i> methods
	''' (<code>readLong</code>, <code>readBytes</code>, and so on)
	''' provide a way  for an implementation of the <code>SQLData</code>
	'''  interface to read the values in an <code>SQLInput</code> object.
	'''  And as described in <code>SQLData</code>, calls to reader methods must
	''' be made in the order that their corresponding attributes appear in the
	''' SQL definition of the type.
	''' The method <code>wasNull</code> is used to determine whether
	''' the last value read was SQL <code>NULL</code>.
	''' <P>When the method <code>getObject</code> is called with an
	''' object of a class implementing the interface <code>SQLData</code>,
	''' the JDBC driver calls the method <code>SQLData.getSQLType</code>
	''' to determine the SQL type of the user-defined type (UDT)
	''' being custom mapped. The driver
	''' creates an instance of <code>SQLInput</code>, populating it with the
	''' attributes of the UDT.  The driver then passes the input
	''' stream to the method <code>SQLData.readSQL</code>, which in turn
	''' calls the <code>SQLInput</code> reader methods
	''' in its implementation for reading the
	''' attributes from the input stream.
	''' @since 1.2
	''' </summary>

	Public Interface SQLInput


		'================================================================
		' Methods for reading attributes from the stream of SQL data.
		' These methods correspond to the column-accessor methods of
		' java.sql.ResultSet.
		'================================================================

		''' <summary>
		''' Reads the next attribute in the stream and returns it as a <code>String</code>
		''' in the Java programming language.
		''' </summary>
		''' <returns> the attribute; if the value is SQL <code>NULL</code>, returns <code>null</code> </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		''' this method
		''' @since 1.2 </exception>
		Function readString() As String

		''' <summary>
		''' Reads the next attribute in the stream and returns it as a <code>boolean</code>
		''' in the Java programming language.
		''' </summary>
		''' <returns> the attribute; if the value is SQL <code>NULL</code>, returns <code>false</code> </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		''' this method
		''' @since 1.2 </exception>
		Function readBoolean() As Boolean

		''' <summary>
		''' Reads the next attribute in the stream and returns it as a <code>byte</code>
		''' in the Java programming language.
		''' </summary>
		''' <returns> the attribute; if the value is SQL <code>NULL</code>, returns <code>0</code> </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		''' this method
		''' @since 1.2 </exception>
		Function readByte() As SByte

		''' <summary>
		''' Reads the next attribute in the stream and returns it as a <code>short</code>
		''' in the Java programming language.
		''' </summary>
		''' <returns> the attribute; if the value is SQL <code>NULL</code>, returns <code>0</code> </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		''' this method
		''' @since 1.2 </exception>
		Function readShort() As Short

		''' <summary>
		''' Reads the next attribute in the stream and returns it as an <code>int</code>
		''' in the Java programming language.
		''' </summary>
		''' <returns> the attribute; if the value is SQL <code>NULL</code>, returns <code>0</code> </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		''' this method
		''' @since 1.2 </exception>
		Function readInt() As Integer

		''' <summary>
		''' Reads the next attribute in the stream and returns it as a <code>long</code>
		''' in the Java programming language.
		''' </summary>
		''' <returns> the attribute; if the value is SQL <code>NULL</code>, returns <code>0</code> </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		''' this method
		''' @since 1.2 </exception>
		Function readLong() As Long

		''' <summary>
		''' Reads the next attribute in the stream and returns it as a <code>float</code>
		''' in the Java programming language.
		''' </summary>
		''' <returns> the attribute; if the value is SQL <code>NULL</code>, returns <code>0</code> </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		''' this method
		''' @since 1.2 </exception>
		Function readFloat() As Single

		''' <summary>
		''' Reads the next attribute in the stream and returns it as a <code>double</code>
		''' in the Java programming language.
		''' </summary>
		''' <returns> the attribute; if the value is SQL <code>NULL</code>, returns <code>0</code> </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		''' this method
		''' @since 1.2 </exception>
		Function readDouble() As Double

		''' <summary>
		''' Reads the next attribute in the stream and returns it as a <code>java.math.BigDecimal</code>
		''' object in the Java programming language.
		''' </summary>
		''' <returns> the attribute; if the value is SQL <code>NULL</code>, returns <code>null</code> </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		''' this method
		''' @since 1.2 </exception>
		Function readBigDecimal() As Decimal

		''' <summary>
		''' Reads the next attribute in the stream and returns it as an array of bytes
		''' in the Java programming language.
		''' </summary>
		''' <returns> the attribute; if the value is SQL <code>NULL</code>, returns <code>null</code> </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		''' this method
		''' @since 1.2 </exception>
		Function readBytes() As SByte()

		''' <summary>
		''' Reads the next attribute in the stream and returns it as a <code>java.sql.Date</code> object.
		''' </summary>
		''' <returns> the attribute; if the value is SQL <code>NULL</code>, returns <code>null</code> </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		''' this method
		''' @since 1.2 </exception>
		Function readDate() As java.sql.Date

		''' <summary>
		''' Reads the next attribute in the stream and returns it as a <code>java.sql.Time</code> object.
		''' </summary>
		''' <returns> the attribute; if the value is SQL <code>NULL</code>, returns <code>null</code> </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		''' this method
		''' @since 1.2 </exception>
		Function readTime() As java.sql.Time

		''' <summary>
		''' Reads the next attribute in the stream and returns it as a <code>java.sql.Timestamp</code> object.
		''' </summary>
		''' <returns> the attribute; if the value is SQL <code>NULL</code>, returns <code>null</code> </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		''' this method
		''' @since 1.2 </exception>
		Function readTimestamp() As java.sql.Timestamp

		''' <summary>
		''' Reads the next attribute in the stream and returns it as a stream of Unicode characters.
		''' </summary>
		''' <returns> the attribute; if the value is SQL <code>NULL</code>, returns <code>null</code> </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		''' this method
		''' @since 1.2 </exception>
		Function readCharacterStream() As java.io.Reader

		''' <summary>
		''' Reads the next attribute in the stream and returns it as a stream of ASCII characters.
		''' </summary>
		''' <returns> the attribute; if the value is SQL <code>NULL</code>, returns <code>null</code> </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		''' this method
		''' @since 1.2 </exception>
		Function readAsciiStream() As java.io.InputStream

		''' <summary>
		''' Reads the next attribute in the stream and returns it as a stream of uninterpreted
		''' bytes.
		''' </summary>
		''' <returns> the attribute; if the value is SQL <code>NULL</code>, returns <code>null</code> </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		''' this method
		''' @since 1.2 </exception>
		Function readBinaryStream() As java.io.InputStream

		'================================================================
		' Methods for reading items of SQL user-defined types from the stream.
		'================================================================

		''' <summary>
		''' Reads the datum at the head of the stream and returns it as an
		''' <code>Object</code> in the Java programming language.  The
		''' actual type of the object returned is determined by the default type
		''' mapping, and any customizations present in this stream's type map.
		''' 
		''' <P>A type map is registered with the stream by the JDBC driver before the
		''' stream is passed to the application.
		''' 
		''' <P>When the datum at the head of the stream is an SQL <code>NULL</code>,
		''' the method returns <code>null</code>.  If the datum is an SQL structured or distinct
		''' type, it determines the SQL type of the datum at the head of the stream.
		''' If the stream's type map has an entry for that SQL type, the driver
		''' constructs an object of the appropriate class and calls the method
		''' <code>SQLData.readSQL</code> on that object, which reads additional data from the
		''' stream, using the protocol described for that method.
		''' </summary>
		''' <returns> the datum at the head of the stream as an <code>Object</code> in the
		''' Java programming language;<code>null</code> if the datum is SQL <code>NULL</code> </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		''' this method
		''' @since 1.2 </exception>
		Function readObject() As Object

		''' <summary>
		''' Reads an SQL <code>REF</code> value from the stream and returns it as a
		''' <code>Ref</code> object in the Java programming language.
		''' </summary>
		''' <returns> a <code>Ref</code> object representing the SQL <code>REF</code> value
		''' at the head of the stream; <code>null</code> if the value read is
		''' SQL <code>NULL</code> </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		''' this method
		''' @since 1.2 </exception>
		Function readRef() As Ref

		''' <summary>
		''' Reads an SQL <code>BLOB</code> value from the stream and returns it as a
		''' <code>Blob</code> object in the Java programming language.
		''' </summary>
		''' <returns> a <code>Blob</code> object representing data of the SQL <code>BLOB</code> value
		''' at the head of the stream; <code>null</code> if the value read is
		''' SQL <code>NULL</code> </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		''' this method
		''' @since 1.2 </exception>
		Function readBlob() As Blob

		''' <summary>
		''' Reads an SQL <code>CLOB</code> value from the stream and returns it as a
		''' <code>Clob</code> object in the Java programming language.
		''' </summary>
		''' <returns> a <code>Clob</code> object representing data of the SQL <code>CLOB</code> value
		''' at the head of the stream; <code>null</code> if the value read is
		''' SQL <code>NULL</code> </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		''' this method
		''' @since 1.2 </exception>
		Function readClob() As Clob

		''' <summary>
		''' Reads an SQL <code>ARRAY</code> value from the stream and returns it as an
		''' <code>Array</code> object in the Java programming language.
		''' </summary>
		''' <returns> an <code>Array</code> object representing data of the SQL
		''' <code>ARRAY</code> value at the head of the stream; <code>null</code>
		''' if the value read is SQL <code>NULL</code> </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		''' this method
		''' @since 1.2 </exception>
		Function readArray() As Array

		''' <summary>
		''' Retrieves whether the last value read was SQL <code>NULL</code>.
		''' </summary>
		''' <returns> <code>true</code> if the most recently read SQL value was SQL
		''' <code>NULL</code>; <code>false</code> otherwise </returns>
		''' <exception cref="SQLException"> if a database access error occurs
		''' </exception>
		''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		''' this method
		''' @since 1.2 </exception>
		Function wasNull() As Boolean

		'---------------------------- JDBC 3.0 -------------------------

		''' <summary>
		''' Reads an SQL <code>DATALINK</code> value from the stream and returns it as a
		''' <code>java.net.URL</code> object in the Java programming language.
		''' </summary>
		''' <returns> a <code>java.net.URL</code> object. </returns>
		''' <exception cref="SQLException"> if a database access error occurs,
		'''            or if a URL is malformed </exception>
		''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		''' this method
		''' @since 1.4 </exception>
		Function readURL() As java.net.URL

		 '---------------------------- JDBC 4.0 -------------------------

		''' <summary>
		''' Reads an SQL <code>NCLOB</code> value from the stream and returns it as a
		''' <code>NClob</code> object in the Java programming language.
		''' </summary>
		''' <returns> a <code>NClob</code> object representing data of the SQL <code>NCLOB</code> value
		''' at the head of the stream; <code>null</code> if the value read is
		''' SQL <code>NULL</code> </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		''' this method
		''' @since 1.6 </exception>
		Function readNClob() As NClob

		''' <summary>
		''' Reads the next attribute in the stream and returns it as a <code>String</code>
		''' in the Java programming language. It is intended for use when
		''' accessing  <code>NCHAR</code>,<code>NVARCHAR</code>
		''' and <code>LONGNVARCHAR</code> columns.
		''' </summary>
		''' <returns> the attribute; if the value is SQL <code>NULL</code>, returns <code>null</code> </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		''' this method
		''' @since 1.6 </exception>
		Function readNString() As String

		''' <summary>
		''' Reads an SQL <code>XML</code> value from the stream and returns it as a
		''' <code>SQLXML</code> object in the Java programming language.
		''' </summary>
		''' <returns> a <code>SQLXML</code> object representing data of the SQL <code>XML</code> value
		''' at the head of the stream; <code>null</code> if the value read is
		''' SQL <code>NULL</code> </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		''' this method
		''' @since 1.6 </exception>
		Function readSQLXML() As SQLXML

		''' <summary>
		''' Reads an SQL <code>ROWID</code> value from the stream and returns it as a
		''' <code>RowId</code> object in the Java programming language.
		''' </summary>
		''' <returns> a <code>RowId</code> object representing data of the SQL <code>ROWID</code> value
		''' at the head of the stream; <code>null</code> if the value read is
		''' SQL <code>NULL</code> </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		''' this method
		''' @since 1.6 </exception>
		Function readRowId() As RowId

		'--------------------------JDBC 4.2 -----------------------------

		''' <summary>
		''' Reads the next attribute in the stream and returns it as an
		''' {@code Object} in the Java programming language. The
		''' actual type of the object returned is determined by the specified
		''' Java data type, and any customizations present in this
		''' stream's type map.
		''' 
		''' <P>A type map is registered with the stream by the JDBC driver before the
		''' stream is passed to the application.
		''' 
		''' <P>When the attribute at the head of the stream is an SQL {@code NULL}
		''' the method returns {@code null}. If the attribute is an SQL
		''' structured or distinct
		''' type, it determines the SQL type of the attribute at the head of the stream.
		''' If the stream's type map has an entry for that SQL type, the driver
		''' constructs an object of the appropriate class and calls the method
		''' {@code SQLData.readSQL} on that object, which reads additional data from the
		''' stream, using the protocol described for that method.
		''' <p>
		''' The default implementation will throw {@code SQLFeatureNotSupportedException}
		''' </summary>
		''' @param <T> the type of the class modeled by this Class object </param>
		''' <param name="type"> Class representing the Java data type to convert the attribute to. </param>
		''' <returns> the attribute at the head of the stream as an {@code Object} in the
		''' Java programming language;{@code null} if the attribute is SQL {@code NULL} </returns>
		''' <exception cref="SQLException"> if a database access error occurs </exception>
		''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
		''' this method
		''' @since 1.8 </exception>
		default Function readObject(  type As [Class]) As T(Of T)
		   throw Function SQLFeatureNotSupportedException() As New
	End Interface

End Namespace