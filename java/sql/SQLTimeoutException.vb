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
	''' <P>The subclass of <seealso cref="SQLException"/> thrown when the timeout specified by
	''' {@code Statement.setQueryTimeout}, {@code DriverManager.setLoginTimeout},
	''' {@code DataSource.setLoginTimeout},{@code XADataSource.setLoginTimeout}
	''' has expired.
	''' <P> This exception does not correspond to a standard SQLState.
	''' 
	''' @since 1.6
	''' </summary>

	Public Class SQLTimeoutException
		Inherits SQLTransientException

			''' <summary>
			''' Constructs a <code>SQLTimeoutException</code> object.
			''' The <code>reason</code>, <code>SQLState</code> are initialized
			''' to <code>null</code> and the vendor code is initialized to 0.
			''' 
			''' The <code>cause</code> is not initialized, and may subsequently be
			''' initialized by a call to the
			''' <seealso cref="Throwable#initCause(java.lang.Throwable)"/> method.
			''' <p>
			''' @since 1.6
			''' </summary>
			Public Sub New()
					MyBase.New()
			End Sub

			''' <summary>
			''' Constructs a <code>SQLTimeoutException</code> object
			''' with a given <code>reason</code>. The <code>SQLState</code>
			''' is initialized to <code>null</code> and the vendor code is initialized
			''' to 0.
			''' 
			''' The <code>cause</code> is not initialized, and may subsequently be
			''' initialized by a call to the
			''' <seealso cref="Throwable#initCause(java.lang.Throwable)"/> method.
			''' <p> </summary>
			''' <param name="reason"> a description of the exception
			''' @since 1.6 </param>
			Public Sub New(  reason As String)
					MyBase.New(reason)
			End Sub

			''' <summary>
			''' Constructs a <code>SQLTimeoutException</code> object
			''' with a given <code>reason</code> and <code>SQLState</code>.
			''' 
			''' The <code>cause</code> is not initialized, and may subsequently be
			''' initialized by a call to the
			''' <seealso cref="Throwable#initCause(java.lang.Throwable)"/> method. The vendor code
			''' is initialized to 0.
			''' <p> </summary>
			''' <param name="reason"> a description of the exception </param>
			''' <param name="SQLState"> an XOPEN or SQL:2003 code identifying the exception
			''' @since 1.6 </param>
			Public Sub New(  reason As String,   SQLState As String)
					MyBase.New(reason, SQLState)
			End Sub

			''' <summary>
			''' Constructs a <code>SQLTimeoutException</code> object
			''' with a given <code>reason</code>, <code>SQLState</code>  and
			''' <code>vendorCode</code>.
			''' 
			''' The <code>cause</code> is not initialized, and may subsequently be
			''' initialized by a call to the
			''' <seealso cref="Throwable#initCause(java.lang.Throwable)"/> method.
			''' <p> </summary>
			''' <param name="reason"> a description of the exception </param>
			''' <param name="SQLState"> an XOPEN or SQL:2003 code identifying the exception </param>
			''' <param name="vendorCode"> a database vendor specific exception code
			''' @since 1.6 </param>
			Public Sub New(  reason As String,   SQLState As String,   vendorCode As Integer)
					MyBase.New(reason, SQLState, vendorCode)
			End Sub

		''' <summary>
		''' Constructs a <code>SQLTimeoutException</code> object
		''' with a given  <code>cause</code>.
		''' The <code>SQLState</code> is initialized
		''' to <code>null</code> and the vendor code is initialized to 0.
		''' The <code>reason</code>  is initialized to <code>null</code> if
		''' <code>cause==null</code> or to <code>cause.toString()</code> if
		''' <code>cause!=null</code>.
		''' <p> </summary>
		''' <param name="cause"> the underlying reason for this <code>SQLException</code> (which is saved for later retrieval by the <code>getCause()</code> method); may be null indicating
		'''     the cause is non-existent or unknown.
		''' @since 1.6 </param>
		Public Sub New(  cause As Throwable)
			MyBase.New(cause)
		End Sub

		''' <summary>
		''' Constructs a <code>SQLTimeoutException</code> object
		''' with a given
		''' <code>reason</code> and  <code>cause</code>.
		''' The <code>SQLState</code> is  initialized to <code>null</code>
		''' and the vendor code is initialized to 0.
		''' <p> </summary>
		''' <param name="reason"> a description of the exception. </param>
		''' <param name="cause"> the underlying reason for this <code>SQLException</code> (which is saved for later retrieval by the <code>getCause()</code> method); may be null indicating
		'''     the cause is non-existent or unknown.
		''' @since 1.6 </param>
		Public Sub New(  reason As String,   cause As Throwable)
			MyBase.New(reason, cause)
		End Sub

		''' <summary>
		''' Constructs a <code>SQLTimeoutException</code> object
		'''  with a given
		''' <code>reason</code>, <code>SQLState</code> and  <code>cause</code>.
		''' The vendor code is initialized to 0.
		''' <p> </summary>
		''' <param name="reason"> a description of the exception. </param>
		''' <param name="SQLState"> an XOPEN or SQL:2003 code identifying the exception </param>
		''' <param name="cause"> the underlying reason for this <code>SQLException</code> (which is saved for later retrieval by the <code>getCause()</code> method); may be null indicating
		'''     the cause is non-existent or unknown.
		''' @since 1.6 </param>
		Public Sub New(  reason As String,   SQLState As String,   cause As Throwable)
			MyBase.New(reason, SQLState, cause)
		End Sub

		''' <summary>
		'''  Constructs a <code>SQLTimeoutException</code> object
		''' with a given
		''' <code>reason</code>, <code>SQLState</code>, <code>vendorCode</code>
		''' and  <code>cause</code>.
		''' <p> </summary>
		''' <param name="reason"> a description of the exception </param>
		''' <param name="SQLState"> an XOPEN or SQL:2003 code identifying the exception </param>
		''' <param name="vendorCode"> a database vendor-specific exception code </param>
		''' <param name="cause"> the underlying reason for this <code>SQLException</code> (which is saved for later retrieval by the <code>getCause()</code> method); may be null indicating
		'''     the cause is non-existent or unknown.
		''' @since 1.6 </param>
		Public Sub New(  reason As String,   SQLState As String,   vendorCode As Integer,   cause As Throwable)
			MyBase.New(reason, SQLState, vendorCode, cause)
		End Sub

		Private Shadows Const serialVersionUID As Long = -4487171280562520262L
	End Class

End Namespace