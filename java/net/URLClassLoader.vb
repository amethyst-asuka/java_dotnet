Imports System.Collections.Generic

'
' * Copyright (c) 1997, 2015, Oracle and/or its affiliates. All rights reserved.
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
	''' This class loader is used to load classes and resources from a search
	''' path of URLs referring to both JAR files and directories. Any URL that
	''' ends with a '/' is assumed to refer to a directory. Otherwise, the URL
	''' is assumed to refer to a JAR file which will be opened as needed.
	''' <p>
	''' The AccessControlContext of the thread that created the instance of
	''' URLClassLoader will be used when subsequently loading classes and
	''' resources.
	''' <p>
	''' The classes that are loaded are by default granted permission only to
	''' access the URLs specified when the URLClassLoader was created.
	''' 
	''' @author  David Connelly
	''' @since   1.2
	''' </summary>
	Public Class URLClassLoader
		Inherits java.security.SecureClassLoader
		Implements java.io.Closeable

		' The search path for classes and resources 
		Private ReadOnly ucp As sun.misc.URLClassPath

		' The context to be used when loading classes and resources 
		Private ReadOnly acc As java.security.AccessControlContext

		''' <summary>
		''' Constructs a new URLClassLoader for the given URLs. The URLs will be
		''' searched in the order specified for classes and resources after first
		''' searching in the specified parent class loader. Any URL that ends with
		''' a '/' is assumed to refer to a directory. Otherwise, the URL is assumed
		''' to refer to a JAR file which will be downloaded and opened as needed.
		''' 
		''' <p>If there is a security manager, this method first
		''' calls the security manager's {@code checkCreateClassLoader} method
		''' to ensure creation of a class loader is allowed.
		''' </summary>
		''' <param name="urls"> the URLs from which to load classes and resources </param>
		''' <param name="parent"> the parent class loader for delegation </param>
		''' <exception cref="SecurityException">  if a security manager exists and its
		'''             {@code checkCreateClassLoader} method doesn't allow
		'''             creation of a class loader. </exception>
		''' <exception cref="NullPointerException"> if {@code urls} is {@code null}. </exception>
		''' <seealso cref= SecurityManager#checkCreateClassLoader </seealso>
		Public Sub New(ByVal urls As URL(), ByVal parent As ClassLoader)
			MyBase.New(parent)
			' this is to make the stack depth consistent with 1.1
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkCreateClassLoader()
			ucp = New sun.misc.URLClassPath(urls)
			Me.acc = java.security.AccessController.context
		End Sub

		Friend Sub New(ByVal urls As URL(), ByVal parent As ClassLoader, ByVal acc As java.security.AccessControlContext)
			MyBase.New(parent)
			' this is to make the stack depth consistent with 1.1
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkCreateClassLoader()
			ucp = New sun.misc.URLClassPath(urls)
			Me.acc = acc
		End Sub

		''' <summary>
		''' Constructs a new URLClassLoader for the specified URLs using the
		''' default delegation parent {@code ClassLoader}. The URLs will
		''' be searched in the order specified for classes and resources after
		''' first searching in the parent class loader. Any URL that ends with
		''' a '/' is assumed to refer to a directory. Otherwise, the URL is
		''' assumed to refer to a JAR file which will be downloaded and opened
		''' as needed.
		''' 
		''' <p>If there is a security manager, this method first
		''' calls the security manager's {@code checkCreateClassLoader} method
		''' to ensure creation of a class loader is allowed.
		''' </summary>
		''' <param name="urls"> the URLs from which to load classes and resources
		''' </param>
		''' <exception cref="SecurityException">  if a security manager exists and its
		'''             {@code checkCreateClassLoader} method doesn't allow
		'''             creation of a class loader. </exception>
		''' <exception cref="NullPointerException"> if {@code urls} is {@code null}. </exception>
		''' <seealso cref= SecurityManager#checkCreateClassLoader </seealso>
		Public Sub New(ByVal urls As URL())
			MyBase.New()
			' this is to make the stack depth consistent with 1.1
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkCreateClassLoader()
			ucp = New sun.misc.URLClassPath(urls)
			Me.acc = java.security.AccessController.context
		End Sub

		Friend Sub New(ByVal urls As URL(), ByVal acc As java.security.AccessControlContext)
			MyBase.New()
			' this is to make the stack depth consistent with 1.1
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkCreateClassLoader()
			ucp = New sun.misc.URLClassPath(urls)
			Me.acc = acc
		End Sub

		''' <summary>
		''' Constructs a new URLClassLoader for the specified URLs, parent
		''' class loader, and URLStreamHandlerFactory. The parent argument
		''' will be used as the parent class loader for delegation. The
		''' factory argument will be used as the stream handler factory to
		''' obtain protocol handlers when creating new jar URLs.
		''' 
		''' <p>If there is a security manager, this method first
		''' calls the security manager's {@code checkCreateClassLoader} method
		''' to ensure creation of a class loader is allowed.
		''' </summary>
		''' <param name="urls"> the URLs from which to load classes and resources </param>
		''' <param name="parent"> the parent class loader for delegation </param>
		''' <param name="factory"> the URLStreamHandlerFactory to use when creating URLs
		''' </param>
		''' <exception cref="SecurityException">  if a security manager exists and its
		'''             {@code checkCreateClassLoader} method doesn't allow
		'''             creation of a class loader. </exception>
		''' <exception cref="NullPointerException"> if {@code urls} is {@code null}. </exception>
		''' <seealso cref= SecurityManager#checkCreateClassLoader </seealso>
		Public Sub New(ByVal urls As URL(), ByVal parent As ClassLoader, ByVal factory As URLStreamHandlerFactory)
			MyBase.New(parent)
			' this is to make the stack depth consistent with 1.1
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkCreateClassLoader()
			ucp = New sun.misc.URLClassPath(urls, factory)
			acc = java.security.AccessController.context
		End Sub

	'     A map (used as a set) to keep track of closeable local resources
	'     * (either JarFiles or FileInputStreams). We don't care about
	'     * Http resources since they don't need to be closed.
	'     *
	'     * If the resource is coming from a jar file
	'     * we keep a (weak) reference to the JarFile object which can
	'     * be closed if URLClassLoader.close() called. Due to jar file
	'     * caching there will typically be only one JarFile object
	'     * per underlying jar file.
	'     *
	'     * For file resources, which is probably a less common situation
	'     * we have to keep a weak reference to each stream.
	'     

		Private closeables As New java.util.WeakHashMap(Of java.io.Closeable, Void)

		''' <summary>
		''' Returns an input stream for reading the specified resource.
		''' If this loader is closed, then any resources opened by this method
		''' will be closed.
		''' 
		''' <p> The search order is described in the documentation for {@link
		''' #getResource(String)}.  </p>
		''' </summary>
		''' <param name="name">
		'''         The resource name
		''' </param>
		''' <returns>  An input stream for reading the resource, or {@code null}
		'''          if the resource could not be found
		''' 
		''' @since  1.7 </returns>
		Public Overrides Function getResourceAsStream(ByVal name As String) As java.io.InputStream
			Dim url As URL = getResource(name)
			Try
				If url Is Nothing Then Return Nothing
				Dim urlc As URLConnection = url.openConnection()
				Dim [is] As java.io.InputStream = urlc.inputStream
				If TypeOf urlc Is JarURLConnection Then
					Dim juc As JarURLConnection = CType(urlc, JarURLConnection)
					Dim jar As java.util.jar.JarFile = juc.jarFile
					SyncLock closeables
						If Not closeables.containsKey(jar) Then closeables.put(jar, Nothing)
					End SyncLock
				ElseIf TypeOf urlc Is sun.net.www.protocol.file.FileURLConnection Then
					SyncLock closeables
						closeables.put([is], Nothing)
					End SyncLock
				End If
				Return [is]
			Catch e As java.io.IOException
				Return Nothing
			End Try
		End Function

	   ''' <summary>
	   ''' Closes this URLClassLoader, so that it can no longer be used to load
	   ''' new classes or resources that are defined by this loader.
	   ''' Classes and resources defined by any of this loader's parents in the
	   ''' delegation hierarchy are still accessible. Also, any classes or resources
	   ''' that are already loaded, are still accessible.
	   ''' <p>
	   ''' In the case of jar: and file: URLs, it also closes any files
	   ''' that were opened by it. If another thread is loading a
	   ''' class when the {@code close} method is invoked, then the result of
	   ''' that load is undefined.
	   ''' <p>
	   ''' The method makes a best effort attempt to close all opened files,
	   ''' by catching <seealso cref="IOException"/>s internally. Unchecked exceptions
	   ''' and errors are not caught. Calling close on an already closed
	   ''' loader has no effect.
	   ''' <p> </summary>
	   ''' <exception cref="IOException"> if closing any file opened by this class loader
	   ''' resulted in an IOException. Any such exceptions are caught internally.
	   ''' If only one is caught, then it is re-thrown. If more than one exception
	   ''' is caught, then the second and following exceptions are added
	   ''' as suppressed exceptions of the first one caught, which is then re-thrown.
	   ''' </exception>
	   ''' <exception cref="SecurityException"> if a security manager is set, and it denies
	   '''   <seealso cref="RuntimePermission"/>{@code ("closeClassLoader")}
	   ''' 
	   ''' @since 1.7 </exception>
		Public Overridable Sub close()
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkPermission(New RuntimePermission("closeClassLoader"))
			Dim errors As IList(Of java.io.IOException) = ucp.closeLoaders()

			' now close any remaining streams.

			SyncLock closeables
				Dim keys As java.util.WeakHashMap(Of java.io.Closeable, Void).KeyCollection = closeables.Keys
				For Each c As java.io.Closeable In keys
					Try
						c.close()
					Catch ioex As java.io.IOException
						errors.Add(ioex)
					End Try
				Next c
				closeables.clear()
			End SyncLock

			If errors.Count = 0 Then Return

			Dim firstex As java.io.IOException = errors.Remove(0)

			' Suppress any remaining exceptions

			For Each error_Renamed As java.io.IOException In errors
				firstex.addSuppressed(error_Renamed)
			Next [error]
			Throw firstex
		End Sub

		''' <summary>
		''' Appends the specified URL to the list of URLs to search for
		''' classes and resources.
		''' <p>
		''' If the URL specified is {@code null} or is already in the
		''' list of URLs, or if this loader is closed, then invoking this
		''' method has no effect.
		''' </summary>
		''' <param name="url"> the URL to be added to the search path of URLs </param>
		Protected Friend Overridable Sub addURL(ByVal url As URL)
			ucp.addURL(url)
		End Sub

		''' <summary>
		''' Returns the search path of URLs for loading classes and resources.
		''' This includes the original list of URLs specified to the constructor,
		''' along with any URLs subsequently appended by the addURL() method. </summary>
		''' <returns> the search path of URLs for loading classes and resources. </returns>
		Public Overridable Property uRLs As URL()
			Get
				Return ucp.uRLs
			End Get
		End Property

		''' <summary>
		''' Finds and loads the class with the specified name from the URL search
		''' path. Any URLs referring to JAR files are loaded and opened as needed
		''' until the class is found.
		''' </summary>
		''' <param name="name"> the name of the class </param>
		''' <returns> the resulting class </returns>
		''' <exception cref="ClassNotFoundException"> if the class could not be found,
		'''            or if the loader is closed. </exception>
		''' <exception cref="NullPointerException"> if {@code name} is {@code null}. </exception>
		Protected Friend Overrides Function findClass(ByVal name As String) As Class
			Dim result As Class
			Try
				result = java.security.AccessController.doPrivileged(New PrivilegedExceptionActionAnonymousInnerClassHelper(Of T)
			Catch pae As java.security.PrivilegedActionException
				Throw CType(pae.exception, ClassNotFoundException)
			End Try
			If result Is Nothing Then Throw New ClassNotFoundException(name)
			Return result
		End Function

		Private Class PrivilegedExceptionActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedExceptionAction(Of T)

			Public Overridable Function run() As Class
				Dim path As String = name.replace("."c, "/"c).concat(".class")
				Dim res As sun.misc.Resource = outerInstance.ucp.getResource(path, False)
				If res IsNot Nothing Then
					Try
						Return outerInstance.defineClass(name, res)
					Catch e As java.io.IOException
						Throw New ClassNotFoundException(name, e)
					End Try
				Else
					Return Nothing
				End If
			End Function
		End Class

	'    
	'     * Retrieve the package using the specified package name.
	'     * If non-null, verify the package using the specified code
	'     * source and manifest.
	'     
		Private Function getAndVerifyPackage(ByVal pkgname As String, ByVal man As java.util.jar.Manifest, ByVal url As URL) As Package
			Dim pkg As Package = getPackage(pkgname)
			If pkg IsNot Nothing Then
				' Package found, so check package sealing.
				If pkg.sealed Then
					' Verify that code source URL is the same.
					If Not pkg.isSealed(url) Then Throw New SecurityException("sealing violation: package " & pkgname & " is sealed")
				Else
					' Make sure we are not attempting to seal the package
					' at this code source URL.
					If (man IsNot Nothing) AndAlso isSealed(pkgname, man) Then Throw New SecurityException("sealing violation: can't seal package " & pkgname & ": already loaded")
				End If
			End If
			Return pkg
		End Function

		' Also called by VM to define Package for classes loaded from the CDS
		' archive
		Private Sub definePackageInternal(ByVal pkgname As String, ByVal man As java.util.jar.Manifest, ByVal url As URL)
			If getAndVerifyPackage(pkgname, man, url) Is Nothing Then
				Try
					If man IsNot Nothing Then
						definePackage(pkgname, man, url)
					Else
						definePackage(pkgname, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing)
					End If
				Catch iae As IllegalArgumentException
					' parallel-capable class loaders: re-verify in case of a
					' race condition
					If getAndVerifyPackage(pkgname, man, url) Is Nothing Then Throw New AssertionError("Cannot find package " & pkgname)
				End Try
			End If
		End Sub

	'    
	'     * Defines a Class using the class bytes obtained from the specified
	'     * Resource. The resulting Class must be resolved before it can be
	'     * used.
	'     
		Private Function defineClass(ByVal name As String, ByVal res As sun.misc.Resource) As Class
			Dim t0 As Long = System.nanoTime()
			Dim i As Integer = name.LastIndexOf("."c)
			Dim url As URL = res.codeSourceURL
			If i <> -1 Then
				Dim pkgname As String = name.Substring(0, i)
				' Check if package already loaded.
				Dim man As java.util.jar.Manifest = res.manifest
				definePackageInternal(pkgname, man, url)
			End If
			' Now read the class bytes and define the class
			Dim bb As java.nio.ByteBuffer = res.byteBuffer
			If bb IsNot Nothing Then
				' Use (direct) ByteBuffer:
				Dim signers_Renamed As java.security.CodeSigner() = res.codeSigners
				Dim cs As New java.security.CodeSource(url, signers_Renamed)
				sun.misc.PerfCounter.readClassBytesTime.addElapsedTimeFrom(t0)
				Return defineClass(name, bb, cs)
			Else
				Dim b As SByte() = res.bytes
				' must read certificates AFTER reading bytes.
				Dim signers_Renamed As java.security.CodeSigner() = res.codeSigners
				Dim cs As New java.security.CodeSource(url, signers_Renamed)
				sun.misc.PerfCounter.readClassBytesTime.addElapsedTimeFrom(t0)
				Return defineClass(name, b, 0, b.Length, cs)
			End If
		End Function

		''' <summary>
		''' Defines a new package by name in this ClassLoader. The attributes
		''' contained in the specified Manifest will be used to obtain package
		''' version and sealing information. For sealed packages, the additional
		''' URL specifies the code source URL from which the package was loaded.
		''' </summary>
		''' <param name="name">  the package name </param>
		''' <param name="man">   the Manifest containing package version and sealing
		'''              information </param>
		''' <param name="url">   the code source url for the package, or null if none </param>
		''' <exception cref="IllegalArgumentException"> if the package name duplicates
		'''              an existing package either in this class loader or one
		'''              of its ancestors </exception>
		''' <returns> the newly defined Package object </returns>
		Protected Friend Overridable Function definePackage(ByVal name As String, ByVal man As java.util.jar.Manifest, ByVal url As URL) As Package
			Dim path As String = name.replace("."c, "/"c) & "/"
			Dim specTitle As String = Nothing, specVersion As String = Nothing, specVendor As String = Nothing
			Dim implTitle As String = Nothing, implVersion As String = Nothing, implVendor As String = Nothing
			Dim sealed_Renamed As String = Nothing
			Dim sealBase As URL = Nothing

			Dim attr As java.util.jar.Attributes = man.getAttributes(path)
			If attr IsNot Nothing Then
				specTitle = attr.getValue(java.util.jar.Attributes.Name.SPECIFICATION_TITLE)
				specVersion = attr.getValue(java.util.jar.Attributes.Name.SPECIFICATION_VERSION)
				specVendor = attr.getValue(java.util.jar.Attributes.Name.SPECIFICATION_VENDOR)
				implTitle = attr.getValue(java.util.jar.Attributes.Name.IMPLEMENTATION_TITLE)
				implVersion = attr.getValue(java.util.jar.Attributes.Name.IMPLEMENTATION_VERSION)
				implVendor = attr.getValue(java.util.jar.Attributes.Name.IMPLEMENTATION_VENDOR)
				sealed_Renamed = attr.getValue(java.util.jar.Attributes.Name.SEALED)
			End If
			attr = man.mainAttributes
			If attr IsNot Nothing Then
				If specTitle Is Nothing Then specTitle = attr.getValue(java.util.jar.Attributes.Name.SPECIFICATION_TITLE)
				If specVersion Is Nothing Then specVersion = attr.getValue(java.util.jar.Attributes.Name.SPECIFICATION_VERSION)
				If specVendor Is Nothing Then specVendor = attr.getValue(java.util.jar.Attributes.Name.SPECIFICATION_VENDOR)
				If implTitle Is Nothing Then implTitle = attr.getValue(java.util.jar.Attributes.Name.IMPLEMENTATION_TITLE)
				If implVersion Is Nothing Then implVersion = attr.getValue(java.util.jar.Attributes.Name.IMPLEMENTATION_VERSION)
				If implVendor Is Nothing Then implVendor = attr.getValue(java.util.jar.Attributes.Name.IMPLEMENTATION_VENDOR)
				If sealed_Renamed Is Nothing Then sealed_Renamed = attr.getValue(java.util.jar.Attributes.Name.SEALED)
			End If
			If "true".equalsIgnoreCase(sealed_Renamed) Then sealBase = url
			Return definePackage(name, specTitle, specVersion, specVendor, implTitle, implVersion, implVendor, sealBase)
		End Function

	'    
	'     * Returns true if the specified package name is sealed according to the
	'     * given manifest.
	'     
		Private Function isSealed(ByVal name As String, ByVal man As java.util.jar.Manifest) As Boolean
			Dim path As String = name.replace("."c, "/"c) & "/"
			Dim attr As java.util.jar.Attributes = man.getAttributes(path)
			Dim sealed_Renamed As String = Nothing
			If attr IsNot Nothing Then sealed_Renamed = attr.getValue(java.util.jar.Attributes.Name.SEALED)
			If sealed_Renamed Is Nothing Then
				attr = man.mainAttributes
				If attr IsNot Nothing Then sealed_Renamed = attr.getValue(java.util.jar.Attributes.Name.SEALED)
			End If
			Return "true".equalsIgnoreCase(sealed_Renamed)
		End Function

		''' <summary>
		''' Finds the resource with the specified name on the URL search path.
		''' </summary>
		''' <param name="name"> the name of the resource </param>
		''' <returns> a {@code URL} for the resource, or {@code null}
		''' if the resource could not be found, or if the loader is closed. </returns>
		Public Overrides Function findResource(ByVal name As String) As URL
	'        
	'         * The same restriction to finding classes applies to resources
	'         
			Dim url As URL = java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)

			Return If(url IsNot Nothing, ucp.checkURL(url), Nothing)
		End Function

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As URL
				Return outerInstance.ucp.findResource(name, True)
			End Function
		End Class

		''' <summary>
		''' Returns an Enumeration of URLs representing all of the resources
		''' on the URL search path having the specified name.
		''' </summary>
		''' <param name="name"> the resource name </param>
		''' <exception cref="IOException"> if an I/O exception occurs </exception>
		''' <returns> an {@code Enumeration} of {@code URL}s
		'''         If the loader is closed, the Enumeration will be empty. </returns>
		Public Overrides Function findResources(ByVal name As String) As System.Collections.IEnumerator(Of URL)
			Dim e As System.Collections.IEnumerator(Of URL) = ucp.findResources(name, True)

			Return New EnumerationAnonymousInnerClassHelper(Of E)
		End Function

		Private Class EnumerationAnonymousInnerClassHelper(Of E)
			Implements System.Collections.IEnumerator(Of E)

			Private url As URL = Nothing

			Private Function [next]() As Boolean
				If url IsNot Nothing Then Return True
				Do
					Dim u As URL = java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper2(Of T)
					If u Is Nothing Then Exit Do
					url = outerInstance.ucp.checkURL(u)
				Loop While url Is Nothing
				Return url IsNot Nothing
			End Function

			Private Class PrivilegedActionAnonymousInnerClassHelper2(Of T)
				Implements java.security.PrivilegedAction(Of T)

				Public Overridable Function run() As URL
					If Not e.hasMoreElements() Then Return Nothing
					Return e.nextElement()
				End Function
			End Class

			Public Overridable Function nextElement() As URL
				If Not next() Then Throw New java.util.NoSuchElementException
				Dim u As URL = url
				url = Nothing
				Return u
			End Function

			Public Overridable Function hasMoreElements() As Boolean
				Return next()
			End Function
		End Class

		''' <summary>
		''' Returns the permissions for the given codesource object.
		''' The implementation of this method first calls super.getPermissions
		''' and then adds permissions based on the URL of the codesource.
		''' <p>
		''' If the protocol of this URL is "jar", then the permission granted
		''' is based on the permission that is required by the URL of the Jar
		''' file.
		''' <p>
		''' If the protocol is "file" and there is an authority component, then
		''' permission to connect to and accept connections from that authority
		''' may be granted. If the protocol is "file"
		''' and the path specifies a file, then permission to read that
		''' file is granted. If protocol is "file" and the path is
		''' a directory, permission is granted to read all files
		''' and (recursively) all files and subdirectories contained in
		''' that directory.
		''' <p>
		''' If the protocol is not "file", then permission
		''' to connect to and accept connections from the URL's host is granted. </summary>
		''' <param name="codesource"> the codesource </param>
		''' <exception cref="NullPointerException"> if {@code codesource} is {@code null}. </exception>
		''' <returns> the permissions granted to the codesource </returns>
		Protected Friend Overridable Function getPermissions(ByVal codesource As java.security.CodeSource) As java.security.PermissionCollection
			Dim perms As java.security.PermissionCollection = MyBase.getPermissions(codesource)

			Dim url As URL = codesource.location

			Dim p As java.security.Permission
			Dim urlConnection As URLConnection

			Try
				urlConnection = url.openConnection()
				p = urlConnection.permission
			Catch ioe As java.io.IOException
				p = Nothing
				urlConnection = Nothing
			End Try

			If TypeOf p Is java.io.FilePermission Then
				' if the permission has a separator char on the end,
				' it means the codebase is a directory, and we need
				' to add an additional permission to read recursively
				Dim path As String = p.name
				If path.EndsWith(File.separator) Then
					path &= "-"
					p = New java.io.FilePermission(path, sun.security.util.SecurityConstants.FILE_READ_ACTION)
				End If
			ElseIf (p Is Nothing) AndAlso (url.protocol.Equals("file")) Then
				Dim path As String = url.file.replace("/"c, System.IO.Path.DirectorySeparatorChar)
				path = sun.net.www.ParseUtil.decode(path)
				If path.EndsWith(File.separator) Then path &= "-"
				p = New java.io.FilePermission(path, sun.security.util.SecurityConstants.FILE_READ_ACTION)
			Else
				''' <summary>
				''' Not loading from a 'file:' URL so we want to give the class
				''' permission to connect to and accept from the remote host
				''' after we've made sure the host is the correct one and is valid.
				''' </summary>
				Dim locUrl As URL = url
				If TypeOf urlConnection Is JarURLConnection Then locUrl = CType(urlConnection, JarURLConnection).jarFileURL
				Dim host As String = locUrl.host
				If host IsNot Nothing AndAlso (host.length() > 0) Then p = New SocketPermission(host, sun.security.util.SecurityConstants.SOCKET_CONNECT_ACCEPT_ACTION)
			End If

			' make sure the person that created this class loader
			' would have this permission

			If p IsNot Nothing Then
				Dim sm As SecurityManager = System.securityManager
				If sm IsNot Nothing Then
					Dim fp As java.security.Permission = p
					java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper3(Of T)
				End If
				perms.add(p)
			End If
			Return perms
		End Function

		Private Class PrivilegedActionAnonymousInnerClassHelper3(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As Void
				sm.checkPermission(fp)
				Return Nothing
			End Function
		End Class

		''' <summary>
		''' Creates a new instance of URLClassLoader for the specified
		''' URLs and parent class loader. If a security manager is
		''' installed, the {@code loadClass} method of the URLClassLoader
		''' returned by this method will invoke the
		''' {@code SecurityManager.checkPackageAccess} method before
		''' loading the class.
		''' </summary>
		''' <param name="urls"> the URLs to search for classes and resources </param>
		''' <param name="parent"> the parent class loader for delegation </param>
		''' <exception cref="NullPointerException"> if {@code urls} is {@code null}. </exception>
		''' <returns> the resulting class loader </returns>
		Public Shared Function newInstance(ByVal urls As URL(), ByVal parent As ClassLoader) As URLClassLoader
			' Save the caller's context
			Dim acc As java.security.AccessControlContext = java.security.AccessController.context
			' Need a privileged block to create the class loader
			Dim ucl As URLClassLoader = java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper4(Of T)
			Return ucl
		End Function

		Private Class PrivilegedActionAnonymousInnerClassHelper4(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As URLClassLoader
				Return New FactoryURLClassLoader(urls, parent, outerInstance.acc)
			End Function
		End Class

		''' <summary>
		''' Creates a new instance of URLClassLoader for the specified
		''' URLs and default parent class loader. If a security manager is
		''' installed, the {@code loadClass} method of the URLClassLoader
		''' returned by this method will invoke the
		''' {@code SecurityManager.checkPackageAccess} before
		''' loading the class.
		''' </summary>
		''' <param name="urls"> the URLs to search for classes and resources </param>
		''' <exception cref="NullPointerException"> if {@code urls} is {@code null}. </exception>
		''' <returns> the resulting class loader </returns>
		Public Shared Function newInstance(ByVal urls As URL()) As URLClassLoader
			' Save the caller's context
			Dim acc As java.security.AccessControlContext = java.security.AccessController.context
			' Need a privileged block to create the class loader
			Dim ucl As URLClassLoader = java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper5(Of T)
			Return ucl
		End Function

		Private Class PrivilegedActionAnonymousInnerClassHelper5(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As URLClassLoader
				Return New FactoryURLClassLoader(urls, outerInstance.acc)
			End Function
		End Class

		Shared Sub New()
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			sun.misc.SharedSecrets.setJavaNetAccess(New sun.misc.JavaNetAccess()
	'		{
	'				public URLClassPath getURLClassPath(URLClassLoader u)
	'				{
	'					Return u.ucp;
	'				}
	'
	'				public String getOriginalHostName(InetAddress ia)
	'				{
	'					Return ia.holder.getOriginalHostName();
	'				}
	'			}
		   )
			ClassLoader.registerAsParallelCapable()
		End Sub
	End Class

	Friend NotInheritable Class FactoryURLClassLoader
		Inherits URLClassLoader

		Shared Sub New()
			ClassLoader.registerAsParallelCapable()
		End Sub

		Friend Sub New(ByVal urls As URL(), ByVal parent As ClassLoader, ByVal acc As java.security.AccessControlContext)
			MyBase.New(urls, parent, acc)
		End Sub

		Friend Sub New(ByVal urls As URL(), ByVal acc As java.security.AccessControlContext)
			MyBase.New(urls, acc)
		End Sub

		Public NotOverridable Overrides Function loadClass(ByVal name As String, ByVal resolve As Boolean) As Class
			' First check if we have permission to access the package. This
			' should go away once we've added support for exported packages.
			Dim sm As SecurityManager = System.securityManager
			If sm IsNot Nothing Then
				Dim i As Integer = name.LastIndexOf("."c)
				If i <> -1 Then sm.checkPackageAccess(name.Substring(0, i))
			End If
			Return MyBase.loadClass(name, resolve)
		End Function
	End Class

End Namespace