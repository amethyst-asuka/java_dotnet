Imports System

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

Namespace javax.imageio.spi


	''' <summary>
	''' A superclass containing instance variables and methods common to
	''' <code>ImageReaderSpi</code> and <code>ImageWriterSpi</code>.
	''' </summary>
	''' <seealso cref= IIORegistry </seealso>
	''' <seealso cref= ImageReaderSpi </seealso>
	''' <seealso cref= ImageWriterSpi
	'''  </seealso>
	Public MustInherit Class ImageReaderWriterSpi
		Inherits IIOServiceProvider

		''' <summary>
		''' An array of strings to be returned from
		''' <code>getFormatNames</code>, initially <code>null</code>.
		''' Constructors should set this to a non-<code>null</code> value.
		''' </summary>
		Protected Friend names As String() = Nothing

		''' <summary>
		''' An array of strings to be returned from
		''' <code>getFileSuffixes</code>, initially <code>null</code>.
		''' </summary>
		Protected Friend suffixes As String() = Nothing

		''' <summary>
		''' An array of strings to be returned from
		''' <code>getMIMETypes</code>, initially <code>null</code>.
		''' </summary>
		Protected Friend MIMETypes As String() = Nothing

		''' <summary>
		''' A <code>String</code> containing the name of the associated
		''' plug-in class, initially <code>null</code>.
		''' </summary>
		Protected Friend pluginClassName As String = Nothing

		''' <summary>
		''' A boolean indicating whether this plug-in supports the
		''' standard metadata format for stream metadata, initially
		''' <code>false</code>.
		''' </summary>
		Protected Friend supportsStandardStreamMetadataFormat As Boolean = False

		''' <summary>
		''' A <code>String</code> containing the name of the native stream
		''' metadata format supported by this plug-in, initially
		''' <code>null</code>.
		''' </summary>
		Protected Friend nativeStreamMetadataFormatName As String = Nothing

		''' <summary>
		''' A <code>String</code> containing the class name of the native
		''' stream metadata format supported by this plug-in, initially
		''' <code>null</code>.
		''' </summary>
		Protected Friend nativeStreamMetadataFormatClassName As String = Nothing

		''' <summary>
		''' An array of <code>String</code>s containing the names of any
		''' additional stream metadata formats supported by this plug-in,
		''' initially <code>null</code>.
		''' </summary>
		Protected Friend extraStreamMetadataFormatNames As String() = Nothing

		''' <summary>
		''' An array of <code>String</code>s containing the class names of
		''' any additional stream metadata formats supported by this plug-in,
		''' initially <code>null</code>.
		''' </summary>
		Protected Friend extraStreamMetadataFormatClassNames As String() = Nothing

		''' <summary>
		''' A boolean indicating whether this plug-in supports the
		''' standard metadata format for image metadata, initially
		''' <code>false</code>.
		''' </summary>
		Protected Friend supportsStandardImageMetadataFormat As Boolean = False

		''' <summary>
		''' A <code>String</code> containing the name of the
		''' native stream metadata format supported by this plug-in,
		''' initially <code>null</code>.
		''' </summary>
		Protected Friend nativeImageMetadataFormatName As String = Nothing

		''' <summary>
		''' A <code>String</code> containing the class name of the
		''' native stream metadata format supported by this plug-in,
		''' initially <code>null</code>.
		''' </summary>
		Protected Friend nativeImageMetadataFormatClassName As String = Nothing

		''' <summary>
		''' An array of <code>String</code>s containing the names of any
		''' additional image metadata formats supported by this plug-in,
		''' initially <code>null</code>.
		''' </summary>
		Protected Friend extraImageMetadataFormatNames As String() = Nothing

		''' <summary>
		''' An array of <code>String</code>s containing the class names of
		''' any additional image metadata formats supported by this
		''' plug-in, initially <code>null</code>.
		''' </summary>
		Protected Friend extraImageMetadataFormatClassNames As String() = Nothing

		''' <summary>
		''' Constructs an <code>ImageReaderWriterSpi</code> with a given
		''' set of values.
		''' </summary>
		''' <param name="vendorName"> the vendor name, as a non-<code>null</code>
		''' <code>String</code>. </param>
		''' <param name="version"> a version identifier, as a non-<code>null</code>
		''' <code>String</code>. </param>
		''' <param name="names"> a non-<code>null</code> array of
		''' <code>String</code>s indicating the format names.  At least one
		''' entry must be present. </param>
		''' <param name="suffixes"> an array of <code>String</code>s indicating the
		''' common file suffixes.  If no suffixes are defined,
		''' <code>null</code> should be supplied.  An array of length 0
		''' will be normalized to <code>null</code>. </param>
		''' <param name="MIMETypes"> an array of <code>String</code>s indicating
		''' the format's MIME types.  If no MIME types are defined,
		''' <code>null</code> should be supplied.  An array of length 0
		''' will be normalized to <code>null</code>. </param>
		''' <param name="pluginClassName"> the fully-qualified name of the
		''' associated <code>ImageReader</code> or <code>ImageWriter</code>
		''' class, as a non-<code>null</code> <code>String</code>. </param>
		''' <param name="supportsStandardStreamMetadataFormat"> a
		''' <code>boolean</code> that indicates whether a stream metadata
		''' object can use trees described by the standard metadata format. </param>
		''' <param name="nativeStreamMetadataFormatName"> a
		''' <code>String</code>, or <code>null</code>, to be returned from
		''' <code>getNativeStreamMetadataFormatName</code>. </param>
		''' <param name="nativeStreamMetadataFormatClassName"> a
		''' <code>String</code>, or <code>null</code>, to be used to instantiate
		''' a metadata format object to be returned from
		''' <code>getNativeStreamMetadataFormat</code>. </param>
		''' <param name="extraStreamMetadataFormatNames"> an array of
		''' <code>String</code>s, or <code>null</code>, to be returned from
		''' <code>getExtraStreamMetadataFormatNames</code>.  An array of length
		''' 0 is normalized to <code>null</code>. </param>
		''' <param name="extraStreamMetadataFormatClassNames"> an array of
		''' <code>String</code>s, or <code>null</code>, to be used to instantiate
		''' a metadata format object to be returned from
		''' <code>getStreamMetadataFormat</code>.  An array of length
		''' 0 is normalized to <code>null</code>. </param>
		''' <param name="supportsStandardImageMetadataFormat"> a
		''' <code>boolean</code> that indicates whether an image metadata
		''' object can use trees described by the standard metadata format. </param>
		''' <param name="nativeImageMetadataFormatName"> a
		''' <code>String</code>, or <code>null</code>, to be returned from
		''' <code>getNativeImageMetadataFormatName</code>. </param>
		''' <param name="nativeImageMetadataFormatClassName"> a
		''' <code>String</code>, or <code>null</code>, to be used to instantiate
		''' a metadata format object to be returned from
		''' <code>getNativeImageMetadataFormat</code>. </param>
		''' <param name="extraImageMetadataFormatNames"> an array of
		''' <code>String</code>s to be returned from
		''' <code>getExtraImageMetadataFormatNames</code>.  An array of length 0
		''' is normalized to <code>null</code>. </param>
		''' <param name="extraImageMetadataFormatClassNames"> an array of
		''' <code>String</code>s, or <code>null</code>, to be used to instantiate
		''' a metadata format object to be returned from
		''' <code>getImageMetadataFormat</code>.  An array of length
		''' 0 is normalized to <code>null</code>.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>vendorName</code>
		''' is <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>version</code>
		''' is <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>names</code>
		''' is <code>null</code> or has length 0. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>pluginClassName</code>
		''' is <code>null</code>. </exception>
		Public Sub New(ByVal vendorName As String, ByVal version As String, ByVal names As String(), ByVal suffixes As String(), ByVal MIMETypes As String(), ByVal pluginClassName As String, ByVal supportsStandardStreamMetadataFormat As Boolean, ByVal nativeStreamMetadataFormatName As String, ByVal nativeStreamMetadataFormatClassName As String, ByVal extraStreamMetadataFormatNames As String(), ByVal extraStreamMetadataFormatClassNames As String(), ByVal supportsStandardImageMetadataFormat As Boolean, ByVal nativeImageMetadataFormatName As String, ByVal nativeImageMetadataFormatClassName As String, ByVal extraImageMetadataFormatNames As String(), ByVal extraImageMetadataFormatClassNames As String())
			MyBase.New(vendorName, version)
			If names Is Nothing Then Throw New System.ArgumentException("names == null!")
			If names.Length = 0 Then Throw New System.ArgumentException("names.length == 0!")
			If pluginClassName Is Nothing Then Throw New System.ArgumentException("pluginClassName == null!")

			Me.names = CType(names.clone(), String())
			' If length == 0, leave it null
			If suffixes IsNot Nothing AndAlso suffixes.Length > 0 Then Me.suffixes = CType(suffixes.clone(), String())
			' If length == 0, leave it null
			If MIMETypes IsNot Nothing AndAlso MIMETypes.Length > 0 Then Me.MIMETypes = CType(MIMETypes.clone(), String())
			Me.pluginClassName = pluginClassName

			Me.supportsStandardStreamMetadataFormat = supportsStandardStreamMetadataFormat
			Me.nativeStreamMetadataFormatName = nativeStreamMetadataFormatName
			Me.nativeStreamMetadataFormatClassName = nativeStreamMetadataFormatClassName
			' If length == 0, leave it null
			If extraStreamMetadataFormatNames IsNot Nothing AndAlso extraStreamMetadataFormatNames.Length > 0 Then Me.extraStreamMetadataFormatNames = CType(extraStreamMetadataFormatNames.clone(), String())
			' If length == 0, leave it null
			If extraStreamMetadataFormatClassNames IsNot Nothing AndAlso extraStreamMetadataFormatClassNames.Length > 0 Then Me.extraStreamMetadataFormatClassNames = CType(extraStreamMetadataFormatClassNames.clone(), String())
			Me.supportsStandardImageMetadataFormat = supportsStandardImageMetadataFormat
			Me.nativeImageMetadataFormatName = nativeImageMetadataFormatName
			Me.nativeImageMetadataFormatClassName = nativeImageMetadataFormatClassName
			' If length == 0, leave it null
			If extraImageMetadataFormatNames IsNot Nothing AndAlso extraImageMetadataFormatNames.Length > 0 Then Me.extraImageMetadataFormatNames = CType(extraImageMetadataFormatNames.clone(), String())
			' If length == 0, leave it null
			If extraImageMetadataFormatClassNames IsNot Nothing AndAlso extraImageMetadataFormatClassNames.Length > 0 Then Me.extraImageMetadataFormatClassNames = CType(extraImageMetadataFormatClassNames.clone(), String())
		End Sub

		''' <summary>
		''' Constructs a blank <code>ImageReaderWriterSpi</code>.  It is up
		''' to the subclass to initialize instance variables and/or
		''' override method implementations in order to provide working
		''' versions of all methods.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Returns an array of <code>String</code>s containing
		''' human-readable names for the formats that are generally usable
		''' by the <code>ImageReader</code> or <code>ImageWriter</code>
		''' implementation associated with this service provider.  For
		''' example, a single <code>ImageReader</code> might be able to
		''' process both PBM and PNM files.
		''' </summary>
		''' <returns> a non-<code>null</code> array of <code>String</code>s
		''' or length at least 1 containing informal format names
		''' associated with this reader or writer. </returns>
		Public Overridable Property formatNames As String()
			Get
				Return CType(names.clone(), String())
			End Get
		End Property

		''' <summary>
		''' Returns an array of <code>String</code>s containing a list of
		''' file suffixes associated with the formats that are generally
		''' usable by the <code>ImageReader</code> or
		''' <code>ImageWriter</code> implementation associated with this
		''' service provider.  For example, a single
		''' <code>ImageReader</code> might be able to process files with
		''' '.pbm' and '.pnm' suffixes, or both '.jpg' and '.jpeg'
		''' suffixes.  If there are no known file suffixes,
		''' <code>null</code> will be returned.
		''' 
		''' <p> Returning a particular suffix does not guarantee that files
		''' with that suffix can be processed; it merely indicates that it
		''' may be worthwhile attempting to decode or encode such files
		''' using this service provider.
		''' </summary>
		''' <returns> an array of <code>String</code>s or length at least 1
		''' containing common file suffixes associated with this reader or
		''' writer, or <code>null</code>. </returns>
		Public Overridable Property fileSuffixes As String()
			Get
				Return If(suffixes Is Nothing, Nothing, CType(suffixes.clone(), String()))
			End Get
		End Property

		''' <summary>
		''' Returns an array of <code>String</code>s containing a list of
		''' MIME types associated with the formats that are generally
		''' usable by the <code>ImageReader</code> or
		''' <code>ImageWriter</code> implementation associated with this
		''' service provider.
		''' 
		''' <p> Ideally, only a single MIME type would be required in order
		''' to describe a particular format.  However, for several reasons
		''' it is necessary to associate a list of types with each service
		''' provider.  First, many common image file formats do not have
		''' standard MIME types, so a list of commonly used unofficial
		''' names will be required, such as <code>image/x-pbm</code> and
		''' <code>image/x-portable-bitmap</code>.  Some file formats have
		''' official MIME types but may sometimes be referred to using
		''' their previous unofficial designations, such as
		''' <code>image/x-png</code> instead of the official
		''' <code>image/png</code>.  Finally, a single service provider may
		''' be capable of parsing multiple distinct types from the MIME
		''' point of view, for example <code>image/x-xbitmap</code> and
		''' <code>image/x-xpixmap</code>.
		''' 
		''' <p> Returning a particular MIME type does not guarantee that
		''' files claiming to be of that type can be processed; it merely
		''' indicates that it may be worthwhile attempting to decode or
		''' encode such files using this service provider.
		''' </summary>
		''' <returns> an array of <code>String</code>s or length at least 1
		''' containing MIME types associated with this reader or writer, or
		''' <code>null</code>. </returns>
		Public Overridable Property mIMETypes As String()
			Get
				Return If(MIMETypes Is Nothing, Nothing, CType(MIMETypes.clone(), String()))
			End Get
		End Property

		''' <summary>
		''' Returns the fully-qualified class name of the
		''' <code>ImageReader</code> or <code>ImageWriter</code> plug-in
		''' associated with this service provider.
		''' </summary>
		''' <returns> the class name, as a non-<code>null</code>
		''' <code>String</code>. </returns>
		Public Overridable Property pluginClassName As String
			Get
				Return pluginClassName
			End Get
		End Property

		''' <summary>
		''' Returns <code>true</code> if the standard metadata format is
		''' among the document formats recognized by the
		''' <code>getAsTree</code> and <code>setFromTree</code> methods on
		''' the stream metadata objects produced or consumed by this
		''' plug-in.
		''' </summary>
		''' <returns> <code>true</code> if the standard format is supported
		''' for stream metadata. </returns>
		Public Overridable Property standardStreamMetadataFormatSupported As Boolean
			Get
				Return supportsStandardStreamMetadataFormat
			End Get
		End Property

		''' <summary>
		''' Returns the name of the "native" stream metadata format for
		''' this plug-in, which typically allows for lossless encoding and
		''' transmission of the stream metadata stored in the format handled by
		''' this plug-in.  If no such format is supported,
		''' <code>null</code>will be returned.
		''' 
		''' <p> The default implementation returns the
		''' <code>nativeStreamMetadataFormatName</code> instance variable,
		''' which is typically set by the constructor.
		''' </summary>
		''' <returns> the name of the native stream metadata format, or
		''' <code>null</code>.
		'''  </returns>
		Public Overridable Property nativeStreamMetadataFormatName As String
			Get
				Return nativeStreamMetadataFormatName
			End Get
		End Property

		''' <summary>
		''' Returns an array of <code>String</code>s containing the names
		''' of additional document formats, other than the native and
		''' standard formats, recognized by the
		''' <code>getAsTree</code> and <code>setFromTree</code> methods on
		''' the stream metadata objects produced or consumed by this
		''' plug-in.
		''' 
		''' <p> If the plug-in does not handle metadata, null should be
		''' returned.
		''' 
		''' <p> The set of formats may differ according to the particular
		''' images being read or written; this method should indicate all
		''' the additional formats supported by the plug-in under any
		''' circumstances.
		''' 
		''' <p> The default implementation returns a clone of the
		''' <code>extraStreamMetadataFormatNames</code> instance variable,
		''' which is typically set by the constructor.
		''' </summary>
		''' <returns> an array of <code>String</code>s, or null.
		''' </returns>
		''' <seealso cref= IIOMetadata#getMetadataFormatNames </seealso>
		''' <seealso cref= #getExtraImageMetadataFormatNames </seealso>
		''' <seealso cref= #getNativeStreamMetadataFormatName </seealso>
		Public Overridable Property extraStreamMetadataFormatNames As String()
			Get
				Return If(extraStreamMetadataFormatNames Is Nothing, Nothing, CType(extraStreamMetadataFormatNames.clone(), String()))
			End Get
		End Property

		''' <summary>
		''' Returns <code>true</code> if the standard metadata format is
		''' among the document formats recognized by the
		''' <code>getAsTree</code> and <code>setFromTree</code> methods on
		''' the image metadata objects produced or consumed by this
		''' plug-in.
		''' </summary>
		''' <returns> <code>true</code> if the standard format is supported
		''' for image metadata. </returns>
		Public Overridable Property standardImageMetadataFormatSupported As Boolean
			Get
				Return supportsStandardImageMetadataFormat
			End Get
		End Property

		''' <summary>
		''' Returns the name of the "native" image metadata format for
		''' this plug-in, which typically allows for lossless encoding and
		''' transmission of the image metadata stored in the format handled by
		''' this plug-in.  If no such format is supported,
		''' <code>null</code>will be returned.
		''' 
		''' <p> The default implementation returns the
		''' <code>nativeImageMetadataFormatName</code> instance variable,
		''' which is typically set by the constructor.
		''' </summary>
		''' <returns> the name of the native image metadata format, or
		''' <code>null</code>.
		''' </returns>
		''' <seealso cref= #getExtraImageMetadataFormatNames </seealso>
		Public Overridable Property nativeImageMetadataFormatName As String
			Get
				Return nativeImageMetadataFormatName
			End Get
		End Property

		''' <summary>
		''' Returns an array of <code>String</code>s containing the names
		''' of additional document formats, other than the native and
		''' standard formats, recognized by the
		''' <code>getAsTree</code> and <code>setFromTree</code> methods on
		''' the image metadata objects produced or consumed by this
		''' plug-in.
		''' 
		''' <p> If the plug-in does not handle image metadata, null should
		''' be returned.
		''' 
		''' <p> The set of formats may differ according to the particular
		''' images being read or written; this method should indicate all
		''' the additional formats supported by the plug-in under any circumstances.
		''' 
		''' <p> The default implementation returns a clone of the
		''' <code>extraImageMetadataFormatNames</code> instance variable,
		''' which is typically set by the constructor.
		''' </summary>
		''' <returns> an array of <code>String</code>s, or null.
		''' </returns>
		''' <seealso cref= IIOMetadata#getMetadataFormatNames </seealso>
		''' <seealso cref= #getExtraStreamMetadataFormatNames </seealso>
		''' <seealso cref= #getNativeImageMetadataFormatName </seealso>
		Public Overridable Property extraImageMetadataFormatNames As String()
			Get
				Return If(extraImageMetadataFormatNames Is Nothing, Nothing, CType(extraImageMetadataFormatNames.clone(), String()))
			End Get
		End Property

		''' <summary>
		''' Returns an <code>IIOMetadataFormat</code> object describing the
		''' given stream metadata format, or <code>null</code> if no
		''' description is available.  The supplied name must be the native
		''' stream metadata format name, the standard metadata format name,
		''' or one of those returned by
		''' <code>getExtraStreamMetadataFormatNames</code>.
		''' </summary>
		''' <param name="formatName"> the desired stream metadata format.
		''' </param>
		''' <returns> an <code>IIOMetadataFormat</code> object.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>formatName</code>
		''' is <code>null</code> or is not a supported name. </exception>
		Public Overridable Function getStreamMetadataFormat(ByVal formatName As String) As javax.imageio.metadata.IIOMetadataFormat
			Return getMetadataFormat(formatName, supportsStandardStreamMetadataFormat, nativeStreamMetadataFormatName, nativeStreamMetadataFormatClassName, extraStreamMetadataFormatNames, extraStreamMetadataFormatClassNames)
		End Function

		''' <summary>
		''' Returns an <code>IIOMetadataFormat</code> object describing the
		''' given image metadata format, or <code>null</code> if no
		''' description is available.  The supplied name must be the native
		''' image metadata format name, the standard metadata format name,
		''' or one of those returned by
		''' <code>getExtraImageMetadataFormatNames</code>.
		''' </summary>
		''' <param name="formatName"> the desired image metadata format.
		''' </param>
		''' <returns> an <code>IIOMetadataFormat</code> object.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>formatName</code>
		''' is <code>null</code> or is not a supported name. </exception>
		Public Overridable Function getImageMetadataFormat(ByVal formatName As String) As javax.imageio.metadata.IIOMetadataFormat
			Return getMetadataFormat(formatName, supportsStandardImageMetadataFormat, nativeImageMetadataFormatName, nativeImageMetadataFormatClassName, extraImageMetadataFormatNames, extraImageMetadataFormatClassNames)
		End Function

		Private Function getMetadataFormat(ByVal formatName As String, ByVal supportsStandard As Boolean, ByVal nativeName As String, ByVal nativeClassName As String, ByVal extraNames As String (), ByVal extraClassNames As String ()) As javax.imageio.metadata.IIOMetadataFormat
			If formatName Is Nothing Then Throw New System.ArgumentException("formatName == null!")
			If supportsStandard AndAlso formatName.Equals(javax.imageio.metadata.IIOMetadataFormatImpl.standardMetadataFormatName) Then Return javax.imageio.metadata.IIOMetadataFormatImpl.standardFormatInstance
			Dim formatClassName As String = Nothing
			If formatName.Equals(nativeName) Then
				formatClassName = nativeClassName
			ElseIf extraNames IsNot Nothing Then
				For i As Integer = 0 To extraNames.Length - 1
					If formatName.Equals(extraNames(i)) Then
						formatClassName = extraClassNames(i)
						Exit For ' out of for
					End If
				Next i
			End If
			If formatClassName Is Nothing Then Throw New System.ArgumentException("Unsupported format name")
			Try
				Dim cls As Type = Type.GetType(formatClassName, True, ClassLoader.systemClassLoader)
				Dim meth As Method = cls.GetMethod("getInstance")
				Return CType(meth.invoke(Nothing), javax.imageio.metadata.IIOMetadataFormat)
			Catch e As Exception
				Dim ex As Exception = New IllegalStateException("Can't obtain format")
				ex.initCause(e)
				Throw ex
			End Try
		End Function
	End Class

End Namespace