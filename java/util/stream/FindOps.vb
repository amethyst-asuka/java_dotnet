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
	''' Factory for instances of a short-circuiting {@code TerminalOp} that searches
	''' for an element in a stream pipeline, and terminates when it finds one.
	''' Supported variants include find-first (find the first element in the
	''' encounter order) and find-any (find any element, may not be the first in
	''' encounter order.)
	''' 
	''' @since 1.8
	''' </summary>
	Friend NotInheritable Class FindOps

		Private Sub New()
		End Sub

		''' <summary>
		''' Constructs a {@code TerminalOp} for streams of objects.
		''' </summary>
		''' @param <T> the type of elements of the stream </param>
		''' <param name="mustFindFirst"> whether the {@code TerminalOp} must produce the
		'''        first element in the encounter order </param>
		''' <returns> a {@code TerminalOp} implementing the find operation </returns>
		Public Shared Function makeRef(Of T)(ByVal mustFindFirst As Boolean) As TerminalOp(Of T, java.util.Optional(Of T))
			Return New FindOp(Of )(mustFindFirst, StreamShape.REFERENCE, java.util.Optional.empty(), java.util.Optional::isPresent, FindSink.OfRef::New)
		End Function

		''' <summary>
		''' Constructs a {@code TerminalOp} for streams of ints.
		''' </summary>
		''' <param name="mustFindFirst"> whether the {@code TerminalOp} must produce the
		'''        first element in the encounter order </param>
		''' <returns> a {@code TerminalOp} implementing the find operation </returns>
		Public Shared Function makeInt(ByVal mustFindFirst As Boolean) As TerminalOp(Of Integer?, java.util.OptionalInt)
			Return New FindOp(Of )(mustFindFirst, StreamShape.INT_VALUE, java.util.OptionalInt.empty(), java.util.OptionalInt::isPresent, FindSink.OfInt::New)
		End Function

		''' <summary>
		''' Constructs a {@code TerminalOp} for streams of longs.
		''' </summary>
		''' <param name="mustFindFirst"> whether the {@code TerminalOp} must produce the
		'''        first element in the encounter order </param>
		''' <returns> a {@code TerminalOp} implementing the find operation </returns>
		Public Shared Function makeLong(ByVal mustFindFirst As Boolean) As TerminalOp(Of Long?, java.util.OptionalLong)
			Return New FindOp(Of )(mustFindFirst, StreamShape.LONG_VALUE, java.util.OptionalLong.empty(), java.util.OptionalLong::isPresent, FindSink.OfLong::New)
		End Function

		''' <summary>
		''' Constructs a {@code FindOp} for streams of doubles.
		''' </summary>
		''' <param name="mustFindFirst"> whether the {@code TerminalOp} must produce the
		'''        first element in the encounter order </param>
		''' <returns> a {@code TerminalOp} implementing the find operation </returns>
		Public Shared Function makeDouble(ByVal mustFindFirst As Boolean) As TerminalOp(Of Double?, java.util.OptionalDouble)
			Return New FindOp(Of )(mustFindFirst, StreamShape.DOUBLE_VALUE, java.util.OptionalDouble.empty(), java.util.OptionalDouble::isPresent, FindSink.OfDouble::New)
		End Function

		''' <summary>
		''' A short-circuiting {@code TerminalOp} that searches for an element in a
		''' stream pipeline, and terminates when it finds one.  Implements both
		''' find-first (find the first element in the encounter order) and find-any
		''' (find any element, may not be the first in encounter order.)
		''' </summary>
		''' @param <T> the output type of the stream pipeline </param>
		''' @param <O> the result type of the find operation, typically an optional
		'''        type </param>
		Private NotInheritable Class FindOp(Of T, O)
			Implements TerminalOp(Of T, O)

			Private ReadOnly shape As StreamShape
			Friend ReadOnly mustFindFirst As Boolean
			Friend ReadOnly emptyValue As O
			Friend ReadOnly presentPredicate As java.util.function.Predicate(Of O)
			Friend ReadOnly sinkSupplier As java.util.function.Supplier(Of TerminalSink(Of T, O))

			''' <summary>
			''' Constructs a {@code FindOp}.
			''' </summary>
			''' <param name="mustFindFirst"> if true, must find the first element in
			'''        encounter order, otherwise can find any element </param>
			''' <param name="shape"> stream shape of elements to search </param>
			''' <param name="emptyValue"> result value corresponding to "found nothing" </param>
			''' <param name="presentPredicate"> {@code Predicate} on result value
			'''        corresponding to "found something" </param>
			''' <param name="sinkSupplier"> supplier for a {@code TerminalSink} implementing
			'''        the matching functionality </param>
			Friend Sub New(ByVal mustFindFirst As Boolean, ByVal shape As StreamShape, ByVal emptyValue As O, ByVal presentPredicate As java.util.function.Predicate(Of O), ByVal sinkSupplier As java.util.function.Supplier(Of TerminalSink(Of T, O)))
				Me.mustFindFirst = mustFindFirst
				Me.shape = shape
				Me.emptyValue = emptyValue
				Me.presentPredicate = presentPredicate
				Me.sinkSupplier = sinkSupplier
			End Sub

			Public  Overrides ReadOnly Property  opFlags As Integer Implements TerminalOp(Of T, O).getOpFlags
				Get
					Return StreamOpFlag.IS_SHORT_CIRCUIT Or (If(mustFindFirst, 0, StreamOpFlag.NOT_ORDERED))
				End Get
			End Property

			Public Overrides Function inputShape() As StreamShape Implements TerminalOp(Of T, O).inputShape
				Return shape
			End Function

			Public Overrides Function evaluateSequential(Of S)(ByVal helper As PipelineHelper(Of T), ByVal spliterator As java.util.Spliterator(Of S)) As O
				Dim result As O = helper.wrapAndCopyInto(sinkSupplier.get(), spliterator).get()
				Return If(result IsNot Nothing, result, emptyValue)
			End Function

			Public Overrides Function evaluateParallel(Of P_IN)(ByVal helper As PipelineHelper(Of T), ByVal spliterator As java.util.Spliterator(Of P_IN)) As O
				Return (New FindTask(Of )(Me, helper, spliterator)).invoke()
			End Function
		End Class

		''' <summary>
		''' Implementation of @{code TerminalSink} that implements the find
		''' functionality, requesting cancellation when something has been found
		''' </summary>
		''' @param <T> The type of input element </param>
		''' @param <O> The result type, typically an optional type </param>
		Private MustInherit Class FindSink(Of T, O)
			Implements TerminalSink(Of T, O)

			Friend hasValue As Boolean
			Friend value As T

			Friend Sub New() ' Avoid creation of special accessor
			End Sub

			Public Overrides Sub accept(ByVal value As T)
				If Not hasValue Then
					hasValue = True
					Me.value = value
				End If
			End Sub

			Public Overrides Function cancellationRequested() As Boolean
				Return hasValue
			End Function

			''' <summary>
			''' Specialization of {@code FindSink} for reference streams </summary>
			Friend NotInheritable Class OfRef(Of T)
				Inherits FindSink(Of T, java.util.Optional(Of T))

				Public Overrides Function [get]() As java.util.Optional(Of T)
					Return If(hasValue, java.util.Optional.of(value), Nothing)
				End Function
			End Class

			''' <summary>
			''' Specialization of {@code FindSink} for int streams </summary>
			Friend NotInheritable Class OfInt
				Inherits FindSink(Of Integer?, java.util.OptionalInt)
				Implements Sink.OfInt

				Public Overrides Sub accept(ByVal value As Integer)
					' Boxing is OK here, since few values will actually flow into the sink
					accept(CInt(value))
				End Sub

				Public Overrides Function [get]() As java.util.OptionalInt
					Return If(hasValue, java.util.OptionalInt.of(value), Nothing)
				End Function
			End Class

			''' <summary>
			''' Specialization of {@code FindSink} for long streams </summary>
			Friend NotInheritable Class OfLong
				Inherits FindSink(Of Long?, java.util.OptionalLong)
				Implements Sink.OfLong

				Public Overrides Sub accept(ByVal value As Long)
					' Boxing is OK here, since few values will actually flow into the sink
					accept(CLng(value))
				End Sub

				Public Overrides Function [get]() As java.util.OptionalLong
					Return If(hasValue, java.util.OptionalLong.of(value), Nothing)
				End Function
			End Class

			''' <summary>
			''' Specialization of {@code FindSink} for double streams </summary>
			Friend NotInheritable Class OfDouble
				Inherits FindSink(Of Double?, java.util.OptionalDouble)
				Implements Sink.OfDouble

				Public Overrides Sub accept(ByVal value As Double)
					' Boxing is OK here, since few values will actually flow into the sink
					accept(CDbl(value))
				End Sub

				Public Overrides Function [get]() As java.util.OptionalDouble
					Return If(hasValue, java.util.OptionalDouble.of(value), Nothing)
				End Function
			End Class
		End Class

		''' <summary>
		''' {@code ForkJoinTask} implementing parallel short-circuiting search </summary>
		''' @param <P_IN> Input element type to the stream pipeline </param>
		''' @param <P_OUT> Output element type from the stream pipeline </param>
		''' @param <O> Result type from the find operation </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private NotInheritable Class FindTask(Of P_IN, P_OUT, O)
			Inherits AbstractShortCircuitTask(Of P_IN, P_OUT, O, FindTask(Of P_IN, P_OUT, O))

			Private ReadOnly op As FindOp(Of P_OUT, O)

			Friend Sub New(ByVal op As FindOp(Of P_OUT, O), ByVal helper As PipelineHelper(Of P_OUT), ByVal spliterator As java.util.Spliterator(Of P_IN))
				MyBase.New(helper, spliterator)
				Me.op = op
			End Sub

			Friend Sub New(ByVal parent As FindTask(Of P_IN, P_OUT, O), ByVal spliterator As java.util.Spliterator(Of P_IN))
				MyBase.New(parent, spliterator)
				Me.op = parent.op
			End Sub

			Protected Friend Overrides Function makeChild(ByVal spliterator As java.util.Spliterator(Of P_IN)) As FindTask(Of P_IN, P_OUT, O)
				Return New FindTask(Of )(Me, spliterator)
			End Function

			Protected Friend  Overrides ReadOnly Property  emptyResult As O
				Get
					Return op.emptyValue
				End Get
			End Property

			Private Sub foundResult(ByVal answer As O)
				If leftmostNode Then
					shortCircuit(answer)
				Else
					cancelLaterNodes()
				End If
			End Sub

			Protected Friend Overrides Function doLeaf() As O
				Dim result As O = helper.wrapAndCopyInto(op.sinkSupplier.get(), spliterator).get()
				If Not op.mustFindFirst Then
					If result IsNot Nothing Then shortCircuit(result)
					Return Nothing
				Else
					If result IsNot Nothing Then
						foundResult(result)
						Return result
					Else
						Return Nothing
					End If
				End If
			End Function

			Public Overrides Sub onCompletion(Of T1)(ByVal caller As java.util.concurrent.CountedCompleter(Of T1))
				If op.mustFindFirst Then
						Dim child As FindTask(Of P_IN, P_OUT, O) = leftChild
						Dim p As FindTask(Of P_IN, P_OUT, O) = Nothing
						Do While child IsNot p
						Dim result As O = child.localResult
						If result IsNot Nothing AndAlso op.presentPredicate.test(result) Then
							localResult = result
							foundResult(result)
							Exit Do
						End If
							p = child
							child = rightChild
						Loop
				End If
				MyBase.onCompletion(caller)
			End Sub
		End Class
	End Class


End Namespace