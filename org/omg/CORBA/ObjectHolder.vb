'
' * Copyright (c) 1996, 2001, Oracle and/or its affiliates. All rights reserved.
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
	''' The Holder for <tt>Object</tt>.  For more information on
	''' Holder files, see <a href="doc-files/generatedfiles.html#holder">
	''' "Generated Files: Holder Files"</a>.<P>
	''' A Holder class for a CORBA object reference (a value of type
	''' <code>org.omg.CORBA.Object</code>).  It is usually
	''' used to store "out" and "inout" parameters in IDL methods.
	''' If an IDL method signature has a CORBA Object reference as an "out"
	''' or "inout" parameter, the programmer must pass an instance of
	''' <code>ObjectHolder</code> as the corresponding
	''' parameter in the method invocation; for "inout" parameters, the programmer
	''' must also fill the "in" value to be sent to the server.
	''' Before the method invocation returns, the ORB will fill in the
	''' value corresponding to the "out" value returned from the server.
	''' <P>
	''' If <code>myObjectHolder</code> is an instance of <code>ObjectHolder</code>,
	''' the value stored in its <code>value</code> field can be accessed with
	''' <code>myObjectHolder.value</code>.
	''' 
	''' @since       JDK1.2
	''' </summary>
	Public NotInheritable Class ObjectHolder
		Implements org.omg.CORBA.portable.Streamable

		''' <summary>
		''' The <code>Object</code> value held by this <code>ObjectHolder</code>
		''' object.
		''' </summary>
		Public value As Object

		''' <summary>
		''' Constructs a new <code>ObjectHolder</code> object with its
		''' <code>value</code> field initialized to <code>null</code>.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Constructs a new <code>ObjectHolder</code> object with its
		''' <code>value</code> field initialized to the given
		''' <code>Object</code>. </summary>
		''' <param name="initial"> the <code>Object</code> with which to initialize
		'''                the <code>value</code> field of the newly-created
		'''                <code>ObjectHolder</code> object </param>
		Public Sub New(ByVal initial As Object)
			value = initial
		End Sub

		''' <summary>
		''' Reads from <code>input</code> and initalizes the value in
		''' this <code>ObjectHolder</code> object
		''' with the unmarshalled data.
		''' </summary>
		''' <param name="input"> the InputStream containing CDR formatted data from the wire. </param>
		Public Sub _read(ByVal input As org.omg.CORBA.portable.InputStream)
			value = input.read_Object()
		End Sub

		''' <summary>
		''' Marshals to <code>output</code> the value in
		''' this <code>ObjectHolder</code> object.
		''' </summary>
		''' <param name="output"> the OutputStream which will contain the CDR formatted data. </param>
		Public Sub _write(ByVal output As org.omg.CORBA.portable.OutputStream)
			output.write_Object(value)
		End Sub

		''' <summary>
		''' Returns the TypeCode corresponding to the value held in
		''' this <code>ObjectHolder</code> object
		''' </summary>
		''' <returns>    the TypeCode of the value held in
		'''            this <code>ObjectHolder</code> object </returns>
		Public Function _type() As org.omg.CORBA.TypeCode
			Return org.omg.CORBA.ORB.init().get_primitive_tc(TCKind.tk_objref)
		End Function
	End Class

End Namespace