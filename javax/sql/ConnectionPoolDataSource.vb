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
	''' A factory for <code>PooledConnection</code>
	''' objects.  An object that implements this interface will typically be
	''' registered with a naming service that is based on the
	''' Java&trade; Naming and Directory Interface
	''' (JNDI).
	''' 
	''' @since 1.4
	''' </summary>

	Public Interface ConnectionPoolDataSource
		Inherits CommonDataSource

	  ''' <summary>
	  ''' Attempts to establish a physical database connection that can
	  ''' be used as a pooled connection.
	  ''' </summary>
	  ''' <returns>  a <code>PooledConnection</code> object that is a physical
	  '''         connection to the database that this
	  '''         <code>ConnectionPoolDataSource</code> object represents </returns>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  ''' <exception cref="java.sql.SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  ''' this method
	  ''' @since 1.4 </exception>
	  ReadOnly Property pooledConnection As PooledConnection

	  ''' <summary>
	  ''' Attempts to establish a physical database connection that can
	  ''' be used as a pooled connection.
	  ''' </summary>
	  ''' <param name="user"> the database user on whose behalf the connection is being made </param>
	  ''' <param name="password"> the user's password </param>
	  ''' <returns>  a <code>PooledConnection</code> object that is a physical
	  '''         connection to the database that this
	  '''         <code>ConnectionPoolDataSource</code> object represents </returns>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  ''' <exception cref="java.sql.SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  ''' this method
	  ''' @since 1.4 </exception>
	  Function getPooledConnection(ByVal user As String, ByVal password As String) As PooledConnection
	End Interface

End Namespace