Imports Microsoft.VisualBasic
Imports System

'
' * Copyright (c) 1996, 2000, Oracle and/or its affiliates. All rights reserved.
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
	''' This is to be used with MergeCollation for adding patterns to an
	''' existing rule table. </summary>
	''' <seealso cref=        MergeCollation
	''' @author     Mark Davis, Helena Shih </seealso>

	Friend Class PatternEntry
		''' <summary>
		''' Gets the current extension, quoted
		''' </summary>
		Public Overridable Sub appendQuotedExtension(  toAddTo As StringBuffer)
			appendQuoted(extension,toAddTo)
		End Sub

		''' <summary>
		''' Gets the current chars, quoted
		''' </summary>
		Public Overridable Sub appendQuotedChars(  toAddTo As StringBuffer)
			appendQuoted(chars,toAddTo)
		End Sub

		''' <summary>
		''' WARNING this is used for searching in a Vector.
		''' Because Vector.indexOf doesn't take a comparator,
		''' this method is ill-defined and ignores strength.
		''' </summary>
		Public Overrides Function Equals(  obj As Object) As Boolean
			If obj Is Nothing Then Return False
			Dim other As PatternEntry = CType(obj, PatternEntry)
			Dim result As Boolean = chars.Equals(other.chars)
			Return result
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return chars.GetHashCode()
		End Function

		''' <summary>
		''' For debugging.
		''' </summary>
		Public Overrides Function ToString() As String
			Dim result As New StringBuffer
			addToBuffer(result, True, False, Nothing)
			Return result.ToString()
		End Function

		''' <summary>
		''' Gets the strength of the entry.
		''' </summary>
		Friend Property strength As Integer
			Get
				Return strength
			End Get
		End Property

		''' <summary>
		''' Gets the expanding characters of the entry.
		''' </summary>
		Friend Property extension As String
			Get
				Return extension
			End Get
		End Property

		''' <summary>
		''' Gets the core characters of the entry.
		''' </summary>
		Friend Property chars As String
			Get
				Return chars
			End Get
		End Property

		' ===== privates =====

		Friend Overridable Sub addToBuffer(  toAddTo As StringBuffer,   showExtension As Boolean,   showWhiteSpace As Boolean,   lastEntry As PatternEntry)
			If showWhiteSpace AndAlso toAddTo.length() > 0 Then
				If strength = Collator.PRIMARY OrElse lastEntry IsNot Nothing Then
					toAddTo.append(ControlChars.Lf)
				Else
					toAddTo.append(" "c)
				End If
			End If
			If lastEntry IsNot Nothing Then
				toAddTo.append("&"c)
				If showWhiteSpace Then toAddTo.append(" "c)
				lastEntry.appendQuotedChars(toAddTo)
				appendQuotedExtension(toAddTo)
				If showWhiteSpace Then toAddTo.append(" "c)
			End If
			Select Case strength
			Case Collator.IDENTICAL
				toAddTo.append("="c)
			Case Collator.TERTIARY
				toAddTo.append(","c)
			Case Collator.SECONDARY
				toAddTo.append(";"c)
			Case Collator.PRIMARY
				toAddTo.append("<"c)
			Case RESET
				toAddTo.append("&"c)
			Case UNSET
				toAddTo.append("?"c)
			End Select
			If showWhiteSpace Then toAddTo.append(" "c)
			appendQuoted(chars,toAddTo)
			If showExtension AndAlso extension.length() <> 0 Then
				toAddTo.append("/"c)
				appendQuoted(extension,toAddTo)
			End If
		End Sub

		Friend Shared Sub appendQuoted(  chars As String,   toAddTo As StringBuffer)
			Dim inQuote As Boolean = False
			Dim ch As Char = chars.Chars(0)
			If Character.isSpaceChar(ch) Then
				inQuote = True
				toAddTo.append("'"c)
			Else
			  If PatternEntry.isSpecialChar(ch) Then
					inQuote = True
					toAddTo.append("'"c)
				Else
					Select Case ch
						Case &H10, ControlChars.FormFeed, ControlChars.Cr, ControlChars.Tab, ControlChars.Lf, "@"c
						inQuote = True
						toAddTo.append("'"c)
					Case "'"c
						inQuote = True
						toAddTo.append("'"c)
					Case Else
						If inQuote Then
							inQuote = False
							toAddTo.append("'"c)
						End If
					End Select
				End If
			End If
			toAddTo.append(chars)
			If inQuote Then toAddTo.append("'"c)
		End Sub

		'========================================================================
		' Parsing a pattern into a list of PatternEntries....
		'========================================================================

		Friend Sub New(  strength As Integer,   chars As StringBuffer,   extension As StringBuffer)
			Me.strength = strength
			Me.chars = chars.ToString()
			Me.extension = If(extension.length() > 0, extension.ToString(), "")
		End Sub

		Friend Class Parser
			Private pattern As String
			Private i As Integer

			Public Sub New(  pattern As String)
				Me.pattern = pattern
				Me.i = 0
			End Sub

			Public Overridable Function [next]() As PatternEntry
				Dim newStrength As Integer = UNSET

				newChars.length = 0
				newExtension.length = 0

				Dim inChars As Boolean = True
				Dim inQuote As Boolean = False
			mainLoop:
				Do While i < pattern.length()
					Dim ch As Char = pattern.Chars(i)
					If inQuote Then
						If ch = "'"c Then
							inQuote = False
						Else
							If newChars.length() = 0 Then
								newChars.append(ch)
							ElseIf inChars Then
								newChars.append(ch)
							Else
								newExtension.append(ch)
							End If
						End If
					Else
						Select Case ch
					Case "="c
						If newStrength <> UNSET Then GoTo mainLoop
						newStrength = Collator.IDENTICAL
					Case ","c
						If newStrength <> UNSET Then GoTo mainLoop
						newStrength = Collator.TERTIARY
					Case ";"c
						If newStrength <> UNSET Then GoTo mainLoop
						newStrength = Collator.SECONDARY
					Case "<"c
						If newStrength <> UNSET Then GoTo mainLoop
						newStrength = Collator.PRIMARY
					Case "&"c
						If newStrength <> UNSET Then GoTo mainLoop
						newStrength = RESET
					Case ControlChars.Tab, ControlChars.Lf, ControlChars.FormFeed, ControlChars.Cr, " "c
					Case "/"c
						inChars = False
					Case "'"c
						inQuote = True
						i += 1
						ch = pattern.Chars(i)
						If newChars.length() = 0 Then
							newChars.append(ch)
						ElseIf inChars Then
							newChars.append(ch)
						Else
							newExtension.append(ch)
						End If
					Case Else
						If newStrength = UNSET Then Throw New ParseException("missing char (=,;<&) : " & pattern.Substring(i,If(10 < pattern.length(), i+10, pattern.length())), i)
						If PatternEntry.isSpecialChar(ch) AndAlso (inQuote = False) Then Throw New ParseException("Unquoted punctuation character : " & Convert.ToString(ch, 16), i)
						If inChars Then
							newChars.append(ch)
						Else
							newExtension.append(ch)
						End If
						End Select
					End If
					i += 1
				Loop
				If newStrength = UNSET Then Return Nothing
				If newChars.length() = 0 Then Throw New ParseException("missing chars (=,;<&): " & pattern.Substring(i,If(10 < pattern.length(), i+10, pattern.length())), i)

				Return New PatternEntry(newStrength, newChars, newExtension)
			End Function

			' We re-use these objects in order to improve performance
			Private newChars As New StringBuffer
			Private newExtension As New StringBuffer

		End Class

		Friend Shared Function isSpecialChar(  ch As Char) As Boolean
			Return ((ch = ChrW(&H0020)) OrElse ((ch <= ChrW(&H002F)) AndAlso (ch >= ChrW(&H0022))) OrElse ((ch <= ChrW(&H003F)) AndAlso (ch >= ChrW(&H003A))) OrElse ((ch <= ChrW(&H0060)) AndAlso (ch >= ChrW(&H005B))) OrElse ((ch <= ChrW(&H007E)) AndAlso (ch >= ChrW(&H007B))))
		End Function


		Friend Const RESET As Integer = -2
		Friend Const UNSET As Integer = -1

		Friend strength As Integer = UNSET
		Friend chars As String = ""
		Friend extension As String = ""
	End Class

End Namespace