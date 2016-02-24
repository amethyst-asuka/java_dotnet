'
' * Copyright (c) 1996, 2000, Oracle and/or its affiliates. All rights reserved.
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
	''' A modifiable list containing <code>NamedValue</code> objects.
	''' <P>
	''' The class <code>NVList</code> is used as follows:
	''' <UL>
	''' <LI>to describe arguments for a <code>Request</code> object
	''' in the Dynamic Invocation Interface and
	''' the Dynamic Skeleton Interface
	''' <LI>to describe context values in a <code>Context</code> object
	''' </UL>
	''' <P>
	''' Each <code>NamedValue</code> object consists of the following:
	''' <UL>
	''' <LI>a name, which is a <code>String</code> object
	''' <LI>a value, as an <code>Any</code> object
	''' <LI>an argument mode flag
	''' </UL>
	''' <P>
	''' An <code>NVList</code> object
	''' may be created using one of the following
	''' <code>ORB</code> methods:
	''' <OL>
	''' <LI><code>org.omg.CORBA.ORB.create_list</code>
	''' <PRE>
	'''    org.omg.CORBA.NVList nv = orb.create_list(3);
	''' </PRE>
	''' The variable <code>nv</code> represents a newly-created
	''' <code>NVList</code> object.  The argument is a memory-management
	''' hint to the orb and does not imply the actual length of the list.
	''' If, for example, you want to use an <code>NVList</code> object
	''' in a request, and the method being invoked takes three parameters,
	''' you might optimize by supplying 3 to the method
	''' <code>create_list</code>.  Note that the new <code>NVList</code>
	''' will not necessarily have a length of 3; it
	''' could have a length of 2 or 4, for instance.
	''' Note also that you can add any number of
	''' <code>NamedValue</code> objects to this list regardless of
	''' its original length.
	''' <P>
	''' <LI><code>org.omg.CORBA.ORB.create_operation_list</code>
	''' <PRE>
	'''    org.omg.CORBA.NVList nv = orb.create_operation_list(myOperationDef);
	''' </PRE>
	''' The variable <code>nv</code> represents a newly-created
	''' <code>NVList</code> object that contains descriptions of the
	''' arguments to the method described in the given
	''' <code>OperationDef</code> object.
	''' </OL>
	''' <P>
	''' The methods in the class <code>NVList</code> all deal with
	''' the <code>NamedValue</code> objects in the list.
	''' There are three methods for adding a <code>NamedValue</code> object,
	''' a method for getting the count of <code>NamedValue</code> objects in
	''' the list, a method for retrieving a <code>NamedValue</code> object
	''' at a given index, and a method for removing a <code>NamedValue</code> object
	''' at a given index.
	''' </summary>
	''' <seealso cref= org.omg.CORBA.Request </seealso>
	''' <seealso cref= org.omg.CORBA.ServerRequest </seealso>
	''' <seealso cref= org.omg.CORBA.NamedValue </seealso>
	''' <seealso cref= org.omg.CORBA.Context
	''' 
	''' @since       JDK1.2 </seealso>

	Public MustInherit Class NVList

		''' <summary>
		''' Returns the number of <code>NamedValue</code> objects that have
		''' been added to this <code>NVList</code> object.
		''' </summary>
		''' <returns>                  an <code>int</code> indicating the number of
		''' <code>NamedValue</code> objects in this <code>NVList</code>. </returns>

		Public MustOverride Function count() As Integer

		''' <summary>
		''' Creates a new <code>NamedValue</code> object initialized with the given flag
		''' and adds it to the end of this <code>NVList</code> object.
		''' The flag can be any one of the argument passing modes:
		''' <code>ARG_IN.value</code>, <code>ARG_OUT.value</code>, or
		''' <code>ARG_INOUT.value</code>.
		''' </summary>
		''' <param name="flags">             one of the argument mode flags </param>
		''' <returns>                  the newly-created <code>NamedValue</code> object </returns>

		Public MustOverride Function add(ByVal flags As Integer) As NamedValue

		''' <summary>
		''' Creates a new <code>NamedValue</code> object initialized with the
		''' given name and flag,
		''' and adds it to the end of this <code>NVList</code> object.
		''' The flag can be any one of the argument passing modes:
		''' <code>ARG_IN.value</code>, <code>ARG_OUT.value</code>, or
		''' <code>ARG_INOUT.value</code>.
		''' </summary>
		''' <param name="item_name"> the name for the new <code>NamedValue</code> object </param>
		''' <param name="flags">             one of the argument mode flags </param>
		''' <returns>                  the newly-created <code>NamedValue</code> object </returns>

		Public MustOverride Function add_item(ByVal item_name As String, ByVal flags As Integer) As NamedValue

		''' <summary>
		''' Creates a new <code>NamedValue</code> object initialized with the
		''' given name, value, and flag,
		''' and adds it to the end of this <code>NVList</code> object.
		''' </summary>
		''' <param name="item_name"> the name for the new <code>NamedValue</code> object </param>
		''' <param name="val">         an <code>Any</code> object containing the  value
		'''                    for the new <code>NamedValue</code> object </param>
		''' <param name="flags">       one of the following argument passing modes:
		'''                    <code>ARG_IN.value</code>, <code>ARG_OUT.value</code>, or
		'''                    <code>ARG_INOUT.value</code> </param>
		''' <returns>            the newly created <code>NamedValue</code> object </returns>

		Public MustOverride Function add_value(ByVal item_name As String, ByVal val As Any, ByVal flags As Integer) As NamedValue

		''' <summary>
		''' Retrieves the <code>NamedValue</code> object at the given index.
		''' </summary>
		''' <param name="index">             the index of the desired <code>NamedValue</code> object,
		'''                    which must be between zero and the length of the list
		'''                    minus one, inclusive.  The first item is at index zero. </param>
		''' <returns>                  the <code>NamedValue</code> object at the given index </returns>
		''' <exception cref="org.omg.CORBA.Bounds">  if the index is greater than
		'''                          or equal to number of <code>NamedValue</code> objects </exception>

		Public MustOverride Function item(ByVal index As Integer) As NamedValue

		''' <summary>
		''' Removes the <code>NamedValue</code> object at the given index.
		''' Note that the indices of all <code>NamedValue</code> objects following
		''' the one removed are shifted down by one.
		''' </summary>
		''' <param name="index">             the index of the <code>NamedValue</code> object to be
		'''                    removed, which must be between zero and the length
		'''                    of the list minus one, inclusive.
		'''                    The first item is at index zero. </param>
		''' <exception cref="org.omg.CORBA.Bounds">  if the index is greater than
		'''                          or equal to number of <code>NamedValue</code> objects in
		'''                the list </exception>

		Public MustOverride Sub remove(ByVal index As Integer)

	End Class

End Namespace