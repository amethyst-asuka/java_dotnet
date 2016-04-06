Imports System

'
' * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang


	''' <summary>
	''' A mutable sequence of characters.  This class provides an API compatible
	''' with {@code StringBuffer}, but with no guarantee of synchronization.
	''' This class is designed for use as a drop-in replacement for
	''' {@code StringBuffer} in places where the string buffer was being
	''' used by a single thread (as is generally the case).   Where possible,
	''' it is recommended that this class be used in preference to
	''' {@code StringBuffer} as it will be faster under most implementations.
	''' 
	''' <p>The principal operations on a {@code StringBuilder} are the
	''' {@code append} and {@code insert} methods, which are
	''' overloaded so as to accept data of any type. Each effectively
	''' converts a given datum to a string and then appends or inserts the
	''' characters of that string to the string builder. The
	''' {@code append} method always adds these characters at the end
	''' of the builder; the {@code insert} method adds the characters at
	''' a specified point.
	''' <p>
	''' For example, if {@code z} refers to a string builder object
	''' whose current contents are "{@code start}", then
	''' the method call {@code z.append("le")} would cause the string
	''' builder to contain "{@code startle}", whereas
	''' {@code z.insert(4, "le")} would alter the string builder to
	''' contain "{@code starlet}".
	''' <p>
	''' In general, if sb refers to an instance of a {@code StringBuilder},
	''' then {@code sb.append(x)} has the same effect as
	''' {@code sb.insert(sb.length(), x)}.
	''' <p>
	''' Every string builder has a capacity. As long as the length of the
	''' character sequence contained in the string builder does not exceed
	''' the capacity, it is not necessary to allocate a new internal
	''' buffer. If the internal buffer overflows, it is automatically made larger.
	''' 
	''' <p>Instances of {@code StringBuilder} are not safe for
	''' use by multiple threads. If such synchronization is required then it is
	''' recommended that <seealso cref="java.lang.StringBuffer"/> be used.
	''' 
	''' <p>Unless otherwise noted, passing a {@code null} argument to a constructor
	''' or method in this class will cause a <seealso cref="NullPointerException"/> to be
	''' thrown.
	''' 
	''' @author      Michael McCloskey </summary>
	''' <seealso cref=         java.lang.StringBuffer </seealso>
	''' <seealso cref=         java.lang.String
	''' @since       1.5 </seealso>
	<Serializable> _
	Public NotInheritable Class StringBuilder
		Inherits AbstractStringBuilder
		Implements CharSequence

		''' <summary>
		''' use serialVersionUID for interoperability </summary>
		Friend Const serialVersionUID As Long = 4383685877147921099L

		''' <summary>
		''' Constructs a string builder with no characters in it and an
		''' initial capacity of 16 characters.
		''' </summary>
		Public Sub New()
			MyBase.New(16)
		End Sub

		''' <summary>
		''' Constructs a string builder with no characters in it and an
		''' initial capacity specified by the {@code capacity} argument.
		''' </summary>
		''' <param name="capacity">  the initial capacity. </param>
		''' <exception cref="NegativeArraySizeException">  if the {@code capacity}
		'''               argument is less than {@code 0}. </exception>
		Public Sub New(  capacity As Integer)
			MyBase.New(capacity)
		End Sub

		''' <summary>
		''' Constructs a string builder initialized to the contents of the
		''' specified string. The initial capacity of the string builder is
		''' {@code 16} plus the length of the string argument.
		''' </summary>
		''' <param name="str">   the initial contents of the buffer. </param>
		Public Sub New(  str As String)
			MyBase.New(str.length() + 16)
			append(str)
		End Sub

		''' <summary>
		''' Constructs a string builder that contains the same characters
		''' as the specified {@code CharSequence}. The initial capacity of
		''' the string builder is {@code 16} plus the length of the
		''' {@code CharSequence} argument.
		''' </summary>
		''' <param name="seq">   the sequence to copy. </param>
		Public Sub New(  seq As CharSequence)
			Me.New(seq.length() + 16)
			append(seq)
		End Sub

		Public Overrides Function append(  obj As Object) As StringBuilder
			Return append(Convert.ToString(obj))
		End Function

		Public Overrides Function append(  str As String) As StringBuilder
			MyBase.append(str)
			Return Me
		End Function

		''' <summary>
		''' Appends the specified {@code StringBuffer} to this sequence.
		''' <p>
		''' The characters of the {@code StringBuffer} argument are appended,
		''' in order, to this sequence, increasing the
		''' length of this sequence by the length of the argument.
		''' If {@code sb} is {@code null}, then the four characters
		''' {@code "null"} are appended to this sequence.
		''' <p>
		''' Let <i>n</i> be the length of this character sequence just prior to
		''' execution of the {@code append} method. Then the character at index
		''' <i>k</i> in the new character sequence is equal to the character at
		''' index <i>k</i> in the old character sequence, if <i>k</i> is less than
		''' <i>n</i>; otherwise, it is equal to the character at index <i>k-n</i>
		''' in the argument {@code sb}.
		''' </summary>
		''' <param name="sb">   the {@code StringBuffer} to append. </param>
		''' <returns>  a reference to this object. </returns>
		Public Overrides Function append(  sb As StringBuffer) As StringBuilder
			MyBase.append(sb)
			Return Me
		End Function

		Public Overrides Function append(  s As CharSequence) As StringBuilder
			MyBase.append(s)
			Return Me
		End Function

		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overrides Function append(  s As CharSequence,   start As Integer,   [end] As Integer) As StringBuilder
			MyBase.append(s, start, [end])
			Return Me
		End Function

		Public Overrides Function append(  str As Char()) As StringBuilder
			MyBase.append(str)
			Return Me
		End Function

		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overrides Function append(  str As Char(),   offset As Integer,   len As Integer) As StringBuilder
			MyBase.append(str, offset, len)
			Return Me
		End Function

		Public Overrides Function append(  b As Boolean) As StringBuilder
			MyBase.append(b)
			Return Me
		End Function

		Public Overrides Function append(  c As Char) As StringBuilder
			MyBase.append(c)
			Return Me
		End Function

		Public Overrides Function append(  i As Integer) As StringBuilder
			MyBase.append(i)
			Return Me
		End Function

		Public Overrides Function append(  lng As Long) As StringBuilder
			MyBase.append(lng)
			Return Me
		End Function

		Public Overrides Function append(  f As Single) As StringBuilder
			MyBase.append(f)
			Return Me
		End Function

		Public Overrides Function append(  d As Double) As StringBuilder
			MyBase.append(d)
			Return Me
		End Function

		''' <summary>
		''' @since 1.5
		''' </summary>
		Public Overrides Function appendCodePoint(  codePoint As Integer) As StringBuilder
			MyBase.appendCodePoint(codePoint)
			Return Me
		End Function

		''' <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overrides Function delete(  start As Integer,   [end] As Integer) As StringBuilder
			MyBase.delete(start, [end])
			Return Me
		End Function

		''' <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overrides Function deleteCharAt(  index As Integer) As StringBuilder
			MyBase.deleteCharAt(index)
			Return Me
		End Function

		''' <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overrides Function replace(  start As Integer,   [end] As Integer,   str As String) As StringBuilder
			MyBase.replace(start, [end], str)
			Return Me
		End Function

		''' <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overrides Function insert(  index As Integer,   str As Char(),   offset As Integer,   len As Integer) As StringBuilder
			MyBase.insert(index, str, offset, len)
			Return Me
		End Function

		''' <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overrides Function insert(  offset As Integer,   obj As Object) As StringBuilder
				MyBase.insert(offset, obj)
				Return Me
		End Function

		''' <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overrides Function insert(  offset As Integer,   str As String) As StringBuilder
			MyBase.insert(offset, str)
			Return Me
		End Function

		''' <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overrides Function insert(  offset As Integer,   str As Char()) As StringBuilder
			MyBase.insert(offset, str)
			Return Me
		End Function

		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overrides Function insert(  dstOffset As Integer,   s As CharSequence) As StringBuilder
				MyBase.insert(dstOffset, s)
				Return Me
		End Function

		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overrides Function insert(  dstOffset As Integer,   s As CharSequence,   start As Integer,   [end] As Integer) As StringBuilder
			MyBase.insert(dstOffset, s, start, [end])
			Return Me
		End Function

		''' <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overrides Function insert(  offset As Integer,   b As Boolean) As StringBuilder
			MyBase.insert(offset, b)
			Return Me
		End Function

		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overrides Function insert(  offset As Integer,   c As Char) As StringBuilder
			MyBase.insert(offset, c)
			Return Me
		End Function

		''' <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overrides Function insert(  offset As Integer,   i As Integer) As StringBuilder
			MyBase.insert(offset, i)
			Return Me
		End Function

		''' <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overrides Function insert(  offset As Integer,   l As Long) As StringBuilder
			MyBase.insert(offset, l)
			Return Me
		End Function

		''' <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overrides Function insert(  offset As Integer,   f As Single) As StringBuilder
			MyBase.insert(offset, f)
			Return Me
		End Function

		''' <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overrides Function insert(  offset As Integer,   d As Double) As StringBuilder
			MyBase.insert(offset, d)
			Return Me
		End Function

		Public Overrides Function indexOf(  str As String) As Integer
			Return MyBase.IndexOf(str)
		End Function

		Public Overrides Function indexOf(  str As String,   fromIndex As Integer) As Integer
			Return MyBase.IndexOf(str, fromIndex)
		End Function

		Public Overrides Function lastIndexOf(  str As String) As Integer
			Return MyBase.LastIndexOf(str)
		End Function

		Public Overrides Function lastIndexOf(  str As String,   fromIndex As Integer) As Integer
			Return MyBase.LastIndexOf(str, fromIndex)
		End Function

		Public Overrides Function reverse() As StringBuilder
			MyBase.reverse()
			Return Me
		End Function

		Public Overrides Function ToString() As String
			' Create a copy, don't share the array
			Return New String(value, 0, count)
		End Function

		''' <summary>
		''' Save the state of the {@code StringBuilder} instance to a stream
		''' (that is, serialize it).
		''' 
		''' @serialData the number of characters currently stored in the string
		'''             builder ({@code int}), followed by the characters in the
		'''             string builder ({@code char[]}).   The length of the
		'''             {@code char} array may be greater than the number of
		'''             characters currently stored in the string builder, in which
		'''             case extra characters are ignored.
		''' </summary>
		Private Sub writeObject(  s As java.io.ObjectOutputStream)
			s.defaultWriteObject()
			s.writeInt(count)
			s.writeObject(value)
		End Sub

		''' <summary>
		''' readObject is called to restore the state of the StringBuffer from
		''' a stream.
		''' </summary>
		Private Sub readObject(  s As java.io.ObjectInputStream)
			s.defaultReadObject()
			count = s.readInt()
			value = CType(s.readObject(), Char())
		End Sub

	End Class

End Namespace