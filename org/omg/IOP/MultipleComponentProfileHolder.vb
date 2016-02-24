Namespace org.omg.IOP


	''' <summary>
	''' org/omg/IOP/MultipleComponentProfileHolder.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/PortableInterceptor/IOP.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>


	''' <summary>
	''' An array of tagged components, forming a multiple component profile. </summary>
	Public NotInheritable Class MultipleComponentProfileHolder
		Implements org.omg.CORBA.portable.Streamable

	  Public value As org.omg.IOP.TaggedComponent() = Nothing

	  Public Sub New()
	  End Sub

	  Public Sub New(ByVal initialValue As org.omg.IOP.TaggedComponent())
		value = initialValue
	  End Sub

	  Public Sub _read(ByVal i As org.omg.CORBA.portable.InputStream)
		value = org.omg.IOP.MultipleComponentProfileHelper.read(i)
	  End Sub

	  Public Sub _write(ByVal o As org.omg.CORBA.portable.OutputStream)
		org.omg.IOP.MultipleComponentProfileHelper.write(o, value)
	  End Sub

	  Public Function _type() As org.omg.CORBA.TypeCode
		Return org.omg.IOP.MultipleComponentProfileHelper.type()
	  End Function

	End Class

End Namespace