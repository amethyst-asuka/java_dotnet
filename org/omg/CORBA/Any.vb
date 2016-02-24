Imports System

'
' * Copyright (c) 1997, 2004, Oracle and/or its affiliates. All rights reserved.
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
	''' Serves as a container for any data that can be
	''' described in IDL or for any IDL primitive type.
	''' An <code>Any</code> object is used as a component of a
	''' <code>NamedValue</code> object, which provides information about
	''' arguments or return values in requests, and which is used to define
	''' name/value pairs in <code>Context</code> objects.
	''' <p>
	''' 
	''' An <code>Any</code> object consists of two parts:
	''' <OL>
	''' <LI>a data value
	''' <LI>a <code>TypeCode</code> object describing the type of the data
	''' value contained in the <code>Any</code> object.  For example,
	''' a <code>TypeCode</code> object for an array contains
	''' a field for the length of the array and a field for
	''' the type of elements in the array. (Note that in     this case, the
	''' second field of the <code>TypeCode</code> object is itself a
	''' <code>TypeCode</code> object.)
	''' </OL>
	''' 
	''' <P>
	''' <a name="anyOps"</a>
	''' A large part of the <code>Any</code> class consists of pairs of methods
	''' for inserting values into and extracting values from an
	''' <code>Any</code> object.
	''' <P>
	''' For a given primitive type X, these methods are:
	'''  <dl>
	'''      <dt><code><bold> void insert_X(X x)</bold></code>
	'''      <dd> This method allows the insertion of
	'''        an instance <code>x</code> of primitive type <code>X</code>
	'''    into the <code>value</code> field of the <code>Any</code> object.
	'''    Note that the method
	'''    <code>insert_X</code> also resets the <code>Any</code> object's
	'''    <code>type</code> field if necessary.
	'''      <dt> <code><bold>X extract_X()</bold></code>
	'''      <dd> This method allows the extraction of an instance of
	'''        type <code>X</code> from the <code>Any</code> object.
	'''    <BR>
	'''    <P>
	'''    This method throws the exception <code>BAD_OPERATION</code> under two conditions:
	'''    <OL>
	'''     <LI> the type of the element contained in the <code>Any</code> object is not
	'''         <code>X</code>
	'''     <LI> the method <code>extract_X</code> is called before
	'''     the <code>value</code> field of the <code>Any</code> object
	'''     has been set
	'''    </OL>
	''' </dl>
	''' <P>
	''' There are distinct method pairs for each
	''' primitive IDL data type (<code>insert_long</code> and <code>extract_long</code>,
	''' <code>insert_string</code> and <code>extract_string</code>, and so on).<BR>
	''' <P>
	''' The class <code>Any</code> also has methods for
	''' getting and setting the type code,
	''' for testing two <code>Any</code> objects for equality,
	''' and for reading an <code>Any</code> object from a stream or
	''' writing it to a stream.
	''' <BR>
	''' @since   JDK1.2
	''' </summary>
	Public MustInherit Class Any
		Implements org.omg.CORBA.portable.IDLEntity

		''' <summary>
		''' Checks for equality between this <code>Any</code> object and the
		''' given <code>Any</code> object.  Two <code>Any</code> objects are
		''' equal if both their values and type codes are equal.
		''' </summary>
		''' <param name="a"> the <code>Any</code> object to test for equality </param>
		''' <returns>  <code>true</code> if the <code>Any</code> objects are equal;
		''' <code>false</code> otherwise </returns>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
		'''      comments for unimplemented features</a> </seealso>
		Public MustOverride Function equal(ByVal a As Any) As Boolean

		''' <summary>
		''' Returns type information for the element contained in this
		''' <code>Any</code> object.
		''' </summary>
		''' <returns>          the <code>TypeCode</code> object containing type information
		'''                about the value contained in this <code>Any</code> object </returns>
		Public MustOverride Function type() As TypeCode

		''' <summary>
		''' Sets this <code>Any</code> object's <code>type</code> field
		''' to the given <code>TypeCode</code> object and clears its value.
		''' <P>
		''' Note that using this method to set the type code wipes out the
		''' value if there is one. The method
		''' is provided primarily so that the type may be set properly for
		''' IDL <code>out</code> parameters.  Generally, setting the type
		''' is done by the <code>insert_X</code> methods, which will set the type
		''' to X if it is not already set to X.
		''' </summary>
		''' <param name="t">       the <code>TypeCode</code> object giving
		'''                information for the value in
		'''                this <code>Any</code> object </param>
		Public MustOverride Sub type(ByVal t As TypeCode)

		'/////////////////////////////////////////////////////////////////////////
		' marshalling/unmarshalling routines

		''' <summary>
		''' Reads off (unmarshals) the value of an <code>Any</code> object from
		''' the given input stream using the given typecode.
		''' </summary>
		''' <param name="is"> the <code>org.omg.CORBA.portable.InputStream</code>
		'''                object from which to read
		'''                the value contained in this <code>Any</code> object
		''' </param>
		''' <param name="t">  a <code>TypeCode</code> object containing type information
		'''           about the value to be read
		''' </param>
		''' <exception cref="MARSHAL"> when the given <code>TypeCode</code> object is
		'''                    not consistent with the value that was contained
		'''                    in the input stream </exception>
		Public MustOverride Sub read_value(ByVal [is] As org.omg.CORBA.portable.InputStream, ByVal t As TypeCode)

		''' <summary>
		''' Writes out the value of this <code>Any</code> object
		''' to the given output stream.  If both <code>typecode</code>
		''' and <code>value</code> need to be written, use
		''' <code>create_output_stream()</code> to create an <code>OutputStream</code>,
		''' then use <code>write_any</code> on the <code>OutputStream</code>.
		''' <P>
		''' If this method is called on an <code>Any</code> object that has not
		''' had a value inserted into its <code>value</code> field, it will throw
		''' the exception <code>java.lang.NullPointerException</code>.
		''' </summary>
		''' <param name="os">        the <code>org.omg.CORBA.portable.OutputStream</code>
		'''                object into which to marshal the value
		'''                of this <code>Any</code> object
		'''  </param>
		Public MustOverride Sub write_value(ByVal os As org.omg.CORBA.portable.OutputStream)

		''' <summary>
		''' Creates an output stream into which this <code>Any</code> object's
		''' value can be marshalled.
		''' </summary>
		''' <returns>          the newly-created <code>OutputStream</code> </returns>
		Public MustOverride Function create_output_stream() As org.omg.CORBA.portable.OutputStream

		''' <summary>
		''' Creates an input stream from which this <code>Any</code> object's value
		''' can be unmarshalled.
		''' </summary>
		''' <returns>          the newly-created <code>InputStream</code> </returns>
		Public MustOverride Function create_input_stream() As org.omg.CORBA.portable.InputStream

		'/////////////////////////////////////////////////////////////////////////
		' basic insertion/extraction methods

		''' <summary>
		''' Extracts the <code>short</code> in this
		''' <code>Any</code> object's <code>value</code> field.
		''' </summary>
		''' <returns> the <code>short</code> stored in this <code>Any</code> object </returns>
		''' <exception cref="BAD_OPERATION"> if this  <code>Any</code> object
		'''              contains something other than a <code>short</code> or the
		'''              <code>value</code> field has not yet been set </exception>
		Public MustOverride Function extract_short() As Short

		''' <summary>
		''' Inserts the given <code>short</code>
		''' into this <code>Any</code> object's <code>value</code> field.
		''' </summary>
		''' <param name="s">         the <code>short</code> to insert into this
		'''                <code>Any</code> object </param>
		Public MustOverride Sub insert_short(ByVal s As Short)

		''' <summary>
		''' Extracts the <code>int</code> in this
		''' <code>Any</code> object's <code>value</code> field.
		''' </summary>
		''' <returns> the <code>int</code> stored in this <code>Any</code> object </returns>
		''' <exception cref="BAD_OPERATION"> if this  <code>Any</code> object
		'''              contains something other than an <code>int</code> or the
		'''              <code>value</code> field has not yet been set </exception>
		Public MustOverride Function extract_long() As Integer

		''' <summary>
		''' Inserts the given <code>int</code>
		''' into this <code>Any</code> object's <code>value</code> field.
		''' </summary>
		''' <param name="l">         the <code>int</code> to insert into this
		'''                <code>Any</code> object </param>
		Public MustOverride Sub insert_long(ByVal l As Integer)


		''' <summary>
		''' Extracts the <code>long</code> in this
		''' <code>Any</code> object's <code>value</code> field.
		''' </summary>
		''' <returns> the <code>long</code> stored in this <code>Any</code> object </returns>
		''' <exception cref="BAD_OPERATION"> if this  <code>Any</code> object
		'''              contains something other than a <code>long</code> or the
		'''              <code>value</code> field has not yet been set </exception>
		Public MustOverride Function extract_longlong() As Long

		''' <summary>
		''' Inserts the given <code>long</code>
		''' into this <code>Any</code> object's <code>value</code> field.
		''' </summary>
		''' <param name="l">         the <code>long</code> to insert into this
		'''                <code>Any</code> object </param>
		Public MustOverride Sub insert_longlong(ByVal l As Long)

		''' <summary>
		''' Extracts the <code>short</code> in this
		''' <code>Any</code> object's <code>value</code> field.
		''' </summary>
		''' <returns> the <code>short</code> stored in this <code>Any</code> object </returns>
		''' <exception cref="BAD_OPERATION"> if this  <code>Any</code> object
		'''              contains something other than a <code>short</code> or the
		'''              <code>value</code> field has not yet been set </exception>
		Public MustOverride Function extract_ushort() As Short

		''' <summary>
		''' Inserts the given <code>short</code>
		''' into this <code>Any</code> object's <code>value</code> field.
		''' </summary>
		''' <param name="s">         the <code>short</code> to insert into this
		'''                <code>Any</code> object </param>
		Public MustOverride Sub insert_ushort(ByVal s As Short)

		''' <summary>
		''' Extracts the <code>int</code> in this
		''' <code>Any</code> object's <code>value</code> field.
		''' </summary>
		''' <returns> the <code>int</code> stored in this <code>Any</code> object </returns>
		''' <exception cref="BAD_OPERATION"> if this  <code>Any</code> object
		'''              contains something other than an <code>int</code> or the
		'''              <code>value</code> field has not yet been set </exception>
		Public MustOverride Function extract_ulong() As Integer

		''' <summary>
		''' Inserts the given <code>int</code>
		''' into this <code>Any</code> object's <code>value</code> field.
		''' </summary>
		''' <param name="l">         the <code>int</code> to insert into this
		'''                <code>Any</code> object </param>
		Public MustOverride Sub insert_ulong(ByVal l As Integer)

		''' <summary>
		''' Extracts the <code>long</code> in this
		''' <code>Any</code> object's <code>value</code> field.
		''' </summary>
		''' <returns> the <code>long</code> stored in this <code>Any</code> object </returns>
		''' <exception cref="BAD_OPERATION"> if this  <code>Any</code> object
		'''              contains something other than a <code>long</code> or the
		'''              <code>value</code> field has not yet been set </exception>
		Public MustOverride Function extract_ulonglong() As Long

		''' <summary>
		''' Inserts the given <code>long</code>
		''' into this <code>Any</code> object's <code>value</code> field.
		''' </summary>
		''' <param name="l">         the <code>long</code> to insert into this
		'''                <code>Any</code> object </param>
		Public MustOverride Sub insert_ulonglong(ByVal l As Long)

		''' <summary>
		''' Extracts the <code>float</code> in this
		''' <code>Any</code> object's <code>value</code> field.
		''' </summary>
		''' <returns> the <code>float</code> stored in this <code>Any</code> object </returns>
		''' <exception cref="BAD_OPERATION"> if this  <code>Any</code> object
		'''              contains something other than a <code>float</code> or the
		'''              <code>value</code> field has not yet been set </exception>
		Public MustOverride Function extract_float() As Single

		''' <summary>
		''' Inserts the given <code>float</code>
		''' into this <code>Any</code> object's <code>value</code> field.
		''' </summary>
		''' <param name="f">         the <code>float</code> to insert into this
		'''                <code>Any</code> object </param>
		Public MustOverride Sub insert_float(ByVal f As Single)

		''' <summary>
		''' Extracts the <code>double</code> in this
		''' <code>Any</code> object's <code>value</code> field.
		''' </summary>
		''' <returns> the <code>double</code> stored in this <code>Any</code> object </returns>
		''' <exception cref="BAD_OPERATION"> if this  <code>Any</code> object
		'''              contains something other than a <code>double</code> or the
		'''              <code>value</code> field has not yet been set </exception>
		Public MustOverride Function extract_double() As Double

		''' <summary>
		''' Inserts the given <code>double</code>
		''' into this <code>Any</code> object's <code>value</code> field.
		''' </summary>
		''' <param name="d">         the <code>double</code> to insert into this
		'''                <code>Any</code> object </param>
		Public MustOverride Sub insert_double(ByVal d As Double)

		''' <summary>
		''' Extracts the <code>boolean</code> in this
		''' <code>Any</code> object's <code>value</code> field.
		''' </summary>
		''' <returns> the <code>boolean</code> stored in this <code>Any</code> object </returns>
		''' <exception cref="BAD_OPERATION"> if this  <code>Any</code> object
		'''              contains something other than a <code>boolean</code> or the
		'''              <code>value</code> field has not yet been set </exception>
		Public MustOverride Function extract_boolean() As Boolean

		''' <summary>
		''' Inserts the given <code>boolean</code>
		''' into this <code>Any</code> object's <code>value</code> field.
		''' </summary>
		''' <param name="b">         the <code>boolean</code> to insert into this
		'''                <code>Any</code> object </param>
		Public MustOverride Sub insert_boolean(ByVal b As Boolean)

		''' <summary>
		''' Extracts the <code>char</code> in this
		''' <code>Any</code> object's <code>value</code> field.
		''' </summary>
		''' <returns> the <code>char</code> stored in this <code>Any</code> object </returns>
		''' <exception cref="BAD_OPERATION"> if this  <code>Any</code> object
		'''              contains something other than a <code>char</code> or the
		'''              <code>value</code> field has not yet been set </exception>
		Public MustOverride Function extract_char() As Char

		''' <summary>
		''' Inserts the given <code>char</code>
		''' into this <code>Any</code> object's <code>value</code> field.
		''' </summary>
		''' <param name="c">         the <code>char</code> to insert into this
		'''                <code>Any</code> object </param>
		''' <exception cref="DATA_CONVERSION"> if there is a data conversion
		'''            error </exception>
		Public MustOverride Sub insert_char(ByVal c As Char)

		''' <summary>
		''' Extracts the <code>char</code> in this
		''' <code>Any</code> object's <code>value</code> field.
		''' </summary>
		''' <returns> the <code>char</code> stored in this <code>Any</code> object </returns>
		''' <exception cref="BAD_OPERATION"> if this  <code>Any</code> object
		'''              contains something other than a <code>char</code> or the
		'''              <code>value</code> field has not yet been set </exception>
		Public MustOverride Function extract_wchar() As Char

		''' <summary>
		''' Inserts the given <code>char</code>
		''' into this <code>Any</code> object's <code>value</code> field.
		''' </summary>
		''' <param name="c">         the <code>char</code> to insert into this
		'''                <code>Any</code> object </param>
		Public MustOverride Sub insert_wchar(ByVal c As Char)

		''' <summary>
		''' Extracts the <code>byte</code> in this
		''' <code>Any</code> object's <code>value</code> field.
		''' </summary>
		''' <returns> the <code>byte</code> stored in this <code>Any</code> object </returns>
		''' <exception cref="BAD_OPERATION"> if this  <code>Any</code> object
		'''              contains something other than a <code>byte</code> or the
		'''              <code>value</code> field has not yet been set </exception>
		Public MustOverride Function extract_octet() As SByte

		''' <summary>
		''' Inserts the given <code>byte</code>
		''' into this <code>Any</code> object's <code>value</code> field.
		''' </summary>
		''' <param name="b">         the <code>byte</code> to insert into this
		'''                <code>Any</code> object </param>
		Public MustOverride Sub insert_octet(ByVal b As SByte)

		''' <summary>
		''' Extracts the <code>Any</code> object in this
		''' <code>Any</code> object's <code>value</code> field.
		''' </summary>
		''' <returns> the <code>Any</code> object stored in this <code>Any</code> object </returns>
		''' <exception cref="BAD_OPERATION"> if this <code>Any</code> object
		'''              contains something other than an <code>Any</code> object or the
		'''              <code>value</code> field has not yet been set </exception>
		Public MustOverride Function extract_any() As Any

		''' <summary>
		''' Inserts the given <code>Any</code> object
		''' into this <code>Any</code> object's <code>value</code> field.
		''' </summary>
		''' <param name="a">         the <code>Any</code> object to insert into this
		'''                <code>Any</code> object </param>
		Public MustOverride Sub insert_any(ByVal a As Any)

		''' <summary>
		''' Extracts the <code>org.omg.CORBA.Object</code> in this
		''' <code>Any</code> object's <code>value</code> field.
		''' </summary>
		''' <returns> the <code>org.omg.CORBA.Object</code> stored in
		'''         this <code>Any</code> object </returns>
		''' <exception cref="BAD_OPERATION"> if this  <code>Any</code> object
		'''              contains something other than an
		'''              <code>org.omg.CORBA.Object</code> or the
		'''              <code>value</code> field has not yet been set </exception>
		Public MustOverride Function extract_Object() As org.omg.CORBA.Object

		''' <summary>
		''' Inserts the given <code>org.omg.CORBA.Object</code> object
		''' into this <code>Any</code> object's <code>value</code> field.
		''' </summary>
		''' <param name="o">         the <code>org.omg.CORBA.Object</code> object to insert into this
		'''                <code>Any</code> object </param>
		Public MustOverride Sub insert_Object(ByVal o As org.omg.CORBA.Object)

		''' <summary>
		''' Extracts the <code>java.io.Serializable</code> object in this
		''' <code>Any</code> object's <code>value</code> field.
		''' </summary>
		''' <returns> the <code>java.io.Serializable</code> object stored in
		'''         this <code>Any</code> object </returns>
		''' <exception cref="BAD_OPERATION"> if this  <code>Any</code> object
		'''              contains something other than a <code>java.io.Serializable</code>
		'''              object or the
		'''              <code>value</code> field has not yet been set </exception>
		Public MustOverride Function extract_Value() As java.io.Serializable

		''' <summary>
		''' Inserts the given <code>java.io.Serializable</code> object
		''' into this <code>Any</code> object's <code>value</code> field.
		''' </summary>
		''' <param name="v">         the <code>java.io.Serializable</code> object to insert into this
		'''                <code>Any</code> object </param>
		Public MustOverride Sub insert_Value(ByVal v As java.io.Serializable)

		''' <summary>
		''' Inserts the given <code>java.io.Serializable</code> object
		''' into this <code>Any</code> object's <code>value</code> field.
		''' </summary>
		''' <param name="v">         the <code>java.io.Serializable</code> object to insert into this
		'''                <code>Any</code> object </param>
		''' <param name="t">     the <code>TypeCode</code> object that is to be inserted into
		'''              this <code>Any</code> object's <code>type</code> field
		'''              and that describes the <code>java.io.Serializable</code>
		'''              object being inserted </param>
		''' <exception cref="MARSHAL"> if the ORB has a problem marshalling or
		'''          unmarshalling parameters </exception>
		Public MustOverride Sub insert_Value(ByVal v As java.io.Serializable, ByVal t As TypeCode)
	''' <summary>
	''' Inserts the given <code>org.omg.CORBA.Object</code> object
	''' into this <code>Any</code> object's <code>value</code> field.
	''' </summary>
	''' <param name="o">         the <code>org.omg.CORBA.Object</code> instance to insert into this
	'''                <code>Any</code> object </param>
	''' <param name="t">     the <code>TypeCode</code> object that is to be inserted into
	'''              this <code>Any</code> object and that describes
	'''              the <code>Object</code> being inserted </param>
	''' <exception cref="BAD_OPERATION"> if this  method is invalid for this
	'''            <code>Any</code> object
	'''      </exception>
		Public MustOverride Sub insert_Object(ByVal o As org.omg.CORBA.Object, ByVal t As TypeCode)

		''' <summary>
		''' Extracts the <code>String</code> object in this
		''' <code>Any</code> object's <code>value</code> field.
		''' </summary>
		''' <returns> the <code>String</code> object stored in this <code>Any</code> object </returns>
		''' <exception cref="BAD_OPERATION"> if this  <code>Any</code> object
		'''              contains something other than a <code>String</code> object or the
		'''              <code>value</code> field has not yet been set </exception>
		Public MustOverride Function extract_string() As String

		''' <summary>
		''' Inserts the given <code>String</code> object
		''' into this <code>Any</code> object's <code>value</code> field.
		''' </summary>
		''' <param name="s">         the <code>String</code> object to insert into this
		'''                <code>Any</code> object </param>
		''' <exception cref="DATA_CONVERSION"> if there is a data conversion error </exception>
		''' <exception cref="MARSHAL"> if the ORB has a problem marshalling or
		'''             unmarshalling parameters </exception>
		Public MustOverride Sub insert_string(ByVal s As String)

		''' <summary>
		''' Extracts the <code>String</code> object in this
		''' <code>Any</code> object's <code>value</code> field.
		''' </summary>
		''' <returns> the <code>String</code> object stored in this <code>Any</code> object </returns>
		''' <exception cref="BAD_OPERATION"> if this  <code>Any</code> object
		'''              contains something other than a <code>String</code> object or the
		'''              <code>value</code> field has not yet been set </exception>
		Public MustOverride Function extract_wstring() As String

		''' <summary>
		''' Inserts the given <code>String</code> object
		''' into this <code>Any</code> object's <code>value</code> field.
		''' </summary>
		''' <param name="s">         the <code>String</code> object to insert into this
		'''                <code>Any</code> object </param>
		''' <exception cref="MARSHAL"> if the ORB has a problem marshalling or
		'''             unmarshalling parameters </exception>
		Public MustOverride Sub insert_wstring(ByVal s As String)

		''' <summary>
		''' Extracts the <code>TypeCode</code> object in this
		''' <code>Any</code> object's <code>value</code> field.
		''' </summary>
		''' <returns> the <code>TypeCode</code> object stored in this <code>Any</code> object </returns>
		''' <exception cref="BAD_OPERATION"> if this  <code>Any</code> object
		'''              contains something other than a <code>TypeCode</code> object or the
		'''              <code>value</code> field has not yet been set </exception>
		Public MustOverride Function extract_TypeCode() As TypeCode

		''' <summary>
		''' Inserts the given <code>TypeCode</code> object
		''' into this <code>Any</code> object's <code>value</code> field.
		''' </summary>
		''' <param name="t">         the <code>TypeCode</code> object to insert into this
		'''                <code>Any</code> object </param>
		Public MustOverride Sub insert_TypeCode(ByVal t As TypeCode)

		''' <summary>
		''' Extracts the <code>Principal</code> object in this
		''' <code>Any</code> object's <code>value</code> field.
		''' Note that the class <code>Principal</code> has been deprecated.
		''' </summary>
		''' <returns> the <code>Principal</code> object stored in this <code>Any</code> object </returns>
		''' <exception cref="BAD_OPERATION"> if this  <code>Any</code> object
		'''              contains something other than a
		'''              <code>Principal</code> object or the
		'''              <code>value</code> field has not yet been set </exception>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
		'''      comments for unimplemented features</a> </seealso>
		''' @deprecated Deprecated by CORBA 2.2. 
		<Obsolete("Deprecated by CORBA 2.2.")> _
		Public Overridable Function extract_Principal() As Principal
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function

		''' <summary>
		''' Inserts the given <code>Principal</code> object
		''' into this <code>Any</code> object's <code>value</code> field.
		''' Note that the class <code>Principal</code> has been deprecated.
		''' </summary>
		''' <param name="p">         the <code>Principal</code> object to insert into this
		'''                <code>Any</code> object </param>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
		'''      comments for unimplemented features</a> </seealso>
		''' @deprecated Deprecated by CORBA 2.2. 
		<Obsolete("Deprecated by CORBA 2.2.")> _
		Public Overridable Sub insert_Principal(ByVal p As Principal)
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Sub

		'/////////////////////////////////////////////////////////////////////////
		' insertion/extraction of streamables

		''' <summary>
		''' Extracts a <code>Streamable</code> from this <code>Any</code> object's
		''' <code>value</code> field.  This method allows the extraction of
		''' non-primitive IDL types.
		''' </summary>
		''' <returns> the <code>Streamable</code> stored in the <code>Any</code> object. </returns>
		''' <exception cref="BAD_INV_ORDER"> if the caller has invoked operations in the wrong order </exception>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
		'''      comments for unimplemented features</a> </seealso>
		Public Overridable Function extract_Streamable() As org.omg.CORBA.portable.Streamable
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function

		''' <summary>
		''' Inserts the given <code>Streamable</code> object
		''' into this <code>Any</code> object's <code>value</code> field.
		''' This method allows the insertion of non-primitive IDL types.
		''' </summary>
		''' <param name="s">         the <code>Streamable</code> object to insert into this
		'''                <code>Any</code> object; may be a non-primitive
		'''                IDL type </param>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
		'''      comments for unimplemented features</a> </seealso>
		Public Overridable Sub insert_Streamable(ByVal s As org.omg.CORBA.portable.Streamable)
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Sub

		''' <summary>
		''' Extracts the <code>java.math.BigDecimal</code> object in this
		''' <code>Any</code> object's <code>value</code> field.
		''' </summary>
		''' <returns> the <code>java.math.BigDecimal</code> object
		'''         stored in this <code>Any</code> object </returns>
		''' <exception cref="BAD_OPERATION"> if this  <code>Any</code> object
		'''              contains something other than a
		'''              <code>java.math.BigDecimal</code> object or the
		'''              <code>value</code> field has not yet been set </exception>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
		'''      comments for unimplemented features</a> </seealso>
		Public Overridable Function extract_fixed() As Decimal
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function

		''' <summary>
		''' Throws an <a href="package-summary.html#NO_IMPLEMENT">
		''' <code>org.omg.CORBA.NO_IMPLEMENT</code></a> exception.
		''' <P>
		''' Inserts the given <code>java.math.BigDecimal</code> object
		''' into this <code>Any</code> object's <code>value</code> field.
		''' </summary>
		''' <param name="value">             the <code>java.math.BigDecimal</code> object
		'''                  to insert into this <code>Any</code> object </param>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
		'''      comments for unimplemented features</a> </seealso>
		Public Overridable Sub insert_fixed(ByVal value As Decimal)
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Sub

		''' <summary>
		''' Throws an <a href="package-summary.html#NO_IMPLEMENT">
		''' <code>org.omg.CORBA.NO_IMPLEMENT</code></a> exception.
		''' <P>
		''' Inserts the given <code>java.math.BigDecimal</code> object
		''' into this <code>Any</code> object's <code>value</code> field.
		''' </summary>
		''' <param name="value">             the <code>java.math.BigDecimal</code> object
		'''                  to insert into this <code>Any</code> object </param>
		''' <param name="type">     the <code>TypeCode</code> object that is to be inserted into
		'''              this <code>Any</code> object's <code>type</code> field
		'''              and that describes the <code>java.math.BigDecimal</code>
		'''              object being inserted </param>
		''' <exception cref="org.omg.CORBA.BAD_INV_ORDER"> if this method is  invoked improperly </exception>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>CORBA</code> package
		'''      comments for unimplemented features</a> </seealso>
		Public Overridable Sub insert_fixed(ByVal value As Decimal, ByVal type As org.omg.CORBA.TypeCode)
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Sub
	End Class

End Namespace