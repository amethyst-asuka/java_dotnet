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
Namespace javax.swing.text

	''' <summary>
	''' <code>DocumentFilter</code>, as the name implies, is a filter for the
	''' <code>Document</code> mutation methods. When a <code>Document</code>
	''' containing a <code>DocumentFilter</code> is modified (either through
	''' <code>insert</code> or <code>remove</code>), it forwards the appropriate
	''' method invocation to the <code>DocumentFilter</code>. The
	''' default implementation allows the modification to
	''' occur. Subclasses can filter the modifications by conditionally invoking
	''' methods on the superclass, or invoking the necessary methods on
	''' the passed in <code>FilterBypass</code>. Subclasses should NOT call back
	''' into the Document for the modification
	''' instead call into the superclass or the <code>FilterBypass</code>.
	''' <p>
	''' When <code>remove</code> or <code>insertString</code> is invoked
	''' on the <code>DocumentFilter</code>, the <code>DocumentFilter</code>
	''' may callback into the
	''' <code>FilterBypass</code> multiple times, or for different regions, but
	''' it should not callback into the <code>FilterBypass</code> after returning
	''' from the <code>remove</code> or <code>insertString</code> method.
	''' <p>
	''' By default, text related document mutation methods such as
	''' <code>insertString</code>, <code>replace</code> and <code>remove</code>
	''' in <code>AbstractDocument</code> use <code>DocumentFilter</code> when
	''' available, and <code>Element</code> related mutation methods such as
	''' <code>create</code>, <code>insert</code> and <code>removeElement</code> in
	''' <code>DefaultStyledDocument</code> do not use <code>DocumentFilter</code>.
	''' If a method doesn't follow these defaults, this must be explicitly stated
	''' in the method documentation.
	''' </summary>
	''' <seealso cref= javax.swing.text.Document </seealso>
	''' <seealso cref= javax.swing.text.AbstractDocument </seealso>
	''' <seealso cref= javax.swing.text.DefaultStyledDocument
	''' 
	''' @since 1.4 </seealso>
	Public Class DocumentFilter
		''' <summary>
		''' Invoked prior to removal of the specified region in the
		''' specified Document. Subclasses that want to conditionally allow
		''' removal should override this and only call supers implementation as
		''' necessary, or call directly into the <code>FilterBypass</code> as
		''' necessary.
		''' </summary>
		''' <param name="fb"> FilterBypass that can be used to mutate Document </param>
		''' <param name="offset"> the offset from the beginning &gt;= 0 </param>
		''' <param name="length"> the number of characters to remove &gt;= 0 </param>
		''' <exception cref="BadLocationException">  some portion of the removal range
		'''   was not a valid part of the document.  The location in the exception
		'''   is the first bad position encountered. </exception>
		Public Overridable Sub remove(ByVal fb As FilterBypass, ByVal offset As Integer, ByVal length As Integer)
			fb.remove(offset, length)
		End Sub

		''' <summary>
		''' Invoked prior to insertion of text into the
		''' specified Document. Subclasses that want to conditionally allow
		''' insertion should override this and only call supers implementation as
		''' necessary, or call directly into the FilterBypass.
		''' </summary>
		''' <param name="fb"> FilterBypass that can be used to mutate Document </param>
		''' <param name="offset">  the offset into the document to insert the content &gt;= 0.
		'''    All positions that track change at or after the given location
		'''    will move. </param>
		''' <param name="string"> the string to insert </param>
		''' <param name="attr">      the attributes to associate with the inserted
		'''   content.  This may be null if there are no attributes. </param>
		''' <exception cref="BadLocationException">  the given insert position is not a
		'''   valid position within the document </exception>
		Public Overridable Sub insertString(ByVal fb As FilterBypass, ByVal offset As Integer, ByVal [string] As String, ByVal attr As AttributeSet)
			fb.insertString(offset, [string], attr)
		End Sub

		''' <summary>
		''' Invoked prior to replacing a region of text in the
		''' specified Document. Subclasses that want to conditionally allow
		''' replace should override this and only call supers implementation as
		''' necessary, or call directly into the FilterBypass.
		''' </summary>
		''' <param name="fb"> FilterBypass that can be used to mutate Document </param>
		''' <param name="offset"> Location in Document </param>
		''' <param name="length"> Length of text to delete </param>
		''' <param name="text"> Text to insert, null indicates no text to insert </param>
		''' <param name="attrs"> AttributeSet indicating attributes of inserted text,
		'''              null is legal. </param>
		''' <exception cref="BadLocationException">  the given insert position is not a
		'''   valid position within the document </exception>
		Public Overridable Sub replace(ByVal fb As FilterBypass, ByVal offset As Integer, ByVal length As Integer, ByVal text As String, ByVal attrs As AttributeSet)
			fb.replace(offset, length, text, attrs)
		End Sub


		''' <summary>
		''' Used as a way to circumvent calling back into the Document to
		''' change it. Document implementations that wish to support
		''' a DocumentFilter must provide an implementation that will
		''' not callback into the DocumentFilter when the following methods
		''' are invoked from the DocumentFilter.
		''' @since 1.4
		''' </summary>
		Public MustInherit Class FilterBypass
			''' <summary>
			''' Returns the Document the mutation is occurring on.
			''' </summary>
			''' <returns> Document that remove/insertString will operate on </returns>
			Public MustOverride ReadOnly Property document As Document

			''' <summary>
			''' Removes the specified region of text, bypassing the
			''' DocumentFilter.
			''' </summary>
			''' <param name="offset"> the offset from the beginning &gt;= 0 </param>
			''' <param name="length"> the number of characters to remove &gt;= 0 </param>
			''' <exception cref="BadLocationException"> some portion of the removal range
			'''   was not a valid part of the document.  The location in the
			'''   exception is the first bad position encountered. </exception>
			Public MustOverride Sub remove(ByVal offset As Integer, ByVal length As Integer)

			''' <summary>
			''' Inserts the specified text, bypassing the
			''' DocumentFilter. </summary>
			''' <param name="offset">  the offset into the document to insert the
			'''   content &gt;= 0. All positions that track change at or after the
			'''   given location will move. </param>
			''' <param name="string"> the string to insert </param>
			''' <param name="attr"> the attributes to associate with the inserted
			'''   content.  This may be null if there are no attributes. </param>
			''' <exception cref="BadLocationException">  the given insert position is not a
			'''   valid position within the document </exception>
			Public MustOverride Sub insertString(ByVal offset As Integer, ByVal [string] As String, ByVal attr As AttributeSet)

			''' <summary>
			''' Deletes the region of text from <code>offset</code> to
			''' <code>offset + length</code>, and replaces it with
			'''  <code>text</code>.
			''' </summary>
			''' <param name="offset"> Location in Document </param>
			''' <param name="length"> Length of text to delete </param>
			''' <param name="string"> Text to insert, null indicates no text to insert </param>
			''' <param name="attrs"> AttributeSet indicating attributes of inserted text,
			'''              null is legal. </param>
			''' <exception cref="BadLocationException">  the given insert is not a
			'''   valid position within the document </exception>
			Public MustOverride Sub replace(ByVal offset As Integer, ByVal length As Integer, ByVal [string] As String, ByVal attrs As AttributeSet)
		End Class
	End Class

End Namespace