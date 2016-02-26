'
' * Copyright (c) 2006, 2008, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.tools


	''' <summary>
	''' File abstraction for tools.  In this context, <em>file</em> means
	''' an abstraction of regular files and other sources of data.  For
	''' example, a file object can be used to represent regular files,
	''' memory cache, or data in databases.
	''' 
	''' <p>All methods in this interface might throw a SecurityException if
	''' a security exception occurs.
	''' 
	''' <p>Unless explicitly allowed, all methods in this interface might
	''' throw a NullPointerException if given a {@code null} argument.
	''' 
	''' @author Peter von der Ah&eacute;
	''' @author Jonathan Gibbons
	''' @since 1.6
	''' </summary>
	Public Interface FileObject

		''' <summary>
		''' Returns a URI identifying this file object. </summary>
		''' <returns> a URI </returns>
		Function toUri() As java.net.URI

		''' <summary>
		''' Gets a user-friendly name for this file object.  The exact
		''' value returned is not specified but implementations should take
		''' care to preserve names as given by the user.  For example, if
		''' the user writes the filename {@code "BobsApp\Test.java"} on
		''' the command line, this method should return {@code
		''' "BobsApp\Test.java"} whereas the <seealso cref="#toUri toUri"/>
		''' method might return {@code
		''' file:///C:/Documents%20and%20Settings/UncleBob/BobsApp/Test.java}.
		''' </summary>
		''' <returns> a user-friendly name </returns>
		ReadOnly Property name As String

		''' <summary>
		''' Gets an InputStream for this file object.
		''' </summary>
		''' <returns> an InputStream </returns>
		''' <exception cref="IllegalStateException"> if this file object was
		''' opened for writing and does not support reading </exception>
		''' <exception cref="UnsupportedOperationException"> if this kind of file
		''' object does not support byte access </exception>
		''' <exception cref="IOException"> if an I/O error occurred </exception>
		Function openInputStream() As java.io.InputStream

		''' <summary>
		''' Gets an OutputStream for this file object.
		''' </summary>
		''' <returns> an OutputStream </returns>
		''' <exception cref="IllegalStateException"> if this file object was
		''' opened for reading and does not support writing </exception>
		''' <exception cref="UnsupportedOperationException"> if this kind of
		''' file object does not support byte access </exception>
		''' <exception cref="IOException"> if an I/O error occurred </exception>
		Function openOutputStream() As java.io.OutputStream

		''' <summary>
		''' Gets a reader for this object.  The returned reader will
		''' replace bytes that cannot be decoded with the default
		''' translation character.  In addition, the reader may report a
		''' diagnostic unless {@code ignoreEncodingErrors} is true.
		''' </summary>
		''' <param name="ignoreEncodingErrors"> ignore encoding errors if true </param>
		''' <returns> a Reader </returns>
		''' <exception cref="IllegalStateException"> if this file object was
		''' opened for writing and does not support reading </exception>
		''' <exception cref="UnsupportedOperationException"> if this kind of
		''' file object does not support character access </exception>
		''' <exception cref="IOException"> if an I/O error occurred </exception>
		Function openReader(ByVal ignoreEncodingErrors As Boolean) As java.io.Reader

		''' <summary>
		''' Gets the character content of this file object, if available.
		''' Any byte that cannot be decoded will be replaced by the default
		''' translation character.  In addition, a diagnostic may be
		''' reported unless {@code ignoreEncodingErrors} is true.
		''' </summary>
		''' <param name="ignoreEncodingErrors"> ignore encoding errors if true </param>
		''' <returns> a CharSequence if available; {@code null} otherwise </returns>
		''' <exception cref="IllegalStateException"> if this file object was
		''' opened for writing and does not support reading </exception>
		''' <exception cref="UnsupportedOperationException"> if this kind of
		''' file object does not support character access </exception>
		''' <exception cref="IOException"> if an I/O error occurred </exception>
		Function getCharContent(ByVal ignoreEncodingErrors As Boolean) As CharSequence

		''' <summary>
		''' Gets a Writer for this file object.
		''' </summary>
		''' <returns> a Writer </returns>
		''' <exception cref="IllegalStateException"> if this file object was
		''' opened for reading and does not support writing </exception>
		''' <exception cref="UnsupportedOperationException"> if this kind of
		''' file object does not support character access </exception>
		''' <exception cref="IOException"> if an I/O error occurred </exception>
		Function openWriter() As java.io.Writer

		''' <summary>
		''' Gets the time this file object was last modified.  The time is
		''' measured in milliseconds since the epoch (00:00:00 GMT, January
		''' 1, 1970).
		''' </summary>
		''' <returns> the time this file object was last modified; or 0 if
		''' the file object does not exist, if an I/O error occurred, or if
		''' the operation is not supported </returns>
		ReadOnly Property lastModified As Long

		''' <summary>
		''' Deletes this file object.  In case of errors, returns false. </summary>
		''' <returns> true if and only if this file object is successfully
		''' deleted; false otherwise </returns>
		Function delete() As Boolean

	End Interface

End Namespace