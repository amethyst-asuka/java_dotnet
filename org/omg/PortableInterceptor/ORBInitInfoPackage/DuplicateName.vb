Namespace org.omg.PortableInterceptor.ORBInitInfoPackage


	''' <summary>
	''' org/omg/PortableInterceptor/ORBInitInfoPackage/DuplicateName.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/PortableInterceptor/Interceptors.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>

	Public NotInheritable Class DuplicateName
		Inherits org.omg.CORBA.UserException

	  ''' <summary>
	  ''' The name for which there was already an interceptor registered.
	  ''' </summary>
	  Public name As String = Nothing

	  Public Sub New()
		MyBase.New(DuplicateNameHelper.id())
	  End Sub ' ctor

	  Public Sub New(ByVal _name As String)
		MyBase.New(DuplicateNameHelper.id())
		name = _name
	  End Sub ' ctor


	  Public Sub New(ByVal $reason As String, ByVal _name As String)
		MyBase.New(DuplicateNameHelper.id() & "  " & $reason)
		name = _name
	  End Sub ' ctor

	End Class ' class DuplicateName

End Namespace