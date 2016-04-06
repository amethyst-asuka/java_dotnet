Imports System.Collections.Generic

'
' * Copyright (c) 1999, 2012, Oracle and/or its affiliates. All rights reserved.
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
	''' This class contains all the code to parse a RuleBasedCollator pattern
	''' and build a RBCollationTables object from it.  A particular instance
	''' of tis class exists only during the actual build process-- once an
	''' RBCollationTables object has been built, the RBTableBuilder object
	''' goes away.  This object carries all of the state which is only needed
	''' during the build process, plus a "shadow" copy of all of the state
	''' that will go into the tables object itself.  This object communicates
	''' with RBCollationTables through a separate [Class], RBCollationTables.BuildAPI,
	''' this is an inner class of RBCollationTables and provides a separate
	''' private API for communication with RBTableBuilder.
	''' This class isn't just an inner class of RBCollationTables itself because
	''' of its large size.  For source-code readability, it seemed better for the
	''' builder to have its own source file.
	''' </summary>
	Friend NotInheritable Class RBTableBuilder

		Public Sub New(  tables As RBCollationTables.BuildAPI)
			Me.tables = tables
		End Sub

		''' <summary>
		''' Create a table-based collation object with the given rules.
		''' This is the main function that actually builds the tables and
		''' stores them back in the RBCollationTables object.  It is called
		''' ONLY by the RBCollationTables constructor. </summary>
		''' <seealso cref= RuleBasedCollator#RuleBasedCollator </seealso>
		''' <exception cref="ParseException"> If the rules format is incorrect. </exception>

		Public Sub build(  pattern As String,   decmp As Integer)
			Dim isSource As Boolean = True
			Dim i As Integer = 0
			Dim expChars As String
			Dim groupChars As String
			If pattern.length() = 0 Then Throw New ParseException("Build rules empty.", 0)

			' This array maps Unicode characters to their collation ordering
			mapping = New sun.text.UCompactIntArray(RBCollationTables.UNMAPPED)
			' Normalize the build rules.  Find occurances of all decomposed characters
			' and normalize the rules before feeding into the builder.  By "normalize",
			' we mean that all precomposed Unicode characters must be converted into
			' a base character and one or more combining characters (such as accents).
			' When there are multiple combining characters attached to a base character,
			' the combining characters must be in their canonical order
			'
			' sherman/Note:
			'(1)decmp will be NO_DECOMPOSITION only in ko locale to prevent decompose
			'hangual syllables to jamos, so we can actually just call decompose with
			'normalizer's IGNORE_HANGUL option turned on
			'
			'(2)just call the "special version" in NormalizerImpl directly
			'pattern = Normalizer.decompose(pattern, false, Normalizer.IGNORE_HANGUL, true);
			'
			'Normalizer.Mode mode = CollatorUtilities.toNormalizerMode(decmp);
			'pattern = Normalizer.normalize(pattern, mode, 0, true);

			pattern = sun.text.normalizer.NormalizerImpl.canonicalDecomposeWithSingleQuotation(pattern)

			' Build the merged collation entries
			' Since rules can be specified in any order in the string
			' (e.g. "c , C < d , D < e , E .... C < CH")
			' this splits all of the rules in the string out into separate
			' objects and then sorts them.  In the above example, it merges the
			' "C < CH" rule in just before the "C < D" rule.
			'

			mPattern = New MergeCollation(pattern)

			Dim order As Integer = 0

			' Now walk though each entry and add it to my own tables
			For i = 0 To mPattern.count - 1
				Dim entry As PatternEntry = mPattern.getItemAt(i)
				If entry IsNot Nothing Then
					groupChars = entry.chars
					If groupChars.length() > 1 Then
						Select Case groupChars.Chars(groupChars.length()-1)
						Case "@"c
							frenchSec = True
							groupChars = groupChars.Substring(0, groupChars.length()-1)
						Case "!"c
							seAsianSwapping = True
							groupChars = groupChars.Substring(0, groupChars.length()-1)
						End Select
					End If

					order = increment(entry.strength, order)
					expChars = entry.extension

					If expChars.length() <> 0 Then
						addExpandOrder(groupChars, expChars, order)
					ElseIf groupChars.length() > 1 Then
						Dim ch As Char = groupChars.Chars(0)
						If Char.IsHighSurrogate(ch) AndAlso groupChars.length() = 2 Then
							addOrder(Character.toCodePoint(ch, groupChars.Chars(1)), order)
						Else
							addContractOrder(groupChars, order)
						End If
					Else
						Dim ch As Char = groupChars.Chars(0)
						addOrder(ch, order)
					End If
				End If
			Next i
			addComposedChars()

			commit()
			mapping.compact()
	'        
	'        System.out.println("mappingSize=" + mapping.getKSize());
	'        for (int j = 0; j < 0xffff; j++) {
	'            int value = mapping.elementAt(j);
	'            if (value != RBCollationTables.UNMAPPED)
	'                System.out.println("index=" +  java.lang.[Integer].toString(j, 16)
	'                           + ", value=" +  java.lang.[Integer].toString(value, 16));
	'        }
	'        
			tables.fillInTables(frenchSec, seAsianSwapping, mapping, contractTable, expandTable, contractFlags, maxSecOrder, maxTerOrder)
		End Sub

		''' <summary>
		''' Add expanding entries for pre-composed unicode characters so that this
		''' collator can be used reasonably well with decomposition turned off.
		''' </summary>
		Private Sub addComposedChars()
			' Iterate through all of the pre-composed characters in Unicode
			Dim iter As New sun.text.ComposedCharIter
			Dim c As Integer
			c = iter.next()
			Do While c <> sun.text.ComposedCharIter.DONE
				If getCharOrder(c) = RBCollationTables.UNMAPPED Then
					'
					' We don't already have an ordering for this pre-composed character.
					'
					' First, see if the decomposed string is already in our
					' tables as a single contracting-string ordering.
					' If so, just map the precomposed character to that order.
					'
					' TODO: What we should really be doing here is trying to find the
					' longest initial substring of the decomposition that is present
					' in the tables as a contracting character sequence, and find its
					' ordering.  Then do this recursively with the remaining chars
					' so that we build a list of orderings, and add that list to
					' the expansion table.
					' That would be more correct but also significantly slower, so
					' I'm not totally sure it's worth doing.
					'
					Dim s As String = iter.decomposition()

					'sherman/Note: if this is 1 character decomposed string, the
					'only thing need to do is to check if this decomposed character
					'has an entry in our order table, this order is not necessary
					'to be a contraction order, if it does have one, add an entry
					'for the precomposed character by using the same order, the
					'previous impl unnecessarily adds a single character expansion
					'entry.
					If s.length() = 1 Then
						Dim order As Integer = getCharOrder(s.Chars(0))
						If order <> RBCollationTables.UNMAPPED Then addOrder(c, order)
						c = iter.next()
						Continue Do
					ElseIf s.length() = 2 Then
						Dim ch0 As Char = s.Chars(0)
						If Char.IsHighSurrogate(ch0) Then
							Dim order As Integer = getCharOrder(s.codePointAt(0))
							If order <> RBCollationTables.UNMAPPED Then addOrder(c, order)
							c = iter.next()
							Continue Do
						End If
					End If
					Dim contractOrder_Renamed As Integer = getContractOrder(s)
					If contractOrder_Renamed <> RBCollationTables.UNMAPPED Then
						addOrder(c, contractOrder_Renamed)
					Else
						'
						' We don't have a contracting ordering for the entire string
						' that results from the decomposition, but if we have orders
						' for each individual character, we can add an expanding
						' table entry for the pre-composed character
						'
						Dim allThere As Boolean = True
						For i As Integer = 0 To s.length() - 1
							If getCharOrder(s.Chars(i)) = RBCollationTables.UNMAPPED Then
								allThere = False
								Exit For
							End If
						Next i
						If allThere Then addExpandOrder(c, s, RBCollationTables.UNMAPPED)
					End If
				End If
				c = iter.next()
			Loop
		End Sub

		''' <summary>
		''' Look up for unmapped values in the expanded character table.
		''' 
		''' When the expanding character tables are built by addExpandOrder,
		''' it doesn't know what the final ordering of each character
		''' in the expansion will be.  Instead, it just puts the raw character
		''' code into the table, adding CHARINDEX as a flag.  Now that we've
		''' finished building the mapping table, we can go back and look up
		''' that character to see what its real collation order is and
		''' stick that into the expansion table.  That lets us avoid doing
		''' a two-stage lookup later.
		''' </summary>
		Private Sub commit()
			If expandTable IsNot Nothing Then
				For i As Integer = 0 To expandTable.Count - 1
					Dim valueList As Integer() = expandTable(i)
					For j As Integer = 0 To valueList.Length - 1
						Dim order As Integer = valueList(j)
						If order < RBCollationTables.EXPANDCHARINDEX AndAlso order > CHARINDEX Then
							' found a expanding character that isn't filled in yet
							Dim ch As Integer = order - CHARINDEX

							' Get the real values for the non-filled entry
							Dim realValue As Integer = getCharOrder(ch)

							If realValue = RBCollationTables.UNMAPPED Then
								' The real value is still unmapped, maybe it's ignorable
								valueList(j) = IGNORABLEMASK And ch
							Else
								' just fill in the value
								valueList(j) = realValue
							End If
						End If
					Next j
				Next i
			End If
		End Sub
		''' <summary>
		'''  Increment of the last order based on the comparison level.
		''' </summary>
		Private Function increment(  aStrength As Integer,   lastValue As Integer) As Integer
			Select Case aStrength
			Case Collator.PRIMARY
				' increment priamry order  and mask off secondary and tertiary difference
				lastValue += PRIMARYORDERINCREMENT
				lastValue = lastValue And RBCollationTables.PRIMARYORDERMASK
				isOverIgnore = True
			Case Collator.SECONDARY
				' increment secondary order and mask off tertiary difference
				lastValue += SECONDARYORDERINCREMENT
				lastValue = lastValue And RBCollationTables.SECONDARYDIFFERENCEONLY
				' record max # of ignorable chars with secondary difference
				If Not isOverIgnore Then maxSecOrder += 1
			Case Collator.TERTIARY
				' increment tertiary order
				lastValue += TERTIARYORDERINCREMENT
				' record max # of ignorable chars with tertiary difference
				If Not isOverIgnore Then maxTerOrder += 1
			End Select
			Return lastValue
		End Function

		''' <summary>
		'''  Adds a character and its designated order into the collation table.
		''' </summary>
		Private Sub addOrder(  ch As Integer,   anOrder As Integer)
			' See if the char already has an order in the mapping table
			Dim order As Integer = mapping.elementAt(ch)

			If order >= RBCollationTables.CONTRACTCHARINDEX Then
				' There's already an entry for this character that points to a contracting
				' character table.  Instead of adding the character directly to the mapping
				' table, we must add it to the contract table instead.
				Dim length As Integer = 1
				If Character.isSupplementaryCodePoint(ch) Then
					length = Character.toChars(ch, keyBuf, 0)
				Else
					keyBuf(0) = ChrW(ch)
				End If
				addContractOrder(New String(keyBuf, 0, length), anOrder)
			Else
				' add the entry to the mapping table,
				' the same later entry replaces the previous one
				mapping.elementAttAt(ch, anOrder)
			End If
		End Sub

		Private Sub addContractOrder(  groupChars As String,   anOrder As Integer)
			addContractOrder(groupChars, anOrder, True)
		End Sub

		''' <summary>
		'''  Adds the contracting string into the collation table.
		''' </summary>
		Private Sub addContractOrder(  groupChars As String,   anOrder As Integer,   fwd As Boolean)
			If contractTable Is Nothing Then contractTable = New List(Of )(INITIALTABLESIZE)

			'initial character
			Dim ch As Integer = groupChars.codePointAt(0)
	'        
	'        char ch0 = groupChars.charAt(0);
	'        int ch = Character.isHighSurrogate(ch0)?
	'          Character.toCodePoint(ch0, groupChars.charAt(1)):ch0;
	'          
			' See if the initial character of the string already has a contract table.
			Dim entry As Integer = mapping.elementAt(ch)
			Dim entryTable As List(Of EntryPair) = getContractValuesImpl(entry - RBCollationTables.CONTRACTCHARINDEX)

			If entryTable Is Nothing Then
				' We need to create a new table of contract entries for this base char
				Dim tableIndex As Integer = RBCollationTables.CONTRACTCHARINDEX + contractTable.Count
				entryTable = New List(Of )(INITIALTABLESIZE)
				contractTable.Add(entryTable)

				' Add the initial character's current ordering first. then
				' update its mapping to point to this contract table
				entryTable.Add(New EntryPair(groupChars.Substring(0,Character.charCount(ch)), entry))
				mapping.elementAttAt(ch, tableIndex)
			End If

			' Now add (or replace) this string in the table
			Dim index As Integer = RBCollationTables.getEntry(entryTable, groupChars, fwd)
			If index <> RBCollationTables.UNMAPPED Then
				Dim pair As EntryPair = entryTable(index)
				pair.value = anOrder
			Else
				Dim pair As EntryPair = entryTable(entryTable.Count - 1)

				' NOTE:  This little bit of logic is here to speed CollationElementIterator
				' .nextContractChar().  This code ensures that the longest sequence in
				' this list is always the _last_ one in the list.  This keeps
				' nextContractChar() from having to search the entire list for the longest
				' sequence.
				If groupChars.length() > pair.entryName.length() Then
					entryTable.Add(New EntryPair(groupChars, anOrder, fwd))
				Else
					entryTable.Insert(entryTable.Count - 1, New EntryPair(groupChars, anOrder, fwd))
				End If
			End If

			' If this was a forward mapping for a contracting string, also add a
			' reverse mapping for it, so that CollationElementIterator.previous
			' can work right
			If fwd AndAlso groupChars.length() > 1 Then
				addContractFlags(groupChars)
				addContractOrder((New StringBuffer(groupChars)).reverse().ToString(), anOrder, False)
			End If
		End Sub

		''' <summary>
		''' If the given string has been specified as a contracting string
		''' in this collation table, return its ordering.
		''' Otherwise return UNMAPPED.
		''' </summary>
		Private Function getContractOrder(  groupChars As String) As Integer
			Dim result As Integer = RBCollationTables.UNMAPPED
			If contractTable IsNot Nothing Then
				Dim ch As Integer = groupChars.codePointAt(0)
	'            
	'            char ch0 = groupChars.charAt(0);
	'            int ch = Character.isHighSurrogate(ch0)?
	'              Character.toCodePoint(ch0, groupChars.charAt(1)):ch0;
	'              
				Dim entryTable As List(Of EntryPair) = getContractValues(ch)
				If entryTable IsNot Nothing Then
					Dim index As Integer = RBCollationTables.getEntry(entryTable, groupChars, True)
					If index <> RBCollationTables.UNMAPPED Then
						Dim pair As EntryPair = entryTable(index)
						result = pair.value
					End If
				End If
			End If
			Return result
		End Function

		Private Function getCharOrder(  ch As Integer) As Integer
			Dim order As Integer = mapping.elementAt(ch)

			If order >= RBCollationTables.CONTRACTCHARINDEX Then
				Dim groupList As List(Of EntryPair) = getContractValuesImpl(order - RBCollationTables.CONTRACTCHARINDEX)
				Dim pair As EntryPair = groupList(0)
				order = pair.value
			End If
			Return order
		End Function

		''' <summary>
		'''  Get the entry of hash table of the contracting string in the collation
		'''  table. </summary>
		'''  <param name="ch"> the starting character of the contracting string </param>
		Private Function getContractValues(  ch As Integer) As List(Of EntryPair)
			Dim index As Integer = mapping.elementAt(ch)
			Return getContractValuesImpl(index - RBCollationTables.CONTRACTCHARINDEX)
		End Function

		Private Function getContractValuesImpl(  index As Integer) As List(Of EntryPair)
			If index >= 0 Then
				Return contractTable(index)
			Else ' not found
				Return Nothing
			End If
		End Function

		''' <summary>
		'''  Adds the expanding string into the collation table.
		''' </summary>
		Private Sub addExpandOrder(  contractChars As String,   expandChars As String,   anOrder As Integer)
			' Create an expansion table entry
			Dim tableIndex As Integer = addExpansion(anOrder, expandChars)

			' And add its index into the main mapping table
			If contractChars.length() > 1 Then
				Dim ch As Char = contractChars.Chars(0)
				If Char.IsHighSurrogate(ch) AndAlso contractChars.length() = 2 Then
					Dim ch2 As Char = contractChars.Chars(1)
					If Char.IsLowSurrogate(ch2) Then addOrder(Character.toCodePoint(ch, ch2), tableIndex)
				Else
					addContractOrder(contractChars, tableIndex)
				End If
			Else
				addOrder(contractChars.Chars(0), tableIndex)
			End If
		End Sub

		Private Sub addExpandOrder(  ch As Integer,   expandChars As String,   anOrder As Integer)
			Dim tableIndex As Integer = addExpansion(anOrder, expandChars)
			addOrder(ch, tableIndex)
		End Sub

		''' <summary>
		''' Create a new entry in the expansion table that contains the orderings
		''' for the given characers.  If anOrder is valid, it is added to the
		''' beginning of the expanded list of orders.
		''' </summary>
		Private Function addExpansion(  anOrder As Integer,   expandChars As String) As Integer
			If expandTable Is Nothing Then expandTable = New List(Of )(INITIALTABLESIZE)

			' If anOrder is valid, we want to add it at the beginning of the list
			Dim offset As Integer = If(anOrder = RBCollationTables.UNMAPPED, 0, 1)

			Dim valueList As Integer() = New Integer(expandChars.length() + offset - 1){}
			If offset = 1 Then valueList(0) = anOrder

			Dim j As Integer = offset
			For i As Integer = 0 To expandChars.length() - 1
				Dim ch0 As Char = expandChars.Chars(i)
				Dim ch1 As Char
				Dim ch As Integer
				If Char.IsHighSurrogate(ch0) Then
					i += 1
					ch1=expandChars.Chars(i)
					If i = expandChars.length() OrElse (Not Character.isLowSurrogatech1) Then Exit For
					ch = Character.toCodePoint(ch0, ch1)

				Else
					ch = AscW(ch0)
				End If

				Dim mapValue As Integer = getCharOrder(ch)

				If mapValue <> RBCollationTables.UNMAPPED Then
					valueList(j) = mapValue
					j += 1
				Else
					' can't find it in the table, will be filled in by commit().
					valueList(j) = CHARINDEX + ch
					j += 1
				End If
			Next i
			If j < valueList.Length Then
				'we had at least one supplementary character, the size of valueList
				'is bigger than it really needs...
				Dim tmpBuf As Integer() = New Integer(j - 1){}
				j -= 1
				Do While j >= 0
					tmpBuf(j) = valueList(j)
					j -= 1
				Loop
				valueList = tmpBuf
			End If
			' Add the expanding char list into the expansion table.
			Dim tableIndex As Integer = RBCollationTables.EXPANDCHARINDEX + expandTable.Count
			expandTable.Add(valueList)

			Return tableIndex
		End Function

		Private Sub addContractFlags(  chars As String)
			Dim c0 As Char
			Dim c As Integer
			Dim len As Integer = chars.length()
			For i As Integer = 0 To len - 1
				c0 = chars.Chars(i)
				i += 1
				c = If(Char.IsHighSurrogate(c0), Character.toCodePoint(c0, chars.Chars(i)), c0)
				contractFlags.put(c, 1)
			Next i
		End Sub

		' ==============================================================
		' constants
		' ==============================================================
		Friend Const CHARINDEX As Integer = &H70000000 ' need look up in .commit()

		Private Const IGNORABLEMASK As Integer = &Hffff
		Private Const PRIMARYORDERINCREMENT As Integer = &H10000
		Private Const SECONDARYORDERINCREMENT As Integer = &H100
		Private Const TERTIARYORDERINCREMENT As Integer = &H1
		Private Const INITIALTABLESIZE As Integer = 20
		Private Const MAXKEYSIZE As Integer = 5

		' ==============================================================
		' instance variables
		' ==============================================================

		' variables used by the build process
		Private tables As RBCollationTables.BuildAPI = Nothing
		Private mPattern As MergeCollation = Nothing
		Private isOverIgnore As Boolean = False
		Private keyBuf As Char() = New Char(MAXKEYSIZE - 1){}
		Private contractFlags As New sun.text.IntHashtable(100)

		' "shadow" copies of the instance variables in RBCollationTables
		' (the values in these variables are copied back into RBCollationTables
		' at the end of the build process)
		Private frenchSec As Boolean = False
		Private seAsianSwapping As Boolean = False

		Private mapping As sun.text.UCompactIntArray = Nothing
		Private contractTable As List(Of List(Of EntryPair)) = Nothing
		Private expandTable As List(Of Integer()) = Nothing

		Private maxSecOrder As Short = 0
		Private maxTerOrder As Short = 0
	End Class

End Namespace