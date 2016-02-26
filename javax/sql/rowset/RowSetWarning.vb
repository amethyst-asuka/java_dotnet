Imports System

'
' * Copyright (c) 2003, 2014, Oracle and/or its affiliates. All rights reserved.
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
	''' An extension of <code>SQLException</code> that provides information
	''' about database warnings set on <code>RowSet</code> objects.
	''' Warnings are silently chained to the object whose method call
	''' caused it to be reported.
	''' This class complements the <code>SQLWarning</code> class.
	''' <P>
	''' Rowset warnings may be retrieved from <code>JdbcRowSet</code>,
	''' <code>CachedRowSet</code>&trade;,
	''' <code>WebRowSet</code>, <code>FilteredRowSet</code>, or <code>JoinRowSet</code>
	''' implementations. To retrieve the first warning reported on any
	''' <code>RowSet</code>
	''' implementation,  use the method <code>getRowSetWarnings</code> defined
	''' in the <code>JdbcRowSet</code> interface or the <code>CachedRowSet</code>
	''' interface. To retrieve a warning chained to the first warning, use the
	''' <code>RowSetWarning</code> method
	''' <code>getNextWarning</code>. To retrieve subsequent warnings, call
	''' <code>getNextWarning</code> on each <code>RowSetWarning</code> object that is
	''' returned.
	''' <P>
	''' The inherited methods <code>getMessage</code>, <code>getSQLState</code>,
	''' and <code>getErrorCode</code> retrieve information contained in a
	''' <code>RowSetWarning</code> object.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class RowSetWarning
		Inherits java.sql.SQLException

		''' <summary>
		''' Constructs a <code>RowSetWarning</code> object
		''' with the given value for the reason; SQLState defaults to null,
		''' and vendorCode defaults to 0.
		''' </summary>
		''' <param name="reason"> a <code>String</code> object giving a description
		'''        of the warning; if the <code>String</code> is <code>null</code>,
		'''        this constructor behaves like the default (zero parameter)
		'''        <code>RowSetWarning</code> constructor </param>
		Public Sub New(ByVal reason As String)
			MyBase.New(reason)
		End Sub

		''' <summary>
		''' Constructs a default <code>RowSetWarning</code> object. The reason
		''' defaults to <code>null</code>, SQLState defaults to null and vendorCode
		''' defaults to 0.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a <code>RowSetWarning</code> object initialized with the
		''' given values for the reason and SQLState. The vendor code defaults to 0.
		''' 
		''' If the <code>reason</code> or <code>SQLState</code> parameters are <code>null</code>,
		''' this constructor behaves like the default (zero parameter)
		''' <code>RowSetWarning</code> constructor.
		''' </summary>
		''' <param name="reason"> a <code>String</code> giving a description of the
		'''        warning; </param>
		''' <param name="SQLState"> an XOPEN code identifying the warning; if a non standard
		'''        XOPEN <i>SQLState</i> is supplied, no exception is thrown. </param>
		Public Sub New(ByVal reason As String, ByVal SQLState As String)
			MyBase.New(reason, SQLState)
		End Sub

		''' <summary>
		''' Constructs a fully specified <code>RowSetWarning</code> object initialized
		''' with the given values for the reason, SQLState and vendorCode.
		''' 
		''' If the <code>reason</code>, or the  <code>SQLState</code>
		''' parameters are <code>null</code>, this constructor behaves like the default
		''' (zero parameter) <code>RowSetWarning</code> constructor.
		''' </summary>
		''' <param name="reason"> a <code>String</code> giving a description of the
		'''        warning; </param>
		''' <param name="SQLState"> an XOPEN code identifying the warning; if a non standard
		'''        XOPEN <i>SQLState</i> is supplied, no exception is thrown. </param>
		''' <param name="vendorCode"> a database vendor-specific warning code </param>
		Public Sub New(ByVal reason As String, ByVal SQLState As String, ByVal vendorCode As Integer)
			MyBase.New(reason, SQLState, vendorCode)
		End Sub

		''' <summary>
		''' Retrieves the warning chained to this <code>RowSetWarning</code>
		''' object.
		''' </summary>
		''' <returns> the <code>RowSetWarning</code> object chained to this one; if no
		'''         <code>RowSetWarning</code> object is chained to this one,
		'''         <code>null</code> is returned (default value) </returns>
		''' <seealso cref= #setNextWarning </seealso>
		Public Overridable Property nextWarning As RowSetWarning
			Get
				Dim warning As java.sql.SQLException = nextException
				If warning Is Nothing OrElse TypeOf warning Is RowSetWarning Then
					Return CType(warning, RowSetWarning)
				Else
					' The chained value isn't a RowSetWarning.
					' This is a programming error by whoever added it to
					' the RowSetWarning chain.  We throw a Java "Error".
					Throw New Exception("RowSetWarning chain holds value that is not a RowSetWarning: ")
				End If
			End Get
			Set(ByVal warning As RowSetWarning)
				nextException = warning
			End Set
		End Property


		Friend Const serialVersionUID As Long = 6678332766434564774L
	End Class

End Namespace