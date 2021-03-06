Imports System.Runtime.CompilerServices

'
' * Copyright (c) 2000, 2001, Oracle and/or its affiliates. All rights reserved.
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
	''' The Helper for <tt>UnknownUserException</tt>.  For more information on
	''' Helper files, see <a href="doc-files/generatedfiles.html#helper">
	''' "Generated Files: Helper Files"</a>.<P>
	''' org/omg/CORBA/UnknownUserExceptionHelper.java
	''' Generated by the IDL-to-Java compiler (portable), version "3.0"
	''' from CORBA.idl
	''' Thursday, August 24, 2000 5:52:22 PM PDT
	''' </summary>

	Public MustInherit Class UnknownUserExceptionHelper
	  Private Shared _id As String = "IDL:omg.org/CORBA/UnknownUserException:1.0"

	  Public Shared Sub insert(ByVal a As org.omg.CORBA.Any, ByVal that As org.omg.CORBA.UnknownUserException)
		Dim out As org.omg.CORBA.portable.OutputStream = a.create_output_stream()
		a.type(type())
		write(out, that)
		a.read_value(out.create_input_stream(), type())
	  End Sub

	  Public Shared Function extract(ByVal a As org.omg.CORBA.Any) As org.omg.CORBA.UnknownUserException
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
			  _tcOf_members0 = org.omg.CORBA.ORB.init().get_primitive_tc(org.omg.CORBA.TCKind.tk_any)
			  _members0(0) = New org.omg.CORBA.StructMember("except", _tcOf_members0, Nothing)
			  __typeCode = org.omg.CORBA.ORB.init().create_exception_tc(org.omg.CORBA.UnknownUserExceptionHelper.id(), "UnknownUserException", _members0)
			  __active = False
			End If
		  End SyncLock
		End If
		Return __typeCode
	  End Function

	  Public Shared Function id() As String
		Return _id
	  End Function

	  Public Shared Function read(ByVal istream As org.omg.CORBA.portable.InputStream) As org.omg.CORBA.UnknownUserException
		Dim value As New org.omg.CORBA.UnknownUserException
		' read and discard the repository ID
		istream.read_string()
		value.except = istream.read_any()
		Return value
	  End Function

	  Public Shared Sub write(ByVal ostream As org.omg.CORBA.portable.OutputStream, ByVal value As org.omg.CORBA.UnknownUserException)
		' write the repository ID
		ostream.write_string(id())
		ostream.write_any(value.except)
	  End Sub

	End Class

End Namespace