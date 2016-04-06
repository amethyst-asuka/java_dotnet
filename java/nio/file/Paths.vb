'
' * Copyright (c) 2007, 2011, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.nio.file


	''' <summary>
	''' This class consists exclusively of static methods that return a <seealso cref="Path"/>
	''' by converting a path string or <seealso cref="URI"/>.
	''' 
	''' @since 1.7
	''' </summary>

	Public NotInheritable Class Paths
		Private Sub New()
		End Sub

		''' <summary>
		''' Converts a path string, or a sequence of strings that when joined form
		''' a path string, to a {@code Path}. If {@code more} does not specify any
		''' elements then the value of the {@code first} parameter is the path string
		''' to convert. If {@code more} specifies one or more elements then each
		''' non-empty string, including {@code first}, is considered to be a sequence
		''' of name elements (see <seealso cref="Path"/>) and is joined to form a path string.
		''' The details as to how the Strings are joined is provider specific but
		''' typically they will be joined using the {@link FileSystem#getSeparator
		''' name-separator} as the separator. For example, if the name separator is
		''' "{@code /}" and {@code getPath("/foo","bar","gus")} is invoked, then the
		''' path string {@code "/foo/bar/gus"} is converted to a {@code Path}.
		''' A {@code Path} representing an empty path is returned if {@code first}
		''' is the empty string and {@code more} does not contain any non-empty
		''' strings.
		''' 
		''' <p> The {@code Path} is obtained by invoking the {@link FileSystem#getPath
		''' getPath} method of the <seealso cref="FileSystems#getDefault default"/> {@link
		''' FileSystem}.
		''' 
		''' <p> Note that while this method is very convenient, using it will imply
		''' an assumed reference to the default {@code FileSystem} and limit the
		''' utility of the calling code. Hence it should not be used in library code
		''' intended for flexible reuse. A more flexible alternative is to use an
		''' existing {@code Path} instance as an anchor, such as:
		''' <pre>
		'''     Path dir = ...
		'''     Path path = dir.resolve("file");
		''' </pre>
		''' </summary>
		''' <param name="first">
		'''          the path string or initial part of the path string </param>
		''' <param name="more">
		'''          additional strings to be joined to form the path string
		''' </param>
		''' <returns>  the resulting {@code Path}
		''' </returns>
		''' <exception cref="InvalidPathException">
		'''          if the path string cannot be converted to a {@code Path}
		''' </exception>
		''' <seealso cref= FileSystem#getPath </seealso>
		Public Shared Function [get](  first As String, ParamArray   more As String()) As Path
			Return FileSystems.default.getPath(first, more)
		End Function

		''' <summary>
		''' Converts the given URI to a <seealso cref="Path"/> object.
		''' 
		''' <p> This method iterates over the {@link FileSystemProvider#installedProviders()
		''' installed} providers to locate the provider that is identified by the
		''' URI <seealso cref="URI#getScheme scheme"/> of the given URI. URI schemes are
		''' compared without regard to case. If the provider is found then its {@link
		''' FileSystemProvider#getPath getPath} method is invoked to convert the
		''' URI.
		''' 
		''' <p> In the case of the default provider, identified by the URI scheme
		''' "file", the given URI has a non-empty path component, and undefined query
		''' and fragment components. Whether the authority component may be present
		''' is platform specific. The returned {@code Path} is associated with the
		''' <seealso cref="FileSystems#getDefault default"/> file system.
		''' 
		''' <p> The default provider provides a similar <em>round-trip</em> guarantee
		''' to the <seealso cref="java.io.File"/> class. For a given {@code Path} <i>p</i> it
		''' is guaranteed that
		''' <blockquote><tt>
		''' Paths.get(</tt><i>p</i><tt>.<seealso cref="Path#toUri() toUri"/>()).equals(</tt>
		''' <i>p</i><tt>.<seealso cref="Path#toAbsolutePath() toAbsolutePath"/>())</tt>
		''' </blockquote>
		''' so long as the original {@code Path}, the {@code URI}, and the new {@code
		''' Path} are all created in (possibly different invocations of) the same
		''' Java virtual machine. Whether other providers make any guarantees is
		''' provider specific and therefore unspecified.
		''' </summary>
		''' <param name="uri">
		'''          the URI to convert
		''' </param>
		''' <returns>  the resulting {@code Path}
		''' </returns>
		''' <exception cref="IllegalArgumentException">
		'''          if preconditions on the {@code uri} parameter do not hold. The
		'''          format of the URI is provider specific. </exception>
		''' <exception cref="FileSystemNotFoundException">
		'''          The file system, identified by the URI, does not exist and
		'''          cannot be created automatically, or the provider identified by
		'''          the URI's scheme component is not installed </exception>
		''' <exception cref="SecurityException">
		'''          if a security manager is installed and it denies an unspecified
		'''          permission to access the file system </exception>
		Public Shared Function [get](  uri As java.net.URI) As Path
			Dim scheme As String = uri.scheme
			If scheme Is Nothing Then Throw New IllegalArgumentException("Missing scheme")

			' check for default provider to avoid loading of installed providers
			If scheme.equalsIgnoreCase("file") Then Return FileSystems.default.provider().getPath(uri)

			' try to find provider
			For Each provider As java.nio.file.spi.FileSystemProvider In java.nio.file.spi.FileSystemProvider.installedProviders()
				If provider.scheme.equalsIgnoreCase(scheme) Then Return provider.getPath(uri)
			Next provider

			Throw New FileSystemNotFoundException("Provider """ & scheme & """ not installed")
		End Function
	End Class

End Namespace