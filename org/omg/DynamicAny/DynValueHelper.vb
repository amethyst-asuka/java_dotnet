Imports System.Runtime.CompilerServices

Namespace org.omg.DynamicAny


	''' <summary>
	''' org/omg/DynamicAny/DynValueHelper.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/DynamicAny/DynamicAny.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>


	''' <summary>
	''' DynValue objects support the manipulation of IDL non-boxed value types.
	''' The DynValue interface can represent both null and non-null value types.
	''' For a DynValue representing a non-null value type, the DynValue's components comprise
	''' the public and private members of the value type, including those inherited from concrete base value types,
	''' in the order of definition. A DynValue representing a null value type has no components
	''' and a current position of -1.
	''' <P>Warning: Indiscriminantly changing the contents of private value type members can cause the value type
	''' implementation to break by violating internal constraints. Access to private members is provided to support
	''' such activities as ORB bridging and debugging and should not be used to arbitrarily violate
	''' the encapsulation of the value type. 
	''' </summary>
	Public MustInherit Class DynValueHelper
	  Private Shared _id As String = "IDL:omg.org/DynamicAny/DynValue:1.0"

	  Public Shared Sub insert(ByVal a As org.omg.CORBA.Any, ByVal that As org.omg.DynamicAny.DynValue)
		Dim out As org.omg.CORBA.portable.OutputStream = a.create_output_stream()
		a.type(type())
		write(out, that)
		a.read_value(out.create_input_stream(), type())
	  End Sub

	  Public Shared Function extract(ByVal a As org.omg.CORBA.Any) As org.omg.DynamicAny.DynValue
		Return read(a.create_input_stream())
	  End Function

	  Private Shared __typeCode As org.omg.CORBA.TypeCode = Nothing
	  <MethodImpl(MethodImplOptions.Synchronized)> _
	  Public Shared Function type() As org.omg.CORBA.TypeCode
		If __typeCode Is Nothing Then __typeCode = org.omg.CORBA.ORB.init().create_interface_tc(org.omg.DynamicAny.DynValueHelper.id(), "DynValue")
		Return __typeCode
	  End Function

	  Public Shared Function id() As String
		Return _id
	  End Function

	  Public Shared Function read(ByVal istream As org.omg.CORBA.portable.InputStream) As org.omg.DynamicAny.DynValue
		  Throw New org.omg.CORBA.MARSHAL
	  End Function

	  Public Shared Sub write(ByVal ostream As org.omg.CORBA.portable.OutputStream, ByVal value As org.omg.DynamicAny.DynValue)
		  Throw New org.omg.CORBA.MARSHAL
	  End Sub

	  Public Shared Function narrow(ByVal obj As org.omg.CORBA.Object) As org.omg.DynamicAny.DynValue
		If obj Is Nothing Then
		  Return Nothing
		ElseIf TypeOf obj Is org.omg.DynamicAny.DynValue Then
		  Return CType(obj, org.omg.DynamicAny.DynValue)
		ElseIf Not obj._is_a(id()) Then
		  Throw New org.omg.CORBA.BAD_PARAM
		Else
		  Dim [delegate] As org.omg.CORBA.portable.Delegate = CType(obj, org.omg.CORBA.portable.ObjectImpl)._get_delegate()
		  Dim stub As New org.omg.DynamicAny._DynValueStub
		  stub._set_delegate([delegate])
		  Return stub
		End If
	  End Function

	  Public Shared Function unchecked_narrow(ByVal obj As org.omg.CORBA.Object) As org.omg.DynamicAny.DynValue
		If obj Is Nothing Then
		  Return Nothing
		ElseIf TypeOf obj Is org.omg.DynamicAny.DynValue Then
		  Return CType(obj, org.omg.DynamicAny.DynValue)
		Else
		  Dim [delegate] As org.omg.CORBA.portable.Delegate = CType(obj, org.omg.CORBA.portable.ObjectImpl)._get_delegate()
		  Dim stub As New org.omg.DynamicAny._DynValueStub
		  stub._set_delegate([delegate])
		  Return stub
		End If
	  End Function

	End Class

End Namespace