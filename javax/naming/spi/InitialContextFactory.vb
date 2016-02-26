Imports System.Collections.Generic
Imports javax.naming

'
' * Copyright (c) 1999, 2004, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.naming.spi

	''' <summary>
	''' This interface represents a factory that creates an initial context.
	''' <p>
	''' The JNDI framework allows for different initial context implementations
	''' to be specified at runtime.  The initial context is created using
	''' an <em>initial context factory</em>.
	''' An initial context factory must implement the InitialContextFactory
	''' interface, which provides a method for creating instances of initial
	''' context that implement the Context interface.
	''' In addition, the factory class must be public and must have a public
	''' constructor that accepts no arguments.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' </summary>
	''' <seealso cref= InitialContextFactoryBuilder </seealso>
	''' <seealso cref= NamingManager#getInitialContext </seealso>
	''' <seealso cref= javax.naming.InitialContext </seealso>
	''' <seealso cref= javax.naming.directory.InitialDirContext
	''' @since 1.3 </seealso>

	Public Interface InitialContextFactory
			''' <summary>
			''' Creates an Initial Context for beginning name resolution.
			''' Special requirements of this context are supplied
			''' using <code>environment</code>.
			''' <p>
			''' The environment parameter is owned by the caller.
			''' The implementation will not modify the object or keep a reference
			''' to it, although it may keep a reference to a clone or copy.
			''' </summary>
			''' <param name="environment"> The possibly null environment
			'''             specifying information to be used in the creation
			'''             of the initial context. </param>
			''' <returns> A non-null initial context object that implements the Context
			'''             interface. </returns>
			''' <exception cref="NamingException"> If cannot create an initial context. </exception>
			Function getInitialContext(Of T1)(ByVal environment As Dictionary(Of T1)) As Context
	End Interface

End Namespace