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
	''' The Holder for <tt>Byte</tt>.  For more information on
	''' Holder files, see <a href="doc-files/generatedfiles.html#holder">
	''' "Generated Files: Holder Files"</a>.<P>
	''' A Holder class for a <code>byte</code>
	''' that is used to store "out" and "inout" parameters in IDL methods.
	''' If an IDL method signature has an IDL <code>octet</code> as an "out"
	''' or "inout" parameter, the programmer must pass an instance of
	''' <code>ByteHolder</code> as the corresponding
	''' parameter in the method invocation; for "inout" parameters, the programmer
	''' must also fill the "in" value to be sent to the server.
	''' Before the method invocation returns, the ORB will fill in the
	''' value corresponding to the "out" value returned from the server.
	''' <P>
	''' If <code>myByteHolder</code> is an instance of <code>ByteHolder</code>,
	''' the value stored in its <code>value</code> field can be accessed with
	''' <code>myByteHolder.value</code>.
	''' 
	''' @since       JDK1.2
	''' </summary>
	Public NotInheritable Class ByteHolder
		Implements org.omg.CORBA.portable.Streamable

		''' <summary>
		''' The <code>byte</code> value held by this <code>ByteHolder</code>
		''' object.
		''' </summary>

		Public value As SByte

		''' <summary>
		''' Constructs a new <code>ByteHolder</code> object with its
		''' <code>value</code> field initialized to 0.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Constructs a new <code>ByteHolder</code> object for the given
		''' <code>byte</code>. </summary>
		''' <param name="initial"> the <code>byte</code> with which to initialize
		'''                the <code>value</code> field of the new
		'''                <code>ByteHolder</code> object </param>
		Public Sub New(ByVal initial As SByte)
			value = initial
		End Sub

		''' <summary>
		''' Reads from <code>input</code> and initalizes the value in
		''' this <code>ByteHolder</code> object
		''' with the unmarshalled data.
		''' </summary>
		''' <param name="input"> the InputStream containing CDR formatted data from the wire. </param>
		Public Sub _read(ByVal input As org.omg.CORBA.portable.InputStream)
			value = input.read_octet()
		End Sub

		''' <summary>
		''' Marshals to <code>output</code> the value in
		''' this <code>ByteHolder</code> object.
		''' </summary>
		''' <param name="output"> the OutputStream which will contain the CDR formatted data. </param>
		Public Sub _write(ByVal output As org.omg.CORBA.portable.OutputStream)
			output.write_octet(value)
		End Sub

		''' <summary>
		''' Returns the TypeCode corresponding to the value held in
		''' this <code>ByteHolder</code> object.
		''' </summary>
		''' <returns>    the TypeCode of the value held in
		'''               this <code>ByteHolder</code> object </returns>
		Public Function _type() As org.omg.CORBA.TypeCode
			Return ORB.init().get_primitive_tc(TCKind.tk_octet)
		End Function
	End Class

End Namespace