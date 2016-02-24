Namespace org.omg.PortableInterceptor


	''' <summary>
	''' org/omg/PortableInterceptor/ORBInitInfo.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/PortableInterceptor/Interceptors.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>


	''' <summary>
	''' Passed to each <code>ORBInitializer</code>, allowing it to
	''' to register interceptors and perform other duties while the ORB is 
	''' initializing.
	''' <p>
	''' The <code>ORBInitInfo</code> object is only valid during 
	''' <code>ORB.init</code>.  If a service keeps a reference to its 
	''' <code>ORBInitInfo</code> object and tries to use it after 
	''' <code>ORB.init</code> returns, the object no longer exists and an 
	''' <code>OBJECT_NOT_EXIST</code> exception shall be thrown.
	''' </summary>
	''' <seealso cref= ORBInitializer </seealso>
	Public Interface ORBInitInfo
		Inherits ORBInitInfoOperations, org.omg.CORBA.Object, org.omg.CORBA.portable.IDLEntity

	End Interface ' interface ORBInitInfo

End Namespace