Imports System.Runtime.CompilerServices

Namespace org.omg.DynamicAny


	''' <summary>
	''' org/omg/DynamicAny/NameDynAnyPairSeqHelper.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/DynamicAny/DynamicAny.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>

	Public MustInherit Class NameDynAnyPairSeqHelper
	  Private Shared _id As String = "IDL:omg.org/DynamicAny/NameDynAnyPairSeq:1.0"

	  Public Shared Sub insert(ByVal a As org.omg.CORBA.Any, ByVal that As org.omg.DynamicAny.NameDynAnyPair())
		Dim out As org.omg.CORBA.portable.OutputStream = a.create_output_stream()
		a.type(type())
		write(out, that)
		a.read_value(out.create_input_stream(), type())
	  End Sub

	  Public Shared Function extract(ByVal a As org.omg.CORBA.Any) As org.omg.DynamicAny.NameDynAnyPair()
		Return read(a.create_input_stream())
	  End Function

	  Private Shared __typeCode As org.omg.CORBA.TypeCode = Nothing
	  <MethodImpl(MethodImplOptions.Synchronized)> _
	  Public Shared Function type() As org.omg.CORBA.TypeCode
		If __typeCode Is Nothing Then
		  __typeCode = org.omg.DynamicAny.NameDynAnyPairHelper.type()
		  __typeCode = org.omg.CORBA.ORB.init().create_sequence_tc(0, __typeCode)
		  __typeCode = org.omg.CORBA.ORB.init().create_alias_tc(org.omg.DynamicAny.NameDynAnyPairSeqHelper.id(), "NameDynAnyPairSeq", __typeCode)
		End If
		Return __typeCode
	  End Function

	  Public Shared Function id() As String
		Return _id
	  End Function

	  Public Shared Function read(ByVal istream As org.omg.CORBA.portable.InputStream) As org.omg.DynamicAny.NameDynAnyPair()
		Dim value As org.omg.DynamicAny.NameDynAnyPair() = Nothing
		Dim _len0 As Integer = istream.read_long()
		value = New org.omg.DynamicAny.NameDynAnyPair(_len0 - 1){}
		For _o1 As Integer = 0 To value.Length - 1
		  value(_o1) = org.omg.DynamicAny.NameDynAnyPairHelper.read(istream)
		Next _o1
		Return value
	  End Function

	  Public Shared Sub write(ByVal ostream As org.omg.CORBA.portable.OutputStream, ByVal value As org.omg.DynamicAny.NameDynAnyPair())
		ostream.write_long(value.Length)
		For _i0 As Integer = 0 To value.Length - 1
		  org.omg.DynamicAny.NameDynAnyPairHelper.write(ostream, value(_i0))
		Next _i0
	  End Sub

	End Class

End Namespace