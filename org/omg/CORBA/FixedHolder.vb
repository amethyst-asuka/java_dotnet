Imports System

'
' * Copyright (c) 1998, 2001, Oracle and/or its affiliates. All rights reserved.
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
	''' The Holder for <tt>Fixed</tt>.  For more information on
	''' Holder files, see <a href="doc-files/generatedfiles.html#holder">
	''' "Generated Files: Holder Files"</a>.<P>
	''' FixedHolder is a container class for values of IDL type "fixed",
	''' which is mapped to the Java class java.math.BigDecimal.
	''' It is usually used to store "out" and "inout" IDL method parameters.
	''' If an IDL method signature has a fixed as an "out" or "inout" parameter,
	''' the programmer must pass an instance of FixedHolder as the corresponding
	''' parameter in the method invocation; for "inout" parameters, the programmer
	''' must also fill the "in" value to be sent to the server.
	''' Before the method invocation returns, the ORB will fill in the contained
	''' value corresponding to the "out" value returned from the server.
	''' 
	''' </summary>
	Public NotInheritable Class FixedHolder
		Implements org.omg.CORBA.portable.Streamable

		''' <summary>
		''' The value held by the FixedHolder
		''' </summary>
		Public value As Decimal

		''' <summary>
		''' Construct the FixedHolder without initializing the contained value.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Construct the FixedHolder and initialize it with the given value. </summary>
		''' <param name="initial"> the value used to initialize the FixedHolder </param>
		Public Sub New(ByVal initial As Decimal)
			value = initial
		End Sub

		''' <summary>
		''' Read a fixed point value from the input stream and store it in
		''' the value member.
		''' </summary>
		''' <param name="input"> the <code>InputStream</code> to read from. </param>
		Public Sub _read(ByVal input As org.omg.CORBA.portable.InputStream)
			value = input.read_fixed()
		End Sub

		''' <summary>
		''' Write the fixed point value stored in this holder to an
		''' <code>OutputStream</code>.
		''' </summary>
		''' <param name="output"> the <code>OutputStream</code> to write into. </param>
		Public Sub _write(ByVal output As org.omg.CORBA.portable.OutputStream)
			output.write_fixed(value)
		End Sub


		''' <summary>
		''' Return the <code>TypeCode</code> of this holder object.
		''' </summary>
		''' <returns> the <code>TypeCode</code> object. </returns>
		Public Function _type() As org.omg.CORBA.TypeCode
			Return ORB.init().get_primitive_tc(TCKind.tk_fixed)
		End Function

	End Class

End Namespace