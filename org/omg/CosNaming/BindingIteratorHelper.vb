Imports System.Runtime.CompilerServices

Namespace org.omg.CosNaming


	''' <summary>
	''' org/omg/CosNaming/BindingIteratorHelper.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/CosNaming/nameservice.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>


	''' <summary>
	''' The BindingIterator interface allows a client to iterate through
	''' the bindings using the next_one or next_n operations.
	''' 
	''' The bindings iterator is obtained by using the <tt>list</tt>
	''' method on the <tt>NamingContext</tt>. </summary>
	''' <seealso cref= org.omg.CosNaming.NamingContext#list </seealso>
	Public MustInherit Class BindingIteratorHelper
	  Private Shared _id As String = "IDL:omg.org/CosNaming/BindingIterator:1.0"

	  Public Shared Sub insert(ByVal a As org.omg.CORBA.Any, ByVal that As org.omg.CosNaming.BindingIterator)
		Dim out As org.omg.CORBA.portable.OutputStream = a.create_output_stream()
		a.type(type())
		write(out, that)
		a.read_value(out.create_input_stream(), type())
	  End Sub

	  Public Shared Function extract(ByVal a As org.omg.CORBA.Any) As org.omg.CosNaming.BindingIterator
		Return read(a.create_input_stream())
	  End Function

	  Private Shared __typeCode As org.omg.CORBA.TypeCode = Nothing
	  <MethodImpl(MethodImplOptions.Synchronized)> _
	  Public Shared Function type() As org.omg.CORBA.TypeCode
		If __typeCode Is Nothing Then __typeCode = org.omg.CORBA.ORB.init().create_interface_tc(org.omg.CosNaming.BindingIteratorHelper.id(), "BindingIterator")
		Return __typeCode
	  End Function

	  Public Shared Function id() As String
		Return _id
	  End Function

	  Public Shared Function read(ByVal istream As org.omg.CORBA.portable.InputStream) As org.omg.CosNaming.BindingIterator
		Return narrow(istream.read_Object(GetType(_BindingIteratorStub)))
	  End Function

	  Public Shared Sub write(ByVal ostream As org.omg.CORBA.portable.OutputStream, ByVal value As org.omg.CosNaming.BindingIterator)
		ostream.write_Object(CType(value, org.omg.CORBA.Object))
	  End Sub

	  Public Shared Function narrow(ByVal obj As org.omg.CORBA.Object) As org.omg.CosNaming.BindingIterator
		If obj Is Nothing Then
		  Return Nothing
		ElseIf TypeOf obj Is org.omg.CosNaming.BindingIterator Then
		  Return CType(obj, org.omg.CosNaming.BindingIterator)
		ElseIf Not obj._is_a(id()) Then
		  Throw New org.omg.CORBA.BAD_PARAM
		Else
		  Dim [delegate] As org.omg.CORBA.portable.Delegate = CType(obj, org.omg.CORBA.portable.ObjectImpl)._get_delegate()
		  Dim stub As New org.omg.CosNaming._BindingIteratorStub
		  stub._set_delegate([delegate])
		  Return stub
		End If
	  End Function

	  Public Shared Function unchecked_narrow(ByVal obj As org.omg.CORBA.Object) As org.omg.CosNaming.BindingIterator
		If obj Is Nothing Then
		  Return Nothing
		ElseIf TypeOf obj Is org.omg.CosNaming.BindingIterator Then
		  Return CType(obj, org.omg.CosNaming.BindingIterator)
		Else
		  Dim [delegate] As org.omg.CORBA.portable.Delegate = CType(obj, org.omg.CORBA.portable.ObjectImpl)._get_delegate()
		  Dim stub As New org.omg.CosNaming._BindingIteratorStub
		  stub._set_delegate([delegate])
		  Return stub
		End If
	  End Function

	End Class

End Namespace