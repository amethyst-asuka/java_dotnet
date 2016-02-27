Imports Microsoft.VisualBasic
Imports System
Imports java.lang

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
    ''' A double buffer.
    ''' 
    ''' <p> This class defines four categories of operations upon
    ''' double buffers:
    ''' 
    ''' <ul>
    ''' 
    '''   <li><p> Absolute and relative <seealso cref="#get() <i>get</i>"/> and
    '''   <seealso cref="#put(double) <i>put</i>"/> methods that read and write
    '''   single doubles; </p></li>
    ''' 
    '''   <li><p> Relative <seealso cref="#get(double[]) <i>bulk get</i>"/>
    '''   methods that transfer contiguous sequences of doubles from this buffer
    '''   into an array; and</p></li>
    ''' 
    '''   <li><p> Relative <seealso cref="#put(double[]) <i>bulk put</i>"/>
    '''   methods that transfer contiguous sequences of doubles from a
    '''   double array or some other double
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
    '''   a double buffer.  </p></li>
    ''' 
    ''' </ul>
    ''' 
    ''' <p> Double buffers can be created either by {@link #allocate
    ''' <i>allocation</i>}, which allocates space for the buffer's
    ''' 
    ''' 
    ''' 
    ''' 
    ''' 
    ''' 
    ''' 
    ''' 
    ''' content, by <seealso cref="#wrap(double[]) <i>wrapping</i>"/> an existing
    ''' double array  into a buffer, or by creating a
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
    ''' <p> Like a byte buffer, a double buffer is either <a
    ''' href="ByteBuffer.html#direct"><i>direct</i> or <i>non-direct</i></a>.  A
    ''' double buffer created via the <tt>wrap</tt> methods of this class will
    ''' be non-direct.  A double buffer created as a view of a byte buffer will
    ''' be direct if, and only if, the byte buffer itself is direct.  Whether or not
    ''' a double buffer is direct may be determined by invoking the {@link
    ''' #isDirect isDirect} method.  </p>
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
    ''' @author Mark Reinhold
    ''' @author JSR-51 Expert Group
    ''' @since 1.4
    ''' </summary>

    Public MustInherit Class DoubleBuffer
        Inherits Buffer
        Implements Comparable(Of DoubleBuffer)

        ' These fields are declared here rather than in Heap-X-Buffer in order to
        ' reduce the number of virtual method invocations needed to access these
        ' values, which is especially costly when coding small buffers.
        '
        Friend ReadOnly hb As Double() ' Non-null only for heap buffers
        Friend ReadOnly offset As Integer
        Friend isReadOnly As Boolean ' Valid only for heap buffers

        ' Creates a new buffer with the given mark, position, limit, capacity,
        ' backing array, and array offset
        '
        Friend Sub New(ByVal mark As Integer, ByVal pos As Integer, ByVal lim As Integer, ByVal cap As Integer, ByVal hb As Double(), ByVal offset As Integer) ' package-private
            MyBase.New(mark, pos, lim, cap)
            Me.hb = hb
            Me.offset = offset
        End Sub

        ' Creates a new buffer with the given mark, position, limit, and capacity
        '
        Friend Sub New(ByVal mark As Integer, ByVal pos As Integer, ByVal lim As Integer, ByVal cap As Integer) ' package-private
            Me.New(mark, pos, lim, cap, Nothing, 0)
        End Sub

























        ''' <summary>
        ''' Allocates a new double buffer.
        ''' 
        ''' <p> The new buffer's position will be zero, its limit will be its
        ''' capacity, its mark will be undefined, and each of its elements will be
        ''' initialized to zero.  It will have a <seealso cref="#array backing array"/>,
        ''' and its <seealso cref="#arrayOffset array offset"/> will be zero.
        ''' </summary>
        ''' <param name="capacity">
        '''         The new buffer's capacity, in doubles
        ''' </param>
        ''' <returns>  The new double buffer
        ''' </returns>
        ''' <exception cref="IllegalArgumentException">
        '''          If the <tt>capacity</tt> is a negative integer </exception>
        Public Shared Function allocate(ByVal capacity As Integer) As DoubleBuffer
            If capacity < 0 Then Throw New IllegalArgumentException
            Return New HeapDoubleBuffer(capacity, capacity)
        End Function

        ''' <summary>
        ''' Wraps a double array into a buffer.
        ''' 
        ''' <p> The new buffer will be backed by the given double array;
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
        ''' <returns>  The new double buffer
        ''' </returns>
        ''' <exception cref="IndexOutOfBoundsException">
        '''          If the preconditions on the <tt>offset</tt> and <tt>length</tt>
        '''          parameters do not hold </exception>
        Public Shared Function wrap(ByVal array As Double(), ByVal offset As Integer, ByVal length As Integer) As DoubleBuffer
            Try
                Return New HeapDoubleBuffer(array, offset, length)
            Catch x As IllegalArgumentException
                Throw New IndexOutOfBoundsException
            End Try
        End Function

        ''' <summary>
        ''' Wraps a double array into a buffer.
        ''' 
        ''' <p> The new buffer will be backed by the given double array;
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
        ''' <returns>  The new double buffer </returns>
        Public Shared Function wrap(ByVal array As Double()) As DoubleBuffer
            Return wrap(array, 0, array.Length)
        End Function






























































































        ''' <summary>
        ''' Creates a new double buffer whose content is a shared subsequence of
        ''' this buffer's content.
        ''' 
        ''' <p> The content of the new buffer will start at this buffer's current
        ''' position.  Changes to this buffer's content will be visible in the new
        ''' buffer, and vice versa; the two buffers' position, limit, and mark
        ''' values will be independent.
        ''' 
        ''' <p> The new buffer's position will be zero, its capacity and its limit
        ''' will be the number of doubles remaining in this buffer, and its mark
        ''' will be undefined.  The new buffer will be direct if, and only if, this
        ''' buffer is direct, and it will be read-only if, and only if, this buffer
        ''' is read-only.  </p>
        ''' </summary>
        ''' <returns>  The new double buffer </returns>
        Public MustOverride Function slice() As DoubleBuffer

        ''' <summary>
        ''' Creates a new double buffer that shares this buffer's content.
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
        ''' <returns>  The new double buffer </returns>
        Public MustOverride Function duplicate() As DoubleBuffer

        ''' <summary>
        ''' Creates a new, read-only double buffer that shares this buffer's
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
        ''' <returns>  The new, read-only double buffer </returns>
        Public MustOverride Function asReadOnlyBuffer() As DoubleBuffer


        ' -- Singleton get/put methods --

        ''' <summary>
        ''' Relative <i>get</i> method.  Reads the double at this buffer's
        ''' current position, and then increments the position.
        ''' </summary>
        ''' <returns>  The double at the buffer's current position
        ''' </returns>
        ''' <exception cref="BufferUnderflowException">
        '''          If the buffer's current position is not smaller than its limit </exception>
        Public MustOverride Function [get]() As Double

        ''' <summary>
        ''' Relative <i>put</i> method&nbsp;&nbsp;<i>(optional operation)</i>.
        ''' 
        ''' <p> Writes the given double into this buffer at the current
        ''' position, and then increments the position. </p>
        ''' </summary>
        ''' <param name="d">
        '''         The double to be written
        ''' </param>
        ''' <returns>  This buffer
        ''' </returns>
        ''' <exception cref="BufferOverflowException">
        '''          If this buffer's current position is not smaller than its limit
        ''' </exception>
        ''' <exception cref="ReadOnlyBufferException">
        '''          If this buffer is read-only </exception>
        Public MustOverride Function put(ByVal d As Double) As DoubleBuffer

        ''' <summary>
        ''' Absolute <i>get</i> method.  Reads the double at the given
        ''' index.
        ''' </summary>
        ''' <param name="index">
        '''         The index from which the double will be read
        ''' </param>
        ''' <returns>  The double at the given index
        ''' </returns>
        ''' <exception cref="IndexOutOfBoundsException">
        '''          If <tt>index</tt> is negative
        '''          or not smaller than the buffer's limit </exception>
        Public MustOverride Function [get](ByVal index As Integer) As Double














        ''' <summary>
        ''' Absolute <i>put</i> method&nbsp;&nbsp;<i>(optional operation)</i>.
        ''' 
        ''' <p> Writes the given double into this buffer at the given
        ''' index. </p>
        ''' </summary>
        ''' <param name="index">
        '''         The index at which the double will be written
        ''' </param>
        ''' <param name="d">
        '''         The double value to be written
        ''' </param>
        ''' <returns>  This buffer
        ''' </returns>
        ''' <exception cref="IndexOutOfBoundsException">
        '''          If <tt>index</tt> is negative
        '''          or not smaller than the buffer's limit
        ''' </exception>
        ''' <exception cref="ReadOnlyBufferException">
        '''          If this buffer is read-only </exception>
        Public MustOverride Function put(ByVal index As Integer, ByVal d As Double) As DoubleBuffer


        ' -- Bulk get operations --

        ''' <summary>
        ''' Relative bulk <i>get</i> method.
        ''' 
        ''' <p> This method transfers doubles from this buffer into the given
        ''' destination array.  If there are fewer doubles remaining in the
        ''' buffer than are required to satisfy the request, that is, if
        ''' <tt>length</tt>&nbsp;<tt>&gt;</tt>&nbsp;<tt>remaining()</tt>, then no
        ''' doubles are transferred and a <seealso cref="BufferUnderflowException"/> is
        ''' thrown.
        ''' 
        ''' <p> Otherwise, this method copies <tt>length</tt> doubles from this
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
        ''' except that it first checks that there are sufficient doubles in
        ''' this buffer and it is potentially much more efficient.
        ''' </summary>
        ''' <param name="dst">
        '''         The array into which doubles are to be written
        ''' </param>
        ''' <param name="offset">
        '''         The offset within the array of the first double to be
        '''         written; must be non-negative and no larger than
        '''         <tt>dst.length</tt>
        ''' </param>
        ''' <param name="length">
        '''         The maximum number of doubles to be written to the given
        '''         array; must be non-negative and no larger than
        '''         <tt>dst.length - offset</tt>
        ''' </param>
        ''' <returns>  This buffer
        ''' </returns>
        ''' <exception cref="BufferUnderflowException">
        '''          If there are fewer than <tt>length</tt> doubles
        '''          remaining in this buffer
        ''' </exception>
        ''' <exception cref="IndexOutOfBoundsException">
        '''          If the preconditions on the <tt>offset</tt> and <tt>length</tt>
        '''          parameters do not hold </exception>
        Public Overridable Function [get](ByVal dst As Double(), ByVal offset As Integer, ByVal length As Integer) As DoubleBuffer
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
        ''' <p> This method transfers doubles from this buffer into the given
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
        '''          If there are fewer than <tt>length</tt> doubles
        '''          remaining in this buffer </exception>
        Public Overridable Function [get](ByVal dst As Double()) As DoubleBuffer
            Return [get](dst, 0, dst.Length)
        End Function


        ' -- Bulk put operations --

        ''' <summary>
        ''' Relative bulk <i>put</i> method&nbsp;&nbsp;<i>(optional operation)</i>.
        ''' 
        ''' <p> This method transfers the doubles remaining in the given source
        ''' buffer into this buffer.  If there are more doubles remaining in the
        ''' source buffer than in this buffer, that is, if
        ''' <tt>src.remaining()</tt>&nbsp;<tt>&gt;</tt>&nbsp;<tt>remaining()</tt>,
        ''' then no doubles are transferred and a {@link
        ''' BufferOverflowException} is thrown.
        ''' 
        ''' <p> Otherwise, this method copies
        ''' <i>n</i>&nbsp;=&nbsp;<tt>src.remaining()</tt> doubles from the given
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
        '''         The source buffer from which doubles are to be read;
        '''         must not be this buffer
        ''' </param>
        ''' <returns>  This buffer
        ''' </returns>
        ''' <exception cref="BufferOverflowException">
        '''          If there is insufficient space in this buffer
        '''          for the remaining doubles in the source buffer
        ''' </exception>
        ''' <exception cref="IllegalArgumentException">
        '''          If the source buffer is this buffer
        ''' </exception>
        ''' <exception cref="ReadOnlyBufferException">
        '''          If this buffer is read-only </exception>
        Public Overridable Function put(ByVal src As DoubleBuffer) As DoubleBuffer
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
        ''' <p> This method transfers doubles into this buffer from the given
        ''' source array.  If there are more doubles to be copied from the array
        ''' than remain in this buffer, that is, if
        ''' <tt>length</tt>&nbsp;<tt>&gt;</tt>&nbsp;<tt>remaining()</tt>, then no
        ''' doubles are transferred and a <seealso cref="BufferOverflowException"/> is
        ''' thrown.
        ''' 
        ''' <p> Otherwise, this method copies <tt>length</tt> doubles from the
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
        '''         The array from which doubles are to be read
        ''' </param>
        ''' <param name="offset">
        '''         The offset within the array of the first double to be read;
        '''         must be non-negative and no larger than <tt>array.length</tt>
        ''' </param>
        ''' <param name="length">
        '''         The number of doubles to be read from the given array;
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
        Public Overridable Function put(ByVal src As Double(), ByVal offset As Integer, ByVal length As Integer) As DoubleBuffer
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
        ''' double array into this buffer.  An invocation of this method of the
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
        Public Function put(ByVal src As Double()) As DoubleBuffer
            Return put(src, 0, src.Length)
        End Function































































































        ' -- Other stuff --

        ''' <summary>
        ''' Tells whether or not this buffer is backed by an accessible double
        ''' array.
        ''' 
        ''' <p> If this method returns <tt>true</tt> then the <seealso cref="#array() array"/>
        ''' and <seealso cref="#arrayOffset() arrayOffset"/> methods may safely be invoked.
        ''' </p>
        ''' </summary>
        ''' <returns>  <tt>true</tt> if, and only if, this buffer
        '''          is backed by an array and is not read-only </returns>
        Public NotOverridable Overrides Function hasArray() As Boolean
            Return (hb IsNot Nothing) AndAlso Not isReadOnly
        End Function

        ''' <summary>
        ''' Returns the double array that backs this
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
        Public NotOverridable Overrides Function array() As Double()
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
        Public NotOverridable Overrides Function arrayOffset() As Integer
            If hb Is Nothing Then Throw New UnsupportedOperationException
            If isReadOnly Then Throw New ReadOnlyBufferException
            Return offset
        End Function

        ''' <summary>
        ''' Compacts this buffer&nbsp;&nbsp;<i>(optional operation)</i>.
        ''' 
        ''' <p> The doubles between the buffer's current position and its limit,
        ''' if any, are copied to the beginning of the buffer.  That is, the
        ''' double at index <i>p</i>&nbsp;=&nbsp;<tt>position()</tt> is copied
        ''' to index zero, the double at index <i>p</i>&nbsp;+&nbsp;1 is copied
        ''' to index one, and so forth until the double at index
        ''' <tt>limit()</tt>&nbsp;-&nbsp;1 is copied to index
        ''' <i>n</i>&nbsp;=&nbsp;<tt>limit()</tt>&nbsp;-&nbsp;<tt>1</tt>&nbsp;-&nbsp;<i>p</i>.
        ''' The buffer's position is then set to <i>n+1</i> and its limit is set to
        ''' its capacity.  The mark, if defined, is discarded.
        ''' 
        ''' <p> The buffer's position is set to the number of doubles copied,
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
        Public MustOverride Function compact() As DoubleBuffer

        ''' <summary>
        ''' Tells whether or not this double buffer is direct.
        ''' </summary>
        ''' <returns>  <tt>true</tt> if, and only if, this buffer is direct </returns>
        Public MustOverride ReadOnly Property direct As Boolean



        ''' <summary>
        ''' Returns a string summarizing the state of this buffer.
        ''' </summary>
        ''' <returns>  A summary string </returns>
        Public Overrides Function ToString() As String
            Dim sb As New StringBuffer
            sb.append(Me.GetType().Name)
            sb.append("[pos=")
            sb.append(position())
            sb.append(" lim=")
            sb.append(limit())
            sb.append(" cap=")
            sb.append(capacity())
            sb.append("]")
            Return sb.ToString()
        End Function






        ''' <summary>
        ''' Returns the current hash code of this buffer.
        ''' 
        ''' <p> The hash code of a double buffer depends only upon its remaining
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



                h = 31 * h + CInt(Fix([get](i)))
            Next i

            Return h
        End Function

        ''' <summary>
        ''' Tells whether or not this buffer is equal to another object.
        ''' 
        ''' <p> Two double buffers are equal if, and only if,
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
        '''   This method considers two double elements {@code a} and {@code b}
        '''   to be equal if
        '''   {@code (a == b) || (Double.isNaN(a) && java.lang.[Double].isNaN(b))}.
        '''   The values {@code -0.0} and {@code +0.0} are considered to be
        '''   equal, unlike <seealso cref="Double#equals(Object)"/>.
        ''' 
        '''   </p></li>
        ''' 
        ''' </ol>
        ''' 
        ''' <p> A double buffer is not equal to any other type of object.  </p>
        ''' </summary>
        ''' <param name="ob">  The object to which this buffer is to be compared
        ''' </param>
        ''' <returns>  <tt>true</tt> if, and only if, this buffer is equal to the
        '''           given object </returns>
        Public Overrides Function Equals(ByVal ob As Object) As Boolean
            If Me Is ob Then Return True
            If Not (TypeOf ob Is DoubleBuffer) Then Return False
            Dim that As DoubleBuffer = CType(ob, DoubleBuffer)
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

        Private Shared Function Equals(ByVal x As Double, ByVal y As Double) As Boolean

            Return (x = y) OrElse (Double.IsNaN(x) AndAlso java.lang.[Double].IsNaN(y))



        End Function

        ''' <summary>
        ''' Compares this buffer to another.
        ''' 
        ''' <p> Two double buffers are compared by comparing their sequences of
        ''' remaining elements lexicographically, without regard to the starting
        ''' position of each sequence within its corresponding buffer.
        ''' 
        ''' Pairs of {@code double} elements are compared as if by invoking
        ''' <seealso cref="Double#compare(double,double)"/>, except that
        ''' {@code -0.0} and {@code 0.0} are considered to be equal.
        ''' {@code java.lang.[Double].NaN} is considered by this method to be equal
        ''' to itself and greater than all other {@code double} values
        ''' (including {@code java.lang.[Double].POSITIVE_INFINITY}).
        ''' 
        ''' 
        ''' 
        ''' 
        ''' 
        ''' <p> A double buffer is not comparable to any other type of object.
        ''' </summary>
        ''' <returns>  A negative integer, zero, or a positive integer as this buffer
        '''          is less than, equal to, or greater than the given buffer </returns>
        Public Overridable Function compareTo(ByVal that As DoubleBuffer) As Integer Implements Comparable(Of DoubleBuffer).compareTo
            Dim n As Integer = Me.position() + System.Math.Min(Me.remaining(), that.remaining())
            Dim i As Integer = Me.position()
            Dim j As Integer = that.position()
            Do While i < n
                Dim cmp As Integer = compare(Me.get(i), that.get(j))
                If cmp <> 0 Then Return cmp
                i += 1
                j += 1
            Loop
            Return Me.remaining() - that.remaining()
        End Function

        Private Shared Function compare(ByVal x As Double, ByVal y As Double) As Integer

            Return (If(x < y, -1, If(x > y, +1, If(x = y, 0, If(Double.IsNaN(x), (If(Double.IsNaN(y), 0, +1)), -1)))))



        End Function

        ' -- Other char stuff --


































































































































































































        ' -- Other byte stuff: Access to binary data --



        ''' <summary>
        ''' Retrieves this buffer's byte order.
        ''' 
        ''' <p> The byte order of a double buffer created by allocation or by
        ''' wrapping an existing <tt>double</tt> array is the {@link
        ''' ByteOrder#nativeOrder native order} of the underlying
        ''' hardware.  The byte order of a double buffer created as a <a
        ''' href="ByteBuffer.html#views">view</a> of a byte buffer is that of the
        ''' byte buffer at the moment that the view is created.  </p>
        ''' </summary>
        ''' <returns>  This buffer's byte order </returns>
        Public MustOverride Function order() As ByteOrder

































































    End Class

End Namespace