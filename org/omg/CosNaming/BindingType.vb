Namespace org.omg.CosNaming


	''' <summary>
	''' org/omg/CosNaming/BindingType.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/CosNaming/nameservice.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>


	''' <summary>
	''' Specifies whether the given binding is for a object (that is not a
	''' naming context) or for a naming context.
	''' </summary>
	Public Class BindingType
		Implements org.omg.CORBA.portable.IDLEntity

	  Private __value As Integer
	  Private Shared __size As Integer = 2
	  Private Shared __array As org.omg.CosNaming.BindingType() = New org.omg.CosNaming.BindingType (__size - 1){}

	  Public Const _nobject As Integer = 0
	  Public Shared ReadOnly nobject As New org.omg.CosNaming.BindingType(_nobject)
	  Public Const _ncontext As Integer = 1
	  Public Shared ReadOnly ncontext As New org.omg.CosNaming.BindingType(_ncontext)

	  Public Overridable Function value() As Integer
		Return __value
	  End Function

	  Public Shared Function from_int(ByVal value As Integer) As org.omg.CosNaming.BindingType
		If value >= 0 AndAlso value < __size Then
		  Return __array(value)
		Else
		  Throw New org.omg.CORBA.BAD_PARAM
		End If
	  End Function

	  Protected Friend Sub New(ByVal value As Integer)
		__value = value
		__array(__value) = Me
	  End Sub
	End Class ' class BindingType

End Namespace