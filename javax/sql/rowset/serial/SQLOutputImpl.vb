Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Text

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
	''' The output stream for writing the attributes of a
	''' custom-mapped user-defined type (UDT) back to the database.
	''' The driver uses this interface internally, and its
	''' methods are never directly invoked by an application programmer.
	''' <p>
	''' When an application calls the
	''' method <code>PreparedStatement.setObject</code>, the driver
	''' checks to see whether the value to be written is a UDT with
	''' a custom mapping.  If it is, there will be an entry in a
	''' type map containing the <code>Class</code> object for the
	''' class that implements <code>SQLData</code> for this UDT.
	''' If the value to be written is an instance of <code>SQLData</code>,
	''' the driver will create an instance of <code>SQLOutputImpl</code>
	''' and pass it to the method <code>SQLData.writeSQL</code>.
	''' The method <code>writeSQL</code> in turn calls the
	''' appropriate <code>SQLOutputImpl.writeXXX</code> methods
	''' to write data from the <code>SQLData</code> object to
	''' the <code>SQLOutputImpl</code> output stream as the
	''' representation of an SQL user-defined type.
	''' </summary>
	Public Class SQLOutputImpl
		Implements SQLOutput

		''' <summary>
		''' A reference to an existing vector that
		''' contains the attributes of a <code>Struct</code> object.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private attribs As ArrayList

		''' <summary>
		''' The type map the driver supplies to a newly created
		''' <code>SQLOutputImpl</code> object.  This type map
		''' indicates the <code>SQLData</code> class whose
		''' <code>writeSQL</code> method will be called.  This
		''' method will in turn call the appropriate
		''' <code>SQLOutputImpl</code> writer methods.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private map As IDictionary

		''' <summary>
		''' Creates a new <code>SQLOutputImpl</code> object
		''' initialized with the given vector of attributes and
		''' type map.  The driver will use the type map to determine
		''' which <code>SQLData.writeSQL</code> method to invoke.
		''' This method will then call the appropriate
		''' <code>SQLOutputImpl</code> writer methods in order and
		''' thereby write the attributes to the new output stream.
		''' </summary>
		''' <param name="attributes"> a <code>Vector</code> object containing the attributes of
		'''        the UDT to be mapped to one or more objects in the Java
		'''        programming language
		''' </param>
		''' <param name="map"> a <code>java.util.Map</code> object containing zero or
		'''        more entries, with each entry consisting of 1) a <code>String</code>
		'''        giving the fully qualified name of a UDT and 2) the
		'''        <code>Class</code> object for the <code>SQLData</code> implementation
		'''        that defines how the UDT is to be mapped </param>
		''' <exception cref="SQLException"> if the <code>attributes</code> or the <code>map</code>
		'''        is a <code>null</code> value </exception>
		Public Sub New(Of T1, T2)(ByVal attributes As List(Of T1), ByVal map As IDictionary(Of T2))
			If (attributes Is Nothing) OrElse (map Is Nothing) Then Throw New SQLException("Cannot instantiate a SQLOutputImpl " & "instance with null parameters")
			Me.attribs = attributes
			Me.map = map
		End Sub

		'================================================================
		' Methods for writing attributes to the stream of SQL data.
		' These methods correspond to the column-accessor methods of
		' java.sql.ResultSet.
		'================================================================

		''' <summary>
		''' Writes a <code>String</code> in the Java programming language
		''' to this <code>SQLOutputImpl</code> object. The driver converts
		''' it to an SQL <code>CHAR</code>, <code>VARCHAR</code>, or
		''' <code>LONGVARCHAR</code> before returning it to the database.
		''' </summary>
		''' <param name="x"> the value to pass to the database </param>
		''' <exception cref="SQLException"> if the <code>SQLOutputImpl</code> object is in
		'''        use by a <code>SQLData</code> object attempting to write the attribute
		'''        values of a UDT to the database. </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub writeString(ByVal x As String)
			'System.out.println("Adding :"+x);
			attribs.Add(x)
		End Sub

		''' <summary>
		''' Writes a <code>boolean</code> in the Java programming language
		''' to this <code>SQLOutputImpl</code> object. The driver converts
		''' it to an SQL <code>BIT</code> before returning it to the database.
		''' </summary>
		''' <param name="x"> the value to pass to the database </param>
		''' <exception cref="SQLException"> if the <code>SQLOutputImpl</code> object is in
		'''        use by a <code>SQLData</code> object attempting to write the attribute
		'''        values of a UDT to the database. </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub writeBoolean(ByVal x As Boolean)
			attribs.Add(Convert.ToBoolean(x))
		End Sub

		''' <summary>
		''' Writes a <code>byte</code> in the Java programming language
		''' to this <code>SQLOutputImpl</code> object. The driver converts
		''' it to an SQL <code>BIT</code> before returning it to the database.
		''' </summary>
		''' <param name="x"> the value to pass to the database </param>
		''' <exception cref="SQLException"> if the <code>SQLOutputImpl</code> object is in
		'''        use by a <code>SQLData</code> object attempting to write the attribute
		'''        values of a UDT to the database. </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub writeByte(ByVal x As SByte)
			attribs.Add(Convert.ToByte(x))
		End Sub

		''' <summary>
		''' Writes a <code>short</code> in the Java programming language
		''' to this <code>SQLOutputImpl</code> object. The driver converts
		''' it to an SQL <code>SMALLINT</code> before returning it to the database.
		''' </summary>
		''' <param name="x"> the value to pass to the database </param>
		''' <exception cref="SQLException"> if the <code>SQLOutputImpl</code> object is in
		'''        use by a <code>SQLData</code> object attempting to write the attribute
		'''        values of a UDT to the database. </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub writeShort(ByVal x As Short)
			attribs.Add(Convert.ToInt16(x))
		End Sub

		''' <summary>
		''' Writes an <code>int</code> in the Java programming language
		''' to this <code>SQLOutputImpl</code> object. The driver converts
		''' it to an SQL <code>INTEGER</code> before returning it to the database.
		''' </summary>
		''' <param name="x"> the value to pass to the database </param>
		''' <exception cref="SQLException"> if the <code>SQLOutputImpl</code> object is in
		'''        use by a <code>SQLData</code> object attempting to write the attribute
		'''        values of a UDT to the database. </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub writeInt(ByVal x As Integer)
			attribs.Add(Convert.ToInt32(x))
		End Sub

		''' <summary>
		''' Writes a <code>long</code> in the Java programming language
		''' to this <code>SQLOutputImpl</code> object. The driver converts
		''' it to an SQL <code>BIGINT</code> before returning it to the database.
		''' </summary>
		''' <param name="x"> the value to pass to the database </param>
		''' <exception cref="SQLException"> if the <code>SQLOutputImpl</code> object is in
		'''        use by a <code>SQLData</code> object attempting to write the attribute
		'''        values of a UDT to the database. </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub writeLong(ByVal x As Long)
			attribs.Add(Convert.ToInt64(x))
		End Sub

		''' <summary>
		''' Writes a <code>float</code> in the Java programming language
		''' to this <code>SQLOutputImpl</code> object. The driver converts
		''' it to an SQL <code>REAL</code> before returning it to the database.
		''' </summary>
		''' <param name="x"> the value to pass to the database </param>
		''' <exception cref="SQLException"> if the <code>SQLOutputImpl</code> object is in
		'''        use by a <code>SQLData</code> object attempting to write the attribute
		'''        values of a UDT to the database. </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub writeFloat(ByVal x As Single)
			attribs.Add(Convert.ToSingle(x))
		End Sub

		''' <summary>
		''' Writes a <code>double</code> in the Java programming language
		''' to this <code>SQLOutputImpl</code> object. The driver converts
		''' it to an SQL <code>DOUBLE</code> before returning it to the database.
		''' </summary>
		''' <param name="x"> the value to pass to the database </param>
		''' <exception cref="SQLException"> if the <code>SQLOutputImpl</code> object is in
		'''        use by a <code>SQLData</code> object attempting to write the attribute
		'''        values of a UDT to the database. </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub writeDouble(ByVal x As Double)
			attribs.Add(Convert.ToDouble(x))
		End Sub

		''' <summary>
		''' Writes a <code>java.math.BigDecimal</code> object in the Java programming
		''' language to this <code>SQLOutputImpl</code> object. The driver converts
		''' it to an SQL <code>NUMERIC</code> before returning it to the database.
		''' </summary>
		''' <param name="x"> the value to pass to the database </param>
		''' <exception cref="SQLException"> if the <code>SQLOutputImpl</code> object is in
		'''        use by a <code>SQLData</code> object attempting to write the attribute
		'''        values of a UDT to the database. </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub writeBigDecimal(ByVal x As Decimal)
			attribs.Add(x)
		End Sub

		''' <summary>
		''' Writes an array of <code>bytes</code> in the Java programming language
		''' to this <code>SQLOutputImpl</code> object. The driver converts
		''' it to an SQL <code>VARBINARY</code> or <code>LONGVARBINARY</code>
		''' before returning it to the database.
		''' </summary>
		''' <param name="x"> the value to pass to the database </param>
		''' <exception cref="SQLException"> if the <code>SQLOutputImpl</code> object is in
		'''        use by a <code>SQLData</code> object attempting to write the attribute
		'''        values of a UDT to the database. </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub writeBytes(ByVal x As SByte())
			attribs.Add(x)
		End Sub

		''' <summary>
		''' Writes a <code>java.sql.Date</code> object in the Java programming
		''' language to this <code>SQLOutputImpl</code> object. The driver converts
		''' it to an SQL <code>DATE</code> before returning it to the database.
		''' </summary>
		''' <param name="x"> the value to pass to the database </param>
		''' <exception cref="SQLException"> if the <code>SQLOutputImpl</code> object is in
		'''        use by a <code>SQLData</code> object attempting to write the attribute
		'''        values of a UDT to the database. </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub writeDate(ByVal x As java.sql.Date)
			attribs.Add(x)
		End Sub

		''' <summary>
		''' Writes a <code>java.sql.Time</code> object in the Java programming
		''' language to this <code>SQLOutputImpl</code> object. The driver converts
		''' it to an SQL <code>TIME</code> before returning it to the database.
		''' </summary>
		''' <param name="x"> the value to pass to the database </param>
		''' <exception cref="SQLException"> if the <code>SQLOutputImpl</code> object is in
		'''        use by a <code>SQLData</code> object attempting to write the attribute
		'''        values of a UDT to the database. </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub writeTime(ByVal x As java.sql.Time)
			attribs.Add(x)
		End Sub

		''' <summary>
		''' Writes a <code>java.sql.Timestamp</code> object in the Java programming
		''' language to this <code>SQLOutputImpl</code> object. The driver converts
		''' it to an SQL <code>TIMESTAMP</code> before returning it to the database.
		''' </summary>
		''' <param name="x"> the value to pass to the database </param>
		''' <exception cref="SQLException"> if the <code>SQLOutputImpl</code> object is in
		'''        use by a <code>SQLData</code> object attempting to write the attribute
		'''        values of a UDT to the database. </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub writeTimestamp(ByVal x As java.sql.Timestamp)
			attribs.Add(x)
		End Sub

		''' <summary>
		''' Writes a stream of Unicode characters to this
		''' <code>SQLOutputImpl</code> object. The driver will do any necessary
		''' conversion from Unicode to the database <code>CHAR</code> format.
		''' </summary>
		''' <param name="x"> the value to pass to the database </param>
		''' <exception cref="SQLException"> if the <code>SQLOutputImpl</code> object is in
		'''        use by a <code>SQLData</code> object attempting to write the attribute
		'''        values of a UDT to the database. </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub writeCharacterStream(ByVal x As java.io.Reader)
			 Dim bufReader As New java.io.BufferedReader(x)
			 Try
				 Dim i As Integer
				 i = bufReader.read()
				 Do While i <> -1
					Dim ch As Char = ChrW(i)
					Dim strBuf As New StringBuilder
					strBuf.Append(ch)

					Dim str As New String(strBuf)
					Dim strLine As String = bufReader.readLine()

					writeString(str + strLine)
					 i = bufReader.read()
				 Loop
			 Catch ioe As java.io.IOException

			 End Try
		End Sub

		''' <summary>
		''' Writes a stream of ASCII characters to this
		''' <code>SQLOutputImpl</code> object. The driver will do any necessary
		''' conversion from ASCII to the database <code>CHAR</code> format.
		''' </summary>
		''' <param name="x"> the value to pass to the database </param>
		''' <exception cref="SQLException"> if the <code>SQLOutputImpl</code> object is in
		'''        use by a <code>SQLData</code> object attempting to write the attribute
		'''        values of a UDT to the database. </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub writeAsciiStream(ByVal x As java.io.InputStream)
			 Dim bufReader As New java.io.BufferedReader(New java.io.InputStreamReader(x))
			 Try
				   Dim i As Integer
				   i=bufReader.read()
				   Do While i <> -1
					Dim ch As Char = ChrW(i)

					Dim strBuf As New StringBuilder
					strBuf.Append(ch)

					Dim str As New String(strBuf)
					Dim strLine As String = bufReader.readLine()

					writeString(str + strLine)
					   i=bufReader.read()
				   Loop
			  Catch ioe As java.io.IOException
				Throw New SQLException(ioe.Message)
			  End Try
		End Sub

		''' <summary>
		''' Writes a stream of uninterpreted bytes to this <code>SQLOutputImpl</code>
		''' object.
		''' </summary>
		''' <param name="x"> the value to pass to the database </param>
		''' <exception cref="SQLException"> if the <code>SQLOutputImpl</code> object is in
		'''        use by a <code>SQLData</code> object attempting to write the attribute
		'''        values of a UDT to the database. </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub writeBinaryStream(ByVal x As java.io.InputStream)
			 Dim bufReader As New java.io.BufferedReader(New java.io.InputStreamReader(x))
			 Try
				   Dim i As Integer
				 i=bufReader.read()
				 Do While i <> -1
					Dim ch As Char = ChrW(i)

					Dim strBuf As New StringBuilder
					strBuf.Append(ch)

					Dim str As New String(strBuf)
					Dim strLine As String = bufReader.readLine()

					writeString(str + strLine)
					 i=bufReader.read()
				 Loop
			Catch ioe As java.io.IOException
				Throw New SQLException(ioe.Message)
			End Try
		End Sub

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
		''' <P>
		''' The implementation of the method <code>SQLData.writeSQ</code>
		''' calls the appropriate <code>SQLOutputImpl.writeXXX</code> method(s)
		''' for writing each of the object's attributes in order.
		''' The attributes must be read from an <code>SQLInput</code>
		''' input stream and written to an <code>SQLOutputImpl</code>
		''' output stream in the same order in which they were
		''' listed in the SQL definition of the user-defined type.
		''' </summary>
		''' <param name="x"> the object representing data of an SQL structured or
		'''          distinct type </param>
		''' <exception cref="SQLException"> if the <code>SQLOutputImpl</code> object is in
		'''        use by a <code>SQLData</code> object attempting to write the attribute
		'''        values of a UDT to the database. </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub writeObject(ByVal x As SQLData)

	'        
	'         * Except for the types that are passed as objects
	'         * this seems to be the only way for an object to
	'         * get a null value for a field in a structure.
	'         *
	'         * Note: this means that the class defining SQLData
	'         * will need to track if a field is SQL null for itself
	'         
			If x Is Nothing Then
				attribs.Add(Nothing)
			Else
	'            
	'             * We have to write out a SerialStruct that contains
	'             * the name of this class otherwise we don't know
	'             * what to re-instantiate during readSQL()
	'             
				attribs.Add(New SerialStruct(x, map))
			End If
		End Sub

		''' <summary>
		''' Writes a <code>Ref</code> object in the Java programming language
		''' to this <code>SQLOutputImpl</code> object.  The driver converts
		''' it to a serializable <code>SerialRef</code> SQL <code>REF</code> value
		''' before returning it to the database.
		''' </summary>
		''' <param name="x"> an object representing an SQL <code>REF</code> value </param>
		''' <exception cref="SQLException"> if the <code>SQLOutputImpl</code> object is in
		'''        use by a <code>SQLData</code> object attempting to write the attribute
		'''        values of a UDT to the database. </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub writeRef(ByVal x As Ref)
			If x Is Nothing Then
				attribs.Add(Nothing)
			Else
				attribs.Add(New SerialRef(x))
			End If
		End Sub

		''' <summary>
		''' Writes a <code>Blob</code> object in the Java programming language
		''' to this <code>SQLOutputImpl</code> object.  The driver converts
		''' it to a serializable <code>SerialBlob</code> SQL <code>BLOB</code> value
		''' before returning it to the database.
		''' </summary>
		''' <param name="x"> an object representing an SQL <code>BLOB</code> value </param>
		''' <exception cref="SQLException"> if the <code>SQLOutputImpl</code> object is in
		'''        use by a <code>SQLData</code> object attempting to write the attribute
		'''        values of a UDT to the database. </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub writeBlob(ByVal x As Blob)
			If x Is Nothing Then
				attribs.Add(Nothing)
			Else
				attribs.Add(New SerialBlob(x))
			End If
		End Sub

		''' <summary>
		''' Writes a <code>Clob</code> object in the Java programming language
		''' to this <code>SQLOutputImpl</code> object.  The driver converts
		''' it to a serializable <code>SerialClob</code> SQL <code>CLOB</code> value
		''' before returning it to the database.
		''' </summary>
		''' <param name="x"> an object representing an SQL <code>CLOB</code> value </param>
		''' <exception cref="SQLException"> if the <code>SQLOutputImpl</code> object is in
		'''        use by a <code>SQLData</code> object attempting to write the attribute
		'''        values of a UDT to the database. </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub writeClob(ByVal x As Clob)
			If x Is Nothing Then
				attribs.Add(Nothing)
			Else
				attribs.Add(New SerialClob(x))
			End If
		End Sub

		''' <summary>
		''' Writes a <code>Struct</code> object in the Java
		''' programming language to this <code>SQLOutputImpl</code>
		''' object. The driver converts this value to an SQL structured type
		''' before returning it to the database.
		''' <P>
		''' This method should be used when an SQL structured type has been
		''' mapped to a <code>Struct</code> object in the Java programming
		''' language (the standard mapping).  The method
		''' <code>writeObject</code> should be used if an SQL structured type
		''' has been custom mapped to a class in the Java programming language.
		''' </summary>
		''' <param name="x"> an object representing the attributes of an SQL structured type </param>
		''' <exception cref="SQLException"> if the <code>SQLOutputImpl</code> object is in
		'''        use by a <code>SQLData</code> object attempting to write the attribute
		'''        values of a UDT to the database. </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub writeStruct(ByVal x As Struct)
			Dim s As New SerialStruct(x,map)
			attribs.Add(s)
		End Sub

		''' <summary>
		''' Writes an <code>Array</code> object in the Java
		''' programming language to this <code>SQLOutputImpl</code>
		''' object. The driver converts this value to a serializable
		''' <code>SerialArray</code> SQL <code>ARRAY</code>
		''' value before returning it to the database.
		''' </summary>
		''' <param name="x"> an object representing an SQL <code>ARRAY</code> value </param>
		''' <exception cref="SQLException"> if the <code>SQLOutputImpl</code> object is in
		'''        use by a <code>SQLData</code> object attempting to write the attribute
		'''        values of a UDT to the database. </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub writeArray(ByVal x As Array)
			If x Is Nothing Then
				attribs.Add(Nothing)
			Else
				attribs.Add(New SerialArray(x, map))
			End If
		End Sub

		''' <summary>
		''' Writes an <code>java.sql.Type.DATALINK</code> object in the Java
		''' programming language to this <code>SQLOutputImpl</code> object. The
		''' driver converts this value to a serializable <code>SerialDatalink</code>
		''' SQL <code>DATALINK</code> value before return it to the database.
		''' </summary>
		''' <param name="url"> an object representing a SQL <code>DATALINK</code> value </param>
		''' <exception cref="SQLException"> if the <code>SQLOutputImpl</code> object is in
		'''        use by a <code>SQLData</code> object attempting to write the attribute
		'''        values of a UDT to the database. </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Sub writeURL(ByVal url As java.net.URL)
			If url Is Nothing Then
				attribs.Add(Nothing)
			Else
				attribs.Add(New SerialDatalink(url))
			End If
		End Sub


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
	  ''' <exception cref="SQLException"> if a database access error occurs
	  ''' @since 1.6 </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	   Public Overridable Sub writeNString(ByVal x As String)
		   attribs.Add(x)
	   End Sub

	  ''' <summary>
	  ''' Writes an SQL <code>NCLOB</code> value to the stream.
	  ''' </summary>
	  ''' <param name="x"> a <code>NClob</code> object representing data of an SQL
	  ''' <code>NCLOB</code> value
	  ''' </param>
	  ''' <exception cref="SQLException"> if a database access error occurs
	  ''' @since 1.6 </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	   Public Overridable Sub writeNClob(ByVal x As NClob)
			   attribs.Add(x)
	   End Sub


	  ''' <summary>
	  ''' Writes an SQL <code>ROWID</code> value to the stream.
	  ''' </summary>
	  ''' <param name="x"> a <code>RowId</code> object representing data of an SQL
	  ''' <code>ROWID</code> value
	  ''' </param>
	  ''' <exception cref="SQLException"> if a database access error occurs
	  ''' @since 1.6 </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	   Public Overridable Sub writeRowId(ByVal x As RowId)
			attribs.Add(x)
	   End Sub


	  ''' <summary>
	  ''' Writes an SQL <code>XML</code> value to the stream.
	  ''' </summary>
	  ''' <param name="x"> a <code>SQLXML</code> object representing data of an SQL
	  ''' <code>XML</code> value
	  ''' </param>
	  ''' <exception cref="SQLException"> if a database access error occurs
	  ''' @since 1.6 </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	   Public Overridable Sub writeSQLXML(ByVal x As SQLXML)
			attribs.Add(x)
	   End Sub

	End Class

End Namespace