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
	''' <h3>1.0 Background</h3>
	''' The <code>Joinable</code> interface provides the methods for getting and
	''' setting a match column, which is the basis for forming the SQL <code>JOIN</code>
	''' formed by adding <code>RowSet</code> objects to a <code>JoinRowSet</code>
	''' object.
	''' <P>
	''' Any standard <code>RowSet</code> implementation <b>may</b> implement
	''' the <code>Joinable</code> interface in order to be
	''' added to a <code>JoinRowSet</code> object. Implementing this interface gives
	''' a <code>RowSet</code> object the ability to use <code>Joinable</code> methods,
	''' which set, retrieve, and get information about match columns.  An
	''' application may add a
	''' <code>RowSet</code> object that has not implemented the <code>Joinable</code>
	''' interface to a <code>JoinRowSet</code> object, but to do so it must use one
	''' of the <code>JoinRowSet.addRowSet</code> methods that takes both a
	''' <code>RowSet</code> object and a match column or an array of <code>RowSet</code>
	''' objects and an array of match columns.
	''' <P>
	''' To get access to the methods in the <code>Joinable</code> interface, a
	''' <code>RowSet</code> object implements at least one of the
	''' five standard <code>RowSet</code> interfaces and also implements the
	''' <code>Joinable</code> interface.  In addition, most <code>RowSet</code>
	''' objects extend the <code>BaseRowSet</code> class.  For example:
	''' <pre>
	'''     class MyRowSetImpl extends BaseRowSet implements CachedRowSet, Joinable {
	'''         :
	'''         :
	'''     }
	''' </pre>
	''' 
	''' <h3>2.0 Usage Guidelines</h3>
	''' <P>
	''' The methods in the <code>Joinable</code> interface allow a <code>RowSet</code> object
	''' to set a match column, retrieve a match column, or unset a match column, which is
	''' the column upon which an SQL <code>JOIN</code> can be based.
	''' An instance of a class that implements these methods can be added to a
	''' <code>JoinRowSet</code> object to allow an SQL <code>JOIN</code> relationship to
	'''  be established.
	''' 
	''' <pre>
	'''     CachedRowSet crs = new MyRowSetImpl();
	'''     crs.populate((ResultSet)rs);
	'''     (Joinable)crs.setMatchColumnIndex(1);
	''' 
	'''     JoinRowSet jrs = new JoinRowSetImpl();
	'''     jrs.addRowSet(crs);
	''' </pre>
	''' In the previous example, <i>crs</i> is a <code>CachedRowSet</code> object that
	''' has implemented the <code>Joinable</code> interface.  In the following example,
	''' <i>crs2</i> has not, so it must supply the match column as an argument to the
	''' <code>addRowSet</code> method. This example assumes that column 1 is the match
	''' column.
	''' <PRE>
	'''     CachedRowSet crs2 = new MyRowSetImpl();
	'''     crs2.populate((ResultSet)rs);
	''' 
	'''     JoinRowSet jrs2 = new JoinRowSetImpl();
	'''     jrs2.addRowSet(crs2, 1);
	''' </PRE>
	''' <p>
	''' The <code>JoinRowSet</code> interface makes it possible to get data from one or
	''' more <code>RowSet</code> objects consolidated into one table without having to incur
	''' the expense of creating a connection to a database. It is therefore ideally suited
	''' for use by disconnected <code>RowSet</code> objects. Nevertheless, any
	''' <code>RowSet</code> object <b>may</b> implement this interface
	''' regardless of whether it is connected or disconnected. Note that a
	''' <code>JdbcRowSet</code> object, being always connected to its data source, can
	''' become part of an SQL <code>JOIN</code> directly without having to become part
	''' of a <code>JoinRowSet</code> object.
	''' 
	''' <h3>3.0 Managing Multiple Match Columns</h3>
	''' The index array passed into the <code>setMatchColumn</code> methods indicates
	''' how many match columns are being set (the length of the array) in addition to
	''' which columns will be used for the match. For example:
	''' <pre>
	'''     int[] i = {1, 2, 4, 7}; // indicates four match columns, with column
	'''                             // indexes 1, 2, 4, 7 participating in the JOIN.
	'''     Joinable.setMatchColumn(i);
	''' </pre>
	''' Subsequent match columns may be added as follows to a different <code>Joinable</code>
	''' object (a <code>RowSet</code> object that has implemented the <code>Joinable</code>
	''' interface).
	''' <pre>
	'''     int[] w = {3, 2, 5, 3};
	'''     Joinable2.setMatchColumn(w);
	''' </pre>
	''' When an application adds two or more <code>RowSet</code> objects to a
	''' <code>JoinRowSet</code> object, the order of the indexes in the array is
	''' particularly important. Each index of
	''' the array maps directly to the corresponding index of the previously added
	''' <code>RowSet</code> object. If overlap or underlap occurs, the match column
	''' data is maintained in the event an additional <code>Joinable</code> RowSet is
	''' added and needs to relate to the match column data. Therefore, applications
	''' can set multiple match columns in any order, but
	''' this order has a direct effect on the outcome of the <code>SQL</code> JOIN.
	''' <p>
	''' This assertion applies in exactly the same manner when column names are used
	''' rather than column indexes to indicate match columns.
	''' </summary>
	''' <seealso cref= JoinRowSet
	''' @author  Jonathan Bruce </seealso>
	Public Interface Joinable

		''' <summary>
		''' Sets the designated column as the match column for this <code>RowSet</code>
		''' object. A <code>JoinRowSet</code> object can now add this <code>RowSet</code>
		''' object based on the match column.
		''' <p>
		''' Sub-interfaces such as the <code>CachedRowSet</code>&trade;
		''' interface define the method <code>CachedRowSet.setKeyColumns</code>, which allows
		''' primary key semantics to be enforced on specific columns.
		''' Implementations of the <code>setMatchColumn(int columnIdx)</code> method
		''' should ensure that the constraints on the key columns are maintained when
		''' a <code>CachedRowSet</code> object sets a primary key column as a match column.
		''' </summary>
		''' <param name="columnIdx"> an <code>int</code> identifying the index of the column to be
		'''        set as the match column </param>
		''' <exception cref="SQLException"> if an invalid column index is set </exception>
		''' <seealso cref= #setMatchColumn(int[]) </seealso>
		''' <seealso cref= #unsetMatchColumn(int)
		'''  </seealso>
		WriteOnly Property matchColumn As Integer

		''' <summary>
		''' Sets the designated columns as the match column for this <code>RowSet</code>
		''' object. A <code>JoinRowSet</code> object can now add this <code>RowSet</code>
		''' object based on the match column.
		''' </summary>
		''' <param name="columnIdxes"> an array of <code>int</code> identifying the indexes of the
		'''      columns to be set as the match columns </param>
		''' <exception cref="SQLException"> if an invalid column index is set </exception>
		''' <seealso cref= #setMatchColumn(int[]) </seealso>
		''' <seealso cref= #unsetMatchColumn(int[]) </seealso>
		WriteOnly Property matchColumn As Integer()

		''' <summary>
		''' Sets the designated column as the match column for this <code>RowSet</code>
		''' object. A <code>JoinRowSet</code> object can now add this <code>RowSet</code>
		''' object based on the match column.
		''' <p>
		''' Subinterfaces such as the <code>CachedRowSet</code> interface define
		''' the method <code>CachedRowSet.setKeyColumns</code>, which allows
		''' primary key semantics to be enforced on specific columns.
		''' Implementations of the <code>setMatchColumn(String columnIdx)</code> method
		''' should ensure that the constraints on the key columns are maintained when
		''' a <code>CachedRowSet</code> object sets a primary key column as a match column.
		''' </summary>
		''' <param name="columnName"> a <code>String</code> object giving the name of the column
		'''      to be set as the match column </param>
		''' <exception cref="SQLException"> if an invalid column name is set, the column name
		'''      is a null, or the column name is an empty string </exception>
		''' <seealso cref= #unsetMatchColumn </seealso>
		''' <seealso cref= #setMatchColumn(int[]) </seealso>
		WriteOnly Property matchColumn As String

		''' <summary>
		''' Sets the designated columns as the match column for this <code>RowSet</code>
		''' object. A <code>JoinRowSet</code> object can now add this <code>RowSet</code>
		''' object based on the match column.
		''' </summary>
		''' <param name="columnNames"> an array of <code>String</code> objects giving the names
		'''     of the column to be set as the match columns </param>
		''' <exception cref="SQLException"> if an invalid column name is set, the column name
		'''      is a null, or the column name is an empty string </exception>
		''' <seealso cref= #unsetMatchColumn </seealso>
		''' <seealso cref= #setMatchColumn(int[]) </seealso>
		WriteOnly Property matchColumn As String()

		''' <summary>
		''' Retrieves the indexes of the match columns that were set for this
		''' <code>RowSet</code> object with the method
		''' <code>setMatchColumn(int[] columnIdxes)</code>.
		''' </summary>
		''' <returns> an <code>int</code> array identifying the indexes of the columns
		'''         that were set as the match columns for this <code>RowSet</code> object </returns>
		''' <exception cref="SQLException"> if no match column has been set </exception>
		''' <seealso cref= #setMatchColumn </seealso>
		''' <seealso cref= #unsetMatchColumn </seealso>
		ReadOnly Property matchColumnIndexes As Integer()

		''' <summary>
		''' Retrieves the names of the match columns that were set for this
		''' <code>RowSet</code> object with the method
		''' <code>setMatchColumn(String [] columnNames)</code>.
		''' </summary>
		''' <returns> an array of <code>String</code> objects giving the names of the columns
		'''         set as the match columns for this <code>RowSet</code> object </returns>
		''' <exception cref="SQLException"> if no match column has been set </exception>
		''' <seealso cref= #setMatchColumn </seealso>
		''' <seealso cref= #unsetMatchColumn
		'''  </seealso>
		ReadOnly Property matchColumnNames As String()

		''' <summary>
		''' Unsets the designated column as the match column for this <code>RowSet</code>
		''' object.
		''' <P>
		''' <code>RowSet</code> objects that implement the <code>Joinable</code> interface
		''' must ensure that a key-like constraint continues to be enforced until the
		''' method <code>CachedRowSet.unsetKeyColumns</code> has been called on the
		''' designated column.
		''' </summary>
		''' <param name="columnIdx"> an <code>int</code> that identifies the index of the column
		'''          that is to be unset as a match column </param>
		''' <exception cref="SQLException"> if an invalid column index is designated or if
		'''          the designated column was not previously set as a match
		'''          column </exception>
		''' <seealso cref= #setMatchColumn </seealso>
		Sub unsetMatchColumn(ByVal columnIdx As Integer)

		''' <summary>
		''' Unsets the designated columns as the match column for this <code>RowSet</code>
		''' object.
		''' </summary>
		''' <param name="columnIdxes"> an array of <code>int</code> that identifies the indexes
		'''     of the columns that are to be unset as match columns </param>
		''' <exception cref="SQLException"> if an invalid column index is designated or if
		'''          the designated column was not previously set as a match
		'''          column </exception>
		''' <seealso cref= #setMatchColumn </seealso>
		Sub unsetMatchColumn(ByVal columnIdxes As Integer())

		''' <summary>
		''' Unsets the designated column as the match column for this <code>RowSet</code>
		''' object.
		''' <P>
		''' <code>RowSet</code> objects that implement the <code>Joinable</code> interface
		''' must ensure that a key-like constraint continues to be enforced until the
		''' method <code>CachedRowSet.unsetKeyColumns</code> has been called on the
		''' designated column.
		''' </summary>
		''' <param name="columnName"> a <code>String</code> object giving the name of the column
		'''          that is to be unset as a match column </param>
		''' <exception cref="SQLException"> if an invalid column name is designated or
		'''          the designated column was not previously set as a match
		'''          column </exception>
		''' <seealso cref= #setMatchColumn </seealso>
		Sub unsetMatchColumn(ByVal columnName As String)

		''' <summary>
		''' Unsets the designated columns as the match columns for this <code>RowSet</code>
		''' object.
		''' </summary>
		''' <param name="columnName"> an array of <code>String</code> objects giving the names of
		'''     the columns that are to be unset as the match columns </param>
		''' <exception cref="SQLException"> if an invalid column name is designated or the
		'''     designated column was not previously set as a match column </exception>
		''' <seealso cref= #setMatchColumn </seealso>
		Sub unsetMatchColumn(ByVal columnName As String())
	End Interface

End Namespace