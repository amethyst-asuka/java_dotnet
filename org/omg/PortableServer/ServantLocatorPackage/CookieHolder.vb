'
' * Copyright (c) 1998, 2000, Oracle and/or its affiliates. All rights reserved.
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

Namespace org.omg.PortableServer.ServantLocatorPackage

	''' <summary>
	''' The native type PortableServer::ServantLocator::Cookie is mapped
	''' to java.lang.Object. A CookieHolder class is provided for passing
	''' the Cookie type as an out parameter. The CookieHolder class
	''' follows exactly the same pattern as the other holder classes
	''' for basic types.
	''' </summary>

	Public NotInheritable Class CookieHolder
		Implements org.omg.CORBA.portable.Streamable

		Public value As Object

		Public Sub New()
		End Sub

		Public Sub New(ByVal initial As Object)
			value = initial
		End Sub

		Public Sub _read(ByVal [is] As org.omg.CORBA.portable.InputStream)
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Sub

		Public Sub _write(ByVal os As org.omg.CORBA.portable.OutputStream)
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Sub

		Public Function _type() As org.omg.CORBA.TypeCode
			Throw New org.omg.CORBA.NO_IMPLEMENT
		End Function
	End Class

End Namespace