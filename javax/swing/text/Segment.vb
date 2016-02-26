Imports System

'
' * Copyright (c) 1997, 2008, Oracle and/or its affiliates. All rights reserved.
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
	''' A segment of a character array representing a fragment
	''' of text.  It should be treated as immutable even though
	''' the array is directly accessible.  This gives fast access
	''' to fragments of text without the overhead of copying
	''' around characters.  This is effectively an unprotected
	''' String.
	''' <p>
	''' The Segment implements the java.text.CharacterIterator
	''' interface to support use with the i18n support without
	''' copying text into a string.
	''' 
	''' @author  Timothy Prinzing
	''' </summary>
	Public Class Segment
		Implements ICloneable, java.text.CharacterIterator, CharSequence

		''' <summary>
		''' This is the array containing the text of
		''' interest.  This array should never be modified;
		''' it is available only for efficiency.
		''' </summary>
		Public array As Char()

		''' <summary>
		''' This is the offset into the array that
		''' the desired text begins.
		''' </summary>
		Public offset As Integer

		''' <summary>
		''' This is the number of array elements that
		''' make up the text of interest.
		''' </summary>
		Public count As Integer

		Private partialReturn As Boolean

		''' <summary>
		''' Creates a new segment.
		''' </summary>
		Public Sub New()
			Me.New(Nothing, 0, 0)
		End Sub

		''' <summary>
		''' Creates a new segment referring to an existing array.
		''' </summary>
		''' <param name="array"> the array to refer to </param>
		''' <param name="offset"> the offset into the array </param>
		''' <param name="count"> the number of characters </param>
		Public Sub New(ByVal array As Char(), ByVal offset As Integer, ByVal count As Integer)
			Me.array = array
			Me.offset = offset
			Me.count = count
			partialReturn = False
		End Sub

		''' <summary>
		''' Flag to indicate that partial returns are valid.  If the flag is true,
		''' an implementation of the interface method Document.getText(position,length,Segment)
		''' should return as much text as possible without making a copy.  The default
		''' state of the flag is false which will cause Document.getText(position,length,Segment)
		''' to provide the same return behavior it always had, which may or may not
		''' make a copy of the text depending upon the request.
		''' </summary>
		''' <param name="p"> whether or not partial returns are valid.
		''' @since 1.4 </param>
		Public Overridable Property partialReturn As Boolean
			Set(ByVal p As Boolean)
				partialReturn = p
			End Set
			Get
				Return partialReturn
			End Get
		End Property


		''' <summary>
		''' Converts a segment into a String.
		''' </summary>
		''' <returns> the string </returns>
		Public Overrides Function ToString() As String
			If array IsNot Nothing Then Return New String(array, offset, count)
			Return ""
		End Function

		' --- CharacterIterator methods -------------------------------------

		''' <summary>
		''' Sets the position to getBeginIndex() and returns the character at that
		''' position. </summary>
		''' <returns> the first character in the text, or DONE if the text is empty </returns>
		''' <seealso cref= #getBeginIndex
		''' @since 1.3 </seealso>
		Public Overridable Function first() As Char
			pos = offset
			If count <> 0 Then Return array(pos)
			Return DONE
		End Function

		''' <summary>
		''' Sets the position to getEndIndex()-1 (getEndIndex() if the text is empty)
		''' and returns the character at that position. </summary>
		''' <returns> the last character in the text, or DONE if the text is empty </returns>
		''' <seealso cref= #getEndIndex
		''' @since 1.3 </seealso>
		Public Overridable Function last() As Char
			pos = offset + count
			If count <> 0 Then
				pos -= 1
				Return array(pos)
			End If
			Return DONE
		End Function

		''' <summary>
		''' Gets the character at the current position (as returned by getIndex()). </summary>
		''' <returns> the character at the current position or DONE if the current
		''' position is off the end of the text. </returns>
		''' <seealso cref= #getIndex
		''' @since 1.3 </seealso>
		Public Overridable Function current() As Char
			If count <> 0 AndAlso pos < offset + count Then Return array(pos)
			Return DONE
		End Function

		''' <summary>
		''' Increments the iterator's index by one and returns the character
		''' at the new index.  If the resulting index is greater or equal
		''' to getEndIndex(), the current index is reset to getEndIndex() and
		''' a value of DONE is returned. </summary>
		''' <returns> the character at the new position or DONE if the new
		''' position is off the end of the text range.
		''' @since 1.3 </returns>
		Public Overridable Function [next]() As Char
			pos += 1
			Dim [end] As Integer = offset + count
			If pos >= [end] Then
				pos = [end]
				Return DONE
			End If
			Return current()
		End Function

		''' <summary>
		''' Decrements the iterator's index by one and returns the character
		''' at the new index. If the current index is getBeginIndex(), the index
		''' remains at getBeginIndex() and a value of DONE is returned. </summary>
		''' <returns> the character at the new position or DONE if the current
		''' position is equal to getBeginIndex().
		''' @since 1.3 </returns>
		Public Overridable Function previous() As Char
			If pos = offset Then Return DONE
			pos -= 1
			Return current()
		End Function

		''' <summary>
		''' Sets the position to the specified position in the text and returns that
		''' character. </summary>
		''' <param name="position"> the position within the text.  Valid values range from
		''' getBeginIndex() to getEndIndex().  An IllegalArgumentException is thrown
		''' if an invalid value is supplied. </param>
		''' <returns> the character at the specified position or DONE if the specified position is equal to getEndIndex()
		''' @since 1.3 </returns>
		Public Overridable Function setIndex(ByVal position As Integer) As Char
			Dim [end] As Integer = offset + count
			If (position < offset) OrElse (position > [end]) Then Throw New System.ArgumentException("bad position: " & position)
			pos = position
			If (pos <> [end]) AndAlso (count <> 0) Then Return array(pos)
			Return DONE
		End Function

		''' <summary>
		''' Returns the start index of the text. </summary>
		''' <returns> the index at which the text begins.
		''' @since 1.3 </returns>
		Public Overridable Property beginIndex As Integer
			Get
				Return offset
			End Get
		End Property

		''' <summary>
		''' Returns the end index of the text.  This index is the index of the first
		''' character following the end of the text. </summary>
		''' <returns> the index after the last character in the text
		''' @since 1.3 </returns>
		Public Overridable Property endIndex As Integer
			Get
				Return offset + count
			End Get
		End Property

		''' <summary>
		''' Returns the current index. </summary>
		''' <returns> the current index.
		''' @since 1.3 </returns>
		Public Overridable Property index As Integer
			Get
				Return pos
			End Get
		End Property

		' --- CharSequence methods -------------------------------------

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.6
		''' </summary>
		Public Overridable Function charAt(ByVal index As Integer) As Char
			If index < 0 OrElse index >= count Then Throw New StringIndexOutOfBoundsException(index)
			Return array(offset + index)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.6
		''' </summary>
		Public Overridable Function length() As Integer
			Return count
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.6
		''' </summary>
		Public Overridable Function subSequence(ByVal start As Integer, ByVal [end] As Integer) As CharSequence
			If start < 0 Then Throw New StringIndexOutOfBoundsException(start)
			If [end] > count Then Throw New StringIndexOutOfBoundsException([end])
			If start > [end] Then Throw New StringIndexOutOfBoundsException([end] - start)
			Dim segment As New Segment
			segment.array = Me.array
			segment.offset = Me.offset + start
			segment.count = [end] - start
			Return segment
		End Function

		''' <summary>
		''' Creates a shallow copy.
		''' </summary>
		''' <returns> the copy </returns>
		Public Overridable Function clone() As Object
			Dim o As Object
			Try
				o = MyBase.clone()
			Catch cnse As CloneNotSupportedException
				o = Nothing
			End Try
			Return o
		End Function

		Private pos As Integer


	End Class

End Namespace