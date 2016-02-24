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


	Friend Class ByteBufferAsFloatBufferRB
		Inherits ByteBufferAsFloatBufferB ' package-private








		Friend Sub New(ByVal bb As ByteBuffer) ' package-private












			MyBase.New(bb)

		End Sub

		Friend Sub New(ByVal bb As ByteBuffer, ByVal mark As Integer, ByVal pos As Integer, ByVal lim As Integer, ByVal cap As Integer, ByVal [off] As Integer)





			MyBase.New(bb, mark, pos, lim, cap, [off])

		End Sub

		Public Overrides Function slice() As FloatBuffer
			Dim pos As Integer = Me.position()
			Dim lim As Integer = Me.limit()
			assert(pos <= lim)
			Dim [rem] As Integer = (If(pos <= lim, lim - pos, 0))
			Dim [off] As Integer = (pos << 2) + offset
			assert([off] >= 0)
			Return New ByteBufferAsFloatBufferRB(bb, -1, 0, [rem], [rem], [off])
		End Function

		Public Overrides Function duplicate() As FloatBuffer
			Return New ByteBufferAsFloatBufferRB(bb, Me.markValue(), Me.position(), Me.limit(), Me.capacity(), offset)
		End Function

		Public Overrides Function asReadOnlyBuffer() As FloatBuffer








			Return duplicate()

		End Function























		Public Overrides Function put(ByVal x As Single) As FloatBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function put(ByVal i As Integer, ByVal x As Single) As FloatBuffer




			Throw New ReadOnlyBufferException

		End Function

		Public Overrides Function compact() As FloatBuffer

















			Throw New ReadOnlyBufferException

		End Function

		Public Property Overrides direct As Boolean
			Get
				Return bb.direct
			End Get
		End Property

		Public Property Overrides [readOnly] As Boolean
			Get
				Return True
			End Get
		End Property











































		Public Overrides Function order() As ByteOrder

			Return ByteOrder.BIG_ENDIAN




		End Function

	End Class

End Namespace