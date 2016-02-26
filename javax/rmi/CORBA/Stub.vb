Imports System
Imports System.Threading

'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Base class from which all RMI-IIOP stubs must inherit.
	''' </summary>
	<Serializable> _
	Public MustInherit Class Stub
		Inherits org.omg.CORBA_2_3.portable.ObjectImpl

		Private Const serialVersionUID As Long = 1087775603798577179L

		' This can only be set at object construction time (no sync necessary).
		<NonSerialized> _
		Private stubDelegate As StubDelegate = Nothing
		Private Shared stubDelegateClass As Type = Nothing
		Private Const StubClassKey As String = "javax.rmi.CORBA.StubClass"

		Shared Sub New()
			Dim stubDelegateInstance As Object = createDelegate(StubClassKey)
			If stubDelegateInstance IsNot Nothing Then stubDelegateClass = stubDelegateInstance.GetType()
		End Sub


		''' <summary>
		''' Returns a hash code value for the object which is the same for all stubs
		''' that represent the same remote object. </summary>
		''' <returns> the hash code value. </returns>
		Public Overrides Function GetHashCode() As Integer

			If stubDelegate Is Nothing Then defaultDelegateate()

			If stubDelegate IsNot Nothing Then Return stubDelegate.hashCode(Me)

			Return 0
		End Function

		''' <summary>
		''' Compares two stubs for equality. Returns <code>true</code> when used to compare stubs
		''' that represent the same remote object, and <code>false</code> otherwise. </summary>
		''' <param name="obj"> the reference object with which to compare. </param>
		''' <returns> <code>true</code> if this object is the same as the <code>obj</code>
		'''          argument; <code>false</code> otherwise. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean

			If stubDelegate Is Nothing Then defaultDelegateate()

			If stubDelegate IsNot Nothing Then Return stubDelegate.Equals(Me, obj)

			Return False
		End Function

		''' <summary>
		''' Returns a string representation of this stub. Returns the same string
		''' for all stubs that represent the same remote object. </summary>
		''' <returns> a string representation of this stub. </returns>
		Public Overrides Function ToString() As String


			If stubDelegate Is Nothing Then defaultDelegateate()

			Dim ior As String
			If stubDelegate IsNot Nothing Then
				ior = stubDelegate.ToString(Me)
				If ior Is Nothing Then
					Return MyBase.ToString()
				Else
					Return ior
				End If
			End If
			Return MyBase.ToString()
		End Function

		''' <summary>
		''' Connects this stub to an ORB. Required after the stub is deserialized
		''' but not after it is demarshalled by an ORB stream. If an unconnected
		''' stub is passed to an ORB stream for marshalling, it is implicitly
		''' connected to that ORB. Application code should not call this method
		''' directly, but should call the portable wrapper method
		''' <seealso cref="javax.rmi.PortableRemoteObject#connect"/>. </summary>
		''' <param name="orb"> the ORB to connect to. </param>
		''' <exception cref="RemoteException"> if the stub is already connected to a different
		''' ORB, or if the stub does not represent an exported remote or local object. </exception>
		Public Overridable Sub connect(ByVal orb As org.omg.CORBA.ORB)

			If stubDelegate Is Nothing Then defaultDelegateate()

			If stubDelegate IsNot Nothing Then stubDelegate.connect(Me, orb)

		End Sub

		''' <summary>
		''' Serialization method to restore the IOR state.
		''' </summary>
		Private Sub readObject(ByVal stream As java.io.ObjectInputStream)

			If stubDelegate Is Nothing Then defaultDelegateate()

			If stubDelegate IsNot Nothing Then stubDelegate.readObject(Me, stream)

		End Sub

		''' <summary>
		''' Serialization method to save the IOR state.
		''' @serialData The length of the IOR type ID (int), followed by the IOR type ID
		''' (byte array encoded using ISO8859-1), followed by the number of IOR profiles
		''' (int), followed by the IOR profiles.  Each IOR profile is written as a
		''' profile tag (int), followed by the length of the profile data (int), followed
		''' by the profile data (byte array).
		''' </summary>
		Private Sub writeObject(ByVal stream As java.io.ObjectOutputStream)

			If stubDelegate Is Nothing Then defaultDelegateate()

			If stubDelegate IsNot Nothing Then stubDelegate.writeObject(Me, stream)
		End Sub

		Private Sub setDefaultDelegate()
			If stubDelegateClass IsNot Nothing Then
				Try
					 stubDelegate = CType(stubDelegateClass.newInstance(), javax.rmi.CORBA.StubDelegate)
				Catch ex As Exception
				' what kind of exception to throw
				' delegate not set therefore it is null and will return default
				' values
				End Try
			End If
		End Sub

		' Same code as in PortableRemoteObject. Can not be shared because they
		' are in different packages and the visibility needs to be package for
		' security reasons. If you know a better solution how to share this code
		' then remove it from PortableRemoteObject. Also in Util.java
		Private Shared Function createDelegate(ByVal classKey As String) As Object
			Dim className As String = CStr(java.security.AccessController.doPrivileged(New com.sun.corba.se.impl.orbutil.GetPropertyAction(classKey)))
			If className Is Nothing Then
				Dim props As java.util.Properties = oRBPropertiesFile
				If props IsNot Nothing Then className = props.getProperty(classKey)
			End If

			If className Is Nothing Then Return New com.sun.corba.se.impl.javax.rmi.CORBA.StubDelegateImpl

			Try
				Return loadDelegateClass(className).newInstance()
			Catch ex As ClassNotFoundException
				Dim exc As New org.omg.CORBA.INITIALIZE("Cannot instantiate " & className)
				exc.initCause(ex)
				Throw exc
			Catch ex As Exception
				Dim exc As New org.omg.CORBA.INITIALIZE("Error while instantiating" & className)
				exc.initCause(ex)
				Throw exc
			End Try

		End Function

		Private Shared Function loadDelegateClass(ByVal className As String) As Type
			Try
				Dim loader As ClassLoader = Thread.CurrentThread.contextClassLoader
				Return Type.GetType(className, False, loader)
			Catch e As ClassNotFoundException
				' ignore, then try RMIClassLoader
			End Try

			Try
				Return java.rmi.server.RMIClassLoader.loadClass(className)
			Catch e As java.net.MalformedURLException
				Dim msg As String = "Could not load " & className & ": " & e.ToString()
				Dim exc As New ClassNotFoundException(msg)
				Throw exc
			End Try
		End Function

		''' <summary>
		''' Load the orb.properties file.
		''' </summary>
		Private Property Shared oRBPropertiesFile As java.util.Properties
			Get
				Return CType(java.security.AccessController.doPrivileged(New GetORBPropertiesFileAction), java.util.Properties)
			End Get
		End Property

	End Class

End Namespace