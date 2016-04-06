Imports System
Imports System.Diagnostics

'
' * Copyright (c) 2007, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.net


	''' <summary>
	''' This class defines a factory for creating DatagramSocketImpls. It defaults
	''' to creating plain DatagramSocketImpls, but may create other DatagramSocketImpls
	''' by setting the impl.prefix system property.
	''' 
	''' For Windows versions lower than Windows Vista a TwoStacksPlainDatagramSocketImpl
	''' is always created. This impl supports IPv6 on these platform where available.
	''' 
	''' On Windows platforms greater than Vista that support a dual layer TCP/IP stack
	''' a DualStackPlainDatagramSocketImpl is created for DatagramSockets. For MulticastSockets
	''' a TwoStacksPlainDatagramSocketImpl is always created. This is to overcome the lack
	''' of behavior defined for multicasting over a dual layer socket by the RFC.
	''' 
	''' @author Chris Hegarty
	''' </summary>

	Friend Class DefaultDatagramSocketImplFactory
		Friend Shared prefixImplClass As  [Class] = Nothing

		' the windows version. 
		Private Shared version As Single

		' java.net.preferIPv4Stack 
		Private Shared preferIPv4Stack As Boolean = False

		' If the version supports a dual stack TCP implementation 
		Private Shared useDualStackImpl As Boolean = False

		' sun.net.useExclusiveBind 
		Private Shared exclBindProp As String

		' True if exclusive binding is on for Windows 
		Private Shared exclusiveBind As Boolean = True


		Shared Sub New()
			' Determine Windows Version.
			java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)

			' (version >= 6.0) implies Vista or greater.
			If version >= 6.0 AndAlso (Not preferIPv4Stack) Then useDualStackImpl = True
			If exclBindProp IsNot Nothing Then
				' sun.net.useExclusiveBind is true
				exclusiveBind = If(exclBindProp.length() = 0, True, Convert.ToBoolean(exclBindProp))
			ElseIf version < 6.0 Then
				exclusiveBind = False
			End If

			' impl.prefix
			Dim prefix As String = Nothing
			Try
				prefix = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("impl.prefix", Nothing))
				If prefix IsNot Nothing Then prefixImplClass = Type.GetType("java.net." & prefix & "DatagramSocketImpl")
			Catch e As Exception
				Console.Error.WriteLine("Can't find class: java.net." & prefix & "DatagramSocketImpl: check impl.prefix property")
			End Try
		End Sub

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As Object
				version = 0
				Try
					version = Convert.ToSingle(System.properties.getProperty("os.version"))
					preferIPv4Stack = Convert.ToBoolean(System.properties.getProperty("java.net.preferIPv4Stack"))
					exclBindProp = System.getProperty("sun.net.useExclusiveBind")
				Catch e As NumberFormatException
					Debug.Assert(False, e)
				End Try
				Return Nothing ' nothing to return
			End Function
		End Class

		''' <summary>
		''' Creates a new <code>DatagramSocketImpl</code> instance.
		''' </summary>
		''' <param name="isMulticast"> true if this impl is to be used for a MutlicastSocket </param>
		''' <returns>  a new instance of <code>PlainDatagramSocketImpl</code>. </returns>
		Friend Shared Function createDatagramSocketImpl(  isMulticast As Boolean) As DatagramSocketImpl
			If prefixImplClass IsNot Nothing Then
				Try
					Return CType(prefixImplClass.newInstance(), DatagramSocketImpl)
				Catch e As Exception
					Throw New SocketException("can't instantiate DatagramSocketImpl")
				End Try
			Else
				If isMulticast Then exclusiveBind = False
				If useDualStackImpl AndAlso (Not isMulticast) Then
					Return New DualStackPlainDatagramSocketImpl(exclusiveBind)
				Else
					Return New TwoStacksPlainDatagramSocketImpl(exclusiveBind)
				End If
			End If
		End Function
	End Class

End Namespace