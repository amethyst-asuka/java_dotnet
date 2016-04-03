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

Namespace java.lang


    ''' <summary>
    ''' A <tt>CharSequence</tt> is a readable sequence of <code>char</code> values. This
    ''' interface provides uniform, read-only access to many different kinds of
    ''' <code>char</code> sequences.
    ''' A <code>char</code> value represents a character in the <i>Basic
    ''' Multilingual Plane (BMP)</i> or a surrogate. Refer to <a
    ''' href="Character.html#unicode">Unicode Character Representation</a> for details.
    '''
    ''' <p> This interface does not refine the general contracts of the {@link
    ''' java.lang.Object#equals(java.lang.Object) equals} and {@link
    ''' java.lang.Object#hashCode() hashCode} methods.  The result of comparing two
    ''' objects that implement <tt>CharSequence</tt> is therefore, in general,
    ''' undefined.  Each object may be implemented by a different [Class], and there
    ''' is no guarantee that each class will be capable of testing its instances
    ''' for equality with those of the other.  It is therefore inappropriate to use
    ''' arbitrary <tt>CharSequence</tt> instances as elements in a set or as keys in
    ''' a map. </p>
    '''
    ''' @author Mike McCloskey
    ''' @since 1.4
    ''' @spec JSR-51
    ''' </summary>

    Public Interface CharSequence

        ''' <summary>
        ''' Returns the length of this character sequence.  The length is the number
        ''' of 16-bit <code>char</code>s in the sequence.
        ''' </summary>
        ''' <returns>  the number of <code>char</code>s in this sequence </returns>
        Function length() As Integer

        ''' <summary>
        ''' Returns the <code>char</code> value at the specified index.  An index ranges from zero
        ''' to <tt>length() - 1</tt>.  The first <code>char</code> value of the sequence is at
        ''' index zero, the next at index one, and so on, as for array
        ''' indexing.
        '''
        ''' <p>If the <code>char</code> value specified by the index is a
        ''' <a href="{@docRoot}/java/lang/Character.html#unicode">surrogate</a>, the surrogate
        ''' value is returned.
        ''' </summary>
        ''' <param name="index">   the index of the <code>char</code> value to be returned
        ''' </param>
        ''' <returns>  the specified <code>char</code> value
        ''' </returns>
        ''' <exception cref="IndexOutOfBoundsException">
        '''          if the <tt>index</tt> argument is negative or not less than
        '''          <tt>length()</tt> </exception>
        Function charAt(ByVal index As Integer) As Char

        ''' <summary>
        ''' Returns a <code>CharSequence</code> that is a subsequence of this sequence.
        ''' The subsequence starts with the <code>char</code> value at the specified index and
        ''' ends with the <code>char</code> value at index <tt>end - 1</tt>.  The length
        ''' (in <code>char</code>s) of the
        ''' returned sequence is <tt>end - start</tt>, so if <tt>start == end</tt>
        ''' then an empty sequence is returned.
        ''' </summary>
        ''' <param name="start">   the start index, inclusive </param>
        ''' <param name="end">     the end index, exclusive
        ''' </param>
        ''' <returns>  the specified subsequence
        ''' </returns>
        ''' <exception cref="IndexOutOfBoundsException">
        '''          if <tt>start</tt> or <tt>end</tt> are negative,
        '''          if <tt>end</tt> is greater than <tt>length()</tt>,
        '''          or if <tt>start</tt> is greater than <tt>end</tt> </exception>
        Function subSequence(ByVal start As Integer, ByVal [end] As Integer) As CharSequence

        ''' <summary>
        ''' Returns a string containing the characters in this sequence in the same
        ''' order as this sequence.  The length of the string will be the length of
        ''' this sequence.
        ''' </summary>
        ''' <returns>  a string consisting of exactly this sequence of characters </returns>
        Function ToString() As String

        ''' <summary>
        ''' Returns a stream of {@code int} zero-extending the {@code char} values
        ''' from this sequence.  Any char which maps to a <a
        ''' href="{@docRoot}/java/lang/Character.html#unicode">surrogate code
        ''' point</a> is passed through uninterpreted.
        '''
        ''' <p>If the sequence is mutated while the stream is being read, the
        ''' result is undefined.
        ''' </summary>
        ''' <returns> an IntStream of char values from this sequence
        ''' @since 1.8 </returns>
        Function chars() As java.util.stream.IntStream
        'JAVA TO VB CONVERTER TODO TASK: Interface inner types are not converted:
        '			class CharIterator implements java.util.PrimitiveIterator.OfInt
        '		{
        '			int cur = 0;
        '
        '			public boolean hasNext()
        '			{
        '				Return cur < length();
        '			}
        '
        '			public int nextInt()
        '			{
        '				if (hasNext())
        '				{
        '					Return charAt(cur++);
        '				}
        '				else
        '				{
        '					throw New NoSuchElementException();
        '				}
        '			}
        '
        '			@Override public  Sub  forEachRemaining(IntConsumer block)
        '			{
        '				for (; cur < length(); cur += 1)
        '				{
        '					block.accept(charAt(cur));
        '				}
        '			}
        '		}

        '	Return java.util.stream.StreamSupport.intStream(() -> java.util.Spliterators.spliterator(New CharIterator, length(), java.util.Spliterator.ORDERED), java.util.Spliterator.SUBSIZED | java.util.Spliterator.SIZED | java.util.Spliterator.ORDERED, False);

        ''' <summary>
        ''' Returns a stream of code point values from this sequence.  Any surrogate
        ''' pairs encountered in the sequence are combined as if by {@linkplain
        ''' Character#toCodePoint Character.toCodePoint} and the result is passed
        ''' to the stream. Any other code units, including ordinary BMP characters,
        ''' unpaired surrogates, and undefined code units, are zero-extended to
        ''' {@code int} values which are then passed to the stream.
        '''
        ''' <p>If the sequence is mutated while the stream is being read, the result
        ''' is undefined.
        ''' </summary>
        ''' <returns> an IntStream of Unicode code points from this sequence
        ''' @since 1.8 </returns>
        Function codePoints() As java.util.stream.IntStream

        '			class CodePointIterator implements java.util.PrimitiveIterator.OfInt
        '		{
        '			int cur = 0;
        '
        '			@Override public  Sub  forEachRemaining(IntConsumer block)
        '			{
        '				final int length = length();
        '				int i = cur;
        '				try
        '				{
        '					while (i < length)
        '					{
        '						char c1 = charAt(i);
        '						i += 1;
        '						if (!Character.isHighSurrogate(c1) || i >= length)
        '						{
        '							block.accept(c1);
        '						}
        '						else
        '						{
        '							char c2 = charAt(i);
        '							if (Character.isLowSurrogate(c2))
        '							{
        '								i += 1;
        '								block.accept(Character.toCodePoint(c1, c2));
        '							}
        '							else
        '							{
        '								block.accept(c1);
        '							}
        '						}
        '					}
        '				}
        '				finally
        '				{
        '					cur = i;
        '				}
        '			}
        '
        '			public boolean hasNext()
        '			{
        '				Return cur < length();
        '			}
        '
        '			public int nextInt()
        '			{
        '				final int length = length();
        '
        '				if (cur >= length)
        '				{
        '					throw New NoSuchElementException();
        '				}
        '				char c1 = charAt(cur);
        '				cur += 1;
        '				if (Character.isHighSurrogate(c1) && cur < length)
        '				{
        '					char c2 = charAt(cur);
        '					if (Character.isLowSurrogate(c2))
        '					{
        '						cur += 1;
        '						Return Character.toCodePoint(c1, c2);
        '					}
        '				}
        '				Return c1;
        '			}
        '		}

        'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        ' Return java.util.stream.StreamSupport.intStream(() -> java.util.Spliterators.spliteratorUnknownSize(New CodePointIterator, java.util.Spliterator.ORDERED), java.util.Spliterator.ORDERED, False);
    End Interface

End Namespace