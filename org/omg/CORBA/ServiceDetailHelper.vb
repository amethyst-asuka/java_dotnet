Imports System.Runtime.CompilerServices

'
' * Copyright (c) 1998, 2001, Oracle and/or its affiliates. All rights reserved.
' * ORACLE PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' 

 ''' <summary>
 ''' The Helper for <tt>ServiceDetail</tt>.  For more information on
 ''' Helper files, see <a href="doc-files/generatedfiles.html#helper">
 ''' "Generated Files: Helper Files"</a>.<P>
 ''' </summary>

Namespace org.omg.CORBA


	Public MustInherit Class ServiceDetailHelper

		Public Shared Sub write(ByVal out As org.omg.CORBA.portable.OutputStream, ByVal that As org.omg.CORBA.ServiceDetail)
			out.write_ulong(that.service_detail_type)
				out.write_long(that.service_detail.Length)
				out.write_octet_array(that.service_detail, 0, that.service_detail.Length)
		End Sub
		Public Shared Function read(ByVal [in] As org.omg.CORBA.portable.InputStream) As org.omg.CORBA.ServiceDetail
			Dim that As New org.omg.CORBA.ServiceDetail
			that.service_detail_type = [in].read_ulong()
				Dim __length As Integer = [in].read_long()
				that.service_detail = New SByte(__length - 1){}
				[in].read_octet_array(that.service_detail, 0, that.service_detail.Length)
			Return that
		End Function
		Public Shared Function extract(ByVal a As org.omg.CORBA.Any) As org.omg.CORBA.ServiceDetail
			Dim [in] As org.omg.CORBA.portable.InputStream = a.create_input_stream()
			Return read([in])
		End Function
		Public Shared Sub insert(ByVal a As org.omg.CORBA.Any, ByVal that As org.omg.CORBA.ServiceDetail)
			Dim out As org.omg.CORBA.portable.OutputStream = a.create_output_stream()
			write(out, that)
			a.read_value(out.create_input_stream(), type())
		End Sub
		Private Shared _tc As org.omg.CORBA.TypeCode
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Shared Function type() As org.omg.CORBA.TypeCode
			Dim _memberCount As Integer = 2
			Dim _members As org.omg.CORBA.StructMember() = Nothing
			If _tc Is Nothing Then
				_members= New org.omg.CORBA.StructMember(1){}
				_members(0) = New org.omg.CORBA.StructMember("service_detail_type", org.omg.CORBA.ORB.init().get_primitive_tc(org.omg.CORBA.TCKind.tk_ulong), Nothing)

				_members(1) = New org.omg.CORBA.StructMember("service_detail", org.omg.CORBA.ORB.init().create_sequence_tc(0, org.omg.CORBA.ORB.init().get_primitive_tc(org.omg.CORBA.TCKind.tk_octet)), Nothing)
				_tc = org.omg.CORBA.ORB.init().create_struct_tc(id(), "ServiceDetail", _members)
			End If
			Return _tc
		End Function
		Public Shared Function id() As String
			Return "IDL:omg.org/CORBA/ServiceDetail:1.0"
		End Function
	End Class

End Namespace