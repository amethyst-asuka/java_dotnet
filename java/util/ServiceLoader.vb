Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Threading

'
' * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.util



	''' <summary>
	''' A simple service-provider loading facility.
	''' 
	''' <p> A <i>service</i> is a well-known set of interfaces and (usually
	''' abstract) classes.  A <i>service provider</i> is a specific implementation
	''' of a service.  The classes in a provider typically implement the interfaces
	''' and subclass the classes defined in the service itself.  Service providers
	''' can be installed in an implementation of the Java platform in the form of
	''' extensions, that is, jar files placed into any of the usual extension
	''' directories.  Providers can also be made available by adding them to the
	''' application's class path or by some other platform-specific means.
	''' 
	''' <p> For the purpose of loading, a service is represented by a single type,
	''' that is, a single interface or abstract class.  (A concrete class can be
	''' used, but this is not recommended.)  A provider of a given service contains
	''' one or more concrete classes that extend this <i>service type</i> with data
	''' and code specific to the provider.  The <i>provider class</i> is typically
	''' not the entire provider itself but rather a proxy which contains enough
	''' information to decide whether the provider is able to satisfy a particular
	''' request together with code that can create the actual provider on demand.
	''' The details of provider classes tend to be highly service-specific; no
	''' single class or interface could possibly unify them, so no such type is
	''' defined here.  The only requirement enforced by this facility is that
	''' provider classes must have a zero-argument constructor so that they can be
	''' instantiated during loading.
	''' 
	''' <p><a name="format"> A service provider is identified by placing a
	''' <i>provider-configuration file</i> in the resource directory
	''' <tt>META-INF/services</tt>.</a>  The file's name is the fully-qualified <a
	''' href="../lang/ClassLoader.html#name">binary name</a> of the service's type.
	''' The file contains a list of fully-qualified binary names of concrete
	''' provider classes, one per line.  Space and tab characters surrounding each
	''' name, as well as blank lines, are ignored.  The comment character is
	''' <tt>'#'</tt> (<tt>'&#92;u0023'</tt>,
	''' <font style="font-size:smaller;">NUMBER SIGN</font>); on
	''' each line all characters following the first comment character are ignored.
	''' The file must be encoded in UTF-8.
	''' 
	''' <p> If a particular concrete provider class is named in more than one
	''' configuration file, or is named in the same configuration file more than
	''' once, then the duplicates are ignored.  The configuration file naming a
	''' particular provider need not be in the same jar file or other distribution
	''' unit as the provider itself.  The provider must be accessible from the same
	''' class loader that was initially queried to locate the configuration file;
	''' note that this is not necessarily the class loader from which the file was
	''' actually loaded.
	''' 
	''' <p> Providers are located and instantiated lazily, that is, on demand.  A
	''' service loader maintains a cache of the providers that have been loaded so
	''' far.  Each invocation of the <seealso cref="#iterator iterator"/> method returns an
	''' iterator that first yields all of the elements of the cache, in
	''' instantiation order, and then lazily locates and instantiates any remaining
	''' providers, adding each one to the cache in turn.  The cache can be cleared
	''' via the <seealso cref="#reload reload"/> method.
	''' 
	''' <p> Service loaders always execute in the security context of the caller.
	''' Trusted system code should typically invoke the methods in this class, and
	''' the methods of the iterators which they return, from within a privileged
	''' security context.
	''' 
	''' <p> Instances of this class are not safe for use by multiple concurrent
	''' threads.
	''' 
	''' <p> Unless otherwise specified, passing a <tt>null</tt> argument to any
	''' method in this class will cause a <seealso cref="NullPointerException"/> to be thrown.
	''' 
	''' 
	''' <p><span style="font-weight: bold; padding-right: 1em">Example</span>
	''' Suppose we have a service type <tt>com.example.CodecSet</tt> which is
	''' intended to represent sets of encoder/decoder pairs for some protocol.  In
	''' this case it is an abstract class with two abstract methods:
	''' 
	''' <blockquote><pre>
	''' public abstract Encoder getEncoder(String encodingName);
	''' public abstract Decoder getDecoder(String encodingName);</pre></blockquote>
	''' 
	''' Each method returns an appropriate object or <tt>null</tt> if the provider
	''' does not support the given encoding.  Typical providers support more than
	''' one encoding.
	''' 
	''' <p> If <tt>com.example.impl.StandardCodecs</tt> is an implementation of the
	''' <tt>CodecSet</tt> service then its jar file also contains a file named
	''' 
	''' <blockquote><pre>
	''' META-INF/services/com.example.CodecSet</pre></blockquote>
	''' 
	''' <p> This file contains the single line:
	''' 
	''' <blockquote><pre>
	''' com.example.impl.StandardCodecs    # Standard codecs</pre></blockquote>
	''' 
	''' <p> The <tt>CodecSet</tt> class creates and saves a single service instance
	''' at initialization:
	''' 
	''' <blockquote><pre>
	''' private static ServiceLoader&lt;CodecSet&gt; codecSetLoader
	'''     = ServiceLoader.load(CodecSet.class);</pre></blockquote>
	''' 
	''' <p> To locate an encoder for a given encoding name it defines a static
	''' factory method which iterates through the known and available providers,
	''' returning only when it has located a suitable encoder or has run out of
	''' providers.
	''' 
	''' <blockquote><pre>
	''' public static Encoder getEncoder(String encodingName) {
	'''     for (CodecSet cp : codecSetLoader) {
	'''         Encoder enc = cp.getEncoder(encodingName);
	'''         if (enc != null)
	'''             return enc;
	'''     }
	'''     return null;
	''' }</pre></blockquote>
	''' 
	''' <p> A <tt>getDecoder</tt> method is defined similarly.
	''' 
	''' 
	''' <p><span style="font-weight: bold; padding-right: 1em">Usage Note</span> If
	''' the class path of a class loader that is used for provider loading includes
	''' remote network URLs then those URLs will be dereferenced in the process of
	''' searching for provider-configuration files.
	''' 
	''' <p> This activity is normal, although it may cause puzzling entries to be
	''' created in web-server logs.  If a web server is not configured correctly,
	''' however, then this activity may cause the provider-loading algorithm to fail
	''' spuriously.
	''' 
	''' <p> A web server should return an HTTP 404 (Not Found) response when a
	''' requested resource does not exist.  Sometimes, however, web servers are
	''' erroneously configured to return an HTTP 200 (OK) response along with a
	''' helpful HTML error page in such cases.  This will cause a {@link
	''' ServiceConfigurationError} to be thrown when this class attempts to parse
	''' the HTML page as a provider-configuration file.  The best solution to this
	''' problem is to fix the misconfigured web server to return the correct
	''' response code (HTTP 404) along with the HTML error page.
	''' </summary>
	''' @param  <S>
	'''         The type of the service to be loaded by this loader
	''' 
	''' @author Mark Reinhold
	''' @since 1.6 </param>

	Public NotInheritable Class ServiceLoader(Of S)
		Implements Iterable(Of S)

		Private Const PREFIX As String = "META-INF/services/"

		' The class or interface representing the service being loaded
		Private ReadOnly service As Class

		' The class loader used to locate, load, and instantiate providers
		Private ReadOnly loader As ClassLoader

		' The access control context taken when the ServiceLoader is created
		Private ReadOnly acc As java.security.AccessControlContext

		' Cached providers, in instantiation order
		Private providers As New LinkedHashMap(Of String, S)

		' The current lazy-lookup iterator
		Private lookupIterator As LazyIterator

		''' <summary>
		''' Clear this loader's provider cache so that all providers will be
		''' reloaded.
		''' 
		''' <p> After invoking this method, subsequent invocations of the {@link
		''' #iterator() iterator} method will lazily look up and instantiate
		''' providers from scratch, just as is done by a newly-created loader.
		''' 
		''' <p> This method is intended for use in situations in which new providers
		''' can be installed into a running Java virtual machine.
		''' </summary>
		Public Sub reload()
			providers.clear()
			lookupIterator = New LazyIterator(Me, service, loader)
		End Sub

		Private Sub New(ByVal svc As Class, ByVal cl As ClassLoader)
			service = Objects.requireNonNull(svc, "Service interface cannot be null")
			loader = If(cl Is Nothing, ClassLoader.systemClassLoader, cl)
			acc = If(System.securityManager IsNot Nothing, java.security.AccessController.context, Nothing)
			reload()
		End Sub

		Private Shared Sub fail(ByVal service As Class, ByVal msg As String, ByVal cause As Throwable)
			Throw New ServiceConfigurationError(service.name & ": " & msg, cause)
		End Sub

		Private Shared Sub fail(ByVal service As Class, ByVal msg As String)
			Throw New ServiceConfigurationError(service.name & ": " & msg)
		End Sub

		Private Shared Sub fail(ByVal service As Class, ByVal u As java.net.URL, ByVal line As Integer, ByVal msg As String)
			fail(service, u & ":" & line & ": " & msg)
		End Sub

		' Parse a single line from the given configuration file, adding the name
		' on the line to the names list.
		'
		Private Function parseLine(ByVal service As Class, ByVal u As java.net.URL, ByVal r As java.io.BufferedReader, ByVal lc As Integer, ByVal names As IList(Of String)) As Integer
			Dim ln As String = r.readLine()
			If ln Is Nothing Then Return -1
			Dim ci As Integer = ln.IndexOf("#"c)
			If ci >= 0 Then ln = ln.Substring(0, ci)
			ln = ln.Trim()
			Dim n As Integer = ln.length()
			If n <> 0 Then
				If (ln.IndexOf(" "c) >= 0) OrElse (ln.IndexOf(ControlChars.Tab) >= 0) Then fail(service, u, lc, "Illegal configuration-file syntax")
				Dim cp As Integer = ln.codePointAt(0)
				If Not Character.isJavaIdentifierStart(cp) Then fail(service, u, lc, "Illegal provider-class name: " & ln)
				For i As Integer = Character.charCount(cp) To n - 1 Step Character.charCount(cp)
					cp = ln.codePointAt(i)
					If (Not Character.isJavaIdentifierPart(cp)) AndAlso (cp <> AscW("."c)) Then fail(service, u, lc, "Illegal provider-class name: " & ln)
				Next i
				If (Not providers.containsKey(ln)) AndAlso (Not names.Contains(ln)) Then names.Add(ln)
			End If
			Return lc + 1
		End Function

		' Parse the content of the given URL as a provider-configuration file.
		'
		' @param  service
		'         The service type for which providers are being sought;
		'         used to construct error detail strings
		'
		' @param  u
		'         The URL naming the configuration file to be parsed
		'
		' @return A (possibly empty) iterator that will yield the provider-class
		'         names in the given configuration file that are not yet members
		'         of the returned set
		'
		' @throws ServiceConfigurationError
		'         If an I/O error occurs while reading from the given URL, or
		'         if a configuration-file format error is detected
		'
		Private Function parse(ByVal service As Class, ByVal u As java.net.URL) As IEnumerator(Of String)
			Dim [in] As java.io.InputStream = Nothing
			Dim r As java.io.BufferedReader = Nothing
			Dim names As New List(Of String)
			Try
				[in] = u.openStream()
				r = New java.io.BufferedReader(New java.io.InputStreamReader([in], "utf-8"))
				Dim lc As Integer = 1
				lc = parseLine(service, u, r, lc, names)
				Do While lc >= 0

					lc = parseLine(service, u, r, lc, names)
				Loop
			Catch x As java.io.IOException
				fail(service, "Error reading configuration file", x)
			Finally
				Try
					If r IsNot Nothing Then r.close()
					If [in] IsNot Nothing Then [in].close()
				Catch y As java.io.IOException
					fail(service, "Error closing configuration file", y)
				End Try
			End Try
			Return names.GetEnumerator()
		End Function

		' Private inner class implementing fully-lazy provider lookup
		'
		Private Class LazyIterator
			Implements IEnumerator(Of S)

			Private ReadOnly outerInstance As ServiceLoader


			Friend service As Class
			Friend loader As ClassLoader
			Friend configs As System.Collections.IEnumerator(Of java.net.URL) = Nothing
			Friend pending As IEnumerator(Of String) = Nothing
			Friend nextName As String = Nothing

			Private Sub New(ByVal outerInstance As ServiceLoader, ByVal service As Class, ByVal loader As ClassLoader)
					Me.outerInstance = outerInstance
				Me.service = service
				Me.loader = loader
			End Sub

			Private Function hasNextService() As Boolean
				If nextName IsNot Nothing Then Return True
				If configs Is Nothing Then
					Try
						Dim fullName As String = PREFIX + service.name
						If loader Is Nothing Then
							configs = ClassLoader.getSystemResources(fullName)
						Else
							configs = loader.getResources(fullName)
						End If
					Catch x As java.io.IOException
						fail(service, "Error locating configuration files", x)
					End Try
				End If
				Do While (pending Is Nothing) OrElse Not pending.MoveNext()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					If Not configs.hasMoreElements() Then Return False
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					pending = outerInstance.parse(service, configs.nextElement())
				Loop
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				nextName = pending.next()
				Return True
			End Function

			Private Function nextService() As S
				If Not hasNextService() Then Throw New java.util.NoSuchElementException
				Dim cn As String = nextName
				nextName = Nothing
				Dim c As Class = Nothing
				Try
					c = Type.GetType(cn, False, loader)
				Catch x As ClassNotFoundException
					fail(service, "Provider " & cn & " not found")
				End Try
				If Not c.IsSubclassOf(service) Then fail(service, "Provider " & cn & " not a subtype")
				Try
					Dim p As S = service.cast(c.newInstance())
					outerInstance.providers.put(cn, p)
					Return p
				Catch x As Throwable
					fail(service, "Provider " & cn & " could not be instantiated", x)
				End Try
				Throw New [Error]() ' This cannot happen
			End Function

			Public Overridable Function hasNext() As Boolean
				If outerInstance.acc Is Nothing Then
					Return hasNextService()
				Else
					Dim action As java.security.PrivilegedAction(Of Boolean?) = New PrivilegedActionAnonymousInnerClassHelper(Of T)
					Return java.security.AccessController.doPrivileged(action, outerInstance.acc)
				End If
			End Function

			Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
				Implements java.security.PrivilegedAction(Of T)

				Public Overridable Function run() As Boolean?
					Return outerInstance.hasNextService()
				End Function
			End Class

			Public Overridable Function [next]() As S
				If outerInstance.acc Is Nothing Then
					Return nextService()
				Else
					Dim action As java.security.PrivilegedAction(Of S) = New PrivilegedActionAnonymousInnerClassHelper2(Of T)
					Return java.security.AccessController.doPrivileged(action, outerInstance.acc)
				End If
			End Function

			Private Class PrivilegedActionAnonymousInnerClassHelper2(Of T)
				Implements java.security.PrivilegedAction(Of T)

				Public Overridable Function run() As S
					Return outerInstance.nextService()
				End Function
			End Class

			Public Overridable Sub remove()
				Throw New UnsupportedOperationException
			End Sub

		End Class

		''' <summary>
		''' Lazily loads the available providers of this loader's service.
		''' 
		''' <p> The iterator returned by this method first yields all of the
		''' elements of the provider cache, in instantiation order.  It then lazily
		''' loads and instantiates any remaining providers, adding each one to the
		''' cache in turn.
		''' 
		''' <p> To achieve laziness the actual work of parsing the available
		''' provider-configuration files and instantiating providers must be done by
		''' the iterator itself.  Its <seealso cref="java.util.Iterator#hasNext hasNext"/> and
		''' <seealso cref="java.util.Iterator#next next"/> methods can therefore throw a
		''' <seealso cref="ServiceConfigurationError"/> if a provider-configuration file
		''' violates the specified format, or if it names a provider class that
		''' cannot be found and instantiated, or if the result of instantiating the
		''' class is not assignable to the service type, or if any other kind of
		''' exception or error is thrown as the next provider is located and
		''' instantiated.  To write robust code it is only necessary to catch {@link
		''' ServiceConfigurationError} when using a service iterator.
		''' 
		''' <p> If such an error is thrown then subsequent invocations of the
		''' iterator will make a best effort to locate and instantiate the next
		''' available provider, but in general such recovery cannot be guaranteed.
		''' 
		''' <blockquote style="font-size: smaller; line-height: 1.2"><span
		''' style="padding-right: 1em; font-weight: bold">Design Note</span>
		''' Throwing an error in these cases may seem extreme.  The rationale for
		''' this behavior is that a malformed provider-configuration file, like a
		''' malformed class file, indicates a serious problem with the way the Java
		''' virtual machine is configured or is being used.  As such it is
		''' preferable to throw an error rather than try to recover or, even worse,
		''' fail silently.</blockquote>
		''' 
		''' <p> The iterator returned by this method does not support removal.
		''' Invoking its <seealso cref="java.util.Iterator#remove() remove"/> method will
		''' cause an <seealso cref="UnsupportedOperationException"/> to be thrown.
		''' 
		''' @implNote When adding providers to the cache, the {@link #iterator
		''' Iterator} processes resources in the order that the {@link
		''' java.lang.ClassLoader#getResources(java.lang.String)
		''' ClassLoader.getResources(String)} method finds the service configuration
		''' files.
		''' </summary>
		''' <returns>  An iterator that lazily loads providers for this loader's
		'''          service </returns>
		Public Function [iterator]() As IEnumerator(Of S) Implements Iterable(Of S).iterator
			Return New IteratorAnonymousInnerClassHelper(Of E)
		End Function

		Private Class IteratorAnonymousInnerClassHelper(Of E)
			Implements IEnumerator(Of E)

			Friend knownProviders As IEnumerator(Of KeyValuePair(Of String, S)) = outerInstance.providers.entrySet().GetEnumerator()

			Public Overridable Function hasNext() As Boolean
				If knownProviders.hasNext() Then Return True
				Return outerInstance.lookupIterator.hasNext()
			End Function

			Public Overridable Function [next]() As S
				If knownProviders.hasNext() Then Return knownProviders.next().value
				Return outerInstance.lookupIterator.next()
			End Function

			Public Overridable Sub remove()
				Throw New UnsupportedOperationException
			End Sub

		End Class

		''' <summary>
		''' Creates a new service loader for the given service type and class
		''' loader.
		''' </summary>
		''' @param  <S> the class of the service type
		''' </param>
		''' <param name="service">
		'''         The interface or abstract class representing the service
		''' </param>
		''' <param name="loader">
		'''         The class loader to be used to load provider-configuration files
		'''         and provider classes, or <tt>null</tt> if the system class
		'''         loader (or, failing that, the bootstrap class loader) is to be
		'''         used
		''' </param>
		''' <returns> A new service loader </returns>
		Public Shared Function load(Of S)(ByVal service As Class, ByVal loader As ClassLoader) As ServiceLoader(Of S)
			Return New ServiceLoader(Of )(service, loader)
		End Function

		''' <summary>
		''' Creates a new service loader for the given service type, using the
		''' current thread's {@link java.lang.Thread#getContextClassLoader
		''' context class loader}.
		''' 
		''' <p> An invocation of this convenience method of the form
		''' 
		''' <blockquote><pre>
		''' ServiceLoader.load(<i>service</i>)</pre></blockquote>
		''' 
		''' is equivalent to
		''' 
		''' <blockquote><pre>
		''' ServiceLoader.load(<i>service</i>,
		'''                    Thread.currentThread().getContextClassLoader())</pre></blockquote>
		''' </summary>
		''' @param  <S> the class of the service type
		''' </param>
		''' <param name="service">
		'''         The interface or abstract class representing the service
		''' </param>
		''' <returns> A new service loader </returns>
		Public Shared Function load(Of S)(ByVal service As Class) As ServiceLoader(Of S)
			Dim cl As ClassLoader = Thread.CurrentThread.contextClassLoader
			Return ServiceLoader.load(service, cl)
		End Function

		''' <summary>
		''' Creates a new service loader for the given service type, using the
		''' extension class loader.
		''' 
		''' <p> This convenience method simply locates the extension class loader,
		''' call it <tt><i>extClassLoader</i></tt>, and then returns
		''' 
		''' <blockquote><pre>
		''' ServiceLoader.load(<i>service</i>, <i>extClassLoader</i>)</pre></blockquote>
		''' 
		''' <p> If the extension class loader cannot be found then the system class
		''' loader is used; if there is no system class loader then the bootstrap
		''' class loader is used.
		''' 
		''' <p> This method is intended for use when only installed providers are
		''' desired.  The resulting service will only find and load providers that
		''' have been installed into the current Java virtual machine; providers on
		''' the application's class path will be ignored.
		''' </summary>
		''' @param  <S> the class of the service type
		''' </param>
		''' <param name="service">
		'''         The interface or abstract class representing the service
		''' </param>
		''' <returns> A new service loader </returns>
		Public Shared Function loadInstalled(Of S)(ByVal service As Class) As ServiceLoader(Of S)
			Dim cl As ClassLoader = ClassLoader.systemClassLoader
			Dim prev As ClassLoader = Nothing
			Do While cl IsNot Nothing
				prev = cl
				cl = cl.parent
			Loop
			Return ServiceLoader.load(service, prev)
		End Function

		''' <summary>
		''' Returns a string describing this service.
		''' </summary>
		''' <returns>  A descriptive string </returns>
		Public Overrides Function ToString() As String
			Return "java.util.ServiceLoader[" & service.name & "]"
		End Function

	End Class

End Namespace