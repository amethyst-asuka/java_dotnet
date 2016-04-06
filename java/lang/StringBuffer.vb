Imports System
Imports System.Runtime.CompilerServices

'
' * Copyright (c) 1994, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' A thread-safe, mutable sequence of characters.
	''' A string buffer is like a <seealso cref="String"/>, but can be modified. At any
	''' point in time it contains some particular sequence of characters, but
	''' the length and content of the sequence can be changed through certain
	''' method calls.
	''' <p>
	''' String buffers are safe for use by multiple threads. The methods
	''' are synchronized where necessary so that all the operations on any
	''' particular instance behave as if they occur in some serial order
	''' that is consistent with the order of the method calls made by each of
	''' the individual threads involved.
	''' <p>
	''' The principal operations on a {@code StringBuffer} are the
	''' {@code append} and {@code insert} methods, which are
	''' overloaded so as to accept data of any type. Each effectively
	''' converts a given datum to a string and then appends or inserts the
	''' characters of that string to the string buffer. The
	''' {@code append} method always adds these characters at the end
	''' of the buffer; the {@code insert} method adds the characters at
	''' a specified point.
	''' <p>
	''' For example, if {@code z} refers to a string buffer object
	''' whose current contents are {@code "start"}, then
	''' the method call {@code z.append("le")} would cause the string
	''' buffer to contain {@code "startle"}, whereas
	''' {@code z.insert(4, "le")} would alter the string buffer to
	''' contain {@code "starlet"}.
	''' <p>
	''' In general, if sb refers to an instance of a {@code StringBuffer},
	''' then {@code sb.append(x)} has the same effect as
	''' {@code sb.insert(sb.length(), x)}.
	''' <p>
	''' Whenever an operation occurs involving a source sequence (such as
	''' appending or inserting from a source sequence), this class synchronizes
	''' only on the string buffer performing the operation, not on the source.
	''' Note that while {@code StringBuffer} is designed to be safe to use
	''' concurrently from multiple threads, if the constructor or the
	''' {@code append} or {@code insert} operation is passed a source sequence
	''' that is shared across threads, the calling code must ensure
	''' that the operation has a consistent and unchanging view of the source
	''' sequence for the duration of the operation.
	''' This could be satisfied by the caller holding a lock during the
	''' operation's call, by using an immutable source sequence, or by not
	''' sharing the source sequence across threads.
	''' <p>
	''' Every string buffer has a capacity. As long as the length of the
	''' character sequence contained in the string buffer does not exceed
	''' the capacity, it is not necessary to allocate a new internal
	''' buffer array. If the internal buffer overflows, it is
	''' automatically made larger.
	''' <p>
	''' Unless otherwise noted, passing a {@code null} argument to a constructor
	''' or method in this class will cause a <seealso cref="NullPointerException"/> to be
	''' thrown.
	''' <p>
	''' As of  release JDK 5, this class has been supplemented with an equivalent
	''' class designed for use by a single thread, <seealso cref="StringBuilder"/>.  The
	''' {@code StringBuilder} class should generally be used in preference to
	''' this one, as it supports all of the same operations but it is faster, as
	''' it performs no synchronization.
	''' 
	''' @author      Arthur van Hoff </summary>
	''' <seealso cref=     java.lang.StringBuilder </seealso>
	''' <seealso cref=     java.lang.String
	''' @since   JDK1.0 </seealso>
	 <Serializable> _
	 Public NotInheritable Class StringBuffer
		 Inherits AbstractStringBuilder
		 Implements CharSequence

		''' <summary>
		''' A cache of the last value returned by toString. Cleared
		''' whenever the StringBuffer is modified.
		''' </summary>
		<NonSerialized> _
		Private toStringCache As Char()

		''' <summary>
		''' use serialVersionUID from JDK 1.0.2 for interoperability </summary>
		Friend Const serialVersionUID As Long = 3388685877147921107L

		''' <summary>
		''' Constructs a string buffer with no characters in it and an
		''' initial capacity of 16 characters.
		''' </summary>
		Public Sub New()
			MyBase.New(16)
		End Sub

		''' <summary>
		''' Constructs a string buffer with no characters in it and
		''' the specified initial capacity.
		''' </summary>
		''' <param name="capacity">  the initial capacity. </param>
		''' <exception cref="NegativeArraySizeException">  if the {@code capacity}
		'''               argument is less than {@code 0}. </exception>
		Public Sub New(  capacity As Integer)
			MyBase.New(capacity)
		End Sub

		''' <summary>
		''' Constructs a string buffer initialized to the contents of the
		''' specified string. The initial capacity of the string buffer is
		''' {@code 16} plus the length of the string argument.
		''' </summary>
		''' <param name="str">   the initial contents of the buffer. </param>
		Public Sub New(  str As String)
			MyBase.New(str.length() + 16)
			append(str)
		End Sub

		''' <summary>
		''' Constructs a string buffer that contains the same characters
		''' as the specified {@code CharSequence}. The initial capacity of
		''' the string buffer is {@code 16} plus the length of the
		''' {@code CharSequence} argument.
		''' <p>
		''' If the length of the specified {@code CharSequence} is
		''' less than or equal to zero, then an empty buffer of capacity
		''' {@code 16} is returned.
		''' </summary>
		''' <param name="seq">   the sequence to copy.
		''' @since 1.5 </param>
		Public Sub New(  seq As CharSequence)
			Me.New(seq.length() + 16)
			append(seq)
		End Sub

        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Function length() As Integer Implements CharSequence.length
            Return Count()
        End Function

        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Function capacity() As Integer
            Return value.Length
        End Function


        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Sub ensureCapacity(  minimumCapacity As Integer)
            If minimumCapacity > value.Length Then expandCapacity(minimumCapacity)
        End Sub

        ''' <summary>
        ''' @since      1.5
        ''' </summary>
        <MethodImpl(MethodImplOptions.Synchronized)>
        Public Sub trimToSize()
            MyBase.trimToSize()
        End Sub

        ''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
        ''' <seealso cref=        #length() </seealso>
        <MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Property length As Integer
			Set(  newLength As Integer)
				toStringCache = Nothing
				MyBase.length = newLength
			End Set
		End Property

		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		''' <seealso cref=        #length() </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function charAt(  index As Integer) As Char Implements CharSequence.charAt
			If (index < 0) OrElse (index >= count) Then Throw New StringIndexOutOfBoundsException(index)
			Return value(index)
		End Function

		''' <summary>
		''' @since      1.5
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function codePointAt(  index As Integer) As Integer
			Return MyBase.codePointAt(index)
		End Function

		''' <summary>
		''' @since     1.5
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function codePointBefore(  index As Integer) As Integer
			Return MyBase.codePointBefore(index)
		End Function

		''' <summary>
		''' @since     1.5
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function codePointCount(  beginIndex As Integer,   endIndex As Integer) As Integer
			Return MyBase.codePointCount(beginIndex, endIndex)
		End Function

		''' <summary>
		''' @since     1.5
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function offsetByCodePoints(  index As Integer,   codePointOffset As Integer) As Integer
			Return MyBase.offsetByCodePoints(index, codePointOffset)
		End Function

		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Sub getChars(  srcBegin As Integer,   srcEnd As Integer,   dst As Char(),   dstBegin As Integer)
			MyBase.getChars(srcBegin, srcEnd, dst, dstBegin)
		End Sub

		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		''' <seealso cref=        #length() </seealso>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Sub setCharAt(  index As Integer,   ch As Char)
			If (index < 0) OrElse (index >= count) Then Throw New StringIndexOutOfBoundsException(index)
			toStringCache = Nothing
			value(index) = ch
		End Sub

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function append(  obj As Object) As StringBuffer
			toStringCache = Nothing
			MyBase.append(Convert.ToString(obj))
			Return Me
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function append(  str As String) As StringBuffer
			toStringCache = Nothing
			MyBase.append(str)
			Return Me
		End Function

		''' <summary>
		''' Appends the specified {@code StringBuffer} to this sequence.
		''' <p>
		''' The characters of the {@code StringBuffer} argument are appended,
		''' in order, to the contents of this {@code StringBuffer}, increasing the
		''' length of this {@code StringBuffer} by the length of the argument.
		''' If {@code sb} is {@code null}, then the four characters
		''' {@code "null"} are appended to this {@code StringBuffer}.
		''' <p>
		''' Let <i>n</i> be the length of the old character sequence, the one
		''' contained in the {@code StringBuffer} just prior to execution of the
		''' {@code append} method. Then the character at index <i>k</i> in
		''' the new character sequence is equal to the character at index <i>k</i>
		''' in the old character sequence, if <i>k</i> is less than <i>n</i>;
		''' otherwise, it is equal to the character at index <i>k-n</i> in the
		''' argument {@code sb}.
		''' <p>
		''' This method synchronizes on {@code this}, the destination
		''' object, but does not synchronize on the source ({@code sb}).
		''' </summary>
		''' <param name="sb">   the {@code StringBuffer} to append. </param>
		''' <returns>  a reference to this object.
		''' @since 1.4 </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function append(  sb As StringBuffer) As StringBuffer
			toStringCache = Nothing
			MyBase.append(sb)
			Return Me
		End Function

		''' <summary>
		''' @since 1.8
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Overrides Function append(  asb As AbstractStringBuilder) As StringBuffer
			toStringCache = Nothing
			MyBase.append(asb)
			Return Me
		End Function

		''' <summary>
		''' Appends the specified {@code CharSequence} to this
		''' sequence.
		''' <p>
		''' The characters of the {@code CharSequence} argument are appended,
		''' in order, increasing the length of this sequence by the length of the
		''' argument.
		''' 
		''' <p>The result of this method is exactly the same as if it were an
		''' invocation of this.append(s, 0, s.length());
		''' 
		''' <p>This method synchronizes on {@code this}, the destination
		''' object, but does not synchronize on the source ({@code s}).
		''' 
		''' <p>If {@code s} is {@code null}, then the four characters
		''' {@code "null"} are appended.
		''' </summary>
		''' <param name="s"> the {@code CharSequence} to append. </param>
		''' <returns>  a reference to this object.
		''' @since 1.5 </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function append(  s As CharSequence) As StringBuffer
			toStringCache = Nothing
			MyBase.append(s)
			Return Me
		End Function

		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc}
		''' @since      1.5 </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function append(  s As CharSequence,   start As Integer,   [end] As Integer) As StringBuffer
			toStringCache = Nothing
			MyBase.append(s, start, [end])
			Return Me
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function append(  str As Char()) As StringBuffer
			toStringCache = Nothing
			MyBase.append(str)
			Return Me
		End Function

		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function append(  str As Char(),   offset As Integer,   len As Integer) As StringBuffer
			toStringCache = Nothing
			MyBase.append(str, offset, len)
			Return Me
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function append(  b As Boolean) As StringBuffer
			toStringCache = Nothing
			MyBase.append(b)
			Return Me
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function append(  c As Char) As StringBuffer
			toStringCache = Nothing
			MyBase.append(c)
			Return Me
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function append(  i As Integer) As StringBuffer
			toStringCache = Nothing
			MyBase.append(i)
			Return Me
		End Function

		''' <summary>
		''' @since 1.5
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function appendCodePoint(  codePoint As Integer) As StringBuffer
			toStringCache = Nothing
			MyBase.appendCodePoint(codePoint)
			Return Me
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function append(  lng As Long) As StringBuffer
			toStringCache = Nothing
			MyBase.append(lng)
			Return Me
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function append(  f As Single) As StringBuffer
			toStringCache = Nothing
			MyBase.append(f)
			Return Me
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function append(  d As Double) As StringBuffer
			toStringCache = Nothing
			MyBase.append(d)
			Return Me
		End Function

		''' <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc}
		''' @since      1.2 </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function delete(  start As Integer,   [end] As Integer) As StringBuffer
			toStringCache = Nothing
			MyBase.delete(start, [end])
			Return Me
		End Function

		''' <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc}
		''' @since      1.2 </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function deleteCharAt(  index As Integer) As StringBuffer
			toStringCache = Nothing
			MyBase.deleteCharAt(index)
			Return Me
		End Function

		''' <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc}
		''' @since      1.2 </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function replace(  start As Integer,   [end] As Integer,   str As String) As StringBuffer
			toStringCache = Nothing
			MyBase.replace(start, [end], str)
			Return Me
		End Function

		''' <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc}
		''' @since      1.2 </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function substring(  start As Integer) As String
			Return Substring(start, count)
		End Function

		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc}
		''' @since      1.4 </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function subSequence(  start As Integer,   [end] As Integer) As CharSequence Implements CharSequence.subSequence
			Return MyBase.Substring(start, [end] - start)
		End Function

		''' <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc}
		''' @since      1.2 </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function substring(  start As Integer,   [end] As Integer) As String
			Return MyBase.Substring(start, [end] - start)
		End Function

		''' <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc}
		''' @since      1.2 </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function insert(  index As Integer,   str As Char(),   offset As Integer,   len As Integer) As StringBuffer
			toStringCache = Nothing
			MyBase.insert(index, str, offset, len)
			Return Me
		End Function

		''' <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc} </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function insert(  offset As Integer,   obj As Object) As StringBuffer
			toStringCache = Nothing
			MyBase.insert(offset, Convert.ToString(obj))
			Return Me
		End Function

		''' <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc} </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function insert(  offset As Integer,   str As String) As StringBuffer
			toStringCache = Nothing
			MyBase.insert(offset, str)
			Return Me
		End Function

		''' <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc} </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function insert(  offset As Integer,   str As Char()) As StringBuffer
			toStringCache = Nothing
			MyBase.insert(offset, str)
			Return Me
		End Function

		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc}
		''' @since      1.5 </exception>
		Public Overrides Function insert(  dstOffset As Integer,   s As CharSequence) As StringBuffer
			' Note, synchronization achieved via invocations of other StringBuffer methods
			' after narrowing of s to specific type
			' Ditto for toStringCache clearing
			MyBase.insert(dstOffset, s)
			Return Me
		End Function

		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc}
		''' @since      1.5 </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function insert(  dstOffset As Integer,   s As CharSequence,   start As Integer,   [end] As Integer) As StringBuffer
			toStringCache = Nothing
			MyBase.insert(dstOffset, s, start, [end])
			Return Me
		End Function

		''' <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overrides Function insert(  offset As Integer,   b As Boolean) As StringBuffer
			' Note, synchronization achieved via invocation of StringBuffer insert(int, String)
			' after conversion of b to String by super class method
			' Ditto for toStringCache clearing
			MyBase.insert(offset, b)
			Return Me
		End Function

		''' <exception cref="IndexOutOfBoundsException"> {@inheritDoc} </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function insert(  offset As Integer,   c As Char) As StringBuffer
			toStringCache = Nothing
			MyBase.insert(offset, c)
			Return Me
		End Function

		''' <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overrides Function insert(  offset As Integer,   i As Integer) As StringBuffer
			' Note, synchronization achieved via invocation of StringBuffer insert(int, String)
			' after conversion of i to String by super class method
			' Ditto for toStringCache clearing
			MyBase.insert(offset, i)
			Return Me
		End Function

		''' <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overrides Function insert(  offset As Integer,   l As Long) As StringBuffer
			' Note, synchronization achieved via invocation of StringBuffer insert(int, String)
			' after conversion of l to String by super class method
			' Ditto for toStringCache clearing
			MyBase.insert(offset, l)
			Return Me
		End Function

		''' <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overrides Function insert(  offset As Integer,   f As Single) As StringBuffer
			' Note, synchronization achieved via invocation of StringBuffer insert(int, String)
			' after conversion of f to String by super class method
			' Ditto for toStringCache clearing
			MyBase.insert(offset, f)
			Return Me
		End Function

		''' <exception cref="StringIndexOutOfBoundsException"> {@inheritDoc} </exception>
		Public Overrides Function insert(  offset As Integer,   d As Double) As StringBuffer
			' Note, synchronization achieved via invocation of StringBuffer insert(int, String)
			' after conversion of d to String by super class method
			' Ditto for toStringCache clearing
			MyBase.insert(offset, d)
			Return Me
		End Function

		''' <summary>
		''' @since      1.4
		''' </summary>
		Public Overrides Function indexOf(  str As String) As Integer
			' Note, synchronization achieved via invocations of other StringBuffer methods
			Return MyBase.IndexOf(str)
		End Function

		''' <summary>
		''' @since      1.4
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function indexOf(  str As String,   fromIndex As Integer) As Integer
			Return MyBase.IndexOf(str, fromIndex)
		End Function

		''' <summary>
		''' @since      1.4
		''' </summary>
		Public Overrides Function lastIndexOf(  str As String) As Integer
			' Note, synchronization achieved via invocations of other StringBuffer methods
			Return LastIndexOf(str, count)
		End Function

		''' <summary>
		''' @since      1.4
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function lastIndexOf(  str As String,   fromIndex As Integer) As Integer
			Return MyBase.LastIndexOf(str, fromIndex)
		End Function

		''' <summary>
		''' @since   JDK1.0.2
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function reverse() As StringBuffer
			toStringCache = Nothing
			MyBase.reverse()
			Return Me
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overrides Function ToString() As String
			If toStringCache Is Nothing Then toStringCache = java.util.Arrays.copyOfRange(value, 0, count)
			Return New String(toStringCache, True)
		End Function

		''' <summary>
		''' Serializable fields for StringBuffer.
		''' 
		''' @serialField value  char[]
		'''              The backing character array of this StringBuffer.
		''' @serialField count int
		'''              The number of characters in this StringBuffer.
		''' @serialField shared  boolean
		'''              A flag indicating whether the backing array is shared.
		'''              The value is ignored upon deserialization.
		''' </summary>
		Private Shared ReadOnly serialPersistentFields As java.io.ObjectStreamField() = { New java.io.ObjectStreamField("value", GetType(Char())), New java.io.ObjectStreamField("count",  java.lang.[Integer].TYPE), New java.io.ObjectStreamField("shared",  java.lang.[Boolean].TYPE) }

		''' <summary>
		''' readObject is called to restore the state of the StringBuffer from
		''' a stream.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Sub writeObject(  s As java.io.ObjectOutputStream)
			Dim fields As java.io.ObjectOutputStream.PutField = s.putFields()
			fields.put("value", value)
			fields.put("count", count)
			fields.put("shared", False)
			s.writeFields()
		End Sub

		''' <summary>
		''' readObject is called to restore the state of the StringBuffer from
		''' a stream.
		''' </summary>
		Private Sub readObject(  s As java.io.ObjectInputStream)
			Dim fields As java.io.ObjectInputStream.GetField = s.readFields()
			value = CType(fields.get("value", Nothing), Char())
			count = fields.get("count", 0)
		End Sub
	 End Class

End Namespace