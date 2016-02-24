'
' * Copyright (c) 1995, 2001, Oracle and/or its affiliates. All rights reserved.
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
	''' The Holder for <tt>Float</tt>.  For more information on
	''' Holder files, see <a href="doc-files/generatedfiles.html#holder">
	''' "Generated Files: Holder Files"</a>.<P>
	''' A Holder class for a <code>float</code>
	''' that is used to store "out" and "inout" parameters in IDL methods.
	''' If an IDL method signature has an IDL <code>float</code> as an "out"
	''' or "inout" parameter, the programmer must pass an instance of
	''' <code>FloatHolder</code> as the corresponding
	''' parameter in the method invocation; for "inout" parameters, the programmer
	''' must also fill the "in" value to be sent to the server.
	''' Before the method invocation returns, the ORB will fill in the
	''' value corresponding to the "out" value returned from the server.
	''' <P>
	''' If <code>myFloatHolder</code> is an instance of <code>FloatHolder</code>,
	''' the value stored in its <code>value</code> field can be accessed with
	''' <code>myFloatHolder.value</code>.
	''' 
	''' @since       JDK1.2
	''' </summary>
	Public NotInheritable Class FloatHolder
		Implements org.omg.CORBA.portable.Streamable

		''' <summary>
		''' The <code>float</code> value held by this <code>FloatHolder</code>
		''' object.
		''' </summary>
		Public value As Single

		''' <summary>
		''' Constructs a new <code>FloatHolder</code> object with its
		''' <code>value</code> field initialized to 0.0.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Constructs a new <code>FloatHolder</code> object for the given
		''' <code>float</code>. </summary>
		''' <param name="initial"> the <code>float</code> with which to initialize
		'''                the <code>value</code> field of the new
		'''                <code>FloatHolder</code> object </param>
		Public Sub New(ByVal initial As Single)
			value = initial
		End Sub

		''' <summary>
		''' Read a float from an input stream and initialize the value
		''' member with the float value.
		''' </summary>
		''' <param name="input"> the <code>InputStream</code> to read from. </param>
		Public Sub _read(ByVal input As org.omg.CORBA.portable.InputStream)
			value = input.read_float()
		End Sub

		''' <summary>
		''' Write the float value into an output stream.
		''' </summary>
		''' <param name="output"> the <code>OutputStream</code> to write into. </param>
		Public Sub _write(ByVal output As org.omg.CORBA.portable.OutputStream)
			output.write_float(value)
		End Sub

		''' <summary>
		''' Return the <code>TypeCode</code> of this Streamable.
		''' </summary>
		''' <returns> the <code>TypeCode</code> object. </returns>
		Public Function _type() As org.omg.CORBA.TypeCode
			Return ORB.init().get_primitive_tc(TCKind.tk_float)
		End Function
	End Class

End Namespace