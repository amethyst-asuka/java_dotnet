'
' * Copyright (c) 2000, 2001, Oracle and/or its affiliates. All rights reserved.
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
	''' The interface that a <code>RowSet</code> object implements in order to
	''' present itself to a <code>RowSetReader</code> or <code>RowSetWriter</code>
	''' object. The <code>RowSetInternal</code> interface contains
	''' methods that let the reader or writer access and modify the internal
	''' state of the rowset.
	''' 
	''' @since 1.4
	''' </summary>

	Public Interface RowSetInternal

	  ''' <summary>
	  ''' Retrieves the parameters that have been set for this
	  ''' <code>RowSet</code> object's command.
	  ''' </summary>
	  ''' <returns> an array of the current parameter values for this <code>RowSet</code>
	  '''         object's command </returns>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  ReadOnly Property params As Object()

	  ''' <summary>
	  ''' Retrieves the <code>Connection</code> object that was passed to this
	  ''' <code>RowSet</code> object.
	  ''' </summary>
	  ''' <returns> the <code>Connection</code> object passed to the rowset
	  '''      or <code>null</code> if none was passed </returns>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  ReadOnly Property connection As Connection

	  ''' <summary>
	  ''' Sets the given <code>RowSetMetaData</code> object as the
	  ''' <code>RowSetMetaData</code> object for this <code>RowSet</code>
	  ''' object. The <code>RowSetReader</code> object associated with the rowset
	  ''' will use <code>RowSetMetaData</code> methods to set the values giving
	  ''' information about the rowset's columns.
	  ''' </summary>
	  ''' <param name="md"> the <code>RowSetMetaData</code> object that will be set with
	  '''        information about the rowset's columns
	  ''' </param>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  WriteOnly Property metaData As RowSetMetaData

	  ''' <summary>
	  ''' Retrieves a <code>ResultSet</code> object containing the original
	  ''' value of this <code>RowSet</code> object.
	  ''' <P>
	  ''' The cursor is positioned before the first row in the result set.
	  ''' Only rows contained in the result set returned by the method
	  ''' <code>getOriginal</code> are said to have an original value.
	  ''' </summary>
	  ''' <returns> the original value of the rowset </returns>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  ReadOnly Property original As ResultSet

	  ''' <summary>
	  ''' Retrieves a <code>ResultSet</code> object containing the original value
	  ''' of the current row only.  If the current row has no original value,
	  ''' an empty result set is returned. If there is no current row,
	  ''' an exception is thrown.
	  ''' </summary>
	  ''' <returns> the original value of the current row as a <code>ResultSet</code>
	  '''          object </returns>
	  ''' <exception cref="SQLException"> if a database access error occurs or this method
	  '''           is called while the cursor is on the insert row, before the
	  '''           first row, or after the last row </exception>
	  ReadOnly Property originalRow As ResultSet

	End Interface

End Namespace