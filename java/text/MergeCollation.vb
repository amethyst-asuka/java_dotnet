Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 1996, 2012, Oracle and/or its affiliates. All rights reserved.
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
' * (C) Copyright IBM Corp. 1996, 1997 - All Rights Reserved
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
	''' Utility class for normalizing and merging patterns for collation.
	''' Patterns are strings of the form <entry>*, where <entry> has the
	''' form:
	''' <pattern> := <entry>*
	''' <entry> := <separator><chars>{"/"<extension>}
	''' <separator> := "=", ",", ";", "<", "&"
	''' <chars>, and <extension> are both arbitrary strings.
	''' unquoted whitespaces are ignored.
	''' 'xxx' can be used to quote characters
	''' One difference from Collator is that & is used to reset to a current
	''' point. Or, in other words, it introduces a new sequence which is to
	''' be added to the old.
	''' That is: "a < b < c < d" is the same as "a < b & b < c & c < d" OR
	''' "a < b < d & b < c"
	''' XXX: make '' be a single quote. </summary>
	''' <seealso cref= PatternEntry
	''' @author             Mark Davis, Helena Shih </seealso>

	Friend NotInheritable Class MergeCollation

		''' <summary>
		''' Creates from a pattern </summary>
		''' <exception cref="ParseException"> If the input pattern is incorrect. </exception>
		Public Sub New(ByVal pattern As String)
			For i As Integer = 0 To statusArray.Length - 1
				statusArray(i) = 0
			Next i
			pattern = pattern
		End Sub

		''' <summary>
		''' recovers current pattern
		''' </summary>
		Public Property pattern As String
			Get
				Return getPattern(True)
			End Get
			Set(ByVal pattern As String)
				patterns.Clear()
				addPattern(pattern)
			End Set
		End Property

		''' <summary>
		''' recovers current pattern. </summary>
		''' <param name="withWhiteSpace"> puts spacing around the entries, and \n
		''' before & and < </param>
		Public Function getPattern(ByVal withWhiteSpace As Boolean) As String
			Dim result As New StringBuffer
			Dim tmp As PatternEntry = Nothing
			Dim extList As List(Of PatternEntry) = Nothing
			Dim i As Integer
			For i = 0 To patterns.Count - 1
				Dim entry As PatternEntry = patterns(i)
				If entry.extension.length() <> 0 Then
					If extList Is Nothing Then extList = New List(Of )
					extList.Add(entry)
				Else
					If extList IsNot Nothing Then
						Dim last As PatternEntry = findLastWithNoExtension(i-1)
						For j As Integer = extList.Count - 1 To 0 Step -1
							tmp = extList(j)
							tmp.addToBuffer(result, False, withWhiteSpace, last)
						Next j
						extList = Nothing
					End If
					entry.addToBuffer(result, False, withWhiteSpace, Nothing)
				End If
			Next i
			If extList IsNot Nothing Then
				Dim last As PatternEntry = findLastWithNoExtension(i-1)
				For j As Integer = extList.Count - 1 To 0 Step -1
					tmp = extList(j)
					tmp.addToBuffer(result, False, withWhiteSpace, last)
				Next j
				extList = Nothing
			End If
			Return result.ToString()
		End Function

		Private Function findLastWithNoExtension(ByVal i As Integer) As PatternEntry
			For i = i - 1 To 0 Step -1
				Dim entry As PatternEntry = patterns(i)
				If entry.extension.length() = 0 Then Return entry
			Next i
			Return Nothing
		End Function

		''' <summary>
		''' emits the pattern for collation builder. </summary>
		''' <returns> emits the string in the format understable to the collation
		''' builder. </returns>
		Public Function emitPattern() As String
			Return emitPattern(True)
		End Function

		''' <summary>
		''' emits the pattern for collation builder. </summary>
		''' <param name="withWhiteSpace"> puts spacing around the entries, and \n
		''' before & and < </param>
		''' <returns> emits the string in the format understable to the collation
		''' builder. </returns>
		Public Function emitPattern(ByVal withWhiteSpace As Boolean) As String
			Dim result As New StringBuffer
			For i As Integer = 0 To patterns.Count - 1
				Dim entry As PatternEntry = patterns(i)
				If entry IsNot Nothing Then entry.addToBuffer(result, True, withWhiteSpace, Nothing)
			Next i
			Return result.ToString()
		End Function


		''' <summary>
		''' adds a pattern to the current one. </summary>
		''' <param name="pattern"> the new pattern to be added </param>
		Public Sub addPattern(ByVal pattern As String)
			If pattern Is Nothing Then Return

			Dim parser As New PatternEntry.Parser(pattern)

			Dim entry As PatternEntry = parser.next()
			Do While entry IsNot Nothing
				fixEntry(entry)
				entry = parser.next()
			Loop
		End Sub

		''' <summary>
		''' gets count of separate entries </summary>
		''' <returns> the size of pattern entries </returns>
		Public Property count As Integer
			Get
				Return patterns.Count
			End Get
		End Property

		''' <summary>
		''' gets count of separate entries </summary>
		''' <param name="index"> the offset of the desired pattern entry </param>
		''' <returns> the requested pattern entry </returns>
		Public Function getItemAt(ByVal index As Integer) As PatternEntry
			Return patterns(index)
		End Function

		'============================================================
		' privates
		'============================================================
		Friend patterns As New List(Of PatternEntry) ' a list of PatternEntries

		<NonSerialized> _
		Private saveEntry As PatternEntry = Nothing
		<NonSerialized> _
		Private lastEntry As PatternEntry = Nothing

		' This is really used as a local variable inside fixEntry, but we cache
		' it here to avoid newing it up every time the method is called.
		<NonSerialized> _
		Private excess As New StringBuffer

		'
		' When building a MergeCollation, we need to do lots of searches to see
		' whether a given entry is already in the table.  Since we're using an
		' array, this would make the algorithm O(N*N).  To speed things up, we
		' use this bit array to remember whether the array contains any entries
		' starting with each Unicode character.  If not, we can avoid the search.
		' Using BitSet would make this easier, but it's significantly slower.
		'
		<NonSerialized> _
		Private statusArray As SByte() = New SByte(8191){}
		Private ReadOnly BITARRAYMASK As SByte = CByte(&H1)
		Private ReadOnly BYTEPOWER As Integer = 3
		Private ReadOnly BYTEMASK As Integer = (1 << BYTEPOWER) - 1

	'    
	'      If the strength is RESET, then just change the lastEntry to
	'      be the current. (If the current is not in patterns, signal an error).
	'      If not, then remove the current entry, and add it after lastEntry
	'      (which is usually at the end).
	'      
		Private Sub fixEntry(ByVal newEntry As PatternEntry)
			' check to see whether the new entry has the same characters as the previous
			' entry did (this can happen when a pattern declaring a difference between two
			' strings that are canonically equivalent is normalized).  If so, and the strength
			' is anything other than IDENTICAL or RESET, throw an exception (you can't
			' declare a string to be unequal to itself).       --rtg 5/24/99
			If lastEntry IsNot Nothing AndAlso newEntry.chars.Equals(lastEntry.chars) AndAlso newEntry.extension.Equals(lastEntry.extension) Then
				If newEntry.strength <> Collator.IDENTICAL AndAlso newEntry.strength <> PatternEntry.RESET Then
						Throw New ParseException("The entries " & lastEntry & " and " & newEntry & " are adjacent in the rules, but have conflicting " & "strengths: A character can't be unequal to itself.", -1)
				Else
					' otherwise, just skip this entry and behave as though you never saw it
					Return
				End If
			End If

			Dim changeLastEntry As Boolean = True
			If newEntry.strength <> PatternEntry.RESET Then
				Dim oldIndex As Integer = -1

				If (newEntry.chars.length() = 1) Then

					Dim c As Char = newEntry.chars.Chars(0)
					Dim statusIndex As Integer = AscW(c) >> BYTEPOWER
					Dim bitClump As SByte = statusArray(statusIndex)
					Dim bitBit As SByte = CByte(BITARRAYMASK << (AscW(c) And BYTEMASK))

					If bitClump <> 0 AndAlso (bitClump And bitBit) <> 0 Then
						oldIndex = patterns.LastIndexOf(newEntry)
					Else
						' We're going to add an element that starts with this
						' character, so go ahead and set its bit.
						statusArray(statusIndex) = CByte(bitClump Or bitBit)
					End If
				Else
					oldIndex = patterns.LastIndexOf(newEntry)
				End If
				If oldIndex <> -1 Then patterns.RemoveAt(oldIndex)

				excess.length = 0
				Dim lastIndex As Integer = findLastEntry(lastEntry, excess)

				If excess.length() <> 0 Then
					newEntry.extension = excess + newEntry.extension
					If lastIndex <> patterns.Count Then
						lastEntry = saveEntry
						changeLastEntry = False
					End If
				End If
				If lastIndex = patterns.Count Then
					patterns.Add(newEntry)
					saveEntry = newEntry
				Else
					patterns.Insert(lastIndex, newEntry)
				End If
			End If
			If changeLastEntry Then lastEntry = newEntry
		End Sub

		Private Function findLastEntry(ByVal entry As PatternEntry, ByVal excessChars As StringBuffer) As Integer
			If entry Is Nothing Then Return 0

			If entry.strength <> PatternEntry.RESET Then
				' Search backwards for string that contains this one;
				' most likely entry is last one

				Dim oldIndex As Integer = -1
				If (entry.chars.length() = 1) Then
					Dim index As Integer = AscW(entry.chars.Chars(0)) >> BYTEPOWER
					If (statusArray(index) And (BITARRAYMASK << (AscW(entry.chars.Chars(0)) And BYTEMASK))) <> 0 Then oldIndex = patterns.LastIndexOf(entry)
				Else
					oldIndex = patterns.LastIndexOf(entry)
				End If
				If (oldIndex = -1) Then Throw New ParseException("couldn't find last entry: " & entry, oldIndex)
				Return oldIndex + 1
			Else
				Dim i As Integer
				For i = patterns.Count - 1 To 0 Step -1
					Dim e As PatternEntry = patterns(i)
					If e.chars.regionMatches(0,entry.chars,0, e.chars.length()) Then
						excessChars.append(entry.chars.Substring(e.chars.length(), entry.chars.length() - (e.chars.length())))
						Exit For
					End If
				Next i
				If i = -1 Then Throw New ParseException("couldn't find: " & entry, i)
				Return i + 1
			End If
		End Function
	End Class

End Namespace