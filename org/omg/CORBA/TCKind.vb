Imports System

'
' * Copyright (c) 1997, 2010, Oracle and/or its affiliates. All rights reserved.
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
	''' The Java mapping of the IDL enum <code>TCKind</code>, which
	''' specifies the kind of a <code>TypeCode</code> object.  There is
	''' one kind for each primitive and essential IDL data type.
	''' <P>
	''' The class <code>TCKind</code> consists of:
	''' <UL>
	''' <LI>a set of <code>int</code> constants, one for each
	''' kind of IDL data type.  These <code>int</code> constants
	''' make it possible to use a <code>switch</code> statement.
	''' <LI>a set of <code>TCKind</code> constants, one for each
	''' kind of IDL data type.  The <code>value</code> field for
	''' each <code>TCKind</code> instance is initialized with
	''' the <code>int</code> constant that corresponds with
	''' the IDL data type that the instance represents.
	''' <LI>the method <code>from_int</code>for converting
	''' an <code>int</code> to its
	''' corresponding <code>TCKind</code> instance
	''' <P>Example:
	''' <PRE>
	'''      org.omg.CORBA.TCKind k = org.omg.CORBA.TCKind.from_int(
	'''                         org.omg.CORBA.TCKind._tk_string);
	''' </PRE>
	''' The variable <code>k</code> represents the <code>TCKind</code>
	''' instance for the IDL type <code>string</code>, which is
	''' <code>tk_string</code>.
	''' <P>
	''' <LI>the method <code>value</code> for accessing the
	''' <code>_value</code> field of a <code>TCKind</code> constant
	''' <P>Example:
	''' <PRE>
	'''   int i = org.omg.CORBA.TCKind.tk_char.value();
	''' </PRE>
	''' The variable <code>i</code> represents 9, the value for the
	''' IDL data type <code>char</code>.
	''' </UL>
	''' <P>The <code>value</code> field of a <code>TCKind</code> instance
	''' is the CDR encoding used for a <code>TypeCode</code> object in
	''' an IIOP message.
	''' </summary>

	Public Class TCKind

		''' <summary>
		''' The <code>int</code> constant for a <code>null</code> IDL data type.
		''' </summary>
		Public Const _tk_null As Integer = 0

		''' <summary>
		''' The <code>int</code> constant for the IDL data type <code>void</code>.
		''' </summary>
		Public Const _tk_void As Integer = 1

		''' <summary>
		''' The <code>int</code> constant for the IDL data type <code>short</code>.
		''' </summary>
		Public Const _tk_short As Integer = 2

		''' <summary>
		''' The <code>int</code> constant for the IDL data type <code>long</code>.
		''' </summary>
		Public Const _tk_long As Integer = 3

		''' <summary>
		''' The <code>int</code> constant for the IDL data type <code>ushort</code>.
		''' </summary>
		Public Const _tk_ushort As Integer = 4

		''' <summary>
		''' The <code>int</code> constant for the IDL data type <code>ulong</code>.
		''' </summary>
		Public Const _tk_ulong As Integer = 5

		''' <summary>
		''' The <code>int</code> constant for the IDL data type <code>float</code>.
		''' </summary>
		Public Const _tk_float As Integer = 6

		''' <summary>
		''' The <code>int</code> constant for the IDL data type <code>double</code>.
		''' </summary>
		Public Const _tk_double As Integer = 7

		''' <summary>
		''' The <code>int</code> constant for the IDL data type <code>boolean</code>.
		''' </summary>
		Public Const _tk_boolean As Integer = 8

		''' <summary>
		''' The <code>int</code> constant for the IDL data type <code>char</code>.
		''' </summary>
		Public Const _tk_char As Integer = 9

		''' <summary>
		''' The <code>int</code> constant for the IDL data type <code>octet</code>.
		''' </summary>
		Public Const _tk_octet As Integer = 10

		''' <summary>
		''' The <code>int</code> constant for the IDL data type <code>any</code>.
		''' </summary>
		Public Const _tk_any As Integer = 11

		''' <summary>
		''' The <code>int</code> constant for the IDL data type <code>TypeCode</code>.
		''' </summary>
		Public Const _tk_TypeCode As Integer = 12

		''' <summary>
		''' The <code>int</code> constant for the IDL data type <code>Principal</code>.
		''' </summary>
		Public Const _tk_Principal As Integer = 13

		''' <summary>
		''' The <code>int</code> constant for the IDL data type <code>objref</code>.
		''' </summary>
		Public Const _tk_objref As Integer = 14

		''' <summary>
		''' The <code>int</code> constant for the IDL data type <code>struct</code>.
		''' </summary>
		Public Const _tk_struct As Integer = 15

		''' <summary>
		''' The <code>int</code> constant for the IDL data type <code>union</code>.
		''' </summary>
		Public Const _tk_union As Integer = 16

		''' <summary>
		''' The <code>int</code> constant for the IDL data type <code>enum</code>.
		''' </summary>
		Public Const _tk_enum As Integer = 17

		''' <summary>
		''' The <code>int</code> constant for the IDL data type <code>string</code>.
		''' </summary>
		Public Const _tk_string As Integer = 18

		''' <summary>
		''' The <code>int</code> constant for the IDL data type <code>sequence</code>.
		''' </summary>
		Public Const _tk_sequence As Integer = 19

		''' <summary>
		''' The <code>int</code> constant for the IDL data type <code>array</code>.
		''' </summary>
		Public Const _tk_array As Integer = 20

		''' <summary>
		''' The <code>int</code> constant for the IDL data type <code>alias</code>.
		''' </summary>
		Public Const _tk_alias As Integer = 21

		''' <summary>
		''' The <code>int</code> constant for the IDL data type <code>except</code>.
		''' </summary>
		Public Const _tk_except As Integer = 22

		''' <summary>
		''' The <code>int</code> constant for the IDL data type <code>longlong</code>.
		''' </summary>
		Public Const _tk_longlong As Integer = 23

		''' <summary>
		''' The <code>int</code> constant for the IDL data type <code>ulonglong</code>.
		''' </summary>
		Public Const _tk_ulonglong As Integer = 24

		''' <summary>
		''' The <code>int</code> constant for the IDL data type <code>longdouble</code>.
		''' </summary>
		Public Const _tk_longdouble As Integer = 25

		''' <summary>
		''' The <code>int</code> constant for the IDL data type <code>wchar</code>.
		''' </summary>
		Public Const _tk_wchar As Integer = 26

		''' <summary>
		''' The <code>int</code> constant for the IDL data type <code>wstring</code>.
		''' </summary>
		Public Const _tk_wstring As Integer = 27

		''' <summary>
		''' The <code>int</code> constant for the IDL data type <code>fixed</code>.
		''' </summary>
		Public Const _tk_fixed As Integer = 28

		''' <summary>
		''' The <code>int</code> constant for the IDL data type <code>value</code>.
		''' </summary>
		Public Const _tk_value As Integer = 29 ' orbos 98-01-18: Objects By Value

		''' <summary>
		''' The <code>int</code> constant for the IDL data type <code>value_box</code>.
		''' </summary>
		Public Const _tk_value_box As Integer = 30 ' orbos 98-01-18: Objects By Value

		''' <summary>
		''' The <code>int</code> constant for the IDL data type <code>native</code>.
		''' </summary>
		Public Const _tk_native As Integer = 31 ' Verify

		''' <summary>
		''' The <code>int</code> constant for the IDL data type <code>abstract interface</code>.
		''' </summary>
		Public Const _tk_abstract_interface As Integer = 32


		''' <summary>
		''' The <code>TCKind</code> constant whose <code>value</code> field is
		''' initialized with <code>TCKind._tk_null</code>.
		''' </summary>
		Public Shared ReadOnly tk_null As New TCKind(_tk_null)

		''' <summary>
		''' The <code>TCKind</code> constant whose <code>value</code> field is
		''' initialized with <code>TCKind._tk_void</code>.
		''' </summary>
		Public Shared ReadOnly tk_void As New TCKind(_tk_void)

		''' <summary>
		''' The <code>TCKind</code> constant whose <code>value</code> field is
		''' initialized with <code>TCKind._tk_short</code>.
		''' </summary>
		Public Shared ReadOnly tk_short As New TCKind(_tk_short)

		''' <summary>
		''' The <code>TCKind</code> constant whose <code>value</code> field is
		''' initialized with <code>TCKind._tk_long</code>.
		''' </summary>
		Public Shared ReadOnly tk_long As New TCKind(_tk_long)

		''' <summary>
		''' The <code>TCKind</code> constant whose <code>value</code> field is
		''' initialized with <code>TCKind._tk_ushort</code>.
		''' </summary>
		Public Shared ReadOnly tk_ushort As New TCKind(_tk_ushort)

		''' <summary>
		''' The <code>TCKind</code> constant whose <code>value</code> field is
		''' initialized with <code>TCKind._tk_ulong</code>.
		''' </summary>
		Public Shared ReadOnly tk_ulong As New TCKind(_tk_ulong)

		''' <summary>
		''' The <code>TCKind</code> constant whose <code>value</code> field is
		''' initialized with <code>TCKind._tk_float</code>.
		''' </summary>
		Public Shared ReadOnly tk_float As New TCKind(_tk_float)

		''' <summary>
		''' The <code>TCKind</code> constant whose <code>value</code> field is
		''' initialized with <code>TCKind._tk_double</code>.
		''' </summary>
		Public Shared ReadOnly tk_double As New TCKind(_tk_double)

		''' <summary>
		''' The <code>TCKind</code> constant whose <code>value</code> field is
		''' initialized with <code>TCKind._tk_boolean</code>.
		''' </summary>
		Public Shared ReadOnly tk_boolean As New TCKind(_tk_boolean)

		''' <summary>
		''' The <code>TCKind</code> constant whose <code>value</code> field is
		''' initialized with <code>TCKind._tk_char</code>.
		''' </summary>
		Public Shared ReadOnly tk_char As New TCKind(_tk_char)

		''' <summary>
		''' The <code>TCKind</code> constant whose <code>value</code> field is
		''' initialized with <code>TCKind._tk_octet</code>.
		''' </summary>
		Public Shared ReadOnly tk_octet As New TCKind(_tk_octet)

		''' <summary>
		''' The <code>TCKind</code> constant whose <code>value</code> field is
		''' initialized with <code>TCKind._tk_any</code>.
		''' </summary>
		Public Shared ReadOnly tk_any As New TCKind(_tk_any)

		''' <summary>
		''' The <code>TCKind</code> constant whose <code>value</code> field is
		''' initialized with <code>TCKind._tk_TypeCode</code>.
		''' </summary>
		Public Shared ReadOnly tk_TypeCode As New TCKind(_tk_TypeCode)

		''' <summary>
		''' The <code>TCKind</code> constant whose <code>value</code> field is
		''' initialized with <code>TCKind._tk_Principal</code>.
		''' </summary>
		Public Shared ReadOnly tk_Principal As New TCKind(_tk_Principal)

		''' <summary>
		''' The <code>TCKind</code> constant whose <code>value</code> field is
		''' initialized with <code>TCKind._tk_objref</code>.
		''' </summary>
		Public Shared ReadOnly tk_objref As New TCKind(_tk_objref)

		''' <summary>
		''' The <code>TCKind</code> constant whose <code>value</code> field is
		''' initialized with <code>TCKind._tk_struct</code>.
		''' </summary>
		Public Shared ReadOnly tk_struct As New TCKind(_tk_struct)

		''' <summary>
		''' The <code>TCKind</code> constant whose <code>value</code> field is
		''' initialized with <code>TCKind._tk_union</code>.
		''' </summary>
		Public Shared ReadOnly tk_union As New TCKind(_tk_union)

		''' <summary>
		''' The <code>TCKind</code> constant whose <code>value</code> field is
		''' initialized with <code>TCKind._tk_enum</code>.
		''' </summary>
		Public Shared ReadOnly tk_enum As New TCKind(_tk_enum)

		''' <summary>
		''' The <code>TCKind</code> constant whose <code>value</code> field is
		''' initialized with <code>TCKind._tk_string</code>.
		''' </summary>
		Public Shared ReadOnly tk_string As New TCKind(_tk_string)

		''' <summary>
		''' The <code>TCKind</code> constant whose <code>value</code> field is
		''' initialized with <code>TCKind._tk_sequence</code>.
		''' </summary>
		Public Shared ReadOnly tk_sequence As New TCKind(_tk_sequence)

		''' <summary>
		''' The <code>TCKind</code> constant whose <code>value</code> field is
		''' initialized with <code>TCKind._tk_array</code>.
		''' </summary>
		Public Shared ReadOnly tk_array As New TCKind(_tk_array)

		''' <summary>
		''' The <code>TCKind</code> constant whose <code>value</code> field is
		''' initialized with <code>TCKind._tk_alias</code>.
		''' </summary>
		Public Shared ReadOnly tk_alias As New TCKind(_tk_alias)

		''' <summary>
		''' The <code>TCKind</code> constant whose <code>value</code> field is
		''' initialized with <code>TCKind._tk_except</code>.
		''' </summary>
		Public Shared ReadOnly tk_except As New TCKind(_tk_except)

		''' <summary>
		''' The <code>TCKind</code> constant whose <code>value</code> field is
		''' initialized with <code>TCKind._tk_longlong</code>.
		''' </summary>
		Public Shared ReadOnly tk_longlong As New TCKind(_tk_longlong)

		''' <summary>
		''' The <code>TCKind</code> constant whose <code>value</code> field is
		''' initialized with <code>TCKind._tk_ulonglong</code>.
		''' </summary>
		Public Shared ReadOnly tk_ulonglong As New TCKind(_tk_ulonglong)

		''' <summary>
		''' The <code>TCKind</code> constant whose <code>value</code> field is
		''' initialized with <code>TCKind._tk_longdouble</code>.
		''' </summary>
		Public Shared ReadOnly tk_longdouble As New TCKind(_tk_longdouble)

		''' <summary>
		''' The <code>TCKind</code> constant whose <code>value</code> field is
		''' initialized with <code>TCKind._tk_wchar</code>.
		''' </summary>
		Public Shared ReadOnly tk_wchar As New TCKind(_tk_wchar)

		''' <summary>
		''' The <code>TCKind</code> constant whose <code>value</code> field is
		''' initialized with <code>TCKind._tk_wstring</code>.
		''' </summary>
		Public Shared ReadOnly tk_wstring As New TCKind(_tk_wstring)

		''' <summary>
		''' The <code>TCKind</code> constant whose <code>value</code> field is
		''' initialized with <code>TCKind._tk_fixed</code>.
		''' </summary>
		Public Shared ReadOnly tk_fixed As New TCKind(_tk_fixed)

		' orbos 98-01-18: Objects By Value -- begin

		''' <summary>
		''' The <code>TCKind</code> constant whose <code>value</code> field is
		''' initialized with <code>TCKind._tk_value</code>.
		''' </summary>
		Public Shared ReadOnly tk_value As New TCKind(_tk_value)

		''' <summary>
		''' The <code>TCKind</code> constant whose <code>value</code> field is
		''' initialized with <code>TCKind._tk_value_box</code>.
		''' </summary>
		Public Shared ReadOnly tk_value_box As New TCKind(_tk_value_box)
		' orbos 98-01-18: Objects By Value -- end

		''' <summary>
		''' The <code>TCKind</code> constant whose <code>value</code> field is
		''' initialized with <code>TCKind._tk_native</code>.
		''' </summary>
		Public Shared ReadOnly tk_native As New TCKind(_tk_native)

		''' <summary>
		''' The <code>TCKind</code> constant whose <code>value</code> field is
		''' initialized with <code>TCKind._tk_abstract_interface</code>.
		''' </summary>
		Public Shared ReadOnly tk_abstract_interface As New TCKind(_tk_abstract_interface)




		''' <summary>
		''' Retrieves the value of this <code>TCKind</code> instance.
		''' </summary>
		''' <returns>  the <code>int</code> that represents the kind of
		''' IDL data type for this <code>TCKind</code> instance </returns>
		Public Overridable Function value() As Integer
			Return _value
		End Function

		''' <summary>
		''' Converts the given <code>int</code> to the corresponding
		''' <code>TCKind</code> instance.
		''' </summary>
		''' <param name="i"> the <code>int</code> to convert.  It must be one of
		'''         the <code>int</code> constants in the class
		'''         <code>TCKind</code>. </param>
		''' <returns>  the <code>TCKind</code> instance whose <code>value</code>
		''' field matches the given <code>int</code> </returns>
		''' <exception cref="BAD_PARAM">  if the given <code>int</code> does not
		''' match the <code>_value</code> field of
		''' any <code>TCKind</code> instance </exception>
		Public Shared Function from_int(ByVal i As Integer) As TCKind
			Select Case i
			Case _tk_null
				Return tk_null
			Case _tk_void
				Return tk_void
			Case _tk_short
				Return tk_short
			Case _tk_long
				Return tk_long
			Case _tk_ushort
				Return tk_ushort
			Case _tk_ulong
				Return tk_ulong
			Case _tk_float
				Return tk_float
			Case _tk_double
				Return tk_double
			Case _tk_boolean
				Return tk_boolean
			Case _tk_char
				Return tk_char
			Case _tk_octet
				Return tk_octet
			Case _tk_any
				Return tk_any
			Case _tk_TypeCode
				Return tk_TypeCode
			Case _tk_Principal
				Return tk_Principal
			Case _tk_objref
				Return tk_objref
			Case _tk_struct
				Return tk_struct
			Case _tk_union
				Return tk_union
			Case _tk_enum
				Return tk_enum
			Case _tk_string
				Return tk_string
			Case _tk_sequence
				Return tk_sequence
			Case _tk_array
				Return tk_array
			Case _tk_alias
				Return tk_alias
			Case _tk_except
				Return tk_except
			Case _tk_longlong
				Return tk_longlong
			Case _tk_ulonglong
				Return tk_ulonglong
			Case _tk_longdouble
				Return tk_longdouble
			Case _tk_wchar
				Return tk_wchar
			Case _tk_wstring
				Return tk_wstring
			Case _tk_fixed
				Return tk_fixed
			Case _tk_value ' orbos 98-01-18: Objects By Value
				Return tk_value
			Case _tk_value_box ' orbos 98-01-18: Objects By Value
				Return tk_value_box
			Case _tk_native
				Return tk_native
			Case _tk_abstract_interface
				Return tk_abstract_interface
			Case Else
				Throw New org.omg.CORBA.BAD_PARAM
			End Select
		End Function


		''' <summary>
		''' Creates a new <code>TCKind</code> instance initialized with the given
		''' <code>int</code>. </summary>
		''' @deprecated Do not use this constructor as this method should be private
		''' according to the OMG specification. Use <seealso cref="#from_int(int)"/> instead.
		''' 
		''' <param name="_value"> the <code>int</code> to convert.  It must be one of
		'''         the <code>int</code> constants in the class
		'''         <code>TCKind</code>. </param>
		<Obsolete("Do not use this constructor as this method should be private")> _
		Protected Friend Sub New(ByVal _value As Integer)
			Me._value = _value
		End Sub
		Private _value As Integer
	End Class

End Namespace