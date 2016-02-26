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
	''' Provides utility methods that can be used by stubs and ties to
	''' perform common operations.
	''' </summary>
	Public Class Util

		' This can only be set at static initialization time (no sync necessary).
		Private Shared ReadOnly utilDelegate As javax.rmi.CORBA.UtilDelegate
		Private Const UtilClassKey As String = "javax.rmi.CORBA.UtilClass"

		Shared Sub New()
			utilDelegate = CType(createDelegate(UtilClassKey), javax.rmi.CORBA.UtilDelegate)
		End Sub

		Private Sub New()
		End Sub

		''' <summary>
		''' Maps a SystemException to a RemoteException. </summary>
		''' <param name="ex"> the SystemException to map. </param>
		''' <returns> the mapped exception. </returns>
		Public Shared Function mapSystemException(ByVal ex As org.omg.CORBA.SystemException) As java.rmi.RemoteException

			If utilDelegate IsNot Nothing Then Return utilDelegate.mapSystemException(ex)
			Return Nothing
		End Function

		''' <summary>
		''' Writes any java.lang.Object as a CORBA any. </summary>
		''' <param name="out"> the stream in which to write the any. </param>
		''' <param name="obj"> the object to write as an any. </param>
		Public Shared Sub writeAny(ByVal out As org.omg.CORBA.portable.OutputStream, ByVal obj As Object)

			If utilDelegate IsNot Nothing Then utilDelegate.writeAny(out, obj)
		End Sub

		''' <summary>
		''' Reads a java.lang.Object as a CORBA any. </summary>
		''' <param name="in"> the stream from which to read the any. </param>
		''' <returns> the object read from the stream. </returns>
		Public Shared Function readAny(ByVal [in] As org.omg.CORBA.portable.InputStream) As Object

			If utilDelegate IsNot Nothing Then Return utilDelegate.readAny([in])
			Return Nothing
		End Function

		''' <summary>
		''' Writes a java.lang.Object as a CORBA Object. If <code>obj</code> is
		''' an exported RMI-IIOP server object, the tie is found
		''' and wired to <code>obj</code>, then written to
		''' <code>out.write_Object(org.omg.CORBA.Object)</code>.
		''' If <code>obj</code> is a CORBA Object, it is written to
		''' <code>out.write_Object(org.omg.CORBA.Object)</code>. </summary>
		''' <param name="out"> the stream in which to write the object. </param>
		''' <param name="obj"> the object to write. </param>
		Public Shared Sub writeRemoteObject(ByVal out As org.omg.CORBA.portable.OutputStream, ByVal obj As Object)

			If utilDelegate IsNot Nothing Then utilDelegate.writeRemoteObject(out, obj)

		End Sub

		''' <summary>
		''' Writes a java.lang.Object as either a value or a CORBA Object.
		''' If <code>obj</code> is a value object or a stub object, it is written to
		''' <code>out.write_abstract_interface(java.lang.Object)</code>. If <code>obj</code>
		''' is
		''' an exported
		''' RMI-IIOP server object, the tie is found and wired to <code>obj</code>,
		''' then written to <code>out.write_abstract_interface(java.lang.Object)</code>. </summary>
		''' <param name="out"> the stream in which to write the object. </param>
		''' <param name="obj"> the object to write. </param>
		Public Shared Sub writeAbstractObject(ByVal out As org.omg.CORBA.portable.OutputStream, ByVal obj As Object)

			If utilDelegate IsNot Nothing Then utilDelegate.writeAbstractObject(out, obj)
		End Sub

		''' <summary>
		''' Registers a target for a tie. Adds the tie to an internal table and calls
		''' <seealso cref="Tie#setTarget"/> on the tie object. </summary>
		''' <param name="tie"> the tie to register. </param>
		''' <param name="target"> the target for the tie. </param>
		Public Shared Sub registerTarget(ByVal tie As javax.rmi.CORBA.Tie, ByVal target As java.rmi.Remote)

			If utilDelegate IsNot Nothing Then utilDelegate.registerTarget(tie, target)

		End Sub

		''' <summary>
		''' Removes the associated tie from an internal table and calls {@link
		''' Tie#deactivate}
		''' to deactivate the object. </summary>
		''' <param name="target"> the object to unexport. </param>
		Public Shared Sub unexportObject(ByVal target As java.rmi.Remote)

			If utilDelegate IsNot Nothing Then utilDelegate.unexportObject(target)

		End Sub

		''' <summary>
		''' Returns the tie (if any) for a given target object. </summary>
		''' <returns> the tie or null if no tie is registered for the given target. </returns>
		Public Shared Function getTie(ByVal target As java.rmi.Remote) As javax.rmi.CORBA.Tie

			If utilDelegate IsNot Nothing Then Return utilDelegate.getTie(target)
			Return Nothing
		End Function


		''' <summary>
		''' Returns a singleton instance of a class that implements the
		''' <seealso cref="ValueHandler"/> interface. </summary>
		''' <returns> a class which implements the ValueHandler interface. </returns>
		Public Shared Function createValueHandler() As ValueHandler

			If utilDelegate IsNot Nothing Then Return utilDelegate.createValueHandler()
			Return Nothing
		End Function

		''' <summary>
		''' Returns the codebase, if any, for the given class. </summary>
		''' <param name="clz"> the class to get a codebase for. </param>
		''' <returns> a space-separated list of URLs, or null. </returns>
		Public Shared Function getCodebase(ByVal clz As Type) As String
			If utilDelegate IsNot Nothing Then Return utilDelegate.getCodebase(clz)
			Return Nothing
		End Function

		''' <summary>
		''' Returns a class instance for the specified class.
		''' <P>The spec for this method is the "Java to IDL language
		''' mapping", ptc/00-01-06.
		''' <P>In Java SE Platform, this method works as follows:
		''' <UL><LI>Find the first non-null <tt>ClassLoader</tt> on the
		''' call stack and attempt to load the class using this
		''' <tt>ClassLoader</tt>.
		''' <LI>If the first step fails, and if <tt>remoteCodebase</tt>
		''' is non-null and
		''' <tt>useCodebaseOnly</tt> is false, then call
		''' <tt>java.rmi.server.RMIClassLoader.loadClass(remoteCodebase, className)</tt>.
		''' <LI>If <tt>remoteCodebase</tt> is null or <tt>useCodebaseOnly</tt>
		''' is true, then call <tt>java.rmi.server.RMIClassLoader.loadClass(className)</tt>.
		''' <LI>If a class was not successfully loaded by step 1, 2, or 3,
		''' and <tt>loader</tt> is non-null, then call <tt>loader.loadClass(className)</tt>.
		''' <LI>If a class was successfully loaded by step 1, 2, 3, or 4, then
		'''  return the loaded class, else throw <tt>ClassNotFoundException</tt>. </summary>
		''' <param name="className"> the name of the class. </param>
		''' <param name="remoteCodebase"> a space-separated list of URLs at which
		''' the class might be found. May be null. </param>
		''' <param name="loader"> a <tt>ClassLoader</tt> that may be used to
		''' load the class if all other methods fail. </param>
		''' <returns> the <code>Class</code> object representing the loaded class. </returns>
		''' <exception cref="ClassNotFoundException"> if class cannot be loaded. </exception>
		Public Shared Function loadClass(ByVal className As String, ByVal remoteCodebase As String, ByVal loader As ClassLoader) As Type
			If utilDelegate IsNot Nothing Then Return utilDelegate.loadClass(className,remoteCodebase,loader)
			Return Nothing
		End Function


		''' <summary>
		''' The <tt>isLocal</tt> method has the same semantics as the
		''' <tt>ObjectImpl._is_local</tt>
		''' method, except that it can throw a <tt>RemoteException</tt>.
		''' 
		''' The <tt>_is_local()</tt> method is provided so that stubs may determine if a
		''' particular object is implemented by a local servant and hence local
		''' invocation APIs may be used.
		''' </summary>
		''' <param name="stub"> the stub to test.
		''' </param>
		''' <returns> The <tt>_is_local()</tt> method returns true if
		''' the servant incarnating the object is located in the same process as
		''' the stub and they both share the same ORB instance.  The <tt>_is_local()</tt>
		''' method returns false otherwise. The default behavior of <tt>_is_local()</tt> is
		''' to return false.
		''' </returns>
		''' <exception cref="RemoteException"> The Java to IDL specification does not
		''' specify the conditions that cause a <tt>RemoteException</tt> to be thrown. </exception>
		Public Shared Function isLocal(ByVal ___stub As Stub) As Boolean

			If utilDelegate IsNot Nothing Then Return utilDelegate.isLocal(___stub)

			Return False
		End Function

		''' <summary>
		''' Wraps an exception thrown by an implementation
		''' method.  It returns the corresponding client-side exception. </summary>
		''' <param name="orig"> the exception to wrap. </param>
		''' <returns> the wrapped exception. </returns>
		Public Shared Function wrapException(ByVal orig As Exception) As java.rmi.RemoteException

			If utilDelegate IsNot Nothing Then Return utilDelegate.wrapException(orig)

			Return Nothing
		End Function

		''' <summary>
		''' Copies or connects an array of objects. Used by local stubs
		''' to copy any number of actual parameters, preserving sharing
		''' across parameters as necessary to support RMI semantics. </summary>
		''' <param name="obj"> the objects to copy or connect. </param>
		''' <param name="orb"> the ORB. </param>
		''' <returns> the copied or connected objects. </returns>
		''' <exception cref="RemoteException"> if any object could not be copied or connected. </exception>
		Public Shared Function copyObjects(ByVal obj As Object(), ByVal orb As org.omg.CORBA.ORB) As Object()

			If utilDelegate IsNot Nothing Then Return utilDelegate.copyObjects(obj, orb)

			Return Nothing
		End Function

		''' <summary>
		''' Copies or connects an object. Used by local stubs to copy
		''' an actual parameter, result object, or exception. </summary>
		''' <param name="obj"> the object to copy. </param>
		''' <param name="orb"> the ORB. </param>
		''' <returns> the copy or connected object. </returns>
		''' <exception cref="RemoteException"> if the object could not be copied or connected. </exception>
		Public Shared Function copyObject(ByVal obj As Object, ByVal orb As org.omg.CORBA.ORB) As Object

			If utilDelegate IsNot Nothing Then Return utilDelegate.copyObject(obj, orb)
			Return Nothing
		End Function

		' Same code as in PortableRemoteObject. Can not be shared because they
		' are in different packages and the visibility needs to be package for
		' security reasons. If you know a better solution how to share this code
		' then remove it from PortableRemoteObject. Also in Stub.java
		Private Shared Function createDelegate(ByVal classKey As String) As Object
			Dim className As String = CStr(java.security.AccessController.doPrivileged(New com.sun.corba.se.impl.orbutil.GetPropertyAction(classKey)))
			If className Is Nothing Then
				Dim props As java.util.Properties = oRBPropertiesFile
				If props IsNot Nothing Then className = props.getProperty(classKey)
			End If

			If className Is Nothing Then Return New com.sun.corba.se.impl.javax.rmi.CORBA.Util

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