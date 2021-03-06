Imports System.Runtime.CompilerServices

Namespace org.omg.PortableInterceptor


	''' <summary>
	''' org/omg/PortableInterceptor/ObjectReferenceTemplateSeqHelper.java .
	''' Generated by the IDL-to-Java compiler (portable), version "3.2"
	''' from d:/re/puppet/workspace/8-2-build-windows-amd64-cygwin/jdk8u73/6086/corba/src/share/classes/org/omg/PortableInterceptor/Interceptors.idl
	''' Friday, January 29, 2016 5:40:18 PM PST
	''' </summary>


	''' <summary>
	''' Sequence of object reference templates is used for reporting state
	''' changes that do not occur on the adapter manager.
	''' </summary>
	Public MustInherit Class ObjectReferenceTemplateSeqHelper
	  Private Shared _id As String = "IDL:omg.org/PortableInterceptor/ObjectReferenceTemplateSeq:1.0"

	  Public Shared Sub insert(ByVal a As org.omg.CORBA.Any, ByVal that As org.omg.PortableInterceptor.ObjectReferenceTemplate())
		Dim out As org.omg.CORBA.portable.OutputStream = a.create_output_stream()
		a.type(type())
		write(out, that)
		a.read_value(out.create_input_stream(), type())
	  End Sub

	  Public Shared Function extract(ByVal a As org.omg.CORBA.Any) As org.omg.PortableInterceptor.ObjectReferenceTemplate()
		Return read(a.create_input_stream())
	  End Function

	  Private Shared __typeCode As org.omg.CORBA.TypeCode = Nothing
	  <MethodImpl(MethodImplOptions.Synchronized)> _
	  Public Shared Function type() As org.omg.CORBA.TypeCode
		If __typeCode Is Nothing Then
		  __typeCode = org.omg.PortableInterceptor.ObjectReferenceTemplateHelper.type()
		  __typeCode = org.omg.CORBA.ORB.init().create_sequence_tc(0, __typeCode)
		  __typeCode = org.omg.CORBA.ORB.init().create_alias_tc(org.omg.PortableInterceptor.ObjectReferenceTemplateSeqHelper.id(), "ObjectReferenceTemplateSeq", __typeCode)
		End If
		Return __typeCode
	  End Function

	  Public Shared Function id() As String
		Return _id
	  End Function

	  Public Shared Function read(ByVal istream As org.omg.CORBA.portable.InputStream) As org.omg.PortableInterceptor.ObjectReferenceTemplate()
		Dim value As org.omg.PortableInterceptor.ObjectReferenceTemplate() = Nothing
		Dim _len0 As Integer = istream.read_long()
		value = New org.omg.PortableInterceptor.ObjectReferenceTemplate(_len0 - 1){}
		For _o1 As Integer = 0 To value.Length - 1
		  value(_o1) = org.omg.PortableInterceptor.ObjectReferenceTemplateHelper.read(istream)
		Next _o1
		Return value
	  End Function

	  Public Shared Sub write(ByVal ostream As org.omg.CORBA.portable.OutputStream, ByVal value As org.omg.PortableInterceptor.ObjectReferenceTemplate())
		ostream.write_long(value.Length)
		For _i0 As Integer = 0 To value.Length - 1
		  org.omg.PortableInterceptor.ObjectReferenceTemplateHelper.write(ostream, value(_i0))
		Next _i0
	  End Sub

	End Class

End Namespace