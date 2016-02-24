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
Namespace org.omg.CORBA.portable


	''' <summary>
	''' InputStream is the Java API for reading IDL types
	''' from CDR marshal streams. These methods are used by the ORB to
	''' unmarshal IDL types as well as to extract IDL types out of Anys.
	''' The <code>_array</code> versions of the methods can be directly
	''' used to read sequences and arrays of IDL types.
	''' 
	''' @since   JDK1.2
	''' </summary>

	Public MustInherit Class InputStream
		Inherits java.io.InputStream

		''' <summary>
		''' Reads a boolean value from this input stream.
		''' </summary>
		''' <returns> the <code>boolean</code> value read from this input stream </returns>
		Public MustOverride Function read_boolean() As Boolean
		''' <summary>
		''' Reads a char value from this input stream.
		''' </summary>
		''' <returns> the <code>char</code> value read from this input stream </returns>
		Public MustOverride Function read_char() As Char
		''' <summary>
		''' Reads a wide char value from this input stream.
		''' </summary>
		''' <returns> the <code>char</code> value read from this input stream </returns>
		Public MustOverride Function read_wchar() As Char
		''' <summary>
		''' Reads an octet (that is, a byte) value from this input stream.
		''' </summary>
		''' <returns> the <code>byte</code> value read from this input stream </returns>
		Public MustOverride Function read_octet() As SByte
		''' <summary>
		''' Reads a short value from this input stream.
		''' </summary>
		''' <returns> the <code>short</code> value read from this input stream </returns>
		Public MustOverride Function read_short() As Short
		''' <summary>
		''' Reads a unsigned short value from this input stream.
		''' </summary>
		''' <returns> the <code>short</code> value read from this input stream </returns>
		Public MustOverride Function read_ushort() As Short
		''' <summary>
		''' Reads a CORBA long (that is, Java int) value from this input stream.
		''' </summary>
		''' <returns> the <code>int</code> value read from this input stream </returns>
		Public MustOverride Function read_long() As Integer
		''' <summary>
		''' Reads an unsigned CORBA long (that is, Java int) value from this input
		''' stream.
		''' </summary>
		''' <returns> the <code>int</code> value read from this input stream </returns>
		Public MustOverride Function read_ulong() As Integer
		''' <summary>
		''' Reads a CORBA longlong (that is, Java long) value from this input stream.
		''' </summary>
		''' <returns> the <code>long</code> value read from this input stream </returns>
		Public MustOverride Function read_longlong() As Long
		''' <summary>
		''' Reads a CORBA unsigned longlong (that is, Java long) value from this input
		''' stream.
		''' </summary>
		''' <returns> the <code>long</code> value read from this input stream </returns>
		Public MustOverride Function read_ulonglong() As Long
		''' <summary>
		''' Reads a float value from this input stream.
		''' </summary>
		''' <returns> the <code>float</code> value read from this input stream </returns>
		Public MustOverride Function read_float() As Single
		''' <summary>
		''' Reads a double value from this input stream.
		''' </summary>
		''' <returns> the <code>double</code> value read from this input stream </returns>
		Public MustOverride Function read_double() As Double
		''' <summary>
		''' Reads a string value from this input stream.
		''' </summary>
		''' <returns> the <code>String</code> value read from this input stream </returns>
		Public MustOverride Function read_string() As String
		''' <summary>
		''' Reads a wide string value from this input stream.
		''' </summary>
		''' <returns> the <code>String</code> value read from this input stream </returns>
		Public MustOverride Function read_wstring() As String

		''' <summary>
		''' Reads an array of booleans from this input stream. </summary>
		''' <param name="value"> returned array of booleans. </param>
		''' <param name="offset"> offset on the stream. </param>
		''' <param name="length"> length of buffer to read. </param>
		Public MustOverride Sub read_boolean_array(ByVal value As Boolean(), ByVal offset As Integer, ByVal length As Integer)
		''' <summary>
		''' Reads an array of chars from this input stream. </summary>
		''' <param name="value"> returned array of chars. </param>
		''' <param name="offset"> offset on the stream. </param>
		''' <param name="length"> length of buffer to read. </param>
		Public MustOverride Sub read_char_array(ByVal value As Char(), ByVal offset As Integer, ByVal length As Integer)
		''' <summary>
		''' Reads an array of wide chars from this input stream. </summary>
		''' <param name="value"> returned array of wide chars. </param>
		''' <param name="offset"> offset on the stream. </param>
		''' <param name="length"> length of buffer to read. </param>
		Public MustOverride Sub read_wchar_array(ByVal value As Char(), ByVal offset As Integer, ByVal length As Integer)
		''' <summary>
		''' Reads an array of octets (that is, bytes) from this input stream. </summary>
		''' <param name="value"> returned array of octets (that is, bytes). </param>
		''' <param name="offset"> offset on the stream. </param>
		''' <param name="length"> length of buffer to read. </param>
		Public MustOverride Sub read_octet_array(ByVal value As SByte(), ByVal offset As Integer, ByVal length As Integer)
		''' <summary>
		''' Reads an array of shorts from this input stream. </summary>
		''' <param name="value"> returned array of shorts. </param>
		''' <param name="offset"> offset on the stream. </param>
		''' <param name="length"> length of buffer to read. </param>
		Public MustOverride Sub read_short_array(ByVal value As Short(), ByVal offset As Integer, ByVal length As Integer)
		''' <summary>
		''' Reads an array of unsigned shorts from this input stream. </summary>
		''' <param name="value"> returned array of shorts. </param>
		''' <param name="offset"> offset on the stream. </param>
		''' <param name="length"> length of buffer to read. </param>
		Public MustOverride Sub read_ushort_array(ByVal value As Short(), ByVal offset As Integer, ByVal length As Integer)
		''' <summary>
		''' Reads an array of CORBA longs (that is, Java ints) from this input stream. </summary>
		''' <param name="value"> returned array of CORBA longs (that is, Java ints). </param>
		''' <param name="offset"> offset on the stream. </param>
		''' <param name="length"> length of buffer to read. </param>
		Public MustOverride Sub read_long_array(ByVal value As Integer(), ByVal offset As Integer, ByVal length As Integer)
		''' <summary>
		''' Reads an array of unsigned CORBA longs (that is, Java ints) from this input
		''' stream. </summary>
		''' <param name="value"> returned array of CORBA longs (that is, Java ints). </param>
		''' <param name="offset"> offset on the stream. </param>
		''' <param name="length"> length of buffer to read. </param>
		Public MustOverride Sub read_ulong_array(ByVal value As Integer(), ByVal offset As Integer, ByVal length As Integer)
		''' <summary>
		''' Reads an array of CORBA longlongs (that is, Java longs) from this input
		''' stream. </summary>
		''' <param name="value"> returned array of CORBA longs (that is, Java longs). </param>
		''' <param name="offset"> offset on the stream. </param>
		''' <param name="length"> length of buffer to read. </param>
		Public MustOverride Sub read_longlong_array(ByVal value As Long(), ByVal offset As Integer, ByVal length As Integer)
		''' <summary>
		''' Reads an array of unsigned CORBA longlongs (that is, Java longs) from this
		''' input stream. </summary>
		''' <param name="value"> returned array of CORBA longs (that is, Java longs). </param>
		''' <param name="offset"> offset on the stream. </param>
		''' <param name="length"> length of buffer to read. </param>
		Public MustOverride Sub read_ulonglong_array(ByVal value As Long(), ByVal offset As Integer, ByVal length As Integer)
		''' <summary>
		''' Reads an array of floats from this input stream. </summary>
		''' <param name="value"> returned array of floats. </param>
		''' <param name="offset"> offset on the stream. </param>
		''' <param name="length"> length of buffer to read. </param>
		Public MustOverride Sub read_float_array(ByVal value As Single(), ByVal offset As Integer, ByVal length As Integer)
		''' <summary>
		''' Reads an array of doubles from this input stream. </summary>
		''' <param name="value"> returned array of doubles. </param>
		''' <param name="offset"> offset on the stream. </param>
		''' <param name="length"> length of buffer to read. </param>
		Public MustOverride Sub read_double_array(ByVal value As Double(), ByVal offset As Integer, ByVal length As Integer)

		''' <summary>
		''' Reads a CORBA object from this input stream.
		''' </summary>
		''' <returns> the <code>Object</code> instance read from this input stream. </returns>
		Public MustOverride Function read_Object() As org.omg.CORBA.Object
		''' <summary>
		''' Reads a <code>TypeCode</code> from this input stream.
		''' </summary>
		''' <returns> the <code>TypeCode</code> instance read from this input stream. </returns>
		Public MustOverride Function read_TypeCode() As org.omg.CORBA.TypeCode
		''' <summary>
		''' Reads an Any from this input stream.
		''' </summary>
		''' <returns> the <code>Any</code> instance read from this input stream. </returns>
		Public MustOverride Function read_any() As org.omg.CORBA.Any

		''' <summary>
		''' Returns principal for invocation. </summary>
		''' <returns> Principal for invocation. </returns>
		''' @deprecated Deprecated by CORBA 2.2. 
		<Obsolete("Deprecated by CORBA 2.2.")> _
		Public Overridable Function read_Principal() As org.omg.CORBA.Principal
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function


		''' <seealso cref= <a href="package-summary.html#unimpl"><code>portable</code>
		''' package comments for unimplemented features</a> </seealso>
		Public Overridable Function read() As Integer
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function

		''' <summary>
		''' Reads a BigDecimal number. </summary>
		''' <returns> a java.math.BigDecimal number </returns>
		Public Overridable Function read_fixed() As Decimal
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function

		''' <summary>
		''' Reads a CORBA context from the stream. </summary>
		''' <returns> a CORBA context </returns>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>portable</code>
		''' package comments for unimplemented features</a> </seealso>
		Public Overridable Function read_Context() As org.omg.CORBA.Context
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function
	'    
	'     * The following methods were added by orbos/98-04-03: Java to IDL
	'     * Mapping. These are used by RMI over IIOP.
	'     

		''' <summary>
		''' Unmarshals an object and returns a CORBA Object,
		''' which is an instance of the class passed as its argument.
		''' This class is the stub class of the expected type.
		''' </summary>
		''' <param name="clz">  The Class object for the stub class which
		''' corresponds to the type that is statistically expected, or
		''' the Class object for the RMI/IDL interface type that
		''' is statistically expected. </param>
		''' <returns> an Object instance of clz read from this stream
		''' </returns>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>portable</code>
		''' package comments for unimplemented features</a> </seealso>
		Public Overridable Function read_Object(ByVal clz As Type) As org.omg.CORBA.Object
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function

		''' <summary>
		''' Returns the ORB that created this InputStream.
		''' </summary>
		''' <returns> the <code>ORB</code> object that created this stream
		''' </returns>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>portable</code>
		''' package comments for unimplemented features</a> </seealso>
		Public Overridable Function orb() As org.omg.CORBA.ORB
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function
	End Class

End Namespace