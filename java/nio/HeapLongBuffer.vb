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
	''' A read/write HeapLongBuffer.
	''' 
	''' 
	''' 
	''' 
	''' 
	''' 
	''' </summary>

	Friend Class HeapLongBuffer
		Inherits LongBuffer

		' For speed these fields are actually declared in X-Buffer;
		' these declarations are here as documentation
	'    
	'
	'    protected final long[] hb;
	'    protected final int offset;
	'
	'    

		Friend Sub New(ByVal cap As Integer, ByVal lim As Integer) ' package-private

			MyBase.New(-1, 0, lim, cap, New Long(cap - 1){}, 0)
	'        
	'        hb = new long[cap];
	'        offset = 0;
	'        




		End Sub

		Friend Sub New(ByVal buf As Long(), ByVal [off] As Integer, ByVal len As Integer) ' package-private

			MyBase.New(-1, [off], [off] + len, buf.Length, buf, 0)
	'        
	'        hb = buf;
	'        offset = 0;
	'        




		End Sub

		Protected Friend Sub New(ByVal buf As Long(), ByVal mark As Integer, ByVal pos As Integer, ByVal lim As Integer, ByVal cap As Integer, ByVal [off] As Integer)

			MyBase.New(mark, pos, lim, cap, buf, [off])
	'        
	'        hb = buf;
	'        offset = off;
	'        




		End Sub

		Public Overrides Function slice() As LongBuffer
			Return New HeapLongBuffer(hb, -1, 0, Me.remaining(), Me.remaining(), Me.position() + offset)
		End Function

		Public Overrides Function duplicate() As LongBuffer
			Return New HeapLongBuffer(hb, Me.markValue(), Me.position(), Me.limit(), Me.capacity(), offset)
		End Function

		Public Overrides Function asReadOnlyBuffer() As LongBuffer

			Return New HeapLongBufferR(hb, Me.markValue(), Me.position(), Me.limit(), Me.capacity(), offset)



		End Function



		Protected Friend Overridable Function ix(ByVal i As Integer) As Integer
			Return i + offset
		End Function

		Public Overrides Function [get]() As Long
			Return hb(ix(nextGetIndex()))
		End Function

		Public Overrides Function [get](ByVal i As Integer) As Long
			Return hb(ix(checkIndex(i)))
		End Function







		Public Overrides Function [get](ByVal dst As Long(), ByVal offset As Integer, ByVal length As Integer) As LongBuffer
			checkBounds(offset, length, dst.Length)
			If length > remaining() Then Throw New BufferUnderflowException
			Array.Copy(hb, ix(position()), dst, offset, length)
			position(position() + length)
			Return Me
		End Function

		Public Property Overrides direct As Boolean
			Get
				Return False
			End Get
		End Property



		Public Property Overrides [readOnly] As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides Function put(ByVal x As Long) As LongBuffer

			hb(ix(nextPutIndex())) = x
			Return Me



		End Function

		Public Overrides Function put(ByVal i As Integer, ByVal x As Long) As LongBuffer

			hb(ix(checkIndex(i))) = x
			Return Me



		End Function

		Public Overrides Function put(ByVal src As Long(), ByVal offset As Integer, ByVal length As Integer) As LongBuffer

			checkBounds(offset, length, src.Length)
			If length > remaining() Then Throw New BufferOverflowException
			Array.Copy(src, offset, hb, ix(position()), length)
			position(position() + length)
			Return Me



		End Function

		Public Overrides Function put(ByVal src As LongBuffer) As LongBuffer

			If TypeOf src Is HeapLongBuffer Then
				If src Is Me Then Throw New IllegalArgumentException
				Dim sb As HeapLongBuffer = CType(src, HeapLongBuffer)
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

		Public Overrides Function compact() As LongBuffer

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