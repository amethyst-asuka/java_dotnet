Imports org.omg.CORBA.TypeCodePackage

'
' * Copyright (c) 1996, 2003, Oracle and/or its affiliates. All rights reserved.
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
	''' A container for information about a specific CORBA data
	''' type.
	''' <P>
	''' <code>TypeCode</code> objects are used:
	''' <UL>
	''' <LI>in the Dynamic Invocation Interface -- to indicate the types
	''' of the actual arguments or the type of the return value.  <BR>
	''' <code>NamedValue</code> objects are used to represent arguments and
	''' return values.  One of their components is an <code>Any</code>
	''' object, which in turn has as one of its components a
	''' <code>TypeCode</code> object.
	''' <LI>by an Interface Repository to represent the type specifications
	''' that are part of many OMG IDL declarations
	''' </UL>
	''' <P>
	''' The representation of a <code>TypeCode</code> object is opaque,
	''' but abstractly, a <code>TypeCode</code> object consists of:
	''' <UL>
	''' <LI>a <code>kind</code> field, which is set to an instance
	''' of the class <code>TCKind</code>
	''' <LI>zero or more additional fields appropriate
	''' for the particular kind. For example, the
	''' <code>TypeCode</code> object
	''' describing the OMG IDL type <code>1ong</code> has kind
	''' <code>TCKind.tk_long</code> and no additional fields.
	''' The <code>TypeCode</code> describing OMG IDL type
	''' <code>sequence&lt;boolean, 10&gt;</code> has a <code>kind</code> field
	''' with the value
	''' <code>TCKind.tk_sequence</code> and also fields with the values
	''' <code>boolean</code> and <code>10</code> for the
	''' type of sequence elements and the length of the sequence. <p>
	''' </UL>
	''' 
	''' <code>TypeCode</code> objects can be obtained in various ways:
	''' <OL>
	''' <LI>from a call to the method <code>Any.insert_X</code>, where X is
	''' a basic IDL type.  This method creates a <code>TypeCode</code> object
	''' for type X and assigns it to the <code>Any</code> object's
	''' <code>type</code> field.
	''' <LI>from invocations of methods in the ORB class
	''' <P>For example, the following creates a <code>TypeCode</code>
	''' object for a <code>string</code> with a maximum of 30 characters:
	''' <PRE>
	'''   org.omg.CORBA.TypeCode tcString = orb.create_string_tc(30);
	''' </PRE>
	''' <P> The following creates a <code>TypeCode</code>
	''' object for an <code>array</code> of five <code>string</code>s:
	''' <PRE>
	'''   org.omg.CORBA.TypeCode tcArray = orb.create_array_tc(
	'''                                       5, TCKind.tk_string);
	''' </PRE>
	''' <P> The following creates a <code>TypeCode</code>
	''' object for an interface named "Account":
	''' <PRE>
	'''   org.omg.CORBA.TypeCode tcInterface = orb.create_interface_tc(
	'''                                                 "thisId", "Account");
	''' </PRE>
	''' <LI>as the return value from the <code>_type</code> method
	''' in <code>Holder</code> classes for user-defined
	''' IDL types.  These <code>Holder</code> classes are generated
	''' by the <code>idltojava</code> compiler.
	''' <LI>from a CORBA Interface Repository
	''' </OL>
	''' <P>
	''' Most of the methods in the class <code>TypeCode</code>
	''' are accessors, and the information contained in a <code>TypeCode</code>
	''' object is specific to a particular type.  Therefore, methods
	''' must be invoked
	''' only on the kind of type codes to which they apply.  If an
	''' accessor method
	''' tries to access information from an inappropriate kind of
	''' type code, it will throw
	''' the exception <code>TypeCodePackage.BadKind</code>.  For example,
	''' if the method <code>discriminator_type</code> is called on anything
	''' other than a <code>union</code>, it will throw <code>BadKind</code>
	''' because only <code>union</code>s have a discriminator.
	''' The following list shows which methods apply to which kinds of
	''' type codes:
	''' <P>
	''' These methods may be invoked on all <code>TypeCode</code> kinds:
	''' <UL>
	''' <LI><code>equal</code>
	''' <LI><code>kind</code>
	''' </UL>
	''' <P>
	''' These methods may be invoked on <code>objref</code>, <code>struct</code>,
	''' <code>union</code>, <code>enum</code>,
	''' <code>alias</code>, <code>exception</code>, <code>value</code>,
	''' <code>value_box</code>, <code>native</code>,
	''' and <code>abstract_interface</code>:
	''' <UL>
	''' <LI><code>id</code>
	''' <LI><code>name</code>
	''' </UL>
	''' <P>
	''' These methods may be invoked on <code>struct</code>,
	''' <code>union</code>, <code>enum</code>,
	''' and <code>exception</code>:
	''' <UL>
	''' <LI><code>member_count</code>
	''' <LI><code>member_name</code>
	''' </UL>
	''' <P>
	''' These methods may be invoked on <code>struct</code>,
	''' <code>union</code>, and <code>exception</code>:
	''' <UL>
	''' <LI><code>member_type(int index)</code>
	''' </UL>
	''' <P>
	''' These methods may be invoked on <code>union</code>:
	''' <UL>
	''' <LI><code>member_label</code>
	''' <LI><code>discriminator_type</code>
	''' <LI><code>default_index</code>
	''' </UL>
	''' <P>
	''' These methods may be invoked on <code>string</code>,
	''' <code>sequence</code>, and <code>array</code>:
	''' <UL>
	''' <LI><code>length</code>
	''' </UL>
	''' <P>
	''' These methods may be invoked on <code>alias</code>,
	''' <code>sequence</code>, <code>array</code>, and <code>value_box</code>:
	''' <UL>
	''' <LI><code>content_type</code>
	''' </UL>
	''' <P>
	''' Unlike other CORBA pseudo-objects, <code>TypeCode</code>
	''' objects can be passed as general IDL parameters. <p>
	''' The methods <code>parameter</code> and <code>param_count</code>,
	''' which are deprecated, are not mapped.  <p>
	''' 
	''' Java IDL extends the CORBA specification to allow all operations permitted
	''' on a <code>struct</code> <code>TypeCode</code> to be permitted
	''' on an <code>exception</code> <code>TypeCode</code> as well. <p>
	''' 
	''' </summary>
	Public MustInherit Class TypeCode
		Implements org.omg.CORBA.portable.IDLEntity

		''' <summary>
		''' Compares this <code>TypeCode</code> object with the given one,
		''' testing for equality. <code>TypeCode</code> objects are equal if
		''' they are interchangeable and give identical results when
		''' <code>TypeCode</code> operations are applied to them.
		''' </summary>
		''' <param name="tc">                the <code>TypeCode</code> object to compare against </param>
		''' <returns>          <code>true</code> if the type codes are equal;
		'''                <code>false</code> otherwise </returns>

		Public MustOverride Function equal(ByVal tc As TypeCode) As Boolean

		''' <summary>
		''' Tests to see if the given <code>TypeCode</code> object is
		''' equivalent to this <code>TypeCode</code> object.
		''' <P>
		'''     
		''' </summary>
		''' <param name="tc"> the typecode to compare with this typecode
		''' </param>
		''' <returns> <code>true</code> if the given typecode is equivalent to
		'''         this typecode; <code>false</code> otherwise
		'''  </returns>
		Public MustOverride Function equivalent(ByVal tc As TypeCode) As Boolean

		''' <summary>
		''' Strips out all optional name and member name fields,
		''' but leaves all alias typecodes intact. </summary>
		''' <returns> a <code>TypeCode</code> object with optional name and
		'''         member name fields stripped out, except for alias typecodes,
		'''         which are left intact </returns>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
		'''      comments for unimplemented features</a> </seealso>
		Public MustOverride Function get_compact_typecode() As TypeCode


		''' <summary>
		''' Retrieves the kind of this <code>TypeCode</code> object.
		''' The kind of a type code determines which <code>TypeCode</code>
		''' methods may legally be invoked on it.
		''' <P>
		''' The method <code>kind</code> may be invoked on any
		''' <code>TypeCode</code> object.
		''' </summary>
		''' <returns>  the <code>TCKind</code> instance indicating the
		'''            value of the <code>kind</code> field of this
		'''                  <code>TypeCode</code> object </returns>

		Public MustOverride Function kind() As TCKind

		''' <summary>
		''' Retrieves the RepositoryId globally identifying the type
		''' of this <code>TypeCode</code> object.
		''' <P>
		''' The method <code>id</code> can be invoked on object reference,
		''' structure, union, enumeration, alias, exception, valuetype,
		''' boxed valuetype, native, and abstract interface type codes.
		''' Object reference, exception, valuetype, boxed valuetype,
		''' native, and abstract interface <code>TypeCode</code> objects
		''' always have a RepositoryId.
		''' Structure, union, enumeration, and alias <code>TypeCode</code> objects
		''' obtained from the Interface Repository or the method
		''' <code>ORB.create_operation_list</code>
		''' also always have a RepositoryId. If there is no RepositoryId, the
		''' method can return an empty string.
		''' </summary>
		''' <returns>          the RepositoryId for this <code>TypeCode</code> object
		'''                or an empty string if there is no RepositoryID </returns>
		''' <exception cref="org.omg.CORBA.TypeCodePackage.BadKind"> if the method
		'''           is invoked on an inappropriate kind of<code>TypeCode</code>
		'''           object </exception>

		Public MustOverride Function id() As String

		''' <summary>
		''' Retrieves the simple name identifying this <code>TypeCode</code>
		''' object within its
		''' enclosing scope. Since names are local to a Repository, the
		''' name returned from a <code>TypeCode</code> object
		''' may not match the name of the
		''' type in any particular Repository, and may even be an empty
		''' string.
		''' <P>
		''' The method <code>name</code> can be invoked on object reference,
		''' structure, union, enumeration, alias, exception, valuetype,
		''' boxed valuetype, native, and abstract interface
		''' <code>TypeCode</code> objects.
		''' </summary>
		''' <returns>          the name identifying this <code>TypeCode</code> object
		'''                or an empty string </returns>
		''' <exception cref="org.omg.CORBA.TypeCodePackage.BadKind"> if the method
		'''           is invoked on an inappropriate kind of<code>TypeCode</code>
		'''           object </exception>

		Public MustOverride Function name() As String

		''' <summary>
		''' Retrieves the number of members in the type described by
		''' this <code>TypeCode</code> object.
		''' <P>
		''' The method <code>member_count</code> can be invoked on
		''' structure, union, and enumeration <code>TypeCode</code> objects.
		''' Java IDL extends the CORBA specification to allow this method to
		''' operate on exceptions as well.
		''' </summary>
		''' <returns>          the number of members constituting the type described
		'''                by this <code>TypeCode</code> object
		''' </returns>
		''' <exception cref="org.omg.CORBA.TypeCodePackage.BadKind"> if the method
		'''           is invoked on an inappropriate kind of <code>TypeCode</code>
		'''           object </exception>

		Public MustOverride Function member_count() As Integer

		''' <summary>
		''' Retrieves the simple name of the member identified by
		''' the given index. Since names are local to a
		''' Repository, the name returned from a <code>TypeCode</code> object
		''' may not match the name of the member in any particular
		''' Repository, and may even be an empty string.
		''' <P>
		''' The  method <code>member_name</code> can be invoked on structure, union,
		''' and enumeration <code>TypeCode</code> objects.
		''' Java IDL extends the CORBA specification to allow this method to
		''' operate on exceptions as well.
		''' </summary>
		''' <param name="index">     index of the member for which a name is being reqested </param>
		''' <returns>          simple name of the member identified by the
		'''                  index or an empty string </returns>
		''' <exception cref="org.omg.CORBA.TypeCodePackage.Bounds"> if the index is equal
		'''            to or greater than
		'''                  the number of members constituting the type </exception>
		''' <exception cref="org.omg.CORBA.TypeCodePackage.BadKind"> if the method
		'''           is invoked on an inappropriate kind of <code>TypeCode</code>
		'''           object </exception>

		Public MustOverride Function member_name(ByVal index As Integer) As String

		''' <summary>
		''' Retrieves the <code>TypeCode</code> object describing the type
		''' of the member identified by the given index.
		''' <P>
		''' The method <code>member_type</code> can be invoked on structure
		''' and union <code>TypeCode</code> objects.
		''' Java IDL extends the CORBA specification to allow this method to
		''' operate on exceptions as well.
		''' </summary>
		''' <param name="index">     index of the member for which type information
		'''                is begin requested </param>
		''' <returns>          the <code>TypeCode</code> object describing the
		'''                member at the given index </returns>
		''' <exception cref="org.omg.CORBA.TypeCodePackage.Bounds"> if the index is
		'''                equal to or greater than
		'''                      the number of members constituting the type </exception>
		''' <exception cref="org.omg.CORBA.TypeCodePackage.BadKind"> if the method
		'''           is invoked on an inappropriate kind of <code>TypeCode</code>
		'''           object </exception>

		Public MustOverride Function member_type(ByVal index As Integer) As TypeCode

		''' <summary>
		''' Retrieves the label of the union member
		''' identified by the given index. For the default member,
		''' the label is the zero octet.
		''' <P>
		''' The method <code>member_label</code> can only be invoked on union
		''' <code>TypeCode</code> objects.
		''' </summary>
		''' <param name="index">     index of the union member for which the
		'''                label is being requested </param>
		''' <returns>          an <code>Any</code> object describing the label of
		'''                the requested union member or the zero octet for
		'''                the default member </returns>
		''' <exception cref="org.omg.CORBA.TypeCodePackage.Bounds"> if the index is
		'''                equal to or greater than
		'''                      the number of members constituting the union </exception>
		''' <exception cref="org.omg.CORBA.TypeCodePackage.BadKind"> if the method
		'''           is invoked on a non-union <code>TypeCode</code>
		'''           object </exception>

		Public MustOverride Function member_label(ByVal index As Integer) As Any

		''' <summary>
		''' Returns a <code>TypeCode</code> object describing
		''' all non-default member labels.
		''' The method <code>discriminator_type</code> can be invoked only
		''' on union <code>TypeCode</code> objects.
		''' </summary>
		''' <returns>          the <code>TypeCode</code> object describing
		'''                the non-default member labels </returns>
		''' <exception cref="org.omg.CORBA.TypeCodePackage.BadKind"> if the method
		'''           is invoked on a non-union <code>TypeCode</code>
		'''           object </exception>

		Public MustOverride Function discriminator_type() As TypeCode

		''' <summary>
		''' Returns the index of the
		''' default member, or -1 if there is no default member.
		''' <P>
		''' The method <code>default_index</code> can be invoked only on union
		''' <code>TypeCode</code> objects.
		''' </summary>
		''' <returns>          the index of the default member, or -1 if
		'''                  there is no default member </returns>
		''' <exception cref="org.omg.CORBA.TypeCodePackage.BadKind"> if the method
		'''           is invoked on a non-union <code>TypeCode</code>
		'''           object </exception>

		Public MustOverride Function default_index() As Integer

		''' <summary>
		''' Returns the number of elements in the type described by
		''' this <code>TypeCode</code> object.
		''' For strings and sequences, it returns the
		''' bound, with zero indicating an unbounded string or sequence.
		''' For arrays, it returns the number of elements in the array.
		''' <P>
		''' The method <code>length</code> can be invoked on string, sequence, and
		''' array <code>TypeCode</code> objects.
		''' </summary>
		''' <returns>          the bound for strings and sequences, or the
		'''                      number of elements for arrays </returns>
		''' <exception cref="org.omg.CORBA.TypeCodePackage.BadKind"> if the method
		'''           is invoked on an inappropriate kind of <code>TypeCode</code>
		'''           object </exception>

		Public MustOverride Function length() As Integer

		''' <summary>
		''' Returns the <code>TypeCode</code> object representing the
		''' IDL type for the members of the object described by this
		''' <code>TypeCode</code> object.
		''' For sequences and arrays, it returns the
		''' element type. For aliases, it returns the original type. Note
		''' that multidimensional arrays are represented by nesting
		''' <code>TypeCode</code> objects, one per dimension.
		''' For boxed valuetypes, it returns the boxed type.
		''' <P>
		''' The method <code>content_type</code> can be invoked on sequence, array,
		''' alias, and boxed valuetype <code>TypeCode</code> objects.
		''' </summary>
		''' <returns>  a <code>TypeCode</code> object representing
		'''            the element type for sequences and arrays, the
		'''          original type for aliases, or the
		'''            boxed type for boxed valuetypes. </returns>
		''' <exception cref="org.omg.CORBA.TypeCodePackage.BadKind"> if the method
		'''           is invoked on an inappropriate kind of <code>TypeCode</code>
		'''           object </exception>

		Public MustOverride Function content_type() As TypeCode


		''' <summary>
		''' Returns the number of digits in the fixed type described by this
		''' <code>TypeCode</code> object. For example, the typecode for
		''' the number 3000.275d could be <code>fixed<7,3></code>, where
		''' 7 is the precision and 3 is the scale.
		''' </summary>
		''' <returns> the total number of digits </returns>
		''' <exception cref="org.omg.CORBA.TypeCodePackage.BadKind"> if this method
		'''           is invoked on an inappropriate kind of <code>TypeCode</code>
		'''           object
		'''      </exception>
		Public MustOverride Function fixed_digits() As Short

		''' <summary>
		''' Returns the scale of the fixed type described by this
		''' <code>TypeCode</code> object. A positive number indicates the
		''' number of digits to the right of the decimal point.
		''' For example, the number 3000d could have the
		''' typecode <code>fixed<4,0></code>, where the first number is
		''' the precision and the second number is the scale.
		''' A negative number is also possible and adds zeroes to the
		''' left of the decimal point.  In this case, <code>fixed<1,-3></code>,
		''' could be the typecode for the number 3000d.
		''' </summary>
		''' <returns> the scale of the fixed type that this
		'''         <code>TypeCode</code> object describes </returns>
		''' <exception cref="org.omg.CORBA.TypeCodePackage.BadKind"> if this method
		'''           is invoked on an inappropriate kind of <code>TypeCode</code>
		'''           object </exception>
		Public MustOverride Function fixed_scale() As Short

		''' <summary>
		''' Returns the constant that indicates the visibility of the member
		''' at the given index.
		''' 
		''' This operation can only be invoked on non-boxed value
		''' <code>TypeCode</code> objects.
		''' </summary>
		''' <param name="index"> an <code>int</code> indicating the index into the
		'''               value </param>
		''' <returns> either <code>PRIVATE_MEMBER.value</code> or
		'''          <code>PUBLIC_MEMBER.value</code> </returns>
		''' <exception cref="org.omg.CORBA.TypeCodePackage.BadKind"> if this method
		'''           is invoked on a non-value type <code>TypeCode</code>
		'''           object </exception>
		''' <exception cref="org.omg.CORBA.TypeCodePackage.Bounds">
		'''           if the given index is out of bounds </exception>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
		'''      comments for unimplemented features</a> </seealso>

		Public MustOverride Function member_visibility(ByVal index As Integer) As Short

		''' <summary>
		''' Returns a constant indicating the modifier of the value type
		''' that this <code>TypeCode</code> object describes.  The constant
		''' returned must be one of the following: <code>VM_NONE.value</code>,
		''' <code>VM_ABSTRACT.value</code>, <code>VM_CUSTOM.value</code>,
		''' or <code>VM_TRUNCATABLE.value</code>,
		''' </summary>
		''' <returns> a constant describing the value type
		'''         that this <code>TypeCode</code> object describes </returns>
		''' <exception cref="org.omg.CORBA.TypeCodePackage.BadKind">
		'''           if this method
		'''           is invoked on a non-value type <code>TypeCode</code>
		'''           object </exception>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
		'''      comments for unimplemented features</a> </seealso>

		Public MustOverride Function type_modifier() As Short

		''' <summary>
		''' Returns the <code>TypeCode</code> object that describes the concrete base type
		''' of the value type that this <code>TypeCode</code> object describes.
		''' Returns null if it doesn't have a concrete base type.
		''' </summary>
		''' <returns> the <code>TypeCode</code> object that describes the
		''' concrete base type of the value type
		''' that this <code>TypeCode</code> object describes </returns>
		''' <exception cref="org.omg.CORBA.TypeCodePackage.BadKind"> if this method
		'''           is invoked on a non-boxed value type <code>TypeCode</code> object </exception>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
		'''      comments for unimplemented features</a> </seealso>

		Public MustOverride Function concrete_base_type() As TypeCode
	End Class

End Namespace