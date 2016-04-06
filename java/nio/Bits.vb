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

		Friend Shared Function swap(  x As Short) As Short
			Return  java.lang.[Short].reverseBytes(x)
		End Function

		Friend Shared Function swap(  x As Char) As Char
			Return Character.reverseBytes(x)
		End Function

		Friend Shared Function swap(  x As Integer) As Integer
			Return  java.lang.[Integer].reverseBytes(x)
		End Function

		Friend Shared Function swap(  x As Long) As Long
			Return java.lang.[Long].reverseBytes(x)
		End Function


		' -- get/put char --

		Private Shared Function makeChar(  b1 As SByte,   b0 As SByte) As Char
			Return CChar((b1 << 8) Or (b0 And &Hff))
		End Function

		Friend Shared Function getCharL(  bb As ByteBuffer,   bi As Integer) As Char
			Return makeChar(bb._get(bi + 1), bb._get(bi))
		End Function

		Friend Shared Function getCharL(  a As Long) As Char
			Return makeChar(_get(a + 1), _get(a))
		End Function

		Friend Shared Function getCharB(  bb As ByteBuffer,   bi As Integer) As Char
			Return makeChar(bb._get(bi), bb._get(bi + 1))
		End Function

		Friend Shared Function getCharB(  a As Long) As Char
			Return makeChar(_get(a), _get(a + 1))
		End Function

		Friend Shared Function getChar(  bb As ByteBuffer,   bi As Integer,   bigEndian As Boolean) As Char
			Return If(bigEndian, getCharB(bb, bi), getCharL(bb, bi))
		End Function

		Friend Shared Function getChar(  a As Long,   bigEndian As Boolean) As Char
			Return If(bigEndian, getCharB(a), getCharL(a))
		End Function

		Private Shared Function char1(  x As Char) As SByte
			Return CByte(AscW(x) >> 8)
		End Function
		Private Shared Function char0(  x As Char) As SByte
			Return AscW(x)
		End Function

		Friend Shared Sub putCharL(  bb As ByteBuffer,   bi As Integer,   x As Char)
			bb._put(bi, char0(x))
			bb._put(bi + 1, char1(x))
		End Sub

		Friend Shared Sub putCharL(  a As Long,   x As Char)
			_put(a, char0(x))
			_put(a + 1, char1(x))
		End Sub

		Friend Shared Sub putCharB(  bb As ByteBuffer,   bi As Integer,   x As Char)
			bb._put(bi, char1(x))
			bb._put(bi + 1, char0(x))
		End Sub

		Friend Shared Sub putCharB(  a As Long,   x As Char)
			_put(a, char1(x))
			_put(a + 1, char0(x))
		End Sub

		Friend Shared Sub putChar(  bb As ByteBuffer,   bi As Integer,   x As Char,   bigEndian As Boolean)
			If bigEndian Then
				putCharB(bb, bi, x)
			Else
				putCharL(bb, bi, x)
			End If
		End Sub

		Friend Shared Sub putChar(  a As Long,   x As Char,   bigEndian As Boolean)
			If bigEndian Then
				putCharB(a, x)
			Else
				putCharL(a, x)
			End If
		End Sub


		' -- get/put short --

		Private Shared Function makeShort(  b1 As SByte,   b0 As SByte) As Short
			Return CShort(Fix((b1 << 8) Or (b0 And &Hff)))
		End Function

		Friend Shared Function getShortL(  bb As ByteBuffer,   bi As Integer) As Short
			Return makeShort(bb._get(bi + 1), bb._get(bi))
		End Function

		Friend Shared Function getShortL(  a As Long) As Short
			Return makeShort(_get(a + 1), _get(a))
		End Function

		Friend Shared Function getShortB(  bb As ByteBuffer,   bi As Integer) As Short
			Return makeShort(bb._get(bi), bb._get(bi + 1))
		End Function

		Friend Shared Function getShortB(  a As Long) As Short
			Return makeShort(_get(a), _get(a + 1))
		End Function

		Friend Shared Function getShort(  bb As ByteBuffer,   bi As Integer,   bigEndian As Boolean) As Short
			Return If(bigEndian, getShortB(bb, bi), getShortL(bb, bi))
		End Function

		Friend Shared Function getShort(  a As Long,   bigEndian As Boolean) As Short
			Return If(bigEndian, getShortB(a), getShortL(a))
		End Function

		Private Shared Function short1(  x As Short) As SByte
			Return CByte(x >> 8)
		End Function
		Private Shared Function short0(  x As Short) As SByte
			Return CByte(x)
		End Function

		Friend Shared Sub putShortL(  bb As ByteBuffer,   bi As Integer,   x As Short)
			bb._put(bi, short0(x))
			bb._put(bi + 1, short1(x))
		End Sub

		Friend Shared Sub putShortL(  a As Long,   x As Short)
			_put(a, short0(x))
			_put(a + 1, short1(x))
		End Sub

		Friend Shared Sub putShortB(  bb As ByteBuffer,   bi As Integer,   x As Short)
			bb._put(bi, short1(x))
			bb._put(bi + 1, short0(x))
		End Sub

		Friend Shared Sub putShortB(  a As Long,   x As Short)
			_put(a, short1(x))
			_put(a + 1, short0(x))
		End Sub

		Friend Shared Sub putShort(  bb As ByteBuffer,   bi As Integer,   x As Short,   bigEndian As Boolean)
			If bigEndian Then
				putShortB(bb, bi, x)
			Else
				putShortL(bb, bi, x)
			End If
		End Sub

		Friend Shared Sub putShort(  a As Long,   x As Short,   bigEndian As Boolean)
			If bigEndian Then
				putShortB(a, x)
			Else
				putShortL(a, x)
			End If
		End Sub


		' -- get/put int --

		Private Shared Function makeInt(  b3 As SByte,   b2 As SByte,   b1 As SByte,   b0 As SByte) As Integer
			Return (((b3) << 24) Or ((b2 And &Hff) << 16) Or ((b1 And &Hff) << 8) Or ((b0 And &Hff)))
		End Function

		Friend Shared Function getIntL(  bb As ByteBuffer,   bi As Integer) As Integer
			Return makeInt(bb._get(bi + 3), bb._get(bi + 2), bb._get(bi + 1), bb._get(bi))
		End Function

		Friend Shared Function getIntL(  a As Long) As Integer
			Return makeInt(_get(a + 3), _get(a + 2), _get(a + 1), _get(a))
		End Function

		Friend Shared Function getIntB(  bb As ByteBuffer,   bi As Integer) As Integer
			Return makeInt(bb._get(bi), bb._get(bi + 1), bb._get(bi + 2), bb._get(bi + 3))
		End Function

		Friend Shared Function getIntB(  a As Long) As Integer
			Return makeInt(_get(a), _get(a + 1), _get(a + 2), _get(a + 3))
		End Function

		Friend Shared Function getInt(  bb As ByteBuffer,   bi As Integer,   bigEndian As Boolean) As Integer
			Return If(bigEndian, getIntB(bb, bi), getIntL(bb, bi))
		End Function

		Friend Shared Function getInt(  a As Long,   bigEndian As Boolean) As Integer
			Return If(bigEndian, getIntB(a), getIntL(a))
		End Function

		Private Shared Function int3(  x As Integer) As SByte
			Return CByte(x >> 24)
		End Function
		Private Shared Function int2(  x As Integer) As SByte
			Return CByte(x >> 16)
		End Function
		Private Shared Function int1(  x As Integer) As SByte
			Return CByte(x >> 8)
		End Function
		Private Shared Function int0(  x As Integer) As SByte
			Return CByte(x)
		End Function

		Friend Shared Sub putIntL(  bb As ByteBuffer,   bi As Integer,   x As Integer)
			bb._put(bi + 3, int3(x))
			bb._put(bi + 2, int2(x))
			bb._put(bi + 1, int1(x))
			bb._put(bi, int0(x))
		End Sub

		Friend Shared Sub putIntL(  a As Long,   x As Integer)
			_put(a + 3, int3(x))
			_put(a + 2, int2(x))
			_put(a + 1, int1(x))
			_put(a, int0(x))
		End Sub

		Friend Shared Sub putIntB(  bb As ByteBuffer,   bi As Integer,   x As Integer)
			bb._put(bi, int3(x))
			bb._put(bi + 1, int2(x))
			bb._put(bi + 2, int1(x))
			bb._put(bi + 3, int0(x))
		End Sub

		Friend Shared Sub putIntB(  a As Long,   x As Integer)
			_put(a, int3(x))
			_put(a + 1, int2(x))
			_put(a + 2, int1(x))
			_put(a + 3, int0(x))
		End Sub

		Friend Shared Sub putInt(  bb As ByteBuffer,   bi As Integer,   x As Integer,   bigEndian As Boolean)
			If bigEndian Then
				putIntB(bb, bi, x)
			Else
				putIntL(bb, bi, x)
			End If
		End Sub

		Friend Shared Sub putInt(  a As Long,   x As Integer,   bigEndian As Boolean)
			If bigEndian Then
				putIntB(a, x)
			Else
				putIntL(a, x)
			End If
		End Sub


		' -- get/put long --

		Private Shared Function makeLong(  b7 As SByte,   b6 As SByte,   b5 As SByte,   b4 As SByte,   b3 As SByte,   b2 As SByte,   b1 As SByte,   b0 As SByte) As Long
			Return (((CLng(b7)) << 56) Or ((CLng(b6) And &Hff) << 48) Or ((CLng(b5) And &Hff) << 40) Or ((CLng(b4) And &Hff) << 32) Or ((CLng(b3) And &Hff) << 24) Or ((CLng(b2) And &Hff) << 16) Or ((CLng(b1) And &Hff) << 8) Or ((CLng(b0) And &Hff)))
		End Function

		Friend Shared Function getLongL(  bb As ByteBuffer,   bi As Integer) As Long
			Return makeLong(bb._get(bi + 7), bb._get(bi + 6), bb._get(bi + 5), bb._get(bi + 4), bb._get(bi + 3), bb._get(bi + 2), bb._get(bi + 1), bb._get(bi))
		End Function

		Friend Shared Function getLongL(  a As Long) As Long
			Return makeLong(_get(a + 7), _get(a + 6), _get(a + 5), _get(a + 4), _get(a + 3), _get(a + 2), _get(a + 1), _get(a))
		End Function

		Friend Shared Function getLongB(  bb As ByteBuffer,   bi As Integer) As Long
			Return makeLong(bb._get(bi), bb._get(bi + 1), bb._get(bi + 2), bb._get(bi + 3), bb._get(bi + 4), bb._get(bi + 5), bb._get(bi + 6), bb._get(bi + 7))
		End Function

		Friend Shared Function getLongB(  a As Long) As Long
			Return makeLong(_get(a), _get(a + 1), _get(a + 2), _get(a + 3), _get(a + 4), _get(a + 5), _get(a + 6), _get(a + 7))
		End Function

		Friend Shared Function getLong(  bb As ByteBuffer,   bi As Integer,   bigEndian As Boolean) As Long
			Return If(bigEndian, getLongB(bb, bi), getLongL(bb, bi))
		End Function

		Friend Shared Function getLong(  a As Long,   bigEndian As Boolean) As Long
			Return If(bigEndian, getLongB(a), getLongL(a))
		End Function

		Private Shared Function long7(  x As Long) As SByte
			Return CByte(x >> 56)
		End Function
		Private Shared Function long6(  x As Long) As SByte
			Return CByte(x >> 48)
		End Function
		Private Shared Function long5(  x As Long) As SByte
			Return CByte(x >> 40)
		End Function
		Private Shared Function long4(  x As Long) As SByte
			Return CByte(x >> 32)
		End Function
		Private Shared Function long3(  x As Long) As SByte
			Return CByte(x >> 24)
		End Function
		Private Shared Function long2(  x As Long) As SByte
			Return CByte(x >> 16)
		End Function
		Private Shared Function long1(  x As Long) As SByte
			Return CByte(x >> 8)
		End Function
		Private Shared Function long0(  x As Long) As SByte
			Return CByte(x)
		End Function

		Friend Shared Sub putLongL(  bb As ByteBuffer,   bi As Integer,   x As Long)
			bb._put(bi + 7, long7(x))
			bb._put(bi + 6, long6(x))
			bb._put(bi + 5, long5(x))
			bb._put(bi + 4, long4(x))
			bb._put(bi + 3, long3(x))
			bb._put(bi + 2, long2(x))
			bb._put(bi + 1, long1(x))
			bb._put(bi, long0(x))
		End Sub

		Friend Shared Sub putLongL(  a As Long,   x As Long)
			_put(a + 7, long7(x))
			_put(a + 6, long6(x))
			_put(a + 5, long5(x))
			_put(a + 4, long4(x))
			_put(a + 3, long3(x))
			_put(a + 2, long2(x))
			_put(a + 1, long1(x))
			_put(a, long0(x))
		End Sub

		Friend Shared Sub putLongB(  bb As ByteBuffer,   bi As Integer,   x As Long)
			bb._put(bi, long7(x))
			bb._put(bi + 1, long6(x))
			bb._put(bi + 2, long5(x))
			bb._put(bi + 3, long4(x))
			bb._put(bi + 4, long3(x))
			bb._put(bi + 5, long2(x))
			bb._put(bi + 6, long1(x))
			bb._put(bi + 7, long0(x))
		End Sub

		Friend Shared Sub putLongB(  a As Long,   x As Long)
			_put(a, long7(x))
			_put(a + 1, long6(x))
			_put(a + 2, long5(x))
			_put(a + 3, long4(x))
			_put(a + 4, long3(x))
			_put(a + 5, long2(x))
			_put(a + 6, long1(x))
			_put(a + 7, long0(x))
		End Sub

		Friend Shared Sub putLong(  bb As ByteBuffer,   bi As Integer,   x As Long,   bigEndian As Boolean)
			If bigEndian Then
				putLongB(bb, bi, x)
			Else
				putLongL(bb, bi, x)
			End If
		End Sub

		Friend Shared Sub putLong(  a As Long,   x As Long,   bigEndian As Boolean)
			If bigEndian Then
				putLongB(a, x)
			Else
				putLongL(a, x)
			End If
		End Sub


		' -- get/put float --

		Friend Shared Function getFloatL(  bb As ByteBuffer,   bi As Integer) As Single
			Return Float.intBitsToFloat(getIntL(bb, bi))
		End Function

		Friend Shared Function getFloatL(  a As Long) As Single
			Return Float.intBitsToFloat(getIntL(a))
		End Function

		Friend Shared Function getFloatB(  bb As ByteBuffer,   bi As Integer) As Single
			Return Float.intBitsToFloat(getIntB(bb, bi))
		End Function

		Friend Shared Function getFloatB(  a As Long) As Single
			Return Float.intBitsToFloat(getIntB(a))
		End Function

		Friend Shared Function getFloat(  bb As ByteBuffer,   bi As Integer,   bigEndian As Boolean) As Single
			Return If(bigEndian, getFloatB(bb, bi), getFloatL(bb, bi))
		End Function

		Friend Shared Function getFloat(  a As Long,   bigEndian As Boolean) As Single
			Return If(bigEndian, getFloatB(a), getFloatL(a))
		End Function

		Friend Shared Sub putFloatL(  bb As ByteBuffer,   bi As Integer,   x As Single)
			putIntL(bb, bi, Float.floatToRawIntBits(x))
		End Sub

		Friend Shared Sub putFloatL(  a As Long,   x As Single)
			putIntL(a, Float.floatToRawIntBits(x))
		End Sub

		Friend Shared Sub putFloatB(  bb As ByteBuffer,   bi As Integer,   x As Single)
			putIntB(bb, bi, Float.floatToRawIntBits(x))
		End Sub

		Friend Shared Sub putFloatB(  a As Long,   x As Single)
			putIntB(a, Float.floatToRawIntBits(x))
		End Sub

		Friend Shared Sub putFloat(  bb As ByteBuffer,   bi As Integer,   x As Single,   bigEndian As Boolean)
			If bigEndian Then
				putFloatB(bb, bi, x)
			Else
				putFloatL(bb, bi, x)
			End If
		End Sub

		Friend Shared Sub putFloat(  a As Long,   x As Single,   bigEndian As Boolean)
			If bigEndian Then
				putFloatB(a, x)
			Else
				putFloatL(a, x)
			End If
		End Sub


		' -- get/put double --

		Friend Shared Function getDoubleL(  bb As ByteBuffer,   bi As Integer) As Double
			Return java.lang.[Double].longBitsToDouble(getLongL(bb, bi))
		End Function

		Friend Shared Function getDoubleL(  a As Long) As Double
			Return java.lang.[Double].longBitsToDouble(getLongL(a))
		End Function

		Friend Shared Function getDoubleB(  bb As ByteBuffer,   bi As Integer) As Double
			Return java.lang.[Double].longBitsToDouble(getLongB(bb, bi))
		End Function

		Friend Shared Function getDoubleB(  a As Long) As Double
			Return java.lang.[Double].longBitsToDouble(getLongB(a))
		End Function

		Friend Shared Function getDouble(  bb As ByteBuffer,   bi As Integer,   bigEndian As Boolean) As Double
			Return If(bigEndian, getDoubleB(bb, bi), getDoubleL(bb, bi))
		End Function

		Friend Shared Function getDouble(  a As Long,   bigEndian As Boolean) As Double
			Return If(bigEndian, getDoubleB(a), getDoubleL(a))
		End Function

		Friend Shared Sub putDoubleL(  bb As ByteBuffer,   bi As Integer,   x As Double)
			putLongL(bb, bi, java.lang.[Double].doubleToRawLongBits(x))
		End Sub

		Friend Shared Sub putDoubleL(  a As Long,   x As Double)
			putLongL(a, java.lang.[Double].doubleToRawLongBits(x))
		End Sub

		Friend Shared Sub putDoubleB(  bb As ByteBuffer,   bi As Integer,   x As Double)
			putLongB(bb, bi, java.lang.[Double].doubleToRawLongBits(x))
		End Sub

		Friend Shared Sub putDoubleB(  a As Long,   x As Double)
			putLongB(a, java.lang.[Double].doubleToRawLongBits(x))
		End Sub

		Friend Shared Sub putDouble(  bb As ByteBuffer,   bi As Integer,   x As Double,   bigEndian As Boolean)
			If bigEndian Then
				putDoubleB(bb, bi, x)
			Else
				putDoubleL(bb, bi, x)
			End If
		End Sub

		Friend Shared Sub putDouble(  a As Long,   x As Double,   bigEndian As Boolean)
			If bigEndian Then
				putDoubleB(a, x)
			Else
				putDoubleL(a, x)
			End If
		End Sub


		' -- Unsafe access --

		Private Shared ReadOnly unsafe_Renamed As sun.misc.Unsafe = sun.misc.Unsafe.unsafe

		Private Shared Function _get(  a As Long) As SByte
			Return unsafe_Renamed.getByte(a)
		End Function

		Private Shared Sub _put(  a As Long,   b As SByte)
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
	'				@Override public  Sub  truncate(Buffer buf)
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

		Friend Shared Function pageCount(  size As Long) As Integer
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
		Friend Shared Sub reserveMemory(  size As Long,   cap As Integer)
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
		Friend Shared Sub unreserveMemory(  size As Long,   cap As Integer)
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
		Friend Shared Sub copyFromArray(  src As Object,   srcBaseOffset As Long,   srcPos As Long,   dstAddr As Long,   length As Long)
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
		Friend Shared Sub copyToArray(  srcAddr As Long,   dst As Object,   dstBaseOffset As Long,   dstPos As Long,   length As Long)
			Dim offset As Long = dstBaseOffset + dstPos
			Do While length > 0
				Dim size As Long = If(length > UNSAFE_COPY_THRESHOLD, UNSAFE_COPY_THRESHOLD, length)
				unsafe_Renamed.copyMemory(Nothing, srcAddr, dst, offset, size)
				length -= size
				srcAddr += size
				offset += size
			Loop
		End Sub

		Friend Shared Sub copyFromCharArray(  src As Object,   srcPos As Long,   dstAddr As Long,   length As Long)
			copyFromShortArray(src, srcPos, dstAddr, length)
		End Sub

		Friend Shared Sub copyToCharArray(  srcAddr As Long,   dst As Object,   dstPos As Long,   length As Long)
			copyToShortArray(srcAddr, dst, dstPos, length)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Sub copyFromShortArray(  src As Object,   srcPos As Long,   dstAddr As Long,   length As Long)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Sub copyToShortArray(  srcAddr As Long,   dst As Object,   dstPos As Long,   length As Long)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Sub copyFromIntArray(  src As Object,   srcPos As Long,   dstAddr As Long,   length As Long)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Sub copyToIntArray(  srcAddr As Long,   dst As Object,   dstPos As Long,   length As Long)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Sub copyFromLongArray(  src As Object,   srcPos As Long,   dstAddr As Long,   length As Long)
		End Sub
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Friend Shared Sub copyToLongArray(  srcAddr As Long,   dst As Object,   dstPos As Long,   length As Long)
		End Sub

	End Class

End Namespace