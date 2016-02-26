Imports System
Imports System.Collections
Imports System.Collections.Generic

'
' * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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
' *
' 

'
' * (C) Copyright IBM Corp. 1999,  All rights reserved.
' 
Namespace java.awt.font


	''' <summary>
	''' This class stores Font, GraphicAttribute, and Decoration intervals
	''' on a paragraph of styled text.
	''' <p>
	''' Currently, this class is optimized for a small number of intervals
	''' (preferrably 1).
	''' </summary>
	Friend NotInheritable Class StyledParagraph

		' the length of the paragraph
		Private length As Integer

		' If there is a single Decoration for the whole paragraph, it
		' is stored here.  Otherwise this field is ignored.

		Private decoration As sun.font.Decoration

		' If there is a single Font or GraphicAttribute for the whole
		' paragraph, it is stored here.  Otherwise this field is ignored.
		Private font_Renamed As Object

		' If there are multiple Decorations in the paragraph, they are
		' stored in this Vector, in order.  Otherwise this vector and
		' the decorationStarts array are null.
		Private decorations As List(Of sun.font.Decoration)
		' If there are multiple Decorations in the paragraph,
		' decorationStarts[i] contains the index where decoration i
		' starts.  For convenience, there is an extra entry at the
		' end of this array with the length of the paragraph.
		Friend decorationStarts As Integer()

		' If there are multiple Fonts/GraphicAttributes in the paragraph,
		' they are
		' stored in this Vector, in order.  Otherwise this vector and
		' the fontStarts array are null.
		Private fonts As List(Of Object)
		' If there are multiple Fonts/GraphicAttributes in the paragraph,
		' fontStarts[i] contains the index where decoration i
		' starts.  For convenience, there is an extra entry at the
		' end of this array with the length of the paragraph.
		Friend fontStarts As Integer()

		Private Shared INITIAL_SIZE As Integer = 8

		''' <summary>
		''' Create a new StyledParagraph over the given styled text. </summary>
		''' <param name="aci"> an iterator over the text </param>
		''' <param name="chars"> the characters extracted from aci </param>
		Public Sub New(ByVal aci As java.text.AttributedCharacterIterator, ByVal chars As Char())

			Dim start As Integer = aci.beginIndex
			Dim [end] As Integer = aci.endIndex
			length = [end] - start

			Dim index As Integer = start
			aci.first()

			Do
				Dim nextRunStart As Integer = aci.runLimit
				Dim localIndex As Integer = index-start

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim attributes As IDictionary(Of ? As java.text.AttributedCharacterIterator.Attribute, ?) = aci.attributes
				attributes = addInputMethodAttrs(attributes)
				Dim d As sun.font.Decoration = sun.font.Decoration.getDecoration(attributes)
				addDecoration(d, localIndex)

				Dim f As Object = getGraphicOrFont(attributes)
				If f Is Nothing Then
					addFonts(chars, attributes, localIndex, nextRunStart-start)
				Else
					addFont(f, localIndex)
				End If

				aci.index = nextRunStart
				index = nextRunStart

			Loop While index < [end]

			' Add extra entries to starts arrays with the length
			' of the paragraph.  'this' is used as a dummy value
			' in the Vector.
			If decorations IsNot Nothing Then decorationStarts = addToVector(Me, length, decorations, decorationStarts)
			If fonts IsNot Nothing Then fontStarts = addToVector(Me, length, fonts, fontStarts)
		End Sub

		''' <summary>
		''' Adjust indices in starts to reflect an insertion after pos.
		''' Any index in starts greater than pos will be increased by 1.
		''' </summary>
		Private Shared Sub insertInto(ByVal pos As Integer, ByVal starts As Integer(), ByVal numStarts As Integer)

			numStarts -= 1
			Do While starts(numStarts) > pos
				starts(numStarts) += 1
				numStarts -= 1
			Loop
		End Sub

		''' <summary>
		''' Return a StyledParagraph reflecting the insertion of a single character
		''' into the text.  This method will attempt to reuse the given paragraph,
		''' but may create a new paragraph. </summary>
		''' <param name="aci"> an iterator over the text.  The text should be the same as the
		'''     text used to create (or most recently update) oldParagraph, with
		'''     the exception of inserting a single character at insertPos. </param>
		''' <param name="chars"> the characters in aci </param>
		''' <param name="insertPos"> the index of the new character in aci </param>
		''' <param name="oldParagraph"> a StyledParagraph for the text in aci before the
		'''     insertion </param>
		Public Shared Function insertChar(ByVal aci As java.text.AttributedCharacterIterator, ByVal chars As Char(), ByVal insertPos As Integer, ByVal oldParagraph As StyledParagraph) As StyledParagraph

			' If the styles at insertPos match those at insertPos-1,
			' oldParagraph will be reused.  Otherwise we create a new
			' paragraph.

			Dim ch As Char = aci.indexdex(insertPos)
			Dim relativePos As Integer = Math.Max(insertPos - aci.beginIndex - 1, 0)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim attributes As IDictionary(Of ? As java.text.AttributedCharacterIterator.Attribute, ?) = addInputMethodAttrs(aci.attributes)
			Dim d As sun.font.Decoration = sun.font.Decoration.getDecoration(attributes)
			If Not oldParagraph.getDecorationAt(relativePos).Equals(d) Then Return New StyledParagraph(aci, chars)
			Dim f As Object = getGraphicOrFont(attributes)
			If f Is Nothing Then
				Dim resolver As sun.font.FontResolver = sun.font.FontResolver.instance
				Dim fontIndex As Integer = resolver.getFontIndex(ch)
				f = resolver.getFont(fontIndex, attributes)
			End If
			If Not oldParagraph.getFontOrGraphicAt(relativePos).Equals(f) Then Return New StyledParagraph(aci, chars)

			' insert into existing paragraph
			oldParagraph.length += 1
			If oldParagraph.decorations IsNot Nothing Then insertInto(relativePos, oldParagraph.decorationStarts, oldParagraph.decorations.Count)
			If oldParagraph.fonts IsNot Nothing Then insertInto(relativePos, oldParagraph.fontStarts, oldParagraph.fonts.Count)
			Return oldParagraph
		End Function

		''' <summary>
		''' Adjust indices in starts to reflect a deletion after deleteAt.
		''' Any index in starts greater than deleteAt will be increased by 1.
		''' It is the caller's responsibility to make sure that no 0-length
		''' runs result.
		''' </summary>
		Private Shared Sub deleteFrom(ByVal deleteAt As Integer, ByVal starts As Integer(), ByVal numStarts As Integer)

			numStarts -= 1
			Do While starts(numStarts) > deleteAt
				starts(numStarts) -= 1
				numStarts -= 1
			Loop
		End Sub

		''' <summary>
		''' Return a StyledParagraph reflecting the insertion of a single character
		''' into the text.  This method will attempt to reuse the given paragraph,
		''' but may create a new paragraph. </summary>
		''' <param name="aci"> an iterator over the text.  The text should be the same as the
		'''     text used to create (or most recently update) oldParagraph, with
		'''     the exception of deleting a single character at deletePos. </param>
		''' <param name="chars"> the characters in aci </param>
		''' <param name="deletePos"> the index where a character was removed </param>
		''' <param name="oldParagraph"> a StyledParagraph for the text in aci before the
		'''     insertion </param>
		Public Shared Function deleteChar(ByVal aci As java.text.AttributedCharacterIterator, ByVal chars As Char(), ByVal deletePos As Integer, ByVal oldParagraph As StyledParagraph) As StyledParagraph

			' We will reuse oldParagraph unless there was a length-1 run
			' at deletePos.  We could do more work and check the individual
			' Font and Decoration runs, but we don't right now...
			deletePos -= aci.beginIndex

			If oldParagraph.decorations Is Nothing AndAlso oldParagraph.fonts Is Nothing Then
				oldParagraph.length -= 1
				Return oldParagraph
			End If

			If oldParagraph.getRunLimit(deletePos) = deletePos+1 Then
				If deletePos = 0 OrElse oldParagraph.getRunLimit(deletePos-1) = deletePos Then Return New StyledParagraph(aci, chars)
			End If

			oldParagraph.length -= 1
			If oldParagraph.decorations IsNot Nothing Then deleteFrom(deletePos, oldParagraph.decorationStarts, oldParagraph.decorations.Count)
			If oldParagraph.fonts IsNot Nothing Then deleteFrom(deletePos, oldParagraph.fontStarts, oldParagraph.fonts.Count)
			Return oldParagraph
		End Function

		''' <summary>
		''' Return the index at which there is a different Font, GraphicAttribute, or
		''' Dcoration than at the given index. </summary>
		''' <param name="index"> a valid index in the paragraph </param>
		''' <returns> the first index where there is a change in attributes from
		'''      those at index </returns>
		Public Function getRunLimit(ByVal index As Integer) As Integer

			If index < 0 OrElse index >= length Then Throw New IllegalArgumentException("index out of range")
			Dim limit1 As Integer = length
			If decorations IsNot Nothing Then
				Dim run As Integer = findRunContaining(index, decorationStarts)
				limit1 = decorationStarts(run+1)
			End If
			Dim limit2 As Integer = length
			If fonts IsNot Nothing Then
				Dim run As Integer = findRunContaining(index, fontStarts)
				limit2 = fontStarts(run+1)
			End If
			Return Math.Min(limit1, limit2)
		End Function

		''' <summary>
		''' Return the Decoration in effect at the given index. </summary>
		''' <param name="index"> a valid index in the paragraph </param>
		''' <returns> the Decoration at index. </returns>
		Public Function getDecorationAt(ByVal index As Integer) As sun.font.Decoration

			If index < 0 OrElse index >= length Then Throw New IllegalArgumentException("index out of range")
			If decorations Is Nothing Then Return decoration
			Dim run As Integer = findRunContaining(index, decorationStarts)
			Return decorations(run)
		End Function

		''' <summary>
		''' Return the Font or GraphicAttribute in effect at the given index.
		''' The client must test the type of the return value to determine what
		''' it is. </summary>
		''' <param name="index"> a valid index in the paragraph </param>
		''' <returns> the Font or GraphicAttribute at index. </returns>
		Public Function getFontOrGraphicAt(ByVal index As Integer) As Object

			If index < 0 OrElse index >= length Then Throw New IllegalArgumentException("index out of range")
			If fonts Is Nothing Then Return font_Renamed
			Dim run As Integer = findRunContaining(index, fontStarts)
			Return fonts(run)
		End Function

		''' <summary>
		''' Return i such that starts[i] &lt;= index &lt; starts[i+1].  starts
		''' must be in increasing order, with at least one element greater
		''' than index.
		''' </summary>
		Private Shared Function findRunContaining(ByVal index As Integer, ByVal starts As Integer()) As Integer

			Dim i As Integer=1
			Do
				If starts(i) > index Then Return i-1
				i += 1
			Loop
		End Function

		''' <summary>
		''' Append the given Object to the given Vector.  Add
		''' the given index to the given starts array.  If the
		''' starts array does not have room for the index, a
		''' new array is created and returned.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Shared Function addToVector(ByVal obj As Object, ByVal index As Integer, ByVal v As ArrayList, ByVal starts As Integer()) As Integer()

			If Not v(v.Count - 1).Equals(obj) Then
				v.Add(obj)
				Dim count As Integer = v.Count
				If starts.Length = count Then
					Dim temp As Integer() = New Integer(starts.Length*2 - 1){}
					Array.Copy(starts, 0, temp, 0, starts.Length)
					starts = temp
				End If
				starts(count-1) = index
			End If
			Return starts
		End Function

		''' <summary>
		''' Add a new Decoration run with the given Decoration at the
		''' given index.
		''' </summary>
		Private Sub addDecoration(ByVal d As sun.font.Decoration, ByVal index As Integer)

			If decorations IsNot Nothing Then
				decorationStarts = addToVector(d, index, decorations, decorationStarts)
			ElseIf decoration Is Nothing Then
				decoration = d
			Else
				If Not decoration.Equals(d) Then
					decorations = New List(Of sun.font.Decoration)(INITIAL_SIZE)
					decorations.Add(decoration)
					decorations.Add(d)
					decorationStarts = New Integer(INITIAL_SIZE - 1){}
					decorationStarts(0) = 0
					decorationStarts(1) = index
				End If
			End If
		End Sub

		''' <summary>
		''' Add a new Font/GraphicAttribute run with the given object at the
		''' given index.
		''' </summary>
		Private Sub addFont(ByVal f As Object, ByVal index As Integer)

			If fonts IsNot Nothing Then
				fontStarts = addToVector(f, index, fonts, fontStarts)
			ElseIf font_Renamed Is Nothing Then
				font_Renamed = f
			Else
				If Not font_Renamed.Equals(f) Then
					fonts = New List(Of Object)(INITIAL_SIZE)
					fonts.Add(font_Renamed)
					fonts.Add(f)
					fontStarts = New Integer(INITIAL_SIZE - 1){}
					fontStarts(0) = 0
					fontStarts(1) = index
				End If
			End If
		End Sub

		''' <summary>
		''' Resolve the given chars into Fonts using FontResolver, then add
		''' font runs for each.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private Sub addFonts(Of T1 As java.text.AttributedCharacterIterator.Attribute, ?)(ByVal chars As Char(), ByVal attributes As IDictionary(Of T1), ByVal start As Integer, ByVal limit As Integer)

			Dim resolver As sun.font.FontResolver = sun.font.FontResolver.instance
			Dim iter As sun.text.CodePointIterator = sun.text.CodePointIterator.create(chars, start, limit)
			Dim runStart As Integer = iter.charIndex()
			Do While runStart < limit
				Dim fontIndex As Integer = resolver.nextFontRunIndex(iter)
				addFont(resolver.getFont(fontIndex, attributes), runStart)
				runStart = iter.charIndex()
			Loop
		End Sub

		''' <summary>
		''' Return a Map with entries from oldStyles, as well as input
		''' method entries, if any.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Friend Shared Function addInputMethodAttrs(Of T1 As java.text.AttributedCharacterIterator.Attribute, ?)(ByVal oldStyles As IDictionary(Of T1)) As IDictionary(Of ? As java.text.AttributedCharacterIterator.Attribute, ?)

			Dim value As Object = oldStyles(TextAttribute.INPUT_METHOD_HIGHLIGHT)

			Try
				If value IsNot Nothing Then
					If TypeOf value Is java.text.Annotation Then value = CType(value, java.text.Annotation).value

					Dim hl As java.awt.im.InputMethodHighlight
					hl = CType(value, java.awt.im.InputMethodHighlight)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim imStyles As IDictionary(Of ? As java.text.AttributedCharacterIterator.Attribute, ?) = Nothing
					Try
						imStyles = hl.style
					Catch e As NoSuchMethodError
					End Try

					If imStyles Is Nothing Then
						Dim tk As java.awt.Toolkit = java.awt.Toolkit.defaultToolkit
						imStyles = tk.mapInputMethodHighlight(hl)
					End If

					If imStyles IsNot Nothing Then
						Dim newStyles As New Dictionary(Of java.text.AttributedCharacterIterator.Attribute, Object)(5, CSng(0.9))
'JAVA TO VB CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
						newStyles.putAll(oldStyles)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
						newStyles.putAll(imStyles)

						Return newStyles
					End If
				End If
			Catch e As  [Class]CastException
			End Try

			Return oldStyles
		End Function

		''' <summary>
		''' Extract a GraphicAttribute or Font from the given attributes.
		''' If attributes does not contain a GraphicAttribute, Font, or
		''' Font family entry this method returns null.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private Shared Function getGraphicOrFont(Of T1 As java.text.AttributedCharacterIterator.Attribute, ?)(ByVal attributes As IDictionary(Of T1)) As Object

			Dim value As Object = attributes(TextAttribute.CHAR_REPLACEMENT)
			If value IsNot Nothing Then Return value
			value = attributes(TextAttribute.FONT)
			If value IsNot Nothing Then Return value

			If attributes(TextAttribute.FAMILY) IsNot Nothing Then
				Return java.awt.Font.getFont(attributes)
			Else
				Return Nothing
			End If
		End Function
	End Class

End Namespace