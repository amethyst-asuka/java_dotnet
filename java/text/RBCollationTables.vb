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
	''' This class contains the static state of a RuleBasedCollator: The various
	''' tables that are used by the collation routines.  Several RuleBasedCollators
	''' can share a single RBCollationTables object, easing memory requirements and
	''' improving performance.
	''' </summary>
	Friend NotInheritable Class RBCollationTables
		'===========================================================================================
		'  The following diagram shows the data structure of the RBCollationTables object.
		'  Suppose we have the rule, where 'o-umlaut' is the unicode char 0x00F6.
		'  "a, A < b, B < c, C, ch, cH, Ch, CH < d, D ... < o, O; 'o-umlaut'/E, 'O-umlaut'/E ...".
		'  What the rule says is, sorts 'ch'ligatures and 'c' only with tertiary difference and
		'  sorts 'o-umlaut' as if it's always expanded with 'e'.
		'
		' mapping table                     contracting list           expanding list
		' (contains all unicode char
		'  entries)                   ___    ____________       _________________________
		'  ________                +>|_*_|->|'c' |v('c') |  +>|v('o')|v('umlaut')|v('e')|
		' |_\u0001_|-> v('\u0001') | |_:_|  |------------|  | |-------------------------|
		' |_\u0002_|-> v('\u0002') | |_:_|  |'ch'|v('ch')|  | |             :           |
		' |____:___|               | |_:_|  |------------|  | |-------------------------|
		' |____:___|               |        |'cH'|v('cH')|  | |             :           |
		' |__'a'___|-> v('a')      |        |------------|  | |-------------------------|
		' |__'b'___|-> v('b')      |        |'Ch'|v('Ch')|  | |             :           |
		' |____:___|               |        |------------|  | |-------------------------|
		' |____:___|               |        |'CH'|v('CH')|  | |             :           |
		' |___'c'__|----------------         ------------   | |-------------------------|
		' |____:___|                                        | |             :           |
		' |o-umlaut|----------------------------------------  |_________________________|
		' |____:___|
		'
		' Noted by Helena Shih on 6/23/97
		'============================================================================================

		Public Sub New(ByVal rules As String, ByVal decmp As Integer)
			Me.rules = rules

			Dim builder As New RBTableBuilder(New BuildAPI(Me))
			builder.build(rules, decmp) ' this object is filled in through
												' the BuildAPI object
		End Sub

		Friend NotInheritable Class BuildAPI
			Private ReadOnly outerInstance As RBCollationTables

			''' <summary>
			''' Private constructor.  Prevents anyone else besides RBTableBuilder
			''' from gaining direct access to the internals of this class.
			''' </summary>
			Private Sub New(ByVal outerInstance As RBCollationTables)
					Me.outerInstance = outerInstance
			End Sub

			''' <summary>
			''' This function is used by RBTableBuilder to fill in all the members of this
			''' object.  (Effectively, the builder class functions as a "friend" of this
			''' class, but to avoid changing too much of the logic, it carries around "shadow"
			''' copies of all these variables until the end of the build process and then
			''' copies them en masse into the actual tables object once all the construction
			''' logic is complete.  This function does that "copying en masse". </summary>
			''' <param name="f2ary"> The value for frenchSec (the French-secondary flag) </param>
			''' <param name="swap"> The value for SE Asian swapping rule </param>
			''' <param name="map"> The collator's character-mapping table (the value for mapping) </param>
			''' <param name="cTbl"> The collator's contracting-character table (the value for contractTable) </param>
			''' <param name="eTbl"> The collator's expanding-character table (the value for expandTable) </param>
			''' <param name="cFlgs"> The hash table of characters that participate in contracting-
			'''              character sequences (the value for contractFlags) </param>
			''' <param name="mso"> The value for maxSecOrder </param>
			''' <param name="mto"> The value for maxTerOrder </param>
			Friend Sub fillInTables(ByVal f2ary As Boolean, ByVal swap As Boolean, ByVal map As sun.text.UCompactIntArray, ByVal cTbl As List(Of List(Of EntryPair)), ByVal eTbl As List(Of Integer()), ByVal cFlgs As sun.text.IntHashtable, ByVal mso As Short, ByVal mto As Short)
				outerInstance.frenchSec = f2ary
				outerInstance.seAsianSwapping = swap
				outerInstance.mapping = map
				outerInstance.contractTable = cTbl
				outerInstance.expandTable = eTbl
				outerInstance.contractFlags = cFlgs
				outerInstance.maxSecOrder = mso
				outerInstance.maxTerOrder = mto
			End Sub
		End Class

		''' <summary>
		''' Gets the table-based rules for the collation object. </summary>
		''' <returns> returns the collation rules that the table collation object
		''' was created from. </returns>
		Public Property rules As String
			Get
				Return rules
			End Get
		End Property

		Public Property frenchSec As Boolean
			Get
				Return frenchSec
			End Get
		End Property

		Public Property sEAsianSwapping As Boolean
			Get
				Return seAsianSwapping
			End Get
		End Property

		' ==============================================================
		' internal (for use by CollationElementIterator)
		' ==============================================================

		''' <summary>
		'''  Get the entry of hash table of the contracting string in the collation
		'''  table. </summary>
		'''  <param name="ch"> the starting character of the contracting string </param>
		Friend Function getContractValues(ByVal ch As Integer) As List(Of EntryPair)
			Dim index As Integer = mapping.elementAt(ch)
			Return getContractValuesImpl(index - CONTRACTCHARINDEX)
		End Function

		'get contract values from contractTable by index
		Private Function getContractValuesImpl(ByVal index As Integer) As List(Of EntryPair)
			If index >= 0 Then
				Return contractTable(index)
			Else ' not found
				Return Nothing
			End If
		End Function

		''' <summary>
		''' Returns true if this character appears anywhere in a contracting
		''' character sequence.  (Used by CollationElementIterator.setOffset().)
		''' </summary>
		Friend Function usedInContractSeq(ByVal c As Integer) As Boolean
			Return contractFlags.get(c) = 1
		End Function

		''' <summary>
		''' Return the maximum length of any expansion sequences that end
		''' with the specified comparison order.
		''' </summary>
		''' <param name="order"> a collation order returned by previous or next. </param>
		''' <returns> the maximum length of any expansion seuences ending
		'''         with the specified order.
		''' </returns>
		''' <seealso cref= CollationElementIterator#getMaxExpansion </seealso>
		Friend Function getMaxExpansion(ByVal order As Integer) As Integer
			Dim result As Integer = 1

			If expandTable IsNot Nothing Then
				' Right now this does a linear search through the entire
				' expansion table.  If a collator had a large number of expansions,
				' this could cause a performance problem, but in practise that
				' rarely happens
				For i As Integer = 0 To expandTable.Count - 1
					Dim valueList As Integer() = expandTable(i)
					Dim length As Integer = valueList.Length

					If length > result AndAlso valueList(length-1) = order Then result = length
				Next i
			End If

			Return result
		End Function

		''' <summary>
		''' Get the entry of hash table of the expanding string in the collation
		''' table. </summary>
		''' <param name="idx"> the index of the expanding string value list </param>
		Friend Function getExpandValueList(ByVal idx As Integer) As Integer()
			Return expandTable(idx - EXPANDCHARINDEX)
		End Function

		''' <summary>
		''' Get the comarison order of a character from the collation table. </summary>
		''' <returns> the comparison order of a character. </returns>
		Friend Function getUnicodeOrder(ByVal ch As Integer) As Integer
			Return mapping.elementAt(ch)
		End Function

		Friend Property maxSecOrder As Short
			Get
				Return maxSecOrder
			End Get
		End Property

		Friend Property maxTerOrder As Short
			Get
				Return maxTerOrder
			End Get
		End Property

		''' <summary>
		''' Reverse a string.
		''' </summary>
		'shemran/Note: this is used for secondary order value reverse, no
		'              need to consider supplementary pair.
		Friend Shared Sub reverse(ByVal result As StringBuffer, ByVal [from] As Integer, ByVal [to] As Integer)
			Dim i As Integer = [from]
			Dim swap As Char

			Dim j As Integer = [to] - 1
			Do While i < j
				swap = result.Chars(i)
				result.charAtrAt(i, result.Chars(j))
				result.charAtrAt(j, swap)
				i += 1
				j -= 1
			Loop
		End Sub

		Friend Shared Function getEntry(ByVal list As List(Of EntryPair), ByVal name As String, ByVal fwd As Boolean) As Integer
			For i As Integer = 0 To list.Count - 1
				Dim pair As EntryPair = list(i)
				If pair.fwd = fwd AndAlso pair.entryName.Equals(name) Then Return i
			Next i
			Return UNMAPPED
		End Function

		' ==============================================================
		' constants
		' ==============================================================
		'sherman/Todo: is the value big enough?????
		Friend Const EXPANDCHARINDEX As Integer = &H7E000000 ' Expand index follows
		Friend Const CONTRACTCHARINDEX As Integer = &H7F000000 ' contract indexes follow
		Friend Const UNMAPPED As Integer = &HFFFFFFFFL

		Friend Const PRIMARYORDERMASK As Integer = &Hffff0000L
		Friend Const SECONDARYORDERMASK As Integer = &Hff00
		Friend Const TERTIARYORDERMASK As Integer = &Hff
		Friend Const PRIMARYDIFFERENCEONLY As Integer = &Hffff0000L
		Friend Const SECONDARYDIFFERENCEONLY As Integer = &Hffffff00L
		Friend Const PRIMARYORDERSHIFT As Integer = 16
		Friend Const SECONDARYORDERSHIFT As Integer = 8

		' ==============================================================
		' instance variables
		' ==============================================================
		Private rules As String = Nothing
		Private frenchSec As Boolean = False
		Private seAsianSwapping As Boolean = False

		Private mapping As sun.text.UCompactIntArray = Nothing
		Private contractTable As List(Of List(Of EntryPair)) = Nothing
		Private expandTable As List(Of Integer()) = Nothing
		Private contractFlags As sun.text.IntHashtable = Nothing

		Private maxSecOrder As Short = 0
		Private maxTerOrder As Short = 0
	End Class

End Namespace