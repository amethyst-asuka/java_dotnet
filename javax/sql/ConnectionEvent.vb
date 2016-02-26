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
	''' <P>An <code>Event</code> object that provides information about the
	''' source of a connection-related event.  <code>ConnectionEvent</code>
	''' objects are generated when an application closes a pooled connection
	''' and when an error occurs.  The <code>ConnectionEvent</code> object
	''' contains two kinds of information:
	''' <UL>
	'''   <LI>The pooled connection closed by the application
	'''   <LI>In the case of an error event, the <code>SQLException</code>
	'''       about to be thrown to the application
	''' </UL>
	''' 
	''' @since 1.4
	''' </summary>

	Public Class ConnectionEvent
		Inherits java.util.EventObject

	  ''' <summary>
	  ''' <P>Constructs a <code>ConnectionEvent</code> object initialized with
	  ''' the given <code>PooledConnection</code> object. <code>SQLException</code>
	  ''' defaults to <code>null</code>.
	  ''' </summary>
	  ''' <param name="con"> the pooled connection that is the source of the event </param>
	  ''' <exception cref="IllegalArgumentException"> if <code>con</code> is null. </exception>
	  Public Sub New(ByVal con As PooledConnection)
		MyBase.New(con)
	  End Sub

	  ''' <summary>
	  ''' <P>Constructs a <code>ConnectionEvent</code> object initialized with
	  ''' the given <code>PooledConnection</code> object and
	  ''' <code>SQLException</code> object.
	  ''' </summary>
	  ''' <param name="con"> the pooled connection that is the source of the event </param>
	  ''' <param name="ex"> the SQLException about to be thrown to the application </param>
	  ''' <exception cref="IllegalArgumentException"> if <code>con</code> is null. </exception>
	  Public Sub New(ByVal con As PooledConnection, ByVal ex As java.sql.SQLException)
		MyBase.New(con)
		Me.ex = ex
	  End Sub

	  ''' <summary>
	  ''' <P>Retrieves the <code>SQLException</code> for this
	  ''' <code>ConnectionEvent</code> object. May be <code>null</code>.
	  ''' </summary>
	  ''' <returns> the SQLException about to be thrown or <code>null</code> </returns>
	  Public Overridable Property sQLException As java.sql.SQLException
		  Get
			  Return ex
		  End Get
	  End Property

	  ''' <summary>
	  ''' The <code>SQLException</code> that the driver will throw to the
	  ''' application when an error occurs and the pooled connection is no
	  ''' longer usable.
	  ''' @serial
	  ''' </summary>
	  Private ex As java.sql.SQLException = Nothing

	  ''' <summary>
	  ''' Private serial version unique ID to ensure serialization
	  ''' compatibility.
	  ''' </summary>
	  Friend Const serialVersionUID As Long = -4843217645290030002L

	End Class

End Namespace