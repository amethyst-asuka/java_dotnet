Imports System

'
' * Copyright (c) 1996, 2004, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.rmi.server


	''' <summary>
	''' <code>RemoteRef</code> represents the handle for a remote object. A
	''' <code>RemoteStub</code> uses a remote reference to carry out a
	''' remote method invocation to a remote object.
	''' 
	''' @author  Ann Wollrath
	''' @since   JDK1.1 </summary>
	''' <seealso cref=     java.rmi.server.RemoteStub </seealso>
	Public Interface RemoteRef
		Inherits java.io.Externalizable

		''' <summary>
		''' indicate compatibility with JDK 1.1.x version of class. </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final long serialVersionUID = 3632638527362204081L;

		''' <summary>
		''' Initialize the server package prefix: assumes that the
		''' implementation of server ref classes (e.g., UnicastRef,
		''' UnicastServerRef) are located in the package defined by the
		''' prefix.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		final static String packagePrefix = "sun.rmi.server";

		''' <summary>
		''' Invoke a method. This form of delegating method invocation
		''' to the reference allows the reference to take care of
		''' setting up the connection to the remote host, marshaling
		''' some representation for the method and parameters, then
		''' communicating the method invocation to the remote host.
		''' This method either returns the result of a method invocation
		''' on the remote object which resides on the remote host or
		''' throws a RemoteException if the call failed or an
		''' application-level exception if the remote invocation throws
		''' an exception.
		''' </summary>
		''' <param name="obj"> the object that contains the RemoteRef (e.g., the
		'''            RemoteStub for the object. </param>
		''' <param name="method"> the method to be invoked </param>
		''' <param name="params"> the parameter list </param>
		''' <param name="opnum">  a hash that may be used to represent the method </param>
		''' <returns> result of remote method invocation </returns>
		''' <exception cref="Exception"> if any exception occurs during remote method
		''' invocation
		''' @since 1.2 </exception>
		Function invoke(ByVal obj As Remote, ByVal method As System.Reflection.MethodInfo, ByVal params As Object(), ByVal opnum As Long) As Object

		''' <summary>
		''' Creates an appropriate call object for a new remote method
		''' invocation on this object.  Passing operation array and index,
		''' allows the stubs generator to assign the operation indexes and
		''' interpret them. The remote reference may need the operation to
		''' encode in the call.
		''' 
		''' @since JDK1.1 </summary>
		''' @deprecated 1.2 style stubs no longer use this method. Instead of
		''' using a sequence of method calls on the stub's the remote reference
		''' (<code>newCall</code>, <code>invoke</code>, and <code>done</code>), a
		''' stub uses a single method, <code>invoke(Remote, Method, Object[],
		''' int)</code>, on the remote reference to carry out parameter
		''' marshalling, remote method executing and unmarshalling of the return
		''' value.
		''' 
		''' <param name="obj"> remote stub through which to make call </param>
		''' <param name="op"> array of stub operations </param>
		''' <param name="opnum"> operation number </param>
		''' <param name="hash"> stub/skeleton interface hash </param>
		''' <returns> call object representing remote call </returns>
		''' <exception cref="RemoteException"> if failed to initiate new remote call </exception>
		''' <seealso cref= #invoke(Remote,java.lang.reflect.Method,Object[],long) </seealso>
		<Obsolete("1.2 style stubs no longer use this method. Instead of")> _
		Function newCall(ByVal obj As RemoteObject, ByVal op As Operation(), ByVal opnum As Integer, ByVal hash As Long) As RemoteCall

		''' <summary>
		''' Executes the remote call.
		''' 
		''' Invoke will raise any "user" exceptions which
		''' should pass through and not be caught by the stub.  If any
		''' exception is raised during the remote invocation, invoke should
		''' take care of cleaning up the connection before raising the
		''' "user" or remote exception.
		''' 
		''' @since JDK1.1 </summary>
		''' @deprecated 1.2 style stubs no longer use this method. Instead of
		''' using a sequence of method calls to the remote reference
		''' (<code>newCall</code>, <code>invoke</code>, and <code>done</code>), a
		''' stub uses a single method, <code>invoke(Remote, Method, Object[],
		''' int)</code>, on the remote reference to carry out parameter
		''' marshalling, remote method executing and unmarshalling of the return
		''' value.
		''' 
		''' <param name="call"> object representing remote call </param>
		''' <exception cref="Exception"> if any exception occurs during remote method </exception>
		''' <seealso cref= #invoke(Remote,java.lang.reflect.Method,Object[],long) </seealso>
		<Obsolete("1.2 style stubs no longer use this method. Instead of")> _
		Sub invoke(ByVal [call] As RemoteCall)

		''' <summary>
		''' Allows the remote reference to clean up (or reuse) the connection.
		''' Done should only be called if the invoke returns successfully
		''' (non-exceptionally) to the stub.
		''' 
		''' @since JDK1.1 </summary>
		''' @deprecated 1.2 style stubs no longer use this method. Instead of
		''' using a sequence of method calls to the remote reference
		''' (<code>newCall</code>, <code>invoke</code>, and <code>done</code>), a
		''' stub uses a single method, <code>invoke(Remote, Method, Object[],
		''' int)</code>, on the remote reference to carry out parameter
		''' marshalling, remote method executing and unmarshalling of the return
		''' value.
		''' 
		''' <param name="call"> object representing remote call </param>
		''' <exception cref="RemoteException"> if remote error occurs during call cleanup </exception>
		''' <seealso cref= #invoke(Remote,java.lang.reflect.Method,Object[],long) </seealso>
		<Obsolete("1.2 style stubs no longer use this method. Instead of")> _
		Sub done(ByVal [call] As RemoteCall)

		''' <summary>
		''' Returns the class name of the ref type to be serialized onto
		''' the stream 'out'. </summary>
		''' <param name="out"> the output stream to which the reference will be serialized </param>
		''' <returns> the class name (without package qualification) of the reference
		''' type
		''' @since JDK1.1 </returns>
		Function getRefClass(ByVal out As java.io.ObjectOutput) As String

		''' <summary>
		''' Returns a hashcode for a remote object.  Two remote object stubs
		''' that refer to the same remote object will have the same hash code
		''' (in order to support remote objects as keys in hash tables).
		''' </summary>
		''' <returns> remote object hashcode </returns>
		''' <seealso cref=             java.util.Hashtable
		''' @since JDK1.1 </seealso>
		Function remoteHashCode() As Integer

		''' <summary>
		''' Compares two remote objects for equality.
		''' Returns a boolean that indicates whether this remote object is
		''' equivalent to the specified Object. This method is used when a
		''' remote object is stored in a hashtable. </summary>
		''' <param name="obj">     the Object to compare with </param>
		''' <returns>  true if these Objects are equal; false otherwise. </returns>
		''' <seealso cref=             java.util.Hashtable
		''' @since JDK1.1 </seealso>
		Function remoteEquals(ByVal obj As RemoteRef) As Boolean

		''' <summary>
		''' Returns a String that represents the reference of this remote
		''' object. </summary>
		''' <returns> string representing remote object reference
		''' @since JDK1.1 </returns>
		Function remoteToString() As String

	End Interface

End Namespace