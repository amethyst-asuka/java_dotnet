Imports System

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
	''' This interface represents an "intermediate context" for name resolution.
	''' <p>
	''' The Resolver interface contains methods that are implemented by contexts
	''' that do not support subtypes of Context, but which can act as
	''' intermediate contexts for resolution purposes.
	''' <p>
	''' A <tt>Name</tt> parameter passed to any method is owned
	''' by the caller.  The service provider will not modify the object
	''' or keep a reference to it.
	''' A <tt>ResolveResult</tt> object returned by any
	''' method is owned by the caller.  The caller may subsequently modify it;
	''' the service provider may not.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' @since 1.3
	''' </summary>

	Public Interface Resolver

		''' <summary>
		''' Partially resolves a name.  Stops at the first
		''' context that is an instance of a given subtype of
		''' <code>Context</code>.
		''' </summary>
		''' <param name="name">
		'''          the name to resolve </param>
		''' <param name="contextType">
		'''          the type of object to resolve.  This should
		'''          be a subtype of <code>Context</code>. </param>
		''' <returns>  the object that was found, along with the unresolved
		'''          suffix of <code>name</code>.  Cannot be null.
		''' </returns>
		''' <exception cref="javax.naming.NotContextException">
		'''          if no context of the appropriate type is found </exception>
		''' <exception cref="NamingException"> if a naming exception was encountered
		''' </exception>
		''' <seealso cref= #resolveToClass(String, Class) </seealso>
		Function resolveToClass(ByVal name As javax.naming.Name, ByVal contextType As Type) As ResolveResult

		''' <summary>
		''' Partially resolves a name.
		''' See <seealso cref="#resolveToClass(Name, Class)"/> for details.
		''' </summary>
		''' <param name="name">
		'''          the name to resolve </param>
		''' <param name="contextType">
		'''          the type of object to resolve.  This should
		'''          be a subtype of <code>Context</code>. </param>
		''' <returns>  the object that was found, along with the unresolved
		'''          suffix of <code>name</code>.  Cannot be null.
		''' </returns>
		''' <exception cref="javax.naming.NotContextException">
		'''          if no context of the appropriate type is found </exception>
		''' <exception cref="NamingException"> if a naming exception was encountered </exception>
		Function resolveToClass(ByVal name As String, ByVal contextType As Type) As ResolveResult
	End Interface

End Namespace