Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.sql


	''' <summary>
	''' <P>An exception that provides information on a database access
	''' error or other errors.
	''' 
	''' <P>Each <code>SQLException</code> provides several kinds of information:
	''' <UL>
	'''   <LI> a string describing the error.  This is used as the Java Exception
	'''       message, available via the method <code>getMesasge</code>.
	'''   <LI> a "SQLstate" string, which follows either the XOPEN SQLstate conventions
	'''        or the SQL:2003 conventions.
	'''       The values of the SQLState string are described in the appropriate spec.
	'''       The <code>DatabaseMetaData</code> method <code>getSQLStateType</code>
	'''       can be used to discover whether the driver returns the XOPEN type or
	'''       the SQL:2003 type.
	'''   <LI> an integer error code that is specific to each vendor.  Normally this will
	'''       be the actual error code returned by the underlying database.
	'''   <LI> a chain to a next Exception.  This can be used to provide additional
	'''       error information.
	'''   <LI> the causal relationship, if any for this <code>SQLException</code>.
	''' </UL>
	''' </summary>
	Public Class SQLException
		Inherits Exception
		Implements Iterable(Of Throwable)

		''' <summary>
		'''  Constructs a <code>SQLException</code> object with a given
		''' <code>reason</code>, <code>SQLState</code>  and
		''' <code>vendorCode</code>.
		''' 
		''' The <code>cause</code> is not initialized, and may subsequently be
		''' initialized by a call to the
		''' <seealso cref="Throwable#initCause(java.lang.Throwable)"/> method.
		''' <p> </summary>
		''' <param name="reason"> a description of the exception </param>
		''' <param name="SQLState"> an XOPEN or SQL:2003 code identifying the exception </param>
		''' <param name="vendorCode"> a database vendor-specific exception code </param>
		Public Sub New(  reason As String,   SQLState As String,   vendorCode As Integer)
			MyBase.New(reason)
			Me.SQLState = SQLState
			Me.vendorCode = vendorCode
			If Not(TypeOf Me Is SQLWarning) Then
				If DriverManager.logWriter IsNot Nothing Then
					DriverManager.println("SQLState(" & SQLState & ") vendor code(" & vendorCode & ")")
					printStackTrace(DriverManager.logWriter)
				End If
			End If
		End Sub


		''' <summary>
		''' Constructs a <code>SQLException</code> object with a given
		''' <code>reason</code> and <code>SQLState</code>.
		''' 
		''' The <code>cause</code> is not initialized, and may subsequently be
		''' initialized by a call to the
		''' <seealso cref="Throwable#initCause(java.lang.Throwable)"/> method. The vendor code
		''' is initialized to 0.
		''' <p> </summary>
		''' <param name="reason"> a description of the exception </param>
		''' <param name="SQLState"> an XOPEN or SQL:2003 code identifying the exception </param>
		Public Sub New(  reason As String,   SQLState As String)
			MyBase.New(reason)
			Me.SQLState = SQLState
			Me.vendorCode = 0
			If Not(TypeOf Me Is SQLWarning) Then
				If DriverManager.logWriter IsNot Nothing Then
					printStackTrace(DriverManager.logWriter)
					DriverManager.println("SQLException: SQLState(" & SQLState & ")")
				End If
			End If
		End Sub

		''' <summary>
		'''  Constructs a <code>SQLException</code> object with a given
		''' <code>reason</code>. The  <code>SQLState</code>  is initialized to
		''' <code>null</code> and the vendor code is initialized to 0.
		''' 
		''' The <code>cause</code> is not initialized, and may subsequently be
		''' initialized by a call to the
		''' <seealso cref="Throwable#initCause(java.lang.Throwable)"/> method.
		''' <p> </summary>
		''' <param name="reason"> a description of the exception </param>
		Public Sub New(  reason As String)
			MyBase.New(reason)
			Me.SQLState = Nothing
			Me.vendorCode = 0
			If Not(TypeOf Me Is SQLWarning) Then
				If DriverManager.logWriter IsNot Nothing Then printStackTrace(DriverManager.logWriter)
			End If
		End Sub

		''' <summary>
		''' Constructs a <code>SQLException</code> object.
		''' The <code>reason</code>, <code>SQLState</code> are initialized
		''' to <code>null</code> and the vendor code is initialized to 0.
		''' 
		''' The <code>cause</code> is not initialized, and may subsequently be
		''' initialized by a call to the
		''' <seealso cref="Throwable#initCause(java.lang.Throwable)"/> method.
		''' 
		''' </summary>
		Public Sub New()
			MyBase.New()
			Me.SQLState = Nothing
			Me.vendorCode = 0
			If Not(TypeOf Me Is SQLWarning) Then
				If DriverManager.logWriter IsNot Nothing Then printStackTrace(DriverManager.logWriter)
			End If
		End Sub

		''' <summary>
		'''  Constructs a <code>SQLException</code> object with a given
		''' <code>cause</code>.
		''' The <code>SQLState</code> is initialized
		''' to <code>null</code> and the vendor code is initialized to 0.
		''' The <code>reason</code>  is initialized to <code>null</code> if
		''' <code>cause==null</code> or to <code>cause.toString()</code> if
		''' <code>cause!=null</code>.
		''' <p> </summary>
		''' <param name="cause"> the underlying reason for this <code>SQLException</code>
		''' (which is saved for later retrieval by the <code>getCause()</code> method);
		''' may be null indicating the cause is non-existent or unknown.
		''' @since 1.6 </param>
		Public Sub New(  cause As Throwable)
			MyBase.New(cause)

			If Not(TypeOf Me Is SQLWarning) Then
				If DriverManager.logWriter IsNot Nothing Then printStackTrace(DriverManager.logWriter)
			End If
		End Sub

		''' <summary>
		''' Constructs a <code>SQLException</code> object with a given
		''' <code>reason</code> and  <code>cause</code>.
		''' The <code>SQLState</code> is  initialized to <code>null</code>
		''' and the vendor code is initialized to 0.
		''' <p> </summary>
		''' <param name="reason"> a description of the exception. </param>
		''' <param name="cause"> the underlying reason for this <code>SQLException</code>
		''' (which is saved for later retrieval by the <code>getCause()</code> method);
		''' may be null indicating the cause is non-existent or unknown.
		''' @since 1.6 </param>
		Public Sub New(  reason As String,   cause As Throwable)
			MyBase.New(reason,cause)

			If Not(TypeOf Me Is SQLWarning) Then
				If DriverManager.logWriter IsNot Nothing Then printStackTrace(DriverManager.logWriter)
			End If
		End Sub

		''' <summary>
		''' Constructs a <code>SQLException</code> object with a given
		''' <code>reason</code>, <code>SQLState</code> and  <code>cause</code>.
		''' The vendor code is initialized to 0.
		''' <p> </summary>
		''' <param name="reason"> a description of the exception. </param>
		''' <param name="sqlState"> an XOPEN or SQL:2003 code identifying the exception </param>
		''' <param name="cause"> the underlying reason for this <code>SQLException</code>
		''' (which is saved for later retrieval by the
		''' <code>getCause()</code> method); may be null indicating
		'''     the cause is non-existent or unknown.
		''' @since 1.6 </param>
		Public Sub New(  reason As String,   sqlState As String,   cause As Throwable)
			MyBase.New(reason,cause)

			Me.SQLState = sqlState
			Me.vendorCode = 0
			If Not(TypeOf Me Is SQLWarning) Then
				If DriverManager.logWriter IsNot Nothing Then
					printStackTrace(DriverManager.logWriter)
					DriverManager.println("SQLState(" & Me.SQLState & ")")
				End If
			End If
		End Sub

		''' <summary>
		''' Constructs a <code>SQLException</code> object with a given
		''' <code>reason</code>, <code>SQLState</code>, <code>vendorCode</code>
		''' and  <code>cause</code>.
		''' <p> </summary>
		''' <param name="reason"> a description of the exception </param>
		''' <param name="sqlState"> an XOPEN or SQL:2003 code identifying the exception </param>
		''' <param name="vendorCode"> a database vendor-specific exception code </param>
		''' <param name="cause"> the underlying reason for this <code>SQLException</code>
		''' (which is saved for later retrieval by the <code>getCause()</code> method);
		''' may be null indicating the cause is non-existent or unknown.
		''' @since 1.6 </param>
		Public Sub New(  reason As String,   sqlState As String,   vendorCode As Integer,   cause As Throwable)
			MyBase.New(reason,cause)

			Me.SQLState = sqlState
			Me.vendorCode = vendorCode
			If Not(TypeOf Me Is SQLWarning) Then
				If DriverManager.logWriter IsNot Nothing Then
					DriverManager.println("SQLState(" & Me.SQLState & ") vendor code(" & vendorCode & ")")
					printStackTrace(DriverManager.logWriter)
				End If
			End If
		End Sub

		''' <summary>
		''' Retrieves the SQLState for this <code>SQLException</code> object.
		''' </summary>
		''' <returns> the SQLState value </returns>
		Public Overridable Property sQLState As String
			Get
				Return (SQLState)
			End Get
		End Property

		''' <summary>
		''' Retrieves the vendor-specific exception code
		''' for this <code>SQLException</code> object.
		''' </summary>
		''' <returns> the vendor's error code </returns>
		Public Overridable Property errorCode As Integer
			Get
				Return (vendorCode)
			End Get
		End Property

		''' <summary>
		''' Retrieves the exception chained to this
		''' <code>SQLException</code> object by setNextException(SQLException ex).
		''' </summary>
		''' <returns> the next <code>SQLException</code> object in the chain;
		'''         <code>null</code> if there are none </returns>
		''' <seealso cref= #setNextException </seealso>
		Public Overridable Property nextException As SQLException
			Get
				Return ([next])
			End Get
			Set(  ex As SQLException)
    
				Dim current As SQLException = Me
				Do
					Dim [next] As SQLException=current.next
					If [next] IsNot Nothing Then
						current = [next]
						Continue Do
					End If
    
					If nextUpdater.compareAndSet(current,Nothing,ex) Then Return
					current=current.next
				Loop
			End Set
		End Property


		''' <summary>
		''' Returns an iterator over the chained SQLExceptions.  The iterator will
		''' be used to iterate over each SQLException and its underlying cause
		''' (if any).
		''' </summary>
		''' <returns> an iterator over the chained SQLExceptions and causes in the proper
		''' order
		''' 
		''' @since 1.6 </returns>
		Public Overridable Function [iterator]() As IEnumerator(Of Throwable) Implements Iterable(Of Throwable).iterator

		   Return New IteratorAnonymousInnerClassHelper(Of E)

		End Function

		Private Class IteratorAnonymousInnerClassHelper(Of E)
			Implements IEnumerator(Of E)

			Friend firstException As SQLException = SQLException.this
			Friend nextException As SQLException = firstException.nextException
			Friend cause As Throwable = firstException.cause

			Public Overridable Function hasNext() As Boolean
				If firstException IsNot Nothing OrElse nextException IsNot Nothing OrElse cause IsNot Nothing Then Return True
				Return False
			End Function

			Public Overridable Function [next]() As Throwable
				Dim throwable As Throwable = Nothing
				If firstException IsNot Nothing Then
					throwable = firstException
					firstException = Nothing
				ElseIf cause IsNot Nothing Then
					throwable = cause
					cause = cause.cause
				ElseIf nextException IsNot Nothing Then
					throwable = nextException
					cause = nextException.cause
					nextException = nextException.nextException
				Else
					Throw New java.util.NoSuchElementException
				End If
				Return throwable
			End Function

			Public Overridable Sub remove()
				Throw New UnsupportedOperationException
			End Sub

		End Class

		''' <summary>
		''' @serial
		''' </summary>
		Private SQLState As String

			''' <summary>
			''' @serial
			''' </summary>
		Private vendorCode As Integer

			''' <summary>
			''' @serial
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private [next] As SQLException

		Private Shared ReadOnly nextUpdater As java.util.concurrent.atomic.AtomicReferenceFieldUpdater(Of SQLException, SQLException) = java.util.concurrent.atomic.AtomicReferenceFieldUpdater.newUpdater(GetType(SQLException),GetType(SQLException),"next")

		Private Shadows Const serialVersionUID As Long = 2135244094396331484L
	End Class

End Namespace