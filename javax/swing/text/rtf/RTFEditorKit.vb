Imports javax.swing.text
Imports javax.swing

'
' * Copyright (c) 1997, 2000, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.text.rtf

	''' <summary>
	''' This is the default implementation of RTF editing
	''' functionality.  The RTF support was not written by the
	''' Swing team.  In the future we hope to improve the support
	''' provided.
	''' 
	''' @author  Timothy Prinzing (of this class, not the package!)
	''' </summary>
	Public Class RTFEditorKit
		Inherits StyledEditorKit

		''' <summary>
		''' Constructs an RTFEditorKit.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Get the MIME type of the data that this
		''' kit represents support for.  This kit supports
		''' the type <code>text/rtf</code>.
		''' </summary>
		''' <returns> the type </returns>
		Public Property Overrides contentType As String
			Get
				Return "text/rtf"
			End Get
		End Property

		''' <summary>
		''' Insert content from the given stream which is expected
		''' to be in a format appropriate for this kind of content
		''' handler.
		''' </summary>
		''' <param name="in">  The stream to read from </param>
		''' <param name="doc"> The destination for the insertion. </param>
		''' <param name="pos"> The location in the document to place the
		'''   content. </param>
		''' <exception cref="IOException"> on any I/O error </exception>
		''' <exception cref="BadLocationException"> if pos represents an invalid
		'''   location within the document. </exception>
		Public Overrides Sub read(ByVal [in] As InputStream, ByVal doc As Document, ByVal pos As Integer)

			If TypeOf doc Is StyledDocument Then
				' PENDING(prinz) this needs to be fixed to
				' insert to the given position.
				Dim rdr As New RTFReader(CType(doc, StyledDocument))
				rdr.readFromStream([in])
				rdr.close()
			Else
				' treat as text/plain
				MyBase.read([in], doc, pos)
			End If
		End Sub

		''' <summary>
		''' Write content from a document to the given stream
		''' in a format appropriate for this kind of content handler.
		''' </summary>
		''' <param name="out">  The stream to write to </param>
		''' <param name="doc"> The source for the write. </param>
		''' <param name="pos"> The location in the document to fetch the
		'''   content. </param>
		''' <param name="len"> The amount to write out. </param>
		''' <exception cref="IOException"> on any I/O error </exception>
		''' <exception cref="BadLocationException"> if pos represents an invalid
		'''   location within the document. </exception>
		Public Overrides Sub write(ByVal out As OutputStream, ByVal doc As Document, ByVal pos As Integer, ByVal len As Integer)

				' PENDING(prinz) this needs to be fixed to
				' use the given document range.
				RTFGenerator.writeDocument(doc, out)
		End Sub

		''' <summary>
		''' Insert content from the given stream, which will be
		''' treated as plain text.
		''' </summary>
		''' <param name="in">  The stream to read from </param>
		''' <param name="doc"> The destination for the insertion. </param>
		''' <param name="pos"> The location in the document to place the
		'''   content. </param>
		''' <exception cref="IOException"> on any I/O error </exception>
		''' <exception cref="BadLocationException"> if pos represents an invalid
		'''   location within the document. </exception>
		Public Overrides Sub read(ByVal [in] As Reader, ByVal doc As Document, ByVal pos As Integer)

			If TypeOf doc Is StyledDocument Then
				Dim rdr As New RTFReader(CType(doc, StyledDocument))
				rdr.readFromReader([in])
				rdr.close()
			Else
				' treat as text/plain
				MyBase.read([in], doc, pos)
			End If
		End Sub

		''' <summary>
		''' Write content from a document to the given stream
		''' as plain text.
		''' </summary>
		''' <param name="out">  The stream to write to </param>
		''' <param name="doc"> The source for the write. </param>
		''' <param name="pos"> The location in the document to fetch the
		'''   content. </param>
		''' <param name="len"> The amount to write out. </param>
		''' <exception cref="IOException"> on any I/O error </exception>
		''' <exception cref="BadLocationException"> if pos represents an invalid
		'''   location within the document. </exception>
		Public Overrides Sub write(ByVal out As Writer, ByVal doc As Document, ByVal pos As Integer, ByVal len As Integer)

			Throw New IOException("RTF is an 8-bit format")
		End Sub

	End Class

End Namespace