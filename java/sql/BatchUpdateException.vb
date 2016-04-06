'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' The subclass of <seealso cref="SQLException"/> thrown when an error
	''' occurs during a batch update operation.  In addition to the
	''' information provided by <seealso cref="SQLException"/>, a
	''' <code>BatchUpdateException</code> provides the update
	''' counts for all commands that were executed successfully during the
	''' batch update, that is, all commands that were executed before the error
	''' occurred.  The order of elements in an array of update counts
	''' corresponds to the order in which commands were added to the batch.
	''' <P>
	''' After a command in a batch update fails to execute properly
	''' and a <code>BatchUpdateException</code> is thrown, the driver
	''' may or may not continue to process the remaining commands in
	''' the batch.  If the driver continues processing after a failure,
	''' the array returned by the method
	''' <code>BatchUpdateException.getUpdateCounts</code> will have
	''' an element for every command in the batch rather than only
	''' elements for the commands that executed successfully before
	''' the error.  In the case where the driver continues processing
	''' commands, the array element for any command
	''' that failed is <code>Statement.EXECUTE_FAILED</code>.
	''' <P>
	''' A JDBC driver implementation should use
	''' the constructor {@code BatchUpdateException(String reason, String SQLState,
	''' int vendorCode, long []updateCounts,Throwable cause) } instead of
	''' constructors that take {@code int[]} for the update counts to avoid the
	''' possibility of overflow.
	''' <p>
	''' If {@code Statement.executeLargeBatch} method is invoked it is recommended that
	''' {@code getLargeUpdateCounts} be called instead of {@code getUpdateCounts}
	''' in order to avoid a possible overflow of the integer update count.
	''' @since 1.2
	''' </summary>

	Public Class BatchUpdateException
		Inherits SQLException

	  ''' <summary>
	  ''' Constructs a <code>BatchUpdateException</code> object initialized with a given
	  ''' <code>reason</code>, <code>SQLState</code>, <code>vendorCode</code> and
	  ''' <code>updateCounts</code>.
	  ''' The <code>cause</code> is not initialized, and may subsequently be
	  ''' initialized by a call to the
	  ''' <seealso cref="Throwable#initCause(java.lang.Throwable)"/> method.
	  ''' <p>
	  ''' <strong>Note:</strong> There is no validation of {@code updateCounts} for
	  ''' overflow and because of this it is recommended that you use the constructor
	  ''' {@code BatchUpdateException(String reason, String SQLState,
	  ''' int vendorCode, long []updateCounts,Throwable cause) }.
	  ''' </p> </summary>
	  ''' <param name="reason"> a description of the error </param>
	  ''' <param name="SQLState"> an XOPEN or SQL:2003 code identifying the exception </param>
	  ''' <param name="vendorCode"> an exception code used by a particular
	  ''' database vendor </param>
	  ''' <param name="updateCounts"> an array of <code>int</code>, with each element
	  ''' indicating the update count, <code>Statement.SUCCESS_NO_INFO</code> or
	  ''' <code>Statement.EXECUTE_FAILED</code> for each SQL command in
	  ''' the batch for JDBC drivers that continue processing
	  ''' after a command failure; an update count or
	  ''' <code>Statement.SUCCESS_NO_INFO</code> for each SQL command in the batch
	  ''' prior to the failure for JDBC drivers that stop processing after a command
	  ''' failure
	  ''' @since 1.2 </param>
	  ''' <seealso cref= #BatchUpdateException(java.lang.String, java.lang.String, int, long[],
	  ''' java.lang.Throwable) </seealso>
	  Public Sub New(  reason As String,   SQLState As String,   vendorCode As Integer,   updateCounts As Integer())
		  MyBase.New(reason, SQLState, vendorCode)
		  Me.updateCounts = If(updateCounts Is Nothing, Nothing, java.util.Arrays.copyOf(updateCounts, updateCounts.Length))
		  Me.longUpdateCounts = If(updateCounts Is Nothing, Nothing, copyUpdateCount(updateCounts))
	  End Sub

	  ''' <summary>
	  ''' Constructs a <code>BatchUpdateException</code> object initialized with a given
	  ''' <code>reason</code>, <code>SQLState</code> and
	  ''' <code>updateCounts</code>.
	  ''' The <code>cause</code> is not initialized, and may subsequently be
	  ''' initialized by a call to the
	  ''' <seealso cref="Throwable#initCause(java.lang.Throwable)"/> method. The vendor code
	  ''' is initialized to 0.
	  ''' <p>
	  ''' <strong>Note:</strong> There is no validation of {@code updateCounts} for
	  ''' overflow and because of this it is recommended that you use the constructor
	  ''' {@code BatchUpdateException(String reason, String SQLState,
	  ''' int vendorCode, long []updateCounts,Throwable cause) }.
	  ''' </p> </summary>
	  ''' <param name="reason"> a description of the exception </param>
	  ''' <param name="SQLState"> an XOPEN or SQL:2003 code identifying the exception </param>
	  ''' <param name="updateCounts"> an array of <code>int</code>, with each element
	  ''' indicating the update count, <code>Statement.SUCCESS_NO_INFO</code> or
	  ''' <code>Statement.EXECUTE_FAILED</code> for each SQL command in
	  ''' the batch for JDBC drivers that continue processing
	  ''' after a command failure; an update count or
	  ''' <code>Statement.SUCCESS_NO_INFO</code> for each SQL command in the batch
	  ''' prior to the failure for JDBC drivers that stop processing after a command
	  ''' failure
	  ''' @since 1.2 </param>
	  ''' <seealso cref= #BatchUpdateException(java.lang.String, java.lang.String, int, long[],
	  ''' java.lang.Throwable) </seealso>
	  Public Sub New(  reason As String,   SQLState As String,   updateCounts As Integer())
		  Me.New(reason, SQLState, 0, updateCounts)
	  End Sub

	  ''' <summary>
	  ''' Constructs a <code>BatchUpdateException</code> object initialized with a given
	  ''' <code>reason</code> and <code>updateCounts</code>.
	  ''' The <code>cause</code> is not initialized, and may subsequently be
	  ''' initialized by a call to the
	  ''' <seealso cref="Throwable#initCause(java.lang.Throwable)"/> method.  The
	  ''' <code>SQLState</code> is initialized to <code>null</code>
	  ''' and the vendor code is initialized to 0.
	  ''' <p>
	  ''' <strong>Note:</strong> There is no validation of {@code updateCounts} for
	  ''' overflow and because of this it is recommended that you use the constructor
	  ''' {@code BatchUpdateException(String reason, String SQLState,
	  ''' int vendorCode, long []updateCounts,Throwable cause) }.
	  ''' </p> </summary>
	  ''' <param name="reason"> a description of the exception </param>
	  ''' <param name="updateCounts"> an array of <code>int</code>, with each element
	  ''' indicating the update count, <code>Statement.SUCCESS_NO_INFO</code> or
	  ''' <code>Statement.EXECUTE_FAILED</code> for each SQL command in
	  ''' the batch for JDBC drivers that continue processing
	  ''' after a command failure; an update count or
	  ''' <code>Statement.SUCCESS_NO_INFO</code> for each SQL command in the batch
	  ''' prior to the failure for JDBC drivers that stop processing after a command
	  ''' failure
	  ''' @since 1.2 </param>
	  ''' <seealso cref= #BatchUpdateException(java.lang.String, java.lang.String, int, long[],
	  ''' java.lang.Throwable) </seealso>
	  Public Sub New(  reason As String,   updateCounts As Integer())
		  Me.New(reason, Nothing, 0, updateCounts)
	  End Sub

	  ''' <summary>
	  ''' Constructs a <code>BatchUpdateException</code> object initialized with a given
	  ''' <code>updateCounts</code>.
	  ''' initialized by a call to the
	  ''' <seealso cref="Throwable#initCause(java.lang.Throwable)"/> method. The  <code>reason</code>
	  ''' and <code>SQLState</code> are initialized to null and the vendor code
	  ''' is initialized to 0.
	  ''' <p>
	  ''' <strong>Note:</strong> There is no validation of {@code updateCounts} for
	  ''' overflow and because of this it is recommended that you use the constructor
	  ''' {@code BatchUpdateException(String reason, String SQLState,
	  ''' int vendorCode, long []updateCounts,Throwable cause) }.
	  ''' </p> </summary>
	  ''' <param name="updateCounts"> an array of <code>int</code>, with each element
	  ''' indicating the update count, <code>Statement.SUCCESS_NO_INFO</code> or
	  ''' <code>Statement.EXECUTE_FAILED</code> for each SQL command in
	  ''' the batch for JDBC drivers that continue processing
	  ''' after a command failure; an update count or
	  ''' <code>Statement.SUCCESS_NO_INFO</code> for each SQL command in the batch
	  ''' prior to the failure for JDBC drivers that stop processing after a command
	  ''' failure
	  ''' @since 1.2 </param>
	  ''' <seealso cref= #BatchUpdateException(java.lang.String, java.lang.String, int, long[],
	  ''' java.lang.Throwable) </seealso>
	  Public Sub New(  updateCounts As Integer())
		  Me.New(Nothing, Nothing, 0, updateCounts)
	  End Sub

	  ''' <summary>
	  ''' Constructs a <code>BatchUpdateException</code> object.
	  ''' The <code>reason</code>, <code>SQLState</code> and <code>updateCounts</code>
	  '''  are initialized to <code>null</code> and the vendor code is initialized to 0.
	  ''' The <code>cause</code> is not initialized, and may subsequently be
	  ''' initialized by a call to the
	  ''' <seealso cref="Throwable#initCause(java.lang.Throwable)"/> method.
	  ''' <p>
	  ''' 
	  ''' @since 1.2 </summary>
	  ''' <seealso cref= #BatchUpdateException(java.lang.String, java.lang.String, int, long[],
	  ''' java.lang.Throwable) </seealso>
	  Public Sub New()
			Me.New(Nothing, Nothing, 0, Nothing)
	  End Sub

	  ''' <summary>
	  ''' Constructs a <code>BatchUpdateException</code> object initialized with
	  '''  a given <code>cause</code>.
	  ''' The <code>SQLState</code> and <code>updateCounts</code>
	  ''' are initialized
	  ''' to <code>null</code> and the vendor code is initialized to 0.
	  ''' The <code>reason</code>  is initialized to <code>null</code> if
	  ''' <code>cause==null</code> or to <code>cause.toString()</code> if
	  '''  <code>cause!=null</code>. </summary>
	  ''' <param name="cause"> the underlying reason for this <code>SQLException</code>
	  ''' (which is saved for later retrieval by the <code>getCause()</code> method);
	  ''' may be null indicating the cause is non-existent or unknown.
	  ''' @since 1.6 </param>
	  ''' <seealso cref= #BatchUpdateException(java.lang.String, java.lang.String, int, long[],
	  ''' java.lang.Throwable) </seealso>
	  Public Sub New(  cause As Throwable)
		  Me.New((If(cause Is Nothing, Nothing, cause.ToString())), Nothing, 0, CType(Nothing, Integer()), cause)
	  End Sub

	  ''' <summary>
	  ''' Constructs a <code>BatchUpdateException</code> object initialized with a
	  ''' given <code>cause</code> and <code>updateCounts</code>.
	  ''' The <code>SQLState</code> is initialized
	  ''' to <code>null</code> and the vendor code is initialized to 0.
	  ''' The <code>reason</code>  is initialized to <code>null</code> if
	  ''' <code>cause==null</code> or to <code>cause.toString()</code> if
	  ''' <code>cause!=null</code>.
	  ''' <p>
	  ''' <strong>Note:</strong> There is no validation of {@code updateCounts} for
	  ''' overflow and because of this it is recommended that you use the constructor
	  ''' {@code BatchUpdateException(String reason, String SQLState,
	  ''' int vendorCode, long []updateCounts,Throwable cause) }.
	  ''' </p> </summary>
	  ''' <param name="updateCounts"> an array of <code>int</code>, with each element
	  ''' indicating the update count, <code>Statement.SUCCESS_NO_INFO</code> or
	  ''' <code>Statement.EXECUTE_FAILED</code> for each SQL command in
	  ''' the batch for JDBC drivers that continue processing
	  ''' after a command failure; an update count or
	  ''' <code>Statement.SUCCESS_NO_INFO</code> for each SQL command in the batch
	  ''' prior to the failure for JDBC drivers that stop processing after a command
	  ''' failure </param>
	  ''' <param name="cause"> the underlying reason for this <code>SQLException</code>
	  ''' (which is saved for later retrieval by the <code>getCause()</code> method); may be null indicating
	  ''' the cause is non-existent or unknown.
	  ''' @since 1.6 </param>
	  ''' <seealso cref= #BatchUpdateException(java.lang.String, java.lang.String, int, long[],
	  ''' java.lang.Throwable) </seealso>
	  Public Sub New(  updateCounts As Integer (),   cause As Throwable)
		  Me.New((If(cause Is Nothing, Nothing, cause.ToString())), Nothing, 0, updateCounts, cause)
	  End Sub

	  ''' <summary>
	  ''' Constructs a <code>BatchUpdateException</code> object initialized with
	  ''' a given <code>reason</code>, <code>cause</code>
	  ''' and <code>updateCounts</code>. The <code>SQLState</code> is initialized
	  ''' to <code>null</code> and the vendor code is initialized to 0.
	  ''' <p>
	  ''' <strong>Note:</strong> There is no validation of {@code updateCounts} for
	  ''' overflow and because of this it is recommended that you use the constructor
	  ''' {@code BatchUpdateException(String reason, String SQLState,
	  ''' int vendorCode, long []updateCounts,Throwable cause) }.
	  ''' </p> </summary>
	  ''' <param name="reason"> a description of the exception </param>
	  ''' <param name="updateCounts"> an array of <code>int</code>, with each element
	  ''' indicating the update count, <code>Statement.SUCCESS_NO_INFO</code> or
	  ''' <code>Statement.EXECUTE_FAILED</code> for each SQL command in
	  ''' the batch for JDBC drivers that continue processing
	  ''' after a command failure; an update count or
	  ''' <code>Statement.SUCCESS_NO_INFO</code> for each SQL command in the batch
	  ''' prior to the failure for JDBC drivers that stop processing after a command
	  ''' failure </param>
	  ''' <param name="cause"> the underlying reason for this <code>SQLException</code> (which is saved for later retrieval by the <code>getCause()</code> method);
	  ''' may be null indicating
	  ''' the cause is non-existent or unknown.
	  ''' @since 1.6 </param>
	  ''' <seealso cref= #BatchUpdateException(java.lang.String, java.lang.String, int, long[],
	  ''' java.lang.Throwable) </seealso>
	  Public Sub New(  reason As String,   updateCounts As Integer (),   cause As Throwable)
		  Me.New(reason, Nothing, 0, updateCounts, cause)
	  End Sub

	  ''' <summary>
	  ''' Constructs a <code>BatchUpdateException</code> object initialized with
	  ''' a given <code>reason</code>, <code>SQLState</code>,<code>cause</code>, and
	  ''' <code>updateCounts</code>. The vendor code is initialized to 0.
	  ''' </summary>
	  ''' <param name="reason"> a description of the exception </param>
	  ''' <param name="SQLState"> an XOPEN or SQL:2003 code identifying the exception </param>
	  ''' <param name="updateCounts"> an array of <code>int</code>, with each element
	  ''' indicating the update count, <code>Statement.SUCCESS_NO_INFO</code> or
	  ''' <code>Statement.EXECUTE_FAILED</code> for each SQL command in
	  ''' the batch for JDBC drivers that continue processing
	  ''' after a command failure; an update count or
	  ''' <code>Statement.SUCCESS_NO_INFO</code> for each SQL command in the batch
	  ''' prior to the failure for JDBC drivers that stop processing after a command
	  ''' failure
	  ''' <p>
	  ''' <strong>Note:</strong> There is no validation of {@code updateCounts} for
	  ''' overflow and because of this it is recommended that you use the constructor
	  ''' {@code BatchUpdateException(String reason, String SQLState,
	  ''' int vendorCode, long []updateCounts,Throwable cause) }.
	  ''' </p> </param>
	  ''' <param name="cause"> the underlying reason for this <code>SQLException</code>
	  ''' (which is saved for later retrieval by the <code>getCause()</code> method);
	  ''' may be null indicating
	  ''' the cause is non-existent or unknown.
	  ''' @since 1.6 </param>
	  ''' <seealso cref= #BatchUpdateException(java.lang.String, java.lang.String, int, long[],
	  ''' java.lang.Throwable) </seealso>
	  Public Sub New(  reason As String,   SQLState As String,   updateCounts As Integer (),   cause As Throwable)
		  Me.New(reason, SQLState, 0, updateCounts, cause)
	  End Sub

	  ''' <summary>
	  ''' Constructs a <code>BatchUpdateException</code> object initialized with
	  ''' a given <code>reason</code>, <code>SQLState</code>, <code>vendorCode</code>
	  ''' <code>cause</code> and <code>updateCounts</code>.
	  ''' </summary>
	  ''' <param name="reason"> a description of the error </param>
	  ''' <param name="SQLState"> an XOPEN or SQL:2003 code identifying the exception </param>
	  ''' <param name="vendorCode"> an exception code used by a particular
	  ''' database vendor </param>
	  ''' <param name="updateCounts"> an array of <code>int</code>, with each element
	  ''' indicating the update count, <code>Statement.SUCCESS_NO_INFO</code> or
	  ''' <code>Statement.EXECUTE_FAILED</code> for each SQL command in
	  ''' the batch for JDBC drivers that continue processing
	  ''' after a command failure; an update count or
	  ''' <code>Statement.SUCCESS_NO_INFO</code> for each SQL command in the batch
	  ''' prior to the failure for JDBC drivers that stop processing after a command
	  ''' failure
	  ''' <p>
	  ''' <strong>Note:</strong> There is no validation of {@code updateCounts} for
	  ''' overflow and because of this it is recommended that you use the constructor
	  ''' {@code BatchUpdateException(String reason, String SQLState,
	  ''' int vendorCode, long []updateCounts,Throwable cause) }.
	  ''' </p> </param>
	  ''' <param name="cause"> the underlying reason for this <code>SQLException</code> (which is saved for later retrieval by the <code>getCause()</code> method);
	  ''' may be null indicating
	  ''' the cause is non-existent or unknown.
	  ''' @since 1.6 </param>
	  ''' <seealso cref= #BatchUpdateException(java.lang.String, java.lang.String, int, long[],
	  ''' java.lang.Throwable) </seealso>
	  Public Sub New(  reason As String,   SQLState As String,   vendorCode As Integer,   updateCounts As Integer (),   cause As Throwable)
			MyBase.New(reason, SQLState, vendorCode, cause)
			Me.updateCounts = If(updateCounts Is Nothing, Nothing, java.util.Arrays.copyOf(updateCounts, updateCounts.Length))
			Me.longUpdateCounts = If(updateCounts Is Nothing, Nothing, copyUpdateCount(updateCounts))
	  End Sub

	  ''' <summary>
	  ''' Retrieves the update count for each update statement in the batch
	  ''' update that executed successfully before this exception occurred.
	  ''' A driver that implements batch updates may or may not continue to
	  ''' process the remaining commands in a batch when one of the commands
	  ''' fails to execute properly. If the driver continues processing commands,
	  ''' the array returned by this method will have as many elements as
	  ''' there are commands in the batch; otherwise, it will contain an
	  ''' update count for each command that executed successfully before
	  ''' the <code>BatchUpdateException</code> was thrown.
	  ''' <P>
	  ''' The possible return values for this method were modified for
	  ''' the Java 2 SDK, Standard Edition, version 1.3.  This was done to
	  ''' accommodate the new option of continuing to process commands
	  ''' in a batch update after a <code>BatchUpdateException</code> object
	  ''' has been thrown.
	  ''' </summary>
	  ''' <returns> an array of <code>int</code> containing the update counts
	  ''' for the updates that were executed successfully before this error
	  ''' occurred.  Or, if the driver continues to process commands after an
	  ''' error, one of the following for every command in the batch:
	  ''' <OL>
	  ''' <LI>an update count
	  '''  <LI><code>Statement.SUCCESS_NO_INFO</code> to indicate that the command
	  '''     executed successfully but the number of rows affected is unknown
	  '''  <LI><code>Statement.EXECUTE_FAILED</code> to indicate that the command
	  '''     failed to execute successfully
	  ''' </OL>
	  ''' @since 1.3 </returns>
	  ''' <seealso cref= #getLargeUpdateCounts() </seealso>
	  Public Overridable Property updateCounts As Integer()
		  Get
			  Return If(updateCounts Is Nothing, Nothing, java.util.Arrays.copyOf(updateCounts, updateCounts.Length))
		  End Get
	  End Property

	  ''' <summary>
	  ''' Constructs a <code>BatchUpdateException</code> object initialized with
	  ''' a given <code>reason</code>, <code>SQLState</code>, <code>vendorCode</code>
	  ''' <code>cause</code> and <code>updateCounts</code>.
	  ''' <p>
	  ''' This constructor should be used when the returned update count may exceed
	  ''' <seealso cref="Integer#MAX_VALUE"/>.
	  ''' <p> </summary>
	  ''' <param name="reason"> a description of the error </param>
	  ''' <param name="SQLState"> an XOPEN or SQL:2003 code identifying the exception </param>
	  ''' <param name="vendorCode"> an exception code used by a particular
	  ''' database vendor </param>
	  ''' <param name="updateCounts"> an array of <code>long</code>, with each element
	  ''' indicating the update count, <code>Statement.SUCCESS_NO_INFO</code> or
	  ''' <code>Statement.EXECUTE_FAILED</code> for each SQL command in
	  ''' the batch for JDBC drivers that continue processing
	  ''' after a command failure; an update count or
	  ''' <code>Statement.SUCCESS_NO_INFO</code> for each SQL command in the batch
	  ''' prior to the failure for JDBC drivers that stop processing after a command
	  ''' failure </param>
	  ''' <param name="cause"> the underlying reason for this <code>SQLException</code>
	  ''' (which is saved for later retrieval by the <code>getCause()</code> method);
	  ''' may be null indicating the cause is non-existent or unknown.
	  ''' @since 1.8 </param>
	  Public Sub New(  reason As String,   SQLState As String,   vendorCode As Integer,   updateCounts As Long (),   cause As Throwable)
		  MyBase.New(reason, SQLState, vendorCode, cause)
		  Me.longUpdateCounts = If(updateCounts Is Nothing, Nothing, java.util.Arrays.copyOf(updateCounts, updateCounts.Length))
		  Me.updateCounts = If(longUpdateCounts Is Nothing, Nothing, copyUpdateCount(longUpdateCounts))
	  End Sub

	  ''' <summary>
	  ''' Retrieves the update count for each update statement in the batch
	  ''' update that executed successfully before this exception occurred.
	  ''' A driver that implements batch updates may or may not continue to
	  ''' process the remaining commands in a batch when one of the commands
	  ''' fails to execute properly. If the driver continues processing commands,
	  ''' the array returned by this method will have as many elements as
	  ''' there are commands in the batch; otherwise, it will contain an
	  ''' update count for each command that executed successfully before
	  ''' the <code>BatchUpdateException</code> was thrown.
	  ''' <p>
	  ''' This method should be used when {@code Statement.executeLargeBatch} is
	  ''' invoked and the returned update count may exceed <seealso cref="Integer#MAX_VALUE"/>.
	  ''' <p> </summary>
	  ''' <returns> an array of <code>long</code> containing the update counts
	  ''' for the updates that were executed successfully before this error
	  ''' occurred.  Or, if the driver continues to process commands after an
	  ''' error, one of the following for every command in the batch:
	  ''' <OL>
	  ''' <LI>an update count
	  '''  <LI><code>Statement.SUCCESS_NO_INFO</code> to indicate that the command
	  '''     executed successfully but the number of rows affected is unknown
	  '''  <LI><code>Statement.EXECUTE_FAILED</code> to indicate that the command
	  '''     failed to execute successfully
	  ''' </OL>
	  ''' @since 1.8 </returns>
	  Public Overridable Property largeUpdateCounts As Long()
		  Get
			  Return If(longUpdateCounts Is Nothing, Nothing, java.util.Arrays.copyOf(longUpdateCounts, longUpdateCounts.Length))
		  End Get
	  End Property

	  ''' <summary>
	  ''' The array that describes the outcome of a batch execution.
	  ''' @serial
	  ''' @since 1.2
	  ''' </summary>
	  Private updateCounts As Integer()

	'  
	'   * Starting with Java SE 8, JDBC has added support for returning an update
	'   * count >  java.lang.[Integer].MAX_VALUE.  Because of this the following changes were made
	'   * to BatchUpdateException:
	'   * <ul>
	'   * <li>Add field longUpdateCounts</li>
	'   * <li>Add Constructorr which takes long[] for update counts</li>
	'   * <li>Add getLargeUpdateCounts method</li>
	'   * </ul>
	'   * When any of the constructors are called, the int[] and long[] updateCount
	'   * fields are populated by copying the one array to each other.
	'   *
	'   * As the JDBC driver passes in the updateCounts, there has always been the
	'   * possiblity for overflow and BatchUpdateException does not need to account
	'   * for that, it simply copies the arrays.
	'   *
	'   * JDBC drivers should always use the constructor that specifies long[] and
	'   * JDBC application developers should call getLargeUpdateCounts.
	'   

	  ''' <summary>
	  ''' The array that describes the outcome of a batch execution.
	  ''' @serial
	  ''' @since 1.8
	  ''' </summary>
	  Private longUpdateCounts As Long()

	  Private Shadows Const serialVersionUID As Long = 5977529877145521757L

	'  
	'   * Utility method to copy int[] updateCount to long[] updateCount
	'   
	  Private Shared Function copyUpdateCount(  uc As Integer()) As Long()
		  Dim copy As Long() = New Long(uc.Length - 1){}
		  For i As Integer = 0 To uc.Length - 1
			  copy(i) = uc(i)
		  Next i
		  Return copy
	  End Function

	'  
	'   * Utility method to copy long[] updateCount to int[] updateCount.
	'   * No checks for overflow will be done as it is expected a  user will call
	'   * getLargeUpdateCounts.
	'   
	  Private Shared Function copyUpdateCount(  uc As Long()) As Integer()
		  Dim copy As Integer() = New Integer(uc.Length - 1){}
		  For i As Integer = 0 To uc.Length - 1
			  copy(i) = CInt(uc(i))
		  Next i
		  Return copy
	  End Function
		''' <summary>
		''' readObject is called to restore the state of the
		''' {@code BatchUpdateException} from a stream.
		''' </summary>
		Private Sub readObject(  s As java.io.ObjectInputStream)

		   Dim fields As java.io.ObjectInputStream.GetField = s.readFields()
		   Dim tmp As Integer() = CType(fields.get("updateCounts", Nothing), Integer())
		   Dim tmp2 As Long() = CType(fields.get("longUpdateCounts", Nothing), Long())
		   If tmp IsNot Nothing AndAlso tmp2 IsNot Nothing AndAlso tmp.Length <> tmp2.Length Then Throw New java.io.InvalidObjectException("update counts are not the expected size")
		   If tmp IsNot Nothing Then updateCounts = tmp.clone()
		   If tmp2 IsNot Nothing Then longUpdateCounts = tmp2.clone()
		   If updateCounts Is Nothing AndAlso longUpdateCounts IsNot Nothing Then updateCounts = copyUpdateCount(longUpdateCounts)
		   If longUpdateCounts Is Nothing AndAlso updateCounts IsNot Nothing Then longUpdateCounts = copyUpdateCount(updateCounts)

		End Sub

		''' <summary>
		''' writeObject is called to save the state of the {@code BatchUpdateException}
		''' to a stream.
		''' </summary>
		Private Sub writeObject(  s As java.io.ObjectOutputStream)

			Dim fields As java.io.ObjectOutputStream.PutField = s.putFields()
			fields.put("updateCounts", updateCounts)
			fields.put("longUpdateCounts", longUpdateCounts)
			s.writeFields()
		End Sub
	End Class

End Namespace