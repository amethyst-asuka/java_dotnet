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

Namespace javax.imageio.metadata


	''' <summary>
	''' An abstract class to be extended by objects that represent metadata
	''' (non-image data) associated with images and streams.  Plug-ins
	''' represent metadata using opaque, plug-in specific objects.  These
	''' objects, however, provide the ability to access their internal
	''' information as a tree of <code>IIOMetadataNode</code> objects that
	''' support the XML DOM interfaces as well as additional interfaces for
	''' storing non-textual data and retrieving information about legal
	''' data values.  The format of such trees is plug-in dependent, but
	''' plug-ins may choose to support a plug-in neutral format described
	''' below.  A single plug-in may support multiple metadata formats,
	''' whose names maybe determined by calling
	''' <code>getMetadataFormatNames</code>.  The plug-in may also support
	''' a single special format, referred to as the "native" format, which
	''' is designed to encode its metadata losslessly.  This format will
	''' typically be designed specifically to work with a specific file
	''' format, so that images may be loaded and saved in the same format
	''' with no loss of metadata, but may be less useful for transferring
	''' metadata between an <code>ImageReader</code> and an
	''' <code>ImageWriter</code> for different image formats.  To convert
	''' between two native formats as losslessly as the image file formats
	''' will allow, an <code>ImageTranscoder</code> object must be used.
	''' </summary>
	''' <seealso cref= javax.imageio.ImageReader#getImageMetadata </seealso>
	''' <seealso cref= javax.imageio.ImageReader#getStreamMetadata </seealso>
	''' <seealso cref= javax.imageio.ImageReader#readAll </seealso>
	''' <seealso cref= javax.imageio.ImageWriter#getDefaultStreamMetadata </seealso>
	''' <seealso cref= javax.imageio.ImageWriter#getDefaultImageMetadata </seealso>
	''' <seealso cref= javax.imageio.ImageWriter#write </seealso>
	''' <seealso cref= javax.imageio.ImageWriter#convertImageMetadata </seealso>
	''' <seealso cref= javax.imageio.ImageWriter#convertStreamMetadata </seealso>
	''' <seealso cref= javax.imageio.IIOImage </seealso>
	''' <seealso cref= javax.imageio.ImageTranscoder
	'''  </seealso>
	Public MustInherit Class IIOMetadata

		''' <summary>
		''' A boolean indicating whether the concrete subclass supports the
		''' standard metadata format, set via the constructor.
		''' </summary>
		Protected Friend standardFormatSupported As Boolean

		''' <summary>
		''' The name of the native metadata format for this object,
		''' initialized to <code>null</code> and set via the constructor.
		''' </summary>
		Protected Friend nativeMetadataFormatName As String = Nothing

		''' <summary>
		''' The name of the class implementing <code>IIOMetadataFormat</code>
		''' and representing the native metadata format, initialized to
		''' <code>null</code> and set via the constructor.
		''' </summary>
		Protected Friend nativeMetadataFormatClassName As String = Nothing

		''' <summary>
		''' An array of names of formats, other than the standard and
		''' native formats, that are supported by this plug-in,
		''' initialized to <code>null</code> and set via the constructor.
		''' </summary>
		Protected Friend extraMetadataFormatNames As String() = Nothing

		''' <summary>
		''' An array of names of classes implementing <code>IIOMetadataFormat</code>
		''' and representing the metadata formats, other than the standard and
		''' native formats, that are supported by this plug-in,
		''' initialized to <code>null</code> and set via the constructor.
		''' </summary>
		Protected Friend extraMetadataFormatClassNames As String() = Nothing

		''' <summary>
		''' An <code>IIOMetadataController</code> that is suggested for use
		''' as the controller for this <code>IIOMetadata</code> object.  It
		''' may be retrieved via <code>getDefaultController</code>.  To
		''' install the default controller, call
		''' <code>setController(getDefaultController())</code>.  This
		''' instance variable should be set by subclasses that choose to
		''' provide their own default controller, usually a GUI, for
		''' setting parameters.
		''' </summary>
		''' <seealso cref= IIOMetadataController </seealso>
		''' <seealso cref= #getDefaultController </seealso>
		Protected Friend defaultController As IIOMetadataController = Nothing

		''' <summary>
		''' The <code>IIOMetadataController</code> that will be
		''' used to provide settings for this <code>IIOMetadata</code>
		''' object when the <code>activateController</code> method
		''' is called.  This value overrides any default controller,
		''' even when <code>null</code>.
		''' </summary>
		''' <seealso cref= IIOMetadataController </seealso>
		''' <seealso cref= #setController(IIOMetadataController) </seealso>
		''' <seealso cref= #hasController() </seealso>
		''' <seealso cref= #activateController() </seealso>
		Protected Friend controller As IIOMetadataController = Nothing

		''' <summary>
		''' Constructs an empty <code>IIOMetadata</code> object.  The
		''' subclass is responsible for supplying values for all protected
		''' instance variables that will allow any non-overridden default
		''' implementations of methods to satisfy their contracts.  For example,
		''' <code>extraMetadataFormatNames</code> should not have length 0.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Constructs an <code>IIOMetadata</code> object with the given
		''' format names and format class names, as well as a boolean
		''' indicating whether the standard format is supported.
		''' 
		''' <p> This constructor does not attempt to check the class names
		''' for validity.  Invalid class names may cause exceptions in
		''' subsequent calls to <code>getMetadataFormat</code>.
		''' </summary>
		''' <param name="standardMetadataFormatSupported"> <code>true</code> if
		''' this object can return or accept a DOM tree using the standard
		''' metadata format. </param>
		''' <param name="nativeMetadataFormatName"> the name of the native metadata
		''' format, as a <code>String</code>, or <code>null</code> if there
		''' is no native format. </param>
		''' <param name="nativeMetadataFormatClassName"> the name of the class of
		''' the native metadata format, or <code>null</code> if there is
		''' no native format. </param>
		''' <param name="extraMetadataFormatNames"> an array of <code>String</code>s
		''' indicating additional formats supported by this object, or
		''' <code>null</code> if there are none. </param>
		''' <param name="extraMetadataFormatClassNames"> an array of <code>String</code>s
		''' indicating the class names of any additional formats supported by
		''' this object, or <code>null</code> if there are none.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if
		''' <code>extraMetadataFormatNames</code> has length 0. </exception>
		''' <exception cref="IllegalArgumentException"> if
		''' <code>extraMetadataFormatNames</code> and
		''' <code>extraMetadataFormatClassNames</code> are neither both
		''' <code>null</code>, nor of the same length. </exception>
		Protected Friend Sub New(ByVal standardMetadataFormatSupported As Boolean, ByVal nativeMetadataFormatName As String, ByVal nativeMetadataFormatClassName As String, ByVal extraMetadataFormatNames As String(), ByVal extraMetadataFormatClassNames As String())
			Me.standardFormatSupported = standardMetadataFormatSupported
			Me.nativeMetadataFormatName = nativeMetadataFormatName
			Me.nativeMetadataFormatClassName = nativeMetadataFormatClassName
			If extraMetadataFormatNames IsNot Nothing Then
				If extraMetadataFormatNames.Length = 0 Then Throw New System.ArgumentException("extraMetadataFormatNames.length == 0!")
				If extraMetadataFormatClassNames Is Nothing Then Throw New System.ArgumentException("extraMetadataFormatNames != null && extraMetadataFormatClassNames == null!")
				If extraMetadataFormatClassNames.Length <> extraMetadataFormatNames.Length Then Throw New System.ArgumentException("extraMetadataFormatClassNames.length != extraMetadataFormatNames.length!")
				Me.extraMetadataFormatNames = CType(extraMetadataFormatNames.clone(), String())
				Me.extraMetadataFormatClassNames = CType(extraMetadataFormatClassNames.clone(), String())
			Else
				If extraMetadataFormatClassNames IsNot Nothing Then Throw New System.ArgumentException("extraMetadataFormatNames == null && extraMetadataFormatClassNames != null!")
			End If
		End Sub

		''' <summary>
		''' Returns <code>true</code> if the standard metadata format is
		''' supported by <code>getMetadataFormat</code>,
		''' <code>getAsTree</code>, <code>setFromTree</code>, and
		''' <code>mergeTree</code>.
		''' 
		''' <p> The default implementation returns the value of the
		''' <code>standardFormatSupported</code> instance variable.
		''' </summary>
		''' <returns> <code>true</code> if the standard metadata format
		''' is supported.
		''' </returns>
		''' <seealso cref= #getAsTree </seealso>
		''' <seealso cref= #setFromTree </seealso>
		''' <seealso cref= #mergeTree </seealso>
		''' <seealso cref= #getMetadataFormat </seealso>
		Public Overridable Property standardMetadataFormatSupported As Boolean
			Get
				Return standardFormatSupported
			End Get
		End Property

		''' <summary>
		''' Returns <code>true</code> if this object does not support the
		''' <code>mergeTree</code>, <code>setFromTree</code>, and
		''' <code>reset</code> methods.
		''' </summary>
		''' <returns> true if this <code>IIOMetadata</code> object cannot be
		''' modified. </returns>
		Public MustOverride ReadOnly Property [readOnly] As Boolean

		''' <summary>
		''' Returns the name of the "native" metadata format for this
		''' plug-in, which typically allows for lossless encoding and
		''' transmission of the metadata stored in the format handled by
		''' this plug-in.  If no such format is supported,
		''' <code>null</code>will be returned.
		''' 
		''' <p> The structure and contents of the "native" metadata format
		''' are defined by the plug-in that created this
		''' <code>IIOMetadata</code> object.  Plug-ins for simple formats
		''' will usually create a dummy node for the root, and then a
		''' series of child nodes representing individual tags, chunks, or
		''' keyword/value pairs.  A plug-in may choose whether or not to
		''' document its native format.
		''' 
		''' <p> The default implementation returns the value of the
		''' <code>nativeMetadataFormatName</code> instance variable.
		''' </summary>
		''' <returns> the name of the native format, or <code>null</code>.
		''' </returns>
		''' <seealso cref= #getExtraMetadataFormatNames </seealso>
		''' <seealso cref= #getMetadataFormatNames </seealso>
		Public Overridable Property nativeMetadataFormatName As String
			Get
				Return nativeMetadataFormatName
			End Get
		End Property

		''' <summary>
		''' Returns an array of <code>String</code>s containing the names
		''' of additional metadata formats, other than the native and standard
		''' formats, recognized by this plug-in's
		''' <code>getAsTree</code>, <code>setFromTree</code>, and
		''' <code>mergeTree</code> methods.  If there are no such additional
		''' formats, <code>null</code> is returned.
		''' 
		''' <p> The default implementation returns a clone of the
		''' <code>extraMetadataFormatNames</code> instance variable.
		''' </summary>
		''' <returns> an array of <code>String</code>s with length at least
		''' 1, or <code>null</code>.
		''' </returns>
		''' <seealso cref= #getAsTree </seealso>
		''' <seealso cref= #setFromTree </seealso>
		''' <seealso cref= #mergeTree </seealso>
		''' <seealso cref= #getNativeMetadataFormatName </seealso>
		''' <seealso cref= #getMetadataFormatNames </seealso>
		Public Overridable Property extraMetadataFormatNames As String()
			Get
				If extraMetadataFormatNames Is Nothing Then Return Nothing
				Return CType(extraMetadataFormatNames.clone(), String())
			End Get
		End Property

		''' <summary>
		''' Returns an array of <code>String</code>s containing the names
		''' of all metadata formats, including the native and standard
		''' formats, recognized by this plug-in's <code>getAsTree</code>,
		''' <code>setFromTree</code>, and <code>mergeTree</code> methods.
		''' If there are no such formats, <code>null</code> is returned.
		''' 
		''' <p> The default implementation calls
		''' <code>getNativeMetadataFormatName</code>,
		''' <code>isStandardMetadataFormatSupported</code>, and
		''' <code>getExtraMetadataFormatNames</code> and returns the
		''' combined results.
		''' </summary>
		''' <returns> an array of <code>String</code>s.
		''' </returns>
		''' <seealso cref= #getNativeMetadataFormatName </seealso>
		''' <seealso cref= #isStandardMetadataFormatSupported </seealso>
		''' <seealso cref= #getExtraMetadataFormatNames </seealso>
		Public Overridable Property metadataFormatNames As String()
			Get
				Dim nativeName As String = nativeMetadataFormatName
				Dim standardName As String = If(standardMetadataFormatSupported, IIOMetadataFormatImpl.standardMetadataFormatName, Nothing)
				Dim extraNames As String() = extraMetadataFormatNames
    
				Dim numFormats As Integer = 0
				If nativeName IsNot Nothing Then numFormats += 1
				If standardName IsNot Nothing Then numFormats += 1
				If extraNames IsNot Nothing Then numFormats += extraNames.Length
				If numFormats = 0 Then Return Nothing
    
				Dim formats As String() = New String(numFormats - 1){}
				Dim index As Integer = 0
				If nativeName IsNot Nothing Then formats(index += 1) = nativeName
				If standardName IsNot Nothing Then formats(index += 1) = standardName
				If extraNames IsNot Nothing Then
					For i As Integer = 0 To extraNames.Length - 1
	'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
						formats(index += 1) = extraNames(i)
					Next i
				End If
    
				Return formats
			End Get
		End Property

		''' <summary>
		''' Returns an <code>IIOMetadataFormat</code> object describing the
		''' given metadata format, or <code>null</code> if no description
		''' is available.  The supplied name must be one of those returned
		''' by <code>getMetadataFormatNames</code> (<i>i.e.</i>, either the
		''' native format name, the standard format name, or one of those
		''' returned by <code>getExtraMetadataFormatNames</code>).
		''' 
		''' <p> The default implementation checks the name against the
		''' global standard metadata format name, and returns that format
		''' if it is supported.  Otherwise, it checks against the native
		''' format names followed by any additional format names.  If a
		''' match is found, it retrieves the name of the
		''' <code>IIOMetadataFormat</code> class from
		''' <code>nativeMetadataFormatClassName</code> or
		''' <code>extraMetadataFormatClassNames</code> as appropriate, and
		''' constructs an instance of that class using its
		''' <code>getInstance</code> method.
		''' </summary>
		''' <param name="formatName"> the desired metadata format.
		''' </param>
		''' <returns> an <code>IIOMetadataFormat</code> object.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>formatName</code>
		''' is <code>null</code> or is not one of the names recognized by
		''' the plug-in. </exception>
		''' <exception cref="IllegalStateException"> if the class corresponding to
		''' the format name cannot be loaded. </exception>
		Public Overridable Function getMetadataFormat(ByVal formatName As String) As IIOMetadataFormat
			If formatName Is Nothing Then Throw New System.ArgumentException("formatName == null!")
			If standardFormatSupported AndAlso formatName.Equals(IIOMetadataFormatImpl.standardMetadataFormatName) Then Return IIOMetadataFormatImpl.standardFormatInstance
			Dim formatClassName As String = Nothing
			If formatName.Equals(nativeMetadataFormatName) Then
				formatClassName = nativeMetadataFormatClassName
			ElseIf extraMetadataFormatNames IsNot Nothing Then
				For i As Integer = 0 To extraMetadataFormatNames.Length - 1
					If formatName.Equals(extraMetadataFormatNames(i)) Then
						formatClassName = extraMetadataFormatClassNames(i)
						Exit For ' out of for
					End If
				Next i
			End If
			If formatClassName Is Nothing Then Throw New System.ArgumentException("Unsupported format name")
			Try
				Dim cls As Type = Nothing
				Dim o As Object = Me

				' firstly we try to use classloader used for loading
				' the IIOMetadata implemantation for this plugin.
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				ClassLoader loader = (ClassLoader) java.security.AccessController.doPrivileged(New java.security.PrivilegedAction()
	'			{
	'							public Object run()
	'							{
	'								Return o.getClass().getClassLoader();
	'							}
	'						});

				Try
					cls = Type.GetType(formatClassName, True, loader)
				Catch e As ClassNotFoundException
					' we failed to load IIOMetadataFormat class by
					' using IIOMetadata classloader.Next try is to
					' use thread context classloader.
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'					loader = (ClassLoader) java.security.AccessController.doPrivileged(New java.security.PrivilegedAction()
	'				{
	'								public Object run()
	'								{
	'									Return Thread.currentThread().getContextClassLoader();
	'								}
	'						});
					Try
						cls = Type.GetType(formatClassName, True, loader)
					Catch e1 As ClassNotFoundException
						' finally we try to use system classloader in case
						' if we failed to load IIOMetadataFormat implementation
						' class above.
						cls = Type.GetType(formatClassName, True, ClassLoader.systemClassLoader)
					End Try
				End Try

				Dim meth As Method = cls.GetMethod("getInstance")
				Return CType(meth.invoke(Nothing), IIOMetadataFormat)
			Catch e As Exception
				Dim ex As Exception = New IllegalStateException("Can't obtain format")
				ex.initCause(e)
				Throw ex
			End Try

		End Function

		''' <summary>
		''' Returns an XML DOM <code>Node</code> object that represents the
		''' root of a tree of metadata contained within this object
		''' according to the conventions defined by a given metadata
		''' format.
		''' 
		''' <p> The names of the available metadata formats may be queried
		''' using the <code>getMetadataFormatNames</code> method.
		''' </summary>
		''' <param name="formatName"> the desired metadata format.
		''' </param>
		''' <returns> an XML DOM <code>Node</code> object forming the
		''' root of a tree.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if <code>formatName</code>
		''' is <code>null</code> or is not one of the names returned by
		''' <code>getMetadataFormatNames</code>.
		''' </exception>
		''' <seealso cref= #getMetadataFormatNames </seealso>
		''' <seealso cref= #setFromTree </seealso>
		''' <seealso cref= #mergeTree </seealso>
		Public MustOverride Function getAsTree(ByVal formatName As String) As org.w3c.dom.Node

		''' <summary>
		''' Alters the internal state of this <code>IIOMetadata</code>
		''' object from a tree of XML DOM <code>Node</code>s whose syntax
		''' is defined by the given metadata format.  The previous state is
		''' altered only as necessary to accommodate the nodes that are
		''' present in the given tree.  If the tree structure or contents
		''' are invalid, an <code>IIOInvalidTreeException</code> will be
		''' thrown.
		''' 
		''' <p> As the semantics of how a tree or subtree may be merged with
		''' another tree are completely format-specific, plug-in authors may
		''' implement this method in whatever manner is most appropriate for
		''' the format, including simply replacing all existing state with the
		''' contents of the given tree.
		''' </summary>
		''' <param name="formatName"> the desired metadata format. </param>
		''' <param name="root"> an XML DOM <code>Node</code> object forming the
		''' root of a tree.
		''' </param>
		''' <exception cref="IllegalStateException"> if this object is read-only. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>formatName</code>
		''' is <code>null</code> or is not one of the names returned by
		''' <code>getMetadataFormatNames</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>root</code> is
		''' <code>null</code>. </exception>
		''' <exception cref="IIOInvalidTreeException"> if the tree cannot be parsed
		''' successfully using the rules of the given format.
		''' </exception>
		''' <seealso cref= #getMetadataFormatNames </seealso>
		''' <seealso cref= #getAsTree </seealso>
		''' <seealso cref= #setFromTree </seealso>
		Public MustOverride Sub mergeTree(ByVal formatName As String, ByVal root As org.w3c.dom.Node)

		''' <summary>
		''' Returns an <code>IIOMetadataNode</code> representing the chroma
		''' information of the standard <code>javax_imageio_1.0</code>
		''' metadata format, or <code>null</code> if no such information is
		''' available.  This method is intended to be called by the utility
		''' routine <code>getStandardTree</code>.
		''' 
		''' <p> The default implementation returns <code>null</code>.
		''' 
		''' <p> Subclasses should override this method to produce an
		''' appropriate subtree if they wish to support the standard
		''' metadata format.
		''' </summary>
		''' <returns> an <code>IIOMetadataNode</code>, or <code>null</code>.
		''' </returns>
		''' <seealso cref= #getStandardTree </seealso>
		Protected Friend Overridable Property standardChromaNode As IIOMetadataNode
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Returns an <code>IIOMetadataNode</code> representing the
		''' compression information of the standard
		''' <code>javax_imageio_1.0</code> metadata format, or
		''' <code>null</code> if no such information is available.  This
		''' method is intended to be called by the utility routine
		''' <code>getStandardTree</code>.
		''' 
		''' <p> The default implementation returns <code>null</code>.
		''' 
		''' <p> Subclasses should override this method to produce an
		''' appropriate subtree if they wish to support the standard
		''' metadata format.
		''' </summary>
		''' <returns> an <code>IIOMetadataNode</code>, or <code>null</code>.
		''' </returns>
		''' <seealso cref= #getStandardTree </seealso>
		Protected Friend Overridable Property standardCompressionNode As IIOMetadataNode
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Returns an <code>IIOMetadataNode</code> representing the data
		''' format information of the standard
		''' <code>javax_imageio_1.0</code> metadata format, or
		''' <code>null</code> if no such information is available.  This
		''' method is intended to be called by the utility routine
		''' <code>getStandardTree</code>.
		''' 
		''' <p> The default implementation returns <code>null</code>.
		''' 
		''' <p> Subclasses should override this method to produce an
		''' appropriate subtree if they wish to support the standard
		''' metadata format.
		''' </summary>
		''' <returns> an <code>IIOMetadataNode</code>, or <code>null</code>.
		''' </returns>
		''' <seealso cref= #getStandardTree </seealso>
		Protected Friend Overridable Property standardDataNode As IIOMetadataNode
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Returns an <code>IIOMetadataNode</code> representing the
		''' dimension information of the standard
		''' <code>javax_imageio_1.0</code> metadata format, or
		''' <code>null</code> if no such information is available.  This
		''' method is intended to be called by the utility routine
		''' <code>getStandardTree</code>.
		''' 
		''' <p> The default implementation returns <code>null</code>.
		''' 
		''' <p> Subclasses should override this method to produce an
		''' appropriate subtree if they wish to support the standard
		''' metadata format.
		''' </summary>
		''' <returns> an <code>IIOMetadataNode</code>, or <code>null</code>.
		''' </returns>
		''' <seealso cref= #getStandardTree </seealso>
		Protected Friend Overridable Property standardDimensionNode As IIOMetadataNode
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Returns an <code>IIOMetadataNode</code> representing the document
		''' information of the standard <code>javax_imageio_1.0</code>
		''' metadata format, or <code>null</code> if no such information is
		''' available.  This method is intended to be called by the utility
		''' routine <code>getStandardTree</code>.
		''' 
		''' <p> The default implementation returns <code>null</code>.
		''' 
		''' <p> Subclasses should override this method to produce an
		''' appropriate subtree if they wish to support the standard
		''' metadata format.
		''' </summary>
		''' <returns> an <code>IIOMetadataNode</code>, or <code>null</code>.
		''' </returns>
		''' <seealso cref= #getStandardTree </seealso>
		Protected Friend Overridable Property standardDocumentNode As IIOMetadataNode
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Returns an <code>IIOMetadataNode</code> representing the textual
		''' information of the standard <code>javax_imageio_1.0</code>
		''' metadata format, or <code>null</code> if no such information is
		''' available.  This method is intended to be called by the utility
		''' routine <code>getStandardTree</code>.
		''' 
		''' <p> The default implementation returns <code>null</code>.
		''' 
		''' <p> Subclasses should override this method to produce an
		''' appropriate subtree if they wish to support the standard
		''' metadata format.
		''' </summary>
		''' <returns> an <code>IIOMetadataNode</code>, or <code>null</code>.
		''' </returns>
		''' <seealso cref= #getStandardTree </seealso>
		Protected Friend Overridable Property standardTextNode As IIOMetadataNode
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Returns an <code>IIOMetadataNode</code> representing the tiling
		''' information of the standard <code>javax_imageio_1.0</code>
		''' metadata format, or <code>null</code> if no such information is
		''' available.  This method is intended to be called by the utility
		''' routine <code>getStandardTree</code>.
		''' 
		''' <p> The default implementation returns <code>null</code>.
		''' 
		''' <p> Subclasses should override this method to produce an
		''' appropriate subtree if they wish to support the standard
		''' metadata format.
		''' </summary>
		''' <returns> an <code>IIOMetadataNode</code>, or <code>null</code>.
		''' </returns>
		''' <seealso cref= #getStandardTree </seealso>
		Protected Friend Overridable Property standardTileNode As IIOMetadataNode
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Returns an <code>IIOMetadataNode</code> representing the
		''' transparency information of the standard
		''' <code>javax_imageio_1.0</code> metadata format, or
		''' <code>null</code> if no such information is available.  This
		''' method is intended to be called by the utility routine
		''' <code>getStandardTree</code>.
		''' 
		''' <p> The default implementation returns <code>null</code>.
		''' 
		''' <p> Subclasses should override this method to produce an
		''' appropriate subtree if they wish to support the standard
		''' metadata format.
		''' </summary>
		''' <returns> an <code>IIOMetadataNode</code>, or <code>null</code>. </returns>
		Protected Friend Overridable Property standardTransparencyNode As IIOMetadataNode
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Appends a new node to an existing node, if the new node is
		''' non-<code>null</code>.
		''' </summary>
		Private Sub append(ByVal root As IIOMetadataNode, ByVal node As IIOMetadataNode)
			If node IsNot Nothing Then root.appendChild(node)
		End Sub

		''' <summary>
		''' A utility method to return a tree of
		''' <code>IIOMetadataNode</code>s representing the metadata
		''' contained within this object according to the conventions of
		''' the standard <code>javax_imageio_1.0</code> metadata format.
		''' 
		''' <p> This method calls the various <code>getStandard*Node</code>
		''' methods to supply each of the subtrees rooted at the children
		''' of the root node.  If any of those methods returns
		''' <code>null</code>, the corresponding subtree will be omitted.
		''' If all of them return <code>null</code>, a tree consisting of a
		''' single root node will be returned.
		''' </summary>
		''' <returns> an <code>IIOMetadataNode</code> representing the root
		''' of a metadata tree in the <code>javax_imageio_1.0</code>
		''' format.
		''' </returns>
		''' <seealso cref= #getStandardChromaNode </seealso>
		''' <seealso cref= #getStandardCompressionNode </seealso>
		''' <seealso cref= #getStandardDataNode </seealso>
		''' <seealso cref= #getStandardDimensionNode </seealso>
		''' <seealso cref= #getStandardDocumentNode </seealso>
		''' <seealso cref= #getStandardTextNode </seealso>
		''' <seealso cref= #getStandardTileNode </seealso>
		''' <seealso cref= #getStandardTransparencyNode </seealso>
		Protected Friend Property standardTree As IIOMetadataNode
			Get
				Dim root As New IIOMetadataNode(IIOMetadataFormatImpl.standardMetadataFormatName)
				append(root, standardChromaNode)
				append(root, standardCompressionNode)
				append(root, standardDataNode)
				append(root, standardDimensionNode)
				append(root, standardDocumentNode)
				append(root, standardTextNode)
				append(root, standardTileNode)
				append(root, standardTransparencyNode)
				Return root
			End Get
		End Property

		''' <summary>
		''' Sets the internal state of this <code>IIOMetadata</code> object
		''' from a tree of XML DOM <code>Node</code>s whose syntax is
		''' defined by the given metadata format.  The previous state is
		''' discarded.  If the tree's structure or contents are invalid, an
		''' <code>IIOInvalidTreeException</code> will be thrown.
		''' 
		''' <p> The default implementation calls <code>reset</code>
		''' followed by <code>mergeTree(formatName, root)</code>.
		''' </summary>
		''' <param name="formatName"> the desired metadata format. </param>
		''' <param name="root"> an XML DOM <code>Node</code> object forming the
		''' root of a tree.
		''' </param>
		''' <exception cref="IllegalStateException"> if this object is read-only. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>formatName</code>
		''' is <code>null</code> or is not one of the names returned by
		''' <code>getMetadataFormatNames</code>. </exception>
		''' <exception cref="IllegalArgumentException"> if <code>root</code> is
		''' <code>null</code>. </exception>
		''' <exception cref="IIOInvalidTreeException"> if the tree cannot be parsed
		''' successfully using the rules of the given format.
		''' </exception>
		''' <seealso cref= #getMetadataFormatNames </seealso>
		''' <seealso cref= #getAsTree </seealso>
		''' <seealso cref= #mergeTree </seealso>
		Public Overridable Sub setFromTree(ByVal formatName As String, ByVal root As org.w3c.dom.Node)
			reset()
			mergeTree(formatName, root)
		End Sub

		''' <summary>
		''' Resets all the data stored in this object to default values,
		''' usually to the state this object was in immediately after
		''' construction, though the precise semantics are plug-in specific.
		''' Note that there are many possible default values, depending on
		''' how the object was created.
		''' </summary>
		''' <exception cref="IllegalStateException"> if this object is read-only.
		''' </exception>
		''' <seealso cref= javax.imageio.ImageReader#getStreamMetadata </seealso>
		''' <seealso cref= javax.imageio.ImageReader#getImageMetadata </seealso>
		''' <seealso cref= javax.imageio.ImageWriter#getDefaultStreamMetadata </seealso>
		''' <seealso cref= javax.imageio.ImageWriter#getDefaultImageMetadata </seealso>
		Public MustOverride Sub reset()

		''' <summary>
		''' Sets the <code>IIOMetadataController</code> to be used
		''' to provide settings for this <code>IIOMetadata</code>
		''' object when the <code>activateController</code> method
		''' is called, overriding any default controller.  If the
		''' argument is <code>null</code>, no controller will be
		''' used, including any default.  To restore the default, use
		''' <code>setController(getDefaultController())</code>.
		''' 
		''' <p> The default implementation sets the <code>controller</code>
		''' instance variable to the supplied value.
		''' </summary>
		''' <param name="controller"> An appropriate
		''' <code>IIOMetadataController</code>, or <code>null</code>.
		''' </param>
		''' <seealso cref= IIOMetadataController </seealso>
		''' <seealso cref= #getController </seealso>
		''' <seealso cref= #getDefaultController </seealso>
		''' <seealso cref= #hasController </seealso>
		''' <seealso cref= #activateController() </seealso>
		Public Overridable Property controller As IIOMetadataController
			Set(ByVal controller As IIOMetadataController)
				Me.controller = controller
			End Set
			Get
				Return controller
			End Get
		End Property


		''' <summary>
		''' Returns the default <code>IIOMetadataController</code>, if there
		''' is one, regardless of the currently installed controller.  If
		''' there is no default controller, returns <code>null</code>.
		''' 
		''' <p> The default implementation returns the value of the
		''' <code>defaultController</code> instance variable.
		''' </summary>
		''' <returns> the default <code>IIOMetadataController</code>, or
		''' <code>null</code>.
		''' </returns>
		''' <seealso cref= IIOMetadataController </seealso>
		''' <seealso cref= #setController(IIOMetadataController) </seealso>
		''' <seealso cref= #getController </seealso>
		''' <seealso cref= #hasController </seealso>
		''' <seealso cref= #activateController() </seealso>
		Public Overridable Property defaultController As IIOMetadataController
			Get
				Return defaultController
			End Get
		End Property

		''' <summary>
		''' Returns <code>true</code> if there is a controller installed
		''' for this <code>IIOMetadata</code> object.
		''' 
		''' <p> The default implementation returns <code>true</code> if the
		''' <code>getController</code> method returns a
		''' non-<code>null</code> value.
		''' </summary>
		''' <returns> <code>true</code> if a controller is installed.
		''' </returns>
		''' <seealso cref= IIOMetadataController </seealso>
		''' <seealso cref= #setController(IIOMetadataController) </seealso>
		''' <seealso cref= #getController </seealso>
		''' <seealso cref= #getDefaultController </seealso>
		''' <seealso cref= #activateController() </seealso>
		Public Overridable Function hasController() As Boolean
			Return (controller IsNot Nothing)
		End Function

		''' <summary>
		''' Activates the installed <code>IIOMetadataController</code> for
		''' this <code>IIOMetadata</code> object and returns the resulting
		''' value.  When this method returns <code>true</code>, all values for this
		''' <code>IIOMetadata</code> object will be ready for the next write
		''' operation.  If <code>false</code> is
		''' returned, no settings in this object will have been disturbed
		''' (<i>i.e.</i>, the user canceled the operation).
		''' 
		''' <p> Ordinarily, the controller will be a GUI providing a user
		''' interface for a subclass of <code>IIOMetadata</code> for a
		''' particular plug-in.  Controllers need not be GUIs, however.
		''' 
		''' <p> The default implementation calls <code>getController</code>
		''' and the calls <code>activate</code> on the returned object if
		''' <code>hasController</code> returns <code>true</code>.
		''' </summary>
		''' <returns> <code>true</code> if the controller completed normally.
		''' </returns>
		''' <exception cref="IllegalStateException"> if there is no controller
		''' currently installed.
		''' </exception>
		''' <seealso cref= IIOMetadataController </seealso>
		''' <seealso cref= #setController(IIOMetadataController) </seealso>
		''' <seealso cref= #getController </seealso>
		''' <seealso cref= #getDefaultController </seealso>
		''' <seealso cref= #hasController </seealso>
		Public Overridable Function activateController() As Boolean
			If Not hasController() Then Throw New IllegalStateException("hasController() == false!")
			Return controller.activate(Me)
		End Function
	End Class

End Namespace