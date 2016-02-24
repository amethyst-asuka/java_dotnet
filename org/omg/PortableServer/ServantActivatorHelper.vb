Imports System.Runtime.CompilerServices

Namespace org.omg.PortableServer


	''' <summary>
	''' org/omg/PortableServer/ServantActivatorHelper.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/PortableServer/poa.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>


	''' <summary>
	''' When the POA has the RETAIN policy it uses servant 
	''' managers that are ServantActivators. 
	''' </summary>
	Public MustInherit Class ServantActivatorHelper
	  Private Shared _id As String = "IDL:omg.org/PortableServer/ServantActivator:2.3"

	  Public Shared Sub insert(ByVal a As org.omg.CORBA.Any, ByVal that As org.omg.PortableServer.ServantActivator)
		Dim out As org.omg.CORBA.portable.OutputStream = a.create_output_stream()
		a.type(type())
		write(out, that)
		a.read_value(out.create_input_stream(), type())
	  End Sub

	  Public Shared Function extract(ByVal a As org.omg.CORBA.Any) As org.omg.PortableServer.ServantActivator
		Return read(a.create_input_stream())
	  End Function

	  Private Shared __typeCode As org.omg.CORBA.TypeCode = Nothing
	  <MethodImpl(MethodImplOptions.Synchronized)> _
	  Public Shared Function type() As org.omg.CORBA.TypeCode
		If __typeCode Is Nothing Then __typeCode = org.omg.CORBA.ORB.init().create_interface_tc(org.omg.PortableServer.ServantActivatorHelper.id(), "ServantActivator")
		Return __typeCode
	  End Function

	  Public Shared Function id() As String
		Return _id
	  End Function

	  Public Shared Function read(ByVal istream As org.omg.CORBA.portable.InputStream) As org.omg.PortableServer.ServantActivator
		  Throw New org.omg.CORBA.MARSHAL
	  End Function

	  Public Shared Sub write(ByVal ostream As org.omg.CORBA.portable.OutputStream, ByVal value As org.omg.PortableServer.ServantActivator)
		  Throw New org.omg.CORBA.MARSHAL
	  End Sub

	  Public Shared Function narrow(ByVal obj As org.omg.CORBA.Object) As org.omg.PortableServer.ServantActivator
		If obj Is Nothing Then
		  Return Nothing
		ElseIf TypeOf obj Is org.omg.PortableServer.ServantActivator Then
		  Return CType(obj, org.omg.PortableServer.ServantActivator)
		ElseIf Not obj._is_a(id()) Then
		  Throw New org.omg.CORBA.BAD_PARAM
		Else
		  Dim [delegate] As org.omg.CORBA.portable.Delegate = CType(obj, org.omg.CORBA.portable.ObjectImpl)._get_delegate()
		  Dim stub As New org.omg.PortableServer._ServantActivatorStub
		  stub._set_delegate([delegate])
		  Return stub
		End If
	  End Function

	  Public Shared Function unchecked_narrow(ByVal obj As org.omg.CORBA.Object) As org.omg.PortableServer.ServantActivator
		If obj Is Nothing Then
		  Return Nothing
		ElseIf TypeOf obj Is org.omg.PortableServer.ServantActivator Then
		  Return CType(obj, org.omg.PortableServer.ServantActivator)
		Else
		  Dim [delegate] As org.omg.CORBA.portable.Delegate = CType(obj, org.omg.CORBA.portable.ObjectImpl)._get_delegate()
		  Dim stub As New org.omg.PortableServer._ServantActivatorStub
		  stub._set_delegate([delegate])
		  Return stub
		End If
	  End Function

	End Class

End Namespace