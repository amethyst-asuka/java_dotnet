Imports System
Imports System.Collections.Generic
Imports javax.sql
Imports javax.sql.rowset.serial

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

Namespace javax.sql.rowset


	''' <summary>
	''' An abstract class providing a <code>RowSet</code> object with its basic functionality.
	''' The basic functions include having properties and sending event notifications,
	''' which all JavaBeans&trade; components must implement.
	''' 
	''' <h3>1.0 Overview</h3>
	''' The <code>BaseRowSet</code> class provides the core functionality
	''' for all <code>RowSet</code> implementations,
	''' and all standard implementations <b>may</b> use this class in combination with
	''' one or more <code>RowSet</code> interfaces in order to provide a standard
	''' vendor-specific implementation.  To clarify, all implementations must implement
	''' at least one of the <code>RowSet</code> interfaces (<code>JdbcRowSet</code>,
	''' <code>CachedRowSet</code>, <code>JoinRowSet</code>, <code>FilteredRowSet</code>,
	''' or <code>WebRowSet</code>). This means that any implementation that extends
	''' the <code>BaseRowSet</code> class must also implement one of the <code>RowSet</code>
	''' interfaces.
	''' <p>
	''' The <code>BaseRowSet</code> class provides the following:
	''' 
	''' <UL>
	''' <LI><b>Properties</b>
	'''     <ul>
	'''     <li>Fields for storing current properties
	'''     <li>Methods for getting and setting properties
	'''     </ul>
	''' 
	''' <LI><b>Event notification</b>
	''' 
	''' <LI><b>A complete set of setter methods</b> for setting the parameters in a
	'''      <code>RowSet</code> object's command
	''' 
	''' <LI> <b>Streams</b>
	'''  <ul>
	'''  <li>Fields for storing stream instances
	'''  <li>Constants for indicating the type of a stream
	'''  </ul>
	'''  <p>
	''' </UL>
	''' 
	''' <h3>2.0 Setting Properties</h3>
	''' All rowsets maintain a set of properties, which will usually be set using
	''' a tool.  The number and kinds of properties a rowset has will vary,
	''' depending on what the <code>RowSet</code> implementation does and how it gets
	''' its data.  For example,
	''' rowsets that get their data from a <code>ResultSet</code> object need to
	''' set the properties that are required for making a database connection.
	''' If a <code>RowSet</code> object uses the <code>DriverManager</code> facility to make a
	''' connection, it needs to set a property for the JDBC URL that identifies the
	''' appropriate driver, and it needs to set the properties that give the
	''' user name and password.
	''' If, on the other hand, the rowset uses a <code>DataSource</code> object
	''' to make the connection, which is the preferred method, it does not need to
	''' set the property for the JDBC URL.  Instead, it needs to set the property
	''' for the logical name of the data source along with the properties for
	''' the user name and password.
	''' <P>
	''' NOTE:  In order to use a <code>DataSource</code> object for making a
	''' connection, the <code>DataSource</code> object must have been registered
	''' with a naming service that uses the Java Naming and Directory
	''' Interface&trade; (JNDI) API.  This registration
	''' is usually done by a person acting in the capacity of a system administrator.
	''' 
	''' <h3>3.0 Setting the Command and Its Parameters</h3>
	''' When a rowset gets its data from a relational database, it executes a command (a query)
	''' that produces a <code>ResultSet</code> object.  This query is the command that is set
	''' for the <code>RowSet</code> object's command property.  The rowset populates itself with data by reading the
	''' data from the <code>ResultSet</code> object into itself. If the query
	''' contains placeholders for values to be set, the <code>BaseRowSet</code> setter methods
	''' are used to set these values. All setter methods allow these values to be set
	''' to <code>null</code> if required.
	''' <P>
	''' The following code fragment illustrates how the
	''' <code>CachedRowSet</code>&trade;
	''' object <code>crs</code> might have its command property set.  Note that if a
	''' tool is used to set properties, this is the code that the tool would use.
	''' <PRE>{@code
	'''    crs.setCommand("SELECT FIRST_NAME, LAST_NAME, ADDRESS FROM CUSTOMERS" +
	'''                   "WHERE CREDIT_LIMIT > ? AND REGION = ?");
	''' }</PRE>
	''' <P>
	''' In this example, the values for <code>CREDIT_LIMIT</code> and
	''' <code>REGION</code> are placeholder parameters, which are indicated with a
	''' question mark (?).  The first question mark is placeholder parameter number
	''' <code>1</code>, the second question mark is placeholder parameter number
	''' <code>2</code>, and so on.  Any placeholder parameters must be set with
	''' values before the query can be executed. To set these
	''' placeholder parameters, the <code>BaseRowSet</code> class provides a set of setter
	''' methods, similar to those provided by the <code>PreparedStatement</code>
	''' interface, for setting values of each data type.  A <code>RowSet</code> object stores the
	''' parameter values internally, and its <code>execute</code> method uses them internally
	''' to set values for the placeholder parameters
	''' before it sends the command to the DBMS to be executed.
	''' <P>
	''' The following code fragment demonstrates
	''' setting the two parameters in the query from the previous example.
	''' <PRE>{@code
	'''    crs.setInt(1, 5000);
	'''    crs.setString(2, "West");
	''' }</PRE>
	''' If the <code>execute</code> method is called at this point, the query
	''' sent to the DBMS will be:
	''' <PRE>{@code
	'''    "SELECT FIRST_NAME, LAST_NAME, ADDRESS FROM CUSTOMERS" +
	'''                   "WHERE CREDIT_LIMIT > 5000 AND REGION = 'West'"
	''' }</PRE>
	''' NOTE: Setting <code>Array</code>, <code>Clob</code>, <code>Blob</code> and
	''' <code>Ref</code> objects as a command parameter, stores these values as
	''' <code>SerialArray</code>, <code>SerialClob</code>, <code>SerialBlob</code>
	''' and <code>SerialRef</code> objects respectively.
	''' 
	''' <h3>4.0 Handling of Parameters Behind the Scenes</h3>
	''' 
	''' NOTE: The <code>BaseRowSet</code> class provides two kinds of setter methods,
	''' those that set properties and those that set placeholder parameters. The setter
	''' methods discussed in this section are those that set placeholder parameters.
	''' <P>
	''' The placeholder parameters set with the <code>BaseRowSet</code> setter methods
	''' are stored as objects in an internal <code>Hashtable</code> object.
	''' Primitives are stored as their <code>Object</code> type. For example, <code>byte</code>
	''' is stored as <code>Byte</code> object, and <code>int</code> is stored as
	''' an <code>Integer</code> object.
	''' When the method <code>execute</code> is called, the values in the
	''' <code>Hashtable</code> object are substituted for the appropriate placeholder
	''' parameters in the command.
	''' <P>
	''' A call to the method <code>getParams</code> returns the values stored in the
	''' <code>Hashtable</code> object as an array of <code>Object</code> instances.
	''' An element in this array may be a simple <code>Object</code> instance or an
	''' array (which is a type of <code>Object</code>). The particular setter method used
	''' determines whether an element in this array is an <code>Object</code> or an array.
	''' <P>
	''' The majority of methods for setting placeholder parameters take two parameters,
	'''  with the first parameter
	''' indicating which placeholder parameter is to be set, and the second parameter
	''' giving the value to be set.  Methods such as <code>setInt</code>,
	''' <code>setString</code>, <code>setBoolean</code>, and <code>setLong</code> fall into
	''' this category.  After these methods have been called, a call to the method
	''' <code>getParams</code> will return an array with the values that have been set. Each
	''' element in the array is an <code>Object</code> instance representing the
	''' values that have been set. The order of these values in the array is determined by the
	''' <code>int</code> (the first parameter) passed to the setter method. The values in the
	''' array are the values (the second parameter) passed to the setter method.
	''' In other words, the first element in the array is the value
	''' to be set for the first placeholder parameter in the <code>RowSet</code> object's
	''' command. The second element is the value to
	''' be set for the second placeholder parameter, and so on.
	''' <P>
	''' Several setter methods send the driver and DBMS information beyond the value to be set.
	''' When the method <code>getParams</code> is called after one of these setter methods has
	''' been used, the elements in the array will themselves be arrays to accommodate the
	''' additional information. In this category, the method <code>setNull</code> is a special case
	''' because one version takes only
	''' two parameters (<code>setNull(int parameterIndex, int SqlType)</code>). Nevertheless,
	''' it requires
	''' an array to contain the information that will be passed to the driver and DBMS.  The first
	''' element in this array is the value to be set, which is <code>null</code>, and the
	''' second element is the <code>int</code> supplied for <i>sqlType</i>, which
	''' indicates the type of SQL value that is being set to <code>null</code>. This information
	''' is needed by some DBMSs and is therefore required in order to ensure that applications
	''' are portable.
	''' The other version is intended to be used when the value to be set to <code>null</code>
	''' is a user-defined type. It takes three parameters
	''' (<code>setNull(int parameterIndex, int sqlType, String typeName)</code>) and also
	''' requires an array to contain the information to be passed to the driver and DBMS.
	''' The first two elements in this array are the same as for the first version of
	''' <code>setNull</code>.  The third element, <i>typeName</i>, gives the SQL name of
	''' the user-defined type. As is true with the other setter methods, the number of the
	''' placeholder parameter to be set is indicated by an element's position in the array
	''' returned by <code>getParams</code>.  So, for example, if the parameter
	''' supplied to <code>setNull</code> is <code>2</code>, the second element in the array
	''' returned by <code>getParams</code> will be an array of two or three elements.
	''' <P>
	''' Some methods, such as <code>setObject</code> and <code>setDate</code> have versions
	''' that take more than two parameters, with the extra parameters giving information
	''' to the driver or the DBMS. For example, the methods <code>setDate</code>,
	''' <code>setTime</code>, and <code>setTimestamp</code> can take a <code>Calendar</code>
	''' object as their third parameter.  If the DBMS does not store time zone information,
	''' the driver uses the <code>Calendar</code> object to construct the <code>Date</code>,
	''' <code>Time</code>, or <code>Timestamp</code> object being set. As is true with other
	''' methods that provide additional information, the element in the array returned
	''' by <code>getParams</code> is an array instead of a simple <code>Object</code> instance.
	''' <P>
	''' The methods <code>setAsciiStream</code>, <code>setBinaryStream</code>,
	''' <code>setCharacterStream</code>, and <code>setUnicodeStream</code> (which is
	''' deprecated, so applications should use <code>getCharacterStream</code> instead)
	''' take three parameters, so for them, the element in the array returned by
	''' <code>getParams</code> is also an array.  What is different about these setter
	''' methods is that in addition to the information provided by parameters, the array contains
	''' one of the <code>BaseRowSet</code> constants indicating the type of stream being set.
	''' <p>
	''' NOTE: The method <code>getParams</code> is called internally by
	''' <code>RowSet</code> implementations extending this class; it is not normally called by an
	''' application programmer directly.
	''' 
	''' <h3>5.0 Event Notification</h3>
	''' The <code>BaseRowSet</code> class provides the event notification
	''' mechanism for rowsets.  It contains the field
	''' <code>listeners</code>, methods for adding and removing listeners, and
	''' methods for notifying listeners of changes.
	''' <P>
	''' A listener is an object that has implemented the <code>RowSetListener</code> interface.
	''' If it has been added to a <code>RowSet</code> object's list of listeners, it will be notified
	'''  when an event occurs on that <code>RowSet</code> object.  Each listener's
	''' implementation of the <code>RowSetListener</code> methods defines what that object
	''' will do when it is notified that an event has occurred.
	''' <P>
	''' There are three possible events for a <code>RowSet</code> object:
	''' <OL>
	''' <LI>the cursor moves
	''' <LI>an individual row is changed (updated, deleted, or inserted)
	''' <LI>the contents of the entire <code>RowSet</code> object  are changed
	''' </OL>
	''' <P>
	''' The <code>BaseRowSet</code> method used for the notification indicates the
	''' type of event that has occurred.  For example, the method
	''' <code>notifyRowChanged</code> indicates that a row has been updated,
	''' deleted, or inserted.  Each of the notification methods creates a
	''' <code>RowSetEvent</code> object, which is supplied to the listener in order to
	''' identify the <code>RowSet</code> object on which the event occurred.
	''' What the listener does with this information, which may be nothing, depends on how it was
	''' implemented.
	''' 
	''' <h3>6.0 Default Behavior</h3>
	''' A default <code>BaseRowSet</code> object is initialized with many starting values.
	''' 
	''' The following is true of a default <code>RowSet</code> instance that extends
	''' the <code>BaseRowSet</code> class:
	''' <UL>
	'''   <LI>Has a scrollable cursor and does not show changes
	'''       made by others.
	'''   <LI>Is updatable.
	'''   <LI>Does not show rows that have been deleted.
	'''   <LI>Has no time limit for how long a driver may take to
	'''       execute the <code>RowSet</code> object's command.
	'''   <LI>Has no limit for the number of rows it may contain.
	'''   <LI>Has no limit for the number of bytes a column may contain. NOTE: This
	'''   limit applies only to columns that hold values of the
	'''   following types:  <code>BINARY</code>, <code>VARBINARY</code>,
	'''   <code>LONGVARBINARY</code>, <code>CHAR</code>, <code>VARCHAR</code>,
	'''   and <code>LONGVARCHAR</code>.
	'''   <LI>Will not see uncommitted data (make "dirty" reads).
	'''   <LI>Has escape processing turned on.
	'''   <LI>Has its connection's type map set to <code>null</code>.
	'''   <LI>Has an empty <code>Vector</code> object for storing the values set
	'''       for the placeholder parameters in the <code>RowSet</code> object's command.
	''' </UL>
	''' <p>
	''' If other values are desired, an application must set the property values
	''' explicitly. For example, the following line of code sets the maximum number
	''' of rows for the <code>CachedRowSet</code> object <i>crs</i> to 500.
	''' <PRE>
	'''    crs.setMaxRows(500);
	''' </PRE>
	''' Methods implemented in extensions of this <code>BaseRowSet</code> class <b>must</b> throw an
	''' <code>SQLException</code> object for any violation of the defined assertions.  Also, if the
	''' extending class overrides and reimplements any <code>BaseRowSet</code> method and encounters
	''' connectivity or underlying data source issues, that method <b>may</b> in addition throw an
	''' <code>SQLException</code> object for that reason.
	''' </summary>

	<Serializable> _
	Public MustInherit Class BaseRowSet
		Implements ICloneable

		''' <summary>
		''' A constant indicating to a <code>RowSetReaderImpl</code> object
		''' that a given parameter is a Unicode stream. This
		''' <code>RowSetReaderImpl</code> object is provided as an extension of the
		''' <code>SyncProvider</code> abstract class defined in the
		''' <code>SyncFactory</code> static factory SPI mechanism.
		''' </summary>
		Public Const UNICODE_STREAM_PARAM As Integer = 0

		''' <summary>
		''' A constant indicating to a <code>RowSetReaderImpl</code> object
		''' that a given parameter is a binary stream. A
		''' <code>RowSetReaderImpl</code> object is provided as an extension of the
		''' <code>SyncProvider</code> abstract class defined in the
		''' <code>SyncFactory</code> static factory SPI mechanism.
		''' </summary>
		Public Const BINARY_STREAM_PARAM As Integer = 1

		''' <summary>
		''' A constant indicating to a <code>RowSetReaderImpl</code> object
		''' that a given parameter is an ASCII stream. A
		''' <code>RowSetReaderImpl</code> object is provided as an extension of the
		''' <code>SyncProvider</code> abstract class defined in the
		''' <code>SyncFactory</code> static factory SPI mechanism.
		''' </summary>
		Public Const ASCII_STREAM_PARAM As Integer = 2

		''' <summary>
		''' The <code>InputStream</code> object that will be
		''' returned by the method <code>getBinaryStream</code>, which is
		''' specified in the <code>ResultSet</code> interface.
		''' @serial
		''' </summary>
		Protected Friend binaryStream As java.io.InputStream

		''' <summary>
		''' The <code>InputStream</code> object that will be
		''' returned by the method <code>getUnicodeStream</code>,
		''' which is specified in the <code>ResultSet</code> interface.
		''' @serial
		''' </summary>
		Protected Friend unicodeStream As java.io.InputStream

		''' <summary>
		''' The <code>InputStream</code> object that will be
		''' returned by the method <code>getAsciiStream</code>,
		''' which is specified in the <code>ResultSet</code> interface.
		''' @serial
		''' </summary>
		Protected Friend asciiStream As java.io.InputStream

		''' <summary>
		''' The <code>Reader</code> object that will be
		''' returned by the method <code>getCharacterStream</code>,
		''' which is specified in the <code>ResultSet</code> interface.
		''' @serial
		''' </summary>
		Protected Friend charStream As java.io.Reader

		''' <summary>
		''' The query that will be sent to the DBMS for execution when the
		''' method <code>execute</code> is called.
		''' @serial
		''' </summary>
		Private command As String

		''' <summary>
		''' The JDBC URL the reader, writer, or both supply to the method
		''' <code>DriverManager.getConnection</code> when the
		''' <code>DriverManager</code> is used to get a connection.
		''' <P>
		''' The JDBC URL identifies the driver to be used to make the conndection.
		''' This URL can be found in the documentation supplied by the driver
		''' vendor.
		''' @serial
		''' </summary>
		Private URL As String

		''' <summary>
		''' The logical name of the data source that the reader/writer should use
		''' in order to retrieve a <code>DataSource</code> object from a Java
		''' Directory and Naming Interface (JNDI) naming service.
		''' @serial
		''' </summary>
		Private dataSource As String

		''' <summary>
		''' The user name the reader, writer, or both supply to the method
		''' <code>DriverManager.getConnection</code> when the
		''' <code>DriverManager</code> is used to get a connection.
		''' @serial
		''' </summary>
		<NonSerialized> _
		Private username As String

		''' <summary>
		''' The password the reader, writer, or both supply to the method
		''' <code>DriverManager.getConnection</code> when the
		''' <code>DriverManager</code> is used to get a connection.
		''' @serial
		''' </summary>
		<NonSerialized> _
		Private password As String

		''' <summary>
		''' A constant indicating the type of this JDBC <code>RowSet</code>
		''' object. It must be one of the following <code>ResultSet</code>
		''' constants:  <code>TYPE_FORWARD_ONLY</code>,
		''' <code>TYPE_SCROLL_INSENSITIVE</code>, or
		''' <code>TYPE_SCROLL_SENSITIVE</code>.
		''' @serial
		''' </summary>
		Private rowSetType As Integer = ResultSet.TYPE_SCROLL_INSENSITIVE

		''' <summary>
		''' A <code>boolean</code> indicating whether deleted rows are visible in this
		''' JDBC <code>RowSet</code> object .
		''' @serial
		''' </summary>
		Private showDeleted As Boolean = False ' default is false

		''' <summary>
		''' The maximum number of seconds the driver
		''' will wait for a command to execute.  This limit applies while
		''' this JDBC <code>RowSet</code> object is connected to its data
		''' source, that is, while it is populating itself with
		''' data and while it is writing data back to the data source.
		''' @serial
		''' </summary>
		Private queryTimeout As Integer = 0 ' default is no timeout

		''' <summary>
		''' The maximum number of rows the reader should read.
		''' @serial
		''' </summary>
		Private maxRows As Integer = 0 ' default is no limit

		''' <summary>
		''' The maximum field size the reader should read.
		''' @serial
		''' </summary>
		Private maxFieldSize As Integer = 0 ' default is no limit

		''' <summary>
		''' A constant indicating the concurrency of this JDBC <code>RowSet</code>
		''' object. It must be one of the following <code>ResultSet</code>
		''' constants: <code>CONCUR_READ_ONLY</code> or
		''' <code>CONCUR_UPDATABLE</code>.
		''' @serial
		''' </summary>
		Private concurrency As Integer = ResultSet.CONCUR_UPDATABLE

		''' <summary>
		''' A <code>boolean</code> indicating whether this JDBC <code>RowSet</code>
		''' object is read-only.  <code>true</code> indicates that it is read-only;
		''' <code>false</code> that it is writable.
		''' @serial
		''' </summary>
		Private [readOnly] As Boolean

		''' <summary>
		''' A <code>boolean</code> indicating whether the reader for this
		''' JDBC <code>RowSet</code> object should perform escape processing.
		''' <code>true</code> means that escape processing is turned on;
		''' <code>false</code> that it is not. The default is <code>true</code>.
		''' @serial
		''' </summary>
		Private escapeProcessing As Boolean = True

		''' <summary>
		''' A constant indicating the isolation level of the connection
		''' for this JDBC <code>RowSet</code> object . It must be one of
		''' the following <code>Connection</code> constants:
		''' <code>TRANSACTION_NONE</code>,
		''' <code>TRANSACTION_READ_UNCOMMITTED</code>,
		''' <code>TRANSACTION_READ_COMMITTED</code>,
		''' <code>TRANSACTION_REPEATABLE_READ</code> or
		''' <code>TRANSACTION_SERIALIZABLE</code>.
		''' @serial
		''' </summary>
		Private isolation As Integer

		''' <summary>
		''' A constant used as a hint to the driver that indicates the direction in
		''' which data from this JDBC <code>RowSet</code> object  is going
		''' to be fetched. The following <code>ResultSet</code> constants are
		''' possible values:
		''' <code>FETCH_FORWARD</code>,
		''' <code>FETCH_REVERSE</code>,
		''' <code>FETCH_UNKNOWN</code>.
		''' <P>
		''' Unused at this time.
		''' @serial
		''' </summary>
		Private fetchDir As Integer = ResultSet.FETCH_FORWARD ' default fetch direction

		''' <summary>
		''' A hint to the driver that indicates the expected number of rows
		''' in this JDBC <code>RowSet</code> object .
		''' <P>
		''' Unused at this time.
		''' @serial
		''' </summary>
		Private fetchSize As Integer = 0 ' default fetchSize

		''' <summary>
		''' The <code>java.util.Map</code> object that contains entries mapping
		''' SQL type names to classes in the Java programming language for the
		''' custom mapping of user-defined types.
		''' @serial
		''' </summary>
		Private map As IDictionary(Of String, Type)

		''' <summary>
		''' A <code>Vector</code> object that holds the list of listeners
		''' that have registered with this <code>RowSet</code> object.
		''' @serial
		''' </summary>
		Private listeners As List(Of RowSetListener)

		''' <summary>
		''' A <code>Vector</code> object that holds the parameters set
		''' for this <code>RowSet</code> object's current command.
		''' @serial
		''' </summary>
		Private params As Dictionary(Of Integer?, Object) ' could be transient?

		''' <summary>
		''' Constructs a new <code>BaseRowSet</code> object initialized with
		''' a default <code>Vector</code> object for its <code>listeners</code>
		''' field. The other default values with which it is initialized are listed
		''' in Section 6.0 of the class comment for this class.
		''' </summary>
		Public Sub New()
			' allocate the listeners collection
			listeners = New List(Of RowSetListener)
		End Sub

		''' <summary>
		''' Performs the necessary internal configurations and initializations
		''' to allow any JDBC <code>RowSet</code> implementation to start using
		''' the standard facilities provided by a <code>BaseRowSet</code>
		''' instance. This method <b>should</b> be called after the <code>RowSet</code> object
		''' has been instantiated to correctly initialize all parameters. This method
		''' <b>should</b> never be called by an application, but is called from with
		''' a <code>RowSet</code> implementation extending this class.
		''' </summary>
		Protected Friend Overridable Sub initParams()
			params = New Dictionary(Of Integer?, Object)
		End Sub

		'--------------------------------------------------------------------
		' Events
		'--------------------------------------------------------------------

		''' <summary>
		''' The listener will be notified whenever an event occurs on this <code>RowSet</code>
		''' object.
		''' <P>
		''' A listener might, for example, be a table or graph that needs to
		''' be updated in order to accurately reflect the current state of
		''' the <code>RowSet</code> object.
		''' <p>
		''' <b>Note</b>: if the <code>RowSetListener</code> object is
		''' <code>null</code>, this method silently discards the <code>null</code>
		''' value and does not add a null reference to the set of listeners.
		''' <p>
		''' <b>Note</b>: if the listener is already set, and the new <code>RowSetListerner</code>
		''' instance is added to the set of listeners already registered to receive
		''' event notifications from this <code>RowSet</code>.
		''' </summary>
		''' <param name="listener"> an object that has implemented the
		'''     <code>javax.sql.RowSetListener</code> interface and wants to be notified
		'''     of any events that occur on this <code>RowSet</code> object; May be
		'''     null. </param>
		''' <seealso cref= #removeRowSetListener </seealso>
		Public Overridable Sub addRowSetListener(ByVal listener As RowSetListener)
			listeners.Add(listener)
		End Sub

		''' <summary>
		''' Removes the designated object from this <code>RowSet</code> object's list of listeners.
		''' If the given argument is not a registered listener, this method
		''' does nothing.
		''' 
		'''  <b>Note</b>: if the <code>RowSetListener</code> object is
		''' <code>null</code>, this method silently discards the <code>null</code>
		''' value.
		''' </summary>
		''' <param name="listener"> a <code>RowSetListener</code> object that is on the list
		'''        of listeners for this <code>RowSet</code> object </param>
		''' <seealso cref= #addRowSetListener </seealso>
		Public Overridable Sub removeRowSetListener(ByVal listener As RowSetListener)
			listeners.Remove(listener)
		End Sub

		''' <summary>
		''' Determine if instance of this class extends the RowSet interface.
		''' </summary>
		Private Sub checkforRowSetInterface()
			If (TypeOf Me Is javax.sql.RowSet) = False Then Throw New SQLException("The class extending abstract class BaseRowSet " & "must implement javax.sql.RowSet or one of it's sub-interfaces.")
		End Sub

		''' <summary>
		''' Notifies all of the listeners registered with this
		''' <code>RowSet</code> object that its cursor has moved.
		''' <P>
		''' When an application calls a method to move the cursor,
		''' that method moves the cursor and then calls this method
		''' internally. An application <b>should</b> never invoke
		''' this method directly.
		''' </summary>
		''' <exception cref="SQLException"> if the class extending the <code>BaseRowSet</code>
		'''     abstract class does not implement the <code>RowSet</code> interface or
		'''     one of it's sub-interfaces. </exception>
		Protected Friend Overridable Sub notifyCursorMoved()
			checkforRowSetInterface()
			If listeners.Count = 0 = False Then
				Dim [event] As New RowSetEvent(CType(Me, RowSet))
				For Each rsl As RowSetListener In listeners
					rsl.cursorMoved([event])
				Next rsl
			End If
		End Sub

		''' <summary>
		''' Notifies all of the listeners registered with this <code>RowSet</code> object that
		''' one of its rows has changed.
		''' <P>
		''' When an application calls a method that changes a row, such as
		''' the <code>CachedRowSet</code> methods <code>insertRow</code>,
		''' <code>updateRow</code>, or <code>deleteRow</code>,
		''' that method calls <code>notifyRowChanged</code>
		''' internally. An application <b>should</b> never invoke
		''' this method directly.
		''' </summary>
		''' <exception cref="SQLException"> if the class extending the <code>BaseRowSet</code>
		'''     abstract class does not implement the <code>RowSet</code> interface or
		'''     one of it's sub-interfaces. </exception>
		Protected Friend Overridable Sub notifyRowChanged()
			checkforRowSetInterface()
			If listeners.Count = 0 = False Then
					Dim [event] As New RowSetEvent(CType(Me, RowSet))
					For Each rsl As RowSetListener In listeners
						rsl.rowChanged([event])
					Next rsl
			End If
		End Sub

	   ''' <summary>
	   ''' Notifies all of the listeners registered with this <code>RowSet</code>
	   ''' object that its entire contents have changed.
	   ''' <P>
	   ''' When an application calls methods that change the entire contents
	   ''' of the <code>RowSet</code> object, such as the <code>CachedRowSet</code> methods
	   ''' <code>execute</code>, <code>populate</code>, <code>restoreOriginal</code>,
	   ''' or <code>release</code>, that method calls <code>notifyRowSetChanged</code>
	   ''' internally (either directly or indirectly). An application <b>should</b>
	   ''' never invoke this method directly.
	   ''' </summary>
	   ''' <exception cref="SQLException"> if the class extending the <code>BaseRowSet</code>
	   '''     abstract class does not implement the <code>RowSet</code> interface or
	   '''     one of it's sub-interfaces. </exception>
		Protected Friend Overridable Sub notifyRowSetChanged()
			checkforRowSetInterface()
			If listeners.Count = 0 = False Then
					Dim [event] As New RowSetEvent(CType(Me, RowSet))
					For Each rsl As RowSetListener In listeners
						rsl.rowSetChanged([event])
					Next rsl
			End If
		End Sub

		''' <summary>
		''' Retrieves the SQL query that is the command for this
		''' <code>RowSet</code> object. The command property contains the query that
		''' will be executed to populate this <code>RowSet</code> object.
		''' <P>
		''' The SQL query returned by this method is used by <code>RowSet</code> methods
		''' such as <code>execute</code> and <code>populate</code>, which may be implemented
		''' by any class that extends the <code>BaseRowSet</code> abstract class and
		''' implements one or more of the standard JSR-114 <code>RowSet</code>
		''' interfaces.
		''' <P>
		''' The command is used by the <code>RowSet</code> object's
		''' reader to obtain a <code>ResultSet</code> object.  The reader then
		''' reads the data from the <code>ResultSet</code> object and uses it to
		''' to populate this <code>RowSet</code> object.
		''' <P>
		''' The default value for the <code>command</code> property is <code>null</code>.
		''' </summary>
		''' <returns> the <code>String</code> that is the value for this
		'''         <code>RowSet</code> object's <code>command</code> property;
		'''         may be <code>null</code> </returns>
		''' <seealso cref= #setCommand </seealso>
		Public Overridable Property command As String
			Get
				Return command
			End Get
			Set(ByVal cmd As String)
				' cmd equal to null or
				' cmd with length 0 (implies url =="")
				' are not independent events.
    
				If cmd Is Nothing Then
				   command = Nothing
				ElseIf cmd.Length = 0 Then
					Throw New SQLException("Invalid command string detected. " & "Cannot be of length less than 0")
				Else
					' "unbind" any parameters from any previous command.
					If params Is Nothing Then Throw New SQLException("Set initParams() before setCommand")
					params.Clear()
					command = cmd
				End If
    
			End Set
		End Property


		''' <summary>
		''' Retrieves the JDBC URL that this <code>RowSet</code> object's
		''' <code>javax.sql.Reader</code> object uses to make a connection
		''' with a relational database using a JDBC technology-enabled driver.
		''' <P>
		''' The <code>Url</code> property will be <code>null</code> if the underlying data
		''' source is a non-SQL data source, such as a spreadsheet or an XML
		''' data source.
		''' </summary>
		''' <returns> a <code>String</code> object that contains the JDBC URL
		'''         used to establish the connection for this <code>RowSet</code>
		'''         object; may be <code>null</code> (default value) if not set </returns>
		''' <exception cref="SQLException"> if an error occurs retrieving the URL value </exception>
		''' <seealso cref= #setUrl </seealso>
		Public Overridable Property url As String
			Get
				Return URL
			End Get
			Set(ByVal url As String)
				If url Is Nothing Then
				   url = Nothing
				ElseIf url.Length < 1 Then
					Throw New SQLException("Invalid url string detected. " & "Cannot be of length less than 1")
				Else
					Me.URL = url
				End If
    
				dataSource = Nothing
    
			End Set
		End Property


		''' <summary>
		''' Returns the logical name that when supplied to a naming service
		''' that uses the Java Naming and Directory Interface (JNDI) API, will
		''' retrieve a <code>javax.sql.DataSource</code> object. This
		''' <code>DataSource</code> object can be used to establish a connection
		''' to the data source that it represents.
		''' <P>
		''' Users should set either the url or the data source name property.
		''' The driver will use the property set most recently to establish a
		''' connection.
		''' </summary>
		''' <returns> a <code>String</code> object that identifies the
		'''         <code>DataSource</code> object to be used for making a
		'''         connection; if no logical name has been set, <code>null</code>
		'''         is returned. </returns>
		''' <seealso cref= #setDataSourceName </seealso>
		Public Overridable Property dataSourceName As String
			Get
				Return dataSource
			End Get
			Set(ByVal name As String)
    
				If name Is Nothing Then
					dataSource = Nothing
				ElseIf name.Equals("") Then
				   Throw New SQLException("DataSource name cannot be empty string")
				Else
				   dataSource = name
				End If
    
				URL = Nothing
			End Set
		End Property



		''' <summary>
		''' Returns the user name used to create a database connection.  Because it
		''' is not serialized, the username property is set at runtime before
		''' calling the method <code>execute</code>.
		''' </summary>
		''' <returns> the <code>String</code> object containing the user name that
		'''         is supplied to the data source to create a connection; may be
		'''         <code>null</code> (default value) if not set </returns>
		''' <seealso cref= #setUsername </seealso>
		Public Overridable Property username As String
			Get
				Return username
			End Get
			Set(ByVal name As String)
				If name Is Nothing Then
				   username = Nothing
				Else
				   username = name
				End If
			End Set
		End Property


		''' <summary>
		''' Returns the password used to create a database connection for this
		''' <code>RowSet</code> object.  Because the password property is not
		''' serialized, it is set at run time before calling the method
		''' <code>execute</code>. The default value is <code>null</code>
		''' </summary>
		''' <returns> the <code>String</code> object that represents the password
		'''         that must be supplied to the database to create a connection </returns>
		''' <seealso cref= #setPassword </seealso>
		Public Overridable Property password As String
			Get
				Return password
			End Get
			Set(ByVal pass As String)
				If pass Is Nothing Then
				   password = Nothing
				Else
				   password = pass
				End If
			End Set
		End Property


		''' <summary>
		''' Sets the type for this <code>RowSet</code> object to the specified type.
		''' The default type is <code>ResultSet.TYPE_SCROLL_INSENSITIVE</code>.
		''' </summary>
		''' <param name="type"> one of the following constants:
		'''             <code>ResultSet.TYPE_FORWARD_ONLY</code>,
		'''             <code>ResultSet.TYPE_SCROLL_INSENSITIVE</code>, or
		'''             <code>ResultSet.TYPE_SCROLL_SENSITIVE</code> </param>
		''' <exception cref="SQLException"> if the parameter supplied is not one of the
		'''         following constants:
		'''          <code>ResultSet.TYPE_FORWARD_ONLY</code> or
		'''          <code>ResultSet.TYPE_SCROLL_INSENSITIVE</code>
		'''          <code>ResultSet.TYPE_SCROLL_SENSITIVE</code> </exception>
		''' <seealso cref= #getConcurrency </seealso>
		''' <seealso cref= #getType </seealso>
		Public Overridable Property type As Integer
			Set(ByVal type As Integer)
    
				If (type <> ResultSet.TYPE_FORWARD_ONLY) AndAlso (type <> ResultSet.TYPE_SCROLL_INSENSITIVE) AndAlso (type <> ResultSet.TYPE_SCROLL_SENSITIVE) Then Throw New SQLException("Invalid type of RowSet set. Must be either " & "ResultSet.TYPE_FORWARD_ONLY or ResultSet.TYPE_SCROLL_INSENSITIVE " & "or ResultSet.TYPE_SCROLL_SENSITIVE.")
				Me.rowSetType = type
			End Set
			Get
				Return rowSetType
			End Get
		End Property


		''' <summary>
		''' Sets the concurrency for this <code>RowSet</code> object to
		''' the specified concurrency. The default concurrency for any <code>RowSet</code>
		''' object (connected or disconnected) is <code>ResultSet.CONCUR_UPDATABLE</code>,
		''' but this method may be called at any time to change the concurrency.
		''' <P> </summary>
		''' <param name="concurrency"> one of the following constants:
		'''                    <code>ResultSet.CONCUR_READ_ONLY</code> or
		'''                    <code>ResultSet.CONCUR_UPDATABLE</code> </param>
		''' <exception cref="SQLException"> if the parameter supplied is not one of the
		'''         following constants:
		'''          <code>ResultSet.CONCUR_UPDATABLE</code> or
		'''          <code>ResultSet.CONCUR_READ_ONLY</code> </exception>
		''' <seealso cref= #getConcurrency </seealso>
		''' <seealso cref= #isReadOnly </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setConcurrency(ByVal concurrency As Integer) 'JavaToDotNetTempPropertySetconcurrency
		Public Overridable Property concurrency As Integer
			Set(ByVal concurrency As Integer)
    
				If (concurrency <> ResultSet.CONCUR_READ_ONLY) AndAlso (concurrency <> ResultSet.CONCUR_UPDATABLE) Then Throw New SQLException("Invalid concurrency set. Must be either " & "ResultSet.CONCUR_READ_ONLY or ResultSet.CONCUR_UPDATABLE.")
				Me.concurrency = concurrency
			End Set
			Get
		End Property

		''' <summary>
		''' Returns a <code>boolean</code> indicating whether this
		''' <code>RowSet</code> object is read-only.
		''' Any attempts to update a read-only <code>RowSet</code> object will result in an
		''' <code>SQLException</code> being thrown. By default,
		''' rowsets are updatable if updates are possible.
		''' </summary>
		''' <returns> <code>true</code> if this <code>RowSet</code> object
		'''         cannot be updated; <code>false</code> otherwise </returns>
		''' <seealso cref= #setConcurrency </seealso>
		''' <seealso cref= #setReadOnly </seealso>
		Public Overridable Property [readOnly] As Boolean
			Get
				Return [readOnly]
			End Get
			Set(ByVal value As Boolean)
				[readOnly] = value
			End Set
		End Property


		''' <summary>
		''' Returns the transaction isolation property for this
		''' <code>RowSet</code> object's connection. This property represents
		''' the transaction isolation level requested for use in transactions.
		''' <P>
		''' For <code>RowSet</code> implementations such as
		''' the <code>CachedRowSet</code> that operate in a disconnected environment,
		''' the <code>SyncProvider</code> object
		''' offers complementary locking and data integrity options. The
		''' options described below are pertinent only to connected <code>RowSet</code>
		''' objects (<code>JdbcRowSet</code> objects).
		''' </summary>
		''' <returns> one of the following constants:
		'''         <code>Connection.TRANSACTION_NONE</code>,
		'''         <code>Connection.TRANSACTION_READ_UNCOMMITTED</code>,
		'''         <code>Connection.TRANSACTION_READ_COMMITTED</code>,
		'''         <code>Connection.TRANSACTION_REPEATABLE_READ</code>, or
		'''         <code>Connection.TRANSACTION_SERIALIZABLE</code> </returns>
		''' <seealso cref= javax.sql.rowset.spi.SyncFactory </seealso>
		''' <seealso cref= javax.sql.rowset.spi.SyncProvider </seealso>
		''' <seealso cref= #setTransactionIsolation
		'''  </seealso>
		Public Overridable Property transactionIsolation As Integer
			Get
				Return isolation
			End Get
			Set(ByVal level As Integer)
				If (level <> Connection.TRANSACTION_NONE) AndAlso (level <> Connection.TRANSACTION_READ_COMMITTED) AndAlso (level <> Connection.TRANSACTION_READ_UNCOMMITTED) AndAlso (level <> Connection.TRANSACTION_REPEATABLE_READ) AndAlso (level <> Connection.TRANSACTION_SERIALIZABLE) Then Throw New SQLException("Invalid transaction isolation set. Must " & "be either " & "Connection.TRANSACTION_NONE or " & "Connection.TRANSACTION_READ_UNCOMMITTED or " & "Connection.TRANSACTION_READ_COMMITTED or " & "Connection.RRANSACTION_REPEATABLE_READ or " & "Connection.TRANSACTION_SERIALIZABLE")
				Me.isolation = level
			End Set
		End Property


		''' <summary>
		''' Retrieves the type map associated with the <code>Connection</code>
		''' object for this <code>RowSet</code> object.
		''' <P>
		''' Drivers that support the JDBC 3.0 API will create
		''' <code>Connection</code> objects with an associated type map.
		''' This type map, which is initially empty, can contain one or more
		''' fully-qualified SQL names and <code>Class</code> objects indicating
		''' the class to which the named SQL value will be mapped. The type mapping
		''' specified in the connection's type map is used for custom type mapping
		''' when no other type map supersedes it.
		''' <p>
		''' If a type map is explicitly supplied to a method that can perform
		''' custom mapping, that type map supersedes the connection's type map.
		''' </summary>
		''' <returns> the <code>java.util.Map</code> object that is the type map
		'''         for this <code>RowSet</code> object's connection </returns>
		Public Overridable Property typeMap As IDictionary(Of String, Type)
			Get
				Return map
			End Get
			Set(ByVal map As IDictionary(Of String, Type))
				Me.map = map
			End Set
		End Property


		''' <summary>
		''' Retrieves the maximum number of bytes that can be used for a column
		''' value in this <code>RowSet</code> object.
		''' This limit applies only to columns that hold values of the
		''' following types:  <code>BINARY</code>, <code>VARBINARY</code>,
		''' <code>LONGVARBINARY</code>, <code>CHAR</code>, <code>VARCHAR</code>,
		''' and <code>LONGVARCHAR</code>.  If the limit is exceeded, the excess
		''' data is silently discarded.
		''' </summary>
		''' <returns> an <code>int</code> indicating the current maximum column size
		'''     limit; zero means that there is no limit </returns>
		''' <exception cref="SQLException"> if an error occurs internally determining the
		'''    maximum limit of the column size </exception>
		Public Overridable Property maxFieldSize As Integer
			Get
				Return maxFieldSize
			End Get
			Set(ByVal max As Integer)
				If max < 0 Then Throw New SQLException("Invalid max field size set. Cannot be of " & "value: " & max)
				maxFieldSize = max
			End Set
		End Property


		''' <summary>
		''' Retrieves the maximum number of rows that this <code>RowSet</code> object may contain. If
		''' this limit is exceeded, the excess rows are silently dropped.
		''' </summary>
		''' <returns> an <code>int</code> indicating the current maximum number of
		'''     rows; zero means that there is no limit </returns>
		''' <exception cref="SQLException"> if an error occurs internally determining the
		'''     maximum limit of rows that a <code>Rowset</code> object can contain </exception>
		Public Overridable Property maxRows As Integer
			Get
				Return maxRows
			End Get
			Set(ByVal max As Integer)
				If max < 0 Then
					Throw New SQLException("Invalid max row size set. Cannot be of " & "value: " & max)
				ElseIf max < Me.fetchSize Then
					Throw New SQLException("Invalid max row size set. Cannot be less " & "than the fetchSize.")
				End If
				Me.maxRows = max
			End Set
		End Property


		''' <summary>
		''' Sets to the given <code>boolean</code> whether or not the driver will
		''' scan for escape syntax and do escape substitution before sending SQL
		''' statements to the database. The default is for the driver to do escape
		''' processing.
		''' <P>
		''' Note: Since <code>PreparedStatement</code> objects have usually been
		''' parsed prior to making this call, disabling escape processing for
		''' prepared statements will likely have no effect.
		''' </summary>
		''' <param name="enable"> <code>true</code> to enable escape processing;
		'''     <code>false</code> to disable it </param>
		''' <exception cref="SQLException"> if an error occurs setting the underlying JDBC
		''' technology-enabled driver to process the escape syntax </exception>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setEscapeProcessing(ByVal enable As Boolean) 'JavaToDotNetTempPropertySetescapeProcessing
		Public Overridable Property escapeProcessing As Boolean
			Set(ByVal enable As Boolean)
				escapeProcessing = enable
			End Set
			Get
		End Property

		''' <summary>
		''' Retrieves the maximum number of seconds the driver will wait for a
		''' query to execute. If the limit is exceeded, an <code>SQLException</code>
		''' is thrown.
		''' </summary>
		''' <returns> the current query timeout limit in seconds; zero means that
		'''     there is no limit </returns>
		''' <exception cref="SQLException"> if an error occurs in determining the query
		'''     time-out value </exception>
		Public Overridable Property queryTimeout As Integer
			Get
				Return queryTimeout
			End Get
			Set(ByVal seconds As Integer)
				If seconds < 0 Then Throw New SQLException("Invalid query timeout value set. Cannot be " & "of value: " & seconds)
				Me.queryTimeout = seconds
			End Set
		End Property


		''' <summary>
		''' Retrieves a <code>boolean</code> indicating whether rows marked
		''' for deletion appear in the set of current rows.
		''' The default value is <code>false</code>.
		''' <P>
		''' Note: Allowing deleted rows to remain visible complicates the behavior
		''' of some of the methods.  However, most <code>RowSet</code> object users
		''' can simply ignore this extra detail because only sophisticated
		''' applications will likely want to take advantage of this feature.
		''' </summary>
		''' <returns> <code>true</code> if deleted rows are visible;
		'''         <code>false</code> otherwise </returns>
		''' <exception cref="SQLException"> if an error occurs determining if deleted rows
		''' are visible or not </exception>
		''' <seealso cref= #setShowDeleted </seealso>
		Public Overridable Property showDeleted As Boolean
			Get
				Return showDeleted
			End Get
			Set(ByVal value As Boolean)
				showDeleted = value
			End Set
		End Property


			Return escapeProcessing
		End Function

		''' <summary>
		''' Gives the driver a performance hint as to the direction in
		''' which the rows in this <code>RowSet</code> object will be
		''' processed.  The driver may ignore this hint.
		''' <P>
		''' A <code>RowSet</code> object inherits the default properties of the
		''' <code>ResultSet</code> object from which it got its data.  That
		''' <code>ResultSet</code> object's default fetch direction is set by
		''' the <code>Statement</code> object that created it.
		''' <P>
		''' This method applies to a <code>RowSet</code> object only while it is
		''' connected to a database using a JDBC driver.
		''' <p>
		''' A <code>RowSet</code> object may use this method at any time to change
		''' its setting for the fetch direction.
		''' </summary>
		''' <param name="direction"> one of <code>ResultSet.FETCH_FORWARD</code>,
		'''                  <code>ResultSet.FETCH_REVERSE</code>, or
		'''                  <code>ResultSet.FETCH_UNKNOWN</code> </param>
		''' <exception cref="SQLException"> if (1) the <code>RowSet</code> type is
		'''     <code>TYPE_FORWARD_ONLY</code> and the given fetch direction is not
		'''     <code>FETCH_FORWARD</code> or (2) the given fetch direction is not
		'''     one of the following:
		'''        ResultSet.FETCH_FORWARD,
		'''        ResultSet.FETCH_REVERSE, or
		'''        ResultSet.FETCH_UNKNOWN </exception>
		''' <seealso cref= #getFetchDirection </seealso>
		Public Overridable Property fetchDirection As Integer
			Set(ByVal direction As Integer)
				' Changed the condition checking to the below as there were two
				' conditions that had to be checked
				' 1. RowSet is TYPE_FORWARD_ONLY and direction is not FETCH_FORWARD
				' 2. Direction is not one of the valid values
    
				If ((type = ResultSet.TYPE_FORWARD_ONLY) AndAlso (direction <> ResultSet.FETCH_FORWARD)) OrElse ((direction <> ResultSet.FETCH_FORWARD) AndAlso (direction <> ResultSet.FETCH_REVERSE) AndAlso (direction <> ResultSet.FETCH_UNKNOWN)) Then Throw New SQLException("Invalid Fetch Direction")
				fetchDir = direction
			End Set
			Get
    
				'Added the following code to throw a
				'SQL Exception if the fetchDir is not
				'set properly.Bug id:4914155
    
				' This checking is not necessary!
    
		'        
		'         if((fetchDir != ResultSet.FETCH_FORWARD) &&
		'           (fetchDir != ResultSet.FETCH_REVERSE) &&
		'           (fetchDir != ResultSet.FETCH_UNKNOWN)) {
		'            throw new SQLException("Fetch Direction Invalid");
		'         }
		'         
				Return (fetchDir)
			End Get
		End Property


		''' <summary>
		''' Sets the fetch size for this <code>RowSet</code> object to the given number of
		''' rows.  The fetch size gives a JDBC technology-enabled driver ("JDBC driver")
		''' a hint as to the
		''' number of rows that should be fetched from the database when more rows
		''' are needed for this <code>RowSet</code> object. If the fetch size specified
		''' is zero, the driver ignores the value and is free to make its own best guess
		''' as to what the fetch size should be.
		''' <P>
		''' A <code>RowSet</code> object inherits the default properties of the
		''' <code>ResultSet</code> object from which it got its data.  That
		''' <code>ResultSet</code> object's default fetch size is set by
		''' the <code>Statement</code> object that created it.
		''' <P>
		''' This method applies to a <code>RowSet</code> object only while it is
		''' connected to a database using a JDBC driver.
		''' For connected <code>RowSet</code> implementations such as
		''' <code>JdbcRowSet</code>, this method has a direct and immediate effect
		''' on the underlying JDBC driver.
		''' <P>
		''' A <code>RowSet</code> object may use this method at any time to change
		''' its setting for the fetch size.
		''' <p>
		''' For <code>RowSet</code> implementations such as
		''' <code>CachedRowSet</code>, which operate in a disconnected environment,
		''' the <code>SyncProvider</code> object being used
		''' may leverage the fetch size to poll the data source and
		''' retrieve a number of rows that do not exceed the fetch size and that may
		''' form a subset of the actual rows returned by the original query. This is
		''' an implementation variance determined by the specific <code>SyncProvider</code>
		''' object employed by the disconnected <code>RowSet</code> object.
		''' <P>
		''' </summary>
		''' <param name="rows"> the number of rows to fetch; <code>0</code> to let the
		'''        driver decide what the best fetch size is; must not be less
		'''        than <code>0</code> or more than the maximum number of rows
		'''        allowed for this <code>RowSet</code> object (the number returned
		'''        by a call to the method <seealso cref="#getMaxRows"/>) </param>
		''' <exception cref="SQLException"> if the specified fetch size is less than <code>0</code>
		'''        or more than the limit for the maximum number of rows </exception>
		''' <seealso cref= #getFetchSize </seealso>
		Public Overridable Property fetchSize As Integer
			Set(ByVal rows As Integer)
				'Added this checking as maxRows can be 0 when this function is called
				'maxRows = 0 means rowset can hold any number of rows, os this checking
				' is needed to take care of this condition.
				If maxRows = 0 AndAlso rows >= 0 Then
					fetchSize = rows
					Return
				End If
				If (rows < 0) OrElse (rows > maxRows) Then Throw New SQLException("Invalid fetch size set. Cannot be of " & "value: " & rows)
				fetchSize = rows
			End Set
			Get
				Return fetchSize
			End Get
		End Property


			Return concurrency
		End Function

		'-----------------------------------------------------------------------
		' Parameters
		'-----------------------------------------------------------------------

		''' <summary>
		''' Checks the given index to see whether it is less than <code>1</code> and
		''' throws an <code>SQLException</code> object if it is.
		''' <P>
		''' This method is called by many methods internally; it is never
		''' called by an application directly.
		''' </summary>
		''' <param name="idx"> an <code>int</code> indicating which parameter is to be
		'''     checked; the first parameter is <code>1</code> </param>
		''' <exception cref="SQLException"> if the parameter is less than <code>1</code> </exception>
		Private Sub checkParamIndex(ByVal idx As Integer)
			If (idx < 1) Then Throw New SQLException("Invalid Parameter Index")
		End Sub

		'---------------------------------------------------------------------
		' setter methods for setting the parameters in a <code>RowSet</code> object's command
		'---------------------------------------------------------------------

		''' <summary>
		''' Sets the designated parameter to SQL <code>NULL</code>.
		''' Note that the parameter's SQL type must be specified using one of the
		''' type codes defined in <code>java.sql.Types</code>.  This SQL type is
		''' specified in the second parameter.
		''' <p>
		''' Note that the second parameter tells the DBMS the data type of the value being
		''' set to <code>NULL</code>. Some DBMSs require this information, so it is required
		''' in order to make code more portable.
		''' <P>
		''' The parameter value set by this method is stored internally and
		''' will be supplied as the appropriate parameter in this <code>RowSet</code>
		''' object's command when the method <code>execute</code> is called.
		''' Methods such as <code>execute</code> and <code>populate</code> must be
		''' provided in any class that extends this class and implements one or
		''' more of the standard JSR-114 <code>RowSet</code> interfaces.
		''' <P>
		''' NOTE: <code>JdbcRowSet</code> does not require the <code>populate</code> method
		''' as it is undefined in this class.
		''' <P>
		''' Calls made to the method <code>getParams</code> after this version of
		''' <code>setNull</code>
		''' has been called will return an <code>Object</code> array containing the parameter values that
		''' have been set.  In that array, the element that represents the values
		''' set with this method will itself be an array. The first element of that array
		''' is <code>null</code>.
		''' The second element is the value set for <i>sqlType</i>.
		''' The parameter number is indicated by an element's position in the array
		''' returned by the method <code>getParams</code>,
		''' with the first element being the value for the first placeholder parameter, the
		''' second element being the value for the second placeholder parameter, and so on.
		''' In other words, if the second placeholder parameter is being set to
		''' <code>null</code>, the array containing it will be the second element in
		''' the array returned by <code>getParams</code>.
		''' <P>
		''' Note that because the numbering of elements in an array starts at zero,
		''' the array element that corresponds to placeholder parameter number
		''' <i>parameterIndex</i> is <i>parameterIndex</i> -1.
		''' </summary>
		''' <param name="parameterIndex"> the ordinal number of the placeholder parameter
		'''        in this <code>RowSet</code> object's command that is to be set.
		'''        The first parameter is 1, the second is 2, and so on; must be
		'''        <code>1</code> or greater </param>
		''' <param name="sqlType"> an <code>int</code> that is one of the SQL type codes
		'''        defined in the class <seealso cref="java.sql.Types"/>. If a non-standard
		'''        <i>sqlType</i> is supplied, this method will not throw a
		'''        <code>SQLException</code>. This allows implicit support for
		'''        non-standard SQL types. </param>
		''' <exception cref="SQLException"> if a database access error occurs or the given
		'''        parameter index is out of bounds </exception>
		''' <seealso cref= #getParams </seealso>
		Public Overridable Sub setNull(ByVal parameterIndex As Integer, ByVal sqlType As Integer)
			Dim nullVal As Object()
			checkParamIndex(parameterIndex)

			nullVal = New Object(1){}
			nullVal(0) = Nothing
			nullVal(1) = Convert.ToInt32(sqlType)

		   If params Is Nothing Then Throw New SQLException("Set initParams() before setNull")

			params(Convert.ToInt32(parameterIndex - 1)) = nullVal
		End Sub

		''' <summary>
		''' Sets the designated parameter to SQL <code>NULL</code>.
		''' 
		''' Although this version of the  method <code>setNull</code> is intended
		''' for user-defined
		''' and <code>REF</code> parameters, this method may be used to set a null
		''' parameter for any JDBC type. The following are user-defined types:
		''' <code>STRUCT</code>, <code>DISTINCT</code>, and <code>JAVA_OBJECT</code>,
		''' and named array types.
		''' 
		''' <P><B>Note:</B> To be portable, applications must give the
		''' SQL type code and the fully qualified SQL type name when specifying
		''' a <code>NULL</code> user-defined or <code>REF</code> parameter.
		''' In the case of a user-defined type, the name is the type name of
		''' the parameter itself.  For a <code>REF</code> parameter, the name is
		''' the type name of the referenced type.  If a JDBC technology-enabled
		''' driver does not need the type code or type name information,
		''' it may ignore it.
		''' <P>
		''' If the parameter does not have a user-defined or <code>REF</code> type,
		''' the given <code>typeName</code> parameter is ignored.
		''' <P>
		''' The parameter value set by this method is stored internally and
		''' will be supplied as the appropriate parameter in this <code>RowSet</code>
		''' object's command when the method <code>execute</code> is called.
		''' Methods such as <code>execute</code> and <code>populate</code> must be
		''' provided in any class that extends this class and implements one or
		''' more of the standard JSR-114 <code>RowSet</code> interfaces.
		''' <P>
		''' NOTE: <code>JdbcRowSet</code> does not require the <code>populate</code> method
		''' as it is undefined in this class.
		''' <P>
		''' Calls made to the method <code>getParams</code> after this version of
		''' <code>setNull</code>
		''' has been called will return an <code>Object</code> array containing the parameter values that
		''' have been set.  In that array, the element that represents the values
		''' set with this method will itself be an array. The first element of that array
		''' is <code>null</code>.
		''' The second element is the value set for <i>sqlType</i>, and the third
		''' element is the value set for <i>typeName</i>.
		''' The parameter number is indicated by an element's position in the array
		''' returned by the method <code>getParams</code>,
		''' with the first element being the value for the first placeholder parameter, the
		''' second element being the value for the second placeholder parameter, and so on.
		''' In other words, if the second placeholder parameter is being set to
		''' <code>null</code>, the array containing it will be the second element in
		''' the array returned by <code>getParams</code>.
		''' <P>
		''' Note that because the numbering of elements in an array starts at zero,
		''' the array element that corresponds to placeholder parameter number
		''' <i>parameterIndex</i> is <i>parameterIndex</i> -1.
		''' </summary>
		''' <param name="parameterIndex"> the ordinal number of the placeholder parameter
		'''        in this <code>RowSet</code> object's command that is to be set.
		'''        The first parameter is 1, the second is 2, and so on; must be
		'''        <code>1</code> or greater </param>
		''' <param name="sqlType"> a value from <code>java.sql.Types</code> </param>
		''' <param name="typeName"> the fully qualified name of an SQL user-defined type,
		'''                 which is ignored if the parameter is not a user-defined
		'''                 type or <code>REF</code> value </param>
		''' <exception cref="SQLException"> if an error occurs or the given parameter index
		'''            is out of bounds </exception>
		''' <seealso cref= #getParams </seealso>
		Public Overridable Sub setNull(ByVal parameterIndex As Integer, ByVal sqlType As Integer, ByVal typeName As String)

			Dim nullVal As Object()
			checkParamIndex(parameterIndex)

			nullVal = New Object(2){}
			nullVal(0) = Nothing
			nullVal(1) = Convert.ToInt32(sqlType)
			nullVal(2) = typeName

		   If params Is Nothing Then Throw New SQLException("Set initParams() before setNull")

			params(Convert.ToInt32(parameterIndex - 1)) = nullVal
		End Sub


		''' <summary>
		''' Sets the designated parameter to the given <code>boolean</code> in the
		''' Java programming language.  The driver converts this to an SQL
		''' <code>BIT</code> value when it sends it to the database.
		''' <P>
		''' The parameter value set by this method is stored internally and
		''' will be supplied as the appropriate parameter in this <code>RowSet</code>
		''' object's command when the method <code>execute</code> is called.
		''' Methods such as <code>execute</code>, <code>populate</code> must be
		''' provided in any class that extends this class and implements one or
		''' more of the standard JSR-114 <code>RowSet</code> interfaces.
		''' <p>
		''' NOTE: <code>JdbcRowSet</code> does not require the <code>populate</code> method
		''' as it is undefined in this class.
		''' </summary>
		''' <param name="parameterIndex"> the ordinal number of the placeholder parameter
		'''        in this <code>RowSet</code> object's command that is to be set.
		'''        The first parameter is 1, the second is 2, and so on; must be
		'''        <code>1</code> or greater </param>
		''' <param name="x"> the parameter value </param>
		''' <exception cref="SQLException"> if an error occurs or the
		'''                         parameter index is out of bounds </exception>
		''' <seealso cref= #getParams </seealso>
		Public Overridable Sub setBoolean(ByVal parameterIndex As Integer, ByVal x As Boolean)
			checkParamIndex(parameterIndex)

		   If params Is Nothing Then Throw New SQLException("Set initParams() before setNull")

			params(Convert.ToInt32(parameterIndex - 1)) = Convert.ToBoolean(x)
		End Sub

		''' <summary>
		''' Sets the designated parameter to the given <code>byte</code> in the Java
		''' programming language.  The driver converts this to an SQL
		''' <code>TINYINT</code> value when it sends it to the database.
		''' <P>
		''' The parameter value set by this method is stored internally and
		''' will be supplied as the appropriate parameter in this <code>RowSet</code>
		''' object's command when the method <code>execute</code> is called.
		''' Methods such as <code>execute</code> and <code>populate</code> must be
		''' provided in any class that extends this class and implements one or
		''' more of the standard JSR-114 <code>RowSet</code> interfaces.
		''' <p>
		''' NOTE: <code>JdbcRowSet</code> does not require the <code>populate</code> method
		''' as it is undefined in this class.
		''' </summary>
		''' <param name="parameterIndex"> the ordinal number of the placeholder parameter
		'''        in this <code>RowSet</code> object's command that is to be set.
		'''        The first parameter is 1, the second is 2, and so on; must be
		'''        <code>1</code> or greater </param>
		''' <param name="x"> the parameter value </param>
		''' <exception cref="SQLException"> if an error occurs or the
		'''                         parameter index is out of bounds </exception>
		''' <seealso cref= #getParams </seealso>
		Public Overridable Sub setByte(ByVal parameterIndex As Integer, ByVal x As SByte)
			checkParamIndex(parameterIndex)

		   If params Is Nothing Then Throw New SQLException("Set initParams() before setByte")

			params(Convert.ToInt32(parameterIndex - 1)) = Convert.ToByte(x)
		End Sub

		''' <summary>
		''' Sets the designated parameter to the given <code>short</code> in the
		''' Java programming language.  The driver converts this to an SQL
		''' <code>SMALLINT</code> value when it sends it to the database.
		''' <P>
		''' The parameter value set by this method is stored internally and
		''' will be supplied as the appropriate parameter in this <code>RowSet</code>
		''' object's command when the method <code>execute</code> is called.
		''' Methods such as <code>execute</code> and <code>populate</code> must be
		''' provided in any class that extends this class and implements one or
		''' more of the standard JSR-114 <code>RowSet</code> interfaces.
		''' <p>
		''' NOTE: <code>JdbcRowSet</code> does not require the <code>populate</code> method
		''' as it is undefined in this class.
		''' <p> </summary>
		''' <param name="parameterIndex"> the ordinal number of the placeholder parameter
		'''        in this <code>RowSet</code> object's command that is to be set.
		'''        The first parameter is 1, the second is 2, and so on; must be
		'''        <code>1</code> or greater </param>
		''' <param name="x"> the parameter value </param>
		''' <exception cref="SQLException"> if an error occurs or the
		'''                         parameter index is out of bounds </exception>
		''' <seealso cref= #getParams </seealso>
		Public Overridable Sub setShort(ByVal parameterIndex As Integer, ByVal x As Short)
			checkParamIndex(parameterIndex)

			If params Is Nothing Then Throw New SQLException("Set initParams() before setShort")

			params(Convert.ToInt32(parameterIndex - 1)) = Convert.ToInt16(x)
		End Sub

		''' <summary>
		''' Sets the designated parameter to an <code>int</code> in the Java
		''' programming language.  The driver converts this to an SQL
		''' <code>INTEGER</code> value when it sends it to the database.
		''' <P>
		''' The parameter value set by this method is stored internally and
		''' will be supplied as the appropriate parameter in this <code>RowSet</code>
		''' object's command when the method <code>execute</code> is called.
		''' Methods such as <code>execute</code> and <code>populate</code> must be
		''' provided in any class that extends this class and implements one or
		''' more of the standard JSR-114 <code>RowSet</code> interfaces.
		''' <P>
		''' NOTE: <code>JdbcRowSet</code> does not require the <code>populate</code> method
		''' as it is undefined in this class.
		''' </summary>
		''' <param name="parameterIndex"> the ordinal number of the placeholder parameter
		'''        in this <code>RowSet</code> object's command that is to be set.
		'''        The first parameter is 1, the second is 2, and so on; must be
		'''        <code>1</code> or greater </param>
		''' <param name="x"> the parameter value </param>
		''' <exception cref="SQLException"> if an error occurs or the
		'''                         parameter index is out of bounds </exception>
		''' <seealso cref= #getParams </seealso>
		Public Overridable Sub setInt(ByVal parameterIndex As Integer, ByVal x As Integer)
			checkParamIndex(parameterIndex)
			If params Is Nothing Then Throw New SQLException("Set initParams() before setInt")
			params(Convert.ToInt32(parameterIndex - 1)) = Convert.ToInt32(x)
		End Sub

		''' <summary>
		''' Sets the designated parameter to the given <code>long</code> in the Java
		''' programming language.  The driver converts this to an SQL
		''' <code>BIGINT</code> value when it sends it to the database.
		''' <P>
		''' The parameter value set by this method is stored internally and
		''' will be supplied as the appropriate parameter in this <code>RowSet</code>
		''' object's command when the method <code>execute</code> is called.
		''' Methods such as <code>execute</code> and <code>populate</code> must be
		''' provided in any class that extends this class and implements one or
		''' more of the standard JSR-114 <code>RowSet</code> interfaces.
		''' <P>
		''' NOTE: <code>JdbcRowSet</code> does not require the <code>populate</code> method
		''' as it is undefined in this class.
		''' </summary>
		''' <param name="parameterIndex"> the ordinal number of the placeholder parameter
		'''        in this <code>RowSet</code> object's command that is to be set.
		'''        The first parameter is 1, the second is 2, and so on; must be
		'''        <code>1</code> or greater </param>
		''' <param name="x"> the parameter value </param>
		''' <exception cref="SQLException"> if an error occurs or the
		'''                         parameter index is out of bounds </exception>
		''' <seealso cref= #getParams </seealso>
		Public Overridable Sub setLong(ByVal parameterIndex As Integer, ByVal x As Long)
			checkParamIndex(parameterIndex)
			If params Is Nothing Then Throw New SQLException("Set initParams() before setLong")
			params(Convert.ToInt32(parameterIndex - 1)) = Convert.ToInt64(x)
		End Sub

		''' <summary>
		''' Sets the designated parameter to the given <code>float</code> in the
		''' Java programming language.  The driver converts this to an SQL
		''' <code>FLOAT</code> value when it sends it to the database.
		''' <P>
		''' The parameter value set by this method is stored internally and
		''' will be supplied as the appropriate parameter in this <code>RowSet</code>
		''' object's command when the method <code>execute</code> is called.
		''' Methods such as <code>execute</code> and <code>populate</code> must be
		''' provided in any class that extends this class and implements one or
		''' more of the standard JSR-114 <code>RowSet</code> interfaces.
		''' <P>
		''' NOTE: <code>JdbcRowSet</code> does not require the <code>populate</code> method
		''' as it is undefined in this class.
		''' </summary>
		''' <param name="parameterIndex"> the ordinal number of the placeholder parameter
		'''        in this <code>RowSet</code> object's command that is to be set.
		'''        The first parameter is 1, the second is 2, and so on; must be
		'''        <code>1</code> or greater </param>
		''' <param name="x"> the parameter value </param>
		''' <exception cref="SQLException"> if an error occurs or the
		'''                         parameter index is out of bounds </exception>
		''' <seealso cref= #getParams </seealso>
		Public Overridable Sub setFloat(ByVal parameterIndex As Integer, ByVal x As Single)
			checkParamIndex(parameterIndex)
			If params Is Nothing Then Throw New SQLException("Set initParams() before setFloat")
			params(Convert.ToInt32(parameterIndex - 1)) = Convert.ToSingle(x)
		End Sub

		''' <summary>
		''' Sets the designated parameter to the given <code>double</code> in the
		''' Java programming language.  The driver converts this to an SQL
		''' <code>DOUBLE</code> value when it sends it to the database.
		''' <P>
		''' The parameter value set by this method is stored internally and
		''' will be supplied as the appropriate parameter in this <code>RowSet</code>
		''' object's command when the method <code>execute</code> is called.
		''' Methods such as <code>execute</code> and <code>populate</code> must be
		''' provided in any class that extends this class and implements one or
		''' more of the standard JSR-114 <code>RowSet</code> interfaces.
		''' <P>
		''' NOTE: <code>JdbcRowSet</code> does not require the <code>populate</code> method
		''' as it is undefined in this class.
		''' S </summary>
		''' <param name="parameterIndex"> the ordinal number of the placeholder parameter
		'''        in this <code>RowSet</code> object's command that is to be set.
		'''        The first parameter is 1, the second is 2, and so on; must be
		'''        <code>1</code> or greater </param>
		''' <param name="x"> the parameter value </param>
		''' <exception cref="SQLException"> if an error occurs or the
		'''                         parameter index is out of bounds </exception>
		''' <seealso cref= #getParams </seealso>
		Public Overridable Sub setDouble(ByVal parameterIndex As Integer, ByVal x As Double)
			checkParamIndex(parameterIndex)
			If params Is Nothing Then Throw New SQLException("Set initParams() before setDouble")
			params(Convert.ToInt32(parameterIndex - 1)) = Convert.ToDouble(x)
		End Sub

		''' <summary>
		''' Sets the designated parameter to the given
		''' <code>java.lang.BigDecimal</code> value.  The driver converts this to
		''' an SQL <code>NUMERIC</code> value when it sends it to the database.
		''' <P>
		''' The parameter value set by this method is stored internally and
		''' will be supplied as the appropriate parameter in this <code>RowSet</code>
		''' object's command when the method <code>execute</code> is called.
		''' Methods such as <code>execute</code> and <code>populate</code> must be
		''' provided in any class that extends this class and implements one or
		''' more of the standard JSR-114 <code>RowSet</code> interfaces.
		''' <P>
		''' Note: <code>JdbcRowSet</code> does not require the <code>populate</code> method
		''' as it is undefined in this class.
		''' </summary>
		''' <param name="parameterIndex"> the ordinal number of the placeholder parameter
		'''        in this <code>RowSet</code> object's command that is to be set.
		'''        The first parameter is 1, the second is 2, and so on; must be
		'''        <code>1</code> or greater </param>
		''' <param name="x"> the parameter value </param>
		''' <exception cref="SQLException"> if an error occurs or the
		'''                         parameter index is out of bounds </exception>
		''' <seealso cref= #getParams </seealso>
		Public Overridable Sub setBigDecimal(ByVal parameterIndex As Integer, ByVal x As Decimal)
			checkParamIndex(parameterIndex)
			If params Is Nothing Then Throw New SQLException("Set initParams() before setBigDecimal")
			params(Convert.ToInt32(parameterIndex - 1)) = x
		End Sub

		''' <summary>
		''' Sets the designated parameter to the given <code>String</code>
		''' value.  The driver converts this to an SQL
		''' <code>VARCHAR</code> or <code>LONGVARCHAR</code> value
		''' (depending on the argument's size relative to the driver's limits
		''' on <code>VARCHAR</code> values) when it sends it to the database.
		''' <P>
		''' The parameter value set by this method is stored internally and
		''' will be supplied as the appropriate parameter in this <code>RowSet</code>
		''' object's command when the method <code>execute</code> is called.
		''' Methods such as <code>execute</code> and <code>populate</code> must be
		''' provided in any class that extends this class and implements one or
		''' more of the standard JSR-114 <code>RowSet</code> interfaces.
		''' <p>
		''' NOTE: <code>JdbcRowSet</code> does not require the <code>populate</code> method
		''' as it is undefined in this class.
		''' <p> </summary>
		''' <param name="parameterIndex"> the ordinal number of the placeholder parameter
		'''        in this <code>RowSet</code> object's command that is to be set.
		'''        The first parameter is 1, the second is 2, and so on; must be
		'''        <code>1</code> or greater </param>
		''' <param name="x"> the parameter value </param>
		''' <exception cref="SQLException"> if an error occurs or the
		'''                         parameter index is out of bounds </exception>
		''' <seealso cref= #getParams </seealso>
		Public Overridable Sub setString(ByVal parameterIndex As Integer, ByVal x As String)
			checkParamIndex(parameterIndex)
			If params Is Nothing Then Throw New SQLException("Set initParams() before setString")
			params(Convert.ToInt32(parameterIndex - 1)) = x
		End Sub

		''' <summary>
		''' Sets the designated parameter to the given array of bytes.
		''' The driver converts this to an SQL
		''' <code>VARBINARY</code> or <code>LONGVARBINARY</code> value
		''' (depending on the argument's size relative to the driver's limits
		''' on <code>VARBINARY</code> values) when it sends it to the database.
		''' <P>
		''' The parameter value set by this method is stored internally and
		''' will be supplied as the appropriate parameter in this <code>RowSet</code>
		''' object's command when the method <code>execute</code> is called.
		''' Methods such as <code>execute</code> and <code>populate</code> must be
		''' provided in any class that extends this class and implements one or
		''' more of the standard JSR-114 <code>RowSet</code> interfaces.
		''' <p>
		''' NOTE: <code>JdbcRowSet</code> does not require the <code>populate</code> method
		''' as it is undefined in this class.
		''' </summary>
		''' <param name="parameterIndex"> the ordinal number of the placeholder parameter
		'''        in this <code>RowSet</code> object's command that is to be set.
		'''        The first parameter is 1, the second is 2, and so on; must be
		'''        <code>1</code> or greater </param>
		''' <param name="x"> the parameter value </param>
		''' <exception cref="SQLException"> if an error occurs or the
		'''                         parameter index is out of bounds </exception>
		''' <seealso cref= #getParams </seealso>
		Public Overridable Sub setBytes(ByVal parameterIndex As Integer, ByVal x As SByte())
			checkParamIndex(parameterIndex)
			If params Is Nothing Then Throw New SQLException("Set initParams() before setBytes")
			params(Convert.ToInt32(parameterIndex - 1)) = x
		End Sub

		''' <summary>
		''' Sets the designated parameter to the given <code>java.sql.Date</code>
		''' value. The driver converts this to an SQL
		''' <code>DATE</code> value when it sends it to the database.
		''' <P>
		''' The parameter value set by this method is stored internally and
		''' will be supplied as the appropriate parameter in this <code>RowSet</code>
		''' object's command when the method <code>execute</code> is called.
		''' Methods such as <code>execute</code> and <code>populate</code> must be
		''' provided in any class that extends this class and implements one or
		''' more of the standard JSR-114 <code>RowSet</code> interfaces.
		''' <P>
		''' NOTE: <code>JdbcRowSet</code> does not require the <code>populate</code> method
		''' as it is undefined in this class.
		''' <P>
		''' Calls made to the method <code>getParams</code> after this version
		''' of <code>setDate</code>
		''' has been called will return an array with the value to be set for
		''' placeholder parameter number <i>parameterIndex</i> being the <code>Date</code>
		''' object supplied as the second parameter.
		''' Note that because the numbering of elements in an array starts at zero,
		''' the array element that corresponds to placeholder parameter number
		''' <i>parameterIndex</i> is <i>parameterIndex</i> -1.
		''' </summary>
		''' <param name="parameterIndex"> the ordinal number of the placeholder parameter
		'''        in this <code>RowSet</code> object's command that is to be set.
		'''        The first parameter is 1, the second is 2, and so on; must be
		'''        <code>1</code> or greater </param>
		''' <param name="x"> the parameter value </param>
		''' <exception cref="SQLException"> if an error occurs or the
		'''                         parameter index is out of bounds </exception>
		''' <seealso cref= #getParams </seealso>
		Public Overridable Sub setDate(ByVal parameterIndex As Integer, ByVal x As java.sql.Date)
			checkParamIndex(parameterIndex)

			If params Is Nothing Then Throw New SQLException("Set initParams() before setDate")
			params(Convert.ToInt32(parameterIndex - 1)) = x
		End Sub

		''' <summary>
		''' Sets the designated parameter to the given <code>java.sql.Time</code>
		''' value.  The driver converts this to an SQL <code>TIME</code> value
		''' when it sends it to the database.
		''' <P>
		''' The parameter value set by this method is stored internally and
		''' will be supplied as the appropriate parameter in this <code>RowSet</code>
		''' object's command when the method <code>execute</code> is called.
		''' Methods such as <code>execute</code> and <code>populate</code> must be
		''' provided in any class that extends this class and implements one or
		''' more of the standard JSR-114 <code>RowSet</code> interfaces.
		''' <P>
		''' NOTE: <code>JdbcRowSet</code> does not require the <code>populate</code> method
		''' as it is undefined in this class.
		''' <P>
		''' Calls made to the method <code>getParams</code> after this version
		''' of the method <code>setTime</code>
		''' has been called will return an array of the parameters that have been set.
		''' The parameter to be set for parameter placeholder number <i>parameterIndex</i>
		''' will be the <code>Time</code> object that was set as the second parameter
		''' to this method.
		''' <P>
		''' Note that because the numbering of elements in an array starts at zero,
		''' the array element that corresponds to placeholder parameter number
		''' <i>parameterIndex</i> is <i>parameterIndex</i> -1.
		''' </summary>
		''' <param name="parameterIndex"> the ordinal number of the placeholder parameter
		'''        in this <code>RowSet</code> object's command that is to be set.
		'''        The first parameter is 1, the second is 2, and so on; must be
		'''        <code>1</code> or greater </param>
		''' <param name="x"> a <code>java.sql.Time</code> object, which is to be set as the value
		'''              for placeholder parameter <i>parameterIndex</i> </param>
		''' <exception cref="SQLException"> if an error occurs or the
		'''                         parameter index is out of bounds </exception>
		''' <seealso cref= #getParams </seealso>
		Public Overridable Sub setTime(ByVal parameterIndex As Integer, ByVal x As java.sql.Time)
			checkParamIndex(parameterIndex)
			If params Is Nothing Then Throw New SQLException("Set initParams() before setTime")

			params(Convert.ToInt32(parameterIndex - 1)) = x
		End Sub

		''' <summary>
		''' Sets the designated parameter to the given
		''' <code>java.sql.Timestamp</code> value.
		''' The driver converts this to an SQL <code>TIMESTAMP</code> value when it
		''' sends it to the database.
		''' <P>
		''' The parameter value set by this method is stored internally and
		''' will be supplied as the appropriate parameter in this <code>RowSet</code>
		''' object's command when the method <code>execute</code> is called.
		''' Methods such as <code>execute</code> and <code>populate</code> must be
		''' provided in any class that extends this class and implements one or
		''' more of the standard JSR-114 <code>RowSet</code> interfaces.
		''' <P>
		''' NOTE: <code>JdbcRowSet</code> does not require the <code>populate</code> method
		''' as it is undefined in this class.
		''' <P>
		''' Calls made to the method <code>getParams</code> after this version of
		''' <code>setTimestamp</code>
		''' has been called will return an array with the value for parameter placeholder
		''' number <i>parameterIndex</i> being the <code>Timestamp</code> object that was
		''' supplied as the second parameter to this method.
		''' Note that because the numbering of elements in an array starts at zero,
		''' the array element that corresponds to placeholder parameter number
		''' <i>parameterIndex</i> is <i>parameterIndex</i> -1.
		''' </summary>
		''' <param name="parameterIndex"> the ordinal number of the placeholder parameter
		'''        in this <code>RowSet</code> object's command that is to be set.
		'''        The first parameter is 1, the second is 2, and so on; must be
		'''        <code>1</code> or greater </param>
		''' <param name="x"> a <code>java.sql.Timestamp</code> object </param>
		''' <exception cref="SQLException"> if an error occurs or the
		'''                         parameter index is out of bounds </exception>
		''' <seealso cref= #getParams </seealso>
		Public Overridable Sub setTimestamp(ByVal parameterIndex As Integer, ByVal x As java.sql.Timestamp)
			checkParamIndex(parameterIndex)
			If params Is Nothing Then Throw New SQLException("Set initParams() before setTimestamp")

			params(Convert.ToInt32(parameterIndex - 1)) = x
		End Sub

		''' <summary>
		''' Sets the designated parameter to the given
		''' <code>java.io.InputStream</code> object,
		''' which will have the specified number of bytes.
		''' The contents of the stream will be read and sent to the database.
		''' This method throws an <code>SQLException</code> object if the number of bytes
		''' read and sent to the database is not equal to <i>length</i>.
		''' <P>
		''' When a very large ASCII value is input to a <code>LONGVARCHAR</code>
		''' parameter, it may be more practical to send it via a
		''' <code>java.io.InputStream</code> object. A JDBC technology-enabled
		''' driver will read the data from the stream as needed until it reaches
		''' end-of-file. The driver will do any necessary conversion from ASCII to
		''' the database <code>CHAR</code> format.
		''' 
		''' <P><B>Note:</B> This stream object can be either a standard
		''' Java stream object or your own subclass that implements the
		''' standard interface.
		''' <P>
		''' The parameter value set by this method is stored internally and
		''' will be supplied as the appropriate parameter in this <code>RowSet</code>
		''' object's command when the method <code>execute</code> is called.
		''' Methods such as <code>execute</code> and <code>populate</code> must be
		''' provided in any class that extends this class and implements one or
		''' more of the standard JSR-114 <code>RowSet</code> interfaces.
		''' <P>
		''' Note: <code>JdbcRowSet</code> does not require the <code>populate</code> method
		''' as it is undefined in this class.
		''' <P>
		''' Calls made to the method <code>getParams</code> after <code>setAsciiStream</code>
		''' has been called will return an array containing the parameter values that
		''' have been set.  The element in the array that represents the values
		''' set with this method will itself be an array. The first element of that array
		''' is the given <code>java.io.InputStream</code> object.
		''' The second element is the value set for <i>length</i>.
		''' The third element is an internal <code>BaseRowSet</code> constant
		''' specifying that the stream passed to this method is an ASCII stream.
		''' The parameter number is indicated by an element's position in the array
		''' returned by the method <code>getParams</code>,
		''' with the first element being the value for the first placeholder parameter, the
		''' second element being the value for the second placeholder parameter, and so on.
		''' In other words, if the input stream being set is the value for the second
		''' placeholder parameter, the array containing it will be the second element in
		''' the array returned by <code>getParams</code>.
		''' <P>
		''' Note that because the numbering of elements in an array starts at zero,
		''' the array element that corresponds to placeholder parameter number
		''' <i>parameterIndex</i> is element number <i>parameterIndex</i> -1.
		''' </summary>
		''' <param name="parameterIndex"> the ordinal number of the placeholder parameter
		'''        in this <code>RowSet</code> object's command that is to be set.
		'''        The first parameter is 1, the second is 2, and so on; must be
		'''        <code>1</code> or greater </param>
		''' <param name="x"> the Java input stream that contains the ASCII parameter value </param>
		''' <param name="length"> the number of bytes in the stream. This is the number of bytes
		'''       the driver will send to the DBMS; lengths of 0 or less are
		'''       are undefined but will cause an invalid length exception to be
		'''       thrown in the underlying JDBC driver. </param>
		''' <exception cref="SQLException"> if an error occurs, the parameter index is out of bounds,
		'''       or when connected to a data source, the number of bytes the driver reads
		'''       and sends to the database is not equal to the number of bytes specified
		'''       in <i>length</i> </exception>
		''' <seealso cref= #getParams </seealso>
		Public Overridable Sub setAsciiStream(ByVal parameterIndex As Integer, ByVal x As java.io.InputStream, ByVal length As Integer)
			Dim ___asciiStream As Object()
			checkParamIndex(parameterIndex)

			___asciiStream = New Object(2){}
			___asciiStream(0) = x
			___asciiStream(1) = Convert.ToInt32(length)
			___asciiStream(2) = Convert.ToInt32(ASCII_STREAM_PARAM)

			If params Is Nothing Then Throw New SQLException("Set initParams() before setAsciiStream")

			params(Convert.ToInt32(parameterIndex - 1)) = ___asciiStream
		End Sub

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
	  Public Overridable Sub setAsciiStream(ByVal parameterIndex As Integer, ByVal x As java.io.InputStream)
		  Throw New SQLFeatureNotSupportedException("Feature not supported")
	  End Sub

		''' <summary>
		''' Sets the designated parameter to the given <code>java.io.InputStream</code>
		''' object, which will have the specified number of bytes.
		''' The contents of the stream will be read and sent to the database.
		''' This method throws an <code>SQLException</code> object if the number of bytes
		''' read and sent to the database is not equal to <i>length</i>.
		''' <P>
		''' When a very large binary value is input to a
		''' <code>LONGVARBINARY</code> parameter, it may be more practical
		''' to send it via a <code>java.io.InputStream</code> object.
		''' A JDBC technology-enabled driver will read the data from the
		''' stream as needed until it reaches end-of-file.
		''' 
		''' <P><B>Note:</B> This stream object can be either a standard
		''' Java stream object or your own subclass that implements the
		''' standard interface.
		''' <P>
		''' The parameter value set by this method is stored internally and
		''' will be supplied as the appropriate parameter in this <code>RowSet</code>
		''' object's command when the method <code>execute</code> is called.
		''' Methods such as <code>execute</code> and <code>populate</code> must be
		''' provided in any class that extends this class and implements one or
		''' more of the standard JSR-114 <code>RowSet</code> interfaces.
		''' <P>
		''' NOTE: <code>JdbcRowSet</code> does not require the <code>populate</code> method
		''' as it is undefined in this class.
		''' <P>
		''' Calls made to the method <code>getParams</code> after <code>setBinaryStream</code>
		''' has been called will return an array containing the parameter values that
		''' have been set.  In that array, the element that represents the values
		''' set with this method will itself be an array. The first element of that array
		''' is the given <code>java.io.InputStream</code> object.
		''' The second element is the value set for <i>length</i>.
		''' The third element is an internal <code>BaseRowSet</code> constant
		''' specifying that the stream passed to this method is a binary stream.
		''' The parameter number is indicated by an element's position in the array
		''' returned by the method <code>getParams</code>,
		''' with the first element being the value for the first placeholder parameter, the
		''' second element being the value for the second placeholder parameter, and so on.
		''' In other words, if the input stream being set is the value for the second
		''' placeholder parameter, the array containing it will be the second element in
		''' the array returned by <code>getParams</code>.
		''' <P>
		''' Note that because the numbering of elements in an array starts at zero,
		''' the array element that corresponds to placeholder parameter number
		''' <i>parameterIndex</i> is element number <i>parameterIndex</i> -1.
		''' </summary>
		''' <param name="parameterIndex"> the ordinal number of the placeholder parameter
		'''        in this <code>RowSet</code> object's command that is to be set.
		'''        The first parameter is 1, the second is 2, and so on; must be
		'''        <code>1</code> or greater </param>
		''' <param name="x"> the input stream that contains the binary value to be set </param>
		''' <param name="length"> the number of bytes in the stream; lengths of 0 or less are
		'''         are undefined but will cause an invalid length exception to be
		'''         thrown in the underlying JDBC driver. </param>
		''' <exception cref="SQLException"> if an error occurs, the parameter index is out of bounds,
		'''         or when connected to a data source, the number of bytes the driver
		'''         reads and sends to the database is not equal to the number of bytes
		'''         specified in <i>length</i> </exception>
		''' <seealso cref= #getParams </seealso>
		Public Overridable Sub setBinaryStream(ByVal parameterIndex As Integer, ByVal x As java.io.InputStream, ByVal length As Integer)
			Dim ___binaryStream As Object()
			checkParamIndex(parameterIndex)

			___binaryStream = New Object(2){}
			___binaryStream(0) = x
			___binaryStream(1) = Convert.ToInt32(length)
			___binaryStream(2) = Convert.ToInt32(BINARY_STREAM_PARAM)
			If params Is Nothing Then Throw New SQLException("Set initParams() before setBinaryStream")

			params(Convert.ToInt32(parameterIndex - 1)) = ___binaryStream
		End Sub


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
	  Public Overridable Sub setBinaryStream(ByVal parameterIndex As Integer, ByVal x As java.io.InputStream)
		  Throw New SQLFeatureNotSupportedException("Feature not supported")
	  End Sub


		''' <summary>
		''' Sets the designated parameter to the given
		''' <code>java.io.InputStream</code> object, which will have the specified
		''' number of bytes. The contents of the stream will be read and sent
		''' to the database.
		''' This method throws an <code>SQLException</code> if the number of bytes
		''' read and sent to the database is not equal to <i>length</i>.
		''' <P>
		''' When a very large Unicode value is input to a
		''' <code>LONGVARCHAR</code> parameter, it may be more practical
		''' to send it via a <code>java.io.InputStream</code> object.
		''' A JDBC technology-enabled driver will read the data from the
		''' stream as needed, until it reaches end-of-file.
		''' The driver will do any necessary conversion from Unicode to the
		''' database <code>CHAR</code> format.
		''' The byte format of the Unicode stream must be Java UTF-8, as
		''' defined in the Java Virtual Machine Specification.
		''' 
		''' <P><B>Note:</B> This stream object can be either a standard
		''' Java stream object or your own subclass that implements the
		''' standard interface.
		''' <P>
		''' This method is deprecated; the method <code>getCharacterStream</code>
		''' should be used in its place.
		''' <P>
		''' The parameter value set by this method is stored internally and
		''' will be supplied as the appropriate parameter in this <code>RowSet</code>
		''' object's command when the method <code>execute</code> is called.
		''' Calls made to the method <code>getParams</code> after <code>setUnicodeStream</code>
		''' has been called will return an array containing the parameter values that
		''' have been set.  In that array, the element that represents the values
		''' set with this method will itself be an array. The first element of that array
		''' is the given <code>java.io.InputStream</code> object.
		''' The second element is the value set for <i>length</i>.
		''' The third element is an internal <code>BaseRowSet</code> constant
		''' specifying that the stream passed to this method is a Unicode stream.
		''' The parameter number is indicated by an element's position in the array
		''' returned by the method <code>getParams</code>,
		''' with the first element being the value for the first placeholder parameter, the
		''' second element being the value for the second placeholder parameter, and so on.
		''' In other words, if the input stream being set is the value for the second
		''' placeholder parameter, the array containing it will be the second element in
		''' the array returned by <code>getParams</code>.
		''' <P>
		''' Note that because the numbering of elements in an array starts at zero,
		''' the array element that corresponds to placeholder parameter number
		''' <i>parameterIndex</i> is element number <i>parameterIndex</i> -1.
		''' </summary>
		''' <param name="parameterIndex"> the ordinal number of the placeholder parameter
		'''        in this <code>RowSet</code> object's command that is to be set.
		'''        The first parameter is 1, the second is 2, and so on; must be
		'''        <code>1</code> or greater </param>
		''' <param name="x"> the <code>java.io.InputStream</code> object that contains the
		'''          UNICODE parameter value </param>
		''' <param name="length"> the number of bytes in the input stream </param>
		''' <exception cref="SQLException"> if an error occurs, the parameter index is out of bounds,
		'''         or the number of bytes the driver reads and sends to the database is
		'''         not equal to the number of bytes specified in <i>length</i> </exception>
		''' @deprecated getCharacterStream should be used in its place 
		''' <seealso cref= #getParams </seealso>
		<Obsolete("getCharacterStream should be used in its place")> _
		Public Overridable Sub setUnicodeStream(ByVal parameterIndex As Integer, ByVal x As java.io.InputStream, ByVal length As Integer)
			Dim ___unicodeStream As Object()
			checkParamIndex(parameterIndex)

			___unicodeStream = New Object(2){}
			___unicodeStream(0) = x
			___unicodeStream(1) = Convert.ToInt32(length)
			___unicodeStream(2) = Convert.ToInt32(UNICODE_STREAM_PARAM)
			If params Is Nothing Then Throw New SQLException("Set initParams() before setUnicodeStream")
			params(Convert.ToInt32(parameterIndex - 1)) = ___unicodeStream
		End Sub

		''' <summary>
		''' Sets the designated parameter to the given <code>java.io.Reader</code>
		''' object, which will have the specified number of characters. The
		''' contents of the reader will be read and sent to the database.
		''' This method throws an <code>SQLException</code> if the number of bytes
		''' read and sent to the database is not equal to <i>length</i>.
		''' <P>
		''' When a very large Unicode value is input to a
		''' <code>LONGVARCHAR</code> parameter, it may be more practical
		''' to send it via a <code>Reader</code> object.
		''' A JDBC technology-enabled driver will read the data from the
		''' stream as needed until it reaches end-of-file.
		''' The driver will do any necessary conversion from Unicode to the
		''' database <code>CHAR</code> format.
		''' The byte format of the Unicode stream must be Java UTF-8, as
		''' defined in the Java Virtual Machine Specification.
		''' 
		''' <P><B>Note:</B> This stream object can be either a standard
		''' Java stream object or your own subclass that implements the
		''' standard interface.
		''' <P>
		''' The parameter value set by this method is stored internally and
		''' will be supplied as the appropriate parameter in this <code>RowSet</code>
		''' object's command when the method <code>execute</code> is called.
		''' Methods such as <code>execute</code> and <code>populate</code> must be
		''' provided in any class that extends this class and implements one or
		''' more of the standard JSR-114 <code>RowSet</code> interfaces.
		''' <P>
		''' NOTE: <code>JdbcRowSet</code> does not require the <code>populate</code> method
		''' as it is undefined in this class.
		''' <P>
		''' Calls made to the method <code>getParams</code> after
		''' <code>setCharacterStream</code>
		''' has been called will return an array containing the parameter values that
		''' have been set.  In that array, the element that represents the values
		''' set with this method will itself be an array. The first element of that array
		''' is the given <code>java.io.Reader</code> object.
		''' The second element is the value set for <i>length</i>.
		''' The parameter number is indicated by an element's position in the array
		''' returned by the method <code>getParams</code>,
		''' with the first element being the value for the first placeholder parameter, the
		''' second element being the value for the second placeholder parameter, and so on.
		''' In other words, if the reader being set is the value for the second
		''' placeholder parameter, the array containing it will be the second element in
		''' the array returned by <code>getParams</code>.
		''' <P>
		''' Note that because the numbering of elements in an array starts at zero,
		''' the array element that corresponds to placeholder parameter number
		''' <i>parameterIndex</i> is element number <i>parameterIndex</i> -1.
		''' </summary>
		''' <param name="parameterIndex"> the ordinal number of the placeholder parameter
		'''        in this <code>RowSet</code> object's command that is to be set.
		'''        The first parameter is 1, the second is 2, and so on; must be
		'''        <code>1</code> or greater </param>
		''' <param name="reader"> the <code>Reader</code> object that contains the
		'''        Unicode data </param>
		''' <param name="length"> the number of characters in the stream; lengths of 0 or
		'''        less are undefined but will cause an invalid length exception to
		'''        be thrown in the underlying JDBC driver. </param>
		''' <exception cref="SQLException"> if an error occurs, the parameter index is out of bounds,
		'''        or when connected to a data source, the number of bytes the driver
		'''        reads and sends to the database is not equal to the number of bytes
		'''        specified in <i>length</i> </exception>
		''' <seealso cref= #getParams </seealso>
		Public Overridable Sub setCharacterStream(ByVal parameterIndex As Integer, ByVal reader As Reader, ByVal length As Integer)
			Dim charStream As Object()
			checkParamIndex(parameterIndex)

			charStream = New Object(1){}
			charStream(0) = reader
			charStream(1) = Convert.ToInt32(length)
			If params Is Nothing Then Throw New SQLException("Set initParams() before setCharacterStream")
			params(Convert.ToInt32(parameterIndex - 1)) = charStream
		End Sub

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
	  Public Overridable Sub setCharacterStream(ByVal parameterIndex As Integer, ByVal reader As java.io.Reader)
		  Throw New SQLFeatureNotSupportedException("Feature not supported")
	  End Sub

		''' <summary>
		''' Sets the designated parameter to an <code>Object</code> in the Java
		''' programming language. The second parameter must be an
		''' <code>Object</code> type.  For integral values, the
		''' <code>java.lang</code> equivalent
		''' objects should be used. For example, use the class <code>Integer</code>
		''' for an <code>int</code>.
		''' <P>
		''' The driver converts this object to the specified
		''' target SQL type before sending it to the database.
		''' If the object has a custom mapping (is of a class implementing
		''' <code>SQLData</code>), the driver should call the method
		''' <code>SQLData.writeSQL</code> to write the object to the SQL
		''' data stream. If, on the other hand, the object is of a class
		''' implementing <code>Ref</code>, <code>Blob</code>, <code>Clob</code>,
		''' <code>Struct</code>, or <code>Array</code>,
		''' the driver should pass it to the database as a value of the
		''' corresponding SQL type.
		''' 
		''' <p>Note that this method may be used to pass database-
		''' specific abstract data types.
		''' <P>
		''' The parameter value set by this method is stored internally and
		''' will be supplied as the appropriate parameter in this <code>RowSet</code>
		''' object's command when the method <code>execute</code> is called.
		''' Methods such as <code>execute</code> and <code>populate</code> must be
		''' provided in any class that extends this class and implements one or
		''' more of the standard JSR-114 <code>RowSet</code> interfaces.
		''' <P>
		''' NOTE: <code>JdbcRowSet</code> does not require the <code>populate</code> method
		''' as it is undefined in this class.
		''' <P>
		''' Calls made to the method <code>getParams</code> after this version of
		''' <code>setObject</code>
		''' has been called will return an array containing the parameter values that
		''' have been set.  In that array, the element that represents the values
		''' set with this method will itself be an array. The first element of that array
		''' is the given <code>Object</code> instance, and the
		''' second element is the value set for <i>targetSqlType</i>.  The
		''' third element is the value set for <i>scale</i>, which the driver will
		''' ignore if the type of the object being set is not
		''' <code>java.sql.Types.NUMERIC</code> or <code>java.sql.Types.DECIMAL</code>.
		''' The parameter number is indicated by an element's position in the array
		''' returned by the method <code>getParams</code>,
		''' with the first element being the value for the first placeholder parameter, the
		''' second element being the value for the second placeholder parameter, and so on.
		''' In other words, if the object being set is the value for the second
		''' placeholder parameter, the array containing it will be the second element in
		''' the array returned by <code>getParams</code>.
		''' <P>
		''' Note that because the numbering of elements in an array starts at zero,
		''' the array element that corresponds to placeholder parameter number
		''' <i>parameterIndex</i> is element number <i>parameterIndex</i> -1.
		''' 
		''' </summary>
		''' <param name="parameterIndex"> the ordinal number of the placeholder parameter
		'''        in this <code>RowSet</code> object's command that is to be set.
		'''        The first parameter is 1, the second is 2, and so on; must be
		'''        <code>1</code> or greater </param>
		''' <param name="x"> the <code>Object</code> containing the input parameter value;
		'''        must be an <code>Object</code> type </param>
		''' <param name="targetSqlType"> the SQL type (as defined in <code>java.sql.Types</code>)
		'''        to be sent to the database. The <code>scale</code> argument may
		'''        further qualify this type. If a non-standard <i>targetSqlType</i>
		'''        is supplied, this method will not throw a <code>SQLException</code>.
		'''        This allows implicit support for non-standard SQL types. </param>
		''' <param name="scale"> for the types <code>java.sql.Types.DECIMAL</code> and
		'''        <code>java.sql.Types.NUMERIC</code>, this is the number
		'''        of digits after the decimal point.  For all other types, this
		'''        value will be ignored. </param>
		''' <exception cref="SQLException"> if an error occurs or the parameter index is out of bounds </exception>
		''' <seealso cref= #getParams </seealso>
		Public Overridable Sub setObject(ByVal parameterIndex As Integer, ByVal x As Object, ByVal targetSqlType As Integer, ByVal scale As Integer)
			Dim obj As Object()
			checkParamIndex(parameterIndex)

			obj = New Object(2){}
			obj(0) = x
			obj(1) = Convert.ToInt32(targetSqlType)
			obj(2) = Convert.ToInt32(scale)
			If params Is Nothing Then Throw New SQLException("Set initParams() before setObject")
			params(Convert.ToInt32(parameterIndex - 1)) = obj
		End Sub

		''' <summary>
		''' Sets the value of the designated parameter with the given
		''' <code>Object</code> value.
		''' This method is like <code>setObject(int parameterIndex, Object x, int
		''' targetSqlType, int scale)</code> except that it assumes a scale of zero.
		''' <P>
		''' The parameter value set by this method is stored internally and
		''' will be supplied as the appropriate parameter in this <code>RowSet</code>
		''' object's command when the method <code>execute</code> is called.
		''' Methods such as <code>execute</code> and <code>populate</code> must be
		''' provided in any class that extends this class and implements one or
		''' more of the standard JSR-114 <code>RowSet</code> interfaces.
		''' <P>
		''' NOTE: <code>JdbcRowSet</code> does not require the <code>populate</code> method
		''' as it is undefined in this class.
		''' <P>
		''' Calls made to the method <code>getParams</code> after this version of
		''' <code>setObject</code>
		''' has been called will return an array containing the parameter values that
		''' have been set.  In that array, the element that represents the values
		''' set with this method will itself be an array. The first element of that array
		''' is the given <code>Object</code> instance.
		''' The second element is the value set for <i>targetSqlType</i>.
		''' The parameter number is indicated by an element's position in the array
		''' returned by the method <code>getParams</code>,
		''' with the first element being the value for the first placeholder parameter, the
		''' second element being the value for the second placeholder parameter, and so on.
		''' In other words, if the object being set is the value for the second
		''' placeholder parameter, the array containing it will be the second element in
		''' the array returned by <code>getParams</code>.
		''' <P>
		''' Note that because the numbering of elements in an array starts at zero,
		''' the array element that corresponds to placeholder parameter number
		''' <i>parameterIndex</i> is element number <i>parameterIndex</i> -1.
		''' </summary>
		''' <param name="parameterIndex"> the ordinal number of the placeholder parameter
		'''        in this <code>RowSet</code> object's command that is to be set.
		'''        The first parameter is 1, the second is 2, and so on; must be
		'''        <code>1</code> or greater </param>
		''' <param name="x"> the <code>Object</code> containing the input parameter value;
		'''        must be an <code>Object</code> type </param>
		''' <param name="targetSqlType"> the SQL type (as defined in <code>java.sql.Types</code>)
		'''        to be sent to the database. If a non-standard <i>targetSqlType</i>
		'''        is supplied, this method will not throw a <code>SQLException</code>.
		'''        This allows implicit support for non-standard SQL types. </param>
		''' <exception cref="SQLException"> if an error occurs or the parameter index
		'''        is out of bounds </exception>
		''' <seealso cref= #getParams </seealso>
		Public Overridable Sub setObject(ByVal parameterIndex As Integer, ByVal x As Object, ByVal targetSqlType As Integer)
			Dim obj As Object()
			checkParamIndex(parameterIndex)

			obj = New Object(1){}
			obj(0) = x
			obj(1) = Convert.ToInt32(targetSqlType)
			If params Is Nothing Then Throw New SQLException("Set initParams() before setObject")
			params(Convert.ToInt32(parameterIndex - 1)) = obj
		End Sub

		''' <summary>
		''' Sets the designated parameter to an <code>Object</code> in the Java
		''' programming language. The second parameter must be an
		''' <code>Object</code>
		''' type.  For integral values, the <code>java.lang</code> equivalent
		''' objects should be used. For example, use the class <code>Integer</code>
		''' for an <code>int</code>.
		''' <P>
		''' The JDBC specification defines a standard mapping from
		''' Java <code>Object</code> types to SQL types.  The driver will
		''' use this standard mapping to  convert the given object
		''' to its corresponding SQL type before sending it to the database.
		''' If the object has a custom mapping (is of a class implementing
		''' <code>SQLData</code>), the driver should call the method
		''' <code>SQLData.writeSQL</code> to write the object to the SQL
		''' data stream.
		''' <P>
		''' If, on the other hand, the object is of a class
		''' implementing <code>Ref</code>, <code>Blob</code>, <code>Clob</code>,
		''' <code>Struct</code>, or <code>Array</code>,
		''' the driver should pass it to the database as a value of the
		''' corresponding SQL type.
		''' <P>
		''' This method throws an exception if there
		''' is an ambiguity, for example, if the object is of a class
		''' implementing more than one interface.
		''' <P>
		''' Note that this method may be used to pass database-specific
		''' abstract data types.
		''' <P>
		''' The parameter value set by this method is stored internally and
		''' will be supplied as the appropriate parameter in this <code>RowSet</code>
		''' object's command when the method <code>execute</code> is called.
		''' Methods such as <code>execute</code> and <code>populate</code> must be
		''' provided in any class that extends this class and implements one or
		''' more of the standard JSR-114 <code>RowSet</code> interfaces.
		''' <p>
		''' NOTE: <code>JdbcRowSet</code> does not require the <code>populate</code> method
		''' as it is undefined in this class.
		''' <P>
		''' After this method has been called, a call to the
		''' method <code>getParams</code>
		''' will return an object array of the current command parameters, which will
		''' include the <code>Object</code> set for placeholder parameter number
		''' <code>parameterIndex</code>.
		''' Note that because the numbering of elements in an array starts at zero,
		''' the array element that corresponds to placeholder parameter number
		''' <i>parameterIndex</i> is element number <i>parameterIndex</i> -1.
		''' </summary>
		''' <param name="parameterIndex"> the ordinal number of the placeholder parameter
		'''        in this <code>RowSet</code> object's command that is to be set.
		'''        The first parameter is 1, the second is 2, and so on; must be
		'''        <code>1</code> or greater </param>
		''' <param name="x"> the object containing the input parameter value </param>
		''' <exception cref="SQLException"> if an error occurs the
		'''                         parameter index is out of bounds, or there
		'''                         is ambiguity in the implementation of the
		'''                         object being set </exception>
		''' <seealso cref= #getParams </seealso>
		Public Overridable Sub setObject(ByVal parameterIndex As Integer, ByVal x As Object)
			checkParamIndex(parameterIndex)
			If params Is Nothing Then Throw New SQLException("Set initParams() before setObject")
			params(Convert.ToInt32(parameterIndex - 1)) = x
		End Sub

		''' <summary>
		''' Sets the designated parameter to the given <code>Ref</code> object in
		''' the Java programming language.  The driver converts this to an SQL
		''' <code>REF</code> value when it sends it to the database. Internally, the
		''' <code>Ref</code> is represented as a <code>SerialRef</code> to ensure
		''' serializability.
		''' <P>
		''' The parameter value set by this method is stored internally and
		''' will be supplied as the appropriate parameter in this <code>RowSet</code>
		''' object's command when the method <code>execute</code> is called.
		''' Methods such as <code>execute</code> and <code>populate</code> must be
		''' provided in any class that extends this class and implements one or
		''' more of the standard JSR-114 <code>RowSet</code> interfaces.
		''' <p>
		''' NOTE: <code>JdbcRowSet</code> does not require the <code>populate</code> method
		''' as it is undefined in this class.
		''' <p>
		''' After this method has been called, a call to the
		''' method <code>getParams</code>
		''' will return an object array of the current command parameters, which will
		''' include the <code>Ref</code> object set for placeholder parameter number
		''' <code>parameterIndex</code>.
		''' Note that because the numbering of elements in an array starts at zero,
		''' the array element that corresponds to placeholder parameter number
		''' <i>parameterIndex</i> is element number <i>parameterIndex</i> -1.
		''' </summary>
		''' <param name="parameterIndex"> the ordinal number of the placeholder parameter
		'''        in this <code>RowSet</code> object's command that is to be set.
		'''        The first parameter is 1, the second is 2, and so on; must be
		'''        <code>1</code> or greater </param>
		''' <param name="ref"> a <code>Ref</code> object representing an SQL <code>REF</code>
		'''         value; cannot be null </param>
		''' <exception cref="SQLException"> if an error occurs; the parameter index is out of
		'''         bounds or the <code>Ref</code> object is <code>null</code>; or
		'''         the <code>Ref</code> object returns a <code>null</code> base type
		'''         name. </exception>
		''' <seealso cref= #getParams </seealso>
		''' <seealso cref= javax.sql.rowset.serial.SerialRef </seealso>
		Public Overridable Sub setRef(ByVal parameterIndex As Integer, ByVal ref As Ref)
			checkParamIndex(parameterIndex)
			If params Is Nothing Then Throw New SQLException("Set initParams() before setRef")
			params(Convert.ToInt32(parameterIndex - 1)) = New SerialRef(ref)
		End Sub

		''' <summary>
		''' Sets the designated parameter to the given <code>Blob</code> object in
		''' the Java programming language.  The driver converts this to an SQL
		''' <code>BLOB</code> value when it sends it to the database. Internally,
		''' the <code>Blob</code> is represented as a <code>SerialBlob</code>
		''' to ensure serializability.
		''' <P>
		''' The parameter value set by this method is stored internally and
		''' will be supplied as the appropriate parameter in this <code>RowSet</code>
		''' object's command when the method <code>execute</code> is called.
		''' Methods such as <code>execute</code> and <code>populate</code> must be
		''' provided in any class that extends this class and implements one or
		''' more of the standard JSR-114 <code>RowSet</code> interfaces.
		''' NOTE: <code>JdbcRowSet</code> does not require the <code>populate</code> method
		''' as it is undefined in this class.
		''' <p>
		''' After this method has been called, a call to the
		''' method <code>getParams</code>
		''' will return an object array of the current command parameters, which will
		''' include the <code>Blob</code> object set for placeholder parameter number
		''' <code>parameterIndex</code>.
		''' Note that because the numbering of elements in an array starts at zero,
		''' the array element that corresponds to placeholder parameter number
		''' <i>parameterIndex</i> is element number <i>parameterIndex</i> -1.
		''' </summary>
		''' <param name="parameterIndex"> the ordinal number of the placeholder parameter
		'''        in this <code>RowSet</code> object's command that is to be set.
		'''        The first parameter is 1, the second is 2, and so on; must be
		'''        <code>1</code> or greater </param>
		''' <param name="x"> a <code>Blob</code> object representing an SQL
		'''          <code>BLOB</code> value </param>
		''' <exception cref="SQLException"> if an error occurs or the
		'''                         parameter index is out of bounds </exception>
		''' <seealso cref= #getParams </seealso>
		''' <seealso cref= javax.sql.rowset.serial.SerialBlob </seealso>
		Public Overridable Sub setBlob(ByVal parameterIndex As Integer, ByVal x As Blob)
			checkParamIndex(parameterIndex)
			If params Is Nothing Then Throw New SQLException("Set initParams() before setBlob")
			params(Convert.ToInt32(parameterIndex - 1)) = New SerialBlob(x)
		End Sub

		''' <summary>
		''' Sets the designated parameter to the given <code>Clob</code> object in
		''' the Java programming language.  The driver converts this to an SQL
		''' <code>CLOB</code> value when it sends it to the database. Internally, the
		''' <code>Clob</code> is represented as a <code>SerialClob</code> to ensure
		''' serializability.
		''' <P>
		''' The parameter value set by this method is stored internally and
		''' will be supplied as the appropriate parameter in this <code>RowSet</code>
		''' object's command when the method <code>execute</code> is called.
		''' Methods such as <code>execute</code> and <code>populate</code> must be
		''' provided in any class that extends this class and implements one or
		''' more of the standard JSR-114 <code>RowSet</code> interfaces.
		''' <p>
		''' NOTE: <code>JdbcRowSet</code> does not require the <code>populate</code> method
		''' as it is undefined in this class.
		''' <p>
		''' After this method has been called, a call to the
		''' method <code>getParams</code>
		''' will return an object array of the current command parameters, which will
		''' include the <code>Clob</code> object set for placeholder parameter number
		''' <code>parameterIndex</code>.
		''' Note that because the numbering of elements in an array starts at zero,
		''' the array element that corresponds to placeholder parameter number
		''' <i>parameterIndex</i> is element number <i>parameterIndex</i> -1.
		''' </summary>
		''' <param name="parameterIndex"> the ordinal number of the placeholder parameter
		'''     in this <code>RowSet</code> object's command that is to be set.
		'''     The first parameter is 1, the second is 2, and so on; must be
		'''     <code>1</code> or greater </param>
		''' <param name="x"> a <code>Clob</code> object representing an SQL
		'''     <code>CLOB</code> value; cannot be null </param>
		''' <exception cref="SQLException"> if an error occurs; the parameter index is out of
		'''     bounds or the <code>Clob</code> is null </exception>
		''' <seealso cref= #getParams </seealso>
		''' <seealso cref= javax.sql.rowset.serial.SerialBlob </seealso>
		Public Overridable Sub setClob(ByVal parameterIndex As Integer, ByVal x As Clob)
			checkParamIndex(parameterIndex)
			If params Is Nothing Then Throw New SQLException("Set initParams() before setClob")
			params(Convert.ToInt32(parameterIndex - 1)) = New SerialClob(x)
		End Sub

		''' <summary>
		''' Sets the designated parameter to an <code>Array</code> object in the
		''' Java programming language.  The driver converts this to an SQL
		''' <code>ARRAY</code> value when it sends it to the database. Internally,
		''' the <code>Array</code> is represented as a <code>SerialArray</code>
		''' to ensure serializability.
		''' <P>
		''' The parameter value set by this method is stored internally and
		''' will be supplied as the appropriate parameter in this <code>RowSet</code>
		''' object's command when the method <code>execute</code> is called.
		''' Methods such as <code>execute</code> and <code>populate</code> must be
		''' provided in any class that extends this class and implements one or
		''' more of the standard JSR-114 <code>RowSet</code> interfaces.
		''' <P>
		''' Note: <code>JdbcRowSet</code> does not require the <code>populate</code> method
		''' as it is undefined in this class.
		''' <p>
		''' After this method has been called, a call to the
		''' method <code>getParams</code>
		''' will return an object array of the current command parameters, which will
		''' include the <code>Array</code> object set for placeholder parameter number
		''' <code>parameterIndex</code>.
		''' Note that because the numbering of elements in an array starts at zero,
		''' the array element that corresponds to placeholder parameter number
		''' <i>parameterIndex</i> is element number <i>parameterIndex</i> -1.
		''' </summary>
		''' <param name="parameterIndex"> the ordinal number of the placeholder parameter
		'''        in this <code>RowSet</code> object's command that is to be set.
		'''        The first parameter is 1, the second is 2, and so on; must be
		'''        <code>1</code> or greater </param>
		''' <param name="array"> an <code>Array</code> object representing an SQL
		'''        <code>ARRAY</code> value; cannot be null. The <code>Array</code> object
		'''        passed to this method must return a non-null Object for all
		'''        <code>getArray()</code> method calls. A null value will cause a
		'''        <code>SQLException</code> to be thrown. </param>
		''' <exception cref="SQLException"> if an error occurs; the parameter index is out of
		'''        bounds or the <code>ARRAY</code> is null </exception>
		''' <seealso cref= #getParams </seealso>
		''' <seealso cref= javax.sql.rowset.serial.SerialArray </seealso>
		Public Overridable Sub setArray(ByVal parameterIndex As Integer, ByVal array As Array)
			checkParamIndex(parameterIndex)
			If params Is Nothing Then Throw New SQLException("Set initParams() before setArray")
			params(Convert.ToInt32(parameterIndex - 1)) = New SerialArray(array)
		End Sub

		''' <summary>
		''' Sets the designated parameter to the given <code>java.sql.Date</code>
		''' object.
		''' When the DBMS does not store time zone information, the driver will use
		''' the given <code>Calendar</code> object to construct the SQL <code>DATE</code>
		''' value to send to the database. With a
		''' <code>Calendar</code> object, the driver can calculate the date
		''' taking into account a custom time zone.  If no <code>Calendar</code>
		''' object is specified, the driver uses the time zone of the Virtual Machine
		''' that is running the application.
		''' <P>
		''' The parameter value set by this method is stored internally and
		''' will be supplied as the appropriate parameter in this <code>RowSet</code>
		''' object's command when the method <code>execute</code> is called.
		''' Methods such as <code>execute</code> and <code>populate</code> must be
		''' provided in any class that extends this class and implements one or
		''' more of the standard JSR-114 <code>RowSet</code> interfaces.
		''' <P>
		''' NOTE: <code>JdbcRowSet</code> does not require the <code>populate</code> method
		''' as it is undefined in this class.
		''' <P>
		''' Calls made to the method <code>getParams</code> after this version of
		''' <code>setDate</code>
		''' has been called will return an array containing the parameter values that
		''' have been set.  In that array, the element that represents the values
		''' set with this method will itself be an array. The first element of that array
		''' is the given <code>java.sql.Date</code> object.
		''' The second element is the value set for <i>cal</i>.
		''' The parameter number is indicated by an element's position in the array
		''' returned by the method <code>getParams</code>,
		''' with the first element being the value for the first placeholder parameter, the
		''' second element being the value for the second placeholder parameter, and so on.
		''' In other words, if the date being set is the value for the second
		''' placeholder parameter, the array containing it will be the second element in
		''' the array returned by <code>getParams</code>.
		''' <P>
		''' Note that because the numbering of elements in an array starts at zero,
		''' the array element that corresponds to placeholder parameter number
		''' <i>parameterIndex</i> is <i>parameterIndex</i> -1.
		''' </summary>
		''' <param name="parameterIndex"> the ordinal number of the placeholder parameter
		'''        in this <code>RowSet</code> object's command that is to be set.
		'''        The first parameter is 1, the second is 2, and so on; must be
		'''        <code>1</code> or greater </param>
		''' <param name="x"> a <code>java.sql.Date</code> object representing an SQL
		'''        <code>DATE</code> value </param>
		''' <param name="cal"> a <code>java.util.Calendar</code> object to use when
		'''        when constructing the date </param>
		''' <exception cref="SQLException"> if an error occurs or the
		'''                         parameter index is out of bounds </exception>
		''' <seealso cref= #getParams </seealso>
		Public Overridable Sub setDate(ByVal parameterIndex As Integer, ByVal x As java.sql.Date, ByVal cal As DateTime)
			Dim ___date As Object()
			checkParamIndex(parameterIndex)

			___date = New Object(1){}
			___date(0) = x
			___date(1) = cal
			If params Is Nothing Then Throw New SQLException("Set initParams() before setDate")
			params(Convert.ToInt32(parameterIndex - 1)) = ___date
		End Sub

		''' <summary>
		''' Sets the designated parameter to the given <code>java.sql.Time</code>
		''' object.  The driver converts this
		''' to an SQL <code>TIME</code> value when it sends it to the database.
		''' <P>
		''' When the DBMS does not store time zone information, the driver will use
		''' the given <code>Calendar</code> object to construct the SQL <code>TIME</code>
		''' value to send to the database. With a
		''' <code>Calendar</code> object, the driver can calculate the date
		''' taking into account a custom time zone.  If no <code>Calendar</code>
		''' object is specified, the driver uses the time zone of the Virtual Machine
		''' that is running the application.
		''' <P>
		''' The parameter value set by this method is stored internally and
		''' will be supplied as the appropriate parameter in this <code>RowSet</code>
		''' object's command when the method <code>execute</code> is called.
		''' Methods such as <code>execute</code> and <code>populate</code> must be
		''' provided in any class that extends this class and implements one or
		''' more of the standard JSR-114 <code>RowSet</code> interfaces.
		''' <P>
		''' NOTE: <code>JdbcRowSet</code> does not require the <code>populate</code> method
		''' as it is undefined in this class.
		''' <P>
		''' Calls made to the method <code>getParams</code> after this version of
		''' <code>setTime</code>
		''' has been called will return an array containing the parameter values that
		''' have been set.  In that array, the element that represents the values
		''' set with this method will itself be an array. The first element of that array
		''' is the given <code>java.sql.Time</code> object.
		''' The second element is the value set for <i>cal</i>.
		''' The parameter number is indicated by an element's position in the array
		''' returned by the method <code>getParams</code>,
		''' with the first element being the value for the first placeholder parameter, the
		''' second element being the value for the second placeholder parameter, and so on.
		''' In other words, if the time being set is the value for the second
		''' placeholder parameter, the array containing it will be the second element in
		''' the array returned by <code>getParams</code>.
		''' <P>
		''' Note that because the numbering of elements in an array starts at zero,
		''' the array element that corresponds to placeholder parameter number
		''' <i>parameterIndex</i> is <i>parameterIndex</i> -1.
		''' </summary>
		''' <param name="parameterIndex"> the ordinal number of the placeholder parameter
		'''        in this <code>RowSet</code> object's command that is to be set.
		'''        The first parameter is 1, the second is 2, and so on; must be
		'''        <code>1</code> or greater </param>
		''' <param name="x"> a <code>java.sql.Time</code> object </param>
		''' <param name="cal"> the <code>java.util.Calendar</code> object the driver can use to
		'''         construct the time </param>
		''' <exception cref="SQLException"> if an error occurs or the
		'''                         parameter index is out of bounds </exception>
		''' <seealso cref= #getParams </seealso>
		Public Overridable Sub setTime(ByVal parameterIndex As Integer, ByVal x As java.sql.Time, ByVal cal As DateTime)
			Dim ___time As Object()
			checkParamIndex(parameterIndex)

			___time = New Object(1){}
			___time(0) = x
			___time(1) = cal
			If params Is Nothing Then Throw New SQLException("Set initParams() before setTime")
			params(Convert.ToInt32(parameterIndex - 1)) = ___time
		End Sub

		''' <summary>
		''' Sets the designated parameter to the given
		''' <code>java.sql.Timestamp</code> object.  The driver converts this
		''' to an SQL <code>TIMESTAMP</code> value when it sends it to the database.
		''' <P>
		''' When the DBMS does not store time zone information, the driver will use
		''' the given <code>Calendar</code> object to construct the SQL <code>TIMESTAMP</code>
		''' value to send to the database. With a
		''' <code>Calendar</code> object, the driver can calculate the timestamp
		''' taking into account a custom time zone.  If no <code>Calendar</code>
		''' object is specified, the driver uses the time zone of the Virtual Machine
		''' that is running the application.
		''' <P>
		''' The parameter value set by this method is stored internally and
		''' will be supplied as the appropriate parameter in this <code>RowSet</code>
		''' object's command when the method <code>execute</code> is called.
		''' Methods such as <code>execute</code> and <code>populate</code> must be
		''' provided in any class that extends this class and implements one or
		''' more of the standard JSR-114 <code>RowSet</code> interfaces.
		''' <P>
		''' NOTE: <code>JdbcRowSet</code> does not require the <code>populate</code> method
		''' as it is undefined in this class.
		''' <P>
		''' Calls made to the method <code>getParams</code> after this version of
		''' <code>setTimestamp</code>
		''' has been called will return an array containing the parameter values that
		''' have been set.  In that array, the element that represents the values
		''' set with this method will itself be an array. The first element of that array
		''' is the given <code>java.sql.Timestamp</code> object.
		''' The second element is the value set for <i>cal</i>.
		''' The parameter number is indicated by an element's position in the array
		''' returned by the method <code>getParams</code>,
		''' with the first element being the value for the first placeholder parameter, the
		''' second element being the value for the second placeholder parameter, and so on.
		''' In other words, if the timestamp being set is the value for the second
		''' placeholder parameter, the array containing it will be the second element in
		''' the array returned by <code>getParams</code>.
		''' <P>
		''' Note that because the numbering of elements in an array starts at zero,
		''' the array element that corresponds to placeholder parameter number
		''' <i>parameterIndex</i> is <i>parameterIndex</i> -1.
		''' </summary>
		''' <param name="parameterIndex"> the ordinal number of the placeholder parameter
		'''        in this <code>RowSet</code> object's command that is to be set.
		'''        The first parameter is 1, the second is 2, and so on; must be
		'''        <code>1</code> or greater </param>
		''' <param name="x"> a <code>java.sql.Timestamp</code> object </param>
		''' <param name="cal"> the <code>java.util.Calendar</code> object the driver can use to
		'''         construct the timestamp </param>
		''' <exception cref="SQLException"> if an error occurs or the
		'''                         parameter index is out of bounds </exception>
		''' <seealso cref= #getParams </seealso>
		Public Overridable Sub setTimestamp(ByVal parameterIndex As Integer, ByVal x As java.sql.Timestamp, ByVal cal As DateTime)
			Dim ___timestamp As Object()
			checkParamIndex(parameterIndex)

			___timestamp = New Object(1){}
			___timestamp(0) = x
			___timestamp(1) = cal
			If params Is Nothing Then Throw New SQLException("Set initParams() before setTimestamp")
			params(Convert.ToInt32(parameterIndex - 1)) = ___timestamp
		End Sub

		''' <summary>
		''' Clears all of the current parameter values in this <code>RowSet</code>
		''' object's internal representation of the parameters to be set in
		''' this <code>RowSet</code> object's command when it is executed.
		''' <P>
		''' In general, parameter values remain in force for repeated use in
		''' this <code>RowSet</code> object's command. Setting a parameter value with the
		''' setter methods automatically clears the value of the
		''' designated parameter and replaces it with the new specified value.
		''' <P>
		''' This method is called internally by the <code>setCommand</code>
		''' method to clear all of the parameters set for the previous command.
		''' <P>
		''' Furthermore, this method differs from the <code>initParams</code>
		''' method in that it maintains the schema of the <code>RowSet</code> object.
		''' </summary>
		''' <exception cref="SQLException"> if an error occurs clearing the parameters </exception>
		Public Overridable Sub clearParameters()
			params.Clear()
		End Sub

		''' <summary>
		''' Retrieves an array containing the parameter values (both Objects and
		''' primitives) that have been set for this
		''' <code>RowSet</code> object's command and throws an <code>SQLException</code> object
		''' if all parameters have not been set.   Before the command is sent to the
		''' DBMS to be executed, these parameters will be substituted
		''' for placeholder parameters in the  <code>PreparedStatement</code> object
		''' that is the command for a <code>RowSet</code> implementation extending
		''' the <code>BaseRowSet</code> class.
		''' <P>
		''' Each element in the array that is returned is an <code>Object</code> instance
		''' that contains the values of the parameters supplied to a setter method.
		''' The order of the elements is determined by the value supplied for
		''' <i>parameterIndex</i>.  If the setter method takes only the parameter index
		''' and the value to be set (possibly null), the array element will contain the value to be set
		''' (which will be expressed as an <code>Object</code>).  If there are additional
		''' parameters, the array element will itself be an array containing the value to be set
		''' plus any additional parameter values supplied to the setter method. If the method
		''' sets a stream, the array element includes the type of stream being supplied to the
		''' method. These additional parameters are for the use of the driver or the DBMS and may or
		''' may not be used.
		''' <P>
		''' NOTE: Stored parameter values of types <code>Array</code>, <code>Blob</code>,
		''' <code>Clob</code> and <code>Ref</code> are returned as <code>SerialArray</code>,
		''' <code>SerialBlob</code>, <code>SerialClob</code> and <code>SerialRef</code>
		''' respectively.
		''' </summary>
		''' <returns> an array of <code>Object</code> instances that includes the
		'''         parameter values that may be set in this <code>RowSet</code> object's
		'''         command; an empty array if no parameters have been set </returns>
		''' <exception cref="SQLException"> if an error occurs retrieving the object array of
		'''         parameters of this <code>RowSet</code> object or if not all parameters have
		'''         been set </exception>
		Public Overridable Property params As Object()
			Get
				If params Is Nothing Then
    
					initParams()
					Dim paramsArray As Object() = New Object(params.Count - 1){}
					Return paramsArray
    
				Else
					' The parameters may be set in random order
					' but all must be set, check to verify all
					' have been set till the last parameter
					' else throw exception.
    
					Dim paramsArray As Object() = New Object(params.Count - 1){}
					For i As Integer = 0 To params.Count - 1
					   paramsArray(i) = params(Convert.ToInt32(i))
					   If paramsArray(i) Is Nothing Then
						 Throw New SQLException("missing parameter: " & (i + 1))
					   End If 'end if
					Next i 'end for
					Return paramsArray
    
				End If 'end if
    
			End Get
		End Property


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
	   Public Overridable Sub setNull(ByVal parameterName As String, ByVal sqlType As Integer)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
	   End Sub


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
	   Public Overridable Sub setNull(ByVal parameterName As String, ByVal sqlType As Integer, ByVal typeName As String)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
	   End Sub



	 ''' <summary>
	 ''' Sets the designated parameter to the given Java <code>boolean</code> value.
	 ''' The driver converts this
	 ''' to an SQL <code>BIT</code> or <code>BOOLEAN</code> value when it sends it to the database.
	 ''' </summary>
	 ''' <param name="parameterName"> the name of the parameter </param>
	 ''' <param name="x"> the parameter value </param>
	 ''' <exception cref="SQLException"> if a database access error occurs or
	 ''' this method is called on a closed <code>CallableStatement</code> </exception>
	 ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	 ''' this method </exception>
	 ''' <seealso cref= #getParams
	 ''' @since 1.4 </seealso>
	   Public Overridable Sub setBoolean(ByVal parameterName As String, ByVal x As Boolean)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
	   End Sub



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
	 ''' <seealso cref= #getParams
	 ''' @since 1.4 </seealso>
	   Public Overridable Sub setByte(ByVal parameterName As String, ByVal x As SByte)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
	   End Sub



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
	 ''' <seealso cref= #getParams
	 ''' @since 1.4 </seealso>
	   Public Overridable Sub setShort(ByVal parameterName As String, ByVal x As Short)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
	   End Sub


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
	 ''' <seealso cref= #getParams
	 ''' @since 1.4 </seealso>
	   Public Overridable Sub setInt(ByVal parameterName As String, ByVal x As Integer)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
	   End Sub


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
	 ''' <seealso cref= #getParams
	 ''' @since 1.4 </seealso>
	   Public Overridable Sub setLong(ByVal parameterName As String, ByVal x As Long)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
	   End Sub


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
	 ''' <seealso cref= #getParams
	 ''' @since 1.4 </seealso>
	   Public Overridable Sub setFloat(ByVal parameterName As String, ByVal x As Single)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
	   End Sub


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
	 ''' <seealso cref= #getParams
	 ''' @since 1.4 </seealso>
	   Public Overridable Sub setDouble(ByVal parameterName As String, ByVal x As Double)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
	   End Sub



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
	 ''' <seealso cref= #getParams
	 ''' @since 1.4 </seealso>
	   Public Overridable Sub setBigDecimal(ByVal parameterName As String, ByVal x As Decimal)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
	   End Sub



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
	 ''' <seealso cref= #getParams
	 ''' @since 1.4 </seealso>
	   Public Overridable Sub setString(ByVal parameterName As String, ByVal x As String)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
	   End Sub



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
	 ''' <seealso cref= #getParams
	 ''' @since 1.4 </seealso>
	   Public Overridable Sub setBytes(ByVal parameterName As String, ByVal x As SByte())
			Throw New SQLFeatureNotSupportedException("Feature not supported")
	   End Sub



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
	 ''' <seealso cref= #getParams
	 ''' @since 1.4 </seealso>
	   Public Overridable Sub setTimestamp(ByVal parameterName As String, ByVal x As java.sql.Timestamp)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
	   End Sub



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
	   Public Overridable Sub setAsciiStream(ByVal parameterName As String, ByVal x As java.io.InputStream, ByVal length As Integer)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
	   End Sub


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
	   Public Overridable Sub setBinaryStream(ByVal parameterName As String, ByVal x As java.io.InputStream, ByVal length As Integer)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
	   End Sub


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
	   Public Overridable Sub setCharacterStream(ByVal parameterName As String, ByVal reader As java.io.Reader, ByVal length As Integer)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
	   End Sub


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
	  Public Overridable Sub setAsciiStream(ByVal parameterName As String, ByVal x As java.io.InputStream)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
	  End Sub


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
	   Public Overridable Sub setBinaryStream(ByVal parameterName As String, ByVal x As java.io.InputStream)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
	   End Sub



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
	   Public Overridable Sub setCharacterStream(ByVal parameterName As String, ByVal reader As java.io.Reader)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
	   End Sub


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
	  Public Overridable Sub setNCharacterStream(ByVal parameterIndex As Integer, ByVal value As Reader)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
	  End Sub



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
	 ''' <seealso cref= #getParams
	 ''' @since 1.4 </seealso>
	   Public Overridable Sub setObject(ByVal parameterName As String, ByVal x As Object, ByVal targetSqlType As Integer, ByVal scale As Integer)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
	   End Sub



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
	 ''' <seealso cref= #getParams
	 ''' @since 1.4 </seealso>
	   Public Overridable Sub setObject(ByVal parameterName As String, ByVal x As Object, ByVal targetSqlType As Integer)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
	   End Sub


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
	 ''' <seealso cref= #getParams
	 ''' @since 1.4 </seealso>
	  Public Overridable Sub setObject(ByVal parameterName As String, ByVal x As Object)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
	  End Sub



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
		Public Overridable Sub setBlob(ByVal parameterIndex As Integer, ByVal inputStream As InputStream, ByVal length As Long)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
		End Sub


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
		Public Overridable Sub setBlob(ByVal parameterIndex As Integer, ByVal inputStream As InputStream)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
		End Sub


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
		 Public Overridable Sub setBlob(ByVal parameterName As String, ByVal inputStream As InputStream, ByVal length As Long)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
		 End Sub


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
	   Public Overridable Sub setBlob(ByVal parameterName As String, ByVal x As Blob)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
	   End Sub


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
		Public Overridable Sub setBlob(ByVal parameterName As String, ByVal inputStream As InputStream)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
		End Sub


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
	   Public Overridable Sub setClob(ByVal parameterIndex As Integer, ByVal reader As Reader, ByVal length As Long)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
	   End Sub


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
	   Public Overridable Sub setClob(ByVal parameterIndex As Integer, ByVal reader As Reader)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
	   End Sub


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
				  Public Overridable Sub setClob(ByVal parameterName As String, ByVal reader As Reader, ByVal length As Long)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
				  End Sub


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
	   Public Overridable Sub setClob(ByVal parameterName As String, ByVal x As Clob)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
	   End Sub


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
		Public Overridable Sub setClob(ByVal parameterName As String, ByVal reader As Reader)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
		End Sub


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
	 ''' <seealso cref= #getParams
	 ''' @since 1.4 </seealso>
	   Public Overridable Sub setDate(ByVal parameterName As String, ByVal x As java.sql.Date)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
	   End Sub


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
	 ''' <seealso cref= #getParams
	 ''' @since 1.4 </seealso>
	   Public Overridable Sub setDate(ByVal parameterName As String, ByVal x As java.sql.Date, ByVal cal As DateTime)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
	   End Sub


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
	 ''' <seealso cref= #getParams
	 ''' @since 1.4 </seealso>
	   Public Overridable Sub setTime(ByVal parameterName As String, ByVal x As java.sql.Time)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
	   End Sub


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
	 ''' <seealso cref= #getParams
	 ''' @since 1.4 </seealso>
	   Public Overridable Sub setTime(ByVal parameterName As String, ByVal x As java.sql.Time, ByVal cal As DateTime)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
	   End Sub


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
	 ''' <seealso cref= #getParams
	 ''' @since 1.4 </seealso>
	   Public Overridable Sub setTimestamp(ByVal parameterName As String, ByVal x As java.sql.Timestamp, ByVal cal As DateTime)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
	   End Sub


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
	 '''  stream does not contain valid XML. </exception>
	 ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not
	 ''' support this method
	 ''' @since 1.6 </exception>
	 Public Overridable Sub setSQLXML(ByVal parameterIndex As Integer, ByVal xmlObject As SQLXML)
		 Throw New SQLFeatureNotSupportedException("Feature not supported")
	 End Sub


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
	 '''  stream does not contain valid XML. </exception>
	 ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not
	 ''' support this method
	 ''' @since 1.6 </exception>
	 Public Overridable Sub setSQLXML(ByVal parameterName As String, ByVal xmlObject As SQLXML)
		 Throw New SQLFeatureNotSupportedException("Feature not supported")
	 End Sub


	 ''' <summary>
	 ''' Sets the designated parameter to the given <code>java.sql.RowId</code> object. The
	 ''' driver converts this to a SQL <code>ROWID</code> value when it sends it
	 ''' to the database
	 ''' </summary>
	 ''' <param name="parameterIndex"> the first parameter is 1, the second is 2, ... </param>
	 ''' <param name="x"> the parameter value </param>
	 ''' <exception cref="SQLException"> if a database access error occurs </exception>
	 ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not
	 ''' support this method
	 ''' 
	 ''' @since 1.6 </exception>
	 Public Overridable Sub setRowId(ByVal parameterIndex As Integer, ByVal x As RowId)
		 Throw New SQLFeatureNotSupportedException("Feature not supported")
	 End Sub


	 ''' <summary>
	 ''' Sets the designated parameter to the given <code>java.sql.RowId</code> object. The
	 ''' driver converts this to a SQL <code>ROWID</code> when it sends it to the
	 ''' database.
	 ''' </summary>
	 ''' <param name="parameterName"> the name of the parameter </param>
	 ''' <param name="x"> the parameter value </param>
	 ''' <exception cref="SQLException"> if a database access error occurs </exception>
	 ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not
	 ''' support this method
	 ''' @since 1.6 </exception>
	 Public Overridable Sub setRowId(ByVal parameterName As String, ByVal x As RowId)
		 Throw New SQLFeatureNotSupportedException("Feature not supported")
	 End Sub

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
	 '''  error could occur ; or if a database access error occurs </exception>
	 ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not
	 ''' support this method
	 ''' @since 1.6 </exception>
	 Public Overridable Sub setNString(ByVal parameterIndex As Integer, ByVal value As String)
		 Throw New SQLFeatureNotSupportedException("Feature not supported")
	 End Sub


	 ''' <summary>
	 ''' Sets the designated parameter to the given <code>String</code> object.
	 ''' The driver converts this to a SQL <code>NCHAR</code> or
	 ''' <code>NVARCHAR</code> or <code>LONGNVARCHAR</code> </summary>
	 ''' <param name="parameterName"> the name of the column to be set </param>
	 ''' <param name="value"> the parameter value </param>
	 ''' <exception cref="SQLException"> if the driver does not support national
	 '''         character sets;  if the driver can detect that a data conversion
	 '''  error could occur; or if a database access error occurs </exception>
	 ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not
	 ''' support this method
	 ''' @since 1.6 </exception>
	 Public Overridable Sub setNString(ByVal parameterName As String, ByVal value As String)
		 Throw New SQLFeatureNotSupportedException("Feature not supported")
	 End Sub


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
	 '''  error could occur ; or if a database access error occurs </exception>
	 ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not
	 ''' support this method
	 ''' @since 1.6 </exception>
	 Public Overridable Sub setNCharacterStream(ByVal parameterIndex As Integer, ByVal value As Reader, ByVal length As Long)
		 Throw New SQLFeatureNotSupportedException("Feature not supported")
	 End Sub


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
	 '''  error could occur; or if a database access error occurs </exception>
	 ''' <exception cref="SQLFeatureNotSupportedException">  if the JDBC driver does not
	 ''' support this method
	 ''' @since 1.6 </exception>
	 Public Overridable Sub setNCharacterStream(ByVal parameterName As String, ByVal value As Reader, ByVal length As Long)
		 Throw New SQLFeatureNotSupportedException("Feature not supported")
	 End Sub


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
	  Public Overridable Sub setNCharacterStream(ByVal parameterName As String, ByVal value As Reader)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
	  End Sub


	  ''' <summary>
	  ''' Sets the designated parameter to a <code>java.sql.NClob</code> object. The object
	  ''' implements the <code>java.sql.NClob</code> interface. This <code>NClob</code>
	  ''' object maps to a SQL <code>NCLOB</code>. </summary>
	  ''' <param name="parameterName"> the name of the column to be set </param>
	  ''' <param name="value"> the parameter value </param>
	  ''' <exception cref="SQLException"> if the driver does not support national
	  '''         character sets;  if the driver can detect that a data conversion
	  '''  error could occur; or if a database access error occurs </exception>
	  ''' <exception cref="SQLFeatureNotSupportedException">  if the JDBC driver does not
	  ''' support this method
	  ''' @since 1.6 </exception>
	  Public Overridable Sub setNClob(ByVal parameterName As String, ByVal value As NClob)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
	  End Sub


	  ''' <summary>
	  ''' Sets the designated parameter to a <code>Reader</code> object.  The <code>reader</code> must contain
	  ''' the number
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
	  Public Overridable Sub setNClob(ByVal parameterName As String, ByVal reader As Reader, ByVal length As Long)
		   Throw New SQLFeatureNotSupportedException("Feature not supported")
	  End Sub


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
	  Public Overridable Sub setNClob(ByVal parameterName As String, ByVal reader As Reader)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
	  End Sub


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
	  ''' <exception cref="SQLFeatureNotSupportedException">  if the JDBC driver does not
	  ''' support this method
	  ''' 
	  ''' @since 1.6 </exception>
	  Public Overridable Sub setNClob(ByVal parameterIndex As Integer, ByVal reader As Reader, ByVal length As Long)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
	  End Sub


	  ''' <summary>
	  ''' Sets the designated parameter to a <code>java.sql.NClob</code> object. The driver converts this oa
	  ''' SQL <code>NCLOB</code> value when it sends it to the database. </summary>
	  ''' <param name="parameterIndex"> of the first parameter is 1, the second is 2, ... </param>
	  ''' <param name="value"> the parameter value </param>
	  ''' <exception cref="SQLException"> if the driver does not support national
	  '''         character sets;  if the driver can detect that a data conversion
	  '''  error could occur ; or if a database access error occurs </exception>
	  ''' <exception cref="SQLFeatureNotSupportedException">  if the JDBC driver does not
	  ''' support this method
	  ''' @since 1.6 </exception>
	 Public Overridable Sub setNClob(ByVal parameterIndex As Integer, ByVal value As NClob)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
	 End Sub


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
	  Public Overridable Sub setNClob(ByVal parameterIndex As Integer, ByVal reader As Reader)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
	  End Sub


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
	  Public Overridable Sub setURL(ByVal parameterIndex As Integer, ByVal x As java.net.URL)
			Throw New SQLFeatureNotSupportedException("Feature not supported")
	  End Sub



	  Friend Const serialVersionUID As Long = 4886719666485113312L

	End Class 'end class

End Namespace