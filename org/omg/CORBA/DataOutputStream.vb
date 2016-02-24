'
' * Copyright (c) 1998, 1999, Oracle and/or its affiliates. All rights reserved.
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
	''' Defines the methods used to write primitive data types to output streams
	''' for marshalling custom value types.  This interface is used by user
	''' written custom marshalling code for custom value types. </summary>
	''' <seealso cref= org.omg.CORBA.DataInputStream </seealso>
	''' <seealso cref= org.omg.CORBA.CustomMarshal </seealso>
	Public Interface DataOutputStream
		Inherits org.omg.CORBA.portable.ValueBase

		''' <summary>
		''' Writes the Any value to the output stream. </summary>
		''' <param name="value"> The value to be written. </param>
		Sub write_any(ByVal value As org.omg.CORBA.Any)

		''' <summary>
		''' Writes the boolean value to the output stream. </summary>
		''' <param name="value"> The value to be written. </param>
		Sub write_boolean(ByVal value As Boolean)

		''' <summary>
		''' Writes the IDL character value to the output stream. </summary>
		''' <param name="value"> The value to be written. </param>
		Sub write_char(ByVal value As Char)

		''' <summary>
		''' Writes the IDL wide character value to the output stream. </summary>
		''' <param name="value"> The value to be written. </param>
		Sub write_wchar(ByVal value As Char)

		''' <summary>
		''' Writes the IDL octet value (represented as a Java byte) to the output stream. </summary>
		''' <param name="value"> The value to be written. </param>
		Sub write_octet(ByVal value As SByte)

		''' <summary>
		''' Writes the IDL short value to the output stream. </summary>
		''' <param name="value"> The value to be written. </param>
		Sub write_short(ByVal value As Short)

		''' <summary>
		''' Writes the IDL unsigned short value (represented as a Java short
		''' value) to the output stream. </summary>
		''' <param name="value"> The value to be written. </param>
		Sub write_ushort(ByVal value As Short)

		''' <summary>
		''' Writes the IDL long value (represented as a Java int) to the output stream. </summary>
		''' <param name="value"> The value to be written. </param>
		Sub write_long(ByVal value As Integer)

		''' <summary>
		''' Writes the IDL unsigned long value (represented as a Java int) to the output stream. </summary>
		''' <param name="value"> The value to be written. </param>
		Sub write_ulong(ByVal value As Integer)

		''' <summary>
		''' Writes the IDL long long value (represented as a Java long) to the output stream. </summary>
		''' <param name="value"> The value to be written. </param>
		Sub write_longlong(ByVal value As Long)

		''' <summary>
		''' Writes the IDL unsigned long long value (represented as a Java long)
		''' to the output stream. </summary>
		''' <param name="value"> The value to be written. </param>
		Sub write_ulonglong(ByVal value As Long)

		''' <summary>
		''' Writes the IDL float value to the output stream. </summary>
		''' <param name="value"> The value to be written. </param>
		Sub write_float(ByVal value As Single)

		''' <summary>
		''' Writes the IDL double value to the output stream. </summary>
		''' <param name="value"> The value to be written. </param>
		Sub write_double(ByVal value As Double)

		' write_longdouble not supported by IDL/Java mapping

		''' <summary>
		''' Writes the IDL string value to the output stream. </summary>
		''' <param name="value"> The value to be written. </param>
		Sub write_string(ByVal value As String)

		''' <summary>
		''' Writes the IDL wide string value (represented as a Java String) to the output stream. </summary>
		''' <param name="value"> The value to be written. </param>
		Sub write_wstring(ByVal value As String)

		''' <summary>
		''' Writes the IDL CORBA::Object value to the output stream. </summary>
		''' <param name="value"> The value to be written. </param>
		Sub write_Object(ByVal value As org.omg.CORBA.Object)

		''' <summary>
		''' Writes the IDL Abstract interface type to the output stream. </summary>
		''' <param name="value"> The value to be written. </param>
		Sub write_Abstract(ByVal value As Object)

		''' <summary>
		''' Writes the IDL value type value to the output stream. </summary>
		''' <param name="value"> The value to be written. </param>
		Sub write_Value(ByVal value As java.io.Serializable)

		''' <summary>
		''' Writes the typecode to the output stream. </summary>
		''' <param name="value"> The value to be written. </param>
		Sub write_TypeCode(ByVal value As org.omg.CORBA.TypeCode)

		''' <summary>
		''' Writes the array of IDL Anys from offset for length elements to the
		''' output stream. </summary>
		''' <param name="seq"> The array to be written. </param>
		''' <param name="offset"> The index into seq of the first element to write to the
		''' output stream. </param>
		''' <param name="length"> The number of elements to write to the output stream. </param>
		Sub write_any_array(ByVal seq As org.omg.CORBA.Any(), ByVal offset As Integer, ByVal length As Integer)

		''' <summary>
		''' Writes the array of IDL booleans from offset for length elements to the
		''' output stream. </summary>
		''' <param name="seq"> The array to be written. </param>
		''' <param name="offset"> The index into seq of the first element to write to the
		''' output stream. </param>
		''' <param name="length"> The number of elements to write to the output stream. </param>
		Sub write_boolean_array(ByVal seq As Boolean(), ByVal offset As Integer, ByVal length As Integer)

		''' <summary>
		''' Writes the array of IDL characters from offset for length elements to the
		''' output stream. </summary>
		''' <param name="seq"> The array to be written. </param>
		''' <param name="offset"> The index into seq of the first element to write to the
		''' output stream. </param>
		''' <param name="length"> The number of elements to write to the output stream. </param>
		Sub write_char_array(ByVal seq As Char(), ByVal offset As Integer, ByVal length As Integer)

		''' <summary>
		''' Writes the array of IDL wide characters from offset for length elements to the
		''' output stream. </summary>
		''' <param name="seq"> The array to be written. </param>
		''' <param name="offset"> The index into seq of the first element to write to the
		''' output stream. </param>
		''' <param name="length"> The number of elements to write to the output stream. </param>
		Sub write_wchar_array(ByVal seq As Char(), ByVal offset As Integer, ByVal length As Integer)

		''' <summary>
		''' Writes the array of IDL octets from offset for length elements to the
		''' output stream. </summary>
		''' <param name="seq"> The array to be written. </param>
		''' <param name="offset"> The index into seq of the first element to write to the
		''' output stream. </param>
		''' <param name="length"> The number of elements to write to the output stream. </param>
		Sub write_octet_array(ByVal seq As SByte(), ByVal offset As Integer, ByVal length As Integer)

		''' <summary>
		''' Writes the array of IDL shorts from offset for length elements to the
		''' output stream. </summary>
		''' <param name="seq"> The array to be written. </param>
		''' <param name="offset"> The index into seq of the first element to write to the
		''' output stream. </param>
		''' <param name="length"> The number of elements to write to the output stream. </param>
		Sub write_short_array(ByVal seq As Short(), ByVal offset As Integer, ByVal length As Integer)

		''' <summary>
		''' Writes the array of IDL unsigned shorts (represented as Java shorts)
		''' from offset for length elements to the output stream. </summary>
		''' <param name="seq"> The array to be written. </param>
		''' <param name="offset"> The index into seq of the first element to write to the
		''' output stream. </param>
		''' <param name="length"> The number of elements to write to the output stream. </param>
		Sub write_ushort_array(ByVal seq As Short(), ByVal offset As Integer, ByVal length As Integer)

		''' <summary>
		''' Writes the array of IDL longs from offset for length elements to the
		''' output stream. </summary>
		''' <param name="seq"> The array to be written. </param>
		''' <param name="offset"> The index into seq of the first element to write to the
		''' output stream. </param>
		''' <param name="length"> The number of elements to write to the output stream. </param>
		Sub write_long_array(ByVal seq As Integer(), ByVal offset As Integer, ByVal length As Integer)

		''' <summary>
		''' Writes the array of IDL unsigned longs (represented as Java ints)
		''' from offset for length elements to the output stream. </summary>
		''' <param name="seq"> The array to be written. </param>
		''' <param name="offset"> The index into seq of the first element to write to the
		''' output stream. </param>
		''' <param name="length"> The number of elements to write to the output stream. </param>
		Sub write_ulong_array(ByVal seq As Integer(), ByVal offset As Integer, ByVal length As Integer)

		''' <summary>
		''' Writes the array of IDL unsigned long longs (represented as Java longs)
		''' from offset for length elements to the output stream. </summary>
		''' <param name="seq"> The array to be written. </param>
		''' <param name="offset"> The index into seq of the first element to write to the
		''' output stream. </param>
		''' <param name="length"> The number of elements to write to the output stream. </param>
		Sub write_ulonglong_array(ByVal seq As Long(), ByVal offset As Integer, ByVal length As Integer)

		''' <summary>
		''' Writes the array of IDL long longs from offset for length elements to the
		''' output stream. </summary>
		''' <param name="seq"> The array to be written. </param>
		''' <param name="offset"> The index into seq of the first element to write to the
		''' output stream. </param>
		''' <param name="length"> The number of elements to write to the output stream. </param>
		Sub write_longlong_array(ByVal seq As Long(), ByVal offset As Integer, ByVal length As Integer)

		''' <summary>
		''' Writes the array of IDL floats from offset for length elements to the
		''' output stream. </summary>
		''' <param name="seq"> The array to be written. </param>
		''' <param name="offset"> The index into seq of the first element to write to the
		''' output stream. </param>
		''' <param name="length"> The number of elements to write to the output stream. </param>
		Sub write_float_array(ByVal seq As Single(), ByVal offset As Integer, ByVal length As Integer)

		''' <summary>
		''' Writes the array of IDL doubles from offset for length elements to the
		''' output stream. </summary>
		''' <param name="seq"> The array to be written. </param>
		''' <param name="offset"> The index into seq of the first element to write to the
		''' output stream. </param>
		''' <param name="length"> The number of elements to write to the output stream. </param>
		Sub write_double_array(ByVal seq As Double(), ByVal offset As Integer, ByVal length As Integer)
	End Interface ' interface DataOutputStream

End Namespace