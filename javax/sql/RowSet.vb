Imports System
Imports System.Collections.Generic
Imports java.io
Imports java.sql

'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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
    ''' The interface that adds support to the JDBC API for the
    ''' JavaBeans&trade; component model.
    ''' A rowset, which can be used as a JavaBeans component in
    ''' a visual Bean development environment, can be created and
    ''' configured at design time and executed at run time.
    ''' <P>
    ''' The <code>RowSet</code>
    ''' interface provides a set of JavaBeans properties that allow a <code>RowSet</code>
    ''' instance to be configured to connect to a JDBC data source and read
    ''' some data from the data source.  A group of setter methods (<code>setInt</code>,
    ''' <code>setBytes</code>, <code>setString</code>, and so on)
    ''' provide a way to pass input parameters to a rowset's command property.
    ''' This command is the SQL query the rowset uses when it gets its data from
    ''' a relational database, which is generally the case.
    ''' <P>
    ''' The <code>RowSet</code>
    ''' interface supports JavaBeans events, allowing other components in an
    ''' application to be notified when an event occurs on a rowset,
    ''' such as a change in its value.
    ''' 
    ''' <P>The <code>RowSet</code> interface is unique in that it is intended to be
    ''' implemented using the rest of the JDBC API.  In other words, a
    ''' <code>RowSet</code> implementation is a layer of software that executes "on top"
    ''' of a JDBC driver.  Implementations of the <code>RowSet</code> interface can
    ''' be provided by anyone, including JDBC driver vendors who want to
    ''' provide a <code>RowSet</code> implementation as part of their JDBC products.
    ''' <P>
    ''' A <code>RowSet</code> object may make a connection with a data source and
    ''' maintain that connection throughout its life cycle, in which case it is
    ''' called a <i>connected</i> rowset.  A rowset may also make a connection with
    ''' a data source, get data from it, and then close the connection. Such a rowset
    ''' is called a <i>disconnected</i> rowset.  A disconnected rowset may make
    ''' changes to its data while it is disconnected and then send the changes back
    ''' to the original source of the data, but it must reestablish a connection to do so.
    ''' <P>
    ''' A disconnected rowset may have a reader (a <code>RowSetReader</code> object)
    ''' and a writer (a <code>RowSetWriter</code> object) associated with it.
    ''' The reader may be implemented in many different ways to populate a rowset
    ''' with data, including getting data from a non-relational data source. The
    ''' writer can also be implemented in many different ways to propagate changes
    ''' made to the rowset's data back to the underlying data source.
    ''' <P>
    ''' Rowsets are easy to use.  The <code>RowSet</code> interface extends the standard
    ''' <code>java.sql.ResultSet</code> interface.  The <code>RowSetMetaData</code>
    ''' interface extends the <code>java.sql.ResultSetMetaData</code> interface.
    ''' Thus, developers familiar
    ''' with the JDBC API will have to learn a minimal number of new APIs to
    ''' use rowsets.  In addition, third-party software tools that work with
    ''' JDBC <code>ResultSet</code> objects will also easily be made to work with rowsets.
    ''' 
    ''' @since 1.4
    ''' </summary>

    Public Interface RowSet
        Inherits ResultSet

        '-----------------------------------------------------------------------
        ' Properties
        '-----------------------------------------------------------------------

        '-----------------------------------------------------------------------
        ' The following properties may be used to create a Connection.
        '-----------------------------------------------------------------------

        ''' <summary>
        ''' Retrieves the url property this <code>RowSet</code> object will use to
        ''' create a connection if it uses the <code>DriverManager</code>
        ''' instead of a <code>DataSource</code> object to establish the connection.
        ''' The default value is <code>null</code>.
        ''' </summary>
        ''' <returns> a string url </returns>
        ''' <exception cref="SQLException"> if a database access error occurs </exception>
        ''' <seealso cref= #setUrl </seealso>
        Property url As String


        ''' <summary>
        ''' Retrieves the logical name that identifies the data source for this
        ''' <code>RowSet</code> object.
        ''' </summary>
        ''' <returns> a data source name </returns>
        ''' <seealso cref= #setDataSourceName </seealso>
        ''' <seealso cref= #setUrl </seealso>
        Property dataSourceName As String


        ''' <summary>
        ''' Retrieves the username used to create a database connection for this
        ''' <code>RowSet</code> object.
        ''' The username property is set at run time before calling the method
        ''' <code>execute</code>.  It is
        ''' not usually part of the serialized state of a <code>RowSet</code> object.
        ''' </summary>
        ''' <returns> the username property </returns>
        ''' <seealso cref= #setUsername </seealso>
        Property username As String


        ''' <summary>
        ''' Retrieves the password used to create a database connection.
        ''' The password property is set at run time before calling the method
        ''' <code>execute</code>.  It is not usually part of the serialized state
        ''' of a <code>RowSet</code> object.
        ''' </summary>
        ''' <returns> the password for making a database connection </returns>
        ''' <seealso cref= #setPassword </seealso>
        Property password As String


        ''' <summary>
        ''' Retrieves the transaction isolation level set for this
        ''' <code>RowSet</code> object.
        ''' </summary>
        ''' <returns> the transaction isolation level; one of
        '''      <code>Connection.TRANSACTION_READ_UNCOMMITTED</code>,
        '''      <code>Connection.TRANSACTION_READ_COMMITTED</code>,
        '''      <code>Connection.TRANSACTION_REPEATABLE_READ</code>, or
        '''      <code>Connection.TRANSACTION_SERIALIZABLE</code> </returns>
        ''' <seealso cref= #setTransactionIsolation </seealso>
        Property transactionIsolation As Integer


        ''' <summary>
        ''' Retrieves the <code>Map</code> object associated with this
        ''' <code>RowSet</code> object, which specifies the custom mapping
        ''' of SQL user-defined types, if any.  The default is for the
        ''' type map to be empty.
        ''' </summary>
        ''' <returns> a <code>java.util.Map</code> object containing the names of
        '''         SQL user-defined types and the Java classes to which they are
        '''         to be mapped
        ''' </returns>
        ''' <exception cref="SQLException"> if a database access error occurs </exception>
        ''' <seealso cref= #setTypeMap </seealso>
        Property typeMap As IDictionary(Of String, Type)


        '-----------------------------------------------------------------------
        ' The following properties may be used to create a Statement.
        '-----------------------------------------------------------------------

        ''' <summary>
        ''' Retrieves this <code>RowSet</code> object's command property.
        ''' 
        ''' The command property contains a command string, which must be an SQL
        ''' query, that can be executed to fill the rowset with data.
        ''' The default value is <code>null</code>.
        ''' </summary>
        ''' <returns> the command string; may be <code>null</code> </returns>
        ''' <seealso cref= #setCommand </seealso>
        Property command As String


        ''' <summary>
        ''' Retrieves whether this <code>RowSet</code> object is read-only.
        ''' If updates are possible, the default is for a rowset to be
        ''' updatable.
        ''' <P>
        ''' Attempts to update a read-only rowset will result in an
        ''' <code>SQLException</code> being thrown.
        ''' </summary>
        ''' <returns> <code>true</code> if this <code>RowSet</code> object is
        '''         read-only; <code>false</code> if it is updatable </returns>
        ''' <seealso cref= #setReadOnly </seealso>
        Property [readOnly] As Boolean


        ''' <summary>
        ''' Retrieves the maximum number of bytes that may be returned
        ''' for certain column values.
        ''' This limit applies only to <code>BINARY</code>,
        ''' <code>VARBINARY</code>, <code>LONGVARBINARYBINARY</code>, <code>CHAR</code>,
        ''' <code>VARCHAR</code>, <code>LONGVARCHAR</code>, <code>NCHAR</code>
        ''' and <code>NVARCHAR</code> columns.
        ''' If the limit is exceeded, the excess data is silently discarded.
        ''' </summary>
        ''' <returns> the current maximum column size limit; zero means that there
        '''          is no limit </returns>
        ''' <exception cref="SQLException"> if a database access error occurs </exception>
        ''' <seealso cref= #setMaxFieldSize </seealso>
        Property maxFieldSize As Integer


        ''' <summary>
        ''' Retrieves the maximum number of rows that this <code>RowSet</code>
        ''' object can contain.
        ''' If the limit is exceeded, the excess rows are silently dropped.
        ''' </summary>
        ''' <returns> the current maximum number of rows that this <code>RowSet</code>
        '''         object can contain; zero means unlimited </returns>
        ''' <exception cref="SQLException"> if a database access error occurs </exception>
        ''' <seealso cref= #setMaxRows </seealso>
        Property maxRows As Integer


        ''' <summary>
        ''' Retrieves whether escape processing is enabled for this
        ''' <code>RowSet</code> object.
        ''' If escape scanning is enabled, which is the default, the driver will do
        ''' escape substitution before sending an SQL statement to the database.
        ''' </summary>
        ''' <returns> <code>true</code> if escape processing is enabled;
        '''         <code>false</code> if it is disabled </returns>
        ''' <exception cref="SQLException"> if a database access error occurs </exception>
        ''' <seealso cref= #setEscapeProcessing </seealso>
        Property escapeProcessing As Boolean


        ''' <summary>
        ''' Retrieves the maximum number of seconds the driver will wait for
        ''' a statement to execute.
        ''' If this limit is exceeded, an <code>SQLException</code> is thrown.
        ''' </summary>
        ''' <returns> the current query timeout limit in seconds; zero means
        '''          unlimited </returns>
        ''' <exception cref="SQLException"> if a database access error occurs </exception>
        ''' <seealso cref= #setQueryTimeout </seealso>
        Property queryTimeout As Integer


        ''' <summary>
        ''' Sets the type of this <code>RowSet</code> object to the given type.
        ''' This method is used to change the type of a rowset, which is by
        ''' default read-only and non-scrollable.
        ''' </summary>
        ''' <param name="type"> one of the <code>ResultSet</code> constants specifying a type:
        '''        <code>ResultSet.TYPE_FORWARD_ONLY</code>,
        '''        <code>ResultSet.TYPE_SCROLL_INSENSITIVE</code>, or
        '''        <code>ResultSet.TYPE_SCROLL_SENSITIVE</code> </param>
        ''' <exception cref="SQLException"> if a database access error occurs </exception>
        ''' <seealso cref= java.sql.ResultSet#getType </seealso>
        WriteOnly Property type As Integer

        ''' <summary>
        ''' Sets the concurrency of this <code>RowSet</code> object to the given
        ''' concurrency level. This method is used to change the concurrency level
        ''' of a rowset, which is by default <code>ResultSet.CONCUR_READ_ONLY</code>
        ''' </summary>
        ''' <param name="concurrency"> one of the <code>ResultSet</code> constants specifying a
        '''        concurrency level:  <code>ResultSet.CONCUR_READ_ONLY</code> or
        '''        <code>ResultSet.CONCUR_UPDATABLE</code> </param>
        ''' <exception cref="SQLException"> if a database access error occurs </exception>
        ''' <seealso cref= ResultSet#getConcurrency </seealso>
        WriteOnly Property concurrency As Integer

        '-----------------------------------------------------------------------
        ' Parameters
        '-----------------------------------------------------------------------

        ''' <summary>
        ''' The <code>RowSet</code> setter methods are used to set any input parameters
        ''' needed by the <code>RowSet</code> object's command.
        ''' Parameters are set at run time, as opposed to design time.
        ''' </summary>

        ''' <summary>
        ''' Sets the designated parameter in this <code>RowSet</code> object's SQL
        ''' command to SQL <code>NULL</code>.
        ''' 
        ''' <P><B>Note:</B> You must specify the parameter's SQL type.
        ''' </summary>
        ''' <param name="parameterIndex"> the first parameter is 1, the second is 2, ... </param>
        ''' <param name="sqlType"> a SQL type code defined by <code>java.sql.Types</code> </param>
        ''' <exception cref="SQLException"> if a database access error occurs </exception>
        Sub setNull(ByVal parameterIndex As Integer, ByVal sqlType As Integer)

        ''' <summary>
        ''' Sets the designated parameter to SQL <code>NULL</code>.
        '''   
        ''' <P><B>Note:</B> You must specify the parameter's SQL type.
        ''' </summary>
        ''' <param name="parameterName"> the name of the parameter </param>
        ''' <param name="sqlType"> the SQL type code defined in <code>java.sql.Types</code> </param>
        ''' <exception cref="SQLException"> if a database access error occurs or
        ''' this method is called on a closed <code>CallableStatement</code> </exception>
        ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
        ''' this method
        ''' @since 1.4 </exception>
        Sub setNull(ByVal parameterName As String, ByVal sqlType As Integer)

        ''' <summary>
        ''' Sets the designated parameter in this <code>RowSet</code> object's SQL
        ''' command to SQL <code>NULL</code>. This version of the method <code>setNull</code>
        ''' should  be used for SQL user-defined types (UDTs) and <code>REF</code> type
        ''' parameters.  Examples of UDTs include: <code>STRUCT</code>, <code>DISTINCT</code>,
        ''' <code>JAVA_OBJECT</code>, and named array types.
        ''' 
        ''' <P><B>Note:</B> To be portable, applications must give the
        ''' SQL type code and the fully qualified SQL type name when specifying
        ''' a NULL UDT or <code>REF</code> parameter.  In the case of a UDT,
        ''' the name is the type name of the parameter itself.  For a <code>REF</code>
        ''' parameter, the name is the type name of the referenced type.  If
        ''' a JDBC driver does not need the type code or type name information,
        ''' it may ignore it.
        ''' 
        ''' Although it is intended for UDT and <code>REF</code> parameters,
        ''' this method may be used to set a null parameter of any JDBC type.
        ''' If the parameter does not have a user-defined or <code>REF</code> type,
        ''' the typeName parameter is ignored.
        ''' 
        ''' </summary>
        ''' <param name="paramIndex"> the first parameter is 1, the second is 2, ... </param>
        ''' <param name="sqlType"> a value from <code>java.sql.Types</code> </param>
        ''' <param name="typeName"> the fully qualified name of an SQL UDT or the type
        '''        name of the SQL structured type being referenced by a <code>REF</code>
        '''        type; ignored if the parameter is not a UDT or <code>REF</code> type </param>
        ''' <exception cref="SQLException"> if a database access error occurs </exception>
        Sub setNull(ByVal paramIndex As Integer, ByVal sqlType As Integer, ByVal typeName As String)

        ''' <summary>
        ''' Sets the designated parameter to SQL <code>NULL</code>.
        ''' This version of the method <code>setNull</code> should
        ''' be used for user-defined types and REF type parameters.  Examples
        ''' of user-defined types include: STRUCT, DISTINCT, JAVA_OBJECT, and
        ''' named array types.
        '''   
        ''' <P><B>Note:</B> To be portable, applications must give the
        ''' SQL type code and the fully-qualified SQL type name when specifying
        ''' a NULL user-defined or REF parameter.  In the case of a user-defined type
        ''' the name is the type name of the parameter itself.  For a REF
        ''' parameter, the name is the type name of the referenced type.  If
        ''' a JDBC driver does not need the type code or type name information,
        ''' it may ignore it.
        '''   
        ''' Although it is intended for user-defined and Ref parameters,
        ''' this method may be used to set a null parameter of any JDBC type.
        ''' If the parameter does not have a user-defined or REF type, the given
        ''' typeName is ignored.
        '''   
        ''' </summary>
        ''' <param name="parameterName"> the name of the parameter </param>
        ''' <param name="sqlType"> a value from <code>java.sql.Types</code> </param>
        ''' <param name="typeName"> the fully-qualified name of an SQL user-defined type;
        '''        ignored if the parameter is not a user-defined type or
        '''        SQL <code>REF</code> value </param>
        ''' <exception cref="SQLException"> if a database access error occurs or
        ''' this method is called on a closed <code>CallableStatement</code> </exception>
        ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
        ''' this method
        ''' @since 1.4 </exception>
        Sub setNull(ByVal parameterName As String, ByVal sqlType As Integer, ByVal typeName As String)

        ''' <summary>
        ''' Sets the designated parameter in this <code>RowSet</code> object's command
        ''' to the given Java <code>boolean</code> value. The driver converts this to
        ''' an SQL <code>BIT</code> value before sending it to the database.
        ''' </summary>
        ''' <param name="parameterIndex"> the first parameter is 1, the second is 2, ... </param>
        ''' <param name="x"> the parameter value </param>
        ''' <exception cref="SQLException"> if a database access error occurs </exception>
        Sub setBoolean(ByVal parameterIndex As Integer, ByVal x As Boolean)

        ''' <summary>
        ''' Sets the designated parameter to the given Java <code>boolean</code> value.
        ''' The driver converts this
        ''' to an SQL <code>BIT</code> or <code>BOOLEAN</code> value when it sends it to the database.
        ''' </summary>
        ''' <param name="parameterName"> the name of the parameter </param>
        ''' <param name="x"> the parameter value </param>
        ''' <exception cref="SQLException"> if a database access error occurs or
        ''' this method is called on a closed <code>CallableStatement</code> </exception>
        ''' <seealso cref= #getBoolean </seealso>
        ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
        ''' this method
        ''' @since 1.4 </exception>
        Sub setBoolean(ByVal parameterName As String, ByVal x As Boolean)

        ''' <summary>
        ''' Sets the designated parameter in this <code>RowSet</code> object's command
        ''' to the given Java <code>byte</code> value. The driver converts this to
        ''' an SQL <code>TINYINT</code> value before sending it to the database.
        ''' </summary>
        ''' <param name="parameterIndex"> the first parameter is 1, the second is 2, ... </param>
        ''' <param name="x"> the parameter value </param>
        ''' <exception cref="SQLException"> if a database access error occurs </exception>
        Sub setByte(ByVal parameterIndex As Integer, ByVal x As SByte)

        ''' <summary>
        ''' Sets the designated parameter to the given Java <code>byte</code> value.
        ''' The driver converts this
        ''' to an SQL <code>TINYINT</code> value when it sends it to the database.
        ''' </summary>
        ''' <param name="parameterName"> the name of the parameter </param>
        ''' <param name="x"> the parameter value </param>
        ''' <exception cref="SQLException"> if a database access error occurs or
        ''' this method is called on a closed <code>CallableStatement</code> </exception>
        ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
        ''' this method </exception>
        ''' <seealso cref= #getByte
        ''' @since 1.4 </seealso>
        Sub setByte(ByVal parameterName As String, ByVal x As SByte)

        ''' <summary>
        ''' Sets the designated parameter in this <code>RowSet</code> object's command
        ''' to the given Java <code>short</code> value. The driver converts this to
        ''' an SQL <code>SMALLINT</code> value before sending it to the database.
        ''' </summary>
        ''' <param name="parameterIndex"> the first parameter is 1, the second is 2, ... </param>
        ''' <param name="x"> the parameter value </param>
        ''' <exception cref="SQLException"> if a database access error occurs </exception>
        Sub setShort(ByVal parameterIndex As Integer, ByVal x As Short)

        ''' <summary>
        ''' Sets the designated parameter to the given Java <code>short</code> value.
        ''' The driver converts this
        ''' to an SQL <code>SMALLINT</code> value when it sends it to the database.
        ''' </summary>
        ''' <param name="parameterName"> the name of the parameter </param>
        ''' <param name="x"> the parameter value </param>
        ''' <exception cref="SQLException"> if a database access error occurs or
        ''' this method is called on a closed <code>CallableStatement</code> </exception>
        ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
        ''' this method </exception>
        ''' <seealso cref= #getShort
        ''' @since 1.4 </seealso>
        Sub setShort(ByVal parameterName As String, ByVal x As Short)

        ''' <summary>
        ''' Sets the designated parameter in this <code>RowSet</code> object's command
        ''' to the given Java <code>int</code> value. The driver converts this to
        ''' an SQL <code>INTEGER</code> value before sending it to the database.
        ''' </summary>
        ''' <param name="parameterIndex"> the first parameter is 1, the second is 2, ... </param>
        ''' <param name="x"> the parameter value </param>
        ''' <exception cref="SQLException"> if a database access error occurs </exception>
        Sub setInt(ByVal parameterIndex As Integer, ByVal x As Integer)

        ''' <summary>
        ''' Sets the designated parameter to the given Java <code>int</code> value.
        ''' The driver converts this
        ''' to an SQL <code>INTEGER</code> value when it sends it to the database.
        ''' </summary>
        ''' <param name="parameterName"> the name of the parameter </param>
        ''' <param name="x"> the parameter value </param>
        ''' <exception cref="SQLException"> if a database access error occurs or
        ''' this method is called on a closed <code>CallableStatement</code> </exception>
        ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
        ''' this method </exception>
        ''' <seealso cref= #getInt
        ''' @since 1.4 </seealso>
        Sub setInt(ByVal parameterName As String, ByVal x As Integer)

        ''' <summary>
        ''' Sets the designated parameter in this <code>RowSet</code> object's command
        ''' to the given Java <code>long</code> value. The driver converts this to
        ''' an SQL <code>BIGINT</code> value before sending it to the database.
        ''' </summary>
        ''' <param name="parameterIndex"> the first parameter is 1, the second is 2, ... </param>
        ''' <param name="x"> the parameter value </param>
        ''' <exception cref="SQLException"> if a database access error occurs </exception>
        Sub setLong(ByVal parameterIndex As Integer, ByVal x As Long)

        ''' <summary>
        ''' Sets the designated parameter to the given Java <code>long</code> value.
        ''' The driver converts this
        ''' to an SQL <code>BIGINT</code> value when it sends it to the database.
        ''' </summary>
        ''' <param name="parameterName"> the name of the parameter </param>
        ''' <param name="x"> the parameter value </param>
        ''' <exception cref="SQLException"> if a database access error occurs or
        ''' this method is called on a closed <code>CallableStatement</code> </exception>
        ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
        ''' this method </exception>
        ''' <seealso cref= #getLong
        ''' @since 1.4 </seealso>
        Sub setLong(ByVal parameterName As String, ByVal x As Long)

        ''' <summary>
        ''' Sets the designated parameter in this <code>RowSet</code> object's command
        ''' to the given Java <code>float</code> value. The driver converts this to
        ''' an SQL <code>REAL</code> value before sending it to the database.
        ''' </summary>
        ''' <param name="parameterIndex"> the first parameter is 1, the second is 2, ... </param>
        ''' <param name="x"> the parameter value </param>
        ''' <exception cref="SQLException"> if a database access error occurs </exception>
        Sub setFloat(ByVal parameterIndex As Integer, ByVal x As Single)

        ''' <summary>
        ''' Sets the designated parameter to the given Java <code>float</code> value.
        ''' The driver converts this
        ''' to an SQL <code>FLOAT</code> value when it sends it to the database.
        ''' </summary>
        ''' <param name="parameterName"> the name of the parameter </param>
        ''' <param name="x"> the parameter value </param>
        ''' <exception cref="SQLException"> if a database access error occurs or
        ''' this method is called on a closed <code>CallableStatement</code> </exception>
        ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
        ''' this method </exception>
        ''' <seealso cref= #getFloat
        ''' @since 1.4 </seealso>
        Sub setFloat(ByVal parameterName As String, ByVal x As Single)

        ''' <summary>
        ''' Sets the designated parameter in this <code>RowSet</code> object's command
        ''' to the given Java <code>double</code> value. The driver converts this to
        ''' an SQL <code>DOUBLE</code> value before sending it to the database.
        ''' </summary>
        ''' <param name="parameterIndex"> the first parameter is 1, the second is 2, ... </param>
        ''' <param name="x"> the parameter value </param>
        ''' <exception cref="SQLException"> if a database access error occurs </exception>
        Sub setDouble(ByVal parameterIndex As Integer, ByVal x As Double)

        ''' <summary>
        ''' Sets the designated parameter to the given Java <code>double</code> value.
        ''' The driver converts this
        ''' to an SQL <code>DOUBLE</code> value when it sends it to the database.
        ''' </summary>
        ''' <param name="parameterName"> the name of the parameter </param>
        ''' <param name="x"> the parameter value </param>
        ''' <exception cref="SQLException"> if a database access error occurs or
        ''' this method is called on a closed <code>CallableStatement</code> </exception>
        ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
        ''' this method </exception>
        ''' <seealso cref= #getDouble
        ''' @since 1.4 </seealso>
        Sub setDouble(ByVal parameterName As String, ByVal x As Double)

        ''' <summary>
        ''' Sets the designated parameter in this <code>RowSet</code> object's command
        ''' to the given <code>java.math.BigDeciaml</code> value.
        ''' The driver converts this to
        ''' an SQL <code>NUMERIC</code> value before sending it to the database.
        ''' </summary>
        ''' <param name="parameterIndex"> the first parameter is 1, the second is 2, ... </param>
        ''' <param name="x"> the parameter value </param>
        ''' <exception cref="SQLException"> if a database access error occurs </exception>
        Sub setBigDecimal(ByVal parameterIndex As Integer, ByVal x As Decimal)

        ''' <summary>
        ''' Sets the designated parameter to the given
        ''' <code>java.math.BigDecimal</code> value.
        ''' The driver converts this to an SQL <code>NUMERIC</code> value when
        ''' it sends it to the database.
        ''' </summary>
        ''' <param name="parameterName"> the name of the parameter </param>
        ''' <param name="x"> the parameter value </param>
        ''' <exception cref="SQLException"> if a database access error occurs or
        ''' this method is called on a closed <code>CallableStatement</code> </exception>
        ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
        ''' this method </exception>
        ''' <seealso cref= #getBigDecimal
        ''' @since 1.4 </seealso>
        Sub setBigDecimal(ByVal parameterName As String, ByVal x As Decimal)

        ''' <summary>
        ''' Sets the designated parameter in this <code>RowSet</code> object's command
        ''' to the given Java <code>String</code> value. Before sending it to the
        ''' database, the driver converts this to an SQL <code>VARCHAR</code> or
        ''' <code>LONGVARCHAR</code> value, depending on the argument's size relative
        ''' to the driver's limits on <code>VARCHAR</code> values.
        ''' </summary>
        ''' <param name="parameterIndex"> the first parameter is 1, the second is 2, ... </param>
        ''' <param name="x"> the parameter value </param>
        ''' <exception cref="SQLException"> if a database access error occurs </exception>
        Sub setString(ByVal parameterIndex As Integer, ByVal x As String)

        ''' <summary>
        ''' Sets the designated parameter to the given Java <code>String</code> value.
        ''' The driver converts this
        ''' to an SQL <code>VARCHAR</code> or <code>LONGVARCHAR</code> value
        ''' (depending on the argument's
        ''' size relative to the driver's limits on <code>VARCHAR</code> values)
        ''' when it sends it to the database.
        ''' </summary>
        ''' <param name="parameterName"> the name of the parameter </param>
        ''' <param name="x"> the parameter value </param>
        ''' <exception cref="SQLException"> if a database access error occurs or
        ''' this method is called on a closed <code>CallableStatement</code> </exception>
        ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
        ''' this method </exception>
        ''' <seealso cref= #getString
        ''' @since 1.4 </seealso>
        Sub setString(ByVal parameterName As String, ByVal x As String)

        ''' <summary>
        ''' Sets the designated parameter in this <code>RowSet</code> object's command
        ''' to the given Java array of <code>byte</code> values. Before sending it to the
        ''' database, the driver converts this to an SQL <code>VARBINARY</code> or
        ''' <code>LONGVARBINARY</code> value, depending on the argument's size relative
        ''' to the driver's limits on <code>VARBINARY</code> values.
        ''' </summary>
        ''' <param name="parameterIndex"> the first parameter is 1, the second is 2, ... </param>
        ''' <param name="x"> the parameter value </param>
        ''' <exception cref="SQLException"> if a database access error occurs </exception>
        Sub setBytes(ByVal parameterIndex As Integer, ByVal x As SByte())

        ''' <summary>
        ''' Sets the designated parameter to the given Java array of bytes.
        ''' The driver converts this to an SQL <code>VARBINARY</code> or
        ''' <code>LONGVARBINARY</code> (depending on the argument's size relative
        ''' to the driver's limits on <code>VARBINARY</code> values) when it sends
        ''' it to the database.
        ''' </summary>
        ''' <param name="parameterName"> the name of the parameter </param>
        ''' <param name="x"> the parameter value </param>
        ''' <exception cref="SQLException"> if a database access error occurs or
        ''' this method is called on a closed <code>CallableStatement</code> </exception>
        ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
        ''' this method </exception>
        ''' <seealso cref= #getBytes
        ''' @since 1.4 </seealso>
        Sub setBytes(ByVal parameterName As String, ByVal x As SByte())

        ''' <summary>
        ''' Sets the designated parameter in this <code>RowSet</code> object's command
        ''' to the given <code>java.sql.Date</code> value. The driver converts this to
        ''' an SQL <code>DATE</code> value before sending it to the database, using the
        ''' default <code>java.util.Calendar</code> to calculate the date.
        ''' </summary>
        ''' <param name="parameterIndex"> the first parameter is 1, the second is 2, ... </param>
        ''' <param name="x"> the parameter value </param>
        ''' <exception cref="SQLException"> if a database access error occurs </exception>
        Sub setDate(ByVal parameterIndex As Integer, ByVal x As java.sql.Date)

        ''' <summary>
        ''' Sets the designated parameter in this <code>RowSet</code> object's command
        ''' to the given <code>java.sql.Time</code> value. The driver converts this to
        ''' an SQL <code>TIME</code> value before sending it to the database, using the
        ''' default <code>java.util.Calendar</code> to calculate it.
        ''' </summary>
        ''' <param name="parameterIndex"> the first parameter is 1, the second is 2, ... </param>
        ''' <param name="x"> the parameter value </param>
        ''' <exception cref="SQLException"> if a database access error occurs </exception>
        Sub setTime(ByVal parameterIndex As Integer, ByVal x As java.sql.Time)

        ''' <summary>
        ''' Sets the designated parameter in this <code>RowSet</code> object's command
        ''' to the given <code>java.sql.Timestamp</code> value. The driver converts this to
        ''' an SQL <code>TIMESTAMP</code> value before sending it to the database, using the
        ''' default <code>java.util.Calendar</code> to calculate it.
        ''' </summary>
        ''' <param name="parameterIndex"> the first parameter is 1, the second is 2, ... </param>
        ''' <param name="x"> the parameter value </param>
        ''' <exception cref="SQLException"> if a database access error occurs </exception>
        Sub setTimestamp(ByVal parameterIndex As Integer, ByVal x As java.sql.Timestamp)

        ''' <summary>
        ''' Sets the designated parameter to the given <code>java.sql.Timestamp</code> value.
        ''' The driver
        ''' converts this to an SQL <code>TIMESTAMP</code> value when it sends it to the
        ''' database.
        ''' </summary>
        ''' <param name="parameterName"> the name of the parameter </param>
        ''' <param name="x"> the parameter value </param>
        ''' <exception cref="SQLException"> if a database access error occurs or
        ''' this method is called on a closed <code>CallableStatement</code> </exception>
        ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
        ''' this method </exception>
        ''' <seealso cref= #getTimestamp
        ''' @since 1.4 </seealso>
        Sub setTimestamp(ByVal parameterName As String, ByVal x As java.sql.Timestamp)

        ''' <summary>
        ''' Sets the designated parameter in this <code>RowSet</code> object's command
        ''' to the given <code>java.io.InputStream</code> value.
        ''' It may be more practical to send a very large ASCII value via a
        ''' <code>java.io.InputStream</code> rather than as a <code>LONGVARCHAR</code>
        ''' parameter. The driver will read the data from the stream
        ''' as needed until it reaches end-of-file.
        ''' 
        ''' <P><B>Note:</B> This stream object can either be a standard
        ''' Java stream object or your own subclass that implements the
        ''' standard interface.
        ''' </summary>
        ''' <param name="parameterIndex"> the first parameter is 1, the second is 2, ... </param>
        ''' <param name="x"> the Java input stream that contains the ASCII parameter value </param>
        ''' <param name="length"> the number of bytes in the stream </param>
        ''' <exception cref="SQLException"> if a database access error occurs </exception>
        Sub setAsciiStream(ByVal parameterIndex As Integer, ByVal x As java.io.InputStream, ByVal length As Integer)

        ''' <summary>
        ''' Sets the designated parameter to the given input stream, which will have
        ''' the specified number of bytes.
        ''' When a very large ASCII value is input to a <code>LONGVARCHAR</code>
        ''' parameter, it may be more practical to send it via a
        ''' <code>java.io.InputStream</code>. Data will be read from the stream
        ''' as needed until end-of-file is reached.  The JDBC driver will
        ''' do any necessary conversion from ASCII to the database char format.
        '''   
        ''' <P><B>Note:</B> This stream object can either be a standard
        ''' Java stream object or your own subclass that implements the
        ''' standard interface.
        ''' </summary>
        ''' <param name="parameterName"> the name of the parameter </param>
        ''' <param name="x"> the Java input stream that contains the ASCII parameter value </param>
        ''' <param name="length"> the number of bytes in the stream </param>
        ''' <exception cref="SQLException"> if a database access error occurs or
        ''' this method is called on a closed <code>CallableStatement</code> </exception>
        ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
        ''' this method
        ''' @since 1.4 </exception>
        Sub setAsciiStream(ByVal parameterName As String, ByVal x As java.io.InputStream, ByVal length As Integer)

        ''' <summary>
        ''' Sets the designated parameter in this <code>RowSet</code> object's command
        ''' to the given <code>java.io.InputStream</code> value.
        ''' It may be more practical to send a very large binary value via a
        ''' <code>java.io.InputStream</code> rather than as a <code>LONGVARBINARY</code>
        ''' parameter. The driver will read the data from the stream
        ''' as needed until it reaches end-of-file.
        ''' 
        ''' <P><B>Note:</B> This stream object can either be a standard
        ''' Java stream object or your own subclass that implements the
        ''' standard interface.
        ''' </summary>
        ''' <param name="parameterIndex"> the first parameter is 1, the second is 2, ... </param>
        ''' <param name="x"> the java input stream which contains the binary parameter value </param>
        ''' <param name="length"> the number of bytes in the stream </param>
        ''' <exception cref="SQLException"> if a database access error occurs </exception>
        Sub setBinaryStream(ByVal parameterIndex As Integer, ByVal x As java.io.InputStream, ByVal length As Integer)

        ''' <summary>
        ''' Sets the designated parameter to the given input stream, which will have
        ''' the specified number of bytes.
        ''' When a very large binary value is input to a <code>LONGVARBINARY</code>
        ''' parameter, it may be more practical to send it via a
        ''' <code>java.io.InputStream</code> object. The data will be read from the stream
        ''' as needed until end-of-file is reached.
        '''   
        ''' <P><B>Note:</B> This stream object can either be a standard
        ''' Java stream object or your own subclass that implements the
        ''' standard interface.
        ''' </summary>
        ''' <param name="parameterName"> the name of the parameter </param>
        ''' <param name="x"> the java input stream which contains the binary parameter value </param>
        ''' <param name="length"> the number of bytes in the stream </param>
        ''' <exception cref="SQLException"> if a database access error occurs or
        ''' this method is called on a closed <code>CallableStatement</code> </exception>
        ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
        ''' this method
        ''' @since 1.4 </exception>
        Sub setBinaryStream(ByVal parameterName As String, ByVal x As java.io.InputStream, ByVal length As Integer)

        ''' <summary>
        ''' Sets the designated parameter in this <code>RowSet</code> object's command
        ''' to the given <code>java.io.Reader</code> value.
        ''' It may be more practical to send a very large UNICODE value via a
        ''' <code>java.io.Reader</code> rather than as a <code>LONGVARCHAR</code>
        ''' parameter. The driver will read the data from the stream
        ''' as needed until it reaches end-of-file.
        ''' 
        ''' <P><B>Note:</B> This stream object can either be a standard
        ''' Java stream object or your own subclass that implements the
        ''' standard interface.
        ''' </summary>
        ''' <param name="parameterIndex"> the first parameter is 1, the second is 2, ... </param>
        ''' <param name="reader"> the <code>Reader</code> object that contains the UNICODE data
        '''        to be set </param>
        ''' <param name="length"> the number of characters in the stream </param>
        ''' <exception cref="SQLException"> if a database access error occurs </exception>
        Sub setCharacterStream(ByVal parameterIndex As Integer, ByVal reader As Reader, ByVal length As Integer)

        ''' <summary>
        ''' Sets the designated parameter to the given <code>Reader</code>
        ''' object, which is the given number of characters long.
        ''' When a very large UNICODE value is input to a <code>LONGVARCHAR</code>
        ''' parameter, it may be more practical to send it via a
        ''' <code>java.io.Reader</code> object. The data will be read from the stream
        ''' as needed until end-of-file is reached.  The JDBC driver will
        ''' do any necessary conversion from UNICODE to the database char format.
        '''   
        ''' <P><B>Note:</B> This stream object can either be a standard
        ''' Java stream object or your own subclass that implements the
        ''' standard interface.
        ''' </summary>
        ''' <param name="parameterName"> the name of the parameter </param>
        ''' <param name="reader"> the <code>java.io.Reader</code> object that
        '''        contains the UNICODE data used as the designated parameter </param>
        ''' <param name="length"> the number of characters in the stream </param>
        ''' <exception cref="SQLException"> if a database access error occurs or
        ''' this method is called on a closed <code>CallableStatement</code> </exception>
        ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
        ''' this method
        ''' @since 1.4 </exception>
        Sub setCharacterStream(ByVal parameterName As String, ByVal reader As java.io.Reader, ByVal length As Integer)

        ''' <summary>
        ''' Sets the designated parameter in this <code>RowSet</code> object's command
        ''' to the given input stream.
        ''' When a very large ASCII value is input to a <code>LONGVARCHAR</code>
        ''' parameter, it may be more practical to send it via a
        ''' <code>java.io.InputStream</code>. Data will be read from the stream
        ''' as needed until end-of-file is reached.  The JDBC driver will
        ''' do any necessary conversion from ASCII to the database char format.
        ''' 
        ''' <P><B>Note:</B> This stream object can either be a standard
        ''' Java stream object or your own subclass that implements the
        ''' standard interface.
        ''' <P><B>Note:</B> Consult your JDBC driver documentation to determine if
        ''' it might be more efficient to use a version of
        ''' <code>setAsciiStream</code> which takes a length parameter.
        ''' </summary>
        ''' <param name="parameterIndex"> the first parameter is 1, the second is 2, ... </param>
        ''' <param name="x"> the Java input stream that contains the ASCII parameter value </param>
        ''' <exception cref="SQLException"> if a database access error occurs or
        ''' this method is called on a closed <code>PreparedStatement</code> </exception>
        ''' <exception cref="SQLFeatureNotSupportedException">  if the JDBC driver does not support this method
        ''' @since 1.6 </exception>
        Sub setAsciiStream(ByVal parameterIndex As Integer, ByVal x As java.io.InputStream)

        ''' <summary>
        ''' Sets the designated parameter to the given input stream.
        ''' When a very large ASCII value is input to a <code>LONGVARCHAR</code>
        ''' parameter, it may be more practical to send it via a
        ''' <code>java.io.InputStream</code>. Data will be read from the stream
        ''' as needed until end-of-file is reached.  The JDBC driver will
        ''' do any necessary conversion from ASCII to the database char format.
        '''  
        ''' <P><B>Note:</B> This stream object can either be a standard
        ''' Java stream object or your own subclass that implements the
        ''' standard interface.
        ''' <P><B>Note:</B> Consult your JDBC driver documentation to determine if
        ''' it might be more efficient to use a version of
        ''' <code>setAsciiStream</code> which takes a length parameter.
        ''' </summary>
        ''' <param name="parameterName"> the name of the parameter </param>
        ''' <param name="x"> the Java input stream that contains the ASCII parameter value </param>
        ''' <exception cref="SQLException"> if a database access error occurs or
        ''' this method is called on a closed <code>CallableStatement</code> </exception>
        ''' <exception cref="SQLFeatureNotSupportedException">  if the JDBC driver does not support this method
        ''' @since 1.6 </exception>
        Sub setAsciiStream(ByVal parameterName As String, ByVal x As java.io.InputStream)

        ''' <summary>
        ''' Sets the designated parameter in this <code>RowSet</code> object's command
        ''' to the given input stream.
        ''' When a very large binary value is input to a <code>LONGVARBINARY</code>
        ''' parameter, it may be more practical to send it via a
        ''' <code>java.io.InputStream</code> object. The data will be read from the
        ''' stream as needed until end-of-file is reached.
        ''' 
        ''' <P><B>Note:</B> This stream object can either be a standard
        ''' Java stream object or your own subclass that implements the
        ''' standard interface.
        ''' <P><B>Note:</B> Consult your JDBC driver documentation to determine if
        ''' it might be more efficient to use a version of
        ''' <code>setBinaryStream</code> which takes a length parameter.
        ''' </summary>
        ''' <param name="parameterIndex"> the first parameter is 1, the second is 2, ... </param>
        ''' <param name="x"> the java input stream which contains the binary parameter value </param>
        ''' <exception cref="SQLException"> if a database access error occurs or
        ''' this method is called on a closed <code>PreparedStatement</code> </exception>
        ''' <exception cref="SQLFeatureNotSupportedException">  if the JDBC driver does not support this method
        ''' @since 1.6 </exception>
        Sub setBinaryStream(ByVal parameterIndex As Integer, ByVal x As java.io.InputStream)

        ''' <summary>
        ''' Sets the designated parameter to the given input stream.
        ''' When a very large binary value is input to a <code>LONGVARBINARY</code>
        ''' parameter, it may be more practical to send it via a
        ''' <code>java.io.InputStream</code> object. The data will be read from the
        ''' stream as needed until end-of-file is reached.
        '''   
        ''' <P><B>Note:</B> This stream object can either be a standard
        ''' Java stream object or your own subclass that implements the
        ''' standard interface.
        ''' <P><B>Note:</B> Consult your JDBC driver documentation to determine if
        ''' it might be more efficient to use a version of
        ''' <code>setBinaryStream</code> which takes a length parameter.
        ''' </summary>
        ''' <param name="parameterName"> the name of the parameter </param>
        ''' <param name="x"> the java input stream which contains the binary parameter value </param>
        ''' <exception cref="SQLException"> if a database access error occurs or
        ''' this method is called on a closed <code>CallableStatement</code> </exception>
        ''' <exception cref="SQLFeatureNotSupportedException">  if the JDBC driver does not support this method
        ''' @since 1.6 </exception>
        Sub setBinaryStream(ByVal parameterName As String, ByVal x As java.io.InputStream)

        ''' <summary>
        ''' Sets the designated parameter in this <code>RowSet</code> object's command
        ''' to the given <code>Reader</code>
        ''' object.
        ''' When a very large UNICODE value is input to a <code>LONGVARCHAR</code>
        ''' parameter, it may be more practical to send it via a
        ''' <code>java.io.Reader</code> object. The data will be read from the stream
        ''' as needed until end-of-file is reached.  The JDBC driver will
        ''' do any necessary conversion from UNICODE to the database char format.
        ''' 
        ''' <P><B>Note:</B> This stream object can either be a standard
        ''' Java stream object or your own subclass that implements the
        ''' standard interface.
        ''' <P><B>Note:</B> Consult your JDBC driver documentation to determine if
        ''' it might be more efficient to use a version of
        ''' <code>setCharacterStream</code> which takes a length parameter.
        ''' </summary>
        ''' <param name="parameterIndex"> the first parameter is 1, the second is 2, ... </param>
        ''' <param name="reader"> the <code>java.io.Reader</code> object that contains the
        '''        Unicode data </param>
        ''' <exception cref="SQLException"> if a database access error occurs or
        ''' this method is called on a closed <code>PreparedStatement</code> </exception>
        ''' <exception cref="SQLFeatureNotSupportedException">  if the JDBC driver does not support this method
        ''' @since 1.6 </exception>
        Sub setCharacterStream(ByVal parameterIndex As Integer, ByVal reader As java.io.Reader)

        ''' <summary>
        ''' Sets the designated parameter to the given <code>Reader</code>
        ''' object.
        ''' When a very large UNICODE value is input to a <code>LONGVARCHAR</code>
        ''' parameter, it may be more practical to send it via a
        ''' <code>java.io.Reader</code> object. The data will be read from the stream
        ''' as needed until end-of-file is reached.  The JDBC driver will
        ''' do any necessary conversion from UNICODE to the database char format.
        '''   
        ''' <P><B>Note:</B> This stream object can either be a standard
        ''' Java stream object or your own subclass that implements the
        ''' standard interface.
        ''' <P><B>Note:</B> Consult your JDBC driver documentation to determine if
        ''' it might be more efficient to use a version of
        ''' <code>setCharacterStream</code> which takes a length parameter.
        ''' </summary>
        ''' <param name="parameterName"> the name of the parameter </param>
        ''' <param name="reader"> the <code>java.io.Reader</code> object that contains the
        '''        Unicode data </param>
        ''' <exception cref="SQLException"> if a database access error occurs or
        ''' this method is called on a closed <code>CallableStatement</code> </exception>
        ''' <exception cref="SQLFeatureNotSupportedException">  if the JDBC driver does not support this method
        ''' @since 1.6 </exception>
        Sub setCharacterStream(ByVal parameterName As String, ByVal reader As java.io.Reader)

        ''' <summary>
        ''' Sets the designated parameter in this <code>RowSet</code> object's command
        ''' to a <code>Reader</code> object. The
        ''' <code>Reader</code> reads the data till end-of-file is reached. The
        ''' driver does the necessary conversion from Java character format to
        ''' the national character set in the database.
        ''' 
        ''' <P><B>Note:</B> This stream object can either be a standard
        ''' Java stream object or your own subclass that implements the
        ''' standard interface.
        ''' <P><B>Note:</B> Consult your JDBC driver documentation to determine if
        ''' it might be more efficient to use a version of
        ''' <code>setNCharacterStream</code> which takes a length parameter.
        ''' </summary>
        ''' <param name="parameterIndex"> of the first parameter is 1, the second is 2, ... </param>
        ''' <param name="value"> the parameter value </param>
        ''' <exception cref="SQLException"> if the driver does not support national
        '''         character sets;  if the driver can detect that a data conversion
        '''  error could occur ; if a database access error occurs; or
        ''' this method is called on a closed <code>PreparedStatement</code> </exception>
        ''' <exception cref="SQLFeatureNotSupportedException">  if the JDBC driver does not support this method
        ''' @since 1.6 </exception>
        Sub setNCharacterStream(ByVal parameterIndex As Integer, ByVal value As Reader)



        ''' <summary>
        ''' Sets the designated parameter in this <code>RowSet</code> object's command
        ''' with the given Java <code>Object</code>.  For integral values, the
        ''' <code>java.lang</code> equivalent objects should be used (for example,
        ''' an instance of the class <code>Integer</code> for an <code>int</code>).
        ''' 
        ''' If the second argument is an <code>InputStream</code> then the stream must contain
        ''' the number of bytes specified by scaleOrLength.  If the second argument is a
        ''' <code>Reader</code> then the reader must contain the number of characters specified    * by scaleOrLength. If these conditions are not true the driver will generate a
        ''' <code>SQLException</code> when the prepared statement is executed.
        ''' 
        ''' <p>The given Java object will be converted to the targetSqlType
        ''' before being sent to the database.
        ''' <P>
        ''' If the object is of a class implementing <code>SQLData</code>,
        ''' the rowset should call the method <code>SQLData.writeSQL</code>
        ''' to write the object to an <code>SQLOutput</code> data stream.
        ''' If, on the other hand, the object is of a class implementing
        ''' <code>Ref</code>, <code>Blob</code>, <code>Clob</code>,  <code>NClob</code>,
        '''  <code>Struct</code>, <code>java.net.URL</code>,
        ''' or <code>Array</code>, the driver should pass it to the database as a
        ''' value of the corresponding SQL type.
        ''' 
        ''' 
        ''' <p>Note that this method may be used to pass datatabase-specific
        ''' abstract data types.
        ''' </summary>
        ''' <param name="parameterIndex"> the first parameter is 1, the second is 2, ... </param>
        ''' <param name="x"> the object containing the input parameter value </param>
        ''' <param name="targetSqlType"> the SQL type (as defined in <code>java.sql.Types</code>)
        '''        to be sent to the database. The scale argument may further qualify this
        '''        type. </param>
        ''' <param name="scaleOrLength"> for <code>java.sql.Types.DECIMAL</code>
        '''          or <code>java.sql.Types.NUMERIC types</code>,
        '''          this is the number of digits after the decimal point. For
        '''          Java Object types <code>InputStream</code> and <code>Reader</code>,
        '''          this is the length
        '''          of the data in the stream or reader.  For all other types,
        '''          this value will be ignored. </param>
        ''' <exception cref="SQLException"> if a database access error occurs </exception>
        ''' <seealso cref= java.sql.Types </seealso>
        Sub setObject(ByVal parameterIndex As Integer, ByVal x As Object, ByVal targetSqlType As Integer, ByVal scaleOrLength As Integer)

        ''' <summary>
        ''' Sets the value of the designated parameter with the given object. The second
        ''' argument must be an object type; for integral values, the
        ''' <code>java.lang</code> equivalent objects should be used.
        '''   
        ''' <p>The given Java object will be converted to the given targetSqlType
        ''' before being sent to the database.
        '''   
        ''' If the object has a custom mapping (is of a class implementing the
        ''' interface <code>SQLData</code>),
        ''' the JDBC driver should call the method <code>SQLData.writeSQL</code> to write it
        ''' to the SQL data stream.
        ''' If, on the other hand, the object is of a class implementing
        ''' <code>Ref</code>, <code>Blob</code>, <code>Clob</code>,  <code>NClob</code>,
        '''  <code>Struct</code>, <code>java.net.URL</code>,
        ''' or <code>Array</code>, the driver should pass it to the database as a
        ''' value of the corresponding SQL type.
        ''' <P>
        ''' Note that this method may be used to pass datatabase-
        ''' specific abstract data types.
        ''' </summary>
        ''' <param name="parameterName"> the name of the parameter </param>
        ''' <param name="x"> the object containing the input parameter value </param>
        ''' <param name="targetSqlType"> the SQL type (as defined in java.sql.Types) to be
        ''' sent to the database. The scale argument may further qualify this type. </param>
        ''' <param name="scale"> for java.sql.Types.DECIMAL or java.sql.Types.NUMERIC types,
        '''          this is the number of digits after the decimal point.  For all other
        '''          types, this value will be ignored. </param>
        ''' <exception cref="SQLException"> if a database access error occurs or
        ''' this method is called on a closed <code>CallableStatement</code> </exception>
        ''' <exception cref="SQLFeatureNotSupportedException"> if <code>targetSqlType</code> is
        ''' a <code>ARRAY</code>, <code>BLOB</code>, <code>CLOB</code>,
        ''' <code>DATALINK</code>, <code>JAVA_OBJECT</code>, <code>NCHAR</code>,
        ''' <code>NCLOB</code>, <code>NVARCHAR</code>, <code>LONGNVARCHAR</code>,
        '''  <code>REF</code>, <code>ROWID</code>, <code>SQLXML</code>
        ''' or  <code>STRUCT</code> data type and the JDBC driver does not support
        ''' this data type </exception>
        ''' <seealso cref= Types </seealso>
        ''' <seealso cref= #getObject
        ''' @since 1.4 </seealso>
        Sub setObject(ByVal parameterName As String, ByVal x As Object, ByVal targetSqlType As Integer, ByVal scale As Integer)

        ''' <summary>
        ''' Sets the designated parameter in this <code>RowSet</code> object's command
        ''' with a Java <code>Object</code>.  For integral values, the
        ''' <code>java.lang</code> equivalent objects should be used.
        ''' This method is like <code>setObject</code> above, but the scale used is the scale
        ''' of the second parameter.  Scalar values have a scale of zero.  Literal
        ''' values have the scale present in the literal.
        ''' <P>
        ''' Even though it is supported, it is not recommended that this method
        ''' be called with floating point input values.
        ''' </summary>
        ''' <param name="parameterIndex"> the first parameter is 1, the second is 2, ... </param>
        ''' <param name="x"> the object containing the input parameter value </param>
        ''' <param name="targetSqlType"> the SQL type (as defined in <code>java.sql.Types</code>)
        '''        to be sent to the database </param>
        ''' <exception cref="SQLException"> if a database access error occurs </exception>
        Sub setObject(ByVal parameterIndex As Integer, ByVal x As Object, ByVal targetSqlType As Integer)

        ''' <summary>
        ''' Sets the value of the designated parameter with the given object.
        ''' This method is like the method <code>setObject</code>
        ''' above, except that it assumes a scale of zero.
        ''' </summary>
        ''' <param name="parameterName"> the name of the parameter </param>
        ''' <param name="x"> the object containing the input parameter value </param>
        ''' <param name="targetSqlType"> the SQL type (as defined in java.sql.Types) to be
        '''                      sent to the database </param>
        ''' <exception cref="SQLException"> if a database access error occurs or
        ''' this method is called on a closed <code>CallableStatement</code> </exception>
        ''' <exception cref="SQLFeatureNotSupportedException"> if <code>targetSqlType</code> is
        ''' a <code>ARRAY</code>, <code>BLOB</code>, <code>CLOB</code>,
        ''' <code>DATALINK</code>, <code>JAVA_OBJECT</code>, <code>NCHAR</code>,
        ''' <code>NCLOB</code>, <code>NVARCHAR</code>, <code>LONGNVARCHAR</code>,
        '''  <code>REF</code>, <code>ROWID</code>, <code>SQLXML</code>
        ''' or  <code>STRUCT</code> data type and the JDBC driver does not support
        ''' this data type </exception>
        ''' <seealso cref= #getObject
        ''' @since 1.4 </seealso>
        Sub setObject(ByVal parameterName As String, ByVal x As Object, ByVal targetSqlType As Integer)

        ''' <summary>
        ''' Sets the value of the designated parameter with the given object.
        ''' The second parameter must be of type <code>Object</code>; therefore, the
        ''' <code>java.lang</code> equivalent objects should be used for built-in types.
        '''  
        ''' <p>The JDBC specification specifies a standard mapping from
        ''' Java <code>Object</code> types to SQL types.  The given argument
        ''' will be converted to the corresponding SQL type before being
        ''' sent to the database.
        '''  
        ''' <p>Note that this method may be used to pass datatabase-
        ''' specific abstract data types, by using a driver-specific Java
        ''' type.
        '''  
        ''' If the object is of a class implementing the interface <code>SQLData</code>,
        ''' the JDBC driver should call the method <code>SQLData.writeSQL</code>
        ''' to write it to the SQL data stream.
        ''' If, on the other hand, the object is of a class implementing
        ''' <code>Ref</code>, <code>Blob</code>, <code>Clob</code>,  <code>NClob</code>,
        '''  <code>Struct</code>, <code>java.net.URL</code>,
        ''' or <code>Array</code>, the driver should pass it to the database as a
        ''' value of the corresponding SQL type.
        ''' <P>
        ''' This method throws an exception if there is an ambiguity, for example, if the
        ''' object is of a class implementing more than one of the interfaces named above.
        ''' </summary>
        ''' <param name="parameterName"> the name of the parameter </param>
        ''' <param name="x"> the object containing the input parameter value </param>
        ''' <exception cref="SQLException"> if a database access error occurs,
        ''' this method is called on a closed <code>CallableStatement</code> or if the given
        '''            <code>Object</code> parameter is ambiguous </exception>
        ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
        ''' this method </exception>
        ''' <seealso cref= #getObject
        ''' @since 1.4 </seealso>
        Sub setObject(ByVal parameterName As String, ByVal x As Object)

        ''' <summary>
        ''' Sets the designated parameter in this <code>RowSet</code> object's command
        ''' with a Java <code>Object</code>.  For integral values, the
        ''' <code>java.lang</code> equivalent objects should be used.
        ''' 
        ''' <p>The JDBC specification provides a standard mapping from
        ''' Java Object types to SQL types.  The driver will convert the
        ''' given Java object to its standard SQL mapping before sending it
        ''' to the database.
        ''' 
        ''' <p>Note that this method may be used to pass datatabase-specific
        ''' abstract data types by using a driver-specific Java type.
        ''' 
        ''' If the object is of a class implementing <code>SQLData</code>,
        ''' the rowset should call the method <code>SQLData.writeSQL</code>
        ''' to write the object to an <code>SQLOutput</code> data stream.
        ''' If, on the other hand, the object is of a class implementing
        ''' <code>Ref</code>, <code>Blob</code>, <code>Clob</code>,  <code>NClob</code>,
        '''  <code>Struct</code>, <code>java.net.URL</code>,
        ''' or <code>Array</code>, the driver should pass it to the database as a
        ''' value of the corresponding SQL type.
        ''' 
        ''' <P>
        ''' An exception is thrown if there is an ambiguity, for example, if the
        ''' object is of a class implementing more than one of these interfaces.
        ''' </summary>
        ''' <param name="parameterIndex"> The first parameter is 1, the second is 2, ... </param>
        ''' <param name="x"> The object containing the input parameter value </param>
        ''' <exception cref="SQLException"> if a database access error occurs </exception>
        Sub setObject(ByVal parameterIndex As Integer, ByVal x As Object)


        ''' <summary>
        ''' Sets the designated parameter in this <code>RowSet</code> object's command
        ''' with the given  <code>Ref</code> value.  The driver will convert this
        ''' to the appropriate <code>REF(&lt;structured-type&gt;)</code> value.
        ''' </summary>
        ''' <param name="i"> the first parameter is 1, the second is 2, ... </param>
        ''' <param name="x"> an object representing data of an SQL <code>REF</code> type </param>
        ''' <exception cref="SQLException"> if a database access error occurs </exception>
        Sub setRef(ByVal i As Integer, ByVal x As Ref)

        ''' <summary>
        ''' Sets the designated parameter in this <code>RowSet</code> object's command
        ''' with the given  <code>Blob</code> value.  The driver will convert this
        ''' to the <code>BLOB</code> value that the <code>Blob</code> object
        ''' represents before sending it to the database.
        ''' </summary>
        ''' <param name="i"> the first parameter is 1, the second is 2, ... </param>
        ''' <param name="x"> an object representing a BLOB </param>
        ''' <exception cref="SQLException"> if a database access error occurs </exception>
        Sub setBlob(ByVal i As Integer, ByVal x As Blob)

        ''' <summary>
        ''' Sets the designated parameter to a <code>InputStream</code> object.  The inputstream must contain  the number
        ''' of characters specified by length otherwise a <code>SQLException</code> will be
        ''' generated when the <code>PreparedStatement</code> is executed.
        ''' This method differs from the <code>setBinaryStream (int, InputStream, int)</code>
        ''' method because it informs the driver that the parameter value should be
        ''' sent to the server as a <code>BLOB</code>.  When the <code>setBinaryStream</code> method is used,
        ''' the driver may have to do extra work to determine whether the parameter
        ''' data should be sent to the server as a <code>LONGVARBINARY</code> or a <code>BLOB</code> </summary>
        ''' <param name="parameterIndex"> index of the first parameter is 1,
        ''' the second is 2, ... </param>
        ''' <param name="inputStream"> An object that contains the data to set the parameter
        ''' value to. </param>
        ''' <param name="length"> the number of bytes in the parameter data. </param>
        ''' <exception cref="SQLException"> if a database access error occurs,
        ''' this method is called on a closed <code>PreparedStatement</code>,
        ''' if parameterIndex does not correspond
        ''' to a parameter marker in the SQL statement,  if the length specified
        ''' is less than zero or if the number of bytes in the inputstream does not match
        ''' the specified length. </exception>
        ''' <exception cref="SQLFeatureNotSupportedException">  if the JDBC driver does not support this method
        '''   
        ''' @since 1.6 </exception>
        Sub setBlob(ByVal parameterIndex As Integer, ByVal inputStream As InputStream, ByVal length As Long)

        ''' <summary>
        ''' Sets the designated parameter to a <code>InputStream</code> object.
        ''' This method differs from the <code>setBinaryStream (int, InputStream)</code>
        ''' method because it informs the driver that the parameter value should be
        ''' sent to the server as a <code>BLOB</code>.  When the <code>setBinaryStream</code> method is used,
        ''' the driver may have to do extra work to determine whether the parameter
        ''' data should be sent to the server as a <code>LONGVARBINARY</code> or a <code>BLOB</code>
        '''   
        ''' <P><B>Note:</B> Consult your JDBC driver documentation to determine if
        ''' it might be more efficient to use a version of
        ''' <code>setBlob</code> which takes a length parameter.
        ''' </summary>
        ''' <param name="parameterIndex"> index of the first parameter is 1,
        ''' the second is 2, ... </param>
        ''' <param name="inputStream"> An object that contains the data to set the parameter
        ''' value to. </param>
        ''' <exception cref="SQLException"> if a database access error occurs,
        ''' this method is called on a closed <code>PreparedStatement</code> or
        ''' if parameterIndex does not correspond
        ''' to a parameter marker in the SQL statement, </exception>
        ''' <exception cref="SQLFeatureNotSupportedException">  if the JDBC driver does not support this method
        '''   
        ''' @since 1.6 </exception>
        Sub setBlob(ByVal parameterIndex As Integer, ByVal inputStream As InputStream)

        ''' <summary>
        ''' Sets the designated parameter to a <code>InputStream</code> object.  The <code>inputstream</code> must contain  the number
        ''' of characters specified by length, otherwise a <code>SQLException</code> will be
        ''' generated when the <code>CallableStatement</code> is executed.
        ''' This method differs from the <code>setBinaryStream (int, InputStream, int)</code>
        ''' method because it informs the driver that the parameter value should be
        ''' sent to the server as a <code>BLOB</code>.  When the <code>setBinaryStream</code> method is used,
        ''' the driver may have to do extra work to determine whether the parameter
        ''' data should be sent to the server as a <code>LONGVARBINARY</code> or a <code>BLOB</code>
        ''' </summary>
        ''' <param name="parameterName"> the name of the parameter to be set
        ''' the second is 2, ...
        ''' </param>
        ''' <param name="inputStream"> An object that contains the data to set the parameter
        ''' value to. </param>
        ''' <param name="length"> the number of bytes in the parameter data. </param>
        ''' <exception cref="SQLException">  if parameterIndex does not correspond
        ''' to a parameter marker in the SQL statement,  or if the length specified
        ''' is less than zero; if the number of bytes in the inputstream does not match
        ''' the specified length; if a database access error occurs or
        ''' this method is called on a closed <code>CallableStatement</code> </exception>
        ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
        ''' this method
        '''   
        ''' @since 1.6 </exception>
        Sub setBlob(ByVal parameterName As String, ByVal inputStream As InputStream, ByVal length As Long)

        ''' <summary>
        ''' Sets the designated parameter to the given <code>java.sql.Blob</code> object.
        ''' The driver converts this to an SQL <code>BLOB</code> value when it
        ''' sends it to the database.
        ''' </summary>
        ''' <param name="parameterName"> the name of the parameter </param>
        ''' <param name="x"> a <code>Blob</code> object that maps an SQL <code>BLOB</code> value </param>
        ''' <exception cref="SQLException"> if a database access error occurs or
        ''' this method is called on a closed <code>CallableStatement</code> </exception>
        ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
        ''' this method
        ''' @since 1.6 </exception>
        Sub setBlob(ByVal parameterName As String, ByVal x As Blob)

        ''' <summary>
        ''' Sets the designated parameter to a <code>InputStream</code> object.
        ''' This method differs from the <code>setBinaryStream (int, InputStream)</code>
        ''' method because it informs the driver that the parameter value should be
        ''' sent to the server as a <code>BLOB</code>.  When the <code>setBinaryStream</code> method is used,
        ''' the driver may have to do extra work to determine whether the parameter
        ''' data should be send to the server as a <code>LONGVARBINARY</code> or a <code>BLOB</code>
        '''   
        ''' <P><B>Note:</B> Consult your JDBC driver documentation to determine if
        ''' it might be more efficient to use a version of
        ''' <code>setBlob</code> which takes a length parameter.
        ''' </summary>
        ''' <param name="parameterName"> the name of the parameter </param>
        ''' <param name="inputStream"> An object that contains the data to set the parameter
        ''' value to. </param>
        ''' <exception cref="SQLException"> if a database access error occurs or
        ''' this method is called on a closed <code>CallableStatement</code> </exception>
        ''' <exception cref="SQLFeatureNotSupportedException">  if the JDBC driver does not support this method
        '''   
        ''' @since 1.6 </exception>
        Sub setBlob(ByVal parameterName As String, ByVal inputStream As InputStream)

        ''' <summary>
        ''' Sets the designated parameter in this <code>RowSet</code> object's command
        ''' with the given  <code>Clob</code> value.  The driver will convert this
        ''' to the <code>CLOB</code> value that the <code>Clob</code> object
        ''' represents before sending it to the database.
        ''' </summary>
        ''' <param name="i"> the first parameter is 1, the second is 2, ... </param>
        ''' <param name="x"> an object representing a CLOB </param>
        ''' <exception cref="SQLException"> if a database access error occurs </exception>
        Sub setClob(ByVal i As Integer, ByVal x As Clob)

        ''' <summary>
        ''' Sets the designated parameter to a <code>Reader</code> object.  The reader must contain  the number
        ''' of characters specified by length otherwise a <code>SQLException</code> will be
        ''' generated when the <code>PreparedStatement</code> is executed.
        ''' This method differs from the <code>setCharacterStream (int, Reader, int)</code> method
        ''' because it informs the driver that the parameter value should be sent to
        ''' the server as a <code>CLOB</code>.  When the <code>setCharacterStream</code> method is used, the
        ''' driver may have to do extra work to determine whether the parameter
        ''' data should be sent to the server as a <code>LONGVARCHAR</code> or a <code>CLOB</code> </summary>
        ''' <param name="parameterIndex"> index of the first parameter is 1, the second is 2, ... </param>
        ''' <param name="reader"> An object that contains the data to set the parameter value to. </param>
        ''' <param name="length"> the number of characters in the parameter data. </param>
        ''' <exception cref="SQLException"> if a database access error occurs, this method is called on
        ''' a closed <code>PreparedStatement</code>, if parameterIndex does not correspond to a parameter
        ''' marker in the SQL statement, or if the length specified is less than zero.
        ''' </exception>
        ''' <exception cref="SQLFeatureNotSupportedException">  if the JDBC driver does not support this method
        ''' @since 1.6 </exception>
        Sub setClob(ByVal parameterIndex As Integer, ByVal reader As Reader, ByVal length As Long)

        ''' <summary>
        ''' Sets the designated parameter to a <code>Reader</code> object.
        ''' This method differs from the <code>setCharacterStream (int, Reader)</code> method
        ''' because it informs the driver that the parameter value should be sent to
        ''' the server as a <code>CLOB</code>.  When the <code>setCharacterStream</code> method is used, the
        ''' driver may have to do extra work to determine whether the parameter
        ''' data should be sent to the server as a <code>LONGVARCHAR</code> or a <code>CLOB</code>
        '''   
        ''' <P><B>Note:</B> Consult your JDBC driver documentation to determine if
        ''' it might be more efficient to use a version of
        ''' <code>setClob</code> which takes a length parameter.
        ''' </summary>
        ''' <param name="parameterIndex"> index of the first parameter is 1, the second is 2, ... </param>
        ''' <param name="reader"> An object that contains the data to set the parameter value to. </param>
        ''' <exception cref="SQLException"> if a database access error occurs, this method is called on
        ''' a closed <code>PreparedStatement</code>or if parameterIndex does not correspond to a parameter
        ''' marker in the SQL statement
        ''' </exception>
        ''' <exception cref="SQLFeatureNotSupportedException">  if the JDBC driver does not support this method
        ''' @since 1.6 </exception>
        Sub setClob(ByVal parameterIndex As Integer, ByVal reader As Reader)

        ''' <summary>
        ''' Sets the designated parameter to a <code>Reader</code> object.  The <code>reader</code> must contain  the number
        ''' of characters specified by length otherwise a <code>SQLException</code> will be
        ''' generated when the <code>CallableStatement</code> is executed.
        ''' This method differs from the <code>setCharacterStream (int, Reader, int)</code> method
        ''' because it informs the driver that the parameter value should be sent to
        ''' the server as a <code>CLOB</code>.  When the <code>setCharacterStream</code> method is used, the
        ''' driver may have to do extra work to determine whether the parameter
        ''' data should be send to the server as a <code>LONGVARCHAR</code> or a <code>CLOB</code> </summary>
        ''' <param name="parameterName"> the name of the parameter to be set </param>
        ''' <param name="reader"> An object that contains the data to set the parameter value to. </param>
        ''' <param name="length"> the number of characters in the parameter data. </param>
        ''' <exception cref="SQLException"> if parameterIndex does not correspond to a parameter
        ''' marker in the SQL statement; if the length specified is less than zero;
        ''' a database access error occurs or
        ''' this method is called on a closed <code>CallableStatement</code> </exception>
        ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
        ''' this method
        '''   
        ''' @since 1.6 </exception>
        Sub setClob(ByVal parameterName As String, ByVal reader As Reader, ByVal length As Long)

        ''' <summary>
        ''' Sets the designated parameter to the given <code>java.sql.Clob</code> object.
        ''' The driver converts this to an SQL <code>CLOB</code> value when it
        ''' sends it to the database.
        ''' </summary>
        ''' <param name="parameterName"> the name of the parameter </param>
        ''' <param name="x"> a <code>Clob</code> object that maps an SQL <code>CLOB</code> value </param>
        ''' <exception cref="SQLException"> if a database access error occurs or
        ''' this method is called on a closed <code>CallableStatement</code> </exception>
        ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
        ''' this method
        ''' @since 1.6 </exception>
        Sub setClob(ByVal parameterName As String, ByVal x As Clob)

        ''' <summary>
        ''' Sets the designated parameter to a <code>Reader</code> object.
        ''' This method differs from the <code>setCharacterStream (int, Reader)</code> method
        ''' because it informs the driver that the parameter value should be sent to
        ''' the server as a <code>CLOB</code>.  When the <code>setCharacterStream</code> method is used, the
        ''' driver may have to do extra work to determine whether the parameter
        ''' data should be send to the server as a <code>LONGVARCHAR</code> or a <code>CLOB</code>
        '''   
        ''' <P><B>Note:</B> Consult your JDBC driver documentation to determine if
        ''' it might be more efficient to use a version of
        ''' <code>setClob</code> which takes a length parameter.
        ''' </summary>
        ''' <param name="parameterName"> the name of the parameter </param>
        ''' <param name="reader"> An object that contains the data to set the parameter value to. </param>
        ''' <exception cref="SQLException"> if a database access error occurs or this method is called on
        ''' a closed <code>CallableStatement</code>
        ''' </exception>
        ''' <exception cref="SQLFeatureNotSupportedException">  if the JDBC driver does not support this method
        ''' @since 1.6 </exception>
        Sub setClob(ByVal parameterName As String, ByVal reader As Reader)

        ''' <summary>
        ''' Sets the designated parameter in this <code>RowSet</code> object's command
        ''' with the given  <code>Array</code> value.  The driver will convert this
        ''' to the <code>ARRAY</code> value that the <code>Array</code> object
        ''' represents before sending it to the database.
        ''' </summary>
        ''' <param name="i"> the first parameter is 1, the second is 2, ... </param>
        ''' <param name="x"> an object representing an SQL array </param>
        ''' <exception cref="SQLException"> if a database access error occurs </exception>
        Sub setArray(ByVal i As Integer, ByVal x As java.sql.Array)

        ''' <summary>
        ''' Sets the designated parameter in this <code>RowSet</code> object's command
        ''' with the given  <code>java.sql.Date</code> value.  The driver will convert this
        ''' to an SQL <code>DATE</code> value, using the given <code>java.util.Calendar</code>
        ''' object to calculate the date.
        ''' </summary>
        ''' <param name="parameterIndex"> the first parameter is 1, the second is 2, ... </param>
        ''' <param name="x"> the parameter value </param>
        ''' <param name="cal"> the <code>java.util.Calendar</code> object to use for calculating the date </param>
        ''' <exception cref="SQLException"> if a database access error occurs </exception>
        Sub setDate(ByVal parameterIndex As Integer, ByVal x As java.sql.Date, ByVal cal As DateTime)

        ''' <summary>
        ''' Sets the designated parameter to the given <code>java.sql.Date</code> value
        ''' using the default time zone of the virtual machine that is running
        ''' the application.
        ''' The driver converts this
        ''' to an SQL <code>DATE</code> value when it sends it to the database.
        ''' </summary>
        ''' <param name="parameterName"> the name of the parameter </param>
        ''' <param name="x"> the parameter value </param>
        ''' <exception cref="SQLException"> if a database access error occurs or
        ''' this method is called on a closed <code>CallableStatement</code> </exception>
        ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
        ''' this method </exception>
        ''' <seealso cref= #getDate
        ''' @since 1.4 </seealso>
        Sub setDate(ByVal parameterName As String, ByVal x As java.sql.Date)

        ''' <summary>
        ''' Sets the designated parameter to the given <code>java.sql.Date</code> value,
        ''' using the given <code>Calendar</code> object.  The driver uses
        ''' the <code>Calendar</code> object to construct an SQL <code>DATE</code> value,
        ''' which the driver then sends to the database.  With a
        ''' a <code>Calendar</code> object, the driver can calculate the date
        ''' taking into account a custom timezone.  If no
        ''' <code>Calendar</code> object is specified, the driver uses the default
        ''' timezone, which is that of the virtual machine running the application.
        ''' </summary>
        ''' <param name="parameterName"> the name of the parameter </param>
        ''' <param name="x"> the parameter value </param>
        ''' <param name="cal"> the <code>Calendar</code> object the driver will use
        '''            to construct the date </param>
        ''' <exception cref="SQLException"> if a database access error occurs or
        ''' this method is called on a closed <code>CallableStatement</code> </exception>
        ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
        ''' this method </exception>
        ''' <seealso cref= #getDate
        ''' @since 1.4 </seealso>
        Sub setDate(ByVal parameterName As String, ByVal x As java.sql.Date, ByVal cal As DateTime)

        ''' <summary>
        ''' Sets the designated parameter in this <code>RowSet</code> object's command
        ''' with the given  <code>java.sql.Time</code> value.  The driver will convert this
        ''' to an SQL <code>TIME</code> value, using the given <code>java.util.Calendar</code>
        ''' object to calculate it, before sending it to the database.
        ''' </summary>
        ''' <param name="parameterIndex"> the first parameter is 1, the second is 2, ... </param>
        ''' <param name="x"> the parameter value </param>
        ''' <param name="cal"> the <code>java.util.Calendar</code> object to use for calculating the time </param>
        ''' <exception cref="SQLException"> if a database access error occurs </exception>
        Sub setTime(ByVal parameterIndex As Integer, ByVal x As java.sql.Time, ByVal cal As DateTime)

        ''' <summary>
        ''' Sets the designated parameter to the given <code>java.sql.Time</code> value.
        ''' The driver converts this
        ''' to an SQL <code>TIME</code> value when it sends it to the database.
        ''' </summary>
        ''' <param name="parameterName"> the name of the parameter </param>
        ''' <param name="x"> the parameter value </param>
        ''' <exception cref="SQLException"> if a database access error occurs or
        ''' this method is called on a closed <code>CallableStatement</code> </exception>
        ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
        ''' this method </exception>
        ''' <seealso cref= #getTime
        ''' @since 1.4 </seealso>
        Sub setTime(ByVal parameterName As String, ByVal x As java.sql.Time)

        ''' <summary>
        ''' Sets the designated parameter to the given <code>java.sql.Time</code> value,
        ''' using the given <code>Calendar</code> object.  The driver uses
        ''' the <code>Calendar</code> object to construct an SQL <code>TIME</code> value,
        ''' which the driver then sends to the database.  With a
        ''' a <code>Calendar</code> object, the driver can calculate the time
        ''' taking into account a custom timezone.  If no
        ''' <code>Calendar</code> object is specified, the driver uses the default
        ''' timezone, which is that of the virtual machine running the application.
        ''' </summary>
        ''' <param name="parameterName"> the name of the parameter </param>
        ''' <param name="x"> the parameter value </param>
        ''' <param name="cal"> the <code>Calendar</code> object the driver will use
        '''            to construct the time </param>
        ''' <exception cref="SQLException"> if a database access error occurs or
        ''' this method is called on a closed <code>CallableStatement</code> </exception>
        ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
        ''' this method </exception>
        ''' <seealso cref= #getTime
        ''' @since 1.4 </seealso>
        Sub setTime(ByVal parameterName As String, ByVal x As java.sql.Time, ByVal cal As DateTime)

        ''' <summary>
        ''' Sets the designated parameter in this <code>RowSet</code> object's command
        ''' with the given  <code>java.sql.Timestamp</code> value.  The driver will
        ''' convert this to an SQL <code>TIMESTAMP</code> value, using the given
        ''' <code>java.util.Calendar</code> object to calculate it, before sending it to the
        ''' database.
        ''' </summary>
        ''' <param name="parameterIndex"> the first parameter is 1, the second is 2, ... </param>
        ''' <param name="x"> the parameter value </param>
        ''' <param name="cal"> the <code>java.util.Calendar</code> object to use for calculating the
        '''        timestamp </param>
        ''' <exception cref="SQLException"> if a database access error occurs </exception>
        Sub setTimestamp(ByVal parameterIndex As Integer, ByVal x As java.sql.Timestamp, ByVal cal As DateTime)

        ''' <summary>
        ''' Sets the designated parameter to the given <code>java.sql.Timestamp</code> value,
        ''' using the given <code>Calendar</code> object.  The driver uses
        ''' the <code>Calendar</code> object to construct an SQL <code>TIMESTAMP</code> value,
        ''' which the driver then sends to the database.  With a
        ''' a <code>Calendar</code> object, the driver can calculate the timestamp
        ''' taking into account a custom timezone.  If no
        ''' <code>Calendar</code> object is specified, the driver uses the default
        ''' timezone, which is that of the virtual machine running the application.
        ''' </summary>
        ''' <param name="parameterName"> the name of the parameter </param>
        ''' <param name="x"> the parameter value </param>
        ''' <param name="cal"> the <code>Calendar</code> object the driver will use
        '''            to construct the timestamp </param>
        ''' <exception cref="SQLException"> if a database access error occurs or
        ''' this method is called on a closed <code>CallableStatement</code> </exception>
        ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
        ''' this method </exception>
        ''' <seealso cref= #getTimestamp
        ''' @since 1.4 </seealso>
        Sub setTimestamp(ByVal parameterName As String, ByVal x As java.sql.Timestamp, ByVal cal As DateTime)

        ''' <summary>
        ''' Clears the parameters set for this <code>RowSet</code> object's command.
        ''' <P>In general, parameter values remain in force for repeated use of a
        ''' <code>RowSet</code> object. Setting a parameter value automatically clears its
        ''' previous value.  However, in some cases it is useful to immediately
        ''' release the resources used by the current parameter values, which can
        ''' be done by calling the method <code>clearParameters</code>.
        ''' </summary>
        ''' <exception cref="SQLException"> if a database access error occurs </exception>
        Sub clearParameters()

        '---------------------------------------------------------------------
        ' Reading and writing data
        '---------------------------------------------------------------------

        ''' <summary>
        ''' Fills this <code>RowSet</code> object with data.
        ''' <P>
        ''' The <code>execute</code> method may use the following properties
        ''' to create a connection for reading data: url, data source name,
        ''' user name, password, transaction isolation, and type map.
        ''' 
        ''' The <code>execute</code> method  may use the following properties
        ''' to create a statement to execute a command:
        ''' command, read only, maximum field size,
        ''' maximum rows, escape processing, and query timeout.
        ''' <P>
        ''' If the required properties have not been set, an exception is
        ''' thrown.  If this method is successful, the current contents of the rowset are
        ''' discarded and the rowset's metadata is also (re)set.  If there are
        ''' outstanding updates, they are ignored.
        ''' <P>
        ''' If this <code>RowSet</code> object does not maintain a continuous connection
        ''' with its source of data, it may use a reader (a <code>RowSetReader</code>
        ''' object) to fill itself with data.  In this case, a reader will have been
        ''' registered with this <code>RowSet</code> object, and the method
        ''' <code>execute</code> will call on the reader's <code>readData</code>
        ''' method as part of its implementation.
        ''' </summary>
        ''' <exception cref="SQLException"> if a database access error occurs or any of the
        '''            properties necessary for making a connection and creating
        '''            a statement have not been set </exception>
        Sub execute()

        '--------------------------------------------------------------------
        ' Events
        '--------------------------------------------------------------------

        ''' <summary>
        ''' Registers the given listener so that it will be notified of events
        ''' that occur on this <code>RowSet</code> object.
        ''' </summary>
        ''' <param name="listener"> a component that has implemented the <code>RowSetListener</code>
        '''        interface and wants to be notified when events occur on this
        '''        <code>RowSet</code> object </param>
        ''' <seealso cref= #removeRowSetListener </seealso>
        Sub addRowSetListener(ByVal listener As RowSetListener)

        ''' <summary>
        ''' Removes the specified listener from the list of components that will be
        ''' notified when an event occurs on this <code>RowSet</code> object.
        ''' </summary>
        ''' <param name="listener"> a component that has been registered as a listener for this
        '''        <code>RowSet</code> object </param>
        ''' <seealso cref= #addRowSetListener </seealso>
        Sub removeRowSetListener(ByVal listener As RowSetListener)

        ''' <summary>
        ''' Sets the designated parameter to the given <code>java.sql.SQLXML</code> object. The driver converts this to an
        ''' SQL <code>XML</code> value when it sends it to the database. </summary>
        ''' <param name="parameterIndex"> index of the first parameter is 1, the second is 2, ... </param>
        ''' <param name="xmlObject"> a <code>SQLXML</code> object that maps an SQL <code>XML</code> value </param>
        ''' <exception cref="SQLException"> if a database access error occurs, this method
        '''  is called on a closed result set,
        ''' the <code>java.xml.transform.Result</code>,
        '''  <code>Writer</code> or <code>OutputStream</code> has not been closed
        ''' for the <code>SQLXML</code> object  or
        '''  if there is an error processing the XML value.  The <code>getCause</code> method
        '''  of the exception may provide a more detailed exception, for example, if the
        '''  stream does not contain valid XML.
        ''' @since 1.6 </exception>
        Sub setSQLXML(ByVal parameterIndex As Integer, ByVal xmlObject As SQLXML)

        ''' <summary>
        ''' Sets the designated parameter to the given <code>java.sql.SQLXML</code> object. The driver converts this to an
        ''' <code>SQL XML</code> value when it sends it to the database. </summary>
        ''' <param name="parameterName"> the name of the parameter </param>
        ''' <param name="xmlObject"> a <code>SQLXML</code> object that maps an <code>SQL XML</code> value </param>
        ''' <exception cref="SQLException"> if a database access error occurs, this method
        '''  is called on a closed result set,
        ''' the <code>java.xml.transform.Result</code>,
        '''  <code>Writer</code> or <code>OutputStream</code> has not been closed
        ''' for the <code>SQLXML</code> object  or
        '''  if there is an error processing the XML value.  The <code>getCause</code> method
        '''  of the exception may provide a more detailed exception, for example, if the
        '''  stream does not contain valid XML.
        ''' @since 1.6 </exception>
        Sub setSQLXML(ByVal parameterName As String, ByVal xmlObject As SQLXML)

        ''' <summary>
        ''' Sets the designated parameter to the given <code>java.sql.RowId</code> object. The
        ''' driver converts this to a SQL <code>ROWID</code> value when it sends it
        ''' to the database
        ''' </summary>
        ''' <param name="parameterIndex"> the first parameter is 1, the second is 2, ... </param>
        ''' <param name="x"> the parameter value </param>
        ''' <exception cref="SQLException"> if a database access error occurs
        ''' 
        ''' @since 1.6 </exception>
        Sub setRowId(ByVal parameterIndex As Integer, ByVal x As RowId)

        ''' <summary>
        ''' Sets the designated parameter to the given <code>java.sql.RowId</code> object. The
        ''' driver converts this to a SQL <code>ROWID</code> when it sends it to the
        ''' database.
        ''' </summary>
        ''' <param name="parameterName"> the name of the parameter </param>
        ''' <param name="x"> the parameter value </param>
        ''' <exception cref="SQLException"> if a database access error occurs
        ''' @since 1.6 </exception>
        Sub setRowId(ByVal parameterName As String, ByVal x As RowId)

        ''' <summary>
        ''' Sets the designated parameter to the given <code>String</code> object.
        ''' The driver converts this to a SQL <code>NCHAR</code> or
        ''' <code>NVARCHAR</code> or <code>LONGNVARCHAR</code> value
        ''' (depending on the argument's
        ''' size relative to the driver's limits on <code>NVARCHAR</code> values)
        ''' when it sends it to the database.
        ''' </summary>
        ''' <param name="parameterIndex"> of the first parameter is 1, the second is 2, ... </param>
        ''' <param name="value"> the parameter value </param>
        ''' <exception cref="SQLException"> if the driver does not support national
        '''         character sets;  if the driver can detect that a data conversion
        '''  error could occur ; or if a database access error occurs
        ''' @since 1.6 </exception>
        Sub setNString(ByVal parameterIndex As Integer, ByVal value As String)

        ''' <summary>
        ''' Sets the designated parameter to the given <code>String</code> object.
        ''' The driver converts this to a SQL <code>NCHAR</code> or
        ''' <code>NVARCHAR</code> or <code>LONGNVARCHAR</code> </summary>
        ''' <param name="parameterName"> the name of the column to be set </param>
        ''' <param name="value"> the parameter value </param>
        ''' <exception cref="SQLException"> if the driver does not support national
        '''         character sets;  if the driver can detect that a data conversion
        '''  error could occur; or if a database access error occurs
        ''' @since 1.6 </exception>
        Sub setNString(ByVal parameterName As String, ByVal value As String)

        ''' <summary>
        ''' Sets the designated parameter to a <code>Reader</code> object. The
        ''' <code>Reader</code> reads the data till end-of-file is reached. The
        ''' driver does the necessary conversion from Java character format to
        ''' the national character set in the database. </summary>
        ''' <param name="parameterIndex"> of the first parameter is 1, the second is 2, ... </param>
        ''' <param name="value"> the parameter value </param>
        ''' <param name="length"> the number of characters in the parameter data. </param>
        ''' <exception cref="SQLException"> if the driver does not support national
        '''         character sets;  if the driver can detect that a data conversion
        '''  error could occur ; or if a database access error occurs
        ''' @since 1.6 </exception>
        Sub setNCharacterStream(ByVal parameterIndex As Integer, ByVal value As Reader, ByVal length As Long)

        ''' <summary>
        ''' Sets the designated parameter to a <code>Reader</code> object. The
        ''' <code>Reader</code> reads the data till end-of-file is reached. The
        ''' driver does the necessary conversion from Java character format to
        ''' the national character set in the database. </summary>
        ''' <param name="parameterName"> the name of the column to be set </param>
        ''' <param name="value"> the parameter value </param>
        ''' <param name="length"> the number of characters in the parameter data. </param>
        ''' <exception cref="SQLException"> if the driver does not support national
        '''         character sets;  if the driver can detect that a data conversion
        '''  error could occur; or if a database access error occurs
        ''' @since 1.6 </exception>
        Sub setNCharacterStream(ByVal parameterName As String, ByVal value As Reader, ByVal length As Long)

        ''' <summary>
        ''' Sets the designated parameter to a <code>Reader</code> object. The
        ''' <code>Reader</code> reads the data till end-of-file is reached. The
        ''' driver does the necessary conversion from Java character format to
        ''' the national character set in the database.
        ''' 
        ''' <P><B>Note:</B> This stream object can either be a standard
        ''' Java stream object or your own subclass that implements the
        ''' standard interface.
        ''' <P><B>Note:</B> Consult your JDBC driver documentation to determine if
        ''' it might be more efficient to use a version of
        ''' <code>setNCharacterStream</code> which takes a length parameter.
        ''' </summary>
        ''' <param name="parameterName"> the name of the parameter </param>
        ''' <param name="value"> the parameter value </param>
        ''' <exception cref="SQLException"> if the driver does not support national
        '''         character sets;  if the driver can detect that a data conversion
        '''  error could occur ; if a database access error occurs; or
        ''' this method is called on a closed <code>CallableStatement</code> </exception>
        ''' <exception cref="SQLFeatureNotSupportedException">  if the JDBC driver does not support this method
        ''' @since 1.6 </exception>
        Sub setNCharacterStream(ByVal parameterName As String, ByVal value As Reader)

        ''' <summary>
        ''' Sets the designated parameter to a <code>java.sql.NClob</code> object. The object
        ''' implements the <code>java.sql.NClob</code> interface. This <code>NClob</code>
        ''' object maps to a SQL <code>NCLOB</code>. </summary>
        ''' <param name="parameterName"> the name of the column to be set </param>
        ''' <param name="value"> the parameter value </param>
        ''' <exception cref="SQLException"> if the driver does not support national
        '''         character sets;  if the driver can detect that a data conversion
        '''  error could occur; or if a database access error occurs
        ''' @since 1.6 </exception>
        Sub setNClob(ByVal parameterName As String, ByVal value As NClob)

        ''' <summary>
        ''' Sets the designated parameter to a <code>Reader</code> object.  The <code>reader</code> must contain  the number
        ''' of characters specified by length otherwise a <code>SQLException</code> will be
        ''' generated when the <code>CallableStatement</code> is executed.
        ''' This method differs from the <code>setCharacterStream (int, Reader, int)</code> method
        ''' because it informs the driver that the parameter value should be sent to
        ''' the server as a <code>NCLOB</code>.  When the <code>setCharacterStream</code> method is used, the
        ''' driver may have to do extra work to determine whether the parameter
        ''' data should be send to the server as a <code>LONGNVARCHAR</code> or a <code>NCLOB</code>
        ''' </summary>
        ''' <param name="parameterName"> the name of the parameter to be set </param>
        ''' <param name="reader"> An object that contains the data to set the parameter value to. </param>
        ''' <param name="length"> the number of characters in the parameter data. </param>
        ''' <exception cref="SQLException"> if parameterIndex does not correspond to a parameter
        ''' marker in the SQL statement; if the length specified is less than zero;
        ''' if the driver does not support national
        '''         character sets;  if the driver can detect that a data conversion
        '''  error could occur; if a database access error occurs or
        ''' this method is called on a closed <code>CallableStatement</code> </exception>
        ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
        ''' this method
        ''' @since 1.6 </exception>
        Sub setNClob(ByVal parameterName As String, ByVal reader As Reader, ByVal length As Long)

        ''' <summary>
        ''' Sets the designated parameter to a <code>Reader</code> object.
        ''' This method differs from the <code>setCharacterStream (int, Reader)</code> method
        ''' because it informs the driver that the parameter value should be sent to
        ''' the server as a <code>NCLOB</code>.  When the <code>setCharacterStream</code> method is used, the
        ''' driver may have to do extra work to determine whether the parameter
        ''' data should be send to the server as a <code>LONGNVARCHAR</code> or a <code>NCLOB</code>
        ''' <P><B>Note:</B> Consult your JDBC driver documentation to determine if
        ''' it might be more efficient to use a version of
        ''' <code>setNClob</code> which takes a length parameter.
        ''' </summary>
        ''' <param name="parameterName"> the name of the parameter </param>
        ''' <param name="reader"> An object that contains the data to set the parameter value to. </param>
        ''' <exception cref="SQLException"> if the driver does not support national character sets;
        ''' if the driver can detect that a data conversion
        '''  error could occur;  if a database access error occurs or
        ''' this method is called on a closed <code>CallableStatement</code> </exception>
        ''' <exception cref="SQLFeatureNotSupportedException">  if the JDBC driver does not support this method
        ''' 
        ''' @since 1.6 </exception>
        Sub setNClob(ByVal parameterName As String, ByVal reader As Reader)

        ''' <summary>
        ''' Sets the designated parameter to a <code>Reader</code> object.  The reader must contain  the number
        ''' of characters specified by length otherwise a <code>SQLException</code> will be
        ''' generated when the <code>PreparedStatement</code> is executed.
        ''' This method differs from the <code>setCharacterStream (int, Reader, int)</code> method
        ''' because it informs the driver that the parameter value should be sent to
        ''' the server as a <code>NCLOB</code>.  When the <code>setCharacterStream</code> method is used, the
        ''' driver may have to do extra work to determine whether the parameter
        ''' data should be sent to the server as a <code>LONGNVARCHAR</code> or a <code>NCLOB</code> </summary>
        ''' <param name="parameterIndex"> index of the first parameter is 1, the second is 2, ... </param>
        ''' <param name="reader"> An object that contains the data to set the parameter value to. </param>
        ''' <param name="length"> the number of characters in the parameter data. </param>
        ''' <exception cref="SQLException"> if parameterIndex does not correspond to a parameter
        ''' marker in the SQL statement; if the length specified is less than zero;
        ''' if the driver does not support national character sets;
        ''' if the driver can detect that a data conversion
        '''  error could occur;  if a database access error occurs or
        ''' this method is called on a closed <code>PreparedStatement</code> </exception>
        ''' <exception cref="SQLFeatureNotSupportedException">  if the JDBC driver does not support this method
        ''' 
        ''' @since 1.6 </exception>
        Sub setNClob(ByVal parameterIndex As Integer, ByVal reader As Reader, ByVal length As Long)

        ''' <summary>
        ''' Sets the designated parameter to a <code>java.sql.NClob</code> object. The driver converts this to a
        ''' SQL <code>NCLOB</code> value when it sends it to the database. </summary>
        ''' <param name="parameterIndex"> of the first parameter is 1, the second is 2, ... </param>
        ''' <param name="value"> the parameter value </param>
        ''' <exception cref="SQLException"> if the driver does not support national
        '''         character sets;  if the driver can detect that a data conversion
        '''  error could occur ; or if a database access error occurs
        ''' @since 1.6 </exception>
        Sub setNClob(ByVal parameterIndex As Integer, ByVal value As NClob)

        ''' <summary>
        ''' Sets the designated parameter to a <code>Reader</code> object.
        ''' This method differs from the <code>setCharacterStream (int, Reader)</code> method
        ''' because it informs the driver that the parameter value should be sent to
        ''' the server as a <code>NCLOB</code>.  When the <code>setCharacterStream</code> method is used, the
        ''' driver may have to do extra work to determine whether the parameter
        ''' data should be sent to the server as a <code>LONGNVARCHAR</code> or a <code>NCLOB</code>
        ''' <P><B>Note:</B> Consult your JDBC driver documentation to determine if
        ''' it might be more efficient to use a version of
        ''' <code>setNClob</code> which takes a length parameter.
        ''' </summary>
        ''' <param name="parameterIndex"> index of the first parameter is 1, the second is 2, ... </param>
        ''' <param name="reader"> An object that contains the data to set the parameter value to. </param>
        ''' <exception cref="SQLException"> if parameterIndex does not correspond to a parameter
        ''' marker in the SQL statement;
        ''' if the driver does not support national character sets;
        ''' if the driver can detect that a data conversion
        '''  error could occur;  if a database access error occurs or
        ''' this method is called on a closed <code>PreparedStatement</code> </exception>
        ''' <exception cref="SQLFeatureNotSupportedException">  if the JDBC driver does not support this method
        ''' 
        ''' @since 1.6 </exception>
        Sub setNClob(ByVal parameterIndex As Integer, ByVal reader As Reader)

        ''' <summary>
        ''' Sets the designated parameter to the given <code>java.net.URL</code> value.
        ''' The driver converts this to an SQL <code>DATALINK</code> value
        ''' when it sends it to the database.
        ''' </summary>
        ''' <param name="parameterIndex"> the first parameter is 1, the second is 2, ... </param>
        ''' <param name="x"> the <code>java.net.URL</code> object to be set </param>
        ''' <exception cref="SQLException"> if a database access error occurs or
        ''' this method is called on a closed <code>PreparedStatement</code> </exception>
        ''' <exception cref="SQLFeatureNotSupportedException">  if the JDBC driver does not support this method
        ''' @since 1.4 </exception>
        Sub setURL(ByVal parameterIndex As Integer, ByVal x As java.net.URL)



    End Interface

End Namespace