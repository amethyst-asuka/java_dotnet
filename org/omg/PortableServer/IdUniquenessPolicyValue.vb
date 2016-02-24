Namespace org.omg.PortableServer


	''' <summary>
	''' org/omg/PortableServer/IdUniquenessPolicyValue.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/PortableServer/poa.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>


	''' <summary>
	''' IdUniquenessPolicyValue can have the following values.
	''' UNIQUE_ID - Servants activated with that POA support 
	''' exactly one Object Id.  MULTIPLE_ID - a servant 
	''' activated with that POA may support one or more 
	''' Object Ids.
	''' </summary>
	Public Class IdUniquenessPolicyValue
		Implements org.omg.CORBA.portable.IDLEntity

	  Private __value As Integer
	  Private Shared __size As Integer = 2
	  Private Shared __array As org.omg.PortableServer.IdUniquenessPolicyValue() = New org.omg.PortableServer.IdUniquenessPolicyValue (__size - 1){}

	  Public Const _UNIQUE_ID As Integer = 0
	  Public Shared ReadOnly UNIQUE_ID As New org.omg.PortableServer.IdUniquenessPolicyValue(_UNIQUE_ID)
	  Public Const _MULTIPLE_ID As Integer = 1
	  Public Shared ReadOnly MULTIPLE_ID As New org.omg.PortableServer.IdUniquenessPolicyValue(_MULTIPLE_ID)

	  Public Overridable Function value() As Integer
		Return __value
	  End Function

	  Public Shared Function from_int(ByVal value As Integer) As org.omg.PortableServer.IdUniquenessPolicyValue
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
	End Class ' class IdUniquenessPolicyValue

End Namespace