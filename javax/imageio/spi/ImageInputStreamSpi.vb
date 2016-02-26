Imports System

'
' * Copyright (c) 2000, 2004, Oracle and/or its affiliates. All rights reserved.
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
	''' The service provider interface (SPI) for
	''' <code>ImageInputStream</code>s.  For more information on service
	''' provider interfaces, see the class comment for the
	''' <code>IIORegistry</code> class.
	''' 
	''' <p> This interface allows arbitrary objects to be "wrapped" by
	''' instances of <code>ImageInputStream</code>.  For example,
	''' a particular <code>ImageInputStreamSpi</code> might allow
	''' a generic <code>InputStream</code> to be used as an input source;
	''' another might take input from a <code>URL</code>.
	''' 
	''' <p> By treating the creation of <code>ImageInputStream</code>s as a
	''' pluggable service, it becomes possible to handle future input
	''' sources without changing the API.  Also, high-performance
	''' implementations of <code>ImageInputStream</code> (for example,
	''' native implementations for a particular platform) can be installed
	''' and used transparently by applications.
	''' </summary>
	''' <seealso cref= IIORegistry </seealso>
	''' <seealso cref= javax.imageio.stream.ImageInputStream
	'''  </seealso>
	Public MustInherit Class ImageInputStreamSpi
		Inherits IIOServiceProvider

		''' <summary>
		''' A <code>Class</code> object indicating the legal object type
		''' for use by the <code>createInputStreamInstance</code> method.
		''' </summary>
		Protected Friend inputClass As Type

		''' <summary>
		''' Constructs a blank <code>ImageInputStreamSpi</code>.  It is up
		''' to the subclass to initialize instance variables and/or
		''' override method implementations in order to provide working
		''' versions of all methods.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Constructs an <code>ImageInputStreamSpi</code> with a given set
		''' of values.
		''' </summary>
		''' <param name="vendorName"> the vendor name. </param>
		''' <param name="version"> a version identifier. </param>
		''' <param name="inputClass"> a <code>Class</code> object indicating the
		''' legal object type for use by the
		''' <code>createInputStreamInstance</code> method.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if <code>vendorName</code>
		''' is <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>version</code>
		''' is <code>null</code>. </exception>
		Public Sub New(ByVal vendorName As String, ByVal version As String, ByVal inputClass As Type)
			MyBase.New(vendorName, version)
			Me.inputClass = inputClass
		End Sub

		''' <summary>
		''' Returns a <code>Class</code> object representing the class or
		''' interface type that must be implemented by an input source in
		''' order to be "wrapped" in an <code>ImageInputStream</code> via
		''' the <code>createInputStreamInstance</code> method.
		''' 
		''' <p> Typical return values might include
		''' <code>InputStream.class</code> or <code>URL.class</code>, but
		''' any class may be used.
		''' </summary>
		''' <returns> a <code>Class</code> variable.
		''' </returns>
		''' <seealso cref= #createInputStreamInstance(Object, boolean, File) </seealso>
		Public Overridable Property inputClass As Type
			Get
				Return inputClass
			End Get
		End Property

		''' <summary>
		''' Returns <code>true</code> if the <code>ImageInputStream</code>
		''' implementation associated with this service provider can
		''' optionally make use of a cache file for improved performance
		''' and/or memory footrprint.  If <code>false</code>, the value of
		''' the <code>useCache</code> argument to
		''' <code>createInputStreamInstance</code> will be ignored.
		''' 
		''' <p> The default implementation returns <code>false</code>.
		''' </summary>
		''' <returns> <code>true</code> if a cache file can be used by the
		''' input streams created by this service provider. </returns>
		Public Overridable Function canUseCacheFile() As Boolean
			Return False
		End Function

		''' <summary>
		''' Returns <code>true</code> if the <code>ImageInputStream</code>
		''' implementation associated with this service provider requires
		''' the use of a cache <code>File</code>.  If <code>true</code>,
		''' the value of the <code>useCache</code> argument to
		''' <code>createInputStreamInstance</code> will be ignored.
		''' 
		''' <p> The default implementation returns <code>false</code>.
		''' </summary>
		''' <returns> <code>true</code> if a cache file is needed by the
		''' input streams created by this service provider. </returns>
		Public Overridable Function needsCacheFile() As Boolean
			Return False
		End Function

		''' <summary>
		''' Returns an instance of the <code>ImageInputStream</code>
		''' implementation associated with this service provider.  If the
		''' use of a cache file is optional, the <code>useCache</code>
		''' parameter will be consulted.  Where a cache is required, or
		''' not applicable, the value of <code>useCache</code> will be ignored.
		''' </summary>
		''' <param name="input"> an object of the class type returned by
		''' <code>getInputClass</code>. </param>
		''' <param name="useCache"> a <code>boolean</code> indicating whether a
		''' cache file should be used, in cases where it is optional. </param>
		''' <param name="cacheDir"> a <code>File</code> indicating where the
		''' cache file should be created, or <code>null</code> to use the
		''' system directory.
		''' </param>
		''' <returns> an <code>ImageInputStream</code> instance.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>input</code> is
		''' not an instance of the correct class or is <code>null</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if a cache file is needed
		''' but <code>cacheDir</code> is non-<code>null</code> and is not a
		''' directory. </exception>
		''' <exception cref="IOException"> if a cache file is needed but cannot be
		''' created.
		''' </exception>
		''' <seealso cref= #getInputClass </seealso>
		''' <seealso cref= #canUseCacheFile </seealso>
		''' <seealso cref= #needsCacheFile </seealso>
		Public MustOverride Function createInputStreamInstance(ByVal input As Object, ByVal useCache As Boolean, ByVal cacheDir As java.io.File) As javax.imageio.stream.ImageInputStream

		''' <summary>
		''' Returns an instance of the <code>ImageInputStream</code>
		''' implementation associated with this service provider.  A cache
		''' file will be created in the system-dependent default
		''' temporary-file directory, if needed.
		''' </summary>
		''' <param name="input"> an object of the class type returned by
		''' <code>getInputClass</code>.
		''' </param>
		''' <returns> an <code>ImageInputStream</code> instance.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>input</code> is
		''' not an instance of the correct class or is <code>null</code>. </exception>
		''' <exception cref="IOException"> if a cache file is needed but cannot be
		''' created.
		''' </exception>
		''' <seealso cref= #getInputClass() </seealso>
		Public Overridable Function createInputStreamInstance(ByVal input As Object) As javax.imageio.stream.ImageInputStream
			Return createInputStreamInstance(input, True, Nothing)
		End Function
	End Class

End Namespace