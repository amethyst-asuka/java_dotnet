Imports System
Imports System.Threading
Imports System.Runtime.InteropServices

'
' * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang



	''' <summary>
	''' The security manager is a class that allows
	''' applications to implement a security policy. It allows an
	''' application to determine, before performing a possibly unsafe or
	''' sensitive operation, what the operation is and whether
	''' it is being attempted in a security context that allows the
	''' operation to be performed. The
	''' application can allow or disallow the operation.
	''' <p>
	''' The <code>SecurityManager</code> class contains many methods with
	''' names that begin with the word <code>check</code>. These methods
	''' are called by various methods in the Java libraries before those
	''' methods perform certain potentially sensitive operations. The
	''' invocation of such a <code>check</code> method typically looks like this:
	''' <blockquote><pre>
	'''     SecurityManager security = System.getSecurityManager();
	'''     if (security != null) {
	'''         security.check<i>XXX</i>(argument, &nbsp;.&nbsp;.&nbsp;.&nbsp;);
	'''     }
	''' </pre></blockquote>
	''' <p>
	''' The security manager is thereby given an opportunity to prevent
	''' completion of the operation by throwing an exception. A security
	''' manager routine simply returns if the operation is permitted, but
	''' throws a <code>SecurityException</code> if the operation is not
	''' permitted. The only exception to this convention is
	''' <code>checkTopLevelWindow</code>, which returns a
	''' <code>boolean</code> value.
	''' <p>
	''' The current security manager is set by the
	''' <code>setSecurityManager</code> method in class
	''' <code>System</code>. The current security manager is obtained
	''' by the <code>getSecurityManager</code> method.
	''' <p>
	''' The special method
	''' <seealso cref="SecurityManager#checkPermission(java.security.Permission)"/>
	''' determines whether an access request indicated by a specified
	''' permission should be granted or denied. The
	''' default implementation calls
	''' 
	''' <pre>
	'''   AccessController.checkPermission(perm);
	''' </pre>
	''' 
	''' <p>
	''' If a requested access is allowed,
	''' <code>checkPermission</code> returns quietly. If denied, a
	''' <code>SecurityException</code> is thrown.
	''' <p>
	''' As of Java 2 SDK v1.2, the default implementation of each of the other
	''' <code>check</code> methods in <code>SecurityManager</code> is to
	''' call the <code>SecurityManager checkPermission</code> method
	''' to determine if the calling thread has permission to perform the requested
	''' operation.
	''' <p>
	''' Note that the <code>checkPermission</code> method with
	''' just a single permission argument always performs security checks
	''' within the context of the currently executing thread.
	''' Sometimes a security check that should be made within a given context
	''' will actually need to be done from within a
	''' <i>different</i> context (for example, from within a worker thread).
	''' The <seealso cref="SecurityManager#getSecurityContext getSecurityContext"/> method
	''' and the {@link SecurityManager#checkPermission(java.security.Permission,
	''' java.lang.Object) checkPermission}
	''' method that includes a context argument are provided
	''' for this situation. The
	''' <code>getSecurityContext</code> method returns a "snapshot"
	''' of the current calling context. (The default implementation
	''' returns an AccessControlContext object.) A sample call is
	''' the following:
	''' 
	''' <pre>
	'''   Object context = null;
	'''   SecurityManager sm = System.getSecurityManager();
	'''   if (sm != null) context = sm.getSecurityContext();
	''' </pre>
	''' 
	''' <p>
	''' The <code>checkPermission</code> method
	''' that takes a context object in addition to a permission
	''' makes access decisions based on that context,
	''' rather than on that of the current execution thread.
	''' Code within a different context can thus call that method,
	''' passing the permission and the
	''' previously-saved context object. A sample call, using the
	''' SecurityManager <code>sm</code> obtained as in the previous example,
	''' is the following:
	''' 
	''' <pre>
	'''   if (sm != null) sm.checkPermission(permission, context);
	''' </pre>
	''' 
	''' <p>Permissions fall into these categories: File, Socket, Net,
	''' Security, Runtime, Property, AWT, Reflect, and Serializable.
	''' The classes managing these various
	''' permission categories are <code>java.io.FilePermission</code>,
	''' <code>java.net.SocketPermission</code>,
	''' <code>java.net.NetPermission</code>,
	''' <code>java.security.SecurityPermission</code>,
	''' <code>java.lang.RuntimePermission</code>,
	''' <code>java.util.PropertyPermission</code>,
	''' <code>java.awt.AWTPermission</code>,
	''' <code>java.lang.reflect.ReflectPermission</code>, and
	''' <code>java.io.SerializablePermission</code>.
	''' 
	''' <p>All but the first two (FilePermission and SocketPermission) are
	''' subclasses of <code>java.security.BasicPermission</code>, which itself
	''' is an abstract subclass of the
	''' top-level class for permissions, which is
	''' <code>java.security.Permission</code>. BasicPermission defines the
	''' functionality needed for all permissions that contain a name
	''' that follows the hierarchical property naming convention
	''' (for example, "exitVM", "setFactory", "queuePrintJob", etc).
	''' An asterisk
	''' may appear at the end of the name, following a ".", or by itself, to
	''' signify a wildcard match. For example: "a.*" or "*" is valid,
	''' "*a" or "a*b" is not valid.
	''' 
	''' <p>FilePermission and SocketPermission are subclasses of the
	''' top-level class for permissions
	''' (<code>java.security.Permission</code>). Classes like these
	''' that have a more complicated name syntax than that used by
	''' BasicPermission subclass directly from Permission rather than from
	''' BasicPermission. For example,
	''' for a <code>java.io.FilePermission</code> object, the permission name is
	''' the path name of a file (or directory).
	''' 
	''' <p>Some of the permission classes have an "actions" list that tells
	''' the actions that are permitted for the object.  For example,
	''' for a <code>java.io.FilePermission</code> object, the actions list
	''' (such as "read, write") specifies which actions are granted for the
	''' specified file (or for files in the specified directory).
	''' 
	''' <p>Other permission classes are for "named" permissions -
	''' ones that contain a name but no actions list; you either have the
	''' named permission or you don't.
	''' 
	''' <p>Note: There is also a <code>java.security.AllPermission</code>
	''' permission that implies all permissions. It exists to simplify the work
	''' of system administrators who might need to perform multiple
	''' tasks that require all (or numerous) permissions.
	''' <p>
	''' See <a href ="../../../technotes/guides/security/permissions.html">
	''' Permissions in the JDK</a> for permission-related information.
	''' This document includes, for example, a table listing the various SecurityManager
	''' <code>check</code> methods and the permission(s) the default
	''' implementation of each such method requires.
	''' It also contains a table of all the version 1.2 methods
	''' that require permissions, and for each such method tells
	''' which permission it requires.
	''' <p>
	''' For more information about <code>SecurityManager</code> changes made in
	''' the JDK and advice regarding porting of 1.1-style security managers,
	''' see the <a href="../../../technotes/guides/security/index.html">security documentation</a>.
	''' 
	''' @author  Arthur van Hoff
	''' @author  Roland Schemers
	''' </summary>
	''' <seealso cref=     java.lang.ClassLoader </seealso>
	''' <seealso cref=     java.lang.SecurityException </seealso>
	''' <seealso cref=     java.lang.SecurityManager#checkTopLevelWindow(java.lang.Object)
	'''  checkTopLevelWindow </seealso>
	''' <seealso cref=     java.lang.System#getSecurityManager() getSecurityManager </seealso>
	''' <seealso cref=     java.lang.System#setSecurityManager(java.lang.SecurityManager)
	'''  setSecurityManager </seealso>
	''' <seealso cref=     java.security.AccessController AccessController </seealso>
	''' <seealso cref=     java.security.AccessControlContext AccessControlContext </seealso>
	''' <seealso cref=     java.security.AccessControlException AccessControlException </seealso>
	''' <seealso cref=     java.security.Permission </seealso>
	''' <seealso cref=     java.security.BasicPermission </seealso>
	''' <seealso cref=     java.io.FilePermission </seealso>
	''' <seealso cref=     java.net.SocketPermission </seealso>
	''' <seealso cref=     java.util.PropertyPermission </seealso>
	''' <seealso cref=     java.lang.RuntimePermission </seealso>
	''' <seealso cref=     java.awt.AWTPermission </seealso>
	''' <seealso cref=     java.security.Policy Policy </seealso>
	''' <seealso cref=     java.security.SecurityPermission SecurityPermission </seealso>
	''' <seealso cref=     java.security.ProtectionDomain
	''' 
	''' @since   JDK1.0 </seealso>
	Public Class SecurityManager

		''' <summary>
		''' This field is <code>true</code> if there is a security check in
		''' progress; <code>false</code> otherwise.
		''' </summary>
		''' @deprecated This type of security checking is not recommended.
		'''  It is recommended that the <code>checkPermission</code>
		'''  call be used instead. 
		<Obsolete("This type of security checking is not recommended.")> _
		Protected Friend inCheck As Boolean

	'    
	'     * Have we been initialized. Effective against finalizer attacks.
	'     
		Private initialized As Boolean = False


		''' <summary>
		''' returns true if the current context has been granted AllPermission
		''' </summary>
		Private Function hasAllPermission() As Boolean
			Try
				checkPermission(sun.security.util.SecurityConstants.ALL_PERMISSION)
				Return True
			Catch se As SecurityException
				Return False
			End Try
		End Function

		''' <summary>
		''' Tests if there is a security check in progress.
		''' </summary>
		''' <returns> the value of the <code>inCheck</code> field. This field
		'''          should contain <code>true</code> if a security check is
		'''          in progress,
		'''          <code>false</code> otherwise. </returns>
		''' <seealso cref=     java.lang.SecurityManager#inCheck </seealso>
		''' @deprecated This type of security checking is not recommended.
		'''  It is recommended that the <code>checkPermission</code>
		'''  call be used instead. 
		<Obsolete("This type of security checking is not recommended.")> _
		Public Overridable Property inCheck As Boolean
			Get
				Return inCheck
			End Get
		End Property

		''' <summary>
		''' Constructs a new <code>SecurityManager</code>.
		''' 
		''' <p> If there is a security manager already installed, this method first
		''' calls the security manager's <code>checkPermission</code> method
		''' with the <code>RuntimePermission("createSecurityManager")</code>
		''' permission to ensure the calling thread has permission to create a new
		''' security manager.
		''' This may result in throwing a <code>SecurityException</code>.
		''' </summary>
		''' <exception cref="java.lang.SecurityException"> if a security manager already
		'''             exists and its <code>checkPermission</code> method
		'''             doesn't allow creation of a new security manager. </exception>
		''' <seealso cref=        java.lang.System#getSecurityManager() </seealso>
		''' <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		''' <seealso cref= java.lang.RuntimePermission </seealso>
		Public Sub New()
			SyncLock GetType(SecurityManager)
				Dim sm As SecurityManager = System.securityManager
				If sm IsNot Nothing Then sm.checkPermission(New RuntimePermission("createSecurityManager"))
				initialized = True
			End SyncLock
		End Sub

		''' <summary>
		''' Returns the current execution stack as an array of classes.
		''' <p>
		''' The length of the array is the number of methods on the execution
		''' stack. The element at index <code>0</code> is the class of the
		''' currently executing method, the element at index <code>1</code> is
		''' the class of that method's caller, and so on.
		''' </summary>
		''' <returns>  the execution stack. </returns>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Protected Friend Function getClassContext() As Class()
		End Function

		''' <summary>
		''' Returns the class loader of the most recently executing method from
		''' a class defined using a non-system class loader. A non-system
		''' class loader is defined as being a class loader that is not equal to
		''' the system class loader (as returned
		''' by <seealso cref="ClassLoader#getSystemClassLoader"/>) or one of its ancestors.
		''' <p>
		''' This method will return
		''' <code>null</code> in the following three cases:
		''' <ol>
		'''   <li>All methods on the execution stack are from classes
		'''   defined using the system class loader or one of its ancestors.
		''' 
		'''   <li>All methods on the execution stack up to the first
		'''   "privileged" caller
		'''   (see <seealso cref="java.security.AccessController#doPrivileged"/>)
		'''   are from classes
		'''   defined using the system class loader or one of its ancestors.
		''' 
		'''   <li> A call to <code>checkPermission</code> with
		'''   <code>java.security.AllPermission</code> does not
		'''   result in a SecurityException.
		''' 
		''' </ol>
		''' </summary>
		''' <returns>  the class loader of the most recent occurrence on the stack
		'''          of a method from a class defined using a non-system class
		'''          loader.
		''' </returns>
		''' @deprecated This type of security checking is not recommended.
		'''  It is recommended that the <code>checkPermission</code>
		'''  call be used instead.
		''' 
		''' <seealso cref=  java.lang.ClassLoader#getSystemClassLoader() getSystemClassLoader </seealso>
		''' <seealso cref=  #checkPermission(java.security.Permission) checkPermission </seealso>
		<Obsolete("This type of security checking is not recommended.")> _
		Protected Friend Overridable Function currentClassLoader() As ClassLoader
			Dim cl As ClassLoader = currentClassLoader0()
			If (cl IsNot Nothing) AndAlso hasAllPermission() Then cl = Nothing
			Return cl
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Function currentClassLoader0() As ClassLoader
		End Function

		''' <summary>
		''' Returns the class of the most recently executing method from
		''' a class defined using a non-system class loader. A non-system
		''' class loader is defined as being a class loader that is not equal to
		''' the system class loader (as returned
		''' by <seealso cref="ClassLoader#getSystemClassLoader"/>) or one of its ancestors.
		''' <p>
		''' This method will return
		''' <code>null</code> in the following three cases:
		''' <ol>
		'''   <li>All methods on the execution stack are from classes
		'''   defined using the system class loader or one of its ancestors.
		''' 
		'''   <li>All methods on the execution stack up to the first
		'''   "privileged" caller
		'''   (see <seealso cref="java.security.AccessController#doPrivileged"/>)
		'''   are from classes
		'''   defined using the system class loader or one of its ancestors.
		''' 
		'''   <li> A call to <code>checkPermission</code> with
		'''   <code>java.security.AllPermission</code> does not
		'''   result in a SecurityException.
		''' 
		''' </ol>
		''' </summary>
		''' <returns>  the class  of the most recent occurrence on the stack
		'''          of a method from a class defined using a non-system class
		'''          loader.
		''' </returns>
		''' @deprecated This type of security checking is not recommended.
		'''  It is recommended that the <code>checkPermission</code>
		'''  call be used instead.
		''' 
		''' <seealso cref=  java.lang.ClassLoader#getSystemClassLoader() getSystemClassLoader </seealso>
		''' <seealso cref=  #checkPermission(java.security.Permission) checkPermission </seealso>
		<Obsolete("This type of security checking is not recommended.")> _
		Protected Friend Overridable Function currentLoadedClass() As Class
			Dim c As Class = currentLoadedClass0()
			If (c IsNot Nothing) AndAlso hasAllPermission() Then c = Nothing
			Return c
		End Function

		''' <summary>
		''' Returns the stack depth of the specified class.
		''' </summary>
		''' <param name="name">   the fully qualified name of the class to search for. </param>
		''' <returns>  the depth on the stack frame of the first occurrence of a
		'''          method from a class with the specified name;
		'''          <code>-1</code> if such a frame cannot be found. </returns>
		''' @deprecated This type of security checking is not recommended.
		'''  It is recommended that the <code>checkPermission</code>
		'''  call be used instead.
		'''  
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Protected Friend Function classDepth(ByVal name As String) As Integer
		End Function

		''' <summary>
		''' Returns the stack depth of the most recently executing method
		''' from a class defined using a non-system class loader.  A non-system
		''' class loader is defined as being a class loader that is not equal to
		''' the system class loader (as returned
		''' by <seealso cref="ClassLoader#getSystemClassLoader"/>) or one of its ancestors.
		''' <p>
		''' This method will return
		''' -1 in the following three cases:
		''' <ol>
		'''   <li>All methods on the execution stack are from classes
		'''   defined using the system class loader or one of its ancestors.
		''' 
		'''   <li>All methods on the execution stack up to the first
		'''   "privileged" caller
		'''   (see <seealso cref="java.security.AccessController#doPrivileged"/>)
		'''   are from classes
		'''   defined using the system class loader or one of its ancestors.
		''' 
		'''   <li> A call to <code>checkPermission</code> with
		'''   <code>java.security.AllPermission</code> does not
		'''   result in a SecurityException.
		''' 
		''' </ol>
		''' </summary>
		''' <returns> the depth on the stack frame of the most recent occurrence of
		'''          a method from a class defined using a non-system class loader.
		''' </returns>
		''' @deprecated This type of security checking is not recommended.
		'''  It is recommended that the <code>checkPermission</code>
		'''  call be used instead.
		''' 
		''' <seealso cref=   java.lang.ClassLoader#getSystemClassLoader() getSystemClassLoader </seealso>
		''' <seealso cref=   #checkPermission(java.security.Permission) checkPermission </seealso>
		<Obsolete("This type of security checking is not recommended.")> _
		Protected Friend Overridable Function classLoaderDepth() As Integer
			Dim depth As Integer = classLoaderDepth0()
			If depth <> -1 Then
				If hasAllPermission() Then
					depth = -1
				Else
					depth -= 1 ' make sure we don't include ourself
				End If
			End If
			Return depth
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Function classLoaderDepth0() As Integer
		End Function

		''' <summary>
		''' Tests if a method from a class with the specified
		'''         name is on the execution stack.
		''' </summary>
		''' <param name="name">   the fully qualified name of the class. </param>
		''' <returns> <code>true</code> if a method from a class with the specified
		'''         name is on the execution stack; <code>false</code> otherwise. </returns>
		''' @deprecated This type of security checking is not recommended.
		'''  It is recommended that the <code>checkPermission</code>
		'''  call be used instead. 
		<Obsolete("This type of security checking is not recommended.")> _
		Protected Friend Overridable Function inClass(ByVal name As String) As Boolean
			Return classDepth(name) >= 0
		End Function

		''' <summary>
		''' Basically, tests if a method from a class defined using a
		'''          class loader is on the execution stack.
		''' </summary>
		''' <returns>  <code>true</code> if a call to <code>currentClassLoader</code>
		'''          has a non-null return value.
		''' </returns>
		''' @deprecated This type of security checking is not recommended.
		'''  It is recommended that the <code>checkPermission</code>
		'''  call be used instead. 
		''' <seealso cref=        #currentClassLoader() currentClassLoader </seealso>
		<Obsolete("This type of security checking is not recommended.")> _
		Protected Friend Overridable Function inClassLoader() As Boolean
			Return currentClassLoader() IsNot Nothing
		End Function

		''' <summary>
		''' Creates an object that encapsulates the current execution
		''' environment. The result of this method is used, for example, by the
		''' three-argument <code>checkConnect</code> method and by the
		''' two-argument <code>checkRead</code> method.
		''' These methods are needed because a trusted method may be called
		''' on to read a file or open a socket on behalf of another method.
		''' The trusted method needs to determine if the other (possibly
		''' untrusted) method would be allowed to perform the operation on its
		''' own.
		''' <p> The default implementation of this method is to return
		''' an <code>AccessControlContext</code> object.
		''' </summary>
		''' <returns>  an implementation-dependent object that encapsulates
		'''          sufficient information about the current execution environment
		'''          to perform some security checks later. </returns>
		''' <seealso cref=     java.lang.SecurityManager#checkConnect(java.lang.String, int,
		'''   java.lang.Object) checkConnect </seealso>
		''' <seealso cref=     java.lang.SecurityManager#checkRead(java.lang.String,
		'''   java.lang.Object) checkRead </seealso>
		''' <seealso cref=     java.security.AccessControlContext AccessControlContext </seealso>
		Public Overridable Property securityContext As Object
			Get
				Return AccessController.context
			End Get
		End Property

		''' <summary>
		''' Throws a <code>SecurityException</code> if the requested
		''' access, specified by the given permission, is not permitted based
		''' on the security policy currently in effect.
		''' <p>
		''' This method calls <code>AccessController.checkPermission</code>
		''' with the given permission.
		''' </summary>
		''' <param name="perm">   the requested permission. </param>
		''' <exception cref="SecurityException"> if access is not permitted based on
		'''            the current security policy. </exception>
		''' <exception cref="NullPointerException"> if the permission argument is
		'''            <code>null</code>.
		''' @since     1.2 </exception>
		Public Overridable Sub checkPermission(ByVal perm As Permission)
			java.security.AccessController.checkPermission(perm)
		End Sub

		''' <summary>
		''' Throws a <code>SecurityException</code> if the
		''' specified security context is denied access to the resource
		''' specified by the given permission.
		''' The context must be a security
		''' context returned by a previous call to
		''' <code>getSecurityContext</code> and the access control
		''' decision is based upon the configured security policy for
		''' that security context.
		''' <p>
		''' If <code>context</code> is an instance of
		''' <code>AccessControlContext</code> then the
		''' <code>AccessControlContext.checkPermission</code> method is
		''' invoked with the specified permission.
		''' <p>
		''' If <code>context</code> is not an instance of
		''' <code>AccessControlContext</code> then a
		''' <code>SecurityException</code> is thrown.
		''' </summary>
		''' <param name="perm">      the specified permission </param>
		''' <param name="context">   a system-dependent security context. </param>
		''' <exception cref="SecurityException">  if the specified security context
		'''             is not an instance of <code>AccessControlContext</code>
		'''             (e.g., is <code>null</code>), or is denied access to the
		'''             resource specified by the given permission. </exception>
		''' <exception cref="NullPointerException"> if the permission argument is
		'''             <code>null</code>. </exception>
		''' <seealso cref=        java.lang.SecurityManager#getSecurityContext() </seealso>
		''' <seealso cref= java.security.AccessControlContext#checkPermission(java.security.Permission)
		''' @since      1.2 </seealso>
		Public Overridable Sub checkPermission(ByVal perm As Permission, ByVal context As Object)
			If TypeOf context Is AccessControlContext Then
				CType(context, AccessControlContext).checkPermission(perm)
			Else
				Throw New SecurityException
			End If
		End Sub

		''' <summary>
		''' Throws a <code>SecurityException</code> if the
		''' calling thread is not allowed to create a new class loader.
		''' <p>
		''' This method calls <code>checkPermission</code> with the
		''' <code>RuntimePermission("createClassLoader")</code>
		''' permission.
		''' <p>
		''' If you override this method, then you should make a call to
		''' <code>super.checkCreateClassLoader</code>
		''' at the point the overridden method would normally throw an
		''' exception.
		''' </summary>
		''' <exception cref="SecurityException"> if the calling thread does not
		'''             have permission
		'''             to create a new class loader. </exception>
		''' <seealso cref=        java.lang.ClassLoader#ClassLoader() </seealso>
		''' <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		Public Overridable Sub checkCreateClassLoader()
			checkPermission(sun.security.util.SecurityConstants.CREATE_CLASSLOADER_PERMISSION)
		End Sub

		''' <summary>
		''' reference to the root thread group, used for the checkAccess
		''' methods.
		''' </summary>

		Private Shared rootGroup As ThreadGroup = rootGroup

		Private Property Shared rootGroup As ThreadGroup
			Get
				Dim root As ThreadGroup = Thread.CurrentThread.threadGroup
				Do While root.parent IsNot Nothing
					root = root.parent
				Loop
				Return root
			End Get
		End Property

		''' <summary>
		''' Throws a <code>SecurityException</code> if the
		''' calling thread is not allowed to modify the thread argument.
		''' <p>
		''' This method is invoked for the current security manager by the
		''' <code>stop</code>, <code>suspend</code>, <code>resume</code>,
		''' <code>setPriority</code>, <code>setName</code>, and
		''' <code>setDaemon</code> methods of class <code>Thread</code>.
		''' <p>
		''' If the thread argument is a system thread (belongs to
		''' the thread group with a <code>null</code> parent) then
		''' this method calls <code>checkPermission</code> with the
		''' <code>RuntimePermission("modifyThread")</code> permission.
		''' If the thread argument is <i>not</i> a system thread,
		''' this method just returns silently.
		''' <p>
		''' Applications that want a stricter policy should override this
		''' method. If this method is overridden, the method that overrides
		''' it should additionally check to see if the calling thread has the
		''' <code>RuntimePermission("modifyThread")</code> permission, and
		''' if so, return silently. This is to ensure that code granted
		''' that permission (such as the JDK itself) is allowed to
		''' manipulate any thread.
		''' <p>
		''' If this method is overridden, then
		''' <code>super.checkAccess</code> should
		''' be called by the first statement in the overridden method, or the
		''' equivalent security check should be placed in the overridden method.
		''' </summary>
		''' <param name="t">   the thread to be checked. </param>
		''' <exception cref="SecurityException">  if the calling thread does not have
		'''             permission to modify the thread. </exception>
		''' <exception cref="NullPointerException"> if the thread argument is
		'''             <code>null</code>. </exception>
		''' <seealso cref=        java.lang.Thread#resume() resume </seealso>
		''' <seealso cref=        java.lang.Thread#setDaemon(boolean) setDaemon </seealso>
		''' <seealso cref=        java.lang.Thread#setName(java.lang.String) setName </seealso>
		''' <seealso cref=        java.lang.Thread#setPriority(int) setPriority </seealso>
		''' <seealso cref=        java.lang.Thread#stop() stop </seealso>
		''' <seealso cref=        java.lang.Thread#suspend() suspend </seealso>
		''' <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		Public Overridable Sub checkAccess(ByVal t As Thread)
			If t Is Nothing Then Throw New NullPointerException("thread can't be null")
			If t.threadGroup Is rootGroup Then
				checkPermission(sun.security.util.SecurityConstants.MODIFY_THREAD_PERMISSION)
			Else
				' just return
			End If
		End Sub
		''' <summary>
		''' Throws a <code>SecurityException</code> if the
		''' calling thread is not allowed to modify the thread group argument.
		''' <p>
		''' This method is invoked for the current security manager when a
		''' new child thread or child thread group is created, and by the
		''' <code>setDaemon</code>, <code>setMaxPriority</code>,
		''' <code>stop</code>, <code>suspend</code>, <code>resume</code>, and
		''' <code>destroy</code> methods of class <code>ThreadGroup</code>.
		''' <p>
		''' If the thread group argument is the system thread group (
		''' has a <code>null</code> parent) then
		''' this method calls <code>checkPermission</code> with the
		''' <code>RuntimePermission("modifyThreadGroup")</code> permission.
		''' If the thread group argument is <i>not</i> the system thread group,
		''' this method just returns silently.
		''' <p>
		''' Applications that want a stricter policy should override this
		''' method. If this method is overridden, the method that overrides
		''' it should additionally check to see if the calling thread has the
		''' <code>RuntimePermission("modifyThreadGroup")</code> permission, and
		''' if so, return silently. This is to ensure that code granted
		''' that permission (such as the JDK itself) is allowed to
		''' manipulate any thread.
		''' <p>
		''' If this method is overridden, then
		''' <code>super.checkAccess</code> should
		''' be called by the first statement in the overridden method, or the
		''' equivalent security check should be placed in the overridden method.
		''' </summary>
		''' <param name="g">   the thread group to be checked. </param>
		''' <exception cref="SecurityException">  if the calling thread does not have
		'''             permission to modify the thread group. </exception>
		''' <exception cref="NullPointerException"> if the thread group argument is
		'''             <code>null</code>. </exception>
		''' <seealso cref=        java.lang.ThreadGroup#destroy() destroy </seealso>
		''' <seealso cref=        java.lang.ThreadGroup#resume() resume </seealso>
		''' <seealso cref=        java.lang.ThreadGroup#setDaemon(boolean) setDaemon </seealso>
		''' <seealso cref=        java.lang.ThreadGroup#setMaxPriority(int) setMaxPriority </seealso>
		''' <seealso cref=        java.lang.ThreadGroup#stop() stop </seealso>
		''' <seealso cref=        java.lang.ThreadGroup#suspend() suspend </seealso>
		''' <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		Public Overridable Sub checkAccess(ByVal g As ThreadGroup)
			If g Is Nothing Then Throw New NullPointerException("thread group can't be null")
			If g Is rootGroup Then
				checkPermission(sun.security.util.SecurityConstants.MODIFY_THREADGROUP_PERMISSION)
			Else
				' just return
			End If
		End Sub

		''' <summary>
		''' Throws a <code>SecurityException</code> if the
		''' calling thread is not allowed to cause the Java Virtual Machine to
		''' halt with the specified status code.
		''' <p>
		''' This method is invoked for the current security manager by the
		''' <code>exit</code> method of class <code>Runtime</code>. A status
		''' of <code>0</code> indicates success; other values indicate various
		''' errors.
		''' <p>
		''' This method calls <code>checkPermission</code> with the
		''' <code>RuntimePermission("exitVM."+status)</code> permission.
		''' <p>
		''' If you override this method, then you should make a call to
		''' <code>super.checkExit</code>
		''' at the point the overridden method would normally throw an
		''' exception.
		''' </summary>
		''' <param name="status">   the exit status. </param>
		''' <exception cref="SecurityException"> if the calling thread does not have
		'''              permission to halt the Java Virtual Machine with
		'''              the specified status. </exception>
		''' <seealso cref=        java.lang.Runtime#exit(int) exit </seealso>
		''' <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		Public Overridable Sub checkExit(ByVal status As Integer)
			checkPermission(New RuntimePermission("exitVM." & status))
		End Sub

		''' <summary>
		''' Throws a <code>SecurityException</code> if the
		''' calling thread is not allowed to create a subprocess.
		''' <p>
		''' This method is invoked for the current security manager by the
		''' <code>exec</code> methods of class <code>Runtime</code>.
		''' <p>
		''' This method calls <code>checkPermission</code> with the
		''' <code>FilePermission(cmd,"execute")</code> permission
		''' if cmd is an absolute path, otherwise it calls
		''' <code>checkPermission</code> with
		''' <code>FilePermission("&lt;&lt;ALL FILES&gt;&gt;","execute")</code>.
		''' <p>
		''' If you override this method, then you should make a call to
		''' <code>super.checkExec</code>
		''' at the point the overridden method would normally throw an
		''' exception.
		''' </summary>
		''' <param name="cmd">   the specified system command. </param>
		''' <exception cref="SecurityException"> if the calling thread does not have
		'''             permission to create a subprocess. </exception>
		''' <exception cref="NullPointerException"> if the <code>cmd</code> argument is
		'''             <code>null</code>. </exception>
		''' <seealso cref=     java.lang.Runtime#exec(java.lang.String) </seealso>
		''' <seealso cref=     java.lang.Runtime#exec(java.lang.String, java.lang.String[]) </seealso>
		''' <seealso cref=     java.lang.Runtime#exec(java.lang.String[]) </seealso>
		''' <seealso cref=     java.lang.Runtime#exec(java.lang.String[], java.lang.String[]) </seealso>
		''' <seealso cref=     #checkPermission(java.security.Permission) checkPermission </seealso>
		Public Overridable Sub checkExec(ByVal cmd As String)
			Dim f As New File(cmd)
			If f.absolute Then
				checkPermission(New java.io.FilePermission(cmd, sun.security.util.SecurityConstants.FILE_EXECUTE_ACTION))
			Else
				checkPermission(New java.io.FilePermission("<<ALL FILES>>", sun.security.util.SecurityConstants.FILE_EXECUTE_ACTION))
			End If
		End Sub

		''' <summary>
		''' Throws a <code>SecurityException</code> if the
		''' calling thread is not allowed to dynamic link the library code
		''' specified by the string argument file. The argument is either a
		''' simple library name or a complete filename.
		''' <p>
		''' This method is invoked for the current security manager by
		''' methods <code>load</code> and <code>loadLibrary</code> of class
		''' <code>Runtime</code>.
		''' <p>
		''' This method calls <code>checkPermission</code> with the
		''' <code>RuntimePermission("loadLibrary."+lib)</code> permission.
		''' <p>
		''' If you override this method, then you should make a call to
		''' <code>super.checkLink</code>
		''' at the point the overridden method would normally throw an
		''' exception.
		''' </summary>
		''' <param name="lib">   the name of the library. </param>
		''' <exception cref="SecurityException"> if the calling thread does not have
		'''             permission to dynamically link the library. </exception>
		''' <exception cref="NullPointerException"> if the <code>lib</code> argument is
		'''             <code>null</code>. </exception>
		''' <seealso cref=        java.lang.Runtime#load(java.lang.String) </seealso>
		''' <seealso cref=        java.lang.Runtime#loadLibrary(java.lang.String) </seealso>
		''' <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		Public Overridable Sub checkLink(ByVal [lib] As String)
			If [lib] Is Nothing Then Throw New NullPointerException("library can't be null")
			checkPermission(New RuntimePermission("loadLibrary." & [lib]))
		End Sub

		''' <summary>
		''' Throws a <code>SecurityException</code> if the
		''' calling thread is not allowed to read from the specified file
		''' descriptor.
		''' <p>
		''' This method calls <code>checkPermission</code> with the
		''' <code>RuntimePermission("readFileDescriptor")</code>
		''' permission.
		''' <p>
		''' If you override this method, then you should make a call to
		''' <code>super.checkRead</code>
		''' at the point the overridden method would normally throw an
		''' exception.
		''' </summary>
		''' <param name="fd">   the system-dependent file descriptor. </param>
		''' <exception cref="SecurityException">  if the calling thread does not have
		'''             permission to access the specified file descriptor. </exception>
		''' <exception cref="NullPointerException"> if the file descriptor argument is
		'''             <code>null</code>. </exception>
		''' <seealso cref=        java.io.FileDescriptor </seealso>
		''' <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		Public Overridable Sub checkRead(ByVal fd As java.io.FileDescriptor)
			If fd Is Nothing Then Throw New NullPointerException("file descriptor can't be null")
			checkPermission(New RuntimePermission("readFileDescriptor"))
		End Sub

		''' <summary>
		''' Throws a <code>SecurityException</code> if the
		''' calling thread is not allowed to read the file specified by the
		''' string argument.
		''' <p>
		''' This method calls <code>checkPermission</code> with the
		''' <code>FilePermission(file,"read")</code> permission.
		''' <p>
		''' If you override this method, then you should make a call to
		''' <code>super.checkRead</code>
		''' at the point the overridden method would normally throw an
		''' exception.
		''' </summary>
		''' <param name="file">   the system-dependent file name. </param>
		''' <exception cref="SecurityException"> if the calling thread does not have
		'''             permission to access the specified file. </exception>
		''' <exception cref="NullPointerException"> if the <code>file</code> argument is
		'''             <code>null</code>. </exception>
		''' <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		Public Overridable Sub checkRead(ByVal file As String)
			checkPermission(New java.io.FilePermission(file, sun.security.util.SecurityConstants.FILE_READ_ACTION))
		End Sub

		''' <summary>
		''' Throws a <code>SecurityException</code> if the
		''' specified security context is not allowed to read the file
		''' specified by the string argument. The context must be a security
		''' context returned by a previous call to
		''' <code>getSecurityContext</code>.
		''' <p> If <code>context</code> is an instance of
		''' <code>AccessControlContext</code> then the
		''' <code>AccessControlContext.checkPermission</code> method will
		''' be invoked with the <code>FilePermission(file,"read")</code> permission.
		''' <p> If <code>context</code> is not an instance of
		''' <code>AccessControlContext</code> then a
		''' <code>SecurityException</code> is thrown.
		''' <p>
		''' If you override this method, then you should make a call to
		''' <code>super.checkRead</code>
		''' at the point the overridden method would normally throw an
		''' exception.
		''' </summary>
		''' <param name="file">      the system-dependent filename. </param>
		''' <param name="context">   a system-dependent security context. </param>
		''' <exception cref="SecurityException">  if the specified security context
		'''             is not an instance of <code>AccessControlContext</code>
		'''             (e.g., is <code>null</code>), or does not have permission
		'''             to read the specified file. </exception>
		''' <exception cref="NullPointerException"> if the <code>file</code> argument is
		'''             <code>null</code>. </exception>
		''' <seealso cref=        java.lang.SecurityManager#getSecurityContext() </seealso>
		''' <seealso cref=        java.security.AccessControlContext#checkPermission(java.security.Permission) </seealso>
		Public Overridable Sub checkRead(ByVal file As String, ByVal context As Object)
			checkPermission(New java.io.FilePermission(file, sun.security.util.SecurityConstants.FILE_READ_ACTION), context)
		End Sub

		''' <summary>
		''' Throws a <code>SecurityException</code> if the
		''' calling thread is not allowed to write to the specified file
		''' descriptor.
		''' <p>
		''' This method calls <code>checkPermission</code> with the
		''' <code>RuntimePermission("writeFileDescriptor")</code>
		''' permission.
		''' <p>
		''' If you override this method, then you should make a call to
		''' <code>super.checkWrite</code>
		''' at the point the overridden method would normally throw an
		''' exception.
		''' </summary>
		''' <param name="fd">   the system-dependent file descriptor. </param>
		''' <exception cref="SecurityException">  if the calling thread does not have
		'''             permission to access the specified file descriptor. </exception>
		''' <exception cref="NullPointerException"> if the file descriptor argument is
		'''             <code>null</code>. </exception>
		''' <seealso cref=        java.io.FileDescriptor </seealso>
		''' <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		Public Overridable Sub checkWrite(ByVal fd As java.io.FileDescriptor)
			If fd Is Nothing Then Throw New NullPointerException("file descriptor can't be null")
			checkPermission(New RuntimePermission("writeFileDescriptor"))

		End Sub

		''' <summary>
		''' Throws a <code>SecurityException</code> if the
		''' calling thread is not allowed to write to the file specified by
		''' the string argument.
		''' <p>
		''' This method calls <code>checkPermission</code> with the
		''' <code>FilePermission(file,"write")</code> permission.
		''' <p>
		''' If you override this method, then you should make a call to
		''' <code>super.checkWrite</code>
		''' at the point the overridden method would normally throw an
		''' exception.
		''' </summary>
		''' <param name="file">   the system-dependent filename. </param>
		''' <exception cref="SecurityException">  if the calling thread does not
		'''             have permission to access the specified file. </exception>
		''' <exception cref="NullPointerException"> if the <code>file</code> argument is
		'''             <code>null</code>. </exception>
		''' <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		Public Overridable Sub checkWrite(ByVal file As String)
			checkPermission(New java.io.FilePermission(file, sun.security.util.SecurityConstants.FILE_WRITE_ACTION))
		End Sub

		''' <summary>
		''' Throws a <code>SecurityException</code> if the
		''' calling thread is not allowed to delete the specified file.
		''' <p>
		''' This method is invoked for the current security manager by the
		''' <code>delete</code> method of class <code>File</code>.
		''' <p>
		''' This method calls <code>checkPermission</code> with the
		''' <code>FilePermission(file,"delete")</code> permission.
		''' <p>
		''' If you override this method, then you should make a call to
		''' <code>super.checkDelete</code>
		''' at the point the overridden method would normally throw an
		''' exception.
		''' </summary>
		''' <param name="file">   the system-dependent filename. </param>
		''' <exception cref="SecurityException"> if the calling thread does not
		'''             have permission to delete the file. </exception>
		''' <exception cref="NullPointerException"> if the <code>file</code> argument is
		'''             <code>null</code>. </exception>
		''' <seealso cref=        java.io.File#delete() </seealso>
		''' <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		Public Overridable Sub checkDelete(ByVal file As String)
			checkPermission(New java.io.FilePermission(file, sun.security.util.SecurityConstants.FILE_DELETE_ACTION))
		End Sub

		''' <summary>
		''' Throws a <code>SecurityException</code> if the
		''' calling thread is not allowed to open a socket connection to the
		''' specified host and port number.
		''' <p>
		''' A port number of <code>-1</code> indicates that the calling
		''' method is attempting to determine the IP address of the specified
		''' host name.
		''' <p>
		''' This method calls <code>checkPermission</code> with the
		''' <code>SocketPermission(host+":"+port,"connect")</code> permission if
		''' the port is not equal to -1. If the port is equal to -1, then
		''' it calls <code>checkPermission</code> with the
		''' <code>SocketPermission(host,"resolve")</code> permission.
		''' <p>
		''' If you override this method, then you should make a call to
		''' <code>super.checkConnect</code>
		''' at the point the overridden method would normally throw an
		''' exception.
		''' </summary>
		''' <param name="host">   the host name port to connect to. </param>
		''' <param name="port">   the protocol port to connect to. </param>
		''' <exception cref="SecurityException">  if the calling thread does not have
		'''             permission to open a socket connection to the specified
		'''               <code>host</code> and <code>port</code>. </exception>
		''' <exception cref="NullPointerException"> if the <code>host</code> argument is
		'''             <code>null</code>. </exception>
		''' <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		Public Overridable Sub checkConnect(ByVal host As String, ByVal port As Integer)
			If host Is Nothing Then Throw New NullPointerException("host can't be null")
			If (Not host.StartsWith("[")) AndAlso host.IndexOf(":"c) <> -1 Then host = "[" & host & "]"
			If port = -1 Then
				checkPermission(New java.net.SocketPermission(host, sun.security.util.SecurityConstants.SOCKET_RESOLVE_ACTION))
			Else
				checkPermission(New java.net.SocketPermission(host & ":" & port, sun.security.util.SecurityConstants.SOCKET_CONNECT_ACTION))
			End If
		End Sub

		''' <summary>
		''' Throws a <code>SecurityException</code> if the
		''' specified security context is not allowed to open a socket
		''' connection to the specified host and port number.
		''' <p>
		''' A port number of <code>-1</code> indicates that the calling
		''' method is attempting to determine the IP address of the specified
		''' host name.
		''' <p> If <code>context</code> is not an instance of
		''' <code>AccessControlContext</code> then a
		''' <code>SecurityException</code> is thrown.
		''' <p>
		''' Otherwise, the port number is checked. If it is not equal
		''' to -1, the <code>context</code>'s <code>checkPermission</code>
		''' method is called with a
		''' <code>SocketPermission(host+":"+port,"connect")</code> permission.
		''' If the port is equal to -1, then
		''' the <code>context</code>'s <code>checkPermission</code> method
		''' is called with a
		''' <code>SocketPermission(host,"resolve")</code> permission.
		''' <p>
		''' If you override this method, then you should make a call to
		''' <code>super.checkConnect</code>
		''' at the point the overridden method would normally throw an
		''' exception.
		''' </summary>
		''' <param name="host">      the host name port to connect to. </param>
		''' <param name="port">      the protocol port to connect to. </param>
		''' <param name="context">   a system-dependent security context. </param>
		''' <exception cref="SecurityException"> if the specified security context
		'''             is not an instance of <code>AccessControlContext</code>
		'''             (e.g., is <code>null</code>), or does not have permission
		'''             to open a socket connection to the specified
		'''             <code>host</code> and <code>port</code>. </exception>
		''' <exception cref="NullPointerException"> if the <code>host</code> argument is
		'''             <code>null</code>. </exception>
		''' <seealso cref=        java.lang.SecurityManager#getSecurityContext() </seealso>
		''' <seealso cref=        java.security.AccessControlContext#checkPermission(java.security.Permission) </seealso>
		Public Overridable Sub checkConnect(ByVal host As String, ByVal port As Integer, ByVal context As Object)
			If host Is Nothing Then Throw New NullPointerException("host can't be null")
			If (Not host.StartsWith("[")) AndAlso host.IndexOf(":"c) <> -1 Then host = "[" & host & "]"
			If port = -1 Then
				checkPermission(New java.net.SocketPermission(host, sun.security.util.SecurityConstants.SOCKET_RESOLVE_ACTION), context)
			Else
				checkPermission(New java.net.SocketPermission(host & ":" & port, sun.security.util.SecurityConstants.SOCKET_CONNECT_ACTION), context)
			End If
		End Sub

		''' <summary>
		''' Throws a <code>SecurityException</code> if the
		''' calling thread is not allowed to wait for a connection request on
		''' the specified local port number.
		''' <p>
		''' This method calls <code>checkPermission</code> with the
		''' <code>SocketPermission("localhost:"+port,"listen")</code>.
		''' <p>
		''' If you override this method, then you should make a call to
		''' <code>super.checkListen</code>
		''' at the point the overridden method would normally throw an
		''' exception.
		''' </summary>
		''' <param name="port">   the local port. </param>
		''' <exception cref="SecurityException">  if the calling thread does not have
		'''             permission to listen on the specified port. </exception>
		''' <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		Public Overridable Sub checkListen(ByVal port As Integer)
			checkPermission(New java.net.SocketPermission("localhost:" & port, sun.security.util.SecurityConstants.SOCKET_LISTEN_ACTION))
		End Sub

		''' <summary>
		''' Throws a <code>SecurityException</code> if the
		''' calling thread is not permitted to accept a socket connection from
		''' the specified host and port number.
		''' <p>
		''' This method is invoked for the current security manager by the
		''' <code>accept</code> method of class <code>ServerSocket</code>.
		''' <p>
		''' This method calls <code>checkPermission</code> with the
		''' <code>SocketPermission(host+":"+port,"accept")</code> permission.
		''' <p>
		''' If you override this method, then you should make a call to
		''' <code>super.checkAccept</code>
		''' at the point the overridden method would normally throw an
		''' exception.
		''' </summary>
		''' <param name="host">   the host name of the socket connection. </param>
		''' <param name="port">   the port number of the socket connection. </param>
		''' <exception cref="SecurityException">  if the calling thread does not have
		'''             permission to accept the connection. </exception>
		''' <exception cref="NullPointerException"> if the <code>host</code> argument is
		'''             <code>null</code>. </exception>
		''' <seealso cref=        java.net.ServerSocket#accept() </seealso>
		''' <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		Public Overridable Sub checkAccept(ByVal host As String, ByVal port As Integer)
			If host Is Nothing Then Throw New NullPointerException("host can't be null")
			If (Not host.StartsWith("[")) AndAlso host.IndexOf(":"c) <> -1 Then host = "[" & host & "]"
			checkPermission(New java.net.SocketPermission(host & ":" & port, sun.security.util.SecurityConstants.SOCKET_ACCEPT_ACTION))
		End Sub

		''' <summary>
		''' Throws a <code>SecurityException</code> if the
		''' calling thread is not allowed to use
		''' (join/leave/send/receive) IP multicast.
		''' <p>
		''' This method calls <code>checkPermission</code> with the
		''' <code>java.net.SocketPermission(maddr.getHostAddress(),
		''' "accept,connect")</code> permission.
		''' <p>
		''' If you override this method, then you should make a call to
		''' <code>super.checkMulticast</code>
		''' at the point the overridden method would normally throw an
		''' exception.
		''' </summary>
		''' <param name="maddr">  Internet group address to be used. </param>
		''' <exception cref="SecurityException">  if the calling thread is not allowed to
		'''  use (join/leave/send/receive) IP multicast. </exception>
		''' <exception cref="NullPointerException"> if the address argument is
		'''             <code>null</code>.
		''' @since      JDK1.1 </exception>
		''' <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		Public Overridable Sub checkMulticast(ByVal maddr As java.net.InetAddress)
			Dim host As String = maddr.hostAddress
			If (Not host.StartsWith("[")) AndAlso host.IndexOf(":"c) <> -1 Then host = "[" & host & "]"
			checkPermission(New java.net.SocketPermission(host, sun.security.util.SecurityConstants.SOCKET_CONNECT_ACCEPT_ACTION))
		End Sub

		''' <summary>
		''' Throws a <code>SecurityException</code> if the
		''' calling thread is not allowed to use
		''' (join/leave/send/receive) IP multicast.
		''' <p>
		''' This method calls <code>checkPermission</code> with the
		''' <code>java.net.SocketPermission(maddr.getHostAddress(),
		''' "accept,connect")</code> permission.
		''' <p>
		''' If you override this method, then you should make a call to
		''' <code>super.checkMulticast</code>
		''' at the point the overridden method would normally throw an
		''' exception.
		''' </summary>
		''' <param name="maddr">  Internet group address to be used. </param>
		''' <param name="ttl">        value in use, if it is multicast send.
		''' Note: this particular implementation does not use the ttl
		''' parameter. </param>
		''' <exception cref="SecurityException">  if the calling thread is not allowed to
		'''  use (join/leave/send/receive) IP multicast. </exception>
		''' <exception cref="NullPointerException"> if the address argument is
		'''             <code>null</code>.
		''' @since      JDK1.1 </exception>
		''' @deprecated Use #checkPermission(java.security.Permission) instead 
		''' <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		<Obsolete("Use #checkPermission(java.security.Permission) instead")> _
		Public Overridable Sub checkMulticast(ByVal maddr As java.net.InetAddress, ByVal ttl As SByte)
			Dim host As String = maddr.hostAddress
			If (Not host.StartsWith("[")) AndAlso host.IndexOf(":"c) <> -1 Then host = "[" & host & "]"
			checkPermission(New java.net.SocketPermission(host, sun.security.util.SecurityConstants.SOCKET_CONNECT_ACCEPT_ACTION))
		End Sub

		''' <summary>
		''' Throws a <code>SecurityException</code> if the
		''' calling thread is not allowed to access or modify the system
		''' properties.
		''' <p>
		''' This method is used by the <code>getProperties</code> and
		''' <code>setProperties</code> methods of class <code>System</code>.
		''' <p>
		''' This method calls <code>checkPermission</code> with the
		''' <code>PropertyPermission("*", "read,write")</code> permission.
		''' <p>
		''' If you override this method, then you should make a call to
		''' <code>super.checkPropertiesAccess</code>
		''' at the point the overridden method would normally throw an
		''' exception.
		''' <p>
		''' </summary>
		''' <exception cref="SecurityException">  if the calling thread does not have
		'''             permission to access or modify the system properties. </exception>
		''' <seealso cref=        java.lang.System#getProperties() </seealso>
		''' <seealso cref=        java.lang.System#setProperties(java.util.Properties) </seealso>
		''' <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		Public Overridable Sub checkPropertiesAccess()
			checkPermission(New java.util.PropertyPermission("*", sun.security.util.SecurityConstants.PROPERTY_RW_ACTION))
		End Sub

		''' <summary>
		''' Throws a <code>SecurityException</code> if the
		''' calling thread is not allowed to access the system property with
		''' the specified <code>key</code> name.
		''' <p>
		''' This method is used by the <code>getProperty</code> method of
		''' class <code>System</code>.
		''' <p>
		''' This method calls <code>checkPermission</code> with the
		''' <code>PropertyPermission(key, "read")</code> permission.
		''' <p>
		''' If you override this method, then you should make a call to
		''' <code>super.checkPropertyAccess</code>
		''' at the point the overridden method would normally throw an
		''' exception.
		''' </summary>
		''' <param name="key">   a system property key.
		''' </param>
		''' <exception cref="SecurityException">  if the calling thread does not have
		'''             permission to access the specified system property. </exception>
		''' <exception cref="NullPointerException"> if the <code>key</code> argument is
		'''             <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>key</code> is empty.
		''' </exception>
		''' <seealso cref=        java.lang.System#getProperty(java.lang.String) </seealso>
		''' <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		Public Overridable Sub checkPropertyAccess(ByVal key As String)
			checkPermission(New java.util.PropertyPermission(key, sun.security.util.SecurityConstants.PROPERTY_READ_ACTION))
		End Sub

		''' <summary>
		''' Returns <code>false</code> if the calling
		''' thread is not trusted to bring up the top-level window indicated
		''' by the <code>window</code> argument. In this case, the caller can
		''' still decide to show the window, but the window should include
		''' some sort of visual warning. If the method returns
		''' <code>true</code>, then the window can be shown without any
		''' special restrictions.
		''' <p>
		''' See class <code>Window</code> for more information on trusted and
		''' untrusted windows.
		''' <p>
		''' This method calls
		''' <code>checkPermission</code> with the
		''' <code>AWTPermission("showWindowWithoutWarningBanner")</code> permission,
		''' and returns <code>true</code> if a SecurityException is not thrown,
		''' otherwise it returns <code>false</code>.
		''' In the case of subset Profiles of Java SE that do not include the
		''' {@code java.awt} package, {@code checkPermission} is instead called
		''' to check the permission {@code java.security.AllPermission}.
		''' <p>
		''' If you override this method, then you should make a call to
		''' <code>super.checkTopLevelWindow</code>
		''' at the point the overridden method would normally return
		''' <code>false</code>, and the value of
		''' <code>super.checkTopLevelWindow</code> should
		''' be returned.
		''' </summary>
		''' <param name="window">   the new window that is being created. </param>
		''' <returns>     <code>true</code> if the calling thread is trusted to put up
		'''             top-level windows; <code>false</code> otherwise. </returns>
		''' <exception cref="NullPointerException"> if the <code>window</code> argument is
		'''             <code>null</code>. </exception>
		''' @deprecated The dependency on {@code AWTPermission} creates an
		'''             impediment to future modularization of the Java platform.
		'''             Users of this method should instead invoke
		'''             <seealso cref="#checkPermission"/> directly.
		'''             This method will be changed in a future release to check
		'''             the permission {@code java.security.AllPermission}. 
		''' <seealso cref=        java.awt.Window </seealso>
		''' <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		<Obsolete("The dependency on {@code AWTPermission} creates an")> _
		Public Overridable Function checkTopLevelWindow(ByVal window As Object) As Boolean
			If window Is Nothing Then Throw New NullPointerException("window can't be null")
			Dim perm As Permission = sun.security.util.SecurityConstants.AWT.TOPLEVEL_WINDOW_PERMISSION
			If perm Is Nothing Then perm = sun.security.util.SecurityConstants.ALL_PERMISSION
			Try
				checkPermission(perm)
				Return True
			Catch se As SecurityException
				' just return false
			End Try
			Return False
		End Function

		''' <summary>
		''' Throws a <code>SecurityException</code> if the
		''' calling thread is not allowed to initiate a print job request.
		''' <p>
		''' This method calls
		''' <code>checkPermission</code> with the
		''' <code>RuntimePermission("queuePrintJob")</code> permission.
		''' <p>
		''' If you override this method, then you should make a call to
		''' <code>super.checkPrintJobAccess</code>
		''' at the point the overridden method would normally throw an
		''' exception.
		''' <p>
		''' </summary>
		''' <exception cref="SecurityException">  if the calling thread does not have
		'''             permission to initiate a print job request.
		''' @since   JDK1.1 </exception>
		''' <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		Public Overridable Sub checkPrintJobAccess()
			checkPermission(New RuntimePermission("queuePrintJob"))
		End Sub

		''' <summary>
		''' Throws a <code>SecurityException</code> if the
		''' calling thread is not allowed to access the system clipboard.
		''' <p>
		''' This method calls <code>checkPermission</code> with the
		''' <code>AWTPermission("accessClipboard")</code>
		''' permission.
		''' In the case of subset Profiles of Java SE that do not include the
		''' {@code java.awt} package, {@code checkPermission} is instead called
		''' to check the permission {@code java.security.AllPermission}.
		''' <p>
		''' If you override this method, then you should make a call to
		''' <code>super.checkSystemClipboardAccess</code>
		''' at the point the overridden method would normally throw an
		''' exception.
		''' 
		''' @since   JDK1.1 </summary>
		''' <exception cref="SecurityException">  if the calling thread does not have
		'''             permission to access the system clipboard. </exception>
		''' @deprecated The dependency on {@code AWTPermission} creates an
		'''             impediment to future modularization of the Java platform.
		'''             Users of this method should instead invoke
		'''             <seealso cref="#checkPermission"/> directly.
		'''             This method will be changed in a future release to check
		'''             the permission {@code java.security.AllPermission}. 
		''' <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		<Obsolete("The dependency on {@code AWTPermission} creates an")> _
		Public Overridable Sub checkSystemClipboardAccess()
			Dim perm As Permission = sun.security.util.SecurityConstants.AWT.ACCESS_CLIPBOARD_PERMISSION
			If perm Is Nothing Then perm = sun.security.util.SecurityConstants.ALL_PERMISSION
			checkPermission(perm)
		End Sub

		''' <summary>
		''' Throws a <code>SecurityException</code> if the
		''' calling thread is not allowed to access the AWT event queue.
		''' <p>
		''' This method calls <code>checkPermission</code> with the
		''' <code>AWTPermission("accessEventQueue")</code> permission.
		''' In the case of subset Profiles of Java SE that do not include the
		''' {@code java.awt} package, {@code checkPermission} is instead called
		''' to check the permission {@code java.security.AllPermission}.
		''' 
		''' <p>
		''' If you override this method, then you should make a call to
		''' <code>super.checkAwtEventQueueAccess</code>
		''' at the point the overridden method would normally throw an
		''' exception.
		''' 
		''' @since   JDK1.1 </summary>
		''' <exception cref="SecurityException">  if the calling thread does not have
		'''             permission to access the AWT event queue. </exception>
		''' @deprecated The dependency on {@code AWTPermission} creates an
		'''             impediment to future modularization of the Java platform.
		'''             Users of this method should instead invoke
		'''             <seealso cref="#checkPermission"/> directly.
		'''             This method will be changed in a future release to check
		'''             the permission {@code java.security.AllPermission}. 
		''' <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		<Obsolete("The dependency on {@code AWTPermission} creates an")> _
		Public Overridable Sub checkAwtEventQueueAccess()
			Dim perm As Permission = sun.security.util.SecurityConstants.AWT.CHECK_AWT_EVENTQUEUE_PERMISSION
			If perm Is Nothing Then perm = sun.security.util.SecurityConstants.ALL_PERMISSION
			checkPermission(perm)
		End Sub

	'    
	'     * We have an initial invalid bit (initially false) for the class
	'     * variables which tell if the cache is valid.  If the underlying
	'     * java.security.Security property changes via setProperty(), the
	'     * Security class uses reflection to change the variable and thus
	'     * invalidate the cache.
	'     *
	'     * Locking is handled by synchronization to the
	'     * packageAccessLock/packageDefinitionLock objects.  They are only
	'     * used in this class.
	'     *
	'     * Note that cache invalidation as a result of the property change
	'     * happens without using these locks, so there may be a delay between
	'     * when a thread updates the property and when other threads updates
	'     * the cache.
	'     
		Private Shared packageAccessValid As Boolean = False
		Private Shared packageAccess As String()
		Private Shared ReadOnly packageAccessLock As New Object

		Private Shared packageDefinitionValid As Boolean = False
		Private Shared packageDefinition As String()
		Private Shared ReadOnly packageDefinitionLock As New Object

		Private Shared Function getPackages(ByVal p As String) As String()
			Dim packages_Renamed As String() = Nothing
			If p IsNot Nothing AndAlso (Not p.Equals("")) Then
				Dim tok As New java.util.StringTokenizer(p, ",")
				Dim n As Integer = tok.countTokens()
				If n > 0 Then
				packages_Renamed = New String(n - 1){}
					Dim i As Integer = 0
					Do While tok.hasMoreElements()
						Dim s As String = tok.nextToken().Trim()
					packages_Renamed(i) = s
						i += 1
					Loop
				End If
			End If

			If packages_Renamed Is Nothing Then packages_Renamed = New String(){}
			Return packages_Renamed
		End Function

		''' <summary>
		''' Throws a <code>SecurityException</code> if the
		''' calling thread is not allowed to access the package specified by
		''' the argument.
		''' <p>
		''' This method is used by the <code>loadClass</code> method of class
		''' loaders.
		''' <p>
		''' This method first gets a list of
		''' restricted packages by obtaining a comma-separated list from
		''' a call to
		''' <code>java.security.Security.getProperty("package.access")</code>,
		''' and checks to see if <code>pkg</code> starts with or equals
		''' any of the restricted packages. If it does, then
		''' <code>checkPermission</code> gets called with the
		''' <code>RuntimePermission("accessClassInPackage."+pkg)</code>
		''' permission.
		''' <p>
		''' If this method is overridden, then
		''' <code>super.checkPackageAccess</code> should be called
		''' as the first line in the overridden method.
		''' </summary>
		''' <param name="pkg">   the package name. </param>
		''' <exception cref="SecurityException">  if the calling thread does not have
		'''             permission to access the specified package. </exception>
		''' <exception cref="NullPointerException"> if the package name argument is
		'''             <code>null</code>. </exception>
		''' <seealso cref=        java.lang.ClassLoader#loadClass(java.lang.String, boolean)
		'''  loadClass </seealso>
		''' <seealso cref=        java.security.Security#getProperty getProperty </seealso>
		''' <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		Public Overridable Sub checkPackageAccess(ByVal pkg As String)
			If pkg Is Nothing Then Throw New NullPointerException("package name can't be null")

			Dim pkgs As String()
			SyncLock packageAccessLock
	'            
	'             * Do we need to update our property array?
	'             
				If Not packageAccessValid Then
					Dim tmpPropertyStr As String = AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
					   )
				packageAccess = getPackages(tmpPropertyStr)
				packageAccessValid = True
				End If

				' Using a snapshot of packageAccess -- don't care if static field
				' changes afterwards; array contents won't change.
				pkgs = packageAccess
			End SyncLock

	'        
	'         * Traverse the list of packages, check for any matches.
	'         
			For i As Integer = 0 To pkgs.Length - 1
				If pkg.StartsWith(pkgs(i)) OrElse pkgs(i).Equals(pkg & ".") Then
					checkPermission(New RuntimePermission("accessClassInPackage." & pkg))
					Exit For ' No need to continue; only need to check this once
				End If
			Next i
		End Sub

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements PrivilegedAction(Of T)

			Public Overridable Function run() As String
				Return java.security.Security.getProperty("package.access")
			End Function
		End Class

		''' <summary>
		''' Throws a <code>SecurityException</code> if the
		''' calling thread is not allowed to define classes in the package
		''' specified by the argument.
		''' <p>
		''' This method is used by the <code>loadClass</code> method of some
		''' class loaders.
		''' <p>
		''' This method first gets a list of restricted packages by
		''' obtaining a comma-separated list from a call to
		''' <code>java.security.Security.getProperty("package.definition")</code>,
		''' and checks to see if <code>pkg</code> starts with or equals
		''' any of the restricted packages. If it does, then
		''' <code>checkPermission</code> gets called with the
		''' <code>RuntimePermission("defineClassInPackage."+pkg)</code>
		''' permission.
		''' <p>
		''' If this method is overridden, then
		''' <code>super.checkPackageDefinition</code> should be called
		''' as the first line in the overridden method.
		''' </summary>
		''' <param name="pkg">   the package name. </param>
		''' <exception cref="SecurityException">  if the calling thread does not have
		'''             permission to define classes in the specified package. </exception>
		''' <seealso cref=        java.lang.ClassLoader#loadClass(java.lang.String, boolean) </seealso>
		''' <seealso cref=        java.security.Security#getProperty getProperty </seealso>
		''' <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		Public Overridable Sub checkPackageDefinition(ByVal pkg As String)
			If pkg Is Nothing Then Throw New NullPointerException("package name can't be null")

			Dim pkgs As String()
			SyncLock packageDefinitionLock
	'            
	'             * Do we need to update our property array?
	'             
				If Not packageDefinitionValid Then
					Dim tmpPropertyStr As String = AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper2(Of T)
					   )
				packageDefinition = getPackages(tmpPropertyStr)
				packageDefinitionValid = True
				End If
				' Using a snapshot of packageDefinition -- don't care if static
				' field changes afterwards; array contents won't change.
				pkgs = packageDefinition
			End SyncLock

	'        
	'         * Traverse the list of packages, check for any matches.
	'         
			For i As Integer = 0 To pkgs.Length - 1
				If pkg.StartsWith(pkgs(i)) OrElse pkgs(i).Equals(pkg & ".") Then
					checkPermission(New RuntimePermission("defineClassInPackage." & pkg))
					Exit For ' No need to continue; only need to check this once
				End If
			Next i
		End Sub

		Private Class PrivilegedActionAnonymousInnerClassHelper2(Of T)
			Implements PrivilegedAction(Of T)

			Public Overridable Function run() As String
				Return java.security.Security.getProperty("package.definition")
			End Function
		End Class

		''' <summary>
		''' Throws a <code>SecurityException</code> if the
		''' calling thread is not allowed to set the socket factory used by
		''' <code>ServerSocket</code> or <code>Socket</code>, or the stream
		''' handler factory used by <code>URL</code>.
		''' <p>
		''' This method calls <code>checkPermission</code> with the
		''' <code>RuntimePermission("setFactory")</code> permission.
		''' <p>
		''' If you override this method, then you should make a call to
		''' <code>super.checkSetFactory</code>
		''' at the point the overridden method would normally throw an
		''' exception.
		''' <p>
		''' </summary>
		''' <exception cref="SecurityException">  if the calling thread does not have
		'''             permission to specify a socket factory or a stream
		'''             handler factory.
		''' </exception>
		''' <seealso cref=        java.net.ServerSocket#setSocketFactory(java.net.SocketImplFactory) setSocketFactory </seealso>
		''' <seealso cref=        java.net.Socket#setSocketImplFactory(java.net.SocketImplFactory) setSocketImplFactory </seealso>
		''' <seealso cref=        java.net.URL#setURLStreamHandlerFactory(java.net.URLStreamHandlerFactory) setURLStreamHandlerFactory </seealso>
		''' <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		Public Overridable Sub checkSetFactory()
			checkPermission(New RuntimePermission("setFactory"))
		End Sub

		''' <summary>
		''' Throws a <code>SecurityException</code> if the
		''' calling thread is not allowed to access members.
		''' <p>
		''' The default policy is to allow access to PUBLIC members, as well
		''' as access to classes that have the same class loader as the caller.
		''' In all other cases, this method calls <code>checkPermission</code>
		''' with the <code>RuntimePermission("accessDeclaredMembers")
		''' </code> permission.
		''' <p>
		''' If this method is overridden, then a call to
		''' <code>super.checkMemberAccess</code> cannot be made,
		''' as the default implementation of <code>checkMemberAccess</code>
		''' relies on the code being checked being at a stack depth of
		''' 4.
		''' </summary>
		''' <param name="clazz"> the class that reflection is to be performed on.
		''' </param>
		''' <param name="which"> type of access, PUBLIC or DECLARED.
		''' </param>
		''' <exception cref="SecurityException"> if the caller does not have
		'''             permission to access members. </exception>
		''' <exception cref="NullPointerException"> if the <code>clazz</code> argument is
		'''             <code>null</code>.
		''' </exception>
		''' @deprecated This method relies on the caller being at a stack depth
		'''             of 4 which is error-prone and cannot be enforced by the runtime.
		'''             Users of this method should instead invoke <seealso cref="#checkPermission"/>
		'''             directly.  This method will be changed in a future release
		'''             to check the permission {@code java.security.AllPermission}.
		''' 
		''' <seealso cref= java.lang.reflect.Member
		''' @since JDK1.1 </seealso>
		''' <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		<Obsolete("This method relies on the caller being at a stack depth")> _
		Public Overridable Sub checkMemberAccess(ByVal clazz As Class, ByVal which As Integer)
			If clazz Is Nothing Then Throw New NullPointerException("class can't be null")
			If which <> Member.PUBLIC Then
				Dim stack As Class() = classContext
	'            
	'             * stack depth of 4 should be the caller of one of the
	'             * methods in java.lang.Class that invoke checkMember
	'             * access. The stack should look like:
	'             *
	'             * someCaller                        [3]
	'             * java.lang.Class.someReflectionAPI [2]
	'             * java.lang.Class.checkMemberAccess [1]
	'             * SecurityManager.checkMemberAccess [0]
	'             *
	'             
				If (stack.Length<4) OrElse (stack(3).classLoader IsNot clazz.classLoader) Then checkPermission(sun.security.util.SecurityConstants.CHECK_MEMBER_ACCESS_PERMISSION)
			End If
		End Sub

		''' <summary>
		''' Determines whether the permission with the specified permission target
		''' name should be granted or denied.
		''' 
		''' <p> If the requested permission is allowed, this method returns
		''' quietly. If denied, a SecurityException is raised.
		''' 
		''' <p> This method creates a <code>SecurityPermission</code> object for
		''' the given permission target name and calls <code>checkPermission</code>
		''' with it.
		''' 
		''' <p> See the documentation for
		''' <code><seealso cref="java.security.SecurityPermission"/></code> for
		''' a list of possible permission target names.
		''' 
		''' <p> If you override this method, then you should make a call to
		''' <code>super.checkSecurityAccess</code>
		''' at the point the overridden method would normally throw an
		''' exception.
		''' </summary>
		''' <param name="target"> the target name of the <code>SecurityPermission</code>.
		''' </param>
		''' <exception cref="SecurityException"> if the calling thread does not have
		''' permission for the requested access. </exception>
		''' <exception cref="NullPointerException"> if <code>target</code> is null. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>target</code> is empty.
		''' 
		''' @since   JDK1.1 </exception>
		''' <seealso cref=        #checkPermission(java.security.Permission) checkPermission </seealso>
		Public Overridable Sub checkSecurityAccess(ByVal target As String)
			checkPermission(New SecurityPermission(target))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Function currentLoadedClass0() As Class
		End Function

		''' <summary>
		''' Returns the thread group into which to instantiate any new
		''' thread being created at the time this is being called.
		''' By default, it returns the thread group of the current
		''' thread. This should be overridden by a specific security
		''' manager to return the appropriate thread group.
		''' </summary>
		''' <returns>  ThreadGroup that new threads are instantiated into
		''' @since   JDK1.1 </returns>
		''' <seealso cref=     java.lang.ThreadGroup </seealso>
		Public Overridable Property threadGroup As ThreadGroup
			Get
				Return Thread.CurrentThread.threadGroup
			End Get
		End Property

	End Class

End Namespace