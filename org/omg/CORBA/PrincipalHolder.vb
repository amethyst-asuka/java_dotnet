Imports System

'
' * Copyright (c) 1996, 2004, Oracle and/or its affiliates. All rights reserved.
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
	''' The Holder for <tt>Principal</tt>.  For more information on
	''' Holder files, see <a href="doc-files/generatedfiles.html#holder">
	''' "Generated Files: Holder Files"</a>.<P>
	''' A container class for values of type <code>Principal</code>
	''' that is used to store "out" and "inout" parameters in IDL methods.
	''' If an IDL method signature has an IDL <code>Principal</code> as an "out"
	''' or "inout" parameter, the programmer must pass an instance of
	''' <code>PrincipalHolder</code> as the corresponding
	''' parameter in the method invocation; for "inout" parameters, the programmer
	''' must also fill the "in" value to be sent to the server.
	''' Before the method invocation returns, the ORB will fill in the
	''' value corresponding to the "out" value returned from the server.
	''' <P>
	''' If <code>myPrincipalHolder</code> is an instance of <code>PrincipalHolder</code>,
	''' the value stored in its <code>value</code> field can be accessed with
	''' <code>myPrincipalHolder.value</code>.
	''' 
	''' @since       JDK1.2 </summary>
	''' @deprecated Deprecated by CORBA 2.2. 
	<Obsolete("Deprecated by CORBA 2.2.")> _
	Public NotInheritable Class PrincipalHolder
		Implements org.omg.CORBA.portable.Streamable

		''' <summary>
		''' The <code>Principal</code> value held by this <code>PrincipalHolder</code>
		''' object.
		''' </summary>
		Public value As Principal

		''' <summary>
		''' Constructs a new <code>PrincipalHolder</code> object with its
		''' <code>value</code> field initialized to <code>null</code>.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Constructs a new <code>PrincipalHolder</code> object with its
		''' <code>value</code> field initialized to the given
		''' <code>Principal</code> object. </summary>
		''' <param name="initial"> the <code>Principal</code> with which to initialize
		'''                the <code>value</code> field of the newly-created
		'''                <code>PrincipalHolder</code> object </param>
		Public Sub New(ByVal initial As Principal)
			value = initial
		End Sub

		Public Sub _read(ByVal input As org.omg.CORBA.portable.InputStream)
			value = input.read_Principal()
		End Sub

		Public Sub _write(ByVal output As org.omg.CORBA.portable.OutputStream)
			output.write_Principal(value)
		End Sub

		Public Function _type() As org.omg.CORBA.TypeCode
			Return ORB.init().get_primitive_tc(TCKind.tk_Principal)
		End Function

	End Class

End Namespace