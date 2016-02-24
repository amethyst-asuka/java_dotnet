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
	''' The Holder for <tt>Boolean</tt>.  For more information on
	''' Holder files, see <a href="doc-files/generatedfiles.html#holder">
	''' "Generated Files: Holder Files"</a>.<P>
	''' A Holder class for a <code>boolean</code>
	''' that is used to store "out" and "inout" parameters in IDL methods.
	''' If an IDL method signature has an IDL <code>boolean</code> as an "out"
	''' or "inout" parameter, the programmer must pass an instance of
	''' <code>BooleanHolder</code> as the corresponding
	''' parameter in the method invocation; for "inout" parameters, the programmer
	''' must also fill the "in" value to be sent to the server.
	''' Before the method invocation returns, the ORB will fill in the
	''' value corresponding to the "out" value returned from the server.
	''' <P>
	''' If <code>myBooleanHolder</code> is an instance of <code>BooleanHolder</code>,
	''' the value stored in its <code>value</code> field can be accessed with
	''' <code>myBooleanHolder.value</code>.
	''' 
	''' @since       JDK1.2
	''' </summary>
	Public NotInheritable Class BooleanHolder
		Implements org.omg.CORBA.portable.Streamable

		''' <summary>
		''' The <code>boolean</code> value held by this <code>BooleanHolder</code>
		''' object.
		''' </summary>
		Public value As Boolean

		''' <summary>
		''' Constructs a new <code>BooleanHolder</code> object with its
		''' <code>value</code> field initialized to <code>false</code>.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Constructs a new <code>BooleanHolder</code> object with its
		''' <code>value</code> field initialized with the given <code>boolean</code>. </summary>
		''' <param name="initial"> the <code>boolean</code> with which to initialize
		'''                the <code>value</code> field of the newly-created
		'''                <code>BooleanHolder</code> object </param>
		Public Sub New(ByVal initial As Boolean)
			value = initial
		End Sub

		''' <summary>
		''' Reads unmarshalled data from <code>input</code> and assigns it to this
		''' <code>BooleanHolder</code> object's <code>value</code> field.
		''' </summary>
		''' <param name="input"> the <code>InputStream</code> object containing
		'''              CDR formatted data from the wire </param>
		Public Sub _read(ByVal input As org.omg.CORBA.portable.InputStream)
			value = input.read_boolean()
		End Sub

		''' <summary>
		''' Marshals the value in this <code>BooleanHolder</code> object's
		''' <code>value</code> field to the output stream <code>output</code>.
		''' </summary>
		''' <param name="output"> the OutputStream which will contain the CDR formatted data </param>
		Public Sub _write(ByVal output As org.omg.CORBA.portable.OutputStream)
			output.write_boolean(value)
		End Sub

		''' <summary>
		''' Retrieves the <code>TypeCode</code> object that corresponds to the
		''' value held in this <code>BooleanHolder</code> object.
		''' </summary>
		''' <returns>    the <code>TypeCode</code> for the value held
		'''            in this <code>BooleanHolder</code> object </returns>
		Public Function _type() As TypeCode
			Return ORB.init().get_primitive_tc(TCKind.tk_boolean)
		End Function
	End Class

End Namespace