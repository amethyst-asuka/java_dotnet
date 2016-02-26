Imports System

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

Namespace javax.swing.filechooser


	''' <summary>
	''' An implementation of {@code FileFilter} that filters using a
	''' specified set of extensions. The extension for a file is the
	''' portion of the file name after the last ".". Files whose name does
	''' not contain a "." have no file name extension. File name extension
	''' comparisons are case insensitive.
	''' <p>
	''' The following example creates a
	''' {@code FileNameExtensionFilter} that will show {@code jpg} files:
	''' <pre>
	''' FileFilter filter = new FileNameExtensionFilter("JPEG file", "jpg", "jpeg");
	''' JFileChooser fileChooser = ...;
	''' fileChooser.addChoosableFileFilter(filter);
	''' </pre>
	''' </summary>
	''' <seealso cref= FileFilter </seealso>
	''' <seealso cref= javax.swing.JFileChooser#setFileFilter </seealso>
	''' <seealso cref= javax.swing.JFileChooser#addChoosableFileFilter </seealso>
	''' <seealso cref= javax.swing.JFileChooser#getFileFilter
	''' 
	''' @since 1.6 </seealso>
	Public NotInheritable Class FileNameExtensionFilter
		Inherits FileFilter

		' Description of this filter.
		Private ReadOnly description As String
		' Known extensions.
		Private ReadOnly extensions As String()
		' Cached ext
		Private ReadOnly lowerCaseExtensions As String()

		''' <summary>
		''' Creates a {@code FileNameExtensionFilter} with the specified
		''' description and file name extensions. The returned {@code
		''' FileNameExtensionFilter} will accept all directories and any
		''' file with a file name extension contained in {@code extensions}.
		''' </summary>
		''' <param name="description"> textual description for the filter, may be
		'''                    {@code null} </param>
		''' <param name="extensions"> the accepted file name extensions </param>
		''' <exception cref="IllegalArgumentException"> if extensions is {@code null}, empty,
		'''         contains {@code null}, or contains an empty string </exception>
		''' <seealso cref= #accept </seealso>
		Public Sub New(ByVal description As String, ParamArray ByVal extensions As String())
			If extensions Is Nothing OrElse extensions.Length = 0 Then Throw New System.ArgumentException("Extensions must be non-null and not empty")
			Me.description = description
			Me.extensions = New String(extensions.Length - 1){}
			Me.lowerCaseExtensions = New String(extensions.Length - 1){}
			For i As Integer = 0 To extensions.Length - 1
				If extensions(i) Is Nothing OrElse extensions(i).Length = 0 Then Throw New System.ArgumentException("Each extension must be non-null and not empty")
				Me.extensions(i) = extensions(i)
				lowerCaseExtensions(i) = extensions(i).ToLower(java.util.Locale.ENGLISH)
			Next i
		End Sub

		''' <summary>
		''' Tests the specified file, returning true if the file is
		''' accepted, false otherwise. True is returned if the extension
		''' matches one of the file name extensions of this {@code
		''' FileFilter}, or the file is a directory.
		''' </summary>
		''' <param name="f"> the {@code File} to test </param>
		''' <returns> true if the file is to be accepted, false otherwise </returns>
		Public Overrides Function accept(ByVal f As java.io.File) As Boolean
			If f IsNot Nothing Then
				If f.directory Then Return True
				' NOTE: we tested implementations using Maps, binary search
				' on a sorted list and this implementation. All implementations
				' provided roughly the same speed, most likely because of
				' overhead associated with java.io.File. Therefor we've stuck
				' with the simple lightweight approach.
				Dim fileName As String = f.name
				Dim i As Integer = fileName.LastIndexOf("."c)
				If i > 0 AndAlso i < fileName.Length - 1 Then
					Dim desiredExtension As String = fileName.Substring(i+1).ToLower(java.util.Locale.ENGLISH)
					For Each extension As String In lowerCaseExtensions
						If desiredExtension.Equals(extension) Then Return True
					Next extension
				End If
			End If
			Return False
		End Function

		''' <summary>
		''' The description of this filter. For example: "JPG and GIF Images."
		''' </summary>
		''' <returns> the description of this filter </returns>
		Public Property Overrides description As String
			Get
				Return description
			End Get
		End Property

		''' <summary>
		''' Returns the set of file name extensions files are tested against.
		''' </summary>
		''' <returns> the set of file name extensions files are tested against </returns>
		Public Property extensions As String()
			Get
				Dim result As String() = New String(extensions.Length - 1){}
				Array.Copy(extensions, 0, result, 0, extensions.Length)
				Return result
			End Get
		End Property

		''' <summary>
		''' Returns a string representation of the {@code FileNameExtensionFilter}.
		''' This method is intended to be used for debugging purposes,
		''' and the content and format of the returned string may vary
		''' between implementations.
		''' </summary>
		''' <returns> a string representation of this {@code FileNameExtensionFilter} </returns>
		Public Overrides Function ToString() As String
			Return MyBase.ToString() & "[description=" & description & " extensions=" & extensions & "]"
		End Function
	End Class

End Namespace