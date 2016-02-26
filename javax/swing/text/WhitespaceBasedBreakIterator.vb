Imports System

'
' * Copyright (c) 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' A simple whitespace-based BreakIterator implementation.
	''' 
	''' @author Sergey Groznyh
	''' </summary>
	Friend Class WhitespaceBasedBreakIterator
		Inherits java.text.BreakIterator

		Private text As Char() = New Char(){}
		Private breaks As Integer() = { 0 }
		Private pos As Integer = 0

		''' <summary>
		''' Calculate break positions eagerly parallel to reading text.
		''' </summary>
		Public Overridable Property text As java.text.CharacterIterator
			Set(ByVal ci As java.text.CharacterIterator)
				Dim begin As Integer = ci.beginIndex
				text = New Char(ci.endIndex - begin - 1){}
				Dim breaks0 As Integer() = New Integer(text.Length){}
				Dim brIx As Integer = 0
				breaks0(brIx) = begin
				brIx += 1
    
				Dim charIx As Integer = 0
				Dim inWs As Boolean = False
				Dim c As Char = ci.first()
				Do While c <> java.text.CharacterIterator.DONE
					text(charIx) = c
					Dim ws As Boolean = Char.IsWhiteSpace(c)
					If inWs AndAlso (Not ws) Then
						breaks0(brIx) = charIx + begin
						brIx += 1
					End If
					inWs = ws
					charIx += 1
					c = ci.next()
				Loop
				If text.Length > 0 Then
					breaks0(brIx) = text.Length + begin
					brIx += 1
				End If
	'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Array.Copy(breaks0, 0, breaks = New Integer(brIx - 1){}, 0, brIx)
			End Set
			Get
				Return New java.text.StringCharacterIterator(New String(text))
			End Get
		End Property


		Public Overridable Function first() As Integer
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Return breaks(pos = 0)
		End Function

		Public Overridable Function last() As Integer
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Return breaks(pos = breaks.Length - 1)
		End Function

		Public Overridable Function current() As Integer
			Return breaks(pos)
		End Function

		Public Overridable Function [next]() As Integer
				pos += 1
				Return (If(pos = breaks.Length - 1, DONE, breaks(pos)))
		End Function

		Public Overridable Function previous() As Integer
				pos -= 1
				Return (If(pos = 0, DONE, breaks(pos)))
		End Function

		Public Overridable Function [next](ByVal n As Integer) As Integer
			Return checkhit(pos + n)
		End Function

		Public Overridable Function following(ByVal n As Integer) As Integer
			Return adjacent(n, 1)
		End Function

		Public Overridable Function preceding(ByVal n As Integer) As Integer
			Return adjacent(n, -1)
		End Function

		Private Function checkhit(ByVal hit As Integer) As Integer
			If (hit < 0) OrElse (hit >= breaks.Length) Then
				Return DONE
			Else
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return breaks(pos = hit)
			End If
		End Function

		Private Function adjacent(ByVal n As Integer, ByVal bias As Integer) As Integer
			Dim hit As Integer = java.util.Arrays.binarySearch(breaks, n)
			Dim offset As Integer = (If(hit < 0, (If(bias < 0, -1, -2)), 0))
			Return checkhit(Math.Abs(hit) + bias + offset)
		End Function
	End Class

End Namespace