Namespace org.omg.IOP


	''' <summary>
	''' org/omg/IOP/Encoding.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/PortableInterceptor/IOP.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>

	Public NotInheritable Class Encoding
		Implements org.omg.CORBA.portable.IDLEntity

	  ''' <summary>
	  ''' The encoding format.
	  ''' </summary>
	  Public format As Short = CShort(0)

	  ''' <summary>
	  ''' The major version of this Encoding format.
	  ''' </summary>
	  Public major_version As SByte = CByte(0)

	  ''' <summary>
	  ''' The minor version of this Encoding format.
	  ''' </summary>
	  Public minor_version As SByte = CByte(0)

	  Public Sub New()
	  End Sub ' ctor

	  Public Sub New(ByVal _format As Short, ByVal _major_version As SByte, ByVal _minor_version As SByte)
		format = _format
		major_version = _major_version
		minor_version = _minor_version
	  End Sub ' ctor

	End Class ' class Encoding

End Namespace