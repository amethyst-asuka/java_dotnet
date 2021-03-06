Namespace org.omg.PortableServer


	''' <summary>
	''' org/omg/PortableServer/ImplicitActivationPolicyValue.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/PortableServer/poa.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>


	''' <summary>
	''' ImplicitActivationPolicyValue has the following
	''' semantics.
	''' IMPLICIT_ACTIVATION to indicate implicit activation
	''' of servants.  This requires SYSTEM_ID and RETAIN 
	''' policies to be set.
	''' NO_IMPLICIT_ACTIVATION to indicate no implicit 
	''' servant activation.
	''' </summary>
	Public Class ImplicitActivationPolicyValue
		Implements org.omg.CORBA.portable.IDLEntity

	  Private __value As Integer
	  Private Shared __size As Integer = 2
	  Private Shared __array As org.omg.PortableServer.ImplicitActivationPolicyValue() = New org.omg.PortableServer.ImplicitActivationPolicyValue (__size - 1){}

	  Public Const _IMPLICIT_ACTIVATION As Integer = 0
	  Public Shared ReadOnly IMPLICIT_ACTIVATION As New org.omg.PortableServer.ImplicitActivationPolicyValue(_IMPLICIT_ACTIVATION)
	  Public Const _NO_IMPLICIT_ACTIVATION As Integer = 1
	  Public Shared ReadOnly NO_IMPLICIT_ACTIVATION As New org.omg.PortableServer.ImplicitActivationPolicyValue(_NO_IMPLICIT_ACTIVATION)

	  Public Overridable Function value() As Integer
		Return __value
	  End Function

	  Public Shared Function from_int(ByVal value As Integer) As org.omg.PortableServer.ImplicitActivationPolicyValue
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
	End Class ' class ImplicitActivationPolicyValue

End Namespace