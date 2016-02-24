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
''' The Helper for <tt>Object</tt>.  For more information on
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

	Public MustInherit Class ObjectHelper
		Private Shared _id As String = ""

		Public Shared Sub insert(ByVal a As org.omg.CORBA.Any, ByVal that As org.omg.CORBA.Object)
			Dim out As org.omg.CORBA.portable.OutputStream = a.create_output_stream()
			a.type(type())
			write(out, that)
			a.read_value(out.create_input_stream(), type())
		End Sub

		Public Shared Function extract(ByVal a As org.omg.CORBA.Any) As org.omg.CORBA.Object
			Return read(a.create_input_stream())
		End Function

		Private Shared __typeCode As org.omg.CORBA.TypeCode = Nothing
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Shared Function type() As org.omg.CORBA.TypeCode
			If __typeCode Is Nothing Then __typeCode = org.omg.CORBA.ORB.init().get_primitive_tc(TCKind.tk_objref)
			Return __typeCode
		End Function

		Public Shared Function id() As String
			Return _id
		End Function

		Public Shared Function read(ByVal istream As org.omg.CORBA.portable.InputStream) As org.omg.CORBA.Object
			Return istream.read_Object()
		End Function

		Public Shared Sub write(ByVal ostream As org.omg.CORBA.portable.OutputStream, ByVal value As org.omg.CORBA.Object)
			ostream.write_Object(value)
		End Sub

	End Class

End Namespace