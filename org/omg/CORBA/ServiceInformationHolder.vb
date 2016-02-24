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
	''' The Holder for <tt>ServiceInformation</tt>.  For more information on
	''' Holder files, see <a href="doc-files/generatedfiles.html#holder">
	''' "Generated Files: Holder Files"</a>.<P>
	''' A Holder class for a <code>ServiceInformation</code> object
	''' that is used to store "out" and "inout" parameters in IDL methods.
	''' If an IDL method signature has an IDL <code>xxx</code> as an "out"
	''' or "inout" parameter, the programmer must pass an instance of
	''' <code>ServiceInformationHolder</code> as the corresponding
	''' parameter in the method invocation; for "inout" parameters, the programmer
	''' must also fill the "in" value to be sent to the server.
	''' Before the method invocation returns, the ORB will fill in the
	''' value corresponding to the "out" value returned from the server.
	''' <P>
	''' If <code>myServiceInformationHolder</code> is an instance of <code>ServiceInformationHolder</code>,
	''' the value stored in its <code>value</code> field can be accessed with
	''' <code>myServiceInformationHolder.value</code>.
	''' </summary>
	Public NotInheritable Class ServiceInformationHolder
		Implements org.omg.CORBA.portable.Streamable

		''' <summary>
		''' The <code>ServiceInformation</code> value held by this
		''' <code>ServiceInformationHolder</code> object in its <code>value</code> field.
		''' </summary>
		Public value As ServiceInformation

		''' <summary>
		''' Constructs a new <code>ServiceInformationHolder</code> object with its
		''' <code>value</code> field initialized to null.
		''' </summary>
		Public Sub New()
			Me.New(Nothing)
		End Sub

		''' <summary>
		''' Constructs a new <code>ServiceInformationHolder</code> object with its
		''' <code>value</code> field initialized to the given
		''' <code>ServiceInformation</code> object.
		''' </summary>
		''' <param name="arg"> the <code>ServiceInformation</code> object with which to initialize
		'''                the <code>value</code> field of the newly-created
		'''                <code>ServiceInformationHolder</code> object </param>
		Public Sub New(ByVal arg As org.omg.CORBA.ServiceInformation)
			value = arg
		End Sub


		''' <summary>
		''' Marshals the value in this <code>ServiceInformationHolder</code> object's
		''' <code>value</code> field to the output stream <code>out</code>.
		''' </summary>
		''' <param name="out"> the <code>OutputStream</code> object that will contain
		'''               the CDR formatted data </param>
		Public Sub _write(ByVal out As org.omg.CORBA.portable.OutputStream)
			org.omg.CORBA.ServiceInformationHelper.write(out, value)
		End Sub

		''' <summary>
		''' Reads unmarshalled data from the input stream <code>in</code> and assigns it to
		''' the <code>value</code> field in this <code>ServiceInformationHolder</code> object.
		''' </summary>
		''' <param name="in"> the <code>InputStream</code> object containing CDR
		'''              formatted data from the wire </param>
		Public Sub _read(ByVal [in] As org.omg.CORBA.portable.InputStream)
			value = org.omg.CORBA.ServiceInformationHelper.read([in])
		End Sub

		''' <summary>
		''' Retrieves the <code>TypeCode</code> object that corresponds
		''' to the value held in this <code>ServiceInformationHolder</code> object's
		''' <code>value</code> field.
		''' </summary>
		''' <returns>    the type code for the value held in this <code>ServiceInformationHolder</code>
		'''            object </returns>
		Public Function _type() As org.omg.CORBA.TypeCode
			Return org.omg.CORBA.ServiceInformationHelper.type()
		End Function
	End Class

End Namespace