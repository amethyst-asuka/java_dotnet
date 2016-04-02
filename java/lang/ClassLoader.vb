Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System.Collections.Concurrent
Imports System.Threading
Imports System.Runtime.InteropServices

'
' * Copyright (c) 2013, 2015, Oracle and/or its affiliates. All rights reserved.
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
	''' A class loader is an object that is responsible for loading classes. The
	''' class <tt>ClassLoader</tt> is an abstract class.  Given the <a
	''' href="#name">binary name</a> of a [Class], a class loader should attempt to
	''' locate or generate data that constitutes a definition for the class.  A
	''' typical strategy is to transform the name into a file name and then read a
	''' "class file" of that name from a file system.
	''' 
	''' <p> Every <seealso cref="Class <tt>Class</tt>"/> object contains a {@link
	''' Class#getClassLoader() reference} to the <tt>ClassLoader</tt> that defined
	''' it.
	''' 
	''' <p> <tt>Class</tt> objects for array classes are not created by class
	''' loaders, but are created automatically as required by the Java runtime.
	''' The class loader for an array [Class], as returned by {@link
	''' Class#getClassLoader()} is the same as the class loader for its element
	''' type; if the element type is a primitive type, then the array class has no
	''' class loader.
	''' 
	''' <p> Applications implement subclasses of <tt>ClassLoader</tt> in order to
	''' extend the manner in which the Java virtual machine dynamically loads
	''' classes.
	''' 
	''' <p> Class loaders may typically be used by security managers to indicate
	''' security domains.
	''' 
	''' <p> The <tt>ClassLoader</tt> class uses a delegation model to search for
	''' classes and resources.  Each instance of <tt>ClassLoader</tt> has an
	''' associated parent class loader.  When requested to find a class or
	''' resource, a <tt>ClassLoader</tt> instance will delegate the search for the
	''' class or resource to its parent class loader before attempting to find the
	''' class or resource itself.  The virtual machine's built-in class loader,
	''' called the "bootstrap class loader", does not itself have a parent but may
	''' serve as the parent of a <tt>ClassLoader</tt> instance.
	''' 
	''' <p> Class loaders that support concurrent loading of classes are known as
	''' <em>parallel capable</em> class loaders and are required to register
	''' themselves at their class initialization time by invoking the
	''' {@link
	''' #registerAsParallelCapable <tt>ClassLoader.registerAsParallelCapable</tt>}
	''' method. Note that the <tt>ClassLoader</tt> class is registered as parallel
	''' capable by default. However, its subclasses still need to register themselves
	''' if they are parallel capable. <br>
	''' In environments in which the delegation model is not strictly
	''' hierarchical, class loaders need to be parallel capable, otherwise class
	''' loading can lead to deadlocks because the loader lock is held for the
	''' duration of the class loading process (see {@link #loadClass
	''' <tt>loadClass</tt>} methods).
	''' 
	''' <p> Normally, the Java virtual machine loads classes from the local file
	''' system in a platform-dependent manner.  For example, on UNIX systems, the
	''' virtual machine loads classes from the directory defined by the
	''' <tt>CLASSPATH</tt> environment variable.
	''' 
	''' <p> However, some classes may not originate from a file; they may originate
	''' from other sources, such as the network, or they could be constructed by an
	''' application.  The method {@link #defineClass(String, byte[], int, int)
	''' <tt>defineClass</tt>} converts an array of bytes into an instance of class
	''' <tt>Class</tt>. Instances of this newly defined class can be created using
	''' <seealso cref="Class#newInstance <tt>Class.newInstance</tt>"/>.
	''' 
	''' <p> The methods and constructors of objects created by a class loader may
	''' reference other classes.  To determine the class(es) referred to, the Java
	''' virtual machine invokes the <seealso cref="#loadClass <tt>loadClass</tt>"/> method of
	''' the class loader that originally created the class.
	''' 
	''' <p> For example, an application could create a network class loader to
	''' download class files from a server.  Sample code might look like:
	''' 
	''' <blockquote><pre>
	'''   ClassLoader loader&nbsp;= new NetworkClassLoader(host,&nbsp;port);
	'''   Object main&nbsp;= loader.loadClass("Main", true).newInstance();
	'''       &nbsp;.&nbsp;.&nbsp;.
	''' </pre></blockquote>
	''' 
	''' <p> The network class loader subclass must define the methods {@link
	''' #findClass <tt>findClass</tt>} and <tt>loadClassData</tt> to load a class
	''' from the network.  Once it has downloaded the bytes that make up the [Class],
	''' it should use the method <seealso cref="#defineClass <tt>defineClass</tt>"/> to
	''' create a class instance.  A sample implementation is:
	''' 
	''' <blockquote><pre>
	'''     class NetworkClassLoader extends ClassLoader {
	'''         String host;
	'''         int port;
	''' 
	'''         public Class findClass(String name) {
	'''             byte[] b = loadClassData(name);
	'''             return defineClass(name, b, 0, b.length);
	'''         }
	''' 
	'''         private byte[] loadClassData(String name) {
	'''             // load the class data from the connection
	'''             &nbsp;.&nbsp;.&nbsp;.
	'''         }
	'''     }
	''' </pre></blockquote>
	''' 
	''' <h3> <a name="name">Binary names</a> </h3>
	''' 
	''' <p> Any class name provided as a <seealso cref="String"/> parameter to methods in
	''' <tt>ClassLoader</tt> must be a binary name as defined by
	''' <cite>The Java&trade; Language Specification</cite>.
	''' 
	''' <p> Examples of valid class names include:
	''' <blockquote><pre>
	'''   "java.lang.String"
	'''   "javax.swing.JSpinner$DefaultEditor"
	'''   "java.security.KeyStore$Builder$FileBuilder$1"
	'''   "java.net.URLClassLoader$3$1"
	''' </pre></blockquote>
	''' </summary>
	''' <seealso cref=      #resolveClass(Class)
	''' @since 1.0 </seealso>
	Public MustInherit Class ClassLoader

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub registerNatives()
		End Sub
		Shared Sub New()
			registerNatives()
				SyncLock loaderTypes
					loaderTypes.add(GetType(ClassLoader))
				End SyncLock
		End Sub

		' The parent class loader for delegation
		' Note: VM hardcoded the offset of this field, thus all new fields
		' must be added *after* it.
		Private ReadOnly parent As  ClassLoader

		''' <summary>
		''' Encapsulates the set of parallel capable loader types.
		''' </summary>
		Private Class ParallelLoaders
			Private Sub New()
			End Sub

			' the set of parallel capable loader types
			Private Shared ReadOnly loaderTypes As java.util.Set(Of [Class]) = java.util.Collections.newSetFromMap(New java.util.WeakHashMap(Of [Class], Boolean?))

			''' <summary>
			''' Registers the given class loader type as parallel capabale.
			''' Returns {@code true} is successfully registered; {@code false} if
			''' loader's super class is not registered.
			''' </summary>
			Friend Shared Function register(ByVal c As [Class]) As Boolean
				SyncLock loaderTypes
					If loaderTypes.contains(c.BaseType) Then
						' register the class loader as parallel capable
						' if and only if all of its super classes are.
						' Note: given current classloading sequence, if
						' the immediate super class is parallel capable,
						' all the super classes higher up must be too.
						loaderTypes.add(c)
						Return True
					Else
						Return False
					End If
				End SyncLock
			End Function

			''' <summary>
			''' Returns {@code true} if the given class loader type is
			''' registered as parallel capable.
			''' </summary>
			Friend Shared Function isRegistered(ByVal c As [Class]) As Boolean
				SyncLock loaderTypes
					Return loaderTypes.contains(c)
				End SyncLock
			End Function
		End Class

		' Maps class name to the corresponding lock object when the current
		' class loader is parallel capable.
		' Note: VM also uses this field to decide if the current class loader
		' is parallel capable and the appropriate lock object for class loading.
		Private ReadOnly parallelLockMap As ConcurrentDictionary(Of String, Object)

		' Hashtable that maps packages to certs
		Private ReadOnly package2certs As IDictionary(Of String, java.security.cert.Certificate())

		' Shared among all packages with unsigned classes
		Private Shared ReadOnly nocerts As java.security.cert.Certificate() = New java.security.cert.Certificate(){}

		' The classes loaded by this class loader. The only purpose of this table
		' is to keep the classes from being GC'ed until the loader is GC'ed.
		Private ReadOnly classes As New List(Of [Class])

		' The "default" domain. Set as the default ProtectionDomain on newly
		' created classes.
		Private ReadOnly defaultDomain As New java.security.ProtectionDomain(New java.security.CodeSource(Nothing, CType(Nothing, java.security.cert.Certificate())), Nothing, Me, Nothing)

		' The initiating protection domains for all classes loaded by this loader
		Private ReadOnly domains As java.util.Set(Of java.security.ProtectionDomain)

		' Invoked by the VM to record every loaded class with this loader.
		Friend Overridable Sub addClass(ByVal c As [Class])
			classes.Add(c)
		End Sub

		' The packages defined in this class loader.  Each package name is mapped
		' to its corresponding Package object.
		' @GuardedBy("itself")
		Private ReadOnly packages As New Dictionary(Of String, Package)

		Private Shared Function checkCreateClassLoader() As Void
			Dim security As SecurityManager = System.securityManager
			If security IsNot Nothing Then security.checkCreateClassLoader()
			Return Nothing
		End Function

		Private Sub New(ByVal unused As Void, ByVal parent As  ClassLoader)
			Me.parent = parent
			If ParallelLoaders.isRegistered(Me.GetType()) Then
				parallelLockMap = New ConcurrentDictionary(Of )
			package2certs = New ConcurrentDictionary(Of )
				domains = java.util.Collections.synchronizedSet(New HashSet(Of java.security.ProtectionDomain))
				assertionLock = New Object
			Else
				' no finer-grained lock; lock on the classloader instance
				parallelLockMap = Nothing
			package2certs = New Dictionary(Of )
				domains = New HashSet(Of )
				assertionLock = Me
			End If
		End Sub

		''' <summary>
		''' Creates a new class loader using the specified parent class loader for
		''' delegation.
		''' 
		''' <p> If there is a security manager, its {@link
		''' SecurityManager#checkCreateClassLoader()
		''' <tt>checkCreateClassLoader</tt>} method is invoked.  This may result in
		''' a security exception.  </p>
		''' </summary>
		''' <param name="parent">
		'''         The parent class loader
		''' </param>
		''' <exception cref="SecurityException">
		'''          If a security manager exists and its
		'''          <tt>checkCreateClassLoader</tt> method doesn't allow creation
		'''          of a new class loader.
		''' 
		''' @since  1.2 </exception>
		Protected Friend Sub New(ByVal parent As  ClassLoader)
			Me.New(checkCreateClassLoader(), parent)
		End Sub

		''' <summary>
		''' Creates a new class loader using the <tt>ClassLoader</tt> returned by
		''' the method {@link #getSystemClassLoader()
		''' <tt>getSystemClassLoader()</tt>} as the parent class loader.
		''' 
		''' <p> If there is a security manager, its {@link
		''' SecurityManager#checkCreateClassLoader()
		''' <tt>checkCreateClassLoader</tt>} method is invoked.  This may result in
		''' a security exception.  </p>
		''' </summary>
		''' <exception cref="SecurityException">
		'''          If a security manager exists and its
		'''          <tt>checkCreateClassLoader</tt> method doesn't allow creation
		'''          of a new class loader. </exception>
		Protected Friend Sub New()
			Me.New(checkCreateClassLoader(), systemClassLoader)
		End Sub

		' -- Class --

		''' <summary>
		''' Loads the class with the specified <a href="#name">binary name</a>.
		''' This method searches for classes in the same manner as the {@link
		''' #loadClass(String, boolean)} method.  It is invoked by the Java virtual
		''' machine to resolve class references.  Invoking this method is equivalent
		''' to invoking {@link #loadClass(String, boolean) <tt>loadClass(name,
		''' false)</tt>}.
		''' </summary>
		''' <param name="name">
		'''         The <a href="#name">binary name</a> of the class
		''' </param>
		''' <returns>  The resulting <tt>Class</tt> object
		''' </returns>
		''' <exception cref="ClassNotFoundException">
		'''          If the class was not found </exception>
		Public Overridable Function loadClass(ByVal name As String) As  [Class]
			Return loadClass(name, False)
		End Function

        ''' <summary>
        ''' Loads the class with the specified <a href="#name">binary name</a>.  The
        ''' default implementation of this method searches for classes in the
        ''' following order:
        ''' 
        ''' <ol>
        ''' 
        '''   <li><p> Invoke <seealso cref="#findLoadedClass(String)"/> to check if the class
        '''   has already been loaded.  </p></li>
        ''' 
        '''   <li><p> Invoke the <seealso cref="#loadClass(String) <tt>loadClass</tt>"/> method
        '''   on the parent class loader.  If the parent is <tt>null</tt> the class
        '''   loader built-in to the virtual machine is used, instead.  </p></li>
        ''' 
        '''   <li><p> Invoke the <seealso cref="#findClass(String)"/> method to find the
        '''   class.  </p></li>
        ''' 
        ''' </ol>
        ''' 
        ''' <p> If the class was found using the above steps, and the
        ''' <tt>resolve</tt> flag is true, this method will then invoke the {@link
        ''' #resolveClass(Class)} method on the resulting <tt>Class</tt> object.
        ''' 
        ''' <p> Subclasses of <tt>ClassLoader</tt> are encouraged to override {@link
        ''' #findClass(String)}, rather than this method.  </p>
        ''' 
        ''' <p> Unless overridden, this method synchronizes on the result of
        ''' <seealso cref="#getClassLoadingLock <tt>getClassLoadingLock</tt>"/> method
        ''' during the entire class loading process.
        ''' </summary>
        ''' <param name="name">
        '''         The <a href="#name">binary name</a> of the class
        ''' </param>
        ''' <param name="resolve">
        '''         If <tt>true</tt> then resolve the class
        ''' </param>
        ''' <returns>  The resulting <tt>Class</tt> object
        ''' </returns>
        ''' <exception cref="ClassNotFoundException">
        '''          If the class could not be found </exception>
        Protected Friend Overridable Function loadClass(ByVal name As String, ByVal resolve As Boolean) As [Class]
            SyncLock getClassLoadingLock(name)
                ' First, check if the class has already been loaded
                Dim c As [Class] = findLoadedClass(name)
                If c Is Nothing Then
                    Dim t0 As Long = System.nanoTime()
                    Try
                        If parent IsNot Nothing Then
                            c = parent.loadClass(name, False)
                        Else
                            c = findBootstrapClassOrNull(name)
                        End If
                    Catch e As  ClassNotFoundException
                        ' ClassNotFoundException thrown if class not found
                        ' from the non-null parent class loader
                    End Try

                    If c Is Nothing Then
                        ' If still not found, then invoke findClass in order
                        ' to find the class.
                        Dim t1 As Long = System.nanoTime()
                        c = findClass(name)

                        ' this is the defining class loader; record the stats
                        sun.misc.PerfCounter.parentDelegationTime.addTime(t1 - t0)
                        sun.misc.PerfCounter.findClassTime.addElapsedTimeFrom(t1)
                        sun.misc.PerfCounter.findClasses.increment()
                    End If
                End If
                If resolve Then resolveClass(c)
                Return c
            End SyncLock
        End Function

        ''' <summary>
        ''' Returns the lock object for class loading operations.
        ''' For backward compatibility, the default implementation of this method
        ''' behaves as follows. If this ClassLoader object is registered as
        ''' parallel capable, the method returns a dedicated object associated
        ''' with the specified class name. Otherwise, the method returns this
        ''' ClassLoader object.
        ''' </summary>
        ''' <param name="className">
        '''         The name of the to-be-loaded class
        ''' </param>
        ''' <returns> the lock for class loading operations
        ''' </returns>
        ''' <exception cref="NullPointerException">
        '''         If registered as parallel capable and <tt>className</tt> is null
        ''' </exception>
        ''' <seealso cref= #loadClass(String, boolean)
        ''' 
        ''' @since  1.7 </seealso>
        Protected Friend Overridable Function getClassLoadingLock(ByVal className As String) As Object
			Dim lock As Object = Me
			If parallelLockMap IsNot Nothing Then
				Dim newLock As New Object
				lock = parallelLockMap.GetOrAdd(className, newLock)
				If lock Is Nothing Then lock = newLock
			End If
			Return lock
		End Function

		' This method is invoked by the virtual machine to load a class.
		Private Function loadClassInternal(ByVal name As String) As  [Class]
			' For backward compatibility, explicitly lock on 'this' when
			' the current class loader is not parallel capable.
			If parallelLockMap Is Nothing Then
				SyncLock Me
					 Return loadClass(name)
				End SyncLock
			Else
				Return loadClass(name)
			End If
		End Function

        ' Invoked by the VM after loading class with this loader.
        Private Sub checkPackageAccess(ByVal cls As [Class], ByVal pd As java.security.ProtectionDomain)
            Dim sm As SecurityManager = System.securityManager
            If sm IsNot Nothing Then
                If sun.reflect.misc.ReflectUtil.isNonPublicProxyClass(cls) Then
                    For Each intf As [Class] In cls.interfaces
                        checkPackageAccess(intf, pd)
                    Next intf
                    Return
                End If

                Dim name As String = cls.name
                Dim i As Integer = name.LastIndexOf("."c)
                If i <> -1 Then java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
            End If
            domains.add(pd)
        End Sub

        Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As Void
				sm.checkPackageAccess(name.Substring(0, i))
				Return Nothing
			End Function
		End Class

		''' <summary>
		''' Finds the class with the specified <a href="#name">binary name</a>.
		''' This method should be overridden by class loader implementations that
		''' follow the delegation model for loading classes, and will be invoked by
		''' the <seealso cref="#loadClass <tt>loadClass</tt>"/> method after checking the
		''' parent class loader for the requested class.  The default implementation
		''' throws a <tt>ClassNotFoundException</tt>.
		''' </summary>
		''' <param name="name">
		'''         The <a href="#name">binary name</a> of the class
		''' </param>
		''' <returns>  The resulting <tt>Class</tt> object
		''' </returns>
		''' <exception cref="ClassNotFoundException">
		'''          If the class could not be found
		''' 
		''' @since  1.2 </exception>
		Protected Friend Overridable Function findClass(ByVal name As String) As  [Class]
			Throw New ClassNotFoundException(name)
		End Function

		''' <summary>
		''' Converts an array of bytes into an instance of class <tt>Class</tt>.
		''' Before the <tt>Class</tt> can be used it must be resolved.  This method
		''' is deprecated in favor of the version that takes a <a
		''' href="#name">binary name</a> as its first argument, and is more secure.
		''' </summary>
		''' <param name="b">
		'''         The bytes that make up the class data.  The bytes in positions
		'''         <tt>off</tt> through <tt>off+len-1</tt> should have the format
		'''         of a valid class file as defined by
		'''         <cite>The Java&trade; Virtual Machine Specification</cite>.
		''' </param>
		''' <param name="off">
		'''         The start offset in <tt>b</tt> of the class data
		''' </param>
		''' <param name="len">
		'''         The length of the class data
		''' </param>
		''' <returns>  The <tt>Class</tt> object that was created from the specified
		'''          class data
		''' </returns>
		''' <exception cref="ClassFormatError">
		'''          If the data did not contain a valid class
		''' </exception>
		''' <exception cref="IndexOutOfBoundsException">
		'''          If either <tt>off</tt> or <tt>len</tt> is negative, or if
		'''          <tt>off+len</tt> is greater than <tt>b.length</tt>.
		''' </exception>
		''' <exception cref="SecurityException">
		'''          If an attempt is made to add this class to a package that
		'''          contains classes that were signed by a different set of
		'''          certificates than this [Class], or if an attempt is made
		'''          to define a class in a package with a fully-qualified name
		'''          that starts with "{@code java.}".
		''' </exception>
		''' <seealso cref=  #loadClass(String, boolean) </seealso>
		''' <seealso cref=  #resolveClass(Class)
		''' </seealso>
		''' @deprecated  Replaced by {@link #defineClass(String, byte[], int, int)
		''' defineClass(String, byte[], int, int)} 
		<Obsolete(" Replaced by {@link #defineClass(String, byte[], int, int)")> _
		Protected Friend Function defineClass(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer) As  [Class]
			Return defineClass(Nothing, b, [off], len, Nothing)
		End Function

		''' <summary>
		''' Converts an array of bytes into an instance of class <tt>Class</tt>.
		''' Before the <tt>Class</tt> can be used it must be resolved.
		''' 
		''' <p> This method assigns a default {@link java.security.ProtectionDomain
		''' <tt>ProtectionDomain</tt>} to the newly defined class.  The
		''' <tt>ProtectionDomain</tt> is effectively granted the same set of
		''' permissions returned when {@link
		''' java.security.Policy#getPermissions(java.security.CodeSource)
		''' <tt>Policy.getPolicy().getPermissions(new CodeSource(null, null))</tt>}
		''' is invoked.  The default domain is created on the first invocation of
		''' <seealso cref="#defineClass(String, byte[], int, int) <tt>defineClass</tt>"/>,
		''' and re-used on subsequent invocations.
		''' 
		''' <p> To assign a specific <tt>ProtectionDomain</tt> to the [Class], use
		''' the {@link #defineClass(String, byte[], int, int,
		''' java.security.ProtectionDomain) <tt>defineClass</tt>} method that takes a
		''' <tt>ProtectionDomain</tt> as one of its arguments.  </p>
		''' </summary>
		''' <param name="name">
		'''         The expected <a href="#name">binary name</a> of the [Class], or
		'''         <tt>null</tt> if not known
		''' </param>
		''' <param name="b">
		'''         The bytes that make up the class data.  The bytes in positions
		'''         <tt>off</tt> through <tt>off+len-1</tt> should have the format
		'''         of a valid class file as defined by
		'''         <cite>The Java&trade; Virtual Machine Specification</cite>.
		''' </param>
		''' <param name="off">
		'''         The start offset in <tt>b</tt> of the class data
		''' </param>
		''' <param name="len">
		'''         The length of the class data
		''' </param>
		''' <returns>  The <tt>Class</tt> object that was created from the specified
		'''          class data.
		''' </returns>
		''' <exception cref="ClassFormatError">
		'''          If the data did not contain a valid class
		''' </exception>
		''' <exception cref="IndexOutOfBoundsException">
		'''          If either <tt>off</tt> or <tt>len</tt> is negative, or if
		'''          <tt>off+len</tt> is greater than <tt>b.length</tt>.
		''' </exception>
		''' <exception cref="SecurityException">
		'''          If an attempt is made to add this class to a package that
		'''          contains classes that were signed by a different set of
		'''          certificates than this class (which is unsigned), or if
		'''          <tt>name</tt> begins with "<tt>java.</tt>".
		''' </exception>
		''' <seealso cref=  #loadClass(String, boolean) </seealso>
		''' <seealso cref=  #resolveClass(Class) </seealso>
		''' <seealso cref=  java.security.CodeSource </seealso>
		''' <seealso cref=  java.security.SecureClassLoader
		''' 
		''' @since  1.1 </seealso>
		Protected Friend Function defineClass(ByVal name As String, ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer) As  [Class]
			Return defineClass(name, b, [off], len, Nothing)
		End Function

	'     Determine protection domain, and check that:
	'        - not define java.* [Class],
	'        - signer of this class matches signers for the rest of the classes in
	'          package.
	'    
		Private Function preDefineClass(ByVal name As String, ByVal pd As java.security.ProtectionDomain) As java.security.ProtectionDomain
			If Not checkName(name) Then Throw New NoClassDefFoundError("IllegalName: " & name)

			If (name IsNot Nothing) AndAlso name.StartsWith("java.") Then Throw New SecurityException("Prohibited package name: " & name.Substring(0, name.LastIndexOf("."c)))
			If pd Is Nothing Then pd = defaultDomain

			If name IsNot Nothing Then checkCerts(name, pd.codeSource)

			Return pd
		End Function

		Private Function defineClassSourceLocation(ByVal pd As java.security.ProtectionDomain) As String
			Dim cs As java.security.CodeSource = pd.codeSource
			Dim source As String = Nothing
			If cs IsNot Nothing AndAlso cs.location IsNot Nothing Then source = cs.location.ToString()
			Return source
		End Function

		Private Sub postDefineClass(ByVal c As [Class], ByVal pd As java.security.ProtectionDomain)
			If pd.codeSource IsNot Nothing Then
				Dim certs As java.security.cert.Certificate() = pd.codeSource.certificates
				If certs IsNot Nothing Then signersers(c, certs)
			End If
		End Sub

		''' <summary>
		''' Converts an array of bytes into an instance of class <tt>Class</tt>,
		''' with an optional <tt>ProtectionDomain</tt>.  If the domain is
		''' <tt>null</tt>, then a default domain will be assigned to the class as
		''' specified in the documentation for {@link #defineClass(String, byte[],
		''' int, int)}.  Before the class can be used it must be resolved.
		''' 
		''' <p> The first class defined in a package determines the exact set of
		''' certificates that all subsequent classes defined in that package must
		''' contain.  The set of certificates for a class is obtained from the
		''' <seealso cref="java.security.CodeSource <tt>CodeSource</tt>"/> within the
		''' <tt>ProtectionDomain</tt> of the class.  Any classes added to that
		''' package must contain the same set of certificates or a
		''' <tt>SecurityException</tt> will be thrown.  Note that if
		''' <tt>name</tt> is <tt>null</tt>, this check is not performed.
		''' You should always pass in the <a href="#name">binary name</a> of the
		''' class you are defining as well as the bytes.  This ensures that the
		''' class you are defining is indeed the class you think it is.
		''' 
		''' <p> The specified <tt>name</tt> cannot begin with "<tt>java.</tt>", since
		''' all classes in the "<tt>java.*</tt> packages can only be defined by the
		''' bootstrap class loader.  If <tt>name</tt> is not <tt>null</tt>, it
		''' must be equal to the <a href="#name">binary name</a> of the class
		''' specified by the byte array "<tt>b</tt>", otherwise a {@link
		''' NoClassDefFoundError <tt>NoClassDefFoundError</tt>} will be thrown. </p>
		''' </summary>
		''' <param name="name">
		'''         The expected <a href="#name">binary name</a> of the [Class], or
		'''         <tt>null</tt> if not known
		''' </param>
		''' <param name="b">
		'''         The bytes that make up the class data. The bytes in positions
		'''         <tt>off</tt> through <tt>off+len-1</tt> should have the format
		'''         of a valid class file as defined by
		'''         <cite>The Java&trade; Virtual Machine Specification</cite>.
		''' </param>
		''' <param name="off">
		'''         The start offset in <tt>b</tt> of the class data
		''' </param>
		''' <param name="len">
		'''         The length of the class data
		''' </param>
		''' <param name="protectionDomain">
		'''         The ProtectionDomain of the class
		''' </param>
		''' <returns>  The <tt>Class</tt> object created from the data,
		'''          and optional <tt>ProtectionDomain</tt>.
		''' </returns>
		''' <exception cref="ClassFormatError">
		'''          If the data did not contain a valid class
		''' </exception>
		''' <exception cref="NoClassDefFoundError">
		'''          If <tt>name</tt> is not equal to the <a href="#name">binary
		'''          name</a> of the class specified by <tt>b</tt>
		''' </exception>
		''' <exception cref="IndexOutOfBoundsException">
		'''          If either <tt>off</tt> or <tt>len</tt> is negative, or if
		'''          <tt>off+len</tt> is greater than <tt>b.length</tt>.
		''' </exception>
		''' <exception cref="SecurityException">
		'''          If an attempt is made to add this class to a package that
		'''          contains classes that were signed by a different set of
		'''          certificates than this [Class], or if <tt>name</tt> begins with
		'''          "<tt>java.</tt>". </exception>
		Protected Friend Function defineClass(ByVal name As String, ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer, ByVal protectionDomain As java.security.ProtectionDomain) As  [Class]
			protectionDomain = preDefineClass(name, protectionDomain)
			Dim source As String = defineClassSourceLocation(protectionDomain)
			Dim c As  [Class] = defineClass1(name, b, [off], len, protectionDomain, source)
			postDefineClass(c, protectionDomain)
			Return c
		End Function

		''' <summary>
		''' Converts a <seealso cref="java.nio.ByteBuffer <tt>ByteBuffer</tt>"/>
		''' into an instance of class <tt>Class</tt>,
		''' with an optional <tt>ProtectionDomain</tt>.  If the domain is
		''' <tt>null</tt>, then a default domain will be assigned to the class as
		''' specified in the documentation for {@link #defineClass(String, byte[],
		''' int, int)}.  Before the class can be used it must be resolved.
		''' 
		''' <p>The rules about the first class defined in a package determining the
		''' set of certificates for the package, and the restrictions on class names
		''' are identical to those specified in the documentation for {@link
		''' #defineClass(String, byte[], int, int, ProtectionDomain)}.
		''' 
		''' <p> An invocation of this method of the form
		''' <i>cl</i><tt>.defineClass(</tt><i>name</i><tt>,</tt>
		''' <i>bBuffer</i><tt>,</tt> <i>pd</i><tt>)</tt> yields exactly the same
		''' result as the statements
		''' 
		''' <p> <tt>
		''' ...<br>
		''' byte[] temp = new byte[bBuffer.{@link
		''' java.nio.ByteBuffer#remaining remaining}()];<br>
		'''     bBuffer.{@link java.nio.ByteBuffer#get(byte[])
		''' get}(temp);<br>
		'''     return {@link #defineClass(String, byte[], int, int, ProtectionDomain)
		''' cl.defineClass}(name, temp, 0,
		''' temp.length, pd);<br>
		''' </tt></p>
		''' </summary>
		''' <param name="name">
		'''         The expected <a href="#name">binary name</a>. of the [Class], or
		'''         <tt>null</tt> if not known
		''' </param>
		''' <param name="b">
		'''         The bytes that make up the class data. The bytes from positions
		'''         <tt>b.position()</tt> through <tt>b.position() + b.limit() -1
		'''         </tt> should have the format of a valid class file as defined by
		'''         <cite>The Java&trade; Virtual Machine Specification</cite>.
		''' </param>
		''' <param name="protectionDomain">
		'''         The ProtectionDomain of the [Class], or <tt>null</tt>.
		''' </param>
		''' <returns>  The <tt>Class</tt> object created from the data,
		'''          and optional <tt>ProtectionDomain</tt>.
		''' </returns>
		''' <exception cref="ClassFormatError">
		'''          If the data did not contain a valid class.
		''' </exception>
		''' <exception cref="NoClassDefFoundError">
		'''          If <tt>name</tt> is not equal to the <a href="#name">binary
		'''          name</a> of the class specified by <tt>b</tt>
		''' </exception>
		''' <exception cref="SecurityException">
		'''          If an attempt is made to add this class to a package that
		'''          contains classes that were signed by a different set of
		'''          certificates than this [Class], or if <tt>name</tt> begins with
		'''          "<tt>java.</tt>".
		''' </exception>
		''' <seealso cref=      #defineClass(String, byte[], int, int, ProtectionDomain)
		''' 
		''' @since  1.5 </seealso>
		Protected Friend Function defineClass(ByVal name As String, ByVal b As java.nio.ByteBuffer, ByVal protectionDomain As java.security.ProtectionDomain) As  [Class]
			Dim len As Integer = b.remaining()

			' Use byte[] if not a direct ByteBufer:
			If Not b.direct Then
				If b.hasArray() Then
					Return defineClass(name, b.array(), b.position() + b.arrayOffset(), len, protectionDomain)
				Else
					' no array, or read-only array
					Dim tb As SByte() = New SByte(len - 1){}
					b.get(tb) ' get bytes out of byte buffer.
					Return defineClass(name, tb, 0, len, protectionDomain)
				End If
			End If

			protectionDomain = preDefineClass(name, protectionDomain)
			Dim source As String = defineClassSourceLocation(protectionDomain)
			Dim c As  [Class] = defineClass2(name, b, b.position(), len, protectionDomain, source)
			postDefineClass(c, protectionDomain)
			Return c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Function defineClass0(ByVal name As String, ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer, ByVal pd As java.security.ProtectionDomain) As  [Class]
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Function defineClass1(ByVal name As String, ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer, ByVal pd As java.security.ProtectionDomain, ByVal source As String) As  [Class]
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Function defineClass2(ByVal name As String, ByVal b As java.nio.ByteBuffer, ByVal [off] As Integer, ByVal len As Integer, ByVal pd As java.security.ProtectionDomain, ByVal source As String) As  [Class]
		End Function

		' true if the name is null or has the potential to be a valid binary name
		Private Function checkName(ByVal name As String) As Boolean
			If (name Is Nothing) OrElse (name.length() = 0) Then Return True
			If (name.IndexOf("/"c) <> -1) OrElse ((Not sun.misc.VM.allowArraySyntax()) AndAlso (name.Chars(0) = "["c)) Then Return False
			Return True
		End Function

		Private Sub checkCerts(ByVal name As String, ByVal cs As java.security.CodeSource)
			Dim i As Integer = name.LastIndexOf("."c)
			Dim pname As String = If(i = -1, "", name.Substring(0, i))

			Dim certs As java.security.cert.Certificate() = Nothing
			If cs IsNot Nothing Then certs = cs.certificates
			Dim pcerts As java.security.cert.Certificate() = Nothing
			If parallelLockMap Is Nothing Then
				SyncLock Me
					pcerts = package2certs(pname)
					If pcerts Is Nothing Then package2certs(pname) = (If(certs Is Nothing, nocerts, certs))
				End SyncLock
			Else
				pcerts = CType(package2certs, ConcurrentDictionary(Of String, java.security.cert.Certificate())).GetOrAdd(pname, (If(certs Is Nothing, nocerts, certs)))
			End If
			If pcerts IsNot Nothing AndAlso (Not compareCerts(pcerts, certs)) Then Throw New SecurityException("class """ & name & """'s signer information does not match signer information of other classes in the same package")
		End Sub

		''' <summary>
		''' check to make sure the certs for the new class (certs) are the same as
		''' the certs for the first class inserted in the package (pcerts)
		''' </summary>
		Private Function compareCerts(ByVal pcerts As java.security.cert.Certificate(), ByVal certs As java.security.cert.Certificate()) As Boolean
			' certs can be null, indicating no certs.
			If (certs Is Nothing) OrElse (certs.Length = 0) Then Return pcerts.Length = 0

			' the length must be the same at this point
			If certs.Length <> pcerts.Length Then Return False

			' go through and make sure all the certs in one array
			' are in the other and vice-versa.
			Dim match As Boolean
			For i As Integer = 0 To certs.Length - 1
				match = False
				For j As Integer = 0 To pcerts.Length - 1
					If certs(i).Equals(pcerts(j)) Then
						match = True
						Exit For
					End If
				Next j
				If Not match Then Return False
			Next i

			' now do the same for pcerts
			For i As Integer = 0 To pcerts.Length - 1
				match = False
				For j As Integer = 0 To certs.Length - 1
					If pcerts(i).Equals(certs(j)) Then
						match = True
						Exit For
					End If
				Next j
				If Not match Then Return False
			Next i

			Return True
		End Function

		''' <summary>
		''' Links the specified class.  This (misleadingly named) method may be
		''' used by a class loader to link a class.  If the class <tt>c</tt> has
		''' already been linked, then this method simply returns. Otherwise, the
		''' class is linked as described in the "Execution" chapter of
		''' <cite>The Java&trade; Language Specification</cite>.
		''' </summary>
		''' <param name="c">
		'''         The class to link
		''' </param>
		''' <exception cref="NullPointerException">
		'''          If <tt>c</tt> is <tt>null</tt>.
		''' </exception>
		''' <seealso cref=  #defineClass(String, byte[], int, int) </seealso>
		Protected Friend Sub resolveClass(ByVal c As [Class])
			resolveClass0(c)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Sub resolveClass0(ByVal c As [Class])
		End Sub

		''' <summary>
		''' Finds a class with the specified <a href="#name">binary name</a>,
		''' loading it if necessary.
		''' 
		''' <p> This method loads the class through the system class loader (see
		''' <seealso cref="#getSystemClassLoader()"/>).  The <tt>Class</tt> object returned
		''' might have more than one <tt>ClassLoader</tt> associated with it.
		''' Subclasses of <tt>ClassLoader</tt> need not usually invoke this method,
		''' because most class loaders need to override just {@link
		''' #findClass(String)}.  </p>
		''' </summary>
		''' <param name="name">
		'''         The <a href="#name">binary name</a> of the class
		''' </param>
		''' <returns>  The <tt>Class</tt> object for the specified <tt>name</tt>
		''' </returns>
		''' <exception cref="ClassNotFoundException">
		'''          If the class could not be found
		''' </exception>
		''' <seealso cref=  #ClassLoader(ClassLoader) </seealso>
		''' <seealso cref=  #getParent() </seealso>
		Protected Friend Function findSystemClass(ByVal name As String) As  [Class]
			Dim system_Renamed As  ClassLoader = systemClassLoader
			If system_Renamed Is Nothing Then
				If Not checkName(name) Then Throw New ClassNotFoundException(name)
				Dim cls As  [Class] = findBootstrapClass(name)
				If cls Is Nothing Then Throw New ClassNotFoundException(name)
				Return cls
			End If
			Return system_Renamed.loadClass(name)
		End Function

		''' <summary>
		''' Returns a class loaded by the bootstrap class loader;
		''' or return null if not found.
		''' </summary>
		Private Function findBootstrapClassOrNull(ByVal name As String) As  [Class]
			If Not checkName(name) Then Return Nothing

			Return findBootstrapClass(name)
		End Function

		' return null if not found
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Function findBootstrapClass(ByVal name As String) As  [Class]
		End Function

		''' <summary>
		''' Returns the class with the given <a href="#name">binary name</a> if this
		''' loader has been recorded by the Java virtual machine as an initiating
		''' loader of a class with that <a href="#name">binary name</a>.  Otherwise
		''' <tt>null</tt> is returned.
		''' </summary>
		''' <param name="name">
		'''         The <a href="#name">binary name</a> of the class
		''' </param>
		''' <returns>  The <tt>Class</tt> object, or <tt>null</tt> if the class has
		'''          not been loaded
		''' 
		''' @since  1.1 </returns>
		Protected Friend Function findLoadedClass(ByVal name As String) As  [Class]
			If Not checkName(name) Then Return Nothing
			Return findLoadedClass0(name)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private final Function findLoadedClass0(ByVal name As String) As  [Class]
		End Function

		''' <summary>
		''' Sets the signers of a class.  This should be invoked after defining a
		''' class.
		''' </summary>
		''' <param name="c">
		'''         The <tt>Class</tt> object
		''' </param>
		''' <param name="signers">
		'''         The signers for the class
		''' 
		''' @since  1.1 </param>
		Protected Friend Sub setSigners(ByVal c As [Class], ByVal signers As Object())
			c.signers = signers
		End Sub


		' -- Resource --

		''' <summary>
		''' Finds the resource with the given name.  A resource is some data
		''' (images, audio, text, etc) that can be accessed by class code in a way
		''' that is independent of the location of the code.
		''' 
		''' <p> The name of a resource is a '<tt>/</tt>'-separated path name that
		''' identifies the resource.
		''' 
		''' <p> This method will first search the parent class loader for the
		''' resource; if the parent is <tt>null</tt> the path of the class loader
		''' built-in to the virtual machine is searched.  That failing, this method
		''' will invoke <seealso cref="#findResource(String)"/> to find the resource.  </p>
		''' 
		''' @apiNote When overriding this method it is recommended that an
		''' implementation ensures that any delegation is consistent with the {@link
		''' #getResources(java.lang.String) getResources(String)} method.
		''' </summary>
		''' <param name="name">
		'''         The resource name
		''' </param>
		''' <returns>  A <tt>URL</tt> object for reading the resource, or
		'''          <tt>null</tt> if the resource could not be found or the invoker
		'''          doesn't have adequate  privileges to get the resource.
		''' 
		''' @since  1.1 </returns>
		Public Overridable Function getResource(ByVal name As String) As java.net.URL
			Dim url As java.net.URL
			If parent IsNot Nothing Then
				url = parent.getResource(name)
			Else
				url = getBootstrapResource(name)
			End If
			If url Is Nothing Then url = findResource(name)
			Return url
		End Function

		''' <summary>
		''' Finds all the resources with the given name. A resource is some data
		''' (images, audio, text, etc) that can be accessed by class code in a way
		''' that is independent of the location of the code.
		''' 
		''' <p>The name of a resource is a <tt>/</tt>-separated path name that
		''' identifies the resource.
		''' 
		''' <p> The search order is described in the documentation for {@link
		''' #getResource(String)}.  </p>
		''' 
		''' @apiNote When overriding this method it is recommended that an
		''' implementation ensures that any delegation is consistent with the {@link
		''' #getResource(java.lang.String) getResource(String)} method. This should
		''' ensure that the first element returned by the Enumeration's
		''' {@code nextElement} method is the same resource that the
		''' {@code getResource(String)} method would return.
		''' </summary>
		''' <param name="name">
		'''         The resource name
		''' </param>
		''' <returns>  An enumeration of <seealso cref="java.net.URL <tt>URL</tt>"/> objects for
		'''          the resource.  If no resources could  be found, the enumeration
		'''          will be empty.  Resources that the class loader doesn't have
		'''          access to will not be in the enumeration.
		''' </returns>
		''' <exception cref="IOException">
		'''          If I/O errors occur
		''' </exception>
		''' <seealso cref=  #findResources(String)
		''' 
		''' @since  1.2 </seealso>
		Public Overridable Function getResources(ByVal name As String) As System.Collections.IEnumerator(Of java.net.URL)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim tmp As System.Collections.IEnumerator(Of java.net.URL)() = CType(New System.Collections.IEnumerator(Of ?)(1){}, System.Collections.IEnumerator(Of java.net.URL)())
			If parent IsNot Nothing Then
				tmp(0) = parent.getResources(name)
			Else
				tmp(0) = getBootstrapResources(name)
			End If
			tmp(1) = findResources(name)

			Return New sun.misc.CompoundEnumeration(Of )(tmp)
		End Function

		''' <summary>
		''' Finds the resource with the given name. Class loader implementations
		''' should override this method to specify where to find resources.
		''' </summary>
		''' <param name="name">
		'''         The resource name
		''' </param>
		''' <returns>  A <tt>URL</tt> object for reading the resource, or
		'''          <tt>null</tt> if the resource could not be found
		''' 
		''' @since  1.2 </returns>
		Protected Friend Overridable Function findResource(ByVal name As String) As java.net.URL
			Return Nothing
		End Function

		''' <summary>
		''' Returns an enumeration of <seealso cref="java.net.URL <tt>URL</tt>"/> objects
		''' representing all the resources with the given name. Class loader
		''' implementations should override this method to specify where to load
		''' resources from.
		''' </summary>
		''' <param name="name">
		'''         The resource name
		''' </param>
		''' <returns>  An enumeration of <seealso cref="java.net.URL <tt>URL</tt>"/> objects for
		'''          the resources
		''' </returns>
		''' <exception cref="IOException">
		'''          If I/O errors occur
		''' 
		''' @since  1.2 </exception>
		Protected Friend Overridable Function findResources(ByVal name As String) As System.Collections.IEnumerator(Of java.net.URL)
			Return java.util.Collections.emptyEnumeration()
		End Function

		''' <summary>
		''' Registers the caller as parallel capable.
		''' The registration succeeds if and only if all of the following
		''' conditions are met:
		''' <ol>
		''' <li> no instance of the caller has been created</li>
		''' <li> all of the super classes (except class Object) of the caller are
		''' registered as parallel capable</li>
		''' </ol>
		''' <p>Note that once a class loader is registered as parallel capable, there
		''' is no way to change it back.</p>
		''' </summary>
		''' <returns>  true if the caller is successfully registered as
		'''          parallel capable and false if otherwise.
		''' 
		''' @since   1.7 </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Protected Friend Shared Function registerAsParallelCapable() As Boolean
			Dim callerClass As  [Class] = sun.reflect.Reflection.callerClass.asSubclass(GetType(ClassLoader))
			Return ParallelLoaders.register(callerClass)
		End Function

		''' <summary>
		''' Find a resource of the specified name from the search path used to load
		''' classes.  This method locates the resource through the system class
		''' loader (see <seealso cref="#getSystemClassLoader()"/>).
		''' </summary>
		''' <param name="name">
		'''         The resource name
		''' </param>
		''' <returns>  A <seealso cref="java.net.URL <tt>URL</tt>"/> object for reading the
		'''          resource, or <tt>null</tt> if the resource could not be found
		''' 
		''' @since  1.1 </returns>
		Public Shared Function getSystemResource(ByVal name As String) As java.net.URL
			Dim system_Renamed As  ClassLoader = systemClassLoader
			If system_Renamed Is Nothing Then Return getBootstrapResource(name)
			Return system_Renamed.getResource(name)
		End Function

		''' <summary>
		''' Finds all resources of the specified name from the search path used to
		''' load classes.  The resources thus found are returned as an
		''' <seealso cref="java.util.Enumeration <tt>Enumeration</tt>"/> of {@link
		''' java.net.URL <tt>URL</tt>} objects.
		''' 
		''' <p> The search order is described in the documentation for {@link
		''' #getSystemResource(String)}.  </p>
		''' </summary>
		''' <param name="name">
		'''         The resource name
		''' </param>
		''' <returns>  An enumeration of resource <seealso cref="java.net.URL <tt>URL</tt>"/>
		'''          objects
		''' </returns>
		''' <exception cref="IOException">
		'''          If I/O errors occur
		''' 
		''' @since  1.2 </exception>
		Public Shared Function getSystemResources(ByVal name As String) As System.Collections.IEnumerator(Of java.net.URL)
			Dim system_Renamed As  ClassLoader = systemClassLoader
			If system_Renamed Is Nothing Then Return getBootstrapResources(name)
			Return system_Renamed.getResources(name)
		End Function

		''' <summary>
		''' Find resources from the VM's built-in classloader.
		''' </summary>
		Private Shared Function getBootstrapResource(ByVal name As String) As java.net.URL
			Dim ucp As sun.misc.URLClassPath = bootstrapClassPath
			Dim res As sun.misc.Resource = ucp.getResource(name)
			Return If(res IsNot Nothing, res.uRL, Nothing)
		End Function

		''' <summary>
		''' Find resources from the VM's built-in classloader.
		''' </summary>
		Private Shared Function getBootstrapResources(ByVal name As String) As System.Collections.IEnumerator(Of java.net.URL)
			Dim e As System.Collections.IEnumerator(Of sun.misc.Resource) = bootstrapClassPath.getResources(name)
			Return New EnumerationAnonymousInnerClassHelper(Of E)
		End Function

		Private Class EnumerationAnonymousInnerClassHelper(Of E)
			Implements System.Collections.IEnumerator(Of E)

			Public Overridable Function nextElement() As java.net.URL
				Return e.nextElement().uRL
			End Function
			Public Overridable Function hasMoreElements() As Boolean
				Return e.hasMoreElements()
			End Function
		End Class

		' Returns the URLClassPath that is used for finding system resources.
		FriendShared ReadOnly PropertybootstrapClassPath As sun.misc.URLClassPath
			Get
				Return sun.misc.Launcher.bootstrapClassPath
			End Get
		End Property


		''' <summary>
		''' Returns an input stream for reading the specified resource.
		''' 
		''' <p> The search order is described in the documentation for {@link
		''' #getResource(String)}.  </p>
		''' </summary>
		''' <param name="name">
		'''         The resource name
		''' </param>
		''' <returns>  An input stream for reading the resource, or <tt>null</tt>
		'''          if the resource could not be found
		''' 
		''' @since  1.1 </returns>
		Public Overridable Function getResourceAsStream(ByVal name As String) As java.io.InputStream
			Dim url As java.net.URL = getResource(name)
			Try
				Return If(url IsNot Nothing, url.openStream(), Nothing)
			Catch e As java.io.IOException
				Return Nothing
			End Try
		End Function

		''' <summary>
		''' Open for reading, a resource of the specified name from the search path
		''' used to load classes.  This method locates the resource through the
		''' system class loader (see <seealso cref="#getSystemClassLoader()"/>).
		''' </summary>
		''' <param name="name">
		'''         The resource name
		''' </param>
		''' <returns>  An input stream for reading the resource, or <tt>null</tt>
		'''          if the resource could not be found
		''' 
		''' @since  1.1 </returns>
		Public Shared Function getSystemResourceAsStream(ByVal name As String) As java.io.InputStream
			Dim url As java.net.URL = getSystemResource(name)
			Try
				Return If(url IsNot Nothing, url.openStream(), Nothing)
			Catch e As java.io.IOException
				Return Nothing
			End Try
		End Function


		' -- Hierarchy --

		''' <summary>
		''' Returns the parent class loader for delegation. Some implementations may
		''' use <tt>null</tt> to represent the bootstrap class loader. This method
		''' will return <tt>null</tt> in such implementations if this class loader's
		''' parent is the bootstrap class loader.
		''' 
		''' <p> If a security manager is present, and the invoker's class loader is
		''' not <tt>null</tt> and is not an ancestor of this class loader, then this
		''' method invokes the security manager's {@link
		''' SecurityManager#checkPermission(java.security.Permission)
		''' <tt>checkPermission</tt>} method with a {@link
		''' RuntimePermission#RuntimePermission(String)
		''' <tt>RuntimePermission("getClassLoader")</tt>} permission to verify
		''' access to the parent class loader is permitted.  If not, a
		''' <tt>SecurityException</tt> will be thrown.  </p>
		''' </summary>
		''' <returns>  The parent <tt>ClassLoader</tt>
		''' </returns>
		''' <exception cref="SecurityException">
		'''          If a security manager exists and its <tt>checkPermission</tt>
		'''          method doesn't allow access to this class loader's parent class
		'''          loader.
		''' 
		''' @since  1.2 </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Property parent As  ClassLoader
			Get
				If parent Is Nothing Then Return Nothing
				Dim sm As SecurityManager = System.securityManager
				If sm IsNot Nothing Then checkClassLoaderPermission(parent, sun.reflect.Reflection.callerClass)
				Return parent
			End Get
		End Property

		''' <summary>
		''' Returns the system class loader for delegation.  This is the default
		''' delegation parent for new <tt>ClassLoader</tt> instances, and is
		''' typically the class loader used to start the application.
		''' 
		''' <p> This method is first invoked early in the runtime's startup
		''' sequence, at which point it creates the system class loader and sets it
		''' as the context class loader of the invoking <tt>Thread</tt>.
		''' 
		''' <p> The default system class loader is an implementation-dependent
		''' instance of this class.
		''' 
		''' <p> If the system property "<tt>java.system.class.loader</tt>" is defined
		''' when this method is first invoked then the value of that property is
		''' taken to be the name of a class that will be returned as the system
		''' class loader.  The class is loaded using the default system class loader
		''' and must define a public constructor that takes a single parameter of
		''' type <tt>ClassLoader</tt> which is used as the delegation parent.  An
		''' instance is then created using this constructor with the default system
		''' class loader as the parameter.  The resulting class loader is defined
		''' to be the system class loader.
		''' 
		''' <p> If a security manager is present, and the invoker's class loader is
		''' not <tt>null</tt> and the invoker's class loader is not the same as or
		''' an ancestor of the system class loader, then this method invokes the
		''' security manager's {@link
		''' SecurityManager#checkPermission(java.security.Permission)
		''' <tt>checkPermission</tt>} method with a {@link
		''' RuntimePermission#RuntimePermission(String)
		''' <tt>RuntimePermission("getClassLoader")</tt>} permission to verify
		''' access to the system class loader.  If not, a
		''' <tt>SecurityException</tt> will be thrown.  </p>
		''' </summary>
		''' <returns>  The system <tt>ClassLoader</tt> for delegation, or
		'''          <tt>null</tt> if none
		''' </returns>
		''' <exception cref="SecurityException">
		'''          If a security manager exists and its <tt>checkPermission</tt>
		'''          method doesn't allow access to the system class loader.
		''' </exception>
		''' <exception cref="IllegalStateException">
		'''          If invoked recursively during the construction of the class
		'''          loader specified by the "<tt>java.system.class.loader</tt>"
		'''          property.
		''' </exception>
		''' <exception cref="Error">
		'''          If the system property "<tt>java.system.class.loader</tt>"
		'''          is defined but the named class could not be loaded, the
		'''          provider class does not define the required constructor, or an
		'''          exception is thrown by that constructor when it is invoked. The
		'''          underlying cause of the error can be retrieved via the
		'''          <seealso cref="Throwable#getCause()"/> method.
		''' 
		''' @revised  1.4 </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		PublicShared ReadOnly PropertysystemClassLoader As  ClassLoader
			Get
				initSystemClassLoader()
				If scl Is Nothing Then Return Nothing
				Dim sm As SecurityManager = System.securityManager
				If sm IsNot Nothing Then checkClassLoaderPermission(scl, sun.reflect.Reflection.callerClass)
				Return scl
			End Get
		End Property

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Shared Sub initSystemClassLoader()
			If Not sclSet Then
				If scl IsNot Nothing Then Throw New IllegalStateException("recursive invocation")
				Dim l As sun.misc.Launcher = sun.misc.Launcher.launcher
				If l IsNot Nothing Then
					Dim oops As Throwable = Nothing
					scl = l.classLoader
					Try
						scl = java.security.AccessController.doPrivileged(New SystemClassLoaderAction(scl))
					Catch pae As java.security.PrivilegedActionException
						oops = pae.InnerException
						If TypeOf oops Is InvocationTargetException Then oops = oops.cause
					End Try
					If oops IsNot Nothing Then
						If TypeOf oops Is Error Then
							Throw CType(oops, [Error])
						Else
							' wrap the exception
							Throw New [Error](oops)
						End If
					End If
				End If
				sclSet = True
			End If
		End Sub

		' Returns true if the specified class loader can be found in this class
		' loader's delegation chain.
		Friend Overridable Function isAncestor(ByVal cl As  ClassLoader) As Boolean
			Dim acl As  ClassLoader = Me
			Do
				acl = acl.parent
				If cl Is acl Then Return True
			Loop While acl IsNot Nothing
			Return False
		End Function

		' Tests if class loader access requires "getClassLoader" permission
		' check.  A class loader 'from' can access class loader 'to' if
		' class loader 'from' is same As  [Class] loader 'to' or an ancestor
		' of 'to'.  The class loader in a system domain can access
		' any class loader.
		Private Shared Function needsClassLoaderPermissionCheck(ByVal [from] As  ClassLoader, ByVal [to] As  ClassLoader) As Boolean
			If [from] Is [to] Then Return False

			If [from] Is Nothing Then Return False

			Return Not [to].isAncestor([from])
		End Function

		' Returns the class's class loader, or null if none.
		Shared Function getClassLoader(ByVal caller As [Class]) As  ClassLoader
			' This can be null if the VM is requesting it
			If caller Is Nothing Then Return Nothing
			' Circumvent security check since this is package-private
			Return caller.classLoader0
		End Function

	'    
	'     * Checks RuntimePermission("getClassLoader") permission
	'     * if caller's class loader is not null and caller's class loader
	'     * is not the same as or an ancestor of the given cl argument.
	'     
		Shared Sub checkClassLoaderPermission(ByVal cl As  ClassLoader, ByVal caller As [Class])
			Dim sm As SecurityManager = System.securityManager
			If sm IsNot Nothing Then
				' caller can be null if the VM is requesting it
				Dim ccl As  ClassLoader = getClassLoader(caller)
				If needsClassLoaderPermissionCheck(ccl, cl) Then sm.checkPermission(sun.security.util.SecurityConstants.GET_CLASSLOADER_PERMISSION)
			End If
		End Sub

		' The class loader for the system
		' @GuardedBy("ClassLoader.class")
		Private Shared scl As  ClassLoader

		' Set to true once the system class loader has been set
		' @GuardedBy("ClassLoader.class")
		Private Shared sclSet As Boolean


		' -- Package --

		''' <summary>
		''' Defines a package by name in this <tt>ClassLoader</tt>.  This allows
		''' class loaders to define the packages for their classes. Packages must
		''' be created before the class is defined, and package names must be
		''' unique within a class loader and cannot be redefined or changed once
		''' created.
		''' </summary>
		''' <param name="name">
		'''         The package name
		''' </param>
		''' <param name="specTitle">
		'''         The specification title
		''' </param>
		''' <param name="specVersion">
		'''         The specification version
		''' </param>
		''' <param name="specVendor">
		'''         The specification vendor
		''' </param>
		''' <param name="implTitle">
		'''         The implementation title
		''' </param>
		''' <param name="implVersion">
		'''         The implementation version
		''' </param>
		''' <param name="implVendor">
		'''         The implementation vendor
		''' </param>
		''' <param name="sealBase">
		'''         If not <tt>null</tt>, then this package is sealed with
		'''         respect to the given code source {@link java.net.URL
		'''         <tt>URL</tt>}  object.  Otherwise, the package is not sealed.
		''' </param>
		''' <returns>  The newly defined <tt>Package</tt> object
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''          If package name duplicates an existing package either in this
		'''          class loader or one of its ancestors
		''' 
		''' @since  1.2 </exception>
		Protected Friend Overridable Function definePackage(ByVal name As String, ByVal specTitle As String, ByVal specVersion As String, ByVal specVendor As String, ByVal implTitle As String, ByVal implVersion As String, ByVal implVendor As String, ByVal sealBase As java.net.URL) As Package
			SyncLock packages
				Dim pkg As Package = getPackage(name)
				If pkg IsNot Nothing Then Throw New IllegalArgumentException(name)
				pkg = New Package(name, specTitle, specVersion, specVendor, implTitle, implVersion, implVendor, sealBase, Me)
			packages(name) = pkg
				Return pkg
			End SyncLock
		End Function

		''' <summary>
		''' Returns a <tt>Package</tt> that has been defined by this class loader
		''' or any of its ancestors.
		''' </summary>
		''' <param name="name">
		'''         The package name
		''' </param>
		''' <returns>  The <tt>Package</tt> corresponding to the given name, or
		'''          <tt>null</tt> if not found
		''' 
		''' @since  1.2 </returns>
		Protected Friend Overridable Function getPackage(ByVal name As String) As Package
			Dim pkg As Package
			SyncLock packages
				pkg = packages(name)
			End SyncLock
			If pkg Is Nothing Then
				If parent IsNot Nothing Then
					pkg = parent.getPackage(name)
				Else
					pkg = Package.getSystemPackage(name)
				End If
				If pkg IsNot Nothing Then
					SyncLock packages
						Dim pkg2 As Package = packages(name)
						If pkg2 Is Nothing Then
						packages(name) = pkg
						Else
							pkg = pkg2
						End If
					End SyncLock
				End If
			End If
			Return pkg
		End Function

		''' <summary>
		''' Returns all of the <tt>Packages</tt> defined by this class loader and
		''' its ancestors.
		''' </summary>
		''' <returns>  The array of <tt>Package</tt> objects defined by this
		'''          <tt>ClassLoader</tt>
		''' 
		''' @since  1.2 </returns>
		Protected Friend Overridable Property packages As Package()
			Get
				Dim map As IDictionary(Of String, Package)
				SyncLock packages
					map = New Dictionary(Of )(packages)
				End SyncLock
				Dim pkgs As Package()
				If parent IsNot Nothing Then
					pkgs = parent.packages
				Else
					pkgs = Package.systemPackages
				End If
				If pkgs IsNot Nothing Then
					For i As Integer = 0 To pkgs.Length - 1
						Dim pkgName As String = pkgs(i).name
						If map(pkgName) Is Nothing Then map(pkgName) = pkgs(i)
					Next i
				End If
				Return map.Values.ToArray(New Package(map.Count - 1){})
			End Get
		End Property


		' -- Native library access --

		''' <summary>
		''' Returns the absolute path name of a native library.  The VM invokes this
		''' method to locate the native libraries that belong to classes loaded with
		''' this class loader. If this method returns <tt>null</tt>, the VM
		''' searches the library along the path specified as the
		''' "<tt>java.library.path</tt>" property.
		''' </summary>
		''' <param name="libname">
		'''         The library name
		''' </param>
		''' <returns>  The absolute path of the native library
		''' </returns>
		''' <seealso cref=  System#loadLibrary(String) </seealso>
		''' <seealso cref=  System#mapLibraryName(String)
		''' 
		''' @since  1.2 </seealso>
		Protected Friend Overridable Function findLibrary(ByVal libname As String) As String
			Return Nothing
		End Function

		''' <summary>
		''' The inner class NativeLibrary denotes a loaded native library instance.
		''' Every classloader contains a vector of loaded native libraries in the
		''' private field <tt>nativeLibraries</tt>.  The native libraries loaded
		''' into the system are entered into the <tt>systemNativeLibraries</tt>
		''' vector.
		''' 
		''' <p> Every native library requires a particular version of JNI. This is
		''' denoted by the private <tt>jniVersion</tt> field.  This field is set by
		''' the VM when it loads the library, and used by the VM to pass the correct
		''' version of JNI to the native methods.  </p>
		''' </summary>
		''' <seealso cref=      ClassLoader
		''' @since    1.2 </seealso>
		Friend Class NativeLibrary
			' opaque handle to native library, used in native code.
			Friend handle As Long
			' the version of JNI environment the native library requires.
			Private jniVersion As Integer
			' the class from which the library is loaded, also indicates
			' the loader this native library belongs.
			Private ReadOnly fromClass As  [Class]
			' the canonicalized name of the native library.
			' or static library name
			Friend name As String
			' Indicates if the native library is linked into the VM
			Friend isBuiltin As Boolean
			' Indicates if the native library is loaded
			Friend loaded As Boolean
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
			<DllImport("unknown")> _
			Friend Sub load(ByVal name As String, ByVal isBuiltin As Boolean)
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
			<DllImport("unknown")> _
			Friend Function find(ByVal name As String) As Long
			End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
			<DllImport("unknown")> _
			Friend Sub unload(ByVal name As String, ByVal isBuiltin As Boolean)
			End Sub

			Public Sub New(ByVal fromClass As [Class], ByVal name As String, ByVal isBuiltin As Boolean)
				Me.name = name
				Me.fromClass = fromClass
				Me.isBuiltin = isBuiltin
			End Sub

			Protected Overrides Sub Finalize()
				SyncLock loadedLibraryNames
					If fromClass.classLoader IsNot Nothing AndAlso loaded Then
						' remove the native library name 
						Dim size As Integer = loadedLibraryNames.Count
						For i As Integer = 0 To size - 1
							If name.Equals(loadedLibraryNames(i)) Then
								loadedLibraryNames.RemoveAt(i)
								Exit For
							End If
						Next i
						' unload the library. 
						ClassLoader.nativeLibraryContext.Push(Me)
						Try
							unload(name, isBuiltin)
						Finally
							ClassLoader.nativeLibraryContext.Pop()
						End Try
					End If
				End SyncLock
			End Sub
			' Invoked in the VM to determine the context class in
			' JNI_Load/JNI_Unload
			FriendShared ReadOnly PropertyfromClass As  [Class]
				Get
					Return ClassLoader.nativeLibraryContext.Peek().fromClass
				End Get
			End Property
		End Class

		' All native library names we've loaded.
		Private Shared loadedLibraryNames As New List(Of String)

		' Native libraries belonging to system classes.
		Private Shared systemNativeLibraries As New List(Of NativeLibrary)

		' Native libraries associated with the class loader.
		Private nativeLibraries As New List(Of NativeLibrary)

		' native libraries being loaded/unloaded.
		Private Shared nativeLibraryContext As New Stack(Of NativeLibrary)

		' The paths searched for libraries
		Private Shared usr_paths As String()
		Private Shared sys_paths As String()

		Private Shared Function initializePath(ByVal propname As String) As String()
			Dim ldpath As String = System.getProperty(propname, "")
			Dim ps As String = File.pathSeparator
			Dim ldlen As Integer = ldpath.length()
			Dim i, j, n As Integer
			' Count the separators in the path
			i = ldpath.IndexOf(ps)
			n = 0
			Do While i >= 0
				n += 1
				i = ldpath.IndexOf(ps, i + 1)
			Loop

			' allocate the array of paths - n :'s = n + 1 path elements
			Dim paths As String() = New String(n){}

			' Fill the array with paths from the ldpath
				i = 0
				n = i
			j = ldpath.IndexOf(ps)
			Do While j >= 0
				If j - i > 0 Then
					paths(n) = ldpath.Substring(i, j - i)
					n += 1
				ElseIf j - i = 0 Then
					paths(n) = "."
					n += 1
				End If
				i = j + 1
				j = ldpath.IndexOf(ps, i)
			Loop
			paths(n) = ldpath.Substring(i, ldlen - i)
			Return paths
		End Function

		' Invoked in the java.lang.Runtime class to implement load and loadLibrary.
		Friend Shared Sub loadLibrary(ByVal fromClass As [Class], ByVal name As String, ByVal isAbsolute As Boolean)
			Dim loader As  ClassLoader = If(fromClass Is Nothing, Nothing, fromClass.classLoader)
			If sys_paths Is Nothing Then
				usr_paths = initializePath("java.library.path")
				sys_paths = initializePath("sun.boot.library.path")
			End If
			If isAbsolute Then
				If loadLibrary0(fromClass, New File(name)) Then Return
				Throw New UnsatisfiedLinkError("Can't load library: " & name)
			End If
			If loader IsNot Nothing Then
				Dim libfilename As String = loader.findLibrary(name)
				If libfilename IsNot Nothing Then
					Dim libfile As New File(libfilename)
					If Not libfile.absolute Then Throw New UnsatisfiedLinkError("ClassLoader.findLibrary failed to return an absolute path: " & libfilename)
					If loadLibrary0(fromClass, libfile) Then Return
					Throw New UnsatisfiedLinkError("Can't load " & libfilename)
				End If
			End If
			For i As Integer = 0 To sys_paths.Length - 1
				Dim libfile As New File(sys_paths(i), System.mapLibraryName(name))
				If loadLibrary0(fromClass, libfile) Then Return
				libfile = ClassLoaderHelper.mapAlternativeName(libfile)
				If libfile IsNot Nothing AndAlso loadLibrary0(fromClass, libfile) Then Return
			Next i
			If loader IsNot Nothing Then
				For i As Integer = 0 To usr_paths.Length - 1
					Dim libfile As New File(usr_paths(i), System.mapLibraryName(name))
					If loadLibrary0(fromClass, libfile) Then Return
					libfile = ClassLoaderHelper.mapAlternativeName(libfile)
					If libfile IsNot Nothing AndAlso loadLibrary0(fromClass, libfile) Then Return
				Next i
			End If
			' Oops, it failed
			Throw New UnsatisfiedLinkError("no " & name & " in java.library.path")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function findBuiltinLib(ByVal name As String) As String
		End Function

		Private Shared Function loadLibrary0(ByVal fromClass As [Class], ByVal file As java.io.File) As Boolean
			' Check to see if we're attempting to access a static library
			Dim name As String = findBuiltinLib(file.name)
			Dim isBuiltin As Boolean = (name IsNot Nothing)
			If Not isBuiltin Then
				Dim exists As Boolean = java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
					IsNot Nothing
				If Not exists Then Return False
				Try
					name = file.canonicalPath
				Catch e As java.io.IOException
					Return False
				End Try
			End If
			Dim loader As  ClassLoader = If(fromClass Is Nothing, Nothing, fromClass.classLoader)
			Dim libs As List(Of NativeLibrary) = If(loader IsNot Nothing, loader.nativeLibraries, systemNativeLibraries)
			SyncLock libs
				Dim size As Integer = libs.Count
				For i As Integer = 0 To size - 1
					Dim [lib] As NativeLibrary = libs(i)
					If name.Equals([lib].name) Then Return True
				Next i

				SyncLock loadedLibraryNames
					If loadedLibraryNames.Contains(name) Then Throw New UnsatisfiedLinkError("Native Library " & name & " already loaded in another classloader")
	'                 If the library is being loaded (must be by the same thread,
	'                 * because Runtime.load and Runtime.loadLibrary are
	'                 * synchronous). The reason is can occur is that the JNI_OnLoad
	'                 * function can cause another loadLibrary invocation.
	'                 *
	'                 * Thus we can use a static stack to hold the list of libraries
	'                 * we are loading.
	'                 *
	'                 * If there is a pending load operation for the library, we
	'                 * immediately return success; otherwise, we raise
	'                 * UnsatisfiedLinkError.
	'                 
					Dim n As Integer = nativeLibraryContext.Count
					For i As Integer = 0 To n - 1
						Dim [lib] As NativeLibrary = nativeLibraryContext.elementAt(i)
						If name.Equals([lib].name) Then
							If loader Is [lib].fromClass.classLoader Then
								Return True
							Else
								Throw New UnsatisfiedLinkError("Native Library " & name & " is being loaded in another classloader")
							End If
						End If
					Next i
					Dim [lib] As New NativeLibrary(fromClass, name, isBuiltin)
					nativeLibraryContext.Push([lib])
					Try
						[lib].load(name, isBuiltin)
					Finally
						nativeLibraryContext.Pop()
					End Try
					If [lib].loaded Then
						loadedLibraryNames.Add(name)
						libs.Add([lib])
						Return True
					End If
					Return False
				End SyncLock
			End SyncLock
		End Function

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As Object
				Return If(file.exists(),  java.lang.[Boolean].TRUE, Nothing)
			End Function
		End Class

		' Invoked in the VM class linking code.
		Shared Function findNative(ByVal loader As  ClassLoader, ByVal name As String) As Long
			Dim libs As List(Of NativeLibrary) = If(loader IsNot Nothing, loader.nativeLibraries, systemNativeLibraries)
			SyncLock libs
				Dim size As Integer = libs.Count
				For i As Integer = 0 To size - 1
					Dim [lib] As NativeLibrary = libs(i)
					Dim entry As Long = [lib].find(name)
					If entry <> 0 Then Return entry
				Next i
			End SyncLock
			Return 0
		End Function


		' -- Assertion management --

		Friend ReadOnly assertionLock As Object

		' The default toggle for assertion checking.
		' @GuardedBy("assertionLock")
		Private defaultAssertionStatus As Boolean = False

		' Maps String packageName to Boolean package default assertion status Note
		' that the default package is placed under a null map key.  If this field
		' is null then we are delegating assertion status queries to the VM, i.e.,
		' none of this ClassLoader's assertion status modification methods have
		' been invoked.
		' @GuardedBy("assertionLock")
		Private packageAssertionStatus As IDictionary(Of String, Boolean?) = Nothing

		' Maps String fullyQualifiedClassName to Boolean assertionStatus If this
		' field is null then we are delegating assertion status queries to the VM,
		' i.e., none of this ClassLoader's assertion status modification methods
		' have been invoked.
		' @GuardedBy("assertionLock")
		Friend classAssertionStatus As IDictionary(Of String, Boolean?) = Nothing

		''' <summary>
		''' Sets the default assertion status for this class loader.  This setting
		''' determines whether classes loaded by this class loader and initialized
		''' in the future will have assertions enabled or disabled by default.
		''' This setting may be overridden on a per-package or per-class basis by
		''' invoking <seealso cref="#setPackageAssertionStatus(String, boolean)"/> or {@link
		''' #setClassAssertionStatus(String, boolean)}.
		''' </summary>
		''' <param name="enabled">
		'''         <tt>true</tt> if classes loaded by this class loader will
		'''         henceforth have assertions enabled by default, <tt>false</tt>
		'''         if they will have assertions disabled by default.
		''' 
		''' @since  1.4 </param>
		Public Overridable Property defaultAssertionStatus As Boolean
			Set(ByVal enabled As Boolean)
				SyncLock assertionLock
					If classAssertionStatus Is Nothing Then initializeJavaAssertionMaps()
    
					defaultAssertionStatus = enabled
				End SyncLock
			End Set
		End Property

		''' <summary>
		''' Sets the package default assertion status for the named package.  The
		''' package default assertion status determines the assertion status for
		''' classes initialized in the future that belong to the named package or
		''' any of its "subpackages".
		''' 
		''' <p> A subpackage of a package named p is any package whose name begins
		''' with "<tt>p.</tt>".  For example, <tt>javax.swing.text</tt> is a
		''' subpackage of <tt>javax.swing</tt>, and both <tt>java.util</tt> and
		''' <tt>java.lang.reflect</tt> are subpackages of <tt>java</tt>.
		''' 
		''' <p> In the event that multiple package defaults apply to a given [Class],
		''' the package default pertaining to the most specific package takes
		''' precedence over the others.  For example, if <tt>javax.lang</tt> and
		''' <tt>javax.lang.reflect</tt> both have package defaults associated with
		''' them, the latter package default applies to classes in
		''' <tt>javax.lang.reflect</tt>.
		''' 
		''' <p> Package defaults take precedence over the class loader's default
		''' assertion status, and may be overridden on a per-class basis by invoking
		''' <seealso cref="#setClassAssertionStatus(String, boolean)"/>.  </p>
		''' </summary>
		''' <param name="packageName">
		'''         The name of the package whose package default assertion status
		'''         is to be set. A <tt>null</tt> value indicates the unnamed
		'''         package that is "current"
		'''         (see section 7.4.2 of
		'''         <cite>The Java&trade; Language Specification</cite>.)
		''' </param>
		''' <param name="enabled">
		'''         <tt>true</tt> if classes loaded by this classloader and
		'''         belonging to the named package or any of its subpackages will
		'''         have assertions enabled by default, <tt>false</tt> if they will
		'''         have assertions disabled by default.
		''' 
		''' @since  1.4 </param>
		Public Overridable Sub setPackageAssertionStatus(ByVal packageName As String, ByVal enabled As Boolean)
			SyncLock assertionLock
				If packageAssertionStatus Is Nothing Then initializeJavaAssertionMaps()

			packageAssertionStatus(packageName) = enabled
			End SyncLock
		End Sub

		''' <summary>
		''' Sets the desired assertion status for the named top-level class in this
		''' class loader and any nested classes contained therein.  This setting
		''' takes precedence over the class loader's default assertion status, and
		''' over any applicable per-package default.  This method has no effect if
		''' the named class has already been initialized.  (Once a class is
		''' initialized, its assertion status cannot change.)
		''' 
		''' <p> If the named class is not a top-level [Class], this invocation will
		''' have no effect on the actual assertion status of any class. </p>
		''' </summary>
		''' <param name="className">
		'''         The fully qualified class name of the top-level class whose
		'''         assertion status is to be set.
		''' </param>
		''' <param name="enabled">
		'''         <tt>true</tt> if the named class is to have assertions
		'''         enabled when (and if) it is initialized, <tt>false</tt> if the
		'''         class is to have assertions disabled.
		''' 
		''' @since  1.4 </param>
		Public Overridable Sub setClassAssertionStatus(ByVal className As String, ByVal enabled As Boolean)
			SyncLock assertionLock
				If classAssertionStatus Is Nothing Then initializeJavaAssertionMaps()

				classAssertionStatus(className) = enabled
			End SyncLock
		End Sub

		''' <summary>
		''' Sets the default assertion status for this class loader to
		''' <tt>false</tt> and discards any package defaults or class assertion
		''' status settings associated with the class loader.  This method is
		''' provided so that class loaders can be made to ignore any command line or
		''' persistent assertion status settings and "start with a clean slate."
		''' 
		''' @since  1.4
		''' </summary>
		Public Overridable Sub clearAssertionStatus()
	'        
	'         * Whether or not "Java assertion maps" are initialized, set
	'         * them to empty maps, effectively ignoring any present settings.
	'         
			SyncLock assertionLock
				classAssertionStatus = New Dictionary(Of )
			packageAssertionStatus = New Dictionary(Of )
				defaultAssertionStatus = False
			End SyncLock
		End Sub

		''' <summary>
		''' Returns the assertion status that would be assigned to the specified
		''' class if it were to be initialized at the time this method is invoked.
		''' If the named class has had its assertion status set, the most recent
		''' setting will be returned; otherwise, if any package default assertion
		''' status pertains to this [Class], the most recent setting for the most
		''' specific pertinent package default assertion status is returned;
		''' otherwise, this class loader's default assertion status is returned.
		''' </p>
		''' </summary>
		''' <param name="className">
		'''         The fully qualified class name of the class whose desired
		'''         assertion status is being queried.
		''' </param>
		''' <returns>  The desired assertion status of the specified class.
		''' </returns>
		''' <seealso cref=  #setClassAssertionStatus(String, boolean) </seealso>
		''' <seealso cref=  #setPackageAssertionStatus(String, boolean) </seealso>
		''' <seealso cref=  #setDefaultAssertionStatus(boolean)
		''' 
		''' @since  1.4 </seealso>
		Friend Overridable Function desiredAssertionStatus(ByVal className As String) As Boolean
			SyncLock assertionLock
				' assert classAssertionStatus   != null;
				' assert packageAssertionStatus != null;

				' Check for a class entry
				Dim result As Boolean? = classAssertionStatus(className)
				If result IsNot Nothing Then Return result

				' Check for most specific package entry
				Dim dotIndex As Integer = className.LastIndexOf(".")
				If dotIndex < 0 Then ' default package
					result = packageAssertionStatus(Nothing)
					If result IsNot Nothing Then Return result
				End If
				Do While dotIndex > 0
					className = className.Substring(0, dotIndex)
					result = packageAssertionStatus(className)
					If result IsNot Nothing Then Return result
					dotIndex = className.LastIndexOf(".", dotIndex-1)
				Loop

				' Return the classloader default
				Return defaultAssertionStatus
			End SyncLock
		End Function

		' Set up the assertions with information provided by the VM.
		' Note: Should only be called inside a synchronized block
		Private Sub initializeJavaAssertionMaps()
			' assert Thread.holdsLock(assertionLock);

			classAssertionStatus = New Dictionary(Of )
		packageAssertionStatus = New Dictionary(Of )
			Dim directives As AssertionStatusDirectives = retrieveDirectives()

			For i As Integer = 0 To directives.classes.Length - 1
				classAssertionStatus(directives.classes(i)) = directives.classEnabled(i)
			Next i

			For i As Integer = 0 To directives.packages.Length - 1
			packageAssertionStatus(directives.packages(i)) = directives.packageEnabled(i)
			Next i

			defaultAssertionStatus = directives.deflt
		End Sub

		' Retrieves the assertion directives from the VM.
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function retrieveDirectives() As AssertionStatusDirectives
		End Function
	End Class


	Friend Class SystemClassLoaderAction
		Implements java.security.PrivilegedExceptionAction(Of ClassLoader)

		Private parent As  ClassLoader

		Friend Sub New(ByVal parent As  ClassLoader)
			Me.parent = parent
		End Sub

		Public Overridable Function run() As  ClassLoader
			Dim cls As String = System.getProperty("java.system.class.loader")
			If cls Is Nothing Then Return parent

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim ctor As Constructor(Of ?) = Type.GetType(cls, True, parent).getDeclaredConstructor(New [Class]() { GetType(ClassLoader) })
			Dim sys As  ClassLoader = CType(ctor.newInstance(New Object() { parent }), ClassLoader)
			Thread.CurrentThread.contextClassLoader = sys
			Return sys
		End Function
	End Class

End Namespace