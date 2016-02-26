Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Threading

'
' * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.imageio.spi


	''' <summary>
	''' A registry for service provider instances.  Service provider
	''' classes may be detected at run time by means of meta-information in
	''' the JAR files containing them.  The intent is that it be relatively
	''' inexpensive to load and inspect all available service provider
	''' classes.  These classes may them be used to locate and instantiate
	''' more heavyweight classes that will perform actual work, in this
	''' case instances of <code>ImageReader</code>,
	''' <code>ImageWriter</code>, <code>ImageTranscoder</code>,
	''' <code>ImageInputStream</code>, and <code>ImageOutputStream</code>.
	''' 
	''' <p> Service providers found on the system classpath (typically
	''' the <code>lib/ext</code> directory in the Java
	''' installation directory) are automatically loaded as soon as this class is
	''' instantiated.
	''' 
	''' <p> When the <code>registerApplicationClasspathSpis</code> method
	''' is called, service provider instances declared in the
	''' meta-information section of JAR files on the application class path
	''' are loaded.  To declare a service provider, a <code>services</code>
	''' subdirectory is placed within the <code>META-INF</code> directory
	''' that is present in every JAR file.  This directory contains a file
	''' for each service provider interface that has one or more
	''' implementation classes present in the JAR file.  For example, if
	''' the JAR file contained a class named
	''' <code>com.mycompany.imageio.MyFormatReaderSpi</code> which
	''' implements the <code>ImageReaderSpi</code> interface, the JAR file
	''' would contain a file named:
	''' 
	''' <pre>
	''' META-INF/services/javax.imageio.spi.ImageReaderSpi
	''' </pre>
	''' 
	''' containing the line:
	''' 
	''' <pre>
	''' com.mycompany.imageio.MyFormatReaderSpi
	''' </pre>
	''' 
	''' <p> The service provider classes are intended to be lightweight
	''' and quick to load.  Implementations of these interfaces
	''' should avoid complex dependencies on other classes and on
	''' native code.
	''' 
	''' <p> It is also possible to manually add service providers not found
	''' automatically, as well as to remove those that are using the
	''' interfaces of the <code>ServiceRegistry</code> class.  Thus
	''' the application may customize the contents of the registry as it
	''' sees fit.
	''' 
	''' <p> For more details on declaring service providers, and the JAR
	''' format in general, see the <a
	''' href="{@docRoot}/../technotes/guides/jar/jar.html">
	''' JAR File Specification</a>.
	''' 
	''' </summary>
	Public NotInheritable Class IIORegistry
		Inherits ServiceRegistry

		''' <summary>
		''' A <code>Vector</code> containing the valid IIO registry
		''' categories (superinterfaces) to be used in the constructor.
		''' </summary>
		Private Shared ReadOnly initialCategories As New ArrayList(5)

		Shared Sub New()
			initialCategories.Add(GetType(ImageReaderSpi))
			initialCategories.Add(GetType(ImageWriterSpi))
			initialCategories.Add(GetType(ImageTranscoderSpi))
			initialCategories.Add(GetType(ImageInputStreamSpi))
			initialCategories.Add(GetType(ImageOutputStreamSpi))
		End Sub

		''' <summary>
		''' Set up the valid service provider categories and automatically
		''' register all available service providers.
		''' 
		''' <p> The constructor is private in order to prevent creation of
		''' additional instances.
		''' </summary>
		Private Sub New()
			MyBase.New(initialCategories.GetEnumerator())
			registerStandardSpis()
			registerApplicationClasspathSpis()
		End Sub

		''' <summary>
		''' Returns the default <code>IIORegistry</code> instance used by
		''' the Image I/O API.  This instance should be used for all
		''' registry functions.
		''' 
		''' <p> Each <code>ThreadGroup</code> will receive its own
		''' instance; this allows different <code>Applet</code>s in the
		''' same browser (for example) to each have their own registry.
		''' </summary>
		''' <returns> the default registry for the current
		''' <code>ThreadGroup</code>. </returns>
		Public Property Shared defaultInstance As IIORegistry
			Get
				Dim context As sun.awt.AppContext = sun.awt.AppContext.appContext
				Dim registry As IIORegistry = CType(context.get(GetType(IIORegistry)), IIORegistry)
				If registry Is Nothing Then
					' Create an instance for this AppContext
					registry = New IIORegistry
					context.put(GetType(IIORegistry), registry)
				End If
				Return registry
			End Get
		End Property

		Private Sub registerStandardSpis()
			' Hardwire standard SPIs
			registerServiceProvider(New com.sun.imageio.plugins.gif.GIFImageReaderSpi)
			registerServiceProvider(New com.sun.imageio.plugins.gif.GIFImageWriterSpi)
			registerServiceProvider(New com.sun.imageio.plugins.bmp.BMPImageReaderSpi)
			registerServiceProvider(New com.sun.imageio.plugins.bmp.BMPImageWriterSpi)
			registerServiceProvider(New com.sun.imageio.plugins.wbmp.WBMPImageReaderSpi)
			registerServiceProvider(New com.sun.imageio.plugins.wbmp.WBMPImageWriterSpi)
			registerServiceProvider(New com.sun.imageio.plugins.png.PNGImageReaderSpi)
			registerServiceProvider(New com.sun.imageio.plugins.png.PNGImageWriterSpi)
			registerServiceProvider(New com.sun.imageio.plugins.jpeg.JPEGImageReaderSpi)
			registerServiceProvider(New com.sun.imageio.plugins.jpeg.JPEGImageWriterSpi)
			registerServiceProvider(New com.sun.imageio.spi.FileImageInputStreamSpi)
			registerServiceProvider(New com.sun.imageio.spi.FileImageOutputStreamSpi)
			registerServiceProvider(New com.sun.imageio.spi.InputStreamImageInputStreamSpi)
			registerServiceProvider(New com.sun.imageio.spi.OutputStreamImageOutputStreamSpi)
			registerServiceProvider(New com.sun.imageio.spi.RAFImageInputStreamSpi)
			registerServiceProvider(New com.sun.imageio.spi.RAFImageOutputStreamSpi)

			registerInstalledProviders()
		End Sub

		''' <summary>
		''' Registers all available service providers found on the
		''' application class path, using the default
		''' <code>ClassLoader</code>.  This method is typically invoked by
		''' the <code>ImageIO.scanForPlugins</code> method.
		''' </summary>
		''' <seealso cref= javax.imageio.ImageIO#scanForPlugins </seealso>
		''' <seealso cref= ClassLoader#getResources </seealso>
		Public Sub registerApplicationClasspathSpis()
			' FIX: load only from application classpath

			Dim loader As ClassLoader = Thread.CurrentThread.contextClassLoader

			Dim ___categories As IEnumerator = categories
			Do While ___categories.hasNext()
				Dim c As Type = CType(___categories.next(), [Class])
				Dim riter As IEnumerator(Of IIOServiceProvider) = java.util.ServiceLoader.load(c, loader).GetEnumerator()
				Do While riter.MoveNext()
					Try
						' Note that the next() call is required to be inside
						' the try/catch block; see 6342404.
						Dim r As IIOServiceProvider = riter.Current
						registerServiceProvider(r)
					Catch err As java.util.ServiceConfigurationError
						If System.securityManager IsNot Nothing Then
							' In the applet case, we will catch the  error so
							' registration of other plugins can  proceed
							err.printStackTrace()
						Else
							' In the application case, we will  throw the
							' error to indicate app/system  misconfiguration
							Throw err
						End If
					End Try
				Loop
			Loop
		End Sub

		Private Sub registerInstalledProviders()
	'        
	'          We need to load installed providers from the
	'          system classpath (typically the <code>lib/ext</code>
	'          directory in in the Java installation directory)
	'          in the privileged mode in order to
	'          be able read corresponding jar files even if
	'          file read capability is restricted (like the
	'          applet context case).
	'         
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			java.security.PrivilegedAction doRegistration = New java.security.PrivilegedAction()
	'		{
	'				public Object run()
	'				{
	'					Iterator categories = getCategories();
	'					while (categories.hasNext())
	'					{
	'						Class c = (Class)categories.next();
	'						for (IIOServiceProvider p : ServiceLoader.loadInstalled(c))
	'						{
	'							registerServiceProvider(p);
	'						}
	'					}
	'					Return Me;
	'				}
	'			};

			java.security.AccessController.doPrivileged(doRegistration)
		End Sub
	End Class

End Namespace