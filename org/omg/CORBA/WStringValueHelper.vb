Imports System.Runtime.CompilerServices

'
' * Copyright (c) 1998, 2002, Oracle and/or its affiliates. All rights reserved.
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
''' The Helper for <tt>WStringValue</tt>.  For more information on
''' Helper files, see <a href="doc-files/generatedfiles.html#helper">
''' "Generated Files: Helper Files"</a>.<P>
''' </summary>

'
' * Licensed Materials - Property of IBM
' * RMI-IIOP v1.0
' * Copyright IBM Corp. 1998 1999  All Rights Reserved
' *
' 

Namespace org.omg.CORBA

	''' <summary>
	''' org/omg/CORBA/WStringValueHelper.java
	''' Generated by the IDL-to-Java compiler (portable), version "3.0"
	''' from orb.idl
	''' 31 May 1999 22:27:30 o'clock GMT+00:00
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

	Public Class WStringValueHelper
		Implements org.omg.CORBA.portable.BoxedValueHelper

		Private Shared _id As String = "IDL:omg.org/CORBA/WStringValue:1.0"

	  Private Shared _instance As New WStringValueHelper

		Public Shared Sub insert(ByVal a As org.omg.CORBA.Any, ByVal that As String)
			Dim out As org.omg.CORBA.portable.OutputStream = a.create_output_stream()
			a.type(type())
			write(out, that)
			a.read_value(out.create_input_stream(), type())
		End Sub

		Public Shared Function extract(ByVal a As org.omg.CORBA.Any) As String
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
									__typeCode = org.omg.CORBA.ORB.init().create_wstring_tc(0)
									__typeCode = org.omg.CORBA.ORB.init().create_value_box_tc(_id, "WStringValue", __typeCode)
									__active = False
							End If
					End SyncLock
			End If
			Return __typeCode
		End Function

		Public Shared Function id() As String
			Return _id
		End Function

		Public Shared Function read(ByVal istream As org.omg.CORBA.portable.InputStream) As String
		If Not(TypeOf istream Is org.omg.CORBA_2_3.portable.InputStream) Then Throw New org.omg.CORBA.BAD_PARAM
		Return CStr(CType(istream, org.omg.CORBA_2_3.portable.InputStream).read_value(_instance))
		End Function

	  Public Overridable Function read_value(ByVal istream As org.omg.CORBA.portable.InputStream) As java.io.Serializable
		Dim tmp As String
		tmp = istream.read_wstring()
		Return CType(tmp, java.io.Serializable)
	  End Function

	  Public Shared Sub write(ByVal ostream As org.omg.CORBA.portable.OutputStream, ByVal value As String)
		If Not(TypeOf ostream Is org.omg.CORBA_2_3.portable.OutputStream) Then Throw New org.omg.CORBA.BAD_PARAM
		CType(ostream, org.omg.CORBA_2_3.portable.OutputStream).write_value(value, _instance)
	  End Sub

		Public Overridable Sub write_value(ByVal ostream As org.omg.CORBA.portable.OutputStream, ByVal value As java.io.Serializable)
		If Not(TypeOf value Is String) Then Throw New org.omg.CORBA.MARSHAL
		Dim valueType As String = CStr(value)
		ostream.write_wstring(valueType)
		End Sub

		Public Overridable Function get_id() As String
			Return _id
		End Function

	End Class

End Namespace