Imports Microsoft.VisualBasic
Imports System.Runtime.CompilerServices
Imports System.Diagnostics
Imports System.Threading
Imports System.Runtime.InteropServices

'
' * Copyright (c) 2000, 2012, Oracle and/or its affiliates. All rights reserved.
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
	''' Access to bits, native and otherwise.
	''' </summary>

	Friend Class Bits ' package-private

		Private Sub New()
		End Sub


		' -- Swapping --

		Friend Shared Function swap(ByVal x As Short) As Short
			Return  java.lang.[Short].reverseBytes(x)
		End Function

		Friend Shared Function swap(ByVal x As Char) As Char
			Return Character.reverseBytes(x)
		End Function

		Friend Shared Function swap(ByVal x As Integer) As Integer
			Return  java.lang.[Integer].reverseBytes(x)
		End Function

		Friend Shared Function swap(ByVal x As Long) As Long
			Return java.lang.[Long].reverseBytes(x)
		End Function


		' -- get/put char --

		Private Shared Function makeChar(ByVal b1 As SByte, ByVal b0 As SByte) As Char
			Return CChar((b1 << 8) Or (b0 And &Hff))
		End Function

		Friend Shared Function getCharL(ByVal bb As ByteBuffer, ByVal bi As Integer) As Char
			Return makeChar(bb._get(bi + 1), bb._get(bi))
		End Function

		Friend Shared Function getCharL(ByVal a As Long) As Char
			Return makeChar(_get(a + 1), _get(a))
		End Function

		Friend Shared Function getCharB(ByVal bb As ByteBuffer, ByVal bi As Integer) As Char
			Return makeChar(bb._get(bi), bb._get(bi + 1))
		End Function

		Friend Shared Function getCharB(ByVal a As Long) As Char
			Return makeChar(_get(a), _get(a + 1))
		End Function

		Friend Shared Function getChar(ByVal bb As ByteBuffer, ByVal bi As Integer, ByVal bigEndian As Boolean) As Char
			Return If(bigEndian, getCharB(bb, bi), getCharL(bb, bi))
		End Function

		Friend Shared Function getChar(ByVal a As Long, ByVal bigEndian As Boolean) As Char
			Return If(bigEndian, getCharB(a), getCharL(a))
		End Function

		Private Shared Function char1(ByVal x As Char) As SByte
			Return CByte(AscW(x) >> 8)
		End Function
		Private Shared Function char0(ByVal x As Char) As SByte
			Return AscW(x)
		End Function

		Friend Shared Sub putCharL(ByVal bb As ByteBuffer, ByVal bi As Integer, ByVal x As Char)
			bb._put(bi, char0(x))
			bb._put(bi + 1, char1(x))
		End Sub

		Friend Shared Sub putCharL(ByVal a As Long, ByVal x As Char)
			_put(a, char0(x))
			_put(a + 1, char1(x))
		End Sub

		Friend Shared Sub putCharB(ByVal bb As ByteBuffer, ByVal bi As Integer, ByVal x As Char)
			bb._put(bi, char1(x))
			bb._put(bi + 1, char0(x))
		End Sub

		Friend Shared Sub putCharB(ByVal a As Long, ByVal x As Char)
			_put(a, char1(x))
			_put(a + 1, char0(x))
		End Sub

		Friend Shared Sub putChar(ByVal bb As ByteBuffer, ByVal bi As Integer, ByVal x As Char, ByVal bigEndian As Boolean)
			If bigEndian Then
				putCharB(bb, bi, x)
			Else
				putCharL(bb, bi, x)
			End If
		End Sub

		Friend Shared Sub putChar(ByVal a As Long, ByVal x As Char, ByVal bigEndian As Boolean)
			If bigEndian Then
				putCharB(a, x)
			Else
				putCharL(a, x)
			End If
		End Sub


		' -- get/put short --

		Private Shared Function makeShort(ByVal b1 As SByte, ByVal b0 As SByte) As Short
			Return CShort(Fix((b1 << 8) Or (b0 And &Hff)))
		End Function

		Friend Shared Function getShortL(ByVal bb As ByteBuffer, ByVal bi As Integer) As Short
			Return makeShort(bb._get(bi + 1), bb._get(bi))
		End Function

		Friend Shared Function getShortL(ByVal a As Long) As Short
			Return makeShort(_get(a + 1), _get(a))
		End Function

		Friend Shared Function getShortB(ByVal bb As ByteBuffer, ByVal bi As Integer) As Short
			Return makeShort(bb._get(bi), bb._get(bi + 1))
		End Function

		Friend Shared Function getShortB(ByVal a As Long) As Short
			Return makeShort(_get(a), _get(a + 1))
		End Function

		Friend Shared Function getShort(ByVal bb As ByteBuffer, ByVal bi As Integer, ByVal bigEndian As Boolean) As Short
			Return If(bigEndian, getShortB(bb, bi), getShortL(bb, bi))
		End Function

		Friend Shared Function getShort(ByVal a As Long, ByVal bigEndian As Boolean) As Short
			Return If(bigEndian, getShortB(a), getShortL(a))
		End Function

		Private Shared Function short1(ByVal x As Short) As SByte
			Return CByte(x >> 8)
		End Function
		Private Shared Function short0(ByVal x As Short) As SByte
			Return CByte(x)
		End Function

		Friend Shared Sub putShortL(ByVal bb As ByteBuffer, ByVal bi As Integer, ByVal x As Short)
			bb._put(bi, short0(x))
			bb._put(bi + 1, short1(x))
		End Sub

		Friend Shared Sub putShortL(ByVal a As Long, ByVal x As Short)
			_put(a, short0(x))
			_put(a + 1, short1(x))
		End Sub

		Friend Shared Sub putShortB(ByVal bb As ByteBuffer, ByVal bi As Integer, ByVal x As Short)
			bb._put(bi, short1(x))
			bb._put(bi + 1, short0(x))
		End Sub

		Friend Shared Sub putShortB(ByVal a As Long, ByVal x As Short)
			_put(a, short1(x))
			_put(a + 1, short0(x))
		End Sub

		Friend Shared Sub putShort(ByVal bb As ByteBuffer, ByVal bi As Integer, ByVal x As Short, ByVal bigEndian As Boolean)
			If bigEndian Then
				putShortB(bb, bi, x)
			Else
				putShortL(bb, bi, x)
			End If
		End Sub

		Friend Shared Sub putShort(ByVal a As Long, ByVal x As Short, ByVal bigEndian As Boolean)
			If bigEndian Then
				putShortB(a, x)
			Else
				putShortL(a, x)
			End If
		End Sub


		' -- get/put int --

		Private Shared Function makeInt(ByVal b3 As SByte, ByVal b2 As SByte, ByVal b1 As SByte, ByVal b0 As SByte) As Integer
			Return (((b3) << 24) Or ((b2 And &Hff) << 16) Or ((b1 And &Hff) << 8) Or ((b0 And &Hff)))
		End Function

		Friend Shared Function getIntL(ByVal bb As ByteBuffer, ByVal bi As Integer) As Integer
			Return makeInt(bb._get(bi + 3), bb._get(bi + 2), bb._get(bi + 1), bb._get(bi))
		End Function

		Friend Shared Function getIntL(ByVal a As Long) As Integer
			Return makeInt(_get(a + 3), _get(a + 2), _get(a + 1), _get(a))
		End Function

		Friend Shared Function getIntB(ByVal bb As ByteBuffer, ByVal bi As Integer) As Integer
			Return makeInt(bb._get(bi), bb._get(bi + 1), bb._get(bi + 2), bb._get(bi + 3))
		End Function

		Friend Shared Function getIntB(ByVal a As Long) As Integer
			Return makeInt(_get(a), _get(a + 1), _get(a + 2), _get(a + 3))
		End Function

		Friend Shared Function getInt(ByVal bb As ByteBuffer, ByVal bi As Integer, ByVal bigEndian As Boolean) As Integer
			Return If(bigEndian, getIntB(bb, bi), getIntL(bb, bi))
		End Function

		Friend Shared Function getInt(ByVal a As Long, ByVal bigEndian As Boolean) As Integer
			Return If(bigEndian, getIntB(a), getIntL(a))
		End Function

		Private Shared Function int3(ByVal x As Integer) As SByte
			Return CByte(x >> 24)
		End Function
		Private Shared Function int2(ByVal x As Integer) As SByte
			Return CByte(x >> 16)
		End Function
		Private Shared Function int1(ByVal x As Integer) As SByte
			Return CByte(x >> 8)
		End Function
		Private Shared Function int0(ByVal x As Integer) As SByte
			Return CByte(x)
		End Function

		Friend Shared Sub putIntL(ByVal bb As ByteBuffer, ByVal bi As Integer, ByVal x As Integer)
			bb._put(bi + 3, int3(x))
			bb._put(bi + 2, int2(x))
			bb._put(bi + 1, int1(x))
			bb._put(bi, int0(x))
		End Sub

		Friend Shared Sub putIntL(ByVal a As Long, ByVal x As Integer)
			_put(a + 3, int3(x))
			_put(a + 2, int2(x))
			_put(a + 1, int1(x))
			_put(a, int0(x))
		End Sub

		Friend Shared Sub putIntB(ByVal bb As ByteBuffer, ByVal bi As Integer, ByVal x As Integer)
			bb._put(bi, int3(x))
			bb._put(bi + 1, int2(x))
			bb._put(bi + 2, int1(x))
			bb._put(bi + 3, int0(x))
		End Sub

		Friend Shared Sub putIntB(ByVal a As Long, ByVal x As Integer)
			_put(a, int3(x))
			_put(a + 1, int2(x))
			_put(a + 2, int1(x))
			_put(a + 3, int0(x))
		End Sub

		Friend Shared Sub putInt(ByVal bb As ByteBuffer, ByVal bi As Integer, ByVal x As Integer, ByVal bigEndian As Boolean)
			If bigEndian Then
				putIntB(bb, bi, x)
			Else
				putIntL(bb, bi, x)
			End If
		End Sub

		Friend Shared Sub putInt(ByVal a As Long, ByVal x As Integer, ByVal bigEndian As Boolean)
			If bigEndian Then
				putIntB(a, x)
			Else
				putIntL(a, x)
			End If
		End Sub


		' -- get/put long --

		Private Shared Function makeLong(ByVal b7 As SByte, ByVal b6 As SByte, ByVal b5 As SByte, ByVal b4 As SByte, ByVal b3 As SByte, ByVal b2 As SByte, ByVal b1 As SByte, ByVal b0 As SByte) As Long
			Return (((CLng(b7)) << 56) Or ((CLng(b6) And &Hff) << 48) Or ((CLng(b5) And &Hff) << 40) Or ((CLng(b4) And &Hff) << 32) Or ((CLng(b3) And &Hff) << 24) Or ((CLng(b2) And &Hff) << 16) Or ((CLng(b1) And &Hff) << 8) Or ((CLng(b0) And &Hff)))
		End Function

		Friend Shared Function getLongL(ByVal bb As ByteBuffer, ByVal bi As Integer) As Long
			Return makeLong(bb._get(bi + 7), bb._get(bi + 6), bb._get(bi + 5), bb._get(bi + 4), bb._get(bi + 3), bb._get(bi + 2), bb._get(bi + 1), bb._get(bi))
		End Function

		Friend Shared Function getLongL(ByVal a As Long) As Long
			Return makeLong(_get(a + 7), _get(a + 6), _get(a + 5), _get(a + 4), _get(a + 3), _get(a + 2), _get(a + 1), _get(a))
		End Function

		Friend Shared Function getLongB(ByVal bb As ByteBuffer, ByVal bi As Integer) As Long
			Return makeLong(bb._get(bi), bb._get(bi + 1), bb._get(bi + 2), bb._get(bi + 3), bb._get(bi + 4), bb._get(bi + 5), bb._get(bi + 6), bb._get(bi + 7))
		End Function

		Friend Shared Function getLongB(ByVal a As Long) As Long
			Return makeLong(_get(a), _get(a + 1), _get(a + 2), _get(a + 3), _get(a + 4), _get(a + 5), _get(a + 6), _get(a + 7))
		End Function

		Friend Shared Function getLong(ByVal bb As ByteBuffer, ByVal bi As Integer, ByVal bigEndian As Boolean) As Long
			Return If(bigEndian, getLongB(bb, bi), getLongL(bb, bi))
		End Function

		Friend Shared Function getLong(ByVal a As Long, ByVal bigEndian As Boolean) As Long
			Return If(bigEndian, getLongB(a), getLongL(a))
		End Function

		Private Shared Function long7(ByVal x As Long) As SByte
			Return CByte(x >> 56)
		End Function
		Private Shared Function long6(ByVal x As Long) As SByte
			Return CByte(x >> 48)
		End Function
		Private Shared Function long5(ByVal x As Long) As SByte
			Return CByte(x >> 40)
		End Function
		Private Shared Function long4(ByVal x As Long) As SByte
			Return CByte(x >> 32)
		End Function
		Private Shared Function long3(ByVal x As Long) As SByte
			Return CByte(x >> 24)
		End Function
		Private Shared Function long2(ByVal x As Long) As SByte
			Return CByte(x >> 16)
		End Function
		Private Shared Function long1(ByVal x As Long) As SByte
			Return CByte(x >> 8)
		End Function
		Private Shared Function long0(ByVal x As Long) As SByte
			Return CByte(x)
		End Function

		Friend Shared Sub putLongL(ByVal bb As ByteBuffer, ByVal bi As Integer, ByVal x As Long)
			bb._put(bi + 7, long7(x))
			bb._put(bi + 6, long6(x))
			bb._put(bi + 5, long5(x))
			bb._put(bi + 4, long4(x))
			bb._put(bi + 3, long3(x))
			bb._put(bi + 2, long2(x))
			bb._put(bi + 1, long1(x))
			bb._put(bi, long0(x))
		End Sub

		Friend Shared Sub putLongL(ByVal a As Long, ByVal x As Long)
			_put(a + 7, long7(x))
			_put(a + 6, long6(x))
			_put(a + 5, long5(x))
			_put(a + 4, long4(x))
			_put(a + 3, long3(x))
			_put(a + 2, long2(x))
			_put(a + 1, long1(x))
			_put(a, long0(x))
		End Sub

		Friend Shared Sub putLongB(ByVal bb As ByteBuffer, ByVal bi As Integer, ByVal x As Long)
			bb._put(bi, long7(x))
			bb._put(bi + 1, long6(x))
			bb._put(bi + 2, long5(x))
			bb._put(bi + 3, long4(x))
			bb._put(bi + 4, long3(x))
			bb._put(bi + 5, long2(x))
			bb._put(bi + 6, long1(x))
			bb._put(bi + 7, long0(x))
		End Sub

		Friend Shared Sub putLongB(ByVal a As Long, ByVal x As Long)
			_put(a, long7(x))
			_put(a + 1, long6(x))
			_put(a + 2, long5(x))
			_put(a + 3, long4(x))
			_put(a + 4, long3(x))
			_put(a + 5, long2(x))
			_put(a + 6, long1(x))
			_put(a + 7, long0(x))
		End Sub

		Friend Shared Sub putLong(ByVal bb As ByteBuffer, ByVal bi As Integer, ByVal x As Long, ByVal bigEndian As Boolean)
			If bigEndian Then
				putLongB(bb, bi, x)
			Else
				putLongL(bb, bi, x)
			End If
		End Sub

		Friend Shared Sub putLong(ByVal a As Long, ByVal x As Long, ByVal bigEndian As Boolean)
			If bigEndian Then
				putLongB(a, x)
			Else
				putLongL(a, x)
			End If
		End Sub


		' -- get/put float --

		Friend Shared Function getFloatL(ByVal bb As ByteBuffer, ByVal bi As Integer) As Single
			Return Float.intBitsToFloat(getIntL(bb, bi))
		End Function

		Friend Shared Function getFloatL(ByVal a As Long) As Single
			Return Float.intBitsToFloat(getIntL(a))
		End Function

		Friend Shared Function getFloatB(ByVal bb As ByteBuffer, ByVal bi As Integer) As Single
			Return Float.intBitsToFloat(getIntB(bb, bi))
		End Function

		Friend Shared Function getFloatB(ByVal a As Long) As Single
			Return Float.intBitsToFloat(getIntB(a))
		End Function

		Friend Shared Function getFloat(ByVal bb As ByteBuffer, ByVal bi As Integer, ByVal bigEndian As Boolean) As Single
			Return If(bigEndian, getFloatB(bb, bi), getFloatL(bb, bi))
		End Function

		Friend Shared Function getFloat(ByVal a As Long, ByVal bigEndian As Boolean) As Single
			Return If(bigEndian, getFloatB(a), getFloatL(a))
		End Function

		Friend Shared Sub putFloatL(ByVal bb As ByteBuffer, ByVal bi As Integer, ByVal x As Single)
			putIntL(bb, bi, Float.floatToRawIntBits(x))
		End Sub

		Friend Shared Sub putFloatL(ByVal a As Long, ByVal x As Single)
			putIntL(a, Float.floatToRawIntBits(x))
		End Sub

		Friend Shared Sub putFloatB(ByVal bb As ByteBuffer, ByVal bi As Integer, ByVal x As Single)
			putIntB(bb, bi, Float.floatToRawIntBits(x))
		End Sub

		Friend Shared Sub putFloatB(ByVal a As Long, ByVal x As Single)
			putIntB(a, Float.floatToRawIntBits(x))
		End Sub

		Friend Shared Sub putFloat(ByVal bb As ByteBuffer, ByVal bi As Integer, ByVal x As Single, ByVal bigEndian As Boolean)
			If bigEndian Then
				putFloatB(bb, bi, x)
			Else
				putFloatL(bb, bi, x)
			End If
		End Sub

		Friend Shared Sub putFloat(ByVal a As Long, ByVal x As Single, ByVal bigEndian As Boolean)
			If bigEndian Then
				putFloatB(a, x)
			Else
				putFloatL(a, x)
			End If
		End Sub


		' -- get/put double --

		Friend Shared Function getDoubleL(ByVal bb As ByteBuffer, ByVal bi As Integer) As Double
			Return java.lang.[Double].longBitsToDouble(getLongL(bb, bi))
		End Function

		Friend Shared Function getDoubleL(ByVal a As Long) As Double
			Return java.lang.[Double].longBitsToDouble(getLongL(a))
		End Function

		Friend Shared Function getDoubleB(ByVal bb As ByteBuffer, ByVal bi As Integer) As Double
			Return java.lang.[Double].longBitsToDouble(getLongB(bb, bi))
		End Function

		Friend Shared Function getDoubleB(ByVal a As Long) As Double
			Return java.lang.[Double].longBitsToDouble(getLongB(a))
		End Function

		Friend Shared Function getDouble(ByVal bb As ByteBuffer, ByVal bi As Integer, ByVal bigEndian As Boolean) As Double
			Return If(bigEndian, getDoubleB(bb, bi), getDoubleL(bb, bi))
		End Function

		Friend Shared Function getDouble(ByVal a As Long, ByVal bigEndian As Boolean) As Double
			Return If(bigEndian, getDoubleB(a), getDoubleL(a))
		End Function

		Friend Shared Sub putDoubleL(ByVal bb As ByteBuffer, ByVal bi As Integer, ByVal x As Double)
			putLongL(bb, bi, java.lang.[Double].doubleToRawLongBits(x))
		End Sub

		Friend Shared Sub putDoubleL(ByVal a As Long, ByVal x As Double)
			putLongL(a, java.lang.[Double].doubleToRawLongBits(x))
		End Sub

		Friend Shared Sub putDoubleB(ByVal bb As ByteBuffer, ByVal bi As Integer, ByVal x As Double)
			putLongB(bb, bi, java.lang.[Double].doubleToRawLongBits(x))
		End Sub

		Friend Shared Sub putDoubleB(ByVal a As Long, ByVal x As Double)
			putLongB(a, java.lang.[Double].doubleToRawLongBits(x))
		End Sub

		Friend Shared Sub putDouble(ByVal bb As ByteBuffer, ByVal bi As Integer, ByVal x As Double, ByVal bigEndian As Boolean)
			If bigEndian Then
				putDoubleB(bb, bi, x)
			Else
				putDoubleL(bb, bi, x)
			End If
		End Sub

		Friend Shared Sub putDouble(ByVal a As Long, ByVal x As Double, ByVal bigEndian As Boolean)
			If bigEndian Then
				putDoubleB(a, x)
			Else
				putDoubleL(a, x)
			End If
		End Sub


		' -- Unsafe access --

		Private Shared ReadOnly unsafe_Renamed As sun.misc.Unsafe = sun.misc.Unsafe.unsafe

		Private Shared Function _get(ByVal a As Long) As SByte
			Return unsafe_Renamed.getByte(a)
		End Function

		Private Shared Sub _put(ByVal a As Long, ByVal b As SByte)
			unsafe_Renamed.putByte(a, b)
		End Sub

		Friend Shared Function unsafe() As sun.misc.Unsafe
			Return unsafe_Renamed
		End Function


		' -- Processor and memory-system properties --

		Private Shared ReadOnly byteOrder_Renamed As ByteOrder

		Friend Shared Function byteOrder() As ByteOrder
			If byteOrder_Renamed Is Nothing Then Throw New [Error]("Unknown byte order")
			Return byteOrder_Renamed
		End Function

		Shared Sub New()
			Dim a As Long = unsafe_Renamed.allocateMemory(8)
			Try
				unsafe_Renamed.putLong(a, &H102030405060708L)
				Dim b As SByte = unsafe_Renamed.getByte(a)
				Select Case b
				Case &H1
					byteOrder_Renamed = ByteOrder.BIG_ENDIAN
				Case &H8
					byteOrder_Renamed = ByteOrder.LITTLE_ENDIAN
				Case Else
					Debug.Assert(False)
					byteOrder_Renamed = Nothing
				End Select
			Finally
				unsafe_Renamed.freeMemory(a)
			End Try
			' setup access to this package in SharedSecrets
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			sun.misc.SharedSecrets.setJavaNioAccess(New sun.misc.JavaNioAccess()
	'		{
	'				@Override public sun.misc.JavaNioAccess.BufferPool getDirectBufferPool()
	'				{
	'					Return New sun.misc.JavaNioAccess.BufferPool()
	'					{
	'						@Override public String getName()
	'						{
	'							Return "direct";
	'						}
	'						@Override public long getCount()
	'						{
	'							Return Bits.count;
	'						}
	'						@Override public long getTotalCapacity()
	'						{
	'							Return Bits.totalCapacity;
	'						}
	'						@Override public long getMemoryUsed()
	'						{
	'							Return Bits.reservedMemory;
	'						}
	'					};
	'				}
	'				@Override public ByteBuffer newDirectByteBuffer(long addr, int cap, Object ob)
	'				{
	'					Return New DirectByteBuffer(addr, cap, ob);
	'				}
	'				@Override public void truncate(Buffer buf)
	'				{
	'					buf.truncate();
	'				}
	'		});
		End Sub


		Private Shared pageSize_Renamed As Integer = -1

		Friend Shared Function pageSize() As Integer
			If pageSize_Renamed = -1 Then pageSize_Renamed = unsafe().pageSize()
			Return pageSize_Renamed
		End Function

		Friend Shared Function pageCount(ByVal size As Long) As Integer
			Return CInt(size + CLng(pageSize()) - 1L) \ pageSize()
		End Function

		Private Shared unaligned_Renamed As Boolean
		Private Shared unalignedKnown As Boolean = False

		Friend Shared Function unaligned() As Boolean
			If unalignedKnown Then Return unaligned_Renamed
			Dim arch As String = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("os.arch"))
			unaligned_Renamed = arch.Equals("i386") OrElse arch.Equals("x86") OrElse arch.Equals("amd64") OrElse arch.Equals("x86_64")
			unalignedKnown = True
			Return unaligned_Renamed
		End Function


		' -- Direct memory management --

		' A user-settable upper limit on the maximum amount of allocatable
		' direct buffer memory.  This value may be changed during VM
		' initialization if it is launched with "-XX:MaxDirectMemorySize=<size>".
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private Shared maxMemory As Long = sun.misc.VM.maxDirectMemory()
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private Shared reservedMemory As Long
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private Shared totalCapacity As Long
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private Shared count As Long
		Private Shared memoryLimitSet As Boolean = False

		' These methods should be called whenever direct memory is allocated or
		' freed.  They allow the user to control the amount of direct memory
		' which a process may access.  All sizes are specified in bytes.
		Friend Shared Sub reserveMemory(ByVal size As Long, ByVal cap As Integer)
			SyncLock GetType(Bits)
				If (Not memoryLimitSet) AndAlso sun.misc.VM.booted Then
					maxMemory = sun.misc.VM.maxDirectMemory()
					memoryLimitSet = True
				End If
				' -XX:MaxDirectMemorySize limits the total capacity rather than the
				' actual memory usage, which will differ when buffers are page
				' aligned.
				If cap <= maxMemory - totalCapacity Then
					reservedMemory += size
					totalCapacity += cap
					count += 1
					Return
				End If
			End SyncLock

			System.gc()
			Try
				Thread.Sleep(100)
			Catch x As InterruptedException
				' Restore interrupt status
				Thread.CurrentThread.Interrupt()
			End Try
			SyncLock GetType(Bits)
				If totalCapacity + cap > maxMemory Then Throw New OutOfMemoryError("Direct buffer memory")
				reservedMemory += size
				totalCapacity += cap
				count += 1
			End SyncLock

		End Sub

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Friend Shared Sub unreserveMemory(ByVal size As Long, ByVal cap As Integer)
			If reservedMemory > 0 Then
				reservedMemory -= size
				totalCapacity -= cap
				count -= 1
				assert(reservedMemory > -1)
			End If
		End Sub

		' -- Monitoring of direct buffer usage --


		' -- Bulk get/put acceleration --

		' These numbers represent the point at which we have empirically
		' determined that the average cost of a JNI call exceeds the expense
		' of an element by element copy.  These numbers may change over time.
		Friend Const JNI_COPY_TO_ARRAY_THRESHOLD As Integer = 6
		Friend Const JNI_COPY_FROM_ARRAY_THRESHOLD As Integer = 6

		' This number limits the number of bytes to copy per call to Unsafe's
		' copyMemory method. A limit is imposed to allow for safepoint polling
		' during a large copy
		Friend Shared ReadOnly UNSAFE_COPY_THRESHOLD As Long = 1024L * 1024L

		' These methods do no bounds checking.  Verification that the copy will not
		' result in memory corruption should be done prior to invocation.
		' All positions and lengths are specified in bytes.

		''' <summary>
		''' Copy from given source array to destination address.
		''' </summary>
		''' <param name="src">
		'''          source array </param>
		''' <param name="srcBaseOffset">
		'''          offset of first element of storage in source array </param>
		''' <param name="srcPos">
		'''          offset within source array of the first element to read </param>
		''' <param name="dstAddr">
		'''          destination address </param>
		''' <param name="length">
		'''          number of bytes to copy </param>
		Friend Shared Sub copyFromArray(ByVal src As Object, ByVal srcBaseOffset As Long, ByVal srcPos As Long, ByVal dstAddr As Long, ByVal length As Long)
			Dim offset As Long = srcBaseOffset + srcPos
			Do While length > 0
				Dim size As Long = If(length > UNSAFE_COPY_THRESHOLD, UNSAFE_COPY_THRESHOLD, length)
				unsafe_Renamed.copyMemory(src, offset, Nothing, dstAddr, size)
				length -= size
				offset += size
				dstAddr += size
			Loop
		End Sub

		''' <summary>
		''' Copy from source address into given destination array.
		''' </summary>
		''' <param name="srcAddr">
		'''          source address </param>
		''' <param name="dst">
		'''          destination array </param>
		''' <param name="dstBaseOffset">
		'''          offset of first element of storage in destination array </param>
		''' <param name="dstPos">
		'''          offset within destination array of the first element to write </param>
		''' <param name="length">
		'''          number of bytes to copy </param>
		Friend Shared Sub copyToArray(ByVal srcAddr As Long, ByVal dst As Object, ByVal dstBaseOffset As Long, ByVal dstPos As Long, ByVal length As Long)
			Dim offset As Long = dstBaseOffset + dstPos
			Do While length > 0
				Dim size As Long = If(length > UNSAFE_COPY_THRESHOLD, UNSAFE_COPY_THRESHOLD, length)
				unsafe_Renamed.copyMemory(Nothing, srcAddr, dst, offset, size)
				length -= size
				srcAddr += size
				offset += size
			Loop
		End Sub

		Friend Shared Sub copyFromCharArray(ByVal src As Object, ByVal srcPos As Long, ByVal dstAddr As Long, ByVal length As Long)
			copyFromShortArray(src, srcPos, dstAddr, length)
		End Sub

		Friend Shared Sub copyToCharArray(ByVal srcAddr As Long, ByVal dst As Object, ByVal dstPos As Long, ByVal length As Long)
			copyToShortArray(srcAddr, dst, dstPos, length)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Sub copyFromShortArray(ByVal src As Object, ByVal srcPos As Long, ByVal dstAddr As Long, ByVal length As Long)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Sub copyToShortArray(ByVal srcAddr As Long, ByVal dst As Object, ByVal dstPos As Long, ByVal length As Long)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Sub copyFromIntArray(ByVal src As Object, ByVal srcPos As Long, ByVal dstAddr As Long, ByVal length As Long)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Sub copyToIntArray(ByVal srcAddr As Long, ByVal dst As Object, ByVal dstPos As Long, ByVal length As Long)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Sub copyFromLongArray(ByVal src As Object, ByVal srcPos As Long, ByVal dstAddr As Long, ByVal length As Long)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Sub copyToLongArray(ByVal srcAddr As Long, ByVal dst As Object, ByVal dstPos As Long, ByVal length As Long)
		End Sub

	End Class

End Namespace