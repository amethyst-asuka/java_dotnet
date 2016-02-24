Imports System

'
' * Copyright (c) 1995, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' The root class for CORBA IDL-defined user exceptions.
	''' All CORBA user exceptions are checked exceptions, which
	''' means that they need to
	''' be declared in method signatures.
	''' </summary>
	''' <seealso cref= <A href="../../../../technotes/guides/idl/jidlExceptions.html">documentation on
	''' Java&nbsp;IDL exceptions</A> </seealso>
	Public MustInherit Class UserException
		Inherits Exception
		Implements org.omg.CORBA.portable.IDLEntity

		''' <summary>
		''' Constructs a <code>UserException</code> object.
		''' This method is called only by subclasses.
		''' </summary>
		Protected Friend Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a <code>UserException</code> object with a
		''' detail message. This method is called only by subclasses.
		''' </summary>
		''' <param name="reason"> a <code>String</code> object giving the reason for this
		'''         exception </param>
		Protected Friend Sub New(ByVal reason As String)
			MyBase.New(reason)
		End Sub
	End Class

End Namespace