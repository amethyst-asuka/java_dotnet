Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 2000, 2012, Oracle and/or its affiliates. All rights reserved.
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
	''' CharacterIteratorFieldDelegate combines the notifications from a Format
	''' into a resulting <code>AttributedCharacterIterator</code>. The resulting
	''' <code>AttributedCharacterIterator</code> can be retrieved by way of
	''' the <code>getIterator</code> method.
	''' 
	''' </summary>
	Friend Class CharacterIteratorFieldDelegate
		Implements Format.FieldDelegate

		''' <summary>
		''' Array of AttributeStrings. Whenever <code>formatted</code> is invoked
		''' for a region > size, a new instance of AttributedString is added to
		''' attributedStrings. Subsequent invocations of <code>formatted</code>
		''' for existing regions result in invoking addAttribute on the existing
		''' AttributedStrings.
		''' </summary>
		Private attributedStrings As List(Of AttributedString)
		''' <summary>
		''' Running count of the number of characters that have
		''' been encountered.
		''' </summary>
		Private size As Integer


		Friend Sub New()
			attributedStrings = New List(Of )
		End Sub

		Public Overridable Sub formatted(  attr As Format.Field,   value As Object,   start As Integer,   [end] As Integer,   buffer As StringBuffer) Implements Format.FieldDelegate.formatted
			If start <> [end] Then
				If start < size Then
					' Adjust attributes of existing runs
					Dim index As Integer = size
					Dim asIndex As Integer = attributedStrings.Count - 1

					Do While start < index
						Dim [as] As AttributedString = attributedStrings(asIndex)
						asIndex -= 1
						Dim newIndex As Integer = index - [as].length()
						Dim aStart As Integer = System.Math.Max(0, start - newIndex)

						[as].addAttribute(attr, value, aStart, System.Math.Min([end] - start, [as].length() - aStart) + aStart)
						index = newIndex
					Loop
				End If
				If size < start Then
					' Pad attributes
					attributedStrings.Add(New AttributedString(buffer.Substring(size, start - size)))
					size = start
				End If
				If size < [end] Then
					' Add new string
					Dim aStart As Integer = System.Math.Max(start, size)
					Dim string_Renamed As New AttributedString(buffer.Substring(aStart, [end] - aStart))

					string_Renamed.addAttribute(attr, value)
					attributedStrings.Add(string_Renamed)
					size = [end]
				End If
			End If
		End Sub

		Public Overridable Sub formatted(  fieldID As Integer,   attr As Format.Field,   value As Object,   start As Integer,   [end] As Integer,   buffer As StringBuffer) Implements Format.FieldDelegate.formatted
			formatted(attr, value, start, [end], buffer)
		End Sub

		''' <summary>
		''' Returns an <code>AttributedCharacterIterator</code> that can be used
		''' to iterate over the resulting formatted String.
		''' 
		''' @pararm string Result of formatting.
		''' </summary>
		Public Overridable Function getIterator(  [string] As String) As AttributedCharacterIterator
			' Add the last AttributedCharacterIterator if necessary
			' assert(size <= string.length());
			If string_Renamed.length() > size Then
				attributedStrings.Add(New AttributedString(string_Renamed.Substring(size)))
				size = string_Renamed.length()
			End If
			Dim iCount As Integer = attributedStrings.Count
			Dim iterators As AttributedCharacterIterator() = New AttributedCharacterIterator(iCount - 1){}

			For counter As Integer = 0 To iCount - 1
				iterators(counter) = attributedStrings(counter).iterator
			Next counter
			Return (New AttributedString(iterators)).iterator
		End Function
	End Class

End Namespace