Imports System

'
' * Copyright (c) 1996, 1999, Oracle and/or its affiliates. All rights reserved.
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

Namespace org.omg.CORBA

	''' <summary>
	''' A container (holder) for an exception that is used in <code>Request</code>
	''' operations to make exceptions available to the client.  An
	''' <code>Environment</code> object is created with the <code>ORB</code>
	''' method <code>create_environment</code>.
	''' 
	''' @since   JDK1.2
	''' </summary>

	Public MustInherit Class Environment

		''' <summary>
		''' Retrieves the exception in this <code>Environment</code> object.
		''' </summary>
		''' <returns>                  the exception in this <code>Environment</code> object </returns>

		Public MustOverride Function exception() As Exception

		''' <summary>
		''' Inserts the given exception into this <code>Environment</code> object.
		''' </summary>
		''' <param name="except">            the exception to be set </param>

		Public MustOverride Sub exception(ByVal except As Exception)

		''' <summary>
		''' Clears this <code>Environment</code> object of its exception.
		''' </summary>

		Public MustOverride Sub clear()

	End Class

End Namespace