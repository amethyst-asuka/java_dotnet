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
' * (C) Copyright IBM Corp. 1996 - 1998 - All Rights Reserved
' *
' * The original version of this source code and documentation
' * is copyrighted and owned by Taligent, Inc., a wholly-owned
' * subsidiary of IBM. These materials are provided under terms
' * of a License Agreement between Taligent and Sun. This technology
' * is protected by multiple US and International patents.
' *
' * This notice and attribution to Taligent may not be removed.
' * Taligent is a registered trademark of Taligent, Inc.
' *
' 

Namespace java.text

	''' <summary>
	''' <code>StringCharacterIterator</code> implements the
	''' <code>CharacterIterator</code> protocol for a <code>String</code>.
	''' The <code>StringCharacterIterator</code> class iterates over the
	''' entire <code>String</code>.
	''' </summary>
	''' <seealso cref= CharacterIterator </seealso>

	Public NotInheritable Class StringCharacterIterator
		Implements CharacterIterator

		Private text As String
		Private begin As Integer
		Private [end] As Integer
		' invariant: begin <= pos <= end
		Private pos As Integer

		''' <summary>
		''' Constructs an iterator with an initial index of 0.
		''' </summary>
		''' <param name="text"> the {@code String} to be iterated over </param>
		Public Sub New(  text As String)
			Me.New(text, 0)
		End Sub

		''' <summary>
		''' Constructs an iterator with the specified initial index.
		''' </summary>
		''' <param name="text">   The String to be iterated over </param>
		''' <param name="pos">    Initial iterator position </param>
		Public Sub New(  text As String,   pos As Integer)
		Me.New(text, 0, text.length(), pos)
		End Sub

		''' <summary>
		''' Constructs an iterator over the given range of the given string, with the
		''' index set at the specified position.
		''' </summary>
		''' <param name="text">   The String to be iterated over </param>
		''' <param name="begin">  Index of the first character </param>
		''' <param name="end">    Index of the character following the last character </param>
		''' <param name="pos">    Initial iterator position </param>
		Public Sub New(  text As String,   begin As Integer,   [end] As Integer,   pos As Integer)
			If text Is Nothing Then Throw New NullPointerException
			Me.text = text

			If begin < 0 OrElse begin > [end] OrElse [end] > text.length() Then Throw New IllegalArgumentException("Invalid substring range")

			If pos < begin OrElse pos > [end] Then Throw New IllegalArgumentException("Invalid position")

			Me.begin = begin
			Me.end = [end]
			Me.pos = pos
		End Sub

		''' <summary>
		''' Reset this iterator to point to a new string.  This package-visible
		''' method is used by other java.text classes that want to avoid allocating
		''' new StringCharacterIterator objects every time their setText method
		''' is called.
		''' </summary>
		''' <param name="text">   The String to be iterated over
		''' @since 1.2 </param>
		Public Property text As String
			Set(  text As String)
				If text Is Nothing Then Throw New NullPointerException
				Me.text = text
				Me.begin = 0
				Me.end = text.length()
				Me.pos = 0
			End Set
		End Property

		''' <summary>
		''' Implements CharacterIterator.first() for String. </summary>
		''' <seealso cref= CharacterIterator#first </seealso>
		Public Function first() As Char Implements CharacterIterator.first
			pos = begin
			Return current()
		End Function

		''' <summary>
		''' Implements CharacterIterator.last() for String. </summary>
		''' <seealso cref= CharacterIterator#last </seealso>
		Public Function last() As Char Implements CharacterIterator.last
			If [end] <> begin Then
				pos = [end] - 1
			Else
				pos = [end]
			End If
			Return current()
		End Function

		''' <summary>
		''' Implements CharacterIterator.setIndex() for String. </summary>
		''' <seealso cref= CharacterIterator#setIndex </seealso>
		Public Function setIndex(  p As Integer) As Char Implements CharacterIterator.setIndex
		If p < begin OrElse p > [end] Then Throw New IllegalArgumentException("Invalid index")
			pos = p
			Return current()
		End Function

		''' <summary>
		''' Implements CharacterIterator.current() for String. </summary>
		''' <seealso cref= CharacterIterator#current </seealso>
		Public Function current() As Char Implements CharacterIterator.current
			If pos >= begin AndAlso pos < [end] Then
				Return text.Chars(pos)
			Else
				Return DONE
			End If
		End Function

		''' <summary>
		''' Implements CharacterIterator.next() for String. </summary>
		''' <seealso cref= CharacterIterator#next </seealso>
		Public Function [next]() As Char Implements CharacterIterator.next
			If pos < [end] - 1 Then
				pos += 1
				Return text.Chars(pos)
			Else
				pos = [end]
				Return DONE
			End If
		End Function

		''' <summary>
		''' Implements CharacterIterator.previous() for String. </summary>
		''' <seealso cref= CharacterIterator#previous </seealso>
		Public Function previous() As Char Implements CharacterIterator.previous
			If pos > begin Then
				pos -= 1
				Return text.Chars(pos)
			Else
				Return DONE
			End If
		End Function

		''' <summary>
		''' Implements CharacterIterator.getBeginIndex() for String. </summary>
		''' <seealso cref= CharacterIterator#getBeginIndex </seealso>
		Public Property beginIndex As Integer Implements CharacterIterator.getBeginIndex
			Get
				Return begin
			End Get
		End Property

		''' <summary>
		''' Implements CharacterIterator.getEndIndex() for String. </summary>
		''' <seealso cref= CharacterIterator#getEndIndex </seealso>
		Public Property endIndex As Integer Implements CharacterIterator.getEndIndex
			Get
				Return [end]
			End Get
		End Property

		''' <summary>
		''' Implements CharacterIterator.getIndex() for String. </summary>
		''' <seealso cref= CharacterIterator#getIndex </seealso>
		Public Property index As Integer Implements CharacterIterator.getIndex
			Get
				Return pos
			End Get
		End Property

		''' <summary>
		''' Compares the equality of two StringCharacterIterator objects. </summary>
		''' <param name="obj"> the StringCharacterIterator object to be compared with. </param>
		''' <returns> true if the given obj is the same as this
		''' StringCharacterIterator object; false otherwise. </returns>
		Public Overrides Function Equals(  obj As Object) As Boolean
			If Me Is obj Then Return True
			If Not(TypeOf obj Is StringCharacterIterator) Then Return False

			Dim that As StringCharacterIterator = CType(obj, StringCharacterIterator)

			If GetHashCode() <> that.GetHashCode() Then Return False
			If Not text.Equals(that.text) Then Return False
			If pos <> that.pos OrElse begin <> that.begin OrElse [end] <> that.end Then Return False
			Return True
		End Function

		''' <summary>
		''' Computes a hashcode for this iterator. </summary>
		''' <returns> A hash code </returns>
		Public Overrides Function GetHashCode() As Integer
			Return text.GetHashCode() Xor pos Xor begin Xor [end]
		End Function

		''' <summary>
		''' Creates a copy of this iterator. </summary>
		''' <returns> A copy of this </returns>
		Public Function clone() As Object Implements CharacterIterator.clone
			Try
				Dim other As StringCharacterIterator = CType(MyBase.clone(), StringCharacterIterator)
				Return other
			Catch e As CloneNotSupportedException
				Throw New InternalError(e)
			End Try
		End Function

	End Class

End Namespace