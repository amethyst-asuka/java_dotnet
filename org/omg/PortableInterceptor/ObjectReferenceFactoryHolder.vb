Namespace org.omg.PortableInterceptor

	''' <summary>
	''' org/omg/PortableInterceptor/ObjectReferenceFactoryHolder.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/PortableInterceptor/Interceptors.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>


	''' <summary>
	''' The object reference factory.  This provides the capability of
	''' creating an object reference.
	''' </summary>
	Public NotInheritable Class ObjectReferenceFactoryHolder
		Implements org.omg.CORBA.portable.Streamable

	  Public value As org.omg.PortableInterceptor.ObjectReferenceFactory = Nothing

	  Public Sub New()
	  End Sub

	  Public Sub New(ByVal initialValue As org.omg.PortableInterceptor.ObjectReferenceFactory)
		value = initialValue
	  End Sub

	  Public Sub _read(ByVal i As org.omg.CORBA.portable.InputStream)
		value = org.omg.PortableInterceptor.ObjectReferenceFactoryHelper.read(i)
	  End Sub

	  Public Sub _write(ByVal o As org.omg.CORBA.portable.OutputStream)
		org.omg.PortableInterceptor.ObjectReferenceFactoryHelper.write(o, value)
	  End Sub

	  Public Function _type() As org.omg.CORBA.TypeCode
		Return org.omg.PortableInterceptor.ObjectReferenceFactoryHelper.type()
	  End Function

	End Class

End Namespace