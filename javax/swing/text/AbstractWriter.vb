Imports Microsoft.VisualBasic
Imports System
Imports System.Collections

'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' AbstractWriter is an abstract class that actually
	''' does the work of writing out the element tree
	''' including the attributes.  In terms of how much is
	''' written out per line, the writer defaults to 100.
	''' But this value can be set by subclasses.
	''' 
	''' @author Sunita Mani
	''' </summary>

	Public MustInherit Class AbstractWriter

		Private it As ElementIterator
		Private out As java.io.Writer
		Private indentLevel As Integer = 0
		Private indentSpace As Integer = 2
		Private doc As Document = Nothing
		Private maxLineLength As Integer = 100
		Private currLength As Integer = 0
		Private startOffset As Integer = 0
		Private endOffset As Integer = 0
		' If (indentLevel * indentSpace) becomes >= maxLineLength, this will
		' get incremened instead of indentLevel to avoid indenting going greater
		' than line length.
		Private offsetIndent As Integer = 0

		''' <summary>
		''' String used for end of line. If the Document has the property
		''' EndOfLineStringProperty, it will be used for newlines. Otherwise
		''' the System property line.separator will be used. The line separator
		''' can also be set.
		''' </summary>
		Private lineSeparator As String

		''' <summary>
		''' True indicates that when writing, the line can be split, false
		''' indicates that even if the line is > than max line length it should
		''' not be split.
		''' </summary>
		Private canWrapLines As Boolean

		''' <summary>
		''' True while the current line is empty. This will remain true after
		''' indenting.
		''' </summary>
		Private ___isLineEmpty As Boolean

		''' <summary>
		''' Used when indenting. Will contain the spaces.
		''' </summary>
		Private indentChars As Char()

		''' <summary>
		''' Used when writing out a string.
		''' </summary>
		Private tempChars As Char()

		''' <summary>
		''' This is used in <code>writeLineSeparator</code> instead of
		''' tempChars. If tempChars were used it would mean write couldn't invoke
		''' <code>writeLineSeparator</code> as it might have been passed
		''' tempChars.
		''' </summary>
		Private newlineChars As Char()

		''' <summary>
		''' Used for writing text.
		''' </summary>
		Private segment As Segment

		''' <summary>
		''' How the text packages models newlines. </summary>
		''' <seealso cref= #getLineSeparator </seealso>
		Protected Friend Shared ReadOnly NEWLINE As Char = ControlChars.Lf


		''' <summary>
		''' Creates a new AbstractWriter.
		''' Initializes the ElementIterator with the default
		''' root of the document.
		''' </summary>
		''' <param name="w"> a Writer. </param>
		''' <param name="doc"> a Document </param>
		Protected Friend Sub New(ByVal w As java.io.Writer, ByVal doc As Document)
			Me.New(w, doc, 0, doc.length)
		End Sub

		''' <summary>
		''' Creates a new AbstractWriter.
		''' Initializes the ElementIterator with the
		''' element passed in.
		''' </summary>
		''' <param name="w"> a Writer </param>
		''' <param name="doc"> an Element </param>
		''' <param name="pos"> The location in the document to fetch the
		'''   content. </param>
		''' <param name="len"> The amount to write out. </param>
		Protected Friend Sub New(ByVal w As java.io.Writer, ByVal doc As Document, ByVal pos As Integer, ByVal len As Integer)
			Me.doc = doc
			it = New ElementIterator(doc.defaultRootElement)
			out = w
			startOffset = pos
			endOffset = pos + len
			Dim docNewline As Object = doc.getProperty(DefaultEditorKit.EndOfLineStringProperty)
			If TypeOf docNewline Is String Then
				lineSeparator = CStr(docNewline)
			Else
				Dim ___newline As String = Nothing
				Try
					___newline = System.getProperty("line.separator")
				Catch se As SecurityException
				End Try
				If ___newline Is Nothing Then ___newline = vbLf
				lineSeparator = ___newline
			End If
			canWrapLines = True
		End Sub

		''' <summary>
		''' Creates a new AbstractWriter.
		''' Initializes the ElementIterator with the
		''' element passed in.
		''' </summary>
		''' <param name="w"> a Writer </param>
		''' <param name="root"> an Element </param>
		Protected Friend Sub New(ByVal w As java.io.Writer, ByVal root As Element)
			Me.New(w, root, 0, root.endOffset)
		End Sub

		''' <summary>
		''' Creates a new AbstractWriter.
		''' Initializes the ElementIterator with the
		''' element passed in.
		''' </summary>
		''' <param name="w"> a Writer </param>
		''' <param name="root"> an Element </param>
		''' <param name="pos"> The location in the document to fetch the
		'''   content. </param>
		''' <param name="len"> The amount to write out. </param>
		Protected Friend Sub New(ByVal w As java.io.Writer, ByVal root As Element, ByVal pos As Integer, ByVal len As Integer)
			Me.doc = root.document
			it = New ElementIterator(root)
			out = w
			startOffset = pos
			endOffset = pos + len
			canWrapLines = True
		End Sub

		''' <summary>
		''' Returns the first offset to be output.
		''' 
		''' @since 1.3
		''' </summary>
		Public Overridable Property startOffset As Integer
			Get
				Return startOffset
			End Get
		End Property

		''' <summary>
		''' Returns the last offset to be output.
		''' 
		''' @since 1.3
		''' </summary>
		Public Overridable Property endOffset As Integer
			Get
				Return endOffset
			End Get
		End Property

		''' <summary>
		''' Fetches the ElementIterator.
		''' </summary>
		''' <returns> the ElementIterator. </returns>
		Protected Friend Overridable Property elementIterator As ElementIterator
			Get
				Return it
			End Get
		End Property

		''' <summary>
		''' Returns the Writer that is used to output the content.
		''' 
		''' @since 1.3
		''' </summary>
		Protected Friend Overridable Property writer As java.io.Writer
			Get
				Return out
			End Get
		End Property

		''' <summary>
		''' Fetches the document.
		''' </summary>
		''' <returns> the Document. </returns>
		Protected Friend Overridable Property document As Document
			Get
				Return doc
			End Get
		End Property

		''' <summary>
		''' This method determines whether the current element
		''' is in the range specified.  When no range is specified,
		''' the range is initialized to be the entire document.
		''' inRange() returns true if the range specified intersects
		''' with the element's range.
		''' </summary>
		''' <param name="next"> an Element. </param>
		''' <returns> boolean that indicates whether the element
		'''         is in the range. </returns>
		Protected Friend Overridable Function inRange(ByVal [next] As Element) As Boolean
			Dim ___startOffset As Integer = startOffset
			Dim ___endOffset As Integer = endOffset
			If ([next].startOffset >= ___startOffset AndAlso [next].startOffset < ___endOffset) OrElse (___startOffset >= [next].startOffset AndAlso ___startOffset < [next].endOffset) Then Return True
			Return False
		End Function

		''' <summary>
		''' This abstract method needs to be implemented
		''' by subclasses.  Its responsibility is to
		''' iterate over the elements and use the write()
		''' methods to generate output in the desired format.
		''' </summary>
		Protected Friend MustOverride Sub write()

		''' <summary>
		''' Returns the text associated with the element.
		''' The assumption here is that the element is a
		''' leaf element.  Throws a BadLocationException
		''' when encountered.
		''' </summary>
		''' <param name="elem"> an <code>Element</code> </param>
		''' <exception cref="BadLocationException"> if pos represents an invalid
		'''            location within the document </exception>
		''' <returns>    the text as a <code>String</code> </returns>
		Protected Friend Overridable Function getText(ByVal elem As Element) As String
			Return doc.getText(elem.startOffset, elem.endOffset - elem.startOffset)
		End Function


		''' <summary>
		''' Writes out text.  If a range is specified when the constructor
		''' is invoked, then only the appropriate range of text is written
		''' out.
		''' </summary>
		''' <param name="elem"> an Element. </param>
		''' <exception cref="IOException"> on any I/O error </exception>
		''' <exception cref="BadLocationException"> if pos represents an invalid
		'''            location within the document. </exception>
		Protected Friend Overridable Sub text(ByVal elem As Element)
			Dim start As Integer = Math.Max(startOffset, elem.startOffset)
			Dim [end] As Integer = Math.Min(endOffset, elem.endOffset)
			If start < [end] Then
				If segment Is Nothing Then segment = New Segment
				document.getText(start, [end] - start, segment)
				If segment.count > 0 Then write(segment.array, segment.offset, segment.count)
			End If
		End Sub

		''' <summary>
		''' Enables subclasses to set the number of characters they
		''' want written per line.   The default is 100.
		''' </summary>
		''' <param name="l"> the maximum line length. </param>
		Protected Friend Overridable Property lineLength As Integer
			Set(ByVal l As Integer)
				maxLineLength = l
			End Set
			Get
				Return maxLineLength
			End Get
		End Property


		''' <summary>
		''' Sets the current line length.
		''' 
		''' @since 1.3
		''' </summary>
		Protected Friend Overridable Property currentLineLength As Integer
			Set(ByVal length As Integer)
				currLength = length
				___isLineEmpty = (currLength = 0)
			End Set
			Get
				Return currLength
			End Get
		End Property


		''' <summary>
		''' Returns true if the current line should be considered empty. This
		''' is true when <code>getCurrentLineLength</code> == 0 ||
		''' <code>indent</code> has been invoked on an empty line.
		''' 
		''' @since 1.3
		''' </summary>
		Protected Friend Overridable Property lineEmpty As Boolean
			Get
				Return ___isLineEmpty
			End Get
		End Property

		''' <summary>
		''' Sets whether or not lines can be wrapped. This can be toggled
		''' during the writing of lines. For example, outputting HTML might
		''' set this to false when outputting a quoted string.
		''' 
		''' @since 1.3
		''' </summary>
		Protected Friend Overridable Property canWrapLines As Boolean
			Set(ByVal newValue As Boolean)
				canWrapLines = newValue
			End Set
			Get
				Return canWrapLines
			End Get
		End Property


		''' <summary>
		''' Enables subclasses to specify how many spaces an indent
		''' maps to. When indentation takes place, the indent level
		''' is multiplied by this mapping.  The default is 2.
		''' </summary>
		''' <param name="space"> an int representing the space to indent mapping. </param>
		Protected Friend Overridable Property indentSpace As Integer
			Set(ByVal space As Integer)
				indentSpace = space
			End Set
			Get
				Return indentSpace
			End Get
		End Property


		''' <summary>
		''' Sets the String used to represent newlines. This is initialized
		''' in the constructor from either the Document, or the System property
		''' line.separator.
		''' 
		''' @since 1.3
		''' </summary>
		Public Overridable Property lineSeparator As String
			Set(ByVal value As String)
				lineSeparator = value
			End Set
			Get
				Return lineSeparator
			End Get
		End Property


		''' <summary>
		''' Increments the indent level. If indenting would cause
		''' <code>getIndentSpace()</code> *<code>getIndentLevel()</code> to be &gt;
		''' than <code>getLineLength()</code> this will not cause an indent.
		''' </summary>
		Protected Friend Overridable Sub incrIndent()
			' Only increment to a certain point.
			If offsetIndent > 0 Then
				offsetIndent += 1
			Else
				indentLevel += 1
				If indentLevel * indentSpace >= lineLength Then
					offsetIndent += 1
					indentLevel -= 1
				End If
			End If
		End Sub

		''' <summary>
		''' Decrements the indent level.
		''' </summary>
		Protected Friend Overridable Sub decrIndent()
			If offsetIndent > 0 Then
				offsetIndent -= 1
			Else
				indentLevel -= 1
			End If
		End Sub

		''' <summary>
		''' Returns the current indentation level. That is, the number of times
		''' <code>incrIndent</code> has been invoked minus the number of times
		''' <code>decrIndent</code> has been invoked.
		''' 
		''' @since 1.3
		''' </summary>
		Protected Friend Overridable Property indentLevel As Integer
			Get
				Return indentLevel
			End Get
		End Property

		''' <summary>
		''' Does indentation. The number of spaces written
		''' out is indent level times the space to map mapping. If the current
		''' line is empty, this will not make it so that the current line is
		''' still considered empty.
		''' </summary>
		''' <exception cref="IOException"> on any I/O error </exception>
		Protected Friend Overridable Sub indent()
			Dim max As Integer = indentLevel * indentSpace
			If indentChars Is Nothing OrElse max > indentChars.Length Then
				indentChars = New Char(max - 1){}
				For counter As Integer = 0 To max - 1
					indentChars(counter) = " "c
				Next counter
			End If
			Dim length As Integer = currentLineLength
			Dim wasEmpty As Boolean = lineEmpty
			output(indentChars, 0, max)
			If wasEmpty AndAlso length = 0 Then ___isLineEmpty = True
		End Sub

		''' <summary>
		''' Writes out a character. This is implemented to invoke
		''' the <code>write</code> method that takes a char[].
		''' </summary>
		''' <param name="ch"> a char. </param>
		''' <exception cref="IOException"> on any I/O error </exception>
		Protected Friend Overridable Sub write(ByVal ch As Char)
			If tempChars Is Nothing Then tempChars = New Char(127){}
			tempChars(0) = ch
			write(tempChars, 0, 1)
		End Sub

		''' <summary>
		''' Writes out a string. This is implemented to invoke the
		''' <code>write</code> method that takes a char[].
		''' </summary>
		''' <param name="content"> a String. </param>
		''' <exception cref="IOException"> on any I/O error </exception>
		Protected Friend Overridable Sub write(ByVal content As String)
			If content Is Nothing Then Return
			Dim size As Integer = content.Length
			If tempChars Is Nothing OrElse tempChars.Length < size Then tempChars = New Char(size - 1){}
			content.getChars(0, size, tempChars, 0)
			write(tempChars, 0, size)
		End Sub

		''' <summary>
		''' Writes the line separator. This invokes <code>output</code> directly
		''' as well as setting the <code>lineLength</code> to 0.
		''' 
		''' @since 1.3
		''' </summary>
		Protected Friend Overridable Sub writeLineSeparator()
			Dim ___newline As String = lineSeparator
			Dim length As Integer = ___newline.Length
			If newlineChars Is Nothing OrElse newlineChars.Length < length Then newlineChars = New Char(length - 1){}
			___newline.getChars(0, length, newlineChars, 0)
			output(newlineChars, 0, length)
			currentLineLength = 0
		End Sub

		''' <summary>
		''' All write methods call into this one. If <code>getCanWrapLines()</code>
		''' returns false, this will call <code>output</code> with each sequence
		''' of <code>chars</code> that doesn't contain a NEWLINE, followed
		''' by a call to <code>writeLineSeparator</code>. On the other hand,
		''' if <code>getCanWrapLines()</code> returns true, this will split the
		''' string, as necessary, so <code>getLineLength</code> is honored.
		''' The only exception is if the current string contains no whitespace,
		''' and won't fit in which case the line length will exceed
		''' <code>getLineLength</code>.
		''' 
		''' @since 1.3
		''' </summary>
		Protected Friend Overridable Sub write(ByVal chars As Char(), ByVal startIndex As Integer, ByVal length As Integer)
			If Not canWrapLines Then
				' We can not break string, just track if a newline
				' is in it.
				Dim lastIndex As Integer = startIndex
				Dim endIndex As Integer = startIndex + length
				Dim newlineIndex As Integer = IndexOf(chars, NEWLINE, startIndex, endIndex)
				Do While newlineIndex <> -1
					If newlineIndex > lastIndex Then output(chars, lastIndex, newlineIndex - lastIndex)
					writeLineSeparator()
					lastIndex = newlineIndex + 1
					newlineIndex = IndexOf(chars, ControlChars.Lf, lastIndex, endIndex)
				Loop
				If lastIndex < endIndex Then output(chars, lastIndex, endIndex - lastIndex)
			Else
				' We can break chars if the length exceeds maxLength.
				Dim lastIndex As Integer = startIndex
				Dim endIndex As Integer = startIndex + length
				Dim ___lineLength As Integer = currentLineLength
				Dim maxLength As Integer = lineLength

				Do While lastIndex < endIndex
					Dim newlineIndex As Integer = IndexOf(chars, NEWLINE, lastIndex, endIndex)
					Dim needsNewline As Boolean = False
					Dim forceNewLine As Boolean = False

					___lineLength = currentLineLength
					If newlineIndex <> -1 AndAlso (___lineLength + (newlineIndex - lastIndex)) < maxLength Then
						If newlineIndex > lastIndex Then output(chars, lastIndex, newlineIndex - lastIndex)
						lastIndex = newlineIndex + 1
						forceNewLine = True
					ElseIf newlineIndex = -1 AndAlso (___lineLength + (endIndex - lastIndex)) < maxLength Then
						If endIndex > lastIndex Then output(chars, lastIndex, endIndex - lastIndex)
						lastIndex = endIndex
					Else
						' Need to break chars, find a place to split chars at,
						' from lastIndex to endIndex,
						' or maxLength - lineLength whichever is smaller
						Dim breakPoint As Integer = -1
						Dim maxBreak As Integer = Math.Min(endIndex - lastIndex, maxLength - ___lineLength - 1)
						Dim counter As Integer = 0
						Do While counter < maxBreak
							If Char.IsWhiteSpace(chars(counter + lastIndex)) Then breakPoint = counter
							counter += 1
						Loop
						If breakPoint <> -1 Then
							' Found a place to break at.
							breakPoint += lastIndex + 1
							output(chars, lastIndex, breakPoint - lastIndex)
							lastIndex = breakPoint
							needsNewline = True
						Else
							' No where good to break.

							' find the next whitespace, or write out the
							' whole string.
								' maxBreak will be negative if current line too
								' long.
								counter = Math.Max(0, maxBreak)
								maxBreak = endIndex - lastIndex
								Do While counter < maxBreak
									If Char.IsWhiteSpace(chars(counter + lastIndex)) Then
										breakPoint = counter
										Exit Do
									End If
									counter += 1
								Loop
								If breakPoint = -1 Then
									output(chars, lastIndex, endIndex - lastIndex)
									breakPoint = endIndex
								Else
									breakPoint += lastIndex
									If chars(breakPoint) = NEWLINE Then
										output(chars, lastIndex, breakPoint - lastIndex)
										breakPoint += 1
									forceNewLine = True
									Else
										breakPoint += 1
										output(chars, lastIndex, breakPoint - lastIndex)
									needsNewline = True
									End If
								End If
								lastIndex = breakPoint
						End If
					End If
					If forceNewLine OrElse needsNewline OrElse lastIndex < endIndex Then
						writeLineSeparator()
						If lastIndex < endIndex OrElse (Not forceNewLine) Then indent()
					End If
				Loop
			End If
		End Sub

		''' <summary>
		''' Writes out the set of attributes as " &lt;name&gt;=&lt;value&gt;"
		''' pairs. It throws an IOException when encountered.
		''' </summary>
		''' <param name="attr"> an AttributeSet. </param>
		''' <exception cref="IOException"> on any I/O error </exception>
		Protected Friend Overridable Sub writeAttributes(ByVal attr As AttributeSet)

			Dim names As System.Collections.IEnumerator = attr.attributeNames
			Do While names.hasMoreElements()
				Dim name As Object = names.nextElement()
				write(" " & name & "=" & attr.getAttribute(name))
			Loop
		End Sub

		''' <summary>
		''' The last stop in writing out content. All the write methods eventually
		''' make it to this method, which invokes <code>write</code> on the
		''' Writer.
		''' <p>This method also updates the line length based on
		''' <code>length</code>. If this is invoked to output a newline, the
		''' current line length will need to be reset as will no longer be
		''' valid. If it is up to the caller to do this. Use
		''' <code>writeLineSeparator</code> to write out a newline, which will
		''' property update the current line length.
		''' 
		''' @since 1.3
		''' </summary>
		Protected Friend Overridable Sub output(ByVal content As Char(), ByVal start As Integer, ByVal length As Integer)
			writer.write(content, start, length)
			currentLineLength = currentLineLength + length
		End Sub

		''' <summary>
		''' Support method to locate an occurrence of a particular character.
		''' </summary>
		Private Function indexOf(ByVal chars As Char(), ByVal sChar As Char, ByVal startIndex As Integer, ByVal endIndex As Integer) As Integer
			Do While startIndex < endIndex
				If chars(startIndex) = sChar Then Return startIndex
				startIndex += 1
			Loop
			Return -1
		End Function
	End Class

End Namespace