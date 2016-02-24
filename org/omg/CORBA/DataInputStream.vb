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
	''' Defines the methods used to read primitive data types from input streams
	''' for unmarshaling custom value types.  This interface is used by user
	''' written custom unmarshaling code for custom value types. </summary>
	''' <seealso cref= org.omg.CORBA.DataOutputStream </seealso>
	''' <seealso cref= org.omg.CORBA.CustomMarshal </seealso>
	Public Interface DataInputStream
		Inherits org.omg.CORBA.portable.ValueBase

		''' <summary>
		''' Reads an IDL <code>Any</code> value from the input stream. </summary>
		''' <returns>  the <code>Any</code> read. </returns>
		''' @throws <code>org.omg.CORBA.MARSHAL</code>
		''' If an inconsistency is detected, including not having registered
		''' a streaming policy, then the standard system exception MARSHAL is raised. </exception>
		Function read_any() As org.omg.CORBA.Any

		''' <summary>
		''' Reads an IDL boolean value from the input stream. </summary>
		''' <returns>  the boolean read. </returns>
		''' @throws <code>org.omg.CORBA.MARSHAL</code>
		''' If an inconsistency is detected, including not having registered
		''' a streaming policy, then the standard system exception MARSHAL is raised. </exception>
		Function read_boolean() As Boolean

		''' <summary>
		''' Reads an IDL character value from the input stream. </summary>
		''' <returns>  the character read. </returns>
		''' @throws <code>org.omg.CORBA.MARSHAL</code>
		''' If an inconsistency is detected, including not having registered
		''' a streaming policy, then the standard system exception MARSHAL is raised. </exception>
		Function read_char() As Char

		''' <summary>
		''' Reads an IDL wide character value from the input stream. </summary>
		''' <returns>  the wide character read. </returns>
		''' @throws <code>org.omg.CORBA.MARSHAL</code>
		''' If an inconsistency is detected, including not having registered
		''' a streaming policy, then the standard system exception MARSHAL is raised. </exception>
		Function read_wchar() As Char

		''' <summary>
		''' Reads an IDL octet value from the input stream. </summary>
		''' <returns>  the octet value read. </returns>
		''' @throws <code>org.omg.CORBA.MARSHAL</code>
		''' If an inconsistency is detected, including not having registered
		''' a streaming policy, then the standard system exception MARSHAL is raised. </exception>
		Function read_octet() As SByte

		''' <summary>
		''' Reads an IDL short from the input stream. </summary>
		''' <returns>  the short read. </returns>
		''' @throws <code>org.omg.CORBA.MARSHAL</code>
		''' If an inconsistency is detected, including not having registered
		''' a streaming policy, then the standard system exception MARSHAL is raised. </exception>
		Function read_short() As Short

		''' <summary>
		''' Reads an IDL unsigned short from the input stream. </summary>
		''' <returns>  the unsigned short read. </returns>
		''' @throws <code>org.omg.CORBA.MARSHAL</code>
		''' If an inconsistency is detected, including not having registered
		''' a streaming policy, then the standard system exception MARSHAL is raised. </exception>
		Function read_ushort() As Short

		''' <summary>
		''' Reads an IDL long from the input stream. </summary>
		''' <returns>  the long read. </returns>
		''' @throws <code>org.omg.CORBA.MARSHAL</code>
		''' If an inconsistency is detected, including not having registered
		''' a streaming policy, then the standard system exception MARSHAL is raised. </exception>
		Function read_long() As Integer

		''' <summary>
		''' Reads an IDL unsigned long from the input stream. </summary>
		''' <returns>  the unsigned long read. </returns>
		''' @throws <code>org.omg.CORBA.MARSHAL</code>
		''' If an inconsistency is detected, including not having registered
		''' a streaming policy, then the standard system exception MARSHAL is raised. </exception>
		Function read_ulong() As Integer

		''' <summary>
		''' Reads an IDL long long from the input stream. </summary>
		''' <returns>  the long long read. </returns>
		''' @throws <code>org.omg.CORBA.MARSHAL</code>
		''' If an inconsistency is detected, including not having registered
		''' a streaming policy, then the standard system exception MARSHAL is raised. </exception>
		Function read_longlong() As Long

		''' <summary>
		''' Reads an unsigned IDL long long from the input stream. </summary>
		''' <returns>  the unsigned long long read. </returns>
		''' @throws <code>org.omg.CORBA.MARSHAL</code>
		''' If an inconsistency is detected, including not having registered
		''' a streaming policy, then the standard system exception MARSHAL is raised. </exception>
		Function read_ulonglong() As Long

		''' <summary>
		''' Reads an IDL float from the input stream. </summary>
		''' <returns>  the float read. </returns>
		''' @throws <code>org.omg.CORBA.MARSHAL</code>
		''' If an inconsistency is detected, including not having registered
		''' a streaming policy, then the standard system exception MARSHAL is raised. </exception>
		Function read_float() As Single

		''' <summary>
		''' Reads an IDL double from the input stream. </summary>
		''' <returns>  the double read. </returns>
		''' @throws <code>org.omg.CORBA.MARSHAL</code>
		''' If an inconsistency is detected, including not having registered
		''' a streaming policy, then the standard system exception MARSHAL is raised. </exception>
		Function read_double() As Double
		' read_longdouble not supported by IDL/Java mapping

		''' <summary>
		''' Reads an IDL string from the input stream. </summary>
		''' <returns>  the string read. </returns>
		''' @throws <code>org.omg.CORBA.MARSHAL</code>
		''' If an inconsistency is detected, including not having registered
		''' a streaming policy, then the standard system exception MARSHAL is raised. </exception>
		Function read_string() As String

		''' <summary>
		''' Reads an IDL wide string from the input stream. </summary>
		''' <returns>  the wide string read. </returns>
		''' @throws <code>org.omg.CORBA.MARSHAL</code>
		''' If an inconsistency is detected, including not having registered
		''' a streaming policy, then the standard system exception MARSHAL is raised. </exception>
		Function read_wstring() As String

		''' <summary>
		''' Reads an IDL CORBA::Object from the input stream. </summary>
		''' <returns>  the CORBA::Object read. </returns>
		''' @throws <code>org.omg.CORBA.MARSHAL</code>
		''' If an inconsistency is detected, including not having registered
		''' a streaming policy, then the standard system exception MARSHAL is raised. </exception>
		Function read_Object() As org.omg.CORBA.Object

		''' <summary>
		''' Reads an IDL Abstract interface from the input stream. </summary>
		''' <returns>  the Abstract interface read. </returns>
		''' @throws <code>org.omg.CORBA.MARSHAL</code>
		''' If an inconsistency is detected, including not having registered
		''' a streaming policy, then the standard system exception MARSHAL is raised. </exception>
		Function read_Abstract() As Object

		''' <summary>
		''' Reads an IDL value type from the input stream. </summary>
		''' <returns>  the value type read. </returns>
		''' @throws <code>org.omg.CORBA.MARSHAL</code>
		''' If an inconsistency is detected, including not having registered
		''' a streaming policy, then the standard system exception MARSHAL is raised. </exception>
		Function read_Value() As java.io.Serializable

		''' <summary>
		''' Reads an IDL typecode from the input stream. </summary>
		''' <returns>  the typecode read. </returns>
		''' @throws <code>org.omg.CORBA.MARSHAL</code>
		''' If an inconsistency is detected, including not having registered
		''' a streaming policy, then the standard system exception MARSHAL is raised. </exception>
		Function read_TypeCode() As org.omg.CORBA.TypeCode

		''' <summary>
		''' Reads array of IDL Anys from offset for length elements from the
		''' input stream. </summary>
		''' <param name="seq"> The out parameter holder for the array to be read. </param>
		''' <param name="offset"> The index into seq of the first element to read from the
		''' input stream. </param>
		''' <param name="length"> The number of elements to read from the input stream. </param>
		''' @throws <code>org.omg.CORBA.MARSHAL</code>
		''' If an inconsistency is detected, including not having registered
		''' a streaming policy, then the standard system exception MARSHAL is raised. </exception>
		Sub read_any_array(ByVal seq As org.omg.CORBA.AnySeqHolder, ByVal offset As Integer, ByVal length As Integer)

		''' <summary>
		''' Reads array of IDL booleans from offset for length elements from the
		''' input stream. </summary>
		''' <param name="seq"> The out parameter holder for the array to be read. </param>
		''' <param name="offset"> The index into seq of the first element to read from the
		''' input stream. </param>
		''' <param name="length"> The number of elements to read from the input stream. </param>
		''' @throws <code>org.omg.CORBA.MARSHAL</code>
		''' If an inconsistency is detected, including not having registered
		''' a streaming policy, then the standard system exception MARSHAL is raised. </exception>
		Sub read_boolean_array(ByVal seq As org.omg.CORBA.BooleanSeqHolder, ByVal offset As Integer, ByVal length As Integer)

		''' <summary>
		''' Reads array of IDL characters from offset for length elements from the
		''' input stream. </summary>
		''' <param name="seq"> The out parameter holder for the array to be read. </param>
		''' <param name="offset"> The index into seq of the first element to read from the
		''' input stream. </param>
		''' <param name="length"> The number of elements to read from the input stream. </param>
		''' @throws <code>org.omg.CORBA.MARSHAL</code>
		''' If an inconsistency is detected, including not having registered
		''' a streaming policy, then the standard system exception MARSHAL is raised. </exception>
		Sub read_char_array(ByVal seq As org.omg.CORBA.CharSeqHolder, ByVal offset As Integer, ByVal length As Integer)

		''' <summary>
		''' Reads array of IDL wide characters from offset for length elements from the
		''' input stream. </summary>
		''' <param name="seq"> The out parameter holder for the array to be read. </param>
		''' <param name="offset"> The index into seq of the first element to read from the
		''' input stream. </param>
		''' <param name="length"> The number of elements to read from the input stream. </param>
		''' @throws <code>org.omg.CORBA.MARSHAL</code>
		''' If an inconsistency is detected, including not having registered
		''' a streaming policy, then the standard system exception MARSHAL is raised. </exception>
		Sub read_wchar_array(ByVal seq As org.omg.CORBA.WCharSeqHolder, ByVal offset As Integer, ByVal length As Integer)

		''' <summary>
		''' Reads array of IDL octets from offset for length elements from the
		''' input stream. </summary>
		''' <param name="seq"> The out parameter holder for the array to be read. </param>
		''' <param name="offset"> The index into seq of the first element to read from the
		''' input stream. </param>
		''' <param name="length"> The number of elements to read from the input stream. </param>
		''' @throws <code>org.omg.CORBA.MARSHAL</code>
		''' If an inconsistency is detected, including not having registered
		''' a streaming policy, then the standard system exception MARSHAL is raised. </exception>
		Sub read_octet_array(ByVal seq As org.omg.CORBA.OctetSeqHolder, ByVal offset As Integer, ByVal length As Integer)

		''' <summary>
		''' Reads array of IDL shorts from offset for length elements from the
		''' input stream. </summary>
		''' <param name="seq"> The out parameter holder for the array to be read. </param>
		''' <param name="offset"> The index into seq of the first element to read from the
		''' input stream. </param>
		''' <param name="length"> The number of elements to read from the input stream. </param>
		''' @throws <code>org.omg.CORBA.MARSHAL</code>
		''' If an inconsistency is detected, including not having registered
		''' a streaming policy, then the standard system exception MARSHAL is raised. </exception>
		Sub read_short_array(ByVal seq As org.omg.CORBA.ShortSeqHolder, ByVal offset As Integer, ByVal length As Integer)

		''' <summary>
		''' Reads array of IDL unsigned shorts from offset for length elements from the
		''' input stream. </summary>
		''' <param name="seq"> The out parameter holder for the array to be read. </param>
		''' <param name="offset"> The index into seq of the first element to read from the
		''' input stream. </param>
		''' <param name="length"> The number of elements to read from the input stream. </param>
		''' @throws <code>org.omg.CORBA.MARSHAL</code>
		''' If an inconsistency is detected, including not having registered
		''' a streaming policy, then the standard system exception MARSHAL is raised. </exception>
		Sub read_ushort_array(ByVal seq As org.omg.CORBA.UShortSeqHolder, ByVal offset As Integer, ByVal length As Integer)

		''' <summary>
		''' Reads array of IDL longs from offset for length elements from the
		''' input stream. </summary>
		''' <param name="seq"> The out parameter holder for the array to be read. </param>
		''' <param name="offset"> The index into seq of the first element to read from the
		''' input stream. </param>
		''' <param name="length"> The number of elements to read from the input stream. </param>
		''' @throws <code>org.omg.CORBA.MARSHAL</code>
		''' If an inconsistency is detected, including not having registered
		''' a streaming policy, then the standard system exception MARSHAL is raised. </exception>
		Sub read_long_array(ByVal seq As org.omg.CORBA.LongSeqHolder, ByVal offset As Integer, ByVal length As Integer)

		''' <summary>
		''' Reads array of IDL unsigned longs from offset for length elements from the
		''' input stream. </summary>
		''' <param name="seq"> The out parameter holder for the array to be read. </param>
		''' <param name="offset"> The index into seq of the first element to read from the
		''' input stream. </param>
		''' <param name="length"> The number of elements to read from the input stream. </param>
		''' @throws <code>org.omg.CORBA.MARSHAL</code>
		''' If an inconsistency is detected, including not having registered
		''' a streaming policy, then the standard system exception MARSHAL is raised. </exception>
		Sub read_ulong_array(ByVal seq As org.omg.CORBA.ULongSeqHolder, ByVal offset As Integer, ByVal length As Integer)

		''' <summary>
		''' Reads array of IDL unsigned long longs from offset for length elements from the
		''' input stream. </summary>
		''' <param name="seq"> The out parameter holder for the array to be read. </param>
		''' <param name="offset"> The index into seq of the first element to read from the
		''' input stream. </param>
		''' <param name="length"> The number of elements to read from the input stream. </param>
		''' @throws <code>org.omg.CORBA.MARSHAL</code>
		''' If an inconsistency is detected, including not having registered
		''' a streaming policy, then the standard system exception MARSHAL is raised. </exception>
		Sub read_ulonglong_array(ByVal seq As org.omg.CORBA.ULongLongSeqHolder, ByVal offset As Integer, ByVal length As Integer)

		''' <summary>
		''' Reads array of IDL long longs from offset for length elements from the
		''' input stream. </summary>
		''' <param name="seq"> The out parameter holder for the array to be read. </param>
		''' <param name="offset"> The index into seq of the first element to read from the
		''' input stream. </param>
		''' <param name="length"> The number of elements to read from the input stream. </param>
		''' @throws <code>org.omg.CORBA.MARSHAL</code>
		''' If an inconsistency is detected, including not having registered
		''' a streaming policy, then the standard system exception MARSHAL is raised. </exception>
		Sub read_longlong_array(ByVal seq As org.omg.CORBA.LongLongSeqHolder, ByVal offset As Integer, ByVal length As Integer)

		''' <summary>
		''' Reads array of IDL floats from offset for length elements from the
		''' input stream. </summary>
		''' <param name="seq"> The out parameter holder for the array to be read. </param>
		''' <param name="offset"> The index into seq of the first element to read from the
		''' input stream. </param>
		''' <param name="length"> The number of elements to read from the input stream. </param>
		''' @throws <code>org.omg.CORBA.MARSHAL</code>
		''' If an inconsistency is detected, including not having registered
		''' a streaming policy, then the standard system exception MARSHAL is raised. </exception>
		Sub read_float_array(ByVal seq As org.omg.CORBA.FloatSeqHolder, ByVal offset As Integer, ByVal length As Integer)

		''' <summary>
		''' Reads array of IDL doubles from offset for length elements from the
		''' input stream. </summary>
		''' <param name="seq"> The out parameter holder for the array to be read. </param>
		''' <param name="offset"> The index into seq of the first element to read from the
		''' input stream. </param>
		''' <param name="length"> The number of elements to read from the input stream. </param>
		''' @throws <code>org.omg.CORBA.MARSHAL</code>
		''' If an inconsistency is detected, including not having registered
		''' a streaming policy, then the standard system exception MARSHAL is raised. </exception>
		Sub read_double_array(ByVal seq As org.omg.CORBA.DoubleSeqHolder, ByVal offset As Integer, ByVal length As Integer)
	End Interface ' interface DataInputStream

End Namespace