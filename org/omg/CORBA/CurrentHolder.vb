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
	''' The Holder for <tt>Current</tt>.  For more information on
	''' Holder files, see <a href="doc-files/generatedfiles.html#holder">
	''' "Generated Files: Holder Files"</a>.<P>
	''' org/omg/CORBA/CurrentHolder.java
	''' Generated by the IDL-to-Java compiler (portable), version "3.0"
	''' from ../../../../../src/share/classes/org/omg/PortableServer/corba.idl
	''' Saturday, July 17, 1999 12:26:21 AM PDT
	''' </summary>

	Public NotInheritable Class CurrentHolder
		Implements org.omg.CORBA.portable.Streamable

	  Public value As org.omg.CORBA.Current = Nothing

	  Public Sub New()
	  End Sub

	  Public Sub New(ByVal initialValue As org.omg.CORBA.Current)
		value = initialValue
	  End Sub

	  Public Sub _read(ByVal i As org.omg.CORBA.portable.InputStream)
		value = org.omg.CORBA.CurrentHelper.read(i)
	  End Sub

	  Public Sub _write(ByVal o As org.omg.CORBA.portable.OutputStream)
		org.omg.CORBA.CurrentHelper.write(o, value)
	  End Sub

	  Public Function _type() As org.omg.CORBA.TypeCode
		Return org.omg.CORBA.CurrentHelper.type()
	  End Function

	End Class

End Namespace