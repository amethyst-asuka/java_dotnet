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


	Friend Class ByteBufferAsLongBufferRL
		Inherits ByteBufferAsLongBufferL ' package-private








		Friend Sub New(  bb As ByteBuffer) ' package-private












			MyBase.New(bb)

		End Sub

		Friend Sub New(  bb As ByteBuffer,   mark As Integer,   pos As Integer,   lim As Integer,   cap As Integer,   [off] As Integer)





			MyBase.New(bb, mark, pos, lim, cap, [off])

		End Sub

		Public Overrides Function slice() As LongBuffer
			Dim pos As Integer = Me.position()
			Dim lim As Integer = Me.limit()
			assert(pos <= lim)
			Dim [rem] As Integer = (If(pos <= lim, lim - pos, 0))
			Dim [off] As Integer = (pos << 3) + offset
			assert([off] >= 0)
			Return New ByteBufferAsLongBufferRL(bb, -1, 0, [rem], [rem], [off])
		End Function

		Public Overrides Function duplicate() As LongBuffer
			Return New ByteBufferAsLongBufferRL(bb, Me.markValue(), Me.position(), Me.limit(), Me.capacity(), offset)
		End Function

		Public Overrides Function asReadOnlyBuffer() As LongBuffer








			Return duplicate()

		End Function























		Public Overrides Function put(  x As Long) As LongBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function put(  i As Integer,   x As Long) As LongBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function compact() As LongBuffer

















			Throw New ReadOnlyBufferException

		End Function

		Public  Overrides ReadOnly Property  direct As Boolean
			Get
				Return bb.direct
			End Get
		End Property

		Public  Overrides ReadOnly Property  [readOnly] As Boolean
			Get
				Return True
			End Get
		End Property











































		Public Overrides Function order() As ByteOrder




			Return ByteOrder.LITTLE_ENDIAN

		End Function

	End Class

End Namespace