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
	''' The Helper for <tt>CompletionStatus</tt>.  For more information on
	''' Helper files, see <a href="doc-files/generatedfiles.html#helper">
	''' "Generated Files: Helper Files"</a>.<P>
	''' org/omg/CORBA/CompletionStatusHelper.java
	''' Generated by the IDL-to-Java compiler (portable), version "3.0"
	''' 03 June 1999 11:52:03 o'clock GMT+00:00
	''' </summary>

	Public MustInherit Class CompletionStatusHelper
	  Private Shared _id As String = "IDL:omg.org/CORBA/CompletionStatus:1.0"

	  Public Shared Sub insert(ByVal a As org.omg.CORBA.Any, ByVal that As org.omg.CORBA.CompletionStatus)
		Dim out As org.omg.CORBA.portable.OutputStream = a.create_output_stream()
		a.type(type())
		write(out, that)
		a.read_value(out.create_input_stream(), type())
	  End Sub

	  Public Shared Function extract(ByVal a As org.omg.CORBA.Any) As org.omg.CORBA.CompletionStatus
		Return read(a.create_input_stream())
	  End Function

	  Private Shared __typeCode As org.omg.CORBA.TypeCode = Nothing
	  <MethodImpl(MethodImplOptions.Synchronized)> _
	  Public Shared Function type() As org.omg.CORBA.TypeCode
		If __typeCode Is Nothing Then __typeCode = org.omg.CORBA.ORB.init().create_enum_tc(org.omg.CORBA.CompletionStatusHelper.id(), "CompletionStatus", New String() { "COMPLETED_YES", "COMPLETED_NO", "COMPLETED_MAYBE"})
		Return __typeCode
	  End Function

	  Public Shared Function id() As String
		Return _id
	  End Function

	  Public Shared Function read(ByVal istream As org.omg.CORBA.portable.InputStream) As org.omg.CORBA.CompletionStatus
		Return org.omg.CORBA.CompletionStatus.from_int(istream.read_long())
	  End Function

	  Public Shared Sub write(ByVal ostream As org.omg.CORBA.portable.OutputStream, ByVal value As org.omg.CORBA.CompletionStatus)
		ostream.write_long(value.value())
	  End Sub

	End Class

End Namespace