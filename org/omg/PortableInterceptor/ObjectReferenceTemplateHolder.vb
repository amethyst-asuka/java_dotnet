Namespace org.omg.PortableInterceptor

	''' <summary>
	''' org/omg/PortableInterceptor/ObjectReferenceTemplateHolder.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/PortableInterceptor/Interceptors.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>


	''' <summary>
	''' The object reference template.  An instance of this must
	''' exist for each object adapter created in an ORB.  The server_id,
	''' orb_id, and adapter_name attributes uniquely identify this template
	''' within the scope of an IMR.  Note that adapter_id is similarly unique
	''' within the same scope, but it is opaque, and less useful in many
	''' cases.
	''' </summary>
	Public NotInheritable Class ObjectReferenceTemplateHolder
		Implements org.omg.CORBA.portable.Streamable

	  Public value As org.omg.PortableInterceptor.ObjectReferenceTemplate = Nothing

	  Public Sub New()
	  End Sub

	  Public Sub New(ByVal initialValue As org.omg.PortableInterceptor.ObjectReferenceTemplate)
		value = initialValue
	  End Sub

	  Public Sub _read(ByVal i As org.omg.CORBA.portable.InputStream)
		value = org.omg.PortableInterceptor.ObjectReferenceTemplateHelper.read(i)
	  End Sub

	  Public Sub _write(ByVal o As org.omg.CORBA.portable.OutputStream)
		org.omg.PortableInterceptor.ObjectReferenceTemplateHelper.write(o, value)
	  End Sub

	  Public Function _type() As org.omg.CORBA.TypeCode
		Return org.omg.PortableInterceptor.ObjectReferenceTemplateHelper.type()
	  End Function

	End Class

End Namespace