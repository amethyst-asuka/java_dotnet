Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections
Imports System.Threading
import static sun.security.util.SecurityConstants.GET_CLASSLOADER_PERMISSION

'
' * Copyright (c) 1996, 2014, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt.datatransfer



	''' <summary>
	''' A {@code DataFlavor} provides meta information about data. {@code DataFlavor}
	''' is typically used to access data on the clipboard, or during
	''' a drag and drop operation.
	''' <p>
	''' An instance of {@code DataFlavor} encapsulates a content type as
	''' defined in <a href="http://www.ietf.org/rfc/rfc2045.txt">RFC 2045</a>
	''' and <a href="http://www.ietf.org/rfc/rfc2046.txt">RFC 2046</a>.
	''' A content type is typically referred to as a MIME type.
	''' <p>
	''' A content type consists of a media type (referred
	''' to as the primary type), a subtype, and optional parameters. See
	''' <a href="http://www.ietf.org/rfc/rfc2045.txt">RFC 2045</a>
	''' for details on the syntax of a MIME type.
	''' <p>
	''' The JRE data transfer implementation interprets the parameter &quot;class&quot;
	''' of a MIME type as <B>a representation class</b>.
	''' The representation class reflects the class of the object being
	''' transferred. In other words, the representation class is the type of
	''' object returned by <seealso cref="Transferable#getTransferData"/>.
	''' For example, the MIME type of <seealso cref="#imageFlavor"/> is
	''' {@code "image/x-java-image;class=java.awt.Image"},
	''' the primary type is {@code image}, the subtype is
	''' {@code x-java-image}, and the representation class is
	''' {@code java.awt.Image}. When {@code getTransferData} is invoked
	''' with a {@code DataFlavor} of {@code imageFlavor}, an instance of
	''' {@code java.awt.Image} is returned.
	''' It's important to note that {@code DataFlavor} does no error checking
	''' against the representation class. It is up to consumers of
	''' {@code DataFlavor}, such as {@code Transferable}, to honor the representation
	''' class.
	''' <br>
	''' Note, if you do not specify a representation class when
	''' creating a {@code DataFlavor}, the default
	''' representation class is used. See appropriate documentation for
	''' {@code DataFlavor}'s constructors.
	''' <p>
	''' Also, {@code DataFlavor} instances with the &quot;text&quot; primary
	''' MIME type may have a &quot;charset&quot; parameter. Refer to
	''' <a href="http://www.ietf.org/rfc/rfc2046.txt">RFC 2046</a> and
	''' <seealso cref="#selectBestTextFlavor"/> for details on &quot;text&quot; MIME types
	''' and the &quot;charset&quot; parameter.
	''' <p>
	''' Equality of {@code DataFlavors} is determined by the primary type,
	''' subtype, and representation class. Refer to <seealso cref="#equals(DataFlavor)"/> for
	''' details. When determining equality, any optional parameters are ignored.
	''' For example, the following produces two {@code DataFlavors} that
	''' are considered identical:
	''' <pre>
	'''   DataFlavor flavor1 = new DataFlavor(Object.class, &quot;X-test/test; class=&lt;java.lang.Object&gt;; foo=bar&quot;);
	'''   DataFlavor flavor2 = new DataFlavor(Object.class, &quot;X-test/test; class=&lt;java.lang.Object&gt;; x=y&quot;);
	'''   // The following returns true.
	'''   flavor1.equals(flavor2);
	''' </pre>
	''' As mentioned, {@code flavor1} and {@code flavor2} are considered identical.
	''' As such, asking a {@code Transferable} for either {@code DataFlavor} returns
	''' the same results.
	''' <p>
	''' For more information on the using data transfer with Swing see
	''' the <a href="https://docs.oracle.com/javase/tutorial/uiswing/dnd/index.html">
	''' How to Use Drag and Drop and Data Transfer</a>,
	''' section in <em>Java Tutorial</em>.
	''' 
	''' @author      Blake Sullivan
	''' @author      Laurence P. G. Cable
	''' @author      Jeff Dunn
	''' </summary>
	Public Class DataFlavor
		Implements java.io.Externalizable, Cloneable

		Private Const serialVersionUID As Long = 8367026044764648243L
		Private Shared ReadOnly ioInputStreamClass As  [Class] = GetType(java.io.InputStream)

		''' <summary>
		''' Tries to load a class from: the bootstrap loader, the system loader,
		''' the context loader (if one is present) and finally the loader specified.
		''' </summary>
		''' <param name="className"> the name of the class to be loaded </param>
		''' <param name="fallback"> the fallback loader </param>
		''' <returns> the class loaded </returns>
		''' <exception cref="ClassNotFoundException"> if class is not found </exception>
		Protected Friend Shared Function tryToLoadClass(ByVal className As String, ByVal fallback As  ClassLoader) As  [Class]
			sun.reflect.misc.ReflectUtil.checkPackageAccess(className)
			Try
				Dim sm As SecurityManager = System.securityManager
				If sm IsNot Nothing Then sm.checkPermission(GET_CLASSLOADER_PERMISSION)
				Dim loader As  ClassLoader = ClassLoader.systemClassLoader
				Try
					' bootstrap class loader and system class loader if present
					Return Type.GetType(className, True, loader)
				Catch exception_Renamed As  ClassNotFoundException
					' thread context class loader if and only if present
					loader = Thread.CurrentThread.contextClassLoader
					If loader IsNot Nothing Then
						Try
							Return Type.GetType(className, True, loader)
						Catch e As  ClassNotFoundException
							' fallback to user's class loader
						End Try
					End If
				End Try
			Catch exception_Renamed As SecurityException
				' ignore secured class loaders
			End Try
			Return Type.GetType(className, True, fallback)
		End Function

	'    
	'     * private initializer
	'     
		Private Shared Function createConstant(ByVal rc As [Class], ByVal prn As String) As DataFlavor
			Try
				Return New DataFlavor(rc, prn)
			Catch e As Exception
				Return Nothing
			End Try
		End Function

	'    
	'     * private initializer
	'     
		Private Shared Function createConstant(ByVal mt As String, ByVal prn As String) As DataFlavor
			Try
				Return New DataFlavor(mt, prn)
			Catch e As Exception
				Return Nothing
			End Try
		End Function

	'    
	'     * private initializer
	'     
		Private Shared Function initHtmlDataFlavor(ByVal htmlFlavorType As String) As DataFlavor
			Try
				Return New DataFlavor("text/html; class=java.lang.String;document=" & htmlFlavorType & ";charset=Unicode")
			Catch e As Exception
				Return Nothing
			End Try
		End Function

		''' <summary>
		''' The <code>DataFlavor</code> representing a Java Unicode String [Class],
		''' where:
		''' <pre>
		'''     representationClass = java.lang.String
		'''     mimeType           = "application/x-java-serialized-object"
		''' </pre>
		''' </summary>
		Public Shared ReadOnly stringFlavor As DataFlavor = createConstant(GetType(String), "Unicode String")

		''' <summary>
		''' The <code>DataFlavor</code> representing a Java Image [Class],
		''' where:
		''' <pre>
		'''     representationClass = java.awt.Image
		'''     mimeType            = "image/x-java-image"
		''' </pre>
		''' </summary>
		Public Shared ReadOnly imageFlavor As DataFlavor = createConstant("image/x-java-image; class=java.awt.Image", "Image")

		''' <summary>
		''' The <code>DataFlavor</code> representing plain text with Unicode
		''' encoding, where:
		''' <pre>
		'''     representationClass = InputStream
		'''     mimeType            = "text/plain; charset=unicode"
		''' </pre>
		''' This <code>DataFlavor</code> has been <b>deprecated</b> because
		''' (1) Its representation is an InputStream, an 8-bit based representation,
		''' while Unicode is a 16-bit character set; and (2) The charset "unicode"
		''' is not well-defined. "unicode" implies a particular platform's
		''' implementation of Unicode, not a cross-platform implementation.
		''' </summary>
		''' @deprecated as of 1.3. Use <code>DataFlavor.getReaderForText(Transferable)</code>
		'''             instead of <code>Transferable.getTransferData(DataFlavor.plainTextFlavor)</code>. 
		<Obsolete("as of 1.3. Use <code>DataFlavor.getReaderForText(Transferable)</code>")> _
		Public Shared ReadOnly plainTextFlavor As DataFlavor = createConstant("text/plain; charset=unicode; class=java.io.InputStream", "Plain Text")

		''' <summary>
		''' A MIME Content-Type of application/x-java-serialized-object represents
		''' a graph of Java object(s) that have been made persistent.
		''' 
		''' The representation class associated with this <code>DataFlavor</code>
		''' identifies the Java type of an object returned as a reference
		''' from an invocation <code>java.awt.datatransfer.getTransferData</code>.
		''' </summary>
		Public Const javaSerializedObjectMimeType As String = "application/x-java-serialized-object"

		''' <summary>
		''' To transfer a list of files to/from Java (and the underlying
		''' platform) a <code>DataFlavor</code> of this type/subtype and
		''' representation class of <code>java.util.List</code> is used.
		''' Each element of the list is required/guaranteed to be of type
		''' <code>java.io.File</code>.
		''' </summary>
		Public Shared ReadOnly javaFileListFlavor As DataFlavor = createConstant("application/x-java-file-list;class=java.util.List", Nothing)

		''' <summary>
		''' To transfer a reference to an arbitrary Java object reference that
		''' has no associated MIME Content-type, across a <code>Transferable</code>
		''' interface WITHIN THE SAME JVM, a <code>DataFlavor</code>
		''' with this type/subtype is used, with a <code>representationClass</code>
		''' equal to the type of the class/interface being passed across the
		''' <code>Transferable</code>.
		''' <p>
		''' The object reference returned from
		''' <code>Transferable.getTransferData</code> for a <code>DataFlavor</code>
		''' with this MIME Content-Type is required to be
		''' an instance of the representation Class of the <code>DataFlavor</code>.
		''' </summary>
		Public Const javaJVMLocalObjectMimeType As String = "application/x-java-jvm-local-objectref"

		''' <summary>
		''' In order to pass a live link to a Remote object via a Drag and Drop
		''' <code>ACTION_LINK</code> operation a Mime Content Type of
		''' application/x-java-remote-object should be used,
		''' where the representation class of the <code>DataFlavor</code>
		''' represents the type of the <code>Remote</code> interface to be
		''' transferred.
		''' </summary>
		Public Const javaRemoteObjectMimeType As String = "application/x-java-remote-object"

		''' <summary>
		''' Represents a piece of an HTML markup. The markup consists of the part
		''' selected on the source side. Therefore some tags in the markup may be
		''' unpaired. If the flavor is used to represent the data in
		''' a <seealso cref="Transferable"/> instance, no additional changes will be made.
		''' This DataFlavor instance represents the same HTML markup as DataFlavor
		''' instances which content MIME type does not contain document parameter
		''' and representation class is the String class.
		''' <pre>
		'''     representationClass = String
		'''     mimeType           = "text/html"
		''' </pre>
		''' </summary>
		Public Shared selectionHtmlFlavor As DataFlavor = initHtmlDataFlavor("selection")

		''' <summary>
		''' Represents a piece of an HTML markup. If possible, the markup received
		''' from a native system is supplemented with pair tags to be
		''' a well-formed HTML markup. If the flavor is used to represent the data in
		''' a <seealso cref="Transferable"/> instance, no additional changes will be made.
		''' <pre>
		'''     representationClass = String
		'''     mimeType           = "text/html"
		''' </pre>
		''' </summary>
		Public Shared fragmentHtmlFlavor As DataFlavor = initHtmlDataFlavor("fragment")

		''' <summary>
		''' Represents a piece of an HTML markup. If possible, the markup
		''' received from a native system is supplemented with additional
		''' tags to make up a well-formed HTML document. If the flavor is used to
		''' represent the data in a <seealso cref="Transferable"/> instance,
		''' no additional changes will be made.
		''' <pre>
		'''     representationClass = String
		'''     mimeType           = "text/html"
		''' </pre>
		''' </summary>
		Public Shared allHtmlFlavor As DataFlavor = initHtmlDataFlavor("all")

		''' <summary>
		''' Constructs a new <code>DataFlavor</code>.  This constructor is
		''' provided only for the purpose of supporting the
		''' <code>Externalizable</code> interface.  It is not
		''' intended for public (client) use.
		''' 
		''' @since 1.2
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a fully specified <code>DataFlavor</code>.
		''' </summary>
		''' <exception cref="NullPointerException"> if either <code>primaryType</code>,
		'''            <code>subType</code> or <code>representationClass</code> is null </exception>
		Private Sub New(ByVal primaryType As String, ByVal subType As String, ByVal params As MimeTypeParameterList, ByVal representationClass As [Class], ByVal humanPresentableName As String)
			MyBase.New()
			If primaryType Is Nothing Then Throw New NullPointerException("primaryType")
			If subType Is Nothing Then Throw New NullPointerException("subType")
			If representationClass Is Nothing Then Throw New NullPointerException("representationClass")

			If params Is Nothing Then params = New MimeTypeParameterList

			params.set("class", representationClass.name)

			If humanPresentableName Is Nothing Then
				humanPresentableName = params.get("humanPresentableName")

				If humanPresentableName Is Nothing Then humanPresentableName = primaryType & "/" & subType
			End If

			Try
				mimeType = New MimeType(primaryType, subType, params)
			Catch mtpe As MimeTypeParseException
				Throw New IllegalArgumentException("MimeType Parse Exception: " & mtpe.Message)
			End Try

			Me.representationClass = representationClass
			Me.humanPresentableName = humanPresentableName

			mimeType.removeParameter("humanPresentableName")
		End Sub

		''' <summary>
		''' Constructs a <code>DataFlavor</code> that represents a Java class.
		''' <p>
		''' The returned <code>DataFlavor</code> will have the following
		''' characteristics:
		''' <pre>
		'''    representationClass = representationClass
		'''    mimeType            = application/x-java-serialized-object
		''' </pre> </summary>
		''' <param name="representationClass"> the class used to transfer data in this flavor </param>
		''' <param name="humanPresentableName"> the human-readable string used to identify
		'''                 this flavor; if this parameter is <code>null</code>
		'''                 then the value of the the MIME Content Type is used </param>
		''' <exception cref="NullPointerException"> if <code>representationClass</code> is null </exception>
		Public Sub New(ByVal representationClass As [Class], ByVal humanPresentableName As String)
			Me.New("application", "x-java-serialized-object", Nothing, representationClass, humanPresentableName)
			If representationClass Is Nothing Then Throw New NullPointerException("representationClass")
		End Sub

		''' <summary>
		''' Constructs a <code>DataFlavor</code> that represents a
		''' <code>MimeType</code>.
		''' <p>
		''' The returned <code>DataFlavor</code> will have the following
		''' characteristics:
		''' <p>
		''' If the <code>mimeType</code> is
		''' "application/x-java-serialized-object; class=&lt;representation class&gt;",
		''' the result is the same as calling
		''' <code>new DataFlavor(Class:forName(&lt;representation class&gt;)</code>.
		''' <p>
		''' Otherwise:
		''' <pre>
		'''     representationClass = InputStream
		'''     mimeType            = mimeType
		''' </pre> </summary>
		''' <param name="mimeType"> the string used to identify the MIME type for this flavor;
		'''                 if the the <code>mimeType</code> does not specify a
		'''                 "class=" parameter, or if the class is not successfully
		'''                 loaded, then an <code>IllegalArgumentException</code>
		'''                 is thrown </param>
		''' <param name="humanPresentableName"> the human-readable string used to identify
		'''                 this flavor; if this parameter is <code>null</code>
		'''                 then the value of the the MIME Content Type is used </param>
		''' <exception cref="IllegalArgumentException"> if <code>mimeType</code> is
		'''                 invalid or if the class is not successfully loaded </exception>
		''' <exception cref="NullPointerException"> if <code>mimeType</code> is null </exception>
		Public Sub New(ByVal mimeType_Renamed As String, ByVal humanPresentableName As String)
			MyBase.New()
			If mimeType_Renamed Is Nothing Then Throw New NullPointerException("mimeType")
			Try
				initialize(mimeType_Renamed, humanPresentableName, Me.GetType().classLoader)
			Catch mtpe As MimeTypeParseException
				Throw New IllegalArgumentException("failed to parse:" & mimeType_Renamed)
			Catch cnfe As  ClassNotFoundException
				Throw New IllegalArgumentException("can't find specified class: " & cnfe.Message)
			End Try
		End Sub

		''' <summary>
		''' Constructs a <code>DataFlavor</code> that represents a
		''' <code>MimeType</code>.
		''' <p>
		''' The returned <code>DataFlavor</code> will have the following
		''' characteristics:
		''' <p>
		''' If the mimeType is
		''' "application/x-java-serialized-object; class=&lt;representation class&gt;",
		''' the result is the same as calling
		''' <code>new DataFlavor(Class:forName(&lt;representation class&gt;)</code>.
		''' <p>
		''' Otherwise:
		''' <pre>
		'''     representationClass = InputStream
		'''     mimeType            = mimeType
		''' </pre> </summary>
		''' <param name="mimeType"> the string used to identify the MIME type for this flavor </param>
		''' <param name="humanPresentableName"> the human-readable string used to
		'''          identify this flavor </param>
		''' <param name="classLoader"> the class loader to use </param>
		''' <exception cref="ClassNotFoundException"> if the class is not loaded </exception>
		''' <exception cref="IllegalArgumentException"> if <code>mimeType</code> is
		'''                 invalid </exception>
		''' <exception cref="NullPointerException"> if <code>mimeType</code> is null </exception>
		Public Sub New(ByVal mimeType_Renamed As String, ByVal humanPresentableName As String, ByVal classLoader_Renamed As  ClassLoader)
			MyBase.New()
			If mimeType_Renamed Is Nothing Then Throw New NullPointerException("mimeType")
			Try
				initialize(mimeType_Renamed, humanPresentableName, classLoader_Renamed)
			Catch mtpe As MimeTypeParseException
				Throw New IllegalArgumentException("failed to parse:" & mimeType_Renamed)
			End Try
		End Sub

		''' <summary>
		''' Constructs a <code>DataFlavor</code> from a <code>mimeType</code> string.
		''' The string can specify a "class=&lt;fully specified Java class name&gt;"
		''' parameter to create a <code>DataFlavor</code> with the desired
		''' representation class. If the string does not contain "class=" parameter,
		''' <code>java.io.InputStream</code> is used as default.
		''' </summary>
		''' <param name="mimeType"> the string used to identify the MIME type for this flavor;
		'''                 if the class specified by "class=" parameter is not
		'''                 successfully loaded, then an
		'''                 <code>ClassNotFoundException</code> is thrown </param>
		''' <exception cref="ClassNotFoundException"> if the class is not loaded </exception>
		''' <exception cref="IllegalArgumentException"> if <code>mimeType</code> is
		'''                 invalid </exception>
		''' <exception cref="NullPointerException"> if <code>mimeType</code> is null </exception>
		Public Sub New(ByVal mimeType_Renamed As String)
			MyBase.New()
			If mimeType_Renamed Is Nothing Then Throw New NullPointerException("mimeType")
			Try
				initialize(mimeType_Renamed, Nothing, Me.GetType().classLoader)
			Catch mtpe As MimeTypeParseException
				Throw New IllegalArgumentException("failed to parse:" & mimeType_Renamed)
			End Try
		End Sub

	   ''' <summary>
	   ''' Common initialization code called from various constructors.
	   ''' </summary>
	   ''' <param name="mimeType"> the MIME Content Type (must have a class= param) </param>
	   ''' <param name="humanPresentableName"> the human Presentable Name or
	   '''                 <code>null</code> </param>
	   ''' <param name="classLoader"> the fallback class loader to resolve against
	   ''' </param>
	   ''' <exception cref="MimeTypeParseException"> </exception>
	   ''' <exception cref="ClassNotFoundException"> </exception>
	   ''' <exception cref="NullPointerException"> if <code>mimeType</code> is null
	   ''' </exception>
	   ''' <seealso cref= #tryToLoadClass </seealso>
		Private Sub initialize(ByVal mimeType_Renamed As String, ByVal humanPresentableName As String, ByVal classLoader_Renamed As  ClassLoader)
			If mimeType_Renamed Is Nothing Then Throw New NullPointerException("mimeType")

			Me.mimeType = New MimeType(mimeType_Renamed) ' throws

			Dim rcn As String = getParameter("class")

			If rcn Is Nothing Then
				If "application/x-java-serialized-object".Equals(Me.mimeType.baseType) Then

					Throw New IllegalArgumentException("no representation class specified for:" & mimeType_Renamed)
				Else
					representationClass = GetType(java.io.InputStream) ' default
				End If ' got a class name
			Else
				representationClass = DataFlavor.tryToLoadClass(rcn, classLoader_Renamed)
			End If

			Me.mimeType.parameterter("class", representationClass.name)

			If humanPresentableName Is Nothing Then
				humanPresentableName = Me.mimeType.getParameter("humanPresentableName")
				If humanPresentableName Is Nothing Then humanPresentableName = Me.mimeType.primaryType & "/" & Me.mimeType.subType
			End If

			Me.humanPresentableName = humanPresentableName ' set it.

			Me.mimeType.removeParameter("humanPresentableName") ' just in case
		End Sub

		''' <summary>
		''' String representation of this <code>DataFlavor</code> and its
		''' parameters. The resulting <code>String</code> contains the name of
		''' the <code>DataFlavor</code> [Class], this flavor's MIME type, and its
		''' representation class. If this flavor has a primary MIME type of "text",
		''' supports the charset parameter, and has an encoded representation, the
		''' flavor's charset is also included. See <code>selectBestTextFlavor</code>
		''' for a list of text flavors which support the charset parameter.
		''' </summary>
		''' <returns>  string representation of this <code>DataFlavor</code> </returns>
		''' <seealso cref= #selectBestTextFlavor </seealso>
		Public Overrides Function ToString() As String
			Dim string_Renamed As String = Me.GetType().name
			string_Renamed &= "[" & paramString() & "]"
			Return string_Renamed
		End Function

		Private Function paramString() As String
			Dim params As String = ""
			params &= "mimetype="
			If mimeType Is Nothing Then
				params &= "null"
			Else
				params += mimeType.baseType
			End If
			params &= ";representationclass="
			If representationClass Is Nothing Then
			   params &= "null"
			Else
			   params += representationClass.name
			End If
			If sun.awt.datatransfer.DataTransferer.isFlavorCharsetTextType(Me) AndAlso (representationClassInputStream OrElse representationClassByteBuffer OrElse GetType(SByte()).Equals(representationClass)) Then params &= ";charset=" & sun.awt.datatransfer.DataTransferer.getTextCharset(Me)
			Return params
		End Function

		''' <summary>
		''' Returns a <code>DataFlavor</code> representing plain text with Unicode
		''' encoding, where:
		''' <pre>
		'''     representationClass = java.io.InputStream
		'''     mimeType            = "text/plain;
		'''                            charset=&lt;platform default Unicode encoding&gt;"
		''' </pre>
		''' Sun's implementation for Microsoft Windows uses the encoding <code>utf-16le</code>.
		''' Sun's implementation for Solaris and Linux uses the encoding
		''' <code>iso-10646-ucs-2</code>.
		''' </summary>
		''' <returns> a <code>DataFlavor</code> representing plain text
		'''    with Unicode encoding
		''' @since 1.3 </returns>
		Public Property Shared textPlainUnicodeFlavor As DataFlavor
			Get
				Dim encoding As String = Nothing
				Dim transferer As sun.awt.datatransfer.DataTransferer = sun.awt.datatransfer.DataTransferer.instance
				If transferer IsNot Nothing Then encoding = transferer.defaultUnicodeEncoding
				Return New DataFlavor("text/plain;charset=" & encoding & ";class=java.io.InputStream", "Plain Text")
			End Get
		End Property

		''' <summary>
		''' Selects the best text <code>DataFlavor</code> from an array of <code>
		''' DataFlavor</code>s. Only <code>DataFlavor.stringFlavor</code>, and
		''' equivalent flavors, and flavors that have a primary MIME type of "text",
		''' are considered for selection.
		''' <p>
		''' Flavors are first sorted by their MIME types in the following order:
		''' <ul>
		''' <li>"text/sgml"
		''' <li>"text/xml"
		''' <li>"text/html"
		''' <li>"text/rtf"
		''' <li>"text/enriched"
		''' <li>"text/richtext"
		''' <li>"text/uri-list"
		''' <li>"text/tab-separated-values"
		''' <li>"text/t140"
		''' <li>"text/rfc822-headers"
		''' <li>"text/parityfec"
		''' <li>"text/directory"
		''' <li>"text/css"
		''' <li>"text/calendar"
		''' <li>"application/x-java-serialized-object"
		''' <li>"text/plain"
		''' <li>"text/&lt;other&gt;"
		''' </ul>
		''' <p>For example, "text/sgml" will be selected over
		''' "text/html", and <code>DataFlavor.stringFlavor</code> will be chosen
		''' over <code>DataFlavor.plainTextFlavor</code>.
		''' <p>
		''' If two or more flavors share the best MIME type in the array, then that
		''' MIME type will be checked to see if it supports the charset parameter.
		''' <p>
		''' The following MIME types support, or are treated as though they support,
		''' the charset parameter:
		''' <ul>
		''' <li>"text/sgml"
		''' <li>"text/xml"
		''' <li>"text/html"
		''' <li>"text/enriched"
		''' <li>"text/richtext"
		''' <li>"text/uri-list"
		''' <li>"text/directory"
		''' <li>"text/css"
		''' <li>"text/calendar"
		''' <li>"application/x-java-serialized-object"
		''' <li>"text/plain"
		''' </ul>
		''' The following MIME types do not support, or are treated as though they
		''' do not support, the charset parameter:
		''' <ul>
		''' <li>"text/rtf"
		''' <li>"text/tab-separated-values"
		''' <li>"text/t140"
		''' <li>"text/rfc822-headers"
		''' <li>"text/parityfec"
		''' </ul>
		''' For "text/&lt;other&gt;" MIME types, the first time the JRE needs to
		''' determine whether the MIME type supports the charset parameter, it will
		''' check whether the parameter is explicitly listed in an arbitrarily
		''' chosen <code>DataFlavor</code> which uses that MIME type. If so, the JRE
		''' will assume from that point on that the MIME type supports the charset
		''' parameter and will not check again. If the parameter is not explicitly
		''' listed, the JRE will assume from that point on that the MIME type does
		''' not support the charset parameter and will not check again. Because
		''' this check is performed on an arbitrarily chosen
		''' <code>DataFlavor</code>, developers must ensure that all
		''' <code>DataFlavor</code>s with a "text/&lt;other&gt;" MIME type specify
		''' the charset parameter if it is supported by that MIME type. Developers
		''' should never rely on the JRE to substitute the platform's default
		''' charset for a "text/&lt;other&gt;" DataFlavor. Failure to adhere to this
		''' restriction will lead to undefined behavior.
		''' <p>
		''' If the best MIME type in the array does not support the charset
		''' parameter, the flavors which share that MIME type will then be sorted by
		''' their representation classes in the following order:
		''' <code>java.io.InputStream</code>, <code>java.nio.ByteBuffer</code>,
		''' <code>[B</code>, &lt;all others&gt;.
		''' <p>
		''' If two or more flavors share the best representation [Class], or if no
		''' flavor has one of the three specified representations, then one of those
		''' flavors will be chosen non-deterministically.
		''' <p>
		''' If the best MIME type in the array does support the charset parameter,
		''' the flavors which share that MIME type will then be sorted by their
		''' representation classes in the following order:
		''' <code>java.io.Reader</code>, <code>java.lang.String</code>,
		''' <code>java.nio.CharBuffer</code>, <code>[C</code>, &lt;all others&gt;.
		''' <p>
		''' If two or more flavors share the best representation [Class], and that
		''' representation is one of the four explicitly listed, then one of those
		''' flavors will be chosen non-deterministically. If, however, no flavor has
		''' one of the four specified representations, the flavors will then be
		''' sorted by their charsets. Unicode charsets, such as "UTF-16", "UTF-8",
		''' "UTF-16BE", "UTF-16LE", and their aliases, are considered best. After
		''' them, the platform default charset and its aliases are selected.
		''' "US-ASCII" and its aliases are worst. All other charsets are chosen in
		''' alphabetical order, but only charsets supported by this implementation
		''' of the Java platform will be considered.
		''' <p>
		''' If two or more flavors share the best charset, the flavors will then
		''' again be sorted by their representation classes in the following order:
		''' <code>java.io.InputStream</code>, <code>java.nio.ByteBuffer</code>,
		''' <code>[B</code>, &lt;all others&gt;.
		''' <p>
		''' If two or more flavors share the best representation [Class], or if no
		''' flavor has one of the three specified representations, then one of those
		''' flavors will be chosen non-deterministically.
		''' </summary>
		''' <param name="availableFlavors"> an array of available <code>DataFlavor</code>s </param>
		''' <returns> the best (highest fidelity) flavor according to the rules
		'''         specified above, or <code>null</code>,
		'''         if <code>availableFlavors</code> is <code>null</code>,
		'''         has zero length, or contains no text flavors
		''' @since 1.3 </returns>
		Public Shared Function selectBestTextFlavor(ByVal availableFlavors As DataFlavor()) As DataFlavor
			If availableFlavors Is Nothing OrElse availableFlavors.Length = 0 Then Return Nothing

			If textFlavorComparator Is Nothing Then textFlavorComparator = New TextFlavorComparator

			Dim bestFlavor As DataFlavor = CType(java.util.Collections.max(java.util.Arrays.asList(availableFlavors), textFlavorComparator), DataFlavor)

			If Not bestFlavor.flavorTextType Then Return Nothing

			Return bestFlavor
		End Function

		Private Shared textFlavorComparator As IComparer(Of DataFlavor)

		Friend Class TextFlavorComparator
			Inherits sun.awt.datatransfer.DataTransferer.DataFlavorComparator

			''' <summary>
			''' Compares two <code>DataFlavor</code> objects. Returns a negative
			''' integer, zero, or a positive integer as the first
			''' <code>DataFlavor</code> is worse than, equal to, or better than the
			''' second.
			''' <p>
			''' <code>DataFlavor</code>s are ordered according to the rules outlined
			''' for <code>selectBestTextFlavor</code>.
			''' </summary>
			''' <param name="obj1"> the first <code>DataFlavor</code> to be compared </param>
			''' <param name="obj2"> the second <code>DataFlavor</code> to be compared </param>
			''' <returns> a negative integer, zero, or a positive integer as the first
			'''         argument is worse, equal to, or better than the second </returns>
			''' <exception cref="ClassCastException"> if either of the arguments is not an
			'''         instance of <code>DataFlavor</code> </exception>
			''' <exception cref="NullPointerException"> if either of the arguments is
			'''         <code>null</code>
			''' </exception>
			''' <seealso cref= #selectBestTextFlavor </seealso>
			Public Overridable Function compare(ByVal obj1 As Object, ByVal obj2 As Object) As Integer
				Dim flavor1 As DataFlavor = CType(obj1, DataFlavor)
				Dim flavor2 As DataFlavor = CType(obj2, DataFlavor)

				If flavor1.flavorTextType Then
					If flavor2.flavorTextType Then
						Return MyBase.Compare(obj1, obj2)
					Else
						Return 1
					End If
				ElseIf flavor2.flavorTextType Then
					Return -1
				Else
					Return 0
				End If
			End Function
		End Class

		''' <summary>
		''' Gets a Reader for a text flavor, decoded, if necessary, for the expected
		''' charset (encoding). The supported representation classes are
		''' <code>java.io.Reader</code>, <code>java.lang.String</code>,
		''' <code>java.nio.CharBuffer</code>, <code>[C</code>,
		''' <code>java.io.InputStream</code>, <code>java.nio.ByteBuffer</code>,
		''' and <code>[B</code>.
		''' <p>
		''' Because text flavors which do not support the charset parameter are
		''' encoded in a non-standard format, this method should not be called for
		''' such flavors. However, in order to maintain backward-compatibility,
		''' if this method is called for such a flavor, this method will treat the
		''' flavor as though it supports the charset parameter and attempt to
		''' decode it accordingly. See <code>selectBestTextFlavor</code> for a list
		''' of text flavors which do not support the charset parameter.
		''' </summary>
		''' <param name="transferable"> the <code>Transferable</code> whose data will be
		'''        requested in this flavor
		''' </param>
		''' <returns> a <code>Reader</code> to read the <code>Transferable</code>'s
		'''         data
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if the representation class
		'''            is not one of the seven listed above </exception>
		''' <exception cref="IllegalArgumentException"> if the <code>Transferable</code>
		'''            has <code>null</code> data </exception>
		''' <exception cref="NullPointerException"> if the <code>Transferable</code> is
		'''            <code>null</code> </exception>
		''' <exception cref="UnsupportedEncodingException"> if this flavor's representation
		'''            is <code>java.io.InputStream</code>,
		'''            <code>java.nio.ByteBuffer</code>, or <code>[B</code> and
		'''            this flavor's encoding is not supported by this
		'''            implementation of the Java platform </exception>
		''' <exception cref="UnsupportedFlavorException"> if the <code>Transferable</code>
		'''            does not support this flavor </exception>
		''' <exception cref="IOException"> if the data cannot be read because of an
		'''            I/O error </exception>
		''' <seealso cref= #selectBestTextFlavor
		''' @since 1.3 </seealso>
		Public Overridable Function getReaderForText(ByVal transferable As Transferable) As java.io.Reader
			Dim transferObject As Object = transferable.getTransferData(Me)
			If transferObject Is Nothing Then Throw New IllegalArgumentException("getTransferData() returned null")

			If TypeOf transferObject Is java.io.Reader Then
				Return CType(transferObject, java.io.Reader)
			ElseIf TypeOf transferObject Is String Then
				Return New java.io.StringReader(CStr(transferObject))
			ElseIf TypeOf transferObject Is java.nio.CharBuffer Then
				Dim buffer As java.nio.CharBuffer = CType(transferObject, java.nio.CharBuffer)
				Dim size As Integer = buffer.remaining()
				Dim chars As Char() = New Char(size - 1){}
				buffer.get(chars, 0, size)
				Return New java.io.CharArrayReader(chars)
			ElseIf TypeOf transferObject Is Char() Then
				Return New java.io.CharArrayReader(CType(transferObject, Char()))
			End If

			Dim stream As java.io.InputStream = Nothing

			If TypeOf transferObject Is java.io.InputStream Then
				stream = CType(transferObject, java.io.InputStream)
			ElseIf TypeOf transferObject Is java.nio.ByteBuffer Then
				Dim buffer As java.nio.ByteBuffer = CType(transferObject, java.nio.ByteBuffer)
				Dim size As Integer = buffer.remaining()
				Dim bytes As SByte() = New SByte(size - 1){}
				buffer.get(bytes, 0, size)
				stream = New java.io.ByteArrayInputStream(bytes)
			ElseIf TypeOf transferObject Is SByte() Then
				stream = New java.io.ByteArrayInputStream(CType(transferObject, SByte()))
			End If

			If stream Is Nothing Then Throw New IllegalArgumentException("transfer data is not Reader, String, CharBuffer, char array, InputStream, ByteBuffer, or byte array")

			Dim encoding As String = getParameter("charset")
			Return If(encoding Is Nothing, New java.io.InputStreamReader(stream), New java.io.InputStreamReader(stream, encoding))
		End Function

		''' <summary>
		''' Returns the MIME type string for this <code>DataFlavor</code>. </summary>
		''' <returns> the MIME type string for this flavor </returns>
		Public Overridable Property mimeType As String
			Get
				Return If(mimeType IsNot Nothing, mimeType.ToString(), Nothing)
			End Get
		End Property

		''' <summary>
		''' Returns the <code>Class</code> which objects supporting this
		''' <code>DataFlavor</code> will return when this <code>DataFlavor</code>
		''' is requested. </summary>
		''' <returns> the <code>Class</code> which objects supporting this
		''' <code>DataFlavor</code> will return when this <code>DataFlavor</code>
		''' is requested </returns>
		Public Overridable Property representationClass As  [Class]
			Get
				Return representationClass
			End Get
		End Property

		''' <summary>
		''' Returns the human presentable name for the data format that this
		''' <code>DataFlavor</code> represents.  This name would be localized
		''' for different countries. </summary>
		''' <returns> the human presentable name for the data format that this
		'''    <code>DataFlavor</code> represents </returns>
		Public Overridable Property humanPresentableName As String
			Get
				Return humanPresentableName
			End Get
			Set(ByVal humanPresentableName As String)
				Me.humanPresentableName = humanPresentableName
			End Set
		End Property

		''' <summary>
		''' Returns the primary MIME type for this <code>DataFlavor</code>. </summary>
		''' <returns> the primary MIME type of this <code>DataFlavor</code> </returns>
		Public Overridable Property primaryType As String
			Get
				Return If(mimeType IsNot Nothing, mimeType.primaryType, Nothing)
			End Get
		End Property

		''' <summary>
		''' Returns the sub MIME type of this <code>DataFlavor</code>. </summary>
		''' <returns> the Sub MIME type of this <code>DataFlavor</code> </returns>
		Public Overridable Property subType As String
			Get
				Return If(mimeType IsNot Nothing, mimeType.subType, Nothing)
			End Get
		End Property

		''' <summary>
		''' Returns the human presentable name for this <code>DataFlavor</code>
		''' if <code>paramName</code> equals "humanPresentableName".  Otherwise
		''' returns the MIME type value associated with <code>paramName</code>.
		''' </summary>
		''' <param name="paramName"> the parameter name requested </param>
		''' <returns> the value of the name parameter, or <code>null</code>
		'''  if there is no associated value </returns>
		Public Overridable Function getParameter(ByVal paramName As String) As String
			If paramName.Equals("humanPresentableName") Then
				Return humanPresentableName
			Else
				Return If(mimeType IsNot Nothing, mimeType.getParameter(paramName), Nothing)
			End If
		End Function


		''' <summary>
		''' {@inheritDoc}
		''' <p>
		''' The equals comparison for the {@code DataFlavor} class is implemented
		''' as follows: Two <code>DataFlavor</code>s are considered equal if and
		''' only if their MIME primary type and subtype and representation class are
		''' equal. Additionally, if the primary type is "text", the subtype denotes
		''' a text flavor which supports the charset parameter, and the
		''' representation class is not <code>java.io.Reader</code>,
		''' <code>java.lang.String</code>, <code>java.nio.CharBuffer</code>, or
		''' <code>[C</code>, the <code>charset</code> parameter must also be equal.
		''' If a charset is not explicitly specified for one or both
		''' <code>DataFlavor</code>s, the platform default encoding is assumed. See
		''' <code>selectBestTextFlavor</code> for a list of text flavors which
		''' support the charset parameter.
		''' </summary>
		''' <param name="o"> the <code>Object</code> to compare with <code>this</code> </param>
		''' <returns> <code>true</code> if <code>that</code> is equivalent to this
		'''         <code>DataFlavor</code>; <code>false</code> otherwise </returns>
		''' <seealso cref= #selectBestTextFlavor </seealso>
		Public Overrides Function Equals(ByVal o As Object) As Boolean
			Return ((TypeOf o Is DataFlavor) AndAlso Equals(CType(o, DataFlavor)))
		End Function

		''' <summary>
		''' This method has the same behavior as <seealso cref="#equals(Object)"/>.
		''' The only difference being that it takes a {@code DataFlavor} instance
		''' as a parameter.
		''' </summary>
		''' <param name="that"> the <code>DataFlavor</code> to compare with
		'''        <code>this</code> </param>
		''' <returns> <code>true</code> if <code>that</code> is equivalent to this
		'''         <code>DataFlavor</code>; <code>false</code> otherwise </returns>
		''' <seealso cref= #selectBestTextFlavor </seealso>
		Public Overrides Function Equals(ByVal that As DataFlavor) As Boolean
			If that Is Nothing Then Return False
			If Me Is that Then Return True

			If Not java.util.Objects.Equals(Me.representationClass, that.representationClass) Then Return False

			If mimeType Is Nothing Then
				If that.mimeType IsNot Nothing Then Return False
			Else
				If Not mimeType.match(that.mimeType) Then Return False

				If "text".Equals(primaryType) Then
					If sun.awt.datatransfer.DataTransferer.doesSubtypeSupportCharset(Me) AndAlso representationClass IsNot Nothing AndAlso (Not standardTextRepresentationClass) Then
						Dim thisCharset As String = sun.awt.datatransfer.DataTransferer.canonicalName(Me.getParameter("charset"))
						Dim thatCharset As String = sun.awt.datatransfer.DataTransferer.canonicalName(that.getParameter("charset"))
						If Not java.util.Objects.Equals(thisCharset, thatCharset) Then Return False
					End If

					If "html".Equals(subType) Then
						Dim thisDocument As String = Me.getParameter("document")
						Dim thatDocument As String = that.getParameter("document")
						If Not java.util.Objects.Equals(thisDocument, thatDocument) Then Return False
					End If
				End If
			End If

			Return True
		End Function

		''' <summary>
		''' Compares only the <code>mimeType</code> against the passed in
		''' <code>String</code> and <code>representationClass</code> is
		''' not considered in the comparison.
		''' 
		''' If <code>representationClass</code> needs to be compared, then
		''' <code>equals(new DataFlavor(s))</code> may be used. </summary>
		''' @deprecated As inconsistent with <code>hashCode()</code> contract,
		'''             use <code>isMimeTypeEqual(String)</code> instead. 
		''' <param name="s"> the {@code mimeType} to compare. </param>
		''' <returns> true if the String (MimeType) is equal; false otherwise or if
		'''         {@code s} is {@code null} </returns>
		<Obsolete("As inconsistent with <code>hashCode()</code> contract,")> _
		Public Overrides Function Equals(ByVal s As String) As Boolean
			If s Is Nothing OrElse mimeType Is Nothing Then Return False
			Return isMimeTypeEqual(s)
		End Function

		''' <summary>
		''' Returns hash code for this <code>DataFlavor</code>.
		''' For two equal <code>DataFlavor</code>s, hash codes are equal.
		''' For the <code>String</code>
		''' that matches <code>DataFlavor.equals(String)</code>, it is not
		''' guaranteed that <code>DataFlavor</code>'s hash code is equal
		''' to the hash code of the <code>String</code>.
		''' </summary>
		''' <returns> a hash code for this <code>DataFlavor</code> </returns>
		Public Overrides Function GetHashCode() As Integer
			Dim total As Integer = 0

			If representationClass IsNot Nothing Then total += representationClass.GetHashCode()

			If mimeType IsNot Nothing Then
				Dim primaryType_Renamed As String = mimeType.primaryType
				If primaryType_Renamed IsNot Nothing Then total += primaryType_Renamed.GetHashCode()

				' Do not add subType.hashCode() to the total. equals uses
				' MimeType.match which reports a match if one or both of the
				' subTypes is '*', regardless of the other subType.

				If "text".Equals(primaryType_Renamed) Then
					If sun.awt.datatransfer.DataTransferer.doesSubtypeSupportCharset(Me) AndAlso representationClass IsNot Nothing AndAlso (Not standardTextRepresentationClass) Then
						Dim charset As String = sun.awt.datatransfer.DataTransferer.canonicalName(getParameter("charset"))
						If charset IsNot Nothing Then total += charset.GetHashCode()
					End If

					If "html".Equals(subType) Then
						Dim document As String = Me.getParameter("document")
						If document IsNot Nothing Then total += document.GetHashCode()
					End If
				End If
			End If

			Return total
		End Function

		''' <summary>
		''' Identical to <seealso cref="#equals(DataFlavor)"/>.
		''' </summary>
		''' <param name="that"> the <code>DataFlavor</code> to compare with
		'''        <code>this</code> </param>
		''' <returns> <code>true</code> if <code>that</code> is equivalent to this
		'''         <code>DataFlavor</code>; <code>false</code> otherwise </returns>
		''' <seealso cref= #selectBestTextFlavor
		''' @since 1.3 </seealso>
		Public Overridable Function match(ByVal that As DataFlavor) As Boolean
			Return Equals(that)
		End Function

		''' <summary>
		''' Returns whether the string representation of the MIME type passed in
		''' is equivalent to the MIME type of this <code>DataFlavor</code>.
		''' Parameters are not included in the comparison.
		''' </summary>
		''' <param name="mimeType"> the string representation of the MIME type </param>
		''' <returns> true if the string representation of the MIME type passed in is
		'''         equivalent to the MIME type of this <code>DataFlavor</code>;
		'''         false otherwise </returns>
		''' <exception cref="NullPointerException"> if mimeType is <code>null</code> </exception>
		Public Overridable Function isMimeTypeEqual(ByVal mimeType_Renamed As String) As Boolean
			' JCK Test DataFlavor0117: if 'mimeType' is null, throw NPE
			If mimeType_Renamed Is Nothing Then Throw New NullPointerException("mimeType")
			If Me.mimeType Is Nothing Then Return False
			Try
				Return Me.mimeType.match(New MimeType(mimeType_Renamed))
			Catch mtpe As MimeTypeParseException
				Return False
			End Try
		End Function

		''' <summary>
		''' Compares the <code>mimeType</code> of two <code>DataFlavor</code>
		''' objects. No parameters are considered.
		''' </summary>
		''' <param name="dataFlavor"> the <code>DataFlavor</code> to be compared </param>
		''' <returns> true if the <code>MimeType</code>s are equal,
		'''  otherwise false </returns>

		Public Function isMimeTypeEqual(ByVal dataFlavor_Renamed As DataFlavor) As Boolean
			Return isMimeTypeEqual(dataFlavor_Renamed.mimeType)
		End Function

		''' <summary>
		''' Compares the <code>mimeType</code> of two <code>DataFlavor</code>
		''' objects.  No parameters are considered.
		''' </summary>
		''' <returns> true if the <code>MimeType</code>s are equal,
		'''  otherwise false </returns>

		Private Function isMimeTypeEqual(ByVal mtype As MimeType) As Boolean
			If Me.mimeType Is Nothing Then Return (mtype Is Nothing)
			Return mimeType.match(mtype)
		End Function

		''' <summary>
		''' Checks if the representation class is one of the standard text
		''' representation classes.
		''' </summary>
		''' <returns> true if the representation class is one of the standard text
		'''              representation classes, otherwise false </returns>
		Private Property standardTextRepresentationClass As Boolean
			Get
				Return representationClassReader OrElse GetType(String).Equals(representationClass) OrElse representationClassCharBuffer OrElse GetType(Char()).Equals(representationClass)
			End Get
		End Property

	   ''' <summary>
	   ''' Does the <code>DataFlavor</code> represent a serialized object?
	   ''' </summary>

		Public Overridable Property mimeTypeSerializedObject As Boolean
			Get
				Return isMimeTypeEqual(javaSerializedObjectMimeType)
			End Get
		End Property

		Public Property defaultRepresentationClass As  [Class]
			Get
				Return ioInputStreamClass
			End Get
		End Property

		Public Property defaultRepresentationClassAsString As String
			Get
				Return defaultRepresentationClass.name
			End Get
		End Property

	   ''' <summary>
	   ''' Does the <code>DataFlavor</code> represent a
	   ''' <code>java.io.InputStream</code>?
	   ''' </summary>

		Public Overridable Property representationClassInputStream As Boolean
			Get
				Return representationClass.IsSubclassOf(ioInputStreamClass)
			End Get
		End Property

		''' <summary>
		''' Returns whether the representation class for this
		''' <code>DataFlavor</code> is <code>java.io.Reader</code> or a subclass
		''' thereof.
		''' 
		''' @since 1.4
		''' </summary>
		Public Overridable Property representationClassReader As Boolean
			Get
				Return representationClass.IsSubclassOf(GetType(java.io.Reader))
			End Get
		End Property

		''' <summary>
		''' Returns whether the representation class for this
		''' <code>DataFlavor</code> is <code>java.nio.CharBuffer</code> or a
		''' subclass thereof.
		''' 
		''' @since 1.4
		''' </summary>
		Public Overridable Property representationClassCharBuffer As Boolean
			Get
				Return representationClass.IsSubclassOf(GetType(java.nio.CharBuffer))
			End Get
		End Property

		''' <summary>
		''' Returns whether the representation class for this
		''' <code>DataFlavor</code> is <code>java.nio.ByteBuffer</code> or a
		''' subclass thereof.
		''' 
		''' @since 1.4
		''' </summary>
		Public Overridable Property representationClassByteBuffer As Boolean
			Get
				Return representationClass.IsSubclassOf(GetType(java.nio.ByteBuffer))
			End Get
		End Property

	   ''' <summary>
	   ''' Returns true if the representation class can be serialized. </summary>
	   ''' <returns> true if the representation class can be serialized </returns>

		Public Overridable Property representationClassSerializable As Boolean
			Get
				Return representationClass.IsSubclassOf(GetType(java.io.Serializable))
			End Get
		End Property

	   ''' <summary>
	   ''' Returns true if the representation class is <code>Remote</code>. </summary>
	   ''' <returns> true if the representation class is <code>Remote</code> </returns>

		Public Overridable Property representationClassRemote As Boolean
			Get
				Return sun.awt.datatransfer.DataTransferer.isRemote(representationClass)
			End Get
		End Property

	   ''' <summary>
	   ''' Returns true if the <code>DataFlavor</code> specified represents
	   ''' a serialized object. </summary>
	   ''' <returns> true if the <code>DataFlavor</code> specified represents
	   '''   a Serialized Object </returns>

		Public Overridable Property flavorSerializedObjectType As Boolean
			Get
				Return representationClassSerializable AndAlso isMimeTypeEqual(javaSerializedObjectMimeType)
			End Get
		End Property

		''' <summary>
		''' Returns true if the <code>DataFlavor</code> specified represents
		''' a remote object. </summary>
		''' <returns> true if the <code>DataFlavor</code> specified represents
		'''  a Remote Object </returns>

		Public Overridable Property flavorRemoteObjectType As Boolean
			Get
				Return representationClassRemote AndAlso representationClassSerializable AndAlso isMimeTypeEqual(javaRemoteObjectMimeType)
			End Get
		End Property


	   ''' <summary>
	   ''' Returns true if the <code>DataFlavor</code> specified represents
	   ''' a list of file objects. </summary>
	   ''' <returns> true if the <code>DataFlavor</code> specified represents
	   '''   a List of File objects </returns>

	   Public Overridable Property flavorJavaFileListType As Boolean
		   Get
				If mimeType Is Nothing OrElse representationClass Is Nothing Then Return False
				Return representationClass.IsSubclassOf(GetType(IList)) AndAlso mimeType.match(javaFileListFlavor.mimeType)
    
		   End Get
	   End Property

		''' <summary>
		''' Returns whether this <code>DataFlavor</code> is a valid text flavor for
		''' this implementation of the Java platform. Only flavors equivalent to
		''' <code>DataFlavor.stringFlavor</code> and <code>DataFlavor</code>s with
		''' a primary MIME type of "text" can be valid text flavors.
		''' <p>
		''' If this flavor supports the charset parameter, it must be equivalent to
		''' <code>DataFlavor.stringFlavor</code>, or its representation must be
		''' <code>java.io.Reader</code>, <code>java.lang.String</code>,
		''' <code>java.nio.CharBuffer</code>, <code>[C</code>,
		''' <code>java.io.InputStream</code>, <code>java.nio.ByteBuffer</code>, or
		''' <code>[B</code>. If the representation is
		''' <code>java.io.InputStream</code>, <code>java.nio.ByteBuffer</code>, or
		''' <code>[B</code>, then this flavor's <code>charset</code> parameter must
		''' be supported by this implementation of the Java platform. If a charset
		''' is not specified, then the platform default charset, which is always
		''' supported, is assumed.
		''' <p>
		''' If this flavor does not support the charset parameter, its
		''' representation must be <code>java.io.InputStream</code>,
		''' <code>java.nio.ByteBuffer</code>, or <code>[B</code>.
		''' <p>
		''' See <code>selectBestTextFlavor</code> for a list of text flavors which
		''' support the charset parameter.
		''' </summary>
		''' <returns> <code>true</code> if this <code>DataFlavor</code> is a valid
		'''         text flavor as described above; <code>false</code> otherwise </returns>
		''' <seealso cref= #selectBestTextFlavor
		''' @since 1.4 </seealso>
		Public Overridable Property flavorTextType As Boolean
			Get
				Return (sun.awt.datatransfer.DataTransferer.isFlavorCharsetTextType(Me) OrElse sun.awt.datatransfer.DataTransferer.isFlavorNoncharsetTextType(Me))
			End Get
		End Property

	   ''' <summary>
	   ''' Serializes this <code>DataFlavor</code>.
	   ''' </summary>

	   <MethodImpl(MethodImplOptions.Synchronized)> _
	   Public Overridable Sub writeExternal(ByVal os As java.io.ObjectOutput)
		   If mimeType IsNot Nothing Then
			   mimeType.parameterter("humanPresentableName", humanPresentableName)
			   os.writeObject(mimeType)
			   mimeType.removeParameter("humanPresentableName")
		   Else
			   os.writeObject(Nothing)
		   End If

		   os.writeObject(representationClass)
	   End Sub

	   ''' <summary>
	   ''' Restores this <code>DataFlavor</code> from a Serialized state.
	   ''' </summary>

	   <MethodImpl(MethodImplOptions.Synchronized)> _
	   Public Overridable Sub readExternal(ByVal [is] As java.io.ObjectInput)
		   Dim rcn As String = Nothing
			mimeType = CType([is].readObject(), MimeType)

			If mimeType IsNot Nothing Then
				humanPresentableName = mimeType.getParameter("humanPresentableName")
				mimeType.removeParameter("humanPresentableName")
				rcn = mimeType.getParameter("class")
				If rcn Is Nothing Then Throw New java.io.IOException("no class parameter specified in: " & mimeType)
			End If

			Try
				representationClass = CType([is].readObject(), [Class])
			Catch ode As java.io.OptionalDataException
				If (Not ode.eof) OrElse ode.length <> 0 Then Throw ode
				' Ensure backward compatibility.
				' Old versions didn't write the representation class to the stream.
				If rcn IsNot Nothing Then representationClass = DataFlavor.tryToLoadClass(rcn, Me.GetType().classLoader)
			End Try
	   End Sub

	   ''' <summary>
	   ''' Returns a clone of this <code>DataFlavor</code>. </summary>
	   ''' <returns> a clone of this <code>DataFlavor</code> </returns>

		Public Overridable Function clone() As Object
			Dim newObj As Object = MyBase.clone()
			If mimeType IsNot Nothing Then CType(newObj, DataFlavor).mimeType = CType(mimeType.clone(), MimeType)
			Return newObj
		End Function ' clone()

	   ''' <summary>
	   ''' Called on <code>DataFlavor</code> for every MIME Type parameter
	   ''' to allow <code>DataFlavor</code> subclasses to handle special
	   ''' parameters like the text/plain <code>charset</code>
	   ''' parameters, whose values are case insensitive.  (MIME type parameter
	   ''' values are supposed to be case sensitive.
	   ''' <p>
	   ''' This method is called for each parameter name/value pair and should
	   ''' return the normalized representation of the <code>parameterValue</code>.
	   ''' 
	   ''' This method is never invoked by this implementation from 1.1 onwards.
	   ''' 
	   ''' @deprecated
	   ''' </summary>
		<Obsolete> _
		Protected Friend Overridable Function normalizeMimeTypeParameter(ByVal parameterName As String, ByVal parameterValue As String) As String
			Return parameterValue
		End Function

	   ''' <summary>
	   ''' Called for each MIME type string to give <code>DataFlavor</code> subtypes
	   ''' the opportunity to change how the normalization of MIME types is
	   ''' accomplished.  One possible use would be to add default
	   ''' parameter/value pairs in cases where none are present in the MIME
	   ''' type string passed in.
	   ''' 
	   ''' This method is never invoked by this implementation from 1.1 onwards.
	   ''' 
	   ''' @deprecated
	   ''' </summary>
		<Obsolete> _
		Protected Friend Overridable Function normalizeMimeType(ByVal mimeType_Renamed As String) As String
			Return mimeType_Renamed
		End Function

	'    
	'     * fields
	'     

		' placeholder for caching any platform-specific data for flavor 

		<NonSerialized> _
		Friend atom As Integer

		' Mime Type of DataFlavor 

		Friend mimeType As MimeType

		Private humanPresentableName As String

		''' <summary>
		''' Java class of objects this DataFlavor represents * </summary>

		Private representationClass As  [Class]

	End Class ' class DataFlavor

End Namespace