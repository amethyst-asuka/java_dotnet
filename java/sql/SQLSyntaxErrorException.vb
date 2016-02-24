'
' * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' The subclass of <seealso cref="SQLException"/> thrown when the SQLState class value
	''' is '<i>42</i>', or under vendor-specified conditions. This indicates that the
	''' in-progress query has violated SQL syntax rules.
	''' <p>
	''' Please consult your driver vendor documentation for the vendor-specified
	''' conditions for which this <code>Exception</code> may be thrown.
	''' @since 1.6
	''' </summary>
	Public Class SQLSyntaxErrorException
		Inherits SQLNonTransientException

			''' <summary>
			''' Constructs a <code>SQLSyntaxErrorException</code> object.
			'''  The <code>reason</code>, <code>SQLState</code> are initialized
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
			''' Constructs a <code>SQLSyntaxErrorException</code> object
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
			Public Sub New(ByVal reason As String)
					MyBase.New(reason)
			End Sub

			''' <summary>
			''' Constructs a <code>SQLSyntaxErrorException</code> object
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
			Public Sub New(ByVal reason As String, ByVal SQLState As String)
					MyBase.New(reason, SQLState)
			End Sub

			''' <summary>
			''' Constructs a <code>SQLSyntaxErrorException</code> object
			'''  with a given <code>reason</code>, <code>SQLState</code>  and
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
			Public Sub New(ByVal reason As String, ByVal SQLState As String, ByVal vendorCode As Integer)
					MyBase.New(reason, SQLState, vendorCode)
			End Sub

		''' <summary>
		''' Constructs a <code>SQLSyntaxErrorException</code> object
		'''   with a given  <code>cause</code>.
		''' The <code>SQLState</code> is initialized
		''' to <code>null</code> and the vendor code is initialized to 0.
		''' The <code>reason</code>  is initialized to <code>null</code> if
		''' <code>cause==null</code> or to <code>cause.toString()</code> if
		''' <code>cause!=null</code>.
		''' <p> </summary>
		''' <param name="cause"> the underlying reason for this <code>SQLException</code> (which is saved for later retrieval by the <code>getCause()</code> method); may be null indicating
		'''     the cause is non-existent or unknown.
		''' @since 1.6 </param>
		Public Sub New(ByVal cause As Throwable)
			MyBase.New(cause)
		End Sub

		''' <summary>
		''' Constructs a <code>SQLSyntaxErrorException</code> object
		''' with a given
		''' <code>reason</code> and  <code>cause</code>.
		''' The <code>SQLState</code> is  initialized to <code>null</code>
		''' and the vendor code is initialized to 0.
		''' <p> </summary>
		''' <param name="reason"> a description of the exception. </param>
		''' <param name="cause"> the underlying reason for this <code>SQLException</code> (which is saved for later retrieval by the <code>getCause()</code> method); may be null indicating
		'''     the cause is non-existent or unknown.
		''' @since 1.6 </param>
		Public Sub New(ByVal reason As String, ByVal cause As Throwable)
			MyBase.New(reason, cause)
		End Sub

		''' <summary>
		''' Constructs a <code>SQLSyntaxErrorException</code> object
		''' with a given
		''' <code>reason</code>, <code>SQLState</code> and  <code>cause</code>.
		''' The vendor code is initialized to 0.
		''' <p> </summary>
		''' <param name="reason"> a description of the exception. </param>
		''' <param name="SQLState"> an XOPEN or SQL:2003 code identifying the exception </param>
		''' <param name="cause"> the (which is saved for later retrieval by the <code>getCause()</code> method); may be null indicating
		'''     the cause is non-existent or unknown.
		''' @since 1.6 </param>
		Public Sub New(ByVal reason As String, ByVal SQLState As String, ByVal cause As Throwable)
			MyBase.New(reason, SQLState, cause)
		End Sub

		''' <summary>
		'''  Constructs a <code>SQLSyntaxErrorException</code> object
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
		Public Sub New(ByVal reason As String, ByVal SQLState As String, ByVal vendorCode As Integer, ByVal cause As Throwable)
			MyBase.New(reason, SQLState, vendorCode, cause)
		End Sub

		Private Shadows Const serialVersionUID As Long = -1843832610477496053L
	End Class

End Namespace