'
' * Copyright (c) 2000, Oracle and/or its affiliates. All rights reserved.
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
	''' The service provider interface (SPI) for <code>ImageTranscoder</code>s.
	''' For more information on service provider classes, see the class comment
	''' for the <code>IIORegistry</code> class.
	''' </summary>
	''' <seealso cref= IIORegistry </seealso>
	''' <seealso cref= javax.imageio.ImageTranscoder
	'''  </seealso>
	Public MustInherit Class ImageTranscoderSpi
		Inherits IIOServiceProvider

		''' <summary>
		''' Constructs a blank <code>ImageTranscoderSpi</code>.  It is up
		''' to the subclass to initialize instance variables and/or
		''' override method implementations in order to provide working
		''' versions of all methods.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Constructs an <code>ImageTranscoderSpi</code> with a given set
		''' of values.
		''' </summary>
		''' <param name="vendorName"> the vendor name. </param>
		''' <param name="version"> a version identifier. </param>
		Public Sub New(ByVal vendorName As String, ByVal version As String)
			MyBase.New(vendorName, version)
		End Sub

		''' <summary>
		''' Returns the fully qualified class name of an
		''' <code>ImageReaderSpi</code> class that generates
		''' <code>IIOMetadata</code> objects that may be used as input to
		''' this transcoder.
		''' </summary>
		''' <returns> a <code>String</code> containing the fully-qualified
		''' class name of the <code>ImageReaderSpi</code> implementation class.
		''' </returns>
		''' <seealso cref= ImageReaderSpi </seealso>
		Public MustOverride ReadOnly Property readerServiceProviderName As String

		''' <summary>
		''' Returns the fully qualified class name of an
		''' <code>ImageWriterSpi</code> class that generates
		''' <code>IIOMetadata</code> objects that may be used as input to
		''' this transcoder.
		''' </summary>
		''' <returns> a <code>String</code> containing the fully-qualified
		''' class name of the <code>ImageWriterSpi</code> implementation class.
		''' </returns>
		''' <seealso cref= ImageWriterSpi </seealso>
		Public MustOverride ReadOnly Property writerServiceProviderName As String

		''' <summary>
		''' Returns an instance of the <code>ImageTranscoder</code>
		''' implementation associated with this service provider.
		''' </summary>
		''' <returns> an <code>ImageTranscoder</code> instance. </returns>
		Public MustOverride Function createTranscoderInstance() As javax.imageio.ImageTranscoder
	End Class

End Namespace