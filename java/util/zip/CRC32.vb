Imports System.Runtime.InteropServices

'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.util.zip


	''' <summary>
	''' A class that can be used to compute the CRC-32 of a data stream.
	''' 
	''' <p> Passing a {@code null} argument to a method in this class will cause
	''' a <seealso cref="NullPointerException"/> to be thrown.
	''' </summary>
	''' <seealso cref=         Checksum
	''' @author      David Connelly </seealso>
	Public Class CRC32
		Implements Checksum

		Private crc As Integer

		''' <summary>
		''' Creates a new CRC32 object.
		''' </summary>
		Public Sub New()
		End Sub


		''' <summary>
		''' Updates the CRC-32 checksum with the specified byte (the low
		''' eight bits of the argument b).
		''' </summary>
		''' <param name="b"> the byte to update the checksum with </param>
		Public Overridable Sub update(ByVal b As Integer) Implements Checksum.update
			crc = update(crc, b)
		End Sub

		''' <summary>
		''' Updates the CRC-32 checksum with the specified array of bytes.
		''' </summary>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''          if {@code off} is negative, or {@code len} is negative,
		'''          or {@code off+len} is greater than the length of the
		'''          array {@code b} </exception>
		Public Overridable Sub update(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer) Implements Checksum.update
			If b Is Nothing Then Throw New NullPointerException
			If [off] < 0 OrElse len < 0 OrElse [off] > b.Length - len Then Throw New ArrayIndexOutOfBoundsException
			crc = updateBytes(crc, b, [off], len)
		End Sub

		''' <summary>
		''' Updates the CRC-32 checksum with the specified array of bytes.
		''' </summary>
		''' <param name="b"> the array of bytes to update the checksum with </param>
		Public Overridable Sub update(ByVal b As SByte())
			crc = updateBytes(crc, b, 0, b.Length)
		End Sub

		''' <summary>
		''' Updates the checksum with the bytes from the specified buffer.
		''' 
		''' The checksum is updated using
		''' buffer.<seealso cref="java.nio.Buffer#remaining() remaining()"/>
		''' bytes starting at
		''' buffer.<seealso cref="java.nio.Buffer#position() position()"/>
		''' Upon return, the buffer's position will
		''' be updated to its limit; its limit will not have been changed.
		''' </summary>
		''' <param name="buffer"> the ByteBuffer to update the checksum with
		''' @since 1.8 </param>
		Public Overridable Sub update(ByVal buffer As java.nio.ByteBuffer)
			Dim pos As Integer = buffer.position()
			Dim limit As Integer = buffer.limit()
			assert(pos <= limit)
			Dim [rem] As Integer = limit - pos
			If [rem] <= 0 Then Return
			If TypeOf buffer Is sun.nio.ch.DirectBuffer Then
				crc = updateByteBuffer(crc, CType(buffer, sun.nio.ch.DirectBuffer).address(), pos, [rem])
			ElseIf buffer.hasArray() Then
				crc = updateBytes(crc, buffer.array(), pos + buffer.arrayOffset(), [rem])
			Else
				Dim b As SByte() = New SByte([rem] - 1){}
				buffer.get(b)
				crc = updateBytes(crc, b, 0, b.Length)
			End If
			buffer.position(limit)
		End Sub

		''' <summary>
		''' Resets CRC-32 to initial value.
		''' </summary>
		Public Overridable Sub reset() Implements Checksum.reset
			crc = 0
		End Sub

		''' <summary>
		''' Returns CRC-32 value.
		''' </summary>
		Public Overridable Property value As Long Implements Checksum.getValue
			Get
				Return CLng(crc) And &HffffffffL
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function update(ByVal crc As Integer, ByVal b As Integer) As Integer
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function updateBytes(ByVal crc As Integer, ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer) As Integer
		End Function

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function updateByteBuffer(ByVal adler As Integer, ByVal addr As Long, ByVal [off] As Integer, ByVal len As Integer) As Integer
		End Function
	End Class

End Namespace