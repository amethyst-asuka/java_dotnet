Imports System.Runtime.CompilerServices

Namespace org.omg.CORBA


	''' <summary>
	''' org/omg/CORBA/PolicyErrorHelper.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/PortableInterceptor/CORBAX.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>


	''' <summary>
	''' Thrown to indicate problems with parameter values passed to the
	''' <code>ORB.create_policy</code> operation.  
	''' </summary>
	Public MustInherit Class PolicyErrorHelper
	  Private Shared _id As String = "IDL:omg.org/CORBA/PolicyError:1.0"

	  Public Shared Sub insert(ByVal a As org.omg.CORBA.Any, ByVal that As org.omg.CORBA.PolicyError)
		Dim out As org.omg.CORBA.portable.OutputStream = a.create_output_stream()
		a.type(type())
		write(out, that)
		a.read_value(out.create_input_stream(), type())
	  End Sub

	  Public Shared Function extract(ByVal a As org.omg.CORBA.Any) As org.omg.CORBA.PolicyError
		Return read(a.create_input_stream())
	  End Function

	  Private Shared __typeCode As org.omg.CORBA.TypeCode = Nothing
	  Private Shared __active As Boolean = False
	  <MethodImpl(MethodImplOptions.Synchronized)> _
	  Public Shared Function type() As org.omg.CORBA.TypeCode
		If __typeCode Is Nothing Then
		  SyncLock GetType(org.omg.CORBA.TypeCode)
			If __typeCode Is Nothing Then
			  If __active Then Return org.omg.CORBA.ORB.init().create_recursive_tc(_id)
			  __active = True
			  Dim _members0 As org.omg.CORBA.StructMember() = New org.omg.CORBA.StructMember (0){}
			  Dim _tcOf_members0 As org.omg.CORBA.TypeCode = Nothing
			  _tcOf_members0 = org.omg.CORBA.ORB.init().get_primitive_tc(org.omg.CORBA.TCKind.tk_short)
			  _tcOf_members0 = org.omg.CORBA.ORB.init().create_alias_tc(org.omg.CORBA.PolicyErrorCodeHelper.id(), "PolicyErrorCode", _tcOf_members0)
			  _members0(0) = New org.omg.CORBA.StructMember("reason", _tcOf_members0, Nothing)
			  __typeCode = org.omg.CORBA.ORB.init().create_exception_tc(org.omg.CORBA.PolicyErrorHelper.id(), "PolicyError", _members0)
			  __active = False
			End If
		  End SyncLock
		End If
		Return __typeCode
	  End Function

	  Public Shared Function id() As String
		Return _id
	  End Function

	  Public Shared Function read(ByVal istream As org.omg.CORBA.portable.InputStream) As org.omg.CORBA.PolicyError
		Dim value As New org.omg.CORBA.PolicyError
		' read and discard the repository ID
		istream.read_string()
		value.reason = istream.read_short()
		Return value
	  End Function

	  Public Shared Sub write(ByVal ostream As org.omg.CORBA.portable.OutputStream, ByVal value As org.omg.CORBA.PolicyError)
		' write the repository ID
		ostream.write_string(id())
		ostream.write_short(value.reason)
	  End Sub

	End Class

End Namespace