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
Namespace java.util

	'
	' * Written by Doug Lea with assistance from members of JCP JSR-166
	' * Expert Group and released to the public domain, as explained at
	' * http://creativecommons.org/publicdomain/zero/1.0/
	' 


	''' <summary>
	''' ForkJoin tasks to perform Arrays.parallelPrefix operations.
	''' 
	''' @author Doug Lea
	''' @since 1.8
	''' </summary>
	Friend Class ArrayPrefixHelpers
		Private Sub New() ' non-instantiable
		End Sub

	'    
	'     * Parallel prefix (aka cumulate, scan) task classes
	'     * are based loosely on Guy Blelloch's original
	'     * algorithm (http://www.cs.cmu.edu/~scandal/alg/scan.html):
	'     *  Keep dividing by two to threshold segment size, and then:
	'     *   Pass 1: Create tree of partial sums for each segment
	'     *   Pass 2: For each segment, cumulate with offset of left sibling
	'     *
	'     * This version improves performance within FJ framework mainly by
	'     * allowing the second pass of ready left-hand sides to proceed
	'     * even if some right-hand side first passes are still executing.
	'     * It also combines first and second pass for leftmost segment,
	'     * and skips the first pass for rightmost segment (whose result is
	'     * not needed for second pass).  It similarly manages to avoid
	'     * requiring that users supply an identity basis for accumulations
	'     * by tracking those segments/subtasks for which the first
	'     * existing element is used as base.
	'     *
	'     * Managing this relies on ORing some bits in the pendingCount for
	'     * phases/states: CUMULATE, SUMMED, and FINISHED. CUMULATE is the
	'     * main phase bit. When false, segments compute only their sum.
	'     * When true, they cumulate array elements. CUMULATE is set at
	'     * root at beginning of second pass and then propagated down. But
	'     * it may also be set earlier for subtrees with lo==0 (the left
	'     * spine of tree). SUMMED is a one bit join count. For leafs, it
	'     * is set when summed. For internal nodes, it becomes true when
	'     * one child is summed.  When the second child finishes summing,
	'     * we then moves up tree to trigger the cumulate phase. FINISHED
	'     * is also a one bit join count. For leafs, it is set when
	'     * cumulated. For internal nodes, it becomes true when one child
	'     * is cumulated.  When the second child finishes cumulating, it
	'     * then moves up tree, completing at the root.
	'     *
	'     * To better exploit locality and reduce overhead, the compute
	'     * method loops starting with the current task, moving if possible
	'     * to one of its subtasks rather than forking.
	'     *
	'     * As usual for this sort of utility, there are 4 versions, that
	'     * are simple copy/paste/adapt variants of each other.  (The
	'     * double and int versions differ from long version soley by
	'     * replacing "long" (with case-matching)).
	'     

		' see above
		Friend Const CUMULATE As Integer = 1
		Friend Const SUMMED As Integer = 2
		Friend Const FINISHED As Integer = 4

		''' <summary>
		''' The smallest subtask array partition size to use as threshold </summary>
		Friend Const MIN_PARTITION As Integer = 16

		Friend NotInheritable Class CumulateTask(Of T)
			Inherits java.util.concurrent.CountedCompleter(Of Void)

			Friend ReadOnly array As T()
			Friend ReadOnly [function] As java.util.function.BinaryOperator(Of T)
			Friend left As CumulateTask(Of T), right As CumulateTask(Of T)
			Friend [in], out As T
			Friend ReadOnly lo, hi, origin, fence, threshold As Integer

			''' <summary>
			''' Root task constructor </summary>
			Public Sub New(ByVal parent As CumulateTask(Of T), ByVal [function] As java.util.function.BinaryOperator(Of T), ByVal array As T(), ByVal lo As Integer, ByVal hi As Integer)
				MyBase.New(parent)
				Me.function = [function]
				Me.array = array
					Me.origin = lo
					Me.lo = Me.origin
					Me.fence = hi
					Me.hi = Me.fence
				Dim p As Integer
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Me.threshold = If((p = (hi - lo) / (java.util.concurrent.ForkJoinPool.commonPoolParallelism << 3)) <= MIN_PARTITION, MIN_PARTITION, p)
			End Sub

			''' <summary>
			''' Subtask constructor </summary>
			Friend Sub New(ByVal parent As CumulateTask(Of T), ByVal [function] As java.util.function.BinaryOperator(Of T), ByVal array As T(), ByVal origin As Integer, ByVal fence As Integer, ByVal threshold As Integer, ByVal lo As Integer, ByVal hi As Integer)
				MyBase.New(parent)
				Me.function = [function]
				Me.array = array
				Me.origin = origin
				Me.fence = fence
				Me.threshold = threshold
				Me.lo = lo
				Me.hi = hi
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Sub compute()
				Dim fn As java.util.function.BinaryOperator(Of T)
				Dim a As T()
				fn = Me.function
				a = Me.array
				If fn Is Nothing OrElse a Is Nothing Then Throw New NullPointerException ' hoist checks
				Dim th As Integer = threshold, org As Integer = origin, fnc As Integer = fence, l As Integer, h As Integer
				Dim t As CumulateTask(Of T) = Me
				outer:
				l = t.lo
				h = t.hi
				Do While l >= 0 AndAlso h <= a.Length
					If h - l > th Then
						Dim lt As CumulateTask(Of T) = t.left, rt As CumulateTask(Of T) = t.right, f As CumulateTask(Of T)
						If lt Is Nothing Then ' first pass
							Dim mid As Integer = CInt(CUInt((l + h)) >> 1)
								t.right = New CumulateTask(Of T)(t, fn, a, org, fnc, th, mid, h)
									rt = t.right
									f = rt
								t.left = New CumulateTask(Of T)(t, fn, a, org, fnc, th, l, mid)
									lt = t.left
									t = lt
						Else ' possibly refork
							Dim pin As T = t.in
							lt.in = pin
								t = Nothing
								f = t
							If rt IsNot Nothing Then
								Dim lout As T = lt.out
								rt.in = (If(l = org, lout, fn.apply(pin, lout)))
								Dim c As Integer
								Do
									c = rt.pendingCount
									If (c And CUMULATE) <> 0 Then Exit Do
									If rt.compareAndSetPendingCount(c, c Or CUMULATE) Then
										t = rt
										Exit Do
									End If
								Loop
							End If
							Dim c As Integer
							Do
								c = lt.pendingCount
								If (c And CUMULATE) <> 0 Then Exit Do
								If lt.compareAndSetPendingCount(c, c Or CUMULATE) Then
									If t IsNot Nothing Then f = t
									t = lt
									Exit Do
								End If
							Loop
							If t Is Nothing Then Exit Do
						End If
						If f IsNot Nothing Then f.fork()
					Else
						Dim state As Integer ' Transition to sum, cumulate, or both
						Dim b As Integer
						Do
							b = t.pendingCount
							If (b And FINISHED) <> 0 Then GoTo outer ' already done
							state = (If((b And CUMULATE) <> 0, FINISHED, If(l > org, SUMMED, (SUMMED Or FINISHED))))
							If t.compareAndSetPendingCount(b, b Or state) Then Exit Do
						Loop

						Dim sum As T
						If state <> SUMMED Then
							Dim first As Integer
							If l = org Then ' leftmost; no in
								sum = a(org)
								first = org + 1
							Else
								sum = t.in
								first = l
							End If
							For i As Integer = first To h - 1 ' cumulate
									sum = fn.apply(sum, a(i))
									a(i) = sum
							Next i
						ElseIf h < fnc Then ' skip rightmost
							sum = a(l)
							For i As Integer = l + 1 To h - 1 ' sum only
								sum = fn.apply(sum, a(i))
							Next i
						Else
							sum = t.in
						End If
						t.out = sum
						Dim par As CumulateTask(Of T)
						Do ' propagate
							par = CType(t.completer, CumulateTask(Of T))
							If par Is Nothing Then
								If (state And FINISHED) <> 0 Then ' enable join t.quietlyComplete()
								GoTo outer
							End If
							Dim b As Integer = par.pendingCount
							If (b And state And FINISHED) <> 0 Then
								t = par ' both done
							ElseIf (b And state And SUMMED) <> 0 Then ' both summed
								Dim nextState As Integer
								Dim lt As CumulateTask(Of T), rt As CumulateTask(Of T)
								lt = par.left
								rt = par.right
								If lt IsNot Nothing AndAlso rt IsNot Nothing Then
									Dim lout As T = lt.out
									par.out = (If(rt.hi = fnc, lout, fn.apply(lout, rt.out)))
								End If
								Dim refork As Integer = (If((b And CUMULATE) = 0 AndAlso par.lo = org, CUMULATE, 0))
								nextState = b Or state Or refork
								If nextState = b OrElse par.compareAndSetPendingCount(b, nextState) Then
									state = SUMMED ' drop finished
									t = par
									If refork <> 0 Then par.fork()
								End If
							ElseIf par.compareAndSetPendingCount(b, b Or state) Then
								GoTo outer ' sib not ready
							End If
						Loop
					End If
					l = t.lo
					h = t.hi
				Loop
			End Sub
		End Class

		Friend NotInheritable Class LongCumulateTask
			Inherits java.util.concurrent.CountedCompleter(Of Void)

			Friend ReadOnly array As Long()
			Friend ReadOnly [function] As java.util.function.LongBinaryOperator
			Friend left, right As LongCumulateTask
			Friend [in], out As Long
			Friend ReadOnly lo, hi, origin, fence, threshold As Integer

			''' <summary>
			''' Root task constructor </summary>
			Public Sub New(ByVal parent As LongCumulateTask, ByVal [function] As java.util.function.LongBinaryOperator, ByVal array As Long(), ByVal lo As Integer, ByVal hi As Integer)
				MyBase.New(parent)
				Me.function = [function]
				Me.array = array
					Me.origin = lo
					Me.lo = Me.origin
					Me.fence = hi
					Me.hi = Me.fence
				Dim p As Integer
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Me.threshold = If((p = (hi - lo) / (java.util.concurrent.ForkJoinPool.commonPoolParallelism << 3)) <= MIN_PARTITION, MIN_PARTITION, p)
			End Sub

			''' <summary>
			''' Subtask constructor </summary>
			Friend Sub New(ByVal parent As LongCumulateTask, ByVal [function] As java.util.function.LongBinaryOperator, ByVal array As Long(), ByVal origin As Integer, ByVal fence As Integer, ByVal threshold As Integer, ByVal lo As Integer, ByVal hi As Integer)
				MyBase.New(parent)
				Me.function = [function]
				Me.array = array
				Me.origin = origin
				Me.fence = fence
				Me.threshold = threshold
				Me.lo = lo
				Me.hi = hi
			End Sub

			Public Sub compute()
				Dim fn As java.util.function.LongBinaryOperator
				Dim a As Long()
				fn = Me.function
				a = Me.array
				If fn Is Nothing OrElse a Is Nothing Then Throw New NullPointerException ' hoist checks
				Dim th As Integer = threshold, org As Integer = origin, fnc As Integer = fence, l As Integer, h As Integer
				Dim t As LongCumulateTask = Me
				outer:
				l = t.lo
				h = t.hi
				Do While l >= 0 AndAlso h <= a.Length
					If h - l > th Then
						Dim lt As LongCumulateTask = t.left, rt As LongCumulateTask = t.right, f As LongCumulateTask
						If lt Is Nothing Then ' first pass
							Dim mid As Integer = CInt(CUInt((l + h)) >> 1)
								t.right = New LongCumulateTask(t, fn, a, org, fnc, th, mid, h)
									rt = t.right
									f = rt
								t.left = New LongCumulateTask(t, fn, a, org, fnc, th, l, mid)
									lt = t.left
									t = lt
						Else ' possibly refork
							Dim pin As Long = t.in
							lt.in = pin
								t = Nothing
								f = t
							If rt IsNot Nothing Then
								Dim lout As Long = lt.out
								rt.in = (If(l = org, lout, fn.applyAsLong(pin, lout)))
								Dim c As Integer
								Do
									c = rt.pendingCount
									If (c And CUMULATE) <> 0 Then Exit Do
									If rt.compareAndSetPendingCount(c, c Or CUMULATE) Then
										t = rt
										Exit Do
									End If
								Loop
							End If
							Dim c As Integer
							Do
								c = lt.pendingCount
								If (c And CUMULATE) <> 0 Then Exit Do
								If lt.compareAndSetPendingCount(c, c Or CUMULATE) Then
									If t IsNot Nothing Then f = t
									t = lt
									Exit Do
								End If
							Loop
							If t Is Nothing Then Exit Do
						End If
						If f IsNot Nothing Then f.fork()
					Else
						Dim state As Integer ' Transition to sum, cumulate, or both
						Dim b As Integer
						Do
							b = t.pendingCount
							If (b And FINISHED) <> 0 Then GoTo outer ' already done
							state = (If((b And CUMULATE) <> 0, FINISHED, If(l > org, SUMMED, (SUMMED Or FINISHED))))
							If t.compareAndSetPendingCount(b, b Or state) Then Exit Do
						Loop

						Dim sum As Long
						If state <> SUMMED Then
							Dim first As Integer
							If l = org Then ' leftmost; no in
								sum = a(org)
								first = org + 1
							Else
								sum = t.in
								first = l
							End If
							For i As Integer = first To h - 1 ' cumulate
									sum = fn.applyAsLong(sum, a(i))
									a(i) = sum
							Next i
						ElseIf h < fnc Then ' skip rightmost
							sum = a(l)
							For i As Integer = l + 1 To h - 1 ' sum only
								sum = fn.applyAsLong(sum, a(i))
							Next i
						Else
							sum = t.in
						End If
						t.out = sum
						Dim par As LongCumulateTask
						Do ' propagate
							par = CType(t.completer, LongCumulateTask)
							If par Is Nothing Then
								If (state And FINISHED) <> 0 Then ' enable join t.quietlyComplete()
								GoTo outer
							End If
							Dim b As Integer = par.pendingCount
							If (b And state And FINISHED) <> 0 Then
								t = par ' both done
							ElseIf (b And state And SUMMED) <> 0 Then ' both summed
								Dim nextState As Integer
								Dim lt, rt As LongCumulateTask
								lt = par.left
								rt = par.right
								If lt IsNot Nothing AndAlso rt IsNot Nothing Then
									Dim lout As Long = lt.out
									par.out = (If(rt.hi = fnc, lout, fn.applyAsLong(lout, rt.out)))
								End If
								Dim refork As Integer = (If((b And CUMULATE) = 0 AndAlso par.lo = org, CUMULATE, 0))
								nextState = b Or state Or refork
								If nextState = b OrElse par.compareAndSetPendingCount(b, nextState) Then
									state = SUMMED ' drop finished
									t = par
									If refork <> 0 Then par.fork()
								End If
							ElseIf par.compareAndSetPendingCount(b, b Or state) Then
								GoTo outer ' sib not ready
							End If
						Loop
					End If
					l = t.lo
					h = t.hi
				Loop
			End Sub
		End Class

		Friend NotInheritable Class DoubleCumulateTask
			Inherits java.util.concurrent.CountedCompleter(Of Void)

			Friend ReadOnly array As Double()
			Friend ReadOnly [function] As java.util.function.DoubleBinaryOperator
			Friend left, right As DoubleCumulateTask
			Friend [in], out As Double
			Friend ReadOnly lo, hi, origin, fence, threshold As Integer

			''' <summary>
			''' Root task constructor </summary>
			Public Sub New(ByVal parent As DoubleCumulateTask, ByVal [function] As java.util.function.DoubleBinaryOperator, ByVal array As Double(), ByVal lo As Integer, ByVal hi As Integer)
				MyBase.New(parent)
				Me.function = [function]
				Me.array = array
					Me.origin = lo
					Me.lo = Me.origin
					Me.fence = hi
					Me.hi = Me.fence
				Dim p As Integer
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Me.threshold = If((p = (hi - lo) / (java.util.concurrent.ForkJoinPool.commonPoolParallelism << 3)) <= MIN_PARTITION, MIN_PARTITION, p)
			End Sub

			''' <summary>
			''' Subtask constructor </summary>
			Friend Sub New(ByVal parent As DoubleCumulateTask, ByVal [function] As java.util.function.DoubleBinaryOperator, ByVal array As Double(), ByVal origin As Integer, ByVal fence As Integer, ByVal threshold As Integer, ByVal lo As Integer, ByVal hi As Integer)
				MyBase.New(parent)
				Me.function = [function]
				Me.array = array
				Me.origin = origin
				Me.fence = fence
				Me.threshold = threshold
				Me.lo = lo
				Me.hi = hi
			End Sub

			Public Sub compute()
				Dim fn As java.util.function.DoubleBinaryOperator
				Dim a As Double()
				fn = Me.function
				a = Me.array
				If fn Is Nothing OrElse a Is Nothing Then Throw New NullPointerException ' hoist checks
				Dim th As Integer = threshold, org As Integer = origin, fnc As Integer = fence, l As Integer, h As Integer
				Dim t As DoubleCumulateTask = Me
				outer:
				l = t.lo
				h = t.hi
				Do While l >= 0 AndAlso h <= a.Length
					If h - l > th Then
						Dim lt As DoubleCumulateTask = t.left, rt As DoubleCumulateTask = t.right, f As DoubleCumulateTask
						If lt Is Nothing Then ' first pass
							Dim mid As Integer = CInt(CUInt((l + h)) >> 1)
								t.right = New DoubleCumulateTask(t, fn, a, org, fnc, th, mid, h)
									rt = t.right
									f = rt
								t.left = New DoubleCumulateTask(t, fn, a, org, fnc, th, l, mid)
									lt = t.left
									t = lt
						Else ' possibly refork
							Dim pin As Double = t.in
							lt.in = pin
								t = Nothing
								f = t
							If rt IsNot Nothing Then
								Dim lout As Double = lt.out
								rt.in = (If(l = org, lout, fn.applyAsDouble(pin, lout)))
								Dim c As Integer
								Do
									c = rt.pendingCount
									If (c And CUMULATE) <> 0 Then Exit Do
									If rt.compareAndSetPendingCount(c, c Or CUMULATE) Then
										t = rt
										Exit Do
									End If
								Loop
							End If
							Dim c As Integer
							Do
								c = lt.pendingCount
								If (c And CUMULATE) <> 0 Then Exit Do
								If lt.compareAndSetPendingCount(c, c Or CUMULATE) Then
									If t IsNot Nothing Then f = t
									t = lt
									Exit Do
								End If
							Loop
							If t Is Nothing Then Exit Do
						End If
						If f IsNot Nothing Then f.fork()
					Else
						Dim state As Integer ' Transition to sum, cumulate, or both
						Dim b As Integer
						Do
							b = t.pendingCount
							If (b And FINISHED) <> 0 Then GoTo outer ' already done
							state = (If((b And CUMULATE) <> 0, FINISHED, If(l > org, SUMMED, (SUMMED Or FINISHED))))
							If t.compareAndSetPendingCount(b, b Or state) Then Exit Do
						Loop

						Dim sum As Double
						If state <> SUMMED Then
							Dim first As Integer
							If l = org Then ' leftmost; no in
								sum = a(org)
								first = org + 1
							Else
								sum = t.in
								first = l
							End If
							For i As Integer = first To h - 1 ' cumulate
									sum = fn.applyAsDouble(sum, a(i))
									a(i) = sum
							Next i
						ElseIf h < fnc Then ' skip rightmost
							sum = a(l)
							For i As Integer = l + 1 To h - 1 ' sum only
								sum = fn.applyAsDouble(sum, a(i))
							Next i
						Else
							sum = t.in
						End If
						t.out = sum
						Dim par As DoubleCumulateTask
						Do ' propagate
							par = CType(t.completer, DoubleCumulateTask)
							If par Is Nothing Then
								If (state And FINISHED) <> 0 Then ' enable join t.quietlyComplete()
								GoTo outer
							End If
							Dim b As Integer = par.pendingCount
							If (b And state And FINISHED) <> 0 Then
								t = par ' both done
							ElseIf (b And state And SUMMED) <> 0 Then ' both summed
								Dim nextState As Integer
								Dim lt, rt As DoubleCumulateTask
								lt = par.left
								rt = par.right
								If lt IsNot Nothing AndAlso rt IsNot Nothing Then
									Dim lout As Double = lt.out
									par.out = (If(rt.hi = fnc, lout, fn.applyAsDouble(lout, rt.out)))
								End If
								Dim refork As Integer = (If((b And CUMULATE) = 0 AndAlso par.lo = org, CUMULATE, 0))
								nextState = b Or state Or refork
								If nextState = b OrElse par.compareAndSetPendingCount(b, nextState) Then
									state = SUMMED ' drop finished
									t = par
									If refork <> 0 Then par.fork()
								End If
							ElseIf par.compareAndSetPendingCount(b, b Or state) Then
								GoTo outer ' sib not ready
							End If
						Loop
					End If
					l = t.lo
					h = t.hi
				Loop
			End Sub
		End Class

		Friend NotInheritable Class IntCumulateTask
			Inherits java.util.concurrent.CountedCompleter(Of Void)

			Friend ReadOnly array As Integer()
			Friend ReadOnly [function] As java.util.function.IntBinaryOperator
			Friend left, right As IntCumulateTask
			Friend [in], out As Integer
			Friend ReadOnly lo, hi, origin, fence, threshold As Integer

			''' <summary>
			''' Root task constructor </summary>
			Public Sub New(ByVal parent As IntCumulateTask, ByVal [function] As java.util.function.IntBinaryOperator, ByVal array As Integer(), ByVal lo As Integer, ByVal hi As Integer)
				MyBase.New(parent)
				Me.function = [function]
				Me.array = array
					Me.origin = lo
					Me.lo = Me.origin
					Me.fence = hi
					Me.hi = Me.fence
				Dim p As Integer
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Me.threshold = If((p = (hi - lo) / (java.util.concurrent.ForkJoinPool.commonPoolParallelism << 3)) <= MIN_PARTITION, MIN_PARTITION, p)
			End Sub

			''' <summary>
			''' Subtask constructor </summary>
			Friend Sub New(ByVal parent As IntCumulateTask, ByVal [function] As java.util.function.IntBinaryOperator, ByVal array As Integer(), ByVal origin As Integer, ByVal fence As Integer, ByVal threshold As Integer, ByVal lo As Integer, ByVal hi As Integer)
				MyBase.New(parent)
				Me.function = [function]
				Me.array = array
				Me.origin = origin
				Me.fence = fence
				Me.threshold = threshold
				Me.lo = lo
				Me.hi = hi
			End Sub

			Public Sub compute()
				Dim fn As java.util.function.IntBinaryOperator
				Dim a As Integer()
				fn = Me.function
				a = Me.array
				If fn Is Nothing OrElse a Is Nothing Then Throw New NullPointerException ' hoist checks
				Dim th As Integer = threshold, org As Integer = origin, fnc As Integer = fence, l As Integer, h As Integer
				Dim t As IntCumulateTask = Me
				outer:
				l = t.lo
				h = t.hi
				Do While l >= 0 AndAlso h <= a.Length
					If h - l > th Then
						Dim lt As IntCumulateTask = t.left, rt As IntCumulateTask = t.right, f As IntCumulateTask
						If lt Is Nothing Then ' first pass
							Dim mid As Integer = CInt(CUInt((l + h)) >> 1)
								t.right = New IntCumulateTask(t, fn, a, org, fnc, th, mid, h)
									rt = t.right
									f = rt
								t.left = New IntCumulateTask(t, fn, a, org, fnc, th, l, mid)
									lt = t.left
									t = lt
						Else ' possibly refork
							Dim pin As Integer = t.in
							lt.in = pin
								t = Nothing
								f = t
							If rt IsNot Nothing Then
								Dim lout As Integer = lt.out
								rt.in = (If(l = org, lout, fn.applyAsInt(pin, lout)))
								Dim c As Integer
								Do
									c = rt.pendingCount
									If (c And CUMULATE) <> 0 Then Exit Do
									If rt.compareAndSetPendingCount(c, c Or CUMULATE) Then
										t = rt
										Exit Do
									End If
								Loop
							End If
							Dim c As Integer
							Do
								c = lt.pendingCount
								If (c And CUMULATE) <> 0 Then Exit Do
								If lt.compareAndSetPendingCount(c, c Or CUMULATE) Then
									If t IsNot Nothing Then f = t
									t = lt
									Exit Do
								End If
							Loop
							If t Is Nothing Then Exit Do
						End If
						If f IsNot Nothing Then f.fork()
					Else
						Dim state As Integer ' Transition to sum, cumulate, or both
						Dim b As Integer
						Do
							b = t.pendingCount
							If (b And FINISHED) <> 0 Then GoTo outer ' already done
							state = (If((b And CUMULATE) <> 0, FINISHED, If(l > org, SUMMED, (SUMMED Or FINISHED))))
							If t.compareAndSetPendingCount(b, b Or state) Then Exit Do
						Loop

						Dim sum As Integer
						If state <> SUMMED Then
							Dim first As Integer
							If l = org Then ' leftmost; no in
								sum = a(org)
								first = org + 1
							Else
								sum = t.in
								first = l
							End If
							For i As Integer = first To h - 1 ' cumulate
									sum = fn.applyAsInt(sum, a(i))
									a(i) = sum
							Next i
						ElseIf h < fnc Then ' skip rightmost
							sum = a(l)
							For i As Integer = l + 1 To h - 1 ' sum only
								sum = fn.applyAsInt(sum, a(i))
							Next i
						Else
							sum = t.in
						End If
						t.out = sum
						Dim par As IntCumulateTask
						Do ' propagate
							par = CType(t.completer, IntCumulateTask)
							If par Is Nothing Then
								If (state And FINISHED) <> 0 Then ' enable join t.quietlyComplete()
								GoTo outer
							End If
							Dim b As Integer = par.pendingCount
							If (b And state And FINISHED) <> 0 Then
								t = par ' both done
							ElseIf (b And state And SUMMED) <> 0 Then ' both summed
								Dim nextState As Integer
								Dim lt, rt As IntCumulateTask
								lt = par.left
								rt = par.right
								If lt IsNot Nothing AndAlso rt IsNot Nothing Then
									Dim lout As Integer = lt.out
									par.out = (If(rt.hi = fnc, lout, fn.applyAsInt(lout, rt.out)))
								End If
								Dim refork As Integer = (If((b And CUMULATE) = 0 AndAlso par.lo = org, CUMULATE, 0))
								nextState = b Or state Or refork
								If nextState = b OrElse par.compareAndSetPendingCount(b, nextState) Then
									state = SUMMED ' drop finished
									t = par
									If refork <> 0 Then par.fork()
								End If
							ElseIf par.compareAndSetPendingCount(b, b Or state) Then
								GoTo outer ' sib not ready
							End If
						Loop
					End If
					l = t.lo
					h = t.hi
				Loop
			End Sub
		End Class
	End Class

End Namespace