'
' * Copyright (c) 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' Forwards calls to a given file object.  Subclasses of this class
	''' might override some of these methods and might also provide
	''' additional fields and methods.
	''' </summary>
	''' @param <F> the kind of file object forwarded to by this object
	''' @author Peter von der Ah&eacute;
	''' @since 1.6 </param>
	Public Class ForwardingFileObject(Of F As FileObject)
		Implements FileObject

		''' <summary>
		''' The file object which all methods are delegated to.
		''' </summary>
		Protected Friend ReadOnly fileObject As F

		''' <summary>
		''' Creates a new instance of ForwardingFileObject. </summary>
		''' <param name="fileObject"> delegate to this file object </param>
		Protected Friend Sub New(ByVal fileObject As F)
			fileObject.GetType() ' null check
			Me.fileObject = fileObject
		End Sub

		Public Overridable Function toUri() As java.net.URI Implements FileObject.toUri
			Return fileObject.toUri()
		End Function

		Public Overridable Property name As String Implements FileObject.getName
			Get
				Return fileObject.name
			End Get
		End Property

		''' <exception cref="IllegalStateException"> {@inheritDoc} </exception>
		''' <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		''' <exception cref="IOException"> {@inheritDoc} </exception>
		Public Overridable Function openInputStream() As java.io.InputStream Implements FileObject.openInputStream
			Return fileObject.openInputStream()
		End Function

		''' <exception cref="IllegalStateException"> {@inheritDoc} </exception>
		''' <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		''' <exception cref="IOException"> {@inheritDoc} </exception>
		Public Overridable Function openOutputStream() As java.io.OutputStream Implements FileObject.openOutputStream
			Return fileObject.openOutputStream()
		End Function

		''' <exception cref="IllegalStateException"> {@inheritDoc} </exception>
		''' <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		''' <exception cref="IOException"> {@inheritDoc} </exception>
		Public Overridable Function openReader(ByVal ignoreEncodingErrors As Boolean) As java.io.Reader Implements FileObject.openReader
			Return fileObject.openReader(ignoreEncodingErrors)
		End Function

		''' <exception cref="IllegalStateException"> {@inheritDoc} </exception>
		''' <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		''' <exception cref="IOException"> {@inheritDoc} </exception>
		Public Overridable Function getCharContent(ByVal ignoreEncodingErrors As Boolean) As CharSequence Implements FileObject.getCharContent
			Return fileObject.getCharContent(ignoreEncodingErrors)
		End Function

		''' <exception cref="IllegalStateException"> {@inheritDoc} </exception>
		''' <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		''' <exception cref="IOException"> {@inheritDoc} </exception>
		Public Overridable Function openWriter() As java.io.Writer Implements FileObject.openWriter
			Return fileObject.openWriter()
		End Function

		Public Overridable Property lastModified As Long Implements FileObject.getLastModified
			Get
				Return fileObject.lastModified
			End Get
		End Property

		Public Overridable Function delete() As Boolean Implements FileObject.delete
			Return fileObject.delete()
		End Function
	End Class

End Namespace