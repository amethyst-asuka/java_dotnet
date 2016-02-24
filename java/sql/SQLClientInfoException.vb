Imports System.Collections.Generic

'
' * Copyright (c) 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' The subclass of <seealso cref="SQLException"/> is thrown when one or more client info properties
	''' could not be set on a <code>Connection</code>.  In addition to the information provided
	''' by <code>SQLException</code>, a <code>SQLClientInfoException</code> provides a list of client info
	''' properties that were not set.
	''' 
	''' Some databases do not allow multiple client info properties to be set
	''' atomically.  For those databases, it is possible that some of the client
	''' info properties had been set even though the <code>Connection.setClientInfo</code>
	''' method threw an exception.  An application can use the <code>getFailedProperties </code>
	''' method to retrieve a list of client info properties that were not set.  The
	''' properties are identified by passing a
	''' <code>Map&lt;String,ClientInfoStatus&gt;</code> to
	''' the appropriate <code>SQLClientInfoException</code> constructor.
	''' <p> </summary>
	''' <seealso cref= ClientInfoStatus </seealso>
	''' <seealso cref= Connection#setClientInfo
	''' @since 1.6 </seealso>
	Public Class SQLClientInfoException
		Inherits SQLException




			Private failedProperties As IDictionary(Of String, ClientInfoStatus)

			''' <summary>
			''' Constructs a <code>SQLClientInfoException</code>  Object.
			''' The <code>reason</code>,
			''' <code>SQLState</code>, and failedProperties list are initialized to
			''' <code> null</code> and the vendor code is initialized to 0.
			''' The <code>cause</code> is not initialized, and may subsequently be
			''' initialized by a call to the
			''' <seealso cref="Throwable#initCause(java.lang.Throwable)"/> method.
			''' <p>
			''' 
			''' @since 1.6
			''' </summary>
			Public Sub New()

					Me.failedProperties = Nothing
			End Sub

			''' <summary>
			''' Constructs a <code>SQLClientInfoException</code> object initialized with a
			''' given <code>failedProperties</code>.
			''' The <code>reason</code> and <code>SQLState</code> are initialized
			''' to <code>null</code> and the vendor code is initialized to 0.
			''' 
			''' The <code>cause</code> is not initialized, and may subsequently be
			''' initialized by a call to the
			''' <seealso cref="Throwable#initCause(java.lang.Throwable)"/> method.
			''' <p>
			''' </summary>
			''' <param name="failedProperties">          A Map containing the property values that could not
			'''                                  be set.  The keys in the Map
			'''                                  contain the names of the client info
			'''                                  properties that could not be set and
			'''                                  the values contain one of the reason codes
			'''                                  defined in <code>ClientInfoStatus</code>
			''' <p>
			''' @since 1.6 </param>
			Public Sub New(ByVal failedProperties As IDictionary(Of String, ClientInfoStatus))

					Me.failedProperties = failedProperties
			End Sub

			''' <summary>
			''' Constructs a <code>SQLClientInfoException</code> object initialized with
			''' a given <code>cause</code> and <code>failedProperties</code>.
			''' 
			''' The <code>reason</code>  is initialized to <code>null</code> if
			''' <code>cause==null</code> or to <code>cause.toString()</code> if
			''' <code>cause!=null</code> and the vendor code is initialized to 0.
			''' 
			''' <p>
			''' </summary>
			''' <param name="failedProperties">          A Map containing the property values that could not
			'''                                  be set.  The keys in the Map
			'''                                  contain the names of the client info
			'''                                  properties that could not be set and
			'''                                  the values contain one of the reason codes
			'''                                  defined in <code>ClientInfoStatus</code> </param>
			''' <param name="cause">                                     the (which is saved for later retrieval by the <code>getCause()</code> method); may be null indicating
			'''     the cause is non-existent or unknown.
			''' <p>
			''' @since 1.6 </param>
			Public Sub New(ByVal failedProperties As IDictionary(Of String, ClientInfoStatus), ByVal cause As Throwable)

					MyBase.New(If(cause IsNot Nothing, cause.ToString(), Nothing))
					initCause(cause)
					Me.failedProperties = failedProperties
			End Sub

			''' <summary>
			''' Constructs a <code>SQLClientInfoException</code> object initialized with a
			''' given <code>reason</code> and <code>failedProperties</code>.
			''' The <code>SQLState</code> is initialized
			''' to <code>null</code> and the vendor code is initialized to 0.
			''' 
			''' The <code>cause</code> is not initialized, and may subsequently be
			''' initialized by a call to the
			''' <seealso cref="Throwable#initCause(java.lang.Throwable)"/> method.
			''' <p>
			''' </summary>
			''' <param name="reason">                            a description of the exception </param>
			''' <param name="failedProperties">          A Map containing the property values that could not
			'''                                  be set.  The keys in the Map
			'''                                  contain the names of the client info
			'''                                  properties that could not be set and
			'''                                  the values contain one of the reason codes
			'''                                  defined in <code>ClientInfoStatus</code>
			''' <p>
			''' @since 1.6 </param>
			Public Sub New(ByVal reason As String, ByVal failedProperties As IDictionary(Of String, ClientInfoStatus))

					MyBase.New(reason)
					Me.failedProperties = failedProperties
			End Sub

			''' <summary>
			''' Constructs a <code>SQLClientInfoException</code> object initialized with a
			''' given <code>reason</code>, <code>cause</code> and
			''' <code>failedProperties</code>.
			''' The  <code>SQLState</code> is initialized
			''' to <code>null</code> and the vendor code is initialized to 0.
			''' <p>
			''' </summary>
			''' <param name="reason">                            a description of the exception </param>
			''' <param name="failedProperties">          A Map containing the property values that could not
			'''                                  be set.  The keys in the Map
			'''                                  contain the names of the client info
			'''                                  properties that could not be set and
			'''                                  the values contain one of the reason codes
			'''                                  defined in <code>ClientInfoStatus</code> </param>
			''' <param name="cause">                                     the underlying reason for this <code>SQLException</code> (which is saved for later retrieval by the <code>getCause()</code> method); may be null indicating
			'''     the cause is non-existent or unknown.
			''' <p>
			''' @since 1.6 </param>
			Public Sub New(ByVal reason As String, ByVal failedProperties As IDictionary(Of String, ClientInfoStatus), ByVal cause As Throwable)

					MyBase.New(reason)
					initCause(cause)
					Me.failedProperties = failedProperties
			End Sub

			''' <summary>
			''' Constructs a <code>SQLClientInfoException</code> object initialized with a
			''' given  <code>reason</code>, <code>SQLState</code>  and
			''' <code>failedProperties</code>.
			''' The <code>cause</code> is not initialized, and may subsequently be
			''' initialized by a call to the
			''' <seealso cref="Throwable#initCause(java.lang.Throwable)"/> method. The vendor code
			''' is initialized to 0.
			''' <p>
			''' </summary>
			''' <param name="reason">                            a description of the exception </param>
			''' <param name="SQLState">                          an XOPEN or SQL:2003 code identifying the exception </param>
			''' <param name="failedProperties">          A Map containing the property values that could not
			'''                                  be set.  The keys in the Map
			'''                                  contain the names of the client info
			'''                                  properties that could not be set and
			'''                                  the values contain one of the reason codes
			'''                                  defined in <code>ClientInfoStatus</code>
			''' <p>
			''' @since 1.6 </param>
			Public Sub New(ByVal reason As String, ByVal SQLState As String, ByVal failedProperties As IDictionary(Of String, ClientInfoStatus))

					MyBase.New(reason, SQLState)
					Me.failedProperties = failedProperties
			End Sub

			''' <summary>
			''' Constructs a <code>SQLClientInfoException</code> object initialized with a
			''' given  <code>reason</code>, <code>SQLState</code>, <code>cause</code>
			''' and <code>failedProperties</code>.  The vendor code is initialized to 0.
			''' <p>
			''' </summary>
			''' <param name="reason">                            a description of the exception </param>
			''' <param name="SQLState">                          an XOPEN or SQL:2003 code identifying the exception </param>
			''' <param name="failedProperties">          A Map containing the property values that could not
			'''                                  be set.  The keys in the Map
			'''                                  contain the names of the client info
			'''                                  properties that could not be set and
			'''                                  the values contain one of the reason codes
			'''                                  defined in <code>ClientInfoStatus</code> </param>
			''' <param name="cause">                                     the underlying reason for this <code>SQLException</code> (which is saved for later retrieval by the <code>getCause()</code> method); may be null indicating
			'''     the cause is non-existent or unknown.
			''' <p>
			''' @since 1.6 </param>
			Public Sub New(ByVal reason As String, ByVal SQLState As String, ByVal failedProperties As IDictionary(Of String, ClientInfoStatus), ByVal cause As Throwable)

					MyBase.New(reason, SQLState)
					initCause(cause)
					Me.failedProperties = failedProperties
			End Sub

			''' <summary>
			''' Constructs a <code>SQLClientInfoException</code> object initialized with a
			''' given  <code>reason</code>, <code>SQLState</code>,
			''' <code>vendorCode</code>  and <code>failedProperties</code>.
			''' The <code>cause</code> is not initialized, and may subsequently be
			''' initialized by a call to the
			''' <seealso cref="Throwable#initCause(java.lang.Throwable)"/> method.
			''' <p>
			''' </summary>
			''' <param name="reason">                            a description of the exception </param>
			''' <param name="SQLState">                          an XOPEN or SQL:2003 code identifying the exception </param>
			''' <param name="vendorCode">                        a database vendor-specific exception code </param>
			''' <param name="failedProperties">          A Map containing the property values that could not
			'''                                  be set.  The keys in the Map
			'''                                  contain the names of the client info
			'''                                  properties that could not be set and
			'''                                  the values contain one of the reason codes
			'''                                  defined in <code>ClientInfoStatus</code>
			''' <p>
			''' @since 1.6 </param>
			Public Sub New(ByVal reason As String, ByVal SQLState As String, ByVal vendorCode As Integer, ByVal failedProperties As IDictionary(Of String, ClientInfoStatus))

					MyBase.New(reason, SQLState, vendorCode)
					Me.failedProperties = failedProperties
			End Sub

			''' <summary>
			''' Constructs a <code>SQLClientInfoException</code> object initialized with a
			''' given  <code>reason</code>, <code>SQLState</code>,
			''' <code>cause</code>, <code>vendorCode</code> and
			''' <code>failedProperties</code>.
			''' <p>
			''' </summary>
			''' <param name="reason">                            a description of the exception </param>
			''' <param name="SQLState">                          an XOPEN or SQL:2003 code identifying the exception </param>
			''' <param name="vendorCode">                        a database vendor-specific exception code </param>
			''' <param name="failedProperties">          A Map containing the property values that could not
			'''                                  be set.  The keys in the Map
			'''                                  contain the names of the client info
			'''                                  properties that could not be set and
			'''                                  the values contain one of the reason codes
			'''                                  defined in <code>ClientInfoStatus</code> </param>
			''' <param name="cause">                     the underlying reason for this <code>SQLException</code> (which is saved for later retrieval by the <code>getCause()</code> method); may be null indicating
			'''     the cause is non-existent or unknown.
			''' <p>
			''' @since 1.6 </param>
			Public Sub New(ByVal reason As String, ByVal SQLState As String, ByVal vendorCode As Integer, ByVal failedProperties As IDictionary(Of String, ClientInfoStatus), ByVal cause As Throwable)

					MyBase.New(reason, SQLState, vendorCode)
					initCause(cause)
					Me.failedProperties = failedProperties
			End Sub

		''' <summary>
		''' Returns the list of client info properties that could not be set.  The
		''' keys in the Map  contain the names of the client info
		''' properties that could not be set and the values contain one of the
		''' reason codes defined in <code>ClientInfoStatus</code>
		''' <p>
		''' </summary>
		''' <returns> Map list containing the client info properties that could
		''' not be set
		''' <p>
		''' @since 1.6 </returns>
			Public Overridable Property failedProperties As IDictionary(Of String, ClientInfoStatus)
				Get
    
						Return Me.failedProperties
				End Get
			End Property

		Private Shadows Const serialVersionUID As Long = -4319604256824655880L
	End Class

End Namespace