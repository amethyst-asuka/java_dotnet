Imports System

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


	''' 
	''' <summary>
	''' A read/write HeapShortBuffer.
	''' 
	''' 
	''' 
	''' 
	''' 
	''' 
	''' </summary>

	Friend Class HeapShortBuffer
		Inherits ShortBuffer

		' For speed these fields are actually declared in X-Buffer;
		' these declarations are here as documentation
	'    
	'
	'    protected final short[] hb;
	'    protected final int offset;
	'
	'    

		Friend Sub New(  cap As Integer,   lim As Integer) ' package-private

			MyBase.New(-1, 0, lim, cap, New Short(cap - 1){}, 0)
	'        
	'        hb = new short[cap];
	'        offset = 0;
	'        




		End Sub

		Friend Sub New(  buf As Short(),   [off] As Integer,   len As Integer) ' package-private

			MyBase.New(-1, [off], [off] + len, buf.Length, buf, 0)
	'        
	'        hb = buf;
	'        offset = 0;
	'        




		End Sub

		Protected Friend Sub New(  buf As Short(),   mark As Integer,   pos As Integer,   lim As Integer,   cap As Integer,   [off] As Integer)

			MyBase.New(mark, pos, lim, cap, buf, [off])
	'        
	'        hb = buf;
	'        offset = off;
	'        




		End Sub

		Public Overrides Function slice() As ShortBuffer
			Return New HeapShortBuffer(hb, -1, 0, Me.remaining(), Me.remaining(), Me.position() + offset)
		End Function

		Public Overrides Function duplicate() As ShortBuffer
			Return New HeapShortBuffer(hb, Me.markValue(), Me.position(), Me.limit(), Me.capacity(), offset)
		End Function

		Public Overrides Function asReadOnlyBuffer() As ShortBuffer

			Return New HeapShortBufferR(hb, Me.markValue(), Me.position(), Me.limit(), Me.capacity(), offset)



		End Function



		Protected Friend Overridable Function ix(  i As Integer) As Integer
			Return i + offset
		End Function

		Public Overrides Function [get]() As Short
			Return hb(ix(nextGetIndex()))
		End Function

		Public Overrides Function [get](  i As Integer) As Short
			Return hb(ix(checkIndex(i)))
		End Function







		Public Overrides Function [get](  dst As Short(),   offset As Integer,   length As Integer) As ShortBuffer
			checkBounds(offset, length, dst.Length)
			If length > remaining() Then Throw New BufferUnderflowException
			Array.Copy(hb, ix(position()), dst, offset, length)
			position(position() + length)
			Return Me
		End Function

		Public  Overrides ReadOnly Property  direct As Boolean
			Get
				Return False
			End Get
		End Property



		Public  Overrides ReadOnly Property  [readOnly] As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides Function put(  x As Short) As ShortBuffer

			hb(ix(nextPutIndex())) = x
			Return Me



		End Function

		Public Overrides Function put(  i As Integer,   x As Short) As ShortBuffer

			hb(ix(checkIndex(i))) = x
			Return Me



		End Function

		Public Overrides Function put(  src As Short(),   offset As Integer,   length As Integer) As ShortBuffer

			checkBounds(offset, length, src.Length)
			If length > remaining() Then Throw New BufferOverflowException
			Array.Copy(src, offset, hb, ix(position()), length)
			position(position() + length)
			Return Me



		End Function

		Public Overrides Function put(  src As ShortBuffer) As ShortBuffer

			If TypeOf src Is HeapShortBuffer Then
				If src Is Me Then Throw New IllegalArgumentException
				Dim sb As HeapShortBuffer = CType(src, HeapShortBuffer)
				Dim n As Integer = sb.remaining()
				If n > remaining() Then Throw New BufferOverflowException
				Array.Copy(sb.hb, sb.ix(sb.position()), hb, ix(position()), n)
				sb.position(sb.position() + n)
				position(position() + n)
			ElseIf src.direct Then
				Dim n As Integer = src.remaining()
				If n > remaining() Then Throw New BufferOverflowException
				src.get(hb, ix(position()), n)
				position(position() + n)
			Else
				MyBase.put(src)
			End If
			Return Me



		End Function

		Public Overrides Function compact() As ShortBuffer

			Array.Copy(hb, ix(position()), hb, ix(0), remaining())
			position(remaining())
			limit(capacity())
			discardMark()
			Return Me



		End Function






































































































































































































































































































































































		Public Overrides Function order() As ByteOrder
			Return ByteOrder.nativeOrder()
		End Function



	End Class

End Namespace