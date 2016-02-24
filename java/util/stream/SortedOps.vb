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
	''' Factory methods for transforming streams into sorted streams.
	''' 
	''' @since 1.8
	''' </summary>
	Friend NotInheritable Class SortedOps

		Private Sub New()
		End Sub

		''' <summary>
		''' Appends a "sorted" operation to the provided stream.
		''' </summary>
		''' @param <T> the type of both input and output elements </param>
		''' <param name="upstream"> a reference stream with element type T </param>
		Friend Shared Function makeRef(Of T, T1)(ByVal upstream As AbstractPipeline(Of T1)) As Stream(Of T)
			Return New OfRef(Of )(upstream)
		End Function

		''' <summary>
		''' Appends a "sorted" operation to the provided stream.
		''' </summary>
		''' @param <T> the type of both input and output elements </param>
		''' <param name="upstream"> a reference stream with element type T </param>
		''' <param name="comparator"> the comparator to order elements by </param>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Friend Shared Function makeRef(Of T, T1, T2)(ByVal upstream As AbstractPipeline(Of T1), ByVal comparator As IComparer(Of T2)) As Stream(Of T)
			Return New OfRef(Of )(upstream, comparator)
		End Function

		''' <summary>
		''' Appends a "sorted" operation to the provided stream.
		''' </summary>
		''' @param <T> the type of both input and output elements </param>
		''' <param name="upstream"> a reference stream with element type T </param>
		Friend Shared Function makeInt(Of T, T1)(ByVal upstream As AbstractPipeline(Of T1)) As IntStream
			Return New OfInt(upstream)
		End Function

		''' <summary>
		''' Appends a "sorted" operation to the provided stream.
		''' </summary>
		''' @param <T> the type of both input and output elements </param>
		''' <param name="upstream"> a reference stream with element type T </param>
		Friend Shared Function makeLong(Of T, T1)(ByVal upstream As AbstractPipeline(Of T1)) As LongStream
			Return New OfLong(upstream)
		End Function

		''' <summary>
		''' Appends a "sorted" operation to the provided stream.
		''' </summary>
		''' @param <T> the type of both input and output elements </param>
		''' <param name="upstream"> a reference stream with element type T </param>
		Friend Shared Function makeDouble(Of T, T1)(ByVal upstream As AbstractPipeline(Of T1)) As DoubleStream
			Return New OfDouble(upstream)
		End Function

		''' <summary>
		''' Specialized subtype for sorting reference streams
		''' </summary>
		Private NotInheritable Class OfRef(Of T)
			Inherits ReferencePipeline.StatefulOp(Of T, T)

			''' <summary>
			''' Comparator used for sorting
			''' </summary>
			Private ReadOnly isNaturalSort As Boolean
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Private ReadOnly comparator As IComparer(Of ?)

			''' <summary>
			''' Sort using natural order of {@literal <T>} which must be
			''' {@code Comparable}.
			''' </summary>
			Friend Sub New(Of T1)(ByVal upstream As AbstractPipeline(Of T1))
				MyBase.New(upstream, StreamShape.REFERENCE, StreamOpFlag.IS_ORDERED Or StreamOpFlag.IS_SORTED)
				Me.isNaturalSort = True
				' Will throw CCE when we try to sort if T is not Comparable
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim comp As IComparer(Of ?) = CType(IComparer.naturalOrder(), IComparer(Of ?))
				Me.comparator = comp
			End Sub

			''' <summary>
			''' Sort using the provided comparator.
			''' </summary>
			''' <param name="comparator"> The comparator to be used to evaluate ordering. </param>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1, T2)(ByVal upstream As AbstractPipeline(Of T1), ByVal comparator As IComparer(Of T2))
				MyBase.New(upstream, StreamShape.REFERENCE, StreamOpFlag.IS_ORDERED Or StreamOpFlag.NOT_SORTED)
				Me.isNaturalSort = False
				Me.comparator = java.util.Objects.requireNonNull(comparator)
			End Sub

			Public Overrides Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of T)) As Sink(Of T)
				java.util.Objects.requireNonNull(sink)

				' If the input is already naturally sorted and this operation
				' also naturally sorted then this is a no-op
				If StreamOpFlag.SORTED.isKnown(flags) AndAlso isNaturalSort Then
					Return sink
				ElseIf StreamOpFlag.SIZED.isKnown(flags) Then
					Return New SizedRefSortingSink(Of )(sink, comparator)
				Else
					Return New RefSortingSink(Of )(sink, comparator)
				End If
			End Function

			Public Overrides Function opEvaluateParallel(Of P_IN)(ByVal helper As PipelineHelper(Of T), ByVal spliterator As java.util.Spliterator(Of P_IN), ByVal generator As java.util.function.IntFunction(Of T())) As Node(Of T)
				' If the input is already naturally sorted and this operation
				' naturally sorts then collect the output
				If StreamOpFlag.SORTED.isKnown(helper.streamAndOpFlags) AndAlso isNaturalSort Then
					Return helper.evaluate(spliterator, False, generator)
				Else
					' @@@ Weak two-pass parallel implementation; parallel collect, parallel sort
					Dim flattenedData As T() = helper.evaluate(spliterator, True, generator).asArray(generator)
					java.util.Arrays.parallelSort(flattenedData, comparator)
					Return Nodes.node(flattenedData)
				End If
			End Function
		End Class

		''' <summary>
		''' Specialized subtype for sorting int streams.
		''' </summary>
		Private NotInheritable Class OfInt
			Inherits IntPipeline.StatefulOp(Of Integer?)

			Friend Sub New(Of T1)(ByVal upstream As AbstractPipeline(Of T1))
				MyBase.New(upstream, StreamShape.INT_VALUE, StreamOpFlag.IS_ORDERED Or StreamOpFlag.IS_SORTED)
			End Sub

			Public Overrides Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of Integer?)) As Sink(Of Integer?)
				java.util.Objects.requireNonNull(sink)

				If StreamOpFlag.SORTED.isKnown(flags) Then
					Return sink
				ElseIf StreamOpFlag.SIZED.isKnown(flags) Then
					Return New SizedIntSortingSink(sink)
				Else
					Return New IntSortingSink(sink)
				End If
			End Function

			Public Overrides Function opEvaluateParallel(Of P_IN)(ByVal helper As PipelineHelper(Of Integer?), ByVal spliterator As java.util.Spliterator(Of P_IN), ByVal generator As java.util.function.IntFunction(Of Integer?())) As Node(Of Integer?)
				If StreamOpFlag.SORTED.isKnown(helper.streamAndOpFlags) Then
					Return helper.evaluate(spliterator, False, generator)
				Else
					Dim n As Node.OfInt = CType(helper.evaluate(spliterator, True, generator), Node.OfInt)

					Dim content As Integer() = n.asPrimitiveArray()
					java.util.Arrays.parallelSort(content)

					Return Nodes.node(content)
				End If
			End Function
		End Class

		''' <summary>
		''' Specialized subtype for sorting long streams.
		''' </summary>
		Private NotInheritable Class OfLong
			Inherits LongPipeline.StatefulOp(Of Long?)

			Friend Sub New(Of T1)(ByVal upstream As AbstractPipeline(Of T1))
				MyBase.New(upstream, StreamShape.LONG_VALUE, StreamOpFlag.IS_ORDERED Or StreamOpFlag.IS_SORTED)
			End Sub

			Public Overrides Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of Long?)) As Sink(Of Long?)
				java.util.Objects.requireNonNull(sink)

				If StreamOpFlag.SORTED.isKnown(flags) Then
					Return sink
				ElseIf StreamOpFlag.SIZED.isKnown(flags) Then
					Return New SizedLongSortingSink(sink)
				Else
					Return New LongSortingSink(sink)
				End If
			End Function

			Public Overrides Function opEvaluateParallel(Of P_IN)(ByVal helper As PipelineHelper(Of Long?), ByVal spliterator As java.util.Spliterator(Of P_IN), ByVal generator As java.util.function.IntFunction(Of Long?())) As Node(Of Long?)
				If StreamOpFlag.SORTED.isKnown(helper.streamAndOpFlags) Then
					Return helper.evaluate(spliterator, False, generator)
				Else
					Dim n As Node.OfLong = CType(helper.evaluate(spliterator, True, generator), Node.OfLong)

					Dim content As Long() = n.asPrimitiveArray()
					java.util.Arrays.parallelSort(content)

					Return Nodes.node(content)
				End If
			End Function
		End Class

		''' <summary>
		''' Specialized subtype for sorting double streams.
		''' </summary>
		Private NotInheritable Class OfDouble
			Inherits DoublePipeline.StatefulOp(Of Double?)

			Friend Sub New(Of T1)(ByVal upstream As AbstractPipeline(Of T1))
				MyBase.New(upstream, StreamShape.DOUBLE_VALUE, StreamOpFlag.IS_ORDERED Or StreamOpFlag.IS_SORTED)
			End Sub

			Public Overrides Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of Double?)) As Sink(Of Double?)
				java.util.Objects.requireNonNull(sink)

				If StreamOpFlag.SORTED.isKnown(flags) Then
					Return sink
				ElseIf StreamOpFlag.SIZED.isKnown(flags) Then
					Return New SizedDoubleSortingSink(sink)
				Else
					Return New DoubleSortingSink(sink)
				End If
			End Function

			Public Overrides Function opEvaluateParallel(Of P_IN)(ByVal helper As PipelineHelper(Of Double?), ByVal spliterator As java.util.Spliterator(Of P_IN), ByVal generator As java.util.function.IntFunction(Of Double?())) As Node(Of Double?)
				If StreamOpFlag.SORTED.isKnown(helper.streamAndOpFlags) Then
					Return helper.evaluate(spliterator, False, generator)
				Else
					Dim n As Node.OfDouble = CType(helper.evaluate(spliterator, True, generator), Node.OfDouble)

					Dim content As Double() = n.asPrimitiveArray()
					java.util.Arrays.parallelSort(content)

					Return Nodes.node(content)
				End If
			End Function
		End Class

		''' <summary>
		''' Abstract <seealso cref="Sink"/> for implementing sort on reference streams.
		''' 
		''' <p>
		''' Note: documentation below applies to reference and all primitive sinks.
		''' <p>
		''' Sorting sinks first accept all elements, buffering then into an array
		''' or a re-sizable data structure, if the size of the pipeline is known or
		''' unknown respectively.  At the end of the sink protocol those elements are
		''' sorted and then pushed downstream.
		''' This class records if <seealso cref="#cancellationRequested"/> is called.  If so it
		''' can be inferred that the source pushing source elements into the pipeline
		''' knows that the pipeline is short-circuiting.  In such cases sub-classes
		''' pushing elements downstream will preserve the short-circuiting protocol
		''' by calling {@code downstream.cancellationRequested()} and checking the
		''' result is {@code false} before an element is pushed.
		''' <p>
		''' Note that the above behaviour is an optimization for sorting with
		''' sequential streams.  It is not an error that more elements, than strictly
		''' required to produce a result, may flow through the pipeline.  This can
		''' occur, in general (not restricted to just sorting), for short-circuiting
		''' parallel pipelines.
		''' </summary>
		Private MustInherit Class AbstractRefSortingSink(Of T)
			Inherits Sink.ChainedReference(Of T, T)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Protected Friend ReadOnly comparator As IComparer(Of ?)
			' @@@ could be a lazy final value, if/when support is added
			Protected Friend cancellationWasRequested As Boolean

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1, T2)(ByVal downstream As Sink(Of T1), ByVal comparator As IComparer(Of T2))
				MyBase.New(downstream)
				Me.comparator = comparator
			End Sub

			''' <summary>
			''' Records is cancellation is requested so short-circuiting behaviour
			''' can be preserved when the sorted elements are pushed downstream.
			''' </summary>
			''' <returns> false, as this sink never short-circuits. </returns>
			Public Overrides Function cancellationRequested() As Boolean
				cancellationWasRequested = True
				Return False
			End Function
		End Class

		''' <summary>
		''' <seealso cref="Sink"/> for implementing sort on SIZED reference streams.
		''' </summary>
		Private NotInheritable Class SizedRefSortingSink(Of T)
			Inherits AbstractRefSortingSink(Of T)

			Private array As T()
			Private offset As Integer

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1, T2)(ByVal sink As Sink(Of T1), ByVal comparator As IComparer(Of T2))
				MyBase.New(sink, comparator)
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Overrides Sub begin(ByVal size As Long)
				If size >= Nodes.MAX_ARRAY_SIZE Then Throw New IllegalArgumentException(Nodes.BAD_SIZE)
				array = CType(New Object(CInt(size) - 1){}, T())
			End Sub

			Public Overrides Sub [end]()
				java.util.Arrays.sort(array, 0, offset, comparator)
				downstream.begin(offset)
				If Not cancellationWasRequested Then
					Dim i As Integer = 0
					Do While i < offset
						downstream.accept(array(i))
						i += 1
					Loop
				Else
					Dim i As Integer = 0
					Do While i < offset AndAlso Not downstream.cancellationRequested()
						downstream.accept(array(i))
						i += 1
					Loop
				End If
				downstream.end()
				array = Nothing
			End Sub

			Public Overrides Sub accept(ByVal t As T)
				array(offset) = t
				offset += 1
			End Sub
		End Class

		''' <summary>
		''' <seealso cref="Sink"/> for implementing sort on reference streams.
		''' </summary>
		Private NotInheritable Class RefSortingSink(Of T)
			Inherits AbstractRefSortingSink(Of T)

			Private list As List(Of T)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1, T2)(ByVal sink As Sink(Of T1), ByVal comparator As IComparer(Of T2))
				MyBase.New(sink, comparator)
			End Sub

			Public Overrides Sub begin(ByVal size As Long)
				If size >= Nodes.MAX_ARRAY_SIZE Then Throw New IllegalArgumentException(Nodes.BAD_SIZE)
				list = If(size >= 0, New List(Of T)(CInt(size)), New List(Of T))
			End Sub

			Public Overrides Sub [end]()
				list.sort(comparator)
				downstream.begin(list.size())
				If Not cancellationWasRequested Then
					list.forEach(downstream::accept)
				Else
					For Each t As T In list
						If downstream.cancellationRequested() Then Exit For
						downstream.accept(t)
					Next t
				End If
				downstream.end()
				list = Nothing
			End Sub

			Public Overrides Sub accept(ByVal t As T)
				list.add(t)
			End Sub
		End Class

		''' <summary>
		''' Abstract <seealso cref="Sink"/> for implementing sort on int streams.
		''' </summary>
		Private MustInherit Class AbstractIntSortingSink
			Inherits Sink.ChainedInt(Of Integer?)

			Protected Friend cancellationWasRequested As Boolean

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1)(ByVal downstream As Sink(Of T1))
				MyBase.New(downstream)
			End Sub

			Public Overrides Function cancellationRequested() As Boolean
				cancellationWasRequested = True
				Return False
			End Function
		End Class

		''' <summary>
		''' <seealso cref="Sink"/> for implementing sort on SIZED int streams.
		''' </summary>
		Private NotInheritable Class SizedIntSortingSink
			Inherits AbstractIntSortingSink

			Private array As Integer()
			Private offset As Integer

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1)(ByVal downstream As Sink(Of T1))
				MyBase.New(downstream)
			End Sub

			Public Overrides Sub begin(ByVal size As Long)
				If size >= Nodes.MAX_ARRAY_SIZE Then Throw New IllegalArgumentException(Nodes.BAD_SIZE)
				array = New Integer(CInt(size) - 1){}
			End Sub

			Public Overrides Sub [end]()
				java.util.Arrays.sort(array, 0, offset)
				downstream.begin(offset)
				If Not cancellationWasRequested Then
					Dim i As Integer = 0
					Do While i < offset
						downstream.accept(array(i))
						i += 1
					Loop
				Else
					Dim i As Integer = 0
					Do While i < offset AndAlso Not downstream.cancellationRequested()
						downstream.accept(array(i))
						i += 1
					Loop
				End If
				downstream.end()
				array = Nothing
			End Sub

			Public Overrides Sub accept(ByVal t As Integer)
				array(offset) = t
				offset += 1
			End Sub
		End Class

		''' <summary>
		''' <seealso cref="Sink"/> for implementing sort on int streams.
		''' </summary>
		Private NotInheritable Class IntSortingSink
			Inherits AbstractIntSortingSink

			Private b As SpinedBuffer.OfInt

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1)(ByVal sink As Sink(Of T1))
				MyBase.New(sink)
			End Sub

			Public Overrides Sub begin(ByVal size As Long)
				If size >= Nodes.MAX_ARRAY_SIZE Then Throw New IllegalArgumentException(Nodes.BAD_SIZE)
				b = If(size > 0, New SpinedBuffer.OfInt(CInt(size)), New SpinedBuffer.OfInt)
			End Sub

			Public Overrides Sub [end]()
				Dim ints As Integer() = b.asPrimitiveArray()
				java.util.Arrays.sort(ints)
				downstream.begin(ints.Length)
				If Not cancellationWasRequested Then
					For Each anInt As Integer In ints
						downstream.accept(anInt)
					Next anInt
				Else
					For Each anInt As Integer In ints
						If downstream.cancellationRequested() Then Exit For
						downstream.accept(anInt)
					Next anInt
				End If
				downstream.end()
			End Sub

			Public Overrides Sub accept(ByVal t As Integer)
				b.accept(t)
			End Sub
		End Class

		''' <summary>
		''' Abstract <seealso cref="Sink"/> for implementing sort on long streams.
		''' </summary>
		Private MustInherit Class AbstractLongSortingSink
			Inherits Sink.ChainedLong(Of Long?)

			Protected Friend cancellationWasRequested As Boolean

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1)(ByVal downstream As Sink(Of T1))
				MyBase.New(downstream)
			End Sub

			Public Overrides Function cancellationRequested() As Boolean
				cancellationWasRequested = True
				Return False
			End Function
		End Class

		''' <summary>
		''' <seealso cref="Sink"/> for implementing sort on SIZED long streams.
		''' </summary>
		Private NotInheritable Class SizedLongSortingSink
			Inherits AbstractLongSortingSink

			Private array As Long()
			Private offset As Integer

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1)(ByVal downstream As Sink(Of T1))
				MyBase.New(downstream)
			End Sub

			Public Overrides Sub begin(ByVal size As Long)
				If size >= Nodes.MAX_ARRAY_SIZE Then Throw New IllegalArgumentException(Nodes.BAD_SIZE)
				array = New Long(CInt(size) - 1){}
			End Sub

			Public Overrides Sub [end]()
				java.util.Arrays.sort(array, 0, offset)
				downstream.begin(offset)
				If Not cancellationWasRequested Then
					Dim i As Integer = 0
					Do While i < offset
						downstream.accept(array(i))
						i += 1
					Loop
				Else
					Dim i As Integer = 0
					Do While i < offset AndAlso Not downstream.cancellationRequested()
						downstream.accept(array(i))
						i += 1
					Loop
				End If
				downstream.end()
				array = Nothing
			End Sub

			Public Overrides Sub accept(ByVal t As Long)
				array(offset) = t
				offset += 1
			End Sub
		End Class

		''' <summary>
		''' <seealso cref="Sink"/> for implementing sort on long streams.
		''' </summary>
		Private NotInheritable Class LongSortingSink
			Inherits AbstractLongSortingSink

			Private b As SpinedBuffer.OfLong

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1)(ByVal sink As Sink(Of T1))
				MyBase.New(sink)
			End Sub

			Public Overrides Sub begin(ByVal size As Long)
				If size >= Nodes.MAX_ARRAY_SIZE Then Throw New IllegalArgumentException(Nodes.BAD_SIZE)
				b = If(size > 0, New SpinedBuffer.OfLong(CInt(size)), New SpinedBuffer.OfLong)
			End Sub

			Public Overrides Sub [end]()
				Dim longs As Long() = b.asPrimitiveArray()
				java.util.Arrays.sort(longs)
				downstream.begin(longs.Length)
				If Not cancellationWasRequested Then
					For Each aLong As Long In longs
						downstream.accept(aLong)
					Next aLong
				Else
					For Each aLong As Long In longs
						If downstream.cancellationRequested() Then Exit For
						downstream.accept(aLong)
					Next aLong
				End If
				downstream.end()
			End Sub

			Public Overrides Sub accept(ByVal t As Long)
				b.accept(t)
			End Sub
		End Class

		''' <summary>
		''' Abstract <seealso cref="Sink"/> for implementing sort on long streams.
		''' </summary>
		Private MustInherit Class AbstractDoubleSortingSink
			Inherits Sink.ChainedDouble(Of Double?)

			Protected Friend cancellationWasRequested As Boolean

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1)(ByVal downstream As Sink(Of T1))
				MyBase.New(downstream)
			End Sub

			Public Overrides Function cancellationRequested() As Boolean
				cancellationWasRequested = True
				Return False
			End Function
		End Class

		''' <summary>
		''' <seealso cref="Sink"/> for implementing sort on SIZED double streams.
		''' </summary>
		Private NotInheritable Class SizedDoubleSortingSink
			Inherits AbstractDoubleSortingSink

			Private array As Double()
			Private offset As Integer

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1)(ByVal downstream As Sink(Of T1))
				MyBase.New(downstream)
			End Sub

			Public Overrides Sub begin(ByVal size As Long)
				If size >= Nodes.MAX_ARRAY_SIZE Then Throw New IllegalArgumentException(Nodes.BAD_SIZE)
				array = New Double(CInt(size) - 1){}
			End Sub

			Public Overrides Sub [end]()
				java.util.Arrays.sort(array, 0, offset)
				downstream.begin(offset)
				If Not cancellationWasRequested Then
					Dim i As Integer = 0
					Do While i < offset
						downstream.accept(array(i))
						i += 1
					Loop
				Else
					Dim i As Integer = 0
					Do While i < offset AndAlso Not downstream.cancellationRequested()
						downstream.accept(array(i))
						i += 1
					Loop
				End If
				downstream.end()
				array = Nothing
			End Sub

			Public Overrides Sub accept(ByVal t As Double)
				array(offset) = t
				offset += 1
			End Sub
		End Class

		''' <summary>
		''' <seealso cref="Sink"/> for implementing sort on double streams.
		''' </summary>
		Private NotInheritable Class DoubleSortingSink
			Inherits AbstractDoubleSortingSink

			Private b As SpinedBuffer.OfDouble

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1)(ByVal sink As Sink(Of T1))
				MyBase.New(sink)
			End Sub

			Public Overrides Sub begin(ByVal size As Long)
				If size >= Nodes.MAX_ARRAY_SIZE Then Throw New IllegalArgumentException(Nodes.BAD_SIZE)
				b = If(size > 0, New SpinedBuffer.OfDouble(CInt(size)), New SpinedBuffer.OfDouble)
			End Sub

			Public Overrides Sub [end]()
				Dim doubles As Double() = b.asPrimitiveArray()
				java.util.Arrays.sort(doubles)
				downstream.begin(doubles.Length)
				If Not cancellationWasRequested Then
					For Each aDouble As Double In doubles
						downstream.accept(aDouble)
					Next aDouble
				Else
					For Each aDouble As Double In doubles
						If downstream.cancellationRequested() Then Exit For
						downstream.accept(aDouble)
					Next aDouble
				End If
				downstream.end()
			End Sub

			Public Overrides Sub accept(ByVal t As Double)
				b.accept(t)
			End Sub
		End Class
	End Class

End Namespace