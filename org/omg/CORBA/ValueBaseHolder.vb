'
' * Copyright (c) 1999, 2001, Oracle and/or its affiliates. All rights reserved.
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
	''' The Holder for <tt>ValueBase</tt>.  For more information on
	''' Holder files, see <a href="doc-files/generatedfiles.html#holder">
	''' "Generated Files: Holder Files"</a>.<P>
	''' A Holder class for a <code>java.io.Serializable</code>
	''' that is used to store "out" and "inout" parameters in IDL methods.
	''' If an IDL method signature has an IDL <code>ValueBase</code> as an "out"
	''' or "inout" parameter, the programmer must pass an instance of
	''' <code>ValueBaseHolder</code> as the corresponding
	''' parameter in the method invocation; for "inout" parameters, the programmer
	''' must also fill the "in" value to be sent to the server.
	''' Before the method invocation returns, the ORB will fill in the
	''' value corresponding to the "out" value returned from the server.
	''' <P>
	''' If <code>myValueBaseHolder</code> is an instance of <code>ValueBaseHolder</code>,
	''' the value stored in its <code>value</code> field can be accessed with
	''' <code>myValueBaseHolder.value</code>.
	''' 
	''' </summary>
	Public NotInheritable Class ValueBaseHolder
		Implements org.omg.CORBA.portable.Streamable

		''' <summary>
		''' The <code>java.io.Serializable</code> value held by this
		''' <code>ValueBaseHolder</code> object.
		''' </summary>
		Public value As java.io.Serializable

		''' <summary>
		''' Constructs a new <code>ValueBaseHolder</code> object with its
		''' <code>value</code> field initialized to <code>0</code>.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Constructs a new <code>ValueBaseHolder</code> object with its
		''' <code>value</code> field initialized to the given
		''' <code>java.io.Serializable</code>. </summary>
		''' <param name="initial"> the <code>java.io.Serializable</code> with which to initialize
		'''                the <code>value</code> field of the newly-created
		'''                <code>ValueBaseHolder</code> object </param>
		Public Sub New(ByVal initial As java.io.Serializable)
			value = initial
		End Sub

		''' <summary>
		''' Reads from <code>input</code> and initalizes the value in the Holder
		''' with the unmarshalled data.
		''' </summary>
		''' <param name="input"> the InputStream containing CDR formatted data from the wire </param>
		Public Sub _read(ByVal input As org.omg.CORBA.portable.InputStream)
			value = CType(input, org.omg.CORBA_2_3.portable.InputStream).read_value()
		End Sub

		''' <summary>
		''' Marshals to <code>output</code> the value in the Holder.
		''' </summary>
		''' <param name="output"> the OutputStream which will contain the CDR formatted data </param>
		Public Sub _write(ByVal output As org.omg.CORBA.portable.OutputStream)
			CType(output, org.omg.CORBA_2_3.portable.OutputStream).write_value(value)
		End Sub

		''' <summary>
		''' Returns the <code>TypeCode</code> object
		''' corresponding to the value held in the Holder.
		''' </summary>
		''' <returns>    the TypeCode of the value held in the holder </returns>
		Public Function _type() As org.omg.CORBA.TypeCode
			Return ORB.init().get_primitive_tc(TCKind.tk_value)
		End Function

	End Class

End Namespace