'
' * Copyright (c) 1997, 2001, Oracle and/or its affiliates. All rights reserved.
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
	''' The Holder for <tt>Int</tt>.  For more information on
	''' Holder files, see <a href="doc-files/generatedfiles.html#holder">
	''' "Generated Files: Holder Files"</a>.<P>
	''' A Holder class for an <code>int</code>
	''' that is used to store "out" and "inout" parameters in IDL methods.
	''' If an IDL method signature has an IDL <code>long</code> as an "out"
	''' or "inout" parameter, the programmer must pass an instance of
	''' <code>IntHolder</code> as the corresponding
	''' parameter in the method invocation; for "inout" parameters, the programmer
	''' must also fill the "in" value to be sent to the server.
	''' Before the method invocation returns, the ORB will fill in the
	''' value corresponding to the "out" value returned from the server.
	''' <P>
	''' If <code>myIntHolder</code> is an instance of <code>IntHolder</code>,
	''' the value stored in its <code>value</code> field can be accessed with
	''' <code>myIntHolder.value</code>.
	''' 
	''' @since       JDK1.2
	''' </summary>
	Public NotInheritable Class IntHolder
		Implements org.omg.CORBA.portable.Streamable

		''' <summary>
		''' The <code>int</code> value held by this <code>IntHolder</code>
		''' object in its <code>value</code> field.
		''' </summary>
		Public value As Integer

		''' <summary>
		''' Constructs a new <code>IntHolder</code> object with its
		''' <code>value</code> field initialized to <code>0</code>.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Constructs a new <code>IntHolder</code> object with its
		''' <code>value</code> field initialized to the given
		''' <code>int</code>. </summary>
		''' <param name="initial"> the <code>int</code> with which to initialize
		'''                the <code>value</code> field of the newly-created
		'''                <code>IntHolder</code> object </param>
		Public Sub New(ByVal initial As Integer)
			value = initial
		End Sub

		''' <summary>
		''' Reads unmarshalled data from <code>input</code> and assigns it to
		''' the <code>value</code> field in this <code>IntHolder</code> object.
		''' </summary>
		''' <param name="input"> the <code>InputStream</code> object containing CDR
		'''              formatted data from the wire </param>
		Public Sub _read(ByVal input As org.omg.CORBA.portable.InputStream)
			value = input.read_long()
		End Sub

		''' <summary>
		''' Marshals the value in this <code>IntHolder</code> object's
		''' <code>value</code> field to the output stream <code>output</code>.
		''' </summary>
		''' <param name="output"> the <code>OutputStream</code> object that will contain
		'''               the CDR formatted data </param>
		Public Sub _write(ByVal output As org.omg.CORBA.portable.OutputStream)
			output.write_long(value)
		End Sub

		''' <summary>
		''' Retrieves the <code>TypeCode</code> object that corresponds
		''' to the value held in this <code>IntHolder</code> object's
		''' <code>value</code> field.
		''' </summary>
		''' <returns>    the type code for the value held in this <code>IntHolder</code>
		'''            object </returns>
		Public Function _type() As org.omg.CORBA.TypeCode
			Return ORB.init().get_primitive_tc(TCKind.tk_long)
		End Function
	End Class

End Namespace