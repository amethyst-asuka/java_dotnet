Imports System.Runtime.CompilerServices

Namespace org.omg.PortableInterceptor


	''' <summary>
	''' org/omg/PortableInterceptor/ORBIdHelper.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/PortableInterceptor/Interceptors.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>


	' This should actually be the CORBA::ORBid type once that is available
	Public MustInherit Class ORBIdHelper
	  Private Shared _id As String = "IDL:omg.org/PortableInterceptor/ORBId:1.0"

	  Public Shared Sub insert(ByVal a As org.omg.CORBA.Any, ByVal that As String)
		Dim out As org.omg.CORBA.portable.OutputStream = a.create_output_stream()
		a.type(type())
		write(out, that)
		a.read_value(out.create_input_stream(), type())
	  End Sub

	  Public Shared Function extract(ByVal a As org.omg.CORBA.Any) As String
		Return read(a.create_input_stream())
	  End Function

	  Private Shared __typeCode As org.omg.CORBA.TypeCode = Nothing
	  <MethodImpl(MethodImplOptions.Synchronized)> _
	  Public Shared Function type() As org.omg.CORBA.TypeCode
		If __typeCode Is Nothing Then
		  __typeCode = org.omg.CORBA.ORB.init().create_string_tc(0)
		  __typeCode = org.omg.CORBA.ORB.init().create_alias_tc(org.omg.PortableInterceptor.ORBIdHelper.id(), "ORBId", __typeCode)
		End If
		Return __typeCode
	  End Function

	  Public Shared Function id() As String
		Return _id
	  End Function

	  Public Shared Function read(ByVal istream As org.omg.CORBA.portable.InputStream) As String
		Dim value As String = Nothing
		value = istream.read_string()
		Return value
	  End Function

	  Public Shared Sub write(ByVal ostream As org.omg.CORBA.portable.OutputStream, ByVal value As String)
		ostream.write_string(value)
	  End Sub

	End Class

End Namespace