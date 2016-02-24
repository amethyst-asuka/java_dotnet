Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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

'
' * (C) Copyright Taligent, Inc. 1996, 1997 - All Rights Reserved
' * (C) Copyright IBM Corp. 1996-1998 - All Rights Reserved
' *
' *   The original version of this source code and documentation is copyrighted
' * and owned by Taligent, Inc., a wholly-owned subsidiary of IBM. These
' * materials are provided under terms of a License Agreement between Taligent
' * and Sun. This technology is protected by multiple US and International
' * patents. This notice and attribution to Taligent may not be removed.
' *   Taligent is a registered trademark of Taligent, Inc.
' *
' 

Namespace java.text


	''' <summary>
	''' The <code>CollationElementIterator</code> class is used as an iterator
	''' to walk through each character of an international string. Use the iterator
	''' to return the ordering priority of the positioned character. The ordering
	''' priority of a character, which we refer to as a key, defines how a character
	''' is collated in the given collation object.
	''' 
	''' <p>
	''' For example, consider the following in Spanish:
	''' <blockquote>
	''' <pre>
	''' "ca" &rarr; the first key is key('c') and second key is key('a').
	''' "cha" &rarr; the first key is key('ch') and second key is key('a').
	''' </pre>
	''' </blockquote>
	''' And in German,
	''' <blockquote>
	''' <pre>
	''' "\u00e4b" &rarr; the first key is key('a'), the second key is key('e'), and
	''' the third key is key('b').
	''' </pre>
	''' </blockquote>
	''' The key of a character is an integer composed of primary order(short),
	''' secondary order(byte), and tertiary order(byte). Java strictly defines
	''' the size and signedness of its primitive data types. Therefore, the static
	''' functions <code>primaryOrder</code>, <code>secondaryOrder</code>, and
	''' <code>tertiaryOrder</code> return <code>int</code>, <code>short</code>,
	''' and <code>short</code> respectively to ensure the correctness of the key
	''' value.
	''' 
	''' <p>
	''' Example of the iterator usage,
	''' <blockquote>
	''' <pre>
	''' 
	'''  String testString = "This is a test";
	'''  Collator col = Collator.getInstance();
	'''  if (col instanceof RuleBasedCollator) {
	'''      RuleBasedCollator ruleBasedCollator = (RuleBasedCollator)col;
	'''      CollationElementIterator collationElementIterator = ruleBasedCollator.getCollationElementIterator(testString);
	'''      int primaryOrder = CollationElementIterator.primaryOrder(collationElementIterator.next());
	'''          :
	'''  }
	''' </pre>
	''' </blockquote>
	''' 
	''' <p>
	''' <code>CollationElementIterator.next</code> returns the collation order
	''' of the next character. A collation order consists of primary order,
	''' secondary order and tertiary order. The data type of the collation
	''' order is <strong>int</strong>. The first 16 bits of a collation order
	''' is its primary order; the next 8 bits is the secondary order and the
	''' last 8 bits is the tertiary order.
	''' 
	''' <p><b>Note:</b> <code>CollationElementIterator</code> is a part of
	''' <code>RuleBasedCollator</code> implementation. It is only usable
	''' with <code>RuleBasedCollator</code> instances.
	''' </summary>
	''' <seealso cref=                Collator </seealso>
	''' <seealso cref=                RuleBasedCollator
	''' @author             Helena Shih, Laura Werner, Richard Gillam </seealso>
	Public NotInheritable Class CollationElementIterator
		''' <summary>
		''' Null order which indicates the end of string is reached by the
		''' cursor.
		''' </summary>
		Public Const NULLORDER As Integer = &HffffffffL

		''' <summary>
		''' CollationElementIterator constructor.  This takes the source string and
		''' the collation object.  The cursor will walk thru the source string based
		''' on the predefined collation rules.  If the source string is empty,
		''' NULLORDER will be returned on the calls to next(). </summary>
		''' <param name="sourceText"> the source string. </param>
		''' <param name="owner"> the collation object. </param>
		Friend Sub New(ByVal sourceText As String, ByVal owner As RuleBasedCollator)
			Me.owner = owner
			ordering = owner.tables
			If sourceText.length() <> 0 Then
				Dim mode As sun.text.normalizer.NormalizerBase.Mode = sun.text.CollatorUtilities.toNormalizerMode(owner.decomposition)
				text = New sun.text.normalizer.NormalizerBase(sourceText, mode)
			End If
		End Sub

		''' <summary>
		''' CollationElementIterator constructor.  This takes the source string and
		''' the collation object.  The cursor will walk thru the source string based
		''' on the predefined collation rules.  If the source string is empty,
		''' NULLORDER will be returned on the calls to next(). </summary>
		''' <param name="sourceText"> the source string. </param>
		''' <param name="owner"> the collation object. </param>
		Friend Sub New(ByVal sourceText As CharacterIterator, ByVal owner As RuleBasedCollator)
			Me.owner = owner
			ordering = owner.tables
			Dim mode As sun.text.normalizer.NormalizerBase.Mode = sun.text.CollatorUtilities.toNormalizerMode(owner.decomposition)
			text = New sun.text.normalizer.NormalizerBase(sourceText, mode)
		End Sub

		''' <summary>
		''' Resets the cursor to the beginning of the string.  The next call
		''' to next() will return the first collation element in the string.
		''' </summary>
		Public Sub reset()
			If text IsNot Nothing Then
				text.reset()
				Dim mode As sun.text.normalizer.NormalizerBase.Mode = sun.text.CollatorUtilities.toNormalizerMode(owner.decomposition)
				text.mode = mode
			End If
			buffer = Nothing
			expIndex = 0
			swapOrder = 0
		End Sub

		''' <summary>
		''' Get the next collation element in the string.  <p>This iterator iterates
		''' over a sequence of collation elements that were built from the string.
		''' Because there isn't necessarily a one-to-one mapping from characters to
		''' collation elements, this doesn't mean the same thing as "return the
		''' collation element [or ordering priority] of the next character in the
		''' string".</p>
		''' <p>This function returns the collation element that the iterator is currently
		''' pointing to and then updates the internal pointer to point to the next element.
		''' previous() updates the pointer first and then returns the element.  This
		''' means that when you change direction while iterating (i.e., call next() and
		''' then call previous(), or call previous() and then call next()), you'll get
		''' back the same element twice.</p>
		''' </summary>
		''' <returns> the next collation element </returns>
		Public Function [next]() As Integer
			If text Is Nothing Then Return NULLORDER
			Dim textMode As sun.text.normalizer.NormalizerBase.Mode = text.mode
			' convert the owner's mode to something the Normalizer understands
			Dim ownerMode As sun.text.normalizer.NormalizerBase.Mode = sun.text.CollatorUtilities.toNormalizerMode(owner.decomposition)
			If textMode IsNot ownerMode Then text.mode = ownerMode

			' if buffer contains any decomposed char values
			' return their strength orders before continuing in
			' the Normalizer's CharacterIterator.
			If buffer IsNot Nothing Then
				If expIndex < buffer.Length Then
						Dim tempVar As Integer = expIndex
						expIndex += 1
						Return strengthOrder(buffer(tempVar))
				Else
					buffer = Nothing
					expIndex = 0
				End If
			ElseIf swapOrder <> 0 Then
				If Character.isSupplementaryCodePoint(swapOrder) Then
					Dim chars As Char() = Character.toChars(swapOrder)
					swapOrder = AscW(chars(1))
					Return AscW(chars(0)) << 16
				End If
				Dim order As Integer = swapOrder << 16
				swapOrder = 0
				Return order
			End If
			Dim ch As Integer = text.next()

			' are we at the end of Normalizer's text?
			If ch = sun.text.normalizer.NormalizerBase.DONE Then Return NULLORDER

			Dim value As Integer = ordering.getUnicodeOrder(ch)
			If value = RuleBasedCollator.UNMAPPED Then
				swapOrder = ch
				Return UNMAPPEDCHARVALUE
			ElseIf value >= RuleBasedCollator.CONTRACTCHARINDEX Then
				value = nextContractChar(ch)
			End If
			If value >= RuleBasedCollator.EXPANDCHARINDEX Then
				buffer = ordering.getExpandValueList(value)
				expIndex = 0
				value = buffer(expIndex)
				expIndex += 1
			End If

			If ordering.sEAsianSwapping Then
				Dim consonant As Integer
				If isThaiPreVowel(ch) Then
					consonant = text.next()
					If isThaiBaseConsonant(consonant) Then
						buffer = makeReorderedBuffer(consonant, value, buffer, True)
						value = buffer(0)
						expIndex = 1
					ElseIf consonant <> sun.text.normalizer.NormalizerBase.DONE Then
						text.previous()
					End If
				End If
				If isLaoPreVowel(ch) Then
					consonant = text.next()
					If isLaoBaseConsonant(consonant) Then
						buffer = makeReorderedBuffer(consonant, value, buffer, True)
						value = buffer(0)
						expIndex = 1
					ElseIf consonant <> sun.text.normalizer.NormalizerBase.DONE Then
						text.previous()
					End If
				End If
			End If

			Return strengthOrder(value)
		End Function

		''' <summary>
		''' Get the previous collation element in the string.  <p>This iterator iterates
		''' over a sequence of collation elements that were built from the string.
		''' Because there isn't necessarily a one-to-one mapping from characters to
		''' collation elements, this doesn't mean the same thing as "return the
		''' collation element [or ordering priority] of the previous character in the
		''' string".</p>
		''' <p>This function updates the iterator's internal pointer to point to the
		''' collation element preceding the one it's currently pointing to and then
		''' returns that element, while next() returns the current element and then
		''' updates the pointer.  This means that when you change direction while
		''' iterating (i.e., call next() and then call previous(), or call previous()
		''' and then call next()), you'll get back the same element twice.</p>
		''' </summary>
		''' <returns> the previous collation element
		''' @since 1.2 </returns>
		Public Function previous() As Integer
			If text Is Nothing Then Return NULLORDER
			Dim textMode As sun.text.normalizer.NormalizerBase.Mode = text.mode
			' convert the owner's mode to something the Normalizer understands
			Dim ownerMode As sun.text.normalizer.NormalizerBase.Mode = sun.text.CollatorUtilities.toNormalizerMode(owner.decomposition)
			If textMode IsNot ownerMode Then text.mode = ownerMode
			If buffer IsNot Nothing Then
				If expIndex > 0 Then
					expIndex -= 1
					Return strengthOrder(buffer(expIndex))
				Else
					buffer = Nothing
					expIndex = 0
				End If
			ElseIf swapOrder <> 0 Then
				If Character.isSupplementaryCodePoint(swapOrder) Then
					Dim chars As Char() = Character.toChars(swapOrder)
					swapOrder = AscW(chars(1))
					Return AscW(chars(0)) << 16
				End If
				Dim order As Integer = swapOrder << 16
				swapOrder = 0
				Return order
			End If
			Dim ch As Integer = text.previous()
			If ch = sun.text.normalizer.NormalizerBase.DONE Then Return NULLORDER

			Dim value As Integer = ordering.getUnicodeOrder(ch)

			If value = RuleBasedCollator.UNMAPPED Then
				swapOrder = UNMAPPEDCHARVALUE
				Return ch
			ElseIf value >= RuleBasedCollator.CONTRACTCHARINDEX Then
				value = prevContractChar(ch)
			End If
			If value >= RuleBasedCollator.EXPANDCHARINDEX Then
				buffer = ordering.getExpandValueList(value)
				expIndex = buffer.Length
				expIndex -= 1
				value = buffer(expIndex)
			End If

			If ordering.sEAsianSwapping Then
				Dim vowel As Integer
				If isThaiBaseConsonant(ch) Then
					vowel = text.previous()
					If isThaiPreVowel(vowel) Then
						buffer = makeReorderedBuffer(vowel, value, buffer, False)
						expIndex = buffer.Length - 1
						value = buffer(expIndex)
					Else
						text.next()
					End If
				End If
				If isLaoBaseConsonant(ch) Then
					vowel = text.previous()
					If isLaoPreVowel(vowel) Then
						buffer = makeReorderedBuffer(vowel, value, buffer, False)
						expIndex = buffer.Length - 1
						value = buffer(expIndex)
					Else
						text.next()
					End If
				End If
			End If

			Return strengthOrder(value)
		End Function

		''' <summary>
		''' Return the primary component of a collation element. </summary>
		''' <param name="order"> the collation element </param>
		''' <returns> the element's primary component </returns>
		Public Shared Function primaryOrder(ByVal order As Integer) As Integer
			order = order And RBCollationTables.PRIMARYORDERMASK
			Return (CInt(CUInt(order) >> RBCollationTables.PRIMARYORDERSHIFT))
		End Function
		''' <summary>
		''' Return the secondary component of a collation element. </summary>
		''' <param name="order"> the collation element </param>
		''' <returns> the element's secondary component </returns>
		Public Shared Function secondaryOrder(ByVal order As Integer) As Short
			order = order And RBCollationTables.SECONDARYORDERMASK
			Return (CShort(Fix(order >> RBCollationTables.SECONDARYORDERSHIFT)))
		End Function
		''' <summary>
		''' Return the tertiary component of a collation element. </summary>
		''' <param name="order"> the collation element </param>
		''' <returns> the element's tertiary component </returns>
		Public Shared Function tertiaryOrder(ByVal order As Integer) As Short
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Return (CShort(Fix(order = order And RBCollationTables.TERTIARYORDERMASK)))
		End Function

		''' <summary>
		'''  Get the comparison order in the desired strength.  Ignore the other
		'''  differences. </summary>
		'''  <param name="order"> The order value </param>
		Friend Function strengthOrder(ByVal order As Integer) As Integer
			Dim s As Integer = owner.strength
			If s = Collator.PRIMARY Then
				order = order And RBCollationTables.PRIMARYDIFFERENCEONLY
			ElseIf s = Collator.SECONDARY Then
				order = order And RBCollationTables.SECONDARYDIFFERENCEONLY
			End If
			Return order
		End Function

		''' <summary>
		''' Sets the iterator to point to the collation element corresponding to
		''' the specified character (the parameter is a CHARACTER offset in the
		''' original string, not an offset into its corresponding sequence of
		''' collation elements).  The value returned by the next call to next()
		''' will be the collation element corresponding to the specified position
		''' in the text.  If that position is in the middle of a contracting
		''' character sequence, the result of the next call to next() is the
		''' collation element for that sequence.  This means that getOffset()
		''' is not guaranteed to return the same value as was passed to a preceding
		''' call to setOffset().
		''' </summary>
		''' <param name="newOffset"> The new character offset into the original text.
		''' @since 1.2 </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Property offset As Integer
			Set(ByVal newOffset As Integer)
				If text IsNot Nothing Then
					If newOffset < text.beginIndex OrElse newOffset >= text.endIndex Then
							text.indexOnly = newOffset
					Else
						Dim c As Integer = text.indexdex(newOffset)
    
						' if the desired character isn't used in a contracting character
						' sequence, bypass all the backing-up logic-- we're sitting on
						' the right character already
						If ordering.usedInContractSeq(c) Then
							' walk backwards through the string until we see a character
							' that DOESN'T participate in a contracting character sequence
							Do While ordering.usedInContractSeq(c)
								c = text.previous()
							Loop
							' now walk forward using this object's next() method until
							' we pass the starting point and set our current position
							' to the beginning of the last "character" before or at
							' our starting position
							Dim last As Integer = text.index
							Do While text.index <= newOffset
								last = text.index
								[next]()
							Loop
							text.indexOnly = last
							' we don't need this, since last is the last index
							' that is the starting of the contraction which encompass
							' newOffset
							' text.previous();
						End If
					End If
				End If
				buffer = Nothing
				expIndex = 0
				swapOrder = 0
			End Set
			Get
				Return If(text IsNot Nothing, text.index, 0)
			End Get
		End Property



		''' <summary>
		''' Return the maximum length of any expansion sequences that end
		''' with the specified comparison order. </summary>
		''' <param name="order"> a collation order returned by previous or next. </param>
		''' <returns> the maximum length of any expansion sequences ending
		'''         with the specified order.
		''' @since 1.2 </returns>
		Public Function getMaxExpansion(ByVal order As Integer) As Integer
			Return ordering.getMaxExpansion(order)
		End Function

		''' <summary>
		''' Set a new string over which to iterate.
		''' </summary>
		''' <param name="source">  the new source text
		''' @since 1.2 </param>
		Public Property text As String
			Set(ByVal source As String)
				buffer = Nothing
				swapOrder = 0
				expIndex = 0
				Dim mode As sun.text.normalizer.NormalizerBase.Mode = sun.text.CollatorUtilities.toNormalizerMode(owner.decomposition)
				If text Is Nothing Then
					text = New sun.text.normalizer.NormalizerBase(source, mode)
				Else
					text.mode = mode
					text.text = source
				End If
			End Set
		End Property

		''' <summary>
		''' Set a new string over which to iterate.
		''' </summary>
		''' <param name="source">  the new source text.
		''' @since 1.2 </param>
		Public Property text As CharacterIterator
			Set(ByVal source As CharacterIterator)
				buffer = Nothing
				swapOrder = 0
				expIndex = 0
				Dim mode As sun.text.normalizer.NormalizerBase.Mode = sun.text.CollatorUtilities.toNormalizerMode(owner.decomposition)
				If text Is Nothing Then
					text = New sun.text.normalizer.NormalizerBase(source, mode)
				Else
					text.mode = mode
					text.text = source
				End If
			End Set
		End Property

		'============================================================
		' privates
		'============================================================

		''' <summary>
		''' Determine if a character is a Thai vowel (which sorts after
		''' its base consonant).
		''' </summary>
		Private Shared Function isThaiPreVowel(ByVal ch As Integer) As Boolean
			Return (ch >= &He40) AndAlso (ch <= &He44)
		End Function

		''' <summary>
		''' Determine if a character is a Thai base consonant
		''' </summary>
		Private Shared Function isThaiBaseConsonant(ByVal ch As Integer) As Boolean
			Return (ch >= &He01) AndAlso (ch <= &He2e)
		End Function

		''' <summary>
		''' Determine if a character is a Lao vowel (which sorts after
		''' its base consonant).
		''' </summary>
		Private Shared Function isLaoPreVowel(ByVal ch As Integer) As Boolean
			Return (ch >= &Hec0) AndAlso (ch <= &Hec4)
		End Function

		''' <summary>
		''' Determine if a character is a Lao base consonant
		''' </summary>
		Private Shared Function isLaoBaseConsonant(ByVal ch As Integer) As Boolean
			Return (ch >= &He81) AndAlso (ch <= &Heae)
		End Function

		''' <summary>
		''' This method produces a buffer which contains the collation
		''' elements for the two characters, with colFirst's values preceding
		''' another character's.  Presumably, the other character precedes colFirst
		''' in logical order (otherwise you wouldn't need this method would you?).
		''' The assumption is that the other char's value(s) have already been
		''' computed.  If this char has a single element it is passed to this
		''' method as lastValue, and lastExpansion is null.  If it has an
		''' expansion it is passed in lastExpansion, and colLastValue is ignored.
		''' </summary>
		Private Function makeReorderedBuffer(ByVal colFirst As Integer, ByVal lastValue As Integer, ByVal lastExpansion As Integer(), ByVal forward As Boolean) As Integer()

			Dim result As Integer()

			Dim firstValue As Integer = ordering.getUnicodeOrder(colFirst)
			If firstValue >= RuleBasedCollator.CONTRACTCHARINDEX Then firstValue = If(forward, nextContractChar(colFirst), prevContractChar(colFirst))

			Dim firstExpansion As Integer() = Nothing
			If firstValue >= RuleBasedCollator.EXPANDCHARINDEX Then firstExpansion = ordering.getExpandValueList(firstValue)

			If Not forward Then
				Dim temp1 As Integer = firstValue
				firstValue = lastValue
				lastValue = temp1
				Dim temp2 As Integer() = firstExpansion
				firstExpansion = lastExpansion
				lastExpansion = temp2
			End If

			If firstExpansion Is Nothing AndAlso lastExpansion Is Nothing Then
				result = New Integer (1){}
				result(0) = firstValue
				result(1) = lastValue
			Else
				Dim firstLength As Integer = If(firstExpansion Is Nothing, 1, firstExpansion.Length)
				Dim lastLength As Integer = If(lastExpansion Is Nothing, 1, lastExpansion.Length)
				result = New Integer(firstLength + lastLength - 1){}

				If firstExpansion Is Nothing Then
					result(0) = firstValue
				Else
					Array.Copy(firstExpansion, 0, result, 0, firstLength)
				End If

				If lastExpansion Is Nothing Then
					result(firstLength) = lastValue
				Else
					Array.Copy(lastExpansion, 0, result, firstLength, lastLength)
				End If
			End If

			Return result
		End Function

		''' <summary>
		'''  Check if a comparison order is ignorable. </summary>
		'''  <returns> true if a character is ignorable, false otherwise. </returns>
		Friend Shared Function isIgnorable(ByVal order As Integer) As Boolean
			Return (If(primaryOrder(order) = 0, True, False))
		End Function

		''' <summary>
		''' Get the ordering priority of the next contracting character in the
		''' string. </summary>
		''' <param name="ch"> the starting character of a contracting character token </param>
		''' <returns> the next contracting character's ordering.  Returns NULLORDER
		''' if the end of string is reached. </returns>
		Private Function nextContractChar(ByVal ch As Integer) As Integer
			' First get the ordering of this single character,
			' which is always the first element in the list
			Dim list As List(Of EntryPair) = ordering.getContractValues(ch)
			Dim pair As EntryPair = list(0)
			Dim order As Integer = pair.value

			' find out the length of the longest contracting character sequence in the list.
			' There's logic in the builder code to make sure the longest sequence is always
			' the last.
			pair = list(list.Count - 1)
			Dim maxLength As Integer = pair.entryName.length()

			' (the Normalizer is cloned here so that the seeking we do in the next loop
			' won't affect our real position in the text)
			Dim tempText As sun.text.normalizer.NormalizerBase = CType(text.clone(), sun.text.normalizer.NormalizerBase)

			' extract the next maxLength characters in the string (we have to do this using the
			' Normalizer to ensure that our offsets correspond to those the rest of the
			' iterator is using) and store it in "fragment".
			tempText.previous()
			key.length = 0
			Dim c As Integer = tempText.next()
			Do While maxLength > 0 AndAlso c <> sun.text.normalizer.NormalizerBase.DONE
				If Character.isSupplementaryCodePoint(c) Then
					key.append(Character.toChars(c))
					maxLength -= 2
				Else
					key.append(ChrW(c))
					maxLength -= 1
				End If
				c = tempText.next()
			Loop
			Dim fragment As String = key.ToString()
			' now that we have that fragment, iterate through this list looking for the
			' longest sequence that matches the characters in the actual text.  (maxLength
			' is used here to keep track of the length of the longest sequence)
			' Upon exit from this loop, maxLength will contain the length of the matching
			' sequence and order will contain the collation-element value corresponding
			' to this sequence
			maxLength = 1
			For i As Integer = list.Count - 1 To 1 Step -1
				pair = list(i)
				If Not pair.fwd Then Continue For

				If fragment.StartsWith(pair.entryName) AndAlso pair.entryName.length() > maxLength Then
					maxLength = pair.entryName.length()
					order = pair.value
				End If
			Next i

			' seek our current iteration position to the end of the matching sequence
			' and return the appropriate collation-element value (if there was no matching
			' sequence, we're already seeked to the right position and order already contains
			' the correct collation-element value for the single character)
			Do While maxLength > 1
				c = text.next()
				maxLength -= Character.charCount(c)
			Loop
			Return order
		End Function

		''' <summary>
		''' Get the ordering priority of the previous contracting character in the
		''' string. </summary>
		''' <param name="ch"> the starting character of a contracting character token </param>
		''' <returns> the next contracting character's ordering.  Returns NULLORDER
		''' if the end of string is reached. </returns>
		Private Function prevContractChar(ByVal ch As Integer) As Integer
			' This function is identical to nextContractChar(), except that we've
			' switched things so that the next() and previous() calls on the Normalizer
			' are switched and so that we skip entry pairs with the fwd flag turned on
			' rather than off.  Notice that we still use append() and startsWith() when
			' working on the fragment.  This is because the entry pairs that are used
			' in reverse iteration have their names reversed already.
			Dim list As List(Of EntryPair) = ordering.getContractValues(ch)
			Dim pair As EntryPair = list(0)
			Dim order As Integer = pair.value

			pair = list(list.Count - 1)
			Dim maxLength As Integer = pair.entryName.length()

			Dim tempText As sun.text.normalizer.NormalizerBase = CType(text.clone(), sun.text.normalizer.NormalizerBase)

			tempText.next()
			key.length = 0
			Dim c As Integer = tempText.previous()
			Do While maxLength > 0 AndAlso c <> sun.text.normalizer.NormalizerBase.DONE
				If Character.isSupplementaryCodePoint(c) Then
					key.append(Character.toChars(c))
					maxLength -= 2
				Else
					key.append(ChrW(c))
					maxLength -= 1
				End If
				c = tempText.previous()
			Loop
			Dim fragment As String = key.ToString()

			maxLength = 1
			For i As Integer = list.Count - 1 To 1 Step -1
				pair = list(i)
				If pair.fwd Then Continue For

				If fragment.StartsWith(pair.entryName) AndAlso pair.entryName.length() > maxLength Then
					maxLength = pair.entryName.length()
					order = pair.value
				End If
			Next i

			Do While maxLength > 1
				c = text.previous()
				maxLength -= Character.charCount(c)
			Loop
			Return order
		End Function

		Friend Const UNMAPPEDCHARVALUE As Integer = &H7FFF0000

		Private text As sun.text.normalizer.NormalizerBase = Nothing
		Private buffer As Integer() = Nothing
		Private expIndex As Integer = 0
		Private key As New StringBuffer(5)
		Private swapOrder As Integer = 0
		Private ordering As RBCollationTables
		Private owner As RuleBasedCollator
	End Class

End Namespace