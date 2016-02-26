'
' * Copyright (c) 2005, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' Provides simple implementations for most methods in JavaFileObject.
	''' This class is designed to be subclassed and used as a basis for
	''' JavaFileObject implementations.  Subclasses can override the
	''' implementation and specification of any method of this class as
	''' long as the general contract of JavaFileObject is obeyed.
	''' 
	''' @author Peter von der Ah&eacute;
	''' @since 1.6
	''' </summary>
	Public Class SimpleJavaFileObject
		Implements JavaFileObject

		''' <summary>
		''' A URI for this file object.
		''' </summary>
		Protected Friend ReadOnly uri As java.net.URI

		''' <summary>
		''' The kind of this file object.
		''' </summary>
		Protected Friend ReadOnly kind As javax.tools.JavaFileObject.Kind

		''' <summary>
		''' Construct a SimpleJavaFileObject of the given kind and with the
		''' given URI.
		''' </summary>
		''' <param name="uri">  the URI for this file object </param>
		''' <param name="kind"> the kind of this file object </param>
		Protected Friend Sub New(ByVal uri As java.net.URI, ByVal kind As javax.tools.JavaFileObject.Kind)
			' null checks
			uri.GetType()
			kind.GetType()
			If uri.path Is Nothing Then Throw New System.ArgumentException("URI must have a path: " & uri)
			Me.uri = uri
			Me.kind = kind
		End Sub

		Public Overridable Function toUri() As java.net.URI Implements FileObject.toUri
			Return uri
		End Function

		Public Overridable Property name As String Implements FileObject.getName
			Get
				Return toUri().path
			End Get
		End Property

		''' <summary>
		''' This implementation always throws {@linkplain
		''' UnsupportedOperationException}.  Subclasses can change this
		''' behavior as long as the contract of <seealso cref="FileObject"/> is
		''' obeyed.
		''' </summary>
		Public Overridable Function openInputStream() As InputStream
			Throw New System.NotSupportedException
		End Function

		''' <summary>
		''' This implementation always throws {@linkplain
		''' UnsupportedOperationException}.  Subclasses can change this
		''' behavior as long as the contract of <seealso cref="FileObject"/> is
		''' obeyed.
		''' </summary>
		Public Overridable Function openOutputStream() As OutputStream
			Throw New System.NotSupportedException
		End Function

		''' <summary>
		''' Wraps the result of <seealso cref="#getCharContent"/> in a Reader.
		''' Subclasses can change this behavior as long as the contract of
		''' <seealso cref="FileObject"/> is obeyed.
		''' </summary>
		''' <param name="ignoreEncodingErrors"> {@inheritDoc} </param>
		''' <returns> a Reader wrapping the result of getCharContent </returns>
		''' <exception cref="IllegalStateException"> {@inheritDoc} </exception>
		''' <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		''' <exception cref="IOException"> {@inheritDoc} </exception>
		Public Overridable Function openReader(ByVal ignoreEncodingErrors As Boolean) As Reader
			Dim ___charContent As CharSequence = getCharContent(ignoreEncodingErrors)
			If ___charContent Is Nothing Then Throw New System.NotSupportedException
			If TypeOf ___charContent Is java.nio.CharBuffer Then
				Dim buffer As java.nio.CharBuffer = CType(___charContent, java.nio.CharBuffer)
				If buffer.hasArray() Then Return New CharArrayReader(buffer.array())
			End If
			Return New StringReader(___charContent.ToString())
		End Function

		''' <summary>
		''' This implementation always throws {@linkplain
		''' UnsupportedOperationException}.  Subclasses can change this
		''' behavior as long as the contract of <seealso cref="FileObject"/> is
		''' obeyed.
		''' </summary>
		Public Overridable Function getCharContent(ByVal ignoreEncodingErrors As Boolean) As CharSequence Implements FileObject.getCharContent
			Throw New System.NotSupportedException
		End Function

		''' <summary>
		''' Wraps the result of openOutputStream in a Writer.  Subclasses
		''' can change this behavior as long as the contract of {@link
		''' FileObject} is obeyed.
		''' </summary>
		''' <returns> a Writer wrapping the result of openOutputStream </returns>
		''' <exception cref="IllegalStateException"> {@inheritDoc} </exception>
		''' <exception cref="UnsupportedOperationException"> {@inheritDoc} </exception>
		''' <exception cref="IOException"> {@inheritDoc} </exception>
		Public Overridable Function openWriter() As Writer
			Return New OutputStreamWriter(openOutputStream())
		End Function

		''' <summary>
		''' This implementation returns {@code 0L}.  Subclasses can change
		''' this behavior as long as the contract of <seealso cref="FileObject"/> is
		''' obeyed.
		''' </summary>
		''' <returns> {@code 0L} </returns>
		Public Overridable Property lastModified As Long Implements FileObject.getLastModified
			Get
				Return 0L
			End Get
		End Property

		''' <summary>
		''' This implementation does nothing.  Subclasses can change this
		''' behavior as long as the contract of <seealso cref="FileObject"/> is
		''' obeyed.
		''' </summary>
		''' <returns> {@code false} </returns>
		Public Overridable Function delete() As Boolean Implements FileObject.delete
			Return False
		End Function

		''' <returns> {@code this.kind} </returns>
		Public Overridable Property kind As javax.tools.JavaFileObject.Kind
			Get
				Return kind
			End Get
		End Property

		''' <summary>
		''' This implementation compares the path of its URI to the given
		''' simple name.  This method returns true if the given kind is
		''' equal to the kind of this object, and if the path is equal to
		''' {@code simpleName + kind.extension} or if it ends with {@code
		''' "/" + simpleName + kind.extension}.
		''' 
		''' <p>This method calls <seealso cref="#getKind"/> and <seealso cref="#toUri"/> and
		''' does not access the fields <seealso cref="#uri"/> and <seealso cref="#kind"/>
		''' directly.
		''' 
		''' <p>Subclasses can change this behavior as long as the contract
		''' of <seealso cref="JavaFileObject"/> is obeyed.
		''' </summary>
		Public Overridable Function isNameCompatible(ByVal simpleName As String, ByVal kind As javax.tools.JavaFileObject.Kind) As Boolean
			Dim baseName As String = simpleName + kind.extension
			Return kind.Equals(kind) AndAlso (baseName.Equals(toUri().path) OrElse toUri().path.EndsWith("/" & baseName))
		End Function

		''' <summary>
		''' This implementation returns {@code null}.  Subclasses can
		''' change this behavior as long as the contract of
		''' <seealso cref="JavaFileObject"/> is obeyed.
		''' </summary>
		Public Overridable Property nestingKind As javax.lang.model.element.NestingKind Implements JavaFileObject.getNestingKind
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' This implementation returns {@code null}.  Subclasses can
		''' change this behavior as long as the contract of
		''' <seealso cref="JavaFileObject"/> is obeyed.
		''' </summary>
		Public Overridable Property accessLevel As javax.lang.model.element.Modifier Implements JavaFileObject.getAccessLevel
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides Function ToString() As String
			Return Me.GetType().name & "[" & toUri() & "]"
		End Function
	End Class

End Namespace