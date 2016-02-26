Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections
Imports System.Collections.Generic

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

Namespace javax.imageio


	''' <summary>
	''' An abstract superclass for parsing and decoding of images.  This
	''' class must be subclassed by classes that read in images in the
	''' context of the Java Image I/O framework.
	''' 
	''' <p> <code>ImageReader</code> objects are normally instantiated by
	''' the service provider interface (SPI) class for the specific format.
	''' Service provider classes (e.g., instances of
	''' <code>ImageReaderSpi</code>) are registered with the
	''' <code>IIORegistry</code>, which uses them for format recognition
	''' and presentation of available format readers and writers.
	''' 
	''' <p> When an input source is set (using the <code>setInput</code>
	''' method), it may be marked as "seek forward only".  This setting
	''' means that images contained within the input source will only be
	''' read in order, possibly allowing the reader to avoid caching
	''' portions of the input containing data associated with images that
	''' have been read previously.
	''' </summary>
	''' <seealso cref= ImageWriter </seealso>
	''' <seealso cref= javax.imageio.spi.IIORegistry </seealso>
	''' <seealso cref= javax.imageio.spi.ImageReaderSpi
	'''  </seealso>
	Public MustInherit Class ImageReader

		''' <summary>
		''' The <code>ImageReaderSpi</code> that instantiated this object,
		''' or <code>null</code> if its identity is not known or none
		''' exists.  By default it is initialized to <code>null</code>.
		''' </summary>
		Protected Friend originatingProvider As javax.imageio.spi.ImageReaderSpi

		''' <summary>
		''' The <code>ImageInputStream</code> or other
		''' <code>Object</code> by <code>setInput</code> and retrieved
		''' by <code>getInput</code>.  By default it is initialized to
		''' <code>null</code>.
		''' </summary>
		Protected Friend input As Object = Nothing

		''' <summary>
		''' <code>true</code> if the current input source has been marked
		''' as allowing only forward seeking by <code>setInput</code>.  By
		''' default, the value is <code>false</code>.
		''' </summary>
		''' <seealso cref= #minIndex </seealso>
		''' <seealso cref= #setInput </seealso>
		Protected Friend seekForwardOnly As Boolean = False

		''' <summary>
		''' <code>true</code> if the current input source has been marked
		''' as allowing metadata to be ignored by <code>setInput</code>.
		''' By default, the value is <code>false</code>.
		''' </summary>
		''' <seealso cref= #setInput </seealso>
		Protected Friend ignoreMetadata As Boolean = False

		''' <summary>
		''' The smallest valid index for reading, initially 0.  When
		''' <code>seekForwardOnly</code> is <code>true</code>, various methods
		''' may throw an <code>IndexOutOfBoundsException</code> on an
		''' attempt to access data associate with an image having a lower
		''' index.
		''' </summary>
		''' <seealso cref= #seekForwardOnly </seealso>
		''' <seealso cref= #setInput </seealso>
		Protected Friend minIndex As Integer = 0

		''' <summary>
		''' An array of <code>Locale</code>s which may be used to localize
		''' warning messages, or <code>null</code> if localization is not
		''' supported.
		''' </summary>
		Protected Friend availableLocales As java.util.Locale() = Nothing

		''' <summary>
		''' The current <code>Locale</code> to be used for localization, or
		''' <code>null</code> if none has been set.
		''' </summary>
		Protected Friend locale As java.util.Locale = Nothing

		''' <summary>
		''' A <code>List</code> of currently registered
		''' <code>IIOReadWarningListener</code>s, initialized by default to
		''' <code>null</code>, which is synonymous with an empty
		''' <code>List</code>.
		''' </summary>
		Protected Friend warningListeners As IList(Of javax.imageio.event.IIOReadWarningListener) = Nothing

		''' <summary>
		''' A <code>List</code> of the <code>Locale</code>s associated with
		''' each currently registered <code>IIOReadWarningListener</code>,
		''' initialized by default to <code>null</code>, which is
		''' synonymous with an empty <code>List</code>.
		''' </summary>
		Protected Friend warningLocales As IList(Of java.util.Locale) = Nothing

		''' <summary>
		''' A <code>List</code> of currently registered
		''' <code>IIOReadProgressListener</code>s, initialized by default
		''' to <code>null</code>, which is synonymous with an empty
		''' <code>List</code>.
		''' </summary>
		Protected Friend progressListeners As IList(Of javax.imageio.event.IIOReadProgressListener) = Nothing

		''' <summary>
		''' A <code>List</code> of currently registered
		''' <code>IIOReadUpdateListener</code>s, initialized by default to
		''' <code>null</code>, which is synonymous with an empty
		''' <code>List</code>.
		''' </summary>
		Protected Friend updateListeners As IList(Of javax.imageio.event.IIOReadUpdateListener) = Nothing

		''' <summary>
		''' If <code>true</code>, the current read operation should be
		''' aborted.
		''' </summary>
		Private abortFlag As Boolean = False

		''' <summary>
		''' Constructs an <code>ImageReader</code> and sets its
		''' <code>originatingProvider</code> field to the supplied value.
		''' 
		''' <p> Subclasses that make use of extensions should provide a
		''' constructor with signature <code>(ImageReaderSpi,
		''' Object)</code> in order to retrieve the extension object.  If
		''' the extension object is unsuitable, an
		''' <code>IllegalArgumentException</code> should be thrown.
		''' </summary>
		''' <param name="originatingProvider"> the <code>ImageReaderSpi</code> that is
		''' invoking this constructor, or <code>null</code>. </param>
		Protected Friend Sub New(ByVal originatingProvider As javax.imageio.spi.ImageReaderSpi)
			Me.originatingProvider = originatingProvider
		End Sub

		''' <summary>
		''' Returns a <code>String</code> identifying the format of the
		''' input source.
		''' 
		''' <p> The default implementation returns
		''' <code>originatingProvider.getFormatNames()[0]</code>.
		''' Implementations that may not have an originating service
		''' provider, or which desire a different naming policy should
		''' override this method.
		''' </summary>
		''' <exception cref="IOException"> if an error occurs reading the
		''' information from the input source.
		''' </exception>
		''' <returns> the format name, as a <code>String</code>. </returns>
		Public Overridable Property formatName As String
			Get
				Return originatingProvider.formatNames(0)
			End Get
		End Property

		''' <summary>
		''' Returns the <code>ImageReaderSpi</code> that was passed in on
		''' the constructor.  Note that this value may be <code>null</code>.
		''' </summary>
		''' <returns> an <code>ImageReaderSpi</code>, or <code>null</code>.
		''' </returns>
		''' <seealso cref= ImageReaderSpi </seealso>
		Public Overridable Property originatingProvider As javax.imageio.spi.ImageReaderSpi
			Get
				Return originatingProvider
			End Get
		End Property

		''' <summary>
		''' Sets the input source to use to the given
		''' <code>ImageInputStream</code> or other <code>Object</code>.
		''' The input source must be set before any of the query or read
		''' methods are used.  If <code>input</code> is <code>null</code>,
		''' any currently set input source will be removed.  In any case,
		''' the value of <code>minIndex</code> will be initialized to 0.
		''' 
		''' <p> The <code>seekForwardOnly</code> parameter controls whether
		''' the value returned by <code>getMinIndex</code> will be
		''' increased as each image (or thumbnail, or image metadata) is
		''' read.  If <code>seekForwardOnly</code> is true, then a call to
		''' <code>read(index)</code> will throw an
		''' <code>IndexOutOfBoundsException</code> if {@code index < this.minIndex};
		''' otherwise, the value of
		''' <code>minIndex</code> will be set to <code>index</code>.  If
		''' <code>seekForwardOnly</code> is <code>false</code>, the value of
		''' <code>minIndex</code> will remain 0 regardless of any read
		''' operations.
		''' 
		''' <p> The <code>ignoreMetadata</code> parameter, if set to
		''' <code>true</code>, allows the reader to disregard any metadata
		''' encountered during the read.  Subsequent calls to the
		''' <code>getStreamMetadata</code> and
		''' <code>getImageMetadata</code> methods may return
		''' <code>null</code>, and an <code>IIOImage</code> returned from
		''' <code>readAll</code> may return <code>null</code> from their
		''' <code>getMetadata</code> method.  Setting this parameter may
		''' allow the reader to work more efficiently.  The reader may
		''' choose to disregard this setting and return metadata normally.
		''' 
		''' <p> Subclasses should take care to remove any cached
		''' information based on the previous stream, such as header
		''' information or partially decoded image data.
		''' 
		''' <p> Use of a general <code>Object</code> other than an
		''' <code>ImageInputStream</code> is intended for readers that
		''' interact directly with a capture device or imaging protocol.
		''' The set of legal classes is advertised by the reader's service
		''' provider's <code>getInputTypes</code> method; most readers
		''' will return a single-element array containing only
		''' <code>ImageInputStream.class</code> to indicate that they
		''' accept only an <code>ImageInputStream</code>.
		''' 
		''' <p> The default implementation checks the <code>input</code>
		''' argument against the list returned by
		''' <code>originatingProvider.getInputTypes()</code> and fails
		''' if the argument is not an instance of one of the classes
		''' in the list.  If the originating provider is set to
		''' <code>null</code>, the input is accepted only if it is an
		''' <code>ImageInputStream</code>.
		''' </summary>
		''' <param name="input"> the <code>ImageInputStream</code> or other
		''' <code>Object</code> to use for future decoding. </param>
		''' <param name="seekForwardOnly"> if <code>true</code>, images and metadata
		''' may only be read in ascending order from this input source. </param>
		''' <param name="ignoreMetadata"> if <code>true</code>, metadata
		''' may be ignored during reads.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>input</code> is
		''' not an instance of one of the classes returned by the
		''' originating service provider's <code>getInputTypes</code>
		''' method, or is not an <code>ImageInputStream</code>.
		''' </exception>
		''' <seealso cref= ImageInputStream </seealso>
		''' <seealso cref= #getInput </seealso>
		''' <seealso cref= javax.imageio.spi.ImageReaderSpi#getInputTypes </seealso>
		Public Overridable Sub setInput(ByVal input As Object, ByVal seekForwardOnly As Boolean, ByVal ignoreMetadata As Boolean)
			If input IsNot Nothing Then
				Dim found As Boolean = False
				If originatingProvider IsNot Nothing Then
					Dim classes As Type() = originatingProvider.inputTypes
					For i As Integer = 0 To classes.Length - 1
						If classes(i).IsInstanceOfType(input) Then
							found = True
							Exit For
						End If
					Next i
				Else
					If TypeOf input Is javax.imageio.stream.ImageInputStream Then found = True
				End If
				If Not found Then Throw New System.ArgumentException("Incorrect input type!")

				Me.seekForwardOnly = seekForwardOnly
				Me.ignoreMetadata = ignoreMetadata
				Me.minIndex = 0
			End If

			Me.input = input
		End Sub

		''' <summary>
		''' Sets the input source to use to the given
		''' <code>ImageInputStream</code> or other <code>Object</code>.
		''' The input source must be set before any of the query or read
		''' methods are used.  If <code>input</code> is <code>null</code>,
		''' any currently set input source will be removed.  In any case,
		''' the value of <code>minIndex</code> will be initialized to 0.
		''' 
		''' <p> The <code>seekForwardOnly</code> parameter controls whether
		''' the value returned by <code>getMinIndex</code> will be
		''' increased as each image (or thumbnail, or image metadata) is
		''' read.  If <code>seekForwardOnly</code> is true, then a call to
		''' <code>read(index)</code> will throw an
		''' <code>IndexOutOfBoundsException</code> if {@code index < this.minIndex};
		''' otherwise, the value of
		''' <code>minIndex</code> will be set to <code>index</code>.  If
		''' <code>seekForwardOnly</code> is <code>false</code>, the value of
		''' <code>minIndex</code> will remain 0 regardless of any read
		''' operations.
		''' 
		''' <p> This method is equivalent to <code>setInput(input,
		''' seekForwardOnly, false)</code>.
		''' </summary>
		''' <param name="input"> the <code>ImageInputStream</code> or other
		''' <code>Object</code> to use for future decoding. </param>
		''' <param name="seekForwardOnly"> if <code>true</code>, images and metadata
		''' may only be read in ascending order from this input source.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>input</code> is
		''' not an instance of one of the classes returned by the
		''' originating service provider's <code>getInputTypes</code>
		''' method, or is not an <code>ImageInputStream</code>.
		''' </exception>
		''' <seealso cref= #getInput </seealso>
		Public Overridable Sub setInput(ByVal input As Object, ByVal seekForwardOnly As Boolean)
			inputput(input, seekForwardOnly, False)
		End Sub

		''' <summary>
		''' Sets the input source to use to the given
		''' <code>ImageInputStream</code> or other <code>Object</code>.
		''' The input source must be set before any of the query or read
		''' methods are used.  If <code>input</code> is <code>null</code>,
		''' any currently set input source will be removed.  In any case,
		''' the value of <code>minIndex</code> will be initialized to 0.
		''' 
		''' <p> This method is equivalent to <code>setInput(input, false,
		''' false)</code>.
		''' </summary>
		''' <param name="input"> the <code>ImageInputStream</code> or other
		''' <code>Object</code> to use for future decoding.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>input</code> is
		''' not an instance of one of the classes returned by the
		''' originating service provider's <code>getInputTypes</code>
		''' method, or is not an <code>ImageInputStream</code>.
		''' </exception>
		''' <seealso cref= #getInput </seealso>
		Public Overridable Property input As Object
			Set(ByVal input As Object)
				inputput(input, False, False)
			End Set
			Get
				Return input
			End Get
		End Property


		''' <summary>
		''' Returns <code>true</code> if the current input source has been
		''' marked as seek forward only by passing <code>true</code> as the
		''' <code>seekForwardOnly</code> argument to the
		''' <code>setInput</code> method.
		''' </summary>
		''' <returns> <code>true</code> if the input source is seek forward
		''' only.
		''' </returns>
		''' <seealso cref= #setInput </seealso>
		Public Overridable Property seekForwardOnly As Boolean
			Get
				Return seekForwardOnly
			End Get
		End Property

		''' <summary>
		''' Returns <code>true</code> if the current input source has been
		''' marked as allowing metadata to be ignored by passing
		''' <code>true</code> as the <code>ignoreMetadata</code> argument
		''' to the <code>setInput</code> method.
		''' </summary>
		''' <returns> <code>true</code> if the metadata may be ignored.
		''' </returns>
		''' <seealso cref= #setInput </seealso>
		Public Overridable Property ignoringMetadata As Boolean
			Get
				Return ignoreMetadata
			End Get
		End Property

		''' <summary>
		''' Returns the lowest valid index for reading an image, thumbnail,
		''' or image metadata.  If <code>seekForwardOnly()</code> is
		''' <code>false</code>, this value will typically remain 0,
		''' indicating that random access is possible.  Otherwise, it will
		''' contain the value of the most recently accessed index, and
		''' increase in a monotonic fashion.
		''' </summary>
		''' <returns> the minimum legal index for reading. </returns>
		Public Overridable Property minIndex As Integer
			Get
				Return minIndex
			End Get
		End Property

		' Localization

		''' <summary>
		''' Returns an array of <code>Locale</code>s that may be used to
		''' localize warning listeners and compression settings.  A return
		''' value of <code>null</code> indicates that localization is not
		''' supported.
		''' 
		''' <p> The default implementation returns a clone of the
		''' <code>availableLocales</code> instance variable if it is
		''' non-<code>null</code>, or else returns <code>null</code>.
		''' </summary>
		''' <returns> an array of <code>Locale</code>s that may be used as
		''' arguments to <code>setLocale</code>, or <code>null</code>. </returns>
		Public Overridable Property availableLocales As java.util.Locale()
			Get
				If availableLocales Is Nothing Then
					Return Nothing
				Else
					Return CType(availableLocales.clone(), java.util.Locale())
				End If
			End Get
		End Property

		''' <summary>
		''' Sets the current <code>Locale</code> of this
		''' <code>ImageReader</code> to the given value.  A value of
		''' <code>null</code> removes any previous setting, and indicates
		''' that the reader should localize as it sees fit.
		''' </summary>
		''' <param name="locale"> the desired <code>Locale</code>, or
		''' <code>null</code>.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>locale</code> is
		''' non-<code>null</code> but is not one of the values returned by
		''' <code>getAvailableLocales</code>.
		''' </exception>
		''' <seealso cref= #getLocale </seealso>
		Public Overridable Property locale As java.util.Locale
			Set(ByVal locale As java.util.Locale)
				If locale IsNot Nothing Then
					Dim locales As java.util.Locale() = availableLocales
					Dim found As Boolean = False
					If locales IsNot Nothing Then
						For i As Integer = 0 To locales.Length - 1
							If locale.Equals(locales(i)) Then
								found = True
								Exit For
							End If
						Next i
					End If
					If Not found Then Throw New System.ArgumentException("Invalid locale!")
				End If
				Me.locale = locale
			End Set
			Get
				Return locale
			End Get
		End Property


		' Image queries

		''' <summary>
		''' Returns the number of images, not including thumbnails, available
		''' from the current input source.
		''' 
		''' <p> Note that some image formats (such as animated GIF) do not
		''' specify how many images are present in the stream.  Thus
		''' determining the number of images will require the entire stream
		''' to be scanned and may require memory for buffering.  If images
		''' are to be processed in order, it may be more efficient to
		''' simply call <code>read</code> with increasing indices until an
		''' <code>IndexOutOfBoundsException</code> is thrown to indicate
		''' that no more images are available.  The
		''' <code>allowSearch</code> parameter may be set to
		''' <code>false</code> to indicate that an exhaustive search is not
		''' desired; the return value will be <code>-1</code> to indicate
		''' that a search is necessary.  If the input has been specified
		''' with <code>seekForwardOnly</code> set to <code>true</code>,
		''' this method throws an <code>IllegalStateException</code> if
		''' <code>allowSearch</code> is set to <code>true</code>.
		''' </summary>
		''' <param name="allowSearch"> if <code>true</code>, the true number of
		''' images will be returned even if a search is required.  If
		''' <code>false</code>, the reader may return <code>-1</code>
		''' without performing the search.
		''' </param>
		''' <returns> the number of images, as an <code>int</code>, or
		''' <code>-1</code> if <code>allowSearch</code> is
		''' <code>false</code> and a search would be required.
		''' </returns>
		''' <exception cref="IllegalStateException"> if the input source has not been set,
		''' or if the input has been specified with <code>seekForwardOnly</code>
		''' set to <code>true</code>. </exception>
		''' <exception cref="IOException"> if an error occurs reading the
		''' information from the input source.
		''' </exception>
		''' <seealso cref= #setInput </seealso>
		Public MustOverride Function getNumImages(ByVal allowSearch As Boolean) As Integer

		''' <summary>
		''' Returns the width in pixels of the given image within the input
		''' source.
		''' 
		''' <p> If the image can be rendered to a user-specified size, then
		''' this method returns the default width.
		''' </summary>
		''' <param name="imageIndex"> the index of the image to be queried.
		''' </param>
		''' <returns> the width of the image, as an <code>int</code>.
		''' </returns>
		''' <exception cref="IllegalStateException"> if the input source has not been set. </exception>
		''' <exception cref="IndexOutOfBoundsException"> if the supplied index is
		''' out of bounds. </exception>
		''' <exception cref="IOException"> if an error occurs reading the width
		''' information from the input source. </exception>
		Public MustOverride Function getWidth(ByVal imageIndex As Integer) As Integer

		''' <summary>
		''' Returns the height in pixels of the given image within the
		''' input source.
		''' 
		''' <p> If the image can be rendered to a user-specified size, then
		''' this method returns the default height.
		''' </summary>
		''' <param name="imageIndex"> the index of the image to be queried.
		''' </param>
		''' <returns> the height of the image, as an <code>int</code>.
		''' </returns>
		''' <exception cref="IllegalStateException"> if the input source has not been set. </exception>
		''' <exception cref="IndexOutOfBoundsException"> if the supplied index is
		''' out of bounds. </exception>
		''' <exception cref="IOException"> if an error occurs reading the height
		''' information from the input source. </exception>
		Public MustOverride Function getHeight(ByVal imageIndex As Integer) As Integer

		''' <summary>
		''' Returns <code>true</code> if the storage format of the given
		''' image places no inherent impediment on random access to pixels.
		''' For most compressed formats, such as JPEG, this method should
		''' return <code>false</code>, as a large section of the image in
		''' addition to the region of interest may need to be decoded.
		''' 
		''' <p> This is merely a hint for programs that wish to be
		''' efficient; all readers must be able to read arbitrary regions
		''' as specified in an <code>ImageReadParam</code>.
		''' 
		''' <p> Note that formats that return <code>false</code> from
		''' this method may nonetheless allow tiling (<i>e.g.</i> Restart
		''' Markers in JPEG), and random access will likely be reasonably
		''' efficient on tiles.  See <seealso cref="#isImageTiled isImageTiled"/>.
		''' 
		''' <p> A reader for which all images are guaranteed to support
		''' easy random access, or are guaranteed not to support easy
		''' random access, may return <code>true</code> or
		''' <code>false</code> respectively without accessing any image
		''' data.  In such cases, it is not necessary to throw an exception
		''' even if no input source has been set or the image index is out
		''' of bounds.
		''' 
		''' <p> The default implementation returns <code>false</code>.
		''' </summary>
		''' <param name="imageIndex"> the index of the image to be queried.
		''' </param>
		''' <returns> <code>true</code> if reading a region of interest of
		''' the given image is likely to be efficient.
		''' </returns>
		''' <exception cref="IllegalStateException"> if an input source is required
		''' to determine the return value, but none has been set. </exception>
		''' <exception cref="IndexOutOfBoundsException"> if an image must be
		''' accessed to determine the return value, but the supplied index
		''' is out of bounds. </exception>
		''' <exception cref="IOException"> if an error occurs during reading. </exception>
		Public Overridable Function isRandomAccessEasy(ByVal imageIndex As Integer) As Boolean
			Return False
		End Function

		''' <summary>
		''' Returns the aspect ratio of the given image (that is, its width
		''' divided by its height) as a <code>float</code>.  For images
		''' that are inherently resizable, this method provides a way to
		''' determine the appropriate width given a desired height, or vice
		''' versa.  For non-resizable images, the true width and height
		''' are used.
		''' 
		''' <p> The default implementation simply returns
		''' <code>(float)getWidth(imageIndex)/getHeight(imageIndex)</code>.
		''' </summary>
		''' <param name="imageIndex"> the index of the image to be queried.
		''' </param>
		''' <returns> a <code>float</code> indicating the aspect ratio of the
		''' given image.
		''' </returns>
		''' <exception cref="IllegalStateException"> if the input source has not been set. </exception>
		''' <exception cref="IndexOutOfBoundsException"> if the supplied index is
		''' out of bounds. </exception>
		''' <exception cref="IOException"> if an error occurs during reading. </exception>
		Public Overridable Function getAspectRatio(ByVal imageIndex As Integer) As Single
			Return CSng(getWidth(imageIndex))/getHeight(imageIndex)
		End Function

		''' <summary>
		''' Returns an <code>ImageTypeSpecifier</code> indicating the
		''' <code>SampleModel</code> and <code>ColorModel</code> which most
		''' closely represents the "raw" internal format of the image.  For
		''' example, for a JPEG image the raw type might have a YCbCr color
		''' space even though the image would conventionally be transformed
		''' into an RGB color space prior to display.  The returned value
		''' should also be included in the list of values returned by
		''' <code>getImageTypes</code>.
		''' 
		''' <p> The default implementation simply returns the first entry
		''' from the list provided by <code>getImageType</code>.
		''' </summary>
		''' <param name="imageIndex"> the index of the image to be queried.
		''' </param>
		''' <returns> an <code>ImageTypeSpecifier</code>.
		''' </returns>
		''' <exception cref="IllegalStateException"> if the input source has not been set. </exception>
		''' <exception cref="IndexOutOfBoundsException"> if the supplied index is
		''' out of bounds. </exception>
		''' <exception cref="IOException"> if an error occurs reading the format
		''' information from the input source. </exception>
		Public Overridable Function getRawImageType(ByVal imageIndex As Integer) As ImageTypeSpecifier
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return CType(getImageTypes(imageIndex).next(), ImageTypeSpecifier)
		End Function

		''' <summary>
		''' Returns an <code>Iterator</code> containing possible image
		''' types to which the given image may be decoded, in the form of
		''' <code>ImageTypeSpecifiers</code>s.  At least one legal image
		''' type will be returned.
		''' 
		''' <p> The first element of the iterator should be the most
		''' "natural" type for decoding the image with as little loss as
		''' possible.  For example, for a JPEG image the first entry should
		''' be an RGB image, even though the image data is stored
		''' internally in a YCbCr color space.
		''' </summary>
		''' <param name="imageIndex"> the index of the image to be
		''' <code>retrieved</code>.
		''' </param>
		''' <returns> an <code>Iterator</code> containing at least one
		''' <code>ImageTypeSpecifier</code> representing suggested image
		''' types for decoding the current given image.
		''' </returns>
		''' <exception cref="IllegalStateException"> if the input source has not been set. </exception>
		''' <exception cref="IndexOutOfBoundsException"> if the supplied index is
		''' out of bounds. </exception>
		''' <exception cref="IOException"> if an error occurs reading the format
		''' information from the input source.
		''' </exception>
		''' <seealso cref= ImageReadParam#setDestination(BufferedImage) </seealso>
		''' <seealso cref= ImageReadParam#setDestinationType(ImageTypeSpecifier) </seealso>
		Public MustOverride Function getImageTypes(ByVal imageIndex As Integer) As IEnumerator(Of ImageTypeSpecifier)

		''' <summary>
		''' Returns a default <code>ImageReadParam</code> object
		''' appropriate for this format.  All subclasses should define a
		''' set of default values for all parameters and return them with
		''' this call.  This method may be called before the input source
		''' is set.
		''' 
		''' <p> The default implementation constructs and returns a new
		''' <code>ImageReadParam</code> object that does not allow source
		''' scaling (<i>i.e.</i>, it returns <code>new
		''' ImageReadParam()</code>.
		''' </summary>
		''' <returns> an <code>ImageReadParam</code> object which may be used
		''' to control the decoding process using a set of default settings. </returns>
		Public Overridable Property defaultReadParam As ImageReadParam
			Get
				Return New ImageReadParam
			End Get
		End Property

		''' <summary>
		''' Returns an <code>IIOMetadata</code> object representing the
		''' metadata associated with the input source as a whole (i.e., not
		''' associated with any particular image), or <code>null</code> if
		''' the reader does not support reading metadata, is set to ignore
		''' metadata, or if no metadata is available.
		''' </summary>
		''' <returns> an <code>IIOMetadata</code> object, or <code>null</code>.
		''' </returns>
		''' <exception cref="IOException"> if an error occurs during reading. </exception>
		Public MustOverride ReadOnly Property streamMetadata As javax.imageio.metadata.IIOMetadata

		''' <summary>
		''' Returns an <code>IIOMetadata</code> object representing the
		''' metadata associated with the input source as a whole (i.e.,
		''' not associated with any particular image).  If no such data
		''' exists, <code>null</code> is returned.
		''' 
		''' <p> The resulting metadata object is only responsible for
		''' returning documents in the format named by
		''' <code>formatName</code>.  Within any documents that are
		''' returned, only nodes whose names are members of
		''' <code>nodeNames</code> are required to be returned.  In this
		''' way, the amount of metadata processing done by the reader may
		''' be kept to a minimum, based on what information is actually
		''' needed.
		''' 
		''' <p> If <code>formatName</code> is not the name of a supported
		''' metadata format, <code>null</code> is returned.
		''' 
		''' <p> In all cases, it is legal to return a more capable metadata
		''' object than strictly necessary.  The format name and node names
		''' are merely hints that may be used to reduce the reader's
		''' workload.
		''' 
		''' <p> The default implementation simply returns the result of
		''' calling <code>getStreamMetadata()</code>, after checking that
		''' the format name is supported.  If it is not,
		''' <code>null</code> is returned.
		''' </summary>
		''' <param name="formatName"> a metadata format name that may be used to retrieve
		''' a document from the returned <code>IIOMetadata</code> object. </param>
		''' <param name="nodeNames"> a <code>Set</code> containing the names of
		''' nodes that may be contained in a retrieved document.
		''' </param>
		''' <returns> an <code>IIOMetadata</code> object, or <code>null</code>.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>formatName</code>
		''' is <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>nodeNames</code>
		''' is <code>null</code>. </exception>
		''' <exception cref="IOException"> if an error occurs during reading. </exception>
		Public Overridable Function getStreamMetadata(ByVal formatName As String, ByVal nodeNames As java.util.Set(Of String)) As javax.imageio.metadata.IIOMetadata
			Return getMetadata(formatName, nodeNames, True, 0)
		End Function

		Private Function getMetadata(ByVal formatName As String, ByVal nodeNames As java.util.Set, ByVal wantStream As Boolean, ByVal imageIndex As Integer) As javax.imageio.metadata.IIOMetadata
			If formatName Is Nothing Then Throw New System.ArgumentException("formatName == null!")
			If nodeNames Is Nothing Then Throw New System.ArgumentException("nodeNames == null!")
			Dim ___metadata As javax.imageio.metadata.IIOMetadata = If(wantStream, streamMetadata, getImageMetadata(imageIndex))
			If ___metadata IsNot Nothing Then
				If ___metadata.standardMetadataFormatSupported AndAlso formatName.Equals(javax.imageio.metadata.IIOMetadataFormatImpl.standardMetadataFormatName) Then Return ___metadata
				Dim nativeName As String = ___metadata.nativeMetadataFormatName
				If nativeName IsNot Nothing AndAlso formatName.Equals(nativeName) Then Return ___metadata
				Dim extraNames As String() = ___metadata.extraMetadataFormatNames
				If extraNames IsNot Nothing Then
					For i As Integer = 0 To extraNames.Length - 1
						If formatName.Equals(extraNames(i)) Then Return ___metadata
					Next i
				End If
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Returns an <code>IIOMetadata</code> object containing metadata
		''' associated with the given image, or <code>null</code> if the
		''' reader does not support reading metadata, is set to ignore
		''' metadata, or if no metadata is available.
		''' </summary>
		''' <param name="imageIndex"> the index of the image whose metadata is to
		''' be retrieved.
		''' </param>
		''' <returns> an <code>IIOMetadata</code> object, or
		''' <code>null</code>.
		''' </returns>
		''' <exception cref="IllegalStateException"> if the input source has not been
		''' set. </exception>
		''' <exception cref="IndexOutOfBoundsException"> if the supplied index is
		''' out of bounds. </exception>
		''' <exception cref="IOException"> if an error occurs during reading. </exception>
		Public MustOverride Function getImageMetadata(ByVal imageIndex As Integer) As javax.imageio.metadata.IIOMetadata

		''' <summary>
		''' Returns an <code>IIOMetadata</code> object representing the
		''' metadata associated with the given image, or <code>null</code>
		''' if the reader does not support reading metadata or none
		''' is available.
		''' 
		''' <p> The resulting metadata object is only responsible for
		''' returning documents in the format named by
		''' <code>formatName</code>.  Within any documents that are
		''' returned, only nodes whose names are members of
		''' <code>nodeNames</code> are required to be returned.  In this
		''' way, the amount of metadata processing done by the reader may
		''' be kept to a minimum, based on what information is actually
		''' needed.
		''' 
		''' <p> If <code>formatName</code> is not the name of a supported
		''' metadata format, <code>null</code> may be returned.
		''' 
		''' <p> In all cases, it is legal to return a more capable metadata
		''' object than strictly necessary.  The format name and node names
		''' are merely hints that may be used to reduce the reader's
		''' workload.
		''' 
		''' <p> The default implementation simply returns the result of
		''' calling <code>getImageMetadata(imageIndex)</code>, after
		''' checking that the format name is supported.  If it is not,
		''' <code>null</code> is returned.
		''' </summary>
		''' <param name="imageIndex"> the index of the image whose metadata is to
		''' be retrieved. </param>
		''' <param name="formatName"> a metadata format name that may be used to retrieve
		''' a document from the returned <code>IIOMetadata</code> object. </param>
		''' <param name="nodeNames"> a <code>Set</code> containing the names of
		''' nodes that may be contained in a retrieved document.
		''' </param>
		''' <returns> an <code>IIOMetadata</code> object, or <code>null</code>.
		''' </returns>
		''' <exception cref="IllegalStateException"> if the input source has not been
		''' set. </exception>
		''' <exception cref="IndexOutOfBoundsException"> if the supplied index is
		''' out of bounds. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>formatName</code>
		''' is <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>nodeNames</code>
		''' is <code>null</code>. </exception>
		''' <exception cref="IOException"> if an error occurs during reading. </exception>
		Public Overridable Function getImageMetadata(ByVal imageIndex As Integer, ByVal formatName As String, ByVal nodeNames As java.util.Set(Of String)) As javax.imageio.metadata.IIOMetadata
			Return getMetadata(formatName, nodeNames, False, imageIndex)
		End Function

		''' <summary>
		''' Reads the image indexed by <code>imageIndex</code> and returns
		''' it as a complete <code>BufferedImage</code>, using a default
		''' <code>ImageReadParam</code>.  This is a convenience method
		''' that calls <code>read(imageIndex, null)</code>.
		''' 
		''' <p> The image returned will be formatted according to the first
		''' <code>ImageTypeSpecifier</code> returned from
		''' <code>getImageTypes</code>.
		''' 
		''' <p> Any registered <code>IIOReadProgressListener</code> objects
		''' will be notified by calling their <code>imageStarted</code>
		''' method, followed by calls to their <code>imageProgress</code>
		''' method as the read progresses.  Finally their
		''' <code>imageComplete</code> method will be called.
		''' <code>IIOReadUpdateListener</code> objects may be updated at
		''' other times during the read as pixels are decoded.  Finally,
		''' <code>IIOReadWarningListener</code> objects will receive
		''' notification of any non-fatal warnings that occur during
		''' decoding.
		''' </summary>
		''' <param name="imageIndex"> the index of the image to be retrieved.
		''' </param>
		''' <returns> the desired portion of the image as a
		''' <code>BufferedImage</code>.
		''' </returns>
		''' <exception cref="IllegalStateException"> if the input source has not been
		''' set. </exception>
		''' <exception cref="IndexOutOfBoundsException"> if the supplied index is
		''' out of bounds. </exception>
		''' <exception cref="IOException"> if an error occurs during reading. </exception>
		Public Overridable Function read(ByVal imageIndex As Integer) As java.awt.image.BufferedImage
			Return read(imageIndex, Nothing)
		End Function

		''' <summary>
		''' Reads the image indexed by <code>imageIndex</code> and returns
		''' it as a complete <code>BufferedImage</code>, using a supplied
		''' <code>ImageReadParam</code>.
		''' 
		''' <p> The actual <code>BufferedImage</code> returned will be
		''' chosen using the algorithm defined by the
		''' <code>getDestination</code> method.
		''' 
		''' <p> Any registered <code>IIOReadProgressListener</code> objects
		''' will be notified by calling their <code>imageStarted</code>
		''' method, followed by calls to their <code>imageProgress</code>
		''' method as the read progresses.  Finally their
		''' <code>imageComplete</code> method will be called.
		''' <code>IIOReadUpdateListener</code> objects may be updated at
		''' other times during the read as pixels are decoded.  Finally,
		''' <code>IIOReadWarningListener</code> objects will receive
		''' notification of any non-fatal warnings that occur during
		''' decoding.
		''' 
		''' <p> The set of source bands to be read and destination bands to
		''' be written is determined by calling <code>getSourceBands</code>
		''' and <code>getDestinationBands</code> on the supplied
		''' <code>ImageReadParam</code>.  If the lengths of the arrays
		''' returned by these methods differ, the set of source bands
		''' contains an index larger that the largest available source
		''' index, or the set of destination bands contains an index larger
		''' than the largest legal destination index, an
		''' <code>IllegalArgumentException</code> is thrown.
		''' 
		''' <p> If the supplied <code>ImageReadParam</code> contains
		''' optional setting values not supported by this reader (<i>e.g.</i>
		''' source render size or any format-specific settings), they will
		''' be ignored.
		''' </summary>
		''' <param name="imageIndex"> the index of the image to be retrieved. </param>
		''' <param name="param"> an <code>ImageReadParam</code> used to control
		''' the reading process, or <code>null</code>.
		''' </param>
		''' <returns> the desired portion of the image as a
		''' <code>BufferedImage</code>.
		''' </returns>
		''' <exception cref="IllegalStateException"> if the input source has not been
		''' set. </exception>
		''' <exception cref="IndexOutOfBoundsException"> if the supplied index is
		''' out of bounds. </exception>
		''' <exception cref="IllegalArgumentException"> if the set of source and
		''' destination bands specified by
		''' <code>param.getSourceBands</code> and
		''' <code>param.getDestinationBands</code> differ in length or
		''' include indices that are out of bounds. </exception>
		''' <exception cref="IllegalArgumentException"> if the resulting image would
		''' have a width or height less than 1. </exception>
		''' <exception cref="IOException"> if an error occurs during reading. </exception>
		Public MustOverride Function read(ByVal imageIndex As Integer, ByVal param As ImageReadParam) As java.awt.image.BufferedImage

		''' <summary>
		''' Reads the image indexed by <code>imageIndex</code> and returns
		''' an <code>IIOImage</code> containing the image, thumbnails, and
		''' associated image metadata, using a supplied
		''' <code>ImageReadParam</code>.
		''' 
		''' <p> The actual <code>BufferedImage</code> referenced by the
		''' returned <code>IIOImage</code> will be chosen using the
		''' algorithm defined by the <code>getDestination</code> method.
		''' 
		''' <p> Any registered <code>IIOReadProgressListener</code> objects
		''' will be notified by calling their <code>imageStarted</code>
		''' method, followed by calls to their <code>imageProgress</code>
		''' method as the read progresses.  Finally their
		''' <code>imageComplete</code> method will be called.
		''' <code>IIOReadUpdateListener</code> objects may be updated at
		''' other times during the read as pixels are decoded.  Finally,
		''' <code>IIOReadWarningListener</code> objects will receive
		''' notification of any non-fatal warnings that occur during
		''' decoding.
		''' 
		''' <p> The set of source bands to be read and destination bands to
		''' be written is determined by calling <code>getSourceBands</code>
		''' and <code>getDestinationBands</code> on the supplied
		''' <code>ImageReadParam</code>.  If the lengths of the arrays
		''' returned by these methods differ, the set of source bands
		''' contains an index larger that the largest available source
		''' index, or the set of destination bands contains an index larger
		''' than the largest legal destination index, an
		''' <code>IllegalArgumentException</code> is thrown.
		''' 
		''' <p> Thumbnails will be returned in their entirety regardless of
		''' the region settings.
		''' 
		''' <p> If the supplied <code>ImageReadParam</code> contains
		''' optional setting values not supported by this reader (<i>e.g.</i>
		''' source render size or any format-specific settings), those
		''' values will be ignored.
		''' </summary>
		''' <param name="imageIndex"> the index of the image to be retrieved. </param>
		''' <param name="param"> an <code>ImageReadParam</code> used to control
		''' the reading process, or <code>null</code>.
		''' </param>
		''' <returns> an <code>IIOImage</code> containing the desired portion
		''' of the image, a set of thumbnails, and associated image
		''' metadata.
		''' </returns>
		''' <exception cref="IllegalStateException"> if the input source has not been
		''' set. </exception>
		''' <exception cref="IndexOutOfBoundsException"> if the supplied index is
		''' out of bounds. </exception>
		''' <exception cref="IllegalArgumentException"> if the set of source and
		''' destination bands specified by
		''' <code>param.getSourceBands</code> and
		''' <code>param.getDestinationBands</code> differ in length or
		''' include indices that are out of bounds. </exception>
		''' <exception cref="IllegalArgumentException"> if the resulting image
		''' would have a width or height less than 1. </exception>
		''' <exception cref="IOException"> if an error occurs during reading. </exception>
		Public Overridable Function readAll(ByVal imageIndex As Integer, ByVal param As ImageReadParam) As IIOImage
			If imageIndex < minIndex Then Throw New System.IndexOutOfRangeException("imageIndex < getMinIndex()!")

			Dim im As java.awt.image.BufferedImage = read(imageIndex, param)

			Dim thumbnails As ArrayList = Nothing
			Dim ___numThumbnails As Integer = getNumThumbnails(imageIndex)
			If ___numThumbnails > 0 Then
				thumbnails = New ArrayList
				For j As Integer = 0 To ___numThumbnails - 1
					thumbnails.Add(readThumbnail(imageIndex, j))
				Next j
			End If

			Dim ___metadata As javax.imageio.metadata.IIOMetadata = getImageMetadata(imageIndex)
			Return New IIOImage(im, thumbnails, ___metadata)
		End Function

		''' <summary>
		''' Returns an <code>Iterator</code> containing all the images,
		''' thumbnails, and metadata, starting at the index given by
		''' <code>getMinIndex</code>, from the input source in the form of
		''' <code>IIOImage</code> objects.  An <code>Iterator</code>
		''' containing <code>ImageReadParam</code> objects is supplied; one
		''' element is consumed for each image read from the input source
		''' until no more images are available.  If the read param
		''' <code>Iterator</code> runs out of elements, but there are still
		''' more images available from the input source, default read
		''' params are used for the remaining images.
		''' 
		''' <p> If <code>params</code> is <code>null</code>, a default read
		''' param will be used for all images.
		''' 
		''' <p> The actual <code>BufferedImage</code> referenced by the
		''' returned <code>IIOImage</code> will be chosen using the
		''' algorithm defined by the <code>getDestination</code> method.
		''' 
		''' <p> Any registered <code>IIOReadProgressListener</code> objects
		''' will be notified by calling their <code>sequenceStarted</code>
		''' method once.  Then, for each image decoded, there will be a
		''' call to <code>imageStarted</code>, followed by calls to
		''' <code>imageProgress</code> as the read progresses, and finally
		''' to <code>imageComplete</code>.  The
		''' <code>sequenceComplete</code> method will be called after the
		''' last image has been decoded.
		''' <code>IIOReadUpdateListener</code> objects may be updated at
		''' other times during the read as pixels are decoded.  Finally,
		''' <code>IIOReadWarningListener</code> objects will receive
		''' notification of any non-fatal warnings that occur during
		''' decoding.
		''' 
		''' <p> The set of source bands to be read and destination bands to
		''' be written is determined by calling <code>getSourceBands</code>
		''' and <code>getDestinationBands</code> on the supplied
		''' <code>ImageReadParam</code>.  If the lengths of the arrays
		''' returned by these methods differ, the set of source bands
		''' contains an index larger that the largest available source
		''' index, or the set of destination bands contains an index larger
		''' than the largest legal destination index, an
		''' <code>IllegalArgumentException</code> is thrown.
		''' 
		''' <p> Thumbnails will be returned in their entirety regardless of the
		''' region settings.
		''' 
		''' <p> If any of the supplied <code>ImageReadParam</code>s contain
		''' optional setting values not supported by this reader (<i>e.g.</i>
		''' source render size or any format-specific settings), they will
		''' be ignored.
		''' </summary>
		''' <param name="params"> an <code>Iterator</code> containing
		''' <code>ImageReadParam</code> objects.
		''' </param>
		''' <returns> an <code>Iterator</code> representing the
		''' contents of the input source as <code>IIOImage</code>s.
		''' </returns>
		''' <exception cref="IllegalStateException"> if the input source has not been
		''' set. </exception>
		''' <exception cref="IllegalArgumentException"> if any
		''' non-<code>null</code> element of <code>params</code> is not an
		''' <code>ImageReadParam</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if the set of source and
		''' destination bands specified by
		''' <code>param.getSourceBands</code> and
		''' <code>param.getDestinationBands</code> differ in length or
		''' include indices that are out of bounds. </exception>
		''' <exception cref="IllegalArgumentException"> if a resulting image would
		''' have a width or height less than 1. </exception>
		''' <exception cref="IOException"> if an error occurs during reading.
		''' </exception>
		''' <seealso cref= ImageReadParam </seealso>
		''' <seealso cref= IIOImage </seealso>
		Public Overridable Function readAll(Of T1 As ImageReadParam)(ByVal params As IEnumerator(Of T1)) As IEnumerator(Of IIOImage)
			Dim output As IList = New ArrayList

			Dim imageIndex As Integer = minIndex

			' Inform IIOReadProgressListeners we're starting a sequence
			processSequenceStarted(imageIndex)

			Do
				' Inform IIOReadProgressListeners and IIOReadUpdateListeners
				' that we're starting a new image

				Dim param As ImageReadParam = Nothing
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				If params IsNot Nothing AndAlso params.hasNext() Then
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim o As Object = params.next()
					If o IsNot Nothing Then
						If TypeOf o Is ImageReadParam Then
							param = CType(o, ImageReadParam)
						Else
							Throw New System.ArgumentException("Non-ImageReadParam supplied as part of params!")
						End If
					End If
				End If

				Dim bi As java.awt.image.BufferedImage = Nothing
				Try
					bi = read(imageIndex, param)
				Catch e As System.IndexOutOfRangeException
					Exit Do
				End Try

				Dim thumbnails As ArrayList = Nothing
				Dim ___numThumbnails As Integer = getNumThumbnails(imageIndex)
				If ___numThumbnails > 0 Then
					thumbnails = New ArrayList
					For j As Integer = 0 To ___numThumbnails - 1
						thumbnails.Add(readThumbnail(imageIndex, j))
					Next j
				End If

				Dim ___metadata As javax.imageio.metadata.IIOMetadata = getImageMetadata(imageIndex)
				Dim im As New IIOImage(bi, thumbnails, ___metadata)
				output.Add(im)

				imageIndex += 1
			Loop

			' Inform IIOReadProgressListeners we're ending a sequence
			processSequenceComplete()

			Return output.GetEnumerator()
		End Function

		''' <summary>
		''' Returns <code>true</code> if this plug-in supports reading
		''' just a <seealso cref="java.awt.image.Raster Raster"/> of pixel data.
		''' If this method returns <code>false</code>, calls to
		''' <seealso cref="#readRaster readRaster"/> or <seealso cref="#readTileRaster readTileRaster"/>
		''' will throw an <code>UnsupportedOperationException</code>.
		''' 
		''' <p> The default implementation returns <code>false</code>.
		''' </summary>
		''' <returns> <code>true</code> if this plug-in supports reading raw
		''' <code>Raster</code>s.
		''' </returns>
		''' <seealso cref= #readRaster </seealso>
		''' <seealso cref= #readTileRaster </seealso>
		Public Overridable Function canReadRaster() As Boolean
			Return False
		End Function

		''' <summary>
		''' Returns a new <code>Raster</code> object containing the raw pixel data
		''' from the image stream, without any color conversion applied.  The
		''' application must determine how to interpret the pixel data by other
		''' means.  Any destination or image-type parameters in the supplied
		''' <code>ImageReadParam</code> object are ignored, but all other
		''' parameters are used exactly as in the <seealso cref="#read read"/>
		''' method, except that any destination offset is used as a logical rather
		''' than a physical offset.  The size of the returned <code>Raster</code>
		''' will always be that of the source region clipped to the actual image.
		''' Logical offsets in the stream itself are ignored.
		''' 
		''' <p> This method allows formats that normally apply a color
		''' conversion, such as JPEG, and formats that do not normally have an
		''' associated colorspace, such as remote sensing or medical imaging data,
		''' to provide access to raw pixel data.
		''' 
		''' <p> Any registered <code>readUpdateListener</code>s are ignored, as
		''' there is no <code>BufferedImage</code>, but all other listeners are
		''' called exactly as they are for the <seealso cref="#read read"/> method.
		''' 
		''' <p> If <seealso cref="#canReadRaster canReadRaster()"/> returns
		''' <code>false</code>, this method throws an
		''' <code>UnsupportedOperationException</code>.
		''' 
		''' <p> If the supplied <code>ImageReadParam</code> contains
		''' optional setting values not supported by this reader (<i>e.g.</i>
		''' source render size or any format-specific settings), they will
		''' be ignored.
		''' 
		''' <p> The default implementation throws an
		''' <code>UnsupportedOperationException</code>.
		''' </summary>
		''' <param name="imageIndex"> the index of the image to be read. </param>
		''' <param name="param"> an <code>ImageReadParam</code> used to control
		''' the reading process, or <code>null</code>.
		''' </param>
		''' <returns> the desired portion of the image as a
		''' <code>Raster</code>.
		''' </returns>
		''' <exception cref="UnsupportedOperationException"> if this plug-in does not
		''' support reading raw <code>Raster</code>s. </exception>
		''' <exception cref="IllegalStateException"> if the input source has not been
		''' set. </exception>
		''' <exception cref="IndexOutOfBoundsException"> if the supplied index is
		''' out of bounds. </exception>
		''' <exception cref="IOException"> if an error occurs during reading.
		''' </exception>
		''' <seealso cref= #canReadRaster </seealso>
		''' <seealso cref= #read </seealso>
		''' <seealso cref= java.awt.image.Raster </seealso>
		Public Overridable Function readRaster(ByVal imageIndex As Integer, ByVal param As ImageReadParam) As java.awt.image.Raster
			Throw New System.NotSupportedException("readRaster not supported!")
		End Function

		''' <summary>
		''' Returns <code>true</code> if the image is organized into
		''' <i>tiles</i>, that is, equal-sized non-overlapping rectangles.
		''' 
		''' <p> A reader plug-in may choose whether or not to expose tiling
		''' that is present in the image as it is stored.  It may even
		''' choose to advertise tiling when none is explicitly present.  In
		''' general, tiling should only be advertised if there is some
		''' advantage (in speed or space) to accessing individual tiles.
		''' Regardless of whether the reader advertises tiling, it must be
		''' capable of reading an arbitrary rectangular region specified in
		''' an <code>ImageReadParam</code>.
		''' 
		''' <p> A reader for which all images are guaranteed to be tiled,
		''' or are guaranteed not to be tiled, may return <code>true</code>
		''' or <code>false</code> respectively without accessing any image
		''' data.  In such cases, it is not necessary to throw an exception
		''' even if no input source has been set or the image index is out
		''' of bounds.
		''' 
		''' <p> The default implementation just returns <code>false</code>.
		''' </summary>
		''' <param name="imageIndex"> the index of the image to be queried.
		''' </param>
		''' <returns> <code>true</code> if the image is tiled.
		''' </returns>
		''' <exception cref="IllegalStateException"> if an input source is required
		''' to determine the return value, but none has been set. </exception>
		''' <exception cref="IndexOutOfBoundsException"> if an image must be
		''' accessed to determine the return value, but the supplied index
		''' is out of bounds. </exception>
		''' <exception cref="IOException"> if an error occurs during reading. </exception>
		Public Overridable Function isImageTiled(ByVal imageIndex As Integer) As Boolean
			Return False
		End Function

		''' <summary>
		''' Returns the width of a tile in the given image.
		''' 
		''' <p> The default implementation simply returns
		''' <code>getWidth(imageIndex)</code>, which is correct for
		''' non-tiled images.  Readers that support tiling should override
		''' this method.
		''' </summary>
		''' <returns> the width of a tile.
		''' </returns>
		''' <param name="imageIndex"> the index of the image to be queried.
		''' </param>
		''' <exception cref="IllegalStateException"> if the input source has not been set. </exception>
		''' <exception cref="IndexOutOfBoundsException"> if the supplied index is
		''' out of bounds. </exception>
		''' <exception cref="IOException"> if an error occurs during reading. </exception>
		Public Overridable Function getTileWidth(ByVal imageIndex As Integer) As Integer
			Return getWidth(imageIndex)
		End Function

		''' <summary>
		''' Returns the height of a tile in the given image.
		''' 
		''' <p> The default implementation simply returns
		''' <code>getHeight(imageIndex)</code>, which is correct for
		''' non-tiled images.  Readers that support tiling should override
		''' this method.
		''' </summary>
		''' <returns> the height of a tile.
		''' </returns>
		''' <param name="imageIndex"> the index of the image to be queried.
		''' </param>
		''' <exception cref="IllegalStateException"> if the input source has not been set. </exception>
		''' <exception cref="IndexOutOfBoundsException"> if the supplied index is
		''' out of bounds. </exception>
		''' <exception cref="IOException"> if an error occurs during reading. </exception>
		Public Overridable Function getTileHeight(ByVal imageIndex As Integer) As Integer
			Return getHeight(imageIndex)
		End Function

		''' <summary>
		''' Returns the X coordinate of the upper-left corner of tile (0,
		''' 0) in the given image.
		''' 
		''' <p> A reader for which the tile grid X offset always has the
		''' same value (usually 0), may return the value without accessing
		''' any image data.  In such cases, it is not necessary to throw an
		''' exception even if no input source has been set or the image
		''' index is out of bounds.
		''' 
		''' <p> The default implementation simply returns 0, which is
		''' correct for non-tiled images and tiled images in most formats.
		''' Readers that support tiling with non-(0, 0) offsets should
		''' override this method.
		''' </summary>
		''' <returns> the X offset of the tile grid.
		''' </returns>
		''' <param name="imageIndex"> the index of the image to be queried.
		''' </param>
		''' <exception cref="IllegalStateException"> if an input source is required
		''' to determine the return value, but none has been set. </exception>
		''' <exception cref="IndexOutOfBoundsException"> if an image must be
		''' accessed to determine the return value, but the supplied index
		''' is out of bounds. </exception>
		''' <exception cref="IOException"> if an error occurs during reading. </exception>
		Public Overridable Function getTileGridXOffset(ByVal imageIndex As Integer) As Integer
			Return 0
		End Function

		''' <summary>
		''' Returns the Y coordinate of the upper-left corner of tile (0,
		''' 0) in the given image.
		''' 
		''' <p> A reader for which the tile grid Y offset always has the
		''' same value (usually 0), may return the value without accessing
		''' any image data.  In such cases, it is not necessary to throw an
		''' exception even if no input source has been set or the image
		''' index is out of bounds.
		''' 
		''' <p> The default implementation simply returns 0, which is
		''' correct for non-tiled images and tiled images in most formats.
		''' Readers that support tiling with non-(0, 0) offsets should
		''' override this method.
		''' </summary>
		''' <returns> the Y offset of the tile grid.
		''' </returns>
		''' <param name="imageIndex"> the index of the image to be queried.
		''' </param>
		''' <exception cref="IllegalStateException"> if an input source is required
		''' to determine the return value, but none has been set. </exception>
		''' <exception cref="IndexOutOfBoundsException"> if an image must be
		''' accessed to determine the return value, but the supplied index
		''' is out of bounds. </exception>
		''' <exception cref="IOException"> if an error occurs during reading. </exception>
		Public Overridable Function getTileGridYOffset(ByVal imageIndex As Integer) As Integer
			Return 0
		End Function

		''' <summary>
		''' Reads the tile indicated by the <code>tileX</code> and
		''' <code>tileY</code> arguments, returning it as a
		''' <code>BufferedImage</code>.  If the arguments are out of range,
		''' an <code>IllegalArgumentException</code> is thrown.  If the
		''' image is not tiled, the values 0, 0 will return the entire
		''' image; any other values will cause an
		''' <code>IllegalArgumentException</code> to be thrown.
		''' 
		''' <p> This method is merely a convenience equivalent to calling
		''' <code>read(int, ImageReadParam)</code> with a read param
		''' specifying a source region having offsets of
		''' <code>tileX*getTileWidth(imageIndex)</code>,
		''' <code>tileY*getTileHeight(imageIndex)</code> and width and
		''' height of <code>getTileWidth(imageIndex)</code>,
		''' <code>getTileHeight(imageIndex)</code>; and subsampling
		''' factors of 1 and offsets of 0.  To subsample a tile, call
		''' <code>read</code> with a read param specifying this region
		''' and different subsampling parameters.
		''' 
		''' <p> The default implementation returns the entire image if
		''' <code>tileX</code> and <code>tileY</code> are 0, or throws
		''' an <code>IllegalArgumentException</code> otherwise.
		''' </summary>
		''' <param name="imageIndex"> the index of the image to be retrieved. </param>
		''' <param name="tileX"> the column index (starting with 0) of the tile
		''' to be retrieved. </param>
		''' <param name="tileY"> the row index (starting with 0) of the tile
		''' to be retrieved.
		''' </param>
		''' <returns> the tile as a <code>BufferedImage</code>.
		''' </returns>
		''' <exception cref="IllegalStateException"> if the input source has not been
		''' set. </exception>
		''' <exception cref="IndexOutOfBoundsException"> if <code>imageIndex</code>
		''' is out of bounds. </exception>
		''' <exception cref="IllegalArgumentException"> if the tile indices are
		''' out of bounds. </exception>
		''' <exception cref="IOException"> if an error occurs during reading. </exception>
		Public Overridable Function readTile(ByVal imageIndex As Integer, ByVal tileX As Integer, ByVal tileY As Integer) As java.awt.image.BufferedImage
			If (tileX <> 0) OrElse (tileY <> 0) Then Throw New System.ArgumentException("Invalid tile indices")
			Return read(imageIndex)
		End Function

		''' <summary>
		''' Returns a new <code>Raster</code> object containing the raw
		''' pixel data from the tile, without any color conversion applied.
		''' The application must determine how to interpret the pixel data by other
		''' means.
		''' 
		''' <p> If <seealso cref="#canReadRaster canReadRaster()"/> returns
		''' <code>false</code>, this method throws an
		''' <code>UnsupportedOperationException</code>.
		''' 
		''' <p> The default implementation checks if reading
		''' <code>Raster</code>s is supported, and if so calls {@link
		''' #readRaster readRaster(imageIndex, null)} if
		''' <code>tileX</code> and <code>tileY</code> are 0, or throws an
		''' <code>IllegalArgumentException</code> otherwise.
		''' </summary>
		''' <param name="imageIndex"> the index of the image to be retrieved. </param>
		''' <param name="tileX"> the column index (starting with 0) of the tile
		''' to be retrieved. </param>
		''' <param name="tileY"> the row index (starting with 0) of the tile
		''' to be retrieved.
		''' </param>
		''' <returns> the tile as a <code>Raster</code>.
		''' </returns>
		''' <exception cref="UnsupportedOperationException"> if this plug-in does not
		''' support reading raw <code>Raster</code>s. </exception>
		''' <exception cref="IllegalArgumentException"> if the tile indices are
		''' out of bounds. </exception>
		''' <exception cref="IllegalStateException"> if the input source has not been
		''' set. </exception>
		''' <exception cref="IndexOutOfBoundsException"> if <code>imageIndex</code>
		''' is out of bounds. </exception>
		''' <exception cref="IOException"> if an error occurs during reading.
		''' </exception>
		''' <seealso cref= #readTile </seealso>
		''' <seealso cref= #readRaster </seealso>
		''' <seealso cref= java.awt.image.Raster </seealso>
		Public Overridable Function readTileRaster(ByVal imageIndex As Integer, ByVal tileX As Integer, ByVal tileY As Integer) As java.awt.image.Raster
			If Not canReadRaster() Then Throw New System.NotSupportedException("readTileRaster not supported!")
			If (tileX <> 0) OrElse (tileY <> 0) Then Throw New System.ArgumentException("Invalid tile indices")
			Return readRaster(imageIndex, Nothing)
		End Function

		' RenderedImages

		''' <summary>
		''' Returns a <code>RenderedImage</code> object that contains the
		''' contents of the image indexed by <code>imageIndex</code>.  By
		''' default, the returned image is simply the
		''' <code>BufferedImage</code> returned by <code>read(imageIndex,
		''' param)</code>.
		''' 
		''' <p> The semantics of this method may differ from those of the
		''' other <code>read</code> methods in several ways.  First, any
		''' destination image and/or image type set in the
		''' <code>ImageReadParam</code> may be ignored.  Second, the usual
		''' listener calls are not guaranteed to be made, or to be
		''' meaningful if they are.  This is because the returned image may
		''' not be fully populated with pixel data at the time it is
		''' returned, or indeed at any time.
		''' 
		''' <p> If the supplied <code>ImageReadParam</code> contains
		''' optional setting values not supported by this reader (<i>e.g.</i>
		''' source render size or any format-specific settings), they will
		''' be ignored.
		''' 
		''' <p> The default implementation just calls
		''' <seealso cref="#read read(imageIndex, param)"/>.
		''' </summary>
		''' <param name="imageIndex"> the index of the image to be retrieved. </param>
		''' <param name="param"> an <code>ImageReadParam</code> used to control
		''' the reading process, or <code>null</code>.
		''' </param>
		''' <returns> a <code>RenderedImage</code> object providing a view of
		''' the image.
		''' </returns>
		''' <exception cref="IllegalStateException"> if the input source has not been
		''' set. </exception>
		''' <exception cref="IndexOutOfBoundsException"> if the supplied index is
		''' out of bounds. </exception>
		''' <exception cref="IllegalArgumentException"> if the set of source and
		''' destination bands specified by
		''' <code>param.getSourceBands</code> and
		''' <code>param.getDestinationBands</code> differ in length or
		''' include indices that are out of bounds. </exception>
		''' <exception cref="IllegalArgumentException"> if the resulting image
		''' would have a width or height less than 1. </exception>
		''' <exception cref="IOException"> if an error occurs during reading. </exception>
		Public Overridable Function readAsRenderedImage(ByVal imageIndex As Integer, ByVal param As ImageReadParam) As java.awt.image.RenderedImage
			Return read(imageIndex, param)
		End Function

		' Thumbnails

		''' <summary>
		''' Returns <code>true</code> if the image format understood by
		''' this reader supports thumbnail preview images associated with
		''' it.  The default implementation returns <code>false</code>.
		''' 
		''' <p> If this method returns <code>false</code>,
		''' <code>hasThumbnails</code> and <code>getNumThumbnails</code>
		''' will return <code>false</code> and <code>0</code>,
		''' respectively, and <code>readThumbnail</code> will throw an
		''' <code>UnsupportedOperationException</code>, regardless of their
		''' arguments.
		''' 
		''' <p> A reader that does not support thumbnails need not
		''' implement any of the thumbnail-related methods.
		''' </summary>
		''' <returns> <code>true</code> if thumbnails are supported. </returns>
		Public Overridable Function readerSupportsThumbnails() As Boolean
			Return False
		End Function

		''' <summary>
		''' Returns <code>true</code> if the given image has thumbnail
		''' preview images associated with it.  If the format does not
		''' support thumbnails (<code>readerSupportsThumbnails</code>
		''' returns <code>false</code>), <code>false</code> will be
		''' returned regardless of whether an input source has been set or
		''' whether <code>imageIndex</code> is in bounds.
		''' 
		''' <p> The default implementation returns <code>true</code> if
		''' <code>getNumThumbnails</code> returns a value greater than 0.
		''' </summary>
		''' <param name="imageIndex"> the index of the image being queried.
		''' </param>
		''' <returns> <code>true</code> if the given image has thumbnails.
		''' </returns>
		''' <exception cref="IllegalStateException"> if the reader supports
		''' thumbnails but the input source has not been set. </exception>
		''' <exception cref="IndexOutOfBoundsException"> if the reader supports
		''' thumbnails but <code>imageIndex</code> is out of bounds. </exception>
		''' <exception cref="IOException"> if an error occurs during reading. </exception>
		Public Overridable Function hasThumbnails(ByVal imageIndex As Integer) As Boolean
			Return getNumThumbnails(imageIndex) > 0
		End Function

		''' <summary>
		''' Returns the number of thumbnail preview images associated with
		''' the given image.  If the format does not support thumbnails,
		''' (<code>readerSupportsThumbnails</code> returns
		''' <code>false</code>), <code>0</code> will be returned regardless
		''' of whether an input source has been set or whether
		''' <code>imageIndex</code> is in bounds.
		''' 
		''' <p> The default implementation returns 0 without checking its
		''' argument.
		''' </summary>
		''' <param name="imageIndex"> the index of the image being queried.
		''' </param>
		''' <returns> the number of thumbnails associated with the given
		''' image.
		''' </returns>
		''' <exception cref="IllegalStateException"> if the reader supports
		''' thumbnails but the input source has not been set. </exception>
		''' <exception cref="IndexOutOfBoundsException"> if the reader supports
		''' thumbnails but <code>imageIndex</code> is out of bounds. </exception>
		''' <exception cref="IOException"> if an error occurs during reading. </exception>
		Public Overridable Function getNumThumbnails(ByVal imageIndex As Integer) As Integer
			Return 0
		End Function

		''' <summary>
		''' Returns the width of the thumbnail preview image indexed by
		''' <code>thumbnailIndex</code>, associated with the image indexed
		''' by <code>ImageIndex</code>.
		''' 
		''' <p> If the reader does not support thumbnails,
		''' (<code>readerSupportsThumbnails</code> returns
		''' <code>false</code>), an <code>UnsupportedOperationException</code>
		''' will be thrown.
		''' 
		''' <p> The default implementation simply returns
		''' <code>readThumbnail(imageindex,
		''' thumbnailIndex).getWidth()</code>.  Subclasses should therefore
		''' override this method if possible in order to avoid forcing the
		''' thumbnail to be read.
		''' </summary>
		''' <param name="imageIndex"> the index of the image to be retrieved. </param>
		''' <param name="thumbnailIndex"> the index of the thumbnail to be retrieved.
		''' </param>
		''' <returns> the width of the desired thumbnail as an <code>int</code>.
		''' </returns>
		''' <exception cref="UnsupportedOperationException"> if thumbnails are not
		''' supported. </exception>
		''' <exception cref="IllegalStateException"> if the input source has not been set. </exception>
		''' <exception cref="IndexOutOfBoundsException"> if either of the supplied
		''' indices are out of bounds. </exception>
		''' <exception cref="IOException"> if an error occurs during reading. </exception>
		Public Overridable Function getThumbnailWidth(ByVal imageIndex As Integer, ByVal thumbnailIndex As Integer) As Integer
			Return readThumbnail(imageIndex, thumbnailIndex).width
		End Function

		''' <summary>
		''' Returns the height of the thumbnail preview image indexed by
		''' <code>thumbnailIndex</code>, associated with the image indexed
		''' by <code>ImageIndex</code>.
		''' 
		''' <p> If the reader does not support thumbnails,
		''' (<code>readerSupportsThumbnails</code> returns
		''' <code>false</code>), an <code>UnsupportedOperationException</code>
		''' will be thrown.
		''' 
		''' <p> The default implementation simply returns
		''' <code>readThumbnail(imageindex,
		''' thumbnailIndex).getHeight()</code>.  Subclasses should
		''' therefore override this method if possible in order to avoid
		''' forcing the thumbnail to be read.
		''' </summary>
		''' <param name="imageIndex"> the index of the image to be retrieved. </param>
		''' <param name="thumbnailIndex"> the index of the thumbnail to be retrieved.
		''' </param>
		''' <returns> the height of the desired thumbnail as an <code>int</code>.
		''' </returns>
		''' <exception cref="UnsupportedOperationException"> if thumbnails are not
		''' supported. </exception>
		''' <exception cref="IllegalStateException"> if the input source has not been set. </exception>
		''' <exception cref="IndexOutOfBoundsException"> if either of the supplied
		''' indices are out of bounds. </exception>
		''' <exception cref="IOException"> if an error occurs during reading. </exception>
		Public Overridable Function getThumbnailHeight(ByVal imageIndex As Integer, ByVal thumbnailIndex As Integer) As Integer
			Return readThumbnail(imageIndex, thumbnailIndex).height
		End Function

		''' <summary>
		''' Returns the thumbnail preview image indexed by
		''' <code>thumbnailIndex</code>, associated with the image indexed
		''' by <code>ImageIndex</code> as a <code>BufferedImage</code>.
		''' 
		''' <p> Any registered <code>IIOReadProgressListener</code> objects
		''' will be notified by calling their
		''' <code>thumbnailStarted</code>, <code>thumbnailProgress</code>,
		''' and <code>thumbnailComplete</code> methods.
		''' 
		''' <p> If the reader does not support thumbnails,
		''' (<code>readerSupportsThumbnails</code> returns
		''' <code>false</code>), an <code>UnsupportedOperationException</code>
		''' will be thrown regardless of whether an input source has been
		''' set or whether the indices are in bounds.
		''' 
		''' <p> The default implementation throws an
		''' <code>UnsupportedOperationException</code>.
		''' </summary>
		''' <param name="imageIndex"> the index of the image to be retrieved. </param>
		''' <param name="thumbnailIndex"> the index of the thumbnail to be retrieved.
		''' </param>
		''' <returns> the desired thumbnail as a <code>BufferedImage</code>.
		''' </returns>
		''' <exception cref="UnsupportedOperationException"> if thumbnails are not
		''' supported. </exception>
		''' <exception cref="IllegalStateException"> if the input source has not been set. </exception>
		''' <exception cref="IndexOutOfBoundsException"> if either of the supplied
		''' indices are out of bounds. </exception>
		''' <exception cref="IOException"> if an error occurs during reading. </exception>
		Public Overridable Function readThumbnail(ByVal imageIndex As Integer, ByVal thumbnailIndex As Integer) As java.awt.image.BufferedImage
			Throw New System.NotSupportedException("Thumbnails not supported!")
		End Function

		' Abort

		''' <summary>
		''' Requests that any current read operation be aborted.  The
		''' contents of the image following the abort will be undefined.
		''' 
		''' <p> Readers should call <code>clearAbortRequest</code> at the
		''' beginning of each read operation, and poll the value of
		''' <code>abortRequested</code> regularly during the read.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub abort()
			Me.abortFlag = True
		End Sub

		''' <summary>
		''' Returns <code>true</code> if a request to abort the current
		''' read operation has been made since the reader was instantiated or
		''' <code>clearAbortRequest</code> was called.
		''' </summary>
		''' <returns> <code>true</code> if the current read operation should
		''' be aborted.
		''' </returns>
		''' <seealso cref= #abort </seealso>
		''' <seealso cref= #clearAbortRequest </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Overridable Function abortRequested() As Boolean
			Return Me.abortFlag
		End Function

		''' <summary>
		''' Clears any previous abort request.  After this method has been
		''' called, <code>abortRequested</code> will return
		''' <code>false</code>.
		''' </summary>
		''' <seealso cref= #abort </seealso>
		''' <seealso cref= #abortRequested </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Protected Friend Overridable Sub clearAbortRequest()
			Me.abortFlag = False
		End Sub

		' Listeners

		' Add an element to a list, creating a new list if the
		' existing list is null, and return the list.
		Friend Shared Function addToList(ByVal l As IList, ByVal elt As Object) As IList
			If l Is Nothing Then l = New ArrayList
			l.Add(elt)
			Return l
		End Function


		' Remove an element from a list, discarding the list if the
		' resulting list is empty, and return the list or null.
		Friend Shared Function removeFromList(ByVal l As IList, ByVal elt As Object) As IList
			If l Is Nothing Then Return l
			l.Remove(elt)
			If l.Count = 0 Then l = Nothing
			Return l
		End Function

		''' <summary>
		''' Adds an <code>IIOReadWarningListener</code> to the list of
		''' registered warning listeners.  If <code>listener</code> is
		''' <code>null</code>, no exception will be thrown and no action
		''' will be taken.  Messages sent to the given listener will be
		''' localized, if possible, to match the current
		''' <code>Locale</code>.  If no <code>Locale</code> has been set,
		''' warning messages may be localized as the reader sees fit.
		''' </summary>
		''' <param name="listener"> an <code>IIOReadWarningListener</code> to be registered.
		''' </param>
		''' <seealso cref= #removeIIOReadWarningListener </seealso>
		Public Overridable Sub addIIOReadWarningListener(ByVal listener As javax.imageio.event.IIOReadWarningListener)
			If listener Is Nothing Then Return
			warningListeners = addToList(warningListeners, listener)
			warningLocales = addToList(warningLocales, locale)
		End Sub

		''' <summary>
		''' Removes an <code>IIOReadWarningListener</code> from the list of
		''' registered error listeners.  If the listener was not previously
		''' registered, or if <code>listener</code> is <code>null</code>,
		''' no exception will be thrown and no action will be taken.
		''' </summary>
		''' <param name="listener"> an IIOReadWarningListener to be unregistered.
		''' </param>
		''' <seealso cref= #addIIOReadWarningListener </seealso>
		Public Overridable Sub removeIIOReadWarningListener(ByVal listener As javax.imageio.event.IIOReadWarningListener)
			If listener Is Nothing OrElse warningListeners Is Nothing Then Return
			Dim index As Integer = warningListeners.IndexOf(listener)
			If index <> -1 Then
				warningListeners.RemoveAt(index)
				warningLocales.RemoveAt(index)
				If warningListeners.Count = 0 Then
					warningListeners = Nothing
					warningLocales = Nothing
				End If
			End If
		End Sub

		''' <summary>
		''' Removes all currently registered
		''' <code>IIOReadWarningListener</code> objects.
		''' 
		''' <p> The default implementation sets the
		''' <code>warningListeners</code> and <code>warningLocales</code>
		''' instance variables to <code>null</code>.
		''' </summary>
		Public Overridable Sub removeAllIIOReadWarningListeners()
			warningListeners = Nothing
			warningLocales = Nothing
		End Sub

		''' <summary>
		''' Adds an <code>IIOReadProgressListener</code> to the list of
		''' registered progress listeners.  If <code>listener</code> is
		''' <code>null</code>, no exception will be thrown and no action
		''' will be taken.
		''' </summary>
		''' <param name="listener"> an IIOReadProgressListener to be registered.
		''' </param>
		''' <seealso cref= #removeIIOReadProgressListener </seealso>
		Public Overridable Sub addIIOReadProgressListener(ByVal listener As javax.imageio.event.IIOReadProgressListener)
			If listener Is Nothing Then Return
			progressListeners = addToList(progressListeners, listener)
		End Sub

		''' <summary>
		''' Removes an <code>IIOReadProgressListener</code> from the list
		''' of registered progress listeners.  If the listener was not
		''' previously registered, or if <code>listener</code> is
		''' <code>null</code>, no exception will be thrown and no action
		''' will be taken.
		''' </summary>
		''' <param name="listener"> an IIOReadProgressListener to be unregistered.
		''' </param>
		''' <seealso cref= #addIIOReadProgressListener </seealso>
		Public Overridable Sub removeIIOReadProgressListener(ByVal listener As javax.imageio.event.IIOReadProgressListener)
			If listener Is Nothing OrElse progressListeners Is Nothing Then Return
			progressListeners = removeFromList(progressListeners, listener)
		End Sub

		''' <summary>
		''' Removes all currently registered
		''' <code>IIOReadProgressListener</code> objects.
		''' 
		''' <p> The default implementation sets the
		''' <code>progressListeners</code> instance variable to
		''' <code>null</code>.
		''' </summary>
		Public Overridable Sub removeAllIIOReadProgressListeners()
			progressListeners = Nothing
		End Sub

		''' <summary>
		''' Adds an <code>IIOReadUpdateListener</code> to the list of
		''' registered update listeners.  If <code>listener</code> is
		''' <code>null</code>, no exception will be thrown and no action
		''' will be taken.  The listener will receive notification of pixel
		''' updates as images and thumbnails are decoded, including the
		''' starts and ends of progressive passes.
		''' 
		''' <p> If no update listeners are present, the reader may choose
		''' to perform fewer updates to the pixels of the destination
		''' images and/or thumbnails, which may result in more efficient
		''' decoding.
		''' 
		''' <p> For example, in progressive JPEG decoding each pass
		''' contains updates to a set of coefficients, which would have to
		''' be transformed into pixel values and converted to an RGB color
		''' space for each pass if listeners are present.  If no listeners
		''' are present, the coefficients may simply be accumulated and the
		''' final results transformed and color converted one time only.
		''' 
		''' <p> The final results of decoding will be the same whether or
		''' not intermediate updates are performed.  Thus if only the final
		''' image is desired it may be preferable not to register any
		''' <code>IIOReadUpdateListener</code>s.  In general, progressive
		''' updating is most effective when fetching images over a network
		''' connection that is very slow compared to local CPU processing;
		''' over a fast connection, progressive updates may actually slow
		''' down the presentation of the image.
		''' </summary>
		''' <param name="listener"> an IIOReadUpdateListener to be registered.
		''' </param>
		''' <seealso cref= #removeIIOReadUpdateListener </seealso>
		Public Overridable Sub addIIOReadUpdateListener(ByVal listener As javax.imageio.event.IIOReadUpdateListener)
			If listener Is Nothing Then Return
			updateListeners = addToList(updateListeners, listener)
		End Sub

		''' <summary>
		''' Removes an <code>IIOReadUpdateListener</code> from the list of
		''' registered update listeners.  If the listener was not
		''' previously registered, or if <code>listener</code> is
		''' <code>null</code>, no exception will be thrown and no action
		''' will be taken.
		''' </summary>
		''' <param name="listener"> an IIOReadUpdateListener to be unregistered.
		''' </param>
		''' <seealso cref= #addIIOReadUpdateListener </seealso>
		Public Overridable Sub removeIIOReadUpdateListener(ByVal listener As javax.imageio.event.IIOReadUpdateListener)
			If listener Is Nothing OrElse updateListeners Is Nothing Then Return
			updateListeners = removeFromList(updateListeners, listener)
		End Sub

		''' <summary>
		''' Removes all currently registered
		''' <code>IIOReadUpdateListener</code> objects.
		''' 
		''' <p> The default implementation sets the
		''' <code>updateListeners</code> instance variable to
		''' <code>null</code>.
		''' </summary>
		Public Overridable Sub removeAllIIOReadUpdateListeners()
			updateListeners = Nothing
		End Sub

		''' <summary>
		''' Broadcasts the start of an sequence of image reads to all
		''' registered <code>IIOReadProgressListener</code>s by calling
		''' their <code>sequenceStarted</code> method.  Subclasses may use
		''' this method as a convenience.
		''' </summary>
		''' <param name="minIndex"> the lowest index being read. </param>
		Protected Friend Overridable Sub processSequenceStarted(ByVal minIndex As Integer)
			If progressListeners Is Nothing Then Return
			Dim numListeners As Integer = progressListeners.Count
			For i As Integer = 0 To numListeners - 1
				Dim listener As javax.imageio.event.IIOReadProgressListener = CType(progressListeners(i), javax.imageio.event.IIOReadProgressListener)
				listener.sequenceStarted(Me, minIndex)
			Next i
		End Sub

		''' <summary>
		''' Broadcasts the completion of an sequence of image reads to all
		''' registered <code>IIOReadProgressListener</code>s by calling
		''' their <code>sequenceComplete</code> method.  Subclasses may use
		''' this method as a convenience.
		''' </summary>
		Protected Friend Overridable Sub processSequenceComplete()
			If progressListeners Is Nothing Then Return
			Dim numListeners As Integer = progressListeners.Count
			For i As Integer = 0 To numListeners - 1
				Dim listener As javax.imageio.event.IIOReadProgressListener = CType(progressListeners(i), javax.imageio.event.IIOReadProgressListener)
				listener.sequenceComplete(Me)
			Next i
		End Sub

		''' <summary>
		''' Broadcasts the start of an image read to all registered
		''' <code>IIOReadProgressListener</code>s by calling their
		''' <code>imageStarted</code> method.  Subclasses may use this
		''' method as a convenience.
		''' </summary>
		''' <param name="imageIndex"> the index of the image about to be read. </param>
		Protected Friend Overridable Sub processImageStarted(ByVal imageIndex As Integer)
			If progressListeners Is Nothing Then Return
			Dim numListeners As Integer = progressListeners.Count
			For i As Integer = 0 To numListeners - 1
				Dim listener As javax.imageio.event.IIOReadProgressListener = CType(progressListeners(i), javax.imageio.event.IIOReadProgressListener)
				listener.imageStarted(Me, imageIndex)
			Next i
		End Sub

		''' <summary>
		''' Broadcasts the current percentage of image completion to all
		''' registered <code>IIOReadProgressListener</code>s by calling
		''' their <code>imageProgress</code> method.  Subclasses may use
		''' this method as a convenience.
		''' </summary>
		''' <param name="percentageDone"> the current percentage of completion,
		''' as a <code>float</code>. </param>
		Protected Friend Overridable Sub processImageProgress(ByVal percentageDone As Single)
			If progressListeners Is Nothing Then Return
			Dim numListeners As Integer = progressListeners.Count
			For i As Integer = 0 To numListeners - 1
				Dim listener As javax.imageio.event.IIOReadProgressListener = CType(progressListeners(i), javax.imageio.event.IIOReadProgressListener)
				listener.imageProgress(Me, percentageDone)
			Next i
		End Sub

		''' <summary>
		''' Broadcasts the completion of an image read to all registered
		''' <code>IIOReadProgressListener</code>s by calling their
		''' <code>imageComplete</code> method.  Subclasses may use this
		''' method as a convenience.
		''' </summary>
		Protected Friend Overridable Sub processImageComplete()
			If progressListeners Is Nothing Then Return
			Dim numListeners As Integer = progressListeners.Count
			For i As Integer = 0 To numListeners - 1
				Dim listener As javax.imageio.event.IIOReadProgressListener = CType(progressListeners(i), javax.imageio.event.IIOReadProgressListener)
				listener.imageComplete(Me)
			Next i
		End Sub

		''' <summary>
		''' Broadcasts the start of a thumbnail read to all registered
		''' <code>IIOReadProgressListener</code>s by calling their
		''' <code>thumbnailStarted</code> method.  Subclasses may use this
		''' method as a convenience.
		''' </summary>
		''' <param name="imageIndex"> the index of the image associated with the
		''' thumbnail. </param>
		''' <param name="thumbnailIndex"> the index of the thumbnail. </param>
		Protected Friend Overridable Sub processThumbnailStarted(ByVal imageIndex As Integer, ByVal thumbnailIndex As Integer)
			If progressListeners Is Nothing Then Return
			Dim numListeners As Integer = progressListeners.Count
			For i As Integer = 0 To numListeners - 1
				Dim listener As javax.imageio.event.IIOReadProgressListener = CType(progressListeners(i), javax.imageio.event.IIOReadProgressListener)
				listener.thumbnailStarted(Me, imageIndex, thumbnailIndex)
			Next i
		End Sub

		''' <summary>
		''' Broadcasts the current percentage of thumbnail completion to
		''' all registered <code>IIOReadProgressListener</code>s by calling
		''' their <code>thumbnailProgress</code> method.  Subclasses may
		''' use this method as a convenience.
		''' </summary>
		''' <param name="percentageDone"> the current percentage of completion,
		''' as a <code>float</code>. </param>
		Protected Friend Overridable Sub processThumbnailProgress(ByVal percentageDone As Single)
			If progressListeners Is Nothing Then Return
			Dim numListeners As Integer = progressListeners.Count
			For i As Integer = 0 To numListeners - 1
				Dim listener As javax.imageio.event.IIOReadProgressListener = CType(progressListeners(i), javax.imageio.event.IIOReadProgressListener)
				listener.thumbnailProgress(Me, percentageDone)
			Next i
		End Sub

		''' <summary>
		''' Broadcasts the completion of a thumbnail read to all registered
		''' <code>IIOReadProgressListener</code>s by calling their
		''' <code>thumbnailComplete</code> method.  Subclasses may use this
		''' method as a convenience.
		''' </summary>
		Protected Friend Overridable Sub processThumbnailComplete()
			If progressListeners Is Nothing Then Return
			Dim numListeners As Integer = progressListeners.Count
			For i As Integer = 0 To numListeners - 1
				Dim listener As javax.imageio.event.IIOReadProgressListener = CType(progressListeners(i), javax.imageio.event.IIOReadProgressListener)
				listener.thumbnailComplete(Me)
			Next i
		End Sub

		''' <summary>
		''' Broadcasts that the read has been aborted to all registered
		''' <code>IIOReadProgressListener</code>s by calling their
		''' <code>readAborted</code> method.  Subclasses may use this
		''' method as a convenience.
		''' </summary>
		Protected Friend Overridable Sub processReadAborted()
			If progressListeners Is Nothing Then Return
			Dim numListeners As Integer = progressListeners.Count
			For i As Integer = 0 To numListeners - 1
				Dim listener As javax.imageio.event.IIOReadProgressListener = CType(progressListeners(i), javax.imageio.event.IIOReadProgressListener)
				listener.readAborted(Me)
			Next i
		End Sub

		''' <summary>
		''' Broadcasts the beginning of a progressive pass to all
		''' registered <code>IIOReadUpdateListener</code>s by calling their
		''' <code>passStarted</code> method.  Subclasses may use this
		''' method as a convenience.
		''' </summary>
		''' <param name="theImage"> the <code>BufferedImage</code> being updated. </param>
		''' <param name="pass"> the index of the current pass, starting with 0. </param>
		''' <param name="minPass"> the index of the first pass that will be decoded. </param>
		''' <param name="maxPass"> the index of the last pass that will be decoded. </param>
		''' <param name="minX"> the X coordinate of the upper-left pixel included
		''' in the pass. </param>
		''' <param name="minY"> the X coordinate of the upper-left pixel included
		''' in the pass. </param>
		''' <param name="periodX"> the horizontal separation between pixels. </param>
		''' <param name="periodY"> the vertical separation between pixels. </param>
		''' <param name="bands"> an array of <code>int</code>s indicating the
		''' set of affected bands of the destination. </param>
		Protected Friend Overridable Sub processPassStarted(ByVal theImage As java.awt.image.BufferedImage, ByVal pass As Integer, ByVal minPass As Integer, ByVal maxPass As Integer, ByVal minX As Integer, ByVal minY As Integer, ByVal periodX As Integer, ByVal periodY As Integer, ByVal bands As Integer())
			If updateListeners Is Nothing Then Return
			Dim numListeners As Integer = updateListeners.Count
			For i As Integer = 0 To numListeners - 1
				Dim listener As javax.imageio.event.IIOReadUpdateListener = CType(updateListeners(i), javax.imageio.event.IIOReadUpdateListener)
				listener.passStarted(Me, theImage, pass, minPass, maxPass, minX, minY, periodX, periodY, bands)
			Next i
		End Sub

		''' <summary>
		''' Broadcasts the update of a set of samples to all registered
		''' <code>IIOReadUpdateListener</code>s by calling their
		''' <code>imageUpdate</code> method.  Subclasses may use this
		''' method as a convenience.
		''' </summary>
		''' <param name="theImage"> the <code>BufferedImage</code> being updated. </param>
		''' <param name="minX"> the X coordinate of the upper-left pixel included
		''' in the pass. </param>
		''' <param name="minY"> the X coordinate of the upper-left pixel included
		''' in the pass. </param>
		''' <param name="width"> the total width of the area being updated, including
		''' pixels being skipped if <code>periodX &gt; 1</code>. </param>
		''' <param name="height"> the total height of the area being updated,
		''' including pixels being skipped if <code>periodY &gt; 1</code>. </param>
		''' <param name="periodX"> the horizontal separation between pixels. </param>
		''' <param name="periodY"> the vertical separation between pixels. </param>
		''' <param name="bands"> an array of <code>int</code>s indicating the
		''' set of affected bands of the destination. </param>
		Protected Friend Overridable Sub processImageUpdate(ByVal theImage As java.awt.image.BufferedImage, ByVal minX As Integer, ByVal minY As Integer, ByVal width As Integer, ByVal height As Integer, ByVal periodX As Integer, ByVal periodY As Integer, ByVal bands As Integer())
			If updateListeners Is Nothing Then Return
			Dim numListeners As Integer = updateListeners.Count
			For i As Integer = 0 To numListeners - 1
				Dim listener As javax.imageio.event.IIOReadUpdateListener = CType(updateListeners(i), javax.imageio.event.IIOReadUpdateListener)
				listener.imageUpdate(Me, theImage, minX, minY, width, height, periodX, periodY, bands)
			Next i
		End Sub

		''' <summary>
		''' Broadcasts the end of a progressive pass to all
		''' registered <code>IIOReadUpdateListener</code>s by calling their
		''' <code>passComplete</code> method.  Subclasses may use this
		''' method as a convenience.
		''' </summary>
		''' <param name="theImage"> the <code>BufferedImage</code> being updated. </param>
		Protected Friend Overridable Sub processPassComplete(ByVal theImage As java.awt.image.BufferedImage)
			If updateListeners Is Nothing Then Return
			Dim numListeners As Integer = updateListeners.Count
			For i As Integer = 0 To numListeners - 1
				Dim listener As javax.imageio.event.IIOReadUpdateListener = CType(updateListeners(i), javax.imageio.event.IIOReadUpdateListener)
				listener.passComplete(Me, theImage)
			Next i
		End Sub

		''' <summary>
		''' Broadcasts the beginning of a thumbnail progressive pass to all
		''' registered <code>IIOReadUpdateListener</code>s by calling their
		''' <code>thumbnailPassStarted</code> method.  Subclasses may use this
		''' method as a convenience.
		''' </summary>
		''' <param name="theThumbnail"> the <code>BufferedImage</code> thumbnail
		''' being updated. </param>
		''' <param name="pass"> the index of the current pass, starting with 0. </param>
		''' <param name="minPass"> the index of the first pass that will be decoded. </param>
		''' <param name="maxPass"> the index of the last pass that will be decoded. </param>
		''' <param name="minX"> the X coordinate of the upper-left pixel included
		''' in the pass. </param>
		''' <param name="minY"> the X coordinate of the upper-left pixel included
		''' in the pass. </param>
		''' <param name="periodX"> the horizontal separation between pixels. </param>
		''' <param name="periodY"> the vertical separation between pixels. </param>
		''' <param name="bands"> an array of <code>int</code>s indicating the
		''' set of affected bands of the destination. </param>
		Protected Friend Overridable Sub processThumbnailPassStarted(ByVal theThumbnail As java.awt.image.BufferedImage, ByVal pass As Integer, ByVal minPass As Integer, ByVal maxPass As Integer, ByVal minX As Integer, ByVal minY As Integer, ByVal periodX As Integer, ByVal periodY As Integer, ByVal bands As Integer())
			If updateListeners Is Nothing Then Return
			Dim numListeners As Integer = updateListeners.Count
			For i As Integer = 0 To numListeners - 1
				Dim listener As javax.imageio.event.IIOReadUpdateListener = CType(updateListeners(i), javax.imageio.event.IIOReadUpdateListener)
				listener.thumbnailPassStarted(Me, theThumbnail, pass, minPass, maxPass, minX, minY, periodX, periodY, bands)
			Next i
		End Sub

		''' <summary>
		''' Broadcasts the update of a set of samples in a thumbnail image
		''' to all registered <code>IIOReadUpdateListener</code>s by
		''' calling their <code>thumbnailUpdate</code> method.  Subclasses may
		''' use this method as a convenience.
		''' </summary>
		''' <param name="theThumbnail"> the <code>BufferedImage</code> thumbnail
		''' being updated. </param>
		''' <param name="minX"> the X coordinate of the upper-left pixel included
		''' in the pass. </param>
		''' <param name="minY"> the X coordinate of the upper-left pixel included
		''' in the pass. </param>
		''' <param name="width"> the total width of the area being updated, including
		''' pixels being skipped if <code>periodX &gt; 1</code>. </param>
		''' <param name="height"> the total height of the area being updated,
		''' including pixels being skipped if <code>periodY &gt; 1</code>. </param>
		''' <param name="periodX"> the horizontal separation between pixels. </param>
		''' <param name="periodY"> the vertical separation between pixels. </param>
		''' <param name="bands"> an array of <code>int</code>s indicating the
		''' set of affected bands of the destination. </param>
		Protected Friend Overridable Sub processThumbnailUpdate(ByVal theThumbnail As java.awt.image.BufferedImage, ByVal minX As Integer, ByVal minY As Integer, ByVal width As Integer, ByVal height As Integer, ByVal periodX As Integer, ByVal periodY As Integer, ByVal bands As Integer())
			If updateListeners Is Nothing Then Return
			Dim numListeners As Integer = updateListeners.Count
			For i As Integer = 0 To numListeners - 1
				Dim listener As javax.imageio.event.IIOReadUpdateListener = CType(updateListeners(i), javax.imageio.event.IIOReadUpdateListener)
				listener.thumbnailUpdate(Me, theThumbnail, minX, minY, width, height, periodX, periodY, bands)
			Next i
		End Sub

		''' <summary>
		''' Broadcasts the end of a thumbnail progressive pass to all
		''' registered <code>IIOReadUpdateListener</code>s by calling their
		''' <code>thumbnailPassComplete</code> method.  Subclasses may use this
		''' method as a convenience.
		''' </summary>
		''' <param name="theThumbnail"> the <code>BufferedImage</code> thumbnail
		''' being updated. </param>
		Protected Friend Overridable Sub processThumbnailPassComplete(ByVal theThumbnail As java.awt.image.BufferedImage)
			If updateListeners Is Nothing Then Return
			Dim numListeners As Integer = updateListeners.Count
			For i As Integer = 0 To numListeners - 1
				Dim listener As javax.imageio.event.IIOReadUpdateListener = CType(updateListeners(i), javax.imageio.event.IIOReadUpdateListener)
				listener.thumbnailPassComplete(Me, theThumbnail)
			Next i
		End Sub

		''' <summary>
		''' Broadcasts a warning message to all registered
		''' <code>IIOReadWarningListener</code>s by calling their
		''' <code>warningOccurred</code> method.  Subclasses may use this
		''' method as a convenience.
		''' </summary>
		''' <param name="warning"> the warning message to send.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>warning</code>
		''' is <code>null</code>. </exception>
		Protected Friend Overridable Sub processWarningOccurred(ByVal warning As String)
			If warningListeners Is Nothing Then Return
			If warning Is Nothing Then Throw New System.ArgumentException("warning == null!")
			Dim numListeners As Integer = warningListeners.Count
			For i As Integer = 0 To numListeners - 1
				Dim listener As javax.imageio.event.IIOReadWarningListener = CType(warningListeners(i), javax.imageio.event.IIOReadWarningListener)

				listener.warningOccurred(Me, warning)
			Next i
		End Sub

		''' <summary>
		''' Broadcasts a localized warning message to all registered
		''' <code>IIOReadWarningListener</code>s by calling their
		''' <code>warningOccurred</code> method with a string taken
		''' from a <code>ResourceBundle</code>.  Subclasses may use this
		''' method as a convenience.
		''' </summary>
		''' <param name="baseName"> the base name of a set of
		''' <code>ResourceBundle</code>s containing localized warning
		''' messages. </param>
		''' <param name="keyword"> the keyword used to index the warning message
		''' within the set of <code>ResourceBundle</code>s.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>baseName</code>
		''' is <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>keyword</code>
		''' is <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if no appropriate
		''' <code>ResourceBundle</code> may be located. </exception>
		''' <exception cref="IllegalArgumentException"> if the named resource is
		''' not found in the located <code>ResourceBundle</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if the object retrieved
		''' from the <code>ResourceBundle</code> is not a
		''' <code>String</code>. </exception>
		Protected Friend Overridable Sub processWarningOccurred(ByVal baseName As String, ByVal keyword As String)
			If warningListeners Is Nothing Then Return
			If baseName Is Nothing Then Throw New System.ArgumentException("baseName == null!")
			If keyword Is Nothing Then Throw New System.ArgumentException("keyword == null!")
			Dim numListeners As Integer = warningListeners.Count
			For i As Integer = 0 To numListeners - 1
				Dim listener As javax.imageio.event.IIOReadWarningListener = CType(warningListeners(i), javax.imageio.event.IIOReadWarningListener)
				Dim ___locale As java.util.Locale = CType(warningLocales(i), java.util.Locale)
				If ___locale Is Nothing Then ___locale = java.util.Locale.default

				''' <summary>
				''' If an applet supplies an implementation of ImageReader and
				''' resource bundles, then the resource bundle will need to be
				''' accessed via the applet class loader. So first try the context
				''' class loader to locate the resource bundle.
				''' If that throws MissingResourceException, then try the
				''' system class loader.
				''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				ClassLoader loader = (ClassLoader) java.security.AccessController.doPrivileged(New java.security.PrivilegedAction()
	'			{
	'					  public Object run()
	'					  {
	'						Return Thread.currentThread().getContextClassLoader();
	'					  }
	'				});

				Dim bundle As java.util.ResourceBundle = Nothing
				Try
					bundle = java.util.ResourceBundle.getBundle(baseName, ___locale, loader)
				Catch mre As java.util.MissingResourceException
					Try
						bundle = java.util.ResourceBundle.getBundle(baseName, ___locale)
					Catch mre1 As java.util.MissingResourceException
						Throw New System.ArgumentException("Bundle not found!")
					End Try
				End Try

				Dim warning As String = Nothing
				Try
					warning = bundle.getString(keyword)
				Catch cce As ClassCastException
					Throw New System.ArgumentException("Resource is not a String!")
				Catch mre As java.util.MissingResourceException
					Throw New System.ArgumentException("Resource is missing!")
				End Try

				listener.warningOccurred(Me, warning)
			Next i
		End Sub

		' State management

		''' <summary>
		''' Restores the <code>ImageReader</code> to its initial state.
		''' 
		''' <p> The default implementation calls <code>setInput(null,
		''' false)</code>, <code>setLocale(null)</code>,
		''' <code>removeAllIIOReadUpdateListeners()</code>,
		''' <code>removeAllIIOReadWarningListeners()</code>,
		''' <code>removeAllIIOReadProgressListeners()</code>, and
		''' <code>clearAbortRequest</code>.
		''' </summary>
		Public Overridable Sub reset()
			inputput(Nothing, False, False)
			locale = Nothing
			removeAllIIOReadUpdateListeners()
			removeAllIIOReadProgressListeners()
			removeAllIIOReadWarningListeners()
			clearAbortRequest()
		End Sub

		''' <summary>
		''' Allows any resources held by this object to be released.  The
		''' result of calling any other method (other than
		''' <code>finalize</code>) subsequent to a call to this method
		''' is undefined.
		''' 
		''' <p>It is important for applications to call this method when they
		''' know they will no longer be using this <code>ImageReader</code>.
		''' Otherwise, the reader may continue to hold on to resources
		''' indefinitely.
		''' 
		''' <p>The default implementation of this method in the superclass does
		''' nothing.  Subclass implementations should ensure that all resources,
		''' especially native resources, are released.
		''' </summary>
		Public Overridable Sub dispose()
		End Sub

		' Utility methods

		''' <summary>
		''' A utility method that may be used by readers to compute the
		''' region of the source image that should be read, taking into
		''' account any source region and subsampling offset settings in
		''' the supplied <code>ImageReadParam</code>.  The actual
		''' subsampling factors, destination size, and destination offset
		''' are <em>not</em> taken into consideration, thus further
		''' clipping must take place.  The <seealso cref="#computeRegions computeRegions"/>
		''' method performs all necessary clipping.
		''' </summary>
		''' <param name="param"> the <code>ImageReadParam</code> being used, or
		''' <code>null</code>. </param>
		''' <param name="srcWidth"> the width of the source image. </param>
		''' <param name="srcHeight"> the height of the source image.
		''' </param>
		''' <returns> the source region as a <code>Rectangle</code>. </returns>
		Protected Friend Shared Function getSourceRegion(ByVal param As ImageReadParam, ByVal srcWidth As Integer, ByVal srcHeight As Integer) As java.awt.Rectangle
			Dim ___sourceRegion As New java.awt.Rectangle(0, 0, srcWidth, srcHeight)
			If param IsNot Nothing Then
				Dim region As java.awt.Rectangle = param.sourceRegion
				If region IsNot Nothing Then ___sourceRegion = ___sourceRegion.intersection(region)

				Dim subsampleXOffset As Integer = param.subsamplingXOffset
				Dim subsampleYOffset As Integer = param.subsamplingYOffset
				___sourceRegion.x += subsampleXOffset
				___sourceRegion.y += subsampleYOffset
				___sourceRegion.width -= subsampleXOffset
				___sourceRegion.height -= subsampleYOffset
			End If

			Return ___sourceRegion
		End Function

		''' <summary>
		''' Computes the source region of interest and the destination
		''' region of interest, taking the width and height of the source
		''' image, an optional destination image, and an optional
		''' <code>ImageReadParam</code> into account.  The source region
		''' begins with the entire source image.  Then that is clipped to
		''' the source region specified in the <code>ImageReadParam</code>,
		''' if one is specified.
		''' 
		''' <p> If either of the destination offsets are negative, the
		''' source region is clipped so that its top left will coincide
		''' with the top left of the destination image, taking subsampling
		''' into account.  Then the result is clipped to the destination
		''' image on the right and bottom, if one is specified, taking
		''' subsampling and destination offsets into account.
		''' 
		''' <p> Similarly, the destination region begins with the source
		''' image, is translated to the destination offset given in the
		''' <code>ImageReadParam</code> if there is one, and finally is
		''' clipped to the destination image, if there is one.
		''' 
		''' <p> If either the source or destination regions end up having a
		''' width or height of 0, an <code>IllegalArgumentException</code>
		''' is thrown.
		''' 
		''' <p> The <seealso cref="#getSourceRegion getSourceRegion>"/>
		''' method may be used if only source clipping is desired.
		''' </summary>
		''' <param name="param"> an <code>ImageReadParam</code>, or <code>null</code>. </param>
		''' <param name="srcWidth"> the width of the source image. </param>
		''' <param name="srcHeight"> the height of the source image. </param>
		''' <param name="image"> a <code>BufferedImage</code> that will be the
		''' destination image, or <code>null</code>. </param>
		''' <param name="srcRegion"> a <code>Rectangle</code> that will be filled with
		''' the source region of interest. </param>
		''' <param name="destRegion"> a <code>Rectangle</code> that will be filled with
		''' the destination region of interest. </param>
		''' <exception cref="IllegalArgumentException"> if <code>srcRegion</code>
		''' is <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>dstRegion</code>
		''' is <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if the resulting source or
		''' destination region is empty. </exception>
		Protected Friend Shared Sub computeRegions(ByVal param As ImageReadParam, ByVal srcWidth As Integer, ByVal srcHeight As Integer, ByVal image As java.awt.image.BufferedImage, ByVal srcRegion As java.awt.Rectangle, ByVal destRegion As java.awt.Rectangle)
			If srcRegion Is Nothing Then Throw New System.ArgumentException("srcRegion == null!")
			If destRegion Is Nothing Then Throw New System.ArgumentException("destRegion == null!")

			' Start with the entire source image
			srcRegion.boundsnds(0, 0, srcWidth, srcHeight)

			' Destination also starts with source image, as that is the
			' maximum extent if there is no subsampling
			destRegion.boundsnds(0, 0, srcWidth, srcHeight)

			' Clip that to the param region, if there is one
			Dim periodX As Integer = 1
			Dim periodY As Integer = 1
			Dim gridX As Integer = 0
			Dim gridY As Integer = 0
			If param IsNot Nothing Then
				Dim paramSrcRegion As java.awt.Rectangle = param.sourceRegion
				If paramSrcRegion IsNot Nothing Then srcRegion.bounds = srcRegion.intersection(paramSrcRegion)
				periodX = param.sourceXSubsampling
				periodY = param.sourceYSubsampling
				gridX = param.subsamplingXOffset
				gridY = param.subsamplingYOffset
				srcRegion.translate(gridX, gridY)
				srcRegion.width -= gridX
				srcRegion.height -= gridY
				destRegion.location = param.destinationOffset
			End If

			' Now clip any negative destination offsets, i.e. clip
			' to the top and left of the destination image
			If destRegion.x < 0 Then
				Dim delta As Integer = -destRegion.x*periodX
				srcRegion.x += delta
				srcRegion.width -= delta
				destRegion.x = 0
			End If
			If destRegion.y < 0 Then
				Dim delta As Integer = -destRegion.y*periodY
				srcRegion.y += delta
				srcRegion.height -= delta
				destRegion.y = 0
			End If

			' Now clip the destination Region to the subsampled width and height
			Dim subsampledWidth As Integer = (srcRegion.width + periodX - 1)/periodX
			Dim subsampledHeight As Integer = (srcRegion.height + periodY - 1)/periodY
			destRegion.width = subsampledWidth
			destRegion.height = subsampledHeight

			' Now clip that to right and bottom of the destination image,
			' if there is one, taking subsampling into account
			If image IsNot Nothing Then
				Dim destImageRect As New java.awt.Rectangle(0, 0, image.width, image.height)
				destRegion.bounds = destRegion.intersection(destImageRect)
				If destRegion.empty Then Throw New System.ArgumentException("Empty destination region!")

				Dim deltaX As Integer = destRegion.x + subsampledWidth - image.width
				If deltaX > 0 Then srcRegion.width -= deltaX*periodX
				Dim deltaY As Integer = destRegion.y + subsampledHeight - image.height
				If deltaY > 0 Then srcRegion.height -= deltaY*periodY
			End If
			If srcRegion.empty OrElse destRegion.empty Then Throw New System.ArgumentException("Empty region!")
		End Sub

		''' <summary>
		''' A utility method that may be used by readers to test the
		''' validity of the source and destination band settings of an
		''' <code>ImageReadParam</code>.  This method may be called as soon
		''' as the reader knows both the number of bands of the source
		''' image as it exists in the input stream, and the number of bands
		''' of the destination image that being written.
		''' 
		''' <p> The method retrieves the source and destination band
		''' setting arrays from param using the <code>getSourceBands</code>
		''' and <code>getDestinationBands</code>methods (or considers them
		''' to be <code>null</code> if <code>param</code> is
		''' <code>null</code>).  If the source band setting array is
		''' <code>null</code>, it is considered to be equal to the array
		''' <code>{ 0, 1, ..., numSrcBands - 1 }</code>, and similarly for
		''' the destination band setting array.
		''' 
		''' <p> The method then tests that both arrays are equal in length,
		''' and that neither array contains a value larger than the largest
		''' available band index.
		''' 
		''' <p> Any failure results in an
		''' <code>IllegalArgumentException</code> being thrown; success
		''' results in the method returning silently.
		''' </summary>
		''' <param name="param"> the <code>ImageReadParam</code> being used to read
		''' the image. </param>
		''' <param name="numSrcBands"> the number of bands of the image as it exists
		''' int the input source. </param>
		''' <param name="numDstBands"> the number of bands in the destination image
		''' being written.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>param</code>
		''' contains an invalid specification of a source and/or
		''' destination band subset. </exception>
		Protected Friend Shared Sub checkReadParamBandSettings(ByVal param As ImageReadParam, ByVal numSrcBands As Integer, ByVal numDstBands As Integer)
			' A null param is equivalent to srcBands == dstBands == null.
			Dim srcBands As Integer() = Nothing
			Dim dstBands As Integer() = Nothing
			If param IsNot Nothing Then
				srcBands = param.sourceBands
				dstBands = param.destinationBands
			End If

			Dim paramSrcBandLength As Integer = If(srcBands Is Nothing, numSrcBands, srcBands.Length)
			Dim paramDstBandLength As Integer = If(dstBands Is Nothing, numDstBands, dstBands.Length)

			If paramSrcBandLength <> paramDstBandLength Then Throw New System.ArgumentException("ImageReadParam num source & dest bands differ!")

			If srcBands IsNot Nothing Then
				For i As Integer = 0 To srcBands.Length - 1
					If srcBands(i) >= numSrcBands Then Throw New System.ArgumentException("ImageReadParam source bands contains a value >= the number of source bands!")
				Next i
			End If

			If dstBands IsNot Nothing Then
				For i As Integer = 0 To dstBands.Length - 1
					If dstBands(i) >= numDstBands Then Throw New System.ArgumentException("ImageReadParam dest bands contains a value >= the number of dest bands!")
				Next i
			End If
		End Sub

		''' <summary>
		''' Returns the <code>BufferedImage</code> to which decoded pixel
		''' data should be written.  The image is determined by inspecting
		''' the supplied <code>ImageReadParam</code> if it is
		''' non-<code>null</code>; if its <code>getDestination</code>
		''' method returns a non-<code>null</code> value, that image is
		''' simply returned.  Otherwise,
		''' <code>param.getDestinationType</code> method is called to
		''' determine if a particular image type has been specified.  If
		''' so, the returned <code>ImageTypeSpecifier</code> is used after
		''' checking that it is equal to one of those included in
		''' <code>imageTypes</code>.
		''' 
		''' <p> If <code>param</code> is <code>null</code> or the above
		''' steps have not yielded an image or an
		''' <code>ImageTypeSpecifier</code>, the first value obtained from
		''' the <code>imageTypes</code> parameter is used.  Typically, the
		''' caller will set <code>imageTypes</code> to the value of
		''' <code>getImageTypes(imageIndex)</code>.
		''' 
		''' <p> Next, the dimensions of the image are determined by a call
		''' to <code>computeRegions</code>.  The actual width and height of
		''' the image being decoded are passed in as the <code>width</code>
		''' and <code>height</code> parameters.
		''' </summary>
		''' <param name="param"> an <code>ImageReadParam</code> to be used to get
		''' the destination image or image type, or <code>null</code>. </param>
		''' <param name="imageTypes"> an <code>Iterator</code> of
		''' <code>ImageTypeSpecifier</code>s indicating the legal image
		''' types, with the default first. </param>
		''' <param name="width"> the true width of the image or tile begin decoded. </param>
		''' <param name="height"> the true width of the image or tile being decoded.
		''' </param>
		''' <returns> the <code>BufferedImage</code> to which decoded pixel
		''' data should be written.
		''' </returns>
		''' <exception cref="IIOException"> if the <code>ImageTypeSpecifier</code>
		''' specified by <code>param</code> does not match any of the legal
		''' ones from <code>imageTypes</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>imageTypes</code>
		''' is <code>null</code> or empty, or if an object not of type
		''' <code>ImageTypeSpecifier</code> is retrieved from it. </exception>
		''' <exception cref="IllegalArgumentException"> if the resulting image would
		''' have a width or height less than 1. </exception>
		''' <exception cref="IllegalArgumentException"> if the product of
		''' <code>width</code> and <code>height</code> is greater than
		''' <code>Integer.MAX_VALUE</code>. </exception>
		Protected Friend Shared Function getDestination(ByVal param As ImageReadParam, ByVal imageTypes As IEnumerator(Of ImageTypeSpecifier), ByVal width As Integer, ByVal height As Integer) As java.awt.image.BufferedImage
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If imageTypes Is Nothing OrElse (Not imageTypes.hasNext()) Then Throw New System.ArgumentException("imageTypes null or empty!")
			If CLng(width)*height > Integer.MaxValue Then Throw New System.ArgumentException("width*height > Integer.MAX_VALUE!")

			Dim dest As java.awt.image.BufferedImage = Nothing
			Dim imageType As ImageTypeSpecifier = Nothing

			' If param is non-null, use it
			If param IsNot Nothing Then
				' Try to get the image itself
				dest = param.destination
				If dest IsNot Nothing Then Return dest

				' No image, get the image type
				imageType = param.destinationType
			End If

			' No info from param, use fallback image type
			If imageType Is Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim o As Object = imageTypes.next()
				If Not(TypeOf o Is ImageTypeSpecifier) Then Throw New System.ArgumentException("Non-ImageTypeSpecifier retrieved from imageTypes!")
				imageType = CType(o, ImageTypeSpecifier)
			Else
				Dim foundIt As Boolean = False
				Do While imageTypes.MoveNext()
					Dim type As ImageTypeSpecifier = CType(imageTypes.Current, ImageTypeSpecifier)
					If type.Equals(imageType) Then
						foundIt = True
						Exit Do
					End If
				Loop

				If Not foundIt Then Throw New IIOException("Destination type from ImageReadParam does not match!")
			End If

			Dim srcRegion As New java.awt.Rectangle(0,0,0,0)
			Dim destRegion As New java.awt.Rectangle(0,0,0,0)
			computeRegions(param, width, height, Nothing, srcRegion, destRegion)

			Dim destWidth As Integer = destRegion.x + destRegion.width
			Dim destHeight As Integer = destRegion.y + destRegion.height
			' Create a new image based on the type specifier
			Return imageType.createBufferedImage(destWidth, destHeight)
		End Function
	End Class

End Namespace