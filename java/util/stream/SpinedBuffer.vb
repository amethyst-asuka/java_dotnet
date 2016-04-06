Imports System
Imports System.Diagnostics
Imports System.Collections.Generic

'
' * Copyright (c) 2012, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.util.stream


	''' <summary>
	''' An ordered collection of elements.  Elements can be added, but not removed.
	''' Goes through a building phase, during which elements can be added, and a
	''' traversal phase, during which elements can be traversed in order but no
	''' further modifications are possible.
	''' 
	''' <p> One or more arrays are used to store elements. The use of a multiple
	''' arrays has better performance characteristics than a single array used by
	''' <seealso cref="ArrayList"/>, as when the capacity of the list needs to be increased
	''' no copying of elements is required.  This is usually beneficial in the case
	''' where the results will be traversed a small number of times.
	''' </summary>
	''' @param <E> the type of elements in this list
	''' @since 1.8 </param>
	Friend Class SpinedBuffer(Of E)
		Inherits AbstractSpinedBuffer
		Implements java.util.function.Consumer(Of E), Iterable(Of E)

	'    
	'     * We optimistically hope that all the data will fit into the first chunk,
	'     * so we try to avoid inflating the spine[] and priorElementCount[] arrays
	'     * prematurely.  So methods must be prepared to deal with these arrays being
	'     * null.  If spine is non-null, then spineIndex points to the current chunk
	'     * within the spine, otherwise it is zero.  The spine and priorElementCount
	'     * arrays are always the same size, and for any i <= spineIndex,
	'     * priorElementCount[i] is the sum of the sizes of all the prior chunks.
	'     *
	'     * The curChunk pointer is always valid.  The elementIndex is the index of
	'     * the next element to be written in curChunk; this may be past the end of
	'     * curChunk so we have to check before writing. When we inflate the spine
	'     * array, curChunk becomes the first element in it.  When we clear the
	'     * buffer, we discard all chunks except the first one, which we clear,
	'     * restoring it to the initial single-chunk state.
	'     

		''' <summary>
		''' Chunk that we're currently writing into; may or may not be aliased with
		''' the first element of the spine.
		''' </summary>
		Protected Friend curChunk As E()

		''' <summary>
		''' All chunks, or null if there is only one chunk.
		''' </summary>
		Protected Friend spine As E()()

		''' <summary>
		''' Constructs an empty list with the specified initial capacity.
		''' </summary>
		''' <param name="initialCapacity">  the initial capacity of the list </param>
		''' <exception cref="IllegalArgumentException"> if the specified initial capacity
		'''         is negative </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Sub New(  initialCapacity As Integer)
			MyBase.New(initialCapacity)
			curChunk = CType(New Object(1 << initialChunkPower - 1){}, E())
		End Sub

		''' <summary>
		''' Constructs an empty list with an initial capacity of sixteen.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Sub New()
			MyBase.New()
			curChunk = CType(New Object(1 << initialChunkPower - 1){}, E())
		End Sub

		''' <summary>
		''' Returns the current capacity of the buffer
		''' </summary>
		Protected Friend Overridable Function capacity() As Long
			Return If(spineIndex = 0, curChunk.Length, priorElementCount(spineIndex) + spine(spineIndex).Length)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Sub inflateSpine()
			If spine Is Nothing Then
				spine = CType(New Object(MIN_SPINE_SIZE - 1)(){}, E()())
				priorElementCount = New Long(MIN_SPINE_SIZE - 1){}
				spine(0) = curChunk
			End If
		End Sub

		''' <summary>
		''' Ensure that the buffer has at least capacity to hold the target size
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Protected Friend Sub ensureCapacity(  targetSize As Long)
			Dim capacity As Long = capacity()
			If targetSize > capacity Then
				inflateSpine()
				Dim i As Integer=spineIndex+1
				Do While targetSize > capacity
					If i >= spine.Length Then
						Dim newSpineSize As Integer = spine.Length * 2
						spine = java.util.Arrays.copyOf(spine, newSpineSize)
						priorElementCount = java.util.Arrays.copyOf(priorElementCount, newSpineSize)
					End If
					Dim nextChunkSize As Integer = chunkSize(i)
					spine(i) = CType(New Object(nextChunkSize - 1){}, E())
					priorElementCount(i) = priorElementCount(i-1) + spine(i-1).Length
					capacity += nextChunkSize
					i += 1
				Loop
			End If
		End Sub

		''' <summary>
		''' Force the buffer to increase its capacity.
		''' </summary>
		Protected Friend Overridable Sub increaseCapacity()
			ensureCapacity(capacity() + 1)
		End Sub

		''' <summary>
		''' Retrieve the element at the specified index.
		''' </summary>
		Public Overridable Function [get](  index As Long) As E
			' @@@ can further optimize by caching last seen spineIndex,
			' which is going to be right most of the time

			' Casts to int are safe since the spine array index is the index minus
			' the prior element count from the current spine
			If spineIndex = 0 Then
				If index < elementIndex Then
					Return curChunk((CInt(index)))
				Else
					Throw New IndexOutOfBoundsException(Convert.ToString(index))
				End If
			End If

			If index >= count() Then Throw New IndexOutOfBoundsException(Convert.ToString(index))

			For j As Integer = 0 To spineIndex
				If index < priorElementCount(j) + spine(j).Length Then Return spine(j)((CInt(index - priorElementCount(j))))
			Next j

			Throw New IndexOutOfBoundsException(Convert.ToString(index))
		End Function

		''' <summary>
		''' Copy the elements, starting at the specified offset, into the specified
		''' array.
		''' </summary>
		Public Overridable Sub copyInto(  array As E(),   offset As Integer)
			Dim finalOffset As Long = offset + count()
			If finalOffset > array.Length OrElse finalOffset < offset Then Throw New IndexOutOfBoundsException("does not fit")

			If spineIndex = 0 Then
				Array.Copy(curChunk, 0, array, offset, elementIndex)
			Else
				' full chunks
				For i As Integer = 0 To spineIndex - 1
					Array.Copy(spine(i), 0, array, offset, spine(i).Length)
					offset += spine(i).Length
				Next i
				If elementIndex > 0 Then Array.Copy(curChunk, 0, array, offset, elementIndex)
			End If
		End Sub

		''' <summary>
		''' Create a new array using the specified array factory, and copy the
		''' elements into it.
		''' </summary>
		Public Overridable Function asArray(  arrayFactory As java.util.function.IntFunction(Of E())) As E()
			Dim size As Long = count()
			If size >= Nodes.MAX_ARRAY_SIZE Then Throw New IllegalArgumentException(Nodes.BAD_SIZE)
			Dim result As E() = arrayFactory.apply(CInt(size))
			copyInto(result, 0)
			Return result
		End Function

		Public Overrides Sub clear()
			If spine IsNot Nothing Then
				curChunk = spine(0)
				For i As Integer = 0 To curChunk.Length - 1
					curChunk(i) = Nothing
				Next i
				spine = Nothing
				priorElementCount = Nothing
			Else
				Dim i As Integer=0
				Do While i<elementIndex
					curChunk(i) = Nothing
					i += 1
				Loop
			End If
			elementIndex = 0
			spineIndex = 0
		End Sub

		Public Overrides Function [iterator]() As IEnumerator(Of E) Implements Iterable(Of E).iterator
			Return java.util.Spliterators.iterator(spliterator())
		End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overrides Sub forEach(Of T1)(  consumer As java.util.function.Consumer(Of T1)) Implements Iterable(Of E).forEach
			' completed chunks, if any
			For j As Integer = 0 To spineIndex - 1
				For Each t As E In spine(j)
					consumer.accept(t)
				Next t
			Next j

			' current chunk
			Dim i As Integer=0
			Do While i<elementIndex
				consumer.accept(curChunk(i))
				i += 1
			Loop
		End Sub

		Public Overrides Sub accept(  e As E)
			If elementIndex = curChunk.Length Then
				inflateSpine()
				If spineIndex+1 >= spine.Length OrElse spine(spineIndex+1) Is Nothing Then increaseCapacity()
				elementIndex = 0
				spineIndex += 1
				curChunk = spine(spineIndex)
			End If
			curChunk(elementIndex) = e
			elementIndex += 1
		End Sub

		Public Overrides Function ToString() As String
			Dim list As IList(Of E) = New List(Of E)
			forEach(list::add)
			Return "SpinedBuffer:" & list.ToString()
		End Function

		Private Shared ReadOnly SPLITERATOR_CHARACTERISTICS As Integer = java.util.Spliterator.SIZED Or java.util.Spliterator.ORDERED Or java.util.Spliterator.SUBSIZED

		''' <summary>
		''' Return a <seealso cref="Spliterator"/> describing the contents of the buffer.
		''' </summary>
		Public Overridable Function spliterator() As java.util.Spliterator(Of E) Implements Iterable(Of E).spliterator
'JAVA TO VB CONVERTER TODO TASK: Local classes are not converted by Java to VB Converter:
'			class Splitr implements java.util.Spliterator(Of E)
	'		{
	'			' The current spine index
	'			int splSpineIndex;
	'
	'			' Last spine index
	'			final int lastSpineIndex;
	'
	'			' The current element index into the current spine
	'			int splElementIndex;
	'
	'			' Last spine's last element index + 1
	'			final int lastSpineElementFence;
	'
	'			' When splSpineIndex >= lastSpineIndex and
	'			' splElementIndex >= lastSpineElementFence then
	'			' this spliterator is fully traversed
	'			' tryAdvance can set splSpineIndex > spineIndex if the last spine is full
	'
	'			' The current spine array
	'			E[] splChunk;
	'
	'			Splitr(int firstSpineIndex, int lastSpineIndex, int firstSpineElementIndex, int lastSpineElementFence)
	'			{
	'				Me.splSpineIndex = firstSpineIndex;
	'				Me.lastSpineIndex = lastSpineIndex;
	'				Me.splElementIndex = firstSpineElementIndex;
	'				Me.lastSpineElementFence = lastSpineElementFence;
	'				assert spine != Nothing || firstSpineIndex == 0 && lastSpineIndex == 0;
	'				splChunk = (spine == Nothing) ? curChunk : spine[firstSpineIndex];
	'			}
	'
	'			@Override public long estimateSize()
	'			{
	'				Return (splSpineIndex == lastSpineIndex) ? (long) lastSpineElementFence - splElementIndex : priorElementCount[lastSpineIndex] + lastSpineElementFence - priorElementCount[splSpineIndex] - splElementIndex; ' # of elements prior to end -
	'					   ' # of elements prior to current
	'			}
	'
	'			@Override public int characteristics()
	'			{
	'				Return SPLITERATOR_CHARACTERISTICS;
	'			}
	'
	'			@Override public boolean tryAdvance(Consumer<? MyBase E> consumer)
	'			{
	'				Objects.requireNonNull(consumer);
	'
	'				if (splSpineIndex < lastSpineIndex || (splSpineIndex == lastSpineIndex && splElementIndex < lastSpineElementFence))
	'				{
	'					consumer.accept(splChunk[splElementIndex]);
	'					splElementIndex += 1;
	'
	'					if (splElementIndex == splChunk.length)
	'					{
	'						splElementIndex = 0;
	'						splSpineIndex += 1;
	'						if (spine != Nothing && splSpineIndex <= lastSpineIndex)
	'							splChunk = spine[splSpineIndex];
	'					}
	'					Return True;
	'				}
	'				Return False;
	'			}
	'
	'			@Override public  Sub  forEachRemaining(Consumer<? MyBase E> consumer)
	'			{
	'				Objects.requireNonNull(consumer);
	'
	'				if (splSpineIndex < lastSpineIndex || (splSpineIndex == lastSpineIndex && splElementIndex < lastSpineElementFence))
	'				{
	'					int i = splElementIndex;
	'					' completed chunks, if any
	'					for (int sp = splSpineIndex; sp < lastSpineIndex; sp += 1)
	'					{
	'						E[] chunk = spine[sp];
	'						for (; i < chunk.length; i += 1)
	'						{
	'							consumer.accept(chunk[i]);
	'						}
	'						i = 0;
	'					}
	'					' last (or current uncompleted) chunk
	'					E[] chunk = (splSpineIndex == lastSpineIndex) ? splChunk : spine[lastSpineIndex];
	'					int hElementIndex = lastSpineElementFence;
	'					for (; i < hElementIndex; i += 1)
	'					{
	'						consumer.accept(chunk[i]);
	'					}
	'					' mark consumed
	'					splSpineIndex = lastSpineIndex;
	'					splElementIndex = lastSpineElementFence;
	'				}
	'			}
	'
	'			@Override public Spliterator<E> trySplit()
	'			{
	'				if (splSpineIndex < lastSpineIndex)
	'				{
	'					' split just before last chunk (if it is full this means 50:50 split)
	'					Spliterator<E> ret = New Splitr(splSpineIndex, lastSpineIndex - 1, splElementIndex, spine[lastSpineIndex-1].length);
	'					' position to start of last chunk
	'					splSpineIndex = lastSpineIndex;
	'					splElementIndex = 0;
	'					splChunk = spine[splSpineIndex];
	'					Return ret;
	'				}
	'				else if (splSpineIndex == lastSpineIndex)
	'				{
	'					int t = (lastSpineElementFence - splElementIndex) / 2;
	'					if (t == 0)
	'						Return Nothing;
	'					else
	'					{
	'						Spliterator<E> ret = Arrays.spliterator(splChunk, splElementIndex, splElementIndex + t);
	'						splElementIndex += t;
	'						Return ret;
	'					}
	'				}
	'				else
	'				{
	'					Return Nothing;
	'				}
	'			}
	'		}
			Return New Splitr(0, spineIndex, 0, elementIndex)
		End Function

		''' <summary>
		''' An ordered collection of primitive values.  Elements can be added, but
		''' not removed. Goes through a building phase, during which elements can be
		''' added, and a traversal phase, during which elements can be traversed in
		''' order but no further modifications are possible.
		''' 
		''' <p> One or more arrays are used to store elements. The use of a multiple
		''' arrays has better performance characteristics than a single array used by
		''' <seealso cref="ArrayList"/>, as when the capacity of the list needs to be increased
		''' no copying of elements is required.  This is usually beneficial in the case
		''' where the results will be traversed a small number of times.
		''' </summary>
		''' @param <E> the wrapper type for this primitive type </param>
		''' @param <T_ARR> the array type for this primitive type </param>
		''' @param <T_CONS> the Consumer type for this primitive type </param>
		Friend MustInherit Class OfPrimitive(Of E, T_ARR, T_CONS)
			Inherits AbstractSpinedBuffer
			Implements Iterable(Of E)

	'        
	'         * We optimistically hope that all the data will fit into the first chunk,
	'         * so we try to avoid inflating the spine[] and priorElementCount[] arrays
	'         * prematurely.  So methods must be prepared to deal with these arrays being
	'         * null.  If spine is non-null, then spineIndex points to the current chunk
	'         * within the spine, otherwise it is zero.  The spine and priorElementCount
	'         * arrays are always the same size, and for any i <= spineIndex,
	'         * priorElementCount[i] is the sum of the sizes of all the prior chunks.
	'         *
	'         * The curChunk pointer is always valid.  The elementIndex is the index of
	'         * the next element to be written in curChunk; this may be past the end of
	'         * curChunk so we have to check before writing. When we inflate the spine
	'         * array, curChunk becomes the first element in it.  When we clear the
	'         * buffer, we discard all chunks except the first one, which we clear,
	'         * restoring it to the initial single-chunk state.
	'         

			' The chunk we're currently writing into
			Friend curChunk As T_ARR

			' All chunks, or null if there is only one chunk
			Friend spine As T_ARR()

			''' <summary>
			''' Constructs an empty list with the specified initial capacity.
			''' </summary>
			''' <param name="initialCapacity">  the initial capacity of the list </param>
			''' <exception cref="IllegalArgumentException"> if the specified initial capacity
			'''         is negative </exception>
			Friend Sub New(  initialCapacity As Integer)
				MyBase.New(initialCapacity)
				curChunk = newArray(1 << initialChunkPower)
			End Sub

			''' <summary>
			''' Constructs an empty list with an initial capacity of sixteen.
			''' </summary>
			Friend Sub New()
				MyBase.New()
				curChunk = newArray(1 << initialChunkPower)
			End Sub

			Public MustOverride Overrides Function [iterator]() As IEnumerator(Of E) Implements Iterable(Of E).iterator

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public MustOverride Overrides Sub forEach(Of T1)(  consumer As java.util.function.Consumer(Of T1)) Implements Iterable(Of E).forEach

			''' <summary>
			''' Create a new array-of-array of the proper type and size </summary>
			Protected Friend MustOverride Function newArrayArray(  size As Integer) As T_ARR()

			''' <summary>
			''' Create a new array of the proper type and size </summary>
			Public MustOverride Function newArray(  size As Integer) As T_ARR

			''' <summary>
			''' Get the length of an array </summary>
			Protected Friend MustOverride Function arrayLength(  array As T_ARR) As Integer

			''' <summary>
			''' Iterate an array with the provided consumer </summary>
			Protected Friend MustOverride Sub arrayForEach(  array As T_ARR,   [from] As Integer,   [to] As Integer,   consumer As T_CONS)

			Protected Friend Overridable Function capacity() As Long
				Return If(spineIndex = 0, arrayLength(curChunk), priorElementCount(spineIndex) + arrayLength(spine(spineIndex)))
			End Function

			Private Sub inflateSpine()
				If spine Is Nothing Then
					spine = newArrayArray(MIN_SPINE_SIZE)
					priorElementCount = New Long(MIN_SPINE_SIZE - 1){}
					spine(0) = curChunk
				End If
			End Sub

			Protected Friend Sub ensureCapacity(  targetSize As Long)
				Dim capacity As Long = capacity()
				If targetSize > capacity Then
					inflateSpine()
					Dim i As Integer=spineIndex+1
					Do While targetSize > capacity
						If i >= spine.Length Then
							Dim newSpineSize As Integer = spine.Length * 2
							spine = java.util.Arrays.copyOf(spine, newSpineSize)
							priorElementCount = java.util.Arrays.copyOf(priorElementCount, newSpineSize)
						End If
						Dim nextChunkSize As Integer = chunkSize(i)
						spine(i) = newArray(nextChunkSize)
						priorElementCount(i) = priorElementCount(i-1) + arrayLength(spine(i - 1))
						capacity += nextChunkSize
						i += 1
					Loop
				End If
			End Sub

			Protected Friend Overridable Sub increaseCapacity()
				ensureCapacity(capacity() + 1)
			End Sub

			Protected Friend Overridable Function chunkFor(  index As Long) As Integer
				If spineIndex = 0 Then
					If index < elementIndex Then
						Return 0
					Else
						Throw New IndexOutOfBoundsException(Convert.ToString(index))
					End If
				End If

				If index >= count() Then Throw New IndexOutOfBoundsException(Convert.ToString(index))

				For j As Integer = 0 To spineIndex
					If index < priorElementCount(j) + arrayLength(spine(j)) Then Return j
				Next j

				Throw New IndexOutOfBoundsException(Convert.ToString(index))
			End Function

			Public Overridable Sub copyInto(  array As T_ARR,   offset As Integer)
				Dim finalOffset As Long = offset + count()
				If finalOffset > arrayLength(array) OrElse finalOffset < offset Then Throw New IndexOutOfBoundsException("does not fit")

				If spineIndex = 0 Then
					Array.Copy(curChunk, 0, array, offset, elementIndex)
				Else
					' full chunks
					For i As Integer = 0 To spineIndex - 1
						Array.Copy(spine(i), 0, array, offset, arrayLength(spine(i)))
						offset += arrayLength(spine(i))
					Next i
					If elementIndex > 0 Then Array.Copy(curChunk, 0, array, offset, elementIndex)
				End If
			End Sub

			Public Overridable Function asPrimitiveArray() As T_ARR
				Dim size As Long = count()
				If size >= Nodes.MAX_ARRAY_SIZE Then Throw New IllegalArgumentException(Nodes.BAD_SIZE)
				Dim result As T_ARR = newArray(CInt(size))
				copyInto(result, 0)
				Return result
			End Function

			Protected Friend Overridable Sub preAccept()
				If elementIndex = arrayLength(curChunk) Then
					inflateSpine()
					If spineIndex+1 >= spine.Length OrElse spine(spineIndex+1) Is Nothing Then increaseCapacity()
					elementIndex = 0
					spineIndex += 1
					curChunk = spine(spineIndex)
				End If
			End Sub

			Public Overrides Sub clear()
				If spine IsNot Nothing Then
					curChunk = spine(0)
					spine = Nothing
					priorElementCount = Nothing
				End If
				elementIndex = 0
				spineIndex = 0
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Overridable Sub forEach(  consumer As T_CONS)
				' completed chunks, if any
				For j As Integer = 0 To spineIndex - 1
					arrayForEach(spine(j), 0, arrayLength(spine(j)), consumer)
				Next j

				' current chunk
				arrayForEach(curChunk, 0, elementIndex, consumer)
			End Sub

			Friend MustInherit Class BaseSpliterator(Of T_SPLITR As java.util.Spliterator.OfPrimitive(Of E, T_CONS, T_SPLITR))
				Implements java.util.Spliterator.OfPrimitive(Of E, T_CONS, T_SPLITR)

				Private ReadOnly outerInstance As SpinedBuffer.OfPrimitive

				' The current spine index
				Friend splSpineIndex As Integer

				' Last spine index
				Friend ReadOnly lastSpineIndex As Integer

				' The current element index into the current spine
				Friend splElementIndex As Integer

				' Last spine's last element index + 1
				Friend ReadOnly lastSpineElementFence As Integer

				' When splSpineIndex >= lastSpineIndex and
				' splElementIndex >= lastSpineElementFence then
				' this spliterator is fully traversed
				' tryAdvance can set splSpineIndex > spineIndex if the last spine is full

				' The current spine array
				Friend splChunk As T_ARR

				Friend Sub New(  outerInstance As SpinedBuffer.OfPrimitive,   firstSpineIndex As Integer,   lastSpineIndex As Integer,   firstSpineElementIndex As Integer,   lastSpineElementFence As Integer)
						Me.outerInstance = outerInstance
					Me.splSpineIndex = firstSpineIndex
					Me.lastSpineIndex = lastSpineIndex
					Me.splElementIndex = firstSpineElementIndex
					Me.lastSpineElementFence = lastSpineElementFence
					Debug.Assert(outerInstance.spine IsNot Nothing OrElse firstSpineIndex = 0 AndAlso lastSpineIndex = 0)
					splChunk = If(outerInstance.spine Is Nothing, outerInstance.curChunk, outerInstance.spine(firstSpineIndex))
				End Sub

				Friend MustOverride Function newSpliterator(  firstSpineIndex As Integer,   lastSpineIndex As Integer,   firstSpineElementIndex As Integer,   lastSpineElementFence As Integer) As T_SPLITR

				Friend MustOverride Sub arrayForOne(  array As T_ARR,   index As Integer,   consumer As T_CONS)

				Friend MustOverride Function arraySpliterator(  array As T_ARR,   offset As Integer,   len As Integer) As T_SPLITR

				Public Overrides Function estimateSize() As Long
					Return If(splSpineIndex = lastSpineIndex, CLng(lastSpineElementFence) - splElementIndex, outerInstance.priorElementCount(lastSpineIndex) + lastSpineElementFence - outerInstance.priorElementCount(splSpineIndex) - splElementIndex) ' # of elements prior to end -
						   ' # of elements prior to current
				End Function

				Public Overrides Function characteristics() As Integer
					Return SPLITERATOR_CHARACTERISTICS
				End Function

				Public Overrides Function tryAdvance(  consumer As T_CONS) As Boolean
					java.util.Objects.requireNonNull(consumer)

					If splSpineIndex < lastSpineIndex OrElse (splSpineIndex = lastSpineIndex AndAlso splElementIndex < lastSpineElementFence) Then
						arrayForOne(splChunk, splElementIndex, consumer)
						splElementIndex += 1

						If splElementIndex = outerInstance.arrayLength(splChunk) Then
							splElementIndex = 0
							splSpineIndex += 1
							If outerInstance.spine IsNot Nothing AndAlso splSpineIndex <= lastSpineIndex Then splChunk = outerInstance.spine(splSpineIndex)
						End If
						Return True
					End If
					Return False
				End Function

				Public Overrides Sub forEachRemaining(  consumer As T_CONS)
					java.util.Objects.requireNonNull(consumer)

					If splSpineIndex < lastSpineIndex OrElse (splSpineIndex = lastSpineIndex AndAlso splElementIndex < lastSpineElementFence) Then
						Dim i As Integer = splElementIndex
						' completed chunks, if any
						For sp As Integer = splSpineIndex To lastSpineIndex - 1
							Dim chunk As T_ARR = outerInstance.spine(sp)
							outerInstance.arrayForEach(chunk, i, outerInstance.arrayLength(chunk), consumer)
							i = 0
						Next sp
						' last (or current uncompleted) chunk
						Dim chunk As T_ARR = If(splSpineIndex = lastSpineIndex, splChunk, outerInstance.spine(lastSpineIndex))
						outerInstance.arrayForEach(chunk, i, lastSpineElementFence, consumer)
						' mark consumed
						splSpineIndex = lastSpineIndex
						splElementIndex = lastSpineElementFence
					End If
				End Sub

				Public Overrides Function trySplit() As T_SPLITR
					If splSpineIndex < lastSpineIndex Then
						' split just before last chunk (if it is full this means 50:50 split)
						Dim ret As T_SPLITR = newSpliterator(splSpineIndex, lastSpineIndex - 1, splElementIndex, outerInstance.arrayLength(outerInstance.spine(lastSpineIndex - 1)))
						' position us to start of last chunk
						splSpineIndex = lastSpineIndex
						splElementIndex = 0
						splChunk = outerInstance.spine(splSpineIndex)
						Return ret
					ElseIf splSpineIndex = lastSpineIndex Then
						Dim t As Integer = (lastSpineElementFence - splElementIndex) \ 2
						If t = 0 Then
							Return Nothing
						Else
							Dim ret As T_SPLITR = arraySpliterator(splChunk, splElementIndex, t)
							splElementIndex += t
							Return ret
						End If
					Else
						Return Nothing
					End If
				End Function
			End Class
		End Class

		''' <summary>
		''' An ordered collection of {@code int} values.
		''' </summary>
		Friend Class OfInt
			Inherits SpinedBuffer.OfPrimitive(Of Integer?, int(), java.util.function.IntConsumer)
			Implements java.util.function.IntConsumer

			Friend Sub New()
			End Sub

			Friend Sub New(  initialCapacity As Integer)
				MyBase.New(initialCapacity)
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub forEach(Of T1)(  consumer As java.util.function.Consumer(Of T1))
				If TypeOf consumer Is java.util.function.IntConsumer Then
					forEach(CType(consumer, java.util.function.IntConsumer))
				Else
					If Tripwire.ENABLED Then Tripwire.trip(Me.GetType(), "{0} calling SpinedBuffer.OfInt.forEach(Consumer)")
					spliterator().forEachRemaining(consumer)
				End If
			End Sub

			Protected Friend Overrides Function newArrayArray(  size As Integer) As Integer()()
				Return New Integer(size - 1)(){}
			End Function

			Public Overrides Function newArray(  size As Integer) As Integer()
				Return New Integer(size - 1){}
			End Function

			Protected Friend Overrides Function arrayLength(  array As Integer()) As Integer
				Return array.Length
			End Function

			Protected Friend Overrides Sub arrayForEach(  array As Integer(),   [from] As Integer,   [to] As Integer,   consumer As java.util.function.IntConsumer)
				For i As Integer = from To [to] - 1
					consumer.accept(array(i))
				Next i
			End Sub

			Public Overrides Sub accept(  i As Integer)
				preAccept()
				curChunk(elementIndex) = i
				elementIndex += 1
			End Sub

			Public Overridable Function [get](  index As Long) As Integer
				' Casts to int are safe since the spine array index is the index minus
				' the prior element count from the current spine
				Dim ch As Integer = chunkFor(index)
				If spineIndex = 0 AndAlso ch = 0 Then
					Return curChunk(CInt(index))
				Else
					Return spine(ch)(CInt(index - priorElementCount(ch)))
				End If
			End Function

			Public Overrides Function [iterator]() As java.util.PrimitiveIterator.OfInt
				Return java.util.Spliterators.iterator(spliterator())
			End Function

			Public Overridable Function spliterator() As java.util.Spliterator.OfInt
'JAVA TO VB CONVERTER TODO TASK: Local classes are not converted by Java to VB Converter:
'				class Splitr extends BaseSpliterator(Of java.util.Spliterator.OfInt) implements java.util.Spliterator.OfInt
	'			{
	'				Splitr(int firstSpineIndex, int lastSpineIndex, int firstSpineElementIndex, int lastSpineElementFence)
	'				{
	'					MyBase(firstSpineIndex, lastSpineIndex, firstSpineElementIndex, lastSpineElementFence);
	'				}
	'
	'				@Override Splitr newSpliterator(int firstSpineIndex, int lastSpineIndex, int firstSpineElementIndex, int lastSpineElementFence)
	'				{
	'					Return New Splitr(firstSpineIndex, lastSpineIndex, firstSpineElementIndex, lastSpineElementFence);
	'				}
	'
	'				@Override  Sub  arrayForOne(int[] array, int index, IntConsumer consumer)
	'				{
	'					consumer.accept(array[index]);
	'				}
	'
	'				@Override Spliterator.OfInt arraySpliterator(int[] array, int offset, int len)
	'				{
	'					Return Arrays.spliterator(array, offset, offset+len);
	'				}
	'			}
				Return New Splitr(0, spineIndex, 0, elementIndex)
			End Function

			Public Overrides Function ToString() As String
				Dim array As Integer() = asPrimitiveArray()
				If array.Length < 200 Then
					Return String.Format("{0}[length={1:D}, chunks={2:D}]{3}", Me.GetType().simpleName, array.Length, spineIndex, java.util.Arrays.ToString(array))
				Else
					Dim array2 As Integer() = java.util.Arrays.copyOf(array, 200)
					Return String.Format("{0}[length={1:D}, chunks={2:D}]{3}...", Me.GetType().simpleName, array.Length, spineIndex, java.util.Arrays.ToString(array2))
				End If
			End Function
		End Class

		''' <summary>
		''' An ordered collection of {@code long} values.
		''' </summary>
		Friend Class OfLong
			Inherits SpinedBuffer.OfPrimitive(Of Long?, long(), java.util.function.LongConsumer)
			Implements java.util.function.LongConsumer

			Friend Sub New()
			End Sub

			Friend Sub New(  initialCapacity As Integer)
				MyBase.New(initialCapacity)
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub forEach(Of T1)(  consumer As java.util.function.Consumer(Of T1))
				If TypeOf consumer Is java.util.function.LongConsumer Then
					forEach(CType(consumer, java.util.function.LongConsumer))
				Else
					If Tripwire.ENABLED Then Tripwire.trip(Me.GetType(), "{0} calling SpinedBuffer.OfLong.forEach(Consumer)")
					spliterator().forEachRemaining(consumer)
				End If
			End Sub

			Protected Friend Overrides Function newArrayArray(  size As Integer) As Long()()
				Return New Long(size - 1)(){}
			End Function

			Public Overrides Function newArray(  size As Integer) As Long()
				Return New Long(size - 1){}
			End Function

			Protected Friend Overrides Function arrayLength(  array As Long()) As Integer
				Return array.Length
			End Function

			Protected Friend Overrides Sub arrayForEach(  array As Long(),   [from] As Integer,   [to] As Integer,   consumer As java.util.function.LongConsumer)
				For i As Integer = from To [to] - 1
					consumer.accept(array(i))
				Next i
			End Sub

			Public Overrides Sub accept(  i As Long)
				preAccept()
				curChunk(elementIndex) = i
				elementIndex += 1
			End Sub

			Public Overridable Function [get](  index As Long) As Long
				' Casts to int are safe since the spine array index is the index minus
				' the prior element count from the current spine
				Dim ch As Integer = chunkFor(index)
				If spineIndex = 0 AndAlso ch = 0 Then
					Return curChunk(CInt(index))
				Else
					Return spine(ch)(CInt(index - priorElementCount(ch)))
				End If
			End Function

			Public Overrides Function [iterator]() As java.util.PrimitiveIterator.OfLong
				Return java.util.Spliterators.iterator(spliterator())
			End Function


			Public Overridable Function spliterator() As java.util.Spliterator.OfLong
'JAVA TO VB CONVERTER TODO TASK: Local classes are not converted by Java to VB Converter:
'				class Splitr extends BaseSpliterator(Of java.util.Spliterator.OfLong) implements java.util.Spliterator.OfLong
	'			{
	'				Splitr(int firstSpineIndex, int lastSpineIndex, int firstSpineElementIndex, int lastSpineElementFence)
	'				{
	'					MyBase(firstSpineIndex, lastSpineIndex, firstSpineElementIndex, lastSpineElementFence);
	'				}
	'
	'				@Override Splitr newSpliterator(int firstSpineIndex, int lastSpineIndex, int firstSpineElementIndex, int lastSpineElementFence)
	'				{
	'					Return New Splitr(firstSpineIndex, lastSpineIndex, firstSpineElementIndex, lastSpineElementFence);
	'				}
	'
	'				@Override  Sub  arrayForOne(long[] array, int index, LongConsumer consumer)
	'				{
	'					consumer.accept(array[index]);
	'				}
	'
	'				@Override Spliterator.OfLong arraySpliterator(long[] array, int offset, int len)
	'				{
	'					Return Arrays.spliterator(array, offset, offset+len);
	'				}
	'			}
				Return New Splitr(0, spineIndex, 0, elementIndex)
			End Function

			Public Overrides Function ToString() As String
				Dim array As Long() = asPrimitiveArray()
				If array.Length < 200 Then
					Return String.Format("{0}[length={1:D}, chunks={2:D}]{3}", Me.GetType().simpleName, array.Length, spineIndex, java.util.Arrays.ToString(array))
				Else
					Dim array2 As Long() = java.util.Arrays.copyOf(array, 200)
					Return String.Format("{0}[length={1:D}, chunks={2:D}]{3}...", Me.GetType().simpleName, array.Length, spineIndex, java.util.Arrays.ToString(array2))
				End If
			End Function
		End Class

		''' <summary>
		''' An ordered collection of {@code double} values.
		''' </summary>
		Friend Class OfDouble
			Inherits SpinedBuffer.OfPrimitive(Of Double?, double(), java.util.function.DoubleConsumer)
			Implements java.util.function.DoubleConsumer

			Friend Sub New()
			End Sub

			Friend Sub New(  initialCapacity As Integer)
				MyBase.New(initialCapacity)
			End Sub

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub forEach(Of T1)(  consumer As java.util.function.Consumer(Of T1))
				If TypeOf consumer Is java.util.function.DoubleConsumer Then
					forEach(CType(consumer, java.util.function.DoubleConsumer))
				Else
					If Tripwire.ENABLED Then Tripwire.trip(Me.GetType(), "{0} calling SpinedBuffer.OfDouble.forEach(Consumer)")
					spliterator().forEachRemaining(consumer)
				End If
			End Sub

			Protected Friend Overrides Function newArrayArray(  size As Integer) As Double()()
				Return New Double(size - 1)(){}
			End Function

			Public Overrides Function newArray(  size As Integer) As Double()
				Return New Double(size - 1){}
			End Function

			Protected Friend Overrides Function arrayLength(  array As Double()) As Integer
				Return array.Length
			End Function

			Protected Friend Overrides Sub arrayForEach(  array As Double(),   [from] As Integer,   [to] As Integer,   consumer As java.util.function.DoubleConsumer)
				For i As Integer = from To [to] - 1
					consumer.accept(array(i))
				Next i
			End Sub

			Public Overrides Sub accept(  i As Double)
				preAccept()
				curChunk(elementIndex) = i
				elementIndex += 1
			End Sub

			Public Overridable Function [get](  index As Long) As Double
				' Casts to int are safe since the spine array index is the index minus
				' the prior element count from the current spine
				Dim ch As Integer = chunkFor(index)
				If spineIndex = 0 AndAlso ch = 0 Then
					Return curChunk(CInt(index))
				Else
					Return spine(ch)(CInt(index - priorElementCount(ch)))
				End If
			End Function

			Public Overrides Function [iterator]() As java.util.PrimitiveIterator.OfDouble
				Return java.util.Spliterators.iterator(spliterator())
			End Function

			Public Overridable Function spliterator() As java.util.Spliterator.OfDouble
'JAVA TO VB CONVERTER TODO TASK: Local classes are not converted by Java to VB Converter:
'				class Splitr extends BaseSpliterator(Of java.util.Spliterator.OfDouble) implements java.util.Spliterator.OfDouble
	'			{
	'				Splitr(int firstSpineIndex, int lastSpineIndex, int firstSpineElementIndex, int lastSpineElementFence)
	'				{
	'					MyBase(firstSpineIndex, lastSpineIndex, firstSpineElementIndex, lastSpineElementFence);
	'				}
	'
	'				@Override Splitr newSpliterator(int firstSpineIndex, int lastSpineIndex, int firstSpineElementIndex, int lastSpineElementFence)
	'				{
	'					Return New Splitr(firstSpineIndex, lastSpineIndex, firstSpineElementIndex, lastSpineElementFence);
	'				}
	'
	'				@Override  Sub  arrayForOne(double[] array, int index, DoubleConsumer consumer)
	'				{
	'					consumer.accept(array[index]);
	'				}
	'
	'				@Override Spliterator.OfDouble arraySpliterator(double[] array, int offset, int len)
	'				{
	'					Return Arrays.spliterator(array, offset, offset+len);
	'				}
	'			}
				Return New Splitr(0, spineIndex, 0, elementIndex)
			End Function

			Public Overrides Function ToString() As String
				Dim array As Double() = asPrimitiveArray()
				If array.Length < 200 Then
					Return String.Format("{0}[length={1:D}, chunks={2:D}]{3}", Me.GetType().simpleName, array.Length, spineIndex, java.util.Arrays.ToString(array))
				Else
					Dim array2 As Double() = java.util.Arrays.copyOf(array, 200)
					Return String.Format("{0}[length={1:D}, chunks={2:D}]{3}...", Me.GetType().simpleName, array.Length, spineIndex, java.util.Arrays.ToString(array2))
				End If
			End Function
		End Class
	End Class


End Namespace