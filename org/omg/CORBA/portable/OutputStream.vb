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
	''' OuputStream is the Java API for writing IDL types
	''' to CDR marshal streams. These methods are used by the ORB to
	''' marshal IDL types as well as to insert IDL types into Anys.
	''' The <code>_array</code> versions of the methods can be directly
	''' used to write sequences and arrays of IDL types.
	''' 
	''' @since   JDK1.2
	''' </summary>


	Public MustInherit Class OutputStream
		Inherits java.io.OutputStream

		''' <summary>
		''' Returns an input stream with the same buffer. </summary>
		''' <returns> an input stream with the same buffer. </returns>
		Public MustOverride Function create_input_stream() As InputStream

		''' <summary>
		''' Writes a boolean value to this stream. </summary>
		''' <param name="value"> the value to be written. </param>
		Public MustOverride Sub write_boolean(ByVal value As Boolean)
		''' <summary>
		''' Writes a char value to this stream. </summary>
		''' <param name="value"> the value to be written. </param>
		Public MustOverride Sub write_char(ByVal value As Char)
		''' <summary>
		''' Writes a wide char value to this stream. </summary>
		''' <param name="value"> the value to be written. </param>
		Public MustOverride Sub write_wchar(ByVal value As Char)
		''' <summary>
		''' Writes a CORBA octet (i.e. byte) value to this stream. </summary>
		''' <param name="value"> the value to be written. </param>
		Public MustOverride Sub write_octet(ByVal value As SByte)
		''' <summary>
		''' Writes a short value to this stream. </summary>
		''' <param name="value"> the value to be written. </param>
		Public MustOverride Sub write_short(ByVal value As Short)
		''' <summary>
		''' Writes an unsigned short value to this stream. </summary>
		''' <param name="value"> the value to be written. </param>
		Public MustOverride Sub write_ushort(ByVal value As Short)
		''' <summary>
		''' Writes a CORBA long (i.e. Java int) value to this stream. </summary>
		''' <param name="value"> the value to be written. </param>
		Public MustOverride Sub write_long(ByVal value As Integer)
		''' <summary>
		''' Writes an unsigned CORBA long (i.e. Java int) value to this stream. </summary>
		''' <param name="value"> the value to be written. </param>
		Public MustOverride Sub write_ulong(ByVal value As Integer)
		''' <summary>
		''' Writes a CORBA longlong (i.e. Java long) value to this stream. </summary>
		''' <param name="value"> the value to be written. </param>
		Public MustOverride Sub write_longlong(ByVal value As Long)
		''' <summary>
		''' Writes an unsigned CORBA longlong (i.e. Java long) value to this stream. </summary>
		''' <param name="value"> the value to be written. </param>
		Public MustOverride Sub write_ulonglong(ByVal value As Long)
		''' <summary>
		''' Writes a float value to this stream. </summary>
		''' <param name="value"> the value to be written. </param>
		Public MustOverride Sub write_float(ByVal value As Single)
		''' <summary>
		''' Writes a double value to this stream. </summary>
		''' <param name="value"> the value to be written. </param>
		Public MustOverride Sub write_double(ByVal value As Double)
		''' <summary>
		''' Writes a string value to this stream. </summary>
		''' <param name="value"> the value to be written. </param>
		Public MustOverride Sub write_string(ByVal value As String)
		''' <summary>
		''' Writes a wide string value to this stream. </summary>
		''' <param name="value"> the value to be written. </param>
		Public MustOverride Sub write_wstring(ByVal value As String)

		''' <summary>
		''' Writes an array of booleans on this output stream. </summary>
		''' <param name="value"> the array to be written. </param>
		''' <param name="offset"> offset on the stream. </param>
		''' <param name="length"> length of buffer to write. </param>
		Public MustOverride Sub write_boolean_array(ByVal value As Boolean(), ByVal offset As Integer, ByVal length As Integer)
		''' <summary>
		''' Writes an array of chars on this output stream. </summary>
		''' <param name="value"> the array to be written. </param>
		''' <param name="offset"> offset on the stream. </param>
		''' <param name="length"> length of buffer to write. </param>
		Public MustOverride Sub write_char_array(ByVal value As Char(), ByVal offset As Integer, ByVal length As Integer)
		''' <summary>
		''' Writes an array of wide chars on this output stream. </summary>
		''' <param name="value"> the array to be written. </param>
		''' <param name="offset"> offset on the stream. </param>
		''' <param name="length"> length of buffer to write. </param>
		Public MustOverride Sub write_wchar_array(ByVal value As Char(), ByVal offset As Integer, ByVal length As Integer)
		''' <summary>
		''' Writes an array of CORBA octets (bytes) on this output stream. </summary>
		''' <param name="value"> the array to be written. </param>
		''' <param name="offset"> offset on the stream. </param>
		''' <param name="length"> length of buffer to write. </param>
		Public MustOverride Sub write_octet_array(ByVal value As SByte(), ByVal offset As Integer, ByVal length As Integer)
		''' <summary>
		''' Writes an array of shorts on this output stream. </summary>
		''' <param name="value"> the array to be written. </param>
		''' <param name="offset"> offset on the stream. </param>
		''' <param name="length"> length of buffer to write. </param>
		Public MustOverride Sub write_short_array(ByVal value As Short(), ByVal offset As Integer, ByVal length As Integer)
		''' <summary>
		''' Writes an array of unsigned shorts on this output stream. </summary>
		''' <param name="value"> the array to be written. </param>
		''' <param name="offset"> offset on the stream. </param>
		''' <param name="length"> length of buffer to write. </param>
		Public MustOverride Sub write_ushort_array(ByVal value As Short(), ByVal offset As Integer, ByVal length As Integer)
		''' <summary>
		''' Writes an array of CORBA longs (i.e. Java ints) on this output stream. </summary>
		''' <param name="value"> the array to be written. </param>
		''' <param name="offset"> offset on the stream. </param>
		''' <param name="length"> length of buffer to write. </param>
		Public MustOverride Sub write_long_array(ByVal value As Integer(), ByVal offset As Integer, ByVal length As Integer)
		''' <summary>
		''' Writes an array of unsigned CORBA longs (i.e. Java ints) on this output stream. </summary>
		''' <param name="value"> the array to be written. </param>
		''' <param name="offset"> offset on the stream. </param>
		''' <param name="length"> length of buffer to write. </param>
		Public MustOverride Sub write_ulong_array(ByVal value As Integer(), ByVal offset As Integer, ByVal length As Integer)
		''' <summary>
		''' Writes an array of CORBA longlongs (i.e. Java longs) on this output stream. </summary>
		''' <param name="value"> the array to be written. </param>
		''' <param name="offset"> offset on the stream. </param>
		''' <param name="length"> length of buffer to write. </param>
		Public MustOverride Sub write_longlong_array(ByVal value As Long(), ByVal offset As Integer, ByVal length As Integer)
		''' <summary>
		''' Writes an array of unsigned CORBA longlongs (i.e. Java ints) on this output stream. </summary>
		''' <param name="value"> the array to be written. </param>
		''' <param name="offset"> offset on the stream. </param>
		''' <param name="length"> length of buffer to write. </param>
		Public MustOverride Sub write_ulonglong_array(ByVal value As Long(), ByVal offset As Integer, ByVal length As Integer)
		''' <summary>
		''' Writes an array of floats on this output stream. </summary>
		''' <param name="value"> the array to be written. </param>
		''' <param name="offset"> offset on the stream. </param>
		''' <param name="length"> length of buffer to write. </param>
		Public MustOverride Sub write_float_array(ByVal value As Single(), ByVal offset As Integer, ByVal length As Integer)
		''' <summary>
		''' Writes an array of doubles on this output stream. </summary>
		''' <param name="value"> the array to be written. </param>
		''' <param name="offset"> offset on the stream. </param>
		''' <param name="length"> length of buffer to write. </param>
		Public MustOverride Sub write_double_array(ByVal value As Double(), ByVal offset As Integer, ByVal length As Integer)
		''' <summary>
		''' Writes a CORBA Object on this output stream. </summary>
		''' <param name="value"> the value to be written. </param>
		Public MustOverride Sub write_Object(ByVal value As org.omg.CORBA.Object)
		''' <summary>
		''' Writes a TypeCode on this output stream. </summary>
		''' <param name="value"> the value to be written. </param>
		Public MustOverride Sub write_TypeCode(ByVal value As org.omg.CORBA.TypeCode)
		''' <summary>
		''' Writes an Any on this output stream. </summary>
		''' <param name="value"> the value to be written. </param>
		Public MustOverride Sub write_any(ByVal value As org.omg.CORBA.Any)

		''' <summary>
		''' Writes a Principle on this output stream. </summary>
		''' <param name="value"> the value to be written. </param>
		''' @deprecated Deprecated by CORBA 2.2. 
		<Obsolete("Deprecated by CORBA 2.2.")> _
		Public Overridable Sub write_Principal(ByVal value As org.omg.CORBA.Principal)
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Sub

		''' <summary>
		''' Writes an integer (length of arrays) onto this stream. </summary>
		''' <param name="b"> the value to be written. </param>
		''' <exception cref="java.io.IOException"> if there is an input/output error </exception>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>portable</code>
		''' package comments for unimplemented features</a> </seealso>
		Public Overridable Sub write(ByVal b As Integer)
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Sub

		''' <summary>
		''' Writes a BigDecimal number. </summary>
		''' <param name="value"> a BidDecimal--value to be written. </param>
		Public Overridable Sub write_fixed(ByVal value As Decimal)
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Sub

		''' <summary>
		''' Writes a CORBA context on this stream. The
		''' Context is marshaled as a sequence of strings.
		''' Only those Context values specified in the contexts
		''' parameter are actually written. </summary>
		''' <param name="ctx"> a CORBA context </param>
		''' <param name="contexts"> a <code>ContextList</code> object containing the list of contexts
		'''        to be written </param>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>portable</code>
		''' package comments for unimplemented features</a> </seealso>
		Public Overridable Sub write_Context(ByVal ctx As org.omg.CORBA.Context, ByVal contexts As org.omg.CORBA.ContextList)
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Sub

		''' <summary>
		''' Returns the ORB that created this OutputStream. </summary>
		''' <returns> the ORB that created this OutputStream </returns>
		''' <seealso cref= <a href="package-summary.html#unimpl"><code>portable</code>
		''' package comments for unimplemented features</a> </seealso>
		Public Overridable Function orb() As org.omg.CORBA.ORB
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function
	End Class

End Namespace