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
	''' The Holder for <tt>WrongTransaction</tt>.  For more information on
	''' Holder files, see <a href="doc-files/generatedfiles.html#holder">
	''' "Generated Files: Holder Files"</a>.<P>
	''' org/omg/CORBA/WrongTransactionHolder.java
	''' Generated by the IDL-to-Java compiler (portable), version "3.0"
	''' from CORBA.idl
	''' Thursday, August 24, 2000 5:32:50 PM PDT
	''' </summary>

	Public NotInheritable Class WrongTransactionHolder
		Implements org.omg.CORBA.portable.Streamable

	  Public value As org.omg.CORBA.WrongTransaction = Nothing

	  Public Sub New()
	  End Sub

	  Public Sub New(ByVal initialValue As org.omg.CORBA.WrongTransaction)
		value = initialValue
	  End Sub

	  Public Sub _read(ByVal i As org.omg.CORBA.portable.InputStream)
		value = org.omg.CORBA.WrongTransactionHelper.read(i)
	  End Sub

	  Public Sub _write(ByVal o As org.omg.CORBA.portable.OutputStream)
		org.omg.CORBA.WrongTransactionHelper.write(o, value)
	  End Sub

	  Public Function _type() As org.omg.CORBA.TypeCode
		Return org.omg.CORBA.WrongTransactionHelper.type()
	  End Function

	End Class

End Namespace