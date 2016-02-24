'
' * Copyright (c) 1996, 1999, Oracle and/or its affiliates. All rights reserved.
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
	''' An output stream that also maintains a checksum of the data being
	''' written. The checksum can then be used to verify the integrity of
	''' the output data.
	''' </summary>
	''' <seealso cref=         Checksum
	''' @author      David Connelly </seealso>
	Public Class CheckedOutputStream
		Inherits java.io.FilterOutputStream

		Private cksum As Checksum

		''' <summary>
		''' Creates an output stream with the specified Checksum. </summary>
		''' <param name="out"> the output stream </param>
		''' <param name="cksum"> the checksum </param>
		Public Sub New(ByVal out As java.io.OutputStream, ByVal cksum As Checksum)
			MyBase.New(out)
			Me.cksum = cksum
		End Sub

		''' <summary>
		''' Writes a byte. Will block until the byte is actually written. </summary>
		''' <param name="b"> the byte to be written </param>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		Public Overrides Sub write(ByVal b As Integer)
			out.write(b)
			cksum.update(b)
		End Sub

		''' <summary>
		''' Writes an array of bytes. Will block until the bytes are
		''' actually written. </summary>
		''' <param name="b"> the data to be written </param>
		''' <param name="off"> the start offset of the data </param>
		''' <param name="len"> the number of bytes to be written </param>
		''' <exception cref="IOException"> if an I/O error has occurred </exception>
		Public Overrides Sub write(ByVal b As SByte(), ByVal [off] As Integer, ByVal len As Integer)
			out.write(b, [off], len)
			cksum.update(b, [off], len)
		End Sub

		''' <summary>
		''' Returns the Checksum for this output stream. </summary>
		''' <returns> the Checksum </returns>
		Public Overridable Property checksum As Checksum
			Get
				Return cksum
			End Get
		End Property
	End Class

End Namespace