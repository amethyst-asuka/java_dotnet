Imports System

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
'
' * Licensed Materials - Property of IBM
' * RMI-IIOP v1.0
' * Copyright IBM Corp. 1998 1999  All Rights Reserved
' *
' 

Namespace javax.rmi.CORBA


	''' <summary>
	''' Supports delegation for method implementations in <seealso cref="Util"/>.  The
	''' delegate is a singleton instance of a class that implements this
	''' interface and provides a replacement implementation for all the
	''' methods of <code>javax.rmi.CORBA.Util</code>.
	''' 
	''' Delegation is enabled by providing the delegate's class name as the
	''' value of the
	''' <code>javax.rmi.CORBA.UtilClass</code>
	''' system property.
	''' </summary>
	''' <seealso cref= Util </seealso>
	Public Interface UtilDelegate

		''' <summary>
		''' Delegation call for <seealso cref="Util#mapSystemException"/>.
		''' </summary>
		Function mapSystemException(ByVal ex As org.omg.CORBA.SystemException) As java.rmi.RemoteException

		''' <summary>
		''' Delegation call for <seealso cref="Util#writeAny"/>.
		''' </summary>
		Sub writeAny(ByVal out As org.omg.CORBA.portable.OutputStream, ByVal obj As Object)

		''' <summary>
		''' Delegation call for <seealso cref="Util#readAny"/>.
		''' </summary>
		Function readAny(ByVal [in] As org.omg.CORBA.portable.InputStream) As Object

		''' <summary>
		''' Delegation call for <seealso cref="Util#writeRemoteObject"/>.
		''' </summary>
		Sub writeRemoteObject(ByVal out As org.omg.CORBA.portable.OutputStream, ByVal obj As Object)

		''' <summary>
		''' Delegation call for <seealso cref="Util#writeAbstractObject"/>.
		''' </summary>
		Sub writeAbstractObject(ByVal out As org.omg.CORBA.portable.OutputStream, ByVal obj As Object)

		''' <summary>
		''' Delegation call for <seealso cref="Util#registerTarget"/>.
		''' </summary>
		Sub registerTarget(ByVal tie As javax.rmi.CORBA.Tie, ByVal target As java.rmi.Remote)

		''' <summary>
		''' Delegation call for <seealso cref="Util#unexportObject"/>.
		''' </summary>
		Sub unexportObject(ByVal target As java.rmi.Remote)

		''' <summary>
		''' Delegation call for <seealso cref="Util#getTie"/>.
		''' </summary>
		Function getTie(ByVal target As java.rmi.Remote) As javax.rmi.CORBA.Tie

		''' <summary>
		''' Delegation call for <seealso cref="Util#createValueHandler"/>.
		''' </summary>
		Function createValueHandler() As javax.rmi.CORBA.ValueHandler

		''' <summary>
		''' Delegation call for <seealso cref="Util#getCodebase"/>.
		''' </summary>
		Function getCodebase(ByVal clz As Type) As String

		''' <summary>
		''' Delegation call for <seealso cref="Util#loadClass"/>.
		''' </summary>
		Function loadClass(ByVal className As String, ByVal remoteCodebase As String, ByVal loader As ClassLoader) As Type

		''' <summary>
		''' Delegation call for <seealso cref="Util#isLocal"/>.
		''' </summary>
		Function isLocal(ByVal stub As Stub) As Boolean

		''' <summary>
		''' Delegation call for <seealso cref="Util#wrapException"/>.
		''' </summary>
		Function wrapException(ByVal obj As Exception) As java.rmi.RemoteException

		''' <summary>
		''' Delegation call for <seealso cref="Util#copyObject"/>.
		''' </summary>
		Function copyObject(ByVal obj As Object, ByVal orb As org.omg.CORBA.ORB) As Object

		''' <summary>
		''' Delegation call for <seealso cref="Util#copyObjects"/>.
		''' </summary>
		Function copyObjects(ByVal obj As Object(), ByVal orb As org.omg.CORBA.ORB) As Object()

	End Interface

End Namespace