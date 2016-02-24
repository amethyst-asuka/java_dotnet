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
	''' The Holder for <tt>Char</tt>.  For more information on
	''' Holder files, see <a href="doc-files/generatedfiles.html#holder">
	''' "Generated Files: Holder Files"</a>.<P>
	''' A Holder class for a <code>char</code>
	''' that is used to store "out" and "inout" parameters in IDL methods.
	''' If an IDL method signature has an IDL <code>char</code> as an "out"
	''' or "inout" parameter, the programmer must pass an instance of
	''' <code>CharHolder</code> as the corresponding
	''' parameter in the method invocation; for "inout" parameters, the programmer
	''' must also fill the "in" value to be sent to the server.
	''' Before the method invocation returns, the ORB will fill in the
	''' value corresponding to the "out" value returned from the server.
	''' <P>
	''' If <code>myCharHolder</code> is an instance of <code>CharHolder</code>,
	''' the value stored in its <code>value</code> field can be accessed with
	''' <code>myCharHolder.value</code>.
	''' 
	''' @since       JDK1.2
	''' </summary>
	Public NotInheritable Class CharHolder
		Implements org.omg.CORBA.portable.Streamable

		''' <summary>
		''' The <code>char</code> value held by this <code>CharHolder</code>
		''' object.
		''' </summary>
		Public value As Char

		''' <summary>
		''' Constructs a new <code>CharHolder</code> object with its
		''' <code>value</code> field initialized to <code>0</code>.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Constructs a new <code>CharHolder</code> object for the given
		''' <code>char</code>. </summary>
		''' <param name="initial"> the <code>char</code> with which to initialize
		'''                the <code>value</code> field of the new
		'''                <code>CharHolder</code> object </param>
		Public Sub New(ByVal initial As Char)
			value = initial
		End Sub

		''' <summary>
		''' Reads from <code>input</code> and initalizes the value in
		''' this <code>CharHolder</code> object
		''' with the unmarshalled data.
		''' </summary>
		''' <param name="input"> the InputStream containing CDR formatted data from the wire </param>
		Public Sub _read(ByVal input As org.omg.CORBA.portable.InputStream)
			value = input.read_char()
		End Sub

		''' <summary>
		''' Marshals to <code>output</code> the value in
		''' this <code>CharHolder</code> object.
		''' </summary>
		''' <param name="output"> the OutputStream which will contain the CDR formatted data </param>
		Public Sub _write(ByVal output As org.omg.CORBA.portable.OutputStream)
			output.write_char(value)
		End Sub

		''' <summary>
		''' Returns the <code>TypeCode</code> object  corresponding
		''' to the value held in
		''' this <code>CharHolder</code> object.
		''' </summary>
		''' <returns>    the TypeCode of the value held in
		'''            this <code>CharHolder</code> object </returns>
		Public Function _type() As org.omg.CORBA.TypeCode
			Return ORB.init().get_primitive_tc(TCKind.tk_char)
		End Function
	End Class

End Namespace