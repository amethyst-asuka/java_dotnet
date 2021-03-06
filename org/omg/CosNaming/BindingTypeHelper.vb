Imports System.Runtime.CompilerServices

Namespace org.omg.CosNaming


	''' <summary>
	''' org/omg/CosNaming/BindingTypeHelper.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/CosNaming/nameservice.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>


	''' <summary>
	''' Specifies whether the given binding is for a object (that is not a
	''' naming context) or for a naming context.
	''' </summary>
	Public MustInherit Class BindingTypeHelper
	  Private Shared _id As String = "IDL:omg.org/CosNaming/BindingType:1.0"

	  Public Shared Sub insert(ByVal a As org.omg.CORBA.Any, ByVal that As org.omg.CosNaming.BindingType)
		Dim out As org.omg.CORBA.portable.OutputStream = a.create_output_stream()
		a.type(type())
		write(out, that)
		a.read_value(out.create_input_stream(), type())
	  End Sub

	  Public Shared Function extract(ByVal a As org.omg.CORBA.Any) As org.omg.CosNaming.BindingType
		Return read(a.create_input_stream())
	  End Function

	  Private Shared __typeCode As org.omg.CORBA.TypeCode = Nothing
	  <MethodImpl(MethodImplOptions.Synchronized)> _
	  Public Shared Function type() As org.omg.CORBA.TypeCode
		If __typeCode Is Nothing Then __typeCode = org.omg.CORBA.ORB.init().create_enum_tc(org.omg.CosNaming.BindingTypeHelper.id(), "BindingType", New String() { "nobject", "ncontext"})
		Return __typeCode
	  End Function

	  Public Shared Function id() As String
		Return _id
	  End Function

	  Public Shared Function read(ByVal istream As org.omg.CORBA.portable.InputStream) As org.omg.CosNaming.BindingType
		Return org.omg.CosNaming.BindingType.from_int(istream.read_long())
	  End Function

	  Public Shared Sub write(ByVal ostream As org.omg.CORBA.portable.OutputStream, ByVal value As org.omg.CosNaming.BindingType)
		ostream.write_long(value.value())
	  End Sub

	End Class

End Namespace