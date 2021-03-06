Namespace org.omg.PortableServer


	''' <summary>
	''' org/omg/PortableServer/ServantRetentionPolicyValue.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/PortableServer/poa.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>


	''' <summary>
	''' ServantRetentionPolicyValue can have the following 
	''' values. RETAIN - to indicate that the POA will retain 
	''' active servants in its Active Object Map. 
	''' NON_RETAIN - to indicate Servants are not retained by 
	''' the POA. If no ServantRetentionPolicy is specified at 
	''' POA creation, the default is RETAIN.
	''' </summary>
	Public Class ServantRetentionPolicyValue
		Implements org.omg.CORBA.portable.IDLEntity

	  Private __value As Integer
	  Private Shared __size As Integer = 2
	  Private Shared __array As org.omg.PortableServer.ServantRetentionPolicyValue() = New org.omg.PortableServer.ServantRetentionPolicyValue (__size - 1){}

	  Public Const _RETAIN As Integer = 0
	  Public Shared ReadOnly RETAIN As New org.omg.PortableServer.ServantRetentionPolicyValue(_RETAIN)
	  Public Const _NON_RETAIN As Integer = 1
	  Public Shared ReadOnly NON_RETAIN As New org.omg.PortableServer.ServantRetentionPolicyValue(_NON_RETAIN)

	  Public Overridable Function value() As Integer
		Return __value
	  End Function

	  Public Shared Function from_int(ByVal value As Integer) As org.omg.PortableServer.ServantRetentionPolicyValue
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
	End Class ' class ServantRetentionPolicyValue

End Namespace