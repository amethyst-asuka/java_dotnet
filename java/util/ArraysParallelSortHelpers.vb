Imports System

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


	''' <summary>
	''' Helper utilities for the parallel sort methods in Arrays.parallelSort.
	''' 
	''' For each primitive type, plus Object, we define a static class to
	''' contain the Sorter and Merger implementations for that type:
	''' 
	''' Sorter classes based mainly on CilkSort
	''' <A href="http://supertech.lcs.mit.edu/cilk/"> Cilk</A>:
	''' Basic algorithm:
	''' if array size is small, just use a sequential quicksort (via Arrays.sort)
	'''         Otherwise:
	'''         1. Break array in half.
	'''         2. For each half,
	'''             a. break the half in half (i.e., quarters),
	'''             b. sort the quarters
	'''             c. merge them together
	'''         3. merge together the two halves.
	''' 
	''' One reason for splitting in quarters is that this guarantees that
	''' the final sort is in the main array, not the workspace array.
	''' (workspace and main swap roles on each subsort step.)  Leaf-level
	''' sorts use the associated sequential sort.
	''' 
	''' Merger classes perform merging for Sorter.  They are structured
	''' such that if the underlying sort is stable (as is true for
	''' TimSort), then so is the full sort.  If big enough, they split the
	''' largest of the two partitions in half, find the greatest point in
	''' smaller partition less than the beginning of the second half of
	''' larger via binary search; and then merge in parallel the two
	''' partitions.  In part to ensure tasks are triggered in
	''' stability-preserving order, the current CountedCompleter design
	''' requires some little tasks to serve as place holders for triggering
	''' completion tasks.  These classes (EmptyCompleter and Relay) don't
	''' need to keep track of the arrays, and are never themselves forked,
	''' so don't hold any task state.
	''' 
	''' The primitive class versions (FJByte... FJDouble) are
	''' identical to each other except for type declarations.
	''' 
	''' The base sequential sorts rely on non-public versions of TimSort,
	''' ComparableTimSort, and DualPivotQuicksort sort methods that accept
	''' temp workspace array slices that we will have already allocated, so
	''' avoids redundant allocation. (Except for DualPivotQuicksort byte[]
	''' sort, that does not ever use a workspace array.)
	''' </summary>
	'package
	 Friend Class ArraysParallelSortHelpers

	'    
	'     * Style note: The task classes have a lot of parameters, that are
	'     * stored as task fields and copied to local variables and used in
	'     * compute() methods, We pack these into as few lines as possible,
	'     * and hoist consistency checks among them before main loops, to
	'     * reduce distraction.
	'     

		''' <summary>
		''' A placeholder task for Sorters, used for the lowest
		''' quartile task, that does not need to maintain array state.
		''' </summary>
		Friend NotInheritable Class EmptyCompleter
			Inherits java.util.concurrent.CountedCompleter(Of Void)

			Friend Const serialVersionUID As Long = 2446542900576103244L
			Friend Sub New(Of T1)(ByVal p As java.util.concurrent.CountedCompleter(Of T1))
				MyBase.New(p)
			End Sub
			Public Sub compute()
			End Sub
		End Class

		''' <summary>
		''' A trigger for secondary merge of two merges
		''' </summary>
		Friend NotInheritable Class Relay
			Inherits java.util.concurrent.CountedCompleter(Of Void)

			Friend Const serialVersionUID As Long = 2446542900576103244L
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Friend ReadOnly task As java.util.concurrent.CountedCompleter(Of ?)
			Friend Sub New(Of T1)(ByVal task As java.util.concurrent.CountedCompleter(Of T1))
				MyBase.New(Nothing, 1)
				Me.task = task
			End Sub
			Public Sub compute()
			End Sub
			Public Sub onCompletion(Of T1)(ByVal t As java.util.concurrent.CountedCompleter(Of T1))
				task.compute()
			End Sub
		End Class

		''' <summary>
		''' Object + Comparator support class </summary>
		Friend NotInheritable Class FJObject
			Friend NotInheritable Class Sorter(Of T)
				Inherits java.util.concurrent.CountedCompleter(Of Void)

				Friend Const serialVersionUID As Long = 2446542900576103244L
				Friend ReadOnly a, w As T()
				Friend ReadOnly base, size, wbase, gran As Integer
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Friend comparator As Comparator(Of ?)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
				Friend Sub New(Of T1, T2)(ByVal par As java.util.concurrent.CountedCompleter(Of T1), ByVal a As T(), ByVal w As T(), ByVal base As Integer, ByVal size As Integer, ByVal wbase As Integer, ByVal gran As Integer, ByVal comparator As Comparator(Of T2))
					MyBase.New(par)
					Me.a = a
					Me.w = w
					Me.base = base
					Me.size = size
					Me.wbase = wbase
					Me.gran = gran
					Me.comparator = comparator
				End Sub
				Public Sub compute()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim s As java.util.concurrent.CountedCompleter(Of ?) = Me
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim c As Comparator(Of ?) = Me.comparator
					Dim a As T() = Me.a, w As T() = Me.w ' localize all params
					Dim b As Integer = Me.base, n As Integer = Me.size, wb As Integer = Me.wbase, g As Integer = Me.gran
					Do While n > g
						Dim h As Integer = CInt(CUInt(n) >> 1), q As Integer = CInt(CUInt(h) >> 1), u As Integer = h + q ' quartiles
						Dim fc As New Relay(New Merger(Of T)(s, w, a, wb, h, wb+h, n-h, b, g, c))
						Dim rc As New Relay(New Merger(Of T)(fc, a, w, b+h, q, b+u, n-u, wb+h, g, c))
						CType(New Sorter(Of T)(rc, a, w, b+u, n-u, wb+u, g, c), Sorter(Of T)).fork()
						CType(New Sorter(Of T)(rc, a, w, b+h, q, wb+h, g, c), Sorter(Of T)).fork()
						Dim bc As New Relay(New Merger(Of T)(fc, a, w, b, q, b+q, h-q, wb, g, c))
						CType(New Sorter(Of T)(bc, a, w, b+q, h-q, wb+q, g, c), Sorter(Of T)).fork()
						s = New EmptyCompleter(bc)
						n = q
					Loop
					TimSort.sort(a, b, b + n, c, w, wb, n)
					s.tryComplete()
				End Sub
			End Class

			Friend NotInheritable Class Merger(Of T)
				Inherits java.util.concurrent.CountedCompleter(Of Void)

				Friend Const serialVersionUID As Long = 2446542900576103244L
				Friend ReadOnly a, w As T() ' main and workspace arrays
				Friend ReadOnly lbase, lsize, rbase, rsize, wbase, gran As Integer
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Friend comparator As Comparator(Of ?)
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
				Friend Sub New(Of T1, T2)(ByVal par As java.util.concurrent.CountedCompleter(Of T1), ByVal a As T(), ByVal w As T(), ByVal lbase As Integer, ByVal lsize As Integer, ByVal rbase As Integer, ByVal rsize As Integer, ByVal wbase As Integer, ByVal gran As Integer, ByVal comparator As Comparator(Of T2))
					MyBase.New(par)
					Me.a = a
					Me.w = w
					Me.lbase = lbase
					Me.lsize = lsize
					Me.rbase = rbase
					Me.rsize = rsize
					Me.wbase = wbase
					Me.gran = gran
					Me.comparator = comparator
				End Sub

				Public Sub compute()
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim c As Comparator(Of ?) = Me.comparator
					Dim a As T() = Me.a, w As T() = Me.w ' localize all params
					Dim lb As Integer = Me.lbase, ln As Integer = Me.lsize, rb As Integer = Me.rbase, rn As Integer = Me.rsize, k As Integer = Me.wbase, g As Integer = Me.gran
					If a Is Nothing OrElse w Is Nothing OrElse lb < 0 OrElse rb < 0 OrElse k < 0 OrElse c Is Nothing Then Throw New IllegalStateException ' hoist checks
					Dim lh As Integer
					rh
					Do ' split larger, find point in smaller
						If ln >= rn Then
							If ln <= g Then Exit Do
							rh = rn
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
							Dim Split As T = a((lh = CInt(CUInt(ln) >> 1)) + lb)
							Dim lo As Integer = 0
							Do While lo < rh
								Dim rm As Integer = CInt(CUInt((lo + rh)) >> 1)
								If c.Compare(Split, a(rm + rb)) <= 0 Then
									rh = rm
								Else
									lo = rm + 1
								End If
							Loop
						Else
							If rn <= g Then Exit Do
							lh = ln
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
							Dim Split As T = a((rh = CInt(CUInt(rn) >> 1)) + rb)
							Dim lo As Integer = 0
							Do While lo < lh
								Dim lm As Integer = CInt(CUInt((lo + lh)) >> 1)
								If c.Compare(Split, a(lm + lb)) <= 0 Then
									lh = lm
								Else
									lo = lm + 1
								End If
							Loop
						End If
						Dim m As New Merger(Of T)(Me, a, w, lb + lh, ln - lh, rb + rh, rn - rh, k + lh + rh, g, c)
						rn = rh
						ln = lh
						addToPendingCount(1)
						m.fork()
					Loop

					Dim lf As Integer = lb + ln, rf As Integer = rb + rn ' index bounds
					Do While lb < lf AndAlso rb < rf
						Dim t, al, ar As T
						al = a(lb)
						ar = a(rb)
						If c.Compare(al , ar) <= 0 Then
							lb += 1
							t = al
						Else
							rb += 1
							t = ar
						End If
						w(k) = t
						k += 1
					Loop
					If rb < rf Then
						Array.Copy(a, rb, w, k, rf - rb)
					ElseIf lb < lf Then
						Array.Copy(a, lb, w, k, lf - lb)
					End If

					tryComplete()
				End Sub

			End Class
		End Class ' FJObject

		''' <summary>
		''' byte support class </summary>
		Friend NotInheritable Class FJByte
			Friend NotInheritable Class Sorter
				Inherits java.util.concurrent.CountedCompleter(Of Void)

				Friend Const serialVersionUID As Long = 2446542900576103244L
				Friend ReadOnly a, w As SByte()
				Friend ReadOnly base, size, wbase, gran As Integer
				Friend Sub New(Of T1)(ByVal par As java.util.concurrent.CountedCompleter(Of T1), ByVal a As SByte(), ByVal w As SByte(), ByVal base As Integer, ByVal size As Integer, ByVal wbase As Integer, ByVal gran As Integer)
					MyBase.New(par)
					Me.a = a
					Me.w = w
					Me.base = base
					Me.size = size
					Me.wbase = wbase
					Me.gran = gran
				End Sub
				Public Sub compute()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim s As java.util.concurrent.CountedCompleter(Of ?) = Me
					Dim a As SByte() = Me.a, w As SByte() = Me.w ' localize all params
					Dim b As Integer = Me.base, n As Integer = Me.size, wb As Integer = Me.wbase, g As Integer = Me.gran
					Do While n > g
						Dim h As Integer = CInt(CUInt(n) >> 1), q As Integer = CInt(CUInt(h) >> 1), u As Integer = h + q ' quartiles
						Dim fc As New Relay(New Merger(s, w, a, wb, h, wb+h, n-h, b, g))
						Dim rc As New Relay(New Merger(fc, a, w, b+h, q, b+u, n-u, wb+h, g))
						CType(New Sorter(rc, a, w, b+u, n-u, wb+u, g), Sorter).fork()
						CType(New Sorter(rc, a, w, b+h, q, wb+h, g), Sorter).fork()
						Dim bc As New Relay(New Merger(fc, a, w, b, q, b+q, h-q, wb, g))
						CType(New Sorter(bc, a, w, b+q, h-q, wb+q, g), Sorter).fork()
						s = New EmptyCompleter(bc)
						n = q
					Loop
					DualPivotQuicksort.sort(a, b, b + n - 1)
					s.tryComplete()
				End Sub
			End Class

			Friend NotInheritable Class Merger
				Inherits java.util.concurrent.CountedCompleter(Of Void)

				Friend Const serialVersionUID As Long = 2446542900576103244L
				Friend ReadOnly a, w As SByte() ' main and workspace arrays
				Friend ReadOnly lbase, lsize, rbase, rsize, wbase, gran As Integer
				Friend Sub New(Of T1)(ByVal par As java.util.concurrent.CountedCompleter(Of T1), ByVal a As SByte(), ByVal w As SByte(), ByVal lbase As Integer, ByVal lsize As Integer, ByVal rbase As Integer, ByVal rsize As Integer, ByVal wbase As Integer, ByVal gran As Integer)
					MyBase.New(par)
					Me.a = a
					Me.w = w
					Me.lbase = lbase
					Me.lsize = lsize
					Me.rbase = rbase
					Me.rsize = rsize
					Me.wbase = wbase
					Me.gran = gran
				End Sub

				Public Sub compute()
					Dim a As SByte() = Me.a, w As SByte() = Me.w ' localize all params
					Dim lb As Integer = Me.lbase, ln As Integer = Me.lsize, rb As Integer = Me.rbase, rn As Integer = Me.rsize, k As Integer = Me.wbase, g As Integer = Me.gran
					If a Is Nothing OrElse w Is Nothing OrElse lb < 0 OrElse rb < 0 OrElse k < 0 Then Throw New IllegalStateException ' hoist checks
					Dim lh As Integer
					rh
					Do ' split larger, find point in smaller
						If ln >= rn Then
							If ln <= g Then Exit Do
							rh = rn
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
							Dim Split As SByte = a((lh = CInt(CUInt(ln) >> 1)) + lb)
							Dim lo As Integer = 0
							Do While lo < rh
								Dim rm As Integer = CInt(CUInt((lo + rh)) >> 1)
								If Split <= a(rm + rb) Then
									rh = rm
								Else
									lo = rm + 1
								End If
							Loop
						Else
							If rn <= g Then Exit Do
							lh = ln
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
							Dim Split As SByte = a((rh = CInt(CUInt(rn) >> 1)) + rb)
							Dim lo As Integer = 0
							Do While lo < lh
								Dim lm As Integer = CInt(CUInt((lo + lh)) >> 1)
								If Split <= a(lm + lb) Then
									lh = lm
								Else
									lo = lm + 1
								End If
							Loop
						End If
						Dim m As New Merger(Me, a, w, lb + lh, ln - lh, rb + rh, rn - rh, k + lh + rh, g)
						rn = rh
						ln = lh
						addToPendingCount(1)
						m.fork()
					Loop

					Dim lf As Integer = lb + ln, rf As Integer = rb + rn ' index bounds
					Do While lb < lf AndAlso rb < rf
						Dim t, al, ar As SByte
						al = a(lb)
						ar = a(rb)
						If al <= ar Then
							lb += 1
							t = al
						Else
							rb += 1
							t = ar
						End If
						w(k) = t
						k += 1
					Loop
					If rb < rf Then
						Array.Copy(a, rb, w, k, rf - rb)
					ElseIf lb < lf Then
						Array.Copy(a, lb, w, k, lf - lb)
					End If
					tryComplete()
				End Sub
			End Class
		End Class ' FJByte

		''' <summary>
		''' char support class </summary>
		Friend NotInheritable Class FJChar
			Friend NotInheritable Class Sorter
				Inherits java.util.concurrent.CountedCompleter(Of Void)

				Friend Const serialVersionUID As Long = 2446542900576103244L
				Friend ReadOnly a, w As Char()
				Friend ReadOnly base, size, wbase, gran As Integer
				Friend Sub New(Of T1)(ByVal par As java.util.concurrent.CountedCompleter(Of T1), ByVal a As Char(), ByVal w As Char(), ByVal base As Integer, ByVal size As Integer, ByVal wbase As Integer, ByVal gran As Integer)
					MyBase.New(par)
					Me.a = a
					Me.w = w
					Me.base = base
					Me.size = size
					Me.wbase = wbase
					Me.gran = gran
				End Sub
				Public Sub compute()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim s As java.util.concurrent.CountedCompleter(Of ?) = Me
					Dim a As Char() = Me.a, w As Char() = Me.w ' localize all params
					Dim b As Integer = Me.base, n As Integer = Me.size, wb As Integer = Me.wbase, g As Integer = Me.gran
					Do While n > g
						Dim h As Integer = CInt(CUInt(n) >> 1), q As Integer = CInt(CUInt(h) >> 1), u As Integer = h + q ' quartiles
						Dim fc As New Relay(New Merger(s, w, a, wb, h, wb+h, n-h, b, g))
						Dim rc As New Relay(New Merger(fc, a, w, b+h, q, b+u, n-u, wb+h, g))
						CType(New Sorter(rc, a, w, b+u, n-u, wb+u, g), Sorter).fork()
						CType(New Sorter(rc, a, w, b+h, q, wb+h, g), Sorter).fork()
						Dim bc As New Relay(New Merger(fc, a, w, b, q, b+q, h-q, wb, g))
						CType(New Sorter(bc, a, w, b+q, h-q, wb+q, g), Sorter).fork()
						s = New EmptyCompleter(bc)
						n = q
					Loop
					DualPivotQuicksort.sort(a, b, b + n - 1, w, wb, n)
					s.tryComplete()
				End Sub
			End Class

			Friend NotInheritable Class Merger
				Inherits java.util.concurrent.CountedCompleter(Of Void)

				Friend Const serialVersionUID As Long = 2446542900576103244L
				Friend ReadOnly a, w As Char() ' main and workspace arrays
				Friend ReadOnly lbase, lsize, rbase, rsize, wbase, gran As Integer
				Friend Sub New(Of T1)(ByVal par As java.util.concurrent.CountedCompleter(Of T1), ByVal a As Char(), ByVal w As Char(), ByVal lbase As Integer, ByVal lsize As Integer, ByVal rbase As Integer, ByVal rsize As Integer, ByVal wbase As Integer, ByVal gran As Integer)
					MyBase.New(par)
					Me.a = a
					Me.w = w
					Me.lbase = lbase
					Me.lsize = lsize
					Me.rbase = rbase
					Me.rsize = rsize
					Me.wbase = wbase
					Me.gran = gran
				End Sub

				Public Sub compute()
					Dim a As Char() = Me.a, w As Char() = Me.w ' localize all params
					Dim lb As Integer = Me.lbase, ln As Integer = Me.lsize, rb As Integer = Me.rbase, rn As Integer = Me.rsize, k As Integer = Me.wbase, g As Integer = Me.gran
					If a Is Nothing OrElse w Is Nothing OrElse lb < 0 OrElse rb < 0 OrElse k < 0 Then Throw New IllegalStateException ' hoist checks
					Dim lh As Integer
					rh
					Do ' split larger, find point in smaller
						If ln >= rn Then
							If ln <= g Then Exit Do
							rh = rn
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
							Dim Split As Char = a((lh = CInt(CUInt(ln) >> 1)) + lb)
							Dim lo As Integer = 0
							Do While lo < rh
								Dim rm As Integer = CInt(CUInt((lo + rh)) >> 1)
								If Split <= a(rm + rb) Then
									rh = rm
								Else
									lo = rm + 1
								End If
							Loop
						Else
							If rn <= g Then Exit Do
							lh = ln
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
							Dim Split As Char = a((rh = CInt(CUInt(rn) >> 1)) + rb)
							Dim lo As Integer = 0
							Do While lo < lh
								Dim lm As Integer = CInt(CUInt((lo + lh)) >> 1)
								If Split <= a(lm + lb) Then
									lh = lm
								Else
									lo = lm + 1
								End If
							Loop
						End If
						Dim m As New Merger(Me, a, w, lb + lh, ln - lh, rb + rh, rn - rh, k + lh + rh, g)
						rn = rh
						ln = lh
						addToPendingCount(1)
						m.fork()
					Loop

					Dim lf As Integer = lb + ln, rf As Integer = rb + rn ' index bounds
					Do While lb < lf AndAlso rb < rf
						Dim t, al, ar As Char
						al = a(lb)
						ar = a(rb)
						If al <= ar Then
							lb += 1
							t = al
						Else
							rb += 1
							t = ar
						End If
						w(k) = t
						k += 1
					Loop
					If rb < rf Then
						Array.Copy(a, rb, w, k, rf - rb)
					ElseIf lb < lf Then
						Array.Copy(a, lb, w, k, lf - lb)
					End If
					tryComplete()
				End Sub
			End Class
		End Class ' FJChar

		''' <summary>
		''' short support class </summary>
		Friend NotInheritable Class FJShort
			Friend NotInheritable Class Sorter
				Inherits java.util.concurrent.CountedCompleter(Of Void)

				Friend Const serialVersionUID As Long = 2446542900576103244L
				Friend ReadOnly a, w As Short()
				Friend ReadOnly base, size, wbase, gran As Integer
				Friend Sub New(Of T1)(ByVal par As java.util.concurrent.CountedCompleter(Of T1), ByVal a As Short(), ByVal w As Short(), ByVal base As Integer, ByVal size As Integer, ByVal wbase As Integer, ByVal gran As Integer)
					MyBase.New(par)
					Me.a = a
					Me.w = w
					Me.base = base
					Me.size = size
					Me.wbase = wbase
					Me.gran = gran
				End Sub
				Public Sub compute()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim s As java.util.concurrent.CountedCompleter(Of ?) = Me
					Dim a As Short() = Me.a, w As Short() = Me.w ' localize all params
					Dim b As Integer = Me.base, n As Integer = Me.size, wb As Integer = Me.wbase, g As Integer = Me.gran
					Do While n > g
						Dim h As Integer = CInt(CUInt(n) >> 1), q As Integer = CInt(CUInt(h) >> 1), u As Integer = h + q ' quartiles
						Dim fc As New Relay(New Merger(s, w, a, wb, h, wb+h, n-h, b, g))
						Dim rc As New Relay(New Merger(fc, a, w, b+h, q, b+u, n-u, wb+h, g))
						CType(New Sorter(rc, a, w, b+u, n-u, wb+u, g), Sorter).fork()
						CType(New Sorter(rc, a, w, b+h, q, wb+h, g), Sorter).fork()
						Dim bc As New Relay(New Merger(fc, a, w, b, q, b+q, h-q, wb, g))
						CType(New Sorter(bc, a, w, b+q, h-q, wb+q, g), Sorter).fork()
						s = New EmptyCompleter(bc)
						n = q
					Loop
					DualPivotQuicksort.sort(a, b, b + n - 1, w, wb, n)
					s.tryComplete()
				End Sub
			End Class

			Friend NotInheritable Class Merger
				Inherits java.util.concurrent.CountedCompleter(Of Void)

				Friend Const serialVersionUID As Long = 2446542900576103244L
				Friend ReadOnly a, w As Short() ' main and workspace arrays
				Friend ReadOnly lbase, lsize, rbase, rsize, wbase, gran As Integer
				Friend Sub New(Of T1)(ByVal par As java.util.concurrent.CountedCompleter(Of T1), ByVal a As Short(), ByVal w As Short(), ByVal lbase As Integer, ByVal lsize As Integer, ByVal rbase As Integer, ByVal rsize As Integer, ByVal wbase As Integer, ByVal gran As Integer)
					MyBase.New(par)
					Me.a = a
					Me.w = w
					Me.lbase = lbase
					Me.lsize = lsize
					Me.rbase = rbase
					Me.rsize = rsize
					Me.wbase = wbase
					Me.gran = gran
				End Sub

				Public Sub compute()
					Dim a As Short() = Me.a, w As Short() = Me.w ' localize all params
					Dim lb As Integer = Me.lbase, ln As Integer = Me.lsize, rb As Integer = Me.rbase, rn As Integer = Me.rsize, k As Integer = Me.wbase, g As Integer = Me.gran
					If a Is Nothing OrElse w Is Nothing OrElse lb < 0 OrElse rb < 0 OrElse k < 0 Then Throw New IllegalStateException ' hoist checks
					Dim lh As Integer
					rh
					Do ' split larger, find point in smaller
						If ln >= rn Then
							If ln <= g Then Exit Do
							rh = rn
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
							Dim Split As Short = a((lh = CInt(CUInt(ln) >> 1)) + lb)
							Dim lo As Integer = 0
							Do While lo < rh
								Dim rm As Integer = CInt(CUInt((lo + rh)) >> 1)
								If Split <= a(rm + rb) Then
									rh = rm
								Else
									lo = rm + 1
								End If
							Loop
						Else
							If rn <= g Then Exit Do
							lh = ln
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
							Dim Split As Short = a((rh = CInt(CUInt(rn) >> 1)) + rb)
							Dim lo As Integer = 0
							Do While lo < lh
								Dim lm As Integer = CInt(CUInt((lo + lh)) >> 1)
								If Split <= a(lm + lb) Then
									lh = lm
								Else
									lo = lm + 1
								End If
							Loop
						End If
						Dim m As New Merger(Me, a, w, lb + lh, ln - lh, rb + rh, rn - rh, k + lh + rh, g)
						rn = rh
						ln = lh
						addToPendingCount(1)
						m.fork()
					Loop

					Dim lf As Integer = lb + ln, rf As Integer = rb + rn ' index bounds
					Do While lb < lf AndAlso rb < rf
						Dim t, al, ar As Short
						al = a(lb)
						ar = a(rb)
						If al <= ar Then
							lb += 1
							t = al
						Else
							rb += 1
							t = ar
						End If
						w(k) = t
						k += 1
					Loop
					If rb < rf Then
						Array.Copy(a, rb, w, k, rf - rb)
					ElseIf lb < lf Then
						Array.Copy(a, lb, w, k, lf - lb)
					End If
					tryComplete()
				End Sub
			End Class
		End Class ' FJShort

		''' <summary>
		''' int support class </summary>
		Friend NotInheritable Class FJInt
			Friend NotInheritable Class Sorter
				Inherits java.util.concurrent.CountedCompleter(Of Void)

				Friend Const serialVersionUID As Long = 2446542900576103244L
				Friend ReadOnly a, w As Integer()
				Friend ReadOnly base, size, wbase, gran As Integer
				Friend Sub New(Of T1)(ByVal par As java.util.concurrent.CountedCompleter(Of T1), ByVal a As Integer(), ByVal w As Integer(), ByVal base As Integer, ByVal size As Integer, ByVal wbase As Integer, ByVal gran As Integer)
					MyBase.New(par)
					Me.a = a
					Me.w = w
					Me.base = base
					Me.size = size
					Me.wbase = wbase
					Me.gran = gran
				End Sub
				Public Sub compute()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim s As java.util.concurrent.CountedCompleter(Of ?) = Me
					Dim a As Integer() = Me.a, w As Integer() = Me.w ' localize all params
					Dim b As Integer = Me.base, n As Integer = Me.size, wb As Integer = Me.wbase, g As Integer = Me.gran
					Do While n > g
						Dim h As Integer = CInt(CUInt(n) >> 1), q As Integer = CInt(CUInt(h) >> 1), u As Integer = h + q ' quartiles
						Dim fc As New Relay(New Merger(s, w, a, wb, h, wb+h, n-h, b, g))
						Dim rc As New Relay(New Merger(fc, a, w, b+h, q, b+u, n-u, wb+h, g))
						CType(New Sorter(rc, a, w, b+u, n-u, wb+u, g), Sorter).fork()
						CType(New Sorter(rc, a, w, b+h, q, wb+h, g), Sorter).fork()
						Dim bc As New Relay(New Merger(fc, a, w, b, q, b+q, h-q, wb, g))
						CType(New Sorter(bc, a, w, b+q, h-q, wb+q, g), Sorter).fork()
						s = New EmptyCompleter(bc)
						n = q
					Loop
					DualPivotQuicksort.sort(a, b, b + n - 1, w, wb, n)
					s.tryComplete()
				End Sub
			End Class

			Friend NotInheritable Class Merger
				Inherits java.util.concurrent.CountedCompleter(Of Void)

				Friend Const serialVersionUID As Long = 2446542900576103244L
				Friend ReadOnly a, w As Integer() ' main and workspace arrays
				Friend ReadOnly lbase, lsize, rbase, rsize, wbase, gran As Integer
				Friend Sub New(Of T1)(ByVal par As java.util.concurrent.CountedCompleter(Of T1), ByVal a As Integer(), ByVal w As Integer(), ByVal lbase As Integer, ByVal lsize As Integer, ByVal rbase As Integer, ByVal rsize As Integer, ByVal wbase As Integer, ByVal gran As Integer)
					MyBase.New(par)
					Me.a = a
					Me.w = w
					Me.lbase = lbase
					Me.lsize = lsize
					Me.rbase = rbase
					Me.rsize = rsize
					Me.wbase = wbase
					Me.gran = gran
				End Sub

				Public Sub compute()
					Dim a As Integer() = Me.a, w As Integer() = Me.w ' localize all params
					Dim lb As Integer = Me.lbase, ln As Integer = Me.lsize, rb As Integer = Me.rbase, rn As Integer = Me.rsize, k As Integer = Me.wbase, g As Integer = Me.gran
					If a Is Nothing OrElse w Is Nothing OrElse lb < 0 OrElse rb < 0 OrElse k < 0 Then Throw New IllegalStateException ' hoist checks
					Dim lh As Integer
					rh
					Do ' split larger, find point in smaller
						If ln >= rn Then
							If ln <= g Then Exit Do
							rh = rn
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
							Dim Split As Integer = a((lh = CInt(CUInt(ln) >> 1)) + lb)
							Dim lo As Integer = 0
							Do While lo < rh
								Dim rm As Integer = CInt(CUInt((lo + rh)) >> 1)
								If Split <= a(rm + rb) Then
									rh = rm
								Else
									lo = rm + 1
								End If
							Loop
						Else
							If rn <= g Then Exit Do
							lh = ln
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
							Dim Split As Integer = a((rh = CInt(CUInt(rn) >> 1)) + rb)
							Dim lo As Integer = 0
							Do While lo < lh
								Dim lm As Integer = CInt(CUInt((lo + lh)) >> 1)
								If Split <= a(lm + lb) Then
									lh = lm
								Else
									lo = lm + 1
								End If
							Loop
						End If
						Dim m As New Merger(Me, a, w, lb + lh, ln - lh, rb + rh, rn - rh, k + lh + rh, g)
						rn = rh
						ln = lh
						addToPendingCount(1)
						m.fork()
					Loop

					Dim lf As Integer = lb + ln, rf As Integer = rb + rn ' index bounds
					Do While lb < lf AndAlso rb < rf
						Dim t, al, ar As Integer
						al = a(lb)
						ar = a(rb)
						If al <= ar Then
							lb += 1
							t = al
						Else
							rb += 1
							t = ar
						End If
						w(k) = t
						k += 1
					Loop
					If rb < rf Then
						Array.Copy(a, rb, w, k, rf - rb)
					ElseIf lb < lf Then
						Array.Copy(a, lb, w, k, lf - lb)
					End If
					tryComplete()
				End Sub
			End Class
		End Class ' FJInt

		''' <summary>
		''' long support class </summary>
		Friend NotInheritable Class FJLong
			Friend NotInheritable Class Sorter
				Inherits java.util.concurrent.CountedCompleter(Of Void)

				Friend Const serialVersionUID As Long = 2446542900576103244L
				Friend ReadOnly a, w As Long()
				Friend ReadOnly base, size, wbase, gran As Integer
				Friend Sub New(Of T1)(ByVal par As java.util.concurrent.CountedCompleter(Of T1), ByVal a As Long(), ByVal w As Long(), ByVal base As Integer, ByVal size As Integer, ByVal wbase As Integer, ByVal gran As Integer)
					MyBase.New(par)
					Me.a = a
					Me.w = w
					Me.base = base
					Me.size = size
					Me.wbase = wbase
					Me.gran = gran
				End Sub
				Public Sub compute()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim s As java.util.concurrent.CountedCompleter(Of ?) = Me
					Dim a As Long() = Me.a, w As Long() = Me.w ' localize all params
					Dim b As Integer = Me.base, n As Integer = Me.size, wb As Integer = Me.wbase, g As Integer = Me.gran
					Do While n > g
						Dim h As Integer = CInt(CUInt(n) >> 1), q As Integer = CInt(CUInt(h) >> 1), u As Integer = h + q ' quartiles
						Dim fc As New Relay(New Merger(s, w, a, wb, h, wb+h, n-h, b, g))
						Dim rc As New Relay(New Merger(fc, a, w, b+h, q, b+u, n-u, wb+h, g))
						CType(New Sorter(rc, a, w, b+u, n-u, wb+u, g), Sorter).fork()
						CType(New Sorter(rc, a, w, b+h, q, wb+h, g), Sorter).fork()
						Dim bc As New Relay(New Merger(fc, a, w, b, q, b+q, h-q, wb, g))
						CType(New Sorter(bc, a, w, b+q, h-q, wb+q, g), Sorter).fork()
						s = New EmptyCompleter(bc)
						n = q
					Loop
					DualPivotQuicksort.sort(a, b, b + n - 1, w, wb, n)
					s.tryComplete()
				End Sub
			End Class

			Friend NotInheritable Class Merger
				Inherits java.util.concurrent.CountedCompleter(Of Void)

				Friend Const serialVersionUID As Long = 2446542900576103244L
				Friend ReadOnly a, w As Long() ' main and workspace arrays
				Friend ReadOnly lbase, lsize, rbase, rsize, wbase, gran As Integer
				Friend Sub New(Of T1)(ByVal par As java.util.concurrent.CountedCompleter(Of T1), ByVal a As Long(), ByVal w As Long(), ByVal lbase As Integer, ByVal lsize As Integer, ByVal rbase As Integer, ByVal rsize As Integer, ByVal wbase As Integer, ByVal gran As Integer)
					MyBase.New(par)
					Me.a = a
					Me.w = w
					Me.lbase = lbase
					Me.lsize = lsize
					Me.rbase = rbase
					Me.rsize = rsize
					Me.wbase = wbase
					Me.gran = gran
				End Sub

				Public Sub compute()
					Dim a As Long() = Me.a, w As Long() = Me.w ' localize all params
					Dim lb As Integer = Me.lbase, ln As Integer = Me.lsize, rb As Integer = Me.rbase, rn As Integer = Me.rsize, k As Integer = Me.wbase, g As Integer = Me.gran
					If a Is Nothing OrElse w Is Nothing OrElse lb < 0 OrElse rb < 0 OrElse k < 0 Then Throw New IllegalStateException ' hoist checks
					Dim lh As Integer
					rh
					Do ' split larger, find point in smaller
						If ln >= rn Then
							If ln <= g Then Exit Do
							rh = rn
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
							Dim Split As Long = a((lh = CInt(CUInt(ln) >> 1)) + lb)
							Dim lo As Integer = 0
							Do While lo < rh
								Dim rm As Integer = CInt(CUInt((lo + rh)) >> 1)
								If Split <= a(rm + rb) Then
									rh = rm
								Else
									lo = rm + 1
								End If
							Loop
						Else
							If rn <= g Then Exit Do
							lh = ln
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
							Dim Split As Long = a((rh = CInt(CUInt(rn) >> 1)) + rb)
							Dim lo As Integer = 0
							Do While lo < lh
								Dim lm As Integer = CInt(CUInt((lo + lh)) >> 1)
								If Split <= a(lm + lb) Then
									lh = lm
								Else
									lo = lm + 1
								End If
							Loop
						End If
						Dim m As New Merger(Me, a, w, lb + lh, ln - lh, rb + rh, rn - rh, k + lh + rh, g)
						rn = rh
						ln = lh
						addToPendingCount(1)
						m.fork()
					Loop

					Dim lf As Integer = lb + ln, rf As Integer = rb + rn ' index bounds
					Do While lb < lf AndAlso rb < rf
						Dim t, al, ar As Long
						al = a(lb)
						ar = a(rb)
						If al <= ar Then
							lb += 1
							t = al
						Else
							rb += 1
							t = ar
						End If
						w(k) = t
						k += 1
					Loop
					If rb < rf Then
						Array.Copy(a, rb, w, k, rf - rb)
					ElseIf lb < lf Then
						Array.Copy(a, lb, w, k, lf - lb)
					End If
					tryComplete()
				End Sub
			End Class
		End Class ' FJLong

		''' <summary>
		''' float support class </summary>
		Friend NotInheritable Class FJFloat
			Friend NotInheritable Class Sorter
				Inherits java.util.concurrent.CountedCompleter(Of Void)

				Friend Const serialVersionUID As Long = 2446542900576103244L
				Friend ReadOnly a, w As Single()
				Friend ReadOnly base, size, wbase, gran As Integer
				Friend Sub New(Of T1)(ByVal par As java.util.concurrent.CountedCompleter(Of T1), ByVal a As Single(), ByVal w As Single(), ByVal base As Integer, ByVal size As Integer, ByVal wbase As Integer, ByVal gran As Integer)
					MyBase.New(par)
					Me.a = a
					Me.w = w
					Me.base = base
					Me.size = size
					Me.wbase = wbase
					Me.gran = gran
				End Sub
				Public Sub compute()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim s As java.util.concurrent.CountedCompleter(Of ?) = Me
					Dim a As Single() = Me.a, w As Single() = Me.w ' localize all params
					Dim b As Integer = Me.base, n As Integer = Me.size, wb As Integer = Me.wbase, g As Integer = Me.gran
					Do While n > g
						Dim h As Integer = CInt(CUInt(n) >> 1), q As Integer = CInt(CUInt(h) >> 1), u As Integer = h + q ' quartiles
						Dim fc As New Relay(New Merger(s, w, a, wb, h, wb+h, n-h, b, g))
						Dim rc As New Relay(New Merger(fc, a, w, b+h, q, b+u, n-u, wb+h, g))
						CType(New Sorter(rc, a, w, b+u, n-u, wb+u, g), Sorter).fork()
						CType(New Sorter(rc, a, w, b+h, q, wb+h, g), Sorter).fork()
						Dim bc As New Relay(New Merger(fc, a, w, b, q, b+q, h-q, wb, g))
						CType(New Sorter(bc, a, w, b+q, h-q, wb+q, g), Sorter).fork()
						s = New EmptyCompleter(bc)
						n = q
					Loop
					DualPivotQuicksort.sort(a, b, b + n - 1, w, wb, n)
					s.tryComplete()
				End Sub
			End Class

			Friend NotInheritable Class Merger
				Inherits java.util.concurrent.CountedCompleter(Of Void)

				Friend Const serialVersionUID As Long = 2446542900576103244L
				Friend ReadOnly a, w As Single() ' main and workspace arrays
				Friend ReadOnly lbase, lsize, rbase, rsize, wbase, gran As Integer
				Friend Sub New(Of T1)(ByVal par As java.util.concurrent.CountedCompleter(Of T1), ByVal a As Single(), ByVal w As Single(), ByVal lbase As Integer, ByVal lsize As Integer, ByVal rbase As Integer, ByVal rsize As Integer, ByVal wbase As Integer, ByVal gran As Integer)
					MyBase.New(par)
					Me.a = a
					Me.w = w
					Me.lbase = lbase
					Me.lsize = lsize
					Me.rbase = rbase
					Me.rsize = rsize
					Me.wbase = wbase
					Me.gran = gran
				End Sub

				Public Sub compute()
					Dim a As Single() = Me.a, w As Single() = Me.w ' localize all params
					Dim lb As Integer = Me.lbase, ln As Integer = Me.lsize, rb As Integer = Me.rbase, rn As Integer = Me.rsize, k As Integer = Me.wbase, g As Integer = Me.gran
					If a Is Nothing OrElse w Is Nothing OrElse lb < 0 OrElse rb < 0 OrElse k < 0 Then Throw New IllegalStateException ' hoist checks
					Dim lh As Integer
					rh
					Do ' split larger, find point in smaller
						If ln >= rn Then
							If ln <= g Then Exit Do
							rh = rn
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
							Dim Split As Single = a((lh = CInt(CUInt(ln) >> 1)) + lb)
							Dim lo As Integer = 0
							Do While lo < rh
								Dim rm As Integer = CInt(CUInt((lo + rh)) >> 1)
								If Split <= a(rm + rb) Then
									rh = rm
								Else
									lo = rm + 1
								End If
							Loop
						Else
							If rn <= g Then Exit Do
							lh = ln
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
							Dim Split As Single = a((rh = CInt(CUInt(rn) >> 1)) + rb)
							Dim lo As Integer = 0
							Do While lo < lh
								Dim lm As Integer = CInt(CUInt((lo + lh)) >> 1)
								If Split <= a(lm + lb) Then
									lh = lm
								Else
									lo = lm + 1
								End If
							Loop
						End If
						Dim m As New Merger(Me, a, w, lb + lh, ln - lh, rb + rh, rn - rh, k + lh + rh, g)
						rn = rh
						ln = lh
						addToPendingCount(1)
						m.fork()
					Loop

					Dim lf As Integer = lb + ln, rf As Integer = rb + rn ' index bounds
					Do While lb < lf AndAlso rb < rf
						Dim t, al, ar As Single
						al = a(lb)
						ar = a(rb)
						If al <= ar Then
							lb += 1
							t = al
						Else
							rb += 1
							t = ar
						End If
						w(k) = t
						k += 1
					Loop
					If rb < rf Then
						Array.Copy(a, rb, w, k, rf - rb)
					ElseIf lb < lf Then
						Array.Copy(a, lb, w, k, lf - lb)
					End If
					tryComplete()
				End Sub
			End Class
		End Class ' FJFloat

		''' <summary>
		''' double support class </summary>
		Friend NotInheritable Class FJDouble
			Friend NotInheritable Class Sorter
				Inherits java.util.concurrent.CountedCompleter(Of Void)

				Friend Const serialVersionUID As Long = 2446542900576103244L
				Friend ReadOnly a, w As Double()
				Friend ReadOnly base, size, wbase, gran As Integer
				Friend Sub New(Of T1)(ByVal par As java.util.concurrent.CountedCompleter(Of T1), ByVal a As Double(), ByVal w As Double(), ByVal base As Integer, ByVal size As Integer, ByVal wbase As Integer, ByVal gran As Integer)
					MyBase.New(par)
					Me.a = a
					Me.w = w
					Me.base = base
					Me.size = size
					Me.wbase = wbase
					Me.gran = gran
				End Sub
				Public Sub compute()
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					Dim s As java.util.concurrent.CountedCompleter(Of ?) = Me
					Dim a As Double() = Me.a, w As Double() = Me.w ' localize all params
					Dim b As Integer = Me.base, n As Integer = Me.size, wb As Integer = Me.wbase, g As Integer = Me.gran
					Do While n > g
						Dim h As Integer = CInt(CUInt(n) >> 1), q As Integer = CInt(CUInt(h) >> 1), u As Integer = h + q ' quartiles
						Dim fc As New Relay(New Merger(s, w, a, wb, h, wb+h, n-h, b, g))
						Dim rc As New Relay(New Merger(fc, a, w, b+h, q, b+u, n-u, wb+h, g))
						CType(New Sorter(rc, a, w, b+u, n-u, wb+u, g), Sorter).fork()
						CType(New Sorter(rc, a, w, b+h, q, wb+h, g), Sorter).fork()
						Dim bc As New Relay(New Merger(fc, a, w, b, q, b+q, h-q, wb, g))
						CType(New Sorter(bc, a, w, b+q, h-q, wb+q, g), Sorter).fork()
						s = New EmptyCompleter(bc)
						n = q
					Loop
					DualPivotQuicksort.sort(a, b, b + n - 1, w, wb, n)
					s.tryComplete()
				End Sub
			End Class

			Friend NotInheritable Class Merger
				Inherits java.util.concurrent.CountedCompleter(Of Void)

				Friend Const serialVersionUID As Long = 2446542900576103244L
				Friend ReadOnly a, w As Double() ' main and workspace arrays
				Friend ReadOnly lbase, lsize, rbase, rsize, wbase, gran As Integer
				Friend Sub New(Of T1)(ByVal par As java.util.concurrent.CountedCompleter(Of T1), ByVal a As Double(), ByVal w As Double(), ByVal lbase As Integer, ByVal lsize As Integer, ByVal rbase As Integer, ByVal rsize As Integer, ByVal wbase As Integer, ByVal gran As Integer)
					MyBase.New(par)
					Me.a = a
					Me.w = w
					Me.lbase = lbase
					Me.lsize = lsize
					Me.rbase = rbase
					Me.rsize = rsize
					Me.wbase = wbase
					Me.gran = gran
				End Sub

				Public Sub compute()
					Dim a As Double() = Me.a, w As Double() = Me.w ' localize all params
					Dim lb As Integer = Me.lbase, ln As Integer = Me.lsize, rb As Integer = Me.rbase, rn As Integer = Me.rsize, k As Integer = Me.wbase, g As Integer = Me.gran
					If a Is Nothing OrElse w Is Nothing OrElse lb < 0 OrElse rb < 0 OrElse k < 0 Then Throw New IllegalStateException ' hoist checks
					Dim lh As Integer
					rh
					Do ' split larger, find point in smaller
						If ln >= rn Then
							If ln <= g Then Exit Do
							rh = rn
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
							Dim Split As Double = a((lh = CInt(CUInt(ln) >> 1)) + lb)
							Dim lo As Integer = 0
							Do While lo < rh
								Dim rm As Integer = CInt(CUInt((lo + rh)) >> 1)
								If Split <= a(rm + rb) Then
									rh = rm
								Else
									lo = rm + 1
								End If
							Loop
						Else
							If rn <= g Then Exit Do
							lh = ln
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
							Dim Split As Double = a((rh = CInt(CUInt(rn) >> 1)) + rb)
							Dim lo As Integer = 0
							Do While lo < lh
								Dim lm As Integer = CInt(CUInt((lo + lh)) >> 1)
								If Split <= a(lm + lb) Then
									lh = lm
								Else
									lo = lm + 1
								End If
							Loop
						End If
						Dim m As New Merger(Me, a, w, lb + lh, ln - lh, rb + rh, rn - rh, k + lh + rh, g)
						rn = rh
						ln = lh
						addToPendingCount(1)
						m.fork()
					Loop

					Dim lf As Integer = lb + ln, rf As Integer = rb + rn ' index bounds
					Do While lb < lf AndAlso rb < rf
						Dim t, al, ar As Double
						al = a(lb)
						ar = a(rb)
						If al <= ar Then
							lb += 1
							t = al
						Else
							rb += 1
							t = ar
						End If
						w(k) = t
						k += 1
					Loop
					If rb < rf Then
						Array.Copy(a, rb, w, k, rf - rb)
					ElseIf lb < lf Then
						Array.Copy(a, lb, w, k, lf - lb)
					End If
					tryComplete()
				End Sub
			End Class
		End Class ' FJDouble

	 End Class

End Namespace