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
	''' <P>
	''' An object that registers to be notified of events generated by a
	''' <code>PooledConnection</code> object.
	''' <P>
	''' The <code>ConnectionEventListener</code> interface is implemented by a
	''' connection pooling component.  A connection pooling component will
	''' usually be provided by a JDBC driver vendor or another system software
	''' vendor.  A JDBC driver notifies a <code>ConnectionEventListener</code>
	''' object when an application is finished using a pooled connection with
	''' which the listener has registered.  The notification
	''' occurs after the application calls the method <code>close</code> on
	''' its representation of a <code>PooledConnection</code> object.  A
	''' <code>ConnectionEventListener</code> is also notified when a
	''' connection error occurs due to the fact that the <code>PooledConnection</code>
	''' is unfit for future use---the server has crashed, for example.
	''' The listener is notified by the JDBC driver just before the driver throws an
	''' <code>SQLException</code> to the application using the
	''' <code>PooledConnection</code> object.
	''' 
	''' @since 1.4
	''' </summary>

	Public Interface ConnectionEventListener
		Inherits java.util.EventListener

	  ''' <summary>
	  ''' Notifies this <code>ConnectionEventListener</code> that
	  ''' the application has called the method <code>close</code> on its
	  ''' representation of a pooled connection.
	  ''' </summary>
	  ''' <param name="event"> an event object describing the source of
	  ''' the event </param>
	  Sub connectionClosed(ByVal [event] As ConnectionEvent)

	  ''' <summary>
	  ''' Notifies this <code>ConnectionEventListener</code> that
	  ''' a fatal error has occurred and the pooled connection can
	  ''' no longer be used.  The driver makes this notification just
	  ''' before it throws the application the <code>SQLException</code>
	  ''' contained in the given <code>ConnectionEvent</code> object.
	  ''' </summary>
	  ''' <param name="event"> an event object describing the source of
	  ''' the event and containing the <code>SQLException</code> that the
	  ''' driver is about to throw </param>
	  Sub connectionErrorOccurred(ByVal [event] As ConnectionEvent)

	End Interface

End Namespace