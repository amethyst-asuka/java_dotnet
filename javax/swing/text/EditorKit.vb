Imports System

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
Namespace javax.swing.text


	''' <summary>
	''' Establishes the set of things needed by a text component
	''' to be a reasonably functioning editor for some <em>type</em>
	''' of text content.  The EditorKit acts as a factory for some
	''' kind of policy.  For example, an implementation
	''' of html and rtf can be provided that is replaceable
	''' with other implementations.
	''' <p>
	''' A kit can safely store editing state as an instance
	''' of the kit will be dedicated to a text component.
	''' New kits will normally be created by cloning a
	''' prototype kit.  The kit will have it's
	''' <code>setComponent</code> method called to establish
	''' it's relationship with a JTextComponent.
	''' 
	''' @author  Timothy Prinzing
	''' </summary>
	<Serializable> _
	Public MustInherit Class EditorKit
		Implements ICloneable

		''' <summary>
		''' Construct an EditorKit.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Creates a copy of the editor kit.  This is implemented
		''' to use <code>Object.clone()</code>.  If the kit cannot be cloned,
		''' null is returned.
		''' </summary>
		''' <returns> the copy </returns>
		Public Overridable Function clone() As Object
			Dim o As Object
			Try
				o = MyBase.clone()
			Catch cnse As CloneNotSupportedException
				o = Nothing
			End Try
			Return o
		End Function

		''' <summary>
		''' Called when the kit is being installed into the
		''' a JEditorPane.
		''' </summary>
		''' <param name="c"> the JEditorPane </param>
		Public Overridable Sub install(ByVal c As javax.swing.JEditorPane)
		End Sub

		''' <summary>
		''' Called when the kit is being removed from the
		''' JEditorPane.  This is used to unregister any
		''' listeners that were attached.
		''' </summary>
		''' <param name="c"> the JEditorPane </param>
		Public Overridable Sub deinstall(ByVal c As javax.swing.JEditorPane)
		End Sub

		''' <summary>
		''' Gets the MIME type of the data that this
		''' kit represents support for.
		''' </summary>
		''' <returns> the type </returns>
		Public MustOverride ReadOnly Property contentType As String

		''' <summary>
		''' Fetches a factory that is suitable for producing
		''' views of any models that are produced by this
		''' kit.
		''' </summary>
		''' <returns> the factory </returns>
		Public MustOverride ReadOnly Property viewFactory As ViewFactory

		''' <summary>
		''' Fetches the set of commands that can be used
		''' on a text component that is using a model and
		''' view produced by this kit.
		''' </summary>
		''' <returns> the set of actions </returns>
		Public MustOverride ReadOnly Property actions As javax.swing.Action()

		''' <summary>
		''' Fetches a caret that can navigate through views
		''' produced by the associated ViewFactory.
		''' </summary>
		''' <returns> the caret </returns>
		Public MustOverride Function createCaret() As Caret

		''' <summary>
		''' Creates an uninitialized text storage model
		''' that is appropriate for this type of editor.
		''' </summary>
		''' <returns> the model </returns>
		Public MustOverride Function createDefaultDocument() As Document

		''' <summary>
		''' Inserts content from the given stream which is expected
		''' to be in a format appropriate for this kind of content
		''' handler.
		''' </summary>
		''' <param name="in">  The stream to read from </param>
		''' <param name="doc"> The destination for the insertion. </param>
		''' <param name="pos"> The location in the document to place the
		'''   content &gt;= 0. </param>
		''' <exception cref="IOException"> on any I/O error </exception>
		''' <exception cref="BadLocationException"> if pos represents an invalid
		'''   location within the document. </exception>
		Public MustOverride Sub read(ByVal [in] As InputStream, ByVal doc As Document, ByVal pos As Integer)

		''' <summary>
		''' Writes content from a document to the given stream
		''' in a format appropriate for this kind of content handler.
		''' </summary>
		''' <param name="out">  The stream to write to </param>
		''' <param name="doc"> The source for the write. </param>
		''' <param name="pos"> The location in the document to fetch the
		'''   content from &gt;= 0. </param>
		''' <param name="len"> The amount to write out &gt;= 0. </param>
		''' <exception cref="IOException"> on any I/O error </exception>
		''' <exception cref="BadLocationException"> if pos represents an invalid
		'''   location within the document. </exception>
		Public MustOverride Sub write(ByVal out As OutputStream, ByVal doc As Document, ByVal pos As Integer, ByVal len As Integer)

		''' <summary>
		''' Inserts content from the given stream which is expected
		''' to be in a format appropriate for this kind of content
		''' handler.
		''' <p>
		''' Since actual text editing is unicode based, this would
		''' generally be the preferred way to read in the data.
		''' Some types of content are stored in an 8-bit form however,
		''' and will favor the InputStream.
		''' </summary>
		''' <param name="in">  The stream to read from </param>
		''' <param name="doc"> The destination for the insertion. </param>
		''' <param name="pos"> The location in the document to place the
		'''   content &gt;= 0. </param>
		''' <exception cref="IOException"> on any I/O error </exception>
		''' <exception cref="BadLocationException"> if pos represents an invalid
		'''   location within the document. </exception>
		Public MustOverride Sub read(ByVal [in] As Reader, ByVal doc As Document, ByVal pos As Integer)

		''' <summary>
		''' Writes content from a document to the given stream
		''' in a format appropriate for this kind of content handler.
		''' <p>
		''' Since actual text editing is unicode based, this would
		''' generally be the preferred way to write the data.
		''' Some types of content are stored in an 8-bit form however,
		''' and will favor the OutputStream.
		''' </summary>
		''' <param name="out">  The stream to write to </param>
		''' <param name="doc"> The source for the write. </param>
		''' <param name="pos"> The location in the document to fetch the
		'''   content &gt;= 0. </param>
		''' <param name="len"> The amount to write out &gt;= 0. </param>
		''' <exception cref="IOException"> on any I/O error </exception>
		''' <exception cref="BadLocationException"> if pos represents an invalid
		'''   location within the document. </exception>
		Public MustOverride Sub write(ByVal out As Writer, ByVal doc As Document, ByVal pos As Integer, ByVal len As Integer)

	End Class

End Namespace