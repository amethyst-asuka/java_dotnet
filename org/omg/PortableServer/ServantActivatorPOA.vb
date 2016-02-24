Imports System.Collections

Namespace org.omg.PortableServer


	''' <summary>
	''' org/omg/PortableServer/ServantActivatorPOA.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/PortableServer/poa.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>


	''' <summary>
	''' When the POA has the RETAIN policy it uses servant 
	''' managers that are ServantActivators. 
	''' </summary>
	Public MustInherit Class ServantActivatorPOA
		Inherits org.omg.PortableServer.Servant
		Implements org.omg.PortableServer.ServantActivatorOperations, org.omg.CORBA.portable.InvokeHandler

			Public MustOverride Function _invoke(ByVal method As String, ByVal input As InputStream, ByVal handler As ResponseHandler) As OutputStream
			Public MustOverride Sub etherealize(ByVal oid As SByte(), ByVal adapter As org.omg.PortableServer.POA, ByVal serv As org.omg.PortableServer.Servant, ByVal cleanup_in_progress As Boolean, ByVal remaining_activations As Boolean)
			Public MustOverride Function incarnate(ByVal oid As SByte(), ByVal adapter As org.omg.PortableServer.POA) As org.omg.PortableServer.Servant

	  ' Constructors

	  Private Shared _methods As New Hashtable
	  Shared Sub New()
		_methods("incarnate") = New Integer?(0)
		_methods("etherealize") = New Integer?(1)
	  End Sub

	  Public Overridable Function _invoke(ByVal $method As String, ByVal [in] As org.omg.CORBA.portable.InputStream, ByVal $rh As org.omg.CORBA.portable.ResponseHandler) As org.omg.CORBA.portable.OutputStream
		Throw New org.omg.CORBA.BAD_OPERATION
	  End Function ' _invoke

	  ' Type-specific CORBA::Object operations
	  Private Shared __ids As String() = { "IDL:omg.org/PortableServer/ServantActivator:2.3", "IDL:omg.org/PortableServer/ServantManager:1.0"}

	  Public Overridable Function _all_interfaces(ByVal poa As org.omg.PortableServer.POA, ByVal objectId As SByte()) As String()
		Return CType(__ids.clone(), String())
	  End Function

	  Public Overridable Function _this() As ServantActivator
		Return ServantActivatorHelper.narrow(MyBase._this_object())
	  End Function

	  Public Overridable Function _this(ByVal orb As org.omg.CORBA.ORB) As ServantActivator
		Return ServantActivatorHelper.narrow(MyBase._this_object(orb))
	  End Function


	End Class ' class ServantActivatorPOA

End Namespace