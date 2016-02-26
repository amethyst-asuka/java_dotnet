Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.sql.rowset.serial


	''' <summary>
	''' An input stream used for custom mapping user-defined types (UDTs).
	''' An <code>SQLInputImpl</code> object is an input stream that contains a
	''' stream of values that are the attributes of a UDT.
	''' <p>
	''' This class is used by the driver behind the scenes when the method
	''' <code>getObject</code> is called on an SQL structured or distinct type
	''' that has a custom mapping; a programmer never invokes
	''' <code>SQLInputImpl</code> methods directly. They are provided here as a
	''' convenience for those who write <code>RowSet</code> implementations.
	''' <P>
	''' The <code>SQLInputImpl</code> class provides a set of
	''' reader methods analogous to the <code>ResultSet</code> getter
	''' methods.  These methods make it possible to read the values in an
	''' <code>SQLInputImpl</code> object.
	''' <P>
	''' The method <code>wasNull</code> is used to determine whether the
	''' the last value read was SQL <code>NULL</code>.
	''' <P>When the method <code>getObject</code> is called with an
	''' object of a class implementing the interface <code>SQLData</code>,
	''' the JDBC driver calls the method <code>SQLData.getSQLType</code>
	''' to determine the SQL type of the UDT being custom mapped. The driver
	''' creates an instance of <code>SQLInputImpl</code>, populating it with the
	''' attributes of the UDT.  The driver then passes the input
	''' stream to the method <code>SQLData.readSQL</code>, which in turn
	''' calls the <code>SQLInputImpl</code> reader methods
	''' to read the attributes from the input stream.
	''' @since 1.5 </summary>
	''' <seealso cref= java.sql.SQLData </seealso>
	Public Class SQLInputImpl
		Implements SQLInput

		''' <summary>
		''' <code>true</code> if the last value returned was <code>SQL NULL</code>;
		''' <code>false</code> otherwise.
		''' </summary>
		Private lastValueWasNull As Boolean

		''' <summary>
		''' The current index into the array of SQL structured type attributes
		''' that will be read from this <code>SQLInputImpl</code> object and
		''' mapped to the fields of a class in the Java programming language.
		''' </summary>
		Private idx As Integer

		''' <summary>
		''' The array of attributes to be read from this stream.  The order
		''' of the attributes is the same as the order in which they were
		''' listed in the SQL definition of the UDT.
		''' </summary>
		Private attrib As Object()

		''' <summary>
		''' The type map to use when the method <code>readObject</code>
		''' is invoked. This is a <code>java.util.Map</code> object in which
		''' there may be zero or more entries.  Each entry consists of the
		''' fully qualified name of a UDT (the value to be mapped) and the
		''' <code>Class</code> object for a class that implements
		''' <code>SQLData</code> (the Java class that defines how the UDT
		''' will be mapped).
		''' </summary>
		Private map As IDictionary(Of String, Type)


		''' <summary>
		''' Creates an <code>SQLInputImpl</code> object initialized with the
		''' given array of attributes and the given type map. If any of the
		''' attributes is a UDT whose name is in an entry in the type map,
		''' the attribute will be mapped according to the corresponding
		''' <code>SQLData</code> implementation.
		''' </summary>
		''' <param name="attributes"> an array of <code>Object</code> instances in which
		'''        each element is an attribute of a UDT. The order of the
		'''        attributes in the array is the same order in which
		'''        the attributes were defined in the UDT definition. </param>
		''' <param name="map"> a <code>java.util.Map</code> object containing zero or more
		'''        entries, with each entry consisting of 1) a <code>String</code>
		'''        giving the fully
		'''        qualified name of the UDT and 2) the <code>Class</code> object
		'''        for the <code>SQLData</code> implementation that defines how
		'''        the UDT is to be mapped </param>
		''' <exception cref="SQLException"> if the <code>attributes</code> or the <code>map</code>
		'''        is a <code>null</code> value </exception>

		Public Sub New(ByVal attributes As Object(), ByVal map As IDictionary(Of String, Type))
			If (attributes Is Nothing) OrElse (map Is Nothing) Then Throw New SQLException("Cannot instantiate a SQLInputImpl " & "object with null parameters")
			' assign our local reference to the attribute stream
			attrib = java.util.Arrays.copyOf(attributes, attributes.Length)
			' init the index point before the head of the stream
			idx = -1
			' set the map
			Me.map = map
		End Sub


		''' <summary>
		''' Retrieves the next attribute in this <code>SQLInputImpl</code> object
		''' as an <code>Object</code> in the Java programming language.
		''' </summary>
		''' <returns> the next value in the input stream
		'''         as an <code>Object</code> in the Java programming language </returns>
		''' <exception cref="SQLException"> if the read position is located at an invalid
		'''         position or if there are no further values in the stream </exception>
		Private Property nextAttribute As Object
			Get
				idx += 1
				If idx >= attrib.Length Then
					Throw New SQLException("SQLInputImpl exception: Invalid read " & "position")
				Else
					lastValueWasNull = attrib(idx) Is Nothing
					Return attrib(idx)
				End If
			End Get
		End Property


		'================================================================
		' Methods for reading attributes from the stream of SQL data.
		' These methods correspond to the column-accessor methods of
		' java.sql.ResultSet.
		'================================================================

		''' <summary>
		''' Retrieves the next attribute in this <code>SQLInputImpl</code> object as
		''' a <code>String</code> in the Java programming language.
		''' <p>
		''' This method does not perform type-safe checking to determine if the
		''' returned type is the expected type; this responsibility is delegated
		''' to the UDT mapping as defined by a <code>SQLData</code>
		''' implementation.
		''' <p> </summary>
		''' <returns> the next attribute in this <code>SQLInputImpl</code> object;
		'''     if the value is <code>SQL NULL</code>, return <code>null</code> </returns>
		''' <exception cref="SQLException"> if the read position is located at an invalid
		'''     position or if there are no further values in the stream. </exception>
		Public Overridable Function readString() As String
			Return CStr(nextAttribute)
		End Function

		''' <summary>
		''' Retrieves the next attribute in this <code>SQLInputImpl</code> object as
		''' a <code>boolean</code> in the Java programming language.
		''' <p>
		''' This method does not perform type-safe checking to determine if the
		''' returned type is the expected type; this responsibility is delegated
		''' to the UDT mapping as defined by a <code>SQLData</code>
		''' implementation.
		''' <p> </summary>
		''' <returns> the next attribute in this <code>SQLInputImpl</code> object;
		'''     if the value is <code>SQL NULL</code>, return <code>null</code> </returns>
		''' <exception cref="SQLException"> if the read position is located at an invalid
		'''     position or if there are no further values in the stream. </exception>
		Public Overridable Function readBoolean() As Boolean
			Dim attrib As Boolean? = CBool(nextAttribute)
			Return If(attrib Is Nothing, False, attrib)
		End Function

		''' <summary>
		''' Retrieves the next attribute in this <code>SQLInputImpl</code> object as
		''' a <code>byte</code> in the Java programming language.
		''' <p>
		''' This method does not perform type-safe checking to determine if the
		''' returned type is the expected type; this responsibility is delegated
		''' to the UDT mapping as defined by a <code>SQLData</code>
		''' implementation.
		''' <p> </summary>
		''' <returns> the next attribute in this <code>SQLInputImpl</code> object;
		'''     if the value is <code>SQL NULL</code>, return <code>null</code> </returns>
		''' <exception cref="SQLException"> if the read position is located at an invalid
		'''     position or if there are no further values in the stream </exception>
		Public Overridable Function readByte() As SByte
			Dim attrib As SByte? = CByte(nextAttribute)
			Return If(attrib Is Nothing, 0, attrib)
		End Function

		''' <summary>
		''' Retrieves the next attribute in this <code>SQLInputImpl</code> object
		''' as a <code>short</code> in the Java programming language.
		''' <P>
		''' This method does not perform type-safe checking to determine if the
		''' returned type is the expected type; this responsibility is delegated
		''' to the UDT mapping as defined by a <code>SQLData</code> implementation.
		''' <P> </summary>
		''' <returns> the next attribute in this <code>SQLInputImpl</code> object;
		'''       if the value is <code>SQL NULL</code>, return <code>null</code> </returns>
		''' <exception cref="SQLException"> if the read position is located at an invalid
		'''       position or if there are no more values in the stream </exception>
		Public Overridable Function readShort() As Short
			Dim attrib As Short? = CShort(Fix(nextAttribute))
			Return If(attrib Is Nothing, 0, attrib)
		End Function

		''' <summary>
		''' Retrieves the next attribute in this <code>SQLInputImpl</code> object
		''' as an <code>int</code> in the Java programming language.
		''' <P>
		''' This method does not perform type-safe checking to determine if the
		''' returned type is the expected type; this responsibility is delegated
		''' to the UDT mapping as defined by a <code>SQLData</code> implementation.
		''' <P> </summary>
		''' <returns> the next attribute in this <code>SQLInputImpl</code> object;
		'''       if the value is <code>SQL NULL</code>, return <code>null</code> </returns>
		''' <exception cref="SQLException"> if the read position is located at an invalid
		'''       position or if there are no more values in the stream </exception>
		Public Overridable Function readInt() As Integer
			Dim attrib As Integer? = CInt(Fix(nextAttribute))
			Return If(attrib Is Nothing, 0, attrib)
		End Function

		''' <summary>
		''' Retrieves the next attribute in this <code>SQLInputImpl</code> object
		''' as a <code>long</code> in the Java programming language.
		''' <P>
		''' This method does not perform type-safe checking to determine if the
		''' returned type is the expected type; this responsibility is delegated
		''' to the UDT mapping as defined by a <code>SQLData</code> implementation.
		''' <P> </summary>
		''' <returns> the next attribute in this <code>SQLInputImpl</code> object;
		'''       if the value is <code>SQL NULL</code>, return <code>null</code> </returns>
		''' <exception cref="SQLException"> if the read position is located at an invalid
		'''       position or if there are no more values in the stream </exception>
		Public Overridable Function readLong() As Long
			Dim attrib As Long? = CLng(Fix(nextAttribute))
			Return If(attrib Is Nothing, 0, attrib)
		End Function

		''' <summary>
		''' Retrieves the next attribute in this <code>SQLInputImpl</code> object
		''' as a <code>float</code> in the Java programming language.
		''' <P>
		''' This method does not perform type-safe checking to determine if the
		''' returned type is the expected type; this responsibility is delegated
		''' to the UDT mapping as defined by a <code>SQLData</code> implementation.
		''' <P> </summary>
		''' <returns> the next attribute in this <code>SQLInputImpl</code> object;
		'''       if the value is <code>SQL NULL</code>, return <code>null</code> </returns>
		''' <exception cref="SQLException"> if the read position is located at an invalid
		'''       position or if there are no more values in the stream </exception>
		Public Overridable Function readFloat() As Single
			Dim attrib As Single? = CSng(nextAttribute)
			Return If(attrib Is Nothing, 0, attrib)
		End Function

		''' <summary>
		''' Retrieves the next attribute in this <code>SQLInputImpl</code> object
		''' as a <code>double</code> in the Java programming language.
		''' <P>
		''' This method does not perform type-safe checking to determine if the
		''' returned type is the expected type; this responsibility is delegated
		''' to the UDT mapping as defined by a <code>SQLData</code> implementation.
		''' <P> </summary>
		''' <returns> the next attribute in this <code>SQLInputImpl</code> object;
		'''       if the value is <code>SQL NULL</code>, return <code>null</code> </returns>
		''' <exception cref="SQLException"> if the read position is located at an invalid
		'''       position or if there are no more values in the stream </exception>
		Public Overridable Function readDouble() As Double
			Dim attrib As Double? = CDbl(nextAttribute)
			Return If(attrib Is Nothing, 0, attrib)
		End Function

		''' <summary>
		''' Retrieves the next attribute in this <code>SQLInputImpl</code> object
		''' as a <code>java.math.BigDecimal</code>.
		''' <P>
		''' This method does not perform type-safe checking to determine if the
		''' returned type is the expected type; this responsibility is delegated
		''' to the UDT mapping as defined by a <code>SQLData</code> implementation.
		''' <P> </summary>
		''' <returns> the next attribute in this <code>SQLInputImpl</code> object;
		'''       if the value is <code>SQL NULL</code>, return <code>null</code> </returns>
		''' <exception cref="SQLException"> if the read position is located at an invalid
		'''       position or if there are no more values in the stream </exception>
		Public Overridable Function readBigDecimal() As Decimal
			Return CDec(nextAttribute)
		End Function

		''' <summary>
		''' Retrieves the next attribute in this <code>SQLInputImpl</code> object
		''' as an array of bytes.
		''' <p>
		''' This method does not perform type-safe checking to determine if the
		''' returned type is the expected type; this responsibility is delegated
		''' to the UDT mapping as defined by a <code>SQLData</code> implementation.
		''' <P> </summary>
		''' <returns> the next attribute in this <code>SQLInputImpl</code> object;
		'''       if the value is <code>SQL NULL</code>, return <code>null</code> </returns>
		''' <exception cref="SQLException"> if the read position is located at an invalid
		'''       position or if there are no more values in the stream </exception>
		Public Overridable Function readBytes() As SByte()
			Return CType(nextAttribute, SByte())
		End Function

		''' <summary>
		''' Retrieves the next attribute in this <code>SQLInputImpl</code> as
		''' a <code>java.sql.Date</code> object.
		''' <P>
		''' This method does not perform type-safe checking to determine if the
		''' returned type is the expected type; this responsibility is delegated
		''' to the UDT mapping as defined by a <code>SQLData</code> implementation.
		''' <P> </summary>
		''' <returns> the next attribute in this <code>SQLInputImpl</code> object;
		'''       if the value is <code>SQL NULL</code>, return <code>null</code> </returns>
		''' <exception cref="SQLException"> if the read position is located at an invalid
		'''       position or if there are no more values in the stream </exception>
		Public Overridable Function readDate() As java.sql.Date
			Return CType(nextAttribute, java.sql.Date)
		End Function

		''' <summary>
		''' Retrieves the next attribute in this <code>SQLInputImpl</code> object as
		''' a <code>java.sql.Time</code> object.
		''' <P>
		''' This method does not perform type-safe checking to determine if the
		''' returned type is the expected type as this responsibility is delegated
		''' to the UDT mapping as implemented by a <code>SQLData</code>
		''' implementation.
		''' </summary>
		''' <returns> the attribute; if the value is <code>SQL NULL</code>, return
		''' <code>null</code> </returns>
		''' <exception cref="SQLException"> if the read position is located at an invalid
		''' position; or if there are no further values in the stream. </exception>
		Public Overridable Function readTime() As java.sql.Time
			Return CType(nextAttribute, java.sql.Time)
		End Function

		''' <summary>
		''' Retrieves the next attribute in this <code>SQLInputImpl</code> object as
		''' a <code>java.sql.Timestamp</code> object.
		''' </summary>
		''' <returns> the attribute; if the value is <code>SQL NULL</code>, return
		''' <code>null</code> </returns>
		''' <exception cref="SQLException"> if the read position is located at an invalid
		''' position; or if there are no further values in the stream. </exception>
		Public Overridable Function readTimestamp() As java.sql.Timestamp
			Return CType(nextAttribute, java.sql.Timestamp)
		End Function

		''' <summary>
		''' Retrieves the next attribute in this <code>SQLInputImpl</code> object
		''' as a stream of Unicode characters.
		''' <P>
		''' This method does not perform type-safe checking to determine if the
		''' returned type is the expected type as this responsibility is delegated
		''' to the UDT mapping as implemented by a <code>SQLData</code>
		''' implementation.
		''' </summary>
		''' <returns> the attribute; if the value is <code>SQL NULL</code>, return <code>null</code> </returns>
		''' <exception cref="SQLException"> if the read position is located at an invalid
		''' position; or if there are no further values in the stream. </exception>
		Public Overridable Function readCharacterStream() As java.io.Reader
			Return CType(nextAttribute, java.io.Reader)
		End Function

		''' <summary>
		''' Returns the next attribute in this <code>SQLInputImpl</code> object
		''' as a stream of ASCII characters.
		''' <P>
		''' This method does not perform type-safe checking to determine if the
		''' returned type is the expected type as this responsibility is delegated
		''' to the UDT mapping as implemented by a <code>SQLData</code>
		''' implementation.
		''' </summary>
		''' <returns> the attribute; if the value is <code>SQL NULL</code>,
		''' return <code>null</code> </returns>
		''' <exception cref="SQLException"> if the read position is located at an invalid
		''' position; or if there are no further values in the stream. </exception>
		Public Overridable Function readAsciiStream() As java.io.InputStream
			Return CType(nextAttribute, java.io.InputStream)
		End Function

		''' <summary>
		''' Returns the next attribute in this <code>SQLInputImpl</code> object
		''' as a stream of uninterpreted bytes.
		''' <P>
		''' This method does not perform type-safe checking to determine if the
		''' returned type is the expected type as this responsibility is delegated
		''' to the UDT mapping as implemented by a <code>SQLData</code>
		''' implementation.
		''' </summary>
		''' <returns> the attribute; if the value is <code>SQL NULL</code>, return
		''' <code>null</code> </returns>
		''' <exception cref="SQLException"> if the read position is located at an invalid
		''' position; or if there are no further values in the stream. </exception>
		Public Overridable Function readBinaryStream() As java.io.InputStream
			Return CType(nextAttribute, java.io.InputStream)
		End Function

		'================================================================
		' Methods for reading items of SQL user-defined types from the stream.
		'================================================================

		''' <summary>
		''' Retrieves the value at the head of this <code>SQLInputImpl</code>
		''' object as an <code>Object</code> in the Java programming language.  The
		''' actual type of the object returned is determined by the default
		''' mapping of SQL types to types in the Java programming language unless
		''' there is a custom mapping, in which case the type of the object
		''' returned is determined by this stream's type map.
		''' <P>
		''' The JDBC technology-enabled driver registers a type map with the stream
		''' before passing the stream to the application.
		''' <P>
		''' When the datum at the head of the stream is an SQL <code>NULL</code>,
		''' this method returns <code>null</code>.  If the datum is an SQL
		''' structured or distinct type with a custom mapping, this method
		''' determines the SQL type of the datum at the head of the stream,
		''' constructs an object of the appropriate class, and calls the method
		''' <code>SQLData.readSQL</code> on that object. The <code>readSQL</code>
		''' method then calls the appropriate <code>SQLInputImpl.readXXX</code>
		''' methods to retrieve the attribute values from the stream.
		''' </summary>
		''' <returns> the value at the head of the stream as an <code>Object</code>
		'''         in the Java programming language; <code>null</code> if
		'''         the value is SQL <code>NULL</code> </returns>
		''' <exception cref="SQLException"> if the read position is located at an invalid
		''' position; or if there are no further values in the stream. </exception>
		Public Overridable Function readObject() As Object
			Dim attrib As Object = nextAttribute
			If TypeOf attrib Is Struct Then
				Dim s As Struct = CType(attrib, Struct)
				' look up the class in the map
				Dim c As Type = map(s.sQLTypeName)
				If c IsNot Nothing Then
					' create new instance of the class
					Dim obj As SQLData = Nothing
					Try
						obj = CType(sun.reflect.misc.ReflectUtil.newInstance(c), SQLData)
					Catch ex As Exception
						Throw New SQLException("Unable to Instantiate: ", ex)
					End Try
					' get the attributes from the struct
					Dim attribs As Object() = s.getAttributes(map)
					' create the SQLInput "stream"
					Dim sqlInput As New SQLInputImpl(attribs, map)
					' read the values...
					obj.readSQL(sqlInput, s.sQLTypeName)
					Return obj
				End If
			End If
			Return attrib
		End Function

		''' <summary>
		''' Retrieves the value at the head of this <code>SQLInputImpl</code> object
		''' as a <code>Ref</code> object in the Java programming language.
		''' </summary>
		''' <returns> a <code>Ref</code> object representing the SQL
		'''         <code>REF</code> value at the head of the stream; if the value
		'''         is <code>SQL NULL</code> return <code>null</code> </returns>
		''' <exception cref="SQLException"> if the read position is located at an invalid
		'''         position; or if there are no further values in the stream. </exception>
		Public Overridable Function readRef() As Ref
			Return CType(nextAttribute, Ref)
		End Function

		''' <summary>
		''' Retrieves the <code>BLOB</code> value at the head of this
		''' <code>SQLInputImpl</code> object as a <code>Blob</code> object
		''' in the Java programming language.
		''' <P>
		''' This method does not perform type-safe checking to determine if the
		''' returned type is the expected type as this responsibility is delegated
		''' to the UDT mapping as implemented by a <code>SQLData</code>
		''' implementation.
		''' </summary>
		''' <returns> a <code>Blob</code> object representing the SQL
		'''         <code>BLOB</code> value at the head of this stream;
		'''         if the value is <code>SQL NULL</code>, return
		'''         <code>null</code> </returns>
		''' <exception cref="SQLException"> if the read position is located at an invalid
		''' position; or if there are no further values in the stream. </exception>
		Public Overridable Function readBlob() As Blob
			Return CType(nextAttribute, Blob)
		End Function

		''' <summary>
		''' Retrieves the <code>CLOB</code> value at the head of this
		''' <code>SQLInputImpl</code> object as a <code>Clob</code> object
		''' in the Java programming language.
		''' <P>
		''' This method does not perform type-safe checking to determine if the
		''' returned type is the expected type as this responsibility is delegated
		''' to the UDT mapping as implemented by a <code>SQLData</code>
		''' implementation.
		''' </summary>
		''' <returns> a <code>Clob</code> object representing the SQL
		'''         <code>CLOB</code> value at the head of the stream;
		'''         if the value is <code>SQL NULL</code>, return
		'''         <code>null</code> </returns>
		''' <exception cref="SQLException"> if the read position is located at an invalid
		''' position; or if there are no further values in the stream. </exception>
		Public Overridable Function readClob() As Clob
			Return CType(nextAttribute, Clob)
		End Function

		''' <summary>
		''' Reads an SQL <code>ARRAY</code> value from the stream and
		''' returns it as an <code>Array</code> object in the Java programming
		''' language.
		''' <P>
		''' This method does not perform type-safe checking to determine if the
		''' returned type is the expected type as this responsibility is delegated
		''' to the UDT mapping as implemented by a <code>SQLData</code>
		''' implementation.
		''' </summary>
		''' <returns> an <code>Array</code> object representing the SQL
		'''         <code>ARRAY</code> value at the head of the stream; *
		'''         if the value is <code>SQL NULL</code>, return
		'''         <code>null</code> </returns>
		''' <exception cref="SQLException"> if the read position is located at an invalid
		''' position; or if there are no further values in the stream.
		'''  </exception>
		Public Overridable Function readArray() As Array
			Return CType(nextAttribute, Array)
		End Function

		''' <summary>
		''' Ascertains whether the last value read from this
		''' <code>SQLInputImpl</code> object was <code>null</code>.
		''' </summary>
		''' <returns> <code>true</code> if the SQL value read most recently was
		'''         <code>null</code>; otherwise, <code>false</code>; by default it
		'''         will return false </returns>
		''' <exception cref="SQLException"> if an error occurs determining the last value
		'''         read was a <code>null</code> value or not; </exception>
		Public Overridable Function wasNull() As Boolean
			Return lastValueWasNull
		End Function

		''' <summary>
		''' Reads an SQL <code>DATALINK</code> value from the stream and
		''' returns it as an <code>URL</code> object in the Java programming
		''' language.
		''' <P>
		''' This method does not perform type-safe checking to determine if the
		''' returned type is the expected type as this responsibility is delegated
		''' to the UDT mapping as implemented by a <code>SQLData</code>
		''' implementation.
		''' </summary>
		''' <returns> an <code>URL</code> object representing the SQL
		'''         <code>DATALINK</code> value at the head of the stream; *
		'''         if the value is <code>SQL NULL</code>, return
		'''         <code>null</code> </returns>
		''' <exception cref="SQLException"> if the read position is located at an invalid
		''' position; or if there are no further values in the stream. </exception>
		Public Overridable Function readURL() As java.net.URL
			Return CType(nextAttribute, java.net.URL)
		End Function

		'---------------------------- JDBC 4.0 -------------------------

		''' <summary>
		''' Reads an SQL <code>NCLOB</code> value from the stream and returns it as a
		''' <code>Clob</code> object in the Java programming language.
		''' </summary>
		''' <returns> a <code>NClob</code> object representing data of the SQL <code>NCLOB</code> value
		''' at the head of the stream; <code>null</code> if the value read is
		''' SQL <code>NULL</code> </returns>
		''' <exception cref="SQLException"> if a database access error occurs
		''' @since 1.6 </exception>
		 Public Overridable Function readNClob() As NClob
			Return CType(nextAttribute, NClob)
		 End Function

		''' <summary>
		''' Reads the next attribute in the stream and returns it as a <code>String</code>
		''' in the Java programming language. It is intended for use when
		''' accessing  <code>NCHAR</code>,<code>NVARCHAR</code>
		''' and <code>LONGNVARCHAR</code> columns.
		''' </summary>
		''' <returns> the attribute; if the value is SQL <code>NULL</code>, returns <code>null</code> </returns>
		''' <exception cref="SQLException"> if a database access error occurs
		''' @since 1.6 </exception>
		Public Overridable Function readNString() As String
			Return CStr(nextAttribute)
		End Function

		''' <summary>
		''' Reads an SQL <code>XML</code> value from the stream and returns it as a
		''' <code>SQLXML</code> object in the Java programming language.
		''' </summary>
		''' <returns> a <code>SQLXML</code> object representing data of the SQL <code>XML</code> value
		''' at the head of the stream; <code>null</code> if the value read is
		''' SQL <code>NULL</code> </returns>
		''' <exception cref="SQLException"> if a database access error occurs
		''' @since 1.6 </exception>
		Public Overridable Function readSQLXML() As SQLXML
			Return CType(nextAttribute, SQLXML)
		End Function

		''' <summary>
		''' Reads an SQL <code>ROWID</code> value from the stream and returns it as a
		''' <code>RowId</code> object in the Java programming language.
		''' </summary>
		''' <returns> a <code>RowId</code> object representing data of the SQL <code>ROWID</code> value
		''' at the head of the stream; <code>null</code> if the value read is
		''' SQL <code>NULL</code> </returns>
		''' <exception cref="SQLException"> if a database access error occurs
		''' @since 1.6 </exception>
		Public Overridable Function readRowId() As RowId
			Return CType(nextAttribute, RowId)
		End Function


	End Class

End Namespace