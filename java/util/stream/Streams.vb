Imports System.Diagnostics

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
	''' Utility methods for operating on and creating streams.
	''' 
	''' <p>Unless otherwise stated, streams are created as sequential streams.  A
	''' sequential stream can be transformed into a parallel stream by calling the
	''' {@code parallel()} method on the created stream.
	''' 
	''' @since 1.8
	''' </summary>
	Friend NotInheritable Class Streams

		Private Sub New()
			Throw New [Error]("no instances")
		End Sub

		Private Class IteratorAnonymousInnerClassHelper(Of E)
			Implements Iterator(Of E)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Friend t As T = CType(Streams.NONE, T)

			Public Overrides Function hasNext() As Boolean Implements Iterator(Of E).hasNext
				Return True
			End Function

			Public Overrides Function [next]() As T
					t = If(t Is Streams.NONE, seed, f.apply(t))
					Return t
			End Function
		End Class

		''' <summary>
		''' An object instance representing no value, that cannot be an actual
		''' data element of a stream.  Used when processing streams that can contain
		''' {@code null} elements to distinguish between a {@code null} value and no
		''' value.
		''' </summary>
		Friend Shared ReadOnly NONE As New Object

		''' <summary>
		''' An {@code int} range spliterator.
		''' </summary>
		Friend NotInheritable Class RangeIntSpliterator
			Implements java.util.Spliterator.OfInt

			' Can never be greater that upTo, this avoids overflow if upper bound
			' is  java.lang.[Integer].MAX_VALUE
			' All elements are traversed if from == upTo & last == 0
			Private [from] As Integer
			Private ReadOnly upTo As Integer
			' 1 if the range is closed and the last element has not been traversed
			' Otherwise, 0 if the range is open, or is a closed range and all
			' elements have been traversed
			Private last As Integer

			Friend Sub New(  [from] As Integer,   upTo As Integer,   closed As Boolean)
				Me.New([from], upTo,If(closed, 1, 0))
			End Sub

			Private Sub New(  [from] As Integer,   upTo As Integer,   last As Integer)
				Me.from = [from]
				Me.upTo = upTo
				Me.last = last
			End Sub

			Public Overrides Function tryAdvance(  consumer As java.util.function.IntConsumer) As Boolean
				java.util.Objects.requireNonNull(consumer)

				Dim i As Integer = [from]
				If i < upTo Then
					[from] += 1
					consumer.accept(i)
					Return True
				ElseIf last > 0 Then
					last = 0
					consumer.accept(i)
					Return True
				End If
				Return False
			End Function

			Public Overrides Sub forEachRemaining(  consumer As java.util.function.IntConsumer)
				java.util.Objects.requireNonNull(consumer)

				Dim i As Integer = [from]
				Dim hUpTo As Integer = upTo
				Dim hLast As Integer = last
				[from] = upTo
				last = 0
				Do While i < hUpTo
					consumer.accept(i)
					i += 1
				Loop
				If hLast > 0 Then consumer.accept(i)
			End Sub

			Public Overrides Function estimateSize() As Long
				' Ensure ranges of size >  java.lang.[Integer].MAX_VALUE report the correct size
				Return (CLng(upTo)) - [from] + last
			End Function

			Public Overrides Function characteristics() As Integer
				Return java.util.Spliterator.ORDERED Or java.util.Spliterator.SIZED Or java.util.Spliterator.SUBSIZED Or java.util.Spliterator.IMMUTABLE Or java.util.Spliterator.NONNULL Or java.util.Spliterator.DISTINCT Or java.util.Spliterator.SORTED
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public  Overrides ReadOnly Property  comparator As IComparer(Of ?)
				Get
					Return Nothing
				End Get
			End Property

			Public Overrides Function trySplit() As java.util.Spliterator.OfInt
				Dim size As Long = estimateSize()
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return If(size <= 1, Nothing, New RangeIntSpliterator([from], [from] = [from] + splitPoint(size), 0))
					   ' Left split always has a half-open range
			End Function

			''' <summary>
			''' The spliterator size below which the spliterator will be split
			''' at the mid-point to produce balanced splits. Above this size the
			''' spliterator will be split at a ratio of
			''' 1:(RIGHT_BALANCED_SPLIT_RATIO - 1)
			''' to produce right-balanced splits.
			''' 
			''' <p>Such splitting ensures that for very large ranges that the left
			''' side of the range will more likely be processed at a lower-depth
			''' than a balanced tree at the expense of a higher-depth for the right
			''' side of the range.
			''' 
			''' <p>This is optimized for cases such as IntStream.ints() that is
			''' implemented as range of 0 to  java.lang.[Integer].MAX_VALUE but is likely to be
			''' augmented with a limit operation that limits the number of elements
			''' to a count lower than this threshold.
			''' </summary>
			Private Shared ReadOnly BALANCED_SPLIT_THRESHOLD As Integer = 1 << 24

			''' <summary>
			''' The split ratio of the left and right split when the spliterator
			''' size is above BALANCED_SPLIT_THRESHOLD.
			''' </summary>
			Private Shared ReadOnly RIGHT_BALANCED_SPLIT_RATIO As Integer = 1 << 3

			Private Function splitPoint(  size As Long) As Integer
				Dim d As Integer = If(size < BALANCED_SPLIT_THRESHOLD, 2, RIGHT_BALANCED_SPLIT_RATIO)
				' Cast to int is safe since:
				'   2 <= size < 2^32
				'   2 <= d <= 8
				Return CInt(size \ d)
			End Function
		End Class

		''' <summary>
		''' A {@code long} range spliterator.
		''' 
		''' This implementation cannot be used for ranges whose size is greater
		''' than java.lang.[Long].MAX_VALUE
		''' </summary>
		Friend NotInheritable Class RangeLongSpliterator
			Implements java.util.Spliterator.OfLong

			' Can never be greater that upTo, this avoids overflow if upper bound
			' is java.lang.[Long].MAX_VALUE
			' All elements are traversed if from == upTo & last == 0
			Private [from] As Long
			Private ReadOnly upTo As Long
			' 1 if the range is closed and the last element has not been traversed
			' Otherwise, 0 if the range is open, or is a closed range and all
			' elements have been traversed
			Private last As Integer

			Friend Sub New(  [from] As Long,   upTo As Long,   closed As Boolean)
				Me.New([from], upTo,If(closed, 1, 0))
			End Sub

			Private Sub New(  [from] As Long,   upTo As Long,   last As Integer)
				Debug.Assert(upTo - [from] + last > 0)
				Me.from = [from]
				Me.upTo = upTo
				Me.last = last
			End Sub

			Public Overrides Function tryAdvance(  consumer As java.util.function.LongConsumer) As Boolean
				java.util.Objects.requireNonNull(consumer)

				Dim i As Long = [from]
				If i < upTo Then
					[from] += 1
					consumer.accept(i)
					Return True
				ElseIf last > 0 Then
					last = 0
					consumer.accept(i)
					Return True
				End If
				Return False
			End Function

			Public Overrides Sub forEachRemaining(  consumer As java.util.function.LongConsumer)
				java.util.Objects.requireNonNull(consumer)

				Dim i As Long = [from]
				Dim hUpTo As Long = upTo
				Dim hLast As Integer = last
				[from] = upTo
				last = 0
				Do While i < hUpTo
					consumer.accept(i)
					i += 1
				Loop
				If hLast > 0 Then consumer.accept(i)
			End Sub

			Public Overrides Function estimateSize() As Long
				Return upTo - [from] + last
			End Function

			Public Overrides Function characteristics() As Integer
				Return java.util.Spliterator.ORDERED Or java.util.Spliterator.SIZED Or java.util.Spliterator.SUBSIZED Or java.util.Spliterator.IMMUTABLE Or java.util.Spliterator.NONNULL Or java.util.Spliterator.DISTINCT Or java.util.Spliterator.SORTED
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public  Overrides ReadOnly Property  comparator As IComparer(Of ?)
				Get
					Return Nothing
				End Get
			End Property

			Public Overrides Function trySplit() As java.util.Spliterator.OfLong
				Dim size As Long = estimateSize()
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Return If(size <= 1, Nothing, New RangeLongSpliterator([from], [from] = [from] + splitPoint(size), 0))
					   ' Left split always has a half-open range
			End Function

			''' <summary>
			''' The spliterator size below which the spliterator will be split
			''' at the mid-point to produce balanced splits. Above this size the
			''' spliterator will be split at a ratio of
			''' 1:(RIGHT_BALANCED_SPLIT_RATIO - 1)
			''' to produce right-balanced splits.
			''' 
			''' <p>Such splitting ensures that for very large ranges that the left
			''' side of the range will more likely be processed at a lower-depth
			''' than a balanced tree at the expense of a higher-depth for the right
			''' side of the range.
			''' 
			''' <p>This is optimized for cases such as LongStream.longs() that is
			''' implemented as range of 0 to java.lang.[Long].MAX_VALUE but is likely to be
			''' augmented with a limit operation that limits the number of elements
			''' to a count lower than this threshold.
			''' </summary>
			Private Shared ReadOnly BALANCED_SPLIT_THRESHOLD As Long = 1 << 24

			''' <summary>
			''' The split ratio of the left and right split when the spliterator
			''' size is above BALANCED_SPLIT_THRESHOLD.
			''' </summary>
			Private Shared ReadOnly RIGHT_BALANCED_SPLIT_RATIO As Long = 1 << 3

			Private Function splitPoint(  size As Long) As Long
				Dim d As Long = If(size < BALANCED_SPLIT_THRESHOLD, 2, RIGHT_BALANCED_SPLIT_RATIO)
				' 2 <= size <= java.lang.[Long].MAX_VALUE
				Return size \ d
			End Function
		End Class

		Private MustInherit Class AbstractStreamBuilderImpl(Of T, S As java.util.Spliterator(Of T))
			Implements java.util.Spliterator(Of T)

			' >= 0 when building, < 0 when built
			' -1 == no elements
			' -2 == one element, held by first
			' -3 == two or more elements, held by buffer
			Friend count As Integer

			' Spliterator implementation for 0 or 1 element
			' count == -1 for no elements
			' count == -2 for one element held by first

			Public Overrides Function trySplit() As S
				Return Nothing
			End Function

			Public Overrides Function estimateSize() As Long
				Return -count - 1
			End Function

			Public Overrides Function characteristics() As Integer
				Return java.util.Spliterator.SIZED Or java.util.Spliterator.SUBSIZED Or java.util.Spliterator.ORDERED Or java.util.Spliterator.IMMUTABLE
			End Function
		End Class

		Friend NotInheritable Class StreamBuilderImpl(Of T)
			Inherits AbstractStreamBuilderImpl(Of T, java.util.Spliterator(Of T))
			Implements Stream.Builder(Of T)

			' The first element in the stream
			' valid if count == 1
			Friend first As T

			' The first and subsequent elements in the stream
			' non-null if count == 2
			Friend buffer As SpinedBuffer(Of T)

			''' <summary>
			''' Constructor for building a stream of 0 or more elements.
			''' </summary>
			Friend Sub New()
			End Sub

			''' <summary>
			''' Constructor for a singleton stream.
			''' </summary>
			''' <param name="t"> the single element </param>
			Friend Sub New(  t As T)
				first = t
				count = -2
			End Sub

			' StreamBuilder implementation

			Public Overrides Sub accept(  t As T)
				If count = 0 Then
					first = t
					count += 1
				ElseIf count > 0 Then
					If buffer Is Nothing Then
						buffer = New SpinedBuffer(Of )
						buffer.accept(first)
						count += 1
					End If

					buffer.accept(t)
				Else
					Throw New IllegalStateException
				End If
			End Sub

			Public Function add(  t As T) As Stream.Builder(Of T)
				accept(t)
				Return Me
			End Function

			Public Overrides Function build() As Stream(Of T)
				Dim c As Integer = count
				If c >= 0 Then
					' Switch count to negative value signalling the builder is built
					count = -count - 1
					' Use this spliterator if 0 or 1 elements, otherwise use
					' the spliterator of the spined buffer
					Return If(c < 2, StreamSupport.stream(Me, False), StreamSupport.stream(buffer.spliterator(), False))
				End If

				Throw New IllegalStateException
			End Function

			' Spliterator implementation for 0 or 1 element
			' count == -1 for no elements
			' count == -2 for one element held by first

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Function tryAdvance(Of T1)(  action As java.util.function.Consumer(Of T1)) As Boolean
				java.util.Objects.requireNonNull(action)

				If count = -2 Then
					action.accept(first)
					count = -1
					Return True
				Else
					Return False
				End If
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub forEachRemaining(Of T1)(  action As java.util.function.Consumer(Of T1))
				java.util.Objects.requireNonNull(action)

				If count = -2 Then
					action.accept(first)
					count = -1
				End If
			End Sub
		End Class

		Friend NotInheritable Class IntStreamBuilderImpl
			Inherits AbstractStreamBuilderImpl(Of Integer?, java.util.Spliterator.OfInt)
			Implements IntStream.Builder, java.util.Spliterator.OfInt

			' The first element in the stream
			' valid if count == 1
			Friend first As Integer

			' The first and subsequent elements in the stream
			' non-null if count == 2
			Friend buffer As SpinedBuffer.OfInt

			''' <summary>
			''' Constructor for building a stream of 0 or more elements.
			''' </summary>
			Friend Sub New()
			End Sub

			''' <summary>
			''' Constructor for a singleton stream.
			''' </summary>
			''' <param name="t"> the single element </param>
			Friend Sub New(  t As Integer)
				first = t
				count = -2
			End Sub

			' StreamBuilder implementation

			Public Overrides Sub accept(  t As Integer)
				If count = 0 Then
					first = t
					count += 1
				ElseIf count > 0 Then
					If buffer Is Nothing Then
						buffer = New SpinedBuffer.OfInt
						buffer.accept(first)
						count += 1
					End If

					buffer.accept(t)
				Else
					Throw New IllegalStateException
				End If
			End Sub

			Public Overrides Function build() As IntStream
				Dim c As Integer = count
				If c >= 0 Then
					' Switch count to negative value signalling the builder is built
					count = -count - 1
					' Use this spliterator if 0 or 1 elements, otherwise use
					' the spliterator of the spined buffer
					Return If(c < 2, StreamSupport.intStream(Me, False), StreamSupport.intStream(buffer.spliterator(), False))
				End If

				Throw New IllegalStateException
			End Function

			' Spliterator implementation for 0 or 1 element
			' count == -1 for no elements
			' count == -2 for one element held by first

			Public Overrides Function tryAdvance(  action As java.util.function.IntConsumer) As Boolean
				java.util.Objects.requireNonNull(action)

				If count = -2 Then
					action.accept(first)
					count = -1
					Return True
				Else
					Return False
				End If
			End Function

			Public Overrides Sub forEachRemaining(  action As java.util.function.IntConsumer)
				java.util.Objects.requireNonNull(action)

				If count = -2 Then
					action.accept(first)
					count = -1
				End If
			End Sub
		End Class

		Friend NotInheritable Class LongStreamBuilderImpl
			Inherits AbstractStreamBuilderImpl(Of Long?, java.util.Spliterator.OfLong)
			Implements LongStream.Builder, java.util.Spliterator.OfLong

			' The first element in the stream
			' valid if count == 1
			Friend first As Long

			' The first and subsequent elements in the stream
			' non-null if count == 2
			Friend buffer As SpinedBuffer.OfLong

			''' <summary>
			''' Constructor for building a stream of 0 or more elements.
			''' </summary>
			Friend Sub New()
			End Sub

			''' <summary>
			''' Constructor for a singleton stream.
			''' </summary>
			''' <param name="t"> the single element </param>
			Friend Sub New(  t As Long)
				first = t
				count = -2
			End Sub

			' StreamBuilder implementation

			Public Overrides Sub accept(  t As Long)
				If count = 0 Then
					first = t
					count += 1
				ElseIf count > 0 Then
					If buffer Is Nothing Then
						buffer = New SpinedBuffer.OfLong
						buffer.accept(first)
						count += 1
					End If

					buffer.accept(t)
				Else
					Throw New IllegalStateException
				End If
			End Sub

			Public Overrides Function build() As LongStream
				Dim c As Integer = count
				If c >= 0 Then
					' Switch count to negative value signalling the builder is built
					count = -count - 1
					' Use this spliterator if 0 or 1 elements, otherwise use
					' the spliterator of the spined buffer
					Return If(c < 2, StreamSupport.longStream(Me, False), StreamSupport.longStream(buffer.spliterator(), False))
				End If

				Throw New IllegalStateException
			End Function

			' Spliterator implementation for 0 or 1 element
			' count == -1 for no elements
			' count == -2 for one element held by first

			Public Overrides Function tryAdvance(  action As java.util.function.LongConsumer) As Boolean
				java.util.Objects.requireNonNull(action)

				If count = -2 Then
					action.accept(first)
					count = -1
					Return True
				Else
					Return False
				End If
			End Function

			Public Overrides Sub forEachRemaining(  action As java.util.function.LongConsumer)
				java.util.Objects.requireNonNull(action)

				If count = -2 Then
					action.accept(first)
					count = -1
				End If
			End Sub
		End Class

		Friend NotInheritable Class DoubleStreamBuilderImpl
			Inherits AbstractStreamBuilderImpl(Of Double?, java.util.Spliterator.OfDouble)
			Implements DoubleStream.Builder, java.util.Spliterator.OfDouble

			' The first element in the stream
			' valid if count == 1
			Friend first As Double

			' The first and subsequent elements in the stream
			' non-null if count == 2
			Friend buffer As SpinedBuffer.OfDouble

			''' <summary>
			''' Constructor for building a stream of 0 or more elements.
			''' </summary>
			Friend Sub New()
			End Sub

			''' <summary>
			''' Constructor for a singleton stream.
			''' </summary>
			''' <param name="t"> the single element </param>
			Friend Sub New(  t As Double)
				first = t
				count = -2
			End Sub

			' StreamBuilder implementation

			Public Overrides Sub accept(  t As Double)
				If count = 0 Then
					first = t
					count += 1
				ElseIf count > 0 Then
					If buffer Is Nothing Then
						buffer = New SpinedBuffer.OfDouble
						buffer.accept(first)
						count += 1
					End If

					buffer.accept(t)
				Else
					Throw New IllegalStateException
				End If
			End Sub

			Public Overrides Function build() As DoubleStream
				Dim c As Integer = count
				If c >= 0 Then
					' Switch count to negative value signalling the builder is built
					count = -count - 1
					' Use this spliterator if 0 or 1 elements, otherwise use
					' the spliterator of the spined buffer
					Return If(c < 2, StreamSupport.doubleStream(Me, False), StreamSupport.doubleStream(buffer.spliterator(), False))
				End If

				Throw New IllegalStateException
			End Function

			' Spliterator implementation for 0 or 1 element
			' count == -1 for no elements
			' count == -2 for one element held by first

			Public Overrides Function tryAdvance(  action As java.util.function.DoubleConsumer) As Boolean
				java.util.Objects.requireNonNull(action)

				If count = -2 Then
					action.accept(first)
					count = -1
					Return True
				Else
					Return False
				End If
			End Function

			Public Overrides Sub forEachRemaining(  action As java.util.function.DoubleConsumer)
				java.util.Objects.requireNonNull(action)

				If count = -2 Then
					action.accept(first)
					count = -1
				End If
			End Sub
		End Class

		Friend MustInherit Class ConcatSpliterator(Of T, T_SPLITR As java.util.Spliterator(Of T))
			Implements java.util.Spliterator(Of T)

			Protected Friend ReadOnly aSpliterator As T_SPLITR
			Protected Friend ReadOnly bSpliterator As T_SPLITR
			' True when no split has occurred, otherwise false
			Friend beforeSplit As Boolean
			' Never read after splitting
			Friend ReadOnly unsized As Boolean

			Public Sub New(  aSpliterator As T_SPLITR,   bSpliterator As T_SPLITR)
				Me.aSpliterator = aSpliterator
				Me.bSpliterator = bSpliterator
				beforeSplit = True
				' The spliterator is known to be unsized before splitting if the
				' sum of the estimates overflows.
				unsized = aSpliterator.estimateSize() + bSpliterator.estimateSize() < 0
			End Sub

			Public Overrides Function trySplit() As T_SPLITR
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				Dim ret As T_SPLITR = If(beforeSplit, aSpliterator, CType(bSpliterator.trySplit(), T_SPLITR))
				beforeSplit = False
				Return ret
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Function tryAdvance(Of T1)(  consumer As java.util.function.Consumer(Of T1)) As Boolean
				Dim hasNext As Boolean
				If beforeSplit Then
					hasNext = aSpliterator.tryAdvance(consumer)
					If Not hasNext Then
						beforeSplit = False
						hasNext = bSpliterator.tryAdvance(consumer)
					End If
				Else
					hasNext = bSpliterator.tryAdvance(consumer)
				End If
				Return hasNext
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Sub forEachRemaining(Of T1)(  consumer As java.util.function.Consumer(Of T1))
				If beforeSplit Then aSpliterator.forEachRemaining(consumer)
				bSpliterator.forEachRemaining(consumer)
			End Sub

			Public Overrides Function estimateSize() As Long
				If beforeSplit Then
					' If one or both estimates are java.lang.[Long].MAX_VALUE then the sum
					' will either be java.lang.[Long].MAX_VALUE or overflow to a negative value
					Dim size As Long = aSpliterator.estimateSize() + bSpliterator.estimateSize()
					Return If(size >= 0, size, java.lang.[Long].Max_Value)
				Else
					Return bSpliterator.estimateSize()
				End If
			End Function

			Public Overrides Function characteristics() As Integer
				If beforeSplit Then
					' Concatenation loses DISTINCT and SORTED characteristics
					Return aSpliterator.characteristics() And bSpliterator.characteristics() And Not(java.util.Spliterator.DISTINCT Or java.util.Spliterator.SORTED Or (If(unsized, java.util.Spliterator.SIZED Or java.util.Spliterator.SUBSIZED, 0)))
				Else
					Return bSpliterator.characteristics()
				End If
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Public  Overrides ReadOnly Property  comparator As IComparer(Of ?)
				Get
					If beforeSplit Then Throw New IllegalStateException
					Return bSpliterator.comparator
				End Get
			End Property

			Friend Class OfRef(Of T)
				Inherits ConcatSpliterator(Of T, java.util.Spliterator(Of T))

				Friend Sub New(  aSpliterator As java.util.Spliterator(Of T),   bSpliterator As java.util.Spliterator(Of T))
					MyBase.New(aSpliterator, bSpliterator)
				End Sub
			End Class

			Private MustInherit Class OfPrimitive(Of T, T_CONS, T_SPLITR As java.util.Spliterator.OfPrimitive(Of T, T_CONS, T_SPLITR))
				Inherits ConcatSpliterator(Of T, T_SPLITR)
				Implements java.util.Spliterator.OfPrimitive(Of T, T_CONS, T_SPLITR)

				Private Sub New(  aSpliterator As T_SPLITR,   bSpliterator As T_SPLITR)
					MyBase.New(aSpliterator, bSpliterator)
				End Sub

				Public Overrides Function tryAdvance(  action As T_CONS) As Boolean
					Dim hasNext As Boolean
					If outerInstance.beforeSplit Then
						hasNext = outerInstance.aSpliterator.tryAdvance(action)
						If Not hasNext Then
							outerInstance.beforeSplit = False
							hasNext = outerInstance.bSpliterator.tryAdvance(action)
						End If
					Else
						hasNext = outerInstance.bSpliterator.tryAdvance(action)
					End If
					Return hasNext
				End Function

				Public Overrides Sub forEachRemaining(  action As T_CONS)
					If outerInstance.beforeSplit Then outerInstance.aSpliterator.forEachRemaining(action)
					outerInstance.bSpliterator.forEachRemaining(action)
				End Sub
			End Class

			Friend Class OfInt
				Inherits ConcatSpliterator.OfPrimitive(Of Integer?, java.util.function.IntConsumer, java.util.Spliterator.OfInt)
				Implements java.util.Spliterator.OfInt

				Friend Sub New(  aSpliterator As java.util.Spliterator.OfInt,   bSpliterator As java.util.Spliterator.OfInt)
					MyBase.New(aSpliterator, bSpliterator)
				End Sub
			End Class

			Friend Class OfLong
				Inherits ConcatSpliterator.OfPrimitive(Of Long?, java.util.function.LongConsumer, java.util.Spliterator.OfLong)
				Implements java.util.Spliterator.OfLong

				Friend Sub New(  aSpliterator As java.util.Spliterator.OfLong,   bSpliterator As java.util.Spliterator.OfLong)
					MyBase.New(aSpliterator, bSpliterator)
				End Sub
			End Class

			Friend Class OfDouble
				Inherits ConcatSpliterator.OfPrimitive(Of Double?, java.util.function.DoubleConsumer, java.util.Spliterator.OfDouble)
				Implements java.util.Spliterator.OfDouble

				Friend Sub New(  aSpliterator As java.util.Spliterator.OfDouble,   bSpliterator As java.util.Spliterator.OfDouble)
					MyBase.New(aSpliterator, bSpliterator)
				End Sub
			End Class
		End Class

		''' <summary>
		''' Given two Runnables, return a Runnable that executes both in sequence,
		''' even if the first throws an exception, and if both throw exceptions, add
		''' any exceptions thrown by the second as suppressed exceptions of the first.
		''' </summary>
		Friend Shared Function composeWithExceptions(  a As Runnable,   b As Runnable) As Runnable
			Return New RunnableAnonymousInnerClassHelper
		End Function

		Private Class RunnableAnonymousInnerClassHelper
			Implements Runnable

			Public Overrides Sub run() Implements Runnable.run
				Try
					a.run()
				Catch e1 As Throwable
					Try
						b.run()
					Catch e2 As Throwable
						Try
							e1.addSuppressed(e2)
						Catch ignore As Throwable
						End Try
					End Try
					Throw e1
				End Try
				b.run()
			End Sub
		End Class

		''' <summary>
		''' Given two streams, return a Runnable that
		''' executes both of their <seealso cref="BaseStream#close"/> methods in sequence,
		''' even if the first throws an exception, and if both throw exceptions, add
		''' any exceptions thrown by the second as suppressed exceptions of the first.
		''' </summary>
		Friend Shared Function composedClose(Of T1, T2)(  a As BaseStream(Of T1),   b As BaseStream(Of T2)) As Runnable
			Return New RunnableAnonymousInnerClassHelper2
		End Function

		Private Class RunnableAnonymousInnerClassHelper2
			Implements Runnable

			Public Overrides Sub run() Implements Runnable.run
				Try
					a.close()
				Catch e1 As Throwable
					Try
						b.close()
					Catch e2 As Throwable
						Try
							e1.addSuppressed(e2)
						Catch ignore As Throwable
						End Try
					End Try
					Throw e1
				End Try
				b.close()
			End Sub
		End Class
	End Class

End Namespace