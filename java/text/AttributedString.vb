Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic

'
' * Copyright (c) 1997, 2012, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.text


	''' <summary>
	''' An AttributedString holds text and related attribute information. It
	''' may be used as the actual data storage in some cases where a text
	''' reader wants to access attributed text through the AttributedCharacterIterator
	''' interface.
	''' 
	''' <p>
	''' An attribute is a key/value pair, identified by the key.  No two
	''' attributes on a given character can have the same key.
	''' 
	''' <p>The values for an attribute are immutable, or must not be mutated
	''' by clients or storage.  They are always passed by reference, and not
	''' cloned.
	''' </summary>
	''' <seealso cref= AttributedCharacterIterator </seealso>
	''' <seealso cref= Annotation
	''' @since 1.2 </seealso>

	Public Class AttributedString

		' since there are no vectors of int, we have to use arrays.
		' We allocate them in chunks of 10 elements so we don't have to allocate all the time.
		Private Const ARRAY_SIZE_INCREMENT As Integer = 10

		' field holding the text
		Friend text As String

		' fields holding run attribute information
		' run attributes are organized by run
		Friend runArraySize As Integer ' current size of the arrays
		Friend runCount As Integer ' actual number of runs, <= runArraySize
		Friend runStarts As Integer() ' start index for each run
		Friend runAttributes As Vector(Of java.text.AttributedCharacterIterator.Attribute)() ' vector of attribute keys for each run
		Friend runAttributeValues As Vector(Of Object)() ' parallel vector of attribute values for each run

		''' <summary>
		''' Constructs an AttributedString instance with the given
		''' AttributedCharacterIterators.
		''' </summary>
		''' <param name="iterators"> AttributedCharacterIterators to construct
		''' AttributedString from. </param>
		''' <exception cref="NullPointerException"> if iterators is null </exception>
		Friend Sub New(ByVal iterators As AttributedCharacterIterator())
			If iterators Is Nothing Then Throw New NullPointerException("Iterators must not be null")
			If iterators.Length = 0 Then
				text = ""
			Else
				' Build the String contents
				Dim buffer As New StringBuffer
				For counter As Integer = 0 To iterators.Length - 1
					appendContents(buffer, iterators(counter))
				Next counter

				text = buffer.ToString()

				If text.length() > 0 Then
					' Determine the runs, creating a new run when the attributes
					' differ.
					Dim offset As Integer = 0
					Dim last As Map(Of java.text.AttributedCharacterIterator.Attribute, Object) = Nothing

					For counter As Integer = 0 To iterators.Length - 1
						Dim iterator_Renamed As AttributedCharacterIterator = iterators(counter)
						Dim start As Integer = iterator_Renamed.beginIndex
						Dim [end] As Integer = iterator_Renamed.endIndex
						Dim index As Integer = start

						Do While index < [end]
							iterator_Renamed.index = index

							Dim attrs As Map(Of java.text.AttributedCharacterIterator.Attribute, Object) = iterator_Renamed.attributes

							If mapsDiffer(last, attrs) Then attributestes(attrs, index - start + offset)
							last = attrs
							index = iterator_Renamed.runLimit
						Loop
						offset += ([end] - start)
					Next counter
				End If
			End If
		End Sub

		''' <summary>
		''' Constructs an AttributedString instance with the given text. </summary>
		''' <param name="text"> The text for this attributed string. </param>
		''' <exception cref="NullPointerException"> if <code>text</code> is null. </exception>
		Public Sub New(ByVal text As String)
			If text Is Nothing Then Throw New NullPointerException
			Me.text = text
		End Sub

		''' <summary>
		''' Constructs an AttributedString instance with the given text and attributes. </summary>
		''' <param name="text"> The text for this attributed string. </param>
		''' <param name="attributes"> The attributes that apply to the entire string. </param>
		''' <exception cref="NullPointerException"> if <code>text</code> or
		'''            <code>attributes</code> is null. </exception>
		''' <exception cref="IllegalArgumentException"> if the text has length 0
		''' and the attributes parameter is not an empty Map (attributes
		''' cannot be applied to a 0-length range). </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Sub New(Of T1 As java.text.AttributedCharacterIterator.Attribute, ?)(ByVal text As String, ByVal attributes As Map(Of T1))
			If text Is Nothing OrElse attributes Is Nothing Then Throw New NullPointerException
			Me.text = text

			If text.length() = 0 Then
				If attributes.empty Then Return
				Throw New IllegalArgumentException("Can't add attribute to 0-length text")
			End If

			Dim attributeCount As Integer = attributes.size()
			If attributeCount > 0 Then
				createRunAttributeDataVectors()
				Dim newRunAttributes As New Vector(Of java.text.AttributedCharacterIterator.Attribute)(attributeCount)
				Dim newRunAttributeValues As New Vector(Of Object)(attributeCount)
				runAttributes(0) = newRunAttributes
				runAttributeValues(0) = newRunAttributeValues

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim iterator_Renamed As [Iterator](Of ? As KeyValuePair(Of ? As java.text.AttributedCharacterIterator.Attribute, ?)) = attributes.entrySet().GetEnumerator()
				Do While iterator_Renamed.MoveNext()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim entry As KeyValuePair(Of ? As java.text.AttributedCharacterIterator.Attribute, ?) = iterator_Renamed.Current
					newRunAttributes.addElement(entry.Key)
					newRunAttributeValues.addElement(entry.Value)
				Loop
			End If
		End Sub

		''' <summary>
		''' Constructs an AttributedString instance with the given attributed
		''' text represented by AttributedCharacterIterator. </summary>
		''' <param name="text"> The text for this attributed string. </param>
		''' <exception cref="NullPointerException"> if <code>text</code> is null. </exception>
		Public Sub New(ByVal text As AttributedCharacterIterator)
			' If performance is critical, this constructor should be
			' implemented here rather than invoking the constructor for a
			' subrange. We can avoid some range checking in the loops.
			Me.New(text, text.beginIndex, text.endIndex, Nothing)
		End Sub

		''' <summary>
		''' Constructs an AttributedString instance with the subrange of
		''' the given attributed text represented by
		''' AttributedCharacterIterator. If the given range produces an
		''' empty text, all attributes will be discarded.  Note that any
		''' attributes wrapped by an Annotation object are discarded for a
		''' subrange of the original attribute range.
		''' </summary>
		''' <param name="text"> The text for this attributed string. </param>
		''' <param name="beginIndex"> Index of the first character of the range. </param>
		''' <param name="endIndex"> Index of the character following the last character
		''' of the range. </param>
		''' <exception cref="NullPointerException"> if <code>text</code> is null. </exception>
		''' <exception cref="IllegalArgumentException"> if the subrange given by
		''' beginIndex and endIndex is out of the text range. </exception>
		''' <seealso cref= java.text.Annotation </seealso>
		Public Sub New(ByVal text As AttributedCharacterIterator, ByVal beginIndex As Integer, ByVal endIndex As Integer)
			Me.New(text, beginIndex, endIndex, Nothing)
		End Sub

		''' <summary>
		''' Constructs an AttributedString instance with the subrange of
		''' the given attributed text represented by
		''' AttributedCharacterIterator.  Only attributes that match the
		''' given attributes will be incorporated into the instance. If the
		''' given range produces an empty text, all attributes will be
		''' discarded. Note that any attributes wrapped by an Annotation
		''' object are discarded for a subrange of the original attribute
		''' range.
		''' </summary>
		''' <param name="text"> The text for this attributed string. </param>
		''' <param name="beginIndex"> Index of the first character of the range. </param>
		''' <param name="endIndex"> Index of the character following the last character
		''' of the range. </param>
		''' <param name="attributes"> Specifies attributes to be extracted
		''' from the text. If null is specified, all available attributes will
		''' be used. </param>
		''' <exception cref="NullPointerException"> if <code>text</code> is null. </exception>
		''' <exception cref="IllegalArgumentException"> if the subrange given by
		''' beginIndex and endIndex is out of the text range. </exception>
		''' <seealso cref= java.text.Annotation </seealso>
		Public Sub New(ByVal text As AttributedCharacterIterator, ByVal beginIndex As Integer, ByVal endIndex As Integer, ByVal attributes As java.text.AttributedCharacterIterator.Attribute())
			If text Is Nothing Then Throw New NullPointerException

			' Validate the given subrange
			Dim textBeginIndex As Integer = text.beginIndex
			Dim textEndIndex As Integer = text.endIndex
			If beginIndex < textBeginIndex OrElse endIndex > textEndIndex OrElse beginIndex > endIndex Then Throw New IllegalArgumentException("Invalid substring range")

			' Copy the given string
			Dim textBuffer As New StringBuffer
			text.index = beginIndex
			Dim c As Char = text.current()
			Do While text.index < endIndex
				textBuffer.append(c)
				c = text.next()
			Loop
			Me.text = textBuffer.ToString()

			If beginIndex = endIndex Then Return

			' Select attribute keys to be taken care of
			Dim keys As New HashSet(Of java.text.AttributedCharacterIterator.Attribute)
			If attributes Is Nothing Then
				keys.addAll(text.allAttributeKeys)
			Else
				For i As Integer = 0 To attributes.Length - 1
					keys.add(attributes(i))
				Next i
				keys.retainAll(text.allAttributeKeys)
			End If
			If keys.empty Then Return

			' Get and set attribute runs for each attribute name. Need to
			' scan from the top of the text so that we can discard any
			' Annotation that is no longer applied to a subset text segment.
			Dim itr As [Iterator](Of java.text.AttributedCharacterIterator.Attribute) = keys.GetEnumerator()
			Do While itr.MoveNext()
				Dim attributeKey As java.text.AttributedCharacterIterator.Attribute = itr.Current
				text.index = textBeginIndex
				Do While text.index < endIndex
					Dim start As Integer = text.getRunStart(attributeKey)
					Dim limit As Integer = text.getRunLimit(attributeKey)
					Dim value As Object = text.getAttribute(attributeKey)

					If value IsNot Nothing Then
						If TypeOf value Is Annotation Then
							If start >= beginIndex AndAlso limit <= endIndex Then
								addAttribute(attributeKey, value, start - beginIndex, limit - beginIndex)
							Else
								If limit > endIndex Then Exit Do
							End If
						Else
							' if the run is beyond the given (subset) range, we
							' don't need to process further.
							If start >= endIndex Then Exit Do
							If limit > beginIndex Then
								' attribute is applied to any subrange
								If start < beginIndex Then start = beginIndex
								If limit > endIndex Then limit = endIndex
								If start <> limit Then addAttribute(attributeKey, value, start - beginIndex, limit - beginIndex)
							End If
						End If
					End If
					text.index = limit
				Loop
			Loop
		End Sub

		''' <summary>
		''' Adds an attribute to the entire string. </summary>
		''' <param name="attribute"> the attribute key </param>
		''' <param name="value"> the value of the attribute; may be null </param>
		''' <exception cref="NullPointerException"> if <code>attribute</code> is null. </exception>
		''' <exception cref="IllegalArgumentException"> if the AttributedString has length 0
		''' (attributes cannot be applied to a 0-length range). </exception>
		Public Overridable Sub addAttribute(ByVal attribute As java.text.AttributedCharacterIterator.Attribute, ByVal value As Object)

			If attribute Is Nothing Then Throw New NullPointerException

			Dim len As Integer = length()
			If len = 0 Then Throw New IllegalArgumentException("Can't add attribute to 0-length text")

			addAttributeImpl(attribute, value, 0, len)
		End Sub

		''' <summary>
		''' Adds an attribute to a subrange of the string. </summary>
		''' <param name="attribute"> the attribute key </param>
		''' <param name="value"> The value of the attribute. May be null. </param>
		''' <param name="beginIndex"> Index of the first character of the range. </param>
		''' <param name="endIndex"> Index of the character following the last character of the range. </param>
		''' <exception cref="NullPointerException"> if <code>attribute</code> is null. </exception>
		''' <exception cref="IllegalArgumentException"> if beginIndex is less then 0, endIndex is
		''' greater than the length of the string, or beginIndex and endIndex together don't
		''' define a non-empty subrange of the string. </exception>
		Public Overridable Sub addAttribute(ByVal attribute As java.text.AttributedCharacterIterator.Attribute, ByVal value As Object, ByVal beginIndex As Integer, ByVal endIndex As Integer)

			If attribute Is Nothing Then Throw New NullPointerException

			If beginIndex < 0 OrElse endIndex > length() OrElse beginIndex >= endIndex Then Throw New IllegalArgumentException("Invalid substring range")

			addAttributeImpl(attribute, value, beginIndex, endIndex)
		End Sub

		''' <summary>
		''' Adds a set of attributes to a subrange of the string. </summary>
		''' <param name="attributes"> The attributes to be added to the string. </param>
		''' <param name="beginIndex"> Index of the first character of the range. </param>
		''' <param name="endIndex"> Index of the character following the last
		''' character of the range. </param>
		''' <exception cref="NullPointerException"> if <code>attributes</code> is null. </exception>
		''' <exception cref="IllegalArgumentException"> if beginIndex is less then
		''' 0, endIndex is greater than the length of the string, or
		''' beginIndex and endIndex together don't define a non-empty
		''' subrange of the string and the attributes parameter is not an
		''' empty Map. </exception>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Sub addAttributes(Of T1 As java.text.AttributedCharacterIterator.Attribute, ?)(ByVal attributes As Map(Of T1), ByVal beginIndex As Integer, ByVal endIndex As Integer)
			If attributes Is Nothing Then Throw New NullPointerException

			If beginIndex < 0 OrElse endIndex > length() OrElse beginIndex > endIndex Then Throw New IllegalArgumentException("Invalid substring range")
			If beginIndex = endIndex Then
				If attributes.empty Then Return
				Throw New IllegalArgumentException("Can't add attribute to 0-length text")
			End If

			' make sure we have run attribute data vectors
			If runCount = 0 Then createRunAttributeDataVectors()

			' break up runs if necessary
			Dim beginRunIndex As Integer = ensureRunBreak(beginIndex)
			Dim endRunIndex As Integer = ensureRunBreak(endIndex)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim iterator_Renamed As [Iterator](Of ? As KeyValuePair(Of ? As java.text.AttributedCharacterIterator.Attribute, ?)) = attributes.entrySet().GetEnumerator()
			Do While iterator_Renamed.MoveNext()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim entry As KeyValuePair(Of ? As java.text.AttributedCharacterIterator.Attribute, ?) = iterator_Renamed.Current
				addAttributeRunData(entry.Key, entry.Value, beginRunIndex, endRunIndex)
			Loop
		End Sub

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Sub addAttributeImpl(ByVal attribute As java.text.AttributedCharacterIterator.Attribute, ByVal value As Object, ByVal beginIndex As Integer, ByVal endIndex As Integer)

			' make sure we have run attribute data vectors
			If runCount = 0 Then createRunAttributeDataVectors()

			' break up runs if necessary
			Dim beginRunIndex As Integer = ensureRunBreak(beginIndex)
			Dim endRunIndex As Integer = ensureRunBreak(endIndex)

			addAttributeRunData(attribute, value, beginRunIndex, endRunIndex)
		End Sub

		Private Sub createRunAttributeDataVectors()
			' use temporary variables so things remain consistent in case of an exception
			Dim newRunStarts As Integer() = New Integer(ARRAY_SIZE_INCREMENT - 1){}

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim newRunAttributes As Vector(Of java.text.AttributedCharacterIterator.Attribute)() = CType(New Vector(Of ?)(ARRAY_SIZE_INCREMENT - 1){}, Vector(Of java.text.AttributedCharacterIterator.Attribute)())

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim newRunAttributeValues As Vector(Of Object)() = CType(New Vector(Of ?)(ARRAY_SIZE_INCREMENT - 1){}, Vector(Of Object)())

			runStarts = newRunStarts
			runAttributes = newRunAttributes
			runAttributeValues = newRunAttributeValues
			runArraySize = ARRAY_SIZE_INCREMENT
			runCount = 1 ' assume initial run starting at index 0
		End Sub

		' ensure there's a run break at offset, return the index of the run
		Private Function ensureRunBreak(ByVal offset As Integer) As Integer
			Return ensureRunBreak(offset, True)
		End Function

		''' <summary>
		''' Ensures there is a run break at offset, returning the index of
		''' the run. If this results in splitting a run, two things can happen:
		''' <ul>
		''' <li>If copyAttrs is true, the attributes from the existing run
		'''     will be placed in both of the newly created runs.
		''' <li>If copyAttrs is false, the attributes from the existing run
		''' will NOT be copied to the run to the right (>= offset) of the break,
		''' but will exist on the run to the left (< offset).
		''' </ul>
		''' </summary>
		Private Function ensureRunBreak(ByVal offset As Integer, ByVal copyAttrs As Boolean) As Integer
			If offset = length() Then Return runCount

			' search for the run index where this offset should be
			Dim runIndex As Integer = 0
			Do While runIndex < runCount AndAlso runStarts(runIndex) < offset
				runIndex += 1
			Loop

			' if the offset is at a run start already, we're done
			If runIndex < runCount AndAlso runStarts(runIndex) = offset Then Return runIndex

			' we'll have to break up a run
			' first, make sure we have enough space in our arrays
			If runCount = runArraySize Then
				Dim newArraySize As Integer = runArraySize + ARRAY_SIZE_INCREMENT
				Dim newRunStarts As Integer() = New Integer(newArraySize - 1){}

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim newRunAttributes As Vector(Of java.text.AttributedCharacterIterator.Attribute)() = CType(New Vector(Of ?)(newArraySize - 1){}, Vector(Of java.text.AttributedCharacterIterator.Attribute)())

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim newRunAttributeValues As Vector(Of Object)() = CType(New Vector(Of ?)(newArraySize - 1){}, Vector(Of Object)())

				For i As Integer = 0 To runArraySize - 1
					newRunStarts(i) = runStarts(i)
					newRunAttributes(i) = runAttributes(i)
					newRunAttributeValues(i) = runAttributeValues(i)
				Next i
				runStarts = newRunStarts
				runAttributes = newRunAttributes
				runAttributeValues = newRunAttributeValues
				runArraySize = newArraySize
			End If

			' make copies of the attribute information of the old run that the new one used to be part of
			' use temporary variables so things remain consistent in case of an exception
			Dim newRunAttributes As Vector(Of java.text.AttributedCharacterIterator.Attribute) = Nothing
			Dim newRunAttributeValues As Vector(Of Object) = Nothing

			If copyAttrs Then
				Dim oldRunAttributes As Vector(Of java.text.AttributedCharacterIterator.Attribute) = runAttributes(runIndex - 1)
				Dim oldRunAttributeValues As Vector(Of Object) = runAttributeValues(runIndex - 1)
				If oldRunAttributes IsNot Nothing Then newRunAttributes = New Vector(Of )(oldRunAttributes)
				If oldRunAttributeValues IsNot Nothing Then newRunAttributeValues = New Vector(Of )(oldRunAttributeValues)
			End If

			' now actually break up the run
			runCount += 1
			For i As Integer = runCount - 1 To runIndex + 1 Step -1
				runStarts(i) = runStarts(i - 1)
				runAttributes(i) = runAttributes(i - 1)
				runAttributeValues(i) = runAttributeValues(i - 1)
			Next i
			runStarts(runIndex) = offset
			runAttributes(runIndex) = newRunAttributes
			runAttributeValues(runIndex) = newRunAttributeValues

			Return runIndex
		End Function

		' add the attribute attribute/value to all runs where beginRunIndex <= runIndex < endRunIndex
		Private Sub addAttributeRunData(ByVal attribute As java.text.AttributedCharacterIterator.Attribute, ByVal value As Object, ByVal beginRunIndex As Integer, ByVal endRunIndex As Integer)

			For i As Integer = beginRunIndex To endRunIndex - 1
				Dim keyValueIndex As Integer = -1 ' index of key and value in our vectors; assume we don't have an entry yet
				If runAttributes(i) Is Nothing Then
					Dim newRunAttributes As New Vector(Of java.text.AttributedCharacterIterator.Attribute)
					Dim newRunAttributeValues As New Vector(Of Object)
					runAttributes(i) = newRunAttributes
					runAttributeValues(i) = newRunAttributeValues
				Else
					' check whether we have an entry already
					keyValueIndex = runAttributes(i).IndexOf(attribute)
				End If

				If keyValueIndex = -1 Then
					' create new entry
					Dim oldSize As Integer = runAttributes(i).size()
					runAttributes(i).addElement(attribute)
					Try
						runAttributeValues(i).addElement(value)
					Catch e As Exception
						runAttributes(i).size = oldSize
						runAttributeValues(i).size = oldSize
					End Try
				Else
					' update existing entry
					runAttributeValues(i).set(keyValueIndex, value)
				End If
			Next i
		End Sub

		''' <summary>
		''' Creates an AttributedCharacterIterator instance that provides access to the entire contents of
		''' this string.
		''' </summary>
		''' <returns> An iterator providing access to the text and its attributes. </returns>
		Public Overridable Property [iterator] As AttributedCharacterIterator
			Get
				Return getIterator(Nothing, 0, length())
			End Get
		End Property

		''' <summary>
		''' Creates an AttributedCharacterIterator instance that provides access to
		''' selected contents of this string.
		''' Information about attributes not listed in attributes that the
		''' implementor may have need not be made accessible through the iterator.
		''' If the list is null, all available attribute information should be made
		''' accessible.
		''' </summary>
		''' <param name="attributes"> a list of attributes that the client is interested in </param>
		''' <returns> an iterator providing access to the entire text and its selected attributes </returns>
		Public Overridable Function getIterator(ByVal attributes As java.text.AttributedCharacterIterator.Attribute()) As AttributedCharacterIterator
			Return getIterator(attributes, 0, length())
		End Function

		''' <summary>
		''' Creates an AttributedCharacterIterator instance that provides access to
		''' selected contents of this string.
		''' Information about attributes not listed in attributes that the
		''' implementor may have need not be made accessible through the iterator.
		''' If the list is null, all available attribute information should be made
		''' accessible.
		''' </summary>
		''' <param name="attributes"> a list of attributes that the client is interested in </param>
		''' <param name="beginIndex"> the index of the first character </param>
		''' <param name="endIndex"> the index of the character following the last character </param>
		''' <returns> an iterator providing access to the text and its attributes </returns>
		''' <exception cref="IllegalArgumentException"> if beginIndex is less then 0,
		''' endIndex is greater than the length of the string, or beginIndex is
		''' greater than endIndex. </exception>
		Public Overridable Function getIterator(ByVal attributes As java.text.AttributedCharacterIterator.Attribute(), ByVal beginIndex As Integer, ByVal endIndex As Integer) As AttributedCharacterIterator
			Return New AttributedStringIterator(Me, attributes, beginIndex, endIndex)
		End Function

		' all (with the exception of length) reading operations are private,
		' since AttributedString instances are accessed through iterators.

		' length is package private so that CharacterIteratorFieldDelegate can
		' access it without creating an AttributedCharacterIterator.
		Friend Overridable Function length() As Integer
			Return text.length()
		End Function

		Private Function charAt(ByVal index As Integer) As Char
			Return text.Chars(index)
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Function getAttribute(ByVal attribute As java.text.AttributedCharacterIterator.Attribute, ByVal runIndex As Integer) As Object
			Dim currentRunAttributes As Vector(Of java.text.AttributedCharacterIterator.Attribute) = runAttributes(runIndex)
			Dim currentRunAttributeValues As Vector(Of Object) = runAttributeValues(runIndex)
			If currentRunAttributes Is Nothing Then Return Nothing
			Dim attributeIndex As Integer = currentRunAttributes.IndexOf(attribute)
			If attributeIndex <> -1 Then
				Return currentRunAttributeValues.elementAt(attributeIndex)
			Else
				Return Nothing
			End If
		End Function

		' gets an attribute value, but returns an annotation only if it's range does not extend outside the range beginIndex..endIndex
		Private Function getAttributeCheckRange(ByVal attribute As java.text.AttributedCharacterIterator.Attribute, ByVal runIndex As Integer, ByVal beginIndex As Integer, ByVal endIndex As Integer) As Object
			Dim value As Object = getAttribute(attribute, runIndex)
			If TypeOf value Is Annotation Then
				' need to check whether the annotation's range extends outside the iterator's range
				If beginIndex > 0 Then
					Dim currIndex As Integer = runIndex
					Dim runStart As Integer = runStarts(currIndex)
					Do While runStart >= beginIndex AndAlso valuesMatch(value, getAttribute(attribute, currIndex - 1))
						currIndex -= 1
						runStart = runStarts(currIndex)
					Loop
					If runStart < beginIndex Then Return Nothing
				End If
				Dim textLength As Integer = length()
				If endIndex < textLength Then
					Dim currIndex As Integer = runIndex
					Dim runLimit As Integer = If(currIndex < runCount - 1, runStarts(currIndex + 1), textLength)
					Do While runLimit <= endIndex AndAlso valuesMatch(value, getAttribute(attribute, currIndex + 1))
						currIndex += 1
						runLimit = If(currIndex < runCount - 1, runStarts(currIndex + 1), textLength)
					Loop
					If runLimit > endIndex Then Return Nothing
				End If
				' annotation's range is subrange of iterator's range,
				' so we can return the value
			End If
			Return value
		End Function

		' returns whether all specified attributes have equal values in the runs with the given indices
		Private Function attributeValuesMatch(Of T1 As java.text.AttributedCharacterIterator.Attribute)(ByVal attributes As [Set](Of T1), ByVal runIndex1 As Integer, ByVal runIndex2 As Integer) As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim iterator_Renamed As [Iterator](Of ? As java.text.AttributedCharacterIterator.Attribute) = attributes.GetEnumerator()
			Do While iterator_Renamed.MoveNext()
				Dim key As java.text.AttributedCharacterIterator.Attribute = iterator_Renamed.Current
			   If Not valuesMatch(getAttribute(key, runIndex1), getAttribute(key, runIndex2)) Then Return False
			Loop
			Return True
		End Function

		' returns whether the two objects are either both null or equal
		Private Shared Function valuesMatch(ByVal value1 As Object, ByVal value2 As Object) As Boolean
			If value1 Is Nothing Then
				Return value2 Is Nothing
			Else
				Return value1.Equals(value2)
			End If
		End Function

		''' <summary>
		''' Appends the contents of the CharacterIterator iterator into the
		''' StringBuffer buf.
		''' </summary>
		Private Sub appendContents(ByVal buf As StringBuffer, ByVal [iterator] As CharacterIterator)
			Dim index As Integer = [iterator].beginIndex
			Dim [end] As Integer = [iterator].endIndex

			Do While index < [end]
				[iterator].index = index
				index += 1
				buf.append([iterator].current())
			Loop
		End Sub

		''' <summary>
		''' Sets the attributes for the range from offset to the next run break
		''' (typically the end of the text) to the ones specified in attrs.
		''' This is only meant to be called from the constructor!
		''' </summary>
		Private Sub setAttributes(ByVal attrs As Map(Of java.text.AttributedCharacterIterator.Attribute, Object), ByVal offset As Integer)
			If runCount = 0 Then createRunAttributeDataVectors()

			Dim index As Integer = ensureRunBreak(offset, False)
			Dim size As Integer

			size = attrs.size()
			If attrs IsNot Nothing AndAlso size > 0 Then
				Dim runAttrs As New Vector(Of java.text.AttributedCharacterIterator.Attribute)(size)
				Dim runValues As New Vector(Of Object)(size)
				Dim iterator_Renamed As [Iterator](Of KeyValuePair(Of java.text.AttributedCharacterIterator.Attribute, Object)) = attrs.entrySet().GetEnumerator()

				Do While iterator_Renamed.MoveNext()
					Dim entry As KeyValuePair(Of java.text.AttributedCharacterIterator.Attribute, Object) = iterator_Renamed.Current

					runAttrs.add(entry.Key)
					runValues.add(entry.Value)
				Loop
				runAttributes(index) = runAttrs
				runAttributeValues(index) = runValues
			End If
		End Sub

		''' <summary>
		''' Returns true if the attributes specified in last and attrs differ.
		''' </summary>
		Private Shared Function mapsDiffer(Of K, V)(ByVal last As Map(Of K, V), ByVal attrs As Map(Of K, V)) As Boolean
			If last Is Nothing Then Return (attrs IsNot Nothing AndAlso attrs.size() > 0)
			Return ((Not last.Equals(attrs)))
		End Function


		' the iterator class associated with this string class

		Private NotInheritable Class AttributedStringIterator
			Implements AttributedCharacterIterator

			Private ReadOnly outerInstance As AttributedString


			' note on synchronization:
			' we don't synchronize on the iterator, assuming that an iterator is only used in one thread.
			' we do synchronize access to the AttributedString however, since it's more likely to be shared between threads.

			' start and end index for our iteration
			Private beginIndex As Integer
			Private endIndex As Integer

			' attributes that our client is interested in
			Private relevantAttributes As java.text.AttributedCharacterIterator.Attribute()

			' the current index for our iteration
			' invariant: beginIndex <= currentIndex <= endIndex
			Private currentIndex As Integer

			' information about the run that includes currentIndex
			Private currentRunIndex As Integer
			Private currentRunStart As Integer
			Private currentRunLimit As Integer

			' constructor
			Friend Sub New(ByVal outerInstance As AttributedString, ByVal attributes As java.text.AttributedCharacterIterator.Attribute(), ByVal beginIndex As Integer, ByVal endIndex As Integer)
					Me.outerInstance = outerInstance

				If beginIndex < 0 OrElse beginIndex > endIndex OrElse endIndex > outerInstance.length() Then Throw New IllegalArgumentException("Invalid substring range")

				Me.beginIndex = beginIndex
				Me.endIndex = endIndex
				Me.currentIndex = beginIndex
				updateRunInfo()
				If attributes IsNot Nothing Then relevantAttributes = attributes.clone()
			End Sub

			' Object methods. See documentation in that class.

			Public Overrides Function Equals(ByVal obj As Object) As Boolean
				If Me Is obj Then Return True
				If Not(TypeOf obj Is AttributedStringIterator) Then Return False

				Dim that As AttributedStringIterator = CType(obj, AttributedStringIterator)

				If AttributedString.this <> that.string Then Return False
				If currentIndex <> that.currentIndex OrElse beginIndex <> that.beginIndex OrElse endIndex <> that.endIndex Then Return False
				Return True
			End Function

			Public Overrides Function GetHashCode() As Integer
				Return outerInstance.text.GetHashCode() Xor currentIndex Xor beginIndex Xor endIndex
			End Function

			Public Function clone() As Object Implements CharacterIterator.clone
				Try
					Dim other As AttributedStringIterator = CType(MyBase.clone(), AttributedStringIterator)
					Return other
				Catch e As CloneNotSupportedException
					Throw New InternalError(e)
				End Try
			End Function

			' CharacterIterator methods. See documentation in that interface.

			Public Function first() As Char Implements CharacterIterator.first
				Return internalSetIndex(beginIndex)
			End Function

			Public Function last() As Char Implements CharacterIterator.last
				If endIndex = beginIndex Then
					Return internalSetIndex(endIndex)
				Else
					Return internalSetIndex(endIndex - 1)
				End If
			End Function

			Public Function current() As Char Implements CharacterIterator.current
				If currentIndex = endIndex Then
					Return DONE
				Else
					Return outerInstance.Chars(currentIndex)
				End If
			End Function

			Public Function [next]() As Char Implements CharacterIterator.next
				If currentIndex < endIndex Then
					Return internalSetIndex(currentIndex + 1)
				Else
					Return DONE
				End If
			End Function

			Public Function previous() As Char Implements CharacterIterator.previous
				If currentIndex > beginIndex Then
					Return internalSetIndex(currentIndex - 1)
				Else
					Return DONE
				End If
			End Function

			Public Function setIndex(ByVal position As Integer) As Char Implements CharacterIterator.setIndex
				If position < beginIndex OrElse position > endIndex Then Throw New IllegalArgumentException("Invalid index")
				Return internalSetIndex(position)
			End Function

			Public Property beginIndex As Integer Implements CharacterIterator.getBeginIndex
				Get
					Return beginIndex
				End Get
			End Property

			Public Property endIndex As Integer Implements CharacterIterator.getEndIndex
				Get
					Return endIndex
				End Get
			End Property

			Public Property index As Integer Implements CharacterIterator.getIndex
				Get
					Return currentIndex
				End Get
			End Property

			' AttributedCharacterIterator methods. See documentation in that interface.

			Public Property runStart As Integer Implements AttributedCharacterIterator.getRunStart
				Get
					Return currentRunStart
				End Get
			End Property

			Public Function getRunStart(ByVal attribute As java.text.AttributedCharacterIterator.Attribute) As Integer
				If currentRunStart = beginIndex OrElse currentRunIndex = -1 Then
					Return currentRunStart
				Else
					Dim value As Object = getAttribute(attribute)
					Dim runStart_Renamed As Integer = currentRunStart
					Dim runIndex As Integer = currentRunIndex
					Do While runStart_Renamed > beginIndex AndAlso valuesMatch(value, outerInstance.getAttribute(attribute, runIndex - 1))
						runIndex -= 1
						runStart_Renamed = outerInstance.runStarts(runIndex)
					Loop
					If runStart_Renamed < beginIndex Then runStart_Renamed = beginIndex
					Return runStart_Renamed
				End If
			End Function

			Public Function getRunStart(Of T1 As java.text.AttributedCharacterIterator.Attribute)(ByVal attributes As [Set](Of T1)) As Integer
				If currentRunStart = beginIndex OrElse currentRunIndex = -1 Then
					Return currentRunStart
				Else
					Dim runStart_Renamed As Integer = currentRunStart
					Dim runIndex As Integer = currentRunIndex
					Do While runStart_Renamed > beginIndex AndAlso outerInstance.attributeValuesMatch(attributes, currentRunIndex, runIndex - 1)
						runIndex -= 1
						runStart_Renamed = outerInstance.runStarts(runIndex)
					Loop
					If runStart_Renamed < beginIndex Then runStart_Renamed = beginIndex
					Return runStart_Renamed
				End If
			End Function

			Public Property runLimit As Integer Implements AttributedCharacterIterator.getRunLimit
				Get
					Return currentRunLimit
				End Get
			End Property

			Public Function getRunLimit(ByVal attribute As java.text.AttributedCharacterIterator.Attribute) As Integer
				If currentRunLimit = endIndex OrElse currentRunIndex = -1 Then
					Return currentRunLimit
				Else
					Dim value As Object = getAttribute(attribute)
					Dim runLimit_Renamed As Integer = currentRunLimit
					Dim runIndex As Integer = currentRunIndex
					Do While runLimit_Renamed < endIndex AndAlso valuesMatch(value, outerInstance.getAttribute(attribute, runIndex + 1))
						runIndex += 1
						runLimit_Renamed = If(runIndex < outerInstance.runCount - 1, outerInstance.runStarts(runIndex + 1), endIndex)
					Loop
					If runLimit_Renamed > endIndex Then runLimit_Renamed = endIndex
					Return runLimit_Renamed
				End If
			End Function

			Public Function getRunLimit(Of T1 As java.text.AttributedCharacterIterator.Attribute)(ByVal attributes As [Set](Of T1)) As Integer
				If currentRunLimit = endIndex OrElse currentRunIndex = -1 Then
					Return currentRunLimit
				Else
					Dim runLimit_Renamed As Integer = currentRunLimit
					Dim runIndex As Integer = currentRunIndex
					Do While runLimit_Renamed < endIndex AndAlso outerInstance.attributeValuesMatch(attributes, currentRunIndex, runIndex + 1)
						runIndex += 1
						runLimit_Renamed = If(runIndex < outerInstance.runCount - 1, outerInstance.runStarts(runIndex + 1), endIndex)
					Loop
					If runLimit_Renamed > endIndex Then runLimit_Renamed = endIndex
					Return runLimit_Renamed
				End If
			End Function

			Public Property attributes As Map(Of java.text.AttributedCharacterIterator.Attribute, Object)
				Get
					If outerInstance.runAttributes Is Nothing OrElse currentRunIndex = -1 OrElse outerInstance.runAttributes(currentRunIndex) Is Nothing Then Return New Dictionary(Of )
					Return New AttributeMap(currentRunIndex, beginIndex, endIndex)
				End Get
			End Property

			Public Property allAttributeKeys As [Set](Of java.text.AttributedCharacterIterator.Attribute)
				Get
					' ??? This should screen out attribute keys that aren't relevant to the client
					If outerInstance.runAttributes Is Nothing Then Return New HashSet(Of )
					SyncLock AttributedString.this
						' ??? should try to create this only once, then update if necessary,
						' and give callers read-only view
						Dim keys As [Set](Of java.text.AttributedCharacterIterator.Attribute) = New HashSet(Of java.text.AttributedCharacterIterator.Attribute)
						Dim i As Integer = 0
						Do While i < outerInstance.runCount
							If outerInstance.runStarts(i) < endIndex AndAlso (i = outerInstance.runCount - 1 OrElse outerInstance.runStarts(i + 1) > beginIndex) Then
								Dim currentRunAttributes As Vector(Of java.text.AttributedCharacterIterator.Attribute) = outerInstance.runAttributes(i)
								If currentRunAttributes IsNot Nothing Then
									Dim j As Integer = currentRunAttributes.size()
									Dim tempVar As Boolean = j > 0
									j -= 1
									Do While tempVar
										keys.add(currentRunAttributes.get(j))
										tempVar = j > 0
										j -= 1
									Loop
								End If
							End If
							i += 1
						Loop
						Return keys
					End SyncLock
				End Get
			End Property

			Public Function getAttribute(ByVal attribute As java.text.AttributedCharacterIterator.Attribute) As Object
				Dim runIndex As Integer = currentRunIndex
				If runIndex < 0 Then Return Nothing
				Return outerInstance.getAttributeCheckRange(attribute, runIndex, beginIndex, endIndex)
			End Function

			' internally used methods

			Private Property [string] As AttributedString
				Get
					Return AttributedString.this
				End Get
			End Property

			' set the current index, update information about the current run if necessary,
			' return the character at the current index
			Private Function internalSetIndex(ByVal position As Integer) As Char
				currentIndex = position
				If position < currentRunStart OrElse position >= currentRunLimit Then updateRunInfo()
				If currentIndex = endIndex Then
					Return DONE
				Else
					Return outerInstance.Chars(position)
				End If
			End Function

			' update the information about the current run
			Private Sub updateRunInfo()
				If currentIndex = endIndex Then
						currentRunLimit = endIndex
						currentRunStart = currentRunLimit
					currentRunIndex = -1
				Else
					SyncLock AttributedString.this
						Dim runIndex As Integer = -1
						Do While runIndex < outerInstance.runCount - 1 AndAlso outerInstance.runStarts(runIndex + 1) <= currentIndex
							runIndex += 1
						Loop
						currentRunIndex = runIndex
						If runIndex >= 0 Then
							currentRunStart = outerInstance.runStarts(runIndex)
							If currentRunStart < beginIndex Then currentRunStart = beginIndex
						Else
							currentRunStart = beginIndex
						End If
						If runIndex < outerInstance.runCount - 1 Then
							currentRunLimit = outerInstance.runStarts(runIndex + 1)
							If currentRunLimit > endIndex Then currentRunLimit = endIndex
						Else
							currentRunLimit = endIndex
						End If
					End SyncLock
				End If
			End Sub

		End Class

		' the map class associated with this string class, giving access to the attributes of one run

		Private NotInheritable Class AttributeMap
			Inherits AbstractMap(Of java.text.AttributedCharacterIterator.Attribute, Object)

			Private ReadOnly outerInstance As AttributedString


			Friend runIndex As Integer
			Friend beginIndex As Integer
			Friend endIndex As Integer

			Friend Sub New(ByVal outerInstance As AttributedString, ByVal runIndex As Integer, ByVal beginIndex As Integer, ByVal endIndex As Integer)
					Me.outerInstance = outerInstance
				Me.runIndex = runIndex
				Me.beginIndex = beginIndex
				Me.endIndex = endIndex
			End Sub

			Public Function entrySet() As [Set](Of KeyValuePair(Of java.text.AttributedCharacterIterator.Attribute, Object))
				Dim [set] As New HashSet(Of KeyValuePair(Of java.text.AttributedCharacterIterator.Attribute, Object))
				SyncLock AttributedString.this
					Dim size As Integer = outerInstance.runAttributes(runIndex).size()
					For i As Integer = 0 To size - 1
						Dim key As java.text.AttributedCharacterIterator.Attribute = outerInstance.runAttributes(runIndex).get(i)
						Dim value As Object = outerInstance.runAttributeValues(runIndex).get(i)
						If TypeOf value Is Annotation Then
							value = outerInstance.getAttributeCheckRange(key, runIndex, beginIndex, endIndex)
							If value Is Nothing Then Continue For
						End If

						Dim entry As KeyValuePair(Of java.text.AttributedCharacterIterator.Attribute, Object) = New AttributeEntry(key, value)
						[set].add(entry)
					Next i
				End SyncLock
				Return [set]
			End Function

			Public Function [get](ByVal key As Object) As Object
				Return outerInstance.getAttributeCheckRange(CType(key, java.text.AttributedCharacterIterator.Attribute), runIndex, beginIndex, endIndex)
			End Function
		End Class
	End Class

	Friend Class AttributeEntry
		Implements KeyValuePair(Of java.text.AttributedCharacterIterator.Attribute, Object)

		Private key As java.text.AttributedCharacterIterator.Attribute
		Private value As Object

		Friend Sub New(ByVal key As java.text.AttributedCharacterIterator.Attribute, ByVal value As Object)
			Me.key = key
			Me.value = value
		End Sub

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Not(TypeOf o Is AttributeEntry) Then Return False
			Dim other As AttributeEntry = CType(o, AttributeEntry)
			Return other.key.Equals(key) AndAlso (If(value Is Nothing, other.value Is Nothing, other.value.Equals(value)))
		End Function

		Public Overridable Property key As java.text.AttributedCharacterIterator.Attribute
			Get
				Return key
			End Get
		End Property

		Public Overridable Property value As Object
			Get
				Return value
			End Get
		End Property

		Public Overridable Function setValue(ByVal newValue As Object) As Object
			Throw New UnsupportedOperationException
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return key.GetHashCode() Xor (If(value Is Nothing, 0, value.GetHashCode()))
		End Function

		Public Overrides Function ToString() As String
			Return key.ToString() & "=" & value.ToString()
		End Function
	End Class

End Namespace