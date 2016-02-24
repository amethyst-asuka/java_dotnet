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
	''' The Helper for <tt>UnionMember</tt>.  For more information on
	''' Helper files, see <a href="doc-files/generatedfiles.html#helper">
	''' "Generated Files: Helper Files"</a>.<P>
	''' org/omg/CORBA/UnionMemberHelper.java
	''' Generated by the IDL-to-Java compiler (portable), version "3.0"
	''' from ir.idl
	''' 03 June 1999 11:33:43 o'clock GMT+00:00
	''' </summary>

	Public MustInherit Class UnionMemberHelper
	  Private Shared _id As String = "IDL:omg.org/CORBA/UnionMember:1.0"

	  Public Shared Sub insert(ByVal a As org.omg.CORBA.Any, ByVal that As org.omg.CORBA.UnionMember)
		Dim out As org.omg.CORBA.portable.OutputStream = a.create_output_stream()
		a.type(type())
		write(out, that)
		a.read_value(out.create_input_stream(), type())
	  End Sub

	  Public Shared Function extract(ByVal a As org.omg.CORBA.Any) As org.omg.CORBA.UnionMember
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
			  Dim _members0 As org.omg.CORBA.StructMember() = New org.omg.CORBA.StructMember (3){}
			  Dim _tcOf_members0 As org.omg.CORBA.TypeCode = Nothing
			  _tcOf_members0 = org.omg.CORBA.ORB.init().create_string_tc(0)
			  _tcOf_members0 = org.omg.CORBA.ORB.init().create_alias_tc(org.omg.CORBA.IdentifierHelper.id(), "Identifier", _tcOf_members0)
			  _members0(0) = New org.omg.CORBA.StructMember("name", _tcOf_members0, Nothing)
			  _tcOf_members0 = org.omg.CORBA.ORB.init().get_primitive_tc(org.omg.CORBA.TCKind.tk_any)
			  _members0(1) = New org.omg.CORBA.StructMember("label", _tcOf_members0, Nothing)
			  _tcOf_members0 = org.omg.CORBA.ORB.init().get_primitive_tc(org.omg.CORBA.TCKind.tk_TypeCode)
			  _members0(2) = New org.omg.CORBA.StructMember("type", _tcOf_members0, Nothing)
			  _tcOf_members0 = org.omg.CORBA.IDLTypeHelper.type()
			  _members0(3) = New org.omg.CORBA.StructMember("type_def", _tcOf_members0, Nothing)
			  __typeCode = org.omg.CORBA.ORB.init().create_struct_tc(org.omg.CORBA.UnionMemberHelper.id(), "UnionMember", _members0)
			  __active = False
			End If
		  End SyncLock
		End If
		Return __typeCode
	  End Function

	  Public Shared Function id() As String
		Return _id
	  End Function

	  Public Shared Function read(ByVal istream As org.omg.CORBA.portable.InputStream) As org.omg.CORBA.UnionMember
		Dim value As New org.omg.CORBA.UnionMember
		value.name = istream.read_string()
		value.label = istream.read_any()
		value.type = istream.read_TypeCode()
		value.type_def = org.omg.CORBA.IDLTypeHelper.read(istream)
		Return value
	  End Function

	  Public Shared Sub write(ByVal ostream As org.omg.CORBA.portable.OutputStream, ByVal value As org.omg.CORBA.UnionMember)
		ostream.write_string(value.name)
		ostream.write_any(value.label)
		ostream.write_TypeCode(value.type)
		org.omg.CORBA.IDLTypeHelper.write(ostream, value.type_def)
	  End Sub

	End Class

End Namespace