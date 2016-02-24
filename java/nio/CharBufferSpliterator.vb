Imports System.Diagnostics

'
' * Copyright (c) 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.nio


	''' <summary>
	''' A Spliterator.OfInt for sources that traverse and split elements
	''' maintained in a CharBuffer.
	''' 
	''' @implNote
	''' The implementation is based on the code for the Array-based spliterators.
	''' </summary>
	Friend Class CharBufferSpliterator
		Implements java.util.Spliterator.OfInt

		Private ReadOnly buffer_Renamed As CharBuffer
		Private index As Integer ' current index, modified on advance/split
		Private ReadOnly limit As Integer

		Friend Sub New(ByVal buffer_Renamed As CharBuffer)
			Me.New(buffer_Renamed, buffer_Renamed.position(), buffer_Renamed.limit())
		End Sub

		Friend Sub New(ByVal buffer_Renamed As CharBuffer, ByVal origin As Integer, ByVal limit As Integer)
			Debug.Assert(origin <= limit)
			Me.buffer_Renamed = buffer_Renamed
			Me.index = If(origin <= limit, origin, limit)
			Me.limit = limit
		End Sub

		Public Overrides Function trySplit() As OfInt
			Dim lo As Integer = index, mid As Integer = CInt(CUInt((lo + limit)) >> 1)
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Return If(lo >= mid, Nothing, New CharBufferSpliterator(buffer_Renamed, lo, index = mid))
		End Function

		Public Overrides Sub forEachRemaining(ByVal action As java.util.function.IntConsumer)
			If action Is Nothing Then Throw New NullPointerException
			Dim cb As CharBuffer = buffer_Renamed
			Dim i As Integer = index
			Dim hi As Integer = limit
			index = hi
			Do While i < hi
				action.accept(cb.getUnchecked(i))
				i += 1
			Loop
		End Sub

		Public Overrides Function tryAdvance(ByVal action As java.util.function.IntConsumer) As Boolean
			If action Is Nothing Then Throw New NullPointerException
			If index >= 0 AndAlso index < limit Then
				action.accept(buffer_Renamed.getUnchecked(index))
				index += 1
				Return True
			End If
			Return False
		End Function

		Public Overrides Function estimateSize() As Long
			Return CLng(limit - index)
		End Function

		Public Overrides Function characteristics() As Integer
			Return Buffer.SPLITERATOR_CHARACTERISTICS
		End Function
	End Class

End Namespace