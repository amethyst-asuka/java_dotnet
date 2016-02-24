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
 ''' The Helper for <tt>ServiceInformation</tt>.  For more information on
 ''' Helper files, see <a href="doc-files/generatedfiles.html#helper">
 ''' "Generated Files: Helper Files"</a>.<P>
 ''' </summary>

Namespace org.omg.CORBA


	Public MustInherit Class ServiceInformationHelper

		Public Shared Sub write(ByVal out As org.omg.CORBA.portable.OutputStream, ByVal that As org.omg.CORBA.ServiceInformation)
			out.write_long(that.service_options.Length)
			out.write_ulong_array(that.service_options, 0, that.service_options.Length)
			out.write_long(that.service_details.Length)
			For i As Integer = 0 To that.service_details.Length - 1
				org.omg.CORBA.ServiceDetailHelper.write(out, that.service_details(i))
			Next i
		End Sub

		Public Shared Function read(ByVal [in] As org.omg.CORBA.portable.InputStream) As org.omg.CORBA.ServiceInformation
			Dim that As New org.omg.CORBA.ServiceInformation
				Dim __length As Integer = [in].read_long()
				that.service_options = New Integer(__length - 1){}
				[in].read_ulong_array(that.service_options, 0, that.service_options.Length)
				Dim __length As Integer = [in].read_long()
				that.service_details = New org.omg.CORBA.ServiceDetail(__length - 1){}
				For __index As Integer = 0 To that.service_details.Length - 1
					that.service_details(__index) = org.omg.CORBA.ServiceDetailHelper.read([in])
				Next __index
			Return that
		End Function
		Public Shared Function extract(ByVal a As org.omg.CORBA.Any) As org.omg.CORBA.ServiceInformation
			Dim [in] As org.omg.CORBA.portable.InputStream = a.create_input_stream()
			Return read([in])
		End Function
		Public Shared Sub insert(ByVal a As org.omg.CORBA.Any, ByVal that As org.omg.CORBA.ServiceInformation)
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
				_members(0) = New org.omg.CORBA.StructMember("service_options", org.omg.CORBA.ORB.init().create_sequence_tc(0, org.omg.CORBA.ORB.init().get_primitive_tc(org.omg.CORBA.TCKind.tk_ulong)), Nothing)

				_members(1) = New org.omg.CORBA.StructMember("service_details", org.omg.CORBA.ORB.init().create_sequence_tc(0, org.omg.CORBA.ServiceDetailHelper.type()), Nothing)
				_tc = org.omg.CORBA.ORB.init().create_struct_tc(id(), "ServiceInformation", _members)
			End If
			Return _tc
		End Function
		Public Shared Function id() As String
			Return "IDL:omg.org/CORBA/ServiceInformation:1.0"
		End Function
	End Class

End Namespace