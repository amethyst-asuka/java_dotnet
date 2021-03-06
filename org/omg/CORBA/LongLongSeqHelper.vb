Imports System.Runtime.CompilerServices

'
' * Copyright (c) 1999, 2001, Oracle and/or its affiliates. All rights reserved.
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
Namespace org.omg.CORBA


	''' <summary>
	''' The Helper for <tt>LongLongSeq</tt>.  For more information on
	''' Helper files, see <a href="doc-files/generatedfiles.html#helper">
	''' "Generated Files: Helper Files"</a>.<P>
	''' org/omg/CORBA/LongLongSeqHelper.java
	''' Generated by the IDL-to-Java compiler (portable), version "3.0"
	''' from streams.idl
	''' 13 May 1999 22:41:37 o'clock GMT+00:00
	''' 
	''' The class definition has been modified to conform to the following
	''' OMG specifications :
	'''   <ul>
	'''       <li> ORB core as defined by CORBA 2.3.1
	'''       (<a href="http://cgi.omg.org/cgi-bin/doc?formal/99-10-07">formal/99-10-07</a>)
	'''       </li>
	''' 
	'''       <li> IDL/Java Language Mapping as defined in
	'''       <a href="http://cgi.omg.org/cgi-bin/doc?ptc/00-01-08">ptc/00-01-08</a>
	'''       </li>
	'''   </ul>
	''' </summary>

	Public MustInherit Class LongLongSeqHelper
		Private Shared _id As String = "IDL:omg.org/CORBA/LongLongSeq:1.0"

		Public Shared Sub insert(ByVal a As org.omg.CORBA.Any, ByVal that As Long())
			Dim out As org.omg.CORBA.portable.OutputStream = a.create_output_stream()
			a.type(type())
			write(out, that)
			a.read_value(out.create_input_stream(), type())
		End Sub

		Public Shared Function extract(ByVal a As org.omg.CORBA.Any) As Long()
			Return read(a.create_input_stream())
		End Function

		Private Shared __typeCode As org.omg.CORBA.TypeCode = Nothing
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Shared Function type() As org.omg.CORBA.TypeCode
			If __typeCode Is Nothing Then
					__typeCode = org.omg.CORBA.ORB.init().get_primitive_tc(org.omg.CORBA.TCKind.tk_longlong)
					__typeCode = org.omg.CORBA.ORB.init().create_sequence_tc(0, __typeCode)
					__typeCode = org.omg.CORBA.ORB.init().create_alias_tc(org.omg.CORBA.LongLongSeqHelper.id(), "LongLongSeq", __typeCode)
			End If
			Return __typeCode
		End Function

		Public Shared Function id() As String
			Return _id
		End Function

		Public Shared Function read(ByVal istream As org.omg.CORBA.portable.InputStream) As Long()
			Dim value As Long() = Nothing
			Dim _len0 As Integer = istream.read_long()
			value = New Long(_len0 - 1){}
			istream.read_longlong_array(value, 0, _len0)
			Return value
		End Function

		Public Shared Sub write(ByVal ostream As org.omg.CORBA.portable.OutputStream, ByVal value As Long())
			ostream.write_long(value.Length)
			ostream.write_longlong_array(value, 0, value.Length)
		End Sub

	End Class

End Namespace