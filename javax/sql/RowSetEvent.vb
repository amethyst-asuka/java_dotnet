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
	''' An <code>Event</code> object generated when an event occurs to a
	''' <code>RowSet</code> object.  A <code>RowSetEvent</code> object is
	''' generated when a single row in a rowset is changed, the whole rowset
	''' is changed, or the rowset cursor moves.
	''' <P>
	''' When an event occurs on a <code>RowSet</code> object, one of the
	''' <code>RowSetListener</code> methods will be sent to all registered
	''' listeners to notify them of the event.  An <code>Event</code> object
	''' is supplied to the <code>RowSetListener</code> method so that the
	''' listener can use it to find out which <code>RowSet</code> object is
	''' the source of the event.
	''' 
	''' @since 1.4
	''' </summary>

	Public Class RowSetEvent
		Inherits java.util.EventObject

	  ''' <summary>
	  ''' Constructs a <code>RowSetEvent</code> object initialized with the
	  ''' given <code>RowSet</code> object.
	  ''' </summary>
	  ''' <param name="source"> the <code>RowSet</code> object whose data has changed or
	  '''        whose cursor has moved </param>
	  ''' <exception cref="IllegalArgumentException"> if <code>source</code> is null. </exception>
	  Public Sub New(ByVal source As RowSet)
			MyBase.New(source)
	  End Sub

	  ''' <summary>
	  ''' Private serial version unique ID to ensure serialization
	  ''' compatibility.
	  ''' </summary>
	  Friend Const serialVersionUID As Long = -1875450876546332005L
	End Class

End Namespace