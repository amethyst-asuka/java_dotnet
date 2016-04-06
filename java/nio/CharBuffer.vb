Imports System

'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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

' -- This file was mechanically generated: Do not edit! -- //

Namespace java.nio






    ''' <summary>
    ''' A char buffer.
    '''
    ''' <p> This class defines four categories of operations upon
    ''' char buffers:
    '''
    ''' <ul>
    '''
    '''   <li><p> Absolute and relative <seealso cref="#get() <i>get</i>"/> and
    '''   <seealso cref="#put(char) <i>put</i>"/> methods that read and write
    '''   single chars; </p></li>
    '''
    '''   <li><p> Relative <seealso cref="#get(char[]) <i>bulk get</i>"/>
    '''   methods that transfer contiguous sequences of chars from this buffer
    '''   into an array; and</p></li>
    '''
    '''   <li><p> Relative <seealso cref="#put(char[]) <i>bulk put</i>"/>
    '''   methods that transfer contiguous sequences of chars from a
    '''   char array,&#32;a&#32;string, or some other char
    '''   buffer into this buffer;&#32;and </p></li>
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''   <li><p> Methods for <seealso cref="#compact compacting"/>, {@link
    '''   #duplicate duplicating}, and <seealso cref="#slice slicing"/>
    '''   a char buffer.  </p></li>
    '''
    ''' </ul>
    '''
    ''' <p> Char buffers can be created either by {@link #allocate
    ''' <i>allocation</i>}, which allocates space for the buffer's
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    ''' content, by <seealso cref="#wrap(char[]) <i>wrapping</i>"/> an existing
    ''' char array or&#32;string into a buffer, or by creating a
    ''' <a href="ByteBuffer.html#views"><i>view</i></a> of an existing byte buffer.
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    ''' <p> Like a byte buffer, a char buffer is either <a
    ''' href="ByteBuffer.html#direct"><i>direct</i> or <i>non-direct</i></a>.  A
    ''' char buffer created via the <tt>wrap</tt> methods of this class will
    ''' be non-direct.  A char buffer created as a view of a byte buffer will
    ''' be direct if, and only if, the byte buffer itself is direct.  Whether or not
    ''' a char buffer is direct may be determined by invoking the {@link
    ''' #isDirect isDirect} method.  </p>
    '''
    '''
    '''
    '''
    '''
    ''' <p> This class implements the <seealso cref="CharSequence"/> interface so that
    ''' character buffers may be used wherever character sequences are accepted, for
    ''' example in the regular-expression package <tt><seealso cref="java.util.regex"/></tt>.
    ''' </p>
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    ''' <p> Methods in this class that do not otherwise have a value to return are
    ''' specified to return the buffer upon which they are invoked.  This allows
    ''' method invocations to be chained.
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    '''
    ''' The sequence of statements
    '''
    ''' <blockquote><pre>
    ''' cb.put("text/");
    ''' cb.put(subtype);
    ''' cb.put("; charset=");
    ''' cb.put(enc);</pre></blockquote>
    '''
    ''' can, for example, be replaced by the single statement
    '''
    ''' <blockquote><pre>
    ''' cb.put("text/").put(subtype).put("; charset=").put(enc);</pre></blockquote>
    '''
    '''
    '''
    '''
    ''' @author Mark Reinhold
    ''' @author JSR-51 Expert Group
    ''' @since 1.4
    ''' </summary>

    Public MustInherit Class CharBuffer
        Inherits Buffer
        Implements Comparable(Of CharBuffer), Appendable, CharSequence, Readable

        '   Public MustOverride Function read(  cb As java.nio.CharBuffer) As Integer

        '   Public MustOverride Return intStream(() -> java.util.Spliterators.spliteratorUnknownSize(New CodePointIterator, java.util.Spliterator.ORDERED), java.util.Spliterator.ORDERED, False);

        '	Public MustOverride = Function() toCodePoint(c1, c2)
        '   Public Function [if](Character.isHighSurrogate(c1) AndAlso cur <   length As ) As [MustOverride]
        '    Public Function [if](cur >=   length As ) As [MustOverride]
        '    Public Function accept(Character.toCodePoint(c1, c2)   As ) As [MustOverride]
        '   Public Function [if](Character.isLowSurrogate(c2)   As ) As [MustOverride]

        '	Public MustOverride accept(c1);
        '   Public Function [if]((Not Character.isHighSurrogate(c1)) OrElse i >=   length As ) As [MustOverride]
        '   Public Function [while](i <   length As ) As [MustOverride]
        '    Public MustOverride Function codePoints() As default

        '	Public MustOverride Return intStream(() -> java.util.Spliterators.spliterator(New CharIterator, length(), java.util.Spliterator.ORDERED), java.util.Spliterator.SUBSIZED | java.util.Spliterator.SIZED | java.util.Spliterator.ORDERED, False);
        '	Public Function accept(charAt(cur)   As ) As [MustOverride]

        '   Public Function [for](; cur < length(); cur +=   1 As ) As [MustOverride]
        Public MustOverride Sub forEachRemaining(  block As java.util.function.IntConsumer) Implements CharSequence.forEachRemaining
        '   Public MustOverride Function NoSuchElementException() As throw
        '	Friend   += 1
        '	Public MustOverride Function charAt(cur   As ) As [Return]
        '  Public MustOverride [if](hasNext())
        Public MustOverride Function nextInt() As Integer
        Public MustOverride Function hasNext() As Boolean

        ' These fields are declared here rather than in Heap-X-Buffer in order to
        ' reduce the number of virtual method invocations needed to access these
        ' values, which is especially costly when coding small buffers.
        '
        Dim hb As Char() ' Non-null only for heap buffers
        Dim offset As Integer
        Dim isReadOnly As Boolean ' Valid only for heap buffers

        ' Creates a new buffer with the given mark, position, limit, capacity,
        ' backing array, and array offset
        '
        Sub New(mark As Integer, pos As Integer, lim As Integer, cap As Integer, hb As Char(), offset As Integer) ' package-private
            MyBase.New(mark, pos, lim, cap)
            Me.hb = hb
            Me.offset = offset
        End Sub

        ' Creates a new buffer with the given mark, position, limit, and capacity
        '
        Sub New(mark As Integer, pos As Integer, lim As Integer, cap As Integer) ' package-private
            Me.New(mark, pos, lim, cap, Nothing, 0)
        End Sub

        ''' <summary>
        ''' Allocates a new char buffer.
        '''
        ''' <p> The new buffer's position will be zero, its limit will be its
        ''' capacity, its mark will be undefined, and each of its elements will be
        ''' initialized to zero.  It will have a <seealso cref="#array backing array"/>,
        ''' and its <seealso cref="#arrayOffset array offset"/> will be zero.
        ''' </summary>
        ''' <param name="capacity">
        '''         The new buffer's capacity, in chars
        ''' </param>
        ''' <returns>  The new char buffer
        ''' </returns>
        ''' <exception cref="IllegalArgumentException">
        '''          If the <tt>capacity</tt> is a negative integer </exception>
        Public Shared Function allocate(capacity As Integer) As CharBuffer
            If capacity < 0 Then Throw New IllegalArgumentException
            Return New HeapCharBuffer(capacity, capacity)
        End Function

        ''' <summary>
        ''' Wraps a char array into a buffer.
        '''
        ''' <p> The new buffer will be backed by the given char array;
        ''' that is, modifications to the buffer will cause the array to be modified
        ''' and vice versa.  The new buffer's capacity will be
        ''' <tt>array.length</tt>, its position will be <tt>offset</tt>, its limit
        ''' will be <tt>offset + length</tt>, and its mark will be undefined.  Its
        ''' <seealso cref="#array backing array"/> will be the given array, and
        ''' its <seealso cref="#arrayOffset array offset"/> will be zero.  </p>
        ''' </summary>
        ''' <param name="array">
        '''         The array that will back the new buffer
        ''' </param>
        ''' <param name="offset">
        '''         The offset of the subarray to be used; must be non-negative and
        '''         no larger than <tt>array.length</tt>.  The new buffer's position
        '''         will be set to this value.
        ''' </param>
        ''' <param name="length">
        '''         The length of the subarray to be used;
        '''         must be non-negative and no larger than
        '''         <tt>array.length - offset</tt>.
        '''         The new buffer's limit will be set to <tt>offset + length</tt>.
        ''' </param>
        ''' <returns>  The new char buffer
        ''' </returns>
        ''' <exception cref="IndexOutOfBoundsException">
        '''          If the preconditions on the <tt>offset</tt> and <tt>length</tt>
        '''          parameters do not hold </exception>
        Public Shared Function wrap(array As Char(), offset As Integer, length As Integer) As CharBuffer
            Try
                Return New HeapCharBuffer(array, offset, length)
            Catch x As IllegalArgumentException
                Throw New IndexOutOfBoundsException
            End Try
        End Function

        ''' <summary>
        ''' Wraps a char array into a buffer.
        '''
        ''' <p> The new buffer will be backed by the given char array;
        ''' that is, modifications to the buffer will cause the array to be modified
        ''' and vice versa.  The new buffer's capacity and limit will be
        ''' <tt>array.length</tt>, its position will be zero, and its mark will be
        ''' undefined.  Its <seealso cref="#array backing array"/> will be the
        ''' given array, and its <seealso cref="#arrayOffset array offset>"/> will
        ''' be zero.  </p>
        ''' </summary>
        ''' <param name="array">
        '''         The array that will back this buffer
        ''' </param>
        ''' <returns>  The new char buffer </returns>
        Public Shared Function wrap(array As Char()) As CharBuffer
            Return wrap(array, 0, array.Length)
        End Function

        ''' <summary>
        ''' Attempts to read characters into the specified character buffer.
        ''' The buffer is used as a repository of characters as-is: the only
        ''' changes made are the results of a put operation. No flipping or
        ''' rewinding of the buffer is performed.
        ''' </summary>
        ''' <param name="target"> the buffer to read characters into </param>
        ''' <returns> The number of characters added to the buffer, or
        '''         -1 if this source of characters is at its end </returns>
        ''' <exception cref="IOException"> if an I/O error occurs </exception>
        ''' <exception cref="NullPointerException"> if target is null </exception>
        ''' <exception cref="ReadOnlyBufferException"> if target is a read only buffer
        ''' @since 1.5 </exception>
        Public Function read(target As CharBuffer) As Integer Implements Readable.read
            ' Determine the number of bytes n that can be transferred
            Dim targetRemaining As Integer = target.remaining()
            Dim remaining As Integer = remaining()
            If remaining = 0 Then Return -1
            Dim n As Integer = System.Math.Min(remaining, targetRemaining)
            Dim limit As Integer = limit()
            ' Set source limit to prevent target overflow
            If targetRemaining < remaining Then limit(position() + n)
            Try
                If n > 0 Then target.put(Me)
            Finally
                limit(limit) ' restore real limit
            End Try
            Return n
        End Function

        ''' <summary>
        ''' Wraps a character sequence into a buffer.
        '''
        ''' <p> The content of the new, read-only buffer will be the content of the
        ''' given character sequence.  The buffer's capacity will be
        ''' <tt>csq.length()</tt>, its position will be <tt>start</tt>, its limit
        ''' will be <tt>end</tt>, and its mark will be undefined.  </p>
        ''' </summary>
        ''' <param name="csq">
        '''         The character sequence from which the new character buffer is to
        '''         be created
        ''' </param>
        ''' <param name="start">
        '''         The index of the first character to be used;
        '''         must be non-negative and no larger than <tt>csq.length()</tt>.
        '''         The new buffer's position will be set to this value.
        ''' </param>
        ''' <param name="end">
        '''         The index of the character following the last character to be
        '''         used; must be no smaller than <tt>start</tt> and no larger
        '''         than <tt>csq.length()</tt>.
        '''         The new buffer's limit will be set to this value.
        ''' </param>
        ''' <returns>  The new character buffer
        ''' </returns>
        ''' <exception cref="IndexOutOfBoundsException">
        '''          If the preconditions on the <tt>start</tt> and <tt>end</tt>
        '''          parameters do not hold </exception>
        Public Shared Function wrap(csq As CharSequence, start As Integer, [end] As Integer) As CharBuffer
            Try
                Return New StringCharBuffer(csq, start, [end])
            Catch x As IllegalArgumentException
                Throw New IndexOutOfBoundsException
            End Try
        End Function

        ''' <summary>
        ''' Wraps a character sequence into a buffer.
        '''
        ''' <p> The content of the new, read-only buffer will be the content of the
        ''' given character sequence.  The new buffer's capacity and limit will be
        ''' <tt>csq.length()</tt>, its position will be zero, and its mark will be
        ''' undefined.  </p>
        ''' </summary>
        ''' <param name="csq">
        '''         The character sequence from which the new character buffer is to
        '''         be created
        ''' </param>
        ''' <returns>  The new character buffer </returns>
        Public Shared Function wrap(csq As CharSequence) As CharBuffer
            Return wrap(csq, 0, csq.length())
        End Function

        ''' <summary>
        ''' Creates a new char buffer whose content is a shared subsequence of
        ''' this buffer's content.
        '''
        ''' <p> The content of the new buffer will start at this buffer's current
        ''' position.  Changes to this buffer's content will be visible in the new
        ''' buffer, and vice versa; the two buffers' position, limit, and mark
        ''' values will be independent.
        '''
        ''' <p> The new buffer's position will be zero, its capacity and its limit
        ''' will be the number of chars remaining in this buffer, and its mark
        ''' will be undefined.  The new buffer will be direct if, and only if, this
        ''' buffer is direct, and it will be read-only if, and only if, this buffer
        ''' is read-only.  </p>
        ''' </summary>
        ''' <returns>  The new char buffer </returns>
        Public MustOverride Function slice() As CharBuffer

        ''' <summary>
        ''' Creates a new char buffer that shares this buffer's content.
        '''
        ''' <p> The content of the new buffer will be that of this buffer.  Changes
        ''' to this buffer's content will be visible in the new buffer, and vice
        ''' versa; the two buffers' position, limit, and mark values will be
        ''' independent.
        '''
        ''' <p> The new buffer's capacity, limit, position, and mark values will be
        ''' identical to those of this buffer.  The new buffer will be direct if,
        ''' and only if, this buffer is direct, and it will be read-only if, and
        ''' only if, this buffer is read-only.  </p>
        ''' </summary>
        ''' <returns>  The new char buffer </returns>
        Public MustOverride Function duplicate() As CharBuffer

        ''' <summary>
        ''' Creates a new, read-only char buffer that shares this buffer's
        ''' content.
        '''
        ''' <p> The content of the new buffer will be that of this buffer.  Changes
        ''' to this buffer's content will be visible in the new buffer; the new
        ''' buffer itself, however, will be read-only and will not allow the shared
        ''' content to be modified.  The two buffers' position, limit, and mark
        ''' values will be independent.
        '''
        ''' <p> The new buffer's capacity, limit, position, and mark values will be
        ''' identical to those of this buffer.
        '''
        ''' <p> If this buffer is itself read-only then this method behaves in
        ''' exactly the same way as the <seealso cref="#duplicate duplicate"/> method.  </p>
        ''' </summary>
        ''' <returns>  The new, read-only char buffer </returns>
        Public MustOverride Function asReadOnlyBuffer() As CharBuffer


        ' -- Singleton get/put methods --

        ''' <summary>
        ''' Relative <i>get</i> method.  Reads the char at this buffer's
        ''' current position, and then increments the position.
        ''' </summary>
        ''' <returns>  The char at the buffer's current position
        ''' </returns>
        ''' <exception cref="BufferUnderflowException">
        '''          If the buffer's current position is not smaller than its limit </exception>
        Public MustOverride Function [get]() As Char

        ''' <summary>
        ''' Relative <i>put</i> method&nbsp;&nbsp;<i>(optional operation)</i>.
        '''
        ''' <p> Writes the given char into this buffer at the current
        ''' position, and then increments the position. </p>
        ''' </summary>
        ''' <param name="c">
        '''         The char to be written
        ''' </param>
        ''' <returns>  This buffer
        ''' </returns>
        ''' <exception cref="BufferOverflowException">
        '''          If this buffer's current position is not smaller than its limit
        ''' </exception>
        ''' <exception cref="ReadOnlyBufferException">
        '''          If this buffer is read-only </exception>
        Public MustOverride Function put(c As Char) As CharBuffer

        ''' <summary>
        ''' Absolute <i>get</i> method.  Reads the char at the given
        ''' index.
        ''' </summary>
        ''' <param name="index">
        '''         The index from which the char will be read
        ''' </param>
        ''' <returns>  The char at the given index
        ''' </returns>
        ''' <exception cref="IndexOutOfBoundsException">
        '''          If <tt>index</tt> is negative
        '''          or not smaller than the buffer's limit </exception>
        Public MustOverride Function [get](index As Integer) As Char


        ''' <summary>
        ''' Absolute <i>get</i> method.  Reads the char at the given
        ''' index without any validation of the index.
        ''' </summary>
        ''' <param name="index">
        '''         The index from which the char will be read
        ''' </param>
        ''' <returns>  The char at the given index </returns>
        Public MustOverride Function getUnchecked(index As Integer) As Char  ' package-private


        ''' <summary>
        ''' Absolute <i>put</i> method&nbsp;&nbsp;<i>(optional operation)</i>.
        '''
        ''' <p> Writes the given char into this buffer at the given
        ''' index. </p>
        ''' </summary>
        ''' <param name="index">
        '''         The index at which the char will be written
        ''' </param>
        ''' <param name="c">
        '''         The char value to be written
        ''' </param>
        ''' <returns>  This buffer
        ''' </returns>
        ''' <exception cref="IndexOutOfBoundsException">
        '''          If <tt>index</tt> is negative
        '''          or not smaller than the buffer's limit
        ''' </exception>
        ''' <exception cref="ReadOnlyBufferException">
        '''          If this buffer is read-only </exception>
        Public MustOverride Function put(index As Integer, c As Char) As CharBuffer


        ' -- Bulk get operations --

        ''' <summary>
        ''' Relative bulk <i>get</i> method.
        '''
        ''' <p> This method transfers chars from this buffer into the given
        ''' destination array.  If there are fewer chars remaining in the
        ''' buffer than are required to satisfy the request, that is, if
        ''' <tt>length</tt>&nbsp;<tt>&gt;</tt>&nbsp;<tt>remaining()</tt>, then no
        ''' chars are transferred and a <seealso cref="BufferUnderflowException"/> is
        ''' thrown.
        '''
        ''' <p> Otherwise, this method copies <tt>length</tt> chars from this
        ''' buffer into the given array, starting at the current position of this
        ''' buffer and at the given offset in the array.  The position of this
        ''' buffer is then incremented by <tt>length</tt>.
        '''
        ''' <p> In other words, an invocation of this method of the form
        ''' <tt>src.get(dst,&nbsp;off,&nbsp;len)</tt> has exactly the same effect as
        ''' the loop
        '''
        ''' <pre>{@code
        '''     for (int i = off; i < off + len; i++)
        '''         dst[i] = src.get():
        ''' }</pre>
        '''
        ''' except that it first checks that there are sufficient chars in
        ''' this buffer and it is potentially much more efficient.
        ''' </summary>
        ''' <param name="dst">
        '''         The array into which chars are to be written
        ''' </param>
        ''' <param name="offset">
        '''         The offset within the array of the first char to be
        '''         written; must be non-negative and no larger than
        '''         <tt>dst.length</tt>
        ''' </param>
        ''' <param name="length">
        '''         The maximum number of chars to be written to the given
        '''         array; must be non-negative and no larger than
        '''         <tt>dst.length - offset</tt>
        ''' </param>
        ''' <returns>  This buffer
        ''' </returns>
        ''' <exception cref="BufferUnderflowException">
        '''          If there are fewer than <tt>length</tt> chars
        '''          remaining in this buffer
        ''' </exception>
        ''' <exception cref="IndexOutOfBoundsException">
        '''          If the preconditions on the <tt>offset</tt> and <tt>length</tt>
        '''          parameters do not hold </exception>
        Public Function [get](dst As Char(), offset As Integer, length As Integer) As CharBuffer
            checkBounds(offset, length, dst.Length)
            If length > remaining() Then Throw New BufferUnderflowException
            Dim [end] As Integer = offset + length
            For i As Integer = offset To [end] - 1
                dst(i) = [get]()
            Next i
            Return Me
        End Function
        ''' <summary>
        ''' Relative bulk <i>get</i> method.
        '''
        ''' <p> This method transfers chars from this buffer into the given
        ''' destination array.  An invocation of this method of the form
        ''' <tt>src.get(a)</tt> behaves in exactly the same way as the invocation
        '''
        ''' <pre>
        '''     src.get(a, 0, a.length) </pre>
        ''' </summary>
        ''' <param name="dst">
        '''          The destination array
        ''' </param>
        ''' <returns>  This buffer
        ''' </returns>
        ''' <exception cref="BufferUnderflowException">
        '''          If there are fewer than <tt>length</tt> chars
        '''          remaining in this buffer </exception>
        Public Function [get](dst As Char()) As CharBuffer
            Return [get](dst, 0, dst.Length)
        End Function

        ' -- Bulk put operations --

        ''' <summary>
        ''' Relative bulk <i>put</i> method&nbsp;&nbsp;<i>(optional operation)</i>.
        '''
        ''' <p> This method transfers the chars remaining in the given source
        ''' buffer into this buffer.  If there are more chars remaining in the
        ''' source buffer than in this buffer, that is, if
        ''' <tt>src.remaining()</tt>&nbsp;<tt>&gt;</tt>&nbsp;<tt>remaining()</tt>,
        ''' then no chars are transferred and a {@link
        ''' BufferOverflowException} is thrown.
        '''
        ''' <p> Otherwise, this method copies
        ''' <i>n</i>&nbsp;=&nbsp;<tt>src.remaining()</tt> chars from the given
        ''' buffer into this buffer, starting at each buffer's current position.
        ''' The positions of both buffers are then incremented by <i>n</i>.
        '''
        ''' <p> In other words, an invocation of this method of the form
        ''' <tt>dst.put(src)</tt> has exactly the same effect as the loop
        '''
        ''' <pre>
        '''     while (src.hasRemaining())
        '''         dst.put(src.get()); </pre>
        '''
        ''' except that it first checks that there is sufficient space in this
        ''' buffer and it is potentially much more efficient.
        ''' </summary>
        ''' <param name="src">
        '''         The source buffer from which chars are to be read;
        '''         must not be this buffer
        ''' </param>
        ''' <returns>  This buffer
        ''' </returns>
        ''' <exception cref="BufferOverflowException">
        '''          If there is insufficient space in this buffer
        '''          for the remaining chars in the source buffer
        ''' </exception>
        ''' <exception cref="IllegalArgumentException">
        '''          If the source buffer is this buffer
        ''' </exception>
        ''' <exception cref="ReadOnlyBufferException">
        '''          If this buffer is read-only </exception>
        Public Function put(src As CharBuffer) As CharBuffer
            If src Is Me Then Throw New IllegalArgumentException
            If [readOnly] Then Throw New ReadOnlyBufferException
            Dim n As Integer = src.remaining()
            If n > remaining() Then Throw New BufferOverflowException
            For i As Integer = 0 To n - 1
                put(src.get())
            Next i
            Return Me
        End Function

        ''' <summary>
        ''' Relative bulk <i>put</i> method&nbsp;&nbsp;<i>(optional operation)</i>.
        '''
        ''' <p> This method transfers chars into this buffer from the given
        ''' source array.  If there are more chars to be copied from the array
        ''' than remain in this buffer, that is, if
        ''' <tt>length</tt>&nbsp;<tt>&gt;</tt>&nbsp;<tt>remaining()</tt>, then no
        ''' chars are transferred and a <seealso cref="BufferOverflowException"/> is
        ''' thrown.
        '''
        ''' <p> Otherwise, this method copies <tt>length</tt> chars from the
        ''' given array into this buffer, starting at the given offset in the array
        ''' and at the current position of this buffer.  The position of this buffer
        ''' is then incremented by <tt>length</tt>.
        '''
        ''' <p> In other words, an invocation of this method of the form
        ''' <tt>dst.put(src,&nbsp;off,&nbsp;len)</tt> has exactly the same effect as
        ''' the loop
        '''
        ''' <pre>{@code
        '''     for (int i = off; i < off + len; i++)
        '''         dst.put(a[i]);
        ''' }</pre>
        '''
        ''' except that it first checks that there is sufficient space in this
        ''' buffer and it is potentially much more efficient.
        ''' </summary>
        ''' <param name="src">
        '''         The array from which chars are to be read
        ''' </param>
        ''' <param name="offset">
        '''         The offset within the array of the first char to be read;
        '''         must be non-negative and no larger than <tt>array.length</tt>
        ''' </param>
        ''' <param name="length">
        '''         The number of chars to be read from the given array;
        '''         must be non-negative and no larger than
        '''         <tt>array.length - offset</tt>
        ''' </param>
        ''' <returns>  This buffer
        ''' </returns>
        ''' <exception cref="BufferOverflowException">
        '''          If there is insufficient space in this buffer
        ''' </exception>
        ''' <exception cref="IndexOutOfBoundsException">
        '''          If the preconditions on the <tt>offset</tt> and <tt>length</tt>
        '''          parameters do not hold
        ''' </exception>
        ''' <exception cref="ReadOnlyBufferException">
        '''          If this buffer is read-only </exception>
        Public Function put(src As Char(), offset As Integer, length As Integer) As CharBuffer
            checkBounds(offset, length, src.Length)
            If length > remaining() Then Throw New BufferOverflowException
            Dim [end] As Integer = offset + length
            For i As Integer = offset To [end] - 1
                Me.put(src(i))
            Next i
            Return Me
        End Function
        ''' <summary>
        ''' Relative bulk <i>put</i> method&nbsp;&nbsp;<i>(optional operation)</i>.
        '''
        ''' <p> This method transfers the entire content of the given source
        ''' char array into this buffer.  An invocation of this method of the
        ''' form <tt>dst.put(a)</tt> behaves in exactly the same way as the
        ''' invocation
        '''
        ''' <pre>
        '''     dst.put(a, 0, a.length) </pre>
        ''' </summary>
        ''' <param name="src">
        '''          The source array
        ''' </param>
        ''' <returns>  This buffer
        ''' </returns>
        ''' <exception cref="BufferOverflowException">
        '''          If there is insufficient space in this buffer
        ''' </exception>
        ''' <exception cref="ReadOnlyBufferException">
        '''          If this buffer is read-only </exception>
        Public Function put(src As Char()) As CharBuffer
            Return put(src, 0, src.Length)
        End Function

        ''' <summary>
        ''' Relative bulk <i>put</i> method&nbsp;&nbsp;<i>(optional operation)</i>.
        '''
        ''' <p> This method transfers chars from the given string into this
        ''' buffer.  If there are more chars to be copied from the string than
        ''' remain in this buffer, that is, if
        ''' <tt>end&nbsp;-&nbsp;start</tt>&nbsp;<tt>&gt;</tt>&nbsp;<tt>remaining()</tt>,
        ''' then no chars are transferred and a {@link
        ''' BufferOverflowException} is thrown.
        '''
        ''' <p> Otherwise, this method copies
        ''' <i>n</i>&nbsp;=&nbsp;<tt>end</tt>&nbsp;-&nbsp;<tt>start</tt> chars
        ''' from the given string into this buffer, starting at the given
        ''' <tt>start</tt> index and at the current position of this buffer.  The
        ''' position of this buffer is then incremented by <i>n</i>.
        '''
        ''' <p> In other words, an invocation of this method of the form
        ''' <tt>dst.put(src,&nbsp;start,&nbsp;end)</tt> has exactly the same effect
        ''' as the loop
        '''
        ''' <pre>{@code
        '''     for (int i = start; i < end; i++)
        '''         dst.put(src.charAt(i));
        ''' }</pre>
        '''
        ''' except that it first checks that there is sufficient space in this
        ''' buffer and it is potentially much more efficient.
        ''' </summary>
        ''' <param name="src">
        '''         The string from which chars are to be read
        ''' </param>
        ''' <param name="start">
        '''         The offset within the string of the first char to be read;
        '''         must be non-negative and no larger than
        '''         <tt>string.length()</tt>
        ''' </param>
        ''' <param name="end">
        '''         The offset within the string of the last char to be read,
        '''         plus one; must be non-negative and no larger than
        '''         <tt>string.length()</tt>
        ''' </param>
        ''' <returns>  This buffer
        ''' </returns>
        ''' <exception cref="BufferOverflowException">
        '''          If there is insufficient space in this buffer
        ''' </exception>
        ''' <exception cref="IndexOutOfBoundsException">
        '''          If the preconditions on the <tt>start</tt> and <tt>end</tt>
        '''          parameters do not hold
        ''' </exception>
        ''' <exception cref="ReadOnlyBufferException">
        '''          If this buffer is read-only </exception>
        Public Function put(src As String, start As Integer, [End] As Integer) As CharBuffer
            checkBounds(start, end - start, src.Length())
            If [readOnly] Then Throw New ReadOnlyBufferException
            If [End] - start > remaining() Then Throw New BufferOverflowException
            For i As Integer = start To [End] - 1
                Me.put(src.Chars(i))
            Next i
            Return Me
        End Function

        ''' <summary>
        ''' Relative bulk <i>put</i> method&nbsp;&nbsp;<i>(optional operation)</i>.
        '''
        ''' <p> This method transfers the entire content of the given source string
        ''' into this buffer.  An invocation of this method of the form
        ''' <tt>dst.put(s)</tt> behaves in exactly the same way as the invocation
        '''
        ''' <pre>
        '''     dst.put(s, 0, s.length()) </pre>
        ''' </summary>
        ''' <param name="src">
        '''          The source string
        ''' </param>
        ''' <returns>  This buffer
        ''' </returns>
        ''' <exception cref="BufferOverflowException">
        '''          If there is insufficient space in this buffer
        ''' </exception>
        ''' <exception cref="ReadOnlyBufferException">
        '''          If this buffer is read-only </exception>
        Public Function put(src As String) As CharBuffer
            Return put(src, 0, src.Length())
        End Function

        ' -- Other stuff --

        ''' <summary>
        ''' Tells whether or not this buffer is backed by an accessible char
        ''' array.
        '''
        ''' <p> If this method returns <tt>true</tt> then the <seealso cref="#array() array"/>
        ''' and <seealso cref="#arrayOffset() arrayOffset"/> methods may safely be invoked.
        ''' </p>
        ''' </summary>
        ''' <returns>  <tt>true</tt> if, and only if, this buffer
        '''          is backed by an array and is not read-only </returns>
        Public Function hasArray() As Boolean
            Return (hb IsNot Nothing) AndAlso Not isReadOnly
        End Function

        ''' <summary>
        ''' Returns the char array that backs this
        ''' buffer&nbsp;&nbsp;<i>(optional operation)</i>.
        '''
        ''' <p> Modifications to this buffer's content will cause the returned
        ''' array's content to be modified, and vice versa.
        '''
        ''' <p> Invoke the <seealso cref="#hasArray hasArray"/> method before invoking this
        ''' method in order to ensure that this buffer has an accessible backing
        ''' array.  </p>
        ''' </summary>
        ''' <returns>  The array that backs this buffer
        ''' </returns>
        ''' <exception cref="ReadOnlyBufferException">
        '''          If this buffer is backed by an array but is read-only
        ''' </exception>
        ''' <exception cref="UnsupportedOperationException">
        '''          If this buffer is not backed by an accessible array </exception>
        Public Function array() As Char()
            If hb Is Nothing Then Throw New UnsupportedOperationException
            If isReadOnly Then Throw New ReadOnlyBufferException
            Return hb
        End Function

        ''' <summary>
        ''' Returns the offset within this buffer's backing array of the first
        ''' element of the buffer&nbsp;&nbsp;<i>(optional operation)</i>.
        '''
        ''' <p> If this buffer is backed by an array then buffer position <i>p</i>
        ''' corresponds to array index <i>p</i>&nbsp;+&nbsp;<tt>arrayOffset()</tt>.
        '''
        ''' <p> Invoke the <seealso cref="#hasArray hasArray"/> method before invoking this
        ''' method in order to ensure that this buffer has an accessible backing
        ''' array.  </p>
        ''' </summary>
        ''' <returns>  The offset within this buffer's array
        '''          of the first element of the buffer
        ''' </returns>
        ''' <exception cref="ReadOnlyBufferException">
        '''          If this buffer is backed by an array but is read-only
        ''' </exception>
        ''' <exception cref="UnsupportedOperationException">
        '''          If this buffer is not backed by an accessible array </exception>
        Public Function arrayOffset() As Integer
            If hb Is Nothing Then Throw New UnsupportedOperationException
            If isReadOnly Then Throw New ReadOnlyBufferException
            Return offset
        End Function

        ''' <summary>
        ''' Compacts this buffer&nbsp;&nbsp;<i>(optional operation)</i>.
        '''
        ''' <p> The chars between the buffer's current position and its limit,
        ''' if any, are copied to the beginning of the buffer.  That is, the
        ''' char at index <i>p</i>&nbsp;=&nbsp;<tt>position()</tt> is copied
        ''' to index zero, the char at index <i>p</i>&nbsp;+&nbsp;1 is copied
        ''' to index one, and so forth until the char at index
        ''' <tt>limit()</tt>&nbsp;-&nbsp;1 is copied to index
        ''' <i>n</i>&nbsp;=&nbsp;<tt>limit()</tt>&nbsp;-&nbsp;<tt>1</tt>&nbsp;-&nbsp;<i>p</i>.
        ''' The buffer's position is then set to <i>n+1</i> and its limit is set to
        ''' its capacity.  The mark, if defined, is discarded.
        '''
        ''' <p> The buffer's position is set to the number of chars copied,
        ''' rather than to zero, so that an invocation of this method can be
        ''' followed immediately by an invocation of another relative <i>put</i>
        ''' method. </p>
        '''
        '''
        '''
        '''
        '''
        '''
        '''
        '''
        '''
        '''
        '''
        '''
        '''
        '''
        '''
        '''
        '''
        ''' </summary>
        ''' <returns>  This buffer
        ''' </returns>
        ''' <exception cref="ReadOnlyBufferException">
        '''          If this buffer is read-only </exception>
        Public MustOverride Function compact() As CharBuffer

        ''' <summary>
        ''' Tells whether or not this char buffer is direct.
        ''' </summary>
        ''' <returns>  <tt>true</tt> if, and only if, this buffer is direct </returns>
        Public MustOverride Function direct() As Boolean


























        ''' <summary>
        ''' Returns the current hash code of this buffer.
        '''
        ''' <p> The hash code of a char buffer depends only upon its remaining
        ''' elements; that is, upon the elements from <tt>position()</tt> up to, and
        ''' including, the element at <tt>limit()</tt>&nbsp;-&nbsp;<tt>1</tt>.
        '''
        ''' <p> Because buffer hash codes are content-dependent, it is inadvisable
        ''' to use buffers as keys in hash maps or similar data structures unless it
        ''' is known that their contents will not change.  </p>
        ''' </summary>
        ''' <returns>  The current hash code of this buffer </returns>
        Public Overrides Function GetHashCode() As Integer
            Dim h As Integer = 1
            Dim p As Integer = position()
            For i As Integer = limit() - 1 To p Step -1



                h = 31 * h + AscW([get](i))
            Next i

            Return h
        End Function
        ''' <summary>
        ''' Tells whether or not this buffer is equal to another object.
        '''
        ''' <p> Two char buffers are equal if, and only if,
        '''
        ''' <ol>
        '''
        '''   <li><p> They have the same element type,  </p></li>
        '''
        '''   <li><p> They have the same number of remaining elements, and
        '''   </p></li>
        '''
        '''   <li><p> The two sequences of remaining elements, considered
        '''   independently of their starting positions, are pointwise equal.
        '''
        '''
        '''
        '''
        '''
        '''
        '''
        '''   </p></li>
        '''
        ''' </ol>
        '''
        ''' <p> A char buffer is not equal to any other type of object.  </p>
        ''' </summary>
        ''' <param name="ob">  The object to which this buffer is to be compared
        ''' </param>
        ''' <returns>  <tt>true</tt> if, and only if, this buffer is equal to the
        '''           given object </returns>
        Public Function Equals(ob As Object) As Boolean
            If Me Is ob Then Return True
            If Not (TypeOf ob Is CharBuffer) Then Return False
            Dim that As CharBuffer = CType(ob, CharBuffer)
            If Me.remaining() <> that.remaining() Then Return False
            Dim p As Integer = Me.position()
            Dim i As Integer = Me.limit() - 1
            Dim j As Integer = that.limit() - 1
            Do While i >= p
                If Not Equals(Me.get(i), that.get(j)) Then Return False
                i -= 1
                j -= 1
            Loop
            Return True
        End Function

        Private Overloads Shared Function Equals(x As Char, y As Char) As Boolean
            Return x = y
        End Function

        ''' <summary>
        ''' Compares this buffer to another.
        '''
        ''' <p> Two char buffers are compared by comparing their sequences of
        ''' remaining elements lexicographically, without regard to the starting
        ''' position of each sequence within its corresponding buffer.
        '''
        '''
        '''
        '''
        '''
        '''
        '''
        '''
        ''' Pairs of {@code char} elements are compared as if by invoking
        ''' <seealso cref="Character#compare(char,char)"/>.
        '''
        '''
        ''' <p> A char buffer is not comparable to any other type of object.
        ''' </summary>
        ''' <returns>  A negative integer, zero, or a positive integer as this buffer
        '''          is less than, equal to, or greater than the given buffer </returns>
        Public Function compareTo(that As CharBuffer) As Integer
            Dim n As Integer = Me.position() + System.Math.Min(Me.remaining(), that.remaining())
            Dim i As Integer = Me.position()
            Dim j As Integer = that.position()
            Do While i < n
                Dim cmp As Integer = Compare(Me.get(i), that.get(j))
                If cmp <> 0 Then Return cmp
                i += 1
                j += 1
            Loop
            Return Me.remaining() - that.remaining()
        End Function

        Private Shared Function compare(x As Char, y As Char) As Integer






            Return Character.Compare(x, y)
        End Function

        ' -- Other char stuff --



        ''' <summary>
        ''' Returns a string containing the characters in this buffer.
        '''
        ''' <p> The first character of the resulting string will be the character at
        ''' this buffer's position, while the last character will be the character
        ''' at index <tt>limit()</tt>&nbsp;-&nbsp;1.  Invoking this method does not
        ''' change the buffer's position. </p>
        ''' </summary>
        ''' <returns>  The specified string </returns>
        Public Overrides Function ToString() As String
            Return ToString(position(), limit())
        End Function

        Public MustOverride Overloads Function ToString(start As Integer, [end] As Integer) As String  ' package-private


        ' --- Methods to support CharSequence ---

        ''' <summary>
        ''' Returns the length of this character buffer.
        '''
        ''' <p> When viewed as a character sequence, the length of a character
        ''' buffer is simply the number of characters between the position
        ''' (inclusive) and the limit (exclusive); that is, it is equivalent to
        ''' <tt>remaining()</tt>. </p>
        ''' </summary>
        ''' <returns>  The length of this character buffer </returns>
        Public Function length() As Integer
            Return remaining()
        End Function

        ''' <summary>
        ''' Reads the character at the given index relative to the current
        ''' position.
        ''' </summary>
        ''' <param name="index">
        '''         The index of the character to be read, relative to the position;
        '''         must be non-negative and smaller than <tt>remaining()</tt>
        ''' </param>
        ''' <returns>  The character at index
        '''          <tt>position()&nbsp;+&nbsp;index</tt>
        ''' </returns>
        ''' <exception cref="IndexOutOfBoundsException">
        '''          If the preconditions on <tt>index</tt> do not hold </exception>
        Public Function charAt(index As Integer) As Char Implements CharSequence.charAt
            Return [get](position() + checkIndex(index, 1))
        End Function

        ''' <summary>
        ''' Creates a new character buffer that represents the specified subsequence
        ''' of this buffer, relative to the current position.
        '''
        ''' <p> The new buffer will share this buffer's content; that is, if the
        ''' content of this buffer is mutable then modifications to one buffer will
        ''' cause the other to be modified.  The new buffer's capacity will be that
        ''' of this buffer, its position will be
        ''' <tt>position()</tt>&nbsp;+&nbsp;<tt>start</tt>, and its limit will be
        ''' <tt>position()</tt>&nbsp;+&nbsp;<tt>end</tt>.  The new buffer will be
        ''' direct if, and only if, this buffer is direct, and it will be read-only
        ''' if, and only if, this buffer is read-only.  </p>
        ''' </summary>
        ''' <param name="start">
        '''         The index, relative to the current position, of the first
        '''         character in the subsequence; must be non-negative and no larger
        '''         than <tt>remaining()</tt>
        ''' </param>
        ''' <param name="end">
        '''         The index, relative to the current position, of the character
        '''         following the last character in the subsequence; must be no
        '''         smaller than <tt>start</tt> and no larger than
        '''         <tt>remaining()</tt>
        ''' </param>
        ''' <returns>  The new character buffer
        ''' </returns>
        ''' <exception cref="IndexOutOfBoundsException">
        '''          If the preconditions on <tt>start</tt> and <tt>end</tt>
        '''          do not hold </exception>
        Public MustOverride Function subSequence(start As Integer, [End] As Integer) As CharBuffer


        ' --- Methods to support Appendable ---

        ''' <summary>
        ''' Appends the specified character sequence  to this
        ''' buffer&nbsp;&nbsp;<i>(optional operation)</i>.
        '''
        ''' <p> An invocation of this method of the form <tt>dst.append(csq)</tt>
        ''' behaves in exactly the same way as the invocation
        '''
        ''' <pre>
        '''     dst.put(csq.toString()) </pre>
        '''
        ''' <p> Depending on the specification of <tt>toString</tt> for the
        ''' character sequence <tt>csq</tt>, the entire sequence may not be
        ''' appended.  For instance, invoking the {@link CharBuffer#toString()
        ''' toString} method of a character buffer will return a subsequence whose
        ''' content depends upon the buffer's position and limit.
        ''' </summary>
        ''' <param name="csq">
        '''         The character sequence to append.  If <tt>csq</tt> is
        '''         <tt>null</tt>, then the four characters <tt>"null"</tt> are
        '''         appended to this character buffer.
        ''' </param>
        ''' <returns>  This buffer
        ''' </returns>
        ''' <exception cref="BufferOverflowException">
        '''          If there is insufficient space in this buffer
        ''' </exception>
        ''' <exception cref="ReadOnlyBufferException">
        '''          If this buffer is read-only
        '''
        ''' @since  1.5 </exception>
        Public Function append(csq As CharSequence) As CharBuffer
            If csq Is Nothing Then
                Return put("null")
            Else
                Return put(csq.ToString())
            End If
        End Function

        ''' <summary>
        ''' Appends a subsequence of the  specified character sequence  to this
        ''' buffer&nbsp;&nbsp;<i>(optional operation)</i>.
        '''
        ''' <p> An invocation of this method of the form <tt>dst.append(csq, start,
        ''' end)</tt> when <tt>csq</tt> is not <tt>null</tt>, behaves in exactly the
        ''' same way as the invocation
        '''
        ''' <pre>
        '''     dst.put(csq.subSequence(start, end).toString()) </pre>
        ''' </summary>
        ''' <param name="csq">
        '''         The character sequence from which a subsequence will be
        '''         appended.  If <tt>csq</tt> is <tt>null</tt>, then characters
        '''         will be appended as if <tt>csq</tt> contained the four
        '''         characters <tt>"null"</tt>.
        ''' </param>
        ''' <returns>  This buffer
        ''' </returns>
        ''' <exception cref="BufferOverflowException">
        '''          If there is insufficient space in this buffer
        ''' </exception>
        ''' <exception cref="IndexOutOfBoundsException">
        '''          If <tt>start</tt> or <tt>end</tt> are negative, <tt>start</tt>
        '''          is greater than <tt>end</tt>, or <tt>end</tt> is greater than
        '''          <tt>csq.length()</tt>
        ''' </exception>
        ''' <exception cref="ReadOnlyBufferException">
        '''          If this buffer is read-only
        '''
        ''' @since  1.5 </exception>
        Public Function append(csq As CharSequence, start As Integer, [End] As Integer) As CharBuffer
            Dim cs As CharSequence = (If(csq Is Nothing, "null", csq))
            Return put(cs.subSequence(start, end).ToString())
        End Function

        ''' <summary>
        ''' Appends the specified char  to this
        ''' buffer&nbsp;&nbsp;<i>(optional operation)</i>.
        '''
        ''' <p> An invocation of this method of the form <tt>dst.append(c)</tt>
        ''' behaves in exactly the same way as the invocation
        '''
        ''' <pre>
        '''     dst.put(c) </pre>
        ''' </summary>
        ''' <param name="c">
        '''         The 16-bit char to append
        ''' </param>
        ''' <returns>  This buffer
        ''' </returns>
        ''' <exception cref="BufferOverflowException">
        '''          If there is insufficient space in this buffer
        ''' </exception>
        ''' <exception cref="ReadOnlyBufferException">
        '''          If this buffer is read-only
        '''
        ''' @since  1.5 </exception>
        Public Function append(c As Char) As CharBuffer
            Return put(c)
        End Function



        ' -- Other byte stuff: Access to binary data --



        ''' <summary>
        ''' Retrieves this buffer's byte order.
        '''
        ''' <p> The byte order of a char buffer created by allocation or by
        ''' wrapping an existing <tt>char</tt> array is the {@link
        ''' ByteOrder#nativeOrder native order} of the underlying
        ''' hardware.  The byte order of a char buffer created as a <a
        ''' href="ByteBuffer.html#views">view</a> of a byte buffer is that of the
        ''' byte buffer at the moment that the view is created.  </p>
        ''' </summary>
        ''' <returns>  This buffer's byte order </returns>
        Public MustOverride Function order() As ByteOrder



        Public Function chars() As java.util.stream.IntStream
            '   Return java.util.stream.StreamSupport.intStream(() -> New CharBufferSpliterator(Me), Buffer.SPLITERATOR_CHARACTERISTICS, False)
        End Function
    End Class
End Namespace