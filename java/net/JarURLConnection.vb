'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.net


	''' <summary>
	''' A URL Connection to a Java ARchive (JAR) file or an entry in a JAR
	''' file.
	''' 
	''' <p>The syntax of a JAR URL is:
	''' 
	''' <pre>
	''' jar:&lt;url&gt;!/{entry}
	''' </pre>
	''' 
	''' <p>for example:
	''' 
	''' <p>{@code jar:http://www.foo.com/bar/baz.jar!/COM/foo/Quux.class}
	''' 
	''' <p>Jar URLs should be used to refer to a JAR file or entries in
	''' a JAR file. The example above is a JAR URL which refers to a JAR
	''' entry. If the entry name is omitted, the URL refers to the whole
	''' JAR file:
	''' 
	''' {@code jar:http://www.foo.com/bar/baz.jar!/}
	''' 
	''' <p>Users should cast the generic URLConnection to a
	''' JarURLConnection when they know that the URL they created is a JAR
	''' URL, and they need JAR-specific functionality. For example:
	''' 
	''' <pre>
	''' URL url = new URL("jar:file:/home/duke/duke.jar!/");
	''' JarURLConnection jarConnection = (JarURLConnection)url.openConnection();
	''' Manifest manifest = jarConnection.getManifest();
	''' </pre>
	''' 
	''' <p>JarURLConnection instances can only be used to read from JAR files.
	''' It is not possible to get a <seealso cref="java.io.OutputStream"/> to modify or write
	''' to the underlying JAR file using this class.
	''' <p>Examples:
	''' 
	''' <dl>
	''' 
	''' <dt>A Jar entry
	''' <dd>{@code jar:http://www.foo.com/bar/baz.jar!/COM/foo/Quux.class}
	''' 
	''' <dt>A Jar file
	''' <dd>{@code jar:http://www.foo.com/bar/baz.jar!/}
	''' 
	''' <dt>A Jar directory
	''' <dd>{@code jar:http://www.foo.com/bar/baz.jar!/COM/foo/}
	''' 
	''' </dl>
	''' 
	''' <p>{@code !/} is referred to as the <em>separator</em>.
	''' 
	''' <p>When constructing a JAR url via {@code new URL(context, spec)},
	''' the following rules apply:
	''' 
	''' <ul>
	''' 
	''' <li>if there is no context URL and the specification passed to the
	''' URL constructor doesn't contain a separator, the URL is considered
	''' to refer to a JarFile.
	''' 
	''' <li>if there is a context URL, the context URL is assumed to refer
	''' to a JAR file or a Jar directory.
	''' 
	''' <li>if the specification begins with a '/', the Jar directory is
	''' ignored, and the spec is considered to be at the root of the Jar
	''' file.
	''' 
	''' <p>Examples:
	''' 
	''' <dl>
	''' 
	''' <dt>context: <b>jar:http://www.foo.com/bar/jar.jar!/</b>,
	''' spec:<b>baz/entry.txt</b>
	''' 
	''' <dd>url:<b>jar:http://www.foo.com/bar/jar.jar!/baz/entry.txt</b>
	''' 
	''' <dt>context: <b>jar:http://www.foo.com/bar/jar.jar!/baz</b>,
	''' spec:<b>entry.txt</b>
	''' 
	''' <dd>url:<b>jar:http://www.foo.com/bar/jar.jar!/baz/entry.txt</b>
	''' 
	''' <dt>context: <b>jar:http://www.foo.com/bar/jar.jar!/baz</b>,
	''' spec:<b>/entry.txt</b>
	''' 
	''' <dd>url:<b>jar:http://www.foo.com/bar/jar.jar!/entry.txt</b>
	''' 
	''' </dl>
	''' 
	''' </ul>
	''' </summary>
	''' <seealso cref= java.net.URL </seealso>
	''' <seealso cref= java.net.URLConnection
	''' </seealso>
	''' <seealso cref= java.util.jar.JarFile </seealso>
	''' <seealso cref= java.util.jar.JarInputStream </seealso>
	''' <seealso cref= java.util.jar.Manifest </seealso>
	''' <seealso cref= java.util.zip.ZipEntry
	''' 
	''' @author Benjamin Renaud
	''' @since 1.2 </seealso>
	Public MustInherit Class JarURLConnection
		Inherits URLConnection

		Private jarFileURL As URL
		Private entryName As String

		''' <summary>
		''' The connection to the JAR file URL, if the connection has been
		''' initiated. This should be set by connect.
		''' </summary>
		Protected Friend jarFileURLConnection As URLConnection

		''' <summary>
		''' Creates the new JarURLConnection to the specified URL. </summary>
		''' <param name="url"> the URL </param>
		''' <exception cref="MalformedURLException"> if no legal protocol
		''' could be found in a specification string or the
		''' string could not be parsed. </exception>

		Protected Friend Sub New(  url As URL)
			MyBase.New(url)
			parseSpecs(url)
		End Sub

	'     get the specs for a given url out of the cache, and compute and
	'     * cache them if they're not there.
	'     
		Private Sub parseSpecs(  url As URL)
			Dim spec As String = url.file

			Dim separator As Integer = spec.IndexOf("!/")
	'        
	'         * REMIND: we don't handle nested JAR URLs
	'         
			If separator = -1 Then Throw New MalformedURLException("no !/ found in url spec:" & spec)

			jarFileURL = New URL(spec.Substring(0, separator))
			separator += 1
			entryName = Nothing

			' if ! is the last letter of the innerURL, entryName is null 
			separator += 1
			If separator <> spec.length() Then
				entryName = spec.Substring(separator, spec.length() - separator)
				entryName = sun.net.www.ParseUtil.decode(entryName)
			End If
		End Sub

		''' <summary>
		''' Returns the URL for the Jar file for this connection.
		''' </summary>
		''' <returns> the URL for the Jar file for this connection. </returns>
		Public Overridable Property jarFileURL As URL
			Get
				Return jarFileURL
			End Get
		End Property

		''' <summary>
		''' Return the entry name for this connection. This method
		''' returns null if the JAR file URL corresponding to this
		''' connection points to a JAR file and not a JAR file entry.
		''' </summary>
		''' <returns> the entry name for this connection, if any. </returns>
		Public Overridable Property entryName As String
			Get
				Return entryName
			End Get
		End Property

		''' <summary>
		''' Return the JAR file for this connection.
		''' </summary>
		''' <returns> the JAR file for this connection. If the connection is
		''' a connection to an entry of a JAR file, the JAR file object is
		''' returned
		''' </returns>
		''' <exception cref="IOException"> if an IOException occurs while trying to
		''' connect to the JAR file for this connection.
		''' </exception>
		''' <seealso cref= #connect </seealso>
		Public MustOverride ReadOnly Property jarFile As java.util.jar.JarFile

		''' <summary>
		''' Returns the Manifest for this connection, or null if none.
		''' </summary>
		''' <returns> the manifest object corresponding to the JAR file object
		''' for this connection.
		''' </returns>
		''' <exception cref="IOException"> if getting the JAR file for this
		''' connection causes an IOException to be thrown.
		''' </exception>
		''' <seealso cref= #getJarFile </seealso>
		Public Overridable Property manifest As java.util.jar.Manifest
			Get
				Return jarFile.manifest
			End Get
		End Property

		''' <summary>
		''' Return the JAR entry object for this connection, if any. This
		''' method returns null if the JAR file URL corresponding to this
		''' connection points to a JAR file and not a JAR file entry.
		''' </summary>
		''' <returns> the JAR entry object for this connection, or null if
		''' the JAR URL for this connection points to a JAR file.
		''' </returns>
		''' <exception cref="IOException"> if getting the JAR file for this
		''' connection causes an IOException to be thrown.
		''' </exception>
		''' <seealso cref= #getJarFile </seealso>
		''' <seealso cref= #getJarEntry </seealso>
		Public Overridable Property jarEntry As java.util.jar.JarEntry
			Get
				Return jarFile.getJarEntry(entryName)
			End Get
		End Property

		''' <summary>
		''' Return the Attributes object for this connection if the URL
		''' for it points to a JAR file entry, null otherwise.
		''' </summary>
		''' <returns> the Attributes object for this connection if the URL
		''' for it points to a JAR file entry, null otherwise.
		''' </returns>
		''' <exception cref="IOException"> if getting the JAR entry causes an
		''' IOException to be thrown.
		''' </exception>
		''' <seealso cref= #getJarEntry </seealso>
		Public Overridable Property attributes As java.util.jar.Attributes
			Get
				Dim e As java.util.jar.JarEntry = jarEntry
				Return If(e IsNot Nothing, e.attributes, Nothing)
			End Get
		End Property

		''' <summary>
		''' Returns the main Attributes for the JAR file for this
		''' connection.
		''' </summary>
		''' <returns> the main Attributes for the JAR file for this
		''' connection.
		''' </returns>
		''' <exception cref="IOException"> if getting the manifest causes an
		''' IOException to be thrown.
		''' </exception>
		''' <seealso cref= #getJarFile </seealso>
		''' <seealso cref= #getManifest </seealso>
		Public Overridable Property mainAttributes As java.util.jar.Attributes
			Get
				Dim man As java.util.jar.Manifest = manifest
				Return If(man IsNot Nothing, man.mainAttributes, Nothing)
			End Get
		End Property

		''' <summary>
		''' Return the Certificate object for this connection if the URL
		''' for it points to a JAR file entry, null otherwise. This method
		''' can only be called once
		''' the connection has been completely verified by reading
		''' from the input stream until the end of the stream has been
		''' reached. Otherwise, this method will return {@code null}
		''' </summary>
		''' <returns> the Certificate object for this connection if the URL
		''' for it points to a JAR file entry, null otherwise.
		''' </returns>
		''' <exception cref="IOException"> if getting the JAR entry causes an
		''' IOException to be thrown.
		''' </exception>
		''' <seealso cref= #getJarEntry </seealso>
		Public Overridable Property certificates As java.security.cert.Certificate()
			Get
				Dim e As java.util.jar.JarEntry = jarEntry
				Return If(e IsNot Nothing, e.certificates, Nothing)
			End Get
		End Property
	End Class

End Namespace