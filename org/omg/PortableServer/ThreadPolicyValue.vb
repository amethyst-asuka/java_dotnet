Namespace org.omg.PortableServer


	''' <summary>
	''' org/omg/PortableServer/ThreadPolicyValue.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/PortableServer/poa.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>


	''' <summary>
	''' The ThreadPolicyValue can have the following values.
	''' ORB_CTRL_MODEL - The ORB is responsible for assigning 
	''' requests for an ORB- controlled POA to threads. 
	''' SINGLE_THREAD_MODEL - Requests for a single-threaded 
	''' POA are processed sequentially. 
	''' </summary>
	Public Class ThreadPolicyValue
		Implements org.omg.CORBA.portable.IDLEntity

	  Private __value As Integer
	  Private Shared __size As Integer = 2
	  Private Shared __array As org.omg.PortableServer.ThreadPolicyValue() = New org.omg.PortableServer.ThreadPolicyValue (__size - 1){}

	  Public Const _ORB_CTRL_MODEL As Integer = 0
	  Public Shared ReadOnly ORB_CTRL_MODEL As New org.omg.PortableServer.ThreadPolicyValue(_ORB_CTRL_MODEL)
	  Public Const _SINGLE_THREAD_MODEL As Integer = 1
	  Public Shared ReadOnly SINGLE_THREAD_MODEL As New org.omg.PortableServer.ThreadPolicyValue(_SINGLE_THREAD_MODEL)

	  Public Overridable Function value() As Integer
		Return __value
	  End Function

	  Public Shared Function from_int(ByVal value As Integer) As org.omg.PortableServer.ThreadPolicyValue
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
	End Class ' class ThreadPolicyValue

End Namespace