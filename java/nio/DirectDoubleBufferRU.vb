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

' -- This file was mechanically generated: Do not edit! -- //

Namespace java.nio



	Friend Class DirectDoubleBufferRU
		Inherits DirectDoubleBufferU
		Implements sun.nio.ch.DirectBuffer















































































































































		' For duplicates and slices
		'
		Friend Sub New(  db As sun.nio.ch.DirectBuffer,   mark As Integer,   pos As Integer,   lim As Integer,   cap As Integer,   [off] As Integer) ' package-private








			MyBase.New(db, mark, pos, lim, cap, [off])

		End Sub

		Public Overrides Function slice() As DoubleBuffer
			Dim pos As Integer = Me.position()
			Dim lim As Integer = Me.limit()
			assert(pos <= lim)
			Dim [rem] As Integer = (If(pos <= lim, lim - pos, 0))
			Dim [off] As Integer = (pos << 3)
			assert([off] >= 0)
			Return New DirectDoubleBufferRU(Me, -1, 0, [rem], [rem], [off])
		End Function

		Public Overrides Function duplicate() As DoubleBuffer
			Return New DirectDoubleBufferRU(Me, Me.markValue(), Me.position(), Me.limit(), Me.capacity(), 0)
		End Function

		Public Overrides Function asReadOnlyBuffer() As DoubleBuffer








			Return duplicate()

		End Function


























































		Public Overrides Function put(  x As Double) As DoubleBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function put(  i As Integer,   x As Double) As DoubleBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function put(  src As DoubleBuffer) As DoubleBuffer




































			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function put(  src As Double(),   offset As Integer,   length As Integer) As DoubleBuffer




























			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function compact() As DoubleBuffer












			Throw New ReadOnlyBufferException

		End Function

		Public  Overrides ReadOnly Property  direct As Boolean
			Get
				Return True
			End Get
		End Property

		Public  Overrides ReadOnly Property  [readOnly] As Boolean
			Get
				Return True
			End Get
		End Property















































		Public Overrides Function order() As ByteOrder





			Return (If(ByteOrder.nativeOrder() IsNot ByteOrder.BIG_ENDIAN, ByteOrder.LITTLE_ENDIAN, ByteOrder.BIG_ENDIAN))

		End Function


























	End Class

End Namespace