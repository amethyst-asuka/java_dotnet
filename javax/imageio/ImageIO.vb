Imports System.Runtime.CompilerServices
Imports System.Collections

'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.imageio


	''' <summary>
	''' A class containing static convenience methods for locating
	''' <code>ImageReader</code>s and <code>ImageWriter</code>s, and
	''' performing simple encoding and decoding.
	''' 
	''' </summary>
	Public NotInheritable Class ImageIO

		Private Shared ReadOnly theRegistry As javax.imageio.spi.IIORegistry = javax.imageio.spi.IIORegistry.defaultInstance

		''' <summary>
		''' Constructor is private to prevent instantiation.
		''' </summary>
		Private Sub New()
		End Sub

		''' <summary>
		''' Scans for plug-ins on the application class path,
		''' loads their service provider classes, and registers a service
		''' provider instance for each one found with the
		''' <code>IIORegistry</code>.
		''' 
		''' <p>This method is needed because the application class path can
		''' theoretically change, or additional plug-ins may become available.
		''' Rather than re-scanning the classpath on every invocation of the
		''' API, the class path is scanned automatically only on the first
		''' invocation. Clients can call this method to prompt a re-scan.
		''' Thus this method need only be invoked by sophisticated applications
		''' which dynamically make new plug-ins available at runtime.
		''' 
		''' <p> The <code>getResources</code> method of the context
		''' <code>ClassLoader</code> is used locate JAR files containing
		''' files named
		''' <code>META-INF/services/javax.imageio.spi.</code><i>classname</i>,
		''' where <i>classname</i> is one of <code>ImageReaderSpi</code>,
		''' <code>ImageWriterSpi</code>, <code>ImageTranscoderSpi</code>,
		''' <code>ImageInputStreamSpi</code>, or
		''' <code>ImageOutputStreamSpi</code>, along the application class
		''' path.
		''' 
		''' <p> The contents of the located files indicate the names of
		''' actual implementation classes which implement the
		''' aforementioned service provider interfaces; the default class
		''' loader is then used to load each of these classes and to
		''' instantiate an instance of each class, which is then placed
		''' into the registry for later retrieval.
		''' 
		''' <p> The exact set of locations searched depends on the
		''' implementation of the Java runtime environment.
		''' </summary>
		''' <seealso cref= ClassLoader#getResources </seealso>
		Public Shared Sub scanForPlugins()
			theRegistry.registerApplicationClasspathSpis()
		End Sub

		' ImageInputStreams

		''' <summary>
		''' A class to hold information about caching.  Each
		''' <code>ThreadGroup</code> will have its own copy
		''' via the <code>AppContext</code> mechanism.
		''' </summary>
		Friend Class CacheInfo
			Friend useCache As Boolean = True
			Friend cacheDirectory As java.io.File = Nothing
			Friend hasPermission As Boolean? = Nothing

			Public Sub New()
			End Sub

			Public Overridable Property useCache As Boolean
				Get
					Return useCache
				End Get
				Set(ByVal useCache As Boolean)
					Me.useCache = useCache
				End Set
			End Property


			Public Overridable Property cacheDirectory As java.io.File
				Get
					Return cacheDirectory
				End Get
				Set(ByVal cacheDirectory As java.io.File)
					Me.cacheDirectory = cacheDirectory
				End Set
			End Property


			Public Overridable Property hasPermission As Boolean?
				Get
					Return hasPermission
				End Get
				Set(ByVal hasPermission As Boolean?)
					Me.hasPermission = hasPermission
				End Set
			End Property

		End Class

		''' <summary>
		''' Returns the <code>CacheInfo</code> object associated with this
		''' <code>ThreadGroup</code>.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Property Shared cacheInfo As CacheInfo
			Get
				Dim context As sun.awt.AppContext = sun.awt.AppContext.appContext
				Dim info As CacheInfo = CType(context.get(GetType(CacheInfo)), CacheInfo)
				If info Is Nothing Then
					info = New CacheInfo
					context.put(GetType(CacheInfo), info)
				End If
				Return info
			End Get
		End Property

		''' <summary>
		''' Returns the default temporary (cache) directory as defined by the
		''' java.io.tmpdir system property.
		''' </summary>
		Private Property Shared tempDir As String
			Get
				Dim a As New sun.security.action.GetPropertyAction("java.io.tmpdir")
				Return CStr(java.security.AccessController.doPrivileged(a))
			End Get
		End Property

		''' <summary>
		''' Determines whether the caller has write access to the cache
		''' directory, stores the result in the <code>CacheInfo</code> object,
		''' and returns the decision.  This method helps to prevent mysterious
		''' SecurityExceptions to be thrown when this convenience class is used
		''' in an applet, for example.
		''' </summary>
		Private Shared Function hasCachePermission() As Boolean
			Dim hasPermission As Boolean? = cacheInfo.hasPermission

			If hasPermission IsNot Nothing Then
				Return hasPermission
			Else
				Try
					Dim security As SecurityManager = System.securityManager
					If security IsNot Nothing Then
						Dim cachedir As File = cacheDirectory
						Dim cachepath As String

						If cachedir IsNot Nothing Then
							cachepath = cachedir.path
						Else
							cachepath = tempDir

							If cachepath Is Nothing OrElse cachepath.Length = 0 Then
								cacheInfo.hasPermission = Boolean.FALSE
								Return False
							End If
						End If

						' we have to check whether we can read, write,
						' and delete cache files.
						' So, compose cache file path and check it.
						Dim filepath As String = cachepath
						If Not filepath.EndsWith(File.separator) Then filepath += File.separator
						filepath &= "*"

						security.checkPermission(New java.io.FilePermission(filepath, "read, write, delete"))
					End If
				Catch e As SecurityException
					cacheInfo.hasPermission = Boolean.FALSE
					Return False
				End Try

				cacheInfo.hasPermission = Boolean.TRUE
				Return True
			End If
		End Function

		''' <summary>
		''' Sets a flag indicating whether a disk-based cache file should
		''' be used when creating <code>ImageInputStream</code>s and
		''' <code>ImageOutputStream</code>s.
		''' 
		''' <p> When reading from a standard <code>InputStream</code>, it
		''' may be necessary to save previously read information in a cache
		''' since the underlying stream does not allow data to be re-read.
		''' Similarly, when writing to a standard
		''' <code>OutputStream</code>, a cache may be used to allow a
		''' previously written value to be changed before flushing it to
		''' the final destination.
		''' 
		''' <p> The cache may reside in main memory or on disk.  Setting
		''' this flag to <code>false</code> disallows the use of disk for
		''' future streams, which may be advantageous when working with
		''' small images, as the overhead of creating and destroying files
		''' is removed.
		''' 
		''' <p> On startup, the value is set to <code>true</code>.
		''' </summary>
		''' <param name="useCache"> a <code>boolean</code> indicating whether a
		''' cache file should be used, in cases where it is optional.
		''' </param>
		''' <seealso cref= #getUseCache </seealso>
		Public Shared Property useCache As Boolean
			Set(ByVal useCache As Boolean)
				cacheInfo.useCache = useCache
			End Set
			Get
				Return cacheInfo.useCache
			End Get
		End Property


		''' <summary>
		''' Sets the directory where cache files are to be created.  A
		''' value of <code>null</code> indicates that the system-dependent
		''' default temporary-file directory is to be used.  If
		''' <code>getUseCache</code> returns false, this value is ignored.
		''' </summary>
		''' <param name="cacheDirectory"> a <code>File</code> specifying a directory.
		''' </param>
		''' <seealso cref= File#createTempFile(String, String, File)
		''' </seealso>
		''' <exception cref="SecurityException"> if the security manager denies
		''' access to the directory. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>cacheDir</code> is
		''' non-<code>null</code> but is not a directory.
		''' </exception>
		''' <seealso cref= #getCacheDirectory </seealso>
		Public Shared Property cacheDirectory As java.io.File
			Set(ByVal cacheDirectory As java.io.File)
				If (cacheDirectory IsNot Nothing) AndAlso Not(cacheDirectory.directory) Then Throw New System.ArgumentException("Not a directory!")
				cacheInfo.cacheDirectory = cacheDirectory
				cacheInfo.hasPermission = Nothing
			End Set
			Get
				Return cacheInfo.cacheDirectory
			End Get
		End Property


		''' <summary>
		''' Returns an <code>ImageInputStream</code> that will take its
		''' input from the given <code>Object</code>.  The set of
		''' <code>ImageInputStreamSpi</code>s registered with the
		''' <code>IIORegistry</code> class is queried and the first one
		''' that is able to take input from the supplied object is used to
		''' create the returned <code>ImageInputStream</code>.  If no
		''' suitable <code>ImageInputStreamSpi</code> exists,
		''' <code>null</code> is returned.
		''' 
		''' <p> The current cache settings from <code>getUseCache</code>and
		''' <code>getCacheDirectory</code> will be used to control caching.
		''' </summary>
		''' <param name="input"> an <code>Object</code> to be used as an input
		''' source, such as a <code>File</code>, readable
		''' <code>RandomAccessFile</code>, or <code>InputStream</code>.
		''' </param>
		''' <returns> an <code>ImageInputStream</code>, or <code>null</code>.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>input</code>
		''' is <code>null</code>. </exception>
		''' <exception cref="IOException"> if a cache file is needed but cannot be
		''' created.
		''' </exception>
		''' <seealso cref= javax.imageio.spi.ImageInputStreamSpi </seealso>
		Public Shared Function createImageInputStream(ByVal input As Object) As javax.imageio.stream.ImageInputStream
			If input Is Nothing Then Throw New System.ArgumentException("input == null!")

			Dim iter As IEnumerator
			' Ensure category is present
			Try
				iter = theRegistry.getServiceProviders(GetType(javax.imageio.spi.ImageInputStreamSpi), True)
			Catch e As System.ArgumentException
				Return Nothing
			End Try

			Dim ___usecache As Boolean = useCache AndAlso hasCachePermission()

			Do While iter.hasNext()
				Dim spi As javax.imageio.spi.ImageInputStreamSpi = CType(iter.next(), javax.imageio.spi.ImageInputStreamSpi)
				If spi.inputClass.IsInstanceOfType(input) Then
					Try
						Return spi.createInputStreamInstance(input, ___usecache, cacheDirectory)
					Catch e As java.io.IOException
						Throw New IIOException("Can't create cache file!", e)
					End Try
				End If
			Loop

			Return Nothing
		End Function

		' ImageOutputStreams

		''' <summary>
		''' Returns an <code>ImageOutputStream</code> that will send its
		''' output to the given <code>Object</code>.  The set of
		''' <code>ImageOutputStreamSpi</code>s registered with the
		''' <code>IIORegistry</code> class is queried and the first one
		''' that is able to send output from the supplied object is used to
		''' create the returned <code>ImageOutputStream</code>.  If no
		''' suitable <code>ImageOutputStreamSpi</code> exists,
		''' <code>null</code> is returned.
		''' 
		''' <p> The current cache settings from <code>getUseCache</code>and
		''' <code>getCacheDirectory</code> will be used to control caching.
		''' </summary>
		''' <param name="output"> an <code>Object</code> to be used as an output
		''' destination, such as a <code>File</code>, writable
		''' <code>RandomAccessFile</code>, or <code>OutputStream</code>.
		''' </param>
		''' <returns> an <code>ImageOutputStream</code>, or
		''' <code>null</code>.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>output</code> is
		''' <code>null</code>. </exception>
		''' <exception cref="IOException"> if a cache file is needed but cannot be
		''' created.
		''' </exception>
		''' <seealso cref= javax.imageio.spi.ImageOutputStreamSpi </seealso>
		Public Shared Function createImageOutputStream(ByVal output As Object) As javax.imageio.stream.ImageOutputStream
			If output Is Nothing Then Throw New System.ArgumentException("output == null!")

			Dim iter As IEnumerator
			' Ensure category is present
			Try
				iter = theRegistry.getServiceProviders(GetType(javax.imageio.spi.ImageOutputStreamSpi), True)
			Catch e As System.ArgumentException
				Return Nothing
			End Try

			Dim ___usecache As Boolean = useCache AndAlso hasCachePermission()

			Do While iter.hasNext()
				Dim spi As javax.imageio.spi.ImageOutputStreamSpi = CType(iter.next(), javax.imageio.spi.ImageOutputStreamSpi)
				If spi.outputClass.IsInstanceOfType(output) Then
					Try
						Return spi.createOutputStreamInstance(output, ___usecache, cacheDirectory)
					Catch e As java.io.IOException
						Throw New IIOException("Can't create cache file!", e)
					End Try
				End If
			Loop

			Return Nothing
		End Function

		Private Enum SpiInfo
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			FORMAT_NAMES
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'				String[] info(javax.imageio.spi.ImageReaderWriterSpi spi)
	'			{
	'				Return spi.getFormatNames();
	'			}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'			},
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			MIME_TYPES
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'				String[] info(javax.imageio.spi.ImageReaderWriterSpi spi)
	'			{
	'				Return spi.getMIMETypes();
	'			}
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'			},
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			FILE_SUFFIXES
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'				String[] info(javax.imageio.spi.ImageReaderWriterSpi spi)
	'			{
	'				Return spi.getFileSuffixes();
	'			}

			abstract = javax.imageio.spi.ImageReaderWriterSpi spi

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		private static (Of S As javax.imageio.spi.ImageReaderWriterSpi) String[] getReaderWriterInfo(Class spiClass, SpiInfo spiInfo)
	'	{
	'		' Ensure category is present
	'		Iterator<S> iter;
	'		try
	'		{
	'			iter = theRegistry.getServiceProviders(spiClass, True);
	'		}
	'		catch (IllegalArgumentException e)
	'		{
	'			Return New String[0];
	'		}
	'
	'		HashSet<String> s = New HashSet<String>();
	'		while (iter.hasNext())
	'		{
	'			ImageReaderWriterSpi spi = iter.next();
	'			Collections.addAll(s, spiInfo.info(spi));
	'		}
	'
	'		Return s.toArray(New String[s.size()]);
	'	}

		' Readers

		''' <summary>
		''' Returns an array of <code>String</code>s listing all of the
		''' informal format names understood by the current set of registered
		''' readers.
		''' </summary>
		''' <returns> an array of <code>String</code>s. </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public static String[] getReaderFormatNames()
	'	{
	'		Return getReaderWriterInfo(ImageReaderSpi.class, SpiInfo.FORMAT_NAMES);
	'	}

		''' <summary>
		''' Returns an array of <code>String</code>s listing all of the
		''' MIME types understood by the current set of registered
		''' readers.
		''' </summary>
		''' <returns> an array of <code>String</code>s. </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public static String[] getReaderMIMETypes()
	'	{
	'		Return getReaderWriterInfo(ImageReaderSpi.class, SpiInfo.MIME_TYPES);
	'	}

		''' <summary>
		''' Returns an array of <code>String</code>s listing all of the
		''' file suffixes associated with the formats understood
		''' by the current set of registered readers.
		''' </summary>
		''' <returns> an array of <code>String</code>s.
		''' @since 1.6 </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public static String[] getReaderFileSuffixes()
	'	{
	'		Return getReaderWriterInfo(ImageReaderSpi.class, SpiInfo.FILE_SUFFIXES);
	'	}

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		static class ImageReaderIterator implements java.util.Iterator<ImageReader>
			' Contains ImageReaderSpis
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'			public java.util.Iterator iter;

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public ImageReaderIterator(java.util.Iterator iter)
	'		{
	'			Me.iter = iter;
	'		}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public boolean hasNext()
	'		{
	'			Return iter.hasNext();
	'		}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public ImageReader next()
	'		{
	'			ImageReaderSpi spi = Nothing;
	'			try
	'			{
	'				spi = (ImageReaderSpi)iter.next();
	'				Return spi.createReaderInstance();
	'			}
	'			catch (IOException e)
	'			{
	'				' Deregister the spi in this case, but only as
	'				' an ImageReaderSpi
	'				theRegistry.deregisterServiceProvider(spi, ImageReaderSpi.class);
	'			}
	'			Return Nothing;
	'		}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public void remove()
	'		{
	'			throw New UnsupportedOperationException();
	'		}

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		static class CanDecodeInputFilter implements javax.imageio.spi.ServiceRegistry.Filter

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'			Object input;

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public CanDecodeInputFilter(Object input)
	'		{
	'			Me.input = input;
	'		}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public boolean filter(Object elt)
	'		{
	'			try
	'			{
	'				ImageReaderSpi spi = (ImageReaderSpi)elt;
	'				ImageInputStream stream = Nothing;
	'				if (input instanceof ImageInputStream)
	'				{
	'					stream = (ImageInputStream)input;
	'				}
	'
	'				' Perform mark/reset as a defensive measure
	'				' even though plug-ins are supposed to take
	'				' care of it.
	'				boolean canDecode = False;
	'				if (stream != Nothing)
	'				{
	'					stream.mark();
	'				}
	'				canDecode = spi.canDecodeInput(input);
	'				if (stream != Nothing)
	'				{
	'					stream.reset();
	'				}
	'
	'				Return canDecode;
	'			}
	'			catch (IOException e)
	'			{
	'				Return False;
	'			}
	'		}

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		static class CanEncodeImageAndFormatFilter implements javax.imageio.spi.ServiceRegistry.Filter

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'			ImageTypeSpecifier type;
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'			String formatName;

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public CanEncodeImageAndFormatFilter(ImageTypeSpecifier type, String formatName)
	'		{
	'			Me.type = type;
	'			Me.formatName = formatName;
	'		}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public boolean filter(Object elt)
	'		{
	'			ImageWriterSpi spi = (ImageWriterSpi)elt;
	'			Return Arrays.asList(spi.getFormatNames()).contains(formatName) && spi.canEncodeImage(type);
	'		}

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		static class ContainsFilter implements javax.imageio.spi.ServiceRegistry.Filter

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'			Method method;
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'			String name;

			' method returns an array of Strings
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public ContainsFilter(Method method, String name)
	'		{
	'			Me.method = method;
	'			Me.name = name;
	'		}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public boolean filter(Object elt)
	'		{
	'			try
	'			{
	'				Return contains((String[])method.invoke(elt), name);
	'			}
	'			catch (Exception e)
	'			{
	'				Return False;
	'			}
	'		}

		''' <summary>
		''' Returns an <code>Iterator</code> containing all currently
		''' registered <code>ImageReader</code>s that claim to be able to
		''' decode the supplied <code>Object</code>, typically an
		''' <code>ImageInputStream</code>.
		''' 
		''' <p> The stream position is left at its prior position upon
		''' exit from this method.
		''' </summary>
		''' <param name="input"> an <code>ImageInputStream</code> or other
		''' <code>Object</code> containing encoded image data.
		''' </param>
		''' <returns> an <code>Iterator</code> containing <code>ImageReader</code>s.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>input</code> is
		''' <code>null</code>.
		''' </exception>
		''' <seealso cref= javax.imageio.spi.ImageReaderSpi#canDecodeInput </seealso>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public static java.util.Iterator(Of ImageReader) getImageReaders(Object input)
	'	{
	'		if (input == Nothing)
	'		{
	'			throw New IllegalArgumentException("input == null!");
	'		}
	'		Iterator iter;
	'		' Ensure category is present
	'		try
	'		{
	'			iter = theRegistry.getServiceProviders(ImageReaderSpi.class, New CanDecodeInputFilter(input), True);
	'		}
	'		catch (IllegalArgumentException e)
	'		{
	'			Return Collections.emptyIterator();
	'		}
	'
	'		Return New ImageReaderIterator(iter);
	'	}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		private static Method readerFormatNamesMethod;
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		private static Method readerFileSuffixesMethod;
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		private static Method readerMIMETypesMethod;
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		private static Method writerFormatNamesMethod;
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		private static Method writerFileSuffixesMethod;
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		private static Method writerMIMETypesMethod;

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		static ImageIO()
	'	{
	'		try
	'		{
	'			readerFormatNamesMethod = ImageReaderSpi.class.getMethod("getFormatNames");
	'			readerFileSuffixesMethod = ImageReaderSpi.class.getMethod("getFileSuffixes");
	'			readerMIMETypesMethod = ImageReaderSpi.class.getMethod("getMIMETypes");
	'
	'			writerFormatNamesMethod = ImageWriterSpi.class.getMethod("getFormatNames");
	'			writerFileSuffixesMethod = ImageWriterSpi.class.getMethod("getFileSuffixes");
	'			writerMIMETypesMethod = ImageWriterSpi.class.getMethod("getMIMETypes");
	'		}
	'		catch (NoSuchMethodException e)
	'		{
	'			e.printStackTrace();
	'		}
	'	}

		''' <summary>
		''' Returns an <code>Iterator</code> containing all currently
		''' registered <code>ImageReader</code>s that claim to be able to
		''' decode the named format.
		''' </summary>
		''' <param name="formatName"> a <code>String</code> containing the informal
		''' name of a format (<i>e.g.</i>, "jpeg" or "tiff".
		''' </param>
		''' <returns> an <code>Iterator</code> containing
		''' <code>ImageReader</code>s.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>formatName</code>
		''' is <code>null</code>.
		''' </exception>
		''' <seealso cref= javax.imageio.spi.ImageReaderSpi#getFormatNames </seealso>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public static java.util.Iterator(Of ImageReader) getImageReadersByFormatName(String formatName)
	'	{
	'		if (formatName == Nothing)
	'		{
	'			throw New IllegalArgumentException("formatName == null!");
	'		}
	'		Iterator iter;
	'		' Ensure category is present
	'		try
	'		{
	'			iter = theRegistry.getServiceProviders(ImageReaderSpi.class, New ContainsFilter(readerFormatNamesMethod, formatName), True);
	'		}
	'		catch (IllegalArgumentException e)
	'		{
	'			Return Collections.emptyIterator();
	'		}
	'		Return New ImageReaderIterator(iter);
	'	}

		''' <summary>
		''' Returns an <code>Iterator</code> containing all currently
		''' registered <code>ImageReader</code>s that claim to be able to
		''' decode files with the given suffix.
		''' </summary>
		''' <param name="fileSuffix"> a <code>String</code> containing a file
		''' suffix (<i>e.g.</i>, "jpg" or "tiff").
		''' </param>
		''' <returns> an <code>Iterator</code> containing
		''' <code>ImageReader</code>s.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>fileSuffix</code>
		''' is <code>null</code>.
		''' </exception>
		''' <seealso cref= javax.imageio.spi.ImageReaderSpi#getFileSuffixes </seealso>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public static java.util.Iterator(Of ImageReader) getImageReadersBySuffix(String fileSuffix)
	'	{
	'		if (fileSuffix == Nothing)
	'		{
	'			throw New IllegalArgumentException("fileSuffix == null!");
	'		}
	'		' Ensure category is present
	'		Iterator iter;
	'		try
	'		{
	'			iter = theRegistry.getServiceProviders(ImageReaderSpi.class, New ContainsFilter(readerFileSuffixesMethod, fileSuffix), True);
	'		}
	'		catch (IllegalArgumentException e)
	'		{
	'			Return Collections.emptyIterator();
	'		}
	'		Return New ImageReaderIterator(iter);
	'	}

		''' <summary>
		''' Returns an <code>Iterator</code> containing all currently
		''' registered <code>ImageReader</code>s that claim to be able to
		''' decode files with the given MIME type.
		''' </summary>
		''' <param name="MIMEType"> a <code>String</code> containing a file
		''' suffix (<i>e.g.</i>, "image/jpeg" or "image/x-bmp").
		''' </param>
		''' <returns> an <code>Iterator</code> containing
		''' <code>ImageReader</code>s.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>MIMEType</code> is
		''' <code>null</code>.
		''' </exception>
		''' <seealso cref= javax.imageio.spi.ImageReaderSpi#getMIMETypes </seealso>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public static java.util.Iterator(Of ImageReader) getImageReadersByMIMEType(String MIMEType)
	'	{
	'		if (MIMEType == Nothing)
	'		{
	'			throw New IllegalArgumentException("MIMEType == null!");
	'		}
	'		' Ensure category is present
	'		Iterator iter;
	'		try
	'		{
	'			iter = theRegistry.getServiceProviders(ImageReaderSpi.class, New ContainsFilter(readerMIMETypesMethod, MIMEType), True);
	'		}
	'		catch (IllegalArgumentException e)
	'		{
	'			Return Collections.emptyIterator();
	'		}
	'		Return New ImageReaderIterator(iter);
	'	}

		' Writers

		''' <summary>
		''' Returns an array of <code>String</code>s listing all of the
		''' informal format names understood by the current set of registered
		''' writers.
		''' </summary>
		''' <returns> an array of <code>String</code>s. </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public static String[] getWriterFormatNames()
	'	{
	'		Return getReaderWriterInfo(ImageWriterSpi.class, SpiInfo.FORMAT_NAMES);
	'	}

		''' <summary>
		''' Returns an array of <code>String</code>s listing all of the
		''' MIME types understood by the current set of registered
		''' writers.
		''' </summary>
		''' <returns> an array of <code>String</code>s. </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public static String[] getWriterMIMETypes()
	'	{
	'		Return getReaderWriterInfo(ImageWriterSpi.class, SpiInfo.MIME_TYPES);
	'	}

		''' <summary>
		''' Returns an array of <code>String</code>s listing all of the
		''' file suffixes associated with the formats understood
		''' by the current set of registered writers.
		''' </summary>
		''' <returns> an array of <code>String</code>s.
		''' @since 1.6 </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public static String[] getWriterFileSuffixes()
	'	{
	'		Return getReaderWriterInfo(ImageWriterSpi.class, SpiInfo.FILE_SUFFIXES);
	'	}

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		static class ImageWriterIterator implements java.util.Iterator<ImageWriter>
			' Contains ImageWriterSpis
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'			public java.util.Iterator iter;

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public ImageWriterIterator(java.util.Iterator iter)
	'		{
	'			Me.iter = iter;
	'		}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public boolean hasNext()
	'		{
	'			Return iter.hasNext();
	'		}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public ImageWriter next()
	'		{
	'			ImageWriterSpi spi = Nothing;
	'			try
	'			{
	'				spi = (ImageWriterSpi)iter.next();
	'				Return spi.createWriterInstance();
	'			}
	'			catch (IOException e)
	'			{
	'				' Deregister the spi in this case, but only as a writerSpi
	'				theRegistry.deregisterServiceProvider(spi, ImageWriterSpi.class);
	'			}
	'			Return Nothing;
	'		}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public void remove()
	'		{
	'			throw New UnsupportedOperationException();
	'		}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		private static boolean contains(String[] names, String name)
	'	{
	'		for (int i = 0; i < names.length; i += 1)
	'		{
	'			if (name.equalsIgnoreCase(names[i]))
	'			{
	'				Return True;
	'			}
	'		}
	'
	'		Return False;
	'	}

		''' <summary>
		''' Returns an <code>Iterator</code> containing all currently
		''' registered <code>ImageWriter</code>s that claim to be able to
		''' encode the named format.
		''' </summary>
		''' <param name="formatName"> a <code>String</code> containing the informal
		''' name of a format (<i>e.g.</i>, "jpeg" or "tiff".
		''' </param>
		''' <returns> an <code>Iterator</code> containing
		''' <code>ImageWriter</code>s.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>formatName</code> is
		''' <code>null</code>.
		''' </exception>
		''' <seealso cref= javax.imageio.spi.ImageWriterSpi#getFormatNames </seealso>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public static java.util.Iterator(Of ImageWriter) getImageWritersByFormatName(String formatName)
	'	{
	'		if (formatName == Nothing)
	'		{
	'			throw New IllegalArgumentException("formatName == null!");
	'		}
	'		Iterator iter;
	'		' Ensure category is present
	'		try
	'		{
	'			iter = theRegistry.getServiceProviders(ImageWriterSpi.class, New ContainsFilter(writerFormatNamesMethod, formatName), True);
	'		}
	'		catch (IllegalArgumentException e)
	'		{
	'			Return Collections.emptyIterator();
	'		}
	'		Return New ImageWriterIterator(iter);
	'	}

		''' <summary>
		''' Returns an <code>Iterator</code> containing all currently
		''' registered <code>ImageWriter</code>s that claim to be able to
		''' encode files with the given suffix.
		''' </summary>
		''' <param name="fileSuffix"> a <code>String</code> containing a file
		''' suffix (<i>e.g.</i>, "jpg" or "tiff").
		''' </param>
		''' <returns> an <code>Iterator</code> containing <code>ImageWriter</code>s.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>fileSuffix</code> is
		''' <code>null</code>.
		''' </exception>
		''' <seealso cref= javax.imageio.spi.ImageWriterSpi#getFileSuffixes </seealso>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public static java.util.Iterator(Of ImageWriter) getImageWritersBySuffix(String fileSuffix)
	'	{
	'		if (fileSuffix == Nothing)
	'		{
	'			throw New IllegalArgumentException("fileSuffix == null!");
	'		}
	'		Iterator iter;
	'		' Ensure category is present
	'		try
	'		{
	'			iter = theRegistry.getServiceProviders(ImageWriterSpi.class, New ContainsFilter(writerFileSuffixesMethod, fileSuffix), True);
	'		}
	'		catch (IllegalArgumentException e)
	'		{
	'			Return Collections.emptyIterator();
	'		}
	'		Return New ImageWriterIterator(iter);
	'	}

		''' <summary>
		''' Returns an <code>Iterator</code> containing all currently
		''' registered <code>ImageWriter</code>s that claim to be able to
		''' encode files with the given MIME type.
		''' </summary>
		''' <param name="MIMEType"> a <code>String</code> containing a file
		''' suffix (<i>e.g.</i>, "image/jpeg" or "image/x-bmp").
		''' </param>
		''' <returns> an <code>Iterator</code> containing <code>ImageWriter</code>s.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>MIMEType</code> is
		''' <code>null</code>.
		''' </exception>
		''' <seealso cref= javax.imageio.spi.ImageWriterSpi#getMIMETypes </seealso>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public static java.util.Iterator(Of ImageWriter) getImageWritersByMIMEType(String MIMEType)
	'	{
	'		if (MIMEType == Nothing)
	'		{
	'			throw New IllegalArgumentException("MIMEType == null!");
	'		}
	'		Iterator iter;
	'		' Ensure category is present
	'		try
	'		{
	'			iter = theRegistry.getServiceProviders(ImageWriterSpi.class, New ContainsFilter(writerMIMETypesMethod, MIMEType), True);
	'		}
	'		catch (IllegalArgumentException e)
	'		{
	'			Return Collections.emptyIterator();
	'		}
	'		Return New ImageWriterIterator(iter);
	'	}

		''' <summary>
		''' Returns an <code>ImageWriter</code>corresponding to the given
		''' <code>ImageReader</code>, if there is one, or <code>null</code>
		''' if the plug-in for this <code>ImageReader</code> does not
		''' specify a corresponding <code>ImageWriter</code>, or if the
		''' given <code>ImageReader</code> is not registered.  This
		''' mechanism may be used to obtain an <code>ImageWriter</code>
		''' that will understand the internal structure of non-pixel
		''' metadata (as encoded by <code>IIOMetadata</code> objects)
		''' generated by the <code>ImageReader</code>.  By obtaining this
		''' data from the <code>ImageReader</code> and passing it on to the
		''' <code>ImageWriter</code> obtained with this method, a client
		''' program can read an image, modify it in some way, and write it
		''' back out preserving all metadata, without having to understand
		''' anything about the structure of the metadata, or even about
		''' the image format.  Note that this method returns the
		''' "preferred" writer, which is the first in the list returned by
		''' <code>javax.imageio.spi.ImageReaderSpi.getImageWriterSpiNames()</code>.
		''' </summary>
		''' <param name="reader"> an instance of a registered <code>ImageReader</code>.
		''' </param>
		''' <returns> an <code>ImageWriter</code>, or null.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>reader</code> is
		''' <code>null</code>.
		''' </exception>
		''' <seealso cref= #getImageReader(ImageWriter) </seealso>
		''' <seealso cref= javax.imageio.spi.ImageReaderSpi#getImageWriterSpiNames() </seealso>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public static ImageWriter getImageWriter(ImageReader reader)
	'	{
	'		if (reader == Nothing)
	'		{
	'			throw New IllegalArgumentException("reader == null!");
	'		}
	'
	'		ImageReaderSpi readerSpi = reader.getOriginatingProvider();
	'		if (readerSpi == Nothing)
	'		{
	'			Iterator readerSpiIter;
	'			' Ensure category is present
	'			try
	'			{
	'				readerSpiIter = theRegistry.getServiceProviders(ImageReaderSpi.class, False);
	'			}
	'			catch (IllegalArgumentException e)
	'			{
	'				Return Nothing;
	'			}
	'
	'			while (readerSpiIter.hasNext())
	'			{
	'				ImageReaderSpi temp = (ImageReaderSpi) readerSpiIter.next();
	'				if (temp.isOwnReader(reader))
	'				{
	'					readerSpi = temp;
	'					break;
	'				}
	'			}
	'			if (readerSpi == Nothing)
	'			{
	'				Return Nothing;
	'			}
	'		}
	'
	'		String[] writerNames = readerSpi.getImageWriterSpiNames();
	'		if (writerNames == Nothing)
	'		{
	'			Return Nothing;
	'		}
	'
	'		Class writerSpiClass = Nothing;
	'		try
	'		{
	'			writerSpiClass = Class.forName(writerNames[0], True, ClassLoader.getSystemClassLoader());
	'		}
	'		catch (ClassNotFoundException e)
	'		{
	'			Return Nothing;
	'		}
	'
	'		ImageWriterSpi writerSpi = (ImageWriterSpi) theRegistry.getServiceProviderByClass(writerSpiClass);
	'		if (writerSpi == Nothing)
	'		{
	'			Return Nothing;
	'		}
	'
	'		try
	'		{
	'			Return writerSpi.createWriterInstance();
	'		}
	'		catch (IOException e)
	'		{
	'			' Deregister the spi in this case, but only as a writerSpi
	'			theRegistry.deregisterServiceProvider(writerSpi, ImageWriterSpi.class);
	'			Return Nothing;
	'		}
	'	}

		''' <summary>
		''' Returns an <code>ImageReader</code>corresponding to the given
		''' <code>ImageWriter</code>, if there is one, or <code>null</code>
		''' if the plug-in for this <code>ImageWriter</code> does not
		''' specify a corresponding <code>ImageReader</code>, or if the
		''' given <code>ImageWriter</code> is not registered.  This method
		''' is provided principally for symmetry with
		''' <code>getImageWriter(ImageReader)</code>.  Note that this
		''' method returns the "preferred" reader, which is the first in
		''' the list returned by
		''' javax.imageio.spi.ImageWriterSpi.<code>getImageReaderSpiNames()</code>.
		''' </summary>
		''' <param name="writer"> an instance of a registered <code>ImageWriter</code>.
		''' </param>
		''' <returns> an <code>ImageReader</code>, or null.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>writer</code> is
		''' <code>null</code>.
		''' </exception>
		''' <seealso cref= #getImageWriter(ImageReader) </seealso>
		''' <seealso cref= javax.imageio.spi.ImageWriterSpi#getImageReaderSpiNames() </seealso>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public static ImageReader getImageReader(ImageWriter writer)
	'	{
	'		if (writer == Nothing)
	'		{
	'			throw New IllegalArgumentException("writer == null!");
	'		}
	'
	'		ImageWriterSpi writerSpi = writer.getOriginatingProvider();
	'		if (writerSpi == Nothing)
	'		{
	'			Iterator writerSpiIter;
	'			' Ensure category is present
	'			try
	'			{
	'				writerSpiIter = theRegistry.getServiceProviders(ImageWriterSpi.class, False);
	'			}
	'			catch (IllegalArgumentException e)
	'			{
	'				Return Nothing;
	'			}
	'
	'			while (writerSpiIter.hasNext())
	'			{
	'				ImageWriterSpi temp = (ImageWriterSpi) writerSpiIter.next();
	'				if (temp.isOwnWriter(writer))
	'				{
	'					writerSpi = temp;
	'					break;
	'				}
	'			}
	'			if (writerSpi == Nothing)
	'			{
	'				Return Nothing;
	'			}
	'		}
	'
	'		String[] readerNames = writerSpi.getImageReaderSpiNames();
	'		if (readerNames == Nothing)
	'		{
	'			Return Nothing;
	'		}
	'
	'		Class readerSpiClass = Nothing;
	'		try
	'		{
	'			readerSpiClass = Class.forName(readerNames[0], True, ClassLoader.getSystemClassLoader());
	'		}
	'		catch (ClassNotFoundException e)
	'		{
	'			Return Nothing;
	'		}
	'
	'		ImageReaderSpi readerSpi = (ImageReaderSpi) theRegistry.getServiceProviderByClass(readerSpiClass);
	'		if (readerSpi == Nothing)
	'		{
	'			Return Nothing;
	'		}
	'
	'		try
	'		{
	'			Return readerSpi.createReaderInstance();
	'		}
	'		catch (IOException e)
	'		{
	'			' Deregister the spi in this case, but only as a readerSpi
	'			theRegistry.deregisterServiceProvider(readerSpi, ImageReaderSpi.class);
	'			Return Nothing;
	'		}
	'	}

		''' <summary>
		''' Returns an <code>Iterator</code> containing all currently
		''' registered <code>ImageWriter</code>s that claim to be able to
		''' encode images of the given layout (specified using an
		''' <code>ImageTypeSpecifier</code>) in the given format.
		''' </summary>
		''' <param name="type"> an <code>ImageTypeSpecifier</code> indicating the
		''' layout of the image to be written. </param>
		''' <param name="formatName"> the informal name of the <code>format</code>.
		''' </param>
		''' <returns> an <code>Iterator</code> containing <code>ImageWriter</code>s.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if any parameter is
		''' <code>null</code>.
		''' </exception>
		''' <seealso cref= javax.imageio.spi.ImageWriterSpi#canEncodeImage(ImageTypeSpecifier) </seealso>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public static java.util.Iterator(Of ImageWriter) getImageWriters(ImageTypeSpecifier type, String formatName)
	'	{
	'		if (type == Nothing)
	'		{
	'			throw New IllegalArgumentException("type == null!");
	'		}
	'		if (formatName == Nothing)
	'		{
	'			throw New IllegalArgumentException("formatName == null!");
	'		}
	'
	'		Iterator iter;
	'		' Ensure category is present
	'		try
	'		{
	'			iter = theRegistry.getServiceProviders(ImageWriterSpi.class, New CanEncodeImageAndFormatFilter(type, formatName), True);
	'		}
	'		catch (IllegalArgumentException e)
	'		{
	'			Return Collections.emptyIterator();
	'		}
	'
	'		Return New ImageWriterIterator(iter);
	'	}

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		static class ImageTranscoderIterator implements java.util.Iterator<ImageTranscoder>
			' Contains ImageTranscoderSpis
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'			public java.util.Iterator iter;

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public ImageTranscoderIterator(java.util.Iterator iter)
	'		{
	'			Me.iter = iter;
	'		}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public boolean hasNext()
	'		{
	'			Return iter.hasNext();
	'		}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public ImageTranscoder next()
	'		{
	'			ImageTranscoderSpi spi = Nothing;
	'			spi = (ImageTranscoderSpi)iter.next();
	'			Return spi.createTranscoderInstance();
	'		}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public void remove()
	'		{
	'			throw New UnsupportedOperationException();
	'		}

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		static class TranscoderFilter implements javax.imageio.spi.ServiceRegistry.Filter

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'			String readerSpiName;
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'			String writerSpiName;

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public TranscoderFilter(javax.imageio.spi.ImageReaderSpi readerSpi, javax.imageio.spi.ImageWriterSpi writerSpi)
	'		{
	'			Me.readerSpiName = readerSpi.getClass().getName();
	'			Me.writerSpiName = writerSpi.getClass().getName();
	'		}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public boolean filter(Object elt)
	'		{
	'			ImageTranscoderSpi spi = (ImageTranscoderSpi)elt;
	'			String readerName = spi.getReaderServiceProviderName();
	'			String writerName = spi.getWriterServiceProviderName();
	'			Return (readerName.equals(readerSpiName) && writerName.equals(writerSpiName));
	'		}

		''' <summary>
		''' Returns an <code>Iterator</code> containing all currently
		''' registered <code>ImageTranscoder</code>s that claim to be
		''' able to transcode between the metadata of the given
		''' <code>ImageReader</code> and <code>ImageWriter</code>.
		''' </summary>
		''' <param name="reader"> an <code>ImageReader</code>. </param>
		''' <param name="writer"> an <code>ImageWriter</code>.
		''' </param>
		''' <returns> an <code>Iterator</code> containing
		''' <code>ImageTranscoder</code>s.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>reader</code> or
		''' <code>writer</code> is <code>null</code>. </exception>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public static java.util.Iterator(Of ImageTranscoder) getImageTranscoders(ImageReader reader, ImageWriter writer)
	'	{
	'		if (reader == Nothing)
	'		{
	'			throw New IllegalArgumentException("reader == null!");
	'		}
	'		if (writer == Nothing)
	'		{
	'			throw New IllegalArgumentException("writer == null!");
	'		}
	'		ImageReaderSpi readerSpi = reader.getOriginatingProvider();
	'		ImageWriterSpi writerSpi = writer.getOriginatingProvider();
	'		ServiceRegistry.Filter filter = New TranscoderFilter(readerSpi, writerSpi);
	'
	'		Iterator iter;
	'		' Ensure category is present
	'		try
	'		{
	'			iter = theRegistry.getServiceProviders(ImageTranscoderSpi.class, filter, True);
	'		}
	'		catch (IllegalArgumentException e)
	'		{
	'			Return Collections.emptyIterator();
	'		}
	'		Return New ImageTranscoderIterator(iter);
	'	}

		' All-in-one methods

		''' <summary>
		''' Returns a <code>BufferedImage</code> as the result of decoding
		''' a supplied <code>File</code> with an <code>ImageReader</code>
		''' chosen automatically from among those currently registered.
		''' The <code>File</code> is wrapped in an
		''' <code>ImageInputStream</code>.  If no registered
		''' <code>ImageReader</code> claims to be able to read the
		''' resulting stream, <code>null</code> is returned.
		''' 
		''' <p> The current cache settings from <code>getUseCache</code>and
		''' <code>getCacheDirectory</code> will be used to control caching in the
		''' <code>ImageInputStream</code> that is created.
		''' 
		''' <p> Note that there is no <code>read</code> method that takes a
		''' filename as a <code>String</code>; use this method instead after
		''' creating a <code>File</code> from the filename.
		''' 
		''' <p> This method does not attempt to locate
		''' <code>ImageReader</code>s that can read directly from a
		''' <code>File</code>; that may be accomplished using
		''' <code>IIORegistry</code> and <code>ImageReaderSpi</code>.
		''' </summary>
		''' <param name="input"> a <code>File</code> to read from.
		''' </param>
		''' <returns> a <code>BufferedImage</code> containing the decoded
		''' contents of the input, or <code>null</code>.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>input</code> is
		''' <code>null</code>. </exception>
		''' <exception cref="IOException"> if an error occurs during reading. </exception>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public static java.awt.image.BufferedImage read(java.io.File input) throws java.io.IOException
	'	{
	'		if (input == Nothing)
	'		{
	'			throw New IllegalArgumentException("input == null!");
	'		}
	'		if (!input.canRead())
	'		{
	'			throw New IIOException("Can't read input file!");
	'		}
	'
	'		ImageInputStream stream = createImageInputStream(input);
	'		if (stream == Nothing)
	'		{
	'			throw New IIOException("Can't create an ImageInputStream!");
	'		}
	'		BufferedImage bi = read(stream);
	'		if (bi == Nothing)
	'		{
	'			stream.close();
	'		}
	'		Return bi;
	'	}

		''' <summary>
		''' Returns a <code>BufferedImage</code> as the result of decoding
		''' a supplied <code>InputStream</code> with an <code>ImageReader</code>
		''' chosen automatically from among those currently registered.
		''' The <code>InputStream</code> is wrapped in an
		''' <code>ImageInputStream</code>.  If no registered
		''' <code>ImageReader</code> claims to be able to read the
		''' resulting stream, <code>null</code> is returned.
		''' 
		''' <p> The current cache settings from <code>getUseCache</code>and
		''' <code>getCacheDirectory</code> will be used to control caching in the
		''' <code>ImageInputStream</code> that is created.
		''' 
		''' <p> This method does not attempt to locate
		''' <code>ImageReader</code>s that can read directly from an
		''' <code>InputStream</code>; that may be accomplished using
		''' <code>IIORegistry</code> and <code>ImageReaderSpi</code>.
		''' 
		''' <p> This method <em>does not</em> close the provided
		''' <code>InputStream</code> after the read operation has completed;
		''' it is the responsibility of the caller to close the stream, if desired.
		''' </summary>
		''' <param name="input"> an <code>InputStream</code> to read from.
		''' </param>
		''' <returns> a <code>BufferedImage</code> containing the decoded
		''' contents of the input, or <code>null</code>.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>input</code> is
		''' <code>null</code>. </exception>
		''' <exception cref="IOException"> if an error occurs during reading. </exception>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public static java.awt.image.BufferedImage read(java.io.InputStream input) throws java.io.IOException
	'	{
	'		if (input == Nothing)
	'		{
	'			throw New IllegalArgumentException("input == null!");
	'		}
	'
	'		ImageInputStream stream = createImageInputStream(input);
	'		BufferedImage bi = read(stream);
	'		if (bi == Nothing)
	'		{
	'			stream.close();
	'		}
	'		Return bi;
	'	}

		''' <summary>
		''' Returns a <code>BufferedImage</code> as the result of decoding
		''' a supplied <code>URL</code> with an <code>ImageReader</code>
		''' chosen automatically from among those currently registered.  An
		''' <code>InputStream</code> is obtained from the <code>URL</code>,
		''' which is wrapped in an <code>ImageInputStream</code>.  If no
		''' registered <code>ImageReader</code> claims to be able to read
		''' the resulting stream, <code>null</code> is returned.
		''' 
		''' <p> The current cache settings from <code>getUseCache</code>and
		''' <code>getCacheDirectory</code> will be used to control caching in the
		''' <code>ImageInputStream</code> that is created.
		''' 
		''' <p> This method does not attempt to locate
		''' <code>ImageReader</code>s that can read directly from a
		''' <code>URL</code>; that may be accomplished using
		''' <code>IIORegistry</code> and <code>ImageReaderSpi</code>.
		''' </summary>
		''' <param name="input"> a <code>URL</code> to read from.
		''' </param>
		''' <returns> a <code>BufferedImage</code> containing the decoded
		''' contents of the input, or <code>null</code>.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>input</code> is
		''' <code>null</code>. </exception>
		''' <exception cref="IOException"> if an error occurs during reading. </exception>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public static java.awt.image.BufferedImage read(java.net.URL input) throws java.io.IOException
	'	{
	'		if (input == Nothing)
	'		{
	'			throw New IllegalArgumentException("input == null!");
	'		}
	'
	'		InputStream istream = Nothing;
	'		try
	'		{
	'			istream = input.openStream();
	'		}
	'		catch (IOException e)
	'		{
	'			throw New IIOException("Can't get input stream from URL!", e);
	'		}
	'		ImageInputStream stream = createImageInputStream(istream);
	'		BufferedImage bi;
	'		try
	'		{
	'			bi = read(stream);
	'			if (bi == Nothing)
	'			{
	'				stream.close();
	'			}
	'		}
	'		finally
	'		{
	'			istream.close();
	'		}
	'		Return bi;
	'	}

		''' <summary>
		''' Returns a <code>BufferedImage</code> as the result of decoding
		''' a supplied <code>ImageInputStream</code> with an
		''' <code>ImageReader</code> chosen automatically from among those
		''' currently registered.  If no registered
		''' <code>ImageReader</code> claims to be able to read the stream,
		''' <code>null</code> is returned.
		''' 
		''' <p> Unlike most other methods in this class, this method <em>does</em>
		''' close the provided <code>ImageInputStream</code> after the read
		''' operation has completed, unless <code>null</code> is returned,
		''' in which case this method <em>does not</em> close the stream.
		''' </summary>
		''' <param name="stream"> an <code>ImageInputStream</code> to read from.
		''' </param>
		''' <returns> a <code>BufferedImage</code> containing the decoded
		''' contents of the input, or <code>null</code>.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>stream</code> is
		''' <code>null</code>. </exception>
		''' <exception cref="IOException"> if an error occurs during reading. </exception>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public static java.awt.image.BufferedImage read(javax.imageio.stream.ImageInputStream stream) throws java.io.IOException
	'	{
	'		if (stream == Nothing)
	'		{
	'			throw New IllegalArgumentException("stream == null!");
	'		}
	'
	'		Iterator iter = getImageReaders(stream);
	'		if (!iter.hasNext())
	'		{
	'			Return Nothing;
	'		}
	'
	'		ImageReader reader = (ImageReader)iter.next();
	'		ImageReadParam param = reader.getDefaultReadParam();
	'		reader.setInput(stream, True, True);
	'		BufferedImage bi;
	'		try
	'		{
	'			bi = reader.read(0, param);
	'		}
	'		finally
	'		{
	'			reader.dispose();
	'			stream.close();
	'		}
	'		Return bi;
	'	}

		''' <summary>
		''' Writes an image using the an arbitrary <code>ImageWriter</code>
		''' that supports the given format to an
		''' <code>ImageOutputStream</code>.  The image is written to the
		''' <code>ImageOutputStream</code> starting at the current stream
		''' pointer, overwriting existing stream data from that point
		''' forward, if present.
		''' 
		''' <p> This method <em>does not</em> close the provided
		''' <code>ImageOutputStream</code> after the write operation has completed;
		''' it is the responsibility of the caller to close the stream, if desired.
		''' </summary>
		''' <param name="im"> a <code>RenderedImage</code> to be written. </param>
		''' <param name="formatName"> a <code>String</code> containing the informal
		''' name of the format. </param>
		''' <param name="output"> an <code>ImageOutputStream</code> to be written to.
		''' </param>
		''' <returns> <code>false</code> if no appropriate writer is found.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if any parameter is
		''' <code>null</code>. </exception>
		''' <exception cref="IOException"> if an error occurs during writing. </exception>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public static boolean write(java.awt.image.RenderedImage im, String formatName, javax.imageio.stream.ImageOutputStream output) throws java.io.IOException
	'	{
	'		if (im == Nothing)
	'		{
	'			throw New IllegalArgumentException("im == null!");
	'		}
	'		if (formatName == Nothing)
	'		{
	'			throw New IllegalArgumentException("formatName == null!");
	'		}
	'		if (output == Nothing)
	'		{
	'			throw New IllegalArgumentException("output == null!");
	'		}
	'
	'		Return doWrite(im, getWriter(im, formatName), output);
	'	}

		''' <summary>
		''' Writes an image using an arbitrary <code>ImageWriter</code>
		''' that supports the given format to a <code>File</code>.  If
		''' there is already a <code>File</code> present, its contents are
		''' discarded.
		''' </summary>
		''' <param name="im"> a <code>RenderedImage</code> to be written. </param>
		''' <param name="formatName"> a <code>String</code> containing the informal
		''' name of the format. </param>
		''' <param name="output"> a <code>File</code> to be written to.
		''' </param>
		''' <returns> <code>false</code> if no appropriate writer is found.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if any parameter is
		''' <code>null</code>. </exception>
		''' <exception cref="IOException"> if an error occurs during writing. </exception>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public static boolean write(java.awt.image.RenderedImage im, String formatName, java.io.File output) throws java.io.IOException
	'	{
	'		if (output == Nothing)
	'		{
	'			throw New IllegalArgumentException("output == null!");
	'		}
	'		ImageOutputStream stream = Nothing;
	'
	'		ImageWriter writer = getWriter(im, formatName);
	'		if (writer == Nothing)
	'		{
	''             Do not make changes in the file system if we have
	''             * no appropriate writer.
	''             
	'			Return False;
	'		}
	'
	'		try
	'		{
	'			output.delete();
	'			stream = createImageOutputStream(output);
	'		}
	'		catch (IOException e)
	'		{
	'			throw New IIOException("Can't create output stream!", e);
	'		}
	'
	'		try
	'		{
	'			Return doWrite(im, writer, stream);
	'		}
	'		finally
	'		{
	'			stream.close();
	'		}
	'	}

		''' <summary>
		''' Writes an image using an arbitrary <code>ImageWriter</code>
		''' that supports the given format to an <code>OutputStream</code>.
		''' 
		''' <p> This method <em>does not</em> close the provided
		''' <code>OutputStream</code> after the write operation has completed;
		''' it is the responsibility of the caller to close the stream, if desired.
		''' 
		''' <p> The current cache settings from <code>getUseCache</code>and
		''' <code>getCacheDirectory</code> will be used to control caching.
		''' </summary>
		''' <param name="im"> a <code>RenderedImage</code> to be written. </param>
		''' <param name="formatName"> a <code>String</code> containing the informal
		''' name of the format. </param>
		''' <param name="output"> an <code>OutputStream</code> to be written to.
		''' </param>
		''' <returns> <code>false</code> if no appropriate writer is found.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if any parameter is
		''' <code>null</code>. </exception>
		''' <exception cref="IOException"> if an error occurs during writing. </exception>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public static boolean write(java.awt.image.RenderedImage im, String formatName, java.io.OutputStream output) throws java.io.IOException
	'	{
	'		if (output == Nothing)
	'		{
	'			throw New IllegalArgumentException("output == null!");
	'		}
	'		ImageOutputStream stream = Nothing;
	'		try
	'		{
	'			stream = createImageOutputStream(output);
	'		}
	'		catch (IOException e)
	'		{
	'			throw New IIOException("Can't create output stream!", e);
	'		}
	'
	'		try
	'		{
	'			Return doWrite(im, getWriter(im, formatName), stream);
	'		}
	'		finally
	'		{
	'			stream.close();
	'		}
	'	}

		''' <summary>
		''' Returns <code>ImageWriter</code> instance according to given
		''' rendered image and image format or <code>null</code> if there
		''' is no appropriate writer.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		private static ImageWriter getWriter(java.awt.image.RenderedImage im, String formatName)
	'	{
	'		ImageTypeSpecifier type = ImageTypeSpecifier.createFromRenderedImage(im);
	'		Iterator<ImageWriter> iter = getImageWriters(type, formatName);
	'
	'		if (iter.hasNext())
	'		{
	'			Return iter.next();
	'		}
	'		else
	'		{
	'			Return Nothing;
	'		}
	'	}

		''' <summary>
		''' Writes image to output stream  using given image writer.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		private static boolean doWrite(java.awt.image.RenderedImage im, ImageWriter writer, javax.imageio.stream.ImageOutputStream output) throws java.io.IOException
	'	{
	'		if (writer == Nothing)
	'		{
	'			Return False;
	'		}
	'		writer.setOutput(output);
	'		try
	'		{
	'			writer.write(im);
	'		}
	'		finally
	'		{
	'			writer.dispose();
	'			output.flush();
	'		}
	'		Return True;
	'	}

End Namespace