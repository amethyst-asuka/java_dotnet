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

Namespace javax.rmi





	''' <summary>
	''' Server implementation objects may either inherit from
	''' javax.rmi.PortableRemoteObject or they may implement a remote interface
	''' and then use the exportObject method to register themselves as a server object.
	''' The toStub method takes a server implementation and returns a stub that
	''' can be used to access that server object.
	''' The connect method makes a Remote object ready for remote communication.
	''' The unexportObject method is used to deregister a server object, allowing it to become
	''' available for garbage collection.
	''' The narrow method takes an object reference or abstract interface type and
	''' attempts to narrow it to conform to
	''' the given interface. If the operation is successful the result will be an
	''' object of the specified type, otherwise an exception will be thrown.
	''' </summary>
	Public Class PortableRemoteObject

		Private Shared ReadOnly proDelegate As javax.rmi.CORBA.PortableRemoteObjectDelegate

		Private Const PortableRemoteObjectClassKey As String = "javax.rmi.CORBA.PortableRemoteObjectClass"

		Shared Sub New()
			proDelegate = CType(createDelegate(PortableRemoteObjectClassKey), javax.rmi.CORBA.PortableRemoteObjectDelegate)
		End Sub

		''' <summary>
		''' Initializes the object by calling <code>exportObject(this)</code>. </summary>
		''' <exception cref="RemoteException"> if export fails. </exception>
		Protected Friend Sub New()
			If proDelegate IsNot Nothing Then PortableRemoteObject.exportObject(CType(Me, java.rmi.Remote))
		End Sub

		''' <summary>
		''' Makes a server object ready to receive remote calls. Note
		''' that subclasses of PortableRemoteObject do not need to call this
		''' method, as it is called by the constructor. </summary>
		''' <param name="obj"> the server object to export. </param>
		''' <exception cref="RemoteException"> if export fails. </exception>
		Public Shared Sub exportObject(ByVal obj As java.rmi.Remote)

			' Let the delegate do everything, including error handling.
			If proDelegate IsNot Nothing Then proDelegate.exportObject(obj)
		End Sub

		''' <summary>
		''' Returns a stub for the given server object. </summary>
		''' <param name="obj"> the server object for which a stub is required. Must either be a subclass
		''' of PortableRemoteObject or have been previously the target of a call to
		''' <seealso cref="#exportObject"/>. </param>
		''' <returns> the most derived stub for the object. </returns>
		''' <exception cref="NoSuchObjectException"> if a stub cannot be located for the given server object. </exception>
		Public Shared Function toStub(ByVal obj As java.rmi.Remote) As java.rmi.Remote

			If proDelegate IsNot Nothing Then Return proDelegate.toStub(obj)
			Return Nothing
		End Function

		''' <summary>
		''' Deregisters a server object from the runtime, allowing the object to become
		''' available for garbage collection. </summary>
		''' <param name="obj"> the object to unexport. </param>
		''' <exception cref="NoSuchObjectException"> if the remote object is not
		''' currently exported. </exception>
		Public Shared Sub unexportObject(ByVal obj As java.rmi.Remote)

			If proDelegate IsNot Nothing Then proDelegate.unexportObject(obj)

		End Sub

		''' <summary>
		''' Checks to ensure that an object of a remote or abstract interface type
		''' can be cast to a desired type. </summary>
		''' <param name="narrowFrom"> the object to check. </param>
		''' <param name="narrowTo"> the desired type. </param>
		''' <returns> an object which can be cast to the desired type. </returns>
		''' <exception cref="ClassCastException"> if narrowFrom cannot be cast to narrowTo. </exception>
		Public Shared Function narrow(ByVal narrowFrom As Object, ByVal narrowTo As Type) As Object

			If proDelegate IsNot Nothing Then Return proDelegate.narrow(narrowFrom, narrowTo)
			Return Nothing

		End Function

		''' <summary>
		''' Makes a Remote object ready for remote communication. This normally
		''' happens implicitly when the object is sent or received as an argument
		''' on a remote method call, but in some circumstances it is useful to
		''' perform this action by making an explicit call.  See the
		''' <seealso cref="javax.rmi.CORBA.Stub#connect"/> method for more information. </summary>
		''' <param name="target"> the object to connect. </param>
		''' <param name="source"> a previously connected object. </param>
		''' <exception cref="RemoteException"> if <code>source</code> is not connected
		''' or if <code>target</code> is already connected to a different ORB than
		''' <code>source</code>. </exception>
		Public Shared Sub connect(ByVal target As java.rmi.Remote, ByVal source As java.rmi.Remote)

			If proDelegate IsNot Nothing Then proDelegate.connect(target, source)

		End Sub

		' Same code as in javax.rmi.CORBA.Util. Can not be shared because they
		' are in different packages and the visibility needs to be package for
		' security reasons. If you know a better solution how to share this code
		' then remove it from here.
		Private Shared Function createDelegate(ByVal classKey As String) As Object
			Dim className As String = CStr(java.security.AccessController.doPrivileged(New com.sun.corba.se.impl.orbutil.GetPropertyAction(classKey)))
			If className Is Nothing Then
				Dim props As java.util.Properties = oRBPropertiesFile
				If props IsNot Nothing Then className = props.getProperty(classKey)
			End If
			If className Is Nothing Then Return New com.sun.corba.se.impl.javax.rmi.PortableRemoteObject

			Try
				Return CObj(loadDelegateClass(className).newInstance())
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

	Friend Class GetORBPropertiesFileAction
		Implements java.security.PrivilegedAction

		Private debug As Boolean = False

		Public Sub New()
		End Sub

		Private Function getSystemProperty(ByVal name As String) As String
			' This will not throw a SecurityException because this
			' class was loaded from rt.jar using the bootstrap classloader.
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			String propValue = (String) java.security.AccessController.doPrivileged(New java.security.PrivilegedAction()
	'		{
	'				public java.lang.Object run()
	'				{
	'					Return System.getProperty(name);
	'				}
	'			}
		   )

			Return propValue
		End Function

		Private Sub getPropertiesFromFile(ByVal props As java.util.Properties, ByVal fileName As String)
			Try
				Dim file As New File(fileName)
				If Not file.exists() Then Return

				Dim [in] As New java.io.FileInputStream(file)

				Try
					props.load([in])
				Finally
					[in].close()
				End Try
			Catch exc As Exception
				If debug Then Console.WriteLine("ORB properties file " & fileName & " not found: " & exc)
			End Try
		End Sub

		Public Overridable Function run() As Object
			Dim defaults As New java.util.Properties

			Dim javaHome As String = getSystemProperty("java.home")
			Dim fileName As String = javaHome + File.separator & "lib" & File.separator & "orb.properties"

			getPropertiesFromFile(defaults, fileName)

			Dim results As New java.util.Properties(defaults)

			Dim userHome As String = getSystemProperty("user.home")
			fileName = userHome + File.separator & "orb.properties"

			getPropertiesFromFile(results, fileName)
			Return results
		End Function
	End Class

End Namespace