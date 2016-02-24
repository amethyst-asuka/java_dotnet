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
	''' The Holder for <tt>WCharSeq</tt>.  For more information on
	''' Holder files, see <a href="doc-files/generatedfiles.html#holder">
	''' "Generated Files: Holder Files"</a>.<P>
	''' org/omg/CORBA/WCharSeqHolder.java
	''' Generated by the IDL-to-Java compiler (portable), version "3.0"
	''' from streams.idl
	''' 13 May 1999 22:41:36 o'clock GMT+00:00
	''' </summary>

	Public NotInheritable Class WCharSeqHolder
		Implements org.omg.CORBA.portable.Streamable

		Public value As Char() = Nothing

		Public Sub New()
		End Sub

		Public Sub New(ByVal initialValue As Char())
			value = initialValue
		End Sub

		Public Sub _read(ByVal i As org.omg.CORBA.portable.InputStream)
			value = org.omg.CORBA.WCharSeqHelper.read(i)
		End Sub

		Public Sub _write(ByVal o As org.omg.CORBA.portable.OutputStream)
			org.omg.CORBA.WCharSeqHelper.write(o, value)
		End Sub

		Public Function _type() As org.omg.CORBA.TypeCode
			Return org.omg.CORBA.WCharSeqHelper.type()
		End Function

	End Class

End Namespace