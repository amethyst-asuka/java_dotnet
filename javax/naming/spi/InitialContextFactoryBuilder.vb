Imports System.Collections.Generic

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
	''' This interface represents a builder that creates initial context factories.
	''' <p>
	''' The JNDI framework allows for different initial context implementations
	''' to be specified at runtime.  An initial context is created using
	''' an initial context factory. A program can install its own builder
	''' that creates initial context factories, thereby overriding the
	''' default policies used by the framework, by calling
	''' NamingManager.setInitialContextFactoryBuilder().
	''' The InitialContextFactoryBuilder interface must be implemented by
	''' such a builder.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' </summary>
	''' <seealso cref= InitialContextFactory </seealso>
	''' <seealso cref= NamingManager#getInitialContext </seealso>
	''' <seealso cref= NamingManager#setInitialContextFactoryBuilder </seealso>
	''' <seealso cref= NamingManager#hasInitialContextFactoryBuilder </seealso>
	''' <seealso cref= javax.naming.InitialContext </seealso>
	''' <seealso cref= javax.naming.directory.InitialDirContext
	''' @since 1.3 </seealso>
	Public Interface InitialContextFactoryBuilder
		''' <summary>
		''' Creates an initial context factory using the specified
		''' environment.
		''' <p>
		''' The environment parameter is owned by the caller.
		''' The implementation will not modify the object or keep a reference
		''' to it, although it may keep a reference to a clone or copy.
		''' </summary>
		''' <param name="environment"> Environment used in creating an initial
		'''                 context implementation. Can be null. </param>
		''' <returns> A non-null initial context factory. </returns>
		''' <exception cref="NamingException"> If an initial context factory could not be created. </exception>
		Function createInitialContextFactory(Of T1)(ByVal environment As Dictionary(Of T1)) As InitialContextFactory
	End Interface

End Namespace