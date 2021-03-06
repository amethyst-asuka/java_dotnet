Namespace org.omg.PortableInterceptor


	''' <summary>
	''' org/omg/PortableInterceptor/ServerRequestInterceptor.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/PortableInterceptor/Interceptors.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>


	''' <summary>
	''' Server-side request interceptor.
	''' <p>
	''' A request Interceptor is designed to intercept the flow of a 
	''' request/reply sequence through the ORB at specific points so that 
	''' services can query the request information and manipulate the service 
	''' contexts which are propagated between clients and servers. The primary 
	''' use of request Interceptors is to enable ORB services to transfer 
	''' context information between clients and servers. There are two types 
	''' of request Interceptors: client-side and server-side. 
	''' <p>
	''' To write a server-side Interceptor, implement the 
	''' ServerRequestInterceptor interface.
	''' </summary>
	''' <seealso cref= ServerRequestInfo </seealso>
	Public Interface ServerRequestInterceptor
		Inherits ServerRequestInterceptorOperations, org.omg.PortableInterceptor.Interceptor, org.omg.CORBA.portable.IDLEntity

	End Interface ' interface ServerRequestInterceptor

End Namespace