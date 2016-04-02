Imports System
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
	''' Factory for instances of a short-circuiting stateful intermediate operations
	''' that produce subsequences of their input stream.
	''' 
	''' @since 1.8
	''' </summary>
	Friend NotInheritable Class SliceOps

		' No instances
		Private Sub New()
		End Sub

		''' <summary>
		''' Calculates the sliced size given the current size, number of elements
		''' skip, and the number of elements to limit.
		''' </summary>
		''' <param name="size"> the current size </param>
		''' <param name="skip"> the number of elements to skip, assumed to be >= 0 </param>
		''' <param name="limit"> the number of elements to limit, assumed to be >= 0, with
		'''        a value of {@code java.lang.[Long].MAX_VALUE} if there is no limit </param>
		''' <returns> the sliced size </returns>
		Private Shared Function calcSize(ByVal size As Long, ByVal skip As Long, ByVal limit As Long) As Long
			Return If(size >= 0, System.Math.Max(-1, System.Math.Min(size - skip, limit)), -1)
		End Function

		''' <summary>
		''' Calculates the slice fence, which is one past the index of the slice
		''' range </summary>
		''' <param name="skip"> the number of elements to skip, assumed to be >= 0 </param>
		''' <param name="limit"> the number of elements to limit, assumed to be >= 0, with
		'''        a value of {@code java.lang.[Long].MAX_VALUE} if there is no limit </param>
		''' <returns> the slice fence. </returns>
		Private Shared Function calcSliceFence(ByVal skip As Long, ByVal limit As Long) As Long
			Dim sliceFence As Long = If(limit >= 0, skip + limit, java.lang.[Long].Max_Value)
			' Check for overflow
			Return If(sliceFence >= 0, sliceFence, java.lang.[Long].Max_Value)
		End Function

		''' <summary>
		''' Creates a slice spliterator given a stream shape governing the
		''' spliterator type.  Requires that the underlying Spliterator
		''' be SUBSIZED.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Shared Function sliceSpliterator(Of P_IN)(ByVal shape As StreamShape, ByVal s As java.util.Spliterator(Of P_IN), ByVal skip As Long, ByVal limit As Long) As java.util.Spliterator(Of P_IN)
			Debug.Assert(s.hasCharacteristics(java.util.Spliterator.SUBSIZED))
			Dim sliceFence As Long = calcSliceFence(skip, limit)
			Select Case shape
				Case StreamShape.REFERENCE
					Return New StreamSpliterators.SliceSpliterator.OfRef(Of )(s, skip, sliceFence)
				Case StreamShape.INT_VALUE
					Return CType(New StreamSpliterators.SliceSpliterator.OfInt(CType(s, java.util.Spliterator.OfInt), skip, sliceFence), java.util.Spliterator(Of P_IN))
				Case StreamShape.LONG_VALUE
					Return CType(New StreamSpliterators.SliceSpliterator.OfLong(CType(s, java.util.Spliterator.OfLong), skip, sliceFence), java.util.Spliterator(Of P_IN))
				Case StreamShape.DOUBLE_VALUE
					Return CType(New StreamSpliterators.SliceSpliterator.OfDouble(CType(s, java.util.Spliterator.OfDouble), skip, sliceFence), java.util.Spliterator(Of P_IN))
				Case Else
					Throw New IllegalStateException("Unknown shape " & shape)
			End Select
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Shared Function castingArray(Of T)() As java.util.function.IntFunction(Of T())
			Return size -> (T()) New Object(size - 1){}
		End Function

		''' <summary>
		''' Appends a "slice" operation to the provided stream.  The slice operation
		''' may be may be skip-only, limit-only, or skip-and-limit.
		''' </summary>
		''' @param <T> the type of both input and output elements </param>
		''' <param name="upstream"> a reference stream with element type T </param>
		''' <param name="skip"> the number of elements to skip.  Must be >= 0. </param>
		''' <param name="limit"> the maximum size of the resulting stream, or -1 if no limit
		'''        is to be imposed </param>
		Public Shared Function makeRef(Of T, T1)(ByVal upstream As AbstractPipeline(Of T1), ByVal skip As Long, ByVal limit As Long) As Stream(Of T)
			If skip < 0 Then Throw New IllegalArgumentException("Skip must be non-negative: " & skip)

			Return New StatefulOpAnonymousInnerClassHelper(Of E_IN, E_OUT)
		End Function

		Private Class StatefulOpAnonymousInnerClassHelper(Of E_IN, E_OUT)
			Inherits StatefulOp(Of E_IN, E_OUT)

			Friend Overridable Function unorderedSkipLimitSpliterator(ByVal s As java.util.Spliterator(Of T), ByVal skip As Long, ByVal limit As Long, ByVal sizeIfKnown As Long) As java.util.Spliterator(Of T)
				If skip <= sizeIfKnown Then
					' Use just the limit if the number of elements
					' to skip is <= the known pipeline size
					limit = If(limit >= 0, System.Math.Min(limit, sizeIfKnown - skip), sizeIfKnown - skip)
					skip = 0
				End If
				Return New StreamSpliterators.UnorderedSliceSpliterator.OfRef(Of )(s, skip, limit)
			End Function

			 Friend Overrides Function opEvaluateParallelLazy(Of P_IN)(ByVal helper As PipelineHelper(Of T), ByVal spliterator As java.util.Spliterator(Of P_IN)) As java.util.Spliterator(Of T)
				Dim size As Long = helper.exactOutputSizeIfKnown(spliterator)
				If size > 0 AndAlso spliterator.hasCharacteristics(java.util.Spliterator.SUBSIZED) Then
					Return New StreamSpliterators.SliceSpliterator.OfRef(Of )(helper.wrapSpliterator(spliterator), skip, calcSliceFence(skip, limit))
				ElseIf Not StreamOpFlag.ORDERED.isKnown(helper.streamAndOpFlags) Then
					Return unorderedSkipLimitSpliterator(helper.wrapSpliterator(spliterator), skip, limit, size)
				Else
					' @@@ OOMEs will occur for LongStream.longs().filter(i -> true).limit(n)
					'     regardless of the value of n
					'     Need to adjust the target size of splitting for the
					'     SliceTask from say (size / k) to say min(size / k, 1 << 14)
					'     This will limit the size of the buffers created at the leaf nodes
					'     cancellation will be more aggressive cancelling later tasks
					'     if the target slice size has been reached from a given task,
					'     cancellation should also clear local results if any
					Return (New SliceTask(Of )(Me, helper, spliterator, castingArray(), skip, limit)).invoke().spliterator()
				End If
			 End Function

			 Friend Overrides Function opEvaluateParallel(Of P_IN)(ByVal helper As PipelineHelper(Of T), ByVal spliterator As java.util.Spliterator(Of P_IN), ByVal generator As java.util.function.IntFunction(Of T())) As Node(Of T)
				Dim size As Long = helper.exactOutputSizeIfKnown(spliterator)
				If size > 0 AndAlso spliterator.hasCharacteristics(java.util.Spliterator.SUBSIZED) Then
					' Because the pipeline is SIZED the slice spliterator
					' can be created from the source, this requires matching
					' to shape of the source, and is potentially more efficient
					' than creating the slice spliterator from the pipeline
					' wrapping spliterator
					Dim s As java.util.Spliterator(Of P_IN) = sliceSpliterator(helper.sourceShape, spliterator, skip, limit)
					Return Nodes.collect(helper, s, True, generator)
				ElseIf Not StreamOpFlag.ORDERED.isKnown(helper.streamAndOpFlags) Then
					Dim s As java.util.Spliterator(Of T) = unorderedSkipLimitSpliterator(helper.wrapSpliterator(spliterator), skip, limit, size)
					' Collect using this pipeline, which is empty and therefore
					' can be used with the pipeline wrapping spliterator
					' Note that we cannot create a slice spliterator from
					' the source spliterator if the pipeline is not SIZED
					Return Nodes.collect(Me, s, True, generator)
				Else
					Return (New SliceTask(Of )(Me, helper, spliterator, generator, skip, limit)).invoke()
				End If
			 End Function

			Friend Overrides Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of T)) As Sink(Of T)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return New Sink.ChainedReference<T, T>(sink)
	'			{
	'				long n = skip;
	'				long m = limit >= 0 ? limit : java.lang.[Long].MAX_VALUE;
	'
	'				@Override public  Sub  begin(long size)
	'				{
	'					downstream.begin(calcSize(size, skip, m));
	'				}
	'
	'				@Override public  Sub  accept(T t)
	'				{
	'					if (n == 0)
	'					{
	'						if (m > 0)
	'						{
	'							m -= 1;
	'							downstream.accept(t);
	'						}
	'					}
	'					else
	'					{
	'						n -= 1;
	'					}
	'				}
	'
	'				@Override public boolean cancellationRequested()
	'				{
	'					Return m == 0 || downstream.cancellationRequested();
	'				}
	'			};
			End Function
		End Class

		''' <summary>
		''' Appends a "slice" operation to the provided IntStream.  The slice
		''' operation may be may be skip-only, limit-only, or skip-and-limit.
		''' </summary>
		''' <param name="upstream"> An IntStream </param>
		''' <param name="skip"> The number of elements to skip.  Must be >= 0. </param>
		''' <param name="limit"> The maximum size of the resulting stream, or -1 if no limit
		'''        is to be imposed </param>
		Public Shared Function makeInt(Of T1)(ByVal upstream As AbstractPipeline(Of T1), ByVal skip As Long, ByVal limit As Long) As IntStream
			If skip < 0 Then Throw New IllegalArgumentException("Skip must be non-negative: " & skip)

			Return New StatefulOpAnonymousInnerClassHelper(Of E_IN)
		End Function

		Private Class StatefulOpAnonymousInnerClassHelper(Of E_IN)
			Inherits StatefulOp(Of E_IN)

			Friend Overridable Function unorderedSkipLimitSpliterator(ByVal s As java.util.Spliterator.OfInt, ByVal skip As Long, ByVal limit As Long, ByVal sizeIfKnown As Long) As java.util.Spliterator.OfInt
				If skip <= sizeIfKnown Then
					' Use just the limit if the number of elements
					' to skip is <= the known pipeline size
					limit = If(limit >= 0, System.Math.Min(limit, sizeIfKnown - skip), sizeIfKnown - skip)
					skip = 0
				End If
				Return New StreamSpliterators.UnorderedSliceSpliterator.OfInt(s, skip, limit)
			End Function

			 Friend Overrides Function opEvaluateParallelLazy(Of P_IN)(ByVal helper As PipelineHelper(Of Integer?), ByVal spliterator As java.util.Spliterator(Of P_IN)) As java.util.Spliterator(Of Integer?)
				Dim size As Long = helper.exactOutputSizeIfKnown(spliterator)
				If size > 0 AndAlso spliterator.hasCharacteristics(java.util.Spliterator.SUBSIZED) Then
					Return New StreamSpliterators.SliceSpliterator.OfInt(CType(helper.wrapSpliterator(spliterator), java.util.Spliterator.OfInt), skip, calcSliceFence(skip, limit))
				ElseIf Not StreamOpFlag.ORDERED.isKnown(helper.streamAndOpFlags) Then
					Return unorderedSkipLimitSpliterator(CType(helper.wrapSpliterator(spliterator), java.util.Spliterator.OfInt), skip, limit, size)
				Else
					Return (New SliceTask(Of )(Me, helper, spliterator, java.lang.Integer() ::New, skip, limit)).invoke().spliterator()
				End If
			 End Function

			 Friend Overrides Function opEvaluateParallel(Of P_IN)(ByVal helper As PipelineHelper(Of Integer?), ByVal spliterator As java.util.Spliterator(Of P_IN), ByVal generator As java.util.function.IntFunction(Of Integer?())) As Node(Of Integer?)
				Dim size As Long = helper.exactOutputSizeIfKnown(spliterator)
				If size > 0 AndAlso spliterator.hasCharacteristics(java.util.Spliterator.SUBSIZED) Then
					' Because the pipeline is SIZED the slice spliterator
					' can be created from the source, this requires matching
					' to shape of the source, and is potentially more efficient
					' than creating the slice spliterator from the pipeline
					' wrapping spliterator
					Dim s As java.util.Spliterator(Of P_IN) = sliceSpliterator(helper.sourceShape, spliterator, skip, limit)
					Return Nodes.collectInt(helper, s, True)
				ElseIf Not StreamOpFlag.ORDERED.isKnown(helper.streamAndOpFlags) Then
					Dim s As java.util.Spliterator.OfInt = unorderedSkipLimitSpliterator(CType(helper.wrapSpliterator(spliterator), java.util.Spliterator.OfInt), skip, limit, size)
					' Collect using this pipeline, which is empty and therefore
					' can be used with the pipeline wrapping spliterator
					' Note that we cannot create a slice spliterator from
					' the source spliterator if the pipeline is not SIZED
					Return Nodes.collectInt(Me, s, True)
				Else
					Return (New SliceTask(Of )(Me, helper, spliterator, generator, skip, limit)).invoke()
				End If
			 End Function

			Friend Overrides Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of Integer?)) As Sink(Of Integer?)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return New Sink.ChainedInt<java.lang.Integer>(sink)
	'			{
	'				long n = skip;
	'				long m = limit >= 0 ? limit : java.lang.[Long].MAX_VALUE;
	'
	'				@Override public  Sub  begin(long size)
	'				{
	'					downstream.begin(calcSize(size, skip, m));
	'				}
	'
	'				@Override public  Sub  accept(int t)
	'				{
	'					if (n == 0)
	'					{
	'						if (m > 0)
	'						{
	'							m -= 1;
	'							downstream.accept(t);
	'						}
	'					}
	'					else
	'					{
	'						n -= 1;
	'					}
	'				}
	'
	'				@Override public boolean cancellationRequested()
	'				{
	'					Return m == 0 || downstream.cancellationRequested();
	'				}
	'			};
			End Function
		End Class

		''' <summary>
		''' Appends a "slice" operation to the provided LongStream.  The slice
		''' operation may be may be skip-only, limit-only, or skip-and-limit.
		''' </summary>
		''' <param name="upstream"> A LongStream </param>
		''' <param name="skip"> The number of elements to skip.  Must be >= 0. </param>
		''' <param name="limit"> The maximum size of the resulting stream, or -1 if no limit
		'''        is to be imposed </param>
		Public Shared Function makeLong(Of T1)(ByVal upstream As AbstractPipeline(Of T1), ByVal skip As Long, ByVal limit As Long) As LongStream
			If skip < 0 Then Throw New IllegalArgumentException("Skip must be non-negative: " & skip)

			Return New StatefulOpAnonymousInnerClassHelper(Of E_IN)
		End Function

		Private Class StatefulOpAnonymousInnerClassHelper(Of E_IN)
			Inherits StatefulOp(Of E_IN)

			Friend Overridable Function unorderedSkipLimitSpliterator(ByVal s As java.util.Spliterator.OfLong, ByVal skip As Long, ByVal limit As Long, ByVal sizeIfKnown As Long) As java.util.Spliterator.OfLong
				If skip <= sizeIfKnown Then
					' Use just the limit if the number of elements
					' to skip is <= the known pipeline size
					limit = If(limit >= 0, System.Math.Min(limit, sizeIfKnown - skip), sizeIfKnown - skip)
					skip = 0
				End If
				Return New StreamSpliterators.UnorderedSliceSpliterator.OfLong(s, skip, limit)
			End Function

			 Friend Overrides Function opEvaluateParallelLazy(Of P_IN)(ByVal helper As PipelineHelper(Of Long?), ByVal spliterator As java.util.Spliterator(Of P_IN)) As java.util.Spliterator(Of Long?)
				Dim size As Long = helper.exactOutputSizeIfKnown(spliterator)
				If size > 0 AndAlso spliterator.hasCharacteristics(java.util.Spliterator.SUBSIZED) Then
					Return New StreamSpliterators.SliceSpliterator.OfLong(CType(helper.wrapSpliterator(spliterator), java.util.Spliterator.OfLong), skip, calcSliceFence(skip, limit))
				ElseIf Not StreamOpFlag.ORDERED.isKnown(helper.streamAndOpFlags) Then
					Return unorderedSkipLimitSpliterator(CType(helper.wrapSpliterator(spliterator), java.util.Spliterator.OfLong), skip, limit, size)
				Else
					Return (New SliceTask(Of )(Me, helper, spliterator, java.lang.Long() ::New, skip, limit)).invoke().spliterator()
				End If
			 End Function

			 Friend Overrides Function opEvaluateParallel(Of P_IN)(ByVal helper As PipelineHelper(Of Long?), ByVal spliterator As java.util.Spliterator(Of P_IN), ByVal generator As java.util.function.IntFunction(Of Long?())) As Node(Of Long?)
				Dim size As Long = helper.exactOutputSizeIfKnown(spliterator)
				If size > 0 AndAlso spliterator.hasCharacteristics(java.util.Spliterator.SUBSIZED) Then
					' Because the pipeline is SIZED the slice spliterator
					' can be created from the source, this requires matching
					' to shape of the source, and is potentially more efficient
					' than creating the slice spliterator from the pipeline
					' wrapping spliterator
					Dim s As java.util.Spliterator(Of P_IN) = sliceSpliterator(helper.sourceShape, spliterator, skip, limit)
					Return Nodes.collectLong(helper, s, True)
				ElseIf Not StreamOpFlag.ORDERED.isKnown(helper.streamAndOpFlags) Then
					Dim s As java.util.Spliterator.OfLong = unorderedSkipLimitSpliterator(CType(helper.wrapSpliterator(spliterator), java.util.Spliterator.OfLong), skip, limit, size)
					' Collect using this pipeline, which is empty and therefore
					' can be used with the pipeline wrapping spliterator
					' Note that we cannot create a slice spliterator from
					' the source spliterator if the pipeline is not SIZED
					Return Nodes.collectLong(Me, s, True)
				Else
					Return (New SliceTask(Of )(Me, helper, spliterator, generator, skip, limit)).invoke()
				End If
			 End Function

			Friend Overrides Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of Long?)) As Sink(Of Long?)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return New Sink.ChainedLong<java.lang.Long>(sink)
	'			{
	'				long n = skip;
	'				long m = limit >= 0 ? limit : java.lang.[Long].MAX_VALUE;
	'
	'				@Override public  Sub  begin(long size)
	'				{
	'					downstream.begin(calcSize(size, skip, m));
	'				}
	'
	'				@Override public  Sub  accept(long t)
	'				{
	'					if (n == 0)
	'					{
	'						if (m > 0)
	'						{
	'							m -= 1;
	'							downstream.accept(t);
	'						}
	'					}
	'					else
	'					{
	'						n -= 1;
	'					}
	'				}
	'
	'				@Override public boolean cancellationRequested()
	'				{
	'					Return m == 0 || downstream.cancellationRequested();
	'				}
	'			};
			End Function
		End Class

		''' <summary>
		''' Appends a "slice" operation to the provided DoubleStream.  The slice
		''' operation may be may be skip-only, limit-only, or skip-and-limit.
		''' </summary>
		''' <param name="upstream"> A DoubleStream </param>
		''' <param name="skip"> The number of elements to skip.  Must be >= 0. </param>
		''' <param name="limit"> The maximum size of the resulting stream, or -1 if no limit
		'''        is to be imposed </param>
		Public Shared Function makeDouble(Of T1)(ByVal upstream As AbstractPipeline(Of T1), ByVal skip As Long, ByVal limit As Long) As DoubleStream
			If skip < 0 Then Throw New IllegalArgumentException("Skip must be non-negative: " & skip)

			Return New StatefulOpAnonymousInnerClassHelper(Of E_IN)
		End Function

		Private Class StatefulOpAnonymousInnerClassHelper(Of E_IN)
			Inherits StatefulOp(Of E_IN)

			Friend Overridable Function unorderedSkipLimitSpliterator(ByVal s As java.util.Spliterator.OfDouble, ByVal skip As Long, ByVal limit As Long, ByVal sizeIfKnown As Long) As java.util.Spliterator.OfDouble
				If skip <= sizeIfKnown Then
					' Use just the limit if the number of elements
					' to skip is <= the known pipeline size
					limit = If(limit >= 0, System.Math.Min(limit, sizeIfKnown - skip), sizeIfKnown - skip)
					skip = 0
				End If
				Return New StreamSpliterators.UnorderedSliceSpliterator.OfDouble(s, skip, limit)
			End Function

			 Friend Overrides Function opEvaluateParallelLazy(Of P_IN)(ByVal helper As PipelineHelper(Of Double?), ByVal spliterator As java.util.Spliterator(Of P_IN)) As java.util.Spliterator(Of Double?)
				Dim size As Long = helper.exactOutputSizeIfKnown(spliterator)
				If size > 0 AndAlso spliterator.hasCharacteristics(java.util.Spliterator.SUBSIZED) Then
					Return New StreamSpliterators.SliceSpliterator.OfDouble(CType(helper.wrapSpliterator(spliterator), java.util.Spliterator.OfDouble), skip, calcSliceFence(skip, limit))
				ElseIf Not StreamOpFlag.ORDERED.isKnown(helper.streamAndOpFlags) Then
					Return unorderedSkipLimitSpliterator(CType(helper.wrapSpliterator(spliterator), java.util.Spliterator.OfDouble), skip, limit, size)
				Else
					Return (New SliceTask(Of )(Me, helper, spliterator, java.lang.Double() ::New, skip, limit)).invoke().spliterator()
				End If
			 End Function

			 Friend Overrides Function opEvaluateParallel(Of P_IN)(ByVal helper As PipelineHelper(Of Double?), ByVal spliterator As java.util.Spliterator(Of P_IN), ByVal generator As java.util.function.IntFunction(Of Double?())) As Node(Of Double?)
				Dim size As Long = helper.exactOutputSizeIfKnown(spliterator)
				If size > 0 AndAlso spliterator.hasCharacteristics(java.util.Spliterator.SUBSIZED) Then
					' Because the pipeline is SIZED the slice spliterator
					' can be created from the source, this requires matching
					' to shape of the source, and is potentially more efficient
					' than creating the slice spliterator from the pipeline
					' wrapping spliterator
					Dim s As java.util.Spliterator(Of P_IN) = sliceSpliterator(helper.sourceShape, spliterator, skip, limit)
					Return Nodes.collectDouble(helper, s, True)
				ElseIf Not StreamOpFlag.ORDERED.isKnown(helper.streamAndOpFlags) Then
					Dim s As java.util.Spliterator.OfDouble = unorderedSkipLimitSpliterator(CType(helper.wrapSpliterator(spliterator), java.util.Spliterator.OfDouble), skip, limit, size)
					' Collect using this pipeline, which is empty and therefore
					' can be used with the pipeline wrapping spliterator
					' Note that we cannot create a slice spliterator from
					' the source spliterator if the pipeline is not SIZED
					Return Nodes.collectDouble(Me, s, True)
				Else
					Return (New SliceTask(Of )(Me, helper, spliterator, generator, skip, limit)).invoke()
				End If
			 End Function

			Friend Overrides Function opWrapSink(ByVal flags As Integer, ByVal sink As Sink(Of Double?)) As Sink(Of Double?)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return New Sink.ChainedDouble<java.lang.Double>(sink)
	'			{
	'				long n = skip;
	'				long m = limit >= 0 ? limit : java.lang.[Long].MAX_VALUE;
	'
	'				@Override public  Sub  begin(long size)
	'				{
	'					downstream.begin(calcSize(size, skip, m));
	'				}
	'
	'				@Override public  Sub  accept(double t)
	'				{
	'					if (n == 0)
	'					{
	'						if (m > 0)
	'						{
	'							m -= 1;
	'							downstream.accept(t);
	'						}
	'					}
	'					else
	'					{
	'						n -= 1;
	'					}
	'				}
	'
	'				@Override public boolean cancellationRequested()
	'				{
	'					Return m == 0 || downstream.cancellationRequested();
	'				}
	'			};
			End Function
		End Class

		Private Shared Function flags(ByVal limit As Long) As Integer
			Return StreamOpFlag.NOT_SIZED Or (If(limit <> -1, StreamOpFlag.IS_SHORT_CIRCUIT, 0))
		End Function

		''' <summary>
		''' {@code ForkJoinTask} implementing slice computation.
		''' </summary>
		''' @param <P_IN> Input element type to the stream pipeline </param>
		''' @param <P_OUT> Output element type from the stream pipeline </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private NotInheritable Class SliceTask(Of P_IN, P_OUT)
			Inherits AbstractShortCircuitTask(Of P_IN, P_OUT, Node(Of P_OUT), SliceTask(Of P_IN, P_OUT))

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Private ReadOnly op As AbstractPipeline(Of P_OUT, P_OUT, ?)
			Private ReadOnly generator As java.util.function.IntFunction(Of P_OUT())
			Private ReadOnly targetOffset, targetSize As Long
			Private thisNodeSize As Long

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
			Private completed As Boolean

			Friend Sub New(Of T1)(ByVal op As AbstractPipeline(Of T1), ByVal helper As PipelineHelper(Of P_OUT), ByVal spliterator As java.util.Spliterator(Of P_IN), ByVal generator As java.util.function.IntFunction(Of P_OUT()), ByVal offset As Long, ByVal size As Long)
				MyBase.New(helper, spliterator)
				Me.op = op
				Me.generator = generator
				Me.targetOffset = offset
				Me.targetSize = size
			End Sub

			Friend Sub New(ByVal parent As SliceTask(Of P_IN, P_OUT), ByVal spliterator As java.util.Spliterator(Of P_IN))
				MyBase.New(parent, spliterator)
				Me.op = parent.op
				Me.generator = parent.generator
				Me.targetOffset = parent.targetOffset
				Me.targetSize = parent.targetSize
			End Sub

			Protected Friend Overrides Function makeChild(ByVal spliterator As java.util.Spliterator(Of P_IN)) As SliceTask(Of P_IN, P_OUT)
				Return New SliceTask(Of )(Me, spliterator)
			End Function

			Protected Friend  Overrides ReadOnly Property  emptyResult As Node(Of P_OUT)
				Get
					Return Nodes.emptyNode(op.outputShape)
				End Get
			End Property

			Protected Friend Overrides Function doLeaf() As Node(Of P_OUT)
				If root Then
					Dim sizeIfKnown As Long = If(StreamOpFlag.SIZED.isPreserved(op.sourceOrOpFlags), op.exactOutputSizeIfKnown(spliterator), -1)
					Dim nb As Node.Builder(Of P_OUT) = op.makeNodeBuilder(sizeIfKnown, generator)
					Dim opSink As Sink(Of P_OUT) = op.opWrapSink(helper.streamAndOpFlags, nb)
					helper.copyIntoWithCancel(helper.wrapSink(opSink), spliterator)
					' There is no need to truncate since the op performs the
					' skipping and limiting of elements
					Return nb.build()
				Else
					Dim node As Node(Of P_OUT) = helper.wrapAndCopyInto(helper.makeNodeBuilder(-1, generator), spliterator).build()
					thisNodeSize = node.count()
					completed = True
					spliterator = Nothing
					Return node
				End If
			End Function

			Public Overrides Sub onCompletion(Of T1)(ByVal caller As java.util.concurrent.CountedCompleter(Of T1))
				If Not leaf Then
					Dim result As Node(Of P_OUT)
					thisNodeSize = leftChild.thisNodeSize + rightChild.thisNodeSize
					If canceled Then
						thisNodeSize = 0
						result = emptyResult
					ElseIf thisNodeSize = 0 Then
						result = emptyResult
					ElseIf leftChild.thisNodeSize = 0 Then
						result = rightChild.localResult
					Else
						result = Nodes.conc(op.outputShape, leftChild.localResult, rightChild.localResult)
					End If
					localResult = If(root, doTruncate(result), result)
					completed = True
				End If
				If targetSize >= 0 AndAlso (Not root) AndAlso isLeftCompleted(targetOffset + targetSize) Then cancelLaterNodes()

				MyBase.onCompletion(caller)
			End Sub

			Protected Friend Overrides Sub cancel()
				MyBase.cancel()
				If completed Then localResult = emptyResult
			End Sub

			Private Function doTruncate(ByVal input As Node(Of P_OUT)) As Node(Of P_OUT)
				Dim [to] As Long = If(targetSize >= 0, System.Math.Min(input.count(), targetOffset + targetSize), thisNodeSize)
				Return input.truncate(targetOffset, [to], generator)
			End Function

			''' <summary>
			''' Determine if the number of completed elements in this node and nodes
			''' to the left of this node is greater than or equal to the target size.
			''' </summary>
			''' <param name="target"> the target size </param>
			''' <returns> true if the number of elements is greater than or equal to
			'''         the target size, otherwise false. </returns>
			Private Function isLeftCompleted(ByVal target As Long) As Boolean
				Dim size As Long = If(completed, thisNodeSize, completedSize(target))
				If size >= target Then Return True
				Dim parent As SliceTask(Of P_IN, P_OUT) = parent
				Dim node As SliceTask(Of P_IN, P_OUT) = Me
				Do While parent IsNot Nothing
					If node Is parent.rightChild Then
						Dim left As SliceTask(Of P_IN, P_OUT) = parent.leftChild
						If left IsNot Nothing Then
							size += left.completedSize(target)
							If size >= target Then Return True
						End If
					End If
					node = parent
					parent = parent.parent
				Loop
				Return size >= target
			End Function

			''' <summary>
			''' Compute the number of completed elements in this node.
			''' <p>
			''' Computation terminates if all nodes have been processed or the
			''' number of completed elements is greater than or equal to the target
			''' size.
			''' </summary>
			''' <param name="target"> the target size </param>
			''' <returns> return the number of completed elements </returns>
			Private Function completedSize(ByVal target As Long) As Long
				If completed Then
					Return thisNodeSize
				Else
					Dim left As SliceTask(Of P_IN, P_OUT) = leftChild
					Dim right As SliceTask(Of P_IN, P_OUT) = rightChild
					If left Is Nothing OrElse right Is Nothing Then
						' must be completed
						Return thisNodeSize
					Else
						Dim leftSize As Long = left.completedSize(target)
						Return If(leftSize >= target, leftSize, leftSize + right.completedSize(target))
					End If
				End If
			End Function
		End Class
	End Class

End Namespace