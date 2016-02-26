Imports Microsoft.VisualBasic

'
' * Copyright (c) 2000, 2007, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.imageio.stream


	''' <summary>
	''' An abstract class implementing the <code>ImageOutputStream</code> interface.
	''' This class is designed to reduce the number of methods that must
	''' be implemented by subclasses.
	''' 
	''' </summary>
	Public MustInherit Class ImageOutputStreamImpl
		Inherits ImageInputStreamImpl
		Implements ImageOutputStream

		''' <summary>
		''' Constructs an <code>ImageOutputStreamImpl</code>.
		''' </summary>
		Public Sub New()
		End Sub

		Public MustOverride Sub write(ByVal b As Integer) Implements ImageOutputStream.write

		Public Overridable Sub write(ByVal b As SByte()) Implements ImageOutputStream.write
			write(b, 0, b.Length)
		End Sub

		Public MustOverride Sub write(SByte ByVal As b(), ByVal [off] As Integer, ByVal len As Integer) Implements ImageOutputStream.write

		Public Overridable Sub writeBoolean(ByVal v As Boolean) Implements ImageOutputStream.writeBoolean
			write(If(v, 1, 0))
		End Sub

		Public Overridable Sub writeByte(ByVal v As Integer) Implements ImageOutputStream.writeByte
			write(v)
		End Sub

		Public Overridable Sub writeShort(ByVal v As Integer) Implements ImageOutputStream.writeShort
			If byteOrder Is java.nio.ByteOrder.BIG_ENDIAN Then
				byteBuf(0) = CByte(CInt(CUInt(v) >> 8))
				byteBuf(1) = CByte(CInt(CUInt(v) >> 0))
			Else
				byteBuf(0) = CByte(CInt(CUInt(v) >> 0))
				byteBuf(1) = CByte(CInt(CUInt(v) >> 8))
			End If
			write(byteBuf, 0, 2)
		End Sub

		Public Overridable Sub writeChar(ByVal v As Integer) Implements ImageOutputStream.writeChar
			writeShort(v)
		End Sub

		Public Overridable Sub writeInt(ByVal v As Integer) Implements ImageOutputStream.writeInt
			If byteOrder Is java.nio.ByteOrder.BIG_ENDIAN Then
				byteBuf(0) = CByte(CInt(CUInt(v) >> 24))
				byteBuf(1) = CByte(CInt(CUInt(v) >> 16))
				byteBuf(2) = CByte(CInt(CUInt(v) >> 8))
				byteBuf(3) = CByte(CInt(CUInt(v) >> 0))
			Else
				byteBuf(0) = CByte(CInt(CUInt(v) >> 0))
				byteBuf(1) = CByte(CInt(CUInt(v) >> 8))
				byteBuf(2) = CByte(CInt(CUInt(v) >> 16))
				byteBuf(3) = CByte(CInt(CUInt(v) >> 24))
			End If
			write(byteBuf, 0, 4)
		End Sub

		Public Overridable Sub writeLong(ByVal v As Long) Implements ImageOutputStream.writeLong
			If byteOrder Is java.nio.ByteOrder.BIG_ENDIAN Then
				byteBuf(0) = CByte(CLng(CULng(v) >> 56))
				byteBuf(1) = CByte(CLng(CULng(v) >> 48))
				byteBuf(2) = CByte(CLng(CULng(v) >> 40))
				byteBuf(3) = CByte(CLng(CULng(v) >> 32))
				byteBuf(4) = CByte(CLng(CULng(v) >> 24))
				byteBuf(5) = CByte(CLng(CULng(v) >> 16))
				byteBuf(6) = CByte(CLng(CULng(v) >> 8))
				byteBuf(7) = CByte(CLng(CULng(v) >> 0))
			Else
				byteBuf(0) = CByte(CLng(CULng(v) >> 0))
				byteBuf(1) = CByte(CLng(CULng(v) >> 8))
				byteBuf(2) = CByte(CLng(CULng(v) >> 16))
				byteBuf(3) = CByte(CLng(CULng(v) >> 24))
				byteBuf(4) = CByte(CLng(CULng(v) >> 32))
				byteBuf(5) = CByte(CLng(CULng(v) >> 40))
				byteBuf(6) = CByte(CLng(CULng(v) >> 48))
				byteBuf(7) = CByte(CLng(CULng(v) >> 56))
			End If
			' REMIND: Once 6277756 is fixed, we should do a bulk write of all 8
			' bytes here as we do in writeShort() and writeInt() for even better
			' performance.  For now, two bulk writes of 4 bytes each is still
			' faster than 8 individual write() calls (see 6347575 for details).
			write(byteBuf, 0, 4)
			write(byteBuf, 4, 4)
		End Sub

		Public Overridable Sub writeFloat(ByVal v As Single) Implements ImageOutputStream.writeFloat
			writeInt(Single.floatToIntBits(v))
		End Sub

		Public Overridable Sub writeDouble(ByVal v As Double) Implements ImageOutputStream.writeDouble
			writeLong(Double.doubleToLongBits(v))
		End Sub

		Public Overridable Sub writeBytes(ByVal s As String) Implements ImageOutputStream.writeBytes
			Dim len As Integer = s.Length
			For i As Integer = 0 To len - 1
				write(AscW(s.Chars(i)))
			Next i
		End Sub

		Public Overridable Sub writeChars(ByVal s As String) Implements ImageOutputStream.writeChars
			Dim len As Integer = s.Length

			Dim b As SByte() = New SByte(len*2 - 1){}
			Dim boff As Integer = 0
			If byteOrder Is java.nio.ByteOrder.BIG_ENDIAN Then
				For i As Integer = 0 To len - 1
					Dim v As Integer = AscW(s.Chars(i))
					b(boff) = CByte(CInt(CUInt(v) >> 8))
					boff += 1
					b(boff) = CByte(CInt(CUInt(v) >> 0))
					boff += 1
				Next i
			Else
				For i As Integer = 0 To len - 1
					Dim v As Integer = AscW(s.Chars(i))
					b(boff) = CByte(CInt(CUInt(v) >> 0))
					boff += 1
					b(boff) = CByte(CInt(CUInt(v) >> 8))
					boff += 1
				Next i
			End If

			write(b, 0, len*2)
		End Sub

		Public Overridable Sub writeUTF(ByVal s As String) Implements ImageOutputStream.writeUTF
			Dim strlen As Integer = s.Length
			Dim utflen As Integer = 0
			Dim charr As Char() = New Char(strlen - 1){}
			Dim c As Integer, boff As Integer = 0

			s.getChars(0, strlen, charr, 0)

			For i As Integer = 0 To strlen - 1
				c = AscW(charr(i))
				If (c >= &H1) AndAlso (c <= &H7F) Then
					utflen += 1
				ElseIf c > &H7FF Then
					utflen += 3
				Else
					utflen += 2
				End If
			Next i

			If utflen > 65535 Then Throw New java.io.UTFDataFormatException("utflen > 65536!")

			Dim b As SByte() = New SByte(utflen+2 - 1){}
			b(boff) = CByte((CInt(CUInt(utflen) >> 8)) And &HFF)
			boff += 1
			b(boff) = CByte((CInt(CUInt(utflen) >> 0)) And &HFF)
			boff += 1
			For i As Integer = 0 To strlen - 1
				c = AscW(charr(i))
				If (c >= &H1) AndAlso (c <= &H7F) Then
					b(boff) = CByte(c)
					boff += 1
				ElseIf c > &H7FF Then
					b(boff) = CByte(&HE0 Or ((c >> 12) And &HF))
					boff += 1
					b(boff) = CByte(&H80 Or ((c >> 6) And &H3F))
					boff += 1
					b(boff) = CByte(&H80 Or ((c >> 0) And &H3F))
					boff += 1
				Else
					b(boff) = CByte(&HC0 Or ((c >> 6) And &H1F))
					boff += 1
					b(boff) = CByte(&H80 Or ((c >> 0) And &H3F))
					boff += 1
				End If
			Next i
			write(b, 0, utflen + 2)
		End Sub

		Public Overridable Sub writeShorts(ByVal s As Short(), ByVal [off] As Integer, ByVal len As Integer) Implements ImageOutputStream.writeShorts
			' Fix 4430357 - if off + len < 0, overflow occurred
			If [off] < 0 OrElse len < 0 OrElse [off] + len > s.Length OrElse [off] + len < 0 Then Throw New System.IndexOutOfRangeException("off < 0 || len < 0 || off + len > s.length!")

			Dim b As SByte() = New SByte(len*2 - 1){}
			Dim boff As Integer = 0
			If byteOrder Is java.nio.ByteOrder.BIG_ENDIAN Then
				For i As Integer = 0 To len - 1
					Dim v As Short = s([off] + i)
					b(boff) = CByte(CShort(CUShort(v) >> 8))
					boff += 1
					b(boff) = CByte(CShort(CUShort(v) >> 0))
					boff += 1
				Next i
			Else
				For i As Integer = 0 To len - 1
					Dim v As Short = s([off] + i)
					b(boff) = CByte(CShort(CUShort(v) >> 0))
					boff += 1
					b(boff) = CByte(CShort(CUShort(v) >> 8))
					boff += 1
				Next i
			End If

			write(b, 0, len*2)
		End Sub

		Public Overridable Sub writeChars(ByVal c As Char(), ByVal [off] As Integer, ByVal len As Integer) Implements ImageOutputStream.writeChars
			' Fix 4430357 - if off + len < 0, overflow occurred
			If [off] < 0 OrElse len < 0 OrElse [off] + len > c.Length OrElse [off] + len < 0 Then Throw New System.IndexOutOfRangeException("off < 0 || len < 0 || off + len > c.length!")

			Dim b As SByte() = New SByte(len*2 - 1){}
			Dim boff As Integer = 0
			If byteOrder Is java.nio.ByteOrder.BIG_ENDIAN Then
				For i As Integer = 0 To len - 1
					Dim v As Char = c([off] + i)
					b(boff) = CByte(CInt(CUInt(v) >> 8))
					boff += 1
					b(boff) = CByte(CInt(CUInt(v) >> 0))
					boff += 1
				Next i
			Else
				For i As Integer = 0 To len - 1
					Dim v As Char = c([off] + i)
					b(boff) = CByte(CInt(CUInt(v) >> 0))
					boff += 1
					b(boff) = CByte(CInt(CUInt(v) >> 8))
					boff += 1
				Next i
			End If

			write(b, 0, len*2)
		End Sub

		Public Overridable Sub writeInts(ByVal i As Integer(), ByVal [off] As Integer, ByVal len As Integer) Implements ImageOutputStream.writeInts
			' Fix 4430357 - if off + len < 0, overflow occurred
			If [off] < 0 OrElse len < 0 OrElse [off] + len > i.Length OrElse [off] + len < 0 Then Throw New System.IndexOutOfRangeException("off < 0 || len < 0 || off + len > i.length!")

			Dim b As SByte() = New SByte(len*4 - 1){}
			Dim boff As Integer = 0
			If byteOrder Is java.nio.ByteOrder.BIG_ENDIAN Then
				For j As Integer = 0 To len - 1
					Dim v As Integer = i([off] + j)
					b(boff) = CByte(CInt(CUInt(v) >> 24))
					boff += 1
					b(boff) = CByte(CInt(CUInt(v) >> 16))
					boff += 1
					b(boff) = CByte(CInt(CUInt(v) >> 8))
					boff += 1
					b(boff) = CByte(CInt(CUInt(v) >> 0))
					boff += 1
				Next j
			Else
				For j As Integer = 0 To len - 1
					Dim v As Integer = i([off] + j)
					b(boff) = CByte(CInt(CUInt(v) >> 0))
					boff += 1
					b(boff) = CByte(CInt(CUInt(v) >> 8))
					boff += 1
					b(boff) = CByte(CInt(CUInt(v) >> 16))
					boff += 1
					b(boff) = CByte(CInt(CUInt(v) >> 24))
					boff += 1
				Next j
			End If

			write(b, 0, len*4)
		End Sub

		Public Overridable Sub writeLongs(ByVal l As Long(), ByVal [off] As Integer, ByVal len As Integer) Implements ImageOutputStream.writeLongs
			' Fix 4430357 - if off + len < 0, overflow occurred
			If [off] < 0 OrElse len < 0 OrElse [off] + len > l.Length OrElse [off] + len < 0 Then Throw New System.IndexOutOfRangeException("off < 0 || len < 0 || off + len > l.length!")

			Dim b As SByte() = New SByte(len*8 - 1){}
			Dim boff As Integer = 0
			If byteOrder Is java.nio.ByteOrder.BIG_ENDIAN Then
				For i As Integer = 0 To len - 1
					Dim v As Long = l([off] + i)
					b(boff) = CByte(CLng(CULng(v) >> 56))
					boff += 1
					b(boff) = CByte(CLng(CULng(v) >> 48))
					boff += 1
					b(boff) = CByte(CLng(CULng(v) >> 40))
					boff += 1
					b(boff) = CByte(CLng(CULng(v) >> 32))
					boff += 1
					b(boff) = CByte(CLng(CULng(v) >> 24))
					boff += 1
					b(boff) = CByte(CLng(CULng(v) >> 16))
					boff += 1
					b(boff) = CByte(CLng(CULng(v) >> 8))
					boff += 1
					b(boff) = CByte(CLng(CULng(v) >> 0))
					boff += 1
				Next i
			Else
				For i As Integer = 0 To len - 1
					Dim v As Long = l([off] + i)
					b(boff) = CByte(CLng(CULng(v) >> 0))
					boff += 1
					b(boff) = CByte(CLng(CULng(v) >> 8))
					boff += 1
					b(boff) = CByte(CLng(CULng(v) >> 16))
					boff += 1
					b(boff) = CByte(CLng(CULng(v) >> 24))
					boff += 1
					b(boff) = CByte(CLng(CULng(v) >> 32))
					boff += 1
					b(boff) = CByte(CLng(CULng(v) >> 40))
					boff += 1
					b(boff) = CByte(CLng(CULng(v) >> 48))
					boff += 1
					b(boff) = CByte(CLng(CULng(v) >> 56))
					boff += 1
				Next i
			End If

			write(b, 0, len*8)
		End Sub

		Public Overridable Sub writeFloats(ByVal f As Single(), ByVal [off] As Integer, ByVal len As Integer) Implements ImageOutputStream.writeFloats
			' Fix 4430357 - if off + len < 0, overflow occurred
			If [off] < 0 OrElse len < 0 OrElse [off] + len > f.Length OrElse [off] + len < 0 Then Throw New System.IndexOutOfRangeException("off < 0 || len < 0 || off + len > f.length!")

			Dim b As SByte() = New SByte(len*4 - 1){}
			Dim boff As Integer = 0
			If byteOrder Is java.nio.ByteOrder.BIG_ENDIAN Then
				For i As Integer = 0 To len - 1
					Dim v As Integer = Single.floatToIntBits(f([off] + i))
					b(boff) = CByte(CInt(CUInt(v) >> 24))
					boff += 1
					b(boff) = CByte(CInt(CUInt(v) >> 16))
					boff += 1
					b(boff) = CByte(CInt(CUInt(v) >> 8))
					boff += 1
					b(boff) = CByte(CInt(CUInt(v) >> 0))
					boff += 1
				Next i
			Else
				For i As Integer = 0 To len - 1
					Dim v As Integer = Single.floatToIntBits(f([off] + i))
					b(boff) = CByte(CInt(CUInt(v) >> 0))
					boff += 1
					b(boff) = CByte(CInt(CUInt(v) >> 8))
					boff += 1
					b(boff) = CByte(CInt(CUInt(v) >> 16))
					boff += 1
					b(boff) = CByte(CInt(CUInt(v) >> 24))
					boff += 1
				Next i
			End If

			write(b, 0, len*4)
		End Sub

		Public Overridable Sub writeDoubles(ByVal d As Double(), ByVal [off] As Integer, ByVal len As Integer) Implements ImageOutputStream.writeDoubles
			' Fix 4430357 - if off + len < 0, overflow occurred
			If [off] < 0 OrElse len < 0 OrElse [off] + len > d.Length OrElse [off] + len < 0 Then Throw New System.IndexOutOfRangeException("off < 0 || len < 0 || off + len > d.length!")

			Dim b As SByte() = New SByte(len*8 - 1){}
			Dim boff As Integer = 0
			If byteOrder Is java.nio.ByteOrder.BIG_ENDIAN Then
				For i As Integer = 0 To len - 1
					Dim v As Long = Double.doubleToLongBits(d([off] + i))
					b(boff) = CByte(CLng(CULng(v) >> 56))
					boff += 1
					b(boff) = CByte(CLng(CULng(v) >> 48))
					boff += 1
					b(boff) = CByte(CLng(CULng(v) >> 40))
					boff += 1
					b(boff) = CByte(CLng(CULng(v) >> 32))
					boff += 1
					b(boff) = CByte(CLng(CULng(v) >> 24))
					boff += 1
					b(boff) = CByte(CLng(CULng(v) >> 16))
					boff += 1
					b(boff) = CByte(CLng(CULng(v) >> 8))
					boff += 1
					b(boff) = CByte(CLng(CULng(v) >> 0))
					boff += 1
				Next i
			Else
				For i As Integer = 0 To len - 1
					Dim v As Long = Double.doubleToLongBits(d([off] + i))
					b(boff) = CByte(CLng(CULng(v) >> 0))
					boff += 1
					b(boff) = CByte(CLng(CULng(v) >> 8))
					boff += 1
					b(boff) = CByte(CLng(CULng(v) >> 16))
					boff += 1
					b(boff) = CByte(CLng(CULng(v) >> 24))
					boff += 1
					b(boff) = CByte(CLng(CULng(v) >> 32))
					boff += 1
					b(boff) = CByte(CLng(CULng(v) >> 40))
					boff += 1
					b(boff) = CByte(CLng(CULng(v) >> 48))
					boff += 1
					b(boff) = CByte(CLng(CULng(v) >> 56))
					boff += 1
				Next i
			End If

			write(b, 0, len*8)
		End Sub

		Public Overridable Sub writeBit(ByVal bit As Integer) Implements ImageOutputStream.writeBit
			writeBits((1L And bit), 1)
		End Sub

		Public Overridable Sub writeBits(ByVal bits As Long, ByVal numBits As Integer) Implements ImageOutputStream.writeBits
			checkClosed()

			If numBits < 0 OrElse numBits > 64 Then Throw New System.ArgumentException("Bad value for numBits!")
			If numBits = 0 Then Return

			' Prologue: deal with pre-existing bits

			' Bug 4499158, 4507868 - if we're at the beginning of the stream
			' and the bit offset is 0, there can't be any pre-existing bits
			If (streamPosition > 0) OrElse (bitOffset > 0) Then
				Dim offset As Integer = bitOffset ' read() will reset bitOffset
				Dim partialByte As Integer = read()
				If partialByte <> -1 Then
					seek(streamPosition - 1)
				Else
					partialByte = 0
				End If

				If numBits + offset < 8 Then
					' Notch out the partial byte and drop in the new bits
					Dim shift As Integer = 8 - (offset+numBits)
					Dim mask As Integer = CInt(CUInt(-1) >> (32 - numBits))
					partialByte = partialByte And Not(mask << shift) ' Clear out old bits
					partialByte = partialByte Or ((bits And mask) << shift) ' Or in new ones
					write(partialByte)
					seek(streamPosition - 1)
					bitOffset = offset + numBits
					numBits = 0 ' Signal that we are done
				Else
					' Fill out the partial byte and reduce numBits
					Dim num As Integer = 8 - offset
					Dim mask As Integer = CInt(CUInt(-1) >> (32 - num))
					partialByte = partialByte And Not mask ' Clear out bits
					partialByte = partialByte Or ((bits >> (numBits - num)) And mask)
					' Note that bitOffset is already 0, so there is no risk
					' of this advancing to the next byte
					write(partialByte)
					numBits -= num
				End If
			End If

			' Now write any whole bytes
			If numBits > 7 Then
				Dim extra As Integer = numBits Mod 8
				For numBytes As Integer = numBits \ 8 To 1 Step -1
					Dim shift As Integer = (numBytes-1)*8+extra
					Dim value As Integer = CInt(Fix(If(shift = 0, bits And &HFF, (bits>>shift) And &HFF)))
					write(value)
				Next numBytes
				numBits = extra
			End If

			' Epilogue: write out remaining partial byte, if any
			' Note that we may be at EOF, in which case we pad with 0,
			' or not, in which case we must preserve the existing bits
			If numBits <> 0 Then
				' If we are not at the end of the file, read the current byte
				' If we are at the end of the file, initialize our byte to 0.
				Dim partialByte As Integer = 0
				partialByte = read()
				If partialByte <> -1 Then
					seek(streamPosition - 1)
				' Fix 4494976: writeBit(int) does not pad the remainder
				' of the current byte with 0s
				Else ' EOF
					partialByte = 0
				End If

				Dim shift As Integer = 8 - numBits
				Dim mask As Integer = CInt(CUInt(-1) >> (32 - numBits))
				partialByte = partialByte And Not(mask << shift)
				partialByte = partialByte Or (bits And mask) << shift
				' bitOffset is always already 0 when we get here.
				write(partialByte)
				seek(streamPosition - 1)
				bitOffset = numBits
			End If
		End Sub

		''' <summary>
		''' If the bit offset is non-zero, forces the remaining bits
		''' in the current byte to 0 and advances the stream position
		''' by one.  This method should be called by subclasses at the
		''' beginning of the <code>write(int)</code> and
		''' <code>write(byte[], int, int)</code> methods.
		''' </summary>
		''' <exception cref="IOException"> if an I/O error occurs. </exception>
		Protected Friend Sub flushBits()
			checkClosed()
			If bitOffset <> 0 Then
				Dim offset As Integer = bitOffset
				Dim partialByte As Integer = read() ' Sets bitOffset to 0
				If partialByte < 0 Then
					' Fix 4465683: When bitOffset is set
					' to something non-zero beyond EOF,
					' we should set that whole byte to
					' zero and write it to stream.
					partialByte = 0
					bitOffset = 0
				Else
					seek(streamPosition - 1)
					partialByte = partialByte And -1 << (8 - offset)
				End If
				write(partialByte)
			End If
		End Sub

	End Class

End Namespace