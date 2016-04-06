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
	''' A class that can be used to compute the Adler-32 checksum of a data
	''' stream. An Adler-32 checksum is almost as reliable as a CRC-32 but
	''' can be computed much faster.
	''' 
	''' <p> Passing a {@code null} argument to a method in this class will cause
	''' a <seealso cref="NullPointerException"/> to be thrown.
	''' </summary>
	''' <seealso cref=         Checksum
	''' @author      David Connelly </seealso>
	Public Class Adler32
		Implements Checksum

		Private adler As Integer = 1

		''' <summary>
		''' Creates a new Adler32 object.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Updates the checksum with the specified byte (the low eight
		''' bits of the argument b).
		''' </summary>
		''' <param name="b"> the byte to update the checksum with </param>
		Public Overridable Sub update(  b As Integer) Implements Checksum.update
			adler = update(adler, b)
		End Sub

		''' <summary>
		''' Updates the checksum with the specified array of bytes.
		''' </summary>
		''' <exception cref="ArrayIndexOutOfBoundsException">
		'''          if {@code off} is negative, or {@code len} is negative,
		'''          or {@code off+len} is greater than the length of the
		'''          array {@code b} </exception>
		Public Overridable Sub update(  b As SByte(),   [off] As Integer,   len As Integer) Implements Checksum.update
			If b Is Nothing Then Throw New NullPointerException
			If [off] < 0 OrElse len < 0 OrElse [off] > b.Length - len Then Throw New ArrayIndexOutOfBoundsException
			adler = updateBytes(adler, b, [off], len)
		End Sub

		''' <summary>
		''' Updates the checksum with the specified array of bytes.
		''' </summary>
		''' <param name="b"> the byte array to update the checksum with </param>
		Public Overridable Sub update(  b As SByte())
			adler = updateBytes(adler, b, 0, b.Length)
		End Sub


		''' <summary>
		''' Updates the checksum with the bytes from the specified buffer.
		''' 
		''' The checksum is updated using
		''' buffer.<seealso cref="java.nio.Buffer#remaining() remaining()"/>
		''' bytes starting at
		''' buffer.<seealso cref="java.nio.Buffer#position() position()"/>
		''' Upon return, the buffer's position will be updated to its
		''' limit; its limit will not have been changed.
		''' </summary>
		''' <param name="buffer"> the ByteBuffer to update the checksum with
		''' @since 1.8 </param>
		Public Overridable Sub update(  buffer As java.nio.ByteBuffer)
			Dim pos As Integer = buffer.position()
			Dim limit As Integer = buffer.limit()
			assert(pos <= limit)
			Dim [rem] As Integer = limit - pos
			If [rem] <= 0 Then Return
			If TypeOf buffer Is sun.nio.ch.DirectBuffer Then
				adler = updateByteBuffer(adler, CType(buffer, sun.nio.ch.DirectBuffer).address(), pos, [rem])
			ElseIf buffer.hasArray() Then
				adler = updateBytes(adler, buffer.array(), pos + buffer.arrayOffset(), [rem])
			Else
				Dim b As SByte() = New SByte([rem] - 1){}
				buffer.get(b)
				adler = updateBytes(adler, b, 0, b.Length)
			End If
			buffer.position(limit)
		End Sub

		''' <summary>
		''' Resets the checksum to initial value.
		''' </summary>
		Public Overridable Sub reset() Implements Checksum.reset
			adler = 1
		End Sub

		''' <summary>
		''' Returns the checksum value.
		''' </summary>
		Public Overridable Property value As Long Implements Checksum.getValue
			Get
				Return CLng(adler) And &HffffffffL
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function update(  adler As Integer,   b As Integer) As Integer
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function updateBytes(  adler As Integer,   b As SByte(),   [off] As Integer,   len As Integer) As Integer
		End Function
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Function updateByteBuffer(  adler As Integer,   addr As Long,   [off] As Integer,   len As Integer) As Integer
		End Function
	End Class

End Namespace