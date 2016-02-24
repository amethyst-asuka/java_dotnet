Namespace org.omg.IOP


	''' <summary>
	''' org/omg/IOP/IOR.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/PortableInterceptor/IOP.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>

	Public NotInheritable Class IOR
		Implements org.omg.CORBA.portable.IDLEntity

	  ''' <summary>
	  ''' The type id, represented as a String. </summary>
	  Public type_id As String = Nothing

	  ''' <summary>
	  ''' An array of tagged profiles associated with this 
	  ''' object reference. 
	  ''' </summary>
	  Public profiles As org.omg.IOP.TaggedProfile() = Nothing

	  Public Sub New()
	  End Sub ' ctor

	  Public Sub New(ByVal _type_id As String, ByVal _profiles As org.omg.IOP.TaggedProfile())
		type_id = _type_id
		profiles = _profiles
	  End Sub ' ctor

	End Class ' class IOR

End Namespace