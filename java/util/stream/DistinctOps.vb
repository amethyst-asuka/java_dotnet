Imports System.Collections.Generic
Imports System.Collections.Concurrent

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
	''' Factory methods for transforming streams into duplicate-free streams, using
	''' <seealso cref="Object#equals(Object)"/> to determine equality.
	''' 
	''' @since 1.8
	''' </summary>
	Friend NotInheritable Class DistinctOps

		Private Sub New()
		End Sub

		''' <summary>
		''' Appends a "distinct" operation to the provided stream, and returns the
		''' new stream.
		''' </summary>
		''' @param <T> the type of both input and output elements </param>
		''' <param name="upstream"> a reference stream with element type T </param>
		''' <returns> the new stream </returns>
		Friend Shared Function makeRef(Of T, T1)(  upstream As AbstractPipeline(Of T1)) As ReferencePipeline(Of T, T)
			Return New StatefulOpAnonymousInnerClassHelper(Of E_IN, E_OUT)
		End Function

		Private Class StatefulOpAnonymousInnerClassHelper(Of E_IN, E_OUT)
			Inherits StatefulOp(Of E_IN, E_OUT)

			 Friend Overridable Function reduce(Of P_IN)(  helper As PipelineHelper(Of T),   spliterator As java.util.Spliterator(Of P_IN)) As Node(Of T)
				' If the stream is SORTED then it should also be ORDERED so the following will also
				' preserve the sort order
				Dim reduceOp As TerminalOp(Of T, java.util.LinkedHashSet(Of T)) = ReduceOps.makeRef(Of T, java.util.LinkedHashSet(Of T))(java.util.LinkedHashSet::New, java.util.LinkedHashSet::add, java.util.LinkedHashSet::addAll)
				Return Nodes.node(reduceOp.evaluateParallel(helper, spliterator))
			 End Function

			 Friend Overrides Function opEvaluateParallel(Of P_IN)(  helper As PipelineHelper(Of T),   spliterator As java.util.Spliterator(Of P_IN),   generator As java.util.function.IntFunction(Of T())) As Node(Of T)
				If StreamOpFlag.DISTINCT.isKnown(helper.streamAndOpFlags) Then
					' No-op
					Return helper.evaluate(spliterator, False, generator)
				ElseIf StreamOpFlag.ORDERED.isKnown(helper.streamAndOpFlags) Then
					Return reduce(helper, spliterator)
				Else
					' Holder of null state since ConcurrentHashMap does not support null values
					Dim seenNull As New java.util.concurrent.atomic.AtomicBoolean(False)
					Dim map As New ConcurrentDictionary(Of T, Boolean?)
					Dim forEachOp As TerminalOp(Of T, Void) = ForEachOps.makeRef(t -> { if(t Is Nothing) seenNull.set(True); else map.GetOrAdd(t,  java.lang.[Boolean].TRUE); }, False)
					forEachOp.evaluateParallel(helper, spliterator)

					' If null has been seen then copy the key set into a HashSet that supports null values
					' and add null
					Dim keys As ConcurrentDictionary(Of T, Boolean?).KeyCollection = map.Keys
					If seenNull.get() Then
						' TODO Implement a more efficient set-union view, rather than copying
						keys = New HashSet(Of )(keys)
						keys.add(Nothing)
					End If
					Return Nodes.node(keys)
				End If
			 End Function

			 Friend Overrides Function opEvaluateParallelLazy(Of P_IN)(  helper As PipelineHelper(Of T),   spliterator As java.util.Spliterator(Of P_IN)) As java.util.Spliterator(Of T)
				If StreamOpFlag.DISTINCT.isKnown(helper.streamAndOpFlags) Then
					' No-op
					Return helper.wrapSpliterator(spliterator)
				ElseIf StreamOpFlag.ORDERED.isKnown(helper.streamAndOpFlags) Then
					' Not lazy, barrier required to preserve order
					Return reduce(helper, spliterator).spliterator()
				Else
					' Lazy
					Return New StreamSpliterators.DistinctSpliterator(Of )(helper.wrapSpliterator(spliterator))
				End If
			 End Function

			Friend Overrides Function opWrapSink(  flags As Integer,   sink As Sink(Of T)) As Sink(Of T)
				java.util.Objects.requireNonNull(sink)

				If StreamOpFlag.DISTINCT.isKnown(flags) Then
					Return sink
				ElseIf StreamOpFlag.SORTED.isKnown(flags) Then
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'					Return New Sink.ChainedReference<T, T>(sink)
	'				{
	'					boolean seenNull;
	'					T lastSeen;
	'
	'					@Override public  Sub  begin(long size)
	'					{
	'						seenNull = False;
	'						lastSeen = Nothing;
	'						downstream.begin(-1);
	'					}
	'
	'					@Override public  Sub  end()
	'					{
	'						seenNull = False;
	'						lastSeen = Nothing;
	'						downstream.end();
	'					}
	'
	'					@Override public  Sub  accept(T t)
	'					{
	'						if (t == Nothing)
	'						{
	'							if (!seenNull)
	'							{
	'								seenNull = True;
	'								downstream.accept(lastSeen = Nothing);
	'							}
	'						}
	'						else if (lastSeen == Nothing || !t.equals(lastSeen))
	'						{
	'							downstream.accept(lastSeen = t);
	'						}
	'					}
	'				};
				Else
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'					Return New Sink.ChainedReference<T, T>(sink)
	'				{
	'					Set<T> seen;
	'
	'					@Override public  Sub  begin(long size)
	'					{
	'						seen = New HashSet<>();
	'						downstream.begin(-1);
	'					}
	'
	'					@Override public  Sub  end()
	'					{
	'						seen = Nothing;
	'						downstream.end();
	'					}
	'
	'					@Override public  Sub  accept(T t)
	'					{
	'						if (!seen.contains(t))
	'						{
	'							seen.add(t);
	'							downstream.accept(t);
	'						}
	'					}
	'				};
				End If
			End Function
		End Class
	End Class

End Namespace