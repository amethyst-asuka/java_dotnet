Imports System

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
	''' The service provider interface (SPI) for <code>ImageReader</code>s.
	''' For more information on service provider classes, see the class comment
	''' for the <code>IIORegistry</code> class.
	''' 
	''' <p> Each <code>ImageReaderSpi</code> provides several types of information
	''' about the <code>ImageReader</code> class with which it is associated.
	''' 
	''' <p> The name of the vendor who defined the SPI class and a
	''' brief description of the class are available via the
	''' <code>getVendorName</code>, <code>getDescription</code>,
	''' and <code>getVersion</code> methods.
	''' These methods may be internationalized to provide locale-specific
	''' output.  These methods are intended mainly to provide short,
	''' human-readable information that might be used to organize a pop-up
	''' menu or other list.
	''' 
	''' <p> Lists of format names, file suffixes, and MIME types associated
	''' with the service may be obtained by means of the
	''' <code>getFormatNames</code>, <code>getFileSuffixes</code>, and
	''' <code>getMIMETypes</code> methods.  These methods may be used to
	''' identify candidate <code>ImageReader</code>s for decoding a
	''' particular file or stream based on manual format selection, file
	''' naming, or MIME associations (for example, when accessing a file
	''' over HTTP or as an email attachment).
	''' 
	''' <p> A more reliable way to determine which <code>ImageReader</code>s
	''' are likely to be able to parse a particular data stream is provided
	''' by the <code>canDecodeInput</code> method.  This methods allows the
	''' service provider to inspect the actual stream contents.
	''' 
	''' <p> Finally, an instance of the <code>ImageReader</code> class
	''' associated with this service provider may be obtained by calling
	''' the <code>createReaderInstance</code> method.  Any heavyweight
	''' initialization, such as the loading of native libraries or creation
	''' of large tables, should be deferred at least until the first
	''' invocation of this method.
	''' </summary>
	''' <seealso cref= IIORegistry </seealso>
	''' <seealso cref= javax.imageio.ImageReader
	'''  </seealso>
	Public MustInherit Class ImageReaderSpi
		Inherits ImageReaderWriterSpi

		''' <summary>
		''' A single-element array, initially containing
		''' <code>ImageInputStream.class</code>, to be returned from
		''' <code>getInputTypes</code>. </summary>
		''' @deprecated Instead of using this field, directly create
		''' the equivalent array <code>{ ImageInputStream.class }</code>. 
		<Obsolete("Instead of using this field, directly create")> _
		Public Shared ReadOnly STANDARD_INPUT_TYPE As Type() = { GetType(javax.imageio.stream.ImageInputStream) }

		''' <summary>
		''' An array of <code>Class</code> objects to be returned from
		''' <code>getInputTypes</code>, initially <code>null</code>.
		''' </summary>
		Protected Friend inputTypes As Type() = Nothing

		''' <summary>
		''' An array of strings to be returned from
		''' <code>getImageWriterSpiNames</code>, initially
		''' <code>null</code>.
		''' </summary>
		Protected Friend writerSpiNames As String() = Nothing

		''' <summary>
		''' The <code>Class</code> of the reader, initially
		''' <code>null</code>.
		''' </summary>
		Private readerClass As Type = Nothing

		''' <summary>
		''' Constructs a blank <code>ImageReaderSpi</code>.  It is up to
		''' the subclass to initialize instance variables and/or override
		''' method implementations in order to provide working versions of
		''' all methods.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Constructs an <code>ImageReaderSpi</code> with a given
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
		''' <param name="readerClassName"> the fully-qualified name of the
		''' associated <code>ImageReader</code> class, as a
		''' non-<code>null</code> <code>String</code>. </param>
		''' <param name="inputTypes"> a non-<code>null</code> array of
		''' <code>Class</code> objects of length at least 1 indicating the
		''' legal input types. </param>
		''' <param name="writerSpiNames"> an array <code>String</code>s naming the
		''' classes of all associated <code>ImageWriter</code>s, or
		''' <code>null</code>.  An array of length 0 is normalized to
		''' <code>null</code>. </param>
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
		''' <exception cref="IllegalArgumentException"> if <code>readerClassName</code>
		''' is <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>inputTypes</code>
		''' is <code>null</code> or has length 0. </exception>
		Public Sub New(ByVal vendorName As String, ByVal version As String, ByVal names As String(), ByVal suffixes As String(), ByVal MIMETypes As String(), ByVal readerClassName As String, ByVal inputTypes As Type(), ByVal writerSpiNames As String(), ByVal supportsStandardStreamMetadataFormat As Boolean, ByVal nativeStreamMetadataFormatName As String, ByVal nativeStreamMetadataFormatClassName As String, ByVal extraStreamMetadataFormatNames As String(), ByVal extraStreamMetadataFormatClassNames As String(), ByVal supportsStandardImageMetadataFormat As Boolean, ByVal nativeImageMetadataFormatName As String, ByVal nativeImageMetadataFormatClassName As String, ByVal extraImageMetadataFormatNames As String(), ByVal extraImageMetadataFormatClassNames As String())
			MyBase.New(vendorName, version, names, suffixes, MIMETypes, readerClassName, supportsStandardStreamMetadataFormat, nativeStreamMetadataFormatName, nativeStreamMetadataFormatClassName, extraStreamMetadataFormatNames, extraStreamMetadataFormatClassNames, supportsStandardImageMetadataFormat, nativeImageMetadataFormatName, nativeImageMetadataFormatClassName, extraImageMetadataFormatNames, extraImageMetadataFormatClassNames)

			If inputTypes Is Nothing Then Throw New System.ArgumentException("inputTypes == null!")
			If inputTypes.Length = 0 Then Throw New System.ArgumentException("inputTypes.length == 0!")

			Me.inputTypes = If(inputTypes = STANDARD_INPUT_TYPE, New Type() { GetType(javax.imageio.stream.ImageInputStream) }, inputTypes.clone())

			' If length == 0, leave it null
			If writerSpiNames IsNot Nothing AndAlso writerSpiNames.Length > 0 Then Me.writerSpiNames = CType(writerSpiNames.clone(), String())
		End Sub

		''' <summary>
		''' Returns an array of <code>Class</code> objects indicating what
		''' types of objects may be used as arguments to the reader's
		''' <code>setInput</code> method.
		''' 
		''' <p> For most readers, which only accept input from an
		''' <code>ImageInputStream</code>, a single-element array
		''' containing <code>ImageInputStream.class</code> should be
		''' returned.
		''' </summary>
		''' <returns> a non-<code>null</code> array of
		''' <code>Class</code>objects of length at least 1. </returns>
		Public Overridable Property inputTypes As Type()
			Get
				Return CType(inputTypes.clone(), Type())
			End Get
		End Property

		''' <summary>
		''' Returns <code>true</code> if the supplied source object appears
		''' to be of the format supported by this reader.  Returning
		''' <code>true</code> from this method does not guarantee that
		''' reading will succeed, only that there appears to be a
		''' reasonable chance of success based on a brief inspection of the
		''' stream contents.  If the source is an
		''' <code>ImageInputStream</code>, implementations will commonly
		''' check the first several bytes of the stream for a "magic
		''' number" associated with the format.  Once actual reading has
		''' commenced, the reader may still indicate failure at any time
		''' prior to the completion of decoding.
		''' 
		''' <p> It is important that the state of the object not be
		''' disturbed in order that other <code>ImageReaderSpi</code>s can
		''' properly determine whether they are able to decode the object.
		''' In particular, if the source is an
		''' <code>ImageInputStream</code>, a
		''' <code>mark</code>/<code>reset</code> pair should be used to
		''' preserve the stream position.
		''' 
		''' <p> Formats such as "raw," which can potentially attempt
		''' to read nearly any stream, should return <code>false</code>
		''' in order to avoid being invoked in preference to a closer
		''' match.
		''' 
		''' <p> If <code>source</code> is not an instance of one of the
		''' classes returned by <code>getInputTypes</code>, the method
		''' should simply return <code>false</code>.
		''' </summary>
		''' <param name="source"> the object (typically an
		''' <code>ImageInputStream</code>) to be decoded.
		''' </param>
		''' <returns> <code>true</code> if it is likely that this stream can
		''' be decoded.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>source</code> is
		''' <code>null</code>. </exception>
		''' <exception cref="IOException"> if an I/O error occurs while reading the
		''' stream. </exception>
		Public MustOverride Function canDecodeInput(ByVal source As Object) As Boolean

		''' <summary>
		''' Returns an instance of the <code>ImageReader</code>
		''' implementation associated with this service provider.
		''' The returned object will initially be in an initial state
		''' as if its <code>reset</code> method had been called.
		''' 
		''' <p> The default implementation simply returns
		''' <code>createReaderInstance(null)</code>.
		''' </summary>
		''' <returns> an <code>ImageReader</code> instance.
		''' </returns>
		''' <exception cref="IOException"> if an error occurs during loading,
		''' or initialization of the reader class, or during instantiation
		''' or initialization of the reader object. </exception>
		Public Overridable Function createReaderInstance() As javax.imageio.ImageReader
			Return createReaderInstance(Nothing)
		End Function

		''' <summary>
		''' Returns an instance of the <code>ImageReader</code>
		''' implementation associated with this service provider.
		''' The returned object will initially be in an initial state
		''' as if its <code>reset</code> method had been called.
		''' 
		''' <p> An <code>Object</code> may be supplied to the plug-in at
		''' construction time.  The nature of the object is entirely
		''' plug-in specific.
		''' 
		''' <p> Typically, a plug-in will implement this method using code
		''' such as <code>return new MyImageReader(this)</code>.
		''' </summary>
		''' <param name="extension"> a plug-in specific extension object, which may
		''' be <code>null</code>.
		''' </param>
		''' <returns> an <code>ImageReader</code> instance.
		''' </returns>
		''' <exception cref="IOException"> if the attempt to instantiate
		''' the reader fails. </exception>
		''' <exception cref="IllegalArgumentException"> if the
		''' <code>ImageReader</code>'s constructor throws an
		''' <code>IllegalArgumentException</code> to indicate that the
		''' extension object is unsuitable. </exception>
		Public MustOverride Function createReaderInstance(ByVal extension As Object) As javax.imageio.ImageReader

		''' <summary>
		''' Returns <code>true</code> if the <code>ImageReader</code> object
		''' passed in is an instance of the <code>ImageReader</code>
		''' associated with this service provider.
		''' 
		''' <p> The default implementation compares the fully-qualified
		''' class name of the <code>reader</code> argument with the class
		''' name passed into the constructor.  This method may be overridden
		''' if more sophisticated checking is required.
		''' </summary>
		''' <param name="reader"> an <code>ImageReader</code> instance.
		''' </param>
		''' <returns> <code>true</code> if <code>reader</code> is recognized.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>reader</code> is
		''' <code>null</code>. </exception>
		Public Overridable Function isOwnReader(ByVal reader As javax.imageio.ImageReader) As Boolean
			If reader Is Nothing Then Throw New System.ArgumentException("reader == null!")
			Dim name As String = reader.GetType().name
			Return name.Equals(pluginClassName)
		End Function

		''' <summary>
		''' Returns an array of <code>String</code>s containing the fully
		''' qualified names of all the <code>ImageWriterSpi</code> classes
		''' that can understand the internal metadata representation used
		''' by the <code>ImageReader</code> associated with this service
		''' provider, or <code>null</code> if there are no such
		''' <code>ImageWriter</code>s specified.  If a
		''' non-<code>null</code> value is returned, it must have non-zero
		''' length.
		''' 
		''' <p> The first item in the array must be the name of the service
		''' provider for the "preferred" writer, as it will be used to
		''' instantiate the <code>ImageWriter</code> returned by
		''' <code>ImageIO.getImageWriter(ImageReader)</code>.
		''' 
		''' <p> This mechanism may be used to obtain
		''' <code>ImageWriters</code> that will understand the internal
		''' structure of non-pixel meta-data (see
		''' <code>IIOTreeInfo</code>) generated by an
		''' <code>ImageReader</code>.  By obtaining this data from the
		''' <code>ImageReader</code> and passing it on to one of the
		''' <code>ImageWriters</code> obtained with this method, a client
		''' program can read an image, modify it in some way, and write it
		''' back out while preserving all meta-data, without having to
		''' understand anything about the internal structure of the
		''' meta-data, or even about the image format.
		''' </summary>
		''' <returns> an array of <code>String</code>s of length at least 1
		''' containing names of <code>ImageWriterSpi</code>, or
		''' <code>null</code>.
		''' </returns>
		''' <seealso cref= javax.imageio.ImageIO#getImageWriter(ImageReader) </seealso>
		Public Overridable Property imageWriterSpiNames As String()
			Get
				Return If(writerSpiNames Is Nothing, Nothing, CType(writerSpiNames.clone(), String()))
			End Get
		End Property
	End Class

End Namespace