Imports System.Runtime.CompilerServices

Namespace org.omg.CosNaming.NamingContextExtPackage


	''' <summary>
	''' org/omg/CosNaming/NamingContextExtPackage/InvalidAddressHelper.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/CosNaming/nameservice.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>

	Public MustInherit Class InvalidAddressHelper
	  Private Shared _id As String = "IDL:omg.org/CosNaming/NamingContextExt/InvalidAddress:1.0"

	  Public Shared Sub insert(ByVal a As org.omg.CORBA.Any, ByVal that As org.omg.CosNaming.NamingContextExtPackage.InvalidAddress)
		Dim out As org.omg.CORBA.portable.OutputStream = a.create_output_stream()
		a.type(type())
		write(out, that)
		a.read_value(out.create_input_stream(), type())
	  End Sub

	  Public Shared Function extract(ByVal a As org.omg.CORBA.Any) As org.omg.CosNaming.NamingContextExtPackage.InvalidAddress
		Return read(a.create_input_stream())
	  End Function

	  Private Shared __typeCode As org.omg.CORBA.TypeCode = Nothing
	  Private Shared __active As Boolean = False
	  <MethodImpl(MethodImplOptions.Synchronized)> _
	  Public Shared Function type() As org.omg.CORBA.TypeCode
		If __typeCode Is Nothing Then
		  SyncLock GetType(org.omg.CORBA.TypeCode)
			If __typeCode Is Nothing Then
			  If __active Then Return org.omg.CORBA.ORB.init().create_recursive_tc(_id)
			  __active = True
			  Dim _members0 As org.omg.CORBA.StructMember() = New org.omg.CORBA.StructMember (){}
			  Dim _tcOf_members0 As org.omg.CORBA.TypeCode = Nothing
			  __typeCode = org.omg.CORBA.ORB.init().create_exception_tc(org.omg.CosNaming.NamingContextExtPackage.InvalidAddressHelper.id(), "InvalidAddress", _members0)
			  __active = False
			End If
		  End SyncLock
		End If
		Return __typeCode
	  End Function

	  Public Shared Function id() As String
		Return _id
	  End Function

	  Public Shared Function read(ByVal istream As org.omg.CORBA.portable.InputStream) As org.omg.CosNaming.NamingContextExtPackage.InvalidAddress
		Dim value As New org.omg.CosNaming.NamingContextExtPackage.InvalidAddress
		' read and discard the repository ID
		istream.read_string()
		Return value
	  End Function

	  Public Shared Sub write(ByVal ostream As org.omg.CORBA.portable.OutputStream, ByVal value As org.omg.CosNaming.NamingContextExtPackage.InvalidAddress)
		' write the repository ID
		ostream.write_string(id())
	  End Sub

	End Class

End Namespace