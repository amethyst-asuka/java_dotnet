'
' * Copyright (c) 1999, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt.font


	Friend Class CharArrayIterator
		Implements java.text.CharacterIterator

		Private chars As Char()
		Private pos As Integer
		Private begin As Integer

		Friend Sub New(  chars As Char())

			reset(chars, 0)
		End Sub

		Friend Sub New(  chars As Char(),   begin As Integer)

			reset(chars, begin)
		End Sub

		''' <summary>
		''' Sets the position to getBeginIndex() and returns the character at that
		''' position. </summary>
		''' <returns> the first character in the text, or DONE if the text is empty </returns>
		''' <seealso cref= getBeginIndex </seealso>
		Public Overridable Function first() As Char

			pos = 0
			Return current()
		End Function

		''' <summary>
		''' Sets the position to getEndIndex()-1 (getEndIndex() if the text is empty)
		''' and returns the character at that position. </summary>
		''' <returns> the last character in the text, or DONE if the text is empty </returns>
		''' <seealso cref= getEndIndex </seealso>
		Public Overridable Function last() As Char

			If chars.Length > 0 Then
				pos = chars.Length-1
			Else
				pos = 0
			End If
			Return current()
		End Function

		''' <summary>
		''' Gets the character at the current position (as returned by getIndex()). </summary>
		''' <returns> the character at the current position or DONE if the current
		''' position is off the end of the text. </returns>
		''' <seealso cref= getIndex </seealso>
		Public Overridable Function current() As Char

			If pos >= 0 AndAlso pos < chars.Length Then
				Return chars(pos)
			Else
				Return DONE
			End If
		End Function

		''' <summary>
		''' Increments the iterator's index by one and returns the character
		''' at the new index.  If the resulting index is greater or equal
		''' to getEndIndex(), the current index is reset to getEndIndex() and
		''' a value of DONE is returned. </summary>
		''' <returns> the character at the new position or DONE if the new
		''' position is off the end of the text range. </returns>
		Public Overridable Function [next]() As Char

			If pos < chars.Length-1 Then
				pos += 1
				Return chars(pos)
			Else
				pos = chars.Length
				Return DONE
			End If
		End Function

		''' <summary>
		''' Decrements the iterator's index by one and returns the character
		''' at the new index. If the current index is getBeginIndex(), the index
		''' remains at getBeginIndex() and a value of DONE is returned. </summary>
		''' <returns> the character at the new position or DONE if the current
		''' position is equal to getBeginIndex(). </returns>
		Public Overridable Function previous() As Char

			If pos > 0 Then
				pos -= 1
				Return chars(pos)
			Else
				pos = 0
				Return DONE
			End If
		End Function

		''' <summary>
		''' Sets the position to the specified position in the text and returns that
		''' character. </summary>
		''' <param name="position"> the position within the text.  Valid values range from
		''' getBeginIndex() to getEndIndex().  An IllegalArgumentException is thrown
		''' if an invalid value is supplied. </param>
		''' <returns> the character at the specified position or DONE if the specified position is equal to getEndIndex() </returns>
		Public Overridable Function setIndex(  position As Integer) As Char

			position -= begin
			If position < 0 OrElse position > chars.Length Then Throw New IllegalArgumentException("Invalid index")
			pos = position
			Return current()
		End Function

		''' <summary>
		''' Returns the start index of the text. </summary>
		''' <returns> the index at which the text begins. </returns>
		Public Overridable Property beginIndex As Integer
			Get
				Return begin
			End Get
		End Property

		''' <summary>
		''' Returns the end index of the text.  This index is the index of the first
		''' character following the end of the text. </summary>
		''' <returns> the index after the last character in the text </returns>
		Public Overridable Property endIndex As Integer
			Get
				Return begin+chars.Length
			End Get
		End Property

		''' <summary>
		''' Returns the current index. </summary>
		''' <returns> the current index. </returns>
		Public Overridable Property index As Integer
			Get
				Return begin+pos
			End Get
		End Property

		''' <summary>
		''' Create a copy of this iterator </summary>
		''' <returns> A copy of this </returns>
		Public Overridable Function clone() As Object
			Dim c As New CharArrayIterator(chars, begin)
			c.pos = Me.pos
			Return c
		End Function

		Friend Overridable Sub reset(  chars As Char())
			reset(chars, 0)
		End Sub

		Friend Overridable Sub reset(  chars As Char(),   begin As Integer)

			Me.chars = chars
			Me.begin = begin
			pos = 0
		End Sub
	End Class

End Namespace