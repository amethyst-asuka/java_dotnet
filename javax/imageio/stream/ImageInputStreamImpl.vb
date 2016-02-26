Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Text

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

Namespace javax.imageio.stream


	''' <summary>
	''' An abstract class implementing the <code>ImageInputStream</code> interface.
	''' This class is designed to reduce the number of methods that must
	''' be implemented by subclasses.
	''' 
	''' <p> In particular, this class handles most or all of the details of
	''' byte order interpretation, buffering, mark/reset, discarding,
	''' closing, and disposing.
	''' </summary>
	Public MustInherit Class ImageInputStreamImpl
		Implements ImageInputStream

		Private markByteStack As New Stack

		Private markBitStack As New Stack

		Private isClosed As Boolean = False

		' Length of the buffer used for readFully(type[], int, int)
		Private Const BYTE_BUF_LENGTH As Integer = 8192

		''' <summary>
		''' Byte buffer used for readFully(type[], int, int).  Note that this
		''' array is also used for bulk reads in readShort(), readInt(), etc, so
		''' it should be large enough to hold a primitive value (i.e. >= 8 bytes).
		''' Also note that this array is package protected, so that it can be
		''' used by ImageOutputStreamImpl in a similar manner.
		''' </summary>
		Friend byteBuf As SByte() = New SByte(BYTE_BUF_LENGTH - 1){}

		''' <summary>
		''' The byte order of the stream as an instance of the enumeration
		''' class <code>java.nio.ByteOrder</code>, where
		''' <code>ByteOrder.BIG_ENDIAN</code> indicates network byte order
		''' and <code>ByteOrder.LITTLE_ENDIAN</code> indicates the reverse
		''' order.  By default, the value is
		''' <code>ByteOrder.BIG_ENDIAN</code>.
		''' </summary>
		Protected Friend byteOrder As java.nio.ByteOrder = java.nio.ByteOrder.BIG_ENDIAN

		''' <summary>
		''' The current read position within the stream.  Subclasses are
		''' responsible for keeping this value current from any method they
		''' override that alters the read position.
		''' </summary>
		Protected Friend streamPos As Long

		''' <summary>
		''' The current bit offset within the stream.  Subclasses are
		''' responsible for keeping this value current from any method they
		''' override that alters the bit offset.
		''' </summary>
		Protected Friend bitOffset As Integer

		''' <summary>
		''' The position prior to which data may be discarded.  Seeking
		''' to a smaller position is not allowed.  <code>flushedPos</code>
		''' will always be {@literal >= 0}.
		''' </summary>
		Protected Friend flushedPos As Long = 0

		''' <summary>
		''' Constructs an <code>ImageInputStreamImpl</code>.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Throws an <code>IOException</code> if the stream has been closed.
		''' Subclasses may call this method from any of their methods that
		''' require the stream not to be closed.
		''' </summary>
		''' <exception cref="IOException"> if the stream is closed. </exception>
		Protected Friend Sub checkClosed()
			If isClosed Then Throw New java.io.IOException("closed")
		End Sub

		Public Overridable Property byteOrder Implements ImageInputStream.setByteOrder As java.nio.ByteOrder
			Set(ByVal byteOrder As java.nio.ByteOrder)
				Me.byteOrder = byteOrder
			End Set
			Get
				Return byteOrder
			End Get
		End Property


		''' <summary>
		''' Reads a single byte from the stream and returns it as an
		''' <code>int</code> between 0 and 255.  If EOF is reached,
		''' <code>-1</code> is returned.
		''' 
		''' <p> Subclasses must provide an implementation for this method.
		''' The subclass implementation should update the stream position
		''' before exiting.
		''' 
		''' <p> The bit offset within the stream must be reset to zero before
		''' the read occurs.
		''' </summary>
		''' <returns> the value of the next byte in the stream, or <code>-1</code>
		''' if EOF is reached.
		''' </returns>
		''' <exception cref="IOException"> if the stream has been closed. </exception>
		Public MustOverride Function read() As Integer Implements ImageInputStream.read

		''' <summary>
		''' A convenience method that calls <code>read(b, 0, b.length)</code>.
		''' 
		''' <p> The bit offset within the stream is reset to zero before
		''' the read occurs.
		''' </summary>
		''' <returns> the number of bytes actually read, or <code>-1</code>
		''' to indicate EOF.
		''' </returns>
		''' <exception cref="NullPointerException"> if <code>b</code> is
		''' <code>null</code>. </exception>
		''' <exception cref="IOException"> if an I/O error occurs. </exception>
		Public Overridable Function read(ByVal b As SByte()) As Integer Implements ImageInputStream.read
			Return read(b, 0, b.Length)
		End Function

		''' <summary>
		''' Reads up to <code>len</code> bytes from the stream, and stores
		''' them into <code>b</code> starting at index <code>off</code>.
		''' If no bytes can be read because the end of the stream has been
		''' reached, <code>-1</code> is returned.
		''' 
		''' <p> The bit offset within the stream must be reset to zero before
		''' the read occurs.
		''' 
		''' <p> Subclasses must provide an implementation for this method.
		''' The subclass implementation should update the stream position
		''' before exiting.
		''' </summary>
		''' <param name="b"> an array of bytes to be written to. </param>
		''' <param name="off"> the starting position within <code>b</code> to write to. </param>
		''' <param name="len"> the maximum number of bytes to read.
		''' </param>
		''' <returns> the number of bytes actually read, or <code>-1</code>
		''' to indicate EOF.
		''' </returns>
		''' <exception cref="IndexOutOfBoundsException"> if <code>off</code> is
		''' negative, <code>len</code> is negative, or <code>off +
		''' len</code> is greater than <code>b.length</code>. </exception>
		''' <exception cref="NullPointerException"> if <code>b</code> is
		''' <code>null</code>. </exception>
		''' <exception cref="IOException"> if an I/O error occurs. </exception>
		Public MustOverride Function read(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer) As Integer Implements ImageInputStream.read

		Public Overridable Sub readBytes(ByVal buf As IIOByteBuffer, ByVal len As Integer) Implements ImageInputStream.readBytes
			If len < 0 Then Throw New System.IndexOutOfRangeException("len < 0!")
			If buf Is Nothing Then Throw New NullPointerException("buf == null!")

			Dim data As SByte() = New SByte(len - 1){}
			len = read(data, 0, len)

			buf.data = data
			buf.offset = 0
			buf.length = len
		End Sub

		Public Overridable Function readBoolean() As Boolean Implements ImageInputStream.readBoolean
			Dim ch As Integer = Me.read()
			If ch < 0 Then Throw New java.io.EOFException
			Return (ch <> 0)
		End Function

		Public Overridable Function readByte() As SByte Implements ImageInputStream.readByte
			Dim ch As Integer = Me.read()
			If ch < 0 Then Throw New java.io.EOFException
			Return CByte(ch)
		End Function

		Public Overridable Function readUnsignedByte() As Integer Implements ImageInputStream.readUnsignedByte
			Dim ch As Integer = Me.read()
			If ch < 0 Then Throw New java.io.EOFException
			Return ch
		End Function

		Public Overridable Function readShort() As Short Implements ImageInputStream.readShort
			If read(byteBuf, 0, 2) <> 2 Then Throw New java.io.EOFException

			If byteOrder Is java.nio.ByteOrder.BIG_ENDIAN Then
				Return CShort(Fix(((byteBuf(0) And &Hff) << 8) Or ((byteBuf(1) And &Hff) << 0)))
			Else
				Return CShort(Fix(((byteBuf(1) And &Hff) << 8) Or ((byteBuf(0) And &Hff) << 0)))
			End If
		End Function

		Public Overridable Function readUnsignedShort() As Integer Implements ImageInputStream.readUnsignedShort
			Return (CInt(readShort())) And &Hffff
		End Function

		Public Overridable Function readChar() As Char Implements ImageInputStream.readChar
			Return ChrW(readShort())
		End Function

		Public Overridable Function readInt() As Integer Implements ImageInputStream.readInt
			If read(byteBuf, 0, 4) <> 4 Then Throw New java.io.EOFException

			If byteOrder Is java.nio.ByteOrder.BIG_ENDIAN Then
				Return (((byteBuf(0) And &Hff) << 24) Or ((byteBuf(1) And &Hff) << 16) Or ((byteBuf(2) And &Hff) << 8) Or ((byteBuf(3) And &Hff) << 0))
			Else
				Return (((byteBuf(3) And &Hff) << 24) Or ((byteBuf(2) And &Hff) << 16) Or ((byteBuf(1) And &Hff) << 8) Or ((byteBuf(0) And &Hff) << 0))
			End If
		End Function

		Public Overridable Function readUnsignedInt() As Long Implements ImageInputStream.readUnsignedInt
			Return (CLng(readInt())) And &HffffffffL
		End Function

		Public Overridable Function readLong() As Long Implements ImageInputStream.readLong
			' REMIND: Once 6277756 is fixed, we should do a bulk read of all 8
			' bytes here as we do in readShort() and readInt() for even better
			' performance (see 6347575 for details).
			Dim i1 As Integer = readInt()
			Dim i2 As Integer = readInt()

			If byteOrder Is java.nio.ByteOrder.BIG_ENDIAN Then
				Return (CLng(i1) << 32) + (i2 And &HFFFFFFFFL)
			Else
				Return (CLng(i2) << 32) + (i1 And &HFFFFFFFFL)
			End If
		End Function

		Public Overridable Function readFloat() As Single Implements ImageInputStream.readFloat
			Return Single.intBitsToFloat(readInt())
		End Function

		Public Overridable Function readDouble() As Double Implements ImageInputStream.readDouble
			Return Double.longBitsToDouble(readLong())
		End Function

		Public Overridable Function readLine() As String Implements ImageInputStream.readLine
			Dim input As New StringBuilder
			Dim c As Integer = -1
			Dim eol As Boolean = False

			Do While Not eol
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Select Case c = read()
				Case -1, ControlChars.Lf
					eol = True
				Case ControlChars.Cr
					eol = True
					Dim cur As Long = streamPosition
					If (read()) <> ControlChars.Lf Then seek(cur)
				Case Else
					input.Append(ChrW(c))
				End Select
			Loop

			If (c = -1) AndAlso (input.Length = 0) Then Return Nothing
			Return input.ToString()
		End Function

		Public Overridable Function readUTF() As String Implements ImageInputStream.readUTF
			Me.bitOffset = 0

			' Fix 4494369: method ImageInputStreamImpl.readUTF()
			' does not work as specified (it should always assume
			' network byte order).
			Dim oldByteOrder As java.nio.ByteOrder = byteOrder
			byteOrder = java.nio.ByteOrder.BIG_ENDIAN

			Dim ret As String
			Try
				ret = java.io.DataInputStream.readUTF(Me)
			Catch e As java.io.IOException
				' Restore the old byte order even if an exception occurs
				byteOrder = oldByteOrder
				Throw e
			End Try

			byteOrder = oldByteOrder
			Return ret
		End Function

		Public Overridable Sub readFully(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer) Implements ImageInputStream.readFully
			' Fix 4430357 - if off + len < 0, overflow occurred
			If [off] < 0 OrElse len < 0 OrElse [off] + len > b.Length OrElse [off] + len < 0 Then Throw New System.IndexOutOfRangeException("off < 0 || len < 0 || off + len > b.length!")

			Do While len > 0
				Dim nbytes As Integer = read(b, [off], len)
				If nbytes = -1 Then Throw New java.io.EOFException
				[off] += nbytes
				len -= nbytes
			Loop
		End Sub

		Public Overridable Sub readFully(ByVal b As SByte()) Implements ImageInputStream.readFully
			readFully(b, 0, b.Length)
		End Sub

		Public Overridable Sub readFully(ByVal s As Short(), ByVal [off] As Integer, ByVal len As Integer) Implements ImageInputStream.readFully
			' Fix 4430357 - if off + len < 0, overflow occurred
			If [off] < 0 OrElse len < 0 OrElse [off] + len > s.Length OrElse [off] + len < 0 Then Throw New System.IndexOutOfRangeException("off < 0 || len < 0 || off + len > s.length!")

			Do While len > 0
				Dim nelts As Integer = Math.Min(len, byteBuf.Length\2)
				readFully(byteBuf, 0, nelts*2)
				toShorts(byteBuf, s, [off], nelts)
				[off] += nelts
				len -= nelts
			Loop
		End Sub

		Public Overridable Sub readFully(ByVal c As Char(), ByVal [off] As Integer, ByVal len As Integer) Implements ImageInputStream.readFully
			' Fix 4430357 - if off + len < 0, overflow occurred
			If [off] < 0 OrElse len < 0 OrElse [off] + len > c.Length OrElse [off] + len < 0 Then Throw New System.IndexOutOfRangeException("off < 0 || len < 0 || off + len > c.length!")

			Do While len > 0
				Dim nelts As Integer = Math.Min(len, byteBuf.Length\2)
				readFully(byteBuf, 0, nelts*2)
				toChars(byteBuf, c, [off], nelts)
				[off] += nelts
				len -= nelts
			Loop
		End Sub

		Public Overridable Sub readFully(ByVal i As Integer(), ByVal [off] As Integer, ByVal len As Integer) Implements ImageInputStream.readFully
			' Fix 4430357 - if off + len < 0, overflow occurred
			If [off] < 0 OrElse len < 0 OrElse [off] + len > i.Length OrElse [off] + len < 0 Then Throw New System.IndexOutOfRangeException("off < 0 || len < 0 || off + len > i.length!")

			Do While len > 0
				Dim nelts As Integer = Math.Min(len, byteBuf.Length\4)
				readFully(byteBuf, 0, nelts*4)
				toInts(byteBuf, i, [off], nelts)
				[off] += nelts
				len -= nelts
			Loop
		End Sub

		Public Overridable Sub readFully(ByVal l As Long(), ByVal [off] As Integer, ByVal len As Integer) Implements ImageInputStream.readFully
			' Fix 4430357 - if off + len < 0, overflow occurred
			If [off] < 0 OrElse len < 0 OrElse [off] + len > l.Length OrElse [off] + len < 0 Then Throw New System.IndexOutOfRangeException("off < 0 || len < 0 || off + len > l.length!")

			Do While len > 0
				Dim nelts As Integer = Math.Min(len, byteBuf.Length\8)
				readFully(byteBuf, 0, nelts*8)
				toLongs(byteBuf, l, [off], nelts)
				[off] += nelts
				len -= nelts
			Loop
		End Sub

		Public Overridable Sub readFully(ByVal f As Single(), ByVal [off] As Integer, ByVal len As Integer) Implements ImageInputStream.readFully
			' Fix 4430357 - if off + len < 0, overflow occurred
			If [off] < 0 OrElse len < 0 OrElse [off] + len > f.Length OrElse [off] + len < 0 Then Throw New System.IndexOutOfRangeException("off < 0 || len < 0 || off + len > f.length!")

			Do While len > 0
				Dim nelts As Integer = Math.Min(len, byteBuf.Length\4)
				readFully(byteBuf, 0, nelts*4)
				toFloats(byteBuf, f, [off], nelts)
				[off] += nelts
				len -= nelts
			Loop
		End Sub

		Public Overridable Sub readFully(ByVal d As Double(), ByVal [off] As Integer, ByVal len As Integer) Implements ImageInputStream.readFully
			' Fix 4430357 - if off + len < 0, overflow occurred
			If [off] < 0 OrElse len < 0 OrElse [off] + len > d.Length OrElse [off] + len < 0 Then Throw New System.IndexOutOfRangeException("off < 0 || len < 0 || off + len > d.length!")

			Do While len > 0
				Dim nelts As Integer = Math.Min(len, byteBuf.Length\8)
				readFully(byteBuf, 0, nelts*8)
				toDoubles(byteBuf, d, [off], nelts)
				[off] += nelts
				len -= nelts
			Loop
		End Sub

		Private Sub toShorts(ByVal b As SByte(), ByVal s As Short(), ByVal [off] As Integer, ByVal len As Integer)
			Dim boff As Integer = 0
			If byteOrder Is java.nio.ByteOrder.BIG_ENDIAN Then
				For j As Integer = 0 To len - 1
					Dim b0 As Integer = b(boff)
					Dim b1 As Integer = b(boff + 1) And &Hff
					s([off] + j) = CShort(Fix((b0 << 8) Or b1))
					boff += 2
				Next j
			Else
				For j As Integer = 0 To len - 1
					Dim b0 As Integer = b(boff + 1)
					Dim b1 As Integer = b(boff) And &Hff
					s([off] + j) = CShort(Fix((b0 << 8) Or b1))
					boff += 2
				Next j
			End If
		End Sub

		Private Sub toChars(ByVal b As SByte(), ByVal c As Char(), ByVal [off] As Integer, ByVal len As Integer)
			Dim boff As Integer = 0
			If byteOrder Is java.nio.ByteOrder.BIG_ENDIAN Then
				For j As Integer = 0 To len - 1
					Dim b0 As Integer = b(boff)
					Dim b1 As Integer = b(boff + 1) And &Hff
					c([off] + j) = CChar((b0 << 8) Or b1)
					boff += 2
				Next j
			Else
				For j As Integer = 0 To len - 1
					Dim b0 As Integer = b(boff + 1)
					Dim b1 As Integer = b(boff) And &Hff
					c([off] + j) = CChar((b0 << 8) Or b1)
					boff += 2
				Next j
			End If
		End Sub

		Private Sub toInts(ByVal b As SByte(), ByVal i As Integer(), ByVal [off] As Integer, ByVal len As Integer)
			Dim boff As Integer = 0
			If byteOrder Is java.nio.ByteOrder.BIG_ENDIAN Then
				For j As Integer = 0 To len - 1
					Dim b0 As Integer = b(boff)
					Dim b1 As Integer = b(boff + 1) And &Hff
					Dim b2 As Integer = b(boff + 2) And &Hff
					Dim b3 As Integer = b(boff + 3) And &Hff
					i([off] + j) = (b0 << 24) Or (b1 << 16) Or (b2 << 8) Or b3
					boff += 4
				Next j
			Else
				For j As Integer = 0 To len - 1
					Dim b0 As Integer = b(boff + 3)
					Dim b1 As Integer = b(boff + 2) And &Hff
					Dim b2 As Integer = b(boff + 1) And &Hff
					Dim b3 As Integer = b(boff) And &Hff
					i([off] + j) = (b0 << 24) Or (b1 << 16) Or (b2 << 8) Or b3
					boff += 4
				Next j
			End If
		End Sub

		Private Sub toLongs(ByVal b As SByte(), ByVal l As Long(), ByVal [off] As Integer, ByVal len As Integer)
			Dim boff As Integer = 0
			If byteOrder Is java.nio.ByteOrder.BIG_ENDIAN Then
				For j As Integer = 0 To len - 1
					Dim b0 As Integer = b(boff)
					Dim b1 As Integer = b(boff + 1) And &Hff
					Dim b2 As Integer = b(boff + 2) And &Hff
					Dim b3 As Integer = b(boff + 3) And &Hff
					Dim b4 As Integer = b(boff + 4)
					Dim b5 As Integer = b(boff + 5) And &Hff
					Dim b6 As Integer = b(boff + 6) And &Hff
					Dim b7 As Integer = b(boff + 7) And &Hff

					Dim i0 As Integer = (b0 << 24) Or (b1 << 16) Or (b2 << 8) Or b3
					Dim i1 As Integer = (b4 << 24) Or (b5 << 16) Or (b6 << 8) Or b7

					l([off] + j) = (CLng(i0) << 32) Or (i1 And &HffffffffL)
					boff += 8
				Next j
			Else
				For j As Integer = 0 To len - 1
					Dim b0 As Integer = b(boff + 7)
					Dim b1 As Integer = b(boff + 6) And &Hff
					Dim b2 As Integer = b(boff + 5) And &Hff
					Dim b3 As Integer = b(boff + 4) And &Hff
					Dim b4 As Integer = b(boff + 3)
					Dim b5 As Integer = b(boff + 2) And &Hff
					Dim b6 As Integer = b(boff + 1) And &Hff
					Dim b7 As Integer = b(boff) And &Hff

					Dim i0 As Integer = (b0 << 24) Or (b1 << 16) Or (b2 << 8) Or b3
					Dim i1 As Integer = (b4 << 24) Or (b5 << 16) Or (b6 << 8) Or b7

					l([off] + j) = (CLng(i0) << 32) Or (i1 And &HffffffffL)
					boff += 8
				Next j
			End If
		End Sub

		Private Sub toFloats(ByVal b As SByte(), ByVal f As Single(), ByVal [off] As Integer, ByVal len As Integer)
			Dim boff As Integer = 0
			If byteOrder Is java.nio.ByteOrder.BIG_ENDIAN Then
				For j As Integer = 0 To len - 1
					Dim b0 As Integer = b(boff)
					Dim b1 As Integer = b(boff + 1) And &Hff
					Dim b2 As Integer = b(boff + 2) And &Hff
					Dim b3 As Integer = b(boff + 3) And &Hff
					Dim i As Integer = (b0 << 24) Or (b1 << 16) Or (b2 << 8) Or b3
					f([off] + j) = Single.intBitsToFloat(i)
					boff += 4
				Next j
			Else
				For j As Integer = 0 To len - 1
					Dim b0 As Integer = b(boff + 3)
					Dim b1 As Integer = b(boff + 2) And &Hff
					Dim b2 As Integer = b(boff + 1) And &Hff
					Dim b3 As Integer = b(boff + 0) And &Hff
					Dim i As Integer = (b0 << 24) Or (b1 << 16) Or (b2 << 8) Or b3
					f([off] + j) = Single.intBitsToFloat(i)
					boff += 4
				Next j
			End If
		End Sub

		Private Sub toDoubles(ByVal b As SByte(), ByVal d As Double(), ByVal [off] As Integer, ByVal len As Integer)
			Dim boff As Integer = 0
			If byteOrder Is java.nio.ByteOrder.BIG_ENDIAN Then
				For j As Integer = 0 To len - 1
					Dim b0 As Integer = b(boff)
					Dim b1 As Integer = b(boff + 1) And &Hff
					Dim b2 As Integer = b(boff + 2) And &Hff
					Dim b3 As Integer = b(boff + 3) And &Hff
					Dim b4 As Integer = b(boff + 4)
					Dim b5 As Integer = b(boff + 5) And &Hff
					Dim b6 As Integer = b(boff + 6) And &Hff
					Dim b7 As Integer = b(boff + 7) And &Hff

					Dim i0 As Integer = (b0 << 24) Or (b1 << 16) Or (b2 << 8) Or b3
					Dim i1 As Integer = (b4 << 24) Or (b5 << 16) Or (b6 << 8) Or b7
					Dim l As Long = (CLng(i0) << 32) Or (i1 And &HffffffffL)

					d([off] + j) = Double.longBitsToDouble(l)
					boff += 8
				Next j
			Else
				For j As Integer = 0 To len - 1
					Dim b0 As Integer = b(boff + 7)
					Dim b1 As Integer = b(boff + 6) And &Hff
					Dim b2 As Integer = b(boff + 5) And &Hff
					Dim b3 As Integer = b(boff + 4) And &Hff
					Dim b4 As Integer = b(boff + 3)
					Dim b5 As Integer = b(boff + 2) And &Hff
					Dim b6 As Integer = b(boff + 1) And &Hff
					Dim b7 As Integer = b(boff) And &Hff

					Dim i0 As Integer = (b0 << 24) Or (b1 << 16) Or (b2 << 8) Or b3
					Dim i1 As Integer = (b4 << 24) Or (b5 << 16) Or (b6 << 8) Or b7
					Dim l As Long = (CLng(i0) << 32) Or (i1 And &HffffffffL)

					d([off] + j) = Double.longBitsToDouble(l)
					boff += 8
				Next j
			End If
		End Sub

		Public Overridable Property streamPosition As Long Implements ImageInputStream.getStreamPosition
			Get
				checkClosed()
				Return streamPos
			End Get
		End Property

		Public Overridable Property bitOffset As Integer Implements ImageInputStream.getBitOffset
			Get
				checkClosed()
				Return bitOffset
			End Get
			Set(ByVal bitOffset As Integer)
				checkClosed()
				If bitOffset < 0 OrElse bitOffset > 7 Then Throw New System.ArgumentException("bitOffset must be betwwen 0 and 7!")
				Me.bitOffset = bitOffset
			End Set
		End Property


		Public Overridable Function readBit() As Integer Implements ImageInputStream.readBit
			checkClosed()

			' Compute final bit offset before we call read() and seek()
			Dim newBitOffset As Integer = (Me.bitOffset + 1) And &H7

			Dim val As Integer = read()
			If val = -1 Then Throw New java.io.EOFException

			If newBitOffset <> 0 Then
				' Move byte position back if in the middle of a byte
				seek(streamPosition - 1)
				' Shift the bit to be read to the rightmost position
				val >>= 8 - newBitOffset
			End If
			Me.bitOffset = newBitOffset

			Return val And &H1
		End Function

		Public Overridable Function readBits(ByVal numBits As Integer) As Long Implements ImageInputStream.readBits
			checkClosed()

			If numBits < 0 OrElse numBits > 64 Then Throw New System.ArgumentException
			If numBits = 0 Then Return 0L

			' Have to read additional bits on the left equal to the bit offset
			Dim bitsToRead As Integer = numBits + bitOffset

			' Compute final bit offset before we call read() and seek()
			Dim newBitOffset As Integer = (Me.bitOffset + numBits) And &H7

			' Read a byte at a time, accumulate
			Dim accum As Long = 0L
			Do While bitsToRead > 0
				Dim val As Integer = read()
				If val = -1 Then Throw New java.io.EOFException

				accum <<= 8
				accum = accum Or val
				bitsToRead -= 8
			Loop

			' Move byte position back if in the middle of a byte
			If newBitOffset <> 0 Then seek(streamPosition - 1)
			Me.bitOffset = newBitOffset

			' Shift away unwanted bits on the right.
			accum >>>= (-bitsToRead) ' Negative of bitsToRead == extra bits read

			' Mask out unwanted bits on the left
			accum = accum And (-CInt(CUInt(1L) >> (64 - numBits)))

			Return accum
		End Function

		''' <summary>
		''' Returns <code>-1L</code> to indicate that the stream has unknown
		''' length.  Subclasses must override this method to provide actual
		''' length information.
		''' </summary>
		''' <returns> -1L to indicate unknown length. </returns>
		Public Overridable Function length() As Long Implements ImageInputStream.length
			Return -1L
		End Function

		''' <summary>
		''' Advances the current stream position by calling
		''' <code>seek(getStreamPosition() + n)</code>.
		''' 
		''' <p> The bit offset is reset to zero.
		''' </summary>
		''' <param name="n"> the number of bytes to seek forward.
		''' </param>
		''' <returns> an <code>int</code> representing the number of bytes
		''' skipped.
		''' </returns>
		''' <exception cref="IOException"> if <code>getStreamPosition</code>
		''' throws an <code>IOException</code> when computing either
		''' the starting or ending position. </exception>
		Public Overridable Function skipBytes(ByVal n As Integer) As Integer Implements ImageInputStream.skipBytes
			Dim pos As Long = streamPosition
			seek(pos + n)
			Return CInt(streamPosition - pos)
		End Function

		''' <summary>
		''' Advances the current stream position by calling
		''' <code>seek(getStreamPosition() + n)</code>.
		''' 
		''' <p> The bit offset is reset to zero.
		''' </summary>
		''' <param name="n"> the number of bytes to seek forward.
		''' </param>
		''' <returns> a <code>long</code> representing the number of bytes
		''' skipped.
		''' </returns>
		''' <exception cref="IOException"> if <code>getStreamPosition</code>
		''' throws an <code>IOException</code> when computing either
		''' the starting or ending position. </exception>
		Public Overridable Function skipBytes(ByVal n As Long) As Long Implements ImageInputStream.skipBytes
			Dim pos As Long = streamPosition
			seek(pos + n)
			Return streamPosition - pos
		End Function

		Public Overridable Sub seek(ByVal pos As Long) Implements ImageInputStream.seek
			checkClosed()

			' This test also covers pos < 0
			If pos < flushedPos Then Throw New System.IndexOutOfRangeException("pos < flushedPos!")

			Me.streamPos = pos
			Me.bitOffset = 0
		End Sub

		''' <summary>
		''' Pushes the current stream position onto a stack of marked
		''' positions.
		''' </summary>
		Public Overridable Sub mark() Implements ImageInputStream.mark
			Try
				markByteStack.Push(Convert.ToInt64(streamPosition))
				markBitStack.Push(Convert.ToInt32(bitOffset))
			Catch e As java.io.IOException
			End Try
		End Sub

		''' <summary>
		''' Resets the current stream byte and bit positions from the stack
		''' of marked positions.
		''' 
		''' <p> An <code>IOException</code> will be thrown if the previous
		''' marked position lies in the discarded portion of the stream.
		''' </summary>
		''' <exception cref="IOException"> if an I/O error occurs. </exception>
		Public Overridable Sub reset() Implements ImageInputStream.reset
			If markByteStack.Count = 0 Then Return

			Dim pos As Long = CLng(Fix(markByteStack.Pop()))
			If pos < flushedPos Then Throw New javax.imageio.IIOException("Previous marked position has been discarded!")
			seek(pos)

			Dim offset As Integer = CInt(Fix(markBitStack.Pop()))
			bitOffset = offset
		End Sub

		Public Overridable Sub flushBefore(ByVal pos As Long) Implements ImageInputStream.flushBefore
			checkClosed()
			If pos < flushedPos Then Throw New System.IndexOutOfRangeException("pos < flushedPos!")
			If pos > streamPosition Then Throw New System.IndexOutOfRangeException("pos > getStreamPosition()!")
			' Invariant: flushedPos >= 0
			flushedPos = pos
		End Sub

		Public Overridable Sub flush() Implements ImageInputStream.flush
			flushBefore(streamPosition)
		End Sub

		Public Overridable Property flushedPosition As Long Implements ImageInputStream.getFlushedPosition
			Get
				Return flushedPos
			End Get
		End Property

		''' <summary>
		''' Default implementation returns false.  Subclasses should
		''' override this if they cache data.
		''' </summary>
		Public Overridable Property cached As Boolean Implements ImageInputStream.isCached
			Get
				Return False
			End Get
		End Property

		''' <summary>
		''' Default implementation returns false.  Subclasses should
		''' override this if they cache data in main memory.
		''' </summary>
		Public Overridable Property cachedMemory As Boolean Implements ImageInputStream.isCachedMemory
			Get
				Return False
			End Get
		End Property

		''' <summary>
		''' Default implementation returns false.  Subclasses should
		''' override this if they cache data in a temporary file.
		''' </summary>
		Public Overridable Property cachedFile As Boolean Implements ImageInputStream.isCachedFile
			Get
				Return False
			End Get
		End Property

		Public Overridable Sub close() Implements ImageInputStream.close
			checkClosed()

			isClosed = True
		End Sub

		''' <summary>
		''' Finalizes this object prior to garbage collection.  The
		''' <code>close</code> method is called to close any open input
		''' source.  This method should not be called from application
		''' code.
		''' </summary>
		''' <exception cref="Throwable"> if an error occurs during superclass
		''' finalization. </exception>
		Protected Overrides Sub Finalize()
			If Not isClosed Then
				Try
					close()
				Catch e As java.io.IOException
				End Try
			End If
			MyBase.Finalize()
		End Sub
	End Class

End Namespace