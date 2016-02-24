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
	''' An object containing a modifiable list of <code>String</code> objects
	''' that represent property names.
	''' This class is used in <code>Request</code> operations to
	''' describe the contexts that need to be resolved and sent with the
	''' invocation.  (A context is resolved by giving a property name
	''' and getting back the value associated with it.)  This is done
	''' by calling the <code>Context</code> method
	''' <code>get_values</code> and supplying a string from a
	''' <code>ContextList</code> object as the third parameter.
	''' The method <code>get_values</code> returns an <code>NVList</code>
	''' object containing the <code>NamedValue</code> objects that hold
	''' the value(s) identified by the given string.
	''' <P>
	''' A <code>ContextList</code> object is created by the ORB, as
	''' illustrated here:
	''' <PRE>
	'''   ORB orb = ORB.init(args, null);
	'''   org.omg.CORBA.ContextList ctxList = orb.create_context_list();
	''' </PRE>
	''' The variable <code>ctxList</code> represents an empty
	''' <code>ContextList</code> object.  Strings are added to
	''' the list with the method <code>add</code>, accessed
	''' with the method <code>item</code>, and removed with the
	''' method <code>remove</code>.
	''' </summary>
	''' <seealso cref= Context
	''' @since   JDK1.2 </seealso>

	Public MustInherit Class ContextList

		''' <summary>
		''' Returns the number of <code>String</code> objects in this
		''' <code>ContextList</code> object.
		''' </summary>
		''' <returns>                  an <code>int</code> representing the number of
		''' <code>String</code>s in this <code>ContextList</code> object </returns>

		Public MustOverride Function count() As Integer

		''' <summary>
		''' Adds a <code>String</code> object to this <code>ContextList</code>
		''' object.
		''' </summary>
		''' <param name="ctx">               the <code>String</code> object to be added </param>

		Public MustOverride Sub add(ByVal ctx As String)

		''' <summary>
		''' Returns the <code>String</code> object at the given index.
		''' </summary>
		''' <param name="index">             the index of the string desired, with 0 being the
		''' index of the first string </param>
		''' <returns>                  the string at the given index </returns>
		''' <exception cref="org.omg.CORBA.Bounds">  if the index is greater than
		'''                          or equal to the number of strings in this
		'''                <code>ContextList</code> object </exception>

		Public MustOverride Function item(ByVal index As Integer) As String

		''' <summary>
		''' Removes the <code>String</code> object at the given index. Note that
		''' the indices of all strings following the one removed are
		''' shifted down by one.
		''' </summary>
		''' <param name="index">     the index of the <code>String</code> object to be removed,
		'''                with 0 designating the first string </param>
		''' <exception cref="org.omg.CORBA.Bounds">  if the index is greater than
		'''                          or equal to the number of <code>String</code> objects in
		'''                this <code>ContextList</code> object </exception>

		Public MustOverride Sub remove(ByVal index As Integer)

	End Class

End Namespace