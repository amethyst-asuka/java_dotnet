'
' * Copyright (c) 2000, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' An object that provides support for distributed
	''' transactions.  An <code>XAConnection</code> object  may be enlisted
	''' in a distributed transaction by means of an <code>XAResource</code> object.
	''' A transaction manager, usually part of a middle tier server, manages an
	''' <code>XAConnection</code> object through the <code>XAResource</code> object.
	''' <P>
	''' An application programmer does not use this interface directly; rather,
	''' it is used by a transaction manager working in the middle tier server.
	''' 
	''' @since 1.4
	''' </summary>

	Public Interface XAConnection
		Inherits PooledConnection


	  ''' <summary>
	  ''' Retrieves an <code>XAResource</code> object that
	  ''' the transaction manager will use
	  ''' to manage this <code>XAConnection</code> object's participation in a
	  ''' distributed transaction.
	  ''' </summary>
	  ''' <returns> the <code>XAResource</code> object </returns>
	  ''' <exception cref="SQLException"> if a database access error occurs </exception>
	  ''' <exception cref="SQLFeatureNotSupportedException"> if the JDBC driver does not support
	  ''' this method
	  ''' @since 1.4 </exception>
	  ReadOnly Property xAResource As javax.transaction.xa.XAResource

	End Interface

End Namespace