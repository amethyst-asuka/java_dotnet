Namespace org.omg.DynamicAny.DynAnyPackage


	''' <summary>
	''' org/omg/DynamicAny/DynAnyPackage/TypeMismatch.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/DynamicAny/DynamicAny.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>

	Public NotInheritable Class TypeMismatch
		Inherits org.omg.CORBA.UserException

	  Public Sub New()
		MyBase.New(TypeMismatchHelper.id())
	  End Sub ' ctor


	  Public Sub New(ByVal $reason As String)
		MyBase.New(TypeMismatchHelper.id() & "  " & $reason)
	  End Sub ' ctor

	End Class ' class TypeMismatch

End Namespace