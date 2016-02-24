Imports System

'
' * Copyright (c) 1998, 2004, Oracle and/or its affiliates. All rights reserved.
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
	''' Enables <tt>org.omg.CORBA.Any</tt> values to be dynamically
	''' interpreted (traversed) and
	'''  constructed. A <tt>DynAny</tt> object is associated with a data value
	'''  which may correspond to a copy of the value inserted into an <tt>Any</tt>.
	'''  The <tt>DynAny</tt> APIs enable traversal of the data value associated with an
	'''  Any at runtime and extraction of the primitive constituents of the
	'''  data value. </summary>
	''' @deprecated Use the new <a href="../DynamicAny/DynAny.html">DynAny</a> instead 
	<Obsolete("Use the new <a href="../DynamicAny/DynAny.html">DynAny</a> instead")> _
	Public Interface DynAny
		Inherits org.omg.CORBA.Object

		''' <summary>
		''' Returns the <code>TypeCode</code> of the object inserted into
		''' this <code>DynAny</code>.
		''' </summary>
		''' <returns> the <code>TypeCode</code> object. </returns>
		Function type() As org.omg.CORBA.TypeCode

		''' <summary>
		''' Copy the contents from one Dynamic Any into another.
		''' </summary>
		''' <param name="dyn_any"> the <code>DynAny</code> object whose contents
		'''                are assigned to this <code>DynAny</code>. </param>
		''' <exception cref="Invalid"> if the source <code>DynAny</code> is
		'''            invalid </exception>
		Sub assign(ByVal dyn_any As org.omg.CORBA.DynAny)

		''' <summary>
		''' Make a <code>DynAny</code> object from an <code>Any</code>
		''' object.
		''' </summary>
		''' <param name="value"> the <code>Any</code> object. </param>
		''' <exception cref="Invalid"> if the source <code>Any</code> object is
		'''                    empty or bad </exception>
		Sub from_any(ByVal value As org.omg.CORBA.Any)

		''' <summary>
		''' Convert a <code>DynAny</code> object to an <code>Any</code>
		''' object.
		''' </summary>
		''' <returns> the <code>Any</code> object. </returns>
		''' <exception cref="Invalid"> if this <code>DynAny</code> is empty or
		'''                    bad.
		'''            created or does not contain a meaningful value </exception>
		Function to_any() As org.omg.CORBA.Any

		''' <summary>
		''' Destroys this <code>DynAny</code> object and frees any resources
		''' used to represent the data value associated with it. This method
		''' also destroys all <code>DynAny</code> objects obtained from it.
		''' <p>
		''' Destruction of <code>DynAny</code> objects should be handled with
		''' care, taking into account issues dealing with the representation of
		''' data values associated with <code>DynAny</code> objects.  A programmer
		''' who wants to destroy a <code>DynAny</code> object but still be able
		''' to manipulate some component of the data value associated with it,
		''' should first create a <code>DynAny</code> object for the component
		''' and then make a copy of the created <code>DynAny</code> object.
		''' </summary>
		Sub destroy()

		''' <summary>
		''' Clones this <code>DynAny</code> object.
		''' </summary>
		''' <returns> a copy of this <code>DynAny</code> object </returns>
		Function copy() As org.omg.CORBA.DynAny

		''' <summary>
		''' Inserts the given <code>boolean</code> as the value for this
		''' <code>DynAny</code> object.
		''' 
		''' <p> If this method is called on a constructed <code>DynAny</code>
		''' object, it initializes the next component of the constructed data
		''' value associated with this <code>DynAny</code> object.
		''' </summary>
		''' <param name="value"> the <code>boolean</code> to insert into this
		'''              <code>DynAny</code> object </param>
		''' <exception cref="org.omg.CORBA.DynAnyPackage.InvalidValue">
		'''            if the value inserted is not consistent with the type
		'''            of the accessed component in this <code>DynAny</code> object </exception>
		Sub insert_boolean(ByVal value As Boolean)

		''' <summary>
		''' Inserts the given <code>byte</code> as the value for this
		''' <code>DynAny</code> object.
		''' 
		''' <p> If this method is called on a constructed <code>DynAny</code>
		''' object, it initializes the next component of the constructed data
		''' value associated with this <code>DynAny</code> object.
		''' </summary>
		''' <param name="value"> the <code>byte</code> to insert into this
		'''              <code>DynAny</code> object </param>
		''' <exception cref="org.omg.CORBA.DynAnyPackage.InvalidValue">
		'''            if the value inserted is not consistent with the type
		'''            of the accessed component in this <code>DynAny</code> object </exception>
		Sub insert_octet(ByVal value As SByte)

		''' <summary>
		''' Inserts the given <code>char</code> as the value for this
		''' <code>DynAny</code> object.
		''' 
		''' <p> If this method is called on a constructed <code>DynAny</code>
		''' object, it initializes the next component of the constructed data
		''' value associated with this <code>DynAny</code> object.
		''' </summary>
		''' <param name="value"> the <code>char</code> to insert into this
		'''              <code>DynAny</code> object </param>
		''' <exception cref="org.omg.CORBA.DynAnyPackage.InvalidValue">
		'''            if the value inserted is not consistent with the type
		'''            of the accessed component in this <code>DynAny</code> object </exception>
		Sub insert_char(ByVal value As Char)

		''' <summary>
		''' Inserts the given <code>short</code> as the value for this
		''' <code>DynAny</code> object.
		''' 
		''' <p> If this method is called on a constructed <code>DynAny</code>
		''' object, it initializes the next component of the constructed data
		''' value associated with this <code>DynAny</code> object.
		''' </summary>
		''' <param name="value"> the <code>short</code> to insert into this
		'''              <code>DynAny</code> object </param>
		''' <exception cref="org.omg.CORBA.DynAnyPackage.InvalidValue">
		'''            if the value inserted is not consistent with the type
		'''            of the accessed component in this <code>DynAny</code> object </exception>
		Sub insert_short(ByVal value As Short)

		''' <summary>
		''' Inserts the given <code>short</code> as the value for this
		''' <code>DynAny</code> object.
		''' 
		''' <p> If this method is called on a constructed <code>DynAny</code>
		''' object, it initializes the next component of the constructed data
		''' value associated with this <code>DynAny</code> object.
		''' </summary>
		''' <param name="value"> the <code>short</code> to insert into this
		'''              <code>DynAny</code> object </param>
		''' <exception cref="org.omg.CORBA.DynAnyPackage.InvalidValue">
		'''            if the value inserted is not consistent with the type
		'''            of the accessed component in this <code>DynAny</code> object </exception>
		Sub insert_ushort(ByVal value As Short)

		''' <summary>
		''' Inserts the given <code>int</code> as the value for this
		''' <code>DynAny</code> object.
		''' 
		''' <p> If this method is called on a constructed <code>DynAny</code>
		''' object, it initializes the next component of the constructed data
		''' value associated with this <code>DynAny</code> object.
		''' </summary>
		''' <param name="value"> the <code>int</code> to insert into this
		'''              <code>DynAny</code> object </param>
		''' <exception cref="org.omg.CORBA.DynAnyPackage.InvalidValue">
		'''            if the value inserted is not consistent with the type
		'''            of the accessed component in this <code>DynAny</code> object </exception>
		Sub insert_long(ByVal value As Integer)

		''' <summary>
		''' Inserts the given <code>int</code> as the value for this
		''' <code>DynAny</code> object.
		''' 
		''' <p> If this method is called on a constructed <code>DynAny</code>
		''' object, it initializes the next component of the constructed data
		''' value associated with this <code>DynAny</code> object.
		''' </summary>
		''' <param name="value"> the <code>int</code> to insert into this
		'''              <code>DynAny</code> object </param>
		''' <exception cref="org.omg.CORBA.DynAnyPackage.InvalidValue">
		'''            if the value inserted is not consistent with the type
		'''            of the accessed component in this <code>DynAny</code> object </exception>
		Sub insert_ulong(ByVal value As Integer)

		''' <summary>
		''' Inserts the given <code>float</code> as the value for this
		''' <code>DynAny</code> object.
		''' 
		''' <p> If this method is called on a constructed <code>DynAny</code>
		''' object, it initializes the next component of the constructed data
		''' value associated with this <code>DynAny</code> object.
		''' </summary>
		''' <param name="value"> the <code>float</code> to insert into this
		'''              <code>DynAny</code> object </param>
		''' <exception cref="org.omg.CORBA.DynAnyPackage.InvalidValue">
		'''            if the value inserted is not consistent with the type
		'''            of the accessed component in this <code>DynAny</code> object </exception>
		Sub insert_float(ByVal value As Single)

		''' <summary>
		''' Inserts the given <code>double</code> as the value for this
		''' <code>DynAny</code> object.
		''' 
		''' <p> If this method is called on a constructed <code>DynAny</code>
		''' object, it initializes the next component of the constructed data
		''' value associated with this <code>DynAny</code> object.
		''' </summary>
		''' <param name="value"> the <code>double</code> to insert into this
		'''              <code>DynAny</code> object </param>
		''' <exception cref="org.omg.CORBA.DynAnyPackage.InvalidValue">
		'''            if the value inserted is not consistent with the type
		'''            of the accessed component in this <code>DynAny</code> object </exception>
		Sub insert_double(ByVal value As Double)

		''' <summary>
		''' Inserts the given <code>String</code> object as the value for this
		''' <code>DynAny</code> object.
		''' 
		''' <p> If this method is called on a constructed <code>DynAny</code>
		''' object, it initializes the next component of the constructed data
		''' value associated with this <code>DynAny</code> object.
		''' </summary>
		''' <param name="value"> the <code>String</code> to insert into this
		'''              <code>DynAny</code> object </param>
		''' <exception cref="org.omg.CORBA.DynAnyPackage.InvalidValue">
		'''            if the value inserted is not consistent with the type
		'''            of the accessed component in this <code>DynAny</code> object </exception>
		Sub insert_string(ByVal value As String)

		''' <summary>
		''' Inserts the given <code>org.omg.CORBA.Object</code> as the value for this
		''' <code>DynAny</code> object.
		''' 
		''' <p> If this method is called on a constructed <code>DynAny</code>
		''' object, it initializes the next component of the constructed data
		''' value associated with this <code>DynAny</code> object.
		''' </summary>
		''' <param name="value"> the <code>org.omg.CORBA.Object</code> to insert into this
		'''              <code>DynAny</code> object </param>
		''' <exception cref="org.omg.CORBA.DynAnyPackage.InvalidValue">
		'''            if the value inserted is not consistent with the type
		'''            of the accessed component in this <code>DynAny</code> object </exception>
		Sub insert_reference(ByVal value As org.omg.CORBA.Object)

		''' <summary>
		''' Inserts the given <code>org.omg.CORBA.TypeCode</code> as the value for this
		''' <code>DynAny</code> object.
		''' 
		''' <p> If this method is called on a constructed <code>DynAny</code>
		''' object, it initializes the next component of the constructed data
		''' value associated with this <code>DynAny</code> object.
		''' </summary>
		''' <param name="value"> the <code>org.omg.CORBA.TypeCode</code> to insert into this
		'''              <code>DynAny</code> object </param>
		''' <exception cref="org.omg.CORBA.DynAnyPackage.InvalidValue">
		'''            if the value inserted is not consistent with the type
		'''            of the accessed component in this <code>DynAny</code> object </exception>
		Sub insert_typecode(ByVal value As org.omg.CORBA.TypeCode)

		''' <summary>
		''' Inserts the given <code>long</code> as the value for this
		''' <code>DynAny</code> object.
		''' 
		''' <p> If this method is called on a constructed <code>DynAny</code>
		''' object, it initializes the next component of the constructed data
		''' value associated with this <code>DynAny</code> object.
		''' </summary>
		''' <param name="value"> the <code>long</code> to insert into this
		'''              <code>DynAny</code> object </param>
		''' <exception cref="org.omg.CORBA.DynAnyPackage.InvalidValue">
		'''            if the value inserted is not consistent with the type
		'''            of the accessed component in this <code>DynAny</code> object </exception>
		Sub insert_longlong(ByVal value As Long)

		''' <summary>
		''' Inserts the given <code>long</code> as the value for this
		''' <code>DynAny</code> object.
		''' 
		''' <p> If this method is called on a constructed <code>DynAny</code>
		''' object, it initializes the next component of the constructed data
		''' value associated with this <code>DynAny</code> object.
		''' </summary>
		''' <param name="value"> the <code>long</code> to insert into this
		'''              <code>DynAny</code> object </param>
		''' <exception cref="org.omg.CORBA.DynAnyPackage.InvalidValue">
		'''            if the value inserted is not consistent with the type
		'''            of the accessed component in this <code>DynAny</code> object </exception>
		Sub insert_ulonglong(ByVal value As Long)

		''' <summary>
		''' Inserts the given <code>char</code> as the value for this
		''' <code>DynAny</code> object.
		''' 
		''' <p> If this method is called on a constructed <code>DynAny</code>
		''' object, it initializes the next component of the constructed data
		''' value associated with this <code>DynAny</code> object.
		''' </summary>
		''' <param name="value"> the <code>char</code> to insert into this
		'''              <code>DynAny</code> object </param>
		''' <exception cref="org.omg.CORBA.DynAnyPackage.InvalidValue">
		'''            if the value inserted is not consistent with the type
		'''            of the accessed component in this <code>DynAny</code> object </exception>
		Sub insert_wchar(ByVal value As Char)

		''' <summary>
		''' Inserts the given <code>String</code> as the value for this
		''' <code>DynAny</code> object.
		''' 
		''' <p> If this method is called on a constructed <code>DynAny</code>
		''' object, it initializes the next component of the constructed data
		''' value associated with this <code>DynAny</code> object.
		''' </summary>
		''' <param name="value"> the <code>String</code> to insert into this
		'''              <code>DynAny</code> object </param>
		''' <exception cref="org.omg.CORBA.DynAnyPackage.InvalidValue">
		'''            if the value inserted is not consistent with the type
		'''            of the accessed component in this <code>DynAny</code> object </exception>
		Sub insert_wstring(ByVal value As String)

		''' <summary>
		''' Inserts the given <code>org.omg.CORBA.Any</code> object as the value for this
		''' <code>DynAny</code> object.
		''' 
		''' <p> If this method is called on a constructed <code>DynAny</code>
		''' object, it initializes the next component of the constructed data
		''' value associated with this <code>DynAny</code> object.
		''' </summary>
		''' <param name="value"> the <code>org.omg.CORBA.Any</code> object to insert into this
		'''              <code>DynAny</code> object </param>
		''' <exception cref="org.omg.CORBA.DynAnyPackage.InvalidValue">
		'''            if the value inserted is not consistent with the type
		'''            of the accessed component in this <code>DynAny</code> object </exception>
		Sub insert_any(ByVal value As org.omg.CORBA.Any)

		' orbos 98-01-18: Objects By Value -- begin

		''' <summary>
		''' Inserts the given <code>java.io.Serializable</code> object as the value for this
		''' <code>DynAny</code> object.
		''' 
		''' <p> If this method is called on a constructed <code>DynAny</code>
		''' object, it initializes the next component of the constructed data
		''' value associated with this <code>DynAny</code> object.
		''' </summary>
		''' <param name="value"> the <code>java.io.Serializable</code> object to insert into this
		'''              <code>DynAny</code> object </param>
		''' <exception cref="org.omg.CORBA.DynAnyPackage.InvalidValue">
		'''            if the value inserted is not consistent with the type
		'''            of the accessed component in this <code>DynAny</code> object </exception>
		Sub insert_val(ByVal value As java.io.Serializable)

		''' <summary>
		''' Retrieves the <code>java.io.Serializable</code> object contained
		''' in this <code>DynAny</code> object.
		''' </summary>
		''' <returns> the <code>java.io.Serializable</code> object that is the
		'''         value for this <code>DynAny</code> object </returns>
		''' <exception cref="org.omg.CORBA.DynAnyPackage.TypeMismatch">
		'''               if the type code of the accessed component in this
		'''               <code>DynAny</code> object is not equivalent to
		'''               the type code for a <code>java.io.Serializable</code> object </exception>
		Function get_val() As java.io.Serializable

		' orbos 98-01-18: Objects By Value -- end

		''' <summary>
		''' Retrieves the <code>boolean</code> contained
		''' in this <code>DynAny</code> object.
		''' </summary>
		''' <returns> the <code>boolean</code> that is the
		'''         value for this <code>DynAny</code> object </returns>
		''' <exception cref="org.omg.CORBA.DynAnyPackage.TypeMismatch">
		'''               if the type code of the accessed component in this
		'''               <code>DynAny</code> object is not equivalent to
		'''               the type code for a <code>boolean</code> </exception>
		Function get_boolean() As Boolean


		''' <summary>
		''' Retrieves the <code>byte</code> contained
		''' in this <code>DynAny</code> object.
		''' </summary>
		''' <returns> the <code>byte</code> that is the
		'''         value for this <code>DynAny</code> object </returns>
		''' <exception cref="org.omg.CORBA.DynAnyPackage.TypeMismatch">
		'''               if the type code of the accessed component in this
		'''               <code>DynAny</code> object is not equivalent to
		'''               the type code for a <code>byte</code> </exception>
		Function get_octet() As SByte

		''' <summary>
		''' Retrieves the <code>char</code> contained
		''' in this <code>DynAny</code> object.
		''' </summary>
		''' <returns> the <code>char</code> that is the
		'''         value for this <code>DynAny</code> object </returns>
		''' <exception cref="org.omg.CORBA.DynAnyPackage.TypeMismatch">
		'''               if the type code of the accessed component in this
		'''               <code>DynAny</code> object is not equivalent to
		'''               the type code for a <code>char</code> </exception>
		Function get_char() As Char


		''' <summary>
		''' Retrieves the <code>short</code> contained
		''' in this <code>DynAny</code> object.
		''' </summary>
		''' <returns> the <code>short</code> that is the
		'''         value for this <code>DynAny</code> object </returns>
		''' <exception cref="org.omg.CORBA.DynAnyPackage.TypeMismatch">
		'''               if the type code of the accessed component in this
		'''               <code>DynAny</code> object is not equivalent to
		'''               the type code for a <code>short</code> </exception>
		Function get_short() As Short


		''' <summary>
		''' Retrieves the <code>short</code> contained
		''' in this <code>DynAny</code> object.
		''' </summary>
		''' <returns> the <code>short</code> that is the
		'''         value for this <code>DynAny</code> object </returns>
		''' <exception cref="org.omg.CORBA.DynAnyPackage.TypeMismatch">
		'''               if the type code of the accessed component in this
		'''               <code>DynAny</code> object is not equivalent to
		'''               the type code for a <code>short</code> </exception>
		Function get_ushort() As Short


		''' <summary>
		''' Retrieves the <code>int</code> contained
		''' in this <code>DynAny</code> object.
		''' </summary>
		''' <returns> the <code>int</code> that is the
		'''         value for this <code>DynAny</code> object </returns>
		''' <exception cref="org.omg.CORBA.DynAnyPackage.TypeMismatch">
		'''               if the type code of the accessed component in this
		'''               <code>DynAny</code> object is not equivalent to
		'''               the type code for a <code>int</code> </exception>
		Function get_long() As Integer


		''' <summary>
		''' Retrieves the <code>int</code> contained
		''' in this <code>DynAny</code> object.
		''' </summary>
		''' <returns> the <code>int</code> that is the
		'''         value for this <code>DynAny</code> object </returns>
		''' <exception cref="org.omg.CORBA.DynAnyPackage.TypeMismatch">
		'''               if the type code of the accessed component in this
		'''               <code>DynAny</code> object is not equivalent to
		'''               the type code for a <code>int</code> </exception>
		Function get_ulong() As Integer


		''' <summary>
		''' Retrieves the <code>float</code> contained
		''' in this <code>DynAny</code> object.
		''' </summary>
		''' <returns> the <code>float</code> that is the
		'''         value for this <code>DynAny</code> object </returns>
		''' <exception cref="org.omg.CORBA.DynAnyPackage.TypeMismatch">
		'''               if the type code of the accessed component in this
		'''               <code>DynAny</code> object is not equivalent to
		'''               the type code for a <code>float</code> </exception>
		Function get_float() As Single


		''' <summary>
		''' Retrieves the <code>double</code> contained
		''' in this <code>DynAny</code> object.
		''' </summary>
		''' <returns> the <code>double</code> that is the
		'''         value for this <code>DynAny</code> object </returns>
		''' <exception cref="org.omg.CORBA.DynAnyPackage.TypeMismatch">
		'''               if the type code of the accessed component in this
		'''               <code>DynAny</code> object is not equivalent to
		'''               the type code for a <code>double</code> </exception>
		Function get_double() As Double


		''' <summary>
		''' Retrieves the <code>String</code> contained
		''' in this <code>DynAny</code> object.
		''' </summary>
		''' <returns> the <code>String</code> that is the
		'''         value for this <code>DynAny</code> object </returns>
		''' <exception cref="org.omg.CORBA.DynAnyPackage.TypeMismatch">
		'''               if the type code of the accessed component in this
		'''               <code>DynAny</code> object is not equivalent to
		'''               the type code for a <code>String</code> </exception>
		Function get_string() As String


		''' <summary>
		''' Retrieves the <code>org.omg.CORBA.Other</code> contained
		''' in this <code>DynAny</code> object.
		''' </summary>
		''' <returns> the <code>org.omg.CORBA.Other</code> that is the
		'''         value for this <code>DynAny</code> object </returns>
		''' <exception cref="org.omg.CORBA.DynAnyPackage.TypeMismatch">
		'''               if the type code of the accessed component in this
		'''               <code>DynAny</code> object is not equivalent to
		'''               the type code for an <code>org.omg.CORBA.Other</code> </exception>
		Function get_reference() As org.omg.CORBA.Object


		''' <summary>
		''' Retrieves the <code>org.omg.CORBA.TypeCode</code> contained
		''' in this <code>DynAny</code> object.
		''' </summary>
		''' <returns> the <code>org.omg.CORBA.TypeCode</code> that is the
		'''         value for this <code>DynAny</code> object </returns>
		''' <exception cref="org.omg.CORBA.DynAnyPackage.TypeMismatch">
		'''               if the type code of the accessed component in this
		'''               <code>DynAny</code> object is not equivalent to
		'''               the type code for a <code>org.omg.CORBA.TypeCode</code> </exception>
		Function get_typecode() As org.omg.CORBA.TypeCode


		''' <summary>
		''' Retrieves the <code>long</code> contained
		''' in this <code>DynAny</code> object.
		''' </summary>
		''' <returns> the <code>long</code> that is the
		'''         value for this <code>DynAny</code> object </returns>
		''' <exception cref="org.omg.CORBA.DynAnyPackage.TypeMismatch">
		'''               if the type code of the accessed component in this
		'''               <code>DynAny</code> object is not equivalent to
		'''               the type code for a <code>long</code> </exception>
		Function get_longlong() As Long


		''' <summary>
		''' Retrieves the <code>long</code> contained
		''' in this <code>DynAny</code> object.
		''' </summary>
		''' <returns> the <code>long</code> that is the
		'''         value for this <code>DynAny</code> object </returns>
		''' <exception cref="org.omg.CORBA.DynAnyPackage.TypeMismatch">
		'''               if the type code of the accessed component in this
		'''               <code>DynAny</code> object is not equivalent to
		'''               the type code for a <code>long</code> </exception>
		Function get_ulonglong() As Long


		''' <summary>
		''' Retrieves the <code>char</code> contained
		''' in this <code>DynAny</code> object.
		''' </summary>
		''' <returns> the <code>char</code> that is the
		'''         value for this <code>DynAny</code> object </returns>
		''' <exception cref="org.omg.CORBA.DynAnyPackage.TypeMismatch">
		'''               if the type code of the accessed component in this
		'''               <code>DynAny</code> object is not equivalent to
		'''               the type code for a <code>char</code> </exception>
		Function get_wchar() As Char


		''' <summary>
		''' Retrieves the <code>String</code> contained
		''' in this <code>DynAny</code> object.
		''' </summary>
		''' <returns> the <code>String</code> that is the
		'''         value for this <code>DynAny</code> object </returns>
		''' <exception cref="org.omg.CORBA.DynAnyPackage.TypeMismatch">
		'''               if the type code of the accessed component in this
		'''               <code>DynAny</code> object is not equivalent to
		'''               the type code for a <code>String</code> </exception>
		Function get_wstring() As String


		''' <summary>
		''' Retrieves the <code>org.omg.CORBA.Any</code> contained
		''' in this <code>DynAny</code> object.
		''' </summary>
		''' <returns> the <code>org.omg.CORBA.Any</code> that is the
		'''         value for this <code>DynAny</code> object </returns>
		''' <exception cref="org.omg.CORBA.DynAnyPackage.TypeMismatch">
		'''               if the type code of the accessed component in this
		'''               <code>DynAny</code> object is not equivalent to
		'''               the type code for an <code>org.omg.CORBA.Any</code> </exception>
		Function get_any() As org.omg.CORBA.Any

		''' <summary>
		''' Returns a <code>DynAny</code> object reference that can
		''' be used to get/set the value of the component currently accessed.
		''' The appropriate <code>insert</code> method
		''' can be called on the resulting <code>DynAny</code> object
		''' to initialize the component.
		''' The appropriate <code>get</code> method
		''' can be called on the resulting <code>DynAny</code> object
		''' to extract the value of the component.
		''' </summary>
		''' <returns> a <code>DynAny</code> object reference that can be
		'''         used to retrieve or set the value of the component currently
		'''         accessed </returns>
		Function current_component() As org.omg.CORBA.DynAny

		''' <summary>
		''' Moves to the next component of this <code>DynAny</code> object.
		''' This method is used for iterating through the components of
		''' a constructed type, effectively moving a pointer from one
		''' component to the next.  The pointer starts out on the first
		''' component when a <code>DynAny</code> object is created.
		''' </summary>
		''' <returns> <code>true</code> if the pointer points to a component;
		''' <code>false</code> if there are no more components or this
		''' <code>DynAny</code> is associated with a basic type rather than
		''' a constructed type </returns>
		Function [next]() As Boolean

		''' <summary>
		''' Moves the internal pointer to the given index. Logically, this method
		''' sets a new offset for this pointer.
		''' </summary>
		''' <param name="index"> an <code>int</code> indicating the position to which
		'''              the pointer should move.  The first position is 0. </param>
		''' <returns> <code>true</code> if the pointer points to a component;
		''' <code>false</code> if there is no component at the designated
		''' index.  If this <code>DynAny</code> object is associated with a
		''' basic type, this method returns <code>false</code> for any index
		''' other than 0. </returns>
		Function seek(ByVal index As Integer) As Boolean

		''' <summary>
		''' Moves the internal pointer to the first component.
		''' </summary>
		Sub rewind()
	End Interface

End Namespace