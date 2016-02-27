'
' * Copyright (c) 1996, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' An input stream that also maintains a checksum of the data being read.
	''' The checksum can then be used to verify the integrity of the input data.
	''' </summary>
	''' <seealso cref=         Checksum
	''' @author      David Connelly </seealso>
	Public Class CheckedInputStream
		Inherits java.io.FilterInputStream

		Private cksum As Checksum

		''' <summary>
		''' Creates an input stream using the specified Checksum. </summary>
		''' <param name="in"> the input stream </param>
		''' <param name="cksum"> the Checksum </param>
		Public Sub New(ByVal [in] As java.io.InputStream, ByVal cksum As Checksum)
			MyBase.New([in])
			Me.cksum = cksum
		End Sub

		''' <summary>
		''' Reads a java.lang.[Byte]. Will block if no input is available. </summary>
		''' <returns> the byte read, or -1 if the end of the stream is reached. </returns>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		Public Overrides Function read() As Integer
			Dim b As Integer = [in].read()
			If b <> -1 Then cksum.update(b)
			Return b
		End Function

		''' <summary>
		''' Reads into an array of bytes. If <code>len</code> is not zero, the method
		''' blocks until some input is available; otherwise, no
		''' bytes are read and <code>0</code> is returned. </summary>
		''' <param name="buf"> the buffer into which the data is read </param>
		''' <param name="off"> the start offset in the destination array <code>b</code> </param>
		''' <param name="len"> the maximum number of bytes read </param>
		''' <returns>    the actual number of bytes read, or -1 if the end
		'''            of the stream is reached. </returns>
		''' <exception cref="NullPointerException"> If <code>buf</code> is <code>null</code>. </exception>
		''' <exception cref="IndexOutOfBoundsException"> If <code>off</code> is negative,
		''' <code>len</code> is negative, or <code>len</code> is greater than
		''' <code>buf.length - off</code> </exception>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		Public Overrides Function read(ByVal buf As SByte(), ByVal [off] As Integer, ByVal len As Integer) As Integer
			len = [in].read(buf, [off], len)
			If len <> -1 Then cksum.update(buf, [off], len)
			Return len
		End Function

		''' <summary>
		''' Skips specified number of bytes of input. </summary>
		''' <param name="n"> the number of bytes to skip </param>
		''' <returns> the actual number of bytes skipped </returns>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		Public Overrides Function skip(ByVal n As Long) As Long
			Dim buf As SByte() = New SByte(511){}
			Dim total As Long = 0
			Do While total < n
				Dim len As Long = n - total
				len = read(buf, 0,If(len < buf.Length, CInt(len), buf.Length))
				If len = -1 Then Return total
				total += len
			Loop
			Return total
		End Function

		''' <summary>
		''' Returns the Checksum for this input stream. </summary>
		''' <returns> the Checksum value </returns>
		Public Overridable Property checksum As Checksum
			Get
				Return cksum
			End Get
		End Property
	End Class

End Namespace