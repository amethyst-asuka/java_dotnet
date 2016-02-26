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

'
' * Created on Apr 28, 2005
' 
Namespace javax.sql


	''' <summary>
	''' A <code>StatementEvent</code> is sent to all <code>StatementEventListener</code>s which were
	''' registered with a <code>PooledConnection</code>. This occurs when the driver determines that a
	''' <code>PreparedStatement</code> that is associated with the <code>PooledConnection</code> has been closed or the driver determines
	''' is invalid.
	''' <p>
	''' @since 1.6
	''' </summary>
	Public Class StatementEvent
		Inherits java.util.EventObject

			Friend Const serialVersionUID As Long = -8089573731826608315L
			Private exception As java.sql.SQLException
			Private statement As java.sql.PreparedStatement

			''' <summary>
			''' Constructs a <code>StatementEvent</code> with the specified <code>PooledConnection</code> and
			''' <code>PreparedStatement</code>.  The <code>SQLException</code> contained in the event defaults to
			''' null.
			''' <p> </summary>
			''' <param name="con">                   The <code>PooledConnection</code> that the closed or invalid
			''' <code>PreparedStatement</code>is associated with. </param>
			''' <param name="statement">             The <code>PreparedStatement</code> that is being closed or is invalid
			''' <p> </param>
			''' <exception cref="IllegalArgumentException"> if <code>con</code> is null.
			''' 
			''' @since 1.6 </exception>
			Public Sub New(ByVal con As PooledConnection, ByVal statement As java.sql.PreparedStatement)

					MyBase.New(con)

					Me.statement = statement
					Me.exception = Nothing
			End Sub

			''' <summary>
			''' Constructs a <code>StatementEvent</code> with the specified <code>PooledConnection</code>,
			''' <code>PreparedStatement</code> and <code>SQLException</code>
			''' <p> </summary>
			''' <param name="con">                   The <code>PooledConnection</code> that the closed or invalid <code>PreparedStatement</code>
			''' is associated with. </param>
			''' <param name="statement">             The <code>PreparedStatement</code> that is being closed or is invalid </param>
			''' <param name="exception">             The <code>SQLException </code>the driver is about to throw to
			'''                                              the application
			''' </param>
			''' <exception cref="IllegalArgumentException"> if <code>con</code> is null.
			''' <p>
			''' @since 1.6 </exception>
			Public Sub New(ByVal con As PooledConnection, ByVal statement As java.sql.PreparedStatement, ByVal exception As java.sql.SQLException)

					MyBase.New(con)

					Me.statement = statement
					Me.exception = exception
			End Sub

			''' <summary>
			''' Returns the <code>PreparedStatement</code> that is being closed or is invalid
			''' <p> </summary>
			''' <returns>      The <code>PreparedStatement</code> that is being closed or is invalid
			''' <p>
			''' @since 1.6 </returns>
			Public Overridable Property statement As java.sql.PreparedStatement
				Get
    
						Return Me.statement
				End Get
			End Property

			''' <summary>
			''' Returns the <code>SQLException</code> the driver is about to throw
			''' <p> </summary>
			''' <returns>      The <code>SQLException</code> the driver is about to throw
			''' <p>
			''' @since 1.6 </returns>
			Public Overridable Property sQLException As java.sql.SQLException
				Get
    
						Return Me.exception
				End Get
			End Property
	End Class

End Namespace