Namespace org.omg.CosNaming

	''' <summary>
	''' org/omg/CosNaming/BindingTypeHolder.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/CosNaming/nameservice.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>


	''' <summary>
	''' Specifies whether the given binding is for a object (that is not a
	''' naming context) or for a naming context.
	''' </summary>
	Public NotInheritable Class BindingTypeHolder
		Implements org.omg.CORBA.portable.Streamable

	  Public value As org.omg.CosNaming.BindingType = Nothing

	  Public Sub New()
	  End Sub

	  Public Sub New(ByVal initialValue As org.omg.CosNaming.BindingType)
		value = initialValue
	  End Sub

	  Public Sub _read(ByVal i As org.omg.CORBA.portable.InputStream)
		value = org.omg.CosNaming.BindingTypeHelper.read(i)
	  End Sub

	  Public Sub _write(ByVal o As org.omg.CORBA.portable.OutputStream)
		org.omg.CosNaming.BindingTypeHelper.write(o, value)
	  End Sub

	  Public Function _type() As org.omg.CORBA.TypeCode
		Return org.omg.CosNaming.BindingTypeHelper.type()
	  End Function

	End Class

End Namespace