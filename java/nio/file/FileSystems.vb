Imports System

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

Namespace java.nio.file


	''' <summary>
	''' Factory methods for file systems. This class defines the {@link #getDefault
	''' getDefault} method to get the default file system and factory methods to
	''' construct other types of file systems.
	''' 
	''' <p> The first invocation of any of the methods defined by this class causes
	''' the default <seealso cref="FileSystemProvider provider"/> to be loaded. The default
	''' provider, identified by the URI scheme "file", creates the <seealso cref="FileSystem"/>
	''' that provides access to the file systems accessible to the Java virtual
	''' machine. If the process of loading or initializing the default provider fails
	''' then an unspecified error is thrown.
	''' 
	''' <p> The first invocation of the {@link FileSystemProvider#installedProviders
	''' installedProviders} method, by way of invoking any of the {@code
	''' newFileSystem} methods defined by this [Class], locates and loads all
	''' installed file system providers. Installed providers are loaded using the
	''' service-provider loading facility defined by the <seealso cref="ServiceLoader"/> class.
	''' Installed providers are loaded using the system class loader. If the
	''' system class loader cannot be found then the extension class loader is used;
	''' if there is no extension class loader then the bootstrap class loader is used.
	''' Providers are typically installed by placing them in a JAR file on the
	''' application class path or in the extension directory, the JAR file contains a
	''' provider-configuration file named {@code java.nio.file.spi.FileSystemProvider}
	''' in the resource directory {@code META-INF/services}, and the file lists one or
	''' more fully-qualified names of concrete subclass of <seealso cref="FileSystemProvider"/>
	''' that have a zero argument constructor.
	''' The ordering that installed providers are located is implementation specific.
	''' If a provider is instantiated and its {@link FileSystemProvider#getScheme()
	''' getScheme} returns the same URI scheme of a provider that was previously
	''' instantiated then the most recently instantiated duplicate is discarded. URI
	''' schemes are compared without regard to case. During construction a provider
	''' may safely access files associated with the default provider but care needs
	''' to be taken to avoid circular loading of other installed providers. If
	''' circular loading of installed providers is detected then an unspecified error
	''' is thrown.
	''' 
	''' <p> This class also defines factory methods that allow a <seealso cref="ClassLoader"/>
	''' to be specified when locating a provider. As with installed providers, the
	''' provider classes are identified by placing the provider configuration file
	''' in the resource directory {@code META-INF/services}.
	''' 
	''' <p> If a thread initiates the loading of the installed file system providers
	''' and another thread invokes a method that also attempts to load the providers
	''' then the method will block until the loading completes.
	''' 
	''' @since 1.7
	''' </summary>

	Public NotInheritable Class FileSystems
		Private Sub New()
		End Sub

		' lazy initialization of default file system
		Private Class DefaultFileSystemHolder
			Friend Shared ReadOnly defaultFileSystem_Renamed As FileSystem = defaultFileSystem()

			' returns default file system
			Private Shared Function defaultFileSystem() As FileSystem
				' load default provider
				Dim provider As java.nio.file.spi.FileSystemProvider = java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)

				' return file system
				Return provider.getFileSystem(java.net.URI.create("file:///"))
			End Function

			Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
				Implements java.security.PrivilegedAction(Of T)

				Public Overridable Function run() As java.nio.file.spi.FileSystemProvider
					Return defaultProvider
				End Function
			End Class

			' returns default provider
			PrivateShared ReadOnly PropertydefaultProvider As java.nio.file.spi.FileSystemProvider
				Get
					Dim provider As java.nio.file.spi.FileSystemProvider = sun.nio.fs.DefaultFileSystemProvider.create()
    
					' if the property java.nio.file.spi.DefaultFileSystemProvider is
					' set then its value is the name of the default provider (or a list)
					Dim propValue As String = System.getProperty("java.nio.file.spi.DefaultFileSystemProvider")
					If propValue IsNot Nothing Then
						For Each cn As String In propValue.Split(",")
							Try
								Dim c As  [Class] = Type.GetType(cn, True, ClassLoader.systemClassLoader)
	'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
								Dim ctor As Constructor(Of ?) = c.getDeclaredConstructor(GetType(java.nio.file.spi.FileSystemProvider))
								provider = CType(ctor.newInstance(provider), java.nio.file.spi.FileSystemProvider)
    
								' must be "file"
								If Not provider.scheme.Equals("file") Then Throw New [Error]("Default provider must use scheme 'file'")
    
							Catch x As Exception
								Throw New [Error](x)
							End Try
						Next cn
					End If
					Return provider
				End Get
			End Property
		End Class

		''' <summary>
		''' Returns the default {@code FileSystem}. The default file system creates
		''' objects that provide access to the file systems accessible to the Java
		''' virtual machine. The <em>working directory</em> of the file system is
		''' the current user directory, named by the system property {@code user.dir}.
		''' This allows for interoperability with the <seealso cref="java.io.File java.io.File"/>
		''' class.
		''' 
		''' <p> The first invocation of any of the methods defined by this class
		''' locates the default <seealso cref="FileSystemProvider provider"/> object. Where the
		''' system property {@code java.nio.file.spi.DefaultFileSystemProvider} is
		''' not defined then the default provider is a system-default provider that
		''' is invoked to create the default file system.
		''' 
		''' <p> If the system property {@code java.nio.file.spi.DefaultFileSystemProvider}
		''' is defined then it is taken to be a list of one or more fully-qualified
		''' names of concrete provider classes identified by the URI scheme
		''' {@code "file"}. Where the property is a list of more than one name then
		''' the names are separated by a comma. Each class is loaded, using the system
		''' class loader, and instantiated by invoking a one argument constructor
		''' whose formal parameter type is {@code FileSystemProvider}. The providers
		''' are loaded and instantiated in the order they are listed in the property.
		''' If this process fails or a provider's scheme is not equal to {@code "file"}
		''' then an unspecified error is thrown. URI schemes are normally compared
		''' without regard to case but for the default provider, the scheme is
		''' required to be {@code "file"}. The first provider class is instantiated
		''' by invoking it with a reference to the system-default provider.
		''' The second provider class is instantiated by invoking it with a reference
		''' to the first provider instance. The third provider class is instantiated
		''' by invoking it with a reference to the second instance, and so on. The
		''' last provider to be instantiated becomes the default provider; its {@code
		''' getFileSystem} method is invoked with the URI {@code "file:///"} to
		''' get a reference to the default file system.
		''' 
		''' <p> Subsequent invocations of this method return the file system that was
		''' returned by the first invocation.
		''' </summary>
		''' <returns>  the default file system </returns>
		PublicShared ReadOnly Property[default] As FileSystem
			Get
				Return DefaultFileSystemHolder.defaultFileSystem_Renamed
			End Get
		End Property

		''' <summary>
		''' Returns a reference to an existing {@code FileSystem}.
		''' 
		''' <p> This method iterates over the {@link FileSystemProvider#installedProviders()
		''' installed} providers to locate the provider that is identified by the URI
		''' <seealso cref="URI#getScheme scheme"/> of the given URI. URI schemes are compared
		''' without regard to case. The exact form of the URI is highly provider
		''' dependent. If found, the provider's {@link FileSystemProvider#getFileSystem
		''' getFileSystem} method is invoked to obtain a reference to the {@code
		''' FileSystem}.
		''' 
		''' <p> Once a file system created by this provider is {@link FileSystem#close
		''' closed} it is provider-dependent if this method returns a reference to
		''' the closed file system or throws <seealso cref="FileSystemNotFoundException"/>.
		''' If the provider allows a new file system to be created with the same URI
		''' as a file system it previously created then this method throws the
		''' exception if invoked after the file system is closed (and before a new
		''' instance is created by the <seealso cref="#newFileSystem newFileSystem"/> method).
		''' 
		''' <p> If a security manager is installed then a provider implementation
		''' may require to check a permission before returning a reference to an
		''' existing file system. In the case of the {@link FileSystems#getDefault
		''' default} file system, no permission check is required.
		''' </summary>
		''' <param name="uri">  the URI to locate the file system
		''' </param>
		''' <returns>  the reference to the file system
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''          if the pre-conditions for the {@code uri} parameter are not met </exception>
		''' <exception cref="FileSystemNotFoundException">
		'''          if the file system, identified by the URI, does not exist </exception>
		''' <exception cref="ProviderNotFoundException">
		'''          if a provider supporting the URI scheme is not installed </exception>
		''' <exception cref="SecurityException">
		'''          if a security manager is installed and it denies an unspecified
		'''          permission </exception>
		Public Shared Function getFileSystem(  uri As java.net.URI) As FileSystem
			Dim scheme As String = uri.scheme
			For Each provider As java.nio.file.spi.FileSystemProvider In java.nio.file.spi.FileSystemProvider.installedProviders()
				If scheme.equalsIgnoreCase(provider.scheme) Then Return provider.getFileSystem(uri)
			Next provider
			Throw New ProviderNotFoundException("Provider """ & scheme & """ not found")
		End Function

		''' <summary>
		''' Constructs a new file system that is identified by a <seealso cref="URI"/>
		''' 
		''' <p> This method iterates over the {@link FileSystemProvider#installedProviders()
		''' installed} providers to locate the provider that is identified by the URI
		''' <seealso cref="URI#getScheme scheme"/> of the given URI. URI schemes are compared
		''' without regard to case. The exact form of the URI is highly provider
		''' dependent. If found, the provider's {@link FileSystemProvider#newFileSystem(URI,Map)
		''' newFileSystem(URI,Map)} method is invoked to construct the new file system.
		''' 
		''' <p> Once a file system is <seealso cref="FileSystem#close closed"/> it is
		''' provider-dependent if the provider allows a new file system to be created
		''' with the same URI as a file system it previously created.
		''' 
		''' <p> <b>Usage Example:</b>
		''' Suppose there is a provider identified by the scheme {@code "memory"}
		''' installed:
		''' <pre>
		'''   Map&lt;String,String&gt; env = new HashMap&lt;&gt;();
		'''   env.put("capacity", "16G");
		'''   env.put("blockSize", "4k");
		'''   FileSystem fs = FileSystems.newFileSystem(URI.create("memory:///?name=logfs"), env);
		''' </pre>
		''' </summary>
		''' <param name="uri">
		'''          the URI identifying the file system </param>
		''' <param name="env">
		'''          a map of provider specific properties to configure the file system;
		'''          may be empty
		''' </param>
		''' <returns>  a new file system
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''          if the pre-conditions for the {@code uri} parameter are not met,
		'''          or the {@code env} parameter does not contain properties required
		'''          by the provider, or a property value is invalid </exception>
		''' <exception cref="FileSystemAlreadyExistsException">
		'''          if the file system has already been created </exception>
		''' <exception cref="ProviderNotFoundException">
		'''          if a provider supporting the URI scheme is not installed </exception>
		''' <exception cref="IOException">
		'''          if an I/O error occurs creating the file system </exception>
		''' <exception cref="SecurityException">
		'''          if a security manager is installed and it denies an unspecified
		'''          permission required by the file system provider implementation </exception>
		Public Shared Function newFileSystem(Of T1)(  uri As java.net.URI,   env As Map(Of T1)) As FileSystem
			Return newFileSystem(uri, env, Nothing)
		End Function

		''' <summary>
		''' Constructs a new file system that is identified by a <seealso cref="URI"/>
		''' 
		''' <p> This method first attempts to locate an installed provider in exactly
		''' the same manner as the <seealso cref="#newFileSystem(URI,Map) newFileSystem(URI,Map)"/>
		''' method. If none of the installed providers support the URI scheme then an
		''' attempt is made to locate the provider using the given class loader. If a
		''' provider supporting the URI scheme is located then its {@link
		''' FileSystemProvider#newFileSystem(URI,Map) newFileSystem(URI,Map)} is
		''' invoked to construct the new file system.
		''' </summary>
		''' <param name="uri">
		'''          the URI identifying the file system </param>
		''' <param name="env">
		'''          a map of provider specific properties to configure the file system;
		'''          may be empty </param>
		''' <param name="loader">
		'''          the class loader to locate the provider or {@code null} to only
		'''          attempt to locate an installed provider
		''' </param>
		''' <returns>  a new file system
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''          if the pre-conditions for the {@code uri} parameter are not met,
		'''          or the {@code env} parameter does not contain properties required
		'''          by the provider, or a property value is invalid </exception>
		''' <exception cref="FileSystemAlreadyExistsException">
		'''          if the URI scheme identifies an installed provider and the file
		'''          system has already been created </exception>
		''' <exception cref="ProviderNotFoundException">
		'''          if a provider supporting the URI scheme is not found </exception>
		''' <exception cref="ServiceConfigurationError">
		'''          when an error occurs while loading a service provider </exception>
		''' <exception cref="IOException">
		'''          an I/O error occurs creating the file system </exception>
		''' <exception cref="SecurityException">
		'''          if a security manager is installed and it denies an unspecified
		'''          permission required by the file system provider implementation </exception>
		Public Shared Function newFileSystem(Of T1)(  uri As java.net.URI,   env As Map(Of T1),   loader As  ClassLoader) As FileSystem
			Dim scheme As String = uri.scheme

			' check installed providers
			For Each provider As java.nio.file.spi.FileSystemProvider In java.nio.file.spi.FileSystemProvider.installedProviders()
				If scheme.equalsIgnoreCase(provider.scheme) Then Return provider.newFileSystem(uri, env)
			Next provider

			' if not found, use service-provider loading facility
			If loader IsNot Nothing Then
				Dim sl As ServiceLoader(Of java.nio.file.spi.FileSystemProvider) = ServiceLoader.load(GetType(java.nio.file.spi.FileSystemProvider), loader)
				For Each provider As java.nio.file.spi.FileSystemProvider In sl
					If scheme.equalsIgnoreCase(provider.scheme) Then Return provider.newFileSystem(uri, env)
				Next provider
			End If

			Throw New ProviderNotFoundException("Provider """ & scheme & """ not found")
		End Function

		''' <summary>
		''' Constructs a new {@code FileSystem} to access the contents of a file as a
		''' file system.
		''' 
		''' <p> This method makes use of specialized providers that create pseudo file
		''' systems where the contents of one or more files is treated as a file
		''' system.
		''' 
		''' <p> This method iterates over the {@link FileSystemProvider#installedProviders()
		''' installed} providers. It invokes, in turn, each provider's {@link
		''' FileSystemProvider#newFileSystem(Path,Map) newFileSystem(Path,Map)} method
		''' with an empty map. If a provider returns a file system then the iteration
		''' terminates and the file system is returned. If none of the installed
		''' providers return a {@code FileSystem} then an attempt is made to locate
		''' the provider using the given class loader. If a provider returns a file
		''' system then the lookup terminates and the file system is returned.
		''' </summary>
		''' <param name="path">
		'''          the path to the file </param>
		''' <param name="loader">
		'''          the class loader to locate the provider or {@code null} to only
		'''          attempt to locate an installed provider
		''' </param>
		''' <returns>  a new file system
		''' </returns>
		''' <exception cref="ProviderNotFoundException">
		'''          if a provider supporting this file type cannot be located </exception>
		''' <exception cref="ServiceConfigurationError">
		'''          when an error occurs while loading a service provider </exception>
		''' <exception cref="IOException">
		'''          if an I/O error occurs </exception>
		''' <exception cref="SecurityException">
		'''          if a security manager is installed and it denies an unspecified
		'''          permission </exception>
		Public Shared Function newFileSystem(  path As Path,   loader As  ClassLoader) As FileSystem
			If path Is Nothing Then Throw New NullPointerException
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim env As Map(Of String, ?) = Collections.emptyMap()

			' check installed providers
			For Each provider As java.nio.file.spi.FileSystemProvider In java.nio.file.spi.FileSystemProvider.installedProviders()
				Try
					Return provider.newFileSystem(path, env)
				Catch uoe As UnsupportedOperationException
				End Try
			Next provider

			' if not found, use service-provider loading facility
			If loader IsNot Nothing Then
				Dim sl As ServiceLoader(Of java.nio.file.spi.FileSystemProvider) = ServiceLoader.load(GetType(java.nio.file.spi.FileSystemProvider), loader)
				For Each provider As java.nio.file.spi.FileSystemProvider In sl
					Try
						Return provider.newFileSystem(path, env)
					Catch uoe As UnsupportedOperationException
					End Try
				Next provider
			End If

			Throw New ProviderNotFoundException("Provider not found")
		End Function
	End Class

End Namespace