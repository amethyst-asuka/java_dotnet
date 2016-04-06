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
	''' <P>An exception that provides information on  database access
	''' warnings. Warnings are silently chained to the object whose method
	''' caused it to be reported.
	''' <P>
	''' Warnings may be retrieved from <code>Connection</code>, <code>Statement</code>,
	''' and <code>ResultSet</code> objects.  Trying to retrieve a warning on a
	''' connection after it has been closed will cause an exception to be thrown.
	''' Similarly, trying to retrieve a warning on a statement after it has been
	''' closed or on a result set after it has been closed will cause
	''' an exception to be thrown. Note that closing a statement also
	''' closes a result set that it might have produced.
	''' </summary>
	''' <seealso cref= Connection#getWarnings </seealso>
	''' <seealso cref= Statement#getWarnings </seealso>
	''' <seealso cref= ResultSet#getWarnings </seealso>
	Public Class SQLWarning
		Inherits SQLException

		''' <summary>
		''' Constructs a  <code>SQLWarning</code> object
		'''  with a given <code>reason</code>, <code>SQLState</code>  and
		''' <code>vendorCode</code>.
		''' 
		''' The <code>cause</code> is not initialized, and may subsequently be
		''' initialized by a call to the
		''' <seealso cref="Throwable#initCause(java.lang.Throwable)"/> method.
		''' <p> </summary>
		''' <param name="reason"> a description of the warning </param>
		''' <param name="SQLState"> an XOPEN or SQL:2003 code identifying the warning </param>
		''' <param name="vendorCode"> a database vendor-specific warning code </param>
		 Public Sub New(  reason As String,   SQLState As String,   vendorCode As Integer)
			MyBase.New(reason, SQLState, vendorCode)
			DriverManager.println("SQLWarning: reason(" & reason & ") SQLState(" & SQLState & ") vendor code(" & vendorCode & ")")
		 End Sub


		''' <summary>
		''' Constructs a <code>SQLWarning</code> object
		''' with a given <code>reason</code> and <code>SQLState</code>.
		''' 
		''' The <code>cause</code> is not initialized, and may subsequently be
		''' initialized by a call to the
		''' <seealso cref="Throwable#initCause(java.lang.Throwable)"/> method. The vendor code
		''' is initialized to 0.
		''' <p> </summary>
		''' <param name="reason"> a description of the warning </param>
		''' <param name="SQLState"> an XOPEN or SQL:2003 code identifying the warning </param>
		Public Sub New(  reason As String,   SQLState As String)
			MyBase.New(reason, SQLState)
			DriverManager.println("SQLWarning: reason(" & reason & ") SQLState(" & SQLState & ")")
		End Sub

		''' <summary>
		''' Constructs a <code>SQLWarning</code> object
		''' with a given <code>reason</code>. The <code>SQLState</code>
		''' is initialized to <code>null</code> and the vendor code is initialized
		''' to 0.
		''' 
		''' The <code>cause</code> is not initialized, and may subsequently be
		''' initialized by a call to the
		''' <seealso cref="Throwable#initCause(java.lang.Throwable)"/> method.
		''' <p> </summary>
		''' <param name="reason"> a description of the warning </param>
		Public Sub New(  reason As String)
			MyBase.New(reason)
			DriverManager.println("SQLWarning: reason(" & reason & ")")
		End Sub

		''' <summary>
		''' Constructs a  <code>SQLWarning</code> object.
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
			DriverManager.println("SQLWarning: ")
		End Sub

		''' <summary>
		''' Constructs a <code>SQLWarning</code> object
		''' with a given  <code>cause</code>.
		''' The <code>SQLState</code> is initialized
		''' to <code>null</code> and the vendor code is initialized to 0.
		''' The <code>reason</code>  is initialized to <code>null</code> if
		''' <code>cause==null</code> or to <code>cause.toString()</code> if
		''' <code>cause!=null</code>.
		''' <p> </summary>
		''' <param name="cause"> the underlying reason for this <code>SQLWarning</code> (which is saved for later retrieval by the <code>getCause()</code> method); may be null indicating
		'''     the cause is non-existent or unknown. </param>
		Public Sub New(  cause As Throwable)
			MyBase.New(cause)
			DriverManager.println("SQLWarning")
		End Sub

		''' <summary>
		''' Constructs a <code>SQLWarning</code> object
		''' with a given
		''' <code>reason</code> and  <code>cause</code>.
		''' The <code>SQLState</code> is  initialized to <code>null</code>
		''' and the vendor code is initialized to 0.
		''' <p> </summary>
		''' <param name="reason"> a description of the warning </param>
		''' <param name="cause">  the underlying reason for this <code>SQLWarning</code>
		''' (which is saved for later retrieval by the <code>getCause()</code> method);
		''' may be null indicating the cause is non-existent or unknown. </param>
		Public Sub New(  reason As String,   cause As Throwable)
			MyBase.New(reason,cause)
			DriverManager.println("SQLWarning : reason(" & reason & ")")
		End Sub

		''' <summary>
		''' Constructs a <code>SQLWarning</code> object
		''' with a given
		''' <code>reason</code>, <code>SQLState</code> and  <code>cause</code>.
		''' The vendor code is initialized to 0.
		''' <p> </summary>
		''' <param name="reason"> a description of the warning </param>
		''' <param name="SQLState"> an XOPEN or SQL:2003 code identifying the warning </param>
		''' <param name="cause"> the underlying reason for this <code>SQLWarning</code> (which is saved for later retrieval by the <code>getCause()</code> method); may be null indicating
		'''     the cause is non-existent or unknown. </param>
		Public Sub New(  reason As String,   SQLState As String,   cause As Throwable)
			MyBase.New(reason,SQLState,cause)
			DriverManager.println("SQLWarning: reason(" & reason & ") SQLState(" & SQLState & ")")
		End Sub

		''' <summary>
		''' Constructs a<code>SQLWarning</code> object
		''' with a given
		''' <code>reason</code>, <code>SQLState</code>, <code>vendorCode</code>
		''' and  <code>cause</code>.
		''' <p> </summary>
		''' <param name="reason"> a description of the warning </param>
		''' <param name="SQLState"> an XOPEN or SQL:2003 code identifying the warning </param>
		''' <param name="vendorCode"> a database vendor-specific warning code </param>
		''' <param name="cause"> the underlying reason for this <code>SQLWarning</code> (which is saved for later retrieval by the <code>getCause()</code> method); may be null indicating
		'''     the cause is non-existent or unknown. </param>
		Public Sub New(  reason As String,   SQLState As String,   vendorCode As Integer,   cause As Throwable)
			MyBase.New(reason,SQLState,vendorCode,cause)
			DriverManager.println("SQLWarning: reason(" & reason & ") SQLState(" & SQLState & ") vendor code(" & vendorCode & ")")

		End Sub
		''' <summary>
		''' Retrieves the warning chained to this <code>SQLWarning</code> object by
		''' <code>setNextWarning</code>.
		''' </summary>
		''' <returns> the next <code>SQLException</code> in the chain; <code>null</code> if none </returns>
		''' <seealso cref= #setNextWarning </seealso>
		Public Overridable Property nextWarning As SQLWarning
			Get
				Try
					Return (CType(nextException, SQLWarning))
				Catch ex As  ClassCastException
					' The chained value isn't a SQLWarning.
					' This is a programming error by whoever added it to
					' the SQLWarning chain.  We throw a Java "Error".
					Throw New [Error]("SQLWarning chain holds value that is not a SQLWarning")
				End Try
			End Get
			Set(  w As SQLWarning)
				nextException = w
			End Set
		End Property


		Private Shadows Const serialVersionUID As Long = 3917336774604784856L
	End Class

End Namespace